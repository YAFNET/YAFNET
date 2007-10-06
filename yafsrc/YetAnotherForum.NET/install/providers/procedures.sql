-- =============================================
-- Author:		Mek
-- Create date: 30 September 2007
-- Description:	MembershipProvider SPROCS
-- =============================================
-- DROP PROCEDURES CHECKED


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

CREATE PROCEDURE dbo.yafprov_createapplication
(
@ApplicationName nvarchar(50),
@ApplicationID uniqueidentifier OUTPUT
)
AS
BEGIN
	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Applications WHERE ApplicationName=@ApplicationName)
	
	IF (@ApplicationID IS Null)
		    SELECT  @ApplicationId = NEWID()
            INSERT  yafprov_Applications(ApplicationId, ApplicationName)
            VALUES  (@ApplicationId, @ApplicationName)
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

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Applications WHERE ApplicationName=@ApplicationName)

	UPDATE yafprov_Members SET Password=@Password, PasswordSalt=@PasswordSalt,
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
	
	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Applications WHERE ApplicationName=@ApplicationName)
	
	UPDATE yafprov_Members SET PasswordQuestion=@PasswordQuestion, PasswordAnswer=@PasswordAnswer
	WHERE Username=@Username and ApplicationID=@ApplicationID;

END
GO

CREATE PROCEDURE dbo.yafprov_createuser
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
	IF @UserKey IS NOT NULL
		SET @UserKey = NEWID()
		
	INSERT INTO yafprov_Members(UserID,ApplicationID,Username,Password,PasswordSalt,PasswordFormat,Email,PasswordQuestion,PasswordAnswer,IsApproved)
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

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Applications WHERE ApplicationName=@ApplicationName)

	DELETE FROM yafprov_Members WHERE ApplicationID=@ApplicationID AND Username=@Username;

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

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Applications WHERE ApplicationName=@ApplicationName)

    DECLARE @PagingLowerBoundary int
    DECLARE @PagingUpperBoundary int
    DECLARE @TotalRecords   int
    SET @PagingLowerBoundary = @PageSize * @PageIndex
    SET @PagingUpperBoundary = @PageSize - 1 + @PagingLowerBoundary
    
	CREATE TABLE #RowNumber (RowNumber int IDENTITY (1, 1),  UserID uniqueidentifier)
	
	INSERT INTO #RowNumber (UserID) SELECT m.UserID FROM yaf_Members m WHERE m.ApplicationName = @ApplicationName AND m.Email = @EmailAddress

	SELECT m.*, r.RowNumber FROM yafprov_Members m INNER JOIN #RowNumber r ON m.UserID = r.UserID WHERE r.RowNumber >= @PagingLowerBoundary AND r.RowNumber <= @PagingUpperBoundary;
    
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

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Applications WHERE ApplicationName=@ApplicationName)

    DECLARE @PagingLowerBoundary int
    DECLARE @PagingUpperBoundary int
    DECLARE @TotalRecords   int
    SET @PagingLowerBoundary = @PageSize * @PageIndex
    SET @PagingUpperBoundary = @PageSize - 1 + @PagingLowerBoundary
    
	CREATE TABLE #RowNumber (RowNumber int IDENTITY (1, 1),  UserID uniqueidentifier)
	
	INSERT INTO #RowNumber (UserID) SELECT m.UserID FROM yaf_Members m WHERE m.ApplicationName = @ApplicationName AND m.Username = @Username

	SELECT m.*, r.RowNumber FROM yafprov_Members m INNER JOIN #RowNumber r ON m.UserID = r.UserID WHERE r.RowNumber >= @PagingLowerBoundary AND r.RowNumber <= @PagingUpperBoundary;
    
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

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Applications WHERE ApplicationName=@ApplicationName)

    DECLARE @PagingLowerBoundary int
    DECLARE @PagingUpperBoundary int
    DECLARE @TotalRecords   int
    SET @PagingLowerBoundary = @PageSize * @PageIndex
    SET @PagingUpperBoundary = @PageSize - 1 + @PagingLowerBoundary
    
	CREATE TABLE #RowNumber (RowNumber int IDENTITY (1, 1),  UserID uniqueidentifier)
	
	INSERT INTO #RowNumber (UserID) SELECT m.UserID FROM yaf_Members m WHERE m.ApplicationName = @ApplicationName

	SELECT m.*, r.RowNumber FROM yafprov_Members m INNER JOIN #RowNumber r ON m.UserID = r.UserID WHERE r.RowNumber >= @PagingLowerBoundary AND r.RowNumber <= @PagingUpperBoundary;
    
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
	SET @NumberActive = (SELECT COUNT(m.UserID) FROM yafprov_Membership m INNER JOIN yafprov_Applications a ON m.ApplicationID = a.ApplicationID  WHERE a.ApplicationName = @ApplicationName AND m.LastLoginDate >= @ActivityDate)
    
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

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Applications WHERE ApplicationName=@ApplicationName)
	
	IF (@UserKey IS NULL)
		SELECT yafprov_Membership.* FROM yafprov_Membership a WHERE a.Username = @Username and a.ApplicationID = @ApplicationID
	ELSE
		SELECT yafprov_Membership.* FROM yafprov_Membership a WHERE a.Username = @Username and a.ApplicationID = @ApplicationID
	
	-- IF USER IS ONLINE DO AN UPDATE USER	
END
GO

CREATE PROCEDURE dbo.yafprov_getusernamebyemail
(
@ApplicationName nvarchar(50),
@Email nvarchar(50)
)
AS
BEGIN
	SELECT m.Username FROM yafprov_Membership m INNER JOIN yafprov_Applications a ON m.ApplicationID = a.ApplicationID  WHERE a.ApplicationName = @ApplicationName AND m.EmailAddress = @Email;
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

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Applications WHERE ApplicationName=@ApplicationName)
	
	UPDATE yafprov_Membership SET
	Password = @Password,
	PasswordSalt = @PasswordSalt,
	PasswordFormat = @PasswordFormat,
	MaxInvalidAttempts = @MaxInvalidAttempts,
	PasswordAttemptWindow = @PasswordAttemptWindow,
	CurrentTimeUtc = @CurrentTimeUtc
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

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Applications WHERE ApplicationName=@ApplicationName)
	
	UPDATE yafprov_Membership SET
	IsLockedOut = false,
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
@LastLoginDate datetime,
@LastActivityDate datetime
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Applications WHERE ApplicationName=@ApplicationName)
	
	-- Check for Unique Email Application Missing
	
	UPDATE yafprov_Membership SET
	Username = @Username,
	Email = @Email,
	Comment = @Comment,
	IsApproved = @IsApproved,
	LastLoginDate = @LastLoginDate,
	LastActivityDate = @LastActivityDate
	WHERE ApplicationID = @ApplicationID AND
	UserID = @UserKey;

END
GO                 



