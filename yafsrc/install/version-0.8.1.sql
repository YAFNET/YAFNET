/* Version 0.8.1 */

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='ShowMoved')
	alter table yaf_System add ShowMoved bit not null default(1)
GO

if exists (select * from sysobjects where id = object_id(N'yaf_poll_stats') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_poll_stats
GO

create procedure yaf_poll_stats(@PollID int) as
begin
	select
		a.PollID,
		b.Question,
		a.ChoiceID,
		a.Choice,
		a.Votes,
		Stats = (select 100 * a.Votes / case sum(x.Votes) when 0 then 1 else sum(x.Votes) end from yaf_Choice x where x.PollID=a.PollID)
	from
		yaf_Choice a,
		yaf_Poll b
	where
		b.PollID = a.PollID and
		b.PollID = @PollID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_smiley_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_smiley_delete
GO

create procedure yaf_smiley_delete(@SmileyID int=null) as begin
	if @SmileyID is not null
		delete from yaf_Smiley where SmileyID=@SmileyID
	else
		delete from yaf_Smiley
end
GO
