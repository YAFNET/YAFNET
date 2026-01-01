
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

namespace YAF.Core.Helpers;

using System;
using System.Collections.Generic;
using System.Reflection;

using YAF.Types.Attributes;

/// <summary>
///     The Enumerator helper.
/// </summary>
public static class EnumHelper
{
    /// <summary>
    /// Converts an enumerator to a Dictionary
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// Returns the Dictionary
    /// </returns>
    public static IDictionary<int, string> EnumToDictionary<T>()
    {
        return InternalToDictionary<T, int>();
    }

    /// <summary>
    /// Converts an enumerator to a List
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public static List<T> EnumToList<T>()
    {
        var enumType = typeof(T);

        // Can't use type constraints on value types, so have to do check like this
        if (enumType.BaseType != typeof(Enum))
        {
            throw new ArgumentException("EnumToList does not support non-enum types");
        }

        var enumValArray = Enum.GetValues(enumType);

        return [.. enumValArray.Cast<int>().Select(val => (T)Enum.Parse(enumType, val.ToString(CultureInfo.InvariantCulture)))];
    }

    /// <summary>
    /// The internal to dictionary.
    /// </summary>
    /// <typeparam name="TEnum">
    /// </typeparam>
    /// <typeparam name="TValue">
    /// </typeparam>
    /// <returns>
    /// The <see cref="IDictionary"/>.
    /// </returns>
    private static IDictionary<TValue, string> InternalToDictionary<TEnum, TValue>()
    {
        var enumType = typeof(TEnum);

        if (enumType.BaseType != typeof(Enum))
        {
            throw new ArgumentException("Enum To Dictionary conversion does not support non-enum types");
        }

        var list = new Dictionary<TValue, string>();

        enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public).ForEach(field =>
            {
                var value = (TValue)field.GetValue(null);
                var display = Enum.GetName(enumType, value);

                // Return the first if there was a match.
                if (field.GetCustomAttributes(typeof(StringValueAttribute), false) is StringValueAttribute[]
                        {
                            Length: > 0
                        } attribs)
                {
                    display = attribs[0].StringValue;
                }

                // add the value...
                list.Add(value, display);
            });

        return list;
    }
}