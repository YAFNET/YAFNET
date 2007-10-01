/*
** Drop old Foreign keys
*/

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Active_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] drop constraint [FK_Active_Forum]
=======
if exists(select 1 from sysobjects where name='FK_Active_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] drop constraint FK_Active_Forum
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Active_Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] drop constraint [FK_Active_Topic]
=======
if exists(select 1 from sysobjects where name='FK_Active_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] drop constraint FK_Active_Topic
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Active_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] drop constraint [FK_Active_User]
=======
if exists(select 1 from sysobjects where name='FK_Active_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] drop constraint FK_Active_User
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_CheckEmail_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] drop constraint [FK_CheckEmail_User]
=======
if exists(select 1 from sysobjects where name='FK_CheckEmail_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_CheckEmail] drop constraint FK_CheckEmail_User
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Choice_Poll' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Choice]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Choice] drop constraint [FK_Choice_Poll]
=======
if exists(select 1 from sysobjects where name='FK_Choice_Poll' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Choice]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Choice] drop constraint FK_Choice_Poll
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Forum_Category' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint [FK_Forum_Category]
=======
if exists(select 1 from sysobjects where name='FK_Forum_Category' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] drop constraint FK_Forum_Category
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Forum_Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint [FK_Forum_Message]
=======
if exists(select 1 from sysobjects where name='FK_Forum_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] drop constraint FK_Forum_Message
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Forum_Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint [FK_Forum_Topic]
=======
if exists(select 1 from sysobjects where name='FK_Forum_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] drop constraint FK_Forum_Topic
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Forum_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint [FK_Forum_User]
=======
if exists(select 1 from sysobjects where name='FK_Forum_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] drop constraint FK_Forum_User
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_ForumAccess_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] drop constraint [FK_ForumAccess_Forum]
=======
if exists(select 1 from sysobjects where name='FK_ForumAccess_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_ForumAccess] drop constraint FK_ForumAccess_Forum
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_ForumAccess_Group' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] drop constraint [FK_ForumAccess_Group]
=======
if exists(select 1 from sysobjects where name='FK_ForumAccess_Group' and parent_obj=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_ForumAccess] drop constraint FK_ForumAccess_Group
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Message_Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Message] drop constraint [FK_Message_Topic]
=======
if exists(select 1 from sysobjects where name='FK_Message_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Message] drop constraint FK_Message_Topic
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Message_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Message] drop constraint [FK_Message_User]
=======
if exists(select 1 from sysobjects where name='FK_Message_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Message] drop constraint FK_Message_User
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_PMessage_User1' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}PMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}PMessage] drop constraint [FK_PMessage_User1]
=======
if exists(select 1 from sysobjects where name='FK_PMessage_User1' and parent_obj=object_id(N'[{databaseOwner}].[yaf_PMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_PMessage] drop constraint FK_PMessage_User1
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Topic_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_Topic_Forum]
=======
if exists(select 1 from sysobjects where name='FK_Topic_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] drop constraint FK_Topic_Forum
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Topic_Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_Topic_Message]
=======
if exists(select 1 from sysobjects where name='FK_Topic_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] drop constraint FK_Topic_Message
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Topic_Poll' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_Topic_Poll]
=======
if exists(select 1 from sysobjects where name='FK_Topic_Poll' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] drop constraint FK_Topic_Poll
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Topic_Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_Topic_Topic]
=======
if exists(select 1 from sysobjects where name='FK_Topic_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] drop constraint FK_Topic_Topic
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Topic_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_Topic_User]
=======
if exists(select 1 from sysobjects where name='FK_Topic_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] drop constraint FK_Topic_User
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Topic_User2' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_Topic_User2]
=======
if exists(select 1 from sysobjects where name='FK_Topic_User2' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] drop constraint FK_Topic_User2
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_WatchForum_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] drop constraint [FK_WatchForum_Forum]
=======
if exists(select 1 from sysobjects where name='FK_WatchForum_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchForum] drop constraint FK_WatchForum_Forum
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_WatchForum_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] drop constraint [FK_WatchForum_User]
=======
if exists(select 1 from sysobjects where name='FK_WatchForum_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchForum] drop constraint FK_WatchForum_User
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_WatchTopic_Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] drop constraint [FK_WatchTopic_Topic]
=======
if exists(select 1 from sysobjects where name='FK_WatchTopic_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchTopic] drop constraint FK_WatchTopic_Topic
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_WatchTopic_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] drop constraint [FK_WatchTopic_User]
=======
if exists(select 1 from sysobjects where name='FK_WatchTopic_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchTopic] drop constraint FK_WatchTopic_User
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Active_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Attachment] drop constraint [FK_Attachment_Message]
=======
if exists(select 1 from sysobjects where name='FK_Active_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Attachment] drop constraint FK_Attachment_Message
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_UserGroup_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserGroup] drop constraint [FK_UserGroup_User]
=======
if exists(select 1 from sysobjects where name='FK_UserGroup_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserGroup] drop constraint FK_UserGroup_User
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_UserGroup_Group' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserGroup] drop constraint [FK_UserGroup_Group]
=======
if exists(select 1 from sysobjects where name='FK_UserGroup_Group' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserGroup] drop constraint FK_UserGroup_Group
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Attachment_Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Attachment]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Attachment] drop constraint [FK_Attachment_Message]
=======
if exists(select 1 from sysobjects where name='FK_Attachment_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Attachment]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Attachment] drop constraint FK_Attachment_Message
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_NntpForum_NntpServer' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpForum] drop constraint [FK_NntpForum_NntpServer]
=======
if exists(select 1 from sysobjects where name='FK_NntpForum_NntpServer' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpForum] drop constraint FK_NntpForum_NntpServer
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_NntpForum_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpForum] drop constraint [FK_NntpForum_Forum]
=======
if exists(select 1 from sysobjects where name='FK_NntpForum_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpForum] drop constraint FK_NntpForum_Forum
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_NntpTopic_NntpForum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpTopic] drop constraint [FK_NntpTopic_NntpForum]
=======
if exists(select 1 from sysobjects where name='FK_NntpTopic_NntpForum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpTopic] drop constraint FK_NntpTopic_NntpForum
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_NntpTopic_Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpTopic] drop constraint [FK_NntpTopic_Topic]
=======
if exists(select 1 from sysobjects where name='FK_NntpTopic_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpTopic] drop constraint FK_NntpTopic_Topic
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_ForumAccess_AccessMask' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] drop constraint [FK_ForumAccess_AccessMask]
=======
if exists(select 1 from sysobjects where name='FK_ForumAccess_AccessMask' and parent_obj=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_ForumAccess] drop constraint FK_ForumAccess_AccessMask
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_UserForum_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserForum] drop constraint [FK_UserForum_User]
=======
if exists(select 1 from sysobjects where name='FK_UserForum_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserForum] drop constraint FK_UserForum_User
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_UserForum_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserForum] drop constraint [FK_UserForum_Forum]
=======
if exists(select 1 from sysobjects where name='FK_UserForum_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserForum] drop constraint FK_UserForum_Forum
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_UserForum_AccessMask' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserForum] drop constraint [FK_UserForum_AccessMask]
=======
if exists(select 1 from sysobjects where name='FK_UserForum_AccessMask' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserForum] drop constraint FK_UserForum_AccessMask
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Category_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Category]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Category] drop constraint [FK_Category_Board]
=======
if exists(select 1 from sysobjects where name='FK_Category_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Category]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Category] drop constraint FK_Category_Board
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_AccessMask_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop constraint [FK_AccessMask_Board]
=======
if exists(select 1 from sysobjects where name='FK_AccessMask_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_AccessMask] drop constraint FK_AccessMask_Board
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Active_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] drop constraint [FK_Active_Board]
=======
if exists(select 1 from sysobjects where name='FK_Active_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] drop constraint FK_Active_Board
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_BannedIP_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] drop constraint [FK_BannedIP_Board]
=======
if exists(select 1 from sysobjects where name='FK_BannedIP_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_BannedIP] drop constraint FK_BannedIP_Board
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Group_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Group]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Group] drop constraint [FK_Group_Board]
=======
if exists(select 1 from sysobjects where name='FK_Group_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Group]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Group] drop constraint FK_Group_Board
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_NntpServer_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpServer]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpServer] drop constraint [FK_NntpServer_Board]
=======
if exists(select 1 from sysobjects where name='FK_NntpServer_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpServer]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpServer] drop constraint FK_NntpServer_Board
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Rank_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Rank] drop constraint [FK_Rank_Board]
=======
if exists(select 1 from sysobjects where name='FK_Rank_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Rank]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Rank] drop constraint FK_Rank_Board
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Smiley_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Smiley] drop constraint [FK_Smiley_Board]
=======
if exists(select 1 from sysobjects where name='FK_Smiley_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Smiley]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Smiley] drop constraint FK_Smiley_Board
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_User_Rank' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}User] drop constraint [FK_User_Rank]
=======
if exists(select 1 from sysobjects where name='FK_User_Rank' and parent_obj=object_id(N'[{databaseOwner}].[yaf_User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_User] drop constraint FK_User_Rank
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_User_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}User] drop constraint [FK_User_Board]
=======
if exists(select 1 from sysobjects where name='FK_User_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_User] drop constraint FK_User_Board
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Forum_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint [FK_Forum_Forum]
=======
if exists(select 1 from sysobjects where name='FK_Forum_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] drop constraint FK_Forum_Forum
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Message_Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Message] drop constraint [FK_Message_Message]
=======
if exists(select 1 from sysobjects where name='FK_Message_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Message] drop constraint FK_Message_Message
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_UserPMessage_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserPMessage] drop constraint [FK_UserPMessage_User]
=======
if exists(select 1 from sysobjects where name='FK_UserPMessage_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserPMessage] drop constraint FK_UserPMessage_User
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_UserPMessage_PMessage' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserPMessage] drop constraint [FK_UserPMessage_PMessage]
=======
if exists(select 1 from sysobjects where name='FK_UserPMessage_PMessage' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserPMessage] drop constraint FK_UserPMessage_PMessage
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_Registry_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Registry]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Registry] drop constraint [FK_Registry_Board]
=======
if exists(select 1 from sysobjects where name='FK_Registry_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Registry]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Registry] drop constraint FK_Registry_Board
>>>>>>> .r1490
go

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_EventLog_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}EventLog]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}EventLog] drop constraint [FK_EventLog_User]
=======
if exists(select 1 from sysobjects where name='FK_EventLog_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_EventLog]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_EventLog] drop constraint FK_EventLog_User
>>>>>>> .r1490
go

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}PollVote_{objectQualifier}Poll' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}PollVote]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}PollVote] drop constraint [FK_{objectQualifier}PollVote_{objectQualifier}Poll]
=======
if exists(select 1 from sysobjects where name='FK_yaf_PollVote_yaf_Poll' and parent_obj=object_id(N'[{databaseOwner}].[yaf_PollVote]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_PollVote] drop constraint FK_yaf_PollVote_yaf_Poll
>>>>>>> .r1490
go

/* Drop old] primary keys */

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and name='PK_BannedIP')
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] drop constraint [PK_BannedIP]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and name='PK_BannedIP')
	alter table [{databaseOwner}].[yaf_BannedIP] drop constraint PK_BannedIP
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Category]') and name='PK_Category')
	alter table [{databaseOwner}].[{objectQualifier}Category] drop constraint [PK_Category]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Category]') and name='PK_Category')
	alter table [{databaseOwner}].[yaf_Category] drop constraint PK_Category
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and name='PK_CheckEmail')
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] drop constraint [PK_CheckEmail]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and name='PK_CheckEmail')
	alter table [{databaseOwner}].[yaf_CheckEmail] drop constraint PK_CheckEmail
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Choice]') and name='PK_Choice')
	alter table [{databaseOwner}].[{objectQualifier}Choice] drop constraint [PK_Choice]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Choice]') and name='PK_Choice')
	alter table [{databaseOwner}].[yaf_Choice] drop constraint PK_Choice
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='PK_Forum')
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint [PK_Forum]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='PK_Forum')
	alter table [{databaseOwner}].[yaf_Forum] drop constraint PK_Forum
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and name='PK_ForumAccess')
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] drop constraint [PK_ForumAccess]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and name='PK_ForumAccess')
	alter table [{databaseOwner}].[yaf_ForumAccess] drop constraint PK_ForumAccess
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='PK_Group')
	alter table [{databaseOwner}].[{objectQualifier}Group] drop constraint [PK_Group]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='PK_Group')
	alter table [{databaseOwner}].[yaf_Group] drop constraint PK_Group
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Mail]') and name='PK_Mail')
	alter table [{databaseOwner}].[{objectQualifier}Mail] drop constraint [PK_Mail]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Mail]') and name='PK_Mail')
	alter table [{databaseOwner}].[yaf_Mail] drop constraint PK_Mail
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='PK_Message')
	alter table [{databaseOwner}].[{objectQualifier}Message] drop constraint [PK_Message]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Message]') and name='PK_Message')
	alter table [{databaseOwner}].[yaf_Message] drop constraint PK_Message
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}PMessage]') and name='PK_PMessage')
	alter table [{databaseOwner}].[{objectQualifier}PMessage] drop constraint [PK_PMessage]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_PMessage]') and name='PK_PMessage')
	alter table [{databaseOwner}].[yaf_PMessage] drop constraint PK_PMessage
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name='PK_Poll')
	alter table [{databaseOwner}].[{objectQualifier}Poll] drop constraint [PK_Poll]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Poll]') and name='PK_Poll')
	alter table [{databaseOwner}].[yaf_Poll] drop constraint PK_Poll
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and name='PK_Smiley')
	alter table [{databaseOwner}].[{objectQualifier}Smiley] drop constraint [PK_Smiley]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Smiley]') and name='PK_Smiley')
	alter table [{databaseOwner}].[yaf_Smiley] drop constraint PK_Smiley
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='PK_Topic')
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [PK_Topic]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Topic]') and name='PK_Topic')
	alter table [{databaseOwner}].[yaf_Topic] drop constraint PK_Topic
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='PK_User')
	alter table [{databaseOwner}].[{objectQualifier}User] drop constraint [PK_User]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='PK_User')
	alter table [{databaseOwner}].[yaf_User] drop constraint PK_User
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and name='PK_WatchForum')
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] drop constraint [PK_WatchForum]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and name='PK_WatchForum')
	alter table [{databaseOwner}].[yaf_WatchForum] drop constraint PK_WatchForum
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and name='PK_WatchTopic')
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] drop constraint [PK_WatchTopic]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and name='PK_WatchTopic')
	alter table [{databaseOwner}].[yaf_WatchTopic] drop constraint PK_WatchTopic
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}UserGroup]') and name='PK_UserGroup')
	alter table [{databaseOwner}].[{objectQualifier}UserGroup] drop constraint [PK_UserGroup]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_UserGroup]') and name='PK_UserGroup')
	alter table [{databaseOwner}].[yaf_UserGroup] drop constraint PK_UserGroup
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='PK_Rank')
	alter table [{databaseOwner}].[{objectQualifier}Rank] drop constraint [PK_Rank]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Rank]') and name='PK_Rank')
	alter table [{databaseOwner}].[yaf_Rank] drop constraint PK_Rank
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}NntpServer]') and name='PK_NntpServer')
	alter table [{databaseOwner}].[{objectQualifier}NntpServer] drop constraint [PK_NntpServer]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_NntpServer]') and name='PK_NntpServer')
	alter table [{databaseOwner}].[yaf_NntpServer] drop constraint PK_NntpServer
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}NntpForum]') and name='PK_NntpForum')
	alter table [{databaseOwner}].[{objectQualifier}NntpForum] drop constraint [PK_NntpForum]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_NntpForum]') and name='PK_NntpForum')
	alter table [{databaseOwner}].[yaf_NntpForum] drop constraint PK_NntpForum
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}NntpTopic]') and name='PK_NntpTopic')
	alter table [{databaseOwner}].[{objectQualifier}NntpTopic] drop constraint [PK_NntpTopic]
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_NntpTopic]') and name='PK_NntpTopic')
	alter table [{databaseOwner}].[yaf_NntpTopic] drop constraint PK_NntpTopic
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='PK_AccessMask')
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop constraint [PK_AccessMask]
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and name='PK_AccessMask')
	alter table [{databaseOwner}].[yaf_AccessMask] drop constraint PK_AccessMask
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and name='PK_UserForum')
	alter table [{databaseOwner}].[{objectQualifier}UserForum] drop constraint [PK_UserForum]
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_UserForum]') and name='PK_UserForum')
	alter table [{databaseOwner}].[yaf_UserForum] drop constraint PK_UserForum
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Board]') and name='PK_Board')
	alter table [{databaseOwner}].[{objectQualifier}Board] drop constraint [PK_Board]
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Board]') and name='PK_Board')
	alter table [{databaseOwner}].[yaf_Board] drop constraint PK_Board
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Active]') and name='PK_Active')
	alter table [{databaseOwner}].[{objectQualifier}Active] drop constraint [PK_Active]
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Active]') and name='PK_Active')
	alter table [{databaseOwner}].[yaf_Active] drop constraint PK_Active
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and name='PK_UserPMessage')
	alter table [{databaseOwner}].[{objectQualifier}UserPMessage] drop constraint [PK_UserPMessage]
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_UserPMessage]') and name='PK_UserPMessage')
	alter table [{databaseOwner}].[yaf_UserPMessage] drop constraint PK_UserPMessage
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Attachment]') and name='PK_Attachment')
	alter table [{databaseOwner}].[{objectQualifier}Attachment] drop constraint [PK_Attachment]
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Attachment]') and name='PK_Attachment')
	alter table [{databaseOwner}].[yaf_Attachment] drop constraint PK_Attachment
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Active]') and name='PK_Active')
	alter table [{databaseOwner}].[{objectQualifier}Active] drop constraint [PK_Active]
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Active]') and name='PK_Active')
	alter table [{databaseOwner}].[yaf_Active] drop constraint PK_Active
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}PollVote]') and name='PK_PollVote')
	alter table [{databaseOwner}].[{objectQualifier}PollVote] drop constraint [PK_PollVote]
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_PollVote]') and name='PK_PollVote')
	alter table [{databaseOwner}].[yaf_PollVote] drop constraint PK_PollVote
>>>>>>> .r1490
GO

/*
** Unique constraints
*/

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and name='IX_CheckEmail')
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] drop constraint IX_CheckEmail
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and name='IX_CheckEmail')
	alter table [{databaseOwner}].[yaf_CheckEmail] drop constraint IX_CheckEmail
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IX_Forum')
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint IX_Forum
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='IX_Forum')
	alter table [{databaseOwner}].[yaf_Forum] drop constraint IX_Forum
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and name='IX_WatchForum')
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] drop constraint IX_WatchForum
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and name='IX_WatchForum')
	alter table [{databaseOwner}].[yaf_WatchForum] drop constraint IX_WatchForum
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and name='IX_WatchTopic')
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] drop constraint IX_WatchTopic
=======
if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and name='IX_WatchTopic')
	alter table [{databaseOwner}].[yaf_WatchTopic] drop constraint IX_WatchTopic
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Category]') and name='IX_Category')
	alter table [{databaseOwner}].[{objectQualifier}Category] drop constraint IX_Category
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Category]') and name='IX_Category')
	alter table [{databaseOwner}].[yaf_Category] drop constraint IX_Category
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='IX_Rank')
	alter table [{databaseOwner}].[{objectQualifier}Rank] drop constraint IX_Rank
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Rank]') and name='IX_Rank')
	alter table [{databaseOwner}].[yaf_Rank] drop constraint IX_Rank
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IX_User')
	alter table [{databaseOwner}].[{objectQualifier}User] drop constraint IX_User
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='IX_User')
	alter table [{databaseOwner}].[yaf_User] drop constraint IX_User
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IX_Group')
	alter table [{databaseOwner}].[{objectQualifier}Group] drop constraint IX_Group
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='IX_Group')
	alter table [{databaseOwner}].[yaf_Group] drop constraint IX_Group
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and name='IX_BannedIP')
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] drop constraint IX_BannedIP
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and name='IX_BannedIP')
	alter table [{databaseOwner}].[yaf_BannedIP] drop constraint IX_BannedIP
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and name='IX_Smiley')
	alter table [{databaseOwner}].[{objectQualifier}Smiley] drop constraint IX_Smiley
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Smiley]') and name='IX_Smiley')
	alter table [{databaseOwner}].[yaf_Smiley] drop constraint IX_Smiley
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and name='IX_BannedIP')
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] drop constraint IX_BannedIP
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and name='IX_BannedIP')
	alter table [{databaseOwner}].[yaf_BannedIP] drop constraint IX_BannedIP
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Category]') and name='IX_Category')
	alter table [{databaseOwner}].[{objectQualifier}Category] drop constraint IX_Category
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Category]') and name='IX_Category')
	alter table [{databaseOwner}].[yaf_Category] drop constraint IX_Category
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and name='IX_CheckEmail')
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] drop constraint IX_CheckEmail
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and name='IX_CheckEmail')
	alter table [{databaseOwner}].[yaf_CheckEmail] drop constraint IX_CheckEmail
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IX_Forum')
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint IX_Forum
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='IX_Forum')
	alter table [{databaseOwner}].[yaf_Forum] drop constraint IX_Forum
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IX_Group')
	alter table [{databaseOwner}].[{objectQualifier}Group] drop constraint IX_Group
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='IX_Group')
	alter table [{databaseOwner}].[yaf_Group] drop constraint IX_Group
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='IX_Rank')
	alter table [{databaseOwner}].[{objectQualifier}Rank] drop constraint IX_Rank
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Rank]') and name='IX_Rank')
	alter table [{databaseOwner}].[yaf_Rank] drop constraint IX_Rank
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and name='IX_Smiley')
	alter table [{databaseOwner}].[{objectQualifier}Smiley] drop constraint IX_Smiley
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Smiley]') and name='IX_Smiley')
	alter table [{databaseOwner}].[yaf_Smiley] drop constraint IX_Smiley
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IX_User')
	alter table [{databaseOwner}].[{objectQualifier}User] drop constraint IX_User
=======
if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='IX_User')
	alter table [{databaseOwner}].[yaf_User] drop constraint IX_User
>>>>>>> .r1490
GO

/* Build new constraints */

/*
** Primary keys
*/

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and name='PK_{objectQualifier}BannedIP')
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] with nocheck add constraint [PK_{objectQualifier}BannedIP] primary key clustered(ID)
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and name='PK_yaf_BannedIP')
	alter table [{databaseOwner}].[yaf_BannedIP] with nocheck add constraint PK_yaf_BannedIP primary key clustered(ID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Category]') and name='PK_{objectQualifier}Category')
	alter table [{databaseOwner}].[{objectQualifier}Category] with nocheck add constraint [PK_{objectQualifier}Category] primary key clustered(CategoryID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Category]') and name='PK_yaf_Category')
	alter table [{databaseOwner}].[yaf_Category] with nocheck add constraint PK_yaf_Category primary key clustered(CategoryID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and name='PK_{objectQualifier}CheckEmail')
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] with nocheck add constraint [PK_{objectQualifier}CheckEmail] primary key clustered(CheckEmailID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and name='PK_yaf_CheckEmail')
	alter table [{databaseOwner}].[yaf_CheckEmail] with nocheck add constraint PK_yaf_CheckEmail primary key clustered(CheckEmailID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Choice]') and name='PK_{objectQualifier}Choice')
	alter table [{databaseOwner}].[{objectQualifier}Choice] with nocheck add constraint [PK_{objectQualifier}Choice] primary key clustered(ChoiceID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Choice]') and name='PK_yaf_Choice')
	alter table [{databaseOwner}].[yaf_Choice] with nocheck add constraint PK_yaf_Choice primary key clustered(ChoiceID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='PK_{objectQualifier}Forum')
	alter table [{databaseOwner}].[{objectQualifier}Forum] with nocheck add constraint [PK_{objectQualifier}Forum] primary key clustered(ForumID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='PK_yaf_Forum')
	alter table [{databaseOwner}].[yaf_Forum] with nocheck add constraint PK_yaf_Forum primary key clustered(ForumID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and name='PK_{objectQualifier}ForumAccess')
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] with nocheck add constraint [PK_{objectQualifier}ForumAccess] primary key clustered(GroupID,ForumID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and name='PK_yaf_ForumAccess')
	alter table [{databaseOwner}].[yaf_ForumAccess] with nocheck add constraint PK_yaf_ForumAccess primary key clustered(GroupID,ForumID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='PK_{objectQualifier}Group')
	alter table [{databaseOwner}].[{objectQualifier}Group] with nocheck add constraint [PK_{objectQualifier}Group] primary key clustered(GroupID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='PK_yaf_Group')
	alter table [{databaseOwner}].[yaf_Group] with nocheck add constraint PK_yaf_Group primary key clustered(GroupID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Mail]') and name='PK_{objectQualifier}Mail')
	alter table [{databaseOwner}].[{objectQualifier}Mail] with nocheck add constraint [PK_{objectQualifier}Mail] primary key clustered(MailID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Mail]') and name='PK_yaf_Mail')
	alter table [{databaseOwner}].[yaf_Mail] with nocheck add constraint PK_yaf_Mail primary key clustered(MailID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='PK_{objectQualifier}Message')
	alter table [{databaseOwner}].[{objectQualifier}Message] with nocheck add constraint [PK_{objectQualifier}Message] primary key clustered(MessageID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Message]') and name='PK_yaf_Message')
	alter table [{databaseOwner}].[yaf_Message] with nocheck add constraint PK_yaf_Message primary key clustered(MessageID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}PMessage]') and name='PK_{objectQualifier}PMessage')
	alter table [{databaseOwner}].[{objectQualifier}PMessage] with nocheck add constraint [PK_{objectQualifier}PMessage] primary key clustered(PMessageID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_PMessage]') and name='PK_yaf_PMessage')
	alter table [{databaseOwner}].[yaf_PMessage] with nocheck add constraint PK_yaf_PMessage primary key clustered(PMessageID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name='PK_{objectQualifier}Poll')
	alter table [{databaseOwner}].[{objectQualifier}Poll] with nocheck add constraint [PK_{objectQualifier}Poll] primary key clustered(PollID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Poll]') and name='PK_yaf_Poll')
	alter table [{databaseOwner}].[yaf_Poll] with nocheck add constraint PK_yaf_Poll primary key clustered(PollID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and name='PK_{objectQualifier}Smiley')
	alter table [{databaseOwner}].[{objectQualifier}Smiley] with nocheck add constraint [PK_{objectQualifier}Smiley] primary key clustered(SmileyID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Smiley]') and name='PK_yaf_Smiley')
	alter table [{databaseOwner}].[yaf_Smiley] with nocheck add constraint PK_yaf_Smiley primary key clustered(SmileyID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='PK_{objectQualifier}Topic')
	alter table [{databaseOwner}].[{objectQualifier}Topic] with nocheck add constraint [PK_{objectQualifier}Topic] primary key clustered(TopicID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Topic]') and name='PK_yaf_Topic')
	alter table [{databaseOwner}].[yaf_Topic] with nocheck add constraint PK_yaf_Topic primary key clustered(TopicID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='PK_{objectQualifier}{objectQualifier}User')
	alter table [{databaseOwner}].[{objectQualifier}User] with nocheck add constraint [PK_{objectQualifier}{objectQualifier}User] primary key clustered(UserID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='PK_yaf_yaf_User')
	alter table [{databaseOwner}].[yaf_User] with nocheck add constraint PK_yaf_yaf_User primary key clustered(UserID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and name='PK_{objectQualifier}WatchForum')
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] with nocheck add constraint [PK_{objectQualifier}WatchForum] primary key clustered(WatchForumID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and name='PK_yaf_WatchForum')
	alter table [{databaseOwner}].[yaf_WatchForum] with nocheck add constraint PK_yaf_WatchForum primary key clustered(WatchForumID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and name='PK_{objectQualifier}WatchTopic')
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] with nocheck add constraint [PK_{objectQualifier}WatchTopic] primary key clustered(WatchTopicID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and name='PK_yaf_WatchTopic')
	alter table [{databaseOwner}].[yaf_WatchTopic] with nocheck add constraint PK_yaf_WatchTopic primary key clustered(WatchTopicID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}UserGroup]') and name='PK_{objectQualifier}UserGroup')
	alter table [{databaseOwner}].[{objectQualifier}UserGroup] with nocheck add constraint [PK_{objectQualifier}UserGroup] primary key clustered(UserID,GroupID)
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_UserGroup]') and name='PK_yaf_UserGroup')
	alter table [{databaseOwner}].[yaf_UserGroup] with nocheck add constraint PK_yaf_UserGroup primary key clustered(UserID,GroupID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='PK_{objectQualifier}Rank')
	alter table [{databaseOwner}].[{objectQualifier}Rank] with nocheck add constraint [PK_{objectQualifier}Rank] primary key clustered(RankID)
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Rank]') and name='PK_yaf_Rank')
	alter table [{databaseOwner}].[yaf_Rank] with nocheck add constraint PK_yaf_Rank primary key clustered(RankID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}NntpServer]') and name='PK_{objectQualifier}NntpServer')
	alter table [{databaseOwner}].[{objectQualifier}NntpServer] with nocheck add constraint [PK_{objectQualifier}NntpServer] primary key clustered (NntpServerID) 
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_NntpServer]') and name='PK_yaf_NntpServer')
	alter table [{databaseOwner}].[yaf_NntpServer] with nocheck add constraint PK_yaf_NntpServer primary key clustered (NntpServerID) 
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}NntpForum]') and name='PK_{objectQualifier}NntpForum')
	alter table [{databaseOwner}].[{objectQualifier}NntpForum] with nocheck add constraint [PK_{objectQualifier}NntpForum] primary key clustered (NntpForumID) 
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_NntpForum]') and name='PK_yaf_NntpForum')
	alter table [{databaseOwner}].[yaf_NntpForum] with nocheck add constraint PK_yaf_NntpForum primary key clustered (NntpForumID) 
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}NntpTopic]') and name='PK_{objectQualifier}NntpTopic')
	alter table [{databaseOwner}].[{objectQualifier}NntpTopic] with nocheck add constraint [PK_{objectQualifier}NntpTopic] primary key clustered (NntpTopicID) 
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_NntpTopic]') and name='PK_yaf_NntpTopic')
	alter table [{databaseOwner}].[yaf_NntpTopic] with nocheck add constraint PK_yaf_NntpTopic primary key clustered (NntpTopicID) 
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='PK_{objectQualifier}AccessMask')
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] with nocheck add constraint [PK_{objectQualifier}AccessMask] primary key clustered (AccessMaskID) 
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and name='PK_yaf_AccessMask')
	alter table [{databaseOwner}].[yaf_AccessMask] with nocheck add constraint PK_yaf_AccessMask primary key clustered (AccessMaskID) 
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and name='PK_{objectQualifier}UserForum')
	alter table [{databaseOwner}].[{objectQualifier}UserForum] with nocheck add constraint [PK_{objectQualifier}UserForum] primary key clustered (UserID,ForumID) 
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_UserForum]') and name='PK_yaf_UserForum')
	alter table [{databaseOwner}].[yaf_UserForum] with nocheck add constraint PK_yaf_UserForum primary key clustered (UserID,ForumID) 
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Board]') and name='PK_{objectQualifier}Board')
	alter table [{databaseOwner}].[{objectQualifier}Board] with nocheck add constraint [PK_{objectQualifier}Board] primary key clustered (BoardID)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Board]') and name='PK_yaf_Board')
	alter table [{databaseOwner}].[yaf_Board] with nocheck add constraint PK_yaf_Board primary key clustered (BoardID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Active]') and name='PK_{objectQualifier}Active')
	alter table [{databaseOwner}].[{objectQualifier}Active]	 [PK_{objectQualifier}Active] primary key clustered(SessionID,BoardID)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Active]') and name='PK_yaf_Active')
	alter table [{databaseOwner}].[yaf_Active] with nocheck add constraint PK_yaf_Active primary key clustered(SessionID,BoardID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and name='PK_{objectQualifier}UserPMessage')
	alter table [{databaseOwner}].[{objectQualifier}UserPMessage] with nocheck add constraint [PK_{objectQualifier}UserPMessage] primary key clustered (UserPMessageID) 
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_UserPMessage]') and name='PK_yaf_UserPMessage')
	alter table [{databaseOwner}].[yaf_UserPMessage] with nocheck add constraint PK_yaf_UserPMessage primary key clustered (UserPMessageID) 
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Attachment]') and name='PK_{objectQualifier}Attachment')
	alter table [{databaseOwner}].[{objectQualifier}Attachment] with nocheck add constraint [PK_{objectQualifier}Attachment] primary key clustered (AttachmentID) 
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Attachment]') and name='PK_yaf_Attachment')
	alter table [{databaseOwner}].[yaf_Attachment] with nocheck add constraint PK_yaf_Attachment primary key clustered (AttachmentID) 
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Active]') and name='PK_{objectQualifier}Active')
	alter table [{databaseOwner}].[{objectQualifier}Active] with nocheck add constraint [PK_{objectQualifier}Active] primary key clustered(SessionID,BoardID)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Active]') and name='PK_yaf_Active')
	alter table [{databaseOwner}].[yaf_Active] with nocheck add constraint PK_yaf_Active primary key clustered(SessionID,BoardID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}PollVote]') and name='PK_{objectQualifier}PollVote')
	alter table [{databaseOwner}].[{objectQualifier}PollVote] with nocheck add constraint [PK_{objectQualifier}PollVote] primary key clustered(PollVoteID)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_PollVote]') and name='PK_yaf_PollVote')
	alter table [{databaseOwner}].[yaf_PollVote] with nocheck add constraint PK_yaf_PollVote primary key clustered(PollVoteID)
>>>>>>> .r1490
GO

/*
** Unique constraints
*/

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and name='IX_{objectQualifier}CheckEmail')
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] add constraint IX_{objectQualifier}CheckEmail unique nonclustered (Hash)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and name='IX_yaf_CheckEmail')
	alter table [{databaseOwner}].[yaf_CheckEmail] add constraint IX_yaf_CheckEmail unique nonclustered (Hash)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IX_{objectQualifier}Forum')
	alter table [{databaseOwner}].[{objectQualifier}Forum] add constraint IX_{objectQualifier}Forum unique nonclustered (CategoryID,Name)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='IX_yaf_Forum')
	alter table [{databaseOwner}].[yaf_Forum] add constraint IX_yaf_Forum unique nonclustered (CategoryID,Name)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and name='IX_{objectQualifier}WatchForum')
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] add constraint IX_{objectQualifier}WatchForum unique nonclustered (ForumID,UserID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and name='IX_yaf_WatchForum')
	alter table [{databaseOwner}].[yaf_WatchForum] add constraint IX_yaf_WatchForum unique nonclustered (ForumID,UserID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and name='IX_{objectQualifier}WatchTopic')
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] add constraint IX_{objectQualifier}WatchTopic unique nonclustered (TopicID,UserID)   
=======
if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and name='IX_yaf_WatchTopic')
	alter table [{databaseOwner}].[yaf_WatchTopic] add constraint IX_yaf_WatchTopic unique nonclustered (TopicID,UserID)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Category]') and name='IX_{objectQualifier}Category')
	alter table [{databaseOwner}].[{objectQualifier}Category] add constraint IX_{objectQualifier}Category unique nonclustered(BoardID,Name)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Category]') and name='IX_yaf_Category')
	alter table [{databaseOwner}].[yaf_Category] add constraint IX_yaf_Category unique nonclustered(BoardID,Name)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='IX_{objectQualifier}Rank')
	alter table [{databaseOwner}].[{objectQualifier}Rank] add constraint IX_{objectQualifier}Rank unique(BoardID,Name)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Rank]') and name='IX_yaf_Rank')
	alter table [{databaseOwner}].[yaf_Rank] add constraint IX_yaf_Rank unique(BoardID,Name)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IX_{objectQualifier}User')
	alter table [{databaseOwner}].[{objectQualifier}User] add constraint IX_{objectQualifier}User unique nonclustered(BoardID,Name)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='IX_yaf_User')
	alter table [{databaseOwner}].[yaf_User] add constraint IX_yaf_User unique nonclustered(BoardID,Name)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IX_{objectQualifier}Group')
	alter table [{databaseOwner}].[{objectQualifier}Group] add constraint IX_{objectQualifier}Group unique nonclustered(BoardID,Name)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='IX_yaf_Group')
	alter table [{databaseOwner}].[yaf_Group] add constraint IX_yaf_Group unique nonclustered(BoardID,Name)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and name='IX_{objectQualifier}BannedIP')
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] add constraint IX_{objectQualifier}BannedIP unique nonclustered(BoardID,Mask)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and name='IX_yaf_BannedIP')
	alter table [{databaseOwner}].[yaf_BannedIP] add constraint IX_yaf_BannedIP unique nonclustered(BoardID,Mask)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and name='IX_{objectQualifier}Smiley')
	alter table [{databaseOwner}].[{objectQualifier}Smiley] add constraint IX_{objectQualifier}Smiley unique nonclustered(BoardID,Code)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Smiley]') and name='IX_yaf_Smiley')
	alter table [{databaseOwner}].[yaf_Smiley] add constraint IX_yaf_Smiley unique nonclustered(BoardID,Code)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and name='IX_{objectQualifier}BannedIP')
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] add constraint IX_{objectQualifier}BannedIP unique nonclustered(BoardID,Mask)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and name='IX_yaf_BannedIP')
	alter table [{databaseOwner}].[yaf_BannedIP] add constraint IX_yaf_BannedIP unique nonclustered(BoardID,Mask)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Category]') and name='IX_{objectQualifier}Category')
	alter table [{databaseOwner}].[{objectQualifier}Category] add constraint IX_{objectQualifier}Category unique nonclustered(BoardID,Name)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Category]') and name='IX_yaf_Category')
	alter table [{databaseOwner}].[yaf_Category] add constraint IX_yaf_Category unique nonclustered(BoardID,Name)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and name='IX_{objectQualifier}CheckEmail')
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] add constraint IX_{objectQualifier}CheckEmail unique nonclustered(Hash)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and name='IX_yaf_CheckEmail')
	alter table [{databaseOwner}].[yaf_CheckEmail] add constraint IX_yaf_CheckEmail unique nonclustered(Hash)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IX_{objectQualifier}Forum')
	alter table [{databaseOwner}].[{objectQualifier}Forum] add constraint IX_{objectQualifier}Forum unique nonclustered(CategoryID,Name)   
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='IX_yaf_Forum')
	alter table [{databaseOwner}].[yaf_Forum] add constraint IX_yaf_Forum unique nonclustered(CategoryID,Name)   
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IX_{objectQualifier}Group')
	alter table [{databaseOwner}].[{objectQualifier}Group] add constraint IX_{objectQualifier}Group unique nonclustered(BoardID,Name)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='IX_yaf_Group')
	alter table [{databaseOwner}].[yaf_Group] add constraint IX_yaf_Group unique nonclustered(BoardID,Name)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='IX_{objectQualifier}Rank')
	alter table [{databaseOwner}].[{objectQualifier}Rank] add constraint IX_{objectQualifier}Rank unique nonclustered(BoardID,Name)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Rank]') and name='IX_yaf_Rank')
	alter table [{databaseOwner}].[yaf_Rank] add constraint IX_yaf_Rank unique nonclustered(BoardID,Name)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and name='IX_{objectQualifier}Smiley')
	alter table [{databaseOwner}].[{objectQualifier}Smiley] add constraint IX_{objectQualifier}Smiley unique nonclustered(BoardID,Code)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Smiley]') and name='IX_yaf_Smiley')
	alter table [{databaseOwner}].[yaf_Smiley] add constraint IX_yaf_Smiley unique nonclustered(BoardID,Code)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IX_{objectQualifier}User')
	alter table [{databaseOwner}].[{objectQualifier}User] add constraint IX_{objectQualifier}User unique nonclustered(BoardID,Name)
=======
if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='IX_yaf_User')
	alter table [{databaseOwner}].[yaf_User] add constraint IX_yaf_User unique nonclustered(BoardID,Name)
>>>>>>> .r1490
GO

/*
** Foreign keys
*/

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Active_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] add constraint [FK_{objectQualifier}Active_{objectQualifier}Forum] foreign key (ForumID) references [{databaseOwner}].[{objectQualifier}Forum (ForumID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Active_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] add constraint FK_yaf_Active_yaf_Forum foreign key (ForumID) references [{databaseOwner}].[yaf_Forum] (ForumID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Active_{objectQualifier}Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] add constraint [FK_{objectQualifier}Active_{objectQualifier}Topic] foreign key (TopicID) references [{databaseOwner}].[{objectQualifier}Topic (TopicID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Active_yaf_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] add constraint FK_yaf_Active_yaf_Topic foreign key (TopicID) references [{databaseOwner}].[yaf_Topic] (TopicID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Active_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] add constraint [FK_{objectQualifier}Active_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User (UserID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Active_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] add constraint FK_yaf_Active_yaf_User foreign key (UserID) references [{databaseOwner}].[yaf_User] (UserID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}CheckEmail_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] add constraint [FK_{objectQualifier}CheckEmail_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User (UserID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_CheckEmail_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_CheckEmail] add constraint FK_yaf_CheckEmail_yaf_User foreign key (UserID) references [{databaseOwner}].[yaf_User] (UserID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Choice_{objectQualifier}Poll' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Choice]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Choice] add constraint [FK_{objectQualifier}Choice_{objectQualifier}Poll] foreign key (PollID) references [{databaseOwner}].[{objectQualifier}Poll (PollID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Choice_yaf_Poll' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Choice]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Choice] add constraint FK_yaf_Choice_yaf_Poll foreign key (PollID) references [{databaseOwner}].[yaf_Poll] (PollID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Forum_{objectQualifier}Category' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] add constraint [FK_{objectQualifier}Forum_{objectQualifier}Category] foreign key (CategoryID) references [{databaseOwner}].[{objectQualifier}Category (CategoryID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Forum_yaf_Category' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] add constraint FK_yaf_Forum_yaf_Category foreign key (CategoryID) references [{databaseOwner}].[yaf_Category] (CategoryID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Forum_{objectQualifier}Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] add constraint [FK_{objectQualifier}Forum_{objectQualifier}Message] foreign key (LastMessageID) references [{databaseOwner}].[{objectQualifier}Message (MessageID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Forum_yaf_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] add constraint FK_yaf_Forum_yaf_Message foreign key (LastMessageID) references [{databaseOwner}].[yaf_Message] (MessageID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Forum_{objectQualifier}Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] add constraint [FK_{objectQualifier}Forum_{objectQualifier}Topic] foreign key (LastTopicID) references [{databaseOwner}].[{objectQualifier}Topic (TopicID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Forum_yaf_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] add constraint FK_yaf_Forum_yaf_Topic foreign key (LastTopicID) references [{databaseOwner}].[yaf_Topic] (TopicID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Forum_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] add constraint [FK_{objectQualifier}Forum_{objectQualifier}User] foreign key (LastUserID) references [{databaseOwner}].[{objectQualifier}User (UserID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Forum_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] add constraint FK_yaf_Forum_yaf_User foreign key (LastUserID) references [{databaseOwner}].[yaf_User] (UserID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}ForumAccess_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] add constraint [FK_{objectQualifier}ForumAccess_{objectQualifier}Forum] foreign key (ForumID) references [{databaseOwner}].[{objectQualifier}Forum (ForumID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_ForumAccess_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_ForumAccess] add constraint FK_yaf_ForumAccess_yaf_Forum foreign key (ForumID) references [{databaseOwner}].[yaf_Forum] (ForumID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}ForumAccess_{objectQualifier}Group' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] add constraint [FK_{objectQualifier}ForumAccess_{objectQualifier}Group] foreign key (GroupID) references [{databaseOwner}].[{objectQualifier}Group (GroupID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_ForumAccess_yaf_Group' and parent_obj=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_ForumAccess] add constraint FK_yaf_ForumAccess_yaf_Group foreign key (GroupID) references [{databaseOwner}].[yaf_Group] (GroupID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Message_{objectQualifier}Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Message] add constraint [FK_{objectQualifier}Message_{objectQualifier}Topic] foreign key (TopicID) references [{databaseOwner}].[{objectQualifier}Topic (TopicID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Message_yaf_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Message] add constraint FK_yaf_Message_yaf_Topic foreign key (TopicID) references [{databaseOwner}].[yaf_Topic] (TopicID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Message_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Message] add constraint [FK_{objectQualifier}Message_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User (UserID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Message_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Message] add constraint FK_yaf_Message_yaf_User foreign key (UserID) references [{databaseOwner}].[yaf_User] (UserID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}PMessage_{objectQualifier}User1' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}PMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}PMessage] add constraint [FK_{objectQualifier}PMessage_{objectQualifier}User1] foreign key (FromUserID) references [{databaseOwner}].[{objectQualifier}User (UserID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_PMessage_yaf_User1' and parent_obj=object_id(N'[{databaseOwner}].[yaf_PMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_PMessage] add constraint FK_yaf_PMessage_yaf_User1 foreign key (FromUserID) references [{databaseOwner}].[yaf_User] (UserID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Topic_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] add constraint [FK_{objectQualifier}Topic_{objectQualifier}Forum] foreign key (ForumID) references [{databaseOwner}].[{objectQualifier}Forum (ForumID) ON DELETE CASCADE
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Topic_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] add constraint FK_yaf_Topic_yaf_Forum foreign key (ForumID) references [{databaseOwner}].[yaf_Forum] (ForumID) ON DELETE CASCADE
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Topic_{objectQualifier}Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] add constraint [FK_{objectQualifier}Topic_{objectQualifier}Message] foreign key (LastMessageID) references [{databaseOwner}].[{objectQualifier}Message (MessageID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Topic_yaf_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] add constraint FK_yaf_Topic_yaf_Message foreign key (LastMessageID) references [{databaseOwner}].[yaf_Message] (MessageID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Topic_{objectQualifier}Poll' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] add constraint [FK_{objectQualifier}Topic_{objectQualifier}Poll] foreign key (PollID) references [{databaseOwner}].[{objectQualifier}Poll (PollID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Topic_yaf_Poll' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] add constraint FK_yaf_Topic_yaf_Poll foreign key (PollID) references [{databaseOwner}].[yaf_Poll] (PollID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Topic_{objectQualifier}Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] add constraint [FK_{objectQualifier}Topic_{objectQualifier}Topic] foreign key (TopicMovedID) references [{databaseOwner}].[{objectQualifier}Topic (TopicID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Topic_yaf_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] add constraint FK_yaf_Topic_yaf_Topic foreign key (TopicMovedID) references [{databaseOwner}].[yaf_Topic] (TopicID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Topic_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] add constraint [FK_{objectQualifier}Topic_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User (UserID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Topic_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] add constraint FK_yaf_Topic_yaf_User foreign key (UserID) references [{databaseOwner}].[yaf_User] (UserID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Topic_{objectQualifier}User2' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] add constraint [FK_{objectQualifier}Topic_{objectQualifier}User2] foreign key (LastUserID) references [{databaseOwner}].[{objectQualifier}User (UserID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Topic_yaf_User2' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] add constraint FK_yaf_Topic_yaf_User2 foreign key (LastUserID) references [{databaseOwner}].[yaf_User] (UserID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}WatchForum_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] add constraint [FK_{objectQualifier}WatchForum_{objectQualifier}Forum] foreign key (ForumID) references [{databaseOwner}].[{objectQualifier}Forum(ForumID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_WatchForum_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchForum] add constraint FK_yaf_WatchForum_yaf_Forum foreign key (ForumID) references [{databaseOwner}].[yaf_Forum](ForumID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}WatchForum_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] add constraint [FK_{objectQualifier}WatchForum_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User(UserID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_WatchForum_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchForum] add constraint FK_yaf_WatchForum_yaf_User foreign key (UserID) references [{databaseOwner}].[yaf_User](UserID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}WatchTopic_{objectQualifier}Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] add constraint [FK_{objectQualifier}WatchTopic_{objectQualifier}Topic] foreign key (TopicID) references [{databaseOwner}].[{objectQualifier}Topic(TopicID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_WatchTopic_yaf_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchTopic] add constraint FK_yaf_WatchTopic_yaf_Topic foreign key (TopicID) references [{databaseOwner}].[yaf_Topic](TopicID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}WatchTopic_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] add constraint [FK_{objectQualifier}WatchTopic_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User(UserID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_WatchTopic_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchTopic] add constraint FK_yaf_WatchTopic_yaf_User foreign key (UserID) references [{databaseOwner}].[yaf_User](UserID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Active_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Attachment] add constraint [FK_{objectQualifier}Active_{objectQualifier}Forum] foreign key (MessageID) references {objectQualifier}Message (MessageID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Active_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Attachment] add constraint FK_yaf_Active_yaf_Forum foreign key (MessageID) references yaf_Message (MessageID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}UserGroup_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserGroup] add constraint [FK_{objectQualifier}UserGroup_{objectQualifier}User] foreign key (UserID) references {objectQualifier}User(UserID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_UserGroup_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserGroup] add constraint FK_yaf_UserGroup_yaf_User foreign key (UserID) references yaf_User(UserID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}UserGroup_{objectQualifier}Group' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserGroup] add constraint [FK_{objectQualifier}UserGroup_{objectQualifier}Group] foreign key(GroupID) references {objectQualifier}Group (GroupID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_UserGroup_yaf_Group' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserGroup] add constraint FK_yaf_UserGroup_yaf_Group foreign key(GroupID) references yaf_Group (GroupID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Attachment_{objectQualifier}Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Attachment]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Attachment] add constraint [FK_{objectQualifier}Attachment_{objectQualifier}Message] foreign key (MessageID) references {objectQualifier}Message (MessageID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Attachment_yaf_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Attachment]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Attachment] add constraint FK_yaf_Attachment_yaf_Message foreign key (MessageID) references yaf_Message (MessageID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}NntpForum_{objectQualifier}NntpServer' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpForum] add constraint [FK_{objectQualifier}NntpForum_{objectQualifier}NntpServer] foreign key (NntpServerID) references {objectQualifier}NntpServer(NntpServerID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_NntpForum_yaf_NntpServer' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpForum] add constraint FK_yaf_NntpForum_yaf_NntpServer foreign key (NntpServerID) references yaf_NntpServer(NntpServerID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}NntpForum_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpForum] add constraint [FK_{objectQualifier}NntpForum_{objectQualifier}Forum] foreign key (ForumID) references {objectQualifier}Forum(ForumID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_NntpForum_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpForum] add constraint FK_yaf_NntpForum_yaf_Forum foreign key (ForumID) references yaf_Forum(ForumID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}NntpTopic_{objectQualifier}NntpForum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpTopic] add constraint [FK_{objectQualifier}NntpTopic_{objectQualifier}NntpForum] foreign key (NntpForumID) references {objectQualifier}NntpForum(NntpForumID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_NntpTopic_yaf_NntpForum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpTopic] add constraint FK_yaf_NntpTopic_yaf_NntpForum foreign key (NntpForumID) references yaf_NntpForum(NntpForumID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}NntpTopic_{objectQualifier}Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpTopic] add constraint [FK_{objectQualifier}NntpTopic_{objectQualifier}Topic] foreign key (TopicID) references {objectQualifier}Topic(TopicID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_NntpTopic_yaf_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpTopic] add constraint FK_yaf_NntpTopic_yaf_Topic foreign key (TopicID) references yaf_Topic(TopicID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}ForumAccess_{objectQualifier}AccessMask' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] add constraint [FK_{objectQualifier}ForumAccess_{objectQualifier}AccessMask] foreign key (AccessMaskID) references {objectQualifier}AccessMask (AccessMaskID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_ForumAccess_yaf_AccessMask' and parent_obj=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_ForumAccess] add constraint FK_yaf_ForumAccess_yaf_AccessMask foreign key (AccessMaskID) references yaf_AccessMask (AccessMaskID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}UserForum_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserForum] add constraint [FK_{objectQualifier}UserForum_{objectQualifier}User] foreign key (UserID) references {objectQualifier}User (UserID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_UserForum_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserForum] add constraint FK_yaf_UserForum_yaf_User foreign key (UserID) references yaf_User (UserID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}UserForum_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserForum] add constraint [FK_{objectQualifier}UserForum_{objectQualifier}Forum] foreign key (ForumID) references {objectQualifier}Forum (ForumID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_UserForum_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserForum] add constraint FK_yaf_UserForum_yaf_Forum foreign key (ForumID) references yaf_Forum (ForumID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}UserForum_{objectQualifier}AccessMask' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserForum] add constraint [FK_{objectQualifier}UserForum_{objectQualifier}AccessMask] foreign key (AccessMaskID) references {objectQualifier}AccessMask (AccessMaskID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_UserForum_yaf_AccessMask' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserForum] add constraint FK_yaf_UserForum_yaf_AccessMask foreign key (AccessMaskID) references yaf_AccessMask (AccessMaskID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Category_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Category]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Category] add constraint [FK_{objectQualifier}Category_{objectQualifier}Board] foreign key(BoardID) references {objectQualifier}Board (BoardID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Category_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Category]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Category] add constraint FK_yaf_Category_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}AccessMask_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] add constraint [FK_{objectQualifier}AccessMask_{objectQualifier}Board] foreign key(BoardID) references {objectQualifier}Board (BoardID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_AccessMask_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_AccessMask] add constraint FK_yaf_AccessMask_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Active_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] add constraint [FK_{objectQualifier}Active_{objectQualifier}Board] foreign key(BoardID) references {objectQualifier}Board (BoardID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Active_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] add constraint FK_yaf_Active_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}BannedIP_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] add constraint [FK_{objectQualifier}BannedIP_{objectQualifier}Board] foreign key(BoardID) references {objectQualifier}Board (BoardID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_BannedIP_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_BannedIP] add constraint FK_yaf_BannedIP_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Group_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Group]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Group] add constraint [FK_{objectQualifier}Group_{objectQualifier}Board] foreign key(BoardID) references {objectQualifier}Board (BoardID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Group_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Group]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Group] add constraint FK_yaf_Group_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}NntpServer_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpServer]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpServer] add constraint [FK_{objectQualifier}NntpServer_{objectQualifier}Board] foreign key(BoardID) references {objectQualifier}Board (BoardID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_NntpServer_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpServer]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpServer] add constraint FK_yaf_NntpServer_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Rank_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Rank] add constraint [FK_{objectQualifier}Rank_{objectQualifier}Board] foreign key(BoardID) references {objectQualifier}Board (BoardID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Rank_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Rank]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Rank] add constraint FK_yaf_Rank_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Smiley_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Smiley] add constraint [FK_{objectQualifier}Smiley_{objectQualifier}Board] foreign key(BoardID) references {objectQualifier}Board (BoardID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Smiley_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Smiley]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Smiley] add constraint FK_yaf_Smiley_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}User_{objectQualifier}Rank' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}User] add constraint [FK_{objectQualifier}User_{objectQualifier}Rank] foreign key(RankID) references {objectQualifier}Rank(RankID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_User_yaf_Rank' and parent_obj=object_id(N'[{databaseOwner}].[yaf_User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_User] add constraint FK_yaf_User_yaf_Rank foreign key(RankID) references yaf_Rank(RankID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}User_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}User] add constraint [FK_{objectQualifier}User_{objectQualifier}Board] foreign key(BoardID) references {objectQualifier}Board(BoardID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_User_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_User] add constraint FK_yaf_User_yaf_Board foreign key(BoardID) references yaf_Board(BoardID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Forum_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] add constraint [FK_{objectQualifier}Forum_{objectQualifier}Forum] foreign key(ParentID) references {objectQualifier}Forum(ForumID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Forum_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] add constraint FK_yaf_Forum_yaf_Forum foreign key(ParentID) references yaf_Forum(ForumID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Message_{objectQualifier}Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Message] add constraint [FK_{objectQualifier}Message_{objectQualifier}Message] foreign key(ReplyTo) references {objectQualifier}Message(MessageID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Message_yaf_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Message] add constraint FK_yaf_Message_yaf_Message foreign key(ReplyTo) references yaf_Message(MessageID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}UserPMessage_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserPMessage] add constraint [FK_{objectQualifier}UserPMessage_{objectQualifier}User] foreign key (UserID) references {objectQualifier}User (UserID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_UserPMessage_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserPMessage] add constraint FK_yaf_UserPMessage_yaf_User foreign key (UserID) references yaf_User (UserID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}UserPMessage_{objectQualifier}PMessage' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserPMessage] add constraint [FK_{objectQualifier}UserPMessage_{objectQualifier}PMessage] foreign key (PMessageID) references {objectQualifier}PMessage (PMessageID)
=======
if not exists(select 1 from sysobjects where name='FK_yaf_UserPMessage_yaf_PMessage' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserPMessage] add constraint FK_yaf_UserPMessage_yaf_PMessage foreign key (PMessageID) references yaf_PMessage (PMessageID)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}Registry_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Registry]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Registry] add constraint [FK_{objectQualifier}Registry_{objectQualifier}Board] foreign key(BoardID) references {objectQualifier}Board(BoardID) on delete cascade
=======
if not exists(select 1 from sysobjects where name='FK_yaf_Registry_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Registry]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Registry] add constraint FK_yaf_Registry_yaf_Board foreign key(BoardID) references yaf_Board(BoardID) on delete cascade
>>>>>>> .r1490
go

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}PollVote_{objectQualifier}Poll' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}PollVote]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}PollVote] add constraint [FK_{objectQualifier}PollVote_{objectQualifier}Poll] foreign key(PollID) references {objectQualifier}Poll(PollID) on delete cascade
=======
if not exists(select 1 from sysobjects where name='FK_yaf_PollVote_yaf_Poll' and parent_obj=object_id(N'[{databaseOwner}].[yaf_PollVote]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_PollVote] add constraint FK_yaf_PollVote_yaf_Poll foreign key(PollID) references yaf_Poll(PollID) on delete cascade
>>>>>>> .r1490
go

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name='FK_{objectQualifier}EventLog_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}EventLog]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}EventLog] add constraint [FK_{objectQualifier}EventLog_{objectQualifier}User] foreign key(UserID) references [{databaseOwner}].[{objectQualifier}User(UserID) on delete cascade
=======
if not exists(select 1 from sysobjects where name='FK_yaf_EventLog_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_EventLog]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_EventLog] add constraint FK_yaf_EventLog_yaf_User foreign key(UserID) references [{databaseOwner}].[yaf_User](UserID) on delete cascade
>>>>>>> .r1490
go

/* Default Constraints */

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name=N'DF_{objectQualifier}Message_Flags' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}Message]'))
	alter table [{databaseOwner}].[{objectQualifier}Message] drop constraint [DF_{objectQualifier}Message_Flags]
=======
if exists(select 1 from sysobjects where name=N'DF_yaf_Message_Flags' and parent_obj=object_id(N'yaf_Message'))
	alter table [{databaseOwner}].[yaf_Message] drop constraint DF_yaf_Message_Flags
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name=N'DF_{objectQualifier}Message_Flags' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}Message]'))
	alter table [{databaseOwner}].[{objectQualifier}Message] add constraint [DF_{objectQualifier}Message_Flags] default (23) for Flags
=======
if not exists(select 1 from sysobjects where name=N'DF_yaf_Message_Flags' and parent_obj=object_id(N'yaf_Message'))
	alter table [{databaseOwner}].[yaf_Message] add constraint DF_yaf_Message_Flags default (23) for Flags
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name=N'DF_EventLog_EventTime' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]'))
	alter table [{databaseOwner}].[{objectQualifier}EventLog] drop constraint [DF_EventLog_EventTime]
=======
if exists(select 1 from sysobjects where name=N'DF_EventLog_EventTime' and parent_obj=object_id(N'yaf_EventLog'))
	alter table [{databaseOwner}].[yaf_EventLog] drop constraint DF_EventLog_EventTime
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name=N'DF_{objectQualifier}EventLog_EventTime' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]'))
	alter table [{databaseOwner}].[{objectQualifier}EventLog] add constraint [DF_{objectQualifier}EventLog_EventTime] default(getdate()) for EventTime
=======
if not exists(select 1 from sysobjects where name=N'DF_yaf_EventLog_EventTime' and parent_obj=object_id(N'yaf_EventLog'))
	alter table [{databaseOwner}].[yaf_EventLog] add constraint DF_yaf_EventLog_EventTime default(getdate()) for EventTime
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from dbo.sysobjects where name=N'DF_EventLog_Type' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]'))
	alter table [{databaseOwner}].[{objectQualifier}EventLog] drop constraint [DF_EventLog_Type]
=======
if exists(select 1 from sysobjects where name=N'DF_EventLog_Type' and parent_obj=object_id(N'yaf_EventLog'))
	alter table [{databaseOwner}].[yaf_EventLog] drop constraint DF_EventLog_Type
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysobjects where name=N'DF_{objectQualifier}EventLog_Type' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]'))
	alter table [{databaseOwner}].[{objectQualifier}EventLog] add constraint [DF_{objectQualifier}EventLog_Type] default(0) for Type
=======
if not exists(select 1 from sysobjects where name=N'DF_yaf_EventLog_Type' and parent_obj=object_id(N'yaf_EventLog'))
	alter table [{databaseOwner}].[yaf_EventLog] add constraint DF_yaf_EventLog_Type default(0) for Type
>>>>>>> .r1490
GO
