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
namespace YAF.Core.Services;

using System.Collections.Generic;
using System.Reflection;

using YAF.Types.Attributes;
using YAF.Types.Models;

/// <summary>
/// Class with link building functions.
/// </summary>
public class LinkBuilder : IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinkBuilder"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public LinkBuilder([NotNull] IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Gets full URL to the Root of the Forum
    /// </summary>
    public string ForumUrl => this.Get<LinkBuilder>().GetAbsoluteLink(ForumPages.Index);

    /// <summary>
    /// The get user profile link.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetUserProfileLink(User user)
    {
        return this.Get<LinkBuilder>().GetLink(
            ForumPages.UserProfile,
            new {u = user.ID, name = user.DisplayOrUserName()});
    }

    /// <summary>
    /// The get user profile link.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetUserProfileLink(int userId, string userName)
    {
        return this.Get<LinkBuilder>().GetLink(ForumPages.UserProfile, new {u = userId, name = userName});
    }

    /// <summary>
    /// The get forum link.
    /// </summary>
    /// <param name="forum">
    /// The forum.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetForumLink(Forum forum)
    {
        return this.Get<LinkBuilder>().GetLink(ForumPages.Topics, new {f = forum.ID, name = forum.Name});
    }

    /// <summary>
    /// The get forum link.
    /// </summary>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <param name="forumName">
    /// The forum name.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetForumLink(int forumId, string forumName)
    {
        return this.Get<LinkBuilder>().GetLink(
            ForumPages.Topics,
            new {f = forumId, name = UrlRewriteHelper.CleanStringForUrl(forumName)});
    }

    /// <summary>
    /// The get category link.
    /// </summary>
    /// <param name="categoryId">
    /// The category id.
    /// </param>
    /// <param name="categoryName">
    /// The category name.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetCategoryLink(int categoryId, string categoryName)
    {
        return this.Get<LinkBuilder>().GetLink(
            ForumPages.Index,
            new {c = categoryId, name = UrlRewriteHelper.CleanStringForUrl(categoryName)});
    }

    /// <summary>
    /// The get topic link.
    /// </summary>
    /// <param name="topicId">
    /// The topic id.
    /// </param>
    /// <param name="topicName">
    /// The topic name.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetTopicLink(int topicId, string topicName)
    {
        return this.Get<LinkBuilder>().GetLink(
            ForumPages.Posts,
            new {t = topicId, name = UrlRewriteHelper.CleanStringForUrl(topicName)});
    }

    /// <summary>
    /// Gets link to the page with given parameters.
    /// </summary>
    /// <param name="page">
    /// Page to which to create a link.
    /// </param>
    /// <returns>
    /// URL to the given page with parameters.
    /// </returns>
    public string GetLink(ForumPages page)
    {
        return this.Get<LinkBuilder>().GetLink(page, null);
    }

    /// <summary>
    /// Gets link to the page with given parameters.
    /// </summary>
    /// <param name="page">
    /// Page to which to create a link.
    /// </param>
    /// <param name="values">
    /// The values.
    /// </param>
    /// <returns>
    /// URL to the given page with parameters.
    /// </returns>
    public string GetLink(ForumPages page, object values)
    {
        var url = this.Get<IUrlHelper>().Page(page.GetPageName(), null, values);

        return url;
    }

    /// <summary>
    /// Gets link to the page with given parameters.
    /// </summary>
    /// <param name="page">
    /// Page to which to create a link.
    /// </param>
    /// <returns>
    /// URL to the given page with parameters.
    /// </returns>
    public string GetAbsoluteLink(ForumPages page)
    {
        return this.Get<LinkBuilder>().GetAbsoluteLink(page, null);
    }

    /// <summary>
    /// Gets link to the page with given parameters.
    /// </summary>
    /// <param name="page">
    /// Page to which to create a link.
    /// </param>
    /// <param name="values">
    /// The values.
    /// </param>
    /// <returns>
    /// URL to the given page with parameters.
    /// </returns>
    public string GetAbsoluteLink(ForumPages page, object values)
    {
        var url = this.Get<IUrlHelper>().Page(page.GetPageName(), null, values);

        return $"{this.Get<BoardInfo>().ForumBaseUrl}{url}";
    }

    /// <summary>
    /// Redirects response to the access denied page.
    /// </summary>
    public IActionResult AccessDenied()
    {
        return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied);
    }

    /// <summary>
    /// Redirects to the given page.
    /// </summary>
    /// <param name="page">
    /// Page to which to redirect response.
    /// </param>
    public IActionResult Redirect(ForumPages page)
    {
        return this.Get<LinkBuilder>().Redirect(page, null);
    }

    /// <summary>
    /// Redirects to the given page with parameters.
    /// </summary>
    /// <param name="page">
    /// Page to which to redirect response.
    /// </param>
    /// <param name="values">
    /// The values.
    /// </param>
    public IActionResult Redirect(ForumPages page, object values)
    {
        return BoardContext.Current.CurrentForumPage != null
                   ? BoardContext.Current.CurrentForumPage.RedirectToPage(page.GetPageName(), null, values)
                   : BoardContext.Current.CurrentForumController.RedirectToPage(page.GetPageName(), null, values);
    }

    /// <summary>
    /// Redirects response to the info page.
    /// </summary>
    /// <param name="infoMessage">
    /// The info Message.
    /// </param>
    public IActionResult RedirectInfoPage(InfoMessage infoMessage)
    {
        return this.Get<LinkBuilder>().Redirect(
            ForumPages.Info,
            new {info = infoMessage.ToType<int>()});
    }

    public IDictionary<string, string> GetAllRouteData(object values)
    {
        const BindingFlags BindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;

        return values.GetType().GetProperties(BindingFlags).ToDictionary(
            propInfo => propInfo.Name,
            propInfo => propInfo.GetValue(values, null).ToString());
    }
}