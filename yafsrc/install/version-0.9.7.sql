/* Version 0.9.7 */

-- PK_Attachment
if not exists(select * from sysindexes where id=object_id('yaf_Attachment') and name='PK_Attachment')
ALTER TABLE [yaf_Attachment] WITH NOCHECK ADD 
	CONSTRAINT [PK_Attachment] PRIMARY KEY  CLUSTERED 
	(
		[AttachmentID]
	) 
GO

-- yaf_user_login
if exists (select * from sysobjects where id = object_id(N'yaf_user_login') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_login
GO

create procedure yaf_user_login(@BoardID int,@Name varchar(50),@Password varchar(32)) as
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
	@UserName		varchar(50) = null,
	@Password		varchar(32) = null,
	@Email			varchar(50) = null,
	@Hash			varchar(32) = null,
	@Location		varchar(50) = null,
	@HomePage		varchar(50) = null,
	@TimeZone		int,
	@Avatar			varchar(100) = null,
	@LanguageFile	varchar(50) = null,
	@ThemeFile		varchar(50) = null,
	@Approved		bit = null,
	@MSN			varchar(50) = null,
	@YIM			varchar(30) = null,
	@AIM			varchar(30) = null,
	@ICQ			int = null,
	@RealName		varchar(50) = null,
	@Occupation		varchar(50) = null,
	@Interests		varchar(100) = null,
	@Gender			tinyint = 0,
	@Weblog			varchar(100) = null
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
		
		select @RankID = RankID from yaf_Rank where IsStart<>0
		
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

-- yaf_board_delete
if exists (select * from sysobjects where id = object_id(N'yaf_board_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_board_delete
GO

create procedure yaf_board_delete(@BoardID int) as
begin
	delete from yaf_ForumAccess where exists(select 1 from yaf_Group x where x.GroupID=yaf_ForumAccess.GroupID and x.BoardID=@BoardID)
	delete from yaf_Forum where exists(select 1 from yaf_Category x where x.CategoryID=yaf_Forum.CategoryID and x.BoardID=@BoardID)
	delete from yaf_UserGroup where exists(select 1 from yaf_User x where x.UserID=yaf_UserGroup.UserID and x.BoardID=@BoardID)
	delete from yaf_Category where BoardID=@BoardID
	delete from yaf_User where BoardID=@BoardID
	delete from yaf_Rank where BoardID=@BoardID
	delete from yaf_Group where BoardID=@BoardID
	delete from yaf_AccessMask where BoardID=@BoardID
	delete from yaf_Board where BoardID=@BoardID
end
GO

