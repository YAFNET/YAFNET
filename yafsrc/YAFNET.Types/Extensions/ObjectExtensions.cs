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

namespace YAF.Types.Extensions;

using System.ComponentModel;
using System.Linq;

/// <summary>
///     The object extensions.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Tests if an object or empty.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <returns>
    /// The is <see langword="null"/> or empty database field.
    /// </returns>
    public static bool IsNullOrEmptyField(this object value)
    {
        return value is null || value == DBNull.Value || value.ToString().IsNotSet();
    }

    /// <summary>
    /// The get attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <param name="objectType">The object type.</param>
    /// <returns>
    /// The <see cref="TAttribute" />.
    /// </returns>
    public static TAttribute GetAttribute<TAttribute>(this Type objectType) where TAttribute : Attribute
    {
         return objectType.GetCustomAttributes(typeof(TAttribute), false).OfType<TAttribute>().FirstOrDefault();
    }

    /// <summary>
    /// Does this instance have this interface?
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="instance">
    /// </param>
    /// <returns>
    /// The has interface.
    /// </returns>
    public static bool HasInterface<T>(this object instance)
    {
        return instance is T;
    }

    /// <summary>
    /// Checks if source is in the list provided.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="source">
    /// </param>
    /// <param name="list">
    /// </param>
    /// <returns>
    /// The is in.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="source"/>
    ///     is
    ///     <c>null</c>
    ///     .
    /// </exception>
    public static bool IsIn<T>(this T source, params T[] list)
    {
        return list.Contains(source);
    }

    /// <param name="instance">
    /// </param>
    extension(object instance)
    {
        /// <summary>
        /// Converts an object to Type using the Convert.ChangeType() call.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T ToType<T>()
        {
            if (instance is null)
            {
                return default;
            }

            if (Equals(instance, default(T)))
            {
                return default;
            }

            if (Equals(instance, DBNull.Value))
            {
                return default;
            }

            var instanceType = instance.GetType();

            if (instanceType == typeof(string))
            {
                if ((instance as string).IsNotSet())
                {
                    return default;
                }
            }
            else if (instanceType.IsClass && instance is not IConvertible)
            {
                // just cast since it's a class....
                return (T)instance;
            }

            var conversionType = typeof(T);

            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                conversionType = new NullableConverter(conversionType).UnderlyingType;
            }

            return (T)Convert.ChangeType(instance, conversionType);
        }

        /// <summary>
        /// The to type or default.
        /// </summary>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T ToTypeOrDefault<T>(T defaultValue)
        {
            try
            {
                return ToType<T>(instance);
            }
            catch (ArgumentNullException)
            {
                // ignore
            }
            catch (FormatException)
            {
                // ignore
            }
            catch (InvalidCastException)
            {
                // ignore
            }
            catch (OverflowException)
            {
                // ignore
            }

            return defaultValue;
        }
    }
}