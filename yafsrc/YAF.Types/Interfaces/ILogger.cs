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
    #region Using

    using System;

    using YAF.Types.Constants;

    #endregion

    /// <summary>
    ///     The logger interface
    /// </summary>
    public interface ILogger
    {
        #region Public Methods and Operators

        /// <summary>
        /// The is log type enabled.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsLogTypeEnabled(EventLogTypes type);

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
        void Log(
            string message, EventLogTypes eventType = EventLogTypes.Error, string username = null, string source = null, Exception exception = null);

        #endregion
    }
}