/* in progress */

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='ShowMoved')
	alter table yaf_System add ShowMoved bit not null default(1)
GO

if exists (select * from sysobjects where id = object_id(N'yaf_system_save') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_system_save
GO

create procedure yaf_system_save(
	@Name				varchar(50),
	@TimeZone			int,
	@SmtpServer			varchar(50),
	@ForumEmail			varchar(50),
	@EmailVerification	bit,
	@ShowMoved			bit
) as
begin
	update yaf_System set
		Name = @Name,
		TimeZone = @TimeZone,
		SmtpServer = @SmtpServer,
		ForumEmail = @ForumEmail,
		EmailVerification = @EmailVerification,
		ShowMoved = @ShowMoved
end
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

if exists (select * from sysobjects where id = object_id(N'yaf_user_find') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_find
GO

create procedure yaf_user_find(@UserName varchar(50),@Email varchar(50)) as
begin
	select UserID from yaf_User where Name=@UserName or Email=@Email
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_forum_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_forum_delete
GO

create procedure yaf_forum_delete(@ForumID int) as
begin
	-- Maybe an idea to use cascading foreign keys instead? Too bad they don't work on MS SQL 7.0...
	update yaf_Forum set LastMessageID=null,LastTopicID=null where ForumID=@ForumID
	update yaf_Topic set LastMessageID=null where ForumID=@ForumID
	delete from yaf_WatchTopic from yaf_Topic where yaf_Topic.ForumID = @ForumID and yaf_WatchTopic.TopicID = yaf_Topic.TopicID
	
	delete from yaf_WatchForum where ForumID = @ForumID
	delete from yaf_Message from yaf_Topic where yaf_Topic.ForumID = @ForumID and yaf_Message.TopicID = yaf_Topic.TopicID
	delete from yaf_Topic where ForumID = @ForumID
	delete from yaf_ForumAccess where ForumID = @ForumID
	delete from yaf_Forum where ForumID = @ForumID
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

if not exists(select * from sysindexes where id=object_id('yaf_Smiley') and name='IX_Smiley')
ALTER TABLE [yaf_Smiley] ADD 
	CONSTRAINT [IX_Smiley] UNIQUE  NONCLUSTERED 
	(
		[Code]
	)  ON [PRIMARY] 
GO

if not exists(select 1 from yaf_Smiley where Code=':D')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':D','icon_biggrin.gif','Very Happy')
GO

if not exists(select 1 from yaf_Smiley where Code=':-D')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':-D','icon_biggrin.gif','Very Happy')
GO

if not exists(select 1 from yaf_Smiley where Code=':grin:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':grin:','icon_biggrin.gif','Very Happy')
GO

if not exists(select 1 from yaf_Smiley where Code=':)')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':)','icon_smile.gif','Smile')
GO

if not exists(select 1 from yaf_Smiley where Code=':-)')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':-)','icon_smile.gif','Smile')
GO

if not exists(select 1 from yaf_Smiley where Code=':smile:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':smile:','icon_smile.gif','Smile')
GO

if not exists(select 1 from yaf_Smiley where Code=':(')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':(','icon_sad.gif','Sad')
GO

if not exists(select 1 from yaf_Smiley where Code=':-(')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':-(','icon_sad.gif','Sad')
GO

if not exists(select 1 from yaf_Smiley where Code=':sad:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':sad:','icon_sad.gif','Sad')
GO

if not exists(select 1 from yaf_Smiley where Code=':o')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':o','icon_surprised.gif','Surprised')
GO

if not exists(select 1 from yaf_Smiley where Code=':-o')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':-o','icon_surprised.gif','Surprised')
GO

if not exists(select 1 from yaf_Smiley where Code=':eek:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':eek:','icon_surprised.gif','Surprised')
GO

if not exists(select 1 from yaf_Smiley where Code=':shock:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':shock:','icon_eek.gif','Shocked')
GO

if not exists(select 1 from yaf_Smiley where Code=':?')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':?','icon_confused.gif','Confused')
GO

if not exists(select 1 from yaf_Smiley where Code=':-?')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':-?','icon_confused.gif','Confused')
GO

if not exists(select 1 from yaf_Smiley where Code=':???:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':???:','icon_confused.gif','Confused')
GO

if not exists(select 1 from yaf_Smiley where Code='8)')
insert into yaf_Smiley(Code,Icon,Emoticon) values('8)','icon_cool.gif','Cool')
GO

if not exists(select 1 from yaf_Smiley where Code='8-)')
insert into yaf_Smiley(Code,Icon,Emoticon) values('8-)','icon_cool.gif','Cool')
GO

if not exists(select 1 from yaf_Smiley where Code=':cool:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':cool:','icon_cool.gif','Cool')
GO

if not exists(select 1 from yaf_Smiley where Code=':lol:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':lol:','icon_lol.gif','Laughing')
GO

if not exists(select 1 from yaf_Smiley where Code=':x')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':x','icon_mad.gif','Mad')
GO

if not exists(select 1 from yaf_Smiley where Code=':-x')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':-x','icon_mad.gif','Mad')
GO

if not exists(select 1 from yaf_Smiley where Code=':mad:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':mad:','icon_mad.gif','Mad')
GO

if not exists(select 1 from yaf_Smiley where Code=':P')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':P','icon_razz.gif','Razz')
GO

if not exists(select 1 from yaf_Smiley where Code=':-P')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':-P','icon_razz.gif','Razz')
GO

if not exists(select 1 from yaf_Smiley where Code=':razz:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':razz:','icon_razz.gif','Razz')
GO

if not exists(select 1 from yaf_Smiley where Code=':oops:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':oops:','icon_redface.gif','Embarassed')
GO

if not exists(select 1 from yaf_Smiley where Code=':cry:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':cry:','icon_cry.gif','Crying or Very sad')
GO

if not exists(select 1 from yaf_Smiley where Code=':evil:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':evil:','icon_evil.gif','Evil or Very Mad')
GO

if not exists(select 1 from yaf_Smiley where Code=':twisted:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':twisted:','icon_twisted.gif','Twisted Evil')
GO

if not exists(select 1 from yaf_Smiley where Code=':roll:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':roll:','icon_rolleyes.gif','Rolling Eyes')
GO

if not exists(select 1 from yaf_Smiley where Code=':wink:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':wink:','icon_wink.gif','Wink')
GO

if not exists(select 1 from yaf_Smiley where Code=';)')
insert into yaf_Smiley(Code,Icon,Emoticon) values(';)','icon_wink.gif','Wink')
GO

if not exists(select 1 from yaf_Smiley where Code=';-)')
insert into yaf_Smiley(Code,Icon,Emoticon) values(';-)','icon_wink.gif','Wink')
GO

if not exists(select 1 from yaf_Smiley where Code=':!:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':!:','icon_exclaim.gif','Exclamation')
GO

if not exists(select 1 from yaf_Smiley where Code=':?:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':?:','icon_question.gif','Question')
GO

if not exists(select 1 from yaf_Smiley where Code=':idea:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':idea:','icon_idea.gif','Idea')
GO

if not exists(select 1 from yaf_Smiley where Code=':arrow:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':arrow:','icon_arrow.gif','Arrow')
GO

if not exists(select 1 from yaf_Smiley where Code=':|')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':|','icon_neutral.gif','Neutral')
GO

if not exists(select 1 from yaf_Smiley where Code=':-|')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':-|','icon_neutral.gif','Neutral')
GO

if not exists(select 1 from yaf_Smiley where Code=':neutral:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':neutral:','icon_neutral.gif','Neutral')
GO

if not exists(select 1 from yaf_Smiley where Code=':arrowd:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':arrowd:','icon_arrowd.gif','Arrow Down')
GO

if not exists(select 1 from yaf_Smiley where Code=':arrowl:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':arrowl:','icon_arrowl.gif','Arrow Left')
GO

if not exists(select 1 from yaf_Smiley where Code=':arrowu:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':arrowu:','icon_arrowu.gif','Arrow Up')
GO

if not exists(select 1 from yaf_Smiley where Code=':cheesy:')
insert into yaf_Smiley(Code,Icon,Emoticon) values(':cheesy:','icon_cheesygrin.gif','Cheesy Grin')
GO
