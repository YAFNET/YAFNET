/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

    /// <summary>
    /// Read Tracking Methods
    /// </summary>
    public interface IReadTracking
    {
        /// <summary>
        /// Add Or Update The Forum Read DateTime
        /// </summary>
        /// <param name="userID">
        /// The user ID.
        /// </param>
        /// <param name="forumID">
        /// The forum ID of the Forum
        /// </param>
        void SetForumRead(int userID, int forumID);

        /// <summary>
        /// Returns the last time that the forum was read or marked as Read.
        /// </summary>
        /// <param name="userID">
        /// The user ID.
        /// </param>
        /// <param name="forumID">
        /// The forum ID of the Forum
        /// </param>
        /// <returns>
        /// Returns the DateTime object from the Forum ID.
        /// </returns>
        DateTime GetForumRead(int userID, int forumID);

        /// <summary>
        /// Returns the last time that the Topic was read.
        /// </summary>
        /// <param name="userID">
        /// The user ID.
        /// </param>
        /// <param name="topicID">
        /// The topicID you wish to find the DateTime object for.
        /// </param>
        /// <returns>
        /// Returns the DateTime object from the topicID.
        /// </returns>
        DateTime GetTopicRead(int userID, int topicID);
    }
}