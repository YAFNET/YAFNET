/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Services.Migrations
{
    using System.Data;
    using System.Threading.Tasks;

    using YAF.Core.Context;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// Version 89 Migrations
    /// </summary>
    public class V89_Migration : IRepositoryMigration, IHaveServiceLocator
    {
        /// <summary>
        /// Migrate Repositories (Database).
        /// </summary>
        /// <param name="dbAccess">
        ///     The Database access.
        /// </param>
        public Task MigrateDatabaseAsync(IDbAccess dbAccess)
        {
            dbAccess.Execute(
                dbCommand =>
                    {
                        UpgradeTable(this.GetRepository<User>(), dbCommand);
                        UpgradeTable(this.GetRepository<Rank>(), dbCommand);
                        UpgradeTable(this.GetRepository<Group>(), dbCommand);
                        UpgradeTable(this.GetRepository<PrivateMessage>(), dbAccess, dbCommand);

                        ///////////////////////////////////////////////////////////

                        return true;
                    });

            return Task.CompletedTask;
        }

        /// <summary>Upgrades the User table.</summary>
        /// <param name="repository">The repository.</param>
        /// <param name="dbCommand">The db command.</param>
        private static void UpgradeTable(IRepository<User> repository, IDbCommand dbCommand)
        {
            if (!dbCommand.Connection.ColumnExists<User>(x => x.DarkMode))
            {
                dbCommand.Connection.AddColumn<User>(x => x.DarkMode);
            }

            if (dbCommand.Connection.ColumnExists<User>("PMNotification"))
            {
                dbCommand.Connection.DropColumn<User>("PMNotification");
            }
        }

        /// <summary>Upgrades the Rank table.</summary>
        /// <param name="repository">The repository.</param>
        /// <param name="dbCommand">The db command.</param>
        private static void UpgradeTable(IRepository<Rank> repository, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnExists<Rank>("PMLimit"))
            {
                dbCommand.Connection.DropColumn<Rank>("PMLimit");
            }
        }

        /// <summary>Upgrades the Group table.</summary>
        /// <param name="repository">The repository.</param>
        /// <param name="dbCommand">The db command.</param>
        private static void UpgradeTable(IRepository<Group> repository, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnExists<Group>("PMLimit"))
            {
                dbCommand.Connection.DropColumn<Group>("PMLimit");
            }
        }

        /// <summary>Upgrades the PrivateMessage table.</summary>
        /// <param name="repository">The repository.</param>
        /// <param name="dbAccess">
        /// The Database access.
        /// </param>
        /// <param name="dbCommand">The db command.</param>
        private static void UpgradeTable(
            IRepository<PrivateMessage> repository,
            IDbAccess dbAccess,
            IDbCommand dbCommand)
        {
            dbAccess.Execute(db => db.Connection.CreateTableIfNotExists<PrivateMessage>());
        }

        /// <summary>
        /// Gets the ServiceLocator.
        /// </summary>
        /// <value>The service locator.</value>
        public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;
    }
}