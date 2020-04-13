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
    using System.Data;
    using System.Text;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.UsersRoles;
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

    using ButtonStyle = YAF.Types.Constants.ButtonStyle;

    #endregion

    /// <summary>
    /// The User Profile Page.
    /// </summary>
    public partial class Profile : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Profile" /> class.
        /// </summary>
        public Profile()
            : base("PROFILE")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets UserId.
        /// </summary>
        public int UserId
        {
            get
            {
                if (this.ViewState["UserId"] == null)
                {
                    return 0;
                }

                return (int)this.ViewState["UserId"];
            }

            set => this.ViewState["UserId"] = value;
        }

        #endregion

        #region Methods

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
            // admin or moderator, set edit control to moderator mode...
            if (this.PageContext.IsAdmin || this.PageContext.IsForumModerator)
            {
                this.SignatureEditControl.InModeratorMode = true;
            }

            if (!this.IsPostBack)
            {
                this.UserId =
                    Security.StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));
                this.userGroupsRow.Visible = this.Get<BoardSettings>().ShowGroupsProfile || this.PageContext.IsAdmin;
            }

            if (this.UserId == 0)
            {
                BuildLink.AccessDenied(/*No such user exists*/);
            }

            this.BindData();
        }

        /// <summary>
        /// The setup theme button with link.
        /// </summary>
        /// <param name="thisButton">
        /// The this button.
        /// </param>
        /// <param name="linkUrl">
        /// The link url.
        /// </param>
        protected void SetupThemeButtonWithLink([NotNull] ThemeButton thisButton, [NotNull] string linkUrl)
        {
            if (linkUrl.IsSet())
            {
                var link = linkUrl.Replace("\"", string.Empty);
                if (!link.ToLower().StartsWith("http"))
                {
                    link = $"http://{link}";
                }

                thisButton.NavigateUrl = link;
                thisButton.Attributes.Add("target", "_blank");
                if (this.Get<BoardSettings>().UseNoFollowLinks)
                {
                    thisButton.Attributes.Add("rel", "nofollow");
                }
            }
            else
            {
                thisButton.NavigateUrl = string.Empty;
            }
        }

        /// <summary>
        /// Add user as Buddy
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        protected void lnk_AddBuddy([NotNull] object sender, [NotNull] CommandEventArgs e)
        {
            if (e.CommandArgument.ToString() == "addbuddy")
            {
                var strBuddyRequest = this.Get<IBuddy>().AddRequest(this.UserId);

                this.PageContext.AddLoadMessage(
                    Convert.ToBoolean(strBuddyRequest[1].ToType<int>())
                        ? this.GetTextFormatted("NOTIFICATION_BUDDYAPPROVED_MUTUAL", strBuddyRequest[0])
                        : this.GetText("NOTIFICATION_BUDDYREQUEST"),
                    MessageTypes.success);
            }
            else
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("REMOVEBUDDY_NOTIFICATION", this.Get<IBuddy>().Remove(this.UserId)),
                    MessageTypes.success);
            }

            this.BindData();
        }

        /// <summary>
        /// add page links.
        /// </summary>
        /// <param name="userDisplayName">
        /// The user display name.
        /// </param>
        private void AddPageLinks([NotNull] string userDisplayName)
        {
            this.PageLinks.Clear();
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("MEMBERS"),
                this.Get<IPermissions>().Check(this.Get<BoardSettings>().MembersListViewPermissions)
                    ? BuildLink.GetLink(ForumPages.Members)
                    : null);
            this.PageLinks.AddLink(userDisplayName, string.Empty);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            MembershipUser user = null;

            try
            {
                user = UserMembershipHelper.GetMembershipUserById(this.UserId);
            }
            catch (Exception ex)
            {
                this.Get<ILogger>().Error(ex, this.UserId.ToString());
            }

            if (user == null || user.ProviderUserKey.ToString() == "0")
            {
                // No such user exists or this is an nntp user ("0")
                BuildLink.AccessDenied();
            }

            if (user.IsApproved == false)
            {
                BuildLink.AccessDenied();
            }

            var userData = new CombinedUserDataHelper(user, this.UserId);

            // populate user information controls...
            // Is BuddyList feature enabled?
            if (this.Get<BoardSettings>().EnableBuddyList)
            {
                this.SetupBuddyList(this.UserId, userData);
            }
            else
            {
                // BuddyList feature is disabled. don't show any link.
                this.lnkBuddy.Visible = false;
                this.BuddyCard.Visible = false;
            }

            var userNameOrDisplayName = this.HtmlEncode(
                this.Get<BoardSettings>().EnableDisplayName ? userData.DisplayName : userData.UserName);

            this.SetupUserProfileInfo(userData);

            this.AddPageLinks(userNameOrDisplayName);

            this.SetupUserStatistics(userData);

            this.SetupUserLinks(userData, userNameOrDisplayName);

            this.SetupAvatar(this.UserId);

            this.Groups.DataSource = RoleMembershipHelper.GetRolesForUser(userData.UserName);

            this.ModerateTab.Visible = this.PageContext.IsAdmin || this.PageContext.IsForumModerator;

            this.AdminUserButton.Visible = this.PageContext.IsAdmin;

            if (this.LastPosts.Visible)
            {
                this.LastPosts.DataSource = this.GetRepository<Message>().AllUserAsDataTable(
                    this.PageContext.PageBoardID,
                    this.UserId,
                    this.PageContext.PageUserID,
                    10).AsEnumerable();

                this.SearchUser.NavigateUrl = BuildLink.GetLinkNotEscaped(
                    ForumPages.Search,
                    "postedby={0}",
                    userNameOrDisplayName);
            }

            this.DataBind();
        }

        /// <summary>
        /// The setup avatar.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        private void SetupAvatar(int userID)
        {
            this.Avatar.ImageUrl = this.Get<IAvatars>().GetAvatarUrlForUser(userID);
        }

        /// <summary>
        /// The setup buddy list.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="userData">
        /// The user data.
        /// </param>
        private void SetupBuddyList(int userID, [NotNull] IUserData userData)
        {
            if (userID == this.PageContext.PageUserID)
            {
                this.lnkBuddy.Visible = false;
            }
            else if (this.Get<IBuddy>().IsBuddy((int)userData.DBRow["userID"], true) && !this.PageContext.IsGuest)
            {
                this.lnkBuddy.Visible = true;
                this.lnkBuddy.Icon = "user-minus";
                this.lnkBuddy.TextLocalizedTag = "REMOVEBUDDY";
                this.lnkBuddy.Type = ButtonStyle.Warning;
                this.lnkBuddy.TextLocalizedPage = "PAGE";
                this.lnkBuddy.CommandArgument = "removebuddy";
                this.lnkBuddy.ReturnConfirmText = this.GetText("FRIENDS", "NOTIFICATION_REMOVE");
            }
            else if (this.Get<IBuddy>().IsBuddy((int)userData.DBRow["userID"], false))
            {
                this.lnkBuddy.Visible = false;
            }
            else
            {
                if (!this.PageContext.IsGuest && !userData.Block.BlockFriendRequests)
                {
                    this.lnkBuddy.Visible = true;
                    this.lnkBuddy.TextLocalizedTag = "ADDBUDDY";
                    this.lnkBuddy.TextLocalizedPage = "PAGE";
                    this.lnkBuddy.Icon = "user-plus";
                    this.lnkBuddy.Type = ButtonStyle.Success;

                    this.lnkBuddy.CommandArgument = "addbuddy";
                }
                else
                {
                    this.lnkBuddy.Visible = false;
                }
            }

            this.BuddyList.CurrentUserID = userID;
            this.BuddyList.Mode = 1;

            this.BuddyCard.Visible = this.BuddyList.Count > 0;
        }

        /// <summary>
        /// The setup user links.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <param name="userName">Name of the user.</param>
        private void SetupUserLinks([NotNull] IUserData userData, string userName)
        {
            // homepage link
            this.HomePlaceHolder.Visible = userData.Profile.Homepage.IsSet();
            this.Home.NavigateUrl = userData.Profile.Homepage;

            // blog link
            this.Blog.Visible = userData.Profile.Blog.IsSet();
            this.SetupThemeButtonWithLink(this.Blog, userData.Profile.Blog);
            this.Blog.ParamTitle0 = userName;

            this.Facebook.Visible = this.User != null && userData.Profile.Facebook.IsSet();

            if (userData.Profile.Facebook.IsSet())
            {
                this.Facebook.NavigateUrl = ValidationHelper.IsNumeric(userData.Profile.Facebook)
                                                ? $"https://www.facebook.com/profile.php?id={userData.Profile.Facebook}"
                                                : userData.Profile.Facebook;
            }

            this.Facebook.ParamTitle0 = userName;

            this.Twitter.Visible = this.User != null && userData.Profile.Twitter.IsSet();
            this.Twitter.NavigateUrl = $"http://twitter.com/{this.HtmlEncode(userData.Profile.Twitter)}";
            this.Twitter.ParamTitle0 = userName;

            if (userData.UserID == this.PageContext.PageUserID)
            {
                return;
            }

            var isFriend = this.GetRepository<Buddy>().CheckIsFriend(this.PageContext.PageUserID, userData.UserID);

            this.PM.Visible = !userData.IsGuest && this.User != null
                                                && this.Get<BoardSettings>().AllowPrivateMessages;

            if (this.PM.Visible)
            {
                if (userData.Block.BlockPMs)
                {
                    this.PM.Visible = false;
                }

                if (this.PageContext.IsAdmin || isFriend)
                {
                    this.PM.Visible = true;
                }
            }

            this.PM.NavigateUrl = BuildLink.GetLinkNotEscaped(ForumPages.PostPrivateMessage, "u={0}", userData.UserID);
            this.PM.ParamTitle0 = userName;

            // email link
            this.Email.Visible = !userData.IsGuest && this.User != null
                                                   && this.Get<BoardSettings>().AllowEmailSending;

            if (this.Email.Visible)
            {
                if (userData.Block.BlockEmails && !this.PageContext.IsAdmin)
                {
                    this.Email.Visible = false;
                }

                if (this.PageContext.IsAdmin || isFriend)
                {
                    this.Email.Visible = true;
                }
            }

            this.Email.NavigateUrl = BuildLink.GetLinkNotEscaped(ForumPages.Email, "u={0}", userData.UserID);
            if (this.PageContext.IsAdmin)
            {
                this.Email.TitleNonLocalized = userData.Membership.Email;
            }

            this.Email.ParamTitle0 = userName;

            this.XMPP.Visible = this.User != null && userData.Profile.XMPP.IsSet();
            this.XMPP.NavigateUrl = BuildLink.GetLinkNotEscaped(ForumPages.Jabber, "u={0}", userData.UserID);
            this.XMPP.ParamTitle0 = userName;

            this.Skype.Visible = this.User != null && userData.Profile.Skype.IsSet();
            this.Skype.NavigateUrl = $"skype:{userData.Profile.Skype}?call";
            this.Skype.ParamTitle0 = userName;

            if (!this.Skype.Visible && !this.Blog.Visible && !this.XMPP.Visible && !this.Facebook.Visible
                && !this.Twitter.Visible)
            {
                this.SocialMediaHolder.Visible = false;
            }
        }

        /// <summary>
        /// The setup user profile info.
        /// </summary>
        /// <param name="userData">The user data.</param>
        private void SetupUserProfileInfo([NotNull] IUserData userData)
        {
            this.UserLabel1.UserID = userData.UserID;

            this.Joined.Text = $"{this.Get<IDateTime>().FormatDateLong(Convert.ToDateTime(userData.Joined))}";

            // vzrus: Show last visit only to admins if user is hidden
            if (!this.PageContext.IsAdmin && Convert.ToBoolean(userData.DBRow["IsActiveExcluded"]))
            {
                this.LastVisit.Text = this.GetText("COMMON", "HIDDEN");
                this.LastVisit.Visible = true;
            }
            else
            {
                this.LastVisitDateTime.DateTime = userData.LastVisit;
                this.LastVisitDateTime.Visible = true;
            }

            if (this.User != null && userData.RankName.IsSet())
            {
                this.RankTR.Visible = true;
                this.Rank.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.RankName));
            }

            if (this.User != null && userData.Profile.Location.IsSet())
            {
                this.LocationTR.Visible = true;
                this.Location.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.Location));
            }

            if (this.User != null && userData.Profile.Country.Trim().IsSet() && !userData.Profile.Country.Equals("N/A"))
            {
                this.CountryTR.Visible = true;
                this.CountryLabel.Text =
                    $"<span class=\"flag-icon flag-icon-{userData.Profile.Country.Trim().ToLower()}\"></span>&nbsp;{this.HtmlEncode(this.Get<IBadWordReplace>().Replace(this.GetText("COUNTRY", userData.Profile.Country.Trim())))}";
            }

            if (this.User != null && userData.Profile.Region.IsSet())
            {
                this.RegionTR.Visible = true;

                try
                {
                    var tag =
                        $"RGN_{(userData.Profile.Country.Trim().IsSet() ? userData.Profile.Country.Trim() : this.Get<ILocalization>().Culture.Name.Remove(0, 3).ToUpperInvariant())}_{userData.Profile.Region}";
                    this.RegionLabel.Text =
                        this.HtmlEncode(this.Get<IBadWordReplace>().Replace(this.GetText("REGION", tag)));
                }
                catch (Exception)
                {
                    this.RegionTR.Visible = false;
                }
            }

            if (this.User != null && userData.Profile.City.IsSet())
            {
                this.CityTR.Visible = true;
                this.CityLabel.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.City));
            }

            if (this.User != null && userData.Profile.Location.IsSet())
            {
                this.LocationTR.Visible = true;
                this.Location.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.Location));
            }

            if (this.User != null && userData.Profile.RealName.IsSet())
            {
                this.RealNameTR.Visible = true;
                this.RealName.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.RealName));
            }

            if (this.User != null && userData.Profile.Interests.IsSet())
            {
                this.InterestsTR.Visible = true;
                this.Interests.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.Interests));
            }

            if (this.User != null && userData.Profile.Gender > 0)
            {
                var imagePath = string.Empty;
                var imageAlt = string.Empty;

                this.GenderTR.Visible = true;

                switch (userData.Profile.Gender)
                {
                    case 1:
                        imagePath = "male";
                        imageAlt = this.GetText("USERGENDER_MAS");
                        break;
                    case 2:
                        imagePath = "female";
                        imageAlt = this.GetText("USERGENDER_FEM");
                        break;
                }

                this.Gender.Text = $@"<i class=""fa fa-{imagePath} fa-fw""></i>&nbsp;{imageAlt}";
            }

            if (this.User != null && userData.Profile.Occupation.IsSet())
            {
                this.OccupationTR.Visible = true;
                this.Occupation.Text =
                    this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.Occupation));
            }

            this.ThanksFrom.Text = this.GetRepository<Thanks>().ThanksFromUser(userData.DBRow["userID"].ToType<int>())
                .ToString();

            var thanksToArray = this.GetRepository<Thanks>().GetUserThanksTo(
                userData.DBRow["userID"].ToType<int>(),
                this.PageContext.PageUserID);

            this.ThanksToTimes.Text = thanksToArray[0].ToString();
            this.ThanksToPosts.Text = thanksToArray[1].ToString();
            this.ReputationReceived.Text = this.Get<IReputation>().GenerateReputationBar(userData.Points.Value, userData.UserID);

            if (this.User != null && userData.Profile.Birthday >= DateTimeHelper.SqlDbMinTime())
            {
                this.BirthdayTR.Visible = true;
                this.Birthday.Text = this.Get<IDateTime>()
                    .FormatDateLong(userData.Profile.Birthday.AddMinutes((double)-userData.TimeZone));
            }
            else
            {
                this.BirthdayTR.Visible = false;
            }

            // Show User Medals
            if (this.Get<BoardSettings>().ShowMedals)
            {
                this.ShowUserMedals();
            }
        }

        /// <summary>
        /// Show the user medals.
        /// </summary>
        private void ShowUserMedals()
        {
            var userMedalsTable = this.Get<DataBroker>().UserMedals(this.UserId);

            if (!userMedalsTable.HasRows())
            {
                this.MedalsRow.Visible = false;

                return;
            }

            var ribbonBar = new StringBuilder(500);
            var medals = new StringBuilder(500);

            DataRow r;
            MedalFlags f;

            var i = 0;
            var inRow = 0;

            // do ribbon bar first
            while (userMedalsTable.Rows.Count > i)
            {
                r = userMedalsTable.Rows[i];
                f = new MedalFlags(r["Flags"]);

                // do only ribbon bar items first
                if (!r["OnlyRibbon"].ToType<bool>())
                {
                    break;
                }

                // skip hidden medals
                if (!f.AllowHiding || !r["Hide"].ToType<bool>())
                {
                    if (inRow == 3)
                    {
                        // add break - only three ribbons in a row
                        ribbonBar.Append("<br />");
                        inRow = 0;
                    }

                    var title = $"{r["Name"]}{(f.ShowMessage ? $": {r["Message"]}" : string.Empty)}";

                    ribbonBar.AppendFormat(
                        "<img src=\"{0}{5}/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}\" title=\"{4}\" class=\"mr-1\" />",
                        BoardInfo.ForumClientFileRoot,
                        r["SmallRibbonURL"],
                        r["SmallRibbonWidth"],
                        r["SmallRibbonHeight"],
                        title,
                        BoardFolders.Current.Medals);

                    inRow++;
                }

                // move to next row
                i++;
            }

            // follow with the rest
            while (userMedalsTable.Rows.Count > i)
            {
                r = userMedalsTable.Rows[i];
                f = new MedalFlags(r["Flags"]);

                // skip hidden medals
                if (!f.AllowHiding || !r["Hide"].ToType<bool>())
                {
                    medals.AppendFormat(
                        "<img src=\"{0}{6}/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}{5}\" title=\"{4}{5}\" class=\"mr-1\" />",
                        BoardInfo.ForumClientFileRoot,
                        r["SmallMedalURL"],
                        r["SmallMedalWidth"],
                        r["SmallMedalHeight"],
                        r["Name"],
                        f.ShowMessage ? $": {r["Message"]}" : string.Empty,
                        BoardFolders.Current.Medals);
                }

                // move to next row
                i++;
            }

            this.MedalsPlaceHolder.Text = $"{ribbonBar}<br />{medals}";
            this.MedalsRow.Visible = true;
        }

        /// <summary>
        /// The setup user statistics.
        /// </summary>
        /// <param name="userData">
        /// The user data.
        /// </param>
        private void SetupUserStatistics([NotNull] IUserData userData)
        {
            var allPosts = 0.0;

            if (userData.DBRow["NumPostsForum"].ToType<int>() > 0)
            {
                allPosts = 100.0 * userData.DBRow["NumPosts"].ToType<int>()
                           / userData.DBRow["NumPostsForum"].ToType<int>();
            }

            this.Stats.Text =
                $"{userData.DBRow["NumPosts"]:N0} [{this.GetTextFormatted("NUMALL", allPosts)} / {this.GetTextFormatted("NUMDAY", (double)userData.DBRow["NumPosts"].ToType<int>() / userData.DBRow["NumDays"].ToType<int>())}]";
        }

        #endregion
    }
}