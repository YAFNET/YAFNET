/* Version 0.9.9 */

if not exists (select * from dbo.sysobjects where id = object_id(N'yaf_Replace_Words') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE yaf_Replace_Words(
		[id] [int] IDENTITY (1, 1) NOT NULL ,
		[badword] [nvarchar] (50) NULL ,
		[goodword] [nvarchar] (50) NULL ,
		constraint PK_Replace_Words primary key(id)
	)
GO

if not exists(select * from syscolumns where id=object_id('yaf_Message') and name='BBCode')
begin
	alter table yaf_Message add BBCode bit null
	exec('update yaf_Message set BBCode=0 where BBCode is null')
	alter table yaf_Message alter column BBCode bit not null
end
go

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

