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
