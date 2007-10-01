/*
  YAF SQL Stored Procedures File Created 03/01/06
	

  Remove Comments RegEx: \/\*(.*)\*\/
  Remove Extra Stuff: SET ANSI_NULLS ON\nGO\nSET QUOTED_IDENTIFIER ON\nGO\n\n\n 
*/

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}accessmask_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}accessmask_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_accessmask_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_accessmask_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}accessmask_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}accessmask_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_accessmask_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_accessmask_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}accessmask_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}accessmask_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_accessmask_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_accessmask_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}active_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}active_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_active_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_active_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}active_listforum]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}active_listforum]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_active_listforum]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_active_listforum]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}active_listtopic]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}active_listtopic]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_active_listtopic]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_active_listtopic]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}active_stats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}active_stats]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_active_stats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_active_stats]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}attachment_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}attachment_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_attachment_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_attachment_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}attachment_download]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}attachment_download]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_attachment_download]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_attachment_download]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}attachment_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}attachment_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_attachment_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_attachment_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}attachment_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}attachment_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_attachment_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_attachment_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bannedip_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bannedip_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_bannedip_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_bannedip_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bannedip_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bannedip_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_bannedip_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_bannedip_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bannedip_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bannedip_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_bannedip_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_bannedip_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_create]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_create]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_board_create]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_board_create]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_board_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_board_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_board_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_board_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_poststats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_poststats]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_board_poststats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_board_poststats]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_resync]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_resync]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_board_resync]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_board_resync]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_board_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_board_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_stats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_stats]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_board_stats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_board_stats]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}category_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}category_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_category_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_category_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}category_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}category_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_category_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_category_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}category_listread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}category_listread]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_category_listread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_category_listread]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}category_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}category_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_category_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_category_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}checkemail_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}checkemail_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_checkemail_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_checkemail_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}checkemail_update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}checkemail_update]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_checkemail_update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_checkemail_update]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}choice_vote]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}choice_vote]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_choice_vote]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_choice_vote]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}eventlog_create]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}eventlog_create]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_eventlog_create]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_eventlog_create]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}eventlog_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}eventlog_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_eventlog_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_eventlog_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}eventlog_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}eventlog_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_eventlog_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_eventlog_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forum_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forum_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listall]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listall]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_listall]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forum_listall]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listall_fromcat]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listall_fromcat]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_listall_fromcat]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forum_listall_fromcat]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listallmymoderated]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listallmymoderated]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_listallmymoderated]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forum_listallmymoderated]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listpath]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listpath]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_listpath]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forum_listpath]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listread]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_listread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forum_listread]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listSubForums]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listSubForums]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_listSubForums]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forum_listSubForums]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listtopics]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listtopics]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_listtopics]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forum_listtopics]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_moderatelist]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_moderatelist]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_moderatelist]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forum_moderatelist]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_moderators]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_moderators]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_moderators]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forum_moderators]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_resync]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_resync]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_resync]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forum_resync]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forum_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_updatelastpost]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_updatelastpost]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_updatelastpost]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forum_updatelastpost]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_updatestats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_updatestats]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forum_updatestats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forum_updatestats]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forumaccess_group]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forumaccess_group]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forumaccess_group]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forumaccess_group]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forumaccess_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forumaccess_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forumaccess_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forumaccess_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forumaccess_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forumaccess_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_forumaccess_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_forumaccess_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_group_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_group_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_group_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_group_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_member]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_member]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_group_member]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_group_member]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_group_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_group_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}mail_create]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}mail_create]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_mail_create]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_mail_create]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}mail_createwatch]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}mail_createwatch]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_mail_createwatch]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_mail_createwatch]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}mail_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}mail_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_mail_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_mail_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}mail_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}mail_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_mail_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_mail_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_approve]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_approve]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_message_approve]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_message_approve]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_message_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_message_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_findunread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_findunread]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_message_findunread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_message_findunread]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_getReplies]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_getReplies]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_message_getReplies]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_message_getReplies]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_message_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_message_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_listreported]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_listreported]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_message_listreported]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_message_listreported]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_report]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_report]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_message_report]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_message_report]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_reportcopyover]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_reportcopyover]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_message_reportcopyover]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_message_reportcopyover]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_reportresolve]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_reportresolve]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_message_reportresolve]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_message_reportresolve]
>>>>>>> .r1490
GO




<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_message_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_message_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_unapproved]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_unapproved]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_message_unapproved]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_message_unapproved]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_update]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_message_update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_message_update]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpforum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpforum_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_nntpforum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_nntpforum_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpforum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpforum_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_nntpforum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_nntpforum_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpforum_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpforum_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_nntpforum_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_nntpforum_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpforum_update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpforum_update]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_nntpforum_update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_nntpforum_update]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpserver_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpserver_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_nntpserver_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_nntpserver_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpserver_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpserver_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_nntpserver_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_nntpserver_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpserver_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpserver_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_nntpserver_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_nntpserver_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntptopic_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntptopic_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_nntptopic_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_nntptopic_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntptopic_savemessage]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntptopic_savemessage]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_nntptopic_savemessage]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_nntptopic_savemessage]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pageload]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pageload]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_pageload]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_pageload]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_pmessage_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_pmessage_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_info]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_info]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_pmessage_info]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_pmessage_info]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_pmessage_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_pmessage_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_markread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_markread]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_pmessage_markread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_pmessage_markread]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_prune]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_prune]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_pmessage_prune]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_pmessage_prune]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_pmessage_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_pmessage_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_archive]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_archive]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_pmessage_archive]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_pmessage_archive]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}poll_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}poll_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_poll_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_poll_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}poll_stats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}poll_stats]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_poll_stats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_poll_stats]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pollvote_check]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pollvote_check]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_pollvote_check]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_pollvote_check]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}post_last10user]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}post_last10user]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_post_last10user]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_post_last10user]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}post_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}post_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_post_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_post_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}post_list_reverse10]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}post_list_reverse10]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_post_list_reverse10]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_post_list_reverse10]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}rank_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}rank_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_rank_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_rank_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}rank_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}rank_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_rank_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_rank_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}rank_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}rank_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_rank_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_rank_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}registry_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}registry_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_registry_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_registry_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}registry_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}registry_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_registry_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_registry_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}replace_words_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}replace_words_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_replace_words_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_replace_words_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}replace_words_edit]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}replace_words_edit]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_replace_words_edit]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_replace_words_edit]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}replace_words_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}replace_words_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_replace_words_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_replace_words_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}replace_words_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}replace_words_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_replace_words_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_replace_words_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}smiley_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}smiley_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_smiley_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_smiley_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}smiley_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}smiley_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_smiley_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_smiley_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}smiley_listunique]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}smiley_listunique]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_smiley_listunique]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_smiley_listunique]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}smiley_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}smiley_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_smiley_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_smiley_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}smiley_resort]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}smiley_resort]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_smiley_resort]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_smiley_resort]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}system_initialize]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}system_initialize]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_system_initialize]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_system_initialize]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}system_updateversion]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}system_updateversion]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_system_updateversion]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_system_updateversion]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_active]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_active]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_topic_active]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_topic_active]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_topic_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_topic_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_findnext]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_findnext]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_topic_findnext]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_topic_findnext]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_findprev]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_findprev]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_topic_findprev]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_topic_findprev]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_info]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_info]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_topic_info]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_topic_info]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_announcements]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_announcements]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_topic_announcements]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_topic_announcements]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_latest]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_latest]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_topic_latest]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_topic_latest]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_topic_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_topic_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_listmessages]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_listmessages]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_topic_listmessages]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_topic_listmessages]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_lock]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_lock]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_topic_lock]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_topic_lock]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_move]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_move]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_topic_move]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_topic_move]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_prune]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_prune]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_topic_prune]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_topic_prune]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_topic_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_topic_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_updatelastpost]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_updatelastpost]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_topic_updatelastpost]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_topic_updatelastpost]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_accessmasks]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_accessmasks]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_accessmasks]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_accessmasks]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_activity_rank]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_activity_rank]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_activity_rank]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_activity_rank]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_addpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_addpoints]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_addpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_addpoints]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_adminsave]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_adminsave]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_adminsave]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_adminsave]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_approve]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_approve]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_approve]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_approve]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_approveall]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_approveall]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_approveall]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_approveall]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_aspnet]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_aspnet]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_aspnet]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_aspnet]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_avatarimage]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_avatarimage]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_avatarimage]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_avatarimage]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_changepassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_changepassword]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_changepassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_changepassword]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_deleteavatar]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_deleteavatar]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_deleteavatar]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_deleteavatar]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_deleteold]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_deleteold]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_deleteold]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_deleteold]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_emails]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_emails]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_emails]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_emails]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_find]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_find]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_find]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_find]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_getpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_getpoints]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_getpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_getpoints]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_getsignature]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_getsignature]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_getsignature]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_getsignature]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_guest]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_guest]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_guest]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_guest]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_login]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_login]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_login]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_login]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_nntp]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_nntp]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_nntp]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_nntp]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_recoverpassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_recoverpassword]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_recoverpassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_recoverpassword]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_removepoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_removepoints]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_removepoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_removepoints]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_removepointsbytopicid]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_removepointsbytopicid]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_removepointsbytopicid]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_removepointsbytopicid]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_resetpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_resetpoints]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_resetpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_resetpoints]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_saveavatar]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_saveavatar]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_saveavatar]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_saveavatar]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_savepassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_savepassword]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_savepassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_savepassword]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_savesignature]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_savesignature]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_savesignature]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_savesignature]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_setpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_setpoints]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_setpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_setpoints]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_setrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_setrole]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_setrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_setrole]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_suspend]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_suspend]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_suspend]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_suspend]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_upgrade]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_upgrade]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_user_upgrade]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_user_upgrade]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}userforum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}userforum_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_userforum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_userforum_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}userforum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}userforum_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_userforum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_userforum_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}userforum_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}userforum_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_userforum_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_userforum_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}usergroup_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}usergroup_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_usergroup_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_usergroup_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}usergroup_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}usergroup_save]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_usergroup_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_usergroup_save]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}userpmessage_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}userpmessage_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_userpmessage_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_userpmessage_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}userpmessage_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}userpmessage_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_userpmessage_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_userpmessage_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchforum_add]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchforum_add]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_watchforum_add]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_watchforum_add]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchforum_check]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchforum_check]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_watchforum_check]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_watchforum_check]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchforum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchforum_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_watchforum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_watchforum_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchforum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchforum_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_watchforum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_watchforum_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchtopic_add]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchtopic_add]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_watchtopic_add]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_watchtopic_add]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchtopic_check]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchtopic_check]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_watchtopic_check]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_watchtopic_check]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchtopic_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchtopic_delete]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_watchtopic_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_watchtopic_delete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchtopic_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchtopic_list]
=======
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[yaf_watchtopic_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_watchtopic_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_reply_list]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_reply_list]
=======
IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'yaf_message_reply_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_message_reply_list]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF EXISTS (SELECT 1 FROM sysobjects WHERE id = object_id(N'[{databaseOwner}].[{objectQualifier}message_deleteundelete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_deleteundelete]
=======
IF EXISTS (SELECT 1 FROM sysobjects WHERE id = object_id(N'yaf_message_deleteundelete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_message_deleteundelete]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF EXISTS (SELECT 1 FROM sysobjects WHERE id = object_id(N'[{databaseOwner}].[{objectQualifier}topic_create_by_message]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_create_by_message]
=======
IF EXISTS (SELECT 1 FROM sysobjects WHERE id = object_id(N'yaf_topic_create_by_message') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_topic_create_by_message]
>>>>>>> .r1490
GO

<<<<<<< .mine
IF EXISTS (SELECT 1 FROM sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}message_move]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_move]
=======
IF EXISTS (SELECT 1 FROM sysobjects where id = object_id(N'yaf_message_move') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[yaf_message_move]
>>>>>>> .r1490
GO

IF EXISTS (SELECT *
<<<<<<< .mine
           FROM   dbo.sysobjects
           WHERE  id = Object_id(N'[{databaseOwner}].[{objectQualifier}category_simplelist]')
=======
           FROM   sysobjects
           WHERE  id = Object_id(N'[{databaseOwner}].[yaf_category_simplelist]')
>>>>>>> .r1490
           AND Objectproperty(id,N'IsProcedure') = 1)
<<<<<<< .mine
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}category_simplelist] 
=======
DROP PROCEDURE [{databaseOwner}].[yaf_category_simplelist] 
>>>>>>> .r1490
GO

IF EXISTS (SELECT *
<<<<<<< .mine
           FROM   dbo.sysobjects
           WHERE  id = Object_id(N'[{databaseOwner}].[{objectQualifier}forum_simplelist]')
=======
           FROM   sysobjects
           WHERE  id = Object_id(N'[{databaseOwner}].[yaf_forum_simplelist]')
>>>>>>> .r1490
           AND Objectproperty(id,N'IsProcedure') = 1)
<<<<<<< .mine
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_simplelist] 
=======
DROP PROCEDURE [{databaseOwner}].[yaf_forum_simplelist] 
>>>>>>> .r1490
GO

IF EXISTS (SELECT *
<<<<<<< .mine
           FROM   dbo.sysobjects
           WHERE  id = Object_id(N'[{databaseOwner}].[{objectQualifier}message_simplelist]')
=======
           FROM   sysobjects
           WHERE  id = Object_id(N'[{databaseOwner}].[yaf_message_simplelist]')
>>>>>>> .r1490
           AND Objectproperty(id,N'IsProcedure') = 1)
<<<<<<< .mine
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_simplelist] 
=======
DROP PROCEDURE [{databaseOwner}].[yaf_message_simplelist] 
>>>>>>> .r1490
GO

IF EXISTS (SELECT *
<<<<<<< .mine
           FROM   dbo.sysobjects
           WHERE  id = Object_id(N'[{databaseOwner}].[{objectQualifier}topic_simplelist]')
=======
           FROM   sysobjects
           WHERE  id = Object_id(N'[{databaseOwner}].[yaf_topic_simplelist]')
>>>>>>> .r1490
           AND Objectproperty(id,N'IsProcedure') = 1)
<<<<<<< .mine
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_simplelist] 
=======
DROP PROCEDURE [{databaseOwner}].[yaf_topic_simplelist] 
>>>>>>> .r1490
GO

IF EXISTS (SELECT *
<<<<<<< .mine
           FROM   dbo.sysobjects
           WHERE  id = Object_id(N'[{databaseOwner}].[{objectQualifier}user_simplelist]')
=======
           FROM   sysobjects
           WHERE  id = Object_id(N'[{databaseOwner}].[yaf_user_simplelist]')
>>>>>>> .r1490
           AND Objectproperty(id,N'IsProcedure') = 1)
<<<<<<< .mine
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_simplelist] 
=======
DROP PROCEDURE [{databaseOwner}].[yaf_user_simplelist] 
>>>>>>> .r1490
GO


<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}accessmask_delete](@AccessMaskID int) as
=======
create procedure [{databaseOwner}].[yaf_accessmask_delete](@AccessMaskID int) as
>>>>>>> .r1490
begin
	declare @flag int
	
	set @flag=1
	if exists(select 1 from [{databaseOwner}].{objectQualifier}ForumAccess where AccessMaskID=@AccessMaskID) or exists(select 1 from [{databaseOwner}].{objectQualifier}UserForum where AccessMaskID=@AccessMaskID)
		set @flag=0
	else
		delete from [{databaseOwner}].{objectQualifier}AccessMask where AccessMaskID=@AccessMaskID
	
	select @flag
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}accessmask_list](@BoardID int,@AccessMaskID int=null) as
=======
create procedure [{databaseOwner}].[yaf_accessmask_list](@BoardID int,@AccessMaskID int=null) as
>>>>>>> .r1490
begin
	if @AccessMaskID is null
		select 
			a.* 
		from 
			{objectQualifier}AccessMask a 
		where
			a.BoardID = @BoardID
		order by 
			a.Name
	else
		select 
			a.* 
		from 
			{objectQualifier}AccessMask a 
		where
			a.BoardID = @BoardID and
			a.AccessMaskID = @AccessMaskID
		order by 
			a.Name
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}accessmask_save](
=======
create procedure [{databaseOwner}].[yaf_accessmask_save](
>>>>>>> .r1490
	@AccessMaskID		int=null,
	@BoardID			int,
	@Name				nvarchar(50),
	@ReadAccess			bit,
	@PostAccess			bit,
	@ReplyAccess		bit,
	@PriorityAccess		bit,
	@PollAccess			bit,
	@VoteAccess			bit,
	@ModeratorAccess	bit,
	@EditAccess			bit,
	@DeleteAccess		bit,
	@UploadAccess		bit
) as
begin
	declare @Flags	int
	
	set @Flags = 0
	if @ReadAccess<>0 set @Flags = @Flags | 1
	if @PostAccess<>0 set @Flags = @Flags | 2
	if @ReplyAccess<>0 set @Flags = @Flags | 4
	if @PriorityAccess<>0 set @Flags = @Flags | 8
	if @PollAccess<>0 set @Flags = @Flags | 16
	if @VoteAccess<>0 set @Flags = @Flags | 32
	if @ModeratorAccess<>0 set @Flags = @Flags | 64
	if @EditAccess<>0 set @Flags = @Flags | 128
	if @DeleteAccess<>0 set @Flags = @Flags | 256
	if @UploadAccess<>0 set @Flags = @Flags | 512

	if @AccessMaskID is null
		insert into [{databaseOwner}].{objectQualifier}AccessMask(Name,BoardID,Flags)
		values(@Name,@BoardID,@Flags)
	else
		update [{databaseOwner}].{objectQualifier}AccessMask set
			Name			= @Name,
			Flags			= @Flags
		where AccessMaskID=@AccessMaskID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}active_list](@BoardID int,@Guests bit=0) as
=======
create procedure [{databaseOwner}].[yaf_active_list](@BoardID int,@Guests bit=0) as
>>>>>>> .r1490
begin
	-- delete non-active
	delete from [{databaseOwner}].{objectQualifier}Active where DATEDIFF(minute,LastActive,getdate())>5
	-- select active
	if @Guests<>0
		select
			a.UserID,
			a.Name,
			c.IP,
			c.SessionID,
			c.ForumID,
			c.TopicID,
			ForumName = (select Name from [{databaseOwner}].{objectQualifier}Forum x where x.ForumID=c.ForumID),
			TopicName = (select Topic from [{databaseOwner}].{objectQualifier}Topic x where x.TopicID=c.TopicID),
			IsGuest = (select 1 from [{databaseOwner}].{objectQualifier}UserGroup x,{objectQualifier}Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and (y.Flags & 2)<>0),
			c.Login,
			c.LastActive,
			c.Location,
			Active = DATEDIFF(minute,c.Login,c.LastActive),
			c.Browser,
			c.Platform
		from
			{objectQualifier}User a,
			{objectQualifier}Active c
		where
			c.UserID = a.UserID and
			c.BoardID = @BoardID
		order by
			c.LastActive desc
	else
		select
			a.UserID,
			a.Name,
			c.IP,
			c.SessionID,
			c.ForumID,
			c.TopicID,
			ForumName = (select Name from [{databaseOwner}].{objectQualifier}Forum x where x.ForumID=c.ForumID),
			TopicName = (select Topic from [{databaseOwner}].{objectQualifier}Topic x where x.TopicID=c.TopicID),
			IsGuest = (select 1 from [{databaseOwner}].{objectQualifier}UserGroup x,{objectQualifier}Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and (y.Flags & 2)<>0),
			c.Login,
			c.LastActive,
			c.Location,
			Active = DATEDIFF(minute,c.Login,c.LastActive),
			c.Browser,
			c.Platform
		from
			{objectQualifier}User a,
			{objectQualifier}Active c
		where
			c.UserID = a.UserID and
			c.BoardID = @BoardID and
			not exists(select 1 from [{databaseOwner}].{objectQualifier}UserGroup x,{objectQualifier}Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and (y.Flags & 2)<>0)
		order by
			c.LastActive desc
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}active_listforum](@ForumID int) as
=======
create procedure [{databaseOwner}].[yaf_active_listforum](@ForumID int) as
>>>>>>> .r1490
begin
	select
		UserID		= a.UserID,
		UserName	= b.Name
	from
		{objectQualifier}Active a join [{databaseOwner}].{objectQualifier}User b on b.UserID=a.UserID
	where
		a.ForumID = @ForumID
	group by
		a.UserID,
		b.Name
	order by
		b.Name
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}active_listtopic](@TopicID int) as
=======
create procedure [{databaseOwner}].[yaf_active_listtopic](@TopicID int) as
>>>>>>> .r1490
begin
	select
		UserID		= a.UserID,
		UserName	= b.Name
	from
		{objectQualifier}Active a with(nolock)
		join [{databaseOwner}].{objectQualifier}User b on b.UserID=a.UserID
	where
		a.TopicID = @TopicID
	group by
		a.UserID,
		b.Name
	order by
		b.Name
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}active_stats](@BoardID int) as
=======
create procedure [{databaseOwner}].[yaf_active_stats](@BoardID int) as
>>>>>>> .r1490
begin
	select
		ActiveUsers = (select count(1) from [{databaseOwner}].{objectQualifier}Active where BoardID=@BoardID),
		ActiveMembers = (select count(1) from [{databaseOwner}].{objectQualifier}Active x where BoardID=@BoardID and exists(select 1 from [{databaseOwner}].{objectQualifier}UserGroup y,{objectQualifier}Group z where y.UserID=x.UserID and y.GroupID=z.GroupID and (z.Flags & 2)=0)),
		ActiveGuests = (select count(1) from [{databaseOwner}].{objectQualifier}Active x where BoardID=@BoardID and exists(select 1 from [{databaseOwner}].{objectQualifier}UserGroup y,{objectQualifier}Group z where y.UserID=x.UserID and y.GroupID=z.GroupID and (z.Flags & 2)<>0))
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}attachment_delete](@AttachmentID int) as begin
	delete from [{databaseOwner}].{objectQualifier}Attachment where AttachmentID=@AttachmentID
=======
create procedure [{databaseOwner}].[yaf_attachment_delete](@AttachmentID int) as begin
	delete from yaf_Attachment where AttachmentID=@AttachmentID
>>>>>>> .r1490
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}attachment_download](@AttachmentID int) as
=======
create procedure [{databaseOwner}].[yaf_attachment_download](@AttachmentID int) as
>>>>>>> .r1490
begin
	update [{databaseOwner}].{objectQualifier}Attachment set Downloads=Downloads+1 where AttachmentID=@AttachmentID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}attachment_list](@MessageID int=null,@AttachmentID int=null,@BoardID int=null) as begin
=======
create procedure [{databaseOwner}].[yaf_attachment_list](@MessageID int=null,@AttachmentID int=null,@BoardID int=null) as begin
>>>>>>> .r1490
	if @MessageID is not null
		select * from [{databaseOwner}].{objectQualifier}Attachment where MessageID=@MessageID
	else if @AttachmentID is not null
		select * from [{databaseOwner}].{objectQualifier}Attachment where AttachmentID=@AttachmentID
	else
		select 
			a.*,
			Posted		= b.Posted,
			ForumID		= d.ForumID,
			ForumName	= d.Name,
			TopicID		= c.TopicID,
			TopicName	= c.Topic
		from 
			{objectQualifier}Attachment a,
			{objectQualifier}Message b,
			{objectQualifier}Topic c,
			{objectQualifier}Forum d,
			{objectQualifier}Category e
		where
			b.MessageID = a.MessageID and
			c.TopicID = b.TopicID and
			d.ForumID = c.ForumID and
			e.CategoryID = d.CategoryID and
			e.BoardID = @BoardID
		order by
			d.Name,
			c.Topic,
			b.Posted
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}attachment_save](@MessageID int,@FileName nvarchar(255),@Bytes int,@ContentType nvarchar(50)=null,@FileData image=null) as begin
	insert into [{databaseOwner}].{objectQualifier}Attachment(MessageID,FileName,Bytes,ContentType,Downloads,FileData) values(@MessageID,@FileName,@Bytes,@ContentType,0,@FileData)
=======
create procedure [{databaseOwner}].[yaf_attachment_save](@MessageID int,@FileName nvarchar(255),@Bytes int,@ContentType nvarchar(50)=null,@FileData image=null) as begin
	insert into yaf_Attachment(MessageID,FileName,Bytes,ContentType,Downloads,FileData) values(@MessageID,@FileName,@Bytes,@ContentType,0,@FileData)
>>>>>>> .r1490
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}bannedip_delete](@ID int) as
=======
create procedure [{databaseOwner}].[yaf_bannedip_delete](@ID int) as
>>>>>>> .r1490
begin
	delete from [{databaseOwner}].{objectQualifier}BannedIP where ID = @ID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}bannedip_list](@BoardID int,@ID int=null) as
=======
create procedure [{databaseOwner}].[yaf_bannedip_list](@BoardID int,@ID int=null) as
>>>>>>> .r1490
begin
	if @ID is null
		select * from [{databaseOwner}].{objectQualifier}BannedIP where BoardID=@BoardID
	else
		select * from [{databaseOwner}].{objectQualifier}BannedIP where BoardID=@BoardID and ID=@ID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}bannedip_save](@ID int=null,@BoardID int,@Mask nvarchar(15)) as
=======
create procedure [{databaseOwner}].[yaf_bannedip_save](@ID int=null,@BoardID int,@Mask nvarchar(15)) as
>>>>>>> .r1490
begin
	if @ID is null or @ID = 0 begin
		insert into [{databaseOwner}].{objectQualifier}BannedIP(BoardID,Mask,Since) values(@BoardID,@Mask,getdate())
	end
	else begin
		update [{databaseOwner}].{objectQualifier}BannedIP set Mask = @Mask where ID = @ID
	end
end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}board_create](
=======
CREATE procedure [{databaseOwner}].[yaf_board_create](
>>>>>>> .r1490
	@BoardName 		nvarchar(50),
	@AllowThreaded	bit,
	@UserName		nvarchar(50),
	@UserEmail		nvarchar(50),
	@UserPass		nvarchar(32),
	@IsHostAdmin	bit
) as 
begin
	declare @BoardID				int
	declare @TimeZone				int
	declare @ForumEmail				nvarchar(50)
	declare	@GroupIDAdmin			int
	declare	@GroupIDGuest			int
	declare @GroupIDMember			int
	declare	@AccessMaskIDAdmin		int
	declare @AccessMaskIDModerator	int
	declare @AccessMaskIDMember		int
	declare	@AccessMaskIDReadOnly	int
	declare @UserIDAdmin			int
	declare @UserIDGuest			int
	declare @RankIDAdmin			int
	declare @RankIDGuest			int
	declare @RankIDNewbie			int
	declare @RankIDMember			int
	declare @RankIDAdvanced			int
	declare	@CategoryID				int
	declare	@ForumID				int
	declare @UserFlags				int

	SET @TimeZone = (SELECT CAST(CAST([Value] as nvarchar(50)) as int) FROM [{databaseOwner}].{objectQualifier}Registry WHERE LOWER([Name]) = LOWER('TimeZone'))
	SET @ForumEmail = (SELECT CAST([Value] as nvarchar(50)) FROM [{databaseOwner}].{objectQualifier}Registry WHERE LOWER([Name]) = LOWER('ForumEmail'))

	-- {objectQualifier}Board
	insert into [{databaseOwner}].{objectQualifier}Board(Name,AllowThreaded) values(@BoardName,@AllowThreaded)
	set @BoardID = SCOPE_IDENTITY()

	-- {objectQualifier}Rank
	insert into [{databaseOwner}].{objectQualifier}Rank(BoardID,Name,Flags,MinPosts) values(@BoardID,'Administration',0,null)
	set @RankIDAdmin = SCOPE_IDENTITY()
	insert into [{databaseOwner}].{objectQualifier}Rank(BoardID,Name,Flags,MinPosts) values(@BoardID,'Guest',0,null)
	set @RankIDGuest = SCOPE_IDENTITY()
	insert into [{databaseOwner}].{objectQualifier}Rank(BoardID,Name,Flags,MinPosts) values(@BoardID,'Newbie',3,0)
	set @RankIDNewbie = SCOPE_IDENTITY()
	insert into [{databaseOwner}].{objectQualifier}Rank(BoardID,Name,Flags,MinPosts) values(@BoardID,'Member',2,10)
	set @RankIDMember = SCOPE_IDENTITY()
	insert into [{databaseOwner}].{objectQualifier}Rank(BoardID,Name,Flags,MinPosts) values(@BoardID,'Advanced Member',2,30)
	set @RankIDAdvanced = SCOPE_IDENTITY()

	-- {objectQualifier}AccessMask
	insert into [{databaseOwner}].{objectQualifier}AccessMask(BoardID,Name,Flags)
	values(@BoardID,'Admin Access',1023)
	set @AccessMaskIDAdmin = SCOPE_IDENTITY()
	insert into [{databaseOwner}].{objectQualifier}AccessMask(BoardID,Name,Flags)
	values(@BoardID,'Moderator Access',487)
	set @AccessMaskIDModerator = SCOPE_IDENTITY()
	insert into [{databaseOwner}].{objectQualifier}AccessMask(BoardID,Name,Flags)
	values(@BoardID,'Member Access',423)
	set @AccessMaskIDMember = SCOPE_IDENTITY()
	insert into [{databaseOwner}].{objectQualifier}AccessMask(BoardID,Name,Flags)
	values(@BoardID,'Read Only Access',1)
	set @AccessMaskIDReadOnly = SCOPE_IDENTITY()
	insert into [{databaseOwner}].{objectQualifier}AccessMask(BoardID,Name,Flags)
	values(@BoardID,'No Access',0)

	-- {objectQualifier}Group
	insert into [{databaseOwner}].{objectQualifier}Group(BoardID,Name,Flags) values(@BoardID,'Administrators',1)
	set @GroupIDAdmin = SCOPE_IDENTITY()
	insert into [{databaseOwner}].{objectQualifier}Group(BoardID,Name,Flags) values(@BoardID,'Guests',2)
	set @GroupIDGuest = SCOPE_IDENTITY()
	insert into [{databaseOwner}].{objectQualifier}Group(BoardID,Name,Flags) values(@BoardID,'Registered',4)
	set @GroupIDMember = SCOPE_IDENTITY()	
	
	-- {objectQualifier}User
	insert into [{databaseOwner}].{objectQualifier}User(BoardID,RankID,Name,Password,Joined,LastVisit,NumPosts,TimeZone,Email,Flags)
	values(@BoardID,@RankIDGuest,'Guest','na',getdate(),getdate(),0,@TimeZone,@ForumEmail,6)
	set @UserIDGuest = SCOPE_IDENTITY()	
	
	SET @UserFlags = 2
	if @IsHostAdmin<>0 SET @UserFlags = 3
	
	insert into [{databaseOwner}].{objectQualifier}User(BoardID,RankID,Name,Password,Joined,LastVisit,NumPosts,TimeZone,Email,Flags)
	values(@BoardID,@RankIDAdmin,@UserName,@UserPass,getdate(),getdate(),0,@TimeZone,@UserEmail,@UserFlags)
	set @UserIDAdmin = SCOPE_IDENTITY()

	-- {objectQualifier}UserGroup
	insert into [{databaseOwner}].{objectQualifier}UserGroup(UserID,GroupID) values(@UserIDAdmin,@GroupIDAdmin)
	insert into [{databaseOwner}].{objectQualifier}UserGroup(UserID,GroupID) values(@UserIDGuest,@GroupIDGuest)

	-- {objectQualifier}Category
	insert into [{databaseOwner}].{objectQualifier}Category(BoardID,Name,SortOrder) values(@BoardID,'Test Category',1)
	set @CategoryID = SCOPE_IDENTITY()
	
	-- {objectQualifier}Forum
	insert into [{databaseOwner}].{objectQualifier}Forum(CategoryID,Name,Description,SortOrder,NumTopics,NumPosts,Flags)
	values(@CategoryID,'Test Forum','A test forum',1,0,0,4)
	set @ForumID = SCOPE_IDENTITY()

	-- {objectQualifier}ForumAccess
	insert into [{databaseOwner}].{objectQualifier}ForumAccess(GroupID,ForumID,AccessMaskID) values(@GroupIDAdmin,@ForumID,@AccessMaskIDAdmin)
	insert into [{databaseOwner}].{objectQualifier}ForumAccess(GroupID,ForumID,AccessMaskID) values(@GroupIDGuest,@ForumID,@AccessMaskIDReadOnly)
	insert into [{databaseOwner}].{objectQualifier}ForumAccess(GroupID,ForumID,AccessMaskID) values(@GroupIDMember,@ForumID,@AccessMaskIDMember)
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}board_delete](@BoardID int) as
=======
create procedure [{databaseOwner}].[yaf_board_delete](@BoardID int) as
>>>>>>> .r1490
begin
	declare @tmpForumID int;
	declare forum_cursor cursor for
		select ForumID 
		from [{databaseOwner}].{objectQualifier}Forum a join [{databaseOwner}].{objectQualifier}Category b on a.CategoryID=b.CategoryID
		where b.BoardID=@BoardID
		order by ForumID desc
	
	open forum_cursor
	fetch next from forum_cursor into @tmpForumID
	while @@FETCH_STATUS = 0
	begin
		exec [{databaseOwner}].{objectQualifier}forum_delete @tmpForumID;
		fetch next from forum_cursor into @tmpForumID
	end
	close forum_cursor
	deallocate forum_cursor

	delete from [{databaseOwner}].{objectQualifier}ForumAccess where exists(select 1 from [{databaseOwner}].{objectQualifier}Group x where x.GroupID={objectQualifier}ForumAccess.GroupID and x.BoardID=@BoardID)
	delete from [{databaseOwner}].{objectQualifier}Forum where exists(select 1 from [{databaseOwner}].{objectQualifier}Category x where x.CategoryID={objectQualifier}Forum.CategoryID and x.BoardID=@BoardID)
	delete from [{databaseOwner}].{objectQualifier}UserGroup where exists(select 1 from [{databaseOwner}].{objectQualifier}User x where x.UserID={objectQualifier}UserGroup.UserID and x.BoardID=@BoardID)
	delete from [{databaseOwner}].{objectQualifier}Category where BoardID=@BoardID
	delete from [{databaseOwner}].{objectQualifier}User where BoardID=@BoardID
	delete from [{databaseOwner}].{objectQualifier}Rank where BoardID=@BoardID
	delete from [{databaseOwner}].{objectQualifier}Group where BoardID=@BoardID
	delete from [{databaseOwner}].{objectQualifier}AccessMask where BoardID=@BoardID
	delete from [{databaseOwner}].{objectQualifier}Active where BoardID=@BoardID
	delete from [{databaseOwner}].{objectQualifier}Board where BoardID=@BoardID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}board_list](@BoardID int=null) as
=======
create procedure [{databaseOwner}].[yaf_board_list](@BoardID int=null) as
>>>>>>> .r1490
begin
	select
		a.*,
		SQLVersion = @@VERSION
	from 
		{objectQualifier}Board a
	where
		(@BoardID is null or a.BoardID = @BoardID)
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}board_poststats](@BoardID int) as
=======
create procedure [{databaseOwner}].[yaf_board_poststats](@BoardID int) as
>>>>>>> .r1490
begin
	select
		Posts = (select count(1) from [{databaseOwner}].{objectQualifier}Message a join [{databaseOwner}].{objectQualifier}Topic b on b.TopicID=a.TopicID join [{databaseOwner}].{objectQualifier}Forum c on c.ForumID=b.ForumID join [{databaseOwner}].{objectQualifier}Category d on d.CategoryID=c.CategoryID where d.BoardID=@BoardID),
		Topics = (select count(1) from [{databaseOwner}].{objectQualifier}Topic a join [{databaseOwner}].{objectQualifier}Forum b on b.ForumID=a.ForumID join [{databaseOwner}].{objectQualifier}Category c on c.CategoryID=b.CategoryID where c.BoardID=@BoardID),
		Forums = (select count(1) from [{databaseOwner}].{objectQualifier}Forum a join [{databaseOwner}].{objectQualifier}Category b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID),
		Members = (select count(1) from [{databaseOwner}].{objectQualifier}User a where a.BoardID=@BoardID),
		LastPostInfo.*,
		LastMemberInfo.*
	from
		(
			select top 1 
				LastMemberInfoID= 1,
				LastMemberID	= UserID,
				LastMember	= Name
			from 
				{objectQualifier}User 
			where 
				(Flags & 2) = 2 and
				BoardID=@BoardID 
			order by 
				Joined desc
		) as LastMemberInfo
		left join (
			select top 1 
				LastPostInfoID	= 1,
				LastPost	= a.Posted,
				LastUserID	= a.UserID,
				LastUser	= e.Name
			from 
				{objectQualifier}Message a 
				join [{databaseOwner}].{objectQualifier}Topic b on b.TopicID=a.TopicID 
				join [{databaseOwner}].{objectQualifier}Forum c on c.ForumID=b.ForumID 
				join [{databaseOwner}].{objectQualifier}Category d on d.CategoryID=c.CategoryID 
				join [{databaseOwner}].{objectQualifier}User e on e.UserID=a.UserID
			where 
				d.BoardID=@BoardID
			order by
				a.Posted desc
		) as LastPostInfo
		on LastMemberInfoID=LastPostInfoID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}board_save](@BoardID int,@Name nvarchar(50),@AllowThreaded bit) as
=======
create procedure [{databaseOwner}].[yaf_board_save](@BoardID int,@Name nvarchar(50),@AllowThreaded bit) as
>>>>>>> .r1490
begin
	update [{databaseOwner}].{objectQualifier}Board set
		Name = @Name,
		AllowThreaded = @AllowThreaded
	where BoardID=@BoardID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}board_stats]
=======
create procedure [{databaseOwner}].[yaf_board_stats]
>>>>>>> .r1490
	@BoardID	int = null
as 
begin
	if (@BoardID is null) begin
		select
			NumPosts	= (select count(1) from [{databaseOwner}].{objectQualifier}Message	where (Flags & 24)=16),
			NumTopics	= (select count(1) from [{databaseOwner}].{objectQualifier}Topic),
			NumUsers	= (select count(1) from [{databaseOwner}].{objectQualifier}User where (Flags & 2) = 2),
			BoardStart	= (select min(Joined) from [{databaseOwner}].{objectQualifier}User)
	end
	else begin
		select
			NumPosts	= (select count(1)	
								from [{databaseOwner}].{objectQualifier}Message a
								join [{databaseOwner}].{objectQualifier}Topic b ON a.TopicID=b.TopicID
								join [{databaseOwner}].{objectQualifier}Forum c ON b.ForumID=c.ForumID
								join [{databaseOwner}].{objectQualifier}Category d ON c.CategoryID=d.CategoryID
								where (a.Flags & 24)=16 and d.BoardID=@BoardID
							),
			NumTopics	= (select count(1) 
								from [{databaseOwner}].{objectQualifier}Topic a
								join [{databaseOwner}].{objectQualifier}Forum b ON a.ForumID=b.ForumID
								join [{databaseOwner}].{objectQualifier}Category c ON b.CategoryID=c.CategoryID
								where c.BoardID=@BoardID
							),
			NumUsers	= (select count(1) from [{databaseOwner}].{objectQualifier}User where (Flags & 2) = 2 and BoardID=@BoardID),
			BoardStart	= (select min(Joined) from [{databaseOwner}].{objectQualifier}User where BoardID=@BoardID)
	end
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}category_delete](@CategoryID int) as
=======
create procedure [{databaseOwner}].[yaf_category_delete](@CategoryID int) as
>>>>>>> .r1490
begin
	declare @flag int
 
	if exists(select 1 from [{databaseOwner}].{objectQualifier}Forum where CategoryID =  @CategoryID)
	begin
		set @flag = 0
	end else
	begin
		delete from [{databaseOwner}].{objectQualifier}Category where CategoryID = @CategoryID
		set @flag = 1
	end

	select @flag
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}category_list](@BoardID int,@CategoryID int=null) as
=======
create procedure [{databaseOwner}].[yaf_category_list](@BoardID int,@CategoryID int=null) as
>>>>>>> .r1490
begin
	if @CategoryID is null
		select * from [{databaseOwner}].{objectQualifier}Category where BoardID = @BoardID order by SortOrder
	else
		select * from [{databaseOwner}].{objectQualifier}Category where BoardID = @BoardID and CategoryID = @CategoryID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}category_listread](@BoardID int,@UserID int,@CategoryID int=null) as
=======
create procedure [{databaseOwner}].[yaf_category_listread](@BoardID int,@UserID int,@CategoryID int=null) as
>>>>>>> .r1490
begin
	select 
		a.CategoryID,
		a.Name
	from 
		{objectQualifier}Category a
		join [{databaseOwner}].{objectQualifier}Forum b on b.CategoryID=a.CategoryID
		join [{databaseOwner}].{objectQualifier}vaccess v on v.ForumID=b.ForumID
	where
		a.BoardID=@BoardID and
		v.UserID=@UserID and
		(v.ReadAccess<>0 or (b.Flags & 2)=0) and
		(@CategoryID is null or a.CategoryID=@CategoryID) and
		b.ParentID is null
	group by
		a.CategoryID,
		a.Name,
		a.SortOrder
	order by 
		a.SortOrder
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}category_save](@BoardID int,@CategoryID int,@Name nvarchar(50),@SortOrder smallint) as
=======
create procedure [{databaseOwner}].[yaf_category_save](@BoardID int,@CategoryID int,@Name nvarchar(50),@SortOrder smallint) as
>>>>>>> .r1490
begin
	if @CategoryID>0 begin
		update [{databaseOwner}].{objectQualifier}Category set Name=@Name,SortOrder=@SortOrder where CategoryID=@CategoryID
		select CategoryID = @CategoryID
	end
	else begin
		insert into [{databaseOwner}].{objectQualifier}Category(BoardID,Name,SortOrder) values(@BoardID,@Name,@SortOrder)
		select CategoryID = SCOPE_IDENTITY()
	end
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}checkemail_save](@UserID int,@Hash nvarchar(32),@Email nvarchar(50)) as
=======
create procedure [{databaseOwner}].[yaf_checkemail_save](@UserID int,@Hash nvarchar(32),@Email nvarchar(50)) as
>>>>>>> .r1490
begin
	insert into [{databaseOwner}].{objectQualifier}CheckEmail(UserID,Email,Created,Hash)
	values(@UserID,@Email,getdate(),@Hash)	
end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}checkemail_update](@Hash nvarchar(32)) as
=======
CREATE procedure [{databaseOwner}].[yaf_checkemail_update](@Hash nvarchar(32)) as
>>>>>>> .r1490
begin
	declare @UserID int
	declare @CheckEmailID int
	declare @Email nvarchar(50)

	set @UserID = null

	select 
		@CheckEmailID = CheckEmailID,
		@UserID = UserID,
		@Email = Email
	from
		{objectQualifier}CheckEmail
	where
		Hash = @Hash

	if @UserID is null
	begin
		select convert(uniqueidentifier,NULL) as ProviderUserKey, convert(nvarchar(255),NULL) as Email
		return
	end

	-- Update new user email
	update [{databaseOwner}].{objectQualifier}User set Email = @Email, Flags = Flags | 2 where UserID = @UserID
	delete [{databaseOwner}].{objectQualifier}CheckEmail where CheckEmailID = @CheckEmailID

	-- return the UserProviderKey
	SELECT ProviderUserKey, Email FROM [{databaseOwner}].{objectQualifier}User WHERE UserID = @UserID
end
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}choice_vote](@ChoiceID int,@UserID int = NULL, @RemoteIP nvarchar(10) = NULL) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_choice_vote](@ChoiceID int,@UserID int = NULL, @RemoteIP nvarchar(10) = NULL) AS
>>>>>>> .r1490
BEGIN
	DECLARE @PollID int

	SET @PollID = (SELECT PollID FROM [{databaseOwner}].{objectQualifier}Choice WHERE ChoiceID = @ChoiceID)

	IF @UserID = NULL
	BEGIN
		IF @RemoteIP != NULL
		BEGIN
			INSERT INTO [{databaseOwner}].{objectQualifier}PollVote (PollID, UserID, RemoteIP) VALUES (@PollID,NULL,@RemoteIP)	
		END
	END
	ELSE
	BEGIN
		INSERT INTO [{databaseOwner}].{objectQualifier}PollVote (PollID, UserID, RemoteIP) VALUES (@PollID,@UserID,@RemoteIP)
	END

	UPDATE [{databaseOwner}].{objectQualifier}Choice SET Votes = Votes + 1 WHERE ChoiceID = @ChoiceID
END
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}eventlog_create](@UserID int,@Source nvarchar(50),@Description ntext,@Type int) as
=======
create procedure [{databaseOwner}].[yaf_eventlog_create](@UserID int,@Source nvarchar(50),@Description ntext,@Type int) as
>>>>>>> .r1490
begin
<<<<<<< .mine
	insert into [{databaseOwner}].{objectQualifier}EventLog(UserID,Source,Description,Type)
=======
	insert into {databaseOwner}.yaf_EventLog(UserID,Source,Description,Type)
>>>>>>> .r1490
	values(@UserID,@Source,@Description,@Type)

	-- delete entries older than 10 days
<<<<<<< .mine
	delete from [{databaseOwner}].{objectQualifier}EventLog where EventTime+10<getdate()
=======
	delete from {databaseOwner}.yaf_EventLog where EventTime+10<getdate()
>>>>>>> .r1490

	-- or if there are more then 1000	
	if ((select count(*) from [{databaseOwner}].{objectQualifier}eventlog) >= 1050)
	begin
		
<<<<<<< .mine
		delete from [{databaseOwner}].{objectQualifier}EventLog WHERE EventLogID IN (SELECT TOP 100 EventLogID FROM [{databaseOwner}].{objectQualifier}EventLog ORDER BY EventTime)
=======
		delete from {databaseOwner}.yaf_EventLog WHERE EventLogID IN (SELECT TOP 100 EventLogID FROM {databaseOwner}.yaf_EventLog ORDER BY EventTime)
>>>>>>> .r1490
	end	
	
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}eventlog_delete](@EventLogID int) as
=======
create procedure [{databaseOwner}].[yaf_eventlog_delete](@EventLogID int) as
>>>>>>> .r1490
begin
<<<<<<< .mine
	delete from [{databaseOwner}].{objectQualifier}EventLog where EventLogID=@EventLogID
=======
	delete from {databaseOwner}.yaf_EventLog where EventLogID=@EventLogID
>>>>>>> .r1490
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}eventlog_list](@BoardID int) as
=======
create procedure [{databaseOwner}].[yaf_eventlog_list](@BoardID int) as
>>>>>>> .r1490
begin
	select
		a.*,
		ISNULL(b.[Name],'System') as [Name]
	from
<<<<<<< .mine
		[{databaseOwner}].{objectQualifier}EventLog a
		left join [{databaseOwner}].{objectQualifier}User b on b.UserID=a.UserID
=======
		{databaseOwner}.yaf_EventLog a
		left join {databaseOwner}.yaf_User b on b.UserID=a.UserID
>>>>>>> .r1490
	where
		(b.UserID IS NULL or b.BoardID = @BoardID)		
	order by
		a.EventLogID desc
end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}forum_delete](@ForumID int) as
=======
CREATE procedure [{databaseOwner}].[yaf_forum_delete](@ForumID int) as
>>>>>>> .r1490
begin
	-- Maybe an idea to use cascading foreign keys instead? Too bad they don't work on MS SQL 7.0...
	update [{databaseOwner}].{objectQualifier}Forum set LastMessageID=null,LastTopicID=null where ForumID=@ForumID
	update [{databaseOwner}].{objectQualifier}Topic set LastMessageID=null where ForumID=@ForumID
	delete from [{databaseOwner}].{objectQualifier}WatchTopic from [{databaseOwner}].{objectQualifier}Topic where [{databaseOwner}].{objectQualifier}Topic.ForumID = @ForumID and [{databaseOwner}].{objectQualifier}WatchTopic.TopicID = [{databaseOwner}].{objectQualifier}Topic.TopicID
	delete from [{databaseOwner}].{objectQualifier}Active from [{databaseOwner}].{objectQualifier}Topic where [{databaseOwner}].{objectQualifier}Topic.ForumID = @ForumID and [{databaseOwner}].{objectQualifier}Active.TopicID = [{databaseOwner}].{objectQualifier}Topic.TopicID
	delete from [{databaseOwner}].{objectQualifier}NntpTopic from [{databaseOwner}].{objectQualifier}NntpForum where [{databaseOwner}].{objectQualifier}NntpForum.ForumID = @ForumID and [{databaseOwner}].{objectQualifier}NntpTopic.NntpForumID = [{databaseOwner}].{objectQualifier}NntpForum.NntpForumID
	delete from [{databaseOwner}].{objectQualifier}NntpForum where ForumID=@ForumID	
	delete from [{databaseOwner}].{objectQualifier}WatchForum where ForumID = @ForumID

	-- BAI CHANGED 02.02.2004
	-- Delete topics, messages and attachments

	declare @tmpTopicID int;
	declare topic_cursor cursor for
		select TopicID from [{databaseOwner}].{objectQualifier}topic
		where ForumId = @ForumID
		order by TopicID desc
	
	open topic_cursor
	
	fetch next from topic_cursor
	into @tmpTopicID
	
	-- Check @@FETCH_STATUS to see if there are any more rows to fetch.
	while @@FETCH_STATUS = 0
	begin
		exec [{databaseOwner}].{objectQualifier}topic_delete @tmpTopicID,1,1;
	
	   -- This is executed as long as the previous fetch succeeds.
		fetch next from topic_cursor
		into @tmpTopicID
	end
	
	close topic_cursor
	deallocate topic_cursor

	-- TopicDelete finished
	-- END BAI CHANGED 02.02.2004

	delete from [{databaseOwner}].{objectQualifier}ForumAccess where ForumID = @ForumID
	--ABOT CHANGED
	--Delete UserForums Too 
	delete from [{databaseOwner}].{objectQualifier}UserForum where ForumID = @ForumID
	--END ABOT CHANGED 09.04.2004
	delete from [{databaseOwner}].{objectQualifier}Forum where ForumID = @ForumID
end

GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}forum_list](@BoardID int,@ForumID int=null) as
=======
create procedure [{databaseOwner}].[yaf_forum_list](@BoardID int,@ForumID int=null) as
>>>>>>> .r1490
begin
	if @ForumID = 0 set @ForumID = null
	if @ForumID is null
		select a.* from [{databaseOwner}].{objectQualifier}Forum a join [{databaseOwner}].{objectQualifier}Category b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID order by a.SortOrder
	else
		select a.* from [{databaseOwner}].{objectQualifier}Forum a join [{databaseOwner}].{objectQualifier}Category b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID and a.ForumID = @ForumID
end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}forum_listall] (@BoardID int,@UserID int,@root int = 0) as
=======
CREATE procedure [{databaseOwner}].[yaf_forum_listall] (@BoardID int,@UserID int,@root int = 0) as
>>>>>>> .r1490
begin
if @root = 0
begin
    select
        b.CategoryID,
        Category = b.Name,
        a.ForumID,
        Forum = a.Name,
        Indent = 0,
        a.ParentID
    from
        [{databaseOwner}].{objectQualifier}Forum a
        join [{databaseOwner}].{objectQualifier}Category b on b.CategoryID=a.CategoryID
        join [{databaseOwner}].{objectQualifier}vaccess c on c.ForumID=a.ForumID
    where
        c.UserID=@UserID and
        b.BoardID=@BoardID and
        c.ReadAccess>0
    order by
        b.SortOrder,
        a.SortOrder,
        b.categoryid,
        a.forumid
end
else if  @root > 0
begin
    select
        b.CategoryID,
        Category = b.Name,
        a.ForumID,
        Forum = a.Name,
        Indent = 0,
        a.ParentID
    from
        [{databaseOwner}].{objectQualifier}Forum a
        join [{databaseOwner}].{objectQualifier}Category b on b.CategoryID=a.CategoryID
        join [{databaseOwner}].{objectQualifier}vaccess c on c.ForumID=a.ForumID
    where
        c.UserID=@UserID and
        b.BoardID=@BoardID and
        c.ReadAccess>0 and
        a.ForumID = @root

    order by
        b.SortOrder,
        a.SortOrder,
        b.categoryid,
        a.forumid
end
else
begin
    select
        b.CategoryID,
        Category = b.Name,
        a.ForumID,
        Forum = a.Name,
        Indent = 0,
        a.ParentID
    from
        [{databaseOwner}].{objectQualifier}Forum a
        join [{databaseOwner}].{objectQualifier}Category b on b.CategoryID=a.CategoryID
        join [{databaseOwner}].{objectQualifier}vaccess c on c.ForumID=a.ForumID
    where
        c.UserID=@UserID and
        b.BoardID=@BoardID and
        c.ReadAccess>0 and
        b.categoryID = -@root

    order by
        b.SortOrder,
        a.SortOrder,
        b.categoryid,
        a.forumid
end
end
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listall_fromcat](@BoardID int,@CategoryID int) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_forum_listall_fromcat](@BoardID int,@CategoryID int) AS
>>>>>>> .r1490
BEGIN
	SELECT     b.CategoryID, b.Name AS Category, a.ForumID, a.Name AS Forum, a.ParentID
	FROM         [{databaseOwner}].{objectQualifier}Forum a INNER JOIN
						  [{databaseOwner}].{objectQualifier}Category b ON b.CategoryID = a.CategoryID
		WHERE
			b.CategoryID=@CategoryID and
			b.BoardID=@BoardID
		ORDER BY
			b.SortOrder,
			a.SortOrder
END
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}forum_listallmymoderated](@BoardID int,@UserID int) as
=======
create procedure [{databaseOwner}].[yaf_forum_listallmymoderated](@BoardID int,@UserID int) as
>>>>>>> .r1490
begin
	select
		b.CategoryID,
		Category = b.Name,
		a.ForumID,
		Forum = a.Name,
		x.Indent
	from
		(select
			b.ForumID,
			Indent = 0
		from
			{objectQualifier}Category a
			join [{databaseOwner}].{objectQualifier}Forum b on b.CategoryID=a.CategoryID
		where
			a.BoardID=@BoardID and
			b.ParentID is null
	
		union
	
		select
			c.ForumID,
			Indent = 1
		from
			{objectQualifier}Category a
			join [{databaseOwner}].{objectQualifier}Forum b on b.CategoryID=a.CategoryID
			join [{databaseOwner}].{objectQualifier}Forum c on c.ParentID=b.ForumID
		where
			a.BoardID=@BoardID and
			b.ParentID is null
	
		union
	
		select
			d.ForumID,
			Indent = 2
		from
			{objectQualifier}Category a
			join [{databaseOwner}].{objectQualifier}Forum b on b.CategoryID=a.CategoryID
			join [{databaseOwner}].{objectQualifier}Forum c on c.ParentID=b.ForumID
			join [{databaseOwner}].{objectQualifier}Forum d on d.ParentID=c.ForumID
		where
			a.BoardID=@BoardID and
			b.ParentID is null
		) as x
		join [{databaseOwner}].{objectQualifier}Forum a on a.ForumID=x.ForumID
		join [{databaseOwner}].{objectQualifier}Category b on b.CategoryID=a.CategoryID
		join [{databaseOwner}].{objectQualifier}vaccess c on c.ForumID=a.ForumID
	where
		c.UserID=@UserID and
		b.BoardID=@BoardID and
		c.ModeratorAccess>0
	order by
		b.SortOrder,
		a.SortOrder
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}forum_listpath](@ForumID int) as
=======
create procedure [{databaseOwner}].[yaf_forum_listpath](@ForumID int) as
>>>>>>> .r1490
begin
	-- supports up to 4 levels of nested forums
	select
		a.ForumID,
		a.Name
	from
		(select
			a.ForumID,
			Indent = 0
		from
			{objectQualifier}Forum a
		where
			a.ForumID=@ForumID

		union

		select
			b.ForumID,
			Indent = 1
		from
			{objectQualifier}Forum a
			join [{databaseOwner}].{objectQualifier}Forum b on b.ForumID=a.ParentID
		where
			a.ForumID=@ForumID

		union

		select
			c.ForumID,
			Indent = 2
		from
			{objectQualifier}Forum a
			join [{databaseOwner}].{objectQualifier}Forum b on b.ForumID=a.ParentID
			join [{databaseOwner}].{objectQualifier}Forum c on c.ForumID=b.ParentID
		where
			a.ForumID=@ForumID

		union 

		select
			d.ForumID,
			Indent = 3
		from
			{objectQualifier}Forum a
			join [{databaseOwner}].{objectQualifier}Forum b on b.ForumID=a.ParentID
			join [{databaseOwner}].{objectQualifier}Forum c on c.ForumID=b.ParentID
			join [{databaseOwner}].{objectQualifier}Forum d on d.ForumID=c.ParentID
		where
			a.ForumID=@ForumID
		) as x	
		join [{databaseOwner}].{objectQualifier}Forum a on a.ForumID=x.ForumID
	order by
		x.Indent desc
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}forum_listread](@BoardID int,@UserID int,@CategoryID int=null,@ParentID int=null) as
=======
create procedure [{databaseOwner}].[yaf_forum_listread](@BoardID int,@UserID int,@CategoryID int=null,@ParentID int=null) as
>>>>>>> .r1490
begin
	select 
		a.CategoryID, 
		Category		= a.Name, 
		ForumID			= b.ForumID,
		Forum			= b.Name, 
		Description,
<<<<<<< .mine
		Topics			= [{databaseOwner}].{objectQualifier}forum_topics(b.ForumID),
		Posts			= [{databaseOwner}].{objectQualifier}forum_posts(b.ForumID),
		Subforums		= [{databaseOwner}].{objectQualifier}forum_subforums(b.ForumID, @UserID),
=======
		Topics			= {databaseOwner}.yaf_forum_topics(b.ForumID),
		Posts			= {databaseOwner}.yaf_forum_posts(b.ForumID),
		Subforums		= {databaseOwner}.yaf_forum_subforums(b.ForumID, @UserID),
>>>>>>> .r1490
		LastPosted		= t.LastPosted,
		LastMessageID	= t.LastMessageID,
		LastUserID		= t.LastUserID,
		LastUser		= IsNull(t.LastUserName,(select Name from [{databaseOwner}].{objectQualifier}User x where x.UserID=t.LastUserID)),
		LastTopicID		= t.TopicID,
		LastTopicName	= t.Topic,
		b.Flags,
		Viewing			= (select count(1) from [{databaseOwner}].{objectQualifier}Active x where x.ForumID=b.ForumID),
		b.RemoteURL,
		x.ReadAccess
	from 
<<<<<<< .mine
		{objectQualifier}Category a
		join [{databaseOwner}].{objectQualifier}Forum b on b.CategoryID=a.CategoryID
		join [{databaseOwner}].{objectQualifier}vaccess x on x.ForumID=b.ForumID
		left outer join [{databaseOwner}].{objectQualifier}Topic t ON t.TopicID = [{databaseOwner}].{objectQualifier}forum_lasttopic(b.ForumID,@UserID,b.LastTopicID,b.LastPosted)
=======
		yaf_Category a
		join yaf_Forum b on b.CategoryID=a.CategoryID
		join yaf_vaccess x on x.ForumID=b.ForumID
		left outer join yaf_Topic t ON t.TopicID = {databaseOwner}.yaf_forum_lasttopic(b.ForumID,@UserID,b.LastTopicID,b.LastPosted)
>>>>>>> .r1490
	where 
		a.BoardID = @BoardID and
		((b.Flags & 2)=0 or x.ReadAccess<>0) and
		(@CategoryID is null or a.CategoryID=@CategoryID) and
		((@ParentID is null and b.ParentID is null) or b.ParentID=@ParentID) and
		x.UserID = @UserID
	order by
		a.SortOrder,
		b.SortOrder
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}forum_listSubForums](@ForumID int) as
=======
create procedure [{databaseOwner}].[yaf_forum_listSubForums](@ForumID int) as
>>>>>>> .r1490
begin
	select Sum(1) from [{databaseOwner}].{objectQualifier}Forum where ParentID = @ForumID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}forum_listtopics](@ForumID int) as
=======
create procedure [{databaseOwner}].[yaf_forum_listtopics](@ForumID int) as
>>>>>>> .r1490
begin
select * from [{databaseOwner}].{objectQualifier}Topic
Where ForumID = @ForumID
end
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}forum_moderatelist](@BoardID int,@UserID int) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_forum_moderatelist](@BoardID int,@UserID int) AS
>>>>>>> .r1490
BEGIN

SELECT
		b.*,
		MessageCount  = 
		(SELECT     count({objectQualifier}Message.MessageID)
		FROM         [{databaseOwner}].{objectQualifier}Message INNER JOIN
							  [{databaseOwner}].{objectQualifier}Topic ON [{databaseOwner}].{objectQualifier}Message.TopicID = [{databaseOwner}].{objectQualifier}Topic.TopicID
		WHERE (({objectQualifier}Message.Flags & 16)=0) and (({objectQualifier}Message.Flags & 8)=0) and (({objectQualifier}Topic.Flags & 8) = 0) AND ({objectQualifier}Topic.ForumID=b.ForumID)),
		ReportCount	= 
		(SELECT     count({objectQualifier}Message.MessageID)
		FROM         [{databaseOwner}].{objectQualifier}Message INNER JOIN
							  [{databaseOwner}].{objectQualifier}Topic ON [{databaseOwner}].{objectQualifier}Message.TopicID = [{databaseOwner}].{objectQualifier}Topic.TopicID
		WHERE (({objectQualifier}Message.Flags & 128)=128) and (({objectQualifier}Message.Flags & 8)=0) and (({objectQualifier}Topic.Flags & 8) = 0) AND ({objectQualifier}Topic.ForumID=b.ForumID)),
		SpamCount	= 
		(SELECT     count({objectQualifier}Message.MessageID)
		FROM         [{databaseOwner}].{objectQualifier}Message INNER JOIN
							  [{databaseOwner}].{objectQualifier}Topic ON [{databaseOwner}].{objectQualifier}Message.TopicID = [{databaseOwner}].{objectQualifier}Topic.TopicID
		WHERE (({objectQualifier}Message.Flags & 256)=256) and (({objectQualifier}Message.Flags & 8)=0) and (({objectQualifier}Topic.Flags & 8) = 0) AND ({objectQualifier}Topic.ForumID=b.ForumID))
		
	FROM
		{objectQualifier}Category a

	JOIN [{databaseOwner}].{objectQualifier}Forum b ON b.CategoryID=a.CategoryID
	JOIN [{databaseOwner}].{objectQualifier}vaccess c ON c.ForumID=b.ForumID

	WHERE
		a.BoardID=@BoardID AND
		c.ModeratorAccess>0 AND
		c.UserID=@UserID
	ORDER BY
		a.SortOrder,
		b.SortOrder
END
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}forum_moderators] as
=======
create procedure [{databaseOwner}].[yaf_forum_moderators] as
>>>>>>> .r1490
begin
	select
		ForumID = a.ForumID, 
		ModeratorID = a.GroupID, 
		ModeratorName = b.Name,
		IsGroup=1
	from
		{objectQualifier}ForumAccess a
		JOIN [{databaseOwner}].{objectQualifier}Group b ON b.GroupID = a.GroupID
		JOIN [{databaseOwner}].{objectQualifier}AccessMask c ON c.AccessMaskID = a.AccessMaskID
	where
		(b.Flags & 1)=0 and
		(c.Flags & 64)<>0
	union
	select 
		ForumID = x.ForumID, 
		ModeratorID = a.UserID, 
		ModeratorName = a.Name,
		IsGroup=0
	from
		{objectQualifier}User a
		JOIN [{databaseOwner}].{objectQualifier}vmaccess x ON a.UserID=x.UserID
	where
		x.ModeratorAccess<>0
	order by
		IsGroup desc,
		ModeratorName asc
end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}forum_save](
=======
CREATE procedure [{databaseOwner}].[yaf_forum_save](
>>>>>>> .r1490
	@ForumID 		int,
	@CategoryID		int,
	@ParentID		int=null,
	@Name			nvarchar(50),
	@Description	nvarchar(255),
	@SortOrder		smallint,
	@Locked			bit,
	@Hidden			bit,
	@IsTest			bit,
	@Moderated		bit,
	@RemoteURL		nvarchar(100)=null,
	@ThemeURL		nvarchar(100)=null,
	@AccessMaskID	int = null
) as
begin
	declare @BoardID	int
	declare @Flags		int
	
	set @Flags = 0
	if @Locked<>0 set @Flags = @Flags | 1
	if @Hidden<>0 set @Flags = @Flags | 2
	if @IsTest<>0 set @Flags = @Flags | 4
	if @Moderated<>0 set @Flags = @Flags | 8

	if @ForumID>0 begin
		update [{databaseOwner}].{objectQualifier}Forum set 
			ParentID=@ParentID,
			Name=@Name,
			Description=@Description,
			SortOrder=@SortOrder,
			CategoryID=@CategoryID,
			RemoteURL = @RemoteURL,
			ThemeURL = @ThemeURL,
			Flags = @Flags
		where ForumID=@ForumID
	end
	else begin
		select @BoardID=BoardID from [{databaseOwner}].{objectQualifier}Category where CategoryID=@CategoryID
	
		insert into [{databaseOwner}].{objectQualifier}Forum(ParentID,Name,Description,SortOrder,CategoryID,NumTopics,NumPosts,RemoteURL,ThemeURL,Flags)
		values(@ParentID,@Name,@Description,@SortOrder,@CategoryID,0,0,@RemoteURL,@ThemeURL,@Flags)
		select @ForumID = SCOPE_IDENTITY()

		insert into [{databaseOwner}].{objectQualifier}ForumAccess(GroupID,ForumID,AccessMaskID) 
		select GroupID,@ForumID,@AccessMaskID
		from [{databaseOwner}].{objectQualifier}Group 
		where BoardID=@BoardID
	end
	select ForumID = @ForumID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}forum_updatelastpost](@ForumID int) as
=======
create procedure [{databaseOwner}].[yaf_forum_updatelastpost](@ForumID int) as
>>>>>>> .r1490
begin
	update [{databaseOwner}].{objectQualifier}Forum set
		LastPosted = (select top 1 y.Posted from [{databaseOwner}].{objectQualifier}Topic x,{objectQualifier}Message y where x.ForumID={objectQualifier}Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16 order by y.Posted desc),
		LastTopicID = (select top 1 y.TopicID from [{databaseOwner}].{objectQualifier}Topic x,{objectQualifier}Message y where x.ForumID={objectQualifier}Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16 order by y.Posted desc),
		LastMessageID = (select top 1 y.MessageID from [{databaseOwner}].{objectQualifier}Topic x,{objectQualifier}Message y where x.ForumID={objectQualifier}Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16 order by y.Posted desc),
		LastUserID = (select top 1 y.UserID from [{databaseOwner}].{objectQualifier}Topic x,{objectQualifier}Message y where x.ForumID={objectQualifier}Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16 order by y.Posted desc),
		LastUserName = (select top 1 y.UserName from [{databaseOwner}].{objectQualifier}Topic x,{objectQualifier}Message y where x.ForumID={objectQualifier}Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16 order by y.Posted desc)
	where ForumID = @ForumID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}forum_updatestats](@ForumID int) as
=======
create procedure [{databaseOwner}].[yaf_forum_updatestats](@ForumID int) as
>>>>>>> .r1490
begin
	update [{databaseOwner}].{objectQualifier}Forum set 
		NumPosts = (select count(1) from [{databaseOwner}].{objectQualifier}Message x,{objectQualifier}Topic y where y.TopicID=x.TopicID and y.ForumID = [{databaseOwner}].{objectQualifier}Forum.ForumID and (x.Flags & 24)=16),
		NumTopics = (select count(distinct x.TopicID) from [{databaseOwner}].{objectQualifier}Topic x,{objectQualifier}Message y where x.ForumID={objectQualifier}Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16)
	where ForumID=@ForumID
end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}forumaccess_group](@GroupID int) as
=======
CREATE procedure [{databaseOwner}].[yaf_forumaccess_group](@GroupID int) as
>>>>>>> .r1490
begin
	select 
		a.*,
		ForumName = b.Name,
		CategoryName = c.Name ,
		CategoryId = b.CategoryID,
		ParentID = b.ParentID 
	from 
		{objectQualifier}ForumAccess a, 
		{objectQualifier}Forum b, 
		{objectQualifier}Category c 
	where 
		a.GroupID = @GroupID and 
		b.ForumID=a.ForumID and 
		c.CategoryID=b.CategoryID 
	order by 
		c.SortOrder,
		b.SortOrder
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}forumaccess_list](@ForumID int) as
=======
create procedure [{databaseOwner}].[yaf_forumaccess_list](@ForumID int) as
>>>>>>> .r1490
begin
	select 
		a.*,
		GroupName=b.Name 
	from 
		{objectQualifier}ForumAccess a, 
		{objectQualifier}Group b 
	where 
		a.ForumID = @ForumID and 
		b.GroupID = a.GroupID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}forumaccess_save](
=======
create procedure [{databaseOwner}].[yaf_forumaccess_save](
>>>>>>> .r1490
	@ForumID			int,
	@GroupID			int,
	@AccessMaskID		int
) as
begin
	update [{databaseOwner}].{objectQualifier}ForumAccess set 
		AccessMaskID=@AccessMaskID
	where 
		ForumID = @ForumID and 
		GroupID = @GroupID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}group_delete](@GroupID int) as
=======
create procedure [{databaseOwner}].[yaf_group_delete](@GroupID int) as
>>>>>>> .r1490
begin
	delete from [{databaseOwner}].{objectQualifier}ForumAccess where GroupID = @GroupID
	delete from [{databaseOwner}].{objectQualifier}UserGroup where GroupID = @GroupID
	delete from [{databaseOwner}].{objectQualifier}Group where GroupID = @GroupID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}group_list](@BoardID int,@GroupID int=null) as
=======
create procedure [{databaseOwner}].[yaf_group_list](@BoardID int,@GroupID int=null) as
>>>>>>> .r1490
begin
	if @GroupID is null
		select * from [{databaseOwner}].{objectQualifier}Group where BoardID=@BoardID
	else
		select * from [{databaseOwner}].{objectQualifier}Group where BoardID=@BoardID and GroupID=@GroupID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}group_member](@BoardID int,@UserID int) as
=======
create procedure [{databaseOwner}].[yaf_group_member](@BoardID int,@UserID int) as
>>>>>>> .r1490
begin
	select 
		a.GroupID,
		a.Name,
		Member = (select count(1) from [{databaseOwner}].{objectQualifier}UserGroup x where x.UserID=@UserID and x.GroupID=a.GroupID)
	from
		{objectQualifier}Group a
	where
		a.BoardID=@BoardID
	order by
		a.Name
end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}group_save](
=======
CREATE procedure [{databaseOwner}].[yaf_group_save](
>>>>>>> .r1490
	@GroupID		int,
	@BoardID		int,
	@Name			nvarchar(50),
	@IsAdmin		bit,
	@IsStart		bit,
	@IsModerator	bit,
	@IsGuest		bit,
	@AccessMaskID	int=null
) as
begin
	declare @Flags	int
	
	set @Flags = 0
	if @IsAdmin<>0 set @Flags = @Flags | 1
	if @IsGuest<>0 set @Flags = @Flags | 2
	if @IsStart<>0 set @Flags = @Flags | 4
	if @IsModerator<>0 set @Flags = @Flags | 8

	if @GroupID>0 begin
		update [{databaseOwner}].{objectQualifier}Group set
			Name = @Name,
			Flags = @Flags
		where GroupID = @GroupID
	end
	else begin
		insert into [{databaseOwner}].{objectQualifier}Group(Name,BoardID,Flags)
		values(@Name,@BoardID,@Flags);
		set @GroupID = SCOPE_IDENTITY()
		insert into [{databaseOwner}].{objectQualifier}ForumAccess(GroupID,ForumID,AccessMaskID)
		select @GroupID,a.ForumID,@AccessMaskID from [{databaseOwner}].{objectQualifier}Forum a join [{databaseOwner}].{objectQualifier}Category b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID
	end
	select GroupID = @GroupID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}mail_create](@From nvarchar(50),@To nvarchar(50),@Subject nvarchar(100),@Body ntext) as 
=======
create procedure [{databaseOwner}].[yaf_mail_create](@From nvarchar(50),@To nvarchar(50),@Subject nvarchar(100),@Body ntext) as 
>>>>>>> .r1490
begin
	insert into [{databaseOwner}].{objectQualifier}Mail(FromUser,ToUser,Created,Subject,Body)
	values(@From,@To,getdate(),@Subject,@Body)
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}mail_createwatch](@TopicID int,@From nvarchar(50),@Subject nvarchar(100),@Body ntext,@UserID int) as begin
	insert into [{databaseOwner}].{objectQualifier}Mail(FromUser,ToUser,Created,Subject,Body)
=======
create procedure [{databaseOwner}].[yaf_mail_createwatch](@TopicID int,@From nvarchar(50),@Subject nvarchar(100),@Body ntext,@UserID int) as begin
	insert into yaf_Mail(FromUser,ToUser,Created,Subject,Body)
>>>>>>> .r1490
	select
		@From,
		b.Email,
		getdate(),
		@Subject,
		@Body
	from
		{objectQualifier}WatchTopic a,
		{objectQualifier}User b
	where
		b.UserID <> @UserID and
		b.UserID = a.UserID and
		a.TopicID = @TopicID and
		(a.LastMail is null or a.LastMail < b.LastVisit)
	
	insert into [{databaseOwner}].{objectQualifier}Mail(FromUser,ToUser,Created,Subject,Body)
	select
		@From,
		b.Email,
		getdate(),
		@Subject,
		@Body
	from
		{objectQualifier}WatchForum a,
		{objectQualifier}User b,
		{objectQualifier}Topic c
	where
		b.UserID <> @UserID and
		b.UserID = a.UserID and
		c.TopicID = @TopicID and
		c.ForumID = a.ForumID and
		(a.LastMail is null or a.LastMail < b.LastVisit) and
		not exists(select 1 from [{databaseOwner}].{objectQualifier}WatchTopic x where x.UserID=b.UserID and x.TopicID=c.TopicID)

	update [{databaseOwner}].{objectQualifier}WatchTopic set LastMail = getdate() 
	where TopicID = @TopicID
	and UserID <> @UserID
	
	update [{databaseOwner}].{objectQualifier}WatchForum set LastMail = getdate() 
	where ForumID = (select ForumID from [{databaseOwner}].{objectQualifier}Topic where TopicID = @TopicID)
	and UserID <> @UserID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}mail_delete](@MailID int) as
=======
create procedure [{databaseOwner}].[yaf_mail_delete](@MailID int) as
>>>>>>> .r1490
begin
	delete from [{databaseOwner}].{objectQualifier}Mail where MailID = @MailID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}mail_list] as
=======
create procedure [{databaseOwner}].[yaf_mail_list] as
>>>>>>> .r1490
begin
	select top 10 * from [{databaseOwner}].{objectQualifier}Mail order by Created
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}message_approve](@MessageID int) as begin
=======
create procedure [{databaseOwner}].[yaf_message_approve](@MessageID int) as begin
>>>>>>> .r1490
	declare	@UserID		int
	declare	@ForumID	int
	declare	@TopicID	int
	declare @Posted		datetime
	declare	@UserName	nvarchar(50)

	select 
		@UserID = a.UserID,
		@TopicID = a.TopicID,
		@ForumID = b.ForumID,
		@Posted = a.Posted,
		@UserName = a.UserName
	from
		{objectQualifier}Message a,
		{objectQualifier}Topic b
	where
		a.MessageID = @MessageID and
		b.TopicID = a.TopicID

	-- update [{databaseOwner}].{objectQualifier}Message
	update [{databaseOwner}].{objectQualifier}Message set Flags = Flags | 16 where MessageID = @MessageID

	-- update [{databaseOwner}].{objectQualifier}User
	if exists(select 1 from [{databaseOwner}].{objectQualifier}Forum where ForumID=@ForumID and (Flags & 4)=0)
	begin
		update [{databaseOwner}].{objectQualifier}User set NumPosts = NumPosts + 1 where UserID = @UserID
		exec [{databaseOwner}].{objectQualifier}user_upgrade @UserID
	end

	-- update [{databaseOwner}].{objectQualifier}Forum
	update [{databaseOwner}].{objectQualifier}Forum set
		LastPosted = @Posted,
		LastTopicID = @TopicID,
		LastMessageID = @MessageID,
		LastUserID = @UserID,
		LastUserName = @UserName
	where ForumID = @ForumID

	-- update [{databaseOwner}].{objectQualifier}Topic
	update [{databaseOwner}].{objectQualifier}Topic set
		LastPosted = @Posted,
		LastMessageID = @MessageID,
		LastUserID = @UserID,
		LastUserName = @UserName,
		NumPosts = (select count(1) from [{databaseOwner}].{objectQualifier}Message x where x.TopicID={objectQualifier}Topic.TopicID and (x.Flags & 24)=16)
	where TopicID = @TopicID
	
	-- update forum stats
	exec [{databaseOwner}].{objectQualifier}forum_updatestats @ForumID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}message_delete](@MessageID int) as
=======
create procedure [{databaseOwner}].[yaf_message_delete](@MessageID int) as
>>>>>>> .r1490
begin
	declare @TopicID		int
	declare @ForumID		int
	declare @MessageCount	int
	declare @LastMessageID	int

	-- Find TopicID and ForumID
	select @TopicID=b.TopicID,@ForumID=b.ForumID from [{databaseOwner}].{objectQualifier}Message a,{objectQualifier}Topic b where a.MessageID=@MessageID and b.TopicID=a.TopicID

	-- Update LastMessageID in Topic and Forum
	update [{databaseOwner}].{objectQualifier}Topic set 
		LastPosted = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	update [{databaseOwner}].{objectQualifier}Forum set 
		LastPosted = null,
		LastTopicID = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	-- "Delete" message
	update [{databaseOwner}].{objectQualifier}Message set Flags = Flags | 8 where MessageID = @MessageID
	
	-- Delete topic if there are no more messages
	select @MessageCount = count(1) from [{databaseOwner}].{objectQualifier}Message where TopicID = @TopicID and (Flags & 8)=0
	if @MessageCount=0 exec [{databaseOwner}].{objectQualifier}topic_delete @TopicID
	-- update lastpost
	exec [{databaseOwner}].{objectQualifier}topic_updatelastpost @ForumID,@TopicID
	exec [{databaseOwner}].{objectQualifier}forum_updatestats @ForumID
	-- update topic numposts
	update [{databaseOwner}].{objectQualifier}Topic set
		NumPosts = (select count(1) from [{databaseOwner}].{objectQualifier}Message x where x.TopicID={objectQualifier}Topic.TopicID and (x.Flags & 24)=16)
	where TopicID = @TopicID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}message_findunread](@TopicID int,@LastRead datetime) as
=======
create procedure [{databaseOwner}].[yaf_message_findunread](@TopicID int,@LastRead datetime) as
>>>>>>> .r1490
begin
	select top 1 MessageID from [{databaseOwner}].{objectQualifier}Message
	where TopicID=@TopicID and Posted>@LastRead
	order by Posted
end
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_getReplies](@MessageID int) as
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_message_getReplies](@MessageID int) as
>>>>>>> .r1490
BEGIN
	SELECT MessageID FROM [{databaseOwner}].{objectQualifier}Message WHERE ReplyTo = @MessageID
END
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_list](@MessageID int) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_message_list](@MessageID int) AS
>>>>>>> .r1490
BEGIN
	SELECT
		a.MessageID,
		a.UserID,
		UserName = b.Name,
		a.Message,
		c.TopicID,
		c.ForumID,
		c.Topic,
		c.Priority,
		a.Flags,
		c.UserID AS TopicOwnerID,
		Edited = IsNull(a.Edited,a.Posted),
		TopicFlags = c.Flags,
		ForumFlags = d.Flags,
		a.EditReason,
		a.Position,
		a.IsModeratorChanged,
		a.DeleteReason,
		a.BlogPostID,
		c.PollID,
        a.IP
	FROM
		{objectQualifier}Message a,
		{objectQualifier}User b,
		{objectQualifier}Topic c,
		{objectQualifier}Forum d
	WHERE
		a.MessageID = @MessageID AND
		b.UserID = a.UserID AND
		c.TopicID = a.TopicID AND
		c.ForumID = d.ForumID
END
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_listreported](@MessageFlag int, @ForumID int) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_message_listreported](@MessageFlag int, @ForumID int) AS
>>>>>>> .r1490
BEGIN
	SELECT
		a.*,
		OriginalMessage = b.Message,
		UserName	= IsNull(b.UserName,d.Name),
		UserID = b.UserID,
		Posted		= b.Posted,
		Topic		= c.Topic,
		NumberOfReports = (SELECT count(LogID) FROM [{databaseOwner}].{objectQualifier}MessageReportedAudit WHERE [{databaseOwner}].{objectQualifier}MessageReportedAudit.MessageID = a.MessageID)
	FROM
		{objectQualifier}MessageReported a
	INNER JOIN
		{objectQualifier}Message b ON a.MessageID = b.MessageID
	INNER JOIN
		{objectQualifier}Topic c ON b.TopicID = c.TopicID
	INNER JOIN
		{objectQualifier}User d ON b.UserID = d.UserID
	WHERE
		c.ForumID = @ForumID and
		(c.Flags & 16)=0 and
		(b.Flags & 8)=0 and
		(c.Flags & 8)=0 and
		(b.Flags & POWER(2,@MessageFlag))=POWER(2,@MessageFlag)
	ORDER BY
		b.TopicID DESC, b.Posted DESC
END
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_report](@ReportFlag int, @MessageID int, @ReporterID int, @ReportedDate datetime ) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_message_report](@ReportFlag int, @MessageID int, @ReporterID int, @ReportedDate datetime ) AS
>>>>>>> .r1490
BEGIN
	
	IF NOT exists(SELECT MessageID from [{databaseOwner}].{objectQualifier}MessageReportedAudit WHERE MessageID=@MessageID AND UserID=@ReporterID)
		INSERT INTO [{databaseOwner}].{objectQualifier}MessageReportedAudit(MessageID,UserID,Reported) VALUES (@MessageID,@ReporterID,@ReportedDate)

	IF NOT exists(SELECT MessageID FROM [{databaseOwner}].{objectQualifier}MessageReported WHERE MessageID=@MessageID)
	BEGIN
		INSERT INTO [{databaseOwner}].{objectQualifier}MessageReported(MessageID, [Message])
		SELECT 
			a.MessageID,
			a.Message
		FROM
			{objectQualifier}Message a
		WHERE
			a.MessageID = @MessageID
	END

	-- update [{databaseOwner}].{objectQualifier}Message
	UPDATE [{databaseOwner}].{objectQualifier}Message SET Flags = Flags | POWER(2, @ReportFlag) WHERE MessageID = @MessageID

END
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_reportresolve](@MessageFlag int, @MessageID int, @UserID int) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_message_reportresolve](@MessageFlag int, @MessageID int, @UserID int) AS
>>>>>>> .r1490
BEGIN
	UPDATE [{databaseOwner}].{objectQualifier}MessageReported 
	SET Resolved = 1, ResolvedBy = @UserID, ResolvedDate = GETDATE()
	WHERE MessageID = @MessageID;
	
	/* Remove Flag */
	UPDATE [{databaseOwner}].{objectQualifier}Message
	SET Flags = Flags & (~POWER(2, @MessageFlag))
	WHERE MessageID = @MessageID;
END
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_reportcopyover](@MessageID int) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_message_reportcopyover](@MessageID int) AS
>>>>>>> .r1490
BEGIN
	UPDATE [{databaseOwner}].{objectQualifier}MessageReported 
	SET [{databaseOwner}].{objectQualifier}MessageReported.Message = (SELECT Message FROM [{databaseOwner}].{objectQualifier}Message WHERE [{databaseOwner}].{objectQualifier}Message.MessageID=@MessageID)
	WHERE MessageID = @MessageID;

END
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}message_save](
=======
CREATE procedure [{databaseOwner}].[yaf_message_save](
>>>>>>> .r1490
	@TopicID		int,
	@UserID			int,
	@Message		ntext,
	@UserName		nvarchar(50)=null,
	@IP				nvarchar(15),
	@Posted			datetime=null,
	@ReplyTo		int,
	@BlogPostID		nvarchar(50) = null,
	@Flags			int,
	@MessageID		int output
)
AS
BEGIN
	DECLARE @ForumID INT, @ForumFlags INT, @Position INT, @Indent INT

	IF @Posted IS NULL
		SET @Posted = GETDATE()

	SELECT @ForumID = x.ForumID, @ForumFlags = y.Flags
	FROM [{databaseOwner}].{objectQualifier}Topic x,{objectQualifier}Forum y
	WHERE x.TopicID = @TopicID AND y.ForumID=x.ForumID

	IF @ReplyTo IS NULL
			SELECT @Position = 0, @Indent = 0 -- New thread

	ELSE IF @ReplyTo<0
		-- Find post to reply to AND indent of this post
		SELECT TOP 1 @ReplyTo = MessageID, @Indent = Indent+1
		FROM [{databaseOwner}].{objectQualifier}Message
		WHERE TopicID = @TopicID AND ReplyTo IS NULL
		ORDER BY Posted

	ELSE
		-- Got reply, find indent of this post
			SELECT @Indent=Indent+1
			FROM [{databaseOwner}].{objectQualifier}Message
			WHERE MessageID=@ReplyTo

	-- Find position
	IF @ReplyTo IS NOT NULL
    BEGIN
        DECLARE @temp INT
		
        SELECT @temp=ReplyTo,@Position=Position FROM [{databaseOwner}].{objectQualifier}Message WHERE MessageID=@ReplyTo

        IF @temp IS NULL
			-- We are replying to first post
            SELECT @Position=MAX(Position)+1 FROM [{databaseOwner}].{objectQualifier}Message WHERE TopicID=@TopicID

        ELSE
			-- Last position of replies to parent post
            SELECT @Position=MIN(Position) FROM [{databaseOwner}].{objectQualifier}Message WHERE ReplyTo=@temp AND Position>@Position

        -- No replies, THEN USE parent post's position+1
        IF @Position IS NULL
            SELECT @Position=Position+1 FROM [{databaseOwner}].{objectQualifier}Message WHERE MessageID=@ReplyTo
		-- Increase position of posts after this

        UPDATE [{databaseOwner}].{objectQualifier}Message SET Position=Position+1 WHERE TopicID=@TopicID AND Position>=@Position
    END

	-- Add points to Users total points
	UPDATE [{databaseOwner}].{objectQualifier}User SET Points = Points + 3 WHERE UserID = @UserID

	INSERT [{databaseOwner}].{objectQualifier}Message ( UserID, Message, TopicID, Posted, UserName, IP, ReplyTo, Position, Indent, Flags, BlogPostID)
	VALUES ( @UserID, @Message, @TopicID, @Posted, @UserName, @IP, @ReplyTo, @Position, @Indent, @Flags & ~16, @BlogPostID)

	SET @MessageID = SCOPE_IDENTITY()

	IF ((@ForumFlags & 8) = 0) OR ((@Flags & 16) = 16)
		EXEC [{databaseOwner}].{objectQualifier}message_approve @MessageID
END
	
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}message_unapproved](@ForumID int) as begin
=======
CREATE procedure [{databaseOwner}].[yaf_message_unapproved](@ForumID int) as begin
>>>>>>> .r1490
	select
		MessageID	= b.MessageID,
		UserName	= IsNull(b.UserName,c.Name),
		Posted		= b.Posted,
		Topic		= a.Topic,
		Message		= b.Message
	from
		{objectQualifier}Topic a,
		{objectQualifier}Message b,
		{objectQualifier}User c
	where
		a.ForumID = @ForumID and
		b.TopicID = a.TopicID and
		(b.Flags & 16)=0 and
		(a.Flags & 8)=0 and
		(b.Flags & 8)=0 and
		c.UserID = b.UserID
	order by
		a.Posted
end

GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}message_update](@MessageID int,@Priority int,@Subject nvarchar(100),@Flags int, @Message ntext, @Reason as nvarchar(100), @IsModeratorChanged bit) as
=======
CREATE procedure [{databaseOwner}].[yaf_message_update](@MessageID int,@Priority int,@Subject nvarchar(100),@Flags int, @Message ntext, @Reason as nvarchar(100), @IsModeratorChanged bit) as
>>>>>>> .r1490
begin
	declare @TopicID	int
	declare	@ForumFlags	int

	set @Flags = @Flags & ~16	
	
	select 
		@TopicID	= a.TopicID,
		@ForumFlags	= c.Flags
	from 
		{objectQualifier}Message a,
		{objectQualifier}Topic b,
		{objectQualifier}Forum c
	where 
		a.MessageID = @MessageID and
		b.TopicID = a.TopicID and
		c.ForumID = b.ForumID

	if (@ForumFlags & 8)=0 set @Flags = @Flags | 16

	update [{databaseOwner}].{objectQualifier}Message set
		Message = @Message,
		Edited = getdate(),
		Flags = @Flags,
		IsModeratorChanged  = @IsModeratorChanged,
                EditReason = @Reason
	where
		MessageID = @MessageID

	if @Priority is not null begin
		update [{databaseOwner}].{objectQualifier}Topic set
			Priority = @Priority
		where
			TopicID = @TopicID
	end

	if not @Subject = '' and @Subject is not null begin
		update [{databaseOwner}].{objectQualifier}Topic set
			Topic = @Subject
		where
			TopicID = @TopicID
	end 
	
	-- If forum is moderated, make sure last post pointers are correct
	if (@ForumFlags & 8)<>0 exec [{databaseOwner}].{objectQualifier}topic_updatelastpost
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}nntpforum_delete](@NntpForumID int) as
=======
create procedure [{databaseOwner}].[yaf_nntpforum_delete](@NntpForumID int) as
>>>>>>> .r1490
begin
	delete from [{databaseOwner}].{objectQualifier}NntpTopic where NntpForumID = @NntpForumID
	delete from [{databaseOwner}].{objectQualifier}NntpForum where NntpForumID = @NntpForumID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}nntpforum_list](@BoardID int,@Minutes int=null,@NntpForumID int=null,@Active bit=null) as
=======
create procedure [{databaseOwner}].[yaf_nntpforum_list](@BoardID int,@Minutes int=null,@NntpForumID int=null,@Active bit=null) as
>>>>>>> .r1490
begin
	select
		a.Name,
		a.Address,
		Port = IsNull(a.Port,119),
		a.UserName,
		a.UserPass,		
		a.NntpServerID,
		b.NntpForumID,		
		b.GroupName,
		b.ForumID,
		b.LastMessageNo,
		b.LastUpdate,
		b.Active,
		ForumName = c.Name
	from
		{objectQualifier}NntpServer a
		join [{databaseOwner}].{objectQualifier}NntpForum b on b.NntpServerID = a.NntpServerID
		join [{databaseOwner}].{objectQualifier}Forum c on c.ForumID = b.ForumID
	where
		(@Minutes is null or datediff(n,b.LastUpdate,getdate())>@Minutes) and
		(@NntpForumID is null or b.NntpForumID=@NntpForumID) and
		a.BoardID=@BoardID and
		(@Active is null or b.Active=@Active)
	order by
		a.Name,
		b.GroupName
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}nntpforum_save](@NntpForumID int=null,@NntpServerID int,@GroupName nvarchar(100),@ForumID int,@Active bit) as
=======
create procedure [{databaseOwner}].[yaf_nntpforum_save](@NntpForumID int=null,@NntpServerID int,@GroupName nvarchar(100),@ForumID int,@Active bit) as
>>>>>>> .r1490
begin
	if @NntpForumID is null
		insert into [{databaseOwner}].{objectQualifier}NntpForum(NntpServerID,GroupName,ForumID,LastMessageNo,LastUpdate,Active)
		values(@NntpServerID,@GroupName,@ForumID,0,getdate(),@Active)
	else
		update [{databaseOwner}].{objectQualifier}NntpForum set
			NntpServerID = @NntpServerID,
			GroupName = @GroupName,
			ForumID = @ForumID,
			Active = @Active
		where NntpForumID = @NntpForumID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}nntpforum_update](@NntpForumID int,@LastMessageNo int,@UserID int) as
=======
create procedure [{databaseOwner}].[yaf_nntpforum_update](@NntpForumID int,@LastMessageNo int,@UserID int) as
>>>>>>> .r1490
begin
	declare	@ForumID	int
	
	select @ForumID=ForumID from [{databaseOwner}].{objectQualifier}NntpForum where NntpForumID=@NntpForumID

	update [{databaseOwner}].{objectQualifier}NntpForum set
		LastMessageNo = @LastMessageNo,
		LastUpdate = getdate()
	where NntpForumID = @NntpForumID

	update [{databaseOwner}].{objectQualifier}Topic set 
		NumPosts = (select count(1) from [{databaseOwner}].{objectQualifier}message x where x.TopicID={objectQualifier}Topic.TopicID and (x.Flags & 24)=16)
	where ForumID=@ForumID

	--exec [{databaseOwner}].{objectQualifier}user_upgrade @UserID
	exec [{databaseOwner}].{objectQualifier}forum_updatestats @ForumID
	-- exec [{databaseOwner}].{objectQualifier}topic_updatelastpost @ForumID,null
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}nntpserver_delete](@NntpServerID int) as
=======
create procedure [{databaseOwner}].[yaf_nntpserver_delete](@NntpServerID int) as
>>>>>>> .r1490
begin
	delete from [{databaseOwner}].{objectQualifier}NntpTopic where NntpForumID in (select NntpForumID from [{databaseOwner}].{objectQualifier}NntpForum where NntpServerID = @NntpServerID)
	delete from [{databaseOwner}].{objectQualifier}NntpForum where NntpServerID = @NntpServerID
	delete from [{databaseOwner}].{objectQualifier}NntpServer where NntpServerID = @NntpServerID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}nntpserver_list](@BoardID int=null,@NntpServerID int=null) as
=======
create procedure [{databaseOwner}].[yaf_nntpserver_list](@BoardID int=null,@NntpServerID int=null) as
>>>>>>> .r1490
begin
	if @NntpServerID is null
		select * from [{databaseOwner}].{objectQualifier}NntpServer where BoardID=@BoardID order by Name
	else
		select * from [{databaseOwner}].{objectQualifier}NntpServer where NntpServerID=@NntpServerID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}nntpserver_save](
=======
create procedure [{databaseOwner}].[yaf_nntpserver_save](
>>>>>>> .r1490
	@NntpServerID 	int=null,
	@BoardID	int,
	@Name		nvarchar(50),
	@Address	nvarchar(100),
	@Port		int,
	@UserName	nvarchar(50)=null,
	@UserPass	nvarchar(50)=null
) as begin
	if @NntpServerID is null
		insert into [{databaseOwner}].{objectQualifier}NntpServer(Name,BoardID,Address,Port,UserName,UserPass)
		values(@Name,@BoardID,@Address,@Port,@UserName,@UserPass)
	else
		update [{databaseOwner}].{objectQualifier}NntpServer set
			Name = @Name,
			Address = @Address,
			Port = @Port,
			UserName = @UserName,
			UserPass = @UserPass
		where NntpServerID = @NntpServerID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}nntptopic_list](@Thread char(32)) as
=======
create procedure [{databaseOwner}].[yaf_nntptopic_list](@Thread char(32)) as
>>>>>>> .r1490
begin
	select
		a.*
	from
		{objectQualifier}NntpTopic a
	where
		a.Thread = @Thread
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}nntptopic_savemessage](
=======
create procedure [{databaseOwner}].[yaf_nntptopic_savemessage](
>>>>>>> .r1490
	@NntpForumID	int,
	@Topic 			nvarchar(100),
	@Body 			ntext,
	@UserID 		int,
	@UserName		nvarchar(50),
	@IP				nvarchar(15),
	@Posted			datetime,
	@Thread			char(32)
) as 
begin
	declare	@ForumID	int
	declare @TopicID	int
	declare	@MessageID	int

	select @ForumID=ForumID from [{databaseOwner}].{objectQualifier}NntpForum where NntpForumID=@NntpForumID

	if exists(select 1 from [{databaseOwner}].{objectQualifier}NntpTopic where Thread=@Thread)
	begin
		-- thread exists
		select @TopicID=TopicID from [{databaseOwner}].{objectQualifier}NntpTopic where Thread=@Thread
	end else
	begin
		-- thread doesn't exists
		insert into [{databaseOwner}].{objectQualifier}Topic(ForumID,UserID,UserName,Posted,Topic,Views,Priority,NumPosts)
		values(@ForumID,@UserID,@UserName,@Posted,@Topic,0,0,0)
		set @TopicID=SCOPE_IDENTITY()

		insert into [{databaseOwner}].{objectQualifier}NntpTopic(NntpForumID,Thread,TopicID)
		values(@NntpForumID,@Thread,@TopicID)
	end

	-- save message
	insert into [{databaseOwner}].{objectQualifier}Message(TopicID,UserID,UserName,Posted,Message,IP,Position,Indent)
	values(@TopicID,@UserID,@UserName,@Posted,@Body,@IP,0,0)
	set @MessageID=SCOPE_IDENTITY()

	-- update user
	if exists(select 1 from [{databaseOwner}].{objectQualifier}Forum where ForumID=@ForumID and (Flags & 4)=0)
	begin
		update [{databaseOwner}].{objectQualifier}User set NumPosts=NumPosts+1 where UserID=@UserID
	end
	
	-- update topic
	update [{databaseOwner}].{objectQualifier}Topic set 
		LastPosted		= @Posted,
		LastMessageID	= @MessageID,
		LastUserID		= @UserID,
		LastUserName	= @UserName
	where TopicID=@TopicID	
	-- update forum
	update [{databaseOwner}].{objectQualifier}Forum set
		LastPosted		= @Posted,
		LastTopicID	= @TopicID,
		LastMessageID	= @MessageID,
		LastUserID		= @UserID,
		LastUserName	= @UserName
	where ForumID=@ForumID and (LastPosted is null or LastPosted<@Posted)
end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}pageload](
=======
CREATE procedure [{databaseOwner}].[yaf_pageload](
>>>>>>> .r1490
	@SessionID	nvarchar(24),
	@BoardID	int,
	@UserKey	uniqueidentifier,
	@IP			nvarchar(15),
	@Location	nvarchar(50),
	@Browser	nvarchar(50),
	@Platform	nvarchar(50),
	@CategoryID	int = null,
	@ForumID	int = null,
	@TopicID	int = null,
	@MessageID	int = null
) as
begin
	declare @UserID			int
	declare @UserBoardID	int
	declare @IsGuest		tinyint
	declare @rowcount		int
	declare @PreviousVisit	datetime
	
	set implicit_transactions off

	if @UserKey is null
	begin
		select @UserID = UserID from [{databaseOwner}].{objectQualifier}User where BoardID=@BoardID and (Flags & 4)<>0
		set @rowcount=@@rowcount
		if @rowcount<>1
		begin
			raiserror('Found %d possible guest users. There should be one and only one user marked as guest.',16,1,@rowcount)
		end
		set @IsGuest = 1
		set @UserBoardID = @BoardID
	end else
	begin
		select @UserID = UserID, @UserBoardID = BoardID from [{databaseOwner}].{objectQualifier}User where BoardID=@BoardID and ProviderUserKey=@UserKey
		set @IsGuest = 0
	end
	-- Check valid ForumID
	if @ForumID is not null and not exists(select 1 from [{databaseOwner}].{objectQualifier}Forum where ForumID=@ForumID) begin
		set @ForumID = null
	end
	-- Check valid CategoryID
	if @CategoryID is not null and not exists(select 1 from [{databaseOwner}].{objectQualifier}Category where CategoryID=@CategoryID) begin
		set @CategoryID = null
	end
	-- Check valid MessageID
	if @MessageID is not null and not exists(select 1 from [{databaseOwner}].{objectQualifier}Message where MessageID=@MessageID) begin
		set @MessageID = null
	end
	-- Check valid TopicID
	if @TopicID is not null and not exists(select 1 from [{databaseOwner}].{objectQualifier}Topic where TopicID=@TopicID) begin
		set @TopicID = null
	end
	
	-- get previous visit
	if @IsGuest=0 begin
<<<<<<< .mine
		select @PreviousVisit = LastVisit from [{databaseOwner}].{objectQualifier}User where UserID = @UserID
=======
		select @PreviousVisit = LastVisit from {databaseOwner}.yaf_User where UserID = @UserID
>>>>>>> .r1490
	end
	
	-- update last visit
	update [{databaseOwner}].{objectQualifier}User set 
		LastVisit = getdate(),
		IP = @IP
	where UserID = @UserID

	-- find missing ForumID/TopicID
	if @MessageID is not null begin
		select
			@CategoryID = c.CategoryID,
			@ForumID = b.ForumID,
			@TopicID = b.TopicID
		from
			{objectQualifier}Message a,
			{objectQualifier}Topic b,
			{objectQualifier}Forum c,
			{objectQualifier}Category d
		where
			a.MessageID = @MessageID and
			b.TopicID = a.TopicID and
			c.ForumID = b.ForumID and
			d.CategoryID = c.CategoryID and
			d.BoardID = @BoardID
	end
	else if @TopicID is not null begin
		select 
			@CategoryID = b.CategoryID,
			@ForumID = a.ForumID 
		from 
			{objectQualifier}Topic a,
			{objectQualifier}Forum b,
			{objectQualifier}Category c
		where 
			a.TopicID = @TopicID and
			b.ForumID = a.ForumID and
			c.CategoryID = b.CategoryID and
			c.BoardID = @BoardID
	end
	else if @ForumID is not null begin
		select
			@CategoryID = a.CategoryID
		from
			{objectQualifier}Forum a,
			{objectQualifier}Category b
		where
			a.ForumID = @ForumID and
			b.CategoryID = a.CategoryID and
			b.BoardID = @BoardID
	end
	-- update active
	if @UserID is not null and @UserBoardID=@BoardID begin
		if exists(select 1 from [{databaseOwner}].{objectQualifier}Active where SessionID=@SessionID and BoardID=@BoardID)
		begin
			update [{databaseOwner}].{objectQualifier}Active set
				UserID = @UserID,
				IP = @IP,
				LastActive = getdate(),
				Location = @Location,
				ForumID = @ForumID,
				TopicID = @TopicID,
				Browser = @Browser,
				Platform = @Platform
			where SessionID = @SessionID
		end
		else begin
			insert into [{databaseOwner}].{objectQualifier}Active(SessionID,BoardID,UserID,IP,Login,LastActive,Location,ForumID,TopicID,Browser,Platform)
			values(@SessionID,@BoardID,@UserID,@IP,getdate(),getdate(),@Location,@ForumID,@TopicID,@Browser,@Platform)
		end
		-- remove duplicate users
		if @IsGuest=0
			delete from [{databaseOwner}].{objectQualifier}Active where UserID=@UserID and BoardID=@BoardID and SessionID<>@SessionID
	end
	-- return information
	select
		a.UserID,
		a.ProviderUserKey,
		UserFlags			= a.Flags,
		UserName			= a.Name,
		Suspended			= a.Suspended,
		ThemeFile			= a.ThemeFile,
		LanguageFile		= a.LanguageFile,
		TimeZoneUser		= a.TimeZone,
		PreviousVisit		= @PreviousVisit,
		IsGuest				= a.Flags & 4,
		x.*,
		CategoryID			= @CategoryID,
		CategoryName		= (select Name from [{databaseOwner}].{objectQualifier}Category where CategoryID = @CategoryID),
		ForumID				= @ForumID,
		ForumName			= (select Name from [{databaseOwner}].{objectQualifier}Forum where ForumID = @ForumID),
		TopicID				= @TopicID,
		TopicName			= (select Topic from [{databaseOwner}].{objectQualifier}Topic where TopicID = @TopicID),
		MailsPending		= (select count(1) from [{databaseOwner}].{objectQualifier}Mail),
		Incoming			= (select count(1) from [{databaseOwner}].{objectQualifier}UserPMessage where UserID=a.UserID and IsRead=0),
		ForumTheme			= (select ThemeURL from [{databaseOwner}].{objectQualifier}Forum where ForumID = @ForumID)
	from
		{objectQualifier}User a
		left join [{databaseOwner}].{objectQualifier}vaccess x on x.UserID=a.UserID and x.ForumID=IsNull(@ForumID,0)
	where
		a.UserID = @UserID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}pmessage_delete](@PMessageID int, @FromOutbox bit = 0) as
=======
create procedure [{databaseOwner}].[yaf_pmessage_delete](@PMessageID int, @FromOutbox bit = 0) as
>>>>>>> .r1490
begin
	if @FromOutbox=1
		update [{objectQualifier}UserPMessage] set [IsInOutbox] = 0 where [PMessageID]=@PMessageID
	else
	BEGIN
		delete from [{objectQualifier}UserPMessage] where [PMessageID]=@PMessageID
		delete from [{objectQualifier}PMessage] where [PMessageID]=@PMessageID
	END
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}pmessage_info] as
=======
create procedure [{databaseOwner}].[yaf_pmessage_info] as
>>>>>>> .r1490
begin
	select
		NumRead	= (select count(1) from [{databaseOwner}].{objectQualifier}UserPMessage where IsRead<>0),
		NumUnread = (select count(1) from [{databaseOwner}].{objectQualifier}UserPMessage where IsRead=0),
		NumTotal = (select count(1) from [{databaseOwner}].{objectQualifier}UserPMessage)
end
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_list](@FromUserID int=null,@ToUserID int=null,@PMessageID int=null) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_pmessage_list](@FromUserID int=null,@ToUserID int=null,@PMessageID int=null) AS
>>>>>>> .r1490
BEGIN
	SELECT PMessageID, UserPMessageID, FromUserID, FromUser, ToUserID, ToUser, Created, Subject, Body, Flags, IsRead, IsInOutbox, IsArchived
		FROM [{databaseOwner}].{objectQualifier}PMessageView
		WHERE	((@PMessageId IS NOT NULL AND PMessageID=@PMessageId) OR 
				 (@ToUserID   IS NOT NULL AND ToUserID = @ToUserID) OR 
				 (@FromUserID IS NOT NULL AND FromUserID = @FromUserID))
		ORDER BY Created DESC
END
-- Old SPOC - modified by MicScoTho 01 June 2007
/*	if @PMessageID is null begin
		select
			a.*,
			FromUser = b.Name,
			ToUserID = c.UserID,
			ToUser = c.Name,
			d.IsRead,
			d.IsInOutbox,
			d.UserPMessageID
		from
			{objectQualifier}PMessage a,
			{objectQualifier}User b,
			{objectQualifier}User c,
			{objectQualifier}UserPMessage d
		where
			b.UserID = a.FromUserID and
			c.UserID = d.UserID and
			d.PMessageID = a.PMessageID and
			((@ToUserID is not null and d.UserID = @ToUserID) or (@FromUserID is not null and a.FromUserID = @FromUserID))
		order by
			Created desc
	end
	else begin
		select
			a.*,
			FromUser = b.Name,
			ToUserID = c.UserID,
			ToUser = c.Name,
			d.IsRead,
			d.UserPMessageID
		from
			{objectQualifier}PMessage a,
			{objectQualifier}User b,
			{objectQualifier}User c,
			{objectQualifier}UserPMessage d
		where
			b.UserID = a.FromUserID and
			c.UserID = d.UserID and
			d.PMessageID = a.PMessageID and
			a.PMessageID = @PMessageID and
			c.UserID = @FromUserID
		order by
			Created desc
	end
*/
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}pmessage_markread](@UserPMessageID int=null) as begin
	update [{databaseOwner}].{objectQualifier}UserPMessage set IsRead=1 where UserPMessageID=@UserPMessageID
=======
create procedure [{databaseOwner}].[yaf_pmessage_markread](@UserPMessageID int=null) as begin
	update yaf_UserPMessage set IsRead=1 where UserPMessageID=@UserPMessageID
>>>>>>> .r1490
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}pmessage_prune](@DaysRead int,@DaysUnread int) as
=======
create procedure [{databaseOwner}].[yaf_pmessage_prune](@DaysRead int,@DaysUnread int) as
>>>>>>> .r1490
begin
	delete from [{databaseOwner}].{objectQualifier}UserPMessage
	where IsRead<>0
	and datediff(dd,(select Created from [{databaseOwner}].{objectQualifier}PMessage x where x.PMessageID={objectQualifier}UserPMessage.PMessageID),getdate())>@DaysRead

	delete from [{databaseOwner}].{objectQualifier}UserPMessage
	where IsRead=0
	and datediff(dd,(select Created from [{databaseOwner}].{objectQualifier}PMessage x where x.PMessageID={objectQualifier}UserPMessage.PMessageID),getdate())>@DaysUnread

	delete from [{databaseOwner}].{objectQualifier}PMessage
	where not exists(select 1 from [{databaseOwner}].{objectQualifier}UserPMessage x where x.PMessageID={objectQualifier}PMessage.PMessageID)
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}pmessage_save](
=======
create procedure [{databaseOwner}].[yaf_pmessage_save](
>>>>>>> .r1490
	@FromUserID	int,
	@ToUserID	int,
	@Subject	nvarchar(100),
	@Body		ntext,
	@Flags		int
) as
begin
	declare @PMessageID int
	declare @UserID int

	insert into [{databaseOwner}].{objectQualifier}PMessage(FromUserID,Created,Subject,Body,Flags)
	values(@FromUserID,getdate(),@Subject,@Body,@Flags)

	set @PMessageID = SCOPE_IDENTITY()
	if (@ToUserID = 0)
	begin
		insert into [{databaseOwner}].{objectQualifier}UserPMessage(UserID,PMessageID,IsRead,IsInOutbox)
		select
				a.UserID,@PMessageID,0,1
		from
				{objectQualifier}User a
				join [{databaseOwner}].{objectQualifier}UserGroup b on b.UserID=a.UserID
				join [{databaseOwner}].{objectQualifier}Group c on c.GroupID=b.GroupID where
				(c.Flags & 2)=0 and
				c.BoardID=(select BoardID from [{databaseOwner}].{objectQualifier}User x where x.UserID=@FromUserID) and a.UserID<>@FromUserID
		group by
				a.UserID
	end
	else
	begin
		insert into [{databaseOwner}].{objectQualifier}UserPMessage(UserID,PMessageID,IsRead,IsInOutbox) values(@ToUserID,@PMessageID,0,1)
	end
end
GO


<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_archive](@PMessageID int = NULL) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_pmessage_archive](@PMessageID int = NULL) AS
>>>>>>> .r1490
BEGIN
	UPDATE [{databaseOwner}].{objectQualifier}UserPMessage SET IsArchived=1 WHERE UserPMessageID=@PMessageID
END
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}poll_save](
=======
CREATE procedure [{databaseOwner}].[yaf_poll_save](
>>>>>>> .r1490
	@Question	nvarchar(50),
	@Choice1	nvarchar(50),
	@Choice2	nvarchar(50),
	@Choice3	nvarchar(50) = null,
	@Choice4	nvarchar(50) = null,
	@Choice5	nvarchar(50) = null,
	@Choice6	nvarchar(50) = null,
	@Choice7	nvarchar(50) = null,
	@Choice8	nvarchar(50) = null,
	@Choice9	nvarchar(50) = null,
	@Closes 	datetime = null
) as
begin
	declare @PollID	int
	insert into [{databaseOwner}].{objectQualifier}Poll(Question,Closes) values(@Question,@Closes)
	set @PollID = SCOPE_IDENTITY()
	if @Choice1<>'' and @Choice1 is not null
		insert into [{databaseOwner}].{objectQualifier}Choice(PollID,Choice,Votes)
		values(@PollID,@Choice1,0)
	if @Choice2<>'' and @Choice2 is not null
		insert into [{databaseOwner}].{objectQualifier}Choice(PollID,Choice,Votes)
		values(@PollID,@Choice2,0)
	if @Choice3<>'' and @Choice3 is not null
		insert into [{databaseOwner}].{objectQualifier}Choice(PollID,Choice,Votes)
		values(@PollID,@Choice3,0)
	if @Choice4<>'' and @Choice4 is not null
		insert into [{databaseOwner}].{objectQualifier}Choice(PollID,Choice,Votes)
		values(@PollID,@Choice4,0)
	if @Choice5<>'' and @Choice5 is not null
		insert into [{databaseOwner}].{objectQualifier}Choice(PollID,Choice,Votes)
		values(@PollID,@Choice5,0)
	if @Choice6<>'' and @Choice6 is not null
		insert into [{databaseOwner}].{objectQualifier}Choice(PollID,Choice,Votes)
		values(@PollID,@Choice6,0)
	if @Choice7<>'' and @Choice7 is not null
		insert into [{databaseOwner}].{objectQualifier}Choice(PollID,Choice,Votes)
		values(@PollID,@Choice7,0)
	if @Choice8<>'' and @Choice8 is not null
		insert into [{databaseOwner}].{objectQualifier}Choice(PollID,Choice,Votes)
		values(@PollID,@Choice8,0)
	if @Choice9<>'' and @Choice9 is not null
		insert into [{databaseOwner}].{objectQualifier}Choice(PollID,Choice,Votes)
		values(@PollID,@Choice9,0)
	select PollID = @PollID
end
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}poll_stats](@PollID int) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_poll_stats](@PollID int) AS
>>>>>>> .r1490
BEGIN
	SELECT
		a.PollID,
		b.Question,
		b.Closes,
		a.ChoiceID,
		a.Choice,
		a.Votes,
		Stats = (select 100 * a.Votes / case sum(x.Votes) when 0 then 1 else sum(x.Votes) end from [{databaseOwner}].{objectQualifier}Choice x where x.PollID=a.PollID)
	FROM
		{objectQualifier}Choice a,
		{objectQualifier}Poll b
	WHERE
		b.PollID = a.PollID AND
		b.PollID = @PollID
END
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}pollvote_check](@PollID int, @UserID int = NULL,@RemoteIP nvarchar(10) = NULL) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_pollvote_check](@PollID int, @UserID int = NULL,@RemoteIP nvarchar(10) = NULL) AS
>>>>>>> .r1490

	IF @UserID IS NULL
	BEGIN
		IF @RemoteIP IS NOT NULL
		BEGIN
			-- check by remote IP
			SELECT PollVoteID FROM [{databaseOwner}].{objectQualifier}PollVote WHERE PollID = @PollID AND RemoteIP = @RemoteIP
		END
	END
	ELSE
	BEGIN
		-- check by userid or remote IP
		SELECT PollVoteID FROM [{databaseOwner}].{objectQualifier}PollVote WHERE PollID = @PollID AND (UserID = @UserID OR RemoteIP = @RemoteIP)
	END
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}post_last10user](@BoardID int,@UserID int,@PageUserID int) as
=======
create procedure [{databaseOwner}].[yaf_post_last10user](@BoardID int,@UserID int,@PageUserID int) as
>>>>>>> .r1490
begin
	set nocount on

	select top 10
		a.Posted,
		Subject = c.Topic,
		a.Message,
		a.UserID,
		a.Flags,
		UserName = IsNull(a.UserName,b.Name),
		b.Signature,
		c.TopicID
	from
		{objectQualifier}Message a
		join [{databaseOwner}].{objectQualifier}User b on b.UserID=a.UserID
		join [{databaseOwner}].{objectQualifier}Topic c on c.TopicID=a.TopicID
		join [{databaseOwner}].{objectQualifier}Forum d on d.ForumID=c.ForumID
		join [{databaseOwner}].{objectQualifier}Category e on e.CategoryID=d.CategoryID
		join [{databaseOwner}].{objectQualifier}vaccess x on x.ForumID=d.ForumID
	where
		a.UserID = @UserID and
		x.UserID = @PageUserID and
		x.ReadAccess <> 0 and
		e.BoardID = @BoardID and
		(a.Flags & 24)=16 and
		(c.Flags & 8)=0
	order by
		a.Posted desc
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}post_list](@TopicID int,@UpdateViewCount smallint=1, @ShowDeleted bit = 1) as
=======
create procedure [{databaseOwner}].[yaf_post_list](@TopicID int,@UpdateViewCount smallint=1, @ShowDeleted bit = 1) as
>>>>>>> .r1490
begin
	set nocount on
	if @UpdateViewCount>0
		update [{databaseOwner}].{objectQualifier}Topic set Views = Views + 1 where TopicID = @TopicID
	select
		d.TopicID,
		TopicFlags	= d.Flags,
		ForumFlags	= g.Flags,
		a.MessageID,
		a.Posted,
		Subject = d.Topic,
		a.Message,
		a.UserID,
		a.Position,
		a.Indent,
		a.IP,
		a.Flags,
		a.EditReason,
    a.IsModeratorChanged,
		a.DeleteReason,
		UserName	= IsNull(a.UserName,b.Name),
		b.Joined,
		b.Avatar,
		b.Signature,
		Posts		= b.NumPosts,
		b.Points,
		d.Views,
		d.ForumID,
		RankName = c.Name,
		c.RankImage,
		Edited = IsNull(a.Edited,a.Posted),
		HasAttachments	= (select count(1) from [{databaseOwner}].{objectQualifier}Attachment x where x.MessageID=a.MessageID),
		HasAvatarImage = (select count(1) from [{databaseOwner}].{objectQualifier}User x where x.UserID=b.UserID and AvatarImage is not null)
	from
		{objectQualifier}Message a
		join [{databaseOwner}].{objectQualifier}User b on b.UserID=a.UserID
		join [{databaseOwner}].{objectQualifier}Topic d on d.TopicID=a.TopicID
		join [{databaseOwner}].{objectQualifier}Forum g on g.ForumID=d.ForumID
		join [{databaseOwner}].{objectQualifier}Category h on h.CategoryID=g.CategoryID
		join [{databaseOwner}].{objectQualifier}Rank c on c.RankID=b.RankID
	where
		a.TopicID = @TopicID and
		((a.Flags & 24)=16 or ((a.Flags & 24)=24  and @showdeleted =1))
	order by
		a.Posted asc
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}post_list_reverse10](@TopicID int) as
=======
create procedure [{databaseOwner}].[yaf_post_list_reverse10](@TopicID int) as
>>>>>>> .r1490
begin
	set nocount on

	select top 10
		a.Posted,
		Subject = d.Topic,
		a.Message,
		a.UserID,
		a.Flags,
		UserName = IsNull(a.UserName,b.Name),
		b.Signature
	from
		{objectQualifier}Message a, 
		{objectQualifier}User b,
		{objectQualifier}Topic d
	where
		(a.Flags & 24)=16 and
		a.TopicID = @TopicID and
		b.UserID = a.UserID and
		d.TopicID = a.TopicID
	order by
		a.Posted desc
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}rank_delete](@RankID int) as begin
	delete from [{databaseOwner}].{objectQualifier}Rank where RankID = @RankID
=======
create procedure [{databaseOwner}].[yaf_rank_delete](@RankID int) as begin
	delete from yaf_Rank where RankID = @RankID
>>>>>>> .r1490
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}rank_list](@BoardID int,@RankID int=null) as begin
=======
create procedure [{databaseOwner}].[yaf_rank_list](@BoardID int,@RankID int=null) as begin
>>>>>>> .r1490
	if @RankID is null
		select
			a.*
		from
			{objectQualifier}Rank a
		where
			a.BoardID=@BoardID
		order by
			a.MinPosts,
			a.Name
	else
		select
			a.*
		from
			{objectQualifier}Rank a
		where
			a.RankID = @RankID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}rank_save](
=======
create procedure [{databaseOwner}].[yaf_rank_save](
>>>>>>> .r1490
	@RankID		int,
	@BoardID	int,
	@Name		nvarchar(50),
	@IsStart	bit,
	@IsLadder	bit,
	@MinPosts	int,
	@RankImage	nvarchar(50)=null
) as
begin
	declare @Flags int

	if @IsLadder=0 set @MinPosts = null
	if @IsLadder=1 and @MinPosts is null set @MinPosts = 0
	
	set @Flags = 0
	if @IsStart<>0 set @Flags = @Flags | 1
	if @IsLadder<>0 set @Flags = @Flags | 2
	
	if @RankID>0 begin
		update [{databaseOwner}].{objectQualifier}Rank set
			Name = @Name,
			Flags = @Flags,
			MinPosts = @MinPosts,
			RankImage = @RankImage
		where RankID = @RankID
	end
	else begin
		insert into [{databaseOwner}].{objectQualifier}Rank(BoardID,Name,Flags,MinPosts,RankImage)
		values(@BoardID,@Name,@Flags,@MinPosts,@RankImage);
	end
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}registry_list](@Name nvarchar(50) = null,@BoardID int = null) as
=======
create procedure [{databaseOwner}].[yaf_registry_list](@Name nvarchar(50) = null,@BoardID int = null) as
>>>>>>> .r1490
BEGIN
	if @BoardID is null
	begin
		IF @Name IS NULL OR @Name = ''
		BEGIN
			SELECT * FROM [{databaseOwner}].{objectQualifier}Registry where BoardID is null
		END ELSE
		BEGIN
			SELECT * FROM [{databaseOwner}].{objectQualifier}Registry WHERE LOWER(Name) = LOWER(@Name) and BoardID is null
		END
	end else 
	begin
		IF @Name IS NULL OR @Name = ''
		BEGIN
			SELECT * FROM [{databaseOwner}].{objectQualifier}Registry where BoardID=@BoardID
		END ELSE
		BEGIN
			SELECT * FROM [{databaseOwner}].{objectQualifier}Registry WHERE LOWER(Name) = LOWER(@Name) and BoardID=@BoardID
		END
	end
END
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}registry_save](
=======
create procedure [{databaseOwner}].[yaf_registry_save](
>>>>>>> .r1490
	@Name nvarchar(50),
	@Value ntext = NULL,
	@BoardID int = null
) AS
BEGIN
	if @BoardID is null
	begin
		if exists(select 1 from [{databaseOwner}].{objectQualifier}Registry where lower(Name)=lower(@Name))
			update [{databaseOwner}].{objectQualifier}Registry set Value = @Value where lower(Name)=lower(@Name) and BoardID is null
		else
		begin
			insert into [{databaseOwner}].{objectQualifier}Registry(Name,Value) values(lower(@Name),@Value)
		end
	end else
	begin
		if exists(select 1 from [{databaseOwner}].{objectQualifier}Registry where lower(Name)=lower(@Name) and BoardID=@BoardID)
			update [{databaseOwner}].{objectQualifier}Registry set Value = @Value where lower(Name)=lower(@Name) and BoardID=@BoardID
		else
		begin
			insert into [{databaseOwner}].{objectQualifier}Registry(Name,Value,BoardID) values(lower(@Name),@Value,@BoardID)
		end
	end
END
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}replace_words_delete](@ID int) as
=======
create procedure [{databaseOwner}].[yaf_replace_words_delete](@ID int) as
>>>>>>> .r1490
begin
<<<<<<< .mine
	delete from [{databaseOwner}].{objectQualifier}replace_words where ID = @ID
=======
	delete from {databaseOwner}.yaf_replace_words where ID = @ID
>>>>>>> .r1490
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}replace_words_edit](@ID int=null) as
=======
create procedure [{databaseOwner}].[yaf_replace_words_edit](@ID int=null) as
>>>>>>> .r1490
begin
	select * from [{databaseOwner}].{objectQualifier}replace_words where ID=@ID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}replace_words_list] as begin
	select * from [{databaseOwner}].{objectQualifier}Replace_Words
=======
create procedure [{databaseOwner}].[yaf_replace_words_list] as begin
	select * from yaf_Replace_Words
>>>>>>> .r1490
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}replace_words_save](@ID int=null,@badword nvarchar(255),@goodword nvarchar(255)) as
=======
create procedure [{databaseOwner}].[yaf_replace_words_save](@ID int=null,@badword nvarchar(255),@goodword nvarchar(255)) as
>>>>>>> .r1490
begin
	if @ID is null or @ID = 0 begin
		insert into [{databaseOwner}].{objectQualifier}replace_words(badword,goodword) values(@badword,@goodword)
	end
	else begin
		update [{databaseOwner}].{objectQualifier}replace_words set badword = @badword,goodword = @goodword where ID = @ID
	end
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}smiley_delete](@SmileyID int=null) as begin
=======
create procedure [{databaseOwner}].[yaf_smiley_delete](@SmileyID int=null) as begin
>>>>>>> .r1490
	if @SmileyID is not null
		delete from [{databaseOwner}].{objectQualifier}Smiley where SmileyID=@SmileyID
	else
		delete from [{databaseOwner}].{objectQualifier}Smiley
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}smiley_list](@BoardID int,@SmileyID int=null) as
=======
create procedure [{databaseOwner}].[yaf_smiley_list](@BoardID int,@SmileyID int=null) as
>>>>>>> .r1490
begin
	if @SmileyID is null
		select * from [{databaseOwner}].{objectQualifier}Smiley where BoardID=@BoardID order by SortOrder, LEN(Code) desc
	else
		select * from [{databaseOwner}].{objectQualifier}Smiley where SmileyID=@SmileyID order by SortOrder
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}smiley_listunique](@BoardID int) as
=======
create procedure [{databaseOwner}].[yaf_smiley_listunique](@BoardID int) as
>>>>>>> .r1490
begin
	select 
		Icon, 
		Emoticon,
		Code = (select top 1 Code from [{databaseOwner}].{objectQualifier}Smiley x where x.Icon={objectQualifier}Smiley.Icon),
		SortOrder = (select top 1 SortOrder from [{databaseOwner}].{objectQualifier}Smiley x where x.Icon={objectQualifier}Smiley.Icon order by x.SortOrder asc)
	from 
		{objectQualifier}Smiley
	where
		BoardID=@BoardID
	group by
		Icon,
		Emoticon
	order by
		SortOrder,
		Code
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}smiley_save](@SmileyID int=null,@BoardID int,@Code nvarchar(10),@Icon nvarchar(50),@Emoticon nvarchar(50),@SortOrder tinyint,@Replace smallint=0) as begin
=======
create procedure [{databaseOwner}].[yaf_smiley_save](@SmileyID int=null,@BoardID int,@Code nvarchar(10),@Icon nvarchar(50),@Emoticon nvarchar(50),@SortOrder tinyint,@Replace smallint=0) as begin
>>>>>>> .r1490
	if @SmileyID is not null begin
		update [{databaseOwner}].{objectQualifier}Smiley set Code = @Code, Icon = @Icon, Emoticon = @Emoticon, SortOrder = @SortOrder where SmileyID = @SmileyID
	end
	else begin
		if @Replace>0
			delete from [{databaseOwner}].{objectQualifier}Smiley where Code=@Code

		if not exists(select 1 from [{databaseOwner}].{objectQualifier}Smiley where BoardID=@BoardID and Code=@Code)
			insert into [{databaseOwner}].{objectQualifier}Smiley(BoardID,Code,Icon,Emoticon,SortOrder) values(@BoardID,@Code,@Icon,@Emoticon,@SortOrder)
	end
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}smiley_resort](@BoardID int,@SmileyID int,@Move int) as
=======
create procedure [{databaseOwner}].[yaf_smiley_resort](@BoardID int,@SmileyID int,@Move int) as
>>>>>>> .r1490
begin
	declare @Position int

	SELECT @Position=SortOrder FROM [{databaseOwner}].{objectQualifier}Smiley WHERE BoardID=@BoardID and SmileyID=@SmileyID

	if (@Position is null) return

	if (@Move > 0) begin
		update [{databaseOwner}].{objectQualifier}Smiley
			set SortOrder=SortOrder-1
			where BoardID=@BoardID and 
				SortOrder between @Position and (@Position + @Move) and
				SortOrder between 1 and 255
	end
	else if (@Move < 0) begin
		update [{databaseOwner}].{objectQualifier}Smiley
			set SortOrder=SortOrder+1
			where BoardID=@BoardID and 
				SortOrder between (@Position+@Move) and @Position and
				SortOrder between 0 and 254
	end

	SET @Position = @Position + @Move

	if (@Position>255) SET @Position = 255
	else if (@Position<0) SET @Position = 0

	update [{databaseOwner}].{objectQualifier}Smiley
		set SortOrder=@Position
		where BoardID=@BoardID and 
			SmileyID=@SmileyID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}system_initialize](
=======
create procedure [{databaseOwner}].[yaf_system_initialize](
>>>>>>> .r1490
	@Name		nvarchar(50),
	@TimeZone	int,
	@ForumEmail	nvarchar(50),
	@SmtpServer	nvarchar(50),
	@User		nvarchar(50),
	@UserEmail	nvarchar(50),
	@Password	nvarchar(32)
) as 
begin
	DECLARE @tmpValue AS nvarchar(100)

	-- initalize required 'registry' settings
	EXEC [{databaseOwner}].{objectQualifier}registry_save 'Version','1'
	EXEC [{databaseOwner}].{objectQualifier}registry_save 'VersionName','1.0.0'
	SET @tmpValue = CAST(@TimeZone AS nvarchar(100))
	EXEC [{databaseOwner}].{objectQualifier}registry_save 'TimeZone', @tmpValue
	EXEC [{databaseOwner}].{objectQualifier}registry_save 'SmtpServer', @SmtpServer
	EXEC [{databaseOwner}].{objectQualifier}registry_save 'ForumEmail', @ForumEmail

	-- initalize new board
	EXEC [{databaseOwner}].{objectQualifier}board_create @Name,0,@User,@UserEmail,@Password,1
end
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}system_updateversion]
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_system_updateversion]
>>>>>>> .r1490
(
	@Version		int,
	@VersionName	nvarchar(50)
) 
AS
BEGIN

	DECLARE @tmpValue AS nvarchar(100)
	SET @tmpValue = CAST(@Version AS nvarchar(100))
	EXEC [{databaseOwner}].{objectQualifier}registry_save 'Version', @tmpValue
	EXEC [{databaseOwner}].{objectQualifier}registry_save 'VersionName',@VersionName

END
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}topic_active](@BoardID int,@UserID int,@Since datetime,@CategoryID int=null) as
=======
create procedure [{databaseOwner}].[yaf_topic_active](@BoardID int,@UserID int,@Since datetime,@CategoryID int=null) as
>>>>>>> .r1490
begin
	select
		c.ForumID,
		c.TopicID,
		c.Posted,
		LinkTopicID = IsNull(c.TopicMovedID,c.TopicID),
		Subject = c.Topic,
		c.UserID,
		Starter = IsNull(c.UserName,b.Name),
		Replies = (select count(1) from [{databaseOwner}].{objectQualifier}Message x where x.TopicID=c.TopicID and (x.Flags & 8)=0) - 1,
		Views = c.Views,
		LastPosted = c.LastPosted,
		LastUserID = c.LastUserID,
		LastUserName = IsNull(c.LastUserName,(select Name from [{databaseOwner}].{objectQualifier}User x where x.UserID=c.LastUserID)),
		LastMessageID = c.LastMessageID,
		LastTopicID = c.TopicID,
		TopicFlags = c.Flags,
		c.Priority,
		c.PollID,
		ForumName = d.Name,
		c.TopicMovedID,
		ForumFlags = d.Flags
	from
		{objectQualifier}Topic c
		join [{databaseOwner}].{objectQualifier}User b on b.UserID=c.UserID
		join [{databaseOwner}].{objectQualifier}Forum d on d.ForumID=c.ForumID
		join [{databaseOwner}].{objectQualifier}vaccess x on x.ForumID=d.ForumID
		join [{databaseOwner}].{objectQualifier}Category e on e.CategoryID=d.CategoryID
	where
		@Since < c.LastPosted and
		x.UserID = @UserID and
		x.ReadAccess <> 0 and
		e.BoardID = @BoardID and
		(@CategoryID is null or e.CategoryID=@CategoryID) and
		(c.Flags & 8)=0
	order by
		d.Name asc,
		Priority desc,
		LastPosted desc
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}topic_delete] (@TopicID int,@UpdateLastPost bit=1,@EraseTopic bit=0) 
=======
create procedure [{databaseOwner}].[yaf_topic_delete] (@TopicID int,@UpdateLastPost bit=1,@EraseTopic bit=0) 
>>>>>>> .r1490
as
begin
	SET NOCOUNT ON
	declare @ForumID int
	declare @pollID int
	
	select @ForumID=ForumID from [{databaseOwner}].{objectQualifier}Topic where TopicID=@TopicID
	update [{databaseOwner}].{objectQualifier}Topic set LastMessageID = null where TopicID = @TopicID
	update [{databaseOwner}].{objectQualifier}Forum set 
		LastTopicID = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null,
		LastPosted = null
	where LastMessageID in (select MessageID from [{databaseOwner}].{objectQualifier}Message where TopicID = @TopicID)
	update [{databaseOwner}].{objectQualifier}Active set TopicID = null where TopicID = @TopicID
	
	--remove polls	
	select @pollID = pollID from [{databaseOwner}].{objectQualifier}topic where TopicID = @TopicID
	if (@pollID is not null)
	begin
		delete from [{databaseOwner}].{objectQualifier}choice where PollID = @PollID
		update [{databaseOwner}].{objectQualifier}topic set PollID = null where TopicID = @TopicID
		delete from [{databaseOwner}].{objectQualifier}poll where PollID = @PollID	
	end	
	
	--delete messages and topics
	delete from [{databaseOwner}].{objectQualifier}nntptopic where TopicID = @TopicID
	delete from [{databaseOwner}].{objectQualifier}topic where TopicMovedID = @TopicID

	if @EraseTopic = 0
	begin
		update [{databaseOwner}].{objectQualifier}topic set Flags = Flags | 8 where TopicID = @TopicID
	end
	else
	begin
		delete from [{databaseOwner}].{objectQualifier}attachment where MessageID IN (select MessageID from [{databaseOwner}].{objectQualifier}message where TopicID = @TopicID) 
		delete from [{databaseOwner}].{objectQualifier}message where TopicID = @TopicID
		delete from [{databaseOwner}].{objectQualifier}topic where TopicID = @TopicID
	end
		
	--commit
	if @UpdateLastPost<>0
		exec [{databaseOwner}].{objectQualifier}forum_updatelastpost @ForumID
	
	if @ForumID is not null
		exec [{databaseOwner}].{objectQualifier}forum_updatestats @ForumID
end

GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}topic_findnext](@TopicID int) as
=======
create procedure [{databaseOwner}].[yaf_topic_findnext](@TopicID int) as
>>>>>>> .r1490
begin
	declare @LastPosted datetime
	declare @ForumID int
	select @LastPosted = LastPosted, @ForumID = ForumID from [{databaseOwner}].{objectQualifier}Topic where TopicID = @TopicID
	select top 1 TopicID from [{databaseOwner}].{objectQualifier}Topic where LastPosted>@LastPosted and ForumID = @ForumID AND (Flags & 8) = 0 order by LastPosted asc
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}topic_findprev](@TopicID int) AS 
=======
create procedure [{databaseOwner}].[yaf_topic_findprev](@TopicID int) AS 
>>>>>>> .r1490
BEGIN
	DECLARE @LastPosted datetime
	DECLARE @ForumID int
	SELECT @LastPosted = LastPosted, @ForumID = ForumID FROM [{databaseOwner}].{objectQualifier}Topic WHERE TopicID = @TopicID
	SELECT TOP 1 TopicID from [{databaseOwner}].{objectQualifier}Topic where LastPosted<@LastPosted AND ForumID = @ForumID AND (Flags & 8) = 0 ORDER BY LastPosted DESC
END
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}topic_info]
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_topic_info]
>>>>>>> .r1490
(
	@TopicID int = null,
	@ShowDeleted bit = 0
)
AS
BEGIN
	IF @TopicID = 0 SET @TopicID = NULL

	IF @TopicID IS NULL
	BEGIN
		IF @ShowDeleted = 1 
			SELECT * FROM [{databaseOwner}].{objectQualifier}Topic
		ELSE
			SELECT * FROM [{databaseOwner}].{objectQualifier}Topic WHERE (Flags & 8) = 0
	END
	ELSE
	BEGIN
		IF @ShowDeleted = 1 
			SELECT * FROM [{databaseOwner}].{objectQualifier}Topic WHERE TopicID = @TopicID
		ELSE
			SELECT * FROM [{databaseOwner}].{objectQualifier}Topic WHERE TopicID = @TopicID AND (Flags & 8) = 0		
	END
END
GO


<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}topic_announcements]
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_topic_announcements]
>>>>>>> .r1490
(
	@BoardID int,
	@NumPosts int,
	@UserID int
)
AS
BEGIN

	DECLARE @SQL nvarchar(500)

	SET @SQL = 'SELECT DISTINCT TOP ' + convert(varchar, @NumPosts) + ' t.Topic, t.LastPosted, t.TopicID, t.LastMessageID FROM'
	SET @SQL = @SQL + ' [{databaseOwner}].{objectQualifier}Topic t INNER JOIN [{databaseOwner}].{objectQualifier}Category c INNER JOIN [{databaseOwner}].{objectQualifier}Forum f ON c.CategoryID = f.CategoryID ON t.ForumID = f.ForumID'
	SET @SQL = @SQL + ' join [{databaseOwner}].{objectQualifier}vaccess v on v.ForumID=f.ForumID'
	SET @SQL = @SQL + ' WHERE c.BoardID = ' + convert(varchar, @BoardID) + ' AND v.UserID=' + convert(varchar,@UserID) + ' AND (v.ReadAccess <> 0 or (f.Flags & 2) = 0) AND (t.Flags & 8) != 8 AND (t.Priority = 2) ORDER BY t.LastPosted DESC'

	EXEC(@SQL)	

END
GO


<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}topic_latest]
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_topic_latest]
>>>>>>> .r1490
(
	@BoardID int,
	@NumPosts int,
	@UserID int
)
AS
BEGIN
	SET ROWCOUNT @NumPosts
	
	SELECT t.LastPosted, t.Topic, t.TopicID, t.LastMessageID FROM [{databaseOwner}].{objectQualifier}Topic t
	INNER JOIN [{databaseOwner}].{objectQualifier}Category c
	INNER JOIN [{databaseOwner}].{objectQualifier}Forum f
	ON c.CategoryID = f.CategoryID
	ON t.ForumID = f.ForumID
	JOIN [{databaseOwner}].{objectQualifier}vaccess v
	ON v.ForumID=f.ForumID
	WHERE c.BoardID = @BoardID AND v.UserID=@UserID AND (v.ReadAccess <> 0) AND (t.Flags & 8) = 0 ORDER BY t.LastPosted DESC;

END
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}topic_list](@ForumID int,@Announcement smallint,@Date datetime=null,@Offset int,@Count int) as
=======
CREATE procedure [{databaseOwner}].[yaf_topic_list](@ForumID int,@Announcement smallint,@Date datetime=null,@Offset int,@Count int) as
>>>>>>> .r1490
begin
	create table #data(
		RowNo	int identity primary key not null,
		TopicID	int not null
	)

	insert into #data(TopicID)
	select
		c.TopicID
	from
		{objectQualifier}Topic c join [{databaseOwner}].{objectQualifier}User b on b.UserID=c.UserID join [{databaseOwner}].{objectQualifier}Forum d on d.ForumID=c.ForumID
	where
		c.ForumID = @ForumID
	and
		(@Date is null or c.Posted>=@Date or c.LastPosted>=@Date or Priority>0) 
	and
		((@Announcement=1 and c.Priority=2) or (@Announcement=0 and c.Priority<>2) or (@Announcement<0)) 
	and	
		(c.TopicMovedID is not null or c.NumPosts > 0) 
	and
		(c.Flags & 8)=0
	order by
		Priority desc,
		c.LastPosted desc

	declare	@RowCount int
	set @RowCount = (select count(1) from #data)

	select
		[RowCount] = @RowCount,
		c.ForumID,
		c.TopicID,
		c.Posted,
		LinkTopicID = IsNull(c.TopicMovedID,c.TopicID),
		c.TopicMovedID,
		[Subject] = c.Topic,
		c.UserID,
		Starter = IsNull(c.UserName,b.Name),
		Replies = c.NumPosts - 1,
		[Views] = c.Views,
		LastPosted = c.LastPosted,
		LastUserID = c.LastUserID,
		LastUserName = IsNull(c.LastUserName,(select Name from [{databaseOwner}].{objectQualifier}User x where x.UserID=c.LastUserID)),
		LastMessageID = c.LastMessageID,
		LastTopicID = c.TopicID,
		TopicFlags = c.Flags,
		c.Priority,
		c.PollID,
		ForumFlags = d.Flags
	from
		{objectQualifier}Topic c 
		join [{databaseOwner}].{objectQualifier}User b on b.UserID=c.UserID 
		join [{databaseOwner}].{objectQualifier}Forum d on d.ForumID=c.ForumID 
		join #data e on e.TopicID=c.TopicID
	where
		e.RowNo between @Offset+1 and @Offset + @Count
	order by
		e.RowNo
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}topic_listmessages](@TopicID int) as
=======
create procedure [{databaseOwner}].[yaf_topic_listmessages](@TopicID int) as
>>>>>>> .r1490
begin
	select * from [{databaseOwner}].{objectQualifier}Message
	where TopicID = @TopicID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}topic_lock](@TopicID int,@Locked bit) as
=======
create procedure [{databaseOwner}].[yaf_topic_lock](@TopicID int,@Locked bit) as
>>>>>>> .r1490
begin
	if @Locked<>0
		update [{databaseOwner}].{objectQualifier}Topic set Flags = Flags | 1 where TopicID = @TopicID
	else
		update [{databaseOwner}].{objectQualifier}Topic set Flags = Flags & ~1 where TopicID = @TopicID
end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}topic_move](@TopicID int,@ForumID int,@ShowMoved bit) AS
=======
CREATE procedure [{databaseOwner}].[yaf_topic_move](@TopicID int,@ForumID int,@ShowMoved bit) AS
>>>>>>> .r1490
begin
    declare @OldForumID int

    select @OldForumID = ForumID from [{databaseOwner}].{objectQualifier}Topic where TopicID = @TopicID

    if @ShowMoved<>0 begin
        -- create a moved message
        insert into [{databaseOwner}].{objectQualifier}Topic(ForumID,UserID,UserName,Posted,Topic,Views,Flags,Priority,PollID,TopicMovedID,LastPosted,NumPosts)
        select ForumID,UserID,UserName,Posted,Topic,0,Flags,Priority,PollID,@TopicID,LastPosted,0
        from [{databaseOwner}].{objectQualifier}Topic where TopicID = @TopicID
    end

    -- move the topic
    update [{databaseOwner}].{objectQualifier}Topic set ForumID = @ForumID where TopicID = @TopicID

    -- update last posts
    exec [{databaseOwner}].{objectQualifier}forum_updatelastpost @OldForumID
    exec [{databaseOwner}].{objectQualifier}forum_updatelastpost @ForumID
    
    -- update stats
    exec [{databaseOwner}].{objectQualifier}forum_updatestats @OldForumID
    exec [{databaseOwner}].{objectQualifier}forum_updatestats @ForumID
    
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}topic_prune](@ForumID int=null,@Days int) as
=======
create procedure [{databaseOwner}].[yaf_topic_prune](@ForumID int=null,@Days int) as
>>>>>>> .r1490
begin
	declare @c cursor
	declare @TopicID int
	declare @Count int
	set @Count = 0
	if @ForumID = 0 set @ForumID = null
	if @ForumID is not null begin
		set @c = cursor for
		select 
			TopicID
		from 
			{objectQualifier}Topic
		where 
			ForumID = @ForumID and
			Priority = 0 and
			(Flags & 512) = 0 and					/* not flagged as persistent */
			datediff(dd,LastPosted,getdate())>@Days
	end
	else begin
		set @c = cursor for
		select 
			TopicID
		from 
			{objectQualifier}Topic
		where 
			Priority = 0 and
			(Flags & 512) = 0 and					/* not flagged as persistent */
			datediff(dd,LastPosted,getdate())>@Days
	end
	open @c
	fetch @c into @TopicID
	while @@FETCH_STATUS=0 begin
		exec [{databaseOwner}].{objectQualifier}topic_delete @TopicID,0
		set @Count = @Count + 1
		fetch @c into @TopicID
	end
	close @c
	deallocate @c

	-- This takes forever with many posts...
	--exec [{databaseOwner}].{objectQualifier}topic_updatelastpost

	select Count = @Count
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}topic_save](
=======
create procedure [{databaseOwner}].[yaf_topic_save](
>>>>>>> .r1490
	@ForumID	int,
	@Subject	nvarchar(100),
	@UserID		int,
	@Message	ntext,
	@Priority	smallint,
	@UserName	nvarchar(50)=null,
	@IP			nvarchar(15),
	@PollID		int=null,
	@Posted		datetime=null,
	@BlogPostID	nvarchar(50),
	@Flags		int
) as
begin
	declare @TopicID int
	declare @MessageID int

	if @Posted is null set @Posted = getdate()

	-- create the topic
	insert into [{databaseOwner}].{objectQualifier}Topic(ForumID,Topic,UserID,Posted,Views,Priority,PollID,UserName,NumPosts)
	values(@ForumID,@Subject,@UserID,@Posted,0,@Priority,@PollID,@UserName,0)

	-- get its id
	set @TopicID = SCOPE_IDENTITY()
	
	-- add message to the topic
	exec [{databaseOwner}].{objectQualifier}message_save @TopicID,@UserID,@Message,@UserName,@IP,@Posted,null,@BlogPostID,@Flags,@MessageID output

	select TopicID = @TopicID, MessageID = @MessageID
end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}topic_updatelastpost]
=======
CREATE procedure [{databaseOwner}].[yaf_topic_updatelastpost]
>>>>>>> .r1490
(@ForumID int=null,@TopicID int=null) as
begin

    if @TopicID is not null
        update [{databaseOwner}].{objectQualifier}Topic set
            LastPosted = (select top 1 x.Posted from [{databaseOwner}].{objectQualifier}Message x where x.TopicID={objectQualifier}Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastMessageID = (select top 1 x.MessageID from [{databaseOwner}].{objectQualifier}Message x where x.TopicID={objectQualifier}Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserID = (select top 1 x.UserID from [{databaseOwner}].{objectQualifier}Message x where x.TopicID={objectQualifier}Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserName = (select top 1 x.UserName from [{databaseOwner}].{objectQualifier}Message x where x.TopicID={objectQualifier}Topic.TopicID and (x.Flags & 24)=16 order by Posted desc)
        where TopicID = @TopicID
    else
        update [{databaseOwner}].{objectQualifier}Topic set
            LastPosted = (select top 1 x.Posted from [{databaseOwner}].{objectQualifier}Message x where x.TopicID={objectQualifier}Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastMessageID = (select top 1 x.MessageID from [{databaseOwner}].{objectQualifier}Message x where x.TopicID={objectQualifier}Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserID = (select top 1 x.UserID from [{databaseOwner}].{objectQualifier}Message x where x.TopicID={objectQualifier}Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserName = (select top 1 x.UserName from [{databaseOwner}].{objectQualifier}Message x where x.TopicID={objectQualifier}Topic.TopicID and (x.Flags & 24)=16 order by Posted desc)
        where TopicMovedID is null
        and (@ForumID is null or ForumID=@ForumID)

    exec [{databaseOwner}].{objectQualifier}forum_updatelastpost @ForumID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_accessmasks](@BoardID int,@UserID int) as
=======
create procedure [{databaseOwner}].[yaf_user_accessmasks](@BoardID int,@UserID int) as
>>>>>>> .r1490
begin
	select * from(
		select
			AccessMaskID	= e.AccessMaskID,
			AccessMaskName	= e.Name,
			ForumID			= f.ForumID,
			ForumName		= f.Name,
			CategoryID		= f.CategoryID,
			ParentID		= f.ParentID
		from
			{objectQualifier}User a 
			join [{databaseOwner}].{objectQualifier}UserGroup b on b.UserID=a.UserID
			join [{databaseOwner}].{objectQualifier}Group c on c.GroupID=b.GroupID
			join [{databaseOwner}].{objectQualifier}ForumAccess d on d.GroupID=c.GroupID
			join [{databaseOwner}].{objectQualifier}AccessMask e on e.AccessMaskID=d.AccessMaskID
			join [{databaseOwner}].{objectQualifier}Forum f on f.ForumID=d.ForumID
		where
			a.UserID=@UserID and
			c.BoardID=@BoardID
		group by
			e.AccessMaskID,
			e.Name,
			f.ForumID,
			f.ParentID,
			f.CategoryID,
			f.Name
		
		union
			
		select
			AccessMaskID	= c.AccessMaskID,
			AccessMaskName	= c.Name,
			ForumID			= d.ForumID,
			ForumName		= d.Name,
			CategoryID		= d.CategoryID,
			ParentID		= d.ParentID
		from
			{objectQualifier}User a 
			join [{databaseOwner}].{objectQualifier}UserForum b on b.UserID=a.UserID
			join [{databaseOwner}].{objectQualifier}AccessMask c on c.AccessMaskID=b.AccessMaskID
			join [{databaseOwner}].{objectQualifier}Forum d on d.ForumID=b.ForumID
		where
			a.UserID=@UserID and
			c.BoardID=@BoardID
		group by
			c.AccessMaskID,
			c.Name,
			d.ForumID,
			d.ParentID,
			d.CategoryID,
			d.Name
	) as x
	order by
		ForumName, AccessMaskName
end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}user_activity_rank](@StartDate as datetime) AS
=======
CREATE procedure [{databaseOwner}].[yaf_user_activity_rank](@StartDate as datetime) AS
>>>>>>> .r1490
begin
	select top 3  ID, Name, NumOfPosts from [{databaseOwner}].{objectQualifier}User u inner join
	(
		select m.UserID as ID, Count(m.UserID) as NumOfPosts from [{databaseOwner}].{objectQualifier}Message m
		where m.Posted >= @StartDate
		group by m.UserID
	) as counter
	on u.UserID = counter.ID
	order by NumOfPosts desc
end
go

<<<<<<< .mine
create PROCEDURE [{databaseOwner}].[{objectQualifier}user_addpoints] (@UserID int,@Points int) AS
=======
create PROCEDURE [{databaseOwner}].[yaf_user_addpoints] (@UserID int,@Points int) AS
>>>>>>> .r1490
BEGIN
	UPDATE [{databaseOwner}].{objectQualifier}User SET Points = Points + @Points WHERE UserID = @UserID
END

GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_adminsave](@BoardID int,@UserID int,@Name nvarchar(50),@Email nvarchar(50),@IsHostAdmin bit,@IsGuest bit,@RankID int) as
=======
create procedure [{databaseOwner}].[yaf_user_adminsave](@BoardID int,@UserID int,@Name nvarchar(50),@Email nvarchar(50),@IsHostAdmin bit,@IsGuest bit,@RankID int) as
>>>>>>> .r1490
begin
	if @IsHostAdmin<>0
		update [{databaseOwner}].{objectQualifier}User set Flags = Flags | 1 where UserID = @UserID
	else
		update [{databaseOwner}].{objectQualifier}User set Flags = Flags & ~1 where UserID = @UserID

	if @IsGuest<>0
		update [{databaseOwner}].{objectQualifier}User set Flags = Flags | 4 where UserID = @UserID
	else
		update [{databaseOwner}].{objectQualifier}User set Flags = Flags & ~4 where UserID = @UserID

	update [{databaseOwner}].{objectQualifier}User set
		Name = @Name,
		Email = @Email,
		RankID = @RankID
	where UserID = @UserID
	select UserID = @UserID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_approve](@UserID int) as
=======
create procedure [{databaseOwner}].[yaf_user_approve](@UserID int) as
>>>>>>> .r1490
begin
	declare @CheckEmailID int
	declare @Email nvarchar(50)

	select 
		@CheckEmailID = CheckEmailID,
		@Email = Email
	from
		{objectQualifier}CheckEmail
	where
		UserID = @UserID

	-- Update new user email
	update [{databaseOwner}].{objectQualifier}User set Email = @Email, Flags = Flags | 2 where UserID = @UserID
	delete [{databaseOwner}].{objectQualifier}CheckEmail where CheckEmailID = @CheckEmailID
	select convert(bit,1)
end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}user_approveall](@BoardID int) as
=======
CREATE procedure [{databaseOwner}].[yaf_user_approveall](@BoardID int) as
>>>>>>> .r1490
begin

	DECLARE userslist CURSOR FOR 
		SELECT UserID FROM [{databaseOwner}].{objectQualifier}User WHERE BoardID=@BoardID AND (Flags & 2)=0
		FOR READ ONLY


	OPEN userslist

	DECLARE @UserID int

	FETCH userslist INTO @UserID

	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXEC [{databaseOwner}].{objectQualifier}user_approve @UserID
		FETCH userslist INTO @UserID		
	END

	CLOSE userslist

end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}user_aspnet](@BoardID int,@UserName nvarchar(50),@Email nvarchar(50),@ProviderUserKey uniqueidentifier,@IsApproved bit) as
=======
CREATE procedure [{databaseOwner}].[yaf_user_aspnet](@BoardID int,@UserName nvarchar(50),@Email nvarchar(50),@ProviderUserKey uniqueidentifier,@IsApproved bit) as
>>>>>>> .r1490
begin
	declare @UserID int, @RankID int, @approvedFlag int

	SET @approvedFlag = 0;
	IF (@IsApproved = 1) SET @approvedFlag = 2;	
	
<<<<<<< .mine
	if exists(select 1 from [{databaseOwner}].{objectQualifier}User where BoardID=@BoardID and [Name]=@UserName)
=======
	if exists(select 1 from {databaseOwner}.yaf_User where BoardID=@BoardID and [Name]=@UserName)
>>>>>>> .r1490
	begin
<<<<<<< .mine
		select @UserID=UserID from [{databaseOwner}].{objectQualifier}User where BoardID=@BoardID and [Name]=@UserName
		update [{databaseOwner}].{objectQualifier}User set 
=======
		select @UserID=UserID from {databaseOwner}.yaf_User where BoardID=@BoardID and [Name]=@UserName
		update {databaseOwner}.yaf_User set 
>>>>>>> .r1490
			[Name] = @UserName,
			Email = @Email,
			ProviderUserKey = @ProviderUserKey,
			Flags = Flags | @approvedFlag
		where
			UserID = @UserID
	end else
	begin
<<<<<<< .mine
		select @RankID = RankID from [{databaseOwner}].{objectQualifier}Rank where (Flags & 1)<>0 and BoardID=@BoardID
=======
		select @RankID = RankID from {databaseOwner}.yaf_Rank where (Flags & 1)<>0 and BoardID=@BoardID
>>>>>>> .r1490

<<<<<<< .mine
		insert into [{databaseOwner}].{objectQualifier}User(BoardID,RankID,[Name],Password,Email,Joined,LastVisit,NumPosts,TimeZone,Flags,ProviderUserKey) 
=======
		insert into {databaseOwner}.yaf_User(BoardID,RankID,[Name],Password,Email,Joined,LastVisit,NumPosts,TimeZone,Flags,ProviderUserKey) 
>>>>>>> .r1490
		values(@BoardID,@RankID,@UserName,'-',@Email,getdate(),getdate(),0,0,@approvedFlag,@ProviderUserKey)
	
		set @UserID = SCOPE_IDENTITY()
	
	end
	
	select UserID=@UserID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_avatarimage](@UserID int) as begin
	select UserID,AvatarImage from [{databaseOwner}].{objectQualifier}User where UserID=@UserID
=======
create procedure [{databaseOwner}].[yaf_user_avatarimage](@UserID int) as begin
	select UserID,AvatarImage from yaf_User where UserID=@UserID
>>>>>>> .r1490
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_changepassword](@UserID int,@OldPassword nvarchar(32),@NewPassword nvarchar(32)) as
=======
create procedure [{databaseOwner}].[yaf_user_changepassword](@UserID int,@OldPassword nvarchar(32),@NewPassword nvarchar(32)) as
>>>>>>> .r1490
begin
	declare @CurrentOld nvarchar(32)
	select @CurrentOld = Password from [{databaseOwner}].{objectQualifier}User where UserID = @UserID
	if @CurrentOld<>@OldPassword begin
		select Success = convert(bit,0)
		return
	end
	update [{databaseOwner}].{objectQualifier}User set Password = @NewPassword where UserID = @UserID
	select Success = convert(bit,1)
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_delete](@UserID int) as
=======
create procedure [{databaseOwner}].[yaf_user_delete](@UserID int) as
>>>>>>> .r1490
begin
	declare @GuestUserID	int
	declare @UserName		nvarchar(50)
	declare @GuestCount		int

	select @UserName = Name from [{databaseOwner}].{objectQualifier}User where UserID=@UserID

	select top 1
		@GuestUserID = a.UserID
	from
		{objectQualifier}User a,
		{objectQualifier}UserGroup b,
		{objectQualifier}Group c
	where
		b.UserID = a.UserID and
		b.GroupID = c.GroupID and
		(c.Flags & 2)<>0

	select 
		@GuestCount = count(1) 
	from 
		{objectQualifier}UserGroup a
		join [{databaseOwner}].{objectQualifier}Group b on b.GroupID=a.GroupID
	where
		(b.Flags & 2)<>0

	if @GuestUserID=@UserID and @GuestCount=1 begin
		return
	end

	update [{databaseOwner}].{objectQualifier}Message set UserName=@UserName,UserID=@GuestUserID where UserID=@UserID
	update [{databaseOwner}].{objectQualifier}Topic set UserName=@UserName,UserID=@GuestUserID where UserID=@UserID
	update [{databaseOwner}].{objectQualifier}Topic set LastUserName=@UserName,LastUserID=@GuestUserID where LastUserID=@UserID
	update [{databaseOwner}].{objectQualifier}Forum set LastUserName=@UserName,LastUserID=@GuestUserID where LastUserID=@UserID

	delete from [{databaseOwner}].{objectQualifier}EventLog where UserID=@UserID	
	delete from [{databaseOwner}].{objectQualifier}UserPMessage where UserID=@UserID
	delete from [{databaseOwner}].{objectQualifier}PMessage where FromUserID=@UserID AND PMessageID NOT IN (select PMessageID FROM [{databaseOwner}].{objectQualifier}PMessage)
	-- set messages as from guest so the User can be deleted
	update [{databaseOwner}].{objectQualifier}PMessage SET FromUserID = @GuestUserID WHERE FromUserID = @UserID
	delete from [{databaseOwner}].{objectQualifier}CheckEmail where UserID = @UserID
	delete from [{databaseOwner}].{objectQualifier}WatchTopic where UserID = @UserID
	delete from [{databaseOwner}].{objectQualifier}WatchForum where UserID = @UserID
	delete from [{databaseOwner}].{objectQualifier}UserGroup where UserID = @UserID
	--ABOT CHANGED
	--Delete UserForums entries Too 
	delete from [{databaseOwner}].{objectQualifier}UserForum where UserID = @UserID
	--END ABOT CHANGED 09.04.2004
	delete from [{databaseOwner}].{objectQualifier}User where UserID = @UserID
end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}user_deleteavatar](@UserID int) as begin
	update [{databaseOwner}].{objectQualifier}User set AvatarImage = null, Avatar = null where UserID = @UserID
=======
CREATE procedure [{databaseOwner}].[yaf_user_deleteavatar](@UserID int) as begin
	update yaf_User set AvatarImage = null, Avatar = null where UserID = @UserID
>>>>>>> .r1490
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_deleteold](@BoardID int) as
=======
create procedure [{databaseOwner}].[yaf_user_deleteold](@BoardID int) as
>>>>>>> .r1490
begin
	declare @Since datetime

	set @Since = getdate()

<<<<<<< .mine
	delete from [{databaseOwner}].{objectQualifier}EventLog  where UserID in(select UserID from [{databaseOwner}].{objectQualifier}User where BoardID=@BoardID and [{databaseOwner}].{objectQualifier}bitset(Flags,2)=0 and datediff(day,Joined,@Since)>2)
	delete from [{databaseOwner}].{objectQualifier}CheckEmail where UserID in(select UserID from [{databaseOwner}].{objectQualifier}User where BoardID=@BoardID and [{databaseOwner}].{objectQualifier}bitset(Flags,2)=0 and datediff(day,Joined,@Since)>2)
	delete from [{databaseOwner}].{objectQualifier}UserGroup where UserID in(select UserID from [{databaseOwner}].{objectQualifier}User where BoardID=@BoardID and [{databaseOwner}].{objectQualifier}bitset(Flags,2)=0 and datediff(day,Joined,@Since)>2)
	delete from [{databaseOwner}].{objectQualifier}User where BoardID=@BoardID and [{databaseOwner}].{objectQualifier}bitset(Flags,2)=0 and datediff(day,Joined,@Since)>2
=======
	delete from yaf_EventLog  where UserID in(select UserID from yaf_User where BoardID=@BoardID and {databaseOwner}.yaf_bitset(Flags,2)=0 and datediff(day,Joined,@Since)>2)
	delete from yaf_CheckEmail where UserID in(select UserID from yaf_User where BoardID=@BoardID and {databaseOwner}.yaf_bitset(Flags,2)=0 and datediff(day,Joined,@Since)>2)
	delete from yaf_UserGroup where UserID in(select UserID from yaf_User where BoardID=@BoardID and {databaseOwner}.yaf_bitset(Flags,2)=0 and datediff(day,Joined,@Since)>2)
	delete from yaf_User where BoardID=@BoardID and {databaseOwner}.yaf_bitset(Flags,2)=0 and datediff(day,Joined,@Since)>2
>>>>>>> .r1490
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_emails](@BoardID int,@GroupID int=null) as
=======
create procedure [{databaseOwner}].[yaf_user_emails](@BoardID int,@GroupID int=null) as
>>>>>>> .r1490
begin
	if @GroupID = 0 set @GroupID = null
	if @GroupID is null
		select 
			a.Email 
		from 
			{objectQualifier}User a
		where 
			a.Email is not null and 
			a.BoardID = @BoardID and
			a.Email is not null and 
			a.Email<>''
	else
		select 
			a.Email 
		from 
			{objectQualifier}User a join [{databaseOwner}].{objectQualifier}UserGroup b on b.UserID=a.UserID
		where 
			b.GroupID = @GroupID and 
			a.Email is not null and 
			a.Email<>''
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_find](@BoardID int,@Filter bit,@UserName nvarchar(50)=null,@Email nvarchar(50)=null) as
=======
create procedure [{databaseOwner}].[yaf_user_find](@BoardID int,@Filter bit,@UserName nvarchar(50)=null,@Email nvarchar(50)=null) as
>>>>>>> .r1490
begin
	if @Filter<>0
	begin
		if @UserName is not null
			set @UserName = '%' + @UserName + '%'

		select 
			a.*,
			IsGuest = (select count(1) from [{databaseOwner}].{objectQualifier}UserGroup x,{objectQualifier}Group y where x.UserID=a.UserID and x.GroupID=y.GroupID and (y.Flags & 2)<>0)
		from 
			{objectQualifier}User a
		where 
			a.BoardID=@BoardID and
			(@UserName is not null and a.Name like @UserName) or (@Email is not null and Email like @Email)
		order by
			a.Name
	end else
	begin
		select 
			a.UserID,
			IsGuest = (select count(1) from [{databaseOwner}].{objectQualifier}UserGroup x,{objectQualifier}Group y where x.UserID=a.UserID and x.GroupID=y.GroupID and (y.Flags & 2)<>0)
		from 
			{objectQualifier}User a
		where 
			a.BoardID=@BoardID and
			((@UserName is not null and a.Name=@UserName) or (@Email is not null and Email=@Email))
	end
end
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_getpoints] (@UserID int) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_user_getpoints] (@UserID int) AS
>>>>>>> .r1490
BEGIN
	SELECT Points FROM [{databaseOwner}].{objectQualifier}User WHERE UserID = @UserID
END
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_getsignature](@UserID int) as
=======
create procedure [{databaseOwner}].[yaf_user_getsignature](@UserID int) as
>>>>>>> .r1490
begin
	select Signature from [{databaseOwner}].{objectQualifier}User where UserID = @UserID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_guest]
=======
create procedure [{databaseOwner}].[yaf_user_guest]
>>>>>>> .r1490
(
	@BoardID int
)
as
begin
	select top 1
		a.UserID
	from
		{objectQualifier}User a,
		{objectQualifier}UserGroup b,
		{objectQualifier}Group c
	where
		b.UserID = a.UserID and
		b.GroupID = c.GroupID and
		a.BoardID = @BoardID and
		(c.Flags & 2)<>0
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_list](@BoardID int,@UserID int=null,@Approved bit=null,@GroupID int=null,@RankID int=null) as
=======
create procedure [{databaseOwner}].[yaf_user_list](@BoardID int,@UserID int=null,@Approved bit=null,@GroupID int=null,@RankID int=null) as
>>>>>>> .r1490
begin
	if @UserID is not null
		select 
			a.*,
			a.NumPosts,
			b.RankID,
			RankName = b.Name,
			NumDays = datediff(d,a.Joined,getdate())+1,
			NumPostsForum = (select count(1) from [{databaseOwner}].{objectQualifier}Message x where (x.Flags & 24)=16),
			HasAvatarImage = (select count(1) from [{databaseOwner}].{objectQualifier}User x where x.UserID=a.UserID and AvatarImage is not null),
			IsAdmin	= IsNull(c.IsAdmin,0),
			IsGuest	= IsNull(a.Flags & 4,0),
			IsHostAdmin	= IsNull(a.Flags & 1,0),
			IsForumModerator	= IsNull(c.IsForumModerator,0),
			IsModerator		= IsNull(c.IsModerator,0)
		from 
			{objectQualifier}User a
			join [{databaseOwner}].{objectQualifier}Rank b on b.RankID=a.RankID
			left join [{databaseOwner}].{objectQualifier}vaccess c on c.UserID=a.UserID
		where 
			a.UserID = @UserID and
			a.BoardID = @BoardID and
			IsNull(c.ForumID,0) = 0 and
			(@Approved is null or (@Approved=0 and (a.Flags & 2)=0) or (@Approved=1 and (a.Flags & 2)=2))
		order by 
			a.Name
	else if @GroupID is null and @RankID is null
		select 
			a.*,
			a.NumPosts,
			IsAdmin = (select count(1) from [{databaseOwner}].{objectQualifier}UserGroup x,{objectQualifier}Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and (y.Flags & 1)<>0),
			IsGuest	= IsNull(a.Flags & 4,0),
			IsHostAdmin	= IsNull(a.Flags & 1,0),
			b.RankID,
			RankName = b.Name
		from 
			{objectQualifier}User a
			join [{databaseOwner}].{objectQualifier}Rank b on b.RankID=a.RankID
		where 
			a.BoardID = @BoardID and
			(@Approved is null or (@Approved=0 and (a.Flags & 2)=0) or (@Approved=1 and (a.Flags & 2)=2))
		order by 
			a.Name
	else
		select 
			a.*,
			a.NumPosts,
			IsAdmin = (select count(1) from [{databaseOwner}].{objectQualifier}UserGroup x,{objectQualifier}Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and (y.Flags & 1)<>0),
			IsGuest	= IsNull(a.Flags & 4,0),
			IsHostAdmin	= IsNull(a.Flags & 1,0),
			b.RankID,
			RankName = b.Name
		from 
			{objectQualifier}User a
			join [{databaseOwner}].{objectQualifier}Rank b on b.RankID=a.RankID
		where 
			a.BoardID = @BoardID and
			(@Approved is null or (@Approved=0 and (a.Flags & 2)=0) or (@Approved=1 and (a.Flags & 2)=2)) and
			(@GroupID is null or exists(select 1 from [{databaseOwner}].{objectQualifier}UserGroup x where x.UserID=a.UserID and x.GroupID=@GroupID)) and
			(@RankID is null or a.RankID=@RankID)
		order by 
			a.Name
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_login](@BoardID int,@Name nvarchar(50),@Password nvarchar(32)) as
=======
create procedure [{databaseOwner}].[yaf_user_login](@BoardID int,@Name nvarchar(50),@Password nvarchar(32)) as
>>>>>>> .r1490
begin
	declare @UserID int

	-- Try correct board first
	if exists(select UserID from [{databaseOwner}].{objectQualifier}User where Name=@Name and Password=@Password and BoardID=@BoardID and (Flags & 2)=2)
	begin
		select UserID from [{databaseOwner}].{objectQualifier}User where Name=@Name and Password=@Password and BoardID=@BoardID and (Flags & 2)=2
		return
	end

	if not exists(select UserID from [{databaseOwner}].{objectQualifier}User where Name=@Name and Password=@Password and (BoardID=@BoardID or (Flags & 3)=3))
		set @UserID=null
	else
		select 
			@UserID=UserID 
		from 
			{objectQualifier}User 
		where 
			Name=@Name and 
			Password=@Password and 
			(BoardID=@BoardID or (Flags & 1)=1) and
			(Flags & 2)=2

	select @UserID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_nntp](@BoardID int,@UserName nvarchar(50),@Email nvarchar(50)) as
=======
create procedure [{databaseOwner}].[yaf_user_nntp](@BoardID int,@UserName nvarchar(50),@Email nvarchar(50)) as
>>>>>>> .r1490
begin
	declare @UserID int

	set @UserName = @UserName + ' (NNTP)'

	select
		@UserID=UserID
	from
		{objectQualifier}User
	where
		BoardID=@BoardID and
		Name=@UserName

	if @@ROWCOUNT<1
	begin
		exec [{databaseOwner}].{objectQualifier}user_save 0,@BoardID,@UserName,@Email,null,'Usenet',0,null,null,null,0,1,null,null,null,null,null,null,null,0,null,null,null,null,null
		-- The next one is not safe, but this procedure is only used for testing
		select @UserID=max(UserID) from [{databaseOwner}].{objectQualifier}User
	end

	select UserID=@UserID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_recoverpassword](@BoardID int,@UserName nvarchar(50),@Email nvarchar(50)) as
=======
create procedure [{databaseOwner}].[yaf_user_recoverpassword](@BoardID int,@UserName nvarchar(50),@Email nvarchar(50)) as
>>>>>>> .r1490
begin
	declare @UserID int
	select @UserID = UserID from [{databaseOwner}].{objectQualifier}User where BoardID = @BoardID and Name = @UserName and Email = @Email
	if @UserID is null begin
		select UserID = convert(int,null)
		return
	end else
	begin
		select UserID = @UserID
	end
end
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_removepoints] (@UserID int,@Points int) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_user_removepoints] (@UserID int,@Points int) AS
>>>>>>> .r1490
BEGIN
	UPDATE [{databaseOwner}].{objectQualifier}User SET Points = Points - @Points WHERE UserID = @UserID
END
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_removepointsbytopicid] (@TopicID int,@Points int) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_user_removepointsbytopicid] (@TopicID int,@Points int) AS
>>>>>>> .r1490
BEGIN
	declare @UserID int
	select @UserID = UserID from [{databaseOwner}].{objectQualifier}Topic where TopicID = @TopicID
	update [{databaseOwner}].{objectQualifier}user SET points = points - @Points WHERE userid = @UserID
END
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_resetpoints] AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_user_resetpoints] AS
>>>>>>> .r1490
BEGIN
	UPDATE [{databaseOwner}].{objectQualifier}User SET Points = NumPosts * 3
END
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}user_save](
=======
CREATE procedure [{databaseOwner}].[yaf_user_save](
>>>>>>> .r1490
	@UserID				int,
	@BoardID			int,
	@UserName			nvarchar(50) = null,
	@Email				nvarchar(50) = null,
	@TimeZone			int,
	@LanguageFile		nvarchar(50) = null,
	@ThemeFile			nvarchar(50) = null,
	@OverrideDefaultTheme	bit = null,
	@Approved			bit = null,
	@PMNotification		bit = null,
	@ProviderUserKey	uniqueidentifier = null)
AS
begin
	declare @RankID int
	declare @Flags int
	
	set @Flags = 0
	if @Approved<>0 set @Flags = @Flags | 2
	
	if @PMNotification is null SET @PMNotification = 1
	if @OverrideDefaultTheme is null SET @OverrideDefaultTheme=0

	if @UserID is null or @UserID<1 begin
		if @Email = '' set @Email = null
		
		select @RankID = RankID from [{databaseOwner}].{objectQualifier}Rank where (Flags & 1)<>0 and BoardID=@BoardID

		insert into [{databaseOwner}].{objectQualifier}User(BoardID,RankID,Name,Password,Email,Joined,LastVisit,NumPosts,TimeZone,Flags,PMNotification,ProviderUserKey) 
		values(@BoardID,@RankID,@UserName,'-',@Email,getdate(),getdate(),0,@TimeZone,@Flags,@PMNotification,@ProviderUserKey)		
	
		set @UserID = SCOPE_IDENTITY()

		insert into [{databaseOwner}].{objectQualifier}UserGroup(UserID,GroupID) select @UserID,GroupID from [{databaseOwner}].{objectQualifier}Group where BoardID=@BoardID and (Flags & 4)<>0
	end
	else begin
		update [{databaseOwner}].{objectQualifier}User set
			TimeZone = @TimeZone,
			LanguageFile = @LanguageFile,
			ThemeFile = @ThemeFile,
			OverrideDefaultThemes = @OverrideDefaultTheme,
			PMNotification = @PMNotification
		where UserID = @UserID
		
		if @Email is not null
			update [{databaseOwner}].{objectQualifier}User set Email = @Email where UserID = @UserID
	end
end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].[{objectQualifier}user_saveavatar]
=======
CREATE procedure [{databaseOwner}].[yaf_user_saveavatar]
>>>>>>> .r1490
(
	@UserID int,
	@Avatar nvarchar(255) = NULL,
	@AvatarImage image = NULL
)
AS
BEGIN
	IF @Avatar IS NOT NULL 
	BEGIN
		UPDATE [{databaseOwner}].{objectQualifier}User SET Avatar = @Avatar, AvatarImage = null WHERE UserID = @UserID
	END
	ELSE IF @AvatarImage IS NOT NULL 
	BEGIN
		UPDATE [{databaseOwner}].{objectQualifier}User SET AvatarImage = @AvatarImage, Avatar = null WHERE UserID = @UserID
	END
END

GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_savepassword](@UserID int,@Password nvarchar(32)) as
=======
create procedure [{databaseOwner}].[yaf_user_savepassword](@UserID int,@Password nvarchar(32)) as
>>>>>>> .r1490
begin
<<<<<<< .mine
	update [{databaseOwner}].{objectQualifier}User set Password = @Password where UserID = @UserID
=======
	update {databaseOwner}.yaf_User set Password = @Password where UserID = @UserID
>>>>>>> .r1490
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_savesignature](@UserID int,@Signature ntext) as
=======
create procedure [{databaseOwner}].[yaf_user_savesignature](@UserID int,@Signature ntext) as
>>>>>>> .r1490
begin
	update [{databaseOwner}].{objectQualifier}User set Signature = @Signature where UserID = @UserID
end
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_setpoints] (@UserID int,@Points int) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_user_setpoints] (@UserID int,@Points int) AS
>>>>>>> .r1490
BEGIN
	UPDATE [{databaseOwner}].{objectQualifier}User SET Points = @Points WHERE UserID = @UserID
END
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_setrole](@BoardID int,@ProviderUserKey uniqueidentifier,@Role nvarchar(50)) as
=======
create procedure [{databaseOwner}].[yaf_user_setrole](@BoardID int,@ProviderUserKey uniqueidentifier,@Role nvarchar(50)) as
>>>>>>> .r1490
begin
	declare @UserID int, @GroupID int
	
<<<<<<< .mine
	select @UserID=UserID from [{databaseOwner}].{objectQualifier}User where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey
=======
	select @UserID=UserID from {databaseOwner}.yaf_User where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey
>>>>>>> .r1490

	if @Role is null
	begin
<<<<<<< .mine
		delete from [{databaseOwner}].{objectQualifier}UserGroup where UserID=@UserID
=======
		delete from {databaseOwner}.yaf_UserGroup where UserID=@UserID
>>>>>>> .r1490
	end else
	begin
<<<<<<< .mine
		if not exists(select 1 from [{databaseOwner}].{objectQualifier}Group where BoardID=@BoardID and Name=@Role)
=======
		if not exists(select 1 from {databaseOwner}.yaf_Group where BoardID=@BoardID and Name=@Role)
>>>>>>> .r1490
		begin
<<<<<<< .mine
			insert into [{databaseOwner}].{objectQualifier}Group(Name,BoardID,Flags)
=======
			insert into {databaseOwner}.yaf_Group(Name,BoardID,Flags)
>>>>>>> .r1490
			values(@Role,@BoardID,0);
			set @GroupID = SCOPE_IDENTITY()

			insert into [{databaseOwner}].{objectQualifier}ForumAccess(GroupID,ForumID,AccessMaskID)
			select
				@GroupID,
				a.ForumID,
				min(a.AccessMaskID)
			from
<<<<<<< .mine
				[{databaseOwner}].{objectQualifier}ForumAccess a
				join [{databaseOwner}].{objectQualifier}Group b on b.GroupID=a.GroupID
=======
				{databaseOwner}.yaf_ForumAccess a
				join {databaseOwner}.yaf_Group b on b.GroupID=a.GroupID
>>>>>>> .r1490
			where
				b.BoardID=@BoardID and
				(b.Flags & 4)=4
			group by
				a.ForumID
		end else
		begin
<<<<<<< .mine
			select @GroupID = GroupID from [{databaseOwner}].{objectQualifier}Group where BoardID=@BoardID and Name=@Role
=======
			select @GroupID = GroupID from {databaseOwner}.yaf_Group where BoardID=@BoardID and Name=@Role
>>>>>>> .r1490
		end
<<<<<<< .mine
		insert into [{databaseOwner}].{objectQualifier}UserGroup(UserID,GroupID) values(@UserID,@GroupID)
=======
		insert into {databaseOwner}.yaf_UserGroup(UserID,GroupID) values(@UserID,@GroupID)
>>>>>>> .r1490
	end
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_suspend](@UserID int,@Suspend datetime=null) as
=======
create procedure [{databaseOwner}].[yaf_user_suspend](@UserID int,@Suspend datetime=null) as
>>>>>>> .r1490
begin
	update [{databaseOwner}].{objectQualifier}User set Suspended = @Suspend where UserID=@UserID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}user_upgrade](@UserID int) as
=======
create procedure [{databaseOwner}].[yaf_user_upgrade](@UserID int) as
>>>>>>> .r1490
begin
	declare @RankID		int
	declare @Flags		int
	declare @MinPosts	int
	declare @NumPosts	int
	-- Get user and rank information
	select
		@RankID = b.RankID,
		@Flags = b.Flags,
		@MinPosts = b.MinPosts,
		@NumPosts = a.NumPosts
	from
		{objectQualifier}User a,
		{objectQualifier}Rank b
	where
		a.UserID = @UserID and
		b.RankID = a.RankID
	
	-- If user isn't member of a ladder rank, exit
	if (@Flags & 2) = 0 return
	
	-- See if user got enough posts for next ladder group
	select top 1
		@RankID = RankID
	from
		{objectQualifier}Rank
	where
		(Flags & 2) = 2 and
		MinPosts <= @NumPosts and
		MinPosts > @MinPosts
	order by
		MinPosts
	if @@ROWCOUNT=1
		update [{databaseOwner}].{objectQualifier}User set RankID = @RankID where UserID = @UserID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}userforum_delete](@UserID int,@ForumID int) as
=======
create procedure [{databaseOwner}].[yaf_userforum_delete](@UserID int,@ForumID int) as
>>>>>>> .r1490
begin
	delete from [{databaseOwner}].{objectQualifier}UserForum where UserID=@UserID and ForumID=@ForumID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}userforum_list](@UserID int=null,@ForumID int=null) as 
=======
create procedure [{databaseOwner}].[yaf_userforum_list](@UserID int=null,@ForumID int=null) as 
>>>>>>> .r1490
begin
	select 
		a.*,
		b.AccessMaskID,
		b.Accepted,
		Access = c.Name
	from
		{objectQualifier}User a join [{databaseOwner}].{objectQualifier}UserForum b on b.UserID=a.UserID
		join [{databaseOwner}].{objectQualifier}AccessMask c on c.AccessMaskID=b.AccessMaskID
	where
		(@UserID is null or a.UserID=@UserID) and
		(@ForumID is null or b.ForumID=@ForumID)
	order by
		a.Name	
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}userforum_save](@UserID int,@ForumID int,@AccessMaskID int) as
=======
create procedure [{databaseOwner}].[yaf_userforum_save](@UserID int,@ForumID int,@AccessMaskID int) as
>>>>>>> .r1490
begin
	if exists(select 1 from [{databaseOwner}].{objectQualifier}UserForum where UserID=@UserID and ForumID=@ForumID)
		update [{databaseOwner}].{objectQualifier}UserForum set AccessMaskID=@AccessMaskID where UserID=@UserID and ForumID=@ForumID
	else
		insert into [{databaseOwner}].{objectQualifier}UserForum(UserID,ForumID,AccessMaskID,Invited,Accepted) values(@UserID,@ForumID,@AccessMaskID,getdate(),1)
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}usergroup_list](@UserID int) as begin
=======
create procedure [{databaseOwner}].[yaf_usergroup_list](@UserID int) as begin
>>>>>>> .r1490
	select 
		b.GroupID,
		b.Name
	from
		{objectQualifier}UserGroup a
		join [{databaseOwner}].{objectQualifier}Group b on b.GroupID=a.GroupID
	where
		a.UserID = @UserID
	order by
		b.Name
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}usergroup_save](@UserID int,@GroupID int,@Member bit) as
=======
create procedure [{databaseOwner}].[yaf_usergroup_save](@UserID int,@GroupID int,@Member bit) as
>>>>>>> .r1490
begin
	if @Member=0
		delete from [{databaseOwner}].{objectQualifier}UserGroup where UserID=@UserID and GroupID=@GroupID
	else
		insert into [{databaseOwner}].{objectQualifier}UserGroup(UserID,GroupID)
		select @UserID,@GroupID
		where not exists(select 1 from [{databaseOwner}].{objectQualifier}UserGroup where UserID=@UserID and GroupID=@GroupID)
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}userpmessage_delete](@UserPMessageID int) as
=======
create procedure [{databaseOwner}].[yaf_userpmessage_delete](@UserPMessageID int) as
>>>>>>> .r1490
begin
	delete from [{databaseOwner}].{objectQualifier}UserPMessage where UserPMessageID=@UserPMessageID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}userpmessage_list](@UserPMessageID int) as
=======
create procedure [{databaseOwner}].[yaf_userpmessage_list](@UserPMessageID int) as
>>>>>>> .r1490
begin
	select
		a.*,
		FromUser = b.Name,
		ToUserID = c.UserID,
		ToUser = c.Name,
		d.IsRead,
		d.UserPMessageID
	from
		{objectQualifier}PMessage a,
		{objectQualifier}User b,
		{objectQualifier}User c,
		{objectQualifier}UserPMessage d
	where
		b.UserID = a.FromUserID and
		c.UserID = d.UserID and
		d.PMessageID = a.PMessageID and
		d.UserPMessageID = @UserPMessageID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}watchforum_add](@UserID int,@ForumID int) as
=======
create procedure [{databaseOwner}].[yaf_watchforum_add](@UserID int,@ForumID int) as
>>>>>>> .r1490
begin
	insert into [{databaseOwner}].{objectQualifier}WatchForum(ForumID,UserID,Created)
	select @ForumID, @UserID, getdate()
	where not exists(select 1 from [{databaseOwner}].{objectQualifier}WatchForum where ForumID=@ForumID and UserID=@UserID)
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}watchforum_check](@UserID int,@ForumID int) as
=======
create procedure [{databaseOwner}].[yaf_watchforum_check](@UserID int,@ForumID int) as
>>>>>>> .r1490
begin
	SELECT WatchForumID FROM [{databaseOwner}].{objectQualifier}WatchForum WHERE UserID = @UserID AND ForumID = @ForumID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}watchforum_delete](@WatchForumID int) as
=======
create procedure [{databaseOwner}].[yaf_watchforum_delete](@WatchForumID int) as
>>>>>>> .r1490
begin
	delete from [{databaseOwner}].{objectQualifier}WatchForum where WatchForumID = @WatchForumID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}watchforum_list](@UserID int) as
=======
create procedure [{databaseOwner}].[yaf_watchforum_list](@UserID int) as
>>>>>>> .r1490
begin
	select
		a.*,
		ForumName = b.Name,
		Messages = (select count(1) from [{databaseOwner}].{objectQualifier}Topic x, [{databaseOwner}].{objectQualifier}Message y where x.ForumID=a.ForumID and y.TopicID=x.TopicID),
		Topics = (select count(1) from [{databaseOwner}].{objectQualifier}Topic x where x.ForumID=a.ForumID and x.TopicMovedID is null),
		b.LastPosted,
		b.LastMessageID,
		LastTopicID = (select TopicID from [{databaseOwner}].{objectQualifier}Message x where x.MessageID=b.LastMessageID),
		b.LastUserID,
		LastUserName = IsNull(b.LastUserName,(select Name from [{databaseOwner}].{objectQualifier}User x where x.UserID=b.LastUserID))
	from
		{objectQualifier}WatchForum a,
		{objectQualifier}Forum b
	where
		a.UserID = @UserID and
		b.ForumID = a.ForumID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}watchtopic_add](@UserID int,@TopicID int) as
=======
create procedure [{databaseOwner}].[yaf_watchtopic_add](@UserID int,@TopicID int) as
>>>>>>> .r1490
begin
	insert into [{databaseOwner}].{objectQualifier}WatchTopic(TopicID,UserID,Created)
	select @TopicID, @UserID, getdate()
	where not exists(select 1 from [{databaseOwner}].{objectQualifier}WatchTopic where TopicID=@TopicID and UserID=@UserID)
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}watchtopic_check](@UserID int,@TopicID int) as
=======
create procedure [{databaseOwner}].[yaf_watchtopic_check](@UserID int,@TopicID int) as
>>>>>>> .r1490
begin
	SELECT WatchTopicID FROM [{databaseOwner}].{objectQualifier}WatchTopic WHERE UserID = @UserID AND TopicID = @TopicID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}watchtopic_delete](@WatchTopicID int) as
=======
create procedure [{databaseOwner}].[yaf_watchtopic_delete](@WatchTopicID int) as
>>>>>>> .r1490
begin
	delete from [{databaseOwner}].{objectQualifier}WatchTopic where WatchTopicID = @WatchTopicID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}watchtopic_list](@UserID int) as
=======
create procedure [{databaseOwner}].[yaf_watchtopic_list](@UserID int) as
>>>>>>> .r1490
begin
	select
		a.*,
		TopicName = b.Topic,
		Replies = (select count(1) from [{databaseOwner}].{objectQualifier}Message x where x.TopicID=b.TopicID),
		b.Views,
		b.LastPosted,
		b.LastMessageID,
		b.LastUserID,
		LastUserName = IsNull(b.LastUserName,(select Name from [{databaseOwner}].{objectQualifier}User x where x.UserID=b.LastUserID))
	from
		{objectQualifier}WatchTopic a,
		{objectQualifier}Topic b
	where
		a.UserID = @UserID and
		b.TopicID = a.TopicID
end
GO

<<<<<<< .mine
CREATE procedure [{databaseOwner}].{objectQualifier}message_reply_list(@MessageID int) as
=======
CREATE procedure {databaseOwner}.yaf_message_reply_list(@MessageID int) as
>>>>>>> .r1490
begin
	set nocount on
	select
                a.MessageID,
		a.Posted,
		Subject = c.Topic,
		a.Message,
		a.UserID,
		a.Flags,
		UserName = IsNull(a.UserName,b.Name),
		b.Signature
	from
		{objectQualifier}Message a,
		{objectQualifier}User b,
		{objectQualifier}Topic c
	where
		(a.Flags & 16)=16 and
		b.UserID = a.UserID and
		c.TopicID = a.TopicID and
		a.ReplyTo = @MessageID



end
GO


<<<<<<< .mine
CREATE procedure [{databaseOwner}].{objectQualifier}message_deleteundelete(@MessageID int, @isModeratorChanged bit, @DeleteReason nvarchar(100), @isDeleteAction int) as
=======
CREATE procedure {databaseOwner}.yaf_message_deleteundelete(@MessageID int, @isModeratorChanged bit, @DeleteReason nvarchar(100), @isDeleteAction int) as
>>>>>>> .r1490
begin
	declare @TopicID		int
	declare @ForumID		int
	declare @MessageCount	int
	declare @LastMessageID	int

	-- Find TopicID and ForumID
	select @TopicID=b.TopicID,@ForumID=b.ForumID from [{databaseOwner}].{objectQualifier}Message a,{objectQualifier}Topic b where a.MessageID=@MessageID and b.TopicID=a.TopicID

	-- Update LastMessageID in Topic and Forum
	update [{databaseOwner}].{objectQualifier}Topic set
		LastPosted = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	update [{databaseOwner}].{objectQualifier}Forum set
		LastPosted = null,
		LastTopicID = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	-- "Delete" message

        update [{databaseOwner}].{objectQualifier}Message set isModeratorChanged = @isModeratorChanged where MessageID = @MessageID and ((Flags & 8) <> @isDeleteAction*8)
        update [{databaseOwner}].{objectQualifier}Message set DeleteReason = @DeleteReason where MessageID = @MessageID and ((Flags & 8) <> @isDeleteAction*8)
        update [{databaseOwner}].{objectQualifier}Message set Flags = Flags ^ 8 where MessageID = @MessageID and ((Flags & 8) <> @isDeleteAction*8)

	-- Delete topic if there are no more messages
	select @MessageCount = count(1) from [{databaseOwner}].{objectQualifier}Message where TopicID = @TopicID and (Flags & 8)=0
	if @MessageCount=0 exec [{databaseOwner}].{objectQualifier}topic_delete @TopicID
	-- update lastpost
	exec [{databaseOwner}].{objectQualifier}topic_updatelastpost @ForumID,@TopicID
	exec [{databaseOwner}].{objectQualifier}forum_updatestats @ForumID
	-- update topic numposts
	update [{databaseOwner}].{objectQualifier}Topic set
		NumPosts = (select count(1) from [{databaseOwner}].{objectQualifier}Message x where x.TopicID={objectQualifier}Topic.TopicID and (x.Flags & 24)=16)
	where TopicID = @TopicID
end
GO

<<<<<<< .mine
create procedure [{databaseOwner}].[{objectQualifier}topic_create_by_message] (
=======
create procedure [{databaseOwner}].[yaf_topic_create_by_message] (
>>>>>>> .r1490
	@MessageID int,
	@ForumID	int,
	@Subject	nvarchar(100)
) as
begin


declare		@UserID		int
declare		@Posted		datetime

set @UserID = (select userid from [{databaseOwner}].{objectQualifier}message where messageid =  @MessageID)
set  @Posted  = (select  posted from [{databaseOwner}].{objectQualifier}message where messageid =  @MessageID)


	declare @TopicID int
	--declare @MessageID int

	if @Posted is null set @Posted = getdate()

	insert into [{databaseOwner}].{objectQualifier}Topic(ForumID,Topic,UserID,Posted,Views,Priority,PollID,UserName,NumPosts)
	values(@ForumID,@Subject,@UserID,@Posted,0,0,null,null,0)

	set @TopicID = @@IDENTITY
--	exec [{databaseOwner}].{objectQualifier}message_save @TopicID,@UserID,@Message,@UserName,@IP,@Posted,null,@Flags,@MessageID output
	select TopicID = @TopicID, MessageID = @MessageID
END
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_move] (@MessageID int, @MoveToTopic int) AS
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_message_move] (@MessageID int, @MoveToTopic int) AS
>>>>>>> .r1490
BEGIN
DECLARE
	@Position int,
	@ReplyToID int,
	@OldTopicID int,
	@OldForumID int

	
	declare @NewForumID		int
	declare @MessageCount	int
	declare @LastMessageID	int

	-- Find TopicID and ForumID
--	select @OldTopicID=b.TopicID,@ForumID=b.ForumID from [{databaseOwner}].{objectQualifier}Message a,{objectQualifier}Topic b where a.MessageID=@MessageID and b.TopicID=a.TopicID

SET 	@NewForumID = (SELECT     ForumId
				FROM         [{databaseOwner}].{objectQualifier}Topic
				WHERE     (TopicId = @MoveToTopic))


SET 	@OldTopicID = 	(SELECT     TopicID
				FROM         [{databaseOwner}].{objectQualifier}Message
				WHERE     (MessageID = @MessageID))

SET 	@OldForumID = (SELECT     ForumId
				FROM         [{databaseOwner}].{objectQualifier}Topic
				WHERE     (TopicId = @OldTopicID))

SET	@ReplyToID = (SELECT     MessageID
			FROM         [{databaseOwner}].{objectQualifier}Message
			WHERE     ([Position] = 0) AND (TopicID = @MoveToTopic))

SET	@Position = 	(SELECT     MAX([Position]) + 1 AS Expr1
			FROM         [{databaseOwner}].{objectQualifier}Message
			WHERE     (TopicID = @MoveToTopic) and posted < (select posted from [{databaseOwner}].{objectQualifier}Message where messageid = @MessageID ) )

if @Position is null  set @Position = 0

update [{databaseOwner}].{objectQualifier}Message set
		Position = Position+1
	 WHERE     (TopicID = @MoveToTopic) and posted > (select posted from [{databaseOwner}].{objectQualifier}Message where messageid = @MessageID)

update [{databaseOwner}].{objectQualifier}Message set
		Position = Position-1
	 WHERE     (TopicID = @OldTopicID) and posted > (select posted from [{databaseOwner}].{objectQualifier}Message where messageid = @MessageID)

	-- Update LastMessageID in Topic and Forum
	update [{databaseOwner}].{objectQualifier}Topic set
		LastPosted = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	update [{databaseOwner}].{objectQualifier}Forum set
		LastPosted = null,
		LastTopicID = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID


UPDATE [{databaseOwner}].{objectQualifier}Message SET
 	TopicID = @MoveToTopic,
	ReplyTo = @ReplyToID,
	[Position] = @Position
WHERE  MessageID = @MessageID

	-- Delete topic if there are no more messages
	select @MessageCount = count(1) from [{databaseOwner}].{objectQualifier}Message where TopicID = @OldTopicID and (Flags & 8)=0
	if @MessageCount=0 exec [{databaseOwner}].{objectQualifier}topic_delete @OldTopicID

	-- update lastpost
	exec [{databaseOwner}].{objectQualifier}topic_updatelastpost @OldForumID,@OldTopicID
	exec [{databaseOwner}].{objectQualifier}topic_updatelastpost @NewForumID,@MoveToTopic

	-- update topic numposts
	update [{databaseOwner}].{objectQualifier}Topic set
		NumPosts = (select count(1) from [{databaseOwner}].{objectQualifier}Message x where x.TopicID={objectQualifier}Topic.TopicID and (x.Flags & 24)=16)
	where TopicID = @OldTopicID
	update [{databaseOwner}].{objectQualifier}Topic set
		NumPosts = (select count(1) from [{databaseOwner}].{objectQualifier}Message x where x.TopicID={objectQualifier}Topic.TopicID and (x.Flags & 24)=16)
	where TopicID = @MoveToTopic

	exec [{databaseOwner}].{objectQualifier}forum_updatelastpost @NewForumID
	exec [{databaseOwner}].{objectQualifier}forum_updatestats @NewForumID
	exec [{databaseOwner}].{objectQualifier}forum_updatelastpost @OldForumID
	exec [{databaseOwner}].{objectQualifier}forum_updatestats @OldForumID

END
GO

<<<<<<< .mine
create proc [{databaseOwner}].[{objectQualifier}forum_resync]
=======
create proc [{databaseOwner}].[yaf_forum_resync]
>>>>>>> .r1490
	@BoardID int,
	@ForumID int = null
AS
begin
	if (@ForumID is null) begin
		declare curForums cursor for
			select 
				a.ForumID
			from
				{objectQualifier}Forum a
				JOIN [{databaseOwner}].{objectQualifier}Category b on a.CategoryID=b.CategoryID
				JOIN [{databaseOwner}].{objectQualifier}Board c on b.BoardID = c.BoardID  
			where
				c.BoardID=@BoardID

		open curForums
		
		-- cycle through forums
		fetch next from curForums into @ForumID
		while @@FETCH_STATUS = 0
		begin
			--update statistics
<<<<<<< .mine
			exec [{databaseOwner}].{objectQualifier}forum_updatestats @ForumID
=======
			exec {databaseOwner}.yaf_forum_updatestats @ForumID
>>>>>>> .r1490
			--update last post
<<<<<<< .mine
			exec [{databaseOwner}].{objectQualifier}forum_updatelastpost @ForumID
=======
			exec {databaseOwner}.yaf_forum_updatelastpost @ForumID
>>>>>>> .r1490

			fetch next from curForums into @ForumID
		end
		close curForums
		deallocate curForums
	end
	else begin
		--update statistics
<<<<<<< .mine
		exec [{databaseOwner}].{objectQualifier}forum_updatestats @ForumID
=======
		exec {databaseOwner}.yaf_forum_updatestats @ForumID
>>>>>>> .r1490
		--update last post
<<<<<<< .mine
		exec [{databaseOwner}].{objectQualifier}forum_updatelastpost @ForumID
=======
		exec {databaseOwner}.yaf_forum_updatelastpost @ForumID
>>>>>>> .r1490
	end
end
GO

<<<<<<< .mine
create proc [{databaseOwner}].[{objectQualifier}board_resync]
=======
create proc [{databaseOwner}].[yaf_board_resync]
>>>>>>> .r1490
	@BoardID int = null
as
begin
	if (@BoardID is null) begin
		declare curBoards cursor for
			select BoardID from	{objectQualifier}Board

		open curBoards
		
		-- cycle through forums
		fetch next from curBoards into @BoardID
		while @@FETCH_STATUS = 0
		begin
			--resync board forums
<<<<<<< .mine
			exec [{databaseOwner}].{objectQualifier}forum_resync @BoardID
=======
			exec {databaseOwner}.yaf_forum_resync @BoardID
>>>>>>> .r1490

			fetch next from curBoards into @BoardID
		end
		close curBoards
		deallocate curBoards
	end
	else begin
		--resync board forums
<<<<<<< .mine
		exec [{databaseOwner}].{objectQualifier}forum_resync @BoardID
=======
		exec {databaseOwner}.yaf_forum_resync @BoardID
>>>>>>> .r1490
	end
end
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}category_simplelist](
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_category_simplelist](
>>>>>>> .r1490
                @StartID INT  = 0,
                @Limit   INT  = 500)
AS
    BEGIN
        SET ROWCOUNT  @Limit
        SELECT   c.[CategoryID],
                 c.[Name]
        FROM     [{databaseOwner}].{objectQualifier}Category c
        WHERE    c.[CategoryID] >= @StartID
        AND c.[CategoryID] < (@StartID + @Limit)
        ORDER BY c.[CategoryID]
        SET ROWCOUNT  0
    END
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}forum_simplelist](
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_forum_simplelist](
>>>>>>> .r1490
                @StartID INT  = 0,
                @Limit   INT  = 500)
AS
    BEGIN
        SET ROWCOUNT  @Limit
        SELECT   f.[ForumID],
                 f.[Name]
        FROM     [{databaseOwner}].{objectQualifier}Forum f
        WHERE    f.[ForumID] >= @StartID
        AND f.[ForumID] < (@StartID + @Limit)
        ORDER BY f.[ForumID]
        SET ROWCOUNT  0
    END
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_simplelist](
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_message_simplelist](
>>>>>>> .r1490
                @StartID INT  = 0,
                @Limit   INT  = 1000)
AS
    BEGIN
        SET ROWCOUNT  @Limit
        SELECT   m.[MessageID],
                 m.[TopicID]
        FROM     [{databaseOwner}].{objectQualifier}Message m
        WHERE    m.[MessageID] >= @StartID
        AND m.[MessageID] < (@StartID + @Limit)
        AND m.[TopicID] IS NOT NULL
        ORDER BY m.[MessageID]
        SET ROWCOUNT  0
    END
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}topic_simplelist](
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_topic_simplelist](
>>>>>>> .r1490
                @StartID INT  = 0,
                @Limit   INT  = 500)
AS
    BEGIN
        SET ROWCOUNT  @Limit
        SELECT   t.[TopicID],
                 t.[Topic]
        FROM     [{databaseOwner}].{objectQualifier}Topic t
        WHERE    t.[TopicID] >= @StartID
        AND t.[TopicID] < (@StartID + @Limit)
        ORDER BY t.[TopicID]
        SET ROWCOUNT  0
    END
GO

<<<<<<< .mine
CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_simplelist](
=======
CREATE PROCEDURE [{databaseOwner}].[yaf_user_simplelist](
>>>>>>> .r1490
                @StartID INT  = 0,
                @Limit   INT  = 500)
AS
    BEGIN
        SET ROWCOUNT  @Limit
        SELECT   a.[UserID],
                 a.[Name]
        FROM     [{databaseOwner}].{objectQualifier}User a
        WHERE    a.[UserID] >= @StartID
        AND a.[UserID] < (@StartID + @Limit)
        ORDER BY a.[UserID]
        SET ROWCOUNT  0
    END
GO