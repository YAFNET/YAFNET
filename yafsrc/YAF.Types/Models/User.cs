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
    using System.Data.Linq.Mapping;

    using ServiceStack.DataAnnotations;

    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;

    /// <summary>
    /// A class which represents the User table.
    /// </summary>
    [Serializable]
    [Table(Name = "User")]
    public partial class User : IEntity, IHaveBoardID, IHaveID
    {
        partial void OnCreated();

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

            this.OnCreated();
        }

        #region Properties

        [Alias("UserID")]
        [AutoIncrement]
        public int ID { get; set; }
        [References(typeof(Board))]
        [Required]
        public int BoardID { get; set; }
        public string ProviderUserKey { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public string Password { get; set; }
        public string Email { get; set; }
        [Required]
        public DateTime Joined { get; set; }
        [Required]
        public DateTime LastVisit { get; set; }
        public string IP { get; set; }
        [Required]
        public int NumPosts { get; set; }
        public string TimeZone { get; set; }
        public string Avatar { get; set; }
        public string Signature { get; set; }
        public byte[] AvatarImage { get; set; }
        public string AvatarImageType { get; set; }
        [References(typeof(Rank))]
        [Required]
        public int RankID { get; set; }
        public DateTime? Suspended { get; set; }
        public string LanguageFile { get; set; }
        public string ThemeFile { get; set; }
        [Required]
        public bool PMNotification { get; set; }
        [Required]
        public bool AutoWatchTopics { get; set; }
        [Required]
        public bool DailyDigest { get; set; }
        public int? NotificationType { get; set; }
        [Required]
        public int Flags { get; set; }
        [Required]
        public int Points { get; set; }
        [Compute]
        public bool? IsApproved { get; set; }
        [Compute]
        public bool? IsActiveExcluded { get; set; }
        public string Culture { get; set; }
        public string TextEditor { get; set; }
        [Required]
        public bool UseSingleSignOn { get; set; }
        [Compute]
        public bool? IsGuest { get; set; }
        [Compute]
        public bool? IsCaptchaExcluded { get; set; }
        [Compute]
        public bool? IsDST { get; set; }
        [Compute]
        public bool? IsDirty { get; set; }
        [Required]
        public bool IsFacebookUser { get; set; }
        [Required]
        public bool IsTwitterUser { get; set; }
        public string UserStyle { get; set; }
        [Required]
        public int StyleFlags { get; set; }
        [Compute]
        public bool? IsUserStyle { get; set; }
        [Compute]
        public bool? IsGroupStyle { get; set; }
        [Compute]
        public bool? IsRankStyle { get; set; }
        [Required]
        public bool IsPossibleSpamBot { get; set; }
        [Required]
        public bool IsGoogleUser { get; set; }
        public string SuspendedReason { get; set; }
        [Required]
        public int SuspendedBy { get; set; }

        #endregion
    }
}