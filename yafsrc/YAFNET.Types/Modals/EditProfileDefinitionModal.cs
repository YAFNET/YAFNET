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

namespace YAF.Types.Modals;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Class EditProfileDefinitionModal.
/// </summary>
public class EditProfileDefinitionModal
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the type of the data.
    /// </summary>
    /// <value>The type of the data.</value>
    public string DataType { get; set; }

    /// <summary>
    /// Gets or sets the length.
    /// </summary>
    /// <value>The length.</value>
    [Required]
    public int Length { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="EditProfileDefinitionModal"/> is required.
    /// </summary>
    /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show in user information].
    /// </summary>
    /// <value><c>true</c> if [show in user information]; otherwise, <c>false</c>.</value>
    public bool ShowInUserInfo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show on register page].
    /// </summary>
    /// <value><c>true</c> if [show on register page]; otherwise, <c>false</c>.</value>
    public bool ShowOnRegisterPage { get; set; }

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    /// <value>The default value.</value>
    public string DefaultValue { get; set; }
}