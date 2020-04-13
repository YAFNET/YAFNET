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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Admin NNTP server page
    /// </summary>
    public partial class NntpServers : AdminPage
    {
        #region Methods

        /// <summary>
        /// News the server click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void NewServerClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.EditDialog.BindData(null);

            BoardContext.Current.PageElements.RegisterJsBlockStartup(
                "openModalJs",
                JavaScriptBlocks.OpenModalJs("NntpServerEditDialog"));
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
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

            this.BindData();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot()
                .AddLink(this.GetText("ADMIN_ADMIN", "Administration"), BuildLink.GetLink(ForumPages.Admin_Admin))
                .AddLink(this.GetText("ADMIN_NNTPSERVERS", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_NNTPSERVERS", "TITLE")}";
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.RankList.DataSource = this.GetRepository<NntpServer>().GetByBoardId();
            this.DataBind();
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.RankList.ItemCommand += this.RankListItemCommand;
        }

        /// <summary>
        /// Ranks the list item command.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        private void RankListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":
                    this.EditDialog.BindData(e.CommandArgument.ToType<int>());

                    BoardContext.Current.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("NntpServerEditDialog"));
                    break;
                case "delete":
                    var serverId = e.CommandArgument.ToType<int>();
                    var forums = this.GetRepository<NntpForum>().Get(n => n.NntpServerID == serverId).Select(forum => forum.ForumID).ToList();

                    this.GetRepository<NntpTopic>().DeleteByIds(forums);
                    this.GetRepository<NntpForum>().Delete(n => n.NntpServerID == serverId);
                    this.GetRepository<NntpForum>().Delete(n => n.NntpServerID == serverId);

                    this.BindData();
                    break;
            }
        }

        #endregion
    }
}