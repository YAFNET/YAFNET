﻿/* Yet Another Forum.NET
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
namespace YAF.Pages
{
    #region Using

    using System;
    using System.Data;
    using System.IO;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Edit User Album Images Page.
    /// </summary>
    public partial class cp_editalbumimages : ForumPageRegistered
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="cp_editalbumimages"/> class.
        /// </summary>
        public cp_editalbumimages()
            : base("CP_EDITALBUMIMAGES")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The back button click event handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Back_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.List.Items.Count > 0)
            {
                var albid = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a");

                // This is an existing album
                if (albid.IsSet() && !albid.Contains("new"))
                {
                    YafBuildLink.Redirect(ForumPages.album, "u={0}&a={1}", this.PageContext.PageUserID.ToString(), albid);
                }
                else
                {
                    // simply redirect to albums page
                    YafBuildLink.Redirect(ForumPages.albums, "u={0}", this.PageContext.PageUserID);
                }
            }
            else
            {
                YafBuildLink.Redirect(ForumPages.albums, "u={0}", this.PageContext.PageUserID);
            }
        }

        /// <summary>
        /// Deletes the album and all the images in it.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteAlbum_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            string sUpDir =
              this.Get<HttpRequestBase>().MapPath(
                string.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

            YafAlbum.Album_Image_Delete(
              sUpDir, this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a"), this.PageContext.PageUserID, null);

            // clear the cache for this user to update albums|images stats...
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageContext.PageUserID));

            YafBuildLink.Redirect(ForumPages.albums, "u={0}", this.PageContext.PageUserID);
        }

        /// <summary>
        /// The btn delete_ load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteAlbum_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((Button)sender).Attributes["onclick"] = "return confirm(\'{0}\')".FormatWith(this.GetText("ASK_DELETEALBUM"));
        }

        /// <summary>
        /// The Upload file delete confirmation dialog.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ImageDelete_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((LinkButton)sender).Attributes["onclick"] = "return confirm('{0}')".FormatWith(this.GetText("ASK_DELETEIMAGE"));
        }

        /// <summary>
        /// The repater Item command event responsible for handling deletion of uploaded files.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void List_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "delete":
                    string sUpDir =
                      this.Get<HttpRequestBase>().MapPath(
                        String.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

                    YafAlbum.Album_Image_Delete(sUpDir, null, this.PageContext.PageUserID, e.CommandArgument.ToType<int>());

                    // If the user is trying to edit an existing album, initialize the repeater.
                    this.BindVariousControls(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a").Equals("new"));

                    DataTable sigData = LegacyDb.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID);

                    //int[] albumSize = LegacyDb.album_getstats(this.PageContext.PageUserID, null);

                    var usrAlbumImagesAllowed = sigData.GetFirstRowColumnAsValue<int?>("UsrAlbumImages", null);

                    // Has the user uploaded maximum number of images?   
                    // vzrus: changed for DB check The default number of album images is 0. In the case albums are disabled.
                    if (usrAlbumImagesAllowed.HasValue && usrAlbumImagesAllowed > 0)
                    {
                        if (this.List.Items.Count >= usrAlbumImagesAllowed)
                        {
                            this.uploadtitletr.Visible = false;
                            this.selectfiletr.Visible = false;
                        }
                        else
                        {
                            this.uploadtitletr.Visible = true;
                            this.selectfiletr.Visible = true;
                        }

                        this.imagesInfo.Text = this.GetTextFormatted(
                          "IMAGES_INFO", this.List.Items.Count, usrAlbumImagesAllowed, this.Get<YafBoardSettings>().AlbumImagesSizeMax / 1024);
                    }
                    else
                    {
                        this.uploadtitletr.Visible = false;
                        this.selectfiletr.Visible = false;
                    }

                    break;
            }
        }

        /// <summary>
        /// the page load event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Get<YafBoardSettings>().EnableAlbum)
            {
                YafBuildLink.AccessDenied();
            }

            if (this.IsPostBack)
            {
                return;
            }

            DataTable sigData = LegacyDb.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID);

            var usrAlbumsAllowed = sigData.GetFirstRowColumnAsValue<int?>("UsrAlbums", null);

            int[] albumSize = LegacyDb.album_getstats(this.PageContext.PageUserID, null);
            int userID;
            switch (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a"))
            {
                // A new album is being created. check the permissions.
                case "new":

                    // Is album feature enabled?
                    if (!this.Get<YafBoardSettings>().EnableAlbum)
                    {
                        YafBuildLink.AccessDenied();
                    }

                    // Has the user created maximum number of albums?
                    if (usrAlbumsAllowed.HasValue && usrAlbumsAllowed > 0)
                    {
                        // Albums count. If we reached limit then we go to info page.
                        if (usrAlbumsAllowed > 0 && (albumSize[0] >= usrAlbumsAllowed))
                        {
                            YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
                        }
                    }

                    /* if (this.Get<YafBoardSettings>().AlbumsMax > 0 &&
                                        albumSize[0] > this.Get<YafBoardSettings>().AlbumsMax - 1)
                              {
                                  YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
                              }*/
                    userID = this.PageContext.PageUserID;
                    break;
                default:
                    userID =
                        LegacyDb.album_list(
                            null, Security.StringToLongOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a")))
                            .Rows[0]["UserID"].ToType<int>();

                    if (userID != this.PageContext.PageUserID)
                    {
                        YafBuildLink.AccessDenied();
                    }

                    break;
            }

            string displayName = UserMembershipHelper.GetDisplayNameFromID(userID);

            // Add the page links.
            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(
               YafContext.Current.Get<YafBoardSettings>().EnableDisplayName  
                             ? displayName : UserMembershipHelper.GetUserNameFromID(userID),
                YafBuildLink.GetLink(ForumPages.profile, "u={0}", userID.ToString()));
            this.PageLinks.AddLink(
                this.GetText("ALBUMS"), YafBuildLink.GetLink(ForumPages.albums, "u={0}", userID.ToString()));
            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

            this.Back.Text = this.GetText("BACK");
            this.Upload.Text = this.GetText("UPLOAD");

            this.BindData();

            var usrAlbumImagesAllowed = sigData.GetFirstRowColumnAsValue<int?>("UsrAlbumImages", null);

            // Has the user uploaded maximum number of images?   
            // vzrus: changed for DB check The default number of album images is 0. In the case albums are disabled.
            if (usrAlbumImagesAllowed.HasValue && usrAlbumImagesAllowed > 0)
            {
                if (this.List.Items.Count >= usrAlbumImagesAllowed)
                {
                    this.uploadtitletr.Visible = false;
                    this.selectfiletr.Visible = false;
                }
                else
                {
                    this.uploadtitletr.Visible = true;
                    this.selectfiletr.Visible = true;
                }

                this.imagesInfo.Text = this.GetTextFormatted(
                    "IMAGES_INFO", this.List.Items.Count, usrAlbumImagesAllowed, this.Get<YafBoardSettings>().AlbumImagesSizeMax / 1024);
            }
            else
            {
                this.uploadtitletr.Visible = false;
                this.selectfiletr.Visible = false;
            }
        }

        /// <summary>
        /// Update the album title.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UpdateTitle_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            string albumID = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a");
            this.txtTitle.Text = HttpUtility.HtmlEncode(this.txtTitle.Text);

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a") == "new")
            {
                albumID = LegacyDb.album_save(null, this.PageContext.PageUserID, this.txtTitle.Text, null).ToString();
            }
            else
            {
                LegacyDb.album_save(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a"), null, this.txtTitle.Text, null);
            }

            YafBuildLink.Redirect(ForumPages.cp_editalbumimages, "a={0}", albumID);
        }

        /// <summary>
        /// The Upload button click event handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Upload_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            try
            {
                if (this.CheckValidFile(this.File))
                {
                    this.SaveAttachment(this.File);
                }

                this.BindData();

                DataTable sigData = LegacyDb.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID);

                // int[] albumSize = LegacyDb.album_getstats(this.PageContext.PageUserID, null);
                var usrAlbumImagesAllowed = sigData.GetFirstRowColumnAsValue<int?>("UsrAlbumImages", null);

                // Has the user uploaded maximum number of images?   
                // vzrus: changed for DB check The default number of album images is 0. In the case albums are disabled.
                if (usrAlbumImagesAllowed.HasValue && usrAlbumImagesAllowed > 0)
                {
                    if (this.List.Items.Count >= usrAlbumImagesAllowed)
                    {
                        this.uploadtitletr.Visible = false;
                        this.selectfiletr.Visible = false;
                    }
                    else
                    {
                        this.uploadtitletr.Visible = true;
                        this.selectfiletr.Visible = true;
                    }

                    this.imagesInfo.Text = this.GetTextFormatted(
                      "IMAGES_INFO", this.List.Items.Count, usrAlbumImagesAllowed, this.Get<YafBoardSettings>().AlbumImagesSizeMax / 1024);
                }
                else
                {
                    this.uploadtitletr.Visible = false;
                    this.selectfiletr.Visible = false;
                }
            }
            catch (Exception x)
            {
                this.Logger.Log(this.PageContext.PageUserID, this, x);
                this.PageContext.AddLoadMessage(x.Message);
            }
        }

        /// <summary>
        /// Initializes the repeater control and the visibilities of form elements.
        /// </summary>
        private void BindData()
        {
            // If the user is trying to edit an existing album, initialize the repeater.
            this.BindVariousControls(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a").Equals("new"));
        }

        /// <summary>
        /// Binds the repeater and title controls and the visibilities of form elements.
        /// </summary>
        /// <param name="isNewAlbum">if set to <c>true</c> [is new album].</param>
        private void BindVariousControls(bool isNewAlbum)
        {
            this.Delete.Visible = !isNewAlbum;

            if (!isNewAlbum)
            {
                this.txtTitle.Text = LegacyDb.album_gettitle(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a"));

                var albumList = LegacyDb.album_image_list(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a"), null);
                this.List.DataSource = albumList;
                this.List.Visible = albumList.Rows.Count > 0;
            }

            this.DataBind();
        }

        /// <summary>
        /// Check to see if the user is trying to upload a valid file.
        /// </summary>
        /// <param name="uploadedFile">
        /// the uploaded file.
        /// </param>
        /// <returns>
        /// true if file is valid for uploading. otherwise false.
        /// </returns>
        private bool CheckValidFile([NotNull] HtmlInputFile uploadedFile)
        {
            string filePath = uploadedFile.PostedFile.FileName.Trim();

            if (filePath.IsNotSet() || uploadedFile.PostedFile.ContentLength == 0)
            {
                return false;
            }

            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            // remove the "period"
            extension = extension.Replace(".", string.Empty);
            string[] aImageExtensions = { "jpg", "gif", "png", "bmp" };

            // If we don't get a match from the db, then the extension is not allowed
            DataTable dt = this.GetRepository<FileExtension>().List(extension);

            // also, check to see an image is being uploaded.
            if (Array.IndexOf(aImageExtensions, extension) == -1 || dt.Rows.Count == 0)
            {
                this.PageContext.AddLoadMessage(this.GetTextFormatted("FILEERROR", extension));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Save the attached file both physically and in the db.
        /// </summary>
        /// <param name="file">the file.</param>
        /// <exception cref="Exception">Album Image File is too big</exception>
        private void SaveAttachment([NotNull] HtmlInputFile file)
        {
            if (file.PostedFile == null || file.PostedFile.FileName.Trim().Length == 0 || file.PostedFile.ContentLength == 0)
            {
                return;
            }

            string sUpDir =
              this.Get<HttpRequestBase>().MapPath(
                string.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

            // check if Uploads folder exists
            if (!Directory.Exists(sUpDir))
            {
                Directory.CreateDirectory(sUpDir);
            }

            string filename = file.PostedFile.FileName;

            int pos = filename.LastIndexOfAny(new[] { '/', '\\' });
            if (pos >= 0)
            {
                filename = filename.Substring(pos + 1);
            }

            // filename can be only 255 characters long (due to table column)
            if (filename.Length > 255)
            {
                filename = filename.Substring(filename.Length - 255);
            }

            // verify the size of the attachment
            if (this.Get<YafBoardSettings>().AlbumImagesSizeMax > 0 &&
                file.PostedFile.ContentLength > this.Get<YafBoardSettings>().AlbumImagesSizeMax)
            {
                throw new Exception(this.GetText("ERROR_TOOBIG"));
            }

            // vzrus: the checks here are useless but in a case...
            DataTable sigData = LegacyDb.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID);

            var usrAlbumsAllowed = sigData.GetFirstRowColumnAsValue<int?>("UsrAlbums", null);
            var usrAlbumImagesAllowed = sigData.GetFirstRowColumnAsValue<int?>("UsrAlbumImages", null);

            // if (!usrAlbums.HasValue || usrAlbums <= 0) return;
            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a") == "new")
            {
                int[] alstats = LegacyDb.album_getstats(this.PageContext.PageUserID, null);

                // Albums count. If we reached limit then we exit.
                if (alstats[0] >= usrAlbumsAllowed)
                {
                    this.PageContext.AddLoadMessage(this.GetTextFormatted("ALBUMS_COUNT_LIMIT", usrAlbumImagesAllowed));
                    return;
                }

                var newAlbumId = LegacyDb.album_save(null, this.PageContext.PageUserID, this.txtTitle.Text, null);
                file.PostedFile.SaveAs(
                  "{0}/{1}.{2}.{3}.yafalbum".FormatWith(sUpDir, this.PageContext.PageUserID, newAlbumId.ToString(), filename));
                LegacyDb.album_image_save(null, newAlbumId, null, filename, file.PostedFile.ContentLength, file.PostedFile.ContentType);

                // clear the cache for this user to update albums|images stats...
                this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageContext.PageUserID));

                YafBuildLink.Redirect(ForumPages.cp_editalbumimages, "a={0}", newAlbumId);
            }
            else
            {
                // vzrus: the checks here are useless but in a case...
                int[] alstats = LegacyDb.album_getstats(
                  this.PageContext.PageUserID, this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a"));

                /*
                    // Albums count. If we reached limit then we exit. 
                    // Check it first as user could be in other group or prev YAF version was used;
                    if (DB.album_getstats(this.PageContext.PageUserID, null)[0] >= usrAlbums)
                    {
                        this.PageContext.AddLoadMessage(this.GetTextFormatted("ALBUMS_COUNT_LIMIT", usrAlbums));
                       return;
                    }*/

                // Images count. If we reached limit then we exit.
                if (alstats[1] >= usrAlbumImagesAllowed)
                {
                    this.PageContext.AddLoadMessage(this.GetTextFormatted("IMAGES_COUNT_LIMIT", usrAlbumImagesAllowed));
                    return;
                }

                file.PostedFile.SaveAs(
                  "{0}/{1}.{2}.{3}.yafalbum".FormatWith(
                    sUpDir,
                    this.PageContext.PageUserID,
                    this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a"),
                    filename));
                LegacyDb.album_image_save(
                  null,
                  this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a"),
                  null,
                  filename,
                  file.PostedFile.ContentLength,
                  file.PostedFile.ContentType);
            }
        }

        #endregion
    }
}