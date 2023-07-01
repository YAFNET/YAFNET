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

namespace YAF.Core.Extensions;

using YAF.Types.Attributes;

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
    [CanBeNull]
    public static T GetQueryOrRouteValue<T>([NotNull] this HttpRequest request, string queryName)
    {
        CodeContracts.VerifyNotNull(request);

        // Check if query string exist
        if (request.Query.ContainsKey(queryName))
        {
            var query = request.Query[queryName].FirstOrDefault().ToType<T>();

            return query;
        }

        // Check if route value exist
        if (request.RouteValues.TryGetValue(queryName, out var value))
        {
            var query = value.ToType<T>();

            return query;
        }

        return default;
    }
}