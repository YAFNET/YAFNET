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
namespace YAF.Types.Interfaces
{
    using System;
    
    using YAF.Types.Constants;
    using YAF.Types.Flags;
    using YAF.Types.Models.Identity;
    using YAF.Types.Objects;

    /// <summary>
    /// The UserData Interface
    /// </summary>
    public interface IUserData
    {
        /// <summary>
        ///   Gets a value indicating whether AutoWatchTopics.
        /// </summary>
        bool AutoWatchTopics { get; }

        /// <summary>
        ///   Gets Avatar.
        /// </summary>
        string Avatar { get; }

        /// <summary>
        ///   Gets Culture.
        /// </summary>
        string CultureUser { get; }

        /// <summary>
        ///   Gets DBRow.
        /// </summary>
        TypedUserList User { get; }

        /// <summary>
        ///   Gets a value indicating whether  DST is Enabled.
        /// </summary>
        bool DSTUser { get; }

        /// <summary>
        /// Gets a value indicating whether DailyDigest.
        /// </summary>
        bool DailyDigest { get; }

        /// <summary>
        /// Gets a value indicating whether activity.
        /// </summary>
        bool Activity { get; }

        /// <summary>
        ///   Gets DisplayName.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        ///   Gets Email.
        /// </summary>
        string Email { get; }

        /// <summary>
        ///   Gets a value indicating whether HasAvatarImage.
        /// </summary>
        bool HasAvatarImage { get; }

        /// <summary>
        ///   Gets a value indicating whether IsActiveExcluded.
        /// </summary>
        bool IsActiveExcluded { get; }

        /// <summary>
        ///   Gets a value indicating whether IsGuest.
        /// </summary>
        bool IsGuest { get; }

        /// <summary>
        ///   Gets Joined.
        /// </summary>
        DateTime? Joined { get; }

        /// <summary>
        ///   Gets LanguageFile.
        /// </summary>
        string LanguageFile { get; }

        /// <summary>
        ///   Gets LastVisit.
        /// </summary>
        DateTime? LastVisit { get; }

        /// <summary>
        ///   Gets Membership.
        /// </summary>
        AspNetUsers Membership { get; }

        /// <summary>
        /// Gets NotificationSetting.
        /// </summary>
        UserNotificationSetting NotificationSetting { get; }

        /// <summary>
        ///   Gets Number of Posts.
        /// </summary>
        int? NumPosts { get; }

        /// <summary>
        ///   Gets a value indicating whether PMNotification.
        /// </summary>
        bool PMNotification { get; }

        /// <summary>
        ///   Gets Points.
        /// </summary>
        int? Points { get; }

        /// <summary>
        ///   Gets Profile.
        /// </summary>
        ProfileInfo Profile { get; }

        /// <summary>
        ///   Gets RankName.
        /// </summary>
        string RankName { get; }

        /// <summary>
        ///   Gets Signature.
        /// </summary>
        string Signature { get; }

        /// <summary>
        ///   Gets ThemeFile.
        /// </summary>
        string ThemeFile { get; }

        /// <summary>
        /// Gets the User Block Flags.
        /// </summary>
        UserBlockFlags Block { get; }

        /// <summary>
        /// Gets the time zone information.
        /// </summary>
        /// <value>
        /// The time zone information.
        /// </value>
        TimeZoneInfo TimeZoneInfo { get; }

        /// <summary>
        ///   Gets TimeZone.
        /// </summary>
        int? TimeZone { get; }

        /// <summary>
        ///   Gets UserID.
        /// </summary>
        int UserID { get; }

        /// <summary>
        ///   Gets UserName.
        /// </summary>
        string UserName { get; }
    }
}