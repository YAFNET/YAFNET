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

using System;

namespace YAF.Core.Extensions;

/// <summary>
///     The HttpRequest extensions.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Gets the query string or route value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request">The request.</param>
    /// <param name="queryName">Name of the query.</param>
    /// <returns>T.</returns>
    public static T GetQueryOrRouteValue<T>(this HttpRequest request, string queryName)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Check if query string exist
        T query;

        if (request.Query.ContainsKey(queryName))
        {
            query = request.Query[queryName].FirstOrDefault().ToType<T>();

            return query;
        }

        // Check if route value exist
        if (!request.RouteValues.TryGetValue(queryName, out var value))
        {
            return default;
        }

        query = value.ToType<T>();

        return query;
    }

    /// <summary>
    /// Gets the Base Url from the HttpRequest
    /// </summary>
    /// <param name="req">The req.</param>
    /// <returns>System.Nullable&lt;System.String&gt;.</returns>
    public static string BaseUrl(this HttpRequest req)
    {
        if (req == null)
        {
            return null;
        }

        var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port ?? -1);
        if (uriBuilder.Uri.IsDefaultPort)
        {
            uriBuilder.Port = -1;
        }

        return uriBuilder.Uri.AbsoluteUri;
    }
}