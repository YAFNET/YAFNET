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

namespace YAF.Core.Data;

using Autofac.Features.Indexed;

using YAF.Core.Events;
using YAF.Types.Objects;

/// <summary>
///     The Database connection provider base.
/// </summary>
public class DbAccessProvider : IDbAccessProvider
{
    /// <summary>
    /// The database access safe.
    /// </summary>
    private readonly SafeReadWriteProvider<IDbAccess> _dbAccessSafe;

    /// <summary>
    /// The service locator.
    /// </summary>
    private readonly IServiceLocator _serviceLocator;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DbAccessProvider" /> class.
    /// </summary>
    /// <param name="dbAccessProviders">
    ///     The database access providers.
    /// </param>
    /// <param name="serviceLocator">
    ///     The service locator.
    /// </param>
    public DbAccessProvider(IIndex<string, IDbAccess> dbAccessProviders, IServiceLocator serviceLocator)
    {
        var dbAccessProviders1 = dbAccessProviders;
        this._serviceLocator = serviceLocator;

        this._dbAccessSafe = new SafeReadWriteProvider<IDbAccess>(
            () =>
                {
                    // attempt to get the provider...
                    if (dbAccessProviders1.TryGetValue(this.ProviderName, out var dbAccess))
                    {
                        // first time...
                        this._serviceLocator.Get<IRaiseEvent>()
                            .Raise(new InitDatabaseProviderEvent(this.ProviderName, dbAccess));
                    }
                    else
                    {
                        throw new NoValidDbAccessProviderFoundException(
                            $"""Unable to Locate Provider Named "{this.ProviderName}" in Data Access Providers (DLL Not Located in Bin Directory?).""");
                    }

                    return dbAccess;
                });
    }

    /// <summary>
    /// Gets or sets the instance of the IDbAccess provider.
    /// </summary>
    /// <returns> </returns>
    /// <exception cref="NoValidDbAccessProviderFoundException">
    ///     <c>NoValidDbAccessProviderFoundException</c>
    ///     .
    /// </exception>
    public IDbAccess Instance
    {
        get => this._dbAccessSafe.Instance;

        set
        {
            this._dbAccessSafe.Instance = value;
            if (value != null)
            {
                this.ProviderName = value.Information.ProviderName;
            }
        }
    }

    /// <summary>
    ///     Gets or sets ProviderName.
    /// </summary>
    public string ProviderName
    {
        get => field ??= this._serviceLocator.Get<BoardConfiguration>().ConnectionProviderName;

        set
        {
            field = value;
            this._dbAccessSafe.Instance = null;
        }
    }
}