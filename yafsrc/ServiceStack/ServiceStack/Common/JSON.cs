// ***********************************************************************
// <copyright file="JSON.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using ServiceStack.Script;

namespace ServiceStack;

/// <summary>
/// Class JS.
/// </summary>
public static class JS
{
    /// <summary>
    /// Creates the scope.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="functions">The functions.</param>
    /// <returns>ScriptScopeContext.</returns>
    public static ScriptScopeContext CreateScope(Dictionary<string, object> args = null, ScriptMethods functions = null)
    {
        var context = new ScriptContext();
        if (functions != null)
            context.ScriptMethods.Insert(0, functions);

        context.Init();
        return new ScriptScopeContext(new PageResult(context.EmptyPage), null, args);
    }
    /// <summary>
    /// Evals the specified js.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public static object eval(string js, ScriptScopeContext scope) => eval(js.AsSpan(), scope);
    /// <summary>
    /// Evals the specified js.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public static object eval(ReadOnlySpan<char> js, ScriptScopeContext scope)
    {
        js.ParseJsExpression(out var token);
        return ScriptLanguage.UnwrapValue(token.Evaluate(scope));
    }
}