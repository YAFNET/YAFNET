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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Extensions;

using System.Collections.Generic;

using YAF.Types.Attributes;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The PageLink Extensions
/// </summary>
public static class PageLinkExtensions
{
    /// <summary>
    /// Adds the root.
    /// </summary>
    /// <param name="pageLinks">The page links.</param>
    /// <returns>returns the Page links including the root</returns>
    public static List<PageLink> AddRoot(this List<PageLink> pageLinks)
    {
        CodeContracts.VerifyNotNull(pageLinks);

        pageLinks.AddLink(
            BoardContext.Current.BoardSettings.Name,
            BoardContext.Current.Get<LinkBuilder>().GetLink(ForumPages.Index));

        return pageLinks;
    }

    /// <summary>
    /// Adds the index of the admin.
    /// </summary>
    /// <param name="pageLinks">The page links.</param>
    /// <returns>List&lt;PageLink&gt;.</returns>
    public static List<PageLink> AddAdminIndex(this List<PageLink> pageLinks)
    {
        CodeContracts.VerifyNotNull(pageLinks);

        pageLinks.AddLink(
            BoardContext.Current.Get<ILocalization>().GetText("ADMIN_ADMIN", "Administration"),
            BoardContext.Current.Get<LinkBuilder>().GetLink(ForumPages.Admin_Admin));

        return pageLinks;
    }

    /// <summary>
    /// The add user.
    /// </summary>
    /// <param name="pageLinks">
    /// The page links.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <returns>
    /// The <see cref="PageLinks"/>.
    /// </returns>
    public static List<PageLink> AddUser(this List<PageLink> pageLinks, [NotNull] int userId, [NotNull] string name)
    {
        CodeContracts.VerifyNotNull(pageLinks);
        CodeContracts.VerifyNotNull(name);

        pageLinks.AddLink(name, BoardContext.Current.Get<LinkBuilder>().GetUserProfileLink(userId, name));

        return pageLinks;
    }

    /// <summary>
    /// The add topic.
    /// </summary>
    /// <param name="pageLinks">
    /// The page links.
    /// </param>
    /// <param name="topic">
    /// The topic.
    /// </param>
    /// <returns>
    /// The <see cref="PageLinks"/>.
    /// </returns>
    public static List<PageLink> AddTopic(this List<PageLink> pageLinks, [NotNull] Topic topic)
    {
        CodeContracts.VerifyNotNull(pageLinks);

        pageLinks.AddLink(topic.TopicName, BoardContext.Current.Get<LinkBuilder>().GetTopicLink(topic.ID, topic.TopicName));

        return pageLinks;
    }

    /// <summary>
    /// Adds the category.
    /// </summary>
    /// <param name="pageLinks">
    /// The page links.
    /// </param>
    /// <param name="category">
    /// The category.
    /// </param>
    /// <returns>
    /// Returns the Page links including the Category
    /// </returns>
    public static List<PageLink> AddCategory(
        this List<PageLink> pageLinks,
        [NotNull] Category category)
    {
        CodeContracts.VerifyNotNull(pageLinks);
        CodeContracts.VerifyNotNull(category);

        pageLinks.AddLink(
            category.Name,
            BoardContext.Current.Get<LinkBuilder>().GetCategoryLink(category.ID, category.Name));

        return pageLinks;
    }

    /// <summary>
    /// Adds the forum links.
    /// </summary>
    /// <param name="pageLinks">
    /// The page links.
    /// </param>
    /// <param name="forum">
    /// The forum.
    /// </param>
    /// <param name="noForumLink">
    /// The no forum link.
    /// </param>
    /// <returns>
    /// Returns the page links
    /// </returns>
    public static List<PageLink> AddForum(this List<PageLink> pageLinks, Forum forum, bool noForumLink = false)
    {
        CodeContracts.VerifyNotNull(pageLinks);

        if (forum.ParentID.HasValue)
        {
            var parent = BoardContext.Current.GetRepository<Forum>()
                .GetById(forum.ParentID.Value);

            if (parent != null)
            {
                pageLinks.AddLink(
                    parent.Name,
                    BoardContext.Current.Get<LinkBuilder>().GetForumLink(parent));
            }
        }

        if (BoardContext.Current.PageForumID == forum.ID)
        {
            pageLinks.AddLink(
                BoardContext.Current.PageForum.Name,
                noForumLink
                    ? string.Empty
                    : BoardContext.Current.Get<LinkBuilder>().GetForumLink(
                        BoardContext.Current.PageForum));
        }

        return pageLinks;
    }

    /// <summary>
    /// Adds the link.
    /// </summary>
    /// <param name="pageLinks">The page links.</param>
    /// <param name="title">The title.</param>
    /// <param name="url">The URL.</param>
    /// <returns>Returns the page links</returns>
    public static List<PageLink> AddLink(this List<PageLink> pageLinks, [NotNull] string title, [CanBeNull] string url = "")
    {
        CodeContracts.VerifyNotNull(pageLinks);
        CodeContracts.VerifyNotNull(title);

        pageLinks.Add(new PageLink { Title = title.Trim(), URL = url?.Trim() });

        return pageLinks;
    }
}