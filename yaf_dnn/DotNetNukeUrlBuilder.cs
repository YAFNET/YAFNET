#region Usings

using System;
using System.Web;
using YAF.Classes;

#endregion

namespace YAF.DotNetNuke
{
	public class DotNetNukeUrlBuilder : IUrlBuilder
	{
		#region IUrlBuilder Members

		public string BuildUrl( string url )
		{
			// escape & to &amp;
			url = url.Replace( "&", "&amp;" );

			string scriptname = HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"];
			string tabid = HttpContext.Current.Request.QueryString["tabid"];

			string builturl = tabid != null
                                  ? string.Format( "{0}?tabid={1}&{2}", scriptname, tabid, url )
                                  : string.Format( "{0}?{1}", scriptname, url );

			return builturl;
		}

		public string BuildUrlFull( string url )
		{
			return String.Format( "{0}{1}", UrlBuilder.BaseUrl, BuildUrl( url ) );
		}

		#endregion
	}
}