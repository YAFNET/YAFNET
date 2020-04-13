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
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Private Message page
    /// </summary>
    public partial class PrivateMessage : ForumPageRegistered
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateMessage"/> class.
        /// </summary>
        public PrivateMessage()
            : base("MESSAGE")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether IsArchived.
        /// </summary>
        protected bool IsArchived
        {
            get => this.ViewState["IsArchived"] != null && (bool)this.ViewState["IsArchived"];

            set => this.ViewState["IsArchived"] = value;
        }

        /// <summary>
        ///   Gets or sets a value indicating whether IsOutbox.
        /// </summary>
        protected bool IsOutbox
        {
            get => this.ViewState["IsOutbox"] != null && (bool)this.ViewState["IsOutbox"];

            set => this.ViewState["IsOutbox"] = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the ItemCommand event of the Inbox control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs" /> instance containing the event data.</param>
        protected void Inbox_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "delete":
                    this.GetRepository<PMessage>().DeleteMessage(e.CommandArgument.ToType<int>(), this.IsOutbox);

                    this.BindData();
                    this.PageContext.AddLoadMessage(this.GetText("msg_deleted"), MessageTypes.success);
                    BuildLink.Redirect(ForumPages.PM);
                    break;
                case "reply":
                    BuildLink.Redirect(ForumPages.PostPrivateMessage, "p={0}&q=0", e.CommandArgument);
                    break;
                case "report":
                    BuildLink.Redirect(ForumPages.PostPrivateMessage, "p={0}&q=1&report=1", e.CommandArgument);
                    break;
                case "quote":
                    BuildLink.Redirect(ForumPages.PostPrivateMessage, "p={0}&q=1", e.CommandArgument);
                    break;
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // check if this feature is disabled
            if (!this.Get<BoardSettings>().AllowPrivateMessages)
            {
                BuildLink.RedirectInfoPage(InfoMessage.Disabled);
            }

            if (this.IsPostBack)
            {
                return;
            }

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("pm").IsNotSet())
            {
                BuildLink.AccessDenied();
            }

            // handle custom YafBBCode javascript or CSS...
            this.Get<IBBCode>().RegisterCustomBBCodePageElements(this.Page, this.GetType());

            this.BindData();
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.Get<BoardSettings>().EnableDisplayName
                    ? this.PageContext.CurrentUserData.DisplayName
                    : this.PageContext.PageUserName,
                BuildLink.GetLink(ForumPages.Account));
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            using (
                var dt =
                    this.GetRepository<PMessage>().ListAsDataTable(
                        Security.StringToLongOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("pm"))))
            {
                if (dt.HasRows())
                {
                    var row = dt.GetFirstRow();

                    // Check if Message is Reply
                    if (!row["ReplyTo"].IsNullOrEmptyDBField())
                    {
                        var replyTo = row["ReplyTo"].ToType<int>();

                        var message = new PMessage
                                          {
                                              ReplyTo = row["ReplyTo"].ToType<int>(),
                                              ID = row["PMessageID"].ToType<int>()
                                          };

                        dt.Merge(this.GetRepository<PMessage>().GetReplies(message, replyTo));
                    }

                    var dataView = dt.DefaultView;
                    dataView.Sort = "Created ASC";

                    this.SetMessageView(
                        row["FromUserID"],
                        row["ToUserID"],
                        Convert.ToBoolean(row["IsInOutbox"]),
                        Convert.ToBoolean(row["IsArchived"]));

                    // get the return link to the pm listing
                    if (this.IsOutbox)
                    {
                        this.PageLinks.AddLink(
                            this.GetText("SENTITEMS"), BuildLink.GetLink(ForumPages.PM, "v=out"));
                    }
                    else if (this.IsArchived)
                    {
                        this.PageLinks.AddLink(
                            this.GetText("ARCHIVE"), BuildLink.GetLink(ForumPages.PM, "v=arch"));
                    }
                    else
                    {
                        this.PageLinks.AddLink(this.GetText("INBOX"), BuildLink.GetLink(ForumPages.PM));
                    }

                    this.PageLinks.AddLink(row["Subject"].ToString());

                    this.Inbox.DataSource = dataView;
                }
                else
                {
                    BuildLink.Redirect(ForumPages.PM);
                }
            }

            this.DataBind();

            if (this.IsOutbox)
            {
                return;
            }

            var userPmessageId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("pm");

            this.GetRepository<UserPMessage>().MarkAsRead(userPmessageId);
            this.Get<IDataCache>().Remove(string.Format(Constants.Cache.ActiveUserLazyData, this.PageContext.PageUserID));
            this.Get<IRaiseEvent>().Raise(
                new UpdateUserPrivateMessageEvent(this.PageContext.PageUserID, userPmessageId));
        }

        /// <summary>
        /// Sets the IsOutbox property as appropriate for this private message.
        /// </summary>
        /// <remarks>
        /// User id parameters are downcast to object to allow for potential future use of non-integer user id's
        /// </remarks>
        /// <param name="fromUserID">
        /// The from User ID.
        /// </param>
        /// <param name="toUserID">
        /// The to User ID.
        /// </param>
        /// <param name="messageIsInOutbox">
        /// Bool indicating whether the message is in the sender's outbox
        /// </param>
        /// <param name="messageIsArchived">
        /// The message Is Archived.
        /// </param>
        private void SetMessageView(
            [NotNull] object fromUserID, [NotNull] object toUserID, bool messageIsInOutbox, bool messageIsArchived)
        {
            var isCurrentUserFrom = fromUserID.Equals(this.PageContext.PageUserID);
            var isCurrentUserTo = toUserID.Equals(this.PageContext.PageUserID);

            // check if it's the same user...
            if (isCurrentUserFrom && isCurrentUserTo)
            {
                // it is... handle the view based on the query string passed
                this.IsOutbox = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("v") == "out";
                this.IsArchived = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("v") == "arch";

                // see if the message got deleted, if so, redirect to their outbox/archive
                if (this.IsOutbox && !messageIsInOutbox)
                {
                    BuildLink.Redirect(ForumPages.PM, "v=out");
                }
                else if (this.IsArchived && !messageIsArchived)
                {
                    BuildLink.Redirect(ForumPages.PM, "v=arch");
                }
            }
            else if (isCurrentUserFrom)
            {
                // see if it's been deleted by the from user...
                if (!messageIsInOutbox)
                {
                    // deleted for this user, redirect...
                    BuildLink.Redirect(ForumPages.PM, "v=out");
                }
                else
                {
                    // nope
                    this.IsOutbox = true;
                }
            }
            else if (isCurrentUserTo)
            {
                // get the status for the receiver
                this.IsArchived = messageIsArchived;
                this.IsOutbox = false;
            }
        }

        #endregion
    }
}