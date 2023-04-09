/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

using Core.Services.Import;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models.Identity;
using YAF.Types.Models;
using YAF.Types.Objects;

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Types.Attributes;

/// <summary>
/// Admin Edit Board Page
/// </summary>
public class EditBoardModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

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
            this.Get<LinkBuilder>().GetLink(ForumPages.Admin_EditBoard));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITBOARD", "TITLE"), string.Empty);
    }

    /// <summary>
    /// The create board.
    /// </summary>
    /// <param name="adminName">The admin name.</param>
    /// <param name="adminPassword">The admin password.</param>
    /// <param name="adminEmail">The admin email.</param>
    /// <param name="boardName">The board name.</param>
    /// <param name="createUserAndRoles">The create user and roles.</param>
    /// <param name="error"></param>
    /// <returns>Returns if the board was created or not</returns>
    protected bool CreateBoard(
        [NotNull] string adminName,
        [NotNull] string adminPassword,
        [NotNull] string adminEmail,
        [NotNull] string boardName,
        bool createUserAndRoles, 
        out IdentityError error)
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
                                           LoweredUserName = adminName,
                                           Email = adminEmail,
                                           IsApproved = true
                                       };

            // Create new admin users
            var result = this.Get<IAspNetUsersHelper>().Create(user, adminPassword);

            if (!result.Succeeded)
            {
                error = result.Errors.FirstOrDefault();

                return false;
            }

            // Create groups required for the new board
            this.Get<IAspNetRolesHelper>().CreateRole("Administrators");
            this.Get<IAspNetRolesHelper>().CreateRole("Registered");

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
            error = null;
            return true;
        }

        // Successfully created the new board
        var boardFolder = Path.Combine(
            this.Get<IWebHostEnvironment>().WebRootPath,
            this.Get<BoardConfiguration>().BoardRoot,
            $"{newBoardId}/");

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

        error = null;

        return true;
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet(int? b)
    {
        this.Input = new InputModel();

        this.Cultures = new SelectList(
            StaticDataHelper.Cultures().OrderBy(x => x.CultureNativeName),
            "CultureTag",
            "CultureNativeName",
            this.PageBoardContext.BoardSettings.Culture);

        if (b.HasValue)
        {
            var board = this.GetRepository<Board>().GetById(b.Value);

            if (board == null)
            {
                return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
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
    public IActionResult OnPostSave()
    {
        int? b = this.Input.Id == 0 ? null : this.Input.Id;

        if (!b.HasValue && this.Input.CreateAdminUser)
        {
            if (this.Input.UserPass1 != this.Input.UserPass2)
            {
                return this.PageBoardContext.Notify(
                    this.GetText("ADMIN_EDITBOARD", "MSG_PASS_MATCH"),
                    MessageTypes.warning);
            }
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
                boardCreated = this.CreateBoard(
                    this.Input.UserName.Trim(),
                    this.Input.UserPass1,
                    this.Input.UserEmail.Trim(),
                    this.Input.Name.Trim(),
                    true,
                    out error);
            }
            else
            {
                // create admin user from logged in user...
                boardCreated = this.CreateBoard(null, null, null, this.Input.Name.Trim(), false, out error);
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

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Boards);
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
        loadWrapper("bbCodeExtensions.xml", s => DataImport.BBCodeExtensionImport(newBoardId, s));

        // load default spam word if available...
        loadWrapper("SpamWords.xml", s => DataImport.SpamWordsImport(newBoardId, s));

        return newBoardId;
    }

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Culture { get; set; }

        public bool CreateAdminUser { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string UserPass1 { get; set; }

        public string UserPass2 { get; set; }
    }
}