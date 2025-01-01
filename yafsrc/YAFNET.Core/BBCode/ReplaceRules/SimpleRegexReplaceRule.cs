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

namespace YAF.Core.BBCode.ReplaceRules;

using System;
using System.Text.RegularExpressions;

/// <summary>
/// For basic regex with no variables
/// </summary>
public class SimpleRegexReplaceRule : BaseReplaceRule
{
    /// <summary>
    ///   The replace regex.
    /// </summary>
    readonly protected string RegExReplace;

    /// <summary>
    ///   The search regex.
    /// </summary>
    readonly protected Regex RegExSearch;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The Search Regex
    /// </param>
    /// <param name="regExReplace">
    /// The Replace Regex.
    /// </param>
    /// <param name="regExOptions">
    /// The Regex options.
    /// </param>
    public SimpleRegexReplaceRule(string regExSearch, string regExReplace, RegexOptions regExOptions)
    {
        this.RegExSearch = new Regex(regExSearch, regExOptions, TimeSpan.FromMilliseconds(100));
        this.RegExReplace = regExReplace;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The regex search.
    /// </param>
    /// <param name="regExReplace">
    /// The regex replace.
    /// </param>
    public SimpleRegexReplaceRule(Regex regExSearch, string regExReplace)
    {
        this.RegExSearch = regExSearch;
        this.RegExReplace = regExReplace;
    }

    /// <summary>
    ///   Gets RuleDescription.
    /// </summary>
    public override string RuleDescription => $"RegExSearch = \"{this.RegExSearch}\"";

    /// <summary>
    /// The replace.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="replacement">
    /// The replacement.
    /// </param>
    public override void Replace(ref string text, IReplaceBlocks replacement)
    {
        var sb = new StringBuilder(text);

        var m = this.RegExSearch.Match(text);
        while (m.Success)
        {
            var replaceString = this.RegExReplace.Replace("${inner}", this.GetInnerValue(m.Groups["inner"].Value));

            // pulls the html into the replacement collection before it's inserted back into the main text
            replacement.ReplaceHtmlFromText(ref replaceString);

            // remove old bbcode...
            sb.Remove(m.Groups[0].Index, m.Groups[0].Length);

            // insert replaced value(s)
            sb.Insert(m.Groups[0].Index, replaceString);

            m = this.RegExSearch.Match(sb.ToString());
        }

        text = sb.ToString();
    }

    /// <summary>
    /// Gets the Inner Value
    /// </summary>
    /// <param name="innerValue">
    /// The inner value.
    /// </param>
    /// <returns>
    /// Returns the Inner Value
    /// </returns>
    protected virtual string GetInnerValue(string innerValue)
    {
        return innerValue;
    }
}