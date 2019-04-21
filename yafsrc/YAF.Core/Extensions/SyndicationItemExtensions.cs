/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
  using System;
  using System.Collections.Generic;
  using System.ServiceModel.Syndication;

  using YAF.Types.Constants;
  using YAF.Types.Extensions;
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
      this List<SyndicationItem> currentList, string title, string content, string summary, string link, string id, DateTime posted, YafSyndicationFeed feed, List<SyndicationLink> mlinks)
    {
      var si = new SyndicationItem(
                   YafContext.Current.Get<IBadWordReplace>().Replace(title),
                   new TextSyndicationContent(
                       YafContext.Current.Get<IBadWordReplace>().Replace(content),
                       TextSyndicationContentKind.Html),

                   // Alternate Link
                   new Uri(link),
                   id,
                   new DateTimeOffset(posted)) {
                                                  PublishDate = new DateTimeOffset(posted)
                                               };
      if (mlinks != null)
      {

        foreach (var syndicationLink in mlinks)
        {
          si.Links.Add(syndicationLink);
        }
      }

      si.Authors.Add(new SyndicationPerson(string.Empty, feed.Contributors[feed.Contributors.Count - 1].Name, string.Empty));
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
    /// <param name="userName">The user name.</param>
    /// <param name="userDisplayName"> The user dispaly name.</param>
    /// <returns>The SyndicationPerson.</returns>
    public static SyndicationPerson NewSyndicationPerson(string userEmail, long userId, string userName, string userDisplayName)
    {
        string userNameToShow;
        if (YafContext.Current.BoardSettings.EnableDisplayName)
        {
            userNameToShow = userDisplayName.IsNotSet() ? UserMembershipHelper.GetDisplayNameFromID(userId) : userDisplayName;
        }
        else
        {
            userNameToShow = userName.IsNotSet() ? UserMembershipHelper.GetUserNameFromID(userId) : userName;
        }

        return new SyndicationPerson(
            userEmail,
            userNameToShow,
            YafBuildLink.GetLinkNotEscaped(ForumPages.profile, true, "u={0}&name={1}", userId, userNameToShow));
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