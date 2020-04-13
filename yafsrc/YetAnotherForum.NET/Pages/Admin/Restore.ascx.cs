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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
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
    /// The Admin Restore Topics Page
    /// </summary>
    public partial class Restore : AdminPage
    {
        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.BindData();
            }
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.Admin_Admin));
            this.PageLinks.AddLink(this.GetText("ADMIN_RESTORE", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_RESTORE", "TITLE")}";
        }

        /// <summary>
        /// The list_ item command.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void List_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var topicId = e.CommandArgument.ToType<int>();

            switch (e.CommandName)
            {
                case "delete":
                    {
                        this.GetRepository<Topic>().Delete(topicId, true);

                        this.PageContext.AddLoadMessage(this.GetText("MSG_DELETED"), MessageTypes.success);

                        this.BindData();
                    }

                    break;
                case "restore":
                    {
                        var getFirstMessage = this.GetRepository<Message>()
                            .GetSingle(m => m.TopicID == topicId && m.Position == 0);

                        if (getFirstMessage != null)
                        {
                            this.GetRepository<Message>().Delete(
                                getFirstMessage.ID,
                                true,
                                string.Empty,
                                0,
                                true,
                                false);
                        }

                        var topic = this.GetRepository<Topic>().GetById(topicId);

                        var flags = topic.TopicFlags;

                        flags.IsDeleted = false;

                        this.GetRepository<Topic>().UpdateOnly(
                            () => new Topic { Flags = flags.BitValue },
                            t => t.ID == topicId);

                        this.PageContext.AddLoadMessage(this.GetText("MSG_RESTORED"), MessageTypes.success);

                        this.BindData();
                    }

                    break;
                case "delete_all":
                    {
                        var topicIds = (from RepeaterItem item in this.DeletedTopics.Items
                                        select item.FindControlAs<HiddenField>("hiddenID") into hiddenId
                                        select hiddenId.Value.ToType<int>()).ToList();

                        topicIds.ForEach(
                            topic =>
                            {
                                this.GetRepository<Topic>().Delete(topic, true);
                            });

                        this.PageContext.AddLoadMessage(this.GetText("MSG_DELETED"), MessageTypes.success);

                        this.PagerTop.CurrentPageIndex--;

                        this.BindData();
                    }

                    break;

                case "delete_complete":
                    {
                        var deletedTopics = this.GetRepository<Topic>().GetDeletedTopics(BoardContext.Current.PageBoardID, this.Filter.Text);

                        deletedTopics.ForEach(
                            topic =>
                            {
                                this.GetRepository<Topic>().Delete(topic.Item2.ID, true);
                            });

                        this.PageContext.AddLoadMessage(this.GetText("MSG_DELETED"), MessageTypes.success);

                        this.BindData();
                    }

                    break;
                case "delete_zero":
                    {
                        var deletedTopics = this.GetRepository<Topic>().Get(t => t.IsDeleted == true && t.NumPosts.Equals(0));

                        deletedTopics.ForEach(
                            topic =>
                                {
                                    this.GetRepository<Topic>().Delete(topic.ID, true);
                                });

                        this.PageContext.AddLoadMessage(this.GetText("MSG_DELETED"), MessageTypes.success);

                        this.BindData();
                    }

                    break;
            }
        }

        /// <summary>
        /// The messages_ item command.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Messages_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var messageId = e.CommandArgument.ToType<int>();

            switch (e.CommandName)
            {
                case "delete":
                    {
                        // delete message
                        this.GetRepository<Message>().Delete(messageId, true, string.Empty, 1, true, true);

                        this.PageContext.AddLoadMessage(this.GetText("MSG_DELETED"), MessageTypes.success);

                        this.BindData();
                    }

                    break;
                case "restore":
                    {
                        this.GetRepository<Message>().Delete(
                            messageId,
                            true,
                            string.Empty,
                            0,
                            true,
                            false);

                        this.PageContext.AddLoadMessage(this.GetText("MSG_RESTORED"), MessageTypes.success);

                        this.BindData();
                    }

                    break;
                case "delete_all":
                    {
                        var messageIds = (from RepeaterItem item in this.DeletedMessages.Items
                                          select item.FindControlAs<HiddenField>("hiddenID")
                                          into hiddenId
                                          select hiddenId.Value.ToType<int>()).ToList();

                        messageIds.ForEach(
                            message =>
                                {
                                    this.GetRepository<Message>().Delete(message, true, string.Empty, 1, true, true);
                                });

                        this.PageContext.AddLoadMessage(this.GetText("MSG_DELETED"), MessageTypes.success);

                        this.PagerTop.CurrentPageIndex--;

                        this.BindData();
                    }

                    break;
            }
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
        /// The refresh click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void RefreshClick(object sender, EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.PagerTop.PageSize = this.PageContext.BoardSettings.TopicsPerPage;
            this.PagerMessages.PageSize = this.PageContext.BoardSettings.TopicsPerPage;

            var deletedTopics = this.GetRepository<Topic>()
                .GetDeletedTopics(BoardContext.Current.PageBoardID, this.Filter.Text);

            var count = deletedTopics.Count;

            var deletedTopicPaged = deletedTopics.GetPaged(this.PagerTop);

            this.DeletedTopics.DataSource = deletedTopicPaged;
            this.DeletedTopics.DataBind();

            this.PagerTop.Count = deletedTopicPaged.Any()
                                      ? count
                                      : 0;

            var deletedMessages = this.GetRepository<Message>()
                .GetDeletedMessages(BoardContext.Current.PageBoardID);

            count = deletedMessages.Count;

            var deletedMessagesPaged = deletedMessages.GetPaged(this.PagerMessages);

            this.DeletedMessages.DataSource = deletedMessagesPaged;
            this.DeletedMessages.DataBind();

            this.PagerMessages.Count = deletedMessagesPaged.Any()
                                      ? count
                                      : 0;
        }

        #endregion
    }
}