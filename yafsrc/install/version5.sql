/* Version 0.8.2 */

if not exists(select * from syscolumns where id=object_id('yaf_ForumAccess') and name='UploadAccess')
	alter table yaf_ForumAccess add UploadAccess bit not null default(0)
GO

if exists (select * from sysobjects where id = object_id(N'yaf_forumaccess_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forumaccess_save
GO

create procedure yaf_forumaccess_save(
	@ForumID			int,
	@GroupID			int,
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
	update yaf_ForumAccess set 
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
	where 
		ForumID = @ForumID and 
		GroupID = @GroupID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_forumaccess_repair') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forumaccess_repair
GO

create procedure yaf_forumaccess_repair as
begin
	insert into yaf_ForumAccess(
		GroupID,
		ForumID,
		ReadAccess,
		PostAccess,
		ReplyAccess,
		PriorityAccess,
		PollAccess,
		VoteAccess,
		ModeratorAccess,
		EditAccess,
		DeleteAccess,
		UploadAccess
	)
	select
		b.GroupID,
		a.ForumID,
		0,0,0,0,0,0,0,0,0,0
	from
		yaf_Forum a,
		yaf_Group b
	where
		not exists(select 1 from yaf_ForumAccess x where x.ForumID=a.ForumID and x.GroupID=b.GroupID)
	order by
		a.ForumID,
		b.GroupID
end
GO

if not exists (select * from sysobjects where id = object_id(N'yaf_Attachment') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
create table [yaf_Attachment](
	[AttachmentID]	[int] identity not null,
	[MessageID]	[int] not null,
	[FileName]	[varchar](50) not null,
	[Bytes]		[int] not null
)
GO

if not exists(select * from sysobjects where name='FK_Active_Forum' and parent_obj=object_id('yaf_Active') and OBJECTPROPERTY(id,N'IsForeignKey')=1)
ALTER TABLE [yaf_Attachment] ADD 
	CONSTRAINT [FK_Attachment_Message] FOREIGN KEY 
	(
		[MessageID]
	) REFERENCES [yaf_Message] (
		[MessageID]
	)
GO

if exists (select * from sysobjects where id = object_id(N'yaf_attachment_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_attachment_save
GO

create procedure yaf_attachment_save(@MessageID int,@FileName varchar(50),@Bytes int) as begin
	insert into yaf_Attachment(MessageID,FileName,Bytes) values(@MessageID,@FileName,@Bytes)
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_attachment_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_attachment_list
GO

create procedure yaf_attachment_list(@MessageID int) as begin
	select FileName,Bytes from yaf_Attachment where MessageID=@MessageID
end
go

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='BlankLinks')
	alter table yaf_System add BlankLinks bit not null default(0)
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='SmtpUserName')
	alter table yaf_System add SmtpUserName varchar(50) null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='SmtpUserPass')
	alter table yaf_System add SmtpUserPass varchar(50) null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='BlankLinks')
	alter table yaf_System add BlankLinks bit not null default(0)
GO
