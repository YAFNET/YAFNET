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

namespace YAF.Dialogs
{
    #region Using

    using System;

    using YAF.Core;
    using YAF.Core.BaseControls;
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
    /// The Admin Banned Email Add/Edit Dialog.
    /// </summary>
    public partial class BannedEmailEdit : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Gets or sets the banned identifier.
        /// </summary>
        /// <value>
        /// The banned identifier.
        /// </value>
        public int? BannedId
        {
            get => this.ViewState[key: "BannedId"].ToType<int?>();

            set => this.ViewState[key: "BannedId"] = value;
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="bannedId">The banned identifier.</param>
        public void BindData(int? bannedId)
        {
            this.BannedId = bannedId;

            this.Title.LocalizedPage = "ADMIN_BANNEDEMAIL_EDIT";
            this.Save.TextLocalizedPage = "ADMIN_BANNEDEMAIL";

            if (this.BannedId.HasValue)
            {
                // Edit
                var banned = this.GetRepository<BannedEmail>().GetById(id: this.BannedId.Value);

                if (banned != null)
                {
                    this.mask.Text = banned.Mask;
                    this.BanReason.Text = banned.Reason;
                }

                this.Title.LocalizedTag = "TITLE_EDIT";
                this.Save.TextLocalizedTag = "SAVE";
            }
            else
            {
                // Add
                this.mask.Text = string.Empty;
                this.BanReason.Text = string.Empty;

                this.Title.LocalizedTag = "TITLE";
                this.Save.TextLocalizedTag = "ADD_IP";
            }
        }

        /// <summary>
        /// Handles the Click event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Save_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.mask.Text.IsSet())
            {
                this.GetRepository<BannedEmail>().Save(
                    id: this.BannedId,
                    mask: this.mask.Text.Trim(),
                    reason: this.BanReason.Text.Trim());

                // go back to banned IP's administration page
                YafBuildLink.Redirect(page: ForumPages.admin_bannedemail);
            }
        }

        #endregion
    }
}