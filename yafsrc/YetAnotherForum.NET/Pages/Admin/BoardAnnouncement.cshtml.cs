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

using System.Collections.Generic;
using System.Globalization;

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Controllers;
using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Types.Extensions;

/// <summary>
/// The Admin Board Announcement Page.
/// </summary>
public class BoardAnnouncementModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public BoardAnnouncementInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the until units.
    /// </summary>
    /// <value>The until units.</value>
    public List<SelectListItem> UntilUnits { get; set; }

    /// <summary>
    /// Gets or sets the types.
    /// </summary>
    /// <value>The types.</value>
    public List<SelectListItem> Types { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "Attachments" /> class.
    /// </summary>
    public BoardAnnouncementModel()
        : base("ADMIN_BOARDSETTINGS", ForumPages.Admin_BoardAnnouncement)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_BOARDSETTINGS", "ANNOUNCEMENT_TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public  void OnGet()
    {
        this.Input = new BoardAnnouncementInputModel();
        this.BindData();
    }

    /// <summary>
    /// Saves the Board Announcement
    /// </summary>
    public IActionResult OnPostSaveAnnouncement()
    {
        var boardAnnouncementUntil = DateTime.Now;

        // number inserted by suspending user
        var count = this.Input.BoardAnnouncementUntil;

        // what time units are used for suspending
        boardAnnouncementUntil = this.Input.BoardAnnouncementUntilUnit switch
            {
                // days
                1 => boardAnnouncementUntil.AddDays(count),
                // hours
                2 => boardAnnouncementUntil.AddHours(count),
                // month
                3 => boardAnnouncementUntil.AddMonths(count),
                _ => boardAnnouncementUntil
            };

        var boardSettings = this.PageBoardContext.BoardSettings;

        boardSettings.BoardAnnouncementUntil = boardAnnouncementUntil.ToString(CultureInfo.InvariantCulture);
        boardSettings.BoardAnnouncement = this.Input.Message;
        boardSettings.BoardAnnouncementType = this.Input.BoardAnnouncementType;

        // save the settings to the database
        this.Get<BoardSettingsService>().SaveRegistry(boardSettings);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_BoardAnnouncement);
    }

    /// <summary>
    /// Deletes the Announcement
    /// </summary>
    public IActionResult OnPostDelete()
    {
        var boardSettings = this.PageBoardContext.BoardSettings;

        boardSettings.BoardAnnouncementUntil = DateTime.MinValue.ToString(CultureInfo.InvariantCulture);
        boardSettings.BoardAnnouncement = null;
        boardSettings.BoardAnnouncementType = "info";

        // save the settings to the database
        this.Get<BoardSettingsService>().SaveRegistry(boardSettings);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_BoardAnnouncement);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        var boardSettings = this.PageBoardContext.BoardSettings;

        // add items to the dropdown
        this.UntilUnits = [
            new SelectListItem { Value = "3", Text = this.GetText("PROFILE", "MONTH") },
            new SelectListItem { Value = "1", Text = this.GetText("PROFILE", "DAYS") },
            new SelectListItem { Value = "2", Text = this.GetText("PROFILE", "HOURS") }
        ];

        this.Types = [
            new SelectListItem { Value = "primary", Text = "primary" },
            new SelectListItem { Value = "secondary", Text = "secondary" },
            new SelectListItem { Value = "success", Text = "success" },
            new SelectListItem { Value = "danger", Text = "danger" },
            new SelectListItem { Value = "warning", Text = "warning" },
            new SelectListItem { Value = "info", Text = "info" },
            new SelectListItem { Value = "light", Text = "light" },
            new SelectListItem { Value = "dark", Text = "dark" }
        ];

        // select hours
        this.Input.BoardAnnouncementUntilUnit = 1;
        this.Input.BoardAnnouncementUntil = 1;

        this.Input.BoardAnnouncementType = "info";

        if (boardSettings.BoardAnnouncement.IsNotSet())
        {
            return;
        }

        this.Input.BoardAnnouncementType = boardSettings.BoardAnnouncementType;
        this.Input.Message = boardSettings.BoardAnnouncement;
    }
}