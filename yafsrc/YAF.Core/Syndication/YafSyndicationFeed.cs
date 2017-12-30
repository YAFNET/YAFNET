/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
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
namespace YAF.Core.Syndication
{
  #region Using

  using System;
  using System.IO;
  using System.ServiceModel.Syndication;

  using YAF.Classes;
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
    public YafSyndicationFeed([NotNull] string subTitle, YafRssFeeds feedType, int sf, [NotNull] string urlAlphaNum)
    {
      this.Copyright =
        new TextSyndicationContent(
          "Copyright {0} {1}".FormatWith(DateTime.Now.Year, YafContext.Current.BoardSettings.Name));
      this.Description =
        new TextSyndicationContent(
          "{0} - {1}".FormatWith(
            YafContext.Current.BoardSettings.Name, 
            sf == YafSyndicationFormats.Atom.ToInt()
              ? YafContext.Current.Get<ILocalization>().GetText("ATOMFEED")
              : YafContext.Current.Get<ILocalization>().GetText("RSSFEED")));
      this.Title =
        new TextSyndicationContent(
          "{0} - {1} - {2}".FormatWith(
            sf == YafSyndicationFormats.Atom.ToInt()
              ? YafContext.Current.Get<ILocalization>().GetText("ATOMFEED")
              : YafContext.Current.Get<ILocalization>().GetText("RSSFEED"), 
            YafContext.Current.BoardSettings.Name, 
            subTitle));

      // Alternate link
      this.Links.Add(SyndicationLink.CreateAlternateLink(new Uri(BaseUrlBuilder.BaseUrl)));

      // Self Link
      var slink =
        new Uri(
          YafBuildLink.GetLinkNotEscaped(ForumPages.rsstopic, true, "pg={0}&ft={1}".FormatWith(feedType.ToInt(), sf)));
      this.Links.Add(SyndicationLink.CreateSelfLink(slink));

      this.Generator = "YetAnotherForum.NET";
      this.LastUpdatedTime = DateTime.UtcNow;
      this.Language = YafContext.Current.Get<ILocalization>().LanguageCode;
      this.ImageUrl =
        new Uri("{0}/YAFLogo.png".FormatWith(Path.Combine(YafForumInfo.ForumBaseUrl, YafBoardFolders.Current.Images)));

      this.Id =
        "urn:{0}:{1}:{2}:{3}:{4}".FormatWith(
          urlAlphaNum, 
          sf == YafSyndicationFormats.Atom.ToInt()
            ? YafContext.Current.Get<ILocalization>().GetText("ATOMFEED")
            : YafContext.Current.Get<ILocalization>().GetText("RSSFEED"), 
          YafContext.Current.BoardSettings.Name, 
          subTitle, 
          YafContext.Current.PageBoardID).Unidecode();

      this.Id = this.Id.Replace(" ", string.Empty);

      // this.Id = "urn:uuid:{0}".FormatWith(Guid.NewGuid().ToString("D"));
      this.BaseUri = slink;
      this.Authors.Add(
        new SyndicationPerson(YafContext.Current.BoardSettings.ForumEmail, "Forum Admin", BaseUrlBuilder.BaseUrl));
      this.Categories.Add(new SyndicationCategory(FeedCategories));
    }

    #endregion
  }
}