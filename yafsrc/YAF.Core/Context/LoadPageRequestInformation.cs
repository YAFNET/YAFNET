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

using YAF.Types.Attributes;

/// <summary>
/// The load page request information.
/// </summary>
[ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
public class LoadPageRequestInformation : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
{
    /// <summary>
    /// The browser detector.
    /// </summary>
    private readonly IBrowserDetector browserDetector;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadPageRequestInformation"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    /// <param name="accessor">
    /// The accessor.
    /// </param>
    /// <param name="browserDetector">
    /// The browser Detector.
    /// </param>
    public LoadPageRequestInformation(
        [NotNull] IServiceLocator serviceLocator,
        [NotNull] IHttpContextAccessor accessor,
        [NotNull] IBrowserDetector browserDetector)
    {
        this.ServiceLocator = serviceLocator;
        this.HttpRequestBase = accessor.HttpContext.Request;
        this.browserDetector = browserDetector;
    }

    /// <summary>
    /// Gets or sets HttpRequestBase.
    /// </summary>
    public HttpRequest HttpRequestBase { get; set; }

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
    public void Handle([NotNull] InitPageLoadEvent @event)
    {
        if (this.browserDetector.Browser == null)
        {
            return;
        }

        var browser = $"{this.browserDetector.Browser.Name} {this.browserDetector.Browser.Version}";
        var platform = this.browserDetector.Browser.OS;

        var userAgent = this.HttpRequestBase.Headers["User-Agent"].ToString();

        // try and get more verbose platform name by ref and other parameters
        UserAgentHelper.Platform(
            userAgent,
            UserAgentHelper.SearchEngineSpiderName(userAgent).IsSet(),
            ref platform,
            ref browser,
            out var isSearchEngine);

        var doNotTrack = !this.Get<BoardSettings>().ShowCrawlersInActiveList && isSearchEngine;

        // don't track if this is a feed reader. May be to make it switchable in host settings.
        // we don't have page 'g' token for the feed page.
        if (browser.Contains("Unknown") && !doNotTrack)
        {
            doNotTrack = UserAgentHelper.IsFeedReader(userAgent);
        }

        @event.UserRequestData.DontTrack = doNotTrack;
        @event.UserRequestData.UserAgent = userAgent;
        @event.UserRequestData.IsSearchEngine = isSearchEngine;
        @event.UserRequestData.Browser = browser;
        @event.UserRequestData.Platform = platform;

        BoardContext.Current.Vars["DontTrack"] = doNotTrack;
    }
}