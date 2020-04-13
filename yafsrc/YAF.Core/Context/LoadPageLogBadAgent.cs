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
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Utils.Helpers;

    /// <summary>
    /// The load page log bad agent.
    /// </summary>
    [ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
    public class LoadPageLogBadAgent : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadPageLogBadAgent"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="httpRequestBase">
        /// The http request base.
        /// </param>
        public LoadPageLogBadAgent(
            [NotNull] IServiceLocator serviceLocator,
            [NotNull] ILogger logger,
            [NotNull] HttpRequestBase httpRequestBase)
        {
            CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");
            CodeContracts.VerifyNotNull(logger, "logger");
            CodeContracts.VerifyNotNull(httpRequestBase, "httpRequestBase");

            this.ServiceLocator = serviceLocator;
            this.Logger = logger;
            this.HttpRequestBase = httpRequestBase;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets HttpRequestBase.
        /// </summary>
        public HttpRequestBase HttpRequestBase { get; set; }

        /// <summary>
        /// Gets or sets Logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        ///   Gets Order.
        /// </summary>
        public int Order => 2000;

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
            // vzrus: to log unhandled UserAgent strings
            if (!this.Get<BoardSettings>().UserAgentBadLog)
            {
                return;
            }

            if (this.HttpRequestBase.Url.ToString().Contains("digest"))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(@event.Data.UserAgent))
            {
                this.Logger.Warn("UserAgent string is empty.");
            }

            if ((@event.Data.Platform.ToLower().Contains("unknown")
                 || @event.Data.Browser.ToLower().Contains("unknown"))
                && !UserAgentHelper.IsSearchEngineSpider(@event.Data.UserAgent))
            {
                this.Logger.Log(
                    BoardContext.Current.User != null ? BoardContext.Current.User.UserName : string.Empty,
                    this,
                    $"Unhandled UserAgent string:'{(string)@event.Data.UserAgent}'<br />Platform:'{this.HttpRequestBase.Browser.Platform}'<br />Browser:'{this.HttpRequestBase.Browser.Browser}'");
            }
        }

        #endregion

        #endregion
    }
}