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
    using System;
    using System.Web;
    using System.Web.UI.WebControls;
    using AjaxPro;
    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Data;
    using YAF.Classes.Utils;
    using YAF.Utilities;

    /// <summary>
    /// The AlbumList control.
    /// </summary>
    public partial class AlbumList : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        /// the _user id.
        /// </summary>
        private int _userID;

        #endregion

        #region Properties

        /// <summary>
        /// the User ID.
        /// </summary>
        public int UserID
        {
            get
            {
                return this._userID;
            }

            set
            {
                this._userID = value;
            }
        }

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
        protected void AddAlbum_Click(object sender, EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.cp_editalbumimages, "u={0}&a=new", this.UserID);
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
            if (!this.IsPostBack)
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

                string umhdn = UserMembershipHelper.GetDisplayNameFromID(this.UserID);
                this.AlbumHeaderLabel.Param0 = !string.IsNullOrEmpty(umhdn) ? Server.HtmlEncode(umhdn) : Server.HtmlEncode(UserMembershipHelper.GetUserNameFromID(this.UserID));
                
                this.BindData();
                // vzrus: replaced registry check for db data
                System.Data.DataTable sigData = YAF.Classes.Data.DB.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID);
                if (sigData.Rows.Count > 0)
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
                }
               
              
                if (this.AddAlbum.Visible)
                {
                    this.AddAlbum.Text = this.PageContext.Localization.GetText("BUTTON", "BUTTON_ADDALBUM");
                }

                HttpContext.Current.Session["imagePreviewWidth"] =
                    this.PageContext.BoardSettings.ImageAttachmentResizeWidth;
                HttpContext.Current.Session["imagePreviewHeight"] =
                    this.PageContext.BoardSettings.ImageAttachmentResizeHeight;
                HttpContext.Current.Session["localizationFile"] = this.PageContext.Localization.LanguageFileName;
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
        protected void Pager_PageChange(object sender, EventArgs e)
        {
            this.BindData();
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
        protected void Albums_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.cp_editalbumimages, "a={0}", e.CommandArgument);
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
                var pds = new PagedDataSource();
                pds.DataSource = albumListDT.DefaultView;
                pds.AllowPaging = true;
                pds.CurrentPageIndex = this.PagerTop.CurrentPageIndex;
                pds.PageSize = this.PagerTop.PageSize;
                this.Albums.DataSource = pds;
                this.DataBind();
            }
        }

        #endregion
    }
}