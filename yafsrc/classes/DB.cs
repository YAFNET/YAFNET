/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;

namespace yaf
{
	#region QueryCounter class
	public sealed class QueryCounter : System.IDisposable
	{
#if DEBUG
		private classes.HiPerfTimer	hiTimer				= new classes.HiPerfTimer(true);
		private string m_cmd;
#endif

		public QueryCounter(string sql) 
		{
#if DEBUG
			m_cmd = sql;

			if(HttpContext.Current.Items["NumQueries"]==null)
				HttpContext.Current.Items["NumQueries"] = (int)1;
			else
				HttpContext.Current.Items["NumQueries"] = 1 + (int)HttpContext.Current.Items["NumQueries"];
#endif
		}

		public void Dispose() 
		{
#if DEBUG
			hiTimer.Stop();

			m_cmd = String.Format("{0}: {1:N3}",m_cmd,hiTimer.Duration);

			if(HttpContext.Current.Items["TimeQueries"]==null)
				HttpContext.Current.Items["TimeQueries"] = hiTimer.Duration;
			else
				HttpContext.Current.Items["TimeQueries"] = hiTimer.Duration + (double)HttpContext.Current.Items["TimeQueries"];

			if(HttpContext.Current.Items["CmdQueries"]==null)
				HttpContext.Current.Items["CmdQueries"] = m_cmd;
			else
				HttpContext.Current.Items["CmdQueries"] += "<br/>" + m_cmd;
#endif
		}

#if DEBUG
		static public void Reset() 
		{
			HttpContext.Current.Items["NumQueries"] = 0;
			HttpContext.Current.Items["TimeQueries"] = (double)0;
			HttpContext.Current.Items["CmdQueries"] = "";
		}

		static public int Count 
		{
			get 
			{
				return (int)System.Web.HttpContext.Current.Items["NumQueries"];
			}
		}
		static public double Duration 
		{
			get 
			{
				return (double)System.Web.HttpContext.Current.Items["TimeQueries"];
			}
		}
		static public string Commands 
		{
			get 
			{
				return (string)System.Web.HttpContext.Current.Items["CmdQueries"];
			}
		}
#endif
	}
	#endregion

	public interface IDataProvider : IDisposable
	{
		#region DB Access Functions
		SqlConnection GetConnection2();
		void ExecuteNonQuery(SqlCommand cmd);
		int DBSize();
		#endregion
		
		#region Forum
		DataRow pageload(object SessionID,object boardID,object User,object IP,object Location,object Browser,
			object Platform,object CategoryID,object ForumID,object TopicID,object MessageID);
		DataTable GetSearchResult( string ToSearch, SEARCH_FIELD sf, SEARCH_WHAT sw, int fid, int UserID );
		#endregion

		#region DataSets
		DataSet board_layout(object boardID,object UserID,object CategoryID,object parentID);
		DataSet ds_forumadmin(object boardID);
		#endregion

		#region yaf_AccessMask
		DataTable accessmask_list(object boardID,object accessMaskID);
		bool accessmask_delete(object accessMaskID);
		void accessmask_save(object accessMaskID,object boardID,object name,object readAccess,object postAccess,object replyAccess,object priorityAccess,object pollAccess,object voteAccess,object moderatorAccess,object editAccess,object deleteAccess,object uploadAccess);
		#endregion

		#region yaf_Active
		DataTable active_list(object boardID,object Guests);
		DataTable active_listforum(object forumID);
		DataTable active_listtopic(object topicID); 
		DataRow active_stats(object boardID);
		#endregion

		#region yaf_Attachment
		DataTable attachment_list(object messageID,object attachmentID);
		void attachment_save(object messageID,object fileName,object bytes,object contentType,System.IO.Stream stream);
		void attachment_delete(object attachmentID);
		void attachment_download(object attachmentID);
		#endregion

		#region yaf_BannedIP
		DataTable bannedip_list(object boardID,object ID);
		void bannedip_save(object ID,object boardID,object Mask);
		void bannedip_delete(object ID);
		#endregion

		#region yaf_Board
		DataTable board_list(object boardID);
		DataRow board_poststats(object boardID);
		DataRow board_stats();
		void board_save(object boardID,object name,object allowThreaded);
		void board_create(object name,object allowThreaded,object userName,object userEmail,object userPass);
		void board_delete(object boardID);
		#endregion

		#region yaf_Category
		bool category_delete(object CategoryID);
		DataTable category_list(object boardID,object categoryID);
		void category_save(object boardID,object CategoryID,object Name,object SortOrder);
		#endregion

		#region yaf_CheckEmail
		void checkemail_save(object UserID,object Hash,object Email);
		bool checkemail_update(object hash);
		#endregion

		#region yaf_Choice
		void choice_vote(object choiceID);
		#endregion

		#region yaf_Forum
		void forum_delete(object ForumID);
		DataTable forum_list(object boardID,object ForumID);
		DataTable forum_listall(object boardID,object userID);
		DataTable forum_listpath(object forumID);
		DataTable forum_listread(object boardID,object UserID,object CategoryID,object parentID);
		DataTable forum_moderatelist();
		DataTable forum_moderators();
		long forum_save(object ForumID,object CategoryID,object parentID,object Name,object Description,object SortOrder,object Locked,object Hidden,object IsTest,object moderated,object accessMaskID,bool dummy);
		#endregion

		#region yaf_ForumAccess
		DataTable forumaccess_list(object ForumID);
		void forumaccess_save(object ForumID,object GroupID,object accessMaskID);
		DataTable forumaccess_group(object GroupID);
		#endregion

		#region yaf_Group
		DataTable group_list(object boardID,object GroupID);
		void group_delete(object GroupID);
		DataTable group_member(object boardID,object UserID);
		long group_save(object GroupID,object boardID,object Name,object IsAdmin,object IsGuest,object IsStart,object isModerator,object accessMaskID);
		#endregion

		#region yaf_Mail
		void mail_delete(object MailID);
		DataTable mail_list();
		void mail_createwatch(object topicID,object from,object subject,object body,object userID);
		#endregion

		#region yaf_Message
		DataTable post_list(object topicID,object updateViewCount);
		DataTable post_list_reverse10(object topicID);
		DataTable post_last10user(object boardID,object userID,object pageUserID);
		DataTable message_list(object messageID);
		void message_delete(object messageID);
		void message_approve(object messageID);
		void message_update(object messageID,object priority,object message);
		bool message_save(object TopicID,object UserID,object Message,object UserName,object IP,object posted,object replyTo,ref long nMessageID);
		DataTable message_unapproved(object forumID);
		DataTable message_findunread(object topicID,object lastRead);
		#endregion

		#region yaf_NntpForum
		DataTable nntpforum_list(object boardID,object minutes,object nntpForumID);
		void nntpforum_update(object nntpForumID,object lastMessageNo,object userID);
		void nntpforum_save(object nntpForumID,object nntpServerID,object groupName,object forumID);
		#endregion

		#region yaf_NntpServer
		DataTable nntpserver_list(object boardID,object nntpServerID);
		void nntpserver_save(object nntpServerID,object boardID,object name,object address,object userName,object userPass);
		void nntpserver_delete(object nntpServerID);
		#endregion

		#region yaf_NntpTopic
		DataTable nntptopic_list(object thread);
		void nntptopic_savemessage(object nntpForumID,object topic,object body,object userID,object userName,object ip,object posted,object thread);
		#endregion

		#region yaf_PMessage
		DataTable pmessage_list(object userID,object sent,object pMessageID);
		void pmessage_delete(object pMessageID);
		void pmessage_save(object fromUserID,object toUserID,object subject,object body);
		void pmessage_markread(object userID,object pMessageID);
		DataTable pmessage_info();
		void pmessage_prune(object daysRead,object daysUnread);
		#endregion

		#region yaf_Poll
		DataTable poll_stats(object pollID);
		int poll_save(object question,object c1,object c2,object c3,object c4,object c5,object c6,object c7,object c8,object c9);
		#endregion

		#region yaf_Rank
		DataTable rank_list(object boardID,object rankID);
		void rank_save(object RankID,object boardID,object Name,object IsStart,object IsLadder,object MinPosts,object RankImage);
		void rank_delete(object RankID);
		#endregion

		#region yaf_Smiley
		DataTable smiley_list(object boardID,object SmileyID);
		DataTable smiley_listunique(object boardID);
		void smiley_delete(object SmileyID);
		void smiley_save(object SmileyID,object boardID,object Code,object Icon,object Emoticon,object Replace);
		#endregion

		#region yaf_System
		void system_save(object TimeZone,object SmtpServer,object SmtpUserName,
			object SmtpUserPass,object ForumEmail,object EmailVerification,object ShowMoved,
			object BlankLinks,object showGroups,
			object AvatarWidth,object AvatarHeight,object avatarUpload,object avatarRemote,object avatarSize,
			object allowRichEdit,object allowUserTheme,object allowUserLanguage,
			object useFileTable,object maxFileSize);
		DataTable system_list();
		#endregion

		#region yaf_Topic
		int topic_prune(object ForumID,object Days);
		DataTable topic_list(object ForumID,object Announcement,object Date,object offset,object count);
		void topic_move(object TopicID,object ForumID,object ShowMoved);
		DataTable topic_active(object boardID,object UserID,object Since);
		void topic_delete(object TopicID);
		DataTable topic_findprev(object topicID);
		DataTable topic_findnext(object topicID);
		void topic_lock(object topicID,object locked);
		long topic_save(object ForumID,object Subject,object Message,object UserID,object Priority,object PollID,object UserName,object IP,object posted,ref long nMessageID);
		DataRow topic_info(object topicID);
		#endregion

		#region yaf_User
		DataTable user_list(object boardID,object UserID,object Approved);
		void user_delete(object UserID);
		void user_approve(object UserID);
		void user_suspend(object userID,object suspend);
		bool user_changepassword(object UserID,object oldpw,object newpw);
		void user_save(object UserID,object boardID,object UserName,object Password,object Email,object Hash,
			object Location,object HomePage,object TimeZone,object Avatar,
			object languageFile,object themeFile,object Approved,
			object msn,object yim,object aim,object icq,
			object realName,object occupation,object interests,object gender,object weblog);
		void user_adminsave(object boardID,object UserID,object Name,object email,object isHostAdmin,object RankID);
		DataTable user_emails(object boardID,object GroupID);
		DataTable user_accessmasks(object boardID,object userID);
		bool user_recoverpassword(object userName,object email,object password);
		object user_login(object boardID,object name,object password);
		DataTable user_avatarimage(object userID);
		DataTable user_find(object boardID,bool filter,object userName,object email);
		string user_getsignature(object userID);
		void user_savesignature(object userID,object signature);
		void user_saveavatar(object userID,System.IO.Stream stream);
		void user_deleteavatar(object userID);
		bool user_register(pages.ForumPage page,object boardID,object userName,object password,object email,object location,object homePage,object timeZone,bool emailVerification);
		bool user_access(object userID,object forumID);
		int user_guest();
		DataTable user_activity_rank();
		#endregion

		#region yaf_UserForum
		DataTable userforum_list(object userID,object forumID);
		void userforum_delete(object userID,object forumID);
		void userforum_save(object userID,object forumID,object accessMaskID);
		#endregion

		#region yaf_UserGroup
		DataTable usergroup_list(object boardID,object UserID);
		void usergroup_save(object UserID,object GroupID,object Member);
		#endregion

		#region yaf_WatchForum
		void watchforum_add(object UserID,object ForumID);
		DataTable watchforum_list(object userID);
		void watchforum_delete(object watchForumID);
		#endregion

		#region yaf_WatchTopic
		DataTable watchtopic_list(object userID);
		void watchtopic_delete(object watchTopicID);
		void watchtopic_add(object userID,object topicID);
		#endregion
	}

	public class DB
	{
		private DB()
		{
		}

		static public IDataProvider DataProvider
		{
			get
			{
				try 
				{
					return (IDataProvider)Activator.CreateInstance(Type.GetType(Config.ConfigSection["dataprovider"]));
				}
				catch(Exception)
				{
					return (IDataProvider)Activator.CreateInstance(Type.GetType("yaf.MsSql,yaf"));
				}
			}
		}
	}

}
