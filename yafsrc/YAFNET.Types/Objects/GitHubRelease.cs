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

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YAF.Types.Objects;

/// <summary>
/// Class GitHubRelease.
/// </summary>
public class GitHubRelease
{
    /// <summary>
    /// Gets or sets the name of the tag.
    /// </summary>
    /// <value>The name of the tag.</value>
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; }

    /// <summary>
    /// Gets or sets the published at.
    /// </summary>
    /// <value>The published at.</value>
    [JsonPropertyName("published_at")]
    public DateTime PublishedAt { get; set; }

    /// <summary>
    /// Gets or sets the url of the release page.
    /// </summary>
    /// <value>The release page url.</value>
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; }

    /// <summary>
    /// Gets or sets the files attached to the release.
    /// </summary>
    /// <value>The assets.</value>
    [JsonPropertyName("assets")]
    public List<GitHubReleaseAsset> Assets { get; set; }
}

/// <summary>
/// A file attached to a GitHub release.
/// </summary>
public class GitHubReleaseAsset
{
    /// <summary>
    /// Gets or sets the file name.
    /// </summary>
    /// <value>The name.</value>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the download url.
    /// </summary>
    /// <value>The browser download url.</value>
    [JsonPropertyName("browser_download_url")]
    public string BrowserDownloadUrl { get; set; }
}
