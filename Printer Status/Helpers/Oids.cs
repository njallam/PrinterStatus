using System.Collections.Generic;

namespace Printer_Status.Helpers
{
    /// <summary>
    /// Helper class for object identifiers.
    /// </summary>
    internal static class Oids
    {
        #region System Information Oids
        public static string SysDescr = "1.3.6.1.2.1.1.1.0";
        public static string SysUpTime = "1.3.6.1.2.1.1.3.0";
        public static string SysContact = "1.3.6.1.2.1.1.4.0";
        public static string SysName = "1.3.6.1.2.1.1.5.0";
        public static string SysLocation = "1.3.6.1.2.1.1.6.0";

        public static string[] Syss = { SysDescr, SysUpTime, SysContact, SysName, SysLocation };

        //public static Dictionary<string, string> syss = new Dictionary<string, string>
        //{
        //    {"sysDescr", "1.3.6.1.2.1.1.1.0"},
        //    {"sysUpTime", "1.3.6.1.2.1.1.3.0"},
        //    {"sysContact", "1.3.6.1.2.1.1.4.0"},
        //    {"sysName", "1.3.6.1.2.1.1.5.0"},
        //    {"sysLocation", "1.3.6.1.2.1.1.6.0"}
        //};
        #endregion
        #region Marker Oids
        public static string PrtMarkerSuppliesEntry = "1.3.6.1.2.1.43.11.1.1";
        public static Dictionary<string, string> PrtMarkerSupplies = new Dictionary<string, string>
        {
            {"Class", "1.3.6.1.2.1.43.11.1.1.4.1."},
            {"Type", "1.3.6.1.2.1.43.11.1.1.5.1."},
            {"Description", "1.3.6.1.2.1.43.11.1.1.6.1."},
            {"SupplyUnit", "1.3.6.1.2.1.43.11.1.1.7.1."},
            {"MaxCapacity", "1.3.6.1.2.1.43.11.1.1.8.1."},
            {"Level", "1.3.6.1.2.1.43.11.1.1.9.1."}
        };

        public static string PrtMarkerEntry = "1.3.6.1.2.1.43.10.2.1";
        public static Dictionary<string, string> PrtMarkers = new Dictionary<string, string>
        {
            {"MarkTech", "1.3.6.1.2.1.43.10.2.1.2.1."},
            {"CounterUnit", "1.3.6.1.2.1.43.10.2.1.3.1."},
            {"LifeCount", "1.3.6.1.2.1.43.10.2.1.4.1."},
            {"PowerOnCount", "1.3.6.1.2.1.43.10.2.1.5.1."},
            {"ProcessColorants", "1.3.6.1.2.1.43.10.2.1.6.1."},
            {"SpotColorants", "1.3.6.1.2.1.43.10.2.1.7.1."},
            {"Status", "1.3.6.1.2.1.43.10.2.1.15.1."}

        };

        public static string PrtMarkerColorantEntry = "1.3.6.1.2.1.43.12.1.1";
        public static Dictionary<string, string> PrtMarketColorants = new Dictionary<string, string>
        {
            {"MarkerIndex", "1.3.6.1.2.1.43.12.1.1.2.1."},
            {"Role", "1.3.6.1.2.1.43.12.1.1.3.1."},
            {"Value", "1.3.6.1.2.1.43.12.1.1.4.1."},
            {"Tonality", "1.3.6.1.2.1.43.12.1.1.5.1."}
        };
        #endregion
        #region Alerts Oids
        public static string PrtAlertEntry = "1.3.6.1.2.1.43.18.1.1";
        public static Dictionary<string, string> PrtAlerts = new Dictionary<string, string>
        {
            {"SeverityLevel", "1.3.6.1.2.1.43.18.1.1.2.1."},
            {"TrainingLevel", "1.3.6.1.2.1.43.18.1.1.3.1."},
            {"Group", "1.3.6.1.2.1.43.18.1.1.4.1."},
            {"GroupIndex", "1.3.6.1.2.1.43.18.1.1.5.1."},
            {"Location", "1.3.6.1.2.1.43.18.1.1.6.1."},
            {"Code", "1.3.6.1.2.1.43.18.1.1.7.1."},
            {"Description", "1.3.6.1.2.1.43.18.1.1.8.1."},
        };
        #endregion
        #region Covers Oids
        public static string PrtCoverEntry = "1.3.6.1.2.1.43.6.1.1";
        public static Dictionary<string, string> PrtCovers = new Dictionary<string, string>
        {
            {"Description", "1.3.6.1.2.1.43.6.1.1.2.1."},
            {"Status", "1.3.6.1.2.1.43.6.1.1.3.1."}
        };
        #endregion
        #region Console
        public static string PrtConsoleDisplayBufferText = "1.3.6.1.2.1.43.16.5.1.2.1.1";

        public static string PrtConsoleLightEntry = "1.3.6.1.2.1.43.17.6.1";
        public static Dictionary<string, string> PrtConsoleLights = new Dictionary<string, string>
        {
            { "OnTime", "1.3.6.1.2.1.43.17.6.1.2.1." },
            { "OffTime", "1.3.6.1.2.1.43.17.6.1.3.1."},
            { "Color", "1.3.6.1.2.1.43.17.6.1.4.1."},
            { "Description", "1.3.6.1.2.1.43.17.6.1.5.1." }
        };
        #endregion
        #region Input
        public static string PrtInputEntry = "1.3.6.1.2.1.43.8.2.1";
        public static Dictionary<string, string> PrtInputs = new Dictionary<string, string>
        {
            {"Type", "1.3.6.1.2.1.43.8.2.1.2.1."},
            {"DimUnit", "1.3.6.1.2.1.43.8.2.1.3.1."},
            {"CapacityUnit", "1.3.6.1.2.1.43.8.2.1.8.1."},
            {"MaxCapacity", "1.3.6.1.2.1.43.8.2.1.9.1."},
            {"CurrentLevel", "1.3.6.1.2.1.43.8.2.1.10.1."},
            {"Status", "1.3.6.1.2.1.43.8.2.1.11.1."},
            {"MediaName", "1.3.6.1.2.1.43.8.2.1.12.1."},
            {"Name", "1.3.6.1.2.1.43.8.2.1.13.1."},
            {"VendorName", "1.3.6.1.2.1.43.8.2.1.14.1."},
            {"Model", "1.3.6.1.2.1.43.8.2.1.15.1."},
            {"Version", "1.3.6.1.2.1.43.8.2.1.16.1."},
            {"SerialNumber", "1.3.6.1.2.1.43.8.2.1.17.1."},
            {"Description", "1.3.6.1.2.1.43.8.2.1.18.1."},
            {"Security", "1.3.6.1.2.1.43.8.2.1.19.1."},
            {"MediaWeight", "1.3.6.1.2.1.43.8.2.1.20.1."},
            {"MediaType", "1.3.6.1.2.1.43.8.2.1.21.1."},
            {"MediaColor", "1.3.6.1.2.1.43.8.2.1.22.1."},
            {"MediaFormParts", "1.3.6.1.2.1.43.8.2.1.23.1."},
            {"MediaLoadTimeout", "1.3.6.1.2.1.43.8.2.1.24.1."},
            {"NextIndex", "1.3.6.1.2.1.43.8.2.1.25.1."}
        };

        #endregion
        #region Output
        public static string PrtOutputEntry = "1.3.6.1.2.1.43.9.2.1";
        public static Dictionary<string, string> PrtOutputs = new Dictionary<string, string>
        {
            {"Type", "1.3.6.1.2.1.43.9.2.1.2.1."},
            {"CapacityUnit", "1.3.6.1.2.1.43.9.2.1.3.1."},
            {"MaxCapacity", "1.3.6.1.2.1.43.9.2.1.4.1."},
            {"RemainingCapacity", "1.3.6.1.2.1.43.9.2.1.5.1."},
            {"Status", "1.3.6.1.2.1.43.9.2.1.6.1."},
            {"Name", "1.3.6.1.2.1.43.9.2.1.7.1."},
            {"VendorName", "1.3.6.1.2.1.43.9.2.1.8.1."},
            {"Model", "1.3.6.1.2.1.43.9.2.1.9.1."},
            {"Version", "1.3.6.1.2.1.43.9.2.1.10.1."},
            {"SerialNumber", "1.3.6.1.2.1.43.9.2.1.11.1."},
            {"Description", "1.3.6.1.2.1.43.9.2.1.12.1."},
            {"Security", "1.3.6.1.2.1.43.9.2.1.13.1."},
            {"DimUnit", "1.3.6.1.2.1.43.9.2.1.14.1."},
            {"StackingOrder", "1.3.6.1.2.1.43.9.2.1.19.1."},
            {"PageDeliveryOrientation", "1.3.6.1.2.1.43.9.2.1.20.1."},
            {"Bursting", "1.3.6.1.2.1.43.9.2.1.21.1."},
            {"Decollating", "1.3.6.1.2.1.43.9.2.1.22.1."},
            {"PageCollated", "1.3.6.1.2.1.43.9.2.1.23.1."},
            {"OffsetStacking", "1.3.6.1.2.1.43.9.2.1.24.1."},
        };
        #endregion
    }
}
