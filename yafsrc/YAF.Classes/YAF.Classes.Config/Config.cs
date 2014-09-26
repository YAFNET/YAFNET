/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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

namespace YAF.Classes
{
    #region Using

    using System;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Configuration;

    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    ///     Static class that access the app settings in the web.config file
    /// </summary>
    public static class Config
    {
        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether AllowLoginAndLogoff.
        /// </summary>
        public static bool AllowLoginAndLogoff
        {
            get
            {
                return GetConfigValueAsBool("YAF.AllowLoginAndLogoff", true);
            }
        }

        /// <summary>
        ///     Gets AppRoot.
        /// </summary>
        [NotNull]
        public static string AppRoot
        {
            get
            {
                return GetConfigValueAsString("YAF.AppRoot") ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets the Used for Url Rewriting -- default is "default.\.(.+)$\.(.+)$"
        /// </summary>
        [NotNull]
        public static string BaseScriptFile
        {
            get
            {
                return GetConfigValueAsString("YAF.BaseScriptFile") ?? "default.aspx";
            }
        }

        /// <summary>
        ///     Gets BaseUrlMask.
        /// </summary>
        [Obsolete("No longer been used Setting is moved to the board settings")]
        public static string BaseUrlMask
        {
            get
            {
                return GetConfigValueAsString("YAF.BaseUrlMask") ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets the Current BoardID -- default is 1.
        /// </summary>
        [NotNull]
        public static string BoardID
        {
            get
            {
                return GetConfigValueAsString("YAF.BoardID") ?? "1";
            }
        }

        /// <summary>
        ///     Gets the Folder to use for board specific uploads, images, themes Example : /Boards/
        /// </summary>
        [NotNull]
        public static string BoardRoot
        {
            get
            {
                return GetConfigValueAsString("YAF.BoardRoot") ?? string.Empty; // Use / to signify root
            }
        }

        /// <summary>
        ///     Gets BotScout API Key.
        /// </summary>
        public static string BotScoutApiKey
        {
            get
            {
                return GetConfigValueAsString("YAF.BotScoutApiKey");
            }
        }

        /// <summary>
        ///     Gets the Allowed browser JS version.
        /// </summary>
        public static string BrowserJSVersion
        {
            get
            {
                return GetConfigValueAsString("YAF.BrowserJSVersion") ?? "1.0";
            }
        }

        /// <summary>
        ///     Gets the Current CategoryID -- default is null.
        /// </summary>
        public static string CategoryID
        {
            get
            {
                return GetConfigValueAsString("YAF.CategoryID");
            }
        }

        /// <summary>
        ///     Gets ClientFileRoot.
        /// </summary>
        [NotNull]
        public static string ClientFileRoot
        {
            get
            {
                return GetConfigValueAsString("YAF.ClientFileRoot") ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether CSS Blocks should be compressed (minified)? -- default is true.
        /// </summary>
        public static bool CompressCSSBlocks
        {
            get
            {
                return GetConfigValueAsBool("YAF.CompressCSSBlocks", true);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether Java Script Blocks should be compressed (minified) 
        ///     Default is true.
        /// </summary>
        public static bool CompressJSBlocks
        {
            get
            {
                return GetConfigValueAsBool("YAF.CompressJSBlocks", true);
            }
        }

        /// <summary>
        /// Gets the name of the connection provider.
        /// </summary>
        /// <value>
        /// The name of the connection provider.
        /// </value>
        public static string ConnectionProviderName
        {
            get
            {
                if (ConnectionStringSettings != null && ConnectionStringSettings.ProviderName.IsSet())
                {
                    return ConnectionStringSettings.ProviderName;
                }

                // default to sql client for backwards compatibility.
                return "System.Data.SqlClient";
            }
        }

        /// <summary>
        ///     Gets ConnectionString.
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return ConnectionStringSettings != null ? ConnectionStringSettings.ConnectionString : null;
            }
        }

        /// <summary>
        ///     Gets ConnectionStringName.
        /// </summary>
        [NotNull]
        public static string ConnectionStringName
        {
            get
            {
                return GetConfigValueAsString("YAF.ConnectionStringName") ?? "yafnet";
            }
        }

        /// <summary>
        /// Gets the connection string settings.
        /// </summary>
        /// <value>
        /// The connection string settings.
        /// </value>
        public static ConnectionStringSettings ConnectionStringSettings
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[ConnectionStringName];
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the Use it if distinct YAF role names are required. Used in integration environments only.
        /// </summary>
        public static bool CreateDistinctRoles
        {
            get
            {
                return GetConfigValueAsBool("YAF.CreateDistinctRoles", false);
            }
        }

        /// <summary>
        ///     Gets DatabaseCollation.
        /// </summary>
        public static string DatabaseCollation
        {
            get
            {
                return GetConfigValueAsString("YAF.DatabaseCollation");
            }
        }

        /// <summary>
        ///     Gets DatabaseEncoding.
        /// </summary>
        public static string DatabaseEncoding
        {
            get
            {
                return GetConfigValueAsString("YAF.DatabaseEncoding");
            }
        }

        /// <summary>
        ///     Gets DatabaseObjectQualifier.
        /// </summary>
        [NotNull]
        public static string DatabaseObjectQualifier
        {
            get
            {
                var value = GetConfigValueAsString("YAF.DatabaseObjectQualifier");
                return value != null
                           ? (value.IsSet() ? GetConfigValueAsString("YAF.DatabaseObjectQualifier") : "yaf_")
                           : "yaf_";
            }
        }

        /// <summary>
        ///     Gets DatabaseOwner.
        /// </summary>
        [NotNull]
        public static string DatabaseOwner
        {
            get
            {
                return GetConfigValueAsString("YAF.DatabaseOwner") ?? "dbo";
            }
        }

        /// <summary>
        ///     Gets DatabaseScheme.
        /// </summary>
        [NotNull]
        public static string DatabaseScheme
        {
            get
            {
                return GetConfigValueAsString("YAF.DatabaseScheme") ?? "public";
            }
        }

        /// <summary>
        ///     Gets a value indicating whether Is jQuery Registration disabled? -- default is false.
        /// </summary>
        public static bool DisableJQuery
        {
            get
            {
                return GetConfigValueAsBool("YAF.DisableJQuery", false);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether Is Url Rewriting enabled? -- default is false.
        /// </summary>
        public static bool EnableURLRewriting
        {
            get
            {
                return GetConfigValueAsBool("YAF.EnableUrlRewriting", false);
            }
        }

        /// <summary>
        /// Gets the google client ID.
        /// </summary>
        /// <value>
        /// The google client ID.
        /// </value>
        public static string GoogleClientID
        {
            get
            {
                return GetConfigValueAsString("YAF.GoogleClientID");
            }
        }

        /// <summary>
        /// Gets the google client secret.
        /// </summary>
        /// <value>
        /// The google client secret.
        /// </value>
        public static string GoogleClientSecret
        {
            get
            {
                return GetConfigValueAsString("YAF.GoogleClientSecret");
            }
        }

        /// <summary>
        ///     Gets Facebook API Key.
        /// </summary>
        public static string FacebookAPIKey
        {
            get
            {
                return GetConfigValueAsString("YAF.FacebookAPIKey");
            }
        }

        /// <summary>
        ///     Gets Facebook Secret Key.
        /// </summary>
        public static string FacebookSecretKey
        {
            get
            {
                return GetConfigValueAsString("YAF.FacebookSecretKey");
            }
        }

        /// <summary>
        ///     Gets a value indicating whether Used for Url Rewriting -- default is null -- used to define what the forum file name is for URLs.
        /// </summary>
        public static string ForceScriptName
        {
            get
            {
                return GetConfigValueAsString("YAF.ForceScriptName");
            }
        }

        /// <summary>
        ///     Gets a value indicating whether IsAnyPortal.
        /// </summary>
        public static bool IsAnyPortal
        {
            get
            {
                return IsDotNetNuke || IsMojoPortal || IsRainbow || IsPortal || IsPortalomatic;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether IsDotNetNuke.
        /// </summary>
        public static bool IsDotNetNuke
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.Contains("DotNetNuke"));
                }

                object obj = HttpContext.Current.Items["PortalSettings"];
                return obj != null
                       && obj.GetType().ToString().ToLower().IndexOf("dotnetnuke", StringComparison.Ordinal) >= 0;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether IsMojoPortal.
        /// </summary>
        public static bool IsMojoPortal
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return false;
                }

                object obj = HttpContext.Current.Items["SiteSettings"];
                return obj != null && obj.GetType().ToString().ToLower().IndexOf("mojoportal", StringComparison.Ordinal) >= 0;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether IsPortal.
        /// </summary>
        public static bool IsPortal
        {
            get
            {
                return HttpContext.Current != null && HttpContext.Current.Session["YetAnotherPortal.net"] != null;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether Is Portalomatic.
        /// </summary>
        public static bool IsPortalomatic
        {
            get
            {
                return HttpContext.Current != null && HttpContext.Current.Session["Portalomatic.NET"] != null;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether IsRainbow.
        /// </summary>
        public static bool IsRainbow
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return false;
                }

                object obj = HttpContext.Current.Items["PortalSettings"];
                return obj != null && obj.GetType().ToString().ToLower().IndexOf("rainbow", StringComparison.Ordinal) >= 0;
            }
        }

        /// <summary>
        ///     Gets jQuery Alias
        /// </summary>
        [NotNull]
        public static string JQueryAlias
        {
            get
            {
                if (IsDotNetNuke)
                {
                    return "jQuery";
                }

                string jQueryAlias = GetConfigValueAsString("YAF.JQueryAlias") ?? "jQuery";

                if (!jQueryAlias.Equals("jQuery") || !jQueryAlias.Equals("$"))
                {
                    return "jQuery";
                }

                return jQueryAlias;
            }
        }

        /// <summary>
        ///     Gets JQuery Override File Name.
        /// </summary>
        public static string JQueryOverrideFile
        {
            get
            {
                return GetConfigValueAsString("YAF.JQueryOverrideFile")
                       ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets JQuery UI Override File Name.
        /// </summary>
        public static string JQueryUIOverrideFile
        {
            get
            {
                return GetConfigValueAsString("YAF.JQueryUIOverrideFile")
                       ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets jQuery UI date picker lang Override File Name.
        /// </summary>
        public static string JQueryUIOverrideLangFile
        {
            get
            {
                return GetConfigValueAsString("YAF.JQueryUIOverrideLangFile")
                       ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether YAF should be optimized for use with very large number of forums. Before enabling you should know exactly what you do and what's it all for. Ask developers.
        /// </summary>
        [NotNull]
        public static bool LargeForumTree
        {
            get
            {
                return GetConfigValueAsBool("YAF.LargeForumTree", false);
            }
        }

        /// <summary>
        ///     Gets LogToMail.
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
        ///     Gets MembershipProvider.
        /// </summary>
        [NotNull]
        public static string MembershipProvider
        {
            get
            {
                return GetConfigValueAsString("YAF.MembershipProvider") ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets MobileUserAgents.
        /// </summary>
        [NotNull]
        public static string MobileUserAgents
        {
            get
            {
                return GetConfigValueAsString("YAF.MobileUserAgents")
                       ?? "iphone,ipad,midp,windows ce,windows phone,android,blackberry,opera mini,mobile,palm,portable,webos,htc,armv,lg/u,elaine,nokia,playstation,symbian,sonyericsson,mmp,hd_mini";
            }
        }

        /// <summary>
        ///     Gets a value indicating whether Boolean to force uploads, and images, themes etc.. from a specific BoardID folder within BoardRoot Example : true /false
        /// </summary>
        public static bool MultiBoardFolders
        {
            get
            {
                return GetConfigValueAsBool("YAF.MultiBoardFolders", false);
            }
        }

        /// <summary>
        ///     Gets the NNTP post domain.
        /// </summary>
        [NotNull]
        public static string NntpPostDomain
        {
            get
            {
                return GetConfigValueAsString("YAF.NntpPostDomain") ?? "myforum.com";
            }
        }

        /// <summary>
        ///     Gets OverrideTrustLevel.
        /// </summary>
        public static string OverrideTrustLevel
        {
            get
            {
                return GetConfigValueAsString("YAF.OverrideTrustLevel");
            }
        }

        /// <summary>
        ///     Gets ProviderKeyType.
        /// </summary>
        [NotNull]
        public static string ProviderKeyType
        {
            get
            {
                return GetConfigValueAsString("YAF.ProviderKeyType") ?? "System.Guid";
            }
        }

        /// <summary>
        ///     Gets ProfileProvider.
        /// </summary>
        [NotNull]
        public static string ProviderProvider
        {
            get
            {
                return GetConfigValueAsString("YAF.ProfileProvider") ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets RadEditorSkin.
        /// </summary>
        [NotNull]
        public static string RadEditorSkin
        {
            get
            {
                return GetConfigValueAsString("YAF.RadEditorSkin") ?? "Vista";
            }
        }

        /// <summary>
        ///     Gets RadEditorToolsFile.
        /// </summary>
        [NotNull]
        public static string RadEditorToolsFile
        {
            get
            {
                return GetConfigValueAsString("YAF.RadEditorToolsFile")
                       ?? "{0}/Scripts/RadEditor/ToolsFile.xml".FormatWith(ServerFileRoot);
            }
        }

        /// <summary>
        ///     Gets RoleProvider.
        /// </summary>
        [NotNull]
        public static string RoleProvider
        {
            get
            {
                return GetConfigValueAsString("YAF.RoleProvider") ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets SchemaName.
        /// </summary>
        public static string SchemaName
        {
            get
            {
                return GetConfigValueAsString("YAF.DatabaseSchemaName");
            }
        }

        /// <summary>
        ///     Gets ServerFileRoot.
        /// </summary>
        [NotNull]
        public static string ServerFileRoot
        {
            get
            {
                return GetConfigValueAsString("YAF.FileRoot")
                       ?? GetConfigValueAsString("YAF.ServerFileRoot") ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether Display the footer at the bottom of the page -- default is "true"
        /// </summary>
        public static bool ShowFooter
        {
            get
            {
                return GetConfigValueAsBool("YAF.ShowFooter", true);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether Display the default toolbar at the top -- default is "true"
        /// </summary>
        public static bool ShowToolBar
        {
            get
            {
                return GetConfigValueAsBool("YAF.ShowToolBar", true);
            }
        }

        /// <summary>
        ///     Gets the Current BoardID -- default is 1.
        /// </summary>
        [NotNull]
        public static string SqlCommandTimeout
        {
            get
            {
                return GetConfigValueAsString("YAF.SqlCommandTimeout") ?? "99999";
            }
        }

        /// <summary>
        ///     Gets TwitterConsumerKey
        /// </summary>
        public static string TwitterConsumerKey
        {
            get
            {
                return GetConfigValueAsString("YAF.TwitterConsumerKey");
            }
        }

        /// <summary>
        ///     Gets TwitterConsumerSecret
        /// </summary>
        public static string TwitterConsumerSecret
        {
            get
            {
                return GetConfigValueAsString("YAF.TwitterConsumerSecret");
            }
        }

        /// <summary>
        /// Gets the twitter token.
        /// </summary>
        /// <value>
        /// The twitter token.
        /// </value>
        public static string TwitterToken
        {
            get
            {
                return GetConfigValueAsString("YAF.TwitterToken");
            }
        }

        /// <summary>
        /// Gets the twitter token secret.
        /// </summary>
        /// <value>
        /// The twitter token secret.
        /// </value>
        public static string TwitterTokenSecret
        {
            get
            {
                return GetConfigValueAsString("YAF.TwitterTokenSecret");
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is twitter enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is twitter enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool IsTwitterEnabled
        {
            get
            {
                return TwitterConsumerKey.IsSet() && TwitterConsumerSecret.IsSet() && TwitterToken.IsSet()
                       && TwitterTokenSecret.IsSet();
            }
        }

        /// <summary>
        ///     Gets the Url Rewriting Format -- default is standard.
        /// </summary>
        [NotNull]
        public static string UrlRewritingFormat
        {
            get
            {
                return GetConfigValueAsString("YAF.UrlRewritingFormat") ?? "standard";
            }
        }

        /// <summary>
        ///     Gets the Url Rewriting URLRewritingMode? -- default is Unicode.
        /// </summary>
        [NotNull]
        public static string UrlRewritingMode
        {
            get
            {
                return GetConfigValueAsString("YAF.URLRewritingMode") ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets the Prefix used for Url Rewriting -- default is "yaf_"
        /// </summary>
        [NotNull]
        public static string UrlRewritingPrefix
        {
            get
            {
                return GetConfigValueAsString("YAF.UrlRewritingPrefix") ?? "yaf_";
            }
        }

        /// <summary>
        ///     Gets a value indicating whether UseRadEditorToolsFile.
        /// </summary>
        public static bool UseRadEditorToolsFile
        {
            get
            {
                string value = GetConfigValueAsString("YAF.UseRadEditorToolsFile");

                if (!value.IsSet())
                {
                    return false;
                }

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

                return false;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether Use an SSL connection for the SMTP server -- default is "false"
        /// </summary>
        public static bool UseSMTPSSL
        {
            get
            {
                return GetConfigValueAsBool("YAF.UseSMTPSSL", false);
            }
        }

        /// <summary>
        ///     Gets WithOIDs.
        /// </summary>
        public static string WithOIDs
        {
            get
            {
                return GetConfigValueAsString("YAF.DatabaseWithOIDs");
            }
        }

        /// <summary>
        /// Gets the banned IP redirect URL.
        /// </summary>
        /// <value>
        /// The banned IP redirect URL.
        /// </value>
        public static string BannedIpRedirectUrl
        {
            get
            {
                return GetConfigValueAsString("YAF.BannedIpRedirectUrl");
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the config value as Boolean.
        /// </summary>
        /// <param name="configKey"> The config key. </param>
        /// <param name="defaultValue"> if set to <c>true</c> [default value]. </param>
        /// <returns> Returns Boolean Value </returns>
        public static bool GetConfigValueAsBool([NotNull] string configKey, bool defaultValue)
        {
            string value = GetConfigValueAsString(configKey);

            return value.IsSet() ? value.ToLower().ToType<bool>() : defaultValue;
        }

        /// <summary>
        ///     Gets the config value as string.
        /// </summary>
        /// <param name="configKey"> The config key. </param>
        /// <returns> Returns String Value </returns>
        public static string GetConfigValueAsString([NotNull] string configKey)
        {
            return (from key in WebConfigurationManager.AppSettings.AllKeys
                    where key.Equals(configKey, StringComparison.CurrentCultureIgnoreCase)
                    select WebConfigurationManager.AppSettings[key]).FirstOrDefault();
        }

        /// <summary>
        ///     Gets a Provider type string from the config.
        /// </summary>
        /// <param name="providerName"> The provider Name. </param>
        /// <returns> Provider type string or <see langword="null" /> if none exist. </returns>
        public static string GetProvider([NotNull] string providerName)
        {
            string key = "YAF.Provider.{0}".FormatWith(providerName);
            return GetConfigValueAsString(key);
        }

        #endregion
    }
}