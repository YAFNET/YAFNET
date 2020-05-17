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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.IO;
    using System.Linq;
    using System.Web;
    
    using Core.Services.Import;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
    using YAF.Types.Models.Identity;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Admin Edit Board Page
    /// </summary>
    public partial class EditBoard : AdminPage
    {
        #region Properties

        /// <summary>
        ///   Gets BoardID.
        /// </summary>
        protected int? BoardId
        {
            get
            {
                if (!this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("b").IsSet())
                {
                    return null;
                }

                if (int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("b"), out var boardId))
                {
                    return boardId;
                }

                return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cancel Edit/Create and return Back to the Boards Listening
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CancelClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Admin_Boards);
        }

        /// <summary>
        /// Show/Hide Create Host Admin User Creating
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CreateAdminUserCheckedChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.AdminInfo.Visible = this.CreateAdminUser.Checked;
        }

        /// <summary>
        /// The create board.
        /// </summary>
        /// <param name="adminName">The admin name.</param>
        /// <param name="adminPassword">The admin password.</param>
        /// <param name="adminEmail">The admin email.</param>
        /// <param name="boardName">The board name.</param>
        /// <param name="createUserAndRoles">The create user and roles.</param>
        /// <returns>Returns if the board was created or not</returns>
        protected bool CreateBoard(
            [NotNull] string adminName,
            [NotNull] string adminPassword,
            [NotNull] string adminEmail,
            [NotNull] string boardName,
            bool createUserAndRoles)
        {
            int newBoardId;
            var cult = StaticDataHelper.Cultures();
            var langFile = "english.xml";

            cult.Where(dataRow => dataRow.CultureTag == this.Culture.SelectedValue)
                .ForEach(row => langFile = row.CultureFile);

            if (createUserAndRoles)
            {
                var user = new AspNetUsers
                {
                    Id = Guid.NewGuid().ToString(),
                    ApplicationId = this.Get<BoardSettings>().ApplicationId,
                    UserName = adminName,
                    LoweredUserName = adminName,
                    Email = adminEmail,
                    IsApproved = true
                };

                // Create new admin users
                var result = this.Get<IAspNetUsersHelper>().Create(user, adminPassword);

                if (!result.Succeeded)
                {
                    this.PageContext.AddLoadMessage(
                        $"Create User Failed: {result.Errors.FirstOrDefault()}",
                        MessageTypes.danger);

                    return false;
                }

                // Create groups required for the new board
                AspNetRolesHelper.CreateRole("Administrators");
                AspNetRolesHelper.CreateRole("Registered");

                // Add new admin users to group
                AspNetRolesHelper.AddUserToRole(user, "Administrators");

                // Create Board
                newBoardId = this.DbCreateBoard(
                    boardName,
                    langFile,
                    user);
            }
            else
            {
                // new admin
                var newAdmin = this.Get<IAspNetUsersHelper>().GetUser();

                // Create Board
                newBoardId = this.DbCreateBoard(
                    boardName,
                    langFile,
                    newAdmin);
            }

            if (newBoardId <= 0 || !Config.MultiBoardFolders)
            {
                return true;
            }

            // Successfully created the new board
            var boardFolder = this.Server.MapPath(Path.Combine(Config.BoardRoot, $"{newBoardId}/"));

            // Create New Folders.
            if (!Directory.Exists(Path.Combine(boardFolder, "Images")))
            {
                // Create the Images Folders
                Directory.CreateDirectory(Path.Combine(boardFolder, "Images"));

                // Create Sub Folders
                Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Avatars"));
                Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Categories"));
                Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Forums"));
                Directory.CreateDirectory(Path.Combine(boardFolder, "Images\\Medals"));
            }

            if (!Directory.Exists(Path.Combine(boardFolder, "Uploads")))
            {
                Directory.CreateDirectory(Path.Combine(boardFolder, "Uploads"));
            }

            return true;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.Culture.DataSource = StaticDataHelper.Cultures().OrderBy(x => x.CultureNativeName);
            this.Culture.DataValueField = "CultureTag";
            this.Culture.DataTextField = "CultureNativeName";

            this.BindData();

            if (this.Culture.Items.Count > 0)
            {
                this.Culture.Items.FindByValue(this.Get<BoardSettings>().Culture).Selected = true;
            }

            if (this.BoardId != null)
            {
                this.CreateNewAdminHolder.Visible = false;

                var board = this.GetRepository<Board>().GetById(this.BoardId.Value);

                this.Name.Text = board.Name;
            }
            else
            {
                this.UserName.Text = this.User.UserName;
                this.UserEmail.Text = this.User.Email;
            }

            if (Config.IsDotNetNuke)
            {
                this.CreateNewAdminHolder.Visible = false;
            }
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.Admin_Admin));
            this.PageLinks.AddLink(
                this.GetText("ADMIN_BOARDS", "TITLE"),
                BuildLink.GetLink(ForumPages.Admin_EditBoard));
            this.PageLinks.AddLink(this.GetText("ADMIN_EDITBOARD", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_BOARDS", "TITLE")} - {this.GetText("ADMIN_EDITBOARD", "TITLE")}";
        }

        /// <summary>
        /// Save Current board / Create new Board
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.Name.Text.Trim().Length == 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITBOARD", "MSG_NAME_BOARD"), MessageTypes.warning);
                return;
            }

            if (this.BoardId == null && this.CreateAdminUser.Checked)
            {
                if (this.UserName.Text.Trim().Length == 0)
                {
                    this.PageContext.AddLoadMessage(
                        this.GetText("ADMIN_EDITBOARD", "MSG_NAME_ADMIN"),
                        MessageTypes.warning);
                    return;
                }

                if (this.UserEmail.Text.Trim().Length == 0)
                {
                    this.PageContext.AddLoadMessage(
                        this.GetText("ADMIN_EDITBOARD", "MSG_EMAIL_ADMIN"),
                        MessageTypes.warning);
                    return;
                }

                if (this.UserPass1.Text.Trim().Length == 0)
                {
                    this.PageContext.AddLoadMessage(
                        this.GetText("ADMIN_EDITBOARD", "MSG_PASS_ADMIN"),
                        MessageTypes.warning);
                    return;
                }

                if (this.UserPass1.Text != this.UserPass2.Text)
                {
                    this.PageContext.AddLoadMessage(
                        this.GetText("ADMIN_EDITBOARD", "MSG_PASS_MATCH"),
                        MessageTypes.warning);
                    return;
                }
            }

            if (this.BoardId != null)
            {
                var cult = StaticDataHelper.Cultures();
                var langFile = "en-US";

                cult
                    .Where(dataRow => dataRow.CultureTag == this.Culture.SelectedValue)
                    .ForEach(row => langFile = row.CultureFile);

                // Save current board settings
                this.GetRepository<Board>().Save(
                    this.BoardId ?? 0,
                    this.Name.Text.Trim(),
                    langFile,
                    this.Culture.SelectedItem.Value);
            }
            else
            {
                // Create board
                if (this.CreateAdminUser.Checked)
                {
                    this.CreateBoard(
                        this.UserName.Text.Trim(),
                        this.UserPass1.Text,
                        this.UserEmail.Text.Trim(),
                        this.Name.Text.Trim(),
                        true);
                }
                else
                {
                    // create admin user from logged in user...
                    this.CreateBoard(
                        null,
                        null,
                        null,
                        this.Name.Text.Trim(),
                        false);
                }
            }

            // Done
            this.PageContext.BoardSettings = null;
            BuildLink.Redirect(ForumPages.Admin_Boards);
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.DataBind();
        }

        /// <summary>
        /// Creates the board in the database.
        /// </summary>
        /// <param name="boardName">
        /// Name of the board.
        /// </param>
        /// <param name="langFile">
        /// The language file.
        /// </param>
        /// <param name="newAdmin">
        /// The new admin.
        /// </param>
        /// <returns>
        /// Returns the New Board Id
        /// </returns>
        private int DbCreateBoard(
            string boardName,
            string langFile,
            AspNetUsers newAdmin)
        {
            var newBoardId = this.GetRepository<Board>()
                .Create(
                    boardName,
                    this.Culture.SelectedItem.Value,
                    langFile,
                    newAdmin.UserName,
                    newAdmin.Email,
                    newAdmin.Id,
                    this.PageContext().IsHostAdmin,
                    Config.CreateDistinctRoles && Config.IsAnyPortal ? "YAF " : string.Empty);

            var loadWrapper = new Action<string, Action<Stream>>(
                (file, streamAction) =>
                {
                    var fullFile = this.Get<HttpRequestBase>().MapPath(file);

                    if (!File.Exists(fullFile))
                    {
                        return;
                    }

                    // import into board...
                    using (var stream = new StreamReader(fullFile))
                    {
                        streamAction(stream.BaseStream);
                        stream.Close();
                    }
                });

            // load default bbcode if available...
            loadWrapper("install/bbCodeExtensions.xml", s => DataImport.BBCodeExtensionImport(newBoardId, s));

            return newBoardId;
        }

        #endregion
    }
}