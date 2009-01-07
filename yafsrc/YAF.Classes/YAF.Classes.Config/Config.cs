/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Management;
using System.Reflection;

namespace YAF.Classes
{
	/// <summary>
	/// Static class that access the app settings in the web.config file
	/// </summary>
	public static class Config
	{
		static public string BoardID
		{
			get
			{
				return ( ConfigurationManager.AppSettings ["boardid"] ?? "1" );
			}
		}

		static public string CategoryID
		{
			get
			{
				return ConfigurationManager.AppSettings ["categoryid"];
			}
		}

		static public string EnableURLRewriting
		{
			get
			{
				return ( ConfigurationManager.AppSettings ["enableurlrewriting"] ?? "false" );
			}
		}

		static public string BaseScriptFile
		{
			get
			{
				return ( ConfigurationManager.AppSettings ["basescriptfile"] ?? "default.aspx" );
			}
		}

		static public string BaseUrl
		{
			get
			{
				return ( ConfigurationManager.AppSettings ["baseurl"] ?? null );
			}
		}

		static public bool BaseUrlOverrideDomain
		{
			get
			{
				if ( ConfigurationManager.AppSettings ["baseurloverridedomain"] != null &&
							ConfigurationManager.AppSettings ["baseurloverridedomain"].ToLower() == "true" )
					return true;

				return false;
			}
		}

		static public string UploadDir
		{
			get
			{
				return ( ConfigurationManager.AppSettings ["uploaddir"] ?? "~/upload/" );
			}
		}

		static public string ProviderKeyType
		{
			get
			{
				return ( ConfigurationManager.AppSettings ["providerkeytype"] ?? "System.Guid" );
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

		static public bool IsAnyPortal
		{
			get
			{
				return ( IsDotNetNuke || IsRainbow || IsPortal );
			}
		}

		static private string UrlBuilderKeyName
		{
			get
			{
				return "yaf_UrlBuilder-Board" + YafControlSettings.Current.BoardID.ToString();
			}
		}

		static public IUrlBuilder UrlBuilder
		{
			get
			{
				if ( HttpContext.Current.Application [ UrlBuilderKeyName ] == null )
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
					else if ( EnableURLRewriting == "true" )
					{
						urlAssembly = "YAF.Classes.RewriteUrlBuilder,YAF.Classes.Utils";
					}
					else
					{
						urlAssembly = "YAF.Classes.UrlBuilder";
					}

					HttpContext.Current.Application [UrlBuilderKeyName] = Activator.CreateInstance( Type.GetType( urlAssembly ) );
				}

				return ( IUrlBuilder ) HttpContext.Current.Application [UrlBuilderKeyName];
			}
		}

		static public bool ShowToolBar
		{
			get
			{
				bool result = true;

				if ( ConfigurationManager.AppSettings ["ShowToolBar"] != null &&
						ConfigurationManager.AppSettings ["ShowToolBar"].ToLower() == "false" )
					result = false;

				return result;
			}
		}
	}
}
