/*
  YAF SQL Stored Procedures File Created 03/01/06
	

  Remove Comments RegEx: \/\*(.*)\*\/
  Remove Extra Stuff: SET ANSI_NULLS ON\nGO\nSET QUOTED_IDENTIFIER ON\nGO\n\n\n 
*/

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}accessmask_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}accessmask_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}accessmask_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}accessmask_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}accessmask_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}accessmask_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}active_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}active_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}active_listforum]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}active_listforum]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}active_listtopic]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}active_listtopic]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}active_stats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}active_stats]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}active_updatemaxstats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}active_updatemaxstats]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}attachment_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}attachment_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}attachment_download]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}attachment_download]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}attachment_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}attachment_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}attachment_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}attachment_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bannedip_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bannedip_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bannedip_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bannedip_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bannedip_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bannedip_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_create]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_create]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_poststats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_poststats]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_resync]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_resync]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_stats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_stats]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}category_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}category_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}category_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}category_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}category_listread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}category_listread]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}category_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}category_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}checkemail_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}checkemail_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}checkemail_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}checkemail_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}checkemail_update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}checkemail_update]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}choice_add]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}choice_add]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}choice_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}choice_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}choice_update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}choice_update]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}choice_vote]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}choice_vote]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}eventlog_create]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}eventlog_create]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}eventlog_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}eventlog_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}eventlog_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}eventlog_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}extension_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}extension_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}extension_edit]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}extension_edit]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}extension_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}extension_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}extension_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}extension_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listall]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listall]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listall_fromcat]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listall_fromcat]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listallmymoderated]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listallmymoderated]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listpath]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listpath]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listread]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listSubForums]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listSubForums]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listtopics]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listtopics]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_moderatelist]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_moderatelist]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_moderators]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_moderators]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_resync]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_resync]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_updatelastpost]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_updatelastpost]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_updatestats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_updatestats]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forumaccess_group]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forumaccess_group]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forumaccess_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forumaccess_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forumaccess_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forumaccess_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_medal_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_medal_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_medal_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_medal_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_medal_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_medal_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_member]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_member]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}mail_create]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}mail_create]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}mail_createwatch]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}mail_createwatch]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}mail_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}mail_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}mail_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}mail_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}medal_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}medal_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}medal_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}medal_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}medal_listusers]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}medal_listusers]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}medal_resort]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}medal_resort]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}medal_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}medal_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_approve]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_approve]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_findunread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_findunread]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_getReplies]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_getReplies]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_listreported]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_listreported]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_report]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_report]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_reportcopyover]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_reportcopyover]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_reportresolve]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_reportresolve]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_unapproved]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_unapproved]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_update]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpforum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpforum_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpforum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpforum_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpforum_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpforum_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpforum_update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpforum_update]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpserver_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpserver_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpserver_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpserver_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpserver_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpserver_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntptopic_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntptopic_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntptopic_savemessage]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntptopic_savemessage]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pageload]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pageload]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_info]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_info]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_markread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_markread]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_prune]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_prune]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_archive]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_archive]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}poll_remove]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}poll_remove]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}poll_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}poll_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}poll_stats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}poll_stats]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}poll_update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}poll_update]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pollvote_check]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pollvote_check]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}post_last10user]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}post_last10user]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}post_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}post_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}post_list_reverse10]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}post_list_reverse10]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}rank_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}rank_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}rank_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}rank_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}rank_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}rank_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}registry_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}registry_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}registry_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}registry_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}replace_words_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}replace_words_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}replace_words_edit]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}replace_words_edit]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}replace_words_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}replace_words_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}replace_words_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}replace_words_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}smiley_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}smiley_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}smiley_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}smiley_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}smiley_listunique]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}smiley_listunique]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}smiley_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}smiley_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}smiley_resort]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}smiley_resort]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}system_initialize]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}system_initialize]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}system_updateversion]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}system_updateversion]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_active]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_active]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_findnext]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_findnext]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_findprev]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_findprev]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_info]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_info]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_announcements]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_announcements]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_latest]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_latest]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_listmessages]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_listmessages]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_lock]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_lock]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_move]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_move]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_poll_update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_poll_update]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_prune]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_prune]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_updatelastpost]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_updatelastpost]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_accessmasks]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_accessmasks]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_activity_rank]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_activity_rank]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_addpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_addpoints]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_adminsave]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_adminsave]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_approve]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_approve]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_approveall]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_approveall]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_aspnet]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_aspnet]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_migrate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_migrate]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_avatarimage]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_avatarimage]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_changepassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_changepassword]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_pmcount]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_pmcount]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_deleteavatar]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_deleteavatar]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_deleteold]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_deleteold]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_emails]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_emails]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_find]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_find]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_getpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_getpoints]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_getsignature]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_getsignature]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_guest]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_guest]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_listmedals]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_listmedals]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_login]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_login]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_medal_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_medal_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_medal_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_medal_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_medal_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_medal_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_nntp]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_nntp]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_recoverpassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_recoverpassword]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_removepoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_removepoints]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_removepointsbytopicid]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_removepointsbytopicid]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_resetpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_resetpoints]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_saveavatar]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_saveavatar]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_savepassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_savepassword]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_savesignature]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_savesignature]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_setpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_setpoints]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_setrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_setrole]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_suspend]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_suspend]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_upgrade]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_upgrade]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}userforum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}userforum_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}userforum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}userforum_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}userforum_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}userforum_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}usergroup_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}usergroup_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}usergroup_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}usergroup_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}userpmessage_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}userpmessage_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}userpmessage_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}userpmessage_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchforum_add]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchforum_add]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchforum_check]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchforum_check]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchforum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchforum_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchforum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchforum_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchtopic_add]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchtopic_add]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchtopic_check]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchtopic_check]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchtopic_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchtopic_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchtopic_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchtopic_list]
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_reply_list]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_reply_list]
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = object_id(N'[{databaseOwner}].[{objectQualifier}message_deleteundelete]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_deleteundelete]
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = object_id(N'[{databaseOwner}].[{objectQualifier}topic_create_by_message]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_create_by_message]
GO

IF EXISTS (SELECT 1 FROM sysobjects where id = object_id(N'[{databaseOwner}].[{objectQualifier}message_move]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_move]
GO

IF EXISTS (SELECT * FROM   dbo.sysobjects
           WHERE  id = Object_id(N'[{databaseOwner}].[{objectQualifier}category_simplelist]')
           AND Objectproperty(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}category_simplelist] 
GO

IF EXISTS (SELECT *
           FROM   dbo.sysobjects
           WHERE  id = Object_id(N'[{databaseOwner}].[{objectQualifier}forum_simplelist]')
           AND Objectproperty(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_simplelist] 
GO

IF EXISTS (SELECT *
           FROM   dbo.sysobjects
           WHERE  id = Object_id(N'[{databaseOwner}].[{objectQualifier}message_simplelist]')
           AND Objectproperty(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_simplelist] 
GO

IF EXISTS (SELECT *
           FROM   dbo.sysobjects
           WHERE  id = Object_id(N'[{databaseOwner}].[{objectQualifier}topic_simplelist]')
           AND Objectproperty(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_simplelist] 
GO

IF EXISTS (SELECT *
           FROM   dbo.sysobjects
           WHERE  id = Object_id(N'[{databaseOwner}].[{objectQualifier}user_simplelist]')
           AND Objectproperty(id,N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_simplelist] 
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bbcode_delete]') AND Objectproperty(id,N'IsProcedure') = 1)
	DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bbcode_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bbcode_list]') AND Objectproperty(id,N'IsProcedure') = 1)
	DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bbcode_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bbcode_save]') AND Objectproperty(id,N'IsProcedure') = 1)
	DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bbcode_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_addignoreduser]') AND Objectproperty(id,N'IsProcedure') = 1)
	DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_addignoreduser]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_removeignoreduser]') AND Objectproperty(id,N'IsProcedure') = 1)
	DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_removeignoreduser]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_isuserignored]') AND Objectproperty(id,N'IsProcedure') = 1)
	DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_isuserignored]
GO


/*****************************************************************************************************************************/
/***** BEGIN CREATE PROCEDURES ******/


create procedure [{databaseOwner}].[{objectQualifier}accessmask_delete](@AccessMaskID int) as
begin
	declare @flag int
	
	set @flag=1
	if exists(select 1 from [{databaseOwner}].[{objectQualifier}ForumAccess] where AccessMaskID=@AccessMaskID) or exists(select 1 from [{databaseOwner}].[{objectQualifier}UserForum] where AccessMaskID=@AccessMaskID)
		set @flag=0
	else
		delete from [{databaseOwner}].[{objectQualifier}AccessMask] where AccessMaskID=@AccessMaskID
	
	select @flag
end
GO

create procedure [{databaseOwner}].[{objectQualifier}accessmask_list](@BoardID int,@AccessMaskID int=null,@ExcludeFlags int = 0) as
begin
	if @AccessMaskID is null
		select 
			a.* 
		from 
			[{databaseOwner}].[{objectQualifier}AccessMask] a 
		where
			a.BoardID = @BoardID and
			(a.Flags & @ExcludeFlags) = 0
		order by 
			a.Name
	else
		select 
			a.* 
		from 
			[{databaseOwner}].[{objectQualifier}AccessMask] a 
		where
			a.BoardID = @BoardID and
			a.AccessMaskID = @AccessMaskID
		order by 
			a.Name
end
GO

create procedure [{databaseOwner}].[{objectQualifier}accessmask_save](
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
	@UploadAccess		bit,
	@DownloadAccess		bit
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
	if @DownloadAccess<>0 set @Flags = @Flags | 1024

	if @AccessMaskID is null
		insert into [{databaseOwner}].[{objectQualifier}AccessMask](Name,BoardID,Flags)
		values(@Name,@BoardID,@Flags)
	else
		update [{databaseOwner}].[{objectQualifier}AccessMask] set
			Name			= @Name,
			Flags			= @Flags
		where AccessMaskID=@AccessMaskID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}active_list](@BoardID int,@Guests bit=0) as
begin
	-- delete non-active
	delete from [{databaseOwner}].[{objectQualifier}Active] where DATEDIFF(minute,LastActive,getdate())>5
	-- select active
	if @Guests<>0
		select
			a.UserID,
			UserName = a.Name,
			c.IP,
			c.SessionID,
			c.ForumID,
			c.TopicID,
			ForumName = (select Name from [{databaseOwner}].[{objectQualifier}Forum] x where x.ForumID=c.ForumID),
			TopicName = (select Topic from [{databaseOwner}].[{objectQualifier}Topic] x where x.TopicID=c.TopicID),
			IsGuest = (select 1 from [{databaseOwner}].[{objectQualifier}UserGroup] x inner join [{databaseOwner}].[{objectQualifier}Group] y on y.GroupID=x.GroupID where x.UserID=a.UserID and (y.Flags & 2)<>0),
			IsHidden = ( a.IsActiveExcluded ),
			UserCount = 1,
			c.Login,
			c.LastActive,
			c.Location,
			Active = DATEDIFF(minute,c.Login,c.LastActive),
			c.Browser,
			c.Platform
		from
			[{databaseOwner}].[{objectQualifier}User] a
			inner join [{databaseOwner}].[{objectQualifier}Active] c ON c.UserID = a.UserID
		where
			c.BoardID = @BoardID
		order by
			c.LastActive desc
	else
		select
			a.UserID,
			UserName = a.Name,
			c.IP,
			c.SessionID,
			c.ForumID,
			c.TopicID,
			ForumName = (select Name from [{databaseOwner}].[{objectQualifier}Forum] x where x.ForumID=c.ForumID),
			TopicName = (select Topic from [{databaseOwner}].[{objectQualifier}Topic] x where x.TopicID=c.TopicID),
			IsGuest = (select 1 from [{databaseOwner}].[{objectQualifier}UserGroup] x inner join [{databaseOwner}].[{objectQualifier}Group] y on y.GroupID=x.GroupID where x.UserID=a.UserID and (y.Flags & 2)<>0),
			IsHidden = ( a.IsActiveExcluded ),
			UserCount = 1,
			c.Login,
			c.LastActive,
			c.Location,
			Active = DATEDIFF(minute,c.Login,c.LastActive),
			c.Browser,
			c.Platform
		from
			[{databaseOwner}].[{objectQualifier}User] a
			INNER JOIN [{databaseOwner}].[{objectQualifier}Active] c ON c.UserID = a.UserID
		where
			c.BoardID = @BoardID and
			not exists(
				select 1 
					from [{databaseOwner}].[{objectQualifier}UserGroup] x
						inner join [{databaseOwner}].[{objectQualifier}Group] y ON y.GroupID=x.GroupID 
					where x.UserID=a.UserID and (y.Flags & 2)<>0
				)
		order by
			c.LastActive desc
end
GO

create procedure [{databaseOwner}].[{objectQualifier}active_listforum](@ForumID int) as
begin
	select
		UserID		= a.UserID,
		UserName	= b.Name,
		IsHidden	= ( b.IsActiveExcluded ),
		UserCount   = COUNT(a.UserID)
	from
		[{databaseOwner}].[{objectQualifier}Active] a 
		join [{databaseOwner}].[{objectQualifier}User] b on b.UserID=a.UserID
	where
		a.ForumID = @ForumID
	group by
		a.UserID,
		b.Name,
		b.IsActiveExcluded
	order by
		b.Name
end
GO

create procedure [{databaseOwner}].[{objectQualifier}active_listtopic](@TopicID int) as
begin
	select
		UserID		= a.UserID,
		UserName	= b.Name,
		IsHidden = ( b.IsActiveExcluded ),
		UserCount   = COUNT(a.UserID)
	from
		[{databaseOwner}].[{objectQualifier}Active] a with(nolock)
		join [{databaseOwner}].[{objectQualifier}User] b on b.UserID=a.UserID
	where
		a.TopicID = @TopicID
	group by
		a.UserID,
		b.Name,
		b.IsActiveExcluded
	order by
		b.Name
end
GO

create procedure [{databaseOwner}].[{objectQualifier}active_stats](@BoardID int) as
begin
	select
		ActiveUsers = (select count(1) from [{databaseOwner}].[{objectQualifier}Active] x JOIN [{databaseOwner}].[{objectQualifier}User] usr ON x.UserID = usr.UserID where x.BoardID = @BoardID AND usr.IsActiveExcluded = 0),
		ActiveMembers = (select count(1) from [{databaseOwner}].[{objectQualifier}Active] x JOIN [{databaseOwner}].[{objectQualifier}User] usr ON x.UserID = usr.UserID where x.BoardID = @BoardID and exists(select 1 from [{databaseOwner}].[{objectQualifier}UserGroup] y inner join [{databaseOwner}].[{objectQualifier}Group] z on y.GroupID=z.GroupID where y.UserID=x.UserID and (z.Flags & 2)=0  AND usr.IsActiveExcluded = 0)),
		ActiveGuests = (select count(1) from [{databaseOwner}].[{objectQualifier}Active] x where x.BoardID = @BoardID and exists(select 1 from [{databaseOwner}].[{objectQualifier}UserGroup] y inner join [{databaseOwner}].[{objectQualifier}Group] z on y.GroupID=z.GroupID where y.UserID=x.UserID and (z.Flags & 2)<>0)),
		ActiveHidden = (select count(1) from [{databaseOwner}].[{objectQualifier}Active] x JOIN [{databaseOwner}].[{objectQualifier}User] usr ON x.UserID = usr.UserID where x.BoardID = @BoardID and exists(select 1 from [{databaseOwner}].[{objectQualifier}UserGroup] y inner join [{databaseOwner}].[{objectQualifier}Group] z on y.GroupID=z.GroupID where y.UserID=x.UserID and (z.Flags & 2)=0  AND usr.IsActiveExcluded = 1))
end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}active_updatemaxstats]
(
	@BoardID int
)
AS
BEGIN
	DECLARE @count int, @max int, @maxStr nvarchar(255), @countStr nvarchar(255), @dtStr nvarchar(255)
	
	SET @count = ISNULL((SELECT COUNT(DISTINCT IP + '.' + CAST(UserID as varchar(10))) FROM [{databaseOwner}].[{objectQualifier}Active] WITH (NOLOCK) WHERE BoardID = @BoardID),0)
	SET @maxStr = ISNULL((SELECT CAST([Value] AS nvarchar) FROM [{databaseOwner}].[{objectQualifier}Registry] WHERE BoardID = @BoardID AND [Name] = N'maxusers'),'1')
	SET @max = CAST(@maxStr AS int)
	SET @countStr = CAST(@count AS nvarchar)
	SET @dtStr = CONVERT(nvarchar,GETDATE(),126)

	IF NOT EXISTS ( SELECT 1 FROM [{databaseOwner}].[{objectQualifier}Registry] WHERE BoardID = @BoardID and [Name] = N'maxusers' )
	BEGIN 
		INSERT INTO [{databaseOwner}].[{objectQualifier}Registry](BoardID,[Name],[Value]) VALUES (@BoardID,N'maxusers',CAST(@countStr AS ntext))
		INSERT INTO [{databaseOwner}].[{objectQualifier}Registry](BoardID,[Name],[Value]) VALUES (@BoardID,N'maxuserswhen',CAST(@dtStr AS ntext))
	END
	ELSE IF (@count > @max)	
	BEGIN
		UPDATE [{databaseOwner}].[{objectQualifier}Registry] SET [Value] = CAST(@countStr AS ntext) WHERE BoardID = @BoardID AND [Name] = N'maxusers'
		UPDATE [{databaseOwner}].[{objectQualifier}Registry] SET [Value] = CAST(@dtStr AS ntext) WHERE BoardID = @BoardID AND [Name] = N'maxuserswhen'
	END
END
GO

create procedure [{databaseOwner}].[{objectQualifier}attachment_delete](@AttachmentID int) as begin
	delete from [{databaseOwner}].[{objectQualifier}Attachment] where AttachmentID=@AttachmentID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}attachment_download](@AttachmentID int) as
begin
	update [{databaseOwner}].[{objectQualifier}Attachment] set Downloads=Downloads+1 where AttachmentID=@AttachmentID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}attachment_list](@MessageID int=null,@AttachmentID int=null,@BoardID int=null) as begin
	if @MessageID is not null
		select 
			a.*,
			e.BoardID
		from
			[{databaseOwner}].[{objectQualifier}Attachment] a
			inner join [{databaseOwner}].[{objectQualifier}Message] b on b.MessageID = a.MessageID
			inner join [{databaseOwner}].[{objectQualifier}Topic] c on c.TopicID = b.TopicID
			inner join [{databaseOwner}].[{objectQualifier}Forum] d on d.ForumID = c.ForumID
			inner join [{databaseOwner}].[{objectQualifier}Category] e on e.CategoryID = d.CategoryID
		where
			a.MessageID=@MessageID
	else if @AttachmentID is not null
		select 
			a.*,
			e.BoardID
		from
			[{databaseOwner}].[{objectQualifier}Attachment] a
			inner join [{databaseOwner}].[{objectQualifier}Message] b on b.MessageID = a.MessageID
			inner join [{databaseOwner}].[{objectQualifier}Topic] c on c.TopicID = b.TopicID
			inner join [{databaseOwner}].[{objectQualifier}Forum] d on d.ForumID = c.ForumID
			inner join [{databaseOwner}].[{objectQualifier}Category] e on e.CategoryID = d.CategoryID
		where 
			a.AttachmentID=@AttachmentID
	else
		select 
			a.*,
			BoardID		= @BoardID,
			Posted		= b.Posted,
			ForumID		= d.ForumID,
			ForumName	= d.Name,
			TopicID		= c.TopicID,
			TopicName	= c.Topic
		from 
			[{databaseOwner}].[{objectQualifier}Attachment] a
			inner join [{databaseOwner}].[{objectQualifier}Message] b on b.MessageID = a.MessageID
			inner join [{databaseOwner}].[{objectQualifier}Topic] c on c.TopicID = b.TopicID
			inner join [{databaseOwner}].[{objectQualifier}Forum] d on d.ForumID = c.ForumID
			inner join [{databaseOwner}].[{objectQualifier}Category] e on e.CategoryID = d.CategoryID
		where
			e.BoardID = @BoardID
		order by
			d.Name,
			c.Topic,
			b.Posted
end
GO

create procedure [{databaseOwner}].[{objectQualifier}attachment_save](@MessageID int,@FileName nvarchar(255),@Bytes int,@ContentType nvarchar(50)=null,@FileData image=null) as begin
	insert into [{databaseOwner}].[{objectQualifier}Attachment](MessageID,FileName,Bytes,ContentType,Downloads,FileData) values(@MessageID,@FileName,@Bytes,@ContentType,0,@FileData)
end
GO

create procedure [{databaseOwner}].[{objectQualifier}bannedip_delete](@ID int) as
begin
	delete from [{databaseOwner}].[{objectQualifier}BannedIP] where ID = @ID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}bannedip_list](@BoardID int,@ID int=null) as
begin
	if @ID is null
		select * from [{databaseOwner}].[{objectQualifier}BannedIP] where BoardID=@BoardID
	else
		select * from [{databaseOwner}].[{objectQualifier}BannedIP] where BoardID=@BoardID and ID=@ID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}bannedip_save](@ID int=null,@BoardID int,@Mask nvarchar(15)) as
begin
	if @ID is null or @ID = 0 begin
		insert into [{databaseOwner}].[{objectQualifier}BannedIP](BoardID,Mask,Since) values(@BoardID,@Mask,getdate())
	end
	else begin
		update [{databaseOwner}].[{objectQualifier}BannedIP] set Mask = @Mask where ID = @ID
	end
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}board_create](
	@BoardName 		nvarchar(50),
	@MembershipAppName nvarchar(50),
	@RolesAppName nvarchar(50),
	@UserName		nvarchar(255),
	@UserKey		nvarchar(64),
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

	SET @TimeZone = (SELECT CAST(CAST([Value] as nvarchar(50)) as int) FROM [{databaseOwner}].[{objectQualifier}Registry] WHERE LOWER([Name]) = LOWER('TimeZone'))
	SET @ForumEmail = (SELECT CAST([Value] as nvarchar(50)) FROM [{databaseOwner}].[{objectQualifier}Registry] WHERE LOWER([Name]) = LOWER('ForumEmail'))

	-- Board
	INSERT INTO [{databaseOwner}].[{objectQualifier}Board](Name, AllowThreaded, MembershipAppName, RolesAppName ) values(@BoardName,0, @MembershipAppName, @RolesAppName)
	SET @BoardID = SCOPE_IDENTITY()

	-- Rank
	INSERT INTO [{databaseOwner}].[{objectQualifier}Rank](BoardID,Name,Flags,MinPosts) VALUES (@BoardID,'Administration',0,null)
	SET @RankIDAdmin = SCOPE_IDENTITY()
	INSERT INTO [{databaseOwner}].[{objectQualifier}Rank](BoardID,Name,Flags,MinPosts) VALUES(@BoardID,'Guest',0,null)
	SET @RankIDGuest = SCOPE_IDENTITY()
	INSERT INTO [{databaseOwner}].[{objectQualifier}Rank](BoardID,Name,Flags,MinPosts) VALUES(@BoardID,'Newbie',3,0)
	SET @RankIDNewbie = SCOPE_IDENTITY()
	INSERT INTO [{databaseOwner}].[{objectQualifier}Rank](BoardID,Name,Flags,MinPosts) VALUES(@BoardID,'Member',2,10)
	SET @RankIDMember = SCOPE_IDENTITY()
	INSERT INTO [{databaseOwner}].[{objectQualifier}Rank](BoardID,Name,Flags,MinPosts) VALUES(@BoardID,'Advanced Member',2,30)
	SET @RankIDAdvanced = SCOPE_IDENTITY()

	-- AccessMask
	INSERT INTO [{databaseOwner}].[{objectQualifier}AccessMask](BoardID,Name,Flags)
	VALUES(@BoardID,'Admin Access',1023 + 1024)
	SET @AccessMaskIDAdmin = SCOPE_IDENTITY()
	INSERT INTO [{databaseOwner}].[{objectQualifier}AccessMask](BoardID,Name,Flags)
	VALUES(@BoardID,'Moderator Access',487 + 1024)
	SET @AccessMaskIDModerator = SCOPE_IDENTITY()
	INSERT INTO [{databaseOwner}].[{objectQualifier}AccessMask](BoardID,Name,Flags)
	VALUES(@BoardID,'Member Access',423 + 1024)
	SET @AccessMaskIDMember = SCOPE_IDENTITY()
	INSERT INTO [{databaseOwner}].[{objectQualifier}AccessMask](BoardID,Name,Flags)
	VALUES(@BoardID,'Read Only Access',1)
	SET @AccessMaskIDReadOnly = SCOPE_IDENTITY()
	INSERT INTO [{databaseOwner}].[{objectQualifier}AccessMask](BoardID,Name,Flags)
	VALUES(@BoardID,'No Access',0)

	-- Group
	INSERT INTO [{databaseOwner}].[{objectQualifier}Group](BoardID,Name,Flags) values(@BoardID,'Administrators',1)
	set @GroupIDAdmin = SCOPE_IDENTITY()
	INSERT INTO [{databaseOwner}].[{objectQualifier}Group](BoardID,Name,Flags) values(@BoardID,'Guests',2)
	SET @GroupIDGuest = SCOPE_IDENTITY()
	INSERT INTO [{databaseOwner}].[{objectQualifier}Group](BoardID,Name,Flags) values(@BoardID,'Registered',4)
	SET @GroupIDMember = SCOPE_IDENTITY()	
	
	-- User (GUEST)
	INSERT INTO [{databaseOwner}].[{objectQualifier}User](BoardID,RankID,Name,Password,Joined,LastVisit,NumPosts,TimeZone,Email,Flags)
	VALUES(@BoardID,@RankIDGuest,'Guest','na',getdate(),getdate(),0,@TimeZone,@ForumEmail,6)
	SET @UserIDGuest = SCOPE_IDENTITY()	
	
	SET @UserFlags = 2
	if @IsHostAdmin<>0 SET @UserFlags = 3
	
	-- User (ADMIN)
	INSERT INTO [{databaseOwner}].[{objectQualifier}User](BoardID,RankID,Name,Password,ProviderUserKey, Joined,LastVisit,NumPosts,TimeZone,Flags)
	VALUES(@BoardID,@RankIDAdmin,@UserName,'na',@UserKey,getdate(),getdate(),0,@TimeZone,@UserFlags)
	SET @UserIDAdmin = SCOPE_IDENTITY()

	-- UserGroup
	INSERT INTO [{databaseOwner}].[{objectQualifier}UserGroup](UserID,GroupID) VALUES(@UserIDAdmin,@GroupIDAdmin)
	INSERT INTO [{databaseOwner}].[{objectQualifier}UserGroup](UserID,GroupID) VALUES(@UserIDGuest,@GroupIDGuest)

	-- Category
	INSERT INTO [{databaseOwner}].[{objectQualifier}Category](BoardID,Name,SortOrder) VALUES(@BoardID,'Test Category',1)
	set @CategoryID = SCOPE_IDENTITY()
	
	-- Forum
	INSERT INTO [{databaseOwner}].[{objectQualifier}Forum](CategoryID,Name,Description,SortOrder,NumTopics,NumPosts,Flags)
	VALUES(@CategoryID,'Test Forum','A test forum',1,0,0,4)
	set @ForumID = SCOPE_IDENTITY()

	-- ForumAccess
	INSERT INTO [{databaseOwner}].[{objectQualifier}ForumAccess](GroupID,ForumID,AccessMaskID) VALUES(@GroupIDAdmin,@ForumID,@AccessMaskIDAdmin)
	INSERT INTO [{databaseOwner}].[{objectQualifier}ForumAccess](GroupID,ForumID,AccessMaskID) VALUES(@GroupIDGuest,@ForumID,@AccessMaskIDReadOnly)
	INSERT INTO [{databaseOwner}].[{objectQualifier}ForumAccess](GroupID,ForumID,AccessMaskID) VALUES(@GroupIDMember,@ForumID,@AccessMaskIDMember)
end
GO

create procedure [{databaseOwner}].[{objectQualifier}board_delete](@BoardID int) as
begin
	declare @tmpForumID int;
	declare forum_cursor cursor for
		select ForumID 
		from [{databaseOwner}].[{objectQualifier}Forum] a join [{databaseOwner}].[{objectQualifier}Category] b on a.CategoryID=b.CategoryID
		where b.BoardID=@BoardID
		order by ForumID desc
	
	open forum_cursor
	fetch next from forum_cursor into @tmpForumID
	while @@FETCH_STATUS = 0
	begin
		exec [{databaseOwner}].[{objectQualifier}forum_delete] @tmpForumID;
		fetch next from forum_cursor into @tmpForumID
	end
	close forum_cursor
	deallocate forum_cursor

	delete from [{databaseOwner}].[{objectQualifier}ForumAccess] where exists(select 1 from [{databaseOwner}].[{objectQualifier}Group] x where x.GroupID=[{databaseOwner}].[{objectQualifier}ForumAccess].GroupID and x.BoardID=@BoardID)
	delete from [{databaseOwner}].[{objectQualifier}Forum] where exists(select 1 from [{databaseOwner}].[{objectQualifier}Category] x where x.CategoryID=[{databaseOwner}].[{objectQualifier}Forum].CategoryID and x.BoardID=@BoardID)
	delete from [{databaseOwner}].[{objectQualifier}UserGroup] where exists(select 1 from [{databaseOwner}].[{objectQualifier}User] x where x.UserID=[{databaseOwner}].[{objectQualifier}UserGroup].UserID and x.BoardID=@BoardID)
	delete from [{databaseOwner}].[{objectQualifier}Category] where BoardID=@BoardID
	delete from [{databaseOwner}].[{objectQualifier}User] where BoardID=@BoardID
	delete from [{databaseOwner}].[{objectQualifier}Rank] where BoardID=@BoardID
	delete from [{databaseOwner}].[{objectQualifier}Group] where BoardID=@BoardID
	delete from [{databaseOwner}].[{objectQualifier}AccessMask] where BoardID=@BoardID
	delete from [{databaseOwner}].[{objectQualifier}Active] where BoardID=@BoardID
	delete from [{databaseOwner}].[{objectQualifier}Board] where BoardID=@BoardID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}board_list](@BoardID int=null) as
begin
	select
		a.*,
		SQLVersion = @@VERSION
	from 
		[{databaseOwner}].[{objectQualifier}Board] a
	where
		(@BoardID is null or a.BoardID = @BoardID)
end
GO

create procedure [{databaseOwner}].[{objectQualifier}board_poststats](@BoardID int) as
BEGIN
	SELECT
		Posts = (select count(1) from [{databaseOwner}].[{objectQualifier}Message] a join [{databaseOwner}].[{objectQualifier}Topic] b on b.TopicID=a.TopicID join [{databaseOwner}].[{objectQualifier}Forum] c on c.ForumID=b.ForumID join [{databaseOwner}].[{objectQualifier}Category] d on d.CategoryID=c.CategoryID where d.BoardID=@BoardID AND (a.Flags & 24)=16),
		Topics = (select count(1) from [{databaseOwner}].[{objectQualifier}Topic] a join [{databaseOwner}].[{objectQualifier}Forum] b on b.ForumID=a.ForumID join [{databaseOwner}].[{objectQualifier}Category] c on c.CategoryID=b.CategoryID where c.BoardID=@BoardID AND (a.Flags & 8) <> 8),
		Forums = (select count(1) from [{databaseOwner}].[{objectQualifier}Forum] a join [{databaseOwner}].[{objectQualifier}Category] b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID),
		Members = (select count(1) from [{databaseOwner}].[{objectQualifier}User] a where a.BoardID=@BoardID AND (Flags & 2) = 2 AND (a.Flags & 4) = 0),
		MaxUsers = (SELECT CAST([Value] as nvarchar(255)) FROM [{databaseOwner}].[{objectQualifier}Registry] WHERE LOWER(Name) = LOWER('maxusers') and BoardID=@BoardID),
        MaxUsersWhen = (SELECT CAST([Value] as nvarchar(255)) FROM [{databaseOwner}].[{objectQualifier}Registry] WHERE LOWER(Name) = LOWER('maxuserswhen') and BoardID=@BoardID),		
		LastPostInfo.*,
		LastMemberInfo.*
	FROM
		(
			SELECT TOP 1 
				LastMemberInfoID= 1,
				LastMemberID	= UserID,
				LastMember	= [Name]
			FROM 
				[{databaseOwner}].[{objectQualifier}User]
			WHERE 
				(Flags & 2) = 2
				AND (Flags & 4) = 0
				AND BoardID = @BoardID 
			ORDER BY 
				Joined DESC
		) as LastMemberInfo
		left join (
			SELECT TOP 1 
				LastPostInfoID	= 1,
				LastPost	= a.Posted,
				LastUserID	= a.UserID,
				LastUser	= e.Name
			FROM 
				[{databaseOwner}].[{objectQualifier}Message] a 
				join [{databaseOwner}].[{objectQualifier}Topic] b on b.TopicID=a.TopicID 
				join [{databaseOwner}].[{objectQualifier}Forum] c on c.ForumID=b.ForumID 
				join [{databaseOwner}].[{objectQualifier}Category] d on d.CategoryID=c.CategoryID 
				join [{databaseOwner}].[{objectQualifier}User] e on e.UserID=a.UserID
			WHERE 
				(a.Flags & 24) = 16
				AND (b.Flags & 8) <> 8 
				AND d.BoardID = @BoardID
			ORDER BY
				a.Posted DESC
		) as LastPostInfo
		on LastMemberInfoID=LastPostInfoID
END
GO

create procedure [{databaseOwner}].[{objectQualifier}board_save](@BoardID int,@Name nvarchar(50),@AllowThreaded bit) as
begin
	update [{databaseOwner}].[{objectQualifier}Board] set
		Name = @Name,
		AllowThreaded = @AllowThreaded
	where BoardID=@BoardID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}board_stats]
	@BoardID	int = null
as 
begin
	if (@BoardID is null) begin
		select
			NumPosts	= (select count(1) from [{databaseOwner}].[{objectQualifier}Message] where IsApproved = 1 AND IsDeleted = 0),
			NumTopics	= (select count(1) from [{databaseOwner}].[{objectQualifier}Topic] where IsDeleted = 0),
			NumUsers	= (select count(1) from [{databaseOwner}].[{objectQualifier}User] where IsApproved = 1),
			BoardStart	= (select min(Joined) from [{databaseOwner}].[{objectQualifier}User])
	end
	else begin
		select
			NumPosts	= (select count(1)	
								from [{databaseOwner}].[{objectQualifier}Message] a
								join [{databaseOwner}].[{objectQualifier}Topic] b ON a.TopicID=b.TopicID
								join [{databaseOwner}].[{objectQualifier}Forum] c ON b.ForumID=c.ForumID
								join [{databaseOwner}].[{objectQualifier}Category] d ON c.CategoryID=d.CategoryID
								where a.IsApproved = 1 AND a.IsDeleted = 0 and b.IsDeleted = 0 AND d.BoardID=@BoardID
							),
			NumTopics	= (select count(1) 
								from [{databaseOwner}].[{objectQualifier}Topic] a
								join [{databaseOwner}].[{objectQualifier}Forum] b ON a.ForumID=b.ForumID
								join [{databaseOwner}].[{objectQualifier}Category] c ON b.CategoryID=c.CategoryID
								where c.BoardID=@BoardID AND a.IsDeleted = 0
							),
			NumUsers	= (select count(1) from [{databaseOwner}].[{objectQualifier}User] where IsApproved = 1 and BoardID=@BoardID),
			BoardStart	= (select min(Joined) from [{databaseOwner}].[{objectQualifier}User] where BoardID=@BoardID)
	end
end
GO

create procedure [{databaseOwner}].[{objectQualifier}category_delete](@CategoryID int) as
begin
	declare @flag int
 
	if exists(select 1 from [{databaseOwner}].[{objectQualifier}Forum] where CategoryID = @CategoryID)
	begin
		set @flag = 0
	end else
	begin
		delete from [{databaseOwner}].[{objectQualifier}Category] where CategoryID = @CategoryID
		set @flag = 1
	end

	select @flag
end
GO

create procedure [{databaseOwner}].[{objectQualifier}category_list](@BoardID int,@CategoryID int=null) as
begin
	if @CategoryID is null
		select * from [{databaseOwner}].[{objectQualifier}Category] where BoardID = @BoardID order by SortOrder
	else
		select * from [{databaseOwner}].[{objectQualifier}Category] where BoardID = @BoardID and CategoryID = @CategoryID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}category_listread](@BoardID int,@UserID int,@CategoryID int=null) as
begin
	select 
		a.CategoryID,
		a.Name,
		a.CategoryImage
	from 
		[{databaseOwner}].[{objectQualifier}Category] a
		join [{databaseOwner}].[{objectQualifier}Forum] b on b.CategoryID=a.CategoryID
		join [{databaseOwner}].[{objectQualifier}vaccess] v on v.ForumID=b.ForumID
	where
		a.BoardID=@BoardID and
		v.UserID=@UserID and
		(v.ReadAccess<>0 or (b.Flags & 2)=0) and
		(@CategoryID is null or a.CategoryID=@CategoryID) and
		b.ParentID is null
	group by
		a.CategoryID,
		a.Name,
		a.SortOrder,
		a.CategoryImage
	order by 
		a.SortOrder
end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}category_save]
(
	@BoardID    INT,
	@CategoryID INT,
	@Name       NVARCHAR(128),	
	@SortOrder  SMALLINT,
	@CategoryImage NVARCHAR(255) = NULL
)
AS
BEGIN
    IF @CategoryID > 0
    BEGIN
        UPDATE [{databaseOwner}].[{objectQualifier}Category]
        SET    Name = @Name,
			   CategoryImage = @CategoryImage,
               SortOrder = @SortOrder
        WHERE  CategoryID = @CategoryID
        SELECT CategoryID = @CategoryID
    END
    ELSE
    BEGIN
        INSERT INTO [{databaseOwner}].[{objectQualifier}Category]
                   (BoardID,
                    [Name],
					[CategoryImage],
                    SortOrder)
        VALUES     (@BoardID,
                    @Name,
					@CategoryImage,
                    @SortOrder)
        SELECT CategoryID = Scope_identity()
    END
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}checkemail_list]
(
	@Email nvarchar(50) = null
)
AS
BEGIN
	IF @Email IS NULL
		SELECT * FROM [{databaseOwner}].[{objectQualifier}CheckEmail]
	ELSE
		SELECT * FROM [{databaseOwner}].[{objectQualifier}CheckEmail] WHERE Email = LOWER(@EMail)
END
GO

create procedure [{databaseOwner}].[{objectQualifier}checkemail_save]
(
	@UserID int,
	@Hash nvarchar(32),
	@Email nvarchar(50)
)
AS
BEGIN
	INSERT INTO [{databaseOwner}].[{objectQualifier}CheckEmail]
		(UserID,Email,Created,Hash)
	VALUES
		(@UserID,LOWER(@Email),getdate(),@Hash)	
END
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}checkemail_update](@Hash nvarchar(32)) as
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
		[{databaseOwner}].[{objectQualifier}CheckEmail]
	where
		Hash = @Hash

	if @UserID is null
	begin
		select convert(nvarchar(64),NULL) as ProviderUserKey, convert(nvarchar(255),NULL) as Email
		return
	end

	-- Update new user email
	update [{databaseOwner}].[{objectQualifier}User] set Email = LOWER(@Email), Flags = Flags | 2 where UserID = @UserID
	delete [{databaseOwner}].[{objectQualifier}CheckEmail] where CheckEmailID = @CheckEmailID

	-- return the UserProviderKey
	SELECT ProviderUserKey, Email FROM [{databaseOwner}].[{objectQualifier}User] WHERE UserID = @UserID
end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}choice_vote](@ChoiceID int,@UserID int = NULL, @RemoteIP nvarchar(10) = NULL) AS
BEGIN
	DECLARE @PollID int

	SET @PollID = (SELECT PollID FROM [{databaseOwner}].[{objectQualifier}Choice] WHERE ChoiceID = @ChoiceID)

	IF @UserID = NULL
	BEGIN
		IF @RemoteIP != NULL
		BEGIN
			INSERT INTO [{databaseOwner}].[{objectQualifier}PollVote] (PollID, UserID, RemoteIP) VALUES (@PollID,NULL,@RemoteIP)	
		END
	END
	ELSE
	BEGIN
		INSERT INTO [{databaseOwner}].[{objectQualifier}PollVote] (PollID, UserID, RemoteIP) VALUES (@PollID,@UserID,@RemoteIP)
	END

	UPDATE [{databaseOwner}].[{objectQualifier}Choice] SET Votes = Votes + 1 WHERE ChoiceID = @ChoiceID
END
GO

create procedure [{databaseOwner}].[{objectQualifier}eventlog_create](@UserID int,@Source nvarchar(50),@Description ntext,@Type int) as
begin
	insert into [{databaseOwner}].[{objectQualifier}EventLog](UserID,Source,Description,Type)
	values(@UserID,@Source,@Description,@Type)

	-- delete entries older than 10 days
	delete from [{databaseOwner}].[{objectQualifier}EventLog] where EventTime+10<getdate()

	-- or if there are more then 1000	
	if ((select count(*) from [{databaseOwner}].[{objectQualifier}eventlog]) >= 1050)
	begin
		
		delete from [{databaseOwner}].[{objectQualifier}EventLog] WHERE EventLogID IN (SELECT TOP 100 EventLogID FROM [{databaseOwner}].[{objectQualifier}EventLog] ORDER BY EventTime)
	end	
	
end
GO

create procedure [{databaseOwner}].[{objectQualifier}eventlog_delete]
(
	@EventLogID int = null, 
	@BoardID int = null
) as
begin
	-- either EventLogID or BoardID must be null, not both at the same time
	if (@EventLogID is null) begin
		-- delete all events of this board
		delete from [{databaseOwner}].[{objectQualifier}EventLog]
		where
			(UserID is null or
			UserID in (select UserID from [{databaseOwner}].[{objectQualifier}User] where BoardID=@BoardID))
	end
	else begin
		-- delete just one event
		delete from [{databaseOwner}].[{objectQualifier}EventLog] where EventLogID=@EventLogID
	end
end
GO

create procedure [{databaseOwner}].[{objectQualifier}eventlog_list](@BoardID int) as
begin
	select
		a.*,
		ISNULL(b.[Name],'System') as [Name]
	from
		[{databaseOwner}].[{objectQualifier}EventLog] a
		left join [{databaseOwner}].[{objectQualifier}User] b on b.UserID=a.UserID
	where
		(b.UserID IS NULL or b.BoardID = @BoardID)		
	order by
		a.EventLogID desc
end
GO

create procedure [{databaseOwner}].[{objectQualifier}extension_delete] (@ExtensionID int) as
begin
	delete from [{databaseOwner}].[{objectQualifier}Extension] 
	where ExtensionID = @ExtensionID
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}extension_edit] (@ExtensionID int=NULL) as
BEGIN
	SELECT * 
	FROM [{databaseOwner}].[{objectQualifier}Extension] 
	WHERE ExtensionID = @ExtensionID 
	ORDER BY Extension
END
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}extension_list] (@BoardID int, @Extension nvarchar(10)) as
BEGIN

	-- If an extension is passed, then we want to check for THAT extension
	IF LEN(@Extension) > 0
		BEGIN
			SELECT
				a.*
			FROM
				[{databaseOwner}].[{objectQualifier}Extension] a
			WHERE
				a.BoardID = @BoardID AND a.Extension=@Extension
			ORDER BY
				a.Extension
		END

	ELSE
		-- Otherwise, just get a list for the given @BoardId
		BEGIN
			SELECT
				a.*
			FROM
				[{databaseOwner}].[{objectQualifier}Extension] a
			WHERE
				a.BoardID = @BoardID	
			ORDER BY
				a.Extension
		END
END
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}extension_save] (@ExtensionID int=null,@BoardID int,@Extension nvarchar(10)) as
begin
	if @ExtensionID is null or @ExtensionID = 0 begin
		insert into [{databaseOwner}].[{objectQualifier}Extension] (BoardID,Extension) 
		values(@BoardID,@Extension)
	end
	else begin
		update [{databaseOwner}].[{objectQualifier}Extension] 
		set Extension = @Extension 
		where ExtensionID = @ExtensionID
	end
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}forum_delete](@ForumID int) as
begin
	-- Maybe an idea to use cascading foreign keys instead? Too bad they don't work on MS SQL 7.0...
	update [{databaseOwner}].[{objectQualifier}Forum] set LastMessageID=null,LastTopicID=null where ForumID=@ForumID
	update [{databaseOwner}].[{objectQualifier}Topic] set LastMessageID=null where ForumID=@ForumID
	update [{databaseOwner}].[{objectQualifier}Active] set ForumID=null where ForumID=@ForumID
	delete from [{databaseOwner}].[{objectQualifier}WatchTopic] from [{databaseOwner}].[{objectQualifier}Topic] where [{databaseOwner}].[{objectQualifier}Topic].ForumID = @ForumID and [{databaseOwner}].[{objectQualifier}WatchTopic].TopicID = [{databaseOwner}].[{objectQualifier}Topic].TopicID
	delete from [{databaseOwner}].[{objectQualifier}Active] from [{databaseOwner}].[{objectQualifier}Topic] where [{databaseOwner}].[{objectQualifier}Topic].ForumID = @ForumID and [{databaseOwner}].[{objectQualifier}Active].TopicID = [{databaseOwner}].[{objectQualifier}Topic].TopicID
	delete from [{databaseOwner}].[{objectQualifier}NntpTopic] from [{databaseOwner}].[{objectQualifier}NntpForum] where [{databaseOwner}].[{objectQualifier}NntpForum].ForumID = @ForumID and [{databaseOwner}].[{objectQualifier}NntpTopic].NntpForumID = [{databaseOwner}].[{objectQualifier}NntpForum].NntpForumID
	delete from [{databaseOwner}].[{objectQualifier}NntpForum] where ForumID=@ForumID	
	delete from [{databaseOwner}].[{objectQualifier}WatchForum] where ForumID = @ForumID

	-- BAI CHANGED 02.02.2004
	-- Delete topics, messages and attachments

	declare @tmpTopicID int;
	declare topic_cursor cursor for
		select TopicID from [{databaseOwner}].[{objectQualifier}topic]
		where ForumID = @ForumID
		order by TopicID desc
	
	open topic_cursor
	
	fetch next from topic_cursor
	into @tmpTopicID
	
	-- Check @@FETCH_STATUS to see if there are any more rows to fetch.
	while @@FETCH_STATUS = 0
	begin
		exec [{databaseOwner}].[{objectQualifier}topic_delete] @tmpTopicID,1,1;
	
	   -- This is executed as long as the previous fetch succeeds.
		fetch next from topic_cursor
		into @tmpTopicID
	end
	
	close topic_cursor
	deallocate topic_cursor

	-- TopicDelete finished
	-- END BAI CHANGED 02.02.2004

	delete from [{databaseOwner}].[{objectQualifier}ForumAccess] where ForumID = @ForumID
	--ABOT CHANGED
	--Delete UserForums Too 
	delete from [{databaseOwner}].[{objectQualifier}UserForum] where ForumID = @ForumID
	--END ABOT CHANGED 09.04.2004
	delete from [{databaseOwner}].[{objectQualifier}Forum] where ForumID = @ForumID
end

GO

create procedure [{databaseOwner}].[{objectQualifier}forum_list](@BoardID int,@ForumID int=null) as
begin
	if @ForumID = 0 set @ForumID = null
	if @ForumID is null
		select a.* from [{databaseOwner}].[{objectQualifier}Forum] a join [{databaseOwner}].[{objectQualifier}Category] b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID order by a.SortOrder
	else
		select a.* from [{databaseOwner}].[{objectQualifier}Forum] a join [{databaseOwner}].[{objectQualifier}Category] b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID and a.ForumID = @ForumID
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}forum_listall] (@BoardID int,@UserID int,@root int = 0) as
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
        [{databaseOwner}].[{objectQualifier}Forum] a
        join [{databaseOwner}].[{objectQualifier}Category] b on b.CategoryID=a.CategoryID
        join [{databaseOwner}].[{objectQualifier}vaccess] c on c.ForumID=a.ForumID
    where
        c.UserID=@UserID and
        b.BoardID=@BoardID and
        c.ReadAccess>0
    order by
        b.SortOrder,
        a.SortOrder,
        b.CategoryID,
        a.ForumID
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
        [{databaseOwner}].[{objectQualifier}Forum] a
        join [{databaseOwner}].[{objectQualifier}Category] b on b.CategoryID=a.CategoryID
        join [{databaseOwner}].[{objectQualifier}vaccess] c on c.ForumID=a.ForumID
    where
        c.UserID=@UserID and
        b.BoardID=@BoardID and
        c.ReadAccess>0 and
        a.ForumID = @root

    order by
        b.SortOrder,
        a.SortOrder,
        b.CategoryID,
        a.ForumID
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
        [{databaseOwner}].[{objectQualifier}Forum] a
        join [{databaseOwner}].[{objectQualifier}Category] b on b.CategoryID=a.CategoryID
        join [{databaseOwner}].[{objectQualifier}vaccess] c on c.ForumID=a.ForumID
    where
        c.UserID=@UserID and
        b.BoardID=@BoardID and
        c.ReadAccess>0 and
        b.CategoryID = -@root

    order by
        b.SortOrder,
        a.SortOrder,
        b.CategoryID,
        a.ForumID
end
end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listall_fromcat](@BoardID int,@CategoryID int) AS
BEGIN
	SELECT     b.CategoryID, b.Name AS Category, a.ForumID, a.Name AS Forum, a.ParentID
	FROM         [{databaseOwner}].[{objectQualifier}Forum] a INNER JOIN
						  [{databaseOwner}].[{objectQualifier}Category] b ON b.CategoryID = a.CategoryID
		WHERE
			b.CategoryID=@CategoryID and
			b.BoardID=@BoardID
		ORDER BY
			b.SortOrder,
			a.SortOrder
END
GO

create procedure [{databaseOwner}].[{objectQualifier}forum_listallmymoderated](@BoardID int,@UserID int) as
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
			[{databaseOwner}].[{objectQualifier}Category] a
			join [{databaseOwner}].[{objectQualifier}Forum] b on b.CategoryID=a.CategoryID
		where
			a.BoardID=@BoardID and
			b.ParentID is null
	
		union
	
		select
			c.ForumID,
			Indent = 1
		from
			[{databaseOwner}].[{objectQualifier}Category] a
			join [{databaseOwner}].[{objectQualifier}Forum] b on b.CategoryID=a.CategoryID
			join [{databaseOwner}].[{objectQualifier}Forum] c on c.ParentID=b.ForumID
		where
			a.BoardID=@BoardID and
			b.ParentID is null
	
		union
	
		select
			d.ForumID,
			Indent = 2
		from
			[{databaseOwner}].[{objectQualifier}Category] a
			join [{databaseOwner}].[{objectQualifier}Forum] b on b.CategoryID=a.CategoryID
			join [{databaseOwner}].[{objectQualifier}Forum] c on c.ParentID=b.ForumID
			join [{databaseOwner}].[{objectQualifier}Forum] d on d.ParentID=c.ForumID
		where
			a.BoardID=@BoardID and
			b.ParentID is null
		) as x
		join [{databaseOwner}].[{objectQualifier}Forum] a on a.ForumID=x.ForumID
		join [{databaseOwner}].[{objectQualifier}Category] b on b.CategoryID=a.CategoryID
		join [{databaseOwner}].[{objectQualifier}vaccess] c on c.ForumID=a.ForumID
	where
		c.UserID=@UserID and
		b.BoardID=@BoardID and
		c.ModeratorAccess>0
	order by
		b.SortOrder,
		a.SortOrder
end
GO

create procedure [{databaseOwner}].[{objectQualifier}forum_listpath](@ForumID int) as
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
			[{databaseOwner}].[{objectQualifier}Forum] a
		where
			a.ForumID=@ForumID

		union

		select
			b.ForumID,
			Indent = 1
		from
			[{databaseOwner}].[{objectQualifier}Forum] a
			join [{databaseOwner}].[{objectQualifier}Forum] b on b.ForumID=a.ParentID
		where
			a.ForumID=@ForumID

		union

		select
			c.ForumID,
			Indent = 2
		from
			[{databaseOwner}].[{objectQualifier}Forum] a
			join [{databaseOwner}].[{objectQualifier}Forum] b on b.ForumID=a.ParentID
			join [{databaseOwner}].[{objectQualifier}Forum] c on c.ForumID=b.ParentID
		where
			a.ForumID=@ForumID

		union 

		select
			d.ForumID,
			Indent = 3
		from
			[{databaseOwner}].[{objectQualifier}Forum] a
			join [{databaseOwner}].[{objectQualifier}Forum] b on b.ForumID=a.ParentID
			join [{databaseOwner}].[{objectQualifier}Forum] c on c.ForumID=b.ParentID
			join [{databaseOwner}].[{objectQualifier}Forum] d on d.ForumID=c.ParentID
		where
			a.ForumID=@ForumID
		) as x	
		join [{databaseOwner}].[{objectQualifier}Forum] a on a.ForumID=x.ForumID
	order by
		x.Indent desc
end
GO

create procedure [{databaseOwner}].[{objectQualifier}forum_listread](@BoardID int,@UserID int,@CategoryID int=null,@ParentID int=null) as
begin
	select 
		a.CategoryID, 
		Category		= a.Name, 
		ForumID			= b.ForumID,
		Forum			= b.Name, 
		Description,
		Topics			= [{databaseOwner}].[{objectQualifier}forum_topics](b.ForumID),
		Posts			= [{databaseOwner}].[{objectQualifier}forum_posts](b.ForumID),
		Subforums		= [{databaseOwner}].[{objectQualifier}forum_subforums](b.ForumID, @UserID),
		LastPosted		= t.LastPosted,
		LastMessageID	= t.LastMessageID,
		LastUserID		= t.LastUserID,
		LastUser		= IsNull(t.LastUserName,(select Name from [{databaseOwner}].[{objectQualifier}User] x where x.UserID=t.LastUserID)),
		LastTopicID		= t.TopicID,
		LastTopicName	= t.Topic,
		b.Flags,
		Viewing			= (select count(1) from [{databaseOwner}].[{objectQualifier}Active] x JOIN [{databaseOwner}].[{objectQualifier}User] usr ON x.UserID = usr.UserID where x.ForumID=b.ForumID AND usr.IsActiveExcluded = 0),
		b.RemoteURL,
		x.ReadAccess
	from 
		[{databaseOwner}].[{objectQualifier}Category] a
		join [{databaseOwner}].[{objectQualifier}Forum] b on b.CategoryID=a.CategoryID
		join [{databaseOwner}].[{objectQualifier}vaccess] x on x.ForumID=b.ForumID
		left outer join [{databaseOwner}].[{objectQualifier}Topic] t ON t.TopicID = [{databaseOwner}].[{objectQualifier}forum_lasttopic](b.ForumID,@UserID,b.LastTopicID,b.LastPosted)
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

create procedure [{databaseOwner}].[{objectQualifier}forum_listSubForums](@ForumID int) as
begin
	select Sum(1) from [{databaseOwner}].[{objectQualifier}Forum] where ParentID = @ForumID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}forum_listtopics](@ForumID int) as
begin
select * from [{databaseOwner}].[{objectQualifier}Topic]
Where ForumID = @ForumID
end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}forum_moderatelist](@BoardID int,@UserID int) AS
BEGIN

SELECT
		b.*,
		MessageCount  = 
		(SELECT     count([{databaseOwner}].[{objectQualifier}Message].MessageID)
		FROM         [{databaseOwner}].[{objectQualifier}Message] INNER JOIN
							  [{databaseOwner}].[{objectQualifier}Topic] ON [{databaseOwner}].[{objectQualifier}Message].TopicID = [{databaseOwner}].[{objectQualifier}Topic].TopicID
		WHERE (([{databaseOwner}].[{objectQualifier}Message].Flags & 16)=0) and (([{databaseOwner}].[{objectQualifier}Message].Flags & 8)=0) and (([{databaseOwner}].[{objectQualifier}Topic].Flags & 8) = 0) AND ([{databaseOwner}].[{objectQualifier}Topic].ForumID=b.ForumID)),
		ReportCount	= 
		(SELECT     count([{databaseOwner}].[{objectQualifier}Message].MessageID)
		FROM         [{databaseOwner}].[{objectQualifier}Message] INNER JOIN
							  [{databaseOwner}].[{objectQualifier}Topic] ON [{databaseOwner}].[{objectQualifier}Message].TopicID = [{databaseOwner}].[{objectQualifier}Topic].TopicID
		WHERE (([{databaseOwner}].[{objectQualifier}Message].Flags & 128)=128) and (([{databaseOwner}].[{objectQualifier}Message].Flags & 8)=0) and (([{databaseOwner}].[{objectQualifier}Topic].Flags & 8) = 0) AND ([{databaseOwner}].[{objectQualifier}Topic].ForumID=b.ForumID)),
		SpamCount	= 
		(SELECT     count([{databaseOwner}].[{objectQualifier}Message].MessageID)
		FROM         [{databaseOwner}].[{objectQualifier}Message] INNER JOIN
							  [{databaseOwner}].[{objectQualifier}Topic] ON [{databaseOwner}].[{objectQualifier}Message].TopicID = [{databaseOwner}].[{objectQualifier}Topic].TopicID
		WHERE (([{databaseOwner}].[{objectQualifier}Message].Flags & 256)=256) and (([{databaseOwner}].[{objectQualifier}Message].Flags & 8)=0) and (([{databaseOwner}].[{objectQualifier}Topic].Flags & 8) = 0) AND ([{databaseOwner}].[{objectQualifier}Topic].ForumID=b.ForumID))
		
	FROM
		[{databaseOwner}].[{objectQualifier}Category] a

	JOIN [{databaseOwner}].[{objectQualifier}Forum] b ON b.CategoryID=a.CategoryID
	JOIN [{databaseOwner}].[{objectQualifier}vaccess] c ON c.ForumID=b.ForumID

	WHERE
		a.BoardID=@BoardID AND
		c.ModeratorAccess>0 AND
		c.UserID=@UserID
	ORDER BY
		a.SortOrder,
		b.SortOrder
END
GO

create procedure [{databaseOwner}].[{objectQualifier}forum_moderators] as
BEGIN
	select
		ForumID = a.ForumID, 
		ModeratorID = a.GroupID, 
		ModeratorName = b.Name,
		IsGroup=1
	from
		[{databaseOwner}].[{objectQualifier}ForumAccess] a
		INNER JOIN [{databaseOwner}].[{objectQualifier}Group] b ON b.GroupID = a.GroupID
		INNER JOIN [{databaseOwner}].[{objectQualifier}AccessMask] c ON c.AccessMaskID = a.AccessMaskID
	where
		(b.Flags & 1)=0 and
		(c.Flags & 64)<>0
	union all
	select 
		ForumID = access.ForumID, 
		ModeratorID = usr.UserID, 
		ModeratorName = usr.Name,
		IsGroup=0
	from
		[{databaseOwner}].[{objectQualifier}User] usr
		INNER JOIN (
			select
				UserID				= a.UserID,
				ForumID				= x.ForumID,
				ModeratorAccess		= max(x.ModeratorAccess)
			from
				[{databaseOwner}].[{objectQualifier}vaccessfull] as x
				INNER JOIN [{databaseOwner}].[{objectQualifier}UserGroup] a on a.UserID=x.UserID
				INNER JOIN [{databaseOwner}].[{objectQualifier}Group] b on b.GroupID=a.GroupID
			WHERE 
				x.AdminGroup = 0
			GROUP BY
				a.UserID,x.ForumID		
		) access ON usr.UserID = access.UserID
	where
		access.ModeratorAccess<>0
	order by
		IsGroup desc,
		ModeratorName asc
END
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}forum_save](
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
		update [{databaseOwner}].[{objectQualifier}Forum] set 
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
		select @BoardID=BoardID from [{databaseOwner}].[{objectQualifier}Category] where CategoryID=@CategoryID
	
		insert into [{databaseOwner}].[{objectQualifier}Forum](ParentID,Name,Description,SortOrder,CategoryID,NumTopics,NumPosts,RemoteURL,ThemeURL,Flags)
		values(@ParentID,@Name,@Description,@SortOrder,@CategoryID,0,0,@RemoteURL,@ThemeURL,@Flags)
		select @ForumID = SCOPE_IDENTITY()

		insert into [{databaseOwner}].[{objectQualifier}ForumAccess](GroupID,ForumID,AccessMaskID) 
		select GroupID,@ForumID,@AccessMaskID
		from [{databaseOwner}].[{objectQualifier}Group]
		where BoardID=@BoardID
	end
	select ForumID = @ForumID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}forum_updatelastpost](@ForumID int) as
begin
	update [{databaseOwner}].[{objectQualifier}Forum] set
		LastPosted = (select top 1 y.Posted from [{databaseOwner}].[{objectQualifier}Topic] x join [{databaseOwner}].[{objectQualifier}Message] y on y.TopicID=x.TopicID where x.ForumID = @ForumID and (y.Flags & 24)=16 and x.IsDeleted = 0 order by y.Posted desc),
		LastTopicID = (select top 1 y.TopicID from [{databaseOwner}].[{objectQualifier}Topic] x join [{databaseOwner}].[{objectQualifier}Message] y on y.TopicID=x.TopicID where x.ForumID = @ForumID and (y.Flags & 24)=16 and x.IsDeleted = 0order by y.Posted desc),
		LastMessageID = (select top 1 y.MessageID from [{databaseOwner}].[{objectQualifier}Topic] x join [{databaseOwner}].[{objectQualifier}Message] y on y.TopicID=x.TopicID where x.ForumID = @ForumID and (y.Flags & 24)=16 and x.IsDeleted = 0order by y.Posted desc),
		LastUserID = (select top 1 y.UserID from [{databaseOwner}].[{objectQualifier}Topic] x join [{databaseOwner}].[{objectQualifier}Message] y on y.TopicID=x.TopicID where x.ForumID = @ForumID and (y.Flags & 24)=16 and x.IsDeleted = 0order by y.Posted desc),
		LastUserName = (select top 1 y.UserName from [{databaseOwner}].[{objectQualifier}Topic] x join [{databaseOwner}].[{objectQualifier}Message] y on y.TopicID=x.TopicID where x.ForumID = @ForumID and (y.Flags & 24)=16 and x.IsDeleted = 0order by y.Posted desc)
	where ForumID = @ForumID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}forum_updatestats](@ForumID int) as
begin
	update [{databaseOwner}].[{objectQualifier}Forum] set 
		NumPosts = (select count(1) from [{databaseOwner}].[{objectQualifier}Message] x join [{databaseOwner}].[{objectQualifier}Topic] y on y.TopicID=x.TopicID where y.ForumID = @ForumID and x.IsApproved = 1 and x.IsDeleted = 0 and y.IsDeleted = 0 ),
		NumTopics = (select count(distinct x.TopicID) from [{databaseOwner}].[{objectQualifier}Topic] x join [{databaseOwner}].[{objectQualifier}Message] y on y.TopicID=x.TopicID where x.ForumID = @ForumID and y.IsApproved = 1 and y.IsDeleted = 0 and x.IsDeleted = 0)
	where ForumID=@ForumID
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}forumaccess_group](@GroupID int) as
begin
	select 
		a.*,
		ForumName = b.Name,
		CategoryName = c.Name ,
		CategoryID = b.CategoryID,
		ParentID = b.ParentID 
	from 
		[{databaseOwner}].[{objectQualifier}ForumAccess] a
		inner join [{databaseOwner}].[{objectQualifier}Forum] b on b.ForumID=a.ForumID
		inner join [{databaseOwner}].[{objectQualifier}Category] c on c.CategoryID=b.CategoryID
	where 
		a.GroupID = @GroupID
	order by 
		c.SortOrder,
		b.SortOrder
end
GO

create procedure [{databaseOwner}].[{objectQualifier}forumaccess_list](@ForumID int) as
begin
	select 
		a.*,
		GroupName=b.Name 
	from 
		[{databaseOwner}].[{objectQualifier}ForumAccess] a 
		inner join [{databaseOwner}].[{objectQualifier}Group] b on b.GroupID=a.GroupID
	where 
		a.ForumID = @ForumID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}forumaccess_save](
	@ForumID			int,
	@GroupID			int,
	@AccessMaskID		int
) as
begin
	update [{databaseOwner}].[{objectQualifier}ForumAccess]
		set AccessMaskID=@AccessMaskID
	where 
		ForumID = @ForumID and 
		GroupID = @GroupID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}group_delete](@GroupID int) as
begin
	delete from [{databaseOwner}].[{objectQualifier}ForumAccess] where GroupID = @GroupID
	delete from [{databaseOwner}].[{objectQualifier}UserGroup] where GroupID = @GroupID
	delete from [{databaseOwner}].[{objectQualifier}Group] where GroupID = @GroupID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}group_list](@BoardID int,@GroupID int=null) as
begin
	if @GroupID is null
		select * from [{databaseOwner}].[{objectQualifier}Group] where BoardID=@BoardID
	else
		select * from [{databaseOwner}].[{objectQualifier}Group] where BoardID=@BoardID and GroupID=@GroupID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}group_member](@BoardID int,@UserID int) as
begin
	select 
		a.GroupID,
		a.Name,
		Member = (select count(1) from [{databaseOwner}].[{objectQualifier}UserGroup] x where x.UserID=@UserID and x.GroupID=a.GroupID)
	from
		[{databaseOwner}].[{objectQualifier}Group] a
	where
		a.BoardID=@BoardID
	order by
		a.Name
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}group_save](
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
		update [{databaseOwner}].[{objectQualifier}Group] set
			Name = @Name,
			Flags = @Flags
		where GroupID = @GroupID
	end
	else begin
		insert into [{databaseOwner}].[{objectQualifier}Group](Name,BoardID,Flags)
		values(@Name,@BoardID,@Flags);
		set @GroupID = SCOPE_IDENTITY()
		insert into [{databaseOwner}].[{objectQualifier}ForumAccess](GroupID,ForumID,AccessMaskID)
		select @GroupID,a.ForumID,@AccessMaskID from [{databaseOwner}].[{objectQualifier}Forum] a join [{databaseOwner}].[{objectQualifier}Category] b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID
	end
	select GroupID = @GroupID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}mail_create]
(
	@From nvarchar(50),
	@FromName nvarchar(50) = NULL,
	@To nvarchar(50),
	@ToName nvarchar(50) = NULL,
	@Subject nvarchar(100),
	@Body ntext,
	@BodyHtml ntext = NULL
)
AS 
BEGIN
	insert into [{databaseOwner}].[{objectQualifier}Mail]
		(FromUser,FromUserName,ToUser,ToUserName,Created,Subject,Body,BodyHtml)
	values
		(@From,@FromName,@To,@ToName,getdate(),@Subject,@Body,@BodyHtml)	
END
GO

create procedure [{databaseOwner}].[{objectQualifier}mail_createwatch]
(
	@TopicID int,
	@From nvarchar(50),
	@FromName nvarchar(50) = NULL,
	@Subject nvarchar(100),
	@Body ntext,
	@BodyHtml ntext = null,
	@UserID int
)
AS
BEGIN
	insert into [{databaseOwner}].[{objectQualifier}Mail](FromUser,FromUserName,ToUser,ToUserName,Created,Subject,Body,BodyHtml)
	select
		@From,
		@FromName,
		b.Email,
		b.Name,
		getdate(),
		@Subject,
		@Body,
		@BodyHtml
	from
		[{databaseOwner}].[{objectQualifier}WatchTopic] a
		inner join [{databaseOwner}].[{objectQualifier}User] b on b.UserID=a.UserID
	where
		b.UserID <> @UserID and
		a.TopicID = @TopicID and
		(a.LastMail is null or a.LastMail < b.LastVisit)
	
	insert into [{databaseOwner}].[{objectQualifier}Mail](FromUser,FromUserName,ToUser,ToUserName,Created,Subject,Body,BodyHtml)
	select
		@From,
		@FromName,
		b.Email,
		b.Name,
		getdate(),
		@Subject,
		@Body,
		@BodyHtml
	from
		[{databaseOwner}].[{objectQualifier}WatchForum] a
		inner join [{databaseOwner}].[{objectQualifier}User] b on b.UserID=a.UserID
		inner join [{databaseOwner}].[{objectQualifier}Topic] c on c.ForumID=a.ForumID
	where
		b.UserID <> @UserID and
		c.TopicID = @TopicID and
		(a.LastMail is null or a.LastMail < b.LastVisit) and
		not exists(select 1 from [{databaseOwner}].[{objectQualifier}WatchTopic] x where x.UserID=b.UserID and x.TopicID=c.TopicID)

	update [{databaseOwner}].[{objectQualifier}WatchTopic] set LastMail = getdate() 
	where TopicID = @TopicID
	and UserID <> @UserID
	
	update [{databaseOwner}].[{objectQualifier}WatchForum] set LastMail = getdate() 
	where ForumID = (select ForumID from [{databaseOwner}].[{objectQualifier}Topic] where TopicID = @TopicID)
	and UserID <> @UserID
end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}mail_delete](@MailID int) as
BEGIN
	DELETE FROM [{databaseOwner}].[{objectQualifier}Mail] WHERE MailID = @MailID
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}mail_list]
(
	@ProcessID int
)
AS
BEGIN
	UPDATE [{databaseOwner}].[{objectQualifier}Mail]
	SET 
		SendTries = SendTries + 1,
		SendAttempt = DATEADD(n,5,GETDATE()),
		ProcessID = @ProcessID
	WHERE
		MailID IN (SELECT TOP 10 MailID FROM [{databaseOwner}].[{objectQualifier}Mail] WHERE SendAttempt < GETDATE() OR SendAttempt IS NULL)

	-- now select all mail reserved for this process...
	SELECT * FROM [{databaseOwner}].[{objectQualifier}Mail] WHERE ProcessID = @ProcessID ORDER BY Created
END
GO

create procedure [{databaseOwner}].[{objectQualifier}message_approve](@MessageID int) as begin
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
		[{databaseOwner}].[{objectQualifier}Message] a
		inner join [{databaseOwner}].[{objectQualifier}Topic] b on b.TopicID=a.TopicID
	where
		a.MessageID = @MessageID

	-- update Message table, set meesage flag to approved
	update [{databaseOwner}].[{objectQualifier}Message] set Flags = Flags | 16 where MessageID = @MessageID

	-- update User table to increase postcount
	if exists(select 1 from [{databaseOwner}].[{objectQualifier}Forum] where ForumID=@ForumID and (Flags & 4)=0)
	begin
		update [{databaseOwner}].[{objectQualifier}User] set NumPosts = NumPosts + 1 where UserID = @UserID
		-- upgrade user, i.e. promote rank if conditions allow it
		exec [{databaseOwner}].[{objectQualifier}user_upgrade] @UserID
	end

	-- update Forum table with last topic/post info
	update [{databaseOwner}].[{objectQualifier}Forum] set
		LastPosted = @Posted,
		LastTopicID = @TopicID,
		LastMessageID = @MessageID,
		LastUserID = @UserID,
		LastUserName = @UserName
	where ForumID = @ForumID

	-- update Topic table with info about last post in topic
	update [{databaseOwner}].[{objectQualifier}Topic] set
		LastPosted = @Posted,
		LastMessageID = @MessageID,
		LastUserID = @UserID,
		LastUserName = @UserName,
		NumPosts = (select count(1) from [{databaseOwner}].[{objectQualifier}Message] x where x.TopicID=[{databaseOwner}].[{objectQualifier}Topic].TopicID and x.IsApproved = 1 and x.IsDeleted = 0)
	where TopicID = @TopicID
	
	-- update forum stats
	exec [{databaseOwner}].[{objectQualifier}forum_updatestats] @ForumID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}message_delete](@MessageID int, @EraseMessage bit = 0) as
begin
	declare @TopicID		int
	declare @ForumID		int
	declare @MessageCount	int
	declare @LastMessageID	int
	declare @UserID			int

	-- Find TopicID and ForumID
	select @TopicID=b.TopicID,@ForumID=b.ForumID,@UserID = a.UserID 
		from 
			[{databaseOwner}].[{objectQualifier}Message] a
			inner join [{databaseOwner}].[{objectQualifier}Topic] b on b.TopicID=a.TopicID
		where
			a.MessageID=@MessageID

	-- Update LastMessageID in Topic
	update [{databaseOwner}].[{objectQualifier}Topic] set 
		LastPosted = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	-- Update LastMessageID in Forum
	update [{databaseOwner}].[{objectQualifier}Forum] set 
		LastPosted = null,
		LastTopicID = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	-- should it be physically deleter or not?
	if (@EraseMessage = 1) begin
		delete [{databaseOwner}].[{objectQualifier}Attachment] where MessageID = @MessageID
		delete [{databaseOwner}].[{objectQualifier}MessageReported] where MessageID = @MessageID
		delete [{databaseOwner}].[{objectQualifier}MessageReportedAudit] where MessageID = @MessageID
		delete [{databaseOwner}].[{objectQualifier}Message] where MessageID = @MessageID
	end
	else begin
		-- "Delete" it only by setting deleted flag message
		update [{databaseOwner}].[{objectQualifier}Message] set Flags = Flags | 8 where MessageID = @MessageID
	end
	
	-- update user post count
	UPDATE [{databaseOwner}].[{objectQualifier}User] SET NumPosts = (SELECT count(MessageID) FROM [{databaseOwner}].[{objectQualifier}Message] WHERE UserID = @UserID AND IsDeleted = 0 AND IsApproved = 1) WHERE UserID = @UserID
	
	-- Delete topic if there are no more messages
	select @MessageCount = count(1) from [{databaseOwner}].[{objectQualifier}Message] where TopicID = @TopicID and (Flags & 8)=0
	if @MessageCount=0 exec [{databaseOwner}].[{objectQualifier}topic_delete] @TopicID, 1, @EraseMessage

	-- update lastpost
	exec [{databaseOwner}].[{objectQualifier}topic_updatelastpost] @ForumID,@TopicID
	exec [{databaseOwner}].[{objectQualifier}forum_updatestats] @ForumID

	-- update topic numposts
	update [{databaseOwner}].[{objectQualifier}Topic] set
		NumPosts = (select count(1) from [{databaseOwner}].[{objectQualifier}Message] x where x.TopicID=[{databaseOwner}].[{objectQualifier}Topic].TopicID and x.IsApproved = 1 and x.IsDeleted = 0)
	where TopicID = @TopicID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}message_findunread](@TopicID int,@LastRead datetime) as
begin
	select top 1 MessageID from [{databaseOwner}].[{objectQualifier}Message]
	where TopicID=@TopicID and Posted>@LastRead
	order by Posted
end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_getReplies](@MessageID int) as
BEGIN
	SELECT MessageID FROM [{databaseOwner}].[{objectQualifier}Message] WHERE ReplyTo = @MessageID
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_list](@MessageID int) AS
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
		[{databaseOwner}].[{objectQualifier}Message] a
		inner join [{databaseOwner}].[{objectQualifier}User] b on b.UserID = a.UserID
		inner join [{databaseOwner}].[{objectQualifier}Topic] c on c.TopicID = a.TopicID
		inner join [{databaseOwner}].[{objectQualifier}Forum] d on c.ForumID = d.ForumID
	WHERE
		a.MessageID = @MessageID
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_listreported](@MessageFlag int, @ForumID int) AS
BEGIN
	SELECT
		a.*,
		OriginalMessage = b.[Message],
		b.[Flags],
		b.[IsModeratorChanged],	
		UserName	= IsNull(b.UserName,d.Name),
		UserID = b.UserID,
		Posted		= b.Posted,
		Topic		= c.Topic,
		NumberOfReports = (SELECT count(LogID) FROM [{databaseOwner}].[{objectQualifier}MessageReportedAudit] WHERE [{databaseOwner}].[{objectQualifier}MessageReportedAudit].MessageID = a.MessageID)
	FROM
		[{databaseOwner}].[{objectQualifier}MessageReported] a
	INNER JOIN
		[{databaseOwner}].[{objectQualifier}Message] b ON a.MessageID = b.MessageID
	INNER JOIN
		[{databaseOwner}].[{objectQualifier}Topic] c ON b.TopicID = c.TopicID
	INNER JOIN
		[{databaseOwner}].[{objectQualifier}User] d ON b.UserID = d.UserID
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

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_report](@ReportFlag int, @MessageID int, @ReporterID int, @ReportedDate datetime ) AS
BEGIN
	
	IF NOT exists(SELECT MessageID from [{databaseOwner}].[{objectQualifier}MessageReportedAudit] WHERE MessageID=@MessageID AND UserID=@ReporterID)
		INSERT INTO [{databaseOwner}].[{objectQualifier}MessageReportedAudit](MessageID,UserID,Reported) VALUES (@MessageID,@ReporterID,@ReportedDate)

	IF NOT exists(SELECT MessageID FROM [{databaseOwner}].[{objectQualifier}MessageReported] WHERE MessageID=@MessageID)
	BEGIN
		INSERT INTO [{databaseOwner}].[{objectQualifier}MessageReported](MessageID, [Message])
		SELECT 
			a.MessageID,
			a.Message
		FROM
			[{databaseOwner}].[{objectQualifier}Message] a
		WHERE
			a.MessageID = @MessageID
	END

	-- update Message table to set message with flag Reported
	UPDATE [{databaseOwner}].[{objectQualifier}Message] SET Flags = Flags | POWER(2, @ReportFlag) WHERE MessageID = @MessageID

END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_reportresolve](@MessageFlag int, @MessageID int, @UserID int) AS
BEGIN
	UPDATE [{databaseOwner}].[{objectQualifier}MessageReported]
	SET Resolved = 1, ResolvedBy = @UserID, ResolvedDate = GETDATE()
	WHERE MessageID = @MessageID;
	
	/* Remove Flag */
	UPDATE [{databaseOwner}].[{objectQualifier}Message]
	SET Flags = Flags & (~POWER(2, @MessageFlag))
	WHERE MessageID = @MessageID;
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_reportcopyover](@MessageID int) AS
BEGIN
	UPDATE [{databaseOwner}].[{objectQualifier}MessageReported]
	SET [{databaseOwner}].[{objectQualifier}MessageReported].Message = m.Message
	FROM [{databaseOwner}].[{objectQualifier}MessageReported] mr
	JOIN [{databaseOwner}].[{objectQualifier}Message] m ON m.MessageID = mr.MessageID
	WHERE mr.MessageID = @MessageID;
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_save](
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
	FROM 
		[{databaseOwner}].[{objectQualifier}Topic] x
	INNER JOIN 
		[{databaseOwner}].[{objectQualifier}Forum] y ON y.ForumID=x.ForumID
	WHERE x.TopicID = @TopicID 

	IF @ReplyTo IS NULL
			SELECT @Position = 0, @Indent = 0 -- New thread

	ELSE IF @ReplyTo<0
		-- Find post to reply to AND indent of this post
		SELECT TOP 1 @ReplyTo = MessageID, @Indent = Indent+1
		FROM [{databaseOwner}].[{objectQualifier}Message]
		WHERE TopicID = @TopicID AND ReplyTo IS NULL
		ORDER BY Posted

	ELSE
		-- Got reply, find indent of this post
			SELECT @Indent=Indent+1
			FROM [{databaseOwner}].[{objectQualifier}Message]
			WHERE MessageID=@ReplyTo

	-- Find position
	IF @ReplyTo IS NOT NULL
    BEGIN
        DECLARE @temp INT
		
        SELECT @temp=ReplyTo,@Position=Position FROM [{databaseOwner}].[{objectQualifier}Message] WHERE MessageID=@ReplyTo

        IF @temp IS NULL
			-- We are replying to first post
            SELECT @Position=MAX(Position)+1 FROM [{databaseOwner}].[{objectQualifier}Message] WHERE TopicID=@TopicID

        ELSE
			-- Last position of replies to parent post
            SELECT @Position=MIN(Position) FROM [{databaseOwner}].[{objectQualifier}Message] WHERE ReplyTo=@temp AND Position>@Position

        -- No replies, THEN USE parent post's position+1
        IF @Position IS NULL
            SELECT @Position=Position+1 FROM [{databaseOwner}].[{objectQualifier}Message] WHERE MessageID=@ReplyTo
		-- Increase position of posts after this

        UPDATE [{databaseOwner}].[{objectQualifier}Message] SET Position=Position+1 WHERE TopicID=@TopicID AND Position>=@Position
    END

	-- Add points to Users total points
	UPDATE [{databaseOwner}].[{objectQualifier}User] SET Points = Points + 3 WHERE UserID = @UserID

	INSERT [{databaseOwner}].[{objectQualifier}Message] ( UserID, Message, TopicID, Posted, UserName, IP, ReplyTo, Position, Indent, Flags, BlogPostID)
	VALUES ( @UserID, @Message, @TopicID, @Posted, @UserName, @IP, @ReplyTo, @Position, @Indent, @Flags & ~16, @BlogPostID)

	SET @MessageID = SCOPE_IDENTITY()

	IF ((@ForumFlags & 8) = 0) OR ((@Flags & 16) = 16)
		EXEC [{databaseOwner}].[{objectQualifier}message_approve] @MessageID
END
	
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}message_unapproved](@ForumID int) as begin
	select
		MessageID	= b.MessageID,
		UserID		= b.UserID,
		UserName	= IsNull(b.UserName,c.Name),
		Posted		= b.Posted,
		Topic		= a.Topic,
		[Message]	= b.Message,
		[Flags]		= b.Flags,
		[IsModeratorChanged] = b.IsModeratorChanged
	from
		[{databaseOwner}].[{objectQualifier}Topic] a
		inner join [{databaseOwner}].[{objectQualifier}Message] b on b.TopicID = a.TopicID
		inner join [{databaseOwner}].[{objectQualifier}User] c on c.UserID = b.UserID
	where
		a.ForumID = @ForumID and
		(b.Flags & 16)=0 and
		(a.Flags & 8)=0 and
		(b.Flags & 8)=0
	order by
		a.Posted
end

GO

CREATE procedure [{databaseOwner}].[{objectQualifier}message_update](@MessageID int,@Priority int,@Subject nvarchar(100),@Flags int, @Message ntext, @Reason as nvarchar(100), @IsModeratorChanged bit, @OverrideApproval bit = null) as
begin
	declare @TopicID	int
	declare	@ForumFlags	int

	set @Flags = @Flags & ~16	
	
	select 
		@TopicID	= a.TopicID,
		@ForumFlags	= c.Flags
	from 
		[{databaseOwner}].[{objectQualifier}Message] a
		inner join [{databaseOwner}].[{objectQualifier}Topic] b on b.TopicID = a.TopicID
		inner join [{databaseOwner}].[{objectQualifier}Forum] c on c.ForumID = b.ForumID
	where 
		a.MessageID = @MessageID

	if (@OverrideApproval = 1 OR (@ForumFlags & 8)=0) set @Flags = @Flags | 16

	update [{databaseOwner}].[{objectQualifier}Message] set
		Message = @Message,
		Edited = getdate(),
		Flags = @Flags,
		IsModeratorChanged  = @IsModeratorChanged,
                EditReason = @Reason
	where
		MessageID = @MessageID

	if @Priority is not null begin
		update [{databaseOwner}].[{objectQualifier}Topic] set
			Priority = @Priority
		where
			TopicID = @TopicID
	end

	if not @Subject = '' and @Subject is not null begin
		update [{databaseOwner}].[{objectQualifier}Topic] set
			Topic = @Subject
		where
			TopicID = @TopicID
	end 
	
	-- If forum is moderated, make sure last post pointers are correct
	if (@ForumFlags & 8)<>0 exec [{databaseOwner}].[{objectQualifier}topic_updatelastpost]
end
GO

create procedure [{databaseOwner}].[{objectQualifier}nntpforum_delete](@NntpForumID int) as
begin
	delete from [{databaseOwner}].[{objectQualifier}NntpTopic] where NntpForumID = @NntpForumID
	delete from [{databaseOwner}].[{objectQualifier}NntpForum] where NntpForumID = @NntpForumID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}nntpforum_list](@BoardID int,@Minutes int=null,@NntpForumID int=null,@Active bit=null) as
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
		[{databaseOwner}].[{objectQualifier}NntpServer] a
		join [{databaseOwner}].[{objectQualifier}NntpForum] b on b.NntpServerID = a.NntpServerID
		join [{databaseOwner}].[{objectQualifier}Forum] c on c.ForumID = b.ForumID
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

create procedure [{databaseOwner}].[{objectQualifier}nntpforum_save](@NntpForumID int=null,@NntpServerID int,@GroupName nvarchar(100),@ForumID int,@Active bit) as
begin
	if @NntpForumID is null
		insert into [{databaseOwner}].[{objectQualifier}NntpForum](NntpServerID,GroupName,ForumID,LastMessageNo,LastUpdate,Active)
		values(@NntpServerID,@GroupName,@ForumID,0,getdate(),@Active)
	else
		update [{databaseOwner}].[{objectQualifier}NntpForum] set
			NntpServerID = @NntpServerID,
			GroupName = @GroupName,
			ForumID = @ForumID,
			Active = @Active
		where NntpForumID = @NntpForumID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}nntpforum_update](@NntpForumID int,@LastMessageNo int,@UserID int) as
begin
	declare	@ForumID	int
	
	select @ForumID=ForumID from [{databaseOwner}].[{objectQualifier}NntpForum] where NntpForumID=@NntpForumID

	update [{databaseOwner}].[{objectQualifier}NntpForum] set
		LastMessageNo = @LastMessageNo,
		LastUpdate = getdate()
	where NntpForumID = @NntpForumID

	update [{databaseOwner}].[{objectQualifier}Topic] set 
		NumPosts = (select count(1) from [{databaseOwner}].[{objectQualifier}message] x where x.TopicID=[{databaseOwner}].[{objectQualifier}Topic].TopicID and x.IsApproved = 1 and x.IsDeleted = 0)
	where ForumID=@ForumID

	--exec [{databaseOwner}].[{objectQualifier}user_upgrade] @UserID
	exec [{databaseOwner}].[{objectQualifier}forum_updatestats] @ForumID
	-- exec [{databaseOwner}].[{objectQualifier}topic_updatelastpost] @ForumID,null
end
GO

create procedure [{databaseOwner}].[{objectQualifier}nntpserver_delete](@NntpServerID int) as
begin
	delete from [{databaseOwner}].[{objectQualifier}NntpTopic] where NntpForumID in (select NntpForumID from [{databaseOwner}].[{objectQualifier}NntpForum] where NntpServerID = @NntpServerID)
	delete from [{databaseOwner}].[{objectQualifier}NntpForum] where NntpServerID = @NntpServerID
	delete from [{databaseOwner}].[{objectQualifier}NntpServer] where NntpServerID = @NntpServerID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}nntpserver_list](@BoardID int=null,@NntpServerID int=null) as
begin
	if @NntpServerID is null
		select * from [{databaseOwner}].[{objectQualifier}NntpServer] where BoardID=@BoardID order by Name
	else
		select * from [{databaseOwner}].[{objectQualifier}NntpServer] where NntpServerID=@NntpServerID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}nntpserver_save](
	@NntpServerID 	int=null,
	@BoardID	int,
	@Name		nvarchar(50),
	@Address	nvarchar(100),
	@Port		int,
	@UserName	nvarchar(50)=null,
	@UserPass	nvarchar(50)=null
) as begin
	if @NntpServerID is null
		insert into [{databaseOwner}].[{objectQualifier}NntpServer](Name,BoardID,Address,Port,UserName,UserPass)
		values(@Name,@BoardID,@Address,@Port,@UserName,@UserPass)
	else
		update [{databaseOwner}].[{objectQualifier}NntpServer] set
			Name = @Name,
			Address = @Address,
			Port = @Port,
			UserName = @UserName,
			UserPass = @UserPass
		where NntpServerID = @NntpServerID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}nntptopic_list](@Thread char(32)) as
begin
	select
		a.*
	from
		[{databaseOwner}].[{objectQualifier}NntpTopic] a
	where
		a.Thread = @Thread
end
GO

create procedure [{databaseOwner}].[{objectQualifier}nntptopic_savemessage](
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

	select @ForumID=ForumID from [{databaseOwner}].[{objectQualifier}NntpForum] where NntpForumID=@NntpForumID

	if exists(select 1 from [{databaseOwner}].[{objectQualifier}NntpTopic] where Thread=@Thread)
	begin
		-- thread exists
		select @TopicID=TopicID from [{databaseOwner}].[{objectQualifier}NntpTopic] where Thread=@Thread
	end else
	begin
		-- thread doesn't exists
		insert into [{databaseOwner}].[{objectQualifier}Topic](ForumID,UserID,UserName,Posted,Topic,Views,Priority,NumPosts)
		values(@ForumID,@UserID,@UserName,@Posted,@Topic,0,0,0)
		set @TopicID=SCOPE_IDENTITY()

		insert into [{databaseOwner}].[{objectQualifier}NntpTopic](NntpForumID,Thread,TopicID)
		values(@NntpForumID,@Thread,@TopicID)
	end

	-- save message
	insert into [{databaseOwner}].[{objectQualifier}Message](TopicID,UserID,UserName,Posted,Message,IP,Position,Indent)
	values(@TopicID,@UserID,@UserName,@Posted,@Body,@IP,0,0)
	set @MessageID=SCOPE_IDENTITY()

	-- update user
	if exists(select 1 from [{databaseOwner}].[{objectQualifier}Forum] where ForumID=@ForumID and (Flags & 4)=0)
	begin
		update [{databaseOwner}].[{objectQualifier}User] set NumPosts=NumPosts+1 where UserID=@UserID
	end
	
	-- update topic
	update [{databaseOwner}].[{objectQualifier}Topic] set 
		LastPosted		= @Posted,
		LastMessageID	= @MessageID,
		LastUserID		= @UserID,
		LastUserName	= @UserName
	where TopicID=@TopicID	
	-- update forum
	update [{databaseOwner}].[{objectQualifier}Forum] set
		LastPosted		= @Posted,
		LastTopicID	= @TopicID,
		LastMessageID	= @MessageID,
		LastUserID		= @UserID,
		LastUserName	= @UserName
	where ForumID=@ForumID and (LastPosted is null or LastPosted<@Posted)
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}pageload](
	@SessionID	nvarchar(24),
	@BoardID	int,
	@UserKey	nvarchar(64),
	@IP			nvarchar(15),
	@Location	nvarchar(50),
	@Browser	nvarchar(50),
	@Platform	nvarchar(50),
	@CategoryID	int = null,
	@ForumID	int = null,
	@TopicID	int = null,
	@MessageID	int = null,
	@DontTrack	bit = 0
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
		select @UserID = UserID from [{databaseOwner}].[{objectQualifier}User] where BoardID=@BoardID and (Flags & 4)<>0
		set @rowcount=@@rowcount
		if @rowcount<>1
		begin
			raiserror('Found %d possible guest users. There should be one and only one user marked as guest.',16,1,@rowcount)
		end
		set @IsGuest = 1
		set @UserBoardID = @BoardID
	end else
	begin
		select @UserID = UserID, @UserBoardID = BoardID from [{databaseOwner}].[{objectQualifier}User] where BoardID=@BoardID and ProviderUserKey=@UserKey
		set @IsGuest = 0
	end
	-- Check valid ForumID
	if @ForumID is not null and not exists(select 1 from [{databaseOwner}].[{objectQualifier}Forum] where ForumID=@ForumID) begin
		set @ForumID = null
	end
	-- Check valid CategoryID
	if @CategoryID is not null and not exists(select 1 from [{databaseOwner}].[{objectQualifier}Category] where CategoryID=@CategoryID) begin
		set @CategoryID = null
	end
	-- Check valid MessageID
	if @MessageID is not null and not exists(select 1 from [{databaseOwner}].[{objectQualifier}Message] where MessageID=@MessageID) begin
		set @MessageID = null
	end
	-- Check valid TopicID
	if @TopicID is not null and not exists(select 1 from [{databaseOwner}].[{objectQualifier}Topic] where TopicID=@TopicID) begin
		set @TopicID = null
	end
	
	-- get previous visit
	if @IsGuest=0 begin
		select @PreviousVisit = LastVisit from [{databaseOwner}].[{objectQualifier}User] where UserID = @UserID
	end
	
	-- update last visit
	update [{databaseOwner}].[{objectQualifier}User] set 
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
			[{databaseOwner}].[{objectQualifier}Message] a
			inner join [{databaseOwner}].[{objectQualifier}Topic] b on b.TopicID = a.TopicID
			inner join [{databaseOwner}].[{objectQualifier}Forum] c on c.ForumID = b.ForumID
			inner join [{databaseOwner}].[{objectQualifier}Category] d on d.CategoryID = c.CategoryID
		where
			a.MessageID = @MessageID and
			d.BoardID = @BoardID
	end
	else if @TopicID is not null begin
		select 
			@CategoryID = b.CategoryID,
			@ForumID = a.ForumID 
		from 
			[{databaseOwner}].[{objectQualifier}Topic] a
			inner join [{databaseOwner}].[{objectQualifier}Forum] b on b.ForumID = a.ForumID
			inner join [{databaseOwner}].[{objectQualifier}Category] c on c.CategoryID = b.CategoryID
		where 
			a.TopicID = @TopicID and
			c.BoardID = @BoardID
	end
	else if @ForumID is not null begin
		select
			@CategoryID = a.CategoryID
		from
			[{databaseOwner}].[{objectQualifier}Forum] a
			inner join [{databaseOwner}].[{objectQualifier}Category] b on b.CategoryID = a.CategoryID
		where
			a.ForumID = @ForumID and
			b.BoardID = @BoardID
	end
	-- update active
	if @DontTrack != 1 and @UserID is not null and @UserBoardID=@BoardID begin
		if exists(select 1 from [{databaseOwner}].[{objectQualifier}Active] where SessionID=@SessionID and BoardID=@BoardID)
		begin
			update [{databaseOwner}].[{objectQualifier}Active] set
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
			insert into [{databaseOwner}].[{objectQualifier}Active](SessionID,BoardID,UserID,IP,Login,LastActive,Location,ForumID,TopicID,Browser,Platform)
			values(@SessionID,@BoardID,@UserID,@IP,getdate(),getdate(),@Location,@ForumID,@TopicID,@Browser,@Platform)
			-- update max user stats
			exec [{databaseOwner}].[{objectQualifier}active_updatemaxstats] @BoardID
		end
		-- remove duplicate users
		if @IsGuest=0
			delete from [{databaseOwner}].[{objectQualifier}Active] where UserID=@UserID and BoardID=@BoardID and SessionID<>@SessionID
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
		CategoryName		= (select Name from [{databaseOwner}].[{objectQualifier}Category] where CategoryID = @CategoryID),
		ForumID				= @ForumID,
		ForumName			= (select Name from [{databaseOwner}].[{objectQualifier}Forum] where ForumID = @ForumID),
		TopicID				= @TopicID,
		TopicName			= (select Topic from [{databaseOwner}].[{objectQualifier}Topic] where TopicID = @TopicID),
		MailsPending		= (select count(1) from [{databaseOwner}].[{objectQualifier}Mail]),
		Incoming			= (select count(1) from [{databaseOwner}].[{objectQualifier}UserPMessage] where UserID=a.UserID and IsRead=0),
		ForumTheme			= (select ThemeURL from [{databaseOwner}].[{objectQualifier}Forum] where ForumID = @ForumID)
	from
		[{databaseOwner}].[{objectQualifier}User] a
		left join [{databaseOwner}].[{objectQualifier}vaccess] x on x.UserID=a.UserID and x.ForumID=IsNull(@ForumID,0)
	where
		a.UserID = @UserID
end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_delete](@UserPMessageID int, @FromOutbox bit = 0) as
BEGIN
	DECLARE @PMessageID int

	SET @PMessageID = (SELECT TOP 1 PMessageID FROM [{databaseOwner}].[{objectQualifier}UserPMessage] where [UserPMessageID] = @UserPMessageID);

	IF @FromOutbox = 1
	BEGIN
		-- set IsInOutbox bit which will remove it from the senders outbox
		UPDATE [{databaseOwner}].[{objectQualifier}UserPMessage] SET [Flags] = ([Flags] ^ 2) WHERE UserPMessageID = @UserPMessageID AND IsInOutbox = 1
	END
	ELSE
	BEGIN		
		DELETE FROM [{databaseOwner}].[{objectQualifier}UserPMessage] WHERE [UserPMessageID] = @UserPMessageID
		DELETE FROM [{databaseOwner}].[{objectQualifier}PMessage] WHERE [PMessageID] = @PMessageID
	END
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_info] as
begin
	select
		NumRead	= (select count(1) from [{databaseOwner}].[{objectQualifier}UserPMessage] where IsRead<>0),
		NumUnread = (select count(1) from [{databaseOwner}].[{objectQualifier}UserPMessage] where IsRead=0),
		NumTotal = (select count(1) from [{databaseOwner}].[{objectQualifier}UserPMessage])
end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_list](@FromUserID int=null,@ToUserID int=null,@UserPMessageID int=null) AS
BEGIN
	SELECT PMessageID, UserPMessageID, FromUserID, FromUser, ToUserID, ToUser, Created, Subject, Body, Flags, IsRead, IsInOutbox, IsArchived
		FROM [{databaseOwner}].[{objectQualifier}PMessageView]
		WHERE	((@UserPMessageID IS NOT NULL AND UserPMessageID=@UserPMessageID) OR 
				 (@ToUserID   IS NOT NULL AND ToUserID = @ToUserID) OR 
				 (@FromUserID IS NOT NULL AND FromUserID = @FromUserID))
		ORDER BY Created DESC
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_markread](@UserPMessageID int=null)
AS
BEGIN
	UPDATE [{databaseOwner}].[{objectQualifier}UserPMessage] SET [Flags] = [Flags] | 1 WHERE UserPMessageID = @UserPMessageID AND IsRead = 0
END
GO

create procedure [{databaseOwner}].[{objectQualifier}pmessage_prune](@DaysRead int,@DaysUnread int) as
begin
	delete from [{databaseOwner}].[{objectQualifier}UserPMessage]
	where IsRead<>0
	and datediff(dd,(select Created from [{databaseOwner}].[{objectQualifier}PMessage] x where x.PMessageID=[{databaseOwner}].[{objectQualifier}UserPMessage].PMessageID),getdate())>@DaysRead

	delete from [{databaseOwner}].[{objectQualifier}UserPMessage]
	where IsRead=0
	and datediff(dd,(select Created from [{databaseOwner}].[{objectQualifier}PMessage] x where x.PMessageID=[{databaseOwner}].[{objectQualifier}UserPMessage].PMessageID),getdate())>@DaysUnread

	delete from [{databaseOwner}].[{objectQualifier}PMessage]
	where not exists(select 1 from [{databaseOwner}].[{objectQualifier}UserPMessage] x where x.PMessageID=[{databaseOwner}].[{objectQualifier}PMessage].PMessageID)
end
GO

create procedure [{databaseOwner}].[{objectQualifier}pmessage_save](
	@FromUserID	int,
	@ToUserID	int,
	@Subject	nvarchar(100),
	@Body		ntext,
	@Flags		int
) as
begin
	declare @PMessageID int
	declare @UserID int

	insert into [{databaseOwner}].[{objectQualifier}PMessage](FromUserID,Created,Subject,Body,Flags)
	values(@FromUserID,getdate(),@Subject,@Body,@Flags)

	set @PMessageID = SCOPE_IDENTITY()
	if (@ToUserID = 0)
	begin
		insert into [{databaseOwner}].[{objectQualifier}UserPMessage](UserID,PMessageID,Flags)
		select
				a.UserID,@PMessageID,2
		from
				{objectQualifier}User a
				join [{databaseOwner}].[{objectQualifier}UserGroup] b on b.UserID=a.UserID
				join [{databaseOwner}].[{objectQualifier}Group] c on c.GroupID=b.GroupID where
				(c.Flags & 2)=0 and
				c.BoardID=(select BoardID from [{databaseOwner}].[{objectQualifier}User] x where x.UserID=@FromUserID) and a.UserID<>@FromUserID
		group by
				a.UserID
	end
	else
	begin
		insert into [{databaseOwner}].[{objectQualifier}UserPMessage](UserID,PMessageID,Flags) values(@ToUserID,@PMessageID,2)
	end
end
GO


CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_archive](@UserPMessageID int = NULL) AS
BEGIN
	-- set IsArchived bit
	UPDATE [{databaseOwner}].[{objectQualifier}UserPMessage] SET [Flags] = ([Flags] | 4) WHERE UserPMessageID = @UserPMessageID AND IsArchived = 0
END
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}poll_save](
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
	insert into [{databaseOwner}].[{objectQualifier}Poll](Question,Closes) values(@Question,@Closes)
	set @PollID = SCOPE_IDENTITY()
	if @Choice1<>'' and @Choice1 is not null
		insert into [{databaseOwner}].[{objectQualifier}Choice](PollID,Choice,Votes)
		values(@PollID,@Choice1,0)
	if @Choice2<>'' and @Choice2 is not null
		insert into [{databaseOwner}].[{objectQualifier}Choice](PollID,Choice,Votes)
		values(@PollID,@Choice2,0)
	if @Choice3<>'' and @Choice3 is not null
		insert into [{databaseOwner}].[{objectQualifier}Choice](PollID,Choice,Votes)
		values(@PollID,@Choice3,0)
	if @Choice4<>'' and @Choice4 is not null
		insert into [{databaseOwner}].[{objectQualifier}Choice](PollID,Choice,Votes)
		values(@PollID,@Choice4,0)
	if @Choice5<>'' and @Choice5 is not null
		insert into [{databaseOwner}].[{objectQualifier}Choice](PollID,Choice,Votes)
		values(@PollID,@Choice5,0)
	if @Choice6<>'' and @Choice6 is not null
		insert into [{databaseOwner}].[{objectQualifier}Choice](PollID,Choice,Votes)
		values(@PollID,@Choice6,0)
	if @Choice7<>'' and @Choice7 is not null
		insert into [{databaseOwner}].[{objectQualifier}Choice](PollID,Choice,Votes)
		values(@PollID,@Choice7,0)
	if @Choice8<>'' and @Choice8 is not null
		insert into [{databaseOwner}].[{objectQualifier}Choice](PollID,Choice,Votes)
		values(@PollID,@Choice8,0)
	if @Choice9<>'' and @Choice9 is not null
		insert into [{databaseOwner}].[{objectQualifier}Choice](PollID,Choice,Votes)
		values(@PollID,@Choice9,0)
	select PollID = @PollID
end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}poll_stats](@PollID int) AS
BEGIN
	SELECT
		a.PollID,
		b.Question,
		b.Closes,
		a.ChoiceID,
		a.Choice,
		a.Votes,
		Stats = (select 100 * a.Votes / case sum(x.Votes) when 0 then 1 else sum(x.Votes) end from [{databaseOwner}].[{objectQualifier}Choice] x where x.PollID=a.PollID)
	FROM
		[{databaseOwner}].[{objectQualifier}Choice] a
	INNER JOIN 
		[{databaseOwner}].[{objectQualifier}Poll] b ON b.PollID = a.PollID
	WHERE
		b.PollID = @PollID
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}pollvote_check](@PollID int, @UserID int = NULL,@RemoteIP nvarchar(10) = NULL) AS

	IF @UserID IS NULL
	BEGIN
		IF @RemoteIP IS NOT NULL
		BEGIN
			-- check by remote IP
			SELECT PollVoteID FROM [{databaseOwner}].[{objectQualifier}PollVote] WHERE PollID = @PollID AND RemoteIP = @RemoteIP
		END
	END
	ELSE
	BEGIN
		-- check by userid or remote IP
		SELECT PollVoteID FROM [{databaseOwner}].[{objectQualifier}PollVote] WHERE PollID = @PollID AND (UserID = @UserID OR RemoteIP = @RemoteIP)
	END
GO

create procedure [{databaseOwner}].[{objectQualifier}post_last10user](@BoardID int,@UserID int,@PageUserID int) as
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
		[{databaseOwner}].[{objectQualifier}Message] a
		join [{databaseOwner}].[{objectQualifier}User] b on b.UserID=a.UserID
		join [{databaseOwner}].[{objectQualifier}Topic] c on c.TopicID=a.TopicID
		join [{databaseOwner}].[{objectQualifier}Forum] d on d.ForumID=c.ForumID
		join [{databaseOwner}].[{objectQualifier}Category] e on e.CategoryID=d.CategoryID
		join [{databaseOwner}].[{objectQualifier}vaccess] x on x.ForumID=d.ForumID
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

create procedure [{databaseOwner}].[{objectQualifier}post_list](@TopicID int,@UpdateViewCount smallint=1, @ShowDeleted bit = 1) as
begin
	set nocount on
	if @UpdateViewCount>0
		update [{databaseOwner}].[{objectQualifier}Topic] set Views = Views + 1 where TopicID = @TopicID
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
		a.IsDeleted,
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
		HasAttachments	= (select count(1) from [{databaseOwner}].[{objectQualifier}Attachment] x where x.MessageID=a.MessageID),
		HasAvatarImage = (select count(1) from [{databaseOwner}].[{objectQualifier}User] x where x.UserID=b.UserID and AvatarImage is not null)
	from
		[{databaseOwner}].[{objectQualifier}Message] a
		join [{databaseOwner}].[{objectQualifier}User] b on b.UserID=a.UserID
		join [{databaseOwner}].[{objectQualifier}Topic] d on d.TopicID=a.TopicID
		join [{databaseOwner}].[{objectQualifier}Forum] g on g.ForumID=d.ForumID
		join [{databaseOwner}].[{objectQualifier}Category] h on h.CategoryID=g.CategoryID
		join [{databaseOwner}].[{objectQualifier}Rank] c on c.RankID=b.RankID
	where
		a.TopicID = @TopicID
		AND a.IsApproved = 1
		AND (a.IsDeleted = 0 OR (@showdeleted = 1 AND a.IsDeleted = 1))
	order by
		a.Posted asc
end
GO

create procedure [{databaseOwner}].[{objectQualifier}post_list_reverse10](@TopicID int) as
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
		[{databaseOwner}].[{objectQualifier}Message] a 
		inner join [{databaseOwner}].[{objectQualifier}User] b on b.UserID = a.UserID
		inner join [{databaseOwner}].[{objectQualifier}Topic] d on d.TopicID = a.TopicID
	where
		(a.Flags & 24)=16 and
		a.TopicID = @TopicID
	order by
		a.Posted desc
end
GO

create procedure [{databaseOwner}].[{objectQualifier}rank_delete](@RankID int) as begin
	delete from [{databaseOwner}].[{objectQualifier}Rank] where RankID = @RankID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}rank_list](@BoardID int,@RankID int=null) as begin
	if @RankID is null
		select
			a.*
		from
			[{databaseOwner}].[{objectQualifier}Rank] a
		where
			a.BoardID=@BoardID
		order by
			a.MinPosts,
			a.Name
	else
		select
			a.*
		from
			[{databaseOwner}].[{objectQualifier}Rank] a
		where
			a.RankID = @RankID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}rank_save](
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
		update [{databaseOwner}].[{objectQualifier}Rank] set
			Name = @Name,
			Flags = @Flags,
			MinPosts = @MinPosts,
			RankImage = @RankImage
		where RankID = @RankID
	end
	else begin
		insert into [{databaseOwner}].[{objectQualifier}Rank](BoardID,Name,Flags,MinPosts,RankImage)
		values(@BoardID,@Name,@Flags,@MinPosts,@RankImage);
	end
end
GO

create procedure [{databaseOwner}].[{objectQualifier}registry_list](@Name nvarchar(50) = null,@BoardID int = null) as
BEGIN
	if @BoardID is null
	begin
		IF @Name IS NULL OR @Name = ''
		BEGIN
			SELECT * FROM [{databaseOwner}].[{objectQualifier}Registry] where BoardID is null
		END ELSE
		BEGIN
			SELECT * FROM [{databaseOwner}].[{objectQualifier}Registry] WHERE LOWER(Name) = LOWER(@Name) and BoardID is null
		END
	end else 
	begin
		IF @Name IS NULL OR @Name = ''
		BEGIN
			SELECT * FROM [{databaseOwner}].[{objectQualifier}Registry] where BoardID=@BoardID
		END ELSE
		BEGIN
			SELECT * FROM [{databaseOwner}].[{objectQualifier}Registry] WHERE LOWER(Name) = LOWER(@Name) and BoardID=@BoardID
		END
	end
END
GO

create procedure [{databaseOwner}].[{objectQualifier}registry_save](
	@Name nvarchar(50),
	@Value ntext = NULL,
	@BoardID int = null
) AS
BEGIN
	if @BoardID is null
	begin
		if exists(select 1 from [{databaseOwner}].[{objectQualifier}Registry] where lower(Name)=lower(@Name))
			update [{databaseOwner}].[{objectQualifier}Registry] set Value = @Value where lower(Name)=lower(@Name) and BoardID is null
		else
		begin
			insert into [{databaseOwner}].[{objectQualifier}Registry](Name,Value) values(lower(@Name),@Value)
		end
	end else
	begin
		if exists(select 1 from [{databaseOwner}].[{objectQualifier}Registry] where lower(Name)=lower(@Name) and BoardID=@BoardID)
			update [{databaseOwner}].[{objectQualifier}Registry] set Value = @Value where lower(Name)=lower(@Name) and BoardID=@BoardID
		else
		begin
			insert into [{databaseOwner}].[{objectQualifier}Registry](Name,Value,BoardID) values(lower(@Name),@Value,@BoardID)
		end
	end
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}replace_words_delete](@ID int) AS
BEGIN
	DELETE FROM [{databaseOwner}].[{objectQualifier}replace_words] WHERE id = @ID
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}replace_words_list]
(
	@BoardID int,
	@ID int = null
)
AS BEGIN
	IF (@ID IS NOT NULL AND @ID <> 0)
		SELECT * FROM [{databaseOwner}].[{objectQualifier}Replace_Words] WHERE BoardID = @BoardID AND ID = @ID
	ELSE
		SELECT * FROM [{databaseOwner}].[{objectQualifier}Replace_Words] WHERE BoardID = @BoardID
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}replace_words_save]
(
	@BoardID int,
	@ID int = null,
	@BadWord nvarchar(255),
	@GoodWord nvarchar(255)
)
AS
BEGIN
	IF (@ID IS NOT NULL AND @ID <> 0)
	BEGIN
		UPDATE [{databaseOwner}].[{objectQualifier}replace_words] SET BadWord = @BadWord, GoodWord = @GoodWord WHERE ID = @ID		
	END
	ELSE BEGIN
		INSERT INTO [{databaseOwner}].[{objectQualifier}replace_words]
			(BoardID,BadWord,GoodWord)
		VALUES
			(@BoardID,@BadWord,@GoodWord)
	END
END
GO

create procedure [{databaseOwner}].[{objectQualifier}smiley_delete](@SmileyID int=null) as begin
	if @SmileyID is not null
		delete from [{databaseOwner}].[{objectQualifier}Smiley] where SmileyID=@SmileyID
	else
		delete from [{databaseOwner}].[{objectQualifier}Smiley]
end
GO

create procedure [{databaseOwner}].[{objectQualifier}smiley_list](@BoardID int,@SmileyID int=null) as
begin
	if @SmileyID is null
		select * from [{databaseOwner}].[{objectQualifier}Smiley] where BoardID=@BoardID order by SortOrder, LEN(Code) desc
	else
		select * from [{databaseOwner}].[{objectQualifier}Smiley] where SmileyID=@SmileyID order by SortOrder
end
GO

create procedure [{databaseOwner}].[{objectQualifier}smiley_listunique](@BoardID int) as
begin
	select 
		Icon, 
		Emoticon,
		Code = (select top 1 Code from [{databaseOwner}].[{objectQualifier}Smiley] x where x.Icon=[{databaseOwner}].[{objectQualifier}Smiley].Icon),
		SortOrder = (select top 1 SortOrder from [{databaseOwner}].[{objectQualifier}Smiley] x where x.Icon=[{databaseOwner}].[{objectQualifier}Smiley].Icon order by x.SortOrder asc)
	from 
		[{databaseOwner}].[{objectQualifier}Smiley]
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

create procedure [{databaseOwner}].[{objectQualifier}smiley_save](@SmileyID int=null,@BoardID int,@Code nvarchar(10),@Icon nvarchar(50),@Emoticon nvarchar(50),@SortOrder tinyint,@Replace smallint=0) as begin
	if @SmileyID is not null begin
		update [{databaseOwner}].[{objectQualifier}Smiley] set Code = @Code, Icon = @Icon, Emoticon = @Emoticon, SortOrder = @SortOrder where SmileyID = @SmileyID
	end
	else begin
		if @Replace>0
			delete from [{databaseOwner}].[{objectQualifier}Smiley] where Code=@Code

		if not exists(select 1 from [{databaseOwner}].[{objectQualifier}Smiley] where BoardID=@BoardID and Code=@Code)
			insert into [{databaseOwner}].[{objectQualifier}Smiley](BoardID,Code,Icon,Emoticon,SortOrder) values(@BoardID,@Code,@Icon,@Emoticon,@SortOrder)
	end
end
GO

create procedure [{databaseOwner}].[{objectQualifier}smiley_resort](@BoardID int,@SmileyID int,@Move int) as
begin
	declare @Position int

	SELECT @Position=SortOrder FROM [{databaseOwner}].[{objectQualifier}Smiley] WHERE BoardID=@BoardID and SmileyID=@SmileyID

	if (@Position is null) return

	if (@Move > 0) begin
		update [{databaseOwner}].[{objectQualifier}Smiley]
			set SortOrder=SortOrder-1
			where BoardID=@BoardID and 
				SortOrder between @Position and (@Position + @Move) and
				SortOrder between 1 and 255
	end
	else if (@Move < 0) begin
		update [{databaseOwner}].[{objectQualifier}Smiley]
			set SortOrder=SortOrder+1
			where BoardID=@BoardID and 
				SortOrder between (@Position+@Move) and @Position and
				SortOrder between 0 and 254
	end

	SET @Position = @Position + @Move

	if (@Position>255) SET @Position = 255
	else if (@Position<0) SET @Position = 0

	update [{databaseOwner}].[{objectQualifier}Smiley]
		set SortOrder=@Position
		where BoardID=@BoardID and 
			SmileyID=@SmileyID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}system_initialize](
	@Name		nvarchar(50),
	@TimeZone	int,
	@ForumEmail	nvarchar(50),
	@SmtpServer	nvarchar(50),
	@User		nvarchar(50),
	@Userkey	nvarchar(50)
	
) as 
begin
	DECLARE @tmpValue AS nvarchar(100)

	-- initalize required 'registry' settings
	EXEC [{databaseOwner}].[{objectQualifier}registry_save] 'Version','1'
	EXEC [{databaseOwner}].[{objectQualifier}registry_save] 'VersionName','1.0.0'
	SET @tmpValue = CAST(@TimeZone AS nvarchar(100))
	EXEC [{databaseOwner}].[{objectQualifier}registry_save] 'TimeZone', @tmpValue
	EXEC [{databaseOwner}].[{objectQualifier}registry_save] 'SmtpServer', @SmtpServer
	EXEC [{databaseOwner}].[{objectQualifier}registry_save] 'ForumEmail', @ForumEmail

	-- initalize new board
	EXEC [{databaseOwner}].[{objectQualifier}board_create] @Name, '','',@User,@UserKey,1
end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}system_updateversion]
(
	@Version		int,
	@VersionName	nvarchar(50)
) 
AS
BEGIN

	DECLARE @tmpValue AS nvarchar(100)
	SET @tmpValue = CAST(@Version AS nvarchar(100))
	EXEC [{databaseOwner}].[{objectQualifier}registry_save] 'Version', @tmpValue
	EXEC [{databaseOwner}].[{objectQualifier}registry_save] 'VersionName',@VersionName

END
GO

create procedure [{databaseOwner}].[{objectQualifier}topic_active](@BoardID int,@UserID int,@Since datetime,@CategoryID int=null) as
begin
	select
		c.ForumID,
		c.TopicID,
		c.Posted,
		LinkTopicID = IsNull(c.TopicMovedID,c.TopicID),
		Subject = c.Topic,
		c.UserID,
		Starter = IsNull(c.UserName,b.Name),
		NumPostsDeleted = (SELECT COUNT(1) FROM [{databaseOwner}].[{objectQualifier}Message] mes WHERE mes.TopicID = c.TopicID AND mes.IsDeleted = 1 AND mes.IsApproved = 1 AND ((@UserID IS NOT NULL AND mes.UserID = @UserID) OR (@UserID IS NULL)) ),
		Replies = (select count(1) from [{databaseOwner}].[{objectQualifier}Message] x where x.TopicID=c.TopicID and (x.Flags & 8)=0) - 1,
		Views = c.Views,
		LastPosted = c.LastPosted,
		LastUserID = c.LastUserID,
		LastUserName = IsNull(c.LastUserName,(select Name from [{databaseOwner}].[{objectQualifier}User] x where x.UserID=c.LastUserID)),
		LastMessageID = c.LastMessageID,
		LastTopicID = c.TopicID,
		TopicFlags = c.Flags,
		c.Priority,
		c.PollID,
		ForumName = d.Name,
		c.TopicMovedID,
		ForumFlags = d.Flags,
		FirstMessage = (SELECT TOP 1 CAST([Message] as nvarchar(1000)) FROM [{databaseOwner}].[{objectQualifier}Message] mes2 where mes2.TopicID = IsNull(c.TopicMovedID,c.TopicID) AND mes2.Position = 0)
	from
		[{databaseOwner}].[{objectQualifier}Topic] c
		join [{databaseOwner}].[{objectQualifier}User] b on b.UserID=c.UserID
		join [{databaseOwner}].[{objectQualifier}Forum] d on d.ForumID=c.ForumID
		join [{databaseOwner}].[{objectQualifier}vaccess] x on x.ForumID=d.ForumID
		join [{databaseOwner}].[{objectQualifier}Category] e on e.CategoryID=d.CategoryID
	where
		@Since < c.LastPosted and
		x.UserID = @UserID and
		x.ReadAccess <> 0 and
		e.BoardID = @BoardID and
		(@CategoryID is null or e.CategoryID=@CategoryID) and
		c.IsDeleted = 0
	order by
		d.Name asc,
		Priority desc,
		LastPosted desc
end
GO

create procedure [{databaseOwner}].[{objectQualifier}topic_delete] (@TopicID int,@UpdateLastPost bit=1,@EraseTopic bit=0) 
as
begin
	SET NOCOUNT ON
	declare @ForumID int
	declare @pollID int
	
	select @ForumID=ForumID from  [{databaseOwner}].[{objectQualifier}Topic] where TopicID=@TopicID
	update  [{databaseOwner}].[{objectQualifier}Topic] set LastMessageID = null where TopicID = @TopicID
	update  [{databaseOwner}].[{objectQualifier}Forum] set 
		LastTopicID = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null,
		LastPosted = null
	where LastMessageID in (select MessageID from  [{databaseOwner}].[{objectQualifier}Message] where TopicID = @TopicID)
	update  [{databaseOwner}].[{objectQualifier}Active] set TopicID = null where TopicID = @TopicID
	
	--remove polls	
	select @pollID = pollID from  [{databaseOwner}].[{objectQualifier}topic] where TopicID = @TopicID
	if (@pollID is not null)
	begin
		delete from  [{databaseOwner}].[{objectQualifier}choice] where PollID = @PollID
		update  [{databaseOwner}].[{objectQualifier}topic] set PollID = null where TopicID = @TopicID
		delete from  [{databaseOwner}].[{objectQualifier}poll] where PollID = @PollID	
	end	
	
	--delete messages and topics
	delete from  [{databaseOwner}].[{objectQualifier}nntptopic] where TopicID = @TopicID
	delete from  [{databaseOwner}].[{objectQualifier}topic] where TopicMovedID = @TopicID

	if @EraseTopic = 0
	begin
		update  [{databaseOwner}].[{objectQualifier}topic] set Flags = Flags | 8 where TopicID = @TopicID
	end
	else
	begin
		delete  [{databaseOwner}].[{objectQualifier}Attachment] where MessageID IN (select MessageID from  [{databaseOwner}].[{objectQualifier}message] where TopicID = @TopicID) 
		delete  [{databaseOwner}].[{objectQualifier}Message] where TopicID = @TopicID
		delete  [{databaseOwner}].[{objectQualifier}WatchTopic] where TopicID = @TopicID
		delete  [{databaseOwner}].[{objectQualifier}Topic] where TopicID = @TopicID
		delete  [{databaseOwner}].[{objectQualifier}MessageReportedAudit] where MessageID IN (select MessageID from  [{databaseOwner}].[{objectQualifier}message] where TopicID = @TopicID) 
		delete  [{databaseOwner}].[{objectQualifier}MessageReported] where MessageID IN (select MessageID from  [{databaseOwner}].[{objectQualifier}message] where TopicID = @TopicID)
	end
		
	--commit
	if @UpdateLastPost<>0
		exec  [{databaseOwner}].[{objectQualifier}forum_updatelastpost] @ForumID
	
	if @ForumID is not null
		exec  [{databaseOwner}].[{objectQualifier}forum_updatestats] @ForumID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}topic_findnext](@TopicID int) as
begin
	declare @LastPosted datetime
	declare @ForumID int
	select @LastPosted = LastPosted, @ForumID = ForumID from [{databaseOwner}].[{objectQualifier}Topic] where TopicID = @TopicID
	select top 1 TopicID from [{databaseOwner}].[{objectQualifier}Topic] where LastPosted>@LastPosted and ForumID = @ForumID AND (Flags & 8) = 0 order by LastPosted asc
end
GO

create procedure [{databaseOwner}].[{objectQualifier}topic_findprev](@TopicID int) AS 
BEGIN
	DECLARE @LastPosted datetime
	DECLARE @ForumID int
	SELECT @LastPosted = LastPosted, @ForumID = ForumID FROM [{databaseOwner}].[{objectQualifier}Topic] WHERE TopicID = @TopicID
	SELECT TOP 1 TopicID from [{databaseOwner}].[{objectQualifier}Topic] where LastPosted<@LastPosted AND ForumID = @ForumID AND (Flags & 8) = 0 ORDER BY LastPosted DESC
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}topic_info]
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
			SELECT * FROM [{databaseOwner}].[{objectQualifier}Topic]
		ELSE
			SELECT * FROM [{databaseOwner}].[{objectQualifier}Topic] WHERE (Flags & 8) = 0
	END
	ELSE
	BEGIN
		IF @ShowDeleted = 1 
			SELECT * FROM [{databaseOwner}].[{objectQualifier}Topic] WHERE TopicID = @TopicID
		ELSE
			SELECT * FROM [{databaseOwner}].[{objectQualifier}Topic] WHERE TopicID = @TopicID AND (Flags & 8) = 0		
	END
END
GO


CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}topic_announcements]
(
	@BoardID int,
	@NumPosts int,
	@UserID int
)
AS
BEGIN

	DECLARE @SQL nvarchar(500)

	SET @SQL = 'SELECT DISTINCT TOP ' + convert(varchar, @NumPosts) + ' t.Topic, t.LastPosted, t.TopicID, t.LastMessageID FROM'
	SET @SQL = @SQL + ' [{databaseOwner}].[{objectQualifier}Topic] t INNER JOIN [{databaseOwner}].[{objectQualifier}Category] c INNER JOIN [{databaseOwner}].[{objectQualifier}Forum] f ON c.CategoryID = f.CategoryID ON t.ForumID = f.ForumID'
	SET @SQL = @SQL + ' join [{databaseOwner}].[{objectQualifier}vaccess] v on v.ForumID=f.ForumID'
	SET @SQL = @SQL + ' WHERE c.BoardID = ' + convert(varchar, @BoardID) + ' AND v.UserID=' + convert(varchar,@UserID) + ' AND (v.ReadAccess <> 0 or (f.Flags & 2) = 0) AND (t.Flags & 8) != 8 AND (t.Priority = 2) ORDER BY t.LastPosted DESC'

	EXEC(@SQL)	

END
GO


CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}topic_latest]
(
	@BoardID int,
	@NumPosts int,
	@UserID int
)
AS
BEGIN
	SET ROWCOUNT @NumPosts
	
	SELECT
		t.LastPosted,
		t.ForumID,
		f.Name as Forum,
		t.Topic,
		t.TopicID,
		t.LastMessageID,
		t.LastUserID,
		t.NumPosts,
		LastUserName = IsNull(t.LastUserName,(select [Name] from [{databaseOwner}].[{objectQualifier}User] x where x.UserID = t.LastUserID))
	FROM 
		[{databaseOwner}].[{objectQualifier}Topic] t
	INNER JOIN
		[{databaseOwner}].[{objectQualifier}Forum] f ON t.ForumID = f.ForumID	
	INNER JOIN
		[{databaseOwner}].[{objectQualifier}Category] c ON c.CategoryID = f.CategoryID
	JOIN
		[{databaseOwner}].[{objectQualifier}vaccess] v ON v.ForumID=f.ForumID
	WHERE
		c.BoardID = @BoardID
		AND t.TopicMovedID is NULL
		AND v.UserID=@UserID
		AND (v.ReadAccess <> 0)
		AND t.IsDeleted != 1
		AND t.LastPosted IS NOT NULL
	ORDER BY
		t.LastPosted DESC;
END
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}topic_list]
(
	@ForumID int,
	@UserID int = null,
	@Announcement smallint,
	@Date datetime=null,
	@Offset int,
	@Count int
)
AS
begin
	create table #data(
		RowNo	int identity primary key not null,
		TopicID	int not null
	)

	insert into #data(TopicID)
	select
		c.TopicID
	from
		[{databaseOwner}].[{objectQualifier}Topic] c join [{databaseOwner}].[{objectQualifier}User] b on b.UserID=c.UserID join [{databaseOwner}].[{objectQualifier}Forum] d on d.ForumID=c.ForumID
	where
		c.ForumID = @ForumID
	and
		(@Date is null or c.Posted>=@Date or c.LastPosted>=@Date or Priority>0) 
	and
		((@Announcement=1 and c.Priority=2) or (@Announcement=0 and c.Priority<>2) or (@Announcement<0)) 
	and	
		(c.TopicMovedID is not null or c.NumPosts > 0) 
	and
		c.IsDeleted = 0
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
		NumPostsDeleted = (SELECT COUNT(1) FROM [{databaseOwner}].[{objectQualifier}Message] mes WHERE mes.TopicID = c.TopicID AND mes.IsDeleted = 1 AND mes.IsApproved = 1 AND ((@UserID IS NOT NULL AND mes.UserID = @UserID) OR (@UserID IS NULL)) ),
		[Views] = c.Views,
		LastPosted = c.LastPosted,
		LastUserID = c.LastUserID,
		LastUserName = IsNull(c.LastUserName,(select Name from [{databaseOwner}].[{objectQualifier}User] x where x.UserID=c.LastUserID)),
		LastMessageID = c.LastMessageID,
		LastTopicID = c.TopicID,
		TopicFlags = c.Flags,
		c.Priority,
		c.PollID,
		ForumFlags = d.Flags,
		FirstMessage = (SELECT TOP 1 CAST([Message] as nvarchar(1000)) FROM [{databaseOwner}].[{objectQualifier}Message] mes2 where mes2.TopicID = IsNull(c.TopicMovedID,c.TopicID) AND mes2.Position = 0)
	from
		[{databaseOwner}].[{objectQualifier}Topic] c 
		join [{databaseOwner}].[{objectQualifier}User] b on b.UserID=c.UserID 
		join [{databaseOwner}].[{objectQualifier}Forum] d on d.ForumID=c.ForumID 
		join #data e on e.TopicID=c.TopicID
	where
		e.RowNo between @Offset+1 and @Offset + @Count
	order by
		e.RowNo
end
GO

create procedure [{databaseOwner}].[{objectQualifier}topic_listmessages](@TopicID int) as
begin
	select * from [{databaseOwner}].[{objectQualifier}Message]
	where TopicID = @TopicID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}topic_lock](@TopicID int,@Locked bit) as
begin
	if @Locked<>0
		update [{databaseOwner}].[{objectQualifier}Topic] set Flags = Flags | 1 where TopicID = @TopicID
	else
		update [{databaseOwner}].[{objectQualifier}Topic] set Flags = Flags & ~1 where TopicID = @TopicID
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}topic_move](@TopicID int,@ForumID int,@ShowMoved bit) AS
begin
    declare @OldForumID int

    select @OldForumID = ForumID from [{databaseOwner}].[{objectQualifier}Topic] where TopicID = @TopicID

    if @ShowMoved<>0 begin
        -- create a moved message
        insert into [{databaseOwner}].[{objectQualifier}Topic](ForumID,UserID,UserName,Posted,Topic,Views,Flags,Priority,PollID,TopicMovedID,LastPosted,NumPosts)
        select ForumID,UserID,UserName,Posted,Topic,0,Flags,Priority,PollID,@TopicID,LastPosted,0
        from [{databaseOwner}].[{objectQualifier}Topic] where TopicID = @TopicID
    end

    -- move the topic
    update [{databaseOwner}].[{objectQualifier}Topic] set ForumID = @ForumID where TopicID = @TopicID

    -- update last posts
    exec [{databaseOwner}].[{objectQualifier}forum_updatelastpost] @OldForumID
    exec [{databaseOwner}].[{objectQualifier}forum_updatelastpost] @ForumID
    
    -- update stats
    exec [{databaseOwner}].[{objectQualifier}forum_updatestats] @OldForumID
    exec [{databaseOwner}].[{objectQualifier}forum_updatestats] @ForumID
    
end
GO

create procedure [{databaseOwner}].[{objectQualifier}topic_prune](@ForumID int=null,@Days int) as
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
		exec [{databaseOwner}].[{objectQualifier}topic_delete] @TopicID,0
		set @Count = @Count + 1
		fetch @c into @TopicID
	end
	close @c
	deallocate @c

	-- This takes forever with many posts...
	--exec [{databaseOwner}].[{objectQualifier}topic_updatelastpost]

	select Count = @Count
end
GO

create procedure [{databaseOwner}].[{objectQualifier}topic_save](
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
	insert into [{databaseOwner}].[{objectQualifier}Topic](ForumID,Topic,UserID,Posted,Views,Priority,PollID,UserName,NumPosts)
	values(@ForumID,@Subject,@UserID,@Posted,0,@Priority,@PollID,@UserName,0)

	-- get its id
	set @TopicID = SCOPE_IDENTITY()
	
	-- add message to the topic
	exec [{databaseOwner}].[{objectQualifier}message_save] @TopicID,@UserID,@Message,@UserName,@IP,@Posted,null,@BlogPostID,@Flags,@MessageID output

	select TopicID = @TopicID, MessageID = @MessageID
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}topic_updatelastpost]
(@ForumID int=null,@TopicID int=null) as
begin

    if @TopicID is not null
        update [{databaseOwner}].[{objectQualifier}Topic] set
            LastPosted = (select top 1 x.Posted from [{databaseOwner}].[{objectQualifier}Message] x where x.TopicID=[{databaseOwner}].[{objectQualifier}Topic].TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastMessageID = (select top 1 x.MessageID from [{databaseOwner}].[{objectQualifier}Message] x where x.TopicID=[{databaseOwner}].[{objectQualifier}Topic].TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserID = (select top 1 x.UserID from [{databaseOwner}].[{objectQualifier}Message] x where x.TopicID=[{databaseOwner}].[{objectQualifier}Topic].TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserName = (select top 1 x.UserName from [{databaseOwner}].[{objectQualifier}Message] x where x.TopicID=[{databaseOwner}].[{objectQualifier}Topic].TopicID and (x.Flags & 24)=16 order by Posted desc)
        where TopicID = @TopicID
    else
        update [{databaseOwner}].[{objectQualifier}Topic] set
            LastPosted = (select top 1 x.Posted from [{databaseOwner}].[{objectQualifier}Message] x where x.TopicID=[{databaseOwner}].[{objectQualifier}Topic].TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastMessageID = (select top 1 x.MessageID from [{databaseOwner}].[{objectQualifier}Message] x where x.TopicID=[{databaseOwner}].[{objectQualifier}Topic].TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserID = (select top 1 x.UserID from [{databaseOwner}].[{objectQualifier}Message] x where x.TopicID=[{databaseOwner}].[{objectQualifier}Topic].TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserName = (select top 1 x.UserName from [{databaseOwner}].[{objectQualifier}Message] x where x.TopicID=[{databaseOwner}].[{objectQualifier}Topic].TopicID and (x.Flags & 24)=16 order by Posted desc)
        where TopicMovedID is null
        and (@ForumID is null or ForumID=@ForumID)

    exec [{databaseOwner}].[{objectQualifier}forum_updatelastpost] @ForumID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}user_accessmasks](@BoardID int,@UserID int) as
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
			[{databaseOwner}].[{objectQualifier}User] a 
			join [{databaseOwner}].[{objectQualifier}UserGroup] b on b.UserID=a.UserID
			join [{databaseOwner}].[{objectQualifier}Group] c on c.GroupID=b.GroupID
			join [{databaseOwner}].[{objectQualifier}ForumAccess] d on d.GroupID=c.GroupID
			join [{databaseOwner}].[{objectQualifier}AccessMask] e on e.AccessMaskID=d.AccessMaskID
			join [{databaseOwner}].[{objectQualifier}Forum] f on f.ForumID=d.ForumID
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
			[{databaseOwner}].[{objectQualifier}User] a 
			join [{databaseOwner}].[{objectQualifier}UserForum] b on b.UserID=a.UserID
			join [{databaseOwner}].[{objectQualifier}AccessMask] c on c.AccessMaskID=b.AccessMaskID
			join [{databaseOwner}].[{objectQualifier}Forum] d on d.ForumID=b.ForumID
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

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_activity_rank]
(
	@BoardID AS int,
	@DisplayNumber AS int,
	@StartDate AS datetime
)
AS
BEGIN
	DECLARE @GuestUserID int

	SET ROWCOUNT @DisplayNumber

	SET @GuestUserID =
	(SELECT top 1
		a.UserID
	from
		[{databaseOwner}].[{objectQualifier}User] a
		inner join [{databaseOwner}].[{objectQualifier}UserGroup] b on b.UserID = a.UserID
		inner join [{databaseOwner}].[{objectQualifier}Group] c on b.GroupID = c.GroupID
	where
		a.BoardID = @BoardID and
		(c.Flags & 2)<>0
	)

	SELECT
		counter.[ID],
		u.[Name],
		counter.[NumOfPosts]
	FROM
		[{databaseOwner}].[{objectQualifier}User] u inner join
		(
			SELECT m.UserID as ID, Count(m.UserID) as NumOfPosts FROM [{databaseOwner}].[{objectQualifier}Message] m
			WHERE m.Posted >= @StartDate
			GROUP BY m.UserID
		) AS counter ON u.UserID = counter.ID
	WHERE
		u.BoardID = @BoardID and u.UserID != @GuestUserID
	ORDER BY
		NumOfPosts DESC

	SET ROWCOUNT 0
END
GO


create PROCEDURE [{databaseOwner}].[{objectQualifier}user_addpoints] (@UserID int,@Points int) AS
BEGIN
	UPDATE [{databaseOwner}].[{objectQualifier}User] SET Points = Points + @Points WHERE UserID = @UserID
END

GO

create procedure [{databaseOwner}].[{objectQualifier}user_adminsave]
(@BoardID int,@UserID int,@Name nvarchar(50),@Email nvarchar(50),@Flags int,@RankID int) as
begin
	update [{databaseOwner}].[{objectQualifier}User] set
		Name = @Name,
		Email = @Email,
		RankID = @RankID,
		Flags = @Flags
	where UserID = @UserID
	select UserID = @UserID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}user_approve](@UserID int) as
begin
	declare @CheckEmailID int
	declare @Email nvarchar(50)

	select 
		@CheckEmailID = CheckEmailID,
		@Email = Email
	from
		[{databaseOwner}].[{objectQualifier}CheckEmail]
	where
		UserID = @UserID

	-- Update new user email
	update [{databaseOwner}].[{objectQualifier}User] set Email = @Email, Flags = Flags | 2 where UserID = @UserID
	delete [{databaseOwner}].[{objectQualifier}CheckEmail] where CheckEmailID = @CheckEmailID
	select convert(bit,1)
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}user_approveall](@BoardID int) as
begin

	DECLARE userslist CURSOR FOR 
		SELECT UserID FROM [{databaseOwner}].[{objectQualifier}User] WHERE BoardID=@BoardID AND (Flags & 2)=0
		FOR READ ONLY


	OPEN userslist

	DECLARE @UserID int

	FETCH userslist INTO @UserID

	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXEC [{databaseOwner}].[{objectQualifier}user_approve] @UserID
		FETCH userslist INTO @UserID		
	END

	CLOSE userslist

end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_aspnet](@BoardID int,@UserName nvarchar(50),@Email nvarchar(50),@ProviderUserKey nvarchar(64),@IsApproved bit) as
BEGIN
	SET NOCOUNT ON

	DECLARE @UserID int, @RankID int, @approvedFlag int

	SET @approvedFlag = 0;
	IF (@IsApproved = 1) SET @approvedFlag = 2;	
	
	IF EXISTS(SELECT 1 FROM [{databaseOwner}].[{objectQualifier}User] where BoardID=@BoardID and ([ProviderUserKey]=@ProviderUserKey OR [Name] = @UserName))
	BEGIN
		SELECT TOP 1 @UserID = UserID FROM [{databaseOwner}].[{objectQualifier}User] WHERE [BoardID]=@BoardID and ([ProviderUserKey]=@ProviderUserKey OR [Name] = @UserName)

		UPDATE [{databaseOwner}].[{objectQualifier}User] SET 
			[Name] = @UserName,
			Email = @Email,
			[ProviderUserKey] = @ProviderUserKey,
			Flags = Flags | @approvedFlag
		WHERE
			UserID = @UserID
	END ELSE
	BEGIN
		SELECT @RankID = RankID from [{databaseOwner}].[{objectQualifier}Rank] where (Flags & 1)<>0 and BoardID=@BoardID

		INSERT INTO [{databaseOwner}].[{objectQualifier}User](BoardID,RankID,[Name],Password,Email,Joined,LastVisit,NumPosts,TimeZone,Flags,ProviderUserKey) 
		VALUES(@BoardID,@RankID,@UserName,'-',@Email,getdate(),getdate(),0,0,@approvedFlag,@ProviderUserKey)
	
		set @UserID = SCOPE_IDENTITY()
	
	END
	
	SELECT UserID=@UserID
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_migrate]
(
	@UserID int,
	@ProviderUserKey nvarchar(64),
	@UpdateProvider bit = 0
)
AS
BEGIN
	DECLARE @Password nvarchar(255), @IsApproved bit, @LastActivity datetime, @Joined datetime
	
	UPDATE {objectQualifier}User SET ProviderUserKey = @ProviderUserKey where UserID = @UserID

	IF (@UpdateProvider = 1)
	BEGIN
		SELECT
			@Password = Password,
			@IsApproved = (CASE (Flags & 2) WHEN 2 THEN 1 ELSE 0 END),
			@LastActivity = LastVisit,
			@Joined = Joined
		FROM
			{objectQualifier}User
		WHERE
			UserID = @UserID
		
		UPDATE
			{objectQualifier}prov_Membership
		SET
			Password = @Password,
			PasswordFormat = '1',
			LastActivity = @LastActivity,
			IsApproved = @IsApproved,
			Joined = @Joined
		WHERE
			UserID = @ProviderUserKey
	END
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_avatarimage]
(
	@UserID int
)
AS
BEGIN
	SELECT
		UserID,
		AvatarImage,
		AvatarImageType
	FROM
		[{databaseOwner}].[{objectQualifier}User]
	WHERE
		UserID = @UserID
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_changepassword](@UserID int,@OldPassword nvarchar(32),@NewPassword nvarchar(32)) as
begin
	declare @CurrentOld nvarchar(32)
	select @CurrentOld = Password from [{databaseOwner}].[{objectQualifier}User] where UserID = @UserID
	if @CurrentOld<>@OldPassword begin
		select Success = convert(bit,0)
		return
	end
	update [{databaseOwner}].[{objectQualifier}User] set Password = @NewPassword where UserID = @UserID
	select Success = convert(bit,1)
end
GO

CREATE PROC [{databaseOwner}].[{objectQualifier}user_pmcount]
	@UserID int
AS
BEGIN
	DECLARE @Count int

	-- get count of pm's in user's sent items
	SELECT 
		@Count=COUNT(*) 
	FROM 
		[{databaseOwner}].[{objectQualifier}UserPMessage] a
	INNER JOIN [{databaseOwner}].[{objectQualifier}PMessage] b ON a.PMessageID=b.PMessageID
	WHERE 
		(a.Flags & 2)<>0 AND
		b.FromUserID = @UserID

	-- add count of pm's in user's inbox
	SELECT 
		@Count=@Count+COUNT(*) 
	FROM 
		[{databaseOwner}].[{objectQualifier}UserPMessage] 
	WHERE 
		UserID = @UserID

	-- return total count
	SELECT @Count
END
GO

create procedure [{databaseOwner}].[{objectQualifier}user_delete](@UserID int) as
begin
	declare @GuestUserID	int
	declare @UserName		nvarchar(50)
	declare @GuestCount		int

	select @UserName = Name from [{databaseOwner}].[{objectQualifier}User] where UserID=@UserID

	select top 1
		@GuestUserID = a.UserID
	from
		[{databaseOwner}].[{objectQualifier}User] a
		inner join [{databaseOwner}].[{objectQualifier}UserGroup] b on b.UserID = a.UserID
		inner join [{databaseOwner}].[{objectQualifier}Group] c on b.GroupID = c.GroupID
	where
		(c.Flags & 2)<>0

	select 
		@GuestCount = count(1) 
	from 
		[{databaseOwner}].[{objectQualifier}UserGroup] a
		join [{databaseOwner}].[{objectQualifier}Group] b on b.GroupID=a.GroupID
	where
		(b.Flags & 2)<>0

	if @GuestUserID=@UserID and @GuestCount=1 begin
		return
	end

	update [{databaseOwner}].[{objectQualifier}Message] set UserName=@UserName,UserID=@GuestUserID where UserID=@UserID
	update [{databaseOwner}].[{objectQualifier}Topic] set UserName=@UserName,UserID=@GuestUserID where UserID=@UserID
	update [{databaseOwner}].[{objectQualifier}Topic] set LastUserName=@UserName,LastUserID=@GuestUserID where LastUserID=@UserID
	update [{databaseOwner}].[{objectQualifier}Forum] set LastUserName=@UserName,LastUserID=@GuestUserID where LastUserID=@UserID

	delete from [{databaseOwner}].[{objectQualifier}Active] where UserID=@UserID
	delete from [{databaseOwner}].[{objectQualifier}EventLog] where UserID=@UserID	
	delete from [{databaseOwner}].[{objectQualifier}UserPMessage] where UserID=@UserID
	delete from [{databaseOwner}].[{objectQualifier}PMessage] where FromUserID=@UserID AND PMessageID NOT IN (select PMessageID FROM [{databaseOwner}].[{objectQualifier}PMessage])
	-- set messages as from guest so the User can be deleted
	update [{databaseOwner}].[{objectQualifier}PMessage] SET FromUserID = @GuestUserID WHERE FromUserID = @UserID
	delete from [{databaseOwner}].[{objectQualifier}CheckEmail] where UserID = @UserID
	delete from [{databaseOwner}].[{objectQualifier}WatchTopic] where UserID = @UserID
	delete from [{databaseOwner}].[{objectQualifier}WatchForum] where UserID = @UserID
	delete from [{databaseOwner}].[{objectQualifier}UserGroup] where UserID = @UserID
	--ABOT CHANGED
	--Delete UserForums entries Too 
	delete from [{databaseOwner}].[{objectQualifier}UserForum] where UserID = @UserID
	delete from [{databaseOwner}].[{objectQualifier}IgnoredUser] where UserID = @UserID OR IgnoredUserID = @UserID
	--END ABOT CHANGED 09.04.2004
	delete from [{databaseOwner}].[{objectQualifier}User] where UserID = @UserID
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}user_deleteavatar](@UserID int) as begin
	UPDATE
		[{databaseOwner}].[{objectQualifier}User]
	SET
		AvatarImage = null,
		Avatar = null,
		AvatarImageType = null
	WHERE
		UserID = @UserID
END
GO

create procedure [{databaseOwner}].[{objectQualifier}user_deleteold](@BoardID int) as
begin
	declare @Since datetime

	set @Since = getdate()

	delete from [{databaseOwner}].[{objectQualifier}EventLog]  where UserID in(select UserID from [{databaseOwner}].[{objectQualifier}User] where BoardID=@BoardID and [{databaseOwner}].[{objectQualifier}bitset](Flags,2)=0 and datediff(day,Joined,@Since)>2)
	delete from [{databaseOwner}].[{objectQualifier}CheckEmail] where UserID in(select UserID from [{databaseOwner}].[{objectQualifier}User] where BoardID=@BoardID and [{databaseOwner}].[{objectQualifier}bitset](Flags,2)=0 and datediff(day,Joined,@Since)>2)
	delete from [{databaseOwner}].[{objectQualifier}UserGroup] where UserID in(select UserID from [{databaseOwner}].[{objectQualifier}User] where BoardID=@BoardID and [{databaseOwner}].[{objectQualifier}bitset](Flags,2)=0 and datediff(day,Joined,@Since)>2)
	delete from [{databaseOwner}].[{objectQualifier}User] where BoardID=@BoardID and [{databaseOwner}].[{objectQualifier}bitset](Flags,2)=0 and datediff(day,Joined,@Since)>2
end
GO

create procedure [{databaseOwner}].[{objectQualifier}user_emails](@BoardID int,@GroupID int=null) as
begin
	if @GroupID = 0 set @GroupID = null
	if @GroupID is null
		select 
			a.Email 
		from 
			[{databaseOwner}].[{objectQualifier}User] a
		where 
			a.Email is not null and 
			a.BoardID = @BoardID and
			a.Email is not null and 
			a.Email<>''
	else
		select 
			a.Email 
		from 
			{objectQualifier}User a join [{databaseOwner}].[{objectQualifier}UserGroup] b on b.UserID=a.UserID
		where 
			b.GroupID = @GroupID and 
			a.Email is not null and 
			a.Email<>''
end
GO

create procedure [{databaseOwner}].[{objectQualifier}user_find](@BoardID int,@Filter bit,@UserName nvarchar(50)=null,@Email nvarchar(50)=null) as
begin
	if @Filter<>0
	begin
		if @UserName is not null
			set @UserName = '%' + @UserName + '%'

		select 
			a.*,
			IsGuest = (select count(1) from [{databaseOwner}].[{objectQualifier}UserGroup] x join [{databaseOwner}].[{objectQualifier}Group] y on x.GroupID=y.GroupID where x.UserID=a.UserID and (y.Flags & 2)<>0)
		from 
			[{databaseOwner}].[{objectQualifier}User] a
		where 
			a.BoardID=@BoardID and
			(@UserName is not null and a.Name like @UserName) or (@Email is not null and Email like @Email)
		order by
			a.Name
	end else
	begin
		select 
			a.UserID,
			IsGuest = (select count(1) from [{databaseOwner}].[{objectQualifier}UserGroup] x join [{databaseOwner}].[{objectQualifier}Group] y on x.GroupID=y.GroupID where x.UserID=a.UserID and (y.Flags & 2)<>0)
		from 
			[{databaseOwner}].[{objectQualifier}User] a
		where 
			a.BoardID=@BoardID and
			((@UserName is not null and a.Name=@UserName) or (@Email is not null and Email=@Email))
	end
end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_getpoints] (@UserID int) AS
BEGIN
	SELECT Points FROM [{databaseOwner}].[{objectQualifier}User] WHERE UserID = @UserID
END
GO

create procedure [{databaseOwner}].[{objectQualifier}user_getsignature](@UserID int) as
begin
	select Signature from [{databaseOwner}].[{objectQualifier}User] where UserID = @UserID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}user_guest]
(
	@BoardID int
)
as
begin
	select top 1
		a.UserID
	from
		[{databaseOwner}].[{objectQualifier}User] a
		inner join [{databaseOwner}].[{objectQualifier}UserGroup] b on b.UserID = a.UserID
		inner join [{databaseOwner}].[{objectQualifier}Group] c on b.GroupID = c.GroupID
	where
		a.BoardID = @BoardID and
		(c.Flags & 2)<>0
end
GO

create procedure [{databaseOwner}].[{objectQualifier}user_list](@BoardID int,@UserID int=null,@Approved bit=null,@GroupID int=null,@RankID int=null) as
begin
	if @UserID is not null
		select 
			a.*,
			a.NumPosts,
			b.RankID,
			RankName = b.Name,
			NumDays = datediff(d,a.Joined,getdate())+1,
			NumPostsForum = (select count(1) from [{databaseOwner}].[{objectQualifier}Message] x where (x.Flags & 24)=16),
			HasAvatarImage = (select count(1) from [{databaseOwner}].[{objectQualifier}User] x where x.UserID=a.UserID and AvatarImage is not null),
			IsAdmin	= IsNull(c.IsAdmin,0),
			IsGuest	= IsNull(a.Flags & 4,0),
			IsHostAdmin	= IsNull(a.Flags & 1,0),
			IsForumModerator	= IsNull(c.IsForumModerator,0),
			IsModerator		= IsNull(c.IsModerator,0)
		from 
			[{databaseOwner}].[{objectQualifier}User] a
			join [{databaseOwner}].[{objectQualifier}Rank] b on b.RankID=a.RankID
			left join [{databaseOwner}].[{objectQualifier}vaccess] c on c.UserID=a.UserID
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
			IsAdmin = (select count(1) from [{databaseOwner}].[{objectQualifier}UserGroup] x join [{databaseOwner}].[{objectQualifier}Group] y on y.GroupID=x.GroupID where x.UserID=a.UserID and (y.Flags & 1)<>0),
			IsGuest	= IsNull(a.Flags & 4,0),
			IsHostAdmin	= IsNull(a.Flags & 1,0),
			b.RankID,
			RankName = b.Name
		from 
			[{databaseOwner}].[{objectQualifier}User] a
			join [{databaseOwner}].[{objectQualifier}Rank] b on b.RankID=a.RankID
		where 
			a.BoardID = @BoardID and
			(@Approved is null or (@Approved=0 and (a.Flags & 2)=0) or (@Approved=1 and (a.Flags & 2)=2))
		order by 
			a.Name
	else
		select 
			a.*,
			a.NumPosts,
			IsAdmin = (select count(1) from [{databaseOwner}].[{objectQualifier}UserGroup] x join [{databaseOwner}].[{objectQualifier}Group] y on y.GroupID=x.GroupID where x.UserID=a.UserID and (y.Flags & 1)<>0),
			IsGuest	= IsNull(a.Flags & 4,0),
			IsHostAdmin	= IsNull(a.Flags & 1,0),
			b.RankID,
			RankName = b.Name
		from 
			[{databaseOwner}].[{objectQualifier}User] a
			join [{databaseOwner}].[{objectQualifier}Rank] b on b.RankID=a.RankID
		where 
			a.BoardID = @BoardID and
			(@Approved is null or (@Approved=0 and (a.Flags & 2)=0) or (@Approved=1 and (a.Flags & 2)=2)) and
			(@GroupID is null or exists(select 1 from [{databaseOwner}].[{objectQualifier}UserGroup] x where x.UserID=a.UserID and x.GroupID=@GroupID)) and
			(@RankID is null or a.RankID=@RankID)
		order by 
			a.Name
end
GO

create procedure [{databaseOwner}].[{objectQualifier}user_login](@BoardID int,@Name nvarchar(50),@Password nvarchar(32)) as
begin
	declare @UserID int

	-- Try correct board first
	if exists(select UserID from [{databaseOwner}].[{objectQualifier}User] where Name=@Name and Password=@Password and BoardID=@BoardID and (Flags & 2)=2)
	begin
		select UserID from [{databaseOwner}].[{objectQualifier}User] where Name=@Name and Password=@Password and BoardID=@BoardID and (Flags & 2)=2
		return
	end

	if not exists(select UserID from [{databaseOwner}].[{objectQualifier}User] where Name=@Name and Password=@Password and (BoardID=@BoardID or (Flags & 3)=3))
		set @UserID=null
	else
		select 
			@UserID=UserID 
		from 
			[{databaseOwner}].[{objectQualifier}User]
		where 
			Name=@Name and 
			Password=@Password and 
			(BoardID=@BoardID or (Flags & 1)=1) and
			(Flags & 2)=2

	select @UserID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}user_nntp](@BoardID int,@UserName nvarchar(50),@Email nvarchar(50)) as
begin
	declare @UserID int

	set @UserName = @UserName + ' (NNTP)'

	select
		@UserID=UserID
	from
		[{databaseOwner}].[{objectQualifier}User]
	where
		BoardID=@BoardID and
		Name=@UserName

	if @@ROWCOUNT<1
	begin
		exec [{databaseOwner}].[{objectQualifier}user_save] 0,@BoardID,@UserName,@Email,null,'Usenet',0,null,null,null,0,1,null,null,null,null,null,null,null,0,null,null,null,null,null
		-- The next one is not safe, but this procedure is only used for testing
		select @UserID=max(UserID) from [{databaseOwner}].[{objectQualifier}User]
	end

	select UserID=@UserID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}user_recoverpassword](@BoardID int,@UserName nvarchar(50),@Email nvarchar(50)) as
begin
	declare @UserID int
	select @UserID = UserID from [{databaseOwner}].[{objectQualifier}User] where BoardID = @BoardID and Name = @UserName and Email = @Email
	if @UserID is null begin
		select UserID = convert(int,null)
		return
	end else
	begin
		select UserID = @UserID
	end
end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_removepoints] (@UserID int,@Points int) AS
BEGIN
	UPDATE [{databaseOwner}].[{objectQualifier}User] SET Points = Points - @Points WHERE UserID = @UserID
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_removepointsbytopicid] (@TopicID int,@Points int) AS
BEGIN
	declare @UserID int
	select @UserID = UserID from [{databaseOwner}].[{objectQualifier}Topic] where TopicID = @TopicID
	update [{databaseOwner}].[{objectQualifier}User] SET points = points - @Points WHERE userID = @UserID
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_resetpoints] AS
BEGIN
	UPDATE [{databaseOwner}].[{objectQualifier}User] SET Points = NumPosts * 3
END
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}user_save](
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
	@ProviderUserKey	nvarchar(64) = null)
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
		
		select @RankID = RankID from [{databaseOwner}].[{objectQualifier}Rank] where (Flags & 1)<>0 and BoardID=@BoardID

		insert into [{databaseOwner}].[{objectQualifier}User](BoardID,RankID,Name,Password,Email,Joined,LastVisit,NumPosts,TimeZone,Flags,PMNotification,ProviderUserKey) 
		values(@BoardID,@RankID,@UserName,'-',@Email,getdate(),getdate(),0,@TimeZone,@Flags,@PMNotification,@ProviderUserKey)		
	
		set @UserID = SCOPE_IDENTITY()

		insert into [{databaseOwner}].[{objectQualifier}UserGroup](UserID,GroupID) select @UserID,GroupID from [{databaseOwner}].[{objectQualifier}Group] where BoardID=@BoardID and (Flags & 4)<>0
	end
	else begin
		update [{databaseOwner}].[{objectQualifier}User] set
			TimeZone = @TimeZone,
			LanguageFile = @LanguageFile,
			ThemeFile = @ThemeFile,
			OverrideDefaultThemes = @OverrideDefaultTheme,
			PMNotification = @PMNotification
		where UserID = @UserID
		
		if @Email is not null
			update [{databaseOwner}].[{objectQualifier}User] set Email = @Email where UserID = @UserID
	end
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}user_saveavatar]
(
	@UserID int,
	@Avatar nvarchar(255) = NULL,
	@AvatarImage image = NULL,
	@AvatarImageType nvarchar(50) = NULL
)
AS
BEGIN
	IF @Avatar IS NOT NULL 
	BEGIN
		UPDATE
			[{databaseOwner}].[{objectQualifier}User]
		SET
			Avatar = @Avatar,
			AvatarImage = null,
			AvatarImageType = null
		WHERE
			UserID = @UserID
	END
	ELSE IF @AvatarImage IS NOT NULL 
	BEGIN
		UPDATE
			[{databaseOwner}].[{objectQualifier}User]
		SET
			AvatarImage = @AvatarImage,
			AvatarImageType = @AvatarImageType,
			Avatar = null
		WHERE
			UserID = @UserID
	END
END

GO

create procedure [{databaseOwner}].[{objectQualifier}user_savepassword](@UserID int,@Password nvarchar(32)) as
begin
	update [{databaseOwner}].[{objectQualifier}User] set Password = @Password where UserID = @UserID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}user_savesignature](@UserID int,@Signature ntext) as
begin
	update [{databaseOwner}].[{objectQualifier}User] set Signature = @Signature where UserID = @UserID
end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_setpoints] (@UserID int,@Points int) AS
BEGIN
	UPDATE [{databaseOwner}].[{objectQualifier}User] SET Points = @Points WHERE UserID = @UserID
END
GO

create procedure [{databaseOwner}].[{objectQualifier}user_setrole](@BoardID int,@ProviderUserKey nvarchar(64),@Role nvarchar(50)) as
begin
	declare @UserID int, @GroupID int
	
	select @UserID=UserID from [{databaseOwner}].[{objectQualifier}User] where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey

	if @Role is null
	begin
		delete from [{databaseOwner}].[{objectQualifier}UserGroup] where UserID=@UserID
	end else
	begin
		if not exists(select 1 from [{databaseOwner}].[{objectQualifier}Group] where BoardID=@BoardID and Name=@Role)
		begin
			insert into [{databaseOwner}].[{objectQualifier}Group](Name,BoardID,Flags)
			values(@Role,@BoardID,0);
			set @GroupID = SCOPE_IDENTITY()

			insert into [{databaseOwner}].[{objectQualifier}ForumAccess](GroupID,ForumID,AccessMaskID)
			select
				@GroupID,
				a.ForumID,
				min(a.AccessMaskID)
			from
				[{databaseOwner}].[{objectQualifier}ForumAccess] a
				join [{databaseOwner}].[{objectQualifier}Group] b on b.GroupID=a.GroupID
			where
				b.BoardID=@BoardID and
				(b.Flags & 4)=4
			group by
				a.ForumID
		end else
		begin
			select @GroupID = GroupID from [{databaseOwner}].[{objectQualifier}Group] where BoardID=@BoardID and Name=@Role
		end
		insert into [{databaseOwner}].[{objectQualifier}UserGroup](UserID,GroupID) values(@UserID,@GroupID)
	end
end
GO

create procedure [{databaseOwner}].[{objectQualifier}user_suspend](@UserID int,@Suspend datetime=null) as
begin
	update [{databaseOwner}].[{objectQualifier}User] set Suspended = @Suspend where UserID=@UserID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}user_upgrade](@UserID int) as
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
		[{databaseOwner}].[{objectQualifier}User] a
		inner join [{databaseOwner}].[{objectQualifier}Rank] b on b.RankID = a.RankID
	where
		a.UserID = @UserID
	
	-- If user isn't member of a ladder rank, exit
	if (@Flags & 2) = 0 return
	
	-- See if user got enough posts for next ladder group
	select top 1
		@RankID = RankID
	from
		[{databaseOwner}].[{objectQualifier}Rank]
	where
		(Flags & 2) = 2 and
		MinPosts <= @NumPosts and
		MinPosts > @MinPosts
	order by
		MinPosts
	if @@ROWCOUNT=1
		update [{databaseOwner}].[{objectQualifier}User] set RankID = @RankID where UserID = @UserID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}userforum_delete](@UserID int,@ForumID int) as
begin
	delete from [{databaseOwner}].[{objectQualifier}UserForum] where UserID=@UserID and ForumID=@ForumID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}userforum_list](@UserID int=null,@ForumID int=null) as 
begin
	select 
		a.*,
		b.AccessMaskID,
		b.Accepted,
		Access = c.Name
	from
		[{databaseOwner}].[{objectQualifier}User] a
		join [{databaseOwner}].[{objectQualifier}UserForum] b on b.UserID=a.UserID
		join [{databaseOwner}].[{objectQualifier}AccessMask] c on c.AccessMaskID=b.AccessMaskID
	where
		(@UserID is null or a.UserID=@UserID) and
		(@ForumID is null or b.ForumID=@ForumID)
	order by
		a.Name	
end
GO

create procedure [{databaseOwner}].[{objectQualifier}userforum_save](@UserID int,@ForumID int,@AccessMaskID int) as
begin
	if exists(select 1 from [{databaseOwner}].[{objectQualifier}UserForum] where UserID=@UserID and ForumID=@ForumID)
		update [{databaseOwner}].[{objectQualifier}UserForum] set AccessMaskID=@AccessMaskID where UserID=@UserID and ForumID=@ForumID
	else
		insert into [{databaseOwner}].[{objectQualifier}UserForum](UserID,ForumID,AccessMaskID,Invited,Accepted) values(@UserID,@ForumID,@AccessMaskID,getdate(),1)
end
GO

create procedure [{databaseOwner}].[{objectQualifier}usergroup_list](@UserID int) as begin
	select 
		b.GroupID,
		b.Name
	from
		[{databaseOwner}].[{objectQualifier}UserGroup] a
		join [{databaseOwner}].[{objectQualifier}Group] b on b.GroupID=a.GroupID
	where
		a.UserID = @UserID
	order by
		b.Name
end
GO

create procedure [{databaseOwner}].[{objectQualifier}usergroup_save](@UserID int,@GroupID int,@Member bit) as
begin
	if @Member=0
		delete from [{databaseOwner}].[{objectQualifier}UserGroup] where UserID=@UserID and GroupID=@GroupID
	else
		insert into [{databaseOwner}].[{objectQualifier}UserGroup](UserID,GroupID)
		select @UserID,@GroupID
		where not exists(select 1 from [{databaseOwner}].[{objectQualifier}UserGroup] where UserID=@UserID and GroupID=@GroupID)
end
GO

create procedure [{databaseOwner}].[{objectQualifier}userpmessage_delete](@UserPMessageID int) as
begin
	delete from [{databaseOwner}].[{objectQualifier}UserPMessage] where UserPMessageID=@UserPMessageID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}userpmessage_list](@UserPMessageID int) as
begin
	select
		a.*,
		FromUser = b.Name,
		ToUserID = c.UserID,
		ToUser = c.Name,
		d.IsRead,
		d.UserPMessageID
	from
		[{databaseOwner}].[{objectQualifier}PMessage] a
		inner join [{databaseOwner}].[{objectQualifier}UserPMessage] d on d.PMessageID = a.PMessageID
		inner join [{databaseOwner}].[{objectQualifier}User] b on b.UserID = a.FromUserID
		inner join [{databaseOwner}].[{objectQualifier}User] c on c.UserID = d.UserID
	where
		d.UserPMessageID = @UserPMessageID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}watchforum_add](@UserID int,@ForumID int) as
begin
	insert into [{databaseOwner}].[{objectQualifier}WatchForum](ForumID,UserID,Created)
	select @ForumID, @UserID, getdate()
	where not exists(select 1 from [{databaseOwner}].[{objectQualifier}WatchForum] where ForumID=@ForumID and UserID=@UserID)
end
GO

create procedure [{databaseOwner}].[{objectQualifier}watchforum_check](@UserID int,@ForumID int) as
begin
	SELECT WatchForumID FROM [{databaseOwner}].[{objectQualifier}WatchForum] WHERE UserID = @UserID AND ForumID = @ForumID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}watchforum_delete](@WatchForumID int) as
begin
	delete from [{databaseOwner}].[{objectQualifier}WatchForum] where WatchForumID = @WatchForumID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}watchforum_list](@UserID int) as
begin
	select
		a.*,
		ForumName = b.Name,
		Messages = (select count(1) from [{databaseOwner}].[{objectQualifier}Topic] x join [{databaseOwner}].[{objectQualifier}Message] y on y.TopicID=x.TopicID where x.ForumID=a.ForumID),
		Topics = (select count(1) from [{databaseOwner}].[{objectQualifier}Topic] x where x.ForumID=a.ForumID and x.TopicMovedID is null),
		b.LastPosted,
		b.LastMessageID,
		LastTopicID = (select TopicID from [{databaseOwner}].[{objectQualifier}Message] x where x.MessageID=b.LastMessageID),
		b.LastUserID,
		LastUserName = IsNull(b.LastUserName,(select Name from [{databaseOwner}].[{objectQualifier}User] x where x.UserID=b.LastUserID))
	from
		[{databaseOwner}].[{objectQualifier}WatchForum] a
		inner join [{databaseOwner}].[{objectQualifier}Forum] b on b.ForumID = a.ForumID
	where
		a.UserID = @UserID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}watchtopic_add](@UserID int,@TopicID int) as
begin
	insert into [{databaseOwner}].[{objectQualifier}WatchTopic](TopicID,UserID,Created)
	select @TopicID, @UserID, getdate()
	where not exists(select 1 from [{databaseOwner}].[{objectQualifier}WatchTopic] where TopicID=@TopicID and UserID=@UserID)
end
GO

create procedure [{databaseOwner}].[{objectQualifier}watchtopic_check](@UserID int,@TopicID int) as
begin
	SELECT WatchTopicID FROM [{databaseOwner}].[{objectQualifier}WatchTopic] WHERE UserID = @UserID AND TopicID = @TopicID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}watchtopic_delete](@WatchTopicID int) as
begin
	delete from [{databaseOwner}].[{objectQualifier}WatchTopic] where WatchTopicID = @WatchTopicID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}watchtopic_list](@UserID int) as
begin
	select
		a.*,
		TopicName = b.Topic,
		Replies = (select count(1) from [{databaseOwner}].[{objectQualifier}Message] x where x.TopicID=b.TopicID),
		b.Views,
		b.LastPosted,
		b.LastMessageID,
		b.LastUserID,
		LastUserName = IsNull(b.LastUserName,(select Name from [{databaseOwner}].[{objectQualifier}User] x where x.UserID=b.LastUserID))
	from
		[{databaseOwner}].[{objectQualifier}WatchTopic] a
		inner join [{databaseOwner}].[{objectQualifier}Topic] b on b.TopicID = a.TopicID
	where
		a.UserID = @UserID
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}message_reply_list](@MessageID int) as
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
		[{databaseOwner}].[{objectQualifier}Message] a
		inner join [{databaseOwner}].[{objectQualifier}User] b on b.UserID = a.UserID
		inner join [{databaseOwner}].[{objectQualifier}Topic] c on c.TopicID = a.TopicID
	where
		(a.Flags & 16)=16 and
		a.ReplyTo = @MessageID



end
GO


CREATE procedure [{databaseOwner}].[{objectQualifier}message_deleteundelete](@MessageID int, @isModeratorChanged bit, @DeleteReason nvarchar(100), @isDeleteAction int) as
begin
	declare @TopicID		int
	declare @ForumID		int
	declare @MessageCount	int
	declare @LastMessageID	int
	declare @UserID			int

	-- Find TopicID and ForumID
	select @TopicID=b.TopicID,@ForumID=b.ForumID,@UserID = a.UserID 
	from 
		[{databaseOwner}].[{objectQualifier}Message] a
		inner join [{databaseOwner}].[{objectQualifier}Topic] b on b.TopicID=a.TopicID
	where 
		a.MessageID=@MessageID

	-- Update LastMessageID in Topic and Forum
	update [{databaseOwner}].[{objectQualifier}Topic] set
		LastPosted = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	update [{databaseOwner}].[{objectQualifier}Forum] set
		LastPosted = null,
		LastTopicID = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	-- "Delete" message
    update [{databaseOwner}].[{objectQualifier}Message]
     set IsModeratorChanged = @isModeratorChanged, DeleteReason = @DeleteReason, Flags = Flags ^ 8
     where MessageID = @MessageID and ((Flags & 8) <> @isDeleteAction*8)
    
    -- update num posts for user now that the delete/undelete status has been toggled...
    UPDATE [{databaseOwner}].[{objectQualifier}User] SET NumPosts = (SELECT count(MessageID) FROM [{databaseOwner}].[{objectQualifier}Message] WHERE UserID = @UserID AND IsDeleted = 0 AND IsApproved = 1) WHERE UserID = @UserID

	-- Delete topic if there are no more messages
	select @MessageCount = count(1) from [{databaseOwner}].[{objectQualifier}Message] where TopicID = @TopicID and (Flags & 8)=0
	if @MessageCount=0 exec [{databaseOwner}].[{objectQualifier}topic_delete] @TopicID
	-- update lastpost
	exec [{databaseOwner}].[{objectQualifier}topic_updatelastpost] @ForumID,@TopicID
	exec [{databaseOwner}].[{objectQualifier}forum_updatestats] @ForumID
	-- update topic numposts
	update [{databaseOwner}].[{objectQualifier}Topic] set
		NumPosts = (select count(1) from [{databaseOwner}].[{objectQualifier}Message] x where x.TopicID=[{databaseOwner}].[{objectQualifier}Topic].TopicID and x.IsApproved = 1 and x.IsDeleted = 0 )
	where TopicID = @TopicID
end
GO

create procedure [{databaseOwner}].[{objectQualifier}topic_create_by_message] (
	@MessageID int,
	@ForumID	int,
	@Subject	nvarchar(100)
) as
begin


declare		@UserID		int
declare		@Posted		datetime

set @UserID = (select UserID from [{databaseOwner}].[{objectQualifier}message] where MessageID =  @MessageID)
set  @Posted  = (select  posted from [{databaseOwner}].[{objectQualifier}message] where MessageID =  @MessageID)


	declare @TopicID int
	--declare @MessageID int

	if @Posted is null set @Posted = getdate()

	insert into [{databaseOwner}].[{objectQualifier}Topic](ForumID,Topic,UserID,Posted,Views,Priority,PollID,UserName,NumPosts)
	values(@ForumID,@Subject,@UserID,@Posted,0,0,null,null,0)

	set @TopicID = @@IDENTITY
--	exec [{databaseOwner}].[{objectQualifier}message_save] @TopicID,@UserID,@Message,@UserName,@IP,@Posted,null,@Flags,@MessageID output
	select TopicID = @TopicID, MessageID = @MessageID
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_move] (@MessageID int, @MoveToTopic int) AS
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
--	select @OldTopicID=b.TopicID,@ForumID=b.ForumID from [{databaseOwner}].[{objectQualifier}Message] a,{objectQualifier}Topic b where a.MessageID=@MessageID and b.TopicID=a.TopicID

SET 	@NewForumID = (SELECT     ForumID
				FROM         [{databaseOwner}].[{objectQualifier}Topic]
				WHERE     (TopicId = @MoveToTopic))


SET 	@OldTopicID = 	(SELECT     TopicID
				FROM         [{databaseOwner}].[{objectQualifier}Message]
				WHERE     (MessageID = @MessageID))

SET 	@OldForumID = (SELECT     ForumID
				FROM         [{databaseOwner}].[{objectQualifier}Topic]
				WHERE     (TopicId = @OldTopicID))

SET	@ReplyToID = (SELECT     MessageID
			FROM         [{databaseOwner}].[{objectQualifier}Message]
			WHERE     ([Position] = 0) AND (TopicID = @MoveToTopic))

SET	@Position = 	(SELECT     MAX([Position]) + 1 AS Expr1
			FROM         [{databaseOwner}].[{objectQualifier}Message]
			WHERE     (TopicID = @MoveToTopic) and posted < (select posted from [{databaseOwner}].[{objectQualifier}Message] where MessageID = @MessageID ) )

if @Position is null  set @Position = 0

update [{databaseOwner}].[{objectQualifier}Message] set
		Position = Position+1
	 WHERE     (TopicID = @MoveToTopic) and posted > (select posted from [{databaseOwner}].[{objectQualifier}Message] where MessageID = @MessageID)

update [{databaseOwner}].[{objectQualifier}Message] set
		Position = Position-1
	 WHERE     (TopicID = @OldTopicID) and posted > (select posted from [{databaseOwner}].[{objectQualifier}Message] where MessageID = @MessageID)

	-- Update LastMessageID in Topic and Forum
	update [{databaseOwner}].[{objectQualifier}Topic] set
		LastPosted = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	update [{databaseOwner}].[{objectQualifier}Forum] set
		LastPosted = null,
		LastTopicID = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID


UPDATE [{databaseOwner}].[{objectQualifier}Message] SET
 	TopicID = @MoveToTopic,
	ReplyTo = @ReplyToID,
	[Position] = @Position
WHERE  MessageID = @MessageID

	-- Delete topic if there are no more messages
	select @MessageCount = count(1) from [{databaseOwner}].[{objectQualifier}Message] where TopicID = @OldTopicID and (Flags & 8)=0
	if @MessageCount=0 exec [{databaseOwner}].[{objectQualifier}topic_delete] @OldTopicID

	-- update lastpost
	exec [{databaseOwner}].[{objectQualifier}topic_updatelastpost] @OldForumID,@OldTopicID
	exec [{databaseOwner}].[{objectQualifier}topic_updatelastpost] @NewForumID,@MoveToTopic

	-- update topic numposts
	update [{databaseOwner}].[{objectQualifier}Topic] set
		NumPosts = (select count(1) from [{databaseOwner}].[{objectQualifier}Message] x where x.TopicID=[{databaseOwner}].[{objectQualifier}Topic].TopicID and x.IsApproved = 1 and x.IsDeleted = 0)
	where TopicID = @OldTopicID
	update [{databaseOwner}].[{objectQualifier}Topic] set
		NumPosts = (select count(1) from [{databaseOwner}].[{objectQualifier}Message] x where x.TopicID=[{databaseOwner}].[{objectQualifier}Topic].TopicID and x.IsApproved = 1 and x.IsDeleted = 0)
	where TopicID = @MoveToTopic

	exec [{databaseOwner}].[{objectQualifier}forum_updatelastpost] @NewForumID
	exec [{databaseOwner}].[{objectQualifier}forum_updatestats] @NewForumID
	exec [{databaseOwner}].[{objectQualifier}forum_updatelastpost] @OldForumID
	exec [{databaseOwner}].[{objectQualifier}forum_updatestats] @OldForumID

END
GO

create proc [{databaseOwner}].[{objectQualifier}forum_resync]
	@BoardID int,
	@ForumID int = null
AS
begin
	if (@ForumID is null) begin
		declare curForums cursor for
			select 
				a.ForumID
			from
				[{databaseOwner}].[{objectQualifier}Forum] a
				JOIN [{databaseOwner}].[{objectQualifier}Category] b on a.CategoryID=b.CategoryID
				JOIN [{databaseOwner}].[{objectQualifier}Board] c on b.BoardID = c.BoardID  
			where
				c.BoardID=@BoardID

		open curForums
		
		-- cycle through forums
		fetch next from curForums into @ForumID
		while @@FETCH_STATUS = 0
		begin
			--update statistics
			exec [{databaseOwner}].[{objectQualifier}forum_updatestats] @ForumID
			--update last post
			exec [{databaseOwner}].[{objectQualifier}forum_updatelastpost] @ForumID

			fetch next from curForums into @ForumID
		end
		close curForums
		deallocate curForums
	end
	else begin
		--update statistics
		exec [{databaseOwner}].[{objectQualifier}forum_updatestats] @ForumID
		--update last post
		exec [{databaseOwner}].[{objectQualifier}forum_updatelastpost] @ForumID
	end
end
GO

create proc [{databaseOwner}].[{objectQualifier}board_resync]
	@BoardID int = null
as
begin
	if (@BoardID is null) begin
		declare curBoards cursor for
			select BoardID from	[{databaseOwner}].[{objectQualifier}Board]

		open curBoards
		
		-- cycle through forums
		fetch next from curBoards into @BoardID
		while @@FETCH_STATUS = 0
		begin
			--resync board forums
			exec [{databaseOwner}].[{objectQualifier}forum_resync] @BoardID

			fetch next from curBoards into @BoardID
		end
		close curBoards
		deallocate curBoards
	end
	else begin
		--resync board forums
		exec [{databaseOwner}].[{objectQualifier}forum_resync] @BoardID
	end
end
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}category_simplelist](
                @StartID INT  = 0,
                @Limit   INT  = 500)
AS
    BEGIN
        SET ROWCOUNT  @Limit
        SELECT   c.[CategoryID],
                 c.[Name]
        FROM     [{databaseOwner}].[{objectQualifier}Category] c
        WHERE    c.[CategoryID] >= @StartID
        AND c.[CategoryID] < (@StartID + @Limit)
        ORDER BY c.[CategoryID]
        SET ROWCOUNT  0
    END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}forum_simplelist](
                @StartID INT  = 0,
                @Limit   INT  = 500)
AS
    BEGIN
        SET ROWCOUNT  @Limit
        SELECT   f.[ForumID],
                 f.[Name]
        FROM     [{databaseOwner}].[{objectQualifier}Forum] f
        WHERE    f.[ForumID] >= @StartID
        AND f.[ForumID] < (@StartID + @Limit)
        ORDER BY f.[ForumID]
        SET ROWCOUNT  0
    END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}message_simplelist](
                @StartID INT  = 0,
                @Limit   INT  = 1000)
AS
    BEGIN
        SET ROWCOUNT  @Limit
        SELECT   m.[MessageID],
                 m.[TopicID]
        FROM     [{databaseOwner}].[{objectQualifier}Message] m
        WHERE    m.[MessageID] >= @StartID
        AND m.[MessageID] < (@StartID + @Limit)
        AND m.[TopicID] IS NOT NULL
        ORDER BY m.[MessageID]
        SET ROWCOUNT  0
    END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}topic_simplelist](
                @StartID INT  = 0,
                @Limit   INT  = 500)
AS
    BEGIN
        SET ROWCOUNT  @Limit
        SELECT   t.[TopicID],
                 t.[Topic]
        FROM     [{databaseOwner}].[{objectQualifier}Topic] t
        WHERE    t.[TopicID] >= @StartID
        AND t.[TopicID] < (@StartID + @Limit)
        ORDER BY t.[TopicID]
        SET ROWCOUNT  0
    END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_simplelist](
                @StartID INT  = 0,
                @Limit   INT  = 500)
AS
    BEGIN
        SET ROWCOUNT  @Limit
        SELECT   a.[UserID],
                 a.[Name]
        FROM     [{databaseOwner}].[{objectQualifier}User] a
        WHERE    a.[UserID] >= @StartID
        AND a.[UserID] < (@StartID + @Limit)
        ORDER BY a.[UserID]
        SET ROWCOUNT  0
    END
GO

-- BBCode

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}bbcode_delete]
(
	@BBCodeID int = NULL
)
AS
BEGIN
	IF @BBCodeID IS NOT NULL
		DELETE FROM [{objectQualifier}BBCode] WHERE BBCodeID = @BBCodeID
	ELSE
		DELETE FROM [{objectQualifier}BBCode]
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}bbcode_list]
(
	@BoardID int,
	@BBCodeID int = null
)
AS
BEGIN
	IF @BBCodeID IS NULL
		SELECT * FROM [{objectQualifier}BBCode] WHERE BoardID = @BoardID ORDER BY ExecOrder, [Name] DESC
	ELSE
		SELECT * FROM [{objectQualifier}BBCode] WHERE BBCodeID = @BBCodeID ORDER BY ExecOrder
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}bbcode_save]
(
	@BBCodeID int = null,
	@BoardID int,
	@Name nvarchar(255),
	@Description nvarchar(4000) = null,
	@OnClickJS nvarchar(1000) = null,
	@DisplayJS ntext = null,
	@EditJS ntext = null,
	@DisplayCSS ntext = null,
	@SearchRegEx ntext,
	@ReplaceRegEx ntext,
	@Variables nvarchar(1000) = null,
	@UseModule bit = null,
	@ModuleClass nvarchar(255) = null,	
	@ExecOrder int = 1
)
AS
BEGIN
	IF @BBCodeID IS NOT NULL BEGIN
		UPDATE
			[{objectQualifier}BBCode]
		SET
			[Name] = @Name,
			[Description] = @Description,
			[OnClickJS] = @OnClickJS,
			[DisplayJS] = @DisplayJS,
			[EditJS] = @EditJS,
			[DisplayCSS] = @DisplayCSS,
			[SearchRegEx] = @SearchRegEx,
			[ReplaceRegEx] = @ReplaceRegEx,
			[Variables] = @Variables,
			[UseModule] = @UseModule,
			[ModuleClass] = @ModuleClass,			
			[ExecOrder] = @ExecOrder
		WHERE
			BBCodeID = @BBCodeID
	END
	ELSE BEGIN
		IF NOT EXISTS(SELECT 1 FROM [{objectQualifier}BBCode] WHERE BoardID = @BoardID AND [Name] = @Name)
			INSERT INTO
				[{objectQualifier}BBCode] ([BoardID],[Name],[Description],[OnClickJS],[DisplayJS],[EditJS],[DisplayCSS],[SearchRegEx],[ReplaceRegEx],[Variables],[UseModule],[ModuleClass],[ExecOrder])
			VALUES (@BoardID,@Name,@Description,@OnClickJS,@DisplayJS,@EditJS,@DisplayCSS,@SearchRegEx,@ReplaceRegEx,@Variables,@UseModule,@ModuleClass,@ExecOrder)
	END
END
GO

-- polls

CREATE procedure [{databaseOwner}].[{objectQualifier}choice_add](
	@PollID		int,
	@Choice		nvarchar(50)
) as
begin

	insert into [{databaseOwner}].[{objectQualifier}Choice]
		(PollID, Choice, Votes)
		values
		(@PollID, @Choice, 0)
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}topic_poll_update](
	@TopicID	int=null,
	@MessageID	int=null,
	@PollID		int=null
) as
begin
	if not (@TopicID is null) begin
		update [{databaseOwner}].[{objectQualifier}Topic] 
			set PollID = @PollID 
			where TopicID = @TopicID
	end
	else if not (@MessageID is null) begin
		update [{databaseOwner}].[{objectQualifier}Topic] 
			set PollID = @PollID 
			where TopicID = (select TopicID from [{databaseOwner}].[{objectQualifier}Message] where MessageID = @MessageID)
	end
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}choice_update](
	@ChoiceID	int,
	@Choice		nvarchar(50)
) as
begin

	update [{databaseOwner}].[{objectQualifier}Choice]
		set Choice = @Choice
		where ChoiceID = @ChoiceID
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}choice_delete](
	@ChoiceID	int
) as
begin
	delete from [{databaseOwner}].[{objectQualifier}Choice]
		where ChoiceID = @ChoiceID
end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}poll_update](
	@PollID		int,
	@Question	nvarchar(50),
	@Closes 	datetime = null
) as
begin
	update [{databaseOwner}].[{objectQualifier}Poll]
		set Question	=	@Question,
			Closes		=	@Closes
		where PollID = @PollID

end
GO

CREATE procedure [{databaseOwner}].[{objectQualifier}poll_remove](
	@PollID int
) as
begin
	-- delete vote records first
	delete from [{databaseOwner}].[{objectQualifier}PollVote] where PollID = @PollID
	-- delete choices first
	delete from [{databaseOwner}].[{objectQualifier}Choice] where PollID = @PollID
	-- delete it from topic itself
	update [{databaseOwner}].[{objectQualifier}Topic] set PollID = null where PollID = @PollID

	-- delete poll
	delete from [{databaseOwner}].[{objectQualifier}Poll] where PollID = @PollID
end
GO


-- medals

create proc [{databaseOwner}].[{objectQualifier}group_medal_delete]
	@GroupID int,
	@MedalID int
as begin


	delete from [{databaseOwner}].[{objectQualifier}GroupMedal] where [GroupID]=@GroupID and [MedalID]=@MedalID

end
GO

CREATE proc [{databaseOwner}].[{objectQualifier}group_medal_list]
	@GroupID int = null,
	@MedalID int = null
as begin

	select 
		a.[MedalID],
		a.[Name],
		a.[MedalURL],
		a.[RibbonURL],
		a.[SmallMedalURL],
		a.[SmallRibbonURL],
		a.[SmallMedalWidth],
		a.[SmallMedalHeight],
		a.[SmallRibbonWidth],
		a.[SmallRibbonHeight],
		b.[SortOrder],
		a.[Flags],
		c.[Name] as [GroupName],
		b.[GroupID],
		isnull(b.[Message],a.[Message]) as [Message],
		b.[Message] as [MessageEx],
		b.[Hide],
		b.[OnlyRibbon],
		b.[SortOrder] as CurrentSortOrder
	from
		[{databaseOwner}].[{objectQualifier}Medal] a
		inner join [{databaseOwner}].[{objectQualifier}GroupMedal] b on b.[MedalID] = a.[MedalID]
		inner join [{databaseOwner}].[{objectQualifier}Group] c on  c.[GroupID] = b.[GroupID]
	where
		(@GroupID is null or b.[GroupID] = @GroupID) and
		(@MedalID is null or b.[MedalID] = @MedalID)		
	order by
		c.[Name] ASC,
		b.[SortOrder] ASC

end
GO

create proc [{databaseOwner}].[{objectQualifier}group_medal_save]
   @GroupID int,
   @MedalID int,
   @Message nvarchar(100) = NULL,
   @Hide bit,
   @OnlyRibbon bit,
   @SortOrder tinyint
as begin

	if exists(select 1 from [{databaseOwner}].[{objectQualifier}GroupMedal] where [GroupID]=@GroupID and [MedalID]=@MedalID) begin
		update [{databaseOwner}].[{objectQualifier}GroupMedal]
		set
			[Message] = @Message,
			[Hide] = @Hide,
			[OnlyRibbon] = @OnlyRibbon,
			[SortOrder] = @SortOrder
		where 
			[GroupID]=@GroupID and 
			[MedalID]=@MedalID
	end
	else begin

		insert into [{databaseOwner}].[{objectQualifier}GroupMedal]
			([GroupID],[MedalID],[Message],[Hide],[OnlyRibbon],[SortOrder])
		values
			(@GroupID,@MedalID,@Message,@Hide,@OnlyRibbon,@SortOrder)
	end

end
GO

CREATE proc [{databaseOwner}].[{objectQualifier}medal_delete]
	@BoardID	int = null,
	@MedalID	int = null,
	@Category	nvarchar(50) = null
as begin

	if not @MedalID is null begin
		delete from [{databaseOwner}].[{objectQualifier}GroupMedal] where [MedalID] = @MedalID
		delete from [{databaseOwner}].[{objectQualifier}UserMedal] where [MedalID] = @MedalID

		delete from [{databaseOwner}].[{objectQualifier}Medal] where [MedalID]=@MedalID
	end
	else if not @Category is null and not @BoardID is null begin
		delete from [{databaseOwner}].[{objectQualifier}GroupMedal] 
			where [MedalID] in (SELECT [MedalID] FROM [{databaseOwner}].[{objectQualifier}Medal] where [Category]=@Category and [BoardID]=@BoardID)

		delete from [{databaseOwner}].[{objectQualifier}UserMedal] 
			where [MedalID] in (SELECT [MedalID] FROM [{databaseOwner}].[{objectQualifier}Medal] where [Category]=@Category and [BoardID]=@BoardID)

		delete from [{databaseOwner}].[{objectQualifier}Medal] where [Category]=@Category
	end
	else if not @BoardID is null begin
		delete from [{databaseOwner}].[{objectQualifier}GroupMedal] 
			where [MedalID] in (SELECT [MedalID] FROM [{databaseOwner}].[{objectQualifier}Medal] where [BoardID]=@BoardID)

		delete from [{databaseOwner}].[{objectQualifier}UserMedal] 
			where [MedalID] in (SELECT [MedalID] FROM [{databaseOwner}].[{objectQualifier}Medal] where [BoardID]=@BoardID)

		delete from [{databaseOwner}].[{objectQualifier}Medal] where [BoardID]=@BoardID
	end

end
GO

CREATE proc [{databaseOwner}].[{objectQualifier}medal_list]
	@BoardID	int = null,
	@MedalID	int = null,
	@Category	nvarchar(50) = null
as begin

	if not @MedalID is null begin
		select 
			* 
		from 
			[{databaseOwner}].[{objectQualifier}Medal] 
		where 
			[MedalID]=@MedalID 
		order by 
			[Category] asc, 
			[SortOrder] asc
	end
	else if not @Category is null and not @BoardID is null begin
		select 
			* 
		from 
			[{databaseOwner}].[{objectQualifier}Medal] 
		where 
			[Category]=@Category and [BoardID]=@BoardID
		order by 
			[Category] asc, 
			[SortOrder] asc
	end
	else if not @BoardID is null begin
		select 
			* 
		from 
			[{databaseOwner}].[{objectQualifier}Medal] 
		where 
			[BoardID]=@BoardID
		order by 
			[Category] asc, 
			[SortOrder] asc
	end

end
GO

CREATE proc [{databaseOwner}].[{objectQualifier}medal_listusers]
	@MedalID	int
as begin

	(select 
		a.UserID, a.Name
	from 
		[{databaseOwner}].[{objectQualifier}User] a
		inner join [{databaseOwner}].[{objectQualifier}UserMedal] b on a.[UserID] = b.[UserID]
	where
		b.[MedalID]=@MedalID) 
	
	union	

	(select 
		a.UserID, a.Name
	from 
		[{databaseOwner}].[{objectQualifier}User] a
		inner join [{databaseOwner}].[{objectQualifier}UserGroup] b on a.[UserID] = b.[UserID]
		inner join [{databaseOwner}].[{objectQualifier}GroupMedal] c on b.[GroupID] = c.[GroupID]
	where
		c.[MedalID]=@MedalID) 


end
GO

create proc [{databaseOwner}].[{objectQualifier}medal_resort]
	@BoardID int,@MedalID int,@Move int
as
begin
	declare @Position int
	declare @Category nvarchar(50)

	select 
		@Position=[SortOrder],
		@Category=[Category]
	from 
		[{databaseOwner}].[{objectQualifier}Medal] 
	where 
		[BoardID]=@BoardID and [MedalID]=@MedalID

	if (@Position is null) return

	if (@Move > 0) begin
		update 
			[{databaseOwner}].[{objectQualifier}Medal]
		set 
			[SortOrder]=[SortOrder]-1
		where 
			[BoardID]=@BoardID and 
			[Category]=@Category and
			[SortOrder] between @Position and (@Position + @Move) and
			[SortOrder] between 1 and 255
	end
	else if (@Move < 0) begin
		update 
			[{databaseOwner}].[{objectQualifier}Medal]
		set
			[SortOrder]=[SortOrder]+1
		where 
			BoardID=@BoardID and 
			[Category]=@Category and
			[SortOrder] between (@Position+@Move) and @Position and
			[SortOrder] between 0 and 254
	end

	SET @Position = @Position + @Move

	if (@Position>255) SET @Position = 255
	else if (@Position<0) SET @Position = 0

	update [{databaseOwner}].[{objectQualifier}Medal]
		set [SortOrder]=@Position
		where [BoardID]=@BoardID and 
			[MedalID]=@MedalID
end
GO

CREATE proc [{databaseOwner}].[{objectQualifier}medal_save]
	@BoardID int = NULL,
	@MedalID int = NULL,
	@Name nvarchar(100),
	@Description ntext,
	@Message nvarchar(100),
	@Category nvarchar(50) = NULL,
	@MedalURL nvarchar(250),
	@RibbonURL nvarchar(250) = NULL,
	@SmallMedalURL nvarchar(250),
	@SmallRibbonURL nvarchar(250) = NULL,
	@SmallMedalWidth smallint,
	@SmallMedalHeight smallint,
	@SmallRibbonWidth smallint = NULL,
	@SmallRibbonHeight smallint = NULL,
	@SortOrder tinyint = 255,
	@Flags int = 0
as begin

	if @MedalID is null begin
		insert into [{databaseOwner}].[{objectQualifier}Medal]
			([BoardID],[Name],[Description],[Message],[Category],
			[MedalURL],[RibbonURL],[SmallMedalURL],[SmallRibbonURL],
			[SmallMedalWidth],[SmallMedalHeight],[SmallRibbonWidth],[SmallRibbonHeight],
			[SortOrder],[Flags])
		values
			(@BoardID,@Name,@Description,@Message,@Category,
			@MedalURL,@RibbonURL,@SmallMedalURL,@SmallRibbonURL,
			@SmallMedalWidth,@SmallMedalHeight,@SmallRibbonWidth,@SmallRibbonHeight,
			@SortOrder,@Flags)

		select @@rowcount
	end
	else begin
		update [{databaseOwner}].[{objectQualifier}Medal]
			set [BoardID] = BoardID,
				[Name] = @Name,
				[Description] = @Description,
				[Message] = @Message,
				[Category] = @Category,
				[MedalURL] = @MedalURL,
				[RibbonURL] = @RibbonURL,
				[SmallMedalURL] = @SmallMedalURL,
				[SmallRibbonURL] = @SmallRibbonURL,
				[SmallMedalWidth] = @SmallMedalWidth,
				[SmallMedalHeight] = @SmallMedalHeight,
				[SmallRibbonWidth] = @SmallRibbonWidth,
				[SmallRibbonHeight] = @SmallRibbonHeight,
				[SortOrder] = @SortOrder,
				[Flags] = @Flags
		where [MedalID] = @MedalID

		select @@rowcount
	end

end
GO

create proc [{databaseOwner}].[{objectQualifier}user_listmedals]
	@UserID	int
as begin

	(select
		a.[MedalID],
		a.[Name],
		isnull(b.[Message], a.[Message]) as [Message],
		a.[MedalURL],
		a.[RibbonURL],
		a.[SmallMedalURL],
		isnull(a.[SmallRibbonURL], a.[SmallMedalURL]) as [SmallRibbonURL],
		a.[SmallMedalWidth],
		a.[SmallMedalHeight],
		isnull(a.[SmallRibbonWidth], a.[SmallMedalWidth]) as [SmallRibbonWidth],
		isnull(a.[SmallRibbonHeight], a.[SmallMedalHeight]) as [SmallRibbonHeight],
		[{databaseOwner}].[{objectQualifier}medal_getsortorder](b.[SortOrder],a.[SortOrder],a.[Flags]) as [SortOrder],
		[{databaseOwner}].[{objectQualifier}medal_gethide](b.[Hide],a.[Flags]) as [Hide],
		[{databaseOwner}].[{objectQualifier}medal_getribbonsetting](a.[SmallRibbonURL],a.[Flags],b.[OnlyRibbon]) as [OnlyRibbon],
		a.[Flags],
		b.[DateAwarded]
	from
		[{databaseOwner}].[{objectQualifier}Medal] a
		inner join [{databaseOwner}].[{objectQualifier}UserMedal] b on a.[MedalID] = b.[MedalID]
	where
		b.[UserID] = @UserID)

	union

	(select
		a.[MedalID],
		a.[Name],
		isnull(b.[Message], a.[Message]) as [Message],
		a.[MedalURL],
		a.[RibbonURL],
		a.[SmallMedalURL],
		isnull(a.[SmallRibbonURL], a.[SmallMedalURL]) as [SmallRibbonURL],
		a.[SmallMedalWidth],
		a.[SmallMedalHeight],
		isnull(a.[SmallRibbonWidth], a.[SmallMedalWidth]) as [SmallRibbonWidth],
		isnull(a.[SmallRibbonHeight], a.[SmallMedalHeight]) as [SmallRibbonHeight],
		[{databaseOwner}].[{objectQualifier}medal_getsortorder](b.[SortOrder],a.[SortOrder],a.[Flags]) as [SortOrder],
		[{databaseOwner}].[{objectQualifier}medal_gethide](b.[Hide],a.[Flags]) as [Hide],
		[{databaseOwner}].[{objectQualifier}medal_getribbonsetting](a.[SmallRibbonURL],a.[Flags],b.[OnlyRibbon]) as [OnlyRibbon],
		a.[Flags],
		NULL as [DateAwarded]
	from
		[{databaseOwner}].[{objectQualifier}Medal] a
		inner join [{databaseOwner}].[{objectQualifier}GroupMedal] b on a.[MedalID] = b.[MedalID]
		inner join [{databaseOwner}].[{objectQualifier}UserGroup] c on b.[GroupID] = c.[GroupID]
	where
		c.[UserID] = @UserID)
	order by
		[OnlyRibbon] desc,
		[SortOrder] asc

end
GO

create proc [{databaseOwner}].[{objectQualifier}user_medal_delete]
	@UserID int,
	@MedalID int
as begin


	delete from [{databaseOwner}].[{objectQualifier}UserMedal] where [UserID]=@UserID and [MedalID]=@MedalID

end
GO

create proc [{databaseOwner}].[{objectQualifier}user_medal_list]
	@UserID int = null,
	@MedalID int = null
as begin

	select 
		a.[MedalID],
		a.[Name],
		a.[MedalURL],
		a.[RibbonURL],
		a.[SmallMedalURL],
		a.[SmallRibbonURL],
		a.[SmallMedalWidth],
		a.[SmallMedalHeight],
		a.[SmallRibbonWidth],
		a.[SmallRibbonHeight],
		b.[SortOrder],
		a.[Flags],
		c.[Name] as [UserName],
		b.[UserID],
		isnull(b.[Message],a.[Message]) as [Message],
		b.[Message] as [MessageEx],
		b.[Hide],
		b.[OnlyRibbon],
		b.[SortOrder] as [CurrentSortOrder],
		b.[DateAwarded]
	from
		[{databaseOwner}].[{objectQualifier}Medal] a
		inner join [{databaseOwner}].[{objectQualifier}UserMedal] b on b.[MedalID] = a.[MedalID]
		inner join [{databaseOwner}].[{objectQualifier}User] c on c.[UserID] = b.[UserID]
	where
		(@UserID is null or b.[UserID] = @UserID) and
		(@MedalID is null or b.[MedalID] = @MedalID)		
	order by
		c.[Name] ASC,
		b.[SortOrder] ASC

end
GO

create proc [{databaseOwner}].[{objectQualifier}user_medal_save]
	@UserID int,
	@MedalID int,
	@Message nvarchar(100) = NULL,
	@Hide bit,
	@OnlyRibbon bit,
	@SortOrder tinyint,
	@DateAwarded datetime = NULL
as begin

	if exists(select 1 from [{databaseOwner}].[{objectQualifier}UserMedal] where [UserID]=@UserID and [MedalID]=@MedalID) begin
		update [{databaseOwner}].[{objectQualifier}UserMedal]
		set
			[Message] = @Message,
			[Hide] = @Hide,
			[OnlyRibbon] = @OnlyRibbon,
			[SortOrder] = @SortOrder
		where 
			[UserID]=@UserID and 
			[MedalID]=@MedalID
	end
	else begin

		if (@DateAwarded is null) set @DateAwarded = getdate() 

		insert into [{databaseOwner}].[{objectQualifier}UserMedal]
			([UserID],[MedalID],[Message],[Hide],[OnlyRibbon],[SortOrder],[DateAwarded])
		values
			(@UserID,@MedalID,@Message,@Hide,@OnlyRibbon,@SortOrder,@DateAwarded)
	end

end
GO

/* User Ignore Procedures */

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_addignoreduser]
	@UserId int,
	@IgnoredUserId int
AS BEGIN
	IF NOT EXISTS (SELECT * FROM [{databaseOwner}].[{objectQualifier}IgnoreUser] WHERE UserID = @userId AND IgnoredUserID = @ignoredUserId)
	BEGIN
		INSERT INTO [{databaseOwner}].[{objectQualifier}IgnoreUser] (UserID, IgnoredUserID) VALUES (@UserId, @IgnoredUserId)
	END
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_removeignoreduser]
    @UserId int,
    @IgnoredUserId int
AS BEGIN

	DELETE FROM [{databaseOwner}].[{objectQualifier}IgnoreUser] WHERE UserID = @userId AND IgnoredUserID = @ignoredUserId
	
END
GO

CREATE PROCEDURE [{databaseOwner}].[{objectQualifier}user_isuserignored]
    @UserId int,
    @IgnoredUserId int
AS BEGIN

	IF EXISTS(SELECT * FROM [{databaseOwner}].[{objectQualifier}IgnoreUser] WHERE UserID = @userId AND IgnoredUserID = @ignoredUserId)
	BEGIN
		RETURN 1
	END
	ELSE
	BEGIN
		RETURN 0
	END
	
END	
GO
