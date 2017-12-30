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
    using YAF.Utilities;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Admin Private messages page
    /// </summary>
    public partial class pm : AdminPage
    {
        #region Methods

        /// <summary>
        /// Löst das <see cref="E:System.Web.UI.Control.Init" />-Ereignis aus.
        /// </summary>
        /// <param name="e">Ein <see cref="T:System.EventArgs" />-Objekt, das die Ereignisdaten enthält.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.commit.Click += this.CommitClick;
            base.OnInit(e);
        }

        /// <summary>
        /// Registers the needed Java Scripts
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            YafContext.Current.PageElements.RegisterJsBlock(
                "TouchSpinLoadJs",
                JavaScriptBlocks.LoadTouchSpin(
                    ".DaysInput",
                    "postfix: '{0}'".FormatWith(this.GetText("ADMIN_PM", "DAYS"))));

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

            this.Days1.Text = "60";
            this.Days2.Text = "180";
            this.BindData();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                YafBuildLink.GetLink(ForumPages.admin_admin));
            this.PageLinks.AddLink(this.GetText("ADMIN_PM", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_PM", "TITLE"));
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            using (var dataTable = LegacyDb.pmessage_info())
            {
                this.Count.Text = dataTable.Rows[0]["NumTotal"].ToString();
            }
        }

        /// <summary>
        /// Commits the click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CommitClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            LegacyDb.pmessage_prune(this.Days1.Text, this.Days2.Text);
            this.BindData();
        }

        #endregion
    }
}