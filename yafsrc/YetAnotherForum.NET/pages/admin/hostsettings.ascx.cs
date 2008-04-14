/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for settings.
	/// </summary>
	public partial class hostsettings : YAF.Classes.Base.AdminPage
	{
		protected System.Web.UI.WebControls.CheckBox AllowRichEditX;
		protected System.Web.UI.HtmlControls.HtmlTableRow Tr1;
		protected CheckBox AllowHTMLX;

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !PageContext.IsHostAdmin )
				YafBuildLink.AccessDenied();

			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin ) );
				PageLinks.AddLink( "Host Settings", "" );

				// Jaben 9/21: Removed localization. Admin isn't localized.
				this.SettingsTab.HeaderText = "Host Settings";
				this.FeaturesTab.HeaderText = "Features";
				this.DisplayTab.HeaderText = "Show/Display Items";
				this.AdvertsTab.HeaderText = "Adverts";
				this.EditorTab.HeaderText = "Editors";
				this.PermissionTab.HeaderText = "Permission";
				this.SMPTTab.HeaderText = "SMTP";
				this.TemplatesTab.HeaderText = "Templates";
				this.AvatarsTab.HeaderText = "Avatars";
				this.CacheTab.HeaderText = "Cache";

				BindData();
			}

			// Ederon : 7/1/2007
			// set widths manually since ASP.NET "forgets" to do it for browsers other then IE
			General.AddStyleAttributeWidth( SmiliesPerRow, "25px" );
			General.AddStyleAttributeWidth( SmiliesColumns, "25px" );			
			General.AddStyleAttributeWidth( ForumEmailEdit, "200px" );
			General.AddStyleAttributeWidth( ForumSmtpServer, "200px" );
			General.AddStyleAttributeWidth( ForumSmtpUserName, "200px" );
			General.AddStyleAttributeWidth( AcceptedHTML, "200px" );
			General.AddStyleAttributeWidth( AcceptedHTML, "200px" );
			General.AddStyleAttributeWidth( DisableNoFollowLinksAfterDay, "100px" );

			// Ederon : 7/14/2007
			General.AddStyleAttributeSize( UserBox, "350px", "100px" );
			General.AddStyleAttributeWidth( UserBoxAvatar, "200px" );
			General.AddStyleAttributeWidth( UserBoxMedals, "200px" );
			General.AddStyleAttributeWidth( UserBoxGroups, "200px" );
			General.AddStyleAttributeWidth( UserBoxJoinDate, "200px" );
			General.AddStyleAttributeWidth( UserBoxLocation, "200px" );
			General.AddStyleAttributeWidth( UserBoxPosts, "200px" );
			General.AddStyleAttributeWidth( UserBoxPoints, "200px" );
			General.AddStyleAttributeWidth( UserBoxRank, "200px" );
			General.AddStyleAttributeWidth( UserBoxRankImage, "200px" );

			// Ederon : 9/9/2007
			General.AddStyleAttributeWidth( ForumSmtpServerPort, "30px" );
			General.AddStyleAttributeSize( AdPost, "400px", "150px" );

			// CheckCache
			CheckCache();
		}

		private void BindData()
		{
			TimeZones.DataSource = YafStaticData.TimeZones();
			ForumEditorList.DataSource = YAF.Editor.EditorHelper.GetEditorsTable();

			DataBind();

			// grab all the settings form the current board settings class
			SQLVersion.Text = PageContext.BoardSettings.SQLVersion;
			TimeZones.Items.FindByValue( PageContext.BoardSettings.TimeZoneRaw.ToString() ).Selected = true;
			ForumEditorList.Items.FindByValue( PageContext.BoardSettings.ForumEditor.ToString() ).Selected = true;
			ForumSmtpServer.Text = PageContext.BoardSettings.SmtpServer;
			ForumSmtpUserName.Text = PageContext.BoardSettings.SmtpUserName;
			// since SMTP password is masked, we need to set it using other method
			//ForumSmtpUserPass.Text = PageContext.BoardSettings.SmtpUserPass;
			ForumSmtpUserPass.Attributes.Add("value", PageContext.BoardSettings.SmtpUserPass);
			ForumEmailEdit.Text = PageContext.BoardSettings.ForumEmail;
			EmailVerification.Checked = PageContext.BoardSettings.EmailVerification;
			ShowMoved.Checked = PageContext.BoardSettings.ShowMoved;
			BlankLinks.Checked = PageContext.BoardSettings.BlankLinks;
			ShowGroupsX.Checked = PageContext.BoardSettings.ShowGroups;
			AvatarWidth.Text = PageContext.BoardSettings.AvatarWidth.ToString();
			AvatarHeight.Text = PageContext.BoardSettings.AvatarHeight.ToString();
			AvatarUpload.Checked = PageContext.BoardSettings.AvatarUpload;
			AvatarRemote.Checked = PageContext.BoardSettings.AvatarRemote;
			AvatarSize.Text = ( PageContext.BoardSettings.AvatarSize != 0 ) ? PageContext.BoardSettings.AvatarSize.ToString() : "";
			AllowUserThemeX.Checked = PageContext.BoardSettings.AllowUserTheme;
			AllowUserLanguageX.Checked = PageContext.BoardSettings.AllowUserLanguage;
			UseFileTableX.Checked = PageContext.BoardSettings.UseFileTable;
			ShowRSSLinkX.Checked = PageContext.BoardSettings.ShowRSSLink;
			ShowForumJumpX.Checked = PageContext.BoardSettings.ShowForumJump;
			AllowPrivateMessagesX.Checked = PageContext.BoardSettings.AllowPrivateMessages;
			AllowEmailSendingX.Checked = PageContext.BoardSettings.AllowEmailSending;
			AllowSignaturesX.Checked = PageContext.BoardSettings.AllowSignatures;
			RemoveNestedQuotesX.Checked = PageContext.BoardSettings.RemoveNestedQuotes;
			MaxFileSize.Text = ( PageContext.BoardSettings.MaxFileSize != 0 ) ? PageContext.BoardSettings.MaxFileSize.ToString() : "";
			SmiliesColumns.Text = PageContext.BoardSettings.SmiliesColumns.ToString();
			SmiliesPerRow.Text = PageContext.BoardSettings.SmiliesPerRow.ToString();
			LockPosts.Text = PageContext.BoardSettings.LockPosts.ToString();
			PostsPerPage.Text = PageContext.BoardSettings.PostsPerPage.ToString();
			TopicsPerPage.Text = PageContext.BoardSettings.TopicsPerPage.ToString();
			DateFormatFromLanguage.Checked = PageContext.BoardSettings.DateFormatFromLanguage;
			AcceptedHTML.Text = PageContext.BoardSettings.AcceptedHTML;
			DisableRegistrations.Checked = PageContext.BoardSettings.DisableRegistrations;
			CreateNntpUsers.Checked = PageContext.BoardSettings.CreateNntpUsers;
			ShowGroupsProfile.Checked = PageContext.BoardSettings.ShowGroupsProfile;
			PostFloodDelay.Text = PageContext.BoardSettings.PostFloodDelay.ToString();
			PollVoteTiedToIPX.Checked = PageContext.BoardSettings.PollVoteTiedToIP;
			AllowPMNotifications.Checked = PageContext.BoardSettings.AllowPMEmailNotification;
			ShowPageGenerationTime.Checked = PageContext.BoardSettings.ShowPageGenerationTime;
			AdPost.Text = PageContext.BoardSettings.AdPost;
			ShowAdsToSignedInUsers.Checked = PageContext.BoardSettings.ShowAdsToSignedInUsers;
			DisplayPoints.Checked = PageContext.BoardSettings.DisplayPoints;
			ShowQuickAnswerX.Checked = PageContext.BoardSettings.ShowQuickAnswer;
			ShowDeletedMessages.Checked = PageContext.BoardSettings.ShowDeletedMessages;
			ShowDeletedMessagesToAll.Checked = PageContext.BoardSettings.ShowDeletedMessagesToAll;
			EditTimeOut.Text = PageContext.BoardSettings.EditTimeOut.ToString();
			ShowYAFVersion.Checked = PageContext.BoardSettings.ShowYAFVersion;
			UseNoFollowLinks.Checked = PageContext.BoardSettings.UseNoFollowLinks;
			DisableNoFollowLinksAfterDay.Text = PageContext.BoardSettings.DisableNoFollowLinksAfterDay.ToString();
			ShowRulesForRegistrationX.Checked = PageContext.BoardSettings.ShowRulesForRegistration;

			// Ederon : 7/1/2007 added
			ShowBrowsingUsers.Checked = PageContext.BoardSettings.ShowBrowsingUsers;
			DisplayJoinDate.Checked = PageContext.BoardSettings.DisplayJoinDate;
			ShowMedals.Checked = PageContext.BoardSettings.ShowMedals;
			AllowPostToBlog.Checked = PageContext.BoardSettings.AllowPostToBlog;

			// Mek : 08/18/2007 Added
			AllowReportAbuse.Checked = PageContext.BoardSettings.AllowReportAbuse;
			AllowReportSpam.Checked = PageContext.BoardSettings.AllowReportSpam;

			// Ederon : 8/29/2007 added
			AllowEmailTopic.Checked = PageContext.BoardSettings.AllowEmailTopic;

			// Ederon : 7/14/2007 added
			UserBox.Text = PageContext.BoardSettings.UserBox;
			UserBoxAvatar.Text = PageContext.BoardSettings.UserBoxAvatar;
			UserBoxMedals.Text = PageContext.BoardSettings.UserBoxMedals;
			UserBoxGroups.Text = PageContext.BoardSettings.UserBoxGroups;
			UserBoxJoinDate.Text = PageContext.BoardSettings.UserBoxJoinDate;
			UserBoxLocation.Text = PageContext.BoardSettings.UserBoxLocation;
			UserBoxPoints.Text = PageContext.BoardSettings.UserBoxPoints;
			UserBoxPosts.Text = PageContext.BoardSettings.UserBoxPosts;
			UserBoxRank.Text = PageContext.BoardSettings.UserBoxRank;
			UserBoxRankImage.Text = PageContext.BoardSettings.UserBoxRankImage;

			// Ederon : 9/9/2007 added
			ForumSmtpServerPort.Text = PageContext.BoardSettings.SmtpServerPort;
			ForumSmtpServerSsl.Checked = PageContext.BoardSettings.SmtpServerSsl;

			// Ederon : 11/21/2007 added
			ProfileViewPermissions.SelectedIndex = PageContext.BoardSettings.ProfileViewPermissions;

			// Ederon : 12/9/2007 added
			RequireLogin.Checked = PageContext.BoardSettings.RequireLogin;
			MembersListViewPermissions.SelectedIndex = PageContext.BoardSettings.MembersListViewPermissions;
			ActiveUsersViewPermissions.SelectedIndex = PageContext.BoardSettings.ActiveUsersViewPermissions;

			// Ederon : 12/14/2007 added
			ShowForumStatistics.Checked = PageContext.BoardSettings.ShowForumStatistics;
			ShowActiveDiscussions.Checked = PageContext.BoardSettings.ShowActiveDiscussions;
			ActiveDiscussionsCount.Text = PageContext.BoardSettings.ActiveDiscussionsCount.ToString();
			SearchStringMinLength.Text = PageContext.BoardSettings.SearchStringMinLength.ToString();
			SearchStringPattern.Text = PageContext.BoardSettings.SearchStringPattern;
			SearchPermissions.SelectedIndex = PageContext.BoardSettings.SearchPermissions;

			// Ederon : 12/18/2007 added
			MaxPrivateMessagesPerUser.Text = PageContext.BoardSettings.MaxPrivateMessagesPerUser.ToString();
			PrivateMessageMaxRecipients.Text = PageContext.BoardSettings.PrivateMessageMaxRecipients.ToString();

			// Caching
			ForumStatisticsCacheTimeout.Text = PageContext.BoardSettings.ForumStatisticsCacheTimeout.ToString();
			ActiveDiscussionsCacheTimeout.Text = PageContext.BoardSettings.ActiveDiscussionsCacheTimeout.ToString();
			BoardModeratorsCacheTimeout.Text = PageContext.BoardSettings.BoardModeratorsCacheTimeout.ToString();
			BoardCategoriesCacheTimeout.Text = PageContext.BoardSettings.BoardCategoriesCacheTimeout.ToString();
			ReplaceRulesCacheTimeout.Text = PageContext.BoardSettings.ReplaceRulesCacheTimeout.ToString();

			// Captcha Settings
			CaptchaSize.Text = PageContext.BoardSettings.CaptchaSize.ToString();
			EnableCaptchaForPost.Checked = PageContext.BoardSettings.EnableCaptchaForPost;
			EnableCaptchaForRegister.Checked = PageContext.BoardSettings.EnableCaptchaForRegister;
			EnableCaptchaForGuests.Checked = PageContext.BoardSettings.EnableCaptchaForGuests;

			// Search Settings
			ReturnSearchMax.Text = PageContext.BoardSettings.ReturnSearchMax.ToString();
			UseFullTextSearch.Checked = PageContext.BoardSettings.UseFullTextSearch;
		}

		protected void Save_Click( object sender, System.EventArgs e )
		{
			// write all the settings back to the settings class
			PageContext.BoardSettings.TimeZoneRaw = Convert.ToInt32( TimeZones.SelectedItem.Value );
			PageContext.BoardSettings.ForumEditor = Convert.ToInt32( ForumEditorList.SelectedItem.Value );
			PageContext.BoardSettings.SmtpServer = ForumSmtpServer.Text;
			PageContext.BoardSettings.SmtpUserName = General.ProcessText( ForumSmtpUserName.Text );
			PageContext.BoardSettings.SmtpUserPass = General.ProcessText( ForumSmtpUserPass.Text );
			PageContext.BoardSettings.ForumEmail = ForumEmailEdit.Text;
			PageContext.BoardSettings.EmailVerification = EmailVerification.Checked;
			PageContext.BoardSettings.ShowMoved = ShowMoved.Checked;
			PageContext.BoardSettings.BlankLinks = BlankLinks.Checked;
			PageContext.BoardSettings.ShowGroups = ShowGroupsX.Checked;
			PageContext.BoardSettings.AvatarWidth = Convert.ToInt32( AvatarWidth.Text );
			PageContext.BoardSettings.AvatarHeight = Convert.ToInt32( AvatarHeight.Text );
			PageContext.BoardSettings.AvatarUpload = AvatarUpload.Checked;
			PageContext.BoardSettings.AvatarRemote = AvatarRemote.Checked;
			PageContext.BoardSettings.AvatarSize = ( AvatarSize.Text.Trim().Length > 0 ) ? Convert.ToInt32( AvatarSize.Text ) : 0;
			PageContext.BoardSettings.AllowUserTheme = AllowUserThemeX.Checked;
			PageContext.BoardSettings.AllowUserLanguage = AllowUserLanguageX.Checked;
			PageContext.BoardSettings.UseFileTable = UseFileTableX.Checked;
			PageContext.BoardSettings.ShowRSSLink = ShowRSSLinkX.Checked;
			PageContext.BoardSettings.ShowForumJump = ShowForumJumpX.Checked;
			PageContext.BoardSettings.AllowPrivateMessages = AllowPrivateMessagesX.Checked;
			PageContext.BoardSettings.AllowEmailSending = AllowEmailSendingX.Checked;
			PageContext.BoardSettings.AllowSignatures = AllowSignaturesX.Checked;
			PageContext.BoardSettings.RemoveNestedQuotes = RemoveNestedQuotesX.Checked;
			PageContext.BoardSettings.MaxFileSize = ( MaxFileSize.Text.Trim().Length > 0 ) ? Convert.ToInt32( MaxFileSize.Text.Trim() ) : 0;
			PageContext.BoardSettings.SmiliesColumns = Convert.ToInt32( SmiliesColumns.Text.Trim() );
			PageContext.BoardSettings.SmiliesPerRow = Convert.ToInt32( SmiliesPerRow.Text.Trim() );
			PageContext.BoardSettings.LockPosts = LockPosts.Text.Trim() == string.Empty ? 0 : Convert.ToInt32( LockPosts.Text.Trim() );
			PageContext.BoardSettings.PostsPerPage = Convert.ToInt32( PostsPerPage.Text.Trim() );
			PageContext.BoardSettings.TopicsPerPage = Convert.ToInt32( TopicsPerPage.Text.Trim() );
			PageContext.BoardSettings.PostFloodDelay = Convert.ToInt32( PostFloodDelay.Text.Trim() );
			PageContext.BoardSettings.DateFormatFromLanguage = DateFormatFromLanguage.Checked;
			PageContext.BoardSettings.AcceptedHTML = AcceptedHTML.Text.Trim();
			PageContext.BoardSettings.DisableRegistrations = DisableRegistrations.Checked;
			PageContext.BoardSettings.CreateNntpUsers = CreateNntpUsers.Checked;
			PageContext.BoardSettings.ShowGroupsProfile = ShowGroupsProfile.Checked;
			PageContext.BoardSettings.PollVoteTiedToIP = PollVoteTiedToIPX.Checked;
			PageContext.BoardSettings.AllowPMEmailNotification = AllowPMNotifications.Checked;
			PageContext.BoardSettings.ShowPageGenerationTime = ShowPageGenerationTime.Checked;
			PageContext.BoardSettings.AdPost = AdPost.Text;
			PageContext.BoardSettings.ShowAdsToSignedInUsers = ShowAdsToSignedInUsers.Checked;
			PageContext.BoardSettings.DisplayPoints = DisplayPoints.Checked;
			PageContext.BoardSettings.ShowQuickAnswer = ShowQuickAnswerX.Checked;
			PageContext.BoardSettings.ShowDeletedMessages = ShowDeletedMessages.Checked;
			PageContext.BoardSettings.ShowDeletedMessagesToAll = ShowDeletedMessagesToAll.Checked;
			PageContext.BoardSettings.EditTimeOut = Convert.ToInt32( EditTimeOut.Text );
			PageContext.BoardSettings.ShowYAFVersion = ShowYAFVersion.Checked;
			PageContext.BoardSettings.DisableNoFollowLinksAfterDay = Convert.ToInt32( DisableNoFollowLinksAfterDay.Text );
			PageContext.BoardSettings.UseNoFollowLinks = UseNoFollowLinks.Checked;
			PageContext.BoardSettings.ShowRulesForRegistration = ShowRulesForRegistrationX.Checked;

			// Ederon : 7/1/2007 added
			PageContext.BoardSettings.ShowBrowsingUsers = ShowBrowsingUsers.Checked;
			PageContext.BoardSettings.ShowMedals = ShowMedals.Checked;
			PageContext.BoardSettings.DisplayJoinDate = DisplayJoinDate.Checked;
			PageContext.BoardSettings.AllowPostToBlog = AllowPostToBlog.Checked;

			// Mek : 8/18/2007 added
			PageContext.BoardSettings.AllowReportAbuse = AllowReportAbuse.Checked;
			PageContext.BoardSettings.AllowReportSpam = AllowReportSpam.Checked;

			// Ederon : 8/29/2007 added
			PageContext.BoardSettings.AllowEmailTopic = AllowEmailTopic.Checked;

			// Ederon : 7/14/2007 added
			PageContext.BoardSettings.UserBox = UserBox.Text;
			PageContext.BoardSettings.UserBoxAvatar = UserBoxAvatar.Text;
			PageContext.BoardSettings.UserBoxMedals = UserBoxMedals.Text;
			PageContext.BoardSettings.UserBoxGroups = UserBoxGroups.Text;
			PageContext.BoardSettings.UserBoxJoinDate = UserBoxJoinDate.Text;
			PageContext.BoardSettings.UserBoxLocation = UserBoxLocation.Text;
			PageContext.BoardSettings.UserBoxPoints = UserBoxPoints.Text;
			PageContext.BoardSettings.UserBoxPosts = UserBoxPosts.Text;
			PageContext.BoardSettings.UserBoxRank = UserBoxRank.Text;
			PageContext.BoardSettings.UserBoxRankImage = UserBoxRankImage.Text;

			// Ederon : 9/9/2007 added
			PageContext.BoardSettings.SmtpServerPort = General.ProcessText( ForumSmtpServerPort.Text );
			PageContext.BoardSettings.SmtpServerSsl = ForumSmtpServerSsl.Checked;

			// Ederon : 11/21/2007 added
			PageContext.BoardSettings.ProfileViewPermissions = ProfileViewPermissions.SelectedIndex;

			// Ederon : 12/9/2007 added
			PageContext.BoardSettings.RequireLogin = RequireLogin.Checked;
			PageContext.BoardSettings.MembersListViewPermissions = MembersListViewPermissions.SelectedIndex;
			PageContext.BoardSettings.ActiveUsersViewPermissions = ActiveUsersViewPermissions.SelectedIndex;

			// Ederon : 12/14/2007 added
			PageContext.BoardSettings.ShowForumStatistics = ShowForumStatistics.Checked;
			PageContext.BoardSettings.ShowActiveDiscussions = ShowActiveDiscussions.Checked;
			PageContext.BoardSettings.ActiveDiscussionsCount = Convert.ToInt32(ActiveDiscussionsCount.Text.Trim());
			PageContext.BoardSettings.SearchStringMinLength = Convert.ToInt32(SearchStringMinLength.Text.Trim());
			PageContext.BoardSettings.SearchStringPattern = SearchStringPattern.Text;
			PageContext.BoardSettings.SearchPermissions = SearchPermissions.SelectedIndex;

			// Ederon : 12/18/2007 added
			PageContext.BoardSettings.MaxPrivateMessagesPerUser = Convert.ToInt32(MaxPrivateMessagesPerUser.Text.Trim());
			PageContext.BoardSettings.PrivateMessageMaxRecipients = Convert.ToInt32(PrivateMessageMaxRecipients.Text.Trim());

			// Caching
			PageContext.BoardSettings.ForumStatisticsCacheTimeout = Convert.ToInt32(ForumStatisticsCacheTimeout.Text.Trim());
			PageContext.BoardSettings.ActiveDiscussionsCacheTimeout = Convert.ToInt32(ActiveDiscussionsCacheTimeout.Text.Trim());
			PageContext.BoardSettings.BoardModeratorsCacheTimeout = Convert.ToInt32(BoardModeratorsCacheTimeout.Text.Trim());
			PageContext.BoardSettings.BoardCategoriesCacheTimeout = Convert.ToInt32(BoardCategoriesCacheTimeout.Text.Trim());
			PageContext.BoardSettings.ReplaceRulesCacheTimeout = Convert.ToInt32(ReplaceRulesCacheTimeout.Text.Trim());

			// CAPTCHA stuff
			PageContext.BoardSettings.CaptchaSize = Convert.ToInt32( CaptchaSize.Text );
			PageContext.BoardSettings.EnableCaptchaForPost = EnableCaptchaForPost.Checked;
			PageContext.BoardSettings.EnableCaptchaForRegister = EnableCaptchaForRegister.Checked;
			PageContext.BoardSettings.EnableCaptchaForGuests = EnableCaptchaForGuests.Checked;

			// Search Settings
			PageContext.BoardSettings.ReturnSearchMax = Convert.ToInt32( ReturnSearchMax.Text.Trim() );
			PageContext.BoardSettings.UseFullTextSearch = UseFullTextSearch.Checked;

			// save the settings to the database
			PageContext.BoardSettings.SaveRegistry();

			// reload all settings from the DB
			PageContext.BoardSettings = null;

			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_admin );
		}

		protected void ForumStatisticsCacheReset_Click(object sender, System.EventArgs e)
		{
			RemoveCacheKey(Constants.Cache.BoardStats);
		}

		protected void ActiveDiscussionsCacheReset_Click(object sender, System.EventArgs e)
		{
			RemoveCacheKey(Constants.Cache.ActiveDiscussions);
			RemoveCacheKey(Constants.Cache.ForumActiveDiscussions);
		}

		protected void BoardModeratorsCacheReset_Click(object sender, System.EventArgs e)
		{
			RemoveCacheKey(Constants.Cache.ForumModerators);
		}

		protected void BoardCategoriesCacheReset_Click(object sender, System.EventArgs e)
		{
			RemoveCacheKey(Constants.Cache.ForumCategory);
		}

		protected void ReplaceRulesCacheReset_Click(object sender, System.EventArgs e)
		{
			YAF.Classes.UI.ReplaceRulesCreator.ClearCache();
			CheckCache();
		}

		protected void ResetCacheAll_Click(object sender, System.EventArgs e)
		{
			// clear all cache keys
			YafCache.Current.Clear();

			CheckCache();
		}

		private void RemoveCacheKey(string key)
		{
			YafCache.Current.Remove(YafCache.GetBoardCacheKey(key));
			CheckCache();
		}

		private bool CheckCacheKey(string key)
		{
			return YafCache.Current[YafCache.GetBoardCacheKey(key)] != null;
		}

		private void CheckCache()
		{
			ForumStatisticsCacheReset.Enabled = CheckCacheKey(Constants.Cache.BoardStats);
			ActiveDiscussionsCacheReset.Enabled = CheckCacheKey(Constants.Cache.ActiveDiscussions) || CheckCacheKey(Constants.Cache.ForumActiveDiscussions);
			BoardModeratorsCacheReset.Enabled = CheckCacheKey(Constants.Cache.ForumModerators);
			BoardCategoriesCacheReset.Enabled = CheckCacheKey(Constants.Cache.ForumCategory);
			ResetCacheAll.Enabled = YafCache.Current.Count > 0;
		}
	}
}
