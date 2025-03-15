/* Yet Another Forum.NET
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
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models.Identity;
using YAF.Types.Models;
using YAF.Types.Objects;

using Microsoft.AspNetCore.Mvc.Rendering;

/// <summary>
/// Admin Edit Board Page
/// </summary>
public class EditBoardModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public EditBoardInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the cultures.
    /// </summary>
    /// <value>The cultures.</value>
    public SelectList Cultures { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditBoardModel"/> class.
    /// </summary>
    public EditBoardModel()
        : base("ADMIN_EDITBOARD", ForumPages.Admin_EditBoard)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMIN_BOARDS", "TITLE"),
            this.Get<ILinkBuilder>().GetLink(ForumPages.Admin_EditBoard));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITBOARD", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Creates the new Board.
    /// </summary>
    /// <param name="adminName">The admin name.</param>
    /// <param name="adminPassword">The admin password.</param>
    /// <param name="adminEmail">The admin email.</param>
    /// <param name="boardName">The board name.</param>
    /// <param name="createUserAndRoles">Create user and roles.</param>
    /// <returns>Returns if the board was created or not</returns>
    private async Task<(bool Result, IdentityError Error)> CreateBoardAsync(
        string adminName,
        string adminPassword,
        string adminEmail,
        string boardName,
        bool createUserAndRoles)
    {
        int newBoardId;
        var cult = StaticDataHelper.Cultures();
        var langFile = "english.json";

        cult.Where(c => c.CultureTag == this.Input.Culture).ForEach(c => langFile = c.CultureFile);

        if (createUserAndRoles)
        {
            var user = new AspNetUsers {
                Id = Guid.NewGuid().ToString(),
                ApplicationId = this.PageBoardContext.BoardSettings.ApplicationId,
                UserName = adminName,
                LoweredUserName = adminName.ToLower(),
                Email = adminEmail,
                LoweredEmail = adminEmail.ToLower(),
                IsApproved = true
            };

            // Create new admin users
            var result = await this.Get<IAspNetUsersHelper>().CreateUserAsync(user, adminPassword);

            if (!result.Succeeded)
            {
                return (false, result.Errors.FirstOrDefault());
            }

            // Create groups required for the new board
            await this.Get<IAspNetRolesHelper>().CreateRoleAsync("Administrators");
            await this.Get<IAspNetRolesHelper>().CreateRoleAsync("Registered");

            // Add new admin users to group
            this.Get<IAspNetRolesHelper>().AddUserToRole(user, "Administrators");

            // Create Board
            newBoardId = this.DbCreateBoard(boardName, langFile, user);
        }
        else
        {
            // new admin
            var newAdmin = this.PageBoardContext.MembershipUser;

            // Create Board
            newBoardId = this.DbCreateBoard(boardName, langFile, newAdmin);
        }

        if (newBoardId <= 0 || !this.Get<BoardConfiguration>().MultiBoardFolders)
        {
            return (true, null);
        }

        // Successfully created the new board
        var boardFolder = Path.Combine(
            this.Get<IWebHostEnvironment>().WebRootPath,
            $"{newBoardId}/");

        // Create New Folders.
        if (!Directory.Exists(Path.Combine(boardFolder, "Images")))
        {
            // Create the Images Folders
            Directory.CreateDirectory(Path.Combine(boardFolder, "Images"));

            // Create Sub Folders
            Directory.CreateDirectory(Path.Combine(boardFolder, "Images", "Avatars"));
            Directory.CreateDirectory(Path.Combine(boardFolder, "Images", "Categories"));
            Directory.CreateDirectory(Path.Combine(boardFolder, "Images", "Forums"));
            Directory.CreateDirectory(Path.Combine(boardFolder, "Images", "Medals"));
        }

        if (!Directory.Exists(Path.Combine(boardFolder, "Uploads")))
        {
            Directory.CreateDirectory(Path.Combine(boardFolder, "Uploads"));
        }

        return (true, null);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet(int b)
    {
        this.Input = new EditBoardInputModel();

        this.Cultures = new SelectList(
            StaticDataHelper.Cultures().OrderBy(x => x.CultureNativeName),
            "CultureTag",
            "CultureNativeName",
            this.PageBoardContext.BoardSettings.Culture);

        if (b > 0)
        {
            var board = this.GetRepository<Board>().GetById(b);

            if (board is null)
            {
                return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            }

            this.Input.Id = board.ID;

            this.Input.Name = board.Name;
        }
        else
        {
            this.Input.UserName = this.AspNetUser.UserName;
            this.Input.UserEmail = this.AspNetUser.Email;
        }

        return this.Page();
    }

    /// <summary>
    /// Save Current board / Create new Board
    /// </summary>
    public async Task<IActionResult> OnPostSaveAsync()
    {
        int? b = this.Input.Id == 0 ? null : this.Input.Id;

        if (!b.HasValue && this.Input.CreateAdminUser && this.Input.UserPass1 != this.Input.UserPass2)
        {
            return this.PageBoardContext.Notify(
                this.GetText("ADMIN_EDITBOARD", "MSG_PASS_MATCH"),
                MessageTypes.warning);
        }

        if (b.HasValue)
        {
            var cult = StaticDataHelper.Cultures();
            var langFile = "en-US";

            cult.Where(dataRow => dataRow.CultureTag == this.Input.Culture).ForEach(row => langFile = row.CultureFile);

            // Save current board settings
            this.GetRepository<Board>().Save(b.Value, this.Input.Name.Trim(), langFile, this.Input.Culture);
        }
        else
        {
            bool boardCreated;
            IdentityError error;

            // Create board
            if (this.Input.CreateAdminUser)
            {
                var result = await this.CreateBoardAsync(
                    this.Input.UserName.Trim(),
                    this.Input.UserPass1,
                    this.Input.UserEmail.Trim(),
                    this.Input.Name.Trim(),
                    true);

                boardCreated = result.Result;
                error = result.Error;
            }
            else
            {
                // create admin user from logged-in user...
                var result = await this.CreateBoardAsync(null, null, null, this.Input.Name.Trim(), false);

                boardCreated = result.Result;
                error = result.Error;
            }

            if (!boardCreated)
            {
                return this.PageBoardContext.Notify(
                    $"Create User Failed: {error.Description}",
                    MessageTypes.danger);
            }
        }

        // Done
        this.PageBoardContext.BoardSettings = null;

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Boards);
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
    /// Returns the New Board ID
    /// </returns>
    private int DbCreateBoard(string boardName, string langFile, AspNetUsers newAdmin)
    {
        var newBoardId = this.GetRepository<Board>().Create(
            boardName,
            this.PageBoardContext.BoardSettings.ForumEmail,
            this.Input.Culture,
            langFile,
            newAdmin.UserName,
            newAdmin.Email,
            newAdmin.Id,
            this.PageBoardContext.PageUser.UserFlags.IsHostAdmin,
            string.Empty);

        var loadWrapper = new Action<string, Action<Stream>>(
            (file, streamAction) =>
            {
                var fullFile = Path.Combine(this.Get<IWebHostEnvironment>().WebRootPath, "Resources", file);

                if (!System.IO.File.Exists(fullFile))
                {
                    return;
                }

                // import into board...
                using var stream = new StreamReader(fullFile);

                streamAction(stream.BaseStream);
                stream.Close();
            });

        // load default bbcode if available...
        loadWrapper("bbCodeExtensions.xml", s => this.Get<IDataImporter>().BBCodeExtensionImport(newBoardId, s));

        // load default spam word if available...
        loadWrapper("SpamWords.xml", s => this.Get<IDataImporter>().SpamWordsImport(newBoardId, s));

        return newBoardId;
    }
}