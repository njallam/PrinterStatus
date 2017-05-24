using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Data;
using Printer_Status.Helpers;
using Printer_Status.Properties;
using PropertyChanged;
using SnmpSharpNet;

namespace Printer_Status.Printers
{
    /// <summary>
    /// An object representation of a printer.
    /// </summary>
    [ImplementPropertyChanged]
    public class Printer
    {
        /// <summary>
        /// The IP address of the printer.
        /// </summary>
        public IPAddress IpAddress { get; }
        /// <summary>
        /// The time since the printer was last re-initialised. (1.3.6.1.2.1.1.3 - sysUpTime)
        /// </summary>
        public TimeTicks Uptime { get; set; }
        /// <summary>
        /// The physical location of this printer. (1.3.6.1.2.1.1.6 - sysLocation)
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// A manufacturer-provided description of the printer hardware (1.3.6.1.2.1.1.1 - sysDescr)
        /// </summary>
        public string Descr { get; set; }
        /// <summary>
        /// An assigned contact person for this printer (1.3.6.1.2.1.1.4 - sysContact)
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// An assigned name for this printer (1.3.6.1.2.1.1.5 - sysName)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The time the properties on this printer object were last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }
        /// <summary>
        /// Whether or not the printer is monitored.
        /// </summary>
        public bool Monitored { get; private set; }

        /// <summary>
        /// A dictionary for supplies, keyed by row.
        /// </summary>
        public ObservableDictionary<int, Supply> Supplies => _supplies;
        private ObservableDictionary<int, Supply> _supplies;
        private readonly object _suppliesLock = new object();
        /// <summary>
        /// A dictionary for alerts, keyed by row.
        /// </summary>
        public ObservableDictionary<int, Alert> Alerts => _alerts;
        private ObservableDictionary<int, Alert> _alerts;
        private readonly object _alertsLock = new object();
        /// <summary>
        /// A dictionary for covers, keyed by row.
        /// </summary>
        public ObservableDictionary<int, Cover> Covers => _covers;
        private ObservableDictionary<int, Cover> _covers; 
        private readonly object _coversLock = new object();

        /// <summary>
        /// The text on the display of the printer.
        /// </summary>
        public string ConsoleText { get; private set; }
        /// <summary>
        /// A dictionary for console lights, keyed by row.
        /// </summary>
        public ObservableDictionary<int, ConsoleLight> ConsoleLights => _consoleLights;
        private ObservableDictionary<int, ConsoleLight> _consoleLights; 
        private readonly object _consoleLightsLock = new object();
        /// <summary>
        /// A dictionary for inputs, keyed by row.
        /// </summary>
        public ObservableDictionary<int, Input> Inputs => _inputs;
        private ObservableDictionary<int, Input> _inputs; 
        private readonly object _inputsLock = new object();
        /// <summary>
        /// A dictionary for outputs, keyed by row.
        /// </summary>
        public ObservableDictionary<int, Output> Outputs => _outputs;
        private ObservableDictionary<int, Output> _outputs; 
        private readonly object _outputsLock = new object();

        /// <summary>
        /// A flag variable to indicate if the printer object is currently being updated.
        /// </summary>
        private bool _refreshing;

        /// <summary>
        /// Creates a SimpleSnmp object for querying this printer (with settings specified in the config file).
        /// </summary>
        /// <returns>A SimpleSnmp object for this printer.</returns>
        private SimpleSnmp GetQuerySnmp() => new SimpleSnmp(IpAddress.ToString(), SnmpHelper.Port, Settings.Default.SNMPCommunity, Settings.Default.QueryTimeout, Settings.Default.QueryRetries);
        /// <summary>
        /// Creates a SimpleSnmp object for discovering this printer (with settings specified in the config file).
        /// </summary>
        /// <returns>A SimpleSnmp object for this printer.</returns>
        private SimpleSnmp GetDiscoverySnmp() => new SimpleSnmp(IpAddress.ToString(), SnmpHelper.Port, Settings.Default.SNMPCommunity, Settings.Default.DiscoveryTimeout, Settings.Default.DiscoveryRetries);

        /// <summary>
        /// Initialises a new instance of a Printer.
        /// </summary>
        /// <param name="ipAddress">The IP address for this printer.</param>
        public Printer(IPAddress ipAddress)
        {
            //Store the IP address given on this object.
            IpAddress = ipAddress;
            //Initialise the dictionaries.
            _supplies = new ObservableDictionary<int, Supply>();
            _alerts = new ObservableDictionary<int, Alert>();
            _covers = new ObservableDictionary<int, Cover>();
            _consoleLights = new ObservableDictionary<int, ConsoleLight>();
            _inputs = new ObservableDictionary<int, Input>();
            _outputs = new ObservableDictionary<int, Output>();
            //Allow the dictionaries to be updated by different threads using lock objects.
            BindingOperations.EnableCollectionSynchronization(Supplies, _suppliesLock);
            BindingOperations.EnableCollectionSynchronization(Alerts, _alertsLock);
            BindingOperations.EnableCollectionSynchronization(Covers, _coversLock);
            BindingOperations.EnableCollectionSynchronization(ConsoleLights, _consoleLightsLock);
            BindingOperations.EnableCollectionSynchronization(Inputs, _inputsLock);
            BindingOperations.EnableCollectionSynchronization(Outputs, _outputsLock);

        }

        /// <summary>
        /// Fetch all information about this printer, updating properties.
        /// </summary>
        /// <returns>Whether the operation succeeded.</returns>
        public bool FetchSystemInfo()
        {
            //Return if the printer is already being updated
            if (_refreshing) return false;
            //Indicate that the printer is being updated
            _refreshing = true;
            //Query the system OIDs for this printer to get basic information.
            Dictionary<Oid, AsnType> result = GetQuerySnmp().Get(SnmpVersion.Ver1, Oids.Syss);
            if (result != null)
            {
                try
                {
                    //Update the relevant properties with the result of the above query.
                    Name = result[new Oid(Oids.SysName)].ToString();
                    Descr = result[new Oid(Oids.SysDescr)].ToString();
                    Uptime = (TimeTicks)result[new Oid(Oids.SysUpTime)];
                    Location = result[new Oid(Oids.SysLocation)].ToString();
                    Contact = result[new Oid(Oids.SysContact)].ToString();

                    //Update dictionaries using defined functions.
                    FetchAlertInfo();
                    FetchSupplyInfo();
                    FetchCoverInfo();
                    FetchInputInfo();
                    FetchOutputInfo();
                    FetchConsoleInfo();
                }
                catch (Exception) { _refreshing = false; return false; } //Return false if an exception is thrown (due to the IP address not actually belonging to a printer)
                //Indicate that the printer was last updated now
                LastUpdated = DateTime.Now;
                _refreshing = false;
                //Return true as all operations succeeded.
                return true;
            }
            //Return false as the printer did not reply.
            _refreshing = false;
            return false;
        }
        /// <summary>
        /// Update the database entry for this printer.
        /// </summary>
        /// <remarks>Can specify the operation to be performed (Insert, Delete or Update).</remarks>
        /// <param name="op">The operation to perform for this printer.</param>
        /// <returns></returns>
        public async Task UpdateDatabase(DatabaseOperation op = DatabaseOperation.Update)
        {
            using (SQLiteConnection connection = new SQLiteConnection(Settings.Default.ConnectionString))
            {
                string sql;
                //Select an appropriate SQL query depending on the operation to be performed.
                switch (op)
                {
                    case DatabaseOperation.Insert:
                        sql = "INSERT INTO `Watched` (IP, Name, Location, LastSeen) VALUES (@ipaddress,@name,@location,@lastseen)";
                        break;
                    case DatabaseOperation.Delete:
                        sql = "DELETE FROM `Watched` WHERE IP=@ipaddress";
                        break;
                    default:
                    //case DatabaseOperation.Update:
                        sql = "UPDATE `Watched` SET Name=@name, Location=@location, LastSeen=@lastseen WHERE IP=@ipaddress";
                        break;
                }
                //Open database connection
                await connection.OpenAsync();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    //Set parameter placeholders (above) to their actual value
                    command.Parameters.AddWithValue("@name", Name);
                    command.Parameters.AddWithValue("@location", Location);
                    command.Parameters.AddWithValue("@lastseen", LastUpdated.ToString("o"));
                    command.Parameters.AddWithValue("@ipaddress", IpAddress);
                    //Execute SQL query on database
                    //Printer is being monitored if a row is affected and the operation was not a delete operation.
                    Monitored = (await command.ExecuteNonQueryAsync() == 1 && op != DatabaseOperation.Delete);
                }
            }
        }
        /// <summary>
        /// Monitor/Unmonitor a printer.
        /// </summary>
        /// <param name="monitor">Whether or not the printer should be monitored.</param>
        /// <returns>A task so that this function can be awaited.</returns>
        public async Task Monitor(bool? monitor = null)
        {
            //If monitor parameter not specified, then do the opposite of what is currently true.
            //Monitor the printer if unmonitored; Unmonitor the printer if monitored.
            if (monitor == null) monitor = !Monitored;
            //Perform an insert operation if the printer is to be monitored, otherwise a delete operation.
            await UpdateDatabase((bool)monitor ? DatabaseOperation.Insert : DatabaseOperation.Delete);
        }
        private void FetchCoverInfo() => UpdateDictionary(ref _covers, Oids.PrtCoverEntry, Oids.PrtCovers);

        private void FetchConsoleInfo()
        {
            UpdateDictionary(ref _consoleLights, Oids.PrtConsoleLightEntry, Oids.PrtConsoleLights);
            //Update the ConsoleText property with the current console buffer text on the printer.
            ConsoleText = GetQuerySnmp().Get(Oids.PrtConsoleDisplayBufferText).ToString();
        }

        private void FetchInputInfo() => UpdateDictionary(ref _inputs, Oids.PrtInputEntry, Oids.PrtInputs);

        private void FetchOutputInfo() => UpdateDictionary(ref _outputs, Oids.PrtOutputEntry, Oids.PrtOutputs);

        private void FetchAlertInfo() => UpdateDictionary(ref _alerts, Oids.PrtAlertEntry, Oids.PrtAlerts);

        private void FetchSupplyInfo() => UpdateDictionary(ref _supplies,Oids.PrtMarkerSuppliesEntry,Oids.PrtMarkerSupplies);

        /// <summary>
        /// Update a dictionary from an SNMP table.
        /// </summary>
        /// <typeparam name="TVal">The type of object which is a row in this table.</typeparam>
        /// <param name="dictionary">The dictionary which is to be updated.</param>
        /// <param name="oidEntry">The OID for the table entries.</param>
        /// <param name="oidColumns">The OIDs for the table's columns.</param>
        private void UpdateDictionary<TVal>(ref ObservableDictionary<int, TVal> dictionary, string oidEntry, Dictionary<string,string> oidColumns)
        {
            //Get a dictionary object representing the snmp table for these OIDs.
            Dictionary<int, Dictionary<string, AsnType>> table = FetchTable(oidEntry, oidColumns);
            if (table == null) return;
            foreach (KeyValuePair<int, Dictionary<string, AsnType>> kvp in table)
            {
                dictionary[kvp.Key] = (TVal)Activator.CreateInstance(typeof (TVal), kvp.Value);
            }
            dictionary.RemoveAll(kvp => !table.ContainsKey(kvp.Key));
        }
        /// <summary>
        /// Return a table relating to an <paramref name="entryOid"/> with the columns <paramref name="oidColumns"/>.
        /// </summary>
        /// <param name="entryOid">The OID for the table's entries.</param>
        /// <param name="oidColumns">The OIDs for the table's columns.</param>
        /// <returns>A dictionary, keyed by row number, with a dictionary representation of each row.</returns>
        private Dictionary<int, Dictionary<string, AsnType>> FetchTable(string entryOid, Dictionary<string,string> oidColumns)
        {
            //Walk (query all child OIDs) on the given root OID
            Dictionary<Oid, AsnType> result = GetQuerySnmp().Walk(SnmpVersion.Ver1, entryOid);
            //If there is no reply, return nothing.
            if (result == null) return null;
            //Retrieve the child OIDs (rows) for the table.
            string[] suffixes = SnmpHelper.GetSuffixes(result, entryOid);
            //Create a new dictionary to store the generated dictionary representation of the table.
            Dictionary<int, Dictionary<string, AsnType>> table = new Dictionary<int, Dictionary<string, AsnType>>();
            foreach (string suffix in suffixes)
            {
                //Transform the column dictionary into a dictionary representation of the current row.
                Dictionary<string, AsnType> suffixResults = oidColumns.Select(kvp =>
                {
                    //An OID comprised of the column OID and the current suffix.
                    Oid oid = new Oid(kvp.Value + suffix);
                    //Transform the value of the key-value pair to be the result of the SNMP walk if one was given, otherwise an empty string.
                    return new KeyValuePair<string, AsnType>(kvp.Key, result.ContainsKey(oid) ? result[oid] : new OctetString(string.Empty));
                }).ToDictionary(x => x.Key, x => x.Value); //Convert the IEnumerable to a Dictionary again.
                //Add this representation to the table created above.
                table.Add(int.Parse(suffix),suffixResults);
            }
            return table;
        }
        /// <summary>
        /// Query the printer for basic system information.
        /// </summary>
        /// <remarks>Sets the Name and Location properties.</remarks>
        /// <returns>Whether or not the query succeeded.</returns>
        public bool FetchBasicInfo()
        {
            try
            {
                Dictionary<Oid, AsnType> result = GetDiscoverySnmp().Get(SnmpVersion.Ver1, Oids.Syss);
                if (result != null)
                {
                    Name = result[new Oid(Oids.SysName)].ToString();
                    Location = result[new Oid(Oids.SysLocation)].ToString();
                    return true;
                }
            }
            catch (TaskCanceledException) { return false; }
            return false;
        }
        /// <summary>
        /// Set values in the printer's SNMP table based upon a PDU.
        /// </summary>
        /// <param name="pdu">The protocol data unit containing the values to be changed.</param>
        public void SetValues(Pdu pdu) => GetQuerySnmp().Set(SnmpVersion.Ver1, pdu);
        /// <summary>
        /// Returns a string representing the printer.
        /// </summary>
        /// <returns>A string that represents the printer.</returns>
        public override string ToString() => $"{Name} in {Location} ({IpAddress})";
        /// <summary>
        /// Determine if another printer object has the same IP address (the printer objects can be considered the same).
        /// </summary>
        /// <param name="printer">The other printer object.</param>
        /// <returns>True if the printers have the same IP address.</returns>
        public bool HasSameIP(Printer printer) => printer.IpAddress.Equals(IpAddress);
    }

    #region Utility Enumerations
    /// <summary>
    /// A database operation
    /// </summary>
    public enum DatabaseOperation
    {
        /// <summary>
        /// An INSERT operation
        /// </summary>
        Insert,
        /// <summary>
        /// An UPDATE operation
        /// </summary>
        Update,
        /// <summary>
        /// A DELETE operation
        /// </summary>
        Delete
    }
    #endregion
    #region Shared Enumerations
    #pragma warning disable 1591
    public enum MediaUnit
    {
        tenThousandthsOfInches = 3,
        micrometers = 4
    }

    public enum CapacityUnit {
        other=1,
        unknown=2, 
        tenThousandthsOfInches= 3, 
        micrometers = 4, 
        sheets= 8, 
        feet=16, 
        meters=17, 
        items=18, 
        percent = 19 
    }

    public enum PresentOnOff
    {
        other = 1,
        on = 3,
        off = 4,
        notPresent = 5
    }
    #pragma warning restore 1591
    #endregion

}
