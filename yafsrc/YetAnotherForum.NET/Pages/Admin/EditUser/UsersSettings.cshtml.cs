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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages.Admin.EditUser;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Context;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

public class UsersSettingsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    public UsersSettingsModel()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

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
    /// Gets or sets the page sizes.
    /// </summary>
    public IReadOnlyCollection<SelectListItem> PageSizes { get; set; }

    public IActionResult OnGet(int userId)
    {
        if (!BoardContext.Current.IsAdmin)
        {
            return this.Get<LinkBuilder>().AccessDenied();
        }

        this.Input = new InputModel
                     {
                         UserId = userId
                     };

        this.BindData(userId);

        return this.Page();
    }

    /// <summary>
    /// Updates the User Info
    /// </summary>
    public async Task<IActionResult> OnPostSaveAsync()
    {
        var user =
            this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, this.Input.UserId)] as
                Tuple<User, AspNetUsers, Rank, VAccess>;

        if (this.Input.Email != user.Item1.Email)
        {
            var newEmail = this.Input.Email.Trim();

            if (!ValidationHelper.IsValidEmail(newEmail))
            {
                this.PageBoardContext.SessionNotify(this.GetText("PROFILE", "BAD_EMAIL"), MessageTypes.warning); 
                return this.Get<LinkBuilder>().Redirect(
                    ForumPages.Admin_EditUser,
                    new { u = this.Input.UserId, tab = "View10" });
            }

            var userFromEmail = await this.Get<IAspNetUsersHelper>().GetUserByEmailAsync(this.Input.Email.Trim());

            if (userFromEmail != null)
            {
                this.PageBoardContext.SessionNotify(this.GetText("PROFILE", "BAD_EMAIL"), MessageTypes.warning);
                return this.Get<LinkBuilder>().Redirect(
                    ForumPages.Admin_EditUser,
                    new { u = this.Input.UserId, tab = "View10" });
            }

            try
            {
                await this.Get<IAspNetUsersHelper>().UpdateEmailAsync(this.PageBoardContext.MembershipUser, this.Input.Email.Trim());
            }
            catch (ApplicationException)
            {
                this.PageBoardContext.SessionNotify(
                    this.GetText("PROFILE", "DUPLICATED_EMAIL"),
                    MessageTypes.warning);
                return this.Get<LinkBuilder>().Redirect(
                    ForumPages.Admin_EditUser,
                    new { u = this.Input.UserId, tab = "View10" });
            }
        }

        // vzrus: We should do it as we need to write null value to db, else it will be empty.
        // Localizer currently treats only nulls.
        string language = null;
        var culture = this.Input.Language;
        var theme = this.Input.Theme;
        var themeMode = this.Input.ThemeMode.ToEnum<ThemeMode>();

        if (this.Input.Theme.IsNotSet())
        {
            theme = null;
        }

        if (this.Input.Language.IsNotSet())
        {
            culture = null;
        }
        else
        {
            StaticDataHelper.Cultures()
                .Where(row => culture == row.CultureTag).ForEach(
                    row => language = row.CultureFile);
        }

        // save remaining settings to the DB
        this.GetRepository<User>().Save(
            this.Input.UserId,
            this.Input.TimeZone,
            language,
            culture,
            theme, 
            themeMode,
            this.Input.HideMe,
            this.Input.Activity,
            this.Input.Size
        );

        if (this.PageBoardContext.PageUser.UserFlags.IsGuest)
        {
            this.GetRepository<Registry>().Save(
                "timezone",
                this.Input.TimeZone,
                this.PageBoardContext.PageBoardID);
        }

        // clear the cache for this user...)
        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.Input.UserId));

        this.Get<IDataCache>().Clear();

        return this.Get<LinkBuilder>().Redirect(
            ForumPages.Admin_EditUser,
            new { u = this.Input.UserId, tab = "View10" });
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData(int userId)
    {
        var user =
            this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, userId)] as
                Tuple<User, AspNetUsers, Rank, VAccess>;

        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        this.TimeZones = StaticDataHelper.TimeZones();

        if (this.PageBoardContext.BoardSettings.AllowUserTheme)
        {
            this.Themes = StaticDataHelper.Themes();
            this.ThemeModes = StaticDataHelper.ThemeModes();
        }

        if (this.PageBoardContext.BoardSettings.AllowUserLanguage)
        {
            this.Languages = StaticDataHelper.Languages();
        }

        this.PageSizes = StaticDataHelper.PageEntries();

        this.Input.Email = user.Item1.Email;

        if (this.TimeZones.Any(x => x.Value == user.Item1.TimeZoneInfo.Id))
        {
            this.Input.TimeZone = user.Item1.TimeZoneInfo.Id;
        }

        if (this.PageBoardContext.BoardSettings.AllowUserTheme && this.Themes.Any())
        {
            // Allows to use different per-forum themes,
            // While "Allow PageUser Change Theme" option in the host settings is true
            var themeFile = this.PageBoardContext.BoardSettings.Theme;

            if (user.Item1.ThemeFile.IsSet())
            {
                themeFile = user.Item1.ThemeFile;
            }

            if (this.Themes.Any(x => x.Value == themeFile))
            {
                this.Input.Theme = themeFile;
            }
            else
            {
                if (this.Themes.Any(x => x.Value == "yaf"))
                {
                    this.Input.Theme = themeFile;
                }
            }

            if (this.ThemeModes.Any(x => x.Value == user.Item1.DarkMode.ToType<int>().ToString()))
            {
                this.Input.ThemeMode = this.PageBoardContext.PageUser.DarkMode.ToType<int>();
            }
        }

        this.Input.HideMe = user.Item1.UserFlags.IsActiveExcluded
                            && this.PageBoardContext.BoardSettings.AllowUserHideHimself;

        this.Input.Activity = user.Item1.Activity;

        this.Input.Size = user.Item1.PageSize;

        if (!this.PageBoardContext.BoardSettings.AllowUserLanguage || !this.Languages.Any())
        {
            return;
        }

        // If 2-letter language code is the same we return Culture, else we return a default full culture from language file
        this.Input.Language = this.GetCulture();
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

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether activity.
        /// </summary>
        public bool Activity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether hide me.
        /// </summary>
        public bool HideMe { get; set; }

        /// <summary>
        /// Gets or sets the time zone.
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        public int ThemeMode { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        public int Size { get; set; }
    }
}