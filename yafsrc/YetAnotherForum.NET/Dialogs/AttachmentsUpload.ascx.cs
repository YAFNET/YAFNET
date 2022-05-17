/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
namespace YAF.Dialogs;

/// <summary>
/// The Attachments Upload Dialog.
/// </summary>
public partial class AttachmentsUpload : BaseUserControl
{
    /// <summary>
    /// Handles the PreRender event
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Setup Hover Card JS
        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.FileUploadLoadJs),
            JavaScriptBlocks.FileUploadLoadJs(
                this.PageBoardContext.BoardSettings.AllowedFileExtensions.Replace(",", "|"),
                this.PageBoardContext.BoardSettings.MaxFileSize,
                $"{BoardInfo.ForumClientFileRoot}FileUploader.ashx",
                this.PageBoardContext.BoardSettings.ImageAttachmentResizeWidth,
                this.PageBoardContext.BoardSettings.ImageAttachmentResizeHeight));
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        // show disallowed or allowed localized text depending on the Board Setting
        this.ExtensionTitle.LocalizedTag = "ALLOWED_EXTENSIONS";

        this.ExtensionsList.Text = this.PageBoardContext.BoardSettings.AllowedFileExtensions.Replace(",", ", ");

        if (this.PageBoardContext.BoardSettings.MaxFileSize > 0)
        {
            this.UploadNodePlaceHold.Visible = true;
            this.UploadNote.Text = this.GetTextFormatted(
                "UPLOAD_NOTE",
                (this.PageBoardContext.BoardSettings.MaxFileSize / 1024).ToString());
        }
        else
        {
            this.UploadNodePlaceHold.Visible = false;
        }
    }
}