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
using System.Web.Security;

namespace yaf
{
	public class DB 
	{
		#region DB Access Functions
		public static SqlConnection GetConnection() 
		{
			try 
			{
				SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
				conn.Open();
				return conn;
			}
			catch(Exception) 
			{
				return null;
			}
		}

		public static DataTable GetData(SqlCommand cmd) 
		{
			if(cmd.Connection!=null) 
			{
				using(DataSet ds = new DataSet()) 
				{
					using(SqlDataAdapter da = new SqlDataAdapter()) 
					{
						da.SelectCommand = cmd;
						da.Fill(ds);
						return ds.Tables[0];
					}
				}
			} 
			else 
			{
				using(SqlConnection conn = GetConnection()) 
				{
					using(DataSet ds = new DataSet()) 
					{
						using(SqlDataAdapter da = new SqlDataAdapter()) 
						{
							da.SelectCommand = cmd;
							da.SelectCommand.Connection = conn;
							da.Fill(ds);
							return ds.Tables[0];
						}
					}
				}
			}
		}
		public static void ExecuteNonQuery(SqlCommand cmd) 
		{
			using(SqlConnection conn = GetConnection()) 
			{
				cmd.Connection = conn;
				cmd.ExecuteNonQuery();
			}
		}
		public static object ExecuteScalar(SqlCommand cmd) 
		{
			using(SqlConnection conn = GetConnection()) 
			{
				cmd.Connection = conn;
				return cmd.ExecuteScalar();
			}
		}
		#endregion
		
		#region Forum
		static public DataRow pageload(object SessionID,object User,object IP,object Location,object Browser,
			object Platform,object CategoryID,object ForumID,object TopicID,object MessageID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_pageload")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@SessionID",SessionID);
				cmd.Parameters.Add("@User",User);
				cmd.Parameters.Add("@IP",IP);
				cmd.Parameters.Add("@Location",Location);
				cmd.Parameters.Add("@Browser",Browser);
				cmd.Parameters.Add("@Platform",Platform);
				cmd.Parameters.Add("@CategoryID",CategoryID);
				cmd.Parameters.Add("@ForumID",ForumID);
				cmd.Parameters.Add("@TopicID",TopicID);
				cmd.Parameters.Add("@MessageID",MessageID);
				using(DataTable dt = GetData(cmd)) 
				{
					if(dt.Rows.Count>0) 
						return dt.Rows[0];
					else
						return null;
				}
			}
		}
		static public DataRow stats() 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_stats")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				using(DataTable dt = GetData(cmd)) 
				{
					return dt.Rows[0];
				}
			}
		}
		static public DataRow board_stats() 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_board_stats")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				using(DataTable dt = GetData(cmd)) 
				{
					return dt.Rows[0];
				}
			}
		}
		#endregion

		#region DataSets
		static public DataSet ds_forumlayout(object UserID,object CategoryID) 
		{
			using(DataSet ds = new DataSet()) 
			{
				using(SqlDataAdapter da = new SqlDataAdapter("yaf_forum_moderators",GetConnection())) 
				{
					da.SelectCommand.CommandType = CommandType.StoredProcedure;
					da.Fill(ds,"Moderators");
					
					da.SelectCommand.Parameters.Add("@UserID",UserID);
					if(CategoryID!=null && long.Parse(CategoryID.ToString())!=0)
						da.SelectCommand.Parameters.Add("@CategoryID",CategoryID);
					da.SelectCommand.CommandText = "yaf_category_listread";
					da.Fill(ds,"yaf_Category");

					da.SelectCommand.CommandText = "yaf_forum_listread";
					da.Fill(ds,"yaf_Forum");
			
					ds.Relations.Add("myrelation",ds.Tables["yaf_Category"].Columns["CategoryID"],ds.Tables["yaf_Forum"].Columns["CategoryID"]);
					ds.Relations.Add("rel2",ds.Tables["yaf_Forum"].Columns["ForumID"],ds.Tables["Moderators"].Columns["ForumID"],false);
			
					return ds;
				}
			}
		}
		static public DataSet ds_forumadmin() 
		{
			using(DataSet ds = new DataSet()) 
			{
				using(SqlDataAdapter da = new SqlDataAdapter("yaf_category_list",GetConnection())) 
				{
					da.SelectCommand.CommandType = CommandType.StoredProcedure;
					da.Fill(ds,"yaf_Category");
					da.SelectCommand.CommandText = "yaf_forum_list";
					da.Fill(ds,"yaf_Forum");
					ds.Relations.Add("myrelation",ds.Tables["yaf_Category"].Columns["CategoryID"],ds.Tables["yaf_Forum"].Columns["CategoryID"]);

					return ds;
				}
			}
		}
		#endregion

		#region yaf_Active
		static public DataTable active_list(object Guests) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_active_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@Guests",Guests);
				return GetData(cmd);
			}
		}
		#endregion

		#region yaf_Attachment
		static public DataTable attachment_list(object messageID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_attachment_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MessageID",messageID);
				return GetData(cmd);
			}
		}
		static public void attachment_save(object messageID,object fileName,object bytes) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_attachment_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MessageID",messageID);
				cmd.Parameters.Add("@FileName",fileName);
				cmd.Parameters.Add("@Bytes",bytes);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_BannedIP
		static public DataTable bannedip_list(object ID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_bannedip_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ID",ID);
				return GetData(cmd);
			}
		}
		static public void bannedip_save(object ID,object Mask) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_bannedip_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ID",ID);
				cmd.Parameters.Add("@Mask",Mask);
				ExecuteNonQuery(cmd);
			}
		}
		static public void bannedip_delete(object ID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_bannedip_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ID",ID);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_Category
		static public void category_delete(object CategoryID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_category_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@CategoryID",CategoryID);
				ExecuteNonQuery(cmd);
			}
		}
		static public DataTable category_list(object CategoryID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_category_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@CategoryID",CategoryID);
				return GetData(cmd);
			}
		}
		static public void category_save(object CategoryID,object Name,object SortOrder) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_category_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@CategoryID",CategoryID);
				cmd.Parameters.Add("@Name",Name);
				cmd.Parameters.Add("@SortOrder",SortOrder);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_CheckEmail
		static public void checkemail_save(object UserID,object Hash,object Email) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_checkemail_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@Hash",Hash);
				cmd.Parameters.Add("@Email",Email);
				ExecuteNonQuery(cmd);
			}
		}
		static public bool checkemail_update(object hash) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_checkemail_update")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@Hash",hash);
				return (bool)ExecuteScalar(cmd);
			}
		}
		#endregion

		#region yaf_Choice
		static public void choice_vote(object choiceID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_choice_vote")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ChoiceID",choiceID);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_Forum
		static public void forum_delete(object ForumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				ExecuteNonQuery(cmd);
			}
		}
		static public DataTable forum_list(object ForumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				return GetData(cmd);
			}
		}
		static public DataTable forum_listread(object UserID,object CategoryID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_listread")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@CategoryID",CategoryID);
				return GetData(cmd);
			}
		}
		static public DataTable forum_moderatelist() 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_moderatelist")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				//cmd.Parameters.Add("@UserID",UserID);
				return GetData(cmd);
			}
		}
		static public DataTable forum_moderators() 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_moderators")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return GetData(cmd);
			}
		}
		static public long forum_save(object ForumID,object CategoryID,object Name,object Description,object SortOrder,object Locked,object Hidden,object IsTest,object moderated) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				cmd.Parameters.Add("@CategoryID",CategoryID);
				cmd.Parameters.Add("@Name",Name);
				cmd.Parameters.Add("@Description",Description);
				cmd.Parameters.Add("@SortOrder",SortOrder);
				cmd.Parameters.Add("@Locked",Locked);
				cmd.Parameters.Add("@Hidden",Hidden);
				cmd.Parameters.Add("@IsTest",IsTest);
				cmd.Parameters.Add("@Moderated",moderated);
				return long.Parse(ExecuteScalar(cmd).ToString());
			}
		}
		#endregion

		#region yaf_ForumAccess
		static public DataTable forumaccess_list(object ForumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forumaccess_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				return GetData(cmd);
			}
		}
		static public void forumaccess_save(
			object ForumID,object GroupID,object ReadAccess,object PostAccess,object ReplyAccess,
			object PriorityAccess,object PollAccess,object VoteAccess,object ModeratorAccess,
			object EditAccess,object DeleteAccess,object UploadAccess
			) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forumaccess_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				cmd.Parameters.Add("@GroupID",GroupID);
				cmd.Parameters.Add("@ReadAccess",ReadAccess);
				cmd.Parameters.Add("@PostAccess",PostAccess);
				cmd.Parameters.Add("@ReplyAccess",ReplyAccess);
				cmd.Parameters.Add("@PriorityAccess",PriorityAccess);
				cmd.Parameters.Add("@PollAccess",PollAccess);
				cmd.Parameters.Add("@VoteAccess",VoteAccess);
				cmd.Parameters.Add("@ModeratorAccess",ModeratorAccess);
				cmd.Parameters.Add("@EditAccess",EditAccess);
				cmd.Parameters.Add("@DeleteAccess",DeleteAccess);
				cmd.Parameters.Add("@UploadAccess",UploadAccess);
				ExecuteNonQuery(cmd);
			}
		}
		static public DataTable forumaccess_group(object GroupID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forumaccess_group")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@GroupID",GroupID);
				return GetData(cmd);
			}
		}
		static public void forumaccess_repair() 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forumaccess_repair")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_Group
		static public DataTable group_list(object GroupID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_group_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@GroupID",GroupID);
				return GetData(cmd);
			}
		}
		static public void group_delete(object GroupID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_group_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@GroupID",GroupID);
				ExecuteNonQuery(cmd);
			}
		}
		static public DataTable group_member(object UserID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_group_member"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				return GetData(cmd);
			}
		}
		static public long group_save(object GroupID,object Name,object IsAdmin,object IsGuest,object IsStart,object isModerator) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_group_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@GroupID",GroupID);
				cmd.Parameters.Add("@Name",Name);
				cmd.Parameters.Add("@IsAdmin",IsAdmin);
				cmd.Parameters.Add("@IsGuest",IsGuest);
				cmd.Parameters.Add("@IsStart",IsStart);
				cmd.Parameters.Add("@IsModerator",isModerator);
				return long.Parse(ExecuteScalar(cmd).ToString());
			}
		}
		#endregion

		#region yaf_Mail
		static public void mail_delete(object MailID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_mail_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MailID",MailID);
				ExecuteNonQuery(cmd);
			}
		}
		static public DataTable mail_list() 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_mail_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return GetData(cmd);
			}
		}
		static public void mail_createwatch(object topicID,object from,object subject,object body,object userID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_mail_createwatch")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",topicID);
				cmd.Parameters.Add("@From",from);
				cmd.Parameters.Add("@Subject",subject);
				cmd.Parameters.Add("@Body",body);
				cmd.Parameters.Add("@UserID",userID);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_Message
		static public DataTable post_list(object topicID,object userID,object updateViewCount) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_post_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",topicID);
				cmd.Parameters.Add("@UserID",userID);
				cmd.Parameters.Add("@UpdateViewCount",updateViewCount);
				return GetData(cmd);
			}
		}
		static public DataTable post_list_reverse10(object topicID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_post_list_reverse10")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",topicID);
				return GetData(cmd);
			}
		}

		static public DataTable message_list(object messageID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_message_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MessageID",messageID);
				return GetData(cmd);
			}
		}
		static public void message_delete(object messageID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_message_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MessageID",messageID);
				ExecuteNonQuery(cmd);
			}
		}
		static public void message_approve(object messageID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_message_approve")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MessageID",messageID);
				ExecuteNonQuery(cmd);
			}
		}
		static public void message_update(object messageID,object priority,object message) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_message_update")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MessageID",messageID);
				cmd.Parameters.Add("@Priority",priority);
				cmd.Parameters.Add("@Message",message);
				ExecuteNonQuery(cmd);
			}
		}
		public static bool message_save(object TopicID,object UserID,object Message,object UserName,object IP,ref long nMessageID) 		{
			if(System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
				UserName = null;

			using(SqlCommand cmd = new SqlCommand("yaf_message_save")) 
			{
				SqlParameter pMessageID = new SqlParameter("@MessageID",nMessageID);
				pMessageID.Direction = ParameterDirection.Output;

				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",TopicID);
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@Message",Message);
				cmd.Parameters.Add("@UserName",UserName);
				cmd.Parameters.Add("@IP",IP);
				cmd.Parameters.Add(pMessageID);
				DB.ExecuteNonQuery(cmd);
				nMessageID = (long)pMessageID.Value;
				return true;
			}
		}
		static public DataTable message_unapproved(object forumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_message_unapproved")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",forumID);
				return GetData(cmd);
			}
		}
		#endregion

		#region yaf_PMessage
		static public DataTable pmessage_list(object userID,object sent,object pMessageID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_pmessage_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				cmd.Parameters.Add("@Sent",sent);
				cmd.Parameters.Add("@PMessageID",pMessageID);
				return GetData(cmd);
			}
		}
		static public void pmessage_delete(object pMessageID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_pmessage_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@PMessageID",pMessageID);
				ExecuteNonQuery(cmd);
			}
		}
		static public void pmessage_save(object from,object to,object subject,object body) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_pmessage_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@From",from);
				cmd.Parameters.Add("@To",to);
				cmd.Parameters.Add("@Subject",subject);
				cmd.Parameters.Add("@Body",body);
				ExecuteNonQuery(cmd);
			}
		}
		static public void pmessage_markread(object userID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_pmessage_markread")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_Poll
		static public DataTable poll_stats(object pollID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_poll_stats")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@PollID",pollID);
				return GetData(cmd);
			}
		}
		static public int poll_save(object question,object c1,object c2,object c3,object c4,object c5,object c6,object c7,object c8,object c9) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_poll_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@Question",question);
				cmd.Parameters.Add("@Choice1",c1);
				cmd.Parameters.Add("@Choice2",c2);
				cmd.Parameters.Add("@Choice3",c3);
				cmd.Parameters.Add("@Choice4",c4);
				cmd.Parameters.Add("@Choice5",c5);
				cmd.Parameters.Add("@Choice6",c6);
				cmd.Parameters.Add("@Choice7",c7);
				cmd.Parameters.Add("@Choice8",c8);
				cmd.Parameters.Add("@Choice9",c9);
				return (int)ExecuteScalar(cmd);
			}
		}
		#endregion

		#region yaf_Rank
		static public DataTable rank_list(object RankID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_rank_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@RankID",RankID);
				return GetData(cmd);
			}
		}
		static public void rank_save(object RankID,object Name,object IsStart,object IsLadder,object MinPosts,object RankImage) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_rank_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@RankID",RankID);
				cmd.Parameters.Add("@Name",Name);
				cmd.Parameters.Add("@IsStart",IsStart);
				cmd.Parameters.Add("@IsLadder",IsLadder);
				cmd.Parameters.Add("@MinPosts",MinPosts);
				cmd.Parameters.Add("@RankImage",RankImage);
				ExecuteNonQuery(cmd);
			}
		}
		static public void rank_delete(object RankID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_rank_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@RankID",RankID);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_Smiley
		static public DataTable smiley_list(object SmileyID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_smiley_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@SmileyID",SmileyID);
				return GetData(cmd);
			}
		}
		static public DataTable smiley_listunique() 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_smiley_listunique")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return GetData(cmd);
			}
		}
		static public void smiley_delete(object SmileyID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_smiley_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@SmileyID",SmileyID);
				ExecuteNonQuery(cmd);
			}
		}
		static public void smiley_save(object SmileyID,object Code,object Icon,object Emoticon,object Replace) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_smiley_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@SmileyID",SmileyID);
				cmd.Parameters.Add("@Code",Code);
				cmd.Parameters.Add("@Icon",Icon);
				cmd.Parameters.Add("@Emoticon",Emoticon);
				cmd.Parameters.Add("@Replace",Replace);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_System
		static public void system_save(object Name,object TimeZone,object SmtpServer,object SmtpUserName,
			object SmtpUserPass,object ForumEmail,object EmailVerification,object ShowMoved,
			object BlankLinks,object AvatarWidth,object AvatarHeight,object avatarUpload,object avatarRemote) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_system_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@Name",Name);
				cmd.Parameters.Add("@TimeZone",TimeZone);
				cmd.Parameters.Add("@SmtpServer",SmtpServer);
				cmd.Parameters.Add("@SmtpUserName",SmtpUserName);
				cmd.Parameters.Add("@SmtpUserPass",SmtpUserPass);
				cmd.Parameters.Add("@ForumEmail",ForumEmail);
				cmd.Parameters.Add("@EmailVerification",EmailVerification);
				cmd.Parameters.Add("@ShowMoved",ShowMoved);
				cmd.Parameters.Add("@BlankLinks",BlankLinks);
				cmd.Parameters.Add("@AvatarWidth",AvatarWidth);
				cmd.Parameters.Add("@AvatarHeight",AvatarHeight);
				cmd.Parameters.Add("@AvatarUpload",avatarUpload);
				cmd.Parameters.Add("@AvatarRemote",avatarRemote);
				ExecuteNonQuery(cmd);
			}
		}
		static public DataTable system_list() 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_system_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return GetData(cmd);
			}
		}
		#endregion

		#region yaf_Topic
		static public int topic_prune(object ForumID,object Days) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_prune")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				cmd.Parameters.Add("@Days",Days);
				return (int)ExecuteScalar(cmd);
			}
		}
		static public DataTable topic_list(object ForumID,object UserID,object Announcement,object Date) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@Announcement",Announcement);
				cmd.Parameters.Add("@Date",Date);
				return GetData(cmd);
			}
		}
		static public void topic_move(object TopicID,object ForumID,object ShowMoved) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_move")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",TopicID);
				cmd.Parameters.Add("@ForumID",ForumID);
				cmd.Parameters.Add("@ShowMoved",ShowMoved);
				ExecuteNonQuery(cmd);
			}
		}
		static public DataTable topic_active(object UserID,object Since) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_active")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@Since",Since);
				return GetData(cmd);
			}
		}
		static public void topic_delete(object TopicID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",TopicID);
				ExecuteNonQuery(cmd);
			}
		}
		static public DataTable topic_findprev(object topicID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_findprev")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",topicID);
				return GetData(cmd);
			}
		}
		static public DataTable topic_findnext(object topicID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_findnext")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",topicID);
				return GetData(cmd);
			}
		}
		static public void topic_lock(object topicID,object locked) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_lock")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",topicID);
				cmd.Parameters.Add("@Locked",locked);
				ExecuteNonQuery(cmd);
			}
		}
		static public long topic_save(object ForumID,object Subject,object Message,object UserID,object Priority,object PollID,object UserName,object IP,ref long nMessageID) 
		{
			if(System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
				UserName = null;

			using(SqlCommand cmd = new SqlCommand("yaf_topic_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				cmd.Parameters.Add("@Subject",Subject);
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@Message",Message);
				cmd.Parameters.Add("@Priority",Priority);
				cmd.Parameters.Add("@UserName",UserName);
				cmd.Parameters.Add("@IP",IP);
				cmd.Parameters.Add("@PollID",PollID);

				DataTable dt = DB.GetData(cmd);
				nMessageID = long.Parse(dt.Rows[0]["MessageID"].ToString());
				return long.Parse(dt.Rows[0]["TopicID"].ToString());
			}
		}
		static public DataRow topic_info(object topicID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_info")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",topicID);
				using(DataTable dt = DB.GetData(cmd)) 
				{
					if(dt.Rows.Count>0)
						return dt.Rows[0];
					else
						return null;
				}
			}
		}
		#endregion

		#region yaf_User
		static public DataTable user_list(object UserID,object Approved) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@Approved",Approved);
				return GetData(cmd);
			}
		}
		static public void user_delete(object UserID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				ExecuteNonQuery(cmd);
			}
		}
		static public bool user_changepassword(object UserID,object oldpw,object newpw) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_changepassword")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@OldPassword",oldpw);
				cmd.Parameters.Add("@NewPassword",newpw);
				return (bool)ExecuteScalar(cmd);
			}
		}
		static public void user_save(object UserID,object UserName,object Password,object Email,object Hash,
			object Location,object HomePage,object TimeZone,object Avatar,object Approved) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@UserName",UserName);
				cmd.Parameters.Add("@Password",Password);
				cmd.Parameters.Add("@Email",Email);
				cmd.Parameters.Add("@Hash",Hash);
				cmd.Parameters.Add("@Location",Location);
				cmd.Parameters.Add("@HomePage",HomePage);
				cmd.Parameters.Add("@TimeZone",TimeZone);
				cmd.Parameters.Add("@Avatar",Avatar);
				cmd.Parameters.Add("@Approved",Approved);
				ExecuteNonQuery(cmd);
			}
		}
		static public void user_adminsave(object UserID,object Name,object RankID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_adminsave")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@Name",Name);
				cmd.Parameters.Add("@RankID",RankID);
				ExecuteNonQuery(cmd);
			}
		}
		static public DataTable user_emails(object GroupID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_emails")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@GroupID",GroupID);

				return GetData(cmd);
			}
		}
		static public bool user_recoverpassword(object userName,object email,object password) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_recoverpassword")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserName",userName);
				cmd.Parameters.Add("@Email",email);
				cmd.Parameters.Add("@Password",password);
				return (bool)ExecuteScalar(cmd);
			}
		}
		static public bool user_login(object name,object password) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_login")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@Name",name);
				cmd.Parameters.Add("@Password",password);
				return (bool)ExecuteScalar(cmd);
			}
		}
		static public DataTable user_avatarimage(object userID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_avatarimage")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				return GetData(cmd);
			}
		}
		static public bool user_find(object userName,object email) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_find")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserName",userName);
				cmd.Parameters.Add("@Email",email);
				using(DataTable dt = GetData(cmd)) 
					return dt.Rows.Count>0;
			}
		}
		static public string user_getsignature(object userID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_getsignature")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				return ExecuteScalar(cmd).ToString();
			}
		}
		static public void user_savesignature(object userID,object signature) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_savesignature")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				cmd.Parameters.Add("@Signature",signature);
				ExecuteNonQuery(cmd);
			}
		}
		static public int user_extvalidate(object userName,object email) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_extvalidate")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@Name",userName);
				cmd.Parameters.Add("@Email",email);
				return (int)ExecuteScalar(cmd);
			}
		}
		static public void user_saveavatar(object userID,System.IO.Stream stream) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_avatarimage",GetConnection())) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);

				using(SqlDataAdapter da = new SqlDataAdapter(cmd)) 
				{
					using(SqlCommandBuilder cb = new SqlCommandBuilder(da)) 
					{
						using(DataSet ds = new DataSet()) 
						{
							byte[] data = new byte[stream.Length];
							stream.Seek(0,System.IO.SeekOrigin.Begin);
							stream.Read(data,0,(int)stream.Length);

							da.Fill(ds);
							ds.Tables[0].Rows[0]["AvatarImage"] = data;
							da.Update(ds);
						}
					}
				}
			}
		}
		static public void user_register(BasePage page,object userName,object password,object email,object location,object homePage,object timeZone,bool emailVerification) 
		{
			string hashinput = DateTime.Now.ToString() + email.ToString() + register.CreatePassword(20);
			string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput,"md5");

			using(SqlConnection conn = GetConnection()) 
			{
				using(SqlTransaction trans = conn.BeginTransaction()) 
				{
					try 
					{
						using(SqlCommand cmd = new SqlCommand("yaf_user_save",conn)) 
						{
							cmd.Transaction = trans;
							cmd.Connection = conn;
							cmd.CommandType = CommandType.StoredProcedure;
							int UserID = 0;
							cmd.Parameters.Add("@UserID",UserID);
							cmd.Parameters.Add("@UserName",userName);
							cmd.Parameters.Add("@Password",FormsAuthentication.HashPasswordForStoringInConfigFile(password.ToString(),"md5"));
							cmd.Parameters.Add("@Email",email);
							cmd.Parameters.Add("@Hash",hash);
							cmd.Parameters.Add("@Location",location);
							cmd.Parameters.Add("@HomePage",homePage);
							cmd.Parameters.Add("@TimeZone",timeZone);
							cmd.Parameters.Add("@Approved",!emailVerification);
							cmd.ExecuteNonQuery();
						}

						if(emailVerification) 
						{
							//  Build a MailMessage
							string body = page.ReadTemplate("verifyemail.txt");
							body = body.Replace("{link}",String.Format("{1}approve.aspx?k={0}",hash,page.ForumURL));
							body = body.Replace("{key}",hash);
							body = body.Replace("{forumname}",page.ForumName);
							body = body.Replace("{forumlink}",String.Format("{0}",page.ForumURL));

							page.SendMail(page.ForumEmail,email.ToString(),String.Format("{0} email verification",page.ForumName),body);
							page.AddLoadMessage("A mail has been sent. Check your inbox and click the link in the mail.");
							trans.Commit();
						} 
						else 
						{
							trans.Commit();
							FormsAuthentication.RedirectFromLoginPage(userName.ToString(), false);
						}
					}
					catch(Exception x) 
					{
						trans.Rollback();
						page.AddLoadMessage(x.Message);
					}
				}
			}
		}
		public static bool user_access(object userID,object forumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_access")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				cmd.Parameters.Add("@ForumID",forumID);
				using(DataTable dt = DB.GetData(cmd)) 
				{
					foreach(DataRow row in dt.Rows) 
					{
						return long.Parse(row["ReadAccess"].ToString())>0;
					}
				}
			}
			return false;
		}
		#endregion

		#region yaf_UserGroup
		static public DataTable usergroup_list(object UserID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_usergroup_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				return GetData(cmd);
			}
		}
		static public void usergroup_save(object UserID,object GroupID,object Member) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_usergroup_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@GroupID",GroupID);
				cmd.Parameters.Add("@Member",Member);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_WatchForum
		static public void watchforum_add(object UserID,object ForumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_watchforum_add")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@ForumID",ForumID);
				ExecuteNonQuery(cmd);
			}
		}
		static public DataTable watchforum_list(object userID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_watchforum_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				return GetData(cmd);
			}
		}
		static public void watchforum_delete(object watchForumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_watchforum_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@WatchForumID",watchForumID);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_WatchTopic
		static public DataTable watchtopic_list(object userID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_watchtopic_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				return GetData(cmd);
			}
		}
		static public void watchtopic_delete(object watchTopicID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_watchtopic_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@WatchTopicID",watchTopicID);
				ExecuteNonQuery(cmd);
			}
		}
		static public void watchtopic_add(object userID,object topicID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_watchtopic_add")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				cmd.Parameters.Add("@TopicID",topicID);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion
	}
}
