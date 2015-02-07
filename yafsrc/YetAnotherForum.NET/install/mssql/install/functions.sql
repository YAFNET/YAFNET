/*
  YAF SQL Functions File Created 09/07/2007
    

  Remove Comments RegEx: \/\*(.*)\*\/
  Remove Extra Stuff: SET ANSI_NULLS ON\nGO\nSET QUOTED_IDENTIFIER ON\nGO\n\n\n 
*/

-- scalar functions
CREATE FUNCTION [{databaseOwner}].[{objectQualifier}registry_value] (
    @Name NVARCHAR(64)
    ,@BoardID INT = NULL
    )
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @returnValue NVARCHAR(MAX)

    IF @BoardID IS NOT NULL AND EXISTS(SELECT 1 FROM [{databaseOwner}].[{objectQualifier}Registry] WHERE LOWER([Name]) = LOWER(@Name) AND [BoardID] = @BoardID)
    BEGIN
        SET @returnValue = (
            SELECT CAST([Value] AS NVARCHAR(MAX))
            FROM [{databaseOwner}].[{objectQualifier}Registry]
            WHERE LOWER([Name]) = LOWER(@Name) AND [BoardID] = @BoardID)
    END
    ELSE
    BEGIN
        SET @returnValue = (
            SELECT CAST([Value] AS NVARCHAR(MAX))
            FROM [{databaseOwner}].[{objectQualifier}Registry]
            WHERE LOWER([Name]) = LOWER(@Name) AND [BoardID] IS NULL)
    END

    RETURN @returnValue
END
GO

create function [{databaseOwner}].[{objectQualifier}forum_posts](@ForumID int) returns int as
begin
    declare @NumPosts int
    declare @tmp int

    select @NumPosts=NumPosts from [{databaseOwner}].[{objectQualifier}Forum] where ForumID=@ForumID


    if exists(select 1 from [{databaseOwner}].[{objectQualifier}Forum] where ParentID=@ForumID)

    begin
        declare c cursor for
        select ForumID from [{databaseOwner}].[{objectQualifier}Forum]

        where ParentID = @ForumID
        
        open c
        
        fetch next from c into @tmp
        while @@FETCH_STATUS = 0
        begin
            set @NumPosts=@NumPosts+[{databaseOwner}].[{objectQualifier}forum_posts](@tmp)

            fetch next from c into @tmp
        end
        close c
        deallocate c
    end

    return @NumPosts
end
GO

create function [{databaseOwner}].[{objectQualifier}forum_topics](@ForumID int) returns int as

begin
    declare @NumTopics int
    declare @tmp int

    select @NumTopics=NumTopics from [{databaseOwner}].[{objectQualifier}Forum] where ForumID=@ForumID


    if exists(select 1 from [{databaseOwner}].[{objectQualifier}Forum] where ParentID=@ForumID)

    begin
        declare c cursor for
        select ForumID from [{databaseOwner}].[{objectQualifier}Forum]

        where ParentID = @ForumID
        
        open c
        
        fetch next from c into @tmp
        while @@FETCH_STATUS = 0
        begin
            set @NumTopics=@NumTopics+[{databaseOwner}].[{objectQualifier}forum_topics](@tmp)

            fetch next from c into @tmp
        end
        close c
        deallocate c
    end

    return @NumTopics
end
GO

CREATE FUNCTION [{databaseOwner}].[{objectQualifier}forum_lasttopic] 

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
    if (@LastTopicID is null or @LastPosted is null) BEGIN
        IF (@UserID IS NULL)
        BEGIN	
                SELECT TOP 1 
                    @LastTopicID=a.LastTopicID,
                    @LastPosted=a.LastPosted
                FROM
                    [{databaseOwner}].[{objectQualifier}Forum] a WITH(NOLOCK)
                    INNER JOIN [{databaseOwner}].[{objectQualifier}ActiveAccess] x WITH(NOLOCK) ON a.ForumID=x.ForumID
                WHERE
                    a.ForumID = @ForumID AND a.IsHidden = 0
        END			
        ELSE
        BEGIN	
                SELECT TOP 1
                    @LastTopicID=a.LastTopicID,
                    @LastPosted=a.LastPosted
                FROM
                    [{databaseOwner}].[{objectQualifier}Forum] a WITH(NOLOCK)
                    INNER JOIN [{databaseOwner}].[{objectQualifier}ActiveAccess] x WITH(NOLOCK) ON a.ForumID=x.ForumID
                WHERE
                    (a.IsHidden = 0 or x.ReadAccess <> 0) AND a.ForumID=@ForumID and x.UserID=@UserID
        END	
    END

    -- look for newer topic/message in subforums
    if exists(select 1 from [{databaseOwner}].[{objectQualifier}Forum] where ParentID=@ForumID)
    begin
        declare c cursor FORWARD_ONLY READ_ONLY for
            SELECT
                a.ForumID,
                a.LastTopicID,
                a.LastPosted
            FROM
                [{databaseOwner}].[{objectQualifier}Forum] a WITH(NOLOCK)
                JOIN [{databaseOwner}].[{objectQualifier}ActiveAccess] x WITH(NOLOCK) ON a.ForumID=x.ForumID
            WHERE
                a.ParentID=@ForumID and
                (					
                    (x.UserID=@UserID and ((a.Flags & 2)=0 or x.ReadAccess<>0))
                )	
            UNION			
            SELECT
                a.ForumID,
                a.LastTopicID,
                a.LastPosted
            FROM
                [{databaseOwner}].[{objectQualifier}Forum] a WITH(NOLOCK)
                JOIN [{databaseOwner}].[{objectQualifier}ActiveAccess]x WITH(NOLOCK) ON a.ForumID=x.ForumID
            WHERE
                a.ParentID=@ForumID and
                (					
                    (@UserID is null and (a.Flags & 2)=0)
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
                [{databaseOwner}].[{objectQualifier}forum_lastposted](@SubforumID, @UserID, @TopicID, @Posted)


            -- if subforum has newer topic/message, make it last for parent forum
            if (@TopicID is not null and @Posted is not null and @LastPosted < @Posted) begin
                SET @LastTopicID = @TopicID
                SET @LastPosted = @Posted
            end
            -- workaround to avoid logical expressions with NULL possible differences through SQL server versions. 
            if (@TopicID is not null and @Posted is not null and @LastPosted is null) begin
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

CREATE FUNCTION [{databaseOwner}].[{objectQualifier}forum_lastposted] 

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
    if (@LastTopicID is null or @LastPosted is null) BEGIN
        IF (@UserID IS NULL)
        BEGIN	
                SELECT TOP 1 
                    @LastTopicID=a.LastTopicID,
                    @LastPosted=a.LastPosted
                FROM
                    [{databaseOwner}].[{objectQualifier}Forum] a WITH(NOLOCK)
                    INNER JOIN [{databaseOwner}].[{objectQualifier}ActiveAccess] x WITH(NOLOCK) ON a.ForumID=x.ForumID
                WHERE
                    a.ForumID = @ForumID AND a.IsHidden = 0
        END			
        ELSE
        BEGIN	
                SELECT TOP 1
                    @LastTopicID=a.LastTopicID,
                    @LastPosted=a.LastPosted
                FROM
                    [{databaseOwner}].[{objectQualifier}Forum] a WITH(NOLOCK)
                    INNER JOIN [{databaseOwner}].[{objectQualifier}ActiveAccess] x WITH(NOLOCK) ON a.ForumID=x.ForumID
                WHERE
                    (a.IsHidden = 0 or x.ReadAccess <> 0) AND a.ForumID=@ForumID and x.UserID=@UserID
        END	
    END

    -- look for newer topic/message in subforums
    if exists(select 1 from [{databaseOwner}].[{objectQualifier}Forum] where ParentID=@ForumID)

    begin
        declare c cursor FORWARD_ONLY READ_ONLY for
            SELECT
                a.ForumID,
                a.LastTopicID,
                a.LastPosted
            FROM
                [{databaseOwner}].[{objectQualifier}Forum] a WITH(NOLOCK)
                JOIN [{databaseOwner}].[{objectQualifier}ActiveAccess] x WITH(NOLOCK) ON a.ForumID=x.ForumID
            WHERE
                a.ParentID=@ForumID and
                (					
                    (x.UserID=@UserID and ((a.Flags & 2)=0 or x.ReadAccess<>0))
                )	
            UNION			
            SELECT
                a.ForumID,
                a.LastTopicID,
                a.LastPosted
            FROM
                [{databaseOwner}].[{objectQualifier}Forum] a WITH(NOLOCK)
                JOIN [{databaseOwner}].[{objectQualifier}ActiveAccess]x WITH(NOLOCK) ON a.ForumID=x.ForumID
            WHERE
                a.ParentID=@ForumID and
                (					
                    (@UserID is null and (a.Flags & 2)=0)
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
                [{databaseOwner}].[{objectQualifier}forum_lastposted](@SubforumID, @UserID, @TopicID, @Posted)


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
END
GO

CREATE FUNCTION [{databaseOwner}].[{objectQualifier}medal_getribbonsetting]
(
    @RibbonURL nvarchar(250),
    @Flags int,
    @OnlyRibbon bit
)
RETURNS bit
AS
BEGIN

    if ((@RibbonURL is null) or ((@Flags & 2) = 0)) set @OnlyRibbon = 0

    return @OnlyRibbon

END
GO

CREATE FUNCTION [{databaseOwner}].[{objectQualifier}medal_getsortorder]
(
    @SortOrder tinyint,
    @DefaultSortOrder tinyint,
    @Flags int
)
RETURNS tinyint
AS
BEGIN

    if ((@Flags & 8) = 0) set @SortOrder = @DefaultSortOrder

    return @SortOrder

END
GO

CREATE FUNCTION [{databaseOwner}].[{objectQualifier}medal_gethide]
(
    @Hide bit,
    @Flags int
)
RETURNS bit
AS
BEGIN

    if ((@Flags & 4) = 0) set @Hide = 0

    return @Hide

END
GO

-- Gets the Thanks info which will be formatted and then placed in "dvThanksInfo" Div Tag in displaypost.ascx.
create function [{databaseOwner}].[{objectQualifier}message_getthanksinfo]
(
@MessageID INT,
@ShowThanksDate bit
) returns VARCHAR(MAX)
BEGIN
    DECLARE @Output VARCHAR(MAX)
        SELECT @Output = COALESCE(@Output+',', '') + CAST(i.ThanksFromUserID AS varchar) + 
    CASE @ShowThanksDate WHEN 1 THEN ',' + CAST (i.ThanksDate AS varchar)  ELSE '' end
            FROM	[{databaseOwner}].[{objectQualifier}Thanks] i
            WHERE	i.MessageID = @MessageID	ORDER BY i.ThanksDate
    -- Add the last comma if @Output has data.
    IF @Output <> ''
        SELECT @Output = @Output + ','
    RETURN @Output
END
GO

create function [{databaseOwner}].[{objectQualifier}forum_save_parentschecker](@ForumID int, @ParentID int) returns int as

begin
-- Checks if the forum is already referenced as a parent 
    declare @dependency int
    declare @haschildren int
    declare @frmtmp int
    declare @prntmp int
    
    set @dependency = 0
    set @haschildren = 0
    
    select @dependency=ForumID from [{databaseOwner}].[{objectQualifier}Forum] where ParentID=@ForumID AND ForumID = @ParentID;
    if @dependency > 0
    begin
    return @ParentID
    end

    if exists(select 1 from [{databaseOwner}].[{objectQualifier}Forum] where ParentID=@ForumID)
        begin        
        declare c cursor for
        select ForumID,ParentID from [{databaseOwner}].[{objectQualifier}Forum]
        where ParentID = @ForumID
        
        open c
        
        fetch next from c into @frmtmp,@prntmp
        while @@FETCH_STATUS = 0
        begin
        if @frmtmp > 0 AND @frmtmp IS NOT NULL
         begin        
            set @haschildren= [{databaseOwner}].[{objectQualifier}forum_save_parentschecker](@frmtmp,@ParentID)            
            if  @prntmp = @ParentID
            begin
            set @dependency= @ParentID
            end    
            else if @haschildren > 0
            begin
            set @dependency= @haschildren
            end        
        end
        fetch next from c into @frmtmp,@prntmp
        end
        close c
        deallocate c    
    end
    return @dependency
end
GO

CREATE FUNCTION [{databaseOwner}].[{objectQualifier}Split]
(
   @sInputList VARCHAR(8000) -- List of delimited items
  , @sDelimiter VARCHAR(8000) = ',' -- delimiter that separates items
) RETURNS @List TABLE (item VARCHAR(8000))

    BEGIN
    DECLARE @sItem VARCHAR(8000)
    WHILE CHARINDEX(@sDelimiter,@sInputList,0) <> 0
     BEGIN
     SELECT
      @sItem=RTRIM(LTRIM(SUBSTRING(@sInputList,1,CHARINDEX(@sDelimiter,@sInputList,0)-1))),
      @sInputList=RTRIM(LTRIM(SUBSTRING(@sInputList,CHARINDEX(@sDelimiter,@sInputList,0)+LEN(@sDelimiter),LEN(@sInputList))))
 
     IF LEN(@sItem) > 0
      INSERT INTO @List SELECT @sItem
     END

    IF LEN(@sInputList) > 0
     INSERT INTO @List SELECT @sInputList -- Put the last item in
    RETURN
    END
GO