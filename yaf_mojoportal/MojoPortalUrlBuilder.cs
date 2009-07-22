using System;
using System.Web;
using yaf;
namespace yaf
{
    

    public class MojoPortalUrlBuilder : IUrlBuilder
    {
        public string BuildUrl(string url)
        {
            string pageURL = "/forumyaf.aspx";
            string currentPath=null;
            string serverPath=HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"];
            string tabID = HttpContext.Current.Request.QueryString["tabid"];
            if (serverPath.Contains(Config.Root))
                currentPath = serverPath;
            else
                currentPath = Config.Root + serverPath;
            //currentPath = pageURL;
            //serverPath - string str = string.Concat("/Modules/yaf",HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"]);
            //tabID string str2 = HttpContext.Current.Request.QueryString["tabid"];
           

            //str = "/Modules/yaf/";
            if (tabID != null)
            {
                return string.Format("{0}?tabid={1}&{2}", currentPath, tabID, url);
            }
            return string.Format("{0}?{1}", currentPath, url);
        }
    }
}

