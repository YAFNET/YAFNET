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
namespace YAF.Core.Extensions
{
    using System;

    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    /// <summary>
    ///     The i logger extensions.
    /// </summary>
    public static class ILoggerExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="userId">
        /// The user id.
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
            int userId, 
            [CanBeNull] object source, 
            [NotNull] string description, 
            EventLogTypes eventType = EventLogTypes.Error)
        {
            CodeContracts.VerifyNotNull(logger, "logger");

            var username = YafContext.Current.Get<IUserDisplayName>().GetName(userId);

            var sourceDescription = "unknown";

            if (source is Type)
            {
                sourceDescription = source.GetType().FullName;
            }
            else if (source != null)
            {
                sourceDescription = source.ToString().Truncate(50);
            }

            logger.Log(description, eventType, username.IsNotSet() ? "N/A" : username, sourceDescription);
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        public static void Log(
            [NotNull] this ILogger logger, 
            [CanBeNull] int? userId, 
            [CanBeNull] object source, 
            [NotNull] Exception exception, 
            EventLogTypes eventType = EventLogTypes.Error)
        {
            CodeContracts.VerifyNotNull(logger, "logger");

            string username = null;

            if (userId.HasValue && userId > 0)
            {
                username = YafContext.Current.Get<IUserDisplayName>().GetName(userId.Value);
            }

            var sourceDescription = "unknown";

            if (source is Type)
            {
                sourceDescription = source.GetType().FullName;
            }
            else if (source != null)
            {
                sourceDescription = source.ToString().Truncate(50);
            }

            logger.Log("Exception", eventType, username, sourceDescription, exception);
        }

        #endregion
    }
}