/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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
    using YAF.Types;
    using YAF.Types.Constants;

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

        /// <summary>
        /// Gets Name.
        /// individual board settings
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
                return this._reg.GetValue("AllowThemedLogo", false);
            }

            set
            {
                this._reg.SetValue("AllowThemedLogo", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether User genger icons in user box are enabled.
        /// </summary>
        public bool AllowGenderInUserBox
        {
            get
            {
                return this._reg.GetValue("AllowGenderInUserBox", true);
            }

            set
            {
                this._reg.SetValue("AllowGenderInUserBox", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable Display Name.
        /// </summary>
        public bool EnableDisplayName
        {
            get
            {
                return this._reg.GetValue("EnableDisplayName", false);
            }

            set
            {
                this._reg.SetValue("EnableDisplayName", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable JqueryUIThemeCDNHosted.
        /// </summary>
        public bool JqueryUIThemeCDNHosted
        {
            get
            {
                return this._reg.GetValue("JqueryUIThemeCDNHosted", true);
            }

            set
            {
                this._reg.SetValue("JqueryUIThemeCDNHosted", value);
            }
        }

        /// <summary>
        /// Gets MaxUsers.
        /// </summary>
        public int MaxUsers
        {
            get
            {
                return this._regBoard.GetValue("MaxUsers", 1);
            }
        }

        /// <summary>
        /// Gets MaxUsersWhen.
        /// </summary>
        public DateTime MaxUsersWhen
        {
            get
            {
                return this._regBoard.GetValue("MaxUsersWhen", DateTime.UtcNow);
            }
        }

        /// <summary>
        /// Gets or sets Theme.
        /// </summary>
        public string MobileTheme
        {
            get
            {
                return this._regBoard.GetValue("MobileTheme", string.Empty);
            }

            set
            {
                this._regBoard.SetValue("MobileTheme", value);
            }
        }

        /// <summary>
        /// Gets or sets Jquery UI Theme.
        /// </summary>
        public string JqueryUITheme
        {
            get
            {
                return this._regBoard.GetValue("JqueryUITheme", "smoothness");
            }

            set
            {
                this._regBoard.SetValue("JqueryUITheme", value);
            }
        }

        /// <summary>
        /// Gets or sets Theme.
        /// </summary>
        public string Theme
        {
            get
            {
                return this._regBoard.GetValue("Theme", "cleanSlate.xml");
            }

            set
            {
                this._regBoard.SetValue("Theme", value);
            }
        }

        /// <summary>
        /// Gets or sets Language.
        /// </summary>
        public string Language
        {
            get
            {
                return this._regBoard.GetValue("Language", "english.xml");
            }

            set
            {
                this._regBoard.SetValue("Language", value);
            }
        }

        /// <summary>
        /// Gets or sets Culture.
        /// </summary>
        public string Culture
        {
            get
            {
                return this._regBoard.GetValue("Culture", "en-US");
            }

            set
            {
                this._regBoard.SetValue("Culture", value);
            }
        }

      /// <summary>
      /// Gets or sets Default Notification Setting.
      /// </summary>
      public UserNotificationSetting DefaultNotificationSetting
        {
            get
            {
                return this._regBoard.GetValue("DefaultNotificationSetting", UserNotificationSetting.TopicsIPostToOrSubscribeTo);
            }

            set
            {
                this._regBoard.SetValue("DefaultNotificationSetting", value);
            }
        }

      /// <summary>
      /// Gets or sets a value indicating whether Default Send Digest Email.
      /// </summary>
      public bool DefaultSendDigestEmail
        {
            get
            {
                return this._regBoard.GetValue("DefaultSendDigestEmail", false);
            }

            set
            {
                this._regBoard.SetValue("DefaultSendDigestEmail", value);
            }
        }

      /// <summary>
      /// Gets or sets a value indicating whether Allow Digest Email.
      /// </summary>
      public bool AllowDigestEmail
        {
            get
            {
                return this._regBoard.GetValue("AllowDigestEmail", false);
            }

            set
            {
                this._regBoard.SetValue("AllowDigestEmail", value);
            }
        }

        /// <summary>
        /// Gets or sets ShowTopicsDefault.
        /// </summary>
        public int ShowTopicsDefault
        {
            get
            {
                return this._regBoard.GetValue("ShowTopicsDefault", 0);
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
                return this._regBoard.GetValue("FileExtensionAreAllowed", true);
            }

            set
            {
                this._regBoard.SetValue("FileExtensionAreAllowed", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Recapture can have Multiple Instances.
        /// </summary>
        public bool RecaptureMultipleInstances
        {
            get
            {
                return this._regBoard.GetValue("RecaptureMultipleInstances", true);
            }

            set
            {
                this._regBoard.SetValue("RecaptureMultipleInstances", value);
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

      /// <summary>
      /// Gets or sets Copyright Removal Domain Key.
      /// </summary>
      public string CopyrightRemovalDomainKey
        {
          get
          {
              return this._regBoard.GetValue<string>("CopyrightRemovalDomainKey", null);
          }

          set
          {
              this._regBoard.SetValue<string>("CopyrightRemovalDomainKey", value);
          }
        }

      /// <summary>
      /// Gets or sets a value indicating whether Email Moderators On Moderated Post.
      /// </summary>
      public bool EmailModeratorsOnModeratedPost
        {
            get
            {
                return this._regBoard.GetValue("EmailModeratorsOnModeratedPost", true);
            }

            set
            {
                this._regBoard.SetValue("EmailModeratorsOnModeratedPost", value);
            }
        }

      /// <summary>
      /// Gets or sets a value indicating whether to enable Check For Spam Content.
      /// </summary>
      public bool CheckForSpamContent
      {
          get
          {
              return this._reg.GetValue("CheckForSpamContent", true);
          }

          set
          {
              this._reg.SetValue("CheckForSpamContent", value);
          }
      }

        /// <summary>
        /// Gets SQLVersion.
        /// NOTE : didn't know where else to put this :)
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
        /// Gets or sets a value indicating whether Show Share Topic To.
        /// </summary>
        public int ShowShareTopicTo
        {
            get
            {
                return this._reg.GetValue("ShowShareTopicTo", 1);
            }

            set
            {
                this._reg.SetValue<int>("ShowShareTopicTo", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Enable Retweet Message.
        /// </summary>
        public int ShowRetweetMessageTo
        {
            get
            {
                return this._reg.GetValue("ShowRetweetMessageTo", 1);
            }

            set
            {
                this._reg.SetValue<int>("ShowRetweetMessageTo", value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether Show Team Page To or not.
        /// </summary>
        public int ShowTeamTo
        {
            get
            {
                return this._reg.GetValue("ShowTeamTo", 0);
            }

            set
            {
                this._reg.SetValue<int>("ShowTeamTo", value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether Show Help Page To or not.
        /// </summary>
        public int ShowHelpTo
        {
            get
            {
                return this._reg.GetValue("ShowHelpTo", 2);
            }

            set
            {
                this._reg.SetValue<int>("ShowHelpTo", value);
            }
        }

        /// <summary>
        /// Gets or sets ServerTimeCorrection.
        /// </summary>
        public int ServerTimeCorrection
        {
            get
            {
                return this._reg.GetValue("ServerTimeCorrection", 0);
            }

            set
            {
                this._reg.SetValue<int>("ServerTimeCorrection", value);
            }
        }

        /// <summary>
        /// Gets or sets MemberListPageSize in minutes.
        /// </summary>
        public int MemberListPageSize
        {
            get
            {
                return this._reg.GetValue("MemberListPageSize", 20);
            }

            set
            {
                this._reg.SetValue<int>("MemberListPageSize", value);
            }
        }

        /// <summary>
        /// Gets or sets PostLatestFeedAccess.
        /// </summary>
        public int PostLatestFeedAccess
        {
            get
            {
                return this._reg.GetValue("PostLatestFeedAccess", 1);
            }

            set
            {
                this._reg.SetValue<int>("PostLatestFeedAccess", value);
            }
        }

        /// <summary>
        /// Gets or sets PostsFeedAccess.
        /// </summary>
        public int PostsFeedAccess
        {
            get
            {
                return this._reg.GetValue("PostsFeedAccess", 1);
            }

            set
            {
                this._reg.SetValue<int>("PostsFeedAccess", value);
            }
        }

        /// <summary>
        /// Gets or sets DigestSendEveryXHours.
        /// </summary>
        public int DigestSendEveryXHours
        {
          get
          {
            return this._reg.GetValue("DigestSendEveryXHours", 24);
          }

          set
          {
            this._reg.SetValue<int>("DigestSendEveryXHours", value);
          }
        }

        /// <summary>
        /// Gets or sets TopicsFeedAccess.
        /// </summary>
        public int TopicsFeedAccess
        {
            get
            {
                return this._reg.GetValue("TopicsFeedAccess", 1);
            }

            set
            {
                this._reg.SetValue<int>("TopicsFeedAccess", value);
            }
        }

        /// <summary>
        /// Gets or sets ForumFeedAccess.
        /// </summary>
        public int ForumFeedAccess
        {
            get
            {
                return this._reg.GetValue("ForumFeedAccess", 1);
            }

            set
            {
                this._reg.SetValue<int>("ForumFeedAccess", value);
            }
        }

        /// <summary>
        /// Gets or sets ActiveTopicFeedAccess.
        /// </summary>
        public int ActiveTopicFeedAccess
        {
            get
            {
                return this._reg.GetValue("ActiveTopicFeedAccess", 1);
            }

            set
            {
                this._reg.SetValue<int>("ActiveTopicFeedAccess", value);
            }
        }

        /// <summary>
        /// Gets or sets FavoriteTopicFeedAccess.
        /// </summary>
        public int FavoriteTopicFeedAccess
        {
            get
            {
                return this._reg.GetValue("FavoriteTopicFeedAccess", 1);
            }

            set
            {
                this._reg.SetValue<int>("FavoriteTopicFeedAccess", value);
            }
        }

        /// <summary>
        /// Gets or sets AvatarWidth.
        /// </summary>
        public int AvatarWidth
        {
            get
            {
                return this._reg.GetValue("AvatarWidth", 50);
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
                return this._reg.GetValue("AvatarHeight", 80);
            }

            set
            {
                this._reg.SetValue<int>("AvatarHeight", value);
            }
        }

        /// <summary>
        /// Gets or sets AllowCreateTopicsSameName.
        /// </summary>
        public int AllowCreateTopicsSameName
        {
            get
            {
                return this._reg.GetValue("AllowCreateTopicsSameName", 0);
            }

            set
            {
                this._reg.SetValue<int>("AllowCreateTopicsSameName", value);
            }
        }

        /// <summary>
        /// Gets or sets AvatarSize.
        /// </summary>
        public int AvatarSize
        {
            get
            {
                return this._reg.GetValue("AvatarSize", 50000);
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
                return this._reg.GetValue("MaxWordLength", 40);
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
                return this._reg.GetValue("MaxFileSize", 0);
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
                return this._reg.GetValue("SmiliesColumns", 3);
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
                return this._reg.GetValue("SmiliesPerRow", 6);
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
                return this._reg.GetValue("MessageHistoryDaysToLog", 30);
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
                return this._reg.GetValue("LockPosts", 0);
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
                return this._reg.GetValue("PostsPerPage", 20);
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
                return this._reg.GetValue("TopicsPerPage", 15);
            }

            set
            {
                this._reg.SetValue<int>("TopicsPerPage", value);
            }
        }

        /// <summary>
        /// Gets or sets ForumEditor.
        /// </summary>
        public string ForumEditor
        {
            get
            {
                return this._reg.GetValue("ForumEditor", "1");
            }

            set
            {
              this._reg.SetValue("ForumEditor", value);
            }
        }

        /// <summary>
        /// Gets or sets PostFloodDelay.
        /// </summary>
        public int PostFloodDelay
        {
            get
            {
                return this._reg.GetValue("PostFloodDelay", 30);
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
                return this._reg.GetValue("AllowedPollChoiceNumber", 10);
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
                return this._reg.GetValue("AllowedPollNumber", 3);
            }

            set
            {
                this._reg.SetValue<int>("AllowedPollNumber", value);
            }
        }

        /// <summary>
        /// Gets or sets PollImageMaxFileSize.
        /// </summary>
        public int PollImageMaxFileSize
        {
            get
            {
                return this._reg.GetValue("PollImageMaxFileSize", 100);
            }

            set
            {
                this._reg.SetValue<int>("PollImageMaxFileSize", value);
            }
        }

        /// <summary>
        /// Gets or sets CaptchaTypeRegister.
        /// </summary>
        public int CaptchaTypeRegister
        {
            get
            {
                return this._reg.GetValue("CaptchaTypeRegister", 1);
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
                return this._reg.GetValue("EditTimeOut", 30);
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
                return this._reg.GetValue("ReportPostPermissions", (int)ViewPermissions.RegisteredUsers);
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
                return this._reg.GetValue("CaptchaSize", 8);
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
                return this._reg.GetValue("ProfileViewPermission", (int)ViewPermissions.RegisteredUsers);
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
                return this._reg.GetValue("ReturnSearchMax", 100);
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
                return this._reg.GetValue("ActiveUsersViewPermissions", (int)ViewPermissions.RegisteredUsers);
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
                return this._reg.GetValue("MembersListViewPermissions", (int)ViewPermissions.RegisteredUsers);
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
                return this._reg.GetValue("ActiveDiscussionsCount", 5);
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
                return this._reg.GetValue("ActiveDiscussionsCacheTimeout", 1);
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
                return this._reg.GetValue("SearchStringMinLength", 4);
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
                return this._reg.GetValue("SearchStringMaxLength", 50);
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
                return this._reg.GetValue("SearchPermissions", (int)ViewPermissions.Everyone);
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
                return this._reg.GetValue("BoardPollID", 0);
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
                return this._reg.GetValue("ExternalSearchPermissions", (int)ViewPermissions.Nobody);
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
                return this._reg.GetValue("ForumStatisticsCacheTimeout", 60);
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
                return this._reg.GetValue("BoardUserStatsCacheTimeout", 60);
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
                return this._reg.GetValue("PrivateMessageMaxRecipients", 1);
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
                return this._reg.GetValue("DisableNoFollowLinksAfterDay", 0);
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
                return this._reg.GetValue("BoardForumListAllGuestCacheTimeout", 1440);
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
                return this._reg.GetValue("BoardModeratorsCacheTimeout", 1440);
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
                return this._reg.GetValue("BoardCategoriesCacheTimeout", 1440);
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
                return this._reg.GetValue("ReplaceRulesCacheTimeout", 1440);
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
                return this._reg.GetValue("FirstPostCacheTimeout", 120);
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
                return this._reg.GetValue("MaxPostSize", Int16.MaxValue);
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
                return this._reg.GetValue("MaxReportPostChars", 128);
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
                return this._reg.GetValue("MaxNumberOfAttachments", 5);
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
                return this._reg.GetValue("PictureAttachmentDisplayTreshold", 262144);
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
                return this._reg.GetValue("ImageAttachmentResizeWidth", 200);
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
                return this._reg.GetValue("ImageAttachmentResizeHeight", 200);
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
                return this._reg.GetValue("ShoutboxShowMessageCount", 30);
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
                return this._regBoard.GetValue("ActiveListTime", 5);
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
                return this._reg.GetValue("ActiveUserLazyDataCacheTimeout", 10);
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
                return this._reg.GetValue("OnlineStatusCacheTimeout", 60000);
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
                return this._reg.GetValue("UserNameMaxLength", 50);
            }

            set
            {
                this._reg.SetValue<int>("UserNameMaxLength", value);
            }
        }


        #endregion

        #region boolean settings

        /// <summary>
        /// Gets or sets a value indicating whether AllowUsersTextEditor.
        /// </summary>
        public bool AllowUsersTextEditor
        {
            get
            {
                return this._reg.GetValue("AllowUsersTextEditor", false);
            }

            set
            {
                this._reg.SetValue("AllowUsersTextEditor", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EmailVerification.
        /// </summary>
        public bool EmailVerification
        {
            get
            {
                return this._reg.GetValue("EmailVerification", false);
            }

            set
            {
                this._reg.SetValue("EmailVerification", value);
            }
        }

        public bool AllowNotificationAllPostsAllTopics
        {
            get
            {
                return this._reg.GetValue("AllowNotificationAllPostsAllTopics", false);
            }

            set
            {
                this._reg.SetValue("AllowNotificationAllPostsAllTopics", value);
            }
        }

        public bool AllowForumsWithSameName
        {
            get
            {
                return this._reg.GetValue("AllowForumsWithSameName", false);
            }

            set
            {
                this._reg.SetValue("AllowForumsWithSameName", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether UseFullTextSearch.
        /// </summary>
        public bool UseFullTextSearch
        {
            get
            {
                return this._reg.GetValue("UseFullTextSearch", false);
            }

            set
            {
                this._reg.SetValue("UseFullTextSearch", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowLastUnreadPost is enabled.
        /// </summary>
        public bool ShowLastUnreadPost
        {
            get
            {
                return this._reg.GetValue("ShowLastUnreadPost", true);
            }

            set
            {
                this._reg.SetValue("ShowLastUnreadPost", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Enable IP Info Service.
        /// </summary>
        public bool EnableIPInfoService
        {
            get
            {
                return this._reg.GetValue("EnableIPInfoService", false);
            }

            set
            {
                this._reg.SetValue("EnableIPInfoService", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EmailVerification.
        /// </summary>
        public bool AbandonSessionsForDontTrack
        {
            get
            {
                return this._reg.GetValue("AbadonSessionsForDontTrack", false);
            }

            set
            {
                this._reg.SetValue("AbadonSessionsForDontTrack", value);
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
                return this._reg.GetValue("UseSSLToLogIn", false);
            }

            set
            {
                this._reg.SetValue("UseSSLToLogIn", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Use SSL To Register.
        /// </summary>
        public bool UseSSLToRegister
        {
            get
            {
                return this._reg.GetValue("UseSSLToRegister", false);
            }

            set
            {
                this._reg.SetValue("UseSSLToRegister", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowMoved.
        /// </summary>
        public bool ShowMoved
        {
            get
            {
                return this._reg.GetValue("ShowMoved", true);
            }

            set
            {
                this._reg.SetValue("ShowMoved", value);
            }
        }

        /// <summary>
        /// Gets or sets the value indivicated if an avatar is should be shown in the topic listings.
        /// </summary>
        public bool ShowAvatarsInTopic
        {
            get
            {
                return this._reg.GetValue("ShowAvatarsInTopic", false);
            }

            set
            {
                this._reg.SetValue("ShowAvatarsInTopic", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Show Guests In Detailed Active List.
        /// </summary>
        public bool ShowGuestsInDetailedActiveList
        {
            get
            {
                return this._reg.GetValue("ShowGuestsInDetailedActiveList", false);
            }

            set
            {
                this._reg.SetValue("ShowGuestsInDetailedActiveList", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Show Crawlers In Active List.
        /// </summary>
        public bool ShowCrawlersInActiveList
        {
            get
            {
                return this._reg.GetValue("ShowCrawlersInActiveList", false);
            }

            set
            {
                this._reg.SetValue("ShowCrawlersInActiveList", value);
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether ShowGroups.
        /// </summary>
        public bool ShowGroups
        {
            get
            {
                return this._reg.GetValue("ShowGroups", true);
            }

            set
            {
                this._reg.SetValue("ShowGroups", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether BlankLinks.
        /// </summary>
        public bool BlankLinks
        {
            get
            {
                return this._reg.GetValue("BlankLinks", false);
            }

            set
            {
                this._reg.SetValue("BlankLinks", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowUserTheme.
        /// </summary>
        public bool AllowUserTheme
        {
            get
            {
                return this._reg.GetValue("AllowUserTheme", false);
            }

            set
            {
                this._reg.SetValue("AllowUserTheme", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowUserHideHimself.
        /// </summary>
        public bool AllowUserHideHimself
        {
            get
            {
                return this._reg.GetValue("AllowUserHideHimself", false);
            }

            set
            {
                this._reg.SetValue("AllowUserHideHimself", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowUserLanguage.
        /// </summary>
        public bool AllowUserLanguage
        {
            get
            {
                return this._reg.GetValue("AllowUserLanguage", false);
            }

            set
            {
                this._reg.SetValue("AllowUserLanguage", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowModeratorsViewIPs.
        /// </summary>
        public bool AllowModeratorsViewIPs
        {
            get
            {
                return this._reg.GetValue("AllowModeratorsViewIPs", false);
            }

            set
            {
                this._reg.SetValue("AllowModeratorsViewIPs", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowPMEmailNotification.
        /// </summary>
        public bool AllowPMEmailNotification
        {
            get
            {
                return this._reg.GetValue("AllowPMEmailNotification", true);
            }

            set
            {
                this._reg.SetValue("AllowPMEmailNotification", value);
            }
        }

        /// <summary>
        /// Gets or sets AllowPollChangesAfterFirstVote. A poll creator can't change choices after the first vote.
        /// </summary>
        public bool AllowPollChangesAfterFirstVote
        {
            get
            {
                return this._reg.GetValue("AllowPollChangesAfterFirstVote", false);
            }

            set
            {
                this._reg.SetValue("AllowPollChangesAfterFirstVote", value);
            }
        }

        /// <summary>
        /// Gets or sets AllowUsersHidePollResults. 
        /// </summary>
        public bool AllowUsersHidePollResults
        {
            get
            {
                return this._reg.GetValue("AllowViewPollVotesIfNoPollAcces", true);
            }

            set
            {
                this._reg.SetValue("AllowViewPollVotesIfNoPollAcces", value);
            }
        }

        /// <summary>
        /// Gets or sets the if relative times are used on the forum.
        /// </summary>
        public bool ShowRelativeTime
        {
            get
            {
                return this._reg.GetValue("ShowRelativeTime", true);
            }

            set
            {
                this._reg.SetValue("ShowRelativeTime", value);
            }
        }

        /// <summary>
        /// Gets or sets the Refresh Rate for the Timeago
        /// </summary>
        public int RelativeTimeRefreshTime
        {
            get
            {
                return this._reg.GetValue("RelativeTimeRefreshTime", 60000);
            }

            set
            {
                this._reg.SetValue<int>("RelativeTimeRefreshTime", value);
            }
        }

        /// <summary>
        /// Gets or sets AllowMultipleChoices. 
        /// </summary>
        public bool AllowMultipleChoices
        {
            get
            {
                return this._reg.GetValue("AllowMultipleChoices", true);
            }

            set
            {
                this._reg.SetValue("AllowMultipleChoices", value);
            }
        }

        /// <summary>
        /// Gets or sets AllowGuestsViewPollOptions. 
        /// </summary>
        public bool AllowGuestsViewPollOptions
        {
            get
            {
                return this._reg.GetValue("AllowGuestsViewPollOptions", true);
            }

            set
            {
                this._reg.SetValue("AllowGuestsViewPollOptions", value);
            }
        }

        /// <summary>
        /// Gets or sets AllowUsersImagedPoll. 
        /// </summary>
        public bool AllowUsersImagedPoll
        {
            get
            {
                return this._reg.GetValue("AllowUsersImagedPoll", false);
            }

            set
            {
                this._reg.SetValue("AllowUsersImagedPoll", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AvatarUpload.
        /// </summary>
        public bool AvatarUpload
        {
            get
            {
                return this._reg.GetValue("AvatarUpload", false);
            }

            set
            {
                this._reg.SetValue("AvatarUpload", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AvatarRemote.
        /// </summary>
        public bool AvatarRemote
        {
            get
            {
                return this._reg.GetValue("AvatarRemote", false);
            }

            set
            {
                this._reg.SetValue("AvatarRemote", value);
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
                return this._reg.GetValue("AvatarGravatar", false);
            }

            set
            {
                this._reg.SetValue("AvatarGravatar", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowEmailChange.
        /// </summary>
        public bool AllowEmailChange
        {
            get
            {
                return this._reg.GetValue("AllowEmailChange", true);
            }

            set
            {
                this._reg.SetValue("AllowEmailChange", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowPasswordChange.
        /// </summary>
        public bool AllowPasswordChange
        {
            get
            {
                return this._reg.GetValue("AllowPasswordChange", true);
            }

            set
            {
                this._reg.SetValue("AllowPasswordChange", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether UseFileTable.
        /// </summary>
        public bool UseFileTable
        {
            get
            {
                return this._reg.GetValue("UseFileTable", false);
            }

            set
            {
                this._reg.SetValue("UseFileTable", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowRSSLink.
        /// </summary>
        public bool ShowRSSLink
        {
            get
            {
                return this._reg.GetValue("ShowRSSLink", true);
            }

            set
            {
                this._reg.SetValue("ShowRSSLink", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowAtomLink.
        /// </summary>
        public bool ShowAtomLink
        {
            get
            {
                return this._reg.GetValue("ShowAtomLink", true);
            }

            set
            {
                this._reg.SetValue("ShowAtomLink", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowPageGenerationTime.
        /// </summary>
        public bool ShowPageGenerationTime
        {
            get
            {
                return this._reg.GetValue("ShowPageGenerationTime", true);
            }

            set
            {
                this._reg.SetValue("ShowPageGenerationTime", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowYAFVersion.
        /// </summary>
        public bool ShowYAFVersion
        {
            get
            {
                return this._reg.GetValue("ShowYAFVersion", true);
            }

            set
            {
                this._reg.SetValue("ShowYAFVersion", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowForumJump.
        /// </summary>
        public bool ShowForumJump
        {
            get
            {
                return this._reg.GetValue("ShowForumJump", true);
            }

            set
            {
                this._reg.SetValue("ShowForumJump", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowPrivateMessages.
        /// </summary>
        public bool AllowPrivateMessages
        {
            get
            {
                return this._reg.GetValue("AllowPrivateMessages", true);
            }

            set
            {
                this._reg.SetValue("AllowPrivateMessages", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowEmailSending.
        /// </summary>
        public bool AllowEmailSending
        {
            get
            {
                return this._reg.GetValue("AllowEmailSending", true);
            }

            set
            {
                this._reg.SetValue("AllowEmailSending", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowSignatures.
        /// </summary>
        public bool AllowSignatures
        {
            get
            {
                return this._reg.GetValue("AllowSignatures", true);
            }

            set
            {
                this._reg.SetValue("AllowSignatures", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ExternalSearchInNewWindow.
        /// </summary>
        public bool ExternalSearchInNewWindow
        {
            get
            {
                return this._reg.GetValue("ExternalSearchInNewWindow", false);
            }

            set
            {
                this._reg.SetValue("ExternalSearchInNewWindow", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Enable Quick Search.
        /// </summary>
        public bool ShowQuickSearch
        {
            get
            {
                return this._reg.GetValue("ShowQuickSearch", false);
            }

            set
            {
                this._reg.SetValue("ShowQuickSearch", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether RemoveNestedQuotes.
        /// </summary>
        public bool RemoveNestedQuotes
        {
            get
            {
                return this._reg.GetValue("RemoveNestedQuotes", false);
            }

            set
            {
                this._reg.SetValue("RemoveNestedQuotes", value);
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether DisableRegistrations.
        /// </summary>
        public bool DisableRegistrations
        {
            get
            {
                return this._reg.GetValue("DisableRegistrations", false);
            }

            set
            {
                this._reg.SetValue("DisableRegistrations", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether CreateNntpUsers.
        /// </summary>
        public bool CreateNntpUsers
        {
            get
            {
                return this._reg.GetValue("CreateNntpUsers", false);
            }

            set
            {
                this._reg.SetValue("CreateNntpUsers", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowGroupsProfile.
        /// </summary>
        public bool ShowGroupsProfile
        {
            get
            {
                return this._reg.GetValue("ShowGroupsProfile", false);
            }

            set
            {
                this._reg.SetValue("ShowGroupsProfile", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether PollVoteTiedToIP.
        /// </summary>
        public bool PollVoteTiedToIP
        {
            get
            {
                return this._reg.GetValue("PollVoteTiedToIP", true);
            }

            set
            {
                this._reg.SetValue("PollVoteTiedToIP", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowAdsToSignedInUsers.
        /// </summary>
        public bool ShowAdsToSignedInUsers
        {
            get
            {
                return this._reg.GetValue("ShowAdsToSignedInUsers", true);
            }

            set
            {
                this._reg.SetValue("ShowAdsToSignedInUsers", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether DisplayPoints.
        /// </summary>
        public bool DisplayPoints
        {
            get
            {
                return this._reg.GetValue("DisplayPoints", false);
            }

            set
            {
                this._reg.SetValue("DisplayPoints", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowQuickAnswer.
        /// </summary>
        public bool ShowQuickAnswer
        {
            get
            {
                return this._reg.GetValue("ShowQuickAnswer", true);
            }

            set
            {
                this._reg.SetValue("ShowQuickAnswer", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowDeletedMessages.
        /// </summary>
        public bool ShowDeletedMessages
        {
            get
            {
                return this._reg.GetValue("ShowDeletedMessages", true);
            }

            set
            {
                this._reg.SetValue("ShowDeletedMessages", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowDeletedMessagesToAll.
        /// </summary>
        public bool ShowDeletedMessagesToAll
        {
            get
            {
                return this._reg.GetValue("ShowDeletedMessagesToAll", false);
            }

            set
            {
                this._reg.SetValue("ShowDeletedMessagesToAll", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowModeratorList.
        /// </summary>
        public bool ShowModeratorList
        {
            get
            {
                return this._reg.GetValue("ShowModeratorList", true);
            }

            set
            {
                this._reg.SetValue("ShowModeratorList", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EnableCaptchaForPost.
        /// </summary>
        public bool EnableCaptchaForPost
        {
            get
            {
                return this._reg.GetValue("EnableCaptchaForPost", false);
            }

            set
            {
                this._reg.SetValue("EnableCaptchaForPost", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EnableCaptchaForRegister.
        /// </summary>
        public bool EnableCaptchaForRegister
        {
            get
            {
                return this._reg.GetValue("EnableCaptchaForRegister", false);
            }

            set
            {
                this._reg.SetValue("EnableCaptchaForRegister", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EnableCaptchaForGuests.
        /// </summary>
        public bool EnableCaptchaForGuests
        {
            get
            {
                return this._reg.GetValue("EnableCaptchaForGuests", true);
            }

            set
            {
                this._reg.SetValue("EnableCaptchaForGuests", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether UseNoFollowLinks.
        /// </summary>
        public bool UseNoFollowLinks
        {
            get
            {
                return this._reg.GetValue("UseNoFollowLinks", true);
            }

            set
            {
                this._reg.SetValue("UseNoFollowLinks", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether DoUrlReferrerSecurityCheck.
        /// </summary>
        public bool DoUrlReferrerSecurityCheck
        {
            get
            {
                return this._reg.GetValue("DoUrlReferrerSecurityCheck", false);
            }

            set
            {
                this._reg.SetValue("DoUrlReferrerSecurityCheck", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EnableImageAttachmentResize.
        /// </summary>
        public bool EnableImageAttachmentResize
        {
            get
            {
                return this._reg.GetValue("EnableImageAttachmentResize", true);
            }

            set
            {
                this._reg.SetValue("EnableImageAttachmentResize", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowShoutbox.
        /// </summary>
        public bool ShowShoutbox
        {
            get
            {
                return this._reg.GetValue("ShowShoutbox", true);
            }

            set
            {
                this._reg.SetValue("ShowShoutbox", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Show smiles in Shoutbox.
        /// </summary>
        public bool ShowShoutboxSmiles
        {
            get
            {
                return this._reg.GetValue("ShowShoutboxSmiles", true);
            }

            set
            {
                this._reg.SetValue("ShowShoutboxSmiles", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowUserInfoCaching.
        /// </summary>
        public bool AllowUserInfoCaching
        {
            get
            {
                return this._reg.GetValue("AllowUserInfoCaching", true);
            }

            set
            {
                this._reg.SetValue("AllowUserInfoCaching", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether display No-Count Forums In ActiveDiscussions.
        /// </summary>
        public bool NoCountForumsInActiveDiscussions
        {
            get
            {
                return this._reg.GetValue("NoCountForumsInActiveDiscussions", true);
            }

            set
            {
                this._reg.SetValue("NoCountForumsInActiveDiscussions", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether UseStyledNicks.
        /// </summary>
        public bool UseStyledNicks
        {
            get
            {
                return this._reg.GetValue("UseStyledNicks", false);
            }

            set
            {
                this._reg.SetValue("UseStyledNicks", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowUserOnlineStatus.
        /// </summary>
        public bool ShowUserOnlineStatus
        {
            get
            {
                return this._reg.GetValue("ShowUserOnlineStatus", false);
            }

            set
            {
                this._reg.SetValue("ShowUserOnlineStatus", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowThanksDate.
        /// </summary>
        public bool ShowThanksDate
        {
            get
            {
                return this._reg.GetValue("ShowThanksDate", true);
            }

            set
            {
                this._reg.SetValue("ShowThanksDate", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EnableThanksMod.
        /// </summary>
        public bool EnableThanksMod
        {
            get
            {
                return this._reg.GetValue("EnableThanksMod", true);
            }

            set
            {
                this._reg.SetValue("EnableThanksMod", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EnableBuddyList.
        /// </summary>
        public bool EnableBuddyList
        {
            get
            {
                return this._reg.GetValue("EnableBuddyList", true);
            }

            set
            {
                this._reg.SetValue("EnableBuddyList", value);
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
                return this._reg.GetValue("EnableDNACalendar", true);
            }

            set
            {
                this._reg.SetValue("EnableDNACalendar", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EnableActiveLocationErrorsLog. A temporary debug setting.
        /// </summary>
        public bool EnableActiveLocationErrorsLog
        {
            get
            {
                return this._reg.GetValue("EnableActiveLocationErrorsLog", false);
            }

            set
            {
                this._reg.SetValue("EnableActiveLocationErrorsLog", value);
            }
        }

        /// <summary>
        /// Log UserAgent strings unhandled by YAF.
        /// </summary>
        public bool UserAgentBadLog
        {
            get
            {
                return this._reg.GetValue("UserAgentBadLog", false);
            }

            set
            {
                this._reg.SetValue("UserAgentBadLog", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EnableAlbum.
        /// </summary>
        public bool EnableAlbum
        {
            get
            {
                return this._reg.GetValue("EnableAlbum", true);
            }

            set
            {
                this._reg.SetValue("EnableAlbum", value);
            }
        }

        /// <summary>
        /// Gets or sets AlbumImagesSizeMax.
        /// </summary>
        public int AlbumImagesSizeMax
        {
            get
            {
                return this._regBoard.GetValue("AlbumImagesSizeMax", 1048576);
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
                return this._regBoard.GetValue("AlbumsPerPage", 6);
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
                return this._regBoard.GetValue("AlbumImagesPerPage", 10);
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
                return this._reg.GetValue("AddDynamicPageMetaTags", true);
            }

            set
            {
                this._reg.SetValue("AddDynamicPageMetaTags", value);
            }
        }

        public bool AllowDisplayNameModification
        {
            get
            {
                return this._reg.GetValue("AllowDisplayNameModification", true);
            }

            set
            {
                this._reg.SetValue("AllowDisplayNameModification", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ImageAttachmentResizeCropped.
        /// </summary>
        public bool ImageAttachmentResizeCropped
        {
            get
            {
                return this._reg.GetValue("ImageAttachmentResizeCropped", false);
            }

            set
            {
                this._reg.SetValue("ImageAttachmentResizeCropped", value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether Use LoginBox or Login Page
        /// </summary>
        public bool UseLoginBox
        {
            get
            {
                return this._reg.GetValue("UseLoginBox", false);
            }

            set
            {
                this._reg.SetValue("UseLoginBox", value);
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
                return this._reg.GetValue("IPInfoPageURL", "http://www.dnsstuff.com/tools/whois.ch?ip={0}");
            }

            set
            {
                this._reg.SetValue("IPInfoPageURL", value);
            }
        }

        /// <summary>
        /// Gets or sets IP Locator Path.
        /// </summary>
        public string IPLocatorPath
        {
            get
            {
                return this._reg.GetValue("IPLocatorPath", "http://ipinfodb.com/ip_query.php?ip={0}&timezone={1}");
            }

            set
            {
                this._reg.SetValue("IPLocatorPath", value);
            }
        }

        /// <summary>
        /// Gets or sets ForumEmail.
        /// </summary>
        public string ForumEmail
        {
            get
            {
                return this._reg.GetValue("ForumEmail", string.Empty);
            }

            set
            {
                this._reg.SetValue("ForumEmail", value);
            }
        }

        /// <summary>
        /// Gets or sets EnableIrkoo.
        /// </summary>
        public bool EnableIrkoo
        {
            get
            {
                return this._reg.GetValue("EnableIrkoo", false);
            }

            set
            {
                this._reg.SetValue("EnableIrkoo", value);
            }
        }

        /// <summary>
        /// Gets or sets IrkooSiteID.
        /// </summary>
        public string IrkooSiteID
        {
            get
            {
                return this._reg.GetValue("IrkooSiteID", string.Empty);
            }

            set
            {
                this._reg.SetValue("IrkooSiteID", value);
            }
        }

        /// <summary>
        /// Gets or sets IrkooSecretKey.
        /// </summary>
        public string IrkooSecretKey
        {
            get
            {
                return this._reg.GetValue("IrkooSecretKey", string.Empty);
            }

            set
            {
                this._reg.SetValue("IrkooSecretKey", value);
            }
        }

        /// <summary>
        /// Gets or sets ShowIrkooRepInUserLink.
        /// </summary>
        public bool ShowIrkooRepOnlyInTopics
        {
            get
            {
                return this._reg.GetValue("ShowIrkooRepOnlyInTopics", false);
            }

            set
            {
                this._reg.SetValue("ShowIrkooRepOnlyInTopics", value);
            }
        }

        /// <summary>
        /// Gets or sets AllowGuestsViewReputation.
        /// </summary>
        public bool AllowGuestsViewReputation
        {
            get
            {
                return this._reg.GetValue("AllowGuestsViewReputation", true);
            }

            set
            {
                this._reg.SetValue("AllowGuestsViewReputation", value);
            }
        }

        /// <summary>
        /// Gets or sets RecaptchaPublicKey .
        /// </summary>
        public string RecaptchaPublicKey
        {
            get
            {
                return this._reg.GetValue("RecaptchaPublicKey", string.Empty);
            }

            set
            {
                this._reg.SetValue("RecaptchaPublicKey", value);
            }
        }

        /// <summary>
        /// Gets or sets RecaptchaPrivateKey.
        /// </summary>
        public string RecaptchaPrivateKey
        {
            get
            {
                return this._reg.GetValue("RecaptchaPrivateKey", string.Empty);
            }

            set
            {
                this._reg.SetValue("RecaptchaPrivateKey", value);
            }
        }

        /// <summary>
        /// Gets or sets SearchEngine1.
        /// </summary>
        public string SearchEngine1
        {
            get
            {
                return this._reg.GetValue("SearchEngine1", "http://google.com/search?as_q={Word}&hl={Language}&num={ResultsPerPage}&btnG={ButtonName}&as_epq={Word}&as_oq={Word}&as_eq={Word}&lr=&cr=&as_ft=i&as_filetype=&as_qdr=&as_occt=&as_dt=i&as_sitesearch={Site}&as_rights=&safe=off");
            }

            set
            {
                this._reg.SetValue("SearchEngine1", value);
            }
        }

        /// <summary>
        /// Gets or sets SearchEngine2.
        /// </summary>
        public string SearchEngine2
        {
            get
            {

                return this._reg.GetValue("SearchEngine2", "http://yandex.ru/yandsearch?date=all&text=&site={Site}&rstr=&zone=all&wordforms=&lang={Language}&within=&from_day=&from_month=&from_year=&to_day=&to_month=&to_year=&mime=&numdoc={ResultsPerPage}&lr=");
            }

            set
            {
                this._reg.SetValue("SearchEngine2", value);
            }
        }

        /// <summary>
        /// Gets or sets SearchEngine1Parameters.
        /// </summary>
        public string SearchEngine1Parameters
        {
            get
            {
                return this._reg.GetValue("SearchEngine1Parameters", "Google^?^&^+^;^AnyWord:as_oq={Word}^AllWords:as_q={Word}^ExactFrase:as_epq={Word}^WithoutWords:as_eq={Word}");
            }

            set
            {
                this._reg.SetValue("SearchEngine1Parameters", value);
            }
        }

        /// <summary>
        /// Gets or sets SearchEngine2Parameters.
        /// </summary>
        public string SearchEngine2Parameters
        {
            get
            {
                return this._reg.GetValue("SearchEngine2Parameters", "Yandex^?^&^+^;^AnyWord:text={Word}/wordforms=any^AllWords:text={Word}/wordforms=all^ExactFrase:text={Word}/wordforms=exact^WithoutWords:text=~~{Word}");
            }

            set
            {
                this._reg.SetValue("SearchEngine2Parameters", value);
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
                return this._reg.GetValue("GravatarRating", "G");
            }

            set
            {
                this._reg.SetValue("GravatarRating", value);
            }
        }

        /// <summary>
        /// Gets or sets AcceptedHTML.
        /// </summary>
        public string AcceptedHTML
        {
            get
            {
                return this._reg.GetValue("AcceptedHTML", "br,hr,b,i,u,a,div,ol,ul,li,blockquote,img,span,p,em,strong,font,pre,h1,h2,h3,h4,h5,h6,address");
            }

            set
            {
                this._reg.SetValue("AcceptedHTML", value.ToLower());
            }
        }

        /// <summary>
        /// Gets or sets AcceptedHeadersHTML.
        /// </summary>
        public string AcceptedHeadersHTML
        {
            get
            {
                return this._reg.GetValue("AcceptedHeadersHTML", String.Empty);
            }

            set
            {
                this._reg.SetValue("AcceptedHeadersHTML", value.ToLower());
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
                this._reg.SetValue("AdPost", value);
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
                this._reg.SetValue("CustomLoginRedirectUrl", value);
            }
        }

        /// <summary>
        /// Gets or sets WebServiceToken.
        /// </summary>
        public string WebServiceToken
        {
            get
            {
                return this._reg.GetValue("WebServiceToken", Guid.NewGuid().ToString());
            }

            set
            {
                this._reg.SetValue("WebServiceToken", value);
            }
        }

        /// <summary>
        /// Gets or sets SearchStringPattern.
        /// </summary>
        public string SearchStringPattern
        {
            get
            {
                return this._reg.GetValue("SearchStringPattern", ".*");
            }

            set
            {
                this._reg.SetValue("SearchStringPattern", value);
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
                return this._reg.GetValue("DisplayJoinDate", true);
            }

            set
            {
                this._reg.SetValue("DisplayJoinDate", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowBrowsingUsers.
        /// </summary>
        public bool ShowBrowsingUsers
        {
            get
            {
                return this._reg.GetValue("ShowBrowsingUsers", true);
            }

            set
            {
                this._reg.SetValue("ShowBrowsingUsers", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowMedals.
        /// </summary>
        public bool ShowMedals
        {
            get
            {
                return this._reg.GetValue("ShowMedals", true);
            }

            set
            {
                this._reg.SetValue("ShowMedals", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowPostToBlog.
        /// </summary>
        public bool AllowPostToBlog
        {
            get
            {
                return this._reg.GetValue("AllowPostToBlog", false);
            }

            set
            {
                this._reg.SetValue("AllowPostToBlog", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AllowReportPosts.
        /// </summary>
        public bool AllowReportPosts
        {
            get
            {
                return this._reg.GetValue("AllowReportPosts", true);
            }

            set
            {
                this._reg.SetValue("AllowReportPosts", value);
            }
        }

        /* Ederon : 8/29/2007 */

        /// <summary>
        /// Gets or sets a value indicating whether Allow Email Topic.
        /// </summary>
        public bool AllowEmailTopic
        {
            get
            {
                return this._reg.GetValue("AllowEmailTopic", true);
            }

            set
            {
                this._reg.SetValue("AllowEmailTopic", value);
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
                return this._reg.GetValue("RequireLogin", false);
            }

            set
            {
                this._reg.SetValue("RequireLogin", value);
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
                return this._reg.GetValue("ShowActiveDiscussions", true);
            }

            set
            {
                this._reg.SetValue("ShowActiveDiscussions", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowForumStatistics.
        /// </summary>
        public bool ShowForumStatistics
        {
            get
            {
                return this._reg.GetValue("ShowForumStatistics", true);
            }

            set
            {
                this._reg.SetValue("ShowForumStatistics", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowRulesForRegistration.
        /// </summary>
        public bool ShowRulesForRegistration
        {
            get
            {
                return this._reg.GetValue("ShowRulesForRegistration", true);
            }

            set
            {
                this._reg.SetValue("ShowRulesForRegistration", value);
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
                return this._reg.GetValue("UserBox", Constants.UserBox.DisplayTemplateDefault);
            }

            set
            {
                this._reg.SetValue("UserBox", value);
            }
        }

        /// <summary>
        /// Gets or sets UserBoxAvatar.
        /// </summary>
        public string UserBoxAvatar
        {
            get
            {
                return this._reg.GetValue("UserBoxAvatar", @"<div class=""section"">{0}</div><br clear=""all"" />");
            }

            set
            {
                this._reg.SetValue("UserBoxAvatar", value);
            }
        }

        /// <summary>
        /// Gets or sets UserBoxMedals.
        /// </summary>
        public string UserBoxMedals
        {
            get
            {
                return this._reg.GetValue("UserBoxMedals", @"<div class=""section medals"">{0} {1}{2}</div><br clear=""all"" />");
            }

            set
            {
                this._reg.SetValue("UserBoxMedals", value);
            }
        }

        /// <summary>
        /// Gets or sets UserBoxRankImage.
        /// </summary>
        public string UserBoxRankImage
        {
            get
            {
                return this._reg.GetValue("UserBoxRankImage", "{0}<br clear=\"all\" />");
            }

            set
            {
                this._reg.SetValue("UserBoxRankImage", value);
            }
        }

        /// <summary>
        /// Gets or sets UserBoxRank.
        /// </summary>
        public string UserBoxRank
        {
            get
            {
                return this._reg.GetValue("UserBoxRank", "{0}: {1}<br clear=\"all\" />");
            }

            set
            {
                this._reg.SetValue("UserBoxRank", value);
            }
        }

        /// <summary>
        /// Gets or sets UserBoxGroups.
        /// </summary>
        public string UserBoxGroups
        {
            get
            {
                return this._reg.GetValue("UserBoxGroups", "{0}: {1}<br clear=\"all\" />");
            }

            set
            {
                this._reg.SetValue("UserBoxGroups", value);
            }
        }

        /// <summary>
        /// Gets or sets UserBoxJoinDate.
        /// </summary>
        public string UserBoxJoinDate
        {
            get
            {
                return this._reg.GetValue("UserBoxJoinDate", "{0}: {1}<br />");
            }

            set
            {
                this._reg.SetValue("UserBoxJoinDate", value);
            }
        }

      /// <summary>
      /// Gets or sets UserBoxGender.
      /// </summary>
      public string UserBoxGender
        {
            get
            {
                return this._reg.GetValue("UserBoxGender", "{0}<br />");
            }

            set
            {
                this._reg.SetValue("UserBoxGender", value);
            }
        }

        /// <summary>
        /// Gets or sets UserBoxPosts.
        /// </summary>
        public string UserBoxPosts
        {
            get
            {
                return this._reg.GetValue("UserBoxPosts", "{0}: {1:N0}<br />");
            }

            set
            {
                this._reg.SetValue("UserBoxPosts", value);
            }
        }

        /// <summary>
        /// Gets or sets UserBoxPoints.
        /// </summary>
        public string UserBoxPoints
        {
            get
            {
                return this._reg.GetValue("UserBoxPoints", "{0}: {1:N0}<br />");
            }

            set
            {
                this._reg.SetValue("UserBoxPoints", value);
            }
        }

        /// <summary>
        /// Gets or sets UserBoxLocation.
        /// </summary>
        public string UserBoxLocation
        {
            get
            {
                return this._reg.GetValue("UserBoxLocation", "{0}: {1}<br />");
            }

            set
            {
                this._reg.SetValue("UserBoxLocation", value);
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
                return this._reg.GetValue("UserBoxThanksFrom", "{0}<br />");
            }

            set
            {
                this._reg.SetValue("UserBoxThanksFrom", value);
            }
        }

        /// <summary>
        /// Gets or sets UserBoxThanksTo.
        /// </summary>
        public string UserBoxThanksTo
        {
            get
            {
                return this._reg.GetValue("UserBoxThanksTo", "{0}<br />");
            }

            set
            {
                this._reg.SetValue("UserBoxThanksTo", value);
            }
        }

      /// <summary>
      /// Gets or sets LastDigestSend.
      /// </summary>
      public string LastDigestSend
        {
            get
            {
                return this._regBoard.GetValue<string>("LastDigestSend", null);
            }

            set
            {
                this._regBoard.SetValue("LastDigestSend", value);
            }
        }

      /// <summary>
      /// Gets or sets TwitterUserName.
      /// </summary>
      public string TwitterUserName
      {
          get
          {
              return this._regBoard.GetValue<string>("TwitterUserName", null);
          }

          set
          {
              this._regBoard.SetValue("TwitterUserName", value);
          }
      }

      /// <summary>
      /// Gets or sets a value indicating whether ForceDigestSend.
      /// </summary>
      public bool ForceDigestSend
        {
            get
            {
                return this._regBoard.GetValue("ForceDigestSend", false);
            }
            set
            {
                this._regBoard.SetValue("ForceDigestSend", value);
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