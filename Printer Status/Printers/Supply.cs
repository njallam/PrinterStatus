using System.Collections.Generic;
using Printer_Status.Helpers;
using SnmpSharpNet;
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 1591

namespace Printer_Status.Printers
{
    /// <summary>
    /// An object representation of a printer's marker supply.
    /// </summary>
    public struct Supply
    {
        /// <summary>
        /// Initialises a Supply instance from a dictionary representation of an SNMP row.
        /// </summary>
        /// <param name="results">A dictionary representation of an SNMP row.</param>
        public Supply(Dictionary<string, AsnType> results)
        {
            SupplyClass = (SupplyClass)results["Class"].ToInt();
            SupplyType = (SupplyType)results["Type"].ToInt();
            Description = results["Description"].ToString();
            SupplyUnit = (SupplyUnit)results["SupplyUnit"].ToInt();
            MaxCapacity = results["MaxCapacity"].ToInt();
            Level = results["Level"].ToInt();
        }
        public override string ToString() => $"{SupplyType} {SupplyClass}: {Description}: {Percent} ({Level} / {MaxCapacity} {SupplyUnit})"; //TODO: Friendly string

        public string Description { get; }

        public SupplyClass SupplyClass { get; }
        public SupplyType SupplyType { get; }
        public SupplyUnit SupplyUnit { get; }
        public int MaxCapacity { get; }
        public int Level { get; }
        public string Percent => ValueHelper.LevelToPercent(MaxCapacity, Level);
        public bool IsLow => ValueHelper.IsLow(MaxCapacity, Level);
    }

    public enum SupplyClass
    {
        other = 1,
        supplyThatIsConsumed = 3,
        receptacleThatIsFilled = 4
    }
    public enum SupplyType
    {
        other = 1,
        unknown = 2,
        toner = 3,
        wasteToner = 4,
        ink = 5,
        inkCartridge = 6,
        inkRibbon = 7,
        wasteInk = 8,
        opc = 9,
        developer = 10,
        fuserOil = 11,
        solidWax = 12,
        ribbonWax = 13,
        wasteWax = 14,
        fuser = 15,
        coronaWire = 16,
        fuserOilWick = 17,
        cleanerUnit = 18,
        fuserCleaningPad = 19,
        transferUnit = 20,
        tonerCartridge = 21,
        fuserOiler = 22,
        water = 23,
        wasteWater = 24,
        glueWaterAdditive = 25,
        wastePaper = 26,
        bindingSupply = 27,
        bandingSupply = 28,
        stitchingWire = 29,
        shrinkWrap = 30,
        paperWrap = 31,
        staples = 32,
        inserts = 33,
        covers = 34
    }
    public enum SupplyUnit
    {
        other = 1,
        unknown = 2,
        tenThousandthsOfInches = 3,
        micrometers = 4,
        impressions = 7,
        sheets = 8,
        hours = 11,
        thousandthsOfOunces = 12,
        tenthsOfGrams = 13,
        hundrethsOfFluidOunces = 14,
        tenthsOfMilliliters = 15,
        feet = 16,
        meters = 17,
        items = 18,
        percent = 19
    }
}