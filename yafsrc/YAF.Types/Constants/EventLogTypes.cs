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
namespace YAF.Types.Constants
{
    /// <summary>
    ///     The event log types. Always use the same numbers in the enumeration and NEVER change the pairs.
    /// </summary>
    public enum EventLogTypes
    {
        /// <summary>
        /// The debug.
        /// </summary>
        Debug = -1000, 

        /// <summary>
        /// The trace.
        /// </summary>
        Trace = -500, 

        /// <summary>
        ///     The error.
        /// </summary>
        Error = 0, 

        /// <summary>
        ///     The warning.
        /// </summary>
        Warning = 1, 

        /// <summary>
        ///     The information.
        /// </summary>
        Information = 2, 

        /// <summary>
        ///     The sql error.
        /// </summary>
        SqlError = 3, 

        /// <summary>
        ///     The user suspended.
        /// </summary>
        UserSuspended = 1000, 

        /// <summary>
        ///     The user unsuspended.
        /// </summary>
        UserUnsuspended = 1001, 

        /// <summary>
        ///     The user deleted.
        /// </summary>
        UserDeleted = 1002, 

        /// <summary>
        ///     The ban set.
        /// </summary>
        IpBanSet = 1003, 

        /// <summary>
        ///     The Ip Ban Lifted.
        /// </summary>
        IpBanLifted = 1004
    }
}