/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.Context
{
    using System.Web;

    using YAF.Configuration;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Utils.Helpers;

    /// <summary>
    /// The load page request information.
    /// </summary>
    [ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
    public class LoadPageRequestInformation : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
    {
        #region Constructors and Destructors

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
            [NotNull] IServiceLocator serviceLocator, [NotNull] HttpRequestBase httpRequestBase)
        {
            this.ServiceLocator = serviceLocator;
            this.HttpRequestBase = httpRequestBase;
        }

        #endregion

        #region Properties

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

        #endregion

        #region Implemented Interfaces

        #region IHandleEvent<InitPageLoadEvent>

        /// <summary>
        /// Handles the specified @event.
        /// </summary>
        /// <param name="event">The @event.</param>
        public void Handle([NotNull] InitPageLoadEvent @event)
        {
            var browser = $"{this.HttpRequestBase.Browser.Browser} {this.HttpRequestBase.Browser.Version}";
            var platform = this.HttpRequestBase.Browser.Platform;

            var userAgent = this.HttpRequestBase.UserAgent;

            var isMobileDevice = UserAgentHelper.IsMobileDevice(this.HttpRequestBase);

            // try and get more verbose platform name by ref and other parameters             
            UserAgentHelper.Platform(
                userAgent,
                this.HttpRequestBase.Browser.Crawler,
                ref platform,
                ref browser,
                out var isSearchEngine,
                out var dontTrack);

            dontTrack = !this.Get<BoardSettings>().ShowCrawlersInActiveList && isSearchEngine;

            // don't track if this is a feed reader. May be to make it switchable in host settings.
            // we don't have page 'g' token for the feed page.
            if (browser.Contains("Unknown") && !dontTrack)
            {
                dontTrack = UserAgentHelper.IsFeedReader(userAgent);
            }

            @event.Data.DontTrack = dontTrack;
            @event.Data.UserAgent = userAgent;
            @event.Data.IsSearchEngine = isSearchEngine;
            @event.Data.IsMobileDevice = isMobileDevice;
            @event.Data.Browser = browser;
            @event.Data.Platform = platform;

            BoardContext.Current.Vars["DontTrack"] = dontTrack;
        }

        #endregion

        #endregion
    }
}