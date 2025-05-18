/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Types.InputModels;

/// <summary>
/// Class HostSettings Input Model.
/// </summary>
public class HostSettingsInputModel
{
    /// <summary>
    /// Gets or sets a value indicating whether [log banned ip].
    /// </summary>
    /// <value><c>true</c> if [log banned ip]; otherwise, <c>false</c>.</value>
    public bool LogBannedIP { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow pm email notification].
    /// </summary>
    /// <value><c>true</c> if [allow pm email notification]; otherwise, <c>false</c>.</value>
    public bool AllowPMEmailNotification { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [enable ip information service].
    /// </summary>
    /// <value><c>true</c> if [enable ip information service]; otherwise, <c>false</c>.</value>
    public bool EnableIPInfoService { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show yaf version].
    /// </summary>
    /// <value><c>true</c> if [show yaf version]; otherwise, <c>false</c>.</value>
    public bool ShowYAFVersion { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [hide copyright].
    /// </summary>
    /// <value><c>true</c> if [hide copyright]; otherwise, <c>false</c>.</value>
    public bool HideCopyright { get; set; }

    /// <summary>
    /// Gets or sets the SQL version.
    /// </summary>
    /// <value>The SQL version.</value>
    public string SQLVersion { get; set; }

    /// <summary>
    /// Gets or sets the ip locator URL path.
    /// </summary>
    /// <value>The ip locator URL path.</value>
    public string IPLocatorUrlPath { get; set; }

    /// <summary>
    /// Gets or sets the ip information page URL.
    /// </summary>
    /// <value>The ip information page URL.</value>
    public string IPInfoPageURL { get; set; }

    /// <summary>
    /// Gets or sets the abuse ip database API key.
    /// </summary>
    /// <value>The abuse ip database API key.</value>
    public string AbuseIpDbApiKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [log user suspended unsuspended].
    /// </summary>
    /// <value><c>true</c> if [log user suspended unsuspended]; otherwise, <c>false</c>.</value>
    public bool LogUserSuspendedUnsuspended { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [log user deleted].
    /// </summary>
    /// <value><c>true</c> if [log user deleted]; otherwise, <c>false</c>.</value>
    public bool LogUserDeleted { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [log view state error].
    /// </summary>
    /// <value><c>true</c> if [log view state error]; otherwise, <c>false</c>.</value>
    public bool LogViewStateError { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [log information].
    /// </summary>
    /// <value><c>true</c> if [log information]; otherwise, <c>false</c>.</value>
    public bool LogInformation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [log warning].
    /// </summary>
    /// <value><c>true</c> if [log warning]; otherwise, <c>false</c>.</value>
    public bool LogWarning { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [log error].
    /// </summary>
    /// <value><c>true</c> if [log error]; otherwise, <c>false</c>.</value>
    public bool LogError { get; set; }

    /// <summary>
    /// Gets or sets the minimum length of the required password.
    /// </summary>
    /// <value>The minimum length of the required password.</value>
    public int MinRequiredPasswordLength { get; set; }

    /// <summary>
    /// Gets or sets the allowed number of urls.
    /// </summary>
    /// <value>The allowed number of urls.</value>
    public int AllowedNumberOfUrls { get; set; }

    /// <summary>
    /// Gets or sets the album images size maximum.
    /// </summary>
    /// <value>The album images size maximum.</value>
    public string AlbumImagesSizeMax { get; set; }

    /// <summary>
    /// Gets or sets the albums per page.
    /// </summary>
    /// <value>The albums per page.</value>
    public int AlbumsPerPage { get; set; }

    /// <summary>
    /// Gets or sets the album images per page.
    /// </summary>
    /// <value>The album images per page.</value>
    public int AlbumImagesPerPage { get; set; }

    /// <summary>
    /// Gets or sets the allowed poll choice number.
    /// </summary>
    /// <value>The allowed poll choice number.</value>
    public int AllowedPollChoiceNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [use file table].
    /// </summary>
    /// <value><c>true</c> if [use file table]; otherwise, <c>false</c>.</value>
    public bool UseFileTable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [enable wysiwyg editor].
    /// </summary>
    /// <value><c>true</c> if [enable wysiwyg editor]; otherwise, <c>false</c>.</value>
    public bool EnableWysiwygEditor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show cookie consent].
    /// </summary>
    /// <value><c>true</c> if [show cookie consent]; otherwise, <c>false</c>.</value>
    public bool ShowCookieConsent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [abandon sessions for dont track].
    /// </summary>
    /// <value><c>true</c> if [abandon sessions for dont track]; otherwise, <c>false</c>.</value>
    public bool AbandonSessionsForDontTrack { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [password require non letter or digit].
    /// </summary>
    /// <value><c>true</c> if [password require non letter or digit]; otherwise, <c>false</c>.</value>
    public bool PasswordRequireNonLetterOrDigit { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [password require digit].
    /// </summary>
    /// <value><c>true</c> if [password require digit]; otherwise, <c>false</c>.</value>
    public bool PasswordRequireDigit { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [password require lowercase].
    /// </summary>
    /// <value><c>true</c> if [password require lowercase]; otherwise, <c>false</c>.</value>
    public bool PasswordRequireLowercase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [password require uppercase].
    /// </summary>
    /// <value><c>true</c> if [password require uppercase]; otherwise, <c>false</c>.</value>
    public bool PasswordRequireUppercase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [ban bot ip on detection].
    /// </summary>
    /// <value><c>true</c> if [ban bot ip on detection]; otherwise, <c>false</c>.</value>
    public bool BanBotIpOnDetection { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [disable registrations].
    /// </summary>
    /// <value><c>true</c> if [disable registrations]; otherwise, <c>false</c>.</value>
    public bool DisableRegistrations { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [require login].
    /// </summary>
    /// <value><c>true</c> if [require login]; otherwise, <c>false</c>.</value>
    public bool RequireLogin { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show connect message in topic].
    /// </summary>
    /// <value><c>true</c> if [show connect message in topic]; otherwise, <c>false</c>.</value>
    public bool ShowConnectMessageInTopic { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show rules for registration].
    /// </summary>
    /// <value><c>true</c> if [show rules for registration]; otherwise, <c>false</c>.</value>
    public bool ShowRulesForRegistration { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [enable album].
    /// </summary>
    /// <value><c>true</c> if [enable album]; otherwise, <c>false</c>.</value>
    public bool EnableAlbum { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [enable image attachment resize].
    /// </summary>
    /// <value><c>true</c> if [enable image attachment resize]; otherwise, <c>false</c>.</value>
    public bool EnableImageAttachmentResize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [use read tracking by database].
    /// </summary>
    /// <value><c>true</c> if [use read tracking by database]; otherwise, <c>false</c>.</value>
    public bool UseReadTrackingByDatabase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show relative time].
    /// </summary>
    /// <value><c>true</c> if [show relative time]; otherwise, <c>false</c>.</value>
    public bool ShowRelativeTime { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [use farsi calender].
    /// </summary>
    /// <value><c>true</c> if [use farsi calender]; otherwise, <c>false</c>.</value>
    public bool UseFarsiCalender { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow user hide himself].
    /// </summary>
    /// <value><c>true</c> if [allow user hide himself]; otherwise, <c>false</c>.</value>
    public bool AllowUserHideHimself { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show user online status].
    /// </summary>
    /// <value><c>true</c> if [show user online status]; otherwise, <c>false</c>.</value>
    public bool ShowUserOnlineStatus { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [enable display name].
    /// </summary>
    /// <value><c>true</c> if [enable display name]; otherwise, <c>false</c>.</value>
    public bool EnableDisplayName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow display name modification].
    /// </summary>
    /// <value><c>true</c> if [allow display name modification]; otherwise, <c>false</c>.</value>
    public bool AllowDisplayNameModification { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [use styled nicks].
    /// </summary>
    /// <value><c>true</c> if [use styled nicks]; otherwise, <c>false</c>.</value>
    public bool UseStyledNicks { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [use styled topic titles].
    /// </summary>
    /// <value><c>true</c> if [use styled topic titles]; otherwise, <c>false</c>.</value>
    public bool UseStyledTopicTitles { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show thanks date].
    /// </summary>
    /// <value><c>true</c> if [show thanks date]; otherwise, <c>false</c>.</value>
    public bool ShowThanksDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [enable buddy list].
    /// </summary>
    /// <value><c>true</c> if [enable buddy list]; otherwise, <c>false</c>.</value>
    public bool EnableBuddyList { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [remove nested quotes].
    /// </summary>
    /// <value><c>true</c> if [remove nested quotes]; otherwise, <c>false</c>.</value>
    public bool RemoveNestedQuotes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show quick answer].
    /// </summary>
    /// <value><c>true</c> if [show quick answer]; otherwise, <c>false</c>.</value>
    public bool ShowQuickAnswer { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow email topic].
    /// </summary>
    /// <value><c>true</c> if [allow email topic]; otherwise, <c>false</c>.</value>
    public bool AllowEmailTopic { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [disable no follow links after day].
    /// </summary>
    /// <value><c>true</c> if [disable no follow links after day]; otherwise, <c>false</c>.</value>
    public bool DisableNoFollowLinksAfterDay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [lock posts].
    /// </summary>
    /// <value><c>true</c> if [lock posts]; otherwise, <c>false</c>.</value>
    public bool LockPosts { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show share topic to].
    /// </summary>
    /// <value><c>true</c> if [show share topic to]; otherwise, <c>false</c>.</value>
    public bool ShowShareTopicTo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [enable user information hover cards].
    /// </summary>
    /// <value><c>true</c> if [enable user information hover cards]; otherwise, <c>false</c>.</value>
    public bool EnableUserInfoHoverCards { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow poll changes after first vote].
    /// </summary>
    /// <value><c>true</c> if [allow poll changes after first vote]; otherwise, <c>false</c>.</value>
    public bool AllowPollChangesAfterFirstVote { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow multiple choices].
    /// </summary>
    /// <value><c>true</c> if [allow multiple choices]; otherwise, <c>false</c>.</value>
    public bool AllowMultipleChoices { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow users hide poll results].
    /// </summary>
    /// <value><c>true</c> if [allow users hide poll results]; otherwise, <c>false</c>.</value>
    public bool AllowUsersHidePollResults { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow users imaged poll].
    /// </summary>
    /// <value><c>true</c> if [allow users imaged poll]; otherwise, <c>false</c>.</value>
    public bool AllowUsersImagedPoll { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow private messages].
    /// </summary>
    /// <value><c>true</c> if [allow private messages]; otherwise, <c>false</c>.</value>
    public bool AllowPrivateMessages { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show atom link].
    /// </summary>
    /// <value><c>true</c> if [show atom link]; otherwise, <c>false</c>.</value>
    public bool ShowAtomLink { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [enable user reputation].
    /// </summary>
    /// <value><c>true</c> if [enable user reputation]; otherwise, <c>false</c>.</value>
    public bool EnableUserReputation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [reputation allow negative].
    /// </summary>
    /// <value><c>true</c> if [reputation allow negative]; otherwise, <c>false</c>.</value>
    public bool ReputationAllowNegative { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show guests in detailed active list].
    /// </summary>
    /// <value><c>true</c> if [show guests in detailed active list]; otherwise, <c>false</c>.</value>
    public bool ShowGuestsInDetailedActiveList { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show crawlers in active list].
    /// </summary>
    /// <value><c>true</c> if [show crawlers in active list]; otherwise, <c>false</c>.</value>
    public bool ShowCrawlersInActiveList { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show deleted messages].
    /// </summary>
    /// <value><c>true</c> if [show deleted messages]; otherwise, <c>false</c>.</value>
    public bool ShowDeletedMessages { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show deleted messages to all].
    /// </summary>
    /// <value><c>true</c> if [show deleted messages to all]; otherwise, <c>false</c>.</value>
    public bool ShowDeletedMessagesToAll { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [blank links].
    /// </summary>
    /// <value><c>true</c> if [blank links]; otherwise, <c>false</c>.</value>
    public bool BlankLinks { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [use no follow links].
    /// </summary>
    /// <value><c>true</c> if [use no follow links]; otherwise, <c>false</c>.</value>
    public bool UseNoFollowLinks { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [no count forums in active discussions].
    /// </summary>
    /// <value><c>true</c> if [no count forums in active discussions]; otherwise, <c>false</c>.</value>
    public bool NoCountForumsInActiveDiscussions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show active discussions].
    /// </summary>
    /// <value><c>true</c> if [show active discussions]; otherwise, <c>false</c>.</value>
    public bool ShowActiveDiscussions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show recent users].
    /// </summary>
    /// <value><c>true</c> if [show recent users]; otherwise, <c>false</c>.</value>
    public bool ShowRecentUsers { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show forum statistics].
    /// </summary>
    /// <value><c>true</c> if [show forum statistics]; otherwise, <c>false</c>.</value>
    public bool ShowForumStatistics { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show page generation time].
    /// </summary>
    /// <value><c>true</c> if [show page generation time]; otherwise, <c>false</c>.</value>
    public bool ShowPageGenerationTime { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show groups profile].
    /// </summary>
    /// <value><c>true</c> if [show groups profile]; otherwise, <c>false</c>.</value>
    public bool ShowGroupsProfile { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show browsing users].
    /// </summary>
    /// <value><c>true</c> if [show browsing users]; otherwise, <c>false</c>.</value>
    public bool ShowBrowsingUsers { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show similar topics].
    /// </summary>
    /// <value><c>true</c> if [show similar topics]; otherwise, <c>false</c>.</value>
    public bool ShowSimilarTopics { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show forum jump].
    /// </summary>
    /// <value><c>true</c> if [show forum jump]; otherwise, <c>false</c>.</value>
    public bool ShowForumJump { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show medals].
    /// </summary>
    /// <value><c>true</c> if [show medals]; otherwise, <c>false</c>.</value>
    public bool ShowMedals { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show moved].
    /// </summary>
    /// <value><c>true</c> if [show moved]; otherwise, <c>false</c>.</value>
    public bool ShowMoved { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show help to].
    /// </summary>
    /// <value><c>true</c> if [show help to]; otherwise, <c>false</c>.</value>
    public bool ShowHelpTo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show team to].
    /// </summary>
    /// <value><c>true</c> if [show team to]; otherwise, <c>false</c>.</value>
    public bool ShowTeamTo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show moderator list].
    /// </summary>
    /// <value><c>true</c> if [show moderator list]; otherwise, <c>false</c>.</value>
    public bool ShowModeratorList { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [scroll to post].
    /// </summary>
    /// <value><c>true</c> if [scroll to post]; otherwise, <c>false</c>.</value>
    public bool ScrollToPost { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show scroll back to top button].
    /// </summary>
    /// <value><c>true</c> if [show scroll back to top button]; otherwise, <c>false</c>.</value>
    public bool ShowScrollBackToTopButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [two column board layout].
    /// </summary>
    /// <value><c>true</c> if [two column board layout]; otherwise, <c>false</c>.</value>
    public bool TwoColumnBoardLayout { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [use custom context menu].
    /// </summary>
    /// <value><c>true</c> if [use custom context menu]; otherwise, <c>false</c>.</value>
    public bool UseCustomContextMenu { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show ads to signed in users].
    /// </summary>
    /// <value><c>true</c> if [show ads to signed in users]; otherwise, <c>false</c>.</value>
    public bool ShowAdsToSignedInUsers { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow user theme].
    /// </summary>
    /// <value><c>true</c> if [allow user theme]; otherwise, <c>false</c>.</value>
    public bool AllowUserTheme { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow user language].
    /// </summary>
    /// <value><c>true</c> if [allow user language]; otherwise, <c>false</c>.</value>
    public bool AllowUserLanguage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow signatures].
    /// </summary>
    /// <value><c>true</c> if [allow signatures]; otherwise, <c>false</c>.</value>
    public bool AllowSignatures { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow email sending].
    /// </summary>
    /// <value><c>true</c> if [allow email sending]; otherwise, <c>false</c>.</value>
    public bool AllowEmailSending { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow email change].
    /// </summary>
    /// <value><c>true</c> if [allow email change]; otherwise, <c>false</c>.</value>
    public bool AllowEmailChange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow moderators view i ps].
    /// </summary>
    /// <value><c>true</c> if [allow moderators view i ps]; otherwise, <c>false</c>.</value>
    public bool AllowModeratorsViewIPs { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [allow forums with same name].
    /// </summary>
    /// <value><c>true</c> if [allow forums with same name]; otherwise, <c>false</c>.</value>
    public bool AllowForumsWithSameName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [avatar gallery].
    /// </summary>
    /// <value><c>true</c> if [avatar gallery]; otherwise, <c>false</c>.</value>
    public bool AvatarGallery { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [avatar upload].
    /// </summary>
    /// <value><c>true</c> if [avatar upload]; otherwise, <c>false</c>.</value>
    public bool AvatarUpload { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show quick search].
    /// </summary>
    /// <value><c>true</c> if [show quick search]; otherwise, <c>false</c>.</value>
    public bool ShowQuickSearch { get; set; }

    /// <summary>
    /// Gets or sets the name of the application os.
    /// </summary>
    /// <value>The name of the application os.</value>
    public string AppOsName { get; set; }

    /// <summary>
    /// Gets or sets the application runtime.
    /// </summary>
    /// <value>The application runtime.</value>
    public string AppRuntime { get; set; }

    /// <summary>
    /// Gets or sets the application cores.
    /// </summary>
    /// <value>The application cores.</value>
    public string AppCores { get; set; }

    /// <summary>
    /// Gets or sets the application memory.
    /// </summary>
    /// <value>The application memory.</value>
    public string AppMemory { get; set; }

    /// <summary>
    /// Gets or sets the server time correction.
    /// </summary>
    /// <value>The server time correction.</value>
    public int ServerTimeCorrection { get; set; }

    /// <summary>
    /// Gets or sets the edit time out.
    /// </summary>
    /// <value>The edit time out.</value>
    public string EditTimeOut { get; set; }

    /// <summary>
    /// Gets or sets the display length of the name minimum.
    /// </summary>
    /// <value>The display length of the name minimum.</value>
    public int DisplayNameMinLength { get; set; }

    /// <summary>
    /// Gets or sets the maximum length of the user name.
    /// </summary>
    /// <value>The maximum length of the user name.</value>
    public int UserNameMaxLength { get; set; }

    /// <summary>
    /// Gets or sets the maximum report post chars.
    /// </summary>
    /// <value>The maximum report post chars.</value>
    public int MaxReportPostChars { get; set; }

    /// <summary>
    /// Gets or sets the maximum size of the post.
    /// </summary>
    /// <value>The maximum size of the post.</value>
    public int MaxPostSize { get; set; }

    /// <summary>
    /// Gets or sets the post flood delay.
    /// </summary>
    /// <value>The post flood delay.</value>
    public int PostFloodDelay { get; set; }

    /// <summary>
    /// Gets or sets the message history days to log.
    /// </summary>
    /// <value>The message history days to log.</value>
    public int MessageHistoryDaysToLog { get; set; }

    /// <summary>
    /// Gets or sets the event log maximum days.
    /// </summary>
    /// <value>The event log maximum days.</value>
    public int EventLogMaxDays { get; set; }

    /// <summary>
    /// Gets or sets the event log maximum messages.
    /// </summary>
    /// <value>The event log maximum messages.</value>
    public int EventLogMaxMessages { get; set; }

    /// <summary>
    /// Gets or sets the minimum length of the search string.
    /// </summary>
    /// <value>The minimum length of the search string.</value>
    public int SearchStringMinLength { get; set; }

    /// <summary>
    /// Gets or sets the return search maximum.
    /// </summary>
    /// <value>The return search maximum.</value>
    public int ReturnSearchMax { get; set; }

    /// <summary>
    /// Gets or sets the forum statistics cache timeout.
    /// </summary>
    /// <value>The forum statistics cache timeout.</value>
    public int ForumStatisticsCacheTimeout { get; set; }

    /// <summary>
    /// Gets or sets the board user stats cache timeout.
    /// </summary>
    /// <value>The board user stats cache timeout.</value>
    public int BoardUserStatsCacheTimeout { get; set; }

    /// <summary>
    /// Gets or sets the active discussions cache timeout.
    /// </summary>
    /// <value>The active discussions cache timeout.</value>
    public int ActiveDiscussionsCacheTimeout { get; set; }

    /// <summary>
    /// Gets or sets the board moderators cache timeout.
    /// </summary>
    /// <value>The board moderators cache timeout.</value>
    public int BoardModeratorsCacheTimeout { get; set; }

    /// <summary>
    /// Gets or sets the online status cache timeout.
    /// </summary>
    /// <value>The online status cache timeout.</value>
    public int OnlineStatusCacheTimeout { get; set; }

    /// <summary>
    /// Gets or sets the active user lazy data cache timeout.
    /// </summary>
    /// <value>The active user lazy data cache timeout.</value>
    public int ActiveUserLazyDataCacheTimeout { get; set; }

    /// <summary>
    /// Gets or sets the search permissions.
    /// </summary>
    /// <value>The search permissions.</value>
    public int SearchPermissions { get; set; }

    /// <summary>
    /// Gets or sets the type of the spam service.
    /// </summary>
    /// <value>The type of the spam service.</value>
    public string SpamServiceType { get; set; }

    /// <summary>
    /// Gets or sets the spam message handling.
    /// </summary>
    /// <value>The spam message handling.</value>
    public string SpamMessageHandling { get; set; }

    /// <summary>
    /// Gets or sets the type of the bot spam service.
    /// </summary>
    /// <value>The type of the bot spam service.</value>
    public string BotSpamServiceType { get; set; }

    /// <summary>
    /// Gets or sets the ignore spam word check post count.
    /// </summary>
    /// <value>The ignore spam word check post count.</value>
    public int IgnoreSpamWordCheckPostCount { get; set; }

    /// <summary>
    /// Gets or sets the bot scout API key.
    /// </summary>
    /// <value>The bot scout API key.</value>
    public string BotScoutApiKey { get; set; }

    /// <summary>
    /// Gets or sets the stop forum spam API key.
    /// </summary>
    /// <value>The stop forum spam API key.</value>
    public string StopForumSpamApiKey { get; set; }

    /// <summary>
    /// Gets or sets the bot handling on register.
    /// </summary>
    /// <value>The bot handling on register.</value>
    public string BotHandlingOnRegister { get; set; }

    /// <summary>
    /// Gets or sets the send welcome notification after register.
    /// </summary>
    /// <value>The send welcome notification after register.</value>
    public string SendWelcomeNotificationAfterRegister { get; set; }

    /// <summary>
    /// Gets or sets the allowed file extensions.
    /// </summary>
    /// <value>The allowed file extensions.</value>
    public string AllowedFileExtensions { get; set; }

    /// <summary>
    /// Gets or sets the maximum size of the file.
    /// </summary>
    /// <value>The maximum size of the file.</value>
    public string MaxFileSize { get; set; }

    /// <summary>
    /// Gets or sets the picture attachment display treshold.
    /// </summary>
    /// <value>The picture attachment display treshold.</value>
    public int PictureAttachmentDisplayTreshold { get; set; }

    /// <summary>
    /// Gets or sets the maximum width of the image thumbnail.
    /// </summary>
    /// <value>The maximum width of the image thumbnail.</value>
    public int ImageThumbnailMaxWidth { get; set; }

    /// <summary>
    /// Gets or sets the maximum height of the image thumbnail.
    /// </summary>
    /// <value>The maximum height of the image thumbnail.</value>
    public int ImageThumbnailMaxHeight { get; set; }

    /// <summary>
    /// Gets or sets the width of the image attachment resize.
    /// </summary>
    /// <value>The width of the image attachment resize.</value>
    public int ImageAttachmentResizeWidth { get; set; }

    /// <summary>
    /// Gets or sets the height of the image attachment resize.
    /// </summary>
    /// <value>The height of the image attachment resize.</value>
    public int ImageAttachmentResizeHeight { get; set; }

    /// <summary>
    /// Gets or sets the hover card open delay.
    /// </summary>
    /// <value>The hover card open delay.</value>
    public int HoverCardOpenDelay { get; set; }

    /// <summary>
    /// Gets or sets the size of the poll image maximum file.
    /// </summary>
    /// <value>The size of the poll image maximum file.</value>
    public int PollImageMaxFileSize { get; set; }

    /// <summary>
    /// Gets or sets the popular topic views.
    /// </summary>
    /// <value>The popular topic views.</value>
    public int PopularTopicViews { get; set; }

    /// <summary>
    /// Gets or sets the popular topic replys.
    /// </summary>
    /// <value>The popular topic replys.</value>
    public int PopularTopicReplys { get; set; }

    /// <summary>
    /// Gets or sets the popular topic days.
    /// </summary>
    /// <value>The popular topic days.</value>
    public int PopularTopicDays { get; set; }

    /// <summary>
    /// Gets or sets the topics feed items count.
    /// </summary>
    /// <value>The topics feed items count.</value>
    public int TopicsFeedItemsCount { get; set; }

    /// <summary>
    /// Gets or sets the posts feed access.
    /// </summary>
    /// <value>The posts feed access.</value>
    public int PostsFeedAccess { get; set; }

    /// <summary>
    /// Gets or sets the post latest feed access.
    /// </summary>
    /// <value>The post latest feed access.</value>
    public int PostLatestFeedAccess { get; set; }

    /// <summary>
    /// Gets or sets the topics feed access.
    /// </summary>
    /// <value>The topics feed access.</value>
    public int TopicsFeedAccess { get; set; }

    /// <summary>
    /// Gets or sets the reputation maximum negative.
    /// </summary>
    /// <value>The reputation maximum negative.</value>
    public int ReputationMaxNegative { get; set; }

    /// <summary>
    /// Gets or sets the reputation maximum positive.
    /// </summary>
    /// <value>The reputation maximum positive.</value>
    public int ReputationMaxPositive { get; set; }

    /// <summary>
    /// Gets or sets the reputation minimum up voting.
    /// </summary>
    /// <value>The reputation minimum up voting.</value>
    public int ReputationMinUpVoting { get; set; }

    /// <summary>
    /// Gets or sets the reputation minimum down voting.
    /// </summary>
    /// <value>The reputation minimum down voting.</value>
    public int ReputationMinDownVoting { get; set; }

    /// <summary>
    /// Gets or sets the duration of the message notifcation.
    /// </summary>
    /// <value>The duration of the message notifcation.</value>
    public int MessageNotifcationDuration { get; set; }

    /// <summary>
    /// Gets or sets the active list time.
    /// </summary>
    /// <value>The active list time.</value>
    public int ActiveListTime { get; set; }

    /// <summary>
    /// Gets or sets the active discussions count.
    /// </summary>
    /// <value>The active discussions count.</value>
    public int ActiveDiscussionsCount { get; set; }

    /// <summary>
    /// Gets or sets the posts per page.
    /// </summary>
    /// <value>The posts per page.</value>
    public int PostsPerPage { get; set; }

    /// <summary>
    /// Gets or sets the sub forums in forum list.
    /// </summary>
    /// <value>The sub forums in forum list.</value>
    public int SubForumsInForumList { get; set; }

    /// <summary>
    /// Gets or sets the title template.
    /// </summary>
    /// <value>The title template.</value>
    public string TitleTemplate { get; set; }

    /// <summary>
    /// Gets or sets the paging title template.
    /// </summary>
    /// <value>The paging title template.</value>
    public string PagingTitleTemplate { get; set; }

    /// <summary>
    /// Gets or sets the ad post.
    /// </summary>
    /// <value>The ad post.</value>
    public string AdPost { get; set; }

    /// <summary>
    /// Gets or sets the name of the allow create topics same.
    /// </summary>
    /// <value>The name of the allow create topics same.</value>
    public int AllowCreateTopicsSameName { get; set; }

    /// <summary>
    /// Gets or sets the report post permissions.
    /// </summary>
    /// <value>The report post permissions.</value>
    public int ReportPostPermissions { get; set; }

    /// <summary>
    /// Gets or sets the active users view permissions.
    /// </summary>
    /// <value>The active users view permissions.</value>
    public int ActiveUsersViewPermissions { get; set; }

    /// <summary>
    /// Gets or sets the profile view permissions.
    /// </summary>
    /// <value>The profile view permissions.</value>
    public int ProfileViewPermissions { get; set; }

    /// <summary>
    /// Gets or sets the members ListView permissions.
    /// </summary>
    /// <value>The members ListView permissions.</value>
    public int MembersListViewPermissions { get; set; }

    /// <summary>
    /// Gets or sets the size of the avatar.
    /// </summary>
    /// <value>The size of the avatar.</value>
    public string AvatarSize { get; set; }

    /// <summary>
    /// Gets or sets the width of the avatar.
    /// </summary>
    /// <value>The width of the avatar.</value>
    public int AvatarWidth { get; set; }

    /// <summary>
    /// Gets or sets the height of the avatar.
    /// </summary>
    /// <value>The height of the avatar.</value>
    public int AvatarHeight { get; set; }

    /// <summary>
    /// Gets or sets the editor enter mode.
    /// </summary>
    /// <value>The editor enter mode.</value>
    public int EditorEnterMode { get; set; }

    /// <summary>
    /// Gets or sets the last tab identifier.
    /// </summary>
    /// <value>The last tab identifier.</value>
    public string LastTabId { get; set; } = "Setup";
}