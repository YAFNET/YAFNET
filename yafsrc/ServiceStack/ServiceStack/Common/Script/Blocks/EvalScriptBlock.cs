// ***********************************************************************
// <copyright file="EvalScriptBlock.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace ServiceStack.Script
{
    /// <summary>
    /// Special block which evaluates the rendered body as a ServiceStack Template
    /// Usages: {{#eval}}emit {{evaluateBodyOfArg}} at {{now}} {{/eval}}
    /// {{#eval {scopeArg:1} }}
    /// emit {{evaluateBodyOfArg}} at {{now}} with {{scopeArg}}
    /// {{/eval}}
    /// {{#eval {htmlDecode:true} }}
    /// emit htmldecoded {{evaluateBodyOfArg}} at {{now}}
    /// {{/eval}}
    /// {{#eval {use:{methods:'ServiceStackScripts',plugins:['MarkdownScriptPlugin']} }}
    /// emit {{evaluateBodyOfArg}} at {{now}} in new context
    /// {{/eval}}
    /// {{#eval { use:{context:true} } }}
    /// emit {{evaluateBodyOfArg}} in host context
    /// {{/eval}}
    /// </summary>
    public class EvalScriptBlock : ScriptBlock
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => "eval";
        /// <summary>
        /// Parse Body using Specified Language. Uses host language if unspecified.
        /// </summary>
        /// <value>The body.</value>
        public override ScriptLanguage Body => ScriptTemplate.Language;

        /// <summary>
        /// Write as an asynchronous operation.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="block">The block.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public override async Task WriteAsync(ScriptScopeContext scope, PageBlockFragment block, CancellationToken token)
        {
            var argValue = await block.Argument.GetJsExpressionAndEvaluateAsync(scope);
            var args = argValue as Dictionary<string, object> ?? new Dictionary<string, object>();

            var format = scope.Context.PageFormats.First().Extension;
            if (args.TryGetValue(ScriptConstants.Format, out var oFormat))
            {
                format = oFormat.ToString();
                args.Remove(ScriptConstants.Format);
            }

            var htmlDecode = false;
            if (args.TryGetValue(nameof(htmlDecode), out var oHtmlDecode)
                && oHtmlDecode is bool b)
            {
                htmlDecode = b;
                args.Remove(nameof(htmlDecode));
            }

            var context = scope.CreateNewContext(args);
            var unrenderedBody = new SharpPartialPage(scope.Context, "eval-page", block.Body, format, args);

            using var ms = MemoryStreamFactory.GetStream();
            var captureScope = scope.ScopeWith(outputStream: ms, scopedParams: args);
            await scope.PageResult.WritePageAsync(unrenderedBody, captureScope, token).ConfigAwait();
            // ReSharper disable once MethodHasAsyncOverload
            var renderedBody = ms.ReadToEnd();

            if (htmlDecode)
            {
                renderedBody = renderedBody.HtmlDecode();
            }

            var pageResult = new PageResult(context.OneTimePage(renderedBody))
            {
                Args = args,
            };
            await pageResult.WriteToAsync(scope.OutputStream, token).ConfigAwait();
        }
    }
}