/* Version 0.9.x */

if exists (select * from sysobjects where id = object_id(N'yaf_forum_listread') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_listread
GO

create procedure yaf_forum_listread(@UserID int,@CategoryID int=null) as
begin
	select 
		a.CategoryID, 
		Category		= a.Name, 
		ForumID			= b.ForumID,
		Forum			= b.Name, 
		Description,
		Topics			= (select count(distinct x.TopicID) from yaf_Topic x,yaf_Message y where x.ForumID=b.ForumID and y.TopicID=x.TopicID and y.Approved<>0),
		Posts			= (select count(1) from yaf_Message x,yaf_Topic y where y.TopicID=x.TopicID and y.ForumID = b.ForumID and x.Approved<>0),
		LastPosted		= b.LastPosted,
		LastMessageID	= b.LastMessageID,
		LastUserID		= b.LastUserID,
		LastUser		= IsNull(b.LastUserName,(select Name from yaf_User x where x.UserID=b.LastUserID)),
		LastTopicID		= b.LastTopicID,
		LastTopicName	= (select x.Topic from yaf_Topic x where x.TopicID=b.LastTopicID),
		b.Locked,
		b.Moderated,
		PostAccess		= (select count(1) from yaf_UserGroup x,yaf_ForumAccess y where x.UserID=@UserID and x.GroupID=y.GroupID and y.ForumID=b.ForumID and y.PostAccess<>0),
		ReplyAccess		= (select count(1) from yaf_UserGroup x,yaf_ForumAccess y where x.UserID=@UserID and x.GroupID=y.GroupID and y.ForumID=b.ForumID and y.ReplyAccess<>0),
		ReadAccess		= (select count(1) from yaf_UserGroup x,yaf_ForumAccess y where x.UserID=@UserID and x.GroupID=y.GroupID and y.ForumID=b.ForumID and y.ReadAccess<>0)		
	from 
		yaf_Category a, 
		yaf_Forum b
	where 
		a.CategoryID = b.CategoryID and
		(b.Hidden=0 or exists(select 1 from yaf_UserGroup x,yaf_ForumAccess y where x.UserID=@UserID and x.GroupID=y.GroupID and y.ForumID=b.ForumID and y.ReadAccess<>0)) and
		(@CategoryID is null or a.CategoryID = @CategoryID)
	order by
		a.SortOrder,
		b.SortOrder
end
GO
