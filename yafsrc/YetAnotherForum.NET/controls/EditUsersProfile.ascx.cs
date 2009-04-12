/* Yet Another Forum.NET
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
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.Globalization;
using System.Collections.Specialized;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public partial class EditUsersProfile : YAF.Classes.Base.BaseUserControl
	{
		private int CurrentUserID;
		private bool AdminEditMode = false;

		protected void Page_Load( object sender, EventArgs e )
		{
			PageContext.QueryIDs = new QueryStringIDHelper( "u" );

			if ( AdminEditMode && PageContext.IsAdmin && this.PageContext.QueryIDs.ContainsKey( "u" ) )
			{
				CurrentUserID = ( int )this.PageContext.QueryIDs ["u"];
			}
			else
			{
				CurrentUserID = PageContext.PageUserID;
			}

			if ( !IsPostBack )
			{
				LoginInfo.Visible = true;

				// Begin Modifications for enhanced profile
				Gender.Items.Add( PageContext.Localization.GetText( "PROFILE", "gender0" ) );
				Gender.Items.Add( PageContext.Localization.GetText( "PROFILE", "gender1" ) );
				Gender.Items.Add( PageContext.Localization.GetText( "PROFILE", "gender2" ) );
				// End Modifications for enhanced profile				

				UpdateProfile.Text = PageContext.Localization.GetText( "COMMON", "SAVE" );
				Cancel.Text = PageContext.Localization.GetText( "COMMON", "CANCEL" );

				ForumSettingsRows.Visible = PageContext.BoardSettings.AllowUserTheme || PageContext.BoardSettings.AllowUserLanguage || PageContext.BoardSettings.AllowPMEmailNotification;
				UserThemeRow.Visible = PageContext.BoardSettings.AllowUserTheme;
				UserLanguageRow.Visible = PageContext.BoardSettings.AllowUserLanguage;
				PMNotificationRow.Visible = PageContext.BoardSettings.AllowPMEmailNotification;
				MetaWeblogAPI.Visible = PageContext.BoardSettings.AllowPostToBlog;
                LoginInfo.Visible = PageContext.BoardSettings.AllowEmailChange;

				BindData();
			}
		}
		private void BindData()
		{
			TimeZones.DataSource = YafStaticData.TimeZones();
			Theme.DataSource = YafStaticData.Themes();
			Theme.DataTextField = "Theme";
			Theme.DataValueField = "FileName";
			Language.DataSource = YafStaticData.Languages();
			Language.DataTextField = "Language";
			Language.DataValueField = "FileName";

			DataBind();

			// get an instance of the combined user data class.
			YafCombinedUserData userData = new YafCombinedUserData( CurrentUserID );

			Location.Text = userData.Profile.Location;
			HomePage.Text = userData.Profile.Homepage;
			Email.Text = userData.Email;
			Realname.Text = userData.Profile.RealName;
			Occupation.Text = userData.Profile.Occupation;
			Interests.Text = userData.Profile.Interests;
			Weblog.Text = userData.Profile.Blog;
			WeblogUrl.Text = userData.Profile.BlogServiceUrl;
			WeblogID.Text = userData.Profile.BlogServicePassword;
			WeblogUsername.Text = userData.Profile.BlogServiceUsername;
			MSN.Text = userData.Profile.MSN;
			YIM.Text = userData.Profile.YIM;
			AIM.Text = userData.Profile.AIM;
			ICQ.Text = userData.Profile.ICQ;
			Skype.Text = userData.Profile.Skype;
			PMNotificationEnabled.Checked = userData.PMNotification;
			Gender.SelectedIndex = userData.Profile.Gender;

			ListItem timeZoneItem = TimeZones.Items.FindByValue( userData.TimeZone.ToString() );
			if ( timeZoneItem != null ) timeZoneItem.Selected = true;

			OverrideForumThemeRow.Visible = PageContext.BoardSettings.AllowUserTheme;

			if ( PageContext.BoardSettings.AllowUserTheme )
			{
				// Allows to use different per-forum themes,
				// While "Allow User Change Theme" option in hostsettings is true
				string themeFile = PageContext.BoardSettings.Theme;
				if ( userData.ThemeFile != null ) themeFile = userData.ThemeFile;
				
				ListItem themeItem = Theme.Items.FindByValue( themeFile );
				if (themeItem != null) themeItem.Selected = true;

				OverrideDefaultThemes.Checked = userData.OverrideDefaultThemes;
			}

			if ( PageContext.BoardSettings.AllowUserLanguage )
			{
				string languageFile = PageContext.BoardSettings.Language;
				if ( userData.LanguageFile != string.Empty ) languageFile = userData.LanguageFile;

				ListItem foundItem = Language.Items.FindByValue( languageFile );
				if ( foundItem != null ) foundItem.Selected = true;
			}
		}

		protected void UpdateProfile_Click( object sender, System.EventArgs e )
		{
			if ( HomePage.Text.Length > 0 && !HomePage.Text.StartsWith( "http://" ) )
				HomePage.Text = "http://" + HomePage.Text;

			if ( MSN.Text.Length > 0 && !General.IsValidEmail( MSN.Text ) )
			{
				PageContext.AddLoadMessage( PageContext.Localization.GetText( "PROFILE", "BAD_MSN" ) );
				return;
			}
			if ( HomePage.Text.Length > 0 && !General.IsValidURL( HomePage.Text ) )
			{
				PageContext.AddLoadMessage( PageContext.Localization.GetText( "PROFILE", "BAD_HOME" ) );
				return;
			}
			if ( Weblog.Text.Length > 0 && !General.IsValidURL( Weblog.Text ) )
			{
				PageContext.AddLoadMessage( PageContext.Localization.GetText( "PROFILE", "BAD_WEBLOG" ) );
				return;
			}
			if ( ICQ.Text.Length > 0 && !General.IsValidInt( ICQ.Text ) )
			{
				PageContext.AddLoadMessage( PageContext.Localization.GetText( "PROFILE", "BAD_ICQ" ) );
				return;
			}

			if ( UpdateEmailFlag )
			{
				string newEmail = Email.Text.Trim();

				if ( !General.IsValidEmail( newEmail ) )
				{
					PageContext.AddLoadMessage( PageContext.Localization.GetText( "PROFILE", "BAD_EMAIL" ) );
					return;
				}

				if ( PageContext.BoardSettings.EmailVerification )
				{
					string hashinput = DateTime.Now.ToString() + Email.Text + Security.CreatePassword( 20 );
					string hash = FormsAuthentication.HashPasswordForStoringInConfigFile( hashinput, "md5" );

					// Create Email
					YafTemplateEmail changeEmail = new YafTemplateEmail( "CHANGEEMAIL" );

					changeEmail.TemplateParams ["{user}"] = PageContext.PageUserName;
					changeEmail.TemplateParams ["{link}"] = String.Format( "{1}{0}\r\n\r\n", YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped( YAF.Classes.Utils.ForumPages.approve, "k={0}", hash ), YafForumInfo.ServerURL );
					changeEmail.TemplateParams ["{newemail}"] = Email.Text;
					changeEmail.TemplateParams ["{key}"] = hash;
					changeEmail.TemplateParams ["{forumname}"] = PageContext.BoardSettings.Name;
					changeEmail.TemplateParams ["{forumlink}"] = YafForumInfo.ForumURL;

					// save a change email reference to the db
					YAF.Classes.Data.DB.checkemail_save( CurrentUserID, hash, newEmail );

					//  send a change email message...
					changeEmail.SendEmail( new System.Net.Mail.MailAddress( newEmail ), PageContext.Localization.GetText( "COMMON", "CHANGEEMAIL_SUBJECT" ), true );

					// show a confirmation
					PageContext.AddLoadMessage( String.Format( PageContext.Localization.GetText( "PROFILE", "mail_sent" ), Email.Text ) );
				}
				else
				{
					// just update the e-mail...
					UserMembershipHelper.UpdateEmail( CurrentUserID, Email.Text.Trim() );
				}
			}

			string userName = UserMembershipHelper.GetUserNameFromID( CurrentUserID );

			YafUserProfile userProfile = PageContext.GetProfile( userName );

			userProfile.Location = Location.Text.Trim();
			userProfile.Homepage = HomePage.Text.Trim();
			userProfile.MSN = MSN.Text.Trim();
			userProfile.YIM = YIM.Text.Trim();
			userProfile.AIM = AIM.Text.Trim();
			userProfile.ICQ = ICQ.Text.Trim();
			userProfile.Skype = Skype.Text.Trim();
			userProfile.RealName = Realname.Text.Trim();
			userProfile.Occupation = Occupation.Text.Trim();
			userProfile.Interests = Interests.Text.Trim();
			userProfile.Gender = Gender.SelectedIndex;
			userProfile.Blog = Weblog.Text.Trim();
			userProfile.BlogServiceUrl = WeblogUrl.Text.Trim();
			userProfile.BlogServiceUsername = WeblogUsername.Text.Trim();
			userProfile.BlogServicePassword = WeblogID.Text.Trim();

			userProfile.Save();

			// save remaining settings to the DB
			YAF.Classes.Data.DB.user_save( CurrentUserID, PageContext.PageBoardID, null, null,
				Convert.ToInt32( TimeZones.SelectedValue ), Language.SelectedValue, Theme.SelectedValue, OverrideDefaultThemes.Checked, null, PMNotificationEnabled.Checked );

			if ( !AdminEditMode )
			{
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.cp_profile );
			}
			else
			{
				BindData();
			}
		}

		protected void Cancel_Click( object sender, System.EventArgs e )
		{
			if ( AdminEditMode )
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_users );
			else
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.cp_profile );
		}

		protected void Email_TextChanged( object sender, System.EventArgs e )
		{
			UpdateEmailFlag = true;
		}

		protected bool UpdateEmailFlag
		{
			get { return ViewState ["bUpdateEmail"] != null ? Convert.ToBoolean( ViewState ["bUpdateEmail"] ) : false; }
			set { ViewState ["bUpdateEmail"] = value; }
		}

		public bool InAdminPages
		{
			get
			{
				return AdminEditMode;
			}
			set
			{
				AdminEditMode = value;
			}
		}

	}
}