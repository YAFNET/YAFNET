
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
        // only attachments of users of the board
        var attachment = await this.GetRepository<Attachment>().GetByIdAsync(id);

        if (attachment != null
            && await this.GetRepository<User>().ExistsAsync(u => u.ID == attachment.UserID && u.BoardID == this.PageBoardContext.PageBoardID))
        {
            // the file is deleted too
            this.GetRepository<Attachment>().Delete(id);
        }

        // re-bind controls
        this.BindData();
    }

    /// <summary>
    /// Gets the preview image (a thumbnail of an image, an icon of the file type for other files).
    /// </summary>
    /// <param name="attachment">The attachment.</param>
    /// <returns>Returns the Preview Image</returns>
    public string GetPreviewImage(Attachment attachment)
    {
        var fileName = attachment.FileName;
        var url = AttachmentLinkHelper.GetUrl(attachment.ID, true);

        if (fileName.IsImageName())
        {
            return $"<img src=\"{url}\" alt=\"{this.HtmlEncode(fileName)}\" title=\"{this.HtmlEncode(fileName)}\" data-url=\"{url}\" class=\"img-thumbnail attachments-preview attachment-thumb\" loading=\"lazy\" />";
        }

        var icon = System.IO.Path.GetExtension(fileName ?? string.Empty).ToLowerInvariant() switch
        {
            ".pdf" => "file-pdf",
            ".zip" or ".rar" or ".7z" or ".gz" or ".tar" => "file-zipper",
            ".doc" or ".docx" or ".odt" or ".rtf" => "file-word",
            ".xls" or ".xlsx" or ".ods" or ".csv" => "file-excel",
            ".ppt" or ".pptx" or ".odp" => "file-powerpoint",
            ".mp3" or ".wav" or ".ogg" or ".flac" => "file-audio",
            ".mp4" or ".avi" or ".mov" or ".mkv" or ".webm" => "file-video",
            ".txt" or ".log" or ".md" => "file-lines",
            ".cs" or ".js" or ".ts" or ".html" or ".css" or ".xml" or ".json" or ".sql" => "file-code",
            _ => "file"
        };

        return $"<span class=\"attachment-icon bg-body-tertiary text-body-secondary\"><i class=\"fas fa-{icon} fa-fw\"></i></span>";
    }

    /// <summary>
    /// Formats the size of a file (bytes, KB, MB).
    /// </summary>
    /// <param name="bytes">The size in bytes.</param>
    /// <returns>The formatted size.</returns>
    public static string FormatSize(long bytes)
    {
        return bytes switch
        {
            < 1024 => $"{bytes} B",
            < 1024 * 1024 => $"{bytes / 1024d:0.#} KB",
            _ => $"{bytes / (1024d * 1024d):0.#} MB"
        };
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