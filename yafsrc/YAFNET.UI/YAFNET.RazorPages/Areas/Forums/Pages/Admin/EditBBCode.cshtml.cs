
/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

using System.Threading.Tasks;

namespace YAF.Pages.Admin;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// The BBCode Admin Edit Page.
/// </summary>
public class EditBBCodeModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public EditBBCodeInputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditBBCodeModel"/> class.
    /// </summary>
    public EditBBCodeModel()
        : base("ADMIN_BBCODE_EDIT", ForumPages.Admin_EditBBCode)
    {
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMIN_BBCODE", "TITLE"),
            this.Get<ILinkBuilder>().GetLink(ForumPages.Admin_BBCodes));
    }

    /// <summary>
    /// Adds the New BB Code or saves the existing one
    /// </summary>
    public async Task<IActionResult> OnPostAddAsync()
    {
        await this.GetRepository<BBCode>().SaveAsync(
            this.Input.BBCodeId,
            this.Input.Name.Trim(),
            this.Input.Description,
            this.Input.OnClickJS,
            this.Input.DisplayJS,
            this.Input.EditJS,
            this.Input.DisplayCSS,
            this.Input.SearchRegEx,
            this.Input.ReplaceRegEx,
            this.Input.Variables,
            this.Input.UseModule,
            this.Input.UseToolbar,
            this.Input.ModuleClass,
            this.Input.ExecOrder);

       return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_BBCodes);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    /// <param name="b">The BBCode Id</param>
    protected void BindData(int? b)
    {
        if (!b.HasValue)
        {
            return;
        }

        var code = this.GetRepository<BBCode>().GetById(b.Value);

        // fill the control values...
        this.Input.BBCodeId = code.ID;
        this.Input.Name = code.Name;
        this.Input.ExecOrder = code.ExecOrder;
        this.Input.Description = code.Description;
        this.Input.OnClickJS = code.OnClickJS;
        this.Input.DisplayJS = code.DisplayJS;
        this.Input.EditJS = code.EditJS;
        this.Input.DisplayCSS = code.DisplayCSS;
        this.Input.SearchRegEx = code.SearchRegex;
        this.Input.ReplaceRegEx = code.ReplaceRegex;
        this.Input.Variables = code.Variables;
        this.Input.ModuleClass = code.ModuleClass;
        this.Input.UseModule = code.UseModule ?? false;
        this.Input.UseToolbar = code.UseToolbar ?? false;
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet(int? b)
    {
        this.Input = new EditBBCodeInputModel();

        var strAddEdit = b is null ? this.GetText("COMMON", "ADD") : this.GetText("COMMON", "EDIT");

        this.PageBoardContext.PageLinks.AddLink(string.Format(this.GetText("ADMIN_BBCODE_EDIT", "TITLE"), strAddEdit), string.Empty);

        this.PageTitle = string.Format(this.GetText("ADMIN_BBCODE_EDIT", "TITLE"), strAddEdit);

        this.BindData(b);

        return this.Page();
    }
}