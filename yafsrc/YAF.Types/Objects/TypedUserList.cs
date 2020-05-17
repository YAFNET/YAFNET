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

    #endregion

    /// <summary>
    /// The typed user list.
    /// </summary>
    [Serializable]
    public class TypedUserList
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedUserList"/> class.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        public TypedUserList([NotNull] DataRow row)
        {
            CodeContracts.VerifyNotNull(row, "row");

            this.UserID = row.Field<int?>("UserID");
            this.BoardID = row.Field<int?>("BoardID");
            this.Name = row.Field<string>("Name");
            this.Password = row.Field<string>("Password");
            this.Email = row.Field<string>("Email");
            this.Joined = row.Field<DateTime?>("Joined");
            this.LastVisit = row.Field<DateTime?>("LastVisit");
            this.IP = row.Field<string>("IP");
            this.NumPosts = row.Field<int?>("NumPosts");
            this.NumDays = row.Field<int>("NumDays");
            this.NumPostsForum = row.Field<int>("NumPostsForum");
            this.TimeZone = row.Field<string>("TimeZone");
            this.Avatar = row.Field<string>("Avatar");
            this.Signature = row.Field<string>("Signature");
            this.AvatarImage = row.Field<byte[]>("AvatarImage");
            this.RankID = row.Field<int?>("RankID");
            this.Suspended = row.Field<DateTime?>("Suspended");
            this.LanguageFile = row.Field<string>("LanguageFile");
            this.ThemeFile = row.Field<string>("ThemeFile");
            this.Flags = row.Field<int?>("Flags");
            this.PMNotification = row.Field<bool?>("PMNotification");
            this.Points = row.Field<int?>("Points");
            this.IsAdmin = Convert.ToBoolean(row.Field<int?>("IsAdmin"));
            this.IsApproved = row.Field<bool?>("IsApproved");
            this.IsActiveExcluded = row.Field<bool?>("IsActiveExcluded");
            this.ProviderUserKey = row.Field<string>("ProviderUserKey");
            this.AvatarImageType = row.Field<string>("AvatarImageType");
            this.AutoWatchTopics = row.Field<bool?>("AutoWatchTopics");
            this.DisplayName = row.Field<string>("DisplayName");
            this.Culture = row.Field<string>("Culture");
            this.NotificationType = row.Field<int?>("NotificationType");
            this.DailyDigest = row.Field<bool?>("DailyDigest");
            this.CultureUser = row.Field<string>("CultureUser");
            this.Style = row.Field<string>("Style");
            this.IsGuest = row.Field<bool?>("IsGuest");
            this.IsHostAdmin = row.Field<int?>("IsHostAdmin");
            this.RankName = row.Field<string>("RankName");
            this.Activity = row.Field<bool>("Activity");
            this.BlockFlags = row.Field<int?>("BlockFlags");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedUserList"/> class.
        /// </summary>
        /// <param name="userid">
        /// The userid.
        /// </param>
        /// <param name="boardid">
        /// The boardid.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="joined">
        /// The joined.
        /// </param>
        /// <param name="lastvisit">
        /// The lastvisit.
        /// </param>
        /// <param name="ip">
        /// The ip.
        /// </param>
        /// <param name="numposts">
        /// The numposts.
        /// </param>
        /// <param name="timezone">
        /// The timezone.
        /// </param>
        /// <param name="avatar">
        /// The avatar.
        /// </param>
        /// <param name="signature">
        /// The signature.
        /// </param>
        /// <param name="avatarimage">
        /// The avatarimage.
        /// </param>
        /// <param name="rankid">
        /// The rankid.
        /// </param>
        /// <param name="suspended">
        /// The suspended.
        /// </param>
        /// <param name="languagefile">
        /// The languagefile.
        /// </param>
        /// <param name="themefile">
        /// The themefile.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <param name="pmnotification">
        /// The pmnotification.
        /// </param>
        /// <param name="points">
        /// The points.
        /// </param>
        /// <param name="isadmin">
        /// The isadmin.
        /// </param>
        /// <param name="isapproved">
        /// The isapproved.
        /// </param>
        /// <param name="isactiveexcluded">
        /// The isactiveexcluded.
        /// </param>
        /// <param name="provideruserkey">
        /// The provideruserkey.
        /// </param>
        /// <param name="overridedefaultthemes">
        /// The overridedefaultthemes.
        /// </param>
        /// <param name="avatarimagetype">
        /// The avatarimagetype.
        /// </param>
        /// <param name="autowatchtopics">
        /// The autowatchtopics.
        /// </param>
        /// <param name="displayname">
        /// The displayname.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <param name="notificationtype">
        /// The notificationtype.
        /// </param>
        /// <param name="dailydigest">
        /// The dailydigest.
        /// </param>
        /// <param name="cultureuser">
        /// The cultureuser.
        /// </param>
        /// <param name="style">
        /// The style.
        /// </param>
        /// <param name="isguest">
        /// The isguest.
        /// </param>
        /// <param name="ishostadmin">
        /// The ishostadmin.
        /// </param>
        /// <param name="rankname">
        /// The rankname.
        /// </param>
        public TypedUserList(
            int? userid,
            int? boardid,
            [CanBeNull] string name,
            [CanBeNull] string password,
            [CanBeNull] string email,
            DateTime? joined,
            DateTime? lastvisit,
            [CanBeNull] string ip,
            int? numposts,
            string timezone,
            [CanBeNull] string avatar,
            [CanBeNull] string signature,
            [CanBeNull] byte[] avatarimage,
            int? rankid,
            DateTime? suspended,
            [CanBeNull] string languagefile,
            [CanBeNull] string themefile,
            int? flags,
            bool? pmnotification,
            int? points,
            bool? isadmin,
            bool? isapproved,
            bool? isactiveexcluded,
            [CanBeNull] string provideruserkey,
            [CanBeNull] string avatarimagetype,
            bool? autowatchtopics,
            [CanBeNull] string displayname,
            [CanBeNull] string culture,
            int? notificationtype,
            bool? dailydigest,
            [CanBeNull] string cultureuser,
            [CanBeNull] string style,
            bool? isguest,
            int? ishostadmin,
            [CanBeNull] string rankname)
        {
            this.UserID = userid;
            this.BoardID = boardid;
            this.Name = name;
            this.Password = password;
            this.Email = email;
            this.Joined = joined;
            this.LastVisit = lastvisit;
            this.IP = ip;
            this.NumPosts = numposts;
            this.TimeZone = timezone;
            this.Avatar = avatar;
            this.Signature = signature;
            this.AvatarImage = avatarimage;
            this.RankID = rankid;
            this.Suspended = suspended;
            this.LanguageFile = languagefile;
            this.ThemeFile = themefile;
            this.Flags = flags;
            this.PMNotification = pmnotification;
            this.Points = points;
            this.IsAdmin = isadmin;
            this.IsApproved = isapproved;
            this.IsActiveExcluded = isactiveexcluded;
            this.ProviderUserKey = provideruserkey;
            this.AvatarImageType = avatarimagetype;
            this.AutoWatchTopics = autowatchtopics;
            this.DisplayName = displayname;
            this.Culture = culture;
            this.NotificationType = notificationtype;
            this.DailyDigest = dailydigest;
            this.CultureUser = cultureuser;
            this.Style = style;
            this.IsGuest = isguest;
            this.IsHostAdmin = ishostadmin;
            this.RankName = rankname;
        }

        #endregion

        #region Properties

        public bool Activity { get; set; }

        /// <summary>
        /// Gets or sets AutoWatchTopics.
        /// </summary>
        public bool? AutoWatchTopics { get; set; }

        /// <summary>
        /// Gets or sets Avatar.
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets AvatarImage.
        /// </summary>
        public byte[] AvatarImage { get; set; }

        /// <summary>
        /// Gets or sets AvatarImageType.
        /// </summary>
        public string AvatarImageType { get; set; }

        /// <summary>
        /// Gets or sets BoardID.
        /// </summary>
        public int? BoardID { get; set; }

        /// <summary>
        /// Gets or sets Culture.
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        /// Gets or sets CultureUser.
        /// </summary>
        public string CultureUser { get; set; }

        /// <summary>
        /// Gets or sets DailyDigest.
        /// </summary>
        public bool? DailyDigest { get; set; }

        /// <summary>
        /// Gets or sets DisplayName.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets Email.
        /// </summary>
        public string Email { get; set; }



        /// <summary>
        /// Gets or sets Block Flags.
        /// </summary>
        public int? BlockFlags { get; set; }

        /// <summary>
        /// Gets or sets Flags.
        /// </summary>
        public int? Flags { get; set; }

        /// <summary>
        /// Gets or sets IP.
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Gets or sets IsActiveExcluded.
        /// </summary>
        public bool? IsActiveExcluded { get; set; }

        /// <summary>
        /// Gets or sets IsAdmin.
        /// </summary>
        public bool? IsAdmin { get; set; }

        /// <summary>
        /// Gets or sets IsAdmin1.
        /// </summary>
        public int? IsAdmin1 { get; set; }

        /// <summary>
        /// Gets or sets IsApproved.
        /// </summary>
        public bool? IsApproved { get; set; }

        /// <summary>
        /// Gets or sets IsGuest.
        /// </summary>
        public bool? IsGuest { get; set; }

        /// <summary>
        /// Gets or sets IsHostAdmin.
        /// </summary>
        public int? IsHostAdmin { get; set; }

        /// <summary>
        /// Gets or sets Joined.
        /// </summary>
        public DateTime? Joined { get; set; }

        /// <summary>
        /// Gets or sets LanguageFile.
        /// </summary>
        public string LanguageFile { get; set; }

        /// <summary>
        /// Gets or sets LastVisit.
        /// </summary>
        public DateTime? LastVisit { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets NotificationType.
        /// </summary>
        public int? NotificationType { get; set; }

        /// <summary>
        /// Gets or sets NumPosts.
        /// </summary>
        public int? NumPosts { get; set; }

        /// <summary>
        /// Gets or sets the num days.
        /// </summary>
        public int NumDays { get; set; }

        /// <summary>
        /// Gets or sets the num posts forum.
        /// </summary>
        public int NumPostsForum { get; set; }

        /// <summary>
        /// Gets or sets PMNotification.
        /// </summary>
        public bool? PMNotification { get; set; }

        /// <summary>
        /// Gets or sets Password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets Points.
        /// </summary>
        public int? Points { get; set; }

        /// <summary>
        /// Gets or sets ProviderUserKey.
        /// </summary>
        public string ProviderUserKey { get; set; }

        /// <summary>
        /// Gets or sets RankID.
        /// </summary>
        public int? RankID { get; set; }

        /// <summary>
        /// Gets or sets RankName.
        /// </summary>
        public string RankName { get; set; }

        /// <summary>
        /// Gets or sets Signature.
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Gets or sets Style.
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Gets or sets Suspended.
        /// </summary>
        public DateTime? Suspended { get; set; }

        /// <summary>
        /// Gets or sets ThemeFile.
        /// </summary>
        public string ThemeFile { get; set; }

        /// <summary>
        /// Gets or sets TimeZone.
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets UserID.
        /// </summary>
        public int? UserID { get; set; }

        #endregion
    }
}