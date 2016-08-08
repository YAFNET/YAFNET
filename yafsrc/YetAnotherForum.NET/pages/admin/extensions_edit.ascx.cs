/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2016 Ingo Herbote
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
    using System.Web;

    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Summary description for bannedip_edit.
    /// </summary>
    public partial class extensions_edit : AdminPage
    {
        #region Methods

        /// <summary>
        /// The is valid extension.
        /// </summary>
        /// <param name="newExtension">
        /// The new extension.
        /// </param>
        /// <returns>
        /// The is valid extension.
        /// </returns>
        protected bool IsValidExtension([NotNull] string newExtension)
        {
            if (newExtension.IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EXTENSIONS_EDIT", "MSG_ENTER"));
                return false;
            }

            if (newExtension.IndexOf('.') != -1)
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EXTENSIONS_EDIT", "MSG_REMOVE"));
                return false;
            }

            // TODO: maybe check for duplicate?
            return true;
        }

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
            //string strAddEdit = (this.Request.QueryString.GetFirstOrDefault("i") == null) ? "Add" : "Edit";

            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
            this.PageLinks.AddLink(this.GetText("ADMIN_EXTENSIONS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_extensions));
            this.PageLinks.AddLink(this.GetText("ADMIN_EXTENSIONS_EDIT", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_EXTENSIONS", "TITLE"),
                this.GetText("ADMIN_EXTENSIONS_EDIT", "TITLE"));

            this.save.Text = "<i class=\"fa fa-floppy-o fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("SAVE"));
            this.cancel.Text = "<i class=\"fa fa-remove fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("CANCEL"));

            this.BindData();

            //this.extension.Attributes.Add("style", "width:250px");
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
            string ext = this.extension.Text.Trim();

            if (!this.IsValidExtension(ext))
            {
                this.BindData();
            }
            else
            {
                var entity = new FileExtension()
                                    {
                                        ID = this.Request.QueryString.GetFirstOrDefaultAs<int?>("i") ?? 0,
                                        BoardId = this.PageContext.PageBoardID,
                                        Extension = ext
                                    };

                this.GetRepository<FileExtension>().Upsert(entity);
                YafBuildLink.Redirect(ForumPages.admin_extensions);
            }
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            var extensionId = this.Request.QueryString.GetFirstOrDefaultAs<int?>("i");

            if (!extensionId.HasValue)
            {
                return;
            }

            var fileExtension = this.GetRepository<FileExtension>().GetByID(extensionId.Value);
            this.extension.Text = fileExtension.Extension;
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
            YafBuildLink.Redirect(ForumPages.admin_extensions);
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