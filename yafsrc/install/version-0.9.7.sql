/* Version 0.9.7 */

-- yaf_UserPMessage
if not exists (select * from sysobjects where id = object_id(N'yaf_UserPMessage') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin
	create table yaf_UserPMessage(
		UserPMessageID	int identity not null,
		UserID			int not null,
		PMessageID		int not null,
		IsRead			bit not null
	)
	EXEC('insert into yaf_UserPMessage(UserID,PMessageID,IsRead) select ToUserID,PMessageID,IsRead from yaf_PMessage')
end
GO

if not exists(select * from syscolumns where id=object_id('yaf_UserPMessage') and name='UserPMessageID')
	alter table yaf_UserPMessage add UserPMessageID int identity not null
GO

if exists(select * from sysindexes where id=object_id('yaf_UserPMessage') and name='PK_UserPMessage')
	alter table yaf_UserPMessage drop constraint PK_UserPMessage
GO

if not exists(select * from sysindexes where id=object_id('yaf_UserPMessage') and name='PK_UserPMessage')
ALTER TABLE [yaf_UserPMessage] WITH NOCHECK ADD 
	CONSTRAINT [PK_UserPMessage] PRIMARY KEY  CLUSTERED 
	(
		[UserPMessageID]
	) 
GO

if not exists(select * from sysobjects where name='FK_UserPMessage_User' and parent_obj=object_id('yaf_UserPMessage') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_UserPMessage] ADD 
	CONSTRAINT [FK_UserPMessage_User] FOREIGN KEY 
	(
		[UserID]
	) REFERENCES [yaf_User] (
		[UserID]
	)
GO

if not exists(select * from sysobjects where name='FK_UserPMessage_PMessage' and parent_obj=object_id('yaf_UserPMessage') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_UserPMessage] ADD 
	CONSTRAINT [FK_UserPMessage_PMessage] FOREIGN KEY 
	(
		[PMessageID]
	) REFERENCES [yaf_PMessage] (
		[PMessageID]
	)
GO

-- yaf_PMessage
if exists(select * from sysobjects where name='FK_PMessage_User2' and parent_obj=object_id('yaf_PMessage') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	ALTER TABLE [yaf_PMessage] DROP CONSTRAINT [FK_PMessage_User2]
GO

if exists(select * from syscolumns where id=object_id('yaf_PMessage') and name='ToUserID')
	alter table yaf_PMessage drop column [ToUserID]
GO

if exists(select * from syscolumns where id=object_id('yaf_PMessage') and name='ToUserID')
	alter table yaf_PMessage drop column [ToUserID]
GO

if exists(select * from syscolumns where id=object_id('yaf_PMessage') and name='IsRead')
	alter table yaf_PMessage drop column [IsRead]
GO

-- PK_Attachment
if not exists(select * from sysindexes where id=object_id('yaf_Attachment') and name='PK_Attachment')
ALTER TABLE [yaf_Attachment] WITH NOCHECK ADD 
	CONSTRAINT [PK_Attachment] PRIMARY KEY  CLUSTERED 
	(
		[AttachmentID]
	) 
GO

-- yaf_board_delete
if exists (select * from sysobjects where id = object_id(N'yaf_board_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_board_delete
GO

create procedure yaf_board_delete(@BoardID int) as
begin
	delete from yaf_ForumAccess where exists(select 1 from yaf_Group x where x.GroupID=yaf_ForumAccess.GroupID and x.BoardID=@BoardID)
	delete from yaf_Forum where exists(select 1 from yaf_Category x where x.CategoryID=yaf_Forum.CategoryID and x.BoardID=@BoardID)
	delete from yaf_UserGroup where exists(select 1 from yaf_User x where x.UserID=yaf_UserGroup.UserID and x.BoardID=@BoardID)
	delete from yaf_Category where BoardID=@BoardID
	delete from yaf_User where BoardID=@BoardID
	delete from yaf_Rank where BoardID=@BoardID
	delete from yaf_Group where BoardID=@BoardID
	delete from yaf_AccessMask where BoardID=@BoardID
	 --ABOT NEW 09.04.2004
	delete from yaf_Active where BoardID=@BoardID
	 --END ABOT NEW 09.04.2004
	delete from yaf_Board where BoardID=@BoardID
end
GO

-- yaf_pmessage_info
if exists (select * from sysobjects where id = object_id(N'yaf_pmessage_info') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pmessage_info
GO

create procedure yaf_pmessage_info as
begin
	select
		NumRead	= (select count(1) from yaf_UserPMessage where IsRead<>0),
		NumUnread = (select count(1) from yaf_UserPMessage where IsRead=0),
		NumTotal = (select count(1) from yaf_UserPMessage)
end
GO

-- yaf_pmessage_prune

if exists (select * from sysobjects where id = object_id(N'yaf_pmessage_prune') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pmessage_prune
GO

create procedure yaf_pmessage_prune(@DaysRead int,@DaysUnread int) as
begin
	delete from yaf_UserPMessage
	where IsRead<>0
	and datediff(dd,(select Created from yaf_PMessage x where x.PMessageID=yaf_UserPMessage.PMessageID),getdate())>@DaysRead

	delete from yaf_UserPMessage
	where IsRead=0
	and datediff(dd,(select Created from yaf_PMessage x where x.PMessageID=yaf_UserPMessage.PMessageID),getdate())>@DaysUnread

	delete from yaf_PMessage
	where not exists(select 1 from yaf_UserPMessage x where x.PMessageID=yaf_PMessage.PMessageID)
end
GO

-- yaf_topic_delete
if exists (select * from sysobjects where id = object_id(N'yaf_topic_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_delete
GO

CREATE procedure yaf_topic_delete (@TopicID int,@UpdateLastPost bit=1) 
as
begin
	declare @ForumID int

	select @ForumID=ForumID from yaf_Topic where TopicID=@TopicID

	--begin transaction
	update yaf_Topic set LastMessageID = null where TopicID = @TopicID
	update yaf_Forum set 
		LastTopicID = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null,
		LastPosted = null
	where LastMessageID in (select MessageID from yaf_Message where TopicID = @TopicID)
	update yaf_Active set TopicID = null where TopicID = @TopicID
	--commit
	--begin transaction
	delete from yaf_NntpTopic where TopicID = @TopicID
	delete from yaf_WatchTopic where TopicID = @TopicID

	-- BAI CHANGED 01.02.2004
	-- Delete messages and attachments
	--delete from yaf_Message where TopicID = @TopicID

	declare @tmpMessageID int;
	declare msg_cursor cursor for
		select MessageID from yaf_message
		where TopicID = @TopicID
		order by MessageID desc
	
	open msg_cursor
	
	fetch next from msg_cursor
	into @tmpMessageID
	
	-- Check @@FETCH_STATUS to see if there are any more rows to fetch.
	while @@FETCH_STATUS = 0
	begin
		delete from yaf_attachment where MessageID = @tmpMessageID;
		delete from yaf_message where MessageID = @tmpMessageID;
	
	   -- This is executed as long as the previous fetch succeeds.
		fetch next from msg_cursor
		into @tmpMessageID
	end
	
	close msg_cursor
	deallocate msg_cursor

	-- Messagedelete finished
	-- ENDED BAI CHANGED 01.02.2004

	delete from yaf_Topic where TopicMovedID = @TopicID
	delete from yaf_Topic where TopicID = @TopicID
	--commit
	if @UpdateLastPost<>0
		exec yaf_topic_updatelastpost
	
	if @ForumID is not null
		exec yaf_forum_updatestats @ForumID
end
GO

-- yaf_message_delete
if exists (select * from sysobjects where id = object_id(N'yaf_message_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_delete
GO

CREATE   procedure yaf_message_delete(@MessageID int) as
begin
	declare @TopicID		int
	declare @ForumID		int
	declare @MessageCount	int
	declare @LastMessageID	int
	-- Find TopicID and ForumID
	select @TopicID=b.TopicID,@ForumID=b.ForumID from yaf_Message a,yaf_Topic b where a.MessageID=@MessageID and b.TopicID=a.TopicID
	-- Update LastMessageID in Topic and Forum
	update yaf_Topic set 
		LastPosted = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID
	update yaf_Forum set 
		LastPosted = null,
		LastTopicID = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	-- Delete attachments
	delete from yaf_attachment where MessageID = @MessageID

	-- Delete message
	delete from yaf_Message where MessageID = @MessageID
	-- Delete topic if there are no more messages
	select @MessageCount = count(1) from yaf_Message where TopicID = @TopicID
	if @MessageCount=0 exec yaf_topic_delete @TopicID
	-- update lastpost
	exec yaf_topic_updatelastpost @ForumID,@TopicID
	exec yaf_forum_updatestats @ForumID
	-- update topic numposts
	update yaf_Topic set
		NumPosts = (select count(1) from yaf_Message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0)
	where TopicID = @TopicID
end
GO

-- yaf_message_getReplies
if exists (select * from sysobjects where id = object_id(N'yaf_message_getReplies') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_getReplies
GO

CREATE procedure yaf_message_getReplies(@MessageID int) as
begin
	select MessageID from yaf_Message where ReplyTo = @MessageID
end
GO

-- yaf_pmessage_delete
if exists (select * from sysobjects where id = object_id(N'yaf_pmessage_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pmessage_delete
GO

create procedure yaf_pmessage_delete(@PMessageID int) as
begin
	delete from yaf_PMessage where PMessageID=@PMessageID
end
GO

-- yaf_userpmessage_delete
if exists (select * from sysobjects where id = object_id(N'yaf_userpmessage_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_userpmessage_delete
GO

create procedure yaf_userpmessage_delete(@UserPMessageID int) as
begin
	delete from yaf_UserPMessage where UserPMessageID=@UserPMessageID
end
GO

-- yaf_pmessage_list
if exists (select * from sysobjects where id = object_id(N'yaf_pmessage_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pmessage_list
GO

create procedure yaf_pmessage_list(@FromUserID int=null,@ToUserID int=null,@PMessageID int=null) as
begin
	if @PMessageID is null begin
		select
			a.*,
			FromUser = b.Name,
			ToUserID = c.UserID,
			ToUser = c.Name,
			d.IsRead,
			d.UserPMessageID
		from
			yaf_PMessage a,
			yaf_User b,
			yaf_User c,
			yaf_UserPMessage d
		where
			b.UserID = a.FromUserID and
			c.UserID = d.UserID and
			d.PMessageID = a.PMessageID and
			((@ToUserID is not null and d.UserID = @ToUserID) or (@FromUserID is not null and a.FromUserID = @FromUserID))
		order by
			Created desc
	end
	else begin
		select
			a.*,
			FromUser = b.Name,
			ToUserID = c.UserID,
			ToUser = c.Name,
			d.IsRead,
			d.UserPMessageID
		from
			yaf_PMessage a,
			yaf_User b,
			yaf_User c,
			yaf_UserPMessage d
		where
			b.UserID = a.FromUserID and
			c.UserID = d.UserID and
			d.PMessageID = a.PMessageID and
			a.PMessageID = @PMessageID and
			c.UserID = @FromUserID
		order by
			Created desc
	end
end
GO

-- yaf_userpmessage_list
if exists (select * from sysobjects where id = object_id(N'yaf_userpmessage_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_userpmessage_list
GO

create procedure yaf_userpmessage_list(@UserPMessageID int) as
begin
	select
		a.*,
		FromUser = b.Name,
		ToUserID = c.UserID,
		ToUser = c.Name,
		d.IsRead,
		d.UserPMessageID
	from
		yaf_PMessage a,
		yaf_User b,
		yaf_User c,
		yaf_UserPMessage d
	where
		b.UserID = a.FromUserID and
		c.UserID = d.UserID and
		d.PMessageID = a.PMessageID and
		d.UserPMessageID = @UserPMessageID
end
GO

-- yaf_forum_delete
if exists (select * from sysobjects where id = object_id(N'yaf_forum_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_delete
GO

create procedure yaf_forum_delete(@ForumID int) as
begin
	-- Maybe an idea to use cascading foreign keys instead? Too bad they don't work on MS SQL 7.0...
	update yaf_Forum set LastMessageID=null,LastTopicID=null where ForumID=@ForumID
	update yaf_Topic set LastMessageID=null where ForumID=@ForumID
	delete from yaf_WatchTopic from yaf_Topic where yaf_Topic.ForumID = @ForumID and yaf_WatchTopic.TopicID = yaf_Topic.TopicID

	delete from yaf_NntpTopic from yaf_NntpForum where yaf_NntpForum.ForumID = @ForumID and yaf_NntpTopic.NntpForumID = yaf_NntpForum.NntpForumID
	delete from yaf_NntpForum where ForumID=@ForumID	
	delete from yaf_WatchForum where ForumID = @ForumID

	-- BAI CHANGED 02.02.2004
	-- Delete topics, messages and attachments

	declare @tmpTopicID int;
	declare topic_cursor cursor for
		select TopicID from yaf_topic
		where ForumId = @ForumID
		order by TopicID desc
	
	open topic_cursor
	
	fetch next from topic_cursor
	into @tmpTopicID
	
	-- Check @@FETCH_STATUS to see if there are any more rows to fetch.
	while @@FETCH_STATUS = 0
	begin
		exec yaf_topic_delete @tmpTopicID;
	
	   -- This is executed as long as the previous fetch succeeds.
		fetch next from topic_cursor
		into @tmpTopicID
	end
	
	close topic_cursor
	deallocate topic_cursor

	--delete from yaf_Message from yaf_Topic where yaf_Topic.ForumID = @ForumID and yaf_Message.TopicID = yaf_Topic.TopicID
	--delete from yaf_Topic where ForumID = @ForumID

	-- TopicDelete finished
	-- END BAI CHANGED 02.02.2004

	delete from yaf_ForumAccess where ForumID = @ForumID
	delete from yaf_Forum where ForumID = @ForumID
end
GO

-- yaf_user_list
if exists (select * from sysobjects where id = object_id(N'yaf_user_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_list
GO

create procedure yaf_user_list(@BoardID int,@UserID int=null,@Approved bit=null) as
begin
	if @UserID is null
		select 
			a.*,
			a.NumPosts,
			IsAdmin = (select count(1) from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and y.IsAdmin<>0),
			b.RankID,
			RankName = b.Name
		from 
			yaf_User a
			join yaf_Rank b on b.RankID=a.RankID
		where 
			a.BoardID = @BoardID and
			(@Approved is null or a.Approved = @Approved)
		order by 
			a.Name
	else
		select 
			a.*,
			a.NumPosts,
			b.RankID,
			RankName = b.Name,
			NumDays = datediff(d,a.Joined,getdate())+1,
			NumPostsForum = (select count(1) from yaf_Message x where x.Approved<>0),
			HasAvatarImage = (select count(1) from yaf_User x where x.UserID=a.UserID and AvatarImage is not null),
			IsAdmin	= IsNull(c.IsAdmin,0),
			IsGuest	= IsNull(c.IsGuest,0),
			IsForumModerator	= IsNull(c.IsForumModerator,0),
			IsModerator		= IsNull(c.IsModerator,0)
		from 
			yaf_User a
			join yaf_Rank b on b.RankID=a.RankID
			left join yaf_vaccess c on c.UserID=a.UserID
		where 
			a.UserID = @UserID and
			a.BoardID = @BoardID and
			IsNull(c.ForumID,0) = 0 and
			(@Approved is null or a.Approved = @Approved)
		order by 
			a.Name
end
GO
-- yaf_forum_listallmymoderated ABOT NEW 16.04.04
if exists (select * from sysobjects where id = object_id(N'yaf_forum_listallmymoderated') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_listallmymoderated
GO

create procedure yaf_forum_listallmymoderated(@BoardID int,@UserID int) as
begin
	select
		b.CategoryID,
		Category = b.Name,
		a.ForumID,
		Forum = a.Name,
		x.Indent
	from
		(select
			b.ForumID,
			Indent = 0
		from
			yaf_Category a
			join yaf_Forum b on b.CategoryID=a.CategoryID
		where
			a.BoardID=@BoardID and
			b.ParentID is null
	
		union
	
		select
			c.ForumID,
			Indent = 1
		from
			yaf_Category a
			join yaf_Forum b on b.CategoryID=a.CategoryID
			join yaf_Forum c on c.ParentID=b.ForumID
		where
			a.BoardID=@BoardID and
			b.ParentID is null
	
		union
	
		select
			d.ForumID,
			Indent = 2
		from
			yaf_Category a
			join yaf_Forum b on b.CategoryID=a.CategoryID
			join yaf_Forum c on c.ParentID=b.ForumID
			join yaf_Forum d on d.ParentID=c.ForumID
		where
			a.BoardID=@BoardID and
			b.ParentID is null
		) as x
		join yaf_Forum a on a.ForumID=x.ForumID
		join yaf_Category b on b.CategoryID=a.CategoryID
		join yaf_vaccess c on c.ForumID=a.ForumID
	where
		c.UserID=@UserID and
		b.BoardID=@BoardID and
		c.ModeratorAccess>0
	order by
		b.SortOrder,
		a.SortOrder
end
GO
-- END ABOT NEW 16.04.04
-- yaf_forum_listall
if exists (select * from sysobjects where id = object_id(N'yaf_forum_listall') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_listall
GO

create procedure yaf_forum_listall(@BoardID int,@UserID int) as
begin
	select
		b.CategoryID,
		Category = b.Name,
		a.ForumID,
		Forum = a.Name,
		x.Indent
	from
		(select
			b.ForumID,
			Indent = 0
		from
			yaf_Category a
			join yaf_Forum b on b.CategoryID=a.CategoryID
		where
			a.BoardID=@BoardID and
			b.ParentID is null
	
		union
	
		select
			c.ForumID,
			Indent = 1
		from
			yaf_Category a
			join yaf_Forum b on b.CategoryID=a.CategoryID
			join yaf_Forum c on c.ParentID=b.ForumID
		where
			a.BoardID=@BoardID and
			b.ParentID is null
	
		union
	
		select
			d.ForumID,
			Indent = 2
		from
			yaf_Category a
			join yaf_Forum b on b.CategoryID=a.CategoryID
			join yaf_Forum c on c.ParentID=b.ForumID
			join yaf_Forum d on d.ParentID=c.ForumID
		where
			a.BoardID=@BoardID and
			b.ParentID is null
		) as x
		join yaf_Forum a on a.ForumID=x.ForumID
		join yaf_Category b on b.CategoryID=a.CategoryID
		join yaf_vaccess c on c.ForumID=a.ForumID
	where
		c.UserID=@UserID and
		b.BoardID=@BoardID and
		c.ReadAccess>0
	order by
		b.SortOrder,
		a.SortOrder
end
GO
