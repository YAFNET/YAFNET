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
    public partial class Topic : IEntity, IHaveID
    {
        partial void OnCreated();

        public Topic()
        {
            this.OnCreated();
        }

        #region Properties

        [AutoIncrement]
        [Alias("TopicID")]
        public int ID { get; set; }

        public int ForumID { get; set; }

        public int UserID { get; set; }

        public string UserName { get; set; }

        public string UserDisplayName { get; set; }

        public DateTime Posted { get; set; }

        [Alias("Topic")]
        public string TopicName { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public string Styles { get; set; }

        public DateTime? LinkDate { get; set; }

        public int Views { get; set; }

        public int Priority { get; set; }

        public int? PollID { get; set; }

        public int? TopicMovedID { get; set; }

        public DateTime? LastPosted { get; set; }

        public int? LastMessageID { get; set; }

        public int? LastUserID { get; set; }

        public string LastUserName { get; set; }

        public string LastUserDisplayName { get; set; }

        public int NumPosts { get; set; }

        public int Flags { get; set; }

        [IgnoreAttribute]
        public TopicFlags TopicFlags { get { return new TopicFlags(this.Flags); } }

        public int? AnswerMessageId { get; set; }

        public int? LastMessageFlags { get; set; }

        public string TopicImage { get; set; }


        #endregion
    }
}