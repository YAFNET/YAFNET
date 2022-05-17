// ***********************************************************************
// <copyright file="PageFormat.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.IO;
using System.Threading.Tasks;
using ServiceStack.Text;
using ServiceStack.Web;

namespace ServiceStack.Script;

/// <summary>
/// Class PageFormat.
/// </summary>
public class PageFormat
{
    /// <summary>
    /// Gets or sets the arguments prefix.
    /// </summary>
    /// <value>The arguments prefix.</value>
    public string ArgsPrefix { get; set; } = "---";

    /// <summary>
    /// Gets or sets the arguments suffix.
    /// </summary>
    /// <value>The arguments suffix.</value>
    public string ArgsSuffix { get; set; } = "---";

    /// <summary>
    /// Gets or sets the extension.
    /// </summary>
    /// <value>The extension.</value>
    public string Extension { get; set; }

    /// <summary>
    /// Gets or sets the type of the content.
    /// </summary>
    /// <value>The type of the content.</value>
    public string ContentType { get; set; } = MimeTypes.PlainText;

    /// <summary>
    /// Gets or sets the encode value.
    /// </summary>
    /// <value>The encode value.</value>
    public Func<object, string> EncodeValue { get; set; }

    /// <summary>
    /// Gets or sets the resolve layout.
    /// </summary>
    /// <value>The resolve layout.</value>
    public Func<SharpPage, SharpPage> ResolveLayout { get; set; }

    /// <summary>
    /// Gets or sets the on expression exception.
    /// </summary>
    /// <value>The on expression exception.</value>
    public Func<PageResult, Exception, object> OnExpressionException { get; set; }

    /// <summary>
    /// Gets or sets the on view exception.
    /// </summary>
    /// <value>The on view exception.</value>
    public Func<PageResult, IRequest, Exception, Task> OnViewException { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageFormat"/> class.
    /// </summary>
    public PageFormat()
    {
        EncodeValue = DefaultEncodeValue;
        ResolveLayout = DefaultResolveLayout;
        OnExpressionException = DefaultExpressionException;
        OnViewException = DefaultViewException;
    }

    /// <summary>
    /// Defaults the encode value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public string DefaultEncodeValue(object value)
    {
        if (value is IRawString rawString)
            return rawString.ToRawString();

        var str = value.ToString();
        if (str == string.Empty)
            return string.Empty;

        return str;
    }

    /// <summary>
    /// Defaults the resolve layout.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <returns>SharpPage.</returns>
    public SharpPage DefaultResolveLayout(SharpPage page)
    {
        page.Args.TryGetValue(SharpPages.Layout, out object layout);
        return page.Context.Pages.ResolveLayoutPage(page, layout as string);
    }

    /// <summary>
    /// Defaults the expression exception.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="ex">The ex.</param>
    /// <returns>System.Object.</returns>
    public virtual object DefaultExpressionException(PageResult result, Exception ex)
    {
        if (result.Page.Context.RenderExpressionExceptions)
            return $"{ex.GetType().Name}: ${ex.Message}";

        // Evaluate Null References in Binding Expressions to null
        if (ScriptConfig.CaptureAndEvaluateExceptionsToNull.Contains(ex.GetType()))
            return JsNull.Value;

        return null;
    }

    /// <summary>
    /// Defaults the view exception.
    /// </summary>
    /// <param name="pageResult">The page result.</param>
    /// <param name="req">The req.</param>
    /// <param name="ex">The ex.</param>
    public virtual async Task DefaultViewException(PageResult pageResult, IRequest req, Exception ex)
    {
        var sb = StringBuilderCache.Allocate();
        if (ContentType == MimeTypes.Html)
            sb.AppendLine("<pre class='error'>");
        sb.AppendLine($"{ex.GetType().Name}: {ex.Message}");
        if (pageResult.Context.DebugMode)
            sb.AppendLine(ex.StackTrace);

        if (ex.InnerException != null)
        {
            sb.AppendLine();
            sb.AppendLine("Inner Exceptions:");
            var innerEx = ex.InnerException;
            while (innerEx != null)
            {
                sb.AppendLine($"{innerEx.GetType().Name}: {innerEx.Message}");
                if (pageResult.Context.DebugMode)
                    sb.AppendLine(innerEx.StackTrace);
                innerEx = innerEx.InnerException;
            }
        }
        if (ContentType == MimeTypes.Html)
            sb.AppendLine("</pre>");
        var html = StringBuilderCache.ReturnAndFree(sb);
        await req.Response.OutputStream.WriteAsync(html).ConfigAwait();
    }
}

/// <summary>
/// Class HtmlPageFormat.
/// Implements the <see cref="ServiceStack.Script.PageFormat" />
/// </summary>
/// <seealso cref="ServiceStack.Script.PageFormat" />
public class HtmlPageFormat : PageFormat
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlPageFormat"/> class.
    /// </summary>
    public HtmlPageFormat()
    {
        ArgsPrefix = "<!--";
        ArgsSuffix = "-->";
        Extension = "html";
        ContentType = MimeTypes.Html;
        EncodeValue = HtmlEncodeValue;
        ResolveLayout = HtmlResolveLayout;
        OnExpressionException = HtmlExpressionException;
    }

    /// <summary>
    /// HTMLs the encode value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public static string HtmlEncodeValue(object value)
    {
        if (value == null)
            return string.Empty;

        if (value is IHtmlString htmlString)
            return htmlString.ToHtmlString();

        if (value is IRawString rawString)
            return rawString.ToRawString();

        var str = value.ToString();
        if (str == string.Empty)
            return string.Empty;

        return str.HtmlEncode();
    }

    /// <summary>
    /// HTMLs the resolve layout.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <returns>SharpPage.</returns>
    public SharpPage HtmlResolveLayout(SharpPage page)
    {
        var isCompletePage = page.BodyContents.Span.StartsWithIgnoreCase("<!DOCTYPE HTML>".AsSpan()) || page.BodyContents.Span.StartsWithIgnoreCase("<html".AsSpan());
        if (isCompletePage)
            return null;

        return base.DefaultResolveLayout(page);
    }

    /// <summary>
    /// HTMLs the expression exception.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="ex">The ex.</param>
    /// <returns>System.Object.</returns>
    public virtual object HtmlExpressionException(PageResult result, Exception ex)
    {
        if (result.Context.RenderExpressionExceptions)
            return ("<div class='error'><span>" + (ex.GetType().Name + ": " + ex.Message).HtmlEncode() + "</span></div>").ToRawString();

        // Evaluate Null References in Binding Expressions to null
        if (ScriptConfig.CaptureAndEvaluateExceptionsToNull.Contains(ex.GetType()))
            return JsNull.Value;

        return null;
    }

    /// <summary>
    /// HTMLs the encode transformer.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>Stream.</returns>
    public static async Task<Stream> HtmlEncodeTransformer(Stream stream)
    {
        var contents = await stream.ReadToEndAsync().ConfigAwait();
        var htmlEncoded = contents.HtmlEncode();
        return MemoryStreamFactory.GetStream(htmlEncoded.ToUtf8Bytes());
    }
}