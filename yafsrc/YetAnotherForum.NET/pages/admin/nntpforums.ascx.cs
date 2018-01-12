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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Web.UI.WebControls;

    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Summary description for ranks.
    /// </summary>
    public partial class nntpforums : AdminPage
    {
        #region Methods

        /// <summary>
        /// News the forum click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void NewForumClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.EditDialog.BindData(null);

            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "openModalJs",
                JavaScriptBlocks.OpenModalJs("NntpForumEditDialog"));
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.BindData();
            }
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));

            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                YafBuildLink.GetLink(ForumPages.admin_admin));
            this.PageLinks.AddLink(this.GetText("ADMIN_NNTPFORUMS", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_NNTPFORUMS", "TITLE"));
        }

        /// <summary>
        /// Ranks the list item command.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void RankListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":
                    this.EditDialog.BindData(e.CommandArgument.ToType<int>());

                    YafContext.Current.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("NntpForumEditDialog"));
                    break;
                case "delete":
                    LegacyDb.nntpforum_delete(e.CommandArgument);
                    this.BindData();
                    break;
            }
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.RankList.DataSource = LegacyDb.nntpforum_list(this.PageContext.PageBoardID, null, null, DBNull.Value);
            this.DataBind();
        }

        #endregion
    }
}