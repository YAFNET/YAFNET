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

using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// Quote Block regular express replace
/// </summary>
public class QuoteRegexReplaceRule : VariableRegexReplaceRule
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuoteRegexReplaceRule" /> class.
    /// </summary>
    /// <param name="searchRegex">The search regex.</param>
    /// <param name="replaceRegex">The replace regex.</param>
    /// <param name="options">The options.</param>
    public QuoteRegexReplaceRule(string searchRegex, string replaceRegex, RegexOptions options)
        : base(searchRegex, replaceRegex, options, ["quote"])
    {
        this.RuleRank = 60;
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
        var sb = new StringBuilder(text);

        var match = this.RegExSearch.Match(text);

        while (match.Success)
        {
            var innerReplace = new StringBuilder(this.RegExReplace);
            var i = 0;

            // special handling to truncate urls
            innerReplace.Replace(
                "${innertrunc}",
                match.Groups["inner"].Value);

            var quote = match.Groups["quote"].Value;
            var quoteText = this.GetInnerValue(match.Groups["inner"].Value);

            var localQuoteWrote = BoardContext.Current.Get<ILocalization>().GetText("COMMON", "BBCODE_QUOTEWROTE");
            var localQuotePosted = BoardContext.Current.Get<ILocalization>().GetText("COMMON", "BBCODE_QUOTEPOSTED");

            // extract post id if exists
            if (quote.Contains(';'))
            {
                string postId, userName, topicLink = string.Empty;

                try
                {
                    postId = quote[(quote.LastIndexOf(';') + 1)..];
                    userName = quote = quote.Remove(quote.LastIndexOf(';'));

                    var topic = BoardContext.Current.GetRepository<Topic>().GetTopicFromMessage(postId.ToType<int>());

                    topicLink = BoardContext.Current.Get<ILinkBuilder>().GetMessageLink(topic, postId.ToType<int>());
                }
                catch (Exception)
                {
                    // if post id is missing
                    postId = string.Empty;
                    userName = quote;
                }

                quote = postId.IsSet()
                            ? $"""
                               <p class="card-text text-end fst-italic"><small class="text-body-secondary">{localQuotePosted.Replace("{0}", userName)}&nbsp;<a class="card-link" href="{topicLink}"><i class="fas fa-external-link-alt ms-2"></i></a></small></p>
                               """
                            : $"""
                               <p class="card-text text-end fst-italic"><small class="text-body-secondary">{localQuoteWrote.Replace("{0}", quote)}</small></p>
                               """;
            }
            else
            {
                quote =
                    $"""
                     <p class="card-text text-end fst-italic"><small class="text-body-secondary">{localQuoteWrote.Replace("{0}", quote)}</small></p>
                     """;
            }

            innerReplace.Replace("${quote}", quote);
            innerReplace.Replace("${quoteText}", quoteText);

            this.Variables.ForEach(
                variable =>
                    {
                        var varName = variable;
                        var handlingValue = string.Empty;

                        if (varName.Contains(':'))
                        {
                            // has handling section
                            var tmpSplit = varName.Split(':');
                            varName = tmpSplit[0];
                            handlingValue = tmpSplit[1];
                        }

                        var value = match.Groups[varName].Value;

                        if (this.VariableDefaults != null && value.Length == 0)
                        {
                            // use default instead
                            value = this.VariableDefaults[i];
                        }

                        innerReplace.Replace(
                            $"${{{varName}}}",
                            this.ManageVariableValue(varName, value, handlingValue));
                        i++;
                    });

            innerReplace.Replace("${inner}", match.Groups["inner"].Value);

            // pulls the html's into the replacement collection before it's inserted back into the main text
            replacement.ReplaceHtmlFromText(ref innerReplace);

            // remove old BBCode...
            sb.Remove(match.Groups[0].Index, match.Groups[0].Length);

            // insert replaced value(s)
            sb.Insert(match.Groups[0].Index, innerReplace.ToString());

            match = this.RegExSearch.Match(sb.ToString());
        }

        text = sb.ToString();
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