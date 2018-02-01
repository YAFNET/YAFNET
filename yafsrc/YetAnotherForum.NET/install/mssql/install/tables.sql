CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}drop_defaultconstraint_oncolumn](@tablename varchar(255), @columnname varchar(255)) as
BEGIN
DECLARE @DefName sysname

SELECT 
  @DefName = o1.name
FROM
  sys.objects o1
  INNER JOIN sys.columns c ON
  o1.object_id = c.default_object_id
  INNER JOIN sys.objects o2 ON
  c.object_id = o2.object_id
WHERE
  o2.name = @tablename AND
  c.name = @columnname
  
IF @DefName IS NOT NULL
  EXECUTE ('ALTER TABLE [{databaseOwner}].[' + @tablename + '] DROP constraint [' + @DefName + ']')
END
GO

/*
** Create missing tables
*/

/* Create Thanks Table */
if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Thanks]') and type in (N'U'))
CREATE TABLE [{databaseOwner}].[{objectQualifier}Thanks](
	[ThanksID] [int] IDENTITY(1,1) NOT NULL,
	[ThanksFromUserID] [int] NOT NULL,
	[ThanksToUserID] [int] NOT NULL,
	[MessageID] [int] NOT NULL,
	[ThanksDate] [smalldatetime] NOT NULL,
	constraint [PK_{objectQualifier}Thanks] primary key(ThanksID)
	)
go

/* YAF Buddy Table */
if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Buddy]') and type in (N'U'))
CREATE TABLE [{databaseOwner}].[{objectQualifier}Buddy](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FromUserID] [int] NOT NULL,
	[ToUserID] [int] NOT NULL,
	[Approved] [bit] NOT NULL,
	[Requested] [datetime] NOT NULL,
	constraint [PK_{objectQualifier}Buddy] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF),
 constraint [IX_{objectQualifier}Buddy] UNIQUE NONCLUSTERED 
(
	[FromUserID] ASC,
	[ToUserID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
go

/* YAF FavoriteTopic Table */
if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}FavoriteTopic]') and type in (N'U'))
CREATE TABLE [{databaseOwner}].[{objectQualifier}FavoriteTopic](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[TopicID] [int] NOT NULL,
	constraint [PK_{objectQualifier}FavoriteTopic] primary key(ID)
	)
GO

/* YAF Album Tables*/
if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}UserAlbum]') and type in (N'U'))
CREATE TABLE [{databaseOwner}].[{objectQualifier}UserAlbum](
	[AlbumID] [INT] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[Title] [NVARCHAR](255),
	[CoverImageID] [INT],
	[Updated] [DATETIME] NOT NULL,
	constraint [PK_{objectQualifier}User_Album] primary key(AlbumID)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}UserAlbumImage]') and type in (N'U'))
CREATE TABLE [{databaseOwner}].[{objectQualifier}UserAlbumImage](
	[ImageID] [INT] IDENTITY(1,1) NOT NULL,
	[AlbumID] [int] NOT NULL,
	[Caption] [NVARCHAR](255),
	[FileName] [NVARCHAR](255) NOT NULL,
	[Bytes] [INT] NOT NULL,
	[ContentType] [NVARCHAR](50),
	[Uploaded] [DATETIME] NOT NULL,
	[Downloads] [INT] NOT NULL,
	constraint [PK_{objectQualifier}User_AlbumImage] primary key(ImageID)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Active]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}Active](
		SessionID		nvarchar (24) NOT NULL,
		BoardID			int NOT NULL,
		UserID			int NOT NULL,
		IP				varchar (39) NOT NULL,
		[Login]			datetime NOT NULL,
		LastActive		datetime NOT NULL,
		Location		nvarchar (255) NOT NULL,
		ForumID			int NULL,
		TopicID			int NULL,
		Browser			nvarchar (50) NULL,
		[Platform]		nvarchar (50) NULL,
		Flags           int NULL,
		ForumPage       nvarchar(1024) NULL,
        constraint [PK_{objectQualifier}Active] PRIMARY KEY CLUSTERED 
(
	[SessionID] ASC,
	[BoardID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
go

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}ActiveAccess]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}ActiveAccess](		
		UserID			    int NOT NULL,
		BoardID			    int NOT NULL,			
		ForumID			    int NOT NULL,
		IsAdmin				bit NOT NULL,
		IsForumModerator	bit NOT NULL,
		IsModerator			bit NOT NULL,
		ReadAccess			bit NOT NULL,
		PostAccess			bit NOT NULL,
		ReplyAccess			bit NOT NULL,
		PriorityAccess		bit NOT NULL,
		PollAccess			bit NOT NULL,
		VoteAccess			bit NOT NULL,
		ModeratorAccess		bit NOT NULL,
		EditAccess			bit NOT NULL,
		DeleteAccess		bit NOT NULL,
		UploadAccess		bit NOT NULL,		
		DownloadAccess		bit NOT NULL,
		LastActive			datetime NULL,
		IsGuestX			bit NOT NULL,
        constraint [PK_{objectQualifier}ActiveAccess] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ForumID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)	
	)
go

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}AdminPageUserAccess]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}AdminPageUserAccess](
		UserID		    int NOT NULL,		
		PageName		nvarchar (128) NOT NULL,
 constraint [PK_{objectQualifier}AdminPageUserAccess] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[PageName] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
go

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}EventLogGroupAccess]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}EventLogGroupAccess](
		GroupID		    int NOT NULL,	
		EventTypeID     int NOT NULL,  	
		EventTypeName	nvarchar (128) NOT NULL,
		DeleteAccess    bit NOT NULL,
 constraint [PK_{objectQualifier}EventLogGroupAccess] PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC,
	[EventTypeID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
go

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}BannedIP]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}BannedIP](
		ID				int IDENTITY (1,1) NOT NULL,
		BoardID			int NOT NULL,
		Mask			nvarchar (15) NOT NULL,
		Since			datetime NOT NULL,
		Reason          nvarchar (128) NULL,
		UserID			int null,
 constraint [PK_{objectQualifier}BannedIP] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF),
 constraint [IX_{objectQualifier}BannedIP] UNIQUE NONCLUSTERED 
(
	[BoardID] ASC,
	[Mask] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
go

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}BannedName]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}BannedName](
		ID				int IDENTITY (1,1) NOT NULL,
		BoardID			int NOT NULL,
		Mask			nvarchar (255) NOT NULL,
		Since			datetime NOT NULL,
		Reason          nvarchar (128) NULL,
 constraint [PK_{objectQualifier}BannedName] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF),
 constraint [IX_{objectQualifier}BannedName] UNIQUE NONCLUSTERED 
(
	[BoardID] ASC,
	[Mask] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
go

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}BannedEmail]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}BannedEmail](
		ID				int IDENTITY (1,1) NOT NULL,
		BoardID			int NOT NULL,
		Mask			nvarchar (255) NOT NULL,
		Since			datetime NOT NULL,
		Reason          nvarchar (128) NULL,
 constraint [PK_{objectQualifier}BannedEmail] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF),
 constraint [IX_{objectQualifier}BannedEmail] UNIQUE NONCLUSTERED 
(
	[BoardID] ASC,
	[Mask] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
go

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Category]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}Category](
		CategoryID		int IDENTITY (1,1) NOT NULL,
		BoardID			int NOT NULL,
		[Name]			[nvarchar](128) NOT NULL,
		[CategoryImage] [nvarchar](255) NULL,		
		SortOrder		smallint NOT NULL,
		PollGroupID     int null,
 constraint [PK_{objectQualifier}Category] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF),
 constraint [IX_{objectQualifier}Category] UNIQUE NONCLUSTERED 
(
	[BoardID] ASC,
	[Name] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}CheckEmail]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}CheckEmail](
		CheckEmailID	int IDENTITY (1,1) NOT NULL,
		UserID			int NOT NULL,
		Email			nvarchar (255) NOT NULL,
		Created			datetime NOT NULL,
		[Hash]			nvarchar (32) NOT NULL,
 constraint [PK_{objectQualifier}CheckEmail] PRIMARY KEY CLUSTERED 
(
	[CheckEmailID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF),
 constraint [IX_{objectQualifier}CheckEmail] UNIQUE NONCLUSTERED 
(
	[Hash] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Choice]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}Choice](
		ChoiceID		int IDENTITY (1,1) NOT NULL,
		PollID			int NOT NULL,
		Choice			nvarchar (50) NOT NULL,
		Votes			int NOT NULL,
		[ObjectPath] nvarchar(255) NULL,
		[MimeType] varchar(50) NULL,
 constraint [PK_{objectQualifier}Choice] PRIMARY KEY CLUSTERED 
(
	[ChoiceID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}PollVote]') and type in (N'U'))
	CREATE TABLE [{databaseOwner}].[{objectQualifier}PollVote] (
		[PollVoteID] [int] IDENTITY (1,1) NOT NULL,
		[PollID] [int] NOT NULL,
		[UserID] [int] NULL,
		[RemoteIP] [varchar] (39) NULL,
		[ChoiceID] [int] NULL,
 constraint [PK_{objectQualifier}PollVote] PRIMARY KEY CLUSTERED 
(
	[PollVoteID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}PollVoteRefuse]') and type in (N'U'))
	CREATE TABLE [{databaseOwner}].[{objectQualifier}PollVoteRefuse] (
		[RefuseID] [int] IDENTITY (1,1) NOT NULL,		
		[PollID] [int] NOT NULL,
		[UserID] [int] NULL,
		[RemoteIP] [varchar] (57) NULL
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Forum]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}Forum](
		ForumID			int IDENTITY (1,1) NOT NULL,
		CategoryID		int NOT NULL,
		ParentID		int NULL,
		Name			nvarchar (50) NOT NULL,
		[Description]	nvarchar (255) NULL,
		SortOrder		smallint NOT NULL,
		LastPosted		datetime NULL,
		LastTopicID		int NULL,
		LastMessageID	int NULL,
		LastUserID		int NULL,
		LastUserName	nvarchar (255) NULL,
		LastUserDisplayName	nvarchar (255) NULL,
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
		ImageURL        nvarchar(128) NULL,
	    Styles          nvarchar(255) NULL,
		UserID          int null,
		ModeratedPostCount int null,
		IsModeratedNewTopicOnly	bit not null constraint [DF_{objectQualifier}Forum_IsModeratedNewTopicOnly] default (0),
 constraint [PK_{objectQualifier}Forum] PRIMARY KEY CLUSTERED 
(
	[ForumID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}ForumAccess]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}ForumAccess](
		GroupID			int NOT NULL,
		ForumID			int NOT NULL,
		AccessMaskID	int NOT NULL,
 constraint [PK_{objectQualifier}ForumAccess] PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC,
	[ForumID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Group]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}Group](
		GroupID		   int IDENTITY (1,1) NOT NULL,
		BoardID		   int NOT NULL,
		[Name]		   nvarchar (255) NOT NULL,
		Flags		   int not null constraint [DF_{objectQualifier}Group_Flags] default (0),
		PMLimit        int NOT NULL	constraint [DF_{objectQualifier}Group_PMLimit] default (0),
	    Style          nvarchar(255) NULL,
	    SortOrder      smallint NOT NULL constraint [DF_{objectQualifier}Group_SortOrder] default (0),
	    [Description]  nvarchar(128) NULL,
	    UsrSigChars    int NOT NULL constraint [DF_{objectQualifier}Group_UsrSigChars] default (0),
	    UsrSigBBCodes  nvarchar(255) NULL,
	    UsrSigHTMLTags nvarchar(255) NULL,
	    UsrAlbums      int NOT NULL constraint [DF_{objectQualifier}Group_UsrAlbums] default (0),
	    UsrAlbumImages int NOT NULL constraint [DF_{objectQualifier}Group_UsrAlbumImages]  default (0),
	    IsHidden       AS (CONVERT([bit],sign([Flags]&(16)),(0))),
	    IsUserGroup    AS (CONVERT([bit],sign([Flags]&(32)),(0))),
 constraint [PK_{objectQualifier}Group] PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF),
 constraint [IX_{objectQualifier}Group] UNIQUE NONCLUSTERED 
(
	[BoardID] ASC,
	[Name] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Mail]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}Mail](
		[MailID] [int] IDENTITY(1,1) NOT NULL,
		[FromUser] [nvarchar](255) NOT NULL,
		[FromUserName] [nvarchar](255) NULL,
		[ToUser] [nvarchar](255) NOT NULL,
		[ToUserName] [nvarchar](255) NULL,
		[Created] [datetime] NOT NULL,
		[Subject] [nvarchar](100) NOT NULL,
		[Body] [nvarchar](max) NOT NULL,
		[BodyHtml] [nvarchar](max) NULL,
		[SendTries] [int] NOT NULL constraint [DF_{objectQualifier}Mail_SendTries]  default (0),
		[SendAttempt] [datetime] NULL,
		[ProcessID] [int] NULL,
 constraint [PK_{objectQualifier}Mail] PRIMARY KEY CLUSTERED 
(
	[MailID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Message]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}Message](
		MessageID		    int IDENTITY (1,1) NOT NULL,
		TopicID			    int NOT NULL,
		ReplyTo			    int NULL,
		Position		    int NOT NULL,
		Indent			    int NOT NULL,
		UserID			    int NOT NULL,
		UserName		    nvarchar (255) NULL,
		UserDisplayName		nvarchar (255) NULL,
		Posted			    datetime NOT NULL,
		[Message]		    nvarchar(max) NOT NULL,
		IP				    varchar (39) NOT NULL,
		Edited			    datetime NULL,
		Flags			    int NOT NULL,
		EditReason          nvarchar (100) NULL,
		IsModeratorChanged  bit NOT NULL constraint [DF_{objectQualifier}Message_IsModeratorChanged] default (0),
	    DeleteReason        nvarchar (100)  NULL,
		ExternalMessageId	nvarchar(255) NULL,
		ReferenceMessageId	nvarchar(255) NULL,
		IsDeleted		    AS (CONVERT([bit],sign([Flags]&(8)),0)),
		IsApproved		    AS (CONVERT([bit],sign([Flags]&(16)),(0))),
		BlogPostID          nvarchar(50) NULL,
	    EditedBy            int NULL,
 constraint [PK_{objectQualifier}Message] PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}MessageHistory]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}MessageHistory](
		MessageID		    int NOT NULL,
		[Message]		    nvarchar(max) NOT NULL,
		IP				    varchar (39) NOT NULL,
		Edited			    datetime NOT NULL,
		EditedBy		    int NULL,	
		EditReason          nvarchar (100) NULL,
		IsModeratorChanged  bit NOT NULL constraint [DF_{objectQualifier}MessageHistory_IsModeratorChanged] default (0),
		Flags               int NOT NULL constraint [DF_{objectQualifier}MessageHistory_Flags] default (23),
 constraint [PK_{objectQualifier}MessageHistory] PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC,
	[Edited] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF) 
	)
GO

exec('[{databaseOwner}].[{objectQualifier}drop_defaultconstraint_oncolumn] {objectQualifier}MessageHistory, MessageHistoryID')
GO

IF NOT EXISTS (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}MessageReported]') and type in (N'U'))
	CREATE TABLE [{databaseOwner}].[{objectQualifier}MessageReported](
		[MessageID] [int] NOT NULL,
		[Message] [nvarchar](max) NULL,
		[Resolved] [bit] NULL,
		[ResolvedBy] [int] NULL,
		[ResolvedDate] [datetime] NULL,
 constraint [PK_{objectQualifier}MessageReported] PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

IF NOT EXISTS (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}MessageReportedAudit]') and type in (N'U'))
	CREATE TABLE [{databaseOwner}].[{objectQualifier}MessageReportedAudit](
		[LogID] [int] IDENTITY(1,1) NOT NULL,
		[UserID] [int] NULL,
		[MessageID] [int] NOT NULL,
		[Reported] [datetime] NULL,
	    [ReportedNumber] [int] NOT NULL default (1),
	    [ReportText] [nvarchar](4000) NULL
		)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}PMessage]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}PMessage](
		PMessageID		int IDENTITY (1,1) NOT NULL,
		FromUserID      int NOT NULL,
		ReplyTo			int NULL,
		Created			datetime NOT NULL,
		[Subject]		nvarchar (100) NOT NULL,
		Body			nvarchar(max) NOT NULL,
		Flags			int NOT NULL constraint [DF_{objectQualifier}Message_Flags] default (23),
 constraint [PK_{objectQualifier}PMessage] PRIMARY KEY CLUSTERED 
(
	[PMessageID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}PollGroupCluster]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}PollGroupCluster](		
		PollGroupID int IDENTITY (1,1) NOT NULL,
		UserID	    int not NULL,
		[Flags]     int NOT NULL constraint [DF_{objectQualifier}PollGroupCluster_Flags] default (0),
		[IsBound]   AS (CONVERT([bit],sign([Flags]&(2)),(0)))
 constraint [PK_{objectQualifier}PollGroupCluster] PRIMARY KEY CLUSTERED 
(
	[PollGroupID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)	
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Poll]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}Poll](
		PollID			       int IDENTITY (1,1) NOT NULL,
		Question		       nvarchar (50) NOT NULL,
		Closes                 datetime NULL,		
		PollGroupID            int NULL,
		UserID                 int not NULL constraint [DF_{objectQualifier}Poll_UserID] default (1),	
		[ObjectPath]           nvarchar(255) NULL,
		[MimeType]             varchar(50) NULL,
		[Flags]                int NOT NULL constraint [DF_{objectQualifier}Poll_Flags] default (0),		
		[IsClosedBound] 	   AS (CONVERT([bit],sign([Flags]&(4)),(0))),
		[AllowMultipleChoices] AS (CONVERT([bit],sign([Flags]&(8)),(0))),
		[ShowVoters]           AS (CONVERT([bit],sign([Flags]&(16)),(0))),
		[AllowSkipVote]        AS (CONVERT([bit],sign([Flags]&(32)),(0))),
 constraint [PK_{objectQualifier}Poll] PRIMARY KEY CLUSTERED 
(
	[PollID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Topic]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}Topic](
		TopicID			    int IDENTITY (1,1) NOT NULL,
		ForumID			    int NOT NULL,
		UserID			    int NOT NULL,
		UserName		    nvarchar (255) NULL,
		UserDisplayName		nvarchar (255) NULL,	
		Posted			    datetime NOT NULL,
		Topic			    nvarchar (100) NOT NULL,
		[Description]		nvarchar (255) NULL,
		[Status]	     	nvarchar (255) NULL,
		[Styles]	     	nvarchar (255) NULL,
		[LinkDate]          datetime NULL,
		[Views]			    int NOT NULL,
		[Priority]		    smallint NOT NULL,
		PollID			    int NULL,
		TopicMovedID	    int NULL,
		LastPosted		    datetime NULL,
		LastMessageID	    int NULL,
		LastUserID		    int NULL,
		LastUserName	    nvarchar (255) NULL,
		LastUserDisplayName	nvarchar (255) NULL,	
		NumPosts		    int NOT NULL,
		Flags			    int not null constraint [DF_{objectQualifier}Topic_Flags] default (0),
		IsDeleted		    AS (CONVERT([bit],sign([Flags]&(8)),0)),
		[IsQuestion]        AS (CONVERT([bit],sign([Flags]&(1024)),(0))),
		[AnswerMessageId]   [int] NULL,
		[LastMessageFlags]	[int] NULL,
		[TopicImage]        nvarchar(255) NULL,
 constraint [PK_{objectQualifier}Topic] PRIMARY KEY CLUSTERED 
(
	[TopicID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}User]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}User](
		UserID			int IDENTITY (1,1) NOT NULL,
		BoardID			int NOT NULL,
		ProviderUserKey	nvarchar(64),
		[Name]			nvarchar (255) NOT NULL,
		[DisplayName]	nvarchar (255) NOT NULL,
		[Password]		nvarchar (32) NOT NULL,
		[Email]			nvarchar (255) NULL,
		Joined			datetime NOT NULL,
		LastVisit		datetime NOT NULL,
		IP				nvarchar (39) NULL,
		NumPosts		int NOT NULL,
		TimeZone		string NOT NULL,
		Avatar			nvarchar (255) NULL,
		[Signature]		nvarchar(max) NULL,
		AvatarImage		image NULL,
		AvatarImageType	nvarchar (50) NULL,
		RankID			[int] NOT NULL,
		Suspended		[datetime] NULL,
		SuspendedReason nvarchar(max) NULL,
		SuspendedBy     int not null default (0),
		LanguageFile	nvarchar(50) NULL,
		ThemeFile		nvarchar(50) NULL,
		TextEditor		nvarchar(50) NULL,
		OverridedefaultThemes	bit NOT NULL constraint [DF_{objectQualifier}User_OverridedefaultThemes] default (1),
		[PMNotification] [bit] NOT NULL constraint [DF_{objectQualifier}User_PMNotification] default (1),
		[AutoWatchTopics] [bit] NOT NULL constraint [DF_{objectQualifier}User_AutoWatchTopics] default (0),
		[DailyDigest] [bit] NOT NULL constraint [DF_{objectQualifier}User_DailyDigest] default (0),
		[NotificationType] [int] default (10),
		[Flags] [int]	NOT NULL  constraint [DF_{objectQualifier}User_Flags]  default (0),
		[Points] [int]	NOT NULL constraint [DF_{objectQualifier}User_Points] default (1),		
		[IsApproved]	AS (CONVERT([bit],sign([Flags]&(2)),(0))),
		[IsGuest]	AS (CONVERT([bit],sign([Flags]&(4)),(0))),
		[IsCaptchaExcluded]	AS (CONVERT([bit],sign([Flags]&(8)),(0))),
		[IsActiveExcluded] AS (CONVERT([bit],sign([Flags]&(16)),(0))),
		[IsDST]	AS (CONVERT([bit],sign([Flags]&(32)),(0))),
		[IsDirty]	AS (CONVERT([bit],sign([Flags]&(64)),(0))),
		[Culture] varchar (10) default (10),
		[IsFacebookUser][bit] NOT NULL constraint [DF_{objectQualifier}User_IsFacebookUser] default (0),
		[IsTwitterUser][bit] NOT NULL constraint [DF_{objectQualifier}User_IsTwitterUser] default (0),
		[UserStyle] [varchar](510) NULL,
	    [StyleFlags] [int] NOT NULL constraint [DF_{objectQualifier}User_StyleFlags] default (0),
	    [IsUserStyle]  AS (CONVERT([bit],sign([StyleFlags]&(1)),(0))),
	    [IsGroupStyle]  AS (CONVERT([bit],sign([StyleFlags]&(2)),(0))),
	    [IsRankStyle]  AS (CONVERT([bit],sign([StyleFlags]&(4)),(0))),
		[IsGoogleUser][bit] NOT NULL constraint [DF_{objectQualifier}User_IsGoogleUser] default (0),
 constraint [PK_{objectQualifier}User] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF),
 constraint [IX_{objectQualifier}User] UNIQUE NONCLUSTERED 
(
	[BoardID] ASC,
	[Name] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO

IF not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}UserProfile]') and type in (N'U'))
	CREATE TABLE [{databaseOwner}].[{objectQualifier}UserProfile]
	(
		[UserID] [int] NOT NULL,
		[LastUpdatedDate] [datetime] NOT NULL,
		-- added columns
		[LastActivity] [datetime],
		[ApplicationName] [nvarchar](255) NOT NULL,	
		[IsAnonymous] [bit] NOT NULL,
		[UserName] [nvarchar](255) NOT NULL,
 constraint [PK_{objectQualifier}UserProfile] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ApplicationName] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF),
 constraint [IX_{objectQualifier}UserProfile] UNIQUE NONCLUSTERED 
(
	[UserID] ASC,
	[ApplicationName] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}WatchForum]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}WatchForum](
		WatchForumID	int IDENTITY (1,1) NOT NULL,
		ForumID			int NOT NULL,
		UserID			int NOT NULL,
		Created			datetime NOT NULL,
		LastMail		datetime null,
 constraint [PK_{objectQualifier}WatchForum] PRIMARY KEY CLUSTERED 
(
	[WatchForumID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF),
 constraint [IX_{objectQualifier}WatchForum] UNIQUE NONCLUSTERED 
(
	[ForumID] ASC,
	[UserID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}WatchTopic]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}WatchTopic](
		WatchTopicID	int IDENTITY (1,1) NOT NULL,
		TopicID			int NOT NULL,
		UserID			int NOT NULL,
		Created			datetime NOT NULL,
		LastMail		datetime null,
 constraint [PK_{objectQualifier}WatchTopic] PRIMARY KEY CLUSTERED 
(
	[WatchTopicID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF),
 constraint [IX_{objectQualifier}WatchTopic] UNIQUE NONCLUSTERED 
(
	[TopicID] ASC,
	[UserID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Attachment]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}Attachment](
		AttachmentID	int IDENTITY (1,1) not null,
		MessageID		int not null default (0),
		UserID          int not null default (0),		
		[FileName]		nvarchar(255) not null,
		Bytes			int not null,
		ContentType		nvarchar(max) null,
		Downloads		int not null,
		FileData		image null,
 constraint [PK_{objectQualifier}Attachment] PRIMARY KEY CLUSTERED 
(
	[AttachmentID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}UserGroup]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}UserGroup](
		UserID			int NOT NULL,
		GroupID			int NOT NULL,
 constraint [PK_{objectQualifier}UserGroup] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[GroupID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Rank]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}Rank](
		RankID			 int IDENTITY (1,1) NOT NULL,
		BoardID			 int NOT NULL,
		Name			 nvarchar (50) NOT NULL,
		MinPosts		 int NULL,
		RankImage		 nvarchar (50) NULL,
		Flags			 int not null constraint [DF_{objectQualifier}Rank_Flags] default (0),
	    [PMLimit]        [int] NULL,
	    [Style]          [nvarchar](255) NULL,
	    [SortOrder]      [smallint] NOT NULL constraint [DF_{objectQualifier}Rank_SortOrder] default (0),
	    [Description]    [nvarchar](128) NULL,
	    [UsrSigChars]    [int] NOT NULL constraint [DF_{objectQualifier}Rank_UsrSigChars] default (0),
	    [UsrSigBBCodes]  [nvarchar](255) NULL,
	    [UsrSigHTMLTags] [nvarchar](255) NULL,
	    [UsrAlbums]      [int] NOT NULL constraint [DF_{objectQualifier}Rank_UsrAlbums] default (0),
	    [UsrAlbumImages] [int] NOT NULL constraint [DF_{objectQualifier}Rank_UsrAlbumImages]  default (0),
 constraint [PK_{objectQualifier}Rank] PRIMARY KEY CLUSTERED 
(
	[RankID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF),
 constraint [IX_{objectQualifier}Rank] UNIQUE NONCLUSTERED 
(
	[BoardID] ASC,
	[Name] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}AccessMask]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}AccessMask](
		AccessMaskID	int IDENTITY (1,1) NOT NULL,
		BoardID			int NOT NULL,
		Name			nvarchar(50) NOT NULL,
		Flags			int not null constraint [DF_{objectQualifier}AccessMask_Flags] default (0),
	    [SortOrder]     [smallint] NOT NULL default (0),
 constraint [PK_{objectQualifier}AccessMask] PRIMARY KEY CLUSTERED 
(
	[AccessMaskID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}UserForum]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}UserForum](
		UserID			int NOT NULL,
		ForumID			int NOT NULL,
		AccessMaskID	int NOT NULL,
		Invited			datetime NOT NULL,
		Accepted		bit NOT NULL,
 constraint [PK_{objectQualifier}UserForum] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ForumID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Board]') and type in (N'U'))
begin
	create table [{databaseOwner}].[{objectQualifier}Board](
		BoardID			int IDENTITY (1,1) NOT NULL,
		Name			nvarchar(50) NOT NULL,
		AllowThreaded	bit NOT NULL,
		MembershipAppName nvarchar(255) NULL,
		RolesAppName nvarchar(255) NULL,
 constraint [PK_{objectQualifier}Board] PRIMARY KEY CLUSTERED 
(
	[BoardID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
end
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}NntpServer]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}NntpServer](
		NntpServerID	int IDENTITY (1,1) not null,
		BoardID			int NOT NULL,
		Name			nvarchar(50) not null,
		[Address]		nvarchar(100) not null,
		Port			int null,
		UserName		nvarchar(255) null,
		UserPass		nvarchar(50) null,
 constraint [PK_{objectQualifier}NntpServer] PRIMARY KEY CLUSTERED 
(
	[NntpServerID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
		
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}NntpForum]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}NntpForum](
		NntpForumID		int IDENTITY (1,1) not null,
		NntpServerID	int not null,
		GroupName		nvarchar(100) not null,
		ForumID			int not null,
		LastMessageNo	int not null,
		LastUpdate		datetime not null,
		Active			bit not null,
		DateCutOff		datetime null,
 constraint [PK_{objectQualifier}NntpForum] PRIMARY KEY CLUSTERED 
(
	[NntpForumID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}NntpTopic]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}NntpTopic](
		NntpTopicID		int IDENTITY (1,1) not null,
		NntpForumID		int not null,
		Thread			nvarchar(64) not null,
		TopicID			int not null,
 constraint [PK_{objectQualifier}NntpTopic] PRIMARY KEY CLUSTERED 
(
	[NntpTopicID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}UserPMessage]') and type in (N'U'))
begin
	create table [{databaseOwner}].[{objectQualifier}UserPMessage](
		UserPMessageID	int IDENTITY (1,1) not null,
		UserID			int not null,
		PMessageID		int not null,
		[Flags]			int NOT NULL constraint [DF_{objectQualifier}UserPMessage_Flags]  default (0),
		[IsRead]		AS (CONVERT([bit],sign([Flags]&(1)),(0))),
		[IsInOutbox]	AS (CONVERT([bit],sign([Flags]&(2)),(0))),
		[IsArchived]	AS (CONVERT([bit],sign([Flags]&(4)),(0))),
		[IsDeleted]		AS (CONVERT([bit],sign([Flags]&(8)),(0))),
		[IsReply]		[bit] NOT NULL  default (0)		,
 constraint [PK_{objectQualifier}UserPMessage] PRIMARY KEY CLUSTERED 
(
	[UserPMessageID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
end
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Replace_Words]') and type in (N'U'))
begin
	create table [{databaseOwner}].[{objectQualifier}Replace_Words](
		ID				int IDENTITY (1,1) NOT NULL,
		BoardId			int NOT NULL constraint [DF_{objectQualifier}Replace_Words_BoardID] default (1),
		BadWord			nvarchar (255) NULL,
		GoodWord		nvarchar (255) NULL,
 constraint [PK_{objectQualifier}Replace_Words] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
end
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Spam_Words]') and type in (N'U'))
begin
	create table [{databaseOwner}].[{objectQualifier}Spam_Words](
		ID				int IDENTITY (1,1) NOT NULL,
		BoardId			int NOT NULL,
		SpamWord			nvarchar (255) NULL,
		constraint [PK_{objectQualifier}Spam_Words] primary key(ID)
	)
end
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Registry]') and type in (N'U'))
begin
	create table [{databaseOwner}].[{objectQualifier}Registry](
		RegistryID		int IDENTITY(1,1) NOT NULL,
		Name			nvarchar(50) NOT NULL,
		Value			nvarchar(max),
		BoardID			int,
		constraint [PK_{objectQualifier}Registry] PRIMARY KEY (RegistryID)
	)
end
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}EventLog]') and type in (N'U'))
begin
	create table [{databaseOwner}].[{objectQualifier}EventLog](
		EventLogID	int identity(1,1) not null,
		EventTime	datetime not null constraint [DF_{objectQualifier}EventLog_EventTime] default GETUTCDATE() ,
		UserID		int, -- deprecated
		UserName	nvarchar(100) null,
		[Source]	nvarchar(50) not null,
		Description	nvarchar(max) not null,
		[Type] [int] NOT NULL constraint [DF_{objectQualifier}EventLog_Type] default (0),
		constraint [PK_{objectQualifier}EventLog] primary key(EventLogID)
	)
end
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Extension]') and type in (N'U'))
BEGIN
	CREATE TABLE [{databaseOwner}].[{objectQualifier}Extension](
		ExtensionID int IDENTITY(1,1) NOT NULL,
		BoardId int NOT NULL,
		Extension nvarchar(10) NOT NULL,
		constraint [PK_{objectQualifier}Extension] PRIMARY KEY(ExtensionID)
	)
END
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}BBCode]') and type in (N'U'))
begin
	create table [{databaseOwner}].[{objectQualifier}BBCode](
		[BBCodeID] [int] IDENTITY(1,1) NOT NULL,
		[BoardID] [int] NOT NULL,
		[Name] [nvarchar](255) NOT NULL,
		[Description] [nvarchar](4000) NULL,
		[OnClickJS] [nvarchar](1000) NULL,
		[DisplayJS] [nvarchar](max) NULL,
		[EditJS] [nvarchar](max) NULL,
		[DisplayCSS] [nvarchar](max) NULL,
		[SearchRegex] [nvarchar](max) NULL,
		[ReplaceRegex] [nvarchar](max) NULL,
		[Variables] [nvarchar](1000) NULL,
		[UseModule] [bit] NULL,
		[ModuleClass] [nvarchar](255) NULL,		
		[ExecOrder] [int] NOT NULL,
		constraint [PK_{objectQualifier}BBCode] PRIMARY KEY (BBCodeID)
	)
end
GO


if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Medal]') and type in (N'U'))
begin
	create table [{databaseOwner}].[{objectQualifier}Medal](
		[BoardID] [int] NOT NULL,
		[MedalID] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](100) NOT NULL,
		[Description] [nvarchar](max) NOT NULL,
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
		[SortOrder] [tinyint] NOT NULL constraint [DF_{objectQualifier}Medal_defaultOrder]  default ((255)),
		[Flags] [int] NOT NULL constraint [DF_{objectQualifier}Medal_Flags]  default ((0)),
		constraint [PK_{objectQualifier}Medal] PRIMARY KEY CLUSTERED ([MedalID] ASC)
		)
end
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}GroupMedal]') and type in (N'U'))
begin
	create table [{databaseOwner}].[{objectQualifier}GroupMedal](
		[GroupID] [int] NOT NULL,
		[MedalID] [int] NOT NULL,
		[Message] [nvarchar](100) NULL,
		[Hide] [bit] NOT NULL constraint [DF_{objectQualifier}GroupMedal_Hide]  default ((0)),
		[OnlyRibbon] [bit] NOT NULL constraint [DF_{objectQualifier}GroupMedal_OnlyRibbon]  default ((0)),
		[SortOrder] [tinyint] NOT NULL constraint [DF_{objectQualifier}GroupMedal_SortOrder]  default ((255)),
 constraint [PK_{objectQualifier}GroupMedal] PRIMARY KEY CLUSTERED 
(
	[MedalID] ASC,
	[GroupID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
		)
end
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}UserMedal]') and type in (N'U'))
begin
	create table [{databaseOwner}].[{objectQualifier}UserMedal](
		[UserID] [int] NOT NULL,
		[MedalID] [int] NOT NULL,
		[Message] [nvarchar](100) NULL,
		[Hide] [bit] NOT NULL constraint [DF_{objectQualifier}UserMedal_Hide]  default ((0)),
		[OnlyRibbon] [bit] NOT NULL constraint [DF_{objectQualifier}UserMedal_OnlyRibbon]  default ((0)),
		[SortOrder] [tinyint] NOT NULL constraint [DF_{objectQualifier}UserMedal_SortOrder]  default ((255)),
		[DateAwarded] [datetime] NOT NULL,
 constraint [PK_{objectQualifier}UserMedal] PRIMARY KEY CLUSTERED 
(
	[MedalID] ASC,
	[UserID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
end
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}IgnoreUser]') and type in (N'U'))
begin
	CREATE TABLE [{databaseOwner}].[{objectQualifier}IgnoreUser]
	(
		[UserID] int NOT NULL,
		[IgnoredUserID] int NOT NULL,
 constraint [PK_{objectQualifier}IgnoreUser] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[IgnoredUserID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
end
GO

-- Create Topic Read Tracking Table

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}TopicReadTracking]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}TopicReadTracking](
		UserID			int NOT NULL,
		TopicID			int NOT NULL,
		LastAccessDate	datetime NOT NULL
 constraint [PK_{objectQualifier}TopicReadTracking] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[TopicID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

-- Create Forum Read Tracking Table

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}ForumReadTracking]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}ForumReadTracking](
		UserID			int NOT NULL,
		ForumID			int NOT NULL,
		LastAccessDate	datetime NOT NULL,
 constraint [PK_{objectQualifier}ForumReadTracking] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[ForumID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}TopicStatus]') and type in (N'U'))
BEGIN
	CREATE TABLE [{databaseOwner}].[{objectQualifier}TopicStatus](
	    TopicStatusID int IDENTITY(1,1) NOT NULL,
		TopicStatusName nvarchar(100) NOT NULL,
		BoardID int NOT NULL,
		defaultDescription nvarchar(100) NOT NULL,
		constraint [PK_{objectQualifier}TopicStatus] PRIMARY KEY(TopicStatusID)
	)
END
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}ReputationVote]') and type in (N'U'))
	create table [{databaseOwner}].[{objectQualifier}ReputationVote](
		ReputationFromUserID  int NOT NULL,
		ReputationToUserID	  int NOT NULL,
		VoteDate	datetime NOT NULL,
 constraint [PK_{objectQualifier}ReputationVote] PRIMARY KEY CLUSTERED 
(
	[ReputationFromUserID] ASC,
	[ReputationToUserID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
GO

if not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}ShoutboxMessage]') and type in (N'U'))
begin
	CREATE TABLE [{databaseOwner}].[{objectQualifier}ShoutboxMessage](
		[ShoutBoxMessageID] [int] IDENTITY(1,1) NOT NULL,		
		[BoardId] [int] NOT NULL constraint [DF_{objectQualifier}ShoutboxMessage_BoardID] default (1),
		[UserID] [int] NULL,
		[UserName] [nvarchar](255) NOT NULL,
		[UserDisplayName] [nvarchar](255) NOT NULL,
		[Message] [nvarchar](max) NULL,
		[Date] [datetime] NOT NULL,
		[IP] [varchar](50) NOT NULL,
 constraint [PK_{objectQualifier}ShoutboxMessage] PRIMARY KEY CLUSTERED 
(
	[ShoutBoxMessageID] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
	)
end
GO	

exec('[{databaseOwner}].[{objectQualifier}drop_defaultconstraint_oncolumn] {objectQualifier}Board, BoardUID')
GO