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

if exists (select * from sysobjects where id = object_id(N'yaf_poll_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_poll_save
GO

create procedure yaf_poll_save(
	@Question	varchar(50),
	@Choice1	varchar(50),
	@Choice2	varchar(50),
	@Choice3	varchar(50) = null,
	@Choice4	varchar(50) = null,
	@Choice5	varchar(50) = null,
	@Choice6	varchar(50) = null,
	@Choice7	varchar(50) = null,
	@Choice8	varchar(50) = null,
	@Choice9	varchar(50) = null
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

if not exists(select * from syscolumns where id=object_id('yaf_Group') and name='RankImage')
	alter table yaf_Group add RankImage varchar(50) null
GO
