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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Forum Moderating Page.
    /// </summary>
    public partial class moderating : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "moderating" /> class.
        /// </summary>
        public moderating()
            : base("MODERATING")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Click event of the AddUser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void AddUser_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.mod_forumuser, "f={0}", this.PageContext.PageForumID);
        }

        /// <summary>
        /// Binds the data
        /// </summary>
        protected void BindData()
        {
            this.PagerTop.PageSize = this.Get<YafBoardSettings>().TopicsPerPage;

            var baseSize = this.Get<YafBoardSettings>().TopicsPerPage;
            var currentPageIndex = this.PagerTop.CurrentPageIndex;

            var topicList = LegacyDb.topic_list(
                this.PageContext.PageForumID,
                null,
                DateTimeHelper.SqlDbMinTime(),
                DateTime.UtcNow,
                currentPageIndex,
                baseSize,
                false,
                true,
                false);

            this.topiclist.DataSource = topicList;
            this.UserList.DataSource = LegacyDb.userforum_list(null, this.PageContext.PageForumID);
           
            if (topicList != null && topicList.HasRows())
            {
                this.PagerTop.Count = topicList.AsEnumerable().First().Field<int>("TotalRows");
            }

            this.ForumList.DataSource = LegacyDb.forum_listall_sorted(
                this.PageContext.PageBoardID,
                this.PageContext.PageUserID);

            this.DataBind();

            var pageItem = this.ForumList.Items.FindByValue(this.PageContext.PageForumID.ToString());

            if (pageItem != null)
            {
                pageItem.Selected = true;
            }
        }

        /// <summary>
        /// Deletes all the Selected Topics
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void DeleteTopics_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var list =
                this.topiclist.Controls.OfType<RepeaterItem>().SelectMany(x => x.Controls.OfType<TopicLine>()).Where(
                    x => x.IsSelected && x.TopicRowID.HasValue).ToList();

            if (!list.Any())
            {
                this.PageContext.AddLoadMessage(this.GetText("MODERATE", "NOTHING"), MessageTypes.warning);
            }
            else
            {
                list.ForEach(x => LegacyDb.topic_delete(x.TopicRowID));

                this.PageContext.AddLoadMessage(this.GetText("moderate", "deleted"), MessageTypes.success);

                this.BindData();
            }
        }

        /// <summary>
        /// Handles the Click event of the Move control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Move_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            int? linkDays = null;
            var ld = -2;

            if (this.LeavePointer.Checked && this.LinkDays.Text.IsSet() && !int.TryParse(this.LinkDays.Text, out ld))
            {
                this.PageContext.AddLoadMessage(this.GetText("POINTER_DAYS_INVALID"), MessageTypes.warning);
                return;
            }

            if (this.ForumList.SelectedValue.ToType<int>() <= 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("CANNOT_MOVE_TO_CATEGORY"), MessageTypes.warning);
                return;
            }

            // only move if it's a destination is a different forum.
            if (this.ForumList.SelectedValue.ToType<int>() != this.PageContext.PageForumID)
            {
                if (ld >= -2)
                {
                    linkDays = ld;
                }

                var list = this.topiclist.Controls.OfType<RepeaterItem>()
                    .SelectMany(x => x.Controls.OfType<TopicLine>()).Where(x => x.IsSelected && x.TopicRowID.HasValue)
                    .ToList();

                if (!list.Any())
                {
                    this.PageContext.AddLoadMessage(this.GetText("MODERATE", "NOTHING"), MessageTypes.warning);
                }
                else
                {
                    list.ForEach(
                        x => LegacyDb.topic_move(
                            x.TopicRowID,
                            this.ForumList.SelectedValue,
                            this.LeavePointer.Checked,
                            linkDays));

                    this.PageContext.AddLoadMessage(this.GetText("MODERATE", "MOVED"), MessageTypes.success);

                    this.BindData();
                }
            }
            else
            {
                this.PageContext.AddLoadMessage(this.GetText("MODERATE", "MOVE_TO_DIFFERENT"), MessageTypes.danger);
            }
        }

        /// <summary>
        /// The delete user_ load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteUser_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("moderate", "confirm_delete_user"));
        }

        /// <summary>
        /// The delete_ load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("moderate", "confirm_delete"));
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageContext.ForumModeratorAccess)
            {
                YafBuildLink.AccessDenied();
            }

            if (!this.PageContext.IsForumModerator || !this.PageContext.IsAdmin)
            {
                this.ModerateUsersHolder.Visible = false;
            }

            if (!this.IsPostBack)
            {
                this.AddUser.Text = this.GetText("MODERATE", "INVITE");

                if (this.PageContext.Settings.LockedForum == 0)
                {
                    this.PageLinks.AddRoot();
                    this.PageLinks.AddLink(
                        this.PageContext.PageCategoryName,
                        YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
                }

                this.PageLinks.AddForum(this.PageContext.PageForumID);
                this.PageLinks.AddLink(this.GetText("MODERATE", "TITLE"), string.Empty);

                this.PagerTop.PageSize = 25;

                this.Move.Text = this.GetText("MOVETOPIC", "MOVE");
                this.Move.ToolTip = "{0}: {1}".FormatWith(this.GetText("MOVETOPIC", "MOVE"), this.PageContext.PageTopicName);

                var showMoved = this.Get<YafBoardSettings>().ShowMoved;

                // Ederon : 7/14/2007 - by default, leave pointer is set on value defined on host level
                this.LeavePointer.Checked = showMoved;

                this.trLeaveLink.Visible = showMoved;
                this.trLeaveLinkDays.Visible = showMoved;

                if (showMoved)
                {
                    this.LinkDays.Text = "1";
                }
            }

            this.BindData();
        }

        /// <summary>
        /// The pager top_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PagerTop_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            // rebind
            this.BindData();
        }

        /// <summary>
        /// The user list_ item command.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void UserList_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":
                    YafBuildLink.Redirect(
                        ForumPages.mod_forumuser, "f={0}&u={1}", this.PageContext.PageForumID, e.CommandArgument);
                    break;
                case "remove":
                    this.GetRepository<UserForum>().Delete(e.CommandArgument.ToType<int>(), this.PageContext.PageForumID);

                    this.BindData();

                    // clear moderatorss cache
                    this.Get<IDataCache>().Remove(Constants.Cache.ForumModerators);
                    break;
            }
        }

        /// <summary>
        /// The topiclist_ item command.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void topiclist_ItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "delete":
                    LegacyDb.topic_delete(e.CommandArgument);
                    this.PageContext.AddLoadMessage(this.GetText("deleted"), MessageTypes.success);
                    this.BindData();
                    break;
            }
        }

        #endregion
    }
}