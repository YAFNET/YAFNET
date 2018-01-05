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

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

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
            YafBuildLink.Redirect(this.PageContext.CurrentForumPage.IsAdminPage ? ForumPages.admin_users : ForumPages.cp_profile);
        }

        /// <summary>
        /// Delete The Current Avatar
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteAvatar_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            LegacyDb.user_deleteavatar(this.currentUserId);

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

            if (this.PageContext.CurrentForumPage.IsAdminPage && this.PageContext.IsAdmin && this.PageContext.QueryIDs.ContainsKey("u"))
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
            if (this.Request.QueryString.GetFirstOrDefault("av") != null)
            {
                // save the avatar right now...
                LegacyDb.user_saveavatar(
                    this.currentUserId,
                    "{0}{1}".FormatWith(
                        BaseUrlBuilder.BaseUrl,
                        this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("av")),
                    null,
                    null);

                // clear the cache for this user...
                this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.currentUserId));
            }

            this.UpdateRemote.Text = "<i class=\"fa fa-save fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("COMMON", "UPDATE"));
            this.UpdateUpload.Text = "<i class=\"fa fa-save fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("COMMON", "UPDATE"));
            this.Back.Text = "<i class=\"fa fa-reply fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("COMMON", "BACK"));

            this.NoAvatar.Text = this.GetText("CP_EDITAVATAR", "NOAVATAR");

            this.DeleteAvatar.Text = "<i class=\"fa fa-trash fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("CP_EDITAVATAR", "AVATARDELETE"));
            this.DeleteAvatar.Attributes["onclick"] =
                "return confirm('{0}?')".FormatWith(this.GetText("CP_EDITAVATAR", "AVATARDELETE"));

            var addAdminParam = string.Empty;
            if (this.PageContext.CurrentForumPage.IsAdminPage)
            {
                addAdminParam = "u={0}".FormatWith(this.currentUserId);
            }

            this.OurAvatar.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.avatar, addAdminParam);
            this.OurAvatar.Text = this.GetText("CP_EDITAVATAR", "OURAVATAR_SELECT");

            this.noteRemote.Text = this.GetTextFormatted(
                "NOTE_REMOTE",
                this.Get<YafBoardSettings>().AvatarWidth.ToString(),
                this.Get<YafBoardSettings>().AvatarHeight.ToString());
            this.noteLocal.Text = this.GetTextFormatted(
                "NOTE_LOCAL",
                this.Get<YafBoardSettings>().AvatarWidth.ToString(),
                this.Get<YafBoardSettings>().AvatarHeight,
                (this.Get<YafBoardSettings>().AvatarSize / 1024).ToString());

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
                this.Avatar.Text = "http://{0}".FormatWith(this.Avatar.Text);
            }

            // update
            LegacyDb.user_saveavatar(this.currentUserId, this.Avatar.Text.Trim(), null, null);

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

            long x = this.Get<YafBoardSettings>().AvatarWidth;
            long y = this.Get<YafBoardSettings>().AvatarHeight;
            var avatarSize = this.Get<YafBoardSettings>().AvatarSize;

            Stream resized = null;

            try
            {
                using (var img = Image.FromStream(this.File.PostedFile.InputStream))
                {
                    if (img.Width > x || img.Height > y)
                    {
                        this.PageContext.AddLoadMessage(
                            "{0} {1} {2}".FormatWith(
                                this.GetTextFormatted("WARN_TOOBIG", x, y),
                                this.GetTextFormatted("WARN_SIZE", img.Width, img.Height),
                                this.GetText("CP_EDITAVATAR", "WARN_RESIZED")));

                        resized = ImageHelper.GetResizedImageStreamFromImage(img, x, y);
                    }
                }

                // Delete old first...
                LegacyDb.user_deleteavatar(this.currentUserId);

                if (this.Get<YafBoardSettings>().UseFileTable)
                {
                    var image = Image.FromStream(resized ?? this.File.PostedFile.InputStream);

                    var memoryStream = new MemoryStream();
                    image.Save(memoryStream, image.RawFormat);
                    memoryStream.Position = 0;

                    LegacyDb.user_saveavatar(this.currentUserId, null, memoryStream, this.File.PostedFile.ContentType);
                }
                else
                {
                    var uploadFolderPath =
                        this.Get<HttpRequestBase>()
                            .MapPath(string.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

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

                    var newFileName = "{0}{1}".FormatWith(this.currentUserId, Path.GetExtension(fileName));

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

                    LegacyDb.user_saveavatar(
                        this.currentUserId,
                        "{0}{1}/{2}".FormatWith(YafForumInfo.ForumBaseUrl, YafBoardFolders.Current.Uploads, newFileName),
                        null,
                        null);
                }

                // clear the cache for this user...
                this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.currentUserId));

                if (avatarSize > 0 && this.File.PostedFile.ContentLength >= avatarSize && resized == null)
                {
                    this.PageContext.AddLoadMessage(
                        "{0} {1}".FormatWith(
                            this.GetTextFormatted("WARN_BIGFILE", avatarSize),
                            this.GetTextFormatted("WARN_FILESIZE", this.File.PostedFile.ContentLength)),
                        MessageTypes.warning);
                }

                this.BindData();
            }
            catch (Exception exception)
            {
                this.Logger
                    .Log(
                        exception.Message,
                        EventLogTypes.Error,
                        this.PageContext.CurrentUserData.UserName,
                        string.Empty,
                        exception);

                // image is probably invalid...
                this.PageContext.AddLoadMessage(this.GetText("CP_EDITAVATAR", "INVALID_FILE"), MessageTypes.danger);
            }
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            DataRow row;

            using (var dt = LegacyDb.user_list(this.PageContext.PageBoardID, this.currentUserId, null))
            {
                row = dt.Rows[0];
            }

            this.AvatarImg.Visible = true;
            this.Avatar.Text = string.Empty;
            this.DeleteAvatar.Visible = false;
            this.NoAvatar.Visible = false;

            if (row["HasAvatarImage"] != null
                && long.Parse(row["HasAvatarImage"].ToString()) > 0)
            {
                this.AvatarImg.ImageUrl = "{0}resource.ashx?u={1}&v={2}".FormatWith(
                    YafForumInfo.ForumClientFileRoot,
                    this.currentUserId,
                    DateTime.Now.Ticks);
                this.Avatar.Text = string.Empty;
                this.DeleteAvatar.Visible = true;
            }
            else if (row["Avatar"].ToString().Length > 0)
            {
                // Took out PageContext.BoardSettings.AvatarRemote
                this.AvatarImg.ImageUrl =
                    "{3}resource.ashx?url={0}&width={1}&height={2}&v={4}".FormatWith(
                        this.Server.UrlEncode(row["Avatar"].ToString()),
                        this.Get<YafBoardSettings>().AvatarWidth,
                        this.Get<YafBoardSettings>().AvatarHeight,
                        YafForumInfo.ForumClientFileRoot,
                        DateTime.Now.Ticks);

                this.Avatar.Text = row["Avatar"].ToString();
                this.DeleteAvatar.Visible = true;
            }
            else if (this.Get<YafBoardSettings>().AvatarGravatar)
            {
                var x = new MD5CryptoServiceProvider();
                var bs = Encoding.UTF8.GetBytes(this.PageContext.User.Email);
                bs = x.ComputeHash(bs);
                var s = new StringBuilder();
                foreach (var b in bs)
                {
                    s.Append(b.ToString("x2").ToLower());
                }

                var emailHash = s.ToString();

                var gravatarUrl = "http://www.gravatar.com/avatar/{0}.jpg?r={1}".FormatWith(
                    emailHash,
                    this.Get<YafBoardSettings>().GravatarRating);

                this.AvatarImg.ImageUrl =
                    "{3}resource.ashx?url={0}&width={1}&height={2}&v={4}".FormatWith(
                        this.Server.UrlEncode(gravatarUrl),
                        this.Get<YafBoardSettings>().AvatarWidth,
                        this.Get<YafBoardSettings>().AvatarHeight,
                        YafForumInfo.ForumClientFileRoot,
                        DateTime.Now.Ticks);

                this.NoAvatar.Text = "Gravatar Image";
                this.NoAvatar.Visible = true;
            }
            else
            {
                this.AvatarImg.ImageUrl = "../images/noavatar.gif";
                this.NoAvatar.Visible = true;
            }

            this.AvatarUploadRow.Visible = this.PageContext.CurrentForumPage.IsAdminPage || this.Get<YafBoardSettings>().AvatarUpload;
            this.AvatarRemoteRow.Visible = this.PageContext.CurrentForumPage.IsAdminPage || this.Get<YafBoardSettings>().AvatarRemote;
            this.AvatarOurs.Visible = this.PageContext.CurrentForumPage.IsAdminPage || this.Get<YafBoardSettings>().AvatarGallery;
        }

        #endregion
    }
}