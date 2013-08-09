if not exists(select top 1 1 from sys.indexes where name=N'IX_{objectQualifier}prov_Membership_ApplicationID' and object_id=object_id(N'[{databaseOwner}].[{objectQualifier}prov_Membership]'))
 CREATE  INDEX [IX_{objectQualifier}prov_Membership_ApplicationID] ON [{databaseOwner}].[{objectQualifier}prov_Membership]([ApplicationID])
GO

if not exists(select top 1 1 from sys.indexes where name=N'IX_{objectQualifier}prov_Membership_Username' and object_id=object_id(N'[{databaseOwner}].[{objectQualifier}prov_Membership]'))
 CREATE  INDEX [IX_{objectQualifier}prov_Membership_Username] ON [{databaseOwner}].[{objectQualifier}prov_Membership]([Username])
GO

if not exists(select top 1 1 from sys.indexes where name=N'IX_{objectQualifier}prov_Membership_Email' and object_id=object_id(N'[{databaseOwner}].[{objectQualifier}prov_Membership]'))
 CREATE  INDEX [IX_{objectQualifier}prov_Membership_Email] ON [{databaseOwner}].[{objectQualifier}prov_Membership]([Email])
go

if not exists(select top 1 1 from sys.indexes where name=N'IX_{objectQualifier}prov_Application_Name' and object_id=object_id(N'[{databaseOwner}].[{objectQualifier}prov_Application]'))
 CREATE  INDEX [IX_{objectQualifier}prov_Application_Name] ON [{databaseOwner}].[{objectQualifier}prov_Application]([ApplicationName])
go

if not exists(select top 1 1 from sys.indexes where name=N'IX_{objectQualifier}prov_Role_Name' and object_id=object_id(N'[{databaseOwner}].[{objectQualifier}prov_Role]'))
 CREATE  INDEX [IX_{objectQualifier}prov_Role_Name] ON [{databaseOwner}].[{objectQualifier}prov_Role]([RoleName])
go

if not exists(select top 1 1 from sys.indexes where name=N'IX_{objectQualifier}prov_Role_ApplicationID' and object_id=object_id(N'[{databaseOwner}].[{objectQualifier}prov_Role]'))
 CREATE  INDEX [IX_{objectQualifier}prov_Role_ApplicationID] ON [{databaseOwner}].[{objectQualifier}prov_Role]([ApplicationID])
go

if not exists(select top 1 1 from sys.indexes where name=N'IX_{objectQualifier}prov_RoleMembership_RoleID' and object_id=object_id(N'[{databaseOwner}].[{objectQualifier}prov_RoleMembership]'))
 CREATE  INDEX [IX_{objectQualifier}prov_RoleMembership_RoleID] ON [{databaseOwner}].[{objectQualifier}prov_RoleMembership]([RoleID])
go

if not exists(select top 1 1 from sys.indexes where name=N'IX_{objectQualifier}prov_RoleMembership_UserID' and object_id=object_id(N'[{databaseOwner}].[{objectQualifier}prov_RoleMembership]'))
 CREATE  INDEX [IX_{objectQualifier}prov_RoleMembership_UserID] ON [{databaseOwner}].[{objectQualifier}prov_RoleMembership]([UserID])
go

if not exists (select top 1 1 from  sys.indexes where object_id=object_id('[{databaseOwner}].[{objectQualifier}prov_RoleMembership]') and name='PK_{objectQualifier}prov_RoleMembership')
	alter table [{databaseOwner}].[{objectQualifier}prov_RoleMembership] with nocheck add constraint [PK_{objectQualifier}prov_RoleMembership] primary key clustered(RoleID,UserID)   
go
