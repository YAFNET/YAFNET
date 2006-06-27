using System;
using System.Web;

namespace yaf_dnn
{
	public class DotNetNukeUrlBuilder : yaf.IUrlBuilder
	{
		public string BuildUrl(string fpURL)
		{
			string strNewURL;
			string strScriptName = HttpContext.Current.Request.Path;
			int i;

			i = strScriptName.LastIndexOf("/");
			if (i > 0)
			{
				strScriptName = strScriptName.Substring(i+1,(strScriptName.Length-i-1));
			}

			// see if we should use rewrite...
			if (strScriptName.ToLower() != "default.aspx")
			{
				// don't use rewrite
				string strTabID	= HttpContext.Current.Request.QueryString["tabid"];
				
				if (strTabID != null)
				{
					strNewURL = string.Format("{0}?tabid={1}&{2}",HttpContext.Current.Request.Path,strTabID,fpURL);
				}
				else
				{
					strNewURL = string.Format("{0}?{1}",HttpContext.Current.Request.Path,fpURL);
				}				
			}
			else
			{
				// use rewrite...
				strNewURL = BuildUrlRewrite(fpURL);
			}

			return strNewURL;
		}

		private string BuildUrlRewrite(string fpURL)
		{
			string strScriptName = HttpContext.Current.Request.Path;
			string strTabID	= HttpContext.Current.Request.QueryString["tabid"];
			string retURL = "";
			string tmpPath = "";
			string tmpAnchor = "";
			int i;

			// convert the URL parameters
			string newURL = fpURL.Replace("=","/").Replace("&","/");
			i = newURL.LastIndexOf("#");

			if (i > 0)
			{
				tmpAnchor = newURL.Substring(i,(newURL.Length-i));
				newURL = newURL.Substring(0,i);				
			}		

			i = strScriptName.LastIndexOf("/");
			if (i > 0)
			{
				tmpPath = strScriptName.Substring(0,i+1);
				strScriptName = strScriptName.Substring(i+1,(strScriptName.Length-i-1));
			}
			
			if (strTabID != null)
			{
				retURL = string.Format("{0}tabid/{1}/{2}/{3}{4}",tmpPath,strTabID,newURL,strScriptName,tmpAnchor);
			}
			else
			{
				retURL = string.Format("{0}{1}/{2}{3}",tmpPath,newURL,strScriptName,tmpAnchor);
			}

			return retURL;
		}
	}
}
