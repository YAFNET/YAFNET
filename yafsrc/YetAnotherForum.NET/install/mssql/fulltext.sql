-- Enables FULLTEXT support for YAF
-- Must be MANUALLY run against the YAF DB

if (select fulltextserviceproperty('IsFulltextInstalled'))=1 and (select DATABASEPROPERTY(DB_NAME(), N'IsFullTextEnabled')) <> 1 
	exec sp_fulltext_database N'enable' 
GO

if (select DATABASEPROPERTY(DB_NAME(), N'IsFullTextEnabled')) = 1
BEGIN
	if not exists (select * from sys.sysfulltextcatalogs where name = N'YafSearch')
	BEGIN
		EXEC sp_fulltext_catalog N'YafSearch', N'create'
		EXEC sp_fulltext_table N'[{databaseOwner}].[{objectQualifier}Message]', N'create', N'YafSearch', N'PK_yaf_Message'	
		EXEC sp_fulltext_column N'[{databaseOwner}].[{objectQualifier}Message]', N'Message', N'add'
		EXEC sp_fulltext_table N'[{databaseOwner}].[{objectQualifier}Message]', N'activate' 
		EXEC sp_fulltext_table N'[{databaseOwner}].[{objectQualifier}Message]', N'Start_change_tracking'
		EXEC sp_fulltext_table N'[{databaseOwner}].[{objectQualifier}Message]', N'Start_background_updateindex'

		EXEC sp_fulltext_table N'[{databaseOwner}].[{objectQualifier}Topic]', N'create', N'YafSearch', N'PK_yaf_Topic'
		EXEC sp_fulltext_column N'[{databaseOwner}].[{objectQualifier}Topic]', N'Topic', N'add'
		EXEC sp_fulltext_table N'[{databaseOwner}].[{objectQualifier}Topic]', N'activate' 
		EXEC sp_fulltext_table N'[{databaseOwner}].[{objectQualifier}Topic]', N'Start_change_tracking'
		EXEC sp_fulltext_table N'[{databaseOwner}].[{objectQualifier}Topic]', N'Start_background_updateindex'
		
		-- enable in yaf_Registry as a default
		IF EXISTS ( SELECT 1 FROM [{databaseOwner}].[{objectQualifier}Registry] where [Name] = N'usefulltextsearch' )
			UPDATE [{databaseOwner}].[{objectQualifier}Registry] SET [Value] = '1' WHERE [Name] = N'usefulltextsearch'
		ELSE
			INSERT INTO [{databaseOwner}].[{objectQualifier}Registry] ([Name],[Value],[BoardID]) VALUES (N'usefulltextsearch','1',NULL);
	END
END
GO

