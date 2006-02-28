/* Version 1.0.2 */

/*
** Added or Modified columns
*/

if not exists(select 1 from dbo.syscolumns where id = object_id(N'yaf_EventLog') and name=N'Type')
begin
	alter table yaf_EventLog add Type int not null constraint DF_EventLog_Type default (0)
	exec('update yaf_EventLog set Type = 0')
end
GO

if exists(select 1 from dbo.syscolumns where id = object_id(N'yaf_EventLog') and name=N'UserID' and isnullable=0)
	alter table yaf_EventLog alter column UserID int null
GO

-- yaf_eventlog_create
if exists (select 1 from sysobjects where id = object_id(N'yaf_eventlog_create') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_eventlog_create
GO

create procedure dbo.yaf_eventlog_create(@UserID int,@Source nvarchar(50),@Description ntext,@Type int) as
begin
	insert into dbo.yaf_EventLog(UserID,Source,Description,Type)
	values(@UserID,@Source,@Description,@Type)

	-- delete entries older than 10 days
	delete from dbo.yaf_EventLog where EventTime+10<getdate()

	-- or if there are more then 1000	
	if ((select count(*) from yaf_eventlog) >= 1050)
	begin
		/* delete oldest hundred */
		delete from dbo.yaf_EventLog WHERE EventLogID IN (SELECT TOP 100 EventLogID FROM dbo.yaf_EventLog ORDER BY EventTime)
	end	
	
end
GO

-- yaf_eventlog_list
if exists (select 1 from sysobjects where id = object_id(N'yaf_eventlog_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_eventlog_list
GO

create procedure dbo.yaf_eventlog_list(@BoardID int) as
begin
	select
		a.*,
		ISNULL(b.[Name],'System') as [Name]
	from
		dbo.yaf_EventLog a
		left join dbo.yaf_User b on b.UserID=a.UserID
	where
		(b.UserID IS NULL or b.BoardID = @BoardID)		
	order by
		a.EventLogID desc
end
GO

