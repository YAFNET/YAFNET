/*
** Drop old Foreign keys
*/


if exists (select top 1 1 from  dbo.sysobjects where name='FK_Active_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] drop constraint [FK_Active_Forum]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Active_Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] drop constraint [FK_Active_Topic]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Active_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] drop constraint [FK_Active_User]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_CheckEmail_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] drop constraint [FK_CheckEmail_User]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Choice_Poll' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Choice]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Choice] drop constraint [FK_Choice_Poll]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Forum_Category' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint [FK_Forum_Category]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Forum_Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint [FK_Forum_Message]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Forum_Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint [FK_Forum_Topic]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Forum_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint [FK_Forum_User]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_ForumAccess_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] drop constraint [FK_ForumAccess_Forum]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_ForumAccess_Group' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] drop constraint [FK_ForumAccess_Group]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Message_Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Message] drop constraint [FK_Message_Topic]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Message_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Message] drop constraint [FK_Message_User]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_PMessage_User1' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}PMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}PMessage] drop constraint [FK_PMessage_User1]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Topic_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_Topic_Forum]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Topic_Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_Topic_Message]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Topic_Poll' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_Topic_Poll]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Topic_Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_Topic_Topic]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Topic_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_Topic_User]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Topic_User2' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_Topic_User2]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_WatchForum_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] drop constraint [FK_WatchForum_Forum]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_WatchForum_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] drop constraint [FK_WatchForum_User]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_WatchTopic_Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] drop constraint [FK_WatchTopic_Topic]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_WatchTopic_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] drop constraint [FK_WatchTopic_User]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Active_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Attachment] drop constraint [FK_Attachment_Message]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_UserGroup_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserGroup] drop constraint [FK_UserGroup_User]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_UserGroup_Group' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserGroup] drop constraint [FK_UserGroup_Group]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_Attachment_Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Attachment]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Attachment] drop constraint [FK_Attachment_Message]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_NntpForum_NntpServer' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpForum] drop constraint [FK_NntpForum_NntpServer]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_NntpForum_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpForum] drop constraint [FK_NntpForum_Forum]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_NntpTopic_NntpForum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpTopic] drop constraint [FK_NntpTopic_NntpForum]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_NntpTopic_Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpTopic] drop constraint [FK_NntpTopic_Topic]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_ForumAccess_AccessMask' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] drop constraint [FK_ForumAccess_AccessMask]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_UserForum_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserForum] drop constraint [FK_UserForum_User]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_UserForum_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserForum] drop constraint [FK_UserForum_Forum]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_UserForum_AccessMask' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserForum] drop constraint [FK_UserForum_AccessMask]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_Category_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Category]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Category] drop constraint [FK_Category_Board]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_AccessMask_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop constraint [FK_AccessMask_Board]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_Active_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] drop constraint [FK_Active_Board]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_BannedIP_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] drop constraint [FK_BannedIP_Board]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_Group_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Group]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Group] drop constraint [FK_Group_Board]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_NntpServer_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpServer]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpServer] drop constraint [FK_NntpServer_Board]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_Rank_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Rank] drop constraint [FK_Rank_Board]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_Smiley_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Smiley] drop constraint [FK_Smiley_Board]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_User_Rank' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}User] drop constraint [FK_User_Rank]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_User_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}User] drop constraint [FK_User_Board]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_Forum_Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint [FK_Forum_Forum]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_Message_Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Message] drop constraint [FK_Message_Message]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_UserPMessage_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserPMessage] drop constraint [FK_UserPMessage_User]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_UserPMessage_PMessage' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserPMessage] drop constraint [FK_UserPMessage_PMessage]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_Registry_Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Registry]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Registry] drop constraint [FK_Registry_Board]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_EventLog_User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}EventLog]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}EventLog] drop constraint [FK_EventLog_User]
go


if exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}PollVote_{objectQualifier}Poll' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}PollVote]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}PollVote] drop constraint [FK_{objectQualifier}PollVote_{objectQualifier}Poll]
go

if exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Topic_{objectQualifier}Poll' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_{objectQualifier}Topic_{objectQualifier}Poll] 
go 

/* Drop old primary keys */

if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and name='PK_BannedIP')
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] drop constraint [PK_BannedIP]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Category]') and name='PK_Category')
	alter table [{databaseOwner}].[{objectQualifier}Category] drop constraint [PK_Category]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and name='PK_CheckEmail')
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] drop constraint [PK_CheckEmail]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Choice]') and name='PK_Choice')
	alter table [{databaseOwner}].[{objectQualifier}Choice] drop constraint [PK_Choice]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='PK_Forum')
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint [PK_Forum]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and name='PK_ForumAccess')
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] drop constraint [PK_ForumAccess]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='PK_Group')
	alter table [{databaseOwner}].[{objectQualifier}Group] drop constraint [PK_Group]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Mail]') and name='PK_Mail')
	alter table [{databaseOwner}].[{objectQualifier}Mail] drop constraint [PK_Mail]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='PK_Message')
	alter table [{databaseOwner}].[{objectQualifier}Message] drop constraint [PK_Message]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}PMessage]') and name='PK_PMessage')
	alter table [{databaseOwner}].[{objectQualifier}PMessage] drop constraint [PK_PMessage]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name='PK_Poll')
	alter table [{databaseOwner}].[{objectQualifier}Poll] drop constraint [PK_Poll]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and name='PK_Smiley')
	alter table [{databaseOwner}].[{objectQualifier}Smiley] drop constraint [PK_Smiley]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='PK_Topic')
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [PK_Topic]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='PK_User')
	alter table [{databaseOwner}].[{objectQualifier}User] drop constraint [PK_User]
go

if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and name='PK_WatchForum')
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] drop constraint [PK_WatchForum]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and name='PK_WatchTopic')
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] drop constraint [PK_WatchTopic]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}UserGroup]') and name='PK_UserGroup')
	alter table [{databaseOwner}].[{objectQualifier}UserGroup] drop constraint [PK_UserGroup]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='PK_Rank')
	alter table [{databaseOwner}].[{objectQualifier}Rank] drop constraint [PK_Rank]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}NntpServer]') and name='PK_NntpServer')
	alter table [{databaseOwner}].[{objectQualifier}NntpServer] drop constraint [PK_NntpServer]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}NntpForum]') and name='PK_NntpForum')
	alter table [{databaseOwner}].[{objectQualifier}NntpForum] drop constraint [PK_NntpForum]
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}NntpTopic]') and name='PK_NntpTopic')
	alter table [{databaseOwner}].[{objectQualifier}NntpTopic] drop constraint [PK_NntpTopic]
go


if exists(select top 1 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='PK_AccessMask')
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop constraint [PK_AccessMask]
go


if exists(select top 1 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and name='PK_UserForum')
	alter table [{databaseOwner}].[{objectQualifier}UserForum] drop constraint [PK_UserForum]
go


if exists(select top 1 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Board]') and name='PK_Board')
	alter table [{databaseOwner}].[{objectQualifier}Board] drop constraint [PK_Board]
go


if exists(select top 1 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Active]') and name='PK_Active')
	alter table [{databaseOwner}].[{objectQualifier}Active] drop constraint [PK_Active]
go


if exists(select top 1 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and name='PK_UserPMessage')
	alter table [{databaseOwner}].[{objectQualifier}UserPMessage] drop constraint [PK_UserPMessage]
go


if exists(select top 1 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Attachment]') and name='PK_Attachment')
	alter table [{databaseOwner}].[{objectQualifier}Attachment] drop constraint [PK_Attachment]
go


if exists(select top 1 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Active]') and name='PK_Active')
	alter table [{databaseOwner}].[{objectQualifier}Active] drop constraint [PK_Active]
go


if exists(select top 1 1 from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}PollVote]') and name='PK_PollVote')
	alter table [{databaseOwner}].[{objectQualifier}PollVote] drop constraint [PK_PollVote]
go

/*
** Unique constraints
*/


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and name='IX_CheckEmail')
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] drop constraint IX_CheckEmail
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IX_Forum')
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint IX_Forum
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and name='IX_WatchForum')
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] drop constraint IX_WatchForum
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and name='IX_WatchTopic')
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] drop constraint IX_WatchTopic
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Category]') and name='IX_Category')
	alter table [{databaseOwner}].[{objectQualifier}Category] drop constraint IX_Category
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='IX_Rank')
	alter table [{databaseOwner}].[{objectQualifier}Rank] drop constraint IX_Rank
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IX_User')
	alter table [{databaseOwner}].[{objectQualifier}User] drop constraint IX_User
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IX_Group')
	alter table [{databaseOwner}].[{objectQualifier}Group] drop constraint IX_Group
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and name='IX_BannedIP')
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] drop constraint IX_BannedIP
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and name='IX_Smiley')
	alter table [{databaseOwner}].[{objectQualifier}Smiley] drop constraint IX_Smiley
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and name='IX_BannedIP')
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] drop constraint IX_BannedIP
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Category]') and name='IX_Category')
	alter table [{databaseOwner}].[{objectQualifier}Category] drop constraint IX_Category
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and name='IX_CheckEmail')
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] drop constraint IX_CheckEmail
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IX_Forum')
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint IX_Forum
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IX_Group')
	alter table [{databaseOwner}].[{objectQualifier}Group] drop constraint IX_Group
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='IX_Rank')
	alter table [{databaseOwner}].[{objectQualifier}Rank] drop constraint IX_Rank
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and name='IX_Smiley')
	alter table [{databaseOwner}].[{objectQualifier}Smiley] drop constraint IX_Smiley
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IX_User')
	alter table [{databaseOwner}].[{objectQualifier}User] drop constraint IX_User
go


if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Thanks]') and name='IX_{objectQualifier}Thanks')
	alter table [{databaseOwner}].[{objectQualifier}Thanks] drop constraint [IX_{objectQualifier}Thanks]
go

/* Wrong index */
/* Modified by Herman_Herman for SQL2K Compatibility */

if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Thanks]') and name='IX_{objectQualifier}Thanks_UserID')
    alter table [{databaseOwner}].[{objectQualifier}Thanks] drop constraint [IX_{objectQualifier}Thanks_UserID]
go

-- vzrus: to allow duplicate forum names
if exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IX_{objectQualifier}Forum')
    alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint IX_{objectQualifier}Forum
go


/* Build new constraints */

/*
** Primary keys
*/


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and name='PK_{objectQualifier}BannedIP')
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] with nocheck add constraint [PK_{objectQualifier}BannedIP] primary key clustered(ID)
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Buddy]') and name='PK_{objectQualifier}Buddy')
	alter table [{databaseOwner}].[{objectQualifier}Buddy] with nocheck add constraint [PK_{objectQualifier}Buddy] primary key clustered(ID)   
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Category]') and name='PK_{objectQualifier}Category')
	alter table [{databaseOwner}].[{objectQualifier}Category] with nocheck add constraint [PK_{objectQualifier}Category] primary key clustered(CategoryID)   
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and name='PK_{objectQualifier}CheckEmail')
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] with nocheck add constraint [PK_{objectQualifier}CheckEmail] primary key clustered(CheckEmailID)   
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Choice]') and name='PK_{objectQualifier}Choice')
	alter table [{databaseOwner}].[{objectQualifier}Choice] with nocheck add constraint [PK_{objectQualifier}Choice] primary key clustered(ChoiceID)   
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='PK_{objectQualifier}Forum')
	alter table [{databaseOwner}].[{objectQualifier}Forum] with nocheck add constraint [PK_{objectQualifier}Forum] primary key clustered(ForumID)   
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and name='PK_{objectQualifier}ForumAccess')
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] with nocheck add constraint [PK_{objectQualifier}ForumAccess] primary key clustered(GroupID,ForumID)   
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='PK_{objectQualifier}Group')
	alter table [{databaseOwner}].[{objectQualifier}Group] with nocheck add constraint [PK_{objectQualifier}Group] primary key clustered(GroupID)   
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Mail]') and name='PK_{objectQualifier}Mail')
	alter table [{databaseOwner}].[{objectQualifier}Mail] with nocheck add constraint [PK_{objectQualifier}Mail] primary key clustered(MailID)   
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='PK_{objectQualifier}Message')
	alter table [{databaseOwner}].[{objectQualifier}Message] with nocheck add constraint [PK_{objectQualifier}Message] primary key clustered(MessageID)   
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}PMessage]') and name='PK_{objectQualifier}PMessage')
	alter table [{databaseOwner}].[{objectQualifier}PMessage] with nocheck add constraint [PK_{objectQualifier}PMessage] primary key clustered(PMessageID)   
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}PollGroupCluster]') and name='PK_{objectQualifier}PollGroupCluster')
	alter table [{databaseOwner}].[{objectQualifier}PollGroupCluster] with nocheck add constraint [PK_{objectQualifier}PollGroupCluster] primary key clustered(PollGroupID)   
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name='PK_{objectQualifier}Poll')
	alter table [{databaseOwner}].[{objectQualifier}Poll] with nocheck add constraint [PK_{objectQualifier}Poll] primary key clustered(PollID)   
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and name='PK_{objectQualifier}Smiley')
	alter table [{databaseOwner}].[{objectQualifier}Smiley] with nocheck add constraint [PK_{objectQualifier}Smiley] primary key clustered(SmileyID)   
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='PK_{objectQualifier}Topic')
	alter table [{databaseOwner}].[{objectQualifier}Topic] with nocheck add constraint [PK_{objectQualifier}Topic] primary key clustered(TopicID)   
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}FavoriteTopic]') and name='PK_{objectQualifier}FavoriteTopic')
	alter table [{databaseOwner}].[{objectQualifier}FavoriteTopic] with nocheck add constraint [PK_{objectQualifier}FavoriteTopic] primary key clustered(ID)   
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and ( name='PK_{objectQualifier}User' OR name='PK_{objectQualifier}{objectQualifier}User' ) )
	alter table [{databaseOwner}].[{objectQualifier}User] with nocheck add constraint [PK_{objectQualifier}User] primary key clustered(UserID)   
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and name='PK_{objectQualifier}WatchForum')
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] with nocheck add constraint [PK_{objectQualifier}WatchForum] primary key clustered(WatchForumID)   
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and name='PK_{objectQualifier}WatchTopic')
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] with nocheck add constraint [PK_{objectQualifier}WatchTopic] primary key clustered(WatchTopicID)   
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}UserGroup]') and name='PK_{objectQualifier}UserGroup')
	alter table [{databaseOwner}].[{objectQualifier}UserGroup] with nocheck add constraint [PK_{objectQualifier}UserGroup] primary key clustered(UserID,GroupID)
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='PK_{objectQualifier}Rank')
	alter table [{databaseOwner}].[{objectQualifier}Rank] with nocheck add constraint [PK_{objectQualifier}Rank] primary key clustered(RankID)
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}NntpServer]') and name='PK_{objectQualifier}NntpServer')
	alter table [{databaseOwner}].[{objectQualifier}NntpServer] with nocheck add constraint [PK_{objectQualifier}NntpServer] primary key clustered (NntpServerID) 
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}NntpForum]') and name='PK_{objectQualifier}NntpForum')
	alter table [{databaseOwner}].[{objectQualifier}NntpForum] with nocheck add constraint [PK_{objectQualifier}NntpForum] primary key clustered (NntpForumID) 
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}NntpTopic]') and name='PK_{objectQualifier}NntpTopic')
	alter table [{databaseOwner}].[{objectQualifier}NntpTopic] with nocheck add constraint [PK_{objectQualifier}NntpTopic] primary key clustered (NntpTopicID) 
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='PK_{objectQualifier}AccessMask')
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] with nocheck add constraint [PK_{objectQualifier}AccessMask] primary key clustered (AccessMaskID) 
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and name='PK_{objectQualifier}UserForum')
	alter table [{databaseOwner}].[{objectQualifier}UserForum] with nocheck add constraint [PK_{objectQualifier}UserForum] primary key clustered (UserID,ForumID) 
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Board]') and name='PK_{objectQualifier}Board')
	alter table [{databaseOwner}].[{objectQualifier}Board] with nocheck add constraint [PK_{objectQualifier}Board] primary key clustered (BoardID)
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Active]') and name='PK_{objectQualifier}Active')
	alter table [{databaseOwner}].[{objectQualifier}Active] with nocheck add constraint [PK_{objectQualifier}Active] primary key clustered(SessionID,BoardID)
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and name='PK_{objectQualifier}UserPMessage')
	alter table [{databaseOwner}].[{objectQualifier}UserPMessage] with nocheck add constraint [PK_{objectQualifier}UserPMessage] primary key clustered (UserPMessageID) 
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Attachment]') and name='PK_{objectQualifier}Attachment')
	alter table [{databaseOwner}].[{objectQualifier}Attachment] with nocheck add constraint [PK_{objectQualifier}Attachment] primary key clustered (AttachmentID) 
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Active]') and name='PK_{objectQualifier}Active')
	alter table [{databaseOwner}].[{objectQualifier}Active] with nocheck add constraint [PK_{objectQualifier}Active] primary key clustered(SessionID,BoardID)
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}PollVote]') and name='PK_{objectQualifier}PollVote')
	alter table [{databaseOwner}].[{objectQualifier}PollVote] with nocheck add constraint [PK_{objectQualifier}PollVote] primary key clustered(PollVoteID)
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}IgnoreUser]') and name='PK_{objectQualifier}IgnoreUser')
	alter table [{databaseOwner}].[{objectQualifier}IgnoreUser] with nocheck add constraint [PK_{objectQualifier}IgnoreUser] PRIMARY KEY CLUSTERED (UserID, IgnoredUserID)
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}ShoutboxMessage]') and name='PK_{objectQualifier}ShoutboxMessage')
	alter table [{databaseOwner}].[{objectQualifier}ShoutboxMessage] with nocheck add constraint [PK_{objectQualifier}ShoutboxMessage] PRIMARY KEY CLUSTERED (ShoutBoxMessageID)
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Thanks]') and name='PK_{objectQualifier}Thanks')
	alter table [{databaseOwner}].[{objectQualifier}Thanks] with nocheck add constraint [PK_{objectQualifier}Thanks] PRIMARY KEY CLUSTERED (ThanksID)
go

/*
** Unique constraints
*/


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and name='IX_{objectQualifier}CheckEmail')
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] add constraint IX_{objectQualifier}CheckEmail unique nonclustered (Hash)   
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and name='IX_{objectQualifier}WatchForum')
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] add constraint IX_{objectQualifier}WatchForum unique nonclustered (ForumID,UserID)   
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}MessageHistory]') and name='IX_{objectQualifier}MessageHistory')
	alter table [{databaseOwner}].[{objectQualifier}MessageHistory] add constraint IX_{objectQualifier}MessageHistory unique nonclustered (Edited,MessageID)   
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}UserProfile]') and name='IX_{objectQualifier}UserProfile')
	alter table [{databaseOwner}].[{objectQualifier}UserProfile] add constraint IX_{objectQualifier}UserProfile unique nonclustered (UserID,ApplicationName)   
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}ForumReadTracking]') and name='IX_{objectQualifier}ForumReadTracking')
	alter table [{databaseOwner}].[{objectQualifier}ForumReadTracking] add constraint IX_{objectQualifier}ForumReadTracking unique nonclustered (UserID,ForumID)   
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}TopicReadTracking]') and name='IX_{objectQualifier}TopicReadTracking')
	alter table [{databaseOwner}].[{objectQualifier}TopicReadTracking] add constraint IX_{objectQualifier}TopicReadTracking unique nonclustered (UserID,TopicID)   
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and name='IX_{objectQualifier}WatchTopic')
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] add constraint IX_{objectQualifier}WatchTopic unique nonclustered (TopicID,UserID)   
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Category]') and name='IX_{objectQualifier}Category')
	alter table [{databaseOwner}].[{objectQualifier}Category] add constraint IX_{objectQualifier}Category unique nonclustered(BoardID,Name)
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='IX_{objectQualifier}Rank')
	alter table [{databaseOwner}].[{objectQualifier}Rank] add constraint IX_{objectQualifier}Rank unique(BoardID,Name)
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IX_{objectQualifier}User')
	alter table [{databaseOwner}].[{objectQualifier}User] add constraint IX_{objectQualifier}User unique nonclustered(BoardID,Name)
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IX_{objectQualifier}Group')
	alter table [{databaseOwner}].[{objectQualifier}Group] add constraint IX_{objectQualifier}Group unique nonclustered(BoardID,Name)
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and name='IX_{objectQualifier}BannedIP')
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] add constraint IX_{objectQualifier}BannedIP unique nonclustered(BoardID,Mask)
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and name='IX_{objectQualifier}Smiley')
	alter table [{databaseOwner}].[{objectQualifier}Smiley] add constraint IX_{objectQualifier}Smiley unique nonclustered(BoardID,Code)
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and name='IX_{objectQualifier}BannedIP')
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] add constraint IX_{objectQualifier}BannedIP unique nonclustered(BoardID,Mask)
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Category]') and name='IX_{objectQualifier}Category')
	alter table [{databaseOwner}].[{objectQualifier}Category] add constraint IX_{objectQualifier}Category unique nonclustered(BoardID,Name)
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and name='IX_{objectQualifier}CheckEmail')
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] add constraint IX_{objectQualifier}CheckEmail unique nonclustered(Hash)
go


/* if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IX_{objectQualifier}Forum')
	alter table [{databaseOwner}].[{objectQualifier}Forum] add constraint IX_{objectQualifier}Forum unique nonclustered(CategoryID,Name)   
*/


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IX_{objectQualifier}Group')
	alter table [{databaseOwner}].[{objectQualifier}Group] add constraint IX_{objectQualifier}Group unique nonclustered(BoardID,Name)
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='IX_{objectQualifier}Rank')
	alter table [{databaseOwner}].[{objectQualifier}Rank] add constraint IX_{objectQualifier}Rank unique nonclustered(BoardID,Name)
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and name='IX_{objectQualifier}Smiley')
	alter table [{databaseOwner}].[{objectQualifier}Smiley] add constraint IX_{objectQualifier}Smiley unique nonclustered(BoardID,Code)
go


if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IX_{objectQualifier}User')
	alter table [{databaseOwner}].[{objectQualifier}User] add constraint IX_{objectQualifier}User unique nonclustered(BoardID,Name)
go

if not exists (select top 1 1 from  dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Buddy]') and name='IX_{objectQualifier}Buddy')
	alter table [{databaseOwner}].[{objectQualifier}Buddy] add constraint IX_{objectQualifier}Buddy unique nonclustered([FromUserID],[ToUserID])
go


/*
** Foreign keys
*/


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Active_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] add constraint [FK_{objectQualifier}Active_{objectQualifier}Forum] foreign key (ForumID) references [{databaseOwner}].[{objectQualifier}Forum] (ForumID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Active_{objectQualifier}Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] add constraint [FK_{objectQualifier}Active_{objectQualifier}Topic] foreign key (TopicID) references [{databaseOwner}].[{objectQualifier}Topic] (TopicID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Active_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] add constraint [FK_{objectQualifier}Active_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User] (UserID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}CheckEmail_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}CheckEmail]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] add constraint [FK_{objectQualifier}CheckEmail_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User] (UserID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Choice_{objectQualifier}Poll' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Choice]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Choice] add constraint [FK_{objectQualifier}Choice_{objectQualifier}Poll] foreign key (PollID) references [{databaseOwner}].[{objectQualifier}Poll] (PollID)
go

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}FavoriteTopic_{objectQualifier}Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}FavoriteTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}FavoriteTopic] add constraint [FK_{objectQualifier}FavoriteTopic_{objectQualifier}Topic] foreign key (TopicID) references [{databaseOwner}].[{objectQualifier}Topic] (TopicID)
go

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}FavoriteTopic_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}FavoriteTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}FavoriteTopic] add constraint [FK_{objectQualifier}FavoriteTopic_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User] (UserID)
go

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}UserProfile_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserProfile]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserProfile] add constraint [FK_{objectQualifier}UserProfile_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User] (UserID) on delete cascade
go

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Forum_{objectQualifier}Category' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] add constraint [FK_{objectQualifier}Forum_{objectQualifier}Category] foreign key (CategoryID) references [{databaseOwner}].[{objectQualifier}Category] (CategoryID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Forum_{objectQualifier}Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] add constraint [FK_{objectQualifier}Forum_{objectQualifier}Message] foreign key (LastMessageID) references [{databaseOwner}].[{objectQualifier}Message] (MessageID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Forum_{objectQualifier}Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] add constraint [FK_{objectQualifier}Forum_{objectQualifier}Topic] foreign key (LastTopicID) references [{databaseOwner}].[{objectQualifier}Topic] (TopicID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Forum_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] add constraint [FK_{objectQualifier}Forum_{objectQualifier}User] foreign key (LastUserID) references [{databaseOwner}].[{objectQualifier}User] (UserID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}ForumAccess_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] add constraint [FK_{objectQualifier}ForumAccess_{objectQualifier}Forum] foreign key (ForumID) references [{databaseOwner}].[{objectQualifier}Forum] (ForumID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}ForumAccess_{objectQualifier}Group' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] add constraint [FK_{objectQualifier}ForumAccess_{objectQualifier}Group] foreign key (GroupID) references [{databaseOwner}].[{objectQualifier}Group] (GroupID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Message_{objectQualifier}Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Message] add constraint [FK_{objectQualifier}Message_{objectQualifier}Topic] foreign key (TopicID) references [{databaseOwner}].[{objectQualifier}Topic] (TopicID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Message_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Message] add constraint [FK_{objectQualifier}Message_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User] (UserID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}PMessage_{objectQualifier}User1' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}PMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}PMessage] add constraint [FK_{objectQualifier}PMessage_{objectQualifier}User1] foreign key (FromUserID) references [{databaseOwner}].[{objectQualifier}User] (UserID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Topic_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] add constraint [FK_{objectQualifier}Topic_{objectQualifier}Forum] foreign key (ForumID) references [{databaseOwner}].[{objectQualifier}Forum] (ForumID) ON DELETE CASCADE
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Topic_{objectQualifier}Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] add constraint [FK_{objectQualifier}Topic_{objectQualifier}Message] foreign key (LastMessageID) references [{databaseOwner}].[{objectQualifier}Message] (MessageID)
go

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Topic_{objectQualifier}Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] add constraint [FK_{objectQualifier}Topic_{objectQualifier}Topic] foreign key (TopicMovedID) references [{databaseOwner}].[{objectQualifier}Topic] (TopicID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Topic_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] add constraint [FK_{objectQualifier}Topic_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User] (UserID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Topic_{objectQualifier}User2' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] add constraint [FK_{objectQualifier}Topic_{objectQualifier}User2] foreign key (LastUserID) references [{databaseOwner}].[{objectQualifier}User] (UserID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}WatchForum_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] add constraint [FK_{objectQualifier}WatchForum_{objectQualifier}Forum] foreign key (ForumID) references [{databaseOwner}].[{objectQualifier}Forum](ForumID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}WatchForum_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchForum] add constraint [FK_{objectQualifier}WatchForum_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User](UserID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}WatchTopic_{objectQualifier}Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] add constraint [FK_{objectQualifier}WatchTopic_{objectQualifier}Topic] foreign key (TopicID) references [{databaseOwner}].[{objectQualifier}Topic](TopicID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}WatchTopic_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}WatchTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}WatchTopic] add constraint [FK_{objectQualifier}WatchTopic_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User](UserID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Active_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Attachment] add constraint [FK_{objectQualifier}Active_{objectQualifier}Forum] foreign key (MessageID) references [{databaseOwner}].[{objectQualifier}Message] (MessageID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}UserGroup_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserGroup] add constraint [FK_{objectQualifier}UserGroup_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User](UserID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}UserGroup_{objectQualifier}Group' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserGroup]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserGroup] add constraint [FK_{objectQualifier}UserGroup_{objectQualifier}Group] foreign key(GroupID) references [{databaseOwner}].[{objectQualifier}Group] (GroupID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Attachment_{objectQualifier}Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Attachment]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Attachment] add constraint [FK_{objectQualifier}Attachment_{objectQualifier}Message] foreign key (MessageID) references [{databaseOwner}].[{objectQualifier}Message] (MessageID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}NntpForum_{objectQualifier}NntpServer' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpForum] add constraint [FK_{objectQualifier}NntpForum_{objectQualifier}NntpServer] foreign key (NntpServerID) references [{databaseOwner}].[{objectQualifier}NntpServer](NntpServerID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}NntpForum_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpForum] add constraint [FK_{objectQualifier}NntpForum_{objectQualifier}Forum] foreign key (ForumID) references [{databaseOwner}].[{objectQualifier}Forum](ForumID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}NntpTopic_{objectQualifier}NntpForum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpTopic] add constraint [FK_{objectQualifier}NntpTopic_{objectQualifier}NntpForum] foreign key (NntpForumID) references [{databaseOwner}].[{objectQualifier}NntpForum](NntpForumID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}NntpTopic_{objectQualifier}Topic' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpTopic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpTopic] add constraint [FK_{objectQualifier}NntpTopic_{objectQualifier}Topic] foreign key (TopicID) references [{databaseOwner}].[{objectQualifier}Topic](TopicID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}ForumAccess_{objectQualifier}AccessMask' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}ForumAccess]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}ForumAccess] add constraint [FK_{objectQualifier}ForumAccess_{objectQualifier}AccessMask] foreign key (AccessMaskID) references [{databaseOwner}].[{objectQualifier}AccessMask] (AccessMaskID)
go

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}UserAlbumImage_{objectQualifier}UserAlbum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserAlbumImage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserAlbumImage] add constraint [FK_{objectQualifier}UserAlbumImage_{objectQualifier}UserAlbum] foreign key (AlbumID) references [{databaseOwner}].[{objectQualifier}UserAlbum] (AlbumID)
go

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}UserForum_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserForum] add constraint [FK_{objectQualifier}UserForum_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User] (UserID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}UserForum_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserForum] add constraint [FK_{objectQualifier}UserForum_{objectQualifier}Forum] foreign key (ForumID) references [{databaseOwner}].[{objectQualifier}Forum] (ForumID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}UserForum_{objectQualifier}AccessMask' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserForum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserForum] add constraint [FK_{objectQualifier}UserForum_{objectQualifier}AccessMask] foreign key (AccessMaskID) references [{databaseOwner}].[{objectQualifier}AccessMask] (AccessMaskID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Category_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Category]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Category] add constraint [FK_{objectQualifier}Category_{objectQualifier}Board] foreign key(BoardID) references [{databaseOwner}].[{objectQualifier}Board] (BoardID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}AccessMask_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] add constraint [FK_{objectQualifier}AccessMask_{objectQualifier}Board] foreign key(BoardID) references [{databaseOwner}].[{objectQualifier}Board] (BoardID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Active_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Active] add constraint [FK_{objectQualifier}Active_{objectQualifier}Board] foreign key(BoardID) references [{databaseOwner}].[{objectQualifier}Board] (BoardID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}BannedIP_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}BannedIP]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}BannedIP] add constraint [FK_{objectQualifier}BannedIP_{objectQualifier}Board] foreign key(BoardID) references [{databaseOwner}].[{objectQualifier}Board] (BoardID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Group_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Group]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Group] add constraint [FK_{objectQualifier}Group_{objectQualifier}Board] foreign key(BoardID) references [{databaseOwner}].[{objectQualifier}Board] (BoardID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}NntpServer_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}NntpServer]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}NntpServer] add constraint [FK_{objectQualifier}NntpServer_{objectQualifier}Board] foreign key(BoardID) references [{databaseOwner}].[{objectQualifier}Board] (BoardID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Rank_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Rank] add constraint [FK_{objectQualifier}Rank_{objectQualifier}Board] foreign key(BoardID) references [{databaseOwner}].[{objectQualifier}Board] (BoardID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Smiley_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Smiley] add constraint [FK_{objectQualifier}Smiley_{objectQualifier}Board] foreign key(BoardID) references [{databaseOwner}].[{objectQualifier}Board] (BoardID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}User_{objectQualifier}Rank' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}User] add constraint [FK_{objectQualifier}User_{objectQualifier}Rank] foreign key(RankID) references [{databaseOwner}].[{objectQualifier}Rank](RankID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}User_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}User]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}User] add constraint [FK_{objectQualifier}User_{objectQualifier}Board] foreign key(BoardID) references [{databaseOwner}].[{objectQualifier}Board](BoardID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Forum_{objectQualifier}Forum' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] add constraint [FK_{objectQualifier}Forum_{objectQualifier}Forum] foreign key(ParentID) references [{databaseOwner}].[{objectQualifier}Forum](ForumID)
go


if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Message_{objectQualifier}Message' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Message]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Message] add constraint [FK_{objectQualifier}Message_{objectQualifier}Message] foreign key(ReplyTo) references [{databaseOwner}].[{objectQualifier}Message](MessageID)
go

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}UserPMessage_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserPMessage] add constraint [FK_{objectQualifier}UserPMessage_{objectQualifier}User] foreign key (UserID) references [{databaseOwner}].[{objectQualifier}User] (UserID)
go

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}UserPMessage_{objectQualifier}PMessage' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}UserPMessage] add constraint [FK_{objectQualifier}UserPMessage_{objectQualifier}PMessage] foreign key (PMessageID) references [{databaseOwner}].[{objectQualifier}PMessage] (PMessageID)
go

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Registry_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Registry]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Registry] add constraint [FK_{objectQualifier}Registry_{objectQualifier}Board] foreign key(BoardID) references [{databaseOwner}].[{objectQualifier}Board](BoardID) on delete cascade
go

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}PollVote_{objectQualifier}Poll' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}PollVote]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}PollVote] add constraint [FK_{objectQualifier}PollVote_{objectQualifier}Poll] foreign key(PollID) references [{databaseOwner}].[{objectQualifier}Poll](PollID) on delete cascade
go

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Poll_{objectQualifier}PollGroupCluster' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Poll] add constraint [FK_{objectQualifier}Poll_{objectQualifier}PollGroupCluster] foreign key(PollGroupID) references [{databaseOwner}].[{objectQualifier}PollGroupCluster](PollGroupID)  on delete cascade 
go

 if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Topic_{objectQualifier}PollGroupCluster' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] add constraint [FK_{objectQualifier}Topic_{objectQualifier}PollGroupCluster] foreign key(PollID) references [{databaseOwner}].[{objectQualifier}PollGroupCluster](PollGroupID)  

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Forum_{objectQualifier}PollGroupCluster' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Forum] add constraint [FK_{objectQualifier}Forum_{objectQualifier}PollGroupCluster] foreign key(PollGroupID) references [{databaseOwner}].[{objectQualifier}PollGroupCluster](PollGroupID)  

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Category_{objectQualifier}PollGroupCluster' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Category]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Category] add constraint [FK_{objectQualifier}Category_{objectQualifier}PollGroupCluster] foreign key(PollGroupID) references [{databaseOwner}].[{objectQualifier}PollGroupCluster](PollGroupID)  

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}EventLog_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}EventLog]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}EventLog] add constraint [FK_{objectQualifier}EventLog_{objectQualifier}User] foreign key(UserID) references [{databaseOwner}].[{objectQualifier}User](UserID) on delete cascade
go

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Extension_{objectQualifier}Board' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Extension]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Extension] add constraint [FK_{objectQualifier}Extension_{objectQualifier}Board] foreign key(BoardID) references [{databaseOwner}].[{objectQualifier}Board](BoardID) on delete cascade
go

if not exists (select top 1 1 from  dbo.sysobjects where name=N'FK_{objectQualifier}BBCode_Board' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}BBCode]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
    ALTER TABLE [{databaseOwner}].[{objectQualifier}BBCode] ADD CONSTRAINT [FK_{objectQualifier}BBCode_Board] FOREIGN KEY([BoardID]) REFERENCES [{databaseOwner}].[{objectQualifier}Board] ([BoardID]) ON DELETE NO ACTION
GO

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Buddy_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Buddy]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Buddy] add constraint [FK_{objectQualifier}Buddy_{objectQualifier}User] foreign key(FromUserID) references [{databaseOwner}].[{objectQualifier}User] (UserID)
go

if not exists (select top 1 1 from  dbo.sysobjects where name='FK_{objectQualifier}Thanks_{objectQualifier}User' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Thanks]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Thanks] add constraint [FK_{objectQualifier}Thanks_{objectQualifier}User] foreign key (ThanksFromUserID) references [{databaseOwner}].[{objectQualifier}User](UserID)
go

if not exists (select top 1 1 from  dbo.sysobjects where name=N'FK_{objectQualifier}MessageHistory_MessageID' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}MessageHistory]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
    ALTER TABLE [{databaseOwner}].[{objectQualifier}MessageHistory] ADD CONSTRAINT [FK_{objectQualifier}MessageHistory_MessageID] FOREIGN KEY([MessageID]) REFERENCES [{databaseOwner}].[{objectQualifier}Message] ([MessageID]) 
	ON DELETE NO ACTION 

/* Default Constraints */
if exists (select top 1 1 from  dbo.sysobjects where name=N'DF_{objectQualifier}Message_Flags' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}Message]'))
	alter table [{databaseOwner}].[{objectQualifier}Message] drop constraint [DF_{objectQualifier}Message_Flags]
go

if not exists (select top 1 1 from  dbo.sysobjects where name=N'DF_{objectQualifier}Message_Flags' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}Message]'))
    alter table [{databaseOwner}].[{objectQualifier}Message] add constraint [DF_{objectQualifier}Message_Flags] default (23) for Flags
go

if not exists (select top 1 1 from  dbo.sysobjects where name=N'DF_{objectQualifier}Rank_PMLimit' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}Rank]'))
	alter table [{databaseOwner}].[{objectQualifier}Rank] add constraint [DF_{objectQualifier}Rank_PMLimit] default (0) for PMLimit
go

if not exists (select top 1 1 from  dbo.sysobjects where name=N'DF_{objectQualifier}Group_PMLimit' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}Group]'))
	alter table [{databaseOwner}].[{objectQualifier}Group] add constraint [DF_{objectQualifier}Group_PMLimit] default (30) for PMLimit
go

if not exists (select top 1 1 from  dbo.sysobjects where name=N'DF_{objectQualifier}User_PMNotification' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}User]'))
	alter table [{databaseOwner}].[{objectQualifier}User] add constraint [DF_{objectQualifier}User_PMNotification] default (1) for PMNotification
go

if exists (select top 1 1 from  dbo.sysobjects where name=N'DF_EventLog_EventTime' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]'))
	alter table [{databaseOwner}].[{objectQualifier}EventLog] drop constraint [DF_EventLog_EventTime]
go

if exists (select top 1 1 from  dbo.sysobjects where name=N'DF_{objectQualifier}EventLog_EventTime' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]'))
	alter table [{databaseOwner}].[{objectQualifier}EventLog] drop constraint [DF_{objectQualifier}EventLog_EventTime]
go

if not exists (select top 1 1 from  dbo.sysobjects where name=N'DF_{objectQualifier}EventLog_EventTime' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]'))
	alter table [{databaseOwner}].[{objectQualifier}EventLog] add constraint [DF_{objectQualifier}EventLog_EventTime] default(GETUTCDATE() ) for EventTime
go


if exists (select top 1 1 from  dbo.sysobjects where name=N'DF_EventLog_Type' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]'))
	alter table [{databaseOwner}].[{objectQualifier}EventLog] drop constraint [DF_EventLog_Type]
go

exec('[{databaseOwner}].[{objectQualifier}drop_defaultconstraint_oncolumn] {objectQualifier}ActiveAccess, IsGuestX')
GO

if not exists (select top 1 1 from  dbo.sysobjects where name=N'DF_{objectQualifier}ActiveAccess_IsGuestX' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}ActiveAccess]'))
	alter table [{databaseOwner}].[{objectQualifier}ActiveAccess] add constraint [DF_{objectQualifier}ActiveAccess_IsGuestX] default(0) for IsGuestX
go

if not exists (select top 1 1 from  dbo.sysobjects where name=N'DF_{objectQualifier}EventLog_Type' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]'))
	alter table [{databaseOwner}].[{objectQualifier}EventLog] add constraint [DF_{objectQualifier}EventLog_Type] default(0) for Type
go

if not exists (select top 1 1 from  dbo.sysobjects where name=N'DF_{objectQualifier}Extension_BoardID' and parent_obj=object_id(N'[{databaseOwner}].[{objectQualifier}Extension]'))
	alter table [{databaseOwner}].[{objectQualifier}Extension] add constraint [DF_{objectQualifier}Extension_BoardID] default(1) for BoardID
go


/***** VIEWS ******/

/****** Object:  Index [{objectQualifier}vaccess_user_UserForum]    Script Date: 09/28/2009 22:30:20 ******/

/****** Object:  Index [{objectQualifier}vaccess_user_UserForum]    Script Date: 09/28/2009 22:30:20 ******/
#IFSRVVER>8#IF NOT exists (select top 1 1 from sys.indexes WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess_user]') AND name = N'{objectQualifier}vaccess_user_UserForum_PK')
SET ARITHABORT ON
CREATE UNIQUE CLUSTERED INDEX [{objectQualifier}vaccess_user_UserForum_PK] ON [{databaseOwner}].[{objectQualifier}vaccess_user] 
(
	[UserID] ASC,
	[ForumID] ASC,
	[AccessMaskID] ASC,
	[GroupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  Index [{objectQualifier}vaccess_null_UserForum]    Script Date: 09/28/2009 22:30:36 ******/
#IFSRVVER>8#IF NOT exists (select top 1 1 from sys.indexes WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess_null]') AND name = N'{objectQualifier}vaccess_null_UserForum_PK')
SET ARITHABORT ON
CREATE UNIQUE CLUSTERED INDEX [{objectQualifier}vaccess_null_UserForum_PK] ON [{databaseOwner}].[{objectQualifier}vaccess_null] 
(
	[UserID] ASC,
	[ForumID] ASC,
	[AccessMaskID] ASC,
	[GroupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  Index [{objectQualifier}vaccess_group_UserGroup]    Script Date: 09/28/2009 22:30:55 ******/
#IFSRVVER>8#IF NOT exists (select top 1 1 from sys.indexes WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess_group]') AND name = N'{objectQualifier}vaccess_group_UserForum_PK')
SET ARITHABORT ON
CREATE UNIQUE CLUSTERED INDEX [{objectQualifier}vaccess_group_UserForum_PK] ON [{databaseOwner}].[{objectQualifier}vaccess_group] 
(
	[UserID] ASC,
	[ForumID] ASC,
	[AccessMaskID] ASC,
	[GroupID] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


