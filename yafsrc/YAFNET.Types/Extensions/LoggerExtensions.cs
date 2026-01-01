/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

#pragma warning disable CA2254
namespace YAF.Types.Extensions;

using Microsoft.Extensions.Logging;

using YAF.Types.Constants;
using YAF.Types.Models;

/// <summary>
///     The logger extensions.
/// </summary>
public static class LoggerExtensions
{
    /// <param name="logger">
    /// The logger.
    /// </param>
    extension(ILogger logger)
    {
        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Debug(string message)
        {
            var logEntry = new EventLog { Type = EventLogTypes.Debug.ToInt() };

            message = message.Replace('\n', '_').Replace('\r', '_');

            using (logger.BeginScope(logEntry))
            {
                logger.Log(LogLevel.Debug, message);
            }
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Error(Exception ex, string message)
        {
            var logEntry = new EventLog { Type = EventLogTypes.Error.ToInt(), Description = ex.ToString(), Exception = ex };

            message = message.Replace('\n', '_').Replace('\r', '_');

            using (logger.BeginScope(logEntry))
            {
                logger.Log(LogLevel.Error, message);
            }
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Info(string message)
        {
            var logEntry = new EventLog { Type = EventLogTypes.Information.ToInt() };

            message = message.Replace('\n', '_').Replace('\r', '_');

            using (logger.BeginScope(logEntry))
            {
                logger.Log(LogLevel.Information, message);
            }
        }

        /// <summary>
        /// Log user deleted.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public void UserDeleted(int? userId, string description)
        {
            var logEntry = new EventLog
            {
                Type = EventLogTypes.UserDeleted.ToInt(), Source = "User Deleted", UserID = userId
            };

            description = description.Replace('\n', '_').Replace('\r', '_');

            using (logger.BeginScope(logEntry))
            {
                logger.Log(LogLevel.Information, description);
            }
        }

        /// <summary>
        /// Log spam message detected.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public void SpamMessageDetected(int? userId,
            string description)
        {
            var logEntry = new EventLog
            {
                Type = EventLogTypes.SpamMessageDetected.ToInt(),
                Source = "Spam Message Detected",
                UserID = userId
            };

            description = description.Replace('\n', '_').Replace('\r', '_');

            using (logger.BeginScope(logEntry))
            {
                logger.Log(LogLevel.Information, description);
            }
        }

        /// <summary>
        /// Log spam bot detected.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public void SpamBotDetected(int? userId,
            string description)
        {
            var logEntry = new EventLog
            {
                Type = EventLogTypes.SpamBotDetected.ToInt(), Source = "Bot Detected", UserID = userId
            };

            description = description.Replace('\n', '_').Replace('\r', '_');

            using (logger.BeginScope(logEntry))
            {
                logger.Log(LogLevel.Information, description);
            }
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
        public void Log(string message,
            EventLogTypes eventType = EventLogTypes.Error,
            int? userId = null,
            string source = null,
            Exception exception = null)
        {
            var logEntry = new EventLog
            {
                Type = eventType.ToInt(), Source = source, UserID = userId, Exception = exception
            };

            message = message.Replace('\n', '_').Replace('\r', '_');

            using (logger.BeginScope(logEntry))
            {
                logger.Log(LogLevel.Information, message);
            }
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        public void Log(int? userId,
            object source,
            string description,
            EventLogTypes eventType = EventLogTypes.Error)
        {
            var sourceDescription = "unknown";

            if (source is Type)
            {
                sourceDescription = source.GetType().FullName;
            }
            else if (source is not null)
            {
                sourceDescription = source.ToString();
            }

            var logEntry = new EventLog { Type = eventType.ToInt(), Source = sourceDescription, UserID = userId };

            description = description.Replace('\n', '_').Replace('\r', '_');

            using (logger.BeginScope(logEntry))
            {
                logger.Log(LogLevel.Error, description);
            }
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Warn(string format, params object[] args)
        {
            var logEntry = new EventLog { Type = EventLogTypes.Warning.ToInt() };
            using (logger.BeginScope(logEntry))
            {
                logger.Log(LogLevel.Warning, string.Format(format, args));
            }
        }
    }
}