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
    using System.Web.UI.WebControls;

    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects.Model;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Post Message History Page.
    /// </summary>
    public partial class MessageHistory : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   To save Forum ID value.
        /// </summary>
        private int forumID;

        /// <summary>
        ///   To save Message ID value.
        /// </summary>
        private int messageID;

        /// <summary>
        ///   To save originalRow value.
        /// </summary>
        private Tuple<Topic, Message, User, Forum> originalMessage;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MessageHistory" /> class.
        /// </summary>
        public MessageHistory()
            : base("MESSAGEHISTORY")
        {
        }

        #endregion

        /// <summary>
        /// Gets or sets the revisions count.
        /// </summary>
        /// <value>
        /// The revisions count.
        /// </value>
        protected int RevisionsCount { get; set; }

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.PageContext.IsGuest)
            {
                this.Get<LinkBuilder>().AccessDenied();
            }

            if (this.Get<HttpRequestBase>().QueryString.Exists("m"))
            {
                this.messageID =
                    this.Get<LinkBuilder>().StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"));

                this.ReturnBtn.Visible = true;
            }

            if (this.Get<HttpRequestBase>().QueryString.Exists("f"))
            {
                // We check here if the user have access to the option
                if (this.PageContext.IsGuest)
                {
                    this.Get<HttpResponseBase>().Redirect(this.Get<LinkBuilder>().GetLink(ForumPages.Info, "i=4"));
                }

                this.forumID =
                    this.Get<LinkBuilder>().StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f"));

                this.ReturnModBtn.Visible = true;
            }

            this.originalMessage = this.GetRepository<Message>().GetMessageWithAccess(this.messageID, this.PageContext.PageUserID);

            if (this.originalMessage == null)
            {
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddForum(this.originalMessage.Item4.ID);
            this.PageLinks.AddTopic(this.originalMessage.Item1.TopicName, this.originalMessage.Item1.ID);

            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

            this.BindData();
        }

        /// <summary>
        /// The create page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
        }

        /// <summary>
        /// Redirect to the changed post
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ReturnBtn_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Get<LinkBuilder>().Redirect(
                ForumPages.Posts,
                "m={0}&name={1}",
                this.messageID,
                this.originalMessage.Item1.TopicName);
        }

        /// <summary>
        /// Redirect to the changed post
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ReturnModBtn_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Get<LinkBuilder>().Redirect(ForumPages.Moderate_ReportedPosts, "f={0}", this.forumID);
        }

        /// <summary>
        /// Handle Commands for restoring an old Message Version
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void RevisionsList_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "restore":
                    var currentMessage = this.GetRepository<Message>().GetMessage(this.messageID);

                    var edited = e.CommandArgument.ToType<DateTime>();

                    var messageToRestore = this.GetRepository<Types.Models.MessageHistory>().GetSingle(
                        m => m.MessageID == this.messageID && m.Edited == edited);

                    if (messageToRestore != null)
                    {
                        this.GetRepository<Message>().Update(
                            null,
                            messageToRestore.Message,
                            null,
                            null,
                            null,
                            null,
                            messageToRestore.EditReason,
                            this.PageContext.PageUserID != currentMessage.Item1.UserID,
                            this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess,
                            currentMessage,
                            this.PageContext.PageUserID);

                        this.PageContext.AddLoadMessage(this.GetText("MESSAGE_RESTORED"), MessageTypes.success);

                        this.BindData();
                    }

                    break;
            }
        }

        /// <summary>
        /// The get IP address.
        /// </summary>
        /// <param name="dataItem">
        /// The data item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string GetIpAddress(MessageHistoryTopic dataItem)
        {
            var ip = IPHelper.GetIpAddressAsString(dataItem.IP);

            return ip.IsSet() ? ip : IPHelper.GetIpAddressAsString(dataItem.MessageIP);
        }

        /// <summary>
        /// Binds data to data source
        /// </summary>
        private void BindData()
        {
            // Fill revisions list repeater.
            var revisionsTable = this.GetRepository<Types.Models.MessageHistory>().List(
                this.messageID,
                this.PageContext.BoardSettings.MessageHistoryDaysToLog);

            this.RevisionsCount = revisionsTable.Count;

            this.RevisionsList.DataSource = revisionsTable;

            this.DataBind();
        }

        #endregion
    }
}