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

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YAF.Pages.Admin.EditUser;

using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Types.Extensions;
using YAF.Types.Interfaces;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// Class UsersAttachmentsModel.
/// Implements the <see cref="YAF.Core.BasePages.AdminPage" />
/// </summary>
/// <seealso cref="YAF.Core.BasePages.AdminPage" />
public class UsersAttachmentsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the User Data.
    /// </summary>
    public Tuple<User, AspNetUsers, Rank, VAccess> EditUser { get; set; }

    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    [BindProperty]
    public List<Attachment> Attachments { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public UsersSignatureInputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersAttachmentsModel"/> class.
    /// </summary>
    public UsersAttachmentsModel()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

    /// <summary>
    /// Called when [get].
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>IActionResult.</returns>
    public IActionResult OnGet(int userId)
    {
        if (!BoardContext.Current.IsAdmin)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        this.Input = new UsersSignatureInputModel {
            UserId = userId
        };

        return this.BindData(userId);
    }

    /// <summary>
    /// Delete selected Attachments.
    /// </summary>
    public async Task<IActionResult> OnPostDeleteSelectedAsync()
    {
        var items = this.Attachments.Where(x => x.Selected).Select(x => x.ID).ToList();

        if (items.Count == 0)
        {
            return this.Get<ILinkBuilder>()
                .Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View11" });
        }

        await this.GetRepository<Attachment>().DeleteByIdsAsync(items);

        this.PageBoardContext.SessionNotify(this.GetTextFormatted("DELETED", items.Count), MessageTypes.success);

        return this.Get<ILinkBuilder>()
            .Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View11" });
    }

    /// <summary>
    /// Gets the preview image.
    /// </summary>
    /// <param name="attach">
    /// The attachment.
    /// </param>
    /// <returns>
    /// Returns the Preview Image
    /// </returns>
    public IHtmlContent GetPreviewImage(Attachment attach)
    {
        var fileName = attach.FileName;
        var isImage = fileName.IsImageName();

        var url =
            this.Get<IUrlHelper>().Action("GetAttachment", "Attachments", new { attachmentId = attach.ID, editor = true });

        var icon = new TagBuilder(HtmlTag.I);
        icon.AddCssClass("far fa-file-alt attachment-icon me-2");

        var image = new TagBuilder(HtmlTag.Img) { TagRenderMode = TagRenderMode.SelfClosing };

        image.AddCssClass("me-2 img-thumbnail attachments-preview");

        image.MergeAttribute("src", url);
        image.MergeAttribute("alt", fileName);
        image.MergeAttribute("title", fileName);
        image.MergeAttribute("data-url", url);
        image.MergeAttribute("style", "max-width:30px");

        return isImage ? image : icon;
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private PageResult BindData(int userId)
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        var list = this.GetRepository<Attachment>().GetPaged(
            a => a.UserID == userId,
            this.PageBoardContext.PageIndex,
            this.Size);

        this.Attachments = list;

        return this.Page();
    }
}