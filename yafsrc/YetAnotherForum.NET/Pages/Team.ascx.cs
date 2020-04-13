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
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;
    using YAF.Web.Extensions;

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
        private List<Moderator> completeModsList = new List<Moderator>();

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

            if (int.TryParse(modForums.SelectedValue, out var redirForum))
            {
                BuildLink.Redirect(ForumPages.topics, "f={0}", modForums.SelectedValue);
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
        protected DataTable GetAdmins()
        {
            // get a row with user lazy data...
            var adminListDataTable = this.Get<IDataCache>()
                .GetOrSet(
                    Constants.Cache.BoardAdmins,
                    () =>
                    this.GetRepository<User>().AdminList(this.Get<BoardSettings>().UseStyledNicks),
                    TimeSpan.FromMinutes(this.Get<BoardSettings>().BoardModeratorsCacheTimeout));

            if (this.Get<BoardSettings>().UseStyledNicks)
            {
                this.Get<IStyleTransform>().DecodeStyleByTable(adminListDataTable, false);
            }

            return adminListDataTable;
        }

        /// <summary>
        /// Get all Moderators Without Groups.
        /// </summary>
        /// <returns>
        /// Moderators List
        /// </returns>
        [NotNull]
        protected List<Moderator> GetModerators()
        {
            var moderators = this.Get<DataBroker>().GetAllModerators();

            var modsSorted = new List<Moderator>();

            foreach (var mod in moderators)
            {
                if (mod.IsGroup)
                {
                    continue;
                }

                var sortedMod = new Moderator
                    {
                        Name = mod.Name,
                        ModeratorID = mod.ModeratorID,
                        Email = mod.Email,
                        Block = new UserBlockFlags(mod.BlockFlags),
                        Avatar = mod.Avatar,
                        AvatarImage = mod.AvatarImage,
                        DisplayName = mod.DisplayName,
                        Style = mod.Style
                    };

                // Check if Mod is already in modsSorted
                if (modsSorted.Find(s => s.Name.Equals(sortedMod.Name) && s.ModeratorID.Equals(sortedMod.ModeratorID))
                    != null)
                {
                    continue;
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
            }

            return modsSorted;
        }

        /// <summary>
        /// Gets the avatar Url for the user
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="avatarString">The avatar string.</param>
        /// <param name="hasAvatarImage">if set to <c>true</c> [has avatar image].</param>
        /// <param name="email">The email.</param>
        /// <returns>Returns the File Url</returns>
        protected string GetAvatarUrlFileName(int userId, string avatarString, bool hasAvatarImage, string email)
        {
            var avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(
                userId, avatarString, hasAvatarImage, email);

            return avatarUrl.IsNotSet()
                       ? $"{BoardInfo.ForumClientFileRoot}images/noavatar.svg"
                       : avatarUrl;
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            base.OnInit(e);

            if (!this.Get<IPermissions>().Check(this.Get<BoardSettings>().ShowTeamTo))
            {
                BuildLink.AccessDenied();
            }
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
        /// The pager_ page change.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.EventArgs
        /// </param>
        protected void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.BindData();
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

            var itemDataItem = (DataRowView)e.Item.DataItem;
            var userid = itemDataItem["UserID"].ToType<int>();
            var displayName = this.Get<BoardSettings>().EnableDisplayName ? itemDataItem.Row["DisplayName"].ToString() : itemDataItem.Row["Name"].ToString();

            adminAvatar.ImageUrl = this.GetAvatarUrlFileName(
                itemDataItem.Row["UserID"].ToType<int>(),
                itemDataItem.Row["Avatar"].ToString(),
                itemDataItem.Row["AvatarImage"].ToString().IsSet(),
                itemDataItem.Row["Email"].ToString());

            adminAvatar.AlternateText = displayName;
            adminAvatar.ToolTip = displayName; 

            // User Buttons 
            var adminUserButton = e.Item.FindControlAs<ThemeButton>("AdminUserButton");
            var pm = e.Item.FindControlAs<ThemeButton>("PM");
            var email = e.Item.FindControlAs<ThemeButton>("Email");

            adminUserButton.Visible = this.PageContext.IsAdmin;

            if (userid == this.PageContext.PageUserID)
            {
                return;
            }

            var blockFlags = new UserBlockFlags(itemDataItem.Row["BlockFlags"].ToType<int>());
            var isFriend = this.GetRepository<Buddy>().CheckIsFriend(this.PageContext.PageUserID, userid);

            pm.Visible = !this.PageContext.IsGuest && this.User != null && this.Get<BoardSettings>().AllowPrivateMessages;

            if (pm.Visible)
            {
                if (blockFlags.BlockPMs)
                {
                    pm.Visible = false;
                }

                if (this.PageContext.IsAdmin || isFriend)
                {
                    pm.Visible = true;
                }
            }

            pm.NavigateUrl = BuildLink.GetLinkNotEscaped(ForumPages.PostPrivateMessage, "u={0}", userid);
            pm.ParamTitle0 = displayName;

            // email link
            email.Visible = !this.PageContext.IsGuest && this.User != null && this.Get<BoardSettings>().AllowEmailSending;

            if (email.Visible)
            {
                if (blockFlags.BlockEmails)
                {
                    email.Visible = false;
                }

                if (this.PageContext.IsAdmin && isFriend)
                {
                    email.Visible = true;
                }

                email.NavigateUrl = BuildLink.GetLinkNotEscaped(ForumPages.Email, "u={0}", userid);
                email.ParamTitle0 = displayName;
            }
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
             where !modForums.Items.Contains(forumListItem)
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

            var itemDataItem = (Moderator)e.Item.DataItem;
            var userid = itemDataItem.ModeratorID.ToType<int>();
            var displayName = this.Get<BoardSettings>().EnableDisplayName ? itemDataItem.DisplayName : itemDataItem.Name;

            var modAvatar = e.Item.FindControlAs<Image>("ModAvatar");

            modAvatar.ImageUrl = this.GetAvatarUrlFileName(userid, itemDataItem.Avatar, itemDataItem.AvatarImage, itemDataItem.Email);

            modAvatar.AlternateText = displayName;
            modAvatar.ToolTip = displayName;

            if (userid == this.PageContext.PageUserID)
            {
                return;
            }

            var isFriend = this.GetRepository<Buddy>().CheckIsFriend(this.PageContext.PageUserID, userid);

            pm.Visible = !this.PageContext.IsGuest && this.User != null && this.Get<BoardSettings>().AllowPrivateMessages;

            if (pm.Visible)
            {
                if (mod.Block.BlockPMs)
                {
                    pm.Visible = false;
                }

                if (this.PageContext.IsAdmin || isFriend)
                {
                    pm.Visible = true;
                }
            }

            pm.NavigateUrl = BuildLink.GetLinkNotEscaped(ForumPages.PostPrivateMessage, "u={0}", userid);
            pm.ParamTitle0 = displayName;

            // email link
            email.Visible = !this.PageContext.IsGuest && this.User != null && this.Get<BoardSettings>().AllowEmailSending;

            if (!email.Visible)
            {
                return;
            }

            if (mod.Block.BlockEmails && !this.PageContext.IsAdmin)
            {
                email.Visible = false;
            }

            if (this.PageContext.IsAdmin || isFriend)
            {
                email.Visible = true;
            }

            email.NavigateUrl = BuildLink.GetLinkNotEscaped(ForumPages.Email, "u={0}", userid);
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

                this.ModeratorsList.DataSource = this.GetModerators();
            }
            else
            {
                this.ModsTable.Visible = false;
            }

            this.DataBind();
        }

        #endregion

        /// <summary>
        /// Moderators List
        /// </summary>
        public class Moderator
        {
            #region Properties

            /// <summary>
            ///   Gets or sets The Moderators Forums
            /// </summary>
            public ModeratorsForums[] ForumIDs { get; set; }

            /// <summary>
            ///   Gets or sets The Moderator ID (User ID)
            /// </summary>
            public long ModeratorID { get; set; }

            /// <summary>
            ///   Gets or sets The Moderator Email
            /// </summary>
            public string Email { get; set; }

            public UserBlockFlags Block { get; set; }

            /// <summary>
            ///   Gets or sets The Moderator Avatar
            /// </summary>
            public string Avatar { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether [avatar image].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [avatar image]; otherwise, <c>false</c>.
            /// </value>
            public bool AvatarImage { get; set; }

            /// <summary>
            ///   Gets or sets The Moderator Name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            ///   Gets or sets The Moderator Display Name
            /// </summary>
            public string DisplayName { get; set; }

            /// <summary>
            ///   Gets or sets The Moderator Style
            /// </summary>
            public string Style { get; set; }

            #endregion
        }

        /// <summary>
        /// Moderator Forums
        /// </summary>
        public class ModeratorsForums
        {
            #region Properties

            /// <summary>
            ///   Gets or sets The Forum ID.
            /// </summary>
            public long ForumID { get; set; }

            /// <summary>
            /// Gets or sets the name of the forum.
            /// </summary>
            /// <value>
            /// The name of the forum.
            /// </value>
            public string ForumName { get; set; }

            #endregion
        }
    }
}