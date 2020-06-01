/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

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
    using System.Collections;
    using System.Data;
    using System.Linq;
    using System.Text;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Utilities;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;

    #endregion

    /// <summary>
    /// The forum moderator list.
    /// </summary>
    public partial class ForumModeratorList : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets DataSource.
        /// </summary>
        public IEnumerable DataSource { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the PreRender event
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnPreRender(EventArgs e)
        { 
            base.OnPreRender(e);

            if (((DataRow[])this.DataSource).Length == 0)
            {
                this.ShowMods.Visible = false;

                return;
            }

            this.PageContext.PageElements.RegisterJsBlockStartup(
                "ForumModsPopoverJs",
                JavaScriptBlocks.ForumModsPopoverJs(
                    $"<i class=\"fa fa-user-secret fa-fw text-secondary\"></i>&nbsp;{this.GetText("DEFAULT", "MODERATORS")} ..."));

            var content = new StringBuilder();

            content.Append(@"<ol class=""list-unstyled"">");

            this.DataSource.Cast<DataRow>().ForEach(
                row =>
                {
                    content.Append("<li>");

                    if (row["IsGroup"].ToType<int>() == 0)
                    {
                        // Render Moderator User Link
                        var userLink = new UserLink
                        {
                            Style = row["Style"].ToString(),
                            UserID = row["ModeratorID"].ToType<int>(),
                            ReplaceName = row[this.Get<BoardSettings>().EnableDisplayName
                                                                     ? "ModeratorDisplayName"
                                                                     : "ModeratorName"].ToString()
                        };

                        content.Append(userLink.RenderToString());
                    }
                    else
                    {
                        // render mod group
                        content.Append(
                            row[this.Get<BoardSettings>().EnableDisplayName
                                    ? "ModeratorDisplayName"
                                    : "ModeratorName"]);
                    }

                    content.Append(@"</li>");
                });

            content.Append("</ol>");

            this.ShowMods.DataContent = content.ToString().ToJsString();
            this.ShowMods.Text = $"{this.GetText("SHOW")} {this.GetText("DEFAULT", "MODERATORS")}";
        }

        #endregion
    }
}