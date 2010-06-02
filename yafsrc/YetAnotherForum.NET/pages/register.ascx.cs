/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Pages
{
  // YAF.Pages
  #region Using

  using System;
  using System.Net.Mail;
  using System.Web.Security;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.UI;
  using YAF.Classes.Utils;
  using YAF.Controls;

  #endregion

  /// <summary>
  /// Summary description for register.
  /// </summary>
  public partial class register : ForumPage
  {
    #region Constants and Fields

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
    ///   Gets a value indicating whether RecaptchaControl.
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
    protected void CreateUserWizard1_ContinueButtonClick(object sender, EventArgs e)
    {
      // vzrus: to clear the cache to show user in the list at once
        this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.UsersOnlineStatus));
        this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.BoardStats));
        

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
    protected void CreateUserWizard1_CreateUserError(object sender, CreateUserErrorEventArgs e)
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
    protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
    {
      MembershipUser user = UserMembershipHelper.GetUser(this.CreateUserWizard1.UserName);

      // setup inital roles (if any) for this user
      RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, this.CreateUserWizard1.UserName);

      string displayName = user.UserName;

      if (this.PageContext.BoardSettings.EnableDisplayName)
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
      if (this.PageContext.BoardSettings.EmailVerification)
      {
        // get the user email
        this.SendVerificationEmail(user, userID);
      }

      if (!String.IsNullOrEmpty(this.PageContext.BoardSettings.NotificationOnUserRegisterEmailList))
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
    protected void CreateUserWizard1_CreatingUser(object sender, LoginCancelEventArgs e)
    {
      // trim username on postback
      string userName = this.CreateUserWizard1.UserName = this.CreateUserWizard1.UserName.Trim();

      // username cannot contain semi-colon or to be a bad word
      bool badWord =
        YafServices.BadWordReplace.ReplaceItems.Exists(
          i => userName.Equals(i.BadWord, StringComparison.CurrentCultureIgnoreCase));

      if (userName.Contains(";") || badWord || userName.ToLower().Equals(UserMembershipHelper.GuestUserName.ToLower()))
      {
        this.PageContext.AddLoadMessage(this.GetText("BAD_USERNAME"));
        e.Cancel = true;
        return;
      }

      if (userName.Length > this.PageContext.BoardSettings.UserNameMaxLength)
      {
        this.PageContext.AddLoadMessage(
          this.GetTextFormatted("USERNAME_TOOLONG", this.PageContext.BoardSettings.UserNameMaxLength));
        e.Cancel = true;
        return;
      }

      if (this.PageContext.BoardSettings.EnableDisplayName)
      {
        var displayName = this.CreateUserStepContainer.FindControlAs<TextBox>("DisplayName");

        if (displayName.Text.Length > this.PageContext.BoardSettings.UserNameMaxLength)
        {
          this.PageContext.AddLoadMessage(
            this.GetTextFormatted("USERNAME_TOOLONG", this.PageContext.BoardSettings.UserNameMaxLength));
          e.Cancel = true;
          return;
        }

        if (this.PageContext.UserDisplayName.GetId(displayName.Text.Trim()).HasValue)
        {
          this.PageContext.AddLoadMessage(this.GetText("ALREADY_REGISTERED_DISPLAYNAME"));
          e.Cancel = true;
        }
      }

      var yafCaptchaText = this.CreateUserStepContainer.FindControlAs<TextBox>("tbCaptcha");

      // vzrus: Here recaptcha should be always valid. This piece of code for testing only.
      if (this.PageContext.BoardSettings.CaptchaTypeRegister == 2)
      {
        var reCaptcha = (RecaptchaControl)this.CreateUserWizard1.FindWizardControlRecursive("Recaptcha1");
        if (!reCaptcha.IsValid)
        {
          this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"));
          e.Cancel = true;
        }
      }

      // verify captcha if enabled
      if (this.PageContext.BoardSettings.CaptchaTypeRegister == 1 &&
          this.Session["CaptchaImageText"].ToString() != yafCaptchaText.Text.Trim())
      {
        this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"));
        e.Cancel = true;
      }
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
    protected void CreateUserWizard1_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
      if (this.CreateUserWizard1.WizardSteps[e.CurrentStepIndex].ID == "profile")
      {
        // this is the "Profile Information" step. Save the data to their profile (+ defaults).
        var timeZones = (DropDownList)this.CreateUserWizard1.FindWizardControlRecursive("TimeZones");
        var locationTextBox = (TextBox)this.CreateUserWizard1.FindWizardControlRecursive("Location");
        var homepageTextBox = (TextBox)this.CreateUserWizard1.FindWizardControlRecursive("Homepage");

        MembershipUser user = UserMembershipHelper.GetUser(this.CreateUserWizard1.UserName);

        // setup/save the profile
        YafUserProfile userProfile = YafUserProfile.GetProfile(this.CreateUserWizard1.UserName);

        userProfile.Location = locationTextBox.Text.Trim();
        userProfile.Homepage = homepageTextBox.Text.Trim();

        userProfile.Save();

        // save the time zone...
        DB.user_save(
          UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey), 
          this.PageContext.PageBoardID, 
          null, 
          null, 
          null, 
          Convert.ToInt32(timeZones.SelectedValue), 
          null, 
          null, 
          null, 
          null, 
          null, 
          null, 
          null, 
          null, 
          null);
      }
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
    protected void CreateUserWizard1_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
    {
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
      // Check if secure connection only is allowed
      if (!this.Page.Request.IsSecureConnection & this.PageContext.BoardSettings.UseSSLToRegister)
      {
        YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
      }

      if (!this.IsPostBack)
      {
        this.CreateUserWizard1.MembershipProvider = Config.MembershipProvider;

        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("TITLE"));

        // handle the CreateUser Step localization
        this.SetupCreateUserStep();

        // handle other steps localization
        ((Button)this.CreateUserWizard1.FindWizardControlRecursive("ProfileNextButton")).Text = this.GetText("SAVE");
        ((Button)this.CreateUserWizard1.FindWizardControlRecursive("ContinueButton")).Text = this.GetText("CONTINUE");

        // get the time zone data source
        var timeZones = (DropDownList)this.CreateUserWizard1.FindWizardControlRecursive("TimeZones");
        timeZones.DataSource = StaticDataHelper.TimeZones();

        if (!this.PageContext.BoardSettings.EmailVerification)
        {
          // automatically log in created users
          this.CreateUserWizard1.LoginCreatedUser = true;
          this.CreateUserWizard1.DisableCreatedUser = false;

          // success notification localization
          ((Literal)this.CreateUserWizard1.FindWizardControlRecursive("AccountCreated")).Text =
            YafBBCode.MakeHtml(this.GetText("ACCOUNT_CREATED"), true, false);
        }
        else
        {
          this.CreateUserWizard1.LoginCreatedUser = false;
          this.CreateUserWizard1.DisableCreatedUser = true;

          // success notification localization
          ((Literal)this.CreateUserWizard1.FindWizardControlRecursive("AccountCreated")).Text =
            YafBBCode.MakeHtml(this.GetText("ACCOUNT_CREATED_VERIFICATION"), true, false);
        }

        this.CreateUserWizard1.FinishDestinationPageUrl = YafForumInfo.ForumURL;

        this.DataBind();

        timeZones.Items.FindByValue("0").Selected = true;
        this.CreateUserWizard1.FindWizardControlRecursive("UserName").Focus();
      }

      // password requirement parameters...
      var requirementText = (LocalizedLabel)this.CreateUserStepContainer.FindControl("LocalizedLabelRequirementsText");
      requirementText.Param0 = this.PageContext.CurrentMembership.MinRequiredPasswordLength.ToString();
      requirementText.Param1 = this.PageContext.CurrentMembership.MinRequiredNonAlphanumericCharacters.ToString();

      // max user name length
      var usernamelehgthText =
        (LocalizedLabel)this.CreateUserStepContainer.FindControl("LocalizedLabelLohgUserNameWarnText");
      usernamelehgthText.Param0 = this.PageContext.BoardSettings.UserNameMaxLength.ToString();

      if (this.PageContext.BoardSettings.CaptchaTypeRegister == 2)
      {
        this.SetupRecaptchaControl();
        
      }
    }

    /// <summary>
    /// The send registration notification email.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    private void SendRegistrationNotificationEmail(MembershipUser user)
    {
      string[] emails = this.PageContext.BoardSettings.NotificationOnUserRegisterEmailList.Split(';');

      var notifyAdmin = new YafTemplateEmail();

      string subject =
        String.Format(
          this.PageContext.Localization.GetText("COMMON", "NOTIFICATION_ON_USER_REGISTER_EMAIL_SUBJECT"), 
          this.PageContext.BoardSettings.Name);

      notifyAdmin.TemplateParams["{adminlink}"] = YafBuildLink.GetLinkNotEscaped(ForumPages.admin_admin, true);
      notifyAdmin.TemplateParams["{user}"] = user.UserName;
      notifyAdmin.TemplateParams["{email}"] = user.Email;
      notifyAdmin.TemplateParams["{forumname}"] = this.PageContext.BoardSettings.Name;

      string emailBody = notifyAdmin.ProcessTemplate("NOTIFICATION_ON_USER_REGISTER");

      foreach (string email in emails)
      {
        if (!String.IsNullOrEmpty(email.Trim()))
        {
          YafServices.SendMail.Queue(this.PageContext.BoardSettings.ForumEmail, email.Trim(), subject, emailBody);
        }
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
    private void SendVerificationEmail(MembershipUser user, int? userID)
    {
      var emailTextBox = (TextBox)this.CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Email");
      string email = emailTextBox.Text.Trim();

      string hashinput = DateTime.UtcNow + email + Security.CreatePassword(20);
      string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput, "md5");

      // save verification record...
      DB.checkemail_save(userID, hash, user.Email);

      var verifyEmail = new YafTemplateEmail("VERIFYEMAIL");

      string subject = this.GetTextFormatted("VERIFICATION_EMAIL_SUBJECT", this.PageContext.BoardSettings.Name);

      verifyEmail.TemplateParams["{link}"] = YafBuildLink.GetLinkNotEscaped(ForumPages.approve, true, "k={0}", hash);
      verifyEmail.TemplateParams["{key}"] = hash;
      verifyEmail.TemplateParams["{forumname}"] = this.PageContext.BoardSettings.Name;
      verifyEmail.TemplateParams["{forumlink}"] = String.Format("{0}", YafForumInfo.ForumURL);

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
      var questionRequired = (RequiredFieldValidator)this.CreateUserStepContainer.FindControl("QuestionRequired");
      var answerRequired = (RequiredFieldValidator)this.CreateUserStepContainer.FindControl("AnswerRequired");
      var createUser = (Button)this.CreateUserStepContainer.FindControl("StepNextButton");

      usernameRequired.ToolTip = usernameRequired.ErrorMessage = this.GetText("NEED_USERNAME");
      passwordRequired.ToolTip = passwordRequired.ErrorMessage = this.GetText("NEED_PASSWORD");
      confirmPasswordRequired.ToolTip = confirmPasswordRequired.ErrorMessage = this.GetText("RETYPE_PASSWORD");
      passwordNoMatch.ToolTip = passwordNoMatch.ErrorMessage = this.GetText("NEED_MATCH");
      emailRequired.ToolTip = emailRequired.ErrorMessage = this.GetText("NEED_EMAIL");
      questionRequired.ToolTip = questionRequired.ErrorMessage = this.GetText("NEED_QUESTION");
      answerRequired.ToolTip = answerRequired.ErrorMessage = this.GetText("NEED_ANSWER");
      createUser.ToolTip = createUser.Text = this.GetText("CREATE_USER");

      var captchaPlaceHolder = (PlaceHolder)this.CreateUserStepContainer.FindControl("YafCaptchaHolder");
      var recaptchaPlaceHolder = (PlaceHolder)this.CreateUserStepContainer.FindControl("RecaptchaPlaceHolder");

      if (this.PageContext.BoardSettings.CaptchaTypeRegister == 1)
      {
        this.Session["CaptchaImageText"] = CaptchaHelper.GetCaptchaString();
        var imgCaptcha = this.CreateUserStepContainer.FindControlAs<Image>("imgCaptcha");

        imgCaptcha.ImageUrl = String.Format("{0}resource.ashx?c=1", YafForumInfo.ForumClientFileRoot);

        captchaPlaceHolder.Visible = true;
      }
      else
      {
        captchaPlaceHolder.Visible = false;
      }

      recaptchaPlaceHolder.Visible = this.PageContext.BoardSettings.CaptchaTypeRegister == 2;

      this.SetupDisplayNameUI(this.CreateUserStepContainer, this.PageContext.BoardSettings.EnableDisplayName);

      var questionAnswerPlaceHolder = (PlaceHolder)this.CreateUserStepContainer.FindControl("QuestionAnswerPlaceHolder");
      questionAnswerPlaceHolder.Visible = this.PageContext.CurrentMembership.RequiresQuestionAndAnswer;
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
    private void SetupDisplayNameUI(Control startControl, bool enabled)
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
      if (string.IsNullOrEmpty(this.PageContext.BoardSettings.RecaptchaPrivateKey) ||
          string.IsNullOrEmpty(this.PageContext.BoardSettings.RecaptchaPrivateKey))
      {
        // this.PageContext.AddLoadMessage(this.GetText("RECAPTCHA_BADSETTING"));              
        DB.eventlog_create(this.PageContext.PageUserID, this, "Private or public key for Recapture required!");
        YafBuildLink.AccessDenied();
      }

      this.Recupt = new RecaptchaControl
        {
          ID = "Recaptcha1", 
          PrivateKey = this.PageContext.BoardSettings.RecaptchaPrivateKey, 
          PublicKey = this.PageContext.BoardSettings.RecaptchaPublicKey, 
          AllowMultipleInstances = this.PageContext.BoardSettings.RecaptureMultipleInstances, 
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