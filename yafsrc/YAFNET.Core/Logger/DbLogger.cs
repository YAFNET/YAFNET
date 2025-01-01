/* Yet Another Forum.NET
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

using Microsoft.AspNetCore.Diagnostics;

namespace YAF.Core.Logger;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using YAF.Configuration;
using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Types.Constants;
using YAF.Types.Extensions;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;

using EventLog = YAF.Types.Models.EventLog;

/// <summary>
///     The YAF Data Base logger.
/// </summary>
public class DbLogger : ILogger, IHaveServiceLocator
{
#if DEBUG
    /// <summary>
    ///     The is debug.
    /// </summary>
    private const bool IsDebug = true;
#else
        private const bool IsDebug = false;

#endif
    /// <summary>
    /// Instance of <see cref="DbLoggerProvider" />.
    /// </summary>
    private readonly DbLoggerProvider provider;

    /// <summary>
    ///     The event log type lookup.
    /// </summary>
    private Dictionary<EventLogTypes, bool> eventLogTypeLookup;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbLogger"/> class.
    /// </summary>
    /// <param name="loggerProvider">The database logger provider.</param>
    /// <param name="type">The type.</param>
    public DbLogger(DbLoggerProvider loggerProvider, string type)
    {
        this.provider = loggerProvider;
        this.Type = type;
    }

    /// <summary>
    /// Gets the event log repository.
    /// </summary>
    /// <value>
    /// The event log repository.
    /// </value>
    public IRepository<EventLog> EventLogRepository => this.GetRepository<EventLog>();

    /// <summary>
    /// Gets or sets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    ///     Gets or sets the logging type.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The is log type enabled.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    /// The <see cref="bool" />.
    /// </returns>
    public bool IsLogTypeEnabled(EventLogTypes type)
    {
        if (this.eventLogTypeLookup == null)
        {
            // create it...
            this.InitLookup();
        }

        return this.eventLogTypeLookup.ContainsKey(type) && this.eventLogTypeLookup[type];
    }

    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
    /// <param name="state">The identifier for the scope.</param>
    /// <returns>An <see cref="T:System.IDisposable" /> that ends the logical operation scope on dispose.</returns>
    public IDisposable BeginScope<TState>(TState state)
    {
        return this.provider.ScopeProvider.Push(state);
    }

    /// <summary>
    /// Whether to log the entry.
    /// </summary>
    /// <param name="logLevel">Level to be checked.</param>
    /// <returns><c>true</c> if enabled.</returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    /// <summary>
    /// Writes a log entry.
    /// </summary>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">Id of the event.</param>
    /// <param name="state">The entry to be written. Can be also an object.</param>
    /// <param name="exception">The exception related to this entry.</param>
    /// <param name="formatter">Function to create a <see cref="T:System.String" /> message of the <paramref name="state" /> and <paramref name="exception" />.</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!this.IsEnabled(logLevel))
        {
            // Don't log the entry if it's not enabled.
            return;
        }

        this.provider.ScopeProvider?.ForEachScope(
            (value, _) =>
                {
                    if (value is not EventLog)
                    {
                        return;
                    }

                    var eventEntry = value.ToType<EventLog>();

                    this.WriteLog(
                        state.ToString(),
                        eventEntry.EventType,
                        eventEntry.UserID,
                        eventEntry.Source,
                        eventEntry.Exception);
                },
            state);
    }

    /// <summary>
    /// The log.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="eventType">
    /// The event type.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="exception">
    /// The exception.
    /// </param>
    public void WriteLog(
        string message,
        EventLogTypes eventType = EventLogTypes.Error,
        int? userId = null,
        string source = null,
        Exception exception = null)
    {
        if (!this.IsLogTypeEnabled(eventType))
        {
            return;
        }

        var url = string.Empty;
        var userIp = string.Empty;
        var userAgent = string.Empty;
        var httpContext = this.Get<IHttpContextAccessor>().HttpContext;

        if (httpContext is not null)
        {
            try
            {
                userIp = httpContext.Request.GetUserRealIPAddress();
            }
            catch (Exception)
            {
                userIp = string.Empty;
            }

            var exceptionHandlerPathFeature =
                httpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature is not null)
            {
                url = HtmlTagHelper.StripHtml(
                    $"{httpContext.Request.Host}{exceptionHandlerPathFeature.Path}{httpContext.Request.QueryString}");

                source = exceptionHandlerPathFeature.Path;
            }
            else
            {
                url = HtmlTagHelper.StripHtml(
                    httpContext.Request.GetDisplayUrl());
            }

            userAgent = httpContext.Request.Headers.UserAgent.ToString();
        }

        var values = new JObject
                         {
                             ["Message"] = message,
                             ["UserIP"] = userIp,
                             ["Url"] = url,
                             ["UserAgent"] = userAgent,
                             ["ExceptionMessage"] = exception?.Message,
                             ["ExceptionStackTrace"] = exception?.StackTrace,
                             ["ExceptionSource"] = exception?.Source
                         };

        var formattedDescription = JsonConvert.SerializeObject(
            values,
            new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    Formatting = Formatting.None
                });

        if (source.IsNotSet())
        {
            source = this.Type.IsSet() ? this.Type : string.Empty;
        }

        if (source.IsSet() && source!.Length > 50)
        {
            source = source.Truncate(47);
        }

        switch (eventType)
        {
            case EventLogTypes.Debug:
                Debug.WriteLine(formattedDescription, source);
                break;
            case EventLogTypes.Information:
                if (this.Get<BoardSettings>().LogInformation)
                {
                    this.EventLogRepository.Insert(
                        new EventLog
                            {
                                EventType = eventType,
                                UserID = userId,
                                Description = formattedDescription,
                                Source = source,
                                EventTime = DateTime.UtcNow
                            });
                }

                break;
            case EventLogTypes.Warning:
                if (this.Get<BoardSettings>().LogWarning)
                {
                    this.EventLogRepository.Insert(
                        new EventLog
                            {
                                EventType = eventType,
                                UserID = userId,
                                Description = formattedDescription,
                                Source = source,
                                EventTime = DateTime.UtcNow
                            });
                }

                break;
            case EventLogTypes.Error:
                if (this.Get<BoardSettings>().LogError)
                {
                    this.EventLogRepository.Insert(
                        new EventLog
                            {
                                EventType = eventType,
                                UserID = userId,
                                Description = formattedDescription,
                                Source = source,
                                EventTime = DateTime.UtcNow
                            });
                }

                break;
            default:
                {
                    var log = new EventLog
                                  {
                                      EventType = eventType,
                                      UserID = userId,
                                      Description = formattedDescription,
                                      Source = source,
                                      EventTime = DateTime.UtcNow
                                  };

                    this.EventLogRepository.Insert(log);
                }

                break;
        }
    }

    /// <summary>
    /// Initialization of Lookup.
    /// </summary>
    private void InitLookup()
    {
        if (this.eventLogTypeLookup != null)
        {
            return;
        }

        var logTypes = EnumHelper.EnumToList<EventLogTypes>().ToDictionary(t => t, _ => true);

        new[] { EventLogTypes.Debug, EventLogTypes.Trace }.ForEach(
            debugTypes => logTypes.AddOrUpdate(debugTypes, IsDebug));

        this.eventLogTypeLookup = logTypes;
    }
}