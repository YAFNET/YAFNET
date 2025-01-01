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

namespace YAF.Types.Interfaces.Data;

using System.Collections.Generic;
using System.Data;

/// <summary>
/// The Database Information Interface
/// </summary>
public interface IDbInformation
{
    /// <summary>
    /// Gets the service locator.
    /// </summary>
    /// <value>The service locator.</value>
    public IServiceLocator ServiceLocator { get; }

    /// <summary>
    /// Gets or sets the DB Connection String
    /// </summary>
    Func<string> ConnectionString { get; set; }

    /// <summary>
    /// Gets the DB Provider Name
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    ///     Gets the YAF Provider Upgrade script list
    /// </summary>
    IEnumerable<string> IdentityUpgradeScripts { get; }

    /// <summary>
    /// Create Table Views
    /// </summary>
    /// <param name="dbAccess">
    /// The database access.
    /// </param>
    /// <param name="dbCommand">
    /// The database command.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    bool CreateViews(IDbAccess dbAccess, IDbCommand dbCommand);

    /// <summary>
    /// Create Indexes on Table Views
    /// </summary>
    /// <param name="dbAccess">
    /// The database access.
    /// </param>
    /// <param name="dbCommand">
    /// The database command.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    bool CreateIndexViews(IDbAccess dbAccess, IDbCommand dbCommand);
}