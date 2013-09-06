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

    /// <summary>
  /// For the font size with replace
  /// </summary>
  public class FontSizeRegexReplaceRule : VariableRegexReplaceRule
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="FontSizeRegexReplaceRule"/> class.
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
    public FontSizeRegexReplaceRule(string regExSearch, string regExReplace, RegexOptions regExOptions)
      : base(regExSearch, regExReplace, regExOptions, new[] { "size" }, new[] { "5" })
    {
      this.RuleRank = 25;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The manage variable value.
    /// </summary>
    /// <param name="variableName">
    /// The variable name.
    /// </param>
    /// <param name="variableValue">
    /// The variable value.
    /// </param>
    /// <param name="handlingValue">
    /// The handling value.
    /// </param>
    /// <returns>
    /// The manage variable value.
    /// </returns>
    protected override string ManageVariableValue(string variableName, string variableValue, string handlingValue)
    {
      if (variableName == "size")
      {
        return this.GetFontSize(variableValue);
      }

      return variableValue;
    }

    /// <summary>
    /// The get font size.
    /// </summary>
    /// <param name="inputStr">
    /// The input str.
    /// </param>
    /// <returns>
    /// The get font size.
    /// </returns>
    private string GetFontSize(string inputStr)
    {
      int[] sizes = { 50, 70, 80, 90, 100, 120, 140, 160, 180 };
      int size = 5;

      // try to parse the input string...
      int.TryParse(inputStr, out size);

      if (size < 1)
      {
        size = 1;
      }

      if (size > sizes.Length)
      {
        size = 5;
      }

      return sizes[size - 1] + "%";
    }

    #endregion
  }
}