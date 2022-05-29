/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;

using YAF.Types.Constants;
using YAF.Types.Objects;
using YAF.Types.Objects.Language;

/// <summary>
/// The static data helper.
/// </summary>
public static class StaticDataHelper
{
    public static IReadOnlyCollection<ListItem> TopicListModes()
    {
        return TopicListModes(BoardContext.Current.Get<ILocalization>());
    }

    public static IReadOnlyCollection<ListItem> TopicListModes([NotNull] ILocalization localization)
    {
        var modesList = new List<ListItem>();

        var modes = EnumExtensions.GetAllItems<TopicListMode>();

        modes.ForEach(
            mode => modesList.Add(
                new ListItem(
                    localization.GetText(
                        "MYTOPICS",
                        mode == TopicListMode.User ? "MYTOPICS" : $"{mode}Topics"),
                    mode.ToString())));

        return modesList;
    }

    public static IReadOnlyCollection<ListItem> Gender()
    {
        return Gender(BoardContext.Current.Get<ILocalization>());
    }

    /// <summary>
    /// The country.
    /// </summary>
    /// <param name="localization">
    /// The localization.
    /// </param>
    /// <returns>
    /// Returns a Data Table with all country names (localized).
    /// </returns>
    public static IReadOnlyCollection<ListItem> Gender([NotNull] ILocalization localization)
    {
        var genderList = new List<ListItem>();

        var genders = EnumExtensions.GetAllItems<Gender>();

        genders.ForEach(
            gender => genderList.Add(
                new ListItem(localization.GetText("GENDER", gender.ToString()), gender.ToString())));

        return genderList;
    }

    /// <summary>
    /// Gets all country names list(localized).
    /// </summary>
    /// <returns>
    /// Returns a List with all country names list(localized)
    /// </returns>
    public static IReadOnlyCollection<ListItem> Country()
    {
        return Country(BoardContext.Current.Get<ILocalization>());
    }

    /// <summary>
    /// The country.
    /// </summary>
    /// <param name="localization">
    /// The localization.
    /// </param>
    /// <returns>
    /// Returns a Data Table with all country names (localized).
    /// </returns>
    public static IReadOnlyCollection<ListItem> Country([NotNull] ILocalization localization)
    {
        var countriesList = new List<ListItem>();

        var item = new ListItem(localization.GetText("COMMON", "NONE"), null);

        countriesList.Add(item);

        var countries = localization.GetRegionNodesUsingQuery("COUNTRY", x => x.Tag.StartsWith(string.Empty))
            .OrderBy(c => c.Text).ToList();

        // vzrus: a temporary hack - it returns all tags if the page is not found
        if (countries.Count > 2000)
        {
            return countriesList;
        }

        countries.ForEach(node => countriesList.Add(new ListItem(node.Text, node.Tag)));

        return countriesList;
    }

    /// <summary>
    /// Gets all country names list(localized).
    /// </summary>
    /// <param name="localization">The localization.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>
    /// Returns a List with all country names list(localized)
    /// </returns>
    public static IReadOnlyCollection<ListItem> Region([NotNull] ILocalization localization, [NotNull] string culture)
    {
        var list = new List<ListItem> { new(null, null) };

        var countries = localization
            .GetCountryNodesUsingQuery("REGION", x => x.Tag.StartsWith($"RGN_{culture}_")).ToList();

        countries.ForEach(
            node => list.Add(new ListItem(node.Text, node.Tag.Replace($"RGN_{culture}_", string.Empty))));

        return list;
    }

    /// <summary>
    /// Gets all region names (localized)
    /// </summary>
    /// <param name="culture">The culture.</param>
    /// <returns>
    /// Returns a List with all region names (localized)
    /// </returns>
    public static IReadOnlyCollection<ListItem> Region([NotNull] string culture)
    {
        return Region(BoardContext.Current.Get<ILocalization>(), culture);
    }

    /// <summary>
    /// Gets all Language Files
    /// </summary>
    /// <returns>
    /// Returns a List with all Language Files
    /// </returns>
    public static IReadOnlyCollection<Culture> LanguageFiles()
    {
        var list = new List<Culture>();

        // Get all language files info
        var dir = new DirectoryInfo(
            BoardContext.Current.Get<HttpRequestBase>().MapPath($"{BoardInfo.ForumServerFileRoot}languages"));
        var files = dir.GetFiles("*.json");

        var resources = new List<LanguageResource>();

        var tagsCount = 0;

        files.ForEach(file =>
        {
            var languageResource = BoardContext.Current.Get<ILocalization>().LoadLanguageFile(file.FullName);

            if (file.Name == "english.json")
            {
                tagsCount = languageResource.Resources.Page.Sum(page => page.Resource.Count);
            }

            resources.Add(languageResource);

            list.Add(new Culture
                         {
                             CultureTag = languageResource.Resources.Code,
                             CultureFile = file.Name,
                             CultureEnglishName = languageResource.Resources.Language
                         });
        });

        var sourceResources = resources.FirstOrDefault(x => x.Resources.Code == "en");

        resources.ForEach(
            resource =>
            {
                var countTranslated = sourceResources.Resources.Page.Sum(
                    sourcePage => (from sourceResource in sourcePage.Resource
                                   let translatePage =
                                       resource.Resources.Page.FirstOrDefault(p => p.Name == sourcePage.Name)
                                   let translateResource =
                                       translatePage.Resource.FirstOrDefault(r => r.Tag == sourceResource.Tag)
                                   where !string.Equals(
                                             sourceResource.Text,
                                             translateResource.Text,
                                             StringComparison.InvariantCultureIgnoreCase)
                                   select sourceResource).Count());

                if (resource.Resources.Language == "English")
                {
                    countTranslated = tagsCount;
                }

                list.First(x => x.CultureTag == resource.Resources.Code).TranslatedPercentage = countTranslated * 100 / tagsCount;

            });

        return list;
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
        var files = dir.GetFiles("*.json");

        // Create an array with tags
        var tags = new Dictionary<string, string>();

        // Extract available language tags into the array
        files.ForEach(file =>
        {
            using var fileContent = File.OpenText(file.FullName);
            using var reader = new JsonTextReader(fileContent);
            var serializer = new JsonSerializer();
            var json = serializer.Deserialize<LanguageResource>(reader);

            tags.Add(file.Name, json.Resources.Code.IsSet() ? json.Resources.Code : "en-US");
        });

        var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

        cultures.ForEach(
            ci =>
            {
                list.AddRange(
                    from tag in tags
                    where !ci.IsNeutralCulture && tag.Value.ToLower().Substring(0, 2)
                              .Contains(ci.TwoLetterISOLanguageName.ToLower())
                    select new Culture {
                                           CultureTag = ci.IetfLanguageTag,
                                           CultureFile = tag.Key,
                                           CultureEnglishName = ci.EnglishName,
                                           CultureNativeName = ci.NativeName,
                                           CultureDisplayName = ci.DisplayName
                                       });
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
        var files = dir.GetFiles("*.json");

        // Create an array with tags
        var tags = new Dictionary<string, string>();

        // Extract available language tags into the array
        files.ForEach(file =>
        {
            using var fileContent = File.OpenText(file.FullName);
            using var reader = new JsonTextReader(fileContent);
            var serializer = new JsonSerializer();
            var json = serializer.Deserialize<LanguageResource>(reader);

            tags.Add(file.Name, json.Resources.Code.IsSet() ? json.Resources.Code : "en-US");
        });

        var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

        cultures.ForEach(
            ci =>
            {
                list.AddRange(
                    from tag in tags
                    where tag.Value.ToLower().Substring(0, 2)
                              .Contains(ci.TwoLetterISOLanguageName.ToLower())
                    select new Culture
                           {
                               CultureTag = ci.IetfLanguageTag,
                               CultureFile = tag.Key,
                               CultureEnglishName = ci.EnglishName,
                               CultureNativeName = ci.NativeName,
                               CultureDisplayName = ci.DisplayName
                           });
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
    public static string CultureDefaultFromFile([CanBeNull] string fileName)
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
            var resource = BoardContext.Current.Get<ILocalization>().LoadLanguageFile(files[0].FullName);
            rawTag = resource.Resources.Code;
        }
        catch (Exception)
        {
            return "en-US";
        }

        var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

        var tag = cultures.FirstOrDefault(
            ci => !ci.IsNeutralCulture
                  && rawTag.ToLower().Substring(0, 2).Contains(ci.TwoLetterISOLanguageName.ToLower())
                  && ci.IetfLanguageTag.Length == 5)?.IetfLanguageTag;

        return tag ?? "en-US";
    }

    /// <summary>
    /// Get All Themes
    /// </summary>
    /// <returns>
    /// Returns a List with all Themes
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
    /// Returns a List with all time zones.
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
    /// Returns a List with all Time Zones
    /// </returns>
    public static IReadOnlyCollection<ListItem> TimeZones([NotNull] IReadOnlyCollection<TimeZoneInfo> getSystemTimeZones)
    {
        var list = new List<ListItem>();

        getSystemTimeZones.ForEach(timeZoneInfo => list.Add(new ListItem(timeZoneInfo.DisplayName, timeZoneInfo.Id)));

        return list;
    }

    /// <summary>
    /// Gets all topic times.
    /// </summary>
    /// <returns>
    /// Returns a List with all topic times.
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

    /// <summary>
    /// Gets the List with Page Entries to Show
    /// </summary>
    /// <returns>
    /// The <see cref="List"/>.
    /// </returns>
    public static List<ListItem> PageEntries()
    {
        var list = new List<ListItem>();

        string[] textArray =
            {
                "ENTRIES_5",
                "ENTRIES_10",
                "ENTRIES_20",
                "ENTRIES_25",
                "ENTRIES_50",
            };

        textArray.ForEach(
            text =>
                {
                    var item = new ListItem
                                   {
                                       Name = BoardContext.Current.Get<ILocalization>().GetText("COMMON", text),
                                       Value = text.Replace("ENTRIES_", string.Empty)
                                   };

                    list.Add(item);
                });

        return list;
    }
}