using System;
using System.Web;

namespace yaf_rainbow
{
	public class RainbowUser : yaf.IForumUser
	{
		private	string	m_userName;
		private string	m_email;
		private	string	m_location;
		private int		m_userID;
		private bool	m_isAuthenticated;

		public void Initialize(string userName,bool isAuthenticated)
		{
			/*
			 * UserID (int)
			 * Email (nvarchar)
			 * Password (nvarchar)
			 * Name (nvarchar)
			 * Company (nvarchar)
			 * Address (nvarchar)
			 * City (nvarchar)
			 * Zip (nvarchar)
			 * CountryId (nchar)
			 * StateId (int)
			 * PIva (nvarchar)
			 * CFiscale (nvarchar)
			 * Phone (nvarchar)
			 * Fax (nvarchar)
			 * SendNewsletter (bit)
			 * MailChecked (tinyint)
			 * PortalId (int)
			 * Country (nvarchar)
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
				yaf.DB.ExecuteNonQuery(cmd);
			}
		}
	}

}
