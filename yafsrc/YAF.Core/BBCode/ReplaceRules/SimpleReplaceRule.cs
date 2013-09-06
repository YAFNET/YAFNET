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
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    /// <summary>
  /// Not regular expression, just a simple replace
  /// </summary>
  public class SimpleReplaceRule : BaseReplaceRule
  {
    #region Constants and Fields

    /// <summary>
    ///   The _find.
    /// </summary>
    private readonly string _find;

    /// <summary>
    ///   The _replace.
    /// </summary>
    private readonly string _replace;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleReplaceRule"/> class.
    /// </summary>
    /// <param name="find">
    /// The find.
    /// </param>
    /// <param name="replace">
    /// The replace.
    /// </param>
    public SimpleReplaceRule(string find, string replace)
    {
      this._find = find;
      this._replace = replace;

      // lower the rank by default
      this.RuleRank = 100;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets RuleDescription.
    /// </summary>
    public override string RuleDescription
    {
      get
      {
        return "Find = \"{0}\"".FormatWith(this._find);
      }
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
      int index = -1;

      do
      {
        
        index = text.FastIndexOf(this._find);

        if (index >= 0)
        {
          // replace it...
          int replaceIndex = replacement.Add(this._replace);
          text = text.Substring(0, index) + replacement.Get(replaceIndex) +
                 text.Substring(index + this._find.Length);
        }
      }
      while (index >= 0);
    }

    #endregion
  }
}