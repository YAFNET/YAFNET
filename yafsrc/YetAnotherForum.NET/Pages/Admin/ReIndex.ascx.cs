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
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Core.BasePages;
    using YAF.Core.Helpers;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Admin Database Maintenance Page
    /// </summary>
    public partial class ReIndex : AdminPage
    {
        #region Methods

        /// <summary>
        /// Registers the needed Java Scripts
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            this.PageContext.PageElements.RegisterJsBlockStartup(
                "BlockUiFunctionJs",
                JavaScriptBlocks.BlockUiFunctionJs(
                    "DeleteForumMessage"));

            base.OnPreRender(e);
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

            // Check and see if it should make panels enable or not
            this.PanelReindex.Visible = true;
            this.PanelShrink.Visible = true;
            this.PanelRecoveryMode.Visible = true;
            this.PanelGetStats.Visible = true;

            this.Shrink.ReturnConfirmText = this.GetText("ADMIN_REINDEX", "CONFIRM_SHRINK");
            this.Shrink.ReturnConfirmEvent = "blockUIMessage";
            
            this.Reindex.ReturnConfirmText = this.GetText("ADMIN_REINDEX", "CONFIRM_REINDEX");
            this.Reindex.ReturnConfirmEvent = "blockUIMessage";

            this.RecoveryMode.ReturnConfirmText = this.GetText("ADMIN_REINDEX", "CONFIRM_RECOVERY");

            this.RadioButtonList1.Items.Add(new ListItem(this.GetText("ADMIN_REINDEX", "RECOVERY1")));
            this.RadioButtonList1.Items.Add(new ListItem(this.GetText("ADMIN_REINDEX", "RECOVERY2")));
            this.RadioButtonList1.Items.Add(new ListItem(this.GetText("ADMIN_REINDEX", "RECOVERY3")));

            this.RadioButtonList1.SelectedIndex = 0;

            this.BindData();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, BuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.Admin_Admin));
            this.PageLinks.AddLink(this.GetText("ADMIN_REINDEX", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMINMENU", "ADMIN_REINDEX")}";
        }

        /// <summary>
        /// Gets the stats click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void GetStatsClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            try
            {
                this.txtIndexStatistics.Text = (string)this.Get<IDbFunction>().Query.getstats();
            }
            catch (Exception ex)
            {
                this.txtIndexStatistics.Text = $"Failure: {ex}";
            }
        }

        /// <summary>
        /// Sets the Recovery mode
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void RecoveryModeClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            var recoveryMode = this.RadioButtonList1.SelectedIndex switch
                {
                    0 => "FULL",
                    1 => "SIMPLE",
                    2 => "BULK_LOGGED",
                    _ => string.Empty
                };

            this.txtIndexStatistics.Text = this.Get<IDbFunction>().ChangeRecoveryMode(recoveryMode);
        }

        /// <summary>
        /// Re-indexing the Database
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ReindexClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.txtIndexStatistics.Text = string.Empty + this.Get<IDbFunction>().ReIndexDatabase();
        }

        /// <summary>
        /// Mod By Touradg (herman_herman) 2009/10/19
        /// Shrinking Database
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ShrinkClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            try
            {
                this.Get<IDbFunction>().ShrinkDatabase();
                this.txtIndexStatistics.Text = string.Empty;
                this.txtIndexStatistics.Text = this.GetTextFormatted(
                    "INDEX_SHRINK",
                    this.Get<IDbFunction>().GetDBSize());

                BuildLink.Redirect(ForumPages.Admin_ReIndex);
            }
            catch (Exception error)
            {
                this.txtIndexStatistics.Text += this.GetTextFormatted("INDEX_STATS_FAIL", error.Message);
            }
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.DataBind();
        }

        #endregion
    }
}