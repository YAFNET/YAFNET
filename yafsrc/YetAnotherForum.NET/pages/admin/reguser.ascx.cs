/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.Security;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using YAF.Classes.Utils;

	/// <summary>
	///		Summary description for reguser.
	/// </summary>
	public partial class reguser : YAF.Classes.Base.AdminPage
	{


		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin ) );
				PageLinks.AddLink( "Users", "" );

				TimeZones.DataSource = YafStaticData.TimeZones();
				DataBind();
				TimeZones.Items.FindByValue( "0" ).Selected = true;
			}
		}

		protected void cancel_Click( object sender, EventArgs e )
		{
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_users );
		}

		protected void ForumRegister_Click( object sender, System.EventArgs e )
		{
			if ( Page.IsValid )
			{
				string newEmail = Email.Text.Trim();
				string newUsername = UserName.Text.Trim();

				if ( !General.IsValidEmail( newEmail ) )
				{
					PageContext.AddLoadMessage( "You have entered an illegal e-mail address." );
					return;
				}

				if ( UserMembershipHelper.UserExists( UserName.Text.Trim(), newEmail ) )
				{
					PageContext.AddLoadMessage( "Username or email are already registered." );
					return;
				}

				string hashinput = DateTime.Now.ToString() + newEmail + Security.CreatePassword( 20 );
				string hash = FormsAuthentication.HashPasswordForStoringInConfigFile( hashinput, "md5" );

				MembershipCreateStatus status;
				MembershipUser user = Membership.CreateUser( newUsername, Password.Text.Trim(), newEmail, Question.Text.Trim(), Answer.Text.Trim(), !PageContext.BoardSettings.EmailVerification, out status);

				if (status != MembershipCreateStatus.Success)
				{
					// error of some kind
					PageContext.AddLoadMessage( "Membership Error Creating User: " + status.ToString() );
					return;
				}

				// setup inital roles (if any) for this user
				RoleMembershipHelper.SetupUserRoles( YafContext.Current.PageBoardID, newUsername );

				// create the user in the YAF DB as well as sync roles...
				int? userID = RoleMembershipHelper.CreateForumUser( user, YafContext.Current.PageBoardID );

				// create profile
				YafUserProfile userProfile = PageContext.GetProfile( newUsername );
				// setup their inital profile information
				userProfile.Location = Location.Text.Trim();
				userProfile.Homepage = HomePage.Text.Trim();
				userProfile.Save();

				// save the time zone...
				YAF.Classes.Data.DB.user_save( UserMembershipHelper.GetUserIDFromProviderUserKey( user.ProviderUserKey ), PageContext.PageBoardID, null, null, Convert.ToInt32( TimeZones.SelectedValue ), null, null, null, null, null );

				if ( PageContext.BoardSettings.EmailVerification )
				{
					// send template email
					YafTemplateEmail verifyEmail = new YafTemplateEmail( "VERIFYEMAIL" );

					verifyEmail.TemplateParams ["{link}"] = String.Format( "{1}{0}", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.approve, "k={0}", hash ), YAF.Classes.Utils.YafForumInfo.ServerURL );
					verifyEmail.TemplateParams ["{key}"] = hash;
					verifyEmail.TemplateParams ["{forumname}"] = PageContext.BoardSettings.Name;
					verifyEmail.TemplateParams ["{forumlink}"] = String.Format( "{0}", ForumURL );

					string subject = String.Format( PageContext.Localization.GetText( "COMMON", "EMAILVERIFICATION_SUBJECT" ), PageContext.BoardSettings.Name );

					verifyEmail.SendEmail( new System.Net.Mail.MailAddress( newEmail, newUsername ), subject, true);
				}

				// success
				PageContext.AddLoadMessage( string.Format( "User {0} Created Successfully.", UserName.Text.Trim() ) );
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_reguser );
			}
		}

	}
}
