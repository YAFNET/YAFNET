/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;

/// <summary>
/// The anti XSS middleware.
/// By https://www.loginradius.com/blog/async/anti-xss-middleware-asp-core/
/// </summary>
public class AntiXssMiddleware
{
    /// <summary>
    /// The status code.
    /// </summary>
    private const int StatusCode = (int)HttpStatusCode.BadRequest;

    /// <summary>
    /// The next.
    /// </summary>
    private readonly RequestDelegate next;

    /// <summary>
    /// The error.
    /// </summary>
    private ErrorResponse error;

    /// <summary>
    /// Initializes a new instance of the <see cref="AntiXssMiddleware" /> class.
    /// </summary>
    /// <param name="requestDelegate">The request delegate.</param>
    /// <exception cref="System.ArgumentNullException">requestDelegate</exception>
    public AntiXssMiddleware(RequestDelegate requestDelegate)
    {
        this.next = requestDelegate ?? throw new ArgumentNullException(nameof(requestDelegate));
    }

    /// <summary>
    /// The invoke.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The <see cref="Task" />.</returns>
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
            var path = context.Request.Path.Value;
            var content = await ReadRequestBodyAsync(context);

            if (path != null && (path.Contains("/api") || path.Contains("/Profile/") || path.Contains("/Admin/")
                                 || path.Contains("/EditAlbumImages/")))
            {
                await this.next(context).ConfigureAwait(false);
                return;
            }

            if (CrossSiteScriptingValidation.IsDangerousString(content, out _))
            {
                await this.RespondWithAnErrorAsync(context).ConfigureAwait(false);
                return;
            }

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
    /// <param name="context">The context.</param>
    /// <returns>The <see cref="Task" />.</returns>
    private async static Task<string> ReadRequestBodyAsync(HttpContext context)
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
    /// <param name="context">The context.</param>
    /// <returns>The <see cref="Task" />.</returns>
    private Task RespondWithAnErrorAsync(HttpContext context)
    {
        context.Response.Clear();
        context.Response.Headers.AddHeaders();
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.StatusCode = StatusCode;

        this.error ??= new ErrorResponse { Description = "Error from AntiXssMiddleware", ErrorCode = 500 };

        return context.Response.WriteAsync(this.error.ToJson());
    }
}

/// <summary>
/// Class AntiXssMiddlewareExtension.
/// </summary>
public static class AntiXssMiddlewareExtension
{
    /// <summary>
    /// Uses the anti XSS middleware.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>IApplicationBuilder.</returns>
    public static IApplicationBuilder UseAntiXssMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AntiXssMiddleware>();
    }
}

/// <summary>
/// Class ErrorResponse.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    /// <value>The error code.</value>
    public int ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }
}