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
namespace YAF.Core.Services.Logger
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Utils.Helpers;

    using EventLog = YAF.Types.Models.EventLog;

    #endregion

    /// <summary>
    ///     The YAF Data Base logger.
    /// </summary>
    public class DbLogger : ILogger, IHaveServiceLocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbLogger"/> class.
        /// </summary>
        /// <param name="logType">
        /// The log type.
        /// </param>
        public DbLogger([CanBeNull] Type logType)
        {
            this.Type = logType;
        }

        /// <summary>
        /// Gets the event log repository.
        /// </summary>
        /// <value>
        /// The event log repository.
        /// </value>
        public IRepository<EventLog> EventLogRepository => this.GetRepository<Types.Models.EventLog>();

        /// <summary>
        /// Gets or sets the service locator.
        /// </summary>
        [Inject]
        public IServiceLocator ServiceLocator { get; set; }        

#if (DEBUG)

        /// <summary>
        ///     The _is debug.
        /// </summary>
        private bool isDebug = true;
#else
    private bool isDebug = false;
#endif

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
            if (this._eventLogTypeLookup == null)
            {
                // create it...
                this.InitLookup();
            }

            return this._eventLogTypeLookup.ContainsKey(type) && this._eventLogTypeLookup[type];
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
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void Log(
            string message, EventLogTypes eventType = EventLogTypes.Error, string username = null, string source = null, Exception exception = null)
        {
            if (!this.IsLogTypeEnabled(eventType))
            {
                return;
            }

            var exceptionDescription = string.Empty;

            if (exception != null)
            {
                exceptionDescription = exception.ToString();
            }

            var formattedDescription = HttpContext.Current != null
                                           ? $"{message} (URL:'{HttpContext.Current.Request.Url}')\r\n{exceptionDescription}"
                                           : $"{message}\r\n{exceptionDescription}";

            if (source.IsNotSet())
            {
                if (this.Type != null)
                {
                    source = this.Type.FullName.Length > 50 ? this.Type.FullName.Truncate(47) : this.Type.FullName;
                }
                else
                {
                    source = string.Empty;
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
                    if (BoardContext.Current.Get<BoardSettings>().LogInformation)
                    {
                        this.EventLogRepository.Insert(
                            new EventLog
                                {
                                    EventType = eventType,
                                    UserName = username,
                                    Description = formattedDescription,
                                    Source = source,
                                    EventTime = DateTime.UtcNow
                                });
                    }

                    break;
                case EventLogTypes.Warning:
                    if (BoardContext.Current.Get<BoardSettings>().LogWarning)
                    {
                        this.EventLogRepository.Insert(
                            new EventLog
                                {
                                    EventType = eventType,
                                    UserName = username,
                                    Description = formattedDescription,
                                    Source = source,
                                    EventTime = DateTime.UtcNow
                                });
                    }

                    break;
                case EventLogTypes.Error:
                    if (BoardContext.Current.Get<BoardSettings>().LogError)
                    {
                        this.EventLogRepository.Insert(
                            new EventLog
                                {
                                    EventType = eventType,
                                    UserName = username,
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
                                          UserName = username,
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
        ///     The _event log type lookup.
        /// </summary>
        private Dictionary<EventLogTypes, bool> _eventLogTypeLookup;

        /// <summary>
        /// Inits. the lookup.
        /// </summary>
        private void InitLookup()
        {
            if (this._eventLogTypeLookup != null)
            {
                return;
            }

            var logTypes = EnumHelper.EnumToList<EventLogTypes>().ToDictionary(t => t, v => true);

            new[] { EventLogTypes.Debug, EventLogTypes.Trace }.ForEach(
                debugTypes => { logTypes.AddOrUpdate(debugTypes, this.isDebug); });

            this._eventLogTypeLookup = logTypes;
        }
    }
}