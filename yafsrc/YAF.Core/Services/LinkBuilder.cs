/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

#region Using

using System.Linq;
using System.Web;

using ServiceStack.Text;

using YAF.Types;
using YAF.Types.Constants;
using YAF.Types.Extensions;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

#endregion

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

    #region Properties

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets full URL to the Root of the Forum
    /// </summary>
    public string ForumUrl => this.Get<LinkBuilder>().GetAbsoluteLink(ForumPages.Board);

    /// <summary>
    /// Gets the safe raw URL.
    /// </summary>
    /// <returns>Returns the safe raw URL</returns>
    public string GetSafeRawUrl()
    {
        return this.Get<LinkBuilder>().GetSafeRawUrl(this.Get<HttpContextBase>().Request.RawUrl);
    }

    /// <summary>
    /// Cleans up a URL so that it doesn't contain any problem characters.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>
    /// The get safe raw URL.
    /// </returns>
    [NotNull]
    public string GetSafeRawUrl([NotNull] string url)
    {
        CodeContracts.VerifyNotNull(url);

        var processedRaw = url;
        processedRaw = processedRaw.Replace("\"", string.Empty);
        processedRaw = processedRaw.Replace("<", "%3C");
        processedRaw = processedRaw.Replace(">", "%3E");
        processedRaw = processedRaw.Replace("&", "%26");
        return processedRaw.Replace("'", string.Empty);
    }

    /// <summary>
    /// Function that verifies a string is an integer value or it redirects to invalid "info" page.
    /// Used as a security feature against invalid values submitted to the page.
    /// </summary>
    /// <param name="intValue">
    /// The string value to test
    /// </param>
    /// <returns>
    /// The converted integer value
    /// </returns>
    public int StringToIntOrRedirect(string intValue)
    {
        if (!int.TryParse(intValue, out var value))
        {
            // it's an invalid request. Redirect to the info page on invalid requests.
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        return value;
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
        return this.Get<LinkBuilder>().GetLink(ForumPages.UserProfile, new { u = userId, name = userName });
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
        return this.Get<LinkBuilder>().GetLink(ForumPages.Topics, new { f = forum.ID, name = forum.Name });
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
        return this.Get<LinkBuilder>().GetLink(ForumPages.Topics, new { f = forumId, name = forumName });
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
        return this.Get<LinkBuilder>().GetLink(ForumPages.Board, new { c = categoryId, name = categoryName })
            .Replace("&amp;", "&");
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
        return this.Get<LinkBuilder>().GetLink(ForumPages.Posts, new { t = topicId, name = topicName })
            .Replace("&amp;", "&");
    }

    /// <summary>
    /// Gets base path to the page without ampersand.
    /// </summary>
    /// <returns>
    /// Base URL to the given page.
    /// </returns>
    public string GetBasePath()
    {
        return this.Get<IUrlBuilder>().BuildUrl(string.Empty).TrimEnd('&');
    }

    /// <summary>
    /// Redirects response to the access denied page.
    /// </summary>
    public void AccessDenied()
    {
        this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied);
    }

    /// <summary>
    /// Gets link to the page.
    /// </summary>
    /// <param name="page">Page to which to create a link.</param>
    /// <returns>
    /// URL to the given page.
    /// </returns>
    public string GetLink(ForumPages page)
    {
        return this.Get<IUrlBuilder>().BuildUrl($"g={page}");
    }

    /// <summary>
    /// Gets link to the page with given parameters.
    /// </summary>
    /// <param name="page">
    /// Page to which to create a link.
    /// </param>
    /// <param name="values">
    /// The query string values.
    /// </param>
    /// <returns>
    /// URL to the given page with parameters.
    /// </returns>
    public string GetLink(ForumPages page, object values)
    {
        var queryString = this.BuildQueryString(values);

        return this.Get<IUrlBuilder>().BuildUrl($"g={page}{queryString}").Replace("&amp;", "&");
    }

    /// <summary>
    /// Gets Absolute link to the page with given parameters.
    /// </summary>
    /// <param name="page">
    /// Page to which to create a link.
    /// </param>
    /// <returns>
    /// URL to the given page with parameters.
    /// </returns>
    public string GetAbsoluteLink(ForumPages page)
    {
        return this.Get<IUrlBuilder>().BuildUrlFull($"g={page}");
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
        var queryString = this.BuildQueryString(values);

        return this.Get<IUrlBuilder>().BuildUrlFull($"g={page}{queryString}");
    }

    /// <summary>
    /// Redirects to the given page.
    /// </summary>
    /// <param name="page">
    /// Page to which to redirect response.
    /// </param>
    public void Redirect(ForumPages page)
    {
        this.Get<HttpResponseBase>().Redirect(this.Get<LinkBuilder>().GetLink(page).Replace("&amp;", "&"));
    }

    /// <summary>
    /// Redirects to the given page with parameters.
    /// </summary>
    /// <param name="page">
    /// Page to which to redirect response.
    /// </param>
    /// <param name="values">
    /// The query string values.
    /// </param>
    public void Redirect(ForumPages page, object values)
    {
        this.Get<HttpResponseBase>().Redirect(this.Get<LinkBuilder>().GetLink(page, values).Replace("&amp;", "&"));
    }

    /// <summary>
    /// Redirects to the given page with parameters.
    /// </summary>
    /// <param name="page">
    /// Page to which to redirect response.
    /// </param>
    /// <param name="endResponse">True to end the Response, false otherwise.</param>
    /// <param name="values">
    /// The query string values.
    /// </param>
    public void Redirect(ForumPages page, bool endResponse, object values)
    {
        this.Get<HttpResponseBase>().Redirect(this.Get<LinkBuilder>().GetLink(page, values), endResponse);
    }

    /// <summary>
    /// Redirects response to the info page.
    /// </summary>
    /// <param name="infoMessage">
    /// The info Message.
    /// </param>
    public void RedirectInfoPage(InfoMessage infoMessage)
    {
        this.Get<LinkBuilder>().Redirect(ForumPages.Info, new { i = infoMessage.ToType<int>() });
    }

    /// <summary>
    /// Build the Query String
    /// </summary>
    /// <param name="values">
    /// The values.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    private string BuildQueryString(object values)
    {
        var queryString = string.Empty;

        var parameters = values.ToObjectDictionary();

        queryString = parameters.Aggregate(
            queryString,
            (current, param) => $"{current}&{param.Key}={param.Value}");

        return queryString;
    }

    #endregion
}