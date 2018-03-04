/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utilities;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The User Register Page.
    /// </summary>
    public partial class register : ForumPage
    {
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
        ///   Gets or sets the User IP Info.
        /// </summary>
        public IDictionary<string, string> UserIpLocator { get; set; }

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
            if (this.IsPossibleSpamBotInternalCheck && this.Get<YafBoardSettings>().BotHandlingOnRegister > 0)
            {
                return;
            }

            if (!this.Get<YafBoardSettings>().EmailVerification)
            {
                FormsAuthentication.SetAuthCookie(this.CreateUserWizard1.UserName, true);
            }

            // vzrus: to clear the cache to show user in the list at once
            this.Get<IDataCache>().Remove(Constants.Cache.UsersOnlineStatus);
            this.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);

            // redirect to the main forum URL      
            YafBuildLink.Redirect(ForumPages.forum);
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
            var createUserError = string.Empty;

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

            // setup inital roles (if any) for this user
            RoleMembershipHelper.SetupUserRoles(YafContext.Current.PageBoardID, this.CreateUserWizard1.UserName);

            var displayName = user.UserName;

            if (this.Get<YafBoardSettings>().EnableDisplayName)
            {
                displayName = this.CreateUserStepContainer.FindControlAs<TextBox>("DisplayName").Text.Trim();
            }

            // create the user in the YAF DB as well as sync roles...
            var userID = RoleMembershipHelper.CreateForumUser(user, displayName, YafContext.Current.PageBoardID);

            // create empty profile just so they have one
            var userProfile = YafUserProfile.GetProfile(this.CreateUserWizard1.UserName);

            // setup their inital profile information
            userProfile.Save();

            if (userID == null)
            {
                // something is seriously wrong here -- redirect to failure...
                YafBuildLink.RedirectInfoPage(InfoMessage.Failure);
            }

            if (this.IsPossibleSpamBot)
            {
                if (this.Get<YafBoardSettings>().BotHandlingOnRegister.Equals(1))
                {
                    this.Get<ISendNotification>().SendSpamBotNotificationToAdmins(user, userID.Value);
                }
            }
            else
            {
                // handle e-mail verification if needed
                if (this.Get<YafBoardSettings>().EmailVerification)
                {
                    // get the user email
                    var emailTextBox =
                        (TextBox)this.CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Email");
                    var email = emailTextBox.Text.Trim();

                    this.Get<ISendNotification>().SendVerificationEmail(user, email, userID);
                }
                else
                {
                    // Send welcome mail/pm to user
                    this.Get<ISendNotification>().SendUserWelcomeNotification(user, userID.Value);
                }

                if (this.Get<YafBoardSettings>().NotificationOnUserRegisterEmailList.IsSet())
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

            // trim username on postback
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

            if (userName.Length < this.Get<YafBoardSettings>().DisplayNameMinLength)
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("USERNAME_TOOSMALL", this.Get<YafBoardSettings>().DisplayNameMinLength), 
                    MessageTypes.danger);

                e.Cancel = true;
                return;
            }

            if (userName.Length > this.Get<YafBoardSettings>().UserNameMaxLength)
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("USERNAME_TOOLONG", this.Get<YafBoardSettings>().UserNameMaxLength), 
                    MessageTypes.danger);

                e.Cancel = true;
                return;
            }

            if (this.Get<YafBoardSettings>().EnableDisplayName)
            {
                var displayName = this.CreateUserStepContainer.FindControlAs<TextBox>("DisplayName");

                if (displayName != null)
                {
                    // Check if name matches the required minimum length
                    if (displayName.Text.Trim().Length < this.Get<YafBoardSettings>().DisplayNameMinLength)
                    {
                        this.PageContext.AddLoadMessage(
                            this.GetTextFormatted("USERNAME_TOOSMALL", this.Get<YafBoardSettings>().DisplayNameMinLength), 
                            MessageTypes.warning);
                        e.Cancel = true;

                        return;
                    }

                    // Check if name matches the required minimum length
                    if (displayName.Text.Length > this.Get<YafBoardSettings>().UserNameMaxLength)
                    {
                        this.PageContext.AddLoadMessage(
                            this.GetTextFormatted("USERNAME_TOOLONG", this.Get<YafBoardSettings>().UserNameMaxLength), 
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
            var spamChecker = new YafSpamCheck();
            string result;

            var userIpAddress = this.Get<HttpRequestBase>().GetUserRealIPAddress();

            // Check content for spam
            if (spamChecker.CheckUserForSpamBot(userName, this.CreateUserWizard1.Email, userIpAddress, out result))
            {
                // Flag user as spam bot
                this.IsPossibleSpamBot = true;

                this.Logger.Log(
                    null, 
                    "Bot Detected", 
                    "Bot Check detected a possible SPAM BOT: (user name : '{0}', email : '{1}', ip: '{2}', reason : {3}), user was rejected."
                        .FormatWith(userName, this.CreateUserWizard1.Email, userIpAddress, result), 
                    EventLogTypes.SpamBotDetected);

                if (this.Get<YafBoardSettings>().BotHandlingOnRegister.Equals(2))
                {
                    this.PageContext.AddLoadMessage(this.GetText("BOT_MESSAGE"), MessageTypes.danger);

                    if (this.Get<YafBoardSettings>().BanBotIpOnDetection)
                    {
                        this.GetRepository<BannedIP>()
                            .Save(
                                null, 
                                userIpAddress, 
                                "A spam Bot who was trying to register was banned by IP {0}".FormatWith(userIpAddress), 
                                this.PageContext.PageUserID);

                        // Clear cache
                        this.Get<IDataCache>().Remove(Constants.Cache.BannedIP);

                        if (YafContext.Current.Get<YafBoardSettings>().LogBannedIP)
                        {
                            this.Get<ILogger>()
                                .Log(
                                    this.PageContext.PageUserID, 
                                    "IP BAN of Bot During Registration", 
                                    "A spam Bot who was trying to register was banned by IP {0}".FormatWith(
                                        userIpAddress), 
                                    EventLogTypes.IpBanSet);
                        }
                    }

                    e.Cancel = true;
                }
            }

            switch (this.Get<YafBoardSettings>().CaptchaTypeRegister)
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

                        // Recupt;
                        if (!recaptcha.IsValid)
                        {
                            this.PageContext.AddLoadMessage(this.GetText("BAD_RECAPTCHA"), MessageTypes.danger);
                            e.Cancel = true;
                        }
                    }

                    break;
            }

            /*
            

            // vzrus: Here recaptcha should be always valid. This piece of code for testing only.
            if (this.Get<YafBoardSettings>().CaptchaTypeRegister == 2)
            {
                var recaptcha =
                    this.CreateUserWizard1.FindWizardControlRecursive("Recaptcha1").ToClass<RecaptchaControl>();

                if (recaptcha != null && !recaptcha.IsValid)
                {
                    this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"), MessageTypes.Error);
                    e.Cancel = true;
                }
            }

            */
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
            // setup jQuery and DatePicker JS...
            var country = (ImageListBox)this.CreateUserWizard1.FindWizardControlRecursive("Country");

            var password = this.CreateUserStepContainer.FindControlAs<TextBox>("Password");
            var confirmPassword = this.CreateUserStepContainer.FindControlAs<TextBox>("ConfirmPassword");

            YafContext.Current.PageElements.RegisterJsBlockStartup(
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

            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(this.GetText("TITLE"));

            // handle the CreateUser Step localization
            this.SetupCreateUserStep();

            // handle other steps localization
            ((Button)this.CreateUserWizard1.FindWizardControlRecursive("ProfileNextButton")).Text =
                    this.GetText("SAVE");
            ((Button)this.CreateUserWizard1.FindWizardControlRecursive("ContinueButton")).Text =
                this.GetText("CONTINUE");

            var dstUser = (CheckBox)this.CreateUserWizard1.FindWizardControlRecursive("DSTUser");

            dstUser.Text = this.GetText("CP_EDITPROFILE", "DST");

            var facebookRegister = (LinkButton)this.CreateUserWizard1.FindWizardControlRecursive("FacebookRegister");
            var twitterRegister = (LinkButton)this.CreateUserWizard1.FindWizardControlRecursive("TwitterRegister");
            var googleRegister = (LinkButton)this.CreateUserWizard1.FindWizardControlRecursive("GoogleRegister");

            var loginButton = (LinkButton)this.CreateUserWizard1.FindWizardControlRecursive("LoginButton");

            var authPanel = (Panel)this.CreateUserWizard1.FindWizardControlRecursive("AuthPanel");

            if (this.PageContext.IsGuest && !Config.IsAnyPortal && Config.AllowLoginAndLogoff)
            {
                loginButton.Visible = true;
                loginButton.Text = this.GetText("LOGIN_INSTEAD");
            }

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

            // get the time zone data source
            var timeZones = (DropDownList)this.CreateUserWizard1.FindWizardControlRecursive("TimeZones");
            timeZones.DataSource = StaticDataHelper.TimeZones();

            // get the country data source
            var country = (ImageListBox)this.CreateUserWizard1.FindWizardControlRecursive("Country");
            country.ImageLocation = YafForumInfo.GetURLToContent("images/flags/{0}.png");
            country.DataSource = StaticDataHelper.Country();

            if (this.Get<YafBoardSettings>().EnableIPInfoService && this.UserIpLocator == null)
            {
                // vzrus: we should always get not null class here
                this.UserIpLocator = new IPDetails().GetData(
                    this.Get<HttpRequestBase>().GetUserRealIPAddress(), 
                    "text", 
                    false, 
                    this.PageContext().CurrentForumPage.Localization.Culture.Name,
                    string.Empty, 
                    string.Empty);
            }

            if (!this.Get<YafBoardSettings>().EmailVerification)
            {
                // automatically log in created users
                this.CreateUserWizard1.LoginCreatedUser = false;
                this.CreateUserWizard1.DisableCreatedUser = false;

                // success notification localization
                ((Literal)this.CreateUserWizard1.FindWizardControlRecursive("AccountCreated")).Text =
                    this.Get<IBBCode>().MakeHtml(this.GetText("ACCOUNT_CREATED"), false, true, false);
            }
            else
            {
                this.CreateUserWizard1.LoginCreatedUser = false;
                this.CreateUserWizard1.DisableCreatedUser = true;

                // success notification localization
                ((Literal)this.CreateUserWizard1.FindWizardControlRecursive("AccountCreated")).Text =
                    this.Get<IBBCode>().MakeHtml(this.GetText("ACCOUNT_CREATED_VERIFICATION"), false, true, false);
            }

            this.CreateUserWizard1.ContinueDestinationPageUrl = YafForumInfo.ForumURL;
            this.CreateUserWizard1.FinishDestinationPageUrl = YafForumInfo.ForumURL;

            this.DataBind();

            // fill location field 
            if (this.Get<YafBoardSettings>().EnableIPInfoService)
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
                        "Error whith Location Data for IP: {0}, exception is: {1}".FormatWith(
                            this.Get<HttpRequestBase>().GetUserRealIPAddress(), 
                            exception));
                }
            }

            // Auto Fill user time zone
            timeZones.Items.FindByValue(TimeZoneInfo.Local.Id).Selected = true;
            dstUser.Checked = TimeZoneInfo.Local.SupportsDaylightSavingTime;

            this.CreateUserWizard1.FindWizardControlRecursive("UserName").Focus();

            if (this.Get<YafBoardSettings>().CaptchaTypeRegister == 2)
            {
                this.SetupRecaptchaControl();
            }
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
            imgCaptcha.ImageUrl = "{0}resource.ashx?c=1&t=".FormatWith(
                YafForumInfo.ForumClientFileRoot, 
                DateTime.UtcNow);
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
            YafBuildLink.Redirect(ForumPages.login, "auth={0}", AuthService.facebook);
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
            YafBuildLink.Redirect(ForumPages.login, "auth={0}", AuthService.twitter);
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
            YafBuildLink.Redirect(ForumPages.login, "auth={0}", AuthService.google);
        }

        /// <summary>
        /// Logins the click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected void LoginClick(object sender, EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.login);
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
        /// Gets the reCAPTCHA secret key.
        /// </summary>
        /// <returns>Returns the reCAPTCHA secret key.</returns>
        protected string GetSecretKey()
        {
            return this.PageContext.BoardSettings.RecaptchaPrivateKey;
        }

        /// <summary>
        /// The setup create user step.
        /// </summary>
        private void SetupCreateUserStep()
        {
            // Set Name lengths
            this.CreateUserStepContainer.FindControlAs<TextBox>("DisplayName").MaxLength =
                this.Get<YafBoardSettings>().UserNameMaxLength;
            this.CreateUserStepContainer.FindControlAs<TextBox>("UserName").MaxLength =
                this.Get<YafBoardSettings>().UserNameMaxLength;

            var usernameRequired = this.CreateUserStepContainer.FindControlAs<RequiredFieldValidator>(
                "UserNameRequired");
            var passwordRequired = this.CreateUserStepContainer.FindControlAs<RequiredFieldValidator>(
                "PasswordRequired");
            var confirmPasswordRequired =
                this.CreateUserStepContainer.FindControlAs<RequiredFieldValidator>("ConfirmPasswordRequired");
            var emailRequired = this.CreateUserStepContainer.FindControlAs<RequiredFieldValidator>("EmailRequired");
            var emailValid = (RegularExpressionValidator)this.CreateUserStepContainer.FindControl("EmailValid");

            var questionRequired = this.CreateUserStepContainer.FindControlAs<RequiredFieldValidator>(
                "QuestionRequired");
            var answerRequired = this.CreateUserStepContainer.FindControlAs<RequiredFieldValidator>("AnswerRequired");
            var createUser = (Button)this.CreateUserStepContainer.FindControl("StepNextButton");

            usernameRequired.ToolTip = usernameRequired.ErrorMessage = this.GetText("NEED_USERNAME");
            passwordRequired.ToolTip = passwordRequired.ErrorMessage = this.GetText("NEED_PASSWORD");
            confirmPasswordRequired.ToolTip = confirmPasswordRequired.ErrorMessage = this.GetText("RETYPE_PASSWORD");
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

                imgCaptcha.ImageUrl = "{0}resource.ashx?c=1&t=".FormatWith(
                    YafForumInfo.ForumClientFileRoot,
                    DateTime.UtcNow);

                var refreshCaptcha = this.CreateUserStepContainer.FindControlAs<LinkButton>("RefreshCaptcha");

                refreshCaptcha.Text = this.GetText("GENERATE_CAPTCHA");

                refreshCaptcha.Click += this.RefreshCaptcha_Click;

                captchaPlaceHolder.Visible = true;
            }
            else
            {
                captchaPlaceHolder.Visible = false;
            }

            recaptchaPlaceHolder.Visible = this.Get<YafBoardSettings>().CaptchaTypeRegister == 2;

            this.SetupDisplayNameUI(this.CreateUserStepContainer, this.Get<YafBoardSettings>().EnableDisplayName);

            var questionAnswerPlaceHolder =
                (PlaceHolder)this.CreateUserStepContainer.FindControl("QuestionAnswerPlaceHolder");
            questionAnswerPlaceHolder.Visible = this.Get<MembershipProvider>().RequiresQuestionAndAnswer;
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
        private void SetupDisplayNameUI([NotNull] Control startControl, bool enabled)
        {
            startControl.FindControlAs<PlaceHolder>("DisplayNamePlaceHolder").Visible = enabled;
            startControl.FindControlAs<LocalizedRequiredFieldValidator>("DisplayNameRequired").Enabled = enabled;
        }

        /// <summary>
        /// The setup reCAPTCHA control.
        /// </summary>
        private void SetupRecaptchaControl()
        {
            this.CreateUserWizard1.FindWizardControlRecursive("RecaptchaPlaceHolder").Visible = true;

            if (this.Get<YafBoardSettings>().RecaptchaPrivateKey.IsNotSet()
                || this.Get<YafBoardSettings>().RecaptchaPublicKey.IsNotSet())
            {
                // this.PageContext.AddLoadMessage(this.GetText("RECAPTCHA_BADSETTING"));              
                this.Logger.Log(this.PageContext.PageUserID, this, "secret or site key is required for reCAPTCHA!");
                YafBuildLink.AccessDenied();
            }
        }

        /// <summary>
        /// Fills the location data.
        /// </summary>
        /// <param name="country">The country.</param>
        private void FillLocationData([NotNull]DropDownList country)
        {
            if (this.UserIpLocator == null || this.UserIpLocator["StatusCode"] != "OK")
            {
                this.Logger.Log(
                    null, 
                    this, 
                    "Geolocation Service reports: {0}".FormatWith(this.UserIpLocator["StatusMessage"]), 
                    EventLogTypes.Information);
            }

            if (this.UserIpLocator["StatusCode"] != "OK")
            {
                this.Logger.Log(
                    null, 
                    this, 
                    "Geolocation Service reports: {0}".FormatWith(this.UserIpLocator["StatusMessage"]), 
                    EventLogTypes.Information);
            }

            if (this.UserIpLocator.Count <= 0 || this.UserIpLocator["StatusCode"] != "OK")
            {
                return;
            }

            var location = new StringBuilder();

            if (this.UserIpLocator["CountryCode"] != null && this.UserIpLocator["CountryCode"].IsSet() && !this.UserIpLocator["CountryCode"].Equals("-"))
            {
                var countryItem = country.Items.FindByValue(this.UserIpLocator["CountryCode"]);

                if (countryItem != null)
                {
                    countryItem.Selected = true;
                }
            }

            if (this.UserIpLocator["RegionName"] != null && this.UserIpLocator["RegionName"].IsSet() && !this.UserIpLocator["RegionName"].Equals("-"))
            {
                location.Append(this.UserIpLocator["RegionName"]);
            }

            if (this.UserIpLocator["CityName"] != null && this.UserIpLocator["CityName"].IsSet() && !this.UserIpLocator["CityName"].Equals("-"))
            {
                location.AppendFormat(", {0}", this.UserIpLocator["CityName"]);
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
            var country = (ImageListBox)this.CreateUserWizard1.FindWizardControlRecursive("Country");
            var locationTextBox = (TextBox)this.CreateUserWizard1.FindWizardControlRecursive("Location");
            var homepageTextBox = (TextBox)this.CreateUserWizard1.FindWizardControlRecursive("Homepage");
            var dstUser = (CheckBox)this.CreateUserWizard1.FindWizardControlRecursive("DSTUser");

            // setup/save the profile
            var userProfile = YafUserProfile.GetProfile(this.CreateUserWizard1.UserName);

            if (country.SelectedValue != null)
            {
                userProfile.Country = country.SelectedValue;
            }

            string result;

            if (this.Get<ISpamWordCheck>().CheckForSpamWord(homepageTextBox.Text.Trim(), out result))
            {
                this.IsPossibleSpamBotInternalCheck = true;

                // Flag user as spam bot
                this.IsPossibleSpamBot = true;

                var userIpAddress = this.Get<HttpRequestBase>().GetUserRealIPAddress();

                if (this.Get<YafBoardSettings>().BotHandlingOnRegister.Equals(1))
                {
                    this.Get<ISendNotification>().SendSpamBotNotificationToAdmins(user, userId);
                }
                else if (this.Get<YafBoardSettings>().BotHandlingOnRegister.Equals(2))
                {
                    // Kill user
                    UserMembershipHelper.DeleteAndBanUser(userId, user, userIpAddress);

                    this.PageContext.AddLoadMessage(this.GetText("BOT_MESSAGE"), MessageTypes.danger);
                }

                this.Logger.Log(
                        null, 
                        "Bot Detected", 
                        "Internal Spam Word Check detected a SPAM BOT: (user name : '{0}', email : '{1}', ip: '{2}') reason word: {3}"
                            .FormatWith(user.UserName, this.CreateUserWizard1.Email, userIpAddress, homepageTextBox.Text.Trim()), 
                        EventLogTypes.SpamBotDetected);
            }

            if (!this.IsPossibleSpamBotInternalCheck)
            {
                userProfile.Location = locationTextBox.Text.Trim();

                // add http:// by default
                if (!Regex.IsMatch(homepageTextBox.Text.Trim(), @"^(http|https|ftp|ftps|git|svn|news)\://.*"))
                {
                    homepageTextBox.Text = "http://{0}".FormatWith(homepageTextBox.Text.Trim());
                }

                if (ValidationHelper.IsValidURL(homepageTextBox.Text))
                {
                    userProfile.Homepage = homepageTextBox.Text.Trim();
                }

                userProfile.Save();

                var autoWatchTopicsEnabled = this.Get<YafBoardSettings>().DefaultNotificationSetting
                                             == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

                // save the time zone...
                LegacyDb.user_save(
                    userID: userId, 
                    boardID: this.PageContext.PageBoardID, 
                    userName: null, 
                    displayName: null, 
                    email: null, 
                    timeZone: timeZones.SelectedValue, 
                    languageFile: null, 
                    culture: null, 
                    themeFile: null, 
                    textEditor: null, 
                    useMobileTheme: null, 
                    approved: null,
                    pmNotification: this.Get<YafBoardSettings>().DefaultNotificationSetting,
                    autoWatchTopics: autoWatchTopicsEnabled,
                    dSTUser: dstUser.Checked, 
                    hideUser: null, 
                    notificationType: null);

                // save the settings...
                LegacyDb.user_savenotification(
                    userId, 
                    true, 
                    autoWatchTopicsEnabled, 
                    this.Get<YafBoardSettings>().DefaultNotificationSetting, 
                    this.Get<YafBoardSettings>().DefaultSendDigestEmail);
            }
        }

        #endregion
    }
}