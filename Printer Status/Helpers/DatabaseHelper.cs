using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading.Tasks;
using Printer_Status.Properties;

namespace Printer_Status.Helpers
{
    /// <summary>
    /// Helper class for database interaction.
    /// </summary>
    public static class DatabaseHelper
    {
        /// <summary>
        /// Get a dictionary of printers representing the `Watched` table in the database.
        /// </summary>
        /// <returns>A dictionary of printers</returns>
        public static async Task<Dictionary<string, Tuple<string, string, string>>> GetDatabasePrintersAsync()
        {
            //An empty dictionary to hold the database's printers.
            Dictionary<string, Tuple<string, string, string>> databasePrinters =
                new Dictionary<string, Tuple<string, string, string>>();
            using (SQLiteConnection connection = new SQLiteConnection(Settings.Default.ConnectionString))
            {
                //Open the database connection
                await connection.OpenAsync();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Watched", connection))
                {
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            //Add every printer in the database to the dictionary, keyed by IP address.
                            databasePrinters[reader["IP"].ToString()] =
                                Tuple.Create(reader["Name"].ToString(), reader["Location"].ToString(), reader["LastSeen"].ToString());
                        }
                    }
                }
            }
            return databasePrinters;
        }
    }
}