/* Version 0.9.1 */

if not exists(select * from syscolumns where id=object_id('yaf_Group') and name='IsModerator')
	alter table yaf_Group add IsModerator bit null
GO

update yaf_Group set IsModerator=0 where IsModerator is null
GO

alter table yaf_Group alter column IsModerator bit not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='AvatarUpload')
	alter table yaf_System add AvatarUpload bit null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='AvatarRemote')
	alter table yaf_System add AvatarRemote bit null
GO

update yaf_System set AvatarUpload=1,AvatarRemote=1 where AvatarUpload is null and AvatarRemote is null
GO

alter table yaf_System alter column AvatarUpload bit not null
GO

alter table yaf_System alter column AvatarRemote bit not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='AvatarSize')
	alter table yaf_System add AvatarSize int null
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='ShowGroups')
	alter table yaf_System add ShowGroups bit null
GO

update yaf_System set ShowGroups=1 where ShowGroups is null
GO

alter table yaf_System alter column ShowGroups bit not null
GO

if not exists(select * from syscolumns where id=object_id('yaf_WatchForum') and name='LastMail')
	alter table yaf_WatchForum add LastMail datetime null
GO

if not exists(select * from syscolumns where id=object_id('yaf_WatchTopic') and name='LastMail')
	alter table yaf_WatchTopic add LastMail datetime null
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_deleteavatar') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_deleteavatar
GO

create procedure yaf_user_deleteavatar(@UserID int) as begin
	update yaf_User set AvatarImage = null where UserID = @UserID
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_activity_rank') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_activity_rank
GO

create procedure yaf_user_activity_rank(@StartDate as datetime) AS
begin
	select top 3  ID, Name, NumOfPosts from yaf_User u inner join
	(
		select m.UserID as ID, Count(m.UserID) as NumOfPosts from yaf_Message m
		where m.Posted >= @StartDate
		group by m.UserID
	) as counter
	on u.UserID = counter.ID
	order by NumOfPosts desc
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_post_list_reverse10') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_post_list_reverse10
GO

create procedure yaf_post_list_reverse10(@TopicID int) as
begin
	set nocount on

	select top 10
		a.Posted,
		Subject = d.Topic,
		a.Message,
		a.UserID,
		UserName = IsNull(a.UserName,b.Name),
		b.Signature
	from
		yaf_Message a, 
		yaf_User b,
		yaf_Topic d
	where
		a.Approved <> 0 and
		a.TopicID = @TopicID and
		b.UserID = a.UserID and
		d.TopicID = a.TopicID
	order by
		a.Posted desc
end
GO

if exists (select * from sysobjects where id = object_id(N'yaf_message_unapproved') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_message_unapproved
GO

create procedure yaf_message_unapproved(@ForumID int) as begin
	select
		MessageID	= b.MessageID,
		UserName	= IsNull(b.UserName,c.Name),
		Posted		= b.Posted,
		Topic		= a.Topic,
		Message		= b.Message
	from
		yaf_Topic a,
		yaf_Message b,
		yaf_User c
	where
		a.ForumID = @ForumID and
		b.TopicID = a.TopicID and
		b.Approved=0 and
		c.UserID = b.UserID
	order by
		a.Posted
end
GO
