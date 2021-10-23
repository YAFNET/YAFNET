/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using YAF.Types.Objects.Model;
    using YAF.Web.Controls;

    #endregion

    /// <summary>
    /// The Forum List Control
    /// </summary>
    public partial class ForumList : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        /// The Data Source
        /// </summary>
        private Tuple<List<SimpleModerator>, List<ForumRead>> dataSource;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets DataSource.
        /// </summary>
        public Tuple<List<SimpleModerator>, List<ForumRead>> DataSource
        {
            get => this.dataSource;

            set
            {
                this.dataSource = value;

                this.ForumList1.DataSource = this.PageContext.PageForumID > 0
                    ? this.dataSource.Item2
                    : this.dataSource.Item2.Where(x => !x.ParentID.HasValue);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Provides the "Forum Link Text" for the ForumList control.
        ///   Automatically disables the link if the current user doesn't
        ///   have proper permissions.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// Forum link text
        /// </returns>
        public string GetForumLink([NotNull] ForumRead item)
        {
            var forumID = item.ForumID;

            // get the Forum Description
            var output = item.Forum;

            if (item.ReadAccess)
            {
                var title = item.Description.IsSet()
                                ? item.Description
                                : this.GetText("COMMON", "VIEW_FORUM");

                output = item.RemoteURL.IsSet()
                             ? $"<a href=\"{item.RemoteURL}\" title=\"{this.GetText("COMMON", "VIEW_FORUM")}\" target=\"_blank\">{this.Page.HtmlEncode(output)}&nbsp;<i class=\"fas fa-external-link-alt fa-fw\"></i></a>"
                             : $"<a href=\"{this.Get<LinkBuilder>().GetForumLink(forumID, output)}\" data-bs-toggle=\"tooltip\" title=\"{title}\">{this.Page.HtmlEncode(output)}</a>";
            }
            else
            {
                // no access to this forum
                output = $"{output} {this.GetText("NO_FORUM_ACCESS")}";
            }

            return output;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the ItemCreated event of the ForumList1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void ForumList1_ItemCreated([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var item = (ForumRead)e.Item.DataItem;
            var flags = new ForumFlags(item.Flags);

            var lastRead = this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                item.ForumID,
                item.LastTopicID,
                item.LastForumAccess ?? DateTimeHelper.SqlDbMinTime(),
                item.LastTopicAccess ?? DateTimeHelper.SqlDbMinTime());

            var lastPosted = item.LastPosted ?? lastRead;

            if (item.ImageURL.IsNotSet())
            {
                var forumIcon = e.Item.FindControlAs<PlaceHolder>("ForumIcon");

                var forumIconNew = new Icon { IconName = "comments", IconSize = "fa-2x", IconType = "text-success" };
                var forumIconNormal =
                    new Icon { IconName = "comments", IconSize = "fa-2x", IconType = "text-secondary" };
                var forumIconLocked = new Icon
                {
                    IconName = "comments",
                    IconStackName = "lock",
                    IconStackType = "text-warning",
                    IconStackSize = "fa-1x",
                    IconType = "text-secondary"
                };

                var icon = new Literal
                {
                    Text =
                                       $@"<a tabindex=""0"" class=""btn btn-link m-0 p-0 forum-icon-legend-popvover"" role=""button"" data-bs-toggle=""popover"" href=""#"">
                                                      {forumIconNew.RenderToString()}
                                                  </a>"
                };

                try
                {
                    if (flags.IsLocked)
                    {
                        icon.Text =
                            $@"<a tabindex=""0"" class=""btn btn-link m-0 p-0 forum-icon-legend-popvover"" role=""button"" data-bs-toggle=""popover"" href=""#"">
                                   {forumIconLocked}
                               </a>";
                    }
                    else if (lastPosted > lastRead && item.ReadAccess)
                    {
                        icon.Text =
                            $@"<a tabindex=""0"" class=""btn btn-link m-0 p-0 forum-icon-legend-popvover"" role=""button"" data-bs-toggle=""popover"" href=""#"">
                                    {forumIconNew.RenderToString()}
                               </a>";
                    }
                    else
                    {
                        icon.Text =
                            $@"<a tabindex=""0"" class=""btn btn-link m-0 p-0 forum-icon-legend-popvover"" role=""button"" data-bs-toggle=""popover"" href=""#"">
                                  <span class=""fa-stack"">
                                       {forumIconNormal.RenderToString()}
                                  </span>
                              </a>";
                    }
                }
                finally
                {
                    forumIcon.Controls.Add(icon);
                }
            }
            else
            {
                var forumImage = e.Item.FindControlAs<Image>("ForumImage1");
                if (forumImage != null)
                {
                    forumImage.ImageUrl =
                        $"{BoardInfo.ForumClientFileRoot}{this.Get<BoardFolders>().Forums}/{item.ImageURL}";

                    // Highlight custom icon images and add tool tips to them. 
                    try
                    {
                        if (flags.IsLocked)
                        {
                            forumImage.CssClass = "forum_customimage_locked";
                            forumImage.AlternateText = this.GetText("ICONLEGEND", "FORUM_LOCKED");
                            forumImage.ToolTip = this.GetText("ICONLEGEND", "FORUM_LOCKED");
                        }
                        else if (lastPosted > lastRead)
                        {
                            forumImage.CssClass = "forum_customimage_newposts";
                            forumImage.AlternateText = this.GetText("ICONLEGEND", "NEW_POSTS");
                            forumImage.ToolTip = this.GetText("ICONLEGEND", "NEW_POSTS");
                        }
                        else
                        {
                            forumImage.CssClass = "forum_customimage_nonewposts";
                            forumImage.AlternateText = this.GetText("ICONLEGEND", "NO_NEW_POSTS");
                            forumImage.ToolTip = this.GetText("ICONLEGEND", "NO_NEW_POSTS");
                        }

                        forumImage.Visible = true;
                    }
                    catch
                    {
                        forumImage.Visible = false;
                    }
                }
            }

            if (!this.PageContext.BoardSettings.ShowModeratorList)
            {
                return;
            }

            if (item.RemoteURL.IsSet())
            {
                return;
            }

            var modList = e.Item.FindControlAs<ForumModeratorList>("ForumModeratorListMob");

            var mods = this.DataSource.Item1.Where(x => x.ForumID == item.ForumID).ToList();

            if (!mods.Any())
            {
                return;
            }

            modList.DataSource = mods;
            modList.Visible = true;
            modList.DataBind();
        }

        /// <summary>
        /// Gets the moderated.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The get moderated.
        /// </returns>
        protected bool GetModerated([NotNull] ForumRead item)
        {
            return item.ForumFlags.IsModerated;
        }

        /// <summary>
        /// Gets the sub Forums.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// Returns the Sub Forums
        /// </returns>
        protected IEnumerable<ForumRead> GetSubForums([NotNull] ForumRead item)
        {
            if (!this.HasSubForums(item))
            {
                return null;
            }

            var subForums = this.DataSource;

            return subForums.Item2.Where(forum => forum.ParentID == item.ForumID)
                .Take(this.PageContext.BoardSettings.SubForumsInForumList);
        }

        /// <summary>
        /// Gets the viewing.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The get viewing.
        /// </returns>
        protected string GetViewing([NotNull] ForumRead item)
        {
            var viewing = item.Viewing;

            return viewing > 0
                       ? $"<i class=\"far fa-eye\" title=\"{this.GetTextFormatted("VIEWING", viewing)}\"></i> {viewing}"
                       : string.Empty;
        }

        /// <summary>
        /// Determines whether the specified row has sub forums.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The has sub forums.
        /// </returns>
        protected bool HasSubForums([NotNull] ForumRead item)
        {
            return this.DataSource.Item2.Any(forum => forum.ParentID == item.ForumID);
        }

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            var iconLegend = this.LoadControl($"{BoardInfo.ForumServerFileRoot}controls/ForumIconLegend.ascx")
                .RenderToString();

            // setup jQuery and DatePicker JS...
            this.PageContext.PageElements.RegisterJsBlockStartup(
                "ForumIconLegendPopoverJs",
                JavaScriptBlocks.ForumIconLegendPopoverJs(
                    iconLegend.ToJsString(),
                    "forum-icon-legend-popvover"));

            base.OnPreRender(e);
        }

        /// <summary>
        /// Gets the Posts string
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// Returns the Posts string
        /// </returns>
        protected string Posts([NotNull] ForumRead item)
        {
            return item.RemoteURL.IsNotSet() ? $"{item.Posts:N0}" : "-";
        }

        /// <summary>
        /// Gets the Topics string
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// Returns the Topics string
        /// </returns>
        protected string Topics([NotNull] ForumRead item)
        {
            return item.RemoteURL.IsNotSet() ? $"{item.Topics:N0}" : "-";
        }

        #endregion
    }
}