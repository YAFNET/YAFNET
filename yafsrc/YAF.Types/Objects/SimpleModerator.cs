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
    using System;

    /// <summary>
    /// The moderator.
    /// </summary>
    [Serializable]
    public class SimpleModerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleModerator"/> class.
        /// </summary>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="forumName">
        /// The forum Name.
        /// </param>
        /// <param name="moderatorID">
        /// The moderator id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="avatar">
        /// The avatar.
        /// </param>
        /// <param name="avatarImage">
        /// The avatar Image.
        /// </param>
        /// <param name="displayName">
        /// The display Name.
        /// </param>
        /// <param name="style">
        /// The style.
        /// </param>
        /// <param name="isGroup">
        /// The is group.
        /// </param>
        public SimpleModerator(
            long forumID, string forumName, long moderatorID,  string name, string email, string avatar, bool avatarImage, string displayName, string style, bool isGroup)
        {
            this.ForumID = forumID;
            this.ForumName = forumName;
            this.ModeratorID = moderatorID;
            this.Name = name;
            this.Email = email;
            this.Avatar = avatar;
            this.AvatarImage = avatarImage;
            this.DisplayName = displayName;
            this.Style = style;
            this.IsGroup = isGroup;
        }

        /// <summary>
        /// Gets or sets ForumID.
        /// </summary>
        public long ForumID { get; set; }

        /// <summary>
        /// Gets or sets Forum Name.
        /// </summary>
        public string ForumName { get; set; }

        /// <summary>
        /// Gets or sets ModeratorID.
        /// </summary>
        public long ModeratorID { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets Avatar.
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [avatar image].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [avatar image]; otherwise, <c>false</c>.
        /// </value>
        public bool AvatarImage { get; set; }

        /// <summary>
        /// Gets or sets Display Name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets Style.
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsGroup.
        /// </summary>
        public bool IsGroup { get; set; }
    }
}