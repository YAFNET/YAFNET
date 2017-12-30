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
namespace YAF.Modules
{
  #region Using

  using System;
  using System.Web.UI.HtmlControls;

  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// Summary description for PageRssFeedLinkModule
  /// </summary>
  [YafModule("Page Rss Feed Link Module", "Tiny Gecko", 1)]
  public class PageRssFeedLinkForumModule : SimpleBaseForumModule
  {
    #region Constants and Fields

    /// <summary>
    ///   The _forum page title.
    /// </summary>
    protected string _forumPageTitle;

    #endregion

    #region Public Methods

    /// <summary>
    /// The init after page.
    /// </summary>
    public override void InitAfterPage()
    {
      this.CurrentForumPage.PreRender += this.ForumPage_PreRender;
    }

    /// <summary>
    /// The init before page.
    /// </summary>
    public override void InitBeforePage()
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// The forum page_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumPage_PreRender([NotNull] object sender, [NotNull] EventArgs e)
    {
      HtmlHead head = this.ForumControl.Page.Header ??
                      this.CurrentForumPage.FindControlRecursiveBothAs<HtmlHead>("YafHead");

      if (head != null)
      {
        bool groupAccess =
          this.Get<IPermissions>().Check(this.PageContext.BoardSettings.PostLatestFeedAccess);

        if (this.PageContext.BoardSettings.ShowRSSLink && groupAccess)
        {
          // setup the rss link...
          var rssLink = new HtmlLink
            {
              Href =
                YafBuildLink.GetLink(
                  ForumPages.rsstopic,
                  true,
                  "pg={0}&ft={1}",
                  YafRssFeeds.LatestPosts.ToInt(),
                  YafSyndicationFormats.Rss.ToInt())
            };

          // defaults to the "Active" rss.

          rssLink.Attributes.Add("rel", "alternate");
          rssLink.Attributes.Add("type", "application/rss+xml");
          rssLink.Attributes.Add(
            "title", 
            "{0} - {1}".FormatWith(
              this.GetText("RSSFEED"), YafContext.Current.BoardSettings.Name));

          head.Controls.Add(rssLink);
        }

        if (this.PageContext.BoardSettings.ShowAtomLink && groupAccess)
        {
          // setup the rss link...
          var atomLink = new HtmlLink
            {
              Href =
                YafBuildLink.GetLink(
                  ForumPages.rsstopic,
                  true,
                  "pg={0}&ft={1}",
                  YafRssFeeds.LatestPosts.ToInt(),
                  YafSyndicationFormats.Atom.ToInt())
            };

          // defaults to the "Active" rss.

          atomLink.Attributes.Add("rel", "alternate");
          atomLink.Attributes.Add("type", "application/atom+xml");
          atomLink.Attributes.Add(
            "title", 
            "{0} - {1}".FormatWith(
              this.GetText("ATOMFEED"), YafContext.Current.BoardSettings.Name));

          head.Controls.Add(atomLink);
        }
      }
    }

    #endregion
  }
}