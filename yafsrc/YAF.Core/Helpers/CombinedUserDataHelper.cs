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

namespace YAF.Core.Helpers
{
    #region Using

    using System;
    using System.Linq;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
    using YAF.Types.Models.Identity;
    using YAF.Types.Objects;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Helps get a complete user profile from various locations
    /// </summary>
    public class CombinedUserDataHelper : IUserData
    {
        #region Constants and Fields

        /// <summary>
        ///   The _membership user.
        /// </summary>
        private AspNetUsers membershipUser;

        /// <summary>
        ///   The user.
        /// </summary>
        private TypedUserList userDbRow;

        /// <summary>
        ///   The _user id.
        /// </summary>
        private int? userId;

        /// <summary>
        ///   The _user profile.
        /// </summary>
        private ProfileInfo userProfile;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedUserDataHelper" /> class.
        /// </summary>
        /// <param name="membershipUser">The membership user.</param>
        /// <param name="userId">The user identifier.</param>
        public CombinedUserDataHelper(AspNetUsers membershipUser, int userId)
        {
            this.userId = userId;
            this.MembershipUser = membershipUser;
            this.InitUserData();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedUserDataHelper"/> class.
        /// </summary>
        /// <param name="membershipUser">
        /// The membership user.
        /// </param>
        public CombinedUserDataHelper(AspNetUsers membershipUser)
            : this(membershipUser, BoardContext.Current.Get<IAspNetUsersHelper>().GetUserIDFromProviderUserKey(membershipUser.Id))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedUserDataHelper"/> class.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        public CombinedUserDataHelper(int userId)
        {
            this.userId = userId;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "CombinedUserDataHelper" /> class.
        /// </summary>
        public CombinedUserDataHelper()
            : this(BoardContext.Current.PageUserID)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether AutoWatchTopics.
        /// </summary>
        public bool AutoWatchTopics
        {
            get
            {
                var value = this.User.NotificationType ?? 0;

                return(this.User.AutoWatchTopics ?? false)
                    || value.ToEnum<UserNotificationSetting>() == UserNotificationSetting.TopicsIPostToOrSubscribeTo;
            }
        }

        /// <summary>
        ///   Gets Avatar.
        /// </summary>
        public string Avatar => this.User.Avatar;

        /// <summary>
        ///   Gets Culture.
        /// </summary>
        public string CultureUser => this.User.Culture;

        /// <summary>
        ///   Gets DBRow.
        /// </summary>
        public TypedUserList User
        {
            get
            {
                if (this.userDbRow == null && this.userId.HasValue)
                {
                    this.userDbRow = BoardContext.Current.GetRepository<User>().UserList(
                        BoardContext.Current.PageBoardID,
                        this.userId.Value,
                        true,
                        null,
                        null,
                        null).FirstOrDefault();
                }

                return this.userDbRow;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether  DST is Enabled.
        /// </summary>
        public bool DSTUser
        {
            get
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(this.User.TimeZone)
                        .SupportsDaylightSavingTime;
                }
                catch (Exception)
                {
                    return TimeZoneInfo.Local.SupportsDaylightSavingTime;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether DailyDigest.
        /// </summary>
        public bool DailyDigest => this.User.DailyDigest ??
        BoardContext.Current.Get<BoardSettings>().DefaultSendDigestEmail;

        /// <summary>
        /// Enable Activity Stream (If checked you get Notifications for Mentions, Quotes and Thanks.
        /// </summary>
        public bool Activity => this.User.Activity;

        /// <summary>
        ///   Gets DisplayName.
        /// </summary>
        public string DisplayName => this.userId.HasValue ? this.User.DisplayName : this.UserName;

        /// <summary>
        ///   Gets Email.
        /// </summary>
        public string Email
        {
            get
            {
                if (this.Membership == null && !this.IsGuest)
                {
                    BoardContext.Current.Get<ILogger>()
                        .Log(
                            this.UserID,
                            "CombinedUserDataHelper.get_Email",
                            $"ATTENTION! The user with id {this.UserID} and name {this.UserName} is very possibly is not in your Membership \r\n data but it's still in you YAF user table. The situation should not normally happen. \r\n You should create a Membership data for the user first and then delete him from YAF user table or leave him.");
                }

                return this.IsGuest ? this.User.Email : this.Membership.Email;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether HasAvatarImage.
        /// </summary>
        public bool HasAvatarImage => this.User?.AvatarImage != null;

        /// <summary>
        ///   Gets a value indicating whether IsActiveExcluded.
        /// </summary>
        public bool IsActiveExcluded => this.User != null && new UserFlags(this.User.Flags).IsActiveExcluded;

        /// <summary>
        ///   Gets a value indicating whether IsGuest.
        /// </summary>
        public bool IsGuest => this.User != null && (this.User.IsGuest ?? false);

        /// <summary>
        ///   Gets Joined.
        /// </summary>
        public DateTime? Joined => this.User.Joined;

        /// <summary>
        ///   Gets LanguageFile.
        /// </summary>
        public string LanguageFile => this.User.LanguageFile;

        /// <summary>
        ///   Gets LastVisit.
        /// </summary>
        public DateTime? LastVisit => this.User.LastVisit;

        /// <summary>
        /// Gets the last IP.
        /// </summary>
        /// <value>
        /// The last IP.
        /// </value>
        public string LastIP => this.User.IP;

        /// <summary>
        ///   Gets Membership.
        /// </summary>
        public AspNetUsers Membership => this.MembershipUser;

        /// <summary>
        /// Gets NotificationSetting.
        /// </summary>
        public UserNotificationSetting NotificationSetting
        {
            get
            {
                var value = this.User.NotificationType ?? 0;

                return value.ToEnum<UserNotificationSetting>();
            }
        }

        /// <summary>
        ///   Gets Number of Posts.
        /// </summary>
        public int? NumPosts => this.User.NumPosts;

        /// <summary>
        ///   Gets a value indicating whether PMNotification.
        /// </summary>
        public bool PMNotification => this.User.PMNotification ?? true;

        /// <summary>
        ///   Gets Points.
        /// </summary>
        public int? Points => this.User.Points;

        /// <summary>
        ///   Gets Profile.
        /// </summary>
        public ProfileInfo Profile
        {
            get
            {
                if (this.userProfile != null || this.UserName.IsNotSet())
                {
                    return this.userProfile;
                }

                var profileInfo = new ProfileInfo
                {
                    Birthday = this.Membership.Profile_Birthday,
                    Blog = this.Membership.Profile_Blog,
                    Gender = this.Membership.Profile_Gender,
                    GoogleId = this.Membership.Profile_GoogleId,
                    Homepage = this.Membership.Profile_Homepage,
                    ICQ = this.Membership.Profile_ICQ,
                    Facebook = this.Membership.Profile_Facebook,
                    FacebookId = this.Membership.Profile_FacebookId,
                    Twitter = this.Membership.Profile_Twitter,
                    TwitterId = this.Membership.Profile_TwitterId,
                    Interests = this.Membership.Profile_Interests,
                    Location = this.Membership.Profile_Location,
                    Country = this.Membership.Profile_Country,
                    Region = this.Membership.Profile_Region,
                    City = this.Membership.Profile_City,
                    Occupation = this.Membership.Profile_Occupation,
                    RealName = this.Membership.Profile_RealName,
                    Skype = this.Membership.Profile_Skype,
                    XMPP = this.Membership.Profile_XMPP,
                    LastSyncedWithDNN = this.Membership.Profile_LastSyncedWithDNN
                };

                // init the profile...
                this.userProfile = profileInfo;

                return this.userProfile;
            }
        }

        /// <summary>
        ///   Gets RankName.
        /// </summary>
        public string RankName => this.User.RankName;

        /// <summary>
        ///   Gets Signature.
        /// </summary>
        public string Signature => this.User.Signature;

        /// <summary>
        ///   Gets ThemeFile.
        /// </summary>
        public string ThemeFile => this.User.ThemeFile;

        /// <summary>
        /// The block.
        /// </summary>
        public UserBlockFlags Block => new UserBlockFlags(this.User.BlockFlags);

        /// <summary>
        ///   Gets TimeZone.
        /// </summary>
        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                TimeZoneInfo timeZoneInfo;

                var tz = this.User.TimeZone;
                if (System.Text.RegularExpressions.Regex.IsMatch(tz, @"^[\-?\+?\d]*$"))
                {
                    return TimeZoneInfo.Local;
                }

                try
                {
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(tz);
                }
                catch (Exception)
                {
                    timeZoneInfo = TimeZoneInfo.Local;
                }

                return timeZoneInfo;
            }
        }

        /// <summary>
        ///   Gets TimeZone.
        /// </summary>
        public int? TimeZone => DateTimeHelper.GetTimeZoneOffset(this.TimeZoneInfo);

        /// <summary>
        ///   Gets UserID.
        /// </summary>
        public int UserID => this.userId?.ToType<int>() ?? 0;

        /// <summary>
        ///   Gets UserName.
        /// </summary>
        public string UserName
        {
            get
            {
                if (this.MembershipUser != null)
                {
                    return this.MembershipUser.UserName;
                }

                return this.userId.HasValue ? this.User.Name : null;
            }
        }

        protected AspNetUsers MembershipUser
        {
            get
            {
                if (this.membershipUser == null && this.userId.HasValue)
                {
                    this.membershipUser = BoardContext.Current.Get<IAspNetUsersHelper>().GetMembershipUserById(this.userId.Value);
                }

                return this.membershipUser;
            }

            set => this.membershipUser = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the user data.
        /// </summary>
        /// <exception cref="System.Exception">Cannot locate user information.</exception>
        /// <exception cref="Exception">Cannot locate user information.</exception>
        private void InitUserData()
        {
            if (this.MembershipUser != null && !this.userId.HasValue)
            {
                this.userId ??= BoardContext.Current.Get<IAspNetUsersHelper>().GetUserIDFromProviderUserKey(this.MembershipUser.Id);
            }

            if (!this.userId.HasValue)
            {
                throw new Exception("Cannot locate user information.");
            }
        }

        #endregion
    }
}