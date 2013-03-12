/* YetAnotherForum.NET
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
namespace YAF.Modules
{
  #region Using

  using System;
  using System.Text;
  using System.Web.UI.HtmlControls;

  using YAF.Controls;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// Summary description for PageTitleModule
  /// </summary>
  [YafModule("Page Title Module", "Tiny Gecko", 1)]
  public class PageTitleForumModule : SimpleBaseForumModule
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
      this.CurrentForumPage.Load += this.ForumPage_Load;
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
    /// The forum page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumPage_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.GeneratePageTitle();
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
    private void ForumPage_PreRender([NotNull] object sender, [NotNull] EventArgs e)
    {
      HtmlHead head = this.ForumControl.Page.Header ??
                      this.CurrentForumPage.FindControlRecursiveBothAs<HtmlHead>("YafHead");

      if (head != null)
      {
        // setup the title...
        string addition = string.Empty;

        if (head.Title.IsSet())
        {
          addition = " - " + head.Title.Trim();
        }

        head.Title = this._forumPageTitle + addition;
      }
      else
      {
        // old style
        var title = this.CurrentForumPage.FindControlRecursiveBothAs<HtmlTitle>("ForumTitle");

        if (title != null)
        {
          title.Text = this._forumPageTitle;
        }
      }
    }

    /// <summary>
    /// Creates this pages title and fires a PageTitleSet event if one is set
    /// </summary>
    private void GeneratePageTitle()
    {
      // compute page title..
      var title = new StringBuilder();

      string pageStr = string.Empty;

      if (this.ForumPageType == ForumPages.posts || this.ForumPageType == ForumPages.topics)
      {
        // get current page...
        var currentPager = (Pager)this.CurrentForumPage.FindControl("Pager");

        if (currentPager != null && currentPager.CurrentPageIndex != 0)
        {
          pageStr = "Page {0} - ".FormatWith(currentPager.CurrentPageIndex + 1);
        }
      }

      if (!this.PageContext.CurrentForumPage.IsAdminPage)
      {
        if (this.PageContext.PageTopicID != 0)
        {
          // Tack on the topic we're viewing
          title.AppendFormat("{0} - ", this.Get<IBadWordReplace>().Replace(this.PageContext.PageTopicName));
        }

        if (this.ForumPageType == ForumPages.posts)
        {
          title.Append(pageStr);
        }

        if (this.PageContext.PageForumName != string.Empty)
        {
          // Tack on the forum we're viewing
          title.AppendFormat("{0} - ", this.CurrentForumPage.HtmlEncode(this.PageContext.PageForumName));
        }

        if (this.ForumPageType == ForumPages.topics)
        {
          title.Append(pageStr);
        }
      }

      title.Append(this.CurrentForumPage.HtmlEncode(this.PageContext.BoardSettings.Name));
        
        // and lastly, tack on the board's name
      this._forumPageTitle = title.ToString();

      this.ForumControl.FirePageTitleSet(this, new ForumPageTitleArgs(this._forumPageTitle));
    }

    #endregion
  }
}