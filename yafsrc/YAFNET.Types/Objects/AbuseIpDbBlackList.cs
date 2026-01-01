/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// AbuseIpDB.com Blacklist
/// https://docs.abuseipdb.com/#blacklist-endpoint
/// </summary>
public class AbuseIpDbBlackList
{
    /// <summary>
    /// Gets or sets the meta.
    /// </summary>
    /// <value>The meta.</value>
    [JsonPropertyName("meta")]
    public Meta Meta { get; set; }

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    /// <value>The data.</value>
    [JsonPropertyName("data")]
    public List<BlackListEntry> Data { get; set; }
}

/// <summary>
/// Class Meta.
/// </summary>
public class Meta
{
    /// <summary>
    /// Gets or sets the generated at.
    /// </summary>
    /// <value>The generated at.</value>
    [JsonPropertyName("generatedAt")]
    public DateTime GeneratedAt { get; set; }
}

/// <summary>
/// Class BlackListEntry.
/// </summary>
public class BlackListEntry
{
    /// <summary>
    /// Gets or sets the ip address.
    /// </summary>
    /// <value>The ip address.</value>
    [JsonPropertyName("ipAddress")]
    public string IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the abuse confidence score.
    /// </summary>
    /// <value>The abuse confidence score.</value>
    [JsonPropertyName("abuseConfidenceScore")]
    public int AbuseConfidenceScore { get; set; }

    /// <summary>
    /// Gets or sets the last reported at.
    /// </summary>
    /// <value>The last reported at.</value>
    [JsonPropertyName("lastReportedAt")]
    public DateTime LastReportedAt { get; set; }
}