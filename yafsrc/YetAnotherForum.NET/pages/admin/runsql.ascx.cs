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

    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The runsql.
    /// </summary>
    public partial class runsql : AdminPage
    {
        #region Methods

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

            this.chkRunInTransaction.Text = this.GetText("ADMIN_RUNSQL", "RUN_TRANSCATION");

            this.BindData();
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
            this.PageLinks.AddLink(this.GetText("ADMIN_RUNSQL", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_RUNSQL", "TITLE"));
        }

        /// <summary>
        /// Runs the query click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void RunQueryClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.txtResult.Text = string.Empty;
            this.ResultHolder.Visible = true;

            /*  using (var connMan = new MsSqlDbConnectionManager())
              {
                connMan.InfoMessage += this.connMan_InfoMessage;
                string sql = this.txtQuery.Text.Trim();
        
                // connMan.DBConnection.FireInfoMessageEventOnUserErrors = true;
                sql = sql.Replace("{databaseOwner}", Config.DatabaseOwner);
                sql = sql.Replace("{objectQualifier}", Config.DatabaseObjectQualifier);
        
                this.txtResult.Text = LegacyDb.db_runsql(sql, connMan, this.chkRunInTransaction.Checked);
              } */
            this.txtResult.Text = LegacyDb.db_runsql_new(this.txtQuery.Text.Trim(), this.chkRunInTransaction.Checked);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.DataBind();
        }

        #endregion
    }
}