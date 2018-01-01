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
    using System.Web;

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
    /// The Topic Status Edit Page
    /// </summary>
    public partial class topicstatus_edit : AdminPage
    {
        #region Methods

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.save.Click += this.Add_Click;
            this.cancel.Click += this.Cancel_Click;

            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            this.InitializeComponent();
            base.OnInit(e);
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
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
            this.PageLinks.AddLink(
                this.GetText("ADMIN_TOPICSTATUS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_topicstatus));
            this.PageLinks.AddLink(this.GetText("ADMIN_TOPICSTATUS_EDIT", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_TOPICSTATUS", "TITLE"),
                this.GetText("ADMIN_TOPICSTATUS_EDIT", "TITLE"));

            this.save.Text = "<i class=\"fa fa-floppy-o fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("SAVE"));
            this.cancel.Text = "<i class=\"fa fa-remove fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("CANCEL"));

            this.BindData();
        }

        /// <summary>
        /// The add_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Add_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (TopicStatusName.Text.Trim().IsNotSet() || DefaultDescription.Text.Trim().IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_TOPICSTATUS_EDIT", "MSG_ENTER"));

                this.BindData();
            }
            else
            {
                this.GetRepository<TopicStatus>().Save(
                    this.Request.QueryString.GetFirstOrDefault("i").ToType<int>(),
                    this.TopicStatusName.Text.Trim(),
                    this.DefaultDescription.Text.Trim(),
                    this.PageContext.PageBoardID);

                YafBuildLink.Redirect(ForumPages.admin_topicstatus);
            }
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            if (!this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("i").IsSet())
            {
                return;
            }

            var topicStatus = this.GetRepository<TopicStatus>().GetById(this.Request.QueryString.GetFirstOrDefault("i").ToType<int>());

            this.TopicStatusName.Text = topicStatus.TopicStatusName;
            this.DefaultDescription.Text = topicStatus.DefaultDescription;
        }

        /// <summary>
        /// The cancel_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_topicstatus);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        #endregion
    }
}