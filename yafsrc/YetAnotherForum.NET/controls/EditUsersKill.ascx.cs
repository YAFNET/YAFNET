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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services.CheckForSpam;
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
                return this._allPostsByUser
                       ?? (this._allPostsByUser =
                           LegacyDb.post_alluser(
                               this.PageContext.PageBoardID,
                               this.CurrentUserID,
                               this.PageContext.PageUserID,
                               null));
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
                var ipList = this.AllPostsByUser.GetColumnAsList<string>("IP").OrderBy(x => x).Distinct().ToList();

                if (ipList.Count.Equals(0))
                {
                    ipList.Add(this.CurrentUserDataHelper.LastIP);
                }

                return ipList;
            }
        }

        /// <summary>
        /// Gets the current user data helper.
        /// </summary>
        /// <value>
        /// The current user data helper.
        /// </value>
        public CombinedUserDataHelper CurrentUserDataHelper
        {
            get
            {
                var user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserID);
                var currentUserId = this.CurrentUserID;

                return currentUserId != null
                           ? new CombinedUserDataHelper(user, currentUserId.Value.ToType<int>())
                           : null;
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
        /// Kills the User
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Kill_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.BanIps.Checked)
            {
                this.BanUserIps();
            }

            this.DeletePosts();

            MembershipUser user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserID);
            this.PageContext.AddLoadMessage(
                this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_USER_KILLED").FormatWith(user.UserName));

            // update the displayed data...
            this.BindData();
        }

        /// <summary>
        /// Reports the User
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Report_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.Get<YafBoardSettings>().StopForumSpamApiKey.IsNotSet())
            {
                return;
            }

            var user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserID);

            try
            {
                var stopForumSpam = new StopForumSpam();

                if (!stopForumSpam.ReportUserAsBot(this.IPAddresses.FirstOrDefault(), user.Email, user.UserName))
                {
                    return;
                }

                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITUSER", "BOT_REPORTED"), MessageTypes.Success);

                this.Logger.Log(
                    this.PageContext.PageUserID,
                    "User Reported to StopForumSpam.com",
                    "User (Name:{0}/ID:{1}/IP:{2}/Email:{3}) Reported to StopForumSpam.com by {4}".FormatWith(
                        user.UserName,
                        this.CurrentUserID,
                        this.IPAddresses.FirstOrDefault(),
                        user.Email,
                        this.Get<YafBoardSettings>().EnableDisplayName
                            ? this.PageContext.CurrentUserData.DisplayName
                            : this.PageContext.CurrentUserData.UserName),
                    EventLogTypes.SpamBotReported);
            }
            catch (Exception exception)
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITUSER", "BOT_REPORTED_FAILED"),
                    MessageTypes.Error);

                this.Logger.Log(
                    this.PageContext.PageUserID,
                    "User (Name{0}/ID:{1}) Report to StopForumSpam.com Failed".FormatWith(
                        user.UserName,
                        this.CurrentUserID),
                    exception);
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // init ids...
            this.PageContext.QueryIDs = new QueryStringIDHelper("u", true);

            // this needs to be done just once, not during postbacks
            if (this.IsPostBack)
            {
                return;
            }

            // bind data
            this.BindData();
        }

        /// <summary>
        /// Bans the user IP Addresses.
        /// </summary>
        private void BanUserIps()
        {
            var usr =
                LegacyDb.UserList(
                    this.PageContext.PageBoardID,
                    this.CurrentUserID.ToType<int?>(),
                    null,
                    null,
                    null,
                    false).FirstOrDefault();

            if (usr != null)
            {
                this.Logger
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
            string name =
                UserMembershipHelper.GetDisplayNameFromID(
                    this.CurrentUserID == null ? -1 : this.CurrentUserID.ToType<int>());

            if (name.IsNotSet())
            {
                name =
                    UserMembershipHelper.GetUserNameFromID(
                        this.CurrentUserID == null ? -1 : this.CurrentUserID.ToType<int>());
            }

            foreach (var ip in this.IPAddresses.Except(allIps).ToList())
            {
                string linkUserBan =
                    this.Get<ILocalization>()
                        .GetText("ADMIN_EDITUSER", "LINK_USER_BAN")
                        .FormatWith(
                            this.CurrentUserID,
                            YafBuildLink.GetLink(ForumPages.profile, "u={0}", this.CurrentUserID),
                            this.HtmlEncode(name));

                this.GetRepository<BannedIP>().Save(null, ip, linkUserBan, this.PageContext.PageUserID);
            }

            if (this.SuspendUser.Checked && this.CurrentUserID > 0)
            {
                LegacyDb.user_suspend(this.CurrentUserID, DateTime.UtcNow.AddYears(5));
            }
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.ViewPostsLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.search,
                "postedby={0}",
                !this.CurrentUserDataHelper.IsGuest
                    ? (this.Get<YafBoardSettings>().EnableDisplayName ? this.CurrentUserDataHelper.DisplayName : this.CurrentUserDataHelper.UserName)
                    : UserMembershipHelper.GuestUserName);

            this.Kill.Text = this.GetText("ADMIN_EDITUSER", "KILL_USER");
            ControlHelper.AddOnClickConfirmDialog(this.Kill, this.GetText("ADMIN_EDITUSER", "KILL_USER_CONFIRM"));

            if (this.Get<YafBoardSettings>().StopForumSpamApiKey.IsSet())
            {
                this.ReportUser.Visible = true;
                this.ReportUser.Text = this.GetText("ADMIN_EDITUSER", "REPORT_USER");
                ControlHelper.AddOnClickConfirmDialog(
                    this.ReportUser,
                    this.GetText("ADMIN_EDITUSER", "REPORT_USER_CONFIRM"));
            }
            else
            {
                this.ReportUser.Visible = false;
            }

            // load ip address history for user...
            this.IpAddresses.Text = this.IPAddresses.ToDelimitedString("<br />");

            // show post count...
            this.PostCount.Text = this.AllPostsByUser.Rows.Count.ToString();

            this.DataBind();
        }

        /// <summary>
        /// Deletes the posts.
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