﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Pages.Admin;

using System.IO;

using Core.Services.Import;

using YAF.Types.Models.Identity;
using YAF.Types.Models;

/// <summary>
/// Admin Edit Board Page
/// </summary>
public partial class EditBoard : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditBoard"/> class.
    /// </summary>
    public EditBoard()
        : base("ADMIN_EDITBOARD", ForumPages.Admin_EditBoard)
    {
    }

    /// <summary>
    ///   Gets BoardID.
    /// </summary>
    protected int? BoardId =>
        this.Get<HttpRequestBase>().QueryString.Exists("b")
            ? this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("b")
            : null;

    /// <summary>
    /// Cancel Edit/Create and return Back to the Boards Listening
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void CancelClick(object sender, EventArgs e)
    {
        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Boards);
    }

    /// <summary>
    /// Show/Hide Create Host Admin User Creating
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void CreateAdminUserCheckedChanged(object sender, EventArgs e)
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
        string adminName,
        string adminPassword,
        string adminEmail,
        string boardName,
        bool createUserAndRoles)
    {
        int newBoardId;
        var cult = StaticDataHelper.Cultures();
        var langFile = "english.json";

        cult.Where(c => c.CultureTag == this.Culture.SelectedValue)
            .ForEach(c => langFile = c.CultureFile);

        if (createUserAndRoles)
        {
            var user = new AspNetUsers
                           {
                               Id = Guid.NewGuid().ToString(),
                               ApplicationId = this.PageBoardContext.BoardSettings.ApplicationId,
                               UserName = adminName,
                               LoweredUserName = adminName.ToLower(),
                               Email = adminEmail,
                               LoweredEmail = adminEmail.ToLower(),
                               IsApproved = true
                           };

            // Create new admin users
            var result = this.Get<IAspNetUsersHelper>().Create(user, adminPassword);

            if (!result.Succeeded)
            {
                this.PageBoardContext.Notify(
                    $"Create User Failed: {result.Errors.FirstOrDefault()}",
                    MessageTypes.danger);

                return false;
            }

            // Create groups required for the new board
            this.Get<IAspNetRolesHelper>().CreateRole("Administrators");
            this.Get<IAspNetRolesHelper>().CreateRole("Registered");

            // Add new admin users to group
            this.Get<IAspNetRolesHelper>().AddUserToRole(user, "Administrators");

            // Create Board
            newBoardId = this.DbCreateBoard(
                boardName,
                langFile,
                user);
        }
        else
        {
            // new admin
            var newAdmin = this.PageBoardContext.MembershipUser;

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
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.FormValidatorJs),
            JavaScriptBlocks.FormValidatorJs(this.Save.ClientID));

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
            this.Culture.Items.FindByValue(this.PageBoardContext.BoardSettings.Culture).Selected = true;
        }

        if (this.BoardId != null)
        {
            this.CreateNewAdminHolder.Visible = false;

            var board = this.GetRepository<Board>().GetById(this.BoardId.Value);

            if (board == null)
            {
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            }

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
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMIN_BOARDS", "TITLE"),
            this.Get<LinkBuilder>().GetLink(ForumPages.Admin_EditBoard));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITBOARD", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Save Current board / Create new Board
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void SaveClick(object sender, EventArgs e)
    {
        if (this.BoardId == null && this.CreateAdminUser.Checked && this.UserPass1.Text != this.UserPass2.Text)
        {
            this.PageBoardContext.Notify(
                this.GetText("ADMIN_EDITBOARD", "MSG_PASS_MATCH"),
                MessageTypes.warning);
            return;
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
        this.PageBoardContext.BoardSettings = null;

        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Boards);
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
                this.PageBoardContext.BoardSettings.ForumEmail,
                this.Culture.SelectedItem.Value,
                langFile,
                newAdmin.UserName,
                newAdmin.Email,
                newAdmin.Id,
                this.PageBoardContext().PageUser.UserFlags.IsHostAdmin,
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
                    using var stream = new StreamReader(fullFile);

                    streamAction(stream.BaseStream);
                    stream.Close();
                });

        // load default bbcode if available...
        loadWrapper($"{Config.InstallPath()}bbCodeExtensions.xml", s => DataImport.BBCodeExtensionImport(newBoardId, s));

        // load default spam word if available...
        loadWrapper($"{Config.InstallPath()}SpamWords.xml", s => DataImport.SpamWordsImport(newBoardId, s));

        return newBoardId;
    }
}