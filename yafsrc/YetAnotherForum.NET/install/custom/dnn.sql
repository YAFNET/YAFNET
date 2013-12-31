/*
** DNN Custom SQL Procedures
*/

IF  EXISTS (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}YafDnn_Messages]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}YafDnn_Messages]
GO

IF  EXISTS (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}YafDnn_Topics]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}YafDnn_Topics]
GO

IF  EXISTS (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}GetReadAccessListForForum]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}GetReadAccessListForForum]
GO

/** Create Stored Procedures **/

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}YafDnn_Messages]
	AS
	SET NOCOUNT ON
	SELECT
		Message, MessageID, TopicID, Posted
	FROM [{databaseOwner}].[{objectQualifier}Message]
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}YafDnn_Topics]
	AS
	SET NOCOUNT ON
	SELECT
		Topic, TopicID, ForumID, Posted
	FROM [{databaseOwner}].[{objectQualifier}Topic]

GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}YafDnn_LastUpdatedProfile](
  @UserID int)
	AS
	SELECT TOP 1
		LastUpdatedDate
	FROM {databaseOwner}{objectQualifier}UserProfile
	WHERE UserID=@UserID
	order by LastUpdatedDate DESC
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}GetReadAccessListForForum](
  @ForumID int)
	AS
	select fa.GroupID, GroupName = g.Name, AccessMaskName = am.Name, am.Flags
	from [{databaseOwner}].[{objectQualifier}ForumAccess] fa
	Inner join [{databaseOwner}].[{objectQualifier}AccessMask] am
    on (fa.AccessMaskID = am.AccessMaskID)
	inner join [{databaseOwner}].[{objectQualifier}Group] g
	on (fa.GroupID = g.GroupID)
	where fa.ForumID = @ForumID
GO