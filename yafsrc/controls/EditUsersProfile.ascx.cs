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
using yaf.pages;

namespace yaf.controls
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

				DeleteAvatar.Text = ForumPage.GetText( "PROFILE", "delete_avatar" );
				UpdateProfile.Text = ForumPage.GetText( "PROFILE", "Save" );
				OurAvatar.NavigateUrl = Forum.GetLink( Pages.avatar );
				OurAvatar.Text = ForumPage.GetText( "PROFILE", "OURAVATAR_SELECT" );

				ForumSettingsRows.Visible = ForumPage.BoardSettings.AllowUserTheme || ForumPage.BoardSettings.AllowUserLanguage || ForumPage.BoardSettings.AllowPMEmailNotification;
				UserThemeRow.Visible = ForumPage.BoardSettings.AllowUserTheme;
				UserLanguageRow.Visible = ForumPage.BoardSettings.AllowUserLanguage;
				PMNotificationRow.Visible = ForumPage.BoardSettings.AllowPMEmailNotification;

				if ( Request.QueryString ["av"] != null )
				{
					AvatarImg.ImageUrl = string.Format( "{2}{0}images/avatars/{1}", Data.ForumRoot, Request.QueryString ["av"], ForumPage.ServerURL );
					AvatarImg.Visible = true;
					Avatar.Text = AvatarImg.ImageUrl;
					// OurAvatar.Visible = false;
				}

				if ( DeleteAvatar.Visible == true )
				{
					AvatarImg.Visible = false;
					Avatar.Text = "";
				}		
		
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

			using ( DataTable dt = DB.user_list( ForumPage.PageBoardID, CurrentUserID, true ) )
			{
				row = dt.Rows [0];
			}

			Location.Text = row ["Location"].ToString();
			HomePage.Text = row ["HomePage"].ToString();
			TimeZones.Items.FindByValue( row ["TimeZone"].ToString() ).Selected = true;
			Avatar.Text = row ["Avatar"].ToString();
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
			}

			if ( ForumPage.BoardSettings.AllowUserLanguage )
			{
				string languageFile = ForumPage.BoardSettings.Language;
				if ( !row.IsNull( "LanguageFile" ) ) languageFile = Convert.ToString( row ["LanguageFile"] );					
				Language.Items.FindByValue( languageFile ).Selected = true;
			}

			AvatarDeleteRow.Visible = row ["AvatarImage"].ToString().Length > 0;
			AvatarImg.Visible = row ["Avatar"].ToString().Length > 0;
			AvatarImg.ImageUrl = row ["Avatar"].ToString();

			AvatarUploadRow.Visible = ForumPage.BoardSettings.AvatarUpload;
			AvatarRemoteRow.Visible = ForumPage.BoardSettings.AvatarRemote;

			AvatarRow.Visible = AvatarOurs.Visible || AvatarUploadRow.Visible || AvatarRemoteRow.Visible || AvatarDeleteRow.Visible;
		}

		protected void DeleteAvatar_Click( object sender, System.EventArgs e )
		{
			DB.user_deleteavatar( CurrentUserID );
			BindData();
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

			if ( File.PostedFile != null && File.PostedFile.FileName.Trim().Length > 0 && File.PostedFile.ContentLength > 0 )
			{
				long x = ForumPage.BoardSettings.AvatarWidth;
				long y = ForumPage.BoardSettings.AvatarHeight;
				int nAvatarSize = ForumPage.BoardSettings.AvatarSize;

				System.IO.Stream resized = null;

				using ( System.Drawing.Image img = System.Drawing.Image.FromStream( File.PostedFile.InputStream ) )
				{
					if ( img.Width > x || img.Height > y )
					{
						ForumPage.AddLoadMessage( String.Format( ForumPage.GetText( "PROFILE", "WARN_TOOBIG" ), x, y ) );
						ForumPage.AddLoadMessage( String.Format( ForumPage.GetText( "PROFILE", "WARN_SIZE" ), img.Width, img.Height ) );
						ForumPage.AddLoadMessage( ForumPage.GetText( "PROFILE", "WARN_RESIZED" ) );

						double newWidth = img.Width;
						double newHeight = img.Height;
						if ( newWidth > x )
						{
							newHeight = newHeight * x / newWidth;
							newWidth = x;
						}
						if ( newHeight > y )
						{
							newWidth = newWidth * y / newHeight;
							newHeight = y;
						}

						using ( System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap( img, new System.Drawing.Size( ( int ) newWidth, ( int ) newHeight ) ) )
						{
							resized = new System.IO.MemoryStream();
							bitmap.Save( resized, System.Drawing.Imaging.ImageFormat.Jpeg );
						}
					}
					if ( nAvatarSize > 0 && File.PostedFile.ContentLength >= nAvatarSize && resized == null )
					{
						ForumPage.AddLoadMessage( String.Format( ForumPage.GetText( "PROFILE", "WARN_BIGFILE" ), nAvatarSize ) );
						ForumPage.AddLoadMessage( String.Format( ForumPage.GetText( "PROFILE", "WARN_FILESIZE" ), File.PostedFile.ContentLength ) );
						return;
					}

					if ( resized == null )
						DB.user_saveavatar( CurrentUserID, File.PostedFile.InputStream );
					else
						DB.user_saveavatar( CurrentUserID, resized );
				}
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
					emailParameters ["{link}"] = String.Format( "{1}{0}\r\n\r\n", Forum.GetLink( Pages.approve, "k={0}", hash ), ForumPage.ServerURL );
					emailParameters ["{newemail}"] = Email.Text;
					emailParameters ["{key}"] = hash;
					emailParameters ["{forumname}"] = ForumPage.BoardSettings.Name;
					emailParameters ["{forumlink}"] = ForumPage.ForumURL;

					string message = Utils.CreateEmailFromTemplate( "changeemail.txt", ref emailParameters );

					DB.checkemail_save( CurrentUserID, hash, Email.Text );
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
					DB.user_savepassword( CurrentUserID, newpw );
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

					if ( !DB.user_changepassword( CurrentUserID, oldpw, newpw ) )
					{
						ForumPage.AddLoadMessage( ForumPage.GetText( "PROFILE", "old_password_wrong" ) );
					}
				}
			}

			object email = null;
			if ( !ForumPage.BoardSettings.EmailVerification )
				email = Email.Text;

			DB.user_save( CurrentUserID, ForumPage.PageBoardID, null, null, email, null, Location.Text, HomePage.Text, TimeZones.SelectedValue, Avatar.Text, Language.SelectedValue, Theme.SelectedValue, null,
					MSN.Text, YIM.Text, AIM.Text, ICQ.Text, Realname.Text, Occupation.Text, Interests.Text, Gender.SelectedIndex, Weblog.Text, PMNotificationEnabled.Checked );

			if ( AdminEditMode )
				Forum.Redirect( Pages.admin_users );
			else
				Forum.Redirect( Pages.cp_profile );
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