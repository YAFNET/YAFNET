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

namespace YAF.Core.Services;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The YAF bad word replace.
/// </summary>
public class BadWordReplace : IBadWordReplace, IHaveServiceLocator
{
    /// <summary>
    ///   The _options.
    /// </summary>
    private const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.Compiled;

    /// <summary>
    /// Initializes a new instance of the <see cref="BadWordReplace" /> class.
    /// </summary>
    /// <param name="objectStore">The object Store.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="serviceLocator">The service locator.</param>
    public BadWordReplace(
        IObjectStore objectStore,
        ILogger<BadWordReplace> logger,
        IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
        this.ObjectStore = objectStore;
        this.Logger = logger;
    }

    /// <summary>
    /// Gets or sets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Gets or sets Logger.
    /// </summary>
    public ILogger Logger { get; set; }

    /// <summary>
    /// Gets or sets ObjectStore.
    /// </summary>
    public IObjectStore ObjectStore { get; set; }

    /// <summary>
    ///   Gets ReplaceItems.
    /// </summary>
    public IEnumerable<BadWordReplaceItem> ReplaceItems
    {
        get
        {
            var replaceItems = this.ObjectStore.GetOrSet(
                Constants.Cache.ReplaceWords, () =>
                {
                    var replaceWords = this.GetRepository<ReplaceWords>().GetByBoardId();

                    // move to collection...
                    return replaceWords.Select(
                        item => new BadWordReplaceItem(item.GoodWord, item.BadWord, Options)).ToList();
                });

            return replaceItems;
        }
    }

    /// <summary>
    /// Searches through SearchText and replaces "bad words" with "good words"
    /// as defined in the database.
    /// </summary>
    /// <param name="searchText">
    /// The string to search through.
    /// </param>
    /// <returns>
    /// The replace.
    /// </returns>
    /// <exception cref="Exception">
    /// <c>Exception</c>.
    /// </exception>
    public string Replace(string searchText)
    {
        if (searchText.IsNotSet())
        {
            return searchText;
        }

        var strReturn = searchText;

        this.ReplaceItems.ForEach(
            item =>
                {
                    try
                    {
                        if (item.BadWordRegEx != null && item.Active)
                        {
                            strReturn = item.BadWordRegEx.Replace(strReturn, item.GoodWord);
                        }
                    }

#if DEBUG
                    catch (Exception e)
                    {
                        throw new FormatException($"Bad Word Regular Expression Failed: {e.Message}", e);
                    }
#else
                        catch (Exception)
                        {
                            // disable this regular expression henceforth...
                            item.Active = false;
                            this.Logger.Warn(
                                "Couldn't run RegEx for Bad Word Replace value: {0}",
                                item.BadWordRegEx);
                        }
#endif
                });

        return strReturn;
    }
}