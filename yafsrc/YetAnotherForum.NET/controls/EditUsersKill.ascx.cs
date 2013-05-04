/* Yet Another Forum.NET
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

using YAF.Classes;

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.Security;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The edit users kill.
    /// </summary>
    public partial class EditUsersKill : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The _all posts by user.
        /// </summary>
        private DataTable _allPostsByUser;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets AllPostsByUser.
        /// </summary>
        public DataTable AllPostsByUser
        {
            get
            {
                return this._allPostsByUser ??
                       (this._allPostsByUser =
                        LegacyDb.post_alluser(
                            this.PageContext.PageBoardID, this.CurrentUserID, this.PageContext.PageUserID, null));
            }
        }

        /// <summary>
        ///   Gets IPAddresses.
        /// </summary>
        [NotNull]
        public List<string> IPAddresses
        {
            get
            {
                return this.AllPostsByUser.GetColumnAsList<string>("IP").OrderBy(x => x).Distinct().ToList();
            }
        }

        /// <summary>
        ///   Gets CurrentUserID.
        /// </summary>
        protected long? CurrentUserID
        {
            get
            {
                return this.PageContext.QueryIDs["u"];
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The kill_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Kill_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.BanIps.Checked)
            {
                this.BanUserIps();
            }

            this.DeletePosts();

            MembershipUser user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserID);
            this.PageContext.AddLoadMessage(this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_USER_KILLED").FormatWith(user.UserName));

            // update the displayed data...
            this.BindData();
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // init ids...
            this.PageContext.QueryIDs = new QueryStringIDHelper("u", true);

            // this needs to be done just once, not during postbacks
            if (this.IsPostBack)
            {
                return;
            }

            MembershipUser user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserID);
            var userData = new CombinedUserDataHelper(user, (int)this.CurrentUserID.Value);

            this.ViewPostsLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.search,
                "postedby={0}",
            !userData.IsGuest ? (this.Get<YafBoardSettings>().EnableDisplayName ? userData.DisplayName : userData.UserName)
            : (UserMembershipHelper.GuestUserName));

            // bind data
            this.BindData();
        }

        /// <summary>
        /// The ban user ips.
        /// </summary>
        private void BanUserIps()
        {
            var usr =
                LegacyDb.UserList(this.PageContext.PageBoardID, this.CurrentUserID.ToType<int?>(), null, null, null, false).FirstOrDefault();

            if (usr != null)
            {
                this.Get<ILogger>()
                    .Log(
                        this.PageContext.PageUserID,
                        "YAF.Controls.EditUsersKill",
                        "User {0} was killed by {1}".FormatWith(
                            this.Get<YafBoardSettings>().EnableDisplayName ? usr.DisplayName : usr.Name,
                            this.Get<YafBoardSettings>().EnableDisplayName
                                ? this.PageContext.CurrentUserData.DisplayName
                                : this.PageContext.CurrentUserData.UserName),
                        EventLogTypes.UserSuspended);
            }

            var allIps = this.GetRepository<BannedIP>().ListTyped().Select(x => x.Mask).ToList();

            // ban user ips...
            string name = UserMembershipHelper.GetDisplayNameFromID(this.CurrentUserID == null ? -1 : (int)this.CurrentUserID);

            if (string.IsNullOrEmpty(name))
            {
                name = UserMembershipHelper.GetUserNameFromID(this.CurrentUserID == null ? -1 : (int)this.CurrentUserID);
            }

            foreach (var ip in this.IPAddresses.Except(allIps).ToList())
            {
                string linkUserBan = this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "LINK_USER_BAN").FormatWith(this.CurrentUserID, YafBuildLink.GetLink(ForumPages.profile, "u={0}", this.CurrentUserID), this.HtmlEncode(name));

                this.GetRepository<BannedIP>().Save(null, ip, linkUserBan, this.PageContext.PageUserID);
            }

            if (this.SuspendUser.Checked && this.CurrentUserID > 0)
            {
                LegacyDb.user_suspend(this.CurrentUserID, DateTime.UtcNow.AddYears(5));
            }
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            // load ip address history for user...
            this.IpAddresses.Text = this.IPAddresses.ToDelimitedString("<br />");

            // show post count...
            this.PostCount.Text = this.AllPostsByUser.Rows.Count.ToString();

            this.DataBind();
        }

        /// <summary>
        /// The delete posts.
        /// </summary>
        private void DeletePosts()
        {
            // delete posts...
            var messageIds =
              (from m in this.AllPostsByUser.AsEnumerable() select m.Field<int>("MessageID")).Distinct().ToList();

            messageIds.ForEach(x => LegacyDb.message_delete(x, true, string.Empty, 1, true));
        }

        #endregion
    }
}