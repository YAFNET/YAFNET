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

namespace YAF.Controls
{
  #region Using

  using System;
  using System.Web;
  using System.Web.UI.WebControls;

  using AjaxPro;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;
  using YAF.Utilities;

  #endregion

  /// <summary>
  /// The AlbumList control.
  /// </summary>
  public partial class AlbumList : BaseUserControl
  {
    #region Properties

    /// <summary>
    ///   the User ID.
    /// </summary>
    public int UserID { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// redirects to the add new album page.
    /// </summary>
    /// <param name="sender">
    /// the sender.
    /// </param>
    /// <param name="e">
    /// the e.
    /// </param>
    protected void AddAlbum_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.cp_editalbumimages, "u={0}&a=new", this.UserID);
    }

    /// <summary>
    /// The item command method for albums repeater. Redirects to the album page.
    /// </summary>
    /// <param name="source">
    /// the source.
    /// </param>
    /// <param name="e">
    /// the e.
    /// </param>
    protected void Albums_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.cp_editalbumimages, "a={0}", e.CommandArgument);
    }

    /// <summary>
    /// Show a Random Cover if none is Set
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Albums_ItemDataBound([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
    {
      // tha_watcha: TODO: Currently disabled this funtion, until yaf 2.0 build
      /*var coverImage = (Image)e.Item.FindControl("coverImage");

            if (coverImage == null) return;

            var curAlbum = DB.album_image_list(coverImage.AlternateText, null);

            Random random = new Random();

            if ((curAlbum != null) && (curAlbum.Rows.Count > 0))
            {
                coverImage.ImageUrl = String.Format("{0}resource.ashx?imgprv={1}", YafForumInfo.ForumClientFileRoot, curAlbum.Rows[random.Next(curAlbum.Rows.Count)]["ImageID"]);
            }*/
    }

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      if (this.UserID == this.PageContext.PageUserID)
      {
        // Register AjaxPro.
        Utility.RegisterTypeForAjax(typeof(YafAlbum));

        // Register Js Blocks.
        YafContext.Current.PageElements.RegisterJsBlockStartup(
          "AlbumEventsJs", 
          JavaScriptBlocks.AlbumEventsJs(
            this.PageContext.Localization.GetText("ALBUM_CHANGE_TITLE"), 
            this.PageContext.Localization.GetText("ALBUM_IMAGE_CHANGE_CAPTION")));
        YafContext.Current.PageElements.RegisterJsBlockStartup(
          "ChangeAlbumTitleJs", JavaScriptBlocks.ChangeAlbumTitleJs);
        YafContext.Current.PageElements.RegisterJsBlockStartup(
          "asynchCallFailedJs", JavaScriptBlocks.asynchCallFailedJs);
        YafContext.Current.PageElements.RegisterJsBlockStartup(
          "AlbumCallbackSuccessJS", JavaScriptBlocks.AlbumCallbackSuccessJS);
      }

      base.OnPreRender(e);
    }

    /// <summary>
    /// Called when the page loads
    /// </summary>
    /// <param name="sender">
    /// the sender.
    /// </param>
    /// <param name="e">
    /// the e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (!this.IsPostBack)
      {
        string umhdn = UserMembershipHelper.GetDisplayNameFromID(this.UserID);
        this.AlbumHeaderLabel.Param0 = !string.IsNullOrEmpty(umhdn)
                                         ? this.HtmlEncode(umhdn)
                                         : this.HtmlEncode(UserMembershipHelper.GetUserNameFromID(this.UserID));

        this.BindData();

        // vzrus: replaced registry check for db data
        // System.Data.DataTable sigData = DB.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID);
        var usrAlbums =
          DB.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID).GetFirstRowColumnAsValue
            <int?>("UsrAlbums", null);

        if (usrAlbums.HasValue && usrAlbums > 0)
        {
          // this.AddAlbum.Visible = true;
          this.AddAlbum.Visible = (DB.album_getstats(this.PageContext.PageUserID, null)[0] < usrAlbums &&
                                   this.UserID == this.PageContext.PageUserID)
                                    ? true
                                    : false;
        }

        /*if (sigData.Rows.Count > 0)
                {   
                        this.AddAlbum.Visible = (DB.album_getstats(this.PageContext.PageUserID, null)[0] <
                                          Convert.ToInt32(sigData.Rows[0]["UsrAlbums"]) &&
                                          this.UserID == this.PageContext.PageUserID)
                                             ? true
                                             : false;

                        /* this.AddAlbum.Visible = (DB.album_getstats(this.PageContext.PageUserID, null)[0] <
                                     this.PageContext.BoardSettings.AlbumsMax &&
                                     this.UserID == this.PageContext.PageUserID)
                                        ? true
                                        : false; */
        // }*/
        if (this.AddAlbum.Visible)
        {
          this.AddAlbum.Text = this.PageContext.Localization.GetText("BUTTON", "BUTTON_ADDALBUM");
        }

        HttpContext.Current.Session["imagePreviewWidth"] = this.PageContext.BoardSettings.ImageAttachmentResizeWidth;
        HttpContext.Current.Session["imagePreviewHeight"] = this.PageContext.BoardSettings.ImageAttachmentResizeHeight;
        HttpContext.Current.Session["imagePreviewCropped"] = this.PageContext.BoardSettings.ImageAttachmentResizeCropped;
        HttpContext.Current.Session["localizationFile"] = this.PageContext.Localization.LanguageFileName;

        // Show Albums Max Info
        if (this.UserID == this.PageContext.PageUserID)
        {
          if (usrAlbums.HasValue && usrAlbums > 0)
          {
            this.albumsInfo.Text = this.PageContext.Localization.GetTextFormatted(
              "ALBUMS_INFO", this.Albums.Items.Count, usrAlbums);
          }
          else if (usrAlbums.HasValue && usrAlbums.Equals(0) || !usrAlbums.HasValue)
          {
            this.albumsInfo.Text = this.PageContext.Localization.GetText("ALBUMS_NOTALLOWED");
          }

          this.albumsInfo.Visible = true;
        }
        else
        {
          this.albumsInfo.Visible = false;
        }
      }
    }

    /// <summary>
    /// The pager_ page change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.PagerTop.PageSize = YafContext.Current.BoardSettings.AlbumsPerPage;

      // set the Datatable
      var albumListDT = DB.album_list(this.UserID, null);

      if ((albumListDT != null) && (albumListDT.Rows.Count > 0))
      {
        this.PagerTop.Count = albumListDT.Rows.Count;

        // create paged data source for the albumlist
        var pds = new PagedDataSource
          {
            DataSource = albumListDT.DefaultView, 
            AllowPaging = true, 
            CurrentPageIndex = this.PagerTop.CurrentPageIndex, 
            PageSize = this.PagerTop.PageSize
          };

        this.Albums.DataSource = pds;
        this.DataBind();
      }
    }

    #endregion
  }
}