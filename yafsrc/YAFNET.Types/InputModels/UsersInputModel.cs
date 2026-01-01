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
public class UsersInputModel
{
    /// <summary>
    /// Gets or sets the years old.
    /// </summary>
    /// <value>The years old.</value>
    public int YearsOld { get; set; } = 5;

    /// <summary>
    /// Gets or sets a value indicating whether [suspended only].
    /// </summary>
    /// <value><c>true</c> if [suspended only]; otherwise, <c>false</c>.</value>
    public bool SuspendedOnly { get; set; }

    /// <summary>
    /// Gets or sets the since.
    /// </summary>
    /// <value>The since.</value>
    public string Since { get; set; }

    /// <summary>
    /// Gets or sets the rank.
    /// </summary>
    /// <value>The rank.</value>
    public int Rank { get; set; }

    /// <summary>
    /// Gets or sets the group.
    /// </summary>
    /// <value>The group.</value>
    public int Group { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    /// <value>The email.</value>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }
}