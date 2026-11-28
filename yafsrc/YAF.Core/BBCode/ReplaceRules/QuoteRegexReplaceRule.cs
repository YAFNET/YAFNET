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

using System;
using System.Text;
using System.Text.RegularExpressions;

using YAF.Core.Model;
using YAF.Types.Constants;
using YAF.Types.Models;

/// <summary>
/// Quote Block regular express replace
/// </summary>
public class QuoteRegexReplaceRule : VariableRegexReplaceRule
{
    /// <summary>
    /// The line breaks and spaces at the start and the end of a text (the rule runs before line breaks become &lt;br /&gt;).
    /// </summary>
    private static readonly Regex TrimLineBreaks = new(@"^\s+|\s+$", RegexOptions.Compiled, RegexHelper.Timeout);

    /// <summary>
    /// An opening quote tag ([quote] or [quote=name]).
    /// </summary>
    private static readonly Regex OpenQuote = new(@"\[quote[=\]]", RegexOptions.IgnoreCase | RegexOptions.Compiled, RegexHelper.Timeout);

    /// <summary>
    /// A closing quote tag.
    /// </summary>
    private static readonly Regex CloseQuote = new(@"\[/quote\]", RegexOptions.IgnoreCase | RegexOptions.Compiled, RegexHelper.Timeout);

    /// <summary>
    /// The depth from which a quote is collapsed (0 = a quote in the post, 1 = a quote in a quote).
    /// </summary>
    private const int CollapsedDepth = 2;

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

            var author = match.Groups["quote"].Value;

            // no empty lines at the start and the end of the quoted text
            var quoteText = TrimLineBreaks.Replace(this.GetInnerValue(match.Groups["inner"].Value), string.Empty);

            var localization = BoardContext.Current.Get<ILocalization>();
            var topicLink = string.Empty;

            if (author.Contains(";"))
            {
                // [quote=name;post id] is the quote of a post: the name links to it
                try
                {
                    var postId = author.Substring(author.LastIndexOf(";", StringComparison.Ordinal) + 1);
                    author = author.Remove(author.LastIndexOf(";", StringComparison.Ordinal));

                    var topic = BoardContext.Current.GetRepository<Topic>().GetTopicFromMessage(postId.ToType<int>());

                    if (topic == null)
                    {
                        // the quoted post does not exist (anymore)
                        throw new InvalidOperationException($"The quoted post {postId} was not found");
                    }

                    topicLink = BoardContext.Current.Get<LinkBuilder>().GetLink(
                        ForumPages.Posts,
                        new { m = postId, name = topic.TopicName });
                }
                catch (Exception)
                {
                    // the post id is missing or the post was deleted
                    topicLink = string.Empty;
                }
            }

            // the quotes are rendered innermost first: the quotes around this one are still BBCode
            var before = sb.ToString(0, match.Index);
            var depth = OpenQuote.Matches(before).Count - CloseQuote.Matches(before).Count;

            // a quote in a quote in a quote is collapsed, the reply is what matters
            var collapsed = depth >= CollapsedDepth;

            var header = new StringBuilder();

            header.Append("""<i class="fa fa-quote-left bbcode-quote-icon" aria-hidden="true"></i>""");

            header.Append(
                author.IsSet()
                    ? $"""<span class="bbcode-quote-author">{localization.GetText("COMMON", "BBCODE_QUOTEWROTE").Replace("{0}", author)}</span>"""
                    : $"""<span class="bbcode-quote-author">{localization.GetText("COMMON", "TT_QUOTE")}</span>""");

            if (topicLink.IsSet())
            {
                var goToMessage = localization.GetText("MESSAGEHISTORY", "TOMESSAGE");

                header.Append(
                    $"""<a class="bbcode-quote-link" href="{topicLink}" title="{goToMessage}" aria-label="{goToMessage}"><i class="fa fa-up-right-from-square" aria-hidden="true"></i></a>""");
            }

            string quoteStart, quoteEnd;

            if (collapsed)
            {
                header.Append("""<i class="fa fa-chevron-down bbcode-quote-toggle" aria-hidden="true"></i>""");

                quoteStart = $"""<details class="bbcode-quote-details"><summary class="bbcode-quote-header">{header}</summary>""";
                quoteEnd = "</details>";
            }
            else
            {
                // a plain [quote] has no header
                quoteStart = author.IsSet() ? $"""<div class="bbcode-quote-header">{header}</div>""" : string.Empty;
                quoteEnd = string.Empty;
            }

            innerReplace.Replace("${quoteClass}", collapsed ? "bbcode-quote bbcode-quote-collapsed" : "bbcode-quote");
            innerReplace.Replace("${quote}", quoteStart);
            innerReplace.Replace("${quoteEnd}", quoteEnd);
            innerReplace.Replace("${quoteText}", quoteText);

            this.Variables.ForEach(
                variable =>
                    {
                        var varName = variable;
                        var handlingValue = string.Empty;

                        if (varName.Contains(":"))
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