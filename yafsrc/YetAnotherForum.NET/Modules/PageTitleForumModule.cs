/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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