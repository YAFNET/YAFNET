/*
** Indexes
*/

-- yaf_Registry

if exists(select 1 from sysindexes where name=N'IX_Name' and id=object_id(N'yaf_Registry'))
	drop index [{databaseOwner}].[yaf_Registry.IX_Name]
go

if not exists(select 1 from sysindexes where name=N'IX_yaf_Registry_Name' and id=object_id(N'yaf_Registry'))
	CREATE  INDEX [IX_yaf_Registry_Name] ON [{databaseOwner}].[yaf_Registry]([BoardID],[Name])
go

-- yaf_PollVote

if not exists(select 1 from sysindexes where name=N'IX_yaf_PollVote_RemoteIP' and id=object_id(N'yaf_PollVote'))
 CREATE  INDEX [IX_yaf_PollVote_RemoteIP] ON [{databaseOwner}].[yaf_PollVote]([RemoteIP])
GO

if not exists(select 1 from sysindexes where name=N'IX_yaf_PollVote_UserID' and id=object_id(N'yaf_PollVote'))
 CREATE  INDEX [IX_yaf_PollVote_UserID] ON [{databaseOwner}].[yaf_PollVote]([UserID])
GO

if not exists(select 1 from sysindexes where name=N'IX_yaf_PollVote_PollID' and id=object_id(N'yaf_PollVote'))
 CREATE  INDEX [IX_yaf_PollVote_PollID] ON [{databaseOwner}].[yaf_PollVote]([PollID])
GO

-- yaf_UserGroup

if not exists(select 1 from sysindexes where name=N'IX_yaf_UserGroup_UserID' and id=object_id(N'yaf_UserGroup'))
 CREATE  INDEX [IX_yaf_UserGroup_UserID] ON [{databaseOwner}].[yaf_UserGroup]([UserID])
GO

-- yaf_Message

if not exists(select 1 from sysindexes where name=N'IX_yaf_Message_TopicID' and id=object_id(N'yaf_Message'))
 CREATE  INDEX [IX_yaf_Message_TopicID] ON [{databaseOwner}].[yaf_Message]([TopicID])
GO

if not exists(select 1 from sysindexes where name=N'IX_yaf_Message_UserID' and id=object_id(N'yaf_Message'))
 CREATE  INDEX [IX_yaf_Message_UserID] ON [{databaseOwner}].[yaf_Message]([UserID])
GO

if not exists(select 1 from sysindexes where name=N'IX_yaf_Message_Flags' and id=object_id(N'yaf_Message'))
 CREATE  INDEX [IX_yaf_Message_Flags] ON [{databaseOwner}].[yaf_Message]([Flags])
GO

-- yaf_Topic

if not exists(select 1 from sysindexes where name=N'IX_yaf_Topic_ForumID' and id=object_id(N'yaf_Topic'))
 CREATE  INDEX [IX_yaf_Topic_ForumID] ON [{databaseOwner}].[yaf_Topic]([ForumID])
GO

if not exists(select 1 from sysindexes where name=N'IX_yaf_Topic_UserID' and id=object_id(N'yaf_Topic'))
 CREATE  INDEX [IX_yaf_Topic_UserID] ON [{databaseOwner}].[yaf_Topic]([UserID])
GO

if not exists(select 1 from sysindexes where name=N'IX_yaf_Topic_Flags' and id=object_id(N'yaf_Topic'))
 CREATE  INDEX [IX_yaf_Topic_Flags] ON [{databaseOwner}].[yaf_Topic]([Flags])
GO

-- yaf_Forum

if not exists(select 1 from sysindexes where name=N'IX_yaf_Forum_CategoryID' and id=object_id(N'yaf_Forum'))
 CREATE  INDEX [IX_yaf_Forum_CategoryID] ON [{databaseOwner}].[yaf_Forum]([CategoryID])
GO

if not exists(select 1 from sysindexes where name=N'IX_yaf_Forum_ParentID' and id=object_id(N'yaf_Forum'))
 CREATE  INDEX [IX_yaf_Forum_ParentID] ON [{databaseOwner}].[yaf_Forum]([ParentID])
GO

if not exists(select 1 from sysindexes where name=N'IX_yaf_Forum_Flags' and id=object_id(N'yaf_Forum'))
 CREATE  INDEX [IX_yaf_Forum_Flags] ON [{databaseOwner}].[yaf_Forum]([Flags])
GO

-- yaf_Active

if not exists(select 1 from sysindexes where name=N'IX_yaf_Active_TopicID' and id=object_id(N'yaf_Active'))
 CREATE  INDEX [IX_yaf_Active_TopicID] ON [{databaseOwner}].[yaf_Active]([TopicID])
GO

if not exists(select 1 from sysindexes where name=N'IX_yaf_Active_UserID' and id=object_id(N'yaf_Active'))
 CREATE  INDEX [IX_yaf_Active_UserID] ON [{databaseOwner}].[yaf_Active]([UserID])
GO

if not exists(select 1 from sysindexes where name=N'IX_yaf_Active_ForumID' and id=object_id(N'yaf_Active'))
 CREATE  INDEX [IX_yaf_Active_ForumID] ON [{databaseOwner}].[yaf_Active]([ForumID])
GO

-- yaf_User

if not exists(select 1 from sysindexes where name=N'IX_yaf_User_Flags' and id=object_id(N'yaf_User'))
 CREATE  INDEX [IX_yaf_User_Flags] ON [{databaseOwner}].[yaf_User]([Flags])
GO

if not exists(select 1 from sysindexes where name=N'IX_yaf_User_ProviderUserKey' and id=object_id(N'yaf_User'))
 CREATE  INDEX [IX_yaf_User_ProviderUserKey] ON [{databaseOwner}].[yaf_User]([ProviderUserKey])
GO

if not exists(select 1 from sysindexes where name=N'IX_yaf_User_Name' and id=object_id(N'yaf_User'))
 CREATE  INDEX [IX_yaf_User_Name] ON [{databaseOwner}].[yaf_User]([Name])
GO

-- yaf_ForumAccess

if not exists(select 1 from sysindexes where name=N'IX_yaf_ForumAccess_ForumID' and id=object_id(N'yaf_ForumAccess'))
 CREATE  INDEX [IX_yaf_ForumAccess_ForumID] ON [{databaseOwner}].[yaf_ForumAccess]([ForumID])
GO

-- yaf_UserPMessage

if not exists(select 1 from sysindexes where name=N'IX_yaf_UserPMessage_UserID' and id=object_id(N'yaf_UserPMessage'))
 CREATE  INDEX [IX_yaf_UserPMessage_UserID] ON [{databaseOwner}].[yaf_UserPMessage]([UserID])
GO
