/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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

namespace YAF.Classes
{
  /// <summary>
  /// The date time format.
  /// </summary>
  public enum DateTimeFormat
  {
    /// <summary>
    /// The both.
    /// </summary>
    Both, 

    /// <summary>
    /// The both date short.
    /// </summary>
    BothDateShort, 

    /// <summary>
    /// The time.
    /// </summary>
    Time, 

    /// <summary>
    /// The date short.
    /// </summary>
    DateShort, 

    /// <summary>
    /// The date long.
    /// </summary>
    DateLong, 

    /// <summary>
    /// The both topic.
    /// </summary>
    BothTopic
  }

  /// <summary>
  /// The search field flags.
  /// </summary>
  public enum SearchFieldFlags
  {
    /// <summary>
    ///   The message.
    /// </summary>
    Message = 0, 

    /// <summary>
    ///   The user name.
    /// </summary>
    UserName = 1
  }

  /// <summary>
  /// The search what flags.
  /// </summary>
  public enum SearchWhatFlags
  {
    /// <summary>
    ///   The all words.
    /// </summary>
    AllWords = 0, 

    /// <summary>
    ///   The any words.
    /// </summary>
    AnyWords = 1, 

    /// <summary>
    ///   The exact match.
    /// </summary>
    ExactMatch = 2
  }

  /// <summary>
  /// The event log types.
  /// </summary>
  public enum EventLogTypes
  {
    /// <summary>
    ///   The error.
    /// </summary>
    Error = 0, 

    /// <summary>
    ///   The warning.
    /// </summary>
    Warning = 1, 

    /// <summary>
    ///   The information.
    /// </summary>
    Information = 2
  }

  /// <summary>
  /// The view permissions.
  /// </summary>
  public enum ViewPermissions
  {
    /// <summary>
    ///   The nobody.
    /// </summary>
    Nobody = 0, 

    /// <summary>
    ///   The registered users.
    /// </summary>
    RegisteredUsers = 1, 

    /// <summary>
    ///   The everyone.
    /// </summary>
    Everyone = 2
  }

  /// <summary>
  /// The user communication type.
  /// </summary>
  public enum UserNotificationSetting
  {
    /// <summary>
    ///   No Notifications
    /// </summary>
    NoNotification = 10, 

    /// <summary>
    ///   The all topics.
    /// </summary>
    AllTopics = 20, 

    /// <summary>
    ///   The topics I post to or subscribe to.
    /// </summary>
    TopicsIPostToOrSubscribeTo = 30, 

    /// <summary>
    ///   The topics i subscribe to.
    /// </summary>
    TopicsISubscribeTo = 0
  }

  /// <summary>
  /// The yaf rss feeds.
  /// </summary>
  public enum YafRssFeeds
  {
    /// <summary>
    ///   The latest posts.
    /// </summary>
    LatestPosts, 

    /// <summary>
    ///   The latest announcements.
    /// </summary>
    LatestAnnouncements, 

    /// <summary>
    ///   The posts.
    /// </summary>
    Posts, 

    /// <summary>
    ///   The forum.
    /// </summary>
    Forum, 

    /// <summary>
    ///   The topics.
    /// </summary>
    Topics, 

    /// <summary>
    ///   The active.
    /// </summary>
    Active, 

    /// <summary>
    ///   The favorite.
    /// </summary>
    Favorite
  }

  /// <summary>
  /// The yaf syndication formats.
  /// </summary>
  public enum YafSyndicationFormats
  {
    /// <summary>
    ///   The RSS format.
    /// </summary>
    Rss, 

    /// <summary>
    ///   The atom format.
    /// </summary>
    Atom
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