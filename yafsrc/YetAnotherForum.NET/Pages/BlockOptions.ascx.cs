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
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// User Page To Manage User Block Option and handle Ignored Users
    /// </summary>
    public partial class BlockOptions : ForumPageRegistered
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockOptions"/> class.
        /// </summary>
        public BlockOptions()
            : base("BLOCK_OPTIONS")
        {
        }

        #region Methods

        /// <summary>
        /// The Item command.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void IgnoredItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "delete":
                    {
                        this.Get<IUserIgnored>().RemoveIgnored(e.CommandArgument.ToType<int>());

                        this.BindData();
                    }

                    break;
            }
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.Get<BoardSettings>().EnableDisplayName
                    ? this.PageContext.CurrentUserData.DisplayName
                    : this.PageContext.PageUserName,
                BuildLink.GetLink(ForumPages.Account));
            this.PageLinks.AddLink(this.GetText("BLOCK_OPTIONS", "TITLE"), string.Empty);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.BindData();

            this.BlockPMs.Text = this.GetText("BLOCK_PMS");
            this.BlockPMs.Visible = this.Get<BoardSettings>().AllowPrivateMessages;

            this.BlockFriendRequests.Text = this.GetText("BLOCK_BUDDYS");
            this.BlockFriendRequests.Visible = this.Get<BoardSettings>().EnableBuddyList;

            this.BlockEmails.Text = this.GetText("BLOCK_EMAILS");
            this.BlockEmails.Visible = this.Get<BoardSettings>().AllowEmailSending;
        }

        /// <summary>
        /// Saves the user Block Options
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void SaveUser_OnClick(object sender, EventArgs e)
        {
            var blockFlags = new UserBlockFlags
                                 {
                                     BlockEmails = this.BlockEmails.Checked,
                                     BlockFriendRequests = this.BlockFriendRequests.Checked,
                                     BlockPMs = this.BlockPMs.Checked
                                 };

            this.GetRepository<User>().UpdateBlockFlags(this.PageContext.PageUserID, blockFlags.BitValue);

            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageContext.PageUserID));
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.BlockPMs.Checked = this.PageContext.CurrentUserData.Block.BlockPMs;
            this.BlockFriendRequests.Checked = this.PageContext.CurrentUserData.Block.BlockFriendRequests;
            this.BlockEmails.Checked = this.PageContext.CurrentUserData.Block.BlockEmails;

            var ignoreUsers = this.GetRepository<IgnoreUser>().Get(u => u.UserID == this.PageContext.PageUserID);

            this.UserIgnoredList.DataSource = ignoreUsers;

            this.IgnoredUserHolder.Visible = ignoreUsers.Any();

            this.DataBind();
        }

        #endregion
    }
}