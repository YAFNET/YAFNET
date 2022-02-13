// ***********************************************************************
// <copyright file="EnumExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceStack
{
    using ServiceStack.Text;

    /// <summary>
    /// Class EnumUtils.
    /// </summary>
    public static class EnumUtils
    {
        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public static IEnumerable<T> GetValues<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>();
    }

    /// <summary>
    /// Class EnumExtensions.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the textual description of the enum if it has one. e.g.
        /// <code>
        /// enum UserColors
        /// {
        /// [Description("Bright Red")]
        /// BrightRed
        /// }
        /// UserColors.BrightRed.ToDescription();
        /// </code>
        /// </summary>
        /// <param name="enum">The enum.</param>
        /// <returns>System.String.</returns>
        public static string ToDescription(this Enum @enum)
        {
            var type = @enum.GetType();

            var memInfo = type.GetMember(@enum.ToString());

            if (memInfo.Length > 0)
            {
                var description = memInfo[0].GetDescription();

                if (description != null)
                    return description;
            }

            return @enum.ToString();
        }

        /// <summary>
        /// Converts to keyvaluepairs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enums">The enums.</param>
        /// <returns>List&lt;KeyValuePair&lt;System.String, System.String&gt;&gt;.</returns>
        public static List<KeyValuePair<string, string>> ToKeyValuePairs<T>(this IEnumerable<T> enums) where T : Enum
            => enums.Map(x => new KeyValuePair<string, string>(
                x.ToString(),
                x.ToDescription()));

        /// <summary>
        /// Converts to list.
        /// </summary>
        /// <param name="enum">The enum.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static List<string> ToList(this Enum @enum)
        {
#if !(SL54 || WP)
            return new List<string>(Enum.GetNames(@enum.GetType()));
#else
            return @enum.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
#endif
        }

        /// <summary>
        /// Gets the type code.
        /// </summary>
        /// <param name="enum">The enum.</param>
        /// <returns>TypeCode.</returns>
        public static TypeCode GetTypeCode(this Enum @enum)
        {
            return Enum.GetUnderlyingType(@enum.GetType()).GetTypeCode();
        }

        /// <summary>
        /// Determines whether [has] [the specified value].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum">The enum.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if [has] [the specified value]; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.NotSupportedException">Enums of type {@enum.GetType().Name}</exception>
        public static bool Has<T>(this Enum @enum, T value)
        {
            var typeCode = @enum.GetTypeCode();
            return typeCode switch
                {
                    TypeCode.Byte => ((byte) (object) @enum & (byte) (object) value) == (byte) (object) value,
                    TypeCode.Int16 => ((short) (object) @enum & (short) (object) value) == (short) (object) value,
                    TypeCode.Int32 => ((int) (object) @enum & (int) (object) value) == (int) (object) value,
                    TypeCode.Int64 => ((long) (object) @enum & (long) (object) value) == (long) (object) value,
                    _ => throw new NotSupportedException($"Enums of type {@enum.GetType().Name}")
                };
        }

        /// <summary>
        /// Determines whether [is] [the specified value].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum">The enum.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if [is] [the specified value]; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.NotSupportedException">Enums of type {@enum.GetType().Name}</exception>
        public static bool Is<T>(this Enum @enum, T value)
        {
            var typeCode = @enum.GetTypeCode();
            return typeCode switch
                {
                    TypeCode.Byte => (byte) (object) @enum == (byte) (object) value,
                    TypeCode.Int16 => (short) (object) @enum == (short) (object) value,
                    TypeCode.Int32 => (int) (object) @enum == (int) (object) value,
                    TypeCode.Int64 => (long) (object) @enum == (long) (object) value,
                    _ => throw new NotSupportedException($"Enums of type {@enum.GetType().Name}")
                };
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum">The enum.</param>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.NotSupportedException">Enums of type {@enum.GetType().Name}</exception>
        public static T Add<T>(this Enum @enum, T value)
        {
            var typeCode = @enum.GetTypeCode();
            return typeCode switch
                {
                    TypeCode.Byte => (T) (object) ((byte) (object) @enum | (byte) (object) value),
                    TypeCode.Int16 => (T) (object) ((short) (object) @enum | (short) (object) value),
                    TypeCode.Int32 => (T) (object) ((int) (object) @enum | (int) (object) value),
                    TypeCode.Int64 => (T) (object) ((long) (object) @enum | (long) (object) value),
                    _ => throw new NotSupportedException($"Enums of type {@enum.GetType().Name}")
                };
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum">The enum.</param>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.NotSupportedException">Enums of type {@enum.GetType().Name}</exception>
        public static T Remove<T>(this Enum @enum, T value)
        {
            var typeCode = @enum.GetTypeCode();
            return typeCode switch
                {
                    TypeCode.Byte => (T) (object) ((byte) (object) @enum & ~(byte) (object) value),
                    TypeCode.Int16 => (T) (object) ((short) (object) @enum & ~(short) (object) value),
                    TypeCode.Int32 => (T) (object) ((int) (object) @enum & ~(int) (object) value),
                    TypeCode.Int64 => (T) (object) ((long) (object) @enum & ~(long) (object) value),
                    _ => throw new NotSupportedException($"Enums of type {@enum.GetType().Name}")
                };
        }

    }

}