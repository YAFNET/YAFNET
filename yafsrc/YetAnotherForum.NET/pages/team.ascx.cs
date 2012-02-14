﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
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
            DataTable adminListDataTable = this.Get<IDataCache>().GetOrSet(
              Constants.Cache.BoardAdmins,
              () => LegacyDb.admin_list(this.PageContext.PageBoardID, this.Get<YafBoardSettings>().UseStyledNicks),
              TimeSpan.FromMinutes(this.Get<YafBoardSettings>().BoardModeratorsCacheTimeout));

            if (this.Get<YafBoardSettings>().UseStyledNicks)
            {
                this.Get<IStyleTransform>().DecodeStyleByTable(ref adminListDataTable, false);
            }

            return adminListDataTable;
        }

        /// <summary>
        /// The get avatar url from id.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <returns>
        /// Returns the Avatar Url
        /// </returns>
        protected string GetAvatarUrlFromID(int userID)
        {
            string avatarUrl;

            try
            {
                avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(userID);

                if (avatarUrl.IsNotSet())
                {
                    avatarUrl = "{0}images/noavatar.gif".FormatWith(YafForumInfo.ForumClientFileRoot);
                }
            }
            catch (Exception)
            {
                avatarUrl = "{0}images/noavatar.gif".FormatWith(YafForumInfo.ForumClientFileRoot);
            }

            return avatarUrl;
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
            var moderators = this.Get<IDBBroker>().GetAllModeratorsTeam();

            var modsSorted = new List<Moderator>();

            foreach (SimpleModerator mod in moderators)
            {
                if (mod.IsGroup)
                {
                    continue;
                }

                var sortedMod = new Moderator { Name = mod.Name, ModeratorID = mod.ModeratorID, Style = mod.Style };

                // Check if Mod is already in modsSorted
                if (
                    modsSorted.Find(
                        s => s.Name.Equals(sortedMod.Name) && s.ModeratorID.Equals(sortedMod.ModeratorID)) != null)
                {
                    continue;
                }

                // Get All Items from that MOD
                var modList = moderators.Where(m => m.Name.Equals(sortedMod.Name)).ToList();
                var forumsCount = modList.Count();

                sortedMod.ForumIDs = new ModeratorsForums[forumsCount];

                for (int i = 0; i < forumsCount; i++)
                {
                    var forumsId = new ModeratorsForums { ForumID = modList[i].ForumID };

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

            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
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

            var adminLink = (UserLink)e.Item.FindControl("AdminLink");
            var adminAvatar = (Image)e.Item.FindControl("AdminAvatar");

            adminAvatar.ImageUrl = this.GetAvatarUrlFromID(adminLink.UserID);
            adminAvatar.AlternateText = adminLink.PostfixText;
            adminAvatar.ToolTip = adminLink.PostfixText;

            // User Buttons 
            var adminUserButton = (ThemeButton)e.Item.FindControl("AdminUserButton");
            var pm = (ThemeButton)e.Item.FindControl("PM");
            var email = (ThemeButton)e.Item.FindControl("Email");

            adminUserButton.Visible = this.PageContext.IsAdmin;

            var userData = new CombinedUserDataHelper(adminLink.UserID);

            if (userData.UserID == this.PageContext.PageUserID)
            {
                return;
            }

            pm.Visible = !userData.IsGuest && this.User != null && this.Get<YafBoardSettings>().AllowPrivateMessages;
            pm.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", userData.UserID);
            pm.ParamTitle0 = userData.UserName;

            // email link
            email.Visible = !userData.IsGuest && this.User != null && this.Get<YafBoardSettings>().AllowEmailSending;
            email.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_email, "u={0}", userData.UserID);
            email.ParamTitle0 = userData.UserName;

            if (this.PageContext.IsAdmin)
            {
                email.TitleNonLocalized = userData.Membership.Email;
            }
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.AdminsGrid.Columns[0].HeaderText = this.GetText("TEAM", "User");
            this.AdminsGrid.Columns[2].HeaderText = this.GetText("TEAM", "Forums");
            this.AdminsGrid.DataSource = this.GetAdmins();

            this.completeModsList = this.Get<IDataCache>().GetOrSet(
               Constants.Cache.BoardModerators,
                this.GetModerators,
                TimeSpan.FromMinutes(this.Get<YafBoardSettings>().BoardModeratorsCacheTimeout));

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

            var allForums = LegacyDb.ForumListAll(this.PageContext.PageBoardID, this.PageContext.PageUserID).ToList();

            var modForums = (DropDownList)e.Item.FindControl("ModForums");

            var modLink = (UserLink)e.Item.FindControl("ModLink");

            var modAvatar = (Image)e.Item.FindControl("ModAvatar");

            Moderator mod = this.completeModsList.Find(m => m.ModeratorID.Equals(modLink.UserID));

            modAvatar.ImageUrl = this.GetAvatarUrlFromID(modLink.UserID);
            modAvatar.AlternateText = mod.Name;
            modAvatar.ToolTip = mod.Name;

            foreach (var forumsItem in (from id in mod.ForumIDs
                                        where allForums.Find(f => f.ForumID.Equals((int)id.ForumID)) != null
                                        select allForums.Find(f => f.ForumID.Equals((int)id.ForumID))
                                        into yafForum
                                        select new ListItem { Value = yafForum.ForumID.ToString(), Text = yafForum.Forum }).Where(forumsItem => !modForums.Items.Contains((ListItem)forumsItem)))
            {
                modForums.Items.Add(forumsItem);
            }

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
            var adminUserButton = (ThemeButton)e.Item.FindControl("AdminUserButton");
            var pm = (ThemeButton)e.Item.FindControl("PM");
            var email = (ThemeButton)e.Item.FindControl("Email");

            adminUserButton.Visible = this.PageContext.IsAdmin;

            try
            {
                CombinedUserDataHelper userData = new CombinedUserDataHelper(modLink.UserID);

                if (userData.UserID == this.PageContext.PageUserID)
                {
                    return;
                }

                pm.Visible = !userData.IsGuest && this.User != null && this.Get<YafBoardSettings>().AllowPrivateMessages;
                pm.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", userData.UserID);
                pm.ParamTitle0 = userData.UserName;

                // email link
                email.Visible = !userData.IsGuest && this.User != null && this.Get<YafBoardSettings>().AllowEmailSending;
                email.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_email, "u={0}", userData.UserID);
                email.ParamTitle0 = userData.UserName;

                if (this.PageContext.IsAdmin)
                {
                    email.TitleNonLocalized = userData.Membership.Email;
                }
            }
            catch (Exception)
            {
                return;
            }
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
            ///   Gets or sets The Moderator Name
            /// </summary>
            public string Name { get; set; }

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

            #endregion
        }
    }
}