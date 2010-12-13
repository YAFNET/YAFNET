/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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
    #region

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Data;
    using YAF.Classes.Pattern;
    using YAF.Classes.Utils;
    using YAF.Controls;

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

            if (modForums.SelectedValue != "intro" || modForums.SelectedValue != "break")
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
        protected DataTable GetAdmins()
        {
            DataTable adminListDataTable = DB.user_list(PageContext.PageBoardID, null, true, null, 1, YafContext.Current.BoardSettings.UseStyledNicks);
            if (YafContext.Current.BoardSettings.UseStyledNicks)
            {
                new StyleTransform(YafContext.Current.Theme).DecodeStyleByTable(ref adminListDataTable, false);
            }

            // select only the guest user (if one exists)
            DataRow[] guestRows = adminListDataTable.Select("IsGuest > 0");

            if (guestRows.Length > 0)
            {
                foreach (DataRow row in guestRows)
                {
                    row.Delete();
                }

                // commits the deletes to the table
                adminListDataTable.AcceptChanges();
            }

            return adminListDataTable;
        }

        /// <summary>
        /// Get all Moderators Without Groups.
        /// </summary>
        /// <returns>
        /// Moderators List
        /// </returns>
        protected List<Moderator> GetModerators()
        {
            var moderators = YafContext.Current.Get<IDBBroker>().GetAllModerators().Where(mod => !mod.IsGroup);

            var modsSorted = new List<Moderator>();

            foreach (SimpleModerator mod in moderators)
            {
                Moderator sortedMod = new Moderator { Name = mod.Name, ModeratorID = mod.ModeratorID };

                // Check if Mod is already in modsSorted
                if (
                    modsSorted.Find(s => s.Name.Equals(sortedMod.Name) && s.ModeratorID.Equals(sortedMod.ModeratorID)) !=
                    null)
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
        protected string GetStringSafely(object svalue)
        {
            return svalue == null ? string.Empty : this.HtmlEncode(svalue.ToString());
        }

        /// <summary>
        /// The get avatar url from id.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <returns>
        /// The get avatar url from id.
        /// </returns>
        protected string GetAvatarUrlFromID(int userID)
        {
            string avatarUrl = "{0}images/noavatar.gif".FormatWith(YafForumInfo.ForumClientFileRoot);

            try
            {
                avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(userID);
            }
            finally
            {
                if (avatarUrl.IsNotSet())
                {
                    avatarUrl = "{0}images/noavatar.gif".FormatWith(YafForumInfo.ForumClientFileRoot);
                }
            }

            return avatarUrl;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
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
        protected void Pager_PageChange(object sender, EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.AdminsGrid.Columns[0].HeaderText = GetText("TEAM", "User");
            this.AdminsGrid.Columns[2].HeaderText = GetText("TEAM", "Forums");
            this.AdminsGrid.DataSource = this.GetAdmins();

            this.completeModsList = this.GetModerators();

            if (this.completeModsList.Count > 0)
            {
                this.ModsTable.Visible = true;

                this.ModeratorsGrid.Columns[0].HeaderText = GetText("TEAM", "User");
                this.ModeratorsGrid.Columns[2].HeaderText = GetText("TEAM", "Forums");

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

            pm.Visible = !userData.IsGuest && this.User != null && this.PageContext.BoardSettings.AllowPrivateMessages;
            pm.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", userData.UserID);

            // email link
            email.Visible = !userData.IsGuest && this.User != null && this.PageContext.BoardSettings.AllowEmailSending;
            email.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_email, "u={0}", userData.UserID);

            if (this.PageContext.IsAdmin)
            {
                email.TitleNonLocalized = userData.Membership.Email;
            }
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

            var allForums = DB.ForumListAll(this.PageContext.PageBoardID, this.PageContext.PageUserID).ToList();

            var modForums = (DropDownList)e.Item.FindControl("ModForums");

            var modLink = (UserLink)e.Item.FindControl("ModLink");

            var modAvatar = (Image)e.Item.FindControl("ModAvatar");

            Moderator mod = this.completeModsList.Find(m => m.ModeratorID.Equals(modLink.UserID));

            modAvatar.ImageUrl = this.GetAvatarUrlFromID(modLink.UserID);
            modAvatar.AlternateText = mod.Name;
            modAvatar.ToolTip = mod.Name;

            foreach (var forumsItem in from id in mod.ForumIDs
                                       where allForums.Find(f => f.ForumID.Equals((int)id.ForumID)) != null
                                       select allForums.Find(f => f.ForumID.Equals((int)id.ForumID))
                                       into yafForum
                                       select
                                           new ListItem { Value = yafForum.ForumID.ToString(), Text = yafForum.Forum })
            {
                modForums.Items.Add(forumsItem);
            }

            if (modForums.Items.Count > 0)
            {
                modForums.Items.Insert(
                    0, new ListItem(GetTextFormatted("VIEW_FORUMS", modForums.Items.Count), "intro"));
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

            var userData = new CombinedUserDataHelper(modLink.UserID);

            if (userData.UserID == this.PageContext.PageUserID)
            {
                return;
            }

            pm.Visible = !userData.IsGuest && this.User != null && this.PageContext.BoardSettings.AllowPrivateMessages;
            pm.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", userData.UserID);

            // email link
            email.Visible = !userData.IsGuest && this.User != null && this.PageContext.BoardSettings.AllowEmailSending;
            email.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_email, "u={0}", userData.UserID);

            if (this.PageContext.IsAdmin)
            {
                email.TitleNonLocalized = userData.Membership.Email;
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