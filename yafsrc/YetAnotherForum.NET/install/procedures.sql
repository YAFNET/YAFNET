/*
  YAF SQL Stored Procedures File Created 03/01/06
	

  Remove Comments RegEx: \/\*(.*)\*\/
  Remove Extra Stuff: SET ANSI_NULLS ON\nGO\nSET QUOTED_IDENTIFIER ON\nGO\n\n\n 
*/

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_accessmask_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_accessmask_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_accessmask_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_accessmask_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_accessmask_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_accessmask_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_active_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_active_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_active_listforum]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_active_listforum]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_active_listtopic]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_active_listtopic]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_active_stats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_active_stats]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_attachment_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_attachment_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_attachment_download]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_attachment_download]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_attachment_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_attachment_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_attachment_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_attachment_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_bannedip_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_bannedip_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_bannedip_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_bannedip_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_bannedip_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_bannedip_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_board_create]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_board_create]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_board_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_board_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_board_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_board_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_board_poststats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_board_poststats]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_board_resync]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_board_resync]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_board_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_board_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_board_stats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_board_stats]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_category_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_category_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_category_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_category_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_category_listread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_category_listread]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_category_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_category_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_checkemail_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_checkemail_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_checkemail_update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_checkemail_update]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_choice_vote]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_choice_vote]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_eventlog_create]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_eventlog_create]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_eventlog_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_eventlog_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_eventlog_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_eventlog_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forum_listall]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_listall]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forum_listall_fromcat]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_listall_fromcat]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forum_listallmymoderated]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_listallmymoderated]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forum_listpath]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_listpath]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forum_listread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_listread]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forum_listSubForums]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_listSubForums]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forum_listtopics]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_listtopics]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forum_moderatelist]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_moderatelist]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forum_moderators]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_moderators]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forum_resync]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_resync]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forum_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forum_updatelastpost]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_updatelastpost]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forum_updatestats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_updatestats]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forumaccess_group]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forumaccess_group]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forumaccess_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forumaccess_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_forumaccess_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forumaccess_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_group_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_group_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_group_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_group_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_group_member]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_group_member]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_group_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_group_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_mail_create]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_mail_create]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_mail_createwatch]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_mail_createwatch]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_mail_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_mail_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_mail_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_mail_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_message_approve]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_approve]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_message_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_message_findunread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_findunread]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_message_getReplies]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_getReplies]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_message_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_message_listreported]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_listreported]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_message_report]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_report]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_message_reportcopyover]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_reportcopyover]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_message_reportresolve]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_reportresolve]
GO




IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_message_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_message_unapproved]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_unapproved]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_message_update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_update]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_nntpforum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_nntpforum_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_nntpforum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_nntpforum_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_nntpforum_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_nntpforum_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_nntpforum_update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_nntpforum_update]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_nntpserver_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_nntpserver_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_nntpserver_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_nntpserver_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_nntpserver_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_nntpserver_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_nntptopic_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_nntptopic_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_nntptopic_savemessage]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_nntptopic_savemessage]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_pageload]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_pageload]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_pmessage_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_pmessage_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_pmessage_info]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_pmessage_info]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_pmessage_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_pmessage_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_pmessage_markread]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_pmessage_markread]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_pmessage_prune]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_pmessage_prune]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_pmessage_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_pmessage_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_pmessage_archive]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_pmessage_archive]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_poll_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_poll_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_poll_stats]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_poll_stats]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_pollvote_check]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_pollvote_check]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_post_last10user]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_post_last10user]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_post_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_post_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_post_list_reverse10]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_post_list_reverse10]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_rank_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_rank_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_rank_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_rank_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_rank_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_rank_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_registry_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_registry_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_registry_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_registry_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_replace_words_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_replace_words_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_replace_words_edit]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_replace_words_edit]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_replace_words_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_replace_words_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_replace_words_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_replace_words_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_smiley_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_smiley_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_smiley_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_smiley_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_smiley_listunique]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_smiley_listunique]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_smiley_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_smiley_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_smiley_resort]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_smiley_resort]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_system_initialize]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_system_initialize]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_system_updateversion]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_system_updateversion]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_topic_active]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_active]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_topic_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_topic_findnext]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_findnext]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_topic_findprev]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_findprev]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_topic_info]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_info]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_topic_announcements]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_announcements]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_topic_latest]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_latest]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_topic_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_topic_listmessages]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_listmessages]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_topic_lock]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_lock]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_topic_move]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_move]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_topic_prune]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_prune]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_topic_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_topic_updatelastpost]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_updatelastpost]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_accessmasks]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_accessmasks]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_activity_rank]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_activity_rank]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_addpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_addpoints]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_adminsave]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_adminsave]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_approve]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_approve]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_approveall]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_approveall]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_aspnet]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_aspnet]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_avatarimage]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_avatarimage]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_changepassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_changepassword]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_deleteavatar]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_deleteavatar]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_deleteold]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_deleteold]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_emails]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_emails]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_find]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_find]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_getpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_getpoints]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_getsignature]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_getsignature]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_guest]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_guest]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_login]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_login]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_nntp]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_nntp]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_recoverpassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_recoverpassword]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_removepoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_removepoints]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_removepointsbytopicid]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_removepointsbytopicid]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_resetpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_resetpoints]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_saveavatar]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_saveavatar]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_savepassword]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_savepassword]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_savesignature]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_savesignature]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_setpoints]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_setpoints]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_setrole]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_setrole]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_suspend]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_suspend]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_user_upgrade]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_upgrade]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_userforum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_userforum_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_userforum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_userforum_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_userforum_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_userforum_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_usergroup_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_usergroup_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_usergroup_save]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_usergroup_save]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_userpmessage_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_userpmessage_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_userpmessage_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_userpmessage_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_watchforum_add]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_watchforum_add]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_watchforum_check]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_watchforum_check]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_watchforum_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_watchforum_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_watchforum_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_watchforum_list]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_watchtopic_add]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_watchtopic_add]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_watchtopic_check]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_watchtopic_check]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_watchtopic_delete]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_watchtopic_delete]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[yaf_watchtopic_list]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_watchtopic_list]
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'yaf_message_reply_list') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_reply_list]
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = object_id(N'yaf_message_deleteundelete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_deleteundelete]
GO

IF EXISTS (SELECT 1 FROM sysobjects WHERE id = object_id(N'yaf_topic_create_by_message') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_create_by_message]
GO

IF EXISTS (SELECT 1 FROM sysobjects where id = object_id(N'yaf_message_move') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_move]
GO

IF EXISTS (SELECT *
           FROM   dbo.sysobjects
           WHERE  id = Object_id(N'[dbo].[yaf_category_simplelist]')
           AND Objectproperty(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_category_simplelist] 
GO

IF EXISTS (SELECT *
           FROM   dbo.sysobjects
           WHERE  id = Object_id(N'[dbo].[yaf_forum_simplelist]')
           AND Objectproperty(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_forum_simplelist] 
GO

IF EXISTS (SELECT *
           FROM   dbo.sysobjects
           WHERE  id = Object_id(N'[dbo].[yaf_message_simplelist]')
           AND Objectproperty(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_message_simplelist] 
GO

IF EXISTS (SELECT *
           FROM   dbo.sysobjects
           WHERE  id = Object_id(N'[dbo].[yaf_topic_simplelist]')
           AND Objectproperty(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_topic_simplelist] 
GO

IF EXISTS (SELECT *
           FROM   dbo.sysobjects
           WHERE  id = Object_id(N'[dbo].[yaf_user_simplelist]')
           AND Objectproperty(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[yaf_user_simplelist] 
GO


create procedure [dbo].[yaf_accessmask_delete](@AccessMaskID int) as
begin
	declare @flag int
	
	set @flag=1
	if exists(select 1 from yaf_ForumAccess where AccessMaskID=@AccessMaskID) or exists(select 1 from yaf_UserForum where AccessMaskID=@AccessMaskID)
		set @flag=0
	else
		delete from yaf_AccessMask where AccessMaskID=@AccessMaskID
	
	select @flag
end
GO

create procedure [dbo].[yaf_accessmask_list](@BoardID int,@AccessMaskID int=null) as
begin
	if @AccessMaskID is null
		select 
			a.* 
		from 
			yaf_AccessMask a 
		where
			a.BoardID = @BoardID
		order by 
			a.Name
	else
		select 
			a.* 
		from 
			yaf_AccessMask a 
		where
			a.BoardID = @BoardID and
			a.AccessMaskID = @AccessMaskID
		order by 
			a.Name
end
GO

create procedure [dbo].[yaf_accessmask_save](
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
		insert into yaf_AccessMask(Name,BoardID,Flags)
		values(@Name,@BoardID,@Flags)
	else
		update yaf_AccessMask set
			Name			= @Name,
			Flags			= @Flags
		where AccessMaskID=@AccessMaskID
end
GO

create procedure [dbo].[yaf_active_list](@BoardID int,@Guests bit=0) as
begin
	-- delete non-active
	delete from yaf_Active where DATEDIFF(minute,LastActive,getdate())>5
	-- select active
	if @Guests<>0
		select
			a.UserID,
			a.Name,
			c.IP,
			c.SessionID,
			c.ForumID,
			c.TopicID,
			ForumName = (select Name from yaf_Forum x where x.ForumID=c.ForumID),
			TopicName = (select Topic from yaf_Topic x where x.TopicID=c.TopicID),
			IsGuest = (select 1 from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and (y.Flags & 2)<>0),
			c.Login,
			c.LastActive,
			c.Location,
			Active = DATEDIFF(minute,c.Login,c.LastActive),
			c.Browser,
			c.Platform
		from
			yaf_User a,
			yaf_Active c
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
			ForumName = (select Name from yaf_Forum x where x.ForumID=c.ForumID),
			TopicName = (select Topic from yaf_Topic x where x.TopicID=c.TopicID),
			IsGuest = (select 1 from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and (y.Flags & 2)<>0),
			c.Login,
			c.LastActive,
			c.Location,
			Active = DATEDIFF(minute,c.Login,c.LastActive),
			c.Browser,
			c.Platform
		from
			yaf_User a,
			yaf_Active c
		where
			c.UserID = a.UserID and
			c.BoardID = @BoardID and
			not exists(select 1 from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and (y.Flags & 2)<>0)
		order by
			c.LastActive desc
end
GO

create procedure [dbo].[yaf_active_listforum](@ForumID int) as
begin
	select
		UserID		= a.UserID,
		UserName	= b.Name
	from
		yaf_Active a join yaf_User b on b.UserID=a.UserID
	where
		a.ForumID = @ForumID
	group by
		a.UserID,
		b.Name
	order by
		b.Name
end
GO

create procedure [dbo].[yaf_active_listtopic](@TopicID int) as
begin
	select
		UserID		= a.UserID,
		UserName	= b.Name
	from
		yaf_Active a with(nolock)
		join yaf_User b on b.UserID=a.UserID
	where
		a.TopicID = @TopicID
	group by
		a.UserID,
		b.Name
	order by
		b.Name
end
GO

create procedure [dbo].[yaf_active_stats](@BoardID int) as
begin
	select
		ActiveUsers = (select count(1) from yaf_Active where BoardID=@BoardID),
		ActiveMembers = (select count(1) from yaf_Active x where BoardID=@BoardID and exists(select 1 from yaf_UserGroup y,yaf_Group z where y.UserID=x.UserID and y.GroupID=z.GroupID and (z.Flags & 2)=0)),
		ActiveGuests = (select count(1) from yaf_Active x where BoardID=@BoardID and exists(select 1 from yaf_UserGroup y,yaf_Group z where y.UserID=x.UserID and y.GroupID=z.GroupID and (z.Flags & 2)<>0))
end
GO

create procedure [dbo].[yaf_attachment_delete](@AttachmentID int) as begin
	delete from yaf_Attachment where AttachmentID=@AttachmentID
end
GO

create procedure [dbo].[yaf_attachment_download](@AttachmentID int) as
begin
	update yaf_Attachment set Downloads=Downloads+1 where AttachmentID=@AttachmentID
end
GO

create procedure [dbo].[yaf_attachment_list](@MessageID int=null,@AttachmentID int=null,@BoardID int=null) as begin
	if @MessageID is not null
		select * from yaf_Attachment where MessageID=@MessageID
	else if @AttachmentID is not null
		select * from yaf_Attachment where AttachmentID=@AttachmentID
	else
		select 
			a.*,
			Posted		= b.Posted,
			ForumID		= d.ForumID,
			ForumName	= d.Name,
			TopicID		= c.TopicID,
			TopicName	= c.Topic
		from 
			yaf_Attachment a,
			yaf_Message b,
			yaf_Topic c,
			yaf_Forum d,
			yaf_Category e
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

create procedure [dbo].[yaf_attachment_save](@MessageID int,@FileName nvarchar(255),@Bytes int,@ContentType nvarchar(50)=null,@FileData image=null) as begin
	insert into yaf_Attachment(MessageID,FileName,Bytes,ContentType,Downloads,FileData) values(@MessageID,@FileName,@Bytes,@ContentType,0,@FileData)
end
GO

create procedure [dbo].[yaf_bannedip_delete](@ID int) as
begin
	delete from yaf_BannedIP where ID = @ID
end
GO

create procedure [dbo].[yaf_bannedip_list](@BoardID int,@ID int=null) as
begin
	if @ID is null
		select * from yaf_BannedIP where BoardID=@BoardID
	else
		select * from yaf_BannedIP where BoardID=@BoardID and ID=@ID
end
GO

create procedure [dbo].[yaf_bannedip_save](@ID int=null,@BoardID int,@Mask nvarchar(15)) as
begin
	if @ID is null or @ID = 0 begin
		insert into yaf_BannedIP(BoardID,Mask,Since) values(@BoardID,@Mask,getdate())
	end
	else begin
		update yaf_BannedIP set Mask = @Mask where ID = @ID
	end
end
GO

CREATE procedure [dbo].[yaf_board_create](
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

	SET @TimeZone = (SELECT CAST(CAST([Value] as nvarchar(50)) as int) FROM yaf_Registry WHERE LOWER([Name]) = LOWER('TimeZone'))
	SET @ForumEmail = (SELECT CAST([Value] as nvarchar(50)) FROM yaf_Registry WHERE LOWER([Name]) = LOWER('ForumEmail'))

	-- yaf_Board
	insert into yaf_Board(Name,AllowThreaded) values(@BoardName,@AllowThreaded)
	set @BoardID = SCOPE_IDENTITY()

	-- yaf_Rank
	insert into yaf_Rank(BoardID,Name,Flags,MinPosts) values(@BoardID,'Administration',0,null)
	set @RankIDAdmin = SCOPE_IDENTITY()
	insert into yaf_Rank(BoardID,Name,Flags,MinPosts) values(@BoardID,'Guest',0,null)
	set @RankIDGuest = SCOPE_IDENTITY()
	insert into yaf_Rank(BoardID,Name,Flags,MinPosts) values(@BoardID,'Newbie',3,0)
	set @RankIDNewbie = SCOPE_IDENTITY()
	insert into yaf_Rank(BoardID,Name,Flags,MinPosts) values(@BoardID,'Member',2,10)
	set @RankIDMember = SCOPE_IDENTITY()
	insert into yaf_Rank(BoardID,Name,Flags,MinPosts) values(@BoardID,'Advanced Member',2,30)
	set @RankIDAdvanced = SCOPE_IDENTITY()

	-- yaf_AccessMask
	insert into yaf_AccessMask(BoardID,Name,Flags)
	values(@BoardID,'Admin Access',1023)
	set @AccessMaskIDAdmin = SCOPE_IDENTITY()
	insert into yaf_AccessMask(BoardID,Name,Flags)
	values(@BoardID,'Moderator Access',487)
	set @AccessMaskIDModerator = SCOPE_IDENTITY()
	insert into yaf_AccessMask(BoardID,Name,Flags)
	values(@BoardID,'Member Access',423)
	set @AccessMaskIDMember = SCOPE_IDENTITY()
	insert into yaf_AccessMask(BoardID,Name,Flags)
	values(@BoardID,'Read Only Access',1)
	set @AccessMaskIDReadOnly = SCOPE_IDENTITY()
	insert into yaf_AccessMask(BoardID,Name,Flags)
	values(@BoardID,'No Access',0)

	-- yaf_Group
	insert into yaf_Group(BoardID,Name,Flags) values(@BoardID,'Administrators',1)
	set @GroupIDAdmin = SCOPE_IDENTITY()
	insert into yaf_Group(BoardID,Name,Flags) values(@BoardID,'Guests',2)
	set @GroupIDGuest = SCOPE_IDENTITY()
	insert into yaf_Group(BoardID,Name,Flags) values(@BoardID,'Registered',4)
	set @GroupIDMember = SCOPE_IDENTITY()	
	
	-- yaf_User
	insert into yaf_User(BoardID,RankID,Name,Password,Joined,LastVisit,NumPosts,TimeZone,Email,Flags)
	values(@BoardID,@RankIDGuest,'Guest','na',getdate(),getdate(),0,@TimeZone,@ForumEmail,6)
	set @UserIDGuest = SCOPE_IDENTITY()	
	
	SET @UserFlags = 2
	if @IsHostAdmin<>0 SET @UserFlags = 3
	
	insert into yaf_User(BoardID,RankID,Name,Password,Joined,LastVisit,NumPosts,TimeZone,Email,Flags)
	values(@BoardID,@RankIDAdmin,@UserName,@UserPass,getdate(),getdate(),0,@TimeZone,@UserEmail,@UserFlags)
	set @UserIDAdmin = SCOPE_IDENTITY()

	-- yaf_UserGroup
	insert into yaf_UserGroup(UserID,GroupID) values(@UserIDAdmin,@GroupIDAdmin)
	insert into yaf_UserGroup(UserID,GroupID) values(@UserIDGuest,@GroupIDGuest)

	-- yaf_Category
	insert into yaf_Category(BoardID,Name,SortOrder) values(@BoardID,'Test Category',1)
	set @CategoryID = SCOPE_IDENTITY()
	
	-- yaf_Forum
	insert into yaf_Forum(CategoryID,Name,Description,SortOrder,NumTopics,NumPosts,Flags)
	values(@CategoryID,'Test Forum','A test forum',1,0,0,4)
	set @ForumID = SCOPE_IDENTITY()

	-- yaf_ForumAccess
	insert into yaf_ForumAccess(GroupID,ForumID,AccessMaskID) values(@GroupIDAdmin,@ForumID,@AccessMaskIDAdmin)
	insert into yaf_ForumAccess(GroupID,ForumID,AccessMaskID) values(@GroupIDGuest,@ForumID,@AccessMaskIDReadOnly)
	insert into yaf_ForumAccess(GroupID,ForumID,AccessMaskID) values(@GroupIDMember,@ForumID,@AccessMaskIDMember)
end
GO

create procedure [dbo].[yaf_board_delete](@BoardID int) as
begin
	declare @tmpForumID int;
	declare forum_cursor cursor for
		select ForumID 
		from yaf_Forum a join yaf_Category b on a.CategoryID=b.CategoryID
		where b.BoardID=@BoardID
		order by ForumID desc
	
	open forum_cursor
	fetch next from forum_cursor into @tmpForumID
	while @@FETCH_STATUS = 0
	begin
		exec yaf_forum_delete @tmpForumID;
		fetch next from forum_cursor into @tmpForumID
	end
	close forum_cursor
	deallocate forum_cursor

	delete from yaf_ForumAccess where exists(select 1 from yaf_Group x where x.GroupID=yaf_ForumAccess.GroupID and x.BoardID=@BoardID)
	delete from yaf_Forum where exists(select 1 from yaf_Category x where x.CategoryID=yaf_Forum.CategoryID and x.BoardID=@BoardID)
	delete from yaf_UserGroup where exists(select 1 from yaf_User x where x.UserID=yaf_UserGroup.UserID and x.BoardID=@BoardID)
	delete from yaf_Category where BoardID=@BoardID
	delete from yaf_User where BoardID=@BoardID
	delete from yaf_Rank where BoardID=@BoardID
	delete from yaf_Group where BoardID=@BoardID
	delete from yaf_AccessMask where BoardID=@BoardID
	delete from yaf_Active where BoardID=@BoardID
	delete from yaf_Board where BoardID=@BoardID
end
GO

create procedure [dbo].[yaf_board_list](@BoardID int=null) as
begin
	select
		a.*,
		SQLVersion = @@VERSION
	from 
		yaf_Board a
	where
		(@BoardID is null or a.BoardID = @BoardID)
end
GO

create procedure [dbo].[yaf_board_poststats](@BoardID int) as
begin
	select
		Posts = (select count(1) from yaf_Message a join yaf_Topic b on b.TopicID=a.TopicID join yaf_Forum c on c.ForumID=b.ForumID join yaf_Category d on d.CategoryID=c.CategoryID where d.BoardID=@BoardID),
		Topics = (select count(1) from yaf_Topic a join yaf_Forum b on b.ForumID=a.ForumID join yaf_Category c on c.CategoryID=b.CategoryID where c.BoardID=@BoardID),
		Forums = (select count(1) from yaf_Forum a join yaf_Category b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID),
		Members = (select count(1) from yaf_User a where a.BoardID=@BoardID),
		LastPostInfo.*,
		LastMemberInfo.*
	from
		(
			select top 1 
				LastMemberInfoID= 1,
				LastMemberID	= UserID,
				LastMember	= Name
			from 
				yaf_User 
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
				yaf_Message a 
				join yaf_Topic b on b.TopicID=a.TopicID 
				join yaf_Forum c on c.ForumID=b.ForumID 
				join yaf_Category d on d.CategoryID=c.CategoryID 
				join yaf_User e on e.UserID=a.UserID
			where 
				d.BoardID=@BoardID
			order by
				a.Posted desc
		) as LastPostInfo
		on LastMemberInfoID=LastPostInfoID
end
GO

create procedure [dbo].[yaf_board_save](@BoardID int,@Name nvarchar(50),@AllowThreaded bit) as
begin
	update yaf_Board set
		Name = @Name,
		AllowThreaded = @AllowThreaded
	where BoardID=@BoardID
end
GO

create procedure [dbo].[yaf_board_stats]
	@BoardID	int = null
as 
begin
	if (@BoardID is null) begin
		select
			NumPosts	= (select count(1) from yaf_Message	where (Flags & 24)=16),
			NumTopics	= (select count(1) from yaf_Topic),
			NumUsers	= (select count(1) from yaf_User where (Flags & 2) = 2),
			BoardStart	= (select min(Joined) from yaf_User)
	end
	else begin
		select
			NumPosts	= (select count(1)	
								from yaf_Message a
								join yaf_Topic b ON a.TopicID=b.TopicID
								join yaf_Forum c ON b.ForumID=c.ForumID
								join yaf_Category d ON c.CategoryID=d.CategoryID
								where (a.Flags & 24)=16 and d.BoardID=@BoardID
							),
			NumTopics	= (select count(1) 
								from yaf_Topic a
								join yaf_Forum b ON a.ForumID=b.ForumID
								join yaf_Category c ON b.CategoryID=c.CategoryID
								where c.BoardID=@BoardID
							),
			NumUsers	= (select count(1) from yaf_User where (Flags & 2) = 2 and BoardID=@BoardID),
			BoardStart	= (select min(Joined) from yaf_User where BoardID=@BoardID)
	end
end
GO

create procedure [dbo].[yaf_category_delete](@CategoryID int) as
begin
	declare @flag int
 
	if exists(select 1 from yaf_Forum where CategoryID =  @CategoryID)
	begin
		set @flag = 0
	end else
	begin
		delete from yaf_Category where CategoryID = @CategoryID
		set @flag = 1
	end

	select @flag
end
GO

create procedure [dbo].[yaf_category_list](@BoardID int,@CategoryID int=null) as
begin
	if @CategoryID is null
		select * from yaf_Category where BoardID = @BoardID order by SortOrder
	else
		select * from yaf_Category where BoardID = @BoardID and CategoryID = @CategoryID
end
GO

create procedure [dbo].[yaf_category_listread](@BoardID int,@UserID int,@CategoryID int=null) as
begin
	select 
		a.CategoryID,
		a.Name
	from 
		yaf_Category a
		join yaf_Forum b on b.CategoryID=a.CategoryID
		join yaf_vaccess v on v.ForumID=b.ForumID
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

create procedure [dbo].[yaf_category_save](@BoardID int,@CategoryID int,@Name nvarchar(50),@SortOrder smallint) as
begin
	if @CategoryID>0 begin
		update yaf_Category set Name=@Name,SortOrder=@SortOrder where CategoryID=@CategoryID
		select CategoryID = @CategoryID
	end
	else begin
		insert into yaf_Category(BoardID,Name,SortOrder) values(@BoardID,@Name,@SortOrder)
		select CategoryID = SCOPE_IDENTITY()
	end
end
GO

create procedure [dbo].[yaf_checkemail_save](@UserID int,@Hash nvarchar(32),@Email nvarchar(50)) as
begin
	insert into yaf_CheckEmail(UserID,Email,Created,Hash)
	values(@UserID,@Email,getdate(),@Hash)	
end
GO

CREATE procedure [dbo].[yaf_checkemail_update](@Hash nvarchar(32)) as
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
		yaf_CheckEmail
	where
		Hash = @Hash

	if @UserID is null
	begin
		select convert(uniqueidentifier,NULL) as ProviderUserKey, convert(nvarchar(255),NULL) as Email
		return
	end

	-- Update new user email
	update yaf_User set Email = @Email, Flags = Flags | 2 where UserID = @UserID
	delete yaf_CheckEmail where CheckEmailID = @CheckEmailID

	-- return the UserProviderKey
	SELECT ProviderUserKey, Email FROM yaf_User WHERE UserID = @UserID
end
GO

CREATE PROCEDURE [dbo].[yaf_choice_vote](@ChoiceID int,@UserID int = NULL, @RemoteIP nvarchar(10) = NULL) AS
BEGIN
	DECLARE @PollID int

	SET @PollID = (SELECT PollID FROM yaf_Choice WHERE ChoiceID = @ChoiceID)

	IF @UserID = NULL
	BEGIN
		IF @RemoteIP != NULL
		BEGIN
			INSERT INTO yaf_PollVote (PollID, UserID, RemoteIP) VALUES (@PollID,NULL,@RemoteIP)	
		END
	END
	ELSE
	BEGIN
		INSERT INTO yaf_PollVote (PollID, UserID, RemoteIP) VALUES (@PollID,@UserID,@RemoteIP)
	END

	UPDATE yaf_Choice SET Votes = Votes + 1 WHERE ChoiceID = @ChoiceID
END
GO

create procedure [dbo].[yaf_eventlog_create](@UserID int,@Source nvarchar(50),@Description ntext,@Type int) as
begin
	insert into dbo.yaf_EventLog(UserID,Source,Description,Type)
	values(@UserID,@Source,@Description,@Type)

	-- delete entries older than 10 days
	delete from dbo.yaf_EventLog where EventTime+10<getdate()

	-- or if there are more then 1000	
	if ((select count(*) from yaf_eventlog) >= 1050)
	begin
		
		delete from dbo.yaf_EventLog WHERE EventLogID IN (SELECT TOP 100 EventLogID FROM dbo.yaf_EventLog ORDER BY EventTime)
	end	
	
end
GO

create procedure [dbo].[yaf_eventlog_delete](@EventLogID int) as
begin
	delete from dbo.yaf_EventLog where EventLogID=@EventLogID
end
GO

create procedure [dbo].[yaf_eventlog_list](@BoardID int) as
begin
	select
		a.*,
		ISNULL(b.[Name],'System') as [Name]
	from
		dbo.yaf_EventLog a
		left join dbo.yaf_User b on b.UserID=a.UserID
	where
		(b.UserID IS NULL or b.BoardID = @BoardID)		
	order by
		a.EventLogID desc
end
GO

CREATE procedure [dbo].[yaf_forum_delete](@ForumID int) as
begin
	-- Maybe an idea to use cascading foreign keys instead? Too bad they don't work on MS SQL 7.0...
	update yaf_Forum set LastMessageID=null,LastTopicID=null where ForumID=@ForumID
	update yaf_Topic set LastMessageID=null where ForumID=@ForumID
	delete from yaf_WatchTopic from yaf_Topic where yaf_Topic.ForumID = @ForumID and yaf_WatchTopic.TopicID = yaf_Topic.TopicID
	delete from yaf_Active from yaf_Topic where yaf_Topic.ForumID = @ForumID and yaf_Active.TopicID = yaf_Topic.TopicID
	delete from yaf_NntpTopic from yaf_NntpForum where yaf_NntpForum.ForumID = @ForumID and yaf_NntpTopic.NntpForumID = yaf_NntpForum.NntpForumID
	delete from yaf_NntpForum where ForumID=@ForumID	
	delete from yaf_WatchForum where ForumID = @ForumID

	-- BAI CHANGED 02.02.2004
	-- Delete topics, messages and attachments

	declare @tmpTopicID int;
	declare topic_cursor cursor for
		select TopicID from yaf_topic
		where ForumId = @ForumID
		order by TopicID desc
	
	open topic_cursor
	
	fetch next from topic_cursor
	into @tmpTopicID
	
	-- Check @@FETCH_STATUS to see if there are any more rows to fetch.
	while @@FETCH_STATUS = 0
	begin
		exec yaf_topic_delete @tmpTopicID,1,1;
	
	   -- This is executed as long as the previous fetch succeeds.
		fetch next from topic_cursor
		into @tmpTopicID
	end
	
	close topic_cursor
	deallocate topic_cursor

	-- TopicDelete finished
	-- END BAI CHANGED 02.02.2004

	delete from yaf_ForumAccess where ForumID = @ForumID
	--ABOT CHANGED
	--Delete UserForums Too 
	delete from yaf_UserForum where ForumID = @ForumID
	--END ABOT CHANGED 09.04.2004
	delete from yaf_Forum where ForumID = @ForumID
end

GO

create procedure [dbo].[yaf_forum_list](@BoardID int,@ForumID int=null) as
begin
	if @ForumID = 0 set @ForumID = null
	if @ForumID is null
		select a.* from yaf_Forum a join yaf_Category b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID order by a.SortOrder
	else
		select a.* from yaf_Forum a join yaf_Category b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID and a.ForumID = @ForumID
end
GO

CREATE procedure [dbo].[yaf_forum_listall] (@BoardID int,@UserID int,@root int = 0) as
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
        yaf_Forum a
        join yaf_Category b on b.CategoryID=a.CategoryID
        join yaf_vaccess c on c.ForumID=a.ForumID
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
        yaf_Forum a
        join yaf_Category b on b.CategoryID=a.CategoryID
        join yaf_vaccess c on c.ForumID=a.ForumID
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
        yaf_Forum a
        join yaf_Category b on b.CategoryID=a.CategoryID
        join yaf_vaccess c on c.ForumID=a.ForumID
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

CREATE PROCEDURE [dbo].[yaf_forum_listall_fromcat](@BoardID int,@CategoryID int) AS
BEGIN
	SELECT     b.CategoryID, b.Name AS Category, a.ForumID, a.Name AS Forum, a.ParentID
	FROM         yaf_Forum a INNER JOIN
						  yaf_Category b ON b.CategoryID = a.CategoryID
		WHERE
			b.CategoryID=@CategoryID and
			b.BoardID=@BoardID
		ORDER BY
			b.SortOrder,
			a.SortOrder
END
GO

create procedure [dbo].[yaf_forum_listallmymoderated](@BoardID int,@UserID int) as
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
			yaf_Category a
			join yaf_Forum b on b.CategoryID=a.CategoryID
		where
			a.BoardID=@BoardID and
			b.ParentID is null
	
		union
	
		select
			c.ForumID,
			Indent = 1
		from
			yaf_Category a
			join yaf_Forum b on b.CategoryID=a.CategoryID
			join yaf_Forum c on c.ParentID=b.ForumID
		where
			a.BoardID=@BoardID and
			b.ParentID is null
	
		union
	
		select
			d.ForumID,
			Indent = 2
		from
			yaf_Category a
			join yaf_Forum b on b.CategoryID=a.CategoryID
			join yaf_Forum c on c.ParentID=b.ForumID
			join yaf_Forum d on d.ParentID=c.ForumID
		where
			a.BoardID=@BoardID and
			b.ParentID is null
		) as x
		join yaf_Forum a on a.ForumID=x.ForumID
		join yaf_Category b on b.CategoryID=a.CategoryID
		join yaf_vaccess c on c.ForumID=a.ForumID
	where
		c.UserID=@UserID and
		b.BoardID=@BoardID and
		c.ModeratorAccess>0
	order by
		b.SortOrder,
		a.SortOrder
end
GO

create procedure [dbo].[yaf_forum_listpath](@ForumID int) as
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
			yaf_Forum a
		where
			a.ForumID=@ForumID

		union

		select
			b.ForumID,
			Indent = 1
		from
			yaf_Forum a
			join yaf_Forum b on b.ForumID=a.ParentID
		where
			a.ForumID=@ForumID

		union

		select
			c.ForumID,
			Indent = 2
		from
			yaf_Forum a
			join yaf_Forum b on b.ForumID=a.ParentID
			join yaf_Forum c on c.ForumID=b.ParentID
		where
			a.ForumID=@ForumID

		union 

		select
			d.ForumID,
			Indent = 3
		from
			yaf_Forum a
			join yaf_Forum b on b.ForumID=a.ParentID
			join yaf_Forum c on c.ForumID=b.ParentID
			join yaf_Forum d on d.ForumID=c.ParentID
		where
			a.ForumID=@ForumID
		) as x	
		join yaf_Forum a on a.ForumID=x.ForumID
	order by
		x.Indent desc
end
GO

create procedure [dbo].[yaf_forum_listread](@BoardID int,@UserID int,@CategoryID int=null,@ParentID int=null) as
begin
	select 
		a.CategoryID, 
		Category		= a.Name, 
		ForumID			= b.ForumID,
		Forum			= b.Name, 
		Description,
		Topics			= dbo.yaf_forum_topics(b.ForumID),
		Posts			= dbo.yaf_forum_posts(b.ForumID),
		Subforums		= dbo.yaf_forum_subforums(b.ForumID, @UserID),
		LastPosted		= t.LastPosted,
		LastMessageID	= t.LastMessageID,
		LastUserID		= t.LastUserID,
		LastUser		= IsNull(t.LastUserName,(select Name from yaf_User x where x.UserID=t.LastUserID)),
		LastTopicID		= t.TopicID,
		LastTopicName	= t.Topic,
		b.Flags,
		Viewing			= (select count(1) from yaf_Active x where x.ForumID=b.ForumID),
		b.RemoteURL,
		x.ReadAccess
	from 
		yaf_Category a
		join yaf_Forum b on b.CategoryID=a.CategoryID
		join yaf_vaccess x on x.ForumID=b.ForumID
		left outer join yaf_Topic t ON t.TopicID = dbo.yaf_forum_lasttopic(b.ForumID,@UserID,b.LastTopicID,b.LastPosted)
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

create procedure [dbo].[yaf_forum_listSubForums](@ForumID int) as
begin
	select Sum(1) from yaf_Forum where ParentID = @ForumID
end
GO

create procedure [dbo].[yaf_forum_listtopics](@ForumID int) as
begin
select * from yaf_Topic
Where ForumID = @ForumID
end
GO

CREATE PROCEDURE [dbo].[yaf_forum_moderatelist](@BoardID int,@UserID int) AS
BEGIN

SELECT
		b.*,
		MessageCount  = 
		(SELECT     count(yaf_Message.MessageID)
		FROM         yaf_Message INNER JOIN
							  yaf_Topic ON yaf_Message.TopicID = yaf_Topic.TopicID
		WHERE ((yaf_Message.Flags & 16)=0) and ((yaf_Message.Flags & 8)=0) and ((yaf_Topic.Flags & 8) = 0) AND (yaf_Topic.ForumID=b.ForumID)),
		ReportCount	= 
		(SELECT     count(yaf_Message.MessageID)
		FROM         yaf_Message INNER JOIN
							  yaf_Topic ON yaf_Message.TopicID = yaf_Topic.TopicID
		WHERE ((yaf_Message.Flags & 128)=128) and ((yaf_Message.Flags & 8)=0) and ((yaf_Topic.Flags & 8) = 0) AND (yaf_Topic.ForumID=b.ForumID)),
		SpamCount	= 
		(SELECT     count(yaf_Message.MessageID)
		FROM         yaf_Message INNER JOIN
							  yaf_Topic ON yaf_Message.TopicID = yaf_Topic.TopicID
		WHERE ((yaf_Message.Flags & 256)=256) and ((yaf_Message.Flags & 8)=0) and ((yaf_Topic.Flags & 8) = 0) AND (yaf_Topic.ForumID=b.ForumID))
		
	FROM
		yaf_Category a

	JOIN yaf_Forum b ON b.CategoryID=a.CategoryID
	JOIN yaf_vaccess c ON c.ForumID=b.ForumID

	WHERE
		a.BoardID=@BoardID AND
		c.ModeratorAccess>0 AND
		c.UserID=@UserID
	ORDER BY
		a.SortOrder,
		b.SortOrder
END
GO

create procedure [dbo].[yaf_forum_moderators] as
begin
	select
		ForumID = a.ForumID, 
		ModeratorID = a.GroupID, 
		ModeratorName = b.Name,
		IsGroup=1
	from
		yaf_ForumAccess a
		JOIN yaf_Group b ON b.GroupID = a.GroupID
		JOIN yaf_AccessMask c ON c.AccessMaskID = a.AccessMaskID
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
		yaf_User a
		JOIN yaf_vmaccess x ON a.UserID=x.UserID
	where
		x.ModeratorAccess<>0
	order by
		IsGroup desc,
		ModeratorName asc
end
GO

CREATE procedure [dbo].[yaf_forum_save](
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
		update yaf_Forum set 
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
		select @BoardID=BoardID from yaf_Category where CategoryID=@CategoryID
	
		insert into yaf_Forum(ParentID,Name,Description,SortOrder,CategoryID,NumTopics,NumPosts,RemoteURL,ThemeURL,Flags)
		values(@ParentID,@Name,@Description,@SortOrder,@CategoryID,0,0,@RemoteURL,@ThemeURL,@Flags)
		select @ForumID = SCOPE_IDENTITY()

		insert into yaf_ForumAccess(GroupID,ForumID,AccessMaskID) 
		select GroupID,@ForumID,@AccessMaskID
		from yaf_Group 
		where BoardID=@BoardID
	end
	select ForumID = @ForumID
end
GO

create procedure [dbo].[yaf_forum_updatelastpost](@ForumID int) as
begin
	update yaf_Forum set
		LastPosted = (select top 1 y.Posted from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16 order by y.Posted desc),
		LastTopicID = (select top 1 y.TopicID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16 order by y.Posted desc),
		LastMessageID = (select top 1 y.MessageID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16 order by y.Posted desc),
		LastUserID = (select top 1 y.UserID from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16 order by y.Posted desc),
		LastUserName = (select top 1 y.UserName from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16 order by y.Posted desc)
	where ForumID = @ForumID
end
GO

create procedure [dbo].[yaf_forum_updatestats](@ForumID int) as
begin
	update yaf_Forum set 
		NumPosts = (select count(1) from yaf_Message x,yaf_Topic y where y.TopicID=x.TopicID and y.ForumID = yaf_Forum.ForumID and (x.Flags & 24)=16),
		NumTopics = (select count(distinct x.TopicID) from yaf_Topic x,yaf_Message y where x.ForumID=yaf_Forum.ForumID and y.TopicID=x.TopicID and (y.Flags & 24)=16)
	where ForumID=@ForumID
end
GO

CREATE procedure [dbo].[yaf_forumaccess_group](@GroupID int) as
begin
	select 
		a.*,
		ForumName = b.Name,
		CategoryName = c.Name ,
		CategoryId = b.CategoryID,
		ParentID = b.ParentID 
	from 
		yaf_ForumAccess a, 
		yaf_Forum b, 
		yaf_Category c 
	where 
		a.GroupID = @GroupID and 
		b.ForumID=a.ForumID and 
		c.CategoryID=b.CategoryID 
	order by 
		c.SortOrder,
		b.SortOrder
end
GO

create procedure [dbo].[yaf_forumaccess_list](@ForumID int) as
begin
	select 
		a.*,
		GroupName=b.Name 
	from 
		yaf_ForumAccess a, 
		yaf_Group b 
	where 
		a.ForumID = @ForumID and 
		b.GroupID = a.GroupID
end
GO

create procedure [dbo].[yaf_forumaccess_save](
	@ForumID			int,
	@GroupID			int,
	@AccessMaskID		int
) as
begin
	update yaf_ForumAccess set 
		AccessMaskID=@AccessMaskID
	where 
		ForumID = @ForumID and 
		GroupID = @GroupID
end
GO

create procedure [dbo].[yaf_group_delete](@GroupID int) as
begin
	delete from yaf_ForumAccess where GroupID = @GroupID
	delete from yaf_UserGroup where GroupID = @GroupID
	delete from yaf_Group where GroupID = @GroupID
end
GO

create procedure [dbo].[yaf_group_list](@BoardID int,@GroupID int=null) as
begin
	if @GroupID is null
		select * from yaf_Group where BoardID=@BoardID
	else
		select * from yaf_Group where BoardID=@BoardID and GroupID=@GroupID
end
GO

create procedure [dbo].[yaf_group_member](@BoardID int,@UserID int) as
begin
	select 
		a.GroupID,
		a.Name,
		Member = (select count(1) from yaf_UserGroup x where x.UserID=@UserID and x.GroupID=a.GroupID)
	from
		yaf_Group a
	where
		a.BoardID=@BoardID
	order by
		a.Name
end
GO

CREATE procedure [dbo].[yaf_group_save](
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
		update yaf_Group set
			Name = @Name,
			Flags = @Flags
		where GroupID = @GroupID
	end
	else begin
		insert into yaf_Group(Name,BoardID,Flags)
		values(@Name,@BoardID,@Flags);
		set @GroupID = SCOPE_IDENTITY()
		insert into yaf_ForumAccess(GroupID,ForumID,AccessMaskID)
		select @GroupID,a.ForumID,@AccessMaskID from yaf_Forum a join yaf_Category b on b.CategoryID=a.CategoryID where b.BoardID=@BoardID
	end
	select GroupID = @GroupID
end
GO

create procedure [dbo].[yaf_mail_create](@From nvarchar(50),@To nvarchar(50),@Subject nvarchar(100),@Body ntext) as 
begin
	insert into yaf_Mail(FromUser,ToUser,Created,Subject,Body)
	values(@From,@To,getdate(),@Subject,@Body)
end
GO

create procedure [dbo].[yaf_mail_createwatch](@TopicID int,@From nvarchar(50),@Subject nvarchar(100),@Body ntext,@UserID int) as begin
	insert into yaf_Mail(FromUser,ToUser,Created,Subject,Body)
	select
		@From,
		b.Email,
		getdate(),
		@Subject,
		@Body
	from
		yaf_WatchTopic a,
		yaf_User b
	where
		b.UserID <> @UserID and
		b.UserID = a.UserID and
		a.TopicID = @TopicID and
		(a.LastMail is null or a.LastMail < b.LastVisit)
	
	insert into yaf_Mail(FromUser,ToUser,Created,Subject,Body)
	select
		@From,
		b.Email,
		getdate(),
		@Subject,
		@Body
	from
		yaf_WatchForum a,
		yaf_User b,
		yaf_Topic c
	where
		b.UserID <> @UserID and
		b.UserID = a.UserID and
		c.TopicID = @TopicID and
		c.ForumID = a.ForumID and
		(a.LastMail is null or a.LastMail < b.LastVisit) and
		not exists(select 1 from yaf_WatchTopic x where x.UserID=b.UserID and x.TopicID=c.TopicID)

	update yaf_WatchTopic set LastMail = getdate() 
	where TopicID = @TopicID
	and UserID <> @UserID
	
	update yaf_WatchForum set LastMail = getdate() 
	where ForumID = (select ForumID from yaf_Topic where TopicID = @TopicID)
	and UserID <> @UserID
end
GO

create procedure [dbo].[yaf_mail_delete](@MailID int) as
begin
	delete from yaf_Mail where MailID = @MailID
end
GO

create procedure [dbo].[yaf_mail_list] as
begin
	select top 10 * from yaf_Mail order by Created
end
GO

create procedure [dbo].[yaf_message_approve](@MessageID int) as begin
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
		yaf_Message a,
		yaf_Topic b
	where
		a.MessageID = @MessageID and
		b.TopicID = a.TopicID

	-- update yaf_Message
	update yaf_Message set Flags = Flags | 16 where MessageID = @MessageID

	-- update yaf_User
	if exists(select 1 from yaf_Forum where ForumID=@ForumID and (Flags & 4)=0)
	begin
		update yaf_User set NumPosts = NumPosts + 1 where UserID = @UserID
		exec yaf_user_upgrade @UserID
	end

	-- update yaf_Forum
	update yaf_Forum set
		LastPosted = @Posted,
		LastTopicID = @TopicID,
		LastMessageID = @MessageID,
		LastUserID = @UserID,
		LastUserName = @UserName
	where ForumID = @ForumID

	-- update yaf_Topic
	update yaf_Topic set
		LastPosted = @Posted,
		LastMessageID = @MessageID,
		LastUserID = @UserID,
		LastUserName = @UserName,
		NumPosts = (select count(1) from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16)
	where TopicID = @TopicID
	
	-- update forum stats
	exec yaf_forum_updatestats @ForumID
end
GO

create procedure [dbo].[yaf_message_delete](@MessageID int) as
begin
	declare @TopicID		int
	declare @ForumID		int
	declare @MessageCount	int
	declare @LastMessageID	int

	-- Find TopicID and ForumID
	select @TopicID=b.TopicID,@ForumID=b.ForumID from yaf_Message a,yaf_Topic b where a.MessageID=@MessageID and b.TopicID=a.TopicID

	-- Update LastMessageID in Topic and Forum
	update yaf_Topic set 
		LastPosted = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	update yaf_Forum set 
		LastPosted = null,
		LastTopicID = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	-- "Delete" message
	update yaf_Message set Flags = Flags | 8 where MessageID = @MessageID
	
	-- Delete topic if there are no more messages
	select @MessageCount = count(1) from yaf_Message where TopicID = @TopicID and (Flags & 8)=0
	if @MessageCount=0 exec yaf_topic_delete @TopicID
	-- update lastpost
	exec yaf_topic_updatelastpost @ForumID,@TopicID
	exec yaf_forum_updatestats @ForumID
	-- update topic numposts
	update yaf_Topic set
		NumPosts = (select count(1) from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16)
	where TopicID = @TopicID
end
GO

create procedure [dbo].[yaf_message_findunread](@TopicID int,@LastRead datetime) as
begin
	select top 1 MessageID from yaf_Message
	where TopicID=@TopicID and Posted>@LastRead
	order by Posted
end
GO

CREATE PROCEDURE [dbo].[yaf_message_getReplies](@MessageID int) as
BEGIN
	SELECT MessageID FROM yaf_Message WHERE ReplyTo = @MessageID
END
GO

CREATE PROCEDURE [dbo].[yaf_message_list](@MessageID int) AS
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
		yaf_Message a,
		yaf_User b,
		yaf_Topic c,
		yaf_Forum d
	WHERE
		a.MessageID = @MessageID AND
		b.UserID = a.UserID AND
		c.TopicID = a.TopicID AND
		c.ForumID = d.ForumID
END
GO

CREATE PROCEDURE [dbo].[yaf_message_listreported](@MessageFlag int, @ForumID int) AS
BEGIN
	SELECT
		a.*,
		OriginalMessage = b.Message,
		UserName	= IsNull(b.UserName,d.Name),
		UserID = b.UserID,
		Posted		= b.Posted,
		Topic		= c.Topic,
		NumberOfReports = (SELECT count(LogID) FROM yaf_MessageReportedAudit WHERE yaf_MessageReportedAudit.MessageID = a.MessageID)
	FROM
		yaf_MessageReported a
	INNER JOIN
		yaf_Message b ON a.MessageID = b.MessageID
	INNER JOIN
		yaf_Topic c ON b.TopicID = c.TopicID
	INNER JOIN
		yaf_User d ON b.UserID = d.UserID
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

CREATE PROCEDURE [dbo].[yaf_message_report](@ReportFlag int, @MessageID int, @ReporterID int, @ReportedDate datetime ) AS
BEGIN
	
	IF NOT exists(SELECT MessageID from yaf_MessageReportedAudit WHERE MessageID=@MessageID AND UserID=@ReporterID)
		INSERT INTO yaf_MessageReportedAudit(MessageID,UserID,Reported) VALUES (@MessageID,@ReporterID,@ReportedDate)

	IF NOT exists(SELECT MessageID FROM yaf_MessageReported WHERE MessageID=@MessageID)
	BEGIN
		INSERT INTO yaf_MessageReported(MessageID, [Message])
		SELECT 
			a.MessageID,
			a.Message
		FROM
			yaf_Message a
		WHERE
			a.MessageID = @MessageID
	END

	-- update yaf_Message
	UPDATE yaf_Message SET Flags = Flags | POWER(2, @ReportFlag) WHERE MessageID = @MessageID

END
GO

CREATE PROCEDURE [dbo].[yaf_message_reportresolve](@MessageFlag int, @MessageID int, @UserID int) AS
BEGIN
	UPDATE yaf_MessageReported 
	SET Resolved = 1, ResolvedBy = @UserID, ResolvedDate = GETDATE()
	WHERE MessageID = @MessageID;
	
	/* Remove Flag */
	UPDATE yaf_Message
	SET Flags = Flags & (~POWER(2, @MessageFlag))
	WHERE MessageID = @MessageID;
END
GO

CREATE PROCEDURE [dbo].[yaf_message_reportcopyover](@MessageID int) AS
BEGIN
	UPDATE yaf_MessageReported 
	SET yaf_MessageReported.Message = (SELECT Message FROM yaf_Message WHERE yaf_Message.MessageID=@MessageID)
	WHERE MessageID = @MessageID;

END
GO

CREATE procedure [dbo].[yaf_message_save](
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
	FROM yaf_Topic x,yaf_Forum y
	WHERE x.TopicID = @TopicID AND y.ForumID=x.ForumID

	IF @ReplyTo IS NULL
			SELECT @Position = 0, @Indent = 0 -- New thread

	ELSE IF @ReplyTo<0
		-- Find post to reply to AND indent of this post
		SELECT TOP 1 @ReplyTo = MessageID, @Indent = Indent+1
		FROM yaf_Message
		WHERE TopicID = @TopicID AND ReplyTo IS NULL
		ORDER BY Posted

	ELSE
		-- Got reply, find indent of this post
			SELECT @Indent=Indent+1
			FROM yaf_Message
			WHERE MessageID=@ReplyTo

	-- Find position
	IF @ReplyTo IS NOT NULL
    BEGIN
        DECLARE @temp INT
		
        SELECT @temp=ReplyTo,@Position=Position FROM yaf_Message WHERE MessageID=@ReplyTo

        IF @temp IS NULL
			-- We are replying to first post
            SELECT @Position=MAX(Position)+1 FROM yaf_Message WHERE TopicID=@TopicID

        ELSE
			-- Last position of replies to parent post
            SELECT @Position=MIN(Position) FROM yaf_Message WHERE ReplyTo=@temp AND Position>@Position

        -- No replies, THEN USE parent post's position+1
        IF @Position IS NULL
            SELECT @Position=Position+1 FROM yaf_Message WHERE MessageID=@ReplyTo
		-- Increase position of posts after this

        UPDATE yaf_Message SET Position=Position+1 WHERE TopicID=@TopicID AND Position>=@Position
    END

	-- Add points to Users total points
	UPDATE yaf_User SET Points = Points + 3 WHERE UserID = @UserID

	INSERT yaf_Message ( UserID, Message, TopicID, Posted, UserName, IP, ReplyTo, Position, Indent, Flags, BlogPostID)
	VALUES ( @UserID, @Message, @TopicID, @Posted, @UserName, @IP, @ReplyTo, @Position, @Indent, @Flags & ~16, @BlogPostID)

	SET @MessageID = SCOPE_IDENTITY()

	IF ((@ForumFlags & 8) = 0) OR ((@Flags & 16) = 16)
		EXEC yaf_message_approve @MessageID
END
	
GO

CREATE procedure [dbo].[yaf_message_unapproved](@ForumID int) as begin
	select
		MessageID	= b.MessageID,
		UserName	= IsNull(b.UserName,c.Name),
		Posted		= b.Posted,
		Topic		= a.Topic,
		Message		= b.Message
	from
		yaf_Topic a,
		yaf_Message b,
		yaf_User c
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

CREATE procedure [dbo].[yaf_message_update](@MessageID int,@Priority int,@Subject nvarchar(100),@Flags int, @Message ntext, @Reason as nvarchar(100), @IsModeratorChanged bit) as
begin
	declare @TopicID	int
	declare	@ForumFlags	int

	set @Flags = @Flags & ~16	
	
	select 
		@TopicID	= a.TopicID,
		@ForumFlags	= c.Flags
	from 
		yaf_Message a,
		yaf_Topic b,
		yaf_Forum c
	where 
		a.MessageID = @MessageID and
		b.TopicID = a.TopicID and
		c.ForumID = b.ForumID

	if (@ForumFlags & 8)=0 set @Flags = @Flags | 16

	update yaf_Message set
		Message = @Message,
		Edited = getdate(),
		Flags = @Flags,
		IsModeratorChanged  = @IsModeratorChanged,
                EditReason = @Reason
	where
		MessageID = @MessageID

	if @Priority is not null begin
		update yaf_Topic set
			Priority = @Priority
		where
			TopicID = @TopicID
	end

	if not @Subject = '' and @Subject is not null begin
		update yaf_Topic set
			Topic = @Subject
		where
			TopicID = @TopicID
	end 
	
	-- If forum is moderated, make sure last post pointers are correct
	if (@ForumFlags & 8)<>0 exec yaf_topic_updatelastpost
end
GO

create procedure [dbo].[yaf_nntpforum_delete](@NntpForumID int) as
begin
	delete from yaf_NntpTopic where NntpForumID = @NntpForumID
	delete from yaf_NntpForum where NntpForumID = @NntpForumID
end
GO

create procedure [dbo].[yaf_nntpforum_list](@BoardID int,@Minutes int=null,@NntpForumID int=null,@Active bit=null) as
begin
	select
		a.Name,
		a.Address,
		Port = IsNull(a.Port,119),
		a.NntpServerID,
		b.NntpForumID,
		b.GroupName,
		b.ForumID,
		b.LastMessageNo,
		b.LastUpdate,
		b.Active,
		ForumName = c.Name
	from
		yaf_NntpServer a
		join yaf_NntpForum b on b.NntpServerID = a.NntpServerID
		join yaf_Forum c on c.ForumID = b.ForumID
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

create procedure [dbo].[yaf_nntpforum_save](@NntpForumID int=null,@NntpServerID int,@GroupName nvarchar(100),@ForumID int,@Active bit) as
begin
	if @NntpForumID is null
		insert into yaf_NntpForum(NntpServerID,GroupName,ForumID,LastMessageNo,LastUpdate,Active)
		values(@NntpServerID,@GroupName,@ForumID,0,getdate(),@Active)
	else
		update yaf_NntpForum set
			NntpServerID = @NntpServerID,
			GroupName = @GroupName,
			ForumID = @ForumID,
			Active = @Active
		where NntpForumID = @NntpForumID
end
GO

create procedure [dbo].[yaf_nntpforum_update](@NntpForumID int,@LastMessageNo int,@UserID int) as
begin
	declare	@ForumID	int
	
	select @ForumID=ForumID from yaf_NntpForum where NntpForumID=@NntpForumID

	update yaf_NntpForum set
		LastMessageNo = @LastMessageNo,
		LastUpdate = getdate()
	where NntpForumID = @NntpForumID

	update yaf_Topic set 
		NumPosts = (select count(1) from yaf_message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16)
	where ForumID=@ForumID

	--exec yaf_user_upgrade @UserID
	exec yaf_forum_updatestats @ForumID
	-- exec yaf_topic_updatelastpost @ForumID,null
end
GO

create procedure [dbo].[yaf_nntpserver_delete](@NntpServerID int) as
begin
	delete from yaf_NntpTopic where NntpForumID in (select NntpForumID from yaf_NntpForum where NntpServerID = @NntpServerID)
	delete from yaf_NntpForum where NntpServerID = @NntpServerID
	delete from yaf_NntpServer where NntpServerID = @NntpServerID
end
GO

create procedure [dbo].[yaf_nntpserver_list](@BoardID int=null,@NntpServerID int=null) as
begin
	if @NntpServerID is null
		select * from yaf_NntpServer where BoardID=@BoardID order by Name
	else
		select * from yaf_NntpServer where NntpServerID=@NntpServerID
end
GO

create procedure [dbo].[yaf_nntpserver_save](
	@NntpServerID 	int=null,
	@BoardID	int,
	@Name		nvarchar(50),
	@Address	nvarchar(100),
	@Port		int,
	@UserName	nvarchar(50)=null,
	@UserPass	nvarchar(50)=null
) as begin
	if @NntpServerID is null
		insert into yaf_NntpServer(Name,BoardID,Address,Port,UserName,UserPass)
		values(@Name,@BoardID,@Address,@Port,@UserName,@UserPass)
	else
		update yaf_NntpServer set
			Name = @Name,
			Address = @Address,
			Port = @Port,
			UserName = @UserName,
			UserPass = @UserPass
		where NntpServerID = @NntpServerID
end
GO

create procedure [dbo].[yaf_nntptopic_list](@Thread char(32)) as
begin
	select
		a.*
	from
		yaf_NntpTopic a
	where
		a.Thread = @Thread
end
GO

create procedure [dbo].[yaf_nntptopic_savemessage](
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

	select @ForumID=ForumID from yaf_NntpForum where NntpForumID=@NntpForumID

	if exists(select 1 from yaf_NntpTopic where Thread=@Thread)
	begin
		-- thread exists
		select @TopicID=TopicID from yaf_NntpTopic where Thread=@Thread
	end else
	begin
		-- thread doesn't exists
		insert into yaf_Topic(ForumID,UserID,UserName,Posted,Topic,Views,Priority,NumPosts)
		values(@ForumID,@UserID,@UserName,@Posted,@Topic,0,0,0)
		set @TopicID=SCOPE_IDENTITY()

		insert into yaf_NntpTopic(NntpForumID,Thread,TopicID)
		values(@NntpForumID,@Thread,@TopicID)
	end

	-- save message
	insert into yaf_Message(TopicID,UserID,UserName,Posted,Message,IP,Position,Indent)
	values(@TopicID,@UserID,@UserName,@Posted,@Body,@IP,0,0)
	set @MessageID=SCOPE_IDENTITY()

	-- update user
	if exists(select 1 from yaf_Forum where ForumID=@ForumID and (Flags & 4)=0)
	begin
		update yaf_User set NumPosts=NumPosts+1 where UserID=@UserID
	end
	
	-- update topic
	update yaf_Topic set 
		LastPosted		= @Posted,
		LastMessageID	= @MessageID,
		LastUserID		= @UserID,
		LastUserName	= @UserName
	where TopicID=@TopicID	
	-- update forum
	update yaf_Forum set
		LastPosted		= @Posted,
		LastTopicID	= @TopicID,
		LastMessageID	= @MessageID,
		LastUserID		= @UserID,
		LastUserName	= @UserName
	where ForumID=@ForumID and (LastPosted is null or LastPosted<@Posted)
end
GO

CREATE procedure [dbo].[yaf_pageload](
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
		select @UserID = UserID from yaf_User where BoardID=@BoardID and (Flags & 4)<>0
		set @rowcount=@@rowcount
		if @rowcount<>1
		begin
			raiserror('Found %d possible guest users. There should be one and only one user marked as guest.',16,1,@rowcount)
		end
		set @IsGuest = 1
		set @UserBoardID = @BoardID
	end else
	begin
		select @UserID = UserID, @UserBoardID = BoardID from yaf_User where BoardID=@BoardID and ProviderUserKey=@UserKey
		set @IsGuest = 0
	end
	-- Check valid ForumID
	if @ForumID is not null and not exists(select 1 from yaf_Forum where ForumID=@ForumID) begin
		set @ForumID = null
	end
	-- Check valid CategoryID
	if @CategoryID is not null and not exists(select 1 from yaf_Category where CategoryID=@CategoryID) begin
		set @CategoryID = null
	end
	-- Check valid MessageID
	if @MessageID is not null and not exists(select 1 from yaf_Message where MessageID=@MessageID) begin
		set @MessageID = null
	end
	-- Check valid TopicID
	if @TopicID is not null and not exists(select 1 from yaf_Topic where TopicID=@TopicID) begin
		set @TopicID = null
	end
	
	-- get previous visit
	if @IsGuest=0 begin
		select @PreviousVisit = LastVisit from dbo.yaf_User where UserID = @UserID
	end
	
	-- update last visit
	update yaf_User set 
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
			yaf_Message a,
			yaf_Topic b,
			yaf_Forum c,
			yaf_Category d
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
			yaf_Topic a,
			yaf_Forum b,
			yaf_Category c
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
			yaf_Forum a,
			yaf_Category b
		where
			a.ForumID = @ForumID and
			b.CategoryID = a.CategoryID and
			b.BoardID = @BoardID
	end
	-- update active
	if @UserID is not null and @UserBoardID=@BoardID begin
		if exists(select 1 from yaf_Active where SessionID=@SessionID and BoardID=@BoardID)
		begin
			update yaf_Active set
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
			insert into yaf_Active(SessionID,BoardID,UserID,IP,Login,LastActive,Location,ForumID,TopicID,Browser,Platform)
			values(@SessionID,@BoardID,@UserID,@IP,getdate(),getdate(),@Location,@ForumID,@TopicID,@Browser,@Platform)
		end
		-- remove duplicate users
		if @IsGuest=0
			delete from yaf_Active where UserID=@UserID and BoardID=@BoardID and SessionID<>@SessionID
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
		CategoryName		= (select Name from yaf_Category where CategoryID = @CategoryID),
		ForumID				= @ForumID,
		ForumName			= (select Name from yaf_Forum where ForumID = @ForumID),
		TopicID				= @TopicID,
		TopicName			= (select Topic from yaf_Topic where TopicID = @TopicID),
		MailsPending		= (select count(1) from yaf_Mail),
		Incoming			= (select count(1) from yaf_UserPMessage where UserID=a.UserID and IsRead=0),
		ForumTheme			= (select ThemeURL from yaf_Forum where ForumID = @ForumID)
	from
		yaf_User a
		left join yaf_vaccess x on x.UserID=a.UserID and x.ForumID=IsNull(@ForumID,0)
	where
		a.UserID = @UserID
end
GO

create procedure [dbo].[yaf_pmessage_delete](@PMessageID int, @FromOutbox bit = 0) as
begin
	if @FromOutbox=1
		update [yaf_UserPMessage] set [IsInOutbox] = 0 where [PMessageID]=@PMessageID
	else
	BEGIN
		delete from [yaf_UserPMessage] where [PMessageID]=@PMessageID
		delete from [yaf_PMessage] where [PMessageID]=@PMessageID
	END
end
GO

create procedure [dbo].[yaf_pmessage_info] as
begin
	select
		NumRead	= (select count(1) from yaf_UserPMessage where IsRead<>0),
		NumUnread = (select count(1) from yaf_UserPMessage where IsRead=0),
		NumTotal = (select count(1) from yaf_UserPMessage)
end
GO

CREATE PROCEDURE [dbo].[yaf_pmessage_list](@FromUserID int=null,@ToUserID int=null,@PMessageID int=null) AS
BEGIN
	SELECT PMessageID, UserPMessageID, FromUserID, FromUser, ToUserID, ToUser, Created, Subject, Body, Flags, IsRead, IsInOutbox, IsArchived
		FROM yaf_PMessageView
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
			yaf_PMessage a,
			yaf_User b,
			yaf_User c,
			yaf_UserPMessage d
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
			yaf_PMessage a,
			yaf_User b,
			yaf_User c,
			yaf_UserPMessage d
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

create procedure [dbo].[yaf_pmessage_markread](@UserPMessageID int=null) as begin
	update yaf_UserPMessage set IsRead=1 where UserPMessageID=@UserPMessageID
end
GO

create procedure [dbo].[yaf_pmessage_prune](@DaysRead int,@DaysUnread int) as
begin
	delete from yaf_UserPMessage
	where IsRead<>0
	and datediff(dd,(select Created from yaf_PMessage x where x.PMessageID=yaf_UserPMessage.PMessageID),getdate())>@DaysRead

	delete from yaf_UserPMessage
	where IsRead=0
	and datediff(dd,(select Created from yaf_PMessage x where x.PMessageID=yaf_UserPMessage.PMessageID),getdate())>@DaysUnread

	delete from yaf_PMessage
	where not exists(select 1 from yaf_UserPMessage x where x.PMessageID=yaf_PMessage.PMessageID)
end
GO

create procedure [dbo].[yaf_pmessage_save](
	@FromUserID	int,
	@ToUserID	int,
	@Subject	nvarchar(100),
	@Body		ntext,
	@Flags		int
) as
begin
	declare @PMessageID int
	declare @UserID int

	insert into yaf_PMessage(FromUserID,Created,Subject,Body,Flags)
	values(@FromUserID,getdate(),@Subject,@Body,@Flags)

	set @PMessageID = SCOPE_IDENTITY()
	if (@ToUserID = 0)
	begin
		insert into yaf_UserPMessage(UserID,PMessageID,IsRead,IsInOutbox)
		select
				a.UserID,@PMessageID,0,1
		from
				yaf_User a
				join yaf_UserGroup b on b.UserID=a.UserID
				join yaf_Group c on c.GroupID=b.GroupID where
				(c.Flags & 2)=0 and
				c.BoardID=(select BoardID from yaf_User x where x.UserID=@FromUserID) and a.UserID<>@FromUserID
		group by
				a.UserID
	end
	else
	begin
		insert into yaf_UserPMessage(UserID,PMessageID,IsRead,IsInOutbox) values(@ToUserID,@PMessageID,0,1)
	end
end
GO


CREATE PROCEDURE [dbo].[yaf_pmessage_archive](@PMessageID int = NULL) AS
BEGIN
	UPDATE yaf_UserPMessage SET IsArchived=1 WHERE UserPMessageID=@PMessageID
END
GO

CREATE procedure [dbo].[yaf_poll_save](
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
	insert into yaf_Poll(Question,Closes) values(@Question,@Closes)
	set @PollID = SCOPE_IDENTITY()
	if @Choice1<>'' and @Choice1 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice1,0)
	if @Choice2<>'' and @Choice2 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice2,0)
	if @Choice3<>'' and @Choice3 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice3,0)
	if @Choice4<>'' and @Choice4 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice4,0)
	if @Choice5<>'' and @Choice5 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice5,0)
	if @Choice6<>'' and @Choice6 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice6,0)
	if @Choice7<>'' and @Choice7 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice7,0)
	if @Choice8<>'' and @Choice8 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice8,0)
	if @Choice9<>'' and @Choice9 is not null
		insert into yaf_Choice(PollID,Choice,Votes)
		values(@PollID,@Choice9,0)
	select PollID = @PollID
end
GO

CREATE PROCEDURE [dbo].[yaf_poll_stats](@PollID int) AS
BEGIN
	SELECT
		a.PollID,
		b.Question,
		b.Closes,
		a.ChoiceID,
		a.Choice,
		a.Votes,
		Stats = (select 100 * a.Votes / case sum(x.Votes) when 0 then 1 else sum(x.Votes) end from yaf_Choice x where x.PollID=a.PollID)
	FROM
		yaf_Choice a,
		yaf_Poll b
	WHERE
		b.PollID = a.PollID AND
		b.PollID = @PollID
END
GO

CREATE PROCEDURE [dbo].[yaf_pollvote_check](@PollID int, @UserID int = NULL,@RemoteIP nvarchar(10) = NULL) AS

	IF @UserID IS NULL
	BEGIN
		IF @RemoteIP IS NOT NULL
		BEGIN
			-- check by remote IP
			SELECT PollVoteID FROM yaf_PollVote WHERE PollID = @PollID AND RemoteIP = @RemoteIP
		END
	END
	ELSE
	BEGIN
		-- check by userid or remote IP
		SELECT PollVoteID FROM yaf_PollVote WHERE PollID = @PollID AND (UserID = @UserID OR RemoteIP = @RemoteIP)
	END
GO

create procedure [dbo].[yaf_post_last10user](@BoardID int,@UserID int,@PageUserID int) as
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
		yaf_Message a
		join yaf_User b on b.UserID=a.UserID
		join yaf_Topic c on c.TopicID=a.TopicID
		join yaf_Forum d on d.ForumID=c.ForumID
		join yaf_Category e on e.CategoryID=d.CategoryID
		join yaf_vaccess x on x.ForumID=d.ForumID
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

create procedure [dbo].[yaf_post_list](@TopicID int,@UpdateViewCount smallint=1, @ShowDeleted bit = 1) as
begin
	set nocount on
	if @UpdateViewCount>0
		update yaf_Topic set Views = Views + 1 where TopicID = @TopicID
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
		HasAttachments	= (select count(1) from yaf_Attachment x where x.MessageID=a.MessageID),
		HasAvatarImage = (select count(1) from yaf_User x where x.UserID=b.UserID and AvatarImage is not null)
	from
		yaf_Message a
		join yaf_User b on b.UserID=a.UserID
		join yaf_Topic d on d.TopicID=a.TopicID
		join yaf_Forum g on g.ForumID=d.ForumID
		join yaf_Category h on h.CategoryID=g.CategoryID
		join yaf_Rank c on c.RankID=b.RankID
	where
		a.TopicID = @TopicID and
		((a.Flags & 24)=16 or ((a.Flags & 24)=24  and @showdeleted =1))
	order by
		a.Posted asc
end
GO

create procedure [dbo].[yaf_post_list_reverse10](@TopicID int) as
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
		yaf_Message a, 
		yaf_User b,
		yaf_Topic d
	where
		(a.Flags & 24)=16 and
		a.TopicID = @TopicID and
		b.UserID = a.UserID and
		d.TopicID = a.TopicID
	order by
		a.Posted desc
end
GO

create procedure [dbo].[yaf_rank_delete](@RankID int) as begin
	delete from yaf_Rank where RankID = @RankID
end
GO

create procedure [dbo].[yaf_rank_list](@BoardID int,@RankID int=null) as begin
	if @RankID is null
		select
			a.*
		from
			yaf_Rank a
		where
			a.BoardID=@BoardID
		order by
			a.MinPosts,
			a.Name
	else
		select
			a.*
		from
			yaf_Rank a
		where
			a.RankID = @RankID
end
GO

create procedure [dbo].[yaf_rank_save](
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
		update yaf_Rank set
			Name = @Name,
			Flags = @Flags,
			MinPosts = @MinPosts,
			RankImage = @RankImage
		where RankID = @RankID
	end
	else begin
		insert into yaf_Rank(BoardID,Name,Flags,MinPosts,RankImage)
		values(@BoardID,@Name,@Flags,@MinPosts,@RankImage);
	end
end
GO

create procedure [dbo].[yaf_registry_list](@Name nvarchar(50) = null,@BoardID int = null) as
BEGIN
	if @BoardID is null
	begin
		IF @Name IS NULL OR @Name = ''
		BEGIN
			SELECT * FROM yaf_Registry where BoardID is null
		END ELSE
		BEGIN
			SELECT * FROM yaf_Registry WHERE LOWER(Name) = LOWER(@Name) and BoardID is null
		END
	end else 
	begin
		IF @Name IS NULL OR @Name = ''
		BEGIN
			SELECT * FROM yaf_Registry where BoardID=@BoardID
		END ELSE
		BEGIN
			SELECT * FROM yaf_Registry WHERE LOWER(Name) = LOWER(@Name) and BoardID=@BoardID
		END
	end
END
GO

create procedure [dbo].[yaf_registry_save](
	@Name nvarchar(50),
	@Value ntext = NULL,
	@BoardID int = null
) AS
BEGIN
	if @BoardID is null
	begin
		if exists(select 1 from yaf_Registry where lower(Name)=lower(@Name))
			update yaf_Registry set Value = @Value where lower(Name)=lower(@Name) and BoardID is null
		else
		begin
			insert into yaf_Registry(Name,Value) values(lower(@Name),@Value)
		end
	end else
	begin
		if exists(select 1 from yaf_Registry where lower(Name)=lower(@Name) and BoardID=@BoardID)
			update yaf_Registry set Value = @Value where lower(Name)=lower(@Name) and BoardID=@BoardID
		else
		begin
			insert into yaf_Registry(Name,Value,BoardID) values(lower(@Name),@Value,@BoardID)
		end
	end
END
GO

create procedure [dbo].[yaf_replace_words_delete](@ID int) as
begin
	delete from dbo.yaf_replace_words where ID = @ID
end
GO

create procedure [dbo].[yaf_replace_words_edit](@ID int=null) as
begin
	select * from yaf_replace_words where ID=@ID
end
GO

create procedure [dbo].[yaf_replace_words_list] as begin
	select * from yaf_Replace_Words
end
GO

create procedure [dbo].[yaf_replace_words_save](@ID int=null,@badword nvarchar(255),@goodword nvarchar(255)) as
begin
	if @ID is null or @ID = 0 begin
		insert into yaf_replace_words(badword,goodword) values(@badword,@goodword)
	end
	else begin
		update yaf_replace_words set badword = @badword,goodword = @goodword where ID = @ID
	end
end
GO

create procedure [dbo].[yaf_smiley_delete](@SmileyID int=null) as begin
	if @SmileyID is not null
		delete from yaf_Smiley where SmileyID=@SmileyID
	else
		delete from yaf_Smiley
end
GO

create procedure [dbo].[yaf_smiley_list](@BoardID int,@SmileyID int=null) as
begin
	if @SmileyID is null
		select * from yaf_Smiley where BoardID=@BoardID order by SortOrder, LEN(Code) desc
	else
		select * from yaf_Smiley where SmileyID=@SmileyID order by SortOrder
end
GO

create procedure [dbo].[yaf_smiley_listunique](@BoardID int) as
begin
	select 
		Icon, 
		Emoticon,
		Code = (select top 1 Code from yaf_Smiley x where x.Icon=yaf_Smiley.Icon),
		SortOrder = (select top 1 SortOrder from yaf_Smiley x where x.Icon=yaf_Smiley.Icon order by x.SortOrder asc)
	from 
		yaf_Smiley
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

create procedure [dbo].[yaf_smiley_save](@SmileyID int=null,@BoardID int,@Code nvarchar(10),@Icon nvarchar(50),@Emoticon nvarchar(50),@SortOrder tinyint,@Replace smallint=0) as begin
	if @SmileyID is not null begin
		update yaf_Smiley set Code = @Code, Icon = @Icon, Emoticon = @Emoticon, SortOrder = @SortOrder where SmileyID = @SmileyID
	end
	else begin
		if @Replace>0
			delete from yaf_Smiley where Code=@Code

		if not exists(select 1 from yaf_Smiley where BoardID=@BoardID and Code=@Code)
			insert into yaf_Smiley(BoardID,Code,Icon,Emoticon,SortOrder) values(@BoardID,@Code,@Icon,@Emoticon,@SortOrder)
	end
end
GO

create procedure [dbo].[yaf_smiley_resort](@BoardID int,@SmileyID int,@Move int) as
begin
	declare @Position int

	SELECT @Position=SortOrder FROM yaf_Smiley WHERE BoardID=@BoardID and SmileyID=@SmileyID

	if (@Position is null) return

	if (@Move > 0) begin
		update yaf_Smiley
			set SortOrder=SortOrder-1
			where BoardID=@BoardID and 
				SortOrder between @Position and (@Position + @Move) and
				SortOrder between 1 and 255
	end
	else if (@Move < 0) begin
		update yaf_Smiley
			set SortOrder=SortOrder+1
			where BoardID=@BoardID and 
				SortOrder between (@Position+@Move) and @Position and
				SortOrder between 0 and 254
	end

	SET @Position = @Position + @Move

	if (@Position>255) SET @Position = 255
	else if (@Position<0) SET @Position = 0

	update yaf_Smiley
		set SortOrder=@Position
		where BoardID=@BoardID and 
			SmileyID=@SmileyID
end
GO

create procedure [dbo].[yaf_system_initialize](
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
	EXEC yaf_registry_save 'Version','1'
	EXEC yaf_registry_save 'VersionName','1.0.0'
	SET @tmpValue = CAST(@TimeZone AS nvarchar(100))
	EXEC yaf_registry_save 'TimeZone', @tmpValue
	EXEC yaf_registry_save 'SmtpServer', @SmtpServer
	EXEC yaf_registry_save 'ForumEmail', @ForumEmail

	-- initalize new board
	EXEC yaf_board_create @Name,0,@User,@UserEmail,@Password,1
end
GO

CREATE PROCEDURE [dbo].[yaf_system_updateversion]
(
	@Version		int,
	@VersionName	nvarchar(50)
) 
AS
BEGIN

	DECLARE @tmpValue AS nvarchar(100)
	SET @tmpValue = CAST(@Version AS nvarchar(100))
	EXEC yaf_registry_save 'Version', @tmpValue
	EXEC yaf_registry_save 'VersionName',@VersionName

END
GO

create procedure [dbo].[yaf_topic_active](@BoardID int,@UserID int,@Since datetime,@CategoryID int=null) as
begin
	select
		c.ForumID,
		c.TopicID,
		c.Posted,
		LinkTopicID = IsNull(c.TopicMovedID,c.TopicID),
		Subject = c.Topic,
		c.UserID,
		Starter = IsNull(c.UserName,b.Name),
		Replies = (select count(1) from yaf_Message x where x.TopicID=c.TopicID and (x.Flags & 8)=0) - 1,
		Views = c.Views,
		LastPosted = c.LastPosted,
		LastUserID = c.LastUserID,
		LastUserName = IsNull(c.LastUserName,(select Name from yaf_User x where x.UserID=c.LastUserID)),
		LastMessageID = c.LastMessageID,
		LastTopicID = c.TopicID,
		TopicFlags = c.Flags,
		c.Priority,
		c.PollID,
		ForumName = d.Name,
		c.TopicMovedID,
		ForumFlags = d.Flags
	from
		yaf_Topic c
		join yaf_User b on b.UserID=c.UserID
		join yaf_Forum d on d.ForumID=c.ForumID
		join yaf_vaccess x on x.ForumID=d.ForumID
		join yaf_Category e on e.CategoryID=d.CategoryID
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

create procedure [dbo].[yaf_topic_delete] (@TopicID int,@UpdateLastPost bit=1,@EraseTopic bit=0) 
as
begin
	SET NOCOUNT ON
	declare @ForumID int
	declare @pollID int
	
	select @ForumID=ForumID from yaf_Topic where TopicID=@TopicID
	update yaf_Topic set LastMessageID = null where TopicID = @TopicID
	update yaf_Forum set 
		LastTopicID = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null,
		LastPosted = null
	where LastMessageID in (select MessageID from yaf_Message where TopicID = @TopicID)
	update yaf_Active set TopicID = null where TopicID = @TopicID
	
	--remove polls	
	select @pollID = pollID from yaf_topic where TopicID = @TopicID
	if (@pollID is not null)
	begin
		delete from yaf_choice where PollID = @PollID
		update yaf_topic set PollID = null where TopicID = @TopicID
		delete from yaf_poll where PollID = @PollID	
	end	
	
	--delete messages and topics
	delete from yaf_nntptopic where TopicID = @TopicID
	delete from yaf_topic where TopicMovedID = @TopicID

	if @EraseTopic = 0
	begin
		update yaf_topic set Flags = Flags | 8 where TopicID = @TopicID
	end
	else
	begin
		delete from yaf_attachment where MessageID IN (select MessageID from yaf_message where TopicID = @TopicID) 
		delete from yaf_message where TopicID = @TopicID
		delete from yaf_topic where TopicID = @TopicID
	end
		
	--commit
	if @UpdateLastPost<>0
		exec yaf_forum_updatelastpost @ForumID
	
	if @ForumID is not null
		exec yaf_forum_updatestats @ForumID
end

GO

create procedure [dbo].[yaf_topic_findnext](@TopicID int) as
begin
	declare @LastPosted datetime
	declare @ForumID int
	select @LastPosted = LastPosted, @ForumID = ForumID from yaf_Topic where TopicID = @TopicID
	select top 1 TopicID from yaf_Topic where LastPosted>@LastPosted and ForumID = @ForumID AND (Flags & 8) = 0 order by LastPosted asc
end
GO

create procedure [dbo].[yaf_topic_findprev](@TopicID int) AS 
BEGIN
	DECLARE @LastPosted datetime
	DECLARE @ForumID int
	SELECT @LastPosted = LastPosted, @ForumID = ForumID FROM yaf_Topic WHERE TopicID = @TopicID
	SELECT TOP 1 TopicID from yaf_Topic where LastPosted<@LastPosted AND ForumID = @ForumID AND (Flags & 8) = 0 ORDER BY LastPosted DESC
END
GO

CREATE PROCEDURE [dbo].[yaf_topic_info]
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
			SELECT * FROM yaf_Topic
		ELSE
			SELECT * FROM yaf_Topic WHERE (Flags & 8) = 0
	END
	ELSE
	BEGIN
		IF @ShowDeleted = 1 
			SELECT * FROM yaf_Topic WHERE TopicID = @TopicID
		ELSE
			SELECT * FROM yaf_Topic WHERE TopicID = @TopicID AND (Flags & 8) = 0		
	END
END
GO


CREATE PROCEDURE [dbo].[yaf_topic_announcements]
(
	@BoardID int,
	@NumPosts int,
	@UserID int
)
AS
BEGIN

	DECLARE @SQL nvarchar(500)

	SET @SQL = 'SELECT DISTINCT TOP ' + convert(varchar, @NumPosts) + ' t.Topic, t.LastPosted, t.TopicID, t.LastMessageID FROM'
	SET @SQL = @SQL + ' yaf_Topic t INNER JOIN yaf_Category c INNER JOIN yaf_Forum f ON c.CategoryID = f.CategoryID ON t.ForumID = f.ForumID'
	SET @SQL = @SQL + ' join yaf_vaccess v on v.ForumID=f.ForumID'
	SET @SQL = @SQL + ' WHERE c.BoardID = ' + convert(varchar, @BoardID) + ' AND v.UserID=' + convert(varchar,@UserID) + ' AND (v.ReadAccess <> 0 or (f.Flags & 2) = 0) AND (t.Flags & 8) != 8 AND (t.Priority = 2) ORDER BY t.LastPosted DESC'

	EXEC(@SQL)	

END
GO


CREATE PROCEDURE [dbo].[yaf_topic_latest]
(
	@BoardID int,
	@NumPosts int,
	@UserID int
)
AS
BEGIN
	SET ROWCOUNT @NumPosts
	
	SELECT t.LastPosted, t.Topic, t.TopicID, t.LastMessageID FROM yaf_Topic t
	INNER JOIN yaf_Category c
	INNER JOIN yaf_Forum f
	ON c.CategoryID = f.CategoryID
	ON t.ForumID = f.ForumID
	JOIN yaf_vaccess v
	ON v.ForumID=f.ForumID
	WHERE c.BoardID = @BoardID AND v.UserID=@UserID AND (v.ReadAccess <> 0) AND (t.Flags & 8) = 0 ORDER BY t.LastPosted DESC;

END
GO

CREATE procedure [dbo].[yaf_topic_list](@ForumID int,@Announcement smallint,@Date datetime=null,@Offset int,@Count int) as
begin
	create table #data(
		RowNo	int identity primary key not null,
		TopicID	int not null
	)

	insert into #data(TopicID)
	select
		c.TopicID
	from
		yaf_Topic c join yaf_User b on b.UserID=c.UserID join yaf_Forum d on d.ForumID=c.ForumID
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
		Subject = c.Topic,
		c.UserID,
		Starter = IsNull(c.UserName,b.Name),
		Replies = c.NumPosts - 1,
		Views = c.Views,
		LastPosted = c.LastPosted,
		LastUserID = c.LastUserID,
		LastUserName = IsNull(c.LastUserName,(select Name from yaf_User x where x.UserID=c.LastUserID)),
		LastMessageID = c.LastMessageID,
		LastTopicID = c.TopicID,
		TopicFlags = c.Flags,
		c.Priority,
		c.PollID,
		ForumFlags = d.Flags
	from
		yaf_Topic c 
		join yaf_User b on b.UserID=c.UserID 
		join yaf_Forum d on d.ForumID=c.ForumID 
		join #data e on e.TopicID=c.TopicID
	where
		e.RowNo between @Offset+1 and @Offset + @Count
	order by
		e.RowNo
end
GO

create procedure [dbo].[yaf_topic_listmessages](@TopicID int) as
begin
	select * from yaf_Message
	where TopicID = @TopicID
end
GO

create procedure [dbo].[yaf_topic_lock](@TopicID int,@Locked bit) as
begin
	if @Locked<>0
		update yaf_Topic set Flags = Flags | 1 where TopicID = @TopicID
	else
		update yaf_Topic set Flags = Flags & ~1 where TopicID = @TopicID
end
GO

CREATE procedure [dbo].[yaf_topic_move](@TopicID int,@ForumID int,@ShowMoved bit) AS
begin
    declare @OldForumID int

    select @OldForumID = ForumID from yaf_Topic where TopicID = @TopicID

    if @ShowMoved<>0 begin
        -- create a moved message
        insert into yaf_Topic(ForumID,UserID,UserName,Posted,Topic,Views,Flags,Priority,PollID,TopicMovedID,LastPosted,NumPosts)
        select ForumID,UserID,UserName,Posted,Topic,0,Flags,Priority,PollID,@TopicID,LastPosted,0
        from yaf_Topic where TopicID = @TopicID
    end

    -- move the topic
    update yaf_Topic set ForumID = @ForumID where TopicID = @TopicID

    -- update last posts
    exec yaf_forum_updatelastpost @OldForumID
    exec yaf_forum_updatelastpost @ForumID
    
    -- update stats
    exec yaf_forum_updatestats @OldForumID
    exec yaf_forum_updatestats @ForumID
    
end
GO

create procedure [dbo].[yaf_topic_prune](@ForumID int=null,@Days int) as
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
			yaf_Topic
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
			yaf_Topic
		where 
			Priority = 0 and
			(Flags & 512) = 0 and					/* not flagged as persistent */
			datediff(dd,LastPosted,getdate())>@Days
	end
	open @c
	fetch @c into @TopicID
	while @@FETCH_STATUS=0 begin
		exec yaf_topic_delete @TopicID,0
		set @Count = @Count + 1
		fetch @c into @TopicID
	end
	close @c
	deallocate @c

	-- This takes forever with many posts...
	--exec yaf_topic_updatelastpost

	select Count = @Count
end
GO

create procedure [dbo].[yaf_topic_save](
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
	insert into yaf_Topic(ForumID,Topic,UserID,Posted,Views,Priority,PollID,UserName,NumPosts)
	values(@ForumID,@Subject,@UserID,@Posted,0,@Priority,@PollID,@UserName,0)

	-- get its id
	set @TopicID = SCOPE_IDENTITY()
	
	-- add message to the topic
	exec yaf_message_save @TopicID,@UserID,@Message,@UserName,@IP,@Posted,null,@BlogPostID,@Flags,@MessageID output

	select TopicID = @TopicID, MessageID = @MessageID
end
GO

CREATE procedure [dbo].[yaf_topic_updatelastpost]
(@ForumID int=null,@TopicID int=null) as
begin

    if @TopicID is not null
        update yaf_Topic set
            LastPosted = (select top 1 x.Posted from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastMessageID = (select top 1 x.MessageID from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserID = (select top 1 x.UserID from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserName = (select top 1 x.UserName from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc)
        where TopicID = @TopicID
    else
        update yaf_Topic set
            LastPosted = (select top 1 x.Posted from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastMessageID = (select top 1 x.MessageID from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserID = (select top 1 x.UserID from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc),
            LastUserName = (select top 1 x.UserName from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16 order by Posted desc)
        where TopicMovedID is null
        and (@ForumID is null or ForumID=@ForumID)

    exec yaf_forum_updatelastpost @ForumID
end
GO

create procedure [dbo].[yaf_user_accessmasks](@BoardID int,@UserID int) as
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
			yaf_User a 
			join yaf_UserGroup b on b.UserID=a.UserID
			join yaf_Group c on c.GroupID=b.GroupID
			join yaf_ForumAccess d on d.GroupID=c.GroupID
			join yaf_AccessMask e on e.AccessMaskID=d.AccessMaskID
			join yaf_Forum f on f.ForumID=d.ForumID
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
			yaf_User a 
			join yaf_UserForum b on b.UserID=a.UserID
			join yaf_AccessMask c on c.AccessMaskID=b.AccessMaskID
			join yaf_Forum d on d.ForumID=b.ForumID
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

CREATE procedure [dbo].[yaf_user_activity_rank](@StartDate as datetime) AS
begin
	select top 3  ID, Name, NumOfPosts from yaf_User u inner join
	(
		select m.UserID as ID, Count(m.UserID) as NumOfPosts from yaf_Message m
		where m.Posted >= @StartDate
		group by m.UserID
	) as counter
	on u.UserID = counter.ID
	order by NumOfPosts desc
end
go

create PROCEDURE [dbo].[yaf_user_addpoints] (@UserID int,@Points int) AS
BEGIN
	UPDATE yaf_User SET Points = Points + @Points WHERE UserID = @UserID
END

GO

create procedure [dbo].[yaf_user_adminsave](@BoardID int,@UserID int,@Name nvarchar(50),@Email nvarchar(50),@IsHostAdmin bit,@IsGuest bit,@RankID int) as
begin
	if @IsHostAdmin<>0
		update yaf_User set Flags = Flags | 1 where UserID = @UserID
	else
		update yaf_User set Flags = Flags & ~1 where UserID = @UserID

	if @IsGuest<>0
		update yaf_User set Flags = Flags | 4 where UserID = @UserID
	else
		update yaf_User set Flags = Flags & ~4 where UserID = @UserID

	update yaf_User set
		Name = @Name,
		Email = @Email,
		RankID = @RankID
	where UserID = @UserID
	select UserID = @UserID
end
GO

create procedure [dbo].[yaf_user_approve](@UserID int) as
begin
	declare @CheckEmailID int
	declare @Email nvarchar(50)

	select 
		@CheckEmailID = CheckEmailID,
		@Email = Email
	from
		yaf_CheckEmail
	where
		UserID = @UserID

	-- Update new user email
	update yaf_User set Email = @Email, Flags = Flags | 2 where UserID = @UserID
	delete yaf_CheckEmail where CheckEmailID = @CheckEmailID
	select convert(bit,1)
end
GO

CREATE procedure [dbo].[yaf_user_approveall](@BoardID int) as
begin

	DECLARE userslist CURSOR FOR 
		SELECT UserID FROM yaf_User WHERE BoardID=@BoardID AND (Flags & 2)=0
		FOR READ ONLY


	OPEN userslist

	DECLARE @UserID int

	FETCH userslist INTO @UserID

	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXEC yaf_user_approve @UserID
		FETCH userslist INTO @UserID		
	END

	CLOSE userslist

end
GO

CREATE procedure [dbo].[yaf_user_aspnet](@BoardID int,@UserName nvarchar(50),@Email nvarchar(50),@ProviderUserKey uniqueidentifier,@IsApproved bit) as
begin
	declare @UserID int, @RankID int, @approvedFlag int

	SET @approvedFlag = 0;
	IF (@IsApproved = 1) SET @approvedFlag = 2;	
	
	if exists(select 1 from dbo.yaf_User where BoardID=@BoardID and [Name]=@UserName)
	begin
		select @UserID=UserID from dbo.yaf_User where BoardID=@BoardID and [Name]=@UserName
		update dbo.yaf_User set 
			[Name] = @UserName,
			Email = @Email,
			ProviderUserKey = @ProviderUserKey,
			Flags = Flags | @approvedFlag
		where
			UserID = @UserID
	end else
	begin
		select @RankID = RankID from dbo.yaf_Rank where (Flags & 1)<>0 and BoardID=@BoardID

		insert into dbo.yaf_User(BoardID,RankID,[Name],Password,Email,Joined,LastVisit,NumPosts,TimeZone,Flags,ProviderUserKey) 
		values(@BoardID,@RankID,@UserName,'-',@Email,getdate(),getdate(),0,0,@approvedFlag,@ProviderUserKey)
	
		set @UserID = SCOPE_IDENTITY()
	
	end
	
	select UserID=@UserID
end
GO

create procedure [dbo].[yaf_user_avatarimage](@UserID int) as begin
	select UserID,AvatarImage from yaf_User where UserID=@UserID
end
GO

create procedure [dbo].[yaf_user_changepassword](@UserID int,@OldPassword nvarchar(32),@NewPassword nvarchar(32)) as
begin
	declare @CurrentOld nvarchar(32)
	select @CurrentOld = Password from yaf_User where UserID = @UserID
	if @CurrentOld<>@OldPassword begin
		select Success = convert(bit,0)
		return
	end
	update yaf_User set Password = @NewPassword where UserID = @UserID
	select Success = convert(bit,1)
end
GO

create procedure [dbo].[yaf_user_delete](@UserID int) as
begin
	declare @GuestUserID	int
	declare @UserName		nvarchar(50)
	declare @GuestCount		int

	select @UserName = Name from yaf_User where UserID=@UserID

	select top 1
		@GuestUserID = a.UserID
	from
		yaf_User a,
		yaf_UserGroup b,
		yaf_Group c
	where
		b.UserID = a.UserID and
		b.GroupID = c.GroupID and
		(c.Flags & 2)<>0

	select 
		@GuestCount = count(1) 
	from 
		yaf_UserGroup a
		join yaf_Group b on b.GroupID=a.GroupID
	where
		(b.Flags & 2)<>0

	if @GuestUserID=@UserID and @GuestCount=1 begin
		return
	end

	update yaf_Message set UserName=@UserName,UserID=@GuestUserID where UserID=@UserID
	update yaf_Topic set UserName=@UserName,UserID=@GuestUserID where UserID=@UserID
	update yaf_Topic set LastUserName=@UserName,LastUserID=@GuestUserID where LastUserID=@UserID
	update yaf_Forum set LastUserName=@UserName,LastUserID=@GuestUserID where LastUserID=@UserID

	delete from yaf_EventLog where UserID=@UserID	
	delete from yaf_UserPMessage where UserID=@UserID
	delete from yaf_PMessage where FromUserID=@UserID AND PMessageID NOT IN (select PMessageID FROM yaf_PMessage)
	-- set messages as from guest so the User can be deleted
	update yaf_PMessage SET FromUserID = @GuestUserID WHERE FromUserID = @UserID
	delete from yaf_CheckEmail where UserID = @UserID
	delete from yaf_WatchTopic where UserID = @UserID
	delete from yaf_WatchForum where UserID = @UserID
	delete from yaf_UserGroup where UserID = @UserID
	--ABOT CHANGED
	--Delete UserForums entries Too 
	delete from yaf_UserForum where UserID = @UserID
	--END ABOT CHANGED 09.04.2004
	delete from yaf_User where UserID = @UserID
end
GO

CREATE procedure [dbo].[yaf_user_deleteavatar](@UserID int) as begin
	update yaf_User set AvatarImage = null, Avatar = null where UserID = @UserID
end
GO

create procedure [dbo].[yaf_user_deleteold](@BoardID int) as
begin
	declare @Since datetime

	set @Since = getdate()

	delete from yaf_EventLog  where UserID in(select UserID from yaf_User where BoardID=@BoardID and dbo.yaf_bitset(Flags,2)=0 and datediff(day,Joined,@Since)>2)
	delete from yaf_CheckEmail where UserID in(select UserID from yaf_User where BoardID=@BoardID and dbo.yaf_bitset(Flags,2)=0 and datediff(day,Joined,@Since)>2)
	delete from yaf_UserGroup where UserID in(select UserID from yaf_User where BoardID=@BoardID and dbo.yaf_bitset(Flags,2)=0 and datediff(day,Joined,@Since)>2)
	delete from yaf_User where BoardID=@BoardID and dbo.yaf_bitset(Flags,2)=0 and datediff(day,Joined,@Since)>2
end
GO

create procedure [dbo].[yaf_user_emails](@BoardID int,@GroupID int=null) as
begin
	if @GroupID = 0 set @GroupID = null
	if @GroupID is null
		select 
			a.Email 
		from 
			yaf_User a
		where 
			a.Email is not null and 
			a.BoardID = @BoardID and
			a.Email is not null and 
			a.Email<>''
	else
		select 
			a.Email 
		from 
			yaf_User a join yaf_UserGroup b on b.UserID=a.UserID
		where 
			b.GroupID = @GroupID and 
			a.Email is not null and 
			a.Email<>''
end
GO

create procedure [dbo].[yaf_user_find](@BoardID int,@Filter bit,@UserName nvarchar(50)=null,@Email nvarchar(50)=null) as
begin
	if @Filter<>0
	begin
		if @UserName is not null
			set @UserName = '%' + @UserName + '%'

		select 
			a.*,
			IsGuest = (select count(1) from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and x.GroupID=y.GroupID and (y.Flags & 2)<>0)
		from 
			yaf_User a
		where 
			a.BoardID=@BoardID and
			(@UserName is not null and a.Name like @UserName) or (@Email is not null and Email like @Email)
		order by
			a.Name
	end else
	begin
		select 
			a.UserID,
			IsGuest = (select count(1) from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and x.GroupID=y.GroupID and (y.Flags & 2)<>0)
		from 
			yaf_User a
		where 
			a.BoardID=@BoardID and
			((@UserName is not null and a.Name=@UserName) or (@Email is not null and Email=@Email))
	end
end
GO

CREATE PROCEDURE [dbo].[yaf_user_getpoints] (@UserID int) AS
BEGIN
	SELECT Points FROM yaf_User WHERE UserID = @UserID
END
GO

create procedure [dbo].[yaf_user_getsignature](@UserID int) as
begin
	select Signature from yaf_User where UserID = @UserID
end
GO

create procedure [dbo].[yaf_user_guest]
(
	@BoardID int
)
as
begin
	select top 1
		a.UserID
	from
		yaf_User a,
		yaf_UserGroup b,
		yaf_Group c
	where
		b.UserID = a.UserID and
		b.GroupID = c.GroupID and
		a.BoardID = @BoardID and
		(c.Flags & 2)<>0
end
GO

create procedure [dbo].[yaf_user_list](@BoardID int,@UserID int=null,@Approved bit=null,@GroupID int=null,@RankID int=null) as
begin
	if @UserID is not null
		select 
			a.*,
			a.NumPosts,
			b.RankID,
			RankName = b.Name,
			NumDays = datediff(d,a.Joined,getdate())+1,
			NumPostsForum = (select count(1) from yaf_Message x where (x.Flags & 24)=16),
			HasAvatarImage = (select count(1) from yaf_User x where x.UserID=a.UserID and AvatarImage is not null),
			IsAdmin	= IsNull(c.IsAdmin,0),
			IsGuest	= IsNull(a.Flags & 4,0),
			IsHostAdmin	= IsNull(a.Flags & 1,0),
			IsForumModerator	= IsNull(c.IsForumModerator,0),
			IsModerator		= IsNull(c.IsModerator,0)
		from 
			yaf_User a
			join yaf_Rank b on b.RankID=a.RankID
			left join yaf_vaccess c on c.UserID=a.UserID
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
			IsAdmin = (select count(1) from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and (y.Flags & 1)<>0),
			IsGuest	= IsNull(a.Flags & 4,0),
			IsHostAdmin	= IsNull(a.Flags & 1,0),
			b.RankID,
			RankName = b.Name
		from 
			yaf_User a
			join yaf_Rank b on b.RankID=a.RankID
		where 
			a.BoardID = @BoardID and
			(@Approved is null or (@Approved=0 and (a.Flags & 2)=0) or (@Approved=1 and (a.Flags & 2)=2))
		order by 
			a.Name
	else
		select 
			a.*,
			a.NumPosts,
			IsAdmin = (select count(1) from yaf_UserGroup x,yaf_Group y where x.UserID=a.UserID and y.GroupID=x.GroupID and (y.Flags & 1)<>0),
			IsGuest	= IsNull(a.Flags & 4,0),
			IsHostAdmin	= IsNull(a.Flags & 1,0),
			b.RankID,
			RankName = b.Name
		from 
			yaf_User a
			join yaf_Rank b on b.RankID=a.RankID
		where 
			a.BoardID = @BoardID and
			(@Approved is null or (@Approved=0 and (a.Flags & 2)=0) or (@Approved=1 and (a.Flags & 2)=2)) and
			(@GroupID is null or exists(select 1 from yaf_UserGroup x where x.UserID=a.UserID and x.GroupID=@GroupID)) and
			(@RankID is null or a.RankID=@RankID)
		order by 
			a.Name
end
GO

create procedure [dbo].[yaf_user_login](@BoardID int,@Name nvarchar(50),@Password nvarchar(32)) as
begin
	declare @UserID int

	-- Try correct board first
	if exists(select UserID from yaf_User where Name=@Name and Password=@Password and BoardID=@BoardID and (Flags & 2)=2)
	begin
		select UserID from yaf_User where Name=@Name and Password=@Password and BoardID=@BoardID and (Flags & 2)=2
		return
	end

	if not exists(select UserID from yaf_User where Name=@Name and Password=@Password and (BoardID=@BoardID or (Flags & 3)=3))
		set @UserID=null
	else
		select 
			@UserID=UserID 
		from 
			yaf_User 
		where 
			Name=@Name and 
			Password=@Password and 
			(BoardID=@BoardID or (Flags & 1)=1) and
			(Flags & 2)=2

	select @UserID
end
GO

create procedure [dbo].[yaf_user_nntp](@BoardID int,@UserName nvarchar(50),@Email nvarchar(50)) as
begin
	declare @UserID int

	set @UserName = @UserName + ' (NNTP)'

	select
		@UserID=UserID
	from
		yaf_User
	where
		BoardID=@BoardID and
		Name=@UserName

	if @@ROWCOUNT<1
	begin
		exec yaf_user_save 0,@BoardID,@UserName,@Email,null,'Usenet',0,null,null,null,0,1,null,null,null,null,null,null,null,0,null,null,null,null,null
		-- The next one is not safe, but this procedure is only used for testing
		select @UserID=max(UserID) from yaf_User
	end

	select UserID=@UserID
end
GO

create procedure [dbo].[yaf_user_recoverpassword](@BoardID int,@UserName nvarchar(50),@Email nvarchar(50)) as
begin
	declare @UserID int
	select @UserID = UserID from yaf_User where BoardID = @BoardID and Name = @UserName and Email = @Email
	if @UserID is null begin
		select UserID = convert(int,null)
		return
	end else
	begin
		select UserID = @UserID
	end
end
GO

CREATE PROCEDURE [dbo].[yaf_user_removepoints] (@UserID int,@Points int) AS
BEGIN
	UPDATE yaf_User SET Points = Points - @Points WHERE UserID = @UserID
END
GO

CREATE PROCEDURE [dbo].[yaf_user_removepointsbytopicid] (@TopicID int,@Points int) AS
BEGIN
	declare @UserID int
	select @UserID = UserID from yaf_Topic where TopicID = @TopicID
	update yaf_user SET points = points - @Points WHERE userid = @UserID
END
GO

CREATE PROCEDURE [dbo].[yaf_user_resetpoints] AS
BEGIN
	UPDATE yaf_User SET Points = NumPosts * 3
END
GO

CREATE procedure [dbo].[yaf_user_save](
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
		
		select @RankID = RankID from yaf_Rank where (Flags & 1)<>0 and BoardID=@BoardID

		insert into yaf_User(BoardID,RankID,Name,Password,Email,Joined,LastVisit,NumPosts,TimeZone,Flags,PMNotification,ProviderUserKey) 
		values(@BoardID,@RankID,@UserName,'-',@Email,getdate(),getdate(),0,@TimeZone,@Flags,@PMNotification,@ProviderUserKey)		
	
		set @UserID = SCOPE_IDENTITY()

		insert into yaf_UserGroup(UserID,GroupID) select @UserID,GroupID from yaf_Group where BoardID=@BoardID and (Flags & 4)<>0
	end
	else begin
		update yaf_User set
			TimeZone = @TimeZone,
			LanguageFile = @LanguageFile,
			ThemeFile = @ThemeFile,
			OverrideDefaultThemes = @OverrideDefaultTheme,
			PMNotification = @PMNotification
		where UserID = @UserID
		
		if @Email is not null
			update yaf_User set Email = @Email where UserID = @UserID
	end
end
GO

CREATE procedure [dbo].[yaf_user_saveavatar]
(
	@UserID int,
	@Avatar nvarchar(255) = NULL,
	@AvatarImage image = NULL
)
AS
BEGIN
	IF @Avatar IS NOT NULL 
	BEGIN
		UPDATE yaf_User SET Avatar = @Avatar, AvatarImage = null WHERE UserID = @UserID
	END
	ELSE IF @AvatarImage IS NOT NULL 
	BEGIN
		UPDATE yaf_User SET AvatarImage = @AvatarImage, Avatar = null WHERE UserID = @UserID
	END
END

GO

create procedure [dbo].[yaf_user_savepassword](@UserID int,@Password nvarchar(32)) as
begin
	update dbo.yaf_User set Password = @Password where UserID = @UserID
end
GO

create procedure [dbo].[yaf_user_savesignature](@UserID int,@Signature ntext) as
begin
	update yaf_User set Signature = @Signature where UserID = @UserID
end
GO

CREATE PROCEDURE [dbo].[yaf_user_setpoints] (@UserID int,@Points int) AS
BEGIN
	UPDATE yaf_User SET Points = @Points WHERE UserID = @UserID
END
GO

create procedure [dbo].[yaf_user_setrole](@BoardID int,@ProviderUserKey uniqueidentifier,@Role nvarchar(50)) as
begin
	declare @UserID int, @GroupID int
	
	select @UserID=UserID from dbo.yaf_User where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey

	if @Role is null
	begin
		delete from dbo.yaf_UserGroup where UserID=@UserID
	end else
	begin
		if not exists(select 1 from dbo.yaf_Group where BoardID=@BoardID and Name=@Role)
		begin
			insert into dbo.yaf_Group(Name,BoardID,Flags)
			values(@Role,@BoardID,0);
			set @GroupID = SCOPE_IDENTITY()

			insert into yaf_ForumAccess(GroupID,ForumID,AccessMaskID)
			select
				@GroupID,
				a.ForumID,
				min(a.AccessMaskID)
			from
				dbo.yaf_ForumAccess a
				join dbo.yaf_Group b on b.GroupID=a.GroupID
			where
				b.BoardID=@BoardID and
				(b.Flags & 4)=4
			group by
				a.ForumID
		end else
		begin
			select @GroupID = GroupID from dbo.yaf_Group where BoardID=@BoardID and Name=@Role
		end
		insert into dbo.yaf_UserGroup(UserID,GroupID) values(@UserID,@GroupID)
	end
end
GO

create procedure [dbo].[yaf_user_suspend](@UserID int,@Suspend datetime=null) as
begin
	update yaf_User set Suspended = @Suspend where UserID=@UserID
end
GO

create procedure [dbo].[yaf_user_upgrade](@UserID int) as
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
		yaf_User a,
		yaf_Rank b
	where
		a.UserID = @UserID and
		b.RankID = a.RankID
	
	-- If user isn't member of a ladder rank, exit
	if (@Flags & 2) = 0 return
	
	-- See if user got enough posts for next ladder group
	select top 1
		@RankID = RankID
	from
		yaf_Rank
	where
		(Flags & 2) = 2 and
		MinPosts <= @NumPosts and
		MinPosts > @MinPosts
	order by
		MinPosts
	if @@ROWCOUNT=1
		update yaf_User set RankID = @RankID where UserID = @UserID
end
GO

create procedure [dbo].[yaf_userforum_delete](@UserID int,@ForumID int) as
begin
	delete from yaf_UserForum where UserID=@UserID and ForumID=@ForumID
end
GO

create procedure [dbo].[yaf_userforum_list](@UserID int=null,@ForumID int=null) as 
begin
	select 
		a.*,
		b.AccessMaskID,
		b.Accepted,
		Access = c.Name
	from
		yaf_User a join yaf_UserForum b on b.UserID=a.UserID
		join yaf_AccessMask c on c.AccessMaskID=b.AccessMaskID
	where
		(@UserID is null or a.UserID=@UserID) and
		(@ForumID is null or b.ForumID=@ForumID)
	order by
		a.Name	
end
GO

create procedure [dbo].[yaf_userforum_save](@UserID int,@ForumID int,@AccessMaskID int) as
begin
	if exists(select 1 from yaf_UserForum where UserID=@UserID and ForumID=@ForumID)
		update yaf_UserForum set AccessMaskID=@AccessMaskID where UserID=@UserID and ForumID=@ForumID
	else
		insert into yaf_UserForum(UserID,ForumID,AccessMaskID,Invited,Accepted) values(@UserID,@ForumID,@AccessMaskID,getdate(),1)
end
GO

create procedure [dbo].[yaf_usergroup_list](@UserID int) as begin
	select 
		b.GroupID,
		b.Name
	from
		yaf_UserGroup a
		join yaf_Group b on b.GroupID=a.GroupID
	where
		a.UserID = @UserID
	order by
		b.Name
end
GO

create procedure [dbo].[yaf_usergroup_save](@UserID int,@GroupID int,@Member bit) as
begin
	if @Member=0
		delete from yaf_UserGroup where UserID=@UserID and GroupID=@GroupID
	else
		insert into yaf_UserGroup(UserID,GroupID)
		select @UserID,@GroupID
		where not exists(select 1 from yaf_UserGroup where UserID=@UserID and GroupID=@GroupID)
end
GO

create procedure [dbo].[yaf_userpmessage_delete](@UserPMessageID int) as
begin
	delete from yaf_UserPMessage where UserPMessageID=@UserPMessageID
end
GO

create procedure [dbo].[yaf_userpmessage_list](@UserPMessageID int) as
begin
	select
		a.*,
		FromUser = b.Name,
		ToUserID = c.UserID,
		ToUser = c.Name,
		d.IsRead,
		d.UserPMessageID
	from
		yaf_PMessage a,
		yaf_User b,
		yaf_User c,
		yaf_UserPMessage d
	where
		b.UserID = a.FromUserID and
		c.UserID = d.UserID and
		d.PMessageID = a.PMessageID and
		d.UserPMessageID = @UserPMessageID
end
GO

create procedure [dbo].[yaf_watchforum_add](@UserID int,@ForumID int) as
begin
	insert into yaf_WatchForum(ForumID,UserID,Created)
	select @ForumID, @UserID, getdate()
	where not exists(select 1 from yaf_WatchForum where ForumID=@ForumID and UserID=@UserID)
end
GO

create procedure [dbo].[yaf_watchforum_check](@UserID int,@ForumID int) as
begin
	SELECT WatchForumID FROM yaf_WatchForum WHERE UserID = @UserID AND ForumID = @ForumID
end
GO

create procedure [dbo].[yaf_watchforum_delete](@WatchForumID int) as
begin
	delete from yaf_WatchForum where WatchForumID = @WatchForumID
end
GO

create procedure [dbo].[yaf_watchforum_list](@UserID int) as
begin
	select
		a.*,
		ForumName = b.Name,
		Messages = (select count(1) from yaf_Topic x, yaf_Message y where x.ForumID=a.ForumID and y.TopicID=x.TopicID),
		Topics = (select count(1) from yaf_Topic x where x.ForumID=a.ForumID and x.TopicMovedID is null),
		b.LastPosted,
		b.LastMessageID,
		LastTopicID = (select TopicID from yaf_Message x where x.MessageID=b.LastMessageID),
		b.LastUserID,
		LastUserName = IsNull(b.LastUserName,(select Name from yaf_User x where x.UserID=b.LastUserID))
	from
		yaf_WatchForum a,
		yaf_Forum b
	where
		a.UserID = @UserID and
		b.ForumID = a.ForumID
end
GO

create procedure [dbo].[yaf_watchtopic_add](@UserID int,@TopicID int) as
begin
	insert into yaf_WatchTopic(TopicID,UserID,Created)
	select @TopicID, @UserID, getdate()
	where not exists(select 1 from yaf_WatchTopic where TopicID=@TopicID and UserID=@UserID)
end
GO

create procedure [dbo].[yaf_watchtopic_check](@UserID int,@TopicID int) as
begin
	SELECT WatchTopicID FROM yaf_WatchTopic WHERE UserID = @UserID AND TopicID = @TopicID
end
GO

create procedure [dbo].[yaf_watchtopic_delete](@WatchTopicID int) as
begin
	delete from yaf_WatchTopic where WatchTopicID = @WatchTopicID
end
GO

create procedure [dbo].[yaf_watchtopic_list](@UserID int) as
begin
	select
		a.*,
		TopicName = b.Topic,
		Replies = (select count(1) from yaf_Message x where x.TopicID=b.TopicID),
		b.Views,
		b.LastPosted,
		b.LastMessageID,
		b.LastUserID,
		LastUserName = IsNull(b.LastUserName,(select Name from yaf_User x where x.UserID=b.LastUserID))
	from
		yaf_WatchTopic a,
		yaf_Topic b
	where
		a.UserID = @UserID and
		b.TopicID = a.TopicID
end
GO

CREATE procedure dbo.yaf_message_reply_list(@MessageID int) as
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
		yaf_Message a,
		yaf_User b,
		yaf_Topic c
	where
		(a.Flags & 16)=16 and
		b.UserID = a.UserID and
		c.TopicID = a.TopicID and
		a.ReplyTo = @MessageID



end
GO


CREATE procedure dbo.yaf_message_deleteundelete(@MessageID int, @isModeratorChanged bit, @DeleteReason nvarchar(100), @isDeleteAction int) as
begin
	declare @TopicID		int
	declare @ForumID		int
	declare @MessageCount	int
	declare @LastMessageID	int

	-- Find TopicID and ForumID
	select @TopicID=b.TopicID,@ForumID=b.ForumID from yaf_Message a,yaf_Topic b where a.MessageID=@MessageID and b.TopicID=a.TopicID

	-- Update LastMessageID in Topic and Forum
	update yaf_Topic set
		LastPosted = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	update yaf_Forum set
		LastPosted = null,
		LastTopicID = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	-- "Delete" message

        update yaf_Message set isModeratorChanged = @isModeratorChanged where MessageID = @MessageID and ((Flags & 8) <> @isDeleteAction*8)
        update yaf_Message set DeleteReason = @DeleteReason where MessageID = @MessageID and ((Flags & 8) <> @isDeleteAction*8)
        update yaf_Message set Flags = Flags ^ 8 where MessageID = @MessageID and ((Flags & 8) <> @isDeleteAction*8)

	-- Delete topic if there are no more messages
	select @MessageCount = count(1) from yaf_Message where TopicID = @TopicID and (Flags & 8)=0
	if @MessageCount=0 exec yaf_topic_delete @TopicID
	-- update lastpost
	exec yaf_topic_updatelastpost @ForumID,@TopicID
	exec yaf_forum_updatestats @ForumID
	-- update topic numposts
	update yaf_Topic set
		NumPosts = (select count(1) from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16)
	where TopicID = @TopicID
end
GO

create procedure [dbo].[yaf_topic_create_by_message] (
	@MessageID int,
	@ForumID	int,
	@Subject	nvarchar(100)
) as
begin


declare		@UserID		int
declare		@Posted		datetime

set @UserID = (select userid from yaf_message where messageid =  @MessageID)
set  @Posted  = (select  posted from yaf_message where messageid =  @MessageID)


	declare @TopicID int
	--declare @MessageID int

	if @Posted is null set @Posted = getdate()

	insert into yaf_Topic(ForumID,Topic,UserID,Posted,Views,Priority,PollID,UserName,NumPosts)
	values(@ForumID,@Subject,@UserID,@Posted,0,0,null,null,0)

	set @TopicID = @@IDENTITY
--	exec yaf_message_save @TopicID,@UserID,@Message,@UserName,@IP,@Posted,null,@Flags,@MessageID output
	select TopicID = @TopicID, MessageID = @MessageID
END
GO

CREATE PROCEDURE [dbo].[yaf_message_move] (@MessageID int, @MoveToTopic int) AS
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
--	select @OldTopicID=b.TopicID,@ForumID=b.ForumID from yaf_Message a,yaf_Topic b where a.MessageID=@MessageID and b.TopicID=a.TopicID

SET 	@NewForumID = (SELECT     ForumId
				FROM         yaf_Topic
				WHERE     (TopicId = @MoveToTopic))


SET 	@OldTopicID = 	(SELECT     TopicID
				FROM         yaf_Message
				WHERE     (MessageID = @MessageID))

SET 	@OldForumID = (SELECT     ForumId
				FROM         yaf_Topic
				WHERE     (TopicId = @OldTopicID))

SET	@ReplyToID = (SELECT     MessageID
			FROM         yaf_Message
			WHERE     ([Position] = 0) AND (TopicID = @MoveToTopic))

SET	@Position = 	(SELECT     MAX([Position]) + 1 AS Expr1
			FROM         yaf_Message
			WHERE     (TopicID = @MoveToTopic) and posted < (select posted from yaf_Message where messageid = @MessageID ) )

if @Position is null  set @Position = 0

update yaf_Message set
		Position = Position+1
	 WHERE     (TopicID = @MoveToTopic) and posted > (select posted from yaf_Message where messageid = @MessageID)

update yaf_Message set
		Position = Position-1
	 WHERE     (TopicID = @OldTopicID) and posted > (select posted from yaf_Message where messageid = @MessageID)

	-- Update LastMessageID in Topic and Forum
	update yaf_Topic set
		LastPosted = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID

	update yaf_Forum set
		LastPosted = null,
		LastTopicID = null,
		LastMessageID = null,
		LastUserID = null,
		LastUserName = null
	where LastMessageID = @MessageID


UPDATE yaf_Message SET
 	TopicID = @MoveToTopic,
	ReplyTo = @ReplyToID,
	[Position] = @Position
WHERE  MessageID = @MessageID

	-- Delete topic if there are no more messages
	select @MessageCount = count(1) from yaf_Message where TopicID = @OldTopicID and (Flags & 8)=0
	if @MessageCount=0 exec yaf_topic_delete @OldTopicID

	-- update lastpost
	exec yaf_topic_updatelastpost @OldForumID,@OldTopicID
	exec yaf_topic_updatelastpost @NewForumID,@MoveToTopic

	-- update topic numposts
	update yaf_Topic set
		NumPosts = (select count(1) from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16)
	where TopicID = @OldTopicID
	update yaf_Topic set
		NumPosts = (select count(1) from yaf_Message x where x.TopicID=yaf_Topic.TopicID and (x.Flags & 24)=16)
	where TopicID = @MoveToTopic

	exec yaf_forum_updatelastpost @NewForumID
	exec yaf_forum_updatestats @NewForumID
	exec yaf_forum_updatelastpost @OldForumID
	exec yaf_forum_updatestats @OldForumID

END
GO

create proc [dbo].[yaf_forum_resync]
	@BoardID int,
	@ForumID int = null
AS
begin
	if (@ForumID is null) begin
		declare curForums cursor for
			select 
				a.ForumID
			from
				yaf_Forum a
				JOIN yaf_Category b on a.CategoryID=b.CategoryID
				JOIN yaf_Board c on b.BoardID = c.BoardID  
			where
				c.BoardID=@BoardID

		open curForums
		
		-- cycle through forums
		fetch next from curForums into @ForumID
		while @@FETCH_STATUS = 0
		begin
			--update statistics
			exec dbo.yaf_forum_updatestats @ForumID
			--update last post
			exec dbo.yaf_forum_updatelastpost @ForumID

			fetch next from curForums into @ForumID
		end
		close curForums
		deallocate curForums
	end
	else begin
		--update statistics
		exec dbo.yaf_forum_updatestats @ForumID
		--update last post
		exec dbo.yaf_forum_updatelastpost @ForumID
	end
end
GO

create proc [dbo].[yaf_board_resync]
	@BoardID int = null
as
begin
	if (@BoardID is null) begin
		declare curBoards cursor for
			select BoardID from	yaf_Board

		open curBoards
		
		-- cycle through forums
		fetch next from curBoards into @BoardID
		while @@FETCH_STATUS = 0
		begin
			--resync board forums
			exec dbo.yaf_forum_resync @BoardID

			fetch next from curBoards into @BoardID
		end
		close curBoards
		deallocate curBoards
	end
	else begin
		--resync board forums
		exec dbo.yaf_forum_resync @BoardID
	end
end
GO

CREATE PROCEDURE [dbo].[yaf_category_simplelist](
                @StartID INT  = 0,
                @Limit   INT  = 500)
AS
    BEGIN
        SET ROWCOUNT  @Limit
        SELECT   c.[CategoryID],
                 c.[Name]
        FROM     yaf_Category c
        WHERE    c.[CategoryID] >= @StartID
        AND c.[CategoryID] < (@StartID + @Limit)
        ORDER BY c.[CategoryID]
        SET ROWCOUNT  0
    END
GO

CREATE PROCEDURE [dbo].[yaf_forum_simplelist](
                @StartID INT  = 0,
                @Limit   INT  = 500)
AS
    BEGIN
        SET ROWCOUNT  @Limit
        SELECT   f.[ForumID],
                 f.[Name]
        FROM     yaf_Forum f
        WHERE    f.[ForumID] >= @StartID
        AND f.[ForumID] < (@StartID + @Limit)
        ORDER BY f.[ForumID]
        SET ROWCOUNT  0
    END
GO

CREATE PROCEDURE [dbo].[yaf_message_simplelist](
                @StartID INT  = 0,
                @Limit   INT  = 1000)
AS
    BEGIN
        SET ROWCOUNT  @Limit
        SELECT   m.[MessageID],
                 m.[TopicID]
        FROM     yaf_Message m
        WHERE    m.[MessageID] >= @StartID
        AND m.[MessageID] < (@StartID + @Limit)
        AND m.[TopicID] IS NOT NULL
        ORDER BY m.[MessageID]
        SET ROWCOUNT  0
    END
GO

CREATE PROCEDURE [dbo].[yaf_topic_simplelist](
                @StartID INT  = 0,
                @Limit   INT  = 500)
AS
    BEGIN
        SET ROWCOUNT  @Limit
        SELECT   t.[TopicID],
                 t.[Topic]
        FROM     yaf_Topic t
        WHERE    t.[TopicID] >= @StartID
        AND t.[TopicID] < (@StartID + @Limit)
        ORDER BY t.[TopicID]
        SET ROWCOUNT  0
    END
GO

CREATE PROCEDURE [dbo].[yaf_user_simplelist](
                @StartID INT  = 0,
                @Limit   INT  = 500)
AS
    BEGIN
        SET ROWCOUNT  @Limit
        SELECT   a.[UserID],
                 a.[Name]
        FROM     yaf_User a
        WHERE    a.[UserID] >= @StartID
        AND a.[UserID] < (@StartID + @Limit)
        ORDER BY a.[UserID]
        SET ROWCOUNT  0
    END
GO