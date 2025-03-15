
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

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YAF.Pages.Admin;

using System.Collections.Generic;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// Admin Page for managing Boards
/// </summary>
public class BoardsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the list.
    /// </summary>
    /// <value>The list.</value>
    [BindProperty]
    public List<Board> List { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoardsModel"/> class.
    /// </summary>
    public BoardsModel()
        : base("ADMIN_BOARDS", ForumPages.Admin_Boards)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_BOARDS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet()
    {
        return !this.PageBoardContext.PageUser.UserFlags.IsHostAdmin ? this.Get<ILinkBuilder>().AccessDenied() : this.BindData();
    }

    /// <summary>
    /// Delete the board.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>IActionResult.</returns>
    public IActionResult OnPostDelete(int id)
    {
        this.GetRepository<Board>().DeleteBoard(id);
        return this.BindData();
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private PageResult BindData()
    {
        this.List = this.GetRepository<Board>().GetAll();

        return this.Page();
    }
}