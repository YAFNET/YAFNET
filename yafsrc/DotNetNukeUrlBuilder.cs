using System;
using System.Web;

namespace yaf_dnn
{
	public class DotNetNukeUrlBuilder : yaf.IUrlBuilder
	{
		public string BuildUrl(string url)
		{
			string	scriptname	= HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"];
			string	tabid		= HttpContext.Current.Request.QueryString["tabid"];
			string	builturl;
			
			if(tabid!=null)
				builturl = string.Format("{0}?tabid={1}&{2}",scriptname,tabid,url);
			else
				builturl = string.Format("{0}?{1}",scriptname,url);

			return builturl;
		}
	}
}
