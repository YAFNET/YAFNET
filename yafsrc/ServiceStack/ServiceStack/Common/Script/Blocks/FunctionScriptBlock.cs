// ***********************************************************************
// <copyright file="FunctionScriptBlock.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace ServiceStack.Script;

/// <summary>
/// Define a reusable function
/// Usage: {{#function calc(a, b) }}
/// a * b | to =&gt; c
/// a + b + c | return
/// {{/function}}
/// </summary>
public class FunctionScriptBlock : ScriptBlock
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public override string Name => "function";
    /// <summary>
    /// Parse Body using Specified Language. Uses host language if unspecified.
    /// </summary>
    /// <value>The body.</value>
    public override ScriptLanguage Body => ScriptCode.Language;

    /// <summary>
    /// Writes the asynchronous.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="block">The block.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task.</returns>
    public override Task WriteAsync(ScriptScopeContext scope, PageBlockFragment block, CancellationToken token)
    {
        // block.Argument key is unique to exact memory fragment, not string equality
        // Parse into AST once for all Page Results
        var invokerCtx = (Tuple<string, StaticMethodInvoker>)scope.Context.CacheMemory.GetOrAdd(block.Argument, _ =>
            {
                var literal = block.Argument.Span.ParseVarName(out var name);
                var strName = name.ToString();
                literal = literal.AdvancePastWhitespace();

                literal = literal.AdvancePastWhitespace();
                var args = TypeConstants.EmptyStringArray;
                if (!literal.IsEmpty)
                {
                    literal.ParseArgumentsList(out var argIdentifiers);
                    args = new string[argIdentifiers.Count];
                    for (var i = 0; i < argIdentifiers.Count; i++)
                    {
                        args[i] = argIdentifiers[i].Name;
                    }
                }

                StaticMethodInvoker invoker = null;

                // Allow recursion by initializing lazy Delegate
                object LazyInvoker(object instance, object[] paramValues)
                {
                    if (invoker == null) throw new NotSupportedException($"Uninitialized function '{strName}'");

                    return invoker(instance, paramValues);
                }

                invoker = paramValues =>
                    {
                        scope.PageResult.StackDepth++;
                        try
                        {
                            var page = new SharpPage(Context, block.Body);
                            var pageResult = new PageResult(page)
                                                 {
                                                     Args = {
                                                                    [strName] = (MethodInvoker) LazyInvoker
                                                                },
                                                     StackDepth = scope.PageResult.StackDepth
                                                 };

                            var len = Math.Min(paramValues.Length, args.Length);
                            for (int i = 0; i < len; i++)
                            {
                                var paramValue = paramValues[i];
                                pageResult.Args[args[i]] = paramValue;
                            }

                            if (pageResult.EvaluateResult(out var returnValue))
                                return returnValue;

                            return IgnoreResult.Value;
                        }
                        finally
                        {
                            scope.PageResult.StackDepth--;
                        }
                    };

                return Tuple.Create(strName, invoker);
            });

        scope.PageResult.Args[invokerCtx.Item1] = invokerCtx.Item2;

        return TypeConstants.EmptyTask;
    }
}