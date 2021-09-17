// ***********************************************************************
// <copyright file="ScriptLanguage.Template.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Extensions;
using ServiceStack.Text;

namespace ServiceStack.Script
{
    /// <summary>
    /// #Script Language which processes ```lang ... ``` blocks
    /// </summary>
    public sealed class SharpScript : ScriptLanguage
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="SharpScript"/> class from being created.
        /// </summary>
        private SharpScript() { } // force usage of singleton

        /// <summary>
        /// The language
        /// </summary>
        public static readonly ScriptLanguage Language = new SharpScript();

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => "script";

        /// <summary>
        /// Parses the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="body">The body.</param>
        /// <param name="modifiers">The modifiers.</param>
        /// <returns>List&lt;PageFragment&gt;.</returns>
        public override List<PageFragment> Parse(ScriptContext context, ReadOnlyMemory<char> body, ReadOnlyMemory<char> modifiers)
        {
            return context.ParseScript(body);
        }
    }

    /// <summary>
    /// The #Script Default Template Language (does not process ```lang ... ``` blocks)
    /// </summary>
    public sealed class ScriptTemplate : ScriptLanguage
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="ScriptTemplate"/> class from being created.
        /// </summary>
        private ScriptTemplate() { } // force usage of singleton

        /// <summary>
        /// The language
        /// </summary>
        public static readonly ScriptLanguage Language = new ScriptTemplate();

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name => "template";

        /// <summary>
        /// Parses the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="body">The body.</param>
        /// <param name="modifiers">The modifiers.</param>
        /// <returns>List&lt;PageFragment&gt;.</returns>
        public override List<PageFragment> Parse(ScriptContext context, ReadOnlyMemory<char> body, ReadOnlyMemory<char> modifiers)
        {
            var pageFragments = context.ParseTemplate(body);
            return pageFragments;
        }

        /// <summary>
        /// Write page fragment as an asynchronous operation.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="fragment">The fragment.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="NotSupportedException">{{page}} can only be called once per render, in the Layout page.</exception>
        public override async Task<bool> WritePageFragmentAsync(ScriptScopeContext scope, PageFragment fragment, CancellationToken token)
        {
            if (fragment is PageStringFragment str)
            {
                await scope.OutputStream.WriteAsync(str.ValueUtf8, token).ConfigAwait();
            }
            else if (fragment is PageVariableFragment var)
            {
                if (var.Binding?.Equals(ScriptConstants.Page) == true
                    && !scope.ScopedParams.ContainsKey(ScriptConstants.PartialArg))
                {
                    if (scope.PageResult.PageProcessed)
                        throw new NotSupportedException("{{page}} can only be called once per render, in the Layout page.");
                    scope.PageResult.PageProcessed = true;

                    await scope.PageResult.WritePageAsync(scope.PageResult.Page, scope.PageResult.CodePage, scope, token).ConfigAwait();

                    if (scope.PageResult.HaltExecution)
                        scope.PageResult.HaltExecution = false; //break out of page but continue evaluating layout
                }
                else
                {
                    await scope.PageResult.WriteVarAsync(scope, var, token).ConfigAwait();
                }
            }
            else if (fragment is PageBlockFragment blockFragment)
            {
                var block = scope.PageResult.GetBlock(blockFragment.Name);
                await block.WriteAsync(scope, blockFragment, token).ConfigAwait();
            }
            else return false;

            return true;
        }
    }

    /// <summary>
    /// Class ScriptTemplateUtils.
    /// </summary>
    public static class ScriptTemplateUtils
    {
        /// <summary>
        /// Create SharpPage configured to use #Script
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="code">The code.</param>
        /// <returns>ServiceStack.Script.SharpPage.</returns>
        public static SharpPage SharpScriptPage(this ScriptContext context, string code)
            => context.Pages.OneTimePage(code, context.PageFormats[0].Extension,
                p => p.ScriptLanguage = SharpScript.Language);

        /// <summary>
        /// Create SharpPage configured to use #Script Templates
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="code">The code.</param>
        /// <returns>ServiceStack.Script.SharpPage.</returns>
        public static SharpPage TemplateSharpPage(this ScriptContext context, string code)
            => context.Pages.OneTimePage(code, context.PageFormats[0].Extension,
                p => p.ScriptLanguage = ScriptTemplate.Language);

        /// <summary>
        /// Render #Script output to string
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="script">The script.</param>
        /// <param name="error">The error.</param>
        /// <returns>string.</returns>
        public static string RenderScript(this ScriptContext context, string script, out ScriptException error) =>
            context.RenderScript(script, null, out error);
        /// <summary>
        /// Alias for EvaluateScript
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="script">The script.</param>
        /// <param name="error">The error.</param>
        /// <returns>string.</returns>
        public static string EvaluateScript(this ScriptContext context, string script, out ScriptException error) =>
            context.EvaluateScript(script, null, out error);

        /// <summary>
        /// Render #Script output to string
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="error">The error.</param>
        /// <returns>string.</returns>
        public static string RenderScript(this ScriptContext context, string script, Dictionary<string, object> args, out ScriptException error) =>
            context.EvaluateScript(script, args, out error);
        /// <summary>
        /// Alias for RenderScript
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="error">The error.</param>
        /// <returns>string.</returns>
        public static string EvaluateScript(this ScriptContext context, string script, Dictionary<string, object> args, out ScriptException error)
        {
            var pageResult = new PageResult(context.SharpScriptPage(script));
            args.Each((x, y) => pageResult.Args[x] = y);
            try
            {
                var output = pageResult.Result;
                error = pageResult.LastFilterError != null ? new ScriptException(pageResult) : null;
                return output;
            }
            catch (Exception e)
            {
                pageResult.LastFilterError = e;
                error = new ScriptException(pageResult);
                return null;
            }
        }

        /// <summary>
        /// Render #Script output to string
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>string.</returns>
        public static string RenderScript(this ScriptContext context, string script, Dictionary<string, object> args = null) =>
            context.EvaluateScript(script, args);
        /// <summary>
        /// Alias for RenderScript
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>string.</returns>
        public static string EvaluateScript(this ScriptContext context, string script, Dictionary<string, object> args = null)
        {
            var pageResult = new PageResult(context.SharpScriptPage(script));
            args.Each((x, y) => pageResult.Args[x] = y);
            return pageResult.RenderScript();
        }

        /// <summary>
        /// Render #Script output to string asynchronously
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Threading.Tasks.Task&lt;string&gt;.</returns>
        public static Task<string> RenderScriptAsync(this ScriptContext context, string script, Dictionary<string, object> args = null) =>
            context.EvaluateScriptAsync(script, args);

        /// <summary>
        /// Alias for RenderScriptAsync
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task<string> EvaluateScriptAsync(this ScriptContext context, string script, Dictionary<string, object> args = null)
        {
            var pageResult = new PageResult(context.SharpScriptPage(script));
            args.Each((x, y) => pageResult.Args[x] = y);
            return await pageResult.RenderScriptAsync().ConfigAwait();
        }

        /// <summary>
        /// Evaluate #Script and convert returned value to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>T.</returns>
        public static T Evaluate<T>(this ScriptContext context, string script, Dictionary<string, object> args = null) =>
            context.Evaluate(script, args).ConvertTo<T>();

        /// <summary>
        /// Evaluate #Script and return value
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>object.</returns>
        /// <exception cref="PageResult">context.SharpScriptPage(script)</exception>
        public static object Evaluate(this ScriptContext context, string script, Dictionary<string, object> args = null)
        {
            var pageResult = new PageResult(context.SharpScriptPage(script));
            args.Each((x, y) => pageResult.Args[x] = y);

            if (!pageResult.EvaluateResult(out var returnValue))
                throw new NotSupportedException(ScriptContextUtils.ErrorNoReturn);

            return ScriptLanguage.UnwrapValue(returnValue);
        }

        /// <summary>
        /// Evaluate #Script and convert returned value to T asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task<T> EvaluateAsync<T>(this ScriptContext context, string script, Dictionary<string, object> args = null) =>
            (await context.EvaluateAsync(script, args).ConfigAwait()).ConvertTo<T>();

        /// <summary>
        /// Evaluate #Script and convert returned value to T asynchronously
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="script">The script.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="PageResult">context.SharpScriptPage(script)</exception>
        public static async Task<object> EvaluateAsync(this ScriptContext context, string script, Dictionary<string, object> args = null)
        {
            var pageResult = new PageResult(context.SharpScriptPage(script));
            args.Each((x, y) => pageResult.Args[x] = y);

            var ret = await pageResult.EvaluateResultAsync().ConfigAwait();
            if (!ret.Item1)
                throw new NotSupportedException(ScriptContextUtils.ErrorNoReturn);

            return ScriptLanguage.UnwrapValue(ret.Item2);
        }


        /// <summary>
        /// Parses the template.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.Collections.Generic.List&lt;ServiceStack.Script.PageFragment&gt;.</returns>
        public static List<PageFragment> ParseTemplate(string text)
        {
            return new ScriptContext().Init().ParseTemplate(text.AsMemory());
        }

        /// <summary>
        /// The filter sep
        /// </summary>
        internal const char FilterSep = '|';
        /// <summary>
        /// The statements sep
        /// </summary>
        internal const char StatementsSep = ';';

        // {{#name}}  {{else if a=b}}  {{else}}  {{/name}}
        //          ^
        // returns    ^                         ^
        /// <summary>
        /// Parses the template body.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="blockName">Name of the block.</param>
        /// <param name="body">The body.</param>
        /// <returns>System.ReadOnlyMemory&lt;char&gt;.</returns>
        /// <exception cref="SyntaxErrorException">$"End block for '{blockName}' not found.</exception>
        public static ReadOnlyMemory<char> ParseTemplateBody(this ReadOnlyMemory<char> literal, ReadOnlyMemory<char> blockName, out ReadOnlyMemory<char> body)
        {
            var inStatements = 0;
            var pos = 0;

            while (true)
            {
                pos = literal.IndexOf("{{", pos);
                if (pos == -1)
                    throw new SyntaxErrorException($"End block for '{blockName}' not found.");

                var c = literal.SafeGetChar(pos + 2);

                if (c == '#')
                {
                    inStatements++;
                    pos = literal.IndexOf("}}", pos) + 2; //end of expression
                    continue;
                }

                if (c == '/')
                {
                    if (inStatements == 0)
                    {
                        literal.Slice(pos + 2 + 1).ParseVarName(out var name);
                        if (name.EqualsOrdinal(blockName))
                        {
                            body = literal.Slice(0, pos).TrimFirstNewLine();
                            return literal.Slice(pos);
                        }
                    }

                    inStatements--;
                }
                else if (literal.Slice(pos + 2).StartsWith("else"))
                {
                    if (inStatements == 0)
                    {
                        body = literal.Slice(0, pos).TrimFirstNewLine();
                        return literal.Slice(pos);
                    }
                }

                pos += 2;
            }
        }

        //   {{else if a=b}}  {{else}}  {{/name}}
        //  ^
        // returns           ^         ^
        /// <summary>
        /// Parses the template else block.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="blockName">Name of the block.</param>
        /// <param name="elseArgument">The else argument.</param>
        /// <param name="elseBody">The else body.</param>
        /// <returns>System.ReadOnlyMemory&lt;char&gt;.</returns>
        /// <exception cref="SyntaxErrorException">$"End block for 'else' not found.</exception>
        /// <exception cref="SyntaxErrorException">$"End block for 'else' not found.</exception>
        public static ReadOnlyMemory<char> ParseTemplateElseBlock(this ReadOnlyMemory<char> literal, string blockName,
            out ReadOnlyMemory<char> elseArgument, out ReadOnlyMemory<char> elseBody)
        {
            var inStatements = 0;
            var pos = 0;
            var statementPos = -1;
            elseBody = default;
            elseArgument = default;

            while (true)
            {
                pos = literal.IndexOf("{{", pos);
                if (pos == -1)
                    throw new SyntaxErrorException($"End block for 'else' not found.");

                var c = literal.SafeGetChar(pos + 2);
                if (c == '#')
                {
                    inStatements++;
                    pos = literal.IndexOf("}}", pos) + 2; //end of expression                    
                }
                else if (c == '/')
                {
                    if (inStatements == 0)
                    {
                        literal.Slice(pos + 2 + 1).ParseVarName(out var name);
                        if (name.EqualsOrdinal(blockName))
                        {
                            elseBody = literal.Slice(statementPos, pos - statementPos).TrimFirstNewLine();
                            return literal.Slice(pos);
                        }
                    }

                    inStatements--;
                }
                else if (literal.Slice(pos + 2).StartsWith("else"))
                {
                    if (inStatements == 0)
                    {
                        if (statementPos >= 0)
                        {
                            elseBody = literal.Slice(statementPos, pos - statementPos).TrimFirstNewLine();
                            return literal.Slice(pos);
                        }

                        var endExprPos = literal.IndexOf("}}", pos);
                        if (endExprPos == -1)
                            throw new SyntaxErrorException($"End expression for 'else' not found.");

                        var exprStartPos = pos + 2 + 4; //= {{else...

                        elseArgument = literal.Slice(exprStartPos, endExprPos - exprStartPos).Trim();
                        statementPos = endExprPos + 2;
                    }
                }

                pos += 2;
            }
        }

        /// <summary>
        /// Parses the script.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="text">The text.</param>
        /// <returns>System.Collections.Generic.List&lt;ServiceStack.Script.PageFragment&gt;.</returns>
        public static List<PageFragment> ParseScript(this ScriptContext context, ReadOnlyMemory<char> text)
        {
            var to = new List<PageFragment>();
            ScriptLanguage scriptLanguage = null;
            ReadOnlyMemory<char> modifiers = default;
            ReadOnlyMemory<char> prevBlock = default;
            int startBlockPos = -1;
            var cursorPos = 0;
            var lastBlockPos = 0;
            var inRawBlock = false;

            const int delim = 3; // '```'.length

            while (text.TryReadLine(out var line, ref cursorPos))
            {
                var lineLength = line.Length;
                line = line.AdvancePastWhitespace();

                if (line.IndexOf("{{#raw") >= 0 && line.IndexOf("{{/raw}}") < 0)
                {
                    inRawBlock = true;
                    continue;
                }
                if (line.IndexOf("{{/raw}}") >= 0)
                {
                    inRawBlock = false;
                    continue;
                }
                if (inRawBlock)
                    continue;

                if (line.StartsWith("```"))
                {
                    if (scriptLanguage != null && startBlockPos >= 0 && line.Slice(delim).AdvancePastWhitespace().IsEmpty) //is end block
                    {
                        var templateFragments = ScriptTemplate.Language.Parse(context, prevBlock);
                        to.AddRange(templateFragments);

                        var blockBody = text.ToLineStart(cursorPos, lineLength, startBlockPos);
                        var blockFragments = scriptLanguage.Parse(context, blockBody, modifiers);
                        to.AddRange(blockFragments);

                        prevBlock = default;
                        startBlockPos = -1;
                        scriptLanguage = null;
                        modifiers = null;
                        lastBlockPos = cursorPos;
                        continue;
                    }

                    if (line.SafeGetChar(delim).IsValidVarNameChar())
                    {
                        line = line.Slice(delim).ParseVarName(out var blockNameSpan);

                        var blockName = blockNameSpan.ToString();
                        scriptLanguage = context.GetScriptLanguage(blockName);
                        if (scriptLanguage == null)
                            continue;

                        modifiers = line.AdvancePastChar('|');
                        var delimLen = text.Span.SafeCharEquals(cursorPos - 2, '\r') ? 2 : 1;
                        prevBlock = text.Slice(lastBlockPos, cursorPos - lastBlockPos - lineLength - delimLen);
                        startBlockPos = cursorPos;
                    }
                }
            }

            var remainingBlock = text.Slice(lastBlockPos);
            if (!remainingBlock.IsEmpty)
            {
                var templateFragments = ScriptTemplate.Language.Parse(context, remainingBlock);
                to.AddRange(templateFragments);
            }

            return to;
        }

        /// <summary>
        /// Parses the template.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="text">The text.</param>
        /// <returns>System.Collections.Generic.List&lt;ServiceStack.Script.PageFragment&gt;.</returns>
        public static List<PageFragment> ParseTemplate(this ScriptContext context, ReadOnlyMemory<char> text)
        {
            var to = new List<PageFragment>();

            if (text.IsNullOrWhiteSpace())
                return to;

            int pos;
            var lastPos = 0;

            int nextPos()
            {
                var c1 = text.IndexOf("{{", lastPos);
                var c2 = text.IndexOf("{|", lastPos);

                if (c2 == -1)
                    return c1;

                return c1 == -1 ? c2 : c1 < c2 ? c1 : c2;
            }

            while ((pos = nextPos()) != -1)
            {
                var block = text.Slice(lastPos, pos - lastPos);
                if (!block.IsNullOrEmpty())
                    to.Add(new PageStringFragment(block));

                var varStartPos = pos + 2;

                if (varStartPos >= text.Span.Length)
                    throw new SyntaxErrorException($"Unterminated '{{{{' expression, near '{text.Slice(lastPos).DebugLiteral()}'");

                if (text.Span.SafeCharEquals(varStartPos - 1, '|')) // lang expression syntax {|lang ... |} https://flow.org/en/docs/types/objects/#toc-exact-object-types
                {
                    var literal = text.Slice(varStartPos);

                    ScriptLanguage lang = null;
                    if (literal.SafeGetChar(0).IsValidVarNameChar())
                    {
                        literal = literal.ParseVarName(out var langSpan);

                        lang = context.GetScriptLanguage(langSpan.ToString());
                        if (lang != null)
                        {
                            var endPos = literal.IndexOf("|}");
                            if (endPos == -1)
                                throw new SyntaxErrorException($"Unterminated '|}}' expression, near '{text.Slice(varStartPos).DebugLiteral()}'");

                            var exprStr = literal.Slice(0, endPos);
                            var langExprFragment = lang.Parse(context, exprStr);
                            to.AddRange(langExprFragment);
                        }
                    }
                    if (lang == null)
                    {
                        var nextLastPos = text.IndexOf("|}", varStartPos) + 2;
                        block = text.Slice(pos, nextLastPos - pos);
                        if (!block.IsNullOrEmpty())
                            to.Add(new PageStringFragment(block));
                    }

                    lastPos = text.IndexOf("|}", varStartPos) + 2;
                    continue;
                }

                var firstChar = text.Span[varStartPos];
                if (firstChar == '*') //comment
                {
                    lastPos = text.IndexOf("*}}", varStartPos) + 3;
                    if (text.Span.SafeCharEquals(lastPos, '\r')) lastPos++;
                    if (text.Span.SafeCharEquals(lastPos, '\n')) lastPos++;
                }
                else if (firstChar == '#') //block statement
                {
                    var literal = text.Slice(varStartPos + 1);

                    literal = literal.ParseTemplateScriptBlock(context, out var blockFragment);

                    var length = text.Length - pos - literal.Length;
                    blockFragment.OriginalText = text.Slice(pos, length);
                    lastPos = pos + length;

                    to.Add(blockFragment);
                }
                else
                {
                    var literal = text.Slice(varStartPos).Span;
                    literal = literal.ParseJsExpression(out var expr, filterExpression: true);

                    var filters = new List<JsCallExpression>();

                    if (!literal.StartsWith("}}"))
                    {
                        literal = literal.AdvancePastWhitespace();
                        if (literal.FirstCharEquals(FilterSep))
                        {
                            literal = literal.AdvancePastPipeOperator();

                            while (true)
                            {
                                literal = literal.ParseJsCallExpression(out var filter, filterExpression: true);

                                filters.Add(filter);

                                literal = literal.AdvancePastWhitespace();

                                if (literal.IsNullOrEmpty())
                                    throw new SyntaxErrorException("Unterminated filter expression");

                                if (literal.StartsWith("}}"))
                                {
                                    literal = literal.Advance(2);
                                    break;
                                }

                                if (!literal.FirstCharEquals(FilterSep))
                                    throw new SyntaxErrorException(
                                        $"Expected pipeline operator '|>' but was {literal.DebugFirstChar()}");

                                literal = literal.AdvancePastPipeOperator();
                            }
                        }
                        else if (!literal.AdvancePastWhitespace().IsNullOrEmpty())
                        {
                            throw new SyntaxErrorException($"Unexpected syntax '{literal.ToString()}', Expected pipeline operator '|>'");
                        }
                    }
                    else
                    {
                        literal = literal.Advance(2);
                    }

                    var length = text.Length - pos - literal.Length;
                    var originalText = text.Slice(pos, length);
                    lastPos = pos + length;

                    var varFragment = new PageVariableFragment(originalText, expr, filters);
                    to.Add(varFragment);

                    var newLineLen = literal.StartsWith("\n")
                        ? 1
                        : literal.StartsWith("\r\n")
                            ? 2
                            : 0;

                    if (newLineLen > 0)
                    {
                        var lastExpr = varFragment.FilterExpressions?.LastOrDefault();
                        var filterName = lastExpr?.Name ??
                                         varFragment?.InitialExpression?.Name ?? varFragment.Binding;
                        if (filterName != null && context.RemoveNewLineAfterFiltersNamed.Contains(filterName)
                            || expr is JsVariableDeclaration)
                        {
                            lastPos += newLineLen;
                        }
                    }
                }
            }

            if (lastPos != text.Length)
            {
                var lastBlock = lastPos == 0 ? text : text.Slice(lastPos);
                to.Add(new PageStringFragment(lastBlock));
            }

            return to;
        }

        // {{#if ...}}
        //    ^
        /// <summary>
        /// Parses the template script block.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="context">The context.</param>
        /// <param name="blockFragment">The block fragment.</param>
        /// <returns>System.ReadOnlyMemory&lt;char&gt;.</returns>
        /// <exception cref="SyntaxErrorException">$"Unterminated '{blockName}' block expression, near '{literal.DebugLiteral()}'</exception>
        /// <exception cref="SyntaxErrorException">$"Unterminated '{blockName}' block expression, near '{literal.DebugLiteral()}'</exception>
        public static ReadOnlyMemory<char> ParseTemplateScriptBlock(this ReadOnlyMemory<char> literal, ScriptContext context, out PageBlockFragment blockFragment)
        {
            literal = literal.ParseVarName(out var blockNameSpan);

            var blockName = blockNameSpan.ToString();
            var endBlock = "{{/" + blockName + "}}";
            var endExprPos = literal.IndexOf("}}");
            if (endExprPos == -1)
                throw new SyntaxErrorException($"Unterminated '{blockName}' block expression, near '{literal.DebugLiteral()}'");

            var argument = literal.Slice(0, endExprPos).Trim();
            literal = literal.Advance(endExprPos + 2);

            var language = context.ParseAsLanguage.TryGetValue(blockName, out var lang)
                ? lang
                : ScriptTemplate.Language;

            if (language.Name == ScriptVerbatim.Language.Name)
            {
                var endBlockPos = literal.IndexOf(endBlock);
                if (endBlockPos == -1)
                    throw new SyntaxErrorException($"Unterminated end block '{endBlock}'");

                var body = literal.Slice(0, endBlockPos);
                literal = literal.Advance(endBlockPos + endBlock.Length).TrimFirstNewLine();

                blockFragment = language.ParseVerbatimBlock(blockName, argument, body);
                return literal;
            }

            literal = literal.ParseTemplateBody(blockNameSpan, out var bodyText);
            var bodyFragments = language.Parse(context, bodyText);

            var elseBlocks = new List<PageElseBlock>();
            while (literal.StartsWith("{{else"))
            {
                literal = literal.ParseTemplateElseBlock(blockName, out var elseArgument, out var elseBody);

                var elseBlock = new PageElseBlock(elseArgument, language.Parse(context, elseBody));
                elseBlocks.Add(elseBlock);
            }

            literal = literal.Advance(2 + 1 + blockName.Length + 2);

            //remove new line after partial block end tag
            literal = literal.TrimFirstNewLine();

            blockFragment = new PageBlockFragment(blockName, argument, bodyFragments, elseBlocks);

            return literal;
        }

        /// <summary>
        /// Converts to rawstring.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>ServiceStack.IRawString.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRawString ToRawString(this string value) => value != null
            ? new RawString(value)
            : RawString.Empty;

        /// <summary>
        /// Gets the binder cache.
        /// </summary>
        /// <value>The binder cache.</value>
        public static ConcurrentDictionary<string, Func<ScriptScopeContext, object, object>> BinderCache { get; } = new ConcurrentDictionary<string, Func<ScriptScopeContext, object, object>>();

        /// <summary>
        /// Gets the member expression.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>System.Func&lt;ServiceStack.Script.ScriptScopeContext, object, object&gt;.</returns>
        /// <exception cref="ArgumentNullException">nameof(targetType)</exception>
        /// <exception cref="ArgumentNullException">nameof(targetType)</exception>
        public static Func<ScriptScopeContext, object, object> GetMemberExpression(Type targetType, ReadOnlyMemory<char> expression)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));
            if (expression.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(expression));

            var key = targetType.FullName + ':' + expression;

            if (BinderCache.TryGetValue(key, out var fn))
                return fn;

            BinderCache[key] = fn = Compile(targetType, expression);

            return fn;
        }

        /// <summary>
        /// Compiles the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="expr">The expr.</param>
        /// <returns>System.Func&lt;ServiceStack.Script.ScriptScopeContext, object, object&gt;.</returns>
        public static Func<ScriptScopeContext, object, object> Compile(Type type, ReadOnlyMemory<char> expr)
        {
            var scope = Expression.Parameter(typeof(ScriptScopeContext), "scope");
            var param = Expression.Parameter(typeof(object), "instance");
            var body = CreateBindingExpression(type, expr, scope, param);

            body = Expression.Convert(body, typeof(object));
            return Expression.Lambda<Func<ScriptScopeContext, object, object>>(body, scope, param).Compile();
        }

        /// <summary>
        /// Creates the binding expression.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="expr">The expr.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>System.Linq.Expressions.Expression.</returns>
        /// <exception cref="BindingExpressionException">$"Calling methods in '{expr}' is not allowed in binding expressions, use a filter instead., member.ToString(), expr.ToString()</exception>
        /// <exception cref="BindingExpressionException">$"Calling methods in '{expr}' is not allowed in binding expressions, use a filter instead., member.ToString(), expr.ToString()</exception>
        /// <exception cref="BindingExpressionException">$"Calling methods in '{expr}' is not allowed in binding expressions, use a filter instead., member.ToString(), expr.ToString()</exception>
        /// <exception cref="BindingExpressionException">$"Calling methods in '{expr}' is not allowed in binding expressions, use a filter instead., member.ToString(), expr.ToString()</exception>
        private static Expression CreateBindingExpression(Type type, ReadOnlyMemory<char> expr, ParameterExpression scope, ParameterExpression instance)
        {
            Expression body = Expression.Convert(instance, type);

            var currType = type;

            var pos = 0;
            var depth = 0;
            var delim = ".".AsMemory();
            while (expr.TryReadPart(delim, out ReadOnlyMemory<char> member, ref pos))
            {
                try
                {
                    if (member.IndexOf('(') >= 0)
                        throw new BindingExpressionException(
                            $"Calling methods in '{expr}' is not allowed in binding expressions, use a filter instead.",
                            member.ToString(), expr.ToString());

                    var indexerPos = member.IndexOf('[');
                    if (indexerPos >= 0)
                    {
                        var prop = member.LeftPart('[');
                        var indexer = member.RightPart('[');
                        indexer.Span.ParseJsExpression(out var token);

                        if (token is JsCallExpression)
                            throw new BindingExpressionException($"Only constant binding expressions are supported: '{expr}'",
                                member.ToString(), expr.ToString());

                        var value = JsToken.UnwrapValue(token);

                        var valueExpr = value == null
                            ? (Expression)Expression.Call(
                                typeof(ScriptTemplateUtils).GetStaticMethod(nameof(EvaluateBinding)),
                                scope,
                                Expression.Constant(token))
                            : Expression.Constant(value);

                        if (currType == typeof(string))
                        {
                            body = CreateStringIndexExpression(body, token, scope, valueExpr, ref currType);
                        }
                        else if (currType.IsArray)
                        {
                            if (token != null)
                            {
                                var evalAsInt = typeof(ScriptTemplateUtils).GetStaticMethod(nameof(EvaluateBindingAs))
                                    .MakeGenericMethod(typeof(int));
                                body = Expression.ArrayIndex(body,
                                    Expression.Call(evalAsInt, scope, Expression.Constant(token)));
                            }
                            else
                            {
                                body = Expression.ArrayIndex(body, valueExpr);
                            }
                        }
                        else if (depth == 0)
                        {
                            var pi = AssertProperty(currType, "Item", expr);
                            currType = pi.PropertyType;

                            if (token != null)
                            {
                                var indexType = pi.GetGetMethod()?.GetParameters().FirstOrDefault()?.ParameterType;
                                if (indexType != typeof(object) && !(valueExpr is ConstantExpression ce && ce.Type == indexType))
                                {
                                    var evalAsIndexType = typeof(ScriptTemplateUtils).GetStaticMethod(nameof(EvaluateBindingAs))
                                        .MakeGenericMethod(indexType);
                                    valueExpr = Expression.Call(evalAsIndexType, scope, Expression.Constant(token));
                                }
                            }

                            body = Expression.Property(body, "Item", valueExpr);
                        }
                        else
                        {
                            var pi = AssertProperty(currType, prop.ToString(), expr);
                            currType = pi.PropertyType;
                            body = Expression.PropertyOrField(body, prop.ToString());

                            if (currType == typeof(string))
                            {
                                body = CreateStringIndexExpression(body, token, scope, valueExpr, ref currType);
                            }
                            else
                            {
                                var indexMethod = currType.GetMethod("get_Item", new[] { value.GetType() });
                                body = Expression.Call(body, indexMethod, valueExpr);
                                currType = indexMethod.ReturnType;
                            }
                        }
                    }
                    else
                    {
                        if (depth >= 1)
                        {
                            var memberName = member.ToString();
                            if (typeof(IDictionary).IsAssignableFrom(currType))
                            {
                                var pi = AssertProperty(currType, "Item", expr);
                                currType = pi.PropertyType;
                                body = Expression.Property(body, "Item", Expression.Constant(memberName));
                            }
                            else
                            {
                                body = Expression.PropertyOrField(body, memberName);
                                var pi = currType.GetProperty(memberName);
                                if (pi != null)
                                {
                                    currType = pi.PropertyType;
                                }
                                else
                                {
                                    var fi = currType.GetField(memberName);
                                    if (fi != null)
                                        currType = fi.FieldType;
                                }

                            }
                        }
                    }

                    depth++;
                }
                catch (BindingExpressionException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new BindingExpressionException($"Could not compile '{member}' from expression '{expr}'",
                        member.ToString(), expr.ToString(), e);
                }
            }
            return body;
        }

        /// <summary>
        /// The object argument
        /// </summary>
        private static readonly Type[] ObjectArg = { typeof(object) };
        /// <summary>
        /// Creates the convert method.
        /// </summary>
        /// <param name="toType">To type.</param>
        /// <returns>System.Reflection.MethodInfo.</returns>
        public static MethodInfo CreateConvertMethod(Type toType) =>
            typeof(AutoMappingUtils).GetStaticMethod(nameof(AutoMappingUtils.ConvertTo), ObjectArg).MakeGenericMethod(toType);

        /// <summary>
        /// Compiles the assign.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="expr">The expr.</param>
        /// <returns>System.Action&lt;ServiceStack.Script.ScriptScopeContext, object, object&gt;.</returns>
        /// <exception cref="BindingExpressionException">$"Assignment expression for '{expr}' not supported yet, valueToAssign, expr.ToString()</exception>
        public static Action<ScriptScopeContext, object, object> CompileAssign(Type type, ReadOnlyMemory<char> expr)
        {
            var scope = Expression.Parameter(typeof(ScriptScopeContext), "scope");
            var instance = Expression.Parameter(typeof(object), "instance");
            var valueToAssign = Expression.Parameter(typeof(object), "valueToAssign");

            var body = CreateBindingExpression(type, expr, scope, instance);
            if (body is IndexExpression propItemExpr)
            {
                var mi = propItemExpr.Indexer.GetSetMethod();
                var indexExpr = propItemExpr.Arguments[0];
                if (propItemExpr.Indexer.PropertyType != typeof(object))
                {
                    body = Expression.Call(propItemExpr.Object, mi, indexExpr,
                        Expression.Call(CreateConvertMethod(propItemExpr.Indexer.DeclaringType.GetCollectionType()), valueToAssign));
                }
                else
                {
                    body = Expression.Call(propItemExpr.Object, mi, indexExpr, valueToAssign);
                }
            }
            else if (body is BinaryExpression binaryExpr && binaryExpr.NodeType == ExpressionType.ArrayIndex)
            {
                var arrayInstance = binaryExpr.Left;
                var indexExpr = binaryExpr.Right;

                if (arrayInstance.Type != typeof(object))
                {
                    body = Expression.Assign(
                        Expression.ArrayAccess(arrayInstance, indexExpr),
                        Expression.Call(CreateConvertMethod(arrayInstance.Type.GetElementType()), valueToAssign));
                }
                else
                {
                    body = Expression.Assign(
                        Expression.ArrayAccess(arrayInstance, indexExpr),
                        valueToAssign);
                }
            }
            else if (body is MemberExpression propExpr)
            {
                if (propExpr.Type != typeof(object))
                {
                    body = Expression.Assign(propExpr, Expression.Call(CreateConvertMethod(propExpr.Type), valueToAssign));
                }
                else
                {
                    body = Expression.Assign(propExpr, valueToAssign);
                }
            }
            else
                throw new BindingExpressionException($"Assignment expression for '{expr}' not supported yet", "valueToAssign", expr.ToString());

            return Expression.Lambda<Action<ScriptScopeContext, object, object>>(body, scope, instance, valueToAssign).Compile();
        }

        /// <summary>
        /// Creates the string index expression.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="binding">The binding.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="valueExpr">The value expr.</param>
        /// <param name="currType">Type of the curr.</param>
        /// <returns>System.Linq.Expressions.Expression.</returns>
        private static Expression CreateStringIndexExpression(Expression body, JsToken binding, ParameterExpression scope,
            Expression valueExpr, ref Type currType)
        {
            body = Expression.Call(body, typeof(string).GetMethod(nameof(string.ToCharArray), Type.EmptyTypes));
            currType = typeof(char[]);

            if (binding != null)
            {
                var evalAsInt = typeof(ScriptTemplateUtils).GetStaticMethod(nameof(EvaluateBindingAs))
                    .MakeGenericMethod(typeof(int));
                body = Expression.ArrayIndex(body, Expression.Call(evalAsInt, scope, Expression.Constant(binding)));
            }
            else
            {
                body = Expression.ArrayIndex(body, valueExpr);
            }
            return body;
        }

        /// <summary>
        /// Evaluates the binding.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="token">The token.</param>
        /// <returns>object.</returns>
        public static object EvaluateBinding(ScriptScopeContext scope, JsToken token)
        {
            var result = token.Evaluate(scope);
            return result;
        }

        /// <summary>
        /// Evaluates the binding as.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope">The scope.</param>
        /// <param name="token">The token.</param>
        /// <returns>T.</returns>
        public static T EvaluateBindingAs<T>(ScriptScopeContext scope, JsToken token)
        {
            var result = EvaluateBinding(scope, token);
            var converted = result.ConvertTo<T>();
            return converted;
        }

        /// <summary>
        /// Asserts the property.
        /// </summary>
        /// <param name="currType">Type of the curr.</param>
        /// <param name="prop">The property.</param>
        /// <param name="expr">The expr.</param>
        /// <returns>System.Reflection.PropertyInfo.</returns>
        /// <exception cref="ArgumentException">$"Property '{prop}' does not exist on Type '{currType.Name}' from binding expression '{expr}'</exception>
        private static PropertyInfo AssertProperty(Type currType, string prop, ReadOnlyMemory<char> expr)
        {
            var pi = currType.GetProperty(prop);
            if (pi == null)
                throw new ArgumentException(
                    $"Property '{prop}' does not exist on Type '{currType.Name}' from binding expression '{expr}'");
            return pi;
        }

        /// <summary>
        /// Determines whether [is white space] [the specified c].
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>bool.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteSpace(this char c) =>
            c == ' ' || c >= '\x0009' && c <= '\x000d' || c == '\x00a0' || c == '\x0085';
    }
}