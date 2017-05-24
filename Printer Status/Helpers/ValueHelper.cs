using System;
using System.Net;
using Printer_Status.Properties;

namespace Printer_Status.Helpers
{
    /// <summary>
    /// A helper class for handling values from the user and printer
    /// </summary>
    public static class ValueHelper
    {
        /// <summary>
        /// Convert raw capacity values into a more easily understandable string.
        /// </summary>
        /// <remarks>If negative values for <paramref name="level"/> are given, the associated status message is shown.
        /// Otherwise if negative values for <paramref name="maxCapacity"/> are given, the value is shown.
        /// Otherwise, a percentage is calculated with the two values</remarks>
        /// <param name="maxCapacity">The returned maximum capacity value.</param>
        /// <param name="level">The returned level value.</param>
        /// <returns>A percentage, otherwise a status message.</returns>
        public static string LevelToPercent(int maxCapacity, int level)
        {
            //If level has a special value, return its message.
            switch (level)
            {
                case -1:
                    return "other";
                case -2:
                    return "unknown";
                case -3:
                    return "OK";
            }
            //If level has an unknown special value, return unknown.
            if (level < 0) return "unknown";
            //If maxCapacity has a special value, return level.
            if (maxCapacity <= 0) return level.ToString();
            //Otherwise return percentage, rounded to nearest integer
            //float cast ensures integer division does not take place.
            return Math.Round((float)level / maxCapacity * 100) + "%";
        }

        /// <summary>
        /// Determine if a subunit is low based upon its <paramref name="level"/>, <paramref name="maxCapacity"/> and the low percentage in the settings file.
        /// </summary>
        /// <param name="maxCapacity">The returned maximum capacity value.</param>
        /// <param name="level">The returned level value.</param>
        /// <returns>Whether or not the subunit is low.</returns>
        public static bool IsLow(int maxCapacity, int level)
        {
            //Cannot be determined if there is no max capacity or no level.
            if (maxCapacity <= 0) return false;
            if (level < 0) return false;
            //Is low if percentage capacity is less than or equal to LowValuePercentage.
            return (float) level / maxCapacity <= Settings.Default.LowValuePercentage;
        }
        /// <summary>
        /// Determines whether a string is a valid IP address.
        /// </summary>
        /// <param name="ipString">The string to validate.</param>
        /// <param name="address">The IPAddress version of <paramref name="ipString"/>.</param>
        /// <returns>If <paramref name="ipString"/> is a valid IP address.</returns>
        public static bool TryIPAddress(string ipString, out IPAddress address)
        {
            if (IPAddress.TryParse(ipString, out address))
            {
                //Return true if the string representation of the IP address matches the original string.
                //Otherwise return false
                return address.ToString() == ipString;
            }
            //Return false if the string could not be converted to an IP address.
            return false;
        }
    }
}