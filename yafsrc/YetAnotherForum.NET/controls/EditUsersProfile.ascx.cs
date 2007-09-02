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
      if ( AdminEditMode && PageContext.IsAdmin && Request.QueryString ["u"] != null )
      {
        CurrentUserID = Convert.ToInt32( Request.QueryString ["u"] );
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

        BindData();
      }
    }
    private void BindData()
    {
      TimeZones.DataSource = yaf_StaticData.TimeZones();
      Theme.DataSource = yaf_StaticData.Themes();
      Theme.DataTextField = "Theme";
      Theme.DataValueField = "FileName";
      Language.DataSource = yaf_StaticData.Languages();
      Language.DataTextField = "Language";
      Language.DataValueField = "FileName";

      DataBind();

      // made the same mistake...
      MembershipUser user = UserMembershipHelper.GetMembershipUser( CurrentUserID );
      YAF_UserProfile userProfile = PageContext.GetProfile( UserMembershipHelper.GetUserNameFromID( CurrentUserID ) );

      Location.Text = userProfile.Location;
      HomePage.Text = userProfile.Homepage;
      TimeZones.Items.FindByValue( userProfile.TimeZone.ToString() ).Selected = true;
      Email.Text = user.Email;
      Realname.Text = userProfile.RealName;
      Occupation.Text = userProfile.Occupation;
      Interests.Text = userProfile.Interests;
      Weblog.Text = userProfile.Blog;
      WeblogUrl.Text = userProfile.BlogServiceUrl;
      WeblogID.Text = userProfile.BlogServicePassword;
      WeblogUsername.Text = userProfile.BlogServiceUsername;
      MSN.Text = userProfile.MSN;
      YIM.Text = userProfile.YIM;
      AIM.Text = userProfile.AIM;
      ICQ.Text = userProfile.ICQ;
      PMNotificationEnabled.Checked = userProfile.PMNotification;

      /*

      using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, CurrentUserID, true ) )
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
      WeblogUrl.Text = row ["WeblogUrl"].ToString();
      WeblogID.Text = row ["WeblogID"].ToString();
      WeblogUsername.Text = row ["WeblogUsername"].ToString();
      MSN.Text = row ["MSN"].ToString();
      YIM.Text = row ["YIM"].ToString();
      AIM.Text = row ["AIM"].ToString();
      ICQ.Text = row ["ICQ"].ToString();
      PMNotificationEnabled.Checked = Convert.ToBoolean( row ["PMNotification"] );
      */
      

      Gender.SelectedIndex = PageContext.Profile.Gender;

      OverrideForumThemeRow.Visible = PageContext.BoardSettings.AllowUserTheme;

      if ( PageContext.BoardSettings.AllowUserTheme )
      {
        // Allows to use different per-forum themes,
        // While "Allow User Change Theme" option in hostsettings is true
        string themeFile = PageContext.BoardSettings.Theme;
        if ( PageContext.Profile.ThemeFile != string.Empty ) themeFile = PageContext.Profile.ThemeFile;
        Theme.Items.FindByValue( themeFile ).Selected = true;
        OverrideDefaultThemes.Checked = PageContext.Profile.OverrideDefaultThemes;
      }

      if ( PageContext.BoardSettings.AllowUserLanguage )
      {
        string languageFile = PageContext.BoardSettings.Language;
        if ( PageContext.Profile.LanguageFile != string.Empty ) languageFile = PageContext.Profile.LanguageFile;
        Language.Items.FindByValue( languageFile ).Selected = true;
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
        if ( !General.IsValidEmail( Email.Text ) )
        {
          PageContext.AddLoadMessage( PageContext.Localization.GetText( "PROFILE", "BAD_EMAIL" ) );
          return;
        }

        if ( PageContext.BoardSettings.EmailVerification )
        {
          string hashinput = DateTime.Now.ToString() + Email.Text + Security.CreatePassword( 20 );
          string hash = FormsAuthentication.HashPasswordForStoringInConfigFile( hashinput, "md5" );

          // Email Body
          StringDictionary emailParameters = new StringDictionary();

          emailParameters ["{user}"] = PageContext.PageUserName;
          emailParameters ["{link}"] = String.Format( "{1}{0}\r\n\r\n", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.approve, "k={0}", hash ), yaf_ForumInfo.ServerURL );
          emailParameters ["{newemail}"] = Email.Text;
          emailParameters ["{key}"] = hash;
          emailParameters ["{forumname}"] = PageContext.BoardSettings.Name;
          emailParameters ["{forumlink}"] = yaf_ForumInfo.ForumURL;

          string message = General.CreateEmailFromTemplate( "changeemail.txt", ref emailParameters );

          YAF.Classes.Data.DB.checkemail_save( CurrentUserID, hash, Email.Text );

          //  Build a MailMessage
          General.SendMail( PageContext.BoardSettings.ForumEmail, Email.Text, "Changed email", message );
          PageContext.AddLoadMessage( String.Format( PageContext.Localization.GetText( "PROFILE", "mail_sent" ), Email.Text ) );
        }
      }
      else
      {
        // just update the e-mail...
        UserMembershipHelper.UpdateEmail( CurrentUserID, Email.Text.Trim() );
      }

      YAF_UserProfile userProfile = PageContext.GetProfile( UserMembershipHelper.GetUserNameFromID( CurrentUserID ) );

      userProfile.Location = Location.Text.Trim();
      userProfile.Homepage = HomePage.Text.Trim();
      userProfile.TimeZone = Convert.ToInt32( TimeZones.SelectedValue );
      userProfile.LanguageFile = Language.SelectedValue;
      userProfile.ThemeFile = Theme.SelectedValue;
      userProfile.OverrideDefaultThemes = OverrideDefaultThemes.Checked;
      userProfile.MSN = MSN.Text.Trim();
      userProfile.YIM = YIM.Text.Trim();
      userProfile.AIM = AIM.Text.Trim();
      userProfile.ICQ = ICQ.Text.Trim();
      userProfile.RealName = Realname.Text.Trim();
      userProfile.Occupation = Occupation.Text.Trim();
      userProfile.Interests = Interests.Text.Trim();
      userProfile.Gender = Gender.SelectedIndex;
      userProfile.Blog = Weblog.Text.Trim();
      userProfile.BlogServiceUrl = WeblogUrl.Text.Trim();
      userProfile.BlogServiceUsername = WeblogUsername.Text.Trim();
      userProfile.BlogServicePassword = WeblogID.Text.Trim();
      userProfile.PMNotification = PMNotificationEnabled.Checked;

      userProfile.Save();

      if ( AdminEditMode )
        YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_users );
      else
        YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.cp_profile );
    }

    protected void Cancel_Click( object sender, System.EventArgs e )
    {
      if ( AdminEditMode )
        YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_users );
      else
        YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.cp_profile );
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