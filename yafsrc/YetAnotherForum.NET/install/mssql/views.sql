 -- vzrus: drop indexes on views here
  
/****** Object:  Index [{objectQualifier}vaccess_user_UserForum]    Script Date: 09/28/2009 22:30:20 ******/
IF  exists (select top 1 1 from dbo.sysindexes WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess_user]') AND name = N'{objectQualifier}vaccess_user_UserForum_PK')
DROP INDEX  [{databaseOwner}].[{objectQualifier}vaccess_user].[{objectQualifier}vaccess_user_UserForum_PK]
GO

/****** Object:  Index [{objectQualifier}vaccess_null_UserForum]    Script Date: 09/28/2009 22:30:36 ******/
IF  exists (select top 1 1 from dbo.sysindexes WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess_null]') AND name = N'{objectQualifier}vaccess_null_UserForum_PK')
DROP INDEX  [{databaseOwner}].[{objectQualifier}vaccess_null].[{objectQualifier}vaccess_null_UserForum_PK]
GO

/****** Object:  Index [{objectQualifier}vaccess_group_UserGroup]    Script Date: 09/28/2009 22:30:55 ******/
IF  exists (select top 1 1 from dbo.sysindexes WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess_group]') AND name = N'{objectQualifier}vaccess_group_UserForum_PK')
DROP INDEX [{databaseOwner}].[{objectQualifier}vaccess_group].[{objectQualifier}vaccess_group_UserForum_PK]
GO

-- drop views

/****** Object:  View [{databaseOwner}].[{objectQualifier}vaccess]    Script Date: 10/27/2009 21:42:29 ******/
IF  exists (select top 1 1 from dbo.sysobjects where id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess]') AND OBJECTPROPERTY(id, N'IsView') = 1)
DROP VIEW [{databaseOwner}].[{objectQualifier}vaccess]
GO

/****** Object:  View [{databaseOwner}].[{objectQualifier}vaccessfull]    Script Date: 10/27/2009 21:42:29 ******/
IF  exists (select top 1 1 from dbo.sysobjects where id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccessfull]') AND OBJECTPROPERTY(id, N'IsView') = 1)
DROP VIEW [{databaseOwner}].[{objectQualifier}vaccessfull]
GO

/****** Object:  View [{databaseOwner}].[{objectQualifier}vaccess_group]    Script Date: 10/27/2009 21:42:29 ******/
IF  exists (select top 1 1 from dbo.sysobjects where id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess_group]') AND OBJECTPROPERTY(id, N'IsView') = 1)
DROP VIEW [{databaseOwner}].[{objectQualifier}vaccess_group]
GO

/****** Object:  View [{databaseOwner}].[{objectQualifier}vaccess_null]    Script Date: 10/27/2009 21:42:29 ******/
IF  exists (select top 1 1 from dbo.sysobjects where id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess_null]') AND OBJECTPROPERTY(id, N'IsView') = 1)
DROP VIEW [{databaseOwner}].[{objectQualifier}vaccess_null]
GO

/****** Object:  View [{databaseOwner}].[{objectQualifier}vaccess_user]    Script Date: 10/27/2009 21:42:29 ******/
IF  exists (select top 1 1 from dbo.sysobjects where id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess_user]') AND OBJECTPROPERTY(id, N'IsView') = 1)
DROP VIEW [{databaseOwner}].[{objectQualifier}vaccess_user]
GO

/****** Object:  View [{databaseOwner}].[{objectQualifier}PMessageView]    Script Date: 10/27/2009 21:42:29 ******/
IF  exists (select top 1 1 from dbo.sysobjects where id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}PMessageView]') AND OBJECTPROPERTY(id, N'IsView') = 1)
DROP VIEW [{databaseOwner}].[{objectQualifier}PMessageView]
GO

IF NOT exists (select top 1 1 from dbo.sysobjects where id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess_group]') AND OBJECTPROPERTY(id, N'IsView') = 1)
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [{databaseOwner}].[{objectQualifier}vaccess_group]
WITH SCHEMABINDING
AS
		select
			b.UserID,
			c.ForumID,
			d.AccessMaskID,
			b.GroupID,
			ReadAccess		= convert(int,d.Flags & 1),
			PostAccess		= convert(int,d.Flags & 2),
			ReplyAccess		= convert(int,d.Flags & 4),
			PriorityAccess	= convert(int,d.Flags & 8),
			PollAccess		= convert(int,d.Flags & 16),
			VoteAccess		= convert(int,d.Flags & 32),
			ModeratorAccess	= convert(int,d.Flags & 64),
			EditAccess		= convert(int,d.Flags & 128),
			DeleteAccess	= convert(int,d.Flags & 256),
			UploadAccess	= convert(int,d.Flags & 512),
			DownloadAccess	= convert(int,d.Flags & 1024),
			AdminGroup		= convert(int,e.Flags & 1)
		from
			[{databaseOwner}].[{objectQualifier}UserGroup] b
			INNER JOIN [{databaseOwner}].[{objectQualifier}ForumAccess] c on c.GroupID=b.GroupID
			INNER JOIN [{databaseOwner}].[{objectQualifier}AccessMask] d on d.AccessMaskID=c.AccessMaskID
			INNER JOIN [{databaseOwner}].[{objectQualifier}Group] e on e.GroupID=b.GroupID' 
GO

IF NOT exists (select top 1 1 from dbo.sysobjects where id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess_null]') AND OBJECTPROPERTY(id, N'IsView') = 1)
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [{databaseOwner}].[{objectQualifier}vaccess_null]
WITH SCHEMABINDING
AS
		select
			a.UserID,
			ForumID			  = convert(int,0),
			AccessMaskID  = convert(int,0),
			GroupID				= convert(int,0),
			ReadAccess		= convert(int,0),
			PostAccess		= convert(int,0),
			ReplyAccess		= convert(int,0),
			PriorityAccess	= convert(int,0),
			PollAccess		= convert(int,0),
			VoteAccess		= convert(int,0),
			ModeratorAccess	= convert(int,0),
			EditAccess		= convert(int,0),
			DeleteAccess	= convert(int,0),
			UploadAccess	= convert(int,0),
			DownloadAccess	= convert(int,0),
			AdminGroup		= convert(int,0)
		from
			[{databaseOwner}].[{objectQualifier}User] a' 
GO


IF NOT exists (select top 1 1 from dbo.sysobjects where id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess_user]') AND OBJECTPROPERTY(id, N'IsView') = 1)
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [{databaseOwner}].[{objectQualifier}vaccess_user]
WITH SCHEMABINDING
AS
	SELECT
			b.UserID,
			b.ForumID,
			c.AccessMaskID,
			GroupID				= convert(int,0),
			ReadAccess		= convert(int,c.Flags & 1),
			PostAccess		= convert(int,c.Flags & 2),
			ReplyAccess		= convert(int,c.Flags & 4),
			PriorityAccess	= convert(int,c.Flags & 8),
			PollAccess		= convert(int,c.Flags & 16),
			VoteAccess		= convert(int,c.Flags & 32),
			ModeratorAccess	= convert(int,c.Flags & 64),
			EditAccess		= convert(int,c.Flags & 128),
			DeleteAccess	= convert(int,c.Flags & 256),
			UploadAccess	= convert(int,c.Flags & 512),
			DownloadAccess	= convert(int,c.Flags & 1024),
			AdminGroup		= convert(int,0)
		from
			[{databaseOwner}].[{objectQualifier}UserForum] b
			INNER JOIN [{databaseOwner}].[{objectQualifier}AccessMask] c on c.AccessMaskID=b.AccessMaskID' 
GO


IF NOT exists (select top 1 1 from dbo.sysobjects where id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccessfull]') AND OBJECTPROPERTY(id, N'IsView') = 1)
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [{databaseOwner}].[{objectQualifier}vaccessfull]
WITH SCHEMABINDING
AS

select 
			UserID,
			ForumID,
                      MAX(ReadAccess) AS ReadAccess, MAX(PostAccess) AS PostAccess, MAX(ReplyAccess) AS ReplyAccess, MAX(PriorityAccess) AS PriorityAccess, 
                      MAX(PollAccess) AS PollAccess, MAX(VoteAccess) AS VoteAccess, MAX(ModeratorAccess) AS ModeratorAccess, MAX(EditAccess) AS EditAccess, 
                      MAX(DeleteAccess) AS DeleteAccess, MAX(UploadAccess) AS UploadAccess, MAX(DownloadAccess) AS DownloadAccess, MAX(AdminGroup) as AdminGroup
		FROM (
		select
			UserID,
			ForumID,
			ReadAccess		,
			PostAccess		,
			ReplyAccess		,
			PriorityAccess	,
			PollAccess		,
			VoteAccess		,
			ModeratorAccess	,
			EditAccess		,
			DeleteAccess	,
			UploadAccess	,
			DownloadAccess	,
			AdminGroup		
		from
			[{databaseOwner}].[{objectQualifier}vaccess_user] b
		
		union all
		
		select
			UserID,
			ForumID,
			ReadAccess		,
			PostAccess		,
			ReplyAccess		,
			PriorityAccess	,
			PollAccess		,
			VoteAccess		,
			ModeratorAccess	,
			EditAccess		,
			DeleteAccess	,
			UploadAccess	,
			DownloadAccess	,
			AdminGroup	
		from
			[{databaseOwner}].[{objectQualifier}vaccess_group] b

		union all

		select
			UserID,
			ForumID,
			ReadAccess		,
			PostAccess		,
			ReplyAccess		,
			PriorityAccess	,
			PollAccess		,
			VoteAccess		,
			ModeratorAccess	,
			EditAccess		,
			DeleteAccess	,
			UploadAccess	,
			DownloadAccess	,
			AdminGroup	
		from
			[{databaseOwner}].[{objectQualifier}vaccess_null] b
) access
	GROUP BY
		UserID,ForumID' 
GO


IF NOT exists (select top 1 1 from dbo.sysobjects where id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess]') AND OBJECTPROPERTY(id, N'IsView') = 1)
EXEC dbo.sp_executesql @statement = N'/****** Object:  View [{databaseOwner}].[{objectQualifier}vaccess]    Script Date: 09/28/2009 22:26:00 ******/
CREATE VIEW [{databaseOwner}].[{objectQualifier}vaccess]
AS
	SELECT
		UserID				= a.UserID,
		ForumID				= x.ForumID,
		IsAdmin				= max(convert(int,b.Flags & 1)),
		IsForumModerator	= max(convert(int,b.Flags & 8)),
		IsModerator			= (select count(1) from [{databaseOwner}].[{objectQualifier}UserGroup] v,[{databaseOwner}].[{objectQualifier}Group] w,[{databaseOwner}].[{objectQualifier}ForumAccess] x,[{databaseOwner}].[{objectQualifier}AccessMask] y where v.UserID=a.UserID and w.GroupID=v.GroupID and x.GroupID=w.GroupID and y.AccessMaskID=x.AccessMaskID and (y.Flags & 64)<>0),
		ReadAccess			= max(x.ReadAccess),
		PostAccess			= max(x.PostAccess),
		ReplyAccess			= max(x.ReplyAccess),
		PriorityAccess		= max(x.PriorityAccess),
		PollAccess			= max(x.PollAccess),
		VoteAccess			= max(x.VoteAccess),
		ModeratorAccess		= max(x.ModeratorAccess),
		EditAccess			= max(x.EditAccess),
		DeleteAccess		= max(x.DeleteAccess),
		UploadAccess		= max(x.UploadAccess),		
		DownloadAccess		= max(x.DownloadAccess)			
	FROM
		[{databaseOwner}].[{objectQualifier}vaccessfull] as x WITH(NOLOCK)
		INNER JOIN [{databaseOwner}].[{objectQualifier}UserGroup] a WITH(NOLOCK) on a.UserID=x.UserID
		INNER JOIN [{databaseOwner}].[{objectQualifier}Group] b WITH(NOLOCK) on b.GroupID=a.GroupID
	GROUP BY
		a.UserID,x.ForumID' 
GO
