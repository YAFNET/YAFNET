-- Disable Captchas

--EXEC [{databaseOwner}].[{objectQualifier}registry_save] 'captchatyperegister','0','1'

insert into [{databaseOwner}].[{objectQualifier}Registry](Name,Value) values(lower('captchatyperegister'),'0')