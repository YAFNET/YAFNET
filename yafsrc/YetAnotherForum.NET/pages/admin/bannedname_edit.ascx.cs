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
    using System.Linq;
    using System.Text;

    using YAF.Classes;
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
    /// Admin Banned Name edit/add page.
    /// </summary>
    public partial class bannedname_edit : AdminPage
    {
        #region Methods

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_bannedname);
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

            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(
                this.GetText("ADMIN_BANNEDNAME", "TITLE"),
                YafBuildLink.GetLink(ForumPages.admin_bannedname));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_BANNEDNAME_EDIT", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_BANNEDNAME", "TITLE"),
                this.GetText("ADMIN_BANNEDNAME_EDIT", "TITLE"));

            this.save.Text = this.GetText("COMMON", "SAVE");
            this.cancel.Text = this.GetText("COMMON", "CANCEL");

            this.BindData();
        }

        /// <summary>
        /// Handles the Click event of the Save control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.mask.Text.Trim().IsNotSet())
            {
                this.BindData();
            }
            else
            {
                this.GetRepository<BannedName>()
                    .Save(
                        this.Request.QueryString.GetFirstOrDefaultAs<int>("i"),
                        this.mask.Text.Trim(),
                        this.BanReason.Text.Trim());

                // go back to banned IP's administration page
                YafBuildLink.Redirect(ForumPages.admin_bannedname);
            }
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            if (this.Request.QueryString.GetFirstOrDefault("i") == null)
            {
                return;
            }

            var name =
                this.GetRepository<BannedName>()
                    .ListTyped(id: this.Request.QueryString.GetFirstOrDefaultAs<int>("i"))
                    .FirstOrDefault();

            this.mask.Text = name.Mask;
            this.BanReason.Text = name.Reason;
        }

        #endregion
    }
}