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

namespace YAF.Web.HtmlHelpers;

using YAF.Types.Attributes;

/// <summary>
/// Class IconHtmlHelper.
/// </summary>
public static class IconHtmlHelper
{
    /// <summary>
    /// Render Normal Icon
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="iconName">
    /// The icon Name.
    /// </param>
    /// <param name="iconType">
    /// The icon Type.
    /// </param>
    /// <param name="iconStyle">
    /// The icon Style.
    /// </param>
    /// <param name="iconSize">
    /// The icon Size.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent Icon(
        [NotNull] this IHtmlHelper htmlHelper,
        [NotNull] string iconName,
        [CanBeNull] string iconType = "",
        [CanBeNull] string iconStyle = "fas",
        [CanBeNull] string iconSize = "")
    {
        var content = new HtmlContentBuilder();

        var className = iconType.IsSet() ? $"fa-fw {iconType} me-1" : "fa-fw me-1";

        if (iconSize.IsSet())
        {
            className += $" {iconSize}";
        }

        var iconTag = new TagBuilder("i");

        iconTag.AddCssClass($"{iconStyle} fa-{iconName} {className}");

        iconTag.TagRenderMode = TagRenderMode.Normal;

        return content.AppendHtml(iconTag);
    }

    /// <summary>
    /// Render Icon Badge
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="iconName">
    /// The icon Name.
    /// </param>
    /// <param name="iconType">
    /// The icon Type.
    /// </param>
    /// <param name="iconStyle">
    /// The icon Style.
    /// </param>
    /// <param name="iconSize">
    /// The icon Size.
    /// </param>
    /// <param name="iconBadgeName">
    /// The icon Badge Name.
    /// </param>
    /// <param name="iconBadgeType">
    /// The icon Badge Type.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent IconBadge(
        [NotNull] this IHtmlHelper htmlHelper,
        [NotNull] string iconName,
        /*[CanBeNull] string iconType = "",
        [CanBeNull] string iconStyle = "fas",
        [CanBeNull] string iconSize = "",*/
        [NotNull] string iconBadgeName,
        [CanBeNull] string iconBadgeType = "")
    {
        var content = new HtmlContentBuilder();

        var span = new TagBuilder("span");

        span.AddCssClass("fa-stack me-1");

        /*var className = iconType.IsSet() ? $"fa-stack-1x {iconType}" : "fa-stack-1x";

        if (iconSize.IsSet())
        {
            className += $" {iconSize}";
        }*/

        var className = "fa-stack-1x";

        var iconTag = new TagBuilder("i");

        //iconTag.AddCssClass($"{iconStyle} fa-{iconName} {className}");
        iconTag.AddCssClass($"fas fa-{iconName} {className}");

        iconTag.TagRenderMode = TagRenderMode.Normal;

        span.InnerHtml.AppendHtml(iconTag);

        var iconCircleTag = new TagBuilder("i");

        iconCircleTag.AddCssClass("fa fa-circle fa-badge-bg fa-inverse text-light");

        span.InnerHtml.AppendHtml(iconCircleTag);

        var iconBadgeTag = new TagBuilder("i");

        iconBadgeTag.AddCssClass($"fa fa-{iconBadgeName} fa-badge {iconBadgeType}");

        span.InnerHtml.AppendHtml(iconBadgeTag);

        return content.AppendHtml(span);
    }

    /// <summary>
    /// Render Icon Stack
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="iconName">
    /// The icon Name.
    /// </param>
    /// <param name="iconType">
    /// The icon Type.
    /// </param>
    /// <param name="iconStackName">
    /// The icon Stack Name.
    /// </param>
    /// <param name="iconStackType">
    /// The icon Stack Type.
    /// </param>
    /// <param name="iconStackSize">
    /// The icon Stack Size.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent IconStack(
        [NotNull] this IHtmlHelper htmlHelper,
        [NotNull] string iconName,
        [NotNull] string iconType,
        [NotNull] string iconStackName,
        [NotNull] string iconStackType,
        [NotNull] string iconStackSize)
    {
        var content = new HtmlContentBuilder();

        if (iconStackSize.IsNotSet())
        {
            iconStackSize = "fa-2x";
        }

        var span = new TagBuilder("span");

        span.AddCssClass($"fa-stack {iconStackSize} me-1");

        var className = iconType.IsSet() ? $"fas fa-stack-2x {iconType}" : "fas fa-stack-2x";

        var iconTag = new TagBuilder("i");

        iconTag.AddCssClass($"fa-{iconName} {className}");

        iconTag.TagRenderMode = TagRenderMode.Normal;

        span.InnerHtml.AppendHtml(iconTag);

        var iconStackTag = new TagBuilder("i");

        iconStackTag.AddCssClass($"fa fa-{iconStackName} fa-stack-1x {iconStackType}");

        span.InnerHtml.AppendHtml(iconStackTag);

        return content.AppendHtml(span);
    }
}