/* Version 0.9.0 */

if not exists(select * from syscolumns where id=object_id('yaf_User') and name='AvatarImage')
	alter table yaf_User add AvatarImage image null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='AvatarWidth')
	alter table yaf_System add AvatarWidth int not null default(50)
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='AvatarHeight')
	alter table yaf_System add AvatarHeight int not null default(80)
GO

if not exists(select * from syscolumns where id=object_id('yaf_Message') and name='Approved')
	alter table yaf_Message add Approved bit null
GO

update yaf_Message set Approved=1 where Approved is null
GO

alter table yaf_Message alter column Approved bit not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_Forum') and name='Moderated')
	alter table yaf_Forum add Moderated bit null
GO

update yaf_Forum set Moderated=0 where Moderated is null
GO

alter table yaf_Forum alter column Moderated bit not null
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

/* Multiple user groups */
if not exists (select * from sysobjects where id = object_id(N'yaf_UserGroup') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE [yaf_UserGroup] (
	[UserID]	[int] NOT NULL,
	[GroupID]	[int] NOT NULL
) ON [PRIMARY]
GO

if not exists(select * from sysindexes where id=object_id('yaf_UserGroup') and name='PK_UserGroup')
ALTER TABLE [yaf_UserGroup] WITH NOCHECK ADD 
	CONSTRAINT [PK_UserGroup] PRIMARY KEY  CLUSTERED 
	(
		[UserID],[GroupID]
	)  ON [PRIMARY] 
GO

if not exists(select * from sysobjects where name='FK_UserGroup_User' and parent_obj=object_id('yaf_UserGroup') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_UserGroup] ADD 
	CONSTRAINT [FK_UserGroup_User] FOREIGN KEY 
	(
		[UserID]
	) REFERENCES [yaf_User] (
		[UserID]
	)
GO

if not exists(select * from sysobjects where name='FK_UserGroup_Group' and parent_obj=object_id('yaf_UserGroup') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_UserGroup] ADD 
	CONSTRAINT [FK_UserGroup_Group] FOREIGN KEY 
	(
		[GroupID]
	) REFERENCES [yaf_Group] (
		[GroupID]
	)
GO

if not exists(select * from syscolumns where id=object_id('yaf_User') and name='GroupID')
	alter table yaf_User add GroupID int
GO

if not exists(select * from yaf_UserGroup)
	insert into yaf_UserGroup(UserID,GroupID) select UserID,GroupID from yaf_User
GO

if exists(select * from sysobjects where name='FK_User_Group' and parent_obj=object_id('yaf_User') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	alter table yaf_User drop constraint FK_User_Group
GO

if exists(select * from syscolumns where id=object_id('yaf_User') and name='GroupID')
	alter table yaf_User drop column GroupID
GO

if exists (select * from sysobjects where id = object_id(N'yaf_usergroup_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_usergroup_save
GO

create procedure yaf_usergroup_save(@UserID int,@GroupID int,@Member bit) as
begin
	if @Member=0
		delete from yaf_UserGroup where UserID=@UserID and GroupID=@GroupID
	else
		insert into yaf_UserGroup(UserID,GroupID)
		select @UserID,@GroupID
		where not exists(select 1 from yaf_UserGroup where UserID=@UserID and GroupID=@GroupID)
end
GO

if not exists (select * from sysobjects where id = object_id(N'yaf_Rank') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE [yaf_Rank] (
	[RankID]		[int] IDENTITY (1, 1) NOT NULL,
	[BoardID]		[int] NOT NULL ,
	[Name]			[varchar] (50) NOT NULL,
	[IsStart]		[bit] NOT NULL,
	[IsLadder]		[bit] NOT NULL,
	[MinPosts]		[int] NULL,
	[RankImage]		[varchar] (50) NULL
) ON [PRIMARY]
GO

if not exists(select * from sysindexes where id=object_id('yaf_Rank') and name='PK_Rank')
ALTER TABLE [yaf_Rank] WITH NOCHECK ADD 
	CONSTRAINT [PK_Rank] PRIMARY KEY  CLUSTERED 
	(
		[RankID]
	)  ON [PRIMARY] 
GO

if not exists(select * from sysindexes where id=object_id('yaf_Rank') and name='IX_Rank')
ALTER TABLE [yaf_Rank] WITH NOCHECK ADD 
	CONSTRAINT [IX_Rank] UNIQUE
	(
		[Name]
	)  ON [PRIMARY] 
GO

if not exists(select * from syscolumns where id=object_id('yaf_Group') and name='IsLadder')
	alter table yaf_Group add IsLadder bit
GO

if not exists(select * from syscolumns where id=object_id('yaf_Group') and name='MinPosts')
	alter table yaf_Group add MinPosts int
GO

if not exists(select * from syscolumns where id=object_id('yaf_Group') and name='RankImage')
	alter table yaf_Group add RankImage varchar(50)
GO

if not exists(select 1 from yaf_Rank)
	insert into yaf_Rank([Name],IsStart,IsLadder,MinPosts,RankImage)
	select [Name],IsStart,IsLadder,MinPosts,RankImage
	from yaf_Group
GO

if exists (select * from sysobjects where id = object_id(N'yaf_rank_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_rank_list
GO

create procedure yaf_rank_list(@RankID int=null) as begin
	if @RankID is null
		select
			a.*
		from
			yaf_Rank a
		order by
			a.Name
	else
		select
			a.*
		from
			yaf_Rank a
		where
			a.RankID = @RankID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_rank_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_rank_delete
GO

create procedure yaf_rank_delete(@RankID int) as begin
	delete from yaf_Rank where RankID = @RankID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_rank_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_rank_save
GO

create procedure yaf_rank_save(
	@RankID		int,
	@Name		varchar(50),
	@IsStart	bit,
	@IsLadder	bit,
	@MinPosts	int,
	@RankImage	varchar(50)=null
) as
begin
	if @IsLadder=0 set @MinPosts = null
	if @IsLadder=1 and @MinPosts is null set @MinPosts = 0
	if @RankID>0 begin
		update yaf_Rank set
			Name = @Name,
			IsStart = @IsStart,
			IsLadder = @IsLadder,
			MinPosts = @MinPosts,
			RankImage = @RankImage
		where RankID = @RankID
	end
	else begin
		insert into yaf_Rank(Name,IsStart,IsLadder,MinPosts,RankImage)
		values(@Name,@IsStart,@IsLadder,@MinPosts,@RankImage);
	end
end
GO

if not exists(select * from syscolumns where id=object_id('yaf_User') and name='RankID')
	alter table yaf_User add RankID int not null default(1)
GO

if not exists(select * from sysobjects where name='FK_User_Rank' and parent_obj=object_id('yaf_User') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	update yaf_User set RankID = (select RankID from yaf_Rank where IsStart<>0)
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_upgrade') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_upgrade
GO

create procedure yaf_user_upgrade(@UserID int) as
begin
	declare @RankID		int
	declare @IsLadder	bit
	declare @MinPosts	int
	declare @NumPosts	int
	-- Get user and rank information
	select
		@RankID = b.RankID,
		@IsLadder = b.IsLadder,
		@MinPosts = b.MinPosts,
		@NumPosts = a.NumPosts
	from
		yaf_User a,
		yaf_Rank b
	where
		a.UserID = @UserID and
		b.RankID = a.RankID
	-- If user isn't member of a ladder rank, exit
	if @IsLadder = 0 return
	-- See if user got enough posts for next ladder group
	select top 1
		@RankID = RankID
	from
		yaf_Rank
	where
		IsLadder = 1 and
		MinPosts <= @NumPosts and
		MinPosts > @MinPosts
	order by
		MinPosts
	if @@ROWCOUNT=1
		update yaf_User set RankID = @RankID where UserID = @UserID
end
GO

if exists(select * from syscolumns where id=object_id('yaf_Group') and name='IsLadder')
	alter table yaf_Group drop column IsLadder
GO

if exists(select * from syscolumns where id=object_id('yaf_Group') and name='MinPosts')
	alter table yaf_Group drop column MinPosts
GO

if exists(select * from syscolumns where id=object_id('yaf_Group') and name='RankImage')
	alter table yaf_Group drop column RankImage
GO

if exists (select * from sysobjects where id = object_id(N'yaf_watchtopic_add') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_watchtopic_add
GO

CREATE  procedure yaf_watchtopic_add(@UserID int,@TopicID int) as
begin
	insert into yaf_WatchTopic(TopicID,UserID,Created)
	select @TopicID, @UserID, getdate()
	where not exists(select 1 from yaf_WatchTopic where TopicID=@TopicID and UserID=@UserID)
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_forum_moderatelist') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_moderatelist
GO

create procedure yaf_forum_moderatelist as begin
	select
		CategoryID	= a.CategoryID,
		CategoryName	= a.Name,
		ForumID		= b.ForumID,
		ForumName	= b.Name,
		MessageCount	= count(d.MessageID)
	from
		yaf_Category a,
		yaf_Forum b,
		yaf_Topic c,
		yaf_Message d
	where
		b.CategoryID = a.CategoryID and
		c.ForumID = b.ForumID and
		d.TopicID = c.TopicID and
		d.Approved=0
	group by
		a.CategoryID,
		a.Name,
		a.SortOrder,
		b.ForumID,
		b.Name,
		b.SortOrder
	order by
		a.SortOrder,
		b.SortOrder
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_topic_updatelastpost') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_updatelastpost
GO

create procedure yaf_topic_updatelastpost as
begin
	update yaf_Topic set
		LastPosted = (select top 1 x.Posted from yaf_Message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0 order by Posted desc),
		LastMessageID = (select top 1 x.MessageID from yaf_Message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0 order by Posted desc),
		LastUserID = (select top 1 x.UserID from yaf_Message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0 order by Posted desc),
		LastUserName = (select top 1 x.UserName from yaf_Message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0 order by Posted desc)
	update yaf_Forum set
		LastPosted = (select top 1 y.Posted from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
		LastTopicID = (select top 1 y.TopicID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
		LastMessageID = (select top 1 y.MessageID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
		LastUserID = (select top 1 y.UserID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
		LastUserName = (select top 1 y.UserName from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc)
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_board_stats') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_board_stats
GO

create procedure yaf_board_stats as begin
	select
		NumPosts	= (select count(1) from yaf_Message where Approved<>0),
		NumTopics	= (select count(1) from yaf_Topic),
		NumUsers	= (select count(1) from yaf_User where Approved<>0),
		BoardStart	= (select min(Joined) from yaf_User)
end
GO
