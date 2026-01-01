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
/// The culture.
/// </summary>
public class Culture
{
    /// <summary>
    /// Gets or sets the culture tag.
    /// </summary>
    public string CultureTag { get; set; }

    /// <summary>
    /// Gets or sets the culture file.
    /// </summary>
    public string CultureFile { get; set; }

    /// <summary>
    /// Gets or sets the culture english name.
    /// </summary>
    public string CultureEnglishName { get; set; }

    /// <summary>
    /// Gets or sets the culture native name.
    /// </summary>
    public string CultureNativeName { get; set; }

    /// <summary>
    /// Gets or sets the culture display name.
    /// </summary>
    public string CultureDisplayName { get; set; }

    /// <summary>
    /// Gets or sets the translated percentage.
    /// </summary>
    public int TranslatedPercentage { get; set; }

    /// <summary>
    /// Gets or sets the translated count.
    /// </summary>
    /// <value>The translated count.</value>
    public int TranslatedCount { get; set; }

    /// <summary>
    /// Gets or sets the tags count.
    /// </summary>
    /// <value>The tags count.</value>
    public int TagsCount { get; set; }
}