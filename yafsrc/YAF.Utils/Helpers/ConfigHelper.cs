/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Utils.Helpers
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Linq;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Configuration;

    /// <summary>
    ///     The config helper.
    /// </summary>
    public class ConfigHelper
    {
        #region Fields

        /// <summary>
        ///     The _app settings full.
        /// </summary>
        private AppSettingsSection appSettingsFull;

        /// <summary>
        ///     The _web config.
        /// </summary>
        private Configuration webConfig;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets AppSettings.
        /// </summary>
        public NameValueCollection AppSettings => WebConfigurationManager.AppSettings;

        /// <summary>
        ///     Gets AppSettingsFull.
        /// </summary>
        public AppSettingsSection AppSettingsFull => this.appSettingsFull
                                                     ?? (this.appSettingsFull =
                                                             this.GetConfigSectionFull<AppSettingsSection>(
                                                                 "appSettings"));

        /// <summary>
        ///     Gets WebConfigFull.
        /// </summary>
        public Configuration WebConfigFull => this.webConfig
                                              ?? (this.webConfig = WebConfigurationManager.OpenWebConfiguration("~/"));

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the configuration section.
        /// </summary>
        /// <typeparam name="T">The type parameter name</typeparam>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns>Returns the the configuration section.</returns>
        public T GetConfigSection<T>(string sectionName)
            where T : class
        {
            var section = WebConfigurationManager.GetWebApplicationSection(sectionName) as T;
            return section;
        }

        /// <summary>
        /// Gets the configuration section full.
        /// </summary>
        /// <typeparam name="T">The type parameter name</typeparam>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns>Returns the full configuration setting</returns>
        public T GetConfigSectionFull<T>(string sectionName)
            where T : class
        {
            var section = this.WebConfigFull.GetSection(sectionName);
            return section as T;
        }

        /// <summary>
        /// Gets the configuration value as string.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns>Returns the configuration value as string.</returns>
        public string GetConfigValueAsString(string keyName)
        {
            var allKeys = this.AppSettingsFull.Settings.AllKeys;
            return allKeys.Where(key => key.Equals(keyName, StringComparison.OrdinalIgnoreCase))
                .Select(key => this.AppSettings[key])
                .FirstOrDefault();
        }

        /// <summary>
        /// Writes the setting to the appSettings section of the web.config
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
        public bool WriteAppSetting(string keyName, string keyValue)
        {
            bool writtenSuccessfully;

            try
            {
                if (this.AppSettingsFull.Settings[keyName] != null)
                {
                    this.AppSettingsFull.Settings.Remove(keyName);
                }

                this.AppSettingsFull.Settings.Add(keyName, keyValue);

                this.WebConfigFull.Save(ConfigurationSaveMode.Modified);

                writtenSuccessfully = true;
            }
            catch
            {
                writtenSuccessfully = false;
            }

            return writtenSuccessfully;
        }

        /// <summary>
        /// Writes the setting to the connectionString section of the web.config
        /// </summary>
        /// <param name="keyName">The key name.</param>
        /// <param name="keyValue">The key value.</param>
        /// <param name="providerValue">The provider value.</param>
        /// <returns>
        /// The write connection string.
        /// </returns>
        [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.High)]
        public bool WriteConnectionString(string keyName, string keyValue, string providerValue)
        {
            var connStrings = this.GetConfigSectionFull<ConnectionStringsSection>("connectionStrings");

            if (connStrings == null)
            {
                return false;
            }

            bool writtenSuccessfully;
            try
            {
                if (connStrings.ConnectionStrings[keyName] != null)
                {
                    connStrings.ConnectionStrings.Remove(keyName);
                }

                connStrings.ConnectionStrings.Add(new ConnectionStringSettings(keyName, keyValue, providerValue));

                this.WebConfigFull.Save(ConfigurationSaveMode.Modified);

                writtenSuccessfully = true;
            }
            catch
            {
                writtenSuccessfully = false;
            }

            return writtenSuccessfully;
        }

        #endregion
    }
}