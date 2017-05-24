using System.Collections.Generic;
using SnmpSharpNet;
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 1591

namespace Printer_Status.Printers
{
    /// <summary>
    /// An object representation of a printer's cover.
    /// </summary>
    public struct Cover
    {
        /// <summary>
        /// Initialises a Cover instance from a dictionary representation of an SNMP row.
        /// </summary>
        /// <param name="results">A dictionary representation of an SNMP row.</param>
        public Cover(Dictionary<string, AsnType> results)
        {
            Description = results["Description"].ToString();
            Status = (CoverStatus)results["Status"].ToInt();
        }
        public string Description { get; private set; }
        public CoverStatus Status { get; private set; }
    }

    public enum CoverStatus
    {
        other = 1,
        unknown = 2,
        coverOpen = 3,
        coverClosed = 4,
        interlockOpen = 5,
        interlockClosed = 6
    }
}