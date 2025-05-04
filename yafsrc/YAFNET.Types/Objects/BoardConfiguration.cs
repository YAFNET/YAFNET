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
    /// Gets or sets a value indicating whether allow login and logoff.
    /// </summary>
    public bool AllowLoginAndLogoff { get; set; }

    /// <summary>
    ///     Gets or sets the Current BoardID -- default is 1.
    /// </summary>
    public int BoardID { get; set; }

    /// <summary>
    ///     Gets or sets the Current CategoryID -- default is null.
    /// </summary>
    public int CategoryID { get; set; }

    /// <summary>
    /// Gets or sets the URL rewriting mode.
    /// </summary>
    /// <value>The URL rewriting mode.</value>
    public string UrlRewritingMode { get; set; }

    /// <summary>
    /// Gets or sets the GDPR controller address.
    /// </summary>
    /// <value>
    /// The GDPR controller address.
    /// </value>
    public string GDPRControllerAddress { get; set; }

    /// <summary>
    /// Gets or sets the name of the connection provider.
    /// </summary>
    /// <value>
    /// The name of the connection provider.
    /// </value>
    public string ConnectionProviderName { get; set; }

    /// <summary>
    ///     Gets or sets DatabaseObjectQualifier.
    /// </summary>
    public string DatabaseObjectQualifier { get; set; }

    /// <summary>
    ///     Gets or sets DatabaseOwner.
    /// </summary>
    public string DatabaseOwner { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether Boolean to force uploads, and images, themes etc. from a specific BoardID folder within Root Example : true /false
    /// </summary>
    public bool MultiBoardFolders { get; set; }

    /// <summary>
    ///     Gets or sets the Current BoardID -- default is 1.
    /// </summary>
    public int SqlCommandTimeout { get; set; }

    /// <summary>
    /// Gets or sets the legacy membership hash algorithm type.
    /// </summary>
    public HashAlgorithmType LegacyMembershipHashAlgorithmType { get; set; }

    /// <summary>
    /// Gets or sets the legacy membership hash case.
    /// </summary>
    public HashCaseType LegacyMembershipHashCase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether legacy membership hash hex.
    /// </summary>
    public bool LegacyMembershipHashHex { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [use HTTPS redirection].
    /// </summary>
    /// <value><c>true</c> if [use HTTPS redirection]; otherwise, <c>false</c>.</value>
    public bool UseHttpsRedirection { get; set; } = true;

    /// <summary>
    /// Gets or sets the name of the area.
    /// </summary>
    /// <value>The name of the area.</value>
    public string Area { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether /[use rate limiter].
    /// </summary>
    /// <value><c>true</c> if [use rate limiter]; otherwise, <c>false</c>.</value>
    public bool UseRateLimiter { get; set; }

    /// <summary>
    /// Maximum number of permit counters that can be allowed in a window.
    /// Must be set to a value > 0.
    /// </summary>
    public int RateLimiterPermitLimit { get; set; } = 30;

    /// <summary>
    /// X-Frame-Options tells the browser whether you want to allow your site to be framed or not.By preventing a browser from framing your site you can defend against attacks like clickjacking. Recommended value "SAMEORIGIN".
    /// </summary>
    /// <value>The x frame options.</value>
    public string XFrameOptions { get; set; } = "SAMEORIGIN";

    /// <summary>
    /// X-Content-Type-Options stops a browser from trying to MIME-sniff the content type and forces it to stick with the declared content-type.The only valid value for this header is "X-Content-Type-Options: nosniff".
    /// </summary>
    /// <value>The x content type options.</value>
    public string XContentTypeOptions { get; set; } = "nosniff";

    /// <summary>
    /// Referrer Policy is a new header that allows a site to control how much information the browser includes with navigations away from a document and should be set by all sites.
    /// </summary>
    /// <value>The referrer policy.</value>
    public string ReferrerPolicy { get; set; } = "no-referrer";

    /// <summary>
    /// Content Security Policy is an effective measure to protect your site from XSS attacks. By whitelisting sources of approved content, you can prevent the browser from loading malicious assets.
    /// </summary>
    /// <value>The permissions policy.</value>
    public string ContentSecurityPolicy { get; set; } = "";
}