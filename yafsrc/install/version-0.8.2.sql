/* Version 0.8.2 */

if not exists (select * from sysobjects where id = object_id(N'yaf_Attachment') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
create table [yaf_Attachment](
	[AttachmentID]	[int] identity not null,
	[MessageID]		[int] not null,
	[FileName]		[nvarchar](250) not null,
	[Bytes]			[int] not null,
	[FileID]		[int] null,
	[ContentType]	[nvarchar](50) null
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

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='BlankLinks')
	alter table yaf_System add BlankLinks bit not null default(0)
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='SmtpUserName')
	alter table yaf_System add SmtpUserName nvarchar(50) null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='SmtpUserPass')
	alter table yaf_System add SmtpUserPass nvarchar(50) null
GO
