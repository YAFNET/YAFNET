-- =============================================
-- Author:		Mek
-- Create date: 30 September 2007
-- Description:	MembershipProvider SPROCS
-- =============================================

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafp_changepassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafp_changepassword]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafp_changepasswordquestionandanswer]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafp_changepasswordquestionandanswer]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafp_createuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafp_createuser]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafp_deleteuser]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafp_deleteuser]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafp_findusersbyemail]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafp_findusersbyemail]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yafp_findusersbyname]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yafp_findusersbyname]
GO

CREATE PROCEDURE dbo.yafp_changepassword
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

	SET @ApplicationID = (SELECT ApplicationID FROM yafp_Applications WHERE ApplicationName=@ApplicationName)

	UPDATE yafp_Members SET Password=@Password, PasswordSalt=@PasswordSalt,
		PasswordFormat=@PasswordFormat, PasswordAnswer=@PasswordAnswer
	WHERE Username=@Username and ApplicationID=@ApplicationID;

END
GO

CREATE PROCEDURE dbo.yafp_changepasswordquestionandanswer
(
@ApplicationName nvarchar(50),
@Username nvarchar(50),
@PasswordQuestion nvarchar(50),
@PasswordAnswer nvarchar(50)
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier
	
	SET @ApplicationID = (SELECT ApplicationID FROM yafp_Applications WHERE ApplicationName=@ApplicationName)
	
	UPDATE yafp_Members SET PasswordQuestion=@PasswordQuestion, PasswordAnswer=@PasswordAnswer
	WHERE Username=@Username and ApplicationID=@ApplicationID;

END
GO

CREATE PROCEDURE dbo.yafp_createuser
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

	SET @ApplicationID = (SELECT ApplicationID FROM yafp_Applications WHERE ApplicationName=@ApplicationName)
	
	INSERT INTO yafp_Members(MembershipID,ApplicationID,Username,Password,PassworldSalt,PasswordFormat,Email,PasswordQuestion,PasswordAnswer,IsApproved)
	VALUES (@UserKey, @ApplicationID,@Username, @Password, @PassworldSalt, @PasswordFormat, @Email, @PasswordQuestion, @PasswordAnswer, @IsApproved);

END
GO

CREATE PROCEDURE dbo.yafp_deleteuser
(
@ApplicationName nvarchar(50),
@Username nvarchar(50),
@DeleteAllRelated bit
)
AS
BEGIN
	DECLARE @ApplicationID uniqueidentifier

	SET @ApplicationID = (SELECT ApplicationID FROM yafp_Applications WHERE ApplicationName=@ApplicationName)

	DELETE FROM yafp_Members WHERE ApplicationID=@ApplicationID AND Username=@Username;

	--INSERT IF STATEMENT TO DELETE MEMBERSHIP/ROLES INFORMATION / PROFILE INFORMATION	
END
GO

CREATE PROCEDURE dbo.yafp_findusersbyemail
(
@ApplicationName nvarchar(50),
@EmailAddress nvarchar(50),
@PageIndex int,
@PageSize int
)
AS
BEGIN
	SELECT yafp_Membership.* FROM yafp_Membership LEFT JOIN yafp_Applications ON yafp_Membership.ApplicationID = yafp_Applications.ApplicationID
	WHERE yafp_Application.ApplicationName = @ApplicationName AND yafp_Membersip.Email = @EmailAddress;
END
GO

CREATE PROCEDURE dbo.yafp_findusersbyname
(
@ApplicationName nvarchar(50),
@Username nvarchar(50),
@PageIndex int,
@PageSize int
)
AS
BEGIN
	SELECT yafp_Membership.* FROM yafp_Membership LEFT JOIN yafp_Applications ON yafp_Membership.ApplicationID = yafp_Applications.ApplicationID
	WHERE yafp_Application.ApplicationName = @ApplicationName AND yafp_Membersip.Username = @Username;
END
GO
