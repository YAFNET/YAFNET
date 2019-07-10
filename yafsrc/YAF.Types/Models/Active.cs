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

    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    ///     A class which represents the Active table.
    /// </summary>
    [Serializable]
    public partial class Active : IEntity, IHaveBoardID
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Active"/> class.
        /// </summary>
        public Active()
        {
            this.OnCreated();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the board id.
        /// </summary>
        [References(typeof(Board))]
        [Required]
        public int BoardID { get; set; }

        /// <summary>
        /// Gets or sets the browser.
        /// </summary>
        [StringLength(50)]
        public string Browser { get; set; }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        public int? Flags { get; set; }

        /// <summary>
        /// Gets or sets the forum id.
        /// </summary>
        [References(typeof(Forum))]
        public int? ForumID { get; set; }

        /// <summary>
        /// Gets or sets the forum page.
        /// </summary>
        [StringLength(1024)]
        public string ForumPage { get; set; }

        /// <summary>
        /// Gets or sets the ip.
        /// </summary>
        [Required]
        [StringLength(39)]
        public string IP { get; set; }

        /// <summary>
        /// Gets or sets the last active.
        /// </summary>
        [Required]
        public DateTime LastActive { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        [StringLength(255)]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        [Required]
        public DateTime Login { get; set; }

        /// <summary>
        /// Gets or sets the platform.
        /// </summary>
        [StringLength(50)]
        public string Platform { get; set; }

        /// <summary>
        /// Gets or sets the session id.
        /// </summary>
        [Required]
        [StringLength(24)]
        public string SessionID { get; set; }

        /// <summary>
        /// Gets or sets the topic id.
        /// </summary>
        [References(typeof(Topic))]
        public int? TopicID { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [References(typeof(User))]
        [Required]
        public int UserID { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The on created.
        /// </summary>
        partial void OnCreated();

        #endregion
    }
}