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
