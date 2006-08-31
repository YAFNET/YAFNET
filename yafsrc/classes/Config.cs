using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Management;
using System.Reflection;
using yaf.pages;

namespace yaf
{
	public static class Config
	{
		static public string BoardID
		{
			get
			{
				return ConfigurationManager.AppSettings ["boardid"];
			}
		}

		static public string CategoryID
		{
			get
			{
				return ConfigurationManager.AppSettings ["categoryid"];
			}
		}

		static public string UploadDir
		{
			get
			{
				return ConfigurationManager.AppSettings ["uploaddir"];
			}
		}

		static public string Root
		{
			get
			{
				return ConfigurationManager.AppSettings ["root"];
			}
		}

		static public string LogToMail
		{
			get
			{
				return ConfigurationManager.AppSettings ["logtomail"];
			}
		}

		static public string ConnectionString
		{
			get
			{
				return ConfigurationManager.ConnectionStrings ["yafnet"].ConnectionString;
			}
		}

		static public bool IsDotNetNuke
		{
			get
			{
				object obj = HttpContext.Current.Items ["PortalSettings"];
				return obj != null && obj.GetType().ToString().ToLower().IndexOf( "dotnetnuke" ) >= 0;
			}
		}

		static public bool IsRainbow
		{
			get
			{
				object obj = HttpContext.Current.Items ["PortalSettings"];
				return obj != null && obj.GetType().ToString().ToLower().IndexOf( "rainbow" ) >= 0;
			}
		}

		static public bool IsPortal
		{
			get
			{
				return HttpContext.Current.Session ["YetAnotherPortal.net"] != null;
			}
		}

		static public IUrlBuilder UrlBuilder
		{
			get
			{
				if ( HttpContext.Current.Application ["yaf_UrlBuilder"] == null )
				{
					string urlAssembly;

					if ( IsRainbow )
					{
						urlAssembly = "yaf_rainbow.RainbowUrlBuilder,yaf_rainbow";
					}
					else if ( IsDotNetNuke )
					{
						urlAssembly = "yaf_dnn.DotNetNukeUrlBuilder,yaf_dnn";
					}
					else if ( IsPortal )
					{
						urlAssembly = "Portal.UrlBuilder,Portal";
					}
					else
					{
						urlAssembly = "yaf.UrlBuilder,yaf";
					}

					HttpContext.Current.Application ["yaf_UrlBuilder"] = Activator.CreateInstance( Type.GetType( urlAssembly ) );
				}

				return ( IUrlBuilder ) HttpContext.Current.Application ["yaf_UrlBuilder"];
			}
		}
	}
}
