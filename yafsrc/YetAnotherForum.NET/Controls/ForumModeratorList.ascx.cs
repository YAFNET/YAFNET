/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
namespace YAF.Controls;

#region Using

using System.Text;

using YAF.Types.Objects;
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
    public List<SimpleModerator> DataSource { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Handles the PreRender event
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            "ForumModsPopoverJs",
            JavaScriptBlocks.ForumModsPopoverJs($"<i class=\"fa fa-user-secret fa-fw text-secondary\"></i>&nbsp;{this.GetText("DEFAULT", "MODERATORS")} ..."));

        var content = new StringBuilder();

        content.Append(@"<ol class=""list-unstyled"">");

        this.DataSource.DistinctBy(x => x.Name).ForEach(
            row =>
                {
                    content.Append("<li>");

                    if (row.IsGroup)
                    {
                        // render mod group
                        content.Append(
                            this.PageBoardContext.BoardSettings.EnableDisplayName
                                ? row.DisplayName
                                : row.Name);
                    }
                    else
                    {
                        // Render Moderator User Link
                        var userLink = new UserLink
                                           {
                                               Style = row.Style,
                                               UserID = row.ModeratorID,
                                               ReplaceName = this.PageBoardContext.BoardSettings.EnableDisplayName
                                                                 ? row.DisplayName
                                                                 : row.Name
                                           };

                        content.Append(userLink.RenderToString());
                    }

                    content.Append(@"</li>");
                });

        content.Append("</ol>");

        this.ShowMods.DataContent = content.ToString().ToJsString();
        this.ShowMods.Text = $"{this.GetText("SHOW")} {this.GetText("DEFAULT", "MODERATORS")}";
    }

    #endregion
}