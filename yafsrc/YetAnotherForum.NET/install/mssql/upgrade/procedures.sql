/*
  Drop All


  Remove Comments RegEx: \/\*(.*)\*\/
  Remove Extra Stuff: SET ANSI_NULLS ON\nGO\nSET QUOTED_IDENTIFIER ON\nGO\n\n\n

*/

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}shoutbox_getmessages]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [{databaseOwner}].[{objectQualifier}shoutbox_getmessages]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}shoutbox_savemessage]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [{databaseOwner}].[{objectQualifier}shoutbox_savemessage]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}shoutbox_clearmessages]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [{databaseOwner}].[{objectQualifier}shoutbox_clearmessages]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}exampleserverversion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}exampleserverversion]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}db_handle_computedcolumns]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}db_handle_computedcolumns]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}adminpageaccess_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}adminpageaccess_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}adminpageaccess_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}adminpageaccess_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}adminpageaccess_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}adminpageaccess_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}eventloggroupaccess_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}eventloggroupaccess_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}eventloggroupaccess_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}eventloggroupaccess_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}eventloggroupaccess_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}eventloggroupaccess_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_savestyle]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_savestyle]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_unanswered]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_unanswered]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_unread]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_unread]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_update_single_sign_on_status]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_update_single_sign_on_status]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_updatefacebookstatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_updatefacebookstatus]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_move]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_move]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}User_ListTodaysBirthdays]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}User_ListTodaysBirthdays]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}TopicStatus_Delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}TopicStatus_Delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}TopicStatus_Edit]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}TopicStatus_Edit]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}TopicStatus_List]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}TopicStatus_List]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}TopicStatus_Save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}TopicStatus_Save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topics_byuser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topics_byuser]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}readtopic_addorupdate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}readtopic_addorupdate]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}readtopic_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}readtopic_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}readtopic_lastread]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}readtopic_lastread]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}readforum_addorupdate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}readforum_addorupdate]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}readforum_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}readforum_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}readforum_lastread]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}readforum_lastread]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_lastread]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_lastread]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}recent_users]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}recent_users]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_thankfromcount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_thankfromcount]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_repliedtopic]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_repliedtopic]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_thankedmessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_thankedmessage]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}accessmask_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}accessmask_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}accessmask_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}accessmask_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}accessmask_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}accessmask_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}active_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}active_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}active_listforum]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}active_listforum]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}active_listtopic]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}active_listtopic]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}active_stats]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}active_stats]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}activeaccess_reset]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}activeaccess_reset]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}active_updatemaxstats]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}active_updatemaxstats]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}attachment_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}attachment_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}attachment_download]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}attachment_download]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}attachment_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}attachment_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}attachment_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}attachment_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bannedip_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bannedip_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bannedip_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bannedip_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bannedip_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bannedip_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bannedname_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bannedname_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bannedname_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bannedname_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bannedname_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bannedname_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bannedemail_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bannedemail_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bannedemail_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bannedemail_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bannedemail_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bannedemail_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_create]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_poststats]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_poststats]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_userstats]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_userstats]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_resync]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_resync]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_setguid]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_setguid]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}board_stats]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}board_stats]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}category_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}category_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}category_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}category_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}category_listread]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}category_listread]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}category_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}category_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}checkemail_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}checkemail_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}checkemail_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}checkemail_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}checkemail_update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}checkemail_update]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}choice_add]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}choice_add]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}choice_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}choice_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}choice_update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}choice_update]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}choice_vote]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}choice_vote]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}eventlog_create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}eventlog_create]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}eventlog_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}eventlog_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}eventlog_deletebyuser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}eventlog_deletebyuser]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}eventlog_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}eventlog_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}extension_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}extension_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}extension_edit]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}extension_edit]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}extension_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}extension_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}extension_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}extension_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_maxid]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_maxid]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listall]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listall]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listall_fromcat]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listall_fromcat]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listallmymoderated]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listallmymoderated]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listpath]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listpath]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listread]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listread]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listSubForums]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listSubForums]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_listtopics]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_listtopics]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_moderatelist]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_moderatelist]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_moderators]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_moderators]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}moderators_team_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}moderators_team_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_resync]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_resync]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_updatelastpost]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_updatelastpost]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forum_updatestats]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_updatestats]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forumaccess_group]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forumaccess_group]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forumaccess_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forumaccess_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}forumaccess_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forumaccess_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_eventlogaccesslist]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_eventlogaccesslist]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_medal_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_medal_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_medal_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_medal_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_medal_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_medal_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_member]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_member]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}group_rank_style]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}group_rank_style]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}mail_create]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}mail_create]
GO

IF  exists (select top 1 1 from sys.objects where object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}mail_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}mail_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}mail_createwatch]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}mail_createwatch]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}mail_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}mail_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}mail_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}mail_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}medal_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}medal_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}medal_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}medal_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}medal_listusers]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}medal_listusers]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}medal_resort]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}medal_resort]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}medal_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}medal_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_approve]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_approve]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_findunread]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_findunread]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_getReplies]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_getReplies]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_list_search]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_list_search]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_listreported]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_listreported]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_listreporters]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_listreporters]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_report]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_report]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_reportcopyover]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_reportcopyover]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_reportresolve]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_reportresolve]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_secdata]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_secdata]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_unapproved]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_unapproved]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_update]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpforum_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpforum_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpforum_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpforum_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpforum_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpforum_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpforum_update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpforum_update]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpserver_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpserver_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpserver_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpserver_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntpserver_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntpserver_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntptopic_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntptopic_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}nntptopic_savemessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}nntptopic_savemessage]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pageaccess]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pageaccess]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pageaccess_path]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pageaccess_path]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pageload]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pageload]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_info]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_info]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_markread]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_markread]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_prune]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_prune]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pmessage_archive]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pmessage_archive]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}poll_remove]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}poll_remove]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pollgroup_attach]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pollgroup_attach]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pollgroup_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pollgroup_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pollgroup_remove]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pollgroup_remove]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}poll_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}poll_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}poll_stats]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}poll_stats]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pollgroup_stats]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pollgroup_stats]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}poll_update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}poll_update]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pollvote_check]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pollvote_check]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}pollgroup_votecheck]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}pollgroup_votecheck]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}post_last10user]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}post_last10user]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}post_alluser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}post_alluser]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}post_alluser_simple]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}post_alluser_simple]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}post_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}post_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}post_list_reverse10]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}post_list_reverse10]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}rank_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}rank_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}rank_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}rank_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}rank_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}rank_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}registry_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}registry_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}registry_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}registry_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}replace_words_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}replace_words_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}replace_words_edit]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}replace_words_edit]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}replace_words_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}replace_words_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}replace_words_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}replace_words_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}spam_words_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}spam_words_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}spam_words_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}spam_words_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}spam_words_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}spam_words_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}smiley_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}smiley_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}smiley_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}smiley_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}smiley_listunique]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}smiley_listunique]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}smiley_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}smiley_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}smiley_resort]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}smiley_resort]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}system_initialize]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}system_initialize]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}system_updateversion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}system_updateversion]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_active]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_active]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_findduplicate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_findduplicate]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_findnext]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_findnext]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_findprev]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_findprev]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_info]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_info]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_announcements]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_announcements]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_latest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_latest]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_latest_in_category]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_latest_in_category]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}rss_topic_latest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}rss_topic_latest]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}rsstopic_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}rsstopic_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}announcements_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}announcements_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_listmessages]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_listmessages]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_deleteattachements]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_deleteattachements]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_lock]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_lock]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_move]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_move]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_poll_update]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_poll_update]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_prune]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_prune]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_updatelastpost]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_updatelastpost]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_updatetopic]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_updatetopic]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_accessmasks]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_accessmasks]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_activity_rank]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_activity_rank]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_addpoints]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_addpoints]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_adminsave]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_adminsave]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_approve]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_approve]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_approveall]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_approveall]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_aspnet]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_aspnet]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_migrate]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_migrate]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_avatarimage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_avatarimage]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_changepassword]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_changepassword]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_pmcount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_pmcount]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_deleteavatar]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_deleteavatar]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_deleteold]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_deleteold]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_emails]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_emails]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_find]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_find]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_getpoints]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_getpoints]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_getsignature]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_getsignature]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_guest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_guest]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}admin_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}admin_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}admin_pageaccesslist]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}admin_pageaccesslist]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_listmembers]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_listmembers]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_listmedals]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_listmedals]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_login]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_login]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_medal_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_medal_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_medal_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_medal_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_medal_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_medal_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_nntp]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_nntp]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_recoverpassword]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_recoverpassword]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_removepoints]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_removepoints]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_removepointsbytopicid]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_removepointsbytopicid]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_resetpoints]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_resetpoints]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_savenotification]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_savenotification]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_saveavatar]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_saveavatar]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_savepassword]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_savepassword]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_savesignature]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_savesignature]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_setnotdirty]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_setnotdirty]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_setpoints]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_setpoints]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_setrole]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_setrole]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_suspend]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_suspend]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_upgrade]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_upgrade]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}userforum_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}userforum_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}userforum_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}userforum_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}userforum_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}userforum_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}usergroup_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}usergroup_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}usergroup_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}usergroup_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}userpmessage_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}userpmessage_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}userpmessage_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}userpmessage_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchforum_add]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchforum_add]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchforum_check]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchforum_check]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchforum_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchforum_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchforum_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchforum_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchtopic_add]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchtopic_add]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchtopic_check]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchtopic_check]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchtopic_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchtopic_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}watchtopic_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}watchtopic_list]
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_reply_list]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_reply_list]
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_deleteundelete]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_deleteundelete]
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_create_by_message]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_create_by_message]
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_move]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_move]
GO

IF exists (select top 1 1 from sys.objects
           WHERE  object_id = Object_id(N'[{databaseOwner}].[{objectQualifier}category_simplelist]')
           AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}category_simplelist]
GO

IF EXISTS (select top 1 1 from sys.objects
           WHERE  object_id = Object_id(N'[{databaseOwner}].[{objectQualifier}forum_simplelist]')
           AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}forum_simplelist]
GO

IF EXISTS (select top 1 1 from sys.objects
           WHERE  object_id = Object_id(N'[{databaseOwner}].[{objectQualifier}message_simplelist]')
           AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_simplelist]
GO

IF EXISTS (select top 1 1 from sys.objects
           WHERE  object_id = Object_id(N'[{databaseOwner}].[{objectQualifier}topic_simplelist]')
           AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_simplelist]
GO

IF EXISTS (select top 1 1 from sys.objects
           WHERE  object_id = Object_id(N'[{databaseOwner}].[{objectQualifier}topic_similarlist]')
           AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_similarlist]
GO

IF EXISTS (select top 1 1 from sys.objects
           WHERE  object_id = Object_id(N'[{databaseOwner}].[{objectQualifier}user_simplelist]')
           AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_simplelist]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bbcode_delete]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bbcode_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bbcode_list]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bbcode_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}bbcode_save]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [{databaseOwner}].[{objectQualifier}bbcode_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_addignoreduser]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_addignoreduser]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_removeignoreduser]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_removeignoreduser]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_isuserignored]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_isuserignored]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_ignoredlist]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_ignoredlist]
GO

/* These stored procedures are for the Thanks Table. For safety, first check to see if they exist. If so, drop them. */
IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_addthanks]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_addthanks]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_getthanks]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_getthanks]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_getallthanks]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_getallthanks]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_removethanks]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_removethanks]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_thanksnumber]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_thanksnumber]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_getthanks_from]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_getthanks_from]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_getthanks_to]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_getthanks_to]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}active_list_user]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}active_list_user]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_viewallthanks]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_viewallthanks]
GO

/* End of Thanks table stored procedures */

/* Buddy feature stored procedures */
IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}buddy_addrequest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}buddy_addrequest]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}buddy_approverequest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}buddy_approverequest]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}buddy_denyrequest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}buddy_denyrequest]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}buddy_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}buddy_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}buddy_remove]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}buddy_remove]
GO
/* End of Buddy feature stored procedures */

/****** Object:  StoredProcedure [{databaseOwner}].[{objectQualifier}topic_favorite_add]    Script Date: 12/08/2009 18:13:19 ******/
IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_favorite_add]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_favorite_add]
GO

/****** Object:  StoredProcedure [{databaseOwner}].[{objectQualifier}topic_favorite_details]    Script Date: 12/08/2009 18:13:20 ******/
IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_favorite_details]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_favorite_details]
GO

/****** Object:  StoredProcedure [{databaseOwner}].[{objectQualifier}topic_favorite_list]    Script Date: 12/08/2009 18:13:20 ******/
IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_favorite_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_favorite_list]
GO

/****** Object:  StoredProcedure [{databaseOwner}].[{objectQualifier}topic_favorite_list]    Script Date: 12/08/2009 18:13:20 ******/
IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_favorite_count]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_favorite_count]
GO

/****** Object:  StoredProcedure [{databaseOwner}].[{objectQualifier}topic_favorite_remove]    Script Date: 12/08/2009 18:13:20 ******/
IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}topic_favorite_remove]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}topic_favorite_remove]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}album_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}album_save]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}album_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}album_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}album_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}album_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}album_gettitle]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}album_gettitle]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}album_getstats]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}album_getstats]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}album_image_save]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}album_image_save]
Go

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}album_image_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}album_image_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}album_images_by_user]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}album_images_by_user]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}album_image_delete]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}album_image_delete]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}album_image_download]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}album_image_download]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_getsignaturedata]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_getsignaturedata]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_getalbumsdata]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_getalbumsdata]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}messagehistory_list]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}messagehistory_list]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}user_lazydata]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}user_lazydata]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_gettextbyids]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_gettextbyids]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}message_GetTextByIds]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}message_GetTextByIds]
GO

IF  exists (select top 1 1 from sys.objects WHERE object_id = OBJECT_ID(N'[{databaseOwner}].[{objectQualifier}init_styles]') AND type in (N'P', N'PC'))
DROP PROCEDURE [{databaseOwner}].[{objectQualifier}init_styles]
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