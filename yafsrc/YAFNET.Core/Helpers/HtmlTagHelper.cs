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

/// <summary>
/// The html tag helper.
/// </summary>
public static class HtmlTagHelper
{
    /// <summary>
    /// Strip Html Tags from String
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// Returns the cleaned string
    /// </returns>
    public static string StripHtml(string text)
    {
        return text.IsNotSet() ? text : Regex.Replace(text, @"<(.|\n)*?>", string.Empty, RegexOptions.None, TimeSpan.FromMilliseconds(100));
    }

    /// <summary>
    /// The clean html string.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// The clean html string.
    /// </returns>
    public static string CleanHtmlString(string text)
    {
        text = text.Replace("<br />", " ");
        text = text.Replace("&quot;", "\"");
        text = text.Replace("&nbsp;", " ");

        return text;
    }

    /// <summary>
    /// Validates a html tag against the allowedTags. Also check that
    /// it doesn't have any "extra" features such as javascript in it.
    /// </summary>
    /// <param name="tag">
    /// </param>
    /// <param name="allowedTags">
    /// </param>
    /// <returns>
    /// The is valid tag.
    /// </returns>
    public static bool IsValidTag(string tag, IEnumerable<string> allowedTags)
    {
        if (tag.Contains("javascript"))
        {
            return false;
        }

        if (tag.Contains("vbscript"))
        {
            return false;
        }

        if (tag.Contains("onclick"))
        {
            return false;
        }

        var endChars = new[] { ' ', '>', '/', '\t' };

        var pos = tag.IndexOfAny(endChars, 1);
        if (pos > 0)
        {
            tag = tag[..pos];
        }

        if (tag[0] == '/')
        {
            tag = tag[1..];
        }

        // check if it's a valid tag
        return allowedTags.Any(allowedTag => tag == allowedTag);
    }
}