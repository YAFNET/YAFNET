/* Version x.x.x */

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
