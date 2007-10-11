-- =============================================
-- Author:		Mek
-- Create date: 30 September 2007
-- Description:	MembershipProvider Tables
-- =============================================
-- To do list
-- ---------------------------------------------
-- Primary Keys for tables


IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'yafprov_Member') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [dbo].[yafprov_Membership](
		[UserID] [uniqueidentifier] NOT NULL,
		[ApplicationID] [uniqueidentifier] NOT NULL,
		[Username] [nvarchar](255) NOT NULL,
		[Password] [nvarchar](255) NULL,
		[PasswordSalt] [nvarchar](255) NULL,
		[PasswordFormat] [nvarchar](255) NULL,
		[Email] [nvarchar](255) NULL,
		[PasswordQuestion] [nvarchar](255) NULL,
		[PasswordAnswer] [nvarchar](255) NULL,
		[IsApproved] [bit] NULL,
		[IsLockedOut] [bit] NULL,
		[LastLogin] [datetime] NULL,
		[LastActivity] [datetime] NULL,
		[LastPasswordChange] [datetime] NULL,
		[LastLockOutDate] [datetime] NULL,
		[FailedPasswordAttempts] [int] NULL,
		[FailedAnswerAttempts] [int] NULL,
		[FailedPasswordWindow] [datetime] NULL,
		[FailedAnswerWindow] [datetime] NULL,
		[Joined] [datetime] NULL,
		[Comment] [ntext] NULL
		)
go

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'yafprov_Application') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [dbo].[yafprov_Application](
		[ApplicationID] [uniqueidentifier] NOT NULL,
		[ApplicationName] [nvarchar](255) NULL,
		[Description] [ntext] NULL
		)
go

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[yafprov_Profile]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [dbo].[yafprov_Profile]
	(
		[UserID] [uniqueidentifier] NOT NULL PRIMARY KEY,
		[LastUpdatedDate]	[datetime] NOT NULL
	)
go


