/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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
            Type enumType = typeof(T);

            // Can't use type constraints on value types, so have to do check like this
            if (enumType.BaseType != typeof(Enum))
            {
                throw new ArgumentException("EnumToList does not support non-enum types");
            }

            Array enumValArray = Enum.GetValues(enumType);

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
            Type enumType = typeof(TEnum);

            if (enumType.BaseType != typeof(Enum))
            {
                throw new ApplicationException("Enum To Dictionary conversion does not support non-enum types");
            }

            var list = new Dictionary<TValue, string>();

            foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
            {
                var value = (TValue)field.GetValue(null);
                string display = Enum.GetName(enumType, value);

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