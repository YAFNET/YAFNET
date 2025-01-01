﻿/* Yet Another Forum.NET
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

namespace YAF.Core.Extensions;

using System.Web.UI;

using YAF.Core.BaseControls;

/// <summary>
/// The localization support extensions.
/// </summary>
public static class LocalizationSupportExtensions
{
    /// <summary>
    /// Localizes the specified support item.
    /// </summary>
    /// <param name="supportItem">The support Item.</param>
    /// <param name="currentControl">The current Control.</param>
    /// <returns>
    /// The get current item.
    /// </returns>
    public static string Localize(this ILocalizationSupport supportItem, Control currentControl)
    {
        if (currentControl.Site is { DesignMode: true })
        {
            return $"[PAGE:{supportItem.LocalizedPage}|TAG:{supportItem.LocalizedTag}]";
        }

        if (supportItem.LocalizedPage.IsSet() && supportItem.LocalizedTag.IsSet())
        {
            return BoardContext.Current.Get<ILocalization>()
                .GetText(supportItem.LocalizedPage, supportItem.LocalizedTag);
        }

        return supportItem.LocalizedTag.IsSet()
                   ? BoardContext.Current.Get<ILocalization>().GetText(supportItem.LocalizedTag)
                   : null;
    }

    /// <summary>
    /// Localizes the and render.
    /// </summary>
    /// <param name="supportedItem">The supported item.</param>
    /// <param name="currentControl">The current control.</param>
    /// <returns>
    /// The localize and render.
    /// </returns>
    public static string LocalizeAndRender(
        this ILocalizationSupport supportedItem,
        BaseControl currentControl)
    {
        var localizedItem = supportedItem.Localize(currentControl);

        // convert from BBCode to HTML
        if (supportedItem.EnableBBCode)
        {
            localizedItem = currentControl.Get<IBBCode>().MakeHtml(localizedItem, true, true);
        }

        return localizedItem.FormatWith(supportedItem.Param0, supportedItem.Param1, supportedItem.Param2);
    }
}