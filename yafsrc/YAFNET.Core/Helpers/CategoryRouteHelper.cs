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

using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;

using YAF.Types.Extensions;

/// <summary>
/// Centralizes the /Category/{id}/{name} route definition
/// used by both URL generation (LinkBuilder) and URL rewriting (middleware).
/// </summary>
public static class CategoryRouteHelper
{
    /// <summary>
    /// The route segment.
    /// </summary>
    private const string PathSegment = "Category";

    /// <summary>
    /// Builds a category URL: /Category/{id}/{name} or /{area}/Category/{id}/{name}.
    /// When <paramref name="routeOptions"/> is provided, the URL respects
    /// <see cref="RouteOptions.LowercaseUrls"/> and <see cref="RouteOptions.AppendTrailingSlash"/>.
    /// </summary>
    public static string BuildUrl(string area, int categoryId, string categoryName,
        RouteOptions routeOptions = null)
    {
        var name = UrlRewriteHelper.CleanStringForUrl(categoryName);

        var url = area.IsSet()
            ? $"/{area}/{PathSegment}/{categoryId}/{name}"
            : $"/{PathSegment}/{categoryId}/{name}";

        if (routeOptions != null)
        {
            if (routeOptions.LowercaseUrls)
            {
                url = url.ToLowerInvariant();
            }
            if (routeOptions.AppendTrailingSlash && !url.EndsWith('/'))
            {
                url += "/";
            }
        }

        return url;
    }

    /// <summary>
    /// Adds rewrite rules that map /Category/{id}/{name} to the Index page.
    /// When <paramref name="routeOptions"/> is provided, the rewrite targets respect
    /// <see cref="RouteOptions.LowercaseUrls"/> and <see cref="RouteOptions.AppendTrailingSlash"/>.
    /// </summary>
    public static RewriteOptions AddCategoryRules(RewriteOptions options, string area,
        RouteOptions routeOptions = null)
    {
        var areaSegment = area.IsSet() ? $"{area}/" : string.Empty;
        var indexPage = "Index";

        var trailingSlash = "";
        if (routeOptions != null)
        {
            if (routeOptions.AppendTrailingSlash)
            {
                trailingSlash = "/";
            }
            if (routeOptions.LowercaseUrls)
            {
                areaSegment = areaSegment.ToLowerInvariant();
                indexPage = indexPage.ToLowerInvariant();
            }
        }

        return options
            .AddRewrite($@"(?i)^{areaSegment}{PathSegment}/(\d+)/([^/]+)/?$",
                $"{areaSegment}{indexPage}{trailingSlash}?c=$1",
                skipRemainingRules: true)
            // Also rewrite /Category/{id}/{name}/{handler} so page handlers like ShowMore work
            .AddRewrite($@"(?i)^{areaSegment}{PathSegment}/(\d+)/([^/]+)/([A-Za-z]+)/?$",
                $"{areaSegment}{indexPage}/$3{trailingSlash}?c=$1",
                skipRemainingRules: true);
    }
}
