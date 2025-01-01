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

namespace YAF.Core.Services.Syndication;

using System;
using System.ServiceModel.Syndication;

using YAF.Core.Services;
using YAF.Core.Utilities.StringUtils;
using YAF.Types.Constants;

/// <summary>
/// Class that generates all feeds
/// </summary>
public class FeedItem : SyndicationFeed
{
    /// <summary>
    /// The feed categories.
    /// </summary>
    private const string FeedCategories = "YAF";

    /// <summary>
    /// Initializes a new instance of the <see cref="FeedItem"/> class.
    /// </summary>
    /// <param name="subTitle">
    /// The sub title.
    /// </param>
    /// <param name="feedType">
    /// The feed source.
    /// </param>
    /// <param name="urlAlphaNum">
    /// The alphanumerically encoded base site Url.
    /// </param>
    public FeedItem(string subTitle, RssFeeds feedType, string urlAlphaNum)
    {
        this.Copyright = new TextSyndicationContent(
            $"Copyright {DateTime.Now.Year} {BoardContext.Current.BoardSettings.Name}");
        this.Description = new TextSyndicationContent(
            $"{BoardContext.Current.BoardSettings.Name} - {BoardContext.Current.Get<ILocalization>().GetText("ATOMFEED")}");
        this.Title = new TextSyndicationContent(
            $"{BoardContext.Current.Get<ILocalization>().GetText("ATOMFEED")} - {BoardContext.Current.BoardSettings.Name} - {subTitle}");

        // Alternate link
        this.Links.Add(SyndicationLink.CreateAlternateLink(new Uri(BoardContext.Current.Get<BoardInfo>().ForumBaseUrl)));

        // Self Link
        var slink = new Uri(
            $"{BoardContext.Current.Get<BoardInfo>().ForumBaseUrl}{BoardContext.Current.Get<IUrlHelper>().Action("GetLatestPosts", "Feed")}");
        this.Links.Add(SyndicationLink.CreateSelfLink(slink));

        this.Generator = "YetAnotherForum.NET";
        this.LastUpdatedTime = DateTime.UtcNow;
        this.Language = BoardContext.Current.Get<ILocalization>().LanguageCode;
        this.ImageUrl = new Uri(
            $"{BoardContext.Current.Get<BoardInfo>().ForumBaseUrl}/{BoardContext.Current.Get<BoardFolders>().Logos}/{BoardContext.Current.BoardSettings.ForumLogo}");

        this.Id =
            $"urn:{urlAlphaNum}:{BoardContext.Current.Get<ILocalization>().GetText("ATOMFEED")}:{BoardContext.Current.BoardSettings.Name}:{subTitle}:{BoardContext.Current.PageBoardID}"
                .Unidecode();

        this.Id = this.Id.Replace(" ", string.Empty);

        this.BaseUri = slink;
        this.Authors.Add(
            new SyndicationPerson(
                BoardContext.Current.BoardSettings.ForumEmail,
                "Forum Admin",
                BoardContext.Current.Get<BoardInfo>().ForumBaseUrl));
        this.Categories.Add(new SyndicationCategory(FeedCategories));
    }
}