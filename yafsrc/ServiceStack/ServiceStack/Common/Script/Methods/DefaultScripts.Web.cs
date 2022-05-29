// ***********************************************************************
// <copyright file="DefaultScripts.Web.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ServiceStack.Text;
using ServiceStack.Text.Common;
using ServiceStack.Web;

namespace ServiceStack.Script;

// ReSharper disable InconsistentNaming
/// <summary>
/// Class DefaultScripts.
/// Implements the <see cref="ServiceStack.Script.ScriptMethods" />
/// Implements the <see cref="ServiceStack.Script.IConfigureScriptContext" />
/// </summary>
/// <seealso cref="ServiceStack.Script.ScriptMethods" />
/// <seealso cref="ServiceStack.Script.IConfigureScriptContext" />
public partial class DefaultScripts
{
    /// <summary>
    /// Reqs the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>IRequest.</returns>
    internal IRequest req(ScriptScopeContext scope) => scope.GetValue(ScriptConstants.Request) as IRequest;

    /// <summary>
    /// Matcheses the path information.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="pathInfo">The path information.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool matchesPathInfo(ScriptScopeContext scope, string pathInfo) =>
        scope.GetValue("PathInfo")?.ToString().TrimEnd('/') == pathInfo?.TrimEnd('/');

    /// <summary>
    /// Startses the with path information.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="pathInfo">The path information.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool startsWithPathInfo(ScriptScopeContext scope, string pathInfo) => pathInfo == "/"
        ? matchesPathInfo(scope, pathInfo)
        : scope.GetValue("PathInfo")?.ToString().TrimEnd('/').StartsWith(pathInfo?.TrimEnd('/') ?? "") == true;

    /// <summary>
    /// Ifs the matches path information.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="returnTarget">The return target.</param>
    /// <param name="pathInfo">The path information.</param>
    /// <returns>System.Object.</returns>
    public object ifMatchesPathInfo(ScriptScopeContext scope, object returnTarget, string pathInfo) =>
        matchesPathInfo(scope, pathInfo) ? returnTarget : null;

    /// <summary>
    /// Determines whether [is HTTP get] [the specified scope].
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns><c>true</c> if [is HTTP get] [the specified scope]; otherwise, <c>false</c>.</returns>
    public bool isHttpGet(ScriptScopeContext scope) => req(scope)?.Verb == HttpMethods.Get;
    /// <summary>
    /// Determines whether [is HTTP post] [the specified scope].
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns><c>true</c> if [is HTTP post] [the specified scope]; otherwise, <c>false</c>.</returns>
    public bool isHttpPost(ScriptScopeContext scope) => req(scope)?.Verb == HttpMethods.Post;
    /// <summary>
    /// Determines whether [is HTTP put] [the specified scope].
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns><c>true</c> if [is HTTP put] [the specified scope]; otherwise, <c>false</c>.</returns>
    public bool isHttpPut(ScriptScopeContext scope) => req(scope)?.Verb == HttpMethods.Put;
    /// <summary>
    /// Determines whether [is HTTP delete] [the specified scope].
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns><c>true</c> if [is HTTP delete] [the specified scope]; otherwise, <c>false</c>.</returns>
    public bool isHttpDelete(ScriptScopeContext scope) => req(scope)?.Verb == HttpMethods.Delete;
    /// <summary>
    /// Determines whether [is HTTP patch] [the specified scope].
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns><c>true</c> if [is HTTP patch] [the specified scope]; otherwise, <c>false</c>.</returns>
    public bool isHttpPatch(ScriptScopeContext scope) => req(scope)?.Verb == HttpMethods.Patch;

    /// <summary>
    /// Ifs the HTTP get.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="ignoreTarget">The ignore target.</param>
    /// <returns>System.Object.</returns>
    public object ifHttpGet(ScriptScopeContext scope, object ignoreTarget) => ifHttpGet(scope);
    /// <summary>
    /// Ifs the HTTP get.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public object ifHttpGet(ScriptScopeContext scope) => isHttpGet(scope) ? IgnoreResult.Value : StopExecution.Value;
    /// <summary>
    /// Ifs the HTTP post.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="ignoreTarget">The ignore target.</param>
    /// <returns>System.Object.</returns>
    public object ifHttpPost(ScriptScopeContext scope, object ignoreTarget) => ifHttpPost(scope);
    /// <summary>
    /// Ifs the HTTP post.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public object ifHttpPost(ScriptScopeContext scope) => isHttpPost(scope) ? IgnoreResult.Value : StopExecution.Value;
    /// <summary>
    /// Ifs the HTTP put.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="ignoreTarget">The ignore target.</param>
    /// <returns>System.Object.</returns>
    public object ifHttpPut(ScriptScopeContext scope, object ignoreTarget) => ifHttpPut(scope);
    /// <summary>
    /// Ifs the HTTP put.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public object ifHttpPut(ScriptScopeContext scope) => isHttpPut(scope) ? IgnoreResult.Value : StopExecution.Value;
    /// <summary>
    /// Ifs the HTTP delete.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="ignoreTarget">The ignore target.</param>
    /// <returns>System.Object.</returns>
    public object ifHttpDelete(ScriptScopeContext scope, object ignoreTarget) => ifHttpDelete(scope);
    /// <summary>
    /// Ifs the HTTP delete.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public object ifHttpDelete(ScriptScopeContext scope) => isHttpDelete(scope) ? IgnoreResult.Value : StopExecution.Value;
    /// <summary>
    /// Ifs the HTTP patch.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="ignoreTarget">The ignore target.</param>
    /// <returns>System.Object.</returns>
    public object ifHttpPatch(ScriptScopeContext scope, object ignoreTarget) => ifHttpPatch(scope);
    /// <summary>
    /// Ifs the HTTP patch.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public object ifHttpPatch(ScriptScopeContext scope) => isHttpPatch(scope) ? IgnoreResult.Value : StopExecution.Value;

    /// <summary>
    /// Imports the request parameters.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public object importRequestParams(ScriptScopeContext scope)
    {
        var args = req(scope).GetRequestParams();
        foreach (var entry in args)
        {
            scope.ScopedParams[entry.Key] = entry.Value;
        }
        return StopExecution.Value;
    }

    /// <summary>
    /// Imports the request parameters.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="onlyImportArgNames">The only import argument names.</param>
    /// <returns>System.Object.</returns>
    public object importRequestParams(ScriptScopeContext scope, IEnumerable onlyImportArgNames)
    {
        var args = req(scope).GetRequestParams();
        var names = toVarNames(onlyImportArgNames);
        foreach (var name in names)
        {
            if (args.TryGetValue(name, out var value))
                scope.ScopedParams[name] = value;

        }
        return StopExecution.Value;
    }

    /// <summary>
    /// Requests the body.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>Stream.</returns>
    public Stream requestBody(ScriptScopeContext scope)
    {
        var httpReq = req(scope);
        httpReq.UseBufferedStream = true;
        return req(scope).InputStream;
    }

    /// <summary>
    /// Raws the body as string.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.String.</returns>
    public string rawBodyAsString(ScriptScopeContext scope) => req(scope).GetRawBody();
    /// <summary>
    /// Raws the body as json.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public object rawBodyAsJson(ScriptScopeContext scope) => JSON.parse(rawBodyAsString(scope));

    /// <summary>
    /// Requests the body as string.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public async Task<object> requestBodyAsString(ScriptScopeContext scope) => await req(scope).GetRawBodyAsync().ConfigAwait();
    /// <summary>
    /// Requests the body as json.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.Object.</returns>
    public async Task<object> requestBodyAsJson(ScriptScopeContext scope) => JSON.parse(await req(scope).GetRawBodyAsync().ConfigAwait());

    /// <summary>
    /// Forms the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>NameValueCollection.</returns>
    public NameValueCollection form(ScriptScopeContext scope) => req(scope).FormData;
    /// <summary>
    /// Queries the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>NameValueCollection.</returns>
    public NameValueCollection query(ScriptScopeContext scope) => req(scope).QueryString;
    /// <summary>
    /// Qses the specified scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>NameValueCollection.</returns>
    public NameValueCollection qs(ScriptScopeContext scope) => req(scope).QueryString;
    /// <summary>
    /// Queries the string.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.String.</returns>
    public string queryString(ScriptScopeContext scope)
    {
        var qs = req(scope).QueryString.ToString();
        return string.IsNullOrEmpty(qs) ? qs : "?" + qs;
    }

    /// <summary>
    /// Queries the dictionary.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    public Dictionary<string, object> queryDictionary(ScriptScopeContext scope) =>
        req(scope).QueryString.ToObjectDictionary();

    /// <summary>
    /// Forms the dictionary.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    public Dictionary<string, object> formDictionary(ScriptScopeContext scope) =>
        req(scope).FormData.ToObjectDictionary();

    /// <summary>
    /// To the query string.
    /// </summary>
    /// <param name="keyValuePairs">The key value pairs.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="System.NotSupportedException"></exception>
    public string toQueryString(object keyValuePairs)
    {
        var sb = StringBuilderCache.Allocate();
        var i = 0;
        if (keyValuePairs is IEnumerable<KeyValuePair<string, object>> kvps)
        {
            foreach (var entry in kvps)
            {
                if (i++ > 0)
                    sb.Append('&');

                sb.Append(entry.Key + "=" + entry.Value?.ToString().UrlEncode());
            }
        }
        else if (keyValuePairs is IDictionary d)
        {
            foreach (var key in d.Keys)
            {
                if (i++ > 0)
                    sb.Append('&');

                sb.Append(key + "=" + d[key]?.ToString().UrlEncode());
            }
        }
        else if (keyValuePairs is NameValueCollection nvc)
        {
            foreach (string key in nvc)
            {
                if (key == null)
                    continue;
                if (i++ > 0)
                    sb.Append('&');
                sb.Append(key + "=" + nvc[key].UrlEncode());
            }
        }
        else throw new NotSupportedException($"{nameof(toQueryString)} expects a collection of KeyValuePair's but was '{keyValuePairs.GetType().Name}'");

        return StringBuilderCache.ReturnAndFree(sb.Length > 0 ? sb.Insert(0, '?') : sb);
    }

    /// <summary>
    /// To the coerced dictionary.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    public Dictionary<string, object> toCoercedDictionary(object target)
    {
        var objDictionary = target.ToObjectDictionary();
        var keys = objDictionary.Keys.ToList();
        foreach (var key in keys)
        {
            var value = objDictionary[key];
            if (value is string str)
                objDictionary[key] = coerce(str);
        }
        return objDictionary;
    }

    /// <summary>
    /// Coerces the specified string.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns>System.Object.</returns>
    public object coerce(string str) => DynamicNumber.TryParse(str, out var numValue)
                                            ? numValue
                                            : str.StartsWith(DateTimeSerializer.WcfJsonPrefix)
                                                ? DateTimeSerializer.ParseDateTime(str)
                                                : str.EqualsIgnoreCase(bool.TrueString)
                                                    ? true
                                                    : str.EqualsIgnoreCase(bool.FalseString)
                                                        ? false
                                                        : str == "null"
                                                            ? (object)null
                                                            : str;

    /// <summary>
    /// HTTPs the method.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.String.</returns>
    public string httpMethod(ScriptScopeContext scope) => req(scope)?.Verb;
    /// <summary>
    /// HTTPs the request URL.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.String.</returns>
    public string httpRequestUrl(ScriptScopeContext scope) => req(scope)?.AbsoluteUri;
    /// <summary>
    /// HTTPs the path information.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <returns>System.String.</returns>
    public string httpPathInfo(ScriptScopeContext scope) => scope.GetValue("PathInfo")?.ToString();

    /// <summary>
    /// Forms the query.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public string formQuery(ScriptScopeContext scope, string name) => ViewUtils.FormQuery(req(scope), name);

    /// <summary>
    /// Forms the query values.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String[].</returns>
    public string[] formQueryValues(ScriptScopeContext scope, string name) => ViewUtils.FormQueryValues(req(scope), name);
    /// <summary>
    /// HTTPs the parameter.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="name">The name.</param>
    /// <returns>System.String.</returns>
    public string httpParam(ScriptScopeContext scope, string name) => ViewUtils.GetParam(req(scope), name);

    /// <summary>
    /// URLs the encode.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="upperCase">if set to <c>true</c> [upper case].</param>
    /// <returns>System.String.</returns>
    public string urlEncode(string value, bool upperCase) => value.UrlEncode(upperCase);
    /// <summary>
    /// URLs the encode.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public string urlEncode(string value) => value.UrlEncode();
    /// <summary>
    /// URLs the decode.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public string urlDecode(string value) => value.UrlDecode();

    /// <summary>
    /// HTMLs the encode.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public string htmlEncode(string value) => value.HtmlEncode();
    /// <summary>
    /// HTMLs the decode.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public string htmlDecode(string value) => value.HtmlDecode();

    /// <summary>
    /// Determines whether the specified target contains XSS.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if the specified target contains XSS; otherwise, <c>false</c>.</returns>
    /// <exception cref="System.NotSupportedException">containsXss cannot validate {target.GetType().Name}</exception>
    public bool containsXss(object target)
    {
        try
        {
            return MatchesStringValue(target, ContainsXss);
        }
        catch (ArgumentException)
        {
            throw new NotSupportedException($"containsXss cannot validate {target.GetType().Name}");
        }
    }

    /// <summary>
    /// Matcheses the string value.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="match">The match.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="System.ArgumentException">target</exception>
    public static bool MatchesStringValue(object target, Func<string, bool> match)
    {
        if (target == null)
            return false;

        if (target is string str)
        {
            if (match(str))
                return true;
        }
        else if (target is IDictionary d)
        {
            foreach (var item in d.Values)
            {
                if (item is string s && match(s))
                    return true;
            }
        }
        else if (target is IEnumerable objs)
        {
            foreach (var item in objs)
            {
                if (item is string s && match(s))
                    return true;
            }
        }
        else
        {
            throw new ArgumentException(nameof(target));
        }

        return false;
    }

    // tests for https://www.owasp.org/index.php/OWASP_Testing_Guide_Appendix_C:_Fuzz_Vectors#Cross_Site_Scripting_.28XSS.29
    /// <summary>
    /// The XSS fragments
    /// </summary>
    public static string[] XssFragments = { //greedy list
                                                  "<script",
                                                  "javascript:",
                                                  "%3A",       //= ':' URL Encode
                                                  "&#0000058", //= ':' HTML Entity Encode
                                                  "SRC=#",
                                                  "SRC=/",
                                                  "SRC=&",
                                                  "SRC= ",
                                                  "onload=",
                                                  "onload =",
                                                  "onunload=",
                                                  "onerror=",
                                                  "@import",
                                                  ":url(",
                                              };

    /// <summary>
    /// Determines whether the specified text contains XSS.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns><c>true</c> if the specified text contains XSS; otherwise, <c>false</c>.</returns>
    public static bool ContainsXss(string text)
    {
        foreach (var needle in XssFragments)
        {
            var pos = text.IndexOf(needle, 0, StringComparison.OrdinalIgnoreCase);
            if (pos >= 0)
                return true;
        }
        return false;
    }
}