
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

using System.Threading.Tasks;

namespace YAF.Pages.Admin;

using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Models;

/// <summary>
/// The Admin Manage User Attachments Page.
/// </summary>
public class AttachmentsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    [BindProperty]
    public List<Tuple<User, Attachment>> List { get; set; }

    /// <summary>
    /// Gets or sets the count.
    /// </summary>
    /// <value>The count.</value>
    [BindProperty]
    public int Count { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "AttachmentsModel" /> class.
    /// </summary>
    public AttachmentsModel()
        : base("ADMIN_ATTACHMENTS", ForumPages.Admin_Attachments)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_ATTACHMENTS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
        // bind data to controls
        this.BindData();
    }

    /// <summary>
    /// The page size on selected index changed.
    /// </summary>
    public void OnPost()
    {
        this.BindData();
    }

    /// <summary>
    /// Called when [post delete].
    /// </summary>
    /// <param name="id">The identifier.</param>
    public async Task OnPostDeleteAsync(int id)
    {
        await this.GetRepository<Attachment>().DeleteByIdAsync(id);

        // re-bind controls
        this.BindData();
    }

    /// <summary>
    /// Gets the preview image.
    /// </summary>
    /// <param name="o">The Data Row object.</param>
    /// <returns>Returns the Preview Image</returns>
    public string GetPreviewImage(object o)
    {
        var attach = o.ToType<Attachment>();

        var fileName = attach.FileName;
        var isImage = fileName.IsImageName();
        var url = this.Get<IUrlHelper>().Action(
            "GetAttachment",
            "Attachments",
            new { attachmentId = attach.ID, editor = true });

        return isImage
                   ? $"<img src=\"{url}\" alt=\"{fileName}\" title=\"{fileName}\" data-url=\"{url}\" style=\"max-width:30px\" class=\"me-2 img-thumbnail attachments-preview\" />"
                   : "<i class=\"far fa-file-alt attachment-icon me-2\"></i>";
    }

    /// <summary>
    /// Populates data source and binds data to controls.
    /// </summary>
    private void BindData()
    {
        this.PageSizeList = new SelectList(
            StaticDataHelper.PageEntries(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        // list event for this board
        this.List = this.GetRepository<Attachment>().GetByBoardPaged(
            out var count,
            this.PageBoardContext.PageBoardID,
            this.PageBoardContext.PageIndex,
            this.Size);

        this.Count = count;
    }
}