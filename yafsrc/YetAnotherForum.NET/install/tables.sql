/* Version 1.0.2 */

/*
** Create missing tables
*/
<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}Active') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}Active(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_Active]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_Active](
>>>>>>> .r1490
		SessionID		nvarchar (24) NOT NULL ,
		BoardID			int NOT NULL ,
		UserID			int NOT NULL ,
		IP				nvarchar (15) NOT NULL ,
		Login			datetime NOT NULL ,
		LastActive		datetime NOT NULL ,
		Location		nvarchar (50) NOT NULL ,
		ForumID			int NULL ,
		TopicID			int NULL ,
		Browser			nvarchar (50) NULL ,
		Platform		nvarchar (50) NULL 
	)
go

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}BannedIP') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}BannedIP(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_BannedIP]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_BannedIP](
>>>>>>> .r1490
		ID				int IDENTITY (1, 1) NOT NULL ,
		BoardID			int NOT NULL ,
		Mask			nvarchar (15) NOT NULL ,
		Since			datetime NOT NULL 
	)
go

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}Category') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}Category(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_Category]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_Category](
>>>>>>> .r1490
		CategoryID		int IDENTITY (1, 1) NOT NULL ,
		BoardID			int NOT NULL ,
		Name			nvarchar (50) NOT NULL ,
		SortOrder		smallint NOT NULL 
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}CheckEmail') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}CheckEmail(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_CheckEmail]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_CheckEmail](
>>>>>>> .r1490
		CheckEmailID	int IDENTITY (1, 1) NOT NULL ,
		UserID			int NOT NULL ,
		Email			nvarchar (50) NOT NULL ,
		Created			datetime NOT NULL ,
		Hash			nvarchar (32) NOT NULL 
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}Choice') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}Choice(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_Choice]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_Choice](
>>>>>>> .r1490
		ChoiceID		int IDENTITY (1, 1) NOT NULL ,
		PollID			int NOT NULL ,
		Choice			nvarchar (50) NOT NULL ,
		Votes			int NOT NULL 
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}PollVote') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [dbo].[[{databaseOwner}].{objectQualifier}PollVote] (
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_PollVote]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [dbo].[yaf_PollVote] (
>>>>>>> .r1490
		[PollVoteID] [int] IDENTITY (1, 1) NOT NULL ,
		[PollID] [int] NOT NULL ,
		[UserID] [int] NULL ,
		[RemoteIP] [nvarchar] (10) NULL 
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}Forum') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}Forum(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_Forum]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_Forum](
>>>>>>> .r1490
		ForumID			int IDENTITY (1, 1) NOT NULL ,
		CategoryID		int NOT NULL ,
		ParentID		int NULL ,
		Name			nvarchar (50) NOT NULL ,
		Description		nvarchar (255) NOT NULL ,
		SortOrder		smallint NOT NULL ,
		LastPosted		datetime NULL ,
		LastTopicID		int NULL ,
		LastMessageID	int NULL ,
		LastUserID		int NULL ,
		LastUserName	nvarchar (50) NULL ,
		NumTopics		int NOT NULL,
		NumPosts		int NOT NULL,
		RemoteURL		nvarchar(100) null,
		Flags			int not null constraint DF_[{databaseOwner}].{objectQualifier}Forum_Flags default (0),
		ThemeURL		nvarchar(50) NULL
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}ForumAccess') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}ForumAccess(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_ForumAccess]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_ForumAccess](
>>>>>>> .r1490
		GroupID			int NOT NULL ,
		ForumID			int NOT NULL ,
		AccessMaskID	int NOT NULL
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}Group') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}Group(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_Group]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_Group](
>>>>>>> .r1490
		GroupID			int IDENTITY (1, 1) NOT NULL ,
		BoardID			int NOT NULL ,
		Name			nvarchar (50) NOT NULL ,
		Flags			int not null constraint DF_[{databaseOwner}].{objectQualifier}Group_Flags default (0)
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}Mail') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}Mail(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_Mail]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_Mail](
>>>>>>> .r1490
		MailID			int IDENTITY (1, 1) NOT NULL ,
		FromUser		nvarchar (50) NOT NULL ,
		ToUser			nvarchar (50) NOT NULL ,
		Created			datetime NOT NULL ,
		Subject			nvarchar (100) NOT NULL ,
		Body			ntext NOT NULL 
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}Message') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}Message(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_Message]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_Message](
>>>>>>> .r1490
		MessageID		int IDENTITY (1, 1) NOT NULL ,
		TopicID			int NOT NULL ,
		ReplyTo			int NULL ,
		Position		int NOT NULL ,
		Indent			int NOT NULL ,
		UserID			int NOT NULL ,
		UserName		nvarchar (50) NULL ,
		Posted			datetime NOT NULL ,
		Message			ntext NOT NULL ,
		IP				nvarchar (15) NOT NULL ,
		Edited			datetime NULL ,
		Flags			int NOT NULL constraint DF_[{databaseOwner}].{objectQualifier}Message_Flags default (23),
		EditReason      nvarchar (100) NULL ,
		IsModeratorChanged      bit NOT NULL CONSTRAINT [DF_[{databaseOwner}].{objectQualifier}Message_IsModeratorChanged] DEFAULT (0),
	    DeleteReason    nvarchar (100)  NULL
	)
GO

<<<<<<< .mine
IF NOT EXISTS (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}MessageReported') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [dbo].[[{databaseOwner}].{objectQualifier}MessageReported](
=======
IF NOT EXISTS (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_MessageReported]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [dbo].[yaf_MessageReported](
>>>>>>> .r1490
		[MessageID] [int] NOT NULL,
		[Message] [ntext] NULL,
		[Resolved] [bit] NULL,
		[ResolvedBy] [int] NULL,
		[ResolvedDate] [datetime] NULL
	)
GO

<<<<<<< .mine
IF NOT EXISTS (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}MessageReportedAudit') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [dbo].[[{databaseOwner}].{objectQualifier}MessageReportedAudit](
=======
IF NOT EXISTS (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_MessageReportedAudit]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [dbo].[yaf_MessageReportedAudit](
>>>>>>> .r1490
		[LogID] [int] IDENTITY(1,1) NOT NULL,
		[UserID] [int] NULL,
		[MessageID] [int] NULL,
		[Reported] [datetime] NULL
		)
GO


<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}PMessage') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}PMessage(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_PMessage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_PMessage](
>>>>>>> .r1490
		PMessageID		int IDENTITY (1, 1) NOT NULL ,
		FromUserID		int NOT NULL ,
		Created			datetime NOT NULL ,
		Subject			nvarchar (100) NOT NULL ,
		Body			ntext NOT NULL,
		Flags			int NOT NULL 
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}Poll') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}Poll(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_Poll]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_Poll](
>>>>>>> .r1490
		PollID			int IDENTITY (1, 1) NOT NULL ,
		Question		nvarchar (50) NOT NULL,
		Closes datetime NULL 		
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}Smiley') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}Smiley(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_Smiley]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_Smiley](
>>>>>>> .r1490
		SmileyID		int IDENTITY (1, 1) NOT NULL ,
		BoardID			int NOT NULL ,
		Code			nvarchar (10) NOT NULL ,
		Icon			nvarchar (50) NOT NULL ,
		Emoticon		nvarchar (50) NULL ,
		SortOrder		tinyint	NOT NULL DEFAULT 0
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}Topic') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}Topic(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_Topic]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_Topic](
>>>>>>> .r1490
		TopicID			int IDENTITY (1, 1) NOT NULL ,
		ForumID			int NOT NULL ,
		UserID			int NOT NULL ,
		UserName		nvarchar (50) NULL ,
		Posted			datetime NOT NULL ,
		Topic			nvarchar (100) NOT NULL ,
		Views			int NOT NULL ,
		Priority		smallint NOT NULL ,
		PollID			int NULL ,
		TopicMovedID	int NULL ,
		LastPosted		datetime NULL ,
		LastMessageID	int NULL ,
		LastUserID		int NULL ,
		LastUserName	nvarchar (50) NULL,
		NumPosts		int NOT NULL,
		Flags			int not null constraint DF_[{databaseOwner}].{objectQualifier}Topic_Flags default (0)
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}User') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}User(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_User]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_User](
>>>>>>> .r1490
		UserID			int IDENTITY (1, 1) NOT NULL ,
		BoardID			int NOT NULL,
		Name			nvarchar (50) NOT NULL ,
		Password		nvarchar (32) NOT NULL ,
		Email			nvarchar (50) NULL ,
		Joined			datetime NOT NULL ,
		LastVisit		datetime NOT NULL ,
		IP				nvarchar (15) NULL ,
		NumPosts		int NOT NULL ,
		TimeZone		int NOT NULL ,
		Avatar			nvarchar (255) NULL ,
		Signature		ntext NULL ,
		AvatarImage		image NULL,
		RankID			int NOT NULL,
		Suspended		datetime NULL,
		LanguageFile	nvarchar(50) NULL,
		ThemeFile		nvarchar(50) NULL,
		OverrideDefaultThemes	bit NOT NULL CONSTRAINT [DF_[{databaseOwner}].{objectQualifier}User_OverrideDefaultThemes] DEFAULT (0),
		[PMNotification] [bit] NOT NULL CONSTRAINT [DF_[{databaseOwner}].{objectQualifier}User_PMNotification] DEFAULT (1),
		[Flags] [int] NOT NULL CONSTRAINT [DF_[{databaseOwner}].{objectQualifier}User_Flags] DEFAULT (0),
		[Points] [int] NOT NULL CONSTRAINT [DF_[{databaseOwner}].{objectQualifier}User_Points] DEFAULT (0),		
		ProviderUserKey	uniqueidentifier
)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}WatchForum') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}WatchForum(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_WatchForum]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_WatchForum](
>>>>>>> .r1490
		WatchForumID	int IDENTITY (1, 1) NOT NULL ,
		ForumID			int NOT NULL ,
		UserID			int NOT NULL ,
		Created			datetime NOT NULL ,
		LastMail		datetime null
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}WatchTopic') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}WatchTopic(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_WatchTopic]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_WatchTopic](
>>>>>>> .r1490
		WatchTopicID	int IDENTITY (1, 1) NOT NULL ,
		TopicID			int NOT NULL ,
		UserID			int NOT NULL ,
		Created			datetime NOT NULL ,
		LastMail		datetime null
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}Attachment') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}Attachment(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_Attachment]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_Attachment](
>>>>>>> .r1490
		AttachmentID	int identity not null,
		MessageID		int not null,
		FileName		nvarchar(255) not null,
		Bytes			int not null,
		FileID			int null,
		ContentType		nvarchar(50) null,
		Downloads		int not null,
		FileData		image null
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}UserGroup') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}UserGroup(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_UserGroup]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_UserGroup](
>>>>>>> .r1490
		UserID			int NOT NULL,
		GroupID			int NOT NULL
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}Rank') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}Rank(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_Rank]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_Rank](
>>>>>>> .r1490
		RankID			int IDENTITY (1, 1) NOT NULL,
		BoardID			int NOT NULL ,
		Name			nvarchar (50) NOT NULL,
		MinPosts		int NULL,
		RankImage		nvarchar (50) NULL,
		Flags			int not null constraint DF_[{databaseOwner}].{objectQualifier}Rank_Flags default (0)
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}AccessMask') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}AccessMask(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_AccessMask]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_AccessMask](
>>>>>>> .r1490
		AccessMaskID	int IDENTITY NOT NULL ,
		BoardID			int NOT NULL ,
		Name			nvarchar(50) NOT NULL ,
		Flags			int not null constraint DF_[{databaseOwner}].{objectQualifier}AccessMask_Flags default (0)
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}UserForum') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}UserForum(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_UserForum]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_UserForum](
>>>>>>> .r1490
		UserID			int NOT NULL ,
		ForumID			int NOT NULL ,
		AccessMaskID	int NOT NULL ,
		Invited			datetime NOT NULL ,
		Accepted		bit NOT NULL
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}Board') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_Board]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
>>>>>>> .r1490
begin
<<<<<<< .mine
	create table [{databaseOwner}].{objectQualifier}Board(
=======
	create table [{databaseOwner}].[yaf_Board](
>>>>>>> .r1490
		BoardID			int NOT NULL IDENTITY(1,1),
		Name			nvarchar(50) NOT NULL,
		AllowThreaded	bit NOT NULL,
	)
end
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}NntpServer') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}NntpServer(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_NntpServer]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_NntpServer](
>>>>>>> .r1490
		NntpServerID	int identity not null,
		BoardID			int NOT NULL ,
		Name			nvarchar(50) not null,
		Address			nvarchar(100) not null,
		Port			int null,
		UserName		nvarchar(50) null,
		UserPass		nvarchar(50) null
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}NntpForum') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}NntpForum(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_NntpForum]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_NntpForum](
>>>>>>> .r1490
		NntpForumID		int identity not null,
		NntpServerID	int not null,
		GroupName		nvarchar(100) not null,
		ForumID			int not null,
		LastMessageNo	int not null,
		LastUpdate		datetime not null,
		Active			bit not null
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}NntpTopic') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].{objectQualifier}NntpTopic(
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_NntpTopic]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	create table [{databaseOwner}].[yaf_NntpTopic](
>>>>>>> .r1490
		NntpTopicID		int identity not null,
		NntpForumID		int not null,
		Thread			char(32) not null,
		TopicID			int not null
	)
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}UserPMessage') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_UserPMessage]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
>>>>>>> .r1490
begin
<<<<<<< .mine
	create table [{databaseOwner}].{objectQualifier}UserPMessage(
=======
	create table [{databaseOwner}].[yaf_UserPMessage](
>>>>>>> .r1490
		UserPMessageID	int identity not null,
		UserID			int not null,
		PMessageID		int not null,
		IsRead			bit not null,
		IsInOutbox		bit not null default (1),
		IsArchived		bit not null default (0)
	)
end
GO

<<<<<<< .mine
if not exists (select * from dbo.sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}Replace_Words') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
=======
if not exists (select * from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_Replace_Words]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
>>>>>>> .r1490
begin
<<<<<<< .mine
	create table [{databaseOwner}].{objectQualifier}Replace_Words(
=======
	create table [{databaseOwner}].[yaf_Replace_Words](
>>>>>>> .r1490
		id				int IDENTITY (1, 1) NOT NULL ,
		badword			nvarchar (255) NULL ,
		goodword		nvarchar (255) NULL ,
		constraint PK_Replace_Words primary key(id)
	)
end
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}Registry') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_Registry]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
>>>>>>> .r1490
begin
<<<<<<< .mine
	create table [{databaseOwner}].{objectQualifier}Registry(
=======
	create table [{databaseOwner}].[yaf_Registry](
>>>>>>> .r1490
		RegistryID		int IDENTITY(1, 1) NOT NULL,
		Name			nvarchar(50) NOT NULL,
		Value			ntext,
		BoardID			int,
		CONSTRAINT PK_Registry PRIMARY KEY (RegistryID)
	)
end
GO

<<<<<<< .mine
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}EventLog') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
=======
if not exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_EventLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
>>>>>>> .r1490
begin
<<<<<<< .mine
	create table [{databaseOwner}].{objectQualifier}EventLog(
=======
	create table [{databaseOwner}].[yaf_EventLog](
>>>>>>> .r1490
		EventLogID	int identity(1,1) not null,
		EventTime	datetime not null constraint DF_[{databaseOwner}].{objectQualifier}EventLog_EventTime default getdate(),
		UserID		int,
		Source		nvarchar(50) not null,
		Description	ntext not null,
		constraint PK_[{databaseOwner}].{objectQualifier}EventLog primary key(EventLogID)
	)
end
GO

/*
** Added columns
*/

-- [{databaseOwner}].{objectQualifier}UserPMessage

<<<<<<< .mine
if not exists (select 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].{objectQualifier}UserPMessage') and name='IsInOutbox')
=======
if not exists (select 1 from [{databaseOwner}].[syscolumns] where id = object_id(N'[{databaseOwner}].[yaf_UserPMessage]') and name='IsInOutbox')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}UserPMessage add IsInOutbox	bit not null default (1)
=======
	alter table [{databaseOwner}].[yaf_UserPMessage] add IsInOutbox	bit not null default (1)
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if not exists (select 1 from dbo.syscolumns where id = object_id('[{databaseOwner}].{objectQualifier}UserPMessage') and name='IsArchived')
=======
if not exists (select 1 from [{databaseOwner}].[syscolumns] where id = object_id(N'[{databaseOwner}].[yaf_UserPMessage]') and name='IsArchived')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}UserPMessage add IsArchived	bit not null default (0)
=======
	alter table [{databaseOwner}].[yaf_UserPMessage] add IsArchived	bit not null default (0)
>>>>>>> .r1490
end
GO


-- [{databaseOwner}].{objectQualifier}User

<<<<<<< .mine
if exists(select 1 from dbo.syscolumns where id = object_id(N'[{databaseOwner}].{objectQualifier}User') and name=N'Signature' and xtype<>99)
	alter table [{databaseOwner}].{objectQualifier}User alter column Signature ntext null
=======
if exists(select 1 from [{databaseOwner}].[syscolumns] where id = object_id(N'[{databaseOwner}].[yaf_User]') and name=N'Signature' and xtype<>99)
	alter table [{databaseOwner}].[yaf_User] alter column Signature ntext null
>>>>>>> .r1490
go

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='Flags')
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='Flags')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User add Flags int not null constraint DF_[{databaseOwner}].{objectQualifier}User_Flags default (0)
=======
	alter table [{databaseOwner}].[yaf_User] add Flags int not null constraint DF_yaf_User_Flags default (0)
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='IsHostAdmin')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='IsHostAdmin')
>>>>>>> .r1490
begin
<<<<<<< .mine
	grant update on [{databaseOwner}].{objectQualifier}User to public
	exec('update [{databaseOwner}].{objectQualifier}User set Flags = Flags | 1 where IsHostAdmin<>0')
	revoke update on [{databaseOwner}].{objectQualifier}User from public
	alter table [{databaseOwner}].{objectQualifier}User drop column IsHostAdmin
=======
	grant update on [{databaseOwner}].[yaf_User] to public
	exec('update [{databaseOwner}].[yaf_User[ set Flags = Flags | 1 where IsHostAdmin<>0')
	revoke update on [{databaseOwner}].[yaf_User] from public
	alter table [{databaseOwner}].[yaf_User] drop column IsHostAdmin
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='Approved')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='Approved')
>>>>>>> .r1490
begin
<<<<<<< .mine
	grant update on [{databaseOwner}].{objectQualifier}User to public
	exec('update [{databaseOwner}].{objectQualifier}User set Flags = Flags | 2 where Approved<>0')
	revoke update on [{databaseOwner}].{objectQualifier}User from public
	alter table [{databaseOwner}].{objectQualifier}User drop column Approved
=======
	grant update on [{databaseOwner}].[yaf_User] to public
	exec('update [{databaseOwner}].[yaf_User] set Flags = Flags | 2 where Approved<>0')
	revoke update on [{databaseOwner}].[yaf_User] from public
	alter table [{databaseOwner}].[yaf_User] drop column Approved
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='ProviderUserKey')
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='ProviderUserKey')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User add ProviderUserKey uniqueidentifier
=======
	alter table [{databaseOwner}].[yaf_User] add ProviderUserKey uniqueidentifier
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='PMNotification')
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='PMNotification')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User add [PMNotification] [bit] NOT NULL CONSTRAINT [DF_[{databaseOwner}].{objectQualifier}User_PMNotification] DEFAULT (1)
=======
	alter table [{databaseOwner}].[yaf_User] add [PMNotification] [bit] NOT NULL CONSTRAINT [DF_yaf_User_PMNotification] DEFAULT (1)
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='OverrideDefaultThemes')
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='OverrideDefaultThemes')
>>>>>>> .r1490
begin
<<<<<<< .mine
alter table [{databaseOwner}].{objectQualifier}User add  [OverrideDefaultThemes]	bit NOT NULL CONSTRAINT [DF_[{databaseOwner}].{objectQualifier}User_OverrideDefaultThemes] DEFAULT (0)
=======
alter table [{databaseOwner}].[yaf_User] add  [OverrideDefaultThemes]	bit NOT NULL CONSTRAINT [DF_yaf_User_OverrideDefaultThemes] DEFAULT (0)
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='Points')
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='Points')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User add [Points] [int] NOT NULL CONSTRAINT [DF_[{databaseOwner}].{objectQualifier}User_Points] DEFAULT (0)
=======
	alter table [{databaseOwner}].[yaf_User] add [Points] [int] NOT NULL CONSTRAINT [DF_yaf_User_Points] DEFAULT (0)
>>>>>>> .r1490
end
GO

-- remove columns that got moved to Profile
<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='Gender')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='Gender')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User drop column Gender
=======
	alter table [{databaseOwner}].[yaf_User] drop column Gender
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='Location')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='Location')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User drop column Location
=======
	alter table [{databaseOwner}].[yaf_User] drop column Location
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='HomePage')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='HomePage')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User drop column HomePage
=======
	alter table [{databaseOwner}].[yaf_User] drop column HomePage
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='MSN')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='MSN')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User drop column MSN
=======
	alter table [{databaseOwner}].[yaf_User] drop column MSN
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='YIM')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='YIM')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User drop column YIM
=======
	alter table [{databaseOwner}].[yaf_User] drop column YIM
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='AIM')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='AIM')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User drop column AIM
=======
	alter table [{databaseOwner}].[yaf_User] drop column AIM
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='ICQ')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='ICQ')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User drop column ICQ
=======
	alter table [{databaseOwner}].[yaf_User] drop column ICQ
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='RealName')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='RealName')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User drop column RealName
=======
	alter table [{databaseOwner}].[yaf_User] drop column RealName
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='Occupation')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='Occupation')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User drop column Occupation
=======
	alter table [{databaseOwner}].[yaf_User] drop column Occupation
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='Interests')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='Interests')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User drop column Interests
=======
	alter table [{databaseOwner}].[yaf_User] drop column Interests
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='Weblog')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='Weblog')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User drop column Weblog
=======
	alter table [{databaseOwner}].[yaf_User] drop column Weblog
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}User') and name='WeblogUrl')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_User]') and name='WeblogUrl')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}User drop column WeblogUrl
	alter table [{databaseOwner}].{objectQualifier}User drop column WeblogUsername
	alter table [{databaseOwner}].{objectQualifier}User drop column WeblogID
=======
	alter table [{databaseOwner}].[yaf_User] drop column WeblogUrl
	alter table [{databaseOwner}].[yaf_User] drop column WeblogUsername
	alter table [{databaseOwner}].[yaf_User] drop column WeblogID
>>>>>>> .r1490
end
GO

-- [{databaseOwner}].{objectQualifier}Mesaage

/* post to blog start */ 

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Message') and name='BlogPostID')
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Message]') and name='BlogPostID')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}Message add BlogPostID nvarchar(50)
=======
	alter table [{databaseOwner}].[yaf_Message] add BlogPostID nvarchar(50)
>>>>>>> .r1490
end
GO

/* post to blog end */ 

-- [{databaseOwner}].{objectQualifier}Forum

<<<<<<< .mine
if not exists(select * from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Forum') and name='RemoteURL')
	alter table [{databaseOwner}].{objectQualifier}Forum add RemoteURL nvarchar(100) null
=======
if not exists(select * from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='RemoteURL')
	alter table [{databaseOwner}].[yaf_Forum] add RemoteURL nvarchar(100) null
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Forum') and name='Flags')
	alter table [{databaseOwner}].{objectQualifier}Forum add Flags int not null constraint DF_[{databaseOwner}].{objectQualifier}Forum_Flags default (0)
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='Flags')
	alter table [{databaseOwner}].[yaf_Forum] add Flags int not null constraint DF_yaf_Forum_Flags default (0)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select * from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Forum') and name='ThemeURL')
	alter table [{databaseOwner}].{objectQualifier}Forum add ThemeURL nvarchar(50) NULL
=======
if not exists(select * from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='ThemeURL')
	alter table [{databaseOwner}].[yaf_Forum] add ThemeURL nvarchar(50) NULL
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Forum') and name='Locked')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='Locked')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}Forum set Flags = Flags | 1 where Locked<>0')
	alter table [{databaseOwner}].{objectQualifier}Forum drop column Locked
=======
	exec('update yaf_Forum set Flags = Flags | 1 where Locked<>0')
	alter table [{databaseOwner}].[yaf_Forum] drop column Locked
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Forum') and name='Hidden')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='Hidden')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}Forum set Flags = Flags | 2 where Hidden<>0')
	alter table [{databaseOwner}].{objectQualifier}Forum drop column Hidden
=======
	exec('update yaf_Forum set Flags = Flags | 2 where Hidden<>0')
	alter table [{databaseOwner}].[yaf_Forum] drop column Hidden
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Forum') and name='IsTest')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='IsTest')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}Forum set Flags = Flags | 4 where IsTest<>0')
	alter table [{databaseOwner}].{objectQualifier}Forum drop column IsTest
=======
	exec('update yaf_Forum set Flags = Flags | 4 where IsTest<>0')
	alter table [{databaseOwner}].[yaf_Forum] drop column IsTest
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Forum') and name='Moderated')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Forum]') and name='Moderated')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}Forum set Flags = Flags | 8 where Moderated<>0')
	alter table [{databaseOwner}].{objectQualifier}Forum drop column Moderated
=======
	exec('update yaf_Forum set Flags = Flags | 8 where Moderated<>0')
	alter table [{databaseOwner}].[yaf_Forum] drop column Moderated
>>>>>>> .r1490
end
GO

-- [{databaseOwner}].{objectQualifier}Group

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Group') and name='Flags')
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='Flags')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}Group add Flags int not null constraint DF_[{databaseOwner}].{objectQualifier}Group_Flags default (0)
=======
	alter table [{databaseOwner}].[yaf_Group] add Flags int not null constraint DF_yaf_Group_Flags default (0)
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Group') and name='IsAdmin')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='IsAdmin')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}Group set Flags = Flags | 1 where IsAdmin<>0')
	alter table [{databaseOwner}].{objectQualifier}Group drop column IsAdmin
=======
	exec('update yaf_Group set Flags = Flags | 1 where IsAdmin<>0')
	alter table [{databaseOwner}].[yaf_Group] drop column IsAdmin
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Group') and name='IsGuest')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='IsGuest')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}Group set Flags = Flags | 2 where IsGuest<>0')
	alter table [{databaseOwner}].{objectQualifier}Group drop column IsGuest
=======
	exec('update yaf_Group set Flags = Flags | 2 where IsGuest<>0')
	alter table [{databaseOwner}].[yaf_Group] drop column IsGuest
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Group') and name='IsStart')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='IsStart')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}Group set Flags = Flags | 4 where IsStart<>0')
	alter table [{databaseOwner}].{objectQualifier}Group drop column IsStart
=======
	exec('update yaf_Group set Flags = Flags | 4 where IsStart<>0')
	alter table [{databaseOwner}].[yaf_Group] drop column IsStart
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Group') and name='IsModerator')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Group]') and name='IsModerator')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}Group set Flags = Flags | 8 where IsModerator<>0')
	alter table [{databaseOwner}].{objectQualifier}Group drop column IsModerator
=======
	exec('update yaf_Group set Flags = Flags | 8 where IsModerator<>0')
	alter table [{databaseOwner}].[yaf_Group] drop column IsModerator
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from [{databaseOwner}].{objectQualifier}Group where (Flags & 2)=2)
=======
if exists(select 1 from [{databaseOwner}].[yaf_Group] where (Flags & 2)=2)
>>>>>>> .r1490
begin
<<<<<<< .mine
	update [{databaseOwner}].{objectQualifier}User set Flags = Flags | 4 where UserID in(select distinct UserID from [{databaseOwner}].{objectQualifier}UserGroup a join [{databaseOwner}].{objectQualifier}Group b on b.GroupID=a.GroupID and (b.Flags & 2)=2)
=======
	update [{databaseOwner}].[yaf_User] set Flags = Flags | 4 where UserID in(select distinct UserID from [{databaseOwner}].[yaf_UserGroup] a join [{databaseOwner}].[yaf_Group] b on b.GroupID=a.GroupID and (b.Flags & 2)=2)
>>>>>>> .r1490
end
GO

-- [{databaseOwner}].{objectQualifier}AccessMask

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}AccessMask') and name='Flags')
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and name='Flags')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}AccessMask add Flags int not null constraint DF_[{databaseOwner}].{objectQualifier}AccessMask_Flags default (0)
=======
	alter table [{databaseOwner}].[yaf_AccessMask] add Flags int not null constraint DF_yaf_AccessMask_Flags default (0)
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}AccessMask') and name='ReadAccess')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and name='ReadAccess')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}AccessMask set Flags = Flags | 1 where ReadAccess<>0')
	alter table [{databaseOwner}].{objectQualifier}AccessMask drop column ReadAccess
=======
	exec('update yaf_AccessMask set Flags = Flags | 1 where ReadAccess<>0')
	alter table [{databaseOwner}].[yaf_AccessMask] drop column ReadAccess
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}AccessMask') and name='PostAccess')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and name='PostAccess')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}AccessMask set Flags = Flags | 2 where PostAccess<>0')
	alter table [{databaseOwner}].{objectQualifier}AccessMask drop column PostAccess
=======
	exec('update yaf_AccessMask set Flags = Flags | 2 where PostAccess<>0')
	alter table [{databaseOwner}].[yaf_AccessMask] drop column PostAccess
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}AccessMask') and name='ReplyAccess')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and name='ReplyAccess')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}AccessMask set Flags = Flags | 4 where ReplyAccess<>0')
	alter table [{databaseOwner}].{objectQualifier}AccessMask drop column ReplyAccess
=======
	exec('update yaf_AccessMask set Flags = Flags | 4 where ReplyAccess<>0')
	alter table [{databaseOwner}].[yaf_AccessMask] drop column ReplyAccess
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}AccessMask') and name='PriorityAccess')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and name='PriorityAccess')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}AccessMask set Flags = Flags | 8 where PriorityAccess<>0')
	alter table [{databaseOwner}].{objectQualifier}AccessMask drop column PriorityAccess
=======
	exec('update yaf_AccessMask set Flags = Flags | 8 where PriorityAccess<>0')
	alter table [{databaseOwner}].[yaf_AccessMask] drop column PriorityAccess
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}AccessMask') and name='PollAccess')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and name='PollAccess')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}AccessMask set Flags = Flags | 16 where PollAccess<>0')
	alter table [{databaseOwner}].{objectQualifier}AccessMask drop column PollAccess
=======
	exec('update yaf_AccessMask set Flags = Flags | 16 where PollAccess<>0')
	alter table [{databaseOwner}].[yaf_AccessMask] drop column PollAccess
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}AccessMask') and name='VoteAccess')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and name='VoteAccess')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}AccessMask set Flags = Flags | 32 where VoteAccess<>0')
	alter table [{databaseOwner}].{objectQualifier}AccessMask drop column VoteAccess
=======
	exec('update yaf_AccessMask set Flags = Flags | 32 where VoteAccess<>0')
	alter table [{databaseOwner}].[yaf_AccessMask] drop column VoteAccess
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}AccessMask') and name='ModeratorAccess')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and name='ModeratorAccess')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}AccessMask set Flags = Flags | 64 where ModeratorAccess<>0')
	alter table [{databaseOwner}].{objectQualifier}AccessMask drop column ModeratorAccess
=======
	exec('update yaf_AccessMask set Flags = Flags | 64 where ModeratorAccess<>0')
	alter table [{databaseOwner}].[yaf_AccessMask] drop column ModeratorAccess
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}AccessMask') and name='EditAccess')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and name='EditAccess')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}AccessMask set Flags = Flags | 128 where EditAccess<>0')
	alter table [{databaseOwner}].{objectQualifier}AccessMask drop column EditAccess
=======
	exec('update yaf_AccessMask set Flags = Flags | 128 where EditAccess<>0')
	alter table [{databaseOwner}].[yaf_AccessMask] drop column EditAccess
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}AccessMask') and name='DeleteAccess')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and name='DeleteAccess')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}AccessMask set Flags = Flags | 256 where DeleteAccess<>0')
	alter table [{databaseOwner}].{objectQualifier}AccessMask drop column DeleteAccess
=======
	exec('update yaf_AccessMask set Flags = Flags | 256 where DeleteAccess<>0')
	alter table [{databaseOwner}].[yaf_AccessMask] drop column DeleteAccess
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}AccessMask') and name='UploadAccess')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_AccessMask]') and name='UploadAccess')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}AccessMask set Flags = Flags | 512 where UploadAccess<>0')
	alter table [{databaseOwner}].{objectQualifier}AccessMask drop column UploadAccess
=======
	exec('update yaf_AccessMask set Flags = Flags | 512 where UploadAccess<>0')
	alter table [{databaseOwner}].[yaf_AccessMask] drop column UploadAccess
>>>>>>> .r1490
end
GO

-- [{databaseOwner}].{objectQualifier}NntpForum

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}NntpForum') and name='Active')
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_NntpForum]') and name='Active')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}NntpForum add Active bit null
	exec('update [{databaseOwner}].{objectQualifier}NntpForum set Active=1 where Active is null')
	alter table [{databaseOwner}].{objectQualifier}NntpForum alter column Active bit not null
=======
	alter table [{databaseOwner}].[yaf_NntpForum] add Active bit null
	exec('update [{databaseOwner}].[yaf_NntpForum] set Active=1 where Active is null')
	alter table [{databaseOwner}].[yaf_NntpForum] alter column Active bit not null
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists (select * from dbo.syscolumns where id = object_id(N'[{databaseOwner}].{objectQualifier}Replace_Words') and name='badword' and prec < 255)
 	alter table [{databaseOwner}].{objectQualifier}Replace_Words alter column badword nvarchar(255) NULL
=======
if exists (select * from [{databaseOwner}].[syscolumns] where id = object_id(N'[{databaseOwner}].[yaf_Replace_Words]') and name='badword' and prec < 255)
 	alter table [{databaseOwner}].[yaf_Replace_Words] alter column badword nvarchar(255) NULL
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists (select * from dbo.syscolumns where id = object_id(N'[{databaseOwner}].{objectQualifier}Replace_Words') and name='goodword' and prec < 255)
	alter table [{databaseOwner}].{objectQualifier}Replace_Words alter column goodword nvarchar(255) NULL
=======
if exists (select * from [{databaseOwner}].[syscolumns] where id = object_id(N'[{databaseOwner}].[yaf_Replace_Words]') and name='goodword' and prec < 255)
	alter table [{databaseOwner}].[yaf_Replace_Words] alter column goodword nvarchar(255) NULL
>>>>>>> .r1490
GO	

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Registry') and name='BoardID')
	alter table [{databaseOwner}].{objectQualifier}Registry add BoardID int
=======
if not exists(select 1 from syscolumns where id = object_id(N'[{databaseOwner}].[yaf_Registry]') and name='BoardID')
	alter table [{databaseOwner}].[yaf_Registry] add BoardID int
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].{objectQualifier}Registry') and name=N'Value' and xtype<>99)
	alter table [{databaseOwner}].{objectQualifier}Registry alter column Value ntext null
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Registry]') and name=N'Value' and xtype<>99)
	alter table [{databaseOwner}].[yaf_Registry] alter column Value ntext null
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}PMessage') and name='Flags')
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_PMessage]') and name='Flags')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}PMessage add Flags int not null constraint DF_[{databaseOwner}].{objectQualifier}Message_Flags default (23)
=======
	alter table [{databaseOwner}].[yaf_PMessage] add Flags int not null constraint DF_yaf_Message_Flags default (23)
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Topic') and name='Flags')
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Topic]') and name='Flags')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}Topic add Flags int not null constraint DF_[{databaseOwner}].{objectQualifier}Topic_Flags default (0)
	update [{databaseOwner}].{objectQualifier}Message set Flags = Flags & 7
=======
	alter table [{databaseOwner}].[yaf_Topic] add Flags int not null constraint DF_yaf_Topic_Flags default (0)
	update [{databaseOwner}].[yaf_Message] set Flags = Flags & 7
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Message') and name='Approved')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Message]') and name='Approved')
>>>>>>> .r1490
begin
<<<<<<< .mine
	exec('update [{databaseOwner}].{objectQualifier}Message set Flags = Flags | 16 where Approved<>0')
	alter table [{databaseOwner}].{objectQualifier}Message drop column Approved
=======
	exec('update yaf_Message set Flags = Flags | 16 where Approved<>0')
	alter table [{databaseOwner}].[yaf_Message] drop column Approved
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Message') and name='EditReason')
	alter table [{databaseOwner}].{objectQualifier}Message add EditReason nvarchar (100) NULL
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Message]') and name='EditReason')
	alter table [{databaseOwner}].[yaf_Message] add EditReason nvarchar (100) NULL
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Message') and name='IsModeratorChanged')
	alter table [{databaseOwner}].{objectQualifier}Message add 	IsModeratorChanged      bit NOT NULL CONSTRAINT [DF_[{databaseOwner}].{objectQualifier}Message_IsModeratorChanged] DEFAULT (0)
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Message]') and name='IsModeratorChanged')
	alter table [{databaseOwner}].[yaf_Message] add 	IsModeratorChanged      bit NOT NULL CONSTRAINT [DF_yaf_Message_IsModeratorChanged] DEFAULT (0)
>>>>>>> .r1490
GO

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Message') and name='DeleteReason')
	alter table [{databaseOwner}].{objectQualifier}Message add DeleteReason            nvarchar (100)  NULL
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Message]') and name='DeleteReason')
	alter table [{databaseOwner}].[yaf_Message] add DeleteReason            nvarchar (100)  NULL
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Topic') and name='IsLocked')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Topic]') and name='IsLocked')
>>>>>>> .r1490
begin
<<<<<<< .mine
	grant update on [{databaseOwner}].{objectQualifier}Topic to public
	exec('update [{databaseOwner}].{objectQualifier}Topic set Flags = Flags | 1 where IsLocked<>0')
	revoke update on [{databaseOwner}].{objectQualifier}Topic from public
	alter table [{databaseOwner}].{objectQualifier}Topic drop column IsLocked
=======
	grant update on [{databaseOwner}].[yaf_Topic] to public
	exec('update [{databaseOwner}].[yaf_Topic] set Flags = Flags | 1 where IsLocked<>0')
	revoke update on [{databaseOwner}].[yaf_Topic] from public
	alter table [{databaseOwner}].[yaf_Topic] drop column IsLocked
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Rank') and name='Flags')
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Rank]') and name='Flags')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}Rank add Flags int not null constraint DF_[{databaseOwner}].{objectQualifier}Rank_Flags default (0)
=======
	alter table [{databaseOwner}].[yaf_Rank] add Flags int not null constraint DF_yaf_Rank_Flags default (0)
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Rank') and name='IsStart')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Rank]') and name='IsStart')
>>>>>>> .r1490
begin
<<<<<<< .mine
	grant update on [{databaseOwner}].{objectQualifier}Rank to public
	exec('update [{databaseOwner}].{objectQualifier}Rank set Flags = Flags | 1 where IsStart<>0')
	revoke update on [{databaseOwner}].{objectQualifier}Rank from public
	alter table [{databaseOwner}].{objectQualifier}Rank drop column IsStart
=======
	grant update on [{databaseOwner}].[yaf_Rank] to public
	exec('update [{databaseOwner}].[yaf_Rank] set Flags = Flags | 1 where IsStart<>0')
	revoke update on [{databaseOwner}].[yaf_Rank] from public
	alter table [{databaseOwner}].[yaf_Rank] drop column IsStart
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Rank') and name='IsLadder')
=======
if exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Rank]') and name='IsLadder')
>>>>>>> .r1490
begin
<<<<<<< .mine
	grant update on [{databaseOwner}].{objectQualifier}Rank to public
	exec('update [{databaseOwner}].{objectQualifier}Rank set Flags = Flags | 2 where IsLadder<>0')
	revoke update on [{databaseOwner}].{objectQualifier}Rank from public
	alter table [{databaseOwner}].{objectQualifier}Rank drop column IsLadder
=======
	grant update on [{databaseOwner}].[yaf_Rank] to public
	exec('update [{databaseOwner}].[yaf_Rank] set Flags = Flags | 2 where IsLadder<>0')
	revoke update on [{databaseOwner}].[yaf_Rank] from public
	alter table [{databaseOwner}].[yaf_Rank] drop column IsLadder
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Poll') and name='Closes')
=======
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Poll]') and name='Closes')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}Poll add Closes datetime null
=======
	alter table [{databaseOwner}].[yaf_Poll] add Closes datetime null
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if not exists(select 1 from dbo.syscolumns where id = object_id(N'[{databaseOwner}].{objectQualifier}EventLog') and name=N'Type')
=======
if not exists(select 1 from [{databaseOwner}].[syscolumns] where id = object_id(N'[{databaseOwner}].[yaf_EventLog]') and name=N'Type')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}EventLog add Type int not null constraint DF_[{databaseOwner}].{objectQualifier}EventLog_Type default (0)
	exec('update [{databaseOwner}].{objectQualifier}EventLog set Type = 0')
=======
	alter table [{databaseOwner}].[yaf_EventLog] add Type int not null constraint DF_yaf_EventLog_Type default (0)
	exec('update [{databaseOwner}].[yaf_EventLog] set Type = 0')
>>>>>>> .r1490
end
GO

<<<<<<< .mine
if exists(select 1 from dbo.syscolumns where id = object_id(N'[{databaseOwner}].{objectQualifier}EventLog') and name=N'UserID' and isnullable=0)
	alter table [{databaseOwner}].{objectQualifier}EventLog alter column UserID int null
=======
if exists(select 1 from [{databaseOwner}].[syscolumns] where id = object_id(N'[{databaseOwner}].[yaf_EventLog]') and name=N'UserID' and isnullable=0)
	alter table [{databaseOwner}].[yaf_EventLog] alter column UserID int null
>>>>>>> .r1490
GO

<<<<<<< .mine
-- [{databaseOwner}].{objectQualifier}Smiley
if not exists(select 1 from syscolumns where id=object_id('[{databaseOwner}].{objectQualifier}Smiley') and name='SortOrder')
=======
-- yaf_Smiley
if not exists(select 1 from syscolumns where id=object_id(N'[{databaseOwner}].[yaf_Smiley]') and name='SortOrder')
>>>>>>> .r1490
begin
<<<<<<< .mine
	alter table [{databaseOwner}].{objectQualifier}Smiley add SortOrder tinyint NOT NULL DEFAULT 0
=======
	alter table [{databaseOwner}].[yaf_Smiley] add SortOrder tinyint NOT NULL DEFAULT 0
>>>>>>> .r1490
end
GO