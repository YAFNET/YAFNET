/*
** Triggers
*/

if exists(select 1 from sysobjects where id=object_id(N'[{databaseOwner}].{objectQualifier}Active_insert') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger [{databaseOwner}].{objectQualifier}Active_insert
go

create trigger [{databaseOwner}].{objectQualifier}Active_insert on [{databaseOwner}].{objectQualifier}Active for insert as
begin
	declare @BoardID int, @count int, @max int, @maxStr nvarchar(255), @countStr nvarchar(255), @dtStr nvarchar(255)

	-- Assumes only one row was inserted - shouldn't be a problem?
	select @BoardID = BoardID from inserted
	
	select @count = count(distinct IP) from [{databaseOwner}].{objectQualifier}Active with(nolock) where BoardID=@BoardID
	select @maxStr = cast(Value as nvarchar) from [{databaseOwner}].{objectQualifier}Registry where BoardID=@BoardID and Name=N'maxusers'
	select @max = cast(@maxStr as int)
	select @countStr = cast(@count as nvarchar)
	select @dtStr = convert(nvarchar,getdate(),126)

	if @@rowcount=0
	begin
		insert into [{databaseOwner}].{objectQualifier}Registry(BoardID,Name,Value) values(@BoardID,N'maxusers',cast(@countStr as ntext))
		insert into [{databaseOwner}].{objectQualifier}Registry(BoardID,Name,Value) values(@BoardID,N'maxuserswhen',cast(@dtStr as ntext))
	end else if @count>@max
	begin
		update [{databaseOwner}].{objectQualifier}Registry set Value=cast(@countStr as ntext) where BoardID=@BoardID and Name=N'maxusers'
		update [{databaseOwner}].{objectQualifier}Registry set Value=cast(@dtStr as ntext) where BoardID=@BoardID and Name=N'maxuserswhen'
	end
end
go

if exists(select 1 from sysobjects where id=object_id(N'[{databaseOwner}].{objectQualifier}Forum_update') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger [{databaseOwner}].{objectQualifier}Forum_update
go

/*
CREATE TRIGGER [{databaseOwner}].{objectQualifier}Forum_update ON [{databaseOwner}].{objectQualifier}Forum FOR UPDATE AS
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
				[{databaseOwner}].{objectQualifier}Forum a, inserted b
			WHERE
				a.ForumID = @ParentID AND ((a.LastPosted < b.LastPosted) OR a.LastPosted IS NULL);
			
			SET @ParentID = (SELECT ParentID FROM [{databaseOwner}].{objectQualifier}Forum WHERE ForumID = @ParentID)
		END
	END
END
*/
GO

if exists(select 1 from sysobjects where id=object_id(N'[{databaseOwner}].{objectQualifier}Group_update') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger [{databaseOwner}].{objectQualifier}Group_update
GO

if exists(select 1 from sysobjects where id=object_id(N'[{databaseOwner}].{objectQualifier}Group_insert') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger [{databaseOwner}].{objectQualifier}Group_insert
GO

if exists(select 1 from sysobjects where id=object_id(N'[{databaseOwner}].{objectQualifier}UserGroup_insert') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger [{databaseOwner}].{objectQualifier}UserGroup_insert
GO

if exists(select 1 from sysobjects where id=object_id(N'[{databaseOwner}].{objectQualifier}UserGroup_delete') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger [{databaseOwner}].{objectQualifier}UserGroup_delete
GO
