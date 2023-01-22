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
    /// Gets or sets the google client ID.
    /// </summary>
    /// <value>
    /// The google client ID.
    /// </value>
    public string GoogleClientID { get; set; }

    /// <summary>
    /// Gets or sets the google client secret.
    /// </summary>
    public string GoogleClientSecret { get; set; }

    /// <summary>
    ///     Gets or sets Facebook API Key.
    /// </summary>
    public string FacebookAPIKey { get; set; }

    /// <summary>
    ///     Gets or sets Facebook Secret Key.
    /// </summary>
    public string FacebookSecretKey { get; set; }

    /// <summary>
    ///     Gets or sets MobileUserAgents.
    /// </summary>
    public string MobileUserAgents { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether Boolean to force uploads, and images, themes etc.. from a specific BoardID folder within BoardRoot Example : true /false
    /// </summary>
    public bool MultiBoardFolders { get; set; }

    /// <summary>
    ///     Gets or sets the Folder to use for board specific uploads, images Example : /Boards/
    /// </summary>
    public string BoardRoot { get; set; }

    /// <summary>
    ///     Gets or sets the Current BoardID -- default is 1.
    /// </summary>
    public int SqlCommandTimeout { get; set; }

    /// <summary>
    ///     Gets or sets the TwitterConsumerKey
    /// </summary>
    public string TwitterConsumerKey { get; set; }

    /// <summary>
    ///     Gets or sets the TwitterConsumerSecret
    /// </summary>
    public string TwitterConsumerSecret { get; set; }

    /// <summary>
    /// Gets or sets the banned IP redirect URL.
    /// </summary>
    public string BannedIpRedirectUrl { get; set; }

    /// <summary>
    /// Gets or sets the jQuery version.
    /// </summary>
    public string JQueryVersion { get; set; }

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

    public bool UseHttpsRedirection { get; set; } = true;
}