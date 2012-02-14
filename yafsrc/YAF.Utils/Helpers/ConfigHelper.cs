/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
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

namespace YAF.Utils.Helpers
{
  using System;
  using System.Collections.Specialized;
  using System.Configuration;
  using System.Security.Permissions;
  using System.Web;
  using System.Web.Configuration;

  /// <summary>
  /// The config helper.
  /// </summary>
  public class ConfigHelper
  {
    /// <summary>
    /// The _app settings full.
    /// </summary>
    private AppSettingsSection _appSettingsFull = null;

    /// <summary>
    /// The _trust level.
    /// </summary>
    private AspNetHostingPermissionLevel? _trustLevel = null;

    /// <summary>
    /// The _web config.
    /// </summary>
    private Configuration _webConfig = null;

    /// <summary>
    /// Gets TrustLevel.
    /// </summary>
    public AspNetHostingPermissionLevel TrustLevel
    {
      get
      {
        if (!this._trustLevel.HasValue)
        {
          this._trustLevel = General.GetCurrentTrustLevel();
        }

        return this._trustLevel.Value;
      }
    }

    /// <summary>
    /// Gets AppSettings.
    /// </summary>
    public NameValueCollection AppSettings
    {
      get
      {
        return WebConfigurationManager.AppSettings;
      }
    }

    /// <summary>
    /// Gets WebConfigFull.
    /// </summary>
    public Configuration WebConfigFull
    {
      get
      {
        if (this._webConfig == null)
        {
          this._webConfig = WebConfigurationManager.OpenWebConfiguration("~/");
        }

        return this._webConfig;
      }
    }

    /// <summary>
    /// Gets AppSettingsFull.
    /// </summary>
    public AppSettingsSection AppSettingsFull
    {
      get
      {
        if (this._appSettingsFull == null)
        {
          this._appSettingsFull = GetConfigSectionFull<AppSettingsSection>("appSettings");
        }

        return this._appSettingsFull;
      }
    }

    /// <summary>
    /// The get config section.
    /// </summary>
    /// <param name="sectionName">
    /// The section name.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public T GetConfigSection<T>(string sectionName) where T : class
    {
      var section = WebConfigurationManager.GetWebApplicationSection(sectionName) as T;
      return section;
    }

    /// <summary>
    /// The get config value as string.
    /// </summary>
    /// <param name="keyName">
    /// The key name.
    /// </param>
    /// <returns>
    /// The get config value as string.
    /// </returns>
    public string GetConfigValueAsString(string keyName)
    {
      if (TrustLevel == AspNetHostingPermissionLevel.High)
      {
        foreach (string key in AppSettingsFull.Settings.AllKeys)
        {
          if (key.Equals(keyName, StringComparison.CurrentCultureIgnoreCase))
          {
            return AppSettingsFull.Settings[key].Value;
          }
        }
      }
      else
      {
        foreach (string key in AppSettings.AllKeys)
        {
          if (key.Equals(keyName, StringComparison.CurrentCultureIgnoreCase))
          {
            return AppSettings[key];
          }
        }
      }

      return null;
    }

    /// <summary>
    /// The get config section full.
    /// </summary>
    /// <param name="sectionName">
    /// The section name.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.High)]
    public T GetConfigSectionFull<T>(string sectionName) where T : class
    {
      ConfigurationSection section = WebConfigFull.GetSection(sectionName);
      if (section is T)
      {
        return section as T;
      }

      return null;
    }

    /// <summary>
    /// The write connection string.
    /// </summary>
    /// <param name="keyName">
    /// The key name.
    /// </param>
    /// <param name="keyValue">
    /// The key value.
    /// </param>
    /// <param name="providerValue">
    /// The provider value.
    /// </param>
    /// <returns>
    /// The write connection string.
    /// </returns>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.High)]
    public bool WriteConnectionString(string keyName, string keyValue, string providerValue)
    {
      var connStrings = GetConfigSectionFull<ConnectionStringsSection>("connectionStrings");

      if (connStrings == null)
      {
        return false;
      }

      bool writtenSuccessfully = false;
      try
      {
        if (connStrings.ConnectionStrings[keyName] != null)
        {
          connStrings.ConnectionStrings.Remove(keyName);
        }

        connStrings.ConnectionStrings.Add(new ConnectionStringSettings(keyName, keyValue, providerValue));

        WebConfigFull.Save(ConfigurationSaveMode.Modified);

        writtenSuccessfully = true;
      }
      catch
      {
        writtenSuccessfully = false;
      }

      return writtenSuccessfully;
    }

    /// <summary>
    /// The write app setting.
    /// </summary>
    /// <param name="keyName">
    /// The key name.
    /// </param>
    /// <param name="keyValue">
    /// The key value.
    /// </param>
    /// <returns>
    /// The write app setting.
    /// </returns>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.High)]
    public bool WriteAppSetting(string keyName, string keyValue)
    {
      bool writtenSuccessfully = false;

      try
      {
        if (AppSettingsFull.Settings[keyName] != null)
        {
          AppSettingsFull.Settings.Remove(keyName);
        }

        AppSettingsFull.Settings.Add(keyName, keyValue);

        WebConfigFull.Save(ConfigurationSaveMode.Modified);

        writtenSuccessfully = true;
      }
      catch
      {
        writtenSuccessfully = false;
      }

      return writtenSuccessfully;
    }
  }
}