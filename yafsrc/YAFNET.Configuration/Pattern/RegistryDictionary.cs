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

namespace YAF.Configuration.Pattern;

/// <summary>
///     The registry dictionary.
/// </summary>
public class RegistryDictionary : Dictionary<string, object>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegistryDictionary"/> class.
    /// </summary>
    public RegistryDictionary()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    /// <summary>
    ///     The get value.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="defaultValue">
    ///     The default value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public virtual T GetValue<T>(string name, T defaultValue)
    {
        if (!this.ContainsKey(name))
        {
            return defaultValue;
        }

        var value = this[name];

        if (value is null)
        {
            return defaultValue;
        }

        var objectType = typeof(T);

        if (objectType.BaseType == typeof(Enum))
        {
            return (T)Enum.Parse(objectType, value.ToString());
        }

        // special handling for boolean...
        if (objectType == typeof(bool))
        {
            return int.TryParse(value.ToString(), out var i)
                       ? (T)Convert.ChangeType(Convert.ToBoolean(i), typeof(T))
                       : (T)Convert.ChangeType(Convert.ToBoolean(value), typeof(T));
        }

        // special handling for int values...
        if (objectType == typeof(int))
        {
            return (T)Convert.ChangeType(value.ToType<int>(), typeof(T));
        }

        return this[name].ToType<T>();
    }

    /// <summary>
    ///     The set value.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public virtual void SetValue<T>(string name, T value)
    {
        var objectType = typeof(T);
        var stringValue = value.ToType<string>();

        if (objectType == typeof(bool) || objectType.BaseType == typeof(Enum))
        {
            stringValue = Convert.ToString(value.ToType<int>());
        }

        this[name] = stringValue;
    }
}