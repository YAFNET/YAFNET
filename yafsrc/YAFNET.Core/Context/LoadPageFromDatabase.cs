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

namespace YAF.Core.Context;

using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Routing;

#if !DEBUG
using Microsoft.AspNetCore.Http.Extensions;
#endif
using Microsoft.Extensions.Logging;

using YAF.Types.Attributes;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The load page from database.
/// </summary>
[ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
public class LoadPageFromDatabase : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadPageFromDatabase"/> class.
    /// </summary>
    /// <param name="serviceLocator">The service locator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="dataCache">The data cache.</param>
    /// <param name="endpointSources">The endpoint sources.</param>
    public LoadPageFromDatabase(
        IServiceLocator serviceLocator, ILogger<LoadPageFromDatabase> logger, IDataCache dataCache, IEnumerable<EndpointDataSource> endpointSources)
    {
        this.ServiceLocator = serviceLocator;
        this.Logger = logger;
        this.DataCache = dataCache;
        this.EndpointSources = endpointSources
            .SelectMany(es => es.Endpoints).Select(x => x.DisplayName.Replace("Page: ", "")).OrderByDescending(x => x);
    }

    /// <summary>
    /// Gets or sets the endpoint sources.
    /// </summary>
    /// <value>The endpoint sources.</value>
    public IEnumerable<string> EndpointSources { get; set; }

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    /// <value>
    /// The logger.
    /// </value>
    public ILogger Logger { get; set; }

    /// <summary>
    /// Gets or sets DataCache.
    /// </summary>
    public IDataCache DataCache { get; set; }

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order => 1000;

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    ///     The event.
    /// </param>
    public void Handle(InitPageLoadEvent @event)
    {
        try
        {
            string userKey = null;

            var context = this.Get<IHttpContextAccessor>().HttpContext;

            var ipAddress = context.GetUserRealIPAddress();

            if (BoardContext.Current.MembershipUser != null)
            {
                userKey = BoardContext.Current.MembershipUser.Id;
            }

            var tries = 0;
            Tuple<PageLoad, User, Category, Forum, Topic, Message> pageRow;

            var path = context!.Request.Path.ToString();

            string forumPage;

            try
            {
                if (!path.Equals("/"))
                {
                    var endpointPath = this.EndpointSources.Where(x => path.Contains(x)).ToList();

                    forumPage = endpointPath.Count != 0 ? endpointPath[0].ToPageName().ToString() : path.ToPageName().ToString();
                }
                else
                {
                    forumPage = string.Empty;
                }
            }
            catch (Exception)
            {
                forumPage = string.Empty;
            }

            var referer = context!.Request.Headers.Referer.ToString();

            var country = string.Empty;

            if (this.Get<IGeoIpCountryService>().DatabaseExists())
            {
                country = this.Get<IGeoIpCountryService>().GetCountry(ipAddress).CountryCode;
            }

            do
            {
                pageRow = this.Get<DataBroker>().GetPageLoad(
                    context.Session.Id,
                    BoardContext.Current.PageBoardID,
                    userKey,
                    ipAddress,
                    path,
                    referer,
                    country,
                    forumPage,
                    @event.UserRequestData.Browser,
                    @event.UserRequestData.Platform,
                    @event.UserRequestData.UserAgent.Truncate(300),
                    @event.PageQueryData.CategoryID,
                    @event.PageQueryData.ForumID,
                    @event.PageQueryData.TopicID,
                    @event.PageQueryData.MessageID,
                    @event.UserRequestData.IsSearchEngine,
                    @event.UserRequestData.DontTrack);

                // if the user doesn't exist create the user...
                if (userKey != null && pageRow == null && !this.Get<IAspNetRolesHelper>().DidCreateForumUser(
                        BoardContext.Current.MembershipUser,
                        BoardContext.Current.PageBoardID))
                {
                    throw new ArgumentException("Failed to create new user.");
                }

                if (pageRow is not null && pageRow.Item1 == null)
                {
                    if (pageRow.Item2 != null)
                    {
                        // FIX user roles ?!
                        this.Get<IAspNetRolesHelper>().SetupUserRolesAsync(
                            BoardContext.Current.PageBoardID,
                            BoardContext.Current.MembershipUser);

                        this.Get<IAspNetRolesHelper>().DidCreateForumUser(
                            BoardContext.Current.MembershipUser,
                            BoardContext.Current.PageBoardID);
                    }

                    pageRow = null;
                }

#pragma warning disable S2583 // Conditionally executed code should be reachable
                if (tries++ < 2)
                {
                    continue;
                }
#pragma warning restore S2583 // Conditionally executed code should be reachable

                if (userKey != null && pageRow == null)
                {
                    // probably no permissions, use guest user instead...
                    userKey = null;
                    continue;
                }

                // fail...
                break;
            } while (pageRow == null && userKey != null);

            // add all loaded page data into our data dictionary...
            @event.PageLoadData = pageRow ?? throw new ArgumentException("Unable to find the Guest User!");

            // update Query Data
            @event.PageQueryData.CategoryID = pageRow.Item3?.ID ?? 0;

            // add all loaded page data into our data dictionary...
            if (pageRow.Item4 != null)
            {
                @event.PageQueryData.ForumID = pageRow.Item4.ID;
            }

            if (pageRow.Item5 != null)
            {
                @event.PageQueryData.TopicID = pageRow.Item5.ID;
            }

            const string cookieName = "YAF-Theme";

            // Set theme mode
            if (context.Request.Cookies[cookieName] != null &&
                context.Request.Cookies[cookieName].IsSet() &&
                (context.Request.Cookies[cookieName] == "dark" ||
                 context.Request.Cookies[cookieName] == "auto"))
            {
                pageRow.Item2.DarkMode = true;
            }
            else
            {
                pageRow.Item2.DarkMode = false;
            }

            // clear active users list
            if (@event.PageLoadData.Item1.ActiveUpdate)
            {
                // purge the cache if something has changed...
                this.DataCache.Remove(Constants.Cache.UsersOnlineStatus);
            }
        }

#if !DEBUG
        catch (Exception x)
        {
            // log the exception...
            this.Logger.Error(
                x,
                $"Failure Initializing User/Page (URL: {this.Get<IHttpContextAccessor>().HttpContext.Request.GetDisplayUrl()}).");

            // log the user out...
            // FormsAuthentication.SignOut();
            if (BoardContext.Current.CurrentForumPage is null ||
                BoardContext.Current.CurrentForumPage.PageName == ForumPages.Info)
            {
                // totally failing... just re-throw the exception...
                throw;
            }

            // show a failure notice since something is probably up with membership...
            this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Failure);
        }
#else
        catch (Exception)
        {
            // re-throw exception...
            throw;
        }
#endif
    }
}