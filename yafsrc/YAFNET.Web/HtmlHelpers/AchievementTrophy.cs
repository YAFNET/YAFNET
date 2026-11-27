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

namespace YAF.Web.HtmlHelpers;

using YAF.Types.Objects;

/// <summary>
/// The trophy of an achievement: the icon in a ring in the color of the rarity (see forum/achievements.scss).
/// </summary>
public static class AchievementTrophyHtmlHelper
{
    /// <param name="htmlHelper">The html helper.</param>
    extension(IHtmlHelper htmlHelper)
    {
        /// <summary>
        /// Renders the trophy of an achievement (a locked achievement is gray).
        /// </summary>
        /// <param name="achievement">The achievement.</param>
        /// <param name="size">The size class (achievement-trophy-sm, achievement-trophy-lg) or nothing.</param>
        /// <param name="showLocked">Show a locked achievement as locked.</param>
        /// <returns>The trophy.</returns>
        public IHtmlContent AchievementTrophy(AchievementInfo achievement, string size = null, bool showLocked = true)
        {
            return htmlHelper.AchievementTrophy(
                achievement.Icon,
                achievement.RarityClass,
                showLocked && !achievement.Unlocked,
                size);
        }

        /// <summary>
        /// Renders a trophy.
        /// </summary>
        /// <param name="icon">The icon (one of the achievement icons).</param>
        /// <param name="rarityClass">The class of the rarity (e.g. rarity-epic).</param>
        /// <param name="locked">Show the trophy as locked.</param>
        /// <param name="size">The size class (achievement-trophy-sm, achievement-trophy-lg) or nothing.</param>
        /// <returns>The trophy.</returns>
        public IHtmlContent AchievementTrophy(string icon, string rarityClass, bool locked = false, string size = null)
        {
            var trophy = new TagBuilder(HtmlTag.Span);

            trophy.AddCssClass("achievement-trophy");
            trophy.AddCssClass(rarityClass);

            if (size.IsSet())
            {
                trophy.AddCssClass(size);
            }

            if (locked)
            {
                trophy.AddCssClass("locked");
            }

            trophy.Attributes.Add("aria-hidden", "true");

            var iconTag = new TagBuilder(HtmlTag.I);

            iconTag.AddCssClass($"fas fa-{(AchievementIcons.IsValid(icon) ? icon : AchievementIcons.Default)}");

            trophy.InnerHtml.AppendHtml(iconTag);

            return new HtmlContentBuilder().AppendHtml(trophy);
        }
    }
}
