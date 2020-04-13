/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.Extensions
{
    using System.Web.UI;

    using ServiceStack;

    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Core.Context;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    /// <summary>
    /// The localization support extensions.
    /// </summary>
    public static class LocalizationSupportExtensions
    {
        #region Public Methods

        /// <summary>
        /// Localizes the specified support item.
        /// </summary>
        /// <param name="supportItem">The support Item.</param>
        /// <param name="currentControl">The current Control.</param>
        /// <returns>
        /// The get current item.
        /// </returns>
        public static string Localize([NotNull] this ILocalizationSupport supportItem, [NotNull] Control currentControl)
        {
            CodeContracts.VerifyNotNull(supportItem, "supportItem");
            CodeContracts.VerifyNotNull(currentControl, "currentControl");

            if (currentControl.Site != null && currentControl.Site.DesignMode)
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
            [NotNull] this ILocalizationSupport supportedItem,
            [NotNull] BaseControl currentControl)
        {
            CodeContracts.VerifyNotNull(supportedItem, "supportedItem");
            CodeContracts.VerifyNotNull(currentControl, "currentControl");

            var localizedItem = supportedItem.Localize(currentControl);

            // convert from BBCode to HTML
            if (supportedItem.EnableBBCode)
            {
                localizedItem = currentControl.Get<IBBCode>().MakeHtml(localizedItem, true, true);
            }

            return localizedItem.Fmt(supportedItem.Param0, supportedItem.Param1, supportedItem.Param2);
        }

        #endregion
    }
}