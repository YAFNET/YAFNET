using System;
using System.Web;

namespace yaf_rainbow
{
	public class RainbowUrlBuilder : yaf.IUrlBuilder
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
}
