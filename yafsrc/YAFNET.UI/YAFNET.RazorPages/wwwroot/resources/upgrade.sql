-- Create AspNetUsers Table
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [{databaseOwner}].[{objectQualifier}AspNetUsers](
    [Id]            NVARCHAR (128) NOT NULL,
    [UserName]      NVARCHAR (MAX) NULL,
    [PasswordHash]  NVARCHAR (MAX) NULL,
    [SecurityStamp] NVARCHAR (MAX) NULL,
    [EmailConfirmed]       BIT            NOT NULL,
    [PhoneNumber]          NVARCHAR (MAX) NULL,
    [PhoneNumberConfirmed] BIT            NOT NULL,
    [TwoFactorEnabled]     BIT            NOT NULL,
    [LockoutEndDateUtc]    DATETIME       NULL,
    [LockoutEnabled]       BIT            NOT NULL,
    [AccessFailedCount]    INT            NOT NULL,
    [ApplicationId]                          UNIQUEIDENTIFIER NOT NULL,
    [LegacyPasswordHash]  NVARCHAR (MAX) NULL,
    [LoweredUserName]  NVARCHAR (256)   NOT NULL,
    [MobileAlias]      NVARCHAR (16)    DEFAULT (NULL) NULL,
    [IsAnonymous]      BIT              DEFAULT ((0)) NOT NULL,
    [LastActivityDate] DATETIME2         NOT NULL,
    [MobilePIN]                              NVARCHAR (16)    NULL,
    [Email]                                  NVARCHAR (256)   NULL,
    [LoweredEmail]                           NVARCHAR (256)   NULL,
    [PasswordQuestion]                       NVARCHAR (256)   NULL,
    [PasswordAnswer]                         NVARCHAR (128)   NULL,
    [IsApproved]                             BIT              NOT NULL,
    [IsLockedOut]                            BIT              NOT NULL,
    [CreateDate]                             DATETIME2               NOT NULL,
    [LastLoginDate]                          DATETIME2         NOT NULL,
    [LastPasswordChangedDate]                DATETIME2         NOT NULL,
    [LastLockoutDate]                        DATETIME2         NOT NULL,
    [FailedPasswordAttemptCount]             INT              NOT NULL,
    [FailedPasswordAttemptWindowStart]       DATETIME2         NOT NULL,
    [FailedPasswordAnswerAttemptCount]       INT              NOT NULL,
    [FailedPasswordAnswerAttemptWindowStart] DATETIME2         NOT NULL,
    [Comment]                                NTEXT            NULL,
    [Profile_Birthday] DateTime NULL,
    [Profile_Blog] NVARCHAR (255) NULL,
    [Profile_Gender] INT NULL,
    [Profile_Homepage] NVARCHAR (255) NULL,
    [Profile_Facebook] NVARCHAR (400) NULL,
    [Profile_Interests] NVARCHAR (4000) NULL,
    [Profile_Location] NVARCHAR (255) NULL,
    [Profile_Country] NVARCHAR (2) NULL,
    [Profile_Region] NVARCHAR (255) NULL,
    [Profile_City] NVARCHAR (255) NULL,
    [Profile_Occupation] NVARCHAR (400) NULL,
    [Profile_RealName] NVARCHAR (255) NULL,
    [Profile_Skype] NVARCHAR (255) NULL,
    [Profile_XMPP] NVARCHAR (255) NULL
    CONSTRAINT [PK_{databaseOwner}.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

-- Create missing profile columns first if not exist

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and name='Interests')
begin
    alter table [{databaseOwner}].[{objectQualifier}prov_Profile] add Interests nvarchar(255)  Null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and name='Blog')
begin
    alter table [{databaseOwner}].[{objectQualifier}prov_Profile] add Blog nvarchar(255)  Null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and name='Occupation')
begin
    alter table [{databaseOwner}].[{objectQualifier}prov_Profile] add Occupation nvarchar(255)  Null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and name='Skype')
begin
    alter table [{databaseOwner}].[{objectQualifier}prov_Profile] add Skype nvarchar(255)  Null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and name='RealName')
begin
    alter table [{databaseOwner}].[{objectQualifier}prov_Profile] add RealName nvarchar(255)  Null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and name='Location')
begin
    alter table [{databaseOwner}].[{objectQualifier}prov_Profile] add [Location] nvarchar(255)  Null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and name='Homepage')
begin
    alter table [{databaseOwner}].[{objectQualifier}prov_Profile] add Homepage nvarchar(255)  Null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and name='Gender')
begin
    alter table [{databaseOwner}].[{objectQualifier}prov_Profile] add Gender int  Null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and name='Birthday')
begin
    alter table [{databaseOwner}].[{objectQualifier}prov_Profile] add Birthday datetime  Null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and name='Facebook')
begin
    alter table [{databaseOwner}].[{objectQualifier}prov_Profile] add Facebook nvarchar(255)  Null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and name='Country')
begin
    alter table [{databaseOwner}].[{objectQualifier}prov_Profile] add Country nvarchar(255) Null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and name='Region')
begin
    alter table [{databaseOwner}].[{objectQualifier}prov_Profile] add Region nvarchar(255) Null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and name='City')
begin
    alter table [{databaseOwner}].[{objectQualifier}prov_Profile] add City nvarchar(255) Null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_Profile]') and name='XMPP')
begin
    alter table [{databaseOwner}].[{objectQualifier}prov_Profile] add XMPP nvarchar(255) Null
end
GO


 -- Migrate users standard provider
if exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[aspnet_Users]') and type in (N'U'))
begin
  INSERT INTO [{databaseOwner}].[{objectQualifier}AspNetUsers] (
        ApplicationId,
        Id,
        UserName,
        PasswordHash,
        SecurityStamp,
        EmailConfirmed,
        PhoneNumber,
        PhoneNumberConfirmed,
        TwoFactorEnabled,
        LockoutEndDateUtc,
        LockoutEnabled,
        AccessFailedCount,
        Email,
        LoweredUserName,
        LastActivityDate,
        IsApproved,
        IsLockedOut,
        CreateDate,
        LastLoginDate,
        LastPasswordChangedDate,
        LastLockOutDate,
        FailedPasswordAttemptCount,
        FailedPasswordAnswerAttemptWindowStart,
        FailedPasswordAnswerAttemptCount,
        FailedPasswordAttemptWindowStart,
        PasswordQuestion,
        PasswordAnswer
      )
  SELECT
      [{databaseOwner}].[aspnet_Users].ApplicationId,
      [{databaseOwner}].[aspnet_Users].UserId,
      [{databaseOwner}].[aspnet_Users].UserName,
      ([{databaseOwner}].[aspnet_Membership].Password+'|'+CAST([{databaseOwner}].[aspnet_Membership].PasswordFormat as varchar)+'|'+ [{databaseOwner}].[aspnet_Membership].PasswordSalt),
      NewID(),
      1,
      NULL,
      0,
      1,
      CASE WHEN [{databaseOwner}].[aspnet_Membership].IsLockedOut = 1 THEN DATEADD(YEAR, 1000, SYSUTCDATETIME()) ELSE NULL END,
      1,
      0,
      [{databaseOwner}].[aspnet_Membership].Email,
      [{databaseOwner}].[aspnet_Users].LoweredUserName,
      [{databaseOwner}].[aspnet_Users].LastActivityDate,
      [{databaseOwner}].[aspnet_Membership].IsApproved,
      [{databaseOwner}].[aspnet_Membership].IsLockedOut,
      [{databaseOwner}].[aspnet_Membership].CreateDate,
      [{databaseOwner}].[aspnet_Membership].LastLoginDate,
      [{databaseOwner}].[aspnet_Membership].LastPasswordChangedDate,
      [{databaseOwner}].[aspnet_Membership].LastLockOutDate,
      [{databaseOwner}].[aspnet_Membership].FailedPasswordAttemptCount,
      [{databaseOwner}].[aspnet_Membership].FailedPasswordAnswerAttemptWindowStart,
      [{databaseOwner}].[aspnet_Membership].FailedPasswordAnswerAttemptCount,
      [{databaseOwner}].[aspnet_Membership].FailedPasswordAttemptWindowStart,
      [{databaseOwner}].[aspnet_Membership].PasswordQuestion,
      [{databaseOwner}].[aspnet_Membership].PasswordAnswer
  FROM
      [{databaseOwner}].[aspnet_Users]
      LEFT OUTER JOIN [{databaseOwner}].[aspnet_Membership] ON [{databaseOwner}].[aspnet_Membership].ApplicationId = [{databaseOwner}].[aspnet_Users].ApplicationId
      AND [{databaseOwner}].[aspnet_Users].UserId = [{databaseOwner}].[aspnet_Membership].UserId
      LEFT OUTER JOIN [{databaseOwner}].[{objectQualifier}AspNetUsers] ON [{databaseOwner}].[aspnet_Membership].UserId = [{databaseOwner}].[{objectQualifier}AspNetUsers].Id
  WHERE
      [{databaseOwner}].[{objectQualifier}AspNetUsers].Id IS NULL and [{databaseOwner}].[aspnet_Membership].IsApproved is not null
      end
GO


 -- Migrate users yaf.net provider
  INSERT INTO [{databaseOwner}].[{objectQualifier}AspNetUsers] (
       ApplicationId,
       Id,
       UserName,
       PasswordHash,
       SecurityStamp,
       EmailConfirmed,
       PhoneNumber,
       PhoneNumberConfirmed,
       TwoFactorEnabled,
       LockoutEndDateUtc,
       LockoutEnabled,
       AccessFailedCount,
       Email,
       LoweredUserName,
       LastActivityDate,
       IsApproved,
       IsLockedOut,
       CreateDate,
       LastLoginDate,
       LastPasswordChangedDate,
       LastLockOutDate,
       FailedPasswordAttemptCount,
       FailedPasswordAnswerAttemptWindowStart,
       FailedPasswordAnswerAttemptCount,
       FailedPasswordAttemptWindowStart,
       PasswordQuestion,
       PasswordAnswer,
       Profile_Birthday,
       Profile_Blog,
       Profile_Gender,
       Profile_Homepage,
       Profile_Facebook,
       Profile_Interests,
       Profile_Location,
       Profile_Country,
       Profile_Region,
       Profile_City,
       Profile_Occupation,
       Profile_RealName,
       Profile_Skype,
       Profile_XMPP
      )
  SELECT
       [{databaseOwner}].[{objectQualifier}prov_Membership].ApplicationId,
       [{databaseOwner}].[{objectQualifier}prov_Membership].UserId,
       [{databaseOwner}].[{objectQualifier}prov_Membership].UserName,
      ([{databaseOwner}].[{objectQualifier}prov_Membership].Password+'|'+CAST([{databaseOwner}].[{objectQualifier}prov_Membership].PasswordFormat as varchar)+'|'+ [{databaseOwner}].[{objectQualifier}prov_Membership].PasswordSalt),
      NewID(),
      1,
      NULL,
      0,
      1,
      CASE WHEN [{databaseOwner}].[{objectQualifier}prov_Membership].IsLockedOut = 1 THEN DATEADD(YEAR, 1000, SYSUTCDATETIME()) ELSE NULL END,
      1,
      0,
      [{databaseOwner}].[{objectQualifier}prov_Membership].Email,
      [{databaseOwner}].[{objectQualifier}prov_Membership].UsernameLwd,
      IsNull([{databaseOwner}].[{objectQualifier}prov_Membership].LastActivity,cast('1753-1-1' as datetime)),
      [{databaseOwner}].[{objectQualifier}prov_Membership].IsApproved,
      0,
      IsNull([{databaseOwner}].[{objectQualifier}prov_Membership].Joined,cast('1753-1-1' as datetime)),
      IsNull([{databaseOwner}].[{objectQualifier}prov_Membership].Joined,cast('1753-1-1' as datetime)),
      cast('1753-1-1' as datetime),
      cast('1753-1-1' as datetime),
      0,
      cast('1753-1-1' as datetime),
      0,
      cast('1753-1-1' as datetime),
      [{databaseOwner}].[{objectQualifier}prov_Membership].PasswordQuestion,
      [{databaseOwner}].[{objectQualifier}prov_Membership].PasswordAnswer,
      p.Birthday,
      p.Blog,
      p.Gender,
      p.Homepage,
      p.Facebook,
      p.Interests,
      p.Location,
      p.Country,
      p.Region,
      p.City,
      p.Occupation,
      p.RealName,
      p.Skype,
      p.XMPP
  FROM
      [{databaseOwner}].[{objectQualifier}prov_Membership]
      LEFT OUTER JOIN [{databaseOwner}].[{objectQualifier}AspNetUsers] ON [{databaseOwner}].[{objectQualifier}prov_Membership].UserId = [{databaseOwner}].[{objectQualifier}AspNetUsers].Id
      LEFT OUTER JOIN [{databaseOwner}].[{objectQualifier}prov_Profile] p ON (p.UserID = [{databaseOwner}].[{objectQualifier}prov_Membership].UserId)
  WHERE [{databaseOwner}].[{objectQualifier}AspNetUsers].Id IS NULL


-- Create AspNetRoles Table
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [{databaseOwner}].[{objectQualifier}AspNetRoles](
  [Id] [nvarchar](128) NOT NULL,
  [Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED
(
  [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

-- Create AspNetUserLogins Table
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [{databaseOwner}].[{objectQualifier}AspNetUserLogins](
  [LoginProvider] [nvarchar](128) NOT NULL,
  [ProviderKey] [nvarchar](128) NOT NULL,
  [UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED
(
  [LoginProvider] ASC,
  [ProviderKey] ASC,
  [UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

ALTER TABLE [{databaseOwner}].[{objectQualifier}AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [{databaseOwner}].[{objectQualifier}AspNetUsers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [{databaseOwner}].[{objectQualifier}AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO

-- Create AspNetUserClaims Table
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [{databaseOwner}].[{objectQualifier}AspNetUserClaims](
  [Id] [int] IDENTITY(1,1) NOT NULL,
  [UserId] [nvarchar](128) NOT NULL,
  [ClaimType] [nvarchar](max) NULL,
  [ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED
(
  [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

ALTER TABLE [{databaseOwner}].[{objectQualifier}AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [{databaseOwner}].[{objectQualifier}AspNetUsers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [{databaseOwner}].[{objectQualifier}AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO



-- Import Provider Roles
if exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[aspnet_Roles]') and type in (N'U'))
begin
INSERT INTO [{databaseOwner}].[{objectQualifier}AspNetRoles](Id,Name)
SELECT RoleId,RoleName
FROM [{databaseOwner}].[aspnet_Roles]
end
GO

INSERT INTO [{databaseOwner}].[{objectQualifier}AspNetRoles](Id,Name)
SELECT RoleId,RoleName
FROM [{databaseOwner}].[{objectQualifier}prov_Role]
GO

-- Create AspNetUserRoles Table
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [{databaseOwner}].[{objectQualifier}AspNetUserRoles] (
    [UserId] NVARCHAR (128) NOT NULL,
    [RoleId] NVARCHAR (128) NOT NULL
);

GO

-- Import User Roles
if exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[aspnet_usersInRoles]') and type in (N'U'))
begin
   INSERT INTO [{databaseOwner}].[{objectQualifier}AspNetUserRoles](UserId,RoleId)
   SELECT UserId,RoleId
   FROM [{databaseOwner}].[aspnet_UsersInRoles]
end
GO

INSERT INTO [{databaseOwner}].[{objectQualifier}AspNetUserRoles](UserId,RoleId)
SELECT UserId,RoleId
FROM [{databaseOwner}].[{objectQualifier}prov_RoleMembership]
GO


declare @ApplicationID uniqueidentifier;


set @ApplicationID = (select top 1 ApplicationId from [{databaseOwner}].[{objectQualifier}AspNetUsers])

-- Import Application Id
insert into [{databaseOwner}].[{objectQualifier}Registry](Name,Value) values('applicationid',@ApplicationID)
go
