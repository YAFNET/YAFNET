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

if exists (select * from sysobjects where id = object_id(N'yaf_category_listread') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_category_listread
GO

create procedure yaf_category_listread(@UserID int,@CategoryID int=null) as
begin
	select 
		a.CategoryID,
		a.Name
	from 
		yaf_Category a,
		yaf_Forum b,
		yaf_ForumAccess x,
		yaf_UserGroup y
	where
		b.CategoryID = a.CategoryID and
		x.ForumID = b.ForumID and
		x.GroupID = y.GroupID and
		y.UserID = @UserID and
		(x.ReadAccess = 1 or b.Hidden = 0) and
		(@CategoryID is null or a.CategoryID = @CategoryID)
	group by
		a.CategoryID,
		a.Name,
		a.SortOrder
	order by 
		a.SortOrder
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_active_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_active_list
GO

create procedure yaf_active_list(@Guests bit=0) as
begin
	-- delete non-active
	delete from yaf_Active where DATEDIFF(minute,LastActive,getdate())>5
	-- select active
	if @Guests<>0
		select
			a.UserID,
			a.Name,
			a.IP,
			c.SessionID,
			c.ForumID,
			c.TopicID,
			ForumName = (select Name from yaf_Forum x where x.ForumID=c.ForumID),
			TopicName = (select Topic from yaf_Topic x where x.TopicID=c.TopicID),
			IsGuest = (select 1 from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and y.IsGuest<>0),
			c.Login,
			c.LastActive,
			c.Location,
			Active = DATEDIFF(minute,c.Login,c.LastActive),
			c.Browser,
			c.Platform
		from
			yaf_User a,
			yaf_Active c
		where
			c.UserID = a.UserID
		order by
			c.LastActive desc
	else
		select
			a.UserID,
			a.Name,
			a.IP,
			c.SessionID,
			c.ForumID,
			c.TopicID,
			ForumName = (select Name from yaf_Forum x where x.ForumID=c.ForumID),
			TopicName = (select Topic from yaf_Topic x where x.TopicID=c.TopicID),
			IsGuest = (select 1 from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and y.IsGuest<>0),
			c.Login,
			c.LastActive,
			c.Location,
			Active = DATEDIFF(minute,c.Login,c.LastActive),
			c.Browser,
			c.Platform
		from
			yaf_User a,
			yaf_Active c
		where
			c.UserID = a.UserID and
			not exists(select 1 from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and y.IsGuest<>0)
		order by
			c.LastActive desc
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_forum_stats') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_stats
GO

CREATE  procedure yaf_forum_stats
as
select
	Posts = (select count(1) from yaf_Message),
	Topics = (select count(1) from yaf_Topic),
	Forums = (select count(1) from yaf_Forum),
	Members = (select count(1) from yaf_User),
	LastPost = (select max(Posted) from yaf_Message),
	LastUserID = (select top 1 UserID from yaf_Message order by Posted desc),
	LastUser = (select top 1 b.Name from yaf_Message a, yaf_User b where b.UserID=a.UserID order by Posted desc),
	LastMemberID = (select top 1 UserID from yaf_User where Approved=1 order by Joined desc),
	LastMember = (select top 1 Name from yaf_User where Approved=1 order by Joined desc),
	ActiveUsers = (select count(1) from yaf_Active),
	ActiveMembers = (select count(1) from yaf_Active x where exists(select 1 from yaf_UserGroup y,yaf_Group z where y.UserID=x.UserID and y.GroupID=z.GroupID and z.IsGuest=0)),
	ActiveGuests = (select count(1) from yaf_Active x where exists(select 1 from yaf_UserGroup y,yaf_Group z where y.UserID=x.UserID and y.GroupID=z.GroupID and z.IsGuest<>0))
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_access') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_access
GO

create procedure yaf_user_access(@UserID int,@ForumID int) as
begin
	select
		ReadAccess			= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.ReadAccess<>0),
		PostAccess			= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.PostAccess<>0),
		ReplyAccess			= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.ReplyAccess<>0),
		PriorityAccess		= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.PriorityAccess<>0),
		PollAccess			= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.PollAccess<>0),
		VoteAccess			= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.VoteAccess<>0),
		ModeratorAccess		= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.ModeratorAccess<>0),
		EditAccess			= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.EditAccess<>0),
		DeleteAccess		= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.DeleteAccess<>0),
		UploadAccess		= (select count(1) from yaf_ForumAccess x,yaf_UserGroup y where y.UserID=a.UserID and x.GroupID=y.GroupID and x.ForumID=@ForumID and x.UploadAccess<>0)
	from
		yaf_User a
	where
		a.UserID = @UserID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_topic_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_list
GO

CREATE  procedure yaf_topic_list(@ForumID int,@UserID int,@Announcement smallint,@Date datetime=null) as
begin
	select
		c.ForumID,
		c.TopicID,
		LinkTopicID = IsNull(c.TopicMovedID,c.TopicID),
		c.TopicMovedID,
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
		PostAccess	= (select count(1) from yaf_UserGroup x,yaf_ForumAccess y where x.UserID=g.UserID and y.GroupID=x.GroupID and y.PostAccess<>0),
		ReplyAccess	= (select count(1) from yaf_UserGroup x,yaf_ForumAccess y where x.UserID=g.UserID and y.GroupID=x.GroupID and y.ReplyAccess<>0),
		ReadAccess	= (select count(1) from yaf_UserGroup x,yaf_ForumAccess y where x.UserID=g.UserID and y.GroupID=x.GroupID and y.ReadAccess<>0)
	from
		yaf_Topic c,
		yaf_User b,
		yaf_Forum d,
		yaf_User g
	where
		c.ForumID = @ForumID and
		b.UserID = c.UserID and
		(@Date is null or c.Posted>=@Date or Priority>0) and
		d.ForumID = c.ForumID and
		g.UserID = @UserID and
		((@Announcement=1 and c.Priority=2) or (@Announcement=0 and c.Priority<>2) or (@Announcement<0)) and
		exists(select 1 from yaf_Message x where x.TopicID=c.TopicID and x.Approved<>0)
	order by
		Priority desc,
		LastPosted desc
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_post_list_reverse10') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_post_list_reverse10
GO

create procedure yaf_post_list_reverse10(@TopicID int) as
begin
	set nocount on

	select top 10
		a.Posted,
		Subject = d.Topic,
		a.Message,
		a.UserID,
		UserName = IsNull(a.UserName,b.Name),
		b.Signature
	from
		yaf_Message a, 
		yaf_User b,
		yaf_Topic d
	where
		a.Approved <> 0 and
		a.TopicID = @TopicID and
		b.UserID = a.UserID and
		d.TopicID = a.TopicID
	order by
		a.Posted desc
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
	@MessageID	int output
) as
begin
	declare @ForumID	int
	declare	@Moderated	bit

	select @ForumID = x.ForumID, @Moderated = y.Moderated from yaf_Topic x,yaf_Forum y where x.TopicID = @TopicID and y.ForumID=x.ForumID

	insert into yaf_Message(UserID,Message,TopicID,Posted,UserName,IP,Approved)
	values(@UserID,@Message,@TopicID,getdate(),@UserName,@IP,0)
	set @MessageID = @@IDENTITY
	
	if @Moderated=0
		exec yaf_message_approve @MessageID
end
GO

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
			RankName = b.Name
		from 
			yaf_User a,
			yaf_Rank b
		where 
			a.UserID = @UserID and
			(@Approved is null or a.Approved = @Approved) and
			b.RankID = a.RankID
		order by 
			a.Name
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
		PostAccess	= (select count(1) from yaf_UserGroup x,yaf_ForumAccess y where x.UserID=g.UserID and y.GroupID=x.GroupID and y.PostAccess<>0),
		ReplyAccess	= (select count(1) from yaf_UserGroup x,yaf_ForumAccess y where x.UserID=g.UserID and y.GroupID=x.GroupID and y.ReplyAccess<>0),
		ReadAccess	= (select count(1) from yaf_UserGroup x,yaf_ForumAccess y where x.UserID=g.UserID and y.GroupID=x.GroupID and y.ReadAccess<>0),
		ForumName = d.Name,
		c.TopicMovedID
	from
		yaf_Topic c,
		yaf_User b,
		yaf_Forum d,
		yaf_User g
	where
		b.UserID = c.UserID and
		@Since < c.LastPosted and
		d.ForumID = c.ForumID and
		g.UserID = @UserID
	order by
		d.Name asc,
		Priority desc,
		LastPosted desc
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_delete
GO

create procedure yaf_user_delete(@UserID int) as
begin
	delete from yaf_CheckEmail where UserID = @UserID
	delete from yaf_UserGroup where UserID = @UserID
	delete from yaf_User where UserID = @UserID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_group_member') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_group_member
GO

create procedure yaf_group_member(@UserID int) as
begin
	select 
		a.GroupID,
		a.Name,
		Member = (select count(1) from yaf_UserGroup x where x.UserID=@UserID and x.GroupID=a.GroupID)
	from
		yaf_Group a
	order by
		a.Name
end
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
	[RankID] [int] IDENTITY (1, 1) NOT NULL,
	[Name] [varchar] (50) NOT NULL,
	[IsStart] [bit] NOT NULL,
	[IsLadder] [bit] NOT NULL,
	[MinPosts] [int] NULL,
	[RankImage] [varchar] (50) NULL
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

if not exists(select * from sysobjects where name='FK_User_Rank' and parent_obj=object_id('yaf_User') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_User] ADD 
	CONSTRAINT [FK_User_Rank] FOREIGN KEY 
	(
		[RankID]
	) REFERENCES [yaf_Rank] (
		[RankID]
	)
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

if exists (select * from sysobjects where id = object_id(N'yaf_user_adminsave') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_adminsave
GO

create procedure yaf_user_adminsave(@UserID int,@Name varchar(50),@RankID int) as
begin
	update yaf_User set
		Name = @Name,
		RankID = @RankID
	where UserID = @UserID
	select UserID = @UserID
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
		d.Views,
		d.ForumID,
		Avatar = b.Avatar,
		b.Location,
		b.HomePage,
		b.Signature,
		RankName = c.Name,
		c.RankImage,
		HasAttachments	= (select count(1) from yaf_Attachment x where x.MessageID=a.MessageID),
		HasAvatarImage = (select count(1) from yaf_User x where x.UserID=b.UserID and AvatarImage is not null)
	from
		yaf_Message a, 
		yaf_User b,
		yaf_Rank c,
		yaf_Topic d
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

if exists(select * from syscolumns where id=object_id('yaf_Group') and name='IsLadder')
	alter table yaf_Group drop column IsLadder
GO

if exists(select * from syscolumns where id=object_id('yaf_Group') and name='MinPosts')
	alter table yaf_Group drop column MinPosts
GO

if exists(select * from syscolumns where id=object_id('yaf_Group') and name='RankImage')
	alter table yaf_Group drop column RankImage
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

	insert into yaf_System(SystemID,Version,VersionName,Name,TimeZone,SmtpServer,ForumEmail,AvatarWidth,AvatarHeight,EmailVerification,ShowMoved,BlankLinks)
	values(1,1,'0.7.0',@Name,@TimeZone,@SmtpServer,@ForumEmail,50,80,1,1,0)

	insert into yaf_Rank(Name,IsStart,IsLadder)
	values('Administration',0,0)
	set @RankID = @@IDENTITY

	insert into yaf_Group(Name,IsAdmin,IsGuest,IsStart)
	values('Administration',1,0,0)
	set @GroupID = @@IDENTITY

	insert into yaf_User(RankID,Name,Password,Joined,LastVisit,NumPosts,TimeZone,Approved,Email)
	values(@RankID,@User,@Password,getdate(),getdate(),0,@TimeZone,1,@UserEmail)
	set @UserID = @@IDENTITY

	insert into yaf_UserGroup(UserID,GroupID) values(@UserID,@GroupID)

	insert into yaf_Rank(Name,IsStart,IsLadder)
	values('Guest',0,0)
	set @RankID = @@IDENTITY

	insert into yaf_Group(Name,IsAdmin,IsGuest,IsStart)
	values('Guest',0,1,0)
	set @GroupID = @@IDENTITY

	insert into yaf_User(RankID,Name,Password,Joined,LastVisit,NumPosts,TimeZone,Approved,Email)
	values(@RankID,'Guest','na',getdate(),getdate(),0,@TimeZone,1,@ForumEmail)
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

	insert into yaf_Group(Name,IsAdmin,IsGuest,IsStart)
	values('Member',0,0,1)
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
	@PollID		int=null
) as
begin
	declare @TopicID int
	declare @MessageID int

	insert into yaf_Topic(ForumID,Topic,UserID,Posted,Views,Priority,IsLocked,PollID,UserName)
	values(@ForumID,@Subject,@UserID,getdate(),0,@Priority,0,@PollID,@UserName)
	set @TopicID = @@IDENTITY
	exec yaf_message_save @TopicID,@UserID,@Message,@UserName,@IP,@MessageID output

	select TopicID = @TopicID, MessageID = @MessageID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_emails') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_emails
GO

create procedure yaf_user_emails(@GroupID int=null) as
begin
	if @GroupID = 0 set @GroupID = null
	if @GroupID is null
		select 
			a.Email 
		from 
			yaf_User a
		where 
			a.Email is not null and 
			a.Email<>''
	else
		select 
			a.Email 
		from 
			yaf_User a,
			yaf_UserGroup b
		where 
			b.UserID = a.UserID and
			b.GroupID = @GroupID and 
			a.Email is not null and 
			a.Email<>''
end
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

if exists (select * from sysobjects where id = object_id(N'yaf_user_extvalidate') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_extvalidate
GO

create procedure yaf_user_extvalidate(@Name varchar(50),@Email varchar(50)) as
begin
	declare @UserID int
	declare @RankID int
	declare @TimeZone int
	select @UserID = UserID from yaf_User where Name = @Name
	if @UserID is null or @UserID<1 begin
		select @RankID = max(RankID) from yaf_Rank where IsStart<>0
		select @TimeZone = TimeZone from yaf_System
		if @Email = '' set @Email = null
		insert into yaf_User(RankID,Name,Password,Email,Joined,LastVisit,NumPosts,Approved,Location,HomePage,TimeZone,Avatar) 
		values(@RankID,@Name,'<external>',@Email,getdate(),getdate(),0,0,null,null,@TimeZone,null)
		set @UserID = @@IDENTITY
		
		insert into yaf_UserGroup(UserID,GroupID)
		select @UserID,GroupID from yaf_Group where IsStart<>0
	end
	select UserID = @UserID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_usergroup_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_usergroup_list
GO

create procedure yaf_usergroup_list(@UserID int) as begin
	select 
		b.GroupID,
		b.Name
	from
		yaf_UserGroup a,
		yaf_Group b
	where
		a.UserID = @UserID and
		a.GroupID = b.GroupID
	order by
		b.Name
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
	declare @RankID int

	if @Location is not null and @Location = '' set @Location = null
	if @HomePage is not null and @HomePage = '' set @HomePage = null
	if @Avatar is not null and @Avatar = '' set @Avatar = null

	if @UserID is null or @UserID<1 begin
		if @Email = '' set @Email = null
		
		select @RankID = RankID from yaf_Rank where IsStart<>0
		
		insert into yaf_User(RankID,Name,Password,Email,Joined,LastVisit,NumPosts,Approved,Location,HomePage,TimeZone,Avatar) 
		values(@RankID,@UserName,@Password,@Email,getdate(),getdate(),0,@Approved,@Location,@HomePage,@TimeZone,@Avatar)
	
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
			Avatar = @Avatar
		where UserID = @UserID
		
		if @Email is not null
			update yaf_User set Email = @Email where UserID = @UserID
	end
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_pmessage_markread') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pmessage_markread
GO

create procedure yaf_pmessage_markread(@UserID int) as begin
	update yaf_pmessage set IsRead=1 where ToUserID=@UserID
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

if exists (select * from sysobjects where id = object_id(N'yaf_message_unapproved') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_unapproved
GO

create procedure yaf_message_unapproved(@ForumID int) as begin
	select
		MessageID	= b.MessageID,
		UserName	= IsNull(b.UserName,c.Name),
		Posted		= b.Posted,
		Topic		= a.Topic,
		Message		= b.Message
	from
		yaf_Topic a,
		yaf_Message b,
		yaf_User c
	where
		a.ForumID = @ForumID and
		b.TopicID = a.TopicID and
		b.Approved=0 and
		c.UserID = b.UserID
	order by
		a.Posted
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_message_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_list
GO

create procedure yaf_message_list(@MessageID int) as
begin
	select
		a.MessageID,
		a.UserID,
		UserName = b.Name,
		a.Message,
		c.TopicID,
		c.ForumID,
		c.Topic,
		c.Priority,
		a.Approved
	from
		yaf_Message a,
		yaf_User b,
		yaf_Topic c
	where
		a.MessageID = @MessageID and
		b.UserID = a.UserID and
		c.TopicID = a.TopicID
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
	@Moderated		bit
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
		insert into yaf_Forum(Name,Description,SortOrder,Hidden,Locked,CategoryID,IsTest,Moderated)
		values(@Name,@Description,@SortOrder,@Hidden,@Locked,@CategoryID,@IsTest,@Moderated)
		select @ForumID = @@IDENTITY
		insert into yaf_ForumAccess(GroupID,ForumID,ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess) 
		select GroupID,@ForumID,0,0,0,0,0,0,0,0,0,0 from yaf_Group
	end
	select ForumID = @ForumID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_message_update') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_update
GO

create procedure yaf_message_update(@MessageID int,@Priority int,@Message text) as
begin
	declare @TopicID	int
	declare	@Moderated	bit
	declare	@Approved	bit
	
	set @Approved = 0
	
	select 
		@TopicID	= a.TopicID,
		@Moderated	= c.Moderated
	from 
		yaf_Message a,
		yaf_Topic b,
		yaf_Forum c
	where 
		a.MessageID = @MessageID and
		b.TopicID = a.TopicID and
		c.ForumID = b.ForumID

	if @Moderated=0 set @Approved = 1

	update yaf_Message set
		Message = @Message,
		Edited = getdate(),
		Approved = @Approved
	where
		MessageID = @MessageID

	if @Priority is not null begin
		update yaf_Topic set
			Priority = @Priority
		where
			TopicID = @TopicID
	end
	
	-- If forum is moderated, make sure last post pointers are correct
	if @Moderated<>0 exec yaf_topic_updatelastpost
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
