using System;
using System.Web;
using yaf;

namespace yaf_dnn
{
	public class DotNetNukeUser : yaf.IForumUser
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

		public void Initialize(string userName,bool isAuthenticated)
		{
			try
			{
				if(isAuthenticated)
				{
					DotNetNuke.PortalSettings _portalSettings = (DotNetNuke.PortalSettings)HttpContext.Current.Items["PortalSettings"];

					System.Data.IDataReader dr;
					if(HttpContext.Current.User.Identity.GetType()==typeof(System.Security.Principal.WindowsIdentity))
						dr = DotNetNuke.DataProvider.Instance().GetUserByUsername(_portalSettings.PortalId,HttpContext.Current.User.Identity.Name);
					else
						dr = DotNetNuke.DataProvider.Instance().GetUser(_portalSettings.PortalId,int.Parse(HttpContext.Current.User.Identity.Name));
					
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
}
