-- =============================================
-- Author:		Mek
-- Create date: 30 September 2007
-- Description:	Membership Provider, Roles Provider SPROCS
-- =============================================


-- =============================================
-- Membership Drop Procedures
-- =============================================
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_createapplication]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_createapplication]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_changepassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_changepassword]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_changepasswordquestionandanswer]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_changepasswordquestionandanswer]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_createuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_createuser]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_deleteuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_deleteuser]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_findusersbyemail]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_findusersbyemail]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_findusersbyname]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_findusersbyname]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_getallusers]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_getallusers]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_getnumberofusersonline]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_getnumberofusersonline]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_getuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_getuser]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_getusernamebyemail]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_getusernamebyemail]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_resetpassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_resetpassword]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_unlockuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_unlockuser]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_updateuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_updateuser]
GO

-- =============================================
-- Roles Drop Procedures
-- =============================================

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_role_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_role_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_role_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_role_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_role_addusertorole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_role_addusertorole]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_role_createrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_role_createrole]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_role_deleterole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_role_deleterole]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_role_findusersinrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_role_findusersinrole]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_role_getroles]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_role_getroles]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_role_isuserinrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_role_isuserinrole]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafprov_role_removeuserfromrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafprov_role_removeuserfromrole]
GO

-- =============================================
-- Profiles Drop Procedures
-- =============================================

-- Not implemented yet!!!!!!!!!!!!!!!!!!!!!!!!!!

-- =============================================
-- Membership Create Procedures
-- =============================================

CREATE PROCEDURE dbo.yafprov_createapplication
(
@ApplicationName nvarchar(50),
@ApplicationID uniqueidentifier OUTPUT
)
AS
BEGIN
	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Application WHERE ApplicationName=@ApplicationName)
	
	IF (@ApplicationID IS Null)
	BEGIN
		    SELECT  @ApplicationId = NEWID()
            INSERT  yafprov_Application(ApplicationId, ApplicationName)
            VALUES  (@ApplicationId, @ApplicationName)
    END
END 
GO

CREATE PROCEDURE dbo.yafprov_changepassword
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

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Application WHERE ApplicationName=@ApplicationName)

	UPDATE yafprov_Membership SET Password=@Password, PasswordSalt=@PasswordSalt,
		PasswordFormat=@PasswordFormat, PasswordAnswer=@PasswordAnswer
	WHERE Username=@Username and ApplicationID=@ApplicationID;

END
GO

CREATE PROCEDURE dbo.yafprov_changepasswordquestionandanswer
(
@ApplicationName nvarchar(50),
@Username nvarchar(50),
@PasswordQuestion nvarchar(50),
@PasswordAnswer nvarchar(50)
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier
	
	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Application WHERE ApplicationName=@ApplicationName)
	
	UPDATE yafprov_Membership SET PasswordQuestion=@PasswordQuestion, PasswordAnswer=@PasswordAnswer
	WHERE Username=@Username and ApplicationID=@ApplicationID;

END
GO

CREATE PROCEDURE [dbo].[yafprov_createuser]
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
	
	EXEC dbo.yafprov_CreateApplication @ApplicationName, @ApplicationId OUTPUT
	IF @UserKey IS NULL
		SET @UserKey = NEWID()
		
	INSERT INTO yafprov_Membership(UserID,ApplicationID,Username,Password,PasswordSalt,PasswordFormat,Email,PasswordQuestion,PasswordAnswer,IsApproved)
		VALUES (@UserKey, @ApplicationID,@Username, @Password, @PasswordSalt, @PasswordFormat, @Email, @PasswordQuestion, @PasswordAnswer, @IsApproved);
END
GO

CREATE PROCEDURE dbo.yafprov_deleteuser
(
@ApplicationName nvarchar(50),
@Username nvarchar(50),
@DeleteAllRelated bit
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Application WHERE ApplicationName=@ApplicationName)

	DELETE FROM yafprov_Membership WHERE ApplicationID=@ApplicationID AND Username=@Username;

	--INSERT IF STATEMENT TO DELETE MEMBERSHIP/ROLES INFORMATION / PROFILE INFORMATION	
END
GO

CREATE PROCEDURE dbo.yafprov_findusersbyemail
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

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Application WHERE ApplicationName=@ApplicationName)

    DECLARE @PagingLowerBoundary int
    DECLARE @PagingUpperBoundary int
    DECLARE @TotalRecords   int
    SET @PagingLowerBoundary = @PageSize * @PageIndex
    SET @PagingUpperBoundary = @PageSize - 1 + @PagingLowerBoundary
    
	CREATE TABLE #RowNumber (RowNumber int IDENTITY (1, 1),  UserID uniqueidentifier)
	
	INSERT INTO #RowNumber (UserID) SELECT m.UserID FROM yafprov_Membership m INNER JOIN yafprov_Application a ON m.ApplicationID = a.ApplicationID  WHERE a.ApplicationName = @ApplicationName AND m.Email = @EmailAddress

	SELECT m.*, r.RowNumber FROM yafprov_Membership m INNER JOIN #RowNumber r ON m.UserID = r.UserID WHERE r.RowNumber >= @PagingLowerBoundary AND r.RowNumber <= @PagingUpperBoundary;
    
	SET @TotalRecords = (SELECT COUNT(RowNumber) FROM #RowNumber)
	DROP TABLE #RowNumber
	RETURN @TotalRecords
   
END
GO

CREATE PROCEDURE dbo.yafprov_findusersbyname
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

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Application WHERE ApplicationName=@ApplicationName)

    DECLARE @PagingLowerBoundary int
    DECLARE @PagingUpperBoundary int
    DECLARE @TotalRecords   int
    SET @PagingLowerBoundary = @PageSize * @PageIndex
    SET @PagingUpperBoundary = @PageSize - 1 + @PagingLowerBoundary
    
	CREATE TABLE #RowNumber (RowNumber int IDENTITY (1, 1),  UserID uniqueidentifier)
	
	INSERT INTO #RowNumber (UserID) SELECT m.UserID FROM yafprov_Membership m INNER JOIN yafprov_Application a ON m.ApplicationID = a.ApplicationID WHERE a.ApplicationName = @ApplicationName AND m.Username = @Username

	SELECT m.*, r.RowNumber FROM yafprov_Membership m INNER JOIN #RowNumber r ON m.UserID = r.UserID WHERE r.RowNumber >= @PagingLowerBoundary AND r.RowNumber <= @PagingUpperBoundary;
    
	SET @TotalRecords = (SELECT COUNT(RowNumber) FROM #RowNumber)
	DROP TABLE #RowNumber
	RETURN @TotalRecords
   
END
GO

CREATE PROCEDURE dbo.yafprov_getallusers
(
@ApplicationName nvarchar(50),
@PageIndex int,
@PageSize int
)
AS
BEGIN

    -- Set the page bounds
	DECLARE @ApplicationID uniqueidentifier

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Application WHERE ApplicationName=@ApplicationName)

    DECLARE @PagingLowerBoundary int
    DECLARE @PagingUpperBoundary int
    DECLARE @TotalRecords   int
    SET @PagingLowerBoundary = @PageSize * @PageIndex
    SET @PagingUpperBoundary = @PageSize - 1 + @PagingLowerBoundary
    
	CREATE TABLE #RowNumber (RowNumber int IDENTITY (1, 1),  UserID uniqueidentifier)
	
	INSERT INTO #RowNumber (UserID) SELECT m.UserID FROM yafprov_Membership m INNER JOIN yafprov_Application a ON m.ApplicationID = a.ApplicationID WHERE a.ApplicationName = @ApplicationName

	SELECT m.*, r.RowNumber FROM yafprov_Membership m INNER JOIN #RowNumber r ON m.UserID = r.UserID WHERE r.RowNumber >= @PagingLowerBoundary AND r.RowNumber <= @PagingUpperBoundary;
    
	SET @TotalRecords = (SELECT COUNT(RowNumber) FROM #RowNumber)
	DROP TABLE #RowNumber
	RETURN @TotalRecords
   
END
GO

CREATE PROCEDURE dbo.yafprov_getnumberofusersonline
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
	SET @NumberActive = (SELECT COUNT(m.UserID) FROM yafprov_Membership m INNER JOIN yafprov_Application a ON m.ApplicationID = a.ApplicationID  WHERE a.ApplicationName = @ApplicationName AND m.LastLogin >= @ActivityDate)
    
    RETURN @NumberActive

END
GO

CREATE PROCEDURE dbo.yafprov_getuser
(
@ApplicationName nvarchar(50),
@Username nvarchar(50) = null,
@UserKey uniqueidentifier = null,
@UserIsOnline bit
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Application WHERE ApplicationName=@ApplicationName)
	
	IF (@UserKey IS NULL)
		SELECT m.* FROM yafprov_Membership m WHERE m.Username = @Username and m.ApplicationID = @ApplicationID
	ELSE
		SELECT m.* FROM yafprov_Membership m WHERE m.UserID = @UserKey and m.ApplicationID = @ApplicationID
	
	-- IF USER IS ONLINE DO AN UPDATE USER	
END


CREATE PROCEDURE dbo.yafprov_getusernamebyemail
(
@ApplicationName nvarchar(50),
@Email nvarchar(50)
)
AS
BEGIN
	SELECT m.Username FROM yafprov_Membership m INNER JOIN yafprov_Application a ON m.ApplicationID = a.ApplicationID  WHERE a.ApplicationName = @ApplicationName AND m.Email = @Email;
END
GO

CREATE PROCEDURE dbo.yafprov_resetpassword
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

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Application WHERE ApplicationName=@ApplicationName)
	
	UPDATE yafprov_Membership SET
	Password = @Password,
	PasswordSalt = @PasswordSalt,
	PasswordFormat = @PasswordFormat,
	LastPasswordChange = @CurrentTimeUtc
	WHERE ApplicationID = @ApplicationID AND
	Username = @Username;

END
GO


CREATE PROCEDURE dbo.yafprov_unlockuser
(
@ApplicationName nvarchar(50),
@UserName nvarchar(50)
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Application WHERE ApplicationName=@ApplicationName)
	
	UPDATE yafprov_Membership SET
	IsLockedOut = 0,
	FailedPasswordAttempts = 0
	WHERE ApplicationID = @ApplicationID AND
	Username = @Username;

END
GO
                
CREATE PROCEDURE dbo.yafprov_updateuser
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

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Application WHERE ApplicationName=@ApplicationName)
	
	-- Check for Unique Email Application Missing
	
	UPDATE yafprov_Membership SET
	Username = @Username,
	Email = @Email,
	IsApproved = @IsApproved,
	LastLogin = @LastLogin,
	LastActivity = @LastActivity
	WHERE ApplicationID = @ApplicationID AND
	UserID = @UserKey;

END
GO                 

-- =============================================
-- Roles Create Procedures
-- =============================================

CREATE PROCEDURE dbo.yafprov_role_addusertorole
(
@ApplicationName nvarchar(255),
@Username nvarchar(255),
@Rolename nvarchar(255)
)
AS
BEGIN
	DECLARE @UserID uniqueidentifier
	DECLARE @RoleID uniqueidentifier

	SET @UserID = (SELECT UserID FROM yafprov_Membership m INNER JOIN yafprov_Application a ON m.ApplicationID = a.ApplicationID WHERE m.Username=@Username AND a.ApplicationName = @ApplicationName)
	SET @RoleID = (SELECT RoleID FROM yafprov_Role r INNER JOIN yafprov_Application a ON r.ApplicationID = a.ApplicationID WHERE r.Rolename=@Rolename AND a.ApplicationName = @ApplicationName)
	
	INSERT INTO yafprov_RoleMembership(RoleID, UserID) VALUES (@UserID, @RoleID);
END 
GO

CREATE PROCEDURE dbo.yafprov_role_deleterole
(
@ApplicationName nvarchar(255),
@Rolename nvarchar(255),
@DeleteOnlyIfRoleIsEmpty bit
)
AS
BEGIN
	DECLARE @RoleID uniqueidentifier
	DECLARE @ErrorCode int
	SET @ErrorCode = 0
	SET @RoleID = (SELECT RoleID FROM yafprov_Role r INNER JOIN yafprov_Application a ON r.ApplicationID = a.ApplicationID WHERE r.Rolename=@Rolename AND a.ApplicationName = @ApplicationName)
	
	IF (@DeleteOnlyIfRoleIsEmpty <> 0)
	BEGIN
		IF (EXISTS (SELECT 1 FROM yafprov_RoleMembership rm WHERE rm.RoleID=@RoleID))
			SELECT @ErrorCode = 2
	ELSE
		DELETE FROM yafprov_RoleMembership WHERE RoleID=@RoleID
	END	

	IF (@ErrorCode = 0)
		DELETE FROM yafprov_Role WHERE RoleID=@RoleID
    
    RETURN @ErrorCode	
END 
GO


CREATE PROCEDURE dbo.yafprov_role_findusersinrole
(
@ApplicationName nvarchar(255),
@Rolename nvarchar(255)
)
AS
BEGIN
	DECLARE @RoleID uniqueidentifier

	SET @RoleID = (SELECT RoleID FROM yafprov_Role r INNER JOIN yafprov_Application a ON r.ApplicationID = a.ApplicationID WHERE r.Rolename=@Rolename AND a.ApplicationName = @ApplicationName)

	SELECT rm.* FROM yafProv_RoleMembership rm WHERE rm.RoleID = @RoleID
		
END 
GO

CREATE PROCEDURE dbo.yafprov_role_createrole
(
@ApplicationName nvarchar(255),
@Rolename nvarchar(255)
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier
	
	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Application WHERE ApplicationName=@ApplicationName)
	
	INSERT INTO yafprov_Role(RoleID, ApplicationID, RoleName) VALUES (NEWID(),@ApplicationID, @Rolename);		
END 
GO


CREATE PROCEDURE dbo.yafprov_role_getroles
(
@ApplicationName nvarchar(255),
@Rolename nvarchar(255) = null
)
AS
BEGIN
	IF (@Rolename is null)
		SELECT r.* FROM yafprov_Role r INNER JOIN yafprov_Application a ON a.ApplicationID = r.ApplicationID WHERE a.ApplicationName=@ApplicationName
	ELSE
		SELECT r.* FROM yafprov_Role r INNER JOIN yafprov_Application a ON a.ApplicationID = r.ApplicationID WHERE a.ApplicationName=@ApplicationName AND r.Rolename = @Rolename	
END 
GO

CREATE PROCEDURE dbo.yafprov_role_isuserinrole
(
@ApplicationName nvarchar(255),
@Username nvarchar(255),
@Rolename nvarchar(255)
)
AS
BEGIN
	SELECT m.* FROM yafprov_RoleMembership rm 
		INNER JOIN yafprov_Membership m ON rm.UserID = m.UserID
		INNER JOIN yafprov_Role r ON rm.RoleID = r.RoleID
		INNER JOIN yafprov_Application a ON r.ApplicationID = a.ApplicationID AND m.ApplicationID = a.ApplicationID
		WHERE m.Username=@Username AND r.Rolename =@Rolename AND a.ApplicationName = @ApplicationName;
END 
GO

CREATE PROCEDURE dbo.yafprov_role_removeuserfromrole
(
@ApplicationName nvarchar(255),
@Username nvarchar(255),
@Rolename nvarchar(255)
)
AS
BEGIN
	DECLARE @UserID uniqueidentifier
	DECLARE @RoleID uniqueidentifier
	
	SET @RoleID = (SELECT RoleID FROM yafprov_Role r INNER JOIN yafprov_Application a ON r.ApplicationID = a.ApplicationID WHERE r.Rolename =@Rolename AND a.ApplicationName = @ApplicationName)
	SET @UserID = (SELECT UserID FROM yafprov_Membership m INNER JOIN yafprov_Application a ON m.ApplicationID = a.ApplicationID WHERE m.Username=@Username AND a.ApplicationName = @ApplicationName)
	
	DELETE FROM yafprov_RoleMembership WHERE RoleID = @RoleID AND UserID=@UserID
	
END 
GO

-- =============================================
-- Profiles Create Procedures
-- =============================================

-- Not implemented yet!!!!!!!!!!!!!!!!!!!!!!!!!!