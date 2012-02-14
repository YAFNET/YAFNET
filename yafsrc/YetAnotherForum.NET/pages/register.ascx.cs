﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
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
  #region Using

    using System;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Classes.Pattern;
    using YAF.Controls;
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
  /// The User Register Page.
  /// </summary>
  public partial class register : ForumPage
  {
    #region Constants and Fields

    /// <summary>
    ///   Gets User IP Info.
    /// </summary>
    public ThreadSafeDictionary<string,string> _UserIpLocator;

    /// <summary>
    ///   The recPH.
    /// </summary>
    private PlaceHolder recPH;
   
    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "register" /> class.
    /// </summary>
    public register()
      : base("REGISTER")
    {
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    ///   Gets CreateUserStepContainer.
    /// </summary>
    private Control CreateUserStepContainer
    {
      get
      {
        return this.CreateUserWizard1.CreateUserStep.ContentTemplateContainer;
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether RecaptchaControl.
    /// </summary>
    private RecaptchaControl Recupt { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// The create user wizard 1_ continue button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void CreateUserWizard1_ContinueButtonClick([NotNull] object sender, [NotNull] EventArgs e)
    {
      // vzrus: to clear the cache to show user in the list at once
      this.Get<IDataCache>().Remove(Constants.Cache.UsersOnlineStatus);
      this.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);

      // redirect to the main forum URL      
      YafBuildLink.Redirect(ForumPages.forum);
    }

    /// <summary>
    /// The create user wizard 1_ create user error.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void CreateUserWizard1_CreateUserError([NotNull] object sender, [NotNull] CreateUserErrorEventArgs e)
    {
      string createUserError = string.Empty;

      // find the type of error
      switch (e.CreateUserError)
      {
        case MembershipCreateStatus.DuplicateEmail:
          createUserError = this.GetText("ALREADY_REGISTERED");
          break;
        case MembershipCreateStatus.DuplicateUserName:
          createUserError = this.GetText("ALREADY_REGISTERED");
          break;
        case MembershipCreateStatus.InvalidEmail:
          createUserError = this.GetText("BAD_EMAIL");
          break;
        case MembershipCreateStatus.InvalidPassword:
          createUserError = this.GetText("BAD_PASSWORD");
          break;
        case MembershipCreateStatus.InvalidQuestion:
          createUserError = this.GetText("INVALID_QUESTION");
          break;
        case MembershipCreateStatus.InvalidUserName:
          createUserError = this.GetText("INVALID_USERNAME");
          break;
        case MembershipCreateStatus.InvalidAnswer:
          createUserError = this.GetText("INVALID_ANSWER");
          break;
        case MembershipCreateStatus.InvalidProviderUserKey:
          createUserError = "Invalid provider user key.";
          break;
        case MembershipCreateStatus.DuplicateProviderUserKey:
          createUserError = "Duplicate provider user key.";
          break;
        case MembershipCreateStatus.ProviderError:
          createUserError = "Provider Error";
          break;
        case MembershipCreateStatus.UserRejected:
          createUserError = "User creation failed: Reason is defined by the provider.";
          break;
      }

      this.PageContext.AddLoadMessage(createUserError);

      // Display the failure message in a client-side alert box
      // Page.ClientScript.RegisterStartupScript(Page.GetType(), "CreateUserError", String.Format("alert('{0}');", createUserError.Replace("'", "\'")), true);
    }

    /// <summary>
    /// The create user wizard 1_ created user.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void CreateUserWizard1_CreatedUser([NotNull] object sender, [NotNull] EventArgs e)
    {
      MembershipUser user = UserMembershipHelper.GetUser(this.CreateUserWizard1.UserName);

      // setup inital roles (if any) for this user
      RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, this.CreateUserWizard1.UserName);

      string displayName = user.UserName;

      if (this.Get<YafBoardSettings>().EnableDisplayName)
      {
        displayName = this.CreateUserStepContainer.FindControlAs<TextBox>("DisplayName").Text.Trim();
      }

      // create the user in the YAF DB as well as sync roles...
      int? userID = RoleMembershipHelper.CreateForumUser(user, displayName, YafContext.Current.PageBoardID);

      // create empty profile just so they have one
      YafUserProfile userProfile = YafUserProfile.GetProfile(this.CreateUserWizard1.UserName);

      // setup their inital profile information
      userProfile.Save();

      if (userID == null)
      {
        // something is seriously wrong here -- redirect to failure...
        YafBuildLink.RedirectInfoPage(InfoMessage.Failure);
      }

      // handle e-mail verification if needed
      if (this.Get<YafBoardSettings>().EmailVerification)
      {
        // get the user email
        this.SendVerificationEmail(user, userID);
      }

      if (this.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
      {
        // send user register notification to the following admin users...
        this.SendRegistrationNotificationEmail(user);
      }
    }

    /// <summary>
    /// The create user wizard 1_ creating user.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Argument is null.
    /// </exception>
    protected void CreateUserWizard1_CreatingUser([NotNull] object sender, [NotNull] LoginCancelEventArgs e)
    {
      string userName = this.CreateUserWizard1.UserName;

      if (userName.IsNotSet())
      {
        throw new ArgumentNullException("CreateUserWizard.UserName", "UserName from CreateUserWizard is Null!");
      }

        userName = userName.Trim();

        // trim username on postback
      this.CreateUserWizard1.UserName = userName;

      // username cannot contain semi-colon or to be a bad word
      bool badWord =
        this.Get<IBadWordReplace>().ReplaceItems.Any(
          i => userName.Equals(i.BadWord, StringComparison.CurrentCultureIgnoreCase));

      string guestUserName = UserMembershipHelper.GuestUserName;

      guestUserName = guestUserName.IsSet() ? guestUserName.ToLower() : string.Empty;

      if (userName.Contains(";") || badWord || userName.ToLower().Equals(guestUserName))
      {
        this.PageContext.AddLoadMessage(this.GetText("BAD_USERNAME"));
        e.Cancel = true;
        return;
      }

      if (userName.Length > this.Get<YafBoardSettings>().UserNameMaxLength)
      {
        this.PageContext.AddLoadMessage(
          this.GetTextFormatted("USERNAME_TOOLONG", this.Get<YafBoardSettings>().UserNameMaxLength));
        e.Cancel = true;
        return;
      }

      if (this.Get<YafBoardSettings>().EnableDisplayName)
      {
        var displayName = this.CreateUserStepContainer.FindControlAs<TextBox>("DisplayName");

        if (displayName != null)
        {
          if (displayName.Text.Length > this.Get<YafBoardSettings>().UserNameMaxLength)
          {
            this.PageContext.AddLoadMessage(
              this.GetTextFormatted("USERNAME_TOOLONG", this.Get<YafBoardSettings>().UserNameMaxLength));
            e.Cancel = true;
            return;
          }

          if (this.Get<IUserDisplayName>().GetId(displayName.Text.Trim()).HasValue)
          {
            this.PageContext.AddLoadMessage(this.GetText("ALREADY_REGISTERED_DISPLAYNAME"));
            e.Cancel = true;
          }
        }
      }

      var yafCaptchaText = this.CreateUserStepContainer.FindControlAs<TextBox>("tbCaptcha");

      // vzrus: Here recaptcha should be always valid. This piece of code for testing only.
      if (this.Get<YafBoardSettings>().CaptchaTypeRegister == 2)
      {
        var recaptcha = this.CreateUserWizard1.FindWizardControlRecursive("Recaptcha1").ToClass<RecaptchaControl>();

        if (recaptcha != null && !recaptcha.IsValid)
        {
          this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"));
          e.Cancel = true;
        }
      }

        // verify captcha if enabled
        if (this.Get<YafBoardSettings>().CaptchaTypeRegister != 1 || CaptchaHelper.IsValid(yafCaptchaText.Text.Trim()))
        {
            return;
        }

        this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"));
        e.Cancel = true;
    }

    /// <summary>
    /// The create user wizard 1_ next button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void CreateUserWizard1_NextButtonClick([NotNull] object sender, [NotNull] WizardNavigationEventArgs e)
    {
        if (this.CreateUserWizard1.WizardSteps[e.CurrentStepIndex].ID != "profile")
        {
            return;
        }

        // this is the "Profile Information" step. Save the data to their profile (+ defaults).
        var timeZones = (DropDownList)this.CreateUserWizard1.FindWizardControlRecursive("TimeZones");
        var country = (DropDownList)this.CreateUserWizard1.FindWizardControlRecursive("Country");
        var locationTextBox = (TextBox)this.CreateUserWizard1.FindWizardControlRecursive("Location");
        var homepageTextBox = (TextBox)this.CreateUserWizard1.FindWizardControlRecursive("Homepage");
        var dstUser = (CheckBox)this.CreateUserWizard1.FindWizardControlRecursive("DSTUser");

        MembershipUser user = UserMembershipHelper.GetUser(this.CreateUserWizard1.UserName);

        // setup/save the profile
        YafUserProfile userProfile = YafUserProfile.GetProfile(this.CreateUserWizard1.UserName);

        if (country.SelectedValue != null)
        {
            userProfile.Country = country.SelectedValue;
        }

        userProfile.Location = locationTextBox.Text.Trim();
        userProfile.Homepage = homepageTextBox.Text.Trim();

        userProfile.Save();

        // save the time zone...
        int userId = UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey);

        LegacyDb.user_save(
            userId, 
            this.PageContext.PageBoardID, 
            null, 
            null, 
            null, 
            timeZones.SelectedValue.ToType<int>(), 
            null, 
            null, 
            null,
            null,
            null,
            null, 
            null, 
            null, 
            null, 
            dstUser.Checked, 
            null, 
            null);

        bool autoWatchTopicsEnabled = this.Get<YafBoardSettings>().DefaultNotificationSetting ==
                                      UserNotificationSetting.TopicsIPostToOrSubscribeTo;

        // save the settings...
        LegacyDb.user_savenotification(
            userId, 
            true, 
            autoWatchTopicsEnabled, 
            this.Get<YafBoardSettings>().DefaultNotificationSetting, 
            this.Get<YafBoardSettings>().DefaultSendDigestEmail);

        // Clearing cache with old Active User Lazy Data ...
        this.Get<IDataCache>().Remove(Constants.Cache.ActiveUserLazyData.FormatWith(userId));
        this.Get<IDataCache>().Remove(Constants.Cache.ForumActiveDiscussions);
        this.Get<IRaiseEvent>().Raise(new NewUserRegisteredEvent(user, userId));
    }

    /// <summary>
    /// The create user wizard 1_ previous button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void CreateUserWizard1_PreviousButtonClick([NotNull] object sender, [NotNull] WizardNavigationEventArgs e)
    {
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

        YafContext.Current.PageElements.RegisterJsResourceInclude("msdropdown", "js/jquery.msDropDown.js");

        var country = (DropDownList)this.CreateUserWizard1.FindWizardControlRecursive("Country");

        YafContext.Current.PageElements.RegisterJsBlockStartup(
            "dropDownJs", JavaScriptBlocks.DropDownLoadJs(country.ClientID));

        YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.msDropDown.css");

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
      if (!this.IsPostBack)
      {
        this.CreateUserWizard1.MembershipProvider = Config.MembershipProvider;

        this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("TITLE"));

        // handle the CreateUser Step localization
        this.SetupCreateUserStep();

        // handle other steps localization
        ((Button)this.CreateUserWizard1.FindWizardControlRecursive("ProfileNextButton")).Text = this.GetText("SAVE");
        ((Button)this.CreateUserWizard1.FindWizardControlRecursive("ContinueButton")).Text = this.GetText("CONTINUE");

        // get the time zone data source
        var timeZones = (DropDownList)this.CreateUserWizard1.FindWizardControlRecursive("TimeZones");
        timeZones.DataSource = StaticDataHelper.TimeZones();

        // get the country data source
        var country = (DropDownList)this.CreateUserWizard1.FindWizardControlRecursive("Country");
        country.DataSource = StaticDataHelper.Country();

        if (this.Get<YafBoardSettings>().EnableIPInfoService && this._UserIpLocator == null)
        {
          // vzrus: we should always get not null class here
            this._UserIpLocator = new IPDetails().GetData(
                this.Get<HttpRequestBase>().UserHostAddress,
                "text",
                false,
                this.PageContext().CurrentForumPage.Localization.Culture.Name,
                string.Empty,
                string.Empty);
        }

        if (!this.Get<YafBoardSettings>().EmailVerification)
        {
          // automatically log in created users
          this.CreateUserWizard1.LoginCreatedUser = true;
          this.CreateUserWizard1.DisableCreatedUser = false;

          // success notification localization
          ((Literal)this.CreateUserWizard1.FindWizardControlRecursive("AccountCreated")).Text =
            this.Get<IBBCode>().MakeHtml(this.GetText("ACCOUNT_CREATED"), true, false);
        }
        else
        {
          this.CreateUserWizard1.LoginCreatedUser = false;
          this.CreateUserWizard1.DisableCreatedUser = true;

          // success notification localization
          ((Literal)this.CreateUserWizard1.FindWizardControlRecursive("AccountCreated")).Text =
            this.Get<IBBCode>().MakeHtml(this.GetText("ACCOUNT_CREATED_VERIFICATION"), true, false);
        }

        this.CreateUserWizard1.FinishDestinationPageUrl = YafForumInfo.ForumURL;

        this.DataBind();

        decimal hours = 0;

        // fill location field 
        // Trying to consume data about user IP whereabouts
        if (this.Get<YafBoardSettings>().EnableIPInfoService && this._UserIpLocator["StatusCode"] != "OK")
        {
            LegacyDb.eventlog_create(null, this.GetType().ToString(), "Geolocation Service reports: " + this._UserIpLocator["StatusMessage"], EventLogTypes.Information);
        }

        if (this.Get<YafBoardSettings>().EnableIPInfoService && this._UserIpLocator.Count > 0 && this._UserIpLocator["StatusCode"] == "OK")
        {
            var location = new StringBuilder();

            if (this._UserIpLocator["CountryName"].IsSet())
            {
                country.Items.FindByValue(this.Get<ILocalization>().Culture.Name.Substring(2, 2)).Selected = true;
            }

            if (this._UserIpLocator["RegionName"].IsSet())
            {
                location.AppendFormat(", {0}", this._UserIpLocator["RegionName"]);
            }

            if (this._UserIpLocator["CityName"].IsSet())
            {
                location.AppendFormat(", {0}", this._UserIpLocator["CityName"]);
            }

            this.CreateUserWizard1.FindControlRecursiveAs<TextBox>("Location").Text = location.ToString();
       
            if (this._UserIpLocator["TimeZone"].IsSet())
            {
                try
                {
                    hours = this._UserIpLocator["TimeZone"].ToType<decimal>() * 60;
                }
                catch (FormatException)
                {
                    hours = 0;
                }
            }
        }

        timeZones.Items.FindByValue(hours.ToString()).Selected = true;
        this.CreateUserWizard1.FindWizardControlRecursive("UserName").Focus();
      }

      // password requirement parameters...
      var requirementText = (LocalizedLabel)this.CreateUserStepContainer.FindControl("LocalizedLabelRequirementsText");
      requirementText.Param0 = this.Get<MembershipProvider>().MinRequiredPasswordLength.ToString();
      requirementText.Param1 = this.Get<MembershipProvider>().MinRequiredNonAlphanumericCharacters.ToString();

      // max user name length
      var usernamelehgthText =
        (LocalizedLabel)this.CreateUserStepContainer.FindControl("LocalizedLabelLohgUserNameWarnText");
      usernamelehgthText.Param0 = this.Get<YafBoardSettings>().UserNameMaxLength.ToString();

      if (this.Get<YafBoardSettings>().CaptchaTypeRegister == 2)
      {
        this.SetupRecaptchaControl();
      }
    }

    /// <summary>
    /// Refresh the Captcha Image
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    /// <exception cref="NotImplementedException">
    /// </exception>
    protected void refreshCaptcha_Click(object sender, EventArgs e)
    {
        var imgCaptcha = this.CreateUserStepContainer.FindControlAs<Image>("imgCaptcha");
        imgCaptcha.ImageUrl = "{0}resource.ashx?c=1&t=".FormatWith(YafForumInfo.ForumClientFileRoot, DateTime.UtcNow);
    }

    /// <summary>
    /// The send registration notification email.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    private void SendRegistrationNotificationEmail([NotNull] MembershipUser user)
    {
      string[] emails = this.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList.Split(';');

      var notifyAdmin = new YafTemplateEmail();

      string subject =
        this.GetText("COMMON", "NOTIFICATION_ON_USER_REGISTER_EMAIL_SUBJECT").FormatWith(
          this.Get<YafBoardSettings>().Name);

      notifyAdmin.TemplateParams["{adminlink}"] = YafBuildLink.GetLinkNotEscaped(ForumPages.admin_admin, true);
      notifyAdmin.TemplateParams["{user}"] = user.UserName;
      notifyAdmin.TemplateParams["{email}"] = user.Email;
      notifyAdmin.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;

      string emailBody = notifyAdmin.ProcessTemplate("NOTIFICATION_ON_USER_REGISTER");

      foreach (string email in emails.Where(email => email.Trim().IsSet()))
      {
          this.Get<ISendMail>().Queue(this.Get<YafBoardSettings>().ForumEmail, email.Trim(), subject, emailBody);
      }
    }

    /// <summary>
    /// The send verification email.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    private void SendVerificationEmail([NotNull] MembershipUser user, int? userID)
    {
      var emailTextBox = (TextBox)this.CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Email");
      string email = emailTextBox.Text.Trim();

      string hashinput = DateTime.UtcNow + email + Security.CreatePassword(20);
      string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput, "md5");

      // save verification record...
      LegacyDb.checkemail_save(userID, hash, user.Email);

      var verifyEmail = new YafTemplateEmail("VERIFYEMAIL");

      string subject = this.GetTextFormatted("VERIFICATION_EMAIL_SUBJECT", this.Get<YafBoardSettings>().Name);

      verifyEmail.TemplateParams["{link}"] = YafBuildLink.GetLinkNotEscaped(ForumPages.approve, true, "k={0}", hash);
      verifyEmail.TemplateParams["{key}"] = hash;
      verifyEmail.TemplateParams["{forumname}"] = this.Get<YafBoardSettings>().Name;
      verifyEmail.TemplateParams["{forumlink}"] = "{0}".FormatWith(YafForumInfo.ForumURL);

      verifyEmail.SendEmail(new MailAddress(email, user.UserName), subject, true);
    }

    /// <summary>
    /// The setup create user step.
    /// </summary>
    private void SetupCreateUserStep()
    {
      var passwordNoMatch = (CompareValidator)this.CreateUserStepContainer.FindControl("PasswordCompare");
      var usernameRequired = (RequiredFieldValidator)this.CreateUserStepContainer.FindControl("UserNameRequired");
      var passwordRequired = (RequiredFieldValidator)this.CreateUserStepContainer.FindControl("PasswordRequired");
      var confirmPasswordRequired =
        (RequiredFieldValidator)this.CreateUserStepContainer.FindControl("ConfirmPasswordRequired");
      var emailRequired = (RequiredFieldValidator)this.CreateUserStepContainer.FindControl("EmailRequired");
      var emailValid = (RegularExpressionValidator)this.CreateUserStepContainer.FindControl("EmailValid");

      var questionRequired = (RequiredFieldValidator)this.CreateUserStepContainer.FindControl("QuestionRequired");
      var answerRequired = (RequiredFieldValidator)this.CreateUserStepContainer.FindControl("AnswerRequired");
      var createUser = (Button)this.CreateUserStepContainer.FindControl("StepNextButton");

      usernameRequired.ToolTip = usernameRequired.ErrorMessage = this.GetText("NEED_USERNAME");
      passwordRequired.ToolTip = passwordRequired.ErrorMessage = this.GetText("NEED_PASSWORD");
      confirmPasswordRequired.ToolTip = confirmPasswordRequired.ErrorMessage = this.GetText("RETYPE_PASSWORD");
      passwordNoMatch.ToolTip = passwordNoMatch.ErrorMessage = this.GetText("NEED_MATCH");
      emailRequired.ToolTip = emailRequired.ErrorMessage = this.GetText("NEED_EMAIL");
      emailValid.ToolTip = emailValid.ErrorMessage = this.GetText("PROFILE", "BAD_EMAIL");
      questionRequired.ToolTip = questionRequired.ErrorMessage = this.GetText("NEED_QUESTION");
      answerRequired.ToolTip = answerRequired.ErrorMessage = this.GetText("NEED_ANSWER");
      createUser.ToolTip = createUser.Text = this.GetText("CREATE_USER");

      var captchaPlaceHolder = (PlaceHolder)this.CreateUserStepContainer.FindControl("YafCaptchaHolder");
      var recaptchaPlaceHolder = (PlaceHolder)this.CreateUserStepContainer.FindControl("RecaptchaPlaceHolder");

      if (this.Get<YafBoardSettings>().CaptchaTypeRegister == 1)
      {
        var imgCaptcha = this.CreateUserStepContainer.FindControlAs<Image>("imgCaptcha");

          imgCaptcha.ImageUrl = "{0}resource.ashx?c=1&t=".FormatWith(YafForumInfo.ForumClientFileRoot, DateTime.UtcNow);

          var refreshCaptcha = this.CreateUserStepContainer.FindControlAs<LinkButton>("RefreshCaptcha");

          refreshCaptcha.Text = this.GetText("GENERATE_CAPTCHA");

          refreshCaptcha.Click += this.refreshCaptcha_Click;

        captchaPlaceHolder.Visible = true;
      }
      else
      {
        captchaPlaceHolder.Visible = false;
      }

      recaptchaPlaceHolder.Visible = this.Get<YafBoardSettings>().CaptchaTypeRegister == 2;

      this.SetupDisplayNameUI(this.CreateUserStepContainer, this.Get<YafBoardSettings>().EnableDisplayName);

      var questionAnswerPlaceHolder = (PlaceHolder)this.CreateUserStepContainer.FindControl("QuestionAnswerPlaceHolder");
      questionAnswerPlaceHolder.Visible = this.Get<MembershipProvider>().RequiresQuestionAndAnswer;
    }      

    /// <summary>
    /// The setup display name ui.
    /// </summary>
    /// <param name="startControl">
    /// The start control.
    /// </param>
    /// <param name="enabled">
    /// The enabled.
    /// </param>
    private void SetupDisplayNameUI([NotNull] Control startControl, bool enabled)
    {
      startControl.FindControlAs<PlaceHolder>("DisplayNamePlaceHolder").Visible = enabled;
      startControl.FindControlAs<LocalizedRequiredFieldValidator>("DisplayNameRequired").Enabled = enabled;
    }

    /// <summary>
    /// The setup recaptcha control.
    /// </summary>
    private void SetupRecaptchaControl()
    {
      this.CreateUserWizard1.FindWizardControlRecursive("RecaptchaPlaceHolder").Visible = true;
      if (string.IsNullOrEmpty(this.Get<YafBoardSettings>().RecaptchaPrivateKey) ||
          string.IsNullOrEmpty(this.Get<YafBoardSettings>().RecaptchaPrivateKey))
      {
        // this.PageContext.AddLoadMessage(this.GetText("RECAPTCHA_BADSETTING"));              
        LegacyDb.eventlog_create(this.PageContext.PageUserID, this, "Private or public key for Recapture required!");
        YafBuildLink.AccessDenied();
      }

      this.Recupt = new RecaptchaControl
        {
          ID = "Recaptcha1", 
          PrivateKey = this.Get<YafBoardSettings>().RecaptchaPrivateKey, 
          PublicKey = this.Get<YafBoardSettings>().RecaptchaPublicKey, 
          AllowMultipleInstances = this.Get<YafBoardSettings>().RecaptureMultipleInstances, 
          Enabled = true, 
          EnableTheming = true, 
          // 'red' , 'white', 'blackglass' , 'clean' , 'custom'	
          Theme = "blackglass", 
          OverrideSecureMode = false
        };

      this.recPH = (PlaceHolder)this.CreateUserWizard1.FindWizardControlRecursive("RecaptchaControl");
      this.recPH.Controls.Add(this.Recupt);
      this.recPH.Visible = true;
    }

    #endregion
  }
}