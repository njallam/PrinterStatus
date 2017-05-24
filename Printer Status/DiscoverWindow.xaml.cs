using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Printer_Status.Helpers;
using Printer_Status.Printers;

namespace Printer_Status
{
    public partial class DiscoverWindow : Window
    {
        #region Properties

        /// <summary>
        /// The number of devices queried so far.
        /// </summary>
        private uint Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                DiscoverProgress.Value = value;
            }
        }
        /// <summary>
        /// The number of devices being queried.
        /// </summary>
        private uint Total
        {
            get { return _total; }
            set
            {
                _total = value;
                DiscoverProgress.Maximum = value;
            }
        }
        #endregion
        #region Fields
        /// <summary>
        /// The source of cancellation tokens for the discover process.
        /// </summary>
        private CancellationTokenSource _discoverCancellation;

        private uint _progress;
        private uint _total;
        #endregion
        #region Constructors
        /// <summary>
        /// Initialises a new instance of the DiscoverWindow Window.
        /// </summary>
        public DiscoverWindow()
        {
            InitializeComponent();
            //Status message show "Ready."
            StatusMessageBox.Text = "Ready.";
        }
        #endregion
        #region Main Methods
        /// <summary>
        /// Queries a network device for information by SNMP.  If the query succeeds, add the device to the listbox.
        /// </summary>
        /// <param name="ipAddress">The IP address of the network device to query.</param>
        private async void DiscoverPrinter(IPAddress ipAddress)
        {
            //Don't do anything if cancellation was requested (more efficient than try-catch)
            if (_discoverCancellation.IsCancellationRequested) return;
            //Create a printer object form this IP Address.
            Printer printer = new Printer(ipAddress);
            try
            {

                if (await Task.Run(() => printer.FetchBasicInfo(), _discoverCancellation.Token))
                {
                    //If this printer already exists in the listbox, get it.
                    Printer printerToRemove = DiscoveredBox.Items.Cast<Printer>().FirstOrDefault(prin => prin.IpAddress.Equals(printer.IpAddress));
                    //If a printer in the listbox was found, remove it.
                    if (printerToRemove != null) DiscoveredBox.Items.Remove(printerToRemove);
                    //Add the discovered printer to the listbox.
                    DiscoveredBox.Items.Add(printer);
                }
            }
            catch (TaskCanceledException) { }
            Progress++;

            if (Progress == Total)
            {
                ChangeUI(false);
            }
        }
        #endregion
        #region Event Handlers

        /// <summary>
        /// Start discovery when the button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiscoverButton_Click(object sender, RoutedEventArgs e)
        {
            //Variables for the start and end IP address
            IPAddress startAddress, endAddress;
            //List of validation errors
            List<string> errors = new List<string>();
            //If either IP address is invalid, add and error message to errors.
            if (!ValueHelper.TryIPAddress(StartAddressBox.Text, out startAddress)) errors.Add("Start IP invalid.");
            if (!ValueHelper.TryIPAddress(EndAddressBox.Text, out endAddress)) errors.Add("End IP invalid.");
            //If there are any errors, show them, separated by new lines, and return.
            if (errors.Any())
            {
                MessageBox.Show(string.Join("\n", errors));
                return;
            }
            //Get unsigned integer representations of the IP addresses
            uint startIpInt = startAddress.ToUInt();
            uint endIpInt = endAddress.ToUInt();
            if (endIpInt < startIpInt)
            //If the IP address range was invalid, show an error message and reutnr.
            {
                MessageBox.Show("IP address range less than zero.");
                return;
            }

            //Change the UI for discovering.
            ChangeUI(true);

            //If there is a cancellation token source, dispose of it.
            _discoverCancellation?.Dispose();
            //Create a new cancellation token source for this discovery.
            _discoverCancellation = new CancellationTokenSource();

            //Reset Progress
            Progress = 0;
            //Set Total to range
            Total = endIpInt - startIpInt;
            //Query all IP addresses in range.
            for (uint ipInt = startIpInt; ipInt <= endIpInt; ipInt++)
            {
                DiscoverPrinter(ipInt.ToIPAddress());
            }

        }

        /// <summary>
        /// Clear the listbox when the button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DiscoveredBox.Items.Clear();
        }

        /// <summary>
        /// Stop discovery when the button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StatusMessageBox.Text = "Stopping...";
            //Cancel the discovery process
            _discoverCancellation.Cancel();
            StopButton.Content = "Stopping";
            StopButton.IsEnabled = false;
        }

        /// <summary>
        /// Open the printer corresponding to a list item when it is double-clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiscoveredBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Do nothing if there is no item selected.
            if (DiscoveredBox.SelectedItem == null) return;

            //Create a printer object from the selected item
            Printer printer = (Printer)DiscoveredBox.SelectedItem;
            //Show the detail window for the printer.
            WindowHelper.ShowOrFocus(printer);
        }
        #endregion
        #region Utility Methods
        /// <summary>
        /// Update the UI elements based upon the current state of discovery.
        /// </summary>
        /// <param name="discovering">Whether or not discovery is taking place.</param>
        private void ChangeUI(bool discovering)
        {
            StopButton.Content = "Stop"; //Stop button should say "Stop".
            StopButton.IsEnabled = discovering; //Stop button enabled if discovering.
            StartAddressBox.IsEnabled = !discovering; //Textbox disabled if discovering
            EndAddressBox.IsEnabled = !discovering; //Textbox disabled if discovering.
            DiscoverButton.IsEnabled = !discovering; //Discover button disabled if discovering.
            //If discovering, show "Discovering..."
            //Otherwise, if stopped, show "Stopped.", otherwise show "Completed."
            StatusMessageBox.Text = discovering ? "Discovering..." : _discoverCancellation.IsCancellationRequested ? "Stopped." : "Completed.";
        }
        #endregion
    }
}
