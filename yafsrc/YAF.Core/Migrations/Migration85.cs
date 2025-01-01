﻿/* Yet Another Forum.NET
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

using YAF.Core.Context;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// Version 85 Migrations
/// </summary>
public class Migration85 : IRepositoryMigration, IHaveServiceLocator
{
    /// <summary>
    /// Migrate Repositories (Database).
    /// </summary>
    /// <param name="dbAccess">
    /// The Database access.
    /// </param>
    public void MigrateDatabase(IDbAccess dbAccess)
    {
        dbAccess.Execute(
            dbCommand =>
            {
                UpgradeTable(this.GetRepository<Category>(), dbCommand);

                ///////////////////////////////////////////////////////////

                return true;
            });
    }

    /// <summary>Upgrades the Category table.</summary>
    /// <param name="repository">The repository.</param>
    /// <param name="dbCommand">The database command.</param>
    private static void UpgradeTable(IRepository<Category> repository, IDbCommand dbCommand)
    {
        CodeContracts.ThrowIfNull(repository);

        if (!dbCommand.Connection.ColumnExists<Category>(x => x.Flags))
        {
            dbCommand.Connection.AddColumn<Category>(x => x.Flags);
        }
    }

    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;
}