using System;
using System.Web;

namespace yaf
{
	public interface IUrlBuilder
	{
		string BuildUrl(string url);
	}

	public class UrlBuilder : IUrlBuilder
	{
		public string BuildUrl(string url)
		{
			return string.Format("{0}?{1}",HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"],url);
		}
	}

	public class UrlBuilderRainbow : IUrlBuilder
	{
		public string BuildUrl(string url)
		{
			string	builturl;

			int tabID = 0;
			Rainbow.Configuration.PortalSettings currentSetting = null;
			if (HttpContext.Current.Items["PortalSettings"] != null)
			{
				currentSetting = (Rainbow.Configuration.PortalSettings)HttpContext.Current.Items["PortalSettings"];
				tabID = currentSetting.ActiveTab.TabID;
			}

			int pos = url.IndexOf('#');
			if(pos>0)
				builturl = Rainbow.HttpUrlBuilder.BuildUrl("~/" + Rainbow.HttpUrlBuilder.DefaultPage, tabID, 0, null, url.Substring(0,pos), string.Empty,string.Empty) + url.Substring(pos);
			else
				builturl = Rainbow.HttpUrlBuilder.BuildUrl("~/" + Rainbow.HttpUrlBuilder.DefaultPage, tabID, 0, null, url, string.Empty,string.Empty);
			
			return builturl;
		}
	}

	public class UrlBuilderDotNetNuke : IUrlBuilder
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
