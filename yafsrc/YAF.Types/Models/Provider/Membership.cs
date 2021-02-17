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
namespace YAF.Types.Models.Provider
{
    #region Using

    using System;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces.Data;

    #endregion
    [Alias("prov_Membership")]
    [Serializable]
    public partial class Membership : IEntity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Membership"/> class.
        /// </summary>
        public Membership()
        {
            this.OnCreated();
        }

        #endregion

        #region Public Properties

        [Alias("UserID")]
        [Required]
        [Index(Clustered = true)]

        [StringLength(64)]
        public string Id { get; set; }

        [Required]
        [Index]
        public Guid ApplicationID { get; set; }

        [Required]
        [Index]
        [StringLength(256)]
        public string Username { get; set; }

        [Required]
        [StringLength(256)]
        public string UsernameLwd { get; set; }

        [StringLength(256)]
        public string Password { get; set; }

        [StringLength(256)]
        public string PasswordSalt { get; set; }

        [StringLength(256)]
        public string PasswordFormat { get; set; }

        [Index]
        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(256)]
        public string EmailLwd { get; set; }

        [StringLength(256)]
        public string PasswordQuestion { get; set; }

        [StringLength(256)]
        public string PasswordAnswer { get; set; }

        public bool? IsApproved { get; set; }

        public bool? IsLockedOut { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime? LastActivity { get; set; }

        public DateTime? LastPasswordChange { get; set; }

        public DateTime? LastLockOut { get; set; }

        public int? FailedPasswordAttempts { get; set; }

        public int? FailedAnswerAttempts { get; set; }

        public DateTime? FailedPasswordWindow { get; set; }

        public DateTime? FailedAnswerWindow { get; set; }

        public DateTime? Joined { get; set; }

        public string Comment { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The on created.
        /// </summary>
        partial void OnCreated();

        #endregion
    }
}