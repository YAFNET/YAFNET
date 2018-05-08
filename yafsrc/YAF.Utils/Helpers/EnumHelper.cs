/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Utils
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using YAF.Types.Attributes;

    #endregion

    /// <summary>
    ///     The enum helper.
    /// </summary>
    public static class EnumHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// Converts an Enum to a Dictionary
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IDictionary"/>.
        /// </returns>
        public static IDictionary<int, string> EnumToDictionary<T>()
        {
            return InternalToDictionary<T, int>();
        }

        /// <summary>
        /// Converts an Enum to a Dictionary
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IDictionary"/>.
        /// </returns>
        public static IDictionary<byte, string> EnumToDictionaryByte<T>()
        {
            return InternalToDictionary<T, byte>();
        }

        /// <summary>
        /// Converts an Enum to a List
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<T> EnumToList<T>()
        {
            var enumType = typeof(T);

            // Can't use type constraints on value types, so have to do check like this
            if (enumType.BaseType != typeof(Enum))
            {
                throw new ArgumentException("EnumToList does not support non-enum types");
            }

            var enumValArray = Enum.GetValues(enumType);

            return enumValArray.Cast<int>().Select(val => (T)Enum.Parse(enumType, val.ToString(CultureInfo.InvariantCulture))).ToList();
        }

        #endregion

        #region Methods

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
        /// <exception cref="ApplicationException">
        /// </exception>
        private static IDictionary<TValue, string> InternalToDictionary<TEnum, TValue>()
        {
            var enumType = typeof(TEnum);

            if (enumType.BaseType != typeof(Enum))
            {
                throw new ApplicationException("Enum To Dictionary conversion does not support non-enum types");
            }

            var list = new Dictionary<TValue, string>();

            foreach (var field in enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
            {
                var value = (TValue)field.GetValue(null);
                var display = Enum.GetName(enumType, value);

                var attribs = field.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

                // Return the first if there was a match.
                if (attribs != null && attribs.Length > 0)
                {
                    display = attribs[0].StringValue;
                }

                // add the value...
                list.Add(value, display);
            }

            return list;
        }

        #endregion
    }
}