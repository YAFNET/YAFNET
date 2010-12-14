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
  using System.Web.UI.WebControls;

  using AjaxPro;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;
  using YAF.Utilities;

  #endregion

  /// <summary>
  /// The AlbumImageList control.
  /// </summary>
  public partial class AlbumImageList : BaseUserControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The _attach group id.
    /// </summary>
    protected string _attachGroupID = string.Empty;

    #endregion

    #region Properties

    /// <summary>
    ///   The Album ID.
    /// </summary>
    public int AlbumID { get; set; }

    /// <summary>
    ///   The User ID.
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    ///   The _cover image id.
    /// </summary>
    private string _coverImageID { get; set; }

    #endregion

    #region Methods

    protected override void OnPreRender(EventArgs e)
    {
        
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
    protected void Page_Load(object sender, EventArgs e)
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
                "ChangeImageCaptionJs", JavaScriptBlocks.ChangeImageCaptionJs);
            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "asynchCallFailedJs", JavaScriptBlocks.asynchCallFailedJs);
            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "AlbumCallbackSuccessJS", JavaScriptBlocks.AlbumCallbackSuccessJS);
            this.ltrTitleOnly.Visible = false;
        }

        this._attachGroupID = Guid.NewGuid().ToString().Substring(0, 5);
        if (this.UserID == this.PageContext.PageUserID)
        {
            this.ltrTitleOnly.Visible = false;

            // Initialize the edit control.
            this.EditAlbums.Visible = true;
            this.EditAlbums.Text = this.PageContext.Localization.GetText("BUTTON", "BUTTON_EDITALBUMIMAGES");
        }

        this.BindData();
    }

    /// <summary>
    /// The ItemCommand method for the cover buttons. Sets/Removes cover image.
    /// </summary>
    /// <param name="sender">
    /// the sender.
    /// </param>
    /// <param name="e">
    /// the e.
    /// </param>
    protected void AlbumImages_ItemCommand(object sender, CommandEventArgs e)
    {
      using (var dt = DB.album_list(null, this.AlbumID))
      {
        if (dt.Rows[0]["CoverImageID"].ToString() == e.CommandArgument.ToString())
        {
          DB.album_save(this.AlbumID, null, null, 0);
        }
        else
        {
          DB.album_save(dt.Rows[0]["AlbumID"], null, null, e.CommandArgument);
        }
      }

      this.BindData();
    }

    /// <summary>
    /// Initialize the scripts for changing images' caption.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void AlbumImages_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (this.UserID != this.PageContext.PageUserID) return;

        var setCover = (Button)e.Item.FindControl("SetCover");

        if (setCover != null)
        {
            // Is this the cover image?
            setCover.Text = setCover.CommandArgument == this._coverImageID
                                ? this.PageContext.Localization.GetText("BUTTON_RESETCOVER")
                                : this.PageContext.Localization.GetText("BUTTON_SETCOVER");
        }
    }

    /// <summary>
    /// Redirect to the edit album page.
    /// </summary>
    /// <param name="sender">
    /// the sender.
    /// </param>
    /// <param name="e">
    /// the e.
    /// </param>
    protected void EditAlbums_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.cp_editalbumimages, "a={0}", this.AlbumID);
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
    protected void Pager_PageChange(object sender, EventArgs e)
    {
      this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.PagerTop.PageSize = YafContext.Current.BoardSettings.AlbumImagesPerPage;
      string albumTitle = DB.album_gettitle(this.AlbumID);

      // if (UserID == PageContext.PageUserID)
      // ltrTitle.Visible = false;
      this.ltrTitleOnly.Text = this.HtmlEncode(albumTitle);
      this.ltrTitle.Text = albumTitle == string.Empty
                             ? this.PageContext.Localization.GetText("ALBUM_CHANGE_TITLE")
                             : this.HtmlEncode(albumTitle);

      // set the Datatable
      var dtAlbumImageList = DB.album_image_list(this.AlbumID, null);
      var dtAlbum = DB.album_list(null, this.AlbumID);

      // Does this album has a cover?
      this._coverImageID = dtAlbum.Rows[0]["CoverImageID"] == DBNull.Value
                             ? string.Empty
                             : dtAlbum.Rows[0]["CoverImageID"].ToString();

      if ((dtAlbumImageList != null) && (dtAlbumImageList.Rows.Count > 0))
      {
        this.PagerTop.Count = dtAlbumImageList.Rows.Count;

        // Create paged data source for the album image list
        var pds = new PagedDataSource();
        pds.DataSource = dtAlbumImageList.DefaultView;
        pds.AllowPaging = true;
        pds.CurrentPageIndex = this.PagerTop.CurrentPageIndex;
        pds.PageSize = this.PagerTop.PageSize;

        this.AlbumImages.DataSource = pds;
        this.DataBind();
      }
    }

    #endregion
  }
}