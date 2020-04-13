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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Web;

    using YAF.Core.BasePages;
    using YAF.Core.Model;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Move Message Page
    /// </summary>
    public partial class MoveMessage : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MoveMessage" /> class.
        /// </summary>
        public MoveMessage()
            : base("MOVEMESSAGE")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the PreRender event
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            this.PageContext.PageElements.RegisterJsBlockStartup(
                "fileUploadjs",
                JavaScriptBlocks.SelectTopicsLoadJs(this.ForumList.ClientID));
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
                var topicId = this.GetRepository<Topic>().CreateByMessage(
                    this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("m"),
                    this.ForumList.SelectedValue.ToType<int>(),
                    this.TopicSubject.Text);

                this.GetRepository<Message>().Move(
                    this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("m"),
                    topicId.ToType<int>(),
                    true);

                BuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
            }
            else
            {
                this.PageContext.AddLoadMessage(this.GetText("Empty_Topic"), MessageTypes.warning);
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ForumList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ForumList_SelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.TopicsList.DataSource = this.GetRepository<Topic>().ListAsDataTable(
                this.ForumList.SelectedValue.ToType<int>(),
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
            this.CreateAndMove.Visible = this.ForumList.SelectedValue.ToType<int>() > 0;
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
                this.GetRepository<Message>().Move(
                    this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("m"),
                    this.TopicsList.SelectedValue.ToType<int>(),
                    true);
            }

            BuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Get<HttpRequestBase>().QueryString.Exists("m") || !this.PageContext.ForumModeratorAccess)
            {
                BuildLink.AccessDenied();
            }

            if (this.IsPostBack)
            {
                return;
            }

            var forumList = this.GetRepository<Forum>().ListAllSortedAsDataTable(
                this.PageContext.PageBoardID,
                this.PageContext.PageUserID);

            this.ForumList.AddForumAndCategoryIcons(forumList);

            this.ForumList.DataTextField = "Title";
            this.ForumList.DataValueField = "ForumID";
            this.DataBind();

            this.ForumList.Items.FindByValue(this.PageContext.PageForumID.ToString()).Selected = true;
            this.ForumList_SelectedIndexChanged(this.ForumList, e);
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();

            this.PageLinks.AddLink(
                this.PageContext.PageCategoryName,
                BuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
            this.PageLinks.AddForum(this.PageContext.PageForumID);
            this.PageLinks.AddLink(
                this.PageContext.PageTopicName,
                BuildLink.GetLink(ForumPages.Posts, "t={0}", this.PageContext.PageTopicID));

            this.PageLinks.AddLink(this.GetText("MOVE_MESSAGE"));
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the TopicsList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void TopicsList_SelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Move.Visible = this.TopicsList.SelectedValue != string.Empty;
        }

        #endregion
    }
}