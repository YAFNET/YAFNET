/*
  YAF SQL Functions File Created 09/07/2007
	

  Remove Comments RegEx: \/\*(.*)\*\/
  Remove Extra Stuff: SET ANSI_NULLS ON\nGO\nSET QUOTED_IDENTIFIER ON\nGO\n\n\n 
*/

-- scalar functions

IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_bitset]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [{databaseOwner}].[yaf_bitset]
GO

IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_posts]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [{databaseOwner}].[yaf_forum_posts]
GO

IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_topics]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [{databaseOwner}].[yaf_forum_topics]
GO

IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_subforums]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [{databaseOwner}].[yaf_forum_subforums]
GO

IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_lasttopic]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [{databaseOwner}].[yaf_forum_lasttopic]
GO

create function [{databaseOwner}].[yaf_bitset](@Flags int,@Mask int) returns bit as
begin
	declare @bool bit

	if (@Flags & @Mask) = @Mask
		set @bool = 1
	else
		set @bool = 0
		
	return @bool
end
GO

create function [{databaseOwner}].[yaf_forum_posts](@ForumID int) returns int as
begin
	declare @NumPosts int
	declare @tmp int

	select @NumPosts=NumPosts from {databaseOwner}.yaf_Forum where ForumID=@ForumID

	if exists(select 1 from {databaseOwner}.yaf_Forum where ParentID=@ForumID)
	begin
		declare c cursor for
		select ForumID from {databaseOwner}.yaf_Forum
		where ParentID = @ForumID
		
		open c
		
		fetch next from c into @tmp
		while @@FETCH_STATUS = 0
		begin
			set @NumPosts=@NumPosts+{databaseOwner}.yaf_forum_posts(@tmp)
			fetch next from c into @tmp
		end
		close c
		deallocate c
	end

	return @NumPosts
end
GO

create function [{databaseOwner}].[yaf_forum_topics](@ForumID int) returns int as
begin
	declare @NumTopics int
	declare @tmp int

	select @NumTopics=NumTopics from {databaseOwner}.yaf_Forum where ForumID=@ForumID

	if exists(select 1 from {databaseOwner}.yaf_Forum where ParentID=@ForumID)
	begin
		declare c cursor for
		select ForumID from {databaseOwner}.yaf_Forum
		where ParentID = @ForumID
		
		open c
		
		fetch next from c into @tmp
		while @@FETCH_STATUS = 0
		begin
			set @NumTopics=@NumTopics+{databaseOwner}.yaf_forum_topics(@tmp)
			fetch next from c into @tmp
		end
		close c
		deallocate c
	end

	return @NumTopics
end
GO

CREATE function [{databaseOwner}].[yaf_forum_subforums](@ForumID int, @UserID int) returns int as
begin
	declare @NumSubforums int

	select 
		@NumSubforums=COUNT(1)	
	from 
		yaf_Forum a 
		join yaf_vaccess x on x.ForumID = a.ForumID 
	where 
		((a.Flags & 2)=0 or x.ReadAccess<>0) and 
		(a.ParentID=@ForumID) and	
		(x.UserID = @UserID)

	return @NumSubforums
end
GO

CREATE FUNCTION [{databaseOwner}].[yaf_forum_lasttopic] 
(	
	@ForumID int,
	@UserID int = null,
	@LastTopicID int = null,
	@LastPosted datetime = null
) RETURNS int AS
BEGIN
	-- local variables for temporary values
	declare @SubforumID int
	declare @TopicID int
	declare @Posted datetime

	-- try to retrieve last direct topic posed in forums if not supplied as argument 
	if (@LastTopicID is null or @LastPosted is null) begin
		SELECT 
			@LastTopicID=a.LastTopicID,
			@LastPosted=a.LastPosted
		FROM
			yaf_Forum a
			JOIN yaf_vaccess x ON a.ForumID=x.ForumID
		WHERE
			a.ForumID=@ForumID and
			(
				(@UserID is null and (a.Flags & 2)=0) or 
				(x.UserID=@UserID and ((a.Flags & 2)=0 or x.ReadAccess<>0))
			)
	end

	-- look for newer topic/message in subforums
	if exists(select 1 from {databaseOwner}.yaf_Forum where ParentID=@ForumID)
	begin
		declare c cursor for
			SELECT 
				a.ForumID,
				a.LastTopicID,
				a.LastPosted
			FROM
				yaf_Forum a
				JOIN yaf_vaccess x ON a.ForumID=x.ForumID
			WHERE
				a.ParentID=@ForumID and
				(
					(@UserID is null and (a.Flags & 2)=0) or 
					(x.UserID=@UserID and ((a.Flags & 2)=0 or x.ReadAccess<>0))
				)
			
		open c
		
		-- cycle through subforums
		fetch next from c into @SubforumID, @TopicID, @Posted
		while @@FETCH_STATUS = 0
		begin
			-- get last topic/message info for subforum
			SELECT 
				@TopicID = LastTopicID,
				@Posted = LastPosted
			FROM
				{databaseOwner}.yaf_forum_lastposted(@SubforumID, @UserID, @TopicID, @Posted)

			-- if subforum has newer topic/message, make it last for parent forum
			if (@TopicID is not null and @Posted is not null and @LastPosted < @Posted) begin
				SET @LastTopicID = @TopicID
				SET @LastPosted = @Posted
			end

			fetch next from c into @SubforumID, @TopicID, @Posted
		end
		close c
		deallocate c
	end

	-- return id of topic with last message in this forum or its subforums
	RETURN @LastTopicID
END
GO

-- table-valued functions

IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_lastposted]') AND xtype in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [{databaseOwner}].[yaf_forum_lastposted]
GO

CREATE FUNCTION [{databaseOwner}].[yaf_forum_lastposted] 
(	
	@ForumID int,
	@UserID int = null,
	@LastTopicID int = null,
	@LastPosted datetime = null
)
RETURNS @LastPostInForum TABLE 
(
	LastTopicID int,
	LastPosted datetime
)
AS
BEGIN
	-- local variables for temporary values
	declare @SubforumID int
	declare @TopicID int
	declare @Posted datetime

	-- try to retrieve last direct topic posed in forums if not supplied as argument 
	if (@LastTopicID is null or @LastPosted is null) begin
		SELECT 
			@LastTopicID=a.LastTopicID,
			@LastPosted=a.LastPosted
		FROM
			yaf_Forum a
			JOIN yaf_vaccess x ON a.ForumID=x.ForumID
		WHERE
			a.ForumID=@ForumID and
			(
				(@UserID is null and (a.Flags & 2)=0) or 
				(x.UserID=@UserID and ((a.Flags & 2)=0 or x.ReadAccess<>0))
			)
	end

	-- look for newer topic/message in subforums
	if exists(select 1 from {databaseOwner}.yaf_Forum where ParentID=@ForumID)
	begin
		declare c cursor for
			SELECT 
				a.ForumID,
				a.LastTopicID,
				a.LastPosted
			FROM
				yaf_Forum a
				JOIN yaf_vaccess x ON a.ForumID=x.ForumID
			WHERE
				a.ParentID=@ForumID and
				(
					(@UserID is null and (a.Flags & 2)=0) or 
					(x.UserID=@UserID and ((a.Flags & 2)=0 or x.ReadAccess<>0))
				)
			
		open c
		
		-- cycle through subforums
		fetch next from c into @SubforumID, @TopicID, @Posted
		while @@FETCH_STATUS = 0
		begin
			-- get last topic/message info for subforum
			SELECT 
				@TopicID = LastTopicID,
				@Posted = LastPosted
			FROM
				{databaseOwner}.yaf_forum_lastposted(@SubforumID, @UserID, @TopicID, @Posted)

			-- if subforum has newer topic/message, make it last for parent forum
			if (@TopicID is not null and @Posted is not null and @LastPosted < @Posted) begin
				SET @LastTopicID = @TopicID
				SET @LastPosted = @Posted
			end

			fetch next from c into @SubforumID, @TopicID, @Posted
		end
		close c
		deallocate c
	end

	-- return vector
	INSERT @LastPostInForum
	SELECT 
		@LastTopicID,
		@LastPosted
	RETURN
END;
