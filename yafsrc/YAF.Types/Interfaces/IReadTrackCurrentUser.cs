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

	#endregion

	/// <summary>
	/// Read Tracking Interface for the Current User
	/// </summary>
	public interface IReadTrackCurrentUser
	{
		#region Public Methods

		/// <summary>
		/// Returns the last time that the forum was read or marked as Read.
		/// </summary>
		/// <param name="forumID">
		/// The forum ID of the Forum 
		/// </param>
		/// <returns>
		/// Returns the DateTime object from the Forum ID. 
		/// </returns>
		DateTime GetForumRead(int forumID, DateTime? readTimeOverride = null);

		/// <summary>
		/// Returns the last time that the Topic was read.
		/// </summary>
		/// <param name="topicID">
		/// The topicID you wish to find the DateTime object for. 
		/// </param>
		/// <returns>
		/// Returns the DateTime object from the topicID. 
		/// </returns>
		DateTime GetTopicRead(int topicID, DateTime? readTimeOverride = null);

		/// <summary>
		/// Get the Global Last Read DateTime a user Reads a topic or marks a forum as read
		/// </summary>
		/// <returns>
		/// Returns the DateTime object with the last read date. 
		/// </returns>
		DateTime LastRead { get; }

		/// <summary>
		/// Add Or Update The Forum Read DateTime
		/// </summary>
		/// <param name="forumID">
		/// The forum ID of the Forum 
		/// </param>
		void SetForumRead(int forumID);

		/// <summary>
		/// Add Or Update The topic Read DateTime
		/// </summary>
		/// <param name="topicID">
		/// The topic id to mark read.
		/// </param>
		void SetTopicRead(int topicID);

		#endregion
	}
}