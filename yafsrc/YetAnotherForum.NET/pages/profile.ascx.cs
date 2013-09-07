/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Data;
    using System.Text;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The User Profile Page.
    /// </summary>
    public partial class profile : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "profile" /> class.
        /// </summary>
        public profile()
            : base("PROFILE")
        {
            this.AllowAsPopup = true;
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

            set
            {
                this.ViewState["UserId"] = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The albums tab is visible.
        /// </summary>
        /// <returns>
        /// The true if albums tab should be visible.
        /// </returns>
        protected bool AlbumsTabIsVisible()
        {
            int albumUser = this.PageContext.PageUserID;

            if (this.PageContext.IsAdmin && this.UserId > 0)
            {
                albumUser = this.UserId;
            }

            // Add check if Albums Tab is visible 
            if (this.PageContext.IsGuest || !this.Get<YafBoardSettings>().EnableAlbum)
            {
                return false;
            }

            int albumCount = LegacyDb.album_getstats(albumUser, null)[0];

            // Check if the user already has albums.
            if (albumCount > 0)
            {
                return true;
            }

            // If this is the album owner we show him the tab, else it should be hidden 
            if ((albumUser != this.PageContext.PageUserID) && !this.PageContext.IsAdmin)
            {
                return false;
            }

            // Check if a user have permissions to have albums, even if he has no albums at all.
            var usrAlbums =
                LegacyDb.user_getalbumsdata(albumUser, YafContext.Current.PageBoardID)
                    .GetFirstRowColumnAsValue<int?>("UsrAlbums", null);

            return usrAlbums.HasValue && usrAlbums > 0;
        }

        /// <summary>
        /// The collapsible image_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void CollapsibleImage_OnClick([NotNull] object sender, [NotNull] ImageClickEventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // setup jQuery and Jquery Ui Tabs.
            YafContext.Current.PageElements.RegisterJQuery();
            YafContext.Current.PageElements.RegisterJQueryUI();

            YafContext.Current.PageElements.RegisterJsBlock(
                "ProfileTabsJs",
                JavaScriptBlocks.JqueryUITabsLoadJs(this.ProfileTabs.ClientID, this.hidLastTab.ClientID, true));

            // Setup Syntax Highlight JS
            YafContext.Current.PageElements.RegisterJsResourceInclude(
                "syntaxhighlighter", "js/jquery.syntaxhighligher.js");
            YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.syntaxhighligher.css");
            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "syntaxhighlighterjs", JavaScriptBlocks.SyntaxHighlightLoadJs);

            base.OnPreRender(e);
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
            this.lnkThanks.Text = "({0})".FormatWith(this.GetText("VIEWTHANKS", "TITLE"));
            this.lnkThanks.Visible = this.Get<YafBoardSettings>().EnableThanksMod;

            // admin or moderator, set edit control to moderator mode...
            if (this.PageContext.IsAdmin || this.PageContext.IsForumModerator)
            {
                this.SignatureEditControl.InModeratorMode = true;
            }

            if (!this.IsPostBack)
            {
                this.UserId = (int)Security.StringToLongOrRedirect(this.Request.QueryString.GetFirstOrDefault("u"));
                this.userGroupsRow.Visible = this.Get<YafBoardSettings>().ShowGroupsProfile || this.PageContext.IsAdmin;
            }

            if (this.UserId == 0)
            {
                YafBuildLink.AccessDenied(/*No such user exists*/);
            }

            this.AlbumListTab.Visible = this.AlbumsTabIsVisible();
            this.AlbumListLi.Visible = this.AlbumsTabIsVisible();

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
                string link = linkUrl.Replace("\"", string.Empty);
                if (!link.ToLower().StartsWith("http"))
                {
                    link = "http://" + link;
                }

                thisButton.NavigateUrl = link;
                thisButton.Attributes.Add("target", "_blank");
                if (this.Get<YafBoardSettings>().UseNoFollowLinks)
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
                string[] strBuddyRequest = this.Get<IBuddy>().AddRequest(this.UserId);

                var linkButton = (LinkButton)this.ProfileTabs.FindControl("lnkBuddy");

                linkButton.Visible = false;

                if (Convert.ToBoolean(strBuddyRequest[1]))
                {
                    this.PageContext.AddLoadMessage(
                        this.GetText("NOTIFICATION_BUDDYAPPROVED_MUTUAL").FormatWith(strBuddyRequest[0]),
                        MessageTypes.Success);
                }
                else
                {
                    var literal = (Literal)this.ProfileTabs.FindControl("ltrApproval");
                    literal.Visible = true;
                    this.PageContext.AddLoadMessage(this.GetText("NOTIFICATION_BUDDYREQUEST"), MessageTypes.Success);
                }
            }
            else
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("REMOVEBUDDY_NOTIFICATION").FormatWith(this.Get<IBuddy>().Remove(this.UserId)),
                    MessageTypes.Success);
            }

            this.BindData();
        }

        /// <summary>
        /// Go to the View Thanks Page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        protected void lnk_ViewThanks([NotNull] object sender, [NotNull] CommandEventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.viewthanks, "u={0}", this.UserId);
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
            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(
                this.GetText("MEMBERS"),
                this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().MembersListViewPermissions)
                    ? YafBuildLink.GetLink(ForumPages.members)
                    : null);
            this.PageLinks.AddLink(userDisplayName, string.Empty);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            MembershipUser user = UserMembershipHelper.GetMembershipUserById(this.UserId);

            if (user == null || user.ProviderUserKey.ToString() == "0")
            {
                // No such user exists or this is an nntp user ("0")
                YafBuildLink.AccessDenied();
            }

            var userData = new CombinedUserDataHelper(user, this.UserId);

            // populate user information controls...      
            // Is BuddyList feature enabled?
            if (this.Get<YafBoardSettings>().EnableBuddyList)
            {
                this.SetupBuddyList(this.UserId, userData);
            }
            else
            {
                // BuddyList feature is disabled. don't show any link.
                this.BuddyLi.Visible = false;
                this.BuddyListTab.Visible = false;
                this.lnkBuddy.Visible = false;
                this.ltrApproval.Visible = false;
            }

            // Is album feature enabled?
            if (this.Get<YafBoardSettings>().EnableAlbum)
            {
                this.AlbumList1.UserID = this.UserId;
            }
            else
            {
                this.AlbumList1.Dispose();
            }

            var userNameOrDisplayName = this.Get<YafBoardSettings>().EnableDisplayName
                                            ? userData.DisplayName
                                            : userData.UserName;

            this.SetupUserProfileInfo(this.UserId, user, userData, userNameOrDisplayName);

            this.AddPageLinks(userNameOrDisplayName);

            this.SetupUserStatistics(userData);

            this.SetupUserLinks(userData, userNameOrDisplayName);

            this.SetupAvatar(this.UserId, userData);

            this.Groups.DataSource = RoleMembershipHelper.GetRolesForUser(userData.UserName);

            // EmailRow.Visible = PageContext.IsAdmin;
            this.ModerateTab.Visible = this.PageContext.IsAdmin || this.PageContext.IsForumModerator;
            this.ModerateLi.Visible = this.PageContext.IsAdmin || this.PageContext.IsForumModerator;

            this.AdminUserButton.Visible = this.PageContext.IsAdmin;

            if (this.LastPosts.Visible)
            {
                this.LastPosts.DataSource =
                    LegacyDb.post_alluser(this.PageContext.PageBoardID, this.UserId, this.PageContext.PageUserID, 10)
                            .AsEnumerable();

                this.SearchUser.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.search,
                    "postedby={0}",
                    this.Get<YafBoardSettings>().EnableDisplayName ? userData.DisplayName : userData.UserName);
            }

            this.DataBind();
        }

        /// <summary>
        /// The setup avatar.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="userData">
        /// The user data.
        /// </param>
        private void SetupAvatar(int userID, [NotNull] CombinedUserDataHelper userData)
        {
            if (this.Get<YafBoardSettings>().AvatarUpload && userData.HasAvatarImage)
            {
                this.Avatar.ImageUrl = YafForumInfo.ForumClientFileRoot + "resource.ashx?u=" + userID;
            }
            else if (userData.Avatar.IsSet())
            {
                // Took out PageContext.BoardSettings.AvatarRemote
                this.Avatar.ImageUrl =
                    "{3}resource.ashx?url={0}&width={1}&height={2}".FormatWith(
                        this.Server.UrlEncode(userData.Avatar),
                        this.Get<YafBoardSettings>().AvatarWidth,
                        this.Get<YafBoardSettings>().AvatarHeight,
                        YafForumInfo.ForumClientFileRoot);
            }
            else
            {
                this.Avatar.Visible = false;

                this.AvatarTab.Visible = false;
                this.AvatarLi.Visible = false;
            }
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
        private void SetupBuddyList(int userID, [NotNull] CombinedUserDataHelper userData)
        {
            if (userID == this.PageContext.PageUserID)
            {
                this.lnkBuddy.Visible = false;
            }
            else if (this.Get<IBuddy>().IsBuddy((int)userData.DBRow["userID"], true) && !this.PageContext.IsGuest)
            {
                this.lnkBuddy.Visible = true;
                this.lnkBuddy.Text = "({0})".FormatWith(this.GetText("BUDDY", "REMOVEBUDDY"));
                this.lnkBuddy.CommandArgument = "removebuddy";
                this.lnkBuddy.Attributes["onclick"] =
                    "return confirm('{0}')".FormatWith(this.GetText("CP_EDITBUDDIES", "NOTIFICATION_REMOVE"));
            }
            else if (this.Get<IBuddy>().IsBuddy((int)userData.DBRow["userID"], false))
            {
                this.lnkBuddy.Visible = false;
                this.ltrApproval.Visible = true;
            }
            else
            {
                if (!this.PageContext.IsGuest)
                {
                    this.lnkBuddy.Visible = true;
                    this.lnkBuddy.Text = "({0})".FormatWith(this.GetText("BUDDY", "ADDBUDDY"));
                    this.lnkBuddy.CommandArgument = "addbuddy";
                    this.lnkBuddy.Attributes["onclick"] = string.Empty;
                }
            }

            this.BuddyList.CurrentUserID = userID;
            this.BuddyList.Mode = 1;
        }

        /// <summary>
        /// The setup user links.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <param name="userName">Name of the user.</param>
        private void SetupUserLinks([NotNull] IUserData userData, string userName)
        {
            // homepage link
            this.Home.Visible = userData.Profile.Homepage.IsSet();
            this.SetupThemeButtonWithLink(this.Home, userData.Profile.Homepage);
            this.Home.ParamTitle0 = userName;

            // blog link
            this.Blog.Visible = userData.Profile.Blog.IsSet();
            this.SetupThemeButtonWithLink(this.Blog, userData.Profile.Blog);
            this.Blog.ParamTitle0 = userName;

            this.Facebook.Visible = this.User != null && userData.Profile.Facebook.IsSet();

            if (userData.Profile.Facebook.IsSet())
            {
                this.Facebook.NavigateUrl = ValidationHelper.IsNumeric(userData.Profile.Facebook)
                                                ? "https://www.facebook.com/profile.php?id={0}".FormatWith(
                                                    userData.Profile.Facebook)
                                                : userData.Profile.Facebook;
            }

            this.Facebook.ParamTitle0 = userName;

            this.Twitter.Visible = this.User != null && userData.Profile.Twitter.IsSet();
            this.Twitter.NavigateUrl = "http://twitter.com/{0}".FormatWith(userData.Profile.Twitter);
            this.Twitter.ParamTitle0 = userName;

            this.Google.Visible = this.User != null && userData.Profile.Google.IsSet();
            this.Google.NavigateUrl = userData.Profile.Google;
            this.Google.ParamTitle0 = userName;

            if (userData.UserID == this.PageContext.PageUserID)
            {
                return;
            }

            this.PM.Visible = !userData.IsGuest && this.User != null
                              && this.Get<YafBoardSettings>().AllowPrivateMessages;
            this.PM.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", userData.UserID);
            this.PM.ParamTitle0 = userName;

            // email link
            this.Email.Visible = !userData.IsGuest && this.User != null
                                 && this.Get<YafBoardSettings>().AllowEmailSending;
            this.Email.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_email, "u={0}", userData.UserID);
            if (this.PageContext.IsAdmin)
            {
                this.Email.TitleNonLocalized = userData.Membership.Email;
            }

            this.Email.ParamTitle0 = userName;

            this.MSN.Visible = this.User != null && userData.Profile.MSN.IsSet();
            this.MSN.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_msn, "u={0}", userData.UserID);
            this.MSN.ParamTitle0 = userName;

            this.YIM.Visible = this.User != null && userData.Profile.YIM.IsSet();
            this.YIM.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_yim, "u={0}", userData.UserID);
            this.YIM.ParamTitle0 = userName;

            this.AIM.Visible = this.User != null && userData.Profile.AIM.IsSet();
            this.AIM.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_aim, "u={0}", userData.UserID);
            this.AIM.ParamTitle0 = userName;

            this.ICQ.Visible = this.User != null && userData.Profile.ICQ.IsSet();
            this.ICQ.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_icq, "u={0}", userData.UserID);
            this.ICQ.ParamTitle0 = userName;

            this.XMPP.Visible = this.User != null && userData.Profile.XMPP.IsSet();
            this.XMPP.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_xmpp, "u={0}", userData.UserID);
            this.XMPP.ParamTitle0 = userName;

            this.Skype.Visible = this.User != null && userData.Profile.Skype.IsSet();
            this.Skype.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_skype, "u={0}", userData.UserID);
            this.Skype.ParamTitle0 = userName;
        }

        /// <summary>
        /// The setup user profile info.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="userData">
        /// The user data.
        /// </param>
        /// <param name="userDisplayName">
        /// The user display name.
        /// </param>
        private void SetupUserProfileInfo(
            int userID, [NotNull] MembershipUser user, [NotNull] IUserData userData, [NotNull] string userDisplayName)
        {
            this.UserLabel1.UserID = userData.UserID;

            if (this.PageContext.IsAdmin && userData.DisplayName != user.UserName)
            {
                this.Name.Text = this.HtmlEncode("{0} ({1})".FormatWith(userData.DisplayName, user.UserName));
            }
            else
            {
                this.Name.Text = this.HtmlEncode(userDisplayName);
            }

            this.Joined.Text =
                "{0}".FormatWith(this.Get<IDateTime>().FormatDateLong(Convert.ToDateTime(userData.Joined)));

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

            if (this.User != null && !string.IsNullOrEmpty(userData.RankName))
            {
                this.RankTR.Visible = true;
                this.Rank.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.RankName));
            }

            if (this.User != null && !string.IsNullOrEmpty(userData.Profile.Location))
            {
                this.LocationTR.Visible = true;
                this.Location.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.Location));
            }

            if (this.User != null && userData.Profile.Country.Trim().IsSet() && !userData.Profile.Country.Equals("N/A"))
            {
                this.CountryTR.Visible = true;
                this.CountryLabel.Text =
                    this.HtmlEncode(
                        this.Get<IBadWordReplace>().Replace(this.GetText("COUNTRY", userData.Profile.Country.Trim())));

                this.CountryFlagImage.Src = this.Get<ITheme>()
                                                .GetItem(
                                                    "FLAGS",
                                                    "{0}_MEDIUM".FormatWith(userData.Profile.Country.Trim()),
                                                    YafForumInfo.GetURLToResource(
                                                        "images/flags/{0}.png".FormatWith(
                                                            userData.Profile.Country.Trim())));

                this.CountryFlagImage.Alt = userData.Profile.Country.Trim();
                this.CountryFlagImage.Attributes.Add("title", this.CountryLabel.Text);
            }

            if (this.User != null && !string.IsNullOrEmpty(userData.Profile.Region))
            {
                this.RegionTR.Visible = true;

                try
                {
                    var tag =
                        "RGN_{0}_{1}".FormatWith(
                            !string.IsNullOrEmpty(userData.Profile.Country.Trim())
                                ? userData.Profile.Country.Trim()
                                : this.Get<ILocalization>().Culture.Name.Remove(0, 3).ToUpperInvariant(),
                            userData.Profile.Region);
                    this.RegionLabel.Text =
                        this.HtmlEncode(this.Get<IBadWordReplace>().Replace(this.GetText("REGION", tag)));
                }
                catch (Exception)
                {
                    this.RegionTR.Visible = false;
                }
            }

            if (this.User != null && !string.IsNullOrEmpty(userData.Profile.City))
            {
                this.CityTR.Visible = true;
                this.CityLabel.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.City));
            }

            if (this.User != null && !string.IsNullOrEmpty(userData.Profile.Location))
            {
                this.LocationTR.Visible = true;
                this.Location.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.Location));
            }

            if (this.User != null && !string.IsNullOrEmpty(userData.Profile.RealName))
            {
                this.RealNameTR.Visible = true;
                this.RealName.InnerText = this.HtmlEncode(
                    this.Get<IBadWordReplace>().Replace(userData.Profile.RealName));
            }

            if (this.User != null && !string.IsNullOrEmpty(userData.Profile.Interests))
            {
                this.InterestsTR.Visible = true;
                this.Interests.InnerText =
                    this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.Interests));
            }

            if (this.User != null && (userData.Profile.Gender > 0))
            {
                string imagePath = string.Empty;
                string imageAlt = string.Empty;

                this.GenderTR.Visible = true;
                switch (userData.Profile.Gender)
                {
                    case 1:
                        imagePath = this.PageContext.Get<ITheme>().GetItem("ICONS", "GENDER_MALE", null);
                        imageAlt = this.GetText("USERGENDER_MAS");
                        break;
                    case 2:
                        imagePath = this.PageContext.Get<ITheme>().GetItem("ICONS", "GENDER_FEMALE", null);
                        imageAlt = this.GetText("USERGENDER_FEM");
                        break;
                }

                this.Gender.InnerHtml =
                    @"<a><img src=""{0}"" alt=""{1}"" title=""{1}"" /></a>&nbsp;{1}".FormatWith(imagePath, imageAlt);
            }

            if (this.User != null && !string.IsNullOrEmpty(userData.Profile.Occupation))
            {
                this.OccupationTR.Visible = true;
                this.Occupation.InnerText =
                    this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.Occupation));
            }

            this.ThanksFrom.Text =
                LegacyDb.user_getthanks_from(userData.DBRow["userID"], this.PageContext.PageUserID).ToString();
            int[] thanksToArray = LegacyDb.user_getthanks_to(userData.DBRow["userID"], this.PageContext.PageUserID);
            this.ThanksToTimes.Text = thanksToArray[0].ToString();
            this.ThanksToPosts.Text = thanksToArray[1].ToString();
            this.ReputationReceived.Text = YafReputation.GenerateReputationBar(userData.Points.Value, userData.UserID);
            this.OnlineStatusImage1.UserID = userID;
            this.OnlineStatusImage1.Visible = this.Get<YafBoardSettings>().ShowUserOnlineStatus;

            if (this.User != null && !string.IsNullOrEmpty(userData.Profile.XMPP))
            {
                this.XmppTR.Visible = true;
                this.lblxmpp.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.XMPP));
            }

            if (this.User != null && !string.IsNullOrEmpty(userData.Profile.AIM))
            {
                this.AimTR.Visible = true;
                this.lblaim.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.AIM));
            }

            if (this.User != null && !string.IsNullOrEmpty(userData.Profile.ICQ))
            {
                this.IcqTR.Visible = true;
                this.lblicq.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.ICQ));
            }

            if (this.User != null && !string.IsNullOrEmpty(userData.Profile.MSN))
            {
                this.MsnTR.Visible = true;
                this.lblmsn.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.MSN));
            }

            if (this.User != null && !string.IsNullOrEmpty(userData.Profile.Skype))
            {
                this.SkypeTR.Visible = true;
                this.lblskype.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.Skype));
            }

            if (this.User != null && !string.IsNullOrEmpty(userData.Profile.YIM))
            {
                this.YimTR.Visible = true;
                this.lblyim.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.YIM));
            }

            var loadHoverCardJs = false;

            if (this.User != null && !string.IsNullOrEmpty(userData.Profile.Facebook))
            {
                this.FacebookTR.Visible = true;
                this.lblfacebook.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.Facebook));

                if (this.Get<YafBoardSettings>().EnableUserInfoHoverCards)
                {
                    this.lblfacebook.Attributes.Add("data-hovercard", this.lblfacebook.Text);
                    this.Facebook.Attributes.Add("data-hovercard", this.lblfacebook.Text);

                    this.lblfacebook.CssClass += " Facebook-HoverCard";
                    this.Facebook.CssClass += " Facebook-HoverCard";

                    loadHoverCardJs = true;
                }
            }

            if (this.User != null && !string.IsNullOrEmpty(userData.Profile.Twitter))
            {
                this.TwitterTR.Visible = true;
                this.lbltwitter.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(userData.Profile.Twitter));

                if (this.Get<YafBoardSettings>().EnableUserInfoHoverCards)
                {
                    this.lbltwitter.Attributes.Add("data-hovercard", this.lbltwitter.Text);
                    this.Twitter.Attributes.Add("data-hovercard", this.lbltwitter.Text);

                    this.lbltwitter.CssClass += " Twitter-HoverCard";
                    this.Twitter.CssClass += " Twitter-HoverCard";

                    loadHoverCardJs = true;
                }
            }

            if (loadHoverCardJs && this.Get<YafBoardSettings>().EnableUserInfoHoverCards)
            {
                var hoverCardLoadJs = new StringBuilder();

                YafContext.Current.PageElements.RegisterJsResourceInclude("hovercard", "js/jquery.hovercard.js");
                YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.hovercard.css");

                hoverCardLoadJs.Append(
                    JavaScriptBlocks.HoverCardLoadJs(
                        ".Facebook-HoverCard",
                        "Facebook",
                        this.GetText("DEFAULT", "LOADING_FB_HOVERCARD"),
                        this.GetText("DEFAULT", "ERROR_FB_HOVERCARD")));

                hoverCardLoadJs.Append(
                    JavaScriptBlocks.HoverCardLoadJs(
                        ".Twitter-HoverCard",
                        "Twitter",
                        this.GetText("DEFAULT", "LOADING_TWIT_HOVERCARD"),
                        this.GetText("DEFAULT", "ERROR_TWIT_HOVERCARD")));

                // Setup Hover Card JS
                YafContext.Current.PageElements.RegisterJsBlockStartup(
                    "hovercardtwitterfacebookjs", hoverCardLoadJs.ToString());

                if (this.Get<YafBoardSettings>().EnableUserReputation)
                {
                    // Setup UserBox Reputation Script Block
                    YafContext.Current.PageElements.RegisterJsBlockStartup(
                        "reputationprogressjs", JavaScriptBlocks.RepuatationProgressLoadJs);
                }
            }

            if (this.User != null && userData.Profile.Birthday >= DateTimeHelper.SqlDbMinTime())
            {
                this.BirthdayTR.Visible = true;
                this.Birthday.Text =
                    this.Get<IDateTime>()
                        .FormatDateLong(userData.Profile.Birthday.AddMinutes((double)(-userData.TimeZone)));
            }
            else
            {
                this.BirthdayTR.Visible = false;
            }
        }

        /// <summary>
        /// The setup user statistics.
        /// </summary>
        /// <param name="userData">
        /// The user data.
        /// </param>
        private void SetupUserStatistics([NotNull] CombinedUserDataHelper userData)
        {
            double allPosts = 0.0;

            if (userData.DBRow["NumPostsForum"].ToType<int>() > 0)
            {
                allPosts = 100.0 * userData.DBRow["NumPosts"].ToType<int>()
                           / userData.DBRow["NumPostsForum"].ToType<int>();
            }

            this.Stats.InnerHtml = "{0:N0}<br />[{1} / {2}]".FormatWith(
                userData.DBRow["NumPosts"],
                this.GetTextFormatted("NUMALL", allPosts),
                this.GetTextFormatted(
                    "NUMDAY", (double)userData.DBRow["NumPosts"].ToType<int>() / userData.DBRow["NumDays"].ToType<int>()));
        }

        #endregion
    }
}