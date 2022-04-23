// ***********************************************************************
// <copyright file="TextExtensions.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.Text;

using System;

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
}