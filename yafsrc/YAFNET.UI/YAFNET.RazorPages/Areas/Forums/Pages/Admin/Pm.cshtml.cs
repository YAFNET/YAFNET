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

using System.Threading.Tasks;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// The Admin Private messages page
/// </summary>
public class PmModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public PmInputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PmModel"/> class.
    /// </summary>
    public PmModel()
        : base("ADMIN_PM", ForumPages.Admin_Pm)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_PM", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
        this.Input = new PmInputModel();

        this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.Input.Days2 = 180;

        this.Input.Count = this.GetRepository<PrivateMessage>().Count(m => (m.Flags & 2) != 2 || (m.Flags & 4) != 4).ToString();
    }

    /// <summary>
    /// Commits the click.
    /// </summary>
    public async Task OnPostCommitAsync()
    {
        await this.GetRepository<PrivateMessage>().PruneAllAsync(this.Input.Days2);

        this.BindData();
    }
}