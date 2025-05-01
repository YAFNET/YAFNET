/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;

/// <summary>
/// Class SecurityHeaderMiddleware.
/// </summary>
public class SecurityHeaderMiddleware
{
    /// <summary>
    /// The next.
    /// </summary>
    private readonly RequestDelegate next;

    /// <summary>
    /// Initializes a new instance of the <see cref="AntiXssMiddleware" /> class.
    /// </summary>
    /// <param name="requestDelegate">The request delegate.</param>
    /// <exception cref="System.ArgumentNullException">requestDelegate</exception>
    public SecurityHeaderMiddleware(RequestDelegate requestDelegate)
    {
        this.next = requestDelegate ?? throw new ArgumentNullException(nameof(requestDelegate));
    }

    /// <summary>
    /// Invokes the asynchronous.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>Task.</returns>
    public Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");

        context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");

        context.Response.Headers.Append("X-Xss-Protection", "1; mode=block");

        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        context.Response.Headers.Append("Referrer-Policy", "no-referrer");

        var csp = $"{context.Request.BaseUrl()};";
        context.Response.Headers.Append("Content-Security-Policy",
            new[] { csp });

        return this.next.Invoke(context);
    }
}

/// <summary>
/// Class ASecurityHeaderMiddlewareExtension.
/// </summary>
public static class SecurityHeaderMiddlewareExtension
{
    /// <summary>
    /// Uses the security header middleware.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>IApplicationBuilder.</returns>
    public static IApplicationBuilder UseSecurityHeader(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeaderMiddleware>();
    }
}