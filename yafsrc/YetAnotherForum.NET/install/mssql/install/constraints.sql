/* Build new constraints */


/***** VIEWS ******/

/****** Object:  Index [{objectQualifier}vaccess_user_UserForum]    Script Date: 09/28/2009 22:30:20 ******/
IF NOT exists (select top 1 1 from sys.indexes WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess_user]') AND name = N'{objectQualifier}vaccess_user_UserForum_PK')
SET ARITHABORT ON
CREATE UNIQUE CLUSTERED INDEX [{objectQualifier}vaccess_user_UserForum_PK] ON [{databaseOwner}].[{objectQualifier}vaccess_user] 
(
	[UserID] ASC,
	[ForumID] ASC,
	[AccessMaskID] ASC,
	[GroupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  Index [{objectQualifier}vaccess_null_UserForum]    Script Date: 09/28/2009 22:30:36 ******/
IF NOT exists (select top 1 1 from sys.indexes WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess_null]') AND name = N'{objectQualifier}vaccess_null_UserForum_PK')
SET ARITHABORT ON
CREATE UNIQUE CLUSTERED INDEX [{objectQualifier}vaccess_null_UserForum_PK] ON [{databaseOwner}].[{objectQualifier}vaccess_null] 
(
	[UserID] ASC,
	[ForumID] ASC,
	[AccessMaskID] ASC,
	[GroupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  Index [{objectQualifier}vaccess_group_UserGroup]    Script Date: 09/28/2009 22:30:55 ******/
IF NOT exists (select top 1 1 from sys.indexes WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}vaccess_group]') AND name = N'{objectQualifier}vaccess_group_UserForum_PK')
SET ARITHABORT ON
CREATE UNIQUE CLUSTERED INDEX [{objectQualifier}vaccess_group_UserForum_PK] ON [{databaseOwner}].[{objectQualifier}vaccess_group] 
(
	[UserID] ASC,
	[ForumID] ASC,
	[AccessMaskID] ASC,
	[GroupID] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO