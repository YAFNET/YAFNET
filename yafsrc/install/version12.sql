/* Version 0.9.6 */

if exists (select * from sysobjects where id = object_id(N'yaf_user_accessmasks') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_accessmasks
GO

create procedure yaf_user_accessmasks(@UserID int) as
begin
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
end
GO
