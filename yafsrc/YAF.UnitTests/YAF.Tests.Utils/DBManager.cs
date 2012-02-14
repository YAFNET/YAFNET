/* Yet Another Forum.NET
 *
 * Copyright (C) Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions 
 * of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
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