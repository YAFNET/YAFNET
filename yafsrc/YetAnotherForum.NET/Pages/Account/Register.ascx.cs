/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Pages.Account;

using YAF.Types.EventProxies;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models.Identity;
using YAF.Types.Models;

/// <summary>
/// The User Register Page.
/// </summary>
public partial class Register : AccountPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "Register" /> class.
    /// </summary>
    public Register()
        : base("REGISTER", ForumPages.Account_Register)
    {
    }

    /// <summary>
    ///   Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected => false;

    /// <summary>
    /// Gets or sets a value indicating whether the user is possible spam bot.
    /// </summary>
    /// <value>
    /// <c>true</c> if the user is possible spam bot; otherwise, <c>false</c>.
    /// </value>
    private bool IsPossibleSpamBot { get; set; }

    /// <summary>
    /// Gets the profile definitions.
    /// </summary>
    /// <value>The profile definitions.</value>
    private IList<ProfileDefinition> ProfileDefinitions =>
        field ??= this.GetRepository<ProfileDefinition>().GetByBoardId();

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    override protected void OnPreRender(EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            "passwordStrengthCheckJs",
            JavaScriptBlocks.PasswordStrengthCheckerJs(
                this.Password.ClientID,
                this.ConfirmPassword.ClientID,
                this.PageBoardContext.BoardSettings.MinRequiredPasswordLength,
                this.GetText("PASSWORD_NOTMATCH"),
                this.GetTextFormatted("PASSWORD_MIN", this.PageBoardContext.BoardSettings.MinRequiredPasswordLength),
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
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.PageBoardContext.BoardSettings.DisableRegistrations)
        {
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied);
        }

        if (this.IsPostBack)
        {
            this.BodyRegister.CssClass = "card-body was-validated";
            return;
        }

        // handle the CreateUser Step localization
        this.SetupCreateUserStep();

        if (this.PageBoardContext.IsGuest && !Config.IsAnyPortal && Config.AllowLoginAndLogoff)
        {
            this.LoginButton.Visible = true;
            this.LoginButton.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.Account_Login);
        }

        this.DataBind();

        this.EmailValid.ErrorMessage = this.GetText("PROFILE", "BAD_EMAIL");

        if (!this.ProfileDefinitions.Any())
        {
            return;
        }

        this.CustomProfile.DataSource = this.ProfileDefinitions;
        this.CustomProfile.DataBind();

        if (!Config.IsDotNetNuke)
        {
            this.CustomProfile.Visible = true;
        }
    }

    /// <summary>
    /// Create user
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void RegisterClick(object sender, EventArgs e)
    {
        if (!this.Page.IsValid)
        {
            return;
        }

        if (!this.ValidateUser())
        {
            return;
        }

        var user = new AspNetUsers
                       {
                           Id = Guid.NewGuid().ToString(),
                           ApplicationId = this.PageBoardContext.BoardSettings.ApplicationId,
                           // saved like checked (trimmed)
                           UserName = this.UserName.Text.Trim(),
                           LoweredUserName = this.UserName.Text.Trim().ToLower(),
                           Email = this.Email.Text.Trim(),
                           LoweredEmail = this.Email.Text.Trim().ToLower(),
                           IsApproved = false,
                           EmailConfirmed = false,
                           Profile_Birthday = null
                       };

        // the password as typed (the login uses it as typed)
        var result = this.Get<IAspNetUsersHelper>().Create(user, this.Password.Text);

        if (!result.Succeeded)
        {
            // error of some kind
            this.PageBoardContext.Notify(result.Errors.FirstOrDefault(), MessageTypes.danger);
        }
        else
        {
            // setup initial roles (if any) for this user
            this.Get<IAspNetRolesHelper>().SetupUserRoles(this.PageBoardContext.PageBoardID, user);

            var displayName = this.DisplayName.Text.Trim();

            // create the user in the YAF DB as well as sync roles...
            var userId = this.Get<IAspNetRolesHelper>().CreateForumUser(user, displayName, this.PageBoardContext.PageBoardID);

            if (userId == null)
            {
                // something is seriously wrong here -- redirect to failure...
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Failure);
                return;
            }

            this.SaveCustomProfile(userId.Value);

            var verificationText = "ACCOUNT_CREATED_VERIFICATION";

            if (this.IsPossibleSpamBot)
            {
                if (this.PageBoardContext.BoardSettings.BotHandlingOnRegister.Equals(1))
                {
                    this.Get<ISendNotification>().SendSpamBotNotificationToAdmins(user, userId.Value);
                }
            }
            else
            {
                // handle e-mail verification
                var email = this.Email.Text.Trim();

                try
                {
                    this.Get<ISendNotification>().SendVerificationEmail(user, email, userId);
                }
                catch (Exception x)
                {
                    // the account is created, the verification mail can be sent again on the login page
                    this.Get<ILoggerService>().Error(x, $"Verification mail to {email} failed");

                    verificationText = "ACCOUNT_CREATED_MAIL_FAILED";
                }

                if (this.PageBoardContext.BoardSettings.NotificationOnUserRegisterEmailList.IsSet())
                {
                    // send user register notification to the following admin users...
                    this.Get<ISendNotification>().SendRegistrationNotificationEmail(user, userId.Value);
                }
            }

            this.BodyRegister.Visible = false;
            this.Footer.Visible = false;

            // success notification localization
            this.Message.Visible = true;
            this.AccountCreated.Text = this.Get<IBBCode>().MakeHtml(
                this.GetText(verificationText),
                true,
                true);
        }
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"));
    }

    protected void CustomProfile_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
        {
            return;
        }

        var profileDef = (ProfileDefinition)e.Item.DataItem;

        if (!profileDef.ShowOnRegisterPage)
        {
            return;
        }

        var hidden = e.Item.FindControlAs<HiddenField>("DefID");
        var label = e.Item.FindControlAs<Label>("DefLabel");
        var textBox = e.Item.FindControlAs<TextBox>("DefText");

        var checkPlaceHolder = e.Item.FindControlAs<PlaceHolder>("CheckPlaceHolder");
        var check = e.Item.FindControlAs<CheckBox>("DefCheck");

        hidden.Value = profileDef.ID.ToString();

        label.Text = this.HtmlEncode(profileDef.Name);

        var type = profileDef.DataType.ToEnum<DataType>();

        switch (type)
        {
            case DataType.Text:
                {
                    textBox.MaxLength = profileDef.Length;
                    textBox.CssClass = "form-control";
                    textBox.Visible = true;

                    if (profileDef.DefaultValue.IsSet())
                    {
                        textBox.Text = profileDef.DefaultValue;
                    }

                    if (profileDef.Required)
                    {
                        textBox.Attributes.Add("required", "required");
                    }

                    label.AssociatedControlID = textBox.ID;

                    break;
                }
            case DataType.Number:
                {
                    textBox.TextMode = TextBoxMode.Number;
                    textBox.MaxLength = profileDef.Length;
                    textBox.CssClass = "form-control";
                    textBox.Visible = true;

                    if (profileDef.DefaultValue.IsSet())
                    {
                        textBox.Text = profileDef.DefaultValue;
                    }

                    if (profileDef.Required)
                    {
                        textBox.Attributes.Add("required", "required");
                    }

                    label.AssociatedControlID = textBox.ID;

                    break;
                }
            case DataType.Check:
                {
                    checkPlaceHolder.Visible = true;
                    check.Visible = true;
                    check.Text = this.HtmlEncode(profileDef.Name);

                    // the default value like on the profile page (it was ignored for check fields)
                    if (profileDef.DefaultValue.IsSet())
                    {
                        check.Checked = profileDef.DefaultValue.ToType<bool>();
                    }

                    label.Visible = false;

                    break;
                }
        }
    }

    /// <summary>
    /// The setup create user step.
    /// </summary>
    private void SetupCreateUserStep()
    {
        this.DisplayNamePlaceHolder.Visible = this.PageBoardContext.BoardSettings.EnableDisplayName;
    }

    /// <summary>
    /// Validate user for user name and or display name and spam
    /// </summary>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private bool ValidateUser()
    {
        var userName = this.UserName.Text.Trim();

        // username cannot contain semicolon, be the guest name or a bad word
        if (!UserHelper.IsAllowedName(userName, this.PageBoardContext.PageBoardID))
        {
            this.PageBoardContext.Notify(this.GetText("BAD_USERNAME"), MessageTypes.warning);

            return false;
        }

        if (userName.Length < this.PageBoardContext.BoardSettings.DisplayNameMinLength)
        {
            this.PageBoardContext.Notify(
                this.GetTextFormatted("USERNAME_TOOSMALL", this.PageBoardContext.BoardSettings.DisplayNameMinLength),
                MessageTypes.danger);

            return false;
        }

        if (userName.Length > this.PageBoardContext.BoardSettings.UserNameMaxLength)
        {
            this.PageBoardContext.Notify(
                this.GetTextFormatted("USERNAME_TOOLONG", this.PageBoardContext.BoardSettings.UserNameMaxLength),
                MessageTypes.danger);

            return false;
        }

        if (this.PageBoardContext.BoardSettings.EnableDisplayName && this.DisplayName.Text.Trim().IsSet())
        {
            var displayName = this.DisplayName.Text.Trim();

            // Check if name matches the required minimum length
            if (displayName.Length < this.PageBoardContext.BoardSettings.DisplayNameMinLength)
            {
                this.PageBoardContext.Notify(
                    this.GetTextFormatted("USERNAME_TOOSMALL", this.PageBoardContext.BoardSettings.DisplayNameMinLength),
                    MessageTypes.warning);

                return false;
            }

            // Check if name matches the required minimum length
            if (displayName.Length > this.PageBoardContext.BoardSettings.UserNameMaxLength)
            {
                this.PageBoardContext.Notify(
                    this.GetTextFormatted("USERNAME_TOOLONG", this.PageBoardContext.BoardSettings.UserNameMaxLength),
                    MessageTypes.warning);

                return false;
            }

            // the same rules as for the user name (another user or the guest can not be impersonated)
            if (!UserHelper.IsAllowedName(displayName, this.PageBoardContext.PageBoardID))
            {
                this.PageBoardContext.Notify(this.GetText("BAD_USERNAME"), MessageTypes.warning);

                return false;
            }

            if (this.Get<IUserDisplayName>().FindUserByName(displayName) != null)
            {
                this.PageBoardContext.Notify(
                    this.GetText("ALREADY_REGISTERED_DISPLAYNAME"),
                    MessageTypes.warning);

                return false;
            }
        }

        if (!this.ValidateCustomProfile())
        {
            return false;
        }

        this.IsPossibleSpamBot = false;

        // Check user for bot
        var userIpAddress = this.Get<HttpRequestBase>().GetUserRealIPAddress();

        // Check content for spam
        if (this.Get<ISpamCheck>().CheckUserForSpamBot(userName, this.Email.Text, userIpAddress, out var result))
        {
            // Flag user as spam bot
            this.IsPossibleSpamBot = true;

            this.GetRepository<Registry>().IncrementDeniedRegistrations();

            this.Logger.Log(
                null,
                "Bot Detected",
                $"Bot Check detected a possible SPAM BOT: (user name : '{userName}', email : '{this.Email.Text}', ip: '{userIpAddress}', reason : {result}), user was rejected.",
                EventLogTypes.SpamBotDetected);

            if (this.PageBoardContext.BoardSettings.BanBotIpOnDetection)
            {
                this.Get<IRaiseEvent>().Raise(
                    new BanUserEvent(this.PageBoardContext.PageUserID, userName, this.Email.Text, userIpAddress));
            }

            if (!this.PageBoardContext.BoardSettings.BotHandlingOnRegister.Equals(2))
            {
                return true;
            }

            this.GetRepository<Registry>().IncrementBannedUsers();

            return false;
        }

        return true;
    }

    private bool ValidateCustomProfile()
    {
        // Save Custom Profile
        if (!this.CustomProfile.Visible)
        {
            return true;
        }

        // the required fields and the types are taken from the database (the fields shown on the register page),
        // number fields must be numbers
        var invalidField = CustomProfileHelper.FindInvalidField(
            this.ProfileDefinitions.Where(x => x.ShowOnRegisterPage),
            this.GetPostedCustomProfile(),
            true,
            out var isNumberError);

        if (invalidField is null)
        {
            return true;
        }

        this.PageBoardContext.Notify(
            string.Format(
                this.GetText("EDIT_PROFILE", isNumberError ? "NEED_CUSTOM_NUMBER" : "NEED_CUSTOM"),
                this.HtmlEncode(invalidField.Name)),
            MessageTypes.warning);
        return false;
    }

    /// <summary>
    /// Gets the posted values of the custom profile fields (the ids and the texts).
    /// </summary>
    /// <returns>The posted values.</returns>
    private List<ProfileDefinition> GetPostedCustomProfile()
    {
        return this.CustomProfile.Items.Cast<RepeaterItem>()
            .Where(x => x.ItemType is ListItemType.Item or ListItemType.AlternatingItem).Select(
                item => new ProfileDefinition
                            {
                                ID = item.FindControlAs<HiddenField>("DefID").Value.ToType<int>(),
                                Value = item.FindControlAs<TextBox>("DefText").Text
                            }).ToList();
    }

    /// <summary>
    /// Limits the value to the length of the field (0 = no limit).
    /// </summary>
    /// <param name="profileDef">The field definition.</param>
    /// <param name="value">The value.</param>
    /// <returns>The value of the allowed length.</returns>
    private static string LimitToLength(ProfileDefinition profileDef, string value)
    {
        return profileDef.Length > 0 && value.Length > profileDef.Length ? value.Substring(0, profileDef.Length) : value;
    }

    private void SaveCustomProfile(int userId)
    {
        // Save Custom Profile
        if (this.CustomProfile.Visible)
        {
            this.CustomProfile.Items.Cast<RepeaterItem>().Where(x => x.ItemType is ListItemType.Item or ListItemType.AlternatingItem).ForEach(
                item =>
                    {
                        var id = item.FindControlAs<HiddenField>("DefID").Value.ToType<int>();
                        var profileDef = this.ProfileDefinitions.FirstOrDefault(x => x.ID == id);

                        if (profileDef == null)
                        {
                            return;
                        }

                        var textBox = item.FindControlAs<TextBox>("DefText");
                        var check = item.FindControlAs<CheckBox>("DefCheck");

                        var type = profileDef.DataType.ToEnum<DataType>();

                        switch (type)
                        {
                            case DataType.Text:
                                {
                                    if (textBox.Text.IsSet())
                                    {
                                        this.GetRepository<ProfileCustom>().Insert(
                                            new ProfileCustom
                                                {
                                                    UserID = userId,
                                                    ProfileDefinitionID = profileDef.ID,
                                                    Value = LimitToLength(profileDef, textBox.Text)
                                                });
                                    }

                                    break;
                                }
                            case DataType.Number:
                                {
                                    if (textBox.Text.IsSet())
                                    {
                                        this.GetRepository<ProfileCustom>().Insert(
                                            new ProfileCustom
                                                {
                                                    UserID = userId,
                                                    ProfileDefinitionID = profileDef.ID,
                                                    Value = LimitToLength(profileDef, textBox.Text)
                                                });
                                    }

                                    break;
                                }
                            case DataType.Check:
                                {
                                    this.GetRepository<ProfileCustom>().Insert(
                                        new ProfileCustom
                                            {
                                                UserID = userId,
                                                ProfileDefinitionID = profileDef.ID,
                                                Value = check.Checked.ToString()
                                            });

                                    break;
                                }
                        }
                    });
        }
    }
}