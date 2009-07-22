using mojoPortal.Web;
using mojoPortal.Web.UI;
using mojoPortal.Business;
using System;
using System.Web;
using System.Web.UI.WebControls;
using yaf;
using yaf.pages;
namespace yaf
{
    

    public class MojoPortalModule : SiteModuleControl
    {
        protected yaf.Forum forum1;
        private string m_email;
        private int m_userID;
        private string m_userName;
        protected Panel pnlModuleContent;
        private SiteUser siteUser;

        private void MojoPortalModule_Load(object sender, EventArgs e)
        {
            try
            {
                //this.Forum1.BoardID=int.Parse(base.get_Settings()["forumboardid"].ToString()));
                this.forum1.BoardID=1;
                //string s = base.get_Settings()["forumcategoryid"].ToString();
                string s = string.Empty;
                if (s != string.Empty)
                {
                    this.forum1.CategoryID=int.Parse(s);
                }
            }
            catch (Exception)
            {
                this.forum1.BoardID = 1;
            }
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
               // PortalSettings settings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
               // UserInfo info = UserController.GetUser(settings.get_PortalId(), base.get_UserId(), false);
                //SiteUtils.GetCurrentSiteUser();
               this.m_userID = SiteUtils.GetCurrentSiteUser().UserId;
               this.m_userName = SiteUtils.GetCurrentSiteUser().LoginName;
               this.m_email = SiteUtils.GetCurrentSiteUser().Email;                 
                //settings.get_AdministratorId();
               int userID = this.m_userID;
                string s = "";
            }
        }

        private void forum1_PageTitleSet(object sender, ForumPageArgs e)
        {
            //this.BasePage.Title = e.get_Title() + " - " + this.BasePage.Title;
        }

        protected override void OnInit(EventArgs e)
        {
            base.Load += new EventHandler(this.MojoPortalModule_Load);
            //this.forum1.PageTitleSet(new EventHandler<ForumPageArgs>(this.Forum1_PageTitleSet));
            base.OnInit(e);
        }

        

        
    }
}

