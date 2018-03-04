-- Enables FULLTEXT support for YAF
-- Must be MANUALLY run against the YAF DB

if exists (select top 1 1 from sys.columns where object_id = object_id('[{databaseOwner}].[{objectQualifier}Message]') and name = 'Message' and system_type_id = 99 
   and exists(select * from sys.sysfulltextcatalogs where name = N'YafSearch'))
begin
    alter fulltext index on [dbo].[yaf_Message] drop ([Message])
   
    alter table [{databaseOwner}].[{objectQualifier}Message] alter column [Message] nvarchar(max)
end
go

