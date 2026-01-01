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

namespace YAF.Types.Interfaces.Services;

using Microsoft.AspNetCore.Mvc;

using YAF.Types.Constants;
using YAF.Types.Models;

/// <summary>
/// Interface ILinkBuilder
/// </summary>
public interface ILinkBuilder
{
    /// <summary>
    /// Gets full URL to the Root of the Forum
    /// </summary>
    string ForumUrl { get; }

    /// <summary>
    /// The get user profile link.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    string GetUserProfileLink(User user);

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
    string GetUserProfileLink(int userId, string userName);

    /// <summary>
    /// The get forum link.
    /// </summary>
    /// <param name="forum">
    /// The forum.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    string GetForumLink(Forum forum);

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
    string GetForumLink(int forumId, string forumName);

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
    string GetCategoryLink(int categoryId, string categoryName);

    /// <summary>
    /// Gets the topic link.
    /// </summary>
    /// <param name="topic">
    /// The topic.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    string GetTopicLink(Topic topic);

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
    string GetTopicLink(int topicId, string topicName);

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
    string GetMessageLink(Topic topic, int messageId);

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
    string GetMessageLink(string topicName, int messageId);

    /// <summary>
    /// Gets link to the page with given parameters.
    /// </summary>
    /// <param name="page">
    /// Page to which to create a link.
    /// </param>
    /// <returns>
    /// URL to the given page with parameters.
    /// </returns>
    string GetLink(ForumPages page);

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
    string GetLink(ForumPages page, object values);

    /// <summary>
    /// Gets link to the page with given parameters.
    /// </summary>
    /// <param name="page">
    /// Page to which to create a link.
    /// </param>
    /// <returns>
    /// URL to the given page with parameters.
    /// </returns>
    string GetAbsoluteLink(ForumPages page);

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
    string GetAbsoluteLink(ForumPages page, object values);

    /// <summary>
    /// Redirects response to the access denied page.
    /// </summary>
    IActionResult AccessDenied();

    /// <summary>
    /// Redirects to the given page.
    /// </summary>
    /// <param name="page">
    /// Page to which to redirect response.
    /// </param>
    IActionResult Redirect(ForumPages page);

    /// <summary>
    /// Redirects to the given page with parameters.
    /// </summary>
    /// <param name="page">
    /// Page to which to redirect response.
    /// </param>
    /// <param name="values">
    /// The values.
    /// </param>
    IActionResult Redirect(ForumPages page, object values);

    /// <summary>
    /// Redirects response to the info page.
    /// </summary>
    /// <param name="infoMessage">
    /// The info Message.
    /// </param>
    IActionResult RedirectInfoPage(InfoMessage infoMessage);
}