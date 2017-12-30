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

    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Move Message Page
    /// </summary>
    public partial class movemessage : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "movemessage" /> class.
        /// </summary>
        public movemessage()
            : base("MOVEMESSAGE")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.PreRender += this.MoveMessage_PreRender;
            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Click event of the CreateAndMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void CreateAndMove_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.TopicSubject.Text.IsSet())
            {
                var topicId = LegacyDb.topic_create_by_message(
                    this.Request.QueryString.GetFirstOrDefault("m"),
                    this.ForumList.SelectedValue,
                    this.TopicSubject.Text);
                LegacyDb.message_move(this.Request.QueryString.GetFirstOrDefault("m"), topicId, true);
                YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
            }
            else
            {
                this.PageContext.AddLoadMessage(this.GetText("Empty_Topic"));
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ForumList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ForumList_SelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.TopicsList.DataSource = LegacyDb.topic_list(
                this.ForumList.SelectedValue,
                null,
                DateTimeHelper.SqlDbMinTime(),
                DateTime.UtcNow,
                0,
                100,
                false,
                false,
                false);

            this.TopicsList.DataTextField = "Subject";
            this.TopicsList.DataValueField = "TopicID";

            this.TopicsList.DataBind();

            this.TopicsList_SelectedIndexChanged(this.ForumList, e);
            this.CreateAndMove.Enabled = this.ForumList.SelectedValue.ToType<int>() > 0;
        }

        /// <summary>
        /// Handles the Click event of the Move control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Move_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.TopicsList.SelectedValue.ToType<int>() != this.PageContext.PageTopicID)
            {
                LegacyDb.message_move(
                    this.Request.QueryString.GetFirstOrDefault("m"),
                    this.TopicsList.SelectedValue,
                    true);
            }

            YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.Request.QueryString.GetFirstOrDefault("m") == null || !this.PageContext.ForumModeratorAccess)
            {
                YafBuildLink.AccessDenied();
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddRoot();

            this.PageLinks.AddLink(
                this.PageContext.PageCategoryName,
                YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
            this.PageLinks.AddForum(this.PageContext.PageForumID);
            this.PageLinks.AddLink(
                this.PageContext.PageTopicName,
                YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.PageContext.PageTopicID));

            this.PageLinks.AddLink(this.GetText("MOVE_MESSAGE"));

            this.Move.Text = this.GetText("MOVE_MESSAGE");
            this.Move.ToolTip = this.GetText("MOVE_TITLE");
            this.CreateAndMove.Text = this.GetText("CREATE_TOPIC");
            this.CreateAndMove.ToolTip = this.GetText("SPLIT_TITLE");

            this.ForumList.DataSource = LegacyDb.forum_listall_sorted(
                this.PageContext.PageBoardID,
                this.PageContext.PageUserID);
            this.ForumList.DataTextField = "Title";
            this.ForumList.DataValueField = "ForumID";
            this.DataBind();

            this.ForumList.Items.FindByValue(this.PageContext.PageForumID.ToString()).Selected = true;
            this.ForumList_SelectedIndexChanged(this.ForumList, e);
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the TopicsList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void TopicsList_SelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Move.Enabled = this.TopicsList.SelectedValue != string.Empty;
        }

        /// <summary>
        /// Handles the PreRender event of the AttachmentsUploadDialog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void MoveMessage_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "fileUploadjs",
                JavaScriptBlocks.SelectTopicsLoadJs(this.ForumList.ClientID));
        }

        #endregion
    }
}