/* Version 1.0.2 */

IF  exists (select top 1 1 from dbo.sysobjects where id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}drop_defaultconstraint_oncolumn]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}drop_defaultconstraint_oncolumn]
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}drop_defaultconstraint_oncolumn](@tablename varchar(255), @columnname varchar(255)) as
BEGIN
DECLARE @DefName sysname

SELECT 
  @DefName = o1.name
FROM
  sysobjects o1
  INNER JOIN syscolumns c ON
  o1.id = c.cdefault
  INNER JOIN sysobjects o2 ON
  c.id = o2.id
WHERE
  o2.name = @tablename AND
  c.name = @columnname
  
IF @DefName IS NOT NULL
  EXECUTE ('ALTER TABLE ' + @tablename + ' DROP CONSTRAINT [' + @DefName + ']')
END
GO

/*
** Create missing tables
*/

/* Create Thanks Table */
if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Thanks]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE [{databaseOwner}].[{objectQualifier}Thanks](
	[ThanksID] [int] IDENTITY(1,1) NOT NULL,
	[ThanksFromUserID] [int] NOT NULL,
	[ThanksToUserID] [int] NOT NULL,
	[MessageID] [int] NOT NULL,
	[ThanksDate] [smalldatetime] NOT NULL
	constraint [PK_{objectQualifier}Thanks] primary key(ThanksID)
	)
go

/* YAF Buddy Table */
if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Buddy]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE [{databaseOwner}].[{objectQualifier}Buddy](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FromUserID] [int] NOT NULL,
	[ToUserID] [int] NOT NULL,
	[Approved] [bit] NOT NULL,
	[Requested] [datetime] NOT NULL
	constraint [PK_{objectQualifier}Buddy] primary key(ID)
	)
go

/* YAF FavoriteTopic Table */
if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}FavoriteTopic]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE [{databaseOwner}].[{objectQualifier}FavoriteTopic](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[TopicID] [int] NOT NULL
	constraint [PK_{objectQualifier}FavoriteTopic] primary key(ID)
	)
GO

/* YAF Album Tables*/
if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}UserAlbum]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE [{databaseOwner}].[{objectQualifier}UserAlbum](
	[AlbumID] [INT] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[Title] [NVARCHAR](255),
	[CoverImageID] [INT],
	[Updated] [DATETIME] NOT NULL
	constraint [PK_{objectQualifier}User_Album] primary key(AlbumID)
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}UserAlbumImage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE [{databaseOwner}].[{objectQualifier}UserAlbumImage](
	[ImageID] [INT] IDENTITY(1,1) NOT NULL,
	[AlbumID] [int] NOT NULL,
	[Caption] [NVARCHAR](255),
	[FileName] [NVARCHAR](255) NOT NULL,
	[Bytes] [INT] NOT NULL,
	[ContentType] [NVARCHAR](50),
	[Uploaded] [DATETIME] NOT NULL,
	[Downloads] [INT] NOT NULL
	constraint [PK_{objectQualifier}User_AlbumImage] primary key(ImageID)
	)
GO
if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}Active](
		SessionID		nvarchar (24) NOT NULL ,
		BoardID			int NOT NULL ,
		UserID			int NOT NULL ,
		IP				nvarchar (15) NOT NULL ,
		[Login]			datetime NOT NULL ,
		LastActive		datetime NOT NULL ,
		Location		nvarchar (50) NOT NULL ,
		ForumID			int NULL ,
		TopicID			int NULL ,
		Browser			nvarchar (50) NULL ,
		[Platform]		nvarchar (50) NULL,
		Flags           int NULL
	)
go

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}ActiveAccess]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}ActiveAccess](		
		UserID			    int NOT NULL ,
		BoardID			    int NOT NULL ,			
		ForumID			    int,
		IsAdmin				bit NOT NULL ,
		IsForumModerator	bit NOT NULL ,
		IsModerator			bit NOT NULL ,
		IsGuestX			bit NOT NULL constraint [DF_{objectQualifier}ActiveAccess_IsGuestX] default(0),
		LastActive			datetime NULL ,
		ReadAccess			bit NOT NULL ,
		PostAccess			bit NOT NULL ,
		ReplyAccess			bit NOT NULL,
		PriorityAccess		bit NOT NULL,
		PollAccess			bit NOT NULL,
		VoteAccess			bit NOT NULL,
		ModeratorAccess		bit NOT NULL,
		EditAccess			bit NOT NULL,
		DeleteAccess		bit NOT NULL,
		UploadAccess		bit NOT NULL,		
		DownloadAccess		bit NOT NULL		
	)
go

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}BannedIP]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}BannedIP](
		ID				int IDENTITY (1, 1) NOT NULL ,
		BoardID			int NOT NULL ,
		Mask			nvarchar (15) NOT NULL ,
		Since			datetime NOT NULL,
		Reason          nvarchar (128) NULL,
		UserID			int null
	)
go

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Category]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}Category](
		CategoryID		int IDENTITY (1, 1) NOT NULL ,
		BoardID			int NOT NULL ,
		[Name]			[nvarchar](128) NOT NULL,
		[CategoryImage] [nvarchar](255) NULL,		
		SortOrder		smallint NOT NULL,
		PollGroupID int null 
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}CheckEmail]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}CheckEmail](
		CheckEmailID	int IDENTITY (1, 1) NOT NULL ,
		UserID			int NOT NULL ,
		Email			nvarchar (255) NOT NULL ,
		Created			datetime NOT NULL ,
		[Hash]			nvarchar (32) NOT NULL 
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Choice]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}Choice](
		ChoiceID		int IDENTITY (1, 1) NOT NULL ,
		PollID			int NOT NULL ,
		Choice			nvarchar (50) NOT NULL ,
		Votes			int NOT NULL,
		[ObjectPath] nvarchar(255) NULL,
		[MimeType] varchar(50) NULL
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}PollVote]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [{databaseOwner}].[{objectQualifier}PollVote] (
		[PollVoteID] [int] IDENTITY (1, 1) NOT NULL ,
		[PollID] [int] NOT NULL ,
		[UserID] [int] NULL ,
		[RemoteIP] [varchar] (57) NULL,
		[ChoiceID] [int] NULL
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}PollVoteRefuse]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [{databaseOwner}].[{objectQualifier}PollVoteRefuse] (
		[RefuseID] [int] IDENTITY (1, 1) NOT NULL ,		
		[PollID] [int] NOT NULL ,
		[UserID] [int] NULL ,
		[RemoteIP] [varchar] (57) NULL
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Forum]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}Forum](
		ForumID			int IDENTITY (1, 1) NOT NULL ,
		CategoryID		int NOT NULL ,
		ParentID		int NULL ,
		Name			nvarchar (50) NOT NULL ,
		[Description]	nvarchar (255) NOT NULL ,
		SortOrder		smallint NOT NULL ,
		LastPosted		datetime NULL ,
		LastTopicID		int NULL ,
		LastMessageID	int NULL ,
		LastUserID		int NULL ,
		LastUserName	nvarchar (255) NULL ,
		NumTopics		int NOT NULL,
		NumPosts		int NOT NULL,
		RemoteURL		nvarchar(100) null,
		Flags			int not null constraint [DF_{objectQualifier}Forum_Flags] default (0),
		[IsLocked]		AS (CONVERT([bit],sign([Flags]&(1)),(0))),
		[IsHidden]		AS (CONVERT([bit],sign([Flags]&(2)),(0))),
		[IsNoCount]		AS (CONVERT([bit],sign([Flags]&(4)),(0))),
		[IsModerated]	AS (CONVERT([bit],sign([Flags]&(8)),(0))),
		ThemeURL		nvarchar(50) NULL,
		PollGroupID     int null,
		UserID          int null 
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}ForumAccess]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}ForumAccess](
		GroupID			int NOT NULL ,
		ForumID			int NOT NULL ,
		AccessMaskID	int NOT NULL
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Group]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}Group](
		GroupID		int IDENTITY (1, 1) NOT NULL ,
		BoardID		int NOT NULL ,
		[Name]		nvarchar (255) NOT NULL ,
		Flags		int not null constraint DF_{objectQualifier}Group_Flags default (0)
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Mail]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}Mail](
		[MailID] [int] IDENTITY(1,1) NOT NULL,
		[FromUser] [nvarchar](255) NOT NULL,
		[FromUserName] [nvarchar](255) NULL,
		[ToUser] [nvarchar](255) NOT NULL,
		[ToUserName] [nvarchar](255) NULL,
		[Created] [datetime] NOT NULL,
		[Subject] [nvarchar](100) NOT NULL,
		[Body] [ntext] NOT NULL,
		[BodyHtml] [ntext] NULL,
		[SendTries] [int] NOT NULL CONSTRAINT [DF_{objectQualifier}Mail_SendTries]  DEFAULT ((0)),
		[SendAttempt] [datetime] NULL,
		[ProcessID] [int] NULL
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Message]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}Message](
		MessageID		    int IDENTITY (1, 1) NOT NULL ,
		TopicID			    int NOT NULL ,
		ReplyTo			    int NULL ,
		Position		    int NOT NULL ,
		Indent			    int NOT NULL ,
		UserID			    int NOT NULL ,
		UserName		    nvarchar (255) NULL ,
		Posted			    datetime NOT NULL ,
		[Message]		    ntext NOT NULL ,
		IP				    nvarchar (15) NOT NULL ,
		Edited			    datetime NULL ,
		Flags			    int NOT NULL constraint [DF_{objectQualifier}Message_Flags] default (23),
		EditReason          nvarchar (100) NULL ,
		IsModeratorChanged  bit NOT NULL CONSTRAINT [DF_{objectQualifier}Message_IsModeratorChanged] DEFAULT (0),
	    DeleteReason        nvarchar (100)  NULL,
		ExternalMessageId	nvarchar(255) NULL,
		ReferenceMessageId	nvarchar(255) NULL,
		IsDeleted		    AS (CONVERT([bit],sign([Flags]&(8)),0)),
		IsApproved		    AS (CONVERT([bit],sign([Flags]&(16)),(0)))
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}MessageHistory]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}MessageHistory](
		MessageID		    int NOT NULL ,
		[Message]		    ntext NOT NULL ,
		IP				    nvarchar (15) NOT NULL ,
		Edited			    datetime NULL,
		EditedBy		    int NULL,	
		EditReason          nvarchar (100) NULL ,
		IsModeratorChanged  bit NOT NULL CONSTRAINT [DF_{objectQualifier}MessageHistory_IsModeratorChanged] DEFAULT (0),
		Flags               int NOT NULL constraint [DF_{objectQualifier}MessageHistory_Flags] default (23)	  
	)
GO

exec('[{databaseOwner}].[{objectQualifier}drop_defaultconstraint_oncolumn] {objectQualifier}MessageHistory, MessageHistoryID')
GO

IF NOT EXISTS (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}MessageReported]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [{databaseOwner}].[{objectQualifier}MessageReported](
		[MessageID] [int] NOT NULL,
		[Message] [ntext] NULL,
		[Resolved] [bit] NULL,
		[ResolvedBy] [int] NULL,
		[ResolvedDate] [datetime] NULL
	)
GO

IF NOT EXISTS (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}MessageReportedAudit]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [{databaseOwner}].[{objectQualifier}MessageReportedAudit](
		[LogID] [int] IDENTITY(1,1) NOT NULL,
		[UserID] [int] NULL,
		[MessageID] [int] NULL,
		[Reported] [datetime] NULL
		)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}PMessage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}PMessage](
		PMessageID		int IDENTITY (1, 1) NOT NULL ,
		FromUserID		int NOT NULL ,
		Created			datetime NOT NULL ,
		[Subject]		nvarchar (100) NOT NULL ,
		Body			ntext NOT NULL,
		Flags			int NOT NULL 
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}PollGroupCluster]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}PollGroupCluster](		
		PollGroupID int IDENTITY (1, 1) NOT NULL,
		UserID	    int not NULL,
		[Flags]     int NOT NULL constraint [DF_{objectQualifier}PollGroupCluster_Flags] default (0),
		[IsBound]   AS (CONVERT([bit],sign([Flags]&(2)),(0)))	
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Poll]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}Poll](
		PollID			int IDENTITY (1, 1) NOT NULL ,
		Question		nvarchar (50) NOT NULL,
		Closes datetime NULL,		
		PollGroupID int NULL,
		UserID int not NULL,	
		[ObjectPath] nvarchar(255) NULL,
		[MimeType] varchar(50) NULL,
		[Flags] int NOT NULL constraint [DF_{objectQualifier}Poll_Flags] default (0),		
		[IsClosedBound]	AS (CONVERT([bit],sign([Flags]&(4)),(0))),
		[AllowMultipleChoices] AS (CONVERT([bit],sign([Flags]&(8)),(0))),
		[ShowVoters] AS (CONVERT([bit],sign([Flags]&(16)),(0))),
		[AllowSkipVote] AS (CONVERT([bit],sign([Flags]&(32)),(0)))
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Smiley]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}Smiley](
		SmileyID		int IDENTITY (1, 1) NOT NULL ,
		BoardID			int NOT NULL ,
		Code			nvarchar (10) NOT NULL ,
		Icon			nvarchar (50) NOT NULL ,
		Emoticon		nvarchar (50) NULL ,
		SortOrder		tinyint	NOT NULL DEFAULT 0
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}Topic](
		TopicID			    int IDENTITY (1, 1) NOT NULL ,
		ForumID			    int NOT NULL ,
		UserID			    int NOT NULL ,
		UserName		    nvarchar (255) NULL ,		
		Posted			    datetime NOT NULL ,
		Topic			    nvarchar (100) NOT NULL ,
		[Description]		nvarchar (255) NULL ,
		[Status]	     	nvarchar (255) NULL ,
		[Styles]	     	nvarchar (255) NULL ,
		[Views]			    int NOT NULL ,
		[Priority]		    smallint NOT NULL ,
		PollID			    int NULL ,
		TopicMovedID	    int NULL ,
		LastPosted		    datetime NULL ,
		LastMessageID	    int NULL ,
		LastUserID		    int NULL ,
		LastUserName	    nvarchar (255) NULL,		
		NumPosts		    int NOT NULL,
		Flags			    int not null constraint [DF_{objectQualifier}Topic_Flags] default (0),
		IsDeleted		    AS (CONVERT([bit],sign([Flags]&(8)),0)),
		[IsQuestion]        AS (CONVERT([bit],sign([Flags]&(1024)),(0))),
		[AnswerMessageId]   [int] NULL,
		[LastMessageFlags]	[int] NULL,
		[TopicImage]        nvarchar(255) NULL
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}User]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}User](
		UserID			int IDENTITY (1, 1) NOT NULL ,
		BoardID			int NOT NULL,
		ProviderUserKey	nvarchar(64),
		[Name]			nvarchar (255) NOT NULL,
		[DisplayName]	nvarchar (255) NOT NULL,
		[Password]		nvarchar (32) NOT NULL,
		[Email]			nvarchar (255) NULL,
		Joined			datetime NOT NULL,
		LastVisit		datetime NOT NULL,
		IP				nvarchar (15) NULL,
		NumPosts		int NOT NULL,
		TimeZone		int NOT NULL,
		Avatar			nvarchar (255) NULL,
		[Signature]		ntext NULL,
		AvatarImage		image NULL,
		AvatarImageType	nvarchar (50) NULL,
		RankID			[int] NOT NULL,
		Suspended		[datetime] NULL,
		LanguageFile	nvarchar(50) NULL,
		ThemeFile		nvarchar(50) NULL,
		[UseSingleSignOn][bit] NOT NULL CONSTRAINT [DF_{objectQualifier}User_UseSingleSignOn] DEFAULT (0),
		TextEditor		nvarchar(50) NULL,
		OverrideDefaultThemes	bit NOT NULL CONSTRAINT [DF_{objectQualifier}User_OverrideDefaultThemes] DEFAULT (0),
		[PMNotification] [bit] NOT NULL CONSTRAINT [DF_{objectQualifier}User_PMNotification] DEFAULT (1),
		[AutoWatchTopics] [bit] NOT NULL CONSTRAINT [DF_{objectQualifier}User_AutoWatchTopics] DEFAULT (0),
		[DailyDigest] [bit] NOT NULL CONSTRAINT [DF_{objectQualifier}User_DailyDigest] DEFAULT (0),
		[NotificationType] [int] DEFAULT (10),
		[Flags] [int]	NOT NULL CONSTRAINT [DF_{objectQualifier}User_Flags] DEFAULT (0),
		[Points] [int]	NOT NULL CONSTRAINT [DF_{objectQualifier}User_Points] DEFAULT (0),		
		[IsApproved]	AS (CONVERT([bit],sign([Flags]&(2)),(0))),
		[IsGuest]	AS (CONVERT([bit],sign([Flags]&(4)),(0))),
		[IsCaptchaExcluded]	AS (CONVERT([bit],sign([Flags]&(8)),(0))),
		[IsActiveExcluded] AS (CONVERT([bit],sign([Flags]&(16)),(0))),
		[IsDST]	AS (CONVERT([bit],sign([Flags]&(32)),(0))),
		[IsDirty]	AS (CONVERT([bit],sign([Flags]&(64)),(0))),
		[Culture] varchar (10) DEFAULT (10),
		[IsFacebookUser][bit] NOT NULL CONSTRAINT [DF_{objectQualifier}User_IsFacebookUser] DEFAULT (0),
		[IsTwitterUser][bit] NOT NULL CONSTRAINT [DF_{objectQualifier}User_IsTwitterUser] DEFAULT (0)
)
GO

IF not exists (select top 1 1 from sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}UserProfile]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [{databaseOwner}].[{objectQualifier}UserProfile]
	(
		[UserID] [int] NOT NULL,
		[LastUpdatedDate] [datetime] NOT NULL,
		-- added columns
		[LastActivity] [datetime],
		[ApplicationName] [nvarchar](255) NOT NULL,	
		[IsAnonymous] [bit] NOT NULL,
		[UserName] [nvarchar](255) NOT NULL
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}WatchForum]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}WatchForum](
		WatchForumID	int IDENTITY (1, 1) NOT NULL ,
		ForumID			int NOT NULL ,
		UserID			int NOT NULL ,
		Created			datetime NOT NULL ,
		LastMail		datetime null
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}WatchTopic]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}WatchTopic](
		WatchTopicID	int IDENTITY (1, 1) NOT NULL ,
		TopicID			int NOT NULL ,
		UserID			int NOT NULL ,
		Created			datetime NOT NULL ,
		LastMail		datetime null
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Attachment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}Attachment](
		AttachmentID	int IDENTITY (1, 1) not null,
		MessageID		int not null,		
		[FileName]		nvarchar(255) not null,
		Bytes			int not null,
		FileID			int null,
		ContentType		nvarchar(50) null,
		Downloads		int not null,
		FileData		image null
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}UserGroup]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}UserGroup](
		UserID			int NOT NULL,
		GroupID			int NOT NULL
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Rank]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}Rank](
		RankID			int IDENTITY (1, 1) NOT NULL,
		BoardID			int NOT NULL ,
		Name			nvarchar (50) NOT NULL,
		MinPosts		int NULL,
		RankImage		nvarchar (50) NULL,
		Flags			int not null constraint [DF_{objectQualifier}Rank_Flags] default (0)
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}AccessMask]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}AccessMask](
		AccessMaskID	int IDENTITY (1, 1) NOT NULL ,
		BoardID			int NOT NULL ,
		Name			nvarchar(50) NOT NULL ,
		Flags			int not null constraint [DF_{objectQualifier}AccessMask_Flags] default (0)
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}UserForum]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}UserForum](
		UserID			int NOT NULL ,
		ForumID			int NOT NULL ,
		AccessMaskID	int NOT NULL ,
		Invited			datetime NOT NULL ,
		Accepted		bit NOT NULL
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Board]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin
	create table [{databaseOwner}].[{objectQualifier}Board](
		BoardID			int IDENTITY (1, 1) NOT NULL,
		Name			nvarchar(50) NOT NULL,
		AllowThreaded	bit NOT NULL,
		MembershipAppName nvarchar(255) NULL,
		RolesAppName nvarchar(255) NULL
	)
end
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}NntpServer]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}NntpServer](
		NntpServerID	int IDENTITY (1, 1) not null,
		BoardID			int NOT NULL ,
		Name			nvarchar(50) not null,
		[Address]			nvarchar(100) not null,
		Port			int null,
		UserName		nvarchar(255) null,
		UserPass		nvarchar(50) null
		
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}NntpForum]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}NntpForum](
		NntpForumID		int IDENTITY (1, 1) not null,
		NntpServerID	int not null,
		GroupName		nvarchar(100) not null,
		ForumID			int not null,
		LastMessageNo	int not null,
		LastUpdate		datetime not null,
		Active			bit not null,
		DateCutOff		datetime null
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}NntpTopic]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}NntpTopic](
		NntpTopicID		int IDENTITY (1, 1) not null,
		NntpForumID		int not null,
		Thread			varchar(64) not null,
		TopicID			int not null
	)
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}UserPMessage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin
	create table [{databaseOwner}].[{objectQualifier}UserPMessage](
		UserPMessageID	int IDENTITY (1, 1) not null,
		UserID			int not null,
		PMessageID		int not null,
		[Flags]			int NOT NULL DEFAULT ((0)),
		[IsRead]		AS (CONVERT([bit],sign([Flags]&(1)),(0))),
		[IsInOutbox]	AS (CONVERT([bit],sign([Flags]&(2)),(0))),
		[IsArchived]	AS (CONVERT([bit],sign([Flags]&(4)),(0))),
		[IsDeleted]		AS (CONVERT([bit],sign([Flags]&(8)),(0)))			
	)
end
GO

if not exists (select top 1 1 from dbo.sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Replace_Words]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin
	create table [{databaseOwner}].[{objectQualifier}Replace_Words](
		ID				int IDENTITY (1, 1) NOT NULL,
		BoardId			int NOT NULL,
		BadWord			nvarchar (255) NULL ,
		GoodWord		nvarchar (255) NULL ,
		constraint [PK_{objectQualifier}Replace_Words] primary key(ID)
	)
end
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Registry]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin
	create table [{databaseOwner}].[{objectQualifier}Registry](
		RegistryID		int IDENTITY(1, 1) NOT NULL,
		Name			nvarchar(50) NOT NULL,
		Value			ntext,
		BoardID			int,
		CONSTRAINT [PK_{objectQualifier}Registry] PRIMARY KEY (RegistryID)
	)
end
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin
	create table [{databaseOwner}].[{objectQualifier}EventLog](
		EventLogID	int identity(1,1) not null,
		EventTime	datetime not null constraint [DF_{objectQualifier}EventLog_EventTime] default GETUTCDATE() ,
		UserID		int,
		[Source]	nvarchar(50) not null,
		Description	ntext not null,
		constraint [PK_{objectQualifier}EventLog] primary key(EventLogID)
	)
end
GO


if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Extension]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [{databaseOwner}].[{objectQualifier}Extension](
		ExtensionID int IDENTITY(1,1) NOT NULL,
		BoardId int NOT NULL,
		Extension nvarchar(10) NOT NULL,
		CONSTRAINT [PK_{objectQualifier}Extension] PRIMARY KEY(ExtensionID)
	)
END
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}BBCode]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin
	create table [{databaseOwner}].[{objectQualifier}BBCode](
		[BBCodeID] [int] IDENTITY(1,1) NOT NULL,
		[BoardID] [int] NOT NULL,
		[Name] [nvarchar](255) NOT NULL,
		[Description] [nvarchar](4000) NULL,
		[OnClickJS] [nvarchar](1000) NULL,
		[DisplayJS] [ntext] NULL,
		[EditJS] [ntext] NULL,
		[DisplayCSS] [ntext] NULL,
		[SearchRegex] [ntext] NULL,
		[ReplaceRegex] [ntext] NULL,
		[Variables] [nvarchar](1000) NULL,
		[UseModule] [bit] NULL,
		[ModuleClass] [nvarchar](255) NULL,		
		[ExecOrder] [int] NOT NULL,
		CONSTRAINT [PK_{objectQualifier}BBCode] PRIMARY KEY (BBCodeID)
	)
end
GO


if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Medal]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin
	create table [{databaseOwner}].[{objectQualifier}Medal](
		[BoardID] [int] NOT NULL,
		[MedalID] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](100) NOT NULL,
		[Description] [ntext] NOT NULL,
		[Message] [nvarchar](100) NOT NULL,
		[Category] [nvarchar](50) NULL,
		[MedalURL] [nvarchar](250) NOT NULL,
		[RibbonURL] [nvarchar](250) NULL,
		[SmallMedalURL] [nvarchar](250) NOT NULL,
		[SmallRibbonURL] [nvarchar](250) NULL,
		[SmallMedalWidth] [smallint] NOT NULL,
		[SmallMedalHeight] [smallint] NOT NULL,
		[SmallRibbonWidth] [smallint] NULL,
		[SmallRibbonHeight] [smallint] NULL,
		[SortOrder] [tinyint] NOT NULL CONSTRAINT [DF_{objectQualifier}Medal_DefaultOrder]  DEFAULT ((255)),
		[Flags] [int] NOT NULL CONSTRAINT [DF_{objectQualifier}Medal_Flags]  DEFAULT ((0)),
		CONSTRAINT [PK_{objectQualifier}Medal] PRIMARY KEY CLUSTERED ([MedalID] ASC)
		)
end
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}GroupMedal]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin
	create table [{databaseOwner}].[{objectQualifier}GroupMedal](
		[GroupID] [int] NOT NULL,
		[MedalID] [int] NOT NULL,
		[Message] [nvarchar](100) NULL,
		[Hide] [bit] NOT NULL CONSTRAINT [DF_{objectQualifier}GroupMedal_Hide]  DEFAULT ((0)),
		[OnlyRibbon] [bit] NOT NULL CONSTRAINT [DF_{objectQualifier}GroupMedal_OnlyRibbon]  DEFAULT ((0)),
		[SortOrder] [tinyint] NOT NULL CONSTRAINT [DF_{objectQualifier}GroupMedal_SortOrder]  DEFAULT ((255))
		)
end
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}UserMedal]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin
	create table [{databaseOwner}].[{objectQualifier}UserMedal](
		[UserID] [int] NOT NULL,
		[MedalID] [int] NOT NULL,
		[Message] [nvarchar](100) NULL,
		[Hide] [bit] NOT NULL CONSTRAINT [DF_{objectQualifier}UserMedal_Hide]  DEFAULT ((0)),
		[OnlyRibbon] [bit] NOT NULL CONSTRAINT [DF_{objectQualifier}UserMedal_OnlyRibbon]  DEFAULT ((0)),
		[SortOrder] [tinyint] NOT NULL CONSTRAINT [DF_{objectQualifier}UserMedal_SortOrder]  DEFAULT ((255)),
		[DateAwarded] [datetime] NOT NULL
	)
end
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}IgnoreUser]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin
	CREATE TABLE [{databaseOwner}].[{objectQualifier}IgnoreUser]
	(
		[UserID] int NOT NULL,
		[IgnoredUserID] int NOT NULL
	)
end
GO


if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}ShoutboxMessage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin
	CREATE TABLE [{databaseOwner}].[{objectQualifier}ShoutboxMessage](
		[ShoutBoxMessageID] [int] IDENTITY(1,1) NOT NULL,		
		[BoardId] [int] NOT NULL,
		[UserID] [int] NULL,
		[UserName] [nvarchar](255) NOT NULL,
		[Message] [ntext] NULL,
		[Date] [datetime] NOT NULL,
		[IP] [varchar](50) NOT NULL
	)
end
GO	

exec('[{databaseOwner}].[{objectQualifier}drop_defaultconstraint_oncolumn] {objectQualifier}Board, BoardUID')
GO

if exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Board]') and name='BoardUID')
begin
alter table [{databaseOwner}].[{objectQualifier}Board] drop column  BoardUID
end
GO

-- Mail Table
if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Mail]') and name='FromUserName')
begin
	alter table [{databaseOwner}].[{objectQualifier}Mail] add [FromUserName] [nvarchar](255) NULL
	alter table [{databaseOwner}].[{objectQualifier}Mail] add [ToUserName] [nvarchar](255) NULL
	alter table [{databaseOwner}].[{objectQualifier}Mail] add [BodyHtml] [ntext] NULL		
	alter table [{databaseOwner}].[{objectQualifier}Mail] add [SendTries] [int] NOT NULL CONSTRAINT [DF_{objectQualifier}Mail_SendTries]  DEFAULT ((0))		
	alter table [{databaseOwner}].[{objectQualifier}Mail] add [SendAttempt] [datetime] NULL
	alter table [{databaseOwner}].[{objectQualifier}Mail] add [ProcessID] [int] NULL	
end
GO

if exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Mail]') and name='FromUserName' and prec < 255)
begin
alter table [{databaseOwner}].[{objectQualifier}Mail] alter column [FromUserName] [nvarchar](255) NULL
end
GO

if exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Mail]') and name='FromUser' and prec < 255)
begin
alter table [{databaseOwner}].[{objectQualifier}Mail] alter column [FromUser] [nvarchar](255) NULL
end
GO

if exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Mail]') and name='ToUserName' and prec < 255)
begin
alter table [{databaseOwner}].[{objectQualifier}Mail] alter column [ToUserName] [nvarchar](255) NULL
end
GO

if exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Mail]') and name='ToUser' and prec < 255)
begin
alter table [{databaseOwner}].[{objectQualifier}Mail] alter column [ToUser] [nvarchar](255) NULL
end
GO

-- Active Table
if exists (select top 1 1 from syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}Active]') and name='Location' and prec < 255)
 	alter table [{databaseOwner}].[{objectQualifier}Active] alter column [Location] nvarchar(255) NOT NULL
GO

if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Active]') and name='ForumPage')
begin
	alter table [{databaseOwner}].[{objectQualifier}Active] add [ForumPage] nvarchar(255)
end
GO

if exists (select top 1 1 from syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}Active]') and name='ForumPage' and prec < 255)
 	alter table [{databaseOwner}].[{objectQualifier}Active] alter column [ForumPage] nvarchar(255) 
GO

if exists (select top 1 1 from syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}Active]') and name='IP' and prec < 39)
 	alter table [{databaseOwner}].[{objectQualifier}Active] alter column [IP] varchar(39) not null 
GO

if not exists (select top 1 1 from syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}Active]') and name='Flags')
 	alter table [{databaseOwner}].[{objectQualifier}Active] add [Flags] int NULL 
GO

if exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}Active]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)    
    grant delete on [{databaseOwner}].[{objectQualifier}Active] to public
	exec('delete from [{databaseOwner}].[{objectQualifier}Active]')
	revoke delete on [{databaseOwner}].[{objectQualifier}Active] from public
GO

-- Board Table
if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Board]') and name='MembershipAppName')
begin
	alter table [{databaseOwner}].[{objectQualifier}Board] add MembershipAppName nvarchar(255)
end
GO

if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Board]') and name='RolesAppName')
begin
	alter table [{databaseOwner}].[{objectQualifier}Board] add RolesAppName nvarchar(255)
end
GO

if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Board]') and name='MembershipAppName')
begin
	alter table [{databaseOwner}].[{objectQualifier}Board] add MembershipAppName nvarchar(255)
end
GO

-- UserPMessage Table
if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and name='Flags')
begin
	-- add new "Flags" field to UserPMessage
	alter table [{databaseOwner}].[{objectQualifier}UserPMessage] add Flags int not null  CONSTRAINT [DF_{objectQualifier}UserPMessage_Flags] DEFAULT (0)
end
GO

if exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and name='IsRead')
BEGIN
	if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and name='IsArchived')
	BEGIN	
		-- Copy "IsRead" value over
		grant update on [{databaseOwner}].[{objectQualifier}UserPMessage] to public
		exec('update [{databaseOwner}].[{objectQualifier}UserPMessage] set Flags = IsRead')
		revoke update on [{databaseOwner}].[{objectQualifier}UserPMessage] from public
		
		-- drop the old column
		alter table [{databaseOwner}].[{objectQualifier}UserPMessage] drop column IsRead
		
		-- Verify flags isn't NULL
		grant update on [{databaseOwner}].[{objectQualifier}UserPMessage] to public
		exec('update [{databaseOwner}].[{objectQualifier}UserPMessage] set Flags = 1 WHERE Flags IS NULL')
		revoke update on [{databaseOwner}].[{objectQualifier}UserPMessage] from public
		
		-- add new calculated columns	
		alter table [{databaseOwner}].[{objectQualifier}UserPMessage] ADD [IsRead] AS (CONVERT([bit],sign([Flags]&(1)),(0)))
		alter table [{databaseOwner}].[{objectQualifier}UserPMessage] ADD [IsInOutbox] AS (CONVERT([bit],sign([Flags]&(2)),(0)))
		alter table [{databaseOwner}].[{objectQualifier}UserPMessage] ADD [IsArchived] AS (CONVERT([bit],sign([Flags]&(4)),(0)))
	END
END
GO

IF NOT exists (select top 1 1 from dbo.syscolumns WHERE id = object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') AND NAME='IsDeleted')
BEGIN
	alter table [{databaseOwner}].[{objectQualifier}UserPMessage] ADD [IsDeleted] AS (CONVERT([bit],sign([Flags]&(8)),(0)))
END
GO

-- User Table
if exists(select top 1 1 from [{databaseOwner}].[{objectQualifier}Group] where ([Flags] & 2)=2)
begin
  update [{databaseOwner}].[{objectQualifier}User] set [Flags] = [Flags] | 4 where UserID in (select distinct UserID from [{databaseOwner}].[{objectQualifier}UserGroup] a join [{databaseOwner}].[{objectQualifier}Group] b on b.GroupID=a.GroupID and (b.[Flags] & 2)=2)
end
GO

if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsApproved')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] ADD [IsApproved] AS (CONVERT([bit],sign([Flags]&(2)),(0)))
end
GO

if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}User]') and name='NotificationType')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] ADD NotificationType int default(10)
end
GO

if exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}User]') and name='NotificationType')
begin
	update  [{databaseOwner}].[{objectQualifier}User] SET [NotificationType]=10 WHERE [NotificationType] IS NULL
end
GO

if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsActiveExcluded')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] ADD [IsActiveExcluded] AS (CONVERT([bit],sign([Flags]&(16)),(0)))
end
GO

if exists(select top 1 1 from dbo.syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}User]') and name=N'Signature' and xtype<>99)
	alter table [{databaseOwner}].[{objectQualifier}User] alter column Signature ntext null
go

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Flags')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add [Flags] int not null constraint DF_{objectQualifier}User_Flags default (0)
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='IsQuestion')
begin
	alter table [{databaseOwner}].[{objectQualifier}Topic] add IsQuestion AS (CONVERT([bit],sign([Flags]&(1024)),(0)))
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='TextEditor')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add TextEditor nvarchar(50) NULL
end
GO
if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='UseSingleSignOn')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add [UseSingleSignOn][bit] NOT NULL CONSTRAINT [DF_{objectQualifier}User_UseSingleSignOn] DEFAULT (0)
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='AnswerMessageId')
begin
	alter table [{databaseOwner}].[{objectQualifier}Topic] add AnswerMessageId INT NULL
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='TopicImage')
begin
	alter table [{databaseOwner}].[{objectQualifier}Topic] add TopicImage nvarchar(255) NULL
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsHostAdmin')
begin
	grant update on [{databaseOwner}].[{objectQualifier}User] to public
	exec('update [{databaseOwner}].[{objectQualifier}User] set Flags = Flags | 1 where IsHostAdmin<>0')
	revoke update on [{databaseOwner}].[{objectQualifier}User] from public
	alter table [{databaseOwner}].[{objectQualifier}User] drop column IsHostAdmin
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}PollVoteRefuse]') and name='BoardID')
begin
alter table [{databaseOwner}].[{objectQualifier}PollVoteRefuse] drop column [BoardID] 
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Approved')
begin
	grant update on [{databaseOwner}].[{objectQualifier}User] to public
	exec('update [{databaseOwner}].[{objectQualifier}User] set Flags = Flags | 2 where Approved<>0')
	revoke update on [{databaseOwner}].[{objectQualifier}User] from public
	alter table [{databaseOwner}].[{objectQualifier}User] drop column Approved
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='ProviderUserKey')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add ProviderUserKey nvarchar(64)
end
GO

if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}User]') and name='DisplayName')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] ADD [DisplayName] nvarchar(255) 
	grant update on [{databaseOwner}].[{objectQualifier}User] to public
	exec('update [{databaseOwner}].[{objectQualifier}User] SET DisplayName = [Name]')
	revoke update on [{databaseOwner}].[{objectQualifier}User] from public	
	ALTER TABLE [{databaseOwner}].[{objectQualifier}User] ALTER COLUMN [DisplayName] nvarchar(255) NOT NULL
end
GO

-- convert uniqueidentifier to nvarchar(64)
if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='ProviderUserKey' and xtype='36')
begin
	-- drop the provider user key index if it exists...
	if exists(select 1 from dbo.sysindexes where name=N'IX_{objectQualifier}User_ProviderUserKey' and id=object_id(N'[{databaseOwner}].[{objectQualifier}User]'))
	begin
		DROP INDEX [{databaseOwner}].[{objectQualifier}User].[IX_{objectQualifier}User_ProviderUserKey]
	end
	-- alter the column
	ALTER TABLE [{databaseOwner}].[{objectQualifier}User] ALTER COLUMN ProviderUserKey nvarchar(64)
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='PMNotification')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add [PMNotification] [bit] NOT NULL CONSTRAINT [DF_{objectQualifier}User_PMNotification] DEFAULT (1)
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='DailyDigest')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add [DailyDigest] [bit] NOT NULL CONSTRAINT [DF_{objectQualifier}User_DailyDigest] DEFAULT (0)
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='AutoWatchTopics')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add [AutoWatchTopics] [bit] NOT NULL CONSTRAINT [DF_{objectQualifier}User_AutoWatchTopics] DEFAULT (0)
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='OverrideDefaultThemes')
begin
alter table [{databaseOwner}].[{objectQualifier}User] add  [OverrideDefaultThemes]	bit NOT NULL CONSTRAINT [DF_{objectQualifier}User_OverrideDefaultThemes] DEFAULT (0)
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Points')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add [Points] [int] NOT NULL CONSTRAINT [DF_{objectQualifier}User_Points] DEFAULT (0)
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='AvatarImageType')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add [AvatarImageType] nvarchar(50) NULL
end
GO

-- make sure the gender column is nullable
if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Gender')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] alter column Gender tinyint NULL
end
GO

-- Add 8-letter Language Code column
if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Culture')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add Culture varchar(10) NULL
end
GO

if exists(select top 1 1 from dbo.syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}User]') and name=N'IP' and prec < 39)
	alter table [{databaseOwner}].[{objectQualifier}User] alter column [IP] varchar(39) null
go

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Name' and prec < 255)
begin
	alter table [{databaseOwner}].[{objectQualifier}User] alter column [Name] nvarchar(255) not null
end
GO

-- Only remove User table columns if version is 30+
IF EXISTS (SELECT ver FROM (SELECT CAST(CAST(value as nvarchar(255)) as int) as ver FROM [{databaseOwner}].[{objectQualifier}Registry] WHERE name = 'version') reg WHERE ver > 30)
BEGIN
	if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Gender')
	begin
		alter table [{databaseOwner}].[{objectQualifier}User] drop column Gender
	end

	if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Location')
	begin
		alter table [{databaseOwner}].[{objectQualifier}User] drop column Location
	end

	if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='HomePage')
	begin
		alter table [{databaseOwner}].[{objectQualifier}User] drop column HomePage
	end

	if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='MSN')
	begin
		alter table [{databaseOwner}].[{objectQualifier}User] drop column MSN
	end

	if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='YIM')
	begin
		alter table [{databaseOwner}].[{objectQualifier}User] drop column YIM
	end

	if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='AIM')
	begin
		alter table [{databaseOwner}].[{objectQualifier}User] drop column AIM
	end

	if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='ICQ')
	begin
		alter table [{databaseOwner}].[{objectQualifier}User] drop column ICQ
	end

	if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='RealName')
	begin
		alter table [{databaseOwner}].[{objectQualifier}User] drop column RealName
	end

	if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Occupation')
	begin
		alter table [{databaseOwner}].[{objectQualifier}User] drop column Occupation
	end
	
	if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Interests')
	begin
		alter table [{databaseOwner}].[{objectQualifier}User] drop column Interests
	end
	
	if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Weblog')
	begin
		alter table [{databaseOwner}].[{objectQualifier}User] drop column Weblog
	end
	
	if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='WeblogUrl')
	begin
		alter table [{databaseOwner}].[{objectQualifier}User] drop column WeblogUrl
		alter table [{databaseOwner}].[{objectQualifier}User] drop column WeblogUsername
		alter table [{databaseOwner}].[{objectQualifier}User] drop column WeblogID
	end
END
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsGuest')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add [IsGuest] AS (CONVERT([bit],sign([Flags]&(4)),(0)))
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsCaptchaExcluded')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add [IsCaptchaExcluded] AS (CONVERT([bit],sign([Flags]&(8)),(0)))
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsDST')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add [IsDST] AS (CONVERT([bit],sign([Flags]&(32)),(0)))
end
GO
if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsDirty')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add [IsDirty] AS (CONVERT([bit],sign([Flags]&(64)),(0)))
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsFacebookUser')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add [IsFacebookUser][bit] NOT NULL CONSTRAINT [DF_{objectQualifier}IsFacebookUser] DEFAULT (0)
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsTwitterUser')
begin
	alter table [{databaseOwner}].[{objectQualifier}User] add [IsTwitterUser][bit] NOT NULL CONSTRAINT [DF_{objectQualifier}IsTwitterUser] DEFAULT (0)
end
GO
		

-- Forum Table
if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='RemoteURL')
	alter table [{databaseOwner}].[{objectQualifier}Forum] add RemoteURL nvarchar(100) null
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='Flags')
	alter table [{databaseOwner}].[{objectQualifier}Forum] add Flags int not null constraint DF_{objectQualifier}Forum_Flags default (0)
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='ThemeURL')
	alter table [{databaseOwner}].[{objectQualifier}Forum] add ThemeURL nvarchar(50) NULL
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='Locked')
begin
	exec('update [{databaseOwner}].[{objectQualifier}Forum] set Flags = Flags | 1 where Locked<>0')
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop column Locked
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='Hidden')
begin
	exec('update [{databaseOwner}].[{objectQualifier}Forum] set Flags = Flags | 2 where Hidden<>0')
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop column Hidden
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IsTest')
begin
	exec('update [{databaseOwner}].[{objectQualifier}Forum] set Flags = Flags | 4 where IsTest<>0')
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop column IsTest
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='Moderated')
begin
	exec('update [{databaseOwner}].[{objectQualifier}Forum] set Flags = Flags | 8 where Moderated<>0')
	alter table [{databaseOwner}].[{objectQualifier}Forum] drop column Moderated
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='ImageURL')
	alter table [{databaseOwner}].[{objectQualifier}Forum] add ImageURL nvarchar(128) NULL
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='Styles')
	alter table [{databaseOwner}].[{objectQualifier}Forum] add Styles nvarchar(255) NULL
GO

if exists (select * from syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}Forum]') and name='LastUserName' and prec < 255)
 	alter table [{databaseOwner}].[{objectQualifier}Forum] alter column [LastUserName]	nvarchar (255) NULL 
GO

if exists (select * from syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}nntptopic]') and name='Thread' and prec < 64)
 	alter table [{databaseOwner}].[{objectQualifier}nntptopic] alter column [Thread]	nvarchar (64) NULL 
GO

if not exists (select * from syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}NntpForum]') and name='DateCutOff')
 	alter table [{databaseOwner}].[{objectQualifier}NntpForum] ADD	DateCutOff datetime NULL
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='PollGroupID')
	alter table [{databaseOwner}].[{objectQualifier}Forum] add PollGroupID int NULL
GO

-- Group Table
if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='Flags')
begin
	alter table [{databaseOwner}].[{objectQualifier}Group] add Flags int not null constraint [DF_{objectQualifier}Group_Flags] default (0)
end
GO

if exists (select * from syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}Group]') and name='Name' and prec < 255)
begin
if exists (select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IX_{objectQualifier}Group')
begin
	alter table [{databaseOwner}].[{objectQualifier}Group] drop constraint IX_{objectQualifier}Group 
end
 	alter table [{databaseOwner}].[{objectQualifier}Group] alter column [Name] nvarchar(255) NOT NULL
end
GO


if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IsAdmin')
begin
	exec('update [{databaseOwner}].[{objectQualifier}Group] set Flags = Flags | 1 where IsAdmin<>0')
	alter table [{databaseOwner}].[{objectQualifier}Group] drop column IsAdmin
end
GO


if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IsGuest')
begin
	exec('update [{databaseOwner}].[{objectQualifier}Group] set Flags = Flags | 2 where IsGuest<>0')
	alter table [{databaseOwner}].[{objectQualifier}Group] drop column IsGuest
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IsStart')
begin
	exec('update [{databaseOwner}].[{objectQualifier}Group] set Flags = Flags | 4 where IsStart<>0')
	alter table [{databaseOwner}].[{objectQualifier}Group] drop column IsStart
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IsModerator')
begin
	exec('update [{databaseOwner}].[{objectQualifier}Group] set Flags = Flags | 8 where IsModerator<>0')
	alter table [{databaseOwner}].[{objectQualifier}Group] drop column IsModerator
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id(N'[{databaseOwner}].[{objectQualifier}Group]') and name=N'PMLimit')
begin
		alter table [{databaseOwner}].[{objectQualifier}Group] add PMLimit int not null		
end
GO

if exists (select top 1 1 from syscolumns where id=object_id(N'[{databaseOwner}].[{objectQualifier}Group]') and name=N'PMLimit' and isnullable=1)
begin		
		grant update on [{databaseOwner}].[{objectQualifier}Group] to public
		exec('update [{databaseOwner}].[{objectQualifier}Group] set PMLimit = 30 WHERE PMLimit IS NULL')
		alter table [{databaseOwner}].[{objectQualifier}Group] alter column [PMLimit] integer NOT NULL
		revoke update on [{databaseOwner}].[{objectQualifier}Group] from public	    
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='Style')
begin
	alter table [{databaseOwner}].[{objectQualifier}Group] add Style nvarchar(255) null
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='SortOrder')
begin
	alter table [{databaseOwner}].[{objectQualifier}Group] add SortOrder smallint not null constraint [DF_{objectQualifier}Group_SortOrder] default (0)
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='Description')
begin
	alter table [{databaseOwner}].[{objectQualifier}Group] add Description nvarchar(128) null
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='UsrSigChars')
begin
	alter table [{databaseOwner}].[{objectQualifier}Group] add UsrSigChars int not null constraint [DF_{objectQualifier}Group_UsrSigChars] default (0)
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='UsrSigChars')
begin
grant update on [{databaseOwner}].[{objectQualifier}Group] to public
		exec('update [{databaseOwner}].[{objectQualifier}Group] set UsrSigChars = 128 WHERE UsrSigChars = 0 AND Name != ''Guest'' ')
		revoke update on [{databaseOwner}].[{objectQualifier}Group] from public	
end
GO


if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='UsrSigBBCodes')
begin
	alter table [{databaseOwner}].[{objectQualifier}Group] add UsrSigBBCodes nvarchar(255) null
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='UsrSigHTMLTags')
begin
	alter table [{databaseOwner}].[{objectQualifier}Group] add UsrSigHTMLTags nvarchar(255) null
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='UsrAlbums')
begin
	alter table [{databaseOwner}].[{objectQualifier}Group] add UsrAlbums int not null constraint [DF_{objectQualifier}Group_UsrAlbums] default (0)
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='UsrAlbumImages')
begin
	alter table [{databaseOwner}].[{objectQualifier}Group] add UsrAlbumImages int not null constraint [DF_{objectQualifier}Group_UsrAlbumImages] default (0)
end
GO

-- AccessMask Table
if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='Flags')
begin
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] add Flags int not null constraint [DF_{objectQualifier}AccessMask_Flags] default (0)
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='ReadAccess')
begin
	exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 1 where ReadAccess<>0')
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column ReadAccess
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='PostAccess')
begin
	exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 2 where PostAccess<>0')
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column PostAccess
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='ReplyAccess')
begin
	exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 4 where ReplyAccess<>0')
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column ReplyAccess
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='PriorityAccess')
begin
	exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 8 where PriorityAccess<>0')
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column PriorityAccess
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='PollAccess')
begin
	exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 16 where PollAccess<>0')
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column PollAccess
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='VoteAccess')
begin
	exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 32 where VoteAccess<>0')
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column VoteAccess
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='ModeratorAccess')
begin
	exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 64 where ModeratorAccess<>0')
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column ModeratorAccess
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='EditAccess')
begin
	exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 128 where EditAccess<>0')
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column EditAccess
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='DeleteAccess')
begin
	exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 256 where DeleteAccess<>0')
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column DeleteAccess
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='UploadAccess')
begin
	exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 512 where UploadAccess<>0')
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column UploadAccess
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='SortOrder')
begin
	alter table [{databaseOwner}].[{objectQualifier}AccessMask] add SortOrder smallint not null default (0)
end
GO

-- NntpForum Table

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}NntpForum]') and name='Active')
begin
	alter table [{databaseOwner}].[{objectQualifier}NntpForum] add Active bit null
	exec('update [{databaseOwner}].[{objectQualifier}NntpForum] set Active=1 where Active is null')
	alter table [{databaseOwner}].[{objectQualifier}NntpForum] alter column Active bit not null
end
GO

-- ReplaceWords Table
if exists (select * from dbo.syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}Replace_Words]') and name='badword' and prec < 255)
 	alter table [{databaseOwner}].[{objectQualifier}Replace_Words] alter column badword nvarchar(255) NULL
GO

if exists (select * from dbo.syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}Replace_Words]') and name='goodword' and prec < 255)
	alter table [{databaseOwner}].[{objectQualifier}Replace_Words] alter column goodword nvarchar(255) NULL
GO	

if not exists (select top 1 1 from syscolumns where id=object_id(N'[{databaseOwner}].[{objectQualifier}Replace_Words]') and name='BoardID')
begin
	alter table [{databaseOwner}].[{objectQualifier}Replace_Words] add BoardID int not null constraint [DF_{objectQualifier}Replace_Words_BoardID] default (1)
end
GO

-- ShoutboxMessage Table
if not exists (select top 1 1 from syscolumns where id=object_id(N'[{databaseOwner}].[{objectQualifier}ShoutboxMessage]') and name='BoardID')
begin
	alter table [{databaseOwner}].[{objectQualifier}ShoutboxMessage] add BoardID int not null constraint [DF_{objectQualifier}ShoutboxMessage_BoardID] default (1)
end
GO

-- BBCode Table
if not exists (select top 1 1 from syscolumns where id=object_id(N'[{databaseOwner}].[{objectQualifier}BBCode]') and name='UseModule')
begin
	alter table [{databaseOwner}].[{objectQualifier}BBCode] add UseModule bit null
	alter table [{databaseOwner}].[{objectQualifier}BBCode] add ModuleClass nvarchar(255) null
end
GO

-- Registry Table
if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Registry]') and name='BoardID')
	alter table [{databaseOwner}].[{objectQualifier}Registry] add BoardID int
GO

if exists (select top 1 1 from syscolumns where id=object_id(N'[{databaseOwner}].[{objectQualifier}Registry]') and name=N'Value' and xtype<>99)
	alter table [{databaseOwner}].[{objectQualifier}Registry] alter column Value ntext null
GO

-- PMessage Table
if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}PMessage]') and name='Flags')
begin
	alter table [{databaseOwner}].[{objectQualifier}PMessage] add Flags int not null constraint [DF_{objectQualifier}Message_Flags] default (23)
end
GO

-- Message Table
if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='Flags')
begin
	alter table [{databaseOwner}].[{objectQualifier}Topic] add Flags int not null constraint [DF_{objectQualifier}Topic_Flags] default (0)
	update [{databaseOwner}].[{objectQualifier}Message] set Flags = Flags & 7
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='Approved')
begin
	exec('update [{databaseOwner}].[{objectQualifier}Message] set Flags = Flags | 16 where Approved<>0')
	alter table [{databaseOwner}].[{objectQualifier}Message] drop column Approved
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='BlogPostID')
begin
	alter table [{databaseOwner}].[{objectQualifier}Message] add BlogPostID nvarchar(50)
end
GO

if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='IsDeleted')
begin
	alter table [{databaseOwner}].[{objectQualifier}Message] ADD [IsDeleted] AS (CONVERT([bit],sign([Flags]&(8)),(0)))
end
GO

if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IsHidden')
begin
	alter table [{databaseOwner}].[{objectQualifier}Forum] ADD [IsHidden] AS (CONVERT([bit],sign([Flags]&(2)),(0)))
end
GO

if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IsLocked')
begin
	alter table [{databaseOwner}].[{objectQualifier}Forum] ADD [IsLocked] AS (CONVERT([bit],sign([Flags]&(1)),(0)))
end
GO

if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IsNoCount')
begin
	alter table [{databaseOwner}].[{objectQualifier}Forum] ADD [IsNoCount] AS (CONVERT([bit],sign([Flags]&(4)),(0)))
end
GO

if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IsModerated')
begin
	alter table [{databaseOwner}].[{objectQualifier}Forum] ADD [IsModerated] AS (CONVERT([bit],sign([Flags]&(8)),(0)))
end
GO

if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='UserID')
begin
	alter table [{databaseOwner}].[{objectQualifier}Forum] ADD [UserID]  int null 
end
GO

if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='IsApproved')
begin
	alter table [{databaseOwner}].[{objectQualifier}Message] ADD [IsApproved] AS (CONVERT([bit],sign([Flags]&(16)),(0)))
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='EditReason')
	alter table [{databaseOwner}].[{objectQualifier}Message] add EditReason nvarchar (100) NULL
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='IsModeratorChanged')
	alter table [{databaseOwner}].[{objectQualifier}Message] add 	IsModeratorChanged      bit NOT NULL CONSTRAINT [DF_{objectQualifier}Message_IsModeratorChanged] DEFAULT (0)
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='DeleteReason')
	alter table [{databaseOwner}].[{objectQualifier}Message] add DeleteReason            nvarchar (100)  NULL
GO
    
-- an attempt to migrate the legacy report abuse and report spam features flags		
 if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='Flags')
begin
	grant update on [{databaseOwner}].[{objectQualifier}Message] to public	
	exec('update [{databaseOwner}].[{objectQualifier}Message] SET [{databaseOwner}].[{objectQualifier}Message].Flags =  ([{databaseOwner}].[{objectQualifier}Message].Flags  ^ POWER(2, 8) | POWER(2, 7))
		WHERE (([{databaseOwner}].[{objectQualifier}Message].Flags & 256)=256)')
	-- exec('update [{databaseOwner}].[{objectQualifier}Message] SET [{databaseOwner}].[{objectQualifier}Message].Flags =  ([{databaseOwner}].[{objectQualifier}Message].Flags  ^ POWER(2, 9) | POWER(2, 7)
	---	WHERE (([{databaseOwner}].[{objectQualifier}Message].Flags & 512)=512)')			
	revoke update on [{databaseOwner}].[{objectQualifier}Message] from public	
end


if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='EditedBy')
	alter table [{databaseOwner}].[{objectQualifier}Message] add [EditedBy]   int  NULL
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='ExternalMessageId')
	alter table [{databaseOwner}].[{objectQualifier}Message] add [ExternalMessageId]   nvarchar(255) NULL
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='ReferenceMessageId')
	alter table [{databaseOwner}].[{objectQualifier}Message] add [ReferenceMessageId]   nvarchar(255) NULL
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='ExternalMessageId' and prec < 255)
begin
	alter table [{databaseOwner}].[{objectQualifier}Message] alter column [ExternalMessageId] nvarchar (255) NULL
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='IP' and prec < 39)
begin
	alter table [{databaseOwner}].[{objectQualifier}Message] alter column [IP] varchar(39) not null
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='UserName' and prec < 255)
begin
	alter table [{databaseOwner}].[{objectQualifier}Message] alter column [UserName] nvarchar (255) NULL
end
GO

-- MessageHistory Table

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}MessageHistory]') and name='MessageHistoryID')
begin
	alter table [{databaseOwner}].[{objectQualifier}MessageHistory] drop column [MessageHistoryID]
end
GO
-- Topic Table
if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='IsLocked')
begin
	grant update on [{databaseOwner}].[{objectQualifier}Topic] to public
	exec('update [{databaseOwner}].[{objectQualifier}Topic] set Flags = Flags | 1 where IsLocked<>0')
	revoke update on [{databaseOwner}].[{objectQualifier}Topic] from public
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop column IsLocked
end
GO

if not exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='IsDeleted')
begin
	alter table [{databaseOwner}].[{objectQualifier}Topic] ADD [IsDeleted] AS (CONVERT([bit],sign([Flags]&(8)),(0)))
end
GO

if exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='UserName')
begin
	alter table [{databaseOwner}].[{objectQualifier}Topic] alter column [UserName]	nvarchar (255) NULL 
end
GO

if exists (select top 1 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='LastUserName')
begin
	alter table [{databaseOwner}].[{objectQualifier}Topic] alter column [LastUserName]	nvarchar (255) NULL 
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='LastMessageFlags')
begin
	alter table [{databaseOwner}].[{objectQualifier}Topic] add [LastMessageFlags] int null
	grant update on [{databaseOwner}].[{objectQualifier}Topic] to public
	-- vzrus : we don't migrate flags to not slow down update and possible timeouts. Users can run maintenance scripts? Else use cursors.
	exec('update [{databaseOwner}].[{objectQualifier}Topic] set LastMessageFlags = 22 WHERE LastMessageFlags IS NULL')
	revoke update on [{databaseOwner}].[{objectQualifier}Topic] from public	
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='Description')
begin
	alter table [{databaseOwner}].[{objectQualifier}Topic] add [Description] nvarchar(255) null	
end
GO

-- Rank Table
if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='Flags')
begin
	alter table [{databaseOwner}].[{objectQualifier}Rank] add Flags int not null constraint [DF_{objectQualifier}Rank_Flags] default (0)
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id(N'[{databaseOwner}].[{objectQualifier}Rank]') and name=N'PMLimit')
begin	
	alter table [{databaseOwner}].[{objectQualifier}Rank] add PMLimit int null 
	grant update on [{databaseOwner}].[{objectQualifier}Rank] to public
	exec('update [{databaseOwner}].[{objectQualifier}Rank] set PMLimit = 0 WHERE PMLimit IS NULL')
	alter table [{databaseOwner}].[{objectQualifier}Rank] alter column [PMLimit] integer NOT NULL
	revoke update on [{databaseOwner}].[{objectQualifier}Rank] from public
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='Style')
begin
	alter table [{databaseOwner}].[{objectQualifier}Rank] add Style nvarchar(255) null
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='SortOrder')
begin
	alter table [{databaseOwner}].[{objectQualifier}Rank] add SortOrder smallint not null constraint [DF_{objectQualifier}Rank_SortOrder] default (0)
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='Description')
begin
	alter table [{databaseOwner}].[{objectQualifier}Rank] add Description nvarchar(128) null
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='UsrSigChars')
begin
	alter table [{databaseOwner}].[{objectQualifier}Rank] add UsrSigChars int not null constraint [DF_{objectQualifier}Rank_UsrSigChars] default (0)
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='UsrSigChars')
begin
grant update on [{databaseOwner}].[{objectQualifier}Rank] to public
		exec('update [{databaseOwner}].[{objectQualifier}Rank] set UsrSigChars = 128 WHERE UsrSigChars = 0 AND Name != ''Guest'' ')
		revoke update on [{databaseOwner}].[{objectQualifier}Rank] from public	
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='UsrSigBBCodes')
begin
	alter table [{databaseOwner}].[{objectQualifier}Rank] add UsrSigBBCodes nvarchar(255) null
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='UsrSigHTMLTags')
begin
	alter table [{databaseOwner}].[{objectQualifier}Rank] add UsrSigHTMLTags nvarchar(255) null
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='UsrAlbums')
begin
	alter table [{databaseOwner}].[{objectQualifier}Rank] add UsrAlbums int not null constraint [DF_{objectQualifier}Rank_UsrAlbums] default (0)
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='UsrAlbumImages')
begin
	alter table [{databaseOwner}].[{objectQualifier}Rank] add UsrAlbumImages int not null constraint [DF_{objectQualifier}Rank_UsrAlbumImages] default (0)
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='IsStart')
begin
	grant update on [{databaseOwner}].[{objectQualifier}Rank] to public
	exec('update [{databaseOwner}].[{objectQualifier}Rank] set Flags = Flags | 1 where IsStart<>0')
	revoke update on [{databaseOwner}].[{objectQualifier}Rank] from public
	alter table [{databaseOwner}].[{objectQualifier}Rank] drop column IsStart
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='IsLadder')
begin
	grant update on [{databaseOwner}].[{objectQualifier}Rank] to public
	exec('update [{databaseOwner}].[{objectQualifier}Rank] set Flags = Flags | 2 where IsLadder<>0')
	revoke update on [{databaseOwner}].[{objectQualifier}Rank] from public
	alter table [{databaseOwner}].[{objectQualifier}Rank] drop column IsLadder
end
GO

--vzrus: eof migrate to independent multiple polls


-- Poll Table
if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name='Closes')
begin
	alter table [{databaseOwner}].[{objectQualifier}Poll] add Closes datetime null
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'Question' AND prec < 256 )
begin
	alter table [{databaseOwner}].[{objectQualifier}Poll] alter column Question nvarchar(256) NOT NULL
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'PollGroupID')
begin
	alter table [{databaseOwner}].[{objectQualifier}Poll] add PollGroupID int NULL
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'UserID')
begin
	alter table [{databaseOwner}].[{objectQualifier}Poll] add [UserID] int NOT NULL constraint [DF_{objectQualifier}Poll_UserID] default (1)
end
GO

IF  EXISTS (SELECT top 1 1 FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pollgroup_migration]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
begin
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pollgroup_migration]		
end
GO

-- should drop it else error
if exists(select top 1 1 from dbo.sysobjects where name='FK_{objectQualifier}Topic_{objectQualifier}Poll' and parent_obj=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_{objectQualifier}Topic_{objectQualifier}Poll] 
go 

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'Flags')
begin
	alter table [{databaseOwner}].[{objectQualifier}Poll] add [Flags] int NULL
end
GO

create procedure [{databaseOwner}].[{objectQualifier}pollgroup_migration]
 as
  begin
     declare @ptmp int
	 declare @ttmp int
	 declare @utmp int 
	 declare @PollGroupID int

        declare c cursor for
        select  PollID,TopicID, UserID from [{databaseOwner}].[{objectQualifier}Topic] where PollID IS NOT NULL
		        
        open c
        
        fetch next from c into @ptmp, @ttmp, @utmp
        while @@FETCH_STATUS = 0
        begin
		if @ptmp is not null
		begin
		insert into [{databaseOwner}].[{objectQualifier}PollGroupCluster](UserID, Flags) values (@utmp, 0)	
		SET @PollGroupID = SCOPE_IDENTITY()  
		
	            update [{databaseOwner}].[{objectQualifier}Topic] SET PollID = @PollGroupID WHERE TopicID = @ttmp
				update [{databaseOwner}].[{objectQualifier}Poll] SET UserID = @utmp, PollGroupID = @PollGroupID, Flags = 0 WHERE PollID = @ptmp
		end       
        fetch next from c into @ptmp, @ttmp, @utmp
        end

        close c
        deallocate c 

		end
GO

if (not exists (select top 1 1 from [{databaseOwner}].[{objectQualifier}PollGroupCluster]) and exists (select top 1 1 from [{databaseOwner}].[{objectQualifier}Poll]))
begin
	--vzrus: migrate to independent multiple polls	
	exec('[{databaseOwner}].[{objectQualifier}pollgroup_migration]')	

		-- vzrus: drop the temporary  sp
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pollgroup_migration]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pollgroup_migration]		
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name='Flags')
begin
	grant update on [{databaseOwner}].[{objectQualifier}Poll] to public
	exec('update [{databaseOwner}].[{objectQualifier}Poll] set Flags = 0 where Flags is null')
	revoke update on [{databaseOwner}].[{objectQualifier}Poll] from public
	-- here computed columns on Flags should be dropped if exist before
	-- alter table [{databaseOwner}].[{objectQualifier}Poll] alter column Flags int not null
	-- alter table [{databaseOwner}].[{objectQualifier}Poll] add constraint [DF_{objectQualifier}Poll_Flags] default(0) for Flags
end
GO

-- TODO: change userid to not null

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'ObjectPath')
begin
	alter table [{databaseOwner}].[{objectQualifier}Poll] add [ObjectPath] nvarchar(255) NULL
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'MimeType')
begin
	alter table [{databaseOwner}].[{objectQualifier}Poll] add [MimeType] varchar(50) NULL
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'IsClosedBound')
begin
	alter table [{databaseOwner}].[{objectQualifier}Poll] add [IsClosedBound] AS (CONVERT([bit],sign([Flags]&(4)),(0)))
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'AllowMultipleChoices')
begin
	alter table [{databaseOwner}].[{objectQualifier}Poll] add [AllowMultipleChoices] AS (CONVERT([bit],sign([Flags]&(8)),(0)))
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'ShowVoters')
begin
	alter table [{databaseOwner}].[{objectQualifier}Poll] add [ShowVoters] AS (CONVERT([bit],sign([Flags]&(16)),(0)))
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'AllowSkipVote')
begin
	alter table [{databaseOwner}].[{objectQualifier}Poll] add [AllowSkipVote] AS (CONVERT([bit],sign([Flags]&(32)),(0)))
end
GO

 -- PollGroupTable
 if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}PollGroupCluster]') and name=N'IsBound')
 begin
 	alter table [{databaseOwner}].[{objectQualifier}PollGroupCluster] add [IsBound]	AS (CONVERT([bit],sign([Flags]&(2)),(0)))
 end
GO
 
if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}PollGroupCluster]') and name='Flags')
begin
	grant update on [{databaseOwner}].[{objectQualifier}PollGroupCluster] to public
	exec('update [{databaseOwner}].[{objectQualifier}PollGroupCluster] set Flags = 0 where Flags is null')
	revoke update on [{databaseOwner}].[{objectQualifier}PollGroupCluster] from public
	-- alter table [{databaseOwner}].[{objectQualifier}PollGroupCluster] alter column Flags int not null
	-- alter table [{databaseOwner}].[{objectQualifier}PollGroupCluster] add constraint [DF_{objectQualifier}PollGroupCluster_Flags] default(0) for Flags
end
GO
-- ActiveAccess Table
if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}ActiveAccess]') and name=N'LastActive')
begin
	alter table [{databaseOwner}].[{objectQualifier}ActiveAccess] add [LastActive] datetime NULL
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}ActiveAccess]') and name=N'IsGuestX')
begin
    delete from [{databaseOwner}].[{objectQualifier}ActiveAccess]
	alter table [{databaseOwner}].[{objectQualifier}ActiveAccess] add [IsGuestX] bit NOT NULL
end
GO

-- Choice Table
-- this is a dummy it doesn't work
if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Choice]') and name= N'Choice' AND prec < 255 )
begin
	alter table [{databaseOwner}].[{objectQualifier}Choice] alter column Choice varchar(255) NOT NULL
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Choice]') and name=N'ObjectPath')
begin
	alter table [{databaseOwner}].[{objectQualifier}Choice] add [ObjectPath] nvarchar(255) NULL
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Choice]') and name=N'MimeType')
begin
	alter table [{databaseOwner}].[{objectQualifier}Choice] add [MimeType] varchar(50) NULL
end
GO

-- EventLog Table
if not exists(select top 1 1 from dbo.syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]') and name=N'Type')
begin
	alter table [{databaseOwner}].[{objectQualifier}EventLog] add Type int not null constraint [DF_{objectQualifier}EventLog_Type] default (0)
	exec('update [{databaseOwner}].[{objectQualifier}EventLog] set Type = 0')
end
GO

if exists(select top 1 1 from dbo.syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]') and name=N'UserID' and isnullable=0)
	alter table [{databaseOwner}].[{objectQualifier}EventLog] alter column UserID int null
GO
	

-- Smiley Table
if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Smiley]') and name='SortOrder')
begin
	alter table [{databaseOwner}].[{objectQualifier}Smiley] add SortOrder tinyint NOT NULL DEFAULT 0
end
GO

-- Category Table
IF NOT exists (select top 1 1 from dbo.syscolumns WHERE id = Object_id(N'[{databaseOwner}].[{objectQualifier}Category]') AND name = N'CategoryImage')
BEGIN
    ALTER TABLE [{databaseOwner}].[{objectQualifier}Category] ADD [CategoryImage] [nvarchar](255) NULL
END
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Category]') and name='PollGroupID')
	alter table [{databaseOwner}].[{objectQualifier}Category] add PollGroupID int NULL
GO


-- MessageReportedAudit Table
IF NOT exists (select top 1 1 from dbo.syscolumns WHERE id = Object_id(N'[{databaseOwner}].[{objectQualifier}MessageReportedAudit]') AND name = N'ReportedNumber')
BEGIN
    ALTER TABLE [{databaseOwner}].[{objectQualifier}MessageReportedAudit] ADD [ReportedNumber] int NOT NULL DEFAULT 1
END
GO

IF NOT exists (select top 1 1 from dbo.syscolumns WHERE id = Object_id(N'[{databaseOwner}].[{objectQualifier}MessageReportedAudit]') AND name = N'ReportText')
BEGIN
    ALTER TABLE [{databaseOwner}].[{objectQualifier}MessageReportedAudit] ADD [ReportText] nvarchar(4000)  NULL 
END
GO

-- BannedIP Table

IF NOT exists (select top 1 1 from dbo.syscolumns WHERE id = Object_id(N'[{databaseOwner}].[{objectQualifier}BannedIP]') AND name = N'Reason')
BEGIN
    ALTER TABLE [{databaseOwner}].[{objectQualifier}BannedIP] ADD [Reason] nvarchar(128)  NULL 
END
GO

IF NOT exists (select top 1 1 from dbo.syscolumns WHERE id = Object_id(N'[{databaseOwner}].[{objectQualifier}BannedIP]') AND name = N'UserID')
BEGIN
    ALTER TABLE [{databaseOwner}].[{objectQualifier}BannedIP] ADD [UserID] int  null 
END
GO

IF NOT exists (select top 1 1 from dbo.syscolumns WHERE id = Object_id(N'[{databaseOwner}].[{objectQualifier}BannedIP]') AND name = N'Mask' AND prec < 56)
BEGIN
    ALTER TABLE [{databaseOwner}].[{objectQualifier}BannedIP] alter column [Mask] varchar(57) not  null 
END
GO

-- PollVote Table

if exists(select top 1 1 from dbo.syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}PollVote]') and name=N'RemoteIP' and prec<39)
    -- vzrus: should drop the index to change the field
    if exists(select * from dbo.sysindexes where id=object_id('[{databaseOwner}].[{objectQualifier}PollVote]') and name='IX_{objectQualifier}PollVote_RemoteIP')
    begin
    begin
    drop index [{databaseOwner}].[{objectQualifier}PollVote].[IX_{objectQualifier}PollVote_RemoteIP]
    end
	alter table [{databaseOwner}].[{objectQualifier}PollVote] alter column [RemoteIP] varchar(39) null
	end
GO	

IF NOT exists (select top 1 1 from dbo.syscolumns WHERE id = Object_id(N'[{databaseOwner}].[{objectQualifier}PollVote]') AND name = N'ChoiceID')
BEGIN
    ALTER TABLE [{databaseOwner}].[{objectQualifier}PollVote] ADD [ChoiceID] int  null 
END
GO


-- MessageHistory Table

if exists(select top 1 1 from dbo.syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}MessageHistory]') and name=N'IP' and prec<39)
	alter table [{databaseOwner}].[{objectQualifier}MessageHistory] alter column [IP] varchar(39) not null
GO

-- NntpServer Table

if exists(select top 1 1 from dbo.syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}NntpServer]') and name=N'UserName' and prec<255)
	alter table [{databaseOwner}].[{objectQualifier}NntpServer] alter column [UserName] nvarchar(255) null
GO

if exists(select top 1 1 from dbo.syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}User]') and name=N'Email' and prec<255)
	alter table [{databaseOwner}].[{objectQualifier}User] alter column [Email] nvarchar(255) null
GO

if exists(select top 1 1 from dbo.syscolumns where id = object_id(N'[{databaseOwner}].[{objectQualifier}CheckEmail]') and name=N'Email' and prec<255)
	alter table [{databaseOwner}].[{objectQualifier}CheckEmail] alter column [Email] nvarchar(255) null
GO

-- Create Topic Read Tracking Table

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}TopicReadTracking]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}TopicReadTracking](
		UserID			int NOT NULL ,
		TopicID			int NOT NULL ,
		LastAccessDate	datetime NOT NULL
	)
GO

-- Create Forum Read Tracking Table

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}ForumReadTracking]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[{objectQualifier}ForumReadTracking](
		UserID			int NOT NULL ,
		ForumID			int NOT NULL ,
		LastAccessDate	datetime NOT NULL
	)
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='Status')
begin
	alter table [{databaseOwner}].[{objectQualifier}Topic] add [Status] nvarchar(255) null	
end
GO


if exists(select top 1 1 from dbo.sysobjects where name='PK_{objectQualifier}ForumReadTracking')
	alter table [{databaseOwner}].[{objectQualifier}ForumReadTracking] drop constraint [PK_{objectQualifier}ForumReadTracking] 
go 

if exists(select top 1 1 from dbo.sysobjects where name='PK_{objectQualifier}TopicReadTracking')
	alter table [{databaseOwner}].[{objectQualifier}TopicReadTracking] drop constraint [PK_{objectQualifier}TopicReadTracking] 
go 

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}ForumReadTracking]') and name='TrackingID')
begin
	alter table [{databaseOwner}].[{objectQualifier}ForumReadTracking] drop column TrackingID
end
GO

if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}TopicReadTracking]') and name='TrackingID')
begin
	alter table [{databaseOwner}].[{objectQualifier}TopicReadTracking] drop column TrackingID
end
GO

if not exists (select top 1 1 from sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}TopicStatus]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [{databaseOwner}].[{objectQualifier}TopicStatus](
	    TopicStatusID int IDENTITY(1,1) NOT NULL,
		TopicStatusName nvarchar(100) NOT NULL,
		BoardID int NOT NULL,
		DefaultDescription nvarchar(100) NOT NULL,
		CONSTRAINT [PK_{objectQualifier}TopicStatus] PRIMARY KEY(TopicStatusID)
	)
END
GO
exec('[{databaseOwner}].[{objectQualifier}drop_defaultconstraint_oncolumn] {objectQualifier}User, Culture')
GO

-- Add 8-letter Language Code column
if exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Culture' and prec=5)
begin
	alter table [{databaseOwner}].[{objectQualifier}User] alter column [Culture] varchar(10) NULL
end
GO

if not exists (select top 1 1 from syscolumns where id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='Styles')
	alter table [{databaseOwner}].[{objectQualifier}Topic] add Styles nvarchar(255) NULL
GO