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

namespace YAF.Core.Helpers;

using System;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// The texts of the help pages name the buttons and menus of the forum with tokens, so every language
/// shows the labels the user sees in the forum (and the help stays right when a label is renamed):
/// <list type="bullet">
/// <item><c>[[PAGE.TAG]]</c> - the localized label, in bold</item>
/// <item><c>[[help:topic]]</c> - the address of a help topic (for a link)</item>
/// <item><c>[[page:ForumPage]]</c> - the address of a forum page (for a link)</item>
/// </list>
/// </summary>
public static partial class HelpTextHelper
{
    /// <summary>
    /// Replaces the tokens of a help text.
    /// </summary>
    /// <param name="text">The help text (html).</param>
    /// <param name="getLabel">Gets the localized text of a page and tag.</param>
    /// <param name="getHelpLink">Gets the address of a help topic.</param>
    /// <param name="getPageLink">Gets the address of a forum page (by its name), or null if there is no such page.</param>
    /// <returns>The help text with the labels and addresses.</returns>
    public static string ReplaceTokens(
        string text,
        Func<string, string, string> getLabel,
        Func<string, string> getHelpLink,
        Func<string, string> getPageLink)
    {
        if (string.IsNullOrEmpty(text) || !text.Contains("[[", StringComparison.Ordinal))
        {
            return text;
        }

        return Token().Replace(
            text,
            match =>
            {
                var kind = match.Groups["kind"].Value;
                var value = match.Groups["value"].Value;

                return kind switch
                {
                    "help" => HttpUtility.HtmlAttributeEncode(getHelpLink(value.ToLowerInvariant())),
                    "page" => HttpUtility.HtmlAttributeEncode(getPageLink(value) ?? "#"),
                    _ => $"<strong>{FormatLabel(getLabel(kind, value))}</strong>"
                };
            });
    }

    /// <summary>
    /// Formats a label for the running text: the label is text (some are html encoded or contain tags),
    /// and a colon or ellipsis at the end (e.g. "Subject:", "Add file(s)...") does not belong into a sentence.
    /// </summary>
    /// <param name="label">The localized label.</param>
    /// <returns>The encoded label.</returns>
    public static string FormatLabel(string label)
    {
        var text = HttpUtility.HtmlDecode(HtmlTag().Replace(label ?? string.Empty, string.Empty)).Trim();

        text = text.TrimEnd(':', '：', '…', '.', ' ', ' ').Trim();

        return HttpUtility.HtmlEncode(text);
    }

    /// <summary>
    /// The tokens: [[PAGE.TAG]], [[help:topic]] and [[page:ForumPage]].
    /// </summary>
    [GeneratedRegex(@"\[\[(?<kind>help|page|[A-Z0-9_]+)[.:](?<value>[A-Za-z0-9_]+)\]\]", RegexOptions.None, 1000)]
    public static partial Regex Token();

    [GeneratedRegex("<[^>]*>", RegexOptions.None, 1000)]
    private static partial Regex HtmlTag();
}
