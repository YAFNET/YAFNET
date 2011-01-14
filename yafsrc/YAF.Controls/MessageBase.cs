/* Yet Another Forum.NET
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Web.Compilation;
  using System.Web.UI;

  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Flags;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// The message base.
  /// </summary>
  public class MessageBase : BaseControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The _options.
    /// </summary>
    private static RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Singleline;

    /// <summary>
    ///   The _rgx module.
    /// </summary>
    private static string _rgxModule =
      @"\<YafModuleFactoryInvocation ClassName=\""(?<classname>(.*?))\""\>(?<inner>(.+?))\</YafModuleFactoryInvocation\>";

    /// <summary>
    ///   The _rgx module param.
    /// </summary>
    private static string _rgxModuleParam = @"\<Param Name=\""(?<name>(.*?))\""\>(?<inner>(.+?))\</Param\>";

    #endregion

    #region Methods

    /// <summary>
    /// The get module parameters.
    /// </summary>
    /// <param name="paramDic">
    /// The param dic.
    /// </param>
    /// <param name="invokeString">
    /// The invoke string.
    /// </param>
    protected virtual void GetModuleParameters(
      [NotNull] ref Dictionary<string, string> paramDic, [NotNull] string invokeString)
    {
      var regExSearch = new Regex(_rgxModuleParam, _options);
      Match m = regExSearch.Match(invokeString);

      var sb = new StringBuilder(invokeString);

      while (m.Success)
      {
        paramDic.Add(m.Groups["name"].Value, m.Groups["inner"].Value);

        // remove old param...
        sb.Remove(m.Groups[0].Index, m.Groups[0].Length);

        // match updated string...
        m = regExSearch.Match(sb.ToString());
      }
    }

    /// <summary>
    /// The render modules in bb code.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="messageStr">
    /// The message Str.
    /// </param>
    /// <param name="theseFlags">
    /// The these flags.
    /// </param>
    /// <param name="displayUserId">
    /// The display user id.
    /// </param>
    protected virtual void RenderModulesInBBCode(
      [NotNull] HtmlTextWriter writer, [NotNull] string messageStr, [NotNull] MessageFlags theseFlags, int? displayUserId)
    {
      var bbcodeTable = this.Get<IDBBroker>().GetCustomBBCode();

      string workingMessage = messageStr;

      // handle custom bbcodes row by row...
      foreach (
        var codeRow in bbcodeTable.Where(b => (b.UseModule ?? false) && b.ModuleClass.IsSet() && b.SearchRegex.IsSet()))
      {
        Match match = null;

        do
        {
          match = this.GetMatch(codeRow.SearchRegex, workingMessage);

          if (!match.Success)
          {
            continue;
          }

          var sb = new StringBuilder();

          var paramDic = new Dictionary<string, string> { { "inner", match.Groups["inner"].Value } };

          if (codeRow.Variables.IsSet() && codeRow.Variables.Split(';').Any())
          {
            var vars = codeRow.Variables.Split(';');

            foreach (var v in vars.Where(v => match.Groups[v] != null))
            {
              paramDic.Add(v, match.Groups[v].Value);
            }
          }

          sb.Append(workingMessage.Substring(0, match.Groups[0].Index));

          // create/render the control...
          Type module = BuildManager.GetType(codeRow.ModuleClass, true, false);
          var customModule = (YafBBCodeControl)Activator.CreateInstance(module);

          // assign parameters...
          customModule.CurrentMessageFlags = theseFlags;
          customModule.DisplayUserID = displayUserId;
          customModule.Parameters = paramDic;

          // render this control...
          sb.Append(customModule.RenderToString());

          sb.Append(workingMessage.Substring(match.Groups[0].Index + match.Groups[0].Length));

          workingMessage = sb.ToString();
        }
        while (match.Success);
      }

      writer.Write(workingMessage);
    }

    /// <summary>
    /// The get match.
    /// </summary>
    /// <param name="searchRegEx">
    /// The search reg ex.
    /// </param>
    /// <param name="searchString">
    /// The search string.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    private Match GetMatch([NotNull] string searchRegEx, [NotNull] string searchString)
    {
      var regExSearch = new Regex(searchRegEx, _options);
      return regExSearch.Match(searchString);
    }

    #endregion
  }
}