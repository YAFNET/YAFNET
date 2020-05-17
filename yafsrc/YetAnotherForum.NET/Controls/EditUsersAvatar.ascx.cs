/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Utils.Helpers.ImageUtils;

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
            BuildLink.Redirect(
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
            this.PageContext.QueryIDs = new QueryStringIDHelper("u");

            if (this.PageContext.CurrentForumPage.IsAdminPage && this.PageContext.IsAdmin
                                                              && this.PageContext.QueryIDs.ContainsKey("u"))
            {
                this.currentUserId = this.PageContext.QueryIDs["u"].ToType<int>();
            }
            else
            {
                this.currentUserId = this.PageContext.PageUserID;
            }

            if (this.IsPostBack)
            {
                return;
            }

            // check if it's a link from the avatar picker
            if (this.Get<HttpRequestBase>().QueryString.Exists("av"))
            {
                // save the avatar right now...
                this.GetRepository<User>().SaveAvatar(
                    this.currentUserId,
                    $"{BaseUrlBuilder.BaseUrl}{this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("av")}",
                    null,
                    null);

                // clear the cache for this user...
                this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.currentUserId));
            }

            this.NoAvatar.Text = this.GetText("EDIT_AVATAR", "NOAVATAR");

            var addAdminParam = string.Empty;
            if (this.PageContext.CurrentForumPage.IsAdminPage)
            {
                addAdminParam = $"u={this.currentUserId}";
            }

            this.OurAvatar.NavigateUrl = BuildLink.GetLinkNotEscaped(ForumPages.Profile_Avatar, addAdminParam);

            this.noteRemote.Text = this.GetTextFormatted(
                "NOTE_REMOTE",
                this.Get<BoardSettings>().AvatarWidth.ToString(),
                this.Get<BoardSettings>().AvatarHeight.ToString());
            this.noteLocal.Text = this.GetTextFormatted(
                "NOTE_LOCAL",
                this.Get<BoardSettings>().AvatarWidth.ToString(),
                this.Get<BoardSettings>().AvatarHeight,
                (this.Get<BoardSettings>().AvatarSize / 1024).ToString());

            this.BindData();
        }

        /// <summary>
        /// Saves the Remote Avatar
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void RemoteUpdate_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.Avatar.Text.Length > 0 && !this.Avatar.Text.StartsWith("http://")
                                            && !this.Avatar.Text.StartsWith("https://"))
            {
                this.Avatar.Text = $"http://{this.Avatar.Text}";
            }

            // update
            this.GetRepository<User>().SaveAvatar(this.currentUserId, this.Avatar.Text.Trim(), null, null);

            // clear the cache for this user...
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.currentUserId));

            // clear the URL out...
            this.Avatar.Text = string.Empty;

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
                                             || this.File.PostedFile.ContentLength <= 0)
            {
                return;
            }

            long x = this.Get<BoardSettings>().AvatarWidth;
            long y = this.Get<BoardSettings>().AvatarHeight;
            var avatarSize = this.Get<BoardSettings>().AvatarSize;

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

                if (this.Get<BoardSettings>().UseFileTable)
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
                        string.Concat(BaseUrlBuilder.ServerFileRoot, BoardFolders.Current.Uploads));

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
                        using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                        {
                            avatarImage.Save(memory, avatarImage.RawFormat);
                            var bytes = memory.ToArray();
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    }

                    this.GetRepository<User>().SaveAvatar(
                        this.currentUserId,
                        $"{BoardInfo.ForumBaseUrl}{BoardFolders.Current.Uploads}/{newFileName}",
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
                    this.PageContext.CurrentUserData.UserName,
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
            DataRow row;

            using (var dt = this.GetRepository<User>().ListAsDataTable(
                this.PageContext.PageBoardID,
                this.currentUserId,
                null))
            {
                row = dt.GetFirstRow();
            }

            this.AvatarImg.Visible = true;
            this.Avatar.Text = string.Empty;
            this.DeleteAvatar.Visible = false;
            this.NoAvatar.Visible = false;

            if (row["HasAvatarImage"] != null && long.Parse(row["HasAvatarImage"].ToString()) > 0)
            {
                this.AvatarImg.ImageUrl =
                    $"{BoardInfo.ForumClientFileRoot}resource.ashx?u={this.currentUserId}&v={DateTime.Now.Ticks}";
                this.Avatar.Text = string.Empty;
                this.DeleteAvatar.Visible = true;
            }
            else if (row["Avatar"].ToString().Length > 0)
            {
                // Took out PageContext.BoardSettings.AvatarRemote
                this.AvatarImg.ImageUrl =
                    $"{BoardInfo.ForumClientFileRoot}resource.ashx?url={this.Server.UrlEncode(row["Avatar"].ToString())}&width={this.Get<BoardSettings>().AvatarWidth}&height={this.Get<BoardSettings>().AvatarHeight}&v={DateTime.Now.Ticks}";

                this.Avatar.Text = row["Avatar"].ToString();
                this.DeleteAvatar.Visible = true;
            }
            else if (this.Get<BoardSettings>().AvatarGravatar)
            {
                var x = new MD5CryptoServiceProvider();
                var bs = Encoding.UTF8.GetBytes(this.PageContext.User.Email);
                bs = x.ComputeHash(bs);
                var s = new StringBuilder();

                bs.ForEach(b => s.Append(b.ToString("x2").ToLower()));

                var emailHash = s.ToString();

                var gravatarUrl =
                    $"http://www.gravatar.com/avatar/{emailHash}.jpg?r={this.Get<BoardSettings>().GravatarRating}";

                this.AvatarImg.ImageUrl =
                    $"{BoardInfo.ForumClientFileRoot}resource.ashx?url={this.Server.UrlEncode(gravatarUrl)}&width={this.Get<BoardSettings>().AvatarWidth}&height={this.Get<BoardSettings>().AvatarHeight}&v={DateTime.Now.Ticks}";

                this.NoAvatar.Text = "Gravatar Image";
                this.NoAvatar.Visible = true;
            }
            else
            {
                this.AvatarImg.ImageUrl = "../images/noavatar.svg";
                this.NoAvatar.Visible = true;
            }

            this.AvatarUploadRow.Visible = this.PageContext.CurrentForumPage.IsAdminPage
                                           || this.Get<BoardSettings>().AvatarUpload;
            this.AvatarRemoteRow.Visible = this.PageContext.CurrentForumPage.IsAdminPage
                                           || this.Get<BoardSettings>().AvatarRemote;
            this.AvatarOurs.Visible = this.PageContext.CurrentForumPage.IsAdminPage
                                      || this.Get<BoardSettings>().AvatarGallery;
        }

        #endregion
    }
}