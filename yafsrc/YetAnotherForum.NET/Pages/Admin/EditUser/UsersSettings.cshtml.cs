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
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// Class UsersSettingsModel.
/// Implements the <see cref="YAF.Core.BasePages.AdminPage" />
/// </summary>
/// <seealso cref="YAF.Core.BasePages.AdminPage" />
public class UsersSettingsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public UsersSettingsInputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersSettingsModel"/> class.
    /// </summary>
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
    /// Gets or sets the languages.
    /// </summary>
    public IReadOnlyCollection<SelectListItem> Languages { get; set; }

    /// <summary>
    /// Gets or sets the page sizes.
    /// </summary>
    public IReadOnlyCollection<SelectListItem> PageSizes { get; set; }

    /// <summary>
    /// Called when [get].
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>IActionResult.</returns>
    public IActionResult OnGet(int userId)
    {
        if (!BoardContext.Current.IsAdmin)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        this.Input = new UsersSettingsInputModel {
            UserId = userId
        };

        return this.BindData(userId);
    }

    /// <summary>
    /// Updates the User Info
    /// </summary>
    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, this.Input.UserId)] is not
            Tuple<User, AspNetUsers, Rank, VAccess> user)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Admin_EditUser,
                new {
                    u = this.Input.UserId
                });
        }

        if (this.Input.Email != user.Item1.Email)
        {
            var newEmail = this.Input.Email.Trim();

            if (!ValidationHelper.IsValidEmail(newEmail))
            {
                this.PageBoardContext.SessionNotify(this.GetText("PROFILE", "BAD_EMAIL"), MessageTypes.warning);
                return this.Get<ILinkBuilder>().Redirect(
                    ForumPages.Admin_EditUser,
                    new { u = this.Input.UserId, tab = "View10" });
            }

            var userFromEmail = await this.Get<IAspNetUsersHelper>().GetUserByEmailAsync(this.Input.Email.Trim());

            if (userFromEmail != null)
            {
                this.PageBoardContext.SessionNotify(this.GetText("PROFILE", "BAD_EMAIL"), MessageTypes.warning);
                return this.Get<ILinkBuilder>().Redirect(
                    ForumPages.Admin_EditUser,
                    new { u = this.Input.UserId, tab = "View10" });
            }

            try
            {
                await this.Get<IAspNetUsersHelper>().UpdateEmailAsync(user.Item2, this.Input.Email.Trim());
            }
            catch (ApplicationException)
            {
                this.PageBoardContext.SessionNotify(
                    this.GetText("PROFILE", "DUPLICATED_EMAIL"),
                    MessageTypes.warning);
                return this.Get<ILinkBuilder>().Redirect(
                    ForumPages.Admin_EditUser,
                    new { u = this.Input.UserId, tab = "View10" });
            }
        }

        string language = null;
        var culture = this.Input.Language;
        var theme = this.Input.Theme;

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
            this.Input.HideMe,
            this.Input.Activity,
            this.Input.Size
        );

        if (user.Item1.UserFlags.IsGuest)
        {
            this.GetRepository<Registry>().Save(
                "timezone",
                this.Input.TimeZone,
                this.PageBoardContext.PageBoardID);
        }

        // clear the cache for this user...
        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.Input.UserId));

        this.Get<IDataCache>().Clear();

        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Admin_EditUser,
            new { u = this.Input.UserId, tab = "View10" });
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private IActionResult BindData(int userId)
    {
        if (this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, userId)] is not Tuple<User, AspNetUsers, Rank, VAccess> user)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Admin_EditUser,
                new {
                    u = this.Input.UserId
                });
        }

        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        this.TimeZones = StaticDataHelper.TimeZones();

        if (this.PageBoardContext.BoardSettings.AllowUserTheme)
        {
            this.Themes = StaticDataHelper.Themes();
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

        if (this.PageBoardContext.BoardSettings.AllowUserTheme && this.Themes.Count != 0)
        {
            // Allows to use different per-forum themes,
            // While "Allow User Change Theme" option in the host settings is true
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
        }

        this.Input.HideMe = user.Item1.UserFlags.IsActiveExcluded
                            && (this.PageBoardContext.BoardSettings.AllowUserHideHimself || this.PageBoardContext.IsAdmin);

        this.Input.Activity = user.Item1.Activity;

        this.Input.Size = user.Item1.PageSize;

        if (!this.PageBoardContext.BoardSettings.AllowUserLanguage || this.Languages.Count == 0)
        {
            return this.Page();
        }

        // If 2-letter language code is the same we return Culture, else we return a default full culture from language file
        this.Input.Language = this.GetCulture(user.Item1);

        return this.Page();
    }

    /// <summary>
    /// Gets the culture.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The get culture.
    /// </returns>
    private string GetCulture(User user)
    {
        // Language and culture
        var languageFile = this.PageBoardContext.BoardSettings.Language;
        var culture4Tag = this.PageBoardContext.BoardSettings.Culture;

        if (user.LanguageFile.IsSet())
        {
            languageFile = user.LanguageFile;
        }

        if (user.Culture.IsSet())
        {
            culture4Tag = user.Culture;
        }

        // Get first default full culture from a language file tag.
        var langFileCulture = StaticDataHelper.CultureDefaultFromFile(languageFile);
        return langFileCulture[..2] == culture4Tag[..2] ? culture4Tag : langFileCulture;
    }
}