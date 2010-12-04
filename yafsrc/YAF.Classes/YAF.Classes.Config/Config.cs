/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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

namespace YAF.Classes
{
  using Pattern;

  /// <summary>
  /// Static class that access the app settings in the web.config file
  /// </summary>
  public static class Config
  {
    /// <summary>
    /// Current BoardID -- default is 1.
    /// </summary>
    public static string BoardID
    {
      get
      {
        return GetConfigValueAsString("YAF.BoardID") ?? "1";
      }
    }

    /// <summary>
    /// Current CategoryID -- default is null.
    /// </summary>
    public static string CategoryID
    {
      get
      {
        return GetConfigValueAsString("YAF.CategoryID");
      }
    }

    /// <summary>
    /// Is Url Rewriting enabled? -- default is false.
    /// </summary>
    public static bool EnableURLRewriting
    {
      get
      {
          return GetConfigValueAsBool("YAF.EnableUrlRewriting", false);
      }
    }

    /// <summary>
    /// Url Rewriting URLRewritingMode? -- default is Unicode.
    /// </summary>
    public static string UrlRewritingMode
    {
        get
        {
            return GetConfigValueAsString("YAF.URLRewritingMode") ?? string.Empty;
        }
    }

      /// <summary>
    /// Is Jquery Registration disabled? -- default is false.
    /// </summary>
    public static bool DisableJQuery
    {
        get
        {
            return GetConfigValueAsBool("YAF.DisableJQuery", false);
        }
    }

    /// <summary>
    /// Gets JQuery File Name.
    /// </summary>
    public static string JQueryFile
    {
        get
        {
            return GetConfigValueAsString("YAF.JQuery") ?? "js/jquery-1.4.4.min.js";
        }
    }

    /// <summary>
    /// Gets JQuery UI File Name.
    /// </summary>
    public static string JQueryUIFile
    {
        get
        {
          return GetConfigValueAsString("YAF.JQueryUI") ?? "http://ajax.googleapis.com/ajax/libs/jqueryui/1/jquery-ui.min.js";
        }
    }

    /// <summary>
    /// Used for Url Rewriting -- default is null -- used to define what the forum file name is for urls.
    /// </summary>
    public static string ForceScriptName
    {
      get
      {
        return GetConfigValueAsString("YAF.ForceScriptName");
      }
    }

    /// <summary>
    /// Used for Url Rewriting -- default is "default.aspx"
    /// </summary>
    public static string BaseScriptFile
    {
      get
      {
        return GetConfigValueAsString("YAF.BaseScriptFile") ?? "default.aspx";
      }
    }

    /// <summary>
    /// Prefix used for Url Rewriting -- default is "yaf_"
    /// </summary>
    public static string UrlRewritingPrefix
    {
      get
      {
        return GetConfigValueAsString("YAF.UrlRewritingPrefix") ?? "yaf_";
      }
    }

    /// <summary>
    /// Gets a Provider type string from the config.
    /// </summary>
    /// <param name="providerName"></param>
    /// <returns>Provider type string or <see langword="null"/> if none exist.</returns>
    public static string GetProvider(string providerName)
    {
      string key = string.Format("YAF.Provider.{0}", providerName);
      return GetConfigValueAsString(key);
    }

    /// <summary>
    /// Gets ServerFileRoot.
    /// </summary>
    public static string ServerFileRoot
    {
      get
      {
        return GetConfigValueAsString("YAF.FileRoot") ?? GetConfigValueAsString("YAF.ServerFileRoot") ?? String.Empty;
      }
    }

    /// <summary>
    /// Gets ClientFileRoot.
    /// </summary>
    public static string ClientFileRoot
    {
      get
      {
        return GetConfigValueAsString("YAF.ClientFileRoot") ?? String.Empty;
      }
    }

    /// <summary>
    /// Gets AppRoot.
    /// </summary>
    public static string AppRoot
    {
      get
      {
        return GetConfigValueAsString("YAF.AppRoot") ?? String.Empty;
      }
    }

    /// <summary>
    /// Gets BaseUrlMask.
    /// </summary>
    public static string BaseUrlMask
    {
      get
      {
        return GetConfigValueAsString("YAF.BaseUrlMask") ?? String.Empty;
      }
    }

    /// <summary>
    /// Folder to use for board specific uploads, images, themes
    /// Example : /Boards/
    /// </summary>
    public static string BoardRoot
    {
      get
      {
        return GetConfigValueAsString("YAF.BoardRoot") ?? String.Empty; // Use / to signify root
      }
    }

    /// <summary>
    /// Boolean to force uploads, and images, themes etc.. from a specific BoardID folder within BoardRoot
    /// Example : true /false
    /// </summary>
    public static bool MultiBoardFolders
    {
      get
      {
        return GetConfigValueAsBool("YAF.MultiBoardFolders", false);
      }
    }


    /// <summary>
    /// Gets ProviderKeyType.
    /// </summary>
    public static string ProviderKeyType
    {
      get
      {
        return GetConfigValueAsString("YAF.ProviderKeyType") ?? "System.Guid";
      }
    }

    /// <summary>
    /// Gets ProfileProvider.
    /// </summary>
    public static string ProviderProvider
    {
      get
      {
        return GetConfigValueAsString("YAF.ProfileProvider") ?? string.Empty;
      }
    }

    /// <summary>
    /// Gets MembershipProvider.
    /// </summary>
    public static string MembershipProvider
    {
      get
      {
        return GetConfigValueAsString("YAF.MembershipProvider") ?? string.Empty;
      }
    }

    /// <summary>
    /// Gets RoleProvider.
    /// </summary>
    public static string RoleProvider
    {
      get
      {
        return GetConfigValueAsString("YAF.RoleProvider") ?? string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether AllowLoginAndLogoff.
    /// </summary>
    public static bool AllowLoginAndLogoff
    {
      get
      {
        return GetConfigValueAsBool("YAF.AllowLoginAndLogoff", true);
      }
    }

    /// <summary>
    /// Display the default toolbar at the top -- default is "true"
    /// </summary>
    public static bool ShowToolBar
    {
      get
      {
        return GetConfigValueAsBool("YAF.ShowToolBar", true);
      }
    }

    /// <summary>
    /// Diisplay the footer at the bottom of the page -- default is "true"
    /// </summary>
    public static bool ShowFooter
    {
      get
      {
        return GetConfigValueAsBool("YAF.ShowFooter", true);
      }
    }

    /// <summary>
    /// Use an SSL connection for the SMTP server -- default is "false"
    /// </summary>
    public static bool UseSMTPSSL
    {
      get
      {
        return GetConfigValueAsBool("YAF.UseSMTPSSL", false);
      }
    }

    /// <summary>
    /// Gets ConnectionString.
    /// </summary>
    public static string ConnectionString
    {
      get
      {
        return ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
      }
    }

    /// <summary>
    /// Gets LogToMail.
    /// </summary>
    [Obsolete("Legacy: Phasing out")]
    public static string LogToMail
    {
      get
      {
        return GetConfigValueAsString("YAF.LogToMail");
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsDotNetNuke.
    /// </summary>
    public static bool IsDotNetNuke
    {
      get
      {
        object obj = HttpContext.Current.Items["PortalSettings"];
        return obj != null && obj.GetType().ToString().ToLower().IndexOf("dotnetnuke") >= 0;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsMojoPortal.
    /// </summary>
    public static bool IsMojoPortal
    {
      get
      {
        object obj = HttpContext.Current.Items["SiteSettings"];
        return obj != null && obj.GetType().ToString().ToLower().IndexOf("mojoportal") >= 0;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsRainbow.
    /// </summary>
    public static bool IsRainbow
    {
      get
      {
        object obj = HttpContext.Current.Items["PortalSettings"];
        return obj != null && obj.GetType().ToString().ToLower().IndexOf("rainbow") >= 0;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsPortal.
    /// </summary>
    public static bool IsPortal
    {
      get
      {
        return HttpContext.Current.Session["YetAnotherPortal.net"] != null;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsPortalomatic.
    /// </summary>
    public static bool IsPortalomatic
    {
      get
      {
        return HttpContext.Current.Session["Portalomatic.NET"] != null;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsAnyPortal.
    /// </summary>
    public static bool IsAnyPortal
    {
      get
      {
        return IsDotNetNuke || IsRainbow || IsMojoPortal || IsPortal || IsPortalomatic;
      }
    }


    #region Telerik Rad Editor Settings

    /// <summary>
    /// Gets RadEditorSkin.
    /// </summary>
    public static string RadEditorSkin
    {
      get
      {
        return GetConfigValueAsString("YAF.RadEditorSkin") ?? "Vista";
      }
    }

    /// <summary>
    /// Gets RadEditorToolsFile.
    /// </summary>
    public static string RadEditorToolsFile
    {
      get
      {
        return GetConfigValueAsString("YAF.RadEditorToolsFile") ?? String.Format("{0}/editors/RadEditor/ToolsFile.xml", ServerFileRoot);
      }
    }

    /// <summary>
    /// Gets a value indicating whether UseRadEditorToolsFile.
    /// </summary>
    public static bool UseRadEditorToolsFile
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

    #region Database Settings

    /// <summary>
    /// Gets ConnectionStringName.
    /// </summary>
    public static string ConnectionStringName
    {
      get
      {
        return GetConfigValueAsString("YAF.ConnectionStringName") ?? "yafnet";
      }
    }

    /// <summary>
    /// Gets DatabaseOwner.
    /// </summary>
    public static string DatabaseOwner
    {
      get
      {
        return GetConfigValueAsString("YAF.DatabaseOwner") ?? "dbo";
      }
    }

    /// <summary>
    /// Gets DatabaseObjectQualifier.
    /// </summary>
    public static string DatabaseObjectQualifier
    {
      get
      {
        return GetConfigValueAsString("YAF.DatabaseObjectQualifier") ?? "yaf_";
      }
    }

    // Different data layers specific settings


    /// <summary>
    /// Gets DatabaseEncoding.
    /// </summary>
    public static string DatabaseEncoding
    {
      get
      {
        return GetConfigValueAsString("YAF.DatabaseEncoding");
      }
    }

    /// <summary>
    /// Gets DatabaseCollation.
    /// </summary>
    public static string DatabaseCollation
    {
      get
      {
        return GetConfigValueAsString("YAF.DatabaseCollation");
      }
    }

    /// <summary>
    /// Gets SchemaName.
    /// </summary>
    public static string SchemaName
    {
      get
      {
        return GetConfigValueAsString("YAF.DatabaseSchemaName");
      }
    }

    /// <summary>
    /// Gets WithOIDs.
    /// </summary>
    public static string WithOIDs
    {
      get
      {
        return GetConfigValueAsString("YAF.DatabaseWithOIDs");
      }
    }

    /// <summary>
    /// Current BoardID -- default is 1.
    /// </summary>
    public static string SqlCommandTimeout
    {
        get
        {
            return GetConfigValueAsString("YAF.SqlCommandTimeout") ?? "99999";
        }
    }

    #endregion

    /// <summary>
    /// The get config value as string.
    /// </summary>
    /// <param name="configKey">
    /// The config key.
    /// </param>
    /// <returns>
    /// The get config value as string.
    /// </returns>
    public static string GetConfigValueAsString(string configKey)
    {
      foreach (string key in WebConfigurationManager.AppSettings.AllKeys)
      {
        if (key.Equals(configKey, StringComparison.CurrentCultureIgnoreCase))
        {
          return WebConfigurationManager.AppSettings[key];
        }
      }

      return null;
    }

    /// <summary>
    /// The get config value as bool.
    /// </summary>
    /// <param name="configKey">
    /// The config key.
    /// </param>
    /// <param name="defaultValue">
    /// The default value.
    /// </param>
    /// <returns>
    /// The get config value as bool.
    /// </returns>
    public static bool GetConfigValueAsBool(string configKey, bool defaultValue)
    {
      string value = GetConfigValueAsString(configKey);

      if (!String.IsNullOrEmpty(value))
      {
        return Convert.ToBoolean(value.ToLower());
      }

      return defaultValue;
    }

    public static string OverrideTrustLevel
    {
        get
        {
            return GetConfigValueAsString("YAF.OverrideTrustLevel");
        }
    }
  }
}