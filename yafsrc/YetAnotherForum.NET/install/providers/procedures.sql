-- =============================================
-- Author:		Mek
-- Create date: 30 September 2007
-- Description:	MembershipProvider SPROCS
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
@PasswordSalt nvarchar(50),
@PasswordFormat nvarchar(50),
@Email nvarchar(50),
@PasswordQuestion nvarchar(50),
@PasswordAnswer nvarchar(50),
@IsApproved bit,
@UserKey uniqueidentifier
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier
	
	EXEC dbo.yafprov_CreateApplication @ApplicationName, @ApplicationId OUTPUT
	
	INSERT INTO yafprov_Members(MembershipID,ApplicationID,Username,Password,PassworldSalt,PasswordFormat,Email,PasswordQuestion,PasswordAnswer,IsApproved)
	VALUES (@UserKey, @ApplicationID,@Username, @Password, @PassworldSalt, @PasswordFormat, @Email, @PasswordQuestion, @PasswordAnswer, @IsApproved);

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
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PagingLowerBoundary = @PageSize * @PageIndex
    SET @PagingUpperBoundart = @PageSize - 1 + @PagingLowerBoundary
    
    SELECT RowNumber = (SELECT count(b.MemberID) FROM yaf_Membership b WHERE b.MemberID = a.MemberID AND b.ApplicationName = @ApplicationName), a.*
    FROM yafprov_Members a
    WHERE a.ApplicationName = @ApplicationName AND a.Email = @EmailAddress AND RowNumber >= @PageLowerBound AND RowNumber <= @PageHigherBound;
    
    RETURN @TotalRecords = SELECT COUNT(RowNumber) FROM #MemberRows;
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

    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PagingLowerBoundary = @PageSize * @PageIndex
    SET @PagingUpperBoundart = @PageSize - 1 + @PagingLowerBoundary
    
    SELECT RowNumber = (SELECT count(b.MemberID) FROM yaf_Membership b WHERE b.MemberID = a.MemberID AND b.ApplicationName = @ApplicationName), a.*
    FROM yafprov_Members a
    WHERE a.ApplicationName = @ApplicationName AND a.Username = @Username AND RowNumber >= @PageLowerBound AND RowNumber <= @PageHigherBound;
    
    RETURN @TotalRecords = SELECT COUNT(RowNumber) FROM #MemberRows;
    END
END
GO

CREATE PROCEDURE dbo.yafprov_getallusers
(
@ApplicationName nvarchar(50),
@PageIndex int,
@PageSize int
)
AS

    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PagingLowerBoundary = @PageSize * @PageIndex
    SET @PagingUpperBoundart = @PageSize - 1 + @PagingLowerBoundary
    
    SELECT RowNumber = (SELECT count(b.MemberID) FROM yaf_Membership b WHERE b.MemberID = a.MemberID AND b.ApplicationName = @ApplicationName), a.*
    FROM yafprov_Members a
    WHERE a.ApplicationName = @ApplicationName AND RowNumber >= @PageLowerBound AND RowNumber <= @PageHigherBound;
    
    RETURN @TotalRecords = SELECT COUNT(RowNumber) FROM #MemberRows;
    END
END
GO

CREATE PROCEDURE dbo.yafprov_getnumberofusersonline
(
@ApplicationName nvarchar(50),
@TimeWindow int,
@CurrentTimeUtc DateTime
)
AS
	DECLARE @ActivityDate DateTime
	SET @ActivityDate = DATEADD(n, - @TimeWindow, @CurrentTimeUTC)
	
	DECLARE @NumberActive
	SET @NumberActive = SELECT COUNT(MembershipID) FROM yafprov_Membership
	WHERE ApplicationName = @ApplicationName AND
	LastLoginDate >= @ActivityDate;
    
    RETURN @NumberActive;
    
END
GO

CREATE PROCEDURE dbo.yafprov_getuser
(
@ApplicationName nvarchar(50),
@Username nvarchar(50) = null,
@UserKey uniqueidentifier = null,
@UserIsOnline bool
)
AS
	DECLARE @ApplicationID uniqueidentifier

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Applications WHERE ApplicationName=@ApplicationName)
	
	IF (@UserKey IS NULL)
		SELECT yafprov_Membership.* FROM yafprov_Membership a WHERE a.Username = @Username and a.ApplicationID = @ApplicationID
	ELSE
		SELECT yafprov_Membership.* FROM yafprov_Membership a WHERE a.Username = @Username and a.ApplicationID = @ApplicationID
	
	-- IF USER IS ONLINE DO AN UPDATE USER	
END
GO

CREATE PROCEDURE dbo.yafprov_getuser
(
@ApplicationName nvarchar(50),
@Email nvarchar(50)
)
AS
	DECLARE @ApplicationID uniqueidentifier

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Applications WHERE ApplicationName=@ApplicationName)
	
	SELECT m.Username FROM yafprov_Membership m WHERE m.ApplicationID = @ApplicationID AND m.EmailAddress = @Email;
	
	-- IF USER IS ONLINE DO AN UPDATE USER	
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
@UserName nvarchar(50),
@Password nvarchar(50),
@PasswordSalt nvarchar(50),
@PasswordFormat nvarchar(50),
@MaxInvalidAttempts int,
@PasswordAttemptWindow int,
@CurrentTimeUtc datetime
)
AS
	DECLARE @ApplicationID uniqueidentifier

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Applications WHERE ApplicationName=@ApplicationName)
	
	-- Check for Unique Email Applicaiton Missing
	
	UPDATE yafprov_Membership SET
	Username = @Username,
	Email = @Email,
	Comment = @Comment,
	IsApproved = @IsApproved,
	LastLoginDate = @LastLoginDate,
	LastActivityDate = @LastActivityDate
	WHERE ApplicationID = @ApplicationID AND
	UserKey = @UserKey;

END
GO                 

CREATE PROCEDURE dbo.yafprov_createapplication
(
@ApplicationName nvarchar(50),
@ApplicationID uniqueidentifier OUTPUT
)
AS
	DECLARE @ApplicationID uniqueidentifier

	SET @ApplicationID = (SELECT ApplicationID FROM yafprov_Applications WHERE ApplicationName=@ApplicationName)
	
	IF (@ApplicationID IS Null)
		    SELECT  @ApplicationId = NEWID()
            INSERT  yafprov_Applications(ApplicationId, ApplicationName)
            VALUES  (@ApplicationId, @ApplicationName)
END
GO