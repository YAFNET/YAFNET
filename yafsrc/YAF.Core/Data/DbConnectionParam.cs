/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Core.Data;

using YAF.Types.Attributes;

/// <summary>
///     The database connection parameter.
/// </summary>
public struct DbConnectionParam : IDbConnectionParam
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbConnectionParam"/> struct.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="defaultValue">
    /// The default value.
    /// </param>
    public DbConnectionParam(int id, string name, [NotNull] string defaultValue = null)
    {
        this.ID = id;
        this.Name = name;
        this.Value = defaultValue ?? string.Empty;
    }

    /// <summary>
    ///     Gets or sets the ID.
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Gets the Label.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the Default Value.
    /// </summary>
    public string Value { get; }
}