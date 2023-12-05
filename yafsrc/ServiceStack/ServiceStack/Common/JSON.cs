// ***********************************************************************
// <copyright file="JSON.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using ServiceStack.Script;
using ServiceStack.Text;
using ServiceStack.Text.Json;

namespace ServiceStack;

/// <summary>
/// Class JSON.
/// </summary>
public static class JSON
{
    /// <summary>
    /// Parses the specified json.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <returns>System.Object.</returns>
    public static object parse(string json)
    {
        json.AsSpan().ParseJsToken(out var token);
        return token?.Evaluate(JS.CreateScope());
    }

    /// <summary>
    /// Parses the span.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <returns>System.Object.</returns>
    public static object parseSpan(ReadOnlySpan<char> json)
    {
        if (json.Length == 0)
            return null;
        var firstChar = json[0];

        bool isEscapedJsonString(ReadOnlySpan<char> js) =>
            js.StartsWith(@"{\") || js.StartsWith(@"[{\");

        if (firstChar is >= '0' and <= '9')
        {
            try
            {
                var longValue = MemoryProvider.Instance.ParseInt64(json);
                return longValue is >= int.MinValue and <= int.MaxValue
                           ? (int)longValue
                           : longValue;
            }
            catch
            {
                // ignored
            }

            if (json.TryParseDouble(out var doubleValue))
                return doubleValue;
        }
        else if (firstChar == '{' || firstChar == '['
                 && !isEscapedJsonString(json.TrimStart()))
        {
            json.ParseJsToken(out var token);
            return token.Evaluate(JS.CreateScope());
        }
        else if (json.Length == 4)
        {
            if (firstChar == 't' && json[1] == 'r' && json[2] == 'u' && json[3] == 'e')
                return true;
            if (firstChar == 'n' && json[1] == 'u' && json[2] == 'l' && json[3] == 'l')
                return null;
        }
        else if (json.Length == 5 && firstChar == 'f' && json[1] == 'a' && json[2] == 'l' && json[3] == 's' && json[4] == 'e')
        {
            return false;
        }

        var unescapedString = JsonTypeSerializer.Unescape(json);
        return unescapedString.ToString();
    }
}

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