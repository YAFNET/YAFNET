// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CombinedUserDataHelper.cs" company="">
//   
// </copyright>
// <summary>
//   Helps get a complete user profile from various locations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Core
{
	#region Using

	using System;
	using System.Collections.Generic;
	using System.Web.Security;

	using YAF.Classes;
	using YAF.Core.Data;
	using YAF.Types;
	using YAF.Types.Constants;
	using YAF.Types.Flags;
	using YAF.Types.Interfaces;
	using YAF.Types.Interfaces.Extensions;
	using YAF.Utils;
	using YAF.Utils.Extensions;

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
		private IDictionary<string, object> _userDictionary;

		/// <summary>
		///   The _user id.
		/// </summary>
		private int? _userID;

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
		public CombinedUserDataHelper([NotNull] MembershipUser membershipUser, int userID)
		{
			this._userID = userID;
			this.MembershipUser = membershipUser;
			this.InitUserData();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CombinedUserDataHelper"/> class.
		/// </summary>
		/// <param name="membershipUser">
		/// The membership user.
		/// </param>
		public CombinedUserDataHelper([NotNull] MembershipUser membershipUser)
			: this(membershipUser, UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.ProviderUserKey))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CombinedUserDataHelper"/> class.
		/// </summary>
		/// <param name="userID">
		/// The user id.
		/// </param>
		public CombinedUserDataHelper(int userID)
		{
			this._userID = userID;
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
				int value = this.UserDictionary.GetValue("NotificationType").ToType<int?>() ?? 0;

				return (this.UserDictionary.GetValue("AutoWatchTopics").ToType<bool?>() ?? false)
							 || value.ToEnum<UserNotificationSetting>() == UserNotificationSetting.TopicsIPostToOrSubscribeTo;
			}
		}

		/// <summary>
		///   Gets Avatar.
		/// </summary>
		public string Avatar
		{
			get
			{
				return this.UserDictionary.GetValue("Avatar").ToType<string>();
			}
		}

		/// <summary>
		///   Gets Culture.
		/// </summary>
		public string CultureUser
		{
			get
			{
				return this.UserDictionary.GetValue("CultureUser").ToType<string>();
			}
		}

		/// <summary>
		///   Gets a value indicating whether  DST is Enabled.
		/// </summary>
		public bool DSTUser
		{
			get
			{
				if (this.UserDictionary != null)
				{
					if (new UserFlags(this.UserDictionary["Flags"]).IsDST)
					{
						return true;
					}
				}

				return false;
			}
		}

		/// <summary>
		///   Gets a value indicating whether DailyDigest.
		/// </summary>
		public bool DailyDigest
		{
			get
			{
				return this.UserDictionary.GetValue("DailyDigest").ToType<bool?>()
							 ?? YafContext.Current.Get<YafBoardSettings>().DefaultSendDigestEmail;
			}
		}

		/// <summary>
		///   Gets DisplayName.
		/// </summary>
		public string DisplayName
		{
			get
			{
				return this._userID.HasValue ? this.UserDictionary.GetValue("DisplayName").ToType<string>() : this.UserName;
			}
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
              LegacyDb.eventlog_create(this.UserID, this,
                  "ATTENTION! The user with id {0} and name {1} is very possibly is not in your Membership \r\n ".FormatWith(this.UserID, this.UserName) +
                  "data but it's still in you YAF user table. The situation should not normally happen. \r\n " +
                  "You should create a Membership data for the user first and " +
                  "then delete him from YAF user table or leave him.", EventLogTypes.Error);

          }

				return this.IsGuest ? this.UserDictionary.GetValue("Email").ToType<string>() : this.Membership.Email;
			}
		}

		/// <summary>
		///   Gets a value indicating whether HasAvatarImage.
		/// </summary>
		public bool HasAvatarImage
		{
			get
			{
				bool hasImage = false;

				if (this.UserDictionary.GetValue("HasAvatarImage") != null)
				{
					hasImage = this.UserDictionary.GetValue("HasAvatarImage").ToType<long?>() > 0;
				}

				return hasImage;
			}
		}

		/// <summary>
		///   Gets a value indicating whether IsActiveExcluded.
		/// </summary>
		public bool IsActiveExcluded
		{
			get
			{
				if (this.UserDictionary != null)
				{
					if (new UserFlags(this.UserDictionary["Flags"]).IsActiveExcluded)
					{
						return true;
					}
				}

				return false;
			}
		}

		/// <summary>
		///   Gets a value indicating whether IsGuest.
		/// </summary>
		public bool IsGuest
		{
			get
			{
				if (this.UserDictionary != null)
				{
					if (this.UserDictionary["IsGuest"].ToType<int>() > 0)
					{
						return true;
					}
				}

				return false;
			}
		}

		/// <summary>
		///   Gets Joined.
		/// </summary>
		public DateTime? Joined
		{
			get
			{
				return this.UserDictionary.GetValue("Joined").ToType<DateTime?>();
			}
		}

		/// <summary>
		///   Gets LanguageFile.
		/// </summary>
		public string LanguageFile
		{
			get
			{
				return this.UserDictionary.GetValue("LanguageFile").ToType<string>();
			}
		}

		/// <summary>
		///   Gets LastVisit.
		/// </summary>
		public DateTime? LastVisit
		{
			get
			{
				return this.UserDictionary.GetValue("LastVisit").ToType<DateTime?>();
			}
		}

		/// <summary>
		///   Gets Membership.
		/// </summary>
		public MembershipUser Membership
		{
			get
			{
				return this.MembershipUser;
			}
		}

		/// <summary>
		///   Gets NotificationSetting.
		/// </summary>
		public UserNotificationSetting NotificationSetting
		{
			get
			{
				int value = this.UserDictionary.GetValue("NotificationType").ToType<int?>() ?? 0;

				return value.ToEnum<UserNotificationSetting>();
			}
		}

		/// <summary>
		///   Gets NumPosts.
		/// </summary>
		public int? NumPosts
		{
			get
			{
				return this.UserDictionary.GetValue("NumPosts").ToType<int?>();
			}
		}

		/// <summary>
		///   Gets a value indicating whether PMNotification.
		/// </summary>
		public bool PMNotification
		{
			get
			{
				return this.UserDictionary.GetValue("PMNotification").ToType<bool?>() ?? true;
			}
		}

		/// <summary>
		///   Gets Points.
		/// </summary>
		public int? Points
		{
			get
			{
				return this.UserDictionary.GetValue("Points").ToType<int?>();
			}
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
			get
			{
				return this.UserDictionary.GetValue("RankName").ToType<string>();
			}
		}

		/// <summary>
		///   Gets Signature.
		/// </summary>
		public string Signature
		{
			get
			{
				return this.UserDictionary.GetValue("Signature").ToType<string>();
			}
		}

		/// <summary>
		///   Gets User's Text Editor.
		/// </summary>
		public string TextEditor
		{
			get
			{
				return this.UserDictionary.GetValue("TextEditor").ToType<string>();
			}
		}

		/// <summary>
		///   Gets ThemeFile.
		/// </summary>
		public string ThemeFile
		{
			get
			{
				return this.UserDictionary.GetValue("ThemeFile").ToType<string>();
			}
		}

		/// <summary>
		///   Gets TimeZone.
		/// </summary>
		public int? TimeZone
		{
			get
			{
				return this.UserDictionary.GetValue("TimeZone").ToType<int?>();
			}
		}

		/// <summary>
		///   Gets a value indicating whether UseMobileTheme.
		/// </summary>
		public bool UseMobileTheme
		{
			get
			{
				return this.UserDictionary.GetValue("OverrideDefaultThemes").ToType<bool?>() ?? true;
			}
		}

		/// <summary>
		///   Gets a value indicating whether Use Single Sign On.
		/// </summary>
		public bool UseSingleSignOn
		{
			get
			{
				return this.UserDictionary.GetValue("UseSingleSignOn").ToType<bool?>() ?? false;
			}
		}

		/// <summary>
		///   Gets UserDictionary.
		/// </summary>
		[CanBeNull]
		public IDictionary<string, object> UserDictionary
		{
			get
			{
				if (this._userDictionary == null && this._userID.HasValue)
				{
					this._userDictionary =
						UserMembershipHelper.GetUserRowForID(
							this._userID.Value, YafContext.Current.Get<YafBoardSettings>().AllowUserInfoCaching).ToDictionary();
				}

				return this._userDictionary;
			}
		}

		/// <summary>
		///   Gets UserID.
		/// </summary>
		public int UserID
		{
			get
			{
				if (this._userID != null)
				{
					return (int)this._userID;
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

				return this._userID.HasValue ? this.UserDictionary.GetValue("Name").ToType<string>() : null;
			}
		}

		/// <summary>
		///   Gets or sets the membership user.
		/// </summary>
		protected MembershipUser MembershipUser
		{
			get
			{
				if (this._membershipUser == null && this._userID.HasValue)
				{
					this._membershipUser = UserMembershipHelper.GetMembershipUserById(this._userID.Value);
				}

				return this._membershipUser;
			}

			set
			{
				this._membershipUser = value;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// The init user data.
		/// </summary>
		/// <exception cref="Exception">
		/// </exception>
		private void InitUserData()
		{
			if (this.MembershipUser != null && !this._userID.HasValue)
			{
				if (this._userID == null)
				{
					// get the user id
					this._userID = UserMembershipHelper.GetUserIDFromProviderUserKey(this.MembershipUser.ProviderUserKey);
				}
			}

			if (!this._userID.HasValue)
			{
				throw new Exception("Cannot locate user information.");
			}
		}

		#endregion
	}
}