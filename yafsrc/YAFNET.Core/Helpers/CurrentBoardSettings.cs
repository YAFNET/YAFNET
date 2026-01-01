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

using System;

namespace YAF.Core.Helpers;

using System.Runtime.Caching;

/// <summary>
///     The current board settings.
/// </summary>
public class CurrentBoardSettings : IReadWriteProvider<BoardSettings>
{
    /// <summary>
    ///     The have board id.
    /// </summary>
    private readonly IHaveBoardID haveBoardId;

    /// <summary>
    ///     The inject services.
    /// </summary>
    private readonly IInjectServices injectServices;

    /// <summary>
    ///     The treat cache key.
    /// </summary>
    private readonly ITreatCacheKey treatCacheKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentBoardSettings"/> class.
    /// </summary>
    /// <param name="injectServices">
    /// The inject services.
    /// </param>
    /// <param name="haveBoardId">
    /// The have board id.
    /// </param>
    /// <param name="treatCacheKey">
    /// The treat Cache Key.
    /// </param>
    public CurrentBoardSettings(
        IInjectServices injectServices,
        IHaveBoardID haveBoardId,
        ITreatCacheKey treatCacheKey)
    {
        ArgumentNullException.ThrowIfNull(injectServices);
        ArgumentNullException.ThrowIfNull(haveBoardId);
        ArgumentNullException.ThrowIfNull(treatCacheKey);

        this.injectServices = injectServices;
        this.haveBoardId = haveBoardId;
        this.treatCacheKey = treatCacheKey;
    }

    /// <summary>
    ///     Gets or sets Object.
    /// </summary>
    public BoardSettings Instance
    {
        get
        {
            return MemoryCache.Default.GetOrSet(
                this.treatCacheKey.Treat(Constants.Cache.BoardSettings),
                () =>
                    {
                        var boardSettings = BoardContext.Current.Get<BoardSettingsService>()
                            .LoadBoardSettings(this.haveBoardId.BoardID, null);

                        // inject
                        this.injectServices.Inject(boardSettings);

                        return boardSettings;
                    });
        }

        set => MemoryCache.Default.Set(this.treatCacheKey.Treat(Constants.Cache.BoardSettings), value);
    }

    /// <summary>
    /// Reset Board Settings
    /// </summary>
    public void Reset()
    {
        MemoryCache.Default.Remove(this.treatCacheKey.Treat(Constants.Cache.BoardSettings));
    }
}