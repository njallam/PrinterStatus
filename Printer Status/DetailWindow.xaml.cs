using SnmpSharpNet;
using System.Threading.Tasks;
using System.Windows;
using Printer_Status.Helpers;
using Printer_Status.Printers;

namespace Printer_Status
{
    public partial class DetailWindow : Window
    {
        #region Fields
        /// <summary>
        /// The printer this window is showing information about.
        /// </summary>
        private readonly Printer _printer;
        #endregion
        #region Constructors
        /// <summary>
        /// Initialises a new instance of the DetailWindow Window.
        /// </summary>
        /// <param name="printer">The printer to show information about.</param>
        public DetailWindow(Printer printer)
        {
            _printer = printer;
            DataContext = _printer;
            InitializeComponent();

            UpdateUI();
        }
        #endregion
        #region Main Methods
        /// <summary>
        /// Update UI elements to the current state of the printer
        /// </summary>
        private async void UpdateUI()
        {
            //Query the printer for the latest information
            await Task.Run(() => _printer.FetchSystemInfo());
            //Change the monitor button text to reflect the current monitoring status
            MonitorButton.Content = _printer.Monitored ? "Unmonitor" : "Monitor";
        }
        #endregion
        #region Event Handlers
        /// <summary>
        /// Save textbox values to the printer when the button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemInfoSave_Click(object sender, RoutedEventArgs e)
        {
            Pdu pdu = new Pdu(PduType.Set);
            pdu.VbList.Add(new Oid(Oids.SysContact), new OctetString(ContactBox.Text));
            pdu.VbList.Add(new Oid(Oids.SysName), new OctetString(NameBox.Text));
            pdu.VbList.Add(new Oid(Oids.SysLocation), new OctetString(LocationBox.Text));
            _printer.SetValues(pdu);
            UpdateUI();
        }

        /// <summary>
        /// Update the UI elements to the current state of the printer when the button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshInfo_Click(object sender, RoutedEventArgs e) => UpdateUI();

        /// <summary>
        /// Mark/Unmark the printer as monitored when the button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MonitorButton_Click(object sender, RoutedEventArgs e)
        {
            await _printer.Monitor();
            UpdateUI();
        }
        #endregion
    }
}
