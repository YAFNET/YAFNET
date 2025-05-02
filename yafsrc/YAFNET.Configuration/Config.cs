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

namespace YAF.Configuration;

/// <summary>
/// Static class that access the app settings in the web.config file
/// </summary>
public static class Config
{
    /// <summary>
    /// Gets or sets the SQL Command Timeout.
    /// </summary>
    public static int SqlCommandTimeout { get; set; }

    /// <summary>
    /// Gets or sets the database schema.
    /// </summary>
    public static string DatabaseSchema { get; set; }

    /// <summary>
    ///     Gets or sets DatabaseObjectQualifier.
    /// </summary>
    public static string DatabaseObjectQualifier { get; set; }

    /// <summary>
    ///     Gets or sets DatabaseOwner.
    /// </summary>
    public static string DatabaseOwner { get; set; }

    /// <summary>
    ///     Gets or sets the Current BoardID -- default is 1.
    /// </summary>
    public static int BoardID { get; set; }

    /// <summary>
    ///     Gets or sets the Current CategoryID -- default is null.
    /// </summary>
    public static int CategoryID { get; set; }

    /// <summary>
    /// Gets or sets the URL rewriting mode.
    /// </summary>
    /// <value>The URL rewriting mode.</value>
    public static string UrlRewritingMode { get; set; }

    /// <summary>
    /// Gets or sets the legacy membership hash algorithm type.
    /// </summary>
    public static HashAlgorithmType LegacyMembershipHashAlgorithmType { get; set; }

    /// <summary>
    /// Gets or sets the legacy membership hash case.
    /// </summary>
    public static HashCaseType LegacyMembershipHashCase { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether legacy membership hash hex.
    /// </summary>
    public static bool LegacyMembershipHashHex { get; set; }

    /// <summary>
    /// Gets or sets the web root path.
    /// </summary>
    /// <value>The web root path.</value>
    public static string WebRootPath { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [use rate limiter].
    /// </summary>
    /// <value><c>true</c> if [use rate limiter]; otherwise, <c>false</c>.</value>
    public static bool UseRateLimiter { get; set; }
}