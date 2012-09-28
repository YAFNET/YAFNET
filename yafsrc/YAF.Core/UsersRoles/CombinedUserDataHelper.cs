/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
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
    ///   The _row convert.
    /// </summary>
    private DataRowConvert _rowConvert;

    /// <summary>
    ///   The _user db row.
    /// </summary>
    private DataRow _userDBRow;

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
    public CombinedUserDataHelper(MembershipUser membershipUser, int userID)
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
    ///   Gets a value indicating whether Use Single Sign On.
    /// </summary>
    public bool UseSingleSignOn
    {
        get
        {
            return this.RowConvert.AsBool("UseSingleSignOn") ?? false;
        }
    }

    /// <summary>
    ///   Gets a value indicating whether AutoWatchTopics.
    /// </summary>
    public bool AutoWatchTopics
    {
      get
      {
          int value = this.RowConvert.AsInt32("NotificationType") ?? 0;

          return (this.RowConvert.AsBool("AutoWatchTopics") ?? false)
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
        return this.RowConvert.AsString("Avatar");
      }
    }

    /// <summary>
    ///   Gets Culture.
    /// </summary>
    public string CultureUser
    {
      get
      {
        return this.RowConvert.AsString("CultureUser");
      }
    }

    /// <summary>
    ///   Gets User's Text Editor.
    /// </summary>
    public string TextEditor
    {
        get
        {
            return this.RowConvert.AsString("TextEditor");
        }
    }

    /// <summary>
    ///   Gets DBRow.
    /// </summary>
    public DataRow DBRow
    {
      get
      {
        if (this._userDBRow == null && this._userID.HasValue)
        {
          this._userDBRow = UserMembershipHelper.GetUserRowForID(
            this._userID.Value, YafContext.Current.Get<YafBoardSettings>().AllowUserInfoCaching);
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
        if (this.DBRow != null)
        {
          if (new UserFlags(this.DBRow["Flags"]).IsDST)
          {
            return true;
          }
        }

        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether DailyDigest.
    /// </summary>
    public bool DailyDigest
    {
      get
      {
          return this.RowConvert.AsBool("DailyDigest") ?? YafContext.Current.Get<YafBoardSettings>().DefaultSendDigestEmail;
      }
    }

    /// <summary>
    ///   Gets DisplayName.
    /// </summary>
    public string DisplayName
    {
      get
      {
          return this._userID.HasValue ? this.RowConvert.AsString("DisplayName") : this.UserName;
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
              LegacyDb.eventlog_create(
                  this.UserID,
                  this,
                  "ATTENTION! The user with id {0} and name {1} is very possibly is not in your Membership \r\n ".FormatWith(this.UserID, this.UserName)
                  + "data but it's still in you YAF user table. The situation should not normally happen. \r\n "
                  + "You should create a Membership data for the user first and "
                  + "then delete him from YAF user table or leave him.",
                  EventLogTypes.Error);
          }

          return this.IsGuest ? this.RowConvert.AsString("Email") : this.Membership.Email;
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

        if (this.DBRow["HasAvatarImage"] != null && this.DBRow["HasAvatarImage"] != DBNull.Value)
        {
          hasImage = this.RowConvert.AsInt64("HasAvatarImage") > 0;
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
        if (this.DBRow != null)
        {
          if (new UserFlags(this.DBRow["Flags"]).IsActiveExcluded)
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
        if (this.DBRow != null)
        {
          if (this.DBRow["IsGuest"].ToType<int>() > 0)
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
        return this.RowConvert.AsDateTime("Joined");
      }
    }

    /// <summary>
    ///   Gets LanguageFile.
    /// </summary>
    public string LanguageFile
    {
      get
      {
        return this.RowConvert.AsString("LanguageFile");
      }
    }

    /// <summary>
    ///   Gets LastVisit.
    /// </summary>
    public DateTime? LastVisit
    {
      get
      {
        return this.RowConvert.AsDateTime("LastVisit");
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
    /// Gets NotificationSetting.
    /// </summary>
    public UserNotificationSetting NotificationSetting
    {
      get
      {
        int value = this.RowConvert.AsInt32("NotificationType") ?? 0;

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
        return this.RowConvert.AsInt32("NumPosts");
      }
    }

    /// <summary>
    ///   Gets a value indicating whether UseMobileTheme.
    /// </summary>
    public bool UseMobileTheme
    {
      get
      {
        return this.RowConvert.AsBool("OverrideDefaultThemes") ?? true;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether PMNotification.
    /// </summary>
    public bool PMNotification
    {
      get
      {
        return this.RowConvert.AsBool("PMNotification") ?? true;
      }
    }

    /// <summary>
    ///   Gets Points.
    /// </summary>
    public int? Points
    {
      get
      {
        return this.RowConvert.AsInt32("Points");
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
        return this.RowConvert.AsString("RankName");
      }
    }

    /// <summary>
    ///   Gets Signature.
    /// </summary>
    public string Signature
    {
      get
      {
        return this.RowConvert.AsString("Signature");
      }
    }

    /// <summary>
    ///   Gets ThemeFile.
    /// </summary>
    public string ThemeFile
    {
      get
      {
        return this.RowConvert.AsString("ThemeFile");
      }
    }

    /// <summary>
    ///   Gets TimeZone.
    /// </summary>
    public int? TimeZone
    {
      get
      {
        return this.RowConvert.AsInt32("TimeZone");
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

          return this._userID.HasValue ? this.RowConvert.AsString("Name") : null;
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

    /// <summary>
    ///   Gets RowConvert.
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
    /// <exception cref="Exception">Cannot locate user information.</exception>
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