using System.Collections.Generic;
using Printer_Status.Helpers;
using SnmpSharpNet;
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 1591

namespace Printer_Status.Printers
{
    /// <summary>
    /// An object representation of a printer's output tray.
    /// </summary>
    public struct Output
    {
        /// <summary>
        /// Initialises an Output instance from a dictionary representation of an SNMP row.
        /// </summary>
        /// <param name="results">A dictionary representation of an SNMP row.</param>
        public Output(Dictionary<string, AsnType> results)
        {
            OutputType = (OutputType) results["Type"].ToInt();
            CapacityUnit = (CapacityUnit)results["CapacityUnit"].ToInt();
            MaxCapacity = results["MaxCapacity"].ToInt();
            RemainingCapacity = results["RemainingCapacity"].ToInt();
            Name = results["Name"].ToString();
            VendorName = results["VendorName"].ToString();
            Model = results["Model"].ToString();
            Version = results["Version"].ToString();
            SerialNumber = results["SerialNumber"].ToString();
            Description = results["Description"].ToString();
            Security = (PresentOnOff)results["Security"].ToInt();
            DimUnit = (MediaUnit)results["DimUnit"].ToInt();
            StackingOrder = (OutputStackingOrder)results["StackingOrder"].ToInt();
            PageDeliveryOrientation = (OutputPageDeliveryOrientation)results["PageDeliveryOrientation"].ToInt();
            Bursting = (PresentOnOff)results["Bursting"].ToInt();
            Decollating = (PresentOnOff)results["Decollating"].ToInt();
            PageCollated = (PresentOnOff)results["PageCollated"].ToInt();
            OffsetStacking = (PresentOnOff)results["OffsetStacking"].ToInt();

            Percent = ValueHelper.LevelToPercent(MaxCapacity, RemainingCapacity);
        }

        public OutputType OutputType { get; }
        public CapacityUnit CapacityUnit { get; }
        public int MaxCapacity { get; }
        public int RemainingCapacity { get; }
        public string Name { get; }
        public string VendorName { get; }
        public string Model { get; }
        public string Version { get; }
        public string SerialNumber { get; }
        public string Description { get; }
        public PresentOnOff Security { get; }
        public MediaUnit DimUnit { get; }
        public OutputStackingOrder StackingOrder { get; }
        public OutputPageDeliveryOrientation PageDeliveryOrientation { get; }
        public PresentOnOff Bursting { get; }
        public PresentOnOff Decollating { get; }
        public PresentOnOff PageCollated { get; }
        public PresentOnOff OffsetStacking { get; }

        public string Percent { get; }
    }

    public enum OutputType
    {
        other = 1,
        unknown = 2,
        removableBin = 3,
        unRemovableBin = 4,
        continuousRollDevice = 5,
        mailBox = 6,
        continuousFanFold = 7
    }
    public enum OutputStackingOrder
    {
        unknown = 2,
        firstToLast = 3,
        LastToFirst = 4
    }
    public enum OutputPageDeliveryOrientation
    {
        faceUp = 3,
        faceDown = 4
    }
}