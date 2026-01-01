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

namespace YAF.Core.Controllers;

using System.Collections.Generic;

using YAF.Core.BasePages;
using YAF.Types.Attributes;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The YAF Attachment controller.
/// </summary>
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public class AttachmentController : ForumBaseController
{
    /// <summary>
    /// Gets the paged attachments.
    /// </summary>
    /// <param name="pagedResults">
    /// The paged Results.
    /// </param>
    /// <returns>
    /// Returns the Attachment List as Grid Data Set
    /// </returns>
    [ValidateAntiForgeryToken]
    [HttpPost("GetAttachments")]
    public IActionResult GetAttachments([FromBody] PagedResults pagedResults)
    {
        var userId = this.PageBoardContext.PageUserID;
        var pageSize = pagedResults.PageSize;
        var pageNumber = pagedResults.PageNumber;

        var attachments = this.GetRepository<Attachment>().GetPaged(
            a => a.UserID == userId,
            pageNumber,
            pageSize);

        var attachmentItems = new List<AttachmentItem>();

        attachments.ForEach(
            attach =>
                {
                    var url =
                        this.Get<IUrlHelper>().Action("GetAttachment", "Attachments", new { attachmentId = attach.ID, editor = true });

                    var description = $"{attach.FileName} ({attach.Bytes / 1024} kb)";

                    var iconImage = attach.FileName.IsImageName()
                                        ? $"""<img src="{url}" alt="{description}" title="{description}" class="img-fluid img-thumbnail me-1 attachments-preview" />"""
                                        : "<i class=\"far fa-file-alt attachment-icon\"></i>";

                    var attachment = new AttachmentItem
                                         {
                                             FileName = attach.FileName,
                                             OnClick = $"insertAttachment('{attach.ID}', '{url}')",
                                             IconImage = $"{iconImage}<span>{description}</span>"
                                         };

                    if (attach.FileName.IsImageName())
                    {
                        attachment.DataURL = url;
                    }

                    attachmentItems.Add(attachment);
                });

        return this.Ok(
            new GridDataSet
                {
                    PageNumber = pageNumber,
                    TotalRecords =
                        attachments.Count != 0
                            ? this.GetRepository<Attachment>().Count(a => a.UserID == userId)
                                .ToType<int>()
                            : 0,
                    PageSize = pageSize,
                    AttachmentList = attachmentItems
                });
    }
}