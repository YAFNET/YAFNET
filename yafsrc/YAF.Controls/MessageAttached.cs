/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The message attached.
    /// </summary>
    public class MessageAttached : BaseControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The _user name.
        /// </summary>
        private string _userName = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets MessageID.
        /// </summary>
        public int MessageID { get; set; }

        /// <summary>
        ///   Gets or sets UserName.
        /// </summary>
        public string UserName
        {
            get
            {
                return this._userName;
            }

            set
            {
                this._userName = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            writer.BeginRender();
            writer.WriteBeginTag("div");
            writer.WriteAttribute("id", this.ClientID);
            writer.Write(HtmlTextWriter.TagRightChar);

            if (this.MessageID != 0)
            {
                this.RenderAttachedFiles(writer);
            }

            base.Render(writer);

            writer.WriteEndTag("div");
            writer.EndRender();
        }

        /// <summary>
        /// The render attached files.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected void RenderAttachedFiles([NotNull] HtmlTextWriter writer)
        {
            var stats = this.GetText("ATTACHMENTINFO");
            var fileIcon = this.Get<ITheme>().GetItem("ICONS", "ATTACHED_FILE");

            var settings = this.Get<YafBoardSettings>();

            var attachments = this.GetRepository<Attachment>().Get(a => a.MessageID == this.MessageID);

            // show file then image attachments...
            var tmpDisplaySort = 0;

            writer.Write(@"<div class=""fileattach smallfont"">");

            while (tmpDisplaySort <= 1)
            {
                var firstItem = true;

                foreach (var attachment in attachments)
                {
                    var filename = attachment.FileName.ToLower();
                    var showImage = false;

                    // verify it's not too large to display
                    // Ederon : 02/17/2009 - made it board setting
                    if (attachment.Bytes.ToType<int>() <= settings.PictureAttachmentDisplayTreshold)
                    {
                        // is it an image file?
                        showImage = filename.IsImageName();
                    }

                    if (showImage && tmpDisplaySort == 1)
                    {
                        if (firstItem)
                        {
                            writer.Write(@"<div class=""imgtitle"">");

                            writer.Write(
                                this.GetText("IMAGE_ATTACHMENT_TEXT"),
                                this.HtmlEncode(Convert.ToString(this.UserName)));

                            writer.Write("</div>");

                            firstItem = false;
                        }

                        // Ederon : download rights
                        if (this.PageContext.ForumDownloadAccess || this.PageContext.ForumModeratorAccess)
                        {
                            // user has rights to download, show him image
                            if (!settings.EnableImageAttachmentResize)
                            {
                                writer.Write(
                                    @"<div class=""attachedimg""><img src=""{0}resource.ashx?a={1}&b={3}"" alt=""{2}"" /></div>",
                                    YafForumInfo.ForumClientFileRoot,
                                    attachment.ID,
                                    this.HtmlEncode(attachment.FileName),
                                    settings.BoardID);
                            }
                            else
                            {
                                var attachFilesText =
                                    "{0} {1}".FormatWith(
                                        this.GetText("IMAGE_ATTACHMENT_TEXT")
                                            .FormatWith(this.HtmlEncode(Convert.ToString(this.UserName))),
                                        this.HtmlEncode(attachment.FileName));

                                // TommyB: Start MOD: Preview Images
                                writer.Write(
                                    @"<div class=""attachedimg"" style=""display: inline;""><a href=""{0}resource.ashx?i={1}&b={4}"" title=""{2}"" title=""{2}"" data-gallery><img src=""{0}resource.ashx?p={1}&b={4}"" alt=""{3}"" title=""{2}"" /></a></div>",
                                    YafForumInfo.ForumClientFileRoot,
                                    attachment.ID,
                                    attachFilesText,
                                    this.HtmlEncode(attachment.FileName),
                                    settings.BoardID);

                                // TommyB: End MOD: Preview Images
                            }
                        }
                        else
                        {
                            var kb = (1023 + attachment.Bytes.ToType<int>()) / 1024;

                            // user doesn't have rights to download, don't show the image
                            writer.Write(@"<div class=""attachedfile"">");

                            writer.Write(
                                @"<img border=""0"" alt="""" src=""{0}"" /> {1} <span class=""attachmentinfo"">{2}</span>",
                                fileIcon,
                                attachment.FileName,
                                stats.FormatWith(kb, attachment.Downloads));

                            writer.Write(@"</div>");
                        }
                    }
                    else if (!showImage && tmpDisplaySort == 0)
                    {
                        if (firstItem)
                        {
                            writer.Write(@"<div class=""filetitle"">{0}</div>", this.GetText("ATTACHMENTS"));
                            firstItem = false;
                        }

                        // regular file attachment
                        var kb = (1023 + attachment.Bytes.ToType<int>()) / 1024;

                        writer.Write(@"<div class=""attachedfile"">");

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
                                settings.BoardID);
                        }
                        else
                        {
                            writer.Write(
                                @"<img border=""0"" alt="""" src=""{0}"" /> {1} <span class=""attachmentinfo"">{2}</span>",
                                fileIcon,
                                attachment.FileName,
                                stats.FormatWith(kb, attachment.Downloads));
                        }

                        writer.Write(@"</div>");
                    }
                }

                // now show images
                tmpDisplaySort++;
            }

            if (!this.PageContext.ForumDownloadAccess)
            {
                writer.Write(@"<br /><div class=""attachmentinfo"">");

                writer.Write(
                    this.PageContext.IsGuest
                        ? this.GetText("POSTS", "CANT_DOWNLOAD_REGISTER")
                        : this.GetText("POSTS", "CANT_DOWNLOAD"));

                writer.Write(@"</div>");
            }

            writer.Write(@"</div>");
        }

        #endregion
    }
}