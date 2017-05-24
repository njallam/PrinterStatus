using System.Collections.Generic;
using Printer_Status.Helpers;
using SnmpSharpNet;
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 1591

namespace Printer_Status.Printers
{
    /// <summary>
    /// An object representation of a printer's input tray.
    /// </summary>

    public struct Input
    {
        /// <summary>
        /// Initialises an Input instance from a dictionary representation of an SNMP row.
        /// </summary>
        /// <param name="results">A dictionary representation of an SNMP row.</param>
        public Input(Dictionary<string, AsnType> results)
        {
            InputType = (InputType) results["Type"].ToInt();
            DimUnit = (MediaUnit)results["DimUnit"].ToInt();
            CapacityUnit = (CapacityUnit)results["CapacityUnit"].ToInt();
            MaxCapacity = results["MaxCapacity"].ToInt();
            CurrentLevel = results["CurrentLevel"].ToInt();
            MediaName = results["MediaName"].ToString();
            Name = results["Name"].ToString();
            VendorName = results["VendorName"].ToString();
            Model = results["Model"].ToString();
            Version = results["Version"].ToString();
            SerialNumber = results["SerialNumber"].ToString();
            Description = results["Description"].ToString();
            Security = (PresentOnOff)results["Security"].ToInt();
            MediaWeight = results["MediaWeight"].ToInt();
            MediaType = results["MediaType"].ToString();
            MediaColor = results["MediaColor"].ToString();
            MediaFormParts = results["MediaFormParts"].ToInt();
            MediaLoadTimeout = results["MediaLoadTimeout"].ToInt();
            NextIndex = results["NextIndex"].ToInt();

            Percent = ValueHelper.LevelToPercent(MaxCapacity, CurrentLevel);
        }

        public InputType InputType { get; }
        public MediaUnit DimUnit { get; }
        public CapacityUnit CapacityUnit { get; }
        public int MaxCapacity { get; }
        public int CurrentLevel { get; }
        public string MediaName { get; }
        public string Name { get; }
        public string VendorName { get; }
        public string Model { get; }
        public string Version { get; }
        public string SerialNumber { get; }
        public string Description { get; }
        public PresentOnOff Security { get; }
        public int MediaWeight { get; }
        public string MediaType { get; }
        public string MediaColor { get; }
        public int MediaFormParts { get; }
        public int MediaLoadTimeout { get; }
        public int NextIndex { get; }

        public string Percent { get; }
    }

    public enum InputType
    {
        other = 1,
        unknown = 2,
        sheetFeedAutoRemovableTray = 3,
        sheetFeedAutoNonRemovableTray = 4,
        sheetFeedManual = 5,
        continuousRoll = 6,
        continuousFanFold = 7
    }
}