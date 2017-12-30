/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
namespace YAF.Types.Objects
{
    #region Using

    using System;
    using System.Data;

    using YAF.Types.Flags;

    #endregion

    /// <summary>
    /// The typed message list.
    /// </summary>
    [Serializable]
    public class TypedMessageList
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedMessageList"/> class.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        public TypedMessageList([NotNull] DataRow row)
        {
            this.MessageID = row.Field<int?>("MessageID");
            this.UserID = row.Field<int?>("UserID");
            this.UserName = row.Field<string>("UserName");
            this.Message = row.Field<string>("Message");
            this.TopicID = row.Field<int?>("TopicID");
            this.ForumID = row.Field<int?>("ForumID");
            this.Topic = row.Field<string>("Topic");
            this.Priority = Convert.ToInt32(row["Priority"]);
            this.Flags = new MessageFlags(row.Field<int?>("Flags") ?? 0);
            this.TopicOwnerID = row.Field<int?>("TopicOwnerID");
            this.Edited = row.Field<DateTime?>("Edited");
            this.TopicFlags = new TopicFlags(row.Field<int?>("TopicFlags") ?? 0);
            this.ForumFlags = new ForumFlags(row.Field<int?>("ForumFlags") ?? 0);
            this.EditReason = row.Field<string>("EditReason");
            this.Position = row.Field<int?>("Position");
            this.IsModeratorChanged = row.Field<bool?>("IsModeratorChanged");
            this.DeleteReason = row.Field<string>("DeleteReason");
            this.BlogPostID = row.Field<string>("BlogPostID");
            this.PollID = row.Field<int?>("PollID");
            this.IP = row.Field<string>("IP");
            this.ExternalMessageId = row.Field<string>("ExternalMessageId");
            this.ReferenceMessageId = row.Field<string>("ReferenceMessageId");
            this.Status = row.Field<string>("Status");
            this.Styles = row.Field<string>("Styles");
            this.Description = row.Field<string>("Description");
            this.HasAttachments = row.Field<bool?>("HasAttachments");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedMessageList" /> class.
        /// </summary>
        /// <param name="messageid">The messageid.</param>
        /// <param name="userid">The userid.</param>
        /// <param name="username">The username.</param>
        /// <param name="message">The message.</param>
        /// <param name="topicid">The topicid.</param>
        /// <param name="forumid">The forumid.</param>
        /// <param name="topic">The topic.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="topicownerid">The topicownerid.</param>
        /// <param name="edited">The edited.</param>
        /// <param name="topicflags">The topicflags.</param>
        /// <param name="forumflags">The forumflags.</param>
        /// <param name="editreason">The editreason.</param>
        /// <param name="position">The position.</param>
        /// <param name="ismoderatorchanged">The ismoderatorchanged.</param>
        /// <param name="deletereason">The deletereason.</param>
        /// <param name="blogpostid">The blogpostid.</param>
        /// <param name="pollid">The pollid.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="replyTo">The reply to.</param>
        /// <param name="externalmessageid">The externalmessageid.</param>
        /// <param name="referencemessageid">The referencemessageid.</param>
        /// <param name="description">The description.</param>
        /// <param name="status">The status.</param>
        /// <param name="styles">The styles.</param>
        public TypedMessageList(
            int? messageid,
            int? userid,
            [CanBeNull] string username,
            [CanBeNull] string message,
            int? topicid,
            int? forumid,
            [CanBeNull] string topic,
            int? priority,
            [CanBeNull] MessageFlags flags,
            int? topicownerid,
            DateTime? edited,
            [CanBeNull] TopicFlags topicflags,
            [CanBeNull] ForumFlags forumflags,
            [CanBeNull] string editreason,
            int? position,
            bool? ismoderatorchanged,
            [CanBeNull] string deletereason,
            [CanBeNull] string blogpostid,
            int? pollid,
            [CanBeNull] string ip,
            int? replyTo,
            [CanBeNull] string externalmessageid,
            [CanBeNull] string referencemessageid,
            [CanBeNull] string description,
            [CanBeNull] string status,
            [CanBeNull] string styles)
        {
            this.MessageID = messageid;
            this.UserID = userid;
            this.UserName = username;
            this.Message = message;
            this.TopicID = topicid;
            this.ForumID = forumid;
            this.Topic = topic;
            this.Priority = priority;
            this.Flags = flags;
            this.TopicOwnerID = topicownerid;
            this.Edited = edited;
            this.TopicFlags = topicflags;
            this.ForumFlags = forumflags;
            this.EditReason = editreason;
            this.Position = position;
            this.IsModeratorChanged = ismoderatorchanged;
            this.DeleteReason = deletereason;
            this.BlogPostID = blogpostid;
            this.PollID = pollid;
            this.IP = ip;
            this.ReplyTo = replyTo;
            this.ExternalMessageId = externalmessageid;
            this.ReferenceMessageId = referencemessageid;
            this.Status = status;
            this.Styles = styles;
            this.Description = description;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets BlogPostID.
        /// </summary>
        public string BlogPostID { get; set; }

        /// <summary>
        ///   Gets or sets DeleteReason.
        /// </summary>
        public string DeleteReason { get; set; }

        /// <summary>
        ///   Gets or sets EditReason.
        /// </summary>
        public string EditReason { get; set; }

        /// <summary>
        ///   Gets or sets Edited.
        /// </summary>
        public DateTime? Edited { get; set; }

        /// <summary>
        /// Gets or sets ExternalMessageId.
        /// </summary>
        public string ExternalMessageId { get; set; }

        /// <summary>
        ///   Gets or sets Flags.
        /// </summary>
        public MessageFlags Flags { get; set; }

        /// <summary>
        ///   Gets or sets ForumFlags.
        /// </summary>
        public ForumFlags ForumFlags { get; set; }

        /// <summary>
        ///   Gets or sets ForumID.
        /// </summary>
        public int? ForumID { get; set; }

        /// <summary>
        /// Gets or sets the reply to.
        /// </summary>
        /// <value>
        /// The reply to.
        /// </value>
        public int? ReplyTo { get; set; }

        /// <summary>
        ///   Gets or sets IP.
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        ///   Gets or sets IsModeratorChanged.
        /// </summary>
        public bool? IsModeratorChanged { get; set; }

        /// <summary>
        ///   Gets or sets Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the has attachments.
        /// </summary>
        /// <value>
        /// The has attachments.
        /// </value>
        public bool? HasAttachments { get; set; }

        /// <summary>
        ///   Gets or sets MessageID.
        /// </summary>
        public int? MessageID { get; set; }

        /// <summary>
        ///   Gets or sets PollID.
        /// </summary>
        public int? PollID { get; set; }

        /// <summary>
        ///   Gets or sets Position.
        /// </summary>
        public int? Position { get; set; }

        /// <summary>
        ///   Gets or sets Priority.
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// Gets or sets ReferenceMessageId.
        /// </summary>
        public string ReferenceMessageId { get; set; }

        /// <summary>
        ///   Gets or sets Topic.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        ///   Gets or sets TopicFlags.
        /// </summary>
        public TopicFlags TopicFlags { get; set; }

        /// <summary>
        ///   Gets or sets TopicID.
        /// </summary>
        public int? TopicID { get; set; }

        /// <summary>
        ///   Gets or sets TopicOwnerID.
        /// </summary>
        public int? TopicOwnerID { get; set; }

        /// <summary>
        ///   Gets or sets UserID.
        /// </summary>
        public int? UserID { get; set; }

        /// <summary>
        ///   Gets or sets UserName.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Topic status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the Topic Styles.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Styles { get; set; }

        #endregion
    }
}