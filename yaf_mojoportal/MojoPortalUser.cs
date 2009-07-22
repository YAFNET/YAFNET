using System;
using System.Data.SqlClient;
using System.Web;
using yaf;
using mojoPortal.Business;
using mojoPortal.Web;

namespace yaf
{
   
    

    public class MojoPortalUser : IForumUser
    {
        private string m_email;
        private string m_firstName;
        private bool m_isAuthenticated = false;
        private bool m_isSuperAdmin = false;
        private string m_lastName;
        private string m_location;
        private int m_userID;
        private string m_userName = "";
        private SiteUser siteUser;
        public MojoPortalUser()
        {
            try
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    //UserInfo info = UserController.GetUserByName(((PortalSettings) HttpContext.Current.Items["PortalSettings"]).get_PortalId(), HttpContext.Current.User.Identity.Name, false);
                    siteUser = SiteUtils.GetCurrentSiteUser();
                    this.m_userID = siteUser.UserId;
                    this.m_userName = siteUser.LoginName;
                    this.m_email = siteUser.Email;
                    this.m_firstName = siteUser.Name;
                    this.m_lastName = "";
                    this.m_location = siteUser.Country;
                    this.m_isSuperAdmin = siteUser.IsInRoles("Administrators");
                    this.m_isAuthenticated = true;
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to find user info from mojoPortal.", exception);
            }
        }

        public void UpdateUserInfo(int userID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = string.Format("update yaf_User set Email='{0}' where UserID={1}", this.m_email, userID);
                DB.ExecuteNonQuery(command);
                if (this.m_isSuperAdmin)
                {
                    command.CommandText = string.Format("update yaf_User set Flags = Flags | 3 where UserID={0}", userID);
                    DB.ExecuteNonQuery(command);
                }
            }
        }

        public bool CanLogin
        {
            get
            {
                return false;
            }
        }

        public string Email
        {
            get
            {
                return this.m_email;
            }
        }

        public object HomePage
        {
            get
            {
                return null;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return this.m_isAuthenticated;
            }
        }

        public object Location
        {
            get
            {
                return this.m_location;
            }
        }

        public string Name
        {
            get
            {
                return this.m_userName;
            }
        }
    }
}

