/*
** Views
*/

<<<<<<< .mine
if exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}vaccess') and OBJECTPROPERTY(id, N'IsView') = 1)
	drop view [{databaseOwner}].{objectQualifier}vaccess
=======
if exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_vaccess]') and OBJECTPROPERTY(id, N'IsView') = 1)
	drop view [{databaseOwner}].[yaf_vaccess]
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}vmaccess') and OBJECTPROPERTY(id, N'IsView') = 1)
	drop view [{databaseOwner}].{objectQualifier}vmaccess
=======
if exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_vmaccess]') and OBJECTPROPERTY(id, N'IsView') = 1)
	drop view [{databaseOwner}].[yaf_vmaccess]
>>>>>>> .r1490
GO

<<<<<<< .mine
if exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].{objectQualifier}PMessageView') and OBJECTPROPERTY(id, N'IsView') = 1)
	drop view [{databaseOwner}].{objectQualifier}PMessageView
=======
if exists (select 1 from sysobjects where id = object_id(N'[{databaseOwner}].[yaf_PMessageView]') and OBJECTPROPERTY(id, N'IsView') = 1)
	drop view [{databaseOwner}].[yaf_PMessageView]
>>>>>>> .r1490
GO

<<<<<<< .mine
create view [{databaseOwner}].{objectQualifier}vaccess as
=======
create view [{databaseOwner}].[yaf_vaccess] as
>>>>>>> .r1490
	select
		UserID				= a.UserID,
		ForumID				= x.ForumID,
		IsAdmin				= max(convert(int,b.Flags & 1)),
		IsForumModerator	= max(convert(int,b.Flags & 8)),
<<<<<<< .mine
		IsModerator			= (select count(1) from [{databaseOwner}].{objectQualifier}UserGroup v,[{databaseOwner}].{objectQualifier}Group w,[{databaseOwner}].{objectQualifier}ForumAccess x,[{databaseOwner}].{objectQualifier}AccessMask y where v.UserID=a.UserID and w.GroupID=v.GroupID and x.GroupID=w.GroupID and y.AccessMaskID=x.AccessMaskID and (y.Flags & 64)<>0),
=======
		IsModerator			= (select count(1) from [{databaseOwner}].[yaf_UserGroup] v,[{databaseOwner}].[yaf_Group] w,[{databaseOwner}].[yaf_ForumAccess] x,[{databaseOwner}].[yaf_AccessMask] y where v.UserID=a.UserID and w.GroupID=v.GroupID and x.GroupID=w.GroupID and y.AccessMaskID=x.AccessMaskID and (y.Flags & 64)<>0),
>>>>>>> .r1490
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
<<<<<<< .mine
			[{databaseOwner}].{objectQualifier}UserForum b
			join [{databaseOwner}].{objectQualifier}AccessMask c on c.AccessMaskID=b.AccessMaskID
=======
			[{databaseOwner}].[yaf_UserForum] b
			join [{databaseOwner}].[yaf_AccessMask] c on c.AccessMaskID=b.AccessMaskID
>>>>>>> .r1490
		
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
<<<<<<< .mine
			[{databaseOwner}].{objectQualifier}UserGroup b
			join [{databaseOwner}].{objectQualifier}ForumAccess c on c.GroupID=b.GroupID
			join [{databaseOwner}].{objectQualifier}AccessMask d on d.AccessMaskID=c.AccessMaskID
=======
			[{databaseOwner}].[yaf_UserGroup] b
			join [{databaseOwner}].[yaf_ForumAccess] c on c.GroupID=b.GroupID
			join [{databaseOwner}].[yaf_AccessMask] d on d.AccessMaskID=c.AccessMaskID
>>>>>>> .r1490

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
<<<<<<< .mine
			[{databaseOwner}].{objectQualifier}User a
=======
			[{databaseOwner}].[yaf_User] a
>>>>>>> .r1490
		) as x
<<<<<<< .mine
		join [{databaseOwner}].{objectQualifier}UserGroup a on a.UserID=x.UserID
		join [{databaseOwner}].{objectQualifier}Group b on b.GroupID=a.GroupID
=======
		join [{databaseOwner}].[yaf_UserGroup] a on a.UserID=x.UserID
		join [{databaseOwner}].[yaf_Group] b on b.GroupID=a.GroupID
>>>>>>> .r1490
	group by a.UserID,x.ForumID
GO

<<<<<<< .mine
create view [{databaseOwner}].[{objectQualifier}vmaccess] as
=======
create view [{databaseOwner}].[yaf_vmaccess] as
>>>>>>> .r1490
	select
		UserID				= a.UserID,
		ForumID				= x.ForumID,
		IsForumModerator	= max(convert(int,b.Flags & 8)),
<<<<<<< .mine
		IsModerator			= (select count(1) from [{databaseOwner}].{objectQualifier}UserGroup v,[{databaseOwner}].{objectQualifier}Group w,[{databaseOwner}].{objectQualifier}ForumAccess x,[{databaseOwner}].{objectQualifier}AccessMask y where v.UserID=a.UserID and w.GroupID=v.GroupID and x.GroupID=w.GroupID and y.AccessMaskID=x.AccessMaskID and (y.Flags & 64)<>0),
=======
		IsModerator			= (select count(1) from [{databaseOwner}].[yaf_UserGroup] v,[{databaseOwner}].[yaf_Group] w,[{databaseOwner}].[yaf_ForumAccess] x,[{databaseOwner}].[yaf_AccessMask] y where v.UserID=a.UserID and w.GroupID=v.GroupID and x.GroupID=w.GroupID and y.AccessMaskID=x.AccessMaskID and (y.Flags & 64)<>0),
>>>>>>> .r1490
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
<<<<<<< .mine
			[{databaseOwner}].{objectQualifier}UserForum b
			join [{databaseOwner}].{objectQualifier}AccessMask c on c.AccessMaskID=b.AccessMaskID
=======
			[{databaseOwner}].[yaf_UserForum] b
			join [{databaseOwner}].[yaf_AccessMask] c on c.AccessMaskID=b.AccessMaskID
>>>>>>> .r1490
		
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
<<<<<<< .mine
			[{databaseOwner}].{objectQualifier}UserGroup b
			join [{databaseOwner}].{objectQualifier}ForumAccess c on c.GroupID=b.GroupID
			join [{databaseOwner}].{objectQualifier}AccessMask d on d.AccessMaskID=c.AccessMaskID
			join [{databaseOwner}].{objectQualifier}Group e on e.GroupID=b.GroupID
=======
			[{databaseOwner}].[yaf_UserGroup] b
			join [{databaseOwner}].[yaf_ForumAccess] c on c.GroupID=b.GroupID
			join [{databaseOwner}].[yaf_AccessMask] d on d.AccessMaskID=c.AccessMaskID
			join [{databaseOwner}].[yaf_Group] e on e.GroupID=b.GroupID
>>>>>>> .r1490
		where 
			(e.Flags & 1) = 0 --is not admin group

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
<<<<<<< .mine
			[{databaseOwner}].{objectQualifier}User a
=======
			[{databaseOwner}].[yaf_User] a
>>>>>>> .r1490
		) as x
<<<<<<< .mine
		join [{databaseOwner}].{objectQualifier}UserGroup a on a.UserID=x.UserID
		join [{databaseOwner}].{objectQualifier}Group b on b.GroupID=a.GroupID
=======
		join [{databaseOwner}].[yaf_UserGroup] a on a.UserID=x.UserID
		join [{databaseOwner}].[yaf_Group] b on b.GroupID=a.GroupID
>>>>>>> .r1490

	group by a.UserID,x.ForumID
GO

-- [{databaseOwner}].{objectQualifier}PMessageView

<<<<<<< .mine
CREATE VIEW [{databaseOwner}].[{objectQualifier}PMessageView]
=======
CREATE VIEW [{databaseOwner}].[yaf_PMessageView]
>>>>>>> .r1490
AS
<<<<<<< .mine
SELECT     [{databaseOwner}].{objectQualifier}PMessage.PMessageID, [{databaseOwner}].{objectQualifier}UserPMessage.UserPMessageID, [{databaseOwner}].{objectQualifier}PMessage.FromUserID, [{databaseOwner}].{objectQualifier}User_From.Name AS FromUser, 
                      [{databaseOwner}].{objectQualifier}UserPMessage.UserID AS ToUserId, [{databaseOwner}].{objectQualifier}User.Name AS ToUser, [{databaseOwner}].{objectQualifier}PMessage.Created, [{databaseOwner}].{objectQualifier}PMessage.Subject, 
                      [{databaseOwner}].{objectQualifier}PMessage.Body, [{databaseOwner}].{objectQualifier}PMessage.Flags, [{databaseOwner}].{objectQualifier}UserPMessage.IsRead, [{databaseOwner}].{objectQualifier}UserPMessage.IsInOutbox, [{databaseOwner}].{objectQualifier}UserPMessage.IsArchived
FROM         [{databaseOwner}].{objectQualifier}PMessage INNER JOIN
                      [{databaseOwner}].{objectQualifier}UserPMessage ON [{databaseOwner}].{objectQualifier}PMessage.PMessageID = [{databaseOwner}].{objectQualifier}UserPMessage.PMessageID INNER JOIN
                      [{databaseOwner}].{objectQualifier}User ON [{databaseOwner}].{objectQualifier}UserPMessage.UserID = [{databaseOwner}].{objectQualifier}User.UserID INNER JOIN
                      [{databaseOwner}].{objectQualifier}User AS [{databaseOwner}].{objectQualifier}User_From ON [{databaseOwner}].{objectQualifier}PMessage.FromUserID = [{databaseOwner}].{objectQualifier}User_From.UserID
=======
SELECT     [{databaseOwner}].[yaf_PMessage].PMessageID, [{databaseOwner}].[yaf_UserPMessage].UserPMessageID, [{databaseOwner}].[yaf_PMessage].FromUserID, yaf_User_From.Name AS FromUser, 
                      [{databaseOwner}].[yaf_UserPMessage].UserID AS ToUserId, [{databaseOwner}].[yaf_User].Name AS ToUser, [{databaseOwner}].[yaf_PMessage].Created, [{databaseOwner}].[yaf_PMessage].Subject, 
                      [{databaseOwner}].[yaf_PMessage].Body, [{databaseOwner}].[yaf_PMessage].Flags, [{databaseOwner}].[yaf_UserPMessage].IsRead, [{databaseOwner}].[yaf_UserPMessage].IsInOutbox, [{databaseOwner}].[yaf_UserPMessage].IsArchived
FROM         [{databaseOwner}].[yaf_PMessage] INNER JOIN
                      [{databaseOwner}].[yaf_UserPMessage] ON [{databaseOwner}].[yaf_PMessage].PMessageID = [{databaseOwner}].[yaf_UserPMessage].PMessageID INNER JOIN
                      [{databaseOwner}].[yaf_User] ON [{databaseOwner}].[yaf_UserPMessage].UserID = [{databaseOwner}].[yaf_User].UserID INNER JOIN
                      [{databaseOwner}].[yaf_User] AS yaf_User_From ON [{databaseOwner}].[yaf_PMessage].FromUserID = yaf_User_From.UserID
>>>>>>> .r1490

GO
