/* Version 0.9.4 */

-- Deletes all defaults for columns (fucks up when dropping columns)
begin
	declare @tname	nvarchar(100)
	declare @name	nvarchar(100)

	declare c cursor for 
	select tname=object_name(parent_obj),name from sysobjects 
	where OBJECTPROPERTY(id,N'IsDefaultCnst')<>0
	and name like '%yaf%'

	open c
	fetch next from c into @tname,@name
	while @@fetch_status=0 begin
		execute('alter table ' + @tname + ' drop constraint ' + @name)
		fetch next from c into @tname,@name
	end
	close c
	deallocate c
end
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='UseFileTable')
	alter table yaf_System add UseFileTable bit null
GO

update yaf_System set UseFileTable=0 where UseFileTable is null
GO

alter table yaf_System alter column UseFileTable bit not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='MaxFileSize')
	alter table yaf_System add MaxFileSize int null
GO

if not exists(select * from syscolumns where id=object_id('yaf_Attachment') and name='ContentType')
	alter table yaf_Attachment add ContentType nvarchar(50) null
GO

if not exists(select * from syscolumns where id=object_id('yaf_Attachment') and name='Downloads')
	alter table yaf_Attachment add Downloads int null
GO

update yaf_Attachment set Downloads=0 where Downloads is null
GO

alter table yaf_Attachment alter column Downloads int not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_Attachment') and name='FileData')
	alter table yaf_Attachment add FileData image null
GO

if not exists (select * from sysobjects where id = object_id(N'yaf_AccessMask') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
CREATE TABLE [yaf_AccessMask](
	[AccessMaskID]		[int] IDENTITY NOT NULL ,
	[BoardID]			[int] NOT NULL ,
	[Name]				[nvarchar](50) NOT NULL ,
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
	select min('Mask ' + convert(nvarchar,GroupID) + ' - ' + convert(nvarchar,ForumID)),ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess
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

if exists (select * from sysobjects where id = object_id(N'yaf_active_listtopic') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_active_listtopic
GO

create procedure yaf_active_listtopic(@TopicID int) as
begin
	select
		UserID		= a.UserID,
		UserName	= b.Name
	from
		yaf_Active a join yaf_User b on b.UserID=a.UserID
	where
		a.TopicID = @TopicID
	group by
		a.UserID,
		b.Name
	order by
		b.Name
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
		c.Posted,
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
		c.PollID,
		ForumLocked = d.Locked
	from
		yaf_Topic c join yaf_User b on b.UserID=c.UserID join yaf_Forum d on d.ForumID=c.ForumID join #data e on e.TopicID=c.TopicID
	where
		e.RowNo between @Offset+1 and @Offset + @Count
	order by
		e.RowNo
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
		insert into yaf_Topic(ForumID,UserID,UserName,Posted,Topic,Views,IsLocked,Priority,PollID,TopicMovedID,LastPosted,NumPosts)
		select ForumID,UserID,UserName,Posted,Topic,0,IsLocked,Priority,PollID,@TopicID,LastPosted,0
		from yaf_Topic where TopicID = @TopicID
	end

	-- move the topic
	update yaf_Topic set ForumID = @ForumID where TopicID = @TopicID

	-- update last posts
	exec yaf_topic_updatelastpost @OldForumID
	exec yaf_topic_updatelastpost @ForumID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_attachment_download') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_attachment_download
GO

create procedure yaf_attachment_download(@AttachmentID int) as
begin
	update yaf_Attachment set Downloads=Downloads+1 where AttachmentID=@AttachmentID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_saveavatar') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_saveavatar
GO

create procedure yaf_user_saveavatar(@UserID int,@AvatarImage image) as
begin
	update yaf_User set AvatarImage=@AvatarImage where UserID = @UserID
end
GO
