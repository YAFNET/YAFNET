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

using Microsoft.AspNetCore.Mvc.ApplicationModels;

using YAF.Types.Extensions;

/// <summary>
/// Replaces the page name in the routes of the public pages with its short url segment,
/// both for the pages of the application and of the area. The page names stay the same,
/// so the links to the pages get the short url without any change.
/// </summary>
public class ShortRouteConvention : IPageRouteModelConvention
{
    /// <summary>
    /// Applies the short url segment to the routes of the page.
    /// </summary>
    /// <param name="model">The page route model.</param>
    public void Apply(PageRouteModel model)
    {
        var page = model.ViewEnginePath.TrimStart('/');

        if (!ForumRouteHelper.PageSegments.TryGetValue(page, out var segment))
        {
            return;
        }

        var areaPrefix = model.AreaName.IsSet() ? $"{model.AreaName}/" : string.Empty;
        var pagePrefix = $"{areaPrefix}{page}";

        foreach (var selector in model.Selectors)
        {
            var route = selector.AttributeRouteModel;

            if (route?.Template is null || !route.Template.StartsWith(pagePrefix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            route.Template = $"{areaPrefix}{segment}{route.Template[pagePrefix.Length..]}";
        }

        if (!page.Equals(nameof(ForumPages.Posts), StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // the page number of a topic is in the path. It is a route of its own, because a topic without
        // a page number shows the first unread post, and the handlers are called without a page number
        model.Selectors.Add(
            new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel
                {
                    Template = $"{areaPrefix}{segment}/{{t:int}}/{{name}}/{{p:int}}/{{handler?}}", Order = -1
                }
            });
    }
}
