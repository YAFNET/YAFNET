/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Types.Constants;

/// <summary>
/// List of all pages available in the YAF forum
/// </summary>
public enum ForumPages
{
    /// <summary>
    /// The Forum page.
    /// </summary>
    Index,

    /// <summary>
    /// The digest page.
    /// </summary>
    Digest,

    /// <summary>
    /// The error page.
    /// </summary>
    Error,

    /// <summary>
    /// The topics.
    /// </summary>
    Topics,

    /// <summary>
    /// The Posts page.
    /// </summary>
    Posts,

    /// <summary>
    /// The post page.
    /// </summary>
    Post,

    /// <summary>
    /// The user profile page.
    /// </summary>
    UserProfile,

    /// <summary>
    /// The ActiveUsers.
    /// </summary>
    ActiveUsers,

    /// <summary>
    /// The site map
    /// </summary>
    SiteMap,

    /// <summary>
    /// The post topic page.
    /// </summary>
    PostTopic,

    /// <summary>
    /// The edit  message page.
    /// </summary>
    EditMessage,

    /// <summary>
    /// The post message page.
    /// </summary>
    PostMessage,

    /// <summary>
    /// The DeleteMessage page.
    /// </summary>
    DeleteMessage,

    /// <summary>
    /// The Move Message page.
    /// </summary>
    MoveMessage,

    /// <summary>
    /// The MessageHistory page displays a user message changes.
    /// </summary>
    MessageHistory,

    /// <summary>
    /// The EmailTopic page.
    /// </summary>
    EmailTopic,

    /// <summary>
    /// The PrintTopic page.
    /// </summary>
    PrintTopic,

    /// <summary>
    /// The Members page.
    /// </summary>
    Members,

    /// <summary>
    /// The page to report posts message.
    /// </summary>
    ReportPost,

    /// <summary>
    /// The Notification.
    /// </summary>
    Notification,

    /// <summary>
    /// The album.
    /// </summary>
    Album,

    /// <summary>
    /// The Albums.
    /// </summary>
    Albums,

    /// <summary>
    /// The EditAlbumImages.
    /// </summary>
    EditAlbumImages,

    /// <summary>
    /// The Info page.
    /// </summary>
    Info,

    /// <summary>
    /// The cookies page.
    /// </summary>
    Cookies,

    /// <summary>
    /// The Rules and Privacy page.
    /// </summary>
    Privacy,

    /// <summary>
    /// The Search page.
    /// </summary>
    Search,

    /// <summary>
    /// The moderate index page.
    /// </summary>
    Moderate_Moderate,

    /// <summary>
    /// The moderate forum page.
    /// </summary>
    Moderate_Forums,

    /// <summary>
    /// The moderate reported posts page.
    /// </summary>
    Moderate_ReportedPosts,

    /// <summary>
    /// The moderate unapproved posts page.
    /// </summary>
    Moderate_UnapprovedPosts,

    /// <summary>
    /// The Email page.
    /// </summary>
    Email,

    /// <summary>
    /// The PollEdit page.
    /// </summary>
    PollEdit,

    /// <summary>
    /// The help page.
    /// </summary>
    Help,

    /// <summary>
    /// The Team page.
    /// </summary>
    Team,

    /// <summary>
    /// The approve.
    /// </summary>
    Account_Approve,

    /// <summary>
    /// The Account Forgot Password page.
    /// </summary>
    Account_ForgotPassword,

    /// <summary>
    /// The Logout page.
    /// </summary>
    Account_Logout,

    /// <summary>
    /// The Account Reset Password page.
    /// </summary>
    Account_ResetPassword,

    /// <summary>
    /// The Register page.
    /// </summary>
    Account_Register,

    /// <summary>
    /// The Login page.
    /// </summary>
    Account_Login,

    /// <summary>
    /// The Login Authorize 2FA page.
    /// </summary>
    Account_Authorize,

    /// <summary>
    /// The Account Info Page.
    /// </summary>
    MyAccount,

    /// <summary>
    /// The Admin admin page.
    /// </summary>
    Admin_Admin,

    /// <summary>
    /// The Admin host settings page.
    /// </summary>
    Admin_HostSettings,

    /// <summary>
    /// The Admin boards.
    /// </summary>
    Admin_Boards,

    /// <summary>
    /// The Admin Announcement page.
    /// </summary>
    Admin_BoardAnnouncement,

    /// <summary>
    /// The Admin board settings page.
    /// </summary>
    Admin_Settings,

    /// <summary>
    /// The Admin forums page.
    /// </summary>
    Admin_Forums,

    /// <summary>
    /// The Admin banned countries page.
    /// </summary>
    Admin_BannedCountries,

    /// <summary>
    /// The Admin banned email page.
    /// </summary>
    Admin_BannedEmails,

    /// <summary>
    /// The Admin banned IP page.
    /// </summary>
    Admin_BannedIps,

    /// <summary>
    /// The Admin banned name page.
    /// </summary>
    Admin_BannedNames,

    /// <summary>
    /// The Admin banned user agents page.
    /// </summary>
    Admin_BannedUserAgents,

    /// <summary>
    /// The Admin access masks page.
    /// </summary>
    Admin_AccessMasks,

    /// <summary>
    /// The Admin groups page.
    /// </summary>
    Admin_Groups,

    /// <summary>
    /// The Admin users page.
    /// </summary>
    Admin_Users,

    /// <summary>
    /// The Admin ranks page.
    /// </summary>
    Admin_Ranks,

    /// <summary>
    /// The Admin mail page.
    /// </summary>
    Admin_Mail,

    /// <summary>
    /// The Admin medals page.
    /// </summary>
    Admin_Medals,

    /// <summary>
    /// The Admin page access list page.
    /// </summary>
    Admin_PageAccessList,

    /// <summary>
    /// The Admin page access edit page.
    /// </summary>
    Admin_PageAccessEdit,

    /// <summary>
    /// The admin profile definitions page.
    /// </summary>
    Admin_ProfileDefinitions,

    /// <summary>
    /// The Admin prune page.
    /// </summary>
    Admin_Prune,

    /// <summary>
    /// The Admin Restore page.
    /// </summary>
    Admin_Restore,

    /// <summary>
    /// The Admin pm page.
    /// </summary>
    Admin_Pm,

    /// <summary>
    /// The Admin event log page.
    /// </summary>
    Admin_EventLog,

    /// <summary>
    /// The Admin spam log page.
    /// </summary>
    Admin_SpamLog,

    /// <summary>
    /// The Admin version page.
    /// </summary>
    Admin_Version,

    /// <summary>
    /// The Admin edit access mask page.
    /// </summary>
    Admin_EditAccessMask,

    /// <summary>
    /// The Admin edit board page.
    /// </summary>
    Admin_EditBoard,

    /// <summary>
    /// The Admin edit category page.
    /// </summary>
    Admin_EditCategory,

    /// <summary>
    /// The Admin edit forum page.
    /// </summary>
    Admin_EditForum,

    /// <summary>
    /// The Admin delete forum.
    /// </summary>
    Admin_DeleteForum,

    /// <summary>
    /// The Admin edit group page.
    /// </summary>
    Admin_EditGroup,

    /// <summary>
    /// The Admin edit medal page.
    /// </summary>
    Admin_EditMedal,

    /// <summary>
    /// The Admin edit rank page.
    /// </summary>
    Admin_EditRank,

    /// <summary>
    /// The Admin edit user page.
    /// </summary>
    Admin_EditUser,

    /// <summary>
    /// The Admin register user page.
    /// </summary>
    Admin_RegisterUser,

    /// <summary>
    /// The Admin replace words page.
    /// </summary>
    Admin_ReplaceWords,

    /// <summary>
    /// The admin spam words page.
    /// </summary>
    Admin_SpamWords,

    /// <summary>
    /// The Admin BBCode page.
    /// </summary>
    Admin_BBCodes,

    /// <summary>
    /// The Admin BBCode edit page.
    /// </summary>
    Admin_EditBBCode,

    /// <summary>
    /// The Admin languages page.
    /// </summary>
    Admin_Languages,

    /// <summary>
    /// The Admin edit language page.
    /// </summary>
    Admin_EditLanguage,

    /// <summary>
    /// The Admin re index page.
    /// </summary>
    Admin_ReIndex,

    /// <summary>
    /// The Admin run SQL page.
    /// </summary>
    Admin_RunSql,

    /// <summary>
    /// The Admin task manager page.
    /// </summary>
    Admin_TaskManager,

    /// <summary>
    /// The admin_ test data.
    /// </summary>
    Admin_TestData,

    /// <summary>
    /// The Admin restart app page.
    /// </summary>
    Admin_RestartApp,

    /// <summary>
    /// Digest Admin page
    /// </summary>
    Admin_Digest,

    /// <summary>
    /// The admin attachments page
    /// </summary>
    Admin_Attachments,

    /// <summary>
    /// The admin tags page
    /// </summary>
    Admin_Tags,

    /// <summary>
    /// The host information page.
    /// </summary>
    Host_ServerInfo,

    /// <summary>
    /// The header setup page.
    /// </summary>
    Host_Setup,

    /// <summary>
    /// The host features page.
    /// </summary>
    Host_Features,

    /// <summary>
    /// The host display page.
    /// </summary>
    Host_Display,

    /// <summary>
    /// The host adverts page.
    /// </summary>
    Host_Adverts,

    /// <summary>
    /// The host permission page.
    /// </summary>
    Host_Permission,

    /// <summary>
    /// The host avatars page.
    /// </summary>
    Host_Avatars,

    /// <summary>
    /// The host cache page.
    /// </summary>
    Host_Cache,

    /// <summary>
    /// The host search page.
    /// </summary>
    Host_Search,

    /// <summary>
    /// The host log page.
    /// </summary>
    Host_Log,

    /// <summary>
    /// The attachments.
    /// </summary>
    Profile_Attachments,

    /// <summary>
    /// The BlockOptions.
    /// </summary>
    Profile_BlockOptions,

    /// <summary>
    /// The Change Password page.
    /// </summary>
    Profile_ChangePassword,

    /// <summary>
    /// The Delete Account page.
    /// </summary>
    Profile_DeleteAccount,

    /// <summary>
    /// The EditAvatar.
    /// </summary>
    Profile_EditAvatar,

    /// <summary>
    /// The EditProfile.
    /// </summary>
    Profile_EditProfile,

    /// <summary>
    /// The EditSettings.
    /// </summary>
    Profile_EditSettings,

    /// <summary>
    /// The EditSignature.
    /// </summary>
    Profile_EditSignature,

    /// <summary>
    /// The Friends page.
    /// </summary>
    MyFriends,

    /// <summary>
    /// The MyMessages Page.
    /// </summary>
    MyMessages,

    /// <summary>
    /// The My Topics page
    /// </summary>
    MyTopics,

    /// <summary>
    /// The Subscriptions.
    /// </summary>
    Profile_Subscriptions
}