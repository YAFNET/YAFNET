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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Services;

using System.Web;

using Microsoft.AspNetCore.Routing;

using YAF.Types.Objects;
using YAF.Types.Models;

/// <summary>
/// Class with link building functions.
/// </summary>
public class LinkBuilder : IHaveServiceLocator, ILinkBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinkBuilder"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public LinkBuilder(IServiceLocator serviceLocator)
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
    public string ForumUrl => this.Get<ILinkBuilder>().GetAbsoluteLink(ForumPages.Index);

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
        return this.Get<ILinkBuilder>().GetLink(
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
    /// The username.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetUserProfileLink(int userId, string userName)
    {
        return this.Get<ILinkBuilder>().GetLink(ForumPages.UserProfile, new {u = userId, name = userName});
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
        return this.Get<ILinkBuilder>().GetLink(ForumPages.Topics, new {f = forum.ID, name = forum.Name});
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
        return this.Get<ILinkBuilder>().GetLink(
            ForumPages.Topics,
            new {f = forumId, name = forumName});
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
        return this.Get<ILinkBuilder>().GetLink(
            ForumPages.Index,
            new {c = categoryId, name = categoryName});
    }

    /// <summary>
    /// Gets the topic link.
    /// </summary>
    /// <param name="topic">
    /// The topic.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetTopicLink(Topic topic)
    {
        return this.Get<ILinkBuilder>().GetTopicLink(topic.ID, topic.TopicName);
    }

    /// <summary>
    /// Gets the topic link.
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
        var topicSubject = HttpUtility.HtmlEncode(this.Get<IBadWordReplace>().Replace(topicName));

        return this.Get<ILinkBuilder>().GetLink(
            ForumPages.Posts,
            new {t = topicId, name = topicSubject });
    }

    /// <summary>
    /// Gets the topic link with post Id.
    /// </summary>
    /// <param name="topic">
    /// The topic.
    /// </param>
    /// <param name="messageId">
    /// The topic name.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetMessageLink(Topic topic, int messageId)
    {
        var topicSubject = HttpUtility.HtmlEncode(this.Get<IBadWordReplace>().Replace(topic.TopicName));

        return this.Get<ILinkBuilder>().GetLink(
            ForumPages.Post,
            new { name = topicSubject, m = messageId });
    }

    /// <summary>
    /// Gets the message link.
    /// </summary>
    /// <param name="topicName">
    /// The topic name.
    /// </param>
    /// <param name="messageId">
    /// The topic name.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetMessageLink(string topicName, int messageId)
    {
        var topicSubject = HttpUtility.HtmlEncode(this.Get<IBadWordReplace>().Replace(topicName));

        return this.Get<ILinkBuilder>().GetLink(
            ForumPages.Post,
            new { name = topicSubject, m = messageId });
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
        return this.Get<ILinkBuilder>().GetLink(page, null);
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
        var routeValues = new RouteValueDictionary(values);

        if (routeValues.ContainsKey("name"))
        {
            routeValues["name"] = UrlRewriteHelper.CleanStringForUrl(routeValues["name"]!.ToString());
        }

        if (!this.Get<BoardConfiguration>().Area.IsSet())
        {
            return this.Get<IUrlHelper>().Page(page.GetPageName(), null, routeValues);
        }

        if (!routeValues.ContainsKey("area"))
        {
            routeValues.AddOrUpdate("area", this.Get<BoardConfiguration>().Area);
        }

        return this.Get<IUrlHelper>().Page(page.GetPageName(), null, routeValues);
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
        return this.Get<ILinkBuilder>().GetAbsoluteLink(page, null);
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
        var url = this.Get<ILinkBuilder>().GetLink(page, values);

        return $"{this.Get<BoardInfo>().ForumBaseUrl}{url}";
    }

    /// <summary>
    /// Redirects response to the access denied page.
    /// </summary>
    public IActionResult AccessDenied()
    {
        return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied);
    }

    /// <summary>
    /// Redirects to the given page.
    /// </summary>
    /// <param name="page">
    /// Page to which to redirect response.
    /// </param>
    public IActionResult Redirect(ForumPages page)
    {
        return this.Get<ILinkBuilder>().Redirect(page, null);
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
        var routeValues = new RouteValueDictionary(values);

        if (routeValues.ContainsKey("name"))
        {
            routeValues["name"] = UrlRewriteHelper.CleanStringForUrl(routeValues["name"]!.ToString());
        }

        if (this.Get<BoardConfiguration>().Area.IsSet())
        {
            routeValues.AddOrUpdate("area", this.Get<BoardConfiguration>().Area);
        }

        if (BoardContext.Current.CurrentForumPage is not null)
        {
            return BoardContext.Current.CurrentForumPage.RedirectToPage(page.GetPageName(), null, routeValues);
        }

        if (BoardContext.Current.CurrentForumController is not null)
        {
            return BoardContext.Current.CurrentForumController.RedirectToPage(page.GetPageName(), null, routeValues);
        }

        this.Get<IHttpContextAccessor>().HttpContext?.Response.Redirect(page.GetPageName());

        return null;
    }

    /// <summary>
    /// Redirects response to the info page.
    /// </summary>
    /// <param name="infoMessage">
    /// The info Message.
    /// </param>
    public IActionResult RedirectInfoPage(InfoMessage infoMessage)
    {
        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Info,
            new {info = infoMessage.ToType<int>()});
    }
}