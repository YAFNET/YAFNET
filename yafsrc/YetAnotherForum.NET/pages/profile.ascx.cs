/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;
using YAF.Classes.UI;
using YAF.Controls;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for profile.
	/// </summary>
	public partial class profile : YAF.Classes.Base.ForumPage
	{
		protected Repeater ForumAccess;

		public profile()
			: base( "PROFILE" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			// check access permissions
			General.HandleRequest( PageContext, PageContext.BoardSettings.ProfileViewPermissions );

			if ( PageContext.IsPrivate && User == null )
			{
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.login, "ReturnUrl={0}", Request.RawUrl );
			}

			if ( Request.QueryString ["u"] == null )
				YafBuildLink.AccessDenied();

			// admin or moderator, set edit control to moderator mode...
			if ( PageContext.IsAdmin || PageContext.IsForumModerator ) SignatureEditControl.InModeratorMode = true;

			if ( !IsPostBack )
			{
				userGroupsRow.Visible = PageContext.BoardSettings.ShowGroupsProfile || PageContext.IsAdmin;
				BindData();

				// handle custom BBCode javascript or CSS...
				YAF.Classes.UI.BBCode.RegisterCustomBBCodePageElements( Page, this.GetType() );
			}
		}

		private void BindData()
		{
			int userID = ( int )Security.StringToLongOrRedirect( Request.QueryString ["u"] );

			MembershipUser user = UserMembershipHelper.GetMembershipUser( userID );

			if ( user == null )
			{
				YafBuildLink.AccessDenied(/*No such user exists*/);
			}

			YafCombinedUserData userData = new YafCombinedUserData( user, userID );

			// populate user information controls...
			UserName.Text = HtmlEncode( userData.Membership.UserName );
			Name.Text = HtmlEncode( userData.Membership.UserName );
			Joined.Text = String.Format( "{0}", YafDateTime.FormatDateLong( Convert.ToDateTime( userData.Joined ) ) );
			LastVisit.Text = YafDateTime.FormatDateTime( userData.LastVisit );
			Rank.Text = userData.RankName;
			Location.Text = HtmlEncode( General.BadWordReplace( userData.Profile.Location ) );
			RealName.InnerHtml = HtmlEncode( General.BadWordReplace( userData.Profile.RealName ) );
			Interests.InnerHtml = HtmlEncode( General.BadWordReplace( userData.Profile.Interests ) );
			Occupation.InnerHtml = HtmlEncode( General.BadWordReplace( userData.Profile.Occupation ) );
			Gender.InnerText = GetText( "GENDER" + userData.Profile.Gender );

			PageLinks.Clear();
			PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
			PageLinks.AddLink( GetText( "MEMBERS" ), YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.members ) );
			PageLinks.AddLink( userData.Membership.UserName, "" );

			double dAllPosts = 0.0;
			if ( ( int )userData.DBRow ["NumPostsForum"] > 0 )
				dAllPosts = 100.0 * ( int )userData.DBRow ["NumPosts"] / ( int )userData.DBRow ["NumPostsForum"];

			Stats.InnerHtml = String.Format( "{0:N0}<br/>[{1} / {2}]",
				userData.DBRow ["NumPosts"],
				String.Format( GetText( "NUMALL" ), dAllPosts ),
				String.Format( GetText( "NUMDAY" ), ( double )( int )userData.DBRow ["NumPosts"] / ( int )userData.DBRow ["NumDays"] )
				);

			// private messages
			PM.Visible = User != null && PageContext.BoardSettings.AllowPrivateMessages;
			PM.NavigateUrl = YafBuildLink.GetLinkNotEscaped( YAF.Classes.Utils.ForumPages.pmessage, "u={0}", userData.UserID );

			// email link
			Email.Visible = User != null && PageContext.BoardSettings.AllowEmailSending;
			Email.NavigateUrl = YafBuildLink.GetLinkNotEscaped( YAF.Classes.Utils.ForumPages.im_email, "u={0}", userData.UserID );
			if ( PageContext.IsAdmin ) Email.TitleNonLocalized = userData.Membership.Email;

			// homepage link
			Home.Visible = !String.IsNullOrEmpty( userData.Profile.Homepage );
			SetupThemeButtonWithLink( Home, userData.Profile.Homepage );

			// blog link
			Blog.Visible = !String.IsNullOrEmpty( userData.Profile.Blog );
			SetupThemeButtonWithLink( Blog, userData.Profile.Blog );

			MSN.Visible = ( User != null && !String.IsNullOrEmpty( userData.Profile.MSN ) );
			MSN.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.im_email, "u={0}", userData.UserID );

			YIM.Visible = ( User != null && !String.IsNullOrEmpty( userData.Profile.YIM ) );
			YIM.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.im_yim, "u={0}", userData.UserID );

			AIM.Visible = ( User != null && !String.IsNullOrEmpty( userData.Profile.AIM ) );
			AIM.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.im_aim, "u={0}", userData.UserID );

			ICQ.Visible = ( User != null && !String.IsNullOrEmpty( userData.Profile.ICQ ) );
			ICQ.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.im_icq, "u={0}", userData.UserID );

			Skype.Visible = ( User != null && !String.IsNullOrEmpty( userData.Profile.Skype ) );
			Skype.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.im_skype, "u={0}", userData.UserID );

			// localize tab titles...
			AboutTab.HeaderText = GetText( "ABOUT" );
			StatisticsTab.HeaderText = GetText( "STATISTICS" );
			AvatarTab.HeaderText = GetText( "AVATAR" );
			Last10PostsTab.HeaderText = GetText( "LAST10" );

			if ( PageContext.BoardSettings.AvatarUpload && userData.HasAvatarImage )
			{
				Avatar.ImageUrl = YafForumInfo.ForumRoot + "resource.ashx?u=" + ( userID );
			}
			else if ( !String.IsNullOrEmpty( userData.Avatar ) ) // Took out PageContext.BoardSettings.AvatarRemote
			{
				Avatar.ImageUrl = String.Format( "{3}resource.ashx?url={0}&width={1}&height={2}",
					Server.UrlEncode( userData.Avatar ),
					PageContext.BoardSettings.AvatarWidth,
					PageContext.BoardSettings.AvatarHeight,
					YafForumInfo.ForumRoot );
			}
			else
			{
				Avatar.Visible = false;
			}

			Groups.DataSource = Roles.GetRolesForUser( UserMembershipHelper.GetUserNameFromID( userID ) );

			//EmailRow.Visible = PageContext.IsAdmin;
			ModerateTab.Visible = PageContext.IsAdmin || PageContext.IsForumModerator;
			AdminUserButton.Visible = PageContext.IsAdmin;

			if ( LastPosts.Visible )
			{
				LastPosts.DataSource = YAF.Classes.Data.DB.post_last10user( PageContext.PageBoardID, Request.QueryString ["u"], PageContext.PageUserID );
			}

			DataBind();
		}

		protected void SetupThemeButtonWithLink( ThemeButton thisButton, string linkUrl )
		{
			if ( !String.IsNullOrEmpty( linkUrl ) )
			{
				string link = linkUrl.Replace( "\"", "" );
				if ( !link.ToLower().StartsWith( "http" ) )
				{
					link = "http://" + link;
				}
				thisButton.NavigateUrl = link;
				thisButton.Attributes.Add( "target", "_blank" );
				if ( PageContext.BoardSettings.UseNoFollowLinks ) thisButton.Attributes.Add( "rel", "nofollow" );
			}
			else
			{
				thisButton.NavigateUrl = "";
			}
		}

		protected void CollapsibleImage_OnClick( object sender, ImageClickEventArgs e )
		{
			BindData();
		}
	}
}
