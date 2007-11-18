-- =============================================
-- Author:		Mek
-- Create date: 30 September 2007
-- Description:	Membership Provider, Roles Provider SPROCS
-- =============================================


-- =============================================
-- Membership Drop Procedures
-- =============================================
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_createapplication]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_createapplication]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_changepassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_changepassword]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_changepasswordquestionandanswer]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_changepasswordquestionandanswer]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_createuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_createuser]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_deleteuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_deleteuser]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_findusersbyemail]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_findusersbyemail]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_findusersbyname]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_findusersbyname]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_getallusers]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getallusers]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_getnumberofusersonline]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getnumberofusersonline]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_getuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getuser]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_getusernamebyemail]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getusernamebyemail]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_resetpassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_resetpassword]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_unlockuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_unlockuser]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_updateuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_updateuser]
GO

-- =============================================
-- Roles Drop Procedures
-- =============================================

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_addusertorole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_addusertorole]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_createrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_createrole]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_deleterole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_deleterole]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_findusersinrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_findusersinrole]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_getroles]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_getroles]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_isuserinrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_isuserinrole]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_removeuserfromrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_removeuserfromrole]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_role_exists]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_exists]
GO

-- =============================================
-- Profiles Drop Procedures
-- =============================================

-- Not implemented yet!!!!!!!!!!!!!!!!!!!!!!!!!!

-- =============================================
-- Membership Create Procedures
-- =============================================

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_createapplication]
(
@ApplicationName nvarchar(50),
@ApplicationID uniqueidentifier OUTPUT
)
AS
BEGIN
	SET @ApplicationID = (SELECT ApplicationID FROM {objectQualifier}prov_Application WHERE ApplicationName=@ApplicationName)
	
	IF (@ApplicationID IS Null)
	BEGIN
		    SELECT  @ApplicationId = NEWID()
            INSERT  {objectQualifier}prov_Application(ApplicationId, ApplicationName)
            VALUES  (@ApplicationId, @ApplicationName)
    END
END 
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_changepassword]
(
@ApplicationName nvarchar(50),
@Username nvarchar(50),
@Password nvarchar(50),
@PasswordSalt nvarchar(50),
@PasswordFormat nvarchar(50),
@PasswordAnswer nvarchar(50)
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier

	SET @ApplicationID = (SELECT ApplicationID FROM {objectQualifier}prov_Application WHERE ApplicationName=@ApplicationName)

	UPDATE {objectQualifier}prov_Membership SET Password=@Password, PasswordSalt=@PasswordSalt,
		PasswordFormat=@PasswordFormat, PasswordAnswer=@PasswordAnswer
	WHERE Username=@Username and ApplicationID=@ApplicationID;

END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_changepasswordquestionandanswer]
(
@ApplicationName nvarchar(50),
@Username nvarchar(50),
@PasswordQuestion nvarchar(50),
@PasswordAnswer nvarchar(50)
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier
	
	SET @ApplicationID = (SELECT ApplicationID FROM {objectQualifier}prov_Application WHERE ApplicationName=@ApplicationName)
	
	UPDATE {objectQualifier}prov_Membership SET PasswordQuestion=@PasswordQuestion, PasswordAnswer=@PasswordAnswer
	WHERE Username=@Username and ApplicationID=@ApplicationID;

END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_createuser]
(
@ApplicationName nvarchar(50),
@Username nvarchar(50),
@Password nvarchar(50),
@PasswordSalt nvarchar(50) = null,
@PasswordFormat nvarchar(50) = null,
@Email nvarchar(50) = null,
@PasswordQuestion nvarchar(50) = null,
@PasswordAnswer nvarchar(50) = null,
@IsApproved bit = null,
@UserKey uniqueidentifier = null out
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier
	
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationId OUTPUT
	IF @UserKey IS NULL
		SET @UserKey = NEWID()
		
	INSERT INTO {objectQualifier}prov_Membership(UserID,ApplicationID,Username,Password,PasswordSalt,PasswordFormat,Email,PasswordQuestion,PasswordAnswer,IsApproved)
		VALUES (@UserKey, @ApplicationID,@Username, @Password, @PasswordSalt, @PasswordFormat, @Email, @PasswordQuestion, @PasswordAnswer, @IsApproved);
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_deleteuser]
(
@ApplicationName nvarchar(50),
@Username nvarchar(50),
@DeleteAllRelated bit
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier
	
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

	DELETE FROM {objectQualifier}prov_Membership WHERE ApplicationID=@ApplicationID AND Username=@Username;

	--INSERT IF STATEMENT TO DELETE MEMBERSHIP/ROLES INFORMATION / PROFILE INFORMATION	
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_findusersbyemail]
(
@ApplicationName nvarchar(50),
@EmailAddress nvarchar(50),
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
    
	CREATE TABLE #RowNumber (RowNumber int IDENTITY (1, 1),  UserID uniqueidentifier)
	
	INSERT INTO #RowNumber (UserID) SELECT m.UserID FROM {objectQualifier}prov_Membership m INNER JOIN {objectQualifier}prov_Application a ON m.ApplicationID = a.ApplicationID  WHERE a.ApplicationName = @ApplicationName AND m.Email = @EmailAddress

	SELECT m.*, r.RowNumber FROM {objectQualifier}prov_Membership m INNER JOIN #RowNumber r ON m.UserID = r.UserID WHERE r.RowNumber >= @PagingLowerBoundary AND r.RowNumber <= @PagingUpperBoundary;
    
	SET @TotalRecords = (SELECT COUNT(RowNumber) FROM #RowNumber)
	DROP TABLE #RowNumber
	RETURN @TotalRecords
   
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_findusersbyname]
(
@ApplicationName nvarchar(50),
@Username nvarchar(50),
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
    
	CREATE TABLE #RowNumber (RowNumber int IDENTITY (1, 1),  UserID uniqueidentifier)
	
	INSERT INTO #RowNumber (UserID) SELECT m.UserID FROM {objectQualifier}prov_Membership m INNER JOIN {objectQualifier}prov_Application a ON m.ApplicationID = a.ApplicationID WHERE a.ApplicationName = @ApplicationName AND m.Username = @Username

	SELECT m.*, r.RowNumber FROM {objectQualifier}prov_Membership m INNER JOIN #RowNumber r ON m.UserID = r.UserID WHERE r.RowNumber >= @PagingLowerBoundary AND r.RowNumber <= @PagingUpperBoundary;
    
	SET @TotalRecords = (SELECT COUNT(RowNumber) FROM #RowNumber)
	DROP TABLE #RowNumber
	RETURN @TotalRecords
   
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getallusers]
(
@ApplicationName nvarchar(50),
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
    
	CREATE TABLE #RowNumber (RowNumber int IDENTITY (1, 1),  UserID uniqueidentifier)
	
	INSERT INTO #RowNumber (UserID) SELECT m.UserID FROM {objectQualifier}prov_Membership m INNER JOIN {objectQualifier}prov_Application a ON m.ApplicationID = a.ApplicationID WHERE a.ApplicationName = @ApplicationName

	SELECT m.*, r.RowNumber FROM {objectQualifier}prov_Membership m INNER JOIN #RowNumber r ON m.UserID = r.UserID WHERE r.RowNumber >= @PagingLowerBoundary AND r.RowNumber <= @PagingUpperBoundary;
    
	SET @TotalRecords = (SELECT COUNT(RowNumber) FROM #RowNumber)
	DROP TABLE #RowNumber
	RETURN @TotalRecords
   
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getnumberofusersonline]
(
@ApplicationName nvarchar(50),
@TimeWindow int,
@CurrentTimeUtc DateTime
)
AS
BEGIN
	DECLARE @ActivityDate DateTime
	SET @ActivityDate = DATEADD(n, - @TimeWindow, @CurrentTimeUTC)
	
	DECLARE @NumberActive int
	SET @NumberActive = (SELECT COUNT(m.UserID) FROM {objectQualifier}prov_Membership m INNER JOIN {objectQualifier}prov_Application a ON m.ApplicationID = a.ApplicationID  WHERE a.ApplicationName = @ApplicationName AND m.LastLogin >= @ActivityDate)
    
    RETURN @NumberActive

END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getuser]
(
@ApplicationName nvarchar(50),
@Username nvarchar(50) = null,
@UserKey uniqueidentifier = null,
@UserIsOnline bit
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT
	
	IF (@UserKey IS NULL)
		SELECT m.* FROM {objectQualifier}prov_Membership m WHERE m.Username = @Username and m.ApplicationID = @ApplicationID
	ELSE
		SELECT m.* FROM {objectQualifier}prov_Membership m WHERE m.UserID = @UserKey and m.ApplicationID = @ApplicationID
	
	-- IF USER IS ONLINE DO AN UPDATE USER	
	IF (@UserIsOnline = 1)
	BEGIN
		UPDATE {objectQualifier}prov_Membership SET LastActivity = GETDATE() WHERE Username = @Username and ApplicationID = @ApplicationID
	END		
END
GO


CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_getusernamebyemail]
(
@ApplicationName nvarchar(50),
@Email nvarchar(50)
)
AS
BEGIN
	SELECT m.Username FROM {objectQualifier}prov_Membership m INNER JOIN {objectQualifier}prov_Application a ON m.ApplicationID = a.ApplicationID  WHERE a.ApplicationName = @ApplicationName AND m.Email = @Email;
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_resetpassword]
(
@ApplicationName nvarchar(50),
@UserName nvarchar(50),
@Password nvarchar(50),
@PasswordSalt nvarchar(50),
@PasswordFormat nvarchar(50),
@MaxInvalidAttempts int,
@PasswordAttemptWindow int,
@CurrentTimeUtc datetime
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT
	
	UPDATE {objectQualifier}prov_Membership SET
	Password = @Password,
	PasswordSalt = @PasswordSalt,
	PasswordFormat = @PasswordFormat,
	LastPasswordChange = @CurrentTimeUtc
	WHERE ApplicationID = @ApplicationID AND
	Username = @Username;

END
GO


CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_unlockuser]
(
@ApplicationName nvarchar(50),
@UserName nvarchar(50)
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT
	
	UPDATE {objectQualifier}prov_Membership SET
	IsLockedOut = 0,
	FailedPasswordAttempts = 0
	WHERE ApplicationID = @ApplicationID AND
	Username = @Username;

END
GO
                
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_updateuser]
(
@ApplicationName nvarchar(50),
@UserKey uniqueidentifier,
@UserName nvarchar(50),
@Email nvarchar(50),
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
		IF (EXISTS (SELECT 1 FROM {objectQualifier}prov_Membership m WHERE m.UserID != @UserKey AND m.Email=LOWER(@Email) AND m.ApplicationID=@ApplicationID) )
			RETURN (2)
	END
	
	UPDATE {objectQualifier}prov_Membership SET
	Username = @Username,
	Email = @Email,
	IsApproved = @IsApproved,
	LastLogin = @LastLogin,
	LastActivity = @LastActivity
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
@ApplicationName nvarchar(255),
@Username nvarchar(255),
@Rolename nvarchar(255)
)
AS
BEGIN
	DECLARE @UserID uniqueidentifier
	DECLARE @RoleID uniqueidentifier
	DECLARE @ApplicationID uniqueidentifier
	
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

	SET @UserID = (SELECT UserID FROM {objectQualifier}prov_Membership m WHERE m.Username=@Username AND m.ApplicationID = @ApplicationID)
	SET @RoleID = (SELECT RoleID FROM {objectQualifier}prov_Role r WHERE r.Rolename=@Rolename AND r.ApplicationID = @ApplicationID)
	
	IF (NOT EXISTS(SELECT 1 FROM {objectQualifier}prov_RoleMembership rm WHERE rm.UserID=@UserID AND rm.RoleID=@RoleID))
		INSERT INTO {objectQualifier}prov_RoleMembership(RoleID, UserID) VALUES (@RoleID, @UserID);
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_deleterole]
(
@ApplicationName nvarchar(255),
@Rolename nvarchar(255),
@DeleteOnlyIfRoleIsEmpty bit
)
AS
BEGIN
	DECLARE @RoleID uniqueidentifier
	DECLARE @ErrorCode int
	DECLARE @ApplicationID uniqueidentifier
	
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT	
	
	SET @ErrorCode = 0
	SET @RoleID = (SELECT RoleID FROM {objectQualifier}prov_Role r WHERE r.Rolename=@Rolename AND r.ApplicationID = @ApplicationID)
	
	IF (@DeleteOnlyIfRoleIsEmpty <> 0)
	BEGIN
		IF (EXISTS (SELECT 1 FROM {objectQualifier}prov_RoleMembership rm WHERE rm.RoleID=@RoleID))
			SELECT @ErrorCode = 2
	ELSE
		DELETE FROM {objectQualifier}prov_RoleMembership WHERE RoleID=@RoleID
	END	

	IF (@ErrorCode = 0)
		DELETE FROM {objectQualifier}prov_Role WHERE RoleID=@RoleID
    
    RETURN @ErrorCode	
END 
GO


CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_findusersinrole]
(
@ApplicationName nvarchar(255),
@Rolename nvarchar(255)
)
AS
BEGIN
	DECLARE @RoleID uniqueidentifier
	DECLARE @ApplicationID uniqueidentifier
	
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

	SET @RoleID = (SELECT RoleID FROM {objectQualifier}prov_Role r INNER JOIN {objectQualifier}prov_Application a ON r.ApplicationID = a.ApplicationID WHERE r.Rolename=@Rolename AND a.ApplicationName = @ApplicationName)

	SELECT m.* FROM {objectQualifier}prov_Membership m INNER JOIN {objectQualifier}prov_RoleMembership rm ON m.UserID = rm.UserID WHERE rm.RoleID = @RoleID
		
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_createrole]
(
@ApplicationName nvarchar(255),
@Rolename nvarchar(255)
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier
	
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT
	
	IF (NOT EXISTS(SELECT 1 FROM {objectQualifier}prov_Role r WHERE r.ApplicationID = @ApplicationID AND r.Rolename = @Rolename))
		INSERT INTO {objectQualifier}prov_Role(RoleID, ApplicationID, RoleName) VALUES (NEWID(),@ApplicationID, @Rolename);		
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_getroles]
(
@ApplicationName nvarchar(255),
@Username nvarchar(255) = null
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier
	
	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

	IF (@Username is null)
		SELECT r.* FROM {objectQualifier}prov_Role r WHERE r.ApplicationID = @ApplicationID
	ELSE
		SELECT
			r.*
		FROM
			{objectQualifier}prov_Role r
		INNER JOIN
			{objectQualifier}prov_Membership m ON m.ApplicationID = r.ApplicationID
		WHERE
			r.ApplicationID  = @ApplicationID
			AND m.Username = @Username
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_isuserinrole]
(
@ApplicationName nvarchar(255),
@Username nvarchar(255),
@Rolename nvarchar(255)
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT

	SELECT m.* FROM {objectQualifier}prov_RoleMembership rm 
		INNER JOIN {objectQualifier}prov_Membership m ON rm.UserID = m.UserID
		INNER JOIN {objectQualifier}prov_Role r ON rm.RoleID = r.RoleID
		WHERE m.Username=@Username AND r.Rolename =@Rolename AND r.ApplicationID = @ApplicationID;
END 
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_removeuserfromrole]
(
@ApplicationName nvarchar(255),
@Username nvarchar(255),
@Rolename nvarchar(255)
)
AS
BEGIN
	DECLARE @UserID uniqueidentifier
	DECLARE @RoleID uniqueidentifier
	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT	
	
	SET @RoleID = (SELECT RoleID FROM {objectQualifier}prov_Role r WHERE r.Rolename =@Rolename AND r.ApplicationID = @ApplicationID)
	SET @UserID = (SELECT UserID FROM {objectQualifier}prov_Membership m WHERE m.Username=@Username AND m.ApplicationID = @ApplicationID)
	
	DELETE FROM {objectQualifier}prov_RoleMembership WHERE RoleID = @RoleID AND UserID=@UserID
	
END 
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}prov_role_exists]
(
@ApplicationName nvarchar(255),
@Rolename nvarchar(255)
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier

	EXEC [{databaseOwner}].[{objectQualifier}prov_CreateApplication] @ApplicationName, @ApplicationID OUTPUT
	
	SELECT COUNT(1) FROM {objectQualifier}prov_Role
		WHERE Rolename = @Rolename;
END 
GO

-- =============================================
-- Profiles Create Procedures
-- =============================================

-- Not implemented yet!!!!!!!!!!!!!!!!!!!!!!!!!!