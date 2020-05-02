/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Utils.Helpers
{
    #region Using

    using System;
    using System.Linq;
    using System.Web;

    using YAF.Configuration;
    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// Helper for Figuring out the User Agent.
    /// </summary>
    public static class UserAgentHelper
    {
        #region Constants and Fields

        /// <summary>
        /// The spider contains.
        /// </summary>
        private static readonly string[] SpiderContains =
            {
                "abachoBOT", "abcdatos_botlink", "ah-ha.com crawler", "antibot", "appie", "AltaVista-Intranet",
                "Acoon Robot", "Atomz", "Arachnoidea", "AESOP_com_SpiderMan", "AxmoRobot", "ArchitextSpider",
                "AlkalineBOT", "Aranha", "asterias", "Baidu", "Bingbot", "Buscaplus Robi", "CanSeek", "ChristCRAWLER",
                "Clushbot", "Crawler", "CrawlerBoy", "DeepIndex", "DefaultCrawler", "DittoSpyder", "DIIbot", "EZResult",
                "EARTHCOM.info", "EuripBot", "ESISmartSpider", "FAST-WebCrawler", "FyberSearch", "Findexa Crawler",
                "Fluffy", "Googlebot", "geckobot", "GenCrawler", "GeonaBot", "getRAX", "Gulliver", "Hubater",
                "ia_archiver", "Slurp", "Scooter", "Mercator", "RaBot", "Jack", "Speedy Spider", "moget", "Toutatis",
                "IlTrovatore-Setaccio", "IncyWincy", "UltraSeek", "InfoSeek Sidewinder", "Mole2", "MP3Bot",
                "Knowledge.com", "kuloko-bot", "LNSpiderguy", "Linknzbot", "lookbot", "MantraAgent",
                "NetResearchServer", "Lycos", "JoocerBot", "HenryTheMiragoRobot", "MojeekBot", "mozDex", "MSNBOT",
                "Navadoo Crawler", "ObjectsSearch", "OnetSzukaj", "PicoSearch", "PJspider", "nttdirectory_robot",
                "maxbot.com", "Openfind", "psbot", "QweeryBot", "StackRambler", "SeznamBot", "Search-10", "Scrubby",
                "speedfind ramBot xtreme", "Kototoi", "SearchByUsa", "Searchspider", "SightQuestBot", "Spider_Monkey",
                "Surfnomore", "teoma", "UK Searcher Spider", "Nazilla", "MuscatFerret", "ZyBorg", "WIRE WebRefiner",
                "WSCbot", "Yandex", "Yellopet-Spider", "YBSbot", "OceanSpiders", "MozSpider"
            };

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates if the user agent owner is a feed reader
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <returns>
        /// The is feed reader.
        /// </returns>
        public static bool IsFeedReader([CanBeNull] string userAgent)
        {
            string[] agentContains = { "Windows-RSS-Platform", "FeedDemon", "Feedreader", "Apple-PubSub" };

            return userAgent.IsSet() && agentContains.Any(
                       agentContain => userAgent.ToLowerInvariant().Contains(agentContain.ToLowerInvariant()));
        }

        /// <summary>
        /// Validates if the user agent is a known ignored UA string
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <returns>
        /// The true if the UA string pattern should not be displayed in active users.
        /// </returns>
        public static bool IsIgnoredForDisplay([CanBeNull] string userAgent)
        {
            if (!userAgent.IsSet())
            {
                return false;
            }

            // Apple-PubSub - Safari RSS reader
            string[] stringContains = { "PlaceHolder" };

            return stringContains.Any(x => userAgent.ToLowerInvariant().Contains(x.ToLowerInvariant()));
        }

        /// <summary>
        /// Tests if the user agent is a mobile device.
        /// </summary>
        /// <param name="requestBase">
        /// The request Base.
        /// </param>
        /// <returns>
        /// The is mobile device.
        /// </returns>
        public static bool IsMobileDevice([CanBeNull] HttpRequestBase requestBase)
        {
            if (requestBase.Browser.IsMobileDevice)
            {
                return true;
            }

            var mobileContains = Config.MobileUserAgents.Split(',').Where(m => m.IsSet())
                .Select(m => m.Trim().ToLowerInvariant());

            return requestBase.UserAgent != null && requestBase.UserAgent.IsSet()
                   && mobileContains.Any(s => requestBase.UserAgent.IndexOf(s, StringComparison.OrdinalIgnoreCase) > 0);
        }

        /// <summary>
        /// Validates if the user agent is a search engine spider
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <returns>
        /// The is search engine spider.
        /// </returns>
        public static bool IsSearchEngineSpider([CanBeNull] string userAgent)
        {
            return userAgent.IsSet()
                   && SpiderContains.Any(x => userAgent.ToLowerInvariant().Contains(x.ToLowerInvariant()));
        }

        /// <summary>
        /// Returns a platform user friendly name.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="isCrawler">if set to <c>true</c> [is crawler].</param>
        /// <param name="platform">The platform.</param>
        /// <param name="browser">The browser.</param>
        /// <param name="isSearchEngine">if set to <c>true</c> [is search engine].</param>
        /// <param name="isIgnoredForDisplay">if set to <c>true</c> [is ignored for display].</param>
        public static void Platform(
            [CanBeNull] string userAgent,
            bool isCrawler,
            [NotNull] ref string platform,
            [NotNull] ref string browser,
            out bool isSearchEngine,
            out bool isIgnoredForDisplay)
        {
            CodeContracts.VerifyNotNull(platform, "platform");

            isSearchEngine = false;
            isIgnoredForDisplay = false;

            if (userAgent.IsNotSet())
            {
                platform = "[Empty]";
                isIgnoredForDisplay = true;

                return;
            }

            if (userAgent.IndexOf("Windows NT ", StringComparison.Ordinal) >= 0)
            {
                if (userAgent.IndexOf("Windows NT 10", StringComparison.Ordinal) >= 0)
                {
                    platform = "Windows 10";
                }
                else if (userAgent.IndexOf("Windows NT 6.3", StringComparison.Ordinal) >= 0)
                {
                    platform = "Windows 8.1";
                }
                else if (userAgent.IndexOf("Windows NT 6.2", StringComparison.Ordinal) >= 0)
                {
                    platform = "Windows 8";
                }
                else if (userAgent.IndexOf("Windows NT 6.1", StringComparison.Ordinal) >= 0)
                {
                    platform = "Windows 7";
                }
                else if (userAgent.IndexOf("Windows NT 6.0", StringComparison.Ordinal) >= 0)
                {
                    platform = "Windows Vista";
                }
                else if (userAgent.IndexOf("Windows NT 5.1", StringComparison.Ordinal) >= 0)
                {
                    platform = "Windows XP";
                }
                else if (userAgent.IndexOf("Windows NT 5.2", StringComparison.Ordinal) >= 0)
                {
                    platform = "Windows 2003";
                }
            }
            else if (userAgent.IndexOf("Linux", StringComparison.Ordinal) >= 0)
            {
                platform = "Linux";
            }
            else if (userAgent.IndexOf("FreeBSD", StringComparison.Ordinal) >= 0)
            {
                platform = "FreeBSD";
            }
            else if (userAgent.IndexOf("iPad", StringComparison.Ordinal) >= 0)
            {
                platform = "iPad(iOS)";
            }
            else if (userAgent.IndexOf("iPhone", StringComparison.Ordinal) >= 0)
            {
                platform = "iPhone(iOS)";
            }
            else if (userAgent.IndexOf("iPod", StringComparison.Ordinal) >= 0)
            {
                platform = "iPod(iOS)";
            }
            else if (userAgent.IndexOf("WindowsMobile", StringComparison.Ordinal) >= 0)
            {
                platform = "WindowsMobile";
            }
            else if (userAgent.IndexOf("Windows Phone OS", StringComparison.Ordinal) >= 0)
            {
                platform = "Windows Phone";
            }
            else if (userAgent.IndexOf("webOS", StringComparison.Ordinal) >= 0)
            {
                platform = "WebOS";
            }
            else if (userAgent.IndexOf("Android", StringComparison.Ordinal) >= 0)
            {
                platform = "Android";
            }
            else if (userAgent.IndexOf("Mac OS X", StringComparison.Ordinal) >= 0)
            {
                platform = "Mac OS X";
            }
            else
            {
                // check if it's a search engine spider or an ignored UI string...
                var san = SearchEngineSpiderName(userAgent);
                if (san.IsSet())
                {
                    browser = san;
                }

                isSearchEngine = isCrawler || san.IsSet();
                isIgnoredForDisplay = IsIgnoredForDisplay(userAgent) | isSearchEngine;
            }
        }

        /// <summary>
        /// Validates if the user agent is a search engine spider
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <returns>
        /// The is search engine spider.
        /// </returns>
        public static string SearchEngineSpiderName([CanBeNull] string userAgent)
        {
            return userAgent.IsSet()
                       ? SpiderContains.FirstOrDefault(x => userAgent.ToLowerInvariant().Contains(x.ToLowerInvariant()))
                       : null;
        }

        #endregion
    }
}