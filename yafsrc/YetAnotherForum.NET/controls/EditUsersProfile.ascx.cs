/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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
  using System.Data;
  using System.Linq;
  using System.Net.Mail;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Web;
  using System.Web.Security;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.EventProxies;
  using YAF.Types.Interfaces;
  using YAF.Utilities;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// The edit users profile.
  /// </summary>
  public partial class EditUsersProfile : BaseUserControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The admin edit mode.
    /// </summary>
    private bool AdminEditMode;

    /// <summary>
    ///   The current user id.
    /// </summary>
    private int CurrentUserID;

    /// <summary>
    ///   The _user data.
    /// </summary>
    private CombinedUserDataHelper _userData;

    /// <summary>
    ///   The current culture.
    /// </summary>
    private string currentCulture = "en-US";

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets a value indicating whether InAdminPages.
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
    ///   Gets or sets a value indicating whether UpdateEmailFlag.
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
    ///   Gets UserData.
    /// </summary>
    [NotNull]
    private CombinedUserDataHelper UserData
    {
      get
      {
        return this._userData ?? (this._userData = new CombinedUserDataHelper(this.CurrentUserID));
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
    protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBuildLink.Redirect(this.AdminEditMode ? ForumPages.admin_users : ForumPages.cp_profile);
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
    protected void Email_TextChanged([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.UpdateEmailFlag = true;
    }

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      // setup jQuery and DatePicker JS...
      YafContext.Current.PageElements.RegisterJQuery();
      YafContext.Current.PageElements.RegisterJQueryUI();

      if (!string.IsNullOrEmpty(this.GetText("COMMON", "CAL_JQ_CULTURE")))
      {
        var jqueryuiUrl = Config.JQueryUILangFile;

        if (!jqueryuiUrl.StartsWith("http"))
        {
          jqueryuiUrl = YafForumInfo.GetURLToResource(Config.JQueryUIFile);
        }

        YafContext.Current.PageElements.RegisterJsInclude("datepickerlang", jqueryuiUrl);
      }

      YafContext.Current.PageElements.RegisterJsBlockStartup(
        "DatePickerJs", 
        JavaScriptBlocks.DatePickerLoadJs(
          this.Birthday.ClientID, 
          Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern, 
          this.GetText("COMMON", "CAL_JQ_CULTURE_DFORMAT"), 
          this.GetText("COMMON", "CAL_JQ_CULTURE")));

      base.OnPreRender(e);
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
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.Page.Form.DefaultButton = this.UpdateProfile.UniqueID;

      this.PageContext.QueryIDs = new QueryStringIDHelper("u");

      if (this.AdminEditMode && this.PageContext.IsAdmin && this.PageContext.QueryIDs.ContainsKey("u"))
      {
        this.CurrentUserID = (int)this.PageContext.QueryIDs["u"];
      }
      else
      {
        this.CurrentUserID = this.PageContext.PageUserID;
      }

      if (this.IsPostBack)
      {
        return;
      }

      this.LoginInfo.Visible = true;

      // Begin Modifications for enhanced profile
      this.Gender.Items.Add(this.GetText("PROFILE", "gender0"));
      this.Gender.Items.Add(this.GetText("PROFILE", "gender1"));
      this.Gender.Items.Add(this.GetText("PROFILE", "gender2"));

      // End Modifications for enhanced profile
      this.UpdateProfile.Text = this.GetText("COMMON", "SAVE");
      this.Cancel.Text = this.GetText("COMMON", "CANCEL");

      this.ForumSettingsRows.Visible = this.Get<YafBoardSettings>().AllowUserTheme ||
                                       this.Get<YafBoardSettings>().AllowUserLanguage ||
                                       this.Get<YafBoardSettings>().AllowPMEmailNotification;

      this.UserThemeRow.Visible = this.Get<YafBoardSettings>().AllowUserTheme;
      this.TrTextEditors.Visible = this.Get<YafBoardSettings>().AllowUsersTextEditor;
      this.UserLanguageRow.Visible = this.Get<YafBoardSettings>().AllowUserLanguage;
      this.UserLoginRow.Visible = this.Get<YafBoardSettings>().AllowSingleSignOn;
      this.MetaWeblogAPI.Visible = this.Get<YafBoardSettings>().AllowPostToBlog;
      this.LoginInfo.Visible = this.Get<YafBoardSettings>().AllowEmailChange;
      this.currentCulture = Thread.CurrentThread.CurrentCulture.IetfLanguageTag;
      this.DisplayNamePlaceholder.Visible = this.Get<YafBoardSettings>().EnableDisplayName &&
                                            this.Get<YafBoardSettings>().AllowDisplayNameModification;

      this.BindData();
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
    protected void UpdateProfile_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this.HomePage.Text.IsSet())
      {
        // add http:// by default
        if (!Regex.IsMatch(this.HomePage.Text.Trim(), @"^(http|https|ftp|ftps|git|svn|news)\://.*"))
        {
          this.HomePage.Text = "http://{0}".FormatWith(this.HomePage.Text.Trim());
        }

        if (!ValidationHelper.IsValidURL(this.HomePage.Text))
        {
          this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_HOME"));
          return;
        }
      }

      if (this.Weblog.Text.IsSet() && !ValidationHelper.IsValidURL(this.Weblog.Text.Trim()))
      {
        this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_WEBLOG"));
        return;
      }

      if (this.MSN.Text.IsSet() && !ValidationHelper.IsValidEmail(this.MSN.Text))
      {
        this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_MSN"));
        return;
      }

      if (this.Xmpp.Text.IsSet() && !ValidationHelper.IsValidXmpp(this.Xmpp.Text))
      {
        this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_XMPP"));
        return;
      }

      if (this.ICQ.Text.IsSet() &&
          !(ValidationHelper.IsValidEmail(this.ICQ.Text) || ValidationHelper.IsValidInt(this.ICQ.Text)))
      {
        this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_ICQ"));
        return;
      }

      if (this.Facebook.Text.IsSet() && !ValidationHelper.IsValidInt(this.Facebook.Text))
      {
          this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_FACEBOOK"));
          return;
      }

      string displayName = null;

      if (this.Get<YafBoardSettings>().EnableDisplayName &&
          this.Get<YafBoardSettings>().AllowDisplayNameModification)
      {
        if (this.DisplayName.Text.Trim().Length < 2)
        {
          this.PageContext.AddLoadMessage(this.GetText("PROFILE", "INVALID_DISPLAYNAME"));
          return;
        }

        if (this.DisplayName.Text.Trim() != this.UserData.DisplayName)
        {
          if (this.Get<IUserDisplayName>().GetId(this.DisplayName.Text.Trim()).HasValue)
          {
            this.PageContext.AddLoadMessage(
              this.GetText("REGISTER", "ALREADY_REGISTERED_DISPLAYNAME"));

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
          this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_EMAIL"));
          return;
        }

        if (this.Get<YafBoardSettings>().EmailVerification)
        {
          this.SendEmailVerification(newEmail);
        }
        else
        {
          // just update the e-mail...
          try
          {
            UserMembershipHelper.UpdateEmail(this.CurrentUserID, this.Email.Text.Trim());
          }
          catch (ApplicationException)
          {
            this.PageContext.AddLoadMessage(this.GetText("PROFILE", "DUPLICATED_EMAIL"));

            return;
          }
        }
      }

      string userName = UserMembershipHelper.GetUserNameFromID(this.CurrentUserID);

      this.UpdateUserProfile(userName);

      // vzrus: We should do it as we need to write null value to db, else it will be empty. 
      // Localizer currently treats only nulls. 
      object language = null;
      object culture = this.Culture.SelectedValue;
      object theme = this.Theme.SelectedValue;
      object editor = this.ForumEditor.SelectedValue;

      if (string.IsNullOrEmpty(this.Theme.SelectedValue))
      {
        theme = null;
      }

      if (string.IsNullOrEmpty(this.ForumEditor.SelectedValue))
      {
          editor = null;
      }

      if (string.IsNullOrEmpty(this.Culture.SelectedValue))
      {
        culture = null;
      }
      else
      {
        foreach (DataRow row in
          StaticDataHelper.Cultures().Rows.Cast<DataRow>().Where(
            row => culture.ToString() == row["CultureTag"].ToString()))
        {
          language = row["CultureFile"].ToString();
        }
      }

      // save remaining settings to the DB
      LegacyDb.user_save(
        this.CurrentUserID, 
        this.PageContext.PageBoardID, 
        null, 
        displayName, 
        null, 
        this.TimeZones.SelectedValue.ToType<int>(), 
        language, 
        culture, 
        theme,
        this.SingleSignOn.Checked,
        editor,
        this.UseMobileTheme.Checked, 
        null, 
        null, 
        null, 
        this.DSTUser.Checked, 
        this.HideMe.Checked, 
        null);

        // vzrus: If it's a guest edited by an admin registry value should be changed
        DataTable dt = LegacyDb.user_list(this.PageContext.PageBoardID, this.CurrentUserID, true, null, null, false);

        if (dt.Rows.Count > 0 && dt.Rows[0]["IsGuest"].ToType<bool>())
        {
            LegacyDb.registry_save("timezone", this.TimeZones.SelectedValue, this.PageContext.PageBoardID);
        }

        // clear the cache for this user...)
      this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.CurrentUserID));

      YafContext.Current.Get<IDataCache>().Clear();

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
      if (this.Get<YafBoardSettings>().AllowUserTheme)
      {
        this.Theme.DataSource = StaticDataHelper.Themes();
        this.Theme.DataTextField = "Theme";
        this.Theme.DataValueField = "FileName";
      }

      if (this.Get<YafBoardSettings>().AllowUserLanguage)
      {
        this.Culture.DataSource = StaticDataHelper.Cultures();
        this.Culture.DataValueField = "CultureTag";
        this.Culture.DataTextField = "CultureNativeName";
      }

      if (this.Get<YafBoardSettings>().AllowUsersTextEditor)
      {
          this.ForumEditor.DataSource = this.Get<IModuleManager<ForumEditor>>().ActiveAsDataTable("Editors");
          this.ForumEditor.DataValueField = "Value";
          this.ForumEditor.DataTextField = "Name";
      }

        this.DataBind();

      if (this.Get<YafBoardSettings>().EnableDNACalendar)
      {
        this.Birthday.Text = this.UserData.Profile.Birthday > DateTime.MinValue
                               ? this.UserData.Profile.Birthday.Date.ToString(
                                 Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern)
                               : DateTime.MinValue.Date.ToString(
                                 Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern);

        this.Birthday.ToolTip = this.GetText("COMMON", "CAL_JQ_TT");
      }
      else
      {
        this.Birthday.Visible = false;
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
      this.Facebook.Text = this.UserData.Profile.Facebook;
      this.Twitter.Text = this.UserData.Profile.Twitter;
      this.Xmpp.Text = this.UserData.Profile.XMPP;
      this.Skype.Text = this.UserData.Profile.Skype;
      this.Gender.SelectedIndex = this.UserData.Profile.Gender;

      ListItem timeZoneItem = this.TimeZones.Items.FindByValue(this.UserData.TimeZone.ToString());
      if (timeZoneItem != null)
      {
        timeZoneItem.Selected = true;
      }

      this.DSTUser.Checked = this.UserData.DSTUser;
      this.HideMe.Checked = this.UserData.IsActiveExcluded &&
                            (this.Get<YafBoardSettings>().AllowUserHideHimself || this.PageContext.IsAdmin);

      if (this.Get<YafBoardSettings>().MobileTheme.IsSet() &&
          UserAgentHelper.IsMobileDevice(HttpContext.Current.Request.UserAgent) ||
          HttpContext.Current.Request.Browser.IsMobileDevice)
      {
          this.UseMobileThemeRow.Visible = true;
          this.UseMobileTheme.Checked = this.UserData.UseMobileTheme;
      }

        if (this.Get<YafBoardSettings>().AllowUserTheme && this.Theme.Items.Count > 0)
      {
        // Allows to use different per-forum themes,
        // While "Allow User Change Theme" option in hostsettings is true
        string themeFile = this.Get<YafBoardSettings>().Theme;

        if (!string.IsNullOrEmpty(this.UserData.ThemeFile))
        {
          themeFile = this.UserData.ThemeFile;
        }

        ListItem themeItem = this.Theme.Items.FindByValue(themeFile);
        if (themeItem != null)
        {
          themeItem.Selected = true;
        }
      }

      if (this.Get<YafBoardSettings>().AllowUsersTextEditor && this.ForumEditor.Items.Count > 0)
      {
          // Text editor
          string textEditor = !string.IsNullOrEmpty(this.UserData.TextEditor)
                                  ? this.UserData.TextEditor
                                  : this.Get<YafBoardSettings>().ForumEditor;

          ListItem editorItem = this.ForumEditor.Items.FindByValue(textEditor);
          if (editorItem != null)
          {
              editorItem.Selected = true;
          }
      }

      if (this.Get<YafBoardSettings>().AllowSingleSignOn)
      {
          this.SingleSignOn.Checked = this.UserData.UseSingleSignOn;
      }

      if (!this.Get<YafBoardSettings>().AllowUserLanguage || this.Culture.Items.Count <= 0)
      {
        return;
      }

      // Language and culture
      string languageFile = this.Get<YafBoardSettings>().Language;
      string culture4tag = this.Get<YafBoardSettings>().Culture;

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
      ListItem foundCultItem =
        this.Culture.Items.FindByValue(
          langFileCulture.Substring(0, 2) == culture4tag.Substring(0, 2) ? culture4tag : langFileCulture);
      if (foundCultItem != null)
      {
        foundCultItem.Selected = true;
      }
    }

    /// <summary>
    /// The send email verification.
    /// </summary>
    /// <param name="newEmail">
    /// The new email.
    /// </param>
    private void SendEmailVerification([NotNull] string newEmail)
    {
      string hashinput = DateTime.UtcNow + this.Email.Text + Security.CreatePassword(20);
      string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput, "md5");

      // Create Email
      var changeEmail = new YafTemplateEmail("CHANGEEMAIL");

      changeEmail.TemplateParams["{user}"] = this.PageContext.PageUserName;
      changeEmail.TemplateParams["{link}"] =
        "{0}\r\n\r\n".FormatWith(YafBuildLink.GetLinkNotEscaped(ForumPages.approve, true, "k={0}", hash));
      changeEmail.TemplateParams["{newemail}"] = this.Email.Text;
      changeEmail.TemplateParams["{key}"] = hash;
      changeEmail.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;
      changeEmail.TemplateParams["{forumlink}"] = YafForumInfo.ForumURL;

      // save a change email reference to the db
      LegacyDb.checkemail_save(this.CurrentUserID, hash, newEmail);

      // send a change email message...
      changeEmail.SendEmail(
        new MailAddress(newEmail), this.GetText("COMMON", "CHANGEEMAIL_SUBJECT"), true);

      // show a confirmation
      this.PageContext.AddLoadMessage(
        this.GetText("PROFILE", "mail_sent").FormatWith(this.Email.Text));
    }

    /// <summary>
    /// The update user profile.
    /// </summary>
    /// <param name="userName">
    /// The user name.
    /// </param>
    private void UpdateUserProfile([NotNull] string userName)
    {
      YafUserProfile userProfile = YafUserProfile.GetProfile(userName);

      userProfile.Location = this.Location.Text.Trim();
      userProfile.Homepage = this.HomePage.Text.Trim();
      userProfile.MSN = this.MSN.Text.Trim();
      userProfile.YIM = this.YIM.Text.Trim();
      userProfile.AIM = this.AIM.Text.Trim();
      userProfile.ICQ = this.ICQ.Text.Trim();
      userProfile.Facebook = this.Facebook.Text.Trim();
      userProfile.Twitter = this.Twitter.Text.Trim();
      userProfile.XMPP = this.Xmpp.Text.Trim();
      userProfile.Skype = this.Skype.Text.Trim();
      userProfile.RealName = this.Realname.Text.Trim();
      userProfile.Occupation = this.Occupation.Text.Trim();
      userProfile.Interests = this.Interests.Text.Trim();
      userProfile.Gender = this.Gender.SelectedIndex;
      userProfile.Blog = this.Weblog.Text.Trim();

      if (this.Get<YafBoardSettings>().EnableDNACalendar && this.Birthday.Text.IsSet())
      {
        DateTime userBirthdate;

        DateTime.TryParse(this.Birthday.Text, out userBirthdate);

        if (userBirthdate > DateTime.MinValue.Date)
        {
          userProfile.Birthday = userBirthdate;
        }
      }

      userProfile.BlogServiceUrl = this.WeblogUrl.Text.Trim();
      userProfile.BlogServiceUsername = this.WeblogUsername.Text.Trim();
      userProfile.BlogServicePassword = this.WeblogID.Text.Trim();

      userProfile.Save();
    }

    #endregion
  }
}