// ***********************************************************************
// <copyright file="TextExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using ServiceStack.Text;

namespace ServiceStack
{
    /// <summary>
    /// Class TextExtensions.
    /// </summary>
    public static class TextExtensions
    {
        /// <summary>
        /// Converts to csvfield.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public static string ToCsvField(this string text)
        {
            return string.IsNullOrEmpty(text) || !CsvWriter.HasAnyEscapeChars(text)
                ? text
                : string.Concat
                        (
                            CsvConfig.ItemDelimiterString,
                            text.Replace(CsvConfig.ItemDelimiterString, CsvConfig.EscapedItemDelimiterString),
                            CsvConfig.ItemDelimiterString
                        );
        }

        /// <summary>
        /// Converts to csvfield.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.Object.</returns>
        public static object ToCsvField(this object text)
        {
            var textSerialized = string.Empty;
            if (text is string)
            {
                textSerialized = text.ToString();
            }
            else
            {
                textSerialized = TypeSerializer.SerializeToString(text).StripQuotes();
            }

            return textSerialized.IsNullOrEmpty() || !CsvWriter.HasAnyEscapeChars(textSerialized)
                ? textSerialized
                : string.Concat
                  (
                      CsvConfig.ItemDelimiterString,
                      textSerialized.Replace(CsvConfig.ItemDelimiterString, CsvConfig.EscapedItemDelimiterString),
                      CsvConfig.ItemDelimiterString
                  );
        }

        /// <summary>
        /// Froms the CSV field.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public static string FromCsvField(this string text)
        {
            return string.IsNullOrEmpty(text) || !text.StartsWith(CsvConfig.ItemDelimiterString, StringComparison.Ordinal)
                ? text
                : text.Substring(CsvConfig.ItemDelimiterString.Length, text.Length - CsvConfig.EscapedItemDelimiterString.Length)
                    .Replace(CsvConfig.EscapedItemDelimiterString, CsvConfig.ItemDelimiterString);
        }

        /// <summary>
        /// Froms the CSV fields.
        /// </summary>
        /// <param name="texts">The texts.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static List<string> FromCsvFields(this IEnumerable<string> texts)
        {
            var safeTexts = new List<string>();
            foreach (var text in texts)
            {
                safeTexts.Add(FromCsvField(text));
            }
            return safeTexts;
        }

        /// <summary>
        /// Froms the CSV fields.
        /// </summary>
        /// <param name="texts">The texts.</param>
        /// <returns>System.String[].</returns>
        public static string[] FromCsvFields(params string[] texts)
        {
            var textsLen = texts.Length;
            var safeTexts = new string[textsLen];
            for (var i = 0; i < textsLen; i++)
            {
                safeTexts[i] = FromCsvField(texts[i]);
            }
            return safeTexts;
        }

        /// <summary>
        /// Serializes to string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string SerializeToString<T>(this T value)
        {
            return JsonSerializer.SerializeToString(value);
        }
    }
}