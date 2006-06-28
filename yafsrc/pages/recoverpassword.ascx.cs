using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace yaf.pages
{
    public partial class recoverpassword : ForumPage
    {
        public recoverpassword()
            : base("RECOVERPASSWORD")
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CanLogin)
                Forum.Redirect(Pages.forum);

            if (!IsPostBack)
            {
                PageLinks.AddLink(BoardSettings.Name, Forum.GetLink(Pages.forum));
            }
        }
    }
}