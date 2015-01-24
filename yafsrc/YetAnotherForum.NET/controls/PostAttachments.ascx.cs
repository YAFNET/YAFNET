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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Text;

    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The post options.
    /// </summary>
    public partial class PostAttachments : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            this.LoadingImage.ImageUrl = YafForumInfo.GetURLToContent("images/loader.gif");
            this.LoadingImage.AlternateText = this.Get<ILocalization>().GetText("COMMON", "LOADING");
            this.LoadingImage.ToolTip = this.Get<ILocalization>().GetText("COMMON", "LOADING");

            this.LoadingText.Text = this.Get<ILocalization>().GetText("COMMON", "LOADING");

            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.CreateAttachments();
        }

        /// <summary>
        /// Creates the attachments.
        /// </summary>
        private void CreateAttachments()
        {
            var html = new StringBuilder();

            html.Append("<div class=\"result\">");
            html.AppendFormat("<ul class=\"PostAttachmentList\">");

            var attachments = this.GetRepository<Attachment>()
                .ListTyped(userID: this.PageContext.PageUserID, pageIndex: 0, pageSize: 10000);

            var rowPanel = 0;

            foreach (var attachment in attachments)
            {
                if (rowPanel == 5 && (rowPanel + 1) < attachments.Count)
                {
                    html.Append("</ul>");
                    html.Append("</div>");
                    html.AppendFormat("<strong>{0}</strong>", this.GetText("ATTACHMENTS", "CURRENT_UPLOADS"));
                    html.Append("<div class=\"result\">");

                    html.Append("<ul class=\"PostAttachmentList\">");

                    rowPanel = 0;
                }

                rowPanel++;

                var url = attachment.FileName.IsImageName()
                              ? "{0}resource.ashx?i={1}&editor=true".FormatWith(
                                  YafForumInfo.ForumClientFileRoot,
                                  attachment.ID)
                              : "{0}Images/document.png".FormatWith(YafForumInfo.ForumClientFileRoot);

                var onClick = "insertAttachment('{0}', '{1}')".FormatWith(attachment.ID, url);

                var iconImage =
                    @"<img class=""popupitemIcon"" src=""{0}"" alt=""{1}"" title=""{1}"" /><span>{1}</span>".FormatWith(
                        url,
                        "{0} ({1}kb)".FormatWith(attachment.FileName, attachment.Bytes / 1024));

                html.AppendFormat(
                    @"<li class=""popupitem AttachmentItem"" onmouseover=""mouseHover(this,true)"" onmouseout=""mouseHover(this,false)"" onclick=""{2}"" style=""white-space:nowrap"" title=""{1}""{3}>{0}</li>",
                    iconImage,
                    attachment.FileName,
                    onClick,
                    attachment.FileName.IsImageName() ? " data-url=\"{0}\"".FormatWith(url) : string.Empty);
            }

            html.Append("</ul>");
            html.Append("</div>");

            html.Append("</ul></div>");

            this.AttachmentsResults.Text = html.ToString();
        }

        #endregion
    }
}