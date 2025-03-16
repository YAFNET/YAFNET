/* Yet Another Forum.NET
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

namespace YAF.Core.Helpers;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// The bb code helper.
/// </summary>
public static partial class BBCodeHelper
{
    /// <summary>
    /// The find user quoting.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public static List<string> FindUserQuoting(string text)
    {
        var mentions = Regex.Matches(
            text,
            @"\[quote\=(?<user>.+?);(?<messageId>.+?)\](?<inner>.+?)\[\/quote\]",
            RegexOptions.Singleline,
            TimeSpan.FromMilliseconds(100));

        return [.. (from Match match in mentions select match.Groups["user"].Value)];
    }

    /// <summary>
    /// Find all User mentions in the text
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    public static List<string> FindMentions(string text)
    {
        var mentions = Regex.Matches(text, @"@\[userlink\](?<inner>.+?)\[\/userlink\]", RegexOptions.IgnoreCase,
            TimeSpan.FromMilliseconds(100));

        return [.. (from Match match in mentions select match.Groups["inner"].Value)];
    }

    /// <summary>
    /// Strips BB Code Tags
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// The strip bb code.
    /// </returns>
    public static string StripBBCode(string text)
    {
        return Regex.Replace(text, @"\[(.|\n)*?\]", string.Empty, RegexOptions.None, TimeSpan.FromMilliseconds(100));
    }

    /// <summary>
    /// Encode Content inside Code Blocks
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public static string EncodeCodeBlocks(string text)
    {
        var regex = CodeBlockRegex();

        return regex.Replace(
            text,
            match => $"]{HttpUtility.HtmlEncode(match.Groups["inner"].Value)}[/code]");
    }

    /// <summary>
    /// Encode Content inside Code Blocks
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public static string DecodeCodeBlocks(string text)
    {
        var regex = new Regex(
            @"\](?<inner>(.*?))\[/code\]",
            RegexOptions.IgnoreCase | RegexOptions.Singleline,
            TimeSpan.FromMilliseconds(100));

        return regex.Replace(
            text,
            match => $"]{HttpUtility.HtmlDecode(match.Groups["inner"].Value)}[/code]");
    }

    /// <summary>
    /// Regex for the code block
    /// </summary>
    /// <returns>Regex.</returns>
    [GeneratedRegex(@"\](?<inner>(.*?))\[/code\]", RegexOptions.IgnoreCase | RegexOptions.Singleline, "de-DE")]
    private static partial Regex CodeBlockRegex();
}