if exists (select * from sysobjects where id = object_id(N'yaf_extension_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_extension_list
GO

if exists (select * from sysobjects where id = object_id(N'yaf_Extension') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table yaf_Extension
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_save
GO

create procedure yaf_user_save(
	@UserID			int,
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

	if @UserID is null or @UserID<1 begin
		if @Email = '' set @Email = null
		
		select @RankID = RankID from yaf_Rank where IsStart<>0
		
		insert into yaf_User(RankID,Name,Password,Email,Joined,LastVisit,NumPosts,Approved,Location,HomePage,TimeZone,Avatar,Gender) 
		values(@RankID,@UserName,@Password,@Email,getdate(),getdate(),0,@Approved,@Location,@HomePage,@TimeZone,@Avatar,@Gender)
	
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

if exists (select * from sysobjects where id = object_id(N'yaf_post_last10user') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_post_last10user
GO

create procedure yaf_post_last10user(@UserID int,@PageUserID int) as
begin
	set nocount on

	select top 10
		a.Posted,
		Subject = c.Topic,
		a.Message,
		a.UserID,
		UserName = IsNull(a.UserName,b.Name),
		b.Signature,
		c.TopicID
	from
		yaf_Message a, 
		yaf_User b,
		yaf_Topic c,
		yaf_Forum d,
		yaf_vaccess x
	where
		a.Approved <> 0 and
		a.UserID = @UserID and
		b.UserID = a.UserID and
		c.TopicID = a.TopicID and
		d.ForumID = c.ForumID and
		x.ForumID = c.ForumID and
		x.UserID = @PageUserID and
		x.ReadAccess <> 0
	order by
		a.Posted desc
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_topic_active') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_active
GO

create procedure yaf_topic_active(@UserID int,@Since datetime) as
begin
	select
		c.ForumID,
		c.TopicID,
		c.Posted,
		LinkTopicID = IsNull(c.TopicMovedID,c.TopicID),
		Subject = c.Topic,
		c.UserID,
		Starter = IsNull(c.UserName,b.Name),
		Replies = (select count(1) from yaf_Message x where x.TopicID=c.TopicID) - 1,
		Views = c.Views,
		LastPosted = c.LastPosted,
		LastUserID = c.LastUserID,
		LastUserName = IsNull(c.LastUserName,(select Name from yaf_User x where x.UserID=c.LastUserID)),
		LastMessageID = c.LastMessageID,
		LastTopicID = c.TopicID,
		c.IsLocked,
		c.Priority,
		c.PollID,
		ForumName = d.Name,
		c.TopicMovedID,
		ForumLocked = d.Locked
	from
		yaf_Topic c,
		yaf_User b,
		yaf_Forum d,
		yaf_vaccess x
	where
		b.UserID = c.UserID and
		@Since < c.LastPosted and
		d.ForumID = c.ForumID and
		x.UserID = @UserID and
		x.ForumID = d.ForumID and
		x.ReadAccess <> 0
	order by
		d.Name asc,
		Priority desc,
		LastPosted desc
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_smiley_listunique') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_smiley_listunique
GO

create procedure yaf_smiley_listunique as
begin
	select 
		Icon, 
		Emoticon,
		Code = (select top 1 Code from yaf_Smiley x where x.Icon=yaf_Smiley.Icon)
	from 
		yaf_Smiley
	group by
		Icon,
		Emoticon
	order by
		Code
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
	declare @UserID		int
	declare @IsGuest	tinyint
	
	if @User is null or @User='' 
	begin
		select @UserID = a.UserID from yaf_User a,yaf_UserGroup b,yaf_Group c where a.UserID=b.UserID and b.GroupID=c.GroupID and c.IsGuest=1
		set @IsGuest = 1
	end else
	begin
		select @UserID = UserID from yaf_User where Name = @User
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
		if @IsGuest=0
			delete from yaf_Active where UserID=@UserID and SessionID<>@SessionID
	end
	-- return information
	select
		a.UserID,
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
		Incoming			= (select count(1) from yaf_PMessage where ToUserID=a.UserID and IsRead=0)
	from
		yaf_User a,
		yaf_vaccess x
	where
		a.UserID = @UserID and
		x.UserID = a.UserID and
		x.ForumID = IsNull(@ForumID,0)
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_extvalidate') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_extvalidate
GO
