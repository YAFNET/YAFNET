/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Configuration;

using System.Configuration;
using System.Linq;
using System.Web.Configuration;

/// <summary>
///     Static class that access the app settings in the web.config file
/// </summary>
public static class Config
{
    /// <summary>
    ///     Gets a value indicating whether AllowLoginAndLogoff.
    /// </summary>
    public static bool AllowLoginAndLogoff => GetConfigValueAsBool("YAF.AllowLoginAndLogoff", true);

    /// <summary>
    ///     Gets AppRoot.
    /// </summary>
    [NotNull]
    public static string AppRoot => GetConfigValueAsString("YAF.AppRoot") ?? string.Empty;

    /// <summary>
    ///     Gets the Used for Url Rewriting -- default is "default.\.(.+)$\.(.+)$"
    /// </summary>
    [NotNull]
    public static string BaseScriptFile => GetConfigValueAsString("YAF.BaseScriptFile") ?? "default.aspx";

    /// <summary>
    ///     Gets the Current BoardID -- default is 1.
    /// </summary>
    public static int BoardID => GetConfigValueAsInt("YAF.BoardID", 1);

    /// <summary>
    ///     Gets the Folder to use for board specific uploads, images Example : /Boards/
    /// </summary>
    [NotNull]
    public static string BoardRoot => GetConfigValueAsString("YAF.BoardRoot") ?? string.Empty;

    /// <summary>
    ///     Gets the Current CategoryID -- default is null.
    /// </summary>
    public static int CategoryID => GetConfigValueAsInt("YAF.CategoryID", 0);

    /// <summary>
    ///     Gets ClientFileRoot.
    /// </summary>
    [NotNull]
    public static string ClientFileRoot => GetConfigValueAsString("YAF.ClientFileRoot") ?? string.Empty;

    /// <summary>
    /// Gets the GDPR controller address.
    /// </summary>
    /// <value>
    /// The GDPR controller address.
    /// </value>
    public static string GDPRControllerAddress => GetConfigValueAsString("YAF.GDPRControllerAddress") ?? string.Empty;

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
    public static string ConnectionString => ConnectionStringSettings?.ConnectionString;

    /// <summary>
    ///     Gets ConnectionStringName.
    /// </summary>
    [NotNull]
    public static string ConnectionStringName => GetConfigValueAsString("YAF.ConnectionStringName") ?? "yafnet";

    /// <summary>
    /// Gets the connection string settings.
    /// </summary>
    /// <value>
    /// The connection string settings.
    /// </value>
    public static ConnectionStringSettings ConnectionStringSettings => ConfigurationManager.ConnectionStrings[ConnectionStringName];

    /// <summary>
    ///     Gets a value indicating whether the Use it if distinct YAF role names are required. Used in integration environments only.
    /// </summary>
    public static bool CreateDistinctRoles => GetConfigValueAsBool("YAF.CreateDistinctRoles", false);

    /// <summary>
    ///     Gets the Database Object Qualifier.
    /// </summary>
    [NotNull]
    public static string DatabaseObjectQualifier
    {
        get
        {
            var value = GetConfigValueAsString("YAF.DatabaseObjectQualifier");

            return value.IsSet() ? GetConfigValueAsString("YAF.DatabaseObjectQualifier") : "yaf_";
        }
    }

    /// <summary>
    ///    Gets the Database Schema.
    /// </summary>
    [NotNull]
    public static string DatabaseSchema => GetConfigValueAsString("YAF.DatabaseSchema") ?? "public";

    /// <summary>
    ///    Gets the Database Owner.
    /// </summary>
    [NotNull]
    public static string DatabaseOwner => GetConfigValueAsString("YAF.DatabaseOwner") ?? "dbo";

    /// <summary>
    ///     Gets a value indicating whether Is Url Rewriting enabled? -- default is "true".
    /// </summary> 
    public static bool EnableURLRewriting => GetConfigValueAsBool("YAF.EnableUrlRewriting", true);

    /// <summary>
    /// Gets the google client ID.
    /// </summary>
    /// <value>
    /// The google client ID.
    /// </value>
    public static string GoogleClientID => GetConfigValueAsString("YAF.GoogleClientID");

    /// <summary>
    /// Gets the google client secret.
    /// </summary>
    /// <value>
    /// The google client secret.
    /// </value>
    public static string GoogleClientSecret => GetConfigValueAsString("YAF.GoogleClientSecret");

    /// <summary>
    ///     Gets Facebook API Key.
    /// </summary>
    public static string FacebookAPIKey => GetConfigValueAsString("YAF.FacebookAPIKey");

    /// <summary>
    ///     Gets Facebook Secret Key.
    /// </summary>
    public static string FacebookSecretKey => GetConfigValueAsString("YAF.FacebookSecretKey");

    /// <summary>
    ///     Gets a value indicating whether Used for Url Rewriting -- default is "default.aspx" -- used to define what the forum file name is for URLs.
    /// </summary>
    public static string ForceScriptName => GetConfigValueAsString("YAF.ForceScriptName") ?? "default.aspx";

    /// <summary>
    ///     Gets a value indicating whether IsAnyPortal.
    /// </summary>
    public static bool IsAnyPortal => IsDotNetNuke || IsMojoPortal || IsPortal;

    /// <summary>
    ///     Gets a value indicating whether IsDotNetNuke.
    /// </summary>
    public static bool IsDotNetNuke
    {
        get
        {
            if (HttpContext.Current is null)
            {
                return AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.Contains("DotNetNuke"));
            }

            var obj = HttpContext.Current.Items["PortalSettings"];

            return obj != null
                   && obj.ToString().Contains("DotNetNuke");
        }
    }

    /// <summary>
    ///     Gets a value indicating whether IsMojoPortal.
    /// </summary>
    public static bool IsMojoPortal
    {
        get
        {
            if (HttpContext.Current is null)
            {
                return false;
            }

            var obj = HttpContext.Current.Items["SiteSettings"];
            return obj != null && obj.ToString().ToLower().IndexOf("mojoportal", StringComparison.Ordinal) >= 0;
        }
    }

    /// <summary>
    ///     Gets a value indicating whether IsPortal.
    /// </summary>
    public static bool IsPortal => HttpContext.Current.Session != null && HttpContext.Current.Session["YetAnotherPortal.net"] != null;

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

            var jqueryAlias = GetConfigValueAsString("YAF.JQueryAlias") ?? "jQuery";

            if (!jqueryAlias.Equals("jQuery") || !jqueryAlias.Equals("$"))
            {
                return "jQuery";
            }

            return jqueryAlias;
        }
    }

    /// <summary>
    ///     Gets JQuery Override File Name.
    /// </summary>
    public static string JQueryOverrideFile => GetConfigValueAsString("YAF.JQueryOverrideFile")
                                               ?? string.Empty;

    /// <summary>
    /// The jQuery version.
    /// </summary>
    public static string JQueryVersion => GetConfigValueAsString("YAF.JQueryVersion")
                                          ?? "3.7.0";

    /// <summary>
    ///     Gets a value indicating whether Boolean to force uploads, and images, themes etc.. from a specific BoardID folder within BoardRoot Example : true /false
    /// </summary>
    public static bool MultiBoardFolders => GetConfigValueAsBool("YAF.MultiBoardFolders", false);

    /// <summary>
    ///     Gets ServerFileRoot.
    /// </summary>
    [NotNull]
    public static string ServerFileRoot => GetConfigValueAsString("YAF.FileRoot")
                                           ?? GetConfigValueAsString("YAF.ServerFileRoot") ?? string.Empty;

    /// <summary>
    ///     Gets a value indicating whether Display the footer at the bottom of the page -- default is "true"
    /// </summary>
    public static bool ShowFooter => GetConfigValueAsBool("YAF.ShowFooter", true);

    /// <summary>
    ///     Gets a value indicating whether Display the default toolbar at the top -- default is "true"
    /// </summary>
    public static bool ShowToolBar => GetConfigValueAsBool("YAF.ShowToolBar", true);

    /// <summary>
    ///     Gets the Current BoardID -- default is 1.
    /// </summary>
    public static int SqlCommandTimeout => GetConfigValueAsInt("YAF.SqlCommandTimeout", 99999);

    /// <summary>
    ///     Gets TwitterConsumerKey
    /// </summary>
    public static string TwitterConsumerKey => GetConfigValueAsString("YAF.TwitterConsumerKey");

    /// <summary>
    ///     Gets TwitterConsumerSecret
    /// </summary>
    public static string TwitterConsumerSecret => GetConfigValueAsString("YAF.TwitterConsumerSecret");

    /// <summary>
    ///     Gets the Url Rewriting URLRewritingMode? -- default is Unicode.
    /// </summary>
    [NotNull]
    public static string UrlRewritingMode => GetConfigValueAsString("YAF.URLRewritingMode") ?? string.Empty;

    /// <summary>
    /// Gets the banned IP redirect URL.
    /// </summary>
    /// <value>
    /// The banned IP redirect URL.
    /// </value>
    public static string BannedIpRedirectUrl => GetConfigValueAsString("YAF.BannedIpRedirectUrl");

    /// <summary>
    /// Gets the legacy membership hash algorithm type.
    /// </summary>
    public static HashAlgorithmType LegacyMembershipHashAlgorithmType
    {
        get
        {
            var value = GetConfigValueAsString("YAF.LegacyMembershipHashAlgorithmType");

            return value.IsSet()
                       ? value.ToEnum<HashAlgorithmType>()
                       : HashAlgorithmType.SHA1;
        }
    }

    /// <summary>
    /// Gets the legacy membership hash case.
    /// </summary>
    public static HashCaseType LegacyMembershipHashCase
    {
        get
        {
            var value = GetConfigValueAsString("YAF.LegacyMembershipHashCase");

            return value.IsSet()
                       ? value.ToEnum<HashCaseType>()
                       : HashCaseType.None;
        }
    }

    /// <summary>
    /// Gets a value indicating whether legacy membership hash hex.
    /// </summary>
    public static bool LegacyMembershipHashHex => GetConfigValueAsBool("YAF.LegacyMembershipHashHex", false);

    /// <summary>
    ///     Gets the config value as Integer.
    /// </summary>
    /// <param name="configKey"> The config key. </param>
    /// <param name="defaultValue"> if set to <c>true</c> [default value]. </param>
    /// <returns> Returns the Integer Value </returns>
    public static int GetConfigValueAsInt([NotNull] string configKey, int defaultValue)
    {
        var value = GetConfigValueAsString(configKey);

        return value.IsSet() ? value.ToLower().ToType<int>() : defaultValue;
    }

    /// <summary>
    ///     Gets the config value as Boolean.
    /// </summary>
    /// <param name="configKey"> The config key. </param>
    /// <param name="defaultValue"> if set to <c>true</c> [default value]. </param>
    /// <returns> Returns Boolean Value </returns>
    public static bool GetConfigValueAsBool([NotNull] string configKey, bool defaultValue)
    {
        var value = GetConfigValueAsString(configKey);

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
}