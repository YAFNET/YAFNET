/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Text;
    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
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
    /// Admin Banned ip edit/add page.
    /// </summary>
    public partial class bannedip_edit : AdminPage
    {
        #region Methods

        /// <summary>
        /// The cancel_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_bannedip);
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_BANNEDIP", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_bannedip));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_BANNEDIP_EDIT", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
               this.GetText("ADMIN_ADMIN", "Administration"),
               this.GetText("ADMIN_BANNEDIP", "TITLE"),
               this.GetText("ADMIN_BANNEDIP_EDIT", "TITLE"));

            this.save.Text = this.GetText("COMMON", "SAVE");
            this.cancel.Text = this.GetText("COMMON", "CANCEL");

            this.BindData();
        }

        /// <summary>
        /// The save_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            string[] ipParts = this.mask.Text.Trim().Split('.');

            // do some validation...
            var ipError = new StringBuilder();

            if (ipParts.Length != 4)
            {
                ipError.AppendLine(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_ADRESS"));
            }

            foreach (string ip in ipParts)
            {
                // see if they are numbers...
                ulong number;
                if (!ulong.TryParse(ip, out number))
                {
                    if (ip.Trim() != "*")
                    {
                        if (ip.Trim().Length == 0)
                        {
                            ipError.AppendLine(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_VALUE"));
                        }
                        else
                        {
                            ipError.AppendFormat(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_SECTION"), ip);
                        }

                        break;
                    }
                }
                else
                {
                    // try parse succeeded... verify number amount...
                    if (number > 255)
                    {
                        ipError.AppendFormat(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_LESS"), ip);
                    }
                }
            }

            // show error(s) if not valid...
            if (ipError.Length > 0)
            {
                this.PageContext.AddLoadMessage(ipError.ToString());
                return;
            }

            this.GetRepository<BannedIP>().Save(
                this.Request.QueryString.GetFirstOrDefaultAs<int>("i"), this.mask.Text.Trim(), this.BanReason.Text.Trim(), this.PageContext.PageUserID);

            this.Get<ILogger>()
                .Log(
                    this.PageContext.PageUserID,
                    "YAF.Pages.Admin.bannedip_edit",
                    "IP or mask {0} was saved by {1}.".FormatWith(
                        this.mask.Text.Trim(),
                        this.Get<YafBoardSettings>().EnableDisplayName
                            ? this.PageContext.CurrentUserData.DisplayName
                            : this.PageContext.CurrentUserData.UserName),
                    EventLogTypes.IpBanSet);

            // go back to banned IP's administration page
            YafBuildLink.Redirect(ForumPages.admin_bannedip);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            if (this.Request.QueryString.GetFirstOrDefault("i") == null)
            {
                return;
            }

            DataRow row = this.GetRepository<BannedIP>().List(this.Request.QueryString.GetFirstOrDefaultAs<int>("i"), null, null).Rows[0];

            this.mask.Text = row["Mask"].ToString();
            this.BanReason.Text = row["Reason"].ToString();
        }

        #endregion
    }
}