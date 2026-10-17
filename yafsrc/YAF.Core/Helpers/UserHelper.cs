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

namespace YAF.Core.Helpers;

using System;

using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;

/// <summary>
/// The user helper.
/// </summary>
public static class UserHelper
{
    /// <summary>
    /// Gets the user language file.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// language file name. If null -- use default language
    /// </returns>
    public static string GetUserLanguageFile(User user)
    {
        if (user != null && user.LanguageFile.IsSet() && BoardContext.Current.BoardSettings.AllowUserLanguage)
        {
            return user.LanguageFile;
        }

        // the language of the board (not of the current user, e.g. the admin who sends a notification)
        return BoardContext.Current.BoardSettings.Language;
    }

    /// <summary>
    /// Checks if a name can be used as user name or display name: no semicolon,
    /// not the name of the guest user and not a bad word (another user can't be impersonated).
    /// </summary>
    /// <param name="name">The user name or display name.</param>
    /// <param name="boardId">The board identifier.</param>
    /// <returns>Returns if the name is allowed.</returns>
    public static bool IsAllowedName(string name, int boardId)
    {
        name = name?.Trim();

        if (name.IsNotSet() || name.Contains(";"))
        {
            return false;
        }

        var guestName = BoardContext.Current.Get<IAspNetUsersHelper>().GuestUser(boardId)?.Name;

        if (guestName.IsSet() && name.Equals(guestName.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !BoardContext.Current.Get<IBadWordReplace>().ReplaceItems.Any(
                   i => name.Equals(i.BadWord, StringComparison.CurrentCultureIgnoreCase));
    }

    /// <summary>
    /// Gets the Guest User Language File based on the current Browser Language
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public static string GetGuestUserLanguageFile()
    {
        var languages = BoardContext.Current.Get<IDataCache>().GetOrSet(
            "Languages",
            StaticDataHelper.NeutralCultures,
            TimeSpan.FromDays(30));

        var languageRow = languages.FirstOrDefault(
            row => row.CultureTag.Equals(CultureInfo.CurrentCulture.TwoLetterISOLanguageName));

        return languageRow != null ? languageRow.CultureFile : BoardContext.Current.BoardSettings.Language;
    }
}