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
    using System.Web;

    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    /// <summary>
  /// For complex regex with variable/default and truncate support
  /// </summary>
  public class VariableRegexReplaceRule : SimpleRegexReplaceRule
  {
    #region Constants and Fields

    /// <summary>
    ///   The _truncate length.
    /// </summary>
    protected int _truncateLength;

    /// <summary>
    ///   The _variable defaults.
    /// </summary>
    protected string[] _variableDefaults;

    /// <summary>
    ///   The _variables.
    /// </summary>
    protected string[] _variables;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The reg ex search.
    /// </param>
    /// <param name="regExReplace">
    /// The reg ex replace.
    /// </param>
    /// <param name="variables">
    /// The variables.
    /// </param>
    /// <param name="varDefaults">
    /// The var defaults.
    /// </param>
    /// <param name="truncateLength">
    /// The truncate length.
    /// </param>
    public VariableRegexReplaceRule(
      Regex regExSearch, string regExReplace, string[] variables, string[] varDefaults, int truncateLength)
      : base(regExSearch, regExReplace)
    {
      this._variables = variables;
      this._variableDefaults = varDefaults;
      this._truncateLength = truncateLength;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The reg ex search.
    /// </param>
    /// <param name="regExReplace">
    /// The reg ex replace.
    /// </param>
    /// <param name="variables">
    /// The variables.
    /// </param>
    /// <param name="varDefaults">
    /// The var defaults.
    /// </param>
    public VariableRegexReplaceRule(Regex regExSearch, string regExReplace, string[] variables, string[] varDefaults)
      : this(regExSearch, regExReplace, variables, varDefaults, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
    /// </summary>
    /// <param name="regExSearch">
    /// The reg ex search.
    /// </param>
    /// <param name="regExReplace">
    /// The reg ex replace.
    /// </param>
    /// <param name="variables">
    /// The variables.
    /// </param>
    public VariableRegexReplaceRule(Regex regExSearch, string regExReplace, string[] variables)
      : this(regExSearch, regExReplace, variables, null, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
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
    /// <param name="variables">
    /// The variables.
    /// </param>
    /// <param name="varDefaults">
    /// The var defaults.
    /// </param>
    /// <param name="truncateLength">
    /// The truncate length.
    /// </param>
    public VariableRegexReplaceRule(
      string regExSearch, 
      string regExReplace, 
      RegexOptions regExOptions, 
      string[] variables, 
      string[] varDefaults, 
      int truncateLength)
      : base(regExSearch, regExReplace, regExOptions)
    {
      this._variables = variables;
      this._variableDefaults = varDefaults;
      this._truncateLength = truncateLength;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
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
    /// <param name="variables">
    /// The variables.
    /// </param>
    /// <param name="varDefaults">
    /// The var defaults.
    /// </param>
    public VariableRegexReplaceRule(
      string regExSearch, string regExReplace, RegexOptions regExOptions, string[] variables, string[] varDefaults)
      : this(regExSearch, regExReplace, regExOptions, variables, varDefaults, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableRegexReplaceRule"/> class.
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
    /// <param name="variables">
    /// The variables.
    /// </param>
    public VariableRegexReplaceRule(
      string regExSearch, string regExReplace, RegexOptions regExOptions, string[] variables)
      : this(regExSearch, regExReplace, regExOptions, variables, null, 0)
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
        var innerReplace = new StringBuilder(this._regExReplace);
        int i = 0;

        foreach (string tVar in this._variables)
        {
          string varName = tVar;
          string handlingValue = String.Empty;

          if (varName.Contains(":"))
          {
            // has handling section
            string[] tmpSplit = varName.Split(':');
            varName = tmpSplit[0];
            handlingValue = tmpSplit[1];
          }

          string tValue = m.Groups[varName].Value;

          if (this._variableDefaults != null && tValue.Length == 0)
          {
            // use default instead
            tValue = this._variableDefaults[i];
          }

          innerReplace.Replace("${" + varName + "}", this.ManageVariableValue(varName, tValue, handlingValue));
          i++;
        }

        innerReplace.Replace("${inner}", m.Groups["inner"].Value);

        if (this._truncateLength > 0)
        {
          // special handling to truncate urls
          innerReplace.Replace(
            "${innertrunc}", m.Groups["inner"].Value.TruncateMiddle(this._truncateLength));
        }

        // pulls the htmls into the replacement collection before it's inserted back into the main text
        replacement.ReplaceHtmlFromText(ref innerReplace);

        // remove old bbcode...
        sb.Remove(m.Groups[0].Index, m.Groups[0].Length);

        // insert replaced value(s)
        sb.Insert(m.Groups[0].Index, innerReplace.ToString());

        // text = text.Substring( 0, m.Groups [0].Index ) + tStr + text.Substring( m.Groups [0].Index + m.Groups [0].Length );
        m = this._regExSearch.Match(sb.ToString());
      }

      text = sb.ToString();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Override to change default variable handling...
    /// </summary>
    /// <param name="variableName">
    /// </param>
    /// <param name="variableValue">
    /// </param>
    /// <param name="handlingValue">
    /// variable transfermation desired
    /// </param>
    /// <returns>
    /// The manage variable value.
    /// </returns>
    protected virtual string ManageVariableValue(string variableName, string variableValue, string handlingValue)
    {
      if (handlingValue.IsSet())
      {
        switch (handlingValue.ToLower())
        {
          case "decode":
            variableValue = HttpUtility.HtmlDecode(variableValue);
            break;
          case "encode":
            variableValue = HttpUtility.HtmlEncode(variableValue);
            break;
        }
      }

      return variableValue;
    }

    #endregion
  }
}