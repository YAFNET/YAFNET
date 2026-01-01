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

namespace YAF.Web.BBCodes;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// The Attachment BB Code Module.
/// </summary>
public class Attach : BBCodeControl
{
    /// <summary>
    /// Render The Album Image as Link with Image
    /// </summary>
    /// <param name="stringBuilder">
    ///     The string Builder.
    /// </param>
    public override Task RenderAsync(StringBuilder stringBuilder)
    {
        var attachId = HtmlTagHelper.StripHtml(this.Parameters["inner"]);

        if (!ValidationHelper.IsNumeric(attachId))
        {
            return Task.CompletedTask;
        }

        var attachment = this.GetRepository<Attachment>().GetById(attachId.ToType<int>());

        if (attachment is null)
        {
            return Task.CompletedTask;
        }

        var filename = attachment.FileName.ToLower();
        var showImage = false;

        // verify it's not too large to display
        // Ederon : 02/17/2009 - made it board setting
        if (attachment.Bytes.ToType<int>() <= PageContext.BoardSettings.PictureAttachmentDisplayTreshold)
        {
            // is it an image file?
            showImage = filename.IsImageName();
        }

        // user doesn't have rights to download, don't show the image
        if (!PageContext.DownloadAccess)
        {
            stringBuilder.Append(
                $"""<i class="fa fa-file"></i>&nbsp;{attachment.FileName} <span class="badge text-bg-warning" role="alert">{this.GetText("ATTACH_NO")}</span>""");

            return Task.CompletedTask;
        }

        if (showImage)
        {
            // user has rights to download, show him image
            if (PageContext.BoardSettings.EnableImageAttachmentResize)
            {
                stringBuilder.Append(
                    $"""<div class="card text-bg-dark" style="max-width:{PageContext.BoardSettings.ImageThumbnailMaxWidth}px">""");

                stringBuilder.AppendFormat(
                    """<a href="{0}" title="{1}" data-toggle="lightbox" data-gallery="gallery-{2}">""",
                    this.Get<IUrlHelper>().Action("GetAttachment", "Attachments", new { attachmentId = attachment.ID, editor = false }),
                    HtmlEncode(attachment.FileName),
                    this.MessageID.Value);

                stringBuilder.AppendFormat(
                    """<img src="{0}" alt="{1}" class="img-user-posted card-img-top" style="max-height:{2}px;max-width:{3}px;object-fit:contain">""",
                    this.Get<IUrlHelper>().Action("GetAttachment", "Attachments", new { attachmentId = attachment.ID, editor = true }),
                    HtmlEncode(attachment.FileName),
                    PageContext.BoardSettings.ImageThumbnailMaxHeight,
                    PageContext.BoardSettings.ImageThumbnailMaxWidth);

                stringBuilder.Append("</a>");

                stringBuilder.Append(
                    $"""<div class="card-body py-1"><p class="card-text small">{this.GetText("IMAGE_RESIZE_ENLARGE")}""");

                stringBuilder.Append(
                    $"""<span class="text-body-secondary float-end">{this.GetTextFormatted("IMAGE_RESIZE_VIEWS", attachment.Downloads)}</span></p>""");

                stringBuilder.Append("</div></div>");
            }
            else
            {
                stringBuilder.AppendFormat(
                    """<img src="{0}" alt="{1}" class="img-user-posted img-thumbnail" style="max-height:{2}px">""",
                    this.Get<IUrlHelper>().Action("GetAttachment", "Attachments", new { attachmentId = attachment.ID, editor = true }),
                    HtmlEncode(attachment.FileName),
                    PageContext.BoardSettings.ImageThumbnailMaxHeight);
            }
        }
        else
        {
            // regular file attachment
            var kb = (1023 + attachment.Bytes.ToType<int>()) / 1024;

            stringBuilder.AppendFormat(
                """
                <i class="fa fa-file"></i>&nbsp;
                                         <a href="{0}">{1}</a>
                                         <span>{2}</span>
                """,
                this.Get<IUrlHelper>().Action("GetAttachment", "Attachments", new { attachmentId = attachment.ID, editor = true }),
                attachment.FileName,
                this.GetTextFormatted("ATTACHMENTINFO", kb, attachment.Downloads));
        }

        return Task.CompletedTask;
    }
}