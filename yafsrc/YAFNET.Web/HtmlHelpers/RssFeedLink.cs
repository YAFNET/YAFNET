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

using Microsoft.AspNetCore.Mvc;

namespace YAF.Web.HtmlHelpers;

/// <summary>
/// The RSS Feed link html helper.
/// </summary>
public static class RssFeedLinkHtmlHelper
{
    /// <summary>
    /// The RSS feed link.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <param name="feedType">
    /// The feed Type.
    /// </param>
    /// <param name="renderAsButton">
    /// The render As Button.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent RssFeedLink(
        this IHtmlHelper htmlHelper,
        RssFeeds feedType,
        bool renderAsButton = false)
    {
        var context = BoardContext.Current;

        var content = new HtmlContentBuilder();

        if (!context.BoardSettings.ShowAtomLink)
        {
            return content;
        }

        // setup the rss link...
        var link = renderAsButton ? new TagBuilder(HtmlTag.A) : new TagBuilder(HtmlTag.Link);

        var url = context.CurrentForumPage.PageName switch
            {
                ForumPages.Topics => context.Get<IUrlHelper>()
                    .Action("GetTopicsFeed", "Feed", new {f = context.PageForumID}),
                ForumPages.Posts => context.Get<IUrlHelper>()
                    .Action("GetPostsFeed", "Feed", new {t = context.PageTopicID}),
                _ => context.Get<IUrlHelper>().Action("GetLatestPosts", "Feed")
            };

        link.MergeAttribute(HtmlAttribute.Href, url);

        link.MergeAttribute(HtmlAttribute.Rel, "alternate");
        link.MergeAttribute(HtmlAttribute.Type, "application/atom+xml");
        link.MergeAttribute(HtmlAttribute.Title, $"{context.Get<ILocalization>().GetText("ATOMFEED")} &#183; {context.BoardSettings.Name}");

        if (renderAsButton)
        {
            link.AddCssClass("btn btn-warning btn-sm");

            link.MergeAttribute(HtmlAttribute.Role, HtmlTag.Button);
            link.MergeAttribute("data-bs-html", "true");
            link.MergeAttribute("data-bs-toggle", "tooltip");

            var icon = new TagBuilder(HtmlTag.I);

            icon.AddCssClass("fa fa-rss-square fa-fw");

            // Render Icon
            link.InnerHtml.AppendHtml(icon);
        }
        else
        {
            link.TagRenderMode = TagRenderMode.SelfClosing;
        }

        return content.AppendHtml(link);
    }
}