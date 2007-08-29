/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2007 Jaben Cargman
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
						urlAssembly = "YAF.Classes.UrlBuilder";
					}

					HttpContext.Current.Application ["yaf_UrlBuilder"] = Activator.CreateInstance( Type.GetType( urlAssembly ) );
				}

				return ( IUrlBuilder ) HttpContext.Current.Application ["yaf_UrlBuilder"];
			}
		}
	}
}
