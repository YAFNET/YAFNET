if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}Board]') and name='BoardUID')
begin
alter table [{databaseOwner}].[{objectQualifier}Board] drop column  BoardUID
end
GO

-- Active Table
if exists (select top 1 1 from sys.columns where object_id = object_id(N'[{databaseOwner}].[{objectQualifier}Active]') and name='Location' and precision < 255)
    alter table [{databaseOwner}].[{objectQualifier}Active] alter column [Location] nvarchar(255) NOT NULL
GO

if not exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}Active]') and name='ForumPage')
begin
    alter table [{databaseOwner}].[{objectQualifier}Active] add [ForumPage] nvarchar(255)
end
GO

if exists (select top 1 1 from sys.columns where object_id = object_id(N'[{databaseOwner}].[{objectQualifier}Active]') and name='ForumPage' and precision < 1024)
    alter table [{databaseOwner}].[{objectQualifier}Active] alter column [ForumPage] nvarchar(1024)
GO

if exists (select top 1 1 from sys.columns where object_id = object_id(N'[{databaseOwner}].[{objectQualifier}Active]') and name='IP' and precision < 39)
    alter table [{databaseOwner}].[{objectQualifier}Active] alter column [IP] varchar(39) not null
GO

if not exists (select top 1 1 from sys.columns where object_id = object_id(N'[{databaseOwner}].[{objectQualifier}Active]') and name='Flags')
    alter table [{databaseOwner}].[{objectQualifier}Active] add [Flags] int NULL
GO

if exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Active]') and type in (N'U'))
    grant delete on [{databaseOwner}].[{objectQualifier}Active] to public
    exec('delete from [{databaseOwner}].[{objectQualifier}Active]')
    revoke delete on [{databaseOwner}].[{objectQualifier}Active] from public
GO

-- Board Table
if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}Board]') and name='MembershipAppName')
begin
    alter table [{databaseOwner}].[{objectQualifier}Board] drop column MembershipAppName
end
GO

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}Board]') and name='RolesAppName')
begin
    alter table [{databaseOwner}].[{objectQualifier}Board] drop column RolesAppName
end
GO

-- UserPMessage Table
if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and name='Flags')
begin
    -- add new "Flags" field to UserPMessage
    alter table [{databaseOwner}].[{objectQualifier}UserPMessage] add Flags int not null  constraint [DF_{objectQualifier}UserPMessage_Flags] default (0)
end
GO

if exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and name='IsRead')
BEGIN
    if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and name='IsArchived')
    BEGIN
        -- Copy "IsRead" value over
        grant update on [{databaseOwner}].[{objectQualifier}UserPMessage] to public
        exec('update [{databaseOwner}].[{objectQualifier}UserPMessage] set Flags = IsRead')
        revoke update on [{databaseOwner}].[{objectQualifier}UserPMessage] from public

        -- drop the old column
        alter table [{databaseOwner}].[{objectQualifier}UserPMessage] drop column IsRead

        -- Verify flags isn't NULL
        grant update on [{databaseOwner}].[{objectQualifier}UserPMessage] to public
        exec('update [{databaseOwner}].[{objectQualifier}UserPMessage] set Flags = 1 WHERE Flags IS NULL')
        revoke update on [{databaseOwner}].[{objectQualifier}UserPMessage] from public

        -- add new calculated columns
        alter table [{databaseOwner}].[{objectQualifier}UserPMessage] ADD [IsRead] AS (CONVERT([bit],sign([Flags]&(1)),(0)))
        alter table [{databaseOwner}].[{objectQualifier}UserPMessage] ADD [IsInOutbox] AS (CONVERT([bit],sign([Flags]&(2)),(0)))
        alter table [{databaseOwner}].[{objectQualifier}UserPMessage] ADD [IsArchived] AS (CONVERT([bit],sign([Flags]&(4)),(0)))
    END
END
GO

IF NOT exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') AND NAME='IsDeleted')
BEGIN
    alter table [{databaseOwner}].[{objectQualifier}UserPMessage] ADD [IsDeleted] AS (CONVERT([bit],sign([Flags]&(8)),(0)))
END
GO

-- User Table
if exists(select top 1 1 from [{databaseOwner}].[{objectQualifier}Group] where ([Flags] & 2)=2)
begin
  update [{databaseOwner}].[{objectQualifier}User] set [Flags] = [Flags] | 4 where UserID in (select distinct UserID from [{databaseOwner}].[{objectQualifier}UserGroup] a join [{databaseOwner}].[{objectQualifier}Group] b on b.GroupID=a.GroupID and (b.[Flags] & 2)=2)
end
GO

if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsApproved')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] ADD [IsApproved] AS (CONVERT([bit],sign([Flags]&(2)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}User]') and name='NotificationType')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] ADD NotificationType int default(10)
end
GO

if exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}User]') and name='NotificationType')
begin
    update  [{databaseOwner}].[{objectQualifier}User] SET [NotificationType]=10 WHERE [NotificationType] IS NULL
end
GO

if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsActiveExcluded')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] ADD [IsActiveExcluded] AS (CONVERT([bit],sign([Flags]&(16)),(0)))
end
GO

if exists(select top 1 1 from sys.columns where object_id =  object_id(N'[{databaseOwner}].[{objectQualifier}User]') and name=N'Signature' and system_type_id<>99)
    alter table [{databaseOwner}].[{objectQualifier}User] alter column Signature nvarchar(max) null
go

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Flags')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [Flags] int not null constraint DF_{objectQualifier}User_Flags default (0)
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='IsQuestion')
begin
    alter table [{databaseOwner}].[{objectQualifier}Topic] add IsQuestion AS (CONVERT([bit],sign([Flags]&(1024)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='AnswerMessageId')
begin
    alter table [{databaseOwner}].[{objectQualifier}Topic] add AnswerMessageId INT NULL
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='TopicImage')
begin
    alter table [{databaseOwner}].[{objectQualifier}Topic] add TopicImage nvarchar(255) NULL
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsHostAdmin')
begin
    grant update on [{databaseOwner}].[{objectQualifier}User] to public
    exec('update [{databaseOwner}].[{objectQualifier}User] set Flags = Flags | 1 where IsHostAdmin<>0')
    revoke update on [{databaseOwner}].[{objectQualifier}User] from public
    alter table [{databaseOwner}].[{objectQualifier}User] drop column IsHostAdmin
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Approved')
begin
    grant update on [{databaseOwner}].[{objectQualifier}User] to public
    exec('update [{databaseOwner}].[{objectQualifier}User] set Flags = Flags | 2 where Approved<>0')
    revoke update on [{databaseOwner}].[{objectQualifier}User] from public
    alter table [{databaseOwner}].[{objectQualifier}User] drop column Approved
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='ProviderUserKey')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add ProviderUserKey nvarchar(64)
end
GO

if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}User]') and name='DisplayName')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] ADD [DisplayName] nvarchar(255)
    grant update on [{databaseOwner}].[{objectQualifier}User] to public
    exec('update [{databaseOwner}].[{objectQualifier}User] SET DisplayName = [Name]')
    revoke update on [{databaseOwner}].[{objectQualifier}User] from public
    ALTER TABLE [{databaseOwner}].[{objectQualifier}User] ALTER COLUMN [DisplayName] nvarchar(255) NOT NULL
end
GO

-- convert uniqueidentifier to nvarchar(64)
if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='ProviderUserKey' and system_type_id='36')
begin
    -- drop the provider user key index if it exists...
    if exists(select 1 from sys.indexes where name=N'IX_{objectQualifier}User_ProviderUserKey' and object_id=object_id(N'[{databaseOwner}].[{objectQualifier}User]'))
    begin
        DROP INDEX [IX_{objectQualifier}User_ProviderUserKey] ON [{databaseOwner}].[{objectQualifier}User]
    end
    -- alter the column
    ALTER TABLE [{databaseOwner}].[{objectQualifier}User] ALTER COLUMN ProviderUserKey nvarchar(64)
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='PMNotification')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [PMNotification] [bit] NOT NULL constraint [DF_{objectQualifier}User_PMNotification] default (1)
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='DailyDigest')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [DailyDigest] [bit] NOT NULL constraint [DF_{objectQualifier}User_DailyDigest] default (0)
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='AutoWatchTopics')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [AutoWatchTopics] [bit] NOT NULL constraint [DF_{objectQualifier}User_AutoWatchTopics] default (0)
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Points')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [Points] [int] NOT NULL constraint [DF_{objectQualifier}User_Points] default (0)
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='AvatarImageType')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [AvatarImageType] nvarchar(50) NULL
end
GO

-- make sure the gender column is nullable
if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Gender')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] alter column Gender tinyint NULL
end
GO

-- Add 8-letter Language Code column
if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Culture')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add Culture varchar(10) NULL
end
GO

if exists(select top 1 1 from sys.columns where object_id =  object_id(N'[{databaseOwner}].[{objectQualifier}User]') and name=N'IP' and precision < 39)
    alter table [{databaseOwner}].[{objectQualifier}User] alter column [IP] varchar(39) null
go

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Name' and precision < 255)
begin
    alter table [{databaseOwner}].[{objectQualifier}User] alter column [Name] nvarchar(255) not null
end
GO

-- Only remove User table columns if version is 30+
IF EXISTS (SELECT ver FROM (SELECT CAST(CAST(Value as nvarchar(255)) as int) as ver FROM [{databaseOwner}].[{objectQualifier}Registry] WHERE Name = 'version') reg WHERE ver > 30)
BEGIN
    if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Gender')
    begin
        alter table [{databaseOwner}].[{objectQualifier}User] drop column Gender
    end

    if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Location')
    begin
        alter table [{databaseOwner}].[{objectQualifier}User] drop column Location
    end

    if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='HomePage')
    begin
        alter table [{databaseOwner}].[{objectQualifier}User] drop column HomePage
    end

    if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='MSN')
    begin
        alter table [{databaseOwner}].[{objectQualifier}User] drop column MSN
    end

    if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='YIM')
    begin
        alter table [{databaseOwner}].[{objectQualifier}User] drop column YIM
    end

    if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='AIM')
    begin
        alter table [{databaseOwner}].[{objectQualifier}User] drop column AIM
    end

    if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='ICQ')
    begin
        alter table [{databaseOwner}].[{objectQualifier}User] drop column ICQ
    end

    if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='RealName')
    begin
        alter table [{databaseOwner}].[{objectQualifier}User] drop column RealName
    end

    if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Occupation')
    begin
        alter table [{databaseOwner}].[{objectQualifier}User] drop column Occupation
    end

    if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Interests')
    begin
        alter table [{databaseOwner}].[{objectQualifier}User] drop column Interests
    end

    if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Weblog')
    begin
        alter table [{databaseOwner}].[{objectQualifier}User] drop column Weblog
    end

    if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='WeblogUrl')
    begin
        alter table [{databaseOwner}].[{objectQualifier}User] drop column WeblogUrl
        alter table [{databaseOwner}].[{objectQualifier}User] drop column WeblogUsername
        alter table [{databaseOwner}].[{objectQualifier}User] drop column WeblogID
    end
END
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsGuest')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [IsGuest] AS (CONVERT([bit],sign([Flags]&(4)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsCaptchaExcluded')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [IsCaptchaExcluded] AS (CONVERT([bit],sign([Flags]&(8)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsDST')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [IsDST] AS (CONVERT([bit],sign([Flags]&(32)),(0)))
end
GO
if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsDirty')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [IsDirty] AS (CONVERT([bit],sign([Flags]&(64)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='UserStyle')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [UserStyle] varchar(510)
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='StyleFlags')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [StyleFlags] [int] NOT NULL constraint [DF_{objectQualifier}User_StyleFlags] default (0)
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsUserStyle')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [IsUserStyle] AS (CONVERT([bit],sign([StyleFlags]&(1)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsGroupStyle')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [IsGroupStyle] AS (CONVERT([bit],sign([StyleFlags]&(2)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='IsRankStyle')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [IsRankStyle] AS (CONVERT([bit],sign([StyleFlags]&(4)),(0)))
end
GO

-- Forum Table
if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='RemoteURL')
    alter table [{databaseOwner}].[{objectQualifier}Forum] add RemoteURL nvarchar(100) null
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='ModeratedPostCount')
    alter table [{databaseOwner}].[{objectQualifier}Forum] add ModeratedPostCount int null
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IsModeratedNewTopicOnly')
    alter table [{databaseOwner}].[{objectQualifier}Forum] add IsModeratedNewTopicOnly	bit not null constraint [DF_{objectQualifier}Forum_IsModeratedNewTopicOnly] default (0)
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='Flags')
    alter table [{databaseOwner}].[{objectQualifier}Forum] add Flags int not null constraint DF_{objectQualifier}Forum_Flags default (0)
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='ThemeURL')
    alter table [{databaseOwner}].[{objectQualifier}Forum] add ThemeURL nvarchar(50) NULL
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='Locked')
begin
    exec('update [{databaseOwner}].[{objectQualifier}Forum] set Flags = Flags | 1 where Locked<>0')
    alter table [{databaseOwner}].[{objectQualifier}Forum] drop column Locked
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='Hidden')
begin
    exec('update [{databaseOwner}].[{objectQualifier}Forum] set Flags = Flags | 2 where Hidden<>0')
    alter table [{databaseOwner}].[{objectQualifier}Forum] drop column Hidden
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IsTest')
begin
    exec('update [{databaseOwner}].[{objectQualifier}Forum] set Flags = Flags | 4 where IsTest<>0')
    alter table [{databaseOwner}].[{objectQualifier}Forum] drop column IsTest
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='Moderated')
begin
    exec('update [{databaseOwner}].[{objectQualifier}Forum] set Flags = Flags | 8 where Moderated<>0')
    alter table [{databaseOwner}].[{objectQualifier}Forum] drop column Moderated
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='ImageURL')
    alter table [{databaseOwner}].[{objectQualifier}Forum] add ImageURL nvarchar(128) NULL
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='Styles')
    alter table [{databaseOwner}].[{objectQualifier}Forum] add Styles nvarchar(255) NULL
GO

if exists (select top 1 1 from sys.columns where object_id = object_id(N'[{databaseOwner}].[{objectQualifier}Forum]') and name='LastUserName' and precision < 255)
    alter table [{databaseOwner}].[{objectQualifier}Forum] alter column [LastUserName]	nvarchar (255) NULL
GO

if not exists (select top 1 1 from sys.columns where object_id = object_id(N'[{databaseOwner}].[{objectQualifier}Forum]') and name='LastUserDisplayName')
    alter table [{databaseOwner}].[{objectQualifier}Forum] add [LastUserDisplayName]	nvarchar (255) NULL

GO

if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IsHidden')
begin
    alter table [{databaseOwner}].[{objectQualifier}Forum] ADD [IsHidden] AS (CONVERT([bit],sign([Flags]&(2)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IsLocked')
begin
    alter table [{databaseOwner}].[{objectQualifier}Forum] ADD [IsLocked] AS (CONVERT([bit],sign([Flags]&(1)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IsNoCount')
begin
    alter table [{databaseOwner}].[{objectQualifier}Forum] ADD [IsNoCount] AS (CONVERT([bit],sign([Flags]&(4)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='IsModerated')
begin
    alter table [{databaseOwner}].[{objectQualifier}Forum] ADD [IsModerated] AS (CONVERT([bit],sign([Flags]&(8)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='UserID')
begin
    alter table [{databaseOwner}].[{objectQualifier}Forum] ADD [UserID]  int null
end
GO

if exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name='Description' and is_nullable=0)
begin
    alter table [{databaseOwner}].[{objectQualifier}Forum] alter column [Description] nvarchar(255) null
end
GO

-- Group Table
if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='Flags')
begin
    alter table [{databaseOwner}].[{objectQualifier}Group] add Flags int not null constraint [DF_{objectQualifier}Group_Flags] default (0)
end
GO

if exists (select top 1 1 from sys.columns where object_id = object_id(N'[{databaseOwner}].[{objectQualifier}Group]') and name='Name' and precision < 255)
begin
if exists (select top 1 1 from sys.indexes where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IX_{objectQualifier}Group')
begin
    alter table [{databaseOwner}].[{objectQualifier}Group] drop constraint IX_{objectQualifier}Group
end
    alter table [{databaseOwner}].[{objectQualifier}Group] alter column [Name] nvarchar(255) NOT NULL
end
GO


if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IsAdmin')
begin
    exec('update [{databaseOwner}].[{objectQualifier}Group] set Flags = Flags | 1 where IsAdmin<>0')
    alter table [{databaseOwner}].[{objectQualifier}Group] drop column IsAdmin
end
GO


if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IsGuest')
begin
    exec('update [{databaseOwner}].[{objectQualifier}Group] set Flags = Flags | 2 where IsGuest<>0')
    alter table [{databaseOwner}].[{objectQualifier}Group] drop column IsGuest
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IsStart')
begin
    exec('update [{databaseOwner}].[{objectQualifier}Group] set Flags = Flags | 4 where IsStart<>0')
    alter table [{databaseOwner}].[{objectQualifier}Group] drop column IsStart
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IsModerator')
begin
    exec('update [{databaseOwner}].[{objectQualifier}Group] set Flags = Flags | 8 where IsModerator<>0')
    alter table [{databaseOwner}].[{objectQualifier}Group] drop column IsModerator
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id(N'[{databaseOwner}].[{objectQualifier}Group]') and name=N'PMLimit')
begin

        alter table [{databaseOwner}].[{objectQualifier}Group] add PMLimit int not null	constraint [DF_{objectQualifier}Group_PMLimit] default (0)
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id(N'[{databaseOwner}].[{objectQualifier}Group]') and name=N'PMLimit' and is_nullable=1)
begin
        grant update on [{databaseOwner}].[{objectQualifier}Group] to public
        exec('update [{databaseOwner}].[{objectQualifier}Group] set PMLimit = 30 WHERE PMLimit IS NULL')
        alter table [{databaseOwner}].[{objectQualifier}Group] alter column [PMLimit] integer NOT NULL
        revoke update on [{databaseOwner}].[{objectQualifier}Group] from public
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='Style')
begin
    alter table [{databaseOwner}].[{objectQualifier}Group] add Style nvarchar(255) null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='SortOrder')
begin
    alter table [{databaseOwner}].[{objectQualifier}Group] add SortOrder smallint not null constraint [DF_{objectQualifier}Group_SortOrder] default (0)
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='Description')
begin
    alter table [{databaseOwner}].[{objectQualifier}Group] add Description nvarchar(128) null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='UsrSigChars')
begin
    alter table [{databaseOwner}].[{objectQualifier}Group] add UsrSigChars int not null constraint [DF_{objectQualifier}Group_UsrSigChars] default (0)
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='UsrSigChars')
begin
grant update on [{databaseOwner}].[{objectQualifier}Group] to public
        exec('update [{databaseOwner}].[{objectQualifier}Group] set UsrSigChars = 128 WHERE UsrSigChars = 0 AND Name != ''Guest'' ')
        revoke update on [{databaseOwner}].[{objectQualifier}Group] from public
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='UsrSigBBCodes')
begin
    alter table [{databaseOwner}].[{objectQualifier}Group] add UsrSigBBCodes nvarchar(255) null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='UsrSigHTMLTags')
begin
    alter table [{databaseOwner}].[{objectQualifier}Group] add UsrSigHTMLTags nvarchar(255) null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='UsrAlbums')
begin
    alter table [{databaseOwner}].[{objectQualifier}Group] add UsrAlbums int not null constraint [DF_{objectQualifier}Group_UsrAlbums] default (0)
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='UsrAlbumImages')
begin
    alter table [{databaseOwner}].[{objectQualifier}Group] add UsrAlbumImages int not null constraint [DF_{objectQualifier}Group_UsrAlbumImages] default (0)
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IsHidden')
begin
alter table [{databaseOwner}].[{objectQualifier}Group] ADD [IsHidden] AS (CONVERT([bit],sign([Flags]&(16)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Group]') and name='IsUserGroup')
begin
alter table [{databaseOwner}].[{objectQualifier}Group] ADD [IsUserGroup] AS (CONVERT([bit],sign([Flags]&(32)),(0)))
end
GO

-- AccessMask Table
if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='Flags')
begin
    alter table [{databaseOwner}].[{objectQualifier}AccessMask] add Flags int not null constraint [DF_{objectQualifier}AccessMask_Flags] default (0)
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='ReadAccess')
begin
    exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 1 where ReadAccess<>0')
    alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column ReadAccess
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='PostAccess')
begin
    exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 2 where PostAccess<>0')
    alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column PostAccess
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='ReplyAccess')
begin
    exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 4 where ReplyAccess<>0')
    alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column ReplyAccess
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='PriorityAccess')
begin
    exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 8 where PriorityAccess<>0')
    alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column PriorityAccess
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='PollAccess')
begin
    exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 16 where PollAccess<>0')
    alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column PollAccess
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='VoteAccess')
begin
    exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 32 where VoteAccess<>0')
    alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column VoteAccess
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='ModeratorAccess')
begin
    exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 64 where ModeratorAccess<>0')
    alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column ModeratorAccess
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='EditAccess')
begin
    exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 128 where EditAccess<>0')
    alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column EditAccess
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='DeleteAccess')
begin
    exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 256 where DeleteAccess<>0')
    alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column DeleteAccess
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='UploadAccess')
begin
    exec('update [{databaseOwner}].[{objectQualifier}AccessMask] set Flags = Flags | 512 where UploadAccess<>0')
    alter table [{databaseOwner}].[{objectQualifier}AccessMask] drop column UploadAccess
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}AccessMask]') and name='SortOrder')
begin
    alter table [{databaseOwner}].[{objectQualifier}AccessMask] add SortOrder smallint not null default (0)
end
GO

-- NntpForum Table

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}NntpForum]') and name='Active')
begin
    alter table [{databaseOwner}].[{objectQualifier}NntpForum] add Active bit null
    exec('update [{databaseOwner}].[{objectQualifier}NntpForum] set Active=1 where Active is null')
    alter table [{databaseOwner}].[{objectQualifier}NntpForum] alter column Active bit not null
end
GO

if not exists (select top 1 1 from sys.columns where object_id = object_id(N'[{databaseOwner}].[{objectQualifier}NntpForum]') and name='DateCutOff')
    alter table [{databaseOwner}].[{objectQualifier}NntpForum] ADD	DateCutOff datetime NULL
GO

-- NntpTopic Table

if exists (select top 1 1 from sys.columns where object_id = object_id(N'[{databaseOwner}].[{objectQualifier}nntptopic]') and name='Thread' and precision < 64)
    alter table [{databaseOwner}].[{objectQualifier}nntptopic] alter column [Thread]	nvarchar (64) NULL
GO

-- ReplaceWords Table
if exists (select * from sys.columns where object_id =  object_id(N'[{databaseOwner}].[{objectQualifier}Replace_Words]') and name='badword' and precision < 255)
    alter table [{databaseOwner}].[{objectQualifier}Replace_Words] alter column badword nvarchar(255) NULL
GO

if exists (select * from sys.columns where object_id =  object_id(N'[{databaseOwner}].[{objectQualifier}Replace_Words]') and name='goodword' and precision < 255)
    alter table [{databaseOwner}].[{objectQualifier}Replace_Words] alter column goodword nvarchar(255) NULL
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id(N'[{databaseOwner}].[{objectQualifier}Replace_Words]') and name='BoardID')
begin
    alter table [{databaseOwner}].[{objectQualifier}Replace_Words] add BoardID int not null constraint [DF_{objectQualifier}Replace_Words_BoardID] default (1)
end
GO

-- BBCode Table
if not exists (select top 1 1 from sys.columns where object_id=object_id(N'[{databaseOwner}].[{objectQualifier}BBCode]') and name='UseModule')
begin
    alter table [{databaseOwner}].[{objectQualifier}BBCode] add UseModule bit null
    alter table [{databaseOwner}].[{objectQualifier}BBCode] add ModuleClass nvarchar(255) null
end
GO

-- BBCode Table
if not exists (select top 1 1 from sys.columns where object_id=object_id(N'[{databaseOwner}].[{objectQualifier}BBCode]') and name='UseToolbar')
begin
    alter table [{databaseOwner}].[{objectQualifier}BBCode] add UseToolbar bit null
end
GO

-- Registry Table
if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Registry]') and name='BoardID')
    alter table [{databaseOwner}].[{objectQualifier}Registry] add BoardID int
GO

if exists (select top 1 1 from sys.columns where object_id=object_id(N'[{databaseOwner}].[{objectQualifier}Registry]') and name=N'Value' and system_type_id<>99)
    alter table [{databaseOwner}].[{objectQualifier}Registry] alter column Value nvarchar(max) null
GO

-- PMessage Table
if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}PMessage]') and name='Flags')
begin
    alter table [{databaseOwner}].[{objectQualifier}PMessage] add Flags int not null constraint [DF_{objectQualifier}Message_Flags] default (23)
end
GO

-- Message Table
if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='Flags')
begin
    alter table [{databaseOwner}].[{objectQualifier}Topic] add Flags int not null constraint [DF_{objectQualifier}Topic_Flags] default (0)
    update [{databaseOwner}].[{objectQualifier}Message] set Flags = Flags & 7
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='Approved')
begin
    exec('update [{databaseOwner}].[{objectQualifier}Message] set Flags = Flags | 16 where Approved<>0')
    alter table [{databaseOwner}].[{objectQualifier}Message] drop column Approved
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='BlogPostID')
begin
    alter table [{databaseOwner}].[{objectQualifier}Message] add BlogPostID nvarchar(50)
end
GO

if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='IsDeleted')
begin
    alter table [{databaseOwner}].[{objectQualifier}Message] ADD [IsDeleted] AS (CONVERT([bit],sign([Flags]&(8)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='UserDisplayName')
begin
    alter table [{databaseOwner}].[{objectQualifier}Message] add UserDisplayName nvarchar(255)
end
GO

if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='IsApproved')
begin
    alter table [{databaseOwner}].[{objectQualifier}Message] ADD [IsApproved] AS (CONVERT([bit],sign([Flags]&(16)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='EditReason')
    alter table [{databaseOwner}].[{objectQualifier}Message] add EditReason nvarchar (100) NULL
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='IsModeratorChanged')
    alter table [{databaseOwner}].[{objectQualifier}Message] add 	IsModeratorChanged      bit NOT NULL constraint [DF_{objectQualifier}Message_IsModeratorChanged] default (0)
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='DeleteReason')
    alter table [{databaseOwner}].[{objectQualifier}Message] add DeleteReason            nvarchar (100)  NULL
GO

-- an attempt to migrate the legacy report abuse and report spam features flags
 if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='Flags')
begin
    grant update on [{databaseOwner}].[{objectQualifier}Message] to public
    exec('update [{databaseOwner}].[{objectQualifier}Message] SET [{databaseOwner}].[{objectQualifier}Message].Flags =  ([{databaseOwner}].[{objectQualifier}Message].Flags  ^ POWER(2, 8) | POWER(2, 7))
        WHERE (([{databaseOwner}].[{objectQualifier}Message].Flags & 256)=256)')
    -- exec('update [{databaseOwner}].[{objectQualifier}Message] SET [{databaseOwner}].[{objectQualifier}Message].Flags =  ([{databaseOwner}].[{objectQualifier}Message].Flags  ^ POWER(2, 9) | POWER(2, 7)
    ---	WHERE (([{databaseOwner}].[{objectQualifier}Message].Flags & 512)=512)')
    revoke update on [{databaseOwner}].[{objectQualifier}Message] from public
end


if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='EditedBy')
    alter table [{databaseOwner}].[{objectQualifier}Message] add [EditedBy]   int  NULL
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='ExternalMessageId')
    alter table [{databaseOwner}].[{objectQualifier}Message] add [ExternalMessageId]   nvarchar(255) NULL
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='ReferenceMessageId')
    alter table [{databaseOwner}].[{objectQualifier}Message] add [ReferenceMessageId]   nvarchar(255) NULL
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='ExternalMessageId' and precision < 255)
begin
    alter table [{databaseOwner}].[{objectQualifier}Message] alter column [ExternalMessageId] nvarchar (255) NULL
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='IP' and precision < 39)
begin
    alter table [{databaseOwner}].[{objectQualifier}Message] alter column [IP] varchar(39) not null
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Message]') and name='UserName' and precision < 255)
begin
    alter table [{databaseOwner}].[{objectQualifier}Message] alter column [UserName] nvarchar (255) NULL
end
GO

-- MessageHistory Table

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}MessageHistory]') and name='MessageHistoryID')
begin
    alter table [{databaseOwner}].[{objectQualifier}MessageHistory] drop column [MessageHistoryID]
end
GO
-- the dependency should be dropped first
if exists (select top 1 1 from  sys.objects where name='IX_{objectQualifier}MessageHistory' and parent_object_id =object_id('[{databaseOwner}].[{objectQualifier}MessageHistory]'))
    alter table [{databaseOwner}].[{objectQualifier}MessageHistory] drop constraint [IX_{objectQualifier}MessageHistory]
go
if exists (select top 1 1 from sys.columns where object_id=object_id(N'[{databaseOwner}].[{objectQualifier}MessageHistory]') and name=N'Edited' and is_nullable=1)
begin
        grant update on [{databaseOwner}].[{objectQualifier}MessageHistory] to public
        -- exec('[{databaseOwner}].[{objectQualifier}drop_defaultconstraint_oncolumn] {objectQualifier}MessageHistory, Edited')
        exec('update [{databaseOwner}].[{objectQualifier}MessageHistory] set Edited = GETDATE() WHERE Edited IS NULL')
        alter table [{databaseOwner}].[{objectQualifier}MessageHistory] alter column [Edited] datetime NOT NULL
        revoke update on [{databaseOwner}].[{objectQualifier}MessageHistory] from public
end
GO

-- Topic Table
if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='IsLocked')
begin
    grant update on [{databaseOwner}].[{objectQualifier}Topic] to public
    exec('update [{databaseOwner}].[{objectQualifier}Topic] set Flags = Flags | 1 where IsLocked<>0')
    revoke update on [{databaseOwner}].[{objectQualifier}Topic] from public
    alter table [{databaseOwner}].[{objectQualifier}Topic] drop column IsLocked
end
GO

if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='IsDeleted')
begin
    alter table [{databaseOwner}].[{objectQualifier}Topic] ADD [IsDeleted] AS (CONVERT([bit],sign([Flags]&(8)),(0)))
end
GO

if exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='UserName')
begin
    alter table [{databaseOwner}].[{objectQualifier}Topic] alter column [UserName]	nvarchar (255) NULL
end
GO

if exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='LastUserName')
begin
    alter table [{databaseOwner}].[{objectQualifier}Topic] alter column [LastUserName]	nvarchar (255) NULL
end
GO

if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='UserDisplayName')
begin
    alter table [{databaseOwner}].[{objectQualifier}Topic] add [UserDisplayName]	nvarchar (255) NULL

end
GO

if not exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='LastUserDisplayName')
begin
    alter table [{databaseOwner}].[{objectQualifier}Topic] add [LastUserDisplayName]		nvarchar (255) NULL
end
GO


if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='LastMessageFlags')
begin
    alter table [{databaseOwner}].[{objectQualifier}Topic] add [LastMessageFlags] int null
    grant update on [{databaseOwner}].[{objectQualifier}Topic] to public
    -- vzrus : we don't migrate flags to not slow down update and possible timeouts. Users can run maintenance scripts? Else use cursors.
    exec('update [{databaseOwner}].[{objectQualifier}Topic] set LastMessageFlags = 22 WHERE LastMessageFlags IS NULL')
    revoke update on [{databaseOwner}].[{objectQualifier}Topic] from public
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='Description')
begin
    alter table [{databaseOwner}].[{objectQualifier}Topic] add [Description] nvarchar(255) null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='LinkDate')
begin
    alter table [{databaseOwner}].[{objectQualifier}Topic] add [LinkDate] datetime null
end
GO

-- Rank Table
if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='Flags')
begin
    alter table [{databaseOwner}].[{objectQualifier}Rank] add Flags int not null constraint [DF_{objectQualifier}Rank_Flags] default (0)
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id(N'[{databaseOwner}].[{objectQualifier}Rank]') and name=N'PMLimit')
begin
    alter table [{databaseOwner}].[{objectQualifier}Rank] add PMLimit int null
    grant update on [{databaseOwner}].[{objectQualifier}Rank] to public
    exec('update [{databaseOwner}].[{objectQualifier}Rank] set PMLimit = 0 WHERE PMLimit IS NULL')
    alter table [{databaseOwner}].[{objectQualifier}Rank] alter column [PMLimit] integer NOT NULL
    revoke update on [{databaseOwner}].[{objectQualifier}Rank] from public
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='Style')
begin
    alter table [{databaseOwner}].[{objectQualifier}Rank] add Style nvarchar(255) null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='SortOrder')
begin
    alter table [{databaseOwner}].[{objectQualifier}Rank] add SortOrder smallint not null constraint [DF_{objectQualifier}Rank_SortOrder] default (0)
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='Description')
begin
    alter table [{databaseOwner}].[{objectQualifier}Rank] add Description nvarchar(128) null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='UsrSigChars')
begin
    alter table [{databaseOwner}].[{objectQualifier}Rank] add UsrSigChars int not null constraint [DF_{objectQualifier}Rank_UsrSigChars] default (0)
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='UsrSigChars')
begin
grant update on [{databaseOwner}].[{objectQualifier}Rank] to public
        exec('update [{databaseOwner}].[{objectQualifier}Rank] set UsrSigChars = 128 WHERE UsrSigChars = 0 AND Name != ''Guest'' ')
        revoke update on [{databaseOwner}].[{objectQualifier}Rank] from public
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='UsrSigBBCodes')
begin
    alter table [{databaseOwner}].[{objectQualifier}Rank] add UsrSigBBCodes nvarchar(255) null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='UsrSigHTMLTags')
begin
    alter table [{databaseOwner}].[{objectQualifier}Rank] add UsrSigHTMLTags nvarchar(255) null
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='UsrAlbums')
begin
    alter table [{databaseOwner}].[{objectQualifier}Rank] add UsrAlbums int not null constraint [DF_{objectQualifier}Rank_UsrAlbums] default (0)
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='UsrAlbumImages')
begin
    alter table [{databaseOwner}].[{objectQualifier}Rank] add UsrAlbumImages int not null constraint [DF_{objectQualifier}Rank_UsrAlbumImages] default (0)
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='IsStart')
begin
    grant update on [{databaseOwner}].[{objectQualifier}Rank] to public
    exec('update [{databaseOwner}].[{objectQualifier}Rank] set Flags = Flags | 1 where IsStart<>0')
    revoke update on [{databaseOwner}].[{objectQualifier}Rank] from public
    alter table [{databaseOwner}].[{objectQualifier}Rank] drop column IsStart
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='IsLadder')
begin
    grant update on [{databaseOwner}].[{objectQualifier}Rank] to public
    exec('update [{databaseOwner}].[{objectQualifier}Rank] set Flags = Flags | 2 where IsLadder<>0')
    revoke update on [{databaseOwner}].[{objectQualifier}Rank] from public
    alter table [{databaseOwner}].[{objectQualifier}Rank] drop column IsLadder
end
GO

--vzrus: eof migrate to independent multiple polls


-- Poll Table
if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name='Closes')
begin
    alter table [{databaseOwner}].[{objectQualifier}Poll] add Closes datetime null
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'Question' AND precision < 256 )
begin
    alter table [{databaseOwner}].[{objectQualifier}Poll] alter column Question nvarchar(256) NOT NULL
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'UserID')
begin
    alter table [{databaseOwner}].[{objectQualifier}Poll] add [UserID] int NOT NULL constraint [DF_{objectQualifier}Poll_UserID] default (1)
end
GO

-- should drop it else error
if exists(select top 1 1 from sys.objects where name='FK_{objectQualifier}Topic_{objectQualifier}Poll' and parent_object_id =object_id('[{databaseOwner}].[{objectQualifier}Topic]') and type in (N'F'))
    alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_{objectQualifier}Topic_{objectQualifier}Poll]
go

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'Flags')
begin
    alter table [{databaseOwner}].[{objectQualifier}Poll] add [Flags] int NULL
end
GO

-- TODO: change userid to not null

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'ObjectPath')
begin
    alter table [{databaseOwner}].[{objectQualifier}Poll] add [ObjectPath] nvarchar(255) NULL
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'IsClosedBound')
begin
    alter table [{databaseOwner}].[{objectQualifier}Poll] add [IsClosedBound] AS (CONVERT([bit],sign([Flags]&(4)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'AllowMultipleChoices')
begin
    alter table [{databaseOwner}].[{objectQualifier}Poll] add [AllowMultipleChoices] AS (CONVERT([bit],sign([Flags]&(8)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'ShowVoters')
begin
    alter table [{databaseOwner}].[{objectQualifier}Poll] add [ShowVoters] AS (CONVERT([bit],sign([Flags]&(16)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'AllowSkipVote')
begin
    alter table [{databaseOwner}].[{objectQualifier}Poll] add [AllowSkipVote] AS (CONVERT([bit],sign([Flags]&(32)),(0)))
end
GO

-- ActiveAccess Table
if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}ActiveAccess]') and name=N'LastActive')
begin
    alter table [{databaseOwner}].[{objectQualifier}ActiveAccess] add [LastActive] datetime NULL
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}ActiveAccess]') and name=N'IsGuestX')
begin
    delete from [{databaseOwner}].[{objectQualifier}ActiveAccess]
    alter table [{databaseOwner}].[{objectQualifier}ActiveAccess] add [IsGuestX] bit NOT NULL
end
GO
-- drop the old contrained just in case
if exists (select top 1 1 from  sys.indexes where object_id=object_id('[{databaseOwner}].[{objectQualifier}ActiveAccess]') and name='IX_{objectQualifier}ActiveAccess')
    alter table [{databaseOwner}].[{objectQualifier}ActiveAccess] drop constraint IX_{objectQualifier}ActiveAccess
go

if exists(select top 1 1 from sys.columns where object_id =  object_id(N'[{databaseOwner}].[{objectQualifier}ActiveAccess]') and name=N'ForumID' and is_nullable=1)
    alter table [{databaseOwner}].[{objectQualifier}ActiveAccess] alter column ForumID int not null
GO

-- Choice Table
-- this is a dummy it doesn't work
if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Choice]') and name= N'Choice' AND precision < 255 )
begin
    alter table [{databaseOwner}].[{objectQualifier}Choice] alter column Choice varchar(255) NOT NULL
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Choice]') and name=N'ObjectPath')
begin
    alter table [{databaseOwner}].[{objectQualifier}Choice] add [ObjectPath] nvarchar(255) NULL
end
GO

-- EventLog Table
if not exists(select top 1 1 from sys.columns where object_id =  object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]') and name=N'Type')
begin
    alter table [{databaseOwner}].[{objectQualifier}EventLog] add Type int not null constraint [DF_{objectQualifier}EventLog_Type] default (0)
    exec('update [{databaseOwner}].[{objectQualifier}EventLog] set Type = 0')
end
GO

if not exists(select top 1 1 from sys.columns where object_id =  object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]') and name=N'UserName')
begin
    alter table [{databaseOwner}].[{objectQualifier}EventLog] add UserName nvarchar(100) null
end
GO

if exists(select top 1 1 from sys.columns where object_id =  object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]') and name=N'UserName' and is_nullable=0)
    alter table [{databaseOwner}].[{objectQualifier}EventLog] alter column UserName nvarchar(100) null
GO

if exists(select top 1 1 from sys.columns where object_id =  object_id(N'[{databaseOwner}].[{objectQualifier}EventLog]') and name=N'UserID' and is_nullable=0)
    alter table [{databaseOwner}].[{objectQualifier}EventLog] alter column UserID int null
GO

-- Category Table
IF NOT exists (select top 1 1 from sys.columns where object_id =  Object_id(N'[{databaseOwner}].[{objectQualifier}Category]') AND name = N'CategoryImage')
BEGIN
    ALTER TABLE [{databaseOwner}].[{objectQualifier}Category] ADD [CategoryImage] [nvarchar](255) NULL
END
GO


-- MessageReportedAudit Table
IF NOT exists (select top 1 1 from sys.columns where object_id =  Object_id(N'[{databaseOwner}].[{objectQualifier}MessageReportedAudit]') AND name = N'ReportedNumber')
BEGIN
    ALTER TABLE [{databaseOwner}].[{objectQualifier}MessageReportedAudit] ADD [ReportedNumber] int NOT NULL default 1
END
GO

IF NOT exists (select top 1 1 from sys.columns where object_id =  Object_id(N'[{databaseOwner}].[{objectQualifier}MessageReportedAudit]') AND name = N'ReportText')
BEGIN
    ALTER TABLE [{databaseOwner}].[{objectQualifier}MessageReportedAudit] ADD [ReportText] nvarchar(4000)  NULL
END
GO

if exists (select top 1 1 from sys.columns where object_id=object_id(N'[{databaseOwner}].[{objectQualifier}MessageReportedAudit]') and name=N'MessageID' and is_nullable=1)
begin
        alter table [{databaseOwner}].[{objectQualifier}MessageReportedAudit] alter column [MessageID] integer NOT NULL
end
GO


-- BannedIP Table

IF NOT exists (select top 1 1 from sys.columns where object_id =  Object_id(N'[{databaseOwner}].[{objectQualifier}BannedIP]') AND name = N'Reason')
BEGIN
    ALTER TABLE [{databaseOwner}].[{objectQualifier}BannedIP] ADD [Reason] nvarchar(128)  NULL
END
GO

IF NOT exists (select top 1 1 from sys.columns where object_id =  Object_id(N'[{databaseOwner}].[{objectQualifier}BannedIP]') AND name = N'UserID')
BEGIN
    ALTER TABLE [{databaseOwner}].[{objectQualifier}BannedIP] ADD [UserID] int  null
END
GO

IF NOT exists (select top 1 1 from sys.columns where object_id =  Object_id(N'[{databaseOwner}].[{objectQualifier}BannedIP]') AND name = N'Mask' AND precision < 56)
BEGIN
    ALTER TABLE [{databaseOwner}].[{objectQualifier}BannedIP] alter column [Mask] varchar(57) not  null
END
GO

-- PollVote Table

if exists(select top 1 1 from sys.columns where object_id =  object_id(N'[{databaseOwner}].[{objectQualifier}PollVote]') and name=N'RemoteIP' and precision<39)
    -- vzrus: should drop the index to change the field
    if exists(select * from sys.indexes where object_id=object_id('[{databaseOwner}].[{objectQualifier}PollVote]') and name='IX_{objectQualifier}PollVote_RemoteIP')
    begin
    begin
    drop index [IX_{objectQualifier}PollVote_RemoteIP] ON [{databaseOwner}].[{objectQualifier}PollVote]
    end
    alter table [{databaseOwner}].[{objectQualifier}PollVote] alter column [RemoteIP] varchar(39) null
    end
GO

IF NOT exists (select top 1 1 from sys.columns where object_id =  Object_id(N'[{databaseOwner}].[{objectQualifier}PollVote]') AND name = N'ChoiceID')
BEGIN
    ALTER TABLE [{databaseOwner}].[{objectQualifier}PollVote] ADD [ChoiceID] int  null
END
GO


-- MessageHistory Table

if exists(select top 1 1 from sys.columns where object_id =  object_id(N'[{databaseOwner}].[{objectQualifier}MessageHistory]') and name=N'IP' and precision<39)
    alter table [{databaseOwner}].[{objectQualifier}MessageHistory] alter column [IP] varchar(39) not null
GO

-- NntpServer Table

if exists(select top 1 1 from sys.columns where object_id =  object_id(N'[{databaseOwner}].[{objectQualifier}NntpServer]') and name=N'UserName' and precision<255)
    alter table [{databaseOwner}].[{objectQualifier}NntpServer] alter column [UserName] nvarchar(255) null
GO

if exists(select top 1 1 from sys.columns where object_id =  object_id(N'[{databaseOwner}].[{objectQualifier}User]') and name=N'Email' and precision<255)
    alter table [{databaseOwner}].[{objectQualifier}User] alter column [Email] nvarchar(255) null
GO

if exists(select top 1 1 from sys.columns where object_id =  object_id(N'[{databaseOwner}].[{objectQualifier}CheckEmail]') and name=N'Email' and precision<255)
    alter table [{databaseOwner}].[{objectQualifier}CheckEmail] alter column [Email] nvarchar(255) null
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='Status')
begin
    alter table [{databaseOwner}].[{objectQualifier}Topic] add [Status] nvarchar(255) null
end
GO


if exists(select top 1 1 from sys.objects where name='PK_{objectQualifier}ForumReadTracking')
    alter table [{databaseOwner}].[{objectQualifier}ForumReadTracking] drop constraint [PK_{objectQualifier}ForumReadTracking]
go

if exists(select top 1 1 from sys.objects where name='PK_{objectQualifier}TopicReadTracking')
    alter table [{databaseOwner}].[{objectQualifier}TopicReadTracking] drop constraint [PK_{objectQualifier}TopicReadTracking]
go

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}ForumReadTracking]') and name='TrackingID')
begin
    alter table [{databaseOwner}].[{objectQualifier}ForumReadTracking] drop column TrackingID
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}TopicReadTracking]') and name='TrackingID')
begin
    alter table [{databaseOwner}].[{objectQualifier}TopicReadTracking] drop column TrackingID
end
GO

-- Add 8-letter Language Code column
if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Culture' and precision=5)
begin
    alter table [{databaseOwner}].[{objectQualifier}User] alter column [Culture] varchar(10) NULL
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and name='Styles')
    alter table [{databaseOwner}].[{objectQualifier}Topic] add Styles nvarchar(255) NULL
GO

-- display names upgrade routine can run really for ages on large forums
IF  exists(select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_initdisplayname]'))
DROP procedure [{databaseOwner}].[{objectQualifier}forum_initdisplayname]
GO

create procedure [{databaseOwner}].[{objectQualifier}forum_initdisplayname] as

begin
    declare @tmpUserName nvarchar(255)
    declare @tmpUserDisplayName nvarchar(255)
    declare @tmpLastUserName nvarchar(255)
    declare @tmpLastUserDisplayName nvarchar(255)
    declare @tmp int
    declare @tmpUserID int
    declare @tmpLastUserID int

     update d
      set    d.LastUserDisplayName = ISNULL((select top 1 f.LastUserDisplayName FROM [{databaseOwner}].[{objectQualifier}Forum] f
          join [{databaseOwner}].[{objectQualifier}User] u on u.UserID = f.UserID where u.UserID = d.UserID),
           (select top 1 f.LastUserName FROM [{databaseOwner}].[{objectQualifier}Forum] f
          join [{databaseOwner}].[{objectQualifier}User] u on u.UserID = f.UserID where u.UserID = d.UserID ))
       from  [{databaseOwner}].[{objectQualifier}Forum] d where d.LastUserDisplayName IS NULL OR d.LastUserDisplayName = d.LastUserName;

      update d
       set    d.UserDisplayName = ISNULL((select top 1 m.UserDisplayName FROM [{databaseOwner}].[{objectQualifier}Message] m
          join [{databaseOwner}].[{objectQualifier}User] u on u.UserID = m.UserID where u.UserID = d.UserID),
           (select top 1 m.UserName FROM [{databaseOwner}].[{objectQualifier}Message] m
          join [{databaseOwner}].[{objectQualifier}User] u on u.UserID = m.UserID where u.UserID = d.UserID ))
       from  [{databaseOwner}].[{objectQualifier}Message] d where d.UserDisplayName IS NULL OR d.UserDisplayName = d.UserName;

      update d
       set    d.UserDisplayName = ISNULL((select top 1 t.UserDisplayName FROM [{databaseOwner}].[{objectQualifier}Topic] t
          join [{databaseOwner}].[{objectQualifier}User] u on u.UserID = t.UserID where u.UserID = d.UserID),
           (select top 1 t.UserName FROM [{databaseOwner}].[{objectQualifier}Topic] t
          join [{databaseOwner}].[{objectQualifier}User] u on u.UserID = t.UserID where u.UserID = d.UserID ))
       from  [{databaseOwner}].[{objectQualifier}Message] d where d.UserDisplayName IS NULL OR d.UserDisplayName = d.UserName;
end
GO

if exists (select top 1 1 from [{databaseOwner}].[{objectQualifier}Message] where UserDisplayName IS NULL)
exec('[{databaseOwner}].[{objectQualifier}forum_initdisplayname]')
GO

-- add ReplyTo Column to PMessage Table if not exists
if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}PMessage]') and name='ReplyTo')
    alter table [{databaseOwner}].[{objectQualifier}PMessage] add ReplyTo int NULL
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}UserPMessage]') and name='IsReply')
    alter table [{databaseOwner}].[{objectQualifier}UserPMessage] ADD [IsReply] [bit] NOT NULL  default (0)
GO

-- a deleted user was not previously deleted from here - clean-up possible needs a prefetch into temp table for perfomance
exec('delete from [{databaseOwner}].[{objectQualifier}UserMedal] where [UserID] NOT IN (select [UserID] from [{databaseOwner}].[{objectQualifier}User])')
GO

-- update default points from 0 to 1
if exists (select top 1 1 from sys.columns where object_id =  object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Points')
begin
    update  [{databaseOwner}].[{objectQualifier}User] SET [Points]=1 WHERE [Points] = 0
end
GO

-- delete old stuff from registry
grant delete on [{databaseOwner}].[{objectQualifier}Registry] to public
grant update on [{databaseOwner}].[{objectQualifier}Registry] to public

-- fix the problem when a user couldn't be registered
exec('delete from [{databaseOwner}].[{objectQualifier}Registry] where Name LIKE ''timezone'' and BoardID IS NOT NULL')

if not exists (select count(name) from [{databaseOwner}].[{objectQualifier}Registry] where [Name] LIKE 'timezone' and BoardID IS NULL)
exec('insert into [{databaseOwner}].[{objectQualifier}Registry] (Name,Value) values (''timezone'',0)')
exec('update [{databaseOwner}].[{objectQualifier}Group] set Style = NULL where Style is not null and len(Style) <=2')
exec('update [{databaseOwner}].[{objectQualifier}Rank] set Style = NULL where Style is not null and len(Style) <=2')

revoke delete on [{databaseOwner}].[{objectQualifier}Registry] from public
revoke update on [{databaseOwner}].[{objectQualifier}Registry] from public
GO

-- delete any old medals without valid groups.
exec('DELETE FROM [{databaseOwner}].[{objectQualifier}GroupMedal] WHERE GroupID NOT IN (SELECT GroupID FROM [{databaseOwner}].[{objectQualifier}Group])')
GO

if exists(select top 1 1 from sys.columns where object_id = object_id(N'[{databaseOwner}].[{objectQualifier}Attachment]') and name=N'ContentType' and precision < 255)
    alter table [{databaseOwner}].[{objectQualifier}Attachment] alter column ContentType nvarchar(max) null
GO

if exists(select top 1 1 from sys.columns where object_id = object_id(N'[{databaseOwner}].[{objectQualifier}Attachment]') and name=N'FileID')
    alter table [{databaseOwner}].[{objectQualifier}Attachment] drop column FileID
GO

if not exists(select top 1 1 from sys.columns where object_id = object_id(N'[{databaseOwner}].[{objectQualifier}Attachment]') and name=N'UserID')
    begin
        alter table [{databaseOwner}].[{objectQualifier}Attachment] add UserID int not null default (0)

        exec('
        declare @MessageID int
        declare @UserID int

        declare curMessages cursor for
            select
                a.MessageID,
                m.UserID
            from
                [{databaseOwner}].[{objectQualifier}Attachment] a
                INNER JOIN [{databaseOwner}].[{objectQualifier}Message] m ON m.MessageID = a.MessageID

            where
                a.UserID = 0

        open curMessages

        -- cycle through messages
        fetch next from curMessages into @MessageID, @UserID
        while @@FETCH_STATUS = 0
        begin
            update [{databaseOwner}].[{objectQualifier}Attachment] SET UserID = @UserID where MessageID = @MessageID and UserID = 0

            fetch next from curMessages into @MessageID, @UserID
        end
        close curMessages
        deallocate curMessages')
    end
go

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='SuspendedReason')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add SuspendedReason nvarchar(max) NULL
    alter table [{databaseOwner}].[{objectQualifier}User] add SuspendedBy     int not null default (0)
end
GO

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}Message]') and name = 'Message' and system_type_id = 99
   and not exists(select * from sys.sysfulltextcatalogs where name = N'YafSearch'))
begin
   alter table [{databaseOwner}].[{objectQualifier}Message] alter column [Message] nvarchar(max)
end
go

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}MessageHistory]') and name = 'Message' and system_type_id = 99)
begin
    alter table [{databaseOwner}].[{objectQualifier}MessageHistory] alter column [Message] nvarchar(max)
end
go

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}MessageReported]') and name = 'Message' and system_type_id = 99)
begin
    alter table [{databaseOwner}].[{objectQualifier}MessageReported] alter column [Message] nvarchar(max)
end
go

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}PMessage]') and name = 'Body' and system_type_id = 99)
begin
    alter table [{databaseOwner}].[{objectQualifier}PMessage] alter column [Body] nvarchar(max)
end
go

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}User]') and name = 'Signature' and system_type_id = 99)
begin
    alter table [{databaseOwner}].[{objectQualifier}User] alter column [Signature] nvarchar(max)
end
go

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}User]') and name = 'SuspendedReason' and system_type_id = 99)
begin
    alter table [{databaseOwner}].[{objectQualifier}User] alter column [SuspendedReason] nvarchar(max)
end
go

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}Registry]') and name = 'Value' and system_type_id = 99)
begin
    alter table [{databaseOwner}].[{objectQualifier}Registry] alter column [Value] nvarchar(max)
end
go

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}Eventlog]') and name = 'Description' and system_type_id = 99)
begin
    alter table [{databaseOwner}].[{objectQualifier}Eventlog] alter column [Description] nvarchar(max)
end
go

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}BBCode]') and name = 'DisplayJS' and system_type_id = 99)
begin
    alter table [{databaseOwner}].[{objectQualifier}BBCode] alter column [DisplayJS] nvarchar(max)
end
go

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}BBCode]') and name = 'EditJS' and system_type_id = 99)
begin
    alter table [{databaseOwner}].[{objectQualifier}BBCode] alter column [EditJS] nvarchar(max)
end
go

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}BBCode]') and name = 'DisplayCSS' and system_type_id = 99)
begin
    alter table [{databaseOwner}].[{objectQualifier}BBCode] alter column [DisplayCSS] nvarchar(max)
end
go

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}BBCode]') and name = 'SearchRegex' and system_type_id = 99)
begin
    alter table [{databaseOwner}].[{objectQualifier}BBCode] alter column [SearchRegex] nvarchar(max)
end
go

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}BBCode]') and name = 'ReplaceRegex' and system_type_id = 99)
begin
    alter table [{databaseOwner}].[{objectQualifier}BBCode] alter column [ReplaceRegex] nvarchar(max)
end
go

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}Medal]') and name = 'Description' and system_type_id = 99)
begin
    alter table [{databaseOwner}].[{objectQualifier}Medal] alter column [Description] nvarchar(max)
end
go

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}User]') and name='TimeZone' and  system_type_id = 56)
begin
alter table [{databaseOwner}].[{objectQualifier}User] alter column TimeZone nvarchar(max) NULL
end
GO

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}ShoutboxMessage]'))
begin
    drop table [{databaseOwner}].[{objectQualifier}ShoutboxMessage]
end
go

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}TopicStatus]'))
begin
    drop table [{databaseOwner}].[{objectQualifier}TopicStatus]
end
go

if not exists (select top 1 1 from sys.columns where object_id = object_id(N'[{databaseOwner}].[{objectQualifier}User]') and name='BlockFlags')
    alter table [{databaseOwner}].[{objectQualifier}User] add [BlockFlags] [int]	NOT NULL  constraint [DF_{objectQualifier}User_Block_Flags]  default (0)
GO

if exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}EventLogGroupAccess]') and type in (N'U'))
    drop table [{databaseOwner}].[{objectQualifier}EventLogGroupAccess]
GO

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}Board]') and name='AllowThreaded')
begin
    alter table [{databaseOwner}].[{objectQualifier}Board] drop column AllowThreaded
end
GO

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}Rank]') and name='RankImage')
begin
    alter table [{databaseOwner}].[{objectQualifier}Rank] drop column RankImage
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Moderated')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [Moderated] AS (CONVERT([bit],sign([Flags]&(128)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}User]') and name='Activity')
begin
    alter table [{databaseOwner}].[{objectQualifier}User] add [Activity] [bit] NOT NULL constraint [DF_{objectQualifier}User_Activity] default (1)
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Activity]') and name=N'CreatedTopic')
begin
    alter table [{databaseOwner}].[{objectQualifier}Activity] add [CreatedTopic] AS (CONVERT([bit],sign([Flags]&(1)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Activity]') and name=N'CreatedReply')
begin
    alter table [{databaseOwner}].[{objectQualifier}Activity] add [CreatedReply] AS (CONVERT([bit],sign([Flags]&(8)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Activity]') and name=N'WasMentioned')
begin
    alter table [{databaseOwner}].[{objectQualifier}Activity] add [WasMentioned] AS (CONVERT([bit],sign([Flags]&(512)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Activity]') and name=N'ReceivedThanks')
begin
    alter table [{databaseOwner}].[{objectQualifier}Activity] add [ReceivedThanks] AS (CONVERT([bit],sign([Flags]&(1024)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Activity]') and name=N'GivenThanks')
begin
    alter table [{databaseOwner}].[{objectQualifier}Activity] add [GivenThanks] AS (CONVERT([bit],sign([Flags]&(2048)),(0)))
end
GO

if not exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Activity]') and name=N'WasQuoted')
begin
    alter table [{databaseOwner}].[{objectQualifier}Activity] add [WasQuoted] AS (CONVERT([bit],sign([Flags]&(4096)),(0)))
end
GO

if exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Mail]') and type in (N'U'))
    drop table [{databaseOwner}].[{objectQualifier}Mail]
GO

if exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}Extension]') and type in (N'U'))
    drop table [{databaseOwner}].[{objectQualifier}Extension]
GO

if exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}UserProfile]') and type in (N'U'))
    drop table [{databaseOwner}].[{objectQualifier}UserProfile]
GO

if exists(select top 1 1 from sys.columns where object_id =  object_id(N'[{databaseOwner}].[{objectQualifier}CheckEmail]') and name=N'Hash' and max_length = 64)
    alter table [{databaseOwner}].[{objectQualifier}CheckEmail] drop constraint [IX_{objectQualifier}CheckEmail]
    alter table [{databaseOwner}].[{objectQualifier}CheckEmail] alter column [Hash] nvarchar(max) NOT NULL
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Choice]') and name=N'MimeType')
begin
    alter table [{databaseOwner}].[{objectQualifier}Choice] drop column MimeType
end
GO

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'MimeType')
begin
    alter table [{databaseOwner}].[{objectQualifier}Poll] drop column MimeType
end
GO

if exists (select top 1 1 from sys.objects where name='FK_{objectQualifier}Topic_{objectQualifier}PollGroupCluster' and parent_object_id=object_id('[{databaseOwner}].[{objectQualifier}Topic]') and type in (N'F'))
    alter table [{databaseOwner}].[{objectQualifier}Topic] drop constraint [FK_{objectQualifier}Topic_{objectQualifier}PollGroupCluster]
go

if exists (select top 1 1 from sys.objects where name='FK_{objectQualifier}Poll_{objectQualifier}PollGroupCluster' and parent_object_id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and type in (N'F'))
    alter table [{databaseOwner}].[{objectQualifier}Poll] drop constraint [FK_{objectQualifier}Poll_{objectQualifier}PollGroupCluster]
go

if exists (select top 1 1 from sys.objects where name='FK_{objectQualifier}Forum_{objectQualifier}PollGroupCluster' and parent_object_id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and type in (N'F'))
    alter table [{databaseOwner}].[{objectQualifier}Forum] drop constraint [FK_{objectQualifier}Forum_{objectQualifier}PollGroupCluster]
go

if exists (select top 1 1 from sys.objects where name='FK_{objectQualifier}Category_{objectQualifier}PollGroupCluster' and parent_object_id=object_id('[{databaseOwner}].[{objectQualifier}Category]') and type in (N'F'))
    alter table [{databaseOwner}].[{objectQualifier}Category] drop constraint [FK_{objectQualifier}Category_{objectQualifier}PollGroupCluster]
go

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Poll]') and name=N'PollGroupID')
begin
    alter table [{databaseOwner}].[{objectQualifier}Poll] drop column PollGroupID
end

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Category]') and name=N'PollGroupID')
begin
    alter table [{databaseOwner}].[{objectQualifier}Category] drop column PollGroupID
end

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Forum]') and name=N'PollGroupID')
begin
    alter table [{databaseOwner}].[{objectQualifier}Forum] drop column PollGroupID
end

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}PollVoteRefuse]'))
begin
    drop table [{databaseOwner}].[{objectQualifier}PollVoteRefuse]
end
go


if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}PollGroupCluster]'))
begin
    drop table [{databaseOwner}].[{objectQualifier}PollGroupCluster]
end
go

if exists (select top 1 1 from sys.columns where object_id=object_id('[{databaseOwner}].[{objectQualifier}Medal]') and name=N'SmallMedalWidth')
begin
    alter table [{databaseOwner}].[{objectQualifier}Medal] drop column SmallMedalWidth
    alter table [{databaseOwner}].[{objectQualifier}Medal] drop column SmallMedalHeight
    alter table [{databaseOwner}].[{objectQualifier}Medal] drop column SmallRibbonWidth
    alter table [{databaseOwner}].[{objectQualifier}Medal] drop column SmallRibbonHeight
end

if exists (select top 1 1 from sys.columns where object_id = object_id(N'[{databaseOwner}].[{objectQualifier}Active]') and name='SessionID' and precision < 50)
    alter table [{databaseOwner}].[{objectQualifier}Active] drop constraint [PK_{objectQualifier}Active] with ( online = off )

    alter table [yaf].[dbo].[yaf_Active] alter column [SessionID] nvarchar (50) not null

    alter table [yaf].[dbo].[yaf_Active] with nocheck add constraint [PK_{objectQualifier}Active] primary key clustered (SessionID,BoardID)
GO