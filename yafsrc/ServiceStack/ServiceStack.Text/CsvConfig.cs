// ***********************************************************************
// <copyright file="CsvConfig.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using ServiceStack.Text.Common;

namespace ServiceStack.Text
{
    /// <summary>
    /// Class CsvConfig.
    /// </summary>
    public static class CsvConfig
    {
        /// <summary>
        /// Initializes static members of the <see cref="CsvConfig"/> class.
        /// </summary>
        static CsvConfig()
        {
            Reset();
        }

        /// <summary>
        /// The s real number culture information
        /// </summary>
        private static CultureInfo sRealNumberCultureInfo;
        /// <summary>
        /// Gets or sets the real number culture information.
        /// </summary>
        /// <value>The real number culture information.</value>
        public static CultureInfo RealNumberCultureInfo
        {
            get => sRealNumberCultureInfo ?? CultureInfo.InvariantCulture;
            set => sRealNumberCultureInfo = value;
        }

        /// <summary>
        /// The ts item seperator string
        /// </summary>
        [ThreadStatic]
        private static string tsItemSeperatorString;
        /// <summary>
        /// The s item seperator string
        /// </summary>
        private static string sItemSeperatorString;
        /// <summary>
        /// Gets or sets the item seperator string.
        /// </summary>
        /// <value>The item seperator string.</value>
        public static string ItemSeperatorString
        {
            get => tsItemSeperatorString ?? sItemSeperatorString ?? JsWriter.ItemSeperatorString;
            set
            {
                tsItemSeperatorString = value;
                if (sItemSeperatorString == null) sItemSeperatorString = value;
                ResetEscapeStrings();
            }
        }

        /// <summary>
        /// The ts item delimiter string
        /// </summary>
        [ThreadStatic]
        private static string tsItemDelimiterString;
        /// <summary>
        /// The s item delimiter string
        /// </summary>
        private static string sItemDelimiterString;
        /// <summary>
        /// Gets or sets the item delimiter string.
        /// </summary>
        /// <value>The item delimiter string.</value>
        public static string ItemDelimiterString
        {
            get => tsItemDelimiterString ?? sItemDelimiterString ?? JsWriter.QuoteString;
            set
            {
                tsItemDelimiterString = value;
                if (sItemDelimiterString == null) sItemDelimiterString = value;
                EscapedItemDelimiterString = value + value;
                ResetEscapeStrings();
            }
        }

        /// <summary>
        /// The default escaped item delimiter string
        /// </summary>
        private const string DefaultEscapedItemDelimiterString = JsWriter.QuoteString + JsWriter.QuoteString;

        /// <summary>
        /// The ts escaped item delimiter string
        /// </summary>
        [ThreadStatic]
        private static string tsEscapedItemDelimiterString;
        /// <summary>
        /// The s escaped item delimiter string
        /// </summary>
        private static string sEscapedItemDelimiterString;
        /// <summary>
        /// Gets or sets the escaped item delimiter string.
        /// </summary>
        /// <value>The escaped item delimiter string.</value>
        internal static string EscapedItemDelimiterString
        {
            get => tsEscapedItemDelimiterString ?? sEscapedItemDelimiterString ?? DefaultEscapedItemDelimiterString;
            set
            {
                tsEscapedItemDelimiterString = value;
                if (sEscapedItemDelimiterString == null) sEscapedItemDelimiterString = value;
            }
        }

        /// <summary>
        /// The default escape strings
        /// </summary>
        private static readonly string[] defaultEscapeStrings = GetEscapeStrings();

        /// <summary>
        /// The ts escape strings
        /// </summary>
        [ThreadStatic]
        private static string[] tsEscapeStrings;
        /// <summary>
        /// The s escape strings
        /// </summary>
        private static string[] sEscapeStrings;
        /// <summary>
        /// Gets the escape strings.
        /// </summary>
        /// <value>The escape strings.</value>
        public static string[] EscapeStrings
        {
            get => tsEscapeStrings ?? sEscapeStrings ?? defaultEscapeStrings;
            private set
            {
                tsEscapeStrings = value;
                if (sEscapeStrings == null) sEscapeStrings = value;
            }
        }

        /// <summary>
        /// Gets the escape strings.
        /// </summary>
        /// <returns>System.String[].</returns>
        private static string[] GetEscapeStrings()
        {
            return new[] { ItemDelimiterString, ItemSeperatorString, RowSeparatorString, "\r", "\n" };
        }

        /// <summary>
        /// Resets the escape strings.
        /// </summary>
        private static void ResetEscapeStrings()
        {
            EscapeStrings = GetEscapeStrings();
        }

        /// <summary>
        /// The ts row separator string
        /// </summary>
        [ThreadStatic]
        private static string tsRowSeparatorString;
        /// <summary>
        /// The s row separator string
        /// </summary>
        private static string sRowSeparatorString;
        /// <summary>
        /// Gets or sets the row separator string.
        /// </summary>
        /// <value>The row separator string.</value>
        public static string RowSeparatorString
        {
            get => tsRowSeparatorString ?? sRowSeparatorString ?? "\r\n";
            set
            {
                tsRowSeparatorString = value;
                if (sRowSeparatorString == null) sRowSeparatorString = value;
                ResetEscapeStrings();
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public static void Reset()
        {
            tsItemSeperatorString = sItemSeperatorString = null;
            tsItemDelimiterString = sItemDelimiterString = null;
            tsEscapedItemDelimiterString = sEscapedItemDelimiterString = null;
            tsRowSeparatorString = sRowSeparatorString = null;
            tsEscapeStrings = sEscapeStrings = null;
        }

    }

    /// <summary>
    /// Class CsvConfig.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class CsvConfig<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether [omit headers].
        /// </summary>
        /// <value><c>true</c> if [omit headers]; otherwise, <c>false</c>.</value>
        public static bool OmitHeaders { get; set; }

        /// <summary>
        /// The custom headers map
        /// </summary>
        private static Dictionary<string, string> customHeadersMap;
        /// <summary>
        /// Gets or sets the custom headers map.
        /// </summary>
        /// <value>The custom headers map.</value>
        public static Dictionary<string, string> CustomHeadersMap
        {
            get => customHeadersMap;
            set
            {
                customHeadersMap = value;
                if (value == null) return;
                CsvWriter<T>.ConfigureCustomHeaders(customHeadersMap);
                CsvReader<T>.ConfigureCustomHeaders(customHeadersMap);
            }
        }

        /// <summary>
        /// Sets the custom headers.
        /// </summary>
        /// <value>The custom headers.</value>
        /// <exception cref="System.ArgumentException">CustomHeaders is a ValueType</exception>
        public static object CustomHeaders
        {
            set
            {
                if (value == null) return;
                if (value.GetType().IsValueType)
                    throw new ArgumentException("CustomHeaders is a ValueType");

                var propertyInfos = value.GetType().GetProperties();
                if (propertyInfos.Length == 0) return;

                customHeadersMap = new Dictionary<string, string>();
                foreach (var pi in propertyInfos)
                {
                    var getMethod = pi.GetGetMethod(nonPublic: true);
                    if (getMethod == null) continue;

                    var oValue = getMethod.Invoke(value, TypeConstants.EmptyObjectArray);
                    if (oValue == null) continue;
                    customHeadersMap[pi.Name] = oValue.ToString();
                }
                CsvWriter<T>.ConfigureCustomHeaders(customHeadersMap);
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public static void Reset()
        {
            OmitHeaders = false;
            CsvWriter<T>.Reset();
        }
    }
}