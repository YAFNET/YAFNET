// ***********************************************************************
// <copyright file="ScriptConstants.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
namespace ServiceStack.Script
{
    /// <summary>
    /// Class ScriptConstants.
    /// </summary>
    public static class ScriptConstants
    {
        /// <summary>
        /// The default culture
        /// </summary>
        public const string DefaultCulture = nameof(ScriptConfig.DefaultCulture);
        /// <summary>
        /// The default date format
        /// </summary>
        public const string DefaultDateFormat = nameof(ScriptConfig.DefaultDateFormat);
        /// <summary>
        /// The default date time format
        /// </summary>
        public const string DefaultDateTimeFormat = nameof(ScriptConfig.DefaultDateTimeFormat);
        /// <summary>
        /// The default time format
        /// </summary>
        public const string DefaultTimeFormat = nameof(ScriptConfig.DefaultTimeFormat);
        /// <summary>
        /// The default file cache expiry
        /// </summary>
        public const string DefaultFileCacheExpiry = nameof(ScriptConfig.DefaultFileCacheExpiry);
        /// <summary>
        /// The default URL cache expiry
        /// </summary>
        public const string DefaultUrlCacheExpiry = nameof(ScriptConfig.DefaultUrlCacheExpiry);
        /// <summary>
        /// The default indent
        /// </summary>
        public const string DefaultIndent = nameof(ScriptConfig.DefaultIndent);
        /// <summary>
        /// The default new line
        /// </summary>
        public const string DefaultNewLine = nameof(ScriptConfig.DefaultNewLine);
        /// <summary>
        /// The default js configuration
        /// </summary>
        public const string DefaultJsConfig = nameof(ScriptConfig.DefaultJsConfig);
        /// <summary>
        /// The default string comparison
        /// </summary>
        public const string DefaultStringComparison = nameof(ScriptConfig.DefaultStringComparison);
        /// <summary>
        /// The default table class name
        /// </summary>
        public const string DefaultTableClassName = nameof(ScriptConfig.DefaultTableClassName);
        /// <summary>
        /// The default error class name
        /// </summary>
        public const string DefaultErrorClassName = nameof(ScriptConfig.DefaultErrorClassName);

        /// <summary>
        /// The debug
        /// </summary>
        public const string Debug = "debug";
        /// <summary>
        /// The assign error
        /// </summary>
        public const string AssignError = "assignError";
        /// <summary>
        /// The catch error
        /// </summary>
        public const string CatchError = "catchError";     //assigns error and continues
        /// <summary>
        /// If error return
        /// </summary>
        public const string IfErrorReturn = "ifErrorReturn"; //returns error and continues
        /// <summary>
        /// The HTML encode
        /// </summary>
        public const string HtmlEncode = "htmlencode";
        /// <summary>
        /// The model
        /// </summary>
        public const string Model = "model";
        /// <summary>
        /// The page
        /// </summary>
        public const string Page = "page";
        /// <summary>
        /// The partial
        /// </summary>
        public const string Partial = "partial";
        /// <summary>
        /// The temporary file path
        /// </summary>
        public const string TempFilePath = "/dev/null";
        /// <summary>
        /// The index
        /// </summary>
        public const string Index = "index";
        /// <summary>
        /// The comparer
        /// </summary>
        public const string Comparer = "comparer";
        /// <summary>
        /// The map
        /// </summary>
        public const string Map = "map";
        /// <summary>
        /// The request
        /// </summary>
        public const string Request = "Request";
        /// <summary>
        /// The path information
        /// </summary>
        public const string PathInfo = "PathInfo";
        /// <summary>
        /// The path base
        /// </summary>
        public const string PathBase = "PathBase";
        /// <summary>
        /// The path arguments
        /// </summary>
        public const string PathArgs = "PathArgs";
        /// <summary>
        /// The dto
        /// </summary>
        public const string Dto = "dto";
        /// <summary>
        /// It
        /// </summary>
        public const string It = "it";
        /// <summary>
        /// The field
        /// </summary>
        public const string Field = "field";
        /// <summary>
        /// The assets base
        /// </summary>
        public const string AssetsBase = "assetsBase";
        /// <summary>
        /// The format
        /// </summary>
        public const string Format = "format";
        /// <summary>
        /// The base URL
        /// </summary>
        public const string BaseUrl = "BaseUrl";
        /// <summary>
        /// The partial argument
        /// </summary>
        public const string PartialArg = "__partial";
        /// <summary>
        /// The global
        /// </summary>
        public const string Global = "global";
        /// <summary>
        /// The return
        /// </summary>
        public const string Return = "return";
        /// <summary>
        /// The error code
        /// </summary>
        public const string ErrorCode = nameof(ErrorCode);
        /// <summary>
        /// The error message
        /// </summary>
        public const string ErrorMessage = nameof(ErrorMessage);

        /// <summary>
        /// Gets the empty raw string.
        /// </summary>
        /// <value>The empty raw string.</value>
        public static IRawString EmptyRawString { get; } = new RawString("");
        /// <summary>
        /// Gets the true raw string.
        /// </summary>
        /// <value>The true raw string.</value>
        public static IRawString TrueRawString { get; } = new RawString("true");
        /// <summary>
        /// Gets the false raw string.
        /// </summary>
        /// <value>The false raw string.</value>
        public static IRawString FalseRawString { get; } = new RawString("false");
    }
}