/*
  YAF SQL Functions File Created 09/07/2007
	

  Remove Comments RegEx: \/\*(.*)\*\/
  Remove Extra Stuff: SET ANSI_NULLS ON\nGO\nSET QUOTED_IDENTIFIER ON\nGO\n\n\n 
*/

-- scalar functions

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bitset]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [{databaseOwner}].[{objectQualifier}bitset]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_bitset]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [{databaseOwner}].[yaf_bitset]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_posts]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [{databaseOwner}].[{objectQualifier}forum_posts]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_posts]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [{databaseOwner}].[yaf_forum_posts]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_topics]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [{databaseOwner}].[{objectQualifier}forum_topics]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_topics]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [{databaseOwner}].[yaf_forum_topics]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_subforums]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [{databaseOwner}].[{objectQualifier}forum_subforums]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_subforums]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [{databaseOwner}].[yaf_forum_subforums]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_lasttopic]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [{databaseOwner}].[{objectQualifier}forum_lasttopic]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_lasttopic]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [{databaseOwner}].[yaf_forum_lasttopic]
>>>>>>> .r1490
GO

<<<<<<< .mine
create function [{databaseOwner}].[{objectQualifier}bitset](@Flags int,@Mask int) returns bit as
=======
create function [{databaseOwner}].[yaf_bitset](@Flags int,@Mask int) returns bit as
>>>>>>> .r1490
begin
	declare @bool bit

	if (@Flags & @Mask) = @Mask
		set @bool = 1
	else
		set @bool = 0
		
	return @bool
end
GO

<<<<<<< .mine
create function [{databaseOwner}].[{objectQualifier}forum_posts](@ForumID int) returns int as
=======
create function [{databaseOwner}].[yaf_forum_posts](@ForumID int) returns int as
>>>>>>> .r1490
begin
	declare @NumPosts int
	declare @tmp int

<<<<<<< .mine
	select @NumPosts=NumPosts from [{databaseOwner}].[{objectQualifier}Forum] where ForumID=@ForumID
=======
	select @NumPosts=NumPosts from {databaseOwner}.yaf_Forum where ForumID=@ForumID
>>>>>>> .r1490

<<<<<<< .mine
	if exists(select 1 from [{databaseOwner}].[{objectQualifier}Forum] where ParentID=@ForumID)
=======
	if exists(select 1 from {databaseOwner}.yaf_Forum where ParentID=@ForumID)
>>>>>>> .r1490
	begin
		declare c cursor for
<<<<<<< .mine
		select ForumID from [{databaseOwner}].[{objectQualifier}Forum]
=======
		select ForumID from {databaseOwner}.yaf_Forum
>>>>>>> .r1490
		where ParentID = @ForumID
		
		open c
		
		fetch next from c into @tmp
		while @@FETCH_STATUS = 0
		begin
<<<<<<< .mine
			set @NumPosts=@NumPosts+[{databaseOwner}].[{objectQualifier}forum_posts](@tmp)
=======
			set @NumPosts=@NumPosts+{databaseOwner}.yaf_forum_posts(@tmp)
>>>>>>> .r1490
			fetch next from c into @tmp
		end
		close c
		deallocate c
	end

	return @NumPosts
end
GO

<<<<<<< .mine
create function [{databaseOwner}].[{objectQualifier}forum_topics](@ForumID int) returns int as
=======
create function [{databaseOwner}].[yaf_forum_topics](@ForumID int) returns int as
>>>>>>> .r1490
begin
	declare @NumTopics int
	declare @tmp int

<<<<<<< .mine
	select @NumTopics=NumTopics from [{databaseOwner}].[{objectQualifier}Forum] where ForumID=@ForumID
=======
	select @NumTopics=NumTopics from {databaseOwner}.yaf_Forum where ForumID=@ForumID
>>>>>>> .r1490

<<<<<<< .mine
	if exists(select 1 from [{databaseOwner}].[{objectQualifier}Forum] where ParentID=@ForumID)
=======
	if exists(select 1 from {databaseOwner}.yaf_Forum where ParentID=@ForumID)
>>>>>>> .r1490
	begin
		declare c cursor for
<<<<<<< .mine
		select ForumID from [{databaseOwner}].[{objectQualifier}Forum]
=======
		select ForumID from {databaseOwner}.yaf_Forum
>>>>>>> .r1490
		where ParentID = @ForumID
		
		open c
		
		fetch next from c into @tmp
		while @@FETCH_STATUS = 0
		begin
<<<<<<< .mine
			set @NumTopics=@NumTopics+[{databaseOwner}].[{objectQualifier}forum_topics](@tmp)
=======
			set @NumTopics=@NumTopics+{databaseOwner}.yaf_forum_topics(@tmp)
>>>>>>> .r1490
			fetch next from c into @tmp
		end
		close c
		deallocate c
	end

	return @NumTopics
end
GO

<<<<<<< .mine
CREATE function [{databaseOwner}].[{objectQualifier}forum_subforums](@ForumID int, @UserID int) returns int as
=======
CREATE function [{databaseOwner}].[yaf_forum_subforums](@ForumID int, @UserID int) returns int as
>>>>>>> .r1490
begin
	declare @NumSubforums int

	select 
		@NumSubforums=COUNT(1)	
	from 
		{objectQualifier}Forum a 
		join {objectQualifier}vaccess x on x.ForumID = a.ForumID 
	where 
		((a.Flags & 2)=0 or x.ReadAccess<>0) and 
		(a.ParentID=@ForumID) and	
		(x.UserID = @UserID)

	return @NumSubforums
end
GO

<<<<<<< .mine
CREATE FUNCTION [{databaseOwner}].[{objectQualifier}forum_lasttopic] 
=======
CREATE FUNCTION [{databaseOwner}].[yaf_forum_lasttopic] 
>>>>>>> .r1490
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
			{objectQualifier}Forum a
			JOIN {objectQualifier}vaccess x ON a.ForumID=x.ForumID
		WHERE
			a.ForumID=@ForumID and
			(
				(@UserID is null and (a.Flags & 2)=0) or 
				(x.UserID=@UserID and ((a.Flags & 2)=0 or x.ReadAccess<>0))
			)
	end

	-- look for newer topic/message in subforums
<<<<<<< .mine
	if exists(select 1 from [{databaseOwner}].[{objectQualifier}Forum] where ParentID=@ForumID)
=======
	if exists(select 1 from {databaseOwner}.yaf_Forum where ParentID=@ForumID)
>>>>>>> .r1490
	begin
		declare c cursor for
			SELECT 
				a.ForumID,
				a.LastTopicID,
				a.LastPosted
			FROM
				{objectQualifier}Forum a
				JOIN {objectQualifier}vaccess x ON a.ForumID=x.ForumID
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
<<<<<<< .mine
				[{databaseOwner}].[{objectQualifier}forum_lastposted](@SubforumID, @UserID, @TopicID, @Posted)
=======
				{databaseOwner}.yaf_forum_lastposted(@SubforumID, @UserID, @TopicID, @Posted)
>>>>>>> .r1490

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

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_lastposted]') AND xtype in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [{databaseOwner}].[{objectQualifier}forum_lastposted]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_lastposted]') AND xtype in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [{databaseOwner}].[yaf_forum_lastposted]
>>>>>>> .r1490
GO

<<<<<<< .mine
CREATE FUNCTION [{databaseOwner}].[{objectQualifier}forum_lastposted] 
=======
CREATE FUNCTION [{databaseOwner}].[yaf_forum_lastposted] 
>>>>>>> .r1490
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
			{objectQualifier}Forum a
			JOIN {objectQualifier}vaccess x ON a.ForumID=x.ForumID
		WHERE
			a.ForumID=@ForumID and
			(
				(@UserID is null and (a.Flags & 2)=0) or 
				(x.UserID=@UserID and ((a.Flags & 2)=0 or x.ReadAccess<>0))
			)
	end

	-- look for newer topic/message in subforums
<<<<<<< .mine
	if exists(select 1 from [{databaseOwner}].[{objectQualifier}Forum] where ParentID=@ForumID)
=======
	if exists(select 1 from {databaseOwner}.yaf_Forum where ParentID=@ForumID)
>>>>>>> .r1490
	begin
		declare c cursor for
			SELECT 
				a.ForumID,
				a.LastTopicID,
				a.LastPosted
			FROM
				{objectQualifier}Forum a
				JOIN {objectQualifier}vaccess x ON a.ForumID=x.ForumID
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
<<<<<<< .mine
				[{databaseOwner}].[{objectQualifier}forum_lastposted](@SubforumID, @UserID, @TopicID, @Posted)
=======
				{databaseOwner}.yaf_forum_lastposted(@SubforumID, @UserID, @TopicID, @Posted)
>>>>>>> .r1490

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
