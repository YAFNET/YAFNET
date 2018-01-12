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
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Xml.Linq;

    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utilities;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Summary description for bannedip.
    /// </summary>
    public partial class extensions : AdminPage
    {
        #region Methods

        /// <summary>
        /// The extension title_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ExtensionTitleLoad([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((Label)sender).Text = "{0} {1}".FormatWith(
                this.PageContext.BoardSettings.FileExtensionAreAllowed
                    ? this.GetText("COMMON", "ALLOWED")
                    : this.GetText("COMMON", "DISALLOWED"),
                this.GetText("ADMIN_EXTENSIONS", "TITLE"));
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.list.ItemCommand += this.ListItemCommand;

            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
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
            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                YafBuildLink.GetLink(ForumPages.admin_admin));
            this.PageLinks.AddLink(this.GetText("ADMIN_EXTENSIONS", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_EXTENSIONS", "TITLE"));
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.list.DataSource =
                this.GetRepository<FileExtension>().Get(e => e.BoardId == this.PageContext.PageBoardID);
            this.DataBind();
        }

        /// <summary>
        /// The list_ item command.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "add":
                    this.EditDialog.BindData(null);

                    YafContext.Current.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("ExtensionsEditDialog"));

                    break;
                case "edit":
                    this.EditDialog.BindData(e.CommandArgument.ToType<int>());

                    YafContext.Current.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("ExtensionsEditDialog"));
                    break;
                case "delete":
                    this.GetRepository<FileExtension>().DeleteById(e.CommandArgument.ToType<int>());
                    this.BindData();
                    break;
                case "export":
                    {
                        this.ExportList();
                    }

                    break;
            }
        }

        /// <summary>
        /// Exports the list.
        /// </summary>
        private void ExportList()
        {
            this.Get<HttpResponseBase>().Clear();
            this.Get<HttpResponseBase>().ClearContent();
            this.Get<HttpResponseBase>().ClearHeaders();

            this.Get<HttpResponseBase>().ContentType = "text/xml";
            this.Get<HttpResponseBase>().AppendHeader(
                "content-disposition",
                "attachment; filename=YafExtensionExport.xml");

            var extensionList = this.GetRepository<FileExtension>()
                .Get(e => e.BoardId == this.PageContext.PageBoardID);

            var element = new XElement(
                "YafSpamWordsList",
                from extension in extensionList
                select new XElement("YafExtension", new XElement("Extension", extension.Extension)));

            element.Save(this.Response.OutputStream);

            this.Get<HttpResponseBase>().Flush();
            this.Get<HttpResponseBase>().End();
        }

        #endregion
    }
}