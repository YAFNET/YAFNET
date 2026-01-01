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

namespace YAF.Types.InputModels;

/// <summary>
/// The input model.
/// </summary>
public class EditMedalInputModel
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>The message.</value>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the category.
    /// </summary>
    /// <value>The category.</value>
    public string Category { get; set; }

    /// <summary>
    /// Gets or sets the medal image.
    /// </summary>
    /// <value>The medal image.</value>
    public string MedalImage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show message].
    /// </summary>
    /// <value><c>true</c> if [show message]; otherwise, <c>false</c>.</value>
    public bool ShowMessage { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether [allow hiding].
    /// </summary>
    /// <value><c>true</c> if [allow hiding]; otherwise, <c>false</c>.</value>
    public bool AllowHiding { get; set; } = true;
}