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

/// <summary>
/// Class UserRequestData.
/// </summary>
public class UserRequestData
{
    /// <summary>
    /// Gets or sets a value indicating whether [dont track].
    /// </summary>
    /// <value><c>true</c> if [dont track]; otherwise, <c>false</c>.</value>
    public bool DontTrack { get; set; }

    /// <summary>
    /// Gets or sets the user agent.
    /// </summary>
    /// <value>The user agent.</value>
    public string UserAgent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is search engine.
    /// </summary>
    /// <value><c>true</c> if this instance is search engine; otherwise, <c>false</c>.</value>
    public bool IsSearchEngine { get; set; }

    /// <summary>
    /// Gets or sets the browser.
    /// </summary>
    /// <value>The browser.</value>
    public string Browser { get; set; }

    /// <summary>
    /// Gets or sets the platform.
    /// </summary>
    /// <value>The platform.</value>
    public string Platform { get; set; }
}