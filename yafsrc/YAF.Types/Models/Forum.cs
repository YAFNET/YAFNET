/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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

namespace YAF.Types.Models
{
    using System;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Flags;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// A class which represents the yaf_Forum table.
    /// </summary>
    [Serializable]
    [UniqueConstraint(nameof(CategoryID), nameof(Name))]
    public partial class Forum : IEntity, IHaveID
    {
        partial void OnCreated();

        public Forum()
        {
            this.OnCreated();
        }

        #region Properties

        [AutoIncrement]
        [Alias("ForumID")]
        public int ID { get; set; }

        [References(typeof(Category))]
        [Required]
        [Index]
        public int CategoryID { get; set; }

        [References(typeof(Forum))]
        [Index]
        public int? ParentID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int SortOrder { get; set; }

        public DateTime? LastPosted { get; set; }

       // [References(typeof(Topic))]
        public int? LastTopicID { get; set; }

//        [References(typeof(Message))]
        public int? LastMessageID { get; set; }

        [References(typeof(User))]
        public int? LastUserID { get; set; }

        public string LastUserName { get; set; }

        public string LastUserDisplayName { get; set; }

        [Required]
        public int NumTopics { get; set; }

        [Required]
        public int NumPosts { get; set; }

        public string RemoteURL { get; set; }

        [Compute]
        public bool? IsHidden { get; set; }

        [Required]
        [Index]
        [Default(0)]
        public int Flags { get; set; }

        [Ignore]
        public ForumFlags ForumFlags
        {
            get
            {
                return new ForumFlags(this.Flags);
            }

            set
            {
                this.Flags = value.BitValue;
            }
        }

        public string ThemeURL { get; set; }

        [References(typeof(PollGroupCluster))]
        public int? PollGroupID { get; set; }

        public string ImageURL { get; set; }

        public string Styles { get; set; }

        [Compute]
        public bool? IsLocked { get; set; }
        [Compute]
        public bool? IsNoCount { get; set; }
        [Compute]
        public bool? IsModerated { get; set; }

        public int? UserID { get; set; }

        public int? ModeratedPostCount { get; set; }

        [Default(0)]
        public bool IsModeratedNewTopicOnly { get; set; }


        #endregion
    }
}