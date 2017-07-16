/*
** DNN Custom SQL Procedures
*/

-- PRINT 'Temporarily add referring columns to YAF.Net tables:';
IF NOT Exists (SELECT * FROM sys.columns where object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}yaf_Board]')    AND name = N'oModuleID')
   ALTER TABLE [{databaseOwner}].[{objectQualifier}Board] ADD oModuleID     Int Null

IF NOT Exists (SELECT * FROM sys.columns where object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Category]') AND name = N'oGroupID')
   ALTER TABLE [{databaseOwner}].[{objectQualifier}Category] ADD oGroupID   Int Null

IF NOT Exists (SELECT * FROM sys.columns where object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Forum]')    AND name = N'oForumID')
   ALTER TABLE [{databaseOwner}].[{objectQualifier}Forum]    ADD oForumID   Int Null

IF NOT Exists (SELECT * FROM sys.columns where object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Topic]')    AND name = N'oTopicID')
   ALTER TABLE [{databaseOwner}].[{objectQualifier}Topic]    ADD oTopicID   Int Null

IF NOT Exists (SELECT * FROM sys.columns where object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Message]')  AND name = N'oContentID')
   ALTER TABLE [{databaseOwner}].[{objectQualifier}Message]  ADD oContentID Int Null

GO

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


IF  EXISTS (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[RemoveUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[RemoveUser]
GO


CREATE PROCEDURE [{databaseOwner}].[RemoveUser]
	@UserID		int,
	@PortalID   int
AS
    DECLARE @Email  nvarchar(255)
	DECLARE @YafUserID int

	SELECT @Email = Email FROM [{databaseOwner}].[Users] WHERE UserId = @UserID

	IF @PortalID IS NULL
		BEGIN
			-- Delete SuperUser
			DELETE FROM [{databaseOwner}].[Users]
				WHERE  UserId = @UserID
		END
	ELSE
		BEGIN
			-- Remove User from Portal			
			DELETE FROM [{databaseOwner}].[UserPortals]
				WHERE  UserId = @UserID
                 AND PortalId = @PortalID
			IF NOT EXISTS (SELECT 1 FROM [{databaseOwner}].[UserPortals] WHERE  UserId = @UserID) 
				-- Delete User (but not if SuperUser)
				BEGIN
					DELETE FROM [{databaseOwner}].[Users]
						WHERE  UserId = @UserID
							AND IsSuperUser = 0
					DELETE FROM [{databaseOwner}].[UserRoles]
						WHERE  UserID = @UserID
				END								
		END

-- Delete user in YAF.NET
SELECT @YafUserID = UserID FROM [{databaseOwner}].[{objectQualifier}User] WHERE Email = @Email
				   
DELETE FROM [{databaseOwner}].[{objectQualifier}UserAlbum] WHERE UserID = @YafUserID
			   
EXEC [{databaseOwner}].[{objectQualifier}user_delete] @YafUserID

GO