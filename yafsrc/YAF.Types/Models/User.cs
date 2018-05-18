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
                this.ProviderUserKey = this.IsGuest ? null : this.ProviderUserKey;
            }
            catch (Exception)
            {
                this.IsGuest = true;
                this.ProviderUserKey = null;
            }

            this.OnCreated();
        }

        #region Properties

        [AutoIncrement]
        [Alias("UserID")]
        public int ID { get; set; }

        public int BoardID { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public DateTime Joined { get; set; }

        public DateTime LastVisit { get; set; }

        public string IP { get; set; }

        public int NumPosts { get; set; }

        public string TimeZone { get; set; }

        public string Avatar { get; set; }

        public string Signature { get; set; }

        public byte[] AvatarImage { get; set; }

        public int RankID { get; set; }

        public DateTime? Suspended { get; set; }

        public string SuspendedReason { get; set; }

        public int SuspendedBy { get; set; }

        public string LanguageFile { get; set; }

        public string ThemeFile { get; set; }

        public int Flags { get; set; }

        public bool PMNotification { get; set; }

        public int Points { get; set; }

        //public bool? IsAdmin { get; set; }

        public bool? IsApproved { get; set; }

        public bool? IsActiveExcluded { get; set; }

        public string ProviderUserKey { get; set; }

        public bool OverrideDefaultThemes { get; set; }

        public string AvatarImageType { get; set; }

        public bool AutoWatchTopics { get; set; }

        public string DisplayName { get; set; }

       // [Alias("CultureUser")]
        public string Culture { get; set; }

        public int? NotificationType { get; set; }

        public bool DailyDigest { get; set; }

        public string TextEditor { get; set; }

        public bool UseSingleSignOn { get; set; }

        public bool IsGuest { get; set; }

        public bool? IsCaptchaExcluded { get; set; }

        public bool? IsDST { get; set; }

        public bool? IsDirty { get; set; }

        public bool IsGoogleUser { get; set; }

        public bool IsFacebookUser { get; set; }

        public bool IsTwitterUser { get; set; }

        public string UserStyle { get; set; }

        public int StyleFlags { get; set; }

        public bool? IsUserStyle { get; set; }

        public bool? IsGroupStyle { get; set; }

        public bool? IsRankStyle { get; set; }

        #endregion
    }
}