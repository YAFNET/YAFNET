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
            : this(YafContext.Current.PageUserID)
        {

        }

        private void InitUserData()
        {
            if (_membershipUser != null && !_userID.HasValue)
            {
                if (_userID == null)
                {
                    // get the user id
                    _userID = UserMembershipHelper.GetUserIDFromProviderUserKey(_membershipUser.ProviderUserKey);
                }
            }

            if (!_userID.HasValue)
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

        public string UserName
        {
            get
            {
                if (_membershipUser != null)
                {
                    return _membershipUser.UserName;
                }
                else if (_userID.HasValue)
                {
                    return RowConvert.AsString("Name");
                }

                return null;
            }
        }

        public bool IsGuest
        {
            get
            {
                if (DBRow != null)
                {
                    if (Convert.ToInt32(DBRow["IsGuest"]) > 0) return true;
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

        private YafUserProfile _userProfile = null;
        public YafUserProfile Profile
        {
            get
            {
                if (_userProfile == null && !String.IsNullOrEmpty(UserName))
                {
                    // init the profile...
                    _userProfile = YafUserProfile.GetProfile(UserName);
                }

                return _userProfile;
            }
        }

        private DataRow _userDBRow = null;
        public DataRow DBRow
        {
            get
            {
                if (_userDBRow == null && _userID.HasValue)
                {
                    _userDBRow = UserMembershipHelper.GetUserRowForID(_userID.Value, YafContext.Current.BoardSettings.AllowUserInfoCaching);
                }

                return _userDBRow;
            }
        }

        private DataRowConvert _rowConvert = null;
        protected DataRowConvert RowConvert
        {
            get
            {
                if (_rowConvert == null && DBRow != null)
                {
                    _rowConvert = new DataRowConvert(DBRow);
                }

                return _rowConvert;
            }
        }

        public string Email
        {
            get
            {
                if (IsGuest)
                {
                    return RowConvert.AsString("Email");
                }

                return Membership.Email;
            }
        }

        public string ThemeFile
        {
            get
            {
                return RowConvert.AsString("ThemeFile");
            }
        }

        public string LanguageFile
        {
            get
            {
                return RowConvert.AsString("LanguageFile");
            }
        }

        public string Signature
        {
            get
            {
                return RowConvert.AsString("Signature");
            }
        }

        public string Avatar
        {
            get
            {
                return RowConvert.AsString("Avatar");
            }
        }

        public string RankName
        {
            get
            {
                return RowConvert.AsString("RankName");
            }
        }

        public int? NumPosts
        {
            get
            {
                return RowConvert.AsInt32("NumPosts");
            }
        }

        public int? TimeZone
        {
            get
            {
                return RowConvert.AsInt32("TimeZone");
            }
        }

        public int? Points
        {
            get
            {
                return RowConvert.AsInt32("Points");
            }
        }

        public bool OverrideDefaultThemes
        {
            get
            {
                return RowConvert.AsBool("OverrideDefaultThemes");
            }
        }

        public bool PMNotification
        {
            get
            {
                return RowConvert.AsBool("PMNotification");
            }
        }

        public DateTime? Joined
        {
            get
            {
                return RowConvert.AsDateTime("Joined");
            }
        }

        public DateTime? LastVisit
        {
            get
            {
                return RowConvert.AsDateTime("LastVisit");
            }
        }

        public bool HasAvatarImage
        {
            get
            {
                bool hasImage = false;

                if (DBRow["HasAvatarImage"] != null && DBRow["HasAvatarImage"] != DBNull.Value)
                {
                    hasImage = RowConvert.AsInt64("HasAvatarImage") > 0;
                }

                return hasImage;
            }
        }

        public bool AutoWatchTopics
        {
            get
            {
                return RowConvert.AsBool("AutoWatchTopics");
            }
        }
    }
}
