/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Types.Objects;

    #endregion

    /// <summary>
    /// The edit users avatar.
    /// </summary>
    public partial class EditUsersAvatar : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The current user id.
        /// </summary>
        private int currentUserId;

        #endregion

        #region Methods

        /// <summary>
        /// Cancel Editing
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Back_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Get<LinkBuilder>().Redirect(
                this.PageContext.CurrentForumPage.IsAdminPage ? ForumPages.Admin_Users : ForumPages.MyAccount);
        }

        /// <summary>
        /// Delete The Current Avatar
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteAvatar_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.GetRepository<User>().DeleteAvatar(this.currentUserId);

            // clear the cache for this user...
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.currentUserId));
            this.BindData();
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.PageContext.CurrentForumPage.IsAdminPage && this.PageContext.IsAdmin
                                                              && this.Get<HttpRequestBase>().QueryString.Exists("u"))
            {
                this.currentUserId = this.Get<LinkBuilder>().StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));
            }
            else
            {
                this.currentUserId = this.PageContext.PageUserID;
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.NoAvatar.Text = this.GetText("EDIT_AVATAR", "NOAVATAR");

            this.noteLocal.Text = this.GetTextFormatted(
                "NOTE_LOCAL",
                this.PageContext.BoardSettings.AvatarWidth.ToString(),
                this.PageContext.BoardSettings.AvatarHeight,
                (this.PageContext.BoardSettings.AvatarSize / 1024).ToString());

            this.BindData();
        }

        /// <summary>
        /// Saves the Gallery Avatar
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void GalleryUpdateClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.AvatarGallery.SelectedIndex <= 0)
            {
                return;
            }

            // save the avatar right now...
            this.GetRepository<User>().SaveAvatar(
                this.currentUserId,
                this.AvatarGallery.SelectedValue,
                null,
                null);

            // clear the cache for this user...
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.currentUserId));

            this.BindData();
        }

        /// <summary>
        /// Saves the local Avatar
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UploadUpdate_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.File.PostedFile == null || this.File.PostedFile.FileName.Trim().Length <= 0
                                             || this.File.PostedFile.ContentLength <= 0 || !this.File.PostedFile.FileName.IsImageName())
            {
                return;
            }

            

            long x = this.PageContext.BoardSettings.AvatarWidth;
            long y = this.PageContext.BoardSettings.AvatarHeight;
            var avatarSize = this.PageContext.BoardSettings.AvatarSize;

            Stream resized = null;

            try
            {
                using (var img = Image.FromStream(this.File.PostedFile.InputStream))
                {
                    if (img.Width > x || img.Height > y)
                    {
                        this.PageContext.AddLoadMessage(
                            $"{this.GetTextFormatted("WARN_TOOBIG", x, y)} {this.GetTextFormatted("WARN_SIZE", img.Width, img.Height)} {this.GetText("EDIT_AVATAR", "WARN_RESIZED")}",
                            MessageTypes.warning);

                        resized = ImageHelper.GetResizedImageStreamFromImage(img, x, y);
                    }
                }

                // Delete old first...
                this.GetRepository<User>().DeleteAvatar(this.currentUserId);

                if (this.PageContext.BoardSettings.UseFileTable)
                {
                    var image = Image.FromStream(resized ?? this.File.PostedFile.InputStream);

                    var memoryStream = new MemoryStream();
                    image.Save(memoryStream, image.RawFormat);
                    memoryStream.Position = 0;

                    this.GetRepository<User>().SaveAvatar(
                        this.currentUserId,
                        null,
                        memoryStream,
                        this.File.PostedFile.ContentType);
                }
                else
                {
                    var uploadFolderPath = this.Get<HttpRequestBase>().MapPath(
                        string.Concat(BaseUrlBuilder.ServerFileRoot, this.Get<BoardFolders>().Uploads));

                    // check if Uploads folder exists
                    if (!Directory.Exists(uploadFolderPath))
                    {
                        Directory.CreateDirectory(uploadFolderPath);
                    }

                    var fileName = this.File.PostedFile.FileName;

                    var pos = fileName.LastIndexOfAny(new[] { '/', '\\' });

                    if (pos >= 0)
                    {
                        fileName = fileName.Substring(pos + 1);
                    }

                    // filename can be only 255 characters long (due to table column)
                    if (fileName.Length > 255)
                    {
                        fileName = fileName.Substring(fileName.Length - 255);
                    }

                    var newFileName = $"{this.currentUserId}{Path.GetExtension(fileName)}";

                    var filePath = Path.Combine(uploadFolderPath, newFileName);

                    // Delete old avatar
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    var avatarImage = Image.FromStream(resized ?? this.File.PostedFile.InputStream);

                    using (var memory = new MemoryStream())
                    {
                        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
                        avatarImage.Save(memory, avatarImage.RawFormat);
                        var bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }

                    this.GetRepository<User>().SaveAvatar(
                        this.currentUserId,
                        $"{BoardInfo.ForumServerFileRoot}{this.Get<BoardFolders>().Uploads}/{newFileName}",
                        null,
                        null);
                }

                // clear the cache for this user...
                this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.currentUserId));

                if (avatarSize > 0 && this.File.PostedFile.ContentLength >= avatarSize && resized == null)
                {
                    this.PageContext.AddLoadMessage(
                        $"{this.GetTextFormatted("WARN_BIGFILE", avatarSize)} {this.GetTextFormatted("WARN_FILESIZE", this.File.PostedFile.ContentLength)}",
                        MessageTypes.warning);
                }

                this.BindData();
            }
            catch (Exception exception)
            {
                this.Logger.Log(
                    exception.Message,
                    EventLogTypes.Error,
                    this.PageContext.PageUserID,
                    string.Empty,
                    exception);

                // image is probably invalid...
                this.PageContext.AddLoadMessage(this.GetText("EDIT_AVATAR", "INVALID_FILE"), MessageTypes.danger);
            }
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            var user = this.PageContext.CurrentForumPage.IsAdminPage
                ? this.GetRepository<User>().GetById(this.currentUserId)
                : this.PageContext.User;

            this.DeleteAvatar.Visible = false;
            this.NoAvatar.Visible = false;

            this.AvatarUploadRow.Visible = this.PageContext.CurrentForumPage.IsAdminPage
                                           || this.PageContext.BoardSettings.AvatarUpload;
            this.AvatarOurs.Visible = this.PageContext.CurrentForumPage.IsAdminPage
                                      || this.PageContext.BoardSettings.AvatarGallery;

            if (this.AvatarOurs.Visible)
            {
                var avatars = new List<NamedParameter>
                {
                    new(this.GetText("OURAVATAR"), "")
                };

                //this.GetText("OURAVATAR")

                var dir = new DirectoryInfo(
                    this.Get<HttpRequestBase>()
                        .MapPath($"{BoardInfo.ForumServerFileRoot}{this.Get<BoardFolders>().Avatars}"));

                var files = dir.GetFiles("*.*").ToList();

                avatars.AddImageFiles(files, this.Get<BoardFolders>().Avatars);

                if (avatars.Any())
                {
                    this.AvatarGallery.PlaceHolder = this.GetText("OURAVATAR");

                    this.AvatarGallery.DataSource = avatars;
                    this.AvatarGallery.DataValueField = "Value";
                    this.AvatarGallery.DataTextField = "Name";
                    this.AvatarGallery.DataBind();
                }
                else
                {
                    this.AvatarOurs.Visible = false;
                }
            }

            var showNoAvatar = false;

            if (!user.AvatarImage.IsNullOrEmptyField())
            {
                this.AvatarImg.ImageUrl =
                    $"{BoardInfo.ForumClientFileRoot}resource.ashx?u={this.currentUserId}&v={DateTime.Now.Ticks}";
                this.DeleteAvatar.Visible = true;
            }
            else if (user.Avatar.IsSet() && user.Avatar.StartsWith("/"))
            {
                if (user.Avatar.Contains(this.Get<BoardFolders>().Avatars))
                {
                    var item = this.AvatarGallery.Items.FindByValue(user.Avatar);

                    if (item == null)
                    {
                        showNoAvatar = true;
                    }

                    this.AvatarImg.ImageUrl = user.Avatar;

                    item.Selected = true;

                    this.DeleteAvatar.Visible = true;
                }
                else
                {
                    this.AvatarImg.ImageUrl = user.Avatar;
                    this.DeleteAvatar.Visible = true;
                }
            }
            else
            {
                showNoAvatar = true;
            }

            if (showNoAvatar)
            {
                this.AvatarImg.ImageUrl =
                    $"{BoardInfo.ForumClientFileRoot}resource.ashx?avatar={this.currentUserId}&v={DateTime.Now.Ticks}";
                this.NoAvatar.Visible = true;
            }

            this.AvatarImg.Attributes.CssStyle.Add("max-width", this.PageContext.BoardSettings.AvatarWidth.ToString());
            this.AvatarImg.Attributes.CssStyle.Add("max-height", this.PageContext.BoardSettings.AvatarHeight.ToString());
        }

        #endregion
    }
}