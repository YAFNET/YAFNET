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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YAF.Pages.Profile;

using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using Core.Extensions;
using Core.Helpers;
using Core.Services;

using Types.Extensions;
using Types.Models;

using System.Threading.Tasks;

/// <summary>
/// The attachments Page Class.
/// </summary>
public class AttachmentsModel : ProfilePage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "AttachmentsModel" /> class.
    /// </summary>
    public AttachmentsModel()
        : base("ATTACHMENTS", ForumPages.Profile_Attachments)
    {
    }

    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    [BindProperty]
    public List<Attachment> Attachments { get; set; }

    /// <summary>
    /// The create page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        var displayName = this.PageBoardContext.PageUser.DisplayOrUserName();

        this.PageBoardContext.PageLinks.AddUser(this.PageBoardContext.PageUserID, displayName);
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public IActionResult OnGet()
    {
        return this.BindData();
    }

    /// <summary>
    /// Delete selected Attachments.
    /// </summary>
    public async Task<IActionResult> OnPostDeleteSelectedAsync()
    {
        if (!this.PageBoardContext.UploadAccess)
        {
            return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied);
        }

        var items = this.Attachments.Where(x => x.Selected).Select(x => x.ID).ToList();

        if (items.Count == 0)
        {
            return this.BindData();
        }

        await this.GetRepository<Attachment>().DeleteByIdsAsync(items);

        this.PageBoardContext.Notify(this.GetTextFormatted("DELETED", items.Count), MessageTypes.success);

        return this.BindData();
    }

    /// <summary>
    /// Called when [post].
    /// </summary>
    /// <returns>IActionResult.</returns>
    public IActionResult OnPost()
    {
        return this.BindData();
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
    private PageResult BindData()
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        var list = this.GetRepository<Attachment>().GetPaged(
            a => a.UserID == this.PageBoardContext.PageUserID,
            this.PageBoardContext.PageIndex,
            this.Size);

        this.Attachments = list;

        return this.Page();
    }
}