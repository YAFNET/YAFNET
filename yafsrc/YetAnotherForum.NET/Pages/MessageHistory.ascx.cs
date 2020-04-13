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

    using YAF.Core;
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
        private DataTable originalRow;

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

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m").IsSet())
            {
                if (!int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"), out this.messageID))
                {
                    this.Get<HttpResponseBase>().Redirect(
                        BuildLink.GetLink(ForumPages.Error, "Incorrect message value: {0}", this.messageID));
                }

                this.ReturnBtn.Visible = true;
            }

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f").IsSet())
            {
                // We check here if the user have access to the option
                if (this.PageContext.IsGuest)
                {
                    this.Get<HttpResponseBase>().Redirect(BuildLink.GetLinkNotEscaped(ForumPages.Info, "i=4"));
                }

                if (!int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f"), out this.forumID))
                {
                    this.Get<HttpResponseBase>().Redirect(
                        BuildLink.GetLink(ForumPages.Error, "Incorrect forum value: {0}", this.forumID));
                }

                this.ReturnModBtn.Visible = true;
            }

            this.originalRow = this.GetRepository<Message>().SecAsDataTable(this.messageID, this.PageContext.PageUserID);

            if (this.originalRow.Rows.Count <= 0)
            {
                this.Get<HttpResponseBase>().Redirect(
                    BuildLink.GetLink(ForumPages.Error, "Incorrect message value: {0}", this.messageID));
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, BuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

            this.BindData();
        }

        /// <summary>
        /// Redirect to the changed post
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ReturnBtn_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Get<HttpResponseBase>().Redirect(BuildLink.GetLinkNotEscaped(ForumPages.Posts, "m={0}#post{0}", this.messageID));
        }

        /// <summary>
        /// Redirect to the changed post
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ReturnModBtn_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Get<HttpResponseBase>().Redirect(
                BuildLink.GetLinkNotEscaped(ForumPages.Moderate_ReportedPosts, "f={0}", this.forumID));
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
                    var currentMessage = this.GetRepository<Message>().MessageList(this.messageID).FirstOrDefault();

                    DataRow restoreMessage = null;

                    var revisionsTable = this.GetRepository<Message>().HistoryListAsDataTable(
                        this.messageID,
                        this.PageContext.BoardSettings.MessageHistoryDaysToLog).AsEnumerable();

                    Enumerable.Where(
                            revisionsTable,
                            row => row["Edited"].ToType<string>().Equals(e.CommandArgument.ToType<string>()))
                        .ForEach(row => restoreMessage = row);

                    if (restoreMessage != null)
                    {
                        this.GetRepository<Message>().Update(
                            this.messageID,
                            currentMessage.Priority.Value,
                            restoreMessage["Message"].ToString(),
                            currentMessage.Description,
                            currentMessage.Status,
                            currentMessage.Styles,
                            currentMessage.Topic,
                            currentMessage.Flags.BitValue,
                            restoreMessage["EditReason"].ToString(),
                            this.PageContext.PageUserID != currentMessage.UserID,
                            this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess,
                            currentMessage,
                            this.PageContext.PageUserID);

                        this.PageContext.AddLoadMessage(this.GetText("MESSAGE_RESTORED"), MessageTypes.success);
                    }

                    break;
            }
        }

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

            revisionsTable.AcceptChanges();

            revisionsTable.Merge(this.GetRepository<Message>().SecAsDataTable(this.messageID, this.PageContext.PageUserID));

            this.RevisionsCount = revisionsTable.Rows.Count;

            this.RevisionsList.DataSource = revisionsTable.AsEnumerable();

            this.DataBind();
        }

        #endregion
    }
}