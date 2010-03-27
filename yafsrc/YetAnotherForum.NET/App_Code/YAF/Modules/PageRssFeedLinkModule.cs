/* YetAnotherForum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
  using System;
  using System.Text;
  using System.Web.UI.HtmlControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Utils;
  using YAF.Controls;

  /// <summary>
  /// Summary description for PageRssFeedLinkModule
  /// </summary>
  [YafModule("Page Rss Feed Link Module", "Tiny Gecko", 1)]
  public class PageRssFeedLinkModule : SimpleBaseModule
  {
    /// <summary>
    /// The _forum page title.
    /// </summary>
    protected string _forumPageTitle = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="PageRssFeedLinkModule"/> class.
    /// </summary>
    public PageRssFeedLinkModule()
    {
    }

    /// <summary>
    /// The init after page.
    /// </summary>
    public override void InitAfterPage()
    {
      CurrentForumPage.PreRender += new EventHandler(ForumPage_PreRender);
    }

    /// <summary>
    /// The init before page.
    /// </summary>
    public override void InitBeforePage()
    {
    }

    /// <summary>
    /// The forum page_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumPage_PreRender(object sender, EventArgs e)
    {
      HtmlHead head = ForumControl.Page.Header ?? this.CurrentForumPage.FindControlRecursiveBothAs<HtmlHead>("YafHead");

      if (head != null && PageContext.BoardSettings.ShowRSSLink)
      {
        // setup the rss link...
        HtmlLink rssLink = new HtmlLink();

        // defaults to the "Active" rss.
        rssLink.Href = YafBuildLink.GetLink(ForumPages.rsstopic, "pg={0}", YafRssFeeds.Active.GetStringValue());

        rssLink.Attributes.Add("rel", "alternate");
        rssLink.Attributes.Add("type", "application/rss+xml");
        rssLink.Attributes.Add("title", String.Format( "{0} - {1}", PageContext.Localization.GetText("RSSFEED"),YafContext.Current.BoardSettings.Name));

        head.Controls.Add(rssLink);
      }
    }
  }
}