/* 0.8.0 */

if exists(select * from syscolumns where id=object_id('yaf_System') and name='Culture')
	alter table yaf_System drop column Culture
GO

if exists(select * from syscolumns where id=object_id('yaf_User') and name='Culture')
	alter table yaf_User drop column Culture
GO

if exists (select * from sysobjects where id = object_id(N'yaf_topic_move') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_move
GO

create procedure yaf_topic_move(@TopicID int,@ForumID int,@ShowMoved bit) as
begin
	if @ShowMoved=1 begin
		-- create a moved message
		insert into yaf_Topic(ForumID,UserID,UserName,Posted,Topic,Views,IsLocked,Priority,PollID,TopicMovedID)
		select ForumID,UserID,UserName,Posted,Topic,0,IsLocked,Priority,PollID,@TopicID
		from yaf_Topic where TopicID = @TopicID
	end

	-- move the topic
	update yaf_Topic set ForumID = @ForumID where TopicID = @TopicID
end
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='EmailVerification')
	alter table yaf_System add EmailVerification bit not null default(1)
GO

if exists (select * from sysobjects where id = object_id(N'yaf_topic_prune') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_topic_prune
GO

create procedure yaf_topic_prune(@ForumID int=null,@Days int) as
begin
	declare @c cursor
	declare @TopicID int
	declare @Count int
	set @Count = 0
	if @ForumID = 0 set @ForumID = null
	if @ForumID is not null begin
		set @c = cursor for
		select 
			TopicID
		from 
			yaf_Topic
		where 
			ForumID = @ForumID and
			Priority = 0 and
			datediff(dd,lastposted,getdate())>@Days
	end
	else begin
		set @c = cursor for
		select 
			TopicID
		from 
			yaf_Topic
		where 
			Priority = 0 and
			datediff(dd,lastposted,getdate())>@Days
	end
	open @c
	fetch @c into @TopicID
	while @@FETCH_STATUS=0 begin
		exec yaf_topic_delete @TopicID
		set @Count = @Count + 1
		fetch @c into @TopicID
	end
	close @c
	deallocate @c
	select Count = @Count
end
GO
