/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
namespace YAF.Pages
{
    #region Using

    using System;
    using System.IO;
    using System.Threading;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
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
            var path =
              this.Get<HttpRequestBase>().MapPath(
                string.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

            YafAlbum.Album_Image_Delete(
              path, this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a"), this.PageContext.PageUserID, null);

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
                    var path =
                      this.Get<HttpRequestBase>().MapPath(
                        String.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

                    YafAlbum.Album_Image_Delete(path, null, this.PageContext.PageUserID, e.CommandArgument.ToType<int>());

                    // If the user is trying to edit an existing album, initialize the repeater.
                    this.BindVariousControls(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a").Equals("new"));

                    var sigData = LegacyDb.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID);

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

            var sigData = LegacyDb.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID);

            var usrAlbumsAllowed = sigData.GetFirstRowColumnAsValue<int?>("UsrAlbums", null);

            var albumSize = LegacyDb.album_getstats(this.PageContext.PageUserID, null);
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

            var displayName = YafContext.Current.Get<YafBoardSettings>().EnableDisplayName
                                  ? UserMembershipHelper.GetDisplayNameFromID(userID)
                                  : UserMembershipHelper.GetUserNameFromID(userID);

            // Add the page links.
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
               displayName,
                YafBuildLink.GetLink(ForumPages.profile, "u={0}&name={1}", userID.ToString(), displayName));
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
            var albumId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a");
            this.txtTitle.Text = HttpUtility.HtmlEncode(this.txtTitle.Text);

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a") == "new")
            {
                albumId = LegacyDb.album_save(null, this.PageContext.PageUserID, this.txtTitle.Text, null).ToString();
            }
            else
            {
                LegacyDb.album_save(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a"), null, this.txtTitle.Text, null);
            }

            YafBuildLink.Redirect(ForumPages.cp_editalbumimages, "a={0}", albumId);
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

                var sigData = LegacyDb.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID);

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
                if (x.GetType() != typeof(ThreadAbortException))
                {
                    this.Logger.Log(this.PageContext.PageUserID, this, x);
                    this.PageContext.AddLoadMessage(x.Message);
                }
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
                this.List.Visible = albumList.HasRows();
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
            var filePath = uploadedFile.PostedFile.FileName.Trim();

            if (filePath.IsNotSet() || uploadedFile.PostedFile.ContentLength == 0)
            {
                return false;
            }

            if (uploadedFile.PostedFile.ContentType.ToLower().Contains("text"))
            {
                return false;
            }

            var extension = Path.GetExtension(filePath).ToLower();

            // remove the "period"
            extension = extension.Replace(".", string.Empty);
            string[] imageExtensions = { "jpg", "gif", "png", "bmp" };

            // If we don't get a match from the db, then the extension is not allowed
            // also, check to see an image is being uploaded.
            if (Array.IndexOf(imageExtensions, extension) == -1 || this.GetRepository<FileExtension>()
                    .Get(e => e.BoardId == this.PageContext.PageBoardID && e.Extension == extension).Count == 0)
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

            var path =
              this.Get<HttpRequestBase>().MapPath(
                string.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

            // check if Uploads folder exists
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var filename = file.PostedFile.FileName;

            var pos = filename.LastIndexOfAny(new[] { '/', '\\' });
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
            var sigData = LegacyDb.user_getalbumsdata(this.PageContext.PageUserID, YafContext.Current.PageBoardID);

            var usrAlbumsAllowed = sigData.GetFirstRowColumnAsValue<int?>("UsrAlbums", null);
            var usrAlbumImagesAllowed = sigData.GetFirstRowColumnAsValue<int?>("UsrAlbumImages", null);

            // if (!usrAlbums.HasValue || usrAlbums <= 0) return;
            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("a") == "new")
            {
                var alstats = LegacyDb.album_getstats(this.PageContext.PageUserID, null);

                // Albums count. If we reached limit then we exit.
                if (alstats[0] >= usrAlbumsAllowed)
                {
                    this.PageContext.AddLoadMessage(this.GetTextFormatted("ALBUMS_COUNT_LIMIT", usrAlbumImagesAllowed));
                    return;
                }

                var newAlbumId = LegacyDb.album_save(null, this.PageContext.PageUserID, this.txtTitle.Text, null);
                file.PostedFile.SaveAs(
                  "{0}/{1}.{2}.{3}.yafalbum".FormatWith(path, this.PageContext.PageUserID, newAlbumId.ToString(), filename));
                LegacyDb.album_image_save(null, newAlbumId, null, filename, file.PostedFile.ContentLength, file.PostedFile.ContentType);

                // clear the cache for this user to update albums|images stats...
                this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageContext.PageUserID));

                YafBuildLink.Redirect(ForumPages.cp_editalbumimages, "a={0}", newAlbumId);
            }
            else
            {
                // vzrus: the checks here are useless but in a case...
                var alstats = LegacyDb.album_getstats(
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
                    path,
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