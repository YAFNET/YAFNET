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

    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Admin Extensions Add/Edit Dialog.
    /// </summary>
    public partial class ExtensionsEdit : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Gets or sets the extension identifier.
        /// </summary>
        /// <value>
        /// The extension identifier.
        /// </value>
        public int? ExtensionId
        {
            get
            {
                return this.ViewState["ExtensionId"].ToType<int?>();
            }

            set
            {
                this.ViewState["ExtensionId"] = value;
            }
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="extensionId">The extension identifier.</param>
        public void BindData(int? extensionId)
        {
            this.ExtensionId = extensionId;

            this.Title.LocalizedPage = "ADMIN_EXTENSIONS_EDIT";
            this.Save.TextLocalizedPage = "ADMIN_EXTENSIONS";

            if (this.ExtensionId.HasValue)
            {
                // Edit
                var exten = this.GetRepository<FileExtension>().GetById(this.ExtensionId.Value);

                if (exten != null)
                {
                    this.extension.Text = exten.Extension;
                }

                this.Title.LocalizedTag = "TITLE_EDIT";
                this.Save.TextLocalizedTag = "SAVE";
            }
            else
            {
                // Add
                this.extension.Text = string.Empty;

                this.Title.LocalizedTag = "TITLE";
                this.Save.TextLocalizedTag = "ADD";
            }
        }

        /// <summary>
        /// Checks if its a valid extension
        /// </summary>
        /// <param name="newExtension">
        /// The new extension.
        /// </param>
        /// <returns>
        /// The is valid extension.
        /// </returns>
        protected bool IsValidExtension([NotNull] string newExtension)
        {
            if (newExtension.IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EXTENSIONS_EDIT", "MSG_ENTER"));
                return false;
            }

            if (newExtension.IndexOf('.') != -1)
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EXTENSIONS_EDIT", "MSG_REMOVE"));
                return false;
            }

            // TODO: maybe check for duplicate?
            return true;
        }

        /// <summary>
        /// Handles the Click event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Save_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            var fileExtension = this.extension.Text.Trim();

            if (this.IsValidExtension(fileExtension))
            {
                this.GetRepository<FileExtension>().Save(
                    this.Request.QueryString.GetFirstOrDefaultAs<int?>("i") ?? 0,
                    fileExtension,
                    this.PageContext.PageBoardID);

                YafBuildLink.Redirect(ForumPages.admin_extensions);
            }
        }

        #endregion
    }
}