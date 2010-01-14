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
  using System;
  using System.Web.Security;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;
  using YAF.Controls;

  /// <summary>
  /// Summary description for profile.
  /// </summary>
  public partial class profile : ForumPage
  {
    /// <summary>
    /// The forum access.
    /// </summary>
    protected Repeater ForumAccess;

    /// <summary>
    /// Initializes a new instance of the <see cref="profile"/> class.
    /// </summary>
    public profile()
      : base("PROFILE")
    {
      AllowAsPopup = true;
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
      if (Request.QueryString["u"] == null)
      {
        YafBuildLink.AccessDenied();
      }

      // admin or moderator, set edit control to moderator mode...
      if (PageContext.IsAdmin || PageContext.IsForumModerator)
      {
        SignatureEditControl.InModeratorMode = true;
      }

      if (!IsPostBack)
      {
        userGroupsRow.Visible = PageContext.BoardSettings.ShowGroupsProfile || PageContext.IsAdmin;
        BindData();
      }
      else
      {
        BindData();
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      var userID = (int) Security.StringToLongOrRedirect(Request.QueryString["u"]);

      MembershipUser user = UserMembershipHelper.GetMembershipUserById(userID);

      if (user == null)
      {
        YafBuildLink.AccessDenied(/*No such user exists*/);
      }

      var userData = new CombinedUserDataHelper(user, userID);

          // populate user information controls...      
          // Is BuddyList feature enabled?
      if (YafContext.Current.BoardSettings.EnableBuddyList)
      {
          if (userID == PageContext.PageUserID)
          {
              lnkBuddy.Visible = false;
          }
          else if (YafBuddies.IsBuddy((int)(userData.DBRow["userID"]), true))
          {
              lnkBuddy.Visible = true;
              lnkBuddy.Text = string.Format("({0})", PageContext.Localization.GetText("BUDDY", "REMOVEBUDDY"));
              lnkBuddy.CommandArgument = "removebuddy";
              lnkBuddy.Attributes["onclick"] = "return confirm('Remove Buddy?')";
          }
          else if (YafBuddies.IsBuddy((int)(userData.DBRow["userID"]), false))
          {
              lnkBuddy.Visible = false;
              ltrApproval.Visible = true;
          }
          else
          {
              lnkBuddy.Visible = true;
              lnkBuddy.Text = string.Format("({0})", PageContext.Localization.GetText("BUDDY", "ADDBUDDY"));
              lnkBuddy.CommandArgument = "addbuddy";
              lnkBuddy.Attributes["onclick"] = "";
          }

          BuddyList.CurrentUserID = userID;
          BuddyList.Mode = 1;
      }
      else
      {
          //  BuddyList feature is disabled. don't show any link.
          lnkBuddy.Visible = false;
          ltrApproval.Visible = false;
      }
      
      // Is album feature enabled?
      if (YafContext.Current.BoardSettings.EnableAlbum)
      {
          AlbumList1.UserID = Convert.ToInt32(Request.QueryString.Get("u"));
      }
      else
      {
          AlbumList1.Dispose();
      }
      this.UserName.Text = HtmlEncode(userData.Membership.UserName);
      Name.Text = HtmlEncode(userData.Membership.UserName);
      Joined.Text = String.Format("{0}", YafServices.DateTime.FormatDateLong(Convert.ToDateTime(userData.Joined)));
      // vzrus: Show last visit only to admins if user is hidden
      if (!PageContext.IsAdmin && Convert.ToBoolean(userData.DBRow["IsActiveExcluded"]))
      {
         LastVisit.Text = GetText("COMMON", "HIDDEN");
      }
      else
      {
      LastVisit.Text = YafServices.DateTime.FormatDateTime(userData.LastVisit);
      }

      Rank.Text = userData.RankName;
      Location.Text = HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.Location));
      RealName.InnerHtml = HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.RealName));
      Interests.InnerHtml = HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.Interests));
      Occupation.InnerHtml = HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.Occupation));
      Gender.InnerText = GetText("GENDER" + userData.Profile.Gender);
      ThanksFrom.Text = DB.user_getthanks_from(userData.DBRow["userID"]).ToString();
      int[] ThanksToArray = DB.user_getthanks_to(userData.DBRow["userID"]);
      ThanksToTimes.Text = ThanksToArray[0].ToString();
      ThanksToPosts.Text = ThanksToArray[1].ToString();
      OnlineStatusImage1.UserID = userID;
      OnlineStatusImage1.Visible = PageContext.BoardSettings.ShowUserOnlineStatus;
        

      lblaim.Text = HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.AIM));
      lblicq.Text = HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.ICQ));
      lblmsn.Text = HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.MSN));
      lblskype.Text = HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.Skype));
      lblyim.Text = HtmlEncode(YafServices.BadWordReplace.Replace(userData.Profile.YIM));

      this.PageLinks.Clear();
      this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
      this.PageLinks.AddLink(GetText("MEMBERS"), YafBuildLink.GetLink(ForumPages.members));
      this.PageLinks.AddLink(userData.Membership.UserName, string.Empty);

      double dAllPosts = 0.0;
      if (SqlDataLayerConverter.VerifyInt32(userData.DBRow["NumPostsForum"]) > 0)
      {
        dAllPosts = 100.0 * SqlDataLayerConverter.VerifyInt32(userData.DBRow["NumPosts"]) / SqlDataLayerConverter.VerifyInt32(userData.DBRow["NumPostsForum"]);
      }

      Stats.InnerHtml = String.Format(
        "{0:N0}<br/>[{1} / {2}]", 
        userData.DBRow["NumPosts"], 
        GetTextFormatted("NUMALL", dAllPosts), 
        GetTextFormatted(
          "NUMDAY", (double) SqlDataLayerConverter.VerifyInt32(userData.DBRow["NumPosts"]) / SqlDataLayerConverter.VerifyInt32(userData.DBRow["NumDays"])));

      // private messages
      this.PM.Visible = !userData.IsGuest && User != null && PageContext.BoardSettings.AllowPrivateMessages;
      this.PM.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", userData.UserID);

      // email link
      this.Email.Visible = !userData.IsGuest && User != null && PageContext.BoardSettings.AllowEmailSending;
      this.Email.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_email, "u={0}", userData.UserID);
      if (PageContext.IsAdmin)
      {
        this.Email.TitleNonLocalized = userData.Membership.Email;
      }

      // homepage link
      this.Home.Visible = !String.IsNullOrEmpty(userData.Profile.Homepage);
      SetupThemeButtonWithLink(this.Home, userData.Profile.Homepage);

      // blog link
      this.Blog.Visible = !String.IsNullOrEmpty(userData.Profile.Blog);
      SetupThemeButtonWithLink(this.Blog, userData.Profile.Blog);

      this.MSN.Visible = User != null && !String.IsNullOrEmpty(userData.Profile.MSN);
      this.MSN.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_msn, "u={0}", userData.UserID);

      this.YIM.Visible = User != null && !String.IsNullOrEmpty(userData.Profile.YIM);
      this.YIM.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_yim, "u={0}", userData.UserID);

      this.AIM.Visible = User != null && !String.IsNullOrEmpty(userData.Profile.AIM);
      this.AIM.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_aim, "u={0}", userData.UserID);

      this.ICQ.Visible = User != null && !String.IsNullOrEmpty(userData.Profile.ICQ);
      this.ICQ.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_icq, "u={0}", userData.UserID);

      this.Skype.Visible = User != null && !String.IsNullOrEmpty(userData.Profile.Skype);
      this.Skype.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_skype, "u={0}", userData.UserID);

      // localize tab titles...
      this.ProfileTabs.Views["AboutTab"].Text = GetText("ABOUT");
      this.ProfileTabs.Views["StatisticsTab"].Text = GetText("STATISTICS");
      this.ProfileTabs.Views["AvatarTab"].Text = GetText("AVATAR");
      this.ProfileTabs.Views["Last10PostsTab"].Text = GetText("LAST10");
      this.ProfileTabs.Views["BuddyListTab"].Text = GetText("BUDDIES");
      this.ProfileTabs.Views["AlbumListTab"].Text = GetText("ALBUMS");

      if (PageContext.BoardSettings.AvatarUpload && userData.HasAvatarImage)
      {
        Avatar.ImageUrl = YafForumInfo.ForumRoot + "resource.ashx?u=" + userID;
      }
      else if (!String.IsNullOrEmpty(userData.Avatar))
      {
        // Took out PageContext.BoardSettings.AvatarRemote
        Avatar.ImageUrl = String.Format(
          "{3}resource.ashx?url={0}&width={1}&height={2}", 
          Server.UrlEncode(userData.Avatar), 
          PageContext.BoardSettings.AvatarWidth, 
          PageContext.BoardSettings.AvatarHeight, 
          YafForumInfo.ForumRoot);
      }
      else
      {
        Avatar.Visible = false;
        AvatarTab.Visible = false;
      }

      Groups.DataSource = RoleMembershipHelper.GetRolesForUser(UserMembershipHelper.GetUserNameFromID(userID));

      // EmailRow.Visible = PageContext.IsAdmin;
      this.ProfileTabs.Views["ModerateTab"].Visible = PageContext.IsAdmin || PageContext.IsForumModerator;
      this.ProfileTabs.Views["ModerateTab"].Text = GetText("MODERATION");
      this.AdminUserButton.Visible = PageContext.IsAdmin;

      if (LastPosts.Visible)
      {
        LastPosts.DataSource = DB.post_last10user(PageContext.PageBoardID, Request.QueryString["u"], PageContext.PageUserID);
        SearchUser.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.search, "postedby={0}", userData.Membership.UserName);
      }
      
      DataBind();
    }

    protected void lnk_AddBuddy(object sender, CommandEventArgs e)
    {
        var userID = (int)Security.StringToLongOrRedirect(Request.QueryString["u"]);
        if ((string)(e.CommandArgument) == "addbuddy")
        {
            string[] strBuddyRequest = new string[2];
            strBuddyRequest = YafBuddies.AddBuddyRequest(userID);
            LinkButton lnkBuddy = (LinkButton)(AboutTab.FindControl("lnkBuddy"));
            lnkBuddy.Visible = false;
            if (Convert.ToBoolean(strBuddyRequest[1]))
            {
                PageContext.AddLoadMessage(string.Format(PageContext.Localization.GetText("NOTIFICATION_BUDDYAPPROVED_MUTUAL"), strBuddyRequest[0]));
            }
            else
            {
                Literal ltrApproval = (Literal)(AboutTab.FindControl("ltrApproval"));
                ltrApproval.Visible = true;
                PageContext.AddLoadMessage(PageContext.Localization.GetText("NOTIFICATION_BUDDYREQUEST"));
            }
        }
        else
        {
            PageContext.AddLoadMessage(string.Format
                                        (
                                        PageContext.Localization.GetText("REMOVEBUDDY_NOTIFICATION"),
                                        YafBuddies.RemoveBuddy(userID))
                                        );
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
        if (PageContext.BoardSettings.UseNoFollowLinks)
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
  }
}