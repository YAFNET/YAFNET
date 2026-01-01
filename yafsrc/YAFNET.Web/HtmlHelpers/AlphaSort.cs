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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Web.HtmlHelpers;

/// <summary>
/// Class AlphaSortHtmlHelper.
/// </summary>
public static class AlphaSortHtmlHelper
{
    /// <summary>
    /// Render Member list alpha sort
    /// </summary>
    /// <param name="htmlHelper">
    ///     The html helper.
    /// </param>
    /// <param name="userList">
    /// The user list.
    ///  </param>
    /// <param name="currentLetter">
    /// The current selected letter.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent AlphaSort(this IHtmlHelper htmlHelper, List<PagedUser> userList, char currentLetter)
    {
        var content = new HtmlContentBuilder();

        var context = BoardContext.Current;

        var buttonGroup = new TagBuilder(HtmlTag.Div);
        buttonGroup.AddCssClass("btn-group mb-3 d-none d-md-block");

        // get the localized character set
        var charSet = context.Get<ILocalization>().GetText("LANGUAGE", "CHARSET").Split('/');

        charSet.ForEach(
            t =>
                {
                    // get the current selected character (if there is one)
                    var selectedLetter = currentLetter;

                    // go through all letters in a set
                    t.ForEach(
                        letter =>
                        {
                            // create a link to this letter
                            var link = new TagBuilder(HtmlTag.A);

                            link.MergeAttribute(
                                HtmlAttribute.Title,
                                context.Get<ILocalization>().GetTextFormatted(
                                    "ALPHABET_FILTER_BY",
                                    letter.ToString()));

                            link.MergeAttribute(
                                HtmlAttribute.Href,
                                context.Get<ILinkBuilder>().GetLink(
                                    ForumPages.Members,
                                    new { letter = letter == '#' ? '_' : letter }));

                            link.InnerHtml.Append(letter.ToString());

                            if (selectedLetter != char.MinValue && selectedLetter == letter)
                            {
                                // current letter is selected, use specified style
                                link.AddCssClass("btn btn-secondary active");
                            }
                            else
                            {
                                link.AddCssClass("btn btn-secondary");
                            }

                            buttonGroup.InnerHtml.AppendHtml(link);
                        });
                });

        return content.AppendHtml(buttonGroup);
    }
}