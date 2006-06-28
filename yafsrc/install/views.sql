/*
** Views
*/

if exists (select 1 from sysobjects where id = object_id(N'yaf_vaccess') and OBJECTPROPERTY(id, N'IsView') = 1)
	drop view yaf_vaccess
GO

create view dbo.yaf_vaccess as
	select
		UserID				= a.UserID,
		ForumID				= x.ForumID,
		IsAdmin				= max(convert(int,b.Flags & 1)),
		IsForumModerator	= max(convert(int,b.Flags & 8)),
		IsModerator			= (select count(1) from dbo.yaf_UserGroup v,dbo.yaf_Group w,dbo.yaf_ForumAccess x,dbo.yaf_AccessMask y where v.UserID=a.UserID and w.GroupID=v.GroupID and x.GroupID=w.GroupID and y.AccessMaskID=x.AccessMaskID and (y.Flags & 64)<>0),
		ReadAccess			= max(x.ReadAccess),
		PostAccess			= max(x.PostAccess),
		ReplyAccess			= max(x.ReplyAccess),
		PriorityAccess		= max(x.PriorityAccess),
		PollAccess			= max(x.PollAccess),
		VoteAccess			= max(x.VoteAccess),
		ModeratorAccess		= max(x.ModeratorAccess),
		EditAccess			= max(x.EditAccess),
		DeleteAccess		= max(x.DeleteAccess),
		UploadAccess		= max(x.UploadAccess)
	from
		(select
			b.UserID,
			b.ForumID,
			ReadAccess		= convert(int,c.Flags & 1),
			PostAccess		= convert(int,c.Flags & 2),
			ReplyAccess		= convert(int,c.Flags & 4),
			PriorityAccess		= convert(int,c.Flags & 8),
			PollAccess		= convert(int,c.Flags & 16),
			VoteAccess		= convert(int,c.Flags & 32),
			ModeratorAccess		= convert(int,c.Flags & 64),
			EditAccess		= convert(int,c.Flags & 128),
			DeleteAccess		= convert(int,c.Flags & 256),
			UploadAccess		= convert(int,c.Flags & 512)
		from
			dbo.yaf_UserForum b
			join dbo.yaf_AccessMask c on c.AccessMaskID=b.AccessMaskID
		
		union
		
		select
			b.UserID,
			c.ForumID,
			ReadAccess		= convert(int,d.Flags & 1),
			PostAccess		= convert(int,d.Flags & 2),
			ReplyAccess		= convert(int,d.Flags & 4),
			PriorityAccess		= convert(int,d.Flags & 8),
			PollAccess		= convert(int,d.Flags & 16),
			VoteAccess		= convert(int,d.Flags & 32),
			ModeratorAccess		= convert(int,d.Flags & 64),
			EditAccess		= convert(int,d.Flags & 128),
			DeleteAccess		= convert(int,d.Flags & 256),
			UploadAccess		= convert(int,d.Flags & 512)
		from
			dbo.yaf_UserGroup b
			join dbo.yaf_ForumAccess c on c.GroupID=b.GroupID
			join dbo.yaf_AccessMask d on d.AccessMaskID=c.AccessMaskID

		union

		select
			a.UserID,
			ForumID			= convert(int,0),
			ReadAccess		= convert(int,0),
			PostAccess		= convert(int,0),
			ReplyAccess		= convert(int,0),
			PriorityAccess	= convert(int,0),
			PollAccess		= convert(int,0),
			VoteAccess		= convert(int,0),
			ModeratorAccess	= convert(int,0),
			EditAccess		= convert(int,0),
			DeleteAccess	= convert(int,0),
			UploadAccess	= convert(int,0)
		from
			dbo.yaf_User a
		) as x
		join dbo.yaf_UserGroup a on a.UserID=x.UserID
		join dbo.yaf_Group b on b.GroupID=a.GroupID
	group by a.UserID,x.ForumID
GO
