/* Copyright (C) 2003 Bjørnar Henden
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
	/// <summary>
	/// Summary description for Data.
	/// </summary>
	public class Data 
	{
		public static DataTable GetTopics(int ForumID,int UserID,bool Announcement,DateTime Date) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@Announcement",Announcement);
				cmd.Parameters.Add("@Date",Date);
				return yaf.DataManager.GetData(cmd);
			}
		}

		public static DataTable GetTopics(int ForumID,int UserID,bool Announcement) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@Announcement",Announcement);
				return yaf.DataManager.GetData(cmd);
			}
		}

		public static bool AddUser(string UserName,string Password,string Email,string Hash,object Location,object HomePage,int TimeZone,bool Approved) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_find")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserName",UserName);
				cmd.Parameters.Add("@Email",Email);
				DataTable dtUser = DataManager.GetData(cmd);
				if(dtUser.Rows.Count>0)
					return false;
			}
			using(SqlCommand cmd = new SqlCommand("yaf_user_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				int UserID = 0;
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@UserName",UserName);
				cmd.Parameters.Add("@Password",FormsAuthentication.HashPasswordForStoringInConfigFile(Password,"md5"));
				cmd.Parameters.Add("@Email",Email);
				cmd.Parameters.Add("@Hash",Hash);
				cmd.Parameters.Add("@Location",Location);
				cmd.Parameters.Add("@HomePage",HomePage);
				cmd.Parameters.Add("@TimeZone",TimeZone);
				cmd.Parameters.Add("@Approved",Approved);
				yaf.DataManager.ExecuteNonQuery(cmd);
				return true;
			}
		}

		public static DataRow TopicInfo(int TopicID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_info")) {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",TopicID);
				return DataManager.GetData(cmd).Rows[0];
			}
		}

		public static DataRow ForumInfo(int ForumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_list")) {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				return DataManager.GetData(cmd).Rows[0];
			}
		}

		public static bool PostReply(int TopicID,string User,string Message,string UserName,string IP) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_message_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",TopicID);
				cmd.Parameters.Add("@User",User);
				cmd.Parameters.Add("@Message",Message);
				cmd.Parameters.Add("@UserName",UserName);
				cmd.Parameters.Add("@IP",IP);
				yaf.DataManager.ExecuteNonQuery(cmd);
				return true;
			}
		}

		public static int PostMessage(int ForumID,string Subject,string Message,string User,int Priority,int PollID,string UserName,string IP) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				cmd.Parameters.Add("@Subject",Subject);
				cmd.Parameters.Add("@User",User);
				cmd.Parameters.Add("@Message",Message);
				cmd.Parameters.Add("@Priority",Priority);
				cmd.Parameters.Add("@UserName",UserName);
				cmd.Parameters.Add("@IP",IP);
				if(PollID>0)
					cmd.Parameters.Add("@PollID",PollID);
				return (int)yaf.DataManager.ExecuteScalar(cmd);
			}
		}

		public static DataRow GetMessageInfo(int MessageID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_message_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MessageID",MessageID);
				return yaf.DataManager.GetData(cmd).Rows[0];
			}
		}

		public static void LockTopic(int TopicID,bool Locked) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_lock")) {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",TopicID);
				cmd.Parameters.Add("@Locked",Locked);
				DataManager.ExecuteNonQuery(cmd);
			}
		}

		#region Forum Access Functions
		public static bool ForumReadAccess(int UserID,object ForumID) {
			//return ForumAccess("ReadAccess",UserID,ForumID);
			using(SqlCommand cmd = new SqlCommand("yaf_user_access")) {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@ForumID",ForumID);
				using(DataTable dt = DataManager.GetData(cmd)) {
					if(dt.Rows.Count>0)
						return (bool)dt.Rows[0]["ReadAccess"];
				}
			}
			return false;
		}
		#endregion

		#region Time Zone Functions
		public static DataTable TimeZones() {
			DataTable dt = new DataTable("TimeZone");
			dt.Columns.Add("Value",Type.GetType("System.Int32"));
			dt.Columns.Add("Name",Type.GetType("System.String"));

			dt.Rows.Add(new object[]{-720,"(GMT - 12:00) Enitwetok, Kwajalien"});
			dt.Rows.Add(new object[]{-660,"(GMT - 11:00) Midway Island, Samoa"});
			dt.Rows.Add(new object[]{-600,"(GMT - 10:00) Hawaii"});
			dt.Rows.Add(new object[]{-540,"(GMT - 9:00) Alaska"});
			dt.Rows.Add(new object[]{-480,"(GMT - 8:00) Pacific Time (US & Canada)"});
			dt.Rows.Add(new object[]{-420,"(GMT - 7:00) Mountain Time (US & Canada)"});
			dt.Rows.Add(new object[]{-360,"(GMT - 6:00) Central Time (US & Canada), Mexico City"});
			dt.Rows.Add(new object[]{-300,"(GMT - 5:00) Eastern Time (US & Canada), Bogota, Lima, Quito"});
			dt.Rows.Add(new object[]{-240,"(GMT - 4:00) Atlantic Time (Canada), Caracas, La Paz"});
			dt.Rows.Add(new object[]{-210,"(GMT - 3:30) Newfoundland"});
			dt.Rows.Add(new object[]{-180,"(GMT - 3:00) Brazil, Buenos Aires, Georgetown, Falkland Is."});
			dt.Rows.Add(new object[]{-120,"(GMT - 2:00) Mid-Atlantic, Ascention Is., St Helena"});
			dt.Rows.Add(new object[]{-60,"(GMT - 1:00) Azores, Cape Verde Islands"});
			dt.Rows.Add(new object[]{0,"(GMT) Casablanca, Dublin, Edinburgh, London, Lisbon, Monrovia"});
			dt.Rows.Add(new object[]{60,"(GMT + 1:00) Berlin, Brussels, Kristiansund, Madrid, Paris, Rome"});
			dt.Rows.Add(new object[]{120,"(GMT + 2:00) Kaliningrad, South Africa, Warsaw"});
			dt.Rows.Add(new object[]{180,"(GMT + 3:00) Baghdad, Riyadh, Moscow, Nairobi"});
			dt.Rows.Add(new object[]{210,"(GMT + 3:30) Tehran"});
			dt.Rows.Add(new object[]{240,"(GMT + 4:00) Adu Dhabi, Baku, Muscat, Tbilisi"});
			dt.Rows.Add(new object[]{270,"(GMT + 4:30) Kabul"});
			dt.Rows.Add(new object[]{300,"(GMT + 5:00) Ekaterinburg, Islamabad, Karachi, Tashkent"});
			dt.Rows.Add(new object[]{330,"(GMT + 5:30) Bombay, Calcutta, Madras, New Delhi"});
			dt.Rows.Add(new object[]{360,"(GMT + 6:00) Almaty, Colomba, Dhakra"});
			dt.Rows.Add(new object[]{420,"(GMT + 7:00) Bangkok, Hanoi, Jakarta"});
			dt.Rows.Add(new object[]{480,"(GMT + 8:00) Beijing, Hong Kong, Perth, Singapore, Taipei"});
			dt.Rows.Add(new object[]{540,"(GMT + 9:00) Osaka, Sapporo, Seoul, Tokyo, Yakutsk"});
			dt.Rows.Add(new object[]{570,"(GMT + 9:30) Adelaide, Darwin"});
			dt.Rows.Add(new object[]{600,"(GMT + 10:00) Melbourne, Papua New Guinea, Sydney, Vladivostok"});
			dt.Rows.Add(new object[]{660,"(GMT + 11:00) Magadan, New Caledonia, Solomon Islands"});
			dt.Rows.Add(new object[]{720,"(GMT + 12:00) Auckland, Wellington, Fiji, Marshall Island"});

			return dt;
		}
		#endregion
	}
}
