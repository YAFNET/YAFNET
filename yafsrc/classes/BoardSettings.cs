using System;
using System.Data;
using System.Collections;

namespace yaf
{
	public class BoardSettings
	{
		private DataRow m_board;
		private RegistryHash m_reg, m_regBoard;

		public BoardSettings(object boardID)
		{
			DataTable dt;
			// get the board table
			dt = DB.board_list(boardID);
			m_board = dt.Rows[0];

			m_reg = new RegistryHash();
			m_regBoard = new RegistryHash();

			// get all the registry values for the forum
			using(dt = DB.registry_list())
			{
				// get all the registry settings into our hash table
				foreach(DataRow dr in dt.Rows)
				{
					if (dr["Value"] == DBNull.Value)
					{
						m_reg.Add(dr["Name"].ToString().ToLower(),null);
					}
					else
					{
						m_reg.Add(dr["Name"].ToString().ToLower(),dr["Value"]);
					}
				}
			}
			using(dt = DB.registry_list(null,boardID))
			{
				// get all the registry settings into our hash table
				foreach(DataRow dr in dt.Rows)
				{
					if (dr["Value"] == DBNull.Value)
					{
						m_regBoard.Add(dr["Name"].ToString().ToLower(),null);
					}
					else
					{
						m_regBoard.Add(dr["Name"].ToString().ToLower(),dr["Value"]);
					}
				}
			}
		}

		/// <summary>
		/// Saves the whole setting registry to the database.
		/// </summary>
		public void SaveRegistry()
		{
			// loop through all values and commit them to the DB
			foreach (DictionaryEntry objItem in m_reg)
			{
				DB.registry_save(objItem.Key,objItem.Value);
			}
		}

		// individual board settings
		public string Name
		{
			get	{ return m_board["Name"].ToString();	}
		}
		public bool AllowThreaded
		{
			get	{	return Convert.ToBoolean(m_board["AllowThreaded"].ToString()); }
		}
		public int MaxUsers
		{
			get { return m_regBoard.GetValueInt("MaxUsers",1); }
		}
		public DateTime MaxUsersWhen
		{
			get { return DateTime.Parse(m_regBoard.GetValueString("MaxUsersWhen",DateTime.Now.ToString()));	}
		}

		// didn't know where else to put this :)
		public string SQLVersion
		{
			get { return Convert.ToString(m_board["SQLVersion"]); }
		}

		// global forum settings from registry
		public TimeSpan TimeZone
		{
			get
			{
				int min = TimeZoneRaw;
				return new TimeSpan(min/60,min%60,0);
			}
		}
		// int settings
		public int TimeZoneRaw
		{
			get { return m_reg.GetValueInt("TimeZone",0);	}
			set	{	m_reg.SetValueInt("TimeZone",value); }
		}
		public int AvatarWidth
		{
			get { return m_reg.GetValueInt("AvatarWidth",50);	}
			set	{	m_reg.SetValueInt("AvatarWidth",value); }
		}
		public int AvatarHeight
		{
			get { return m_reg.GetValueInt("AvatarHeight",80);	}
			set	{	m_reg.SetValueInt("AvatarHeight",value); }
		}
		public int AvatarSize
		{
			get { return m_reg.GetValueInt("AvatarSize",50000);	}
			set	{	m_reg.SetValueInt("AvatarSize",value); }
		}
		public int MaxFileSize
		{
			get { return m_reg.GetValueInt("MaxFileSize",0);	}
			set	{	m_reg.SetValueInt("MaxFileSize",value); }
		}
		public int SmiliesColumns
		{
			get { return m_reg.GetValueInt("SmiliesColumns",3); }
			set { m_reg.SetValueInt("SmiliesColumns",value); }
		}
		public int SmiliesPerRow
		{
			get { return m_reg.GetValueInt("SmiliesPerRow",6); }
			set { m_reg.SetValueInt("SmiliesPerRow",value); }
		}
		public int LockPosts
		{
			get { return m_reg.GetValueInt("LockPosts",0); }
			set { m_reg.SetValueInt("LockPosts",value); }
		}
		public int PostsPerPage
		{
			get { return m_reg.GetValueInt("PostsPerPage",20); }
			set { m_reg.SetValueInt("PostsPerPage",value); }
		}
		public int TopicsPerPage
		{
			get { return m_reg.GetValueInt("TopicsPerPage",15); }
			set { m_reg.SetValueInt("TopicsPerPage",value); }
		}
		public int ForumEditor
		{
			get { return m_reg.GetValueInt("ForumEditor",1); }
			set { m_reg.SetValueInt("ForumEditor",value); }
		}

		// boolean settings
		public bool EmailVerification
		{
			get	{	return m_reg.GetValueBool("EmailVerification",false);	}
			set	{	m_reg.SetValueBool("EmailVerification",value); }
		}
		public bool ShowMoved
		{
			get {	return m_reg.GetValueBool("ShowMoved",true); }
			set	{ m_reg.SetValueBool("ShowMoved",value); }
		}
		public bool ShowGroups 
		{
			get {	return m_reg.GetValueBool("ShowGroups",true);	}
			set { m_reg.SetValueBool("ShowGroups",value); }
		}
		public bool BlankLinks 
		{
			get {	return m_reg.GetValueBool("BlankLinks",false); }
			set { m_reg.SetValueBool("BlankLinks",value); }
		}
		public bool AllowUserTheme 
		{
			get { return m_reg.GetValueBool("AllowUserTheme",false); }
			set { m_reg.SetValueBool("AllowUserTheme",value); }
		}
		public bool AllowUserLanguage 
		{
			get { return m_reg.GetValueBool("AllowUserLanguage",false); }
			set { m_reg.SetValueBool("AllowUserLanguage",value); }
		}
		public bool AvatarUpload
		{
			get { return m_reg.GetValueBool("AvatarUpload", false); }
			set { m_reg.SetValueBool("AvatarUpload",value); }
		}
		public bool AvatarRemote
		{
			get { return m_reg.GetValueBool("AvatarRemote", false); }
			set { m_reg.SetValueBool("AvatarRemote",value); }
		}
		public bool UseFileTable
		{
			get { return m_reg.GetValueBool("UseFileTable", false); }
			set { m_reg.SetValueBool("UseFileTable",value); }
		}
		public bool ShowRSSLink
		{
			get { return m_reg.GetValueBool("ShowRSSLink",true); }
			set { m_reg.SetValueBool("ShowRSSLink",value); }
		}
		public bool ShowForumJump
		{
			get { return m_reg.GetValueBool("ShowForumJump",true); }
			set { m_reg.SetValueBool("ShowForumJump",value); }
		}
		public bool AllowPrivateMessages
		{
			get { return m_reg.GetValueBool("AllowPrivateMessages",true); }
			set { m_reg.SetValueBool("AllowPrivateMessages",value); }
		}
		public bool AllowEmailSending
		{
			get { return m_reg.GetValueBool("AllowEmailSending",true); }
			set { m_reg.SetValueBool("AllowEmailSending",value); }
		}
		public bool AllowSignatures
		{
			get { return m_reg.GetValueBool("AllowSignatures",true); }
			set { m_reg.SetValueBool("AllowSignatures",value); }
		}
		public bool RemoveNestedQuotes
		{
			get { return m_reg.GetValueBool("RemoveNestedQuotes",false); }
			set { m_reg.SetValueBool("RemoveNestedQuotes",value); }
		}
		public bool DateFormatFromLanguage
		{
			get { return m_reg.GetValueBool("DateFormatFromLanguage",false); }
			set { m_reg.SetValueBool("DateFormatFromLanguage",value); }
		}

		// string settings
		public string ForumEmail 
		{
			get {	return m_reg.GetValueString("ForumEmail",""); }
			set { m_reg.SetValueString("ForumEmail",value); }
		}
		public string SmtpServer 
		{
			get { return m_reg.GetValueString("SmtpServer",null); }
			set { m_reg.SetValueString("SmtpServer",value); }
		}
		public string SmtpUserName 
		{
			get { return m_reg.GetValueString("SmtpUserName",null); }
			set { m_reg.SetValueString("SmtpUserName",value); }
		}
		public string SmtpUserPass
		{
			get { return m_reg.GetValueString("SmtpUserPass",null); }
			set { m_reg.SetValueString("SmtpUserPass",value); }
		}
		public string AcceptedHTML
		{
			get { return m_reg.GetValueString("AcceptedHTML","br,hr,b,i,u,a,div,ol,ul,li,blockquote,img,span,p,em,strong,font,pre,h1,h2,h3,h4,h5,h6,address"); }
			set { m_reg.SetValueString("AcceptedHTML",value.ToLower()); }
		}
	}

	public class RegistryHash : System.Collections.Hashtable
	{		
		// helper class functions
		public int GetValueInt(string Name,int Default)
		{
			if (this[Name.ToLower()] == null) return Default;
			return Convert.ToInt32(this[Name.ToLower()]);
		}
		public void SetValueInt(string Name,int Value)
		{
			this[Name.ToLower()] = Convert.ToString(Value);
		}
		public bool GetValueBool(string Name,bool Default)
		{
			if (this[Name.ToLower()] == null) return Default;
			return Convert.ToBoolean(Convert.ToInt32(this[Name.ToLower()]));
		}
		public void SetValueBool(string Name,bool Value)
		{
			this[Name.ToLower()] = Convert.ToString(Convert.ToInt32(Value));
		}
		public string GetValueString(string Name,string Default)
		{	
			if (this[Name.ToLower()] == null) return Default;
			return Convert.ToString(this[Name.ToLower()]);		
		}
		public void SetValueString(string Name,string Value)
		{
			this[Name.ToLower()] = Value;		
		}
	}
													
}
