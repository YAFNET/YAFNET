/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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