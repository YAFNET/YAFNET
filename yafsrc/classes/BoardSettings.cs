using System;
using System.Data;
using System.Collections;

namespace yaf
{
	public class BoardSettings
	{
		private DataRow m_board;
		private Hashtable m_reg, m_regBoard;

		public BoardSettings(object boardID)
		{
			DataTable dt;
			// get the board table
			dt = DB.board_list(boardID);
			m_board = dt.Rows[0];

			m_reg = new Hashtable();
			m_regBoard = new Hashtable();

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
			get	{ return Convert.ToString(m_board["Name"]);	}
		}
		public bool AllowThreaded
		{
			get	{	return Convert.ToBoolean(m_board["AllowThreaded"]); }
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
				int min = GetValueInt("TimeZone",0);
				return new TimeSpan(min/60,min%60,0);
			}
		}
		// int settings
		public int TimeZoneRaw
		{
			get { return GetValueInt("TimeZone",0);	}
			set	{	SetValueInt("TimeZone",value); }
		}
		public int AvatarWidth
		{
			get { return GetValueInt("AvatarWidth",50);	}
			set	{	SetValueInt("AvatarWidth",value); }
		}
		public int AvatarHeight
		{
			get { return GetValueInt("AvatarHeight",80);	}
			set	{	SetValueInt("AvatarHeight",value); }
		}
		public int AvatarSize
		{
			get { return GetValueInt("AvatarSize",50000);	}
			set	{	SetValueInt("AvatarSize",value); }
		}
		public int MaxFileSize
		{
			get { return GetValueInt("MaxFileSize",0);	}
			set	{	SetValueInt("MaxFileSize",value); }
		}
		public int SmiliesColumns
		{
			get { return GetValueInt("SmiliesColumns",3); }
			set { SetValueInt("SmiliesColumns",value); }
		}
		public int SmiliesPerRow
		{
			get { return GetValueInt("SmiliesPerRow",6); }
			set { SetValueInt("SmiliesPerRow",value); }
		}
		public int LockPosts
		{
			get { return GetValueInt("LockPosts",0); }
			set { SetValueInt("LockPosts",value); }
		}
		// boolean settings
		public bool EmailVerification
		{
			get	{	return GetValueBool("EmailVerification",false);	}
			set	{	SetValueBool("EmailVerification",value); }
		}
		public bool ShowMoved
		{
			get {	return GetValueBool("ShowMoved",true); }
			set	{ SetValueBool("ShowMoved",value); }
		}
		public bool ShowGroups 
		{
			get {	return GetValueBool("ShowGroups",true);	}
			set { SetValueBool("ShowGroups",value); }
		}
		public bool BlankLinks 
		{
			get {	return GetValueBool("BlankLinks",false); }
			set { SetValueBool("BlankLinks",value); }
		}
		public bool AllowRichEdit 
		{
			get {	return GetValueBool("AllowRichEdit",true); }
			set { SetValueBool("AllowRichEdit",value); }
		}
		public bool AllowUserTheme 
		{
			get { return GetValueBool("AllowUserTheme",false); }
			set { SetValueBool("AllowUserTheme",value); }
		}
		public bool AllowUserLanguage 
		{
			get { return GetValueBool("AllowUserLanguage",false); }
			set { SetValueBool("AllowUserLanguage",value); }
		}
		public bool AvatarUpload
		{
			get { return GetValueBool("AvatarUpload", false); }
			set { SetValueBool("AvatarUpload",value); }
		}
		public bool AvatarRemote
		{
			get { return GetValueBool("AvatarRemote", false); }
			set { SetValueBool("AvatarRemote",value); }
		}
		public bool UseFileTable
		{
			get { return GetValueBool("UseFileTable", false); }
			set { SetValueBool("UseFileTable",value); }
		}
		public bool ShowRSSLink
		{
			get { return GetValueBool("ShowRSSLink",true); }
			set { SetValueBool("ShowRSSLink",value); }
		}
		public bool ShowForumJump
		{
			get { return GetValueBool("ShowForumJump",true); }
			set { SetValueBool("ShowForumJump",value); }
		}
		public int MaxUsers
		{
			get 
			{ 
				if (m_regBoard["maxusers"] == null) return 1;
				return Convert.ToInt32(m_regBoard["maxusers"]);
			}
		}
		public DateTime MaxUsersWhen
		{
			get 
			{ 
				if(m_regBoard["maxuserswhen"] == null) return DateTime.Now;
				return DateTime.Parse(m_regBoard["maxuserswhen"].ToString());
			}
		}

		// string settings
		public string ForumEmail 
		{
			get {	return GetValueString("ForumEmail",""); }
			set { SetValueString("ForumEmail",value); }
		}
		public string SmtpServer 
		{
			get { return GetValueString("SmtpServer",null); }
			set { SetValueString("SmtpServer",value); }
		}
		public string SmtpUserName 
		{
			get { return GetValueString("SmtpUserName",null); }
			set { SetValueString("SmtpUserName",value); }
		}
		public string SmtpUserPass
		{
			get { return GetValueString("SmtpUserPass",null); }
			set { SetValueString("SmtpUserPass",value); }
		}
			
		// helper class functions
		protected int GetValueInt(string Name,int Default)
		{
			if (m_reg[Name.ToLower()] == null) return Default;
			return Convert.ToInt32(m_reg[Name.ToLower()]);
		}
		protected void SetValueInt(string Name,int Value)
		{
			m_reg[Name.ToLower()] = Convert.ToString(Value);
		}
		protected bool GetValueBool(string Name,bool Default)
		{
			if (m_reg[Name.ToLower()] == null) return Default;
			return Convert.ToBoolean(Convert.ToInt32(m_reg[Name.ToLower()]));
		}
		protected void SetValueBool(string Name,bool Value)
		{
			m_reg[Name.ToLower()] = Convert.ToString(Convert.ToInt32(Value));
		}
		protected string GetValueString(string Name,string Default)
		{	
			if (m_reg[Name.ToLower()] == null) return Default;
			return Convert.ToString(m_reg[Name.ToLower()]);		
		}
		protected void SetValueString(string Name,string Value)
		{
			m_reg[Name.ToLower()] = Value;		
		}
	}
}
