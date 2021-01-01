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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
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
        protected bool IsArchived { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether IsOutbox.
        /// </summary>
        protected bool IsOutbox { get; set; }

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
                    this.GetRepository<UserPMessage>().Delete(e.CommandArgument.ToType<int>(), this.IsOutbox);

                    this.BindData();
                    this.PageContext.AddLoadMessage(this.GetText("msg_deleted"), MessageTypes.success);
                    this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages);
                    break;
                case "reply":
                    this.Get<LinkBuilder>().Redirect(ForumPages.PostPrivateMessage, "p={0}&q=0", e.CommandArgument);
                    break;
                case "report":
                    this.Get<LinkBuilder>().Redirect(ForumPages.PostPrivateMessage, "p={0}&q=1&report=1", e.CommandArgument);
                    break;
                case "quote":
                    this.Get<LinkBuilder>().Redirect(ForumPages.PostPrivateMessage, "p={0}&q=1", e.CommandArgument);
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
            if (!this.PageContext.BoardSettings.AllowPrivateMessages)
            {
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Disabled);
            }

            if (!this.Get<HttpRequestBase>().QueryString.Exists("pm"))
            {
                this.Get<LinkBuilder>().AccessDenied();
            }

            if (this.IsPostBack)
            {
                return;
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
            this.PageLinks.AddLink(this.PageContext.User.DisplayOrUserName(), this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            var messageId =
                this.Get<LinkBuilder>().StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("pm"));

            var messages = this.GetRepository<PMessage>().List(messageId, true);

            if (messages != null && messages.Any())
            {
                messages.ForEach(
                    m => this.GetRepository<UserPMessage>().MarkAsRead((int)m.PMessageID, new PMessageFlags(m.UserPMFlags)));

                var message = messages.FirstOrDefault();

                this.SetMessageView(message.FromUserID, message.ToUserID, message.IsInOutbox, message.IsArchived);

                // get the return link to the pm listing
                if (this.IsOutbox)
                {
                    this.PageLinks.AddLink(
                        this.GetText("SENTITEMS"),
                        this.Get<LinkBuilder>().GetLink(ForumPages.MyMessages, "v=out"));
                }
                else if (this.IsArchived)
                {
                    this.PageLinks.AddLink(this.GetText("ARCHIVE"), this.Get<LinkBuilder>().GetLink(ForumPages.MyMessages, "v=arch"));
                }
                else
                {
                    this.PageLinks.AddLink(this.GetText("INBOX"), this.Get<LinkBuilder>().GetLink(ForumPages.MyMessages));
                }

                this.PageLinks.AddLink((string)message.Subject);

                this.Inbox.DataSource = messages;
            }
            else
            {
                this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages);
            }

            this.DataBind();

            if (this.IsOutbox)
            {
                return;
            }

            this.Get<IRaiseEvent>().Raise(new UpdateUserPrivateMessageEvent(this.PageContext.PageUserID, messageId));
        }

        /// <summary>
        /// Sets the IsOutbox property as appropriate for this private message.
        /// </summary>
        /// <param name="fromUserId">
        /// The from User Id.
        /// </param>
        /// <param name="toUserId">
        /// The to User Id.
        /// </param>
        /// <param name="messageIsInOutbox">
        /// Indicating whether the message is in the sender's outbox
        /// </param>
        /// <param name="messageIsArchived">
        /// The message Is Archived.
        /// </param>
        private void SetMessageView(
            [NotNull] int fromUserId,
            [NotNull] int toUserId,
            bool messageIsInOutbox,
            bool messageIsArchived)
        {
            var isCurrentUserFrom = fromUserId.Equals(this.PageContext.PageUserID);
            var isCurrentUserTo = toUserId.Equals(this.PageContext.PageUserID);

            // check if it's the same user...
            if (isCurrentUserFrom && isCurrentUserTo)
            {
                // it is... handle the view based on the query string passed
                this.IsOutbox = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("v") == "out";
                this.IsArchived = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("v") == "arch";

                // see if the message got deleted, if so, redirect to their outbox/archive
                if (this.IsOutbox && !messageIsInOutbox)
                {
                    this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages, "v=out");
                }
                else if (this.IsArchived && !messageIsArchived)
                {
                    this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages, "v=arch");
                }
            }
            else if (isCurrentUserFrom)
            {
                // see if it's been deleted by the from user...
                if (!messageIsInOutbox)
                {
                    // deleted for this user, redirect...
                    this.Get<LinkBuilder>().Redirect(ForumPages.MyMessages, "v=out");
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
            else
            {
                this.Get<LinkBuilder>().AccessDenied();
            }
        }

        #endregion
    }
}