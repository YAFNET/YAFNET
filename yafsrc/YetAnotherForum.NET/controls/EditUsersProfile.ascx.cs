/* Yet Another Forum.NET
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Net.Mail;
  using System.Threading;
  using System.Web.Security;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The edit users profile.
  /// </summary>
  public partial class EditUsersProfile : BaseUserControl
  {
    #region Constants and Fields

    /// <summary>
    /// The _user data.
    /// </summary>
    private CombinedUserDataHelper _userData = null;

    /// <summary>
    /// The admin edit mode.
    /// </summary>
    private bool AdminEditMode = false;

    /// <summary>
    /// The current culture.
    /// </summary>
    private string currentCulture = "en-US";

    /// <summary>
    /// The current user id.
    /// </summary>
    private int CurrentUserID;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether InAdminPages.
    /// </summary>
    public bool InAdminPages
    {
      get
      {
        return this.AdminEditMode;
      }

      set
      {
        this.AdminEditMode = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether UpdateEmailFlag.
    /// </summary>
    protected bool UpdateEmailFlag
    {
      get
      {
        return this.ViewState["bUpdateEmail"] != null ? Convert.ToBoolean(this.ViewState["bUpdateEmail"]) : false;
      }

      set
      {
        this.ViewState["bUpdateEmail"] = value;
      }
    }

    /// <summary>
    /// Gets UserData.
    /// </summary>
    private CombinedUserDataHelper UserData
    {
      get
      {
        if (this._userData == null)
        {
          this._userData = new CombinedUserDataHelper(this.CurrentUserID);
        }

        return this._userData;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Cancel_Click(object sender, EventArgs e)
    {
      if (this.AdminEditMode)
      {
        YafBuildLink.Redirect(ForumPages.admin_users);
      }
      else
      {
        YafBuildLink.Redirect(ForumPages.cp_profile);
      }
    }

    /// <summary>
    /// The email_ text changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Email_TextChanged(object sender, EventArgs e)
    {
      this.UpdateEmailFlag = true;
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
      this.PageContext.QueryIDs = new QueryStringIDHelper("u");

      if (this.AdminEditMode && this.PageContext.IsAdmin && this.PageContext.QueryIDs.ContainsKey("u"))
      {
        this.CurrentUserID = (int)this.PageContext.QueryIDs["u"];
      }
      else
      {
        this.CurrentUserID = this.PageContext.PageUserID;
      }

      if (!this.IsPostBack)
      {
        this.LoginInfo.Visible = true;

        // Begin Modifications for enhanced profile
        this.Gender.Items.Add(this.PageContext.Localization.GetText("PROFILE", "gender0"));
        this.Gender.Items.Add(this.PageContext.Localization.GetText("PROFILE", "gender1"));
        this.Gender.Items.Add(this.PageContext.Localization.GetText("PROFILE", "gender2"));

        // End Modifications for enhanced profile				
        this.UpdateProfile.Text = this.PageContext.Localization.GetText("COMMON", "SAVE");
        this.Cancel.Text = this.PageContext.Localization.GetText("COMMON", "CANCEL");

        this.ForumSettingsRows.Visible = this.PageContext.BoardSettings.AllowUserTheme ||
                                         this.PageContext.BoardSettings.AllowUserLanguage ||
                                         this.PageContext.BoardSettings.AllowPMEmailNotification;

        this.UserThemeRow.Visible = this.PageContext.BoardSettings.AllowUserTheme;
        this.UserLanguageRow.Visible = this.PageContext.BoardSettings.AllowUserLanguage;
        this.MetaWeblogAPI.Visible = this.PageContext.BoardSettings.AllowPostToBlog;
        this.LoginInfo.Visible = this.PageContext.BoardSettings.AllowEmailChange;
        this.currentCulture = Thread.CurrentThread.CurrentCulture.IetfLanguageTag;
        this.DisplayNamePlaceholder.Visible = this.PageContext.BoardSettings.EnableDisplayName &&
                                              this.PageContext.BoardSettings.AllowDisplayNameModification;

        this.BindData();
      }
    }

    /// <summary>
    /// The update profile_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void UpdateProfile_Click(object sender, EventArgs e)
    {
      if (this.HomePage.Text.IsSet() && !this.HomePage.Text.StartsWith("http://"))
      {
        this.HomePage.Text = "http://" + this.HomePage.Text;
      }

      if (this.Weblog.Text.IsSet() && !this.Weblog.Text.StartsWith("http://"))
      {
        this.Weblog.Text = "http://" + this.Weblog.Text;
      }

      if (this.HomePage.Text.IsSet() && !ValidationHelper.IsValidURL(this.HomePage.Text))
      {
        this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("PROFILE", "BAD_HOME"));
        return;
      }

      if (this.Weblog.Text.IsSet() && !ValidationHelper.IsValidURL(this.Weblog.Text))
      {
        this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("PROFILE", "BAD_WEBLOG"));
        return;
      }

      if (this.MSN.Text.IsSet() && !ValidationHelper.IsValidEmail(this.MSN.Text))
      {
        this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("PROFILE", "BAD_MSN"));
        return;
      }

      if (this.ICQ.Text.IsSet() && !ValidationHelper.IsValidInt(this.ICQ.Text))
      {
        this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("PROFILE", "BAD_ICQ"));
        return;
      }

      string displayName = null;

      if (this.PageContext.BoardSettings.EnableDisplayName &&
          this.PageContext.BoardSettings.AllowDisplayNameModification)
      {
        if (this.DisplayName.Text.Trim().Length < 2)
        {
          this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("PROFILE", "INVALID_DISPLAYNAME"));
          return;
        }

        if (this.DisplayName.Text.Trim() != this.UserData.DisplayName)
        {
          if (this.PageContext.UserDisplayName.GetId(this.DisplayName.Text.Trim()).HasValue)
          {
            this.PageContext.AddLoadMessage(
              this.PageContext.Localization.GetText("REGISTER", "ALREADY_REGISTERED_DISPLAYNAME"));

            return;
          }

          displayName = this.DisplayName.Text.Trim();
        }
      }

      if (this.UpdateEmailFlag)
      {
        string newEmail = this.Email.Text.Trim();

        if (!ValidationHelper.IsValidEmail(newEmail))
        {
          this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("PROFILE", "BAD_EMAIL"));
          return;
        }

        if (this.PageContext.BoardSettings.EmailVerification)
        {
          this.SendEmailVerification(newEmail);
        }
        else
        {
          // just update the e-mail...
          UserMembershipHelper.UpdateEmail(this.CurrentUserID, this.Email.Text.Trim());
        }
      }

      string userName = UserMembershipHelper.GetUserNameFromID(this.CurrentUserID);

      this.UpdateUserProfile(userName);

      // vzrus: We should do it as we need to write null value to db, else it will be empty. 
      // Localizer currently treats only nulls. 
      object language = null;
      object culture = this.Culture.SelectedValue;
      object theme = this.Theme.SelectedValue;     

      if (string.IsNullOrEmpty(this.Theme.SelectedValue))
      {
        theme = null;
      }

      if (string.IsNullOrEmpty(this.Culture.SelectedValue))
      {
          culture = null;
      }
      else
      {
          foreach (System.Data.DataRow row in StaticDataHelper.Cultures().Rows)
          {
              if (culture.ToString() == row["CultureTag"].ToString())
              {
                  language = row["CultureFile"].ToString();
              }
          }
      }  

        
      // save remaining settings to the DB
      DB.user_save(
        this.CurrentUserID, 
        this.PageContext.PageBoardID, 
        null, 
        displayName, 
        null, 
        this.TimeZones.SelectedValue.ToType<int>(), 
        language, 
        culture,
        theme, 
        this.OverrideDefaultThemes.Checked, 
        null, 
        null, 
        null,
        this.DSTUser.Checked,
        this.HideMe.Checked,
        null);

      // clear the cache for this user...
      UserMembershipHelper.ClearCacheForUserId(this.CurrentUserID);

      // Clearing cache with old Active User Lazy Data ...
      this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ActiveUserLazyData.FormatWith(this.CurrentUserID)));

      if (!this.AdminEditMode)
      {
        YafBuildLink.Redirect(ForumPages.cp_profile);
      }
      else
      {
        this.BindData();
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.TimeZones.DataSource = StaticDataHelper.TimeZones();
      if (this.PageContext.BoardSettings.AllowUserTheme)
      {
        this.Theme.DataSource = StaticDataHelper.Themes();
        this.Theme.DataTextField = "Theme";
        this.Theme.DataValueField = "FileName";
      }

      if (this.PageContext.BoardSettings.AllowUserLanguage)
      {

        this.Culture.DataSource = StaticDataHelper.Cultures();
        this.Culture.DataValueField = "CultureTag";
        this.Culture.DataTextField = "CultureNativeName";
       
      }

      this.DataBind();

      if (this.PageContext.BoardSettings.EnableDNACalendar)
      {
        this.datePicker.LocID = this.PageContext.Localization.GetText("COMMON", "CAL_JQ_CULTURE");
        this.datePicker.AnotherFormatString = this.PageContext.Localization.GetText("COMMON", "CAL_JQ_CULTURE_DFORMAT");
        this.datePicker.DateFormatString = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;

        if (this.UserData.Profile.Birthday > DateTime.MinValue)
        {
          this.datePicker.Value = this.UserData.Profile.Birthday.Date;
        }
        else
        {
          this.datePicker.Value = DateTime.MinValue.Date;
        }

        this.datePicker.ToolTip = this.PageContext.Localization.GetText("COMMON", "CAL_JQ_TT");
        this.datePicker.DefaultDateString = string.Empty;
      }
      else
      {
        this.datePicker.Enabled = false;
        this.datePicker.Visible = false;
        this.BirthdayLabel.Visible = false;
      }

      this.DisplayName.Text = this.UserData.DisplayName;
      this.Location.Text = this.UserData.Profile.Location;
      this.HomePage.Text = this.UserData.Profile.Homepage;
      this.Email.Text = this.UserData.Email;
      this.Realname.Text = this.UserData.Profile.RealName;
      this.Occupation.Text = this.UserData.Profile.Occupation;
      this.Interests.Text = this.UserData.Profile.Interests;
      this.Weblog.Text = this.UserData.Profile.Blog;
      this.WeblogUrl.Text = this.UserData.Profile.BlogServiceUrl;
      this.WeblogID.Text = this.UserData.Profile.BlogServicePassword;
      this.WeblogUsername.Text = this.UserData.Profile.BlogServiceUsername;
      this.MSN.Text = this.UserData.Profile.MSN;
      this.YIM.Text = this.UserData.Profile.YIM;
      this.AIM.Text = this.UserData.Profile.AIM;
      this.ICQ.Text = this.UserData.Profile.ICQ;
      this.Xmpp.Text = this.UserData.Profile.XMPP;
      this.Skype.Text = this.UserData.Profile.Skype;
      this.Gender.SelectedIndex = this.UserData.Profile.Gender;

      ListItem timeZoneItem = this.TimeZones.Items.FindByValue(this.UserData.TimeZone.ToString());
      if (timeZoneItem != null)
      {
        timeZoneItem.Selected = true;
      }

      this.DSTUser.Checked = this.UserData.DSTUser;
      this.HideMe.Checked = this.UserData.IsActiveExcluded && (this.PageContext.BoardSettings.AllowUserHideHimself || this.PageContext.IsAdmin);
      this.OverrideForumThemeRow.Visible = this.PageContext.BoardSettings.AllowUserTheme;

      if (this.PageContext.BoardSettings.AllowUserTheme && this.Theme.Items.Count > 0)
      {
        // Allows to use different per-forum themes,
        // While "Allow User Change Theme" option in hostsettings is true
        string themeFile = this.PageContext.BoardSettings.Theme;
        if (!string.IsNullOrEmpty(this.UserData.ThemeFile))
        {
          themeFile = this.UserData.ThemeFile;
        }

        ListItem themeItem = this.Theme.Items.FindByValue(themeFile);
        if (themeItem != null)
        {
          themeItem.Selected = true;
        }

        this.OverrideDefaultThemes.Checked = this.UserData.OverrideDefaultThemes;
      }

      if (this.PageContext.BoardSettings.AllowUserLanguage && this.Culture.Items.Count > 0)
      {
        string languageFile = this.PageContext.BoardSettings.Language;
        string culture4tag = this.PageContext.BoardSettings.Culture;

        if (!string.IsNullOrEmpty(this.UserData.LanguageFile))
        {
            languageFile = this.UserData.LanguageFile;
        }

        if (!string.IsNullOrEmpty(this.UserData.CultureUser))
        {
            culture4tag = this.UserData.CultureUser;
        }      

        // Get first default full culture from a language file tag.
        string langFileCulture = StaticDataHelper.CultureDefaultFromFile(languageFile);

        // If 2-letter language code is the same we return Culture, else we return a default full culture from language file
        ListItem foundCultItem = this.Culture.Items.FindByValue((langFileCulture.Substring(0, 2) == culture4tag ? culture4tag : langFileCulture));
        if (foundCultItem != null)
        {
            foundCultItem.Selected = true;           
        }
         
      }
    }

    /// <summary>
    /// The send email verification.
    /// </summary>
    /// <param name="newEmail">
    /// The new email.
    /// </param>
    private void SendEmailVerification(string newEmail)
    {
      string hashinput = DateTime.UtcNow.ToString() + this.Email.Text + Security.CreatePassword(20);
      string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput, "md5");

      // Create Email
      var changeEmail = new YafTemplateEmail("CHANGEEMAIL");

      changeEmail.TemplateParams["{user}"] = this.PageContext.PageUserName;
      changeEmail.TemplateParams["{link}"] = "{0}\r\n\r\n".FormatWith(YafBuildLink.GetLinkNotEscaped(ForumPages.approve, true, "k={0}", hash));
      changeEmail.TemplateParams["{newemail}"] = this.Email.Text;
      changeEmail.TemplateParams["{key}"] = hash;
      changeEmail.TemplateParams["{forumname}"] = this.PageContext.BoardSettings.Name;
      changeEmail.TemplateParams["{forumlink}"] = YafForumInfo.ForumURL;

      // save a change email reference to the db
      DB.checkemail_save(this.CurrentUserID, hash, newEmail);

      // send a change email message...
      changeEmail.SendEmail(
        new MailAddress(newEmail), this.PageContext.Localization.GetText("COMMON", "CHANGEEMAIL_SUBJECT"), true);

      // show a confirmation
      this.PageContext.AddLoadMessage(
        this.PageContext.Localization.GetText("PROFILE", "mail_sent").FormatWith(this.Email.Text));
    }

    /// <summary>
    /// The update user profile.
    /// </summary>
    /// <param name="userName">
    /// The user name.
    /// </param>
    private void UpdateUserProfile(string userName)
    {
      YafUserProfile userProfile = YafUserProfile.GetProfile(userName);

      userProfile.Location = this.Location.Text.Trim();
      userProfile.Homepage = this.HomePage.Text.Trim();
      userProfile.MSN = this.MSN.Text.Trim();
      userProfile.YIM = this.YIM.Text.Trim();
      userProfile.AIM = this.AIM.Text.Trim();
      userProfile.ICQ = this.ICQ.Text.Trim();
      userProfile.XMPP = this.Xmpp.Text.Trim();
      userProfile.Skype = this.Skype.Text.Trim();
      userProfile.RealName = this.Realname.Text.Trim();
      userProfile.Occupation = this.Occupation.Text.Trim();
      userProfile.Interests = this.Interests.Text.Trim();
      userProfile.Gender = this.Gender.SelectedIndex;
      userProfile.Blog = this.Weblog.Text.Trim();

      if (this.PageContext.BoardSettings.EnableDNACalendar && this.datePicker.Value > DateTime.MinValue.Date)
      { 
        userProfile.Birthday = this.datePicker.Value.ToUniversalTime();
      }

      userProfile.BlogServiceUrl = this.WeblogUrl.Text.Trim();
      userProfile.BlogServiceUsername = this.WeblogUsername.Text.Trim();
      userProfile.BlogServicePassword = this.WeblogID.Text.Trim();

      userProfile.Save();
    }

    #endregion
  }
}