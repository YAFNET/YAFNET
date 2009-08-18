/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
		static public string GetConfigValueAsString(string configKey)
		{
			foreach (string key in WebConfigurationManager.AppSettings.AllKeys)
			{
				if (key.Equals(configKey, StringComparison.CurrentCultureIgnoreCase))
				{
					return ConfigurationManager.AppSettings[key];
				}
			}

			return null;
		}

		static public bool GetConfigValueAsBool(string configKey, bool defaultValue)
		{
			string value = GetConfigValueAsString(configKey);

			if (!String.IsNullOrEmpty(value))
			{
				return Convert.ToBoolean(value.ToLower());
			}

			return defaultValue;
		}

		/// <summary>
		/// Current BoardID -- default is 1.
		/// </summary>
		static public string BoardID
		{
			get
			{
				return (GetConfigValueAsString("YAF.BoardID") ?? "1");
			}
		}

		/// <summary>
		/// Current CategoryID -- default is null.
		/// </summary>
		static public string CategoryID
		{
			get
			{
				return GetConfigValueAsString("YAF.CategoryID");
			}
		}

		/// <summary>
		/// Is Url Rewriting enabled? -- default is false.
		/// </summary>
		static public bool EnableURLRewriting
		{
			get
			{
				return GetConfigValueAsBool("YAF.EnableUrlRewriting", false);
			}
		}

		/// <summary>
		/// Used for Url Rewriting -- default is "default.aspx"
		/// </summary>
		static public string BaseScriptFile
		{
			get
			{
				return (GetConfigValueAsString("YAF.BaseScriptFile") ?? "default.aspx");
			}
		}

		/// <summary>
		/// Returns null if value does not exist.
		/// </summary>
		static public string BaseUrl
		{
			get
			{
				return GetConfigValueAsString("YAF.BaseURL");
			}
		}

		static public bool BaseUrlOverrideDomain
		{
			get
			{
				return GetConfigValueAsBool("YAF.BaseURLOverrideDomain", false);
			}
		}

		/// <summary>
		/// Directory to use for uploading -- default is "~/upload/"
		/// </summary>
		static public string UploadDir
		{
			get
			{
				return GetConfigValueAsString("YAF.UploadDir") ?? "~/upload/";
			}
		}

		static public string ProviderKeyType
		{
			get
			{
				return GetConfigValueAsString("YAF.ProviderKeyType") ?? "System.Guid";
			}
		}

		static public bool AllowLoginAndLogoff
		{
			get
			{
				return GetConfigValueAsBool("YAF.AllowLoginAndLogoff", true);
			}
		}

		static public string Root
		{
			get
			{
				return GetConfigValueAsString("YAF.Root");
			}
		}

		#region Database Settings
		static public string ConnectionStringName
		{
			get
			{
				return GetConfigValueAsString("YAF.ConnectionStringName") ?? "yafnet";
			}
		}

		static public string DatabaseOwner
		{
			get
			{
				return Config.GetConfigValueAsString("YAF.DatabaseOwner") ?? "dbo";
			}
		}
        
        static public string DatabaseObjectQualifier
		{
			get
			{
				return Config.GetConfigValueAsString("YAF.DatabaseObjectQualifier") ?? "yaf_";
			}
		}

        // Different data layers specific settings

       
        static public String DatabaseEncoding
        {          
            get          
            {              
                return Config.GetConfigValueAsString("YAF.DatabaseEncoding"); 
           
            }       
        }
    
        static public String DatabaseCollation       
        {            
            get            
            {              
                return Config.GetConfigValueAsString("YAF.DatabaseCollation");
           
            }        
        } 
       
        static public String SchemaName
        {
           get            
           {      
               return Config.GetConfigValueAsString("YAF.DatabaseSchemaName");       
           }        
        }

       static public String WithOIDs
       {            
          get           
          {           
              return Config.GetConfigValueAsString("YAF.DatabaseWithOIDs");
           
          }       
       }

		#endregion

		/// <summary>
		/// Display the default toolbar at the top -- default is "true"
		/// </summary>
		static public bool ShowToolBar
		{
			get
			{
				return GetConfigValueAsBool("YAF.ShowToolBar", true);
			}
		}

		/// <summary>
		/// Diisplay the footer at the bottom of the page -- default is "true"
		/// </summary>
		static public bool ShowFooter
		{
			get
			{
				return GetConfigValueAsBool("YAF.ShowFooter", true);
			}
		}

		/// <summary>
		/// Use an SSL connection for the SMTP server -- default is "false"
		/// </summary>
		static public bool UseSMTPSSL
		{
			get
			{
				return GetConfigValueAsBool("YAF.UseSMTPSSL", false);
			}
		}

		#region Telerik Rad Editor Settings
		static public string RadEditorSkin
		{
			get
			{
				return GetConfigValueAsString("YAF.RadEditorSkin") ?? "Vista";
			}
		}

		static public string RadEditorToolsFile
		{
			get
			{
				return GetConfigValueAsString("YAF.RadEditorToolsFile") ??
							 String.Format("{0}/editors/RadEditor/ToolsFile.xml", Config.Root);
			}
		}

		static public bool UseRadEditorToolsFile
		{
			get
			{
				string value = GetConfigValueAsString("YAF.UseRadEditorToolsFile");

				if (!String.IsNullOrEmpty(value))
				{
					switch (value.ToLower().Substring(0, 1))
					{
						case "1":
						case "t":
						case "y":
							return true;
						case "0":
						case "f":
						case "n":
							return false;
					}
				}

				return false;
			}
		}
		#endregion

		static public string ConnectionString
		{
			get
			{
				return ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
			}
		}

		[Obsolete("Legacy: Phasing out")]
		static public string LogToMail
		{
			get
			{
				return GetConfigValueAsString("YAF.LogToMail");
			}
		}

		static public bool IsDotNetNuke
		{
			get
			{
				object obj = HttpContext.Current.Items["PortalSettings"];
				return obj != null && obj.GetType().ToString().ToLower().IndexOf("dotnetnuke") >= 0;
			}
		}

		static public bool IsMojoPortal
		{
			get
			{
				object obj = HttpContext.Current.Items["SiteSettings"];
				return obj != null && obj.GetType().ToString().ToLower().IndexOf("mojoportal") >= 0;
			}
		}

		static public bool IsRainbow
		{
			get
			{
				object obj = HttpContext.Current.Items["PortalSettings"];
				return obj != null && obj.GetType().ToString().ToLower().IndexOf("rainbow") >= 0;
			}
		}

		static public bool IsPortal
		{
			get
			{
				return HttpContext.Current.Session["YetAnotherPortal.net"] != null;
			}
		}

		static public bool IsAnyPortal
		{
			get
			{
				return (IsDotNetNuke || IsRainbow || IsMojoPortal || IsPortal);
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
				if (HttpContext.Current.Application[UrlBuilderKeyName] == null)
				{
					string urlAssembly;

					if (IsRainbow)
					{
						urlAssembly = "yaf_rainbow.RainbowUrlBuilder,yaf_rainbow";
					}
					else if (IsDotNetNuke)
					{
						urlAssembly = "yaf_dnn.DotNetNukeUrlBuilder,yaf_dnn";
					}
					else if (IsMojoPortal)
					{
						urlAssembly = "yaf_mojo.MojoPortalUrlBuilder,yaf_mojo";
					}
					else if (IsPortal)
					{
						urlAssembly = "Portal.UrlBuilder,Portal";
					}
					else if (EnableURLRewriting)
					{
						urlAssembly = "YAF.Classes.RewriteUrlBuilder,YAF.Classes.Utils";
					}
					else
					{
						urlAssembly = "YAF.Classes.UrlBuilder";
					}

					HttpContext.Current.Application[UrlBuilderKeyName] = Activator.CreateInstance(Type.GetType(urlAssembly));
				}

				return (IUrlBuilder)HttpContext.Current.Application[UrlBuilderKeyName];
			}
		}
	}
}
