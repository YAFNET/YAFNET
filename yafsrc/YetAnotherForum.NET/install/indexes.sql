/*
** Indexes
*/

-- {objectQualifier}Registry

<<<<<<< .mine
if exists(select 1 from dbo.sysindexes where name=N'IX_Name' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Registry]'))
	drop index [{databaseOwner}].[{objectQualifier}Registry].[IX_Name]
=======
if exists(select 1 from sysindexes where name=N'IX_Name' and id=object_id(N'yaf_Registry'))
	drop index [{databaseOwner}].[yaf_Registry.IX_Name]
>>>>>>> .r1490
go

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Registry_Name' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Registry]'))
	CREATE  INDEX [IX_{objectQualifier}Registry_Name] ON [{databaseOwner}].[{objectQualifier}Registry]([BoardID],[Name])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_Registry_Name' and id=object_id(N'yaf_Registry'))
	CREATE  INDEX [IX_yaf_Registry_Name] ON [{databaseOwner}].[yaf_Registry]([BoardID],[Name])
>>>>>>> .r1490
go

-- {objectQualifier}PollVote

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}PollVote_RemoteIP' and id=object_id(N'[{databaseOwner}].[{objectQualifier}PollVote]'))
 CREATE  INDEX [IX_{objectQualifier}PollVote_RemoteIP] ON [{databaseOwner}].[{objectQualifier}PollVote]([RemoteIP])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_PollVote_RemoteIP' and id=object_id(N'yaf_PollVote'))
 CREATE  INDEX [IX_yaf_PollVote_RemoteIP] ON [{databaseOwner}].[yaf_PollVote]([RemoteIP])
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}PollVote_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}PollVote]'))
 CREATE  INDEX [IX_{objectQualifier}PollVote_UserID] ON [{databaseOwner}].[{objectQualifier}PollVote]([UserID])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_PollVote_UserID' and id=object_id(N'yaf_PollVote'))
 CREATE  INDEX [IX_yaf_PollVote_UserID] ON [{databaseOwner}].[yaf_PollVote]([UserID])
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}PollVote_PollID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}PollVote]'))
 CREATE  INDEX [IX_{objectQualifier}PollVote_PollID] ON [{databaseOwner}].[{objectQualifier}PollVote]([PollID])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_PollVote_PollID' and id=object_id(N'yaf_PollVote'))
 CREATE  INDEX [IX_yaf_PollVote_PollID] ON [{databaseOwner}].[yaf_PollVote]([PollID])
>>>>>>> .r1490
GO

-- {objectQualifier}UserGroup

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}UserGroup_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}UserGroup]'))
 CREATE  INDEX [IX_{objectQualifier}UserGroup_UserID] ON [{databaseOwner}].[{objectQualifier}UserGroup]([UserID])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_UserGroup_UserID' and id=object_id(N'yaf_UserGroup'))
 CREATE  INDEX [IX_yaf_UserGroup_UserID] ON [{databaseOwner}].[yaf_UserGroup]([UserID])
>>>>>>> .r1490
GO

-- {objectQualifier}Message

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Message_TopicID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Message]'))
 CREATE  INDEX [IX_{objectQualifier}Message_TopicID] ON [{databaseOwner}].[{objectQualifier}Message]([TopicID])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_Message_TopicID' and id=object_id(N'yaf_Message'))
 CREATE  INDEX [IX_yaf_Message_TopicID] ON [{databaseOwner}].[yaf_Message]([TopicID])
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Message_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Message]'))
 CREATE  INDEX [IX_{objectQualifier}Message_UserID] ON [{databaseOwner}].[{objectQualifier}Message]([UserID])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_Message_UserID' and id=object_id(N'yaf_Message'))
 CREATE  INDEX [IX_yaf_Message_UserID] ON [{databaseOwner}].[yaf_Message]([UserID])
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Message_Flags' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Message]'))
 CREATE  INDEX [IX_{objectQualifier}Message_Flags] ON [{databaseOwner}].[{objectQualifier}Message]([Flags])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_Message_Flags' and id=object_id(N'yaf_Message'))
 CREATE  INDEX [IX_yaf_Message_Flags] ON [{databaseOwner}].[yaf_Message]([Flags])
>>>>>>> .r1490
GO

-- {objectQualifier}Topic

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Topic_ForumID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Topic]'))
 CREATE  INDEX [IX_{objectQualifier}Topic_ForumID] ON [{databaseOwner}].[{objectQualifier}Topic]([ForumID])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_Topic_ForumID' and id=object_id(N'yaf_Topic'))
 CREATE  INDEX [IX_yaf_Topic_ForumID] ON [{databaseOwner}].[yaf_Topic]([ForumID])
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Topic_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Topic]'))
 CREATE  INDEX [IX_{objectQualifier}Topic_UserID] ON [{databaseOwner}].[{objectQualifier}Topic]([UserID])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_Topic_UserID' and id=object_id(N'yaf_Topic'))
 CREATE  INDEX [IX_yaf_Topic_UserID] ON [{databaseOwner}].[yaf_Topic]([UserID])
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Topic_Flags' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Topic]'))
 CREATE  INDEX [IX_{objectQualifier}Topic_Flags] ON [{databaseOwner}].[{objectQualifier}Topic]([Flags])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_Topic_Flags' and id=object_id(N'yaf_Topic'))
 CREATE  INDEX [IX_yaf_Topic_Flags] ON [{databaseOwner}].[yaf_Topic]([Flags])
>>>>>>> .r1490
GO

-- {objectQualifier}Forum

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Forum_CategoryID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Forum]'))
 CREATE  INDEX [IX_{objectQualifier}Forum_CategoryID] ON [{databaseOwner}].[{objectQualifier}Forum]([CategoryID])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_Forum_CategoryID' and id=object_id(N'yaf_Forum'))
 CREATE  INDEX [IX_yaf_Forum_CategoryID] ON [{databaseOwner}].[yaf_Forum]([CategoryID])
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Forum_ParentID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Forum]'))
 CREATE  INDEX [IX_{objectQualifier}Forum_ParentID] ON [{databaseOwner}].[{objectQualifier}Forum]([ParentID])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_Forum_ParentID' and id=object_id(N'yaf_Forum'))
 CREATE  INDEX [IX_yaf_Forum_ParentID] ON [{databaseOwner}].[yaf_Forum]([ParentID])
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Forum_Flags' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Forum]'))
 CREATE  INDEX [IX_{objectQualifier}Forum_Flags] ON [{databaseOwner}].[{objectQualifier}Forum]([Flags])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_Forum_Flags' and id=object_id(N'yaf_Forum'))
 CREATE  INDEX [IX_yaf_Forum_Flags] ON [{databaseOwner}].[yaf_Forum]([Flags])
>>>>>>> .r1490
GO

-- {objectQualifier}Active

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Active_TopicID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Active]'))
 CREATE  INDEX [IX_{objectQualifier}Active_TopicID] ON [{databaseOwner}].[{objectQualifier}Active]([TopicID])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_Active_TopicID' and id=object_id(N'yaf_Active'))
 CREATE  INDEX [IX_yaf_Active_TopicID] ON [{databaseOwner}].[yaf_Active]([TopicID])
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Active_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Active]'))
 CREATE  INDEX [IX_{objectQualifier}Active_UserID] ON [{databaseOwner}].[{objectQualifier}Active]([UserID])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_Active_UserID' and id=object_id(N'yaf_Active'))
 CREATE  INDEX [IX_yaf_Active_UserID] ON [{databaseOwner}].[yaf_Active]([UserID])
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Active_ForumID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Active]'))
 CREATE  INDEX [IX_{objectQualifier}Active_ForumID] ON [{databaseOwner}].[{objectQualifier}Active]([ForumID])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_Active_ForumID' and id=object_id(N'yaf_Active'))
 CREATE  INDEX [IX_yaf_Active_ForumID] ON [{databaseOwner}].[yaf_Active]([ForumID])
>>>>>>> .r1490
GO

-- {objectQualifier}User

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}User_Flags' and id=object_id(N'[{databaseOwner}].[{objectQualifier}User]'))
 CREATE  INDEX [IX_{objectQualifier}User_Flags] ON [{databaseOwner}].[{objectQualifier}User]([Flags])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_User_Flags' and id=object_id(N'yaf_User'))
 CREATE  INDEX [IX_yaf_User_Flags] ON [{databaseOwner}].[yaf_User]([Flags])
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}User_ProviderUserKey' and id=object_id(N'[{databaseOwner}].[{objectQualifier}User]'))
 CREATE  INDEX [IX_{objectQualifier}User_ProviderUserKey] ON [{databaseOwner}].[{objectQualifier}User]([ProviderUserKey])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_User_ProviderUserKey' and id=object_id(N'yaf_User'))
 CREATE  INDEX [IX_yaf_User_ProviderUserKey] ON [{databaseOwner}].[yaf_User]([ProviderUserKey])
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}User_Name' and id=object_id(N'[{databaseOwner}].[{objectQualifier}User]'))
 CREATE  INDEX [IX_{objectQualifier}User_Name] ON [{databaseOwner}].[{objectQualifier}User]([Name])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_User_Name' and id=object_id(N'yaf_User'))
 CREATE  INDEX [IX_yaf_User_Name] ON [{databaseOwner}].[yaf_User]([Name])
>>>>>>> .r1490
GO

-- {objectQualifier}ForumAccess

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}ForumAccess_ForumID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}ForumAccess]'))
 CREATE  INDEX [IX_{objectQualifier}ForumAccess_ForumID] ON [{databaseOwner}].[{objectQualifier}ForumAccess]([ForumID])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_ForumAccess_ForumID' and id=object_id(N'yaf_ForumAccess'))
 CREATE  INDEX [IX_yaf_ForumAccess_ForumID] ON [{databaseOwner}].[yaf_ForumAccess]([ForumID])
>>>>>>> .r1490
GO

-- {objectQualifier}UserPMessage

<<<<<<< .mine
if not exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}UserPMessage_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}UserPMessage]'))
 CREATE  INDEX [IX_{objectQualifier}UserPMessage_UserID] ON [{databaseOwner}].[{objectQualifier}UserPMessage]([UserID])
=======
if not exists(select 1 from sysindexes where name=N'IX_yaf_UserPMessage_UserID' and id=object_id(N'yaf_UserPMessage'))
 CREATE  INDEX [IX_yaf_UserPMessage_UserID] ON [{databaseOwner}].[yaf_UserPMessage]([UserID])
>>>>>>> .r1490
GO
