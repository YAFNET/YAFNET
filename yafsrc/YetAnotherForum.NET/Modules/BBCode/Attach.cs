/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
 * http://www.yetanotherforum.net/
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
namespace YAF.Modules.BBCode
{
    using System.Linq;
    using System.Web;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    /// <summary>
    /// The Attachment BB Code Module.
    /// </summary>
    public class Attach : YafBBCodeControl
    {
        /// <summary>
        /// Render The Album Image as Link with Image
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            var attachment =
                this.GetRepository<Attachment>()
                    .ListTyped(attachmentID: this.Parameters["inner"].ToType<int>())
                    .FirstOrDefault();

            if (attachment == null)
            {
                return;
            }

            var stats = this.GetText("ATTACHMENTINFO");
            var fileIcon = this.Get<ITheme>().GetItem("ICONS", "ATTACHED_FILE");
            var filename = attachment.FileName.ToLower();
            var showImage = false;

            var session = this.Get<HttpSessionStateBase>();
            var settings = this.Get<YafBoardSettings>();

            if (session["imagePreviewWidth"] == null)
            {
                session["imagePreviewWidth"] = settings.ImageAttachmentResizeWidth;
            }

            if (session["imagePreviewHeight"] == null)
            {
                session["imagePreviewHeight"] = settings.ImageAttachmentResizeHeight;
            }

            if (session["imagePreviewCropped"] == null)
            {
                session["imagePreviewCropped"] = settings.ImageAttachmentResizeCropped;
            }

            if (session["localizationFile"] == null)
            {
                session["localizationFile"] = this.Get<ILocalization>().LanguageFileName;
            }

            // verify it's not too large to display
            // Ederon : 02/17/2009 - made it board setting
            if (attachment.Bytes.ToType<int>() <= this.Get<YafBoardSettings>().PictureAttachmentDisplayTreshold)
            {
                // is it an image file?
                showImage = filename.IsImageName();
            }

            if (showImage)
            {
                // Ederon : download rights
                if (this.PageContext.ForumDownloadAccess || this.PageContext.ForumModeratorAccess)
                {
                    // user has rights to download, show him image
                    writer.Write(
                        !this.Get<YafBoardSettings>().EnableImageAttachmentResize
                            ? @"<img src=""{0}resource.ashx?a={1}&b={3}"" alt=""{2}"" class=""UserPostedImage attachedImage"" />"
                            : @"<a href=""{0}resource.ashx?i={1}&b={3}"" date-img=""{0}resource.ashx?a={1}&b={3}"" class=""attachedImage""><img src=""{0}resource.ashx?p={1}&b={3}"" alt=""{2}"" title=""{2}"" /></a>",
                        YafForumInfo.ForumClientFileRoot,
                        attachment.ID,
                        this.HtmlEncode(attachment.FileName),
                        this.PageContext.PageBoardID);
                }
                else
                {
                    var kb = (1023 + attachment.Bytes.ToType<int>()) / 1024;

                    // user doesn't have rights to download, don't show the image
                    writer.Write(
                        @"<img border=""0"" alt="""" src=""{0}"" /> {1} <span class=""attachmentinfo"">{2}</span>",
                        fileIcon,
                        attachment.FileName,
                        stats.FormatWith(kb, attachment.Downloads));
                }
            }
            else
            {
                // regular file attachment
                var kb = (1023 + attachment.Bytes.ToType<int>()) / 1024;

                // Ederon : download rights
                if (this.PageContext.ForumDownloadAccess || this.PageContext.ForumModeratorAccess)
                {
                    writer.Write(
                        @"<img border=""0"" alt="""" src=""{0}"" /> <a class=""attachedImageLink {{html:false,image:false,video:false}}"" href=""{1}resource.ashx?a={2}&b={5}"">{3}</a> <span class=""attachmentinfo"">{4}</span>",
                        fileIcon,
                        YafForumInfo.ForumClientFileRoot,
                        attachment.ID,
                        attachment.FileName,
                        stats.FormatWith(kb, attachment.Downloads),
                        this.PageContext.PageBoardID);
                }
                else
                {
                    writer.Write(
                        @"<img border=""0"" alt="""" src=""{0}"" /> {1} <span class=""attachmentinfo"">{2}</span>",
                        fileIcon,
                        attachment.FileName,
                        stats.FormatWith(kb, attachment.Downloads));
                }
            }
        }
    }
}