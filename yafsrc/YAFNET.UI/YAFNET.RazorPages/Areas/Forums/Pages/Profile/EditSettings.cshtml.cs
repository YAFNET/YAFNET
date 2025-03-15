/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;

using Core.Helpers;
using Core.Model;

using Types.EventProxies;
using Types.Extensions;
using Types.Interfaces.Events;
using Types.Interfaces.Identity;
using Types.Models;

using YAF.Core.Extensions;

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

    [BindProperty]
    public string TwoFactorKey { get; set; }

    [BindProperty]
    public string BarcodeImageUrl { get; set; }

    [BindProperty]
    public string SetupCode { get; set; }

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
        if (this.Email != this.PageBoardContext.PageUser.Email)
        {
            var newEmail = this.Email.Trim();

            if (!ValidationHelper.IsValidEmail(newEmail))
            {
                return this.PageBoardContext.Notify(this.GetText("PROFILE", "BAD_EMAIL"), MessageTypes.warning);
            }

            var userFromEmail = await this.Get<IAspNetUsersHelper>().GetUserByEmailAsync(this.Email.Trim());

            if (userFromEmail != null)
            {
                return this.PageBoardContext.Notify(this.GetText("PROFILE", "BAD_EMAIL"), MessageTypes.warning);
            }

            try
            {
                await this.Get<IAspNetUsersHelper>().UpdateEmailAsync(
                    this.PageBoardContext.MembershipUser,
                    this.Email.Trim());
            }
            catch (ApplicationException)
            {
                this.PageBoardContext.Notify(this.GetText("PROFILE", "DUPLICATED_EMAIL"), MessageTypes.warning);

                this.BindData();
            }
        }

        string language = null;
        var culture = this.Language;
        var theme = this.Theme;

        if (this.Theme.IsNotSet())
        {
            theme = null;
        }

        if (this.Language.IsNotSet())
        {
            culture = null;
        }
        else
        {
            StaticDataHelper.Cultures().Where(row => culture == row.CultureTag)
                .ForEach(row => language = row.CultureFile);
        }

        // save remaining settings to the DB
        this.GetRepository<User>().Save(
            this.PageBoardContext.PageUserID,
            this.TimeZone,
            language,
            culture,
            theme,
            this.HideMe,
            this.Activity,
            this.Size);

        if (this.PageBoardContext.PageUser.UserFlags.IsGuest)
        {
            this.GetRepository<Registry>().Save("timezone", this.TimeZone, this.PageBoardContext.PageBoardID);
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
        if (!this.Get<ITwoFactorAuthService>().ValidatePin(this.TwoFactorKey, this.InputCode))
        {
            this.BindData();

            return this.PageBoardContext.Notify(this.GetText("EDIT_SETTINGS", "WRONG_CODE"), MessageTypes.warning);
        }

        await this.Get<IAspNetUsersHelper>().SetTwoFactorEnabledAsync(
            this.PageBoardContext.MembershipUser,
            this.TwoFactorKey);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Profile_EditSettings);
    }

    /// <summary>
    /// Check Entered 2FA Code, and if correct disable 2FA for the Current User
    /// </summary>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostDisableTwoFaAsync()
    {
        if (!this.Get<ITwoFactorAuthService>().ValidatePin(this.TwoFactorKey, this.InputCode))
        {
            this.BindData();

            return this.PageBoardContext.Notify(this.GetText("EDIT_SETTINGS", "WRONG_CODE"), MessageTypes.warning);
        }

        await this.Get<IAspNetUsersHelper>().SetTwoFactorDisabledAsync(
            this.PageBoardContext.MembershipUser,
            this.TwoFactorKey);

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

            if (this.Themes.Any(x => x.Value == themeFile))
            {
                this.Theme = themeFile;
            }
            else
            {
                if (this.Themes.Any(x => x.Value == "yaf"))
                {
                    this.Theme = themeFile;
                }
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
            // Render Disable 2FA UI
            this.TwoFactorKey = this.PageBoardContext.MembershipUser.MobilePIN;
        }
        else
        {
            // Render Enable 2FA UI

            this.TwoFactorKey = this.Get<ITwoFactorAuthService>().CreateAccountSecretKey(this.PageBoardContext.PageUser);

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
        var langFileCulture = StaticDataHelper.CultureDefaultFromFile(languageFile);
        return langFileCulture[..2] == culture4Tag[..2] ? culture4Tag : langFileCulture;
    }
}