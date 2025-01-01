/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core.Migrations;

using ServiceStack.OrmLite;

using System.Data;
using System.Threading.Tasks;

using YAF.Core.Context;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

using System;

/// <summary>
/// Version 86 Migrations
/// </summary>
public class Migration86 : IRepositoryMigration, IHaveServiceLocator
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
                this.UpgradeTable(this.GetRepository<BBCode>(), dbCommand);
                this.UpgradeTable(this.GetRepository<EventLog>(), dbCommand);
                UpgradeTable(this.GetRepository<Medal>(), dbCommand);
                UpgradeTable(this.GetRepository<Message>(), dbCommand);
                this.UpgradeTable(this.GetRepository<MessageHistory>(), dbCommand);
                UpgradeTable(this.GetRepository<PMessage>(), dbCommand);
                this.UpgradeTable(this.GetRepository<ProfileCustom>(), dbCommand);
                UpgradeTable(this.GetRepository<User>(), dbCommand);

                ///////////////////////////////////////////////////////////

                return true;
            });

        return Task.CompletedTask;
    }

    /// <summary>Upgrades the BBCode table.</summary>
    /// <param name="repository">The repository.</param>
    /// <param name="dbCommand">The database command.</param>
    private void UpgradeTable(IRepository<BBCode> repository, IDbCommand dbCommand)
    {
        ArgumentNullException.ThrowIfNull(repository);

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
    /// <param name="dbCommand">The database command.</param>
    private void UpgradeTable(IRepository<EventLog> repository, IDbCommand dbCommand)
    {
        ArgumentNullException.ThrowIfNull(repository);

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
    /// <param name="dbCommand">The database command.</param>
    private static void UpgradeTable(IRepository<Medal> repository, IDbCommand dbCommand)
    {
        ArgumentNullException.ThrowIfNull(repository);

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
    /// <param name="dbCommand">The database command.</param>
    private static void UpgradeTable(IRepository<Message> repository, IDbCommand dbCommand)
    {
        ArgumentNullException.ThrowIfNull(repository);

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
    /// <param name="dbCommand">The database command.</param>
    private void UpgradeTable(IRepository<MessageHistory> repository, IDbCommand dbCommand)
    {
        ArgumentNullException.ThrowIfNull(repository);

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
    /// <param name="dbCommand">The database command.</param>
    private static void UpgradeTable(IRepository<PMessage> repository, IDbCommand dbCommand)
    {
        ArgumentNullException.ThrowIfNull(repository);

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
    /// <param name="dbCommand">The database command.</param>
    private void UpgradeTable(IRepository<ProfileCustom> repository, IDbCommand dbCommand)
    {
        ArgumentNullException.ThrowIfNull(repository);

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
    /// <param name="dbCommand">The database command.</param>
    private static void UpgradeTable(IRepository<User> repository, IDbCommand dbCommand)
    {
        ArgumentNullException.ThrowIfNull(repository);

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

    /// <summary>
    /// Gets the ServiceLocator.
    /// </summary>
    /// <value>The service locator.</value>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;
}