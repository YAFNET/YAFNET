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
    using System.Collections.Generic;

    #endregion

	/// <summary>
	/// The read track current user extensions.
	/// </summary>
	public static class IReadTrackCurrentUserExtensions
	{
		#region Public Methods

		/// <summary>
		/// The get forum topic read.
		/// </summary>
		/// <param name="readTrackCurrentUser">
		/// The read track current user.
		/// </param>
		/// <param name="forumId">
		/// The forum id.
		/// </param>
		/// <param name="topicId">
		/// The topic id.
		/// </param>
		/// <param name="forumReadOverride">
		/// The forum read override.
		/// </param>
		/// <param name="topicReadOverride">
		/// The topic read override.
		/// </param>
		/// <returns>
		/// </returns>
		public static DateTime GetForumTopicRead(
			this IReadTrackCurrentUser readTrackCurrentUser, 
			int forumId, 
			int topicId, 
			DateTime? forumReadOverride = null, 
			DateTime? topicReadOverride = null)
		{
			CodeContracts.VerifyNotNull(readTrackCurrentUser, "readTrackCurrentUser");

			DateTime lastRead = readTrackCurrentUser.GetTopicRead(topicId, topicReadOverride);
			DateTime lastReadForum = readTrackCurrentUser.GetForumRead(forumId, forumReadOverride);

			if (lastReadForum > lastRead)
			{
				lastRead = lastReadForum;
			}

			return lastRead;
		}

		/// <summary>
		/// The set forum read.
		/// </summary>
		/// <param name="readTrackCurrentUser">
		/// The read track current user. 
		/// </param>
		/// <param name="forumIds">
		/// The forum ids. 
		/// </param>
		public static void SetForumRead(this IReadTrackCurrentUser readTrackCurrentUser, IEnumerable<int> forumIds)
		{
			CodeContracts.VerifyNotNull(readTrackCurrentUser, "readTrackCurrentUser");
			CodeContracts.VerifyNotNull(forumIds, "forumIds");

			foreach (var id in forumIds)
			{
				readTrackCurrentUser.SetForumRead(id);
			}
		}

		/// <summary>
		/// The set topic read.
		/// </summary>
		/// <param name="readTrackCurrentUser">
		/// The read track current user. 
		/// </param>
		/// <param name="topicIds">
		/// The topic ids. 
		/// </param>
		public static void SetTopicRead(this IReadTrackCurrentUser readTrackCurrentUser, IEnumerable<int> topicIds)
		{
			CodeContracts.VerifyNotNull(readTrackCurrentUser, "readTrackCurrentUser");
			CodeContracts.VerifyNotNull(topicIds, "topicIds");

			foreach (var id in topicIds)
			{
				readTrackCurrentUser.SetTopicRead(id);
			}
		}

		#endregion
	}
}