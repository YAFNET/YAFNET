/* Version 0.9.3 */

if not exists(select * from syscolumns where id=object_id('yaf_Forum') and name='NumTopics')
	alter table yaf_Forum add NumTopics int null
GO

update yaf_Forum set NumTopics = (select count(distinct x.TopicID) from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0) where NumTopics is null
GO

alter table yaf_Forum alter column NumTopics int not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_Forum') and name='NumPosts')
	alter table yaf_Forum add NumPosts int null
GO

update yaf_Forum set NumPosts = (select count(1) from yaf_Message x,yaf_Topic y where y.TopicID=x.TopicID and y.ForumID = yaf_Forum.ForumID and x.Approved<>0) where NumPosts is null
GO

alter table yaf_Forum alter column NumPosts int not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_Topic') and name='NumPosts')
	alter table yaf_Topic add NumPosts int null
GO

update yaf_Topic set NumPosts = (select count(1) from yaf_Message x where x.TopicID=yaf_Topic.TopicID) where NumPosts is null
GO

alter table yaf_Topic alter column NumPosts int not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='AllowRichEdit')
	alter table yaf_System add AllowRichEdit bit null
GO

update yaf_System set AllowRichEdit=1 where AllowRichEdit is null
GO

alter table yaf_System alter column AllowRichEdit bit not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='AllowUserTheme')
	alter table yaf_System add AllowUserTheme bit null
GO

update yaf_System set AllowUserTheme=0 where AllowUserTheme is null
GO

alter table yaf_System alter column AllowUserTheme bit not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='AllowUserLanguage')
	alter table yaf_System add AllowUserLanguage bit null
GO

update yaf_System set AllowUserLanguage=0 where AllowUserLanguage is null
GO

alter table yaf_System alter column AllowUserLanguage bit not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_User') and name='LanguageFile')
	alter table yaf_User add LanguageFile varchar(50) null
GO

if not exists(select * from syscolumns where id=object_id('yaf_User') and name='ThemeFile')
	alter table yaf_User add ThemeFile varchar(50) null
GO

if not exists(select * from sysobjects where name='FK_Attachment_Message' and parent_obj=object_id('yaf_Attachment') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_Attachment] ADD 
	CONSTRAINT [FK_Attachment_Message] FOREIGN KEY 
	(
		[MessageID]
	) REFERENCES [yaf_Message] (
		[MessageID]
	)
GO

if not exists(select * from syscolumns where id=object_id('yaf_User') and name='MSN')
	alter table yaf_User add 
		[MSN] [varchar] (50) NULL ,
		[YIM] [varchar] (30) NULL ,
		[AIM] [varchar] (30) NULL ,
		[ICQ] [int] NULL ,
		[RealName] [varchar] (50) NULL ,
		[Occupation] [varchar] (50) NULL ,
		[Interests] [varchar] (100) NULL ,
		[Gender] [tinyint] NULL ,
		[Weblog] [varchar] (100) NULL
GO

update yaf_User set Gender=0 where Gender is null
GO

alter table yaf_User alter column Gender tinyint not null
GO

-- NNTP START
if not exists (select * from sysobjects where id = object_id(N'yaf_NntpServer') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
create table yaf_NntpServer(
	[NntpServerID]	[int] identity not null,
	[Name]			[varchar](50) not null,
	[Address]		[varchar](100) not null,
	[UserName]		[varchar](50) null,
	[UserPass]		[varchar](50) null
)
GO

if not exists(select * from sysindexes where id=object_id('yaf_NntpServer') and name='PK_NntpServer')
ALTER TABLE [yaf_NntpServer] WITH NOCHECK ADD 
	CONSTRAINT [PK_NntpServer] PRIMARY KEY  CLUSTERED 
	(
		[NntpServerID]
	) 
GO

if not exists (select * from sysobjects where id = object_id(N'yaf_NntpForum') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
create table yaf_NntpForum(
	[NntpForumID]	[int] identity not null,
	[NntpServerID]	[int] not null,
	[GroupName]		[varchar](100) not null,
	[ForumID]		[int] not null,
	[LastMessageNo]	[int] not null,
	[LastUpdate]	[datetime] not null
)
GO

if not exists(select * from sysindexes where id=object_id('yaf_NntpForum') and name='PK_NntpForum')
ALTER TABLE [yaf_NntpForum] WITH NOCHECK ADD 
	CONSTRAINT [PK_NntpForum] PRIMARY KEY  CLUSTERED 
	(
		[NntpForumID]
	) 
GO

if not exists(select * from sysobjects where name='FK_NntpForum_NntpServer' and parent_obj=object_id('yaf_NntpForum') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_NntpForum] ADD 
	CONSTRAINT [FK_NntpForum_NntpServer] FOREIGN KEY 
	(
		[NntpServerID]
	) REFERENCES [yaf_NntpServer] (
		[NntpServerID]
	)
GO

if not exists(select * from sysobjects where name='FK_NntpForum_Forum' and parent_obj=object_id('yaf_NntpForum') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_NntpForum] ADD 
	CONSTRAINT [FK_NntpForum_Forum] FOREIGN KEY 
	(
		[ForumID]
	) REFERENCES [yaf_Forum] (
		[ForumID]
	)
GO

if not exists (select * from sysobjects where id = object_id(N'yaf_NntpTopic') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
create table yaf_NntpTopic(
	[NntpTopicID]		[int] identity not null,
	[NntpForumID]		[int] not null,
	[Thread]			[char](32) not null,
	[TopicID]			[int] not null
)
GO

if not exists(select * from sysindexes where id=object_id('yaf_NntpTopic') and name='PK_NntpTopic')
ALTER TABLE [yaf_NntpTopic] WITH NOCHECK ADD 
	CONSTRAINT [PK_NntpTopic] PRIMARY KEY  CLUSTERED 
	(
		[NntpTopicID]
	) 
GO

if not exists(select * from sysobjects where name='FK_NntpTopic_NntpForum' and parent_obj=object_id('yaf_NntpTopic') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_NntpTopic] ADD 
	CONSTRAINT [FK_NntpTopic_NntpForum] FOREIGN KEY 
	(
		[NntpForumID]
	) REFERENCES [yaf_NntpForum] (
		[NntpForumID]
	)
GO

if not exists(select * from sysobjects where name='FK_NntpTopic_Topic' and parent_obj=object_id('yaf_NntpTopic') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_NntpTopic] ADD 
	CONSTRAINT [FK_NntpTopic_Topic] FOREIGN KEY 
	(
		[TopicID]
	) REFERENCES [yaf_Topic] (
		[TopicID]
	)
GO

if exists (select * from sysobjects where id = object_id(N'yaf_nntpforum_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_nntpforum_list
GO

create procedure yaf_nntpforum_list(@Minutes int=null,@NntpForumID int=null) as
begin
	select
		a.Name,
		a.Address,
		a.NntpServerID,
		b.NntpForumID,
		b.GroupName,
		b.ForumID,
		b.LastMessageNo,
		b.LastUpdate,
		ForumName = c.Name
	from
		yaf_NntpServer a,
		yaf_NntpForum b,
		yaf_Forum c
	where
		b.NntpServerID = a.NntpServerID and
		(@Minutes is null or datediff(n,b.LastUpdate,getdate())>@Minutes) and
		(@NntpForumID is null or b.NntpForumID=@NntpForumID) and
		c.ForumID = b.ForumID
	order by
		a.Name,
		b.GroupName
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_nntptopic_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_nntptopic_list
GO

create procedure yaf_nntptopic_list(@Thread char(32)) as
begin
	select
		a.*
	from
		yaf_NntpTopic a
	where
		a.Thread = @Thread
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_nntpserver_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_nntpserver_list
GO

create procedure yaf_nntpserver_list(@NntpServerID int=null) as
begin
	if @NntpServerID is null
		select * from yaf_NntpServer order by Name
	else
		select * from yaf_NntpServer where NntpServerID=@NntpServerID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_nntpserver_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_nntpserver_save
GO

create procedure yaf_nntpserver_save(
	@NntpServerID 	int=null,
	@Name		varchar(50),
	@Address	varchar(100),
	@UserName	varchar(50)=null,
	@UserPass	varchar(50)=null
) as begin
	if @NntpServerID is null
		insert into yaf_NntpServer(Name,Address,UserName,UserPass)
		values(@Name,@Address,@UserName,@UserPass)
	else
		update yaf_NntpServer set
			Name = @Name,
			Address = @Address,
			UserName = @UserName,
			UserPass = @UserPass
		where NntpServerID = @NntpServerID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_nntpserver_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_nntpserver_delete
GO

create procedure yaf_nntpserver_delete(@NntpServerID int) as
begin
	delete from yaf_NntpServer where NntpServerID = @NntpServerID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_nntpforum_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_nntpforum_save
GO

create procedure yaf_nntpforum_save(@NntpForumID int=null,@NntpServerID int,@GroupName varchar(100),@ForumID int) as
begin
	if @NntpForumID is null
		insert into yaf_NntpForum(NntpServerID,GroupName,ForumID,LastMessageNo,LastUpdate)
		values(@NntpServerID,@GroupName,@ForumID,0,getdate())
	else
		update yaf_NntpForum set
			NntpServerID = @NntpServerID,
			GroupName = @GroupName,
			ForumID = @ForumID
		where NntpForumID = @NntpForumID
end
GO

-- NNTP END

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
	@ShowGroups			bit,
	@AvatarWidth		int,
	@AvatarHeight		int,
	@AvatarUpload		bit,
	@AvatarRemote		bit,
	@AvatarSize			int=null,
	@AllowRichEdit		bit,
	@AllowUserTheme		bit,
	@AllowUserLanguage	bit
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
		ShowGroups = @ShowGroups,
		AvatarWidth = @AvatarWidth,
		AvatarHeight = @AvatarHeight,
		AvatarUpload = @AvatarUpload,
		AvatarRemote = @AvatarRemote,
		AvatarSize = @AvatarSize,
		AllowRichEdit = @AllowRichEdit,
		AllowUserTheme = @AllowUserTheme,
		AllowUserLanguage = @AllowUserLanguage
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
	declare @RankID int
	declare @UserID int

	insert into yaf_System(SystemID,Version,VersionName,Name,TimeZone,SmtpServer,ForumEmail,AvatarWidth,AvatarHeight,AvatarUpload,AvatarRemote,EmailVerification,ShowMoved,BlankLinks,ShowGroups,AllowRichEdit,AllowUserTheme,AllowUserLanguage)
	values(1,1,'0.7.0',@Name,@TimeZone,@SmtpServer,@ForumEmail,50,80,0,0,1,1,0,1,1,0,0)

	insert into yaf_Rank(Name,IsStart,IsLadder)
	values('Administration',0,0)
	set @RankID = @@IDENTITY

	insert into yaf_Group(Name,IsAdmin,IsGuest,IsStart,IsModerator)
	values('Administration',1,0,0,0)
	set @GroupID = @@IDENTITY

	insert into yaf_User(RankID,Name,Password,Joined,LastVisit,NumPosts,TimeZone,Approved,Email,Gender)
	values(@RankID,@User,@Password,getdate(),getdate(),0,@TimeZone,1,@UserEmail,0)
	set @UserID = @@IDENTITY

	insert into yaf_UserGroup(UserID,GroupID) values(@UserID,@GroupID)

	insert into yaf_Rank(Name,IsStart,IsLadder)
	values('Guest',0,0)
	set @RankID = @@IDENTITY

	insert into yaf_Group(Name,IsAdmin,IsGuest,IsStart,IsModerator)
	values('Guest',0,1,0,0)
	set @GroupID = @@IDENTITY

	insert into yaf_User(RankID,Name,Password,Joined,LastVisit,NumPosts,TimeZone,Approved,Email,Gender)
	values(@RankID,'Guest','na',getdate(),getdate(),0,@TimeZone,1,@ForumEmail,0)
	set @UserID = @@IDENTITY

	insert into yaf_UserGroup(UserID,GroupID) values(@UserID,@GroupID)

	-- users starts as Newbie
	insert into yaf_Rank(Name,IsStart,IsLadder,MinPosts)
	values('Newbie',1,1,0)

	-- advances to Member
	insert into yaf_Rank(Name,IsStart,IsLadder,MinPosts)
	values('Member',0,1,10)

	-- and ends up as Advanced Member
	insert into yaf_Rank(Name,IsStart,IsLadder,MinPosts)
	values('Advanced Member',0,1,30)

	insert into yaf_Group(Name,IsAdmin,IsGuest,IsStart,IsModerator)
	values('Member',0,0,1,0)
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_topic_move') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_move
GO

create procedure yaf_topic_move(@TopicID int,@ForumID int,@ShowMoved bit) as
begin
	declare @OldForumID int

	select @OldForumID = ForumID from yaf_Topic where TopicID = @TopicID

	if @ShowMoved<>0 begin
		-- create a moved message
		insert into yaf_Topic(ForumID,UserID,UserName,Posted,Topic,Views,IsLocked,Priority,PollID,TopicMovedID,LastPosted)
		select ForumID,UserID,UserName,Posted,Topic,0,IsLocked,Priority,PollID,@TopicID,LastPosted
		from yaf_Topic where TopicID = @TopicID
	end

	-- move the topic
	update yaf_Topic set ForumID = @ForumID where TopicID = @TopicID

	-- update last posts
	exec yaf_topic_updatelastpost @OldForumID
	exec yaf_topic_updatelastpost @ForumID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_topic_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_list
GO

CREATE  procedure yaf_topic_list(@ForumID int,@Announcement smallint,@Date datetime=null,@Offset int,@Count int) as
begin
	create table #data(
		RowNo	int identity primary key not null,
		TopicID	int not null
	)

	insert into #data(TopicID)
	select
		c.TopicID
	from
		yaf_Topic c join yaf_User b on b.UserID=c.UserID join yaf_Forum d on d.ForumID=c.ForumID
	where
		c.ForumID = @ForumID and
		(@Date is null or c.Posted>=@Date or c.LastPosted>=@Date or Priority>0) and
		((@Announcement=1 and c.Priority=2) or (@Announcement=0 and c.Priority<>2) or (@Announcement<0)) and
		(c.TopicMovedID is not null or c.NumPosts>0)
	order by
		Priority desc,
		c.LastPosted desc

	declare	@RowCount int
	set @RowCount = (select count(1) from #data)

	select
		[RowCount] = @RowCount,
		c.ForumID,
		c.TopicID,
		LinkTopicID = IsNull(c.TopicMovedID,c.TopicID),
		c.TopicMovedID,
		Subject = c.Topic,
		c.UserID,
		Starter = IsNull(c.UserName,b.Name),
		Replies = c.NumPosts - 1,
		Views = c.Views,
		LastPosted = c.LastPosted,
		LastUserID = c.LastUserID,
		LastUserName = IsNull(c.LastUserName,(select Name from yaf_User x where x.UserID=c.LastUserID)),
		LastMessageID = c.LastMessageID,
		LastTopicID = c.TopicID,
		c.IsLocked,
		c.Priority,
		c.PollID
	from
		yaf_Topic c join yaf_User b on b.UserID=c.UserID join yaf_Forum d on d.ForumID=c.ForumID join #data e on e.TopicID=c.TopicID
	where
		e.RowNo between @Offset+1 and @Offset + @Count
	order by
		e.RowNo
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_topic_updatelastpost') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_updatelastpost
GO

create procedure yaf_topic_updatelastpost(@ForumID int=null,@TopicID int=null) as
begin
	-- this really needs some work...
	if @TopicID is not null
		update yaf_Topic set
			LastPosted = (select top 1 x.Posted from yaf_Message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0 order by Posted desc),
			LastMessageID = (select top 1 x.MessageID from yaf_Message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0 order by Posted desc),
			LastUserID = (select top 1 x.UserID from yaf_Message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0 order by Posted desc),
			LastUserName = (select top 1 x.UserName from yaf_Message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0 order by Posted desc)
		where TopicID = @TopicID
	else
		update yaf_Topic set
			LastPosted = (select top 1 x.Posted from yaf_Message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0 order by Posted desc),
			LastMessageID = (select top 1 x.MessageID from yaf_Message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0 order by Posted desc),
			LastUserID = (select top 1 x.UserID from yaf_Message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0 order by Posted desc),
			LastUserName = (select top 1 x.UserName from yaf_Message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0 order by Posted desc)
		where TopicMovedID is null
		and (@ForumID is null or ForumID=@ForumID)

	if @ForumID is not null
		update yaf_Forum set
			LastPosted = (select top 1 y.Posted from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
			LastTopicID = (select top 1 y.TopicID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
			LastMessageID = (select top 1 y.MessageID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
			LastUserID = (select top 1 y.UserID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
			LastUserName = (select top 1 y.UserName from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc)
		where ForumID = @ForumID
	else 
		update yaf_Forum set
			LastPosted = (select top 1 y.Posted from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
			LastTopicID = (select top 1 y.TopicID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
			LastMessageID = (select top 1 y.MessageID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
			LastUserID = (select top 1 y.UserID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
			LastUserName = (select top 1 y.UserName from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc)
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_watchforum_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_watchforum_list
GO

create procedure yaf_watchforum_list(@UserID int) as
begin
	select
		a.*,
		ForumName = b.Name,
		Messages = (select count(1) from yaf_Topic x, yaf_Message y where x.ForumID=a.ForumID and y.TopicID=x.TopicID),
		Topics = (select count(1) from yaf_Topic x where x.ForumID=a.ForumID and x.TopicMovedID is null),
		b.LastPosted,
		b.LastMessageID,
		LastTopicID = (select TopicID from yaf_Message x where x.MessageID=b.LastMessageID),
		b.LastUserID,
		LastUserName = IsNull(b.LastUserName,(select Name from yaf_User x where x.UserID=b.LastUserID))
	from
		yaf_WatchForum a,
		yaf_Forum b
	where
		a.UserID = @UserID and
		b.ForumID = a.ForumID
end
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
	@Location		varchar(50),
	@HomePage		varchar(50),
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

if exists (select * from sysobjects where id = object_id(N'yaf_attachment_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_attachment_list
GO

create procedure yaf_attachment_list(@MessageID int=null) as begin
	if @MessageID is not null
		select * from yaf_Attachment where MessageID=@MessageID
	else
		select 
			a.*,
			Posted		= b.Posted,
			ForumID		= d.ForumID,
			ForumName	= d.Name,
			TopicID		= c.TopicID,
			TopicName	= c.Topic
		from 
			yaf_Attachment a,
			yaf_Message b,
			yaf_Topic c,
			yaf_Forum d
		where
			b.MessageID = a.MessageID and
			c.TopicID = b.TopicID and
			d.ForumID = c.ForumID
		order by
			d.Name,
			c.Topic,
			b.Posted
end
go

if exists (select * from sysobjects where id = object_id(N'yaf_attachment_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_attachment_delete
GO

create procedure yaf_attachment_delete(@AttachmentID int) as begin
	delete from yaf_Attachment where AttachmentID=@AttachmentID
end
go

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

	delete from yaf_PMessage where FromUserID=@UserID or ToUserID=@UserID
	delete from yaf_CheckEmail where UserID = @UserID
	delete from yaf_WatchTopic where UserID = @UserID
	delete from yaf_WatchForum where UserID = @UserID
	delete from yaf_UserGroup where UserID = @UserID
	delete from yaf_User where UserID = @UserID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_guest') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_guest
GO

create procedure yaf_user_guest as
begin
	select top 1
		a.UserID
	from
		yaf_User a,
		yaf_UserGroup b,
		yaf_Group c
	where
		b.UserID = a.UserID and
		b.GroupID = c.GroupID and
		c.IsGuest<>0
end
go

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
	delete from yaf_Message from yaf_Topic where yaf_Topic.ForumID = @ForumID and yaf_Message.TopicID = yaf_Topic.TopicID
	delete from yaf_Topic where ForumID = @ForumID
	delete from yaf_ForumAccess where ForumID = @ForumID
	delete from yaf_Forum where ForumID = @ForumID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_topic_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_delete
GO

CREATE   procedure yaf_topic_delete(@TopicID int,@UpdateLastPost bit=1) as
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
	delete from yaf_Message where TopicID = @TopicID
	delete from yaf_Topic where TopicMovedID = @TopicID
	delete from yaf_Topic where TopicID = @TopicID
	--commit
	if @UpdateLastPost<>0
		exec yaf_topic_updatelastpost
	
	if @ForumID is not null
		exec yaf_forum_updatestats @ForumID
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
			datediff(dd,LastPosted,getdate())>@Days
	end
	else begin
		set @c = cursor for
		select 
			TopicID
		from 
			yaf_Topic
		where 
			Priority = 0 and
			datediff(dd,LastPosted,getdate())>@Days
	end
	open @c
	fetch @c into @TopicID
	while @@FETCH_STATUS=0 begin
		exec yaf_topic_delete @TopicID,0
		set @Count = @Count + 1
		fetch @c into @TopicID
	end
	close @c
	deallocate @c

	-- This takes forever with many posts...
	--exec yaf_topic_updatelastpost

	select [Count] = @Count
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_forum_updatestats') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_updatestats
GO

create procedure yaf_forum_updatestats(@ForumID int) as
begin
	update yaf_Forum set 
		NumPosts = (select count(1) from yaf_Message x,yaf_Topic y where y.TopicID=x.TopicID and y.ForumID = yaf_Forum.ForumID and x.Approved<>0),
		NumTopics = (select count(distinct x.TopicID) from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0)
	where ForumID=@ForumID
end
go

if exists (select * from sysobjects where id = object_id(N'yaf_message_approve') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_approve
GO

create procedure yaf_message_approve(@MessageID int) as begin
	declare	@UserID		int
	declare	@ForumID	int
	declare	@TopicID	int
	declare @Posted		datetime
	declare	@UserName	varchar(50)

	select 
		@UserID = a.UserID,
		@TopicID = a.TopicID,
		@ForumID = b.ForumID,
		@Posted = a.Posted,
		@UserName = a.UserName
	from
		yaf_Message a,
		yaf_Topic b
	where
		a.MessageID = @MessageID and
		b.TopicID = a.TopicID

	-- update yaf_Message
	update yaf_Message set Approved = 1 where MessageID = @MessageID

	-- update yaf_User
	update yaf_User set NumPosts = NumPosts + 1 where UserID = @UserID
	exec yaf_user_upgrade @UserID

	-- update yaf_Forum
	update yaf_Forum set
		LastPosted = @Posted,
		LastTopicID = @TopicID,
		LastMessageID = @MessageID,
		LastUserID = @UserID,
		LastUserName = @UserName
	where ForumID = @ForumID

	-- update yaf_Topic
	update yaf_Topic set
		LastPosted = @Posted,
		LastMessageID = @MessageID,
		LastUserID = @UserID,
		LastUserName = @UserName,
		NumPosts = (select count(1) from yaf_Message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0)
	where TopicID = @TopicID
	
	-- update forum stats
	exec yaf_forum_updatestats @ForumID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_message_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_delete
GO

CREATE  procedure yaf_message_delete(@MessageID int) as
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

if exists (select * from sysobjects where id = object_id(N'yaf_post_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_post_list
GO

create procedure yaf_post_list(@TopicID int,@UpdateViewCount smallint=1) as
begin
	set nocount on

	if @UpdateViewCount>0
		update yaf_Topic set [Views] = [Views] + 1 where TopicID = @TopicID

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
		d.Views,
		d.ForumID,
		Avatar = b.Avatar,
		b.Location,
		b.HomePage,
		b.Signature,
		RankName = c.Name,
		c.RankImage,
		HasAttachments	= (select count(1) from yaf_Attachment x where x.MessageID=a.MessageID),
		HasAvatarImage = (select count(1) from yaf_User x where x.UserID=b.UserID and AvatarImage is not null),
		e.AvatarUpload,
		e.AvatarRemote,
		e.AvatarWidth,
		e.AvatarHeight
	from
		yaf_Message a, 
		yaf_User b,
		yaf_Rank c,
		yaf_Topic d,
		yaf_System e
	where
		a.Approved <> 0 and
		a.TopicID = @TopicID and
		b.UserID = a.UserID and
		c.RankID = b.RankID and
		d.TopicID = a.TopicID
	order by
		a.Posted asc
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_message_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_save
GO

CREATE  procedure yaf_message_save(
	@TopicID	int,
	@UserID		int,
	@Message	text,
	@UserName	varchar(50)=null,
	@IP			varchar(15),
	@Posted		datetime=null,
	@MessageID	int output
) as
begin
	declare @ForumID	int
	declare	@Moderated	bit

	if @Posted is null set @Posted = getdate()

	select @ForumID = x.ForumID, @Moderated = y.Moderated from yaf_Topic x,yaf_Forum y where x.TopicID = @TopicID and y.ForumID=x.ForumID

	insert into yaf_Message(UserID,Message,TopicID,Posted,UserName,IP,Approved)
	values(@UserID,@Message,@TopicID,@Posted,@UserName,@IP,0)
	set @MessageID = @@IDENTITY
	
	if @Moderated=0
		exec yaf_message_approve @MessageID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_topic_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_save
GO

create procedure yaf_topic_save(
	@ForumID	int,
	@Subject	varchar(100),
	@UserID		int,
	@Message	text,
	@Priority	smallint,
	@UserName	varchar(50)=null,
	@IP			varchar(15),
	@PollID		int=null,
	@Posted		datetime=null
) as
begin
	declare @TopicID int
	declare @MessageID int

	if @Posted is null set @Posted = getdate()

	insert into yaf_Topic(ForumID,Topic,UserID,Posted,Views,Priority,IsLocked,PollID,UserName,NumPosts)
	values(@ForumID,@Subject,@UserID,@Posted,0,@Priority,0,@PollID,@UserName,0)
	set @TopicID = @@IDENTITY
	exec yaf_message_save @TopicID,@UserID,@Message,@UserName,@IP,@Posted,@MessageID output

	select TopicID = @TopicID, MessageID = @MessageID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_nntptopic_savemessage') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_nntptopic_savemessage
GO

create procedure yaf_nntptopic_savemessage(
	@NntpForumID	int,
	@Topic 		varchar(100),
	@Body 		text,
	@UserID 	int,
	@UserName	varchar(50),
	@IP		varchar(15),
	@Posted		datetime,
	@Thread		char(32)
) as 
begin
	declare	@ForumID	int
	declare @TopicID	int

	select @ForumID=ForumID from yaf_NntpForum where NntpForumID=@NntpForumID

	if exists(select 1 from yaf_NntpTopic where Thread=@Thread)
	begin
		-- thread exists
		select @TopicID=TopicID from yaf_NntpTopic where Thread=@Thread
	end else
	begin
		-- thread doesn't exists
		insert into yaf_Topic(ForumID,UserID,UserName,Posted,Topic,[Views],IsLocked,Priority,NumPosts)
		values(@ForumID,@UserID,@UserName,@Posted,@Topic,0,0,0,0)
		set @TopicID=@@IDENTITY

		insert into yaf_NntpTopic(NntpForumID,Thread,TopicID)
		values(@NntpForumID,@Thread,@TopicID)
	end

	-- save message
	insert into yaf_Message(TopicID,UserID,UserName,Posted,Message,IP,Approved)
	values(@TopicID,@UserID,@UserName,@Posted,@Body,@IP,1)

	-- update user
	update yaf_User set NumPosts=NumPosts+1 where UserID=@UserID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_nntpforum_update') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_nntpforum_update
GO

create procedure yaf_nntpforum_update(@NntpForumID int,@LastMessageNo int,@UserID int) as
begin
	declare	@ForumID	int
	
	select @ForumID=ForumID from yaf_NntpForum where NntpForumID=@NntpForumID

	update yaf_NntpForum set
		LastMessageNo = @LastMessageNo,
		LastUpdate = getdate()
	where NntpForumID = @NntpForumID

	update yaf_Topic set 
		NumPosts = (select count(1) from yaf_message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0)
	where ForumID=@ForumID

	exec yaf_user_upgrade @UserID
	exec yaf_forum_updatestats @ForumID
	exec yaf_topic_updatelastpost @ForumID,null
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_category_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_category_delete
GO

create procedure yaf_category_delete(@CategoryID int) as
begin
	declare @flag int
 
	if exists(select 1 from yaf_Forum where CategoryID =  @CategoryID)
	begin
		set @flag = 0
	end else
	begin
		delete from yaf_Category where CategoryID = @CategoryID
		set @flag = 1
	end

	select @flag
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_active_listforum') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_active_listforum
GO

create procedure yaf_active_listforum(@ForumID int) as
begin
	select
		UserID		= a.UserID,
		UserName	= b.Name
	from
		yaf_Active a join yaf_User b on b.UserID=a.UserID
	where
		a.ForumID = @ForumID
	group by
		a.UserID,
		b.Name
	order by
		b.Name
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_list
GO

create procedure yaf_user_list(@UserID int=null,@Approved bit=null) as
begin
	if @UserID is null
		select 
			a.*,
			a.NumPosts,
			IsAdmin = (select count(1) from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and y.IsAdmin<>0),
			RankName = b.Name
		from 
			yaf_User a,
			yaf_Rank b
		where 
			(@Approved is null or a.Approved = @Approved) and
			b.RankID = a.RankID
		order by 
			a.Name
	else
		select 
			a.*,
			a.NumPosts,
			IsAdmin = (select count(1) from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and y.IsAdmin<>0),
			RankName = b.Name,
			NumDays = datediff(d,a.Joined,getdate())+1,
			NumPostsForum = (select count(1) from yaf_Message x where x.Approved<>0),
			HasAvatarImage = (select count(1) from yaf_User x where x.UserID=a.UserID and AvatarImage is not null),
			c.AvatarUpload,
			c.AvatarRemote,
			c.AvatarWidth,
			c.AvatarHeight
		from 
			yaf_User a,
			yaf_Rank b,
			yaf_System c
		where 
			a.UserID = @UserID and
			(@Approved is null or a.Approved = @Approved) and
			b.RankID = a.RankID
		order by 
			a.Name
end
GO
