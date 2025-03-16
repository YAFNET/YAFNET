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

namespace YAF.Configuration;

using System.Reflection;

/// <summary>
/// Gets the Board Settings as dictionary item for easy iteration.
/// </summary>
public class BoardSettingCollection
{
    /// <summary>
    /// The settings.
    /// </summary>
    private readonly List<PropertyInfo> settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoardSettingCollection"/> class.
    /// </summary>
    /// <param name="boardSettings">
    /// The board settings.
    /// </param>
    public BoardSettingCollection(BoardSettings boardSettings)
    {
        // load up the settings...
        var boardSettingsType = boardSettings.GetType();
        this.settings = [.. boardSettingsType.GetProperties()];
    }

    /// <summary>
    /// Gets SettingsString.
    /// </summary>
    public Dictionary<string, PropertyInfo> SettingsString =>
        this.settings.Where(x => x.PropertyType == typeof(string)).ToDictionary(x => x.Name, x => x);

    /// <summary>
    /// Gets Boolean Settings.
    /// </summary>
    public Dictionary<string, PropertyInfo> SettingsBool =>
        this.settings.Where(x => x.PropertyType == typeof(bool)).ToDictionary(x => x.Name, x => x);

    /// <summary>
    /// Gets Integer Settings.
    /// </summary>
    public Dictionary<string, PropertyInfo> SettingsInt =>
        this.settings.Where(x => x.PropertyType == typeof(int)).ToDictionary(x => x.Name, x => x);

    /// <summary>
    /// Gets SettingsOther.
    /// </summary>
    public Dictionary<string, PropertyInfo> SettingsOther
    {
        get
        {
            var excludeTypes = new List<Type> { typeof(string), typeof(bool), typeof(int), typeof(double) };

            return this.settings.Where(x => !excludeTypes.Contains(x.PropertyType))
                .ToDictionary(x => x.Name, x => x);
        }
    }
}