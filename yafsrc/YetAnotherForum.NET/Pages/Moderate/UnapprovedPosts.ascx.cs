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

namespace YAF.Pages.Moderate
{
    #region Using

    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
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
                BuildLink.GetLink(ForumPages.Moderate_Index));

            // current page
            this.PageLinks.AddLink(this.PageContext.PageForumName);
        }

        /// <summary>
        /// Format message.
        /// </summary>
        /// <param name="row">
        /// Message data row.
        /// </param>
        /// <returns>
        /// Formatted string with escaped HTML markup and formatted.
        /// </returns>
        protected string FormatMessage([NotNull] DataRowView row)
        {
            // get message flags
            var messageFlags = new MessageFlags(row["Flags"]);

            // message
            string msg;

            // format message?
            if (messageFlags.NotFormatted)
            {
                // just encode it for HTML output
                msg = this.HtmlEncode(row["Message"].ToString());
            }
            else
            {
                // fully format message (YafBBCode)
                msg = this.Get<IFormatMessage>().Format(
                    row["Message"].ToString(),
                    messageFlags,
                    row["IsModeratorChanged"].ToType<bool>());
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
            var messageList = this.GetRepository<Message>().UnapprovedAsDataTable(this.PageContext.PageForumID);

            if (!messageList.HasRows())
            {
                // redirect back to the moderate main if no messages found
                BuildLink.Redirect(ForumPages.Moderate_Index);
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
                    this.GetRepository<Message>().ApproveMessage(e.CommandArgument.ToType<int>());

                    // Update statistics
                    this.Get<IDataCache>().Remove(Constants.Cache.BoardStats);

                    // re-bind data
                    this.BindData();

                    // tell user message was approved
                    this.PageContext.AddLoadMessage(this.GetText("APPROVED"), MessageTypes.success);

                    // send notification to watching users...
                    this.Get<ISendNotification>().ToWatchingUsers(e.CommandArgument.ToType<int>());
                    break;
                case "delete":

                    // delete message
                    this.GetRepository<Message>().Delete(e.CommandArgument.ToType<int>(), true, string.Empty, 1, true);

                    // Update statistics
                    this.Get<IDataCache>().Remove(Constants.Cache.BoardStats);

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