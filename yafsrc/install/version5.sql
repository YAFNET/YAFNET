if not exists(select * from syscolumns where id=object_id('yaf_ForumAccess') and name='UploadAccess')
	alter table yaf_ForumAccess add UploadAccess bit not null default(0)
GO

if exists (select * from sysobjects where id = object_id(N'yaf_forumaccess_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forumaccess_save
GO

create procedure yaf_forumaccess_save(
	@ForumID			int,
	@GroupID			int,
	@ReadAccess			bit,
	@PostAccess			bit,
	@ReplyAccess		bit,
	@PriorityAccess		bit,
	@PollAccess			bit,
	@VoteAccess			bit,
	@ModeratorAccess	bit,
	@EditAccess			bit,
	@DeleteAccess		bit,
	@UploadAccess		bit
) as
begin
	update yaf_ForumAccess set 
		ReadAccess		= @ReadAccess,
		PostAccess		= @PostAccess,
		ReplyAccess		= @ReplyAccess,
		PriorityAccess	= @PriorityAccess,
		PollAccess		= @PollAccess,
		VoteAccess		= @VoteAccess,
		ModeratorAccess	= @ModeratorAccess,
		EditAccess		= @EditAccess,
		DeleteAccess	= @DeleteAccess,
		UploadAccess	= @UploadAccess
	where 
		ForumID = @ForumID and 
		GroupID = @GroupID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_forumaccess_repair') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forumaccess_repair
GO

create procedure yaf_forumaccess_repair as
begin
	insert into yaf_ForumAccess(
		GroupID,
		ForumID,
		ReadAccess,
		PostAccess,
		ReplyAccess,
		PriorityAccess,
		PollAccess,
		VoteAccess,
		ModeratorAccess,
		EditAccess,
		DeleteAccess,
		UploadAccess
	)
	select
		b.GroupID,
		a.ForumID,
		0,0,0,0,0,0,0,0,0,0
	from
		yaf_Forum a,
		yaf_Group b
	where
		not exists(select 1 from yaf_ForumAccess x where x.ForumID=a.ForumID and x.GroupID=b.GroupID)
	order by
		a.ForumID,
		b.GroupID
end
GO

if not exists (select * from sysobjects where id = object_id(N'yaf_Attachment') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
create table [yaf_Attachment](
	[AttachmentID]	[int] identity not null,
	[MessageID]	[int] not null,
	[FileName]	[varchar](50) not null,
	[Bytes]		[int] not null
)
GO

if not exists(select * from sysobjects where name='FK_Active_Forum' and parent_obj=object_id('yaf_Active') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_Attachment] ADD 
	CONSTRAINT [FK_Attachment_Message] FOREIGN KEY 
	(
		[MessageID]
	) REFERENCES [yaf_Message] (
		[MessageID]
	)
GO

if exists (select * from sysobjects where id = object_id(N'yaf_attachment_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_attachment_save
GO

create procedure yaf_attachment_save(@MessageID int,@FileName varchar(50),@Bytes int) as begin
	insert into yaf_Attachment(MessageID,FileName,Bytes) values(@MessageID,@FileName,@Bytes)
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_post_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_post_list
GO

create procedure yaf_post_list(@TopicID int,@UserID int,@UpdateViewCount smallint=1) as
begin
	set nocount on

	if @UpdateViewCount>0
		update yaf_Topic set Views = Views + 1 where TopicID = @TopicID

	select
		d.TopicID,
		a.MessageID,
		a.Posted,
		Subject = d.Topic,
		a.Message,
		a.UserID,
		UserName	= IsNull(a.UserName,b.Name),
		b.Joined,
		Posts		= b.NumPosts,
		GroupName	= c.Name,
		d.Views,
		d.ForumID,
		Avatar = b.Avatar,
		b.Location,
		b.HomePage,
		b.Signature,
		c.RankImage,
		HasAttachments	= (select count(1) from yaf_Attachment x where x.MessageID=a.MessageID)
	from
		yaf_Message a, 
		yaf_User b,
		yaf_Group c,
		yaf_Topic d
	where
		a.TopicID = @TopicID and
		b.UserID = a.UserID and
		c.GroupID = b.GroupID and
		d.TopicID = a.TopicID
	order by
		a.Posted asc
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_attachment_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_attachment_list
GO

create procedure yaf_attachment_list(@MessageID int) as begin
	select FileName,Bytes from yaf_Attachment where MessageID=@MessageID
end
go

if exists (select * from sysobjects where id = object_id(N'yaf_message_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_save
GO

CREATE  procedure yaf_message_save(
	@TopicID	int,
	@User		varchar(50),
	@Message	text,
	@UserName	varchar(50),
	@IP			varchar(15),
	@MessageID	int output
) as
begin
	declare @UserID int
	declare @ForumID int

	if @User is null or @User='' 
		select @UserID = a.UserID from yaf_User a,yaf_Group b where a.GroupID=b.GroupID and b.IsGuest=1
	else begin
		select @UserID = UserID from yaf_User where Name = @User
		set @UserName = null
	end
	insert into yaf_Message(UserID,Message,TopicID,Posted,UserName,IP)
	values(@UserID,@Message,@TopicID,getdate(),@UserName,@IP)
	set @MessageID = @@IDENTITY
	update yaf_User set NumPosts = NumPosts + 1 where UserID = @UserID
	exec yaf_user_upgrade @UserID
	-- update yaf_Forum
	select @ForumID = ForumID from yaf_Topic where TopicID = @TopicID
	update yaf_Forum set
		LastPosted = getdate(),
		LastTopicID = @TopicID,
		LastMessageID = @MessageID,
		LastUserID = @UserID,
		LastUserName = @UserName
	where ForumID = @ForumID
	-- update yaf_Topic
	update yaf_Topic set
		LastPosted = getdate(),
		LastMessageID = @MessageID,
		LastUserID = @UserID,
		LastUserName = @UserName
	where TopicID = @TopicID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_topic_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_save
GO

create procedure yaf_topic_save(
	@ForumID	int,
	@Subject	varchar(100),
	@User		varchar(50),
	@Message	text,
	@Priority	smallint,
	@UserName	varchar(50),
	@IP		varchar(15),
	@PollID		int=null
) as
begin
	declare @UserID int
	declare @TopicID int
	declare @MessageID int

	if @User is null or @User='' 
		select @UserID = a.UserID from yaf_User a,yaf_Group b where a.GroupID=b.GroupID and b.IsGuest=1
	else begin
		select @UserID = UserID from yaf_User where Name = @User
		set @UserName = null
	end
	insert into yaf_Topic(ForumID,Topic,UserID,Posted,Views,Priority,IsLocked,PollID,UserName)
	values(@ForumID,@Subject,@UserID,getdate(),0,@Priority,0,@PollID,@UserName)
	set @TopicID = @@IDENTITY
	exec yaf_message_save @TopicID,@User,@Message,@UserName,@IP,@MessageID output

	select TopicID = @TopicID, MessageID = @MessageID
end
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='BlankLinks')
	alter table yaf_System add BlankLinks bit not null default(0)
GO

if exists (select * from sysobjects where id = object_id(N'yaf_system_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_system_save
GO

create procedure yaf_system_save(
	@Name				varchar(50),
	@TimeZone			int,
	@SmtpServer			varchar(50),
	@ForumEmail			varchar(50),
	@EmailVerification	bit,
	@ShowMoved			bit,
	@BlankLinks			bit
) as
begin
	update yaf_System set
		Name = @Name,
		TimeZone = @TimeZone,
		SmtpServer = @SmtpServer,
		ForumEmail = @ForumEmail,
		EmailVerification = @EmailVerification,
		ShowMoved = @ShowMoved,
		BlankLinks = @BlankLinks
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
		UploadAccess		= (select UploadAccess from yaf_ForumAccess x where x.GroupID=b.GroupID and x.ForumID=@ForumID),
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
		BlankLinks			= s.BlankLinks,
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
