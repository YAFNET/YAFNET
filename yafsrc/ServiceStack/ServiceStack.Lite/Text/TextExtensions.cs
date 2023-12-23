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
        var itemDelim = CsvConfig.ItemDelimiterString;
        return string.IsNullOrEmpty(text) || !CsvWriter.HasAnyEscapeChars(text)
                   ? text
                   : string.Concat(
                       itemDelim,
                       text.Replace(itemDelim, CsvConfig.EscapedItemDelimiterString),
                       itemDelim);
    }

    /// <summary>
    /// Converts to csvfield.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>System.Object.</returns>
    public static object ToCsvField(this object text)
    {
        var textSerialized = text is string
                                 ? text.ToString()
                                 : TypeSerializer.SerializeToString(text).StripQuotes();

        if (textSerialized.IsNullOrEmpty() || !CsvWriter.HasAnyEscapeChars(textSerialized))
            return textSerialized;

        var itemDelim = CsvConfig.ItemDelimiterString;
        return string.Concat(
            itemDelim,
            textSerialized.Replace(itemDelim, CsvConfig.EscapedItemDelimiterString),
            itemDelim);
    }

    /// <summary>
    /// Froms the CSV field.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>System.String.</returns>
    public static string FromCsvField(this string text)
    {
        var itemDelim = CsvConfig.ItemDelimiterString;
        if (string.IsNullOrEmpty(text) || !text.StartsWith(itemDelim, StringComparison.Ordinal))
            return text;
        var escapedDelim = CsvConfig.EscapedItemDelimiterString;
        return text.Substring(itemDelim.Length, text.Length - escapedDelim.Length)
            .Replace(escapedDelim, itemDelim);
    }
}