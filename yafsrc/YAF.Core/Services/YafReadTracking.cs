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

namespace YAF.Core.Services
{
		using System;

		using YAF.Types.Interfaces;
		using YAF.Utils;

	/// <summary>
		/// YAF Read Tracking Methods
		/// </summary>
		public class YafReadTracking : IReadTracking
		{
			private readonly IDbFunction _dbFunction;

			public YafReadTracking(IDbFunction dbFunction)
			{
				_dbFunction = dbFunction;
			}

			#region Public Methods

				/// <summary>
				/// Add Or Update The Forum Read DateTime
				/// </summary>
				/// <param name="userID">
				/// The user ID.
				/// </param>
				/// <param name="forumID">
				/// The forum ID of the Forum
				/// </param>
				public void SetForumRead(int userID, int forumID)
				{
						if (!YafContext.Current.IsGuest)
						{
							this._dbFunction.Query.readforum_addorupdate(userID, forumID);
						}
				}

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
				public DateTime GetForumRead(int userID, int forumID)
				{
					return YafContext.Current.IsGuest
					       	? DateTime.UtcNow
					       	: ((object)this._dbFunction.Scalar.ReadForum_lastread(userID, forumID)).ToType<DateTime?>()
					       	  ?? DateTime.MinValue.AddYears(1902);
				}

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
				/// Returns the  DateTime object from the topicID.
				/// </returns>
				public DateTime GetTopicRead(int userID, int topicID)
				{
					return YafContext.Current.IsGuest
					       	? DateTime.UtcNow
					       	: ((object)this._dbFunction.Scalar.Readtopic_lastread(userID, topicID)).ToType<DateTime?>()
					       	  ?? DateTime.MinValue.AddYears(1902);
				}

				/// <summary>
				/// Get the Global Last Read DateTime a user Reads a topic or marks a forum as read
				/// </summary>
				/// <param name="userID">
				/// The user ID.
				/// </param>
				/// <returns>
				/// Returns the DateTime object with the last read date.
				/// </returns>
				public DateTime GetUserLastRead(int userID)
				{
					return YafContext.Current.IsGuest
					       	? YafContext.Current.Get<IYafSession>().LastVisit
					       	: ((object)this._dbFunction.Scalar.User_LastRead(userID)).ToType<DateTime?>()
					       	  ?? YafContext.Current.Get<IYafSession>().LastVisit;
				}
				
				#endregion
		}
}