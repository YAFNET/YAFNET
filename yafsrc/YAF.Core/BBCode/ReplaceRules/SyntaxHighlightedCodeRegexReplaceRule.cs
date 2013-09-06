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
    using System.Text.RegularExpressions;

    using YAF.Types.Interfaces;

    /// <summary>
    /// Syntax Highlighted code block regular express replace
    /// </summary>
    public class SyntaxHighlightedCodeRegexReplaceRule : SimpleRegexReplaceRule
    {
        #region Constants and Fields

        /// <summary>
        ///   The _syntax highlighter.
        /// </summary>
        private readonly HighLighter _syntaxHighlighter = new HighLighter();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxHighlightedCodeRegexReplaceRule"/> class.
        /// </summary>
        /// <param name="regExSearch">
        /// The reg ex search.
        /// </param>
        /// <param name="regExReplace">
        /// The reg ex replace.
        /// </param>
        public SyntaxHighlightedCodeRegexReplaceRule(Regex regExSearch, string regExReplace)
            : base(regExSearch, regExReplace)
        {
            this._syntaxHighlighter.ReplaceEnter = true;
            this.RuleRank = 1;
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
            Match m = this._regExSearch.Match(text);
            while (m.Success)
            {
                string inner = this._syntaxHighlighter.ColorText(
                    this.GetInnerValue(m.Groups["inner"].Value), m.Groups["language"].Value);

                string replaceItem = this._regExReplace.Replace("${inner}", inner);

                // pulls the htmls into the replacement collection before it's inserted back into the main text
                int replaceIndex = replacement.Add(replaceItem);

                text = text.Substring(0, m.Groups[0].Index) + replacement.Get(replaceIndex)
                       + text.Substring(m.Groups[0].Index + m.Groups[0].Length);

                m = this._regExSearch.Match(text);
            }
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