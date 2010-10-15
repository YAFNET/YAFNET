/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Classes.Core.BBCode
{
  using System.Text.RegularExpressions;

  /// <summary>
  /// Simple code block regular express replace
  /// </summary>
  public class CodeRegexReplaceRule : SimpleRegexReplaceRule
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The reg ex search.
    /// </param>
    /// <param name="regExReplace">
    /// The reg ex replace.
    /// </param>
    public CodeRegexReplaceRule(Regex regExSearch, string regExReplace)
      : base(regExSearch, regExReplace)
    {
      // default high rank...
      this.RuleRank = 2;
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
        string replaceItem = this._regExReplace.Replace("${inner}", this.GetInnerValue(m.Groups["inner"].Value));

        int replaceIndex = replacement.Add(replaceItem);
        text = text.Substring(0, m.Groups[0].Index) + replacement.Get(replaceIndex) +
               text.Substring(m.Groups[0].Index + m.Groups[0].Length);

        m = this._regExSearch.Match(text);
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// This just overrides how the inner value is handled
    /// </summary>
    /// <param name="innerValue">
    /// </param>
    /// <returns>
    /// The get inner value.
    /// </returns>
    protected override string GetInnerValue(string innerValue)
    {
      innerValue = innerValue.Replace("  ", "&nbsp; ");
      innerValue = innerValue.Replace("  ", " &nbsp;");
      innerValue = innerValue.Replace("\t", "&nbsp; &nbsp;&nbsp;");
      innerValue = innerValue.Replace("[", "&#91;");
      innerValue = innerValue.Replace("]", "&#93;");
      innerValue = innerValue.Replace("<", "&lt;");
      innerValue = innerValue.Replace(">", "&gt;");
      innerValue = innerValue.Replace("\r\n", "<br />");
      return innerValue;
    }

    #endregion
  }
}