using System;
using System.Data;

namespace yaf
{
	public class BoardSettings
	{
		private DataRow m_row;

		public BoardSettings(object boardID)
		{
			using(DataTable dt = DB.board_list(boardID))
			{
				foreach(DataRow dr in dt.Rows)
				{
					m_row = dr;
					break;
				}
			}
		}
		public string Name
		{
			get
			{
				return (string)m_row["Name"];
			}
		}
		public TimeSpan TimeZone
		{
			get
			{
				int min = (int)m_row["TimeZone"];
				return new TimeSpan(min/60,min%60,0);
			}
		}
		public bool EmailVerification
		{
			get
			{
				return (bool)m_row["EmailVerification"];
			}
		}
		public bool ShowMoved
		{
			get 
			{
				return (bool)m_row["ShowMoved"];
			}
		}
		public bool ShowGroups 
		{
			get 
			{
				return (bool)m_row["ShowGroups"];
			}
		}
		public bool BlankLinks 
		{
			get 
			{
				return (bool)m_row["BlankLinks"];
			}
		}
		public bool AllowRichEdit 
		{
			get 
			{
				return (bool)m_row["AllowRichEdit"];
			}
		}
		public bool AllowUserTheme 
		{
			get 
			{
				return (bool)m_row["AllowUserTheme"];
			}
		}
		public bool AllowUserLanguage 
		{
			get 
			{
				return (bool)m_row["AllowUserLanguage"];
			}
		}
		public bool AllowThreaded
		{
			get
			{
				return (bool)m_row["AllowThreaded"];
			}
		}
		public string ForumEmail 
		{
			get 
			{
				return (string)m_row["ForumEmail"];
			}
		}
		public string SmtpServer 
		{
			get 
			{
				return (string)m_row["SmtpServer"];
			}
		}
		public string SmtpUserName 
		{
			get 
			{
				return m_row["SmtpUserName"]!=DBNull.Value ? (string)m_row["SmtpUserName"] : null;
			}
		}
		public string SmtpUserPass
		{
			get 
			{
				return m_row["SmtpUserPass"]!=DBNull.Value ? (string)m_row["SmtpUserPass"] : null;
			}
		}
		public bool AvatarUpload
		{
			get
			{
				return (bool)m_row["AvatarUpload"];
			}
		}
		public bool AvatarRemote
		{
			get
			{
				return (bool)m_row["AvatarRemote"];
			}
		}
		public int AvatarWidth
		{
			get
			{
				return (int)m_row["AvatarWidth"];
			}
		}
		public int AvatarHeight
		{
			get
			{
				return (int)m_row["AvatarHeight"];
			}
		}
	}
}
