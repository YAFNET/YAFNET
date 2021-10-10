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

namespace YAF.Pages.Moderate
{
    #region Using

    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Moderating Page for Unapproved Posts.
    /// </summary>
    public partial class UnapprovedPosts : ModerateForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "UnapprovedPosts" /> class. 
        ///   Default constructor.
        /// </summary>
        public UnapprovedPosts()
            : base("MODERATE_FORUM")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // forum index
            this.PageLinks.AddRoot();

            // moderation index
            this.PageLinks.AddLink(
                this.GetText("MODERATE_DEFAULT", "TITLE"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Moderate_Index));

            // current page
            this.PageLinks.AddLink(this.PageContext.PageForum.Name);
        }

        /// <summary>
        /// Format message.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// Formatted string with escaped HTML markup and formatted.
        /// </returns>
        protected string FormatMessage([NotNull] Tuple<Topic, Message, User> item)
        {
            // get message flags
            var messageFlags = item.Item2.MessageFlags;

            // message
            string msg;

            // format message?
            if (messageFlags.NotFormatted)
            {
                // just encode it for HTML output
                msg = this.HtmlEncode(item.Item2.MessageText);
            }
            else
            {
                // fully format message (YafBBCode)
                msg = this.Get<IFormatMessage>().Format(
                    item.Item2.ID,
                    item.Item2.MessageText,
                    messageFlags,
                    item.Item2.IsModeratorChanged.Value);
            }

            // return formatted message
            return msg;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.List.ItemCommand += this.List_ItemCommand;
            base.OnInit(e);
        }

        /// <summary>
        /// Handles page load event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // do this just on page load, not post-backs
            if (this.IsPostBack)
            {
                return;
            }

            // bind data
            this.BindData();
        }

        /// <summary>
        /// Bind data for this control.
        /// </summary>
        private void BindData()
        {
            var messageList = this.GetRepository<Message>().Unapproved(this.PageContext.PageForumID);

            if (!messageList.Any())
            {
                // redirect back to the moderate main if no messages found
                this.Get<LinkBuilder>().Redirect(ForumPages.Moderate_Index);
            }
            else
            {
                this.List.DataSource = messageList;

                // bind data to controls
                this.DataBind();
            }
        }

        /// <summary>
        /// Handles post moderation events/buttons.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        private void List_ItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
        {
            // which command are we handling
            switch (e.CommandName.ToLower())
            {
                case "approve":

                    // approve post
                    this.GetRepository<Message>().Approve(
                        e.CommandArgument.ToType<int>(),
                        this.PageContext.PageForumID);

                    // re-bind data
                    this.BindData();

                    // tell user message was approved
                    this.PageContext.AddLoadMessage(this.GetText("APPROVED"), MessageTypes.success);

                    // send notification to watching users...
                    this.Get<ISendNotification>().ToWatchingUsers(e.CommandArgument.ToType<int>());
                    break;
                case "delete":

                    var commandArgs = e.CommandArgument.ToString().Split(';');

                    var topicId = commandArgs[1].ToType<int>();
                    var messageId = commandArgs[0].ToType<int>();

                    // delete message
                    this.GetRepository<Message>().Delete(
                        this.PageContext.PageForumID,
                        topicId,
                        messageId,
                        true,
                        string.Empty,
                        true,
                        true);

                    // re-bind data
                    this.BindData();

                    // tell user message was deleted
                    this.PageContext.AddLoadMessage(this.GetText("DELETED"), MessageTypes.info);
                    break;
            }

            this.BindData();
        }

        #endregion
    }
}