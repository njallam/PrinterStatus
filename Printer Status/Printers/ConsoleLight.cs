using System.Collections.Generic;
using System.Windows.Media;
using SnmpSharpNet;
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 1591

namespace Printer_Status.Printers
{
    /// <summary>
    /// An object representation of a printer's indicator light.
    /// </summary>
    public struct ConsoleLight
    {
        /// <summary>
        /// Initialises a ConsoleLight instance from a dictionary representation of an SNMP row.
        /// </summary>
        /// <param name="results">A dictionary representation of an SNMP row.</param>
        public ConsoleLight(Dictionary<string, AsnType> results)
        {
            OnTime = results["OnTime"].ToInt();
            OffTime = results["OffTime"].ToInt();
            ConsoleColor = (ConsoleColor)results["Color"].ToInt();
            Description = results["Description"].ToString();
        }

        public int OnTime { get; }
        public int OffTime { get; }
        public ConsoleColor ConsoleColor { get; }

        public Color RealColor {
            get
            {
                switch (ConsoleColor)
                {
                    case ConsoleColor.red:
                        return Color.FromRgb(255,0,0);
                    case ConsoleColor.green:
                        return Color.FromRgb(0,255,0);
                    case ConsoleColor.blue:
                        return Color.FromRgb(0,0,255);
                    case ConsoleColor.cyan:
                        return Color.FromRgb(0,255,255);
                    case ConsoleColor.magenta:
                        return Color.FromRgb(255, 0, 255);
                    case ConsoleColor.yellow:
                        return Color.FromRgb(255, 255, 0);
                    case ConsoleColor.orange:
                        return Color.FromRgb(255,128,0);
                    default:
                        return Color.FromRgb(255, 255, 255);
                }
            }
        }
        public string Description { get; }
    }

    public enum ConsoleColor
    {
        unknown = 2,
        white = 3,
        red = 4,
        green = 5,
        blue = 6,
        cyan = 7,
        magenta = 8,
        yellow = 9,
        orange = 10
    }
}