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
namespace YAF.Types.Objects
{
    #region Using

    using System;
    using System.Data;
    using System.Globalization;

    #endregion

    /// <summary>
    /// The Search Message
    /// </summary>
    public class SearchMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchMessage"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        public SearchMessage([NotNull] DataRow row)
        {
            this.MessageId = row.Field<int?>("MessageID");
            this.Message = row.Field<string>("Message");
            this.Flags = row.Field<int>("Flags");
            this.Posted = row.Field<DateTime>("Posted").ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            this.UserName = row.Field<string>("UserName");
            this.UserDisplayName = row.Field<string>("UserDisplayName");
            this.UserStyle = row.Field<string>("UserStyle");
            this.UserId = row.Field<int?>("UserID");

            this.TopicId = row.Field<int?>("TopicID");
            this.Topic = row.Field<string>("Topic");
            this.ForumId = row.Field<int?>("ForumID");
            this.ForumName = row.Field<string>("Name");
            this.Description = row.Field<string>("Description");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchMessage"/> class.
        /// </summary>
        public SearchMessage()
        {
        }

        #region Properties

        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public int? MessageId { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        /// <value>
        /// The flags.
        /// </value>
        public int Flags { get; set; }

        /// <summary>
        /// Gets or sets the posted.
        /// </summary>
        /// <value>
        /// The posted.
        /// </value>
        public string Posted { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the display name of the user.
        /// </summary>
        /// <value>
        /// The display name of the user.
        /// </value>
        public string UserDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the user style.
        /// </summary>
        /// <value>
        /// The user style.
        /// </value>
        public string UserStyle { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int? UserId { get; set; }

        /// <summary>
        /// Gets or sets the topic identifier.
        /// </summary>
        /// <value>
        /// The topic identifier.
        /// </value>
        public int? TopicId { get; set; }

        /// <summary>
        /// Gets or sets the topic.
        /// </summary>
        /// <value>
        /// The topic.
        /// </value>
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the topic tags.
        /// </summary>
        public string TopicTags { get; set; }

        /// <summary>
        /// Gets or sets the forum identifier.
        /// </summary>
        /// <value>
        /// The forum identifier.
        /// </value>
        public int? ForumId { get; set; }

        /// <summary>
        /// Gets or sets the name of the forum.
        /// </summary>
        /// <value>
        /// The name of the forum.
        /// </value>
        public string ForumName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the topic URL.
        /// </summary>
        /// <value>
        /// The topic URL.
        /// </value>
        public string TopicUrl { get; set; }

        /// <summary>
        /// Gets or sets the message URL.
        /// </summary>
        /// <value>
        /// The message URL.
        /// </value>
        public string MessageUrl { get; set; }

        /// <summary>
        /// Gets or sets the forum URL.
        /// </summary>
        /// <value>
        /// The forum URL.
        /// </value>
        public string ForumUrl { get; set; }

        #endregion
    }
}