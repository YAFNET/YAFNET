using System;
using System.Web;

namespace yaf
{
	public interface IForumUser
	{
		string Name
		{
			get;
		}
		string Email
		{
			get;
		}
		bool IsAuthenticated
		{
			get;
		}
		object Location
		{
			get;
		}
		object HomePage
		{
			get;
		}
		bool CanLogin
		{
			get;
		}
		void UpdateUserInfo(int userID);
	}

	public class RainbowUser : IForumUser
	{
		private	string	m_userName;
		private string	m_email;
		private	string	m_location;
		private int		m_userID;
		private bool	m_isAuthenticated;

		public RainbowUser(string userName,bool isAuthenticated)
		{
			/*
			 * UserID (int)=1
			 * Email (nvarchar)=bh@bhenden.org
			 * Password (nvarchar)=altchs
			 * Name (nvarchar)=bhenden
			 * Company (nvarchar)=
			 * Address (nvarchar)=Engsoleia 13
			 * City (nvarchar)=Kristiansund
			 * Zip (nvarchar)=6518
			 * CountryId (nchar)=NO
			 * StateId (int)=9889982
			 * PIva (nvarchar)=
			 * CFiscale (nvarchar)=
			 * Phone (nvarchar)=71583338
			 * Fax (nvarchar)=
			 * SendNewsletter (bit)=False
			 * MailChecked (tinyint)=
			 * PortalId (int)=0
			 * Country (nvarchar)=Norvegia
			 */
			try 
			{
				if(isAuthenticated)
				{
					m_userName = Rainbow.Configuration.PortalSettings.CurrentUser.Identity.Name;
					m_email = Rainbow.Configuration.PortalSettings.CurrentUser.Identity.Email;
					m_userID = Convert.ToInt32(Rainbow.Configuration.PortalSettings.CurrentUser.Identity.ID);

					Rainbow.Configuration.PortalSettings portalSettings = (Rainbow.Configuration.PortalSettings)HttpContext.Current.Items["PortalSettings"];
					Rainbow.Security.UsersDB usersDB = new Rainbow.Security.UsersDB();
					System.Data.SqlClient.SqlDataReader dr = usersDB.GetSingleUser(m_email,portalSettings.PortalID);
					if(dr.Read())
					{
						m_userName	= dr["Name"].ToString();
						m_email		= dr["Email"].ToString();
						m_userID	= (int)dr["UserID"];
						m_location	= dr["Country"].ToString();
					}
					dr.Close();

					m_isAuthenticated = true;
					return;
				} 
			}
			catch(Exception x) 
			{
				throw new Exception("Failed to read user data from Rainbow.",x);
			}
			m_userName = "";
			m_email = "";
			m_isAuthenticated = false;
		}

		public string Name
		{
			get
			{
				return m_userName;
			}
		}

		public string Email
		{
			get
			{
				return m_email;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return m_isAuthenticated;
			}
		}
		public object Location
		{
			get
			{
				return m_location;
			}
		}
		public object HomePage
		{
			get
			{
				return null;
			}
		}
		public bool CanLogin
		{
			get
			{
				return false;
			}
		}
		public void UpdateUserInfo(int userID) 
		{
			using(System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
			{
				cmd.CommandText = string.Format("update yaf_User set Email='{0}' where UserID={1}",m_email,userID);
				DB.ExecuteNonQuery(cmd);
			}
		}
	}

	public class DotNetNukeUser : IForumUser
	{
		private	int		m_userID;
		private	string	m_userName;
		private string	m_email;
		private	string	m_firstName;
		private string	m_lastName;
		private string	m_location;
		private bool	m_isAuthenticated;

		/*
		 * UserID (int)=2
		 * Username (nvarchar)=host
		 * Password (nvarchar)=host
		 * Email (nvarchar)=host
		 * FullName (nvarchar)=Host Account
		 * FirstName (nvarchar)=Host
		 * LastName (nvarchar)=Account
		 * Unit (nvarchar)=
		 * Street (nvarchar)=
		 * City (nvarchar)=
		 * Region (nvarchar)=
		 * PostalCode (nvarchar)=
		 * Country (nvarchar)=
		 * Telephone (nvarchar)=
		 * IsSuperUser (bit)=True
		 * Authorized (bit)=
		 * CreatedDate (datetime)=
		 * LastLoginDate (datetime)=
		 */

		public DotNetNukeUser(string userName,bool isAuthenticated)
		{
			try
			{
				if(isAuthenticated)
				{
					DotNetNuke.UsersDB objUser = new DotNetNuke.UsersDB();
					DotNetNuke.PortalSettings _portalSettings = (DotNetNuke.PortalSettings)HttpContext.Current.Items["PortalSettings"];

					System.Data.SqlClient.SqlDataReader dr;
					if(HttpContext.Current.User.Identity.GetType()==typeof(System.Security.Principal.WindowsIdentity))
						dr = objUser.GetSingleUserByUsername(_portalSettings.PortalId,HttpContext.Current.User.Identity.Name);
					else
						dr = objUser.GetSingleUser(_portalSettings.PortalId,int.Parse(HttpContext.Current.User.Identity.Name));
					
					if(dr.Read())
					{
						m_userID			= (int)dr["UserId"];
						m_userName			= dr["Username"].ToString();
						m_email				= dr["Email"].ToString();
						m_firstName			= dr["FirstName"].ToString();
						m_lastName			= dr["LastName"].ToString();
						m_location			= dr["Country"].ToString();
					}
					dr.Close();

					m_isAuthenticated = true;
					return;
				}
			}
			catch(Exception x)
			{
				throw new Exception("Failed to find user info from DotNetNuke.",x);
			}
			m_userName = "";
			m_isAuthenticated = false;
		}

		public string Name
		{
			get
			{
				return m_userName;
			}
		}

		public string Email
		{
			get
			{
				return m_email;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return m_isAuthenticated;
			}
		}
	
		public object Location
		{
			get
			{
				return m_location;
			}
		}
		public object HomePage
		{
			get
			{
				return null;
			}
		}
		public bool CanLogin
		{
			get
			{
				return false;
			}
		}
		public void UpdateUserInfo(int userID) 
		{
			using(System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
			{
				cmd.CommandText = string.Format("update yaf_User set Email='{0}' where UserID={1}",m_email,userID);
				DB.ExecuteNonQuery(cmd);
			}
		}
	}

	public class WindowsUser : IForumUser
	{
		private	string	m_userName;
		private	string	m_email;
		private	bool	m_isAuthenticated;

		public WindowsUser(string userName,bool isAuthenticated)
		{
			try
			{
				if(isAuthenticated)
				{
					string[] parts = userName.Split('\\');
					m_userName  = parts[parts.Length-1];
					if(parts.Length>1)
						m_email = String.Format("{0}@{1}",parts[1],parts[0]);
					else
						m_email = m_userName;
					
					m_isAuthenticated = true;
				}
			}
			catch(Exception)
			{
			}
			m_userName = "";
			m_email = "";
			m_isAuthenticated = false;
		}

		public string Name
		{
			get
			{
				return m_userName;
			}
		}

		public string Email
		{
			get
			{
				return m_email;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return m_isAuthenticated;
			}
		}
	
		public object Location
		{
			get
			{
				return null;
			}
		}
		public object HomePage
		{
			get
			{
				return null;
			}
		}
		public bool CanLogin
		{
			get
			{
				return false;
			}
		}
		public void UpdateUserInfo(int userID) 
		{
		}
	}
	
	public class FormsUser : IForumUser
	{
		private	string	m_userName;
		private	int		m_userID;
		private	int		m_boardID;
		private	bool	m_isAuthenticated;

		public FormsUser(string userName,bool isAuthenticated)
		{
			try
			{
				if(isAuthenticated)
				{
					string[] parts = userName.Split(';');
					if(parts.Length==3)
					{
						m_userID			= int.Parse(parts[0]);
						m_boardID			= int.Parse(parts[1]);
						m_userName			= parts[2];
						m_isAuthenticated	= true;
						return;
					}
				}
			}
			catch(Exception)
			{
			}
			m_userName			= "";
			m_userID			= 0;
			m_boardID			= 0;
			m_isAuthenticated	= false;
		}

		public string Name
		{
			get
			{
				return m_userName;
			}
		}

		public string Email
		{
			get
			{
				return "";
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return m_isAuthenticated;
			}
		}
	
		public object Location
		{
			get
			{
				return null;
			}
		}
		public object HomePage
		{
			get
			{
				return null;
			}
		}
		public bool CanLogin
		{
			get
			{
				return true;
			}
		}
		public void UpdateUserInfo(int userID) 
		{
		}
	}
	public class GuestUser : IForumUser
	{
		public string Name
		{
			get
			{
				return "";
			}
		}

		public string Email
		{
			get
			{
				return "";
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return false;
			}
		}
	
		public object Location
		{
			get
			{
				return null;
			}
		}
		public object HomePage
		{
			get
			{
				return null;
			}
		}
		public bool CanLogin
		{
			get
			{
				return true;
			}
		}
		public void UpdateUserInfo(int userID) 
		{
		}
	}
}
