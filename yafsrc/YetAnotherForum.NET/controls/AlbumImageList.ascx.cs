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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;

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
        ///   Gets or sets the Album ID.
        /// </summary>
        public int AlbumID { get; set; }

        /// <summary>
        ///   Gets or sets the User ID.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        ///   Gets or sets the _cover image id.
        /// </summary>
        private string _coverImageID { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The ItemCommand method for the cover buttons. Sets/Removes cover image.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        protected void AlbumImages_ItemCommand([NotNull] object sender, [NotNull] CommandEventArgs e)
        {
            using (var dt = LegacyDb.album_list(null, this.AlbumID))
            {
                if (dt.Rows[0]["CoverImageID"].ToString() == e.CommandArgument.ToString())
                {
                    LegacyDb.album_save(this.AlbumID, null, null, 0);
                }
                else
                {
                    LegacyDb.album_save(dt.Rows[0]["AlbumID"], null, null, e.CommandArgument);
                }
            }

            this.BindData();
        }

        /// <summary>
        /// Initialize the scripts for changing images' caption.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void AlbumImages_ItemDataBound([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
        {
            if (this.UserID != this.PageContext.PageUserID)
            {
                return;
            }

            var setCover = (Button)e.Item.FindControl("SetCover");

            if (setCover != null)
            {
                // Is this the cover image?
                setCover.Text = setCover.CommandArgument == this._coverImageID
                                    ? this.GetText("BUTTON_RESETCOVER")
                                    : this.GetText("BUTTON_SETCOVER");
            }
        }

        /// <summary>
        /// Redirect to the edit album page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void EditAlbums_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.cp_editalbumimages, "a={0}", this.AlbumID);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            if (this.UserID == this.PageContext.PageUserID)
            {
                // Register jQuery Ajax Plugin.
                YafContext.Current.PageElements.RegisterJsResourceInclude("yafPageMethodjs", "js/jquery.pagemethod.js");

                // Register Js Blocks.
                YafContext.Current.PageElements.RegisterJsBlockStartup(
                    "AlbumEventsJs",
                    JavaScriptBlocks.AlbumEventsJs(
                        this.GetText("ALBUM_CHANGE_TITLE"), this.GetText("ALBUM_IMAGE_CHANGE_CAPTION")));
                YafContext.Current.PageElements.RegisterJsBlockStartup(
                    "ChangeAlbumTitleJs", JavaScriptBlocks.ChangeAlbumTitleJs);
                YafContext.Current.PageElements.RegisterJsBlockStartup(
                    "ChangeImageCaptionJs", JavaScriptBlocks.ChangeImageCaptionJs);
                YafContext.Current.PageElements.RegisterJsBlockStartup(
                    "asynchCallFailedJs", JavaScriptBlocks.AsynchCallFailedJs);
                YafContext.Current.PageElements.RegisterJsBlockStartup(
                    "AlbumCallbackSuccessJS", JavaScriptBlocks.AlbumCallbackSuccessJS);
                this.ltrTitleOnly.Visible = false;
            }

            base.OnPreRender(e);
        }

        /// <summary>
        /// Called when the page loads
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this._attachGroupID = Guid.NewGuid().ToString().Substring(0, 5);

            if (this.UserID == this.PageContext.PageUserID)
            {
                this.ltrTitleOnly.Visible = false;

                // Initialize the edit control.
                this.EditAlbums.Visible = true;
                this.EditAlbums.Text = this.GetText("BUTTON", "BUTTON_EDITALBUMIMAGES");
            }

            this.BindData();
        }

        /// <summary>
        /// Re-Binds the Data after Page Change
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.PagerTop.PageSize = this.Get<YafBoardSettings>().AlbumImagesPerPage;
            string albumTitle = LegacyDb.album_gettitle(this.AlbumID);

            // if (UserID == PageContext.PageUserID)
            // ltrTitle.Visible = false;
            this.ltrTitleOnly.Text = this.HtmlEncode(albumTitle);
            this.ltrTitle.Text = albumTitle == string.Empty
                                     ? this.GetText("ALBUM_CHANGE_TITLE")
                                     : this.HtmlEncode(albumTitle);

            // set the Datatable
            var dtAlbumImageList = LegacyDb.album_image_list(this.AlbumID, null);
            var dtAlbum = LegacyDb.album_list(null, this.AlbumID);

            // Does this album has a cover?
            this._coverImageID = dtAlbum.Rows[0]["CoverImageID"] == DBNull.Value
                                     ? string.Empty
                                     : dtAlbum.Rows[0]["CoverImageID"].ToString();

            if ((dtAlbumImageList == null) || (dtAlbumImageList.Rows.Count <= 0))
            {
                return;
            }

            this.PagerTop.Count = dtAlbumImageList.Rows.Count;

            // Create paged data source for the album image list
            var pds = new PagedDataSource
                          {
                              DataSource = dtAlbumImageList.DefaultView,
                              AllowPaging = true,
                              CurrentPageIndex = this.PagerTop.CurrentPageIndex,
                              PageSize = this.PagerTop.PageSize
                          };

            this.AlbumImages.DataSource = pds;
            this.DataBind();
        }

        #endregion
    }
}