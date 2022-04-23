/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

    using YAF.Core.Tasks;

    #endregion

    /// <summary>
    /// The Admin Prune Topics Page
    /// </summary>
    public partial class Prune : AdminPage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Prune"/> class. 
        /// </summary>
        public Prune()
            : base("ADMIN_PRUNE", ForumPages.Admin_Prune)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                nameof(JavaScriptBlocks.SelectForumsLoadJs),
                JavaScriptBlocks.SelectForumsLoadJs(
                    "ForumList",
                    this.GetText("PRUNE_FORUM"),
                    false,
                    true,
                    this.ForumListSelected.ClientID));

            base.OnPreRender(e);
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
                this.days.Text = "60";
            }

            this.lblPruneInfo.Text = string.Empty;

            if (!this.Get<ITaskModuleManager>().IsTaskRunning(PruneTopicTask.TaskName))
            {
                return;
            }

            this.lblPruneInfo.Text = this.GetText("ADMIN_PRUNE", "PRUNE_INFO");
            this.commit.Visible = false;
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddAdminIndex();
            this.PageLinks.AddLink(this.GetText("ADMIN_PRUNE", "TITLE"), string.Empty);
        }

        /// <summary>
        /// Handles the Click event of the commit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void CommitClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            PruneTopicTask.Start(
                this.PageBoardContext.PageBoardID,
                this.ForumListSelected.Value.ToType<int>(),
                this.days.Text.ToType<int>(),
                this.permDeleteChkBox.Checked);

            this.PageBoardContext.Notify(this.GetText("ADMIN_PRUNE", "MSG_TASK"), MessageTypes.info);
        }

        #endregion
    }
}