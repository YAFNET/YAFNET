using System; 
using System.Web; 
using yaf;

namespace yaf_dnn
{ 
	public class DotNetNukeUser : yaf.IForumUser 
	{ 
		private int m_userID; 
		private string m_userName; 
		private string m_email; 
		private string m_firstName; 
		private string m_lastName; 
		private string m_location; 
		private bool m_isAuthenticated;
		private bool m_isSuperAdmin;

		public DotNetNukeUser() 
		{
			m_isAuthenticated = false; 
			m_isSuperAdmin = false;
			m_userName = "";

			try 
			{ 
				if(HttpContext.Current.User.Identity.IsAuthenticated) 
				{ 
					DotNetNuke.Entities.Users.UserInfo userInfo; 
					DotNetNuke.Entities.Portals.PortalSettings _portalSettings = (DotNetNuke.Entities.Portals.PortalSettings)HttpContext.Current.Items["PortalSettings"];
					userInfo = DotNetNuke.Entities.Users.UserController.GetUserByName( _portalSettings.PortalId, HttpContext.Current.User.Identity.Name, false );

					m_userID = userInfo.UserID; 
					m_userName = userInfo.Username; 
					m_email = userInfo.Membership.Email; 
					m_firstName = userInfo.FirstName; 
					m_lastName = userInfo.LastName; 
					m_location = userInfo.Profile.Country; 
					m_isSuperAdmin = userInfo.IsSuperUser;
					m_isAuthenticated = true;
				} 
			} 
			catch(Exception x) 
			{
				throw new Exception("Failed to find user info from DotNetNuke.",x); 
			}
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

				// if this user is super admin (host user) -- make sure they have permissions...
				if (m_isSuperAdmin)
				{
					cmd.CommandText = string.Format("update yaf_User set Flags = Flags | 3 where UserID={0}",userID);
					DB.ExecuteNonQuery(cmd);
				}
			} 
		} 
	} 
}