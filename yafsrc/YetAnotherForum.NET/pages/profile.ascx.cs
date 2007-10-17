/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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
			if ( PageContext.IsPrivate && User == null )
			{
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.login, "ReturnUrl={0}", Request.RawUrl );
			}

			if ( Request.QueryString ["u"] == null )
				YafBuildLink.AccessDenied();

			if ( !IsPostBack )
			{
				userGroupsRow.Visible = PageContext.BoardSettings.ShowGroupsProfile || PageContext.IsAdmin;

				UpdateLast10Panel();

				BindData();

				// handle custom BBCode javascript or CSS...
				YAF.Classes.UI.BBCode.RegisterCustomBBCodePageElements( Page, this.GetType() );
			}
		}

		private void BindData()
		{
			int userID = (int)Security.StringToLongOrRedirect( Request.QueryString ["u"] );

			MembershipUser user = UserMembershipHelper.GetMembershipUser( userID );

			if ( user == null )
			{
				YafBuildLink.AccessDenied(/*No such user exists*/);
			}

			YafCombinedUserData userData = new YafCombinedUserData( user, userID );

			// populate user information controls...
			UserName.Text = HtmlEncode( userData.Membership.UserName );
			Name.Text = HtmlEncode( userData.Membership.UserName );
			Joined.Text = String.Format( "{0}", YafDateTime.FormatDateLong( Convert.ToDateTime(userData.Joined) ) );
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
			if ( ( int ) userData.DBRow ["NumPostsForum"] > 0 )
				dAllPosts = 100.0 * ( int ) userData.DBRow ["NumPosts"] / ( int ) userData.DBRow ["NumPostsForum"];

			Stats.InnerHtml = String.Format( "{0:N0}<br/>[{1} / {2}]",
				userData.DBRow ["NumPosts"],
				String.Format( GetText( "NUMALL" ), dAllPosts ),
				String.Format( GetText( "NUMDAY" ), ( double ) ( int ) userData.DBRow ["NumPosts"] / ( int ) userData.DBRow ["NumDays"] )
				);

			// private messages
			Pm.Visible = User != null && PageContext.BoardSettings.AllowPrivateMessages;
			Pm.Text = GetThemeContents( "BUTTONS", "PM" );
			Pm.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.pmessage, "u={0}", userData.UserID );
			// email link
			Email.Visible = User != null && PageContext.BoardSettings.AllowEmailSending;
			Email.Text = GetThemeContents( "BUTTONS", "EMAIL" );
			Email.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.im_email, "u={0}", userData.UserID );

			if ( PageContext.IsAdmin ) Email.ToolTip = userData.Membership.Email;

			Home.Visible = userData.Profile.Homepage != string.Empty;
			Home.NavigateUrl = userData.Profile.Homepage;
			Home.Text = GetThemeContents( "BUTTONS", "WWW" );
			Blog.Visible = userData.Profile.Blog != string.Empty;
			Blog.NavigateUrl = userData.Profile.Blog;
			Blog.Text = GetThemeContents( "BUTTONS", "WEBLOG" );
			Msn.Visible = User != null && userData.Profile.MSN != string.Empty;
			Msn.Text = GetThemeContents( "BUTTONS", "MSN" );
			Msn.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.im_email, "u={0}", userData.UserID );
			Yim.Visible = User != null && userData.Profile.YIM != string.Empty;;
			Yim.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.im_yim, "u={0}", userData.UserID );
			Yim.Text = GetThemeContents( "BUTTONS", "YAHOO" );
			Aim.Visible = User != null && userData.Profile.AIM != string.Empty;;
			Aim.Text = GetThemeContents( "BUTTONS", "AIM" );
			Aim.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.im_aim, "u={0}", userData.UserID );
			Icq.Visible = User != null &&userData.Profile.ICQ != string.Empty;;
			Icq.Text = GetThemeContents( "BUTTONS", "ICQ" );
			Icq.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.im_icq, "u={0}", userData.UserID );

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
			ModeratorInfo.Visible = PageContext.IsAdmin || PageContext.IsForumModerator;
			AdminUser.Visible = PageContext.IsAdmin;

			if ( PageContext.IsAdmin || PageContext.IsForumModerator )
			{
				// Ederon : 9/6/2007
				SignatureEditControl.InAdminPages = true;

				using ( DataTable dt2 = YAF.Classes.Data.DB.user_accessmasks( PageContext.PageBoardID, userID ) )
				{
					System.Text.StringBuilder html = new System.Text.StringBuilder();
					int nLastForumID = 0;
					foreach ( DataRow row in dt2.Rows )
					{
						if ( nLastForumID != Convert.ToInt32( row ["ForumID"] ) )
						{
							if ( nLastForumID != 0 )
								html.AppendFormat( "</td></tr>" );
							html.AppendFormat( "<tr><td width='50%' class='postheader'>{0}</td><td width='50%' class='post'>", row ["ForumName"] );
							nLastForumID = Convert.ToInt32( row ["ForumID"] );
						}
						else
						{
							html.AppendFormat( ", " );
						}
						html.AppendFormat( "{0}", row ["AccessMaskName"] );
					}
					if ( nLastForumID != 0 )
						html.AppendFormat( "</td></tr>" );
					AccessMaskRow.Text = html.ToString();
				}
			}


			if ( LastPosts.Visible )
			{
				LastPosts.DataSource = YAF.Classes.Data.DB.post_last10user( PageContext.PageBoardID, Request.QueryString ["u"], PageContext.PageUserID );
			}

			DataBind();
		}

		protected string FormatBody( object o )
		{
			DataRowView row = ( DataRowView ) o;
			string html = FormatMsg.FormatMessage( row ["Message"].ToString(), new MessageFlags( Convert.ToInt32( row ["Flags"] ) ) );

			if ( row ["Signature"].ToString().Length > 0 )
			{
				string sig = row ["Signature"].ToString();

				// don't allow any HTML on signatures
				MessageFlags tFlags = new MessageFlags();
				tFlags.IsHTML = false;

				sig = FormatMsg.FormatMessage( sig, tFlags );
				html += "<br/><hr noshade/>" + sig;
			}

			return html;
		}

		private void UpdateLast10Panel()
		{
			expandLast10.ImageUrl = GetCollapsiblePanelImageURL( "ProfileLast10Posts", PanelSessionState.CollapsiblePanelState.Collapsed );
			LastPosts.Visible = ( Mession.PanelState ["ProfileLast10Posts"] == PanelSessionState.CollapsiblePanelState.Expanded );
		}

		protected void expandLast10_Click( object sender, ImageClickEventArgs e )
		{
			// toggle the panel visability state...
			Mession.PanelState.TogglePanelState( "ProfileLast10Posts", PanelSessionState.CollapsiblePanelState.Collapsed );

			UpdateLast10Panel();

			BindData();
		}
	}
}
