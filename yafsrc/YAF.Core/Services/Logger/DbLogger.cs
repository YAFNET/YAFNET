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

namespace YAF.Core.Services.Logger;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Diagnostics;

using YAF.Types.Constants;

using EventLog = YAF.Types.Models.EventLog;

/// <summary>
///     The YAF Data Base logger.
/// </summary>
public class DbLogger : ILoggerService, IHaveServiceLocator
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
    ///     The event log type lookup.
    /// </summary>
    private Dictionary<EventLogTypes, bool> eventLogTypeLookup;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbLogger"/> class.
    /// </summary>
    /// <param name="logType">
    /// The log type.
    /// </param>
    public DbLogger(Type logType)
    {
        this.Type = logType;
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
    [Inject]
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    ///     Gets or sets the logging type.
    /// </summary>
    public Type Type { get; set; }

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

        return this.eventLogTypeLookup!.ContainsKey(type) && this.eventLogTypeLookup[type];
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
    public void Log(
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

        var userIp = HttpContext.Current != null ? HttpContext.Current.Request.GetUserRealIPAddress() : string.Empty;

        var values = new JObject {
                                     ["Message"] = message,
                                     ["UserIP"] = userIp,
                                     ["Url"] = HttpContext.Current != null ? HtmlTagHelper.StripHtml(HttpContext.Current.Request.Url.ToString()) : "",
                                     ["UserAgent"] = HttpContext.Current != null ? HttpContext.Current.Request.UserAgent : "",
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
            if (this.Type != null)
            {
                source = this.Type.FullName!.Length > 50 ? this.Type.FullName.Truncate(47) : this.Type.FullName;
            }
            else
            {
                source = string.Empty;
            }
        }
        else
        {
            if (source!.Length > 50)
            {
                source = source.Truncate(47);
            }
        }

        switch (eventType)
        {
            case EventLogTypes.Debug:
                Debug.WriteLine(formattedDescription, source);
                break;
            case EventLogTypes.Trace:
                Trace.TraceInformation(formattedDescription);
                if (exception != null)
                {
                    Trace.TraceError(exception.ToString());
                }

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