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

namespace YAF.Core.BBCode.ReplaceRules;

using System.Text.RegularExpressions;

/// <summary>
/// Syntax Highlighted code block regular express replace
/// </summary>
public class SyntaxHighlighterRegexReplaceRule : SimpleRegexReplaceRule
{
    /// <summary>
    ///   The syntax highlighter.
    /// </summary>
    private readonly HighLighter syntaxHighlighter = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="SyntaxHighlighterRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The Search Regex.
    /// </param>
    /// <param name="regExReplace">
    /// The Replace Regex.
    /// </param>
    public SyntaxHighlighterRegexReplaceRule(Regex regExSearch, string regExReplace)
        : base(regExSearch, regExReplace)
    {
        this.syntaxHighlighter.ReplaceEnter = true;
        this.RuleRank = 1;
    }

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
        var m = this.RegExSearch.Match(text);

        while (m.Success)
        {
            var inner = this.syntaxHighlighter.ColorText(
                this.GetInnerValue(m.Groups["inner"].Value), m.Groups["language"].Value);

            var replaceItem = this.RegExReplace.Replace("${inner}", inner);

            // pulls the html's into the replacement collection before it's inserted back into the main text
            var replaceIndex = replacement.Add(replaceItem);

            text =
                $"{text[..m.Groups[0].Index]}{replacement.Get(replaceIndex)}{text[(m.Groups[0].Index + m.Groups[0].Length)..]}";

            m = this.RegExSearch.Match(text);
        }
    }

    /// <summary>
    /// This just overrides how the inner value is handled
    /// </summary>
    /// <param name="innerValue">The inner value.</param>
    /// <returns>
    /// The get inner value.
    /// </returns>
    override protected string GetInnerValue(string innerValue)
    {
        return innerValue;
    }
}