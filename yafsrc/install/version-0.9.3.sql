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
	alter table yaf_User add LanguageFile nvarchar(50) null
GO

if not exists(select * from syscolumns where id=object_id('yaf_User') and name='ThemeFile')
	alter table yaf_User add ThemeFile nvarchar(50) null
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
		[MSN] [nvarchar] (50) NULL ,
		[YIM] [nvarchar] (30) NULL ,
		[AIM] [nvarchar] (30) NULL ,
		[ICQ] [int] NULL ,
		[RealName] [nvarchar] (50) NULL ,
		[Occupation] [nvarchar] (50) NULL ,
		[Interests] [nvarchar] (100) NULL ,
		[Gender] [tinyint] NULL ,
		[Weblog] [nvarchar] (100) NULL
GO

update yaf_User set Gender=0 where Gender is null
GO

alter table yaf_User alter column Gender tinyint not null
GO

-- NNTP START
if not exists (select * from sysobjects where id = object_id(N'yaf_NntpServer') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
create table yaf_NntpServer(
	[NntpServerID]	[int] identity not null,
	[BoardID]		[int] NOT NULL ,
	[Name]			[nvarchar](50) not null,
	[Address]		[nvarchar](100) not null,
	[UserName]		[nvarchar](50) null,
	[UserPass]		[nvarchar](50) null
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
	[GroupName]		[nvarchar](100) not null,
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

if exists (select * from sysobjects where id = object_id(N'yaf_nntpserver_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_nntpserver_delete
GO

create procedure yaf_nntpserver_delete(@NntpServerID int) as
begin
	delete from yaf_NntpServer where NntpServerID = @NntpServerID
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

if exists (select * from sysobjects where id = object_id(N'yaf_attachment_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_attachment_delete
GO

create procedure yaf_attachment_delete(@AttachmentID int) as begin
	delete from yaf_Attachment where AttachmentID=@AttachmentID
end
go

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

	--exec yaf_user_upgrade @UserID
	exec yaf_forum_updatestats @ForumID
	--exec yaf_topic_updatelastpost @ForumID,null
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
