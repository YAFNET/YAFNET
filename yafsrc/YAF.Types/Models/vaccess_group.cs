/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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
namespace YAF.Types.Models
{
    using System;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    ///     A class which represents the yaf_vaccess_group views.
    /// </summary>
    [Serializable]
    public class vaccess_group : IEntity, IHaveBoardID
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the board id.
        /// </summary>
        [References(typeof(Board))]
        [Required]
        public int BoardID { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [Required]
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the forum id.
        /// </summary>
        [Required]
        public int ForumID { get; set; }

        /// <summary>
        /// Gets or sets the access mask id.
        /// </summary>
        [Required]
        public int AccessMaskID { get; set; }

        /// <summary>
        /// Gets or sets the group id.
        /// </summary>
        [Required]
        public int GroupID { get; set; }

        /// <summary>
        /// Gets or sets the read access.
        /// </summary>
        public int? ReadAccess { get; set; }

        /// <summary>
        /// Gets or sets the post access.
        /// </summary>
        public int? PostAccess { get; set; }

        /// <summary>
        /// Gets or sets the reply access.
        /// </summary>
        public int? ReplyAccess { get; set; }

        /// <summary>
        /// Gets or sets the priority access.
        /// </summary>
        public int? PriorityAccess { get; set; }

        /// <summary>
        /// Gets or sets the poll access.
        /// </summary>
        public int? PollAccess { get; set; }

        /// <summary>
        /// Gets or sets the vote access.
        /// </summary>
        public int? VoteAccess { get; set; }

        /// <summary>
        /// Gets or sets the moderator access.
        /// </summary>
        public int? ModeratorAccess { get; set; }

        /// <summary>
        /// Gets or sets the edit access.
        /// </summary>
        public int? EditAccess { get; set; }

        /// <summary>
        /// Gets or sets the delete access.
        /// </summary>
        public int? DeleteAccess { get; set; }

        /// <summary>
        /// Gets or sets the upload access.
        /// </summary>
        public int? UploadAccess { get; set; }

        /// <summary>
        /// Gets or sets the download access.
        /// </summary>
        public int? DownloadAccess { get; set; }

        /// <summary>
        /// Gets or sets the admin group.
        /// </summary>
        public int? AdminGroup { get; set; }

        #endregion
    }
}