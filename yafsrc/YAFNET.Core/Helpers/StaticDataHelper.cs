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

using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.AspNetCore.Mvc.Rendering;

using Newtonsoft.Json;

using YAF.Types.Objects;
using YAF.Types.Objects.Language;

/// <summary>
/// The static data helper.
/// </summary>
public static class StaticDataHelper
{
    /// <summary>
    /// Loads the help menu json.
    /// </summary>
    /// <returns>List&lt;HelpNavigation&gt;.</returns>
    public static List<HelpNavigation> LoadHelpMenuJson()
    {
        var json = File.ReadAllText(Path.Combine(BoardContext.Current.Get<BoardInfo>().WebRootPath, "resources", "helpMenu.json"));

        return JsonConvert.DeserializeObject<List<HelpNavigation>>(json);
    }

    /// <summary>
    /// Gets the Friend list modes.
    /// </summary>
    /// <returns>IReadOnlyCollection&lt;SelectListItem&gt;.</returns>
    public static IReadOnlyCollection<SelectListItem> FriendListModes()
    {
        var modesList = new List<SelectListItem>();

        var modes = EnumExtensions.GetAllItems<FriendMode>();

        var localization = BoardContext.Current.Get<ILocalization>();

        modes.ForEach(mode =>
            {
                switch (mode)
                {
                    case FriendMode.Friends:
                        modesList.Add(
                            new SelectListItem(localization.GetText("FRIENDS", "BUDDYLIST"), mode.ToInt().ToString()));
                        break;
                    case FriendMode.ReceivedRequests:
                        modesList.Add(
                            new SelectListItem(
                                localization.GetText("FRIENDS", "PENDING_REQUESTS"),
                                mode.ToInt().ToString()));
                        break;
                    case FriendMode.SendRequests:
                        modesList.Add(
                            new SelectListItem(
                                localization.GetText("FRIENDS", "YOUR_REQUESTS"),
                                mode.ToInt().ToString()));
                        break;
                }
            });

        return modesList;
    }

    /// <summary>
    /// Gets the My Topics Page Topic List Modes.
    /// </summary>
    /// <returns>IReadOnlyCollection&lt;SelectListItem&gt;.</returns>
    public static IReadOnlyCollection<SelectListItem> TopicListModes()
    {
        var localization = BoardContext.Current.Get<ILocalization>();

        var modesList = new List<SelectListItem>();

        var modes = EnumExtensions.GetAllItems<TopicListMode>();

        modes.ForEach(
            mode => modesList.Add(
                new SelectListItem(
                    localization.GetText("MYTOPICS", mode == TopicListMode.User ? "MYTOPICS" : $"{mode}Topics"),
                    mode.ToInt().ToString())));

        return modesList;
    }

    /// <summary>
    /// Gets the Gender List
    /// </summary>
    /// <returns>
    /// Returns a Select List with all country names (localized).
    /// </returns>
    public static IReadOnlyCollection<SelectListItem> Gender()
    {
        var localization = BoardContext.Current.Get<ILocalization>();

        var genderList = new List<SelectListItem>();

        var genders = EnumExtensions.GetAllItems<Gender>();

        genders.ForEach(
            gender => genderList.Add(
                new SelectListItem(localization.GetText("GENDER", gender.ToString()), gender.ToString())));

        return genderList;
    }

    /// <summary>
    /// Gets all country names list(localized).
    /// </summary>
    /// <returns>
    /// Returns a List with all country names list(localized)
    /// </returns>
    public static IReadOnlyCollection<SelectListItem> Countries()
    {
        var localization = BoardContext.Current.Get<ILocalization>();

        var countriesList = new List<SelectListItem>();

        var item = new SelectListItem(localization.GetText("COMMON", "NONE"), null);

        countriesList.Add(item);

        var countries = localization.GetNodesUsingQuery("COUNTRY", x => x.Tag.StartsWith(string.Empty))
            .OrderBy(c => c.Text).ToList();

        if (countries.Count > 2000)
        {
            return countriesList;
        }

        countries.ForEach(node => countriesList.Add(new SelectListItem(node.Text, node.Tag)));

        return countriesList;
    }

    /// <summary>
    /// Gets all region names (localized)
    /// </summary>
    /// <param name="culture">The culture.</param>
    /// <returns>
    /// Returns a Data Table with all region names (localized)
    /// </returns>
    public static IReadOnlyCollection<SelectListItem> Regions(string culture)
    {
        return Regions(BoardContext.Current.Get<ILocalization>(), culture);
    }

    /// <summary>
    /// Gets all country names list(localized).
    /// </summary>
    /// <param name="localization">The localization.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>
    /// Returns a Data Table with all country names list(localized)
    /// </returns>
    public static IReadOnlyCollection<SelectListItem> Regions(ILocalization localization, string culture)
    {
        var list = new List<SelectListItem> { new(localization.GetText("COMMON", "NONE"), null) };

        var countries = localization.GetNodesUsingQuery("REGION", x => x.Tag.StartsWith($"RGN_{culture}_")).ToList();

        countries.ForEach(
            node => list.Add(new SelectListItem(node.Text, node.Tag.Replace($"RGN_{culture}_", string.Empty))));

        return list;
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

        var webRootPath = BoardContext.Current.Get<BoardInfo>().WebRootPath;

        // Get all language files info
        var dir = new DirectoryInfo(Path.Combine(webRootPath, "languages"));

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

        var sourceResources = resources.First(x => x.Resources.Code == "en");

        resources.ForEach(
            resource =>
            {
                var countTranslated = sourceResources.Resources.Page.Sum(
                    sourcePage => (sourcePage.Resource
                        .Select(sourceResource => new {
                            sourceResource,
                            translatePage = resource.Resources.Page.First(p => p.Name == sourcePage.Name)
                        })
                        .Select(t => new {
                            t,
                            translateResource =
                                t.translatePage.Resource.First(r => r.Tag == t.sourceResource.Tag)
                        })
                        .Where(t => !string.Equals(t.t.sourceResource.Text, t.translateResource.Text,
                            StringComparison.InvariantCultureIgnoreCase))
                        .Select(t => t.t.sourceResource)).Count());

                if (resource.Resources.Language == "English")
                {
                    countTranslated = tagsCount;
                }

                var resourceFile = list.First(x => x.CultureTag == resource.Resources.Code);

                resourceFile.TranslatedPercentage = countTranslated * 100 / tagsCount;
                resourceFile.TranslatedCount = countTranslated;
                resourceFile.TagsCount = tagsCount;
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

        var webRootPath = BoardContext.Current.Get<BoardInfo>().WebRootPath;

        // Get all language files info
        var dir = new DirectoryInfo(Path.Combine(webRootPath, "languages"));

        var files = dir.GetFiles("*.json");

        var tags = new Dictionary<string, string>();

        files.ForEach(
            file =>
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
                        where !ci.IsNeutralCulture && tag.Value.ToLower()[..2]
.Contains(ci.TwoLetterISOLanguageName, StringComparison.CurrentCultureIgnoreCase)
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
    /// Gets the available Languages
    /// </summary>
    public static IReadOnlyCollection<SelectListItem> Languages()
    {
        var list = new List<SelectListItem>();

        var webRootPath = BoardContext.Current.Get<BoardInfo>().WebRootPath;

        // Get all language files info
        var dir = new DirectoryInfo(Path.Combine(webRootPath, "languages"));

        var files = dir.GetFiles("*.json");

        var tags = new Dictionary<string, string>();

        files.ForEach(
            file =>
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
                        where !ci.IsNeutralCulture && tag.Value.ToLower()[..2]
                                  .Contains(ci.TwoLetterISOLanguageName.ToLower())
                        select new SelectListItem {Text = ci.NativeName, Value = ci.IetfLanguageTag});
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

        var webRootPath = BoardContext.Current.Get<BoardInfo>().WebRootPath;

        // Get all language files info
        var dir = new DirectoryInfo(Path.Combine(webRootPath, "languages"));

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
                        where tag.Value.ToLower()[..2]
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
    public static string CultureDefaultFromFile(string fileName)
    {
        if (fileName.IsNotSet())
        {
            return "en-US";
        }

        string rawTag;

        var webRootPath = BoardContext.Current.Get<BoardInfo>().WebRootPath;

        // Get all language files info
        var dir = new DirectoryInfo(Path.Combine(webRootPath, "languages"));

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

        var tag = cultures.Find(
            ci => !ci.IsNeutralCulture
                  && rawTag.ToLower()[..2].Contains(ci.TwoLetterISOLanguageName.ToLower())
                  && ci.IetfLanguageTag.Length == 5)?.IetfLanguageTag;

        return tag ?? "en-US";
    }

    /// <summary>
    /// Get All Themes
    /// </summary>
    /// <returns>
    /// Returns a Data Table with all Themes
    /// </returns>
    public static IReadOnlyCollection<SelectListItem> Themes()
    {
        var webRootPath = BoardContext.Current.Get<BoardInfo>().WebRootPath;

        var dir = new DirectoryInfo(Path.Combine(webRootPath, "css", "themes"));

        var list = new List<SelectListItem>();

        dir.GetDirectories().Select(folder => folder.Name)
            .ForEach(theme => list.Add(new SelectListItem(theme, theme)));

        return list;
    }

    /// <summary>
    /// Get all time zones.
    /// </summary>
    /// <returns>
    /// Returns a Data Table with all time zones.
    /// </returns>
    public static IReadOnlyCollection<SelectListItem> TimeZones()
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
    public static IReadOnlyCollection<SelectListItem> TimeZones(
        IReadOnlyCollection<TimeZoneInfo> getSystemTimeZones)
    {
        var list = new List<SelectListItem>();

        getSystemTimeZones.ForEach(
            timeZoneInfo => list.Add(new SelectListItem(timeZoneInfo.DisplayName, timeZoneInfo.Id)));

        return list;
    }

    /// <summary>
    /// Gets all topic times.
    /// </summary>
    /// <returns>
    /// Returns a Data Table with all topic times.
    /// </returns>
    public static IReadOnlyCollection<SelectListItem> TopicTimes()
    {
        var list = new List<SelectListItem>();

        string[] textArray = [
            "all", "last_day", "last_two_days", "last_week", "last_two_weeks", "last_month", "last_two_months",
                "last_six_months", "last_year"
        ];

        string[] textArrayProp = [
            "All", "Last Day", "Last Two Days", "Last Week", "Last Two Weeks", "Last Month", "Last Two Months",
                "Last Six Months", "Last Year"
        ];

        for (var i = 0; i < 8; i++)
        {
            var item = new SelectListItem
                           {
                               Text = BoardContext.Current.Get<ILocalization>().TransPage == null
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
    /// Returns List with Page Entries
    /// </returns>
    public static List<SelectListItem> PageEntries()
    {
        var list = new List<SelectListItem>();

        string[] textArray = ["ENTRIES_5", "ENTRIES_10", "ENTRIES_20", "ENTRIES_25", "ENTRIES_50"];

        textArray.ForEach(
            text =>
                {
                    var item = new SelectListItem
                                   {
                                       Text = BoardContext.Current.Get<ILocalization>().GetText("COMMON", text),
                                       Value = text.Replace("ENTRIES_", string.Empty)
                                   };

                    list.Add(item);
                });

        return list;
    }

    /// <summary>
    /// Gets the topic priorities.
    /// </summary>
    /// <returns>IReadOnlyCollection&lt;SelectListItem&gt;.</returns>
    public static IReadOnlyCollection<SelectListItem> TopicPriorities()
    {
        var list = new List<SelectListItem>();

        var normal = new SelectListItem(BoardContext.Current.Get<ILocalization>().GetText("normal"), "0");

        list.Add(normal);

        var sticky = new SelectListItem(BoardContext.Current.Get<ILocalization>().GetText("sticky"), "1");

        list.Add(sticky);

        var announcement = new SelectListItem(BoardContext.Current.Get<ILocalization>().GetText("announcement"), "2");

        list.Add(announcement);

        return list;
    }

    /// <summary>
    /// Get a string array of the supported image file types.
    /// </summary>
    /// <returns>System.String[].</returns>
    public static string[] ImageFormats => ["webp", "jpg", "jpeg", "gif", "png", "bmp"];
}