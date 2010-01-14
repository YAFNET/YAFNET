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

  /// <summary>
  /// Summary description for register.
  /// </summary>
  public partial class register : ForumPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="register"/> class.
    /// </summary>
    public register()
      : base("REGISTER")
    {
    }

    /// <summary>
    /// Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected
    {
      get
      {
        return false;
      }
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
      if (!IsPostBack)
      {
        this.CreateUserWizard1.MembershipProvider = Config.MembershipProvider;

        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(GetText("TITLE"));

        // handle the CreateUser Step localization
        Control createUserTemplateRef = this.CreateUserWizard1.CreateUserStep.ContentTemplateContainer;

        var passwordNoMatch = (CompareValidator) createUserTemplateRef.FindControl("PasswordCompare");
        var usernameRequired = (RequiredFieldValidator) createUserTemplateRef.FindControl("UserNameRequired");
        var passwordRequired = (RequiredFieldValidator) createUserTemplateRef.FindControl("PasswordRequired");
        var confirmPasswordRequired = (RequiredFieldValidator) createUserTemplateRef.FindControl("ConfirmPasswordRequired");
        var emailRequired = (RequiredFieldValidator) createUserTemplateRef.FindControl("EmailRequired");
        var questionRequired = (RequiredFieldValidator) createUserTemplateRef.FindControl("QuestionRequired");
        var answerRequired = (RequiredFieldValidator) createUserTemplateRef.FindControl("AnswerRequired");
        var createUser = (Button) createUserTemplateRef.FindControl("StepNextButton");

        usernameRequired.ToolTip = usernameRequired.ErrorMessage = GetText("NEED_USERNAME");
        passwordRequired.ToolTip = passwordRequired.ErrorMessage = GetText("NEED_PASSWORD");
        confirmPasswordRequired.ToolTip = confirmPasswordRequired.ErrorMessage = GetText("RETYPE_PASSWORD");
        passwordNoMatch.ToolTip = passwordNoMatch.ErrorMessage = GetText("NEED_MATCH");
        emailRequired.ToolTip = emailRequired.ErrorMessage = GetText("NEED_EMAIL");
        questionRequired.ToolTip = questionRequired.ErrorMessage = GetText("NEED_QUESTION");
        answerRequired.ToolTip = answerRequired.ErrorMessage = GetText("NEED_ANSWER");
        createUser.ToolTip = createUser.Text = GetText("CREATE_USER");

        // password requirement parameters...
        var requirementText = (LocalizedLabel) createUserTemplateRef.FindControl("LocalizedLabelRequirementsText");
        requirementText.Param0 = PageContext.CurrentMembership.MinRequiredPasswordLength.ToString();
        requirementText.Param1 = PageContext.CurrentMembership.MinRequiredNonAlphanumericCharacters.ToString();

        // handle other steps localization
        ((Button) ControlHelper.FindWizardControlRecursive(this.CreateUserWizard1, "ProfileNextButton")).Text = GetText("SAVE");
        ((Button) ControlHelper.FindWizardControlRecursive(this.CreateUserWizard1, "ContinueButton")).Text = GetText("CONTINUE");

        // get the time zone data source
        var timeZones = (DropDownList) ControlHelper.FindWizardControlRecursive(this.CreateUserWizard1, "TimeZones");
        timeZones.DataSource = StaticDataHelper.TimeZones();

        if (!PageContext.BoardSettings.EmailVerification)
        {
          // automatically log in created users
          this.CreateUserWizard1.LoginCreatedUser = true;
          this.CreateUserWizard1.DisableCreatedUser = false;

          // success notification localization
          ((Literal) ControlHelper.FindWizardControlRecursive(this.CreateUserWizard1, "AccountCreated")).Text = YafBBCode.MakeHtml(
            GetText("ACCOUNT_CREATED"), true, false);
        }
        else
        {
          this.CreateUserWizard1.LoginCreatedUser = false;
          this.CreateUserWizard1.DisableCreatedUser = true;

          // success notification localization
          ((Literal) ControlHelper.FindWizardControlRecursive(this.CreateUserWizard1, "AccountCreated")).Text =
            YafBBCode.MakeHtml(GetText("ACCOUNT_CREATED_VERIFICATION"), true, false);
        }

        if (PageContext.BoardSettings.EnableCaptchaForRegister)
        {
          Session["CaptchaImageText"] = CaptchaHelper.GetCaptchaString();
          var imgCaptcha = (Image) createUserTemplateRef.FindControl("imgCaptcha");
          var captchaPlaceHolder = (PlaceHolder) createUserTemplateRef.FindControl("CaptchaPlaceHolder");

          imgCaptcha.ImageUrl = String.Format("{0}resource.ashx?c=1", YafForumInfo.ForumRoot);
          captchaPlaceHolder.Visible = true;
        }

        var questionAnswerPlaceHolder = (PlaceHolder) createUserTemplateRef.FindControl("QuestionAnswerPlaceHolder");
        questionAnswerPlaceHolder.Visible = PageContext.CurrentMembership.RequiresQuestionAndAnswer;

        this.CreateUserWizard1.FinishDestinationPageUrl = YafForumInfo.ForumURL;

        DataBind();

        timeZones.Items.FindByValue("0").Selected = true;
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
        var timeZones = (DropDownList) ControlHelper.FindWizardControlRecursive(this.CreateUserWizard1, "TimeZones");
        var locationTextBox = (TextBox) ControlHelper.FindWizardControlRecursive(this.CreateUserWizard1, "Location");
        var homepageTextBox = (TextBox) ControlHelper.FindWizardControlRecursive(this.CreateUserWizard1, "Homepage");

        MembershipUser user = UserMembershipHelper.GetUser(this.CreateUserWizard1.UserName);

        // setup/save the profile
        YafUserProfile userProfile = YafUserProfile.GetProfile(this.CreateUserWizard1.UserName);

        userProfile.Location = locationTextBox.Text.Trim();
        userProfile.Homepage = homepageTextBox.Text.Trim();

        userProfile.Save();

        // save the time zone...
        DB.user_save(
          UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey), 
          PageContext.PageBoardID, 
          null, 
          null, 
          Convert.ToInt32(timeZones.SelectedValue), 
          null, 
          null, 
          null, 
          null, 
          null,
          null);
      }
    }

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
          createUserError = GetText("ALREADY_REGISTERED");
          break;
        case MembershipCreateStatus.DuplicateUserName:
          createUserError = GetText("ALREADY_REGISTERED");
          break;
        case MembershipCreateStatus.InvalidEmail:
          createUserError = GetText("BAD_EMAIL");
          break;
        case MembershipCreateStatus.InvalidPassword:
          createUserError = GetText("BAD_PASSWORD");
          break;
        case MembershipCreateStatus.InvalidQuestion:
          createUserError = GetText("INVALID_QUESTION");
          break;
        case MembershipCreateStatus.InvalidUserName:
          createUserError = GetText("INVALID_USERNAME");
          break;
        case MembershipCreateStatus.InvalidAnswer:
          createUserError = GetText("INVALID_ANSWER");
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

      PageContext.AddLoadMessage(createUserError);

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

      // create the user in the YAF DB as well as sync roles...
      int? userID = RoleMembershipHelper.CreateForumUser(user, YafContext.Current.PageBoardID);

      // create empty profile just so they have one
      YafUserProfile userProfile = YafUserProfile.GetProfile(this.CreateUserWizard1.UserName);

      // setup their inital profile information
      userProfile.Save();

      if (userID == null)
      {
        // something is seriously wrong here -- redirect to failure...
        YafBuildLink.Redirect(ForumPages.info, "i=7");
      }

      // handle e-mail verification if needed
      if (PageContext.BoardSettings.EmailVerification)
      {
        // get the user email
        var emailTextBox = (TextBox) this.CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Email");
        string email = emailTextBox.Text.Trim();

        string hashinput = DateTime.Now.ToString() + email + Security.CreatePassword(20);
        string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput, "md5");

        // save verification record...
        DB.checkemail_save(userID, hash, user.Email);

        var verifyEmail = new YafTemplateEmail("VERIFYEMAIL");

        string subject = GetTextFormatted("VERIFICATION_EMAIL_SUBJECT", PageContext.BoardSettings.Name);

        verifyEmail.TemplateParams["{link}"] = YafBuildLink.GetLinkNotEscaped(ForumPages.approve, true, "k={0}", hash);
        verifyEmail.TemplateParams["{key}"] = hash;
        verifyEmail.TemplateParams["{forumname}"] = PageContext.BoardSettings.Name;
        verifyEmail.TemplateParams["{forumlink}"] = String.Format("{0}", YafForumInfo.ForumURL);

        verifyEmail.SendEmail(new MailAddress(email, user.UserName), subject, true);
      }

      if (!String.IsNullOrEmpty(PageContext.BoardSettings.NotificationOnUserRegisterEmailList))
      {
        // send user register notification to the following admin users...
        string[] emails = PageContext.BoardSettings.NotificationOnUserRegisterEmailList.Split(';');

        var notifyAdmin = new YafTemplateEmail();

        string subject = String.Format(
          PageContext.Localization.GetText("COMMON", "NOTIFICATION_ON_USER_REGISTER_EMAIL_SUBJECT"), PageContext.BoardSettings.Name);

        notifyAdmin.TemplateParams["{adminlink}"] = YafBuildLink.GetLinkNotEscaped(ForumPages.admin_admin, true);
        notifyAdmin.TemplateParams["{user}"] = user.UserName;
        notifyAdmin.TemplateParams["{email}"] = user.Email;
        notifyAdmin.TemplateParams["{forumname}"] = PageContext.BoardSettings.Name;

        string emailBody = notifyAdmin.ProcessTemplate("NOTIFICATION_ON_USER_REGISTER");

        foreach (string email in emails)
        {
          if (!String.IsNullOrEmpty(email.Trim()))
          {
            YafServices.SendMail.Queue(PageContext.BoardSettings.ForumEmail, email.Trim(), subject, emailBody);
          }
        }
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
      this.CreateUserWizard1.UserName = this.CreateUserWizard1.UserName.Trim();

      // username cannot contain semi-colon or to be a bad word
      bool badWord = false;

      YafServices.BadWordReplace.ReplaceItems.ForEach(
        i =>
          {
            if (this.CreateUserWizard1.UserName.Equals(i.BadWord, StringComparison.CurrentCultureIgnoreCase))
            {
              badWord = true;
            }
          });
      
      if (this.CreateUserWizard1.UserName.Contains(";") || badWord || this.CreateUserWizard1.UserName.ToLower().Equals(UserMembershipHelper.GuestUserName.ToLower()))
      {
        PageContext.AddLoadMessage(GetText("BAD_USERNAME"));
        e.Cancel = true;
        return;
      }

      Control createUserTemplateRef = this.CreateUserWizard1.CreateUserStep.ContentTemplateContainer;
      var tbCaptcha = (TextBox) createUserTemplateRef.FindControl("tbCaptcha");

      // verify captcha if enabled
      if (PageContext.BoardSettings.EnableCaptchaForRegister && Session["CaptchaImageText"].ToString() != tbCaptcha.Text.Trim())
      {
        PageContext.AddLoadMessage(GetText("BAD_CAPTCHA"));
        e.Cancel = true;
      }
    }
  }
}