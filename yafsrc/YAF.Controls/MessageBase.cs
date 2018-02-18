/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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
        private const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled;

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
                                    codeRow => codeRow, codeRow => new Regex(codeRow.SearchRegex, Options));
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
            var workingMessage = messageStr;

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
                    var module = BuildManager.GetType(codeRow.ModuleClass, true, false);
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