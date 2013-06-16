/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Admin edit user page.
    /// </summary>
    public partial class edituser : AdminPage
    {
        #region Properties

        /// <summary>
        ///   Gets user ID of edited user.
        /// </summary>
        protected int CurrentUserID
        {
            get
            {
                return this.PageContext.QueryIDs["u"].ToType<int>();
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Is Guest User.
        /// </summary>
        protected bool IsGuestUser
        {
            get
            {
                return UserMembershipHelper.IsGuestUser(this.CurrentUserID);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether [is user host admin]
        /// </summary>
        /// <param name="userRow">The user row.</param>
        /// <returns>
        /// The is user host admin.
        /// </returns>
        protected bool IsUserHostAdmin([NotNull] DataRow userRow)
        {
            var userFlags = new UserFlags(userRow["Flags"]);
            return userFlags.IsHostAdmin;
        }

        /// <summary>
        /// Registers the java scripts
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // setup jQuery and Jquery Ui Tabs.
            YafContext.Current.PageElements.RegisterJQuery();
            YafContext.Current.PageElements.RegisterJQueryUI();

            YafContext.Current.PageElements.RegisterJsBlock(
                "EditUserTabsJs",
                JavaScriptBlocks.JqueryUITabsLoadJs(this.EditUserTabs.ClientID, this.hidLastTab.ClientID, false));

            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // we're in the admin section...
            this.ProfileEditControl.InAdminPages = true;
            this.SignatureEditControl.InAdminPages = true;
            this.AvatarEditControl.InAdminPages = true;

            this.PageContext.QueryIDs = new QueryStringIDHelper("u", true);

            DataTable dt = LegacyDb.user_list(this.PageContext.PageBoardID, this.CurrentUserID, null);

            if (dt.Rows.Count != 1)
            {
                return;
            }

            DataRow userRow = dt.Rows[0];

            // do admin permission check...
            if (!this.PageContext.IsHostAdmin && this.IsUserHostAdmin(userRow))
            {
                // user is not host admin and is attempted to edit host admin account...
                YafBuildLink.AccessDenied();
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_USERS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_users));

            var userName = this.Get<YafBoardSettings>().EnableDisplayName
                               ? userRow["DisplayName"].ToString()
                               : userRow["Name"].ToString();

            // current page label (no link)
            this.PageLinks.AddLink(
                this.GetText("ADMIN_EDITUSER", "TITLE").FormatWith(userName),
                string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_USERS", "TITLE"),
                this.GetText("ADMIN_EDITUSER", "TITLE").FormatWith(userName));

            // do a quick user membership sync...
            MembershipUser user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserID);

            // update if the user is not Guest
            if (!this.IsGuestUser)
            {
                RoleMembershipHelper.UpdateForumUser(user, this.PageContext.PageBoardID);
            }

            this.EditUserTabs.DataBind();
        }

        #endregion
    }
}