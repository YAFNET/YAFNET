-- =============================================
-- Author:		Mek
-- Create date: 30 September 2007
-- Description:	Membership Provider, Roles Provider SPROCS
-- =============================================


-- =============================================
-- Membership Drop Procedures
-- =============================================
IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_upgrade]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_upgrade]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_createapplication]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_createapplication]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_changepassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_changepassword]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_changepasswordquestionandanswer]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_changepasswordquestionandanswer]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_createuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_createuser]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_deleteuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_deleteuser]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_findusersbyemail]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_findusersbyemail]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_findusersbyname]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_findusersbyname]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_getallusers]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getallusers]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_getnumberofusersonline]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getnumberofusersonline]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_getuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getuser]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_getusernamebyemail]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getusernamebyemail]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_resetpassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_resetpassword]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_unlockuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_unlockuser]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_updateuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_updateuser]
GO

-- =============================================
-- Roles Drop Procedures
-- =============================================

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_list]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_delete]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_addusertorole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_addusertorole]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_createrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_createrole]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_deleterole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_deleterole]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_findusersinrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_findusersinrole]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_getroles]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_getroles]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_isuserinrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_isuserinrole]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_removeuserfromrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_removeuserfromrole]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_exists]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_exists]
GO

-- =============================================
-- Profiles Drop Procedures
-- =============================================

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_profile_deleteinactive]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_profile_deleteinactive]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_profile_deleteprofiles]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_profile_deleteprofiles]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_profile_getprofiles]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_profile_getprofiles]
GO

IF  exists(select top 1 1 from dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_profile_getnumberinactiveprofiles]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_profile_getnumberinactiveprofiles]
GO

-- =============================================
-- Membership Create Procedures
-- =============================================

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_upgrade]
(
@PreviousVersion int,
@NewVersion int
)
AS
BEGIN
    		IF (@PreviousVersion = 31) OR (@PreviousVersion = 32)
		BEGIN
			-- RESOLVE SALT ISSUE IN 193 BETA and RC2
			UPDATE [{databaseOwner}].[{objectQualifier}prov_Membership] SET PasswordSalt='UwB5AHMAdABlAG0ALgBCAHkAdABlAFsAXQA=' WHERE PasswordSalt IS NOT NULL;
			UPDATE [{databaseOwner}].[{objectQualifier}prov_Membership] SET Joined=GETUTCDATE()  WHERE Joined IS NULL;
		END	
END 
GO


CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_createapplication]
(
@ApplicationName nvarchar(256),
@ApplicationID uniqueidentifier OUTPUT
)
AS
BEGIN
    	SET @ApplicationID = (SELECT ApplicationID FROM [{databaseOwner}].[{objectQualifier}prov_Application] WHERE ApplicationNameLwd=LOWER(@ApplicationName))
	
	IF (@ApplicationID IS Null)
	BEGIN
		    SELECT  @ApplicationId = NEWID()
            INSERT  [{databaseOwner}].[{objectQualifier}prov_Application] (ApplicationId, ApplicationName, ApplicationNameLwd)
            VALUES  (@ApplicationId, @ApplicationName, LOWER(@Applicationname))
    END
END 
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_changepassword]
(
@ApplicationName nvarchar(256),
@Username nvarchar(256),
@Password nvarchar(256),
@PasswordSalt nvarchar(256),
@PasswordFormat nvarchar(256),
@PasswordAnswer nvarchar(256)
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationId OUTPUT
	
	UPDATE [{databaseOwner}].[{objectQualifier}prov_Membership] SET [Password]=@Password, PasswordSalt=@PasswordSalt,
		PasswordFormat=@PasswordFormat, PasswordAnswer=@PasswordAnswer
	WHERE UsernameLwd=LOWER(@Username) and ApplicationID=@ApplicationID;

END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_changepasswordquestionandanswer]
(
@ApplicationName nvarchar(256),
@Username nvarchar(256),
@PasswordQuestion nvarchar(256),
@PasswordAnswer nvarchar(256)
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier
	
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationId OUTPUT
	
	UPDATE [{databaseOwner}].[{objectQualifier}prov_Membership] SET PasswordQuestion=@PasswordQuestion, PasswordAnswer=@PasswordAnswer
	WHERE UsernameLwd=LOWER(@Username) and ApplicationID=@ApplicationID;

END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_createuser]
(
@ApplicationName nvarchar(256),
@Username nvarchar(256),
@Password nvarchar(256),
@PasswordSalt nvarchar(256) = null,
@PasswordFormat nvarchar(256) = null,
@Email nvarchar(256) = null,
@PasswordQuestion nvarchar(256) = null,
@PasswordAnswer nvarchar(256) = null,
@IsApproved bit = null,
@UserKey nvarchar(64) = null out
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier
	
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationId OUTPUT
	IF @UserKey IS NULL
		SET @UserKey = NEWID()
		
	INSERT INTO [{databaseOwner}].[{objectQualifier}prov_Membership] (UserID,ApplicationID,Joined,Username,UsernameLwd,[Password],PasswordSalt,PasswordFormat,Email,EmailLwd,PasswordQuestion,PasswordAnswer,IsApproved)
		VALUES (@UserKey, @ApplicationID, GETUTCDATE() ,@Username, LOWER(@Username), @Password, @PasswordSalt, @PasswordFormat, @Email, LOWER(@Email), @PasswordQuestion, @PasswordAnswer, @IsApproved);
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_deleteuser]
(
@ApplicationName nvarchar(256),
@Username nvarchar(256),
@DeleteAllRelated bit
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier, @UserID nvarchar(64)
	
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

	-- get the userID
	SELECT @UserID = UserID FROM [{databaseOwner}].[{objectQualifier}prov_Membership] WHERE ApplicationID = @ApplicationID AND UsernameLwd = LOWER(@Username);

	IF (@UserID IS NOT NULL)
	BEGIN
		-- Delete records from membership
		DELETE FROM [{databaseOwner}].[{objectQualifier}prov_Membership] WHERE UserID = @UserID
		-- Delete from Role table
		DELETE FROM [{databaseOwner}].[{objectQualifier}prov_RoleMembership] WHERE UserID = @UserID
		-- Delete from Profile table
		DELETE FROM [{databaseOwner}].[{objectQualifier}prov_Profile] WHERE UserID = @UserID
	END	
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_findusersbyemail]
(
@ApplicationName nvarchar(256),
@EmailAddress nvarchar(256),
@PageIndex int,
@PageSize int
)
AS
BEGIN
    
    -- Set the page bounds
	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

    DECLARE @PagingLowerBoundary int
    DECLARE @PagingUpperBoundary int
    DECLARE @TotalRecords   int
    SET @PagingLowerBoundary = @PageSize * @PageIndex
    SET @PagingUpperBoundary = @PageSize - 1 + @PagingLowerBoundary
    
	CREATE TABLE #RowNumber (RowNumber int IDENTITY (1, 1), UserID nvarchar(64) collate database_default)
	
	INSERT INTO #RowNumber (UserID) SELECT m.UserID FROM [{databaseOwner}].[{objectQualifier}prov_Membership] m INNER JOIN [{databaseOwner}].[{objectQualifier}prov_Application] a ON m.ApplicationID = a.ApplicationID  WHERE a.ApplicationID = @ApplicationID AND m.EmailLwd = LOWER(@EmailAddress)

	SELECT m.*, r.RowNumber FROM [{databaseOwner}].[{objectQualifier}prov_Membership] m INNER JOIN #RowNumber r ON m.UserID = r.UserID WHERE r.RowNumber >= @PagingLowerBoundary AND r.RowNumber <= @PagingUpperBoundary;
    
	SET @TotalRecords = (SELECT COUNT(RowNumber) FROM #RowNumber)
	DROP TABLE #RowNumber
	RETURN @TotalRecords
   
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_findusersbyname]
(
@ApplicationName nvarchar(256),
@Username nvarchar(256),
@PageIndex int,
@PageSize int
)
AS
BEGIN
    
    -- Set the page bounds
	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

    DECLARE @PagingLowerBoundary int
    DECLARE @PagingUpperBoundary int
    DECLARE @TotalRecords   int
    SET @PagingLowerBoundary = @PageSize * @PageIndex
    SET @PagingUpperBoundary = @PageSize - 1 + @PagingLowerBoundary
    
	CREATE TABLE #RowNumber (RowNumber int IDENTITY (1, 1),  UserID nvarchar(64) collate database_default)
	
	INSERT INTO #RowNumber (UserID) SELECT m.UserID FROM [{databaseOwner}].[{objectQualifier}prov_Membership] m INNER JOIN [{databaseOwner}].[{objectQualifier}prov_Application] a ON m.ApplicationID = a.ApplicationID WHERE a.ApplicationID = @ApplicationID AND m.UsernameLwd LIKE '%' + LOWER(@Username) + '%'

	SELECT m.*, r.RowNumber FROM [{databaseOwner}].[{objectQualifier}prov_Membership] m INNER JOIN #RowNumber r ON m.UserID = r.UserID WHERE r.RowNumber >= @PagingLowerBoundary AND r.RowNumber <= @PagingUpperBoundary;
    
	SET @TotalRecords = (SELECT COUNT(RowNumber) FROM #RowNumber)
	DROP TABLE #RowNumber
	RETURN @TotalRecords
   
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getallusers]
(
@ApplicationName nvarchar(256),
@PageIndex int,
@PageSize int
)
AS
BEGIN
    
    -- Set the page bounds
	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

    DECLARE @PagingLowerBoundary int
    DECLARE @PagingUpperBoundary int
    DECLARE @TotalRecords   int
    SET @PagingLowerBoundary = @PageSize * @PageIndex
    SET @PagingUpperBoundary = @PageSize - 1 + @PagingLowerBoundary
    
	CREATE TABLE #RowNumber (RowNumber int IDENTITY (1, 1),  UserID nvarchar(64) collate database_default)
	
	INSERT INTO #RowNumber (UserID) SELECT m.UserID FROM [{databaseOwner}].[{objectQualifier}prov_Membership] m INNER JOIN [{databaseOwner}].[{objectQualifier}prov_Application] a ON m.ApplicationID = a.ApplicationID WHERE a.ApplicationID = @ApplicationID

	SELECT m.*, r.RowNumber FROM [{databaseOwner}].[{objectQualifier}prov_Membership] m INNER JOIN #RowNumber r ON m.UserID = r.UserID WHERE r.RowNumber >= @PagingLowerBoundary AND r.RowNumber <= @PagingUpperBoundary;
    
	SET @TotalRecords = (SELECT COUNT(RowNumber) FROM #RowNumber)
	DROP TABLE #RowNumber
	RETURN @TotalRecords
   
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getnumberofusersonline]
(
@ApplicationName nvarchar(256),
@TimeWindow int,
@CurrentTimeUtc DateTime
)
AS
BEGIN
    	DECLARE @ActivityDate DateTime
	SET @ActivityDate = DATEADD(n, - @TimeWindow, @CurrentTimeUTC)

	DECLARE @ApplicationID uniqueidentifier
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT
	
	DECLARE @NumberActive int
	SET @NumberActive = (SELECT COUNT(1) FROM [{databaseOwner}].[{objectQualifier}prov_Membership] m INNER JOIN [{databaseOwner}].[{objectQualifier}prov_Application] a ON m.ApplicationID = a.ApplicationID  WHERE a.ApplicationID = @ApplicationID AND m.LastLogin >= @ActivityDate)
    
    RETURN @NumberActive

END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getuser]
(
@ApplicationName nvarchar(256),
@Username nvarchar(256) = null,
@UserKey nvarchar(64) = null,
@UserIsOnline bit
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT
	
	IF (@UserKey IS NULL)
		SELECT m.* FROM [{databaseOwner}].[{objectQualifier}prov_Membership] m WHERE m.UsernameLwd = LOWER(@Username) and m.ApplicationID = @ApplicationID
	ELSE
		SELECT m.* FROM [{databaseOwner}].[{objectQualifier}prov_Membership] m WHERE m.UserID = @UserKey and m.ApplicationID = @ApplicationID
	
	-- IF USER IS ONLINE DO AN UPDATE USER	
	IF (@UserIsOnline = 1)
	BEGIN
		UPDATE [{databaseOwner}].[{objectQualifier}prov_Membership] SET LastActivity = GETUTCDATE()  WHERE UsernameLwd = LOWER(@Username) and ApplicationID = @ApplicationID
	END		
END
GO


CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getusernamebyemail]
(
@ApplicationName nvarchar(256),
@Email nvarchar(256)
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

	SELECT m.Username FROM [{databaseOwner}].[{objectQualifier}prov_Membership] m INNER JOIN [{databaseOwner}].[{objectQualifier}prov_Application] a ON m.ApplicationID = a.ApplicationID  WHERE a.ApplicationID = @ApplicationID AND m.EmailLwd = LOWER(@Email);
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_resetpassword]
(
@ApplicationName nvarchar(256),
@UserName nvarchar(256),
@Password nvarchar(256),
@PasswordSalt nvarchar(256),
@PasswordFormat nvarchar(256),
@MaxInvalidAttempts int,
@PasswordAttemptWindow int,
@CurrentTimeUtc datetime
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT
	
	UPDATE [{databaseOwner}].[{objectQualifier}prov_Membership] SET
	[Password] = @Password,
	PasswordSalt = @PasswordSalt,
	PasswordFormat = @PasswordFormat,
	LastPasswordChange = @CurrentTimeUtc
	WHERE ApplicationID = @ApplicationID AND
	UsernameLwd = LOWER(@Username);

END
GO


CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_unlockuser]
(
@ApplicationName nvarchar(256),
@UserName nvarchar(256)
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT
	
	UPDATE [{databaseOwner}].[{objectQualifier}prov_Membership] SET
	IsLockedOut = 0,
	FailedPasswordAttempts = 0
	WHERE ApplicationID = @ApplicationID AND
	UsernameLwd = LOWER(@Username);

END
GO
                
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_updateuser]
(
@ApplicationName nvarchar(256),
@UserKey nvarchar(64),
@UserName nvarchar(256),
@Email nvarchar(256),
@Comment text,
@IsApproved bit,
@LastLogin datetime,
@LastActivity datetime,
@UniqueEmail bit
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT
	
		-- Check UserKey
	IF (@UserKey IS NULL)
        RETURN(1) -- 

	-- Check for UniqueEmail
	IF (@UniqueEmail = 1)
	BEGIN
		IF (EXISTS (SELECT 1 FROM [{databaseOwner}].[{objectQualifier}prov_Membership] m WHERE m.UserID != @UserKey AND m.EmailLwd=LOWER(@Email) AND m.ApplicationID=@ApplicationID) )
			RETURN (2)
	END
	
	UPDATE [{databaseOwner}].[{objectQualifier}prov_Membership] SET
	Username = @Username,
	UsernameLwd = LOWER(@Username),
	Email = @Email,
	EmailLwd = LOWER(@Email),
	IsApproved = @IsApproved,
	LastLogin = @LastLogin,
	LastActivity = @LastActivity,
	Comment = @Comment
	WHERE ApplicationID = @ApplicationID AND
	UserID = @UserKey;

	-- Return successful
	RETURN(0)
END
GO                 

-- =============================================
-- Roles Create Procedures
-- =============================================

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_addusertorole]
(
@ApplicationName nvarchar(256),
@Username nvarchar(256),
@Rolename nvarchar(256)
)
AS
BEGIN
	DECLARE @UserID nvarchar(64)
	DECLARE @RoleID uniqueidentifier
	DECLARE @ApplicationID uniqueidentifier
	
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

	SET @UserID = (SELECT UserID FROM [{databaseOwner}].[{objectQualifier}prov_Membership] m WHERE m.UsernameLwd=LOWER(@Username) AND m.ApplicationID = @ApplicationID)
	SET @RoleID = (SELECT RoleID FROM [{databaseOwner}].[{objectQualifier}prov_Role] r WHERE r.RolenameLwd=LOWER(@Rolename) AND r.ApplicationID = @ApplicationID)

	IF (@UserID IS NULL OR @RoleID IS NULL)
		RETURN;
	
	IF (NOT EXISTS(SELECT 1 FROM [{databaseOwner}].[{objectQualifier}prov_RoleMembership] rm WHERE rm.UserID=@UserID AND rm.RoleID=@RoleID))
		INSERT INTO [{databaseOwner}].[{objectQualifier}prov_RoleMembership] (RoleID, UserID) VALUES (@RoleID, @UserID);
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_deleterole]
(
@ApplicationName nvarchar(256),
@Rolename nvarchar(256),
@DeleteOnlyIfRoleIsEmpty bit
)
AS
BEGIN
    	DECLARE @RoleID uniqueidentifier
	DECLARE @ErrorCode int
	DECLARE @ApplicationID uniqueidentifier
	
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT	
	
	SET @ErrorCode = 0
	SET @RoleID = (SELECT RoleID FROM [{databaseOwner}].[{objectQualifier}prov_Role] r WHERE r.RolenameLwd=LOWER(@Rolename) AND r.ApplicationID = @ApplicationID)
	
	IF (@DeleteOnlyIfRoleIsEmpty <> 0)
	BEGIN
		IF (EXISTS (SELECT 1 FROM [{databaseOwner}].[{objectQualifier}prov_RoleMembership] rm WHERE rm.RoleID=@RoleID))
			SELECT @ErrorCode = 2
	ELSE
		DELETE FROM [{databaseOwner}].[{objectQualifier}prov_RoleMembership] WHERE RoleID=@RoleID
	END	

	IF (@ErrorCode = 0)
		DELETE FROM [{databaseOwner}].[{objectQualifier}prov_Role] WHERE RoleID=@RoleID
    
    RETURN @ErrorCode	
END 
GO


CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_findusersinrole]
(
@ApplicationName nvarchar(256),
@Rolename nvarchar(256)
)
AS
BEGIN
    	DECLARE @RoleID uniqueidentifier
	DECLARE @ApplicationID uniqueidentifier
	
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

	SET @RoleID = (SELECT RoleID FROM [{databaseOwner}].[{objectQualifier}prov_Role] r INNER JOIN [{databaseOwner}].[{objectQualifier}prov_Application] a ON r.ApplicationID = a.ApplicationID WHERE r.RolenameLwd=LOWER(@Rolename) AND a.ApplicationID = @ApplicationID)

	SELECT m.* FROM [{databaseOwner}].[{objectQualifier}prov_Membership] m INNER JOIN [{databaseOwner}].[{objectQualifier}prov_RoleMembership] rm ON m.UserID = rm.UserID WHERE rm.RoleID = @RoleID
		
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_createrole]
(
@ApplicationName nvarchar(256),
@Rolename nvarchar(256)
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier
	
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT
	
	IF (NOT EXISTS(SELECT 1 FROM [{databaseOwner}].[{objectQualifier}prov_Role] r WHERE r.ApplicationID = @ApplicationID AND r.RolenameLwd = LOWER(@Rolename)))
		INSERT INTO [{databaseOwner}].[{objectQualifier}prov_Role] (RoleID, ApplicationID, RoleName, RoleNameLwd) VALUES (NEWID(),@ApplicationID, @Rolename,LOWER(@Rolename));		
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_getroles]
(
@ApplicationName nvarchar(256),
@Username nvarchar(256) = null
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier
	
 	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

	IF (@Username is null)
		SELECT r.* FROM [{databaseOwner}].[{objectQualifier}prov_Role] r WHERE r.ApplicationID = @ApplicationID
	ELSE
		SELECT
			r.*
		FROM
			[{databaseOwner}].[{objectQualifier}prov_Role] r
		INNER JOIN
			[{databaseOwner}].[{objectQualifier}prov_RoleMembership] rm ON r.RoleID = rm.RoleID
		INNER JOIN
			[{databaseOwner}].[{objectQualifier}prov_Membership] m ON m.UserID = rm.UserID
		WHERE
			r.ApplicationID  = @ApplicationID
			AND m.UsernameLwd = LOWER(@Username)
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_isuserinrole]
(
@ApplicationName nvarchar(256),
@Username nvarchar(256),
@Rolename nvarchar(256)
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

	SELECT m.* FROM [{databaseOwner}].[{objectQualifier}prov_RoleMembership] rm 
		INNER JOIN [{databaseOwner}].[{objectQualifier}prov_Membership] m ON rm.UserID = m.UserID
		INNER JOIN [{databaseOwner}].[{objectQualifier}prov_Role] r ON rm.RoleID = r.RoleID
		WHERE m.UsernameLwd=LOWER(@Username) AND r.RolenameLwd =LOWER(@Rolename) AND r.ApplicationID = @ApplicationID;
END 
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_removeuserfromrole]
(
@ApplicationName nvarchar(256),
@Username nvarchar(256),
@Rolename nvarchar(256)
)
AS
BEGIN
    	DECLARE @UserID nvarchar(64)
	DECLARE @RoleID uniqueidentifier
	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT	
	
	SET @RoleID = (SELECT RoleID FROM [{databaseOwner}].[{objectQualifier}prov_Role] r WHERE r.RolenameLwd = LOWER(@Rolename) AND r.ApplicationID = @ApplicationID)
	SET @UserID = (SELECT UserID FROM [{databaseOwner}].[{objectQualifier}prov_Membership] m WHERE m.UsernameLwd=LOWER(@Username) AND m.ApplicationID = @ApplicationID)
	
	DELETE FROM [{databaseOwner}].[{objectQualifier}prov_RoleMembership] WHERE RoleID = @RoleID AND UserID=@UserID
	
END 
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_exists]
(
@ApplicationName nvarchar(256),
@Rolename nvarchar(256)
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT
	
	SELECT COUNT(1) FROM [{databaseOwner}].[{objectQualifier}prov_Role]
		WHERE RolenameLwd = LOWER(@Rolename) AND ApplicationID = @ApplicationID
END 
GO

-- =============================================
-- Profiles Create Procedures
-- =============================================

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_profile_deleteinactive]
(
@ApplicationName nvarchar(256),
@InactiveSinceDate datetime
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

    DELETE
    FROM    [{databaseOwner}].[{objectQualifier}prov_Profile]
    WHERE   UserID IN
            (   SELECT  UserID
                FROM    [{databaseOwner}].[{objectQualifier}prov_Membership] m
                WHERE   ApplicationID = @ApplicationID
                        AND (LastActivity <= @InactiveSinceDate)
            )

    SELECT  @@ROWCOUNT
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_profile_deleteprofiles]
(
@ApplicationName nvarchar(256),
@UserNames nvarchar(4000)
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier
    DECLARE @UserName     nvarchar(256)
    DECLARE @CurrentPos   int
    DECLARE @NextPos      int
    DECLARE @NumDeleted   int
    DECLARE @DeletedUser  int
    DECLARE @ErrorCode    int

    SET @ErrorCode = 0
    SET @CurrentPos = 1
    SET @NumDeleted = 0

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

    WHILE (@CurrentPos <= LEN(@UserNames))
    BEGIN
        SELECT @NextPos = CHARINDEX(N',', @UserNames,  @CurrentPos)
        IF (@NextPos = 0 OR @NextPos IS NULL)
            SELECT @NextPos = LEN(@UserNames) + 1

        SELECT @UserName = SUBSTRING(@UserNames, @CurrentPos, @NextPos - @CurrentPos)
        SELECT @CurrentPos = @NextPos+1

        IF (LEN(@UserName) > 0)
        BEGIN
            SELECT @DeletedUser = 0

			DELETE FROM [{databaseOwner}].[{objectQualifier}prov_Profile] WHERE UserID IN (SELECT UserID FROM [{databaseOwner}].[{objectQualifier}prov_Membership] WHERE UsernameLwd = LOWER(@UserName) AND ApplicationID = @ApplicationID)

            IF( @@ERROR <> 0 )
            BEGIN
                SET @ErrorCode = -1
                GOTO Error
            END
            IF (@@ROWCOUNT <> 0)
                SELECT @NumDeleted = @NumDeleted + 1
        END
    END

    SELECT @NumDeleted

    RETURN 0

Error:

    RETURN @ErrorCode
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_profile_getprofiles]
(
	@ApplicationName        nvarchar(256),
	@PageIndex              int,
	@PageSize               int,
	@UserNameToMatch        nvarchar(256) = NULL,
	@InactiveSinceDate      datetime      = NULL
)
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table TO store the select results
    CREATE TABLE #PageIndexForUsers
    (
        IndexID int IDENTITY (0, 1) NOT NULL,
        UserID nvarchar(64) collate database_default
    )

    -- Insert into our temp table
    INSERT INTO #PageIndexForUsers (UserID)
        SELECT  m.UserID
        FROM    [{databaseOwner}].[{objectQualifier}prov_Membership] m, [{databaseOwner}].[{objectQualifier}prov_Profile] p
        WHERE   ApplicationID = @ApplicationID
            AND m.UserID = p.UserID
            AND (@InactiveSinceDate IS NULL OR LastActivity <= @InactiveSinceDate)
            AND (@UserNameToMatch IS NULL OR m.UserNameLwd LIKE LOWER(@UserNameToMatch))
        ORDER BY UserName


    SELECT  m.UserName, m.LastActivity, p.*
    FROM    [{databaseOwner}].[{objectQualifier}prov_Membership] m, [{databaseOwner}].[{objectQualifier}prov_Profile] p, #PageIndexForUsers i
    WHERE   m.UserId = p.UserId AND p.UserId = i.UserId AND i.IndexId >= @PageLowerBound AND i.IndexId <= @PageUpperBound

    SELECT COUNT(*)
    FROM   #PageIndexForUsers

    DROP TABLE #PageIndexForUsers
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_profile_getnumberinactiveprofiles]
    @ApplicationName        nvarchar(256),
    @InactiveSinceDate      datetime
AS
BEGIN
    	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

    SELECT  COUNT(*)
    FROM    [{databaseOwner}].[{objectQualifier}prov_Membership] m, [{databaseOwner}].[{objectQualifier}prov_Profile] p
    WHERE   ApplicationID = @ApplicationID
        AND m.UserID = p.UserID
        AND (LastActivity <= @InactiveSinceDate)
END
GO