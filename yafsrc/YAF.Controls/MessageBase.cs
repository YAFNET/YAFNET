/* Yet Another Forum.NET
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
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Types.Objects;
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
        private const RegexOptions _Options = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled;

        #endregion

        /// <summary>
        /// Gets CustomBBCode.
        /// </summary>
        protected IDictionary<BBCode, Regex> CustomBBCode
        {
            get
            {
                return this.Get<IObjectStore>().GetOrSet(
                    "CustomBBCodeRegExDictionary",
                    () =>
                        {
                            var bbcodeTable = this.Get<YafDbBroker>().GetCustomBBCode();
                            return
                                bbcodeTable.Where(b => (b.UseModule ?? false) && b.ModuleClass.IsSet() && b.SearchRegex.IsSet()).ToDictionary(
                                    codeRow => codeRow, codeRow => new Regex(codeRow.SearchRegex, _Options));
                        });
            }
        }

        #region Methods

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
        /// <param name="messageId">
        /// The Message Id.
        /// </param>
        protected virtual void RenderModulesInBBCode(
        [NotNull] HtmlTextWriter writer, [NotNull] string messageStr, [NotNull] MessageFlags theseFlags, int? displayUserId, int? messageId)
        {
            string workingMessage = messageStr;

            // handle custom bbcodes row by row...
            foreach (var keyPair in this.CustomBBCode)
            {
                var codeRow = keyPair.Key;

                Match match = null;

                do
                {
                    match = keyPair.Value.Match(workingMessage);

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
                    customModule.MessageID = messageId;
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

        #endregion
    }
}