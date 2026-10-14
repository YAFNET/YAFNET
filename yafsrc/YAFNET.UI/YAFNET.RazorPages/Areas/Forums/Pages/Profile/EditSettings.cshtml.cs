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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages.Profile;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

using MimeKit;

using Core.Helpers;
using Core.Model;

using Types.EventProxies;
using Types.Extensions;
using Types.Interfaces.Events;
using Types.Interfaces.Identity;
using Types.Models;

using YAF.Core.Extensions;
using YAF.Core.Identity;
using YAF.Core.Services;
using YAF.Types.Models.Identity;

/// <summary>
/// The edit user Settings page
/// </summary>
public class EditSettingsModel : ProfilePage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "EditSettingsModel" /> class.
    /// </summary>
    public EditSettingsModel()
        : base("EDIT_SETTINGS", ForumPages.Profile_EditSettings)
    {
    }

    /// <summary>
    /// Gets or sets the two factor key.
    /// </summary>
    /// <value>The two factor key.</value>
    [BindProperty]
    public string TwoFactorKey { get; set; }

    /// <summary>
    /// Gets or sets the barcode image URL.
    /// </summary>
    /// <value>The barcode image URL.</value>
    [BindProperty]
    public string BarcodeImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the setup code.
    /// </summary>
    /// <value>The setup code.</value>
    [BindProperty]
    public string SetupCode { get; set; }

    /// <summary>
    /// Gets or sets the input code.
    /// </summary>
    /// <value>The input code.</value>
    [BindProperty]
    public string InputCode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [two factor enabled].
    /// </summary>
    /// <value><c>true</c> if [two factor enabled]; otherwise, <c>false</c>.</value>
    [BindProperty]
    public bool TwoFactorEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether activity.
    /// </summary>
    [BindProperty]
    public bool Activity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether hide me.
    /// </summary>
    [BindProperty]
    public bool HideMe { get; set; }

    /// <summary>
    /// Gets or sets the time zone.
    /// </summary>
    [BindProperty]
    public string TimeZone { get; set; }

    /// <summary>
    /// Gets or sets the theme.
    /// </summary>
    [BindProperty]
    public string Theme { get; set; }

    /// <summary>
    /// Gets or sets the language.
    /// </summary>
    [BindProperty]
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    [BindProperty]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the current password (needed to change the email address).
    /// </summary>
    [BindProperty]
    public string CurrentPassword { get; set; }

    /// <summary>
    /// Gets or sets the time zones.
    /// </summary>
    public IReadOnlyCollection<SelectListItem> TimeZones { get; set; }

    /// <summary>
    /// Gets or sets the themes.
    /// </summary>
    public IReadOnlyCollection<SelectListItem> Themes { get; set; }

    /// <summary>
    /// Gets or sets the theme modes.
    /// </summary>
    public IReadOnlyCollection<SelectListItem> ThemeModes { get; set; }

    /// <summary>
    /// Gets or sets the languages.
    /// </summary>
    public IReadOnlyCollection<SelectListItem> Languages { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(
            this.PageBoardContext.PageUser.DisplayOrUserName(),
            this.Get<ILinkBuilder>().GetLink(ForumPages.MyAccount));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("EDIT_SETTINGS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Bind Data
    /// </summary>
    public IActionResult OnGet()
    {
        this.BindData();

        return this.Page();
    }

    /// <summary>
    /// Saves the Updated Profile
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        if (this.PageBoardContext.BoardSettings.AllowEmailChange && this.Email.IsSet()
            && !this.Email.Trim().Equals(this.PageBoardContext.PageUser.Email, StringComparison.OrdinalIgnoreCase))
        {
            var newEmail = this.Email.Trim();

            if (!ValidationHelper.IsValidEmail(newEmail))
            {
                this.BindData();

                return this.PageBoardContext.Notify(this.GetText("EDIT_PROFILE", "BAD_EMAIL"), MessageTypes.warning);
            }

            var userFromEmail = await this.Get<IAspNetUsersHelper>().GetUserByEmailAsync(newEmail);

            if (userFromEmail != null)
            {
                this.BindData();

                return this.PageBoardContext.Notify(this.GetText("EDIT_PROFILE", "DUPLICATED_EMAIL"), MessageTypes.warning);
            }

            // the password is needed, a session alone can not take over the account
            if (!this.IsCurrentPassword(this.PageBoardContext.MembershipUser))
            {
                this.BindData();

                return this.PageBoardContext.Notify(this.GetText("EDIT_PROFILE", "WRONG_PASSWORD"), MessageTypes.warning);
            }

            // the address is changed after it is confirmed (Approve page)
            await this.SendChangeEmailConfirmationAsync(this.PageBoardContext.MembershipUser, newEmail);

            this.PageBoardContext.SessionNotify(
                string.Format(this.GetText("EDIT_PROFILE", "CHANGE_EMAIL_SENT"), this.HtmlEncode(newEmail)),
                MessageTypes.info);
        }

        string language = null;
        var culture = this.Language;
        var theme = this.Theme;

        // only the themes of the list, when the users can choose a theme (the value is sent by the client)
        if (this.Theme.IsNotSet() || !this.PageBoardContext.BoardSettings.AllowUserTheme
                                  || StaticDataHelper.Themes().All(x => x.Value != this.Theme))
        {
            theme = null;
        }

        // only the time zones of the list, otherwise the user keeps the time zone
        var timeZone = StaticDataHelper.TimeZones().Any(x => x.Value == this.TimeZone)
                           ? this.TimeZone
                           : this.PageBoardContext.PageUser.TimeZone;

        if (this.Language.IsNotSet())
        {
            culture = null;
        }
        else
        {
            StaticDataHelper.Cultures().Where(row => culture == row.CultureTag)
                .ForEach(row => language = row.CultureFile);

            // only the cultures of the list (the value is sent by the client)
            if (language is null)
            {
                culture = null;
            }
        }

        // the option is only shown when the users can hide themselves (the value is sent by the client)
        var hideMe = this.HideMe
                     && (this.PageBoardContext.BoardSettings.AllowUserHideHimself || this.PageBoardContext.IsAdmin);

        // save remaining settings to the DB
        await this.GetRepository<User>().SaveAsync(
            this.PageBoardContext.PageUserID,
            this.PageBoardContext.PageUser.UserFlags,
            timeZone,
            language,
            culture,
            theme,
            hideMe,
            this.Activity,
            this.Size);

        if (this.PageBoardContext.PageUser.UserFlags.IsGuest)
        {
            this.GetRepository<Registry>().Save("timezone", timeZone, this.PageBoardContext.PageBoardID);
        }

        // clear the cache for this user...
        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageBoardContext.PageUserID));

        this.Get<IDataCache>().Clear();

        return this.Get<ILinkBuilder>().Redirect(ForumPages.MyAccount);
    }

    /// <summary>
    /// Check Entered 2FA Code, and if correct enable 2FA for the Current User
    /// </summary>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostEnableTwoFaAsync()
    {
        // an enabled 2FA can not be replaced by a new key without the current code (disable it first)
        if (this.PageBoardContext.MembershipUser.TwoFactorEnabled)
        {
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Profile_EditSettings);
        }

        // the password is needed, a session alone can not lock the user out with an own authenticator
        if (!this.IsCurrentPassword(this.PageBoardContext.MembershipUser))
        {
            this.BindData();

            return this.PageBoardContext.Notify(this.GetText("EDIT_PROFILE", "WRONG_PASSWORD"), MessageTypes.warning);
        }

        if (!this.Get<ITwoFactorAuthService>().ValidatePin(this.TwoFactorKey, this.InputCode))
        {
            this.BindData();

            return this.PageBoardContext.Notify(this.GetText("EDIT_SETTINGS", "WRONG_CODE"), MessageTypes.warning);
        }

        await this.Get<IAspNetUsersHelper>().SetTwoFactorEnabledAsync(
            this.PageBoardContext.MembershipUser,
            this.TwoFactorKey);

        // the security stamp changed, the other logins end: this login gets the new stamp
        await this.Get<SignInManager<AspNetUsers>>().RefreshSignInAsync(this.PageBoardContext.MembershipUser);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Profile_EditSettings);
    }

    /// <summary>
    /// Check Entered 2FA Code, and if correct disable 2FA for the Current User
    /// </summary>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostDisableTwoFaAsync()
    {
        // the code must match the stored key of the user (the key of the form is sent by the client)
        var userKey = this.PageBoardContext.MembershipUser.MobilePIN;

        if (!this.PageBoardContext.MembershipUser.TwoFactorEnabled || userKey.IsNotSet()
            || !this.Get<ITwoFactorAuthService>().ValidatePin(userKey, this.InputCode))
        {
            this.BindData();

            return this.PageBoardContext.Notify(this.GetText("EDIT_SETTINGS", "WRONG_CODE"), MessageTypes.warning);
        }

        await this.Get<IAspNetUsersHelper>().SetTwoFactorDisabledAsync(
            this.PageBoardContext.MembershipUser,
            userKey);

        // the security stamp changed, the other logins end: this login gets the new stamp
        await this.Get<SignInManager<AspNetUsers>>().RefreshSignInAsync(this.PageBoardContext.MembershipUser);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Profile_EditSettings);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.PageSizeList = new SelectList(
            StaticDataHelper.PageEntries(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        this.TimeZones = StaticDataHelper.TimeZones();

        if (this.PageBoardContext.BoardSettings.AllowUserTheme)
        {
            this.Themes = StaticDataHelper.Themes();
        }

        if (this.PageBoardContext.BoardSettings.AllowUserLanguage)
        {
            this.Languages = StaticDataHelper.Languages();
        }

        this.Email = this.PageBoardContext.PageUser.Email;

        if (this.TimeZones.Any(x => x.Value == this.PageBoardContext.PageUser.TimeZoneInfo.Id))
        {
            this.TimeZone = this.PageBoardContext.PageUser.TimeZoneInfo.Id;
        }

        if (this.PageBoardContext.BoardSettings.AllowUserTheme && this.Themes.Count != 0)
        {
            // Allows to use different per-forum themes,
            // While "Allow PageUser Change Theme" option in the host settings is true
            var themeFile = this.PageBoardContext.BoardSettings.Theme;

            if (this.PageBoardContext.PageUser.ThemeFile.IsSet())
            {
                themeFile = this.PageBoardContext.PageUser.ThemeFile;
            }

            if (this.Themes.Any(x => x.Value == themeFile) || this.Themes.Any(x => x.Value == "yaf"))
            {
                this.Theme = themeFile;
            }
        }

        this.HideMe = this.PageBoardContext.PageUser.UserFlags.IsActiveExcluded
                      && (this.PageBoardContext.BoardSettings.AllowUserHideHimself || this.PageBoardContext.IsAdmin);

        this.Activity = this.PageBoardContext.PageUser.Activity;

        this.RenderTwoFactor();

        if (!this.PageBoardContext.BoardSettings.AllowUserLanguage || this.Languages.Count == 0)
        {
            return;
        }

        // If 2-letter language code is the same we return Culture, else we return a default full culture from language file
        this.Language = this.GetCulture();
    }

    /// <summary>
    /// Renders the two factor authentication UI if user has enabled or disabled the setting.
    /// </summary>
    private void RenderTwoFactor()
    {
        this.TwoFactorEnabled = this.PageBoardContext.MembershipUser.TwoFactorEnabled;

        if (this.TwoFactorEnabled)
        {
            // Render Disable 2FA UI, the stored key is never sent to the browser
            this.TwoFactorKey = null;
        }
        else
        {
            // Render Enable 2FA UI, a new key is kept when the form is shown again (wrong code or password),
            // so the authenticator that was just set up still matches
            if (this.TwoFactorKey.IsNotSet())
            {
                this.TwoFactorKey = this.Get<ITwoFactorAuthService>().CreateAccountSecretKey(this.PageBoardContext.PageUser);
            }

            var setupInfo = this.Get<ITwoFactorAuthService>().GenerateSetupCode(this.TwoFactorKey);

            this.SetupCode = setupInfo.ManualEntryKey;
            this.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
        }
    }

    /// <summary>
    /// Gets the culture.
    /// </summary>
    /// <returns>
    /// The get culture.
    /// </returns>
    private string GetCulture()
    {
        // Language and culture
        var languageFile = this.PageBoardContext.BoardSettings.Language;
        var culture4Tag = this.PageBoardContext.BoardSettings.Culture;

        if (this.PageBoardContext.PageUser.LanguageFile.IsSet())
        {
            languageFile = this.PageBoardContext.PageUser.LanguageFile;
        }

        if (this.PageBoardContext.PageUser.Culture.IsSet())
        {
            culture4Tag = this.PageBoardContext.PageUser.Culture;
        }

        // Get first default full culture from a language file tag.
        // the language file can be missing (e.g. removed), use the default culture then
        var langFileCulture = StaticDataHelper.CultureDefaultFromFile(languageFile) ?? "en-US";
        return langFileCulture[..2] == culture4Tag[..2] ? culture4Tag : langFileCulture;
    }

    /// <summary>
    /// Checks the entered current password (accounts of external logins have no password).
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Returns if the password is correct.</returns>
    private bool IsCurrentPassword(AspNetUsers user)
    {
        if (user.PasswordHash.IsNotSet())
        {
            return true;
        }

        return this.CurrentPassword.IsSet() && this.Get<IAspNetUsersHelper>().PasswordHasher
                   .VerifyHashedPassword(user, user.PasswordHash, this.CurrentPassword) != PasswordVerificationResult.Failed;
    }

    /// <summary>
    /// Sends the confirmation link for the new email address.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="newEmail">The new email address.</param>
    private async Task SendChangeEmailConfirmationAsync(AspNetUsers user, string newEmail)
    {
        var token = await this.Get<AspNetUsersManager>().GenerateChangeEmailTokenAsync(user, newEmail);

        var code = HttpUtility.UrlEncode(token, Encoding.UTF8);

        // only the latest request is valid
        await this.GetRepository<CheckEmail>().DeleteAsync(c => c.UserID == this.PageBoardContext.PageUserID);
        await this.GetRepository<CheckEmail>().SaveAsync(this.PageBoardContext.PageUserID, code, newEmail);

        var userName = this.PageBoardContext.PageUser.DisplayOrUserName();

        var changeEmail = new TemplateEmail("CHANGEEMAIL")
                          {
                              TemplateParams =
                              {
                                  ["{user}"] = userName,
                                  ["{newemail}"] = newEmail,
                                  ["{link}"] = this.Get<ILinkBuilder>().GetAbsoluteLink(
                                      ForumPages.Account_Approve,
                                      new { code }),
                                  ["{key}"] = code
                              }
                          };

        await changeEmail.SendEmailAsync(
            new MailboxAddress(userName, newEmail),
            string.Format(this.GetText("EDIT_PROFILE", "CHANGE_EMAIL_SUBJECT"), this.PageBoardContext.BoardSettings.Name));
    }
}