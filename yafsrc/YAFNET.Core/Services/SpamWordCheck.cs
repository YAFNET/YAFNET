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
public class SpamWordCheck : ISpamWordCheck, IHaveServiceLocator
{
    /// <summary>
    ///   The _options.
    /// </summary>
    private const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.Compiled;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpamWordCheck" /> class.
    /// </summary>
    /// <param name="objectStore">The object Store.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="serviceLocator">The service locator.</param>
    public SpamWordCheck(IObjectStore objectStore, ILogger<SpamWordCheck> logger, IServiceLocator serviceLocator)
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
    /// Gets the spam word items.
    /// </summary>
    /// <value>
    /// The spam word items.
    /// </value>
    public IEnumerable<SpamWordCheckItem> SpamWordItems
    {
        get
        {
            var spamItems = this.ObjectStore.GetOrSet(
                Constants.Cache.SpamWords,
                () =>
                    {
                        var spamWords = this.GetRepository<SpamWords>().Get(
                            x => x.BoardID == this.GetRepository<SpamWords>().BoardID);

                        // move to collection...
                        return
                            spamWords.Select(
                                item => new SpamWordCheckItem(item.SpamWord, Options)).ToList();
                    });

            return spamItems;
        }
    }

    /// <summary>
    /// Checks for spam word.
    /// </summary>
    /// <param name="searchText">The search text.</param>
    /// <param name="spamWord">The spam word.</param>
    /// <returns>
    /// Returns if the search Text contains a spam word
    /// </returns>
    public bool CheckForSpamWord(string searchText, out string spamWord)
    {
        spamWord = string.Empty;

        if (searchText.IsNotSet())
        {
            return false;
        }

        foreach (var item in this.SpamWordItems)
        {
            try
            {
                if (item.SpamWordRegEx == null || !item.Active)
                {
                    continue;
                }

                var match = item.SpamWordRegEx.Match(searchText);

                if (!match.Success)
                {
                    continue;
                }

                spamWord = match.Value;
                return true;
            }

#if DEBUG
            catch (Exception e)
            {
                throw new FormatException($"Spam Word Regular Expression Failed: {e.Message}", e);
            }

#else
            catch (Exception)
            {
                // disable this regular expression henceforth...
                item.Active = false;
                this.Logger.Warn("Couldn't run RegEx for Spam Word Replace value: {0}", item.SpamWordRegEx);
            }

#endif
        }

        spamWord = string.Empty;
        return false;
    }
}