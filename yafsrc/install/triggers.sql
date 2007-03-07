/*
** Triggers
*/

if exists(select 1 from sysobjects where id=object_id(N'yaf_Active_insert') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger yaf_Active_insert
go

create trigger yaf_Active_insert on dbo.yaf_Active for insert as
begin
	declare @BoardID int, @count int, @max int, @maxStr nvarchar(255), @countStr nvarchar(255), @dtStr nvarchar(255)

	-- Assumes only one row was inserted - shouldn't be a problem?
	select @BoardID = BoardID from inserted
	
	select @count = count(distinct IP) from yaf_Active with(nolock) where BoardID=@BoardID
	select @maxStr = cast(Value as nvarchar) from yaf_Registry where BoardID=@BoardID and Name=N'maxusers'
	select @max = cast(@maxStr as int)
	select @countStr = cast(@count as nvarchar)
	select @dtStr = convert(nvarchar,getdate(),126)

	if @@rowcount=0
	begin
		insert into yaf_Registry(BoardID,Name,Value) values(@BoardID,N'maxusers',cast(@countStr as ntext))
		insert into yaf_Registry(BoardID,Name,Value) values(@BoardID,N'maxuserswhen',cast(@dtStr as ntext))
	end else if @count>@max
	begin
		update yaf_Registry set Value=cast(@countStr as ntext) where BoardID=@BoardID and Name=N'maxusers'
		update yaf_Registry set Value=cast(@dtStr as ntext) where BoardID=@BoardID and Name=N'maxuserswhen'
	end
end
go

if exists(select 1 from sysobjects where id=object_id(N'yaf_Forum_update') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger yaf_Forum_update
go

CREATE TRIGGER yaf_Forum_update ON dbo.yaf_Forum FOR UPDATE AS
BEGIN
	IF UPDATE(LastTopicID) OR UPDATE(LastMessageID)
	BEGIN	
		-- recursively update the forum
		DECLARE @ParentID int		

		SET @ParentID = (SELECT TOP 1 ParentID FROM inserted)
		
		WHILE (@ParentID IS NOT NULL)
		BEGIN
			UPDATE a SET
				a.LastPosted = b.LastPosted,
				a.LastTopicID = b.LastTopicID,
				a.LastMessageID = b.LastMessageID,
				a.LastUserID = b.LastUserID,
				a.LastUserName = b.LastUserName
			FROM
				yaf_Forum a, inserted b
			WHERE
				a.ForumID = @ParentID AND ((a.LastPosted < b.LastPosted) OR a.LastPosted IS NULL);
			
			SET @ParentID = (SELECT ParentID FROM yaf_Forum WHERE ForumID = @ParentID)
		END
	END
END
GO

if exists(select 1 from sysobjects where id=object_id(N'yaf_Group_update') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger yaf_Group_update
GO

if exists(select 1 from sysobjects where id=object_id(N'yaf_Group_insert') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger yaf_Group_insert
GO

if exists(select 1 from sysobjects where id=object_id(N'yaf_UserGroup_insert') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger yaf_UserGroup_insert
GO

if exists(select 1 from sysobjects where id=object_id(N'yaf_UserGroup_delete') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger yaf_UserGroup_delete
GO
