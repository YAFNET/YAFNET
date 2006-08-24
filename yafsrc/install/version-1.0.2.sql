/* Version 1.0.2 */

/*
** Added or Modified columns
*/

if not exists(select 1 from dbo.syscolumns where id = object_id(N'yaf_Forum') and name=N'ThemeURL')
begin
	alter table yaf_Forum add ThemeURL nvarchar(50) NULL
end
GO

if not exists(select 1 from dbo.syscolumns where id = object_id(N'yaf_EventLog') and name=N'Type')
begin
	alter table yaf_EventLog add Type int not null constraint DF_EventLog_Type default (0)
	exec('update yaf_EventLog set Type = 0')
end
GO

if exists(select 1 from dbo.syscolumns where id = object_id(N'yaf_EventLog') and name=N'UserID' and isnullable=0)
	alter table yaf_EventLog alter column UserID int null
GO

if not exists(select 1 from syscolumns where id=object_id('yaf_User') and name='PMNotification')
begin
	alter table dbo.yaf_User add PMNotification bit NOT NULL CONSTRAINT DF_yaf_User_PMNotification  DEFAULT (1)
end
GO


-- yaf_eventlog_create
if exists (select 1 from sysobjects where id = object_id(N'yaf_eventlog_create') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_eventlog_create
GO

create procedure dbo.yaf_eventlog_create(@UserID int,@Source nvarchar(50),@Description ntext,@Type int) as
begin
	insert into dbo.yaf_EventLog(UserID,Source,Description,Type)
	values(@UserID,@Source,@Description,@Type)

	-- delete entries older than 10 days
	delete from dbo.yaf_EventLog where EventTime+10<getdate()

	-- or if there are more then 1000	
	if ((select count(*) from yaf_eventlog) >= 1050)
	begin
		/* delete oldest hundred */
		delete from dbo.yaf_EventLog WHERE EventLogID IN (SELECT TOP 100 EventLogID FROM dbo.yaf_EventLog ORDER BY EventTime)
	end	
	
end
GO

-- yaf_eventlog_list
if exists (select 1 from sysobjects where id = object_id(N'yaf_eventlog_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_eventlog_list
GO

create procedure dbo.yaf_eventlog_list(@BoardID int) as
begin
	select
		a.*,
		ISNULL(b.[Name],'System') as [Name]
	from
		dbo.yaf_EventLog a
		left join dbo.yaf_User b on b.UserID=a.UserID
	where
		(b.UserID IS NULL or b.BoardID = @BoardID)		
	order by
		a.EventLogID desc
end
GO

-- yaf_forum_listonlycat
if exists (select 1 from sysobjects where id = object_id(N'yaf_forum_listall_fromcat') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_listall_fromcat
GO

-- yaf_forum_listonlycat

CREATE PROCEDURE dbo.yaf_forum_listall_fromcat
(
@BoardID int,
@CategoryID int
)
AS
SELECT     b.CategoryID, b.Name AS Category, a.ForumID, a.Name AS Forum, a.ParentID
FROM         yaf_Forum a INNER JOIN
                      yaf_Category b ON b.CategoryID = a.CategoryID
    WHERE
        b.CategoryID=@CategoryID and
        b.BoardID=@BoardID
    ORDER BY
        b.SortOrder,
        a.SortOrder
GO

-- yaf_forum_listall
if exists (select 1 from sysobjects where id = object_id(N'yaf_forum_listall') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_listall
GO

CREATE procedure dbo.yaf_forum_listall(@BoardID int,@UserID int) as
BEGIN
    SELECT
        b.CategoryID,
        Category = b.Name,
        a.ForumID,
        Forum = a.Name,
        a.ParentID
    FROM
        yaf_Forum a
        JOIN yaf_Category b ON b.CategoryID=a.CategoryID
        JOIN yaf_vaccess c ON c.ForumID=a.ForumID
    WHERE
        c.UserID=@UserID AND
        b.BoardID=@BoardID AND
        c.ReadAccess>0
    ORDER BY
        b.SortOrder,
        a.SortOrder
END

GO


-- yaf_forum_save
if exists (select 1 from sysobjects where id = object_id(N'yaf_forum_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_save
GO

create procedure dbo.yaf_forum_save(
	@ForumID 		int,
	@CategoryID		int,
	@ParentID		int=null,
	@Name			nvarchar(50),
	@Description	nvarchar(255),
	@SortOrder		smallint,
	@Locked			bit,
	@Hidden			bit,
	@IsTest			bit,
	@Moderated		bit,
	@RemoteURL		nvarchar(100)=null,
	@ThemeURL		nvarchar(100)=null,
	@AccessMaskID	int = null
) as
begin
	declare @BoardID	int
	declare @Flags		int
	
	set @Flags = 0
	if @Locked<>0 set @Flags = @Flags | 1
	if @Hidden<>0 set @Flags = @Flags | 2
	if @IsTest<>0 set @Flags = @Flags | 4
	if @Moderated<>0 set @Flags = @Flags | 8

	if @ForumID>0 begin
		update yaf_Forum set 
			ParentID=@ParentID,
			Name=@Name,
			Description=@Description,
			SortOrder=@SortOrder,
			CategoryID=@CategoryID,
			RemoteURL = @RemoteURL,
			ThemeURL = @ThemeURL,
			Flags = @Flags
		where ForumID=@ForumID
	end
	else begin
		select @BoardID=BoardID from yaf_Category where CategoryID=@CategoryID
	
		insert into yaf_Forum(ParentID,Name,Description,SortOrder,CategoryID,NumTopics,NumPosts,RemoteURL,ThemeURL,Flags)
		values(@ParentID,@Name,@Description,@SortOrder,@CategoryID,0,0,@RemoteURL,@ThemeURL,@Flags)
		select @ForumID = @@IDENTITY

		insert into yaf_ForumAccess(GroupID,ForumID,AccessMaskID) 
		select GroupID,@ForumID,@AccessMaskID
		from yaf_Group 
		where BoardID=@BoardID
	end
	select ForumID = @ForumID
end
GO

-- yaf_topic_move
if exists (select 1 from sysobjects where id = object_id(N'yaf_topic_move') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_move
GO

CREATE procedure dbo.yaf_topic_move
(@TopicID int,@ForumID int,@ShowMoved bit) as
begin
    declare @OldForumID int

    select @OldForumID = ForumID from yaf_Topic where TopicID = @TopicID

    if @ShowMoved<>0 begin
        -- create a moved message
        insert into yaf_Topic(ForumID,UserID,UserName,Posted,Topic,Views,Flags,Priority,PollID,TopicMovedID,LastPosted,NumPosts)
        select ForumID,UserID,UserName,Posted,Topic,0,Flags,Priority,PollID,@TopicID,LastPosted,0
        from yaf_Topic where TopicID = @TopicID
    end

    -- move the topic
    update yaf_Topic set ForumID = @ForumID where TopicID = @TopicID

    -- update last posts
    exec yaf_forum_updatelastpost @OldForumID
    exec yaf_forum_updatelastpost @ForumID
end
GO

-- yaf_topic_updatelastpost
if exists (select 1 from sysobjects where id = object_id(N'yaf_topic_updatelastpost') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_updatelastpost
GO

CREATE procedure dbo.yaf_topic_updatelastpost
(@ForumID int=null,@TopicID int=null) as
begin
    if @TopicID is not null
        update yaf_Topic set
            LastPosted = (select top 1 x.Posted from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastMessageID = (select top 1 x.MessageID from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserID = (select top 1 x.UserID from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserName = (select top 1 x.UserName from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc)
        where TopicID = @TopicID
    else
        update yaf_Topic set
            LastPosted = (select top 1 x.Posted from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastMessageID = (select top 1 x.MessageID from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserID = (select top 1 x.UserID from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserName = (select top 1 x.UserName from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc)
        where TopicMovedID is null
        and (@ForumID is null or ForumID=@ForumID)

    exec yaf_forum_updatelastpost @ForumID
end
GO

-- yaf_forum_updatelastpost
if exists (select 1 from sysobjects where id = object_id(N'yaf_forum_updatelastpost') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_updatelastpost
GO

CREATE procedure dbo.yaf_forum_updatelastpost(@ForumID int) as
begin
    update yaf_Forum set
        LastPosted = (select top 1 y.Posted from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16 order by y.Posted desc),
        LastTopicID = (select top 1 y.TopicID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16 order by y.Posted desc),
        LastMessageID = (select top 1 y.MessageID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16 order by y.Posted desc),
        LastUserID = (select top 1 y.UserID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16 order by y.Posted desc),
        LastUserName = (select top 1 y.UserName from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16 order by y.Posted desc)
    where ForumID = @ForumID
end
GO
-- yaf_pageload
if exists (select 1 from sysobjects where id = object_id(N'yaf_pageload') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pageload
GO

create procedure dbo.yaf_pageload(
	@SessionID	nvarchar(24),
	@BoardID	int,
	@User		nvarchar(50),
	@IP			nvarchar(15),
	@Location	nvarchar(50),
	@Browser	nvarchar(50),
	@Platform	nvarchar(50),
	@CategoryID	int = null,
	@ForumID	int = null,
	@TopicID	int = null,
	@MessageID	int = null
) as
begin
	declare @UserID			int
	declare @UserBoardID	int
	declare @IsGuest		tinyint
	declare @rowcount		int
	declare @PreviousVisit	datetime
	
	set implicit_transactions off

	if @User is null or @User='' 
	begin
		select @UserID = a.UserID from yaf_User a,yaf_UserGroup b,yaf_Group c where a.UserID=b.UserID and a.BoardID=@BoardID and b.GroupID=c.GroupID and (c.Flags & 2)<>0
		set @rowcount=@@rowcount
		if @rowcount<>1
		begin
			raiserror('Found %d possible guest users. Only 1 guest user should be a member of the group marked as the guest group.',16,1,@rowcount)
		end
		set @IsGuest = 1
		set @UserBoardID = @BoardID
	end else
	begin
		select @UserID = UserID, @UserBoardID = BoardID from yaf_User where BoardID=@BoardID and Name=@User
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
	
	-- get previous visit
	if @IsGuest=0 begin
		select @PreviousVisit = LastVisit from dbo.yaf_User where UserID = @UserID
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
			yaf_Forum c,
			yaf_Category d
		where
			a.MessageID = @MessageID and
			b.TopicID = a.TopicID and
			c.ForumID = b.ForumID and
			d.CategoryID = c.CategoryID and
			d.BoardID = @BoardID
	end
	else if @TopicID is not null begin
		select 
			@CategoryID = b.CategoryID,
			@ForumID = a.ForumID 
		from 
			yaf_Topic a,
			yaf_Forum b,
			yaf_Category c
		where 
			a.TopicID = @TopicID and
			b.ForumID = a.ForumID and
			c.CategoryID = b.CategoryID and
			c.BoardID = @BoardID
	end
	else if @ForumID is not null begin
		select
			@CategoryID = a.CategoryID
		from
			yaf_Forum a,
			yaf_Category b
		where
			a.ForumID = @ForumID and
			b.CategoryID = a.CategoryID and
			b.BoardID = @BoardID
	end
	-- update active
	if @UserID is not null and @UserBoardID=@BoardID begin
		if exists(select 1 from yaf_Active where SessionID=@SessionID and BoardID=@BoardID)
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
			insert into yaf_Active(SessionID,BoardID,UserID,IP,Login,LastActive,Location,ForumID,TopicID,Browser,Platform)
			values(@SessionID,@BoardID,@UserID,@IP,getdate(),getdate(),@Location,@ForumID,@TopicID,@Browser,@Platform)
		end
		-- remove duplicate users
		if @IsGuest=0
			delete from yaf_Active where UserID=@UserID and BoardID=@BoardID and SessionID<>@SessionID
	end
	-- return information
	select
		a.UserID,
		UserFlags			= a.Flags,
		UserName			= a.Name,
		Suspended			= a.Suspended,
		ThemeFile			= a.ThemeFile,
		LanguageFile		= a.LanguageFile,
		TimeZoneUser		= a.TimeZone,
		PreviousVisit		= @PreviousVisit,
		x.*,
		CategoryID			= @CategoryID,
		CategoryName		= (select Name from yaf_Category where CategoryID = @CategoryID),
		ForumID				= @ForumID,
		ForumName			= (select Name from yaf_Forum where ForumID = @ForumID),
		TopicID				= @TopicID,
		TopicName			= (select Topic from yaf_Topic where TopicID = @TopicID),
		MailsPending		= (select count(1) from yaf_Mail),
		Incoming			= (select count(1) from yaf_UserPMessage where UserID=a.UserID and IsRead=0),
		ForumTheme			= (select ThemeURL from yaf_Forum where ForumID = @ForumID)
	from
		yaf_User a,
		yaf_vaccess x
	where
		a.UserID = @UserID and
		x.UserID = a.UserID and
		x.ForumID = IsNull(@ForumID,0)
end
GO

-- yaf_topic_delete
if exists (select 1 from sysobjects where id = object_id(N'yaf_topic_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_delete
GO

create procedure dbo.yaf_topic_delete (@TopicID int,@UpdateLastPost bit=1) 
as
begin
	SET NOCOUNT ON

	declare @ForumID int
	declare @pollID int
	
	select @ForumID=ForumID from yaf_Topic where TopicID=@TopicID

	update yaf_Topic set LastMessageID = null where TopicID = @TopicID
	update yaf_Forum set 
		LastTopicID = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null,
		LastPosted = null
	where LastMessageID in (select MessageID from yaf_Message where TopicID = @TopicID)
	update yaf_Active set TopicID = null where TopicID = @TopicID
	
	--remove polls	
	select @pollID = pollID from yaf_topic where TopicID = @TopicID
	if (@pollID is not null)
	begin
		delete from yaf_choice where PollID = @PollID
		update yaf_topic set PollID = null where TopicID = @TopicID
		delete from yaf_poll where PollID = @PollID	
	end	
	
	--delete messages and topics
	delete from yaf_nntptopic where TopicID = @TopicID
	delete from yaf_topic where TopicMovedID = @TopicID
		
	update yaf_topic set Flags = Flags | 8 where TopicID = @TopicID
	
	--delete from yaf_message where TopicID = @TopicID
	--delete from yaf_topic where TopicID = @TopicID
		
	--commit
	if @UpdateLastPost<>0
		exec yaf_forum_updatelastpost @ForumID
	
	if @ForumID is not null
		exec yaf_forum_updatestats @ForumID
end
GO

-- yaf_message_list
if exists (select 1 from sysobjects where id = object_id(N'yaf_message_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_list
GO

CREATE PROCEDURE dbo.yaf_message_list(@MessageID int) AS
BEGIN
	SELECT
		a.MessageID,
		a.UserID,
		UserName = b.Name,
		a.Message,
		c.TopicID,
		c.ForumID,
		c.Topic,
		c.Priority,
		a.Flags,
		c.UserID AS TopicOwnerID,
		Edited = IsNull(a.Edited,a.Posted)
	FROM
		yaf_Message a,
		yaf_User b,
		yaf_Topic c
	WHERE
		a.MessageID = @MessageID AND
		b.UserID = a.UserID AND
		c.TopicID = a.TopicID
END
GO

-- yaf_topic_info
IF EXISTS (SELECT 1 FROM sysobjects WHERE id = object_id(N'yaf_topic_info') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_info
GO

CREATE PROCEDURE [dbo].[yaf_topic_info]
(
	@TopicID int = null,
	@ShowDeleted bit = 0
)
AS
BEGIN
	IF @TopicID = 0 SET @TopicID = NULL

	IF @TopicID IS NULL
	BEGIN
		IF @ShowDeleted = 1 
			SELECT * FROM yaf_Topic
		ELSE
			SELECT * FROM yaf_Topic WHERE (Flags & 8) = 0
	END
	ELSE
	BEGIN
		IF @ShowDeleted = 1 
			SELECT * FROM yaf_Topic WHERE TopicID = @TopicID
		ELSE
			SELECT * FROM yaf_Topic WHERE TopicID = @TopicID AND (Flags & 8) = 0		
	END
END
GO

if exists (select 1 from sysobjects where id = object_id(N'yaf_topic_findnext') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_findnext
GO

create procedure dbo.yaf_topic_findnext(@TopicID int) as
begin
	declare @LastPosted datetime
	declare @ForumID int
	select @LastPosted = LastPosted, @ForumID = ForumID from yaf_Topic where TopicID = @TopicID
	select top 1 TopicID from yaf_Topic where LastPosted>@LastPosted and ForumID = @ForumID AND (Flags & 8) = 0 order by LastPosted asc
end
GO

if exists (select 1 from sysobjects where id = object_id(N'yaf_topic_findprev') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_findprev
GO

create procedure dbo.yaf_topic_findprev(@TopicID int) AS 
BEGIN
	DECLARE @LastPosted datetime
	DECLARE @ForumID int
	SELECT @LastPosted = LastPosted, @ForumID = ForumID FROM yaf_Topic WHERE TopicID = @TopicID
	SELECT TOP 1 TopicID from yaf_Topic where LastPosted<@LastPosted AND ForumID = @ForumID AND (Flags & 8) = 0 ORDER BY LastPosted DESC
END
GO

-- yaf_user_save
if exists (select 1 from sysobjects where id = object_id(N'yaf_user_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_save
GO

CREATE procedure [dbo].[yaf_user_save](
	@UserID			int,
	@BoardID		int,
	@UserName		nvarchar(50) = null,
	@Password		nvarchar(32) = null,
	@Email			nvarchar(50) = null,
	@Hash			nvarchar(32) = null,
	@Location		nvarchar(50) = null,
	@HomePage		nvarchar(50) = null,
	@TimeZone		int,
	@Avatar			nvarchar(255) = null,
	@LanguageFile	nvarchar(50) = null,
	@ThemeFile		nvarchar(50) = null,
	@Approved		bit = null,
	@MSN			nvarchar(50) = null,
	@YIM			nvarchar(30) = null,
	@AIM			nvarchar(30) = null,
	@ICQ			int = null,
	@RealName		nvarchar(50) = null,
	@Occupation		nvarchar(50) = null,
	@Interests		nvarchar(100) = null,
	@Gender			tinyint = 0,
	@Weblog			nvarchar(100) = null,
	@PMNotification bit = null
) as
begin
	declare @RankID int
	declare @Flags int
	
	set @Flags = 0
	if @Approved<>0 set @Flags = @Flags | 2
	
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
	if @Weblog is not null and @Weblog = '' set @Weblog = null
	if @PMNotification is null SET @PMNotification = 1

	if @UserID is null or @UserID<1 begin
		if @Email = '' set @Email = null
		
		select @RankID = RankID from yaf_Rank where (Flags & 1)<>0 and BoardID=@BoardID
		
		insert into yaf_User(BoardID,RankID,Name,Password,Email,Joined,LastVisit,NumPosts,Location,HomePage,TimeZone,Avatar,Gender,Flags,PMNotification) 
		values(@BoardID,@RankID,@UserName,@Password,@Email,getdate(),getdate(),0,@Location,@HomePage,@TimeZone,@Avatar,@Gender,@Flags,@PMNotification)
	
		set @UserID = @@IDENTITY

		insert into yaf_UserGroup(UserID,GroupID) select @UserID,GroupID from yaf_Group where BoardID=@BoardID and (Flags & 4)<>0
		
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
			Weblog = @Weblog,
			PMNotification = @PMNotification
		where UserID = @UserID
		
		if @Email is not null
			update yaf_User set Email = @Email where UserID = @UserID
	end
end
