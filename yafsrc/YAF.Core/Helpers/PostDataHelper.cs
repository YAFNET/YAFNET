/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2017 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core
{
    using System;
    using System.Data;

    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    /// <summary>
    /// The post data helper wrapper.
    /// </summary>
    public class PostDataHelperWrapper
    {
        /// <summary>
        /// The _forum flags.
        /// </summary>
        private ForumFlags _forumFlags;

        /// <summary>
        /// The _message flags.
        /// </summary>
        private MessageFlags _messageFlags;

        /// <summary>
        /// The current data row for this post.
        /// </summary>
        private DataRow _row;

        /// <summary>
        /// The _topic flags.
        /// </summary>
        private TopicFlags _topicFlags;

        /// <summary>
        /// The _user profile.
        /// </summary>
        private YafUserProfile _userProfile;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostDataHelperWrapper"/> class.
        /// </summary>
        public PostDataHelperWrapper()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostDataHelperWrapper"/> class.
        /// </summary>
        /// <param name="dataRow">
        /// The data row.
        /// </param>
        public PostDataHelperWrapper(DataRow dataRow)
            : this()
        {
            DataRow = dataRow;
        }

        /// <summary>
        /// Gets or sets DataRow.
        /// </summary>
        public DataRow DataRow
        {
            get
            {
                return this._row;
            }

            set
            {
                this._row = value;

                // get all flags for forum, topic and message
                if (this._row != null)
                {
                    this._forumFlags = new ForumFlags(this._row["ForumFlags"]);
                    this._topicFlags = new TopicFlags(this._row["TopicFlags"]);
                    this._messageFlags = new MessageFlags(this._row["Flags"]);
                }
                else
                {
                    this._forumFlags = new ForumFlags(0);
                    this._topicFlags = new TopicFlags(0);
                    this._messageFlags = new MessageFlags(0);
                }
            }
        }

        /// <summary>
        /// Gets UserProfile.
        /// </summary>
        public YafUserProfile UserProfile
        {
            get
            {
                if (this._userProfile != null)
                {
                    return this._userProfile;
                }

                // setup instance of the user profile...
                if (this.DataRow != null)
                {
                    this._userProfile = YafUserProfile.GetProfile(UserMembershipHelper.GetUserNameFromID(this.UserId));
                }

                return this._userProfile;
            }
        }

        /// <summary>
        /// Gets UserId.
        /// </summary>
        public int UserId
        {
            get
            {
                return this.DataRow != null ? Convert.ToInt32(this.DataRow["UserID"]) : 0;
            }
        }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public int MessageId
        {
            get
            {
                return this.DataRow != null ? this.DataRow["MessageID"].ToType<int>() : 0;
            }
        }

        /// <summary>
        /// Gets the topic identifier.
        /// </summary>
        /// <value>
        /// The topic identifier.
        /// </value>
        public int TopicId
        {
            get
            {
                return this.DataRow != null ? this.DataRow["TopicID"].ToType<int>() : 0;
            }
        }

        /// <summary>
        /// IsLocked flag should only be used for "ghost" posts such as the
        /// Sponsor post that isn't really there.
        /// </summary>
        public bool IsLocked
        {
            get
            {
                return this._messageFlags != null && this._messageFlags.IsLocked;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is sponser message.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is sponser message; otherwise, <c>false</c>.
        /// </value>
        public bool IsSponserMessage
        {
            get
            {
                return DataRow["IP"].ToString() == "none";
            }
        }

        /// <summary>
        /// Gets a value indicating whether CanThankPost.
        /// </summary>
        public bool CanThankPost
        {
            get
            {
                return DataRow["UserID"].ToType<int>() != YafContext.Current.PageUserID;
            }
        }

        /// <summary>
        /// Gets a value indicating whether CanEditPost.
        /// </summary>
        public bool CanEditPost
        {
            get
            {
                // Ederon : 9/9/2007 - moderaotrs can edit locked posts
                // Ederon : 12/5/2007 - new flags implementation
                return ((!this.PostLocked && !this._forumFlags.IsLocked && !this._topicFlags.IsLocked
                         && (((this.UserId == YafContext.Current.PageUserID) && !DataRow["IsGuest"].ToType<bool>())
                             || (DataRow["IsGuest"].ToType<bool>()
                                 && (DataRow["IP"].ToString()
                                     == YafContext.Current.CurrentForumPage.Request.GetUserRealIPAddress()))))
                        || YafContext.Current.ForumModeratorAccess) && YafContext.Current.ForumEditAccess;
            }
        }

        /// <summary>
        /// Gets a value indicating whether PostLocked.
        /// </summary>
        public bool PostLocked
        {
            get
            {
                // post is explicitly locked
                if (this._messageFlags.IsLocked)
                {
                    return true;
                }

                // there is auto-lock period defined
                if (YafContext.Current.IsAdmin || YafContext.Current.BoardSettings.LockPosts <= 0)
                {
                    return false;
                }

                var edited = this.DataRow["Edited"].ToType<DateTime>();

                // check if post is locked according to this rule
                return edited.AddDays(YafContext.Current.BoardSettings.LockPosts) < DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Gets a value indicating whether PostDeleted.
        /// </summary>
        public bool PostDeleted
        {
            get
            {
                return this._messageFlags.IsDeleted;
            }
        }

        /// <summary>
        /// Gets a value indicating whether CanAttach.
        /// </summary>
        public bool CanAttach
        {
            get
            {
                // Ederon : 9/9/2007 - moderaotrs can attack to locked posts
                return ((!this.PostLocked && !this._forumFlags.IsLocked && !this._topicFlags.IsLocked
                         && this.UserId == YafContext.Current.PageUserID) || YafContext.Current.ForumModeratorAccess)
                       && YafContext.Current.ForumUploadAccess;
            }
        }

        /// <summary>
        /// Gets a value indicating whether CanDeletePost.
        /// </summary>
        public bool CanDeletePost
        {
            get
            {
                // Ederon : 9/9/2007 - moderators can delete in locked posts
                // vzrus : only guests with the same IP can delete guest posts 
                return ((!this.PostLocked && !this._forumFlags.IsLocked && !this._topicFlags.IsLocked
                         && (((this.UserId == YafContext.Current.PageUserID) && !DataRow["IsGuest"].ToType<bool>())
                             || (DataRow["IsGuest"].ToType<bool>()
                                 && (DataRow["IP"].ToString()
                                     == YafContext.Current.CurrentForumPage.Request.GetUserRealIPAddress()))))
                        || YafContext.Current.ForumModeratorAccess) && YafContext.Current.ForumDeleteAccess;
            }
        }

        /// <summary>
        /// Gets a value indicating whether CanUnDeletePost.
        /// </summary>
        public bool CanUnDeletePost
        {
            get
            {
                return this.PostDeleted && this.CanDeletePost;
            }
        }

        /// <summary>
        /// Gets a value indicating whether CanReply.
        /// </summary>
        public bool CanReply
        {
            get
            {
                // Ederon : 9/9/2007 - moderaotrs can reply in locked posts
                return ((!this._messageFlags.IsLocked && !this._forumFlags.IsLocked && !this._topicFlags.IsLocked)
                        || YafContext.Current.ForumModeratorAccess) && YafContext.Current.ForumReplyAccess;
            }
        }
    }
}