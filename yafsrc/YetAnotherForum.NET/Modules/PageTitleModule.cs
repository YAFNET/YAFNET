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
  /// Summary description for PageTitleModule
  /// </summary>
  [YafModule("Page Title Module", "Tiny Gecko", 1)]
  public class PageTitleModule : SimpleBaseModule
  {
    /// <summary>
    /// The _forum page title.
    /// </summary>
    protected string _forumPageTitle = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="PageTitleModule"/> class.
    /// </summary>
    public PageTitleModule()
    {
    }

    /// <summary>
    /// The init after page.
    /// </summary>
    public override void InitAfterPage()
    {
      CurrentForumPage.PreRender += new EventHandler(ForumPage_PreRender);
      CurrentForumPage.Load += new EventHandler(ForumPage_Load);
    }

    /// <summary>
    /// The init before page.
    /// </summary>
    public override void InitBeforePage()
    {
    }

    /// <summary>
    /// The forum page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumPage_Load(object sender, EventArgs e)
    {
      GeneratePageTitle();
    }

    /// <summary>
    /// Creates this pages title and fires a PageTitleSet event if one is set
    /// </summary>
    private void GeneratePageTitle()
    {
      // compute page title..
      var title = new StringBuilder();

      string pageStr = string.Empty;

      if (ForumPageType == ForumPages.posts || ForumPageType == ForumPages.topics)
      {
        // get current page...
        var currentPager = (Pager) CurrentForumPage.FindControl("Pager");

        if (currentPager != null && currentPager.CurrentPageIndex != 0)
        {
          pageStr = "Page {0} - ".FormatWith(currentPager.CurrentPageIndex + 1);
        }
      }

      if (!PageContext.CurrentForumPage.IsAdminPage)
      {
        if (PageContext.PageTopicID != 0)
        {
          // Tack on the topic we're viewing
          title.AppendFormat("{0} - ", YafServices.BadWordReplace.Replace(PageContext.PageTopicName));
        }

        if (ForumPageType == ForumPages.posts)
        {
          title.Append(pageStr);
        }

        if (PageContext.PageForumName != string.Empty)
        {
          // Tack on the forum we're viewing
          title.AppendFormat("{0} - ", CurrentForumPage.HtmlEncode(PageContext.PageForumName));
        }

        if (ForumPageType == ForumPages.topics)
        {
          title.Append(pageStr);
        }
      }

      title.Append(CurrentForumPage.HtmlEncode(PageContext.BoardSettings.Name)); // and lastly, tack on the board's name
      this._forumPageTitle = title.ToString();

      ForumControl.FirePageTitleSet(this, new ForumPageTitleArgs(this._forumPageTitle));
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
      HtmlHead head = ForumControl.Page.Header ?? ControlHelper.FindControlRecursiveBothAs<HtmlHead>(CurrentForumPage, "YafHead");

      if (head != null)
      {
        // setup the title...
        string addition = string.Empty;

        if (head.Title.Trim().Length > 0)
        {
          addition = " - " + head.Title;
        }

        head.Title = this._forumPageTitle + addition;
      }
      else
      {
        // old style
        var title = ControlHelper.FindControlRecursiveBothAs<HtmlTitle>(CurrentForumPage, "ForumTitle");

        if (title != null)
        {
          title.Text = this._forumPageTitle;
        }
      }
    }
  }
}