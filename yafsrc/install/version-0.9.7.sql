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

