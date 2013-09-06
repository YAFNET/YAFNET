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
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;

    using YAF.Types.Models;

    /// <summary>
    /// The Yaf Session Interface
    /// </summary>
    public interface IYafSession
    {
        /// <summary>
        /// Gets or sets Twitter Token.
        /// </summary>
        string TwitterToken { get; set; }

        /// <summary>
        /// Gets or sets Twitter Token Secret.
        /// </summary>
        string TwitterTokenSecret { get; set; }

        /// <summary>
        /// Gets or sets the multi quote ids.
        /// </summary>
        /// <value>
        /// The multi quote ids.
        /// </value>
        List<int> MultiQuoteIds { get; set; }

        /// <summary>
        ///   Gets or sets Unread Topic Since.
        /// </summary>
        int? UnreadTopicSince { get; set; }

        /// <summary>
        ///   Gets or sets User Topic Since.
        /// </summary>
        int? UserTopicSince { get; set; }

        /// <summary>
        ///   Gets or sets ActiveTopicSince.
        /// </summary>
        int? ActiveTopicSince { get; set; }

         /// <summary>
        ///   Gets or sets UnansweredTopicSince.
        /// </summary>
        int? UnansweredTopicSince { get; set; }

        /// <summary>
        /// Gets or sets if the user wants to use the mobile theme.
        /// </summary>
        bool? UseMobileTheme { get; set; }

        /// <summary>
        ///   Gets or sets FavoriteTopicSince.
        /// </summary>
        int? FavoriteTopicSince { get; set; }

        /// <summary>
        ///   Gets or sets ForumRead.
        /// </summary>
        Hashtable ForumRead { get; set; }

        /// <summary>
        ///   Gets or sets LastPm.
        /// </summary>
        DateTime LastPendingBuddies { get; set; }

        /// <summary>
        ///   Gets or sets LastPm.
        /// </summary>
        DateTime LastPm { get; set; }

        /// <summary>
        ///   Gets or sets LastPost.
        /// </summary>
        DateTime LastPost { get; set; }

        /// <summary>
        ///   Gets or sets LastVisit.
        /// </summary>
        DateTime? LastVisit { get; set; }

        /// <summary>
        ///   Gets PanelState.
        /// </summary>
        [NotNull]
        IPanelSessionState PanelState { get; }

        /// <summary>
        ///   Gets or sets SearchData.
        /// </summary>
        [CanBeNull]
        SearchResult[] SearchData { get; set; }

        /// <summary>
        ///   Gets or sets ShowList.
        /// </summary>
        int ShowList { get; set; }

        /// <summary>
        ///   Gets or sets TopicRead.
        /// </summary>
        Hashtable TopicRead { get; set; }

        /// <summary>
        ///   Gets or sets UnreadTopics.
        /// </summary>
        int UnreadTopics { get; set; }

        /// <summary>
        /// Gets the last time the forum was read.
        /// </summary>
        /// <param name="forumID">
        /// This is the ID of the forum you wish to get the last read date from.
        /// </param>
        /// <returns>
        /// A DateTime object of when the forum was last read.
        /// </returns>
        DateTime GetForumRead(int forumID);

        /// <summary>
        /// Returns the last time that the topicID was read.
        /// </summary>
        /// <param name="topicID">
        /// The topicID you wish to find the DateTime object for.
        /// </param>
        /// <returns>
        /// The DateTime object from the topicID.
        /// </returns>
        DateTime GetTopicRead(int topicID);

        /// <summary>
        /// Sets the time that the forum was read.
        /// </summary>
        /// <param name="forumID">
        /// The forum ID that was read.
        /// </param>
        /// <param name="date">
        /// The DateTime you wish to set the read to.
        /// </param>
        void SetForumRead(int forumID, DateTime date);

        /// <summary>
        /// Sets the time that the <paramref name="topicID"/> was read.
        /// </summary>
        /// <param name="topicID">
        /// The topic ID that was read.
        /// </param>
        /// <param name="date">
        /// The DateTime you wish to set the read to.
        /// </param>
        void SetTopicRead(int topicID, DateTime date);
    }
}