using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using mojoPortal.Web;

namespace yaf_mojo
{
    public partial class YafModule : mojoBasePage
    {
        public void Page_Error(object sender, System.EventArgs e)
        {
            Exception x = Server.GetLastError();
            string exceptionInfo = "";
            while (x != null)
            {
                exceptionInfo += DateTime.Now.ToString("g");
                exceptionInfo += " in " + x.Source + "\r\n";
                exceptionInfo += x.Message + "\r\n" + x.StackTrace + "\r\n-----------------------------\r\n";
                x = x.InnerException;
            }
            //yaf.DB.eventlog_create(forum.PageUserID, this, exceptionInfo);
            yaf.Utils.LogToMail(x);
        }
    }

}
