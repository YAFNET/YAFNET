/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjّrnar Henden
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

namespace YAF.Pages
{
  #region Using

  using System;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Utils;
  using YAF.Utilities;

  #endregion

  /// <summary>
  ///   the Albums Page.
  /// </summary>
  public partial class Albums : ForumPage
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the Albums class.
    /// </summary>
    public Albums()
      : base("ALBUM")
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
    protected override void OnPreRender(EventArgs e)
    {
        // setup jQuery, LightBox and YAF JS...
        YafContext.Current.PageElements.RegisterJQuery();
        YafContext.Current.PageElements.RegisterJsResourceInclude("yafjs", "js/yaf.js");
        YafContext.Current.PageElements.RegisterJsBlock("toggleMessageJs", JavaScriptBlocks.ToggleMessageJs);

        // lightbox only need if the browser is not IE6...
        if (!UserAgentHelper.IsBrowserIE6())
        {
            YafContext.Current.PageElements.RegisterJsResourceInclude("lightboxjs", "js/jquery.lightbox.min.js");
            YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.lightbox.css");
            YafContext.Current.PageElements.RegisterJsBlock("lightboxloadjs", JavaScriptBlocks.LightBoxLoadJs);
        }

        base.OnPreRender(e);
    }

    /// <summary>
    ///   The page load event.
    /// </summary>
    /// <param name = "sender">
    ///   the sender.
    /// </param>
    /// <param name = "e">
    ///   the e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!this.PageContext.BoardSettings.EnableAlbum)
      {
        YafBuildLink.AccessDenied();
      }

      if (this.Request.QueryString.GetFirstOrDefault("u") == null)
      {
        YafBuildLink.AccessDenied();
      }

      string displayName =
        UserMembershipHelper.GetDisplayNameFromID(
          Security.StringToLongOrRedirect(this.Request.QueryString.GetFirstOrDefault("u")));
      // Generate the Page Links.

      this.PageLinks.Clear();
      this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
      this.PageLinks.AddLink(
        displayName.IsSet()
          ? displayName
          : UserMembershipHelper.GetUserNameFromID(
            Security.StringToLongOrRedirect(this.Request.QueryString.GetFirstOrDefault("u"))),
        YafBuildLink.GetLink(ForumPages.profile, "u={0}", this.Request.QueryString.GetFirstOrDefault("u")));
      this.PageLinks.AddLink(this.GetText("ALBUMS"), string.Empty);

      // Initialize the Album List control.
      this.AlbumList1.UserID = this.Request.QueryString.GetFirstOrDefault("u").ToType<int>();
    }

    #endregion
  }
}