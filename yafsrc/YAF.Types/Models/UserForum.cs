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

    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// A class which represents the WatchForum table.
    /// </summary>
    [Serializable]
    [PostCreateTable("alter table [{databaseOwner}].[{tableName}] drop constraint [PK_{tableName}]" +
                     "alter table [{databaseOwner}].[{tableName}] with nocheck add constraint [PK_{tableName}] primary key clustered (UserID,ForumID)")]
    public class UserForum : IEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [References(typeof(User))]
        [Required]
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the forum id.
        /// </summary>
        [References(typeof(Forum))]
        [Required]
        public int ForumID { get; set; }

        /// <summary>
        /// Gets or sets the access mask id.
        /// </summary>
        [References(typeof(AccessMask))]
        [Required]
        public int AccessMaskID { get; set; }

        /// <summary>
        /// Gets or sets the invited.
        /// </summary>
        [Required]
        public DateTime Invited { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether accepted.
        /// </summary>
        [Required]
        public bool Accepted { get; set; }

        #endregion
    }
}