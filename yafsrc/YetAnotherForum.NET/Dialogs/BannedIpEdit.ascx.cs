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

namespace YAF.Dialogs
{
    #region Using

    using System;
    using System.Text;

    using YAF.Configuration;
    
    using YAF.Core.BaseControls;
    using YAF.Core.Context;
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
    /// The Admin Banned IP Add/Edit Dialog.
    /// </summary>
    public partial class BannedIpEdit : BaseUserControl
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
            get => this.ViewState["BannedId"].ToType<int?>();

            set => this.ViewState["BannedId"] = value;
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="bannedId">The banned identifier.</param>
        public void BindData(int? bannedId)
        {
            this.BannedId = bannedId;

            this.Title.LocalizedPage = "ADMIN_BANNEDIP_EDIT";
            this.Save.TextLocalizedPage = "ADMIN_BANNEDIP";

            if (this.BannedId.HasValue)
            {
                // Edit
                var banned = this.GetRepository<BannedIP>().GetById(this.BannedId.Value);

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
            var ipParts = this.mask.Text.Trim().Split('.');

            // do some validation...
            var ipError = new StringBuilder();

            if (ipParts.Length != 4)
            {
                ipError.AppendLine(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_ADRESS"));
            }

            foreach (var ip in ipParts)
            {
                // see if they are numbers...
                if (!ulong.TryParse(ip, out var number))
                {
                    if (ip.Trim() == "*")
                    {
                        continue;
                    }

                    if (ip.Trim().Length != 0)
                    {
                        ipError.AppendFormat(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_SECTION"), ip);
                    }
                    else
                    {
                        ipError.AppendLine(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_VALUE"));
                    }

                    break;
                }

                // try parse succeeded... verify number amount...
                if (number > 255)
                {
                    ipError.AppendFormat(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_LESS"), ip);
                }
            }

            // show error(s) if not valid...
            if (ipError.Length > 0)
            {
                this.PageContext.AddLoadMessage(ipError.ToString(), MessageTypes.warning);
                return;
            }

            this.GetRepository<BannedIP>().Save(
                this.BannedId,
                this.mask.Text.Trim(),
                this.BanReason.Text.Trim(),
                this.PageContext.PageUserID);

            if (BoardContext.Current.Get<BoardSettings>().LogBannedIP)
            {
                this.Logger.Log(
                    $"IP or mask {this.mask.Text.Trim()} was saved by {(this.Get<BoardSettings>().EnableDisplayName ? this.PageContext.CurrentUserData.DisplayName : this.PageContext.CurrentUserData.UserName)}.",
                    EventLogTypes.IpBanSet);
            }

            // go back to banned IP's administration page
            BuildLink.Redirect(ForumPages.Admin_BannedIps);
        }

        #endregion
    }
}