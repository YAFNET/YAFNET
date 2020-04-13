/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.Syndication
{
    #region Using

    using System;
    using System.ServiceModel.Syndication;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers.StringUtils;

    #endregion

    /// <summary>
    /// Class that generates all feeds
    /// </summary>
    public class YafSyndicationFeed : SyndicationFeed
    {
        #region Constants and Fields

        /// <summary>
        /// The feed categories.
        /// </summary>
        private const string FeedCategories = "YAF";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafSyndicationFeed" /> class.
        /// </summary>
        /// <param name="subTitle">The sub title.</param>
        /// <param name="feedType">The feed source.</param>
        /// <param name="sf">The feed type Atom/RSS.</param>
        /// <param name="urlAlphaNum">The alphanumerically encoded base site Url.</param>
        public YafSyndicationFeed([NotNull] string subTitle, RssFeeds feedType, int sf, [NotNull] string urlAlphaNum)
        {
            this.Copyright =
                new TextSyndicationContent($"Copyright {DateTime.Now.Year} {BoardContext.Current.BoardSettings.Name}");
            this.Description = new TextSyndicationContent(
                $"{BoardContext.Current.BoardSettings.Name} - {(sf == SyndicationFormats.Atom.ToInt() ? BoardContext.Current.Get<ILocalization>().GetText("ATOMFEED") : BoardContext.Current.Get<ILocalization>().GetText("RSSFEED"))}");
            this.Title = new TextSyndicationContent(
                $"{(sf == SyndicationFormats.Atom.ToInt() ? BoardContext.Current.Get<ILocalization>().GetText("ATOMFEED") : BoardContext.Current.Get<ILocalization>().GetText("RSSFEED"))} - {BoardContext.Current.BoardSettings.Name} - {subTitle}");

            // Alternate link
            this.Links.Add(SyndicationLink.CreateAlternateLink(new Uri(BaseUrlBuilder.BaseUrl)));

            // Self Link
            var slink = new Uri(
                BuildLink.GetLinkNotEscaped(ForumPages.RssTopic, true, $"pg={feedType.ToInt()}&ft={sf}"));
            this.Links.Add(SyndicationLink.CreateSelfLink(slink));

            this.Generator = "YetAnotherForum.NET";
            this.LastUpdatedTime = DateTime.UtcNow;
            this.Language = BoardContext.Current.Get<ILocalization>().LanguageCode;
            this.ImageUrl = new Uri(
                $"{BaseUrlBuilder.BaseUrl}{BoardInfo.ForumClientFileRoot}{BoardFolders.Current.Logos}/{BoardContext.Current.BoardSettings.ForumLogo}");

            this.Id =
                $"urn:{urlAlphaNum}:{(sf == SyndicationFormats.Atom.ToInt() ? BoardContext.Current.Get<ILocalization>().GetText("ATOMFEED") : BoardContext.Current.Get<ILocalization>().GetText("RSSFEED"))}:{BoardContext.Current.BoardSettings.Name}:{subTitle}:{BoardContext.Current.PageBoardID}"
                    .Unidecode();

            this.Id = this.Id.Replace(" ", string.Empty);

            // this.Id = "urn:uuid:{0}".FormatWith(Guid.NewGuid().ToString("D"));
            this.BaseUri = slink;
            this.Authors.Add(
                new SyndicationPerson(
                    BoardContext.Current.BoardSettings.ForumEmail,
                    "Forum Admin",
                    BaseUrlBuilder.BaseUrl));
            this.Categories.Add(new SyndicationCategory(FeedCategories));
        }

        #endregion
    }
}