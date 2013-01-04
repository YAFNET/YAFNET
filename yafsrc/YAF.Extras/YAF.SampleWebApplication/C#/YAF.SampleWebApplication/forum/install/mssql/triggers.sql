/*
** Triggers
*/

if exists(select 1 from sysobjects where id=object_id(N'[{databaseOwner}].[{objectQualifier}Active_insert]') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger [{databaseOwner}].[{objectQualifier}Active_insert]
go

if exists(select 1 from sysobjects where id=object_id(N'[{databaseOwner}].[{objectQualifier}Forum_update]') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger [{databaseOwner}].[{objectQualifier}Forum_update]
go

/*
CREATE TRIGGER [{databaseOwner}].[{objectQualifier}Forum_update] ON [{databaseOwner}].[{objectQualifier}Forum] FOR UPDATE AS
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
				[{databaseOwner}].[{objectQualifier}Forum]] a, inserted b
			WHERE
				a.ForumID = @ParentID AND ((a.LastPosted < b.LastPosted) OR a.LastPosted IS NULL);
			
			SET @ParentID = (SELECT ParentID FROM [{databaseOwner}].[{objectQualifier}Forum] WHERE ForumID = @ParentID)
		END
	END
END
*/
GO

if exists(select 1 from sysobjects where id=object_id(N'[{databaseOwner}].[{objectQualifier}Group_update]') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger [{databaseOwner}].[{objectQualifier}Group_update]
GO

if exists(select 1 from sysobjects where id=object_id(N'[{databaseOwner}].[{objectQualifier}Group_insert]') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger [{databaseOwner}].[{objectQualifier}Group_insert]
GO

if exists(select 1 from sysobjects where id=object_id(N'[{databaseOwner}].[{objectQualifier}UserGroup_insert]') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger [{databaseOwner}].[{objectQualifier}UserGroup_insert]
GO

if exists(select 1 from sysobjects where id=object_id(N'[{databaseOwner}].[{objectQualifier}UserGroup_delete]') and objectproperty(id, N'IsTrigger') = 1)
	drop trigger [{databaseOwner}].[{objectQualifier}UserGroup_delete]
GO

