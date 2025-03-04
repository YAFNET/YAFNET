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

using System.Reflection;

using YAF.Core.Services;

/// <summary>
/// The footer html helper.
/// </summary>
public static class FooterHtmlHelper
{
    /// <summary>
    /// The footer helper.
    /// </summary>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent FooterHelper(this IHtmlHelper _)
    {
        var content = new HtmlContentBuilder();

        RenderRulesLink(content);

        RenderVersion(content);

        RenderGeneratedAndDebug(content);

        return content;
    }

    /// <summary>
    /// Renders the rules link.
    /// </summary>
    /// <param name="content">
    /// The content.
    /// </param>
    private static void RenderRulesLink(HtmlContentBuilder content)
    {
        var rulesTag = new TagBuilder(HtmlTag.A);

        var privacyText = BoardContext.Current.Get<ILocalization>().GetText("COMMON", "PRIVACY_POLICY");

        rulesTag.MergeAttribute(HtmlAttribute.Title, privacyText);
        rulesTag.MergeAttribute(HtmlAttribute.Href,
            BoardContext.Current.Get<ILinkBuilder>().GetLink(ForumPages.Privacy));

        rulesTag.InnerHtml.Append(privacyText);

        rulesTag.TagRenderMode = TagRenderMode.Normal;

        content.AppendHtml(rulesTag);
    }

    /// <summary>
    /// The render version.
    /// </summary>
    /// <param name="content">
    /// The content.
    /// </param>
    private static void RenderVersion(HtmlContentBuilder content)
    {
        if (BoardContext.Current.BoardSettings.HideCopyright)
        {
            return;
        }

        content.AppendHtml(" | ");

        var yafUrlTag = new TagBuilder(HtmlTag.A);

        yafUrlTag.MergeAttribute(HtmlAttribute.Target, "_blank");
        yafUrlTag.MergeAttribute(HtmlAttribute.Title, "YetAnotherForum.NET");
        yafUrlTag.MergeAttribute(HtmlAttribute.Href, "https://www.yetanotherforum.net");

        yafUrlTag.InnerHtml.Append(
            $"{BoardContext.Current.Get<ILocalization>().GetText("COMMON", "POWERED_BY")} YAF.NET");

        if (BoardContext.Current.BoardSettings.ShowYAFVersion)
        {
            yafUrlTag.InnerHtml.AppendHtml($" {BoardContext.Current.Get<BoardInfo>().AppVersionName} ");
        }

        yafUrlTag.TagRenderMode = TagRenderMode.Normal;

        content.AppendHtml(yafUrlTag);

        content.AppendHtml(" | ");

        var yafCopyrightTag = new TagBuilder(HtmlTag.A);

        yafCopyrightTag.MergeAttribute(HtmlAttribute.Target, "_blank");
        yafCopyrightTag.MergeAttribute(HtmlAttribute.Title, "YetAnotherForum.NET");
        yafCopyrightTag.MergeAttribute(HtmlAttribute.Href, "https://www.yetanotherforum.net");

        yafCopyrightTag.InnerHtml.Append($"YAF.NET © 2003-{DateTime.UtcNow.Year} Yet Another Forum.NET");

        yafCopyrightTag.TagRenderMode = TagRenderMode.Normal;

        content.AppendHtml(yafCopyrightTag);
    }

    /// <summary>
    /// The render generated and debug.
    /// </summary>
    /// <param name="content">
    /// The content.
    /// </param>
    private static void RenderGeneratedAndDebug(HtmlContentBuilder content)
    {
        if (BoardContext.Current.BoardSettings.ShowPageGenerationTime)
        {
            var generatedTag = new TagBuilder(HtmlTag.P);

            generatedTag.AddCssClass("text-body-secondary small");

            generatedTag.InnerHtml.Append(
                BoardContext.Current.Get<ILocalization>().GetText("COMMON", "GENERATED")
                    .FormatWith(BoardContext.Current.Get<IStopWatch>().Duration));

            content.AppendHtml(generatedTag);
        }

#if DEBUG
        var debugTextTag = new TagBuilder(HtmlTag.P);

        debugTextTag.AddCssClass("text-danger small");

        debugTextTag.InnerHtml.Append(
            "YAF Compiled in ");

        var strongDebugTag = new TagBuilder(HtmlTag.Strong);

        strongDebugTag.InnerHtml.AppendHtml("DEBUG MODE");

        debugTextTag.InnerHtml.AppendHtml(strongDebugTag);

        debugTextTag.InnerHtml.Append(
            " Recompile in ");

        var strongReleaseTag = new TagBuilder(HtmlTag.Strong);

        strongReleaseTag.InnerHtml.AppendHtml("RELEASE MODE");

        debugTextTag.InnerHtml.AppendHtml(strongReleaseTag);

        debugTextTag.InnerHtml.Append(
            " to remove this information");

        content.AppendHtml(debugTextTag);

        var extensions = BoardContext.Current.Get<IList<Assembly>>("ExtensionAssemblies").Select(a => a.FullName)
            .ToList();

        content.AppendHtml(
            $"""<div style="margin:auto;padding:5px;text-align:right;font-size:7pt;"><span style="color: green">{BoardContext.Current.Get<BoardConfiguration>().ConnectionProviderName}</span></div>"""
        );

        if (extensions.Exists(x => x.Contains(".Module")))
        {
            content.AppendHtml(
                $"""<br /><br />Extensions Loaded: <span style="color: green">{extensions.Where(x => x.Contains(".Module")).ToDelimitedString("<br />")}</span>"""
            );
        }

        content.AppendHtml("</div>");
#endif
    }
}