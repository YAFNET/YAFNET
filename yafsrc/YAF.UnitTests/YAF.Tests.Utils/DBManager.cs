/* Yet Another Forum.NET
 *
 * Copyright (C) Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 3
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Tests.Utils
{
    using System.Collections.Specialized;
    using System.Threading;

    using Microsoft.SqlServer.Management.Smo;

    /// <summary>
    /// Database Manager
    /// </summary>
    public class DBManager
    {
        /// <summary>
        /// Attaches the database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="databaseFile">The database file.</param>
        public static void AttachDatabase(string databaseName, string databaseFile)
        {
            var server = new Server(TestConfig.DatabaseServer);

            // Drop the database
            DropDatabase(server, databaseName);

            server.AttachDatabase(databaseName, new StringCollection { databaseFile });

            while (server.Databases[databaseName].State != SqlSmoState.Existing)
            {
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Drops the database.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        public static void DropDatabase(string databaseName)
        {
            var server = new Server(TestConfig.DatabaseServer);

            DropDatabase(server, databaseName);
        }

        /// <summary>
        /// Drops the database.
        /// </summary>
        /// <param name="server">The <paramref name="server"/>.</param>
        /// <param name="databaseName">Name of the database.</param>
        public static void DropDatabase(Server server, string databaseName)
        {
            var database = server.Databases[databaseName];

            if (database == null)
            {
                return;
            }

            server.KillDatabase(databaseName);

            while (server.Databases[databaseName] != null)
            {
                Thread.Sleep(100);
            }
        }
    }
}