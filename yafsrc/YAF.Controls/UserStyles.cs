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

    using System.Linq;
    using System.Web.UI;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Control displaying list of user currently active on a forum.
    /// </summary>
    public class RoleRankStyles : BaseControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets styles data string to display in control.
        /// </summary>
        [CanBeNull]
        public string RawStyles { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Implements rendering of control to the client through use of <see cref="HtmlTextWriter"/>.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            // we shall continue only if there is a style string available
            if (this.RawStyles.IsNotSet())
            {
                return;
            }

            // writes starting tag
            writer.WriteLine(@"<span class=""grouprankstyles"" id=""{0}"">".FormatWith(this.ClientID));

            var index = 1;

            var styles = this.RawStyles.Split('/').Where(s => s.IsSet()).Select(s => s.Trim().Split('!')).ToList();

            foreach (var styleElements in styles)
            {
                if (styleElements.Length >= 2)
                {
                    writer.WriteLine(
                        "<span style=\"{0}\">{1}{2}</span>".FormatWith(
                            styleElements[1], styleElements[0], (index++ == styles.Count) ? string.Empty : ", "));
                }

                // write ending tag
                writer.WriteLine("</span>");
            }
        }

        #endregion
    }
}