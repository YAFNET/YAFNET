/* Version 0.9.8 */

-- Tables
alter table yaf_AccessMask alter column "Name" nvarchar(50) not null
GO

if exists(select * from sysindexes where id=object_id('yaf_Active') and name='PK_Active')
	ALTER TABLE yaf_Active DROP CONSTRAINT PK_Active
GO

alter table yaf_Active alter column SessionID nvarchar(24) not null
GO

if not exists(select * from sysindexes where id=object_id('yaf_Active') and name='PK_Active')
ALTER TABLE [yaf_Active] WITH NOCHECK ADD 
	CONSTRAINT [PK_Active] PRIMARY KEY CLUSTERED([SessionID],[BoardID])
GO

alter table yaf_Active alter column IP nvarchar(15) not null
GO

alter table yaf_Active alter column Location nvarchar(50) not null
GO

alter table yaf_Active alter column Browser nvarchar(50) null
GO

alter table yaf_Active alter column Platform nvarchar(50) null
GO

alter table yaf_Attachment alter column FileName nvarchar(250) not null
GO

alter table yaf_Attachment alter column ContentType nvarchar(50) null
GO

if exists(select * from sysindexes where id=object_id('yaf_BannedIP') and name='IX_BannedIP')
	ALTER TABLE [yaf_BannedIP] DROP CONSTRAINT IX_BannedIP
GO

alter table yaf_BannedIP alter column Mask nvarchar(15) not null
GO

if not exists(select * from sysindexes where id=object_id('yaf_BannedIP') and name='IX_BannedIP')
ALTER TABLE [yaf_BannedIP] ADD 
	CONSTRAINT [IX_BannedIP] UNIQUE NONCLUSTERED([BoardID],[Mask])
GO

alter table yaf_Board alter column "Name" nvarchar(50) not null
GO

if exists(select * from sysindexes where id=object_id('yaf_Category') and name='IX_Category')
	ALTER TABLE [yaf_Category] DROP CONSTRAINT [IX_Category]
GO

alter table yaf_Category alter column "Name" nvarchar(50) not null
GO

if not exists(select * from sysindexes where id=object_id('yaf_Category') and name='IX_Category')
	ALTER TABLE [yaf_Category] ADD CONSTRAINT [IX_Category] UNIQUE NONCLUSTERED([BoardID],[Name])
GO

alter table yaf_CheckEmail alter column Email nvarchar(50) not null
GO

if exists(select * from sysindexes where id=object_id('yaf_CheckEmail') and name='IX_CheckEmail')
	ALTER TABLE yaf_CheckEmail DROP CONSTRAINT IX_CheckEmail
GO

alter table yaf_CheckEmail alter column Hash nvarchar(32) not null
GO

if not exists(select * from sysindexes where id=object_id('yaf_CheckEmail') and name='IX_CheckEmail')
ALTER TABLE [yaf_CheckEmail] ADD 
	CONSTRAINT [IX_CheckEmail] UNIQUE  NONCLUSTERED 
	(
		[Hash]
	)  ON [PRIMARY] 
GO

alter table yaf_Choice alter column Choice nvarchar(50) not null
GO

if exists(select * from sysindexes where id=object_id('yaf_Forum') and name='IX_Forum')
	ALTER TABLE yaf_Forum DROP CONSTRAINT IX_Forum
GO

alter table yaf_Forum alter column "Name" nvarchar(50) not null
GO

if not exists(select * from sysindexes where id=object_id('yaf_Forum') and name='IX_Forum')
ALTER TABLE [yaf_Forum] ADD 
	CONSTRAINT [IX_Forum] UNIQUE  NONCLUSTERED 
	(
		[CategoryID],
		[Name]
	)  ON [PRIMARY] 
GO

alter table yaf_Forum alter column Description nvarchar(255) not null
GO

alter table yaf_Forum alter column LastUserName nvarchar(50) null
GO

if exists(select * from sysindexes where id=object_id('yaf_Group') and name='IX_Group')
	ALTER TABLE [yaf_Group] DROP CONSTRAINT IX_Group
GO

alter table yaf_Group alter column "Name" nvarchar(50) not null
GO

if not exists(select * from sysindexes where id=object_id('yaf_Group') and name='IX_Group')
ALTER TABLE [yaf_Group] ADD 
	CONSTRAINT [IX_Group] UNIQUE NONCLUSTERED([BoardID],[Name])
GO

alter table yaf_Mail alter column FromUser nvarchar(50) not null
GO

alter table yaf_Mail alter column ToUser nvarchar(50) not null
GO

alter table yaf_Mail alter column Subject nvarchar(100) not null
GO

alter table yaf_Message alter column UserName nvarchar(50) null
GO

alter table yaf_Message alter column IP nvarchar(15) not null
GO

alter table yaf_NntpForum alter column GroupName nvarchar(100) not null
GO

alter table yaf_NntpServer alter column "Name" nvarchar(50) not null
GO

alter table yaf_NntpServer alter column Address nvarchar(100) not null
GO

alter table yaf_NntpServer alter column UserName nvarchar(50) null
GO

alter table yaf_NntpServer alter column UserPass nvarchar(50) null
GO

alter table yaf_PMessage alter column Subject nvarchar(100) not null
GO

alter table yaf_Poll alter column Question nvarchar(50) not null
GO

if exists(select * from sysindexes where id=object_id('yaf_Rank') and name='IX_Rank')
	ALTER TABLE [yaf_Rank] DROP CONSTRAINT [IX_Rank]
GO

alter table yaf_Rank alter column "Name" nvarchar(50) not null
GO

if not exists(select * from sysindexes where id=object_id('yaf_Rank') and name='IX_Rank')
	ALTER TABLE [yaf_Rank] ADD CONSTRAINT [IX_Rank] UNIQUE([BoardID],[Name])
GO

alter table yaf_Rank alter column RankImage nvarchar(50) null
GO

if exists(select * from sysindexes where id=object_id('yaf_Smiley') and name='IX_Smiley')
	ALTER TABLE [yaf_Smiley] DROP CONSTRAINT [IX_Smiley]
GO

alter table yaf_Smiley alter column Code nvarchar(10) not null
GO

if not exists(select * from sysindexes where id=object_id('yaf_Smiley') and name='IX_Smiley')
	ALTER TABLE [yaf_Smiley] ADD 
		CONSTRAINT [IX_Smiley] UNIQUE NONCLUSTERED([BoardID],[Code])
GO

alter table yaf_Smiley alter column Icon nvarchar(50) not null
GO

alter table yaf_Smiley alter column Emoticon nvarchar(50) null
GO

alter table yaf_System alter column VersionName nvarchar(50) not null
GO

alter table yaf_System alter column SmtpServer nvarchar(50) not null
GO

alter table yaf_System alter column SmtpUserName nvarchar(50) null
GO

alter table yaf_System alter column SmtpUserPass nvarchar(50) null
GO

alter table yaf_System alter column ForumEmail nvarchar(50) not null
GO

alter table yaf_Topic alter column UserName nvarchar(50) null
GO

alter table yaf_Topic alter column Topic nvarchar(100) not null
GO

alter table yaf_Topic alter column LastUserName nvarchar(50) null
GO

if exists(select * from sysindexes where id=object_id('yaf_User') and name='IX_User')
	ALTER TABLE [yaf_User] DROP CONSTRAINT [IX_User]
GO

alter table yaf_User alter column "Name" nvarchar(50) not null
GO

if not exists(select * from sysindexes where id=object_id('yaf_User') and name='IX_User')
	ALTER TABLE [yaf_User] ADD CONSTRAINT [IX_User] UNIQUE NONCLUSTERED([BoardID],[Name])
GO

alter table yaf_User alter column Password nvarchar(32) not null
GO

alter table yaf_User alter column Email nvarchar(50) null
GO

alter table yaf_User alter column IP nvarchar(15) null
GO

alter table yaf_User alter column Location nvarchar(50) null
GO

alter table yaf_User alter column HomePage nvarchar(50) null
GO

alter table yaf_User alter column Avatar nvarchar(255) null
GO

alter table yaf_User alter column Signature nvarchar(255) null
GO

alter table yaf_User alter column LanguageFile nvarchar(50) null
GO

alter table yaf_User alter column ThemeFile nvarchar(50) null
GO

alter table yaf_User alter column MSN nvarchar(50) null
GO

alter table yaf_User alter column YIM nvarchar(30) null
GO

alter table yaf_User alter column AIM nvarchar(30) null
GO

alter table yaf_User alter column RealName nvarchar(50) null
GO

alter table yaf_User alter column Occupation nvarchar(50) null
GO

alter table yaf_User alter column Interests nvarchar(100) null
GO

alter table yaf_User alter column Weblog nvarchar(100) null
GO

-- ntext (not possible to alter)
--alter table yaf_Mail alter column Body ntext not null
GO

--alter table yaf_Message alter column Message ntext not null
GO

--alter table yaf_PMessage alter column Body ntext not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_NntpServer') and name='Port')
	alter table yaf_NntpServer add Port int null
GO

-- yaf_poll_save
if exists (select * from sysobjects where id = object_id(N'yaf_poll_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_poll_save
GO

create procedure yaf_poll_save(
	@Question	nvarchar(50),
	@Choice1	nvarchar(50),
	@Choice2	nvarchar(50),
	@Choice3	nvarchar(50) = null,
	@Choice4	nvarchar(50) = null,
	@Choice5	nvarchar(50) = null,
	@Choice6	nvarchar(50) = null,
	@Choice7	nvarchar(50) = null,
	@Choice8	nvarchar(50) = null,
	@Choice9	nvarchar(50) = null
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

-- yaf_system_updateversion
if exists (select * from sysobjects where id = object_id(N'yaf_system_updateversion') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_system_updateversion
GO

create procedure yaf_system_updateversion(
	@Version		int,
	@VersionName	nvarchar(50)
) as
begin
	update yaf_System set
		Version = @Version,
		VersionName = @VersionName
end
GO

-- yaf_mail_createwatch
if exists (select * from sysobjects where id = object_id(N'yaf_mail_createwatch') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_mail_createwatch
GO

create procedure yaf_mail_createwatch(@TopicID int,@From nvarchar(50),@Subject nvarchar(100),@Body ntext,@UserID int) as begin
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

-- yaf_nntpforum_save
if exists (select * from sysobjects where id = object_id(N'yaf_nntpforum_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_nntpforum_save
GO

create procedure yaf_nntpforum_save(@NntpForumID int=null,@NntpServerID int,@GroupName nvarchar(100),@ForumID int) as
begin
	if @NntpForumID is null
		insert into yaf_NntpForum(NntpServerID,GroupName,ForumID,LastMessageNo,LastUpdate)
		values(@NntpServerID,@GroupName,@ForumID,0,getdate())
	else
		update yaf_NntpForum set
			NntpServerID = @NntpServerID,
			GroupName = @GroupName,
			ForumID = @ForumID
		where NntpForumID = @NntpForumID
end
GO

-- yaf_checkemail_save
if exists (select * from sysobjects where id = object_id(N'yaf_checkemail_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_checkemail_save
GO

create procedure yaf_checkemail_save(@UserID int,@Hash nvarchar(32),@Email nvarchar(50)) as
begin
	insert into yaf_CheckEmail(UserID,Email,Created,Hash)
	values(@UserID,@Email,getdate(),@Hash)	
end
GO

-- yaf_checkemail_update
if exists (select * from sysobjects where id = object_id(N'yaf_checkemail_update') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_checkemail_update
GO

create procedure yaf_checkemail_update(@Hash nvarchar(32)) as
begin
	declare @UserID int
	declare @CheckEmailID int
	declare @Email nvarchar(50)
	set @UserID = null
	select 
		@CheckEmailID = CheckEmailID,
		@UserID = UserID,
		@Email = Email
	from
		yaf_CheckEmail
	where
		Hash = @Hash
	if @UserID is null begin
		select convert(bit,0)
		return
	end
	-- Update new user email
	update yaf_User set Email = @Email, Approved = 1 where UserID = @UserID
	delete yaf_CheckEmail where CheckEmailID = @CheckEmailID
	select convert(bit,1)
end
GO

-- yaf_message_searchphrase
if exists (select * from sysobjects where id = object_id(N'yaf_message_searchphrase') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_searchphrase
GO

-- yaf_user_changepassword
if exists (select * from sysobjects where id = object_id(N'yaf_user_changepassword') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_changepassword
GO

create procedure yaf_user_changepassword(@UserID int,@OldPassword nvarchar(32),@NewPassword nvarchar(32)) as
begin
	declare @CurrentOld nvarchar(32)
	select @CurrentOld = Password from yaf_User where UserID = @UserID
	if @CurrentOld<>@OldPassword begin
		select Success = convert(bit,0)
		return
	end
	update yaf_User set Password = @NewPassword where UserID = @UserID
	select Success = convert(bit,1)
end
GO

-- yaf_user_recoverpassword
if exists (select * from sysobjects where id = object_id(N'yaf_user_recoverpassword') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_recoverpassword
GO

create procedure yaf_user_recoverpassword(@UserName nvarchar(50),@Email nvarchar(50),@Password nvarchar(32)) as
begin
	declare @UserID int
	select @UserID = UserID from yaf_User where Name = @UserName and Email = @Email
	if @UserID is null begin
		select Success = convert(bit,0)
		return
	end
	update yaf_User set Password = @Password where UserID = @UserID
	select Success = convert(bit,1)
end
GO

-- yaf_message_approve
if exists (select * from sysobjects where id = object_id(N'yaf_message_approve') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_approve
GO

create procedure yaf_message_approve(@MessageID int) as begin
	declare	@UserID		int
	declare	@ForumID	int
	declare	@TopicID	int
	declare @Posted		datetime
	declare	@UserName	nvarchar(50)

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
		LastPosted = @Posted,
		LastTopicID = @TopicID,
		LastMessageID = @MessageID,
		LastUserID = @UserID,
		LastUserName = @UserName
	where ForumID = @ForumID

	-- update yaf_Topic
	update yaf_Topic set
		LastPosted = @Posted,
		LastMessageID = @MessageID,
		LastUserID = @UserID,
		LastUserName = @UserName,
		NumPosts = (select count(1) from yaf_Message x where x.TopicID=yaf_Topic.TopicID and x.Approved<>0)
	where TopicID = @TopicID
	
	-- update forum stats
	exec yaf_forum_updatestats @ForumID
end
GO

-- yaf_user_approve
if exists (select * from sysobjects where id = object_id(N'yaf_user_approve') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_approve
GO

create procedure yaf_user_approve(@UserID int) as
begin
	declare @CheckEmailID int
	declare @Email nvarchar(50)

	select 
		@CheckEmailID = CheckEmailID,
		@Email = Email
	from
		yaf_CheckEmail
	where
		UserID = @UserID

	-- Update new user email
	update yaf_User set Email = @Email, Approved = 1 where UserID = @UserID
	delete yaf_CheckEmail where CheckEmailID = @CheckEmailID
	select convert(bit,1)
end
GO

-- yaf_attachment_save
if exists (select * from sysobjects where id = object_id(N'yaf_attachment_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_attachment_save
GO

create procedure yaf_attachment_save(@MessageID int,@FileName nvarchar(50),@Bytes int,@ContentType nvarchar(50)=null,@FileData image=null) as begin
	insert into yaf_Attachment(MessageID,FileName,Bytes,ContentType,Downloads,FileData) values(@MessageID,@FileName,@Bytes,@ContentType,0,@FileData)
end
GO

-- yaf_category_save
if exists (select * from sysobjects where id = object_id(N'yaf_category_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_category_save
GO

create procedure yaf_category_save(@BoardID int,@CategoryID int,@Name nvarchar(50),@SortOrder smallint) as
begin
	if @CategoryID>0 begin
		update yaf_Category set Name=@Name,SortOrder=@SortOrder where CategoryID=@CategoryID
		select CategoryID = @CategoryID
	end
	else begin
		insert into yaf_Category(BoardID,Name,SortOrder) values(@BoardID,@Name,@SortOrder)
		select CategoryID = @@IDENTITY
	end
end
GO

-- yaf_accessmask_save
if exists (select * from sysobjects where id = object_id(N'yaf_accessmask_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_accessmask_save
GO

create procedure yaf_accessmask_save(
	@AccessMaskID		int=null,
	@BoardID			int,
	@Name				nvarchar(50),
	@ReadAccess			bit,
	@PostAccess			bit,
	@ReplyAccess		bit,
	@PriorityAccess		bit,
	@PollAccess			bit,
	@VoteAccess			bit,
	@ModeratorAccess	bit,
	@EditAccess			bit,
	@DeleteAccess		bit,
	@UploadAccess		bit
) as
begin
	if @AccessMaskID is null
		insert into yaf_AccessMask([Name],BoardID,ReadAccess,PostAccess,ReplyAccess,PriorityAccess,PollAccess,VoteAccess,ModeratorAccess,EditAccess,DeleteAccess,UploadAccess)
		values(@Name,@BoardID,@ReadAccess,@PostAccess,@ReplyAccess,@PriorityAccess,@PollAccess,@VoteAccess,@ModeratorAccess,@EditAccess,@DeleteAccess,@UploadAccess)
	else
		update yaf_AccessMask set
			[Name]			= @Name,
			ReadAccess		= @ReadAccess,
			PostAccess		= @PostAccess,
			ReplyAccess		= @ReplyAccess,
			PriorityAccess	= @PriorityAccess,
			PollAccess		= @PollAccess,
			VoteAccess		= @VoteAccess,
			ModeratorAccess	= @ModeratorAccess,
			EditAccess		= @EditAccess,
			DeleteAccess	= @DeleteAccess,
			UploadAccess	= @UploadAccess
		where AccessMaskID=@AccessMaskID
end
GO

-- yaf_group_save
if exists (select * from sysobjects where id = object_id(N'yaf_group_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_group_save
GO

create procedure yaf_group_save(
	@GroupID		int,
	@BoardID		int,
	@Name			nvarchar(50),
	@IsAdmin		bit,
	@IsGuest		bit,
	@IsStart		bit,
	@IsModerator	bit,
	@AccessMaskID	int=null
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
		insert into yaf_Group(Name,BoardID,IsAdmin,IsGuest,IsStart,IsModerator)
		values(@Name,@BoardID,@IsAdmin,@IsGuest,@IsStart,@IsModerator);
		set @GroupID = @@IDENTITY
		insert into yaf_ForumAccess(GroupID,ForumID,AccessMaskID)
		select @GroupID,a.ForumID,@AccessMaskID from yaf_Forum a join yaf_Category b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID
	end
	select GroupID = @GroupID
end
GO

-- yaf_bannedip_save
if exists (select * from sysobjects where id = object_id(N'yaf_bannedip_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_bannedip_save
GO

create procedure yaf_bannedip_save(@ID int=null,@BoardID int,@Mask nvarchar(15)) as
begin
	if @ID is null or @ID = 0 begin
		insert into yaf_BannedIP(BoardID,Mask,Since) values(@BoardID,@Mask,getdate())
	end
	else begin
		update yaf_BannedIP set Mask = @Mask where ID = @ID
	end
end
GO

-- yaf_nntpserver_save
if exists (select * from sysobjects where id = object_id(N'yaf_nntpserver_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_nntpserver_save
GO

create procedure yaf_nntpserver_save(
	@NntpServerID 	int=null,
	@BoardID	int,
	@Name		nvarchar(50),
	@Address	nvarchar(100),
	@Port		int,
	@UserName	nvarchar(50)=null,
	@UserPass	nvarchar(50)=null
) as begin
	if @NntpServerID is null
		insert into yaf_NntpServer(Name,BoardID,Address,Port,UserName,UserPass)
		values(@Name,@BoardID,@Address,@Port,@UserName,@UserPass)
	else
		update yaf_NntpServer set
			Name = @Name,
			Address = @Address,
			Port = @Port,
			UserName = @UserName,
			UserPass = @UserPass
		where NntpServerID = @NntpServerID
end
GO

-- yaf_smiley_save
if exists (select * from sysobjects where id = object_id(N'yaf_smiley_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_smiley_save
GO

create procedure yaf_smiley_save(@SmileyID int=null,@BoardID int,@Code nvarchar(10),@Icon nvarchar(50),@Emoticon nvarchar(50),@Replace smallint=0) as begin
	if @SmileyID is not null begin
		update yaf_Smiley set Code = @Code, Icon = @Icon, Emoticon = @Emoticon where SmileyID = @SmileyID
	end
	else begin
		if @Replace>0
			delete from yaf_Smiley where Code=@Code

		if not exists(select 1 from yaf_Smiley where BoardID=@BoardID and Code=@Code)
			insert into yaf_Smiley(BoardID,Code,Icon,Emoticon) values(@BoardID,@Code,@Icon,@Emoticon)
	end
end
GO

-- yaf_user_adminsave
if exists (select * from sysobjects where id = object_id(N'yaf_user_adminsave') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_adminsave
GO

create procedure yaf_user_adminsave(@BoardID int,@UserID int,@Name nvarchar(50),@Email nvarchar(50),@IsHostAdmin bit,@RankID int) as
begin
	update yaf_User set
		Name = @Name,
		Email = @Email,
		IsHostAdmin = @IsHostAdmin,
		RankID = @RankID
	where UserID = @UserID
	select UserID = @UserID
end
GO

-- yaf_system_save
if exists (select * from sysobjects where id = object_id(N'yaf_system_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_system_save
GO

create procedure yaf_system_save(
	@TimeZone			int,
	@SmtpServer			nvarchar(50),
	@SmtpUserName		nvarchar(50)=null,
	@SmtpUserPass		nvarchar(50)=null,
	@ForumEmail			nvarchar(50),
	@EmailVerification	bit,
	@ShowMoved			bit,
	@BlankLinks			bit,
	@ShowGroups			bit,
	@AvatarWidth		int,
	@AvatarHeight		int,
	@AvatarUpload		bit,
	@AvatarRemote		bit,
	@AvatarSize			int=null,
	@AllowRichEdit		bit,
	@AllowUserTheme		bit,
	@AllowUserLanguage	bit,
	@UseFileTable		bit,
	@MaxFileSize		int=null
) as
begin
	update yaf_System set
		TimeZone = @TimeZone,
		SmtpServer = @SmtpServer,
		SmtpUserName = @SmtpUserName,
		SmtpUserPass = @SmtpUserPass,
		ForumEmail = @ForumEmail,
		EmailVerification = @EmailVerification,
		ShowMoved = @ShowMoved,
		BlankLinks = @BlankLinks,
		ShowGroups = @ShowGroups,
		AvatarWidth = @AvatarWidth,
		AvatarHeight = @AvatarHeight,
		AvatarUpload = @AvatarUpload,
		AvatarRemote = @AvatarRemote,
		AvatarSize = @AvatarSize,
		AllowRichEdit = @AllowRichEdit,
		AllowUserTheme = @AllowUserTheme,
		AllowUserLanguage = @AllowUserLanguage,
		UseFileTable = @UseFileTable,
		MaxFileSize = @MaxFileSize
end
GO

-- yaf_board_save
if exists (select * from sysobjects where id = object_id(N'yaf_board_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_board_save
GO

create procedure yaf_board_save(@BoardID int,@Name nvarchar(50),@AllowThreaded bit) as
begin
	update yaf_Board set
		[Name] = @Name,
		AllowThreaded = @AllowThreaded
	where BoardID=@BoardID
end
GO

-- yaf_system_initialize
if exists (select * from sysobjects where id = object_id(N'yaf_system_initialize') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_system_initialize
GO

create procedure yaf_system_initialize(
	@Name		nvarchar(50),
	@TimeZone	int,
	@ForumEmail	nvarchar(50),
	@SmtpServer	nvarchar(50),
	@User		nvarchar(50),
	@UserEmail	nvarchar(50),
	@Password	nvarchar(32)
) as 
begin
	SET IDENTITY_INSERT yaf_System ON
	insert into yaf_System(SystemID,Version,VersionName,TimeZone,SmtpServer,ForumEmail,AvatarWidth,AvatarHeight,AvatarUpload,AvatarRemote,EmailVerification,ShowMoved,BlankLinks,ShowGroups,AllowRichEdit,AllowUserTheme,AllowUserLanguage,UseFileTable)
	values(1,1,'0.9.5',@TimeZone,@SmtpServer,@ForumEmail,50,80,0,0,1,1,0,1,1,0,0,0)
	SET IDENTITY_INSERT yaf_System OFF

	exec yaf_board_create @Name,0,@User,@UserEmail,@Password,1
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

	select @TimeZone=TimeZone,@ForumEmail=ForumEmail from yaf_System

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
			Moderated = @Moderated
		where ForumID=@ForumID
	end
	else begin
		select @BoardID=BoardID from yaf_Category where CategoryID=@CategoryID
	
		insert into yaf_Forum(ParentID,Name,Description,SortOrder,Hidden,Locked,CategoryID,IsTest,Moderated,NumTopics,NumPosts)
		values(@ParentID,@Name,@Description,@SortOrder,@Hidden,@Locked,@CategoryID,@IsTest,@Moderated,0,0)
		select @ForumID = @@IDENTITY

		insert into yaf_ForumAccess(GroupID,ForumID,AccessMaskID) 
		select GroupID,@ForumID,@AccessMaskID
		from yaf_Group 
		where BoardID=@BoardID
	end
	select ForumID = @ForumID
end
GO

-- yaf_rank_save
if exists (select * from sysobjects where id = object_id(N'yaf_rank_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_rank_save
GO

create procedure yaf_rank_save(
	@RankID		int,
	@BoardID	int,
	@Name		nvarchar(50),
	@IsStart	bit,
	@IsLadder	bit,
	@MinPosts	int,
	@RankImage	nvarchar(50)=null
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
		insert into yaf_Rank(BoardID,Name,IsStart,IsLadder,MinPosts,RankImage)
		values(@BoardID,@Name,@IsStart,@IsLadder,@MinPosts,@RankImage);
	end
end
GO

-- yaf_message_save
if exists (select * from sysobjects where id = object_id(N'yaf_message_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_save
GO

create procedure yaf_message_save(
	@TopicID	int,
	@UserID		int,
	@Message	ntext,
	@UserName	nvarchar(50)=null,
	@IP			nvarchar(15),
	@Posted		datetime=null,
	@ReplyTo	int,
	@MessageID	int output
) as
begin
	declare @ForumID	int
	declare	@Moderated	bit
	declare @Position	int
	declare	@Indent		int

	if @Posted is null set @Posted = getdate()

	select @ForumID = x.ForumID, @Moderated = y.Moderated from yaf_Topic x,yaf_Forum y where x.TopicID = @TopicID and y.ForumID=x.ForumID

	if @ReplyTo is null
	begin
		-- New thread
		set @Position = 0
		set @Indent = 0
	end else if @ReplyTo<0
	begin
		-- Find post to reply to and indent of this post
		select top 1 @ReplyTo=MessageID,@Indent=Indent+1
		from yaf_Message 
		where TopicID=@TopicID and ReplyTo is null
		order by Posted
	end else
	begin
		-- Got reply, find indent of this post
		select @Indent=Indent+1
		from yaf_Message 
		where MessageID=@ReplyTo
	end

	-- Find position
	if @ReplyTo is not null
	begin
		declare @temp int
		
		select @temp=ReplyTo,@Position=Position from yaf_Message where MessageID=@ReplyTo
		if @temp is null
			-- We are replying to first post
			select @Position=max(Position)+1 from yaf_Message where TopicID=@TopicID
		else
			-- Last position of replies to parent post
			select @Position=min(Position) from yaf_Message where ReplyTo=@temp and Position>@Position
		-- No replies, then use parent post's position+1
		if @Position is null select @Position=Position+1 from yaf_Message where MessageID=@ReplyTo
		-- Increase position of posts after this
		update yaf_Message set Position=Position+1 where TopicID=@TopicID and Position>=@Position
	end

	insert into yaf_Message(UserID,Message,TopicID,Posted,UserName,IP,Approved,ReplyTo,Position,Indent)
	values(@UserID,@Message,@TopicID,@Posted,@UserName,@IP,0,@ReplyTo,@Position,@Indent)
	set @MessageID = @@IDENTITY
	
	if @Moderated=0
		exec yaf_message_approve @MessageID
end
GO

-- yaf_topic_save
if exists (select * from sysobjects where id = object_id(N'yaf_topic_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_save
GO

create procedure yaf_topic_save(
	@ForumID	int,
	@Subject	nvarchar(100),
	@UserID		int,
	@Message	ntext,
	@Priority	smallint,
	@UserName	nvarchar(50)=null,
	@IP			nvarchar(15),
	@PollID		int=null,
	@Posted		datetime=null
) as
begin
	declare @TopicID int
	declare @MessageID int

	if @Posted is null set @Posted = getdate()

	insert into yaf_Topic(ForumID,Topic,UserID,Posted,Views,Priority,IsLocked,PollID,UserName,NumPosts)
	values(@ForumID,@Subject,@UserID,@Posted,0,@Priority,0,@PollID,@UserName,0)
	set @TopicID = @@IDENTITY
	exec yaf_message_save @TopicID,@UserID,@Message,@UserName,@IP,@Posted,null,@MessageID output

	select TopicID = @TopicID, MessageID = @MessageID
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
end
GO

-- yaf_user_login
if exists (select * from sysobjects where id = object_id(N'yaf_user_login') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_login
GO

create procedure yaf_user_login(@BoardID int,@Name nvarchar(50),@Password nvarchar(32)) as
begin
	declare @UserID int

	if not exists(select UserID from yaf_User where [Name]=@Name and [Password]=@Password and (BoardID=@BoardID or IsHostAdmin<>0) and Approved<>0)
		set @UserID=null
	else
		select 
			@UserID=UserID 
		from 
			yaf_User 
		where 
			[Name]=@Name and 
			[Password]=@Password and 
			(BoardID=@BoardID or IsHostAdmin<>0) and
			Approved<>0

	select @UserID
end
GO

-- yaf_user_save
if exists (select * from sysobjects where id = object_id(N'yaf_user_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_save
GO

create procedure yaf_user_save(
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
	@Weblog			nvarchar(100) = null
) as
begin
	declare @RankID int

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

	if @UserID is null or @UserID<1 begin
		if @Email = '' set @Email = null
		
		select @RankID = RankID from yaf_Rank where IsStart<>0 and BoardID=@BoardID
		
		insert into yaf_User(BoardID,RankID,Name,Password,Email,Joined,LastVisit,NumPosts,Approved,Location,HomePage,TimeZone,Avatar,Gender,IsHostAdmin) 
		values(@BoardID,@RankID,@UserName,@Password,@Email,getdate(),getdate(),0,@Approved,@Location,@HomePage,@TimeZone,@Avatar,@Gender,0)
	
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
			Weblog = @Weblog
		where UserID = @UserID
		
		if @Email is not null
			update yaf_User set Email = @Email where UserID = @UserID
	end
end
GO

if exists(select 1 from yaf_User where Weblog is not null and Weblog='')
	update yaf_User set Weblog=null where Weblog is not null and Weblog=''
GO

-- yaf_pmessage_save
if exists (select * from sysobjects where id = object_id(N'yaf_pmessage_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pmessage_save
GO

create procedure yaf_pmessage_save(
	@FromUserID	int,
	@ToUserID	int,
	@Subject	nvarchar(100),
	@Body		ntext
) as
begin
	declare @PMessageID int
	declare @UserID int

	insert into yaf_PMessage(FromUserID,Created,Subject,Body)
	values(@FromUserID,getdate(),@Subject,@Body)

	set @PMessageID = @@IDENTITY
	if (@ToUserID = 0)
	begin
		insert into yaf_UserPMessage(UserID,PMessageID,IsRead)
		select
				a.UserID,@PMessageID,0
		from
				yaf_User a
				join yaf_UserGroup b on b.UserID=a.UserID
				join yaf_Group c on c.GroupID=b.GroupID where
				c.IsGuest=0 and
				c.BoardID=(select BoardID from yaf_User x where x.UserID=@FromUserID) and a.UserID<>@FromUserID
		group by
				a.UserID
	end
	else
	begin
		insert into yaf_UserPMessage(UserID,PMessageID,IsRead) values(@ToUserID,@PMessageID,0)
	end
end
GO

-- yaf_pageload
if exists (select * from sysobjects where id = object_id(N'yaf_pageload') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pageload
GO

create procedure yaf_pageload(
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
	
	if @User is null or @User='' 
	begin
		select @UserID = a.UserID from yaf_User a,yaf_UserGroup b,yaf_Group c where a.UserID=b.UserID and a.BoardID=@BoardID and b.GroupID=c.GroupID and c.IsGuest=1
		set @IsGuest = 1
		set @UserBoardID = @BoardID
	end else
	begin
		select @UserID = UserID, @UserBoardID = BoardID from yaf_User where BoardID=@BoardID and [Name]=@User
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
		a.IsHostAdmin,
		UserName			= a.Name,
		Suspended			= a.Suspended,
		ThemeFile			= a.ThemeFile,
		LanguageFile		= a.LanguageFile,
		TimeZoneUser		= a.TimeZone,
		x.*,
		CategoryID			= @CategoryID,
		CategoryName		= (select Name from yaf_Category where CategoryID = @CategoryID),
		ForumID				= @ForumID,
		ForumName			= (select Name from yaf_Forum where ForumID = @ForumID),
		TopicID				= @TopicID,
		TopicName			= (select Topic from yaf_Topic where TopicID = @TopicID),
		MailsPending		= (select count(1) from yaf_Mail),
		Incoming			= (select count(1) from yaf_UserPMessage where UserID=a.UserID and IsRead=0)
	from
		yaf_User a,
		yaf_vaccess x
	where
		a.UserID = @UserID and
		x.UserID = a.UserID and
		x.ForumID = IsNull(@ForumID,0)
end
GO

-- yaf_user_delete
if exists (select * from sysobjects where id = object_id(N'yaf_user_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_delete
GO

create procedure yaf_user_delete(@UserID int) as
begin
	declare @GuestUserID	int
	declare @UserName		nvarchar(50)
	declare @GuestCount		int

	select @UserName = Name from yaf_User where UserID=@UserID

	select top 1
		@GuestUserID = a.UserID
	from
		yaf_User a,
		yaf_UserGroup b,
		yaf_Group c
	where
		b.UserID = a.UserID and
		b.GroupID = c.GroupID and
		c.IsGuest<>0

	select 
		@GuestCount = count(1) 
	from 
		yaf_UserGroup a
		join yaf_Group b on b.GroupID=a.GroupID
	where
		b.IsGuest<>0

	if @GuestUserID=@UserID and @GuestCount=1 begin
		return
	end

	update yaf_Message set UserName=@UserName,UserID=@GuestUserID where UserID=@UserID
	update yaf_Topic set UserName=@UserName,UserID=@GuestUserID where UserID=@UserID
	update yaf_Topic set LastUserName=@UserName,LastUserID=@GuestUserID where LastUserID=@UserID
	update yaf_Forum set LastUserName=@UserName,LastUserID=@GuestUserID where LastUserID=@UserID

	delete from yaf_PMessage where FromUserID=@UserID
	delete from yaf_UserPMessage where UserID=@UserID
	delete from yaf_CheckEmail where UserID = @UserID
	delete from yaf_WatchTopic where UserID = @UserID
	delete from yaf_WatchForum where UserID = @UserID
	delete from yaf_UserGroup where UserID = @UserID
	--ABOT CHANGED
	--Delete UserForums entries Too 
	delete from yaf_UserForum where UserID = @UserID
	--END ABOT CHANGED 09.04.2004
	delete from yaf_User where UserID = @UserID
end
GO

-- yaf_user_find
if exists (select * from sysobjects where id = object_id(N'yaf_user_find') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_find
GO

create procedure yaf_user_find(@BoardID int,@Filter bit,@UserName nvarchar(50)=null,@Email nvarchar(50)=null) as
begin
	if @Filter<>0
	begin
		if @UserName is not null
			set @UserName = '%' + @UserName + '%'

		select 
			a.*,
			IsGuest = (select count(1) from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and x.GroupID=y.GroupID and y.IsGuest<>0)
		from 
			yaf_User a
		where 
			a.BoardID=@BoardID and
			(@UserName is not null and a.Name like @UserName) or (@Email is not null and Email like @Email)
		order by
			a.Name
	end else
	begin
		select 
			a.UserID,
			IsGuest = (select count(1) from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and x.GroupID=y.GroupID and y.IsGuest<>0)
		from 
			yaf_User a
		where 
			a.BoardID=@BoardID and
			((@UserName is not null and a.Name=@UserName) or (@Email is not null and Email=@Email))
	end
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

-- yaf_message_update
if exists (select * from sysobjects where id = object_id(N'yaf_message_update') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_update
GO

create procedure yaf_message_update(@MessageID int,@Priority int,@Message ntext) as
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

-- yaf_pmessage_markread
if exists (select * from sysobjects where id = object_id(N'yaf_pmessage_markread') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_pmessage_markread
GO

CREATE procedure yaf_pmessage_markread(@UserID int,@PMessageID int=null) as begin
	if @PMessageID is null
		update yaf_UserPMessage set IsRead=1 where UserID=@UserID
	else
		update yaf_UserPMessage set IsRead=1 where UserID=@UserID and UserPMessageID=@PMessageID
end
GO

-- yaf_attachment_list
if exists (select * from sysobjects where id = object_id(N'yaf_attachment_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_attachment_list
GO

create procedure yaf_attachment_list(@MessageID int=null,@AttachmentID int=null,@BoardID int=null) as begin
	if @MessageID is not null
		select * from yaf_Attachment where MessageID=@MessageID
	else if @AttachmentID is not null
		select * from yaf_Attachment where AttachmentID=@AttachmentID
	else
		select 
			a.*,
			Posted		= b.Posted,
			ForumID		= d.ForumID,
			ForumName	= d.Name,
			TopicID		= c.TopicID,
			TopicName	= c.Topic
		from 
			yaf_Attachment a,
			yaf_Message b,
			yaf_Topic c,
			yaf_Forum d,
			yaf_Category e
		where
			b.MessageID = a.MessageID and
			c.TopicID = b.TopicID and
			d.ForumID = c.ForumID and
			e.CategoryID = d.CategoryID and
			e.BoardID = @BoardID
		order by
			d.Name,
			c.Topic,
			b.Posted
end
GO

-- yaf_nntpserver_list
if exists (select * from sysobjects where id = object_id(N'yaf_nntpserver_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_nntpserver_list
GO

create procedure yaf_nntpserver_list(@BoardID int=null,@NntpServerID int=null) as
begin
	if @NntpServerID is null
		select * from yaf_NntpServer where BoardID=@BoardID order by Name
	else
		select * from yaf_NntpServer where NntpServerID=@NntpServerID
end
GO

-- yaf_nntpforum_list
if exists (select * from sysobjects where id = object_id(N'yaf_nntpforum_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_nntpforum_list
GO

create procedure yaf_nntpforum_list(@BoardID int,@Minutes int=null,@NntpForumID int=null) as
begin
	select
		a.Name,
		a.Address,
		Port = IsNull(a.Port,119),
		a.NntpServerID,
		b.NntpForumID,
		b.GroupName,
		b.ForumID,
		b.LastMessageNo,
		b.LastUpdate,
		ForumName = c.Name
	from
		yaf_NntpServer a,
		yaf_NntpForum b,
		yaf_Forum c
	where
		b.NntpServerID = a.NntpServerID and
		(@Minutes is null or datediff(n,b.LastUpdate,getdate())>@Minutes) and
		(@NntpForumID is null or b.NntpForumID=@NntpForumID) and
		c.ForumID = b.ForumID and
		a.BoardID=@BoardID
	order by
		a.Name,
		b.GroupName
end
GO

