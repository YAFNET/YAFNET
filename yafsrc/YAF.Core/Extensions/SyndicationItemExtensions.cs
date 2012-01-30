/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
  using System;
  using System.Collections.Generic;
  using System.ServiceModel.Syndication;

  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  /// <summary>
  /// The syndication item extensions.
  /// </summary>
  public static class SyndicationItemExtensions
  {
    #region Public Methods

    /// <summary>
    /// The add syndication item.
    /// </summary>
    /// <param name="currentList">
    /// The current list.
    /// </param>
    /// <param name="title">
    /// The title.
    /// </param>
    /// <param name="content">
    /// The content.
    /// </param>
    /// <param name="summary">
    /// The summary.
    /// </param>
    /// <param name="link">
    /// The alternate link.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="posted">
    /// The posted.
    /// </param>
    public static void AddSyndicationItem(
      this List<SyndicationItem> currentList, string title, string content, string summary,string link, string id, DateTime posted, YafSyndicationFeed feed, List<SyndicationLink> mlinks)
    {
      var si = new SyndicationItem(
        YafContext.Current.Get<IBadWordReplace>().Replace(title),
        new TextSyndicationContent(YafContext.Current.Get<IBadWordReplace>().Replace(content),TextSyndicationContentKind.Html),
        // Alternate Link
        new Uri(link),
        id,
        new DateTimeOffset(posted)) {PublishDate = new DateTimeOffset(posted)};
      if (mlinks != null)
      {

        foreach (var syndicationLink in mlinks)
        {
          si.Links.Add(syndicationLink);
        }
      }
      si.Authors.Add(new SyndicationPerson(String.Empty, feed.Contributors[feed.Contributors.Count - 1].Name, String.Empty));
      si.SourceFeed = feed;
      if (summary.IsNotSet())
      {
        si.Summary = new TextSyndicationContent(YafContext.Current.Get<IBadWordReplace>().Replace(content),
          TextSyndicationContentKind.Html);
      }
      else
      {
        si.Summary = new TextSyndicationContent(YafContext.Current.Get<IBadWordReplace>().Replace(summary),
          TextSyndicationContentKind.Html);  
      }

      

      currentList.Add(si);
    }

    /// <summary>
    /// The add syndication item.
    /// </summary>
    /// <param name="currentList">
    /// The current list.
    /// </param>
    /// <param name="title">
    /// The title.
    /// </param>
    /// <param name="content">
    /// The content.
    /// </param>
    /// <param name="summary">
    /// The summary.
    /// </param>
    /// <param name="link">
    /// The alternate link.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="posted">
    /// The posted.
    /// </param>
    public static void AddSyndicationItem(
      this List<SyndicationItem> currentList, string title, string content, string summary, string link, string id, DateTime posted, YafSyndicationFeed feed)
    {
      AddSyndicationItem(
        currentList, title, content, summary, link, id, posted, feed, null);
    }

    /// <summary>
    /// Add a new syndication person.
    /// </summary>
    /// <param name="userEmail">The email.</param>
    /// <param name="userId">The user Id.</param>
    /// <returns>The SyndicationPerson.</returns>
    public static SyndicationPerson NewSyndicationPerson(string userEmail, long userId)
    {
      return new SyndicationPerson(userEmail, YafContext.Current.BoardSettings.EnableDisplayName
                                                ? UserMembershipHelper.GetDisplayNameFromID(userId)
                                                : UserMembershipHelper.GetUserNameFromID(userId), YafBuildLink.GetLinkNotEscaped(ForumPages.profile,true,"u={0}", userId));
    }

    /// <summary>
    /// The add syndication item.
    /// </summary>
    /// <param name="currentList">
    /// The current list.
    /// </param>
    /// <param name="title">
    /// The title.
    /// </param>
    /// <param name="content">
    /// The content.
    /// </param>
    /// <param name="link">
    /// The link.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="posted">
    /// The posted.
    /// </param>
    public static void AddSyndicationItem(
      this List<SyndicationItem> currentList, string title, string content, string link, string id, DateTime posted)
    {
      var si = new SyndicationItem(
        YafContext.Current.Get<IBadWordReplace>().Replace(title),
        new TextSyndicationContent(YafContext.Current.Get<IBadWordReplace>().Replace(content)),
        new Uri(link),
        id,
        new DateTimeOffset(posted));
      currentList.Add(si);
    }
    #endregion
  }
}