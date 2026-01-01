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
/// The canonical meta tag html helper.
/// </summary>
public static class CanonicalMetaTagHtmlHelper
{
    /// <summary>
    /// The canonical meta tag.
    /// </summary>
    /// <param name="htmlHelper">
    /// The html helper.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent CanonicalMetaTag(this IHtmlHelper htmlHelper)
    {
        var context = BoardContext.Current;

        var content = new HtmlContentBuilder();

        // in cases where we are not going to index, but follow, we will not add a canonical tag.
        if (context.CurrentForumPage.PageName == ForumPages.Posts)
        {
            if (htmlHelper.ViewContext.HttpContext.Request.Query.ContainsKey("m")
                || htmlHelper.ViewContext.HttpContext.Request.RouteValues.ContainsKey("m"))
            {
                // add no-index tag
                return content.AppendHtml(GetNoIndexMetaBuilder());
            }

            var link = new TagBuilder(HtmlTag.Link);

            link.MergeAttribute(
                HtmlAttribute.Href,
                context.Get<ILinkBuilder>().GetAbsoluteLink(
                    ForumPages.Posts,
                    new { t = context.PageTopicID, name = context.PageTopic.TopicName }));

            link.MergeAttribute(HtmlAttribute.Rel, "canonical");

            link.TagRenderMode = TagRenderMode.SelfClosing;

            return content.AppendHtml(link);
        }

        if (context.CurrentForumPage.PageName != ForumPages.Index &&
            context.CurrentForumPage.PageName != ForumPages.Topics)
        {
            // there is not much SEO value to having lists indexed
            // because they change as soon as some adds a new topic
            // or post so don't index them, but follow the links
            // add no-index tag
            return content.AppendHtml(GetNoIndexMetaBuilder());
        }

        return content;
    }

    /// <summary>
    /// The get no index meta builder.
    /// </summary>
    /// <returns>
    /// The <see cref="TagBuilder"/>.
    /// </returns>
    private static TagBuilder GetNoIndexMetaBuilder()
    {
        // setup no index meta tag
        var noIndexMeta = new TagBuilder(HtmlTag.Meta);

        noIndexMeta.MergeAttribute("name", "robots");
        noIndexMeta.MergeAttribute("content", "noindex,follow");

        noIndexMeta.TagRenderMode = TagRenderMode.SelfClosing;

        return noIndexMeta;
    }
}