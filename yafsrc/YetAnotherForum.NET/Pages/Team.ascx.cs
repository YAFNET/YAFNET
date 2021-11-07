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
    using System.Collections.Generic;
    using System.Linq;
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
    using YAF.Types.Objects;
    using YAF.Web.Controls;
    using YAF.Web.Extensions;

    using ListItem = System.Web.UI.WebControls.ListItem;

    #endregion

    /// <summary>
    /// The Team Page
    /// </summary>
    public partial class Team : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   The Moderators List
        /// </summary>
        private List<SimpleModerator> completeModsList = new ();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Team" /> class.
        /// </summary>
        public Team()
            : base("TEAM")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Go to Selected Forum, if one is Selected
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void GoToForum([NotNull] object sender, [NotNull] EventArgs e)
        {
            var forumButton = sender as ThemeButton;

            var gridItem = (RepeaterItem)forumButton.NamingContainer;

            var modForums = gridItem.FindControlAs<DropDownList>("ModForums");

            if (modForums.SelectedValue != "intro" && modForums.SelectedValue != "break")
            {
                this.Get<LinkBuilder>().Redirect(
                    ForumPages.Topics,
                    "f={0}&name={1}",
                    modForums.SelectedValue,
                    modForums.SelectedItem.Text);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get all Admins.
        /// </summary>
        /// <returns>
        /// Moderators List
        /// </returns>
        [NotNull]
        protected List<User> GetAdmins()
        {
            // get a row with user lazy data...
            var adminList = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.BoardAdmins,
                () => this.GetRepository<User>().ListAdmins(),
                TimeSpan.FromMinutes(this.PageContext.BoardSettings.BoardModeratorsCacheTimeout));

            return adminList;
        }

        /// <summary>
        /// Get all Moderators Without Groups.
        /// </summary>
        /// <returns>
        /// Moderators List
        /// </returns>
        [NotNull]
        protected List<SimpleModerator> GetModerators()
        {
            var moderators = this.Get<DataBroker>().GetModerators();

            var modsSorted = new List<SimpleModerator>();

            moderators.Where(m => !m.IsGroup).ForEach(
                mod =>
                {
                    var sortedMod = mod;

                    // Check if Mod is already in modsSorted
                    if (modsSorted.Find(
                        s => s.Name.Equals(sortedMod.Name) && s.ModeratorID.Equals(sortedMod.ModeratorID)) != null)
                    {
                        return;
                    }

                    // Get All Items from that MOD
                    var modList = moderators.Where(m => m.Name.Equals(sortedMod.Name)).ToList();
                    var forumsCount = modList.Count;

                    sortedMod.ForumIDs = new ModeratorsForums[forumsCount];

                    for (var i = 0; i < forumsCount; i++)
                    {
                        var forumsId = new ModeratorsForums
                        {
                            ForumID = modList[i].ForumID, ForumName = modList[i].ForumName
                        };

                        sortedMod.ForumIDs[i] = forumsId;
                    }

                    modsSorted.Add(sortedMod);
                });

            return modsSorted;
        }

        /// <summary>
        /// Called when the page loads
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e(EventArgs).
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Get<IPermissions>().Check(this.PageContext.BoardSettings.ShowTeamTo))
            {
                this.Get<LinkBuilder>().AccessDenied();
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.BindData();
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(this.GetText("TEAM", "TITLE"), string.Empty);
        }

        /// <summary>
        /// The admins list_ on item data bound.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void AdminsList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var adminAvatar = e.Item.FindControlAs<Image>("AdminAvatar");

            var user = (User)e.Item.DataItem;
            var displayName = user.DisplayOrUserName();

            adminAvatar.ImageUrl = this.Get<IAvatars>().GetAvatarUrlForUser(user);

            adminAvatar.AlternateText = displayName;
            adminAvatar.ToolTip = displayName;

            // User Buttons
            var adminUserButton = e.Item.FindControlAs<ThemeButton>("AdminUserButton");
            var pm = e.Item.FindControlAs<ThemeButton>("PM");
            var email = e.Item.FindControlAs<ThemeButton>("Email");

            adminUserButton.Visible = this.PageContext.IsAdmin;

            if (user.ID == this.PageContext.PageUserID)
            {
                return;
            }

            var isFriend = this.Get<IFriends>().IsBuddy(user.ID, true);

            pm.Visible = !this.PageContext.IsGuest && this.User != null && this.PageContext.BoardSettings.AllowPrivateMessages;

            if (pm.Visible)
            {
                if (user.Block.BlockPMs)
                {
                    pm.Visible = false;
                }

                if (this.PageContext.IsAdmin || isFriend)
                {
                    pm.Visible = true;
                }
            }

            pm.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.PostPrivateMessage, "u={0}", user.ID);
            pm.ParamTitle0 = displayName;

            // email link
            email.Visible = !this.PageContext.IsGuest && this.User != null && this.PageContext.BoardSettings.AllowEmailSending;

            if (!email.Visible)
            {
                return;
            }

            if (user.Block.BlockEmails)
            {
                email.Visible = false;
            }

            if (this.PageContext.IsAdmin && isFriend)
            {
                email.Visible = true;
            }

            email.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.Email, "u={0}", user.ID);
            email.ParamTitle0 = displayName;
        }

        /// <summary>
        /// The moderators list_ on item data bound.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ModeratorsList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var modForums = e.Item.FindControlAs<DropDownList>("ModForums");

            var modLink = e.Item.FindControlAs<UserLink>("ModLink");

            var mod = this.completeModsList.Find(m => m.ModeratorID.Equals(modLink.UserID));

            (from forumsItem in mod.ForumIDs
             let forumListItem = new ListItem { Value = forumsItem.ForumID.ToString(), Text = forumsItem.ForumName }
             select forumsItem).ForEach(
                forumsItem => modForums.Items.Add(
                    new ListItem { Value = forumsItem.ForumID.ToString(), Text = forumsItem.ForumName }));

            if (modForums.Items.Count > 0)
            {
                modForums.Items.Insert(0, new ListItem(this.GetTextFormatted("VIEW_FORUMS", modForums.Items.Count), "intro"));
                modForums.Items.Insert(1, new ListItem("--------------------------", "break"));
            }
            else
            {
                modForums.Visible = false;
            }

            // User Buttons
            var adminUserButton = e.Item.FindControlAs<ThemeButton>("AdminUserButton");
            var pm = e.Item.FindControlAs<ThemeButton>("PM");
            var email = e.Item.FindControlAs<ThemeButton>("Email");

            adminUserButton.Visible = this.PageContext.IsAdmin;

            var itemDataItem = (SimpleModerator)e.Item.DataItem;
            var userid = itemDataItem.ModeratorID.ToType<int>();
            var displayName = this.PageContext.BoardSettings.EnableDisplayName ? itemDataItem.DisplayName : itemDataItem.Name;

            var modAvatar = e.Item.FindControlAs<Image>("ModAvatar");

            modAvatar.ImageUrl = this.Get<IAvatars>().GetAvatarUrlForUser(
                userid,
                itemDataItem.Avatar,
                itemDataItem.AvatarImage);

            modAvatar.AlternateText = displayName;
            modAvatar.ToolTip = displayName;

            if (userid == this.PageContext.PageUserID)
            {
                return;
            }

            var isFriend = this.Get<IFriends>().IsBuddy(userid, true);

            pm.Visible = !this.PageContext.IsGuest && this.User != null && this.PageContext.BoardSettings.AllowPrivateMessages;

            if (pm.Visible)
            {
                if (mod.UserBlockFlags.BlockPMs)
                {
                    pm.Visible = false;
                }

                if (this.PageContext.IsAdmin || isFriend)
                {
                    pm.Visible = true;
                }
            }

            pm.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.PostPrivateMessage, "u={0}", userid);
            pm.ParamTitle0 = displayName;

            // email link
            email.Visible = !this.PageContext.IsGuest && this.User != null && this.PageContext.BoardSettings.AllowEmailSending;

            if (!email.Visible)
            {
                return;
            }

            if (mod.UserBlockFlags.BlockEmails && !this.PageContext.IsAdmin)
            {
                email.Visible = false;
            }

            if (this.PageContext.IsAdmin || isFriend)
            {
                email.Visible = true;
            }

            email.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.Email, "u={0}", userid);
            email.ParamTitle0 = displayName;
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.AdminsList.DataSource = this.GetAdmins();

            this.completeModsList = this.GetModerators();

            if (this.completeModsList.Count > 0)
            {
                this.ModsTable.Visible = true;

                this.ModeratorsList.DataSource = this.completeModsList;
            }
            else
            {
                this.ModsTable.Visible = false;
            }

            this.DataBind();
        }

        #endregion
    }
}