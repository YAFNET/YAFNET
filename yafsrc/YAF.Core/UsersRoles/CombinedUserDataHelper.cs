/* Yet Another Forum.net
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Core
{
    #region Using

    using System;
    using System.Data;
    using System.Web.Security;
    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core.Extensions;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;

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
        private MembershipUser _membershipUser;

        /// <summary>
        ///   The _user db row.
        /// </summary>
        private DataRow _userDBRow;

        /// <summary>
        ///   The _user id.
        /// </summary>
        private int? _userId;

        /// <summary>
        ///   The _user profile.
        /// </summary>
        private YafUserProfile _userProfile;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedUserDataHelper"/> class.
        /// </summary>
        /// <param name="membershipUser">
        /// The membership user.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        public CombinedUserDataHelper(MembershipUser membershipUser, int userID)
        {
            this._userId = userID;
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
            this._userId = userId;
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
                int value = this.DBRow.Field<int?>("NotificationType") ?? 0;

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
                if (this._userDBRow == null && this._userId.HasValue)
                {
                    this._userDBRow = UserMembershipHelper.GetUserRowForID(this._userId.Value,
                        YafContext.Current.Get<YafBoardSettings>().AllowUserInfoCaching);
                }

                return this._userDBRow;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether  DST is Enabled.
        /// </summary>
        public bool DSTUser
        {
            get
            {
                return this.DBRow != null && new UserFlags(this.DBRow["Flags"]).IsDST;
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
            get { return this._userId.HasValue ? this.DBRow.Field<string>("DisplayName") : this.UserName; }
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
                            (int) this.UserID,
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
                int value = this.DBRow.Field<int?>("NotificationType") ?? 0;

                return value.ToEnum<UserNotificationSetting>();
            }
        }

        /// <summary>
        ///   Gets NumPosts.
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
                if (this._userProfile == null && this.UserName.IsSet())
                {
                    // init the profile...
                    this._userProfile = YafUserProfile.GetProfile(this.UserName);
                }

                return this._userProfile;
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
        public int? TimeZone
        {
            get { return this.DBRow.Field<int>("TimeZone"); }
        }

        /// <summary>
        ///   Gets UserID.
        /// </summary>
        public int UserID
        {
            get
            {
                if (this._userId != null)
                {
                    return (int) this._userId;
                }

                return 0;
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

                return this._userId.HasValue ? this.DBRow.Field<string>("Name") : null;
            }
        }

        /// <summary>
        ///   Gets or sets the membership user.
        /// </summary>
        protected MembershipUser MembershipUser
        {
            get
            {
                if (this._membershipUser == null && this._userId.HasValue)
                {
                    this._membershipUser = UserMembershipHelper.GetMembershipUserById(this._userId.Value);
                }

                return this._membershipUser;
            }

            set { this._membershipUser = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The init user data.
        /// </summary>
        /// <exception cref="Exception">Cannot locate user information.</exception>
        private void InitUserData()
        {
            if (this.MembershipUser != null && !this._userId.HasValue)
            {
                if (this._userId == null)
                {
                    // get the user id
                    this._userId = UserMembershipHelper.GetUserIDFromProviderUserKey(this.MembershipUser.ProviderUserKey);
                }
            }

            if (!this._userId.HasValue)
            {
                throw new Exception("Cannot locate user information.");
            }
        }

        #endregion
    }
}