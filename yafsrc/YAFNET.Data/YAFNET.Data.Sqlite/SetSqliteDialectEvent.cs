/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Data.Sqlite;

/// <summary>
/// Set the MS SQL dialect event.
/// </summary>
[ExportService(ServiceLifetimeScope.InstancePerDependency, new[] { typeof(IHandleEvent<InitDatabaseProviderEvent>) })]
public class SetSqliteDialectEvent : IHandleEvent<InitDatabaseProviderEvent>
{
    /// <summary>
    ///     Gets the order.
    /// </summary>
    public int Order => 1000;

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    ///     The event.
    /// </param>
    public void Handle(InitDatabaseProviderEvent @event)
    {
        if (@event.ProviderName != SqliteDbAccess.ProviderTypeName)
        {
            return;
        }

        // set the OrmLite dialect provider...
        OrmLiteConfig.DialectProvider = YafSqliteOrmLiteDialectProvider.Instance;

        OrmLiteConfig.DialectProvider.GetDateTimeConverter().DateStyle = DateTimeKind.Utc;
        OrmLiteConfig.DialectProvider.GetStringConverter().UseUnicode = true;
        OrmLiteConfig.CommandTimeout = Config.SqlCommandTimeout;
    }
}