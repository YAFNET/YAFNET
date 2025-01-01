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

using System.Text.RegularExpressions;

namespace YAF.Core.Helpers;

/// <summary>
/// Helper for Figuring out the User Agent.
/// </summary>
public static class UserAgentHelper
{
    /// <summary>
    /// The spiders detection regex
    /// </summary>
    private readonly static Regex spiders = new("bot|spider|yandex|crawler|appie|robot|atomz");

    /// <summary>
    /// Validates if the user agent is a search engine spider
    /// </summary>
    /// <param name="userAgent">The user agent.</param>
    /// <returns>
    /// The is search engine spider.
    /// </returns>
    public static bool IsSearchEngineSpider(string userAgent)
    {
        return userAgent.IsSet() && spiders.Match(userAgent.ToLowerInvariant()).Success;
    }

    /// <summary>
    /// Returns a platform user friendly name.
    /// </summary>
    /// <param name="userAgent">The user agent.</param>
    /// <param name="isCrawler">if set to <c>true</c> [is crawler].</param>
    /// <param name="platform">The platform.</param>
    /// <param name="isSearchEngine">if set to <c>true</c> [is search engine].</param>
    public static void Platform(
        string userAgent,
        bool isCrawler,
        ref string platform,
        out bool isSearchEngine)
    {
        isSearchEngine = false;

        if (userAgent.IsNotSet())
        {
            platform = "[Empty]";

            return;
        }

        isSearchEngine = isCrawler || IsSearchEngineSpider(userAgent);

        if (userAgent.Contains("Windows NT "))
        {
            if (userAgent.Contains("Windows NT 10"))
            {
                platform = "Windows 10";
            }
            else if (userAgent.Contains("Windows NT 6.3"))
            {
                platform = "Windows 8.1";
            }
            else if (userAgent.Contains("Windows NT 6.2"))
            {
                platform = "Windows 8";
            }
            else if (userAgent.Contains("Windows NT 6.1"))
            {
                platform = "Windows 7";
            }
            else if (userAgent.Contains("Windows NT 6.0"))
            {
                platform = "Windows Vista";
            }
            else if (userAgent.Contains("Windows NT 5.1"))
            {
                platform = "Windows XP";
            }
            else if (userAgent.Contains("Windows NT 5.2"))
            {
                platform = "Windows 2003";
            }
        }
        else if (userAgent.Contains("Linux"))
        {
            platform = "Linux";
        }
        else if (userAgent.Contains("FreeBSD"))
        {
            platform = "FreeBSD";
        }
        else if (userAgent.Contains("iPad"))
        {
            platform = "iPad(iOS)";
        }
        else if (userAgent.Contains("iPhone"))
        {
            platform = "iPhone(iOS)";
        }
        else if (userAgent.Contains("iPod"))
        {
            platform = "iPod(iOS)";
        }
        else if (userAgent.Contains("WindowsMobile"))
        {
            platform = "WindowsMobile";
        }
        else if (userAgent.Contains("Windows Phone OS"))
        {
            platform = "Windows Phone";
        }
        else if (userAgent.Contains("webOS"))
        {
            platform = "WebOS";
        }
        else if (userAgent.Contains("Android"))
        {
            platform = "Android";
        }
        else if (userAgent.Contains("Mac OS X"))
        {
            platform = "Mac OS X";
        }
    }
}