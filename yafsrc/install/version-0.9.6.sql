/* Version 0.9.6 */

if exists (select * from sysobjects where id = object_id(N'yaf_Buddy') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table yaf_Buddy
GO

if exists (select * from sysobjects where id = object_id(N'yaf_message_findunread') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_findunread
GO

create procedure yaf_message_findunread(@TopicID int,@LastRead datetime) as
begin
	select top 1 MessageID from yaf_Message
	where TopicID=@TopicID and Posted>@LastRead
	order by Posted
end
go

-- yaf_Board
if not exists (select * from sysobjects where id = object_id(N'yaf_Board') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin
	CREATE TABLE [yaf_Board] (
		[BoardID]		[int]			NOT NULL IDENTITY(1,1),
		[Name]			[nvarchar](50)	NOT NULL,
		[AllowThreaded]	[bit]			NOT NULL,
	)
	EXEC('insert into yaf_Board(Name,AllowThreaded) select Name,0 from yaf_System')
end
GO

if not exists(select * from sysindexes where id=object_id('yaf_Board') and name='PK_Board')
ALTER TABLE [yaf_Board] WITH NOCHECK ADD 
	CONSTRAINT [PK_Board] PRIMARY KEY  CLUSTERED 
	(
		[BoardID]
	)
GO

-- yaf_Category
if not exists(select * from syscolumns where id=object_id('yaf_Category') and name='BoardID')
begin
	alter table yaf_Category add BoardID int null
	EXEC('update yaf_Category set BoardID=(select min(BoardID) from yaf_Board)')
	alter table yaf_Category alter column BoardID int not null
end
GO

if not exists(select * from sysobjects where name='FK_Category_Board' and parent_obj=object_id('yaf_Category') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_Category] ADD 
	CONSTRAINT [FK_Category_Board] FOREIGN KEY 
	(
		[BoardID]
	) REFERENCES [yaf_Board] (
		[BoardID]
	)
GO

if exists(select * from sysindexes where id=object_id('yaf_Category') and name='IX_Category')
	ALTER TABLE [yaf_Category] DROP CONSTRAINT [IX_Category]
GO

if not exists(select * from sysindexes where id=object_id('yaf_Category') and name='IX_Category')
	ALTER TABLE [yaf_Category] ADD CONSTRAINT [IX_Category] UNIQUE NONCLUSTERED([BoardID],[Name])
GO

-- yaf_AccessMask
if not exists(select * from syscolumns where id=object_id('yaf_AccessMask') and name='BoardID')
begin
	alter table yaf_AccessMask add BoardID int null
	EXEC('update yaf_AccessMask set BoardID=(select min(BoardID) from yaf_Board)')
	alter table yaf_AccessMask alter column BoardID int not null
end
GO

if not exists(select * from sysobjects where name='FK_AccessMask_Board' and parent_obj=object_id('yaf_AccessMask') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_AccessMask] ADD 
	CONSTRAINT [FK_AccessMask_Board] FOREIGN KEY 
	(
		[BoardID]
	) REFERENCES [yaf_Board] (
		[BoardID]
	)
GO

-- yaf_Active
if not exists(select * from syscolumns where id=object_id('yaf_Active') and name='BoardID')
begin
	alter table yaf_Active add BoardID int null
	EXEC('update yaf_Active set BoardID=(select min(BoardID) from yaf_Board)')
	alter table yaf_Active alter column BoardID int not null
end
GO

if not exists(select * from sysobjects where name='FK_Active_Board' and parent_obj=object_id('yaf_Active') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_Active] ADD 
	CONSTRAINT [FK_Active_Board] FOREIGN KEY 
	(
		[BoardID]
	) REFERENCES [yaf_Board] (
		[BoardID]
	)
GO

-- yaf_BannedIP
if not exists(select * from syscolumns where id=object_id('yaf_BannedIP') and name='BoardID')
begin
	alter table yaf_BannedIP add BoardID int null
	EXEC('update yaf_BannedIP set BoardID=(select min(BoardID) from yaf_Board)')
	alter table yaf_BannedIP alter column BoardID int not null
end
GO

if not exists(select * from sysobjects where name='FK_BannedIP_Board' and parent_obj=object_id('yaf_BannedIP') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_BannedIP] ADD 
	CONSTRAINT [FK_BannedIP_Board] FOREIGN KEY 
	(
		[BoardID]
	) REFERENCES [yaf_Board] (
		[BoardID]
	)
GO

-- yaf_Group
if not exists(select * from syscolumns where id=object_id('yaf_Group') and name='BoardID')
begin
	alter table yaf_Group add BoardID int null
	EXEC('update yaf_Group set BoardID=(select min(BoardID) from yaf_Board)')
	alter table yaf_Group alter column BoardID int not null
end
GO

if not exists(select * from sysobjects where name='FK_Group_Board' and parent_obj=object_id('yaf_Group') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_Group] ADD 
	CONSTRAINT [FK_Group_Board] FOREIGN KEY 
	(
		[BoardID]
	) REFERENCES [yaf_Board] (
		[BoardID]
	)
GO

-- yaf_NntpServer
if not exists(select * from syscolumns where id=object_id('yaf_NntpServer') and name='BoardID')
begin
	alter table yaf_NntpServer add BoardID int null
	EXEC('update yaf_NntpServer set BoardID=(select min(BoardID) from yaf_Board)')
	alter table yaf_NntpServer alter column BoardID int not null
end
GO

if not exists(select * from sysobjects where name='FK_NntpServer_Board' and parent_obj=object_id('yaf_NntpServer') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_NntpServer] ADD 
	CONSTRAINT [FK_NntpServer_Board] FOREIGN KEY 
	(
		[BoardID]
	) REFERENCES [yaf_Board] (
		[BoardID]
	)
GO

-- yaf_Rank
if not exists(select * from syscolumns where id=object_id('yaf_Rank') and name='BoardID')
begin
	alter table yaf_Rank add BoardID int null
	EXEC('update yaf_Rank set BoardID=(select min(BoardID) from yaf_Board)')
	alter table yaf_Rank alter column BoardID int not null
end
GO

if not exists(select * from sysobjects where name='FK_Rank_Board' and parent_obj=object_id('yaf_Rank') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_Rank] ADD 
	CONSTRAINT [FK_Rank_Board] FOREIGN KEY 
	(
		[BoardID]
	) REFERENCES [yaf_Board] (
		[BoardID]
	)
GO

if exists(select * from sysindexes where id=object_id('yaf_Rank') and name='IX_Rank')
	ALTER TABLE [yaf_Rank] DROP CONSTRAINT [IX_Rank]
GO

if not exists(select * from sysindexes where id=object_id('yaf_Rank') and name='IX_Rank')
	ALTER TABLE [yaf_Rank] ADD CONSTRAINT [IX_Rank] UNIQUE([BoardID],[Name])
GO

-- yaf_Smiley
if not exists(select * from syscolumns where id=object_id('yaf_Smiley') and name='BoardID')
begin
	alter table yaf_Smiley add BoardID int null
	EXEC('update yaf_Smiley set BoardID=(select min(BoardID) from yaf_Board)')
	alter table yaf_Smiley alter column BoardID int not null
end
GO

if not exists(select * from sysobjects where name='FK_Smiley_Board' and parent_obj=object_id('yaf_Smiley') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_Smiley] ADD 
	CONSTRAINT [FK_Smiley_Board] FOREIGN KEY 
	(
		[BoardID]
	) REFERENCES [yaf_Board] (
		[BoardID]
	)
GO

-- yaf_User
if not exists(select * from syscolumns where id=object_id('yaf_User') and name='BoardID')
begin
	alter table yaf_User add BoardID int null
	EXEC('update yaf_User set BoardID=(select min(BoardID) from yaf_Board) where BoardID is null')
	alter table yaf_User alter column BoardID int not null
end
GO

if not exists(select * from syscolumns where id=object_id('yaf_User') and name='IsHostAdmin')
begin
	alter table yaf_User add IsHostAdmin bit null
	EXEC('update yaf_User set IsHostAdmin=(select IsAdmin from yaf_vaccess where yaf_vaccess.UserID=yaf_User.UserID and ForumID=0) where IsHostAdmin is null')
	alter table yaf_User alter column IsHostAdmin bit not null
end
GO

if exists(select * from sysindexes where id=object_id('yaf_User') and name='IX_User')
	ALTER TABLE [yaf_User] DROP CONSTRAINT [IX_User]
GO

if not exists(select * from sysindexes where id=object_id('yaf_User') and name='IX_User')
	ALTER TABLE [yaf_User] ADD CONSTRAINT [IX_User] UNIQUE NONCLUSTERED([BoardID],[Name])
GO

if not exists(select * from sysobjects where name='FK_User_Rank' and parent_obj=object_id('yaf_User') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	ALTER TABLE [yaf_User] ADD 
		CONSTRAINT [FK_User_Rank] FOREIGN KEY([RankID]) REFERENCES [yaf_Rank]([RankID])
GO

if not exists(select * from sysobjects where name='FK_User_Board' and parent_obj=object_id('yaf_User') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	ALTER TABLE [yaf_User] ADD 
		CONSTRAINT [FK_User_Board] FOREIGN KEY([BoardID]) REFERENCES [yaf_Board]([BoardID])
GO

-- yaf_System
if exists(select * from syscolumns where id=object_id('yaf_System') and name='Name')
	alter table yaf_System drop column [Name]
GO

-- yaf_Active
if exists(select * from sysindexes where id=object_id('yaf_Active') and name='PK_Active')
	ALTER TABLE yaf_Active DROP CONSTRAINT PK_Active
GO

if not exists(select * from sysindexes where id=object_id('yaf_Active') and name='PK_Active')
ALTER TABLE [yaf_Active] WITH NOCHECK ADD 
	CONSTRAINT [PK_Active] PRIMARY KEY CLUSTERED([SessionID],[BoardID])
GO

-- yaf_Group
if exists(select * from sysindexes where id=object_id('yaf_Group') and name='IX_Group')
	ALTER TABLE [yaf_Group] DROP CONSTRAINT IX_Group
GO

if not exists(select * from sysindexes where id=object_id('yaf_Group') and name='IX_Group')
ALTER TABLE [yaf_Group] ADD 
	CONSTRAINT [IX_Group] UNIQUE NONCLUSTERED([BoardID],[Name])
GO

-- yaf_BannedIP
if exists(select * from sysindexes where id=object_id('yaf_BannedIP') and name='IX_BannedIP')
	ALTER TABLE [yaf_BannedIP] DROP CONSTRAINT IX_BannedIP
GO

if not exists(select * from sysindexes where id=object_id('yaf_BannedIP') and name='IX_BannedIP')
ALTER TABLE [yaf_BannedIP] ADD 
	CONSTRAINT [IX_BannedIP] UNIQUE NONCLUSTERED([BoardID],[Mask])
GO

-- yaf_Smiley
if exists(select * from sysindexes where id=object_id('yaf_Smiley') and name='IX_Smiley')
	ALTER TABLE [yaf_Smiley] DROP CONSTRAINT [IX_Smiley]
GO

if not exists(select * from sysindexes where id=object_id('yaf_Smiley') and name='IX_Smiley')
	ALTER TABLE [yaf_Smiley] ADD 
		CONSTRAINT [IX_Smiley] UNIQUE NONCLUSTERED([BoardID],[Code])
GO

-- yaf_Forum
if not exists(select * from syscolumns where id=object_id('yaf_Forum') and name='ParentID')
	alter table yaf_Forum add ParentID int null
GO

if not exists(select * from sysobjects where name='FK_Forum_Forum' and parent_obj=object_id('yaf_Forum') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	ALTER TABLE [yaf_Forum] ADD 
		CONSTRAINT FK_Forum_Forum FOREIGN KEY(ParentID)
		REFERENCES yaf_Forum(ForumID)
GO

-- yaf_Message
if not exists(select * from syscolumns where id=object_id('yaf_Message') and name='ReplyTo')
	alter table yaf_Message add ReplyTo int null
GO

if not exists(select * from syscolumns where id=object_id('yaf_Message') and name='Position')
	alter table yaf_Message add [Position] int null
	EXEC('update yaf_Message set [Position]=0 where [Position] is null')
	alter table yaf_Message alter column [Position] int not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_Message') and name='Indent')
	alter table yaf_Message add Indent int null
	EXEC('update yaf_Message set Indent=0 where Indent is null')
	alter table yaf_Message alter column Indent int not null
GO

if not exists(select * from sysobjects where name='FK_Message_Message' and parent_obj=object_id('yaf_Message') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
	ALTER TABLE [yaf_Message] ADD 
		CONSTRAINT FK_Message_Message FOREIGN KEY(ReplyTo)
		REFERENCES yaf_Message(MessageID)
GO

-- yaf_forumlayout
if exists (select * from sysobjects where id = object_id(N'yaf_forumlayout') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forumlayout
GO

-- yaf_board_layout
if exists (select * from sysobjects where id = object_id(N'yaf_board_layout') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_board_layout
GO

-- yaf_category_list
if exists (select * from sysobjects where id = object_id(N'yaf_category_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_category_list
GO

create procedure yaf_category_list(@BoardID int,@CategoryID int=null) as
begin
	if @CategoryID is null
		select * from yaf_Category where BoardID = @BoardID order by SortOrder
	else
		select * from yaf_Category where BoardID = @BoardID and CategoryID = @CategoryID
end
GO

-- yaf_forum_list
if exists (select * from sysobjects where id = object_id(N'yaf_forum_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_list
GO

create procedure yaf_forum_list(@BoardID int,@ForumID int=null) as
begin
	if @ForumID = 0 set @ForumID = null
	if @ForumID is null
		select a.* from yaf_Forum a join yaf_Category b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID order by a.SortOrder
	else
		select a.* from yaf_Forum a join yaf_Category b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID and a.ForumID = @ForumID
end
GO

-- yaf_forum_stats
if exists (select * from sysobjects where id = object_id(N'yaf_forum_stats') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_stats
GO

-- yaf_board_stats
if exists (select * from sysobjects where id = object_id(N'yaf_board_poststats') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_board_poststats
GO

create procedure yaf_board_poststats(@BoardID int) as
begin
	select
		Posts = (select count(1) from yaf_Message a join yaf_Topic b on b.TopicID=a.TopicID join yaf_Forum c on c.ForumID=b.ForumID join yaf_Category d on d.CategoryID=c.CategoryID where d.BoardID=@BoardID),
		Topics = (select count(1) from yaf_Topic a join yaf_Forum b on b.ForumID=a.ForumID join yaf_Category c on c.CategoryID=b.CategoryID where c.BoardID=@BoardID),
		Forums = (select count(1) from yaf_Forum a join yaf_Category b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID),
		Members = (select count(1) from yaf_User a where a.BoardID=@BoardID),
		LastPostInfo.*,
		LastMemberInfo.*
	from
		(
			select top 1 
				LastMemberInfoID= 1,
				LastMemberID	= UserID,
				LastMember	= Name
			from 
				yaf_User 
			where 
				Approved=1 and 
				BoardID=@BoardID 
			order by 
				Joined desc
		) as LastMemberInfo
		left join (
			select top 1 
				LastPostInfoID	= 1,
				LastPost	= a.Posted,
				LastUserID	= a.UserID,
				LastUser	= e.Name
			from 
				yaf_Message a 
				join yaf_Topic b on b.TopicID=a.TopicID 
				join yaf_Forum c on c.ForumID=b.ForumID 
				join yaf_Category d on d.CategoryID=c.CategoryID 
				join yaf_User e on e.UserID=a.UserID
			where 
				d.BoardID=@BoardID
			order by
				a.Posted desc
		) as LastPostInfo
		on LastMemberInfoID=LastPostInfoID
end
GO

-- yaf_accessmask_list
if exists (select * from sysobjects where id = object_id(N'yaf_accessmask_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_accessmask_list
GO

create procedure yaf_accessmask_list(@BoardID int,@AccessMaskID int=null) as
begin
	if @AccessMaskID is null
		select 
			a.* 
		from 
			yaf_AccessMask a 
		where
			a.BoardID = @BoardID
		order by 
			a.Name
	else
		select 
			a.* 
		from 
			yaf_AccessMask a 
		where
			a.BoardID = @BoardID and
			a.AccessMaskID = @AccessMaskID
		order by 
			a.Name
end
GO

-- yaf_active_list
if exists (select * from sysobjects where id = object_id(N'yaf_active_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_active_list
GO

create procedure yaf_active_list(@BoardID int,@Guests bit=0) as
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
			c.UserID = a.UserID and
			c.BoardID = @BoardID
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
			c.BoardID = @BoardID and
			not exists(select 1 from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and y.IsGuest<>0)
		order by
			c.LastActive desc
end
GO

-- yaf_active_stats
if exists (select * from sysobjects where id = object_id(N'yaf_active_stats') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_active_stats
GO

create procedure yaf_active_stats(@BoardID int) as
begin
	select
		ActiveUsers = (select count(1) from yaf_Active where BoardID=@BoardID),
		ActiveMembers = (select count(1) from yaf_Active x where BoardID=@BoardID and exists(select 1 from yaf_UserGroup y,yaf_Group z where y.UserID=x.UserID and y.GroupID=z.GroupID and z.IsGuest=0)),
		ActiveGuests = (select count(1) from yaf_Active x where BoardID=@BoardID and exists(select 1 from yaf_UserGroup y,yaf_Group z where y.UserID=x.UserID and y.GroupID=z.GroupID and z.IsGuest<>0))
end
GO

-- yaf_group_list
if exists (select * from sysobjects where id = object_id(N'yaf_group_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_group_list
GO

create procedure yaf_group_list(@BoardID int,@GroupID int=null) as
begin
	if @GroupID is null
		select * from yaf_Group where BoardID=@BoardID
	else
		select * from yaf_Group where BoardID=@BoardID and GroupID=@GroupID
end
GO

-- yaf_group_member
if exists (select * from sysobjects where id = object_id(N'yaf_group_member') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_group_member
GO

create procedure yaf_group_member(@BoardID int,@UserID int) as
begin
	select 
		a.GroupID,
		a.Name,
		Member = (select count(1) from yaf_UserGroup x where x.UserID=@UserID and x.GroupID=a.GroupID)
	from
		yaf_Group a
	where
		a.BoardID=@BoardID
	order by
		a.Name
end
GO

-- yaf_bannedip_list
if exists (select * from sysobjects where id = object_id(N'yaf_bannedip_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_bannedip_list
GO

create procedure yaf_bannedip_list(@BoardID int,@ID int=null) as
begin
	if @ID is null
		select * from yaf_BannedIP where BoardID=@BoardID
	else
		select * from yaf_BannedIP where BoardID=@BoardID and ID=@ID
end
GO

-- yaf_user_emails
if exists (select * from sysobjects where id = object_id(N'yaf_user_emails') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_emails
GO

create procedure yaf_user_emails(@BoardID int,@GroupID int=null) as
begin
	if @GroupID = 0 set @GroupID = null
	if @GroupID is null
		select 
			a.Email 
		from 
			yaf_User a
		where 
			a.Email is not null and 
			a.BoardID = @BoardID and
			a.Email is not null and 
			a.Email<>''
	else
		select 
			a.Email 
		from 
			yaf_User a join yaf_UserGroup b on b.UserID=a.UserID
		where 
			b.GroupID = @GroupID and 
			a.Email is not null and 
			a.Email<>''
end
GO

-- yaf_smiley_listunique
if exists (select * from sysobjects where id = object_id(N'yaf_smiley_listunique') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_smiley_listunique
GO

create procedure yaf_smiley_listunique(@BoardID int) as
begin
	select 
		Icon, 
		Emoticon,
		Code = (select top 1 Code from yaf_Smiley x where x.Icon=yaf_Smiley.Icon)
	from 
		yaf_Smiley
	where
		BoardID=@BoardID
	group by
		Icon,
		Emoticon
	order by
		Code
end
GO

-- yaf_smiley_list
if exists (select * from sysobjects where id = object_id(N'yaf_smiley_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_smiley_list
GO

create procedure yaf_smiley_list(@BoardID int,@SmileyID int=null) as
begin
	if @SmileyID is null
		select * from yaf_Smiley where BoardID=@BoardID order by LEN(Code) desc
	else
		select * from yaf_Smiley where SmileyID=@SmileyID
end
GO

-- yaf_post_last10user
if exists (select * from sysobjects where id = object_id(N'yaf_post_last10user') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_post_last10user
GO

create procedure yaf_post_last10user(@BoardID int,@UserID int,@PageUserID int) as
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
		yaf_Message a
		join yaf_User b on b.UserID=a.UserID
		join yaf_Topic c on c.TopicID=a.TopicID
		join yaf_Forum d on d.ForumID=c.ForumID
		join yaf_Category e on e.CategoryID=d.CategoryID
		join yaf_vaccess x on x.ForumID=d.ForumID
	where
		a.Approved <> 0 and
		a.UserID = @UserID and
		x.UserID = @PageUserID and
		x.ReadAccess <> 0 and
		e.BoardID = @BoardID
	order by
		a.Posted desc
end
GO

-- yaf_topic_active
if exists (select * from sysobjects where id = object_id(N'yaf_topic_active') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_active
GO

create procedure yaf_topic_active(@BoardID int,@UserID int,@Since datetime) as
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
		yaf_Topic c
		join yaf_User b on b.UserID=c.UserID
		join yaf_Forum d on d.ForumID=c.ForumID
		join yaf_vaccess x on x.ForumID=d.ForumID
		join yaf_Category e on e.CategoryID=d.CategoryID
	where
		@Since < c.LastPosted and
		x.UserID = @UserID and
		x.ReadAccess <> 0 and
		e.BoardID = @BoardID
	order by
		d.Name asc,
		Priority desc,
		LastPosted desc
end
GO

-- yaf_user_accessmasks
if exists (select * from sysobjects where id = object_id(N'yaf_user_accessmasks') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_accessmasks
GO

create procedure yaf_user_accessmasks(@BoardID int,@UserID int) as
begin
	select * from(
		select
			AccessMaskID	= e.AccessMaskID,
			AccessMaskName	= e.Name,
			ForumID			= f.ForumID,
			ForumName		= f.Name
		from
			yaf_User a 
			join yaf_UserGroup b on b.UserID=a.UserID
			join yaf_Group c on c.GroupID=b.GroupID
			join yaf_ForumAccess d on d.GroupID=c.GroupID
			join yaf_AccessMask e on e.AccessMaskID=d.AccessMaskID
			join yaf_Forum f on f.ForumID=d.ForumID
		where
			a.UserID=@UserID and
			c.BoardID=@BoardID
		group by
			e.AccessMaskID,
			e.Name,
			f.ForumID,
			f.Name
		
		union
			
		select
			AccessMaskID	= c.AccessMaskID,
			AccessMaskName	= c.Name,
			ForumID			= d.ForumID,
			ForumName		= d.Name
		from
			yaf_User a 
			join yaf_UserForum b on b.UserID=a.UserID
			join yaf_AccessMask c on c.AccessMaskID=b.AccessMaskID
			join yaf_Forum d on d.ForumID=b.ForumID
		where
			a.UserID=@UserID and
			c.BoardID=@BoardID
		group by
			c.AccessMaskID,
			c.Name,
			d.ForumID,
			d.Name
	) as x
	order by
		ForumName, AccessMaskName
end
GO

-- yaf_usergroup_list
if exists (select * from sysobjects where id = object_id(N'yaf_usergroup_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_usergroup_list
GO

create procedure yaf_usergroup_list(@BoardID int,@UserID int) as begin
	select 
		b.GroupID,
		b.Name
	from
		yaf_UserGroup a
		join yaf_Group b on b.GroupID=a.GroupID
	where
		a.UserID = @UserID and
		b.BoardID = @BoardID
	order by
		b.Name
end
GO

-- yaf_forum_listpath
if exists (select * from sysobjects where id = object_id(N'yaf_forum_listpath') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_listpath
GO

create procedure yaf_forum_listpath(@ForumID int) as
begin
	select
		a.ForumID,
		a.Name
	from
		(select
			a.ForumID,
			Indent = 0
		from
			yaf_Forum a
		where
			a.ForumID=@ForumID

		union

		select
			b.ForumID,
			Indent = 1
		from
			yaf_Forum a
			join yaf_Forum b on b.ForumID=a.ParentID
		where
			a.ForumID=@ForumID

		union

		select
			c.ForumID,
			Indent = 2
		from
			yaf_Forum a
			join yaf_Forum b on b.ForumID=a.ParentID
			join yaf_Forum c on c.ForumID=b.ParentID
		where
			a.ForumID=@ForumID
		) as x	
		join yaf_Forum a on a.ForumID=x.ForumID
	order by
		x.Indent desc
end
GO

-- yaf_category_listread
if exists (select * from sysobjects where id = object_id(N'yaf_category_listread') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_category_listread
GO

create procedure yaf_category_listread(@BoardID int,@UserID int,@CategoryID int=null) as
begin
	select 
		a.CategoryID,
		a.Name
	from 
		yaf_Category a
		join yaf_Forum b on b.CategoryID=a.CategoryID
		join yaf_vaccess v on v.ForumID=b.ForumID
	where
		a.BoardID=@BoardID and
		v.UserID=@UserID and
		(v.ReadAccess<>0 or b.Hidden=0) and
		(@CategoryID is null or a.CategoryID=@CategoryID) and
		b.ParentID is null
	group by
		a.CategoryID,
		a.Name,
		a.SortOrder
	order by 
		a.SortOrder
end
GO

-- yaf_rank_list
if exists (select * from sysobjects where id = object_id(N'yaf_rank_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_rank_list
GO

create procedure yaf_rank_list(@BoardID int,@RankID int=null) as begin
	if @RankID is null
		select
			a.*
		from
			yaf_Rank a
		where
			a.BoardID=@BoardID
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

-- yaf_topic_info
if exists (select * from sysobjects where id = object_id(N'yaf_topic_info') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_info
GO

create procedure yaf_topic_info(@TopicID int=null) as
begin
	if @TopicID = 0 set @TopicID = null
	if @TopicID is null
		select * from yaf_Topic
	else
		select * from yaf_Topic where TopicID = @TopicID
end
GO
