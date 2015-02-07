-- =============================================
-- Author:		Mek
-- Create date: 30 September 2007
-- Description:	MembershipProvider Tables
-- =============================================

IF not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_Membership]') and type in (N'U'))
	CREATE TABLE [{databaseOwner}].[{objectQualifier}prov_Membership](
		[UserID] [nvarchar](64) NOT NULL,
		[ApplicationID] [uniqueidentifier] NOT NULL,
		[Username] [nvarchar](256) NOT NULL,
		[UsernameLwd] [nvarchar](256) NOT NULL,
		[Password] [nvarchar](256) NULL,
		[PasswordSalt] [nvarchar](256) NULL,
		[PasswordFormat] [nvarchar](256) NULL,
		[Email] [nvarchar](256) NULL,
		[EmailLwd] [nvarchar](256) NULL,
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
		[Comment] [ntext] NULL, 
		CONSTRAINT [PK_{objectQualifier}prov_Membership] PRIMARY KEY CLUSTERED ([UserID] ASC)
		)
GO

IF not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_Application]') and type in (N'U'))
	CREATE TABLE [{databaseOwner}].[{objectQualifier}prov_Application](
		[ApplicationID] [uniqueidentifier] NOT NULL,
		[ApplicationName] [nvarchar](256) NULL,
		[ApplicationNameLwd] [nvarchar](256) NULL,
		[Description] [ntext] NULL,
		CONSTRAINT [PK_{objectQualifier}prov_Application] PRIMARY KEY CLUSTERED ([ApplicationID] ASC)
		)
GO

IF not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_Profile]') and type in (N'U'))
	CREATE TABLE [{databaseOwner}].[{objectQualifier}prov_Profile]
	(
		[UserID] [nvarchar](64) NOT NULL,
		[LastUpdatedDate] [datetime] NOT NULL,
		CONSTRAINT [PK_{objectQualifier}prov_Profile] PRIMARY KEY CLUSTERED ([UserID] ASC)
	)
GO

IF not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_Role]') and type in (N'U'))
	CREATE TABLE [{databaseOwner}].[{objectQualifier}prov_Role]
	(
	[RoleID] [uniqueidentifier] NOT NULL,
	[ApplicationID] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](256) NOT NULL,
	[RoleNameLwd] [nvarchar](256) NOT NULL,
	CONSTRAINT [PK_{objectQualifier}prov_Role] PRIMARY KEY CLUSTERED ([RoleID] ASC)
	)
GO

IF not exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}prov_RoleMembership]') and type in (N'U'))
	CREATE TABLE [{databaseOwner}].[{objectQualifier}prov_RoleMembership]
	(
	[RoleID] [uniqueidentifier] NOT NULL,
	[UserID] [nvarchar](64) NOT NULL
	)
GO

if exists(select 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Membership]') and name='UserID' and system_type_id='36')
begin
	if exists(select 1 from sys.indexes where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Membership]') and is_primary_key = 1)
	begin
		-- drop the primary key constrant
		DECLARE @PrimaryIXName nvarchar(255)		
		SET @PrimaryIXName = (select [name] from sys.indexes where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Membership]') and is_primary_key = 1)
		exec('ALTER TABLE [{databaseOwner}].[{objectQualifier}prov_Membership] DROP CONSTRAINT ' + @PrimaryIXName);
	end
	-- alter the column
	ALTER TABLE [{databaseOwner}].[{objectQualifier}prov_Membership] ALTER COLUMN UserID nvarchar(64) NOT NULL
	-- add primary key constraint back...
	ALTER TABLE [{databaseOwner}].[{objectQualifier}prov_Membership] ADD CONSTRAINT [PK_{objectQualifier}prov_Membership] PRIMARY KEY CLUSTERED ([UserID] ASC)
end
GO

if exists(select 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and name='UserID' and system_type_id='36')
begin
	if exists(select 1 from sys.indexes where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and is_primary_key = 1)
	begin
		-- drop the primary key constrant
		DECLARE @PrimaryIXName nvarchar(255)		
		SET @PrimaryIXName = (select [name] from sys.indexes where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and is_primary_key = 1)
		exec('ALTER TABLE [{databaseOwner}].[{objectQualifier}prov_Profile] DROP CONSTRAINT ' + @PrimaryIXName);
	end
	-- alter the column
	ALTER TABLE [{databaseOwner}].[{objectQualifier}prov_Profile] ALTER COLUMN UserID nvarchar(64) NOT NULL
	-- add primary key constraint back...
	ALTER TABLE [{databaseOwner}].[{objectQualifier}prov_Profile] ADD CONSTRAINT [PK_{objectQualifier}prov_Profile] PRIMARY KEY CLUSTERED ([UserID] ASC)
end
GO

if exists(select 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_RoleMembership]') and name='UserID' and system_type_id='36')
begin
	-- drop the provider user key index if it exists...
	if exists(select 1 from sys.indexes where name=N'IX_{objectQualifier}prov_RoleMembership_UserID' and object_id=object_id(N'[{databaseOwner}].[{objectQualifier}prov_RoleMembership]'))
	begin
		DROP INDEX [IX_{objectQualifier}prov_RoleMembership_UserID] ON [{databaseOwner}].[{objectQualifier}prov_RoleMembership]
	end
	-- alter the column
	ALTER TABLE [{databaseOwner}].[{objectQualifier}prov_RoleMembership] ALTER COLUMN UserID nvarchar(64) NOT NULL
end
GO