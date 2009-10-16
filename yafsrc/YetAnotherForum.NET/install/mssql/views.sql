SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_vaccess_group]') AND OBJECTPROPERTY(id, N'IsView') = 1)
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[yaf_vaccess_group]
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
			[dbo].[yaf_UserGroup] b
			INNER JOIN [dbo].[yaf_ForumAccess] c on c.GroupID=b.GroupID
			INNER JOIN [dbo].[yaf_AccessMask] d on d.AccessMaskID=c.AccessMaskID
			INNER JOIN [dbo].[yaf_Group] e on e.GroupID=b.GroupID' 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_PMessageView]') AND OBJECTPROPERTY(id, N'IsView') = 1)
EXEC dbo.sp_executesql @statement = N'-- [dbo].[yaf_PMessageView]

CREATE VIEW [dbo].[yaf_PMessageView]
AS
SELECT
	a.PMessageID, b.UserPMessageID, a.FromUserID, d.[Name] AS FromUser, 
	b.[UserID] AS ToUserId, c.[Name] AS ToUser, a.Created, a.Subject, 
	a.Body, a.Flags, b.IsRead, b.IsInOutbox, b.IsArchived, b.IsDeleted
FROM
	[dbo].[yaf_PMessage] a
INNER JOIN
	[dbo].[yaf_UserPMessage] b ON a.PMessageID = b.PMessageID
INNER JOIN
	[dbo].[yaf_User] c ON b.UserID = c.UserID
INNER JOIN
	[dbo].[yaf_User] d ON a.FromUserID = d.UserID' 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_vaccess_null]') AND OBJECTPROPERTY(id, N'IsView') = 1)
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[yaf_vaccess_null]
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
			[dbo].[yaf_User] a' 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_vaccess_user]') AND OBJECTPROPERTY(id, N'IsView') = 1)
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[yaf_vaccess_user]
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
			[dbo].[yaf_UserForum] b
			INNER JOIN [dbo].[yaf_AccessMask] c on c.AccessMaskID=b.AccessMaskID' 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_vaccessfull]') AND OBJECTPROPERTY(id, N'IsView') = 1)
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[yaf_vaccessfull]
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
			[dbo].[yaf_vaccess_user] b
		
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
			[dbo].[yaf_vaccess_group] b

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
			[dbo].[yaf_vaccess_null] b
) access
	GROUP BY
		UserID,ForumID' 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_vaccess]') AND OBJECTPROPERTY(id, N'IsView') = 1)
EXEC dbo.sp_executesql @statement = N'/****** Object:  View [dbo].[yaf_vaccess]    Script Date: 09/28/2009 22:26:00 ******/
CREATE VIEW [dbo].[yaf_vaccess]
AS
	SELECT
		UserID				= a.UserID,
		ForumID				= x.ForumID,
		IsAdmin				= max(convert(int,b.Flags & 1)),
		IsForumModerator	= max(convert(int,b.Flags & 8)),
		IsModerator			= (select count(1) from [dbo].[yaf_UserGroup] v,[dbo].[yaf_Group] w,[dbo].[yaf_ForumAccess] x,[dbo].[yaf_AccessMask] y where v.UserID=a.UserID and w.GroupID=v.GroupID and x.GroupID=w.GroupID and y.AccessMaskID=x.AccessMaskID and (y.Flags & 64)<>0),
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
		[dbo].[yaf_vaccessfull] as x WITH(NOLOCK)
		INNER JOIN [dbo].[yaf_UserGroup] a WITH(NOLOCK) on a.UserID=x.UserID
		INNER JOIN [dbo].[yaf_Group] b WITH(NOLOCK) on b.GroupID=a.GroupID
	GROUP BY
		a.UserID,x.ForumID' 
