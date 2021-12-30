/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
namespace YAF.Types.Extensions
{
    using System;

    using YAF.Types.Constants;
    using YAF.Types.Interfaces.Services;

    /// <summary>
    ///     The logger extensions.
    /// </summary>
    public static class LoggerExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Debug(this ILoggerService logger, [NotNull] string message)
        {
            logger.Log(message, EventLogTypes.Debug);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Error(this ILoggerService logger, Exception ex, [NotNull] string message)
        {
            logger.Log(message, EventLogTypes.Error, exception: ex);
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Info(this ILoggerService logger, [NotNull] string message)
        {
            logger.Log(message, EventLogTypes.Information);
        }

        /// <summary>
        /// Log user deleted.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public static void UserDeleted(
            [NotNull] this ILoggerService logger,
            [CanBeNull] int? userId,
            [NotNull] string description)
        {
            CodeContracts.VerifyNotNull(logger);

            logger.Log(description, EventLogTypes.UserDeleted, userId, "User Deleted");
        }

        /// <summary>
        /// Log spam message detected.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public static void SpamMessageDetected(
            [NotNull] this ILoggerService logger,
            [CanBeNull] int? userId,
            [NotNull] string description)
        {
            CodeContracts.VerifyNotNull(logger);

            logger.Log(description, EventLogTypes.SpamMessageDetected, userId, "Spam Message Detected");
        }

        /// <summary>
        /// Log spam bot detected.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public static void SpamBotDetected(
            [NotNull] this ILoggerService logger,
            [CanBeNull] int? userId,
            [NotNull] string description)
        {
            CodeContracts.VerifyNotNull(logger);

            logger.Log(description, EventLogTypes.SpamBotDetected, userId, "Bot Detected");
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
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
        public static void Log(
            [NotNull] this ILoggerService logger,
            [CanBeNull] int? userId,
            [CanBeNull] object source,
            [NotNull] string description,
            [NotNull] EventLogTypes eventType = EventLogTypes.Error)
        {
            CodeContracts.VerifyNotNull(logger);

            var sourceDescription = "unknown";

            if (source is Type)
            {
                sourceDescription = source.GetType().FullName;
            }
            else if (source != null)
            {
                sourceDescription = source.ToString();
            }

            logger.Log(description, eventType, userId, sourceDescription);
        }

        /// <summary>
        /// The trace.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Trace(this ILoggerService logger, [NotNull] string format, [NotNull] params object[] args)
        {
            logger.Log(string.Format(format, args), EventLogTypes.Trace);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Warn(this ILoggerService logger, [NotNull] string format, [NotNull] params object[] args)
        {
            logger.Log(string.Format(format, args), EventLogTypes.Warning);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Warn(this ILoggerService logger, Exception ex, [NotNull] string format, [NotNull] params object[] args)
        {
            logger.Log(string.Format(format, args), EventLogTypes.Warning, exception: ex);
        }

        #endregion
    }
}