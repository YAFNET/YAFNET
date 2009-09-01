using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Security;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
{
	/// <summary>
	/// Helps get a complete user profile from various locations
	/// </summary>
	public class CombinedUserDataHelper
	{
		private MembershipUser _membershipUser = null;
		private YafUserProfile _userProfile = null;
		private DataRow _userDBRow = null;
		private DataRowConvert _rowConvert = null;
		private int? _userID = null;

		public CombinedUserDataHelper(MembershipUser membershipUser, int userID)
		{
			_userID = userID;
			_membershipUser = membershipUser;
			InitUserData();
		}

		public CombinedUserDataHelper(MembershipUser membershipUser)
			: this(membershipUser, UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.ProviderUserKey))
		{
		}

		public CombinedUserDataHelper(int userID)
			: this(UserMembershipHelper.GetMembershipUserById(userID), userID)
		{
		}

		public CombinedUserDataHelper()
			: this( YafContext.Current.PageUserID )
		{
			
		}

		private void InitUserData()
		{
			if (_membershipUser != null)
			{
				if (_userID == null)
				{
					// get the user id
					_userID = UserMembershipHelper.GetUserIDFromProviderUserKey(_membershipUser.ProviderUserKey);
				}
				_userProfile = YafUserProfile.GetProfile(_membershipUser.UserName);
				// get the data for this user from the DB...
				DataRow userRow = UserMembershipHelper.GetUserRowForID((int)_userID, false);
				if (userRow != null)
				{
					_userDBRow = userRow;
					_rowConvert = new DataRowConvert(_userDBRow);
				}
			}
			else if (_userID != null)
			{
				// see if this is the guest user
				DataRow userRow = UserMembershipHelper.GetUserRowForID((int)_userID, false);
				if (userRow != null)
				{
					_userDBRow = userRow;
					_userProfile = YafUserProfile.GetProfile(_userDBRow["Name"].ToString());
					_rowConvert = new DataRowConvert(_userDBRow);
				}
			}
			else
			{
				throw new Exception("Cannot locate user information.");
			}
		}

		public int UserID
		{
			get
			{
				if (_userID != null) return (int)_userID;

				return 0;
			}
		}

		public bool IsGuest
		{
			get
			{
				if (_userDBRow != null)
				{
					if (Convert.ToInt32(_userDBRow["IsGuest"]) > 0) return true;
				}

				return false;
			}
		}

		public MembershipUser Membership
		{
			get
			{
				return _membershipUser;
			}
		}

		public YafUserProfile Profile
		{
			get
			{
				return _userProfile;
			}
		}

		public DataRow DBRow
		{
			get
			{
				return _userDBRow;
			}
		}

		public string Email
		{
			get
			{
				if (IsGuest)
				{
					return _rowConvert.AsString("Email");
				}

				return Membership.Email;
			}
		}

		public string ThemeFile
		{
			get
			{
				return _rowConvert.AsString("ThemeFile");
			}
		}

		public string LanguageFile
		{
			get
			{
				return _rowConvert.AsString("LanguageFile");
			}
		}

		public string Signature
		{
			get
			{
				return _rowConvert.AsString("Signature");
			}
		}

		public string Avatar
		{
			get
			{
				return _rowConvert.AsString("Avatar");
			}
		}

		public string RankName
		{
			get
			{
				return _rowConvert.AsString("RankName");
			}
		}

		public int? NumPosts
		{
			get
			{
				return _rowConvert.AsInt32("NumPosts");
			}
		}

		public int? TimeZone
		{
			get
			{
				return _rowConvert.AsInt32("TimeZone");
			}
		}

		public int? Points
		{
			get
			{
				return _rowConvert.AsInt32("Points");
			}
		}

		public bool OverrideDefaultThemes
		{
			get
			{
				return _rowConvert.AsBool("OverrideDefaultThemes");
			}
		}

		public bool PMNotification
		{
			get
			{
				return _rowConvert.AsBool("PMNotification");
			}
		}

		public DateTime? Joined
		{
			get
			{
				return _rowConvert.AsDateTime("Joined");
			}
		}

		public DateTime? LastVisit
		{
			get
			{
				return _rowConvert.AsDateTime("LastVisit");
			}
		}

		public bool HasAvatarImage
		{
			get
			{
				bool hasImage = false;

				if (DBRow["HasAvatarImage"] != null && DBRow["HasAvatarImage"] != DBNull.Value)
				{
					hasImage = _rowConvert.AsInt64("HasAvatarImage") > 0;
				}

				return hasImage;
			}
		}
	}
}
