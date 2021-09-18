// ***********************************************************************
// <copyright file="TemplateContext.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.IO;
using ServiceStack.Script;
using ServiceStack.Text;


namespace ServiceStack.Script
{
    /// <summary>
    /// Class SharpPages.
    /// Implements the <see cref="ServiceStack.Script.ISharpPages" />
    /// Implements the <see cref="ServiceStack.Templates.ITemplatePages" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ISharpPages" />
    /// <seealso cref="ServiceStack.Templates.ITemplatePages" />
    public partial class SharpPages : ISharpPages, Templates.ITemplatePages { }

    /// <summary>
    /// Class ScriptContext.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public partial class ScriptContext
    {
        /// <summary>
        /// Gets the default filters.
        /// </summary>
        /// <value>The default filters.</value>
        [Obsolete("Use DefaultScripts")]
        public DefaultScripts DefaultFilters => DefaultMethods;

        /// <summary>
        /// Gets the protected filters.
        /// </summary>
        /// <value>The protected filters.</value>
        [Obsolete("Use ProtectedScripts")]
        public ProtectedScripts ProtectedFilters => ProtectedMethods;
    }
}

namespace ServiceStack.Templates
{
    /// <summary>
    /// Class TemplateDefaultFilters.
    /// Implements the <see cref="ServiceStack.Script.DefaultScripts" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.DefaultScripts" />
    [Obsolete("Use DefaultScripts")]
    public class TemplateDefaultFilters : Script.DefaultScripts
    {
    }

    /// <summary>
    /// Class TemplateHtmlFilters.
    /// Implements the <see cref="ServiceStack.Script.HtmlScripts" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.HtmlScripts" />
    [Obsolete("Use HtmlScripts")]
    public class TemplateHtmlFilters : Script.HtmlScripts
    {
    }

    /// <summary>
    /// Class TemplateProtectedFilters.
    /// Implements the <see cref="ServiceStack.Script.ProtectedScripts" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ProtectedScripts" />
    [Obsolete("Use ProtectedScripts")]
    public class TemplateProtectedFilters : Script.ProtectedScripts
    {
    }

    /// <summary>
    /// Class TemplateBlock.
    /// Implements the <see cref="ServiceStack.Script.ScriptBlock" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptBlock" />
    [Obsolete("Use ScriptBlock")]
    public abstract class TemplateBlock : ServiceStack.Script.ScriptBlock
    {
        /// <summary>
        /// Writes the asynchronous.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="block">The block.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public override Task WriteAsync(ScriptScopeContext scope, PageBlockFragment block, CancellationToken token) =>
            WriteAsync((TemplateScopeContext)scope, block, token);

        /// <summary>
        /// Writes the asynchronous.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="block">The block.</param>
        /// <param name="ct">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public abstract Task WriteAsync(TemplateScopeContext scope, PageBlockFragment block, CancellationToken ct);
    }

    /// <summary>
    /// Class TemplateContext.
    /// Implements the <see cref="ServiceStack.Script.ScriptContext" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ScriptContext" />
    [Obsolete("Use ScriptContext")]
    public class TemplateContext : Script.ScriptContext
    {

    }

    /// <summary>
    /// Struct TemplateScopeContext
    /// </summary>
    [Obsolete("Use ScriptScopeContext")]
    public struct TemplateScopeContext
    {
        /// <summary>
        /// Gets the page result.
        /// </summary>
        /// <value>The page result.</value>
        public Script.PageResult PageResult { get; }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <value>The page.</value>
        public Script.SharpPage Page => PageResult.Page;

        /// <summary>
        /// Gets the code page.
        /// </summary>
        /// <value>The code page.</value>
        public Script.SharpCodePage CodePage => PageResult.CodePage;

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public Script.ScriptContext Context => PageResult.Context;

        /// <summary>
        /// Gets the scoped parameters.
        /// </summary>
        /// <value>The scoped parameters.</value>
        public Dictionary<string, object> ScopedParams { get; internal set; }

        /// <summary>
        /// Gets the output stream.
        /// </summary>
        /// <value>The output stream.</value>
        public Stream OutputStream { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateScopeContext"/> struct.
        /// </summary>
        /// <param name="pageResult">The page result.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="scopedParams">The scoped parameters.</param>
        public TemplateScopeContext(
            Script.PageResult pageResult,
            Stream outputStream,
            Dictionary<string, object> scopedParams)
        {
            PageResult = pageResult;
            ScopedParams = scopedParams ?? new Dictionary<string, object>();
            OutputStream = outputStream;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="TemplateScopeContext"/> to <see cref="Script.ScriptScopeContext"/>.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Script.ScriptScopeContext(TemplateScopeContext from)
        {
            return new Script.ScriptScopeContext(from.PageResult, from.OutputStream, from.ScopedParams);
        }
    }

    /// <summary>
    /// Interface ITemplatePages
    /// Implements the <see cref="ServiceStack.Script.ISharpPages" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.ISharpPages" />
    [Obsolete("Use ISharpPages")]
    public interface ITemplatePages : ISharpPages
    {
    }

    /// <summary>
    /// Interface ITemplatePlugin
    /// Implements the <see cref="ServiceStack.Script.IScriptPlugin" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.IScriptPlugin" />
    [Obsolete("Use IScriptPlugin")]
    public interface ITemplatePlugin : IScriptPlugin
    {
    }

    /// <summary>
    /// Class TemplatePage.
    /// Implements the <see cref="ServiceStack.Script.SharpPage" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.SharpPage" />
    [Obsolete("Use SharpPage")]
    public class TemplatePage : Script.SharpPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatePage"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="file">The file.</param>
        /// <param name="format">The format.</param>
        public TemplatePage(TemplateContext context, IVirtualFile file, Script.PageFormat format = null)
            : base(context, file, format)
        {
        }
    }

    /// <summary>
    /// Class TemplateCodePage.
    /// Implements the <see cref="ServiceStack.Script.SharpCodePage" />
    /// </summary>
    /// <seealso cref="ServiceStack.Script.SharpCodePage" />
    [Obsolete("Use SharpCodePage")]
    public class TemplateCodePage : Script.SharpCodePage
    {
    }

    /// <summary>
    /// Class TemplateContextExtensions.
    /// </summary>
    public static class TemplateContextExtensions
    {
        /// <summary>
        /// Evaluates the template.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
        public static string EvaluateTemplate(
            this TemplateContext context,
            string script,
            Dictionary<string, object> args = null)
        {
            var result = context.EvaluateScript(script, args, out var ex);
            if (ex?.InnerException is NotSupportedException)
                throw ex.InnerException;
            return result;
        }

        /// <summary>
        /// Evaluate template as an asynchronous operation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
        public static async Task<string> EvaluateTemplateAsync(
            this TemplateContext context,
            string script,
            Dictionary<string, object> args = null)
        {
            try
            {
                var result = await context.EvaluateScriptAsync(script, args).ConfigAwait();
                return result;
            }
            catch (ScriptException ex)
            {
                if (ex.InnerException is NotSupportedException)
                    throw ex.InnerException;
                throw;
            }
        }
    }

    /// <summary>
    /// Class TemplateConstants.
    /// </summary>
    [Obsolete("Use ScriptConstants")]
    public static class TemplateConstants
    {
        /// <summary>
        /// The default culture
        /// </summary>
        public const string DefaultCulture = ScriptConstants.DefaultCulture;

        /// <summary>
        /// The default date format
        /// </summary>
        public const string DefaultDateFormat = ScriptConstants.DefaultDateFormat;

        /// <summary>
        /// The default date time format
        /// </summary>
        public const string DefaultDateTimeFormat = ScriptConstants.DefaultDateTimeFormat;

        /// <summary>
        /// The default time format
        /// </summary>
        public const string DefaultTimeFormat = ScriptConstants.DefaultTimeFormat;

        /// <summary>
        /// The default file cache expiry
        /// </summary>
        public const string DefaultFileCacheExpiry = ScriptConstants.DefaultFileCacheExpiry;

        /// <summary>
        /// The default URL cache expiry
        /// </summary>
        public const string DefaultUrlCacheExpiry = ScriptConstants.DefaultUrlCacheExpiry;

        /// <summary>
        /// The default indent
        /// </summary>
        public const string DefaultIndent = ScriptConstants.DefaultIndent;

        /// <summary>
        /// The default new line
        /// </summary>
        public const string DefaultNewLine = ScriptConstants.DefaultNewLine;

        /// <summary>
        /// The default js configuration
        /// </summary>
        public const string DefaultJsConfig = ScriptConstants.DefaultJsConfig;

        /// <summary>
        /// The default string comparison
        /// </summary>
        public const string DefaultStringComparison = ScriptConstants.DefaultStringComparison;

        /// <summary>
        /// The default table class name
        /// </summary>
        public const string DefaultTableClassName = ScriptConstants.DefaultTableClassName;

        /// <summary>
        /// The default error class name
        /// </summary>
        public const string DefaultErrorClassName = ScriptConstants.DefaultErrorClassName;

        /// <summary>
        /// The debug
        /// </summary>
        public const string Debug = ScriptConstants.Debug;

        /// <summary>
        /// The assign error
        /// </summary>
        public const string AssignError = ScriptConstants.AssignError;

        /// <summary>
        /// The catch error
        /// </summary>
        public const string CatchError = ScriptConstants.CatchError; //assigns error and continues

        /// <summary>
        /// The HTML encode
        /// </summary>
        public const string HtmlEncode = ScriptConstants.HtmlEncode;

        /// <summary>
        /// The model
        /// </summary>
        public const string Model = ScriptConstants.Model;

        /// <summary>
        /// The page
        /// </summary>
        public const string Page = ScriptConstants.Page;

        /// <summary>
        /// The partial
        /// </summary>
        public const string Partial = ScriptConstants.Partial;

        /// <summary>
        /// The temporary file path
        /// </summary>
        public const string TempFilePath = ScriptConstants.TempFilePath;

        /// <summary>
        /// The index
        /// </summary>
        public const string Index = ScriptConstants.Index;

        /// <summary>
        /// The comparer
        /// </summary>
        public const string Comparer = ScriptConstants.Comparer;

        /// <summary>
        /// The map
        /// </summary>
        public const string Map = ScriptConstants.Map;

        /// <summary>
        /// The request
        /// </summary>
        public const string Request = ScriptConstants.Request;

        /// <summary>
        /// The path information
        /// </summary>
        public const string PathInfo = ScriptConstants.PathInfo;

        /// <summary>
        /// The path arguments
        /// </summary>
        public const string PathArgs = ScriptConstants.PathArgs;

        /// <summary>
        /// The assets base
        /// </summary>
        public const string AssetsBase = ScriptConstants.AssetsBase;

        /// <summary>
        /// The format
        /// </summary>
        public const string Format = ScriptConstants.Format;

        /// <summary>
        /// Gets the empty raw string.
        /// </summary>
        /// <value>The empty raw string.</value>
        public static IRawString EmptyRawString { get; } = ScriptConstants.EmptyRawString;

        /// <summary>
        /// Gets the true raw string.
        /// </summary>
        /// <value>The true raw string.</value>
        public static IRawString TrueRawString { get; } = ScriptConstants.TrueRawString;

        /// <summary>
        /// Gets the false raw string.
        /// </summary>
        /// <value>The false raw string.</value>
        public static IRawString FalseRawString { get; } = ScriptConstants.FalseRawString;
    }

    /// <summary>
    /// Class TemplateConfig.
    /// </summary>
    [Obsolete("Use ScriptConfig")]
    public static class TemplateConfig
    {
        /// <summary>
        /// Gets the fatal exceptions.
        /// </summary>
        /// <value>The fatal exceptions.</value>
        public static HashSet<Type> FatalExceptions => ScriptConfig.FatalExceptions;

        /// <summary>
        /// Gets the capture and evaluate exceptions to null.
        /// </summary>
        /// <value>The capture and evaluate exceptions to null.</value>
        public static HashSet<Type> CaptureAndEvaluateExceptionsToNull =>
            ScriptConfig.CaptureAndEvaluateExceptionsToNull;

        /// <summary>
        /// Gets or sets the maximum quota.
        /// </summary>
        /// <value>The maximum quota.</value>
        public static int MaxQuota { get; set; } = 10000;

        /// <summary>
        /// Gets the default culture.
        /// </summary>
        /// <value>The default culture.</value>
        public static CultureInfo DefaultCulture => ScriptConfig.DefaultCulture;

        /// <summary>
        /// Gets the default date format.
        /// </summary>
        /// <value>The default date format.</value>
        public static string DefaultDateFormat => ScriptConfig.DefaultDateFormat;

        /// <summary>
        /// Gets the default date time format.
        /// </summary>
        /// <value>The default date time format.</value>
        public static string DefaultDateTimeFormat => ScriptConfig.DefaultDateTimeFormat;

        /// <summary>
        /// Gets the default time format.
        /// </summary>
        /// <value>The default time format.</value>
        public static string DefaultTimeFormat => ScriptConfig.DefaultTimeFormat;

        /// <summary>
        /// Gets the default file cache expiry.
        /// </summary>
        /// <value>The default file cache expiry.</value>
        public static TimeSpan DefaultFileCacheExpiry => ScriptConfig.DefaultFileCacheExpiry;

        /// <summary>
        /// Gets the default URL cache expiry.
        /// </summary>
        /// <value>The default URL cache expiry.</value>
        public static TimeSpan DefaultUrlCacheExpiry => ScriptConfig.DefaultUrlCacheExpiry;

        /// <summary>
        /// Gets the default indent.
        /// </summary>
        /// <value>The default indent.</value>
        public static string DefaultIndent => ScriptConfig.DefaultIndent;

        /// <summary>
        /// Gets the default new line.
        /// </summary>
        /// <value>The default new line.</value>
        public static string DefaultNewLine => ScriptConfig.DefaultNewLine;

        /// <summary>
        /// Gets the default js configuration.
        /// </summary>
        /// <value>The default js configuration.</value>
        public static string DefaultJsConfig => ScriptConfig.DefaultJsConfig;

        /// <summary>
        /// Gets the default string comparison.
        /// </summary>
        /// <value>The default string comparison.</value>
        public static StringComparison DefaultStringComparison => ScriptConfig.DefaultStringComparison;

        /// <summary>
        /// Gets the default name of the table class.
        /// </summary>
        /// <value>The default name of the table class.</value>
        public static string DefaultTableClassName => ScriptConfig.DefaultTableClassName;

        /// <summary>
        /// Gets the default name of the error class.
        /// </summary>
        /// <value>The default name of the error class.</value>
        public static string DefaultErrorClassName => ScriptConfig.DefaultErrorClassName;

        /// <summary>
        /// Creates the culture.
        /// </summary>
        /// <returns>CultureInfo.</returns>
        public static CultureInfo CreateCulture() => ScriptConfig.CreateCulture();
    }
}