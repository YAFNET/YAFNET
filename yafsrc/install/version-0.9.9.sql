/* Version 0.9.9 */

if not exists (select * from dbo.sysobjects where id = object_id(N'yaf_Replace_Words') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE yaf_Replace_Words(
		[id] [int] IDENTITY (1, 1) NOT NULL ,
		[badword] [nvarchar] (50) NULL ,
		[goodword] [nvarchar] (50) NULL ,
		constraint PK_Replace_Words primary key(id)
	)
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
