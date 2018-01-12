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
    using System.Data;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The cp_message.
    /// </summary>
    public partial class cp_message : ForumPageRegistered
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "cp_message" /> class.
        /// </summary>
        public cp_message()
            : base("CP_MESSAGE")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether IsArchived.
        /// </summary>
        protected bool IsArchived
        {
            get
            {
                return this.ViewState["IsArchived"] != null && (bool)this.ViewState["IsArchived"];
            }

            set
            {
                this.ViewState["IsArchived"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether IsOutbox.
        /// </summary>
        protected bool IsOutbox
        {
            get
            {
                return this.ViewState["IsOutbox"] != null && (bool)this.ViewState["IsOutbox"];
            }

            set
            {
                this.ViewState["IsOutbox"] = value;
            }
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
                    if (this.IsOutbox)
                    {
                        LegacyDb.pmessage_delete(e.CommandArgument, true);
                    }
                    else
                    {
                        LegacyDb.pmessage_delete(e.CommandArgument);
                    }

                    this.BindData();
                    this.PageContext.AddLoadMessage(this.GetText("msg_deleted"), MessageTypes.success);
                    YafBuildLink.Redirect(ForumPages.cp_pm);
                    break;
                case "reply":
                    YafBuildLink.Redirect(ForumPages.pmessage, "p={0}&q=0", e.CommandArgument);
                    break;
                case "quote":
                    YafBuildLink.Redirect(ForumPages.pmessage, "p={0}&q=1", e.CommandArgument);
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
            if (!this.Get<YafBoardSettings>().AllowPrivateMessages)
            {
                YafBuildLink.RedirectInfoPage(InfoMessage.Disabled);
            }

            if (this.IsPostBack)
            {
                return;
            }

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("pm").IsNotSet())
            {
                YafBuildLink.AccessDenied();
            }

            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.Get<YafBoardSettings>().EnableDisplayName
                    ? this.PageContext.CurrentUserData.DisplayName
                    : this.PageContext.PageUserName,
                YafBuildLink.GetLink(ForumPages.cp_profile));

            // handle custom YafBBCode javascript or CSS...
            this.Get<IBBCode>().RegisterCustomBBCodePageElements(this.Page, this.GetType());

            this.BindData();
        }

        /// <summary>
        /// The theme button delete_ load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void ThemeButtonDelete_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            var themeButton = (ThemeButton)sender;
            themeButton.Attributes["onclick"] = "return confirm('{0}')".FormatWith(
                this.GetText("confirm_deletemessage"));
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            using (
                var dt =
                    LegacyDb.pmessage_list(
                        Security.StringToLongOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("pm"))))
            {
                if (dt.HasRows())
                {
                    var row = dt.Rows[0];

                    // if the pm isn't from or two the current user--then it's access denied
                    if ((int)row["ToUserID"] != this.PageContext.PageUserID
                        && (int)row["FromUserID"] != this.PageContext.PageUserID)
                    {
                        YafBuildLink.AccessDenied();
                    }

                    this.SetMessageView(
                        row["FromUserID"],
                        row["ToUserID"],
                        Convert.ToBoolean(row["IsInOutbox"]),
                        Convert.ToBoolean(row["IsArchived"]));

                    // get the return link to the pm listing
                    if (this.IsOutbox)
                    {
                        this.PageLinks.AddLink(
                            this.GetText("SENTITEMS"), YafBuildLink.GetLink(ForumPages.cp_pm, "v=out"));
                    }
                    else if (this.IsArchived)
                    {
                        this.PageLinks.AddLink(
                            this.GetText("ARCHIVE"), YafBuildLink.GetLink(ForumPages.cp_pm, "v=arch"));
                    }
                    else
                    {
                        this.PageLinks.AddLink(this.GetText("INBOX"), YafBuildLink.GetLink(ForumPages.cp_pm));
                    }

                    this.PageLinks.AddLink(row["Subject"].ToString());

                    this.Inbox.DataSource = dt;
                }
                else
                {
                    YafBuildLink.Redirect(ForumPages.cp_pm);
                }
            }

            this.DataBind();

            if (this.IsOutbox)
            {
                return;
            }

            var userPmessageId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("pm").ToType<int>();

            LegacyDb.pmessage_markread(userPmessageId);
            this.Get<IDataCache>().Remove(Constants.Cache.ActiveUserLazyData.FormatWith(this.PageContext.PageUserID));
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
                    YafBuildLink.Redirect(ForumPages.cp_pm, "v=out");
                }
                else if (this.IsArchived && !messageIsArchived)
                {
                    YafBuildLink.Redirect(ForumPages.cp_pm, "v=arch");
                }
            }
            else if (isCurrentUserFrom)
            {
                // see if it's been deleted by the from user...
                if (!messageIsInOutbox)
                {
                    // deleted for this user, redirect...
                    YafBuildLink.Redirect(ForumPages.cp_pm, "v=out");
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