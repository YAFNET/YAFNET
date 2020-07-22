/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Core.Helpers
{
    using System;
    using System.Data;

    using YAF.Core.Context;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Utils.Helpers;

    /// <summary>
    /// The post data helper wrapper.
    /// </summary>
    public class PostDataHelperWrapper
    {
        /// <summary>
        /// The _forum flags.
        /// </summary>
        private ForumFlags forumFlags;

        /// <summary>
        /// The _message flags.
        /// </summary>
        private MessageFlags messageFlags;

        /// <summary>
        /// The current data row for this post.
        /// </summary>
        private DataRow row;

        /// <summary>
        /// The _topic flags.
        /// </summary>
        private TopicFlags topicFlags;

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
            this.DataRow = dataRow;
        }

        /// <summary>
        /// Gets or sets DataRow.
        /// </summary>
        public DataRow DataRow
        {
            get => this.row;

            set
            {
                this.row = value;

                // get all flags for forum, topic and message
                if (this.row != null)
                {
                    this.forumFlags = new ForumFlags(this.row["ForumFlags"]);
                    this.topicFlags = new TopicFlags(this.row["TopicFlags"]);
                    this.messageFlags = new MessageFlags(this.row["Flags"]);
                }
                else
                {
                    this.forumFlags = new ForumFlags(0);
                    this.topicFlags = new TopicFlags(0);
                    this.messageFlags = new MessageFlags(0);
                }
            }
        }

        /// <summary>
        /// Gets UserId.
        /// </summary>
        public int UserId => this.DataRow?["UserID"].ToType<int>() ?? 0;

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public int MessageId => this.DataRow?["MessageID"].ToType<int>() ?? 0;

        /// <summary>
        /// Gets the topic identifier.
        /// </summary>
        /// <value>
        /// The topic identifier.
        /// </value>
        public int TopicId => this.DataRow?["TopicID"].ToType<int>() ?? 0;

        /// <summary>
        /// IsLocked flag should only be used for "ghost" posts such as the
        /// Sponsor post that isn't really there.
        /// </summary>
        public bool IsLocked => this.messageFlags != null && this.messageFlags.IsLocked;

        /// <summary>
        /// Gets a value indicating whether CanThankPost.
        /// </summary>
        public bool CanThankPost => this.DataRow["UserID"].ToType<int>() != BoardContext.Current.PageUserID;

        /// <summary>
        /// Gets a value indicating whether CanEditPost.
        /// </summary>
        public bool CanEditPost =>
            (!this.PostLocked && !this.forumFlags.IsLocked && !this.topicFlags.IsLocked
             && (this.UserId == BoardContext.Current.PageUserID && !this.DataRow["IsGuest"].ToType<bool>()
                 || this.DataRow["IsGuest"].ToType<bool>() && this.DataRow["IP"].ToString()
                 == BoardContext.Current.CurrentForumPage.Request.GetUserRealIPAddress())
             || BoardContext.Current.ForumModeratorAccess) && BoardContext.Current.ForumEditAccess;

        /// <summary>
        /// Gets a value indicating whether PostLocked.
        /// </summary>
        public bool PostLocked
        {
            get
            {
                // post is explicitly locked
                if (this.messageFlags.IsLocked)
                {
                    return true;
                }

                // there is auto-lock period defined
                if (BoardContext.Current.IsAdmin || BoardContext.Current.BoardSettings.LockPosts <= 0)
                {
                    return false;
                }

                var edited = this.DataRow["Edited"].ToType<DateTime>();

                // check if post is locked according to this rule
                return edited.AddDays(BoardContext.Current.BoardSettings.LockPosts) < DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Post is Deleted.
        /// </summary>
        public bool PostDeleted => this.messageFlags.IsDeleted;

        /// <summary>
        /// Gets a value indicating whether PostDeleted.
        /// </summary>
        public bool PostIsAnswer => this.messageFlags.IsAnswer;

        /// <summary>
        /// Gets a value indicating whether CanAttach.
        /// </summary>
        public bool CanAttach =>
            (!this.PostLocked && !this.forumFlags.IsLocked && !this.topicFlags.IsLocked
             && this.UserId == BoardContext.Current.PageUserID || BoardContext.Current.ForumModeratorAccess)
            && BoardContext.Current.ForumUploadAccess;

        /// <summary>
        /// Gets a value indicating whether CanDeletePost.
        /// </summary>
        public bool CanDeletePost =>
            (!this.PostLocked && !this.forumFlags.IsLocked && !this.topicFlags.IsLocked
             && (this.UserId == BoardContext.Current.PageUserID && !this.DataRow["IsGuest"].ToType<bool>()
                 || this.DataRow["IsGuest"].ToType<bool>() && this.DataRow["IP"].ToString()
                 == BoardContext.Current.CurrentForumPage.Request.GetUserRealIPAddress())
             || BoardContext.Current.ForumModeratorAccess) && BoardContext.Current.ForumDeleteAccess;

        /// <summary>
        /// Gets a value indicating whether CanUnDeletePost.
        /// </summary>
        public bool CanUnDeletePost => this.PostDeleted && this.CanDeletePost;

        /// <summary>
        /// Gets a value indicating whether CanReply.
        /// </summary>
        public bool CanReply =>
            (!this.messageFlags.IsLocked && !this.forumFlags.IsLocked && !this.topicFlags.IsLocked
             || BoardContext.Current.ForumModeratorAccess) && BoardContext.Current.ForumReplyAccess;
    }
}