/* Version 0.9.7 */

-- yaf_UserPMessage
if not exists (select * from sysobjects where id = object_id(N'yaf_UserPMessage') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin
	create table yaf_UserPMessage(
		UserID		int not null,
		PMessageID	int not null,
		IsRead		bit not null
	)
	EXEC('insert into yaf_UserPMessage(UserID,PMessageID,IsRead) select ToUserID,PMessageID,IsRead from yaf_PMessage')
end
GO

if not exists(select * from sysindexes where id=object_id('yaf_UserPMessage') and name='PK_UserPMessage')
ALTER TABLE [yaf_UserPMessage] WITH NOCHECK ADD 
	CONSTRAINT [PK_UserPMessage] PRIMARY KEY  CLUSTERED 
	(
		[UserID],[PMessageID]
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

-- yaf_user_login
if exists (select * from sysobjects where id = object_id(N'yaf_user_login') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_login
GO

create procedure yaf_user_login(@BoardID int,@Name varchar(50),@Password varchar(32)) as
begin
	declare @UserID int

	if not exists(select UserID from yaf_User where [Name]=@Name and [Password]=@Password and (BoardID=@BoardID or IsHostAdmin<>0) and Approved<>0)
		set @UserID=null
	else
		select 
			@UserID=UserID 
		from 
			yaf_User 
		where 
			[Name]=@Name and 
			[Password]=@Password and 
			(BoardID=@BoardID or IsHostAdmin<>0) and
			Approved<>0

	select @UserID
end
GO

-- yaf_user_save
if exists (select * from sysobjects where id = object_id(N'yaf_user_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_save
GO

create procedure yaf_user_save(
	@UserID			int,
	@BoardID		int,
	@UserName		varchar(50) = null,
	@Password		varchar(32) = null,
	@Email			varchar(50) = null,
	@Hash			varchar(32) = null,
	@Location		varchar(50) = null,
	@HomePage		varchar(50) = null,
	@TimeZone		int,
	@Avatar			varchar(100) = null,
	@LanguageFile	varchar(50) = null,
	@ThemeFile		varchar(50) = null,
	@Approved		bit = null,
	@MSN			varchar(50) = null,
	@YIM			varchar(30) = null,
	@AIM			varchar(30) = null,
	@ICQ			int = null,
	@RealName		varchar(50) = null,
	@Occupation		varchar(50) = null,
	@Interests		varchar(100) = null,
	@Gender			tinyint = 0,
	@Weblog			varchar(100) = null
) as
begin
	declare @RankID int

	if @Location is not null and @Location = '' set @Location = null
	if @HomePage is not null and @HomePage = '' set @HomePage = null
	if @Avatar is not null and @Avatar = '' set @Avatar = null
	if @MSN is not null and @MSN = '' set @MSN = null
	if @YIM is not null and @YIM = '' set @YIM = null
	if @AIM is not null and @AIM = '' set @AIM = null
	if @ICQ is not null and @ICQ = 0 set @ICQ = null
	if @RealName is not null and @RealName = '' set @RealName = null
	if @Occupation is not null and @Occupation = '' set @Occupation = null
	if @Interests is not null and @Interests = '' set @Interests = null
	if @Weblog is not null and @Weblog = '' set @Weblog = null

	if @UserID is null or @UserID<1 begin
		if @Email = '' set @Email = null
		
		select @RankID = RankID from yaf_Rank where IsStart<>0
		
		insert into yaf_User(BoardID,RankID,Name,Password,Email,Joined,LastVisit,NumPosts,Approved,Location,HomePage,TimeZone,Avatar,Gender,IsHostAdmin) 
		values(@BoardID,@RankID,@UserName,@Password,@Email,getdate(),getdate(),0,@Approved,@Location,@HomePage,@TimeZone,@Avatar,@Gender,0)
	
		set @UserID = @@IDENTITY

		insert into yaf_UserGroup(UserID,GroupID) select @UserID,GroupID from yaf_Group where IsStart<>0
		
		if @Hash is not null and @Hash <> '' and @Approved=0 begin
			insert into yaf_CheckEmail(UserID,Email,Created,Hash)
			values(@UserID,@Email,getdate(),@Hash)	
		end
	end
	else begin
		update yaf_User set
			Location = @Location,
			HomePage = @HomePage,
			TimeZone = @TimeZone,
			Avatar = @Avatar,
			LanguageFile = @LanguageFile,
			ThemeFile = @ThemeFile,
			MSN = @MSN,
			YIM = @YIM,
			AIM = @AIM,
			ICQ = @ICQ,
			RealName = @RealName,
			Occupation = @Occupation,
			Interests = @Interests,
			Gender = @Gender,
			Weblog = @Weblog
		where UserID = @UserID
		
		if @Email is not null
			update yaf_User set Email = @Email where UserID = @UserID
	end
end
GO

if exists(select 1 from yaf_User where Weblog is not null and Weblog='')
	update yaf_User set Weblog=null where Weblog is not null and Weblog=''
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
	delete from yaf_Board where BoardID=@BoardID
end
GO

-- yaf_pmessage_list

if exists (select * from sysobjects where id = object_id(N'yaf_pmessage_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pmessage_list
GO

create procedure yaf_pmessage_list(@UserID int=null,@Sent bit=null,@PMessageID int=null) as
begin
	if @PMessageID is null begin
		select
			a.*,
			FromUser = b.Name,
			ToUserID = c.UserID,
			ToUser = c.Name,
			d.IsRead
		from
			yaf_PMessage a,
			yaf_User b,
			yaf_User c,
			yaf_UserPMessage d
		where
			b.UserID = a.FromUserID and
			c.UserID = d.UserID and
			d.PMessageID = a.PMessageID and
			((@Sent=0 and d.UserID = @UserID) or (@Sent=1 and a.FromUserID = @UserID))
		order by
			Created desc
	end
	else begin
		select
			a.*,
			FromUser = b.Name,
			ToUserID = c.UserID,
			ToUser = c.Name,
			d.IsRead
		from
			yaf_PMessage a,
			yaf_User b,
			yaf_User c,
			yaf_UserPMessage d
		where
			b.UserID = a.FromUserID and
			c.UserID = d.UserID and
			d.PMessageID = a.PMessageID and
			a.PMessageID = @PMessageID
		order by
			Created desc
	end
end
GO

-- yaf_pmessage_markread

if exists (select * from sysobjects where id = object_id(N'yaf_pmessage_markread') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pmessage_markread
GO

create procedure yaf_pmessage_markread(@UserID int,@PMessageID int=null) as begin
	if @PMessageID is null
		update yaf_UserPMessage set IsRead=1 where UserID=@UserID
	else
		update yaf_UserPMessage set IsRead=1 where UserID=@UserID and PMessageID=@PMessageID
end
GO

-- yaf_pmessage_save

if exists (select * from sysobjects where id = object_id(N'yaf_pmessage_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pmessage_save
GO

create procedure yaf_pmessage_save(
	@FromUserID	int,
	@ToUserID	int,
	@Subject	varchar(100),
	@Body		text
) as
begin
	declare @PMessageID int

	insert into yaf_PMessage(FromUserID,Created,Subject,Body)
	values(@FromUserID,getdate(),@Subject,@Body)

	set @PMessageID = @@IDENTITY
	insert into yaf_UserPMessage(UserID,PMessageID,IsRead) values(@ToUserID,@PMessageID,0)
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

-- yaf_pageload

if exists (select * from sysobjects where id = object_id(N'yaf_pageload') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pageload
GO

create procedure yaf_pageload(
	@SessionID	varchar(24),
	@BoardID	int,
	@User		varchar(50),
	@IP			varchar(15),
	@Location	varchar(50),
	@Browser	varchar(50),
	@Platform	varchar(50),
	@CategoryID	int = null,
	@ForumID	int = null,
	@TopicID	int = null,
	@MessageID	int = null
) as
begin
	declare @UserID			int
	declare @UserBoardID	int
	declare @IsGuest		tinyint
	
	if @User is null or @User='' 
	begin
		select @UserID = a.UserID from yaf_User a,yaf_UserGroup b,yaf_Group c where a.UserID=b.UserID and a.BoardID=@BoardID and b.GroupID=c.GroupID and c.IsGuest=1
		set @IsGuest = 1
		set @UserBoardID = @BoardID
	end else
	begin
		select @UserID = UserID, @UserBoardID = BoardID from yaf_User where BoardID=@BoardID and [Name]=@User
		set @IsGuest = 0
	end
	-- Check valid ForumID
	if @ForumID is not null and not exists(select 1 from yaf_Forum where ForumID=@ForumID) begin
		set @ForumID = null
	end
	-- Check valid CategoryID
	if @CategoryID is not null and not exists(select 1 from yaf_Category where CategoryID=@CategoryID) begin
		set @CategoryID = null
	end
	-- Check valid MessageID
	if @MessageID is not null and not exists(select 1 from yaf_Message where MessageID=@MessageID) begin
		set @MessageID = null
	end
	-- Check valid TopicID
	if @TopicID is not null and not exists(select 1 from yaf_Topic where TopicID=@TopicID) begin
		set @TopicID = null
	end

	-- update last visit
	update yaf_User set 
		LastVisit = getdate(),
		IP = @IP
	where UserID = @UserID
	-- find missing ForumID/TopicID
	if @MessageID is not null begin
		select
			@CategoryID = c.CategoryID,
			@ForumID = b.ForumID,
			@TopicID = b.TopicID
		from
			yaf_Message a,
			yaf_Topic b,
			yaf_Forum c,
			yaf_Category d
		where
			a.MessageID = @MessageID and
			b.TopicID = a.TopicID and
			c.ForumID = b.ForumID and
			d.CategoryID = c.CategoryID and
			d.BoardID = @BoardID
	end
	else if @TopicID is not null begin
		select 
			@CategoryID = b.CategoryID,
			@ForumID = a.ForumID 
		from 
			yaf_Topic a,
			yaf_Forum b,
			yaf_Category c
		where 
			a.TopicID = @TopicID and
			b.ForumID = a.ForumID and
			c.CategoryID = b.CategoryID and
			c.BoardID = @BoardID
	end
	else if @ForumID is not null begin
		select
			@CategoryID = a.CategoryID
		from
			yaf_Forum a,
			yaf_Category b
		where
			a.ForumID = @ForumID and
			b.CategoryID = a.CategoryID and
			b.BoardID = @BoardID
	end
	-- update active
	if @UserID is not null and @UserBoardID=@BoardID begin
		if exists(select 1 from yaf_Active where SessionID=@SessionID and BoardID=@BoardID)
		begin
			update yaf_Active set
				UserID = @UserID,
				IP = @IP,
				LastActive = getdate(),
				Location = @Location,
				ForumID = @ForumID,
				TopicID = @TopicID,
				Browser = @Browser,
				Platform = @Platform
			where SessionID = @SessionID
		end
		else begin
			insert into yaf_Active(SessionID,BoardID,UserID,IP,Login,LastActive,Location,ForumID,TopicID,Browser,Platform)
			values(@SessionID,@BoardID,@UserID,@IP,getdate(),getdate(),@Location,@ForumID,@TopicID,@Browser,@Platform)
		end
		-- remove duplicate users
		if @IsGuest=0
			delete from yaf_Active where UserID=@UserID and BoardID=@BoardID and SessionID<>@SessionID
	end
	-- return information
	select
		a.UserID,
		a.IsHostAdmin,
		UserName			= a.Name,
		Suspended			= a.Suspended,
		ThemeFile			= a.ThemeFile,
		LanguageFile		= a.LanguageFile,
		TimeZoneUser		= a.TimeZone,
		x.*,
		CategoryID			= @CategoryID,
		CategoryName		= (select Name from yaf_Category where CategoryID = @CategoryID),
		ForumID				= @ForumID,
		ForumName			= (select Name from yaf_Forum where ForumID = @ForumID),
		TopicID				= @TopicID,
		TopicName			= (select Topic from yaf_Topic where TopicID = @TopicID),
		MailsPending		= (select count(1) from yaf_Mail),
		Incoming			= (select count(1) from yaf_UserPMessage where UserID=a.UserID and IsRead=0)
	from
		yaf_User a,
		yaf_vaccess x
	where
		a.UserID = @UserID and
		x.UserID = a.UserID and
		x.ForumID = IsNull(@ForumID,0)
end
GO

-- yaf_user_delete

if exists (select * from sysobjects where id = object_id(N'yaf_user_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_delete
GO

create procedure yaf_user_delete(@UserID int) as
begin
	declare @GuestUserID int
	declare @UserName varchar(50)

	select @UserName = Name from yaf_User where UserID=@UserID

	select top 1
		@GuestUserID = a.UserID
	from
		yaf_User a,
		yaf_UserGroup b,
		yaf_Group c
	where
		b.UserID = a.UserID and
		b.GroupID = c.GroupID and
		c.IsGuest<>0

	update yaf_Message set UserName=@UserName,UserID=@GuestUserID where UserID=@UserID
	update yaf_Topic set UserName=@UserName,UserID=@GuestUserID where UserID=@UserID
	update yaf_Topic set LastUserName=@UserName,LastUserID=@GuestUserID where LastUserID=@UserID
	update yaf_Forum set LastUserName=@UserName,LastUserID=@GuestUserID where LastUserID=@UserID

	delete from yaf_PMessage where FromUserID=@UserID
	delete from yaf_UserPMessage where UserID=@UserID
	delete from yaf_CheckEmail where UserID = @UserID
	delete from yaf_WatchTopic where UserID = @UserID
	delete from yaf_WatchForum where UserID = @UserID
	delete from yaf_UserGroup where UserID = @UserID
	delete from yaf_User where UserID = @UserID
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
