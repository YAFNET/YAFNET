/* Version x.x.x */

if not exists(select * from syscolumns where id=object_id('yaf_User') and name='AvatarImage')
	alter table yaf_User add AvatarImage image null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='AvatarWidth')
	alter table yaf_System add AvatarWidth int not null default(50)
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='AvatarHeight')
	alter table yaf_System add AvatarHeight int not null default(80)
GO

if exists (select * from sysobjects where id = object_id(N'yaf_pageload') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pageload
GO

create procedure yaf_pageload(
	@SessionID	varchar(24),
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
	declare @UserID int
	if @User is null or @User='' 
		select @UserID = a.UserID from yaf_User a,yaf_Group b where a.GroupID=b.GroupID and b.IsGuest=1
	else
		select @UserID = UserID from yaf_User where Name = @User
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
		SmtpUserName		= s.SmtpUserName,
		SmtpUserPass		= s.SmtpUserPass,
		ForumEmail			= s.ForumEmail,
		EmailVerification	= s.EmailVerification,
		BlankLinks			= s.BlankLinks,
		ShowMoved			= s.ShowMoved,
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

if exists (select * from sysobjects where id = object_id(N'yaf_mail_createwatch') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_mail_createwatch
GO

create procedure yaf_mail_createwatch(@TopicID int,@From varchar(50),@Subject varchar(100),@Body text,@UserID int) as
begin
	insert into yaf_Mail(FromUser,ToUser,Created,Subject,Body)
	select
		@From,
		b.Email,
		getdate(),
		@Subject,
		@Body
	from
		yaf_WatchTopic a,
		yaf_User b
	where
		b.UserID <> @UserID and
		b.UserID = a.UserID and
		a.TopicID = @TopicID
	
	insert into yaf_Mail(FromUser,ToUser,Created,Subject,Body)
	select
		@From,
		b.Email,
		getdate(),
		@Subject,
		@Body
	from
		yaf_WatchForum a,
		yaf_User b,
		yaf_Topic c
	where
		b.UserID <> @UserID and
		b.UserID = a.UserID and
		c.TopicID = @TopicID and
		c.ForumID = a.ForumID
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
		HasAttachments	= (select count(1) from yaf_Attachment x where x.MessageID=a.MessageID),
		HasAvatarImage = (select count(1) from yaf_User x where x.UserID=b.UserID and AvatarImage is not null)
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

if exists (select * from sysobjects where id = object_id(N'yaf_system_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_system_save
GO

create procedure yaf_system_save(
	@Name				varchar(50),
	@TimeZone			int,
	@SmtpServer			varchar(50),
	@SmtpUserName		varchar(50)=null,
	@SmtpUserPass		varchar(50)=null,
	@ForumEmail			varchar(50),
	@EmailVerification	bit,
	@ShowMoved			bit,
	@BlankLinks			bit,
	@AvatarWidth		int,
	@AvatarHeight		int
) as
begin
	update yaf_System set
		Name = @Name,
		TimeZone = @TimeZone,
		SmtpServer = @SmtpServer,
		SmtpUserName = @SmtpUserName,
		SmtpUserPass = @SmtpUserPass,
		ForumEmail = @ForumEmail,
		EmailVerification = @EmailVerification,
		ShowMoved = @ShowMoved,
		BlankLinks = @BlankLinks,
		AvatarWidth = @AvatarWidth,
		AvatarHeight = @AvatarHeight
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_avatarimage') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_avatarimage
GO

create procedure yaf_user_avatarimage(@UserID int) as begin
	select UserID,AvatarImage from yaf_User where UserID=@UserID
end
GO

if exists(select * from sysobjects where name='FK_User_Avatar' and parent_obj=object_id('yaf_User') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table yaf_User drop constraint FK_User_Avatar
GO

if exists(select * from syscolumns where id=object_id('yaf_User') and name='AvatarID')
	alter table yaf_User drop column AvatarID
GO

if exists (select * from sysobjects where id = object_id(N'yaf_Avatar') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table yaf_Avatar
GO
