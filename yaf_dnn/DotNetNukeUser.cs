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

		public void Initialize(string userName,bool isAuthenticated) 
		{ 
			try 
			{ 
				if(isAuthenticated) 
				{ 
					DotNetNuke.UserController userController = new DotNetNuke.UserController(); 
					DotNetNuke.UserInfo userInfo; 
					DotNetNuke.PortalSettings _portalSettings = (DotNetNuke.PortalSettings)HttpContext.Current.Items["PortalSettings"]; 
					userInfo = userController.GetUser(_portalSettings.PortalId, int.Parse(HttpContext.Current.User.Identity.Name)); 

					m_userID = userInfo.UserID; 
					m_userName = userInfo.Username; 
					m_email = userInfo.Email; 
					m_firstName = userInfo.FirstName; 
					m_lastName = userInfo.LastName; 
					m_location = userInfo.Country; 

					m_isAuthenticated = true; 
					return; 
				} 
			} 
			catch(Exception x) 
			{ 
				m_isAuthenticated = false; 
				m_userName = "";
				throw new Exception("Failed to find user info from DotNetNuke.",x); 
			} 

			m_isAuthenticated = false; 
			m_userName = "";
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