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

namespace YAF.Pages.Profile
{
    #region Using

    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// User Page To Manage User Block Option and handle Ignored Users
    /// </summary>
    public partial class BlockOptions : ProfilePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockOptions"/> class.
        /// </summary>
        public BlockOptions()
            : base("BLOCK_OPTIONS", ForumPages.Profile_BlockOptions)
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
            this.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(), this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));
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
            this.BlockPMs.Visible = this.PageBoardContext.BoardSettings.AllowPrivateMessages;

            this.BlockFriendRequests.Text = this.GetText("BLOCK_BUDDYS");
            this.BlockFriendRequests.Visible = this.PageBoardContext.BoardSettings.EnableBuddyList;

            this.BlockEmails.Text = this.GetText("BLOCK_EMAILS");
            this.BlockEmails.Visible = this.PageBoardContext.BoardSettings.AllowEmailSending;
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

            this.GetRepository<User>().UpdateBlockFlags(this.PageBoardContext.PageUserID, blockFlags.BitValue);

            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageBoardContext.PageUserID));
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.BlockPMs.Checked = this.PageBoardContext.PageUser.Block.BlockPMs;
            this.BlockFriendRequests.Checked = this.PageBoardContext.PageUser.Block.BlockFriendRequests;
            this.BlockEmails.Checked = this.PageBoardContext.PageUser.Block.BlockEmails;

            var ignoreUsers = this.GetRepository<IgnoreUser>().IgnoredUsers(this.PageBoardContext.PageUserID);

            this.UserIgnoredList.DataSource = ignoreUsers;

            this.IgnoredUserHolder.Visible = ignoreUsers.Any();

            this.DataBind();
        }

        #endregion
    }
}