using System.Collections.Generic;
using SnmpSharpNet;
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 1591

namespace Printer_Status.Printers
{
    /// <summary>
    /// An object representation of a printer's alert.
    /// </summary>
    public struct Alert
    {
        /// <summary>
        /// Initialises an Alert instance from a dictionary representation of an SNMP row.
        /// </summary>
        /// <param name="results">A dictionary representation of an SNMP row.</param>
        public Alert(Dictionary<string, AsnType> results)
        {
            SeverityLevel = (AlertSeverityLevel)results["SeverityLevel"].ToInt();
            TrainingLevel = (AlertTrainingLevel)results["TrainingLevel"].ToInt();
            Group = (AlertGroup)results["Group"].ToInt();
            GroupIndex = results["GroupIndex"].ToInt();
            Location = results["Location"].ToInt();
            Code = (AlertCode)results["Code"].ToInt();
            Description = results["Description"].ToString();
        }
        public override string ToString()
        {
            return $"({SeverityLevel} - {TrainingLevel}): {Code} '{Description}' in {Group}.{GroupIndex}";
            //return $"{SeverityLevel}, {TrainingLevel}: {Group}.{GroupIndex} {Code} - {Description}";
        }
        public AlertSeverityLevel SeverityLevel { get; }
        public AlertTrainingLevel TrainingLevel { get; }
        public AlertGroup Group { get; }
        public int GroupIndex { get; }
        public int Location { get; }
        public AlertCode Code { get; }
        public string Description { get; }
    }

    public enum AlertSeverityLevel
    {
        other = 1,
        critical = 3,
        warning = 4,
        warningBinaryChangeEvent = 5
    }
    public enum AlertTrainingLevel
    {
        other = 1,
        unknown = 2,
        untrained = 3,
        trained = 4,
        fieldService = 5,
        management = 6,
        noInterventionRequired = 7
    }
    public enum AlertGroup
    {
        other = 1,
        unknown = 2,
        hostResourcesMIBStorageTable = 3,
        hostResourcesMIBDeviceTable = 4,
        generalPrinter = 5,
        cover = 6,
        localization = 7,
        input = 8,
        output = 9,
        marker = 10,
        markerSupplies = 11,
        markerColorant = 12,
        mediaPath = 13,
        channel = 14,
        interpreter = 15,
        consoleDisplayBuffer = 16,
        consoleLights = 17,
        alert = 18,
        finDevice = 30,
        finSupply = 31,
        finSupplyMediaInput = 32,
        finAttribute = 33
    }
    public enum AlertCode
    {
        other = 1,
        unknown = 2,
        coverOpen = 3,
        coverClosed = 4,
        interlockOpen = 5,
        interlockClosed = 6,
        configurationChange = 7,
        jam = 8,
        subunitMissing = 9,
        subunitLifeAlmostOver = 10,
        subunitLifeOver = 11,
        subunitAlmostEmpty = 12,
        subunitEmpty = 13,
        subunitAlmostFull = 14,
        subunitFull = 15,
        subunitNearLimit = 16,
        subunitAtLimit = 17,
        subunitOpened = 18,
        subunitClosed = 19,
        subunitTurnedOn = 20,
        subunitTurnedOff = 21,
        subunitOffline = 22,
        subunitPowerSaver = 23,
        subunitWarmingUp = 24,
        subunitAdded = 25,
        subunitRemoved = 26,
        subunitResourceAdded = 27,
        subunitResourceRemoved = 28,
        subunitRecoverableFailure = 29,
        subunitUnrecoverableFailure = 30,
        subunitRecoverableStorageError = 31,
        subunitUnrecoverableStorageError = 32,
        subunitMotorFailure = 33,
        subunitMemoryExhausted = 34,
        subunitUnderTemperature = 35,
        subunitOverTemperature = 36,
        subunitTimingFailure = 37,
        subunitThermistorFailure = 38,
        doorOpen = 501,
        doorClosed = 502,
        powerUp = 503,
        powerDown = 504,
        printerNMSReset = 505,
        printerManualReset = 506,
        printerReadyToPrint = 507,
        inputMediaTrayMissing = 801,
        inputMediaSizeChange = 802,
        inputMediaWeightChange = 803,
        inputMediaTypeChange = 804,
        inputMediaColorChange = 805,
        inputMediaFormPartsChange = 806,
        inputMediaSupplyLow = 807,
        inputMediaSupplyEmpty = 808,
        inputMediaChangeRequest = 809,
        inputManualInputRequest = 810,
        inputTrayPositionFailure = 811,
        inputTrayElevationFailure = 812,
        inputCannotFeedSizeSelected = 813,
        outputMediaTrayMissing = 901,
        outputMediaTrayAlmostFull = 902,
        outputMediaTrayFull = 903,
        outputMailboxSelectFailure = 904,
        markerFuserUnderTemperature = 1001,
        markerFuserOverTemperature = 1002,
        markerFuserTimingFailure = 1003,
        markerFuserThermistorFailure = 1004,
        markerAdjustingPrintQuality = 1005,
        markerTonerEmpty = 1101,
        markerInkEmpty = 1102,
        markerPrintRibbonEmpty = 1103,
        markerTonerAlmostEmpty = 1104,
        markerInkAlmostEmpty = 1105,
        markerPrintRibbonAlmostEmpty = 1106,
        markerWasteTonerReceptacleAlmostFull = 1107,
        markerWasteInkReceptacleAlmostFull = 1108,
        markerWasteTonerReceptacleFull = 1109,
        markerWasteInkReceptacleFull = 1110,
        markerOpcLifeAlmostOver = 1111,
        markerOpcLifeOver = 1112,
        markerDeveloperAlmostEmpty = 1113,
        markerDeveloperEmpty = 1114,
        markerTonerCartridgeMissing = 1115,
        mediaPathMediaTrayMissing = 1301,
        mediaPathMediaTrayAlmostFull = 1302,
        mediaPathMediaTrayFull = 1303,
        mediaPathCannotDuplexMediaSelected = 1304,
        interpreterMemoryIncrease = 1501,
        interpreterMemoryDecrease = 1502,
        interpreterCartridgeAdded = 1503,
        interpreterCartridgeDeleted = 1504,
        interpreterResourceAdded = 1505,
        interpreterResourceDeleted = 1506,
        interpreterResourceUnavailable = 1507,
        interpreterComplexPageEncountered = 1509,
        alertRemovalOfBinaryChangeEntry = 180
    }
}