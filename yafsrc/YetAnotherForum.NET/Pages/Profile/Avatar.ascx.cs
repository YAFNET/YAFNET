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

namespace YAF.Pages.Profile
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Local Avatar Page.
    /// </summary>
    public partial class Avatar : ProfilePage
    {
        #region Constants and Fields

        /// <summary>
        ///   The return user id.
        /// </summary>
        private int returnUserID;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Avatar" /> class.
        /// </summary>
        public Avatar()
            : base("AVATAR")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets CurrentDirectory.
        /// </summary>
        protected string CurrentDirectory
        {
            get => this.ViewState["CurrentDir"] != null ? (string)this.ViewState["CurrentDir"] : string.Empty;

            set => this.ViewState["CurrentDir"] = value;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the Bind event of the Directories control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataListItemEventArgs"/> instance containing the event data.</param>
        public void Directories_Bind([NotNull] object sender, [NotNull] DataListItemEventArgs e)
        {
            var directory = $"{BoardInfo.ForumClientFileRoot}{BoardFolders.Current.Avatars}/";

            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var dirName = e.Item.FindControlAs<LinkButton>("dirName");
            dirName.CommandArgument = directory + Convert.ToString(DataBinder.Eval(e.Item.DataItem, "name"));
            dirName.Text =
                string
                    .Format(
                        @"<p style=""text-align:center""><i class=""far fa-folder"" alt=""{0}"" title=""{0}"" style=""font-size:50px"" /></i><br />{0}</p>", Convert.ToString(DataBinder.Eval(e.Item.DataItem, "name")));
        }

        /// <summary>
        /// Handles the Bind event of the Files control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataListItemEventArgs"/> instance containing the event data.</param>
        public void Files_Bind([NotNull] object sender, [NotNull] DataListItemEventArgs e)
        {
            var directoryPath = Path.Combine(BoardInfo.ForumClientFileRoot, BoardFolders.Current.Avatars);

            var fileName = e.Item.FindControlAs<Literal>("fname");

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var info = new FileInfo(
                    this.Server.MapPath(Convert.ToString(DataBinder.Eval(e.Item.DataItem, "name"))));

                if (this.CurrentDirectory.IsSet())
                {
                    directoryPath = this.CurrentDirectory;
                }

                var tmpExt = info.Extension.ToLower();

                if (tmpExt == ".gif" || tmpExt == ".jpg" || tmpExt == ".jpeg" || tmpExt == ".png" || tmpExt == ".bmp")
                {
                    string link;
                    var encodedFileName = info.Name.Replace(".", "%2E");

                    if (this.returnUserID > 0)
                    {
                        link = BuildLink.GetLink(
                            ForumPages.Admin_EditUser,
                            "u={0}&av={1}",
                            this.returnUserID,
                            this.Server.UrlEncode($"{directoryPath}/{encodedFileName}"));
                    }
                    else
                    {
                        link = BuildLink.GetLink(
                            ForumPages.Profile_EditAvatar,
                            "av={0}",
                            this.Server.UrlEncode($"{directoryPath}/{encodedFileName}"));
                    }

                    fileName.Text =
                        string
                            .Format(
                                @"<div style=""text-align:center""><a href=""{0}""><img src=""{1}"" alt=""{2}"" title=""{2}"" class=""borderless"" /></a><br /><small>{2}</small></div>{3}",
                                link,
                                $"{directoryPath}/{info.Name}",
                                    info.Name,
                                    Environment.NewLine);
                }
            }

            if (e.Item.ItemType != ListItemType.Header)
            {
                return;
            }

            // get the previous directory...
            var previousDirectory = Path.Combine(BoardInfo.ForumClientFileRoot, BoardFolders.Current.Avatars);

            var up = e.Item.FindControlAs<LinkButton>("up");
            up.CommandArgument = previousDirectory;
            up.Text = $@"<p style=""text-align:center"">
                     <i class=""far fa-folder-open""style=""font-size:50px""></i><br />
                     <button type=""button"" class=""btn btn-primary btn-sm""><i class=""fas fa-arrow-left""></i>&nbsp;{this.GetText("UP")}</button></p>";
            up.ToolTip = this.GetText("UP_TITLE");

            // Hide if Top Folder
            if (this.CurrentDirectory.Equals(previousDirectory))
            {
                up.Visible = false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the Clean directory list
        /// </summary>
        /// <param name="baseDir">The base directory.</param>
        /// <returns>
        /// Returns the Clean directory list
        /// </returns>
        [NotNull]
        protected List<DirectoryInfo> DirectoryListClean([NotNull] DirectoryInfo baseDir)
        {
            var avatarDirectories = baseDir.GetDirectories();

            return
                avatarDirectories.Where(
                    dir =>
                    (dir.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden
                    && (dir.Attributes & FileAttributes.System) != FileAttributes.System).ToList();
        }

        /// <summary>
        /// Gets the Clean files list
        /// </summary>
        /// <param name="baseDir">The base directory.</param>
        /// <returns>Returns the Clean files list</returns>
        [NotNull]
        protected List<FileInfo> FilesListClean([NotNull] DirectoryInfo baseDir)
        {
            var avatarFiles = baseDir.GetFiles("*.*");

            return
                avatarFiles.Where(
                    file =>
                    (file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden
                    && (file.Attributes & FileAttributes.System) != FileAttributes.System
                    && this.IsValidAvatarExtension(file.Extension.ToLower())).ToList();
        }

        /// <summary>
        /// Determines whether [is valid avatar extension] [the specified extension].
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>
        /// The is valid avatar extension.
        /// </returns>
        protected bool IsValidAvatarExtension([NotNull] string extension)
        {
            return extension == ".gif" || extension == ".jpg" || extension == ".jpeg" || extension == ".png"
                   || extension == ".bmp";
        }

        /// <summary>
        /// Items the command.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="DataListCommandEventArgs"/> instance containing the event data.</param>
        protected void ItemCommand([NotNull] object source, [NotNull] DataListCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "directory":
                    this.CurrentDirectory = e.CommandArgument.ToString();
                    this.BindData(e.CommandArgument.ToString());
                    break;
            }
        }

        /// <summary>
        /// Returns to the Edit Avatar Page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void BtnCancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Redirect to the edit avatar page
            BuildLink.Redirect(ForumPages.Profile_EditAvatar);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.Get<HttpRequestBase>().QueryString.Exists("u"))
            {
                this.returnUserID = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("u").Value;
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.CurrentDirectory = Path.Combine(BoardInfo.ForumClientFileRoot, BoardFolders.Current.Avatars);

            this.BindData(this.CurrentDirectory);
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();

            if (this.returnUserID > 0)
            {
                this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), string.Empty);
                this.PageLinks.AddLink("Users", BuildLink.GetLink(ForumPages.Admin_Users));
            }
            else
            {
                this.PageLinks.AddLink(
                    this.HtmlEncode(this.PageContext.PageUserName),
                    BuildLink.GetLink(ForumPages.MyAccount));
                this.PageLinks.AddLink(
                    this.GetText("EDIT_AVATAR", "TITLE"),
                    BuildLink.GetLink(ForumPages.Profile_EditAvatar));
            }

            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="currentFolder">The current Folder.</param>
        private void BindData([NotNull] string currentFolder)
        {
            var directoryPath = Path.Combine(BoardInfo.ForumClientFileRoot, BoardFolders.Current.Avatars);

            if (currentFolder.IsSet())
            {
                directoryPath = currentFolder;
            }

            var baseDirectory = new DirectoryInfo(this.Server.MapPath(directoryPath));

            var avatarSubDirs = this.DirectoryListClean(baseDirectory);

            if (avatarSubDirs.Count > 0)
            {
                this.directories.Visible = true;
                this.directories.DataSource = avatarSubDirs;
                this.directories.DataBind();
            }
            else
            {
                this.directories.Visible = false;
            }

            this.files.DataSource = this.FilesListClean(baseDirectory);
            this.files.Visible = true;
            this.files.DataBind();
        }

        #endregion
    }
}