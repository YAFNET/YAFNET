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
namespace YAF.Types.Models
{
    using System;
    using System.Data.Linq.Mapping;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// A class which represents the UserPMessage table.
    /// </summary>
    [Serializable]
    [Table(Name = "UserPMessage")]
    [PostCreateTable("alter table [{databaseOwner}].[{tableName}] add [IsRead]	   as (CONVERT([bit],sign([Flags]&(1)),(0)))" +
                         "alter table [{databaseOwner}].[{tableName}] add [IsInOutbox] as (CONVERT([bit],sign([Flags]&(2)),(0)))" +
                         "alter table [{databaseOwner}].[{tableName}] add [IsArchived] as (CONVERT([bit],sign([Flags]&(4)),(0)))" +
                         "alter table [{databaseOwner}].[{tableName}] add [IsDeleted]  as (CONVERT([bit],sign([Flags]&(8)),(0)))")]
    public class UserPMessage : IEntity, IHaveID
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Alias("UserPMessageID")]
        [AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [References(typeof(User))]
        [Required]
        [Index]
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the p message id.
        /// </summary>
        [References(typeof(PMessage))]
        [Required]
        public int PMessageID { get; set; }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        [Required]
        [Default(0)]
        public int Flags { get; set; }

        /// <summary>
        /// Gets or sets the is read.
        /// </summary>
        [Compute]
        public bool? IsRead { get; set; }

        /// <summary>
        /// Gets or sets the is in outbox.
        /// </summary>
        [Compute]
        public bool? IsInOutbox { get; set; }

        /// <summary>
        /// Gets or sets the is archived.
        /// </summary>
        [Compute]
        public bool? IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the is deleted.
        /// </summary>
        [Compute]
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is reply.
        /// </summary>
        [Required]
        public bool IsReply { get; set; }

        #endregion
    }
}