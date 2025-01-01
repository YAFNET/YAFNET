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

namespace YAF.Web.HtmlHelpers;

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
    /// <param name="marginEnd">
    /// Add margin end to icon
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent Icon(
        this IHtmlHelper htmlHelper,
        string iconName,
        string iconType = "",
        string iconStyle = "fas",
        string iconSize = "",
        bool marginEnd = true)
    {
        var content = new HtmlContentBuilder();

        var className = new StringBuilder();

        className.Append(iconType.IsSet() ? $"fa-fw {iconType}" : "fa-fw");

        if (marginEnd)
        {
            className.Append(" me-1");
        }

        if (iconSize.IsSet())
        {
            className.Append($" {iconSize}");
        }

        var iconTag = new TagBuilder(HtmlTag.I);

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
        this IHtmlHelper htmlHelper,
        string iconName,
        string iconBadgeName,
        string iconBadgeType = "")
    {
        var content = new HtmlContentBuilder();

        var span = new TagBuilder(HtmlTag.Span);

        span.AddCssClass($"fa-stack-badge me-1 {iconBadgeType}");

        var iconTag = new TagBuilder(HtmlTag.I);

        iconTag.AddCssClass($"fas fa-{iconName} fa-stack-badge-1x");

        iconTag.TagRenderMode = TagRenderMode.Normal;

        span.InnerHtml.AppendHtml(iconTag);

        var iconBadgeTag = new TagBuilder(HtmlTag.I);

        iconBadgeTag.AddCssClass(
            iconBadgeType.IsSet() ? $"fa fa-{iconBadgeName} fa-badge {iconBadgeType}" : $"fa fa-{iconBadgeName} fa-badge");

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
        this IHtmlHelper htmlHelper,
        string iconName,
        string iconType,
        string iconStackName,
        string iconStackType,
        string iconStackSize)
    {
        var content = new HtmlContentBuilder();

        if (iconStackSize.IsNotSet())
        {
            iconStackSize = "fa-2x";
        }

        var span = new TagBuilder(HtmlTag.Span);

        span.AddCssClass($"fa-stack {iconStackSize} me-1");

        var className = iconType.IsSet() ? $"fas fa-stack-2x {iconType}" : "fas fa-stack-2x";

        var iconTag = new TagBuilder(HtmlTag.I);

        iconTag.AddCssClass($"fa-{iconName} {className}");

        iconTag.TagRenderMode = TagRenderMode.Normal;

        span.InnerHtml.AppendHtml(iconTag);

        var iconStackTag = new TagBuilder(HtmlTag.I);

        iconStackTag.AddCssClass($"fa fa-{iconStackName} fa-stack-1x {iconStackType}");

        span.InnerHtml.AppendHtml(iconStackTag);

        return content.AppendHtml(span);
    }
}