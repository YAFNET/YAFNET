/*
** Triggers
*/
IF EXISTS (
		SELECT 1
		FROM sys.objects
		WHERE object_id = object_id(N'[{databaseOwner}].[{objectQualifier}Active_insert]')
			AND type IN (N'TR')
		)
	DROP TRIGGER [{databaseOwner}].[{objectQualifier}Active_insert]
GO

IF EXISTS (
		SELECT 1
		FROM sys.objects
		WHERE object_id = object_id(N'[{databaseOwner}].[{objectQualifier}Forum_update]')
			AND type IN (N'TR')
		)
	DROP TRIGGER [{databaseOwner}].[{objectQualifier}Forum_update]
GO

IF EXISTS (
		SELECT 1
		FROM sys.objects
		WHERE object_id = object_id(N'[{databaseOwner}].[{objectQualifier}Group_update]')
			AND type IN (N'TR')
		)
	DROP TRIGGER [{databaseOwner}].[{objectQualifier}Group_update]
GO

IF EXISTS (
		SELECT 1
		FROM sys.objects
		WHERE object_id = object_id(N'[{databaseOwner}].[{objectQualifier}Group_insert]')
			AND type IN (N'TR')
		)
	DROP TRIGGER [{databaseOwner}].[{objectQualifier}Group_insert]
GO

IF EXISTS (
		SELECT 1
		FROM sys.objects
		WHERE object_id = object_id(N'[{databaseOwner}].[{objectQualifier}UserGroup_insert]')
			AND type IN (N'TR')
		)
	DROP TRIGGER [{databaseOwner}].[{objectQualifier}UserGroup_insert]
GO

IF EXISTS (
		SELECT 1
		FROM sys.objects
		WHERE object_id = object_id(N'[{databaseOwner}].[{objectQualifier}UserGroup_delete]')
			AND type IN (N'TR')
		)
	DROP TRIGGER [{databaseOwner}].[{objectQualifier}UserGroup_delete]
GO
