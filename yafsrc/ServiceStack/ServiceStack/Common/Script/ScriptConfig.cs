// ***********************************************************************
// <copyright file="ScriptConfig.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;

using ServiceStack.Text;

namespace ServiceStack.Script;

/// <summary>
/// Class ScriptConfig.
/// </summary>
public static class ScriptConfig
{
    /// <summary>
    /// Rethrow fatal exceptions thrown on incorrect API usage
    /// </summary>
    /// <value>The fatal exceptions.</value>
    public static HashSet<Type> FatalExceptions { get; set; } = new()
                                                                    {
                                                                        typeof(NotSupportedException),
                                                                        typeof(NotImplementedException),
                                                                        typeof(StackOverflowException),
                                                                    };

    /// <summary>
    /// Gets or sets the capture and evaluate exceptions to null.
    /// </summary>
    /// <value>The capture and evaluate exceptions to null.</value>
    public static HashSet<Type> CaptureAndEvaluateExceptionsToNull { get; set; } = new()
        {
            typeof(NullReferenceException),
            typeof(ArgumentNullException),
        };
    /// <summary>
    /// Gets or sets the default culture.
    /// </summary>
    /// <value>The default culture.</value>
    public static CultureInfo DefaultCulture { get; set; } //Uses CurrentCulture by default
    /// <summary>
    /// Gets or sets the default date format.
    /// </summary>
    /// <value>The default date format.</value>
    public static string DefaultDateFormat { get; set; } = "yyyy-MM-dd";
    /// <summary>
    /// Gets or sets the default date time format.
    /// </summary>
    /// <value>The default date time format.</value>
    public static string DefaultDateTimeFormat { get; set; } = "u";
    /// <summary>
    /// Gets or sets the default time format.
    /// </summary>
    /// <value>The default time format.</value>
    public static string DefaultTimeFormat { get; set; } = @"h\:mm\:ss";
    /// <summary>
    /// Gets or sets the default file cache expiry.
    /// </summary>
    /// <value>The default file cache expiry.</value>
    public static TimeSpan DefaultFileCacheExpiry { get; set; } = TimeSpan.FromMinutes(1);
    /// <summary>
    /// Gets or sets the default URL cache expiry.
    /// </summary>
    /// <value>The default URL cache expiry.</value>
    public static TimeSpan DefaultUrlCacheExpiry { get; set; } = TimeSpan.FromMinutes(1);
    /// <summary>
    /// Gets or sets the default indent.
    /// </summary>
    /// <value>The default indent.</value>
    public static string DefaultIndent { get; set; } = "\t";
    /// <summary>
    /// Gets or sets the default new line.
    /// </summary>
    /// <value>The default new line.</value>
    public static string DefaultNewLine { get; set; } = Environment.NewLine;
    /// <summary>
    /// Gets or sets the default js configuration.
    /// </summary>
    /// <value>The default js configuration.</value>
    public static string DefaultJsConfig { get; set; } = "excludetypeinfo";
    /// <summary>
    /// Gets or sets the default string comparison.
    /// </summary>
    /// <value>The default string comparison.</value>
    public static StringComparison DefaultStringComparison { get; set; } = StringComparison.Ordinal;
    /// <summary>
    /// Gets or sets the default name of the table class.
    /// </summary>
    /// <value>The default name of the table class.</value>
    public static string DefaultTableClassName { get; set; } = "table";
    /// <summary>
    /// Gets or sets the default name of the error class.
    /// </summary>
    /// <value>The default name of the error class.</value>
    public static string DefaultErrorClassName { get; set; } = "alert alert-danger";
    /// <summary>
    /// Gets or sets a value indicating whether [allow unix pipe syntax].
    /// </summary>
    /// <value><c>true</c> if [allow unix pipe syntax]; otherwise, <c>false</c>.</value>
    public static bool AllowUnixPipeSyntax { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether [allow assignment expressions].
    /// </summary>
    /// <value><c>true</c> if [allow assignment expressions]; otherwise, <c>false</c>.</value>
    public static bool AllowAssignmentExpressions { get; set; } = true;
    /// <summary>
    /// The parse real number
    /// </summary>
    public static ParseRealNumber ParseRealNumber = numLiteral => numLiteral.ParseDouble();

    /// <summary>
    /// Creates the culture.
    /// </summary>
    /// <returns>CultureInfo.</returns>
    public static CultureInfo CreateCulture()
    {
        var culture = DefaultCulture ?? CultureInfo.CurrentCulture;
        if (!Equals(culture, CultureInfo.InvariantCulture)) return culture;
        culture = (CultureInfo)culture.Clone();
        culture.NumberFormat.CurrencySymbol = "$";
        return culture;
    }
}

/// <summary>
/// Delegate ParseRealNumber
/// </summary>
/// <param name="numLiteral">The number literal.</param>
/// <returns>System.Object.</returns>
public delegate object ParseRealNumber(ReadOnlySpan<char> numLiteral);