using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Printer_Status.Helpers;
using Printer_Status.Printers;

namespace Printer_Status
{
    public partial class AllPrintersWindow : Window
    {
        #region Properties
        /// <summary>
        /// The collection of printers to be displayed in this window
        /// </summary>
        private ObservableDictionary<string, Tuple<string, string, string>> DatabasePrinters { get; }
        #endregion
        #region Constructors
        /// <summary>
        /// Initialises a new instance of the AllPrintersWindow Window.
        /// </summary>
        public AllPrintersWindow()
        {
            DatabasePrinters = new ObservableDictionary<string, Tuple<string, string, string>>();
            InitializeComponent();
            AllPrintersView.ItemsSource = DatabasePrinters;
            GetPrintersList();
        }
        #endregion
        #region Main Methods
        /// <summary>
        /// Updates the window's list of printers using the database.
        /// </summary>
        private async void GetPrintersList()
        {
            //Get the list of database printers using the helper function
            Dictionary<string, Tuple<string, string, string>> databasePrinters = await DatabaseHelper.GetDatabasePrintersAsync();
            //Remove all printers from the list which are not in the database
            DatabasePrinters.RemoveAll(kvp => !databasePrinters.ContainsKey(kvp.Key));
            //Add each printer from the database to the list (overwriting any old information)
            foreach (KeyValuePair<string, Tuple<string, string, string>> kvp in databasePrinters)
                DatabasePrinters[kvp.Key] = kvp.Value;
        }
        #endregion
        #region Event Handlers
        /// <summary>
        /// Open the printer corresponding to a list item when it is double-clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AllPrintersView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Do nothing if there is no item selected.
            if (AllPrintersView.SelectedIndex == -1) return;
            //Create a printer object from the selected item
            Printer printer = new Printer(IPAddress.Parse(((KeyValuePair<string, Tuple<string, string, string>>)AllPrintersView.SelectedItem).Key));
            //If the printer can be queried, open its detail window
            if (await Task.Run(() => printer.FetchSystemInfo()))
                WindowHelper.ShowOrFocus(printer);
            //Otherwise inform the user the printer is unreachable
            else
                MessageBox.Show("Could not reach printer.");
        }
        #endregion
    }
}
