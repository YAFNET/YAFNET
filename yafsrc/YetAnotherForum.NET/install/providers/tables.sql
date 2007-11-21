-- =============================================
-- Author:		Mek
-- Create date: 30 September 2007
-- Description:	MembershipProvider Tables
-- =============================================

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_Membership]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [{databaseOwner}].[{objectQualifier}prov_Membership](
		[UserID] [uniqueidentifier] NOT NULL PRIMARY KEY,
		[ApplicationID] [uniqueidentifier] NOT NULL,
		[Username] [nvarchar](256) NOT NULL,
		[Password] [nvarchar](256) NULL,
		[PasswordSalt] [nvarchar](256) NULL,
		[PasswordFormat] [nvarchar](256) NULL,
		[Email] [nvarchar](256) NULL,
		[PasswordQuestion] [nvarchar](256) NULL,
		[PasswordAnswer] [nvarchar](256) NULL,
		[IsApproved] [bit] NULL,
		[IsLockedOut] [bit] NULL,
		[LastLogin] [datetime] NULL,
		[LastActivity] [datetime] NULL,
		[LastPasswordChange] [datetime] NULL,
		[LastLockOut] [datetime] NULL,
		[FailedPasswordAttempts] [int] NULL,
		[FailedAnswerAttempts] [int] NULL,
		[FailedPasswordWindow] [datetime] NULL,
		[FailedAnswerWindow] [datetime] NULL,
		[Joined] [datetime] NULL,
		[Comment] [ntext] NULL
		)
GO

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_Application]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [{databaseOwner}].[{objectQualifier}prov_Application](
		[ApplicationID] [uniqueidentifier] NOT NULL PRIMARY KEY,
		[ApplicationName] [nvarchar](256) NULL,
		[Description] [ntext] NULL
		)
GO

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_Profile]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [{databaseOwner}].[{objectQualifier}prov_Profile]
	(
		[UserID] [uniqueidentifier] NOT NULL PRIMARY KEY,
		[LastUpdatedDate]	[datetime] NOT NULL
	)
GO

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_Role]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [{databaseOwner}].[{objectQualifier}prov_Role]
	(
	[RoleID] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[ApplicationID] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](256) NOT NULL
	)
GO

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_RoleMembership]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	CREATE TABLE [{databaseOwner}].[{objectQualifier}prov_RoleMembership]
	(
	[RoleID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL
	)
GO
