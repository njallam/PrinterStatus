using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace Printer_Status
{
    /// <summary>
    /// A static class of extension methods for various classes used in this project.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Gets an unsigned integer representation of an IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address to be converted.</param>
        /// <returns>An unsigned integer representation of <paramref name="ipAddress"/></returns>
        public static uint ToUInt(this IPAddress ipAddress)
        {
            //Get the byte array representation of the IP address.
            //Reverse the order of the byte array as IP address bytes are stored big-endian (network order).
            byte[] bytes = ipAddress.GetAddressBytes().Reverse().ToArray();
            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// Gets an IP address representation of an unsigned integer.
        /// </summary>
        /// <param name="ipInt">The unsigned integer to be converted.</param>
        /// <returns>An IP address representation of <paramref name="ipInt"/></returns>
        public static IPAddress ToIPAddress(this uint ipInt)
        {
            var bytes = BitConverter.GetBytes(ipInt);
            Array.Reverse(bytes);
            return new IPAddress(bytes);
        }

        /// <summary>
        /// Convert an AsnType value into an integer.
        /// </summary>
        /// <param name="value">The AsnType value to be converted.</param>
        /// <returns>An integer representation of <paramref name="value"/>, otherwise '-2' indicating 'unknown'.</returns>
        public static int ToInt(this AsnType value)
        {
            int result;
            //Convert value to a string
            //Return the parsed integer if the string can be parsed as an integer, otherwise -2 (reasons above)
            return int.TryParse(value.ToString(), out result) ? result : -2;
        }

        /// <summary>
        /// Remove all items matching the specified condition.
        /// </summary>
        /// <param name="coll">The collection to remove items from.</param>
        /// <param name="condition">The condition to remove items based upon.</param>
        /// <typeparam name="T">The type of the collection.</typeparam>
        /// <returns>The number of items removed from the collection.</returns>
        public static int RemoveAll<T>(this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            var itemsToRemove = coll.Where(condition).ToList();
            foreach (var itemToRemove in itemsToRemove)
            {
                coll.Remove(itemToRemove);
            }
            return itemsToRemove.Count;
        }
        /// <summary>
        /// Remove all items matching the specified condition.
        /// </summary>
        /// <param name="dict">The dictionary to remove items from.</param>
        /// <param name="condition">The condition to remove items based upon.</param>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TVal">The type of the values in the dictionary.</typeparam>
        /// <returns>The number of items removed from the dictionary.</returns>
        public static int RemoveAll<TKey,TVal>(this ObservableDictionary<TKey,TVal> dict, Func<KeyValuePair<TKey, TVal>, bool> condition)
        {
            var itemsToRemove = dict.Where(condition).ToList();
            foreach (var itemToRemove in itemsToRemove)
            {
                dict.Remove(itemToRemove.Key);
            }
            return itemsToRemove.Count;
        }

        /// <summary>
        /// Get the value assigned to a single OID.
        /// </summary>
        /// <param name="snmp">The SimpleSnmp object to be used to get the OID.</param>
        /// <param name="oid">The OID to retrieve a value for.</param>
        /// <returns>The value associated with <paramref name="oid"/></returns>
        public static AsnType Get(this SimpleSnmp snmp, string oid)
        {
            return snmp.Get(SnmpVersion.Ver1, new[] { oid })[new Oid(oid)];
        }
    }
}
