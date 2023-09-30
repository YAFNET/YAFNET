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

namespace YAF.Core.Middleware;

using System;
using System.Threading.Tasks;
using System.Xml.Serialization;

using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// Class SiteMapMiddleware.
/// Implements the <see cref="YAF.Types.Interfaces.IHaveServiceLocator" />
/// </summary>
/// <seealso cref="YAF.Types.Interfaces.IHaveServiceLocator" />
public class SiteMapMiddleware : IHaveServiceLocator
{
    /// <summary>
    /// The request delegate.
    /// </summary>
    private readonly RequestDelegate requestDelegate;

    /// <summary>
    /// Initializes a new instance of the <see cref="SiteMapMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next.</param>
    /// <param name="serviceLocator">The service locator.</param>
    public SiteMapMiddleware(RequestDelegate next, IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
        this.requestDelegate = next;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// The invoke.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task InvokeAsync(HttpContext context)
    {
        var siteMap = new SiteMap();

        var forumList = this.GetRepository<Forum>().ListAllWithAccess(
            BoardContext.Current.BoardSettings.BoardId,
            BoardContext.Current.GuestUserID);

        forumList.ForEach(
            forum => siteMap.Add(
                new UrlLocation
                    {
                        Url = this.Get<LinkBuilder>().GetTopicLink(forum.Item1.ID, forum.Item1.Name),
                        Priority = 0.8D,
                        LastModified =
                            forum.Item1.LastPosted.HasValue
                                ? forum.Item1.LastPosted.Value.ToString(
                                    "yyyy-MM-ddTHH:mm:ss",
                                    CultureInfo.InvariantCulture)
                                : DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                        ChangeFrequency = UrlLocation.ChangeFrequencies.always
                    }));

        context.Response.Clear();

        var xs = new XmlSerializer(typeof(SiteMap));

        context.Response.ContentType = "text/xml";

        xs.Serialize(context.Response.Body, siteMap);

        return this.requestDelegate(context);
    }
}