/*
** Indexes
*/

if exists(select 1 from dbo.sysindexes where name=N'IX_Name' and id=object_id(N'yaf_Registry'))
	drop index dbo.yaf_Registry.IX_Name
go

if not exists(select 1 from dbo.sysindexes where name=N'IX_Name' and id=object_id(N'yaf_Registry'))
	create unique index IX_Name on dbo.yaf_Registry(BoardID,Name)
go

if not exists(select 1 from dbo.sysindexes where name=N'IX_yaf_PollVote_RemoteIP' and id=object_id(N'yaf_PollVote'))
 CREATE  INDEX [IX_yaf_PollVote_RemoteIP] ON [dbo].[yaf_PollVote]([RemoteIP])
GO

if not exists(select 1 from dbo.sysindexes where name=N'IX_yaf_PollVote_UserID' and id=object_id(N'yaf_PollVote'))
 CREATE  INDEX [IX_yaf_PollVote_UserID] ON [dbo].[yaf_PollVote]([UserID])
GO

if not exists(select 1 from dbo.sysindexes where name=N'IX_yaf_PollVote_PollID' and id=object_id(N'yaf_PollVote'))
 CREATE  INDEX [IX_yaf_PollVote_PollID] ON [dbo].[yaf_PollVote]([PollID])
GO

