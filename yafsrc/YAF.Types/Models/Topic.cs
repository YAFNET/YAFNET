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

    using ServiceStack.DataAnnotations;

    using YAF.Types.Flags;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// A class which represents the Topic table.
    /// </summary>
    [Serializable]
    public class Topic : IEntity, IHaveID
    {
        #region Properties

        [AutoIncrement]
        [Alias("TopicID")]
        public int ID { get; set; }

        [References(typeof(Forum))]
        [Required]
        public int ForumID { get; set; }

        [References(typeof(User))]
        [Required]
        [Index]

        public int UserID { get; set; }

        [StringLength(255)]
        public string UserName { get; set; }

        [Required]
        public DateTime Posted { get; set; }

        [Required]
        [Alias("Topic")]
        [StringLength(100)]
        public string TopicName { get; set; }

        [Required]
        public int Views { get; set; }

        [Required]
        public short Priority { get; set; }

        [References(typeof(PollGroupCluster))]
        public int? PollID { get; set; }

        [References(typeof(Topic))]
        public int? TopicMovedID { get; set; }

        [Index]
        public DateTime? LastPosted { get; set; }

        // [References(typeof(Message))]
        public int? LastMessageID { get; set; }

        [References(typeof(User))]
        public int? LastUserID { get; set; }

        [StringLength(255)]
        public string LastUserName { get; set; }

        [Required]
        public int NumPosts { get; set; }



        [Ignore]
        public TopicFlags TopicFlags
        {
            get => new TopicFlags(this.Flags);

            set => this.Flags = value.BitValue;
        }

        [Required]
        [Index]

        public int Flags { get; set; }

        [Compute]
        public bool? IsDeleted { get; set; }

        [Compute]
        public bool? IsQuestion { get; set; }

        public int? AnswerMessageId { get; set; }

        public int? LastMessageFlags { get; set; }

        [StringLength(255)]
        public string TopicImage { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [StringLength(255)]
        public string Status { get; set; }

        [StringLength(255)]
        public string Styles { get; set; }

        public DateTime? LinkDate { get; set; }

        [StringLength(255)]
        public string UserDisplayName { get; set; }

        [StringLength(255)]
        public string LastUserDisplayName { get; set; }


        #endregion
    }
}