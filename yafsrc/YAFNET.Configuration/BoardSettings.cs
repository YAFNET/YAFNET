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

namespace YAF.Configuration;

/// <summary>
/// The YAF board settings.
/// </summary>
public class BoardSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoardSettings"/> class.
    /// </summary>
    public BoardSettings()
    {
        this.BoardId = 0;
        this.Name = string.Empty;
        this.Description = string.Empty;
        this.Registry = [];
        this.RegistryBoard = [];

        // set the board dictionary as the override...
        this.Registry.OverrideDictionary = this.RegistryBoard;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:YAF.Configuration.BoardSettings" /> class.
    /// </summary>
    /// <param name="boardId">The board identifier.</param>
    /// <param name="boardName">Name of the board.</param>
    /// <param name="boardDescription">The description of the board.</param>
    /// <param name="registry">The registry.</param>
    /// <param name="registryBoard">The registry board.</param>
    public BoardSettings(
        int boardId,
        string boardName,
        string boardDescription,
        RegistryDictionaryOverride registry,
        RegistryDictionary registryBoard)
    {
        this.BoardId = boardId;
        this.Name = boardName;
        this.Description = boardDescription;
        this.Registry = registry;
        this.RegistryBoard = registryBoard;

        // set the board dictionary as the override...
        this.Registry.OverrideDictionary = this.RegistryBoard;
    }

    /// <summary>
    /// Gets or sets the denied user registrations count.
    /// </summary>
    public int DeniedRegistrations
    {
        get => this.RegistryBoard.GetValue("DeniedRegistrations", 0);

        set => this.RegistryBoard.SetValue("DeniedRegistrations", value);
    }

    /// <summary>
    /// Gets or sets the banned users count.
    /// </summary>
    public int BannedUsers
    {
        get => this.RegistryBoard.GetValue("BannedUsers", 0);

        set => this.RegistryBoard.SetValue("BannedUsers", value);
    }

    /// <summary>
    /// Gets or sets the reported spammers count.
    /// </summary>
    public int ReportedSpammers
    {
        get => this.RegistryBoard.GetValue("ReportedSpammers", 0);

        set => this.RegistryBoard.SetValue("ReportedSpammers", value);
    }

    /// <summary>
    /// Gets or sets the max users.
    /// </summary>
    public int MaxUsers
    {
        get => this.RegistryBoard.GetValue("MaxUsers", 0);

        set => this.RegistryBoard.SetValue("MaxUsers", value);
    }

    /// <summary>
    /// Gets or sets the max users when.
    /// </summary>
    public DateTime MaxUsersWhen
    {
        get => this.RegistryBoard.GetValue("MaxUsersWhen", DateTime.MinValue);

        set => this.RegistryBoard.SetValue("MaxUsersWhen", value);
    }

    /// <summary>
    /// The application id.
    /// </summary>
    public Guid ApplicationId => new(this.Registry.GetValue("ApplicationId", string.Empty));

    /// <summary>
    /// Gets or sets the min required password length.
    /// </summary>
    public int MinRequiredPasswordLength
    {
        get => this.RegistryBoard.GetValue("MinRequiredPasswordLength", 6);

        set => this.RegistryBoard.SetValue("MinRequiredPasswordLength", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the password requires a non-letter or digit character.
    /// </summary>
    public bool PasswordRequireNonLetterOrDigit
    {
        get => this.RegistryBoard.GetValue("PasswordRequireNonLetterOrDigit", true);

        set => this.RegistryBoard.SetValue("PasswordRequireNonLetterOrDigit", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the password requires a numeric digit ('0' - '9').
    /// </summary>
    public bool PasswordRequireDigit
    {
        get => this.RegistryBoard.GetValue("PasswordRequireDigit", true);

        set => this.RegistryBoard.SetValue("PasswordRequireDigit", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the password requires a lower case letter ('a' - 'z').
    /// </summary>
    public bool PasswordRequireLowercase
    {
        get => this.RegistryBoard.GetValue("PasswordRequireLowercase", true);

        set => this.RegistryBoard.SetValue("PasswordRequireLowercase", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the password requires an upper case letter ('A' - 'Z').
    /// </summary>
    public bool PasswordRequireUppercase
    {
        get => this.RegistryBoard.GetValue("PasswordRequireUppercase", true);

        set => this.RegistryBoard.SetValue("PasswordRequireUppercase", value);
    }

    /// <summary>
    /// Gets or sets the board name.
    /// </summary>
    /// <value>The board name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the board description.
    /// </summary>
    /// <value>The board description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the title template.
    /// </summary>
    /// <value>
    /// The title template.
    /// </value>
    public string TitleTemplate
    {
        get => this.RegistryBoard.GetValue("TitleTemplate", "{title} &#183; {boardName}");

        set => this.RegistryBoard.SetValue("TitleTemplate", value);
    }

    /// <summary>
    /// Gets or sets the paging title template.
    /// </summary>
    /// <value>
    /// The paging title template.
    /// </value>
    public string PagingTitleTemplate
    {
        get => this.RegistryBoard.GetValue("PagingTitleTemplate", " &#183; {paging}");

        set => this.RegistryBoard.SetValue("PagingTitleTemplate", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to enable Display Name.
    /// </summary>
    public bool EnableDisplayName
    {
        get => this.Registry.GetValue("EnableDisplayName", false);

        set => this.Registry.SetValue("EnableDisplayName", value);
    }

    /// <summary>
    /// Gets or sets Theme.
    /// </summary>
    public string Theme
    {
        get => this.RegistryBoard.GetValue("Theme", "yaf");

        set => this.RegistryBoard.SetValue("Theme", value);
    }

    /// <summary>
    /// Gets or sets Language.
    /// </summary>
    public string Language
    {
        get => this.RegistryBoard.GetValue("Language", "english.json");

        set => this.RegistryBoard.SetValue("Language", value);
    }

    /// <summary>
    /// Gets or sets Culture.
    /// </summary>
    public string Culture
    {
        get => this.RegistryBoard.GetValue("Culture", "en-US");

        set => this.RegistryBoard.SetValue("Culture", value);
    }

    /// <summary>
    /// Gets or sets Default Notification Setting.
    /// </summary>
    public UserNotificationSetting DefaultNotificationSetting
    {
        get => this.RegistryBoard.GetValue("DefaultNotificationSetting", UserNotificationSetting.NoNotification);

        set => this.RegistryBoard.SetValue("DefaultNotificationSetting", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Default Send Digest Email.
    /// </summary>
    public bool DefaultSendDigestEmail
    {
        get => this.RegistryBoard.GetValue("DefaultSendDigestEmail", false);

        set => this.RegistryBoard.SetValue("DefaultSendDigestEmail", value);
    }

    /// <summary>
    /// Gets or sets the forum default access mask.
    /// </summary>
    public int ForumDefaultAccessMask
    {
        get => this.RegistryBoard.GetValue("ForumDefaultAccessMask", 3);

        set => this.RegistryBoard.SetValue("ForumDefaultAccessMask", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Allow Digest Email.
    /// </summary>
    public bool AllowDigestEmail
    {
        get => this.RegistryBoard.GetValue("AllowDigestEmail", false);

        set => this.RegistryBoard.SetValue("AllowDigestEmail", value);
    }

    /// <summary>
    /// Gets or sets ShowTopicsDefault.
    /// </summary>
    public int ShowTopicsDefault
    {
        get => this.RegistryBoard.GetValue("ShowTopicsDefault", 0);

        set => this.RegistryBoard.SetValue("ShowTopicsDefault", value);
    }

    /// <summary>
    /// Gets or sets default page size for new users
    /// </summary>
    public int PageSizeDefault {
        get => this.RegistryBoard.GetValue("PageSizeDefault", 5);

        set => this.RegistryBoard.SetValue("PageSizeDefault", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [log error].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [log error]; otherwise, <c>false</c>.
    /// </value>
    public bool LogError
    {
        get => this.Registry.GetValue("LogError", true);

        set => this.Registry.SetValue("LogError", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [log warning].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [log warning]; otherwise, <c>false</c>.
    /// </value>
    public bool LogWarning
    {
        get => this.Registry.GetValue("LogWarning", true);

        set => this.Registry.SetValue("LogWarning", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [log banned IP].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [log banned IP]; otherwise, <c>false</c>.
    /// </value>
    public bool LogBannedIP
    {
        get => this.Registry.GetValue("LogBannedIP", false);

        set => this.Registry.SetValue("LogBannedIP", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [log information].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [log information]; otherwise, <c>false</c>.
    /// </value>
    public bool LogInformation
    {
        get => this.Registry.GetValue("LogInformation", true);

        set => this.Registry.SetValue("LogInformation", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [log user deleted].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [log user deleted]; otherwise, <c>false</c>.
    /// </value>
    public bool LogUserDeleted
    {
        get => this.Registry.GetValue("LogUserDeleted", false);

        set => this.Registry.SetValue("LogUserDeleted", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [log user suspended unsuspended].
    /// </summary>
    /// <value>
    /// <c>true</c> if [log user suspended unsuspended]; otherwise, <c>false</c>.
    /// </value>
    public bool LogUserSuspendedUnsuspended
    {
        get => this.Registry.GetValue("LogUserSuspendedUnsuspended", false);

        set => this.Registry.SetValue("LogUserSuspendedUnsuspended", value);
    }

    /// <summary>
    /// Gets or sets NotificationOnUserRegisterEmailList.
    /// </summary>
    public string NotificationOnUserRegisterEmailList
    {
        get => this.RegistryBoard.GetValue<string>("NotificationOnUserRegisterEmailList", null);

        set => this.RegistryBoard.SetValue("NotificationOnUserRegisterEmailList", value);
    }

    /// <summary>
    /// Gets or sets the hide copyright.
    /// </summary>
    /// <value>The hide copyright.</value>
    public bool HideCopyright
    {
        get => this.RegistryBoard.GetValue("HideCopyright", false);

        set => this.RegistryBoard.SetValue("HideCopyright", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Email Moderators On Moderated Post.
    /// </summary>
    public bool EmailModeratorsOnModeratedPost
    {
        get => this.RegistryBoard.GetValue("EmailModeratorsOnModeratedPost", true);

        set => this.RegistryBoard.SetValue("EmailModeratorsOnModeratedPost", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Email Moderators On Reported Post.
    /// </summary>
    public bool EmailModeratorsOnReportedPost
    {
        get => this.RegistryBoard.GetValue("EmailModeratorsOnReportedPost", true);

        set => this.RegistryBoard.SetValue("EmailModeratorsOnReportedPost", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether send an
    /// Email when a Single User is awarded with a Medal.
    /// </summary>
    public bool EmailUserOnMedalAward
    {
        get => this.RegistryBoard.GetValue("EmailUserOnMedalAward", true);

        set => this.RegistryBoard.SetValue("EmailUserOnMedalAward", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [show cookie consent].
    /// </summary>
    public bool ShowCookieConsent
    {
        get => this.RegistryBoard.GetValue("ShowCookieConsent", true);

        set => this.RegistryBoard.SetValue("ShowCookieConsent", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to go to post Anchor.
    /// </summary>
    public bool ScrollToPost
    {
        get => this.RegistryBoard.GetValue("ScrollToPost", true);

        set => this.RegistryBoard.SetValue("ScrollToPost", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether show scroll back to top button.
    /// </summary>
    public bool ShowScrollBackToTopButton
    {
        get => this.RegistryBoard.GetValue("ShowScrollBackToTopButton", true);

        set => this.RegistryBoard.SetValue("ShowScrollBackToTopButton", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether
    /// sending the user a welcome notification after register
    /// 0 = No Mail
    /// 1 = Send as Mail Message
    /// 2 = Send as Private Message
    /// </summary>
    public int SendWelcomeNotificationAfterRegister
    {
        get => this.Registry.GetValue("SendWelcomeNotificationAfterRegister", 1);

        set => this.Registry.SetValue("SendWelcomeNotificationAfterRegister", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether
    /// which Spam Service Type should be used
    /// </summary>
    public SpamService SpamService => this.SpamServiceType.ToEnum<SpamService>();

    /// <summary>
    /// Gets or sets a value indicating whether
    /// which Spam Service Type should be used
    /// </summary>
    public int SpamServiceType
    {
        get => this.Registry.GetValue("SpamServiceType", SpamService.Internal.ToInt());

        set => this.Registry.SetValue("SpamServiceType", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether which Spam Service Type should be used
    /// </summary>
    public BotSpamService BotSpamService => this.BotSpamServiceType.ToEnum<BotSpamService>();

    /// <summary>
    /// Gets or sets a value indicating whether
    /// which Spam Service Type should be used
    /// </summary>
    public int BotSpamServiceType
    {
        get => this.Registry.GetValue("BotSpamServiceType", BotSpamService.NoService.ToInt());

        set => this.Registry.SetValue("BotSpamServiceType", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether how to handle a SPAM Message
    /// </summary>
    public SpamPostHandling SpamPostHandling => this.SpamMessageHandling.ToEnum<SpamPostHandling>();

    /// <summary>
    /// Gets or sets a value indicating whether how to handle with a SPAM Message
    /// </summary>
    public int SpamMessageHandling
    {
        get => this.Registry.GetValue("SpamMessageHandling", SpamPostHandling.DoNothing.ToInt());

        set => this.Registry.SetValue("SpamMessageHandling", value);
    }

    /// <summary>
    /// How to handle Bots during Registration
    /// 0 = Disabled
    /// 1 = Log and Send Message to Admins
    /// 2 = Block user from Registration
    /// </summary>
    public int BotHandlingOnRegister
    {
        get => this.Registry.GetValue("BotHandlingOnRegister", 0);

        set => this.Registry.SetValue("BotHandlingOnRegister", value);
    }

    /// <summary>
    /// Gets or sets the amount of posts a user has to have to be ignored
    /// by the spam check
    /// </summary>
    /// <value>
    /// The ignore spam word check post count.
    /// </value>
    public int IgnoreSpamWordCheckPostCount
    {
        get => this.Registry.GetValue("IgnoreSpamWordCheckPostCount", 20);

        set => this.Registry.SetValue("IgnoreSpamWordCheckPostCount", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [ban bot IP on detection].
    /// </summary>
    /// <value>
    /// <c>true</c> if [ban bot IP on detection]; otherwise, <c>false</c>.
    /// </value>
    public bool BanBotIpOnDetection
    {
        get => this.Registry.GetValue("BanBotIpOnDetection", false);

        set => this.Registry.SetValue("BanBotIpOnDetection", value);
    }

    /// <summary>
    /// Gets or sets the BotScout.com API key.
    /// </summary>
    /// <value>
    /// The BotScout.com API key.
    /// </value>
    public string BotScoutApiKey
    {
        get => this.Registry.GetValue("BotScoutApiKey", string.Empty);

        set => this.Registry.SetValue("BotScoutApiKey", value);
    }

    /// <summary>
    /// Gets or sets the StopForumSpam.com API key.
    /// </summary>
    /// <value>
    /// The StopForumSpam.com API key.
    /// </value>
    public string StopForumSpamApiKey
    {
        get => this.Registry.GetValue("StopForumSpamApiKey", string.Empty);

        set => this.Registry.SetValue("StopForumSpamApiKey", value);
    }

    /// <summary>
    /// Gets or sets the allowed number of URLs before the message is flagged as spam.
    /// </summary>
    /// <value>
    /// The allowed number of URLs.
    /// </value>
    public int AllowedNumberOfUrls
    {
        get => this.Registry.GetValue("AllowedNumberOfUrls", 10);

        set => this.Registry.SetValue("AllowedNumberOfUrls", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Show Share Topic To.
    /// </summary>
    public int ShowShareTopicTo
    {
        get => this.Registry.GetValue("ShowShareTopicTo", 1);

        set => this.Registry.SetValue("ShowShareTopicTo", value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether Show Team Page To or not.
    /// </summary>
    public int ShowTeamTo
    {
        get => this.Registry.GetValue("ShowTeamTo", 0);

        set => this.Registry.SetValue("ShowTeamTo", value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether Show Help Page To or not.
    /// </summary>
    public int ShowHelpTo
    {
        get => this.Registry.GetValue("ShowHelpTo", 2);

        set => this.Registry.SetValue("ShowHelpTo", value);
    }

    /// <summary>
    /// Gets or sets ServerTimeCorrection.
    /// </summary>
    public int ServerTimeCorrection
    {
        get => this.Registry.GetValue("ServerTimeCorrection", 0);

        set => this.Registry.SetValue("ServerTimeCorrection", value);
    }

    /// <summary>
    /// Gets or sets PostLatestFeedAccess.
    /// </summary>
    public int PostLatestFeedAccess
    {
        get => this.Registry.GetValue("PostLatestFeedAccess", 1);

        set => this.Registry.SetValue("PostLatestFeedAccess", value);
    }

    /// <summary>
    /// Gets or sets PostsFeedAccess.
    /// </summary>
    public int PostsFeedAccess
    {
        get => this.Registry.GetValue("PostsFeedAccess", 1);

        set => this.Registry.SetValue("PostsFeedAccess", value);
    }

    /// <summary>
    /// Gets or sets DigestSendEveryXHours.
    /// </summary>
    public int DigestSendEveryXHours
    {
        get => this.Registry.GetValue("DigestSendEveryXHours", 24);

        set => this.Registry.SetValue("DigestSendEveryXHours", value);
    }

    /// <summary>
    /// Gets or sets the update search index every x hours.
    /// </summary>
    public int UpdateSearchIndexEveryXHours
    {
        get => this.Registry.GetValue("UpdateSearchIndex", 24);

        set => this.Registry.SetValue("UpdateSearchIndex", value);
    }

    /// <summary>
    /// Gets or sets TopicsFeedAccess.
    /// </summary>
    public int TopicsFeedAccess
    {
        get => this.Registry.GetValue("TopicsFeedAccess", 1);

        set => this.Registry.SetValue("TopicsFeedAccess", value);
    }

    /// <summary>
    /// Gets or sets AvatarWidth.
    /// </summary>
    public int AvatarWidth
    {
        get => this.Registry.GetValue("AvatarWidth", 200);

        set => this.Registry.SetValue("AvatarWidth", value);
    }

    /// <summary>
    /// Gets or sets AvatarHeight.
    /// </summary>
    public int AvatarHeight
    {
        get => this.Registry.GetValue("AvatarHeight", 100);

        set => this.Registry.SetValue("AvatarHeight", value);
    }

    /// <summary>
    /// Gets or sets AllowCreateTopicsSameName.
    /// </summary>
    public int AllowCreateTopicsSameName
    {
        get => this.Registry.GetValue("AllowCreateTopicsSameName", 0);

        set => this.Registry.SetValue("AllowCreateTopicsSameName", value);
    }

    /// <summary>
    /// Gets or sets AvatarSize.
    /// </summary>
    public int AvatarSize
    {
        get => this.Registry.GetValue("AvatarSize", 50000);

        set => this.Registry.SetValue("AvatarSize", value);
    }

    /// <summary>
    /// Gets or sets MaxFileSize.
    /// </summary>
    public int MaxFileSize
    {
        get => this.Registry.GetValue("MaxFileSize", 0);

        set => this.Registry.SetValue("MaxFileSize", value);
    }

    /// <summary>
    /// Gets or sets Message History Days To Trace.
    /// </summary>
    public int MessageHistoryDaysToLog
    {
        get => this.Registry.GetValue("MessageHistoryDaysToLog", 30);

        set => this.Registry.SetValue("MessageHistoryDaysToLog", value);
    }

    /// <summary>
    /// Gets or sets LockPosts.
    /// </summary>
    public int LockPosts
    {
        get => this.Registry.GetValue("LockPosts", 0);

        set => this.Registry.SetValue("LockPosts", value);
    }

    /// <summary>
    /// Gets or sets the amount of Sub Forums In Forums List.
    /// </summary>
    public int SubForumsInForumList
    {
        get => this.Registry.GetValue("SubForumsInForumList", 5);

        set => this.Registry.SetValue("SubForumsInForumList", value);
    }

    /// <summary>
    /// Gets or sets PostsPerPage.
    /// </summary>
    public int PostsPerPage
    {
        get => this.Registry.GetValue("PostsPerPage", 20);

        set => this.Registry.SetValue("PostsPerPage", value);
    }

    /// <summary>
    /// Gets or sets PostFloodDelay.
    /// </summary>
    public int PostFloodDelay
    {
        get => this.Registry.GetValue("PostFloodDelay", 30);

        set => this.Registry.SetValue("PostFloodDelay", value);
    }

    /// <summary>
    /// Gets or sets AllowedPollChoiceNumber.
    /// </summary>
    public int AllowedPollChoiceNumber
    {
        get => this.Registry.GetValue("AllowedPollChoiceNumber", 10);

        set => this.Registry.SetValue("AllowedPollChoiceNumber", value);
    }

    /// <summary>
    /// Gets or sets EditTimeOut.
    /// </summary>
    public int EditTimeOut
    {
        get => this.Registry.GetValue("EditTimeOut", 30);

        set => this.Registry.SetValue("EditTimeOut", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether someone can report posts as violating forum rules.
    /// </summary>
    public int ReportPostPermissions
    {
        get => this.Registry.GetValue("ReportPostPermissions", (int)ViewPermissions.RegisteredUsers);

        set => this.Registry.SetValue("ReportPostPermissions", value);
    }

    /// <summary>
    /// Gets or sets ProfileViewPermissions.
    /// </summary>
    public int ProfileViewPermissions
    {
        get => this.Registry.GetValue("ProfileViewPermission", (int)ViewPermissions.RegisteredUsers);

        set => this.Registry.SetValue("ProfileViewPermission", value);
    }

    /// <summary>
    /// Gets or sets ReturnSearchMax.
    /// </summary>
    public int ReturnSearchMax
    {
        get => this.Registry.GetValue("ReturnSearchMax", 1000);

        set => this.Registry.SetValue("ReturnSearchMax", value);
    }

    /// <summary>
    /// Gets or sets ActiveUsersViewPermissions.
    /// </summary>
    public int ActiveUsersViewPermissions
    {
        get => this.Registry.GetValue("ActiveUsersViewPermissions", (int)ViewPermissions.RegisteredUsers);

        set => this.Registry.SetValue("ActiveUsersViewPermissions", value);
    }

    /// <summary>
    /// Gets or sets MembersListViewPermissions.
    /// </summary>
    public int MembersListViewPermissions
    {
        get => this.Registry.GetValue("MembersListViewPermissions", (int)ViewPermissions.RegisteredUsers);

        set => this.Registry.SetValue("MembersListViewPermissions", value);
    }

    /// <summary>
    /// Gets or sets ActiveDiscussionsCount.
    /// </summary>
    public int ActiveDiscussionsCount
    {
        get => this.Registry.GetValue("ActiveDiscussionsCount", 5);

        set => this.Registry.SetValue("ActiveDiscussionsCount", value);
    }

    /// <summary>
    /// Gets or sets ActiveDiscussionsCacheTimeout.
    /// </summary>
    public int ActiveDiscussionsCacheTimeout
    {
        get => this.Registry.GetValue("ActiveDiscussionsCacheTimeout", 1);

        set => this.Registry.SetValue("ActiveDiscussionsCacheTimeout", value);
    }

    /// <summary>
    /// Gets or sets SearchStringMinLength.
    /// </summary>
    public int SearchStringMinLength
    {
        get => this.Registry.GetValue("SearchStringMinLength", 4);

        set => this.Registry.SetValue("SearchStringMinLength", value);
    }

    /// <summary>
    /// Gets or sets SearchPermissions.
    /// </summary>
    public int SearchPermissions
    {
        get => this.Registry.GetValue("SearchPermissions", (int)ViewPermissions.Everyone);

        set => this.Registry.SetValue("SearchPermissions", value);
    }

    /// <summary>
    /// Gets or sets ForumStatisticsCacheTimeout.
    /// </summary>
    public int ForumStatisticsCacheTimeout
    {
        get => this.Registry.GetValue("ForumStatisticsCacheTimeout", 60);

        set => this.Registry.SetValue("ForumStatisticsCacheTimeout", value);
    }

    /// <summary>
    /// Gets or sets BoardUserStatsCacheTimeout.
    /// </summary>
    public int BoardUserStatsCacheTimeout
    {
        get => this.Registry.GetValue("BoardUserStatsCacheTimeout", 60);

        set => this.Registry.SetValue("BoardUserStatsCacheTimeout", value);
    }

    /// <summary>
    /// Gets or sets DisableNoFollowLinksAfterDay.
    /// </summary>
    public int DisableNoFollowLinksAfterDay
    {
        get => this.Registry.GetValue("DisableNoFollowLinksAfterDay", 0);

        set => this.Registry.SetValue("DisableNoFollowLinksAfterDay", value);
    }

    /// <summary>
    /// Gets or sets BoardModeratorsCacheTimeout.
    /// </summary>
    public int BoardModeratorsCacheTimeout
    {
        get => this.Registry.GetValue("BoardModeratorsCacheTimeout", 1440);

        set => this.Registry.SetValue("BoardModeratorsCacheTimeout", value);
    }

    /// <summary>
    /// Gets or sets Max. Post Size.
    /// </summary>
    public int MaxPostSize
    {
        get => this.Registry.GetValue<int>("MaxPostSize", short.MaxValue);

        set => this.Registry.SetValue("MaxPostSize", value);
    }

    /// <summary>
    /// Gets or sets MaxReportPostChars.
    /// </summary>
    public int MaxReportPostChars
    {
        get => this.Registry.GetValue("MaxReportPostChars", 128);

        set => this.Registry.SetValue("MaxReportPostChars", value);
    }

    /// <summary>
    /// Gets or sets Picture Attachment Display Threshold.
    /// </summary>
    public int PictureAttachmentDisplayTreshold
    {
        get => this.Registry.GetValue("PictureAttachmentDisplayTreshold", 262144);

        set => this.Registry.SetValue("PictureAttachmentDisplayTreshold", value);
    }

    /// <summary>
    /// Gets or sets ImageAttachmentResizeWidth.
    /// </summary>
    public int ImageAttachmentResizeWidth
    {
        get => this.Registry.GetValue("ImageAttachmentResizeWidth", 2000);

        set => this.Registry.SetValue("ImageAttachmentResizeWidth", value);
    }

    /// <summary>
    /// Gets or sets ImageAttachmentResizeHeight.
    /// </summary>
    public int ImageAttachmentResizeHeight
    {
        get => this.Registry.GetValue("ImageAttachmentResizeHeight", 2000);

        set => this.Registry.SetValue("ImageAttachmentResizeHeight", value);
    }

    /// <summary>
    /// Gets or sets the image thumbnail max width.
    /// </summary>
    public int ImageThumbnailMaxWidth
    {
        get => this.Registry.GetValue("ImageThumbnailMaxWidth", 200);

        set => this.Registry.SetValue("ImageThumbnailMaxWidth", value);
    }

    /// <summary>
    /// Gets or sets the image thumbnail max height.
    /// </summary>
    public int ImageThumbnailMaxHeight
    {
        get => this.Registry.GetValue("ImageThumbnailMaxHeight", 200);

        set => this.Registry.SetValue("ImageThumbnailMaxHeight", value);
    }

    /// <summary>
    /// Gets or sets ActiveListTime.
    /// </summary>
    public int ActiveListTime
    {
        get => this.RegistryBoard.GetValue("ActiveListTime", 5);

        set => this.RegistryBoard.SetValue("ActiveListTime", value);
    }

    /// <summary>
    /// Gets or sets User Lazy Data Cache Timeout.
    /// </summary>
    public int ActiveUserLazyDataCacheTimeout
    {
        get => this.Registry.GetValue("ActiveUserLazyDataCacheTimeout", 10);

        set => this.Registry.SetValue("ActiveUserLazyDataCacheTimeout", value);
    }

    /// <summary>
    /// Gets or sets OnlineStatusCacheTimeout.
    /// </summary>
    public int OnlineStatusCacheTimeout
    {
        get => this.Registry.GetValue("OnlineStatusCacheTimeout", 60000);

        set => this.Registry.SetValue("OnlineStatusCacheTimeout", value);
    }

    /// <summary>
    /// Gets or sets User/Display Name Max Length.
    /// </summary>
    public int UserNameMaxLength
    {
        get => this.Registry.GetValue("UserNameMaxLength", 50);

        set => this.Registry.SetValue("UserNameMaxLength", value);
    }

    /// <summary>
    /// Gets or sets User/Display Name Min Length.
    /// </summary>
    public int DisplayNameMinLength
    {
        get => this.Registry.GetValue("DisplayNameMinLength", 3);

        set => this.Registry.SetValue("DisplayNameMinLength", value);
    }

    /// <summary>
    /// Gets or sets Event Log Max Messages.
    /// </summary>
    public int EventLogMaxMessages
    {
        get => this.Registry.GetValue("EventLogMaxMessages", 1050);

        set => this.Registry.SetValue("EventLogMaxMessages", value);
    }

    /// <summary>
    /// Gets or sets Event Log Max Days.
    /// </summary>
    public int EventLogMaxDays
    {
        get => this.Registry.GetValue("EventLogMaxDays", 365);

        set => this.Registry.SetValue("EventLogMaxDays", value);
    }

    /// <summary>
    /// Gets or sets Message Notification Duration
    /// </summary>
    public int MessageNotifcationDuration
    {
        get => this.Registry.GetValue("MessageNotifcationDuration", 30);

        set => this.Registry.SetValue("MessageNotifcationDuration", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [allow forums with same name].
    /// </summary>
    public bool AllowForumsWithSameName
    {
        get => this.Registry.GetValue("AllowForumsWithSameName", false);

        set => this.Registry.SetValue("AllowForumsWithSameName", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Use Read Tracking By Database is enabled.
    /// </summary>
    public bool UseReadTrackingByDatabase
    {
        get => this.Registry.GetValue("UseReadTrackingByDatabase", true);

        set => this.Registry.SetValue("UseReadTrackingByDatabase", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Enable IP Info Service.
    /// </summary>
    public bool EnableIPInfoService
    {
        get => this.Registry.GetValue("EnableIPInfoService", false);

        set => this.Registry.SetValue("EnableIPInfoService", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowMoved.
    /// </summary>
    public bool ShowMoved
    {
        get => this.Registry.GetValue("ShowMoved", true);

        set => this.Registry.SetValue("ShowMoved", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Show Guests In Detailed Active List.
    /// </summary>
    public bool ShowGuestsInDetailedActiveList
    {
        get => this.Registry.GetValue("ShowGuestsInDetailedActiveList", false);

        set => this.Registry.SetValue("ShowGuestsInDetailedActiveList", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Show Crawlers In Active List.
    /// </summary>
    public bool ShowCrawlersInActiveList
    {
        get => this.Registry.GetValue("ShowCrawlersInActiveList", false);

        set => this.Registry.SetValue("ShowCrawlersInActiveList", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether BlankLinks.
    /// </summary>
    public bool BlankLinks
    {
        get => this.Registry.GetValue("BlankLinks", true);

        set => this.Registry.SetValue("BlankLinks", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowUserTheme.
    /// </summary>
    public bool AllowUserTheme
    {
        get => this.Registry.GetValue("AllowUserTheme", false);

        set => this.Registry.SetValue("AllowUserTheme", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowUserHideHimself.
    /// </summary>
    public bool AllowUserHideHimself
    {
        get => this.Registry.GetValue("AllowUserHideHimself", false);

        set => this.Registry.SetValue("AllowUserHideHimself", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowUserLanguage.
    /// </summary>
    public bool AllowUserLanguage
    {
        get => this.Registry.GetValue("AllowUserLanguage", false);

        set => this.Registry.SetValue("AllowUserLanguage", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowModeratorsViewIPs.
    /// </summary>
    public bool AllowModeratorsViewIPs
    {
        get => this.Registry.GetValue("AllowModeratorsViewIPs", false);

        set => this.Registry.SetValue("AllowModeratorsViewIPs", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowPollChangesAfterFirstVote.
    /// A poll creator can't change choices after the first vote.
    /// </summary>
    public bool AllowPollChangesAfterFirstVote
    {
        get => this.Registry.GetValue("AllowPollChangesAfterFirstVote", false);

        set => this.Registry.SetValue("AllowPollChangesAfterFirstVote", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowUsersHidePollResults.
    /// </summary>
    public bool AllowUsersHidePollResults
    {
        get => this.Registry.GetValue("AllowViewPollVotesIfNoPollAcces", true);

        set => this.Registry.SetValue("AllowViewPollVotesIfNoPollAcces", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether if Use Farsi Calendar
    /// </summary>
    public bool UseFarsiCalender
    {
        get => this.Registry.GetValue("UseFarsiCalender", false);

        set => this.Registry.SetValue("UseFarsiCalender", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether if relative times are used on the forum.
    /// </summary>
    public bool ShowRelativeTime
    {
        get => this.Registry.GetValue("ShowRelativeTime", true);

        set => this.Registry.SetValue("ShowRelativeTime", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowMultipleChoices.
    /// </summary>
    public bool AllowMultipleChoices
    {
        get => this.Registry.GetValue("AllowMultipleChoices", true);

        set => this.Registry.SetValue("AllowMultipleChoices", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowUsersImagedPoll.
    /// </summary>
    public bool AllowUsersImagedPoll
    {
        get => this.Registry.GetValue("AllowUsersImagedPoll", false);

        set => this.Registry.SetValue("AllowUsersImagedPoll", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether AvatarUpload.
    /// </summary>
    public bool AvatarUpload
    {
        get => this.Registry.GetValue("AvatarUpload", false);

        set => this.Registry.SetValue("AvatarUpload", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Avatar Gallery.
    /// </summary>
    public bool AvatarGallery
    {
        get => this.Registry.GetValue("AvatarGallery", true);

        set => this.Registry.SetValue("AvatarGallery", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowEmailChange.
    /// </summary>
    public bool AllowEmailChange
    {
        get => this.Registry.GetValue("AllowEmailChange", true);

        set => this.Registry.SetValue("AllowEmailChange", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether UseFileTable.
    /// </summary>
    public bool UseFileTable
    {
        get => this.Registry.GetValue("UseFileTable", false);

        set => this.Registry.SetValue("UseFileTable", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowAtomLink.
    /// </summary>
    public bool ShowAtomLink
    {
        get => this.Registry.GetValue("ShowAtomLink", false);

        set => this.Registry.SetValue("ShowAtomLink", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowPageGenerationTime.
    /// </summary>
    public bool ShowPageGenerationTime
    {
        get => this.Registry.GetValue("ShowPageGenerationTime", true);

        set => this.Registry.SetValue("ShowPageGenerationTime", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowYAFVersion.
    /// </summary>
    public bool ShowYAFVersion
    {
        get => this.Registry.GetValue("ShowYAFVersion", true);

        set => this.Registry.SetValue("ShowYAFVersion", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowForumJump.
    /// </summary>
    public bool ShowForumJump
    {
        get => this.Registry.GetValue("ShowForumJump", true);

        set => this.Registry.SetValue("ShowForumJump", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowPrivateMessages.
    /// </summary>
    public bool AllowPrivateMessages
    {
        get => this.Registry.GetValue("AllowPrivateMessages", true);

        set => this.Registry.SetValue("AllowPrivateMessages", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowEmailSending.
    /// </summary>
    public bool AllowEmailSending
    {
        get => this.Registry.GetValue("AllowEmailSending", true);

        set => this.Registry.SetValue("AllowEmailSending", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether AllowSignatures.
    /// </summary>
    public bool AllowSignatures
    {
        get => this.Registry.GetValue("AllowSignatures", true);

        set => this.Registry.SetValue("AllowSignatures", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Enable Quick Search.
    /// </summary>
    public bool ShowQuickSearch
    {
        get => this.Registry.GetValue("ShowQuickSearch", false);

        set => this.Registry.SetValue("ShowQuickSearch", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether RemoveNestedQuotes.
    /// </summary>
    public bool RemoveNestedQuotes
    {
        get => this.Registry.GetValue("RemoveNestedQuotes", false);

        set => this.Registry.SetValue("RemoveNestedQuotes", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether DisableRegistrations.
    /// </summary>
    public bool DisableRegistrations
    {
        get => this.Registry.GetValue("DisableRegistrations", false);

        set => this.Registry.SetValue("DisableRegistrations", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowGroupsProfile.
    /// </summary>
    public bool ShowGroupsProfile
    {
        get => this.Registry.GetValue("ShowGroupsProfile", false);

        set => this.Registry.SetValue("ShowGroupsProfile", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowAdsToSignedInUsers.
    /// </summary>
    public bool ShowAdsToSignedInUsers
    {
        get => this.Registry.GetValue("ShowAdsToSignedInUsers", true);

        set => this.Registry.SetValue("ShowAdsToSignedInUsers", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowQuickAnswer.
    /// </summary>
    public bool ShowQuickAnswer
    {
        get => this.Registry.GetValue("ShowQuickAnswer", true);

        set => this.Registry.SetValue("ShowQuickAnswer", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowDeletedMessages.
    /// </summary>
    public bool ShowDeletedMessages
    {
        get => this.Registry.GetValue("ShowDeletedMessages", true);

        set => this.Registry.SetValue("ShowDeletedMessages", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowDeletedMessagesToAll.
    /// </summary>
    public bool ShowDeletedMessagesToAll
    {
        get => this.Registry.GetValue("ShowDeletedMessagesToAll", false);

        set => this.Registry.SetValue("ShowDeletedMessagesToAll", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowModeratorList.
    /// </summary>
    public bool ShowModeratorList
    {
        get => this.Registry.GetValue("ShowModeratorList", true);

        set => this.Registry.SetValue("ShowModeratorList", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether UseNoFollowLinks.
    /// </summary>
    public bool UseNoFollowLinks
    {
        get => this.Registry.GetValue("UseNoFollowLinks", true);

        set => this.Registry.SetValue("UseNoFollowLinks", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether EnableImageAttachmentResize.
    /// </summary>
    public bool EnableImageAttachmentResize
    {
        get => this.Registry.GetValue("EnableImageAttachmentResize", true);

        set => this.Registry.SetValue("EnableImageAttachmentResize", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether display No-Count Forums In ActiveDiscussions.
    /// </summary>
    public bool NoCountForumsInActiveDiscussions
    {
        get => this.Registry.GetValue("NoCountForumsInActiveDiscussions", true);

        set => this.Registry.SetValue("NoCountForumsInActiveDiscussions", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether UseStyledNicks.
    /// </summary>
    public bool UseStyledNicks
    {
        get => this.Registry.GetValue("UseStyledNicks", true);

        set => this.Registry.SetValue("UseStyledNicks", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Use Styled Topic Titles.
    /// </summary>
    public bool UseStyledTopicTitles
    {
        get => this.Registry.GetValue("UseStyledTopicTitles", false);

        set => this.Registry.SetValue("UseStyledTopicTitles", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowUserOnlineStatus.
    /// </summary>
    public bool ShowUserOnlineStatus
    {
        get => this.Registry.GetValue("ShowUserOnlineStatus", true);

        set => this.Registry.SetValue("ShowUserOnlineStatus", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowThanksDate.
    /// </summary>
    public bool ShowThanksDate
    {
        get => this.Registry.GetValue("ShowThanksDate", true);

        set => this.Registry.SetValue("ShowThanksDate", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether EnableBuddyList.
    /// </summary>
    public bool EnableBuddyList
    {
        get => this.Registry.GetValue("EnableBuddyList", true);

        set => this.Registry.SetValue("EnableBuddyList", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether EnableAlbum.
    /// </summary>
    public bool EnableAlbum
    {
        get => this.Registry.GetValue("EnableAlbum", false);

        set => this.Registry.SetValue("EnableAlbum", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use Custom Context Menu
    /// or the Default browser Context Menu
    /// </summary>
    public bool UseCustomContextMenu
    {
        get => this.Registry.GetValue("UseCustomContextMenu", true);

        set => this.Registry.SetValue("UseCustomContextMenu", value);
    }

    /// <summary>
    /// Gets or sets AlbumImagesSizeMax.
    /// </summary>
    public int AlbumImagesSizeMax
    {
        get => this.RegistryBoard.GetValue("AlbumImagesSizeMax", 1048576);

        set => this.RegistryBoard.SetValue("AlbumImagesSizeMax", value);
    }

    /// <summary>
    /// Gets or sets AlbumsPerPage.
    /// </summary>
    public int AlbumsPerPage
    {
        get => this.RegistryBoard.GetValue("AlbumsPerPage", 6);

        set => this.RegistryBoard.SetValue("AlbumsPerPage", value);
    }

    /// <summary>
    /// Gets or sets AlbumImagesPerPage.
    /// </summary>
    public int AlbumImagesPerPage
    {
        get => this.RegistryBoard.GetValue("AlbumImagesPerPage", 10);

        set => this.RegistryBoard.SetValue("AlbumImagesPerPage", value);
    }

    /// <summary>
    /// Gets or sets the Number of Views a Topic must have to became
    /// Hot.
    /// </summary>
    public int PopularTopicViews
    {
        get => this.RegistryBoard.GetValue("PopularTopicViews", 100);

        set => this.RegistryBoard.SetValue("PopularTopicViews", value);
    }

    /// <summary>
    /// Gets or sets the Number of Replies a Topic must have to became
    /// Hot.
    /// </summary>
    public int PopularTopicReplys
    {
        get => this.RegistryBoard.GetValue("PopularTopicReplys", 10);

        set => this.RegistryBoard.SetValue("PopularTopicReplys", value);
    }

    /// <summary>
    /// Gets or sets the Number of Days a topic must have a Reply in t
    /// remain Hot (Popular)
    /// </summary>
    public int PopularTopicDays
    {
        get => this.RegistryBoard.GetValue("PopularTopicDays", 7);

        set => this.RegistryBoard.SetValue("PopularTopicDays", value);
    }

    /// <summary>
    /// Gets or sets the Number of Topics to show on the Forum Feed
    /// </summary>
    public int TopicsFeedItemsCount
    {
        get => this.RegistryBoard.GetValue("TopicsFeedItemsCount", 20);

        set => this.RegistryBoard.SetValue("TopicsFeedItemsCount", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [allow display name modification].
    /// </summary>
    public bool AllowDisplayNameModification
    {
        get => this.Registry.GetValue("AllowDisplayNameModification", true);

        set => this.Registry.SetValue("AllowDisplayNameModification", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [show connect message in topic].
    /// </summary>
    /// <value>
    /// <c>true</c> if [show connect message in topic]; otherwise, <c>false</c>.
    /// </value>
    public bool ShowConnectMessageInTopic
    {
        get => this.Registry.GetValue("ShowConnectMessageInTopic", true);

        set => this.Registry.SetValue("ShowConnectMessageInTopic", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use user info hover Cards.
    /// </summary>
    public bool EnableUserInfoHoverCards
    {
        get => this.Registry.GetValue("EnableUserInfoHoverCards", true);

        set => this.Registry.SetValue("EnableUserInfoHoverCards", value);
    }

    /// <summary>
    /// Gets or sets the hover card open delay.
    /// </summary>
    /// <value>
    /// The hover card open delay.
    /// </value>
    public int HoverCardOpenDelay
    {
        get => this.Registry.GetValue("HoverCardOpenDelay", 2000);

        set => this.Registry.SetValue("HoverCardOpenDelay", value);
    }

    /// <summary>
    /// Gets or sets IPInfo page Url.
    /// </summary>
    public string IPInfoPageURL
    {
        get => this.Registry.GetValue("IPInfoPageURL", "https://www.ip2location.com/{0}");

        set => this.Registry.SetValue("IPInfoPageURL", value);
    }

    /// <summary>
    /// Gets or sets IP Locator Path.
    /// </summary>
    public string IPLocatorUrlPath
    {
        get => this.Registry.GetValue("IPLocatorUrlPath", "https://api.ipinfodb.com/v3/ip-city/?key=<your_api_key>&ip={0}");

        set => this.Registry.SetValue("IPLocatorUrlPath", value);
    }

    /// <summary>
    /// Gets or sets the abuse ip database API key.
    /// </summary>
    /// <value>The abuse ip database API key.</value>
    public string AbuseIpDbApiKey {
        get => this.Registry.GetValue("AbuseIpDbApiKey", string.Empty);

        set => this.Registry.SetValue("AbuseIpDbApiKey", value);
    }

    /// <summary>
    /// Gets or sets Forum Logo.
    /// </summary>
    public string ForumLogo
    {
        get => this.Registry.GetValue("ForumLogo", "YAFLogo.svg");

        set => this.Registry.SetValue("ForumLogo", value);
    }

    /// <summary>
    /// Gets or sets ForumEmail.
    /// </summary>
    public string ForumEmail
    {
        get => this.Registry.GetValue("ForumEmail", string.Empty);

        set => this.Registry.SetValue("ForumEmail", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Enable User Reputation System
    /// </summary>
    public bool EnableUserReputation
    {
        get => this.Registry.GetValue("EnableUserReputation", true);

        set => this.Registry.SetValue("EnableUserReputation", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Allow Negative Reputation
    /// </summary>
    public bool ReputationAllowNegative
    {
        get => this.Registry.GetValue("ReputationAllowNegative", true);

        set => this.Registry.SetValue("ReputationAllowNegative", value);
    }

    /// <summary>
    /// Gets or sets ReputationMaxNegative
    /// </summary>
    public int ReputationMaxNegative
    {
        get => this.Registry.GetValue("ReputationMaxNegative", -100);

        set => this.Registry.SetValue("ReputationMaxNegative", value);
    }

    /// <summary>
    /// Gets or sets ReputationMaxPositive
    /// </summary>
    public int ReputationMaxPositive
    {
        get => this.Registry.GetValue("ReputationMaxPositive", 500);

        set => this.Registry.SetValue("ReputationMaxPositive", value);
    }

    /// <summary>
    /// Gets or sets Minimum Reputation Value to Allow User to Give Reputation
    /// </summary>
    public int ReputationMinUpVoting
    {
        get => this.Registry.GetValue("ReputationMinUpVoting", 1);

        set => this.Registry.SetValue("ReputationMinUpVoting", value);
    }

    /// <summary>
    /// Gets or sets Minimum Reputation Value to Allow User to Remove Reputation
    /// </summary>
    public int ReputationMinDownVoting
    {
        get => this.Registry.GetValue("ReputationMinDownVoting", 100);

        set => this.Registry.SetValue("ReputationMinDownVoting", value);
    }

    /// <summary>
    /// Gets or sets the allowed file extensions.
    /// </summary>
    public string AllowedFileExtensions
    {
        get =>
            this.Registry.GetValue(
                "AllowedFileExtensions",
                "bmp,doc,gif,jpg,jpeg,mov,mp3,mpg,png,rar,tif,txt,xls,xml,zip");

        set => this.Registry.SetValue("AllowedFileExtensions", value.ToLower());
    }

    /// <summary>
    /// Gets or sets AdPost.
    /// </summary>
    public string AdPost
    {
        get => this.Registry.GetValue<string>("AdPost", null);

        set => this.Registry.SetValue("AdPost", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowBrowsingUsers.
    /// </summary>
    public bool ShowBrowsingUsers
    {
        get => this.Registry.GetValue("ShowBrowsingUsers", true);

        set => this.Registry.SetValue("ShowBrowsingUsers", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Show Similar Topics.
    /// </summary>
    public bool ShowSimilarTopics
    {
        get => this.Registry.GetValue("ShowSimilarTopics", true);

        set => this.Registry.SetValue("ShowSimilarTopics", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowMedals.
    /// </summary>
    public bool ShowMedals
    {
        get => this.Registry.GetValue("ShowMedals", true);

        set => this.Registry.SetValue("ShowMedals", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Allow Email Topic.
    /// </summary>
    public bool AllowEmailTopic
    {
        get => this.Registry.GetValue("AllowEmailTopic", true);

        set => this.Registry.SetValue("AllowEmailTopic", value);
    }

    /* Ederon : 12/9/2007 */

    /// <summary>
    /// Gets or sets a value indicating whether RequireLogin.
    /// </summary>
    public bool RequireLogin
    {
        get => this.Registry.GetValue("RequireLogin", false);

        set => this.Registry.SetValue("RequireLogin", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowActiveDiscussions.
    /// </summary>
    public bool ShowActiveDiscussions
    {
        get => this.Registry.GetValue("ShowActiveDiscussions", true);

        set => this.Registry.SetValue("ShowActiveDiscussions", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowForumStatistics.
    /// </summary>
    public bool ShowForumStatistics
    {
        get => this.Registry.GetValue("ShowForumStatistics", true);

        set => this.Registry.SetValue("ShowForumStatistics", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether Show Recent Users.
    /// </summary>
    public bool ShowRecentUsers
    {
        get => this.Registry.GetValue("ShowRecentUsers", false);

        set => this.Registry.SetValue("ShowRecentUsers", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowRulesForRegistration.
    /// </summary>
    public bool ShowRulesForRegistration
    {
        get => this.Registry.GetValue("ShowRulesForRegistration", true);

        set => this.Registry.SetValue("ShowRulesForRegistration", value);
    }

    /// <summary>
    /// Gets or sets LastDigestSend.
    /// </summary>
    public string LastDigestSend
    {
        get => this.RegistryBoard.GetValue<string>("LastDigestSend", null);

        set => this.RegistryBoard.SetValue("LastDigestSend", value);
    }

    /// <summary>
    /// Gets or sets LastIpListImport.
    /// </summary>
    public string LastIpListImport {
        get => this.RegistryBoard.GetValue<string>("LastIpListImport", null);

        set => this.RegistryBoard.SetValue("LastIpListImport", value);
    }

    /// <summary>
    /// Gets or sets the last search index updated.
    /// </summary>
    public string LastSearchIndexUpdated
    {
        get => this.RegistryBoard.GetValue<string>("LastSearchIndexUpdated", null);

        set => this.RegistryBoard.SetValue("LastSearchIndexUpdated", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ForceDigestSend.
    /// </summary>
    public bool ForceDigestSend
    {
        get => this.RegistryBoard.GetValue("ForceDigestSend", false);

        set => this.RegistryBoard.SetValue("ForceDigestSend", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether ForceDigestSend.
    /// </summary>
    public bool ForceUpdateSearchIndex
    {
        get => this.RegistryBoard.GetValue("ForceUpdateSearchIndex", false);

        set => this.RegistryBoard.SetValue("ForceUpdateSearchIndex", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether two column board layout.
    /// </summary>
    public bool TwoColumnBoardLayout
    {
        get => this.RegistryBoard.GetValue("TwoColumnBoardLayout", false);

        set => this.RegistryBoard.SetValue("TwoColumnBoardLayout", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether enable wysiwyg editor.
    /// </summary>
    public bool EnableWysiwygEditor {
        get => this.RegistryBoard.GetValue("EnableWysiwygEditor", false);

        set => this.RegistryBoard.SetValue("EnableWysiwygEditor", value);
    }

    /// <summary>
    /// Gets or sets the board announcement.
    /// </summary>
    public string BoardAnnouncement
    {
        get => this.RegistryBoard.GetValue("BoardAnnouncement", string.Empty);

        set => this.RegistryBoard.SetValue("BoardAnnouncement", value);
    }

    /// <summary>
    /// Gets or sets the board announcement until.
    /// </summary>
    public string BoardAnnouncementUntil
    {
        get =>
            this.RegistryBoard.GetValue(
                "BoardAnnouncementUntil",
                DateTime.MinValue.ToString(CultureInfo.InvariantCulture));

        set => this.RegistryBoard.SetValue("BoardAnnouncementUntil", value);
    }

    /// <summary>
    /// Gets or sets the board announcement type.
    /// </summary>
    public string BoardAnnouncementType
    {
        get => this.RegistryBoard.GetValue("BoardAnnouncementType", "info");

        set => this.RegistryBoard.SetValue("BoardAnnouncementType", value);
    }

    /// <summary>
    /// Gets or sets the default collapsible panel state.
    /// </summary>
    public CollapsiblePanelState DefaultCollapsiblePanelState
    {
        get => this.RegistryBoard.GetValue("DefaultCollapsiblePanelState", CollapsiblePanelState.Expanded);

        set => this.RegistryBoard.SetValue("DefaultCollapsiblePanelState", value);
    }

    /// <summary>
    /// Gets or sets the editor enter mode.
    /// </summary>
    /// <value>The editor enter mode.</value>
    public EnterMode EditorEnterMode {
        get => this.RegistryBoard.GetValue("EditorEnterMode", EnterMode.Br);

        set => this.RegistryBoard.SetValue("EditorEnterMode", value);
    }

    /// <summary>
    /// Gets or sets the RegistryDictionaryOverride.
    /// </summary>
    public RegistryDictionaryOverride Registry { get; set; }

    /// <summary>
    /// Gets or sets the RegistryDictionary.
    /// </summary>
    public RegistryDictionary RegistryBoard { get; set; }

    /// <summary>
    ///  Gets or sets the board id.
    /// </summary>
    public int BoardId { get; set; }
}