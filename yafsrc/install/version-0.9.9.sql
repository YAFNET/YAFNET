/* Version 0.9.9 */

if not exists (select * from dbo.sysobjects where id = object_id(N'yaf_Replace_Words') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE yaf_Replace_Words(
		[id] [int] IDENTITY (1, 1) NOT NULL ,
		[badword] [nvarchar] (50) NULL ,
		[goodword] [nvarchar] (50) NULL ,
		constraint PK_Replace_Words primary key(id)
	)
GO

if exists(select 1 from dbo.syscolumns where id = object_id(N'yaf_User') and name=N'Signature' and xtype<>99)
	alter table yaf_User alter column Signature ntext null
go

if not exists(select * from syscolumns where id=object_id('yaf_Forum') and name='RemoteURL')
	alter table yaf_Forum add RemoteURL nvarchar(100) null
GO

if exists (select * from sysobjects where id = object_id(N'yaf_replace_words_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_replace_words_delete
GO

create procedure yaf_replace_words_delete(@ID int) as
begin
	delete from yaf_replace_words where ID = @ID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_replace_words_edit') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_replace_words_edit
GO

create procedure yaf_replace_words_edit(@ID int=null) as
begin
	select * from yaf_replace_words where ID=@ID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_replace_words_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_replace_words_list
GO

CREATE procedure yaf_replace_words_list as begin
	select * from yaf_Replace_Words
end
GO

if exists (select * from dbo.sysobjects where id = object_id(N'yaf_replace_words_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_replace_words_save
GO

create procedure yaf_replace_words_save(@ID int=null,@badword nvarchar(30),@goodword nvarchar(30)) as
begin
	if @ID is null or @ID = 0 begin
		insert into yaf_replace_words(badword,goodword) values(@badword,@goodword)
	end
	else begin
		update yaf_replace_words set badword = @badword,goodword = @goodword where ID = @ID
	end
end
GO

/* subject editing added by Jaben Cargman */

-- yaf_message_update
if exists (select * from dbo.sysobjects where id = object_id(N'yaf_message_update') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_update
GO

CREATE procedure yaf_message_update(@MessageID int,@Priority int,@Subject nvarchar(100),@Message ntext) as
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

	if not @Subject = '' and @Subject is not null begin
		update yaf_Topic set
			Topic = @Subject
		where
			TopicID = @TopicID
	end 
	
	-- If forum is moderated, make sure last post pointers are correct
	if @Moderated<>0 exec yaf_topic_updatelastpost
end
GO

-- yaf_message_list
if exists (select * from sysobjects where id = object_id(N'yaf_message_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_list
GO

CREATE procedure yaf_message_list(@MessageID int) as
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
		a.Approved,
		c.UserID as TopicOwnerID
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

-- registry implementation by Jaben Cargman

if not exists (select * from sysobjects where id = object_id(N'[yaf_Registry]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	CREATE TABLE [yaf_Registry] (
	[RegistryID] int IDENTITY(1, 1) NOT NULL,
	[Name] nvarchar(50) NOT NULL,
	[Value] nvarchar(400),
	CONSTRAINT [PK_Registry] PRIMARY KEY ([RegistryID])
	)
	ON [PRIMARY]
	
	CREATE UNIQUE INDEX [IX_Name] ON [yaf_Registry]
	([Name])
	ON [PRIMARY]
END	
GO

-- yaf_registry_list
if exists (select * from sysobjects where id = object_id(N'yaf_registry_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_registry_list
GO

CREATE PROCEDURE yaf_registry_list(@Name nvarchar(50) = null) as
BEGIN
	IF @Name IS NULL OR @Name = ''
	BEGIN
		SELECT * FROM yaf_Registry
	END ELSE
	BEGIN
		SELECT * FROM yaf_Registry WHERE LOWER([Name]) = LOWER(@Name)
	END
END
GO

-- yaf_registry_save
if exists (select * from sysobjects where id = object_id(N'yaf_registry_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_registry_save
GO

CREATE PROCEDURE yaf_registry_save(
	@Name nvarchar(50),
	@Value nvarchar(400) = NULL
) AS
BEGIN
	DECLARE @RegistryID as int
	
	SET @RegistryID = (SELECT RegistryID FROM yaf_Registry WHERE LOWER([Name]) = LOWER(@Name))

	IF @RegistryID IS NULL 
	BEGIN
		-- Create new record
		INSERT INTO yaf_Registry([Name],Value) VALUES(LOWER(@Name),@Value)
	END ELSE
	BEGIN
		-- Update existing record
		UPDATE yaf_Registry SET Value = @Value WHERE LOWER([Name]) = LOWER(@Name)
	END
END
GO

-- yaf_system_initialize
if exists (select * from sysobjects where id = object_id(N'yaf_system_initialize') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_system_initialize
GO

CREATE procedure yaf_system_initialize(
	@Name		nvarchar(50),
	@TimeZone	int,
	@ForumEmail	nvarchar(50),
	@SmtpServer	nvarchar(50),
	@User		nvarchar(50),
	@UserEmail	nvarchar(50),
	@Password	nvarchar(32)
) as 
begin
	DECLARE @tmpValue AS nvarchar(100)

	-- initalize required 'registry' settings
	EXEC yaf_registry_save 'Version','1'
	EXEC yaf_registry_save 'VersionName','0.9.9'
	SET @tmpValue = CAST(@TimeZone AS nvarchar(100))
	EXEC yaf_registry_save 'TimeZone', @tmpValue
	EXEC yaf_registry_save 'SmtpServer', @SmtpServer
	EXEC yaf_registry_save 'ForumEmail', @ForumEmail

	-- initalize new board
	EXEC yaf_board_create @Name,0,@User,@UserEmail,@Password,1
end
GO

-- yaf_system_updateversion
if exists (select * from sysobjects where id = object_id(N'yaf_system_updateversion') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_system_updateversion
GO

CREATE procedure yaf_system_updateversion(
	@Version		int,
	@VersionName	nvarchar(50)
) as
begin
	EXEC yaf_registry_save 'Version', @Version
	EXEC yaf_registry_save 'VersionName',@VersionName
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_system_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_system_list
GO

if exists (select * from sysobjects where id = object_id(N'yaf_system_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_system_save
GO

-- yaf_board_list
if exists (select * from sysobjects where id = object_id(N'yaf_board_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_board_list
GO

CREATE procedure yaf_board_list(@BoardID int=null) as
begin
	select
		a.*,
		SQLVersion = @@VERSION
	from 
		yaf_Board a
	where
		(@BoardID is null or a.BoardID = @BoardID)
end
GO

-- yaf_board_create
if exists (select * from sysobjects where id = object_id(N'yaf_board_create') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_board_create
GO

create procedure yaf_board_create(
	@BoardName 		nvarchar(50),
	@AllowThreaded	bit,
	@UserName		nvarchar(50),
	@UserEmail		nvarchar(50),
	@UserPass		nvarchar(32),
	@IsHostAdmin	bit
) as 
begin
	declare @BoardID				int
	declare @TimeZone				int
	declare @ForumEmail				nvarchar(50)
	declare	@GroupIDAdmin			int
	declare	@GroupIDGuest			int
	declare @GroupIDMember			int
	declare	@AccessMaskIDAdmin		int
	declare @AccessMaskIDModerator	int
	declare @AccessMaskIDMember		int
	declare	@AccessMaskIDReadOnly	int
	declare @UserIDAdmin			int
	declare @UserIDGuest			int
	declare @RankIDAdmin			int
	declare @RankIDGuest			int
	declare @RankIDNewbie			int
	declare @RankIDMember			int
	declare @RankIDAdvanced			int
	declare	@CategoryID				int
	declare	@ForumID				int

	SET @TimeZone = (SELECT CAST(Value as int) FROM yaf_Registry WHERE LOWER([Name]) = LOWER('TimeZone'))
	SET @ForumEmail = (SELECT Value FROM yaf_Registry WHERE LOWER([Name]) = LOWER('ForumEmail'))

	-- yaf_Board
	insert into yaf_Board([Name],AllowThreaded) values(@BoardName,@AllowThreaded)
	set @BoardID = @@IDENTITY

	-- yaf_Rank
	insert into yaf_Rank(BoardID,Name,IsStart,IsLadder,MinPosts) values(@BoardID,'Administration',0,0,null)
	set @RankIDAdmin = @@IDENTITY
	insert into yaf_Rank(BoardID,Name,IsStart,IsLadder,MinPosts) values(@BoardID,'Guest',0,0,null)
	set @RankIDGuest = @@IDENTITY
	insert into yaf_Rank(BoardID,Name,IsStart,IsLadder,MinPosts) values(@BoardID,'Newbie',1,1,0)
	set @RankIDNewbie = @@IDENTITY
	insert into yaf_Rank(BoardID,Name,IsStart,IsLadder,MinPosts) values(@BoardID,'Member',0,1,10)
	set @RankIDMember = @@IDENTITY
	insert into yaf_Rank(BoardID,Name,IsStart,IsLadder,MinPosts) values(@BoardID,'Advanced Member',0,1,30)
	set @RankIDAdvanced = @@IDENTITY

	-- yaf_AccessMask
	insert into yaf_AccessMask(BoardID,Name,ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess)
	values(@BoardID,'Admin Access Mask',1,1,1,1,1,1,1,1,1,1)
	set @AccessMaskIDAdmin = @@IDENTITY
	insert into yaf_AccessMask(BoardID,Name,ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess)
	values(@BoardID,'Moderator Access Mask',1,1,1,0,0,1,1,1,1,0)
	set @AccessMaskIDModerator = @@IDENTITY
	insert into yaf_AccessMask(BoardID,Name,ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess)
	values(@BoardID,'Member Access Mask',1,1,1,0,0,1,0,1,1,0)
	set @AccessMaskIDMember = @@IDENTITY
	insert into yaf_AccessMask(BoardID,Name,ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess)
	values(@BoardID,'Read Only Access Mask',1,0,0,0,0,0,0,0,0,0)
	set @AccessMaskIDReadOnly = @@IDENTITY

	-- yaf_Group
	insert into yaf_Group(BoardID,Name,IsAdmin,IsGuest,IsStart,IsModerator) values(@BoardID,'Administration',1,0,0,0)
	set @GroupIDAdmin = @@IDENTITY
	insert into yaf_Group(BoardID,Name,IsAdmin,IsGuest,IsStart,IsModerator) values(@BoardID,'Guest',0,1,0,0)
	set @GroupIDGuest = @@IDENTITY
	insert into yaf_Group(BoardID,Name,IsAdmin,IsGuest,IsStart,IsModerator) values(@BoardID,'Member',0,0,1,0)
	set @GroupIDMember = @@IDENTITY

	-- yaf_User
	insert into yaf_User(BoardID,RankID,Name,Password,Joined,LastVisit,NumPosts,TimeZone,Approved,Email,Gender,IsHostAdmin)
	values(@BoardID,@RankIDAdmin,@UserName,@UserPass,getdate(),getdate(),0,@TimeZone,1,@UserEmail,0,@IsHostAdmin)
	set @UserIDAdmin = @@IDENTITY

	insert into yaf_User(BoardID,RankID,Name,Password,Joined,LastVisit,NumPosts,TimeZone,Approved,Email,Gender,IsHostAdmin)
	values(@BoardID,@RankIDGuest,'Guest','na',getdate(),getdate(),0,@TimeZone,1,@ForumEmail,0,0)
	set @UserIDGuest = @@IDENTITY

	-- yaf_UserGroup
	insert into yaf_UserGroup(UserID,GroupID) values(@UserIDAdmin,@GroupIDAdmin)
	insert into yaf_UserGroup(UserID,GroupID) values(@UserIDGuest,@GroupIDGuest)

	-- yaf_Category
	insert into yaf_Category(BoardID,Name,SortOrder) values(@BoardID,'Test Category',1)
	set @CategoryID = @@IDENTITY
	
	-- yaf_Forum
	insert into yaf_Forum(CategoryID,Name,Description,SortOrder,Locked,Hidden,IsTest,Moderated,NumTopics,NumPosts)
	values(@CategoryID,'Test Forum','A test forum',1,0,0,1,0,0,0)
	set @ForumID = @@IDENTITY

	-- yaf_ForumAccess
	insert into yaf_ForumAccess(GroupID,ForumID,AccessMaskID) values(@GroupIDAdmin,@ForumID,@AccessMaskIDAdmin)
	insert into yaf_ForumAccess(GroupID,ForumID,AccessMaskID) values(@GroupIDGuest,@ForumID,@AccessMaskIDReadOnly)
	insert into yaf_ForumAccess(GroupID,ForumID,AccessMaskID) values(@GroupIDMember,@ForumID,@AccessMaskIDMember)
end
GO

CREATE procedure yaf_system_upgrade_to_registry as
begin
	DECLARE @TimeZone			int
	DECLARE @SmtpServer			nvarchar(50)
	DECLARE @SmtpUserName		nvarchar(50)
	DECLARE @SmtpUserPass		nvarchar(50)
	DECLARE @ForumEmail			nvarchar(50)
	DECLARE @EmailVerification		bit
	DECLARE @ShowMoved		bit
	DECLARE @BlankLinks			bit
	DECLARE @ShowGroups		bit
	DECLARE @AvatarWidth		int
	DECLARE @AvatarHeight		int
	DECLARE @AvatarUpload		bit
	DECLARE @AvatarRemote		bit
	DECLARE @AvatarSize			int
	DECLARE @AllowRichEdit		bit
	DECLARE @AllowUserTheme		bit
	DECLARE @AllowUserLanguage		bit
	DECLARE @UseFileTable		bit
	DECLARE @MaxFileSize		int
	DECLARE @tmp			nvarchar(100)	

	select 	@TimeZone = TimeZone,
		@SmtpServer = SmtpServer,
		@SmtpUserName = SmtpUserName,
		@SmtpUserPass = SmtpUserPass,
		@ForumEmail = ForumEmail,
		@EmailVerification = EmailVerification,
		@ShowMoved = ShowMoved,
		@BlankLinks = BlankLinks,
		@ShowGroups = ShowGroups,
		@AvatarWidth = AvatarWidth,
		@AvatarHeight = AvatarHeight,
		@AvatarUpload = AvatarUpload,
		@AvatarRemote = AvatarRemote,
		@AvatarSize = AvatarSize,
		@AllowRichEdit = AllowRichEdit,
		@AllowUserTheme = AllowUserTheme,
		@AllowUserLanguage = AllowUserLanguage,
		@UseFileTable = UseFileTable,
		@MaxFileSize = MaxFileSize
	FROM yaf_System WHERE SystemID = 1

	-- put old settings into new registry table
	EXEC yaf_registry_save 'SmtpServer',@SmtpServer
	EXEC yaf_registry_save 'SmtpUserName',@SmtpUserName
	EXEC yaf_registry_save 'SmtpUserPass',@SmtpUserPass
	EXEC yaf_registry_save 'ForumEmail',@ForumEmail

	SET @tmp = CAST(@TimeZone AS nvarchar(100))
	EXEC yaf_registry_save 'TimeZone',@tmp
	SET @tmp = CAST(@AvatarWidth AS nvarchar(100))
	EXEC yaf_registry_save 'AvatarWidth',@AvatarWidth
	SET @tmp = CAST(@AvatarHeight AS nvarchar(100))
	EXEC yaf_registry_save 'AvatarHeight',@AvatarHeight
	SET @tmp = CAST(@AvatarSize AS nvarchar(100))
	EXEC yaf_registry_save 'AvatarSize',@AvatarSize
	SET @tmp = CAST(@MaxFileSize AS nvarchar(100))
	EXEC yaf_registry_save 'MaxFileSize',@MaxFileSize

	SET @tmp = CAST(@EmailVerification AS nvarchar(100))
	EXEC yaf_registry_save 'EmailVerification',@EmailVerification
	SET @tmp = CAST(@ShowMoved AS nvarchar(100))
	EXEC yaf_registry_save 'ShowMoved',@ShowMoved
	SET @tmp = CAST(@BlankLinks AS nvarchar(100))
	EXEC yaf_registry_save 'BlankLinks',@BlankLinks
	SET @tmp = CAST(@ShowGroups AS nvarchar(100))
	EXEC yaf_registry_save 'ShowGroups',@ShowGroups
	SET @tmp = CAST(@AvatarUpload AS nvarchar(100))
	EXEC yaf_registry_save 'AvatarUpload',@AvatarUpload
	SET @tmp = CAST(@AvatarRemote AS nvarchar(100))
	EXEC yaf_registry_save 'AvatarRemote',@AvatarRemote
	SET @tmp = CAST(@AllowRichEdit AS nvarchar(100))
	EXEC yaf_registry_save 'AllowRichEdit',@AllowRichEdit
	SET @tmp = CAST(@AllowUserTheme AS nvarchar(100))
	EXEC yaf_registry_save 'AllowUserTheme',@AllowUserTheme
	SET @tmp = CAST(@AllowUserLanguage AS nvarchar(100))
	EXEC yaf_registry_save 'AllowUserLanguage',@AllowUserLanguage
	SET @tmp = CAST(@UseFileTable AS nvarchar(100))
	EXEC yaf_registry_save 'UseFileTable',@UseFileTable

end
GO

-- no longer require system table
if exists (select * from dbo.sysobjects where id = object_id(N'yaf_System') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
begin
	-- upgrade then delete
	EXEC yaf_system_upgrade_to_registry
	drop table yaf_System
end
GO

-- and upgrade procedure
if exists (select * from sysobjects where id = object_id(N'yaf_system_upgrade_to_registry') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_system_upgrade_to_registry
GO

if exists (select * from sysobjects where id = object_id(N'yaf_watchtopic_check') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_watchtopic_check
GO

create procedure yaf_watchtopic_check(@UserID int,@TopicID int) as
begin
	SELECT WatchTopicID FROM yaf_WatchTopic WHERE UserID = @UserID AND TopicID = @TopicID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_watchforum_check') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_watchforum_check
GO

create procedure yaf_watchforum_check(@UserID int,@ForumID int) as
begin
	SELECT WatchForumID FROM yaf_WatchForum WHERE UserID = @UserID AND ForumID = @ForumID
end
GO

-- yaf_user_deleteold
if exists (select * from sysobjects where id = object_id(N'yaf_user_deleteold') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_deleteold
GO

create procedure yaf_user_deleteold(@BoardID int) as
begin
	declare @Since datetime
	
	set @Since = getdate()

	delete from yaf_CheckEmail where UserID in(select UserID from yaf_User where BoardID=@BoardID and Approved=0 and datediff(day,Joined,@Since)>2)
	delete from yaf_UserGroup where UserID in(select UserID from yaf_User where BoardID=@BoardID and Approved=0 and datediff(day,Joined,@Since)>2)
	delete from yaf_User where BoardID=@BoardID and Approved=0 and datediff(day,Joined,@Since)>2
end
GO

-- yaf_post_list
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
		TopicLocked	= d.IsLocked,
		ForumLocked	= g.Locked,
		a.MessageID,
		a.Posted,
		Subject = d.Topic,
		a.Message,
		a.UserID,
		a.Position,
		a.Indent,
		UserName	= IsNull(a.UserName,b.Name),
		b.Joined,
		b.Avatar,
		b.Location,
		b.Signature,
		b.HomePage,
		b.Weblog,
		b.MSN,
		b.YIM,
		b.AIM,
		b.ICQ,
		Posts		= b.NumPosts,
		d.Views,
		d.ForumID,
		RankName = c.Name,
		c.RankImage,
		Edited = IsNull(a.Edited,a.Posted),
		HasAttachments	= (select count(1) from yaf_Attachment x where x.MessageID=a.MessageID),
		HasAvatarImage = (select count(1) from yaf_User x where x.UserID=b.UserID and AvatarImage is not null)
	from
		yaf_Message a
		join yaf_User b on b.UserID=a.UserID
		join yaf_Topic d on d.TopicID=a.TopicID
		join yaf_Forum g on g.ForumID=d.ForumID
		join yaf_Category h on h.CategoryID=g.CategoryID
		join yaf_Rank c on c.RankID=b.RankID
	where
		a.Approved <> 0 and
		a.TopicID = @TopicID
	order by
		a.Posted asc
end
GO

-- yaf_user_savesignature
if exists (select * from sysobjects where id = object_id(N'yaf_user_savesignature') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_savesignature
GO

create procedure yaf_user_savesignature(@UserID int,@Signature ntext) as
begin
	update yaf_User set Signature = @Signature where UserID = @UserID
end
GO

-- yaf_nntpforum_update
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
	-- exec yaf_topic_updatelastpost @ForumID,null
end
GO

-- yaf_nntptopic_savemessage
if exists (select * from sysobjects where id = object_id(N'yaf_nntptopic_savemessage') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_nntptopic_savemessage
GO

create procedure yaf_nntptopic_savemessage(
	@NntpForumID	int,
	@Topic 			nvarchar(100),
	@Body 			ntext,
	@UserID 		int,
	@UserName		nvarchar(50),
	@IP				nvarchar(15),
	@Posted			datetime,
	@Thread			char(32)
) as 
begin
	declare	@ForumID	int
	declare @TopicID	int
	declare	@MessageID	int

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
	insert into yaf_Message(TopicID,UserID,UserName,Posted,Message,IP,Approved,[Position],Indent)
	values(@TopicID,@UserID,@UserName,@Posted,@Body,@IP,1,0,0)
	set @MessageID=@@IDENTITY

	-- update user
	update yaf_User set NumPosts=NumPosts+1 where UserID=@UserID
	-- update topic
	update yaf_Topic set 
		LastPosted		= @Posted,
		LastMessageID	= @MessageID,
		LastUserID		= @UserID,
		LastUserName	= @UserName
	where TopicID=@TopicID	
	-- update forum
	update yaf_Forum set
		LastPosted		= @Posted,
		LastTopicID	= @TopicID,
		LastMessageID	= @MessageID,
		LastUserID		= @UserID,
		LastUserName	= @UserName
	where ForumID=@ForumID and (LastPosted is null or LastPosted<@Posted)
end
GO

-- yaf_forum_save
if exists (select * from sysobjects where id = object_id(N'yaf_forum_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_save
GO

create procedure yaf_forum_save(
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
	@AccessMaskID	int = null
) as
begin
	declare @BoardID	int

	if @ForumID>0 begin
		update yaf_Forum set 
			ParentID=@ParentID,
			Name=@Name,
			Description=@Description,
			SortOrder=@SortOrder,
			Hidden=@Hidden,
			Locked=@Locked,
			CategoryID=@CategoryID,
			IsTest = @IsTest,
			Moderated = @Moderated,
			RemoteURL = @RemoteURL
		where ForumID=@ForumID
	end
	else begin
		select @BoardID=BoardID from yaf_Category where CategoryID=@CategoryID
	
		insert into yaf_Forum(ParentID,Name,Description,SortOrder,Hidden,Locked,CategoryID,IsTest,Moderated,NumTopics,NumPosts,RemoteURL)
		values(@ParentID,@Name,@Description,@SortOrder,@Hidden,@Locked,@CategoryID,@IsTest,@Moderated,0,0,@RemoteURL)
		select @ForumID = @@IDENTITY

		insert into yaf_ForumAccess(GroupID,ForumID,AccessMaskID) 
		select GroupID,@ForumID,@AccessMaskID
		from yaf_Group 
		where BoardID=@BoardID
	end
	select ForumID = @ForumID
end
GO

-- yaf_message_delete
if exists (select * from sysobjects where id = object_id(N'yaf_message_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_delete
GO

CREATE   procedure yaf_message_delete(@MessageID int) as
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

	-- Delete attachments
	delete from yaf_attachment where MessageID = @MessageID

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

if exists (select * from sysobjects where id = object_id(N'yaf_forum_updatelastpost') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_updatelastpost
GO

create procedure yaf_forum_updatelastpost(@ForumID int) as
begin
	update yaf_Forum set
		LastPosted = (select top 1 y.Posted from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
		LastTopicID = (select top 1 y.TopicID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
		LastMessageID = (select top 1 y.MessageID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
		LastUserID = (select top 1 y.UserID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc),
		LastUserName = (select top 1 y.UserName from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and y.Approved<>0 order by y.Posted desc)
	where ForumID = @ForumID
end
GO

-- yaf_topic_delete
if exists (select * from sysobjects where id = object_id(N'yaf_topic_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_delete
GO

CREATE procedure yaf_topic_delete (@TopicID int,@UpdateLastPost bit=1) 
as
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

	-- BAI CHANGED 01.02.2004
	-- Delete messages and attachments
	--delete from yaf_Message where TopicID = @TopicID

	declare @tmpMessageID int;
	declare msg_cursor cursor for
		select MessageID from yaf_message
		where TopicID = @TopicID
		order by MessageID desc
	
	open msg_cursor
	
	fetch next from msg_cursor
	into @tmpMessageID
	
	-- Check @@FETCH_STATUS to see if there are any more rows to fetch.
	while @@FETCH_STATUS = 0
	begin
		delete from yaf_attachment where MessageID = @tmpMessageID;
		delete from yaf_message where MessageID = @tmpMessageID;
	
	   -- This is executed as long as the previous fetch succeeds.
		fetch next from msg_cursor
		into @tmpMessageID
	end
	
	close msg_cursor
	deallocate msg_cursor

	-- Messagedelete finished
	-- ENDED BAI CHANGED 01.02.2004

	delete from yaf_Topic where TopicMovedID = @TopicID
	delete from yaf_Topic where TopicID = @TopicID
	--commit
	if @UpdateLastPost<>0
		exec yaf_forum_updatelastpost @ForumID
	
	if @ForumID is not null
		exec yaf_forum_updatestats @ForumID
end
GO

-- yaf_topic_listmessages
--ABOT NEW 16.04.04
if exists (select * from sysobjects where id = object_id(N'yaf_topic_listmessages') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_listmessages
GO

create procedure yaf_topic_listmessages(@TopicID int) as
begin
	select * from yaf_Message
	where TopicID = @TopicID
end
GO
--END ABOT NEW 16.04.04

-- yaf_nntpforum_delete
if exists (select * from sysobjects where id = object_id(N'yaf_nntpforum_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_nntpforum_delete
GO

create procedure yaf_nntpforum_delete(@NntpForumID int) as
begin
	delete from yaf_NntpTopic where NntpForumID = @NntpForumID
	delete from yaf_NntpForum where NntpForumID = @NntpForumID
end
GO

--ABOT NEW 16.04.04
-- yaf_forum_listSubForums
if exists (select * from sysobjects where id = object_id(N'yaf_forum_listSubForums') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_listSubForums
GO

CREATE procedure yaf_forum_listSubForums(@ForumID int) as
begin
	select Sum(1) from yaf_Forum where ParentID = @ForumID
end
GO
--END ABOT NEW 16.04.04
--ABOT NEW 16.04.04

-- yaf_forum_listtopics
if exists (select * from sysobjects where id = object_id(N'yaf_forum_listtopics') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_listtopics
GO

create procedure yaf_forum_listtopics(@ForumID int) as
begin
select * from yaf_Topic
Where ForumID = @ForumID
end
GO
--END ABOT NEW 16.04.04

if exists(select 1 from sysobjects where id = object_id(N'yaf_forum_posts') and OBJECTPROPERTY(id, N'IsScalarFunction')=1)
	drop function {dbo}.yaf_forum_posts
go

create function {dbo}.yaf_forum_posts(@ForumID int) returns int as
begin
	declare @NumPosts int
	declare @tmp int

	select @NumPosts=NumPosts from yaf_Forum where ForumID=@ForumID

	if exists(select 1 from yaf_Forum where ParentID=@ForumID)
	begin
		declare c cursor for
		select ForumID from yaf_Forum
		where ParentID = @ForumID
		
		open c
		
		fetch next from c into @tmp
		while @@FETCH_STATUS = 0
		begin
			set @NumPosts=@NumPosts+{dbo}.yaf_forum_posts(@tmp)
			fetch next from c into @tmp
		end
		close c
		deallocate c
	end

	return @NumPosts
end
go

if exists(select 1 from sysobjects where id = object_id(N'yaf_forum_topics') and OBJECTPROPERTY(id, N'IsScalarFunction')=1)
	drop function {dbo}.yaf_forum_topics
go

create function {dbo}.yaf_forum_topics(@ForumID int) returns int as
begin
	declare @NumTopics int
	declare @tmp int

	select @NumTopics=NumTopics from yaf_Forum where ForumID=@ForumID

	if exists(select 1 from yaf_Forum where ParentID=@ForumID)
	begin
		declare c cursor for
		select ForumID from yaf_Forum
		where ParentID = @ForumID
		
		open c
		
		fetch next from c into @tmp
		while @@FETCH_STATUS = 0
		begin
			set @NumTopics=@NumTopics+{dbo}.yaf_forum_topics(@tmp)
			fetch next from c into @tmp
		end
		close c
		deallocate c
	end

	return @NumTopics
end
go

-- yaf_forum_listread
if exists (select * from sysobjects where id = object_id(N'yaf_forum_listread') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_listread
GO

create procedure yaf_forum_listread(@BoardID int,@UserID int,@CategoryID int=null,@ParentID int=null) as
begin
	select 
		a.CategoryID, 
		Category		= a.Name, 
		ForumID			= b.ForumID,
		Forum			= b.Name, 
		Description,
		Topics			= {dbo}.yaf_forum_topics(b.ForumID),
		Posts			= {dbo}.yaf_forum_posts(b.ForumID),
		LastPosted		= b.LastPosted,
		LastMessageID	= b.LastMessageID,
		LastUserID		= b.LastUserID,
		LastUser		= IsNull(b.LastUserName,(select Name from yaf_User x where x.UserID=b.LastUserID)),
		LastTopicID		= b.LastTopicID,
		LastTopicName	= (select x.Topic from yaf_Topic x where x.TopicID=b.LastTopicID),
		b.Locked,
		b.Moderated,
		Viewing			= (select count(1) from yaf_Active x where x.ForumID=b.ForumID),
		b.RemoteURL,
		x.ReadAccess
	from 
		yaf_Category a
		join yaf_Forum b on b.CategoryID=a.CategoryID
		join yaf_vaccess x on x.ForumID=b.ForumID
	where 
		a.BoardID = @BoardID and
		(b.Hidden=0 or x.ReadAccess<>0) and
		(@CategoryID is null or a.CategoryID=@CategoryID) and
		((@ParentID is null and b.ParentID is null) or b.ParentID=@ParentID) and
		x.UserID = @UserID
	order by
		a.SortOrder,
		b.SortOrder
end
GO

-- yaf_user_access
if exists (select * from sysobjects where id = object_id(N'yaf_user_access') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_access
GO
