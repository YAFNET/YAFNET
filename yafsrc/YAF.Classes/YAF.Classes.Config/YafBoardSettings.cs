/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Classes
{
  using System;
  using System.Web.Security;
  using YAF.Classes.Pattern;

  /// <summary>
  /// The yaf board settings.
  /// </summary>
  public class YafBoardSettings
  {
    /// <summary>
    /// The _reg.
    /// </summary>
    protected readonly RegistryDictionaryOverride _reg;

    /// <summary>
    /// The _reg board.
    /// </summary>
    protected readonly RegistryDictionary _regBoard;

    /// <summary>
    /// The _board id.
    /// </summary>
    protected object _boardID;

    /// <summary>
    /// The _legacy board settings.
    /// </summary>
    protected YafLegacyBoardSettings _legacyBoardSettings = new YafLegacyBoardSettings();

    /// <summary>
    /// The _membership app name.
    /// </summary>
    protected string _membershipAppName;

    /// <summary>
    /// The _roles app name.
    /// </summary>
    protected string _rolesAppName;

    /// <summary>
    /// Initializes a new instance of the <see cref="YafBoardSettings"/> class.
    /// </summary>
    public YafBoardSettings()
    {
      this._boardID = 0;
      this._reg = new RegistryDictionaryOverride();
      this._regBoard = new RegistryDictionary();

      // set the board dictionary as the override...
      this._reg.OverrideDictionary = this._regBoard;

      this._membershipAppName = Membership.ApplicationName;
      this._rolesAppName = Roles.ApplicationName;
    }

    // Board/Override properties...
    // Future stuff... still in progress.
    /// <summary>
    /// Gets or sets a value indicating whether SetBoardRegistryOnly.
    /// </summary>
    public bool SetBoardRegistryOnly
    {
      get
      {
        return this._reg.DefaultSetOverride;
      }

      set
      {
        this._reg.DefaultSetOverride = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether GetBoardRegistryOverride.
    /// </summary>
    public bool GetBoardRegistryOverride
    {
      get
      {
        return this._reg.DefaultGetOverride;
      }

      set
      {
        this._reg.DefaultGetOverride = value;
      }
    }

    // Provider Settings

    /// <summary>
    /// Gets MembershipAppName.
    /// </summary>
    public string MembershipAppName
    {
      get
      {
        return this._membershipAppName;
      }
    }

    /// <summary>
    /// Gets RolesAppName.
    /// </summary>
    public string RolesAppName
    {
      get
      {
        return this._rolesAppName;
      }
    }

    // individual board settings
    /// <summary>
    /// Gets Name.
    /// </summary>
    public string Name
    {
      get
      {
        return this._legacyBoardSettings.BoardName;
      }
    }

    /// <summary>
    /// Gets a value indicating whether AllowThreaded.
    /// </summary>
    public bool AllowThreaded
    {
      get
      {
        return this._legacyBoardSettings.AllowThreaded;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowThemedLogo.
    /// </summary>
    public bool AllowThemedLogo
    {
      get
      {
        return this._reg.GetValue<bool>("AllowThemedLogo", false);
      }

      set
      {
        this._reg.SetValue<bool>("AllowThemedLogo", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether User genger icons in user box are enabled.
    /// </summary>
    public bool AllowGenderInUserBox
    {
        get
        {
            return this._reg.GetValue<bool>("AllowGenderInUserBox", true);
        }

        set
        {
            this._reg.SetValue<bool>("AllowGenderInUserBox", value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to enable Display Name.
    /// </summary>
    public bool EnableDisplayName
    {
      get
      {
        return this._reg.GetValue<bool>("EnableDisplayName", false);
      }

      set
      {
        this._reg.SetValue<bool>("EnableDisplayName", value);
      }
    }

    /// <summary>
    /// Gets MaxUsers.
    /// </summary>
    public int MaxUsers
    {
      get
      {
        return this._regBoard.GetValue<int>("MaxUsers", 1);
      }
    }

    /// <summary>
    /// Gets MaxUsersWhen.
    /// </summary>
    public DateTime MaxUsersWhen
    {
      get
      {
        return this._regBoard.GetValue<DateTime>("MaxUsersWhen", DateTime.UtcNow);
      }
    }

    /// <summary>
    /// Gets or sets Theme.
    /// </summary>
    public string MobileTheme
    {
      get
      {
        return this._regBoard.GetValue<string>("MobileTheme", "yafmobile.xml");
      }

      set
      {
        this._regBoard.SetValue<string>("MobileTheme", value);
      }
    }

    /// <summary>
    /// Gets or sets Theme.
    /// </summary>
    public string Theme
    {
      get
      {
        return this._regBoard.GetValue<string>("Theme", "cleanslate.xml");
      }

      set
      {
        this._regBoard.SetValue<string>("Theme", value);
      }
    }

    /// <summary>
    /// Gets or sets Language.
    /// </summary>
    public string Language
    {
      get
      {
        return this._regBoard.GetValue<string>("Language", "english.xml");
      }

      set
      {
        this._regBoard.SetValue<string>("Language", value);
      }
    }

    /// <summary>
    /// Gets or sets Culture.
    /// </summary>
    public string Culture
    {
        get
        {
            return this._regBoard.GetValue<string>("Culture", "en-US");
        }

        set
        {
            this._regBoard.SetValue<string>("Culture", value);
        }
    }

    public UserNotificationSetting DefaultNotificationSetting
    {
      get
      {
        return this._regBoard.GetValue<UserNotificationSetting>("DefaultNotificationSetting", UserNotificationSetting.TopicsIPostToOrSubscribeTo);
      }

      set
      {
        this._regBoard.SetValue<UserNotificationSetting>("DefaultNotificationSetting", value);
      }
    }

    public bool DefaultSendDigestEmail
    {
      get
      {
        return this._regBoard.GetValue<bool>("DefaultSendDigestEmail", false);
      }

      set
      {
        this._regBoard.SetValue<bool>("DefaultSendDigestEmail", value);
      }
    }

    public bool AllowDigestEmail
    {
      get
      {
        return this._regBoard.GetValue<bool>("AllowDigestEmail", false);
      }

      set
      {
        this._regBoard.SetValue<bool>("AllowDigestEmail", value);
      }
    }

    /// <summary>
    /// Gets or sets ShowTopicsDefault.
    /// </summary>
    public int ShowTopicsDefault
    {
      get
      {
        return this._regBoard.GetValue<int>("ShowTopicsDefault", 0);
      }

      set
      {
        this._regBoard.SetValue<int>("ShowTopicsDefault", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether FileExtensionAreAllowed.
    /// </summary>
    public bool FileExtensionAreAllowed
    {
      get
      {
        return this._regBoard.GetValue<bool>("FileExtensionAreAllowed", true);
      }

      set
      {
        this._regBoard.SetValue<bool>("FileExtensionAreAllowed", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Recapture can have Multiple Instances.
    /// </summary>
    public bool RecaptureMultipleInstances
    {
        get
        {
            return this._regBoard.GetValue<bool>("RecaptureMultipleInstances", true);
        }

        set
        {
            this._regBoard.SetValue<bool>("RecaptureMultipleInstances", value);
        }
    }

    /// <summary>
    /// Gets or sets NotificationOnUserRegisterEmailList.
    /// </summary>
    public string NotificationOnUserRegisterEmailList
    {
      get
      {
        return this._regBoard.GetValue<string>("NotificationOnUserRegisterEmailList", null);
      }

      set
      {
        this._regBoard.SetValue<string>("NotificationOnUserRegisterEmailList", value);
      }
    }

    public bool EmailModeratorsOnModeratedPost
    {
      get
      {
        return this._regBoard.GetValue<bool>("EmailModeratorsOnModeratedPost", true);
      }

      set
      {
        this._regBoard.SetValue<bool>("EmailModeratorsOnModeratedPost", value);
      }
    }

    // didn't know where else to put this :)
    /// <summary>
    /// Gets SQLVersion.
    /// </summary>
    public string SQLVersion
    {
      get
      {
        return this._legacyBoardSettings.SqlVersion;
      }
    }

    #region int settings

    /// <summary>
    /// Gets or sets ServerTimeCorrection.
    /// </summary>
    public int ServerTimeCorrection
    {
      get
      {
          return this._reg.GetValue<int>("ServerTimeCorrection", 0);
      }

      set
      {
          this._reg.SetValue<int>("ServerTimeCorrection", value);
      }
    }

    /// <summary>
    /// Gets or sets AvatarWidth.
    /// </summary>
    public int AvatarWidth
    {
      get
      {
        return this._reg.GetValue<int>("AvatarWidth", 50);
      }

      set
      {
        this._reg.SetValue<int>("AvatarWidth", value);
      }
    }

    /// <summary>
    /// Gets or sets AvatarHeight.
    /// </summary>
    public int AvatarHeight
    {
      get
      {
        return this._reg.GetValue<int>("AvatarHeight", 80);
      }

      set
      {
        this._reg.SetValue<int>("AvatarHeight", value);
      }
    }

    /// <summary>
    /// Gets or sets AvatarSize.
    /// </summary>
    public int AvatarSize
    {
      get
      {
        return this._reg.GetValue<int>("AvatarSize", 50000);
      }

      set
      {
        this._reg.SetValue<int>("AvatarSize", value);
      }
    }

    /// <summary>
    /// Gets or sets MaxWordLength. Used in topic names etc. to avoid layout distortions.
    /// </summary>
    public int MaxWordLength
    {
        get
        {
            return this._reg.GetValue<int>("MaxWordLength", 40);
        }

        set
        {
            this._reg.SetValue<int>("MaxWordLength", value);
        }
    }
    /// <summary>
    /// Gets or sets MaxFileSize.
    /// </summary>
    public int MaxFileSize
    {
      get
      {
        return this._reg.GetValue<int>("MaxFileSize", 0);
      }

      set
      {
        this._reg.SetValue<int>("MaxFileSize", value);
      }
    }
    /// <summary>
    /// Gets or sets SmiliesColumns.
    /// </summary>
    public int SmiliesColumns
    {
      get
      {
        return this._reg.GetValue<int>("SmiliesColumns", 3);
      }

      set
      {
        this._reg.SetValue<int>("SmiliesColumns", value);
      }
    }

    /// <summary>
    /// Gets or sets SmiliesPerRow.
    /// </summary>
    public int SmiliesPerRow
    {
      get
      {
        return this._reg.GetValue<int>("SmiliesPerRow", 6);
      }

      set
      {
        this._reg.SetValue<int>("SmiliesPerRow", value);
      }
    }

    /// <summary>
    /// Message History Days To Trace.
    /// </summary>
    public int MessageHistoryDaysToLog
    {
        get
        {
            return this._reg.GetValue<int>("MessageHistoryDaysToLog", 30);
        }

        set
        {
            this._reg.SetValue<int>("MessageHistoryDaysToLog", value);
        }
    }      

    /// <summary>
    /// Gets or sets LockPosts.
    /// </summary>
    public int LockPosts
    {
      get
      {
        return this._reg.GetValue<int>("LockPosts", 0);
      }

      set
      {
        this._reg.SetValue<int>("LockPosts", value);
      }
    }

    /// <summary>
    /// Gets or sets PostsPerPage.
    /// </summary>
    public int PostsPerPage
    {
      get
      {
        return this._reg.GetValue<int>("PostsPerPage", 20);
      }

      set
      {
        this._reg.SetValue<int>("PostsPerPage", value);
      }
    }

    /// <summary>
    /// Gets or sets TopicsPerPage.
    /// </summary>
    public int TopicsPerPage
    {
      get
      {
        return this._reg.GetValue<int>("TopicsPerPage", 15);
      }

      set
      {
        this._reg.SetValue<int>("TopicsPerPage", value);
      }
    }

    /// <summary>
    /// Gets or sets ForumEditor.
    /// </summary>
    public int ForumEditor
    {
      get
      {
        return this._reg.GetValue<int>("ForumEditor", 1);
      }

      set
      {
        this._reg.SetValue<int>("ForumEditor", value);
      }
    }

    /// <summary>
    /// Gets or sets PostFloodDelay.
    /// </summary>
    public int PostFloodDelay
    {
      get
      {
        return this._reg.GetValue<int>("PostFloodDelay", 30);
      }

      set
      {
        this._reg.SetValue<int>("PostFloodDelay", value);
      }
    }

    /// <summary>
    /// Gets or sets AllowedPollChoiceNumber.
    /// </summary>
    public int AllowedPollChoiceNumber
    {
        get
        {
            return this._reg.GetValue<int>("AllowedPollChoiceNumber", 10);
        }

        set
        {
            this._reg.SetValue<int>("AllowedPollChoiceNumber", value);
        }
    }

    /// <summary>
    /// Gets or sets AllowedPollNumber.
    /// </summary>
    public int AllowedPollNumber
    {
        get
        {
            return this._reg.GetValue<int>("AllowedPollNumber", 3);
        }

        set
        {
            this._reg.SetValue<int>("AllowedPollNumber", value);
        }
    }

    /// <summary>
    /// Gets or sets CaptchaTypeRegister.
    /// </summary>
    public int CaptchaTypeRegister
    {
        get
        {
            return this._reg.GetValue<int>("CaptchaTypeRegister", 1);
        }

        set
        {
            this._reg.SetValue<int>("CaptchaTypeRegister", value);
        }
    }

    /// <summary>
    /// Gets or sets EditTimeOut.
    /// </summary>
    public int EditTimeOut
    {
      get
      {
        return this._reg.GetValue<int>("EditTimeOut", 30);
      }

      set
      {
        this._reg.SetValue<int>("EditTimeOut", value);
      }
    }

    // vzrus 6/11/10
    /// <summary>
    /// Gets or sets a value indicating whether someone can report posts as violating forum rules.
    /// </summary>
    public int ReportPostPermissions
    {
        get
        {
            return this._reg.GetValue<int>("ReportPostPermissions", (int)ViewPermissions.RegisteredUsers);
        }

        set
        {
            this._reg.SetValue<int>("ReportPostPermissions", value);
        }
    }

    /// <summary>
    /// Gets or sets CaptchaSize.
    /// </summary>
    public int CaptchaSize
    {
      get
      {
        return this._reg.GetValue<int>("CaptchaSize", 5);
      }

      set
      {
        this._reg.SetValue<int>("CaptchaSize", value);
      }
    }

    // Ederon : 11/21/2007
    /// <summary>
    /// Gets or sets ProfileViewPermissions.
    /// </summary>
    public int ProfileViewPermissions
    {
      get
      {
        return this._reg.GetValue<int>("ProfileViewPermission", (int) ViewPermissions.RegisteredUsers);
      }

      set
      {
        this._reg.SetValue<int>("ProfileViewPermission", value);
      }
    }

    /// <summary>
    /// Gets or sets ReturnSearchMax.
    /// </summary>
    public int ReturnSearchMax
    {
      get
      {
        return this._reg.GetValue<int>("ReturnSearchMax", 100);
      }

      set
      {
        this._reg.SetValue<int>("ReturnSearchMax", value);
      }
    }

    // Ederon : 12/9/2007
    /// <summary>
    /// Gets or sets ActiveUsersViewPermissions.
    /// </summary>
    public int ActiveUsersViewPermissions
    {
      get
      {
        return this._reg.GetValue<int>("ActiveUsersViewPermissions", (int) ViewPermissions.RegisteredUsers);
      }

      set
      {
        this._reg.SetValue<int>("ActiveUsersViewPermissions", value);
      }
    }

    /// <summary>
    /// Gets or sets MembersListViewPermissions.
    /// </summary>
    public int MembersListViewPermissions
    {
      get
      {
        return this._reg.GetValue<int>("MembersListViewPermissions", (int) ViewPermissions.RegisteredUsers);
      }

      set
      {
        this._reg.SetValue<int>("MembersListViewPermissions", value);
      }
    }

    // Ederon : 12/14/2007
    /// <summary>
    /// Gets or sets ActiveDiscussionsCount.
    /// </summary>
    public int ActiveDiscussionsCount
    {
      get
      {
        return this._reg.GetValue<int>("ActiveDiscussionsCount", 5);
      }

      set
      {
        this._reg.SetValue<int>("ActiveDiscussionsCount", value);
      }
    }

    /// <summary>
    /// Gets or sets ActiveDiscussionsCacheTimeout.
    /// </summary>
    public int ActiveDiscussionsCacheTimeout
    {
      get
      {
        return this._reg.GetValue<int>("ActiveDiscussionsCacheTimeout", 1);
      }

      set
      {
        this._reg.SetValue<int>("ActiveDiscussionsCacheTimeout", value);
      }
    }

    /// <summary>
    /// Gets or sets SearchStringMinLength.
    /// </summary>
    public int SearchStringMinLength
    {
      get
      {
        return this._reg.GetValue<int>("SearchStringMinLength", 4);
      }

      set
      {
        this._reg.SetValue<int>("SearchStringMinLength", value);
      }
    }

    /// <summary>
    /// Gets or sets SearchStringMaxLength.
    /// </summary>
    public int SearchStringMaxLength
    {
      get
      {
        return this._reg.GetValue<int>("SearchStringMaxLength", 50);
      }

      set
      {
        this._reg.SetValue<int>("SearchStringMaxLength", value);
      }
    }

    /// <summary>
    /// Gets or sets SearchPermissions.
    /// </summary>
    public int SearchPermissions
    {
      get
      {
        return this._reg.GetValue<int>("SearchPermissions", (int) ViewPermissions.Everyone);
      }

      set
      {
        this._reg.SetValue<int>("SearchPermissions", value);
      }
    }


    /// <summary>
    /// Gets or sets BoardPollID.
    /// </summary>
    public int BoardPollID
    {
        get
        {
            return this._reg.GetValue<int>("BoardPollID", 0);
        }

        set
        {
            this._reg.SetValue<int>("BoardPollID", value);
        }
    }

    /// <summary>
    /// Gets or sets ExternalSearchPermissions.
    /// </summary>
    public int ExternalSearchPermissions
    {
        get
        {
            return this._reg.GetValue<int>("ExternalSearchPermissions", (int)ViewPermissions.Nobody);
        }

        set
        {
            this._reg.SetValue<int>("ExternalSearchPermissions", value);
        }
    }

    /// <summary>
    /// Gets or sets ForumStatisticsCacheTimeout.
    /// </summary>
    public int ForumStatisticsCacheTimeout
    {
      get
      {
        return this._reg.GetValue<int>("ForumStatisticsCacheTimeout", 60);
      }

      set
      {
        this._reg.SetValue<int>("ForumStatisticsCacheTimeout", value);
      }
    }

    /// <summary>
    /// Gets or sets BoardUserStatsCacheTimeout.
    /// </summary>
    public int BoardUserStatsCacheTimeout
    {
        get
        {
            return this._reg.GetValue<int>("BoardUserStatsCacheTimeout", 60);
        }

        set
        {
            this._reg.SetValue<int>("BoardUserStatsCacheTimeout", value);
        }
    }

    // Ederon 12/18/2007
    /// <summary>
    /// Gets or sets PrivateMessageMaxRecipients.
    /// </summary>
    public int PrivateMessageMaxRecipients
    {
      get
      {
        return this._reg.GetValue<int>("PrivateMessageMaxRecipients", 1);
      }

      set
      {
        this._reg.SetValue<int>("PrivateMessageMaxRecipients", value);
      }
    }

    /// <summary>
    /// Gets or sets DisableNoFollowLinksAfterDay.
    /// </summary>
    public int DisableNoFollowLinksAfterDay
    {
      get
      {
        return this._reg.GetValue<int>("DisableNoFollowLinksAfterDay", 0);
      }

      set
      {
        this._reg.SetValue<int>("DisableNoFollowLinksAfterDay", value);
      }
    }

    // Ederon : 01/18/2007
    /// <summary>
    /// Gets or sets BoardForumListAllGuestCacheTimeout.
    /// </summary>
    public int BoardForumListAllGuestCacheTimeout
    {
      get
      {
        return this._reg.GetValue<int>("BoardForumListAllGuestCacheTimeout", 1440);
      }

      set
      {
        this._reg.SetValue<int>("BoardForumListAllGuestCacheTimeout", value);
      }
    }

    /// <summary>
    /// Gets or sets BoardModeratorsCacheTimeout.
    /// </summary>
    public int BoardModeratorsCacheTimeout
    {
      get
      {
        return this._reg.GetValue<int>("BoardModeratorsCacheTimeout", 1440);
      }

      set
      {
        this._reg.SetValue<int>("BoardModeratorsCacheTimeout", value);
      }
    }

    /// <summary>
    /// Gets or sets BoardCategoriesCacheTimeout.
    /// </summary>
    public int BoardCategoriesCacheTimeout
    {
      get
      {
        return this._reg.GetValue<int>("BoardCategoriesCacheTimeout", 1440);
      }

      set
      {
        this._reg.SetValue<int>("BoardCategoriesCacheTimeout", value);
      }
    }

    // Ederon : 02/07/2008
    /// <summary>
    /// Gets or sets ReplaceRulesCacheTimeout.
    /// </summary>
    public int ReplaceRulesCacheTimeout
    {
      get
      {
        return this._reg.GetValue<int>("ReplaceRulesCacheTimeout", 1440);
      }

      set
      {
        this._reg.SetValue<int>("ReplaceRulesCacheTimeout", value);
      }
    }

    /// <summary>
    /// Gets or sets FirstPostCacheTimeout.
    /// </summary>
    public int FirstPostCacheTimeout
    {
      get
      {
        return this._reg.GetValue<int>("FirstPostCacheTimeout", 120);
      }

      set
      {
        this._reg.SetValue<int>("FirstPostCacheTimeout", value);
      }
    }

    /// <summary>
    /// Gets or sets MaxPostSize.
    /// </summary>
    public int MaxPostSize
    {
      get
      {
        return this._reg.GetValue<int>("MaxPostSize", Int16.MaxValue);
      }

      set
      {
        this._reg.SetValue<int>("MaxPostSize", value);
      }
    }

    /// <summary>
    /// Gets or sets MaxReportPostChars.
    /// </summary>
    public int MaxReportPostChars
    {
        get
        {
            return this._reg.GetValue<int>("MaxReportPostChars", 128);
        }

        set
        {
            this._reg.SetValue<int>("MaxReportPostChars", value);
        }
    }

    /// <summary>
    /// Gets or sets MaxNumberOfAttachments.
    /// </summary>
    public int MaxNumberOfAttachments
    {
      get
      {
        return this._reg.GetValue<int>("MaxNumberOfAttachments", 5);
      }

      set
      {
        this._reg.SetValue<int>("MaxNumberOfAttachments", value);
      }
    }

    // Ederon : 02/17/2009
    /// <summary>
    /// Gets or sets PictureAttachmentDisplayTreshold.
    /// </summary>
    public int PictureAttachmentDisplayTreshold
    {
      get
      {
        return this._reg.GetValue<int>("PictureAttachmentDisplayTreshold", 262144);
      }

      set
      {
        this._reg.SetValue<int>("PictureAttachmentDisplayTreshold", value);
      }
    }

    /// <summary>
    /// Gets or sets ImageAttachmentResizeWidth.
    /// </summary>
    public int ImageAttachmentResizeWidth
    {
      get
      {
        return this._reg.GetValue<int>("ImageAttachmentResizeWidth", 200);
      }

      set
      {
        this._reg.SetValue<int>("ImageAttachmentResizeWidth", value);
      }
    }

    /// <summary>
    /// Gets or sets ImageAttachmentResizeHeight.
    /// </summary>
    public int ImageAttachmentResizeHeight
    {
      get
      {
        return this._reg.GetValue<int>("ImageAttachmentResizeHeight", 200);
      }

      set
      {
        this._reg.SetValue<int>("ImageAttachmentResizeHeight", value);
      }
    }

    /// <summary>
    /// Gets or sets ShoutboxShowMessageCount.
    /// </summary>
    public int ShoutboxShowMessageCount
    {
      get
      {
        return this._reg.GetValue<int>("ShoutboxShowMessageCount", 30);
      }

      set
      {
        this._reg.SetValue<int>("ShoutboxShowMessageCount", value);
      }
    }

    // vzrus
    /// <summary>
    /// Gets or sets ActiveListTime.
    /// </summary>
    public int ActiveListTime
    {
      get
      {
        return this._regBoard.GetValue<int>("ActiveListTime", 5);
      }

      set
      {
        this._regBoard.SetValue<int>("ActiveListTime", value);
      }
    }

    /// <summary>
    /// Gets or sets User Lazy Data Cache Timeout.
    /// </summary>
    public int ActiveUserLazyDataCacheTimeout
    {
      get
      {
          return this._reg.GetValue<int>("ActiveUserLazyDataCacheTimeout", 10);
      }

      set
      {
          this._reg.SetValue<int>("ActiveUserLazyDataCacheTimeout", value);
      }
    }

    /// <summary>
    /// Gets or sets OnlineStatusCacheTimeout.
    /// </summary>
    public int OnlineStatusCacheTimeout
    {
        get
        {
            return this._reg.GetValue<int>("OnlineStatusCacheTimeout", 60000);
        }

        set
        {
            this._reg.SetValue<int>("OnlineStatusCacheTimeout", value);
        }
    }

    /// <summary>
    /// Gets or sets User Name Max Length.
    /// </summary>
    public int UserNameMaxLength
    {
        get
        {
            return this._reg.GetValue<int>("UserNameMaxLength", 50);
        }

        set
        {
            this._reg.SetValue<int>("UserNameMaxLength", value);
        }
    }


    #endregion

    #region boolean settings

    /// <summary>
    /// Gets or sets a value indicating whether EmailVerification.
    /// </summary>
    public bool EmailVerification
    {
      get
      {
        return this._reg.GetValue<bool>("EmailVerification", false);
      }

      set
      {
        this._reg.SetValue<bool>("EmailVerification", value);
      }
    }

    public bool AllowNotificationAllPostsAllTopics
    {
      get
      {
        return this._reg.GetValue<bool>("AllowNotificationAllPostsAllTopics", false);
      }

      set
      {
        this._reg.SetValue<bool>("AllowNotificationAllPostsAllTopics", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether UseFullTextSearch.
    /// </summary>
    public bool UseFullTextSearch
    {
      get
      {
        return this._reg.GetValue<bool>("UseFullTextSearch", false);
      }

      set
      {
        this._reg.SetValue<bool>("UseFullTextSearch", value);
      }
    }

    // vzrus: 10/4/10 SSL registration and login options
    /// <summary>
    /// Gets or sets a value indicating whether Use SSL To Log In.
    /// </summary>
    public bool UseSSLToLogIn
    {
        get
        {
            return this._reg.GetValue<bool>("UseSSLToLogIn", false);
        }

        set
        {
            this._reg.SetValue<bool>("UseSSLToLogIn", value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Use SSL To Register.
    /// </summary>
    public bool UseSSLToRegister
    {
        get
        {
            return this._reg.GetValue<bool>("UseSSLToRegister", false);
        }

        set
        {
            this._reg.SetValue<bool>("UseSSLToRegister", value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowMoved.
    /// </summary>
    public bool ShowMoved
    {
      get
      {
        return this._reg.GetValue<bool>("ShowMoved", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowMoved", value);
      }
    }

    /// <summary>
    /// Gets or sets the value indivicated if an avatar is should be shown in the topic listings.
    /// </summary>
    public bool ShowAvatarsInTopic
    {
      get
      {
        return this._reg.GetValue<bool>("ShowAvatarsInTopic", false);
      }

      set
      {
        this._reg.SetValue<bool>("ShowAvatarsInTopic", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Show Guests In Detailed Active List.
    /// </summary>
    public bool ShowGuestsInDetailedActiveList
    {
        get
        {
            return this._reg.GetValue<bool>("ShowGuestsInDetailedActiveList", false);
        }

        set
        {
            this._reg.SetValue<bool>("ShowGuestsInDetailedActiveList", value);
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether ShowGroups.
    /// </summary>
    public bool ShowGroups
    {
      get
      {
        return this._reg.GetValue<bool>("ShowGroups", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowGroups", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether BlankLinks.
    /// </summary>
    public bool BlankLinks
    {
      get
      {
        return this._reg.GetValue<bool>("BlankLinks", false);
      }

      set
      {
        this._reg.SetValue<bool>("BlankLinks", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowUserTheme.
    /// </summary>
    public bool AllowUserTheme
    {
      get
      {
        return this._reg.GetValue<bool>("AllowUserTheme", false);
      }

      set
      {
        this._reg.SetValue<bool>("AllowUserTheme", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowUserHideHimself.
    /// </summary>
    public bool AllowUserHideHimself
    {
        get
        {
            return this._reg.GetValue<bool>("AllowUserHideHimself", false);
        }

        set
        {
            this._reg.SetValue<bool>("AllowUserHideHimself", value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowUserLanguage.
    /// </summary>
    public bool AllowUserLanguage
    {
      get
      {
        return this._reg.GetValue<bool>("AllowUserLanguage", false);
      }

      set
      {
        this._reg.SetValue<bool>("AllowUserLanguage", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowModeratorsViewIPs.
    /// </summary>
    public bool AllowModeratorsViewIPs
    {
        get
        {
            return this._reg.GetValue<bool>("AllowModeratorsViewIPs", false);
        }

        set
        {
            this._reg.SetValue<bool>("AllowModeratorsViewIPs", value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowPMEmailNotification.
    /// </summary>
    public bool AllowPMEmailNotification
    {
      get
      {
        return this._reg.GetValue<bool>("AllowPMEmailNotification", true);
      }

      set
      {
        this._reg.SetValue<bool>("AllowPMEmailNotification", value);
      }
    }

    /// <summary>
    /// Gets or sets AllowPollChangesAfterFirstVote. A poll creator can't change choices after the first vote.
    /// </summary>
    public bool AllowPollChangesAfterFirstVote
    {
        get
        {
            return this._reg.GetValue<bool>("AllowPollChangesAfterFirstVote", false);
        }

        set
        {
            this._reg.SetValue<bool>("AllowPollChangesAfterFirstVote", value);
        }
    }

    /// <summary>
    /// Gets or sets AllowUsersViewPollVotesBefore. 
    /// </summary>
    public bool AllowUsersViewPollVotesBefore
    {
        get
        {
            return this._reg.GetValue<bool>("AllowViewPollVotesIfNoPollAcces", true);
        }

        set
        {
            this._reg.SetValue<bool>("AllowViewPollVotesIfNoPollAcces", value);
        }
    }

    /// <summary>
    /// Gets or sets AllowGuestsViewPollOptions. 
    /// </summary>
    public bool AllowGuestsViewPollOptions
    {
        get
        {
            return this._reg.GetValue<bool>("AllowGuestsViewPollOptions", true);
        }

        set
        {
            this._reg.SetValue<bool>("AllowGuestsViewPollOptions", value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AvatarUpload.
    /// </summary>
    public bool AvatarUpload
    {
      get
      {
        return this._reg.GetValue<bool>("AvatarUpload", false);
      }

      set
      {
        this._reg.SetValue<bool>("AvatarUpload", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AvatarRemote.
    /// </summary>
    public bool AvatarRemote
    {
      get
      {
        return this._reg.GetValue<bool>("AvatarRemote", false);
      }

      set
      {
        this._reg.SetValue<bool>("AvatarRemote", value);
      }
    }

    // JoeOuts: added 8/17/09
    /// <summary>
    /// Gets or sets a value indicating whether AvatarGravatar.
    /// </summary>
    public bool AvatarGravatar
    {
      get
      {
        return this._reg.GetValue<bool>("AvatarGravatar", false);
      }

      set
      {
        this._reg.SetValue<bool>("AvatarGravatar", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowEmailChange.
    /// </summary>
    public bool AllowEmailChange
    {
      get
      {
        return this._reg.GetValue<bool>("AllowEmailChange", true);
      }

      set
      {
        this._reg.SetValue<bool>("AllowEmailChange", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowPasswordChange.
    /// </summary>
    public bool AllowPasswordChange
    {
      get
      {
        return this._reg.GetValue<bool>("AllowPasswordChange", true);
      }

      set
      {
        this._reg.SetValue<bool>("AllowPasswordChange", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether UseFileTable.
    /// </summary>
    public bool UseFileTable
    {
      get
      {
        return this._reg.GetValue<bool>("UseFileTable", false);
      }

      set
      {
        this._reg.SetValue<bool>("UseFileTable", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowRSSLink.
    /// </summary>
    public bool ShowRSSLink
    {
      get
      {
        return this._reg.GetValue<bool>("ShowRSSLink", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowRSSLink", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowPageGenerationTime.
    /// </summary>
    public bool ShowPageGenerationTime
    {
      get
      {
        return this._reg.GetValue<bool>("ShowPageGenerationTime", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowPageGenerationTime", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowYAFVersion.
    /// </summary>
    public bool ShowYAFVersion
    {
      get
      {
        return this._reg.GetValue<bool>("ShowYAFVersion", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowYAFVersion", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowForumJump.
    /// </summary>
    public bool ShowForumJump
    {
      get
      {
        return this._reg.GetValue<bool>("ShowForumJump", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowForumJump", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowPrivateMessages.
    /// </summary>
    public bool AllowPrivateMessages
    {
      get
      {
        return this._reg.GetValue<bool>("AllowPrivateMessages", true);
      }

      set
      {
        this._reg.SetValue<bool>("AllowPrivateMessages", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowEmailSending.
    /// </summary>
    public bool AllowEmailSending
    {
      get
      {
        return this._reg.GetValue<bool>("AllowEmailSending", true);
      }

      set
      {
        this._reg.SetValue<bool>("AllowEmailSending", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowSignatures.
    /// </summary>
    public bool AllowSignatures
    {
      get
      {
        return this._reg.GetValue<bool>("AllowSignatures", true);
      }

      set
      {
        this._reg.SetValue<bool>("AllowSignatures", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ExternalSearchInNewWindow.
    /// </summary>
    public bool ExternalSearchInNewWindow
    {
        get
        {
            return this._reg.GetValue<bool>("ExternalSearchInNewWindow", false);
        }

        set
        {
            this._reg.SetValue<bool>("ExternalSearchInNewWindow", value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether RemoveNestedQuotes.
    /// </summary>
    public bool RemoveNestedQuotes
    {
      get
      {
        return this._reg.GetValue<bool>("RemoveNestedQuotes", false);
      }

      set
      {
        this._reg.SetValue<bool>("RemoveNestedQuotes", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether DateFormatFromLanguage.
    /// </summary>
    public bool DateFormatFromLanguage
    {
      get
      {
        return this._reg.GetValue<bool>("DateFormatFromLanguage", false);
      }

      set
      {
        this._reg.SetValue<bool>("DateFormatFromLanguage", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether DisableRegistrations.
    /// </summary>
    public bool DisableRegistrations
    {
      get
      {
        return this._reg.GetValue<bool>("DisableRegistrations", false);
      }

      set
      {
        this._reg.SetValue<bool>("DisableRegistrations", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether CreateNntpUsers.
    /// </summary>
    public bool CreateNntpUsers
    {
      get
      {
        return this._reg.GetValue<bool>("CreateNntpUsers", false);
      }

      set
      {
        this._reg.SetValue<bool>("CreateNntpUsers", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowGroupsProfile.
    /// </summary>
    public bool ShowGroupsProfile
    {
      get
      {
        return this._reg.GetValue<bool>("ShowGroupsProfile", false);
      }

      set
      {
        this._reg.SetValue<bool>("ShowGroupsProfile", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether PollVoteTiedToIP.
    /// </summary>
    public bool PollVoteTiedToIP
    {
      get
      {
        return this._reg.GetValue<bool>("PollVoteTiedToIP", true);
      }

      set
      {
        this._reg.SetValue<bool>("PollVoteTiedToIP", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowAdsToSignedInUsers.
    /// </summary>
    public bool ShowAdsToSignedInUsers
    {
      get
      {
        return this._reg.GetValue<bool>("ShowAdsToSignedInUsers", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowAdsToSignedInUsers", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether DisplayPoints.
    /// </summary>
    public bool DisplayPoints
    {
      get
      {
        return this._reg.GetValue<bool>("DisplayPoints", false);
      }

      set
      {
        this._reg.SetValue<bool>("DisplayPoints", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowQuickAnswer.
    /// </summary>
    public bool ShowQuickAnswer
    {
      get
      {
        return this._reg.GetValue<bool>("ShowQuickAnswer", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowQuickAnswer", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowDeletedMessages.
    /// </summary>
    public bool ShowDeletedMessages
    {
      get
      {
        return this._reg.GetValue<bool>("ShowDeletedMessages", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowDeletedMessages", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowDeletedMessagesToAll.
    /// </summary>
    public bool ShowDeletedMessagesToAll
    {
      get
      {
        return this._reg.GetValue<bool>("ShowDeletedMessagesToAll", false);
      }

      set
      {
        this._reg.SetValue<bool>("ShowDeletedMessagesToAll", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowModeratorList.
    /// </summary>
    public bool ShowModeratorList
    {
      get
      {
        return this._reg.GetValue<bool>("ShowModeratorList", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowModeratorList", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether EnableCaptchaForPost.
    /// </summary>
    public bool EnableCaptchaForPost
    {
      get
      {
        return this._reg.GetValue<bool>("EnableCaptchaForPost", false);
      }

      set
      {
        this._reg.SetValue<bool>("EnableCaptchaForPost", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether EnableCaptchaForRegister.
    /// </summary>
    public bool EnableCaptchaForRegister
    {
      get
      {
        return this._reg.GetValue<bool>("EnableCaptchaForRegister", false);
      }

      set
      {
        this._reg.SetValue<bool>("EnableCaptchaForRegister", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether EnableCaptchaForGuests.
    /// </summary>
    public bool EnableCaptchaForGuests
    {
      get
      {
        return this._reg.GetValue<bool>("EnableCaptchaForGuests", true);
      }

      set
      {
        this._reg.SetValue<bool>("EnableCaptchaForGuests", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether UseNoFollowLinks.
    /// </summary>
    public bool UseNoFollowLinks
    {
      get
      {
        return this._reg.GetValue<bool>("UseNoFollowLinks", true);
      }

      set
      {
        this._reg.SetValue<bool>("UseNoFollowLinks", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether DoUrlReferrerSecurityCheck.
    /// </summary>
    public bool DoUrlReferrerSecurityCheck
    {
      get
      {
        return this._reg.GetValue<bool>("DoUrlReferrerSecurityCheck", false);
      }

      set
      {
        this._reg.SetValue<bool>("DoUrlReferrerSecurityCheck", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether EnableImageAttachmentResize.
    /// </summary>
    public bool EnableImageAttachmentResize
    {
      get
      {
        return this._reg.GetValue<bool>("EnableImageAttachmentResize", true);
      }

      set
      {
        this._reg.SetValue<bool>("EnableImageAttachmentResize", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowShoutbox.
    /// </summary>
    public bool ShowShoutbox
    {
      get
      {
        return this._reg.GetValue<bool>("ShowShoutbox", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowShoutbox", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Show smiles in Shoutbox.
    /// </summary>
    public bool ShowShoutboxSmiles
    {
        get
        {
            return this._reg.GetValue<bool>("ShowShoutboxSmiles", true);
        }

        set
        {
            this._reg.SetValue<bool>("ShowShoutboxSmiles", value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowUserInfoCaching.
    /// </summary>
    public bool AllowUserInfoCaching
    {
      get
      {
        return this._reg.GetValue<bool>("AllowUserInfoCaching", true);
      }

      set
      {
        this._reg.SetValue<bool>("AllowUserInfoCaching", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether display No-Count Forums In ActiveDiscussions.
    /// </summary>
    public bool NoCountForumsInActiveDiscussions
    {
      get
      {
          return this._reg.GetValue<bool>("NoCountForumsInActiveDiscussions", true);
      }

      set
      {
          this._reg.SetValue<bool>("NoCountForumsInActiveDiscussions", value);
      }
    }
      
    /// <summary>
    /// Gets or sets a value indicating whether UseStyledNicks.
    /// </summary>
    public bool UseStyledNicks
    {
      get
      {
        return this._reg.GetValue<bool>("UseStyledNicks", false);
      }

      set
      {
        this._reg.SetValue<bool>("UseStyledNicks", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowUserOnlineStatus.
    /// </summary>
    public bool ShowUserOnlineStatus
    {
      get
      {
        return this._reg.GetValue<bool>("ShowUserOnlineStatus", false);
      }

      set
      {
        this._reg.SetValue<bool>("ShowUserOnlineStatus", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowThanksDate.
    /// </summary>
    public bool ShowThanksDate
    {
      get
      {
        return this._reg.GetValue<bool>("ShowThanksDate", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowThanksDate", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether EnableThanksMod.
    /// </summary>
    public bool EnableThanksMod
    {
      get
      {
        return this._reg.GetValue<bool>("EnableThanksMod", true);
      }

      set
      {
        this._reg.SetValue<bool>("EnableThanksMod", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether EnableBuddyList.
    /// </summary>
    public bool EnableBuddyList
    {
        get
        {
            return this._reg.GetValue<bool>("EnableBuddyList", true);
        }

        set
        {
            this._reg.SetValue<bool>("EnableBuddyList", value);
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether EnableDNACalendar.
    /// This is temporary feature to disable calendar 
    /// if it causes troubles with different cultures
    /// </summary>
    public bool EnableDNACalendar
    {
        get
        {
            return this._reg.GetValue<bool>("EnableDNACalendar", true);
        }

        set
        {
            this._reg.SetValue<bool>("EnableDNACalendar", value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether EnableActiveLocationErrorsLog. A temporary debug setting.
    /// </summary>
    public bool EnableActiveLocationErrorsLog
    {
        get
        {
            return this._reg.GetValue<bool>("EnableActiveLocationErrorsLog", false);
        }

        set
        {
            this._reg.SetValue<bool>("EnableActiveLocationErrorsLog", value);
        }
    }

    /// <summary>
    /// Log UserAgent strings unhandled by YAF.
    /// </summary>
    public bool UserAgentBadLog
    {
        get
        {
            return this._reg.GetValue<bool>("UserAgentBadLog", false);
        }

        set
        {
            this._reg.SetValue<bool>("UserAgentBadLog", value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether EnableAlbum.
    /// </summary>
    public bool EnableAlbum
    {
        get
        {
            return this._reg.GetValue<bool>("EnableAlbum", true);
        }

        set
        {
            this._reg.SetValue<bool>("EnableAlbum", value);
        }
    }

    /// <summary>
    /// Gets or sets AlbumsMax.
    /// </summary>
    public int AlbumsMax
    {
        get
        {
            return this._regBoard.GetValue<int>("AlbumsMax", 10);
        }

        set
        {
            this._regBoard.SetValue<int>("AlbumsMax", value);
        }
    }

    /// <summary>
    /// Gets or sets AlbumImagesNumberMax.
    /// </summary>
    public int AlbumImagesNumberMax
    {
        get
        {
            return this._regBoard.GetValue<int>("AlbumImagesNumberMax", 50);
        }

        set
        {
            this._regBoard.SetValue<int>("AlbumImagesNumberMax", value);
        }
    }

    /// <summary>
    /// Gets or sets AlbumImagesSizeMax.
    /// </summary>
    public int AlbumImagesSizeMax
    {
        get
        {
            return this._regBoard.GetValue<int>("AlbumImagesSizeMax", 1048576);
        }

        set
        {
            this._regBoard.SetValue<int>("AlbumImagesSizeMax", value);
        }
    }

    /// <summary>
    /// Gets or sets AlbumsPerPage.
    /// </summary>
    public int AlbumsPerPage
    {
        get
        {
            return this._regBoard.GetValue<int>("AlbumsPerPage", 6);
        }

        set
        {
            this._regBoard.SetValue<int>("AlbumsPerPage", value);
        }
    }

    /// <summary>
    /// Gets or sets AlbumImagesPerPage.
    /// </summary>
    public int AlbumImagesPerPage
    {
        get
        {
            return this._regBoard.GetValue<int>("AlbumImagesPerPage", 10);
        }

        set
        {
            this._regBoard.SetValue<int>("AlbumImagesPerPage", value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to AddDynamicPageMetaTags.
    /// </summary>
    public bool AddDynamicPageMetaTags
    {
      get
      {
        return this._reg.GetValue<bool>("AddDynamicPageMetaTags", true);
      }

      set
      {
        this._reg.SetValue<bool>("AddDynamicPageMetaTags", value);
      }
    }

    public bool AllowDisplayNameModification
    {
      get
      {
        return this._reg.GetValue<bool>("AllowDisplayNameModification", true);
      }

      set
      {
        this._reg.SetValue<bool>("AllowDisplayNameModification", value);
      }
    }

    #endregion

    #region string settings

    /// <summary>
    /// Gets or sets IPInfo page Url.
    /// </summary>
    public string IPInfoPageURL
    {
        get
        {
            return this._reg.GetValue<string>("IPInfoPageURL", "http://www.dnsstuff.com/tools/whois.ch?ip={0}");
        }

        set
        {
            this._reg.SetValue<string>("IPInfoPageURL", value);
        }
    }

    /// <summary>
    /// Gets or sets ForumEmail.
    /// </summary>
    public string ForumEmail
    {
      get
      {
        return this._reg.GetValue<string>("ForumEmail", string.Empty);
      }

      set
      {
        this._reg.SetValue<string>("ForumEmail", value);
      }
    }

    /// <summary>
    /// Gets or sets RecaptchaPublicKey .
    /// </summary>
    public string RecaptchaPublicKey
    {
        get
        {
            return this._reg.GetValue<string>("RecaptchaPublicKey",  string.Empty);
        }

        set
        {
            this._reg.SetValue<string>("RecaptchaPublicKey", value);
        }
    }

    /// <summary>
    /// Gets or sets RecaptchaPrivateKey.
    /// </summary>
    public string RecaptchaPrivateKey
    {
        get
        {
            return this._reg.GetValue<string>("RecaptchaPrivateKey", string.Empty);
        }

        set
        {
            this._reg.SetValue<string>("RecaptchaPrivateKey", value);
        }
    }

    /// <summary>
    /// Gets or sets SearchEngine1.
    /// </summary>
    public string SearchEngine1
    {
        get
        {
            return this._reg.GetValue<string>("SearchEngine1", "http://google.com/search?as_q={Word}&hl={Language}&num={ResultsPerPage}&btnG={ButtonName}&as_epq={Word}&as_oq={Word}&as_eq={Word}&lr=&cr=&as_ft=i&as_filetype=&as_qdr=&as_occt=&as_dt=i&as_sitesearch={Site}&as_rights=&safe=off");
        }

        set
        {
            this._reg.SetValue<string>("SearchEngine1", value);
        }
    }

    /// <summary>
    /// Gets or sets SearchEngine2.
    /// </summary>
    public string SearchEngine2
    {
        get
        {

            return this._reg.GetValue<string>("SearchEngine2", "http://yandex.ru/yandsearch?date=all&text=&site={Site}&rstr=&zone=all&wordforms=&lang={Language}&within=&from_day=&from_month=&from_year=&to_day=&to_month=&to_year=&mime=&numdoc={ResultsPerPage}&lr=");
        }

        set
        {
            this._reg.SetValue<string>("SearchEngine2", value);
        }
    }

    /// <summary>
    /// Gets or sets SearchEngine1Parameters.
    /// </summary>
    public string SearchEngine1Parameters
    {
        get
        {
            return this._reg.GetValue<string>("SearchEngine1Parameters", "Google^?^&^+^;^AnyWord:as_oq={Word}^AllWords:as_q={Word}^ExactFrase:as_epq={Word}^WithoutWords:as_eq={Word}");
        }

        set
        {
            this._reg.SetValue<string>("SearchEngine1Parameters", value);
        }
    }

    /// <summary>
    /// Gets or sets SearchEngine2Parameters.
    /// </summary>
    public string SearchEngine2Parameters
    {
        get
        {
            return this._reg.GetValue<string>("SearchEngine2Parameters", "Yandex^?^&^+^;^AnyWord:text={Word}/wordforms=any^AllWords:text={Word}/wordforms=all^ExactFrase:text={Word}/wordforms=exact^WithoutWords:text=~~{Word}");
        }

        set
        {
            this._reg.SetValue<string>("SearchEngine2Parameters", value);
        }
    }

    // JoeOuts: added 8/17/09
    /// <summary>
    /// Gets or sets GravatarRating.
    /// </summary>
    public string GravatarRating
    {
      get
      {
        return this._reg.GetValue<string>("GravatarRating", "G");
      }

      set
      {
        this._reg.SetValue<string>("GravatarRating", value);
      }
    }

    /// <summary>
    /// Gets or sets AcceptedHTML.
    /// </summary>
    public string AcceptedHTML
    {
      get
      {
        return this._reg.GetValue<string>("AcceptedHTML", "br,hr,b,i,u,a,div,ol,ul,li,blockquote,img,span,p,em,strong,font,pre,h1,h2,h3,h4,h5,h6,address");
      }

      set
      {
        this._reg.SetValue<string>("AcceptedHTML", value.ToLower());
      }
    }

    /// <summary>
    /// Gets or sets AdPost.
    /// </summary>
    public string AdPost
    {
      get
      {
        return this._reg.GetValue<string>("AdPost", null);
      }

      set
      {
        this._reg.SetValue<string>("AdPost", value);
      }
    }

    /// <summary>
    /// Gets or sets CustomLoginRedirectUrl.
    /// </summary>
    public string CustomLoginRedirectUrl
    {
      get
      {
        return this._reg.GetValue<string>("CustomLoginRedirectUrl", null);
      }

      set
      {
        this._reg.SetValue<string>("CustomLoginRedirectUrl", value);
      }
    }

    /// <summary>
    /// Gets or sets WebServiceToken.
    /// </summary>
    public string WebServiceToken
    {
      get
      {
        return this._reg.GetValue<string>("WebServiceToken", Guid.NewGuid().ToString());
      }

      set
      {
        this._reg.SetValue<string>("WebServiceToken", value);
      }
    }

    /// <summary>
    /// Gets or sets SearchStringPattern.
    /// </summary>
    public string SearchStringPattern
    {
      get
      {
        return this._reg.GetValue<string>("SearchStringPattern", ".*");
      }

      set
      {
        this._reg.SetValue<string>("SearchStringPattern", value);
      }
    }

    /* Ederon : 6/16/2007 */

    /// <summary>
    /// Gets or sets a value indicating whether DisplayJoinDate.
    /// </summary>
    public bool DisplayJoinDate
    {
      get
      {
        return this._reg.GetValue<bool>("DisplayJoinDate", true);
      }

      set
      {
        this._reg.SetValue<bool>("DisplayJoinDate", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowBrowsingUsers.
    /// </summary>
    public bool ShowBrowsingUsers
    {
      get
      {
        return this._reg.GetValue<bool>("ShowBrowsingUsers", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowBrowsingUsers", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowMedals.
    /// </summary>
    public bool ShowMedals
    {
      get
      {
        return this._reg.GetValue<bool>("ShowMedals", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowMedals", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowPostToBlog.
    /// </summary>
    public bool AllowPostToBlog
    {
      get
      {
        return this._reg.GetValue<bool>("AllowPostToBlog", false);
      }

      set
      {
        this._reg.SetValue<bool>("AllowPostToBlog", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowReportPosts.
    /// </summary>
    public bool AllowReportPosts
    {
        get
        {
            return this._reg.GetValue<bool>("AllowReportPosts", true);
        }

        set
        {
            this._reg.SetValue<bool>("AllowReportPosts", value);
        }
    }
    /* Ederon : 8/29/2007 */

    /// <summary>
    /// Gets or sets a value indicating whether AllowEmailTopic.
    /// </summary>
    public bool AllowEmailTopic
    {
      get
      {
        return this._reg.GetValue<bool>("AllowEmailTopic", true);
      }

      set
      {
        this._reg.SetValue<bool>("AllowEmailTopic", value);
      }
    }

    /* Ederon : 12/9/2007 */

    /// <summary>
    /// Gets or sets a value indicating whether RequireLogin.
    /// </summary>
    public bool RequireLogin
    {
      get
      {
        return this._reg.GetValue<bool>("RequireLogin", false);
      }

      set
      {
        this._reg.SetValue<bool>("RequireLogin", value);
      }
    }

    /* Ederon : 12/14/2007 */

    /// <summary>
    /// Gets or sets a value indicating whether ShowActiveDiscussions.
    /// </summary>
    public bool ShowActiveDiscussions
    {
      get
      {
        return this._reg.GetValue<bool>("ShowActiveDiscussions", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowActiveDiscussions", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowForumStatistics.
    /// </summary>
    public bool ShowForumStatistics
    {
      get
      {
        return this._reg.GetValue<bool>("ShowForumStatistics", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowForumStatistics", value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowRulesForRegistration.
    /// </summary>
    public bool ShowRulesForRegistration
    {
      get
      {
        return this._reg.GetValue<bool>("ShowRulesForRegistration", true);
      }

      set
      {
        this._reg.SetValue<bool>("ShowRulesForRegistration", value);
      }
    }

    /* 6/16/2007 */
    /* Ederon : 7/14/2007 */

    /// <summary>
    /// Gets or sets UserBox.
    /// </summary>
    public string UserBox
    {
      get
      {
        return this._reg.GetValue<string>("UserBox", Constants.UserBox.DisplayTemplateDefault);
      }

      set
      {
        this._reg.SetValue<string>("UserBox", value);
      }
    }

    /// <summary>
    /// Gets or sets UserBoxAvatar.
    /// </summary>
    public string UserBoxAvatar
    {
      get
      {
        return this._reg.GetValue<string>("UserBoxAvatar", @"<div class=""section"">{0}</div><br clear=""all"" />");
      }

      set
      {
        this._reg.SetValue<string>("UserBoxAvatar", value);
      }
    }

    /// <summary>
    /// Gets or sets UserBoxMedals.
    /// </summary>
    public string UserBoxMedals
    {
      get
      {
        return this._reg.GetValue<string>("UserBoxMedals", @"<div class=""section medals"">{0} {1}{2}</div><br clear=""all"" />");
      }

      set
      {
        this._reg.SetValue<string>("UserBoxMedals", value);
      }
    }

    /// <summary>
    /// Gets or sets UserBoxRankImage.
    /// </summary>
    public string UserBoxRankImage
    {
      get
      {
        return this._reg.GetValue<string>("UserBoxRankImage", "{0}<br clear=\"all\" />");
      }

      set
      {
        this._reg.SetValue<string>("UserBoxRankImage", value);
      }
    }

    /// <summary>
    /// Gets or sets UserBoxRank.
    /// </summary>
    public string UserBoxRank
    {
      get
      {
        return this._reg.GetValue<string>("UserBoxRank", "{0}: {1}<br clear=\"all\" />");
      }

      set
      {
        this._reg.SetValue<string>("UserBoxRank", value);
      }
    }

    /// <summary>
    /// Gets or sets UserBoxGroups.
    /// </summary>
    public string UserBoxGroups
    {
      get
      {
        return this._reg.GetValue<string>("UserBoxGroups", "{0}: {1}<br clear=\"all\" />");
      }

      set
      {
        this._reg.SetValue<string>("UserBoxGroups", value);
      }
    }

    /// <summary>
    /// Gets or sets UserBoxJoinDate.
    /// </summary>
    public string UserBoxJoinDate
    {
      get
      {
        return this._reg.GetValue<string>("UserBoxJoinDate", "{0}: {1}<br />");
      }

      set
      {
        this._reg.SetValue<string>("UserBoxJoinDate", value);
      }
    }
    public string UserBoxGender
    {
        get
        {
            return this._reg.GetValue<string>("UserBoxGender", "{0}<br />");
        }

        set
        {
            this._reg.SetValue<string>("UserBoxGender", value);
        }
    }


    /// <summary>
    /// Gets or sets UserBoxPosts.
    /// </summary>
    public string UserBoxPosts
    {
      get
      {
        return this._reg.GetValue<string>("UserBoxPosts", "{0}: {1:N0}<br />");
      }

      set
      {
        this._reg.SetValue<string>("UserBoxPosts", value);
      }
    }

    /// <summary>
    /// Gets or sets UserBoxPoints.
    /// </summary>
    public string UserBoxPoints
    {
      get
      {
        return this._reg.GetValue<string>("UserBoxPoints", "{0}: {1:N0}<br />");
      }

      set
      {
        this._reg.SetValue<string>("UserBoxPoints", value);
      }
    }

    /// <summary>
    /// Gets or sets UserBoxLocation.
    /// </summary>
    public string UserBoxLocation
    {
      get
      {
        return this._reg.GetValue<string>("UserBoxLocation", "{0}: {1}<br />");
      }

      set
      {
        this._reg.SetValue<string>("UserBoxLocation", value);
      }
    }

    /* 7/14/2007 */

    /// <summary>
    /// Gets or sets UserBoxThanksFrom.
    /// </summary>
    public string UserBoxThanksFrom
    {
      get
      {
        return this._reg.GetValue<string>("UserBoxThanksFrom", "{0}<br />");
      }

      set
      {
        this._reg.SetValue<string>("UserBoxThanksFrom", value);
      }
    }

    /// <summary>
    /// Gets or sets UserBoxThanksTo.
    /// </summary>
    public string UserBoxThanksTo
    {
      get
      {
        return this._reg.GetValue<string>("UserBoxThanksTo", "{0}<br />");
      }

      set
      {
        this._reg.SetValue<string>("UserBoxThanksTo", value);
      }
    }

    public string LastDigestSend
    {
      get
      {
        return this._regBoard.GetValue<string>("LastDigestSend", null);
      }
      set
      {
        this._regBoard.SetValue<string>("LastDigestSend", value);
      }
    }

    #endregion

    #region Nested type: YafLegacyBoardSettings

    /// <summary>
    /// The yaf legacy board settings.
    /// </summary>
    public class YafLegacyBoardSettings
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="YafLegacyBoardSettings"/> class.
      /// </summary>
      public YafLegacyBoardSettings()
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="YafLegacyBoardSettings"/> class.
      /// </summary>
      /// <param name="boardName">
      /// The board name.
      /// </param>
      /// <param name="sqlVersion">
      /// The sql version.
      /// </param>
      /// <param name="allowThreaded">
      /// The allow threaded.
      /// </param>
      /// <param name="membershipAppName">
      /// The membership app name.
      /// </param>
      /// <param name="rolesAppName">
      /// The roles app name.
      /// </param>
      public YafLegacyBoardSettings(string boardName, string sqlVersion, bool allowThreaded, string membershipAppName, string rolesAppName)
        : this()
      {
        BoardName = boardName;
        SqlVersion = sqlVersion;
        AllowThreaded = allowThreaded;
        MembershipAppName = membershipAppName;
        RolesAppName = rolesAppName;
      }

      /// <summary>
      /// Gets or sets BoardName.
      /// </summary>
      public string BoardName
      {
        get;
        set;
      }

      /// <summary>
      /// Gets or sets SqlVersion.
      /// </summary>
      public string SqlVersion
      {
        get;
        set;
      }

      /// <summary>
      /// Gets or sets a value indicating whether AllowThreaded.
      /// </summary>
      public bool AllowThreaded
      {
        get;
        set;
      }

      /// <summary>
      /// Gets or sets MembershipAppName.
      /// </summary>
      public string MembershipAppName
      {
        get;
        set;
      }

      /// <summary>
      /// Gets or sets RolesAppName.
      /// </summary>
      public string RolesAppName
      {
        get;
        set;
      }
    }

    #endregion
  }
}