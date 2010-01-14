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
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Compilation;
using System.Web.UI;
using YAF.Classes.Data;
using YAF.Modules;

namespace YAF.Controls
{
  /// <summary>
  /// The message base.
  /// </summary>
  public class MessageBase : BaseControl
  {
    /// <summary>
    /// The _options.
    /// </summary>
    private static RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Singleline;

    /// <summary>
    /// The _rgx module.
    /// </summary>
    private static string _rgxModule = @"\<YafModuleFactoryInvocation ClassName=\""(?<classname>(.*?))\""\>(?<inner>(.+?))\</YafModuleFactoryInvocation\>";

    /// <summary>
    /// The _rgx module param.
    /// </summary>
    private static string _rgxModuleParam = @"\<Param Name=\""(?<name>(.*?))\""\>(?<inner>(.+?))\</Param\>";

    /// <summary>
    /// The render modules in bb code.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="theseFlags">
    /// The these flags.
    /// </param>
    /// <param name="displayUserId">
    /// The display user id.
    /// </param>
    protected virtual void RenderModulesInBBCode(HtmlTextWriter writer, string message, MessageFlags theseFlags, int? displayUserId)
    {
      var _regExSearch = new Regex(_rgxModule, _options);
      Match m = _regExSearch.Match(message);

      while (m.Success)
      {
        // get the class name
        string className = m.Groups["classname"].Value;

        // get all parameters as a name/value dictionary
        var paramDic = new Dictionary<string, string>();

        // pull the parameters...
        GetModuleParameters(ref paramDic, m.Groups["inner"].Value);

        // render what is before the control...
        writer.Write(message.Substring(0, m.Groups[0].Index));

        // create/render the control...
        Type module = BuildManager.GetType(className, true, false);
        var customModule = (YafBBCodeControl) Activator.CreateInstance(module);

        // assign parameters...
        customModule.CurrentMessageFlags = theseFlags;
        customModule.DisplayUserID = displayUserId;
        customModule.Parameters = paramDic;

        // render this control...
        customModule.RenderControl(writer);

        // now we are just concerned with what is after...
        message = message.Substring(m.Groups[0].Index + m.Groups[0].Length);

        m = _regExSearch.Match(message);
      }

      // render anything remaining...
      writer.Write(message);
    }

    /// <summary>
    /// The get module parameters.
    /// </summary>
    /// <param name="paramDic">
    /// The param dic.
    /// </param>
    /// <param name="invokeString">
    /// The invoke string.
    /// </param>
    protected virtual void GetModuleParameters(ref Dictionary<string, string> paramDic, string invokeString)
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
  }
}