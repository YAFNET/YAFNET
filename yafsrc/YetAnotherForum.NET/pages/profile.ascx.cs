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
  // YAF.Pages
  #region Using

  using System;
  using System.Data;
  using System.Web.Security;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;
  using YAF.Controls;

  #endregion

  /// <summary>
  /// Summary description for profile.
  /// </summary>
  public partial class profile : ForumPage
  {
    #region Constants and Fields

    /// <summary>
    /// The forum access.
    /// </summary>
    protected Repeater ForumAccess;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="profile"/> class.
    /// </summary>
    public profile()
      : base("PROFILE")
    {
      this.AllowAsPopup = true;
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
      this.PageContext.QueryIDs = new QueryStringIDHelper("u");
      if (this.PageContext.IsAdmin && this.PageContext.QueryIDs.ContainsKey("u"))
      {
        albumUser = (int)this.PageContext.QueryIDs["u"];
      }

      // Add check if Albums Tab is visible 
      if (!this.PageContext.IsGuest && YafContext.Current.BoardSettings.EnableAlbum)
      {
        int albumCount = DB.album_getstats(albumUser, null)[0];

        // Check if the user already has albums.
        if (albumCount > 0)
        {
          return true;
        }
        else
        {
          // Check if a user have permissions to have albums, even if he has no albums at all.
          int? usrAlbums =
            DB.user_getalbumsdata(albumUser, YafContext.Current.PageBoardID).GetFirstRowColumnAsValue<int?>(
              "UsrAlbums", null);

          if (usrAlbums.HasValue)
          {
              return true;
          }
        }
      }

      return false;
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
    protected void CollapsibleImage_OnClick(object sender, ImageClickEventArgs e)
    {
      this.BindData();
    }

    /// <summary>
    /// The lnk_ add buddy.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void lnk_AddBuddy(object sender, CommandEventArgs e)
    {
      var userID = (int)Security.StringToLongOrRedirect(this.Request.QueryString["u"]);
      if ((string)e.CommandArgument == "addbuddy")
      {
        var strBuddyRequest = new string[2];
        strBuddyRequest = YafBuddies.AddBuddyRequest(userID);
        var lnkBuddy = (LinkButton)AboutTab.FindControl("lnkBuddy");
        lnkBuddy.Visible = false;
        if (Convert.ToBoolean(strBuddyRequest[1]))
        {
          this.PageContext.AddLoadMessage(
            string.Format(
              this.PageContext.Localization.GetText("NOTIFICATION_BUDDYAPPROVED_MUTUAL"), strBuddyRequest[0]));
        }
        else
        {
          var ltrApproval = (Literal)AboutTab.FindControl("ltrApproval");
          ltrApproval.Visible = true;
          this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("NOTIFICATION_BUDDYREQUEST"));
        }
      }
      else
      {
        this.PageContext.AddLoadMessage(
          string.Format(
            this.PageContext.Localization.GetText("REMOVEBUDDY_NOTIFICATION"), YafBuddies.RemoveBuddy(userID)));
      }

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
    protected void Page_Load(object sender, EventArgs e)
    {
      if (this.Request.QueryString["u"] == null)
      {
        YafBuildLink.AccessDenied();
      }

      // admin or moderator, set edit control to moderator mode...
      if (this.PageContext.IsAdmin || this.PageContext.IsForumModerator)
      {
        SignatureEditControl.InModeratorMode = true;
      }

      this.AlbumListTab.Visible = this.AlbumsTabIsVisible();

      if (!this.IsPostBack)
      {
        userGroupsRow.Visible = this.PageContext.BoardSettings.ShowGroupsProfile || this.PageContext.IsAdmin;
        this.BindData();
      }
      else
      {
        this.BindData();
      }
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
    protected void SetupThemeButtonWithLink(ThemeButton thisButton, string linkUrl)
    {
      if (!String.IsNullOrEmpty(linkUrl))
      {
        string link = linkUrl.Replace("\"", string.Empty);
        if (!link.ToLower().StartsWith("http"))
        {
          link = "http://" + link;
        }

        thisButton.NavigateUrl = link;
        thisButton.Attributes.Add("target", "_blank");
        if (this.PageContext.BoardSettings.UseNoFollowLinks)
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
    /// The add page links.
    /// </summary>
    /// <param name="userDisplayName">
    /// The user display name.
    /// </param>
    private void AddPageLinks(string userDisplayName)
    {
      this.PageLinks.Clear();
      this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
      this.PageLinks.AddLink(
        this.GetText("MEMBERS"), 
        YafServices.Permissions.Check(this.PageContext.BoardSettings.MembersListViewPermissions)
          ? YafBuildLink.GetLink(ForumPages.members)
          : null);
      this.PageLinks.AddLink(userDisplayName, string.Empty);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      var userID = (int)Security.StringToLongOrRedirect(this.Request.QueryString["u"]);

      MembershipUser user = UserMembershipHelper.GetMembershipUserById(userID);

      if (user == null)
      {
        YafBuildLink.AccessDenied( /*No such user exists*/);
      }

      var userData = new CombinedUserDataHelper(user, userID);

      // populate user information controls...      
      // Is BuddyList feature enabled?
      if (YafContext.Current.BoardSettings.EnableBuddyList)
      {
        this.SetupBuddyList(userID, userData);
      }
      else
      {
        // BuddyList feature is disabled. don't show any link.
        lnkBuddy.Visible = false;
        ltrApproval.Visible = false;
      }

      // Is album feature enabled?
      if (YafContext.Current.BoardSettings.EnableAlbum)
      {
        AlbumList1.UserID = userID;
      }
      else
      {
        AlbumList1.Dispose();
      }

      string userDisplayName = PageContext.UserDisplayName.GetName(userID);

      this.SetupUserProfileInfo(userID, user, userData, userDisplayName);

      this.AddPageLinks(userDisplayName);

      this.SetupUserStatistics(userData);

      // private messages
      this.SetupUserLinks(userData);

      // localize tab titles...
      this.LocalizeTabTitles();

      this.SetupAvatar(userID, userData);

      Groups.DataSource = RoleMembershipHelper.GetRolesForUser(UserMembershipHelper.GetUserNameFromID(userID));

      // EmailRow.Visible = PageContext.IsAdmin;
      this.ProfileTabs.Views["ModerateTab"].Visible = this.PageContext.IsAdmin || this.PageContext.IsForumModerator;
      this.ProfileTabs.Views["ModerateTab"].Text = this.GetText("MODERATION");
      this.AdminUserButton.Visible = this.PageContext.IsAdmin;

      if (LastPosts.Visible)
      {
        LastPosts.DataSource = DB.post_alluser(this.PageContext.PageBoardID, userID, this.PageContext.PageUserID, 10);
        SearchUser.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.search, "postedby={0}", userID);
      }

      this.DataBind();
    }

    /// <summary>
    /// The localize tab titles.
    /// </summary>
    private void LocalizeTabTitles()
    {
      this.ProfileTabs.Views["AboutTab"].Text = this.GetText("ABOUT");
      this.ProfileTabs.Views["StatisticsTab"].Text = this.GetText("STATISTICS");
      this.ProfileTabs.Views["AvatarTab"].Text = this.GetText("AVATAR");
      this.ProfileTabs.Views["Last10PostsTab"].Text = this.GetText("LAST10");
      this.ProfileTabs.Views["BuddyListTab"].Text = this.GetText("BUDDIES");
      this.ProfileTabs.Views["AlbumListTab"].Text = this.GetText("ALBUMS");
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
    private void SetupAvatar(int userID, CombinedUserDataHelper userData)
    {
      if (this.PageContext.BoardSettings.AvatarUpload && userData.HasAvatarImage)
      {
        Avatar.ImageUrl = YafForumInfo.ForumClientFileRoot + "resource.ashx?u=" + userID;
      }
      else if (!String.IsNullOrEmpty(userData.Avatar))
      {
        // Took out PageContext.BoardSettings.AvatarRemote
        Avatar.ImageUrl = String.Format(
          "{3}resource.ashx?url={0}&width={1}&height={2}", 
          this.Server.UrlEncode(userData.Avatar), 
          this.PageContext.BoardSettings.AvatarWidth, 
          this.PageContext.BoardSettings.AvatarHeight, 
          YafForumInfo.ForumClientFileRoot);
      }
      else
      {
        Avatar.Visible = false;
        AvatarTab.Visible = false;
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
    private void SetupBuddyList(int userID, CombinedUserDataHelper userData)
    {
      if (userID == this.PageContext.PageUserID)
      {
        lnkBuddy.Visible = false;
      }
      else if (YafBuddies.IsBuddy((int)userData.DBRow["userID"], true))
      {
        lnkBuddy.Visible = true;
        lnkBuddy.Text = string.Format("({0})", this.PageContext.Localization.GetText("BUDDY", "REMOVEBUDDY"));
        lnkBuddy.CommandArgument = "removebuddy";
        lnkBuddy.Attributes["onclick"] = "return confirm('Remove Buddy?')";
      }
      else if (YafBuddies.IsBuddy((int)userData.DBRow["userID"], false))
      {
        lnkBuddy.Visible = false;
        ltrApproval.Visible = true;
      }
      else
      {
        lnkBuddy.Visible = true;
        lnkBuddy.Text = string.Format("({0})", this.PageContext.Localization.GetText("BUDDY", "ADDBUDDY"));
        lnkBuddy.CommandArgument = "addbuddy";
        lnkBuddy.Attributes["onclick"] = string.Empty;
      }

      BuddyList.CurrentUserID = userID;
      BuddyList.Mode = 1;
    }

    /// <summary>
    /// The setup user links.
    /// </summary>
    /// <param name="userData">
    /// The user data.
    /// </param>
    private void SetupUserLinks(CombinedUserDataHelper userData)
    {
      this.PM.Visible = !userData.IsGuest && this.User != null && this.PageContext.BoardSettings.AllowPrivateMessages;
      this.PM.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", userData.UserID);

      // email link
      this.Email.Visible = !userData.IsGuest && this.User != null && this.PageContext.BoardSettings.AllowEmailSending;
      this.Email.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_email, "u={0}", userData.UserID);
      if (this.PageContext.IsAdmin)
      {
        this.Email.TitleNonLocalized = userData.Membership.Email;
      }

      // homepage link
      this.Home.Visible = !String.IsNullOrEmpty(userData.Profile.Homepage);
      this.SetupThemeButtonWithLink(this.Home, userData.Profile.Homepage);

      // blog link
      this.Blog.Visible = !String.IsNullOrEmpty(userData.Profile.Blog);
      this.SetupThemeButtonWithLink(this.Blog, userData.Profile.Blog);

      this.MSN.Visible = this.User != null && !String.IsNullOrEmpty(userData.Profile.MSN);
      this.MSN.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_msn, "u={0}", userData.UserID);

      this.YIM.Visible = this.User != null && !String.IsNullOrEmpty(userData.Profile.YIM);
      this.YIM.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_yim, "u={0}", userData.UserID);

      this.AIM.Visible = this.User != null && !String.IsNullOrEmpty(userData.Profile.AIM);
      this.AIM.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_aim, "u={0}", userData.UserID);

      this.ICQ.Visible = this.User != null && !String.IsNullOrEmpty(userData.Profile.ICQ);
      this.ICQ.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_icq, "u={0}", userData.UserID);

      this.XMPP.Visible = this.User != null && !String.IsNullOrEmpty(userData.Profile.XMPP);
      this.XMPP.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_xmpp, "u={0}", userData.UserID);

      this.Skype.Visible = this.User != null && !String.IsNullOrEmpty(userData.Profile.Skype);
      this.Skype.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_skype, "u={0}", userData.UserID);
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
      int userID, MembershipUser user, CombinedUserDataHelper userData, string userDisplayName)
    {
      this.UserLabel1.UserID = userData.UserID;

      if (this.PageContext.IsAdmin && userDisplayName != user.UserName)
      {
        Name.Text = this.HtmlEncode(String.Format("{0} ({1})", userDisplayName, user.UserName));
      }
      else
      {
        Name.Text = this.HtmlEncode(userDisplayName);
      }

      Joined.Text = String.Format("{0}", YafServices.DateTime.FormatDateLong(Convert.ToDateTime(userData.Joined)));

      // vzrus: Show last visit only to admins if user is hidden
      if (!this.PageContext.IsAdmin && Convert.ToBoolean(userData.DBRow["IsActiveExcluded"]))
      {
        LastVisit.Text = this.GetText("COMMON", "HIDDEN");
      }
      else
      {
        LastVisit.Text = YafServices.DateTime.FormatDateTime(userData.LastVisit);
      }

      if (this.User != null && !string.IsNullOrEmpty(userData.RankName))
      {
          this.RankTR.Visible = true;
          this.Rank.Text = this.HtmlEncode(YafServices.BadWordReplace.Replace(userData.RankName));
      }

      if (this.User != null && !string.IsNullOrEmpty(userData.Profile.Location))
      {
          this.LocationTR.Visible = true;
          this.Location.Text = this.HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.Location));
      }

      if (this.User != null && !string.IsNullOrEmpty(userData.Profile.Location))
      {
          this.LocationTR.Visible = true;
          this.Location.Text = this.HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.Location));
      }

      if (this.User != null && !string.IsNullOrEmpty(userData.Profile.RealName))
      {
          this.RealNameTR.Visible = true;
          this.RealName.InnerHtml = this.HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.RealName));
      }

      if (this.User != null && !string.IsNullOrEmpty(userData.Profile.Interests))
      {
          this.InterestsTR.Visible = true;
          this.Interests.InnerHtml = this.HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.Interests));
      }

      if (this.User != null && !string.IsNullOrEmpty(userData.Profile.Occupation))
      {
          this.OccupationTR.Visible = true;
          this.Occupation.InnerHtml = this.HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.Occupation));
      }

          // Handled in localization. 
          this.Gender.InnerText = this.GetText("GENDER" + userData.Profile.Gender);     
      
      this.ThanksFrom.Text = DB.user_getthanks_from(userData.DBRow["userID"]).ToString();
      int[] ThanksToArray = DB.user_getthanks_to(userData.DBRow["userID"]);
      this.ThanksToTimes.Text = ThanksToArray[0].ToString();
      this.ThanksToPosts.Text = ThanksToArray[1].ToString();
      this.OnlineStatusImage1.UserID = userID;
      this.OnlineStatusImage1.Visible = this.PageContext.BoardSettings.ShowUserOnlineStatus;

         if (this.User != null && !string.IsNullOrEmpty(userData.Profile.XMPP))
        {
          this.XmppTR.Visible = true;
          this.lblxmpp.Text = this.HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.XMPP));
        }

        if (this.User != null && !string.IsNullOrEmpty(userData.Profile.AIM))
        {
          this.AimTR.Visible = true;
          this.lblaim.Text = this.HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.AIM));
        }

        if (this.User != null && !string.IsNullOrEmpty(userData.Profile.ICQ))
        {
          this.IcqTR.Visible = true;
          this.lblicq.Text = this.HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.ICQ));
        }

        if (this.User != null && !string.IsNullOrEmpty(userData.Profile.MSN))
      {
          this.MsnTR.Visible = true;
          this.lblmsn.Text = this.HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.MSN));          
      }

        if (this.User != null && !string.IsNullOrEmpty(userData.Profile.Skype))
      {
          this.SkypeTR.Visible = true;
          this.lblskype.Text = this.HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.Skype));
      }

        if (this.User != null && !string.IsNullOrEmpty(userData.Profile.YIM))
      {
          this.YimTR.Visible = true;
          this.lblyim.Text = this.HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.YIM));
      }

      if (this.User != null && userData.Profile.Birthday != DateTime.MinValue)
      {
          this.BirthdayTR.Visible = true;
          this.Birthday.Text = YafServices.DateTime.FormatDateLong(userData.Profile.Birthday.Date.Add(-YafServices.DateTime.TimeOffset));
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
    private void SetupUserStatistics(CombinedUserDataHelper userData)
    {
      double dAllPosts = 0.0;

      if (SqlDataLayerConverter.VerifyInt32(userData.DBRow["NumPostsForum"]) > 0)
      {
        dAllPosts = 100.0 * SqlDataLayerConverter.VerifyInt32(userData.DBRow["NumPosts"]) /
                    SqlDataLayerConverter.VerifyInt32(userData.DBRow["NumPostsForum"]);
      }

      Stats.InnerHtml = String.Format(
        "{0:N0}<br/>[{1} / {2}]", 
        userData.DBRow["NumPosts"], 
        this.GetTextFormatted("NUMALL", dAllPosts), 
        this.GetTextFormatted(
          "NUMDAY", 
          (double)SqlDataLayerConverter.VerifyInt32(userData.DBRow["NumPosts"]) /
          SqlDataLayerConverter.VerifyInt32(userData.DBRow["NumDays"])));
    }

    #endregion
  }
}