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

namespace YAF.Core.Helpers;

using System.Text.RegularExpressions;

/// <summary>
/// Helper for Figuring out the PageUser Agent.
/// </summary>
public static partial class UserAgentHelper
{
    /// <summary>
    /// Spider Detection Regex
    /// </summary>
    /// <returns>Regex.</returns>
    [GeneratedRegex("bot|spider|yandex|crawler|appie|robot|atomz")]
    private static partial Regex Spiders();

    /// <summary>
    /// Validates if the user agent is a search engine spider
    /// </summary>
    /// <param name="userAgent">The user agent.</param>
    /// <returns>
    /// The is search engine spider.
    /// </returns>
    public static bool SearchEngineSpiderName(string userAgent)
    {
        return userAgent.IsSet() && Spiders().Match(userAgent.ToLowerInvariant()).Success;
    }
}