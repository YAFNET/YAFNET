/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The User Register Page.
    /// </summary>
    public partial class Register : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Register" /> class.
        /// </summary>
        public Register()
            : base("REGISTER")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether IsProtected.
        /// </summary>
        public override bool IsProtected => false;

        /// <summary>
        ///   Gets CreateUserStepContainer.
        /// </summary>
        private Control CreateUserStepContainer => this.CreateUserWizard1.CreateUserStep.ContentTemplateContainer;

        /// <summary>
        /// Gets or sets a value indicating whether the user is possible spam bot.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user is possible spam bot; otherwise, <c>false</c>.
        /// </value>
        private bool IsPossibleSpamBot { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is possible spam bot internal check.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is possible spam bot internal check; otherwise, <c>false</c>.
        /// </value>
        private bool IsPossibleSpamBotInternalCheck { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the ContinueButtonClick event of the CreateUserWizard1 control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected void CreateUserWizard1_ContinueButtonClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPossibleSpamBotInternalCheck && this.Get<BoardSettings>().BotHandlingOnRegister > 0)
            {
                return;
            }

            if (!this.Get<BoardSettings>().EmailVerification)
            {
                FormsAuthentication.SetAuthCookie(this.CreateUserWizard1.UserName, true);
            }

            // vzrus: to clear the cache to show user in the list at once
            this.Get<IDataCache>().Remove(Constants.Cache.UsersOnlineStatus);
            this.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);

            // redirect to the main forum URL      
            BuildLink.Redirect(ForumPages.Board);
        }

        /// <summary>
        /// Handles the CreateUserError event of the CreateUserWizard1 control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="CreateUserErrorEventArgs"/> instance containing the event data.
        /// </param>
        protected void CreateUserWizard1_CreateUserError([NotNull] object sender, [NotNull] CreateUserErrorEventArgs e)
        {
            // find the type of error
            var createUserError = e.CreateUserError switch
                {
                    MembershipCreateStatus.DuplicateEmail => this.GetText("ALREADY_REGISTERED"),
                    MembershipCreateStatus.DuplicateUserName => this.GetText("ALREADY_REGISTERED"),
                    MembershipCreateStatus.InvalidEmail => this.GetText("BAD_EMAIL"),
                    MembershipCreateStatus.InvalidPassword => this.GetText("BAD_PASSWORD"),
                    MembershipCreateStatus.InvalidQuestion => this.GetText("INVALID_QUESTION"),
                    MembershipCreateStatus.InvalidUserName => this.GetText("INVALID_USERNAME"),
                    MembershipCreateStatus.InvalidAnswer => this.GetText("INVALID_ANSWER"),
                    MembershipCreateStatus.InvalidProviderUserKey => "Invalid provider user key.",
                    MembershipCreateStatus.DuplicateProviderUserKey => "Duplicate provider user key.",
                    MembershipCreateStatus.ProviderError => "Provider Error",
                    MembershipCreateStatus.UserRejected => "User creation failed: Reason is defined by the provider.",
                    _ => string.Empty
                };

            this.PageContext.AddLoadMessage(createUserError, MessageTypes.danger);
        }

        /// <summary>
        /// Handles the CreatedUser event of the CreateUserWizard1 control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected void CreateUserWizard1_CreatedUser([NotNull] object sender, [NotNull] EventArgs e)
        {
            var user = UserMembershipHelper.GetUser(this.CreateUserWizard1.UserName);

            // setup initial roles (if any) for this user
            RoleMembershipHelper.SetupUserRoles(BoardContext.Current.PageBoardID, this.CreateUserWizard1.UserName);

            var displayName = user.UserName;

            if (this.Get<BoardSettings>().EnableDisplayName)
            {
                displayName = this.CreateUserStepContainer.FindControlAs<TextBox>("DisplayName").Text.Trim();
            }

            // create the user in the YAF DB as well as sync roles...
            var userID = RoleMembershipHelper.CreateForumUser(user, displayName, BoardContext.Current.PageBoardID);

            // create empty profile just so they have one
            var userProfile = Utils.UserProfile.GetProfile(this.CreateUserWizard1.UserName);

            // setup their initial profile information
            userProfile.Save();

            if (userID == null)
            {
                // something is seriously wrong here -- redirect to failure...
                BuildLink.RedirectInfoPage(InfoMessage.Failure);
            }

            if (this.IsPossibleSpamBot)
            {
                if (this.Get<BoardSettings>().BotHandlingOnRegister.Equals(1))
                {
                    this.Get<ISendNotification>().SendSpamBotNotificationToAdmins(user, userID.Value);
                }
            }
            else
            {
                // handle e-mail verification if needed
                if (this.Get<BoardSettings>().EmailVerification)
                {
                    // get the user email
                    var emailTextBox =
                        this.CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControlAs<TextBox>("Email");
                    var email = emailTextBox.Text.Trim();

                    this.Get<ISendNotification>().SendVerificationEmail(user, email, userID);
                }
                else
                {
                    // Send welcome mail/pm to user
                    this.Get<ISendNotification>().SendUserWelcomeNotification(user, userID.Value);
                }

                if (this.Get<BoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
                {
                    // send user register notification to the following admin users...
                    this.Get<ISendNotification>().SendRegistrationNotificationEmail(user, userID.Value);
                }
            }
        }

        /// <summary>
        /// Handles the CreatingUser event of the CreateUserWizard1 control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="LoginCancelEventArgs"/> instance containing the event data.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// CreateUserWizard.UserName;UserName from CreateUserWizard is Null!
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Argument is null.
        /// </exception>
        protected void CreateUserWizard1_CreatingUser([NotNull] object sender, [NotNull] LoginCancelEventArgs e)
        {
            var userName = this.CreateUserWizard1.UserName;

            if (userName.IsNotSet())
            {
                throw new ArgumentNullException("CreateUserWizard.UserName", "UserName from CreateUserWizard is Null!");
            }

            userName = userName.Trim();

            // trim username on post-back
            this.CreateUserWizard1.UserName = userName;

            // username cannot contain semi-colon or to be a bad word
            var badWord =
                this.Get<IBadWordReplace>()
                    .ReplaceItems.Any(i => userName.Equals(i.BadWord, StringComparison.CurrentCultureIgnoreCase));

            var guestUserName = UserMembershipHelper.GuestUserName;

            guestUserName = guestUserName.IsSet() ? guestUserName.ToLower() : string.Empty;

            if (userName.Contains(";") || badWord || userName.ToLower().Equals(guestUserName))
            {
                this.PageContext.AddLoadMessage(this.GetText("BAD_USERNAME"), MessageTypes.warning);
                e.Cancel = true;
                return;
            }

            if (userName.Length < this.Get<BoardSettings>().DisplayNameMinLength)
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("USERNAME_TOOSMALL", this.Get<BoardSettings>().DisplayNameMinLength), 
                    MessageTypes.danger);

                e.Cancel = true;
                return;
            }

            if (userName.Length > this.Get<BoardSettings>().UserNameMaxLength)
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("USERNAME_TOOLONG", this.Get<BoardSettings>().UserNameMaxLength), 
                    MessageTypes.danger);

                e.Cancel = true;
                return;
            }

            if (this.Get<BoardSettings>().EnableDisplayName)
            {
                var displayName = this.CreateUserStepContainer.FindControlAs<TextBox>("DisplayName");

                if (displayName != null)
                {
                    // Check if name matches the required minimum length
                    if (displayName.Text.Trim().Length < this.Get<BoardSettings>().DisplayNameMinLength)
                    {
                        this.PageContext.AddLoadMessage(
                            this.GetTextFormatted("USERNAME_TOOSMALL", this.Get<BoardSettings>().DisplayNameMinLength), 
                            MessageTypes.warning);
                        e.Cancel = true;

                        return;
                    }

                    // Check if name matches the required minimum length
                    if (displayName.Text.Length > this.Get<BoardSettings>().UserNameMaxLength)
                    {
                        this.PageContext.AddLoadMessage(
                            this.GetTextFormatted("USERNAME_TOOLONG", this.Get<BoardSettings>().UserNameMaxLength), 
                            MessageTypes.warning);

                        e.Cancel = true;

                        return;
                    }

                    if (this.Get<IUserDisplayName>().GetId(displayName.Text.Trim()).HasValue)
                    {
                        this.PageContext.AddLoadMessage(
                            this.GetText("ALREADY_REGISTERED_DISPLAYNAME"), 
                            MessageTypes.warning);

                        e.Cancel = true;
                    }
                }
            }

            this.IsPossibleSpamBot = false;

            // Check user for bot
            var userIpAddress = this.Get<HttpRequestBase>().GetUserRealIPAddress();

            // Check content for spam
            if (this.Get<ISpamCheck>().CheckUserForSpamBot(userName, this.CreateUserWizard1.Email, userIpAddress, out var result))
            {
                // Flag user as spam bot
                this.IsPossibleSpamBot = true;

                this.GetRepository<Registry>().IncrementDeniedRegistrations();

                this.Logger.Log(
                    null, 
                    "Bot Detected",
                    $"Bot Check detected a possible SPAM BOT: (user name : '{userName}', email : '{this.CreateUserWizard1.Email}', ip: '{userIpAddress}', reason : {result}), user was rejected.", 
                    EventLogTypes.SpamBotDetected);

                if (this.Get<BoardSettings>().BotHandlingOnRegister.Equals(2))
                {
                    this.GetRepository<Registry>().IncrementBannedUsers();

                    this.PageContext.AddLoadMessage(this.GetText("BOT_MESSAGE"), MessageTypes.danger);

                    if (this.Get<BoardSettings>().BanBotIpOnDetection)
                    {
                        this.GetRepository<BannedIP>()
                            .Save(
                                null, 
                                userIpAddress,
                                $"A spam Bot who was trying to register was banned by IP {userIpAddress}", 
                                this.PageContext.PageUserID);

                        // Clear cache
                        this.Get<IDataCache>().Remove(Constants.Cache.BannedIP);

                        if (BoardContext.Current.Get<BoardSettings>().LogBannedIP)
                        {
                            this.Logger
                                .Log(
                                    this.PageContext.PageUserID, 
                                    "IP BAN of Bot During Registration",
                                    $"A spam Bot who was trying to register was banned by IP {userIpAddress}", 
                                    EventLogTypes.IpBanSet);
                        }
                    }

                    // Ban Name ?
                    BoardContext.Current.GetRepository<BannedName>().Save(null, userName, "Name was reported by the automatic spam system.");

                    // Ban User Email?
                    BoardContext.Current.GetRepository<BannedEmail>().Save(null, this.CreateUserWizard1.Email, "Email was reported by the automatic spam system.");

                    e.Cancel = true;
                }
            }

            switch (this.Get<BoardSettings>().CaptchaTypeRegister)
            {
                case 1:
                    {
                        // Check YAF Captcha
                        var yafCaptchaText = this.CreateUserStepContainer.FindControlAs<TextBox>("tbCaptcha");

                        if (!CaptchaHelper.IsValid(yafCaptchaText.Text.Trim()))
                        {
                            this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"), MessageTypes.danger);
                            e.Cancel = true;
                        }
                    }

                    break;
                case 2:
                    {
                        // Check reCAPTCHA
                        var recaptcha =

                            // this.CreateUserWizard1.FindWizardControlRecursive("Recaptcha1").ToClass<RecaptchaControl>();
                            this.CreateUserStepContainer.FindControlAs<RecaptchaControl>("Recaptcha1");

                        if (!recaptcha.IsValid)
                        {
                            this.PageContext.AddLoadMessage(this.GetText("BAD_RECAPTCHA"), MessageTypes.danger);
                            e.Cancel = true;
                        }
                    }

                    break;
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
        protected void CreateUserWizard1_NextButtonClick([NotNull] object sender, [NotNull] WizardNavigationEventArgs e)
        {
            if (this.CreateUserWizard1.WizardSteps[e.CurrentStepIndex].ID != "profile")
            {
                return;
            }

            var user = UserMembershipHelper.GetUser(this.CreateUserWizard1.UserName);

            // save the time zone...
            var userId = UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey);

            this.SetupUserProfile(user, userId);

            // Clearing cache with old Active User Lazy Data ...
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
        protected void CreateUserWizard1_PreviousButtonClick(
            [NotNull] object sender, 
            [NotNull] WizardNavigationEventArgs e)
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
            var password = this.CreateUserStepContainer.FindControlAs<TextBox>("Password");
            var confirmPassword = this.CreateUserStepContainer.FindControlAs<TextBox>("ConfirmPassword");

            BoardContext.Current.PageElements.RegisterJsBlockStartup(
                "passwordStrengthCheckJs",
                JavaScriptBlocks.PasswordStrengthCheckerJs(
                    password.ClientID,
                    confirmPassword.ClientID,
                    this.Get<MembershipProvider>().MinRequiredPasswordLength,
                    this.GetText("PASSWORD_NOTMATCH"),
                    this.GetTextFormatted("PASSWORD_MIN", this.Get<MembershipProvider>().MinRequiredPasswordLength),
                    this.GetText("PASSWORD_GOOD"),
                    this.GetText("PASSWORD_STRONGER"),
                    this.GetText("PASSWORD_WEAK")));

            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.CreateUserWizard1.MembershipProvider = Config.MembershipProvider;

            // handle the CreateUser Step localization
            this.SetupCreateUserStep();

            var loginButton = this.CreateUserStepContainer.FindControlAs<ThemeButton>("LoginButton");

            // handle other steps localization
            ((Button)this.CreateUserWizard1.FindWizardControlRecursive("ContinueButton")).Text =
                this.GetText("CONTINUE");

            var facebookRegister = (ThemeButton)this.CreateUserWizard1.FindWizardControlRecursive("FacebookRegister");
            var twitterRegister = (ThemeButton)this.CreateUserWizard1.FindWizardControlRecursive("TwitterRegister");
            var googleRegister = (ThemeButton)this.CreateUserWizard1.FindWizardControlRecursive("GoogleRegister");

            var authPanel = (PlaceHolder)this.CreateUserWizard1.FindWizardControlRecursive("AuthPanel");

            if (this.PageContext.IsGuest && !Config.IsAnyPortal && Config.AllowLoginAndLogoff)
            {
                loginButton.Visible = true;
                loginButton.Text = this.GetText("LOGIN_INSTEAD");
                loginButton.NavigateUrl = BuildLink.GetLink(ForumPages.Login);
            }

            if (this.Get<BoardSettings>().AllowSingleSignOn)
            {
                if (Config.FacebookAPIKey.IsSet() && Config.FacebookSecretKey.IsSet())
                {
                    facebookRegister.Visible = authPanel.Visible = true;
                    facebookRegister.Text = this.GetTextFormatted("REGISTER_AUTH", "Facebook");
                }

                if (Config.TwitterConsumerKey.IsSet() && Config.TwitterConsumerSecret.IsSet())
                {
                    twitterRegister.Visible = authPanel.Visible = true;
                    twitterRegister.Text = this.GetTextFormatted("REGISTER_AUTH", "Twitter");
                }

                if (Config.GoogleClientID.IsSet() && Config.GoogleClientSecret.IsSet())
                {
                    googleRegister.Visible = authPanel.Visible = true;
                    googleRegister.Text = this.GetTextFormatted("REGISTER_AUTH", "Google");
                }
            }

            // get the time zone data source
            var timeZones = (DropDownList)this.CreateUserWizard1.FindWizardControlRecursive("TimeZones");
            timeZones.DataSource = StaticDataHelper.TimeZones();

            // get the country data source
            var country = (CountryImageListBox)this.CreateUserWizard1.FindWizardControlRecursive("Country");
            country.DataSource = StaticDataHelper.Country();

            if (!this.Get<BoardSettings>().EmailVerification)
            {
                // automatically log in created users
                this.CreateUserWizard1.LoginCreatedUser = false;
                this.CreateUserWizard1.DisableCreatedUser = false;

                // success notification localization
                ((Literal)this.CreateUserWizard1.FindWizardControlRecursive("AccountCreated")).Text =
                    this.Get<IBBCode>().MakeHtml(this.GetText("ACCOUNT_CREATED"), false, true);
            }
            else
            {
                this.CreateUserWizard1.LoginCreatedUser = false;
                this.CreateUserWizard1.DisableCreatedUser = true;

                // success notification localization
                ((Literal)this.CreateUserWizard1.FindWizardControlRecursive("AccountCreated")).Text =
                    this.Get<IBBCode>().MakeHtml(this.GetText("ACCOUNT_CREATED_VERIFICATION"), false, true);
            }

            this.CreateUserWizard1.ContinueDestinationPageUrl = BoardInfo.ForumURL;
            this.CreateUserWizard1.FinishDestinationPageUrl = BoardInfo.ForumURL;

            this.DataBind();

            // fill location field 
            if (this.Get<BoardSettings>().EnableIPInfoService)
            {
                try
                {
                    this.FillLocationData(country);
                }
                catch (Exception exception)
                {
                    this.Logger.Log(
                        null, 
                        this,
                        $"Error whith Location Data for IP: {this.Get<HttpRequestBase>().GetUserRealIPAddress()}, exception is: {exception}");
                }
            }

            // Auto Fill user time zone
            timeZones.Items.FindByValue(TimeZoneInfo.Local.Id).Selected = true;

            this.CreateUserWizard1.FindWizardControlRecursive("UserName").Focus();

            if (this.Get<BoardSettings>().CaptchaTypeRegister == 2)
            {
                this.SetupRecaptchaControl();
            }
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(this.GetText("TITLE"));
        }

        /// <summary>
        /// Handles the Click event of the RefreshCaptcha control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected void RefreshCaptcha_Click(object sender, EventArgs e)
        {
            var imgCaptcha = this.CreateUserStepContainer.FindControlAs<Image>("imgCaptcha");
            imgCaptcha.ImageUrl = $"{BoardInfo.ForumClientFileRoot}resource.ashx?c=1&t=";
        }

        /// <summary>
        /// Redirects to the Facebook login/register page.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected void FacebookRegisterClick(object sender, EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Login, "auth={0}", AuthService.facebook);
        }

        /// <summary>
        /// Redirects to the Twitter login/register page.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected void TwitterRegisterClick(object sender, EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Login, "auth={0}", AuthService.twitter);
        }

        /// <summary>
        /// Redirects to the Google login/register page.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected void GoogleRegisterClick(object sender, EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Login, "auth={0}", AuthService.google);
        }

        /// <summary>
        /// Gets the reCAPTCHA site key.
        /// </summary>
        /// <returns>Returns the eCAPTCHA site key.</returns>
        protected string GetSiteKey()
        {
            return this.PageContext.BoardSettings.RecaptchaPublicKey;
        }

        /// <summary>
        /// The setup display name UI.
        /// </summary>
        /// <param name="startControl">
        /// The start control.
        /// </param>
        /// <param name="enabled">
        /// The enabled.
        /// </param>
        private static void SetupDisplayNameUI([NotNull] Control startControl, bool enabled)
        {
            startControl.FindControlAs<PlaceHolder>("DisplayNamePlaceHolder").Visible = enabled;
            startControl.FindControlAs<LocalizedRequiredFieldValidator>("DisplayNameRequired").Enabled = enabled;
        }

        /// <summary>
        /// The setup create user step.
        /// </summary>
        private void SetupCreateUserStep()
        {
            // Set Name lengths
            this.CreateUserStepContainer.FindControlAs<TextBox>("DisplayName").MaxLength =
                this.Get<BoardSettings>().UserNameMaxLength;
            this.CreateUserStepContainer.FindControlAs<TextBox>("UserName").MaxLength =
                this.Get<BoardSettings>().UserNameMaxLength;

            var usernameRequired = this.CreateUserStepContainer.FindControlAs<RequiredFieldValidator>(
                "UserNameRequired");
            var passwordRequired = this.CreateUserStepContainer.FindControlAs<RequiredFieldValidator>(
                "PasswordRequired");
            var confirmPasswordRequired =
                this.CreateUserStepContainer.FindControlAs<RequiredFieldValidator>("ConfirmPasswordRequired");
            var emailRequired = this.CreateUserStepContainer.FindControlAs<RequiredFieldValidator>("EmailRequired");
            var emailValid = this.CreateUserStepContainer.FindControlAs<RegularExpressionValidator>("EmailValid");

            var questionRequired = this.CreateUserStepContainer.FindControlAs<RequiredFieldValidator>(
                "QuestionRequired");
            var answerRequired = this.CreateUserStepContainer.FindControlAs<RequiredFieldValidator>("AnswerRequired");
            var createUser = this.CreateUserStepContainer.FindControlAs<Button>("StepNextButton");

            usernameRequired.ToolTip = usernameRequired.ErrorMessage = this.GetText("NEED_USERNAME");
            passwordRequired.ToolTip = passwordRequired.ErrorMessage = this.GetText("NEED_PASSWORD");
            confirmPasswordRequired.ToolTip = confirmPasswordRequired.ErrorMessage = this.GetText("RETYPE_PASSWORD");
            emailRequired.ToolTip = emailRequired.ErrorMessage = this.GetText("NEED_EMAIL");
            emailValid.ToolTip = emailValid.ErrorMessage = this.GetText("PROFILE", "BAD_EMAIL");
            questionRequired.ToolTip = questionRequired.ErrorMessage = this.GetText("NEED_QUESTION");
            answerRequired.ToolTip = answerRequired.ErrorMessage = this.GetText("NEED_ANSWER");
            createUser.ToolTip = createUser.Text = this.GetText("CREATE_USER");

            var captchaPlaceHolder = this.CreateUserStepContainer.FindControlAs<PlaceHolder>("YafCaptchaHolder");
            var recaptchaPlaceHolder = this.CreateUserStepContainer.FindControlAs<PlaceHolder>("RecaptchaPlaceHolder");

            if (this.Get<BoardSettings>().CaptchaTypeRegister == 1)
            {
                var imgCaptcha = this.CreateUserStepContainer.FindControlAs<Image>("imgCaptcha");

                imgCaptcha.ImageUrl = $"{BoardInfo.ForumClientFileRoot}resource.ashx?c=1&t={DateTime.UtcNow}";

                var refreshCaptcha = this.CreateUserStepContainer.FindControlAs<LinkButton>("RefreshCaptcha");

                refreshCaptcha.Text = this.GetText("GENERATE_CAPTCHA");

                refreshCaptcha.Click += this.RefreshCaptcha_Click;

                captchaPlaceHolder.Visible = true;
            }
            else
            {
                captchaPlaceHolder.Visible = false;
            }

            recaptchaPlaceHolder.Visible = this.Get<BoardSettings>().CaptchaTypeRegister == 2;

            SetupDisplayNameUI(this.CreateUserStepContainer, this.Get<BoardSettings>().EnableDisplayName);

            var questionAnswerPlaceHolder =
                this.CreateUserStepContainer.FindControlAs<PlaceHolder>("QuestionAnswerPlaceHolder");
            questionAnswerPlaceHolder.Visible = this.Get<MembershipProvider>().RequiresQuestionAndAnswer;
        }

        /// <summary>
        /// The setup reCAPTCHA control.
        /// </summary>
        private void SetupRecaptchaControl()
        {
            this.CreateUserWizard1.FindWizardControlRecursive("RecaptchaPlaceHolder").Visible = true;

            if (this.Get<BoardSettings>().RecaptchaPrivateKey.IsSet()
                && this.Get<BoardSettings>().RecaptchaPublicKey.IsSet())
            {
                return;
            }

            this.Logger.Log(this.PageContext.PageUserID, this, "secret or site key is required for reCAPTCHA!");
            BuildLink.AccessDenied();
        }

        /// <summary>
        /// Fills the location data.
        /// </summary>
        /// <param name="country">The country.</param>
        private void FillLocationData([NotNull]ListControl country)
        {
            var userIpLocator = BoardContext.Current.Get<IIpInfoService>().GetUserIpLocator();

            if (userIpLocator == null)
            {
                return;
            }

            var location = new StringBuilder();

            if (userIpLocator["CountryCode"] != null && userIpLocator["CountryCode"].IsSet() && !userIpLocator["CountryCode"].Equals("-"))
            {
                var countryItem = country.Items.FindByValue(userIpLocator["CountryCode"]);

                if (countryItem != null)
                {
                    countryItem.Selected = true;
                }
            }

            if (userIpLocator["RegionName"] != null && userIpLocator["RegionName"].IsSet() && !userIpLocator["RegionName"].Equals("-"))
            {
                location.Append(userIpLocator["RegionName"]);
            }

            if (userIpLocator["CityName"] != null && userIpLocator["CityName"].IsSet() && !userIpLocator["CityName"].Equals("-"))
            {
                location.AppendFormat(", {0}", userIpLocator["CityName"]);
            }

            this.CreateUserWizard1.FindControlRecursiveAs<TextBox>("Location").Text = location.ToString();
        }

        /// <summary>
        /// Setups the user profile.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="userId">
        /// The user identifier.
        /// </param>
        private void SetupUserProfile(MembershipUser user, int userId)
        {
            // this is the "Profile Information" step. Save the data to their profile (+ defaults).
            var timeZones = (DropDownList)this.CreateUserWizard1.FindWizardControlRecursive("TimeZones");
            var country = (CountryImageListBox)this.CreateUserWizard1.FindWizardControlRecursive("Country");
            var locationTextBox = (TextBox)this.CreateUserWizard1.FindWizardControlRecursive("Location");
            var homepageTextBox = (TextBox)this.CreateUserWizard1.FindWizardControlRecursive("Homepage");

            // setup/save the profile
            var userProfile = Utils.UserProfile.GetProfile(this.CreateUserWizard1.UserName);

            if (country.SelectedValue != null)
            {
                userProfile.Country = country.SelectedValue;
            }

            if (this.Get<ISpamWordCheck>().CheckForSpamWord(homepageTextBox.Text.Trim(), out _))
            {
                this.IsPossibleSpamBotInternalCheck = true;

                // Flag user as spam bot
                this.IsPossibleSpamBot = true;

                var userIpAddress = this.Get<HttpRequestBase>().GetUserRealIPAddress();

                if (this.Get<BoardSettings>().BotHandlingOnRegister.Equals(1))
                {
                    this.Get<ISendNotification>().SendSpamBotNotificationToAdmins(user, userId);
                }
                else if (this.Get<BoardSettings>().BotHandlingOnRegister.Equals(2))
                {
                    // Kill user
                    UserMembershipHelper.DeleteAndBanUser(userId, user, userIpAddress);

                    this.PageContext.AddLoadMessage(this.GetText("BOT_MESSAGE"), MessageTypes.danger);
                }

                this.GetRepository<Registry>().IncrementDeniedRegistrations();

                this.Logger.Log(
                        null, 
                        "Bot Detected",
                        $"Internal Spam Word Check detected a SPAM BOT: (user name : '{user.UserName}', email : '{this.CreateUserWizard1.Email}', ip: '{userIpAddress}') reason word: {homepageTextBox.Text.Trim()}", 
                        EventLogTypes.SpamBotDetected);
            }

            if (this.IsPossibleSpamBotInternalCheck)
            {
                return;
            }

            userProfile.Location = locationTextBox.Text.Trim();

            // add http:// by default
            if (!Regex.IsMatch(homepageTextBox.Text.Trim(), @"^(http|https|ftp|ftps|git|svn|news)\://.*"))
            {
                homepageTextBox.Text = $"http://{homepageTextBox.Text.Trim()}";
            }

            if (ValidationHelper.IsValidURL(homepageTextBox.Text))
            {
                userProfile.Homepage = homepageTextBox.Text.Trim();
            }

            userProfile.Save();

            var autoWatchTopicsEnabled = this.Get<BoardSettings>().DefaultNotificationSetting
                                         == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

            // save the time zone...
            this.GetRepository<User>().Save(
                userId,
                this.PageContext.PageBoardID,
                null,
                null,
                null,
                timeZones.SelectedValue,
                null,
                null,
                null,
                false);

            // save the settings...
            this.GetRepository<User>().SaveNotification(
                userId, 
                true, 
                autoWatchTopicsEnabled, 
                this.Get<BoardSettings>().DefaultNotificationSetting.ToInt(), 
                this.Get<BoardSettings>().DefaultSendDigestEmail);
        }

        #endregion
    }
}