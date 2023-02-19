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

namespace YAF.Pages.Admin.EditUser;

using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Services;
using YAF.Types;
using YAF.Types.Attributes;
using YAF.Types.Extensions;
using YAF.Types.Interfaces;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

public class UsersAttachmentsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the User Data.
    /// </summary>
    [NotNull]
    public Tuple<User, AspNetUsers, Rank, vaccess> EditUser { get; set; }

    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    [BindProperty]
    public List<Attachment> Attachments { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    public UsersAttachmentsModel()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

    public IActionResult OnGet(int userId)
    {
        if (!BoardContext.Current.IsAdmin)
        {
            return this.Get<LinkBuilder>().AccessDenied();
        }

        this.Input = new InputModel
                     {
                         UserId = userId
                     };

        this.BindData(userId);

        return this.Page();
    }

    /// <summary>
    /// Delete selected Attachments.
    /// </summary>
    public IActionResult OnPostDeleteSelected()
    {
       var items = this.Attachments.Where(x => x.Selected).ToList();

        if (items.Any())
        {
            items.ForEach(item => this.GetRepository<Attachment>().DeleteById(item.ID));

            this.PageBoardContext.SessionNotify(this.GetTextFormatted("DELETED", items.Count), MessageTypes.success);
        }

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View11" });
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
    public IHtmlContent GetPreviewImage([NotNull] Attachment attach)
    {
        var fileName = attach.FileName;
        var isImage = fileName.IsImageName();

        var url =
            this.Get<IUrlHelper>().Action("GetAttachment", "Attachments", new { attachmentId = attach.ID, editor = true });

        var icon = new TagBuilder("i");
        icon.AddCssClass("far fa-file-alt attachment-icon me-2");

        var image = new TagBuilder("img") { TagRenderMode = TagRenderMode.SelfClosing };

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
    private void BindData(int userId)
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        var list = this.GetRepository<Attachment>().GetPaged(
            a => a.UserID == userId,
            this.PageBoardContext.PageIndex,
            this.Size);

        this.Attachments = list;
    }

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        public int UserId { get; set; }
    }
}