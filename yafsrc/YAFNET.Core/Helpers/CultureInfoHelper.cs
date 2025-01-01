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

namespace YAF.Core.Helpers;

using YAF.Types.Models;

/// <summary>
/// CultureInfo Helper Class
/// </summary>
public static class CultureInfoHelper
{
    /// <summary>
    /// Gets the culture by user.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <param name="user">The user.</param>
    /// <returns>Returns the CultureInfo.</returns>
    public static CultureInfo GetCultureByUser(BoardSettings settings, User user)
    {
        // Language and culture
        var languageFile = settings.Language;
        var culture4Tag = settings.Culture;

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

        var langTag =  langFileCulture[..2] == culture4Tag[..2] ? culture4Tag : langFileCulture;

        return CultureInfo.CreateSpecificCulture(langTag);
    }
}