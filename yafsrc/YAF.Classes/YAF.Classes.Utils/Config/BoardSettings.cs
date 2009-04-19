/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
using System.Data;
using System.Collections;
using YAF.Classes.Data;

namespace YAF.Classes.Utils
{
	public class YafBoardSettings
	{
		/* Ederon : 6/16/2007 - conventions */

		private readonly DataRow _board;
		private readonly RegistryHash _reg, _regBoard;
		private readonly object _boardID;
		private readonly string _membershipAppName, _rolesAppName;

		public YafBoardSettings( object boardID )
		{
			_boardID = boardID;
			DataTable dt;
			// get the board table
			dt = YAF.Classes.Data.DB.board_list( boardID );
			if ( dt.Rows.Count == 0 )
				throw new Exception( "No data for board with id: " + boardID );
			_board = dt.Rows [0];
			_membershipAppName = ( _board ["MembershipAppName"].ToString() == "" ) ? System.Web.Security.Membership.ApplicationName : _board ["MembershipAppName"].ToString();
			_rolesAppName = ( _board ["RolesAppName"].ToString() == "" ) ? System.Web.Security.Roles.ApplicationName : _board ["RolesAppName"].ToString();

			_reg = new RegistryHash();
			_regBoard = new RegistryHash();

			// get all the registry values for the forum
			using ( dt = YAF.Classes.Data.DB.registry_list() )
			{
				// get all the registry settings into our hash table
				foreach ( DataRow dr in dt.Rows )
				{
					if ( dr ["Value"] == DBNull.Value )
					{
						_reg.Add( dr ["Name"].ToString().ToLower(), null );
					}
					else
					{
						_reg.Add( dr ["Name"].ToString().ToLower(), dr ["Value"] );
					}
				}
			}
			using ( dt = YAF.Classes.Data.DB.registry_list( null, boardID ) )
			{
				// get all the registry settings into our hash table
				foreach ( DataRow dr in dt.Rows )
				{
					if ( dr ["Value"] == DBNull.Value )
					{
						_regBoard.Add( dr ["Name"].ToString().ToLower(), null );
					}
					else
					{
						_regBoard.Add( dr ["Name"].ToString().ToLower(), dr ["Value"] );
					}
				}
			}
		}

		/// <summary>
		/// Saves the whole setting registry to the database.
		/// </summary>
		public void SaveRegistry()
		{
			// loop through all values and commit them to the DB
			foreach ( DictionaryEntry entry in _reg )
			{
				YAF.Classes.Data.DB.registry_save( entry.Key, entry.Value );
			}
			foreach ( DictionaryEntry entry in _regBoard )
			{
				YAF.Classes.Data.DB.registry_save( entry.Key, entry.Value, _boardID );
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
			get { return _board ["Name"].ToString(); }
		}
		public bool AllowThreaded
		{
			get { return SqlDataLayerConverter.VerifyBool( _board ["AllowThreaded"].ToString() ); }
		}
		public bool AllowThemedLogo
		{
			get { return _reg.GetValueBool( "AllowThemedLogo", false ); }
			set { _reg.SetValueBool( "AllowThemedLogo", value ); }
		}
		public int MaxUsers
		{
			get { return _regBoard.GetValueInt( "MaxUsers", 1 ); }
		}
		public DateTime MaxUsersWhen
		{
			get { return DateTime.Parse( _regBoard.GetValueString( "MaxUsersWhen", DateTime.Now.ToString() ) ); }
		}
		public string Theme
		{
			get { return _regBoard.GetValueString( "Theme", "yafpro.xml" ); }
			set { _regBoard.SetValueString( "Theme", value ); }
		}
		public string Language
		{
			get { return _regBoard.GetValueString( "Language", "english.xml" ); }
			set { _regBoard.SetValueString( "Language", value ); }
		}
		public int ShowTopicsDefault
		{
			get { return _regBoard.GetValueInt( "ShowTopicsDefault", 0 ); }
			set { _regBoard.SetValueInt( "ShowTopicsDefault", value ); }
		}
		public bool FileExtensionAreAllowed
		{
			get { return _regBoard.GetValueBool( "FileExtensionAreAllowed", true ); }
			set { _regBoard.SetValueBool( "FileExtensionAreAllowed", value ); }
		}


		// didn't know where else to put this :)
		public string SQLVersion
		{
			get { return Convert.ToString( _board ["SQLVersion"] ); }
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
		// int settings
		public int TimeZoneRaw
		{
			get { return _reg.GetValueInt( "TimeZone", 0 ); }
			set { _reg.SetValueInt( "TimeZone", value ); }
		}
		public int AvatarWidth
		{
			get { return _reg.GetValueInt( "AvatarWidth", 50 ); }
			set { _reg.SetValueInt( "AvatarWidth", value ); }
		}
		public int AvatarHeight
		{
			get { return _reg.GetValueInt( "AvatarHeight", 80 ); }
			set { _reg.SetValueInt( "AvatarHeight", value ); }
		}
		public int AvatarSize
		{
			get { return _reg.GetValueInt( "AvatarSize", 50000 ); }
			set { _reg.SetValueInt( "AvatarSize", value ); }
		}
		public int MaxFileSize
		{
			get { return _reg.GetValueInt( "MaxFileSize", 0 ); }
			set { _reg.SetValueInt( "MaxFileSize", value ); }
		}
		public int SmiliesColumns
		{
			get { return _reg.GetValueInt( "SmiliesColumns", 3 ); }
			set { _reg.SetValueInt( "SmiliesColumns", value ); }
		}
		public int SmiliesPerRow
		{
			get { return _reg.GetValueInt( "SmiliesPerRow", 6 ); }
			set { _reg.SetValueInt( "SmiliesPerRow", value ); }
		}
		public int LockPosts
		{
			get { return _reg.GetValueInt( "LockPosts", 0 ); }
			set { _reg.SetValueInt( "LockPosts", value ); }
		}
		public int PostsPerPage
		{
			get { return _reg.GetValueInt( "PostsPerPage", 20 ); }
			set { _reg.SetValueInt( "PostsPerPage", value ); }
		}
		public int TopicsPerPage
		{
			get { return _reg.GetValueInt( "TopicsPerPage", 15 ); }
			set { _reg.SetValueInt( "TopicsPerPage", value ); }
		}
		public int ForumEditor
		{
			get { return _reg.GetValueInt( "ForumEditor", 1 ); }
			set { _reg.SetValueInt( "ForumEditor", value ); }
		}
		public int PostFloodDelay
		{
			get { return _reg.GetValueInt( "PostFloodDelay", 30 ); }
			set { _reg.SetValueInt( "PostFloodDelay", value ); }
		}
		public int EditTimeOut
		{
			get { return _reg.GetValueInt( "EditTimeOut", 30 ); }
			set { _reg.SetValueInt( "EditTimeOut", value ); }
		}
		public int CaptchaSize
		{
			get { return _reg.GetValueInt( "CaptchaSize", 5 ); }
			set { _reg.SetValueInt( "CaptchaSize", value ); }
		}
		// Ederon : 11/21/2007
		public int ProfileViewPermissions
		{
			get { return _reg.GetValueInt( "ProfileViewPermission", ( int )ViewPermissions.RegisteredUsers ); }
			set { _reg.SetValueInt( "ProfileViewPermission", value ); }
		}
		public int ReturnSearchMax
		{
			get { return _reg.GetValueInt( "ReturnSearchMax", 100 ); }
			set { _reg.SetValueInt( "ReturnSearchMax", value ); }
		}
		// Ederon : 12/9/2007
		public int ActiveUsersViewPermissions
		{
			get { return _reg.GetValueInt( "ActiveUsersViewPermissions", ( int )ViewPermissions.RegisteredUsers ); }
			set { _reg.SetValueInt( "ActiveUsersViewPermissions", value ); }
		}
		public int MembersListViewPermissions
		{
			get { return _reg.GetValueInt( "MembersListViewPermissions", ( int )ViewPermissions.RegisteredUsers ); }
			set { _reg.SetValueInt( "MembersListViewPermissions", value ); }
		}
		// Ederon : 12/14/2007
		public int ActiveDiscussionsCount
		{
			get { return _reg.GetValueInt( "ActiveDiscussionsCount", 5 ); }
			set { _reg.SetValueInt( "ActiveDiscussionsCount", value ); }
		}
		public int ActiveDiscussionsCacheTimeout
		{
			get { return _reg.GetValueInt( "ActiveDiscussionsCacheTimeout", 1 ); }
			set { _reg.SetValueInt( "ActiveDiscussionsCacheTimeout", value ); }
		}
		public int SearchStringMinLength
		{
			get { return _reg.GetValueInt( "SearchStringMinLength", 4 ); }
			set { _reg.SetValueInt( "SearchStringMinLength", value ); }
		}
		public int SearchPermissions
		{
			get { return _reg.GetValueInt( "SearchPermissions", ( int )ViewPermissions.Everyone ); }
			set { _reg.SetValueInt( "SearchPermissions", value ); }
		}
		public int ForumStatisticsCacheTimeout
		{
			get { return _reg.GetValueInt( "ForumStatisticsCacheTimeout", 15 ); }
			set { _reg.SetValueInt( "ForumStatisticsCacheTimeout", value ); }
		}
		// Ederon 12/18/2007
		public int PrivateMessageMaxRecipients
		{
			get { return _reg.GetValueInt( "PrivateMessageMaxRecipients", 1 ); }
			set { _reg.SetValueInt( "PrivateMessageMaxRecipients", value ); }
		}
		public int MaxPrivateMessagesPerUser
		{
			get { return _reg.GetValueInt( "MaxPrivateMessagesPerUser", 30 ); }
			set { _reg.SetValueInt( "MaxPrivateMessagesPerUser", value ); }
		}
		public int DisableNoFollowLinksAfterDay
		{
			get { return _reg.GetValueInt( "DisableNoFollowLinksAfterDay", 0 ); }
			set { _reg.GetValueInt( "DisableNoFollowLinksAfterDay", value ); }
		}
		// Ederon : 01/18/2007
		public int BoardModeratorsCacheTimeout
		{
			get { return _reg.GetValueInt( "BoardModeratorsCacheTimeout", 1440 ); }
			set { _reg.SetValueInt( "BoardModeratorsCacheTimeout", value ); }
		}
		public int BoardCategoriesCacheTimeout
		{
			get { return _reg.GetValueInt( "BoardCategoriesCacheTimeout", 1440 ); }
			set { _reg.SetValueInt( "BoardCategoriesCacheTimeout", value ); }
		}
		// Ederon : 02/07/2008
		public int ReplaceRulesCacheTimeout
		{
			get { return _reg.GetValueInt("ReplaceRulesCacheTimeout", 1440); }
			set { _reg.SetValueInt("ReplaceRulesCacheTimeout", value); }
		}

		public int MaxPostSize
		{
			get { return _reg.GetValueInt( "MaxPostSize", Int16.MaxValue ); }
			set { _reg.SetValueInt( "MaxPostSize", value ); }
		}
		// Ederon : 02/17/2009
		public int PictureAttachmentDisplayTreshold
		{
			get { return _reg.GetValueInt("PictureAttachmentDisplayTreshold", 262144); }
			set { _reg.SetValueInt("PictureAttachmentDisplayTreshold", value); }
		}


		// boolean settings
		public bool EmailVerification
		{
			get { return _reg.GetValueBool( "EmailVerification", false ); }
			set { _reg.SetValueBool( "EmailVerification", value ); }
		}
		public bool UseFullTextSearch
		{
			get { return _reg.GetValueBool( "UseFullTextSearch", false ); }
			set { _reg.SetValueBool( "UseFullTextSearch", value ); }
		}
		public bool ShowMoved
		{
			get { return _reg.GetValueBool( "ShowMoved", true ); }
			set { _reg.SetValueBool( "ShowMoved", value ); }
		}
		public bool ShowGroups
		{
			get { return _reg.GetValueBool( "ShowGroups", true ); }
			set { _reg.SetValueBool( "ShowGroups", value ); }
		}
		public bool BlankLinks
		{
			get { return _reg.GetValueBool( "BlankLinks", false ); }
			set { _reg.SetValueBool( "BlankLinks", value ); }
		}
		public bool AllowUserTheme
		{
			get { return _reg.GetValueBool( "AllowUserTheme", false ); }
			set { _reg.SetValueBool( "AllowUserTheme", value ); }
		}
		public bool AllowUserLanguage
		{
			get { return _reg.GetValueBool( "AllowUserLanguage", false ); }
			set { _reg.SetValueBool( "AllowUserLanguage", value ); }
		}
		public bool AllowPMEmailNotification
		{
			get { return _reg.GetValueBool( "AllowPMEmailNotification", true ); }
			set { _reg.SetValueBool( "AllowPMEmailNotification", value ); }
		}
		public bool AvatarUpload
		{
			get { return _reg.GetValueBool( "AvatarUpload", false ); }
			set { _reg.SetValueBool( "AvatarUpload", value ); }
		}
		public bool AvatarRemote
		{
			get { return _reg.GetValueBool( "AvatarRemote", false ); }
			set { _reg.SetValueBool( "AvatarRemote", value ); }
		}
		public bool AllowLoginAndLogoff
		{
			get { return _reg.GetValueBool( "AllowLoginAndLogoff", true ); }
			set { _reg.SetValueBool( "AllowLoginAndLogoff", value ); }
		}
        public bool AllowEmailChange
        {
            get { return _reg.GetValueBool("AllowEmailChange", true); }
            set { _reg.SetValueBool("AllowEmailChange", value); }
        }
		public bool UseFileTable
		{
			get { return _reg.GetValueBool( "UseFileTable", false ); }
			set { _reg.SetValueBool( "UseFileTable", value ); }
		}
		public bool ShowRSSLink
		{
			get { return _reg.GetValueBool( "ShowRSSLink", true ); }
			set { _reg.SetValueBool( "ShowRSSLink", value ); }
		}
		public bool ShowPageGenerationTime
		{
			get { return _reg.GetValueBool( "ShowPageGenerationTime", true ); }
			set { _reg.SetValueBool( "ShowPageGenerationTime", value ); }
		}
		public bool ShowYAFVersion
		{
			get { return _reg.GetValueBool( "ShowYAFVersion", true ); }
			set { _reg.SetValueBool( "ShowYAFVersion", value ); }
		}
		public bool ShowForumJump
		{
			get { return _reg.GetValueBool( "ShowForumJump", true ); }
			set { _reg.SetValueBool( "ShowForumJump", value ); }
		}
		public bool AllowPrivateMessages
		{
			get { return _reg.GetValueBool( "AllowPrivateMessages", true ); }
			set { _reg.SetValueBool( "AllowPrivateMessages", value ); }
		}
		public bool AllowEmailSending
		{
			get { return _reg.GetValueBool( "AllowEmailSending", true ); }
			set { _reg.SetValueBool( "AllowEmailSending", value ); }
		}
		public bool AllowSignatures
		{
			get { return _reg.GetValueBool( "AllowSignatures", true ); }
			set { _reg.SetValueBool( "AllowSignatures", value ); }
		}
		public bool RemoveNestedQuotes
		{
			get { return _reg.GetValueBool( "RemoveNestedQuotes", false ); }
			set { _reg.SetValueBool( "RemoveNestedQuotes", value ); }
		}
		public bool DateFormatFromLanguage
		{
			get { return _reg.GetValueBool( "DateFormatFromLanguage", false ); }
			set { _reg.SetValueBool( "DateFormatFromLanguage", value ); }
		}
		public bool DisableRegistrations
		{
			get { return _reg.GetValueBool( "DisableRegistrations", false ); }
			set { _reg.SetValueBool( "DisableRegistrations", value ); }
		}
		public bool CreateNntpUsers
		{
			get { return _reg.GetValueBool( "CreateNntpUsers", false ); }
			set { _reg.SetValueBool( "CreateNntpUsers", value ); }
		}
		public bool ShowGroupsProfile
		{
			get { return _reg.GetValueBool( "ShowGroupsProfile", false ); }
			set { _reg.SetValueBool( "ShowGroupsProfile", value ); }
		}
		public bool PollVoteTiedToIP
		{
			get { return _reg.GetValueBool( "PollVoteTiedToIP", true ); }
			set { _reg.SetValueBool( "PollVoteTiedToIP", value ); }
		}

		public bool ShowAdsToSignedInUsers
		{
			get { return _reg.GetValueBool( "ShowAdsToSignedInUsers", true ); }
			set { _reg.SetValueBool( "ShowAdsToSignedInUsers", value ); }
		}

		public bool DisplayPoints
		{
			get { return _reg.GetValueBool( "DisplayPoints", false ); }
			set { _reg.SetValueBool( "DisplayPoints", value ); }
		}

		public bool ShowQuickAnswer
		{
			get { return _reg.GetValueBool( "ShowQuickAnswer", true ); }
			set { _reg.SetValueBool( "ShowQuickAnswer", value ); }
		}

		public bool ShowDeletedMessages
		{
			get { return _reg.GetValueBool( "ShowDeletedMessages", true ); }
			set { _reg.SetValueBool( "ShowDeletedMessages", value ); }
		}
		public bool ShowDeletedMessagesToAll
		{
			get { return _reg.GetValueBool( "ShowDeletedMessagesToAll", false ); }
			set { _reg.SetValueBool( "ShowDeletedMessagesToAll", value ); }
		}
		public bool ShowModeratorList
		{
			get { return _reg.GetValueBool( "ShowModeratorList", true ); }
			set { _reg.SetValueBool( "ShowModeratorList", value ); }
		}
		public bool EnableCaptchaForPost
		{
			get { return _reg.GetValueBool( "EnableCaptchaForPost", false ); }
			set { _reg.SetValueBool( "EnableCaptchaForPost", value ); }
		}

		public bool EnableCaptchaForRegister
		{
			get { return _reg.GetValueBool( "EnableCaptchaForRegister", false ); }
			set { _reg.SetValueBool( "EnableCaptchaForRegister", value ); }
		}

		// Ederon : 12/16/2007
		public bool EnableCaptchaForGuests
		{
			get { return _reg.GetValueBool( "EnableCaptchaForGuests", true ); }
			set { _reg.SetValueBool( "EnableCaptchaForGuests", value ); }
		}

		public bool UseNoFollowLinks
		{
			get { return _reg.GetValueBool( "UseNoFollowLinks", true ); }
			set { _reg.SetValueBool( "UseNoFollowLinks", value ); }
		}

		public bool DoUrlReferrerSecurityCheck
		{
			get { return _reg.GetValueBool( "DoUrlReferrerSecurityCheck", true ); }
			set { _reg.SetValueBool( "DoUrlReferrerSecurityCheck", value ); }
		}

		// string settings
		public string ForumEmail
		{
			get { return _reg.GetValueString( "ForumEmail", "" ); }
			set { _reg.SetValueString( "ForumEmail", value ); }
		}

        // Wes: Removed
        //public string SmtpServer
        //{
        //    get { return _reg.GetValueString( "SmtpServer", null ); }
        //    set { _reg.SetValueString( "SmtpServer", value ); }
        //}
        //public string SmtpUserName
        //{
        //    get { return _reg.GetValueString( "SmtpUserName", null ); }
        //    set { _reg.SetValueString( "SmtpUserName", value ); }
        //}
        //public string SmtpUserPass
        //{
        //    get { return _reg.GetValueString( "SmtpUserPass", null ); }
        //    set { _reg.SetValueString( "SmtpUserPass", value ); }
        //}
		public string AcceptedHTML
		{
			get { return _reg.GetValueString( "AcceptedHTML", "br,hr,b,i,u,a,div,ol,ul,li,blockquote,img,span,p,em,strong,font,pre,h1,h2,h3,h4,h5,h6,address" ); }
			set { _reg.SetValueString( "AcceptedHTML", value.ToLower() ); }
		}
		public string AdPost
		{
			get { return _reg.GetValueString( "AdPost", null ); }
			set { _reg.SetValueString( "AdPost", value ); }
		}

        // Wes:Removed
        ///* Ederon : 9/9/2007 */
        //public string SmtpServerPort
        //{
        //    get { return _reg.GetValue<string>( "SmtpServerPort", null ); }
        //    set { _reg.SetValue<string>( "SmtpServerPort", value ); }
        //}
		// Ederon : 12/14/2007
		public string SearchStringPattern
		{
			get { return _reg.GetValueString( "SearchStringPattern", ".*" ); }
			set { _reg.GetValueString( "SearchStringPattern", value ); }
		}


		/* Ederon : 6/16/2007 */
		public bool DisplayJoinDate
		{
			get { return _reg.GetValueBool( "DisplayJoinDate", true ); }
			set { _reg.SetValue<bool>( "DisplayJoinDate", value ); }
		}
		public bool ShowBrowsingUsers
		{
			get { return _reg.GetValueBool( "ShowBrowsingUsers", true ); }
			set { _reg.SetValue<bool>( "ShowBrowsingUsers", value ); }
		}
		public bool ShowMedals
		{
			get { return _reg.GetValueBool( "ShowMedals", true ); }
			set { _reg.SetValue<bool>( "ShowMedals", value ); }
		}
		public bool AllowPostToBlog
		{
			get { return _reg.GetValueBool( "AllowPostToBlog", false ); }
			set { _reg.SetValue<bool>( "AllowPostToBlog", value ); }
		}
		/* Mek : 8/18/2007 */
		public bool AllowReportAbuse
		{
			get { return _reg.GetValueBool( "AllowReportAbuse", true ); }
			set { _reg.SetValue<bool>( "AllowReportAbuse", value ); }
		}
		public bool AllowReportSpam
		{
			get { return _reg.GetValueBool( "AllowReportSpam", true ); }
			set { _reg.SetValue<bool>( "AllowReportSpam", value ); }
		}
		/* Ederon : 8/29/2007 */
		public bool AllowEmailTopic
		{
			get { return _reg.GetValueBool( "AllowEmailTopic", true ); }
			set { _reg.SetValue<bool>( "AllowEmailTopic", value ); }
		}

        // Wes: Removed
        ///* Ederon : 9/9/2007 */
        //public bool SmtpServerSsl
        //{
        //    get { return _reg.GetValueBool( "SmtpServerSsl", false ); }
        //    set { _reg.SetValue<bool>( "SmtpServerSsl", value ); }
        //}
		/* Ederon : 12/9/2007 */
		public bool RequireLogin
		{
			get { return _reg.GetValueBool( "RequireLogin", false ); }
			set { _reg.SetValue<bool>( "RequireLogin", value ); }
		}
		/* Ederon : 12/14/2007 */
		public bool ShowActiveDiscussions
		{
			get { return _reg.GetValueBool( "ShowActiveDiscussions", true ); }
			set { _reg.SetValue<bool>( "ShowActiveDiscussions", value ); }
		}
		public bool ShowForumStatistics
		{
			get { return _reg.GetValueBool( "ShowForumStatistics", true ); }
			set { _reg.SetValue<bool>( "ShowForumStatistics", value ); }
		}
		public bool ShowRulesForRegistration
		{
			get { return _reg.GetValueBool( "ShowRulesForRegistration", true ); }
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
	}

	public class RegistryHash : System.Collections.Hashtable
	{
		/* Ederon : 6/16/2007 - conventions */

		// helper class functions
		public int GetValueInt( string name, int defaultValue )
		{
			if ( this [name.ToLower()] == null ) return defaultValue;
            return SqlDataLayerConverter.VerifyInt32(this[name.ToLower()]);
		}
		public void SetValueInt( string name, int value )
		{
			this [name.ToLower()] = Convert.ToString( value );
		}
		public bool GetValueBool( string name, bool defaultValue )
		{
			if ( this [name.ToLower()] == null ) return defaultValue;

			int i;
			if (int.TryParse(this[name.ToLower()].ToString(), out i))
                return SqlDataLayerConverter.VerifyBool(i);
            else return SqlDataLayerConverter.VerifyBool(this[name.ToLower()]);
		}
		public void SetValueBool( string name, bool value )
		{
			this [name.ToLower()] = Convert.ToString( Convert.ToInt32( value ) );
		}
		public string GetValueString( string name, string defaultValue )
		{
			if ( this [name.ToLower()] == null ) return defaultValue;
			return Convert.ToString( this [name.ToLower()] );
		}
		public void SetValueString( string name, string value )
		{
			this [name.ToLower()] = value;
		}

		/* Ederon : 6/16/2007 */
		public T GetValue<T>( string name, T defaultValue )
		{
			if ( this [name.ToLower()] == null ) return defaultValue;
			return ( T )Convert.ChangeType( this [name.ToLower()], typeof( T ) );
		}
		public void SetValue<T>( string name, T value )
		{
			this [name.ToLower()] = Convert.ToString( value );
		}
		/* 6/16/2007 */
	}

}
