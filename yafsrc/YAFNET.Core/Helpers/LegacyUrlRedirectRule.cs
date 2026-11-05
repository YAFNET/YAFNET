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

namespace YAF.Core.Helpers;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Rewrite;

/// <summary>
/// Redirects the old urls of the public pages (e.g. /Posts/1/name) permanently to their
/// short url (/topic/1/name), so existing links and search engine results keep working.
/// </summary>
public class LegacyUrlRedirectRule : IRule
{
    /// <summary>
    /// The area of the forum.
    /// </summary>
    private readonly string area;

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyUrlRedirectRule"/> class.
    /// </summary>
    /// <param name="area">The area of the forum, if any.</param>
    public LegacyUrlRedirectRule(string area)
    {
        this.area = area;
    }

    /// <summary>
    /// Redirects the request if it is an old url, other methods than GET and HEAD are not redirected
    /// because a redirect would lose the posted data.
    /// </summary>
    /// <param name="context">The rewrite context.</param>
    public void ApplyRule(RewriteContext context)
    {
        var request = context.HttpContext.Request;

        if (!HttpMethods.IsGet(request.Method) && !HttpMethods.IsHead(request.Method))
        {
            return;
        }

        var shortPath = ForumRouteHelper.GetShortPath(request.Path.Value, this.area);

        if (shortPath is null)
        {
            return;
        }

        var response = context.HttpContext.Response;

        response.StatusCode = StatusCodes.Status301MovedPermanently;

        // the request path is decoded, BuildRelative encodes it again (e.g. non latin names)
        response.Headers.Location = UriHelper.BuildRelative(request.PathBase, new PathString(shortPath), request.QueryString);

        context.Result = RuleResult.EndResponse;
    }
}
