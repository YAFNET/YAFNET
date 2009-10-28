/* Yet Another Forum.NET
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
using System;
using System.Collections.Generic;
using System.Reflection;
using YAF.Classes.Pattern;

namespace YAF.Classes
{
	public class YafBoardSettings
	{
		public class YafLegacyBoardSettings
		{
			public string BoardName
			{
				get;
				set;
			}

			public string SqlVersion
			{
				get;
				set;
			}

			public bool AllowThreaded
			{
				get;
				set;
			}

			public string MembershipAppName
			{
				get;
				set;
			}

			public string RolesAppName
			{
				get;
				set;
			}

			public YafLegacyBoardSettings()
			{

			}

			public YafLegacyBoardSettings( string boardName, string sqlVersion, bool allowThreaded, string membershipAppName, string rolesAppName )
				: this()
			{
				BoardName = boardName;
				SqlVersion = sqlVersion;
				AllowThreaded = allowThreaded;
				MembershipAppName = membershipAppName;
				RolesAppName = rolesAppName;
			}
		}

		/* Ederon : 6/16/2007 - conventions */
		protected YafLegacyBoardSettings _legacyBoardSettings = new YafLegacyBoardSettings();
		protected readonly RegistryDictionaryOverride _reg;
		protected readonly RegistryDictionary _regBoard;
		protected object _boardID;
		protected string _membershipAppName, _rolesAppName;

		public YafBoardSettings()
		{
			_boardID = 0;
			_reg = new RegistryDictionaryOverride();
			_regBoard = new RegistryDictionary();

			// set the board dictionary as the override...
			_reg.OverrideDictionary = _regBoard;

			_membershipAppName = System.Web.Security.Membership.ApplicationName;
			_rolesAppName = System.Web.Security.Roles.ApplicationName;
		}

		// Board/Override properties...
		// Future stuff... still in progress.
		public bool SetBoardRegistryOnly
		{
			get
			{
				return _reg.DefaultSetOverride;
			}
			set
			{
				_reg.DefaultSetOverride = value;
			}
		}

		public bool GetBoardRegistryOverride
		{
			get
			{
				return _reg.DefaultGetOverride;
			}
			set
			{
				_reg.DefaultGetOverride = value;
			}
		}

		// Provider Settings

		public string MembershipAppName
		{
			get { return _membershipAppName; }
		}

		public string RolesAppName
		{
			get { return _rolesAppName; }
		}

		// individual board settings
		public string Name
		{
			get { return _legacyBoardSettings.BoardName; }
		}
		public bool AllowThreaded
		{
			get { return _legacyBoardSettings.AllowThreaded; }
		}
		public bool AllowThemedLogo
		{
			get { return _reg.GetValue<bool>( "AllowThemedLogo", false ); }
			set { _reg.SetValue<bool>( "AllowThemedLogo", value ); }
		}
		public int MaxUsers
		{
			get { return _regBoard.GetValue<int>( "MaxUsers", 1 ); }
		}
		public DateTime MaxUsersWhen
		{
			get { return _regBoard.GetValue<DateTime>( "MaxUsersWhen", DateTime.Now ); }
		}
		public string Theme
		{
			get { return _regBoard.GetValue<string>( "Theme", "cleanslate.xml" ); }
			set { _regBoard.SetValue<string>( "Theme", value ); }
		}
		public string Language
		{
			get { return _regBoard.GetValue<string>( "Language", "english.xml" ); }
			set { _regBoard.SetValue<string>( "Language", value ); }
		}
		public int ShowTopicsDefault
		{
			get { return _regBoard.GetValue<int>( "ShowTopicsDefault", 0 ); }
			set { _regBoard.SetValue<int>( "ShowTopicsDefault", value ); }
		}
		public bool FileExtensionAreAllowed
		{
			get { return _regBoard.GetValue<bool>( "FileExtensionAreAllowed", true ); }
			set { _regBoard.SetValue<bool>( "FileExtensionAreAllowed", value ); }
		}
		public string NotificationOnUserRegisterEmailList
		{
			get { return _regBoard.GetValue<string>( "NotificationOnUserRegisterEmailList", null ); }
			set { _regBoard.SetValue<string>( "NotificationOnUserRegisterEmailList", value ); }
		}

		// didn't know where else to put this :)
		public string SQLVersion
		{
			get { return _legacyBoardSettings.SqlVersion; }
		}

		// global forum settings from registry
		public TimeSpan TimeZone
		{
			get
			{
				int min = TimeZoneRaw;
				return new TimeSpan( min / 60, min % 60, 0 );
			}
		}
		#region int settings

		public int TimeZoneRaw
		{
			get { return _reg.GetValue<int>( "TimeZone", 0 ); }
			set { _reg.SetValue<int>( "TimeZone", value ); }
		}
		public int AvatarWidth
		{
			get { return _reg.GetValue<int>( "AvatarWidth", 50 ); }
			set { _reg.SetValue<int>( "AvatarWidth", value ); }
		}
		public int AvatarHeight
		{
			get { return _reg.GetValue<int>( "AvatarHeight", 80 ); }
			set { _reg.SetValue<int>( "AvatarHeight", value ); }
		}
		public int AvatarSize
		{
			get { return _reg.GetValue<int>( "AvatarSize", 50000 ); }
			set { _reg.SetValue<int>( "AvatarSize", value ); }
		}
		public int MaxFileSize
		{
			get { return _reg.GetValue<int>( "MaxFileSize", 0 ); }
			set { _reg.SetValue<int>( "MaxFileSize", value ); }
		}
		public int SmiliesColumns
		{
			get { return _reg.GetValue<int>( "SmiliesColumns", 3 ); }
			set { _reg.SetValue<int>( "SmiliesColumns", value ); }
		}
		public int SmiliesPerRow
		{
			get { return _reg.GetValue<int>( "SmiliesPerRow", 6 ); }
			set { _reg.SetValue<int>( "SmiliesPerRow", value ); }
		}
		public int LockPosts
		{
			get { return _reg.GetValue<int>( "LockPosts", 0 ); }
			set { _reg.SetValue<int>( "LockPosts", value ); }
		}
		public int PostsPerPage
		{
			get { return _reg.GetValue<int>( "PostsPerPage", 20 ); }
			set { _reg.SetValue<int>( "PostsPerPage", value ); }
		}
		public int TopicsPerPage
		{
			get { return _reg.GetValue<int>( "TopicsPerPage", 15 ); }
			set { _reg.SetValue<int>( "TopicsPerPage", value ); }
		}
		public int ForumEditor
		{
			get { return _reg.GetValue<int>( "ForumEditor", 1 ); }
			set { _reg.SetValue<int>( "ForumEditor", value ); }
		}
		public int PostFloodDelay
		{
			get { return _reg.GetValue<int>( "PostFloodDelay", 30 ); }
			set { _reg.SetValue<int>( "PostFloodDelay", value ); }
		}
		public int EditTimeOut
		{
			get { return _reg.GetValue<int>( "EditTimeOut", 30 ); }
			set { _reg.SetValue<int>( "EditTimeOut", value ); }
		}
		public int CaptchaSize
		{
			get { return _reg.GetValue<int>( "CaptchaSize", 5 ); }
			set { _reg.SetValue<int>( "CaptchaSize", value ); }
		}
		// Ederon : 11/21/2007
		public int ProfileViewPermissions
		{
			get { return _reg.GetValue<int>( "ProfileViewPermission", (int)ViewPermissions.RegisteredUsers ); }
			set { _reg.SetValue<int>( "ProfileViewPermission", value ); }
		}
		public int ReturnSearchMax
		{
			get { return _reg.GetValue<int>( "ReturnSearchMax", 100 ); }
			set { _reg.SetValue<int>( "ReturnSearchMax", value ); }
		}
		// Ederon : 12/9/2007
		public int ActiveUsersViewPermissions
		{
			get { return _reg.GetValue<int>( "ActiveUsersViewPermissions", (int)ViewPermissions.RegisteredUsers ); }
			set { _reg.SetValue<int>( "ActiveUsersViewPermissions", value ); }
		}
		public int MembersListViewPermissions
		{
			get { return _reg.GetValue<int>( "MembersListViewPermissions", (int)ViewPermissions.RegisteredUsers ); }
			set { _reg.SetValue<int>( "MembersListViewPermissions", value ); }
		}
		// Ederon : 12/14/2007
		public int ActiveDiscussionsCount
		{
			get { return _reg.GetValue<int>( "ActiveDiscussionsCount", 5 ); }
			set { _reg.SetValue<int>( "ActiveDiscussionsCount", value ); }
		}
		public int ActiveDiscussionsCacheTimeout
		{
			get { return _reg.GetValue<int>( "ActiveDiscussionsCacheTimeout", 1 ); }
			set { _reg.SetValue<int>( "ActiveDiscussionsCacheTimeout", value ); }
		}
		public int SearchStringMinLength
		{
			get { return _reg.GetValue<int>( "SearchStringMinLength", 4 ); }
			set { _reg.SetValue<int>( "SearchStringMinLength", value ); }
		}
		public int SearchStringMaxLength
		{
			get { return _reg.GetValue<int>( "SearchStringMaxLength", 50 ); }
			set { _reg.SetValue<int>( "SearchStringMaxLength", value ); }
		}
		public int SearchPermissions
		{
			get { return _reg.GetValue<int>( "SearchPermissions", (int)ViewPermissions.Everyone ); }
			set { _reg.SetValue<int>( "SearchPermissions", value ); }
		}
		public int ForumStatisticsCacheTimeout
		{
			get { return _reg.GetValue<int>( "ForumStatisticsCacheTimeout", 15 ); }
			set { _reg.SetValue<int>( "ForumStatisticsCacheTimeout", value ); }
		}
		// Ederon 12/18/2007
		public int PrivateMessageMaxRecipients
		{
			get { return _reg.GetValue<int>( "PrivateMessageMaxRecipients", 1 ); }
			set { _reg.SetValue<int>( "PrivateMessageMaxRecipients", value ); }
		}
		public int DisableNoFollowLinksAfterDay
		{
			get { return _reg.GetValue<int>( "DisableNoFollowLinksAfterDay", 0 ); }
			set { _reg.SetValue<int>( "DisableNoFollowLinksAfterDay", value ); }
		}
		// Ederon : 01/18/2007
		public int BoardForumListAllGuestCacheTimeout
		{
			get { return _reg.GetValue<int>( "BoardForumListAllGuestCacheTimeout", 1440 ); }
			set { _reg.SetValue<int>( "BoardForumListAllGuestCacheTimeout", value ); }
		}
		public int BoardModeratorsCacheTimeout
		{
			get { return _reg.GetValue<int>( "BoardModeratorsCacheTimeout", 1440 ); }
			set { _reg.SetValue<int>( "BoardModeratorsCacheTimeout", value ); }
		}
		public int BoardCategoriesCacheTimeout
		{
			get { return _reg.GetValue<int>( "BoardCategoriesCacheTimeout", 1440 ); }
			set { _reg.SetValue<int>( "BoardCategoriesCacheTimeout", value ); }
		}
		// Ederon : 02/07/2008
		public int ReplaceRulesCacheTimeout
		{
			get { return _reg.GetValue<int>( "ReplaceRulesCacheTimeout", 1440 ); }
			set { _reg.SetValue<int>( "ReplaceRulesCacheTimeout", value ); }
		}
		public int FirstPostCacheTimeout
		{
			get { return _reg.GetValue<int>( "FirstPostCacheTimeout", 120 ); }
			set { _reg.SetValue<int>( "FirstPostCacheTimeout", value ); }
		}

		public int MaxPostSize
		{
			get { return _reg.GetValue<int>( "MaxPostSize", Int16.MaxValue ); }
			set { _reg.SetValue<int>( "MaxPostSize", value ); }
		}

		public int MaxNumberOfAttachments
		{
			get { return _reg.GetValue<int>( "MaxNumberOfAttachments", 5 ); }
			set { _reg.SetValue<int>( "MaxNumberOfAttachments", value ); }
		}

		// Ederon : 02/17/2009
		public int PictureAttachmentDisplayTreshold
		{
			get { return _reg.GetValue<int>( "PictureAttachmentDisplayTreshold", 262144 ); }
			set { _reg.SetValue<int>( "PictureAttachmentDisplayTreshold", value ); }
		}

		public int ImageAttachmentResizeWidth
		{
			get { return _reg.GetValue<int>( "ImageAttachmentResizeWidth", 200 ); }
			set { _reg.SetValue<int>( "ImageAttachmentResizeWidth", value ); }
		}

		public int ImageAttachmentResizeHeight
		{
			get { return _reg.GetValue<int>( "ImageAttachmentResizeHeight", 200 ); }
			set { _reg.SetValue<int>( "ImageAttachmentResizeHeight", value ); }
		}

		public int ShoutboxShowMessageCount
		{
			get { return _reg.GetValue<int>( "ShoutboxShowMessageCount", 30 ); }
			set { _reg.SetValue<int>( "ShoutboxShowMessageCount", value ); }
		}
		//vzrus
		public int ActiveListTime
		{
			get { return _regBoard.GetValue<int>( "ActiveListTime", 5 ); }
			set { _regBoard.SetValue<int>( "ActiveListTime", value ); }
		}
		public int OnlineStatusCacheTimeout
		{
			get { return _reg.GetValue<int>( "OnlineStatusCacheTimeout", 100 ); }
			set { _reg.SetValue<int>( "OnlineStatusCacheTimeout", value ); }
		}

		#endregion

		#region boolean settings

		public bool EmailVerification
		{
			get { return _reg.GetValue<bool>( "EmailVerification", false ); }
			set { _reg.SetValue<bool>( "EmailVerification", value ); }
		}
		public bool UseFullTextSearch
		{
			get { return _reg.GetValue<bool>( "UseFullTextSearch", false ); }
			set { _reg.SetValue<bool>( "UseFullTextSearch", value ); }
		}
		public bool ShowMoved
		{
			get { return _reg.GetValue<bool>( "ShowMoved", true ); }
			set { _reg.SetValue<bool>( "ShowMoved", value ); }
		}
		public bool ShowGroups
		{
			get { return _reg.GetValue<bool>( "ShowGroups", true ); }
			set { _reg.SetValue<bool>( "ShowGroups", value ); }
		}
		public bool BlankLinks
		{
			get { return _reg.GetValue<bool>( "BlankLinks", false ); }
			set { _reg.SetValue<bool>( "BlankLinks", value ); }
		}
		public bool AllowUserTheme
		{
			get { return _reg.GetValue<bool>( "AllowUserTheme", false ); }
			set { _reg.SetValue<bool>( "AllowUserTheme", value ); }
		}
		public bool AllowUserLanguage
		{
			get { return _reg.GetValue<bool>( "AllowUserLanguage", false ); }
			set { _reg.SetValue<bool>( "AllowUserLanguage", value ); }
		}
		public bool AllowPMEmailNotification
		{
			get { return _reg.GetValue<bool>( "AllowPMEmailNotification", true ); }
			set { _reg.SetValue<bool>( "AllowPMEmailNotification", value ); }
		}
		public bool AvatarUpload
		{
			get { return _reg.GetValue<bool>( "AvatarUpload", false ); }
			set { _reg.SetValue<bool>( "AvatarUpload", value ); }
		}
		public bool AvatarRemote
		{
			get { return _reg.GetValue<bool>( "AvatarRemote", false ); }
			set { _reg.SetValue<bool>( "AvatarRemote", value ); }
		}
		//JoeOuts: added 8/17/09
		public bool AvatarGravatar
		{
			get { return _reg.GetValue<bool>( "AvatarGravatar", false ); }
			set { _reg.SetValue<bool>( "AvatarGravatar", value ); }
		}
		public bool AllowEmailChange
		{
			get { return _reg.GetValue<bool>( "AllowEmailChange", true ); }
			set { _reg.SetValue<bool>( "AllowEmailChange", value ); }
		}
		public bool AllowPasswordChange
		{
			get { return _reg.GetValue<bool>( "AllowPasswordChange", true ); }
			set { _reg.SetValue<bool>( "AllowPasswordChange", value ); }
		}
		public bool UseFileTable
		{
			get { return _reg.GetValue<bool>( "UseFileTable", false ); }
			set { _reg.SetValue<bool>( "UseFileTable", value ); }
		}
		public bool ShowRSSLink
		{
			get { return _reg.GetValue<bool>( "ShowRSSLink", true ); }
			set { _reg.SetValue<bool>( "ShowRSSLink", value ); }
		}
		public bool ShowPageGenerationTime
		{
			get { return _reg.GetValue<bool>( "ShowPageGenerationTime", true ); }
			set { _reg.SetValue<bool>( "ShowPageGenerationTime", value ); }
		}
		public bool ShowYAFVersion
		{
			get { return _reg.GetValue<bool>( "ShowYAFVersion", true ); }
			set { _reg.SetValue<bool>( "ShowYAFVersion", value ); }
		}
		public bool ShowForumJump
		{
			get { return _reg.GetValue<bool>( "ShowForumJump", true ); }
			set { _reg.SetValue<bool>( "ShowForumJump", value ); }
		}
		public bool AllowPrivateMessages
		{
			get { return _reg.GetValue<bool>( "AllowPrivateMessages", true ); }
			set { _reg.SetValue<bool>( "AllowPrivateMessages", value ); }
		}
		public bool AllowEmailSending
		{
			get { return _reg.GetValue<bool>( "AllowEmailSending", true ); }
			set { _reg.SetValue<bool>( "AllowEmailSending", value ); }
		}
		public bool AllowSignatures
		{
			get { return _reg.GetValue<bool>( "AllowSignatures", true ); }
			set { _reg.SetValue<bool>( "AllowSignatures", value ); }
		}
		public bool RemoveNestedQuotes
		{
			get { return _reg.GetValue<bool>( "RemoveNestedQuotes", false ); }
			set { _reg.SetValue<bool>( "RemoveNestedQuotes", value ); }
		}
		public bool DateFormatFromLanguage
		{
			get { return _reg.GetValue<bool>( "DateFormatFromLanguage", false ); }
			set { _reg.SetValue<bool>( "DateFormatFromLanguage", value ); }
		}
		public bool DisableRegistrations
		{
			get { return _reg.GetValue<bool>( "DisableRegistrations", false ); }
			set { _reg.SetValue<bool>( "DisableRegistrations", value ); }
		}
		public bool CreateNntpUsers
		{
			get { return _reg.GetValue<bool>( "CreateNntpUsers", false ); }
			set { _reg.SetValue<bool>( "CreateNntpUsers", value ); }
		}
		public bool ShowGroupsProfile
		{
			get { return _reg.GetValue<bool>( "ShowGroupsProfile", false ); }
			set { _reg.SetValue<bool>( "ShowGroupsProfile", value ); }
		}
		public bool PollVoteTiedToIP
		{
			get { return _reg.GetValue<bool>( "PollVoteTiedToIP", true ); }
			set { _reg.SetValue<bool>( "PollVoteTiedToIP", value ); }
		}

		public bool ShowAdsToSignedInUsers
		{
			get { return _reg.GetValue<bool>( "ShowAdsToSignedInUsers", true ); }
			set { _reg.SetValue<bool>( "ShowAdsToSignedInUsers", value ); }
		}

		public bool DisplayPoints
		{
			get { return _reg.GetValue<bool>( "DisplayPoints", false ); }
			set { _reg.SetValue<bool>( "DisplayPoints", value ); }
		}

		public bool ShowQuickAnswer
		{
			get { return _reg.GetValue<bool>( "ShowQuickAnswer", true ); }
			set { _reg.SetValue<bool>( "ShowQuickAnswer", value ); }
		}

		public bool ShowDeletedMessages
		{
			get { return _reg.GetValue<bool>( "ShowDeletedMessages", true ); }
			set { _reg.SetValue<bool>( "ShowDeletedMessages", value ); }
		}
		public bool ShowDeletedMessagesToAll
		{
			get { return _reg.GetValue<bool>( "ShowDeletedMessagesToAll", false ); }
			set { _reg.SetValue<bool>( "ShowDeletedMessagesToAll", value ); }
		}
		public bool ShowModeratorList
		{
			get { return _reg.GetValue<bool>( "ShowModeratorList", true ); }
			set { _reg.SetValue<bool>( "ShowModeratorList", value ); }
		}
		public bool EnableCaptchaForPost
		{
			get { return _reg.GetValue<bool>( "EnableCaptchaForPost", false ); }
			set { _reg.SetValue<bool>( "EnableCaptchaForPost", value ); }
		}

		public bool EnableCaptchaForRegister
		{
			get { return _reg.GetValue<bool>( "EnableCaptchaForRegister", false ); }
			set { _reg.SetValue<bool>( "EnableCaptchaForRegister", value ); }
		}

		// Ederon : 12/16/2007
		public bool EnableCaptchaForGuests
		{
			get { return _reg.GetValue<bool>( "EnableCaptchaForGuests", true ); }
			set { _reg.SetValue<bool>( "EnableCaptchaForGuests", value ); }
		}

		public bool UseNoFollowLinks
		{
			get { return _reg.GetValue<bool>( "UseNoFollowLinks", true ); }
			set { _reg.SetValue<bool>( "UseNoFollowLinks", value ); }
		}

		public bool DoUrlReferrerSecurityCheck
		{
			get { return _reg.GetValue<bool>( "DoUrlReferrerSecurityCheck", true ); }
			set { _reg.SetValue<bool>( "DoUrlReferrerSecurityCheck", value ); }
		}

		public bool EnableImageAttachmentResize
		{
			get { return _reg.GetValue<bool>( "EnableImageAttachmentResize", true ); }
			set { _reg.SetValue<bool>( "EnableImageAttachmentResize", value ); }
		}

		public bool ShowShoutbox
		{
			get { return _reg.GetValue<bool>( "ShowShoutbox", true ); }
			set { _reg.SetValue<bool>( "ShowShoutbox", value ); }
		}

		public bool AllowUserInfoCaching
		{
			get { return _reg.GetValue<bool>( "AllowUserInfoCaching", true ); }
			set { _reg.SetValue<bool>( "AllowUserInfoCaching", value ); }
		}

		public bool UseStyledNicks
		{
			get { return _reg.GetValue<bool>( "UseStyledNicks", false ); }
			set { _reg.SetValue<bool>( "UseStyledNicks", value ); }
		}

		public bool ShowUserOnlineStatusInPosts
		{
			get { return _reg.GetValue<bool>( "ShowUserOnlineStatusInPosts", false ); }
			set { _reg.SetValue<bool>( "ShowUserOnlineStatusInPosts", value ); }
		}
		#endregion

		#region string settings

		public string ForumEmail
		{
			get { return _reg.GetValue<string>( "ForumEmail", "" ); }
			set { _reg.SetValue<string>( "ForumEmail", value ); }
		}

		//JoeOuts: added 8/17/09
		public string GravatarRating
		{
			get { return _reg.GetValue<string>( "GravatarRating", "G" ); }
			set { _reg.SetValue<string>( "GravatarRating", value ); }
		}

		public string AcceptedHTML
		{
			get { return _reg.GetValue<string>( "AcceptedHTML", "br,hr,b,i,u,a,div,ol,ul,li,blockquote,img,span,p,em,strong,font,pre,h1,h2,h3,h4,h5,h6,address" ); }
			set { _reg.SetValue<string>( "AcceptedHTML", value.ToLower() ); }
		}

		public string AdPost
		{
			get { return _reg.GetValue<string>( "AdPost", null ); }
			set { _reg.SetValue<string>( "AdPost", value ); }
		}

		public string CustomLoginRedirectUrl
		{
			get { return _reg.GetValue<string>( "CustomLoginRedirectUrl", null ); }
			set { _reg.SetValue<string>( "CustomLoginRedirectUrl", value ); }
		}

		public string WebServiceToken
		{
			get { return _reg.GetValue<string>( "WebServiceToken", Guid.NewGuid().ToString() ); }
			set { _reg.SetValue<string>( "WebServiceToken", value ); }
		}

		public string SearchStringPattern
		{
			get { return _reg.GetValue<string>( "SearchStringPattern", ".*" ); }
			set { _reg.SetValue<string>( "SearchStringPattern", value ); }
		}

		/* Ederon : 6/16/2007 */
		public bool DisplayJoinDate
		{
			get { return _reg.GetValue<bool>( "DisplayJoinDate", true ); }
			set { _reg.SetValue<bool>( "DisplayJoinDate", value ); }
		}
		public bool ShowBrowsingUsers
		{
			get { return _reg.GetValue<bool>( "ShowBrowsingUsers", true ); }
			set { _reg.SetValue<bool>( "ShowBrowsingUsers", value ); }
		}
		public bool ShowMedals
		{
			get { return _reg.GetValue<bool>( "ShowMedals", true ); }
			set { _reg.SetValue<bool>( "ShowMedals", value ); }
		}
		public bool AllowPostToBlog
		{
			get { return _reg.GetValue<bool>( "AllowPostToBlog", false ); }
			set { _reg.SetValue<bool>( "AllowPostToBlog", value ); }
		}
		/* Mek : 8/18/2007 */
		public bool AllowReportAbuse
		{
			get { return _reg.GetValue<bool>( "AllowReportAbuse", true ); }
			set { _reg.SetValue<bool>( "AllowReportAbuse", value ); }
		}
		public bool AllowReportSpam
		{
			get { return _reg.GetValue<bool>( "AllowReportSpam", true ); }
			set { _reg.SetValue<bool>( "AllowReportSpam", value ); }
		}
		/* Ederon : 8/29/2007 */
		public bool AllowEmailTopic
		{
			get { return _reg.GetValue<bool>( "AllowEmailTopic", true ); }
			set { _reg.SetValue<bool>( "AllowEmailTopic", value ); }
		}

		/* Ederon : 12/9/2007 */
		public bool RequireLogin
		{
			get { return _reg.GetValue<bool>( "RequireLogin", false ); }
			set { _reg.SetValue<bool>( "RequireLogin", value ); }
		}
		/* Ederon : 12/14/2007 */
		public bool ShowActiveDiscussions
		{
			get { return _reg.GetValue<bool>( "ShowActiveDiscussions", true ); }
			set { _reg.SetValue<bool>( "ShowActiveDiscussions", value ); }
		}
		public bool ShowForumStatistics
		{
			get { return _reg.GetValue<bool>( "ShowForumStatistics", true ); }
			set { _reg.SetValue<bool>( "ShowForumStatistics", value ); }
		}
		public bool ShowRulesForRegistration
		{
			get { return _reg.GetValue<bool>( "ShowRulesForRegistration", true ); }
			set { _reg.SetValue<bool>( "ShowRulesForRegistration", value ); }
		}

		/* 6/16/2007 */
		/* Ederon : 7/14/2007 */
		public string UserBox
		{
			get { return _reg.GetValue<string>( "UserBox", Constants.UserBox.DisplayTemplateDefault ); }
			set { _reg.SetValue<string>( "UserBox", value ); }
		}
		public string UserBoxAvatar
		{
			get { return _reg.GetValue<string>( "UserBoxAvatar", "{0}<br clear=\"all\" />" ); }
			set { _reg.SetValue<string>( "UserBoxAvatar", value ); }
		}
		public string UserBoxMedals
		{
			get { return _reg.GetValue<string>( "UserBoxMedals", "{0}{1}<br clear=\"all\" />" ); }
			set { _reg.SetValue<string>( "UserBoxMedals", value ); }
		}
		public string UserBoxRankImage
		{
			get { return _reg.GetValue<string>( "UserBoxRankImage", "{0}<br clear=\"all\" />" ); }
			set { _reg.SetValue<string>( "UserBoxRankImage", value ); }
		}
		public string UserBoxRank
		{
			get { return _reg.GetValue<string>( "UserBoxRank", "{0}: {1}<br clear=\"all\" />" ); }
			set { _reg.SetValue<string>( "UserBoxRank", value ); }
		}
		public string UserBoxGroups
		{
			get { return _reg.GetValue<string>( "UserBoxGroups", "{0}: {1}<br clear=\"all\" />" ); }
			set { _reg.SetValue<string>( "UserBoxGroups", value ); }
		}
		public string UserBoxJoinDate
		{
			get { return _reg.GetValue<string>( "UserBoxJoinDate", "{0}: {1}<br />" ); }
			set { _reg.SetValue<string>( "UserBoxJoinDate", value ); }
		}
		public string UserBoxPosts
		{
			get { return _reg.GetValue<string>( "UserBoxPosts", "{0}: {1:N0}<br />" ); }
			set { _reg.SetValue<string>( "UserBoxPosts", value ); }
		}
		public string UserBoxPoints
		{
			get { return _reg.GetValue<string>( "UserBoxPoints", "{0}: {1:N0}<br />" ); }
			set { _reg.SetValue<string>( "UserBoxPoints", value ); }
		}
		public string UserBoxLocation
		{
			get { return _reg.GetValue<string>( "UserBoxLocation", "{0}: {1}<br />" ); }
			set { _reg.SetValue<string>( "UserBoxLocation", value ); }
		}
		/* 7/14/2007 */

		public string UserBoxThanksFrom
		{
			get { return _reg.GetValue<string>( "UserBoxThanksFrom", "{0}<br />" ); }
			set { _reg.SetValue<string>( "UserBoxThanksFrom", value ); }
		}

		public string UserBoxThanksTo
		{
			get { return _reg.GetValue<string>( "UserBoxThanksTo", "{0}<br />" ); }
			set { _reg.SetValue<string>( "UserBoxThanksTo", value ); }
		}

		#endregion
	}
}
