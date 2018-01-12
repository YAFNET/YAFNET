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
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Yaf Team Page
    /// </summary>
    public partial class team : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   The Moderators List
        /// </summary>
        private List<Moderator> completeModsList = new List<Moderator>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "team" /> class.
        /// </summary>
        public team()
            : base("TEAM")
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Go to Selected Forum, if one is Selected
        /// </summary>
        /// <param name="sender">
        /// The Button sender.
        /// </param>
        /// <param name="e">
        /// The Button Eventargs.
        /// </param>
        public void GoToForum([NotNull] object sender, [NotNull] EventArgs e)
        {
            var goToForumButton = sender as ThemeButton;

            var gridItem = (DataGridItem)goToForumButton.NamingContainer;

            var modForums = (DropDownList)gridItem.FindControl("ModForums");

            int redirForum;

            if (int.TryParse(modForums.SelectedValue, out redirForum))
            {
                YafBuildLink.Redirect(ForumPages.topics, "f={0}", modForums.SelectedValue);
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
                    this.GetRepository<User>().AdminList(useStyledNicks: this.Get<YafBoardSettings>().UseStyledNicks),
                    TimeSpan.FromMinutes(this.Get<YafBoardSettings>().BoardModeratorsCacheTimeout));

            if (this.Get<YafBoardSettings>().UseStyledNicks)
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
            var moderators = this.Get<YafDbBroker>().GetAllModerators();

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
                var forumsCount = modList.Count();

                sortedMod.ForumIDs = new ModeratorsForums[forumsCount];

                for (var i = 0; i < forumsCount; i++)
                {
                    var forumsId = new ModeratorsForums
                        { ForumID = modList[i].ForumID, ForumName = modList[i].ForumName };

                    sortedMod.ForumIDs[i] = forumsId;
                }

                modsSorted.Add(sortedMod);
            }

            return modsSorted;
        }

        /// <summary>
        /// Protects from script in "location" field
        /// </summary>
        /// <param name="svalue">
        /// the Object to format
        /// </param>
        /// <returns>
        /// The get string safely.
        /// </returns>
        protected string GetStringSafely([NotNull] object svalue)
        {
            return svalue == null ? string.Empty : this.HtmlEncode(svalue.ToString());
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
                       ? "{0}images/noavatar.gif".FormatWith(YafForumInfo.ForumClientFileRoot)
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
            this.InitializeComponent();
            base.OnInit(e);
            if (!this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ShowTeamTo))
            {
                YafBuildLink.AccessDenied();
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

            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(this.GetText("TEAM", "TITLE"), string.Empty);

            this.BindData();
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
        /// Admins Grid Data Bound Set up all Controls
        /// </summary>
        /// <param name="sender">
        /// The Databound sender.
        /// </param>
        /// <param name="e">
        /// The Databound Eventargs.
        /// </param>
        private void AdminsGridItemDataBound([NotNull] object sender, [NotNull] DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var adminAvatar = (Image)e.Item.FindControl("AdminAvatar");

            var drowv = (DataRowView)e.Item.DataItem;
            var userid = drowv.Row["UserID"].ToType<int>();
            var displayName = this.Get<YafBoardSettings>().EnableDisplayName ? drowv.Row["DisplayName"].ToString() : drowv.Row["Name"].ToString();

            adminAvatar.ImageUrl = this.GetAvatarUrlFileName(
                drowv.Row["UserID"].ToType<int>(),
                drowv.Row["Avatar"].ToString(),
                drowv.Row["AvatarImage"].ToString().IsSet(),
                drowv.Row["Email"].ToString());

            adminAvatar.AlternateText = displayName;
            adminAvatar.ToolTip = displayName;

            // User Buttons 
            var adminUserButton = (ThemeButton)e.Item.FindControl("AdminUserButton");
            var pm = (ThemeButton)e.Item.FindControl("PM");
            var email = (ThemeButton)e.Item.FindControl("Email");
           
            adminUserButton.Visible = this.PageContext.IsAdmin;

            if (userid == this.PageContext.PageUserID)
            {
                return;
            }

            pm.Visible = !PageContext.IsGuest && this.User != null && this.Get<YafBoardSettings>().AllowPrivateMessages;
            pm.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", userid);
            pm.ParamTitle0 = displayName;

            // email link
            email.Visible = !PageContext.IsGuest && this.User != null && this.Get<YafBoardSettings>().AllowEmailSending;
            email.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_email, "u={0}", userid);
            email.ParamTitle0 = displayName;
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.AdminsGrid.Columns[0].HeaderText = this.GetText("TEAM", "User");
            this.AdminsGrid.Columns[2].HeaderText = this.GetText("TEAM", "Forums");
            this.AdminsGrid.DataSource = this.GetAdmins();

            this.completeModsList = this.GetModerators();

            if (this.completeModsList.Count > 0)
            {
                this.ModsTable.Visible = true;

                this.ModeratorsGrid.Columns[0].HeaderText = this.GetText("TEAM", "User");
                this.ModeratorsGrid.Columns[2].HeaderText = this.GetText("TEAM", "Forums");

                this.ModeratorsGrid.DataSource = this.completeModsList;
            }
            else
            {
                this.ModsTable.Visible = false;
            }

            this.DataBind();
        }

        /// <summary>
        /// The initialize component.
        /// </summary>
        private void InitializeComponent()
        {
            this.AdminsGrid.ItemDataBound += this.AdminsGridItemDataBound;
            this.ModeratorsGrid.ItemDataBound += this.ModeratorsGridItemDataBound;
        }

        /// <summary>
        /// Mods Grid Data Bound Set up all Controls
        /// </summary>
        /// <param name="sender">
        /// The Databound sender.
        /// </param>
        /// <param name="e">
        /// The Databound Eventargs.
        /// </param>
        private void ModeratorsGridItemDataBound([NotNull] object sender, [NotNull] DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var modForums = (DropDownList)e.Item.FindControl("ModForums");

            var modLink = (UserLink)e.Item.FindControl("ModLink");

            var mod = this.completeModsList.Find(m => m.ModeratorID.Equals(modLink.UserID));

            foreach (var forumsItem in from forumsItem in mod.ForumIDs
                                       let forumListItem =
                                           new ListItem
                                               {
                                                   Value = forumsItem.ForumID.ToString(), Text = forumsItem.ForumName 
                                               }
                                       where !modForums.Items.Contains(forumListItem)
                                       select forumsItem)
            {
                modForums.Items.Add(new ListItem { Value = forumsItem.ForumID.ToString(), Text = forumsItem.ForumName });
            }

            if (modForums.Items.Count > 0)
            {
                modForums.Items.Insert(
                    0, new ListItem(this.GetTextFormatted("VIEW_FORUMS", modForums.Items.Count), "intro"));
                modForums.Items.Insert(1, new ListItem("--------------------------", "break"));
            }
            else
            {
                modForums.Visible = false;
            }

            // User Buttons 
            var adminUserButton = (ThemeButton)e.Item.FindControl("AdminUserButton");
            var pm = (ThemeButton)e.Item.FindControl("PM");
            var email = (ThemeButton)e.Item.FindControl("Email");

            adminUserButton.Visible = this.PageContext.IsAdmin;

            /*try
            {*/
            var drowv = (Moderator)e.Item.DataItem;
            var userid = drowv.ModeratorID;
            var displayName = this.Get<YafBoardSettings>().EnableDisplayName ? drowv.DisplayName : drowv.Name;

            var modAvatar = (Image)e.Item.FindControl("ModAvatar");

            modAvatar.ImageUrl = this.GetAvatarUrlFileName(
                userid.ToType<int>(), drowv.Avatar, drowv.AvatarImage, drowv.Email);

            modAvatar.AlternateText = displayName;
            modAvatar.ToolTip = displayName;

            if (userid == this.PageContext.PageUserID)
            {
                return;
            }

            pm.Visible = !this.PageContext.IsGuest && this.User != null
                         && this.Get<YafBoardSettings>().AllowPrivateMessages;
            pm.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", userid);
            pm.ParamTitle0 = displayName;

            // email link
            email.Visible = !this.PageContext.IsGuest && this.User != null
                            && this.Get<YafBoardSettings>().AllowEmailSending;
            email.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_email, "u={0}", userid);
            email.ParamTitle0 = displayName;
            /*}
            catch (Exception)
            {
                return;
            }*/
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