/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
    using System.Web;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

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
            if (!ValidationHelper.IsNumeric(valueToCheck: this.Parameters[key: "inner"]))
            {
                return;
            }

            var attachment = this.GetRepository<Attachment>().GetById(id: this.Parameters[key: "inner"].ToType<int>());

            if (attachment == null)
            {
                return;
            }

            var stats = this.GetText(tag: "ATTACHMENTINFO");
            var filename = attachment.FileName.ToLower();
            var showImage = false;

            var session = this.Get<HttpSessionStateBase>();
            var settings = this.Get<YafBoardSettings>();

            if (session[name: "imagePreviewWidth"] == null)
            {
                session[name: "imagePreviewWidth"] = settings.ImageAttachmentResizeWidth;
            }

            if (session[name: "imagePreviewHeight"] == null)
            {
                session[name: "imagePreviewHeight"] = settings.ImageAttachmentResizeHeight;
            }

            if (session[name: "imagePreviewCropped"] == null)
            {
                session[name: "imagePreviewCropped"] = settings.ImageAttachmentResizeCropped;
            }

            if (session[name: "localizationFile"] == null)
            {
                session[name: "localizationFile"] = this.Get<ILocalization>().LanguageFileName;
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
                        format: !this.Get<YafBoardSettings>().EnableImageAttachmentResize
                            ? @"<img src=""{0}resource.ashx?a={1}&b={3}"" alt=""{2}"" class=""UserPostedImage attachedImage"" />"
                            : @"<a href=""{0}resource.ashx?i={1}&b={3}"" class=""attachedImage"" data-gallery><img src=""{0}resource.ashx?p={1}&b={3}"" alt=""{2}"" title=""{2}"" /></a>",
                        YafForumInfo.ForumClientFileRoot,
                        attachment.ID,
                        this.HtmlEncode(data: attachment.FileName),
                        this.PageContext.PageBoardID);
                }
                else
                {
                    // user doesn't have rights to download, don't show the image
                    writer.Write(
                        format: @"<i class=""fa fa-file fa-fw""></i>&nbsp;{0} <span class=""badge badge-warning"" role=""alert"">{1}</span>",
                        arg0: attachment.FileName,
                        arg1: this.GetText(tag: "ATTACH_NO"));
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
                        format: @"<i class=""fa fa-file fa-fw""></i>&nbsp;<a class=""attachedImageLink {{html:false,image:false,video:false}}"" href=""{0}resource.ashx?a={1}&b={4}"">{2}</a> <span class=""attachmentinfo"">{3}</span>",
                        YafForumInfo.ForumClientFileRoot,
                        attachment.ID,
                        attachment.FileName,
                        string.Format(format: stats, arg0: kb,arg1: attachment.Downloads),
                        this.PageContext.PageBoardID);
                }
                else
                {
                    writer.Write(
                        format: @"<i class=""fa fa-file fa-fw""></i>&nbsp;{0} <span class=""badge badge-warning"" role=""alert"">{1}</span>",
                        arg0: attachment.FileName,
                        arg1: this.GetText(tag: "ATTACH_NO"));
                }
            }
        }
    }
}