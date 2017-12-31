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
namespace YAF.Dialogs
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utilities;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Attachments Upload Dialog.
    /// </summary>
    public partial class AttachmentsUpload : BaseUserControl
    {
        /// <summary>
        /// Gets the file extensions.
        /// </summary>
        /// <value>
        /// The file extensions.
        /// </value>
        private IEnumerable<FileExtension> FileExentsions
        {
            get
            {
                return this.GetRepository<FileExtension>().Get(e => e.BoardId == this.PageContext.PageBoardID);
            }
        }

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.PreRender += this.AttachmentsUploadDialogPreRender;
            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // show disallowed or allowed localized text depending on the Board Setting
            this.ExtensionTitle.LocalizedTag = this.Get<YafBoardSettings>().FileExtensionAreAllowed
                                                   ? "ALLOWED_EXTENSIONS"
                                                   : "DISALLOWED_EXTENSIONS";

            var types = string.Empty;
            var first = true;

            foreach (var extension in this.FileExentsions)
            {
                types += "{1}*.{0}".FormatWith(extension.Extension, first ? string.Empty : ", ");

                if (first)
                {
                    first = false;
                }
            }

            if (types.IsSet())
            {
                this.ExtensionsList.Text = types;
            }

            if (this.Get<YafBoardSettings>().MaxFileSize > 0)
            {
                this.UploadNodePlaceHold.Visible = true;
                this.UploadNote.Text = this.GetTextFormatted(
                    "UPLOAD_NOTE",
                    (this.Get<YafBoardSettings>().MaxFileSize / 1024).ToString());
            }
            else
            {
                this.UploadNodePlaceHold.Visible = false;
            }
        }

        /// <summary>
        /// Handles the PreRender event of the AttachmentsUploadDialog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void AttachmentsUploadDialogPreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Setup Hover Card JS
            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "fileUploadjs",
                JavaScriptBlocks.FileUploadLoadJs(
                    string.Join("|", this.FileExentsions.Select(ext => ext.Extension)),
                    this.Get<YafBoardSettings>().MaxFileSize,
                    "{0}YafUploader.ashx".FormatWith(YafForumInfo.ForumClientFileRoot),
                    this.PageContext.PageForumID,
                    this.PageContext.PageBoardID));
        }

        #endregion
    }
}