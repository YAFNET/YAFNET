﻿/* Yet Another Forum.NET
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
namespace YAF.Core.Context;

/// <summary>
/// The load page request information.
/// </summary>
[ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
public class LoadPageRequestInformation : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadPageRequestInformation"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    /// <param name="httpRequestBase">
    /// The http request base.
    /// </param>
    public LoadPageRequestInformation(
        IServiceLocator serviceLocator, HttpRequestBase httpRequestBase)
    {
        this.ServiceLocator = serviceLocator;
        this.HttpRequestBase = httpRequestBase;
    }

    /// <summary>
    /// Gets or sets HttpRequestBase.
    /// </summary>
    public HttpRequestBase HttpRequestBase { get; set; }

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order => 10;

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Handles the specified @event.
    /// </summary>
    /// <param name="event">The @event.</param>
    public void Handle(InitPageLoadEvent @event)
    {
        var browser = $"{this.HttpRequestBase.Browser.Browser} {this.HttpRequestBase.Browser.Version}";
        var platform = this.HttpRequestBase.Browser.Platform;

        var userAgent = this.HttpRequestBase.UserAgent;

        // try and get more verbose platform name by ref and other parameters
        UserAgentHelper.Platform(
            userAgent,
            this.HttpRequestBase.Browser.Crawler,
            ref platform,
            out var isSearchEngine);

        var doNotTrack = !this.Get<BoardSettings>().ShowCrawlersInActiveList && isSearchEngine;

        @event.UserRequestData.DontTrack = doNotTrack;
        @event.UserRequestData.UserAgent = userAgent;
        @event.UserRequestData.IsSearchEngine = isSearchEngine;
        @event.UserRequestData.Browser = browser;
        @event.UserRequestData.Platform = platform;

        BoardContext.Current.Vars["DontTrack"] = doNotTrack;
    }
}