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
namespace YAF.Types.Extensions
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using YAF.Types.Attributes;

    #endregion

    /// <summary>
    /// The enum extensions.
    /// </summary>
    public static class EnumExtensions
    {
        #region Public Methods

        /// <summary>
        /// Will get the string value for a given enums value, this will
        ///   only work if you assign the StringValue attribute to
        ///   the items in your enum.
        /// </summary>
        /// <param name="value">
        /// </param>
        /// <returns>
        /// The get alt string value.
        /// </returns>
        public static string GetAltStringValue(this Enum value)
        {
            string strValue = string.Empty;

            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            var altAttribs =
              fieldInfo.GetCustomAttributes(typeof(AltStringValueAttribute), false) as AltStringValueAttribute[];

            if (altAttribs != null && altAttribs.Length > 0)
            {
                strValue = altAttribs[0].AltStringValue;
            }
            else
            {
                // Get the stringvalue attributes
                var attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

                if (attribs != null && attribs.Length > 0)
                {
                    strValue = attribs[0].StringValue;
                }
            }

            return strValue.IsNotSet() ? Enum.GetName(type, value) : strValue;
        }

        /// <summary>
        /// Will get the string value for a given enums value, this will
        ///   only work if you assign the StringValue attribute to
        ///   the items in your enum.
        /// </summary>
        /// <param name="value">
        /// </param>
        /// <returns>
        /// The get string value.
        /// </returns>
        public static string GetStringValue(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            if (fieldInfo != null)
            {
                // Get the stringvalue attributes
                var attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

                // Return the first if there was a match.
                if (attribs != null)
                {
                    return attribs.Length > 0 ? attribs[0].StringValue : Enum.GetName(type, value);
                }
            }

            return Enum.GetName(type, value);
        }

        /// <summary>
        /// Checks if an enumerated type contains a value
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="check">
        /// The check.
        /// </param>
        /// <returns>
        /// The has.
        /// </returns>
        public static bool Has<T>(this Enum value, T check)
        {
            Type type = value.GetType();

            // determine the values
            object result = value;
            var parsed = new _Value(check, type);
            if (parsed.Signed is long)
            {
                return (Convert.ToInt64(value) & (long)parsed.Signed) == (long)parsed.Signed;
            }
            else if (parsed.Unsigned is ulong)
            {
                return (Convert.ToUInt64(value) & (ulong)parsed.Unsigned) == (ulong)parsed.Unsigned;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Includes an enumerated type and returns the new value
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="append">
        /// The append.
        /// </param>
        public static T Include<T>(this Enum value, T append)
        {
            Type type = value.GetType();

            // determine the values
            object result = value;
            var parsed = new _Value(append, type);
            if (parsed.Signed is long)
            {
                result = Convert.ToInt64(value) | (long)parsed.Signed;
            }
            else if (parsed.Unsigned is ulong)
            {
                result = Convert.ToUInt64(value) | (ulong)parsed.Unsigned;
            }

            // return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>
        /// Includes an enumerated type and returns the new value
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="bitValues">
        /// The bit Values.
        /// </param>
        public static T Include<T>(this Enum value, ulong bitValues)
        {
            Type type = value.GetType();

            // determine the values
            object result = value;
            var parsed = new _Value(bitValues, type);
            if (parsed.Signed is long)
            {
                result = Convert.ToInt64(value) | (long)parsed.Signed;
            }
            else if (parsed.Unsigned is ulong)
            {
                result = Convert.ToUInt64(value) | (ulong)parsed.Unsigned;
            }

            // return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>
        /// Checks if an enumerated type is missing a value
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The missing.
        /// </returns>
        public static bool Missing<T>(this Enum obj, T value)
        {
            return !obj.Has(value);
        }

        /// <summary>
        /// Removes an enumerated type and returns the new value
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="remove">
        /// The remove.
        /// </param>
        public static T Remove<T>(this Enum value, T remove)
        {
            Type type = value.GetType();

            // determine the values
            object result = value;
            var parsed = new _Value(remove, type);
            if (parsed.Signed is long)
            {
                result = Convert.ToInt64(value) & (long)parsed.Signed;
            }
            else if (parsed.Unsigned is ulong)
            {
                result = Convert.ToUInt64(value) & (ulong)parsed.Unsigned;
            }

            // return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>
        /// Will get the enum value as an Byte saving a cast (byte).
        /// </summary>
        /// <param name="value">
        /// </param>
        /// <returns>
        /// The to byte.
        /// </returns>
        public static byte ToByte(this Enum value)
        {
            return Convert.ToByte(value);
        }

        /// <summary>
        /// Will get the enum value as an char saving a cast (char).
        /// </summary>
        /// <param name="value">
        /// </param>
        /// <returns>
        /// The to char.
        /// </returns>
        public static char ToChar(this Enum value)
        {
            return Convert.ToChar(value);
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
            Type enumType = typeof(T);
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
        public static T ToEnum<T>(this ulong value)
        {
            Type enumType = typeof(T);
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
            Type enumType = typeof(T);
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
            Type enumType = typeof(T);
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
            Type enumType = typeof(T);
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
        /// </param>
        /// <returns>
        /// The to int.
        /// </returns>
        public static int ToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        #endregion

        /// <summary>
        /// The _ value.
        /// </summary>
        private class _Value
        {
            // cached comparisons for tye to use
            #region Constants and Fields

            /// <summary>
            /// The signed.
            /// </summary>
            public readonly long? Signed;

            /// <summary>
            /// The unsigned.
            /// </summary>
            public readonly ulong? Unsigned;

            /// <summary>
            /// The int.
            /// </summary>
            public long? Int;

            /// <summary>
            /// The _ int 32.
            /// </summary>
            private static readonly Type _Int32 = typeof(int);

            /// <summary>
            /// The _ u int 32.
            /// </summary>
            private static readonly Type _UInt32 = typeof(long);

            /// <summary>
            /// The _ u int 64.
            /// </summary>
            private static readonly Type _UInt64 = typeof(ulong);

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="_Value"/> class.
            /// </summary>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <param name="type">
            /// The type.
            /// </param>
            /// <exception cref="ArgumentException">
            /// </exception>
            public _Value(object value, Type type)
            {
                // make sure it is even an enum to work with
                if (!type.IsEnum)
                {
                    throw new ArgumentException("Value provided is not an enumerated type!");
                }

                // then check for the enumerated value
                Type compare = Enum.GetUnderlyingType(type);

                // if this is an unsigned long then the only
                // value that can hold it would be a ulong
                if (compare.Equals(_UInt32) || compare.Equals(_UInt64))
                {
                    this.Unsigned = Convert.ToUInt64(value);
                }
                else if (compare.Equals(_Int32))
                {
                    this.Int = Convert.ToInt32(value);
                }

                  // otherwise, a long should cover anything else
                else
                {
                    this.Signed = Convert.ToInt64(value);
                }
            }

            #endregion
        }
    }
}