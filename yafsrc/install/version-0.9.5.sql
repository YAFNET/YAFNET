/* Version 0.9.5 */

if exists (select * from sysobjects where id = object_id(N'yaf_extension_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_extension_list
GO

if exists (select * from sysobjects where id = object_id(N'yaf_Extension') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table yaf_Extension
GO

if exists (select * from sysobjects where id = object_id(N'yaf_user_extvalidate') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_user_extvalidate
GO
