/* Version 8.0.1 */

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='ShowMoved')
	alter table yaf_System add ShowMoved bit not null default(1)
GO

if exists (select * from sysobjects where id = object_id(N'yaf_system_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_system_save
GO

create procedure yaf_system_save(
	@Name				varchar(50),
	@TimeZone			int,
	@SmtpServer			varchar(50),
	@ForumEmail			varchar(50),
	@EmailVerification	bit,
	@ShowMoved			bit
) as
begin
	update yaf_System set
		Name = @Name,
		TimeZone = @TimeZone,
		SmtpServer = @SmtpServer,
		ForumEmail = @ForumEmail,
		EmailVerification = @EmailVerification,
		ShowMoved = @ShowMoved
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_poll_stats') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_poll_stats
GO

create procedure yaf_poll_stats(@PollID int) as
begin
	select
		a.PollID,
		b.Question,
		a.ChoiceID,
		a.Choice,
		a.Votes,
		Stats = (select 100 * a.Votes / case sum(x.Votes) when 0 then 1 else sum(x.Votes) end from yaf_Choice x where x.PollID=a.PollID)
	from
		yaf_Choice a,
		yaf_Poll b
	where
		b.PollID = a.PollID and
		b.PollID = @PollID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_find') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_find
GO

create procedure yaf_user_find(@UserName varchar(50),@Email varchar(50)) as
begin
	select UserID from yaf_User where Name=@UserName or Email=@Email
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_forum_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_delete
GO

create procedure yaf_forum_delete(@ForumID int) as
begin
	-- Maybe an idea to use cascading foreign keys instead? Too bad they don't work on MS SQL 7.0...
	update yaf_Forum set LastMessageID=null,LastTopicID=null where ForumID=@ForumID
	update yaf_Topic set LastMessageID=null where ForumID=@ForumID
	delete from yaf_WatchTopic from yaf_Topic where yaf_Topic.ForumID = @ForumID and yaf_WatchTopic.TopicID = yaf_Topic.TopicID
	
	delete from yaf_WatchForum where ForumID = @ForumID
	delete from yaf_Message from yaf_Topic where yaf_Topic.ForumID = @ForumID and yaf_Message.TopicID = yaf_Topic.TopicID
	delete from yaf_Topic where ForumID = @ForumID
	delete from yaf_ForumAccess where ForumID = @ForumID
	delete from yaf_Forum where ForumID = @ForumID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_poll_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_poll_save
GO

create procedure yaf_poll_save(
	@Question	varchar(50),
	@Choice1	varchar(50),
	@Choice2	varchar(50),
	@Choice3	varchar(50) = null,
	@Choice4	varchar(50) = null,
	@Choice5	varchar(50) = null,
	@Choice6	varchar(50) = null,
	@Choice7	varchar(50) = null,
	@Choice8	varchar(50) = null,
	@Choice9	varchar(50) = null
) as
begin
	declare @PollID	int
	insert into yaf_Poll(Question) values(@Question)
	set @PollID = @@IDENTITY
	if @Choice1<>'' and @Choice1 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice1,0)
	if @Choice2<>'' and @Choice2 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice2,0)
	if @Choice3<>'' and @Choice3 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice3,0)
	if @Choice4<>'' and @Choice4 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice4,0)
	if @Choice5<>'' and @Choice5 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice5,0)
	if @Choice6<>'' and @Choice6 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice6,0)
	if @Choice7<>'' and @Choice7 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice7,0)
	if @Choice8<>'' and @Choice8 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice8,0)
	if @Choice9<>'' and @Choice9 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice9,0)
	select PollID = @PollID
end
GO

if not exists(select * from sysindexes where id=object_id('yaf_Smiley') and name='IX_Smiley')
ALTER TABLE [yaf_Smiley] ADD 
	CONSTRAINT [IX_Smiley] UNIQUE  NONCLUSTERED 
	(
		[Code]
	)  ON [PRIMARY] 
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
		yaf_Group c,
		yaf_Topic d
	where
		a.TopicID = @TopicID and
		b.UserID = a.UserID and
		c.GroupID = b.GroupID and
		d.TopicID = a.TopicID
	order by
		a.Posted desc
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_smiley_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_smiley_save
GO

create procedure yaf_smiley_save(@SmileyID int=null,@Code varchar(10),@Icon varchar(50),@Emoticon varchar(50),@Replace smallint=0) as begin
	if @SmileyID is not null begin
		update yaf_Smiley set Code = @Code, Icon = @Icon, Emoticon = @Emoticon where SmileyID = @SmileyID
	end
	else begin
		if @Replace>0
			delete from yaf_Smiley where Code=@Code

		if not exists(select 1 from yaf_Smiley where Code=@Code)
			insert into yaf_Smiley(Code,Icon,Emoticon) values(@Code,@Icon,@Emoticon)
	end
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_smiley_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_smiley_delete
GO

create procedure yaf_smiley_delete(@SmileyID int=null) as begin
	if @SmileyID is not null
		delete from yaf_Smiley where SmileyID=@SmileyID
	else
		delete from yaf_Smiley
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_smiley_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_smiley_list
GO

create procedure yaf_smiley_list(@SmileyID int=null) as
begin
	if @SmileyID is null
		select * from yaf_Smiley order by LEN(Code) desc
	else
		select * from yaf_Smiley where SmileyID=@SmileyID
end
GO

if not exists(select * from syscolumns where id=object_id('yaf_Group') and name='RankImage')
	alter table yaf_Group add RankImage varchar(50) null
GO

if exists (select * from sysobjects where id = object_id(N'yaf_group_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_group_save
GO

create procedure yaf_group_save(
	@GroupID	int,
	@Name		varchar(50),
	@IsAdmin	bit,
	@IsGuest	bit,
	@IsStart	bit,
	@IsLadder	bit,
	@MinPosts	int,
	@RankImage	varchar(50)=null
) as
begin
	if @IsAdmin = 1 update yaf_Group set IsAdmin = 0
	if @IsGuest = 1 update yaf_Group set IsGuest = 0
	if @IsStart = 1 update yaf_Group set IsStart = 0
	if @IsLadder=0 set @MinPosts = null
	if @IsLadder=1 and @MinPosts is null set @MinPosts = 0
	if @GroupID>0 begin
		update yaf_Group set
			Name = @Name,
			IsAdmin = @IsAdmin,
			IsGuest = @IsGuest,
			IsStart = @IsStart,
			IsLadder = @IsLadder,
			MinPosts = @MinPosts,
			RankImage = @RankImage
		where GroupID = @GroupID
	end
	else begin
		insert into yaf_Group(Name,IsAdmin,IsGuest,IsStart,IsLadder,MinPosts,RankImage)
		values(@Name,@IsAdmin,@IsGuest,@IsStart,@IsLadder,@MinPosts,@RankImage);
		set @GroupID = @@IDENTITY
		insert into yaf_ForumAccess(GroupID,ForumID,ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess)
		select @GroupID,ForumID,0,0,0,0,0,0,0,0,0 from yaf_Forum
	end
	select GroupID = @GroupID
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
		GroupName	= c.Name,
		d.Views,
		d.ForumID,
		Avatar = b.Avatar,
		b.Location,
		b.HomePage,
		b.Signature,
		c.RankImage
	from
		yaf_Message a, 
		yaf_User b,
		yaf_Group c,
		yaf_Topic d
	where
		a.TopicID = @TopicID and
		b.UserID = a.UserID and
		c.GroupID = b.GroupID and
		d.TopicID = a.TopicID
	order by
		a.Posted asc
end
GO
