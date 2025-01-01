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

namespace YAF.Types.Extensions;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The Enumerator Extensions
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets all items for an Enumerator type.
    /// </summary>
    /// <typeparam name="T">
    /// the Typed Parameter
    /// </typeparam>
    /// <returns>
    /// Returns all Enumerator Items
    /// </returns>
    public static IEnumerable<T> GetAllItems<T>() where T : struct
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    /// <summary>
    /// Converts A Integer to an Enumerator.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <typeparam name="T">
    /// The Typed Enumerator.
    /// </typeparam>
    /// <returns>
    /// Returns the Typed Enumerator.
    /// </returns>
    public static T ToEnum<T>(this int value)
    {
        var enumType = typeof(T);
        if (enumType.BaseType != typeof(Enum))
        {
            throw new ArgumentNullException(nameof(value), "ToEnum does not support non-enum types");
        }

        return (T)Enum.Parse(enumType, value.ToString());
    }

    /// <summary>
    /// Converts A String to an Enumerator.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <typeparam name="T">
    /// The Typed Enumerator.
    /// </typeparam>
    /// <returns>
    /// Returns the Typed Enumerator.
    /// </returns>
    public static T ToEnum<T>(this string value)
    {
        var enumType = typeof(T);
        if (enumType.BaseType != typeof(Enum))
        {
            throw new ArgumentNullException(nameof(value), "ToEnum does not support non-enum types");
        }

        return (T)Enum.Parse(enumType, value);
    }

    /// <summary>
    /// Converts A String to an Enumerator.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="ignoreCase">
    /// The ignore Case.
    /// </param>
    /// <typeparam name="T">
    /// The Typed Enumerator.
    /// </typeparam>
    /// <returns>
    /// Returns the Typed Enumerator.
    /// </returns>
    public static T ToEnum<T>(this string value, bool ignoreCase)
    {
        var enumType = typeof(T);
        if (enumType.BaseType != typeof(Enum))
        {
            throw new ArgumentNullException(nameof(value), "ToEnum does not support non-enum types");
        }

        return (T)Enum.Parse(enumType, value, ignoreCase);
    }

    /// <summary>
    /// Will get the Enumerator value as an integer saving a cast
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <returns>
    /// The Integer value.
    /// </returns>
    public static int ToInt(this Enum value)
    {
        return value.ToType<int>();
    }
}