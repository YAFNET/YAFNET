/* 0.8.0 */

if exists(select * from syscolumns where id=object_id('yaf_System') and name='Culture')
	alter table yaf_System drop column Culture
GO

if exists(select * from syscolumns where id=object_id('yaf_User') and name='Culture')
	alter table yaf_User drop column Culture
GO

if exists (select * from sysobjects where id = object_id(N'yaf_topic_move') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_move
GO

create procedure yaf_topic_move(@TopicID int,@ForumID int,@ShowMoved bit) as
begin
	if @ShowMoved=1 begin
		-- create a moved message
		insert into yaf_Topic(ForumID,UserID,UserName,Posted,Topic,Views,IsLocked,Priority,PollID,TopicMovedID)
		select ForumID,UserID,UserName,Posted,Topic,0,IsLocked,Priority,PollID,@TopicID
		from yaf_Topic where TopicID = @TopicID
	end

	-- move the topic
	update yaf_Topic set ForumID = @ForumID where TopicID = @TopicID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_save
GO

create procedure yaf_user_save(
	@UserID		int,
	@UserName	varchar(50) = null,
	@Password	varchar(32) = null,
	@Email		varchar(50) = null,
	@Hash		varchar(32) = null,
	@Location	varchar(50),
	@HomePage	varchar(50),
	@TimeZone	int,
	@Avatar		varchar(100) = null,
	@Approved	bit = null
) as
begin
	declare @GroupID int

	if @Location is not null and @Location = '' set @Location = null
	if @HomePage is not null and @HomePage = '' set @HomePage = null
	if @Avatar is not null and @Avatar = '' set @Avatar = null

	if @UserID is null or @UserID<1 begin
		select @GroupID = max(GroupID) from yaf_Group where IsStart=1
		
		if @Email = '' set @Email = null
		
		insert into yaf_User(GroupID,Name,Password,Email,Joined,LastVisit,NumPosts,Approved,Location,HomePage,TimeZone,Avatar) 
		values(@GroupID,@UserName,@Password,@Email,getdate(),getdate(),0,@Approved,@Location,@HomePage,@TimeZone,@Avatar)
	
		set @UserID = @@IDENTITY
		
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
			Avatar = @Avatar
		where UserID = @UserID
		
		if @Email is not null
			update yaf_User set Email = @Email where UserID = @UserID
	end
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_system_initialize') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_system_initialize
GO

create procedure yaf_system_initialize(
	@Name		varchar(50),
	@TimeZone	int,
	@ForumEmail	varchar(50),
	@SmtpServer	varchar(50),
	@User		varchar(50),
	@UserEmail	varchar(50),
	@Password	varchar(32)
) as 
begin
	declare @GroupID int
	insert into yaf_System(SystemID,Version,VersionName,Name,TimeZone,SmtpServer,ForumEmail)
	values(1,1,'0.7.0',@Name,@TimeZone,@SmtpServer,@ForumEmail)
	insert into yaf_Group(Name,IsAdmin,IsGuest,IsStart,IsLadder)
	values('Administration',1,0,0,0)
	set @GroupID = @@IDENTITY
	insert into yaf_User(GroupID,Name,Password,Joined,LastVisit,NumPosts,TimeZone,Approved,Email)
	values(@GroupID,@User,@Password,getdate(),getdate(),0,@TimeZone,1,@UserEmail)
	insert into yaf_Group(Name,IsAdmin,IsGuest,IsStart,IsLadder)
	values('Guest',0,1,0,0)
	set @GroupID = @@IDENTITY
	insert into yaf_User(GroupID,Name,Password,Joined,LastVisit,NumPosts,TimeZone,Approved,Email)
	values(@GroupID,'Guest','na',getdate(),getdate(),0,@TimeZone,1,@ForumEmail)
	insert into yaf_Group(Name,IsAdmin,IsGuest,IsStart,IsLadder,MinPosts)
	values('Newbie',0,0,1,1,0)
end
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='EmailVerification')
	alter table yaf_System add EmailVerification bit not null default(1)
GO

if exists (select * from sysobjects where id = object_id(N'yaf_system_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_system_save
GO

create procedure yaf_system_save(
	@Name				varchar(50),
	@TimeZone			int,
	@SmtpServer			varchar(50),
	@ForumEmail			varchar(50),
	@EmailVerification	bit
) as
begin
	update yaf_System set
		Name = @Name,
		TimeZone = @TimeZone,
		SmtpServer = @SmtpServer,
		ForumEmail = @ForumEmail,
		EmailVerification = @EmailVerification
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_extvalidate') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_extvalidate
GO

create procedure yaf_user_extvalidate(@Name varchar(50),@Email varchar(50)) as
begin
	declare @UserID int
	declare @GroupID int
	declare @TimeZone int
	select @UserID = UserID from yaf_User where Name = @Name
	if @UserID is null or @UserID<1 begin
		select @GroupID = max(GroupID) from yaf_Group where IsStart=1
		select @TimeZone = TimeZone from yaf_System
		if @Email = '' set @Email = null
		insert into yaf_User(GroupID,Name,Password,Email,Joined,LastVisit,NumPosts,Approved,Location,HomePage,TimeZone,Avatar) 
		values(@GroupID,@Name,'<external>',@Email,getdate(),getdate(),0,0,null,null,@TimeZone,null)
		set @UserID = @@IDENTITY
	end
	select UserID = @UserID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_pageload') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pageload
GO

create procedure yaf_pageload(
	@SessionID	varchar(24),
	@User		varchar(50),
	@IP		varchar(15),
	@Location	varchar(50),
	@Browser	varchar(50),
	@Platform	varchar(50),
	@CategoryID	int = null,
	@ForumID	int = null,
	@TopicID	int = null,
	@MessageID	int = null
) as
begin
	declare @UserID int
	if @User is null or @User='' 
		select @UserID = a.UserID from yaf_User a,yaf_Group b where a.GroupID=b.GroupID and b.IsGuest=1
	else
		select @UserID = UserID from yaf_User where Name = @User
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
			yaf_Forum c
		where
			a.MessageID = @MessageID and
			b.TopicID = a.TopicID and
			c.ForumID = b.ForumID
	end
	else if @TopicID is not null begin
		select 
			@CategoryID = b.CategoryID,
			@ForumID = a.ForumID 
		from 
			yaf_Topic a,
			yaf_Forum b
		where 
			a.TopicID = @TopicID and
			b.ForumID = a.ForumID
	end
	else if @ForumID is not null begin
		select
			@CategoryID = a.CategoryID
		from
			yaf_Forum a
		where
			a.ForumID = @ForumID
	end
	-- update active
	if @UserID is not null begin
		declare @count int
		select @count = count(1) from yaf_Active where SessionID = @SessionID
		if @count>0 begin
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
			insert into yaf_Active(SessionID,UserID,IP,Login,LastActive,Location,ForumID,TopicID,Browser,Platform)
			values(@SessionID,@UserID,@IP,getdate(),getdate(),@Location,@ForumID,@TopicID,@Browser,@Platform)
		end
	end
	-- return information
	select
		a.UserID,
		UserName			= a.Name,
		b.IsAdmin,
		b.IsGuest,
		ReadAccess			= (select ReadAccess from yaf_ForumAccess x where x.GroupID=b.GroupID and x.ForumID=@ForumID),
		PostAccess			= (select PostAccess from yaf_ForumAccess x where x.GroupID=b.GroupID and x.ForumID=@ForumID),
		ReplyAccess			= (select ReplyAccess from yaf_ForumAccess x where x.GroupID=b.GroupID and x.ForumID=@ForumID),
		PriorityAccess		= (select PriorityAccess from yaf_ForumAccess x where x.GroupID=b.GroupID and x.ForumID=@ForumID),
		PollAccess			= (select PollAccess from yaf_ForumAccess x where x.GroupID=b.GroupID and x.ForumID=@ForumID),
		VoteAccess			= (select VoteAccess from yaf_ForumAccess x where x.GroupID=b.GroupID and x.ForumID=@ForumID),
		ModeratorAccess		= (select ModeratorAccess from yaf_ForumAccess x where x.GroupID=b.GroupID and x.ForumID=@ForumID),
		EditAccess			= (select EditAccess from yaf_ForumAccess x where x.GroupID=b.GroupID and x.ForumID=@ForumID),
		DeleteAccess		= (select DeleteAccess from yaf_ForumAccess x where x.GroupID=b.GroupID and x.ForumID=@ForumID),
		CategoryID			= @CategoryID,
		CategoryName		= (select Name from yaf_Category where CategoryID = @CategoryID),
		ForumID				= @ForumID,
		ForumName			= (select Name from yaf_Forum where ForumID = @ForumID),
		TopicID				= @TopicID,
		TopicName			= (select Topic from Yaf_Topic where TopicID = @TopicID),
		TimeZone			= a.TimeZone,
		BBName				= s.Name,
		SmtpServer			= s.SmtpServer,
		ForumEmail			= s.ForumEmail,
		EmailVerification	= s.EmailVerification,
		MailsPending		= (select count(1) from yaf_Mail)
	from
		yaf_User a,
		yaf_Group b,
		yaf_System s
	where
		a.UserID = @UserID and
		b.GroupID = a.GroupID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_topic_prune') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_prune
GO

create procedure yaf_topic_prune(@ForumID int=null,@Days int) as
begin
	declare @c cursor
	declare @TopicID int
	declare @Count int
	set @Count = 0
	if @ForumID = 0 set @ForumID = null
	if @ForumID is not null begin
		set @c = cursor for
		select 
			TopicID
		from 
			yaf_Topic
		where 
			ForumID = @ForumID and
			Priority = 0 and
			datediff(dd,lastposted,getdate())>@Days
	end
	else begin
		set @c = cursor for
		select 
			TopicID
		from 
			yaf_Topic
		where 
			Priority = 0 and
			datediff(dd,lastposted,getdate())>@Days
	end
	open @c
	fetch @c into @TopicID
	while @@FETCH_STATUS=0 begin
		exec yaf_topic_delete @TopicID
		set @Count = @Count + 1
		fetch @c into @TopicID
	end
	close @c
	deallocate @c
	select Count = @Count
end
GO
