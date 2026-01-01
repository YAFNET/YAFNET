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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages;

using System.Globalization;
using System.IO;
using System.Xml.Serialization;

using Core.Model;

using Types.Models;
using Types.Interfaces;

using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Types.Objects;

/// <summary>
/// Class SiteMapModel.
/// Implements the <see cref="YAF.Core.BasePages.ForumPage" />
/// </summary>
/// <seealso cref="YAF.Core.BasePages.ForumPage" />
public class SiteMapModel : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "SiteMapModel" /> class.
    /// </summary>
    public SiteMapModel()
        : base("SITEMAP", ForumPages.SiteMap)
    {
    }

    /// <summary>
    /// Loads the site map
    /// </summary>
    /// <returns>IActionResult.</returns>
    public IActionResult OnGet()
    {
        var siteMap = new SiteMap();

        var forumList = BoardContext.Current.GetRepository<Forum>().ListAllWithAccess(
            BoardContext.Current.BoardSettings.BoardId,
            BoardContext.Current.GuestUserID);

        forumList.ForEach(
            forum => siteMap.Add(
                new UrlLocation {
                                    Url =
                                        $"{this.Request.BaseUrl()}{BoardContext.Current.Get<ILinkBuilder>().GetForumLink(
                                            forum.Item1.ID,
                                            forum.Item1.Name)}",
                                    Priority = 0.8D,
                                    LastModified = forum.Item1.LastPosted.HasValue
                                                       ? forum.Item1.LastPosted.Value.ToString(
                                                           "yyyy-MM-ddTHH:mm:ss",
                                                           CultureInfo.InvariantCulture)
                                                       : DateTime.UtcNow.ToString(
                                                           "yyyy-MM-ddTHH:mm:ss",
                                                           CultureInfo.InvariantCulture),
                                    ChangeFrequency = UrlLocation.ChangeFrequencies.always
                                }));

        var xmlSerializer = new XmlSerializer(typeof(SiteMap));

        using var textWriter = new StringWriter();
        xmlSerializer.Serialize(textWriter, siteMap);

        return new ContentResult {
                                     ContentType = "application/xml",
                                     Content = textWriter.ToString(),
                                     StatusCode = 200
                                 };
    }
}