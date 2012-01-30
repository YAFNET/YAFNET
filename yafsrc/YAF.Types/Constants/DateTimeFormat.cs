/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
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
  /// The date time format.
  /// </summary>
  public enum DateTimeFormat
  {
    /// <summary>
    ///   The both.
    /// </summary>
    Both, 

    /// <summary>
    ///   The both date short.
    /// </summary>
    BothDateShort, 

    /// <summary>
    ///   The time.
    /// </summary>
    Time, 

    /// <summary>
    ///   The date short.
    /// </summary>
    DateShort, 

    /// <summary>
    ///   The date long.
    /// </summary>
    DateLong, 

    /// <summary>
    ///   The both topic.
    /// </summary>
    BothTopic
  }

  /* Ederon
	public enum ForumFlags : int
	{
		Locked = 1,						// users can't post/edit/delete topics in such forum
		Hidden = 2,						// only users with read access can see such forum, hidden to others
		IsTest = 4,						// messages posted in such forum do not count to user's postcount
		Moderated = 8					// messages in such forums have to be approved by mod before they are published
	}

	public enum GroupFlags : int
	{
		IsAdmin = 1,					// users of this group can administer board
		IsGuest = 2,					// users of this group are guests (typically one group)
		IsStart = 4,					// new users are automatically members of such group
		IsModerator = 8					// users of this group have moderatio rights (???)
	}

	public enum AccessFlags : int
	{
		ReadAccess = 1,					// can view/read topic/posts
		PostAccess = 2,					// can post new topics/posts
		ReplyAccess = 4,				// can reply to posts
		PriorityAccess = 8,				// can set/change topic's priority (normal/sticky/announcement)
		PollAccess = 16,				// can create polls
		VoteAccess = 32,				// can vote in polls
		ModeratorAccess = 64,			// can moderate forum - edit, move, delete all posts/topics, grant access to users
		EditAccess = 128,				// can edit own posts
		DeleteAccess = 256,				// can delete own posts
		UploadAccess = 512				// can upload/attach files to posts
	}

	public enum TopicFlags : int
	{
		Locked = 1,						// topic is locked, it cannot be edited
		Deleted = 8,					// topic is deleted (but still in the database), moderators can undelete it
		Persistent = 512				// topic is not subject to purging
	}

	public enum UserFlags : int
	{
		IsHostAdmin = 1,				// user is host administrator
		Approved = 2,					// user us approved (by administrator or after verifying registration email)
		IsGuest = 4						// user is guest - not registered/logged in
	}

	public enum RankFlags : int
	{
		IsStart = 1,					// rank of this type is default rank for new users
		IsLadder = 2					// user can advance to such rank by posting (increasing postcount)
	}
	*/
}