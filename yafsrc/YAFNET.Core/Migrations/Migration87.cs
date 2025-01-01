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

using System;
using System.Threading.Tasks;

using YAF.Core.Context;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// Version 87 Migrations
/// </summary>
public class Migration87 : IRepositoryMigration, IHaveServiceLocator
{
    /// <summary>
    /// Migrate Repositories (Database).
    /// </summary>
    /// <param name="dbAccess">
    ///     The Database access.
    /// </param>
    public Task MigrateDatabaseAsync(IDbAccess dbAccess)
    {
        ArgumentNullException.ThrowIfNull(dbAccess);

        dbAccess.Execute(
            _ =>
            {
                UpgradeTable(this.GetRepository<Forum>());
                UpgradeTable(this.GetRepository<Registry>());
                UpgradeTable(this.GetRepository<User>());

                ///////////////////////////////////////////////////////////

                return true;
            });

        return Task.CompletedTask;
    }

    /// <summary>Upgrades the Forum table.</summary>
    /// <param name="repository">The repository.</param>
    private static void UpgradeTable(IRepository<Forum> repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        repository.UpdateOnly(() => new Forum {RemoteURL = null}, f => f.RemoteURL == "");
    }

    /// <summary>Upgrades the Registry table.</summary>
    /// <param name="repository">The repository.</param>
    private static void UpgradeTable(IRepository<Registry> repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        var entries = repository.Get(x => x.Value.Contains(".xml") && x.Name == "language");

        entries.ForEach(
            item =>
            {
                item.Value = item.Value.Replace(".xml", ".json");

                repository.Update(item);
            });
    }

    /// <summary>Upgrades the Registry table.</summary>
    /// <param name="repository">The repository.</param>
    private static void UpgradeTable(IRepository<User> repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        var entries = repository.Get(x => x.LanguageFile.Contains(".xml"));

        entries.ForEach(
            item =>
            {
                item.LanguageFile = item.LanguageFile.Replace(".xml", ".json");

                repository.Update(item);
            });
    }

    /// <summary>
    /// Gets the ServiceLocator.
    /// </summary>
    /// <value>The service locator.</value>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;
}