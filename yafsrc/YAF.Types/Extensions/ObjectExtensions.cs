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
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.UI;

    using YAF.Types.Attributes;

    #endregion

    /// <summary>
    ///     The object extensions.
    /// </summary>
    public static class ObjectExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Turns any object into a Dictionary
        /// </summary>
        /// <param name="thingy">
        /// The thingy. 
        /// </param>
        /// <returns>
        /// The <see cref="IDictionary"/>.
        /// </returns>
        public static IDictionary<string, object> AnyToDictionary([NotNull] this object thingy)
        {
            CodeContracts.VerifyNotNull(thingy, "thingy");

            return (IDictionary<string, object>)thingy.ToExpando();
        }

        /// <summary>
        /// Converts an object to a type.
        /// </summary>
        /// <param name="value">
        /// Object to convert 
        /// </param>
        /// <param name="type">
        /// Type to convert to e.g. System.Guid 
        /// </param>
        /// <returns>
        /// The convert object to type. 
        /// </returns>
        [CanBeNull]
        public static object ConvertObjectToType([CanBeNull] object value, [NotNull] string type)
        {
            if (value == null)
            {
                return null;
            }

            var convertType = Type.GetType(type, true, true);

            var converter = TypeDescriptor.GetConverter(convertType);
            return converter.ConvertFrom(value);
        }

        /// <summary>
        /// Provides a chaining action with the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="action">The action.</param>
        /// <returns>
        /// The <see cref="T" />.
        /// </returns>
        public static T DoWith<T>(this T obj, [NotNull] Action<T> action)
        {
            action(obj);
            return obj;
        }

        /// <summary>
        /// The get attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="objectType">The object type.</param>
        /// <returns>
        /// The <see cref="TAttribute" />.
        /// </returns>
        public static TAttribute GetAttribute<TAttribute>([NotNull] this Type objectType) where TAttribute : Attribute
        {
            CodeContracts.VerifyNotNull(objectType, "objectType");

            return objectType.GetCustomAttributes(typeof(TAttribute), false).OfType<TAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// The get attributes.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="objectType">The object type.</param>
        /// <returns>
        /// The <see cref="IEnumerable" />.
        /// </returns>
        [NotNull]
        public static IEnumerable<TAttribute> GetAttributes<TAttribute>([NotNull] this Type objectType)
            where TAttribute : Attribute
        {
            CodeContracts.VerifyNotNull(objectType, "objectType");

            return objectType.GetCustomAttributes(typeof(TAttribute), false).OfType<TAttribute>();
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
        public static bool HasInterface<T>([NotNull] this object instance)
        {
            return instance is T;
        }

        /// <summary>
        /// The is default.
        /// </summary>
        /// <param name="valueType">
        /// The value type.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsDefault<T>(this T valueType)
            where T : struct
        {
            return Equals(valueType, default(T));
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
        public static bool IsIn<T>(this T source, [NotNull] params T[] list)
        {
            CodeContracts.VerifyNotNull(list, "list");

            return list.Contains(source);
        }

        /// <summary>
        /// The is not default.
        /// </summary>
        /// <param name="valueType">
        /// The value type.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsNotDefault<T>(this T valueType)
            where T : struct
        {
            return !Equals(valueType, default(T));
        }

        /// <summary>
        /// Converts the object to the class (T) or returns null if it's not an instance of that class or instance is null.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="instance">
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        [CanBeNull]
        public static T ToClass<T>([CanBeNull] this object instance) where T : class
        {
            return instance as T;
        }

        /// <summary>
        /// Converts an object to a different object (class) by copying fields (if they exist). Used to convert annonomous objects to strongly typed objects.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        [NotNull]
        public static T ToDifferentClassType<T>([NotNull] this object obj) where T : class
        {
            // create instance of T type object:
            var tmp = Activator.CreateInstance(typeof(T));

            // loop through the fields of the object you want to covert:       
            foreach (var fi in obj.GetType().GetFields())
            {
                try
                {
                    tmp.GetType().GetField(fi.Name).SetValue(tmp, fi.GetValue(obj));
                }
                catch
                {
                }
            }

            // return the T type object:         
            return (T)tmp;
        }

        /// <summary>
        /// The to dynamic.
        /// </summary>
        /// <param name="instance">
        /// The instance. 
        /// </param>
        /// <returns>
        /// The to dynamic. 
        /// </returns>
        public static dynamic ToDynamic([NotNull] this object instance)
        {
            return instance;
        }

        /// <summary>
        /// Turns the object into an ExpandoObject
        /// </summary>
        /// <param name="obj">
        /// The object. 
        /// </param>
        /// <returns>
        /// The to expando. 
        /// </returns>
        [NotNull]
        public static dynamic ToExpando([NotNull] this object obj)
        {
            CodeContracts.VerifyNotNull(obj, "obj");

            var result = new ExpandoObject();

            var d = result as IDictionary<string, object>;

            if (obj is ExpandoObject)
            {
                return obj;
            }

            if (obj.GetType().IsSubclassOf(typeof(NameValueCollection)))
            {
                var nameValueCollection = (NameValueCollection)obj;
                nameValueCollection.Cast<string>()
                    .Select(key => new KeyValuePair<string, object>(key, nameValueCollection[key]))
                    .ToList()
                    .ForEach(d.Add);
            }
            else
            {
                var props = obj.GetType().GetProperties();

                props.Where(p => !p.GetCustomAttributes(typeof(ExcludeAttribute), true).Any()).ToList().ForEach(
                    item => d.Add(item.Name, item.GetValue(obj, null)));
            }

            return result;
        }

        /// <summary>
        /// The to generic list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listObjects">The list objects.</param>
        /// <returns>
        /// The <see cref="List" />.
        /// </returns>
        [NotNull]
        public static List<T> ToGenericList<T>([NotNull] this IList listObjects)
        {
            var convertedList = new List<T>(listObjects.Count);

            convertedList.AddRange(listObjects.Cast<T>());

            return convertedList;
        }

        // The ToString overloads are inspired by:
        // A Smarter (or Pure Evil) ToString with Extension Methods (4)
        // FormatWith 2.0 - String formatting with named variables (5)
        // This implementation is a combination of both: the regular expressions 
        // from Scott Hanselman combined with the DataBinder.Eval idea 
        // from James Newton-King.

        /// <summary>
        /// Enables you to get a string representation of the object using string formatting with property names, rather than index based values.
        /// </summary>
        /// <param name="anObject">
        /// The object being extended. 
        /// </param>
        /// <param name="aFormat">
        /// The formatting string, like "Hi, my name is {FirstName} {LastName}". 
        /// </param>
        /// <returns>
        /// A formatted string with the values from the object replaced in the format string. 
        /// </returns>
        /// <remarks>
        /// To embed a pair of {} on the string, simply double them: "I am a {{Literal}}".
        /// </remarks>
        [NotNull]
        public static string ToString([NotNull] this object anObject, [NotNull] string aFormat)
        {
            return anObject.ToString(aFormat, null);
        }

        /// <summary>
        /// Enables you to get a string representation of the object using string formatting with property names, rather than index based values.
        /// </summary>
        /// <param name="anObject">
        /// The object being extended. 
        /// </param>
        /// <param name="aFormat">
        /// The formatting string, like "Hi, my name is {FirstName} {LastName}". 
        /// </param>
        /// <param name="formatProvider">
        /// An System. <see cref="IFormatProvider"/> that provides culture-specific formatting information. 
        /// </param>
        /// <returns>
        /// A formatted string with the values from the object replaced in the format string. 
        /// </returns>
        /// <remarks>
        /// To embed a pair of {} on the string, simply double them: "I am a {{Literal}}".
        /// </remarks>
        [NotNull]
        public static string ToString(
            [NotNull] this object anObject, [NotNull] string aFormat, [NotNull] IFormatProvider formatProvider)
        {
            var sb = new StringBuilder();
            var reg = new Regex(@"({)([^}]+)(})", RegexOptions.IgnoreCase);
            var mc = reg.Matches(aFormat);

            var startIndex = 0;

            foreach (Match m in mc)
            {
                var g = m.Groups[2];
                var length = g.Index - startIndex - 1;
                sb.Append(aFormat.Substring(startIndex, length));

                string getValue;
                var format = string.Empty;

                var formatIndex = g.Value.IndexOf(":", StringComparison.Ordinal);
                if (formatIndex == -1)
                {
                    getValue = g.Value;
                }
                else
                {
                    getValue = g.Value.Substring(0, formatIndex);
                    format = g.Value.Substring(formatIndex + 1);
                }

                // Make sure we're not dealing 
                if (!getValue.StartsWith("{"))
                {
                    // with a string literal wrapped in {}
                    // Get the object's value using DataBinder.Eval.
                    var resultAsObject = DataBinder.Eval(anObject, getValue);

                    // Format the value based on the incoming formatProvider 
                    // and format string
                    var result = string.Format(formatProvider, "{0:" + format + "}", resultAsObject);

                    sb.Append(result);
                }
                else
                {
                    // Property name started with a { which means we treat it as a literal.
                    sb.Append(g.Value);
                }

                startIndex = g.Index + g.Length + 1;
            }

            if (startIndex < aFormat.Length)
            {
                sb.Append(aFormat.Substring(startIndex));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts an object to Type using the Convert.ChangeType() call.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="instance">
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T ToType<T>([CanBeNull] this object instance)
        {
            if (instance == null)
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
            else if (instanceType.IsClass && !(instance is IConvertible))
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
        /// <param name="instance">
        /// The instance. 
        /// </param>
        /// <param name="defaultValue">
        /// The default value. 
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T ToTypeOrDefault<T>([CanBeNull] this object instance, T defaultValue)
        {
            try
            {
                return ToType<T>(instance);
            }
            catch (ArgumentNullException)
            {
            }
            catch (FormatException)
            {
            }
            catch (InvalidCastException)
            {
            }
            catch (OverflowException)
            {
            }

            return defaultValue;
        }

        /// <summary>
        /// The verify bool.
        /// </summary>
        /// <param name="o">
        /// The o. 
        /// </param>
        /// <returns>
        /// The verify bool. 
        /// </returns>
        public static bool VerifyBool([NotNull] object o)
        {
            return Convert.ToBoolean(o);
        }

        /// <summary>
        /// The verify int 32.
        /// </summary>
        /// <param name="o">
        /// The o. 
        /// </param>
        /// <returns>
        /// The verify int 32. 
        /// </returns>
        public static int VerifyInt32([NotNull] object o)
        {
            return Convert.ToInt32(o);
        }

        #endregion
    }
}
