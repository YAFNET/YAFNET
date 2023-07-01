/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Middleware;

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;

using Newtonsoft.Json;

/// <summary>
/// The anti XSS middleware.
/// By https://www.loginradius.com/blog/async/anti-xss-middleware-asp-core/
/// </summary>
public class AntiXssMiddleware
{
    /// <summary>
    /// The status code.
    /// </summary>
    private readonly int statusCode = (int)HttpStatusCode.BadRequest;

    /// <summary>
    /// The next.
    /// </summary>
    private readonly RequestDelegate next;

    /// <summary>
    /// The error.
    /// </summary>
    private ErrorResponse error;

    /// <summary>
    /// Initializes a new instance of the <see cref="AntiXssMiddleware"/> class.
    /// </summary>
    /// <param name="requestDelegate">
    /// The request delegate.
    /// </param>
    public AntiXssMiddleware(RequestDelegate requestDelegate)
    {
        this.next = requestDelegate ?? throw new ArgumentNullException(nameof(requestDelegate));
    }

    /// <summary>
    /// The invoke.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // Check XSS in URL
        if (!string.IsNullOrWhiteSpace(context.Request.Path.Value))
        {
            var url = context.Request.Path.Value;

            if (CrossSiteScriptingValidation.IsDangerousString(url, out _))
            {
                await this.RespondWithAnErrorAsync(context).ConfigureAwait(false);
                return;
            }
        }

        // Check XSS in query string
        if (!string.IsNullOrWhiteSpace(context.Request.QueryString.Value))
        {
            var queryString = WebUtility.UrlDecode(context.Request.QueryString.Value);

            if (CrossSiteScriptingValidation.IsDangerousString(queryString, out _))
            {
                await this.RespondWithAnErrorAsync(context).ConfigureAwait(false);
                return;
            }
        }

        // Check XSS in request content
        var originalBody = context.Request.Body;
        try
        {
            /*var content = await ReadRequestBodyAsync(context);

            if (CrossSiteScriptingValidation.IsDangerousString(content, out _))
            {
                await this.RespondWithAnErrorAsync(context).ConfigureAwait(false);
                return;
            }*/

            await this.next(context).ConfigureAwait(false);
        }
        finally
        {
            context.Request.Body = originalBody;
        }
    }

    /// <summary>
    /// The read request body.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    private static async Task<string> ReadRequestBodyAsync(HttpContext context)
    {
        var buffer = new MemoryStream();
        await context.Request.Body.CopyToAsync(buffer);
        context.Request.Body = buffer;
        buffer.Position = 0;

        var encoding = Encoding.UTF8;

        var requestContent = await new StreamReader(buffer, encoding).ReadToEndAsync();
        context.Request.Body.Position = 0;

        return requestContent;
    }

    /// <summary>
    /// The respond with an error.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    private Task RespondWithAnErrorAsync(HttpContext context)
    {
        context.Response.Clear();
        context.Response.Headers.AddHeaders();
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.StatusCode = this.statusCode;

        this.error ??= new ErrorResponse { Description = "Error from AntiXssMiddleware", ErrorCode = 500 };

        return context.Response.WriteAsync(this.error.ToJson());
    }
}

public static class AntiXssMiddlewareExtension
{
    public static IApplicationBuilder UseAntiXssMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AntiXssMiddleware>();
    }
}

/// <summary>
/// Imported from System.Web.CrossSiteScriptingValidation Class
/// </summary>
public static class CrossSiteScriptingValidation
{
    private static readonly char[] StartingChars = { '<', '&' };

    public static bool IsDangerousString(string s, out int matchIndex)
    {
        // bool inComment = false;
        matchIndex = 0;

        for (var i = 0; ;)
        {
            // Look for the start of one of our patterns
            var n = s.IndexOfAny(StartingChars, i);

            // If not found, the string is safe
            if (n < 0) return false;

            // If it's the last char, it's safe
            if (n == s.Length - 1) return false;

            matchIndex = n;

            switch (s[n])
            {
                case '<':
                    // If the < is followed by a letter or '!', it's unsafe (looks like a tag or HTML comment)
                    if (IsAtoZ(s[n + 1]) || s[n + 1] == '!' || s[n + 1] == '/' || s[n + 1] == '?') return true;
                    break;
                case '&':
                    // If the & is followed by a #, it's unsafe (e.g. S)
                    if (s[n + 1] == '#') return true;
                    break;
            }

            // Continue searching
            i = n + 1;
        }
    }

    private static bool IsAtoZ(char c)
    {
        return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
    }

    public static void AddHeaders(this IHeaderDictionary headers)
    {
        if (headers["P3P"].NullOrEmpty())
        {
            headers.Add("P3P", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");
        }
    }

    public static string ToJson(this object value)
    {
        return JsonConvert.SerializeObject(value);
    }
}

public class ErrorResponse
{
    public int ErrorCode { get; set; }

    public string Description { get; set; }
}