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

namespace YAF.Core.Context;

using Microsoft.AspNetCore.Routing;

using YAF.Types.Attributes;

/// <summary>
/// The load page variables from query.
/// </summary>
[ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
public class LoadPageVariablesFromQuery : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadPageVariablesFromQuery"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public LoadPageVariablesFromQuery(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order => 10;

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Handles the specified @event.
    /// </summary>
    /// <param name="event">The @event.</param>
    public void Handle(InitPageLoadEvent @event)
    {
        var queryString = this.Get<IHttpContextAccessor>().HttpContext.Request.Query;

        var routeData = this.Get<IHttpContextAccessor>().HttpContext.GetRouteData();

        if (queryString.Count == 0 && routeData.Values.Count > 0)
        {
            @event.PageQueryData.CategoryID = routeData.Values["c"].ToTypeOrDefault(0);
            @event.PageQueryData.ForumID = routeData.Values["f"].ToTypeOrDefault(0);
            @event.PageQueryData.TopicID = routeData.Values["t"].ToTypeOrDefault(0);
            @event.PageQueryData.MessageID = routeData.Values["m"].ToTypeOrDefault(0);
            @event.PageQueryData.PageIndex = routeData.Values["p"].ToTypeOrDefault(0);
        }
        else
        {
            @event.PageQueryData.CategoryID = queryString["c"].FirstOrDefault().ToTypeOrDefault(0);
            @event.PageQueryData.ForumID = queryString["f"].FirstOrDefault().ToTypeOrDefault(0);
            @event.PageQueryData.TopicID = queryString["t"].FirstOrDefault().ToTypeOrDefault(0);
            @event.PageQueryData.MessageID = queryString["m"].FirstOrDefault().ToTypeOrDefault(0);
            @event.PageQueryData.PageIndex = queryString["p"].FirstOrDefault().ToTypeOrDefault(0);
        }

        var topicId = this.Get<IDataCache>().Get("TopicID");

        if (topicId != null)
        {
            if (@event.PageQueryData.TopicID == 0)
            {
                @event.PageQueryData.TopicID = topicId.ToType<int>();
            }
        }

        if (BoardContext.Current.Settings.CategoryID != 0)
        {
            @event.PageQueryData.CategoryID = BoardContext.Current.Settings.CategoryID;
        }
    }
}