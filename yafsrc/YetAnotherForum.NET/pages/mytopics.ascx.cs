/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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
namespace YAF.Pages
{
  #region Using

  using System;

  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utilities;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The mytopics.
  /// </summary>
  public partial class mytopics : ForumPage
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the mytopics class.
    /// </summary>
    public mytopics()
      : base("MYTOPICS")
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      // setup jQuery and Jquery Ui Tabs.
      YafContext.Current.PageElements.RegisterJQuery();
      YafContext.Current.PageElements.RegisterJQueryUI();

      YafContext.Current.PageElements.RegisterJsBlock(
        "TopicsTabsJs", JavaScriptBlocks.JqueryUITabsLoadJs(this.TopicsTabs.ClientID, this.hidLastTab.ClientID, false));

      base.OnPreRender(e);
    }

    /// <summary>
    /// The Page_ Load Event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (!this.IsPostBack)
      {
        this.FavoriteTopicsTabTitle.Visible = !this.PageContext.IsGuest;
        this.FavoriteTopicsTabContent.Visible = !this.PageContext.IsGuest;

        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        if (this.PageContext.IsGuest)
        {
          this.PageLinks.AddLink(this.GetText("GUESTTITLE"), string.Empty);
        }
        else
        {
          this.PageLinks.AddLink(this.GetText("MEMBERTITLE"), string.Empty);
        }

        this.ForumJumpHolder.Visible = this.PageContext.BoardSettings.ShowForumJump &&
                                       this.PageContext.Settings.LockedForum == 0;
      }
    }

    #endregion
  }
}