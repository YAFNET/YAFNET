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
/// The YAF version.
/// </summary>
public class YafVersion
{
    /// <summary>
    /// Gets or sets the major.
    /// </summary>
    public int Major { get; set; }

    /// <summary>
    /// Gets or sets the minor.
    /// </summary>
    public int Minor { get; set; }

    /// <summary>
    /// Gets or sets the build.
    /// </summary>
    public int Build { get; set; }

    /// <summary>
    /// Gets or sets the release type.
    /// </summary>
    public ReleaseType ReleaseType { get; set; }

    /// <summary>
    /// Gets or sets the release number.
    /// </summary>
    public int ReleaseNumber { get; set; }
}