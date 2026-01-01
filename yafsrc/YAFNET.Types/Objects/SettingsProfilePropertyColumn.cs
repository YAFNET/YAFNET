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

using System.Configuration;
using System.Data;

namespace YAF.Types.Objects;

/// <summary>
/// The settings property column.
/// </summary>
public class SettingsPropertyColumn
{
    /// <summary>
    /// The data type.
    /// </summary>
    public SqlDbType DataType { get; }

    /// <summary>
    /// The settings.
    /// </summary>
    public SettingsProperty Settings { get; }

    /// <summary>
    /// The size.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPropertyColumn"/> class.
    /// </summary>
    public SettingsPropertyColumn()
    {
        // empty for default constructor...
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPropertyColumn"/> class.
    /// </summary>
    /// <param name="settings">
    /// The settings.
    /// </param>
    /// <param name="dataType">
    /// The data type.
    /// </param>
    /// <param name="size">
    /// The size.
    /// </param>
    public SettingsPropertyColumn(SettingsProperty settings, SqlDbType dataType, int size)
    {
        this.DataType = dataType;
        this.Settings = settings;
        this.Size = size;
    }
}