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

using Microsoft.Extensions.Logging;

using YAF.Types.Attributes;

/// <summary>
/// The load page log bad agent.
/// </summary>
[ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
public class LoadPageLogBadAgent : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
{
    /// <summary>
    /// The browser detector.
    /// </summary>
    private readonly IUAParserOutput userAgentParser;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadPageLogBadAgent"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="accessor">
    /// The accessor.
    /// </param>
    /// <param name="browserDetector">
    /// The browser Detector.
    /// </param>
    public LoadPageLogBadAgent(
        [NotNull] IServiceLocator serviceLocator,
        [NotNull] ILogger<LoadPageLogBadAgent> logger,
        [NotNull] IHttpContextAccessor accessor,
        [NotNull] IUserAgentParser parser)
    {
        CodeContracts.VerifyNotNull(serviceLocator);
        CodeContracts.VerifyNotNull(logger);
        CodeContracts.VerifyNotNull(accessor);
        CodeContracts.VerifyNotNull(parser);

        this.ServiceLocator = serviceLocator;
        this.Logger = logger;
        this.HttpRequestBase = accessor.HttpContext.Request;
        this.userAgentParser = parser.ClientInfo;
    }

    /// <summary>
    /// Gets or sets HttpRequestBase.
    /// </summary>
    public HttpRequest HttpRequestBase { get; set; }

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

    /// <summary>
    /// Handles the specified @event.
    /// </summary>
    /// <param name="event">The @event.</param>
    public void Handle([NotNull] InitPageLoadEvent @event)
    {
        // log unhandled UserAgent strings
        if (!this.Get<BoardSettings>().UserAgentBadLog)
        {
            return;
        }

        if (this.HttpRequestBase.Path.ToString().Contains("digest"))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(@event.UserRequestData.UserAgent))
        {
            this.Logger.Warn("UserAgent string is empty.");
        }

        if (@event.UserRequestData.Platform.ToLower().Contains("unknown") || @event.UserRequestData.Browser.ToLower().Contains("unknown"))
        {
            this.Logger.Log(
                BoardContext.Current.PageUserID,
                this,
                $@"Unhandled UserAgent string:'{@event.UserRequestData.UserAgent}'<br />
                                 Platform:'{this.userAgentParser.OS}'<br />
                                 Browser:'{this.userAgentParser.Browser}'");
        }
    }
}