/* Yet Another Forum.net
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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
        /// </returns>
        public static DataTable AllowDisallow()
        {
            using (var dt = new DataTable("AllowDisallow"))
            {
                dt.Columns.Add("Text", typeof(string));
                dt.Columns.Add("Value", typeof(int));

                string[] tTextArray = {
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
        /// The country names list(localized).
        /// </summary>
        /// <param name="localization">
        /// The localization.
        /// </param>
        /// <param name="addEmptyRow">
        /// Add empty row to data table for dropdown lists with empty selection option.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable Country(ILocalization localization)
        {
            using (var dt = new DataTable("Country"))
            {
                dt.Columns.Add("Value", Type.GetType("System.String"));
                dt.Columns.Add("Name", Type.GetType("System.String"));

                // Add empty row to data table for dropdown lists with empty selection option.
                bool addEmptyRow = true;

                if (addEmptyRow)
                {
                    var drow = dt.NewRow();
                    drow["Value"] = null;
                    drow["Name"] = "None";
                    dt.Rows.Add(drow);
                }

                var countries =
                    localization.GetRegionNodesUsingQuery("COUNTRY", x => x.tag.StartsWith(string.Empty)).ToList();

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
        /// The country names list(localized).
        /// </summary>
        /// <returns>
        /// </returns>
        public static DataTable Country()
        {
            return Country(YafContext.Current.Get<ILocalization>());
        }

        /// <summary>
        /// The country names list(localized).
        /// </summary>
        /// <param name="forceLanguage">
        /// The force a specific language.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable Country(string forceLanguage)
        {
            var localization = new YafLocalization();
            localization.LoadTranslation(forceLanguage);

            return Country(localization);
        }

        /// <summary>
        /// The country names list(localized).
        /// </summary>
        /// <param name="localization">
        /// The localization.
        /// </param>
        /// <param name="addEmptyRow">
        /// Add empty row to data table for dropdown lists with empty selection option.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable Region(ILocalization localization, string culture)
        {
            using (var dt = new DataTable("Region"))
            {
                dt.Columns.Add("Value", Type.GetType("System.String"));
                dt.Columns.Add("Name", Type.GetType("System.String"));

                // Add empty row to data table for dropdown lists with empty selection option.
                bool addEmptyRow = true;
                if (addEmptyRow)
                {
                    var drow = dt.NewRow();
                    drow["Value"] = null;
                    drow["Name"] = null;
                    dt.Rows.Add(drow);
                }

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
        /// The country names list(localized).
        /// </summary>
        /// <returns>
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
        /// <param name="fileName"></param>
        /// <returns>A default full 4-letter culture from the existing language file.</returns>
        public static string CultureDefaultFromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return "en-US";

            string rawTag = null;
            // Get all language files info
            var dir =
              new DirectoryInfo(
                YafContext.Current.Get<HttpRequestBase>().MapPath("{0}languages".FormatWith(YafForumInfo.ForumServerFileRoot)));
            FileInfo[] files = dir.GetFiles(fileName);

            if (files.Length <= 0) return rawTag;
            try
            {
                var doc = new XmlDocument();
                doc.Load(files[0].FullName);
                rawTag = doc.DocumentElement.Attributes["code"].Value.Trim();

            }
            catch (Exception)
            {
            }

            System.Globalization.CultureInfo[] cultures = System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures);
            foreach (System.Globalization.CultureInfo ci in cultures)
            {
                // We check only the language part as we need a default here.
                if (!ci.IsNeutralCulture && rawTag.ToLower().Substring(0, 2).Contains(ci.TwoLetterISOLanguageName.ToLower()) && ci.IetfLanguageTag.Length == 5)
                {
                    return ci.IetfLanguageTag;
                }
            }

            return "en-US";
        }

        /// <summary>
        /// The languages.
        /// </summary>
        /// <returns>
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
        /// The themes.
        /// </summary>
        /// <returns>
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
        /// The themes.
        /// </summary>
        /// <returns>
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
        /// The time zones.
        /// </summary>
        /// <param name="localization">
        /// The localization.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable TimeZones(ILocalization localization)
        {
            using (var dt = new DataTable("TimeZone"))
            {
                dt.Columns.Add("Value", Type.GetType("System.Int32"));
                dt.Columns.Add("Name", Type.GetType("System.String"));

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
        /// The time zones.
        /// </summary>
        /// <returns>
        /// </returns>
        public static DataTable TimeZones()
        {
            return TimeZones(YafContext.Current.Get<ILocalization>());
        }

        /// <summary>
        /// The time zones.
        /// </summary>
        /// <param name="forceLanguage">
        /// The force language.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable TimeZones(string forceLanguage)
        {
            var localization = new YafLocalization();
            localization.LoadTranslation(forceLanguage);

            return TimeZones(localization);
        }

        /// <summary>
        /// The topic times.
        /// </summary>
        /// <returns>
        /// </returns>
        public static DataTable TopicTimes()
        {
            using (var dt = new DataTable("TopicTimes"))
            {
                dt.Columns.Add("TopicText", typeof(string));
                dt.Columns.Add("TopicValue", typeof(int));

                string[] tTextArray = {
                                "all", "last_day", "last_two_days", "last_week", "last_two_weeks", "last_month", 
                                "last_two_months", "last_six_months", "last_year"
                              };
                string[] tTextArrayProp = {
                                    "All", "Last Day", "Last Two Days", "Last Week", "Last Two Weeks", "Last Month", 
                                    "Last Two Months", "Last Six Months", "Last Year"
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
    /// The yaf language node comparer.
    /// </summary>
    public class YafLanguageNodeComparer : IComparer<LanuageResourcesPageResource>
    {
        #region IComparer<XmlNode>

        /// <summary>
        /// The compare.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The compare.
        /// </returns>
        public int Compare(LanuageResourcesPageResource x, LanuageResourcesPageResource y)
        {
            return x.GetHoursOffset().CompareTo(y.GetHoursOffset());
        }

        #endregion
    }
}
