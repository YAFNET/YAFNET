/* Version 0.9.6 */

if exists (select * from sysobjects where id = object_id(N'yaf_Buddy') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table yaf_Buddy
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_accessmasks') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_accessmasks
GO

create procedure yaf_user_accessmasks(@UserID int) as
begin
	select * from(
		select
			AccessMaskID	= e.AccessMaskID,
			AccessMaskName	= e.Name,
			ForumID		= f.ForumID,
			ForumName	= f.Name
		from
			yaf_User a 
			join yaf_UserGroup b on b.UserID=a.UserID
			join yaf_Group c on c.GroupID=b.GroupID
			join yaf_ForumAccess d on d.GroupID=c.GroupID
			join yaf_AccessMask e on e.AccessMaskID=d.AccessMaskID
			join yaf_Forum f on f.ForumID=d.ForumID
		where
			a.UserID=@UserID
		group by
			e.AccessMaskID,
			e.Name,
			f.ForumID,
			f.Name
		
		union
			
		select
			AccessMaskID	= c.AccessMaskID,
			AccessMaskName	= c.Name,
			ForumID		= d.ForumID,
			ForumName	= d.Name
		from
			yaf_User a 
			join yaf_UserForum b on b.UserID=a.UserID
			join yaf_AccessMask c on c.AccessMaskID=b.AccessMaskID
			join yaf_Forum d on d.ForumID=b.ForumID
		where
			a.UserID=@UserID
		group by
			c.AccessMaskID,
			c.Name,
			d.ForumID,
			d.Name
	) as x
	order by
		ForumName, AccessMaskName
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_message_findunread') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_findunread
GO

create procedure yaf_message_findunread(@TopicID int,@LastRead datetime) as
begin
	select top 1 MessageID from yaf_Message
	where TopicID=@TopicID and Posted>@LastRead
	order by Posted
end
go
