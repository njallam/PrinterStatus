using System.Collections.Generic;
using System.Linq;
using SnmpSharpNet;

namespace Printer_Status.Helpers
{
    /// <summary>
    /// Helper class for SNMP interaction.
    /// </summary>
    public static class SnmpHelper
    {
        /// <summary>
        /// The default SNMP port.
        /// </summary>
        public static int Port = 161;

        /// <summary>
        /// Get a list of all the suffixes under a root OID from the result of an SNMP query.
        /// </summary>
        /// <param name="result">The result of an SNMP query.</param>
        /// <param name="root">The root OID to look under.</param>
        /// <param name="bytes">The depth of the child nodes.</param>
        /// <returns>An array of suffixes to the root OID.</returns>
        public static string[] GetSuffixes(Dictionary<Oid, AsnType> result, string root, int bytes = 1)
        {
            return result.Keys.Where(key => new Oid(root).IsRootOf(key)) //Get a list of Oids which are children of 'root'
                .Select(key => string.Join(".", key.Reverse().Take(bytes).Reverse())) //Remove all but the last 'bytes' bytes from the list
                .Distinct().ToArray(); //Remove duplicates and convert to array.
        }

    }
}