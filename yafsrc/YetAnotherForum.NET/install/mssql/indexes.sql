/*
** Indexes
*/
-- {objectQualifier}Buddy
if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Buddy_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Buddy]'))
	CREATE  INDEX [IX_{objectQualifier}Buddy_UserID] ON [{databaseOwner}].[{objectQualifier}Buddy]([FromUserID],[ToUserID])
go

-- {objectQualifier}Registry

if exists(select top 1 1 from dbo.sysindexes where name=N'IX_Name' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Registry]'))
	drop index [{databaseOwner}].[{objectQualifier}Registry].[IX_Name]
go

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Registry_Name' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Registry]'))
	CREATE  INDEX [IX_{objectQualifier}Registry_Name] ON [{databaseOwner}].[{objectQualifier}Registry]([BoardID],[Name])
go

-- {objectQualifier}PollVote

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}PollVote_RemoteIP' and id=object_id(N'[{databaseOwner}].[{objectQualifier}PollVote]'))
 CREATE  INDEX [IX_{objectQualifier}PollVote_RemoteIP] ON [{databaseOwner}].[{objectQualifier}PollVote]([RemoteIP])
go

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}PollVote_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}PollVote]'))
 CREATE  INDEX [IX_{objectQualifier}PollVote_UserID] ON [{databaseOwner}].[{objectQualifier}PollVote]([UserID])
go

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}PollVote_PollID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}PollVote]'))
 CREATE  INDEX [IX_{objectQualifier}PollVote_PollID] ON [{databaseOwner}].[{objectQualifier}PollVote]([PollID])
go

-- {objectQualifier}UserGroup

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}UserGroup_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}UserGroup]'))
 CREATE  INDEX [IX_{objectQualifier}UserGroup_UserID] ON [{databaseOwner}].[{objectQualifier}UserGroup]([UserID])
go

-- {objectQualifier}Message

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Message_TopicID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Message]'))
 CREATE  INDEX [IX_{objectQualifier}Message_TopicID] ON [{databaseOwner}].[{objectQualifier}Message]([TopicID])
go

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Message_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Message]'))
 CREATE  INDEX [IX_{objectQualifier}Message_UserID] ON [{databaseOwner}].[{objectQualifier}Message]([UserID])
go

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Message_Flags' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Message]'))
 CREATE  INDEX [IX_{objectQualifier}Message_Flags] ON [{databaseOwner}].[{objectQualifier}Message]([Flags])
go

IF  NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Message]') AND name = N'IX_{objectQualifier}Message_Posted_Desc')
CREATE NONCLUSTERED INDEX [IX_{objectQualifier}Message_Posted_Desc] ON [{databaseOwner}].[{objectQualifier}Message] 
(
	[Posted] DESC
) ON [PRIMARY]
GO

-- {objectQualifier}Topic

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Topic_ForumID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Topic]'))
 CREATE  INDEX [IX_{objectQualifier}Topic_ForumID] ON [{databaseOwner}].[{objectQualifier}Topic]([ForumID])
go

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Topic_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Topic]'))
 CREATE  INDEX [IX_{objectQualifier}Topic_UserID] ON [{databaseOwner}].[{objectQualifier}Topic]([UserID])
go

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Topic_Flags' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Topic]'))
 CREATE  INDEX [IX_{objectQualifier}Topic_Flags] ON [{databaseOwner}].[{objectQualifier}Topic]([Flags])
go

-- {objectQualifier}Forum

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Forum_CategoryID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Forum]'))
 CREATE  INDEX [IX_{objectQualifier}Forum_CategoryID] ON [{databaseOwner}].[{objectQualifier}Forum]([CategoryID])
go

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Forum_ParentID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Forum]'))
 CREATE  INDEX [IX_{objectQualifier}Forum_ParentID] ON [{databaseOwner}].[{objectQualifier}Forum]([ParentID])
go

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Forum_Flags' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Forum]'))
 CREATE  INDEX [IX_{objectQualifier}Forum_Flags] ON [{databaseOwner}].[{objectQualifier}Forum]([Flags])
go

-- {objectQualifier}User

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}User_Flags' and id=object_id(N'[{databaseOwner}].[{objectQualifier}User]'))
 CREATE  INDEX [IX_{objectQualifier}User_Flags] ON [{databaseOwner}].[{objectQualifier}User]([Flags])
go

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}User_Joined' and id=object_id(N'[{databaseOwner}].[{objectQualifier}User]'))
 CREATE  INDEX [IX_{objectQualifier}User_Joined] ON [{databaseOwner}].[{objectQualifier}User]([Joined])
go

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}User_ProviderUserKey' and id=object_id(N'[{databaseOwner}].[{objectQualifier}User]'))
 CREATE  INDEX [IX_{objectQualifier}User_ProviderUserKey] ON [{databaseOwner}].[{objectQualifier}User]([ProviderUserKey])
go

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}User_Name' and id=object_id(N'[{databaseOwner}].[{objectQualifier}User]'))
 CREATE  INDEX [IX_{objectQualifier}User_Name] ON [{databaseOwner}].[{objectQualifier}User]([Name])
go

-- {objectQualifier}ForumAccess

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}ForumAccess_ForumID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}ForumAccess]'))
 CREATE  INDEX [IX_{objectQualifier}ForumAccess_ForumID] ON [{databaseOwner}].[{objectQualifier}ForumAccess]([ForumID])
go

-- {objectQualifier}UserPMessage

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}UserPMessage_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}UserPMessage]'))
 CREATE  INDEX [IX_{objectQualifier}UserPMessage_UserID] ON [{databaseOwner}].[{objectQualifier}UserPMessage]([UserID])
go

-- {objectQualifier}Category

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Category_BoardID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Category]'))
 CREATE  INDEX [IX_{objectQualifier}Category_BoardID] ON [{databaseOwner}].[{objectQualifier}Category]([BoardID])
go

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}Category_Name' and id=object_id(N'[{databaseOwner}].[{objectQualifier}Category]'))
 CREATE  INDEX [IX_{objectQualifier}Category_Name] ON [{databaseOwner}].[{objectQualifier}Category]([Name])
go

-- {objectQualifier}FavoriteTopic

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}FavoriteTopic_TopicID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}FavoriteTopic]'))
 CREATE  INDEX [IX_{objectQualifier}FavoriteTopic_TopicID] ON [{databaseOwner}].[{objectQualifier}FavoriteTopic]([TopicID])
go

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}FavoriteTopic_UserID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}FavoriteTopic]'))
 CREATE  INDEX [IX_{objectQualifier}FavoriteTopic_UserID] ON [{databaseOwner}].[{objectQualifier}FavoriteTopic]([UserID])
go

-- {objectQualifier}UserAlbum

if not exists(select top 1 1 from dbo.sysindexes where name=N'IX_{objectQualifier}UserAlbumImage_AlbumID' and id=object_id(N'[{databaseOwner}].[{objectQualifier}UserAlbumImage]'))
 CREATE  INDEX [IX_{objectQualifier}UserAlbumImage_AlbumID] ON [{databaseOwner}].[{objectQualifier}UserAlbumImage]([AlbumID])
go

-- {objectQualifier}Thanks

IF NOT EXISTS (SELECT 1 FROM dbo.sysindexes WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Thanks]') AND name = N'IX_{objectQualifier}Thanks_MessageID')
CREATE  INDEX [IX_{objectQualifier}Thanks_MessageID] ON [{databaseOwner}].[{objectQualifier}Thanks] 
(
	[MessageID] ASC
)
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysindexes WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Thanks]') AND name = N'IX_{objectQualifier}Thanks_ThanksFromUserID')
CREATE  INDEX [IX_{objectQualifier}Thanks_ThanksFromUserID] ON [{databaseOwner}].[{objectQualifier}Thanks] 
(
	[ThanksFromUserID] ASC
)
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysindexes WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Thanks]') AND name = N'IX_{objectQualifier}Thanks_ThanksToUserID')
CREATE  INDEX [IX_{objectQualifier}Thanks_ThanksToUserID] ON [{databaseOwner}].[{objectQualifier}Thanks] 
(
	[ThanksToUserID] ASC
)
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysindexes WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}FavoriteTopic]') AND name = N'IX_{objectQualifier}FavoriteTopic_TopicID')
CREATE  INDEX [IX_{objectQualifier}FavoriteTopic_TopicID] ON [{databaseOwner}].[{objectQualifier}FavoriteTopic] 
(
	[TopicID] ASC
)
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysindexes WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}FavoriteTopic]') AND name = N'IX_{objectQualifier}FavoriteTopic_UserID')
CREATE  INDEX [IX_{objectQualifier}FavoriteTopic_UserID] ON [{databaseOwner}].[{objectQualifier}FavoriteTopic] 
(
	[UserID] ASC
)
GO

IF  NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Topic]') AND name = N'IX_{objectQualifier}Topic_LastPosted_Desc')
CREATE NONCLUSTERED INDEX [IX_{objectQualifier}Topic_LastPosted_Desc] ON [{databaseOwner}].[{objectQualifier}Topic] 
(
	[LastPosted] DESC
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Group]') AND name = N'IX_{objectQualifier}Group_SortOrder')
CREATE NONCLUSTERED INDEX [IX_{objectQualifier}Group_SortOrder] ON [{databaseOwner}].[{objectQualifier}Group] 
(
	[SortOrder] ASC
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}User]') AND name = N'IX_{objectQualifier}User_DisplayName')
CREATE NONCLUSTERED INDEX [IX_{objectQualifier}User_DisplayName] ON [{databaseOwner}].[{objectQualifier}User] 
(
	[DisplayName] ASC
) ON [PRIMARY]
GO
