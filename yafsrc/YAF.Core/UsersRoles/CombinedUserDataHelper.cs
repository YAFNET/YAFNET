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

namespace YAF.Core
{
    #region Using

    using System;
    using System.Data;
    using System.Web.Security;
    using YAF.Classes;
    using YAF.Core.Extensions;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;
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
        private MembershipUser membershipUser;

        /// <summary>
        ///   The _user DB row.
        /// </summary>
        private DataRow userDbRow;

        /// <summary>
        ///   The _user id.
        /// </summary>
        private int? userId;

        /// <summary>
        ///   The _user profile.
        /// </summary>
        private YafUserProfile userProfile;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedUserDataHelper" /> class.
        /// </summary>
        /// <param name="membershipUser">The membership user.</param>
        /// <param name="userId">The user identifier.</param>
        public CombinedUserDataHelper(MembershipUser membershipUser, int userId)
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
        public CombinedUserDataHelper(MembershipUser membershipUser)
            : this(membershipUser, UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.ProviderUserKey))
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
            : this(YafContext.Current.PageUserID)
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
                var value = this.DBRow.Field<int?>("NotificationType") ?? 0;

                return (this.DBRow.Field<bool?>("AutoWatchTopics") ?? false)
                       || value.ToEnum<UserNotificationSetting>() == UserNotificationSetting.TopicsIPostToOrSubscribeTo;
            }
        }

        /// <summary>
        ///   Gets Avatar.
        /// </summary>
        public string Avatar
        {
            get { return this.DBRow.Field<string>("Avatar"); }
        }

        /// <summary>
        ///   Gets Culture.
        /// </summary>
        public string CultureUser
        {
            get { return this.DBRow.Field<string>("CultureUser"); }
        }

        /// <summary>
        ///   Gets User's Text Editor.
        /// </summary>
        public string TextEditor
        {
            get { return this.DBRow.Field<string>("TextEditor"); }
        }

        /// <summary>
        ///   Gets DBRow.
        /// </summary>
        public DataRow DBRow
        {
            get
            {
                if (this.userDbRow == null && this.userId.HasValue)
                {
                    this.userDbRow = UserMembershipHelper.GetUserRowForID(
                        this.userId.Value,
                        YafContext.Current.Get<YafBoardSettings>().AllowUserInfoCaching);
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
                    return TimeZoneInfo.FindSystemTimeZoneById(this.DBRow.Field<string>("TimeZone"))
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
        public bool DailyDigest
        {
            get
            {
                return this.DBRow.Field<bool?>("DailyDigest") ??
                       YafContext.Current.Get<YafBoardSettings>().DefaultSendDigestEmail;
            }
        }

        /// <summary>
        ///   Gets DisplayName.
        /// </summary>
        public string DisplayName
        {
            get { return this.userId.HasValue ? this.DBRow.Field<string>("DisplayName") : this.UserName; }
        }

        /// <summary>
        ///   Gets Email.
        /// </summary>
        public string Email
        {
            get
            {
                if (this.Membership == null && !this.IsGuest)
                {
                    YafContext.Current.Get<ILogger>()
                        .Log(
                            this.UserID,
                            "CombinedUserDataHelper.get_Email",
                            "ATTENTION! The user with id {0} and name {1} is very possibly is not in your Membership \r\n "
                                .FormatWith(
                                    this.UserID, this.UserName)
                            +
                            "data but it's still in you YAF user table. The situation should not normally happen. \r\n "
                            + "You should create a Membership data for the user first and " +
                            "then delete him from YAF user table or leave him.");
                }

                return this.IsGuest ? this.DBRow.Field<string>("Email") : this.Membership.Email;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether HasAvatarImage.
        /// </summary>
        public bool HasAvatarImage
        {
            get
            {
                return this.DBRow != null && (this.DBRow["HasAvatarImage"].ToType<bool?>() ?? false);
            }
        }

        /// <summary>
        ///   Gets a value indicating whether IsActiveExcluded.
        /// </summary>
        public bool IsActiveExcluded
        {
            get
            {
                return this.DBRow != null && new UserFlags(this.DBRow["Flags"]).IsActiveExcluded;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether IsGuest.
        /// </summary>
        public bool IsGuest
        {
            get { return this.DBRow != null && (this.DBRow["IsGuest"].ToType<bool?>() ?? false); }
        }

        /// <summary>
        ///   Gets Joined.
        /// </summary>
        public DateTime? Joined
        {
            get { return this.DBRow.Field<DateTime>("Joined"); }
        }

        /// <summary>
        ///   Gets LanguageFile.
        /// </summary>
        public string LanguageFile
        {
            get { return this.DBRow.Field<string>("LanguageFile"); }
        }

        /// <summary>
        ///   Gets LastVisit.
        /// </summary>
        public DateTime? LastVisit
        {
            get { return this.DBRow.Field<DateTime>("LastVisit"); }
        }

        /// <summary>
        /// Gets the last IP.
        /// </summary>
        /// <value>
        /// The last IP.
        /// </value>
        public string LastIP
        {
            get { return this.DBRow.Field<string>("IP"); }
        }

        /// <summary>
        ///   Gets Membership.
        /// </summary>
        public MembershipUser Membership
        {
            get { return this.MembershipUser; }
        }

        /// <summary>
        /// Gets NotificationSetting.
        /// </summary>
        public UserNotificationSetting NotificationSetting
        {
            get
            {
                var value = this.DBRow.Field<int?>("NotificationType") ?? 0;

                return value.ToEnum<UserNotificationSetting>();
            }
        }

        /// <summary>
        ///   Gets Number of Posts.
        /// </summary>
        public int? NumPosts
        {
            get { return this.DBRow.Field<int>("NumPosts"); }
        }

        /// <summary>
        ///   Gets a value indicating whether UseMobileTheme.
        /// </summary>
        public bool UseMobileTheme
        {
            get { return this.DBRow.Field<bool?>("OverrideDefaultThemes") ?? true; }
        }

        /// <summary>
        ///   Gets a value indicating whether PMNotification.
        /// </summary>
        public bool PMNotification
        {
            get { return this.DBRow.Field<bool?>("PMNotification") ?? true; }
        }

        /// <summary>
        ///   Gets Points.
        /// </summary>
        public int? Points
        {
            get { return this.DBRow.Field<int>("Points"); }
        }

        /// <summary>
        ///   Gets Profile.
        /// </summary>
        public IYafUserProfile Profile
        {
            get
            {
                if (this.userProfile == null && this.UserName.IsSet())
                {
                    // init the profile...
                    this.userProfile = YafUserProfile.GetProfile(this.UserName);
                }

                return this.userProfile;
            }
        }

        /// <summary>
        ///   Gets RankName.
        /// </summary>
        public string RankName
        {
            get { return this.DBRow.Field<string>("RankName"); }
        }

        /// <summary>
        ///   Gets Signature.
        /// </summary>
        public string Signature
        {
            get { return this.DBRow.Field<string>("Signature"); }
        }

        /// <summary>
        ///   Gets ThemeFile.
        /// </summary>
        public string ThemeFile
        {
            get { return this.DBRow.Field<string>("ThemeFile"); }
        }

        /// <summary>
        ///   Gets TimeZone.
        /// </summary>
        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                TimeZoneInfo timeZoneInfo;

                try
                {
                    timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(this.DBRow.Field<string>("TimeZone"));
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
        public int? TimeZone
        {
            get
            {
               return DateTimeHelper.GetTimeZoneOffset(this.TimeZoneInfo);
            }
        }

        /// <summary>
        ///   Gets UserID.
        /// </summary>
        public int UserID
        {
            get
            {
                return this.userId != null ? this.userId.ToType<int>() : 0;
            }
        }

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

                return this.userId.HasValue ? this.DBRow.Field<string>("Name") : null;
            }
        }

        /// <summary>
        ///   Gets or sets the membership user.
        /// </summary>
        protected MembershipUser MembershipUser
        {
            get
            {
                if (this.membershipUser == null && this.userId.HasValue)
                {
                    this.membershipUser = UserMembershipHelper.GetMembershipUserById(this.userId.Value);
                }

                return this.membershipUser;
            }

            set
            {
                this.membershipUser = value;
            }
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
                if (this.userId == null)
                {
                    // get the user id
                    this.userId = UserMembershipHelper.GetUserIDFromProviderUserKey(this.MembershipUser.ProviderUserKey);
                }
            }

            if (!this.userId.HasValue)
            {
                throw new Exception("Cannot locate user information.");
            }
        }

        #endregion
    }
}