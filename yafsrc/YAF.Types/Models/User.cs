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

    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// A class which represents the User table.
    /// </summary>
    [Serializable]

    [UniqueConstraint(nameof(BoardID), nameof(Name))]
    [Table(Name = "User")]
    public class User : IEntity, IHaveBoardID, IHaveID
    {
        public User()
        {
            try
            {
                this.ProviderUserKey = this.IsGuest.Value ? null : this.ProviderUserKey;
            }
            catch (Exception)
            {
                this.IsGuest = true;
                this.ProviderUserKey = null;
            }
        }

        #region Properties

        [Alias("UserID")]
        [AutoIncrement]
        public int ID { get; set; }

        [References(typeof(Board))]
        [Required]
        [Index(NonClustered = true)]
        public int BoardID { get; set; }

        [StringLength(255)]
        public string ProviderUserKey { get; set; }

        [Required]
        [Index]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [Index]
        [StringLength(255)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(32)]
        public string Password { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [Index]
        public DateTime Joined { get; set; }

        [Required]
        public DateTime LastVisit { get; set; }

        [StringLength(39)]
        public string IP { get; set; }

        [Required]
        public int NumPosts { get; set; }

        public string TimeZone { get; set; }

        [StringLength(255)]
        public string Avatar { get; set; }

        public string Signature { get; set; }

        public byte[] AvatarImage { get; set; }

        [StringLength(50)]
        public string AvatarImageType { get; set; }

        [References(typeof(Rank))]
        [Required]
        public int RankID { get; set; }

        public DateTime? Suspended { get; set; }

        [StringLength(50)]
        public string LanguageFile { get; set; }

        [StringLength(50)]
        public string ThemeFile { get; set; }

        [Required]
        [Default(1)]
        public bool PMNotification { get; set; }

        [Required]
        [Default(1)]
        public bool AutoWatchTopics { get; set; }

        [Required]
        [Default(0)]
        public bool DailyDigest { get; set; }

        [Required]
        [Default(1)]
        public bool Activity { get; set; }

        [Default(10)]
        public int? NotificationType { get; set; }

        [Required]
        [Default(0)]
        public int Flags { get; set; }

        [Ignore]
        public UserFlags UserFlags
        {
            get => new UserFlags(this.Flags);

            set => this.Flags = value.BitValue;
        }

        [Required]
        [Default(0)]
        public int BlockFlags { get; set; }

        [Ignore]
        public UserBlockFlags Block
        {
            get => new UserBlockFlags(this.BlockFlags);

            set => this.BlockFlags = value.BitValue;
        }

        [Required]
        [Default(1)]
        public int Points { get; set; }

        [Compute]
        public bool? IsApproved { get; set; }

        [Compute]
        public bool? IsActiveExcluded { get; set; }

        [StringLength(10)]
        public string Culture { get; set; }

        [Compute]
        public bool? IsGuest { get; set; }

        [Compute]
        public bool? IsCaptchaExcluded { get; set; }

        [Compute]
        public bool? IsDST { get; set; }

        [Compute]
        public bool? IsDirty { get; set; }

        [Compute]
        public bool? Moderated { get; set; }

        [Required]
        [Default(0)]
        public bool IsFacebookUser { get; set; }

        [Required]
        [Default(0)]
        public bool IsTwitterUser { get; set; }

        [Index]
        [StringLength(510)]
        public string UserStyle { get; set; }

        [Required]
        [Default(0)]
        public int StyleFlags { get; set; }

        [Compute]
        public bool? IsUserStyle { get; set; }

        [Compute]
        public bool? IsGroupStyle { get; set; }

        [Compute]
        public bool? IsRankStyle { get; set; }

        [Required]
        [Default(0)]
        public bool IsGoogleUser { get; set; }

        public string SuspendedReason { get; set; }

        [Required]
        [Default(0)]
        public int SuspendedBy { get; set; }

        #endregion
    }
}