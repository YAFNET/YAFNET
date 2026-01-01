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

namespace YAF.Core.Events;

/// <summary>
/// The init database provider event.
/// </summary>
public class InitDatabaseProviderEvent : IAmEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InitDatabaseProviderEvent"/> class.
    /// </summary>
    /// <param name="providerName">
    /// The provider name.
    /// </param>
    /// <param name="dbAccess">
    /// The db access.
    /// </param>
    public InitDatabaseProviderEvent(string providerName, IDbAccess dbAccess)
    {
        this.ProviderName = providerName;
        this.DbAccess = dbAccess;
    }

    /// <summary>
    /// Gets or sets the db access.
    /// </summary>
    public IDbAccess DbAccess { get; set; }

    /// <summary>
    /// Gets or sets the provider name.
    /// </summary>
    public string ProviderName { get; set; }
}