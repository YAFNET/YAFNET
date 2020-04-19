/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.BBCode.ReplaceRules
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    /// <summary>
    /// Quote Block regular express replace
    /// </summary>
    public class QuoteRegexReplaceRule : VariableRegexReplaceRule
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteRegexReplaceRule" /> class.
        /// </summary>
        /// <param name="searchRegex">The search regex.</param>
        /// <param name="replaceRegex">The replace regex.</param>
        /// <param name="options">The options.</param>
        public QuoteRegexReplaceRule(string searchRegex, string replaceRegex, RegexOptions options)
            : base(searchRegex, replaceRegex, options, new[] { "quote" })
        {
            this.RuleRank = 60;
        }

        #endregion

        #region Public Methods

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

                if (this.TruncateLength > 0)
                {
                    // special handling to truncate urls
                    innerReplace.Replace(
                        "${innertrunc}",
                        match.Groups["inner"].Value.TruncateMiddle(this.TruncateLength));
                }

                var quote = match.Groups["quote"].Value;

                var localQuoteWrote = BoardContext.Current.Get<ILocalization>().GetText("COMMON", "BBCODE_QUOTEWROTE");
                var localQuotePosted = BoardContext.Current.Get<ILocalization>().GetText("COMMON", "BBCODE_QUOTEPOSTED");

                // extract post id if exists
                if (quote.Contains(";"))
                {
                    string postId;

                    string userName;

                    try
                    {
                        postId = quote.Substring(quote.LastIndexOf(";", StringComparison.Ordinal) + 1);
                        userName = quote = quote.Remove(quote.LastIndexOf(";", StringComparison.Ordinal));
                    }
                    catch (Exception)
                    {
                        // if post id is missing
                        postId = string.Empty;
                        userName = quote;
                    }

                    quote = postId.IsSet()
                                ? $@"<footer class=""blockquote-footer"">
                                         <cite>{localQuotePosted.Replace("{0}", userName)}&nbsp;<a href=""{BuildLink.GetLink(ForumPages.Posts, "m={0}#post{0}", postId)}""><i class=""fas fa-external-link-alt""></i></a></cite></footer>
                                         <p class=""mb-0 mt-2"">"
                                : $@"<footer class=""blockquote-footer"">
                                         <cite>{localQuoteWrote.Replace("{0}", quote)}</cite></footer><p class=""mb-0"">";
                }
                else
                {
                    quote =
                        $@"<footer class=""blockquote-footer"">
                               <cite>{localQuoteWrote.Replace("{0}", quote)}</cite></footer><p class=""mb-0"">";
                }

                innerReplace.Replace("${quote}", quote);

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

                // text = text.Substring( 0, m.Groups [0].Index ) + tStr + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
                match = this.RegExSearch.Match(sb.ToString());
            }

            text = sb.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// This just overrides how the inner value is handled
        /// </summary>
        /// <param name="innerValue">The inner value.</param>
        /// <returns>
        /// The get inner value.
        /// </returns>
        protected override string GetInnerValue(string innerValue)
        {
            return innerValue;
        }

        #endregion
    }
}