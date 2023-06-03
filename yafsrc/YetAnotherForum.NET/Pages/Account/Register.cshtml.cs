/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
 * https://www.yetanotherforum.net/
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

namespace YAF.Pages.Account;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

using DataType = System.ComponentModel.DataAnnotations.DataType;

/// <summary>
/// The User Register Page.
/// </summary>
[AllowAnonymous]
public class RegisterModel : AccountPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterModel"/> class.
    /// </summary>
    public RegisterModel()
        : base("REGISTER", ForumPages.Account_Register)
    {
    }

    /// <summary>
    ///   Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected => false;

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    [TempData]
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is possible spam bot.
    /// </summary>
    /// <value>
    /// <c>true</c> if the user is possible spam bot; otherwise, <c>false</c>.
    /// </value>
    private bool IsPossibleSpamBot { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.GetText("REGISTER","TITLE"));
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public IActionResult OnGet()
    {
        this.Input = new InputModel();

        if (this.ErrorMessage.IsSet())
        {
            this.ModelState.AddModelError(string.Empty, this.ErrorMessage);
        }

        this.LoadCustomProfile();

        return this.Page();
    }

    /// <summary>
    /// The on post async.
    /// </summary>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IActionResult> OnPostAsync()
    {
        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        if (!await this.ValidateUserAsync().ConfigureAwait(false))
        {
            return this.Page();
        }

        var user = new AspNetUsers
                   {
                       Id = Guid.NewGuid().ToString(),
                       ApplicationId = this.PageBoardContext.BoardSettings.ApplicationId,
                       UserName = this.Input.UserName,
                       LoweredUserName = this.Input.UserName,
                       Email = this.Input.Email,
                       IsApproved = false,
                       EmailConfirmed = false,
                       Profile_Birthday = null
                   };

        var result = this.Get<IAspNetUsersHelper>().Create(user, this.Input.Password.Trim());

        if (!result.Succeeded)
        {
            // error of some kind
            return this.PageBoardContext.Notify(result.Errors.FirstOrDefault()?.Description, MessageTypes.danger);
        }

        // setup initial roles (if any) for this user
        this.Get<IAspNetRolesHelper>().SetupUserRoles(this.PageBoardContext.PageBoardID, user);

        var displayName = this.Input.DisplayName;

        // create the user in the YAF DB as well as sync roles...
        var userId = this.Get<IAspNetRolesHelper>().CreateForumUser(user, displayName, this.PageBoardContext.PageBoardID);

        if (userId == null)
        {
            // something is seriously wrong here -- redirect to failure...
            return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Failure);
        }

        this.SaveCustomProfile(userId.Value);

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
            var email = this.Input.Email.Trim();

            this.Get<ISendNotification>().SendVerificationEmail(user, email, userId);

            if (this.PageBoardContext.BoardSettings.NotificationOnUserRegisterEmailList.IsSet())
            {
                // send user register notification to the following admin users...
                this.Get<ISendNotification>().SendRegistrationNotificationEmail(user, userId.Value);
            }
        }

        this.ErrorMessage = this.Get<IBBCodeService>().MakeHtml(
            this.GetText("ACCOUNT_CREATED_VERIFICATION"),
            true,
            true);

        return this.Page();
    }

    /// <summary>
    /// Validate user for user name and or display name, captcha and spam
    /// </summary>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private async Task<bool> ValidateUserAsync()
    {
        var userName = this.Input.UserName.Trim();

        // username cannot contain semi-colon or to be a bad word
        var badWord = this.Get<IBadWordReplace>().ReplaceItems.Any(
            i => userName.Equals(i.BadWord, StringComparison.CurrentCultureIgnoreCase));

        var guestUserName = this.Get<IAspNetUsersHelper>().GuestUser(this.PageBoardContext.PageBoardID).Name;

        guestUserName = guestUserName.IsSet() ? guestUserName.ToLower() : string.Empty;

        if (userName.Contains(';') || badWord || userName.ToLower().Equals(guestUserName))
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

        if (this.PageBoardContext.BoardSettings.EnableDisplayName && this.Input.DisplayName.IsSet())
        {
            var displayName = this.Input.DisplayName.Trim();

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

            if (this.Get<IUserDisplayName>().FindUserByName(displayName.Trim()) != null)
            {
                this.PageBoardContext.Notify(
                    this.GetText("ALREADY_REGISTERED_DISPLAYNAME"),
                    MessageTypes.warning);
            }
        }

        if (!this.ValidateCustomProfile())
        {
            return false;
        }

        this.IsPossibleSpamBot = false;

        // Check user for bot
        var userIpAddress = this.HttpContext.GetUserRealIPAddress();

        var check = await this.Get<ISpamCheck>().CheckUserForSpamBotAsync(userName, this.Input.Email, userIpAddress);

        // Check content for spam
        if (check.IsBot)
        {
            // Flag user as spam bot
            this.IsPossibleSpamBot = true;

            this.GetRepository<Registry>().IncrementDeniedRegistrations();

            this.Get<ILogger<RegisterModel>>().Log(
                null,
                "Bot Detected",
                $@"Bot Check detected a possible SPAM BOT: (user name : '{userName}', 
                                  email : '{this.Input.Email}', ip: '{userIpAddress}', reason : {check.Result}), user was rejected.",
                EventLogTypes.SpamBotDetected);

            if (this.PageBoardContext.BoardSettings.BanBotIpOnDetection)
            {
                this.Get<IRaiseEvent>().Raise(
                    new BanUserEvent(this.PageBoardContext.PageUserID, userName, this.Input.Email, userIpAddress));
            }

            if (this.PageBoardContext.BoardSettings.BotHandlingOnRegister.Equals(2))
            {
                this.GetRepository<Registry>().IncrementBannedUsers();

                return false;
            }
        }

        return true;
    }

    private bool ValidateCustomProfile()
    {
        // Save Custom Profile
        if (this.Input.CustomProfile.NullOrEmpty())
        {
            return true;
        }

        if (!(from item in this.Input.CustomProfile
              let type = item.DataType.ToEnum<Types.Constants.DataType>()
              where item.Required && type is Types.Constants.DataType.Number or Types.Constants.DataType.Text
              select item).Any(item => item.Value.IsNotSet()))
        {
            return true;
        }

        this.PageBoardContext.Notify(this.GetText("NEED_CUSTOM"), MessageTypes.warning);
        return false;
    }

    /// <summary>
    /// Load the custom profile.
    /// </summary>
    private void LoadCustomProfile()
    {
        this.Input.CustomProfile = this.GetRepository<ProfileDefinition>().GetByBoardId().ToList();

        if (this.Input.CustomProfile == null || !this.Input.CustomProfile.Any())
        {
            return;
        }

        this.Input.CustomProfile.Where(profileDef => profileDef.DefaultValue.IsSet()).ForEach(
            profileDef =>
            {
                profileDef.Value = profileDef.DefaultValue;
            });
    }

    /// <summary>
    /// Saves the custom profile.
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    private void SaveCustomProfile(int userId)
    {
        if (this.Input.CustomProfile.NullOrEmpty())
        {
            return;
        }

        this.Input.CustomProfile.ForEach(
            item =>
            {
                var type = item.DataType.ToEnum<Types.Constants.DataType>();

                switch (type)
                {
                    case Types.Constants.DataType.Text:
                    {
                        if (item.Value.IsSet())
                        {
                            this.GetRepository<ProfileCustom>().Insert(
                                new ProfileCustom
                                {
                                    UserID = userId,
                                    ProfileDefinitionID = item.ID,
                                    Value = item.Value
                                });
                        }

                        break;
                    }

                    case Types.Constants.DataType.Number:
                    {
                        if (item.Value.IsSet())
                        {
                            this.GetRepository<ProfileCustom>().Insert(
                                new ProfileCustom
                                {
                                    UserID = userId,
                                    ProfileDefinitionID = item.ID,
                                    Value = item.Value
                                });
                        }

                        break;
                    }

                    case Types.Constants.DataType.Check:
                    {
                        this.GetRepository<ProfileCustom>().Insert(
                            new ProfileCustom
                            {
                                UserID = userId,
                                ProfileDefinitionID = item.ID,
                                Value = item.Value
                            });

                        break;
                    }
                }
            });
    }

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the confirm password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets the custom profile.
        /// </summary>
        public List<ProfileDefinition> CustomProfile { get; set; }
    }
}