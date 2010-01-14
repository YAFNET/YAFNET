/* Yet Another Forum.net
 * Copyright (C) 2006-2009 Jaben Cargman
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
namespace YAF.Classes.Core
{
  using System;
  using System.Data;
  using System.Web.Security;

  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Helps get a complete user profile from various locations
  /// </summary>
  public class CombinedUserDataHelper
  {
    #region Constants and Fields

    /// <summary>
    /// The _membership user.
    /// </summary>
    private MembershipUser _membershipUser = null;

    /// <summary>
    /// The _row convert.
    /// </summary>
    private DataRowConvert _rowConvert = null;

    /// <summary>
    /// The _user db row.
    /// </summary>
    private DataRow _userDBRow = null;

    /// <summary>
    /// The _user id.
    /// </summary>
    private int? _userID = null;

    /// <summary>
    /// The _user profile.
    /// </summary>
    private YafUserProfile _userProfile = null;

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
      this._userID = userID;
      this._membershipUser = membershipUser;
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
    /// <param name="userID">
    /// The user id.
    /// </param>
    public CombinedUserDataHelper(int userID)
      : this(UserMembershipHelper.GetMembershipUserById(userID), userID)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CombinedUserDataHelper"/> class.
    /// </summary>
    public CombinedUserDataHelper()
      : this(YafContext.Current.PageUserID)
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether AutoWatchTopics.
    /// </summary>
    public bool AutoWatchTopics
    {
      get
      {
        return this.RowConvert.AsBool("AutoWatchTopics");
      }
    }

    /// <summary>
    /// Gets Avatar.
    /// </summary>
    public string Avatar
    {
      get
      {
        return this.RowConvert.AsString("Avatar");
      }
    }

    /// <summary>
    /// Gets DBRow.
    /// </summary>
    public DataRow DBRow
    {
      get
      {
        if (this._userDBRow == null && this._userID.HasValue)
        {
          this._userDBRow = UserMembershipHelper.GetUserRowForID(
            this._userID.Value, YafContext.Current.BoardSettings.AllowUserInfoCaching);
        }

        return this._userDBRow;
      }
    }

    /// <summary>
    /// Gets Email.
    /// </summary>
    public string Email
    {
      get
      {
        if (this.IsGuest)
        {
          return this.RowConvert.AsString("Email");
        }

        return this.Membership.Email;
      }
    }

    /// <summary>
    /// Gets a value indicating whether HasAvatarImage.
    /// </summary>
    public bool HasAvatarImage
    {
      get
      {
        bool hasImage = false;

        if (this.DBRow["HasAvatarImage"] != null && this.DBRow["HasAvatarImage"] != DBNull.Value)
        {
          hasImage = this.RowConvert.AsInt64("HasAvatarImage") > 0;
        }

        return hasImage;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsGuest.
    /// </summary>
    public bool IsGuest
    {
      get
      {
        if (this.DBRow != null)
        {
          if (Convert.ToInt32(this.DBRow["IsGuest"]) > 0)
          {
            return true;
          }
        }

        return false;
      }
    }

    /// <summary>
    /// Gets Joined.
    /// </summary>
    public DateTime? Joined
    {
      get
      {
        return this.RowConvert.AsDateTime("Joined");
      }
    }

    /// <summary>
    /// Gets LanguageFile.
    /// </summary>
    public string LanguageFile
    {
      get
      {
        return this.RowConvert.AsString("LanguageFile");
      }
    }

    /// <summary>
    /// Gets LastVisit.
    /// </summary>
    public DateTime? LastVisit
    {
      get
      {
        return this.RowConvert.AsDateTime("LastVisit");
      }
    }

    /// <summary>
    /// Gets Membership.
    /// </summary>
    public MembershipUser Membership
    {
      get
      {
        return this._membershipUser;
      }
    }

    /// <summary>
    /// Gets NumPosts.
    /// </summary>
    public int? NumPosts
    {
      get
      {
        return this.RowConvert.AsInt32("NumPosts");
      }
    }

    /// <summary>
    /// Gets a value indicating whether OverrideDefaultThemes.
    /// </summary>
    public bool OverrideDefaultThemes
    {
      get
      {
        return this.RowConvert.AsBool("OverrideDefaultThemes");
      }
    }

    /// <summary>
    /// Gets a value indicating whether PMNotification.
    /// </summary>
    public bool PMNotification
    {
      get
      {
        return this.RowConvert.AsBool("PMNotification");
      }
    }

    /// <summary>
    /// Gets Points.
    /// </summary>
    public int? Points
    {
      get
      {
        return this.RowConvert.AsInt32("Points");
      }
    }

    /// <summary>
    /// Gets Profile.
    /// </summary>
    public YafUserProfile Profile
    {
      get
      {
        if (this._userProfile == null && !String.IsNullOrEmpty(this.UserName))
        {
          // init the profile...
          this._userProfile = YafUserProfile.GetProfile(this.UserName);
        }

        return this._userProfile;
      }
    }

    /// <summary>
    /// Gets RankName.
    /// </summary>
    public string RankName
    {
      get
      {
        return this.RowConvert.AsString("RankName");
      }
    }

    /// <summary>
    /// Gets Signature.
    /// </summary>
    public string Signature
    {
      get
      {
        return this.RowConvert.AsString("Signature");
      }
    }

    /// <summary>
    /// Gets ThemeFile.
    /// </summary>
    public string ThemeFile
    {
      get
      {
        return this.RowConvert.AsString("ThemeFile");
      }
    }

    /// <summary>
    /// Gets TimeZone.
    /// </summary>
    public int? TimeZone
    {
      get
      {
        return this.RowConvert.AsInt32("TimeZone");
      }
    }

    /// <summary>
    /// Gets UserID.
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
    /// Gets UserName.
    /// </summary>
    public string UserName
    {
      get
      {
        if (this._membershipUser != null)
        {
          return this._membershipUser.UserName;
        }
        else if (this._userID.HasValue)
        {
          return this.RowConvert.AsString("Name");
        }

        return null;
      }
    }

    /// <summary>
    /// Gets RowConvert.
    /// </summary>
    protected DataRowConvert RowConvert
    {
      get
      {
        if (this._rowConvert == null && this.DBRow != null)
        {
          this._rowConvert = new DataRowConvert(this.DBRow);
        }

        return this._rowConvert;
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
      if (this._membershipUser != null && !this._userID.HasValue)
      {
        if (this._userID == null)
        {
          // get the user id
          this._userID = UserMembershipHelper.GetUserIDFromProviderUserKey(this._membershipUser.ProviderUserKey);
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