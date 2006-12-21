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
using YAF.Pages;

namespace YAF.Controls
{
	public partial class EditUsersProfile : BaseUserControl
	{
		private int CurrentUserID;
		private bool AdminEditMode = false;

		protected void Page_Load( object sender, EventArgs e )
		{
			if ( AdminEditMode && ForumPage.IsAdmin && Request.QueryString ["u"] != null )
			{
				CurrentUserID = Convert.ToInt32( Request.QueryString ["u"] );
			}
			else
			{
				CurrentUserID = ForumPage.PageUserID;
			}

			if ( !IsPostBack )
			{
				if ( AdminEditMode )
				{
					ShowOldPassword.Visible = !AdminEditMode;
					LoginInfo.Visible = AdminEditMode;
				}
				else
				{
					LoginInfo.Visible = ForumPage.CanLogin ;
				}

				// Begin Modifications for enhanced profile
				Gender.Items.Add( ForumPage.GetText( "PROFILE", "gender0" ) );
				Gender.Items.Add( ForumPage.GetText( "PROFILE", "gender1" ) );
				Gender.Items.Add( ForumPage.GetText( "PROFILE", "gender2" ) );
				// End Modifications for enhanced profile				

				UpdateProfile.Text = ForumPage.GetText( "COMMON", "SAVE" );
				Cancel.Text = ForumPage.GetText( "COMMON", "CANCEL" );

				ForumSettingsRows.Visible = ForumPage.BoardSettings.AllowUserTheme || ForumPage.BoardSettings.AllowUserLanguage || ForumPage.BoardSettings.AllowPMEmailNotification;
				UserThemeRow.Visible = ForumPage.BoardSettings.AllowUserTheme;
				UserLanguageRow.Visible = ForumPage.BoardSettings.AllowUserLanguage;
				PMNotificationRow.Visible = ForumPage.BoardSettings.AllowPMEmailNotification;
		
				BindData();
			}
		}
		private void BindData()
		{
			DataRow row;
			TimeZones.DataSource = Data.TimeZones();
			Theme.DataSource = Data.Themes();
			Theme.DataTextField = "Theme";
			Theme.DataValueField = "FileName";
			Language.DataSource = Data.Languages();
			Language.DataTextField = "Language";
			Language.DataValueField = "FileName";

			DataBind();

			using ( DataTable dt = YAF.Classes.Data.DB.user_list( ForumPage.PageBoardID, CurrentUserID, true ) )
			{
				row = dt.Rows [0];
			}

			Location.Text = row ["Location"].ToString();
			HomePage.Text = row ["HomePage"].ToString();
			TimeZones.Items.FindByValue( row ["TimeZone"].ToString() ).Selected = true;
			Email.Text = row ["Email"].ToString();
			Realname.Text = row ["RealName"].ToString();
			Occupation.Text = row ["Occupation"].ToString();
			Interests.Text = row ["Interests"].ToString();
			Weblog.Text = row ["Weblog"].ToString();
			MSN.Text = row ["MSN"].ToString();
			YIM.Text = row ["YIM"].ToString();
			AIM.Text = row ["AIM"].ToString();
			ICQ.Text = row ["ICQ"].ToString();
			PMNotificationEnabled.Checked = Convert.ToBoolean( row ["PMNotification"] );

			Gender.SelectedIndex = Convert.ToInt32( row ["Gender"] );

			if ( ForumPage.BoardSettings.AllowUserTheme )  
			{
				string themeFile = ForumPage.BoardSettings.Theme;
				if ( !row.IsNull( "ThemeFile" ) ) themeFile = Convert.ToString( row ["ThemeFile"] );
				Theme.Items.FindByValue( themeFile ).Selected = true;
                // Allows to use different per-forum themes,
                // While "Allow User Change Theme" option in hostsettings is true
                OverrideDefaultThemes.Checked = Convert.ToBoolean(row["OverrideDefaultThemes"]);
			}

			if ( ForumPage.BoardSettings.AllowUserLanguage )
			{
				string languageFile = ForumPage.BoardSettings.Language;
				if ( !row.IsNull( "LanguageFile" ) ) languageFile = Convert.ToString( row ["LanguageFile"] );					
				Language.Items.FindByValue( languageFile ).Selected = true;
			}
		}

		protected void UpdateProfile_Click( object sender, System.EventArgs e )
		{
			if ( HomePage.Text.Length > 0 && !HomePage.Text.StartsWith( "http://" ) )
				HomePage.Text = "http://" + HomePage.Text;

			if ( MSN.Text.Length > 0 && !Utils.IsValidEmail( MSN.Text ) )
			{
				ForumPage.AddLoadMessage( ForumPage.GetText( "PROFILE", "BAD_MSN" ) );
				return;
			}
			if ( HomePage.Text.Length > 0 && !Utils.IsValidURL( HomePage.Text ) )
			{
				ForumPage.AddLoadMessage( ForumPage.GetText( "PROFILE", "BAD_HOME" ) );
				return;
			}
			if ( Weblog.Text.Length > 0 && !Utils.IsValidURL( Weblog.Text ) )
			{
				ForumPage.AddLoadMessage( ForumPage.GetText( "PROFILE", "BAD_WEBLOG" ) );
				return;
			}
			if ( ICQ.Text.Length > 0 && !Utils.IsValidInt( ICQ.Text ) )
			{
				ForumPage.AddLoadMessage( ForumPage.GetText( "PROFILE", "BAD_ICQ" ) );
						return;
					}

			if ( UpdateEmailFlag )
			{
				if ( !Utils.IsValidEmail( Email.Text ) )
				{
					ForumPage.AddLoadMessage( ForumPage.GetText( "PROFILE", "BAD_EMAIL" ) );
					return;
				}

				if ( ForumPage.BoardSettings.EmailVerification )
				{
					string hashinput = DateTime.Now.ToString() + Email.Text + register.CreatePassword( 20 );
					string hash = FormsAuthentication.HashPasswordForStoringInConfigFile( hashinput, "md5" );

					// Email Body
					StringDictionary emailParameters = new StringDictionary();

					emailParameters ["{user}"] = ForumPage.PageUserName;
					emailParameters ["{link}"] = String.Format( "{1}{0}\r\n\r\n", Forum.GetLink( ForumPages.approve, "k={0}", hash ), ForumPage.ServerURL );
					emailParameters ["{newemail}"] = Email.Text;
					emailParameters ["{key}"] = hash;
					emailParameters ["{forumname}"] = ForumPage.BoardSettings.Name;
					emailParameters ["{forumlink}"] = ForumPage.ForumURL;

					string message = Utils.CreateEmailFromTemplate( "changeemail.txt", ref emailParameters );

					YAF.Classes.Data.DB.checkemail_save( CurrentUserID, hash, Email.Text );
					//  Build a MailMessage
					Utils.SendMail( ForumPage, ForumPage.BoardSettings.ForumEmail, Email.Text, "Changed email", message );
					ForumPage.AddLoadMessage( String.Format( ForumPage.GetText( "PROFILE", "mail_sent" ), Email.Text ) );
				}
			}
			if ( AdminEditMode )
			{
				if ( ( NewPassword1.Text.Length > 0 ) )
				{
					if ( NewPassword1.Text.Length == 0 || NewPassword2.Text.Length == 0 )
					{
						ForumPage.AddLoadMessage( ForumPage.GetText( "PROFILE", "no_empty_password" ) );
						return;
					}
					if ( NewPassword1.Text != NewPassword2.Text )
					{
						ForumPage.AddLoadMessage( ForumPage.GetText( "PROFILE", "no_password_match" ) );
						return;
					}
					// No need to hash string as its hashed by procedure.
					string newpw = NewPassword1.Text;
					YAF.Classes.Data.DB.user_savepassword( CurrentUserID, newpw );
				}
			}
			else
			{
				if ( OldPassword.Text.Length > 0 )
				{
					if ( NewPassword1.Text.Length == 0 || NewPassword2.Text.Length == 0 )
					{
						ForumPage.AddLoadMessage( ForumPage.GetText( "PROFILE", "no_empty_password" ) );
						return;
					}
					if ( NewPassword1.Text != NewPassword2.Text )
					{
						ForumPage.AddLoadMessage( ForumPage.GetText( "PROFILE", "no_password_match" ) );
						return;
					}

					string oldpw = FormsAuthentication.HashPasswordForStoringInConfigFile( OldPassword.Text, "md5" );
					string newpw = FormsAuthentication.HashPasswordForStoringInConfigFile( NewPassword1.Text, "md5" );

					if ( !YAF.Classes.Data.DB.user_changepassword( CurrentUserID, oldpw, newpw ) )
					{
						ForumPage.AddLoadMessage( ForumPage.GetText( "PROFILE", "old_password_wrong" ) );
					}
				}
			}

			object email = null;
			if ( !ForumPage.BoardSettings.EmailVerification )
				email = Email.Text;

			YAF.Classes.Data.DB.user_save( CurrentUserID, ForumPage.PageBoardID, null, null, email, null, Location.Text, HomePage.Text, TimeZones.SelectedValue, null, Language.SelectedValue, Theme.SelectedValue,OverrideDefaultThemes.Checked, null,
					MSN.Text, YIM.Text, AIM.Text, ICQ.Text, Realname.Text, Occupation.Text, Interests.Text, Gender.SelectedIndex, Weblog.Text, PMNotificationEnabled.Checked );

			if ( AdminEditMode )
				Forum.Redirect( ForumPages.admin_users );
			else
				Forum.Redirect( ForumPages.cp_profile );
		}

		protected void Cancel_Click( object sender, System.EventArgs e )		
		{
			if ( AdminEditMode )
				Forum.Redirect( ForumPages.admin_users );
			else
				Forum.Redirect( ForumPages.cp_profile );
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