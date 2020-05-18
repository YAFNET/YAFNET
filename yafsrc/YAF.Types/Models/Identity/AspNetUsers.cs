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

namespace YAF.Types.Models.Identity
{
    #region Using

    using System;

    using Microsoft.AspNet.Identity;

    using ServiceStack.DataAnnotations;
    using ServiceStack.Model;

    using YAF.Types.Interfaces.Data;

    #endregion

    /// <summary>
    /// The asp net users.
    /// </summary>
    public class AspNetUsers : AspNetUsers<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetUsers"/> class.
        /// </summary>
        public AspNetUsers()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.IsApproved = false;
            this.LastLoginDate = DateTime.Now;
            this.LastActivityDate = DateTime.Now;
            this.LastPasswordChangedDate = DateTime.Now;
            this.LastLockoutDate = DateTime.MinValue.AddYears(1902);
            this.FailedPasswordAnswerAttemptWindowStart = DateTime.MinValue.AddYears(1902);
            this.FailedPasswordAttemptWindowStart = DateTime.MinValue.AddYears(1902);
            this.Profile_Birthday = DateTime.MinValue.AddYears(1902);

            /*this.Profile = new ProfileInfo
            {
                Birthday = this.Profile_Birthday,
                Blog = this.Profile_Blog,
                Gender = this.Profile_Gender,
                GoogleId = this.Profile_GoogleId,
                Homepage = this.Profile_Homepage,
                ICQ = this.Profile_ICQ,
                Facebook = this.Profile_Facebook,
                FacebookId = this.Profile_FacebookId,
                Twitter = this.Profile_Twitter,
                TwitterId = this.Profile_TwitterId,
                Interests = this.Profile_Interests,
                Location = this.Profile_Location,
                Country = this.Profile_Country,
                Region = this.Profile_Region,
                City = this.Profile_City,
                Occupation = this.Profile_Occupation,
                RealName = this.Profile_RealName,
                Skype = this.Profile_Skype,
                XMPP = this.Profile_XMPP,
                LastSyncedWithDNN = this.Profile_LastSyncedWithDNN
            };*/
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetUsers"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public AspNetUsers(string name)
            : base(name)
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.IsApproved = false;
            this.LastLoginDate = DateTime.Now;
            this.LastActivityDate = DateTime.Now;
            this.LastPasswordChangedDate = DateTime.Now;
            this.LastLockoutDate = DateTime.MinValue.AddYears(1902);
            this.FailedPasswordAnswerAttemptWindowStart = DateTime.MinValue.AddYears(1902);
            this.FailedPasswordAttemptWindowStart = DateTime.MinValue.AddYears(1902);
            this.Profile_Birthday = DateTime.MinValue.AddYears(1902);

            /*this.Profile = new ProfileInfo
            {
                Birthday = this.Profile_Birthday,
                Blog = this.Profile_Blog,
                Gender = this.Profile_Gender,
                GoogleId = this.Profile_GoogleId,
                Homepage = this.Profile_Homepage,
                ICQ = this.Profile_ICQ,
                Facebook = this.Profile_Facebook,
                FacebookId = this.Profile_FacebookId,
                Twitter = this.Profile_Twitter,
                TwitterId = this.Profile_TwitterId,
                Interests = this.Profile_Interests,
                Location = this.Profile_Location,
                Country = this.Profile_Country,
                Region = this.Profile_Region,
                City = this.Profile_City,
                Occupation = this.Profile_Occupation,
                RealName = this.Profile_RealName,
                Skype = this.Profile_Skype,
                XMPP = this.Profile_XMPP,
                LastSyncedWithDNN = this.Profile_LastSyncedWithDNN
            };*/
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetUsers"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public AspNetUsers(string id, string name)
            : base(id, name)
        {
        }
    }

    /// <summary>
    /// The asp net users.
    /// </summary>
    /// <typeparam name="TKey">
    /// </typeparam>
    public class AspNetUsers<TKey> : IUser<TKey>, IEntity, IHasId<TKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetUsers{TKey}"/> class. 
        /// Default constructor 
        /// </summary>
        public AspNetUsers()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetUsers{TKey}"/> class. 
        /// Constructor that takes user name as argument
        /// </summary>
        /// <param name="userName">
        /// </param>
        public AspNetUsers(string userName)
            : this()
        {
            this.UserName = userName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetUsers{TKey}"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        public AspNetUsers(TKey id, string userName)
            : this(userName)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets or sets the User ID
        /// </summary>
        [StringLength(56)]
        [Required]
        public TKey Id { get; set; }

        /// <summary>
        /// Gets or sets the User's name
        /// </summary>
        [StringLength(50)]
        [Index(Unique = true)]
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the Email
        /// </summary>
        [StringLength(100)]
        [Required]
        public virtual string Email { get; set; }

        /// <summary>
        ///   Gets or sets the True if the email is confirmed, default is false
        /// </summary>
        public virtual bool EmailConfirmed { get; set; }

        /// <summary>
        ///    Gets or sets the  The salted/hashed form of the user password
        /// </summary>
        [StringLength(100)]
        public virtual string PasswordHash { get; set; }

        /// <summary>
        ///   Gets or sets the   A random value that should change whenever a users credentials have changed (password changed, login removed)
        /// </summary>
        [StringLength(100)]
        public virtual string SecurityStamp { get; set; }

        /// <summary>
        ///   Gets or sets the   PhoneNumber for the user
        /// </summary>
        [StringLength(40)]
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        ///   Gets or sets the   True if the phone number is confirmed, default is false
        /// </summary>
        public virtual bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        ///   Gets or sets the   Is two factor enabled for the user
        /// </summary>
        public virtual bool TwoFactorEnabled { get; set; }

        /// <summary>
        ///   Gets or sets the   DateTime in UTC when lockout ends, any time in the past is considered not locked out.
        /// </summary>
        public virtual DateTime? LockoutEndDateUtc { get; set; }

        /// <summary>
        ///   Gets or sets the   Is lockout enabled for this user
        /// </summary>
        public virtual bool LockoutEnabled { get; set; }

        /// <summary>
        ///  Gets or sets the    Used to record failures for the purposes of lockout
        /// </summary>
        public virtual int AccessFailedCount { get; set; }

        public Guid ApplicationId { get; set; }

        public string MobileAlias { get; set; }

        public bool IsAnonymous { get; set; }

        public DateTime? LastActivityDate { get; set; }

        public string MobilePIN { get; set; }

        public string LoweredEmail { get; set; }

        public string LoweredUserName { get; set; }

        public bool IsApproved { get; set; }

        public bool IsLockedOut { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LastPasswordChangedDate { get; set; }

        public DateTime? LastLockoutDate { get; set; }

        public int FailedPasswordAttemptCount { get; set; }

        public DateTime? FailedPasswordAttemptWindowStart { get; set; }

        public int FailedPasswordAnswerAttemptCount { get; set; }

        public DateTime? FailedPasswordAnswerAttemptWindowStart { get; set; }

        public string Comment { get; set; }

        //        [Ignore]
        //public virtual ProfileInfo Profile { get; set; }

        /// <summary>
        /// Gets or sets Birthday.
        /// </summary>
        public DateTime Profile_Birthday { get; set; }

        /// <summary>
        /// Gets or sets Blog.
        /// </summary>
        public string Profile_Blog { get; set; }

        /// <summary>
        /// Gets or sets Gender.
        /// </summary>
        public int Profile_Gender { get; set; }

        /// <summary>
        /// Gets or sets Google Id
        /// </summary>
        public string Profile_GoogleId { get; set; }

        /// <summary>
        /// Gets or sets Homepage.
        /// </summary>
        public string Profile_Homepage { get; set; }

        /// <summary>
        /// Gets or sets ICQ.
        /// </summary>
        public string Profile_ICQ { get; set; }

        /// <summary>
        /// Gets or sets Facebook.
        /// </summary>
        public string Profile_Facebook { get; set; }

        /// <summary>
        /// Gets or sets Facebook.
        /// </summary>
        public string Profile_FacebookId { get; set; }

        /// <summary>
        /// Gets or sets Twitter.
        /// </summary>
        public string Profile_Twitter { get; set; }

        /// <summary>
        /// Gets or sets Twitter.
        /// </summary>
        public string Profile_TwitterId { get; set; }

        /// <summary>
        /// Gets or sets Interests.
        /// </summary>
        public string Profile_Interests { get; set; }

        /// <summary>
        /// Gets or sets Location.
        /// </summary>
        public string Profile_Location { get; set; }

        /// <summary>
        /// Gets or sets Country.
        /// </summary>
        public string Profile_Country { get; set; }

        /// <summary>
        /// Gets or sets Region or State(US).
        /// </summary>
        public string Profile_Region { get; set; }

        /// <summary>
        /// Gets or sets a City.
        /// </summary>
        public string Profile_City { get; set; }

        /// <summary>
        /// Gets or sets Occupation.
        /// </summary>
        public string Profile_Occupation { get; set; }

        /// <summary>
        /// Gets or sets RealName.
        /// </summary>
        public string Profile_RealName { get; set; }

        /// <summary>
        /// Gets or sets Skype.
        /// </summary>
        public string Profile_Skype { get; set; }

        /// <summary>
        /// Gets or sets XMPP.
        /// </summary>
        public string Profile_XMPP { get; set; }

        /// <summary>
        /// Gets or sets Last Synced With DNN.
        /// </summary>
        public DateTime? Profile_LastSyncedWithDNN { get; set; }
    }
}