using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Controls;
using Printer_Status.Helpers;
using Printer_Status.Printers;
using Printer_Status.Properties;

namespace Printer_Status
{
    public partial class MainWindow : Window
    {
        #region Properties
        /// <summary>
        /// The list of printers with alerts.
        /// </summary>
        private ObservableCollection<Tuple<Alert, Printer>> AlertsPrinters { get; }
        /// <summary>
        /// The list of printers with low supplies.
        /// </summary>
        private ObservableCollection<Tuple<Supply, Printer>> SuppliesPrinters { get; }
        /// <summary>
        /// The list of printers which are offline.
        /// </summary>
        private ObservableDictionary<string,Tuple<string,string,string>> OfflinePrinters { get; }
        #endregion
        #region Constructor
        /// <summary>
        /// Initialises a new instance of the MainWindow Window.
        /// </summary>
        public MainWindow()
        {
            //Create the 'Watched' database in the table if it does not already exist.
            using (SQLiteConnection connection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn()
                )
                new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS Watched (IP VARCHAR(15), Name TINYTEXT, Location TINYTEXT, LastSeen DATETIME)",
                    connection).ExecuteNonQuery();

            //Instantiate the lists to be displayed on this window
            AlertsPrinters = new ObservableCollection<Tuple<Alert, Printer>>();
            SuppliesPrinters = new ObservableCollection<Tuple<Supply, Printer>>();
            OfflinePrinters = new ObservableDictionary<string, Tuple<string, string, string>>();

            //Initialise components on this window 
            InitializeComponent();
            //Set the tray icon to the application's icon
            TaskbarIcon.Icon = Properties.Resources.appicon;

            //Set the item sources for the views to their relevant list.
            AlertsView.ItemsSource = AlertsPrinters;
            SuppliesView.ItemsSource = SuppliesPrinters;
            UnreachableView.ItemsSource = OfflinePrinters;

            //Update the lists
            UpdateLists();

            //Configure a timer to update the lists every 5 seconds.
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Start();
        }
        #endregion
        #region Main Methods
        /// <summary>
        /// Update the lists when the timer ticks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e) => UpdateLists();

        /// <summary>
        /// Update the alerts, low-supply and offline lists for all printers in the database.
        /// </summary>
        private async void UpdateLists()
        {
            //Retrieve a list of the printers in the database.
            Dictionary<string, Tuple<string, string, string>> databasePrinters = await DatabaseHelper.GetDatabasePrintersAsync();
            //Repeat for every printer in the list
            foreach (KeyValuePair<string,Tuple<string,string,string>> kvp in databasePrinters)
            {
                //Create a printer object for this printer
                Printer printer = new Printer(IPAddress.Parse(kvp.Key));
                //If printer information cannot be retrieved
                if (!await Task.Run(() => printer.FetchSystemInfo()))
                {
                    //If the printer is not already in the offline list
                    if (!OfflinePrinters.ContainsKey(kvp.Key))
                    {
                        //Add the printer to the offline list
                        OfflinePrinters[kvp.Key] = Tuple.Create(kvp.Value.Item1, kvp.Value.Item2, kvp.Value.Item3);
                        //AlertsPrinters.RemoveAll(ap => ap.Item2.IpAddress.ToString() == kvp.Key);
                        //Display a notification to indicate the printer is offline.
                        DisplayNotification($"{kvp.Value.Item1} unreachable", $"{kvp.Value.Item1} in {kvp.Value.Item2} has been unreachable since {kvp.Value.Item3}", BalloonIcon.Error);
                    }
                    //Skip to the next printer in the list
                    continue;
                }
                //Remove the printer from the offline list (as it now must be online)
                OfflinePrinters.Remove(kvp.Key);
                //Update the printer's information in the database (in case the name, etc. has chanced)
                await printer.UpdateDatabase();

                //Get an array of alerts from the printer where the alert has one of the severity levels in the configuration file (default 'critical and 'warning').
                //School printers seem to mark 'powerUp' alert codes as warnings, despite being of no concern to anyone, these have been filtered out.
                Alert[] alerts =
                    printer.Alerts.Select(alert => alert.Value)
                        .Where(
                            alert =>
                                Settings.Default.AlertSeverities.Cast<string>()
                                    .Select(s => (AlertSeverityLevel) int.Parse(s))
                                    .Contains(alert.SeverityLevel) && alert.Code != AlertCode.powerUp).ToArray();
                //Repeat for every alert in the list
                foreach (Alert alert in alerts)
                {
                    var existingAlertPrinter = AlertsPrinters.FirstOrDefault(ap => ap.Item2.IpAddress.Equals(printer.IpAddress) && ap.Item1.Equals(alert));
                    //If the list of alerts does not already contain this alert for this printer
                    if (existingAlertPrinter == null)
                    {
                        //Add this alert to the list of alerts for all printers.
                        AlertsPrinters.Add(new Tuple<Alert, Printer>(alert, printer));
                        //Display a notification to indicate the printer has this alert.
                        DisplayNotification(printer.Name, $"{printer} has alert {alert}", BalloonIcon.Warning);
                    }
                    else
                    {
                        //Update the printer object in the list.
                        AlertsPrinters[AlertsPrinters.IndexOf(existingAlertPrinter)] = new Tuple<Alert, Printer>(alert, printer);
                    }
                }
                //Remove any alerts which no longer exist for this printer.
                AlertsPrinters.RemoveAll(ap => ap.Item2.HasSameIP(printer) && !alerts.Contains(ap.Item1));
                //Get an array of supplies from this printer which indicate that they are low.
                Supply[] supplies =
                    printer.Supplies.Select(supply => supply.Value)
                        .Where(supply => supply.IsLow)
                        .ToArray();
                //Repeat for every supply in the list
                foreach (Supply supply in supplies)
                {
                    var existingSupplyPrinter = SuppliesPrinters.FirstOrDefault(ap => ap.Item2.IpAddress.Equals(printer.IpAddress) && ap.Item1.Equals(supply));
                    //If the list of supply shortages does not already contain this supply for this printer.
                    if (!SuppliesPrinters.Any(sp => sp.Item2.HasSameIP(printer) && sp.Item1.Equals(supply)))
                    {
                        SuppliesPrinters.Add(new Tuple<Supply, Printer>(supply, printer));
                    }
                    else
                    {
                        //Update the printer object in the list.
                        SuppliesPrinters[SuppliesPrinters.IndexOf(existingSupplyPrinter)] = new Tuple<Supply, Printer>(supply, printer);
                    }
                }
                //Remove any "low supplies" which no longer exist for this printer.
                SuppliesPrinters.RemoveAll(sp => sp.Item2.HasSameIP(printer) && !supplies.Contains(sp.Item1));
            }

            //Remove any printers from both lists which are no longer to be monitored.
            AlertsPrinters.RemoveAll(ap => !databasePrinters.ContainsKey(ap.Item2.IpAddress.ToString()));
            SuppliesPrinters.RemoveAll(sp => !databasePrinters.ContainsKey(sp.Item2.IpAddress.ToString()));
        }
        #endregion
        #region Utility Methods
        /// <summary>
        /// Display a notification to the user, while flashing the window in the taskbar if it is not focused.
        /// </summary>
        /// <param name="title">The title for the notification.</param>
        /// <param name="message">The message body for the notification.</param>
        /// <param name="icon">The icon for the notification.</param>
        private void DisplayNotification(string title, string message, BalloonIcon icon)
        {
            //Use a background thread to show a notification.
            Task.Run(() => TaskbarIcon.ShowBalloonTip(title, message, icon));
            //If the window is minimised, make sure it is visible in the taskbar.
            if (WindowState == WindowState.Minimized) ShowInTaskbar = true;
            //Flash this window until it receives focus.
            FlashWindow.Flash(this);
        }

        /// <summary>
        /// Toggle whether or not the window is minimised.
        /// </summary>
        private void ToggleMinimise()
        {
            //Temporary variable for whether or not the window is minimised.
            //True if WindowState is 'Minimized' otherwise false.
            bool minimised = WindowState == WindowState.Minimized;
            //If minimised, restore the window, otherwise minimise it.
            WindowState = minimised ? WindowState.Normal : WindowState.Minimized;
            //Show in the taskbar only if was minimised (and now not minimised)
            ShowInTaskbar = minimised;
        }
        #endregion
        #region Event Handlers
        #region Button
        /// <summary>
        /// Open a DiscoverWindow when the button for it is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiscoverButton_Click(object sender, RoutedEventArgs e) => WindowHelper.ShowOrFocus<DiscoverWindow>();

        /// <summary>
        /// Open a particular printer's detail window via a dialog when the Inspect button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InspectButton_Click(object sender, RoutedEventArgs e)
        {
            //Create a new IPEntryDialog
            IPEntryDialog ipEntryDialog = new IPEntryDialog();
            if (ipEntryDialog.ShowDialog() == true && ipEntryDialog.DialogResult == true)
            {
                //If the dialog gave a result (printer), open the window for that printer.
                WindowHelper.ShowOrFocus(ipEntryDialog.DialogResultValue);
            }
        }
        /// <summary>
        /// Exit the application when the exit button in the window or tray icon button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton_OnClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown(0);

        /// <summary>
        /// Open an AllPrintersWindow when its button on this window is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllPrintersButton_OnClick(object sender, RoutedEventArgs e) => WindowHelper.ShowOrFocus<AllPrintersWindow>();

        /// <summary>
        /// Toggle whether or not the window is minimised when the tray icon is double-clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskbarIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e) => ToggleMinimise();
        #endregion
        #region View
        /// <summary>
        /// Resize the columns in the view to fit the content.
        /// </summary>
        /// <param name="sender">The view which may need to be resized.</param>
        /// <param name="e"></param>
        private void View_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //For each column in the view where the width is less than the width of its containing content.
            foreach (GridViewColumn column in ((GridView)((ListView)sender).View).Columns.Where(column => column.Width < column.ActualWidth))
            {
                //Set the column's width to the width of its containing content
                column.Width = column.ActualWidth;
            }
        }
        /// <summary>
        /// Open the printer corresponding to a list item when it is double-clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView listView = (ListView)sender;
            //Do nothing if there is no item selected.
            if (listView.SelectedIndex == -1) return;
            //Create a printer object from the selected item.
            //Handle cases where first item is tuple is either Alert or Supply (does not matter)
            Printer printer = null;
            if (listView.SelectedItem is Tuple<Alert, Printer>)
                printer = ((Tuple<Alert, Printer>)listView.SelectedItem).Item2;
            else if (listView.SelectedItem is Tuple<Supply, Printer>)
                printer = ((Tuple<Supply, Printer>)listView.SelectedItem).Item2;
            //Show the detail window for the printer if determined above.
            if (printer != null) WindowHelper.ShowOrFocus(printer);
        }
        #endregion
        #region Event Interception
        /// <summary>
        /// Stop the window from being closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The closing event to be cancelled.</param>
        /// <remarks>This window should only be closed by the application being completely shutdown.</remarks>
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            //Stop the window from being closed
            e.Cancel = true;
            //Minimise the window instead.
            ToggleMinimise();
        }
        #endregion
        #endregion
    }
}
