// ***********************************************************************
// <copyright file="CsvConfig.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Globalization;

using ServiceStack.OrmLite.Base.Text.Common;

namespace ServiceStack.OrmLite.Base.Text;

/// <summary>
/// Class CsvConfig.
/// </summary>
public static class CsvConfig
{
    /// <summary>
    /// Initializes static members of the <see cref="CsvConfig" /> class.
    /// </summary>
    static CsvConfig()
    {
        Reset();
    }

    /// <summary>
    /// Gets or sets the real number culture information.
    /// </summary>
    /// <value>The real number culture information.</value>
    public static CultureInfo RealNumberCultureInfo {
        get => field ?? CultureInfo.InvariantCulture;
        set;
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
            sItemSeperatorString ??= value;
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
            sItemDelimiterString ??= value;
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
    static internal string EscapedItemDelimiterString
    {
        get => tsEscapedItemDelimiterString ?? sEscapedItemDelimiterString ?? DefaultEscapedItemDelimiterString;
        set
        {
            tsEscapedItemDelimiterString = value;
            sEscapedItemDelimiterString ??= value;
        }
    }

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
        get => tsEscapeStrings ?? sEscapeStrings ?? field;
        private set
        {
            tsEscapeStrings = value;
            sEscapeStrings ??= value;
        }
    } = GetEscapeStrings();

    /// <summary>
    /// Gets the escape strings.
    /// </summary>
    /// <returns>System.String[].</returns>
    private static string[] GetEscapeStrings()
    {
        return [ItemDelimiterString, ItemSeperatorString, RowSeparatorString, "\r", "\n"];
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
            sRowSeparatorString ??= value;
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
}