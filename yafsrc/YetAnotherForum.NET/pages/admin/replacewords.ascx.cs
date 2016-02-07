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
    using System.Web.UI.WebControls;

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
    /// The Replace Words Admin Page.
    /// </summary>
    public partial class replacewords : AdminPage
    {
        #region Methods

        /// <summary>
        /// Handles the Load event of the Delete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("ADMIN_REPLACEWORDS", "MSG_DELETE"));
        }

        /// <summary>
        /// Adds the load.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void addLoad(object sender, EventArgs e)
        {
            var add = (Button)sender;
            add.Text = this.GetText("ADMIN_REPLACEWORDS", "ADD");
        }

        /// <summary>
        /// Add Localized Text to Button
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void exportLoad(object sender, EventArgs e)
        {
            var export = (Button)sender;
            export.Text = this.GetText("ADMIN_REPLACEWORDS", "EXPORT");
        }

        /// <summary>
        /// Add Localized Text to Button
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void importLoad(object sender, EventArgs e)
        {
            var import = (Button)sender;
            import.Text = this.GetText("ADMIN_REPLACEWORDS", "IMPORT");
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.list.ItemCommand += this.List_ItemCommand;

            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
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

            this.PageLinks.AddRoot()
                .AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin))
                .AddLink(this.GetText("ADMIN_REPLACEWORDS", "TITLE"));

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_REPLACEWORDS", "TITLE"));

            this.BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.list.DataSource = this.GetRepository<Replace_Words>().List();
            this.DataBind();
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        /// Handles the ItemCommand event of the List control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        private void List_ItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "add":
                    YafBuildLink.Redirect(ForumPages.admin_replacewords_edit);
                    break;
                case "edit":
                    YafBuildLink.Redirect(ForumPages.admin_replacewords_edit, "i={0}", e.CommandArgument);
                    break;
                case "delete":
                    this.GetRepository<Replace_Words>().DeleteByID(e.CommandArgument.ToType<int>());
                    this.Get<IObjectStore>().Remove(Constants.Cache.ReplaceWords);
                    this.BindData();
                    break;
                case "export":
                    {
                        DataTable replaceDT = this.GetRepository<Replace_Words>().List();
                        replaceDT.DataSet.DataSetName = "YafReplaceWordsList";
                        replaceDT.TableName = "YafReplaceWords";
                        replaceDT.Columns.Remove("ID");
                        replaceDT.Columns.Remove("BoardID");

                        this.Response.ContentType = "text/xml";
                        this.Response.AppendHeader(
                            "Content-Disposition",
                            "attachment; filename=YafReplaceWordsExport.xml");
                        replaceDT.DataSet.WriteXml(this.Response.OutputStream);
                        this.Response.End();
                    }

                    break;
                case "import":
                    YafBuildLink.Redirect(ForumPages.admin_replacewords_import);
                    break;
            }
        }

        #endregion
    }
}