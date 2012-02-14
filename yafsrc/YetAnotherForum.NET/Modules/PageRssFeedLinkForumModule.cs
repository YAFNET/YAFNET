/* YetAnotherForum.NET
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
namespace YAF.Modules
{
  #region Using

  using System;
  using System.Web.UI.HtmlControls;

  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Constants;
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