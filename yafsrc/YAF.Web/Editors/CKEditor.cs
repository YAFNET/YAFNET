/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Web.Editors
{
    #region Using

    using System;

    using YAF.Configuration;
    using YAF.Core.Context;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The CKEditor.
    /// </summary>
    public abstract class CKEditor : TextEditor
    {
        #region Properties

        /// <summary>
        ///   Gets or sets Text.
        /// </summary>
        public override string Text
        {
            get => this.TextAreaControl.InnerText;

            set => this.TextAreaControl.InnerText = value;
        }

        /// <summary>
        ///   Gets SafeID.
        /// </summary>
        [NotNull]
        protected string SafeID => this.TextAreaControl.ClientID.Replace("$", "_");

        #endregion

        #region Methods

        /// <summary>
        /// Handles the PreRender event of the Editor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override void Editor_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            BoardContext.Current.PageElements.AddScriptReference("ckeditor", "ckeditor/ckeditor.js");

            BoardContext.Current.PageElements.AddScriptReference(
                "ckeditor-jQuery-Adapter",
                "ckeditor/adapters/jquery.js");

            if (this.UserCanUpload && this.AllowsUploads)
            {
                BoardContext.Current.PageElements.RegisterJsBlock(
                    "autoUpload",
                    JavaScriptBlocks.FileAutoUploadLoadJs(
                        this.Get<BoardSettings>().AllowedFileExtensions.Replace(",", "|"),
                        this.Get<BoardSettings>().MaxFileSize,
                        $"{BoardInfo.ForumClientFileRoot}FileUploader.ashx",
                        this.PageContext.PageForumID,
                        this.PageContext.PageBoardID,
                        this.Get<BoardSettings>().ImageAttachmentResizeWidth,
                        this.Get<BoardSettings>().ImageAttachmentResizeHeight,
                        this.TextAreaControl.ClientID));
            }

            this.RegisterCKEditorCustomJS();

            // register custom BBCode javascript (if there is any)
            // this call is supposed to be after editor load since it may use
            // JS variables created in editor_load...
            this.Get<IBBCode>().RegisterCustomBBCodePageElements(this.Page, this.GetType(), this.SafeID);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            base.OnInit(e);
        }

        /// <summary>
        /// Register the custom CKEditor java script.
        /// </summary>
        protected abstract void RegisterCKEditorCustomJS();

        #endregion
    }
}