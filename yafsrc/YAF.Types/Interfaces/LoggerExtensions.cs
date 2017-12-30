/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Types.Interfaces
{
    using System;

    using YAF.Types.Constants;
    using YAF.Types.Extensions;

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
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Debug(this ILogger logger, [NotNull] string format, [NotNull] params object[] args)
        {
            logger.Log(format.FormatWith(args), EventLogTypes.Debug);
        }

        /// <summary>
        /// The error.
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
        public static void Error(this ILogger logger, [NotNull] string format, [NotNull] params object[] args)
        {
            logger.Log(format.FormatWith(args), EventLogTypes.Error);
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
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Error(this ILogger logger, Exception ex, [NotNull] string format, [NotNull] params object[] args)
        {
            logger.Log(format.FormatWith(args), EventLogTypes.Error, exception: ex);
        }

        /// <summary>
        /// The fatal.
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
        public static void Fatal(this ILogger logger, [NotNull] string format, [NotNull] params object[] args)
        {
            logger.Log(format.FormatWith(args), EventLogTypes.Error);
        }

        /// <summary>
        /// The fatal.
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
        public static void Fatal(this ILogger logger, Exception ex, [NotNull] string format, [NotNull] params object[] args)
        {
            logger.Log(format.FormatWith(args), EventLogTypes.Error, exception: ex);
        }

        /// <summary>
        /// The info.
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
        public static void Info(this ILogger logger, [NotNull] string format, [NotNull] params object[] args)
        {
            logger.Log(format.FormatWith(args), EventLogTypes.Information);
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="username">
        /// The username.
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
            [NotNull] this ILogger logger, 
            [CanBeNull] string username, 
            [CanBeNull] object source, 
            [NotNull] string description, 
            [NotNull] EventLogTypes eventType = EventLogTypes.Error)
        {
            CodeContracts.VerifyNotNull(logger, "logger");

            var sourceDescription = "unknown";

            if (source is Type)
            {
                sourceDescription = source.GetType().FullName;
            }
            else if (source != null)
            {
                sourceDescription = source.ToString();
            }

            logger.Log(description, eventType, username, sourceDescription);
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
        public static void Trace(this ILogger logger, [NotNull] string format, [NotNull] params object[] args)
        {
            logger.Log(format.FormatWith(args), EventLogTypes.Trace);
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
        public static void Warn(this ILogger logger, [NotNull] string format, [NotNull] params object[] args)
        {
            logger.Log(format.FormatWith(args), EventLogTypes.Warning);
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
        public static void Warn(this ILogger logger, Exception ex, [NotNull] string format, [NotNull] params object[] args)
        {
            logger.Log(format.FormatWith(args), EventLogTypes.Warning, exception: ex);
        }

        #endregion
    }
}