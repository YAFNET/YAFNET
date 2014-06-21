/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Xml;
    using YAF.Classes;
    using YAF.Core.Services.Localization;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    /// <summary>
    /// The static data helper.
    /// </summary>
    public class StaticDataHelper
    {
        #region Public Methods

        /// <summary>
        /// The allow disallow.
        /// </summary>
        /// <returns>
        /// Returns a Data Table with allow disallow values.
        /// </returns>
        public static DataTable AllowDisallow()
        {
            using (var dt = new DataTable("AllowDisallow"))
            {
                dt.Columns.Add("Text", typeof(string));
                dt.Columns.Add("Value", typeof(int));

                string[] tTextArray =
                    {
                        YafContext.Current.Get<ILocalization>().GetText("COMMON", "ALLOWED"),
                        YafContext.Current.Get<ILocalization>().GetText("COMMON", "DISALLOWED")
                    };

                for (int i = 0; i < tTextArray.Length; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["Text"] = tTextArray[i];
                    dr["Value"] = i;
                    dt.Rows.Add(dr);
                }

                return dt;
            }
        }

        /// <summary>
        /// Gets all country names (localized).
        /// </summary>
        /// <param name="localization">
        /// The localization.
        /// </param>
        /// <returns>
        /// Returns a Data Table with all country names (localized).
        /// </returns>
        public static DataTable Country(ILocalization localization)
        {
            using (var dt = new DataTable("Country"))
            {
                dt.Columns.Add("Value", typeof(string));
                dt.Columns.Add("Name", typeof(string));

                // Add empty row to data table for dropdown lists with empty selection option.
                var drow = dt.NewRow();
                drow["Value"] = null;
                drow["Name"] = "None";
                dt.Rows.Add(drow);

                var countries =
                    localization.GetRegionNodesUsingQuery("COUNTRY", x => x.tag.StartsWith(string.Empty))
                        .OrderBy(c => c.Value)
                        .ToList();

                // vzrus: a temporary hack - it returns all tags if the page is not found
                if (countries.Count > 2000)
                {
                    return dt;
                }

                foreach (var node in countries)
                {
                    dt.Rows.Add(new object[] { node.tag, node.Value });
                }

                return dt;
            }
        }

        /// <summary>
        /// Gets all country names list(localized).
        /// </summary>
        /// <returns>
        /// Returns a Data Table with all country names list(localized)
        /// </returns>
        public static DataTable Country()
        {
            return Country(YafContext.Current.Get<ILocalization>());
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
        public static DataTable Country(string forceLanguage)
        {
            var localization = new YafLocalization();
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
        public static DataTable Region(ILocalization localization, string culture)
        {
            using (var dt = new DataTable("Region"))
            {
                dt.Columns.Add("Value", typeof(string));
                dt.Columns.Add("Name", typeof(string));

                // Add empty row to data table for dropdown lists with empty selection option.
                var drow = dt.NewRow();

                drow["Value"] = null;
                drow["Name"] = null;

                dt.Rows.Add(drow);

                var countries =
                 localization.GetCountryNodesUsingQuery("REGION", x => x.tag.StartsWith("RGN_{0}_".FormatWith(culture))).ToList();

                foreach (var node in countries)
                {
                    dt.Rows.Add(new object[] { node.tag.Replace("RGN_{0}_".FormatWith(culture), string.Empty), node.Value });
                }

                return dt;
            }
        }

        /// <summary>
        /// Gets all region names (localized)
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// Returns a Data Table with all region names (localized)
        /// </returns>
        public static DataTable Region(string culture)
        {
            return Region(YafContext.Current.Get<ILocalization>(), culture);
        }

        /// <summary>
        /// The cultures IetfLangTags (4-letter).
        /// </summary>
        /// <returns>
        /// The cultures filtered by first 2 letters in the language tag in a language file
        /// </returns>
        public static DataTable Cultures()
        {
            using (var dt = new DataTable("Cultures"))
            {
                dt.Columns.Add("CultureTag", typeof(string));
                dt.Columns.Add("CultureFile", typeof(string));
                dt.Columns.Add("CultureEnglishName", typeof(string));
                dt.Columns.Add("CultureNativeName", typeof(string));
                dt.Columns.Add("CultureDisplayName", typeof(string));

                // Get all language files info
                var dir = new DirectoryInfo(YafContext.Current.Get<HttpRequestBase>().MapPath("{0}languages".FormatWith(YafForumInfo.ForumServerFileRoot)));
                FileInfo[] files = dir.GetFiles("*.xml");

                // Create an array with tags
                var tags = new string[2, files.Length];

                // Extract availabe language tags into the array          
                for (int i = 0; i < files.Length; i++)
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

                CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
                foreach (CultureInfo ci in cultures)
                {
                    for (int j = 0; j < files.Length; j++)
                    {
                        if (ci.IsNeutralCulture || !tags[1, j].ToLower().Substring(0, 2).Contains(ci.TwoLetterISOLanguageName.ToLower()))
                        {
                            continue;
                        }

                        DataRow dr = dt.NewRow();
                        dr["CultureTag"] = ci.IetfLanguageTag;
                        dr["CultureFile"] = tags[0, j];
                        dr["CultureEnglishName"] = ci.EnglishName;
                        dr["CultureNativeName"] = ci.NativeName;
                        dr["CultureDisplayName"] = ci.DisplayName;
                        dt.Rows.Add(dr);
                    }
                }

                return dt;
            }
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

            string rawTag = null;

            // Get all language files info
            var dir =
              new DirectoryInfo(
                YafContext.Current.Get<HttpRequestBase>().MapPath("{0}languages".FormatWith(YafForumInfo.ForumServerFileRoot)));
            FileInfo[] files = dir.GetFiles(fileName);

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
            }

            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (
                CultureInfo ci in
                    cultures.Where(
                        ci =>
                        !ci.IsNeutralCulture
                        && rawTag.ToLower().Substring(0, 2).Contains(ci.TwoLetterISOLanguageName.ToLower())
                        && ci.IetfLanguageTag.Length == 5))
            {
                return ci.IetfLanguageTag;
            }

            return "en-US";
        }

        /// <summary>
        /// The languages.
        /// </summary>
        /// <returns>
        /// Returns a Data Table with all Languages
        /// </returns>
        public static DataTable Languages()
        {
            using (var dt = new DataTable("Languages"))
            {
                dt.Columns.Add("Language", typeof(string));
                dt.Columns.Add("FileName", typeof(string));

                var dir =
                  new DirectoryInfo(
                    YafContext.Current.Get<HttpRequestBase>().MapPath("{0}languages".FormatWith(YafForumInfo.ForumServerFileRoot)));
                FileInfo[] files = dir.GetFiles("*.xml");
                foreach (FileInfo file in files)
                {
                    try
                    {
                        var doc = new XmlDocument();
                        doc.Load(file.FullName);
                        DataRow dr = dt.NewRow();
                        dr["Language"] = doc.DocumentElement.Attributes["language"].Value;
                        dr["FileName"] = file.Name;
                        dt.Rows.Add(dr);
                    }
                    catch (Exception)
                    {
                    }
                }

                return dt;
            }
        }

        /// <summary>
        /// Get All Themes
        /// </summary>
        /// <returns>
        /// Returns a Data Table with all Themes
        /// </returns>
        public static DataTable Themes()
        {
            using (var dt = new DataTable("Themes"))
            {
                dt.Columns.Add("Theme", typeof(string));
                dt.Columns.Add("FileName", typeof(string));
                dt.Columns.Add("IsMobile", typeof(bool));

                var dir = new DirectoryInfo(YafContext.Current.Get<HttpRequestBase>().MapPath("{0}{1}".FormatWith(YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Themes)));

                foreach (FileInfo file in dir.GetFiles("*.xml"))
                {
                    try
                    {
                        var doc = new XmlDocument();
                        doc.Load(file.FullName);

                        DataRow dr = dt.NewRow();
                        dr["Theme"] = doc.DocumentElement.Attributes["theme"].Value;
                        dr["IsMobile"] = false;

                        if (doc.DocumentElement.HasAttribute("ismobile"))
                        {
                            dr["IsMobile"] = Convert.ToBoolean(doc.DocumentElement.Attributes["ismobile"].Value ?? "false");
                        }

                        dr["FileName"] = file.Name;
                        dt.Rows.Add(dr);
                    }
                    catch (Exception)
                    {
                    }
                }

                return dt;
            }
        }

        /// <summary>
        /// Gets all jQuery-UI Themes
        /// </summary>
        /// <returns>
        /// Returns a Data Table with all jQuery-UI Themes
        /// </returns>
        public static DataTable JqueryUIThemes()
        {
            using (var dt = new DataTable("JqueryUIThemes"))
            {
                dt.Columns.Add("Theme", typeof(string));

                var themeDir = new DirectoryInfo(HttpContext.Current.Request.MapPath(YafForumInfo.GetURLToResource("css/jquery-ui-themes")));

                foreach (DirectoryInfo dir in themeDir.GetDirectories())
                {
                    try
                    {
                        DataRow dr = dt.NewRow();
                        dr["Theme"] = dir.Name;

                        dt.Rows.Add(dr);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                return dt;
            }
        }

        /// <summary>
        /// Gets all Time Zones
        /// </summary>
        /// <param name="localization">
        /// The localization.
        /// </param>
        /// <returns>
        /// Returns a Data Table with all Time Zones
        /// </returns>
        public static DataTable TimeZones(ILocalization localization)
        {
            using (var dt = new DataTable("TimeZone"))
            {
                dt.Columns.Add("Value", typeof(string));
                dt.Columns.Add("Name", typeof(string));

                var timezones =
                  localization.GetNodesUsingQuery("TIMEZONES", x => x.tag.StartsWith("UTC")).ToList();

                timezones.Sort(new YafLanguageNodeComparer());

                foreach (var node in timezones)
                {
                    dt.Rows.Add(new object[] { node.GetHoursOffset() * 60, node.Value });
                }

                return dt;
            }
        }

        /// <summary>
        /// Get all time zones.
        /// </summary>
        /// <returns>
        /// Returns a Data Table with all time zones.
        /// </returns>
        public static DataTable TimeZones()
        {
            return TimeZones(YafContext.Current.Get<ILocalization>());
        }

        /// <summary>
        /// Get all time zones.
        /// </summary>
        /// <param name="forceLanguage">
        /// The force language.
        /// </param>
        /// <returns>
        /// Returns a Data Table with all time zones.
        /// </returns>
        public static DataTable TimeZones(string forceLanguage)
        {
            var localization = new YafLocalization();
            localization.LoadTranslation(forceLanguage);

            return TimeZones(localization);
        }

        /// <summary>
        /// Gets all topic times.
        /// </summary>
        /// <returns>
        /// Returns a Data Table with all topic times.
        /// </returns>
        public static DataTable TopicTimes()
        {
            using (var dt = new DataTable("TopicTimes"))
            {
                dt.Columns.Add("TopicText", typeof(string));
                dt.Columns.Add("TopicValue", typeof(int));

                string[] tTextArray =
                    {
                        "all", "last_day", "last_two_days", "last_week", "last_two_weeks", "last_month",
                        "last_two_months", "last_six_months", "last_year"
                    };

                string[] tTextArrayProp =
                    {
                        "All", "Last Day", "Last Two Days", "Last Week", "Last Two Weeks",
                        "Last Month", "Last Two Months", "Last Six Months", "Last Year"
                    };

                for (int i = 0; i < 8; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["TopicText"] = (YafContext.Current.Get<ILocalization>().TransPage == null)
                                        ? tTextArrayProp[i]
                                        : YafContext.Current.Get<ILocalization>().GetText(tTextArray[i]);
                    dr["TopicValue"] = i;
                    dt.Rows.Add(dr);
                }

                return dt;
            }
        }

        #endregion
    }

    /// <summary>
    /// The YAF language node comparer.
    /// </summary>
    public class YafLanguageNodeComparer : IComparer<LanuageResourcesPageResource>
    {
        #region IComparer<XmlNode>

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.
        /// </returns>
        public int Compare(LanuageResourcesPageResource x, LanuageResourcesPageResource y)
        {
            return x.GetHoursOffset().CompareTo(y.GetHoursOffset());
        }

        #endregion
    }
}
