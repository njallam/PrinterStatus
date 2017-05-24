using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Printer_Status.Helpers;
using Printer_Status.Printers;

namespace Printer_Status
{
    public partial class IPEntryDialog : Window
    {
        #region Properties
        /// <summary>
        /// The printer returned by this dialog box
        /// </summary>
        public Printer DialogResultValue { get; private set; }
        #endregion
        #region Constructor
        /// <summary>
        /// Initialises a new instance of the IPEntryDialog Window.
        /// </summary>
        public IPEntryDialog()
        {
            InitializeComponent();
        }
        #endregion
        #region Event Handlers
        /// <summary>
        /// Return a printer object to the calling window if the printer was able to be contacted when the button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GoButton_Click(object sender, RoutedEventArgs e)
        {
            GoButton.IsEnabled = false;
            GoButton.Content = "Checking";

            IPAddress ipAddress;
            if (ValueHelper.TryIPAddress(IpBox.Text, out ipAddress))
            {
                Printer printer = new Printer(ipAddress);
                if (await Task.Run(() => printer.FetchSystemInfo()))
                {
                    DialogResultValue = printer;
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Unable to reach printer.");
                }
            } else
            {
                MessageBox.Show("Invalid IP address.");
            }
            GoButton.Content = "Go";
            GoButton.IsEnabled = true;
        }
        #endregion
    }
}
