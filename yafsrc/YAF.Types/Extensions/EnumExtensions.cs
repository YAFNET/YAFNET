/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Types.Extensions
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using YAF.Types.Attributes;

    #endregion

    /// <summary>
    /// The Enumerator Extensions
    /// </summary>
    public static class EnumExtensions
    {
        #region Public Methods

        /// <summary>
        /// Gets all items for an enum type.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<T> GetAllItems<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Will get the string value for a given enums value, this will
        ///   only work if you assign the StringValue attribute to
        ///   the items in your enum.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetStringValue(this Enum value)
        {
            // Get the type
            var type = value.GetType();

            // Get field info for this type
            var fieldInfo = type.GetField(value.ToString());

            if (fieldInfo == null)
            {
                return Enum.GetName(type, value);
            }

            // Return the first if there was a match.
            if (fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) is StringValueAttribute[] attribs)
            {
                return attribs.Length > 0 ? attribs[0].StringValue : Enum.GetName(type, value);
            }

            return Enum.GetName(type, value);
        }

        /// <summary>
        /// The to enum.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        public static T ToEnum<T>(this int value)
        {
            var enumType = typeof(T);
            if (enumType.BaseType != typeof(Enum))
            {
                throw new ApplicationException("ToEnum does not support non-enum types");
            }

            return (T)Enum.Parse(enumType, value.ToString());
        }

        /// <summary>
        /// The to enum.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        public static T ToEnum<T>(this string value)
        {
            var enumType = typeof(T);
            if (enumType.BaseType != typeof(Enum))
            {
                throw new ApplicationException("ToEnum does not support non-enum types");
            }

            return (T)Enum.Parse(enumType, value);
        }

        /// <summary>
        /// The to enum.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="ignoreCase">
        /// The ignore Case.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        public static T ToEnum<T>(this string value, bool ignoreCase)
        {
            var enumType = typeof(T);
            if (enumType.BaseType != typeof(Enum))
            {
                throw new ApplicationException("ToEnum does not support non-enum types");
            }

            return (T)Enum.Parse(enumType, value, ignoreCase);
        }

        /// <summary>
        /// The to enum.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        public static T ToEnum<T>(this object value)
        {
            var enumType = typeof(T);
            if (enumType.BaseType != typeof(Enum))
            {
                throw new ApplicationException("ObjToEnum does not support non-enum types");
            }

            return (T)Enum.Parse(enumType, value.ToString());
        }

        /// <summary>
        /// Will get the enum value as an integer saving a cast (int).
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The to int.
        /// </returns>
        public static int ToInt(this Enum value)
        {
            return value.ToType<int>();
        }

        #endregion
    }
}