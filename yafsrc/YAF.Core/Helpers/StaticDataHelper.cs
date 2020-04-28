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

namespace YAF.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Xml;

    using YAF.Core.Context;
    using YAF.Core.Services.Localization;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using YAF.Utils;

    /// <summary>
    /// The static data helper.
    /// </summary>
    public class StaticDataHelper
    {
        #region Public Methods  

        /// <summary>
        /// The country.
        /// </summary>
        /// <param name="localization">
        /// The localization.
        /// </param>
        /// <returns>
        /// Returns a Data Table with all country names (localized).
        /// </returns>
        public static IReadOnlyCollection<ListItem> Country(ILocalization localization)
        {
            var countriesList = new List<ListItem>();

            var item = new ListItem(localization.GetText("COMMON", "NONE"), null);

            countriesList.Add(item);

            var countries = localization.GetRegionNodesUsingQuery("COUNTRY", x => x.tag.StartsWith(string.Empty))
                .OrderBy(c => c.Value).ToList();

            // vzrus: a temporary hack - it returns all tags if the page is not found
            if (countries.Count > 2000)
            {
                return countriesList;
            }

            countries.ForEach(node => countriesList.Add(new ListItem(node.Value, node.tag)));

            return countriesList;
        }

        /// <summary>
        /// Gets all country names list(localized).
        /// </summary>
        /// <returns>
        /// Returns a Data Table with all country names list(localized)
        /// </returns>
        public static IReadOnlyCollection<ListItem> Country()
        {
            return Country(BoardContext.Current.Get<ILocalization>());
        }

        /// <summary>
        /// Gets all country names list(localized).
        /// </summary>
        /// <param name="forceLanguage">
        /// The force a specific language.
        /// </param>
        /// <returns>
        /// Returns a Data Table with all country names list(localized)
        /// </returns>
        public static IReadOnlyCollection<ListItem> Country(string forceLanguage)
        {
            var localization = new Localization();
            localization.LoadTranslation(forceLanguage);

            return Country(localization);
        }

        /// <summary>
        /// Gets all country names list(localized).
        /// </summary>
        /// <param name="localization">The localization.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// Returns a Data Table with all country names list(localized)
        /// </returns>
        public static IReadOnlyCollection<ListItem> Region(ILocalization localization, string culture)
        {
            var list = new List<ListItem> { new ListItem(null, null) };

            var countries = localization
                .GetCountryNodesUsingQuery("REGION", x => x.tag.StartsWith($"RGN_{culture}_")).ToList();

            countries.ForEach(
                node => list.Add(new ListItem(node.Value, node.tag.Replace($"RGN_{culture}_", string.Empty))));

            return list;
        }

        /// <summary>
        /// Gets all region names (localized)
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// Returns a Data Table with all region names (localized)
        /// </returns>
        public static IReadOnlyCollection<ListItem> Region(string culture)
        {
            return Region(BoardContext.Current.Get<ILocalization>(), culture);
        }

        /// <summary>
        /// The cultures IetfLangTags (4-letter).
        /// </summary>
        /// <returns>
        /// The cultures filtered by first 2 letters in the language tag in a language file
        /// </returns>
        public static IReadOnlyCollection<Culture> Cultures()
        {
            var list = new List<Culture>();

            // Get all language files info
            var dir = new DirectoryInfo(
                BoardContext.Current.Get<HttpRequestBase>().MapPath($"{BoardInfo.ForumServerFileRoot}languages"));
            var files = dir.GetFiles("*.xml");

            // Create an array with tags
            var tags = new string[2, files.Length];

            // Extract available language tags into the array
            for (var i = 0; i < files.Length; i++)
            {
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(files[i].FullName);
                    tags[0, i] = files[i].Name;
                    var attr = doc.DocumentElement.Attributes["code"];
                    if (attr != null)
                    {
                        tags[1, i] = attr.Value.Trim();
                    }
                    else
                    {
                        tags[1, i] = "en-US";
                    }
                }
                catch (Exception)
                {
                }
            }

            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            cultures.ForEach(
                ci =>
                {
                    for (var j = 0; j < files.Length; j++)
                    {
                        if (ci.IsNeutralCulture || !tags[1, j].ToLower().Substring(0, 2)
                                    .Contains(ci.TwoLetterISOLanguageName.ToLower()))
                        {
                            continue;
                        }

                        var item = new Culture
                                       {
                                           CultureTag = ci.IetfLanguageTag,
                                           CultureFile = tags[0, j],
                                           CultureEnglishName = ci.EnglishName,
                                           CultureNativeName = ci.NativeName,
                                           CultureDisplayName = ci.DisplayName
                                       };

                        list.Add(item);
                    }
                });

            return list;
        }

        /// <summary>
        /// The cultures IetfLangTags (4-letter).
        /// </summary>
        /// <returns>
        /// The cultures filtered by first 2 letters in the language tag in a language file
        /// </returns>
        public static IReadOnlyCollection<Culture> NeutralCultures()
        {
            var list = new List<Culture>();

            // Get all language files info
            var dir = new DirectoryInfo(
                BoardContext.Current.Get<HttpRequestBase>().MapPath($"{BoardInfo.ForumServerFileRoot}languages"));
            var files = dir.GetFiles("*.xml");

            // Create an array with tags
            var tags = new string[2, files.Length];

            // Extract available language tags into the array
            for (var i = 0; i < files.Length; i++)
            {
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(files[i].FullName);
                    tags[0, i] = files[i].Name;
                    var attr = doc.DocumentElement.Attributes["code"];
                    if (attr != null)
                    {
                        tags[1, i] = attr.Value.Trim();
                    }
                    else
                    {
                        tags[1, i] = "en-US";
                    }
                }
                catch (Exception)
                {
                }
            }

            var cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);

            cultures.ForEach(
                ci =>
                {
                    for (var j = 0; j < files.Length; j++)
                    {
                        if (!tags[1, j].ToLower().Substring(0, 2)
                                .Contains(ci.TwoLetterISOLanguageName.ToLower()))
                        {
                            continue;
                        }

                        var item = new Culture
                                       {
                                           CultureTag = ci.IetfLanguageTag,
                                           CultureFile = tags[0, j],
                                           CultureEnglishName = ci.EnglishName,
                                           CultureNativeName = ci.NativeName,
                                           CultureDisplayName = ci.DisplayName
                                       };

                        list.Add(item);
                    }
                });

            return list;
        }

        /// <summary>
        /// Gets language tag info from language file tag.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// A default full 4-letter culture from the existing language file.
        /// </returns>
        public static string CultureDefaultFromFile(string fileName)
        {
            if (fileName.IsNotSet())
            {
                return "en-US";
            }

            string rawTag;

            // Get all language files info
            var dir = new DirectoryInfo(
                BoardContext.Current.Get<HttpRequestBase>().MapPath($"{BoardInfo.ForumServerFileRoot}languages"));
            var files = dir.GetFiles(fileName);

            if (files.Length <= 0)
            {
                return null;
            }

            try
            {
                var doc = new XmlDocument();
                doc.Load(files[0].FullName);
                rawTag = doc.DocumentElement.Attributes["code"].Value.Trim();
            }
            catch (Exception)
            {
                return "en-US";
            }

            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            return cultures.FirstOrDefault(
                    ci => !ci.IsNeutralCulture
                          && rawTag.ToLower().Substring(0, 2).Contains(ci.TwoLetterISOLanguageName.ToLower())
                          && ci.IetfLanguageTag.Length == 5)
                ?.IetfLanguageTag;
        }

        /// <summary>
        /// Get All Themes
        /// </summary>
        /// <returns>
        /// Returns a Data Table with all Themes
        /// </returns>
        public static IReadOnlyCollection<string> Themes()
        {
            var dir = new DirectoryInfo(
                BoardContext.Current.Get<HttpRequestBase>().MapPath(
                    $"{BoardInfo.ForumServerFileRoot}/Content/Themes"));

            return dir.GetDirectories().Select(folder => folder.Name).ToList();
        }

        /// <summary>
        /// Get all time zones.
        /// </summary>
        /// <returns>
        /// Returns a Data Table with all time zones.
        /// </returns>
        public static IReadOnlyCollection<ListItem> TimeZones()
        {
            return TimeZones(TimeZoneInfo.GetSystemTimeZones());
        }

        /// <summary>
        /// Gets all Time Zones
        /// </summary>
        /// <param name="getSystemTimeZones">The get system time zones.</param>
        /// <returns>
        /// Returns a Data Table with all Time Zones
        /// </returns>
        public static IReadOnlyCollection<ListItem> TimeZones(IReadOnlyCollection<TimeZoneInfo> getSystemTimeZones)
        {
            var list = new List<ListItem>();

            getSystemTimeZones.ForEach(timeZoneInfo => list.Add(new ListItem(timeZoneInfo.DisplayName, timeZoneInfo.Id)));

            return list;
        }

        /// <summary>
        /// Gets all topic times.
        /// </summary>
        /// <returns>
        /// Returns a Data Table with all topic times.
        /// </returns>
        public static IReadOnlyCollection<ListItem> TopicTimes()
        {
            var list = new List<ListItem>();

            string[] textArray =
                {
                    "all", "last_day", "last_two_days", "last_week", "last_two_weeks", "last_month",
                    "last_two_months", "last_six_months", "last_year"
                };

            string[] textArrayProp =
                {
                    "All", "Last Day", "Last Two Days", "Last Week", "Last Two Weeks", "Last Month",
                    "Last Two Months", "Last Six Months", "Last Year"
                };

            for (var i = 0; i < 8; i++)
            {
                var item = new ListItem
                               {
                                   Name = BoardContext.Current.Get<ILocalization>().TransPage == null
                                              ? textArrayProp[i]
                                              : BoardContext.Current.Get<ILocalization>().GetText(textArray[i]),
                                   Value = i.ToString()
                               };

                list.Add(item);
            }

            return list;
        }

        #endregion
    }
}