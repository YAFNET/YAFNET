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

using System;
using System.Collections.Generic;

using YAF.Types.Extensions;

/// <summary>
/// Centralizes the short urls of the public pages, used by the route convention
/// (<see cref="ShortRouteConvention"/>) and the redirects of the old urls (<see cref="LegacyUrlRedirectRule"/>).
/// </summary>
public static class ForumRouteHelper
{
    /// <summary>
    /// The first url segment of the pages with a short url, by page name.
    /// </summary>
    public readonly static IReadOnlyDictionary<string, string> PageSegments =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { nameof(ForumPages.Posts), "topic" },
            { nameof(ForumPages.Topics), "forum" },
            { nameof(ForumPages.Post), "post" },
            { nameof(ForumPages.UserProfile), "user" },
            { nameof(ForumPages.Album), "album" },
            { nameof(ForumPages.Albums), "albums" },
            { nameof(ForumPages.Members), "members" },
            { nameof(ForumPages.ActiveUsers), "active-users" },
            { nameof(ForumPages.Team), "team" }
        };

    /// <summary>
    /// Gets the short path of an old url.
    /// </summary>
    /// <param name="path">The request path.</param>
    /// <param name="area">The area of the forum, if any.</param>
    /// <returns>The short path, or null if the path is not an old url.</returns>
    public static string GetShortPath(string path, string area)
    {
        var prefix = area.IsSet() ? $"/{area}/" : "/";

        if (path is null || !path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var rest = path[prefix.Length..];
        var slash = rest.IndexOf('/');
        var segment = slash < 0 ? rest : rest[..slash];
        var tail = slash < 0 ? string.Empty : rest[slash..];

        string shortSegment;

        if (segment.Equals(CategoryRouteHelper.PathSegment, StringComparison.OrdinalIgnoreCase))
        {
            shortSegment = CategoryRouteHelper.PathSegment;
        }
        else if (!PageSegments.TryGetValue(segment, out shortSegment))
        {
            return null;
        }

        // the actions of the PostsController are also below /Posts, only a topic starts with its id
        if (segment.Equals(nameof(ForumPages.Posts), StringComparison.OrdinalIgnoreCase)
            && (tail.Length < 2 || !char.IsAsciiDigit(tail[1])))
        {
            return null;
        }

        return segment.Equals(shortSegment, StringComparison.Ordinal) ? null : $"{prefix}{shortSegment}{tail}";
    }
}
