/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjّrnar Henden
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

using YAF.Classes;

namespace YAF.Pages
{
  #region Using

  using System;
  using System.Web;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utilities;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// the album page.
  /// </summary>
  public partial class Album : ForumPage
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the Album class.
    /// </summary>
    public Album()
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
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      // setup jQuery and YAF JS...
      YafContext.Current.PageElements.RegisterJQuery();
      YafContext.Current.PageElements.RegisterJsResourceInclude("yafjs", "js/yaf.js");
      YafContext.Current.PageElements.RegisterJsBlock("toggleMessageJs", JavaScriptBlocks.ToggleMessageJs);

      // ceebox Js
      YafContext.Current.PageElements.RegisterJsResourceInclude("ceeboxjs", "js/jquery.ceebox-min.js");
      YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.ceebox.css");
      YafContext.Current.PageElements.RegisterJsBlock("ceeboxloadjs", JavaScriptBlocks.CeeBoxLoadJs);

      base.OnPreRender(e);
    }

    /// <summary>
    /// The page load event.
    /// </summary>
    /// <param name="sender">
    /// the sender.
    /// </param>
    /// <param name="e">
    /// the e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.Get<YafBoardSettings>().EnableAlbum)
      {
        YafBuildLink.AccessDenied();
      }

      if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u") == null ||
          this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a") == null)
      {
        YafBuildLink.AccessDenied();
      }

      var userId = Security.StringToLongOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));
      var albumId = Security.StringToLongOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a"));

      string displayName = this.Get<IUserDisplayName>().GetName((int)userId);

      // Generate the page links.
      this.PageLinks.Clear();
      this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
      this.PageLinks.AddLink(displayName, YafBuildLink.GetLink(ForumPages.profile, "u={0}", userId));
      this.PageLinks.AddLink(this.GetText("ALBUMS"), YafBuildLink.GetLink(ForumPages.albums, "u={0}", userId));
      this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

      // Set the title text.
      this.LocalizedLabel1.Param0 = this.Get<YafBoardSettings>().EnableDisplayName  
                                      ? this.Server.HtmlEncode(displayName)
                                      : this.Server.HtmlEncode(this.PageContext.User.UserName);
      this.LocalizedLabel1.Param1 = this.Server.HtmlEncode(LegacyDb.album_gettitle(albumId));

      // Initialize the Album Image List control.
      this.AlbumImageList1.UserID = (int)userId;
      this.AlbumImageList1.AlbumID = (int)albumId;
    }

    #endregion
  }
}