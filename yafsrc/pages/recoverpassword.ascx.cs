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
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
    public partial class recoverpassword : YAF.Classes.Base.ForumPage
    {
        public recoverpassword()
            : base("RECOVERPASSWORD")
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CanLogin)
                YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.forum);

            if (!IsPostBack)
            {
                PageLinks.AddLink(PageContext.BoardSettings.Name, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum));
            }
        }
    }
}