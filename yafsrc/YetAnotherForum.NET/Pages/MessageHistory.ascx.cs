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
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Core.BasePages;
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
                BuildLink.AccessDenied();
            }

            if (this.Get<HttpRequestBase>().QueryString.Exists("m"))
            {
                this.messageID =
                    Security.StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"));

                this.ReturnBtn.Visible = true;
            }

            if (this.Get<HttpRequestBase>().QueryString.Exists("f"))
            {
                // We check here if the user have access to the option
                if (this.PageContext.IsGuest)
                {
                    this.Get<HttpResponseBase>().Redirect(BuildLink.GetLinkNotEscaped(ForumPages.Info, "i=4"));
                }

                this.forumID =
                    Security.StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f"));

                this.ReturnModBtn.Visible = true;
            }

            this.originalMessage = this.GetRepository<Message>().GetMessageWithAccess(this.messageID, this.PageContext.PageUserID);

            if (this.originalMessage == null)
            {
                this.Get<HttpResponseBase>().Redirect(
                    BuildLink.GetLink(ForumPages.Error, "Incorrect message value: {0}", this.messageID));
            }

            this.PageLinks.AddForum(this.originalMessage.Item4.ID);
            this.PageLinks.AddTopic(this.originalMessage.Item1.TopicName, this.originalMessage.Item1.ID);

            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

            if (this.IsPostBack)
            {
                return;
            }

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
            BuildLink.Redirect(
                ForumPages.Posts,
                "m={0}&name={1}#post{0}",
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
            BuildLink.Redirect(ForumPages.Moderate_ReportedPosts, "f={0}", this.forumID);
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

                    DataRow restoreMessage = null;

                    var revisionsTable = this.GetRepository<Message>().HistoryListAsDataTable(
                        this.messageID,
                        this.PageContext.BoardSettings.MessageHistoryDaysToLog).AsEnumerable();

                    Enumerable.Where(
                            revisionsTable,
                            row => row["Edited"].ToString().Equals(e.CommandArgument.ToType<string>()))
                        .ForEach(row => restoreMessage = row);

                    if (restoreMessage != null)
                    {
                        this.GetRepository<Message>().Update(
                            this.messageID,
                            null,
                            restoreMessage["Message"].ToString(),
                            null,
                            null,
                            null,
                            null,
                            restoreMessage["EditReason"].ToString(),
                            this.PageContext.PageUserID != currentMessage.Item1.UserID,
                            this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess,
                            currentMessage,
                            this.PageContext.PageUserID);

                        this.PageContext.AddLoadMessage(this.GetText("MESSAGE_RESTORED"), MessageTypes.success);
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
        protected string GetIpAddress(object dataItem)
        {
            var row = (DataRow)dataItem;

            var ip = IPHelper.GetIp4Address(row["IP"].ToString());

            return ip.IsSet() ? ip : IPHelper.GetIp4Address(row["MessageIP"].ToString());
        }

        /// <summary>
        /// Binds data to data source
        /// </summary>
        private void BindData()
        {
            // Fill revisions list repeater.
            var revisionsTable = this.GetRepository<Message>().HistoryListAsDataTable(
                this.messageID,
                this.PageContext.BoardSettings.MessageHistoryDaysToLog);

            this.RevisionsCount = revisionsTable.Rows.Count;

            this.RevisionsList.DataSource = revisionsTable.AsEnumerable();

            this.DataBind();
        }

        #endregion
    }
}