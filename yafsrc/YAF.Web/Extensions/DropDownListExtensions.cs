/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Web.Extensions;

/// <summary>
/// The DropDown Extensions
/// </summary>
public static class DropDownListExtensions
{
    /// <summary>
    /// Add forum and category icons to dropdown list.
    /// </summary>
    /// <param name="dropDownList">
    ///     The drop down list.
    /// </param>
    /// <param name="forumList">
    ///     The forum list.
    /// </param>
    /// <param name="placeHolderText">the place holder text</param>
    public static void AddForumAndCategoryIcons(
        this DropDownList dropDownList,
        List<ForumSorted> forumList,
        string placeHolderText)
    {
        CodeContracts.VerifyNotNull(dropDownList);

        CodeContracts.VerifyNotNull(forumList);

        dropDownList.Items.Add(new ListItem(placeHolderText));

        forumList.ForEach(
            row =>
                {
                    // don't render categories
                    if (row.Icon == "folder")
                    {
                        return;
                    }

                    var item = new ListItem { Value = row.ForumID.ToString(), Text = row.Forum };

                    item.Attributes.Add("data-category", row.Category);

                    item.Attributes.Add(
                        "data-custom-properties",
                        $$"""{ "label": "<i class='fas fa-{{row.Icon}} fa-fw text-secondary me-1'></i>{{row.Forum}}", "url": "{{row.ForumLink}}" }""");

                    dropDownList.Items.Add(item);
                });
    }

    /// <summary>
    /// Get All Themes
    /// </summary>
    /// <returns>
    /// Returns a List with all Themes
    /// </returns>
    public static List<ListItem> AddThemeModes(this DropDownList dropDownList)
    {
        var modesList = new List<ListItem>();

        var modes = EnumExtensions.GetAllItems<ThemeMode>();

        var localization = BoardContext.Current.Get<ILocalization>();

        string[] textArray =
        {
            "sun", "moon"
        };

        modes.ForEach(
            mode =>
            {
                var text = localization.GetText("THEME_MODE", mode.ToString());
                var value = mode.ToInt();

                var item = new ListItem { Value = value.ToString(), Text = text };

                item.Attributes.Add(
                    "data-custom-properties",
                    $$"""{ "label": "<i class='far fa-{{textArray[value]}} fa-fw text-secondary me-1'></i>{{text}}" }""");

                dropDownList.Items.Add(item);
            });

        return modesList;
    }
}