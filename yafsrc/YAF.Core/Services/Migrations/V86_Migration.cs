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
    using System;

    using ServiceStack.OrmLite;
    using System.Data;
    using System.Threading.Tasks;

    using ServiceStack.Text;

    using YAF.Core.Context;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// Version 86 Migrations
    /// </summary>
    public class V86_Migration : IRepositoryMigration, IHaveServiceLocator
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
                    this.UpgradeTable(this.GetRepository<BBCode>(), dbAccess, dbCommand);
                    this.UpgradeTable(this.GetRepository<EventLog>(), dbAccess, dbCommand);
                    this.UpgradeTable(this.GetRepository<Medal>(), dbAccess, dbCommand);
                    this.UpgradeTable(this.GetRepository<Message>(), dbAccess, dbCommand);
                    this.UpgradeTable(this.GetRepository<MessageHistory>(), dbAccess, dbCommand);
                    this.UpgradeTable(this.GetRepository<PMessage>(), dbAccess, dbCommand);
                    this.UpgradeTable(this.GetRepository<ProfileCustom>(), dbAccess, dbCommand);
                    this.UpgradeTable(this.GetRepository<User>(), dbAccess, dbCommand);

                    ///////////////////////////////////////////////////////////

                    return true;
                });

            return Task.CompletedTask;
        }

        /// <summary>Upgrades the BBCode table.</summary>
        /// <param name="repository">The repository.</param>
        /// <param name="dbAccess">The database access.</param>
        /// <param name="dbCommand">The database command.</param>
        private void UpgradeTable(IRepository<BBCode> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnDataType<BBCode>(x => x.DisplayJS).StartsWithIgnoreCase("varchar"))
            {
                if (OrmLiteConfig.DialectProvider.SQLServerName() == "SQLite")
                {
                    dbCommand.Connection.AlterColumn<BBCode>(x => x.DisplayJS, "_old");
                }
                else
                { 
                    dbCommand.Connection.AlterColumn<BBCode>(x => x.DisplayJS);
                }
            }

            if (dbCommand.Connection.ColumnDataType<BBCode>(x => x.EditJS).StartsWithIgnoreCase("varchar"))
            {
                if (OrmLiteConfig.DialectProvider.SQLServerName() == "SQLite")
                {
                    dbCommand.Connection.AlterColumn<BBCode>(x => x.EditJS, "_old");
                }
                else
                { 
                    dbCommand.Connection.AlterColumn<BBCode>(x => x.EditJS);
                }
            }

            if (dbCommand.Connection.ColumnDataType<BBCode>(x => x.DisplayCSS).StartsWithIgnoreCase("varchar"))
            {
                if (OrmLiteConfig.DialectProvider.SQLServerName() == "SQLite")
                {
                    dbCommand.Connection.AlterColumn<BBCode>(x => x.DisplayCSS, "_old");
                }
                else
                { 
                    dbCommand.Connection.AlterColumn<BBCode>(x => x.DisplayCSS);
                }
            }

            if (dbCommand.Connection.ColumnDataType<BBCode>(x => x.SearchRegex).StartsWithIgnoreCase("varchar"))
            {
                if (OrmLiteConfig.DialectProvider.SQLServerName() == "SQLite")
                {
                    dbCommand.Connection.AlterColumn<BBCode>(x => x.SearchRegex, "_old");
                }
                else
                { 
                    dbCommand.Connection.AlterColumn<BBCode>(x => x.SearchRegex);
                }
            }

            if (dbCommand.Connection.ColumnDataType<BBCode>(x => x.ReplaceRegex).StartsWithIgnoreCase("varchar"))
            {
                if (OrmLiteConfig.DialectProvider.SQLServerName() == "SQLite")
                {
                    dbCommand.Connection.AlterColumn<BBCode>(x => x.ReplaceRegex, "_old");
                }
                else
                { 
                    dbCommand.Connection.AlterColumn<BBCode>(x => x.ReplaceRegex);
                }
            }
        }

        /// <summary>Upgrades the EventLog table.</summary>
        /// <param name="repository">The repository.</param>
        /// <param name="dbAccess">The database access.</param>
        /// <param name="dbCommand">The database command.</param>
        private void UpgradeTable(IRepository<EventLog> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnDataType<EventLog>(x => x.Description).StartsWithIgnoreCase("varchar"))
            {
                if (OrmLiteConfig.DialectProvider.SQLServerName() == "SQLite")
                {
                    dbCommand.Connection.AlterColumn<EventLog>(x => x.Description, "_old");
                }
                else
                {
                    dbCommand.Connection.AlterColumn<EventLog>(x => x.Description);
                }
            }
        }

        /// <summary>Upgrades the Medal table.</summary>
        /// <param name="repository">The repository.</param>
        /// <param name="dbAccess">The database access.</param>
        /// <param name="dbCommand">The database command.</param>
        private void UpgradeTable(IRepository<Medal> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnDataType<Medal>(x => x.Description).StartsWithIgnoreCase("varchar"))
            {
                if (OrmLiteConfig.DialectProvider.SQLServerName() == "SQLite")
                {
                    dbCommand.Connection.AlterColumn<Medal>(x => x.Description, "_old");
                }
                else
                { 
                    dbCommand.Connection.AlterColumn<Medal>(x => x.Description);
                }
            }
        }

        /// <summary>Upgrades the Message table.</summary>
        /// <param name="repository">The repository.</param>
        /// <param name="dbAccess">The database access.</param>
        /// <param name="dbCommand">The database command.</param>
        private void UpgradeTable(IRepository<Message> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnDataType<Message>(x => x.MessageText).StartsWithIgnoreCase("varchar"))
            {
                if (OrmLiteConfig.DialectProvider.SQLServerName() == "SQLite")
                {
                    dbCommand.Connection.AlterColumn<Message>(x => x.MessageText, "_old");
                }
                else
                {
                    dbCommand.Connection.AlterColumn<Message>(x => x.MessageText);
                }
            }
        }

        /// <summary>Upgrades the MessageHistory table.</summary>
        /// <param name="repository">The repository.</param>
        /// <param name="dbAccess">The database access.</param>
        /// <param name="dbCommand">The database command.</param>
        private void UpgradeTable(IRepository<MessageHistory> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnDataType<MessageHistory>(x => x.Message).StartsWithIgnoreCase("varchar"))
            {
                if (OrmLiteConfig.DialectProvider.SQLServerName() == "SQLite")
                {
                    dbCommand.Connection.AlterColumn<MessageHistory>(x => x.Message, "_old");
                }
                else
                { 
                    dbCommand.Connection.AlterColumn<MessageHistory>(x => x.Message);
                }
            }
        }

        /// <summary>Upgrades the PMessage table.</summary>
        /// <param name="repository">The repository.</param>
        /// <param name="dbAccess">The database access.</param>
        /// <param name="dbCommand">The database command.</param>
        private void UpgradeTable(IRepository<PMessage> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnDataType<PMessage>(x => x.Body).StartsWithIgnoreCase("varchar"))
            {
                if (OrmLiteConfig.DialectProvider.SQLServerName() == "SQLite")
                {
                    dbCommand.Connection.AlterColumn<PMessage>(x => x.Body, "_old");
                }
                else
                {
                    dbCommand.Connection.AlterColumn<PMessage>(x => x.Body);
                }
            }
        }

        /// <summary>Upgrades the ProfileCustom table.</summary>
        /// <param name="repository">The repository.</param>
        /// <param name="dbAccess">The database access.</param>
        /// <param name="dbCommand">The database command.</param>
        private void UpgradeTable(IRepository<ProfileCustom> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnDataType<ProfileCustom>(x => x.Value).StartsWithIgnoreCase("varchar"))
            {
                if (OrmLiteConfig.DialectProvider.SQLServerName() == "SQLite")
                {
                    dbCommand.Connection.AlterColumn<ProfileCustom>(x => x.Value, "_old");
                }
                else
                {
                    dbCommand.Connection.AlterColumn<ProfileCustom>(x => x.Value);
                }
            }
        }

        /// <summary>Upgrades the User table.</summary>
        /// <param name="repository">The repository.</param>
        /// <param name="dbAccess">The database access.</param>
        /// <param name="dbCommand">The database command.</param>
        private void UpgradeTable(IRepository<User> repository, IDbAccess dbAccess, IDbCommand dbCommand)
        {
            if (dbCommand.Connection.ColumnDataType<User>(x => x.Signature).StartsWithIgnoreCase("varchar"))
            {
                if (OrmLiteConfig.DialectProvider.SQLServerName() == "SQLite")
                {
                    dbCommand.Connection.AlterColumn<User>(x => x.Signature, "_old");
                }
                else
                {
                    dbCommand.Connection.AlterColumn<User>(x => x.Signature);
                }
            }
        }

        public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;
    }
}