/*
** Drop old Foreign keys
*/

if exists(select 1 from sysobjects where name='FK_Active_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] drop constraint FK_Active_Forum
GO

if exists(select 1 from sysobjects where name='FK_Active_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] drop constraint FK_Active_Topic
GO

if exists(select 1 from sysobjects where name='FK_Active_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] drop constraint FK_Active_User
GO

if exists(select 1 from sysobjects where name='FK_CheckEmail_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_CheckEmail] drop constraint FK_CheckEmail_User
GO

if exists(select 1 from sysobjects where name='FK_Choice_Poll' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Choice]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Choice] drop constraint FK_Choice_Poll
GO

if exists(select 1 from sysobjects where name='FK_Forum_Category' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] drop constraint FK_Forum_Category
GO

if exists(select 1 from sysobjects where name='FK_Forum_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] drop constraint FK_Forum_Message
GO

if exists(select 1 from sysobjects where name='FK_Forum_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] drop constraint FK_Forum_Topic
GO

if exists(select 1 from sysobjects where name='FK_Forum_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] drop constraint FK_Forum_User
GO

if exists(select 1 from sysobjects where name='FK_ForumAccess_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_ForumAccess] drop constraint FK_ForumAccess_Forum
GO

if exists(select 1 from sysobjects where name='FK_ForumAccess_Group' and parent_obj=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_ForumAccess] drop constraint FK_ForumAccess_Group
GO

if exists(select 1 from sysobjects where name='FK_Message_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Message] drop constraint FK_Message_Topic
GO

if exists(select 1 from sysobjects where name='FK_Message_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Message] drop constraint FK_Message_User
GO

if exists(select 1 from sysobjects where name='FK_PMessage_User1' and parent_obj=object_id(N'[{databaseOwner}].[yaf_PMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_PMessage] drop constraint FK_PMessage_User1
GO

if exists(select 1 from sysobjects where name='FK_Topic_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] drop constraint FK_Topic_Forum
GO

if exists(select 1 from sysobjects where name='FK_Topic_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] drop constraint FK_Topic_Message
GO

if exists(select 1 from sysobjects where name='FK_Topic_Poll' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] drop constraint FK_Topic_Poll
GO

if exists(select 1 from sysobjects where name='FK_Topic_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] drop constraint FK_Topic_Topic
GO

if exists(select 1 from sysobjects where name='FK_Topic_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] drop constraint FK_Topic_User
GO

if exists(select 1 from sysobjects where name='FK_Topic_User2' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] drop constraint FK_Topic_User2
GO

if exists(select 1 from sysobjects where name='FK_WatchForum_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchForum] drop constraint FK_WatchForum_Forum
GO

if exists(select 1 from sysobjects where name='FK_WatchForum_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchForum] drop constraint FK_WatchForum_User
GO

if exists(select 1 from sysobjects where name='FK_WatchTopic_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchTopic] drop constraint FK_WatchTopic_Topic
GO

if exists(select 1 from sysobjects where name='FK_WatchTopic_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchTopic] drop constraint FK_WatchTopic_User
GO

if exists(select 1 from sysobjects where name='FK_Active_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Attachment] drop constraint FK_Attachment_Message
GO

if exists(select 1 from sysobjects where name='FK_UserGroup_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserGroup] drop constraint FK_UserGroup_User
GO

if exists(select 1 from sysobjects where name='FK_UserGroup_Group' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserGroup] drop constraint FK_UserGroup_Group
GO

if exists(select 1 from sysobjects where name='FK_Attachment_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Attachment]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Attachment] drop constraint FK_Attachment_Message
GO

if exists(select 1 from sysobjects where name='FK_NntpForum_NntpServer' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpForum] drop constraint FK_NntpForum_NntpServer
GO

if exists(select 1 from sysobjects where name='FK_NntpForum_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpForum] drop constraint FK_NntpForum_Forum
GO

if exists(select 1 from sysobjects where name='FK_NntpTopic_NntpForum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpTopic] drop constraint FK_NntpTopic_NntpForum
GO

if exists(select 1 from sysobjects where name='FK_NntpTopic_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpTopic] drop constraint FK_NntpTopic_Topic
GO

if exists(select 1 from sysobjects where name='FK_ForumAccess_AccessMask' and parent_obj=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_ForumAccess] drop constraint FK_ForumAccess_AccessMask
GO

if exists(select 1 from sysobjects where name='FK_UserForum_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserForum] drop constraint FK_UserForum_User
GO

if exists(select 1 from sysobjects where name='FK_UserForum_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserForum] drop constraint FK_UserForum_Forum
GO

if exists(select 1 from sysobjects where name='FK_UserForum_AccessMask' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserForum] drop constraint FK_UserForum_AccessMask
GO

if exists(select 1 from sysobjects where name='FK_Category_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Category]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Category] drop constraint FK_Category_Board
GO

if exists(select 1 from sysobjects where name='FK_AccessMask_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_AccessMask] drop constraint FK_AccessMask_Board
GO

if exists(select 1 from sysobjects where name='FK_Active_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] drop constraint FK_Active_Board
GO

if exists(select 1 from sysobjects where name='FK_BannedIP_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_BannedIP] drop constraint FK_BannedIP_Board
GO

if exists(select 1 from sysobjects where name='FK_Group_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Group]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Group] drop constraint FK_Group_Board
GO

if exists(select 1 from sysobjects where name='FK_NntpServer_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpServer]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpServer] drop constraint FK_NntpServer_Board
GO

if exists(select 1 from sysobjects where name='FK_Rank_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Rank]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Rank] drop constraint FK_Rank_Board
GO

if exists(select 1 from sysobjects where name='FK_Smiley_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Smiley]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Smiley] drop constraint FK_Smiley_Board
GO

if exists(select 1 from sysobjects where name='FK_User_Rank' and parent_obj=object_id(N'[{databaseOwner}].[yaf_User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_User] drop constraint FK_User_Rank
GO

if exists(select 1 from sysobjects where name='FK_User_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_User] drop constraint FK_User_Board
GO

if exists(select 1 from sysobjects where name='FK_Forum_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] drop constraint FK_Forum_Forum
GO

if exists(select 1 from sysobjects where name='FK_Message_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Message] drop constraint FK_Message_Message
GO

if exists(select 1 from sysobjects where name='FK_UserPMessage_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserPMessage] drop constraint FK_UserPMessage_User
GO

if exists(select 1 from sysobjects where name='FK_UserPMessage_PMessage' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserPMessage] drop constraint FK_UserPMessage_PMessage
GO

if exists(select 1 from sysobjects where name='FK_Registry_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Registry]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Registry] drop constraint FK_Registry_Board
go

if exists(select 1 from sysobjects where name='FK_EventLog_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_EventLog]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_EventLog] drop constraint FK_EventLog_User
go

if exists(select 1 from sysobjects where name='FK_yaf_PollVote_yaf_Poll' and parent_obj=object_id(N'[{databaseOwner}].[yaf_PollVote]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_PollVote] drop constraint FK_yaf_PollVote_yaf_Poll
go

/* Drop old primary keys */

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and name='PK_BannedIP')
	alter table [{databaseOwner}].[yaf_BannedIP] drop constraint PK_BannedIP
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Category]') and name='PK_Category')
	alter table [{databaseOwner}].[yaf_Category] drop constraint PK_Category
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and name='PK_CheckEmail')
	alter table [{databaseOwner}].[yaf_CheckEmail] drop constraint PK_CheckEmail
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Choice]') and name='PK_Choice')
	alter table [{databaseOwner}].[yaf_Choice] drop constraint PK_Choice
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='PK_Forum')
	alter table [{databaseOwner}].[yaf_Forum] drop constraint PK_Forum
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and name='PK_ForumAccess')
	alter table [{databaseOwner}].[yaf_ForumAccess] drop constraint PK_ForumAccess
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='PK_Group')
	alter table [{databaseOwner}].[yaf_Group] drop constraint PK_Group
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Mail]') and name='PK_Mail')
	alter table [{databaseOwner}].[yaf_Mail] drop constraint PK_Mail
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Message]') and name='PK_Message')
	alter table [{databaseOwner}].[yaf_Message] drop constraint PK_Message
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_PMessage]') and name='PK_PMessage')
	alter table [{databaseOwner}].[yaf_PMessage] drop constraint PK_PMessage
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Poll]') and name='PK_Poll')
	alter table [{databaseOwner}].[yaf_Poll] drop constraint PK_Poll
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Smiley]') and name='PK_Smiley')
	alter table [{databaseOwner}].[yaf_Smiley] drop constraint PK_Smiley
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Topic]') and name='PK_Topic')
	alter table [{databaseOwner}].[yaf_Topic] drop constraint PK_Topic
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='PK_User')
	alter table [{databaseOwner}].[yaf_User] drop constraint PK_User
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and name='PK_WatchForum')
	alter table [{databaseOwner}].[yaf_WatchForum] drop constraint PK_WatchForum
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and name='PK_WatchTopic')
	alter table [{databaseOwner}].[yaf_WatchTopic] drop constraint PK_WatchTopic
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_UserGroup]') and name='PK_UserGroup')
	alter table [{databaseOwner}].[yaf_UserGroup] drop constraint PK_UserGroup
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Rank]') and name='PK_Rank')
	alter table [{databaseOwner}].[yaf_Rank] drop constraint PK_Rank
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_NntpServer]') and name='PK_NntpServer')
	alter table [{databaseOwner}].[yaf_NntpServer] drop constraint PK_NntpServer
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_NntpForum]') and name='PK_NntpForum')
	alter table [{databaseOwner}].[yaf_NntpForum] drop constraint PK_NntpForum
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_NntpTopic]') and name='PK_NntpTopic')
	alter table [{databaseOwner}].[yaf_NntpTopic] drop constraint PK_NntpTopic
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and name='PK_AccessMask')
	alter table [{databaseOwner}].[yaf_AccessMask] drop constraint PK_AccessMask
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_UserForum]') and name='PK_UserForum')
	alter table [{databaseOwner}].[yaf_UserForum] drop constraint PK_UserForum
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Board]') and name='PK_Board')
	alter table [{databaseOwner}].[yaf_Board] drop constraint PK_Board
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Active]') and name='PK_Active')
	alter table [{databaseOwner}].[yaf_Active] drop constraint PK_Active
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_UserPMessage]') and name='PK_UserPMessage')
	alter table [{databaseOwner}].[yaf_UserPMessage] drop constraint PK_UserPMessage
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Attachment]') and name='PK_Attachment')
	alter table [{databaseOwner}].[yaf_Attachment] drop constraint PK_Attachment
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Active]') and name='PK_Active')
	alter table [{databaseOwner}].[yaf_Active] drop constraint PK_Active
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_PollVote]') and name='PK_PollVote')
	alter table [{databaseOwner}].[yaf_PollVote] drop constraint PK_PollVote
GO

/*
** Unique constraints
*/

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and name='IX_CheckEmail')
	alter table [{databaseOwner}].[yaf_CheckEmail] drop constraint IX_CheckEmail
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='IX_Forum')
	alter table [{databaseOwner}].[yaf_Forum] drop constraint IX_Forum
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and name='IX_WatchForum')
	alter table [{databaseOwner}].[yaf_WatchForum] drop constraint IX_WatchForum
GO

if exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and name='IX_WatchTopic')
	alter table [{databaseOwner}].[yaf_WatchTopic] drop constraint IX_WatchTopic
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Category]') and name='IX_Category')
	alter table [{databaseOwner}].[yaf_Category] drop constraint IX_Category
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Rank]') and name='IX_Rank')
	alter table [{databaseOwner}].[yaf_Rank] drop constraint IX_Rank
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='IX_User')
	alter table [{databaseOwner}].[yaf_User] drop constraint IX_User
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='IX_Group')
	alter table [{databaseOwner}].[yaf_Group] drop constraint IX_Group
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and name='IX_BannedIP')
	alter table [{databaseOwner}].[yaf_BannedIP] drop constraint IX_BannedIP
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Smiley]') and name='IX_Smiley')
	alter table [{databaseOwner}].[yaf_Smiley] drop constraint IX_Smiley
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and name='IX_BannedIP')
	alter table [{databaseOwner}].[yaf_BannedIP] drop constraint IX_BannedIP
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Category]') and name='IX_Category')
	alter table [{databaseOwner}].[yaf_Category] drop constraint IX_Category
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and name='IX_CheckEmail')
	alter table [{databaseOwner}].[yaf_CheckEmail] drop constraint IX_CheckEmail
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='IX_Forum')
	alter table [{databaseOwner}].[yaf_Forum] drop constraint IX_Forum
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='IX_Group')
	alter table [{databaseOwner}].[yaf_Group] drop constraint IX_Group
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Rank]') and name='IX_Rank')
	alter table [{databaseOwner}].[yaf_Rank] drop constraint IX_Rank
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Smiley]') and name='IX_Smiley')
	alter table [{databaseOwner}].[yaf_Smiley] drop constraint IX_Smiley
GO

if exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='IX_User')
	alter table [{databaseOwner}].[yaf_User] drop constraint IX_User
GO

/* Build new constraints */

/*
** Primary keys
*/

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and name='PK_yaf_BannedIP')
	alter table [{databaseOwner}].[yaf_BannedIP] with nocheck add constraint PK_yaf_BannedIP primary key clustered(ID)
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Category]') and name='PK_yaf_Category')
	alter table [{databaseOwner}].[yaf_Category] with nocheck add constraint PK_yaf_Category primary key clustered(CategoryID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and name='PK_yaf_CheckEmail')
	alter table [{databaseOwner}].[yaf_CheckEmail] with nocheck add constraint PK_yaf_CheckEmail primary key clustered(CheckEmailID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Choice]') and name='PK_yaf_Choice')
	alter table [{databaseOwner}].[yaf_Choice] with nocheck add constraint PK_yaf_Choice primary key clustered(ChoiceID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='PK_yaf_Forum')
	alter table [{databaseOwner}].[yaf_Forum] with nocheck add constraint PK_yaf_Forum primary key clustered(ForumID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and name='PK_yaf_ForumAccess')
	alter table [{databaseOwner}].[yaf_ForumAccess] with nocheck add constraint PK_yaf_ForumAccess primary key clustered(GroupID,ForumID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='PK_yaf_Group')
	alter table [{databaseOwner}].[yaf_Group] with nocheck add constraint PK_yaf_Group primary key clustered(GroupID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Mail]') and name='PK_yaf_Mail')
	alter table [{databaseOwner}].[yaf_Mail] with nocheck add constraint PK_yaf_Mail primary key clustered(MailID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Message]') and name='PK_yaf_Message')
	alter table [{databaseOwner}].[yaf_Message] with nocheck add constraint PK_yaf_Message primary key clustered(MessageID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_PMessage]') and name='PK_yaf_PMessage')
	alter table [{databaseOwner}].[yaf_PMessage] with nocheck add constraint PK_yaf_PMessage primary key clustered(PMessageID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Poll]') and name='PK_yaf_Poll')
	alter table [{databaseOwner}].[yaf_Poll] with nocheck add constraint PK_yaf_Poll primary key clustered(PollID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Smiley]') and name='PK_yaf_Smiley')
	alter table [{databaseOwner}].[yaf_Smiley] with nocheck add constraint PK_yaf_Smiley primary key clustered(SmileyID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Topic]') and name='PK_yaf_Topic')
	alter table [{databaseOwner}].[yaf_Topic] with nocheck add constraint PK_yaf_Topic primary key clustered(TopicID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='PK_yaf_yaf_User')
	alter table [{databaseOwner}].[yaf_User] with nocheck add constraint PK_yaf_yaf_User primary key clustered(UserID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and name='PK_yaf_WatchForum')
	alter table [{databaseOwner}].[yaf_WatchForum] with nocheck add constraint PK_yaf_WatchForum primary key clustered(WatchForumID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and name='PK_yaf_WatchTopic')
	alter table [{databaseOwner}].[yaf_WatchTopic] with nocheck add constraint PK_yaf_WatchTopic primary key clustered(WatchTopicID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_UserGroup]') and name='PK_yaf_UserGroup')
	alter table [{databaseOwner}].[yaf_UserGroup] with nocheck add constraint PK_yaf_UserGroup primary key clustered(UserID,GroupID)
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Rank]') and name='PK_yaf_Rank')
	alter table [{databaseOwner}].[yaf_Rank] with nocheck add constraint PK_yaf_Rank primary key clustered(RankID)
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_NntpServer]') and name='PK_yaf_NntpServer')
	alter table [{databaseOwner}].[yaf_NntpServer] with nocheck add constraint PK_yaf_NntpServer primary key clustered (NntpServerID) 
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_NntpForum]') and name='PK_yaf_NntpForum')
	alter table [{databaseOwner}].[yaf_NntpForum] with nocheck add constraint PK_yaf_NntpForum primary key clustered (NntpForumID) 
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_NntpTopic]') and name='PK_yaf_NntpTopic')
	alter table [{databaseOwner}].[yaf_NntpTopic] with nocheck add constraint PK_yaf_NntpTopic primary key clustered (NntpTopicID) 
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and name='PK_yaf_AccessMask')
	alter table [{databaseOwner}].[yaf_AccessMask] with nocheck add constraint PK_yaf_AccessMask primary key clustered (AccessMaskID) 
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_UserForum]') and name='PK_yaf_UserForum')
	alter table [{databaseOwner}].[yaf_UserForum] with nocheck add constraint PK_yaf_UserForum primary key clustered (UserID,ForumID) 
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Board]') and name='PK_yaf_Board')
	alter table [{databaseOwner}].[yaf_Board] with nocheck add constraint PK_yaf_Board primary key clustered (BoardID)
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Active]') and name='PK_yaf_Active')
	alter table [{databaseOwner}].[yaf_Active] with nocheck add constraint PK_yaf_Active primary key clustered(SessionID,BoardID)
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_UserPMessage]') and name='PK_yaf_UserPMessage')
	alter table [{databaseOwner}].[yaf_UserPMessage] with nocheck add constraint PK_yaf_UserPMessage primary key clustered (UserPMessageID) 
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Attachment]') and name='PK_yaf_Attachment')
	alter table [{databaseOwner}].[yaf_Attachment] with nocheck add constraint PK_yaf_Attachment primary key clustered (AttachmentID) 
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Active]') and name='PK_yaf_Active')
	alter table [{databaseOwner}].[yaf_Active] with nocheck add constraint PK_yaf_Active primary key clustered(SessionID,BoardID)
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_PollVote]') and name='PK_yaf_PollVote')
	alter table [{databaseOwner}].[yaf_PollVote] with nocheck add constraint PK_yaf_PollVote primary key clustered(PollVoteID)
GO

/*
** Unique constraints
*/

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and name='IX_yaf_CheckEmail')
	alter table [{databaseOwner}].[yaf_CheckEmail] add constraint IX_yaf_CheckEmail unique nonclustered (Hash)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='IX_yaf_Forum')
	alter table [{databaseOwner}].[yaf_Forum] add constraint IX_yaf_Forum unique nonclustered (CategoryID,Name)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and name='IX_yaf_WatchForum')
	alter table [{databaseOwner}].[yaf_WatchForum] add constraint IX_yaf_WatchForum unique nonclustered (ForumID,UserID)   
GO

if not exists(select 1 from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and name='IX_yaf_WatchTopic')
	alter table [{databaseOwner}].[yaf_WatchTopic] add constraint IX_yaf_WatchTopic unique nonclustered (TopicID,UserID)   
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Category]') and name='IX_yaf_Category')
	alter table [{databaseOwner}].[yaf_Category] add constraint IX_yaf_Category unique nonclustered(BoardID,Name)
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Rank]') and name='IX_yaf_Rank')
	alter table [{databaseOwner}].[yaf_Rank] add constraint IX_yaf_Rank unique(BoardID,Name)
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='IX_yaf_User')
	alter table [{databaseOwner}].[yaf_User] add constraint IX_yaf_User unique nonclustered(BoardID,Name)
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='IX_yaf_Group')
	alter table [{databaseOwner}].[yaf_Group] add constraint IX_yaf_Group unique nonclustered(BoardID,Name)
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and name='IX_yaf_BannedIP')
	alter table [{databaseOwner}].[yaf_BannedIP] add constraint IX_yaf_BannedIP unique nonclustered(BoardID,Mask)
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Smiley]') and name='IX_yaf_Smiley')
	alter table [{databaseOwner}].[yaf_Smiley] add constraint IX_yaf_Smiley unique nonclustered(BoardID,Code)
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and name='IX_yaf_BannedIP')
	alter table [{databaseOwner}].[yaf_BannedIP] add constraint IX_yaf_BannedIP unique nonclustered(BoardID,Mask)
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Category]') and name='IX_yaf_Category')
	alter table [{databaseOwner}].[yaf_Category] add constraint IX_yaf_Category unique nonclustered(BoardID,Name)
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and name='IX_yaf_CheckEmail')
	alter table [{databaseOwner}].[yaf_CheckEmail] add constraint IX_yaf_CheckEmail unique nonclustered(Hash)
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='IX_yaf_Forum')
	alter table [{databaseOwner}].[yaf_Forum] add constraint IX_yaf_Forum unique nonclustered(CategoryID,Name)   
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='IX_yaf_Group')
	alter table [{databaseOwner}].[yaf_Group] add constraint IX_yaf_Group unique nonclustered(BoardID,Name)
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Rank]') and name='IX_yaf_Rank')
	alter table [{databaseOwner}].[yaf_Rank] add constraint IX_yaf_Rank unique nonclustered(BoardID,Name)
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_Smiley]') and name='IX_yaf_Smiley')
	alter table [{databaseOwner}].[yaf_Smiley] add constraint IX_yaf_Smiley unique nonclustered(BoardID,Code)
GO

if not exists(select * from sysindexes where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='IX_yaf_User')
	alter table [{databaseOwner}].[yaf_User] add constraint IX_yaf_User unique nonclustered(BoardID,Name)
GO

/*
** Foreign keys
*/

if not exists(select 1 from sysobjects where name='FK_yaf_Active_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] add constraint FK_yaf_Active_yaf_Forum foreign key (ForumID) references [{databaseOwner}].[yaf_Forum] (ForumID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Active_yaf_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] add constraint FK_yaf_Active_yaf_Topic foreign key (TopicID) references [{databaseOwner}].[yaf_Topic] (TopicID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Active_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] add constraint FK_yaf_Active_yaf_User foreign key (UserID) references [{databaseOwner}].[yaf_User] (UserID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_CheckEmail_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_CheckEmail] add constraint FK_yaf_CheckEmail_yaf_User foreign key (UserID) references [{databaseOwner}].[yaf_User] (UserID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Choice_yaf_Poll' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Choice]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Choice] add constraint FK_yaf_Choice_yaf_Poll foreign key (PollID) references [{databaseOwner}].[yaf_Poll] (PollID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Forum_yaf_Category' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] add constraint FK_yaf_Forum_yaf_Category foreign key (CategoryID) references [{databaseOwner}].[yaf_Category] (CategoryID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Forum_yaf_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] add constraint FK_yaf_Forum_yaf_Message foreign key (LastMessageID) references [{databaseOwner}].[yaf_Message] (MessageID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Forum_yaf_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] add constraint FK_yaf_Forum_yaf_Topic foreign key (LastTopicID) references [{databaseOwner}].[yaf_Topic] (TopicID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Forum_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] add constraint FK_yaf_Forum_yaf_User foreign key (LastUserID) references [{databaseOwner}].[yaf_User] (UserID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_ForumAccess_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_ForumAccess] add constraint FK_yaf_ForumAccess_yaf_Forum foreign key (ForumID) references [{databaseOwner}].[yaf_Forum] (ForumID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_ForumAccess_yaf_Group' and parent_obj=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_ForumAccess] add constraint FK_yaf_ForumAccess_yaf_Group foreign key (GroupID) references [{databaseOwner}].[yaf_Group] (GroupID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Message_yaf_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Message] add constraint FK_yaf_Message_yaf_Topic foreign key (TopicID) references [{databaseOwner}].[yaf_Topic] (TopicID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Message_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Message] add constraint FK_yaf_Message_yaf_User foreign key (UserID) references [{databaseOwner}].[yaf_User] (UserID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_PMessage_yaf_User1' and parent_obj=object_id(N'[{databaseOwner}].[yaf_PMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_PMessage] add constraint FK_yaf_PMessage_yaf_User1 foreign key (FromUserID) references [{databaseOwner}].[yaf_User] (UserID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Topic_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] add constraint FK_yaf_Topic_yaf_Forum foreign key (ForumID) references [{databaseOwner}].[yaf_Forum] (ForumID) ON DELETE CASCADE
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Topic_yaf_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] add constraint FK_yaf_Topic_yaf_Message foreign key (LastMessageID) references [{databaseOwner}].[yaf_Message] (MessageID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Topic_yaf_Poll' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] add constraint FK_yaf_Topic_yaf_Poll foreign key (PollID) references [{databaseOwner}].[yaf_Poll] (PollID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Topic_yaf_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] add constraint FK_yaf_Topic_yaf_Topic foreign key (TopicMovedID) references [{databaseOwner}].[yaf_Topic] (TopicID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Topic_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] add constraint FK_yaf_Topic_yaf_User foreign key (UserID) references [{databaseOwner}].[yaf_User] (UserID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Topic_yaf_User2' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Topic] add constraint FK_yaf_Topic_yaf_User2 foreign key (LastUserID) references [{databaseOwner}].[yaf_User] (UserID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_WatchForum_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchForum] add constraint FK_yaf_WatchForum_yaf_Forum foreign key (ForumID) references [{databaseOwner}].[yaf_Forum](ForumID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_WatchForum_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchForum] add constraint FK_yaf_WatchForum_yaf_User foreign key (UserID) references [{databaseOwner}].[yaf_User](UserID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_WatchTopic_yaf_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchTopic] add constraint FK_yaf_WatchTopic_yaf_Topic foreign key (TopicID) references [{databaseOwner}].[yaf_Topic](TopicID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_WatchTopic_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_WatchTopic] add constraint FK_yaf_WatchTopic_yaf_User foreign key (UserID) references [{databaseOwner}].[yaf_User](UserID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Active_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Attachment] add constraint FK_yaf_Active_yaf_Forum foreign key (MessageID) references yaf_Message (MessageID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_UserGroup_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserGroup] add constraint FK_yaf_UserGroup_yaf_User foreign key (UserID) references yaf_User(UserID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_UserGroup_yaf_Group' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserGroup] add constraint FK_yaf_UserGroup_yaf_Group foreign key(GroupID) references yaf_Group (GroupID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Attachment_yaf_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Attachment]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Attachment] add constraint FK_yaf_Attachment_yaf_Message foreign key (MessageID) references yaf_Message (MessageID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_NntpForum_yaf_NntpServer' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpForum] add constraint FK_yaf_NntpForum_yaf_NntpServer foreign key (NntpServerID) references yaf_NntpServer(NntpServerID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_NntpForum_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpForum] add constraint FK_yaf_NntpForum_yaf_Forum foreign key (ForumID) references yaf_Forum(ForumID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_NntpTopic_yaf_NntpForum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpTopic] add constraint FK_yaf_NntpTopic_yaf_NntpForum foreign key (NntpForumID) references yaf_NntpForum(NntpForumID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_NntpTopic_yaf_Topic' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpTopic] add constraint FK_yaf_NntpTopic_yaf_Topic foreign key (TopicID) references yaf_Topic(TopicID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_ForumAccess_yaf_AccessMask' and parent_obj=object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_ForumAccess] add constraint FK_yaf_ForumAccess_yaf_AccessMask foreign key (AccessMaskID) references yaf_AccessMask (AccessMaskID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_UserForum_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserForum] add constraint FK_yaf_UserForum_yaf_User foreign key (UserID) references yaf_User (UserID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_UserForum_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserForum] add constraint FK_yaf_UserForum_yaf_Forum foreign key (ForumID) references yaf_Forum (ForumID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_UserForum_yaf_AccessMask' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserForum] add constraint FK_yaf_UserForum_yaf_AccessMask foreign key (AccessMaskID) references yaf_AccessMask (AccessMaskID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Category_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Category]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Category] add constraint FK_yaf_Category_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_AccessMask_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_AccessMask] add constraint FK_yaf_AccessMask_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Active_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Active] add constraint FK_yaf_Active_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_BannedIP_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_BannedIP]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_BannedIP] add constraint FK_yaf_BannedIP_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Group_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Group]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Group] add constraint FK_yaf_Group_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_NntpServer_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_NntpServer]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_NntpServer] add constraint FK_yaf_NntpServer_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Rank_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Rank]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Rank] add constraint FK_yaf_Rank_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Smiley_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Smiley]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Smiley] add constraint FK_yaf_Smiley_yaf_Board foreign key(BoardID) references yaf_Board (BoardID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_User_yaf_Rank' and parent_obj=object_id(N'[{databaseOwner}].[yaf_User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_User] add constraint FK_yaf_User_yaf_Rank foreign key(RankID) references yaf_Rank(RankID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_User_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_User] add constraint FK_yaf_User_yaf_Board foreign key(BoardID) references yaf_Board(BoardID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Forum_yaf_Forum' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Forum] add constraint FK_yaf_Forum_yaf_Forum foreign key(ParentID) references yaf_Forum(ForumID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Message_yaf_Message' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Message] add constraint FK_yaf_Message_yaf_Message foreign key(ReplyTo) references yaf_Message(MessageID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_UserPMessage_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserPMessage] add constraint FK_yaf_UserPMessage_yaf_User foreign key (UserID) references yaf_User (UserID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_UserPMessage_yaf_PMessage' and parent_obj=object_id(N'[{databaseOwner}].[yaf_UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_UserPMessage] add constraint FK_yaf_UserPMessage_yaf_PMessage foreign key (PMessageID) references yaf_PMessage (PMessageID)
GO

if not exists(select 1 from sysobjects where name='FK_yaf_Registry_yaf_Board' and parent_obj=object_id(N'[{databaseOwner}].[yaf_Registry]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_Registry] add constraint FK_yaf_Registry_yaf_Board foreign key(BoardID) references yaf_Board(BoardID) on delete cascade
go

if not exists(select 1 from sysobjects where name='FK_yaf_PollVote_yaf_Poll' and parent_obj=object_id(N'[{databaseOwner}].[yaf_PollVote]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_PollVote] add constraint FK_yaf_PollVote_yaf_Poll foreign key(PollID) references yaf_Poll(PollID) on delete cascade
go

if not exists(select 1 from sysobjects where name='FK_yaf_EventLog_yaf_User' and parent_obj=object_id(N'[{databaseOwner}].[yaf_EventLog]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[yaf_EventLog] add constraint FK_yaf_EventLog_yaf_User foreign key(UserID) references [{databaseOwner}].[yaf_User](UserID) on delete cascade
go

/* Default Constraints */

if exists(select 1 from sysobjects where name=N'DF_yaf_Message_Flags' and parent_obj=object_id(N'yaf_Message'))
	alter table [{databaseOwner}].[yaf_Message] drop constraint DF_yaf_Message_Flags
GO

if not exists(select 1 from sysobjects where name=N'DF_yaf_Message_Flags' and parent_obj=object_id(N'yaf_Message'))
	alter table [{databaseOwner}].[yaf_Message] add constraint DF_yaf_Message_Flags default (23) for Flags
GO

if exists(select 1 from sysobjects where name=N'DF_EventLog_EventTime' and parent_obj=object_id(N'yaf_EventLog'))
	alter table [{databaseOwner}].[yaf_EventLog] drop constraint DF_EventLog_EventTime
GO

if not exists(select 1 from sysobjects where name=N'DF_yaf_EventLog_EventTime' and parent_obj=object_id(N'yaf_EventLog'))
	alter table [{databaseOwner}].[yaf_EventLog] add constraint DF_yaf_EventLog_EventTime default(getdate()) for EventTime
GO

if exists(select 1 from sysobjects where name=N'DF_EventLog_Type' and parent_obj=object_id(N'yaf_EventLog'))
	alter table [{databaseOwner}].[yaf_EventLog] drop constraint DF_EventLog_Type
GO

if not exists(select 1 from sysobjects where name=N'DF_yaf_EventLog_Type' and parent_obj=object_id(N'yaf_EventLog'))
	alter table [{databaseOwner}].[yaf_EventLog] add constraint DF_yaf_EventLog_Type default(0) for Type
GO
