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
    using System.Text;
    using System.Text.RegularExpressions;

    using YAF.Types.Interfaces;

    /// <summary>
  /// For basic regex with no variables
  /// </summary>
  public class SingleRegexReplaceRule : SimpleRegexReplaceRule
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The reg ex search.
    /// </param>
    /// <param name="regExReplace">
    /// The reg ex replace.
    /// </param>
    /// <param name="regExOptions">
    /// The reg ex options.
    /// </param>
    public SingleRegexReplaceRule(string regExSearch, string regExReplace, RegexOptions regExOptions)
      : base(regExSearch, regExReplace, regExOptions)
    {
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

      Match m = this._regExSearch.Match(text);
      while (m.Success)
      {
        // just replaces with no "inner"
        int replaceIndex = replacement.Add(this._regExReplace);

        // remove old bbcode...
        sb.Remove(m.Groups[0].Index, m.Groups[0].Length);

        // insert replaced value(s)
        sb.Insert(m.Groups[0].Index, replacement.Get(replaceIndex));

        // text = text.Substring( 0, m.Groups [0].Index ) + replacement.GetReplaceValue( replaceIndex ) + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
        m = this._regExSearch.Match(sb.ToString());
      }

      text = sb.ToString();
    }

    #endregion
  }
}