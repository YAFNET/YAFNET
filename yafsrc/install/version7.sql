/* Version 0.9.1 */

if not exists(select * from syscolumns where id=object_id('yaf_Group') and name='IsModerator')
	alter table yaf_Group add IsModerator bit null
GO

update yaf_Group set IsModerator=0 where IsModerator is null
GO

alter table yaf_Group alter column IsModerator bit not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='AvatarUpload')
	alter table yaf_System add AvatarUpload bit null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='AvatarRemote')
	alter table yaf_System add AvatarRemote bit null
GO

update yaf_System set AvatarUpload=1,AvatarRemote=1 where AvatarUpload is null and AvatarRemote is null
GO

alter table yaf_System alter column AvatarUpload bit not null
GO

alter table yaf_System alter column AvatarRemote bit not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='AvatarSize')
	alter table yaf_System add AvatarSize int null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='ShowGroups')
	alter table yaf_System add ShowGroups bit null
GO

update yaf_System set ShowGroups=1 where ShowGroups is null
GO

alter table yaf_System alter column ShowGroups bit not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_WatchForum') and name='LastMail')
	alter table yaf_WatchForum add LastMail datetime null
GO

if not exists(select * from syscolumns where id=object_id('yaf_WatchTopic') and name='LastMail')
	alter table yaf_WatchTopic add LastMail datetime null
GO

if not exists (select * from sysobjects where id = object_id(N'yaf_Extension') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE [yaf_Extension] (
	[ExtensionID]	[int] IDENTITY NOT NULL,
	[Name]			[varchar](50) NOT NULL,
	[Priority]		[smallint] NOT NULL,
	[Class]			[varchar](50) NOT NULL,
	[Code]			[varchar](50) NOT NULL
)
GO

if not exists(select * from sysindexes where id=object_id('yaf_Extension') and name='PK_Extension')
ALTER TABLE [yaf_Extension] WITH NOCHECK ADD 
	CONSTRAINT [PK_Extension] PRIMARY KEY  CLUSTERED 
	(
		[ExtensionID]
	)
GO

if exists (select * from sysobjects where id = object_id(N'yaf_forum_listread') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_listread
GO

create procedure yaf_forum_listread(@UserID int,@CategoryID int=null) as
begin
	select 
		a.CategoryID, 
		Category		= a.Name, 
		ForumID			= b.ForumID,
		Forum			= b.Name, 
		Description,
		Topics			= (select count(distinct x.TopicID) from yaf_Topic x,yaf_Message y where x.ForumID=b.ForumID and y.TopicID=x.TopicID and y.Approved<>0),
		Posts			= (select count(1) from yaf_Message x,yaf_Topic y where y.TopicID=x.TopicID and y.ForumID = b.ForumID and x.Approved<>0),
		LastPosted		= b.LastPosted,
		LastMessageID	= b.LastMessageID,
		LastUserID		= b.LastUserID,
		LastUser		= IsNull(b.LastUserName,(select Name from yaf_User x where x.UserID=b.LastUserID)),
		LastTopicID		= b.LastTopicID,
		LastTopicName	= (select x.Topic from yaf_Topic x where x.TopicID=b.LastTopicID),
		b.Locked,
		b.Moderated,
		PostAccess		= (select count(1) from yaf_UserGroup x,yaf_ForumAccess y where x.UserID=@UserID and x.GroupID=y.GroupID and y.ForumID=b.ForumID and y.PostAccess<>0),
		ReplyAccess		= (select count(1) from yaf_UserGroup x,yaf_ForumAccess y where x.UserID=@UserID and x.GroupID=y.GroupID and y.ForumID=b.ForumID and y.ReplyAccess<>0),
		ReadAccess		= (select count(1) from yaf_UserGroup x,yaf_ForumAccess y where x.UserID=@UserID and x.GroupID=y.GroupID and y.ForumID=b.ForumID and y.ReadAccess<>0)		
	from 
		yaf_Category a, 
		yaf_Forum b
	where 
		a.CategoryID = b.CategoryID and
		(b.Hidden=0 or exists(select 1 from yaf_UserGroup x,yaf_ForumAccess y where x.UserID=@UserID and x.GroupID=y.GroupID and y.ForumID=b.ForumID and y.ReadAccess<>0)) and
		(@CategoryID is null or a.CategoryID = @CategoryID)
	order by
		a.SortOrder,
		b.SortOrder
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
	@IsModerator	bit
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
		insert into yaf_ForumAccess(GroupID,ForumID,ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess)
		select @GroupID,ForumID,0,0,0,0,0,0,0,0,0,0 from yaf_Forum
	end
	select GroupID = @GroupID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_deleteavatar') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_deleteavatar
GO

create procedure yaf_user_deleteavatar(@UserID int) as begin
	update yaf_User set AvatarImage = null where UserID = @UserID
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
		yaf_Forum d
	where
		a.Approved <> 0 and
		a.UserID = @UserID and
		b.UserID = a.UserID and
		c.TopicID = a.TopicID and
		d.ForumID = c.ForumID and
		exists(select 1 from yaf_ForumAccess x,yaf_Group y,yaf_UserGroup z where x.ForumID=d.ForumID and y.GroupID=x.GroupID and z.GroupID=y.GroupID and z.UserID=@PageUserID and x.ReadAccess<>0)
	order by
		a.Posted desc
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
	@TemplateID		int = null
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

		if @TemplateID is not null
			insert into yaf_ForumAccess(GroupID,ForumID,ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess) 
			select GroupID,@ForumID,ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess
			from yaf_ForumAccess where ForumID=@TemplateID
		else
			insert into yaf_ForumAccess(GroupID,ForumID,ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess) 
			select GroupID,@ForumID,0,0,0,0,0,0,0,0,0,0 from yaf_Group
	end
	select ForumID = @ForumID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_activity_rank') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_activity_rank
GO

create procedure yaf_user_activity_rank(@StartDate as datetime) AS
begin
	select top 3  ID, Name, NumOfPosts from yaf_User u inner join
	(
		select m.UserID as ID, Count(m.UserID) as NumOfPosts from yaf_Message m
		where m.Posted >= @StartDate
		group by m.UserID
	) as counter
	on u.UserID = counter.ID
	order by NumOfPosts desc
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
		g.UserID = @UserID and
		exists(select 1 from yaf_ForumAccess x,yaf_Group y,yaf_UserGroup z where x.ForumID=d.ForumID and y.GroupID=x.GroupID and z.GroupID=y.GroupID and z.UserID=@UserID and x.ReadAccess<>0)
	order by
		d.Name asc,
		Priority desc,
		LastPosted desc
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_pmessage_markread') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pmessage_markread
GO

create procedure yaf_pmessage_markread(@UserID int,@PMessageID int=null) as begin
	if @PMessageID is null
		update yaf_pmessage set IsRead=1 where ToUserID=@UserID
	else
		update yaf_pmessage set IsRead=1 where ToUserID=@UserID and PMessageID=@PMessageID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_mail_createwatch') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_mail_createwatch
GO

create procedure yaf_mail_createwatch(@TopicID int,@From varchar(50),@Subject varchar(100),@Body text,@UserID int) as begin
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
		a.TopicID = @TopicID and
		(a.LastMail is null or a.LastMail < b.LastVisit)
	
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
		c.ForumID = a.ForumID and
		(a.LastMail is null or a.LastMail < b.LastVisit) and
		not exists(select 1 from yaf_WatchTopic x where x.UserID=b.UserID and x.TopicID=c.TopicID)

	update yaf_WatchTopic set LastMail = getdate() 
	where TopicID = @TopicID
	and UserID <> @UserID
	
	update yaf_WatchForum set LastMail = getdate() 
	where ForumID = (select ForumID from yaf_Topic where TopicID = @TopicID)
	and UserID <> @UserID
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

if exists (select * from sysobjects where id = object_id(N'yaf_extension_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_extension_list
GO

create procedure yaf_extension_list as
begin
	select
		a.*
	from
		yaf_Extension a
	order by
		a.Priority desc
end
GO
