using System;
using System.Web;

namespace YAF.Classes.Utils
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
}
