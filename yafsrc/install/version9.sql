/* Version 0.9.2 */

if not exists(select * from syscolumns where id=object_id('yaf_User') and name='Suspended')
	alter table yaf_User add Suspended datetime null
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_suspend') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_suspend
GO

create procedure yaf_user_suspend(@UserID int,@Suspend datetime=null) as
begin
	update yaf_User set Suspended = @Suspend where UserID=@UserID
end
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
		select @UserID = a.UserID from yaf_User a,yaf_UserGroup b,yaf_Group c where a.UserID=b.UserID and b.GroupID=c.GroupID and c.IsGuest=1
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
		if exists(select 1 from yaf_Active where SessionID = @SessionID)
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
			insert into yaf_Active(SessionID,UserID,IP,Login,LastActive,Location,ForumID,TopicID,Browser,Platform)
			values(@SessionID,@UserID,@IP,getdate(),getdate(),@Location,@ForumID,@TopicID,@Browser,@Platform)
		end
		-- remove duplicate users
		delete from yaf_Active where UserID=@UserID and SessionID<>@SessionID
	end
	-- return information
	select
		a.UserID,
		UserName			= a.Name,
		Suspended			= a.Suspended,
		IsAdmin				= (select count(1) from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and x.GroupID=y.GroupID and y.IsAdmin<>0),
		IsGuest				= (select count(1) from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and x.GroupID=y.GroupID and y.IsGuest<>0),
		IsForumModerator	= (select count(1) from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and x.GroupID=y.GroupID and y.IsModerator<>0),
		IsModerator			= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ModeratorAccess<>0),
		ReadAccess			= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.ReadAccess<>0),
		PostAccess			= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.PostAccess<>0),
		ReplyAccess			= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.ReplyAccess<>0),
		PriorityAccess		= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.PriorityAccess<>0),
		PollAccess			= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.PollAccess<>0),
		VoteAccess			= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.VoteAccess<>0),
		ModeratorAccess		= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.ModeratorAccess<>0),
		EditAccess			= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.EditAccess<>0),
		DeleteAccess		= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.DeleteAccess<>0),
		UploadAccess		= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.UploadAccess<>0),
		CategoryID			= @CategoryID,
		CategoryName		= (select Name from yaf_Category where CategoryID = @CategoryID),
		ForumID				= @ForumID,
		ForumName			= (select Name from yaf_Forum where ForumID = @ForumID),
		TopicID				= @TopicID,
		TopicName			= (select Topic from Yaf_Topic where TopicID = @TopicID),
		TimeZoneUser		= a.TimeZone,
		TimeZoneForum		= s.TimeZone,
		BBName				= s.Name,
		SmtpServer			= s.SmtpServer,
		SmtpUserName		= s.SmtpUserName,
		SmtpUserPass		= s.SmtpUserPass,
		ForumEmail			= s.ForumEmail,
		EmailVerification	= s.EmailVerification,
		BlankLinks			= s.BlankLinks,
		ShowMoved			= s.ShowMoved,
		ShowGroups			= s.ShowGroups,
		MailsPending		= (select count(1) from yaf_Mail),
		Incoming			= (select count(1) from yaf_PMessage where ToUserID=a.UserID and IsRead=0)
	from
		yaf_User a,
		yaf_System s
	where
		a.UserID = @UserID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_find') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_find
GO

create procedure yaf_user_find(@Filter bit,@UserName varchar(50)=null,@Email varchar(50)=null) as
begin
	if @Filter<>0
	begin
		if @UserName is not null
			set @UserName = '%' + @UserName + '%'

		select 
			a.*,
			IsGuest = (select count(1) from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and x.GroupID=y.GroupID and y.IsGuest<>0)
		from 
			yaf_User a
		where 
			(@UserName is not null and a.Name like @UserName) or (@Email is not null and Email like @Email)
		order by
			a.Name
	end else
	begin
		select 
			a.UserID,
			IsGuest = (select count(1) from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and x.GroupID=y.GroupID and y.IsGuest<>0)
		from 
			yaf_User a
		where 
			(@UserName is not null and a.Name=@UserName) or (@Email is not null and Email=@Email)
	end
end
GO

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
	insert into yaf_PMessage(FromUserID,ToUserID,Created,Subject,Body,IsRead)
	values(@FromUserID,@ToUserID,getdate(),@Subject,@Body,0)
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_pmessage_info') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pmessage_info
GO

create procedure yaf_pmessage_info as
begin
	select
		NumRead	= (select count(1) from yaf_PMessage where IsRead<>0),
		NumUnread = (select count(1) from yaf_PMessage where IsRead=0),
		NumTotal = (select count(1) from yaf_PMessage)
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_pmessage_prune') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pmessage_prune
GO

create procedure yaf_pmessage_prune(@DaysRead int,@DaysUnread int) as
begin
	delete from yaf_PMessage
	where IsRead<>0
	and datediff(dd,Created,getdate())>@DaysRead

	delete from yaf_PMessage
	where IsRead=0
	and datediff(dd,Created,getdate())>@DaysUnread
end
GO
