/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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
  /// Summary description for YafSyndicationFeed.
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
    /// Initializes a new instance of the <see cref="YafSyndicationFeed"/> class.
    /// </summary>
    /// <param name="subTitle">
    /// </param>
    /// <param name="feedType">
    /// The feed source.
    /// </param>
    /// <param name="sf">
    /// The feed type Atom/Rss.
    /// </param>
    /// <param name="urlAlphaNum">
    /// The alphanumerically encoded base site Url.
    /// </param>
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

      this.Id = this.Id.Replace(" ", String.Empty);

      // this.Id = "urn:uuid:{0}".FormatWith(Guid.NewGuid().ToString("D"));
      this.BaseUri = slink;
      this.Authors.Add(
        new SyndicationPerson(YafContext.Current.BoardSettings.ForumEmail, "Forum Admin", BaseUrlBuilder.BaseUrl));
      this.Categories.Add(new SyndicationCategory(FeedCategories));

      // writer.WriteRaw("<?xml-stylesheet type=\"text/xsl\" href=\"" + YafForumInfo.ForumClientFileRoot + "rss.xsl\" media=\"screen\"?>");
    }

    #endregion
  }
}