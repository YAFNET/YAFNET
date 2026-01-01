/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Context;

using System;
using System.Collections.Generic;
using System.Threading;

using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
/// PageUser Page Class.
/// </summary>
public abstract class UserPageBase
{
    /// <summary>
    /// Gets or sets a value indicating whether the page data is loaded.
    /// </summary>
    public bool UserPageDataLoaded { get; set; }

    /// <summary>
    ///   Gets a value indicating whether the current user can delete own messages in the current forum (True).
    /// </summary>
    public bool ForumDeleteAccess => this.PageData.Item2.Item1.DeleteAccess;

    /// <summary>
    ///   Gets a value indicating whether the current user can download attachments (True).
    /// </summary>
    public bool DownloadAccess => this.PageData.Item2.Item1.DownloadAccess;

    /// <summary>
    ///   Gets a value indicating whether the current user can edit own messages in the current forum (True).
    /// </summary>
    public bool ForumEditAccess => this.PageData.Item2.Item1.EditAccess;

    /// <summary>
    ///   Gets a value indicating whether the current user is a moderator of the current forum (True).
    /// </summary>
    public bool ForumModeratorAccess => this.PageData.Item2.Item1.ModeratorAccess;

    /// <summary>
    ///   Gets a value indicating whether the current user has access to create polls in the current forum (True).
    /// </summary>
    public bool ForumPollAccess => this.PageData.Item2.Item1.PollAccess;

    /// <summary>
    ///   Gets a value indicating whether the current user has post access in the current forum (True).
    /// </summary>
    public bool ForumPostAccess => this.PageData.Item2.Item1.PostAccess;

    /// <summary>
    ///   Gets a value indicating whether the current user has access to create priority topics in the current forum (True).
    /// </summary>
    public bool ForumPriorityAccess => this.PageData.Item2.Item1.PriorityAccess;

    /// <summary>
    ///   Gets a value indicating whether the current user has read access in the current forum (True)
    /// </summary>
    public bool ForumReadAccess => this.PageData.Item2.Item1.ReadAccess;

    /// <summary>
    ///   Gets a value indicating whether the current user has reply access in the current forum (True)
    /// </summary>
    public bool ForumReplyAccess => this.PageData.Item2.Item1.ReplyAccess;

    /// <summary>
    ///   Gets a value indicating whether the current user can upload attachments (True).
    /// </summary>
    public bool UploadAccess => this.PageData.Item2.Item1.UploadAccess;

    /// <summary>
    ///   Gets a value indicating whether the current user has access to vote on polls in the current forum (True).
    /// </summary>
    public bool ForumVoteAccess => this.PageData.Item2.Item1.VoteAccess;

    /// <summary>
    ///   Gets a value indicating whether the  current user is an administrator (True).
    /// </summary>
    public bool IsForumAdmin => this.PageData.Item2.Item1.IsAdmin;

    /// <summary>
    ///   Gets a value indicating whether the current user IsCrawler (True).
    /// </summary>
    public bool IsCrawler => this.PageData.Item2.Item1.IsCrawler;

    /// <summary>
    ///   Gets a value indicating whether the current user is a forum moderator (mini-admin) (True).
    /// </summary>
    public bool IsForumModerator => this.PageData.Item2.Item1.IsForumModerator;

    /// <summary>
    ///   Gets a value indicating whether the current user is a guest (True).
    /// </summary>
    public bool IsGuest => this.PageData.Item3.IsGuest;

    /// <summary>
    ///   Gets a value indicating whether the current user is a moderator for at least one forum (True);
    /// </summary>
    public bool IsModeratorInAnyForum =>
        this.PageData.Item2.Item1.IsModerator || this.PageData.Item2.Item1.IsModeratorAny;

    /// <summary>
    ///   Gets a value indicating whether the current user is suspended (True).
    /// </summary>
    public bool IsSuspended => this.PageData.Item3.Suspended.HasValue;

    /// <summary>
    ///   Gets the number of pending buddy requests.
    /// </summary>
    public DateTime LastPendingBuddies => this.PageData.Item3.LastPendingBuddies;

    /// <summary>
    ///   Gets the number of albums which a user already has
    /// </summary>
    public int NumAlbums => this.PageData.Item3.NumAlbums;

    public DateTime? LastPosted => this.PageData.Item2.Item1.LastPosted;

    /// <summary>
    ///   Gets or sets Page.
    /// </summary>
    public virtual Tuple<UserRequestData, Tuple<PageLoad, User, Category, Forum, Topic, Message>, UserLazyData, PageQueryData> PageData
    {
        get
        {
            if (this.UserPageDataLoaded || !Monitor.TryEnter(this))
            {
                return field;
            }

            try
            {
                if (!this.UserPageDataLoaded)
                {
                    this.InitUserAndPage();
                }
            }
            finally
            {
                Monitor.Exit(this);
            }

            return field;
        }

        set
        {
            field = value;
            this.UserPageDataLoaded = value != null;
        }
    }

    /// <summary>
    ///   Gets PageBoardID.
    /// </summary>
    public int PageBoardID => BoardContext.Current.Get<ControlSettings>().BoardID;

    /// <summary>
    ///   Gets the CategoryID for the current page, or 0 if not in any category
    /// </summary>
    public int PageCategoryID => BoardContext.Current.Settings.CategoryID != 0
                                     ? BoardContext.Current.Settings.CategoryID
                                     : this.PageData.Item4.CategoryID;

    /// <summary>
    /// The page category.
    /// </summary>
    public Category PageCategory => this.PageData.Item2.Item3;

    /// <summary>
    ///   Gets the ForumID for the current page, or 0 if not in any forum
    /// </summary>
    public int PageForumID => this.PageData.Item4.ForumID;

    /// <summary>
    ///   Gets the Name of forum for the current page, or an empty string if not in any forum
    /// </summary>
    public Forum PageForum => this.PageData.Item2.Item4;

    /// <summary>
    ///   Gets the  TopicID of the current page, or 0 if not in any topic
    /// </summary>
    public int PageTopicID => this.PageData.Item4.TopicID;

    /// <summary>
    ///   Gets the Name of topic for the current page, or an empty string if not in any topic
    /// </summary>
    public Topic PageTopic => this.PageData.Item2.Item5;

    /// <summary>
    /// The page message if exist and in the page query
    /// </summary>
    public Message PageMessage => this.PageData.Item2.Item6;

    /// <summary>
    ///   Gets the UserID of the current user.
    /// </summary>
    public int PageUserID => this.PageData.Item2.Item2.ID;

    /// <summary>
    /// The guest user id.
    /// </summary>
    public int GuestUserID => this.PageData.Item2.Item1.GuestUserID;

    /// <summary>
    ///   Gets the number of pending buddy requests
    /// </summary>
    public int PendingBuddies => this.PageData.Item3.PendingBuddies;

    /// <summary>
    ///   Gets the DateTime the user is suspended until
    /// </summary>
    public DateTime SuspendedUntil => this.IsSuspended ? this.PageData.Item3.Suspended ?? DateTime.UtcNow : DateTime.UtcNow;

    /// <summary>
    ///   Gets the DateTime the user is suspended until
    /// </summary>
    public string SuspendedReason => this.IsSuspended ? this.PageData.Item3.SuspendedReason : string.Empty;

    /// <summary>
    ///   Gets the time zone offset for the user
    /// </summary>
    public string TimeZoneUser => this.PageData.Item3.TimeZoneUser;

    /// <summary>
    /// Gets the time zone information user.
    /// </summary>
    /// <value>
    /// The time zone information user.
    /// </value>
    public TimeZoneInfo TimeZoneInfoUser => DateTimeHelper.GetTimeZoneInfo(this.TimeZoneUser);

    /// <summary>
    /// Gets the time zone user off set.
    /// </summary>
    /// <value>
    /// The time zone user off set.
    /// </value>
    public int TimeZoneUserOffSet => DateTimeHelper.GetTimeZoneOffset(this.PageData.Item3.TimeZoneUser);

    /// <summary>
    /// Number of Unread New Topics in a Watch Forum and/or Replies in a Watch Topic
    /// </summary>
    public int WatchTopic => this.PageData.Item3.WatchTopic;

    /// <summary>
    /// The received thanks.
    /// </summary>
    public int ReceivedThanks => this.PageData.Item3.ReceivedThanks;

    /// <summary>
    /// The mention.
    /// </summary>
    public int Mention => this.PageData.Item3.Mention;

    /// <summary>
    /// The quoted.
    /// </summary>
    public int Quoted => this.PageData.Item3.Quoted;

    /// <summary>
    ///   Gets the number of private messages that are unread
    /// </summary>
    public int UnreadPrivate => this.PageData.Item3.UnreadPrivate;

    /// <summary>
    ///   Gets the number of posts that needs moderating
    /// </summary>
    public int ModeratePosts => this.PageData.Item3.ModeratePosts;

    /// <summary>
    ///   Gets a value indicating whether a user has buddies
    /// </summary>
    public bool UserHasBuddies => this.PageData.Item3.UserHasBuddies;

    /// <summary>
    /// Gets a value indicating whether [user has private conversations].
    /// </summary>
    /// <value><c>true</c> if [user has private conversations]; otherwise, <c>false</c>.</value>
    public bool UserHasPrivateConversations => this.PageData.Item3.UserHasPrivateConversations;

    /// <summary>
    /// Gets the index of the page from the Pager.
    /// </summary>
    /// <value>The index of the page.</value>
    public int PageIndex => this.PageData.Item4.PageIndex > 0 ? this.PageData.Item4.PageIndex - 1 : 0;

    /// <summary>
    /// Gets or sets the page links (breadcrumb).
    /// </summary>
    /// <value>The page links.</value>
    public List<PageLink> PageLinks { get; set; }

    /// <summary>
    /// Helper function to see if the Page variable is populated
    /// </summary>
    /// <returns>
    /// The page is null.
    /// </returns>
    public bool PageIsNull()
    {
        return this.PageData is null;
    }

    /// <summary>
    /// Initialize the user data and page data...
    /// </summary>
    protected virtual void InitUserAndPage()
    {
    }
}