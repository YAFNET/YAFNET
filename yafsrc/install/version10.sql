/* Version 0.9.4 */

if not exists (select * from sysobjects where id = object_id(N'yaf_AccessMask') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE [yaf_AccessMask](
	[AccessMaskID]		[int] IDENTITY NOT NULL ,
	[Name]				[varchar](50) NOT NULL ,
	[ReadAccess] 		[bit] NOT NULL ,
	[PostAccess] 		[bit] NOT NULL ,
	[ReplyAccess] 		[bit] NOT NULL ,
	[PriorityAccess] 	[bit] NOT NULL ,
	[PollAccess] 		[bit] NOT NULL ,
	[VoteAccess] 		[bit] NOT NULL ,
	[ModeratorAccess] 	[bit] NOT NULL ,		-- can moderate posts, user management
	[EditAccess] 		[bit] NOT NULL ,
	[DeleteAccess] 		[bit] NOT NULL ,
	[UploadAccess] 		[bit] NOT NULL
)
GO

if not exists(select * from sysindexes where id=object_id('yaf_AccessMask') and name='PK_AccessMask')
ALTER TABLE [yaf_AccessMask] WITH NOCHECK ADD 
	CONSTRAINT [PK_AccessMask] PRIMARY KEY  CLUSTERED 
	(
		[AccessMaskID]
	) 
GO

if not exists(select * from syscolumns where id=object_id('yaf_ForumAccess') and name='ReadAccess')
	alter table yaf_ForumAccess add ReadAccess bit null,PostAccess bit null,ReplyAccess bit null,PriorityAccess bit null,PollAccess bit null,VoteAccess bit null,ModeratorAccess bit null,EditAccess bit null,DeleteAccess bit null,UploadAccess bit null
GO

if not exists(select 1 from yaf_AccessMask)
	insert into yaf_AccessMask([Name],ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess)
	select min('Mask ' + convert(varchar,GroupID) + ' - ' + convert(varchar,ForumID)),ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess
	from yaf_ForumAccess
	group by ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess
GO

if not exists(select * from syscolumns where id=object_id('yaf_ForumAccess') and name='AccessMaskID')
	alter table yaf_ForumAccess add AccessMaskID int null
GO

if not exists(select * from sysobjects where name='FK_ForumAccess_AccessMask' and parent_obj=object_id('yaf_ForumAccess') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_ForumAccess] ADD 
	CONSTRAINT [FK_ForumAccess_AccessMask] FOREIGN KEY 
	(
		[AccessMaskID]
	) REFERENCES [yaf_AccessMask] (
		[AccessMaskID]
	)
GO

if exists(select 1 from yaf_ForumAccess where AccessMaskID is null)
	update yaf_ForumAccess set AccessMaskID=(
		select AccessMaskID from yaf_AccessMask x
		where
			x.ReadAccess		= yaf_ForumAccess.ReadAccess and
			x.PostAccess		= yaf_ForumAccess.PostAccess and
			x.ReplyAccess		= yaf_ForumAccess.ReplyAccess and
			x.PriorityAccess	= yaf_ForumAccess.PriorityAccess and
			x.PollAccess		= yaf_ForumAccess.PollAccess and
			x.VoteAccess		= yaf_ForumAccess.VoteAccess and
			x.ModeratorAccess	= yaf_ForumAccess.ModeratorAccess and
			x.EditAccess		= yaf_ForumAccess.EditAccess and
			x.DeleteAccess		= yaf_ForumAccess.DeleteAccess and
			x.UploadAccess		= yaf_ForumAccess.UploadAccess
	) where AccessMaskID is null
GO

alter table yaf_ForumAccess alter column AccessMaskID int not null
GO

if exists(select * from syscolumns where id=object_id('yaf_ForumAccess') and name='ReadAccess')
	alter table yaf_ForumAccess drop column ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess
GO

if not exists (select * from sysobjects where id = object_id(N'yaf_UserForum') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE [yaf_UserForum](
	[UserID]			[int] NOT NULL ,
	[ForumID]			[int] NOT NULL ,
	[AccessMaskID]		[int] NOT NULL ,
	[Invited]			[datetime] NOT NULL ,
	[Accepted]			[bit] NOT NULL
)
GO

if not exists(select * from sysindexes where id=object_id('yaf_UserForum') and name='PK_UserForum')
ALTER TABLE [yaf_UserForum] WITH NOCHECK ADD 
	CONSTRAINT [PK_UserForum] PRIMARY KEY  CLUSTERED 
	(
		[UserID],[ForumID]
	) 
GO

if not exists(select * from sysobjects where name='FK_UserForum_User' and parent_obj=object_id('yaf_UserForum') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_UserForum] ADD 
	CONSTRAINT [FK_UserForum_User] FOREIGN KEY 
	(
		[UserID]
	) REFERENCES [yaf_User] (
		[UserID]
	)
GO

if not exists(select * from sysobjects where name='FK_UserForum_Forum' and parent_obj=object_id('yaf_UserForum') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_UserForum] ADD 
	CONSTRAINT [FK_UserForum_Forum] FOREIGN KEY 
	(
		[ForumID]
	) REFERENCES [yaf_Forum] (
		[ForumID]
	)
GO

if not exists(select * from sysobjects where name='FK_UserForum_AccessMask' and parent_obj=object_id('yaf_UserForum') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_UserForum] ADD 
	CONSTRAINT [FK_UserForum_AccessMask] FOREIGN KEY 
	(
		[AccessMaskID]
	) REFERENCES [yaf_AccessMask] (
		[AccessMaskID]
	)
GO

if exists (select * from sysobjects where id = object_id(N'yaf_vaccess') and OBJECTPROPERTY(id, N'IsView') = 1)
	drop view yaf_vaccess
GO

CREATE VIEW yaf_vaccess as
	select
		UserID				= a.UserID,
		ForumID				= x.ForumID,
		IsAdmin				= max(convert(tinyint,b.IsAdmin)),
		IsGuest				= max(convert(tinyint,b.IsGuest)),
		IsForumModerator	= max(convert(tinyint,b.IsModerator)),
		IsModerator			= (select count(1) from yaf_UserGroup v,yaf_Group w,yaf_ForumAccess x,yaf_AccessMask y where v.UserID=a.UserID and w.GroupID=v.GroupID and x.GroupID=w.GroupID and y.AccessMaskID=x.AccessMaskID and y.ModeratorAccess<>0),
		ReadAccess			= max(x.ReadAccess),
		PostAccess			= max(x.PostAccess),
		ReplyAccess			= max(x.ReplyAccess),
		PriorityAccess		= max(x.PriorityAccess),
		PollAccess			= max(x.PollAccess),
		VoteAccess			= max(x.VoteAccess),
		ModeratorAccess		= max(x.ModeratorAccess),
		EditAccess			= max(x.EditAccess),
		DeleteAccess		= max(x.DeleteAccess),
		UploadAccess		= max(x.UploadAccess)
	from
		(select
			b.UserID,
			b.ForumID,
			ReadAccess		= convert(tinyint,c.ReadAccess),
			PostAccess		= convert(tinyint,c.PostAccess),
			ReplyAccess		= convert(tinyint,c.ReplyAccess),
			PriorityAccess	= convert(tinyint,c.PriorityAccess),
			PollAccess		= convert(tinyint,c.PollAccess),
			VoteAccess		= convert(tinyint,c.VoteAccess),
			ModeratorAccess	= convert(tinyint,c.ModeratorAccess),
			EditAccess		= convert(tinyint,c.EditAccess),
			DeleteAccess	= convert(tinyint,c.DeleteAccess),
			UploadAccess	= convert(tinyint,c.UploadAccess)
		from
			yaf_UserForum b
			join yaf_AccessMask c on c.AccessMaskID=b.AccessMaskID
		
		union
		
		select
			b.UserID,
			c.ForumID,
			ReadAccess		= convert(tinyint,d.ReadAccess),
			PostAccess		= convert(tinyint,d.PostAccess),
			ReplyAccess		= convert(tinyint,d.ReplyAccess),
			PriorityAccess	= convert(tinyint,d.PriorityAccess),
			PollAccess		= convert(tinyint,d.PollAccess),
			VoteAccess		= convert(tinyint,d.VoteAccess),
			ModeratorAccess	= convert(tinyint,d.ModeratorAccess),
			EditAccess		= convert(tinyint,d.EditAccess),
			DeleteAccess	= convert(tinyint,d.DeleteAccess),
			UploadAccess	= convert(tinyint,d.UploadAccess)
		from
			yaf_UserGroup b
			join yaf_ForumAccess c on c.GroupID=b.GroupID
			join yaf_AccessMask d on d.AccessMaskID=c.AccessMaskID

		union

		select
			a.UserID,
			ForumID			= convert(int,0),
			ReadAccess		= convert(tinyint,0),
			PostAccess		= convert(tinyint,0),
			ReplyAccess		= convert(tinyint,0),
			PriorityAccess	= convert(tinyint,0),
			PollAccess		= convert(tinyint,0),
			VoteAccess		= convert(tinyint,0),
			ModeratorAccess	= convert(tinyint,0),
			EditAccess		= convert(tinyint,0),
			DeleteAccess	= convert(tinyint,0),
			UploadAccess	= convert(tinyint,0)
		from
			yaf_User a
		) as x
		join yaf_UserGroup a on a.UserID=x.UserID
		join yaf_Group b on b.GroupID=a.GroupID
	group by a.UserID,x.ForumID

GO

select * from yaf_vaccess where userid=1

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
		IsAdmin				= x.IsAdmin,
		IsGuest				= x.IsGuest,
		IsForumModerator	= x.IsForumModerator,
		IsModerator			= x.IsModerator,
		ReadAccess			= x.ReadAccess,
		PostAccess			= x.PostAccess,
		ReplyAccess			= x.ReplyAccess,
		PriorityAccess		= x.PriorityAccess,
		PollAccess			= x.PollAccess,
		VoteAccess			= x.VoteAccess,
		ModeratorAccess		= x.ModeratorAccess,
		EditAccess			= x.EditAccess,
		DeleteAccess		= x.DeleteAccess,
		UploadAccess		= x.UploadAccess,
		CategoryID			= @CategoryID,
		CategoryName		= (select Name from yaf_Category where CategoryID = @CategoryID),
		ForumID				= @ForumID,
		ForumName			= (select Name from yaf_Forum where ForumID = @ForumID),
		TopicID				= @TopicID,
		TopicName			= (select Topic from yaf_Topic where TopicID = @TopicID),
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
		AllowRichEdit		= s.AllowRichEdit,
		AllowUserTheme		= s.AllowUserTheme,
		AllowUserLanguage	= s.AllowUserLanguage,
		MailsPending		= (select count(1) from yaf_Mail),
		Incoming			= (select count(1) from yaf_PMessage where ToUserID=a.UserID and IsRead=0)
	from
		yaf_User a,
		yaf_System s,
		yaf_vaccess x
	where
		a.UserID = @UserID and
		x.UserID = a.UserID and
		x.ForumID = IsNull(@ForumID,0)
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_forum_moderators') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_moderators
GO

create procedure yaf_forum_moderators as
begin
	select
		a.ForumID, 
		a.GroupID, 
		GroupName = b.Name
	from
		yaf_ForumAccess a,
		yaf_Group b,
		yaf_AccessMask c
	where
		c.ModeratorAccess <> 0 and
		b.GroupID = a.GroupID and
		c.AccessMaskID = a.AccessMaskID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_category_listread') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_category_listread
GO

if exists (select * from sysobjects where id = object_id(N'yaf_forum_listread') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_listread
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_access') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_access
GO

create procedure yaf_user_access(@UserID int,@ForumID int) as
begin
	select * from yaf_vaccess where UserID=@UserID and ForumID=@ForumID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_accessmask_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_accessmask_list
GO

create procedure yaf_accessmask_list(@AccessMaskID int=null) as
begin
	if @AccessMaskID is null
		select 
			a.* 
		from 
			yaf_AccessMask a 
		order by 
			a.Name
	else
		select 
			a.* 
		from 
			yaf_AccessMask a 
		where
			a.AccessMaskID = @AccessMaskID
		order by 
			a.Name
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_accessmask_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_accessmask_delete
GO

create procedure yaf_accessmask_delete(@AccessMaskID int) as
begin
	declare @flag int
	
	set @flag=1
	if exists(select 1 from yaf_ForumAccess where AccessMaskID=@AccessMaskID) or exists(select 1 from yaf_UserForum where AccessMaskID=@AccessMaskID)
		set @flag=0
	else
		delete from yaf_AccessMask where AccessMaskID=@AccessMaskID
	
	select @flag
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_accessmask_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_accessmask_save
GO

create procedure yaf_accessmask_save(
	@AccessMaskID		int=null,
	@Name				varchar(50),
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
	if @AccessMaskID is null
		insert into yaf_AccessMask([Name],ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess)
		values(@Name,@ReadAccess,@PostAccess,@ReplyAccess,@PriorityAccess,@PollAccess,@VoteAccess,@ModeratorAccess,@EditAccess,@DeleteAccess,@UploadAccess)
	else
		update yaf_AccessMask set
			[Name]			= @Name,
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
		where AccessMaskID=@AccessMaskID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_forumaccess_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forumaccess_save
GO

create procedure yaf_forumaccess_save(
	@ForumID			int,
	@GroupID			int,
	@AccessMaskID		int
) as
begin
	update yaf_ForumAccess set 
		AccessMaskID=@AccessMaskID
	where 
		ForumID = @ForumID and 
		GroupID = @GroupID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_group_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_group_save
GO

create procedure yaf_group_save(
	@GroupID		int,
	@Name			varchar(50),
	@IsAdmin		bit,
	@IsGuest		bit,
	@IsStart		bit,
	@IsModerator	bit,
	@AccessMaskID	int=null
) as
begin
	if @IsAdmin = 1 update yaf_Group set IsAdmin = 0
	if @IsGuest = 1 update yaf_Group set IsGuest = 0
	if @IsStart = 1 update yaf_Group set IsStart = 0
	if @GroupID>0 begin
		update yaf_Group set
			Name = @Name,
			IsAdmin = @IsAdmin,
			IsGuest = @IsGuest,
			IsStart = @IsStart,
			IsModerator = @IsModerator
		where GroupID = @GroupID
	end
	else begin
		insert into yaf_Group(Name,IsAdmin,IsGuest,IsStart,IsModerator)
		values(@Name,@IsAdmin,@IsGuest,@IsStart,@IsModerator);
		set @GroupID = @@IDENTITY
		insert into yaf_ForumAccess(GroupID,ForumID,AccessMaskID)
		select @GroupID,ForumID,@AccessMaskID from yaf_Forum
	end
	select GroupID = @GroupID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_forum_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_save
GO

create procedure yaf_forum_save(
	@ForumID 		int,
	@CategoryID		int,
	@Name			varchar(50),
	@Description	varchar(255),
	@SortOrder		smallint,
	@Locked			bit,
	@Hidden			bit,
	@IsTest			bit,
	@Moderated		bit,
	@AccessMaskID	int = null
) as
begin
	if @ForumID>0 begin
		update yaf_Forum set 
			Name=@Name,
			Description=@Description,
			SortOrder=@SortOrder,
			Hidden=@Hidden,
			Locked=@Locked,
			CategoryID=@CategoryID,
			IsTest = @IsTest,
			Moderated = @Moderated
		where ForumID=@ForumID
	end
	else begin
		insert into yaf_Forum(Name,Description,SortOrder,Hidden,Locked,CategoryID,IsTest,Moderated,NumTopics,NumPosts)
		values(@Name,@Description,@SortOrder,@Hidden,@Locked,@CategoryID,@IsTest,@Moderated,0,0)
		select @ForumID = @@IDENTITY

		insert into yaf_ForumAccess(GroupID,ForumID,AccessMaskID) 
		select GroupID,@ForumID,@AccessMaskID
		from yaf_Group
	end
	select ForumID = @ForumID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_userforum_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_userforum_list
GO

create procedure yaf_userforum_list(@UserID int=null,@ForumID int=null) as 
begin
	select 
		a.*,
		b.AccessMaskID,
		b.Accepted,
		Access = c.Name
	from
		yaf_User a join yaf_UserForum b on b.UserID=a.UserID
		join yaf_AccessMask c on c.AccessMaskID=b.AccessMaskID
	where
		(@UserID is null or a.UserID=@UserID) and
		(@ForumID is null or b.ForumID=@ForumID)
	order by
		a.Name	
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_userforum_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_userforum_delete
GO

create procedure yaf_userforum_delete(@UserID int,@ForumID int) as
begin
	delete from yaf_UserForum where UserID=@UserID and ForumID=@ForumID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_userforum_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_userforum_save
GO

create procedure yaf_userforum_save(@UserID int,@ForumID int,@AccessMaskID int) as
begin
	if exists(select 1 from yaf_UserForum where UserID=@UserID and ForumID=@ForumID)
		update yaf_UserForum set AccessMaskID=@AccessMaskID where UserID=@UserID and ForumID=@ForumID
	else
		insert into yaf_UserForum(UserID,ForumID,AccessMaskID,Invited,Accepted) values(@UserID,@ForumID,@AccessMaskID,getdate(),1)
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_forumlayout') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forumlayout
GO

create procedure yaf_forumlayout(@UserID int,@CategoryID int=null,@OnlyForum tinyint=0) as
begin
	-- categories	
	if @OnlyForum=0
	select 
		a.CategoryID,
		a.Name
	from 
		yaf_Category a
		join yaf_Forum b on b.CategoryID=a.CategoryID
		join yaf_vaccess v on v.ForumID=b.ForumID
	where
		v.UserID = @UserID and
		(v.ReadAccess <> 0 or b.Hidden = 0) and
		(@CategoryID is null or a.CategoryID = @CategoryID)
	group by
		a.CategoryID,
		a.Name,
		a.SortOrder
	order by 
		a.SortOrder

	-- forums
	select 
		a.CategoryID, 
		Category		= a.Name, 
		ForumID			= b.ForumID,
		Forum			= b.Name, 
		Description,
		Topics			= b.NumTopics,
		Posts			= b.NumPosts,
		LastPosted		= b.LastPosted,
		LastMessageID	= b.LastMessageID,
		LastUserID		= b.LastUserID,
		LastUser		= IsNull(b.LastUserName,(select Name from yaf_User x where x.UserID=b.LastUserID)),
		LastTopicID		= b.LastTopicID,
		LastTopicName	= (select x.Topic from yaf_Topic x where x.TopicID=b.LastTopicID),
		b.Locked,
		b.Moderated,
		Viewing			= (select count(1) from yaf_Active x where x.ForumID=b.ForumID)
	from 
		yaf_Category a, 
		yaf_Forum b,
		yaf_vaccess x
	where 
		a.CategoryID = b.CategoryID and
		(b.Hidden=0 or x.ReadAccess<>0) and
		(@CategoryID is null or a.CategoryID = @CategoryID) and
		x.UserID = @UserID and
		x.ForumID = b.ForumID
	order by
		a.SortOrder,
		b.SortOrder
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
		c.TopicMovedID
	from
		yaf_Topic c,
		yaf_User b,
		yaf_Forum d,
		yaf_User g,
		yaf_vaccess x
	where
		b.UserID = c.UserID and
		@Since < c.LastPosted and
		d.ForumID = c.ForumID and
		g.UserID = @UserID and
		x.UserID = b.UserID and
		x.ForumID = d.ForumID and
		x.ReadAccess <> 0
	order by
		d.Name asc,
		Priority desc,
		LastPosted desc
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
		x.UserID = a.UserID and
		x.ReadAccess <> 0
	order by
		a.Posted desc
end
GO
