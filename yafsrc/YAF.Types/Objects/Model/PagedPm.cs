/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Types.Objects.Model
{
    using System;

    /// <summary>
    /// The paged private Message.
    /// </summary>
    [Serializable]
    public class PagedPm
    {
        /// <summary>
        /// Gets or sets the reply to.
        /// </summary>
        public int? ReplyTo { get; set; }

        /// <summary>
        /// Gets or sets the p message id.
        /// </summary>
        public int PMessageID { get; set; }

        /// <summary>
        /// Gets or sets the user p message id.
        /// </summary>
        public int UserPMessageID { get; set; }

        /// <summary>
        /// Gets or sets the from user id.
        /// </summary>
        public int FromUserID { get; set; }

        /// <summary>
        /// Gets or sets the from user.
        /// </summary>
        public string FromUser { get; set; }

        /// <summary>
        /// Gets or sets the from user display name.
        /// </summary>
        public string FromUserDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the from style.
        /// </summary>
        public string FromStyle { get; set; }

        /// <summary>
        /// Gets or sets the from suspended.
        /// </summary>
        public DateTime? FromSuspended { get; set; }

        public string FromAvatar { get; set; }

        public bool FromHasAvatarImage { get; set; }

        /// <summary>
        /// Gets or sets the to user id.
        /// </summary>
        public int ToUserID { get; set; }

        /// <summary>
        /// Gets or sets the to user.
        /// </summary>
        public string ToUser { get; set; }

        /// <summary>
        /// Gets or sets the to user display name.
        /// </summary>
        public string ToUserDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the to style.
        /// </summary>
        public string ToStyle { get; set; }

        /// <summary>
        /// Gets or sets the to suspended.
        /// </summary>
        public DateTime? ToSuspended { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// Gets or sets the user pm flags.
        /// </summary>
        public int UserPMFlags { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is read.
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is reply.
        /// </summary>
        public bool IsReply { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is in outbox.
        /// </summary>
        public bool IsInOutbox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
