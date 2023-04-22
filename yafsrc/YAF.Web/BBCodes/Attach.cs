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

namespace YAF.Web.BBCodes;

/// <summary>
/// The Attachment BB Code Module.
/// </summary>
public class Attach : BBCodeControl
{
    /// <summary>
    /// Render The Album Image as Link with Image
    /// </summary>
    /// <param name="writer">The writer.</param>
    protected override void Render(HtmlTextWriter writer)
    {
        var attachId = HtmlTagHelper.StripHtml(this.Parameters["inner"]);

        if (!ValidationHelper.IsNumeric(attachId))
        {
            return;
        }

        var attachment = this.GetRepository<Attachment>().GetById(attachId.ToType<int>());

        if (attachment == null)
        {
            return;
        }

        var filename = attachment.FileName.ToLower();
        var showImage = false;

        // verify it's not too large to display
        // Ederon : 02/17/2009 - made it board setting
        if (attachment.Bytes.ToType<int>() <= this.PageBoardContext.BoardSettings.PictureAttachmentDisplayTreshold)
        {
            // is it an image file?
            showImage = filename.IsImageName();
        }

        // user doesn't have rights to download, don't show the image
        if (!this.PageBoardContext.DownloadAccess)
        {
            writer.Write(
                @"<i class=""fa fa-file fa-fw""></i>&nbsp;{0} <span class=""badge bg-warning text-dark"" role=""alert"">{1}</span>",
                attachment.FileName,
                this.GetText("ATTACH_NO"));

            return;
        }

        if (showImage)
        {
            // user has rights to download, show him image
            if (this.PageBoardContext.BoardSettings.EnableImageAttachmentResize)
            {
                writer.Write(
                    @"<div class=""card bg-dark text-white"" style=""max-width:{0}px;"">",
                    this.PageBoardContext.BoardSettings.ImageThumbnailMaxWidth);

                writer.Write(
                    @"<a href=""{0}resource.ashx?i={1}&b={3}"" title=""{2}""  data-gallery=""#blueimp-gallery-{4}"">",
                    BoardInfo.ForumClientFileRoot,
                    attachment.ID,
                    this.HtmlEncode(attachment.FileName),
                    this.PageBoardContext.PageBoardID,
                    this.MessageID.Value);

                writer.Write(
                    @"<img src=""{0}resource.ashx?p={1}&b={3}"" alt=""{2}"" class=""img-user-posted card-img-top"" style=""max-height:{4}px;object-fit:contain"">",
                    BoardInfo.ForumClientFileRoot,
                    attachment.ID,
                    this.HtmlEncode(attachment.FileName),
                    this.PageBoardContext.PageBoardID,
                    this.PageBoardContext.BoardSettings.ImageThumbnailMaxHeight);

                writer.Write(@"</a>");

                writer.Write(
                    @"<div class=""card-body py-1""><p class=""card-text small"">{0}",
                    this.GetText("IMAGE_RESIZE_ENLARGE"));

                writer.Write(
                    @"<span class=""text-muted float-end"">{0}</span></p>",
                    this.GetTextFormatted("IMAGE_RESIZE_VIEWS", attachment.Downloads));

                writer.Write(@"</div></div>");
            }
            else
            {
                writer.Write(
                    @"<img src=""{0}resource.ashx?a={1}&b={3}"" alt=""{2}"" class=""img-user-posted img-thumbnail"" style=""max-height:{4}px;object-fit:contain"">",
                    BoardInfo.ForumClientFileRoot,
                    attachment.ID,
                    this.HtmlEncode(attachment.FileName),
                    this.PageBoardContext.PageBoardID,
                    this.PageBoardContext.BoardSettings.ImageThumbnailMaxHeight);
            }
        }
        else
        {
            // regular file attachment
            var kb = (1023 + attachment.Bytes.ToType<int>()) / 1024;

            writer.Write(
                @"<i class=""fa fa-file fa-fw""></i>&nbsp;
                         <a href=""{0}resource.ashx?a={1}&b={4}"">{2}</a>
                         <span>{3}</span>",
                BoardInfo.ForumClientFileRoot,
                attachment.ID,
                attachment.FileName,
                this.GetTextFormatted("ATTACHMENTINFO", kb, attachment.Downloads),
                this.PageBoardContext.PageBoardID);
        }
    }
}