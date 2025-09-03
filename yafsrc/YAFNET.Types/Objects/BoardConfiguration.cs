/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Types.Objects;

using YAF.Types.Constants;

/// <summary>
///    Board Configuration from the AppSettings
/// </summary>
public class BoardConfiguration
{
    /// <summary>
    /// Setting to turn on/off the ability to log in/off. This is usefully when you want the main application to handle the log-in/off process.
    ///
    /// ** Defaults to: true
    /// </summary>
    public bool AllowLoginAndLogoff { get; set; } = true;

    /// <summary>
    /// Gets or sets the Current BoardID of the board that is used inside the application.
    /// You can create as many boards as you want, and they will function as separate forums (with separate users). (Host -> Boards to create new boards.)
    /// 
    /// ** Defaults to: 1 **
    /// </summary>
    public int BoardID { get; set; } = 1;

    /// <summary>
    /// Gets or sets the Current CategoryID. If set the main page shows this as category, otherwise all categories will be shown.
    ///
    /// ** Defaults to: null/0. **
    /// </summary>
    public int CategoryID { get; set; }

    /// <summary>
    /// Gets or sets URLRewritingMode Key
    /// Unicode - will replace url symbols with unicode characters
    /// Translit - will replace unicode characters with ascii characters
    /// no entry - will replace non-ascii symbols with dashes
    /// 
    ///  ** Defaults to: Empty **
    /// </summary>
    /// <value>The URL rewriting mode.</value>
    public string UrlRewritingMode { get; set; }

    /// <summary>
    /// Gets or sets the email address on the privacy page in the GDPR section.
    /// 
    /// ** Defaults to: Empty  **
    /// </summary>
    /// <value>
    /// The GDPR controller address.
    /// </value>
    public string GDPRControllerAddress { get; set; }

    /// <summary>
    /// Gets or sets the name DB provider...
    /// 'Microsoft.Data.SqlClient' - Use for SqlServer
    /// 'Npgsql Data Provider' - Use for PostgreSQL
    /// 'System.Data.SQLite' - Use for SQLite
    /// 'MySql.Data.MySqlClient' - Use for MySQL
    /// 
    /// ** Defaults to: Empty  **
    /// </summary>
    /// <value>
    /// The name of the connection provider.
    /// </value>
    public string ConnectionProviderName { get; set; }

    /// <summary>
    /// Gets or sets the db table prefix. For advanced users who want to change the prefix for Yaf DB structure.
    ///
    /// ** Defaults to: 'yaf_' **
    /// </summary>
    public string DatabaseObjectQualifier { get; set; }

    /// <summary>
    ///      For advanced users who want to the change the default permissions for the YAF DB structure.
    ///
    /// ** Defaults to: 'dbo' **
    /// </summary>
    public string DatabaseOwner { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to force uploads, and images, themes etc. from a specific BoardID folder within wwwroot...
    /// E.g. wwwroot/Boards/1/Uploads
    ///
    /// ** Defaults to: false **
    /// </summary>
    public bool MultiBoardFolders { get; set; }

    /// <summary>
    /// Gets or sets the Option to Set the SQL Command Timeout
    /// In some cases is need to set it to Unlimited value="0"
    ///
    /// ** Defaults to: 99999 **
    /// </summary>
    public int SqlCommandTimeout { get; set; } = 99999;

    /// <summary>
    /// Gets or sets the legacy membership hash algorithm type.
    ///
    /// NOTE: Legacy ASP.NET Membership Migration. This can be skipped on a New Install, only relevant if you upgrade from pre 3x
    /// More info here: https://github.com/YAFNET/YAFNET/wiki/Upgrade-an-Existing-YAF.NET-Installation
    /// 
    /// ** Defaults to: Empty  **
    /// </summary>
    public HashAlgorithmType LegacyMembershipHashAlgorithmType { get; set; }

    /// <summary>
    /// Gets or sets the legacy membership hash case.
    ///
    /// NOTE: Legacy ASP.NET Membership Migration. This can be skipped on a New Install, only relevant if you upgrade from pre 3x
    /// More info here: https://github.com/YAFNET/YAFNET/wiki/Upgrade-an-Existing-YAF.NET-Installation
    /// 
    /// ** Defaults to: Empty  **
    /// </summary>
    public HashCaseType LegacyMembershipHashCase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether legacy membership hash hex.
    /// 
    /// NOTE: Legacy ASP.NET Membership Migration. This can be skipped on a New Install, only relevant if you upgrade from pre 3x
    /// More info here: https://github.com/YAFNET/YAFNET/wiki/Upgrade-an-Existing-YAF.NET-Installation
    /// 
    /// ** Defaults to: Empty  **
    /// </summary>
    public bool LegacyMembershipHashHex { get; set; }

    /// <summary>
    /// This Setting if enabled actives the Http Redirection Middleware.
    /// More info here: https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.builder.httpspolicybuilderextensions.usehttpsredirection
    /// 
    /// ** Defaults to: true  **
    /// </summary>
    /// <value><c>true</c> if [use HTTPS redirection]; otherwise, <c>false</c>.</value>
    public bool UseHttpsRedirection { get; set; } = true;

    /// <summary>
    /// This setting is needed if YAF.NET is embedded into another application. This sets the Areas folder name.
    /// More info here: https://github.com/YAFNET/YAFNET/wiki/YAF.NET-Integration-in-to-an-existing-ASP.NET-Core-Application
    /// 
    /// ** Defaults to: Empty  **
    /// </summary>
    /// <value>The name of the area.</value>
    public string Area { get; set; }

    /// <summary>
    /// This setting if enabled actives the rate limiting services (Rate Limiter Middleware)
    /// More info here: https://learn.microsoft.com/aspnet/core/performance/rate-limit
    /// 
    /// ** Defaults to: true  **
    /// </summary>
    /// <value><c>true</c> if [use rate limiter]; otherwise, <c>false</c>.</value>
    public bool UseRateLimiter { get; set; } = true;

    /// <summary>
    /// When *UseRateLimiter* is used, then this sets the maximum number of permit counters that can be allowed in a window.
    /// Must be set to a value > 0.
    /// 
    /// ** Defaults to: 30  **
    /// </summary>
    public int RateLimiterPermitLimit { get; set; } = 10;

    /// <summary>
    /// X-Frame-Options tells the browser whether you want to allow your site to be framed or not.By preventing a browser from framing your site you can defend against attacks like clickjacking.
    /// Recommended value "SAMEORIGIN".
    /// 
    /// ** Defaults to: 'SAMEORIGIN'  **
    /// </summary>
    /// <value>The x frame options.</value>
    public string XFrameOptions { get; set; } = "SAMEORIGIN";

    /// <summary>
    /// X-Content-Type-Options stops a browser from trying to MIME-sniff the content type and forces it to stick with the declared content-type.
    /// The only valid value for this header is "X-Content-Type-Options: nosniff".
    /// 
    /// ** Defaults to: 'nosniff'  **
    /// </summary>
    /// <value>The x content type options.</value>
    public string XContentTypeOptions { get; set; } = "nosniff";

    /// <summary>
    /// Referrer Policy is a new header that allows a site to control how much information the browser includes with navigations away from a document and should be set by all sites.
    /// 
    /// ** Defaults to: 'no-referrer'  **
    /// </summary>
    /// <value>The referrer policy.</value>
    public string ReferrerPolicy { get; set; } = "no-referrer";

    /// <summary>
    /// Content Security Policy is an effective measure to protect your site from XSS attacks. By whitelisting sources of approved content, you can prevent the browser from loading malicious assets.
    /// 
    /// ** Defaults to: empty  **
    /// </summary>
    /// <value>The permissions policy.</value>
    public string ContentSecurityPolicy { get; set; } = "";
}