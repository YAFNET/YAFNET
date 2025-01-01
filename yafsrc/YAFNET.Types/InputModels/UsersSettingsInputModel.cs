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

namespace YAF.Types.InputModels;

/// <summary>
/// The input model.
/// </summary>
public class UsersSettingsInputModel
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>The user identifier.</value>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether activity.
    /// </summary>
    /// <value><c>true</c> if activity; otherwise, <c>false</c>.</value>
    public bool Activity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether hide me.
    /// </summary>
    /// <value><c>true</c> if [hide me]; otherwise, <c>false</c>.</value>
    public bool HideMe { get; set; }

    /// <summary>
    /// Gets or sets the time zone.
    /// </summary>
    /// <value>The time zone.</value>
    public string TimeZone { get; set; }

    /// <summary>
    /// Gets or sets the theme.
    /// </summary>
    /// <value>The theme.</value>
    public string Theme { get; set; }

    /// <summary>
    /// Gets or sets the language.
    /// </summary>
    /// <value>The language.</value>
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    /// <value>The email.</value>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>The size.</value>
    public int Size { get; set; }
}