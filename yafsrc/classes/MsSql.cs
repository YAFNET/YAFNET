using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;

namespace yaf
{
	public class MsSql : IDataProvider
	{
		private SqlConnection	m_conn;
		public MsSql()
		{
			m_conn = new SqlConnection(Config.ConfigSection["connstr"]);
			m_conn.Open();
		}

		virtual public void Dispose()
		{
			m_conn.Close();
			m_conn.Dispose();
		}

		#region DB Access Functions
		public SqlConnection Conn
		{
			get
			{
				return m_conn;
			}
		}
		public SqlConnection GetConnection2() 
		{
			return m_conn;
		}

		private DataTable GetData(SqlCommand cmd) 
		{
			QueryCounter qc = new QueryCounter(cmd.CommandText);
			try 
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
					using(DataSet ds = new DataSet()) 
					{
						using(SqlDataAdapter da = new SqlDataAdapter()) 
						{
							da.SelectCommand = cmd;
							da.SelectCommand.Connection = Conn;
							da.Fill(ds);
							//da.Fill(ds,0,20,"dummy");
							return ds.Tables[0];
						}
					}
				}
			}
			finally 
			{
				qc.Dispose();
			}
		}
		public void ExecuteNonQuery(SqlCommand cmd) 
		{
			QueryCounter qc = new QueryCounter(cmd.CommandText);
			try 
			{
				cmd.Connection = Conn;
				cmd.ExecuteNonQuery();
			}
			finally 
			{
				qc.Dispose();
			}
		}
		public object ExecuteScalar(SqlCommand cmd) 
		{
			QueryCounter qc = new QueryCounter(cmd.CommandText);
			try 
			{
				cmd.Connection = Conn;
				return cmd.ExecuteScalar();
			}
			finally
			{
				qc.Dispose();
			}
		}
		public int DBSize() 
		{
			using(SqlCommand cmd = new SqlCommand("select sum(size) * 8 * 1024 from sysfiles")) 
			{
				cmd.CommandType = CommandType.Text;
				return (int)ExecuteScalar(cmd);
			}
		}
		#endregion
		
		#region Forum
		public DataRow pageload(object SessionID,object boardID,object User,object IP,object Location,object Browser,
			object Platform,object CategoryID,object ForumID,object TopicID,object MessageID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_pageload")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@SessionID",SessionID);
				cmd.Parameters.Add("@BoardID",boardID);
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
		public DataTable GetSearchResult( string ToSearch, SEARCH_FIELD sf, SEARCH_WHAT sw, int fid, int UserID ) 
		{
			if( ToSearch.Length == 0 )
				return new DataTable();

			if( ToSearch == "*" )
				ToSearch = "";

			ToSearch = ToSearch.Replace("'","''");

			string sql = "select a.ForumID, a.TopicID, a.Topic, b.UserID, b.Name, c.MessageID, c.Posted, c.Message ";
			sql += "from yaf_topic a left join yaf_message c on a.TopicID = c.TopicID left join yaf_user b on c.UserID = b.UserID join yaf_vaccess x on x.ForumID=a.ForumID ";
			
			sql += String.Format("where x.ReadAccess<>0 and x.UserID={0} ",UserID);

			if( sf == SEARCH_FIELD.sfUSER_NAME )
			{
				sql += string.Format( "and b.Name like '%{0}%' ", ToSearch );
			}
			else
			{
				string[] words;
				sql += "and ( ";
				switch( sw )
				{
					case SEARCH_WHAT.sfALL_WORDS:
						words = ToSearch.Split( ' ' );
						foreach( string word in words )
						{
							sql += string.Format( "(c.Message like '%{0}%' or a.Topic like '%{0}%' ) and ", word );
						}
						// remove last OR in sql query
						sql = sql.Substring( 0, sql.Length - 5 );
						break;
					case SEARCH_WHAT.sfANY_WORDS:
						words = ToSearch.Split( ' ' );
						foreach( string word in words )
						{
							sql += string.Format( "c.Message like '%{0}%' or a.Topic like '%{0}%' or ", word );
						}
						// remove last OR in sql query
						sql = sql.Substring( 0, sql.Length - 4 );
						break;
					case SEARCH_WHAT.sfEXACT:
						sql += string.Format( "c.Message like '%{0}%' or a.Topic like '%{0}%' ", ToSearch );
						break;
				}
				sql += " ) ";
			}

			if( fid >= 0 )
			{
				sql += string.Format( "and a.ForumID = {0}", fid );
			}

			sql += " order by c.Posted desc";

			using(SqlCommand cmd = new SqlCommand()) 
			{
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = sql;
				return GetData(cmd);
			}
		}

		#endregion

		#region DataSets
		public DataSet board_layout(object boardID,object UserID,object CategoryID,object parentID) 
		{
			if(CategoryID!=null && long.Parse(CategoryID.ToString())==0) 
				CategoryID = null;

			using(DataSet ds = new DataSet()) 
			{
				using(SqlDataAdapter da = new SqlDataAdapter("yaf_forum_moderators",Conn)) 
				{
					da.SelectCommand.CommandType = CommandType.StoredProcedure;
					da.Fill(ds,"Moderator");
				}
				using(SqlDataAdapter da = new SqlDataAdapter("yaf_category_listread",Conn)) 
				{
					da.SelectCommand.CommandType = CommandType.StoredProcedure;
					da.SelectCommand.Parameters.Add("@BoardID",boardID);
					da.SelectCommand.Parameters.Add("@UserID",UserID);
					da.SelectCommand.Parameters.Add("@CategoryID",CategoryID);
					da.Fill(ds,"yaf_Category");
				}
				using(SqlDataAdapter da = new SqlDataAdapter("yaf_forum_listread",Conn)) 
				{
					da.SelectCommand.CommandType = CommandType.StoredProcedure;
					da.SelectCommand.Parameters.Add("@BoardID",boardID);
					da.SelectCommand.Parameters.Add("@UserID",UserID);
					da.SelectCommand.Parameters.Add("@CategoryID",CategoryID);
					da.SelectCommand.Parameters.Add("@ParentID",parentID);
					da.Fill(ds,"yaf_Forum");
				}
				ds.Relations.Add("FK_Forum_Category",ds.Tables["yaf_Category"].Columns["CategoryID"],ds.Tables["yaf_Forum"].Columns["CategoryID"]);
				ds.Relations.Add("FK_Moderator_Forum",ds.Tables["yaf_Forum"].Columns["ForumID"],ds.Tables["Moderator"].Columns["ForumID"],false);
				return ds;
			}
		}
		public DataSet ds_forumadmin(object boardID) 
		{
			using(DataSet ds = new DataSet()) 
			{
				using(SqlDataAdapter da = new SqlDataAdapter("yaf_category_list",Conn)) 
				{
					da.SelectCommand.Parameters.Add("@BoardID",boardID);
					da.SelectCommand.CommandType = CommandType.StoredProcedure;
					da.Fill(ds,"yaf_Category");
					da.SelectCommand.CommandText = "yaf_forum_list";
					da.Fill(ds,"yaf_Forum");
					ds.Relations.Add("FK_Forum_Category",ds.Tables["yaf_Category"].Columns["CategoryID"],ds.Tables["yaf_Forum"].Columns["CategoryID"]);

					return ds;
				}
			}
		}
		#endregion

		#region yaf_AccessMask
		public DataTable accessmask_list(object boardID,object accessMaskID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_accessmask_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@AccessMaskID",accessMaskID);
				return GetData(cmd);
			}
		}
		public bool accessmask_delete(object accessMaskID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_accessmask_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@AccessMaskID",accessMaskID);
				return (int)ExecuteScalar(cmd)!=0;
			}
		}
		public void accessmask_save(object accessMaskID,object boardID,object name,object readAccess,object postAccess,object replyAccess,object priorityAccess,object pollAccess,object voteAccess,object moderatorAccess,object editAccess,object deleteAccess,object uploadAccess) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_accessmask_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@AccessMaskID",accessMaskID);
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@Name",name);
				cmd.Parameters.Add("@ReadAccess",readAccess);
				cmd.Parameters.Add("@PostAccess",postAccess);
				cmd.Parameters.Add("@ReplyAccess",replyAccess);
				cmd.Parameters.Add("@PriorityAccess",priorityAccess);
				cmd.Parameters.Add("@PollAccess",pollAccess);
				cmd.Parameters.Add("@VoteAccess",voteAccess);
				cmd.Parameters.Add("@ModeratorAccess",moderatorAccess);
				cmd.Parameters.Add("@EditAccess",editAccess);
				cmd.Parameters.Add("@DeleteAccess",deleteAccess);
				cmd.Parameters.Add("@UploadAccess",uploadAccess);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_Active
		public DataTable active_list(object boardID,object Guests) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_active_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@Guests",Guests);
				return GetData(cmd);
			}
		}
		public DataTable active_listforum(object forumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_active_listforum")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",forumID);
				return GetData(cmd);
			}
		}
		public DataTable active_listtopic(object topicID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_active_listtopic")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",topicID);
				return GetData(cmd);
			}
		}
		public DataRow active_stats(object boardID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_active_stats")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				using(DataTable dt = GetData(cmd)) 
				{
					return dt.Rows[0];
				}
			}
		}
		#endregion

		#region yaf_Attachment
		public DataTable attachment_list(object messageID,object attachmentID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_attachment_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MessageID",messageID);
				cmd.Parameters.Add("@AttachmentID",attachmentID);
				return GetData(cmd);
			}
		}
		public void attachment_save(object messageID,object fileName,object bytes,object contentType,System.IO.Stream stream) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_attachment_save")) 
			{
				byte[] fileData = null;
				if(stream!=null) 
				{
					fileData = new byte[stream.Length];
					stream.Seek(0,System.IO.SeekOrigin.Begin);
					stream.Read(fileData,0,(int)stream.Length);
				}
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MessageID",messageID);
				cmd.Parameters.Add("@FileName",fileName);
				cmd.Parameters.Add("@Bytes",bytes);
				cmd.Parameters.Add("@ContentType",contentType);
				cmd.Parameters.Add("@FileData",fileData);
				ExecuteNonQuery(cmd);
			}
		}
		public void attachment_delete(object attachmentID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_attachment_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@AttachmentID",attachmentID);
				ExecuteNonQuery(cmd);
			}
		}
		public void attachment_download(object attachmentID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_attachment_download")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@AttachmentID",attachmentID);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_BannedIP
		public DataTable bannedip_list(object boardID,object ID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_bannedip_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@ID",ID);
				return GetData(cmd);
			}
		}
		public void bannedip_save(object ID,object boardID,object Mask) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_bannedip_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ID",ID);
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@Mask",Mask);
				ExecuteNonQuery(cmd);
			}
		}
		public void bannedip_delete(object ID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_bannedip_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ID",ID);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_Board
		public DataTable board_list(object boardID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_board_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				return GetData(cmd);
			}
		}
		public DataRow board_poststats(object boardID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_board_poststats")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				using(DataTable dt = GetData(cmd)) 
				{
					return dt.Rows[0];
				}
			}
		}
		public DataRow board_stats() 
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
		public void board_save(object boardID,object name,object allowThreaded)
		{
			using(SqlCommand cmd = new SqlCommand("yaf_board_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@Name",name);
				cmd.Parameters.Add("@AllowThreaded",allowThreaded);
				ExecuteNonQuery(cmd);
			}
		}
		public void board_create(object name,object allowThreaded,object userName,object userEmail,object userPass)
		{
			using(SqlCommand cmd = new SqlCommand("yaf_board_create")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardName",name);
				cmd.Parameters.Add("@AllowThreaded",allowThreaded);
				cmd.Parameters.Add("@UserName",userName);
				cmd.Parameters.Add("@UserEmail",userEmail);
				cmd.Parameters.Add("@UserPass",userPass);
				cmd.Parameters.Add("@IsHostAdmin",false);
				ExecuteNonQuery(cmd);
			}
		}
		public void board_delete(object boardID)
		{
			using(SqlCommand cmd = new SqlCommand("yaf_board_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_Category
		public bool category_delete(object CategoryID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_category_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@CategoryID",CategoryID);
				return (int)ExecuteScalar(cmd)!=0;
			}
		}
		public DataTable category_list(object boardID,object categoryID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_category_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@CategoryID",categoryID);
				return GetData(cmd);
			}
		}
		public void category_save(object boardID,object CategoryID,object Name,object SortOrder) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_category_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@CategoryID",CategoryID);
				cmd.Parameters.Add("@Name",Name);
				cmd.Parameters.Add("@SortOrder",SortOrder);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_CheckEmail
		public void checkemail_save(object UserID,object Hash,object Email) 
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
		public bool checkemail_update(object hash) 
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
		public void choice_vote(object choiceID) 
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
		public void forum_delete(object ForumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				ExecuteNonQuery(cmd);
			}
		}
		public DataTable forum_list(object boardID,object ForumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@ForumID",ForumID);
				return GetData(cmd);
			}
		}
		public DataTable forum_listall(object boardID,object userID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_listall")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@UserID",userID);
				return GetData(cmd);
			}
		}
		public DataTable forum_listpath(object forumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_listpath")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",forumID);
				return GetData(cmd);
			}
		}
		public DataTable forum_listread(object boardID,object UserID,object CategoryID,object parentID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_listread")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@CategoryID",CategoryID);
				cmd.Parameters.Add("@ParentID",parentID);
				return GetData(cmd);
			}
		}
		public DataTable forum_moderatelist() 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_moderatelist")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				//cmd.Parameters.Add("@UserID",UserID);
				return GetData(cmd);
			}
		}
		public DataTable forum_moderators() 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_moderators")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return GetData(cmd);
			}
		}
		public long forum_save(object ForumID,object CategoryID,object parentID,object Name,object Description,object SortOrder,object Locked,object Hidden,object IsTest,object moderated,object accessMaskID,bool dummy) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forum_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				cmd.Parameters.Add("@CategoryID",CategoryID);
				cmd.Parameters.Add("@ParentID",parentID);
				cmd.Parameters.Add("@Name",Name);
				cmd.Parameters.Add("@Description",Description);
				cmd.Parameters.Add("@SortOrder",SortOrder);
				cmd.Parameters.Add("@Locked",Locked);
				cmd.Parameters.Add("@Hidden",Hidden);
				cmd.Parameters.Add("@IsTest",IsTest);
				cmd.Parameters.Add("@Moderated",moderated);
				cmd.Parameters.Add("@AccessMaskID",accessMaskID);
				return long.Parse(ExecuteScalar(cmd).ToString());
			}
		}
		#endregion

		#region yaf_ForumAccess
		public DataTable forumaccess_list(object ForumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forumaccess_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				return GetData(cmd);
			}
		}
		public void forumaccess_save(object ForumID,object GroupID,object accessMaskID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forumaccess_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				cmd.Parameters.Add("@GroupID",GroupID);
				cmd.Parameters.Add("@AccessMaskID",accessMaskID);
				ExecuteNonQuery(cmd);
			}
		}
		public DataTable forumaccess_group(object GroupID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_forumaccess_group")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@GroupID",GroupID);
				return GetData(cmd);
			}
		}
		#endregion

		#region yaf_Group
		public DataTable group_list(object boardID,object GroupID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_group_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@GroupID",GroupID);
				return GetData(cmd);
			}
		}
		public void group_delete(object GroupID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_group_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@GroupID",GroupID);
				ExecuteNonQuery(cmd);
			}
		}
		public DataTable group_member(object boardID,object UserID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_group_member"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@UserID",UserID);
				return GetData(cmd);
			}
		}
		public long group_save(object GroupID,object boardID,object Name,object IsAdmin,object IsGuest,object IsStart,object isModerator,object accessMaskID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_group_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@GroupID",GroupID);
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@Name",Name);
				cmd.Parameters.Add("@IsAdmin",IsAdmin);
				cmd.Parameters.Add("@IsGuest",IsGuest);
				cmd.Parameters.Add("@IsStart",IsStart);
				cmd.Parameters.Add("@IsModerator",isModerator);
				cmd.Parameters.Add("@AccessMaskID",accessMaskID);
				return long.Parse(ExecuteScalar(cmd).ToString());
			}
		}
		#endregion

		#region yaf_Mail
		public void mail_delete(object MailID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_mail_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MailID",MailID);
				ExecuteNonQuery(cmd);
			}
		}
		public DataTable mail_list() 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_mail_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return GetData(cmd);
			}
		}
		public void mail_createwatch(object topicID,object from,object subject,object body,object userID) 
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
		public DataTable post_list(object topicID,object updateViewCount) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_post_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",topicID);
				cmd.Parameters.Add("@UpdateViewCount",updateViewCount);
				return GetData(cmd);
			}
		}
		public DataTable post_list_reverse10(object topicID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_post_list_reverse10")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",topicID);
				return GetData(cmd);
			}
		}
		public DataTable post_last10user(object boardID,object userID,object pageUserID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_post_last10user")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@UserID",userID);
				cmd.Parameters.Add("@PageUserID",pageUserID);
				return GetData(cmd);
			}
		}
		//BAI ADDED 30.01.2004
		private void message_deleteRecursively(object messageID)
		{
			//Delete replies
			using(SqlCommand cmd = new SqlCommand("yaf_message_getReplies")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MessageID",messageID);
				DataTable tbReplies = GetData(cmd);
				foreach (DataRow row in tbReplies.Rows)
					message_deleteRecursively(row["MessageID"]);
			}
		  
			//Delete Message
			using(SqlCommand cmd = new SqlCommand("yaf_message_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MessageID",messageID);
				ExecuteNonQuery(cmd);
			}
		}
		public DataTable message_list(object messageID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_message_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MessageID",messageID);
				return GetData(cmd);
			}
		}
		public void message_delete(object messageID) 
		{
			message_deleteRecursively(messageID);
			/*
			using(SqlCommand cmd = new SqlCommand("yaf_message_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MessageID",messageID);
				ExecuteNonQuery(cmd);
			}
			*/
		}
		public void message_approve(object messageID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_message_approve")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@MessageID",messageID);
				ExecuteNonQuery(cmd);
			}
		}
		public void message_update(object messageID,object priority,object message) 
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
		public bool message_save(object TopicID,object UserID,object Message,object UserName,object IP,object posted,object replyTo,ref long nMessageID) 
		{
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
				cmd.Parameters.Add("@Posted",posted);
				cmd.Parameters.Add("@ReplyTo",replyTo);
				cmd.Parameters.Add(pMessageID);
				ExecuteNonQuery(cmd);
				nMessageID = (long)pMessageID.Value;
				return true;
			}
		}
		public DataTable message_unapproved(object forumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_message_unapproved")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",forumID);
				return GetData(cmd);
			}
		}
		public DataTable message_findunread(object topicID,object lastRead) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_message_findunread")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",topicID);
				cmd.Parameters.Add("@LastRead",lastRead);
				return GetData(cmd);
			}
		}
		#endregion

		#region yaf_NntpForum
		public DataTable nntpforum_list(object boardID,object minutes,object nntpForumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_nntpforum_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@Minutes",minutes);
				cmd.Parameters.Add("@NntpForumID",nntpForumID);
				return GetData(cmd);
			}
		}
		public void nntpforum_update(object nntpForumID,object lastMessageNo,object userID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_nntpforum_update")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@NntpForumID",nntpForumID);
				cmd.Parameters.Add("@LastMessageNo",lastMessageNo);
				cmd.Parameters.Add("@UserID",userID);
				ExecuteNonQuery(cmd);
			}
		}
		public void nntpforum_save(object nntpForumID,object nntpServerID,object groupName,object forumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_nntpforum_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@NntpForumID",nntpForumID);
				cmd.Parameters.Add("@NntpServerID",nntpServerID);
				cmd.Parameters.Add("@GroupName",groupName);
				cmd.Parameters.Add("@ForumID",forumID);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_NntpServer
		public DataTable nntpserver_list(object boardID,object nntpServerID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_nntpserver_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@NntpServerID",nntpServerID);
				return GetData(cmd);
			}
		}
		public void nntpserver_save(object nntpServerID,object boardID,object name,object address,object userName,object userPass) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_nntpserver_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@NntpServerID",nntpServerID);
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@Name",name);
				cmd.Parameters.Add("@Address",address);
				cmd.Parameters.Add("@UserName",userName);
				cmd.Parameters.Add("@UserPass",userPass);
				ExecuteNonQuery(cmd);
			}
		}
		public void nntpserver_delete(object nntpServerID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_nntpserver_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@NntpServerID",nntpServerID);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_NntpTopic
		public DataTable nntptopic_list(object thread) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_nntptopic_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@Thread",thread);
				return GetData(cmd);
			}
		}
		public void nntptopic_savemessage(object nntpForumID,object topic,object body,object userID,object userName,object ip,object posted,object thread) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_nntptopic_savemessage")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@NntpForumID",nntpForumID);
				cmd.Parameters.Add("@Topic",topic);
				cmd.Parameters.Add("@Body",body);
				cmd.Parameters.Add("@UserID",userID);
				cmd.Parameters.Add("@UserName",userName);
				cmd.Parameters.Add("@IP",ip);
				cmd.Parameters.Add("@Posted",posted);
				cmd.Parameters.Add("@Thread",thread);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_PMessage
		public DataTable pmessage_list(object toUserID,object fromUserID,object pMessageID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_pmessage_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ToUserID",toUserID);
				cmd.Parameters.Add("@FromUserID",fromUserID);
				cmd.Parameters.Add("@PMessageID",pMessageID);
				return GetData(cmd);
			}
		}
		public void pmessage_delete(object pMessageID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_pmessage_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@PMessageID",pMessageID);
				ExecuteNonQuery(cmd);
			}
		}
		public void pmessage_save(object fromUserID,object toUserID,object subject,object body) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_pmessage_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@FromUserID",fromUserID);
				cmd.Parameters.Add("@ToUserID",toUserID);
				cmd.Parameters.Add("@Subject",subject);
				cmd.Parameters.Add("@Body",body);
				ExecuteNonQuery(cmd);
			}
		}
		public void pmessage_markread(object userID,object pMessageID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_pmessage_markread")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				cmd.Parameters.Add("@PMessageID",pMessageID);
				ExecuteNonQuery(cmd);
			}
		}
		public DataTable pmessage_info() 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_pmessage_info")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return GetData(cmd);
			}
		}
		public void pmessage_prune(object daysRead,object daysUnread) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_pmessage_prune")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@DaysRead",daysRead);
				cmd.Parameters.Add("@DaysUnread",daysUnread);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_Poll
		public DataTable poll_stats(object pollID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_poll_stats")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@PollID",pollID);
				return GetData(cmd);
			}
		}
		public int poll_save(object question,object c1,object c2,object c3,object c4,object c5,object c6,object c7,object c8,object c9) 
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
		public DataTable rank_list(object boardID,object rankID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_rank_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@RankID",rankID);
				return GetData(cmd);
			}
		}
		public void rank_save(object RankID,object boardID,object Name,object IsStart,object IsLadder,object MinPosts,object RankImage) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_rank_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@RankID",RankID);
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@Name",Name);
				cmd.Parameters.Add("@IsStart",IsStart);
				cmd.Parameters.Add("@IsLadder",IsLadder);
				cmd.Parameters.Add("@MinPosts",MinPosts);
				cmd.Parameters.Add("@RankImage",RankImage);
				ExecuteNonQuery(cmd);
			}
		}
		public void rank_delete(object RankID) 
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
		public DataTable smiley_list(object boardID,object SmileyID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_smiley_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@SmileyID",SmileyID);
				return GetData(cmd);
			}
		}
		public DataTable smiley_listunique(object boardID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_smiley_listunique")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				return GetData(cmd);
			}
		}
		public void smiley_delete(object SmileyID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_smiley_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@SmileyID",SmileyID);
				ExecuteNonQuery(cmd);
			}
		}
		public void smiley_save(object SmileyID,object boardID,object Code,object Icon,object Emoticon,object Replace) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_smiley_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@SmileyID",SmileyID);
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@Code",Code);
				cmd.Parameters.Add("@Icon",Icon);
				cmd.Parameters.Add("@Emoticon",Emoticon);
				cmd.Parameters.Add("@Replace",Replace);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_System
		public void system_save(object TimeZone,object SmtpServer,object SmtpUserName,
			object SmtpUserPass,object ForumEmail,object EmailVerification,object ShowMoved,
			object BlankLinks,object showGroups,
			object AvatarWidth,object AvatarHeight,object avatarUpload,object avatarRemote,object avatarSize,
			object allowRichEdit,object allowUserTheme,object allowUserLanguage,
			object useFileTable,object maxFileSize) 
		{
			if(avatarSize!=null && avatarSize.ToString().Length==0)
				avatarSize = null;
			if(SmtpUserName!=null && SmtpUserName.ToString().Length==0)
				SmtpUserName = null;
			if(SmtpUserPass!=null && SmtpUserPass.ToString().Length==0)
				SmtpUserPass = null;

			using(SqlCommand cmd = new SqlCommand("yaf_system_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TimeZone",TimeZone);
				cmd.Parameters.Add("@SmtpServer",SmtpServer);
				cmd.Parameters.Add("@SmtpUserName",SmtpUserName);
				cmd.Parameters.Add("@SmtpUserPass",SmtpUserPass);
				cmd.Parameters.Add("@ForumEmail",ForumEmail);
				cmd.Parameters.Add("@EmailVerification",EmailVerification);
				cmd.Parameters.Add("@ShowMoved",ShowMoved);
				cmd.Parameters.Add("@BlankLinks",BlankLinks);
				cmd.Parameters.Add("@ShowGroups",showGroups);
				cmd.Parameters.Add("@AvatarWidth",AvatarWidth);
				cmd.Parameters.Add("@AvatarHeight",AvatarHeight);
				cmd.Parameters.Add("@AvatarUpload",avatarUpload);
				cmd.Parameters.Add("@AvatarRemote",avatarRemote);
				cmd.Parameters.Add("@AvatarSize",avatarSize);
				cmd.Parameters.Add("@AllowRichEdit",allowRichEdit);
				cmd.Parameters.Add("@AllowUserTheme",allowUserTheme);
				cmd.Parameters.Add("@AllowUserLanguage",allowUserLanguage);
				cmd.Parameters.Add("@UseFileTable",useFileTable);
				cmd.Parameters.Add("@MaxFileSize",maxFileSize);
				ExecuteNonQuery(cmd);
			}
		}
		public DataTable system_list() 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_system_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return GetData(cmd);
			}
		}
		#endregion

		#region yaf_Topic
		public int topic_prune(object ForumID,object Days) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_prune")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				cmd.Parameters.Add("@Days",Days);
				return (int)ExecuteScalar(cmd);
			}
		}
		public DataTable topic_list(object ForumID,object Announcement,object Date,object offset,object count) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@ForumID",ForumID);
				cmd.Parameters.Add("@Announcement",Announcement);
				cmd.Parameters.Add("@Date",Date);
				cmd.Parameters.Add("@Offset",offset);
				cmd.Parameters.Add("@Count",count);
				return GetData(cmd);
			}
		}
		public void topic_move(object TopicID,object ForumID,object ShowMoved) 
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
		public DataTable topic_active(object boardID,object UserID,object Since) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_active")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@Since",Since);
				return GetData(cmd);
			}
		}
		public void topic_delete(object TopicID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",TopicID);
				ExecuteNonQuery(cmd);
			}
		}
		public DataTable topic_findprev(object topicID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_findprev")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",topicID);
				return GetData(cmd);
			}
		}
		public DataTable topic_findnext(object topicID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_findnext")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",topicID);
				return GetData(cmd);
			}
		}
		public void topic_lock(object topicID,object locked) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_lock")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",topicID);
				cmd.Parameters.Add("@Locked",locked);
				ExecuteNonQuery(cmd);
			}
		}
		public long topic_save(object ForumID,object Subject,object Message,object UserID,object Priority,object PollID,object UserName,object IP,object posted,ref long nMessageID) 
		{
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
				cmd.Parameters.Add("@Posted",posted);

				DataTable dt = GetData(cmd);
				nMessageID = long.Parse(dt.Rows[0]["MessageID"].ToString());
				return long.Parse(dt.Rows[0]["TopicID"].ToString());
			}
		}
		public DataRow topic_info(object topicID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_topic_info")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@TopicID",topicID);
				using(DataTable dt = GetData(cmd)) 
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
		public DataTable user_list(object boardID,object UserID,object Approved) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@Approved",Approved);
				return GetData(cmd);
			}
		}
		public void user_delete(object UserID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				ExecuteNonQuery(cmd);
			}
		}
		public void user_approve(object UserID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_approve")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				ExecuteNonQuery(cmd);
			}
		}
		public void user_suspend(object userID,object suspend) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_suspend")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				cmd.Parameters.Add("@Suspend",suspend);
				ExecuteNonQuery(cmd);
			}
		}
		public bool user_changepassword(object UserID,object oldpw,object newpw) 
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
		public void user_save(object UserID,object boardID,object UserName,object Password,object Email,object Hash,
			object Location,object HomePage,object TimeZone,object Avatar,
			object languageFile,object themeFile,object Approved,
			object msn,object yim,object aim,object icq,
			object realName,object occupation,object interests,object gender,object weblog) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@UserName",UserName);
				cmd.Parameters.Add("@Password",Password);
				cmd.Parameters.Add("@Email",Email);
				cmd.Parameters.Add("@Hash",Hash);
				cmd.Parameters.Add("@Location",Location);
				cmd.Parameters.Add("@HomePage",HomePage);
				cmd.Parameters.Add("@TimeZone",TimeZone);
				cmd.Parameters.Add("@Avatar",Avatar);
				cmd.Parameters.Add("@LanguageFile",languageFile);
				cmd.Parameters.Add("@ThemeFile",themeFile);
				cmd.Parameters.Add("@Approved",Approved);
				cmd.Parameters.Add("@MSN",msn);
				cmd.Parameters.Add("@YIM",yim);
				cmd.Parameters.Add("@AIM",aim);
				cmd.Parameters.Add("@ICQ",icq);
				cmd.Parameters.Add("@RealName",realName);
				cmd.Parameters.Add("@Occupation",occupation);
				cmd.Parameters.Add("@Interests",interests);
				cmd.Parameters.Add("@Gender",gender);
				cmd.Parameters.Add("@Weblog",weblog);
				ExecuteNonQuery(cmd);
			}
		}
		public void user_adminsave(object boardID,object UserID,object Name,object email,object isHostAdmin,object RankID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_adminsave")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@Name",Name);
				cmd.Parameters.Add("@Email",email);
				cmd.Parameters.Add("@IsHostAdmin",isHostAdmin);
				cmd.Parameters.Add("@RankID",RankID);
				ExecuteNonQuery(cmd);
			}
		}
		public DataTable user_emails(object boardID,object GroupID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_emails")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@GroupID",GroupID);

				return GetData(cmd);
			}
		}
		public DataTable user_accessmasks(object boardID,object userID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_accessmasks")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@UserID",userID);

				return GetData(cmd);
			}
		}
		public bool user_recoverpassword(object userName,object email,object password) 
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
		public object user_login(object boardID,object name,object password) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_login")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@Name",name);
				cmd.Parameters.Add("@Password",password);
				return ExecuteScalar(cmd);
			}
		}
		public DataTable user_avatarimage(object userID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_avatarimage")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				return GetData(cmd);
			}
		}
		public DataTable user_find(object boardID,bool filter,object userName,object email) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_find")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@Filter",filter);
				cmd.Parameters.Add("@UserName",userName);
				cmd.Parameters.Add("@Email",email);
				return GetData(cmd);
			}
		}
		public string user_getsignature(object userID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_getsignature")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				return ExecuteScalar(cmd).ToString();
			}
		}
		public void user_savesignature(object userID,object signature) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_savesignature")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				cmd.Parameters.Add("@Signature",signature);
				ExecuteNonQuery(cmd);
			}
		}
		public void user_saveavatar(object userID,System.IO.Stream stream) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_saveavatar")) 
			{
				byte[] data = new byte[stream.Length];
				stream.Seek(0,System.IO.SeekOrigin.Begin);
				stream.Read(data,0,(int)stream.Length);

				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				cmd.Parameters.Add("@AvatarImage",data);
				ExecuteNonQuery(cmd);
			}
		}
		public void user_deleteavatar(object userID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_deleteavatar")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				ExecuteNonQuery(cmd);
			}
		}
		public bool user_register(pages.ForumPage page,object boardID,object userName,object password,object email,object location,object homePage,object timeZone,bool emailVerification) 
		{
			string hashinput = DateTime.Now.ToString() + email.ToString() + pages.register.CreatePassword(20);
			string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput,"md5");

			using(SqlTransaction trans = Conn.BeginTransaction()) 
			{
				try 
				{
					using(SqlCommand cmd = new SqlCommand("yaf_user_save",Conn)) 
					{
						cmd.Transaction = trans;
						cmd.Connection = Conn;
						cmd.CommandType = CommandType.StoredProcedure;
						int UserID = 0;
						cmd.Parameters.Add("@UserID",UserID);
						cmd.Parameters.Add("@BoardID",boardID);
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
						string body = Utils.ReadTemplate("verifyemail.txt");
						body = body.Replace("{link}",String.Format("{1}{0}",Forum.GetLink(Pages.approve,"k={0}",hash),page.ServerURL));
						body = body.Replace("{key}",hash);
						body = body.Replace("{forumname}",Config.BoardSettings.Name);
						body = body.Replace("{forumlink}",String.Format("{0}",page.ForumURL));

						Utils.SendMail(Config.BoardSettings.ForumEmail,email.ToString(),String.Format("{0} email verification",Config.BoardSettings.Name),body);
						page.AddLoadMessage("A mail has been sent. Check your inbox and click the link in the mail.");
						trans.Commit();
					} 
					else 
					{
						trans.Commit();
					}
				}
				catch(Exception x) 
				{
					trans.Rollback();
					page.AddLoadMessage(x.Message);
					return false;
				}
			}
			return true;
		}
		public bool user_access(object userID,object forumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_access")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				cmd.Parameters.Add("@ForumID",forumID);
				using(DataTable dt = GetData(cmd)) 
				{
					foreach(DataRow row in dt.Rows) 
					{
						return long.Parse(row["ReadAccess"].ToString())>0;
					}
				}
			}
			return false;
		}
		public int user_guest() 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_guest")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return (int)ExecuteScalar(cmd);
			}
		}
		public DataTable user_activity_rank()
		{
			// This define the date since the posts are counted (can pass as parameter??)
			TimeSpan tsRange = new TimeSpan(30,0,0,0);
			DateTime StartDate = DateTime.Now.Subtract( tsRange );

			using(SqlCommand cmd = new SqlCommand("yaf_user_activity_rank"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@StartDate",StartDate);
				return GetData(cmd);
			}
		}
		#endregion

		#region yaf_UserForum
		public DataTable userforum_list(object userID,object forumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_userforum_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				cmd.Parameters.Add("@ForumID",forumID);
				return GetData(cmd);
			}
		}
		public void userforum_delete(object userID,object forumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_userforum_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				cmd.Parameters.Add("@ForumID",forumID);
				ExecuteNonQuery(cmd);
			}
		}
		public void userforum_save(object userID,object forumID,object accessMaskID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_userforum_save")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				cmd.Parameters.Add("@ForumID",forumID);
				cmd.Parameters.Add("@AccessMaskID",accessMaskID);
				ExecuteNonQuery(cmd);
			}
		}
		#endregion

		#region yaf_UserGroup
		public DataTable usergroup_list(object boardID,object UserID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_usergroup_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@BoardID",boardID);
				cmd.Parameters.Add("@UserID",UserID);
				return GetData(cmd);
			}
		}
		public void usergroup_save(object UserID,object GroupID,object Member) 
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

		#region yaf_UserPMessage
		public void userpmessage_delete(object userPMessageID)
		{
			using(SqlCommand cmd = new SqlCommand("yaf_userpmessage_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserPMessageID",userPMessageID);
				ExecuteNonQuery(cmd);
			}
		}
		public DataTable userpmessage_list(object userPMessageID)
		{
			using(SqlCommand cmd = new SqlCommand("yaf_userpmessage_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserPMessageID",userPMessageID);
				return GetData(cmd);
			}
		}
		#endregion

		#region yaf_WatchForum
		public void watchforum_add(object UserID,object ForumID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_watchforum_add")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",UserID);
				cmd.Parameters.Add("@ForumID",ForumID);
				ExecuteNonQuery(cmd);
			}
		}
		public DataTable watchforum_list(object userID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_watchforum_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				return GetData(cmd);
			}
		}
		public void watchforum_delete(object watchForumID) 
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
		public DataTable watchtopic_list(object userID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_watchtopic_list")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",userID);
				return GetData(cmd);
			}
		}
		public void watchtopic_delete(object watchTopicID) 
		{
			using(SqlCommand cmd = new SqlCommand("yaf_watchtopic_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@WatchTopicID",watchTopicID);
				ExecuteNonQuery(cmd);
			}
		}
		public void watchtopic_add(object userID,object topicID) 
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
