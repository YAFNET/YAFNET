/* Yet Another Forum.net
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Core.BBCode.ReplaceRules
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

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

            var match = this._regExSearch.Match(text);

            while (match.Success)
            {
                var innerReplace = new StringBuilder(this._regExReplace);
                int i = 0;

                if (this._truncateLength > 0)
                {
                    // special handling to truncate urls
                    innerReplace.Replace(
                      "${innertrunc}", match.Groups["inner"].Value.TruncateMiddle(this._truncateLength));
                }

                var quote = match.Groups["quote"].Value;

                var localQuoteWrote = YafContext.Current.Get<ILocalization>().GetText("COMMON", "BBCODE_QUOTEWROTE");
                var localQuotePosted = YafContext.Current.Get<ILocalization>()
                                                    .GetText("COMMON", "BBCODE_QUOTEPOSTED");

                // extract post id if exists
                if (quote.Contains(";"))
                {
                    string postId;

                    string userName;

                    try
                    {
                        postId = quote.Substring(quote.IndexOf(";") + 1);
                        userName = quote = quote.Remove(quote.IndexOf(";"));
                    }
                    catch (Exception)
                    {
                        // if post id is missing
                        postId = string.Empty;
                        userName = quote;
                    }

                    if (postId.IsSet())
                    {
                        quote =
                            @"{0} <a href=""{1}""><img src=""{2}"" title=""{3}"" alt=""{3}"" /></a>".FormatWith(
                                localQuotePosted.Replace("{0}", userName),
                                YafBuildLink.GetLink(ForumPages.posts, "m={0}#post{0}", postId),
                                YafContext.Current.Get<ITheme>().GetItem("ICONS", "ICON_LATEST"),
                                YafContext.Current.Get<ILocalization>().GetText("COMMON", "BBCODE_QUOTEPOSTED_TT"));
                    }
                    else
                    {
                        quote = localQuoteWrote.Replace("{0}", quote);
                    }
                }
                else
                {
                    quote = localQuoteWrote.Replace("{0}", quote);
                }

                innerReplace.Replace("${quote}", quote);

                foreach (string tVar in this._variables)
                {
                    string varName = tVar;
                    string handlingValue = string.Empty;

                    if (varName.Contains(":"))
                    {
                        // has handling section
                        string[] tmpSplit = varName.Split(':');
                        varName = tmpSplit[0];
                        handlingValue = tmpSplit[1];
                    }

                    string tValue = match.Groups[varName].Value;

                    if (this._variableDefaults != null && tValue.Length == 0)
                    {
                        // use default instead
                        tValue = this._variableDefaults[i];
                    }

                    innerReplace.Replace("${" + varName + "}", this.ManageVariableValue(varName, tValue, handlingValue));
                    i++;
                }

                innerReplace.Replace("${inner}", match.Groups["inner"].Value);

                // pulls the htmls into the replacement collection before it's inserted back into the main text
                replacement.ReplaceHtmlFromText(ref innerReplace);

                // remove old bbcode...
                sb.Remove(match.Groups[0].Index, match.Groups[0].Length);

                // insert replaced value(s)
                sb.Insert(match.Groups[0].Index, innerReplace.ToString());

                // text = text.Substring( 0, m.Groups [0].Index ) + tStr + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
                match = this._regExSearch.Match(sb.ToString());
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