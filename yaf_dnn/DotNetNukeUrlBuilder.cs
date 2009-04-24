using System;
using System.Web;
using YAF.Classes;

namespace yaf_dnn
{
	public class DotNetNukeUrlBuilder : YAF.Classes.IUrlBuilder
	{
		public string BuildUrl( string url )
		{
			// escape & to &amp;
			url = url.Replace( "&", "&amp;" );

			string scriptname = HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"];
			string tabid = HttpContext.Current.Request.QueryString["tabid"];

			string builturl = tabid != null ? string.Format( "{0}?tabid={1}&{2}", scriptname, tabid, url ) : string.Format( "{0}?{1}", scriptname, url );

			return builturl;
		}
	}
}
