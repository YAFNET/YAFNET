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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Web;

    using YAF.Core.BasePages;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
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
            : base("MOVEMESSAGE", ForumPages.MoveMessage)
        {
        }

        #endregion

        /// <summary>
        /// The move message id.
        /// </summary>
        protected int MoveMessageId =>
            this.Get<LinkBuilder>().StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"));

        #region Methods

        /// <summary>
        /// Handles the PreRender event
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                nameof(JavaScriptBlocks.SelectTopicsLoadJs),
                JavaScriptBlocks.SelectTopicsLoadJs(this.TopicsList.ClientID, this.ForumList.ClientID));
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
                    this.MoveMessageId,
                    this.ForumList.SelectedValue.ToType<int>(),
                    this.TopicSubject.Text);

                this.GetRepository<Message>().Move(
                    this.MoveMessageId,
                    topicId.ToType<int>(),
                    true);

                this.Get<LinkBuilder>().Redirect(
                    ForumPages.Topics,
                    new { f = this.PageBoardContext.PageForumID, name = this.PageBoardContext.PageForum.Name });
            }
            else
            {
                this.PageBoardContext.Notify(this.GetText("Empty_Topic"), MessageTypes.warning);
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ForumList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ForumList_SelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.TopicsList.DataSource = this.GetRepository<Topic>().ListPaged(
                this.ForumList.SelectedValue.ToType<int>(),
                this.PageBoardContext.PageUserID,
                DateTimeHelper.SqlDbMinTime(),
                0,
                100,
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
            if (this.TopicsList.SelectedValue.ToType<int>() != this.PageBoardContext.PageTopicID)
            {
                this.GetRepository<Message>().Move(
                    this.MoveMessageId,
                    this.TopicsList.SelectedValue.ToType<int>(),
                    true);
            }

            this.Get<LinkBuilder>().Redirect(
                ForumPages.Topics,
                new { f = this.PageBoardContext.PageForumID, name = this.PageBoardContext.PageForum.Name });
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Get<HttpRequestBase>().QueryString.Exists("m") || !this.PageBoardContext.ForumModeratorAccess)
            {
                this.Get<LinkBuilder>().AccessDenied();
            }

            if (this.IsPostBack)
            {
                return;
            }

            var forumList = this.GetRepository<Forum>().ListAllSorted(
                this.PageBoardContext.PageBoardID,
                this.PageBoardContext.PageUserID);

            this.ForumList.AddForumAndCategoryIcons(forumList);

            this.ForumList.DataTextField = "Forum";
            this.ForumList.DataValueField = "ForumID";
            this.DataBind();

            this.ForumList.Items.FindByValue(this.PageBoardContext.PageForumID.ToString()).Selected = true;
            this.ForumList_SelectedIndexChanged(this.ForumList, e);
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddCategory(this.PageBoardContext.PageCategory.Name, this.PageBoardContext.PageCategoryID);
            this.PageLinks.AddForum(this.PageBoardContext.PageForumID);
            this.PageLinks.AddTopic(this.PageBoardContext.PageTopic.TopicName, this.PageBoardContext.PageTopicID);

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