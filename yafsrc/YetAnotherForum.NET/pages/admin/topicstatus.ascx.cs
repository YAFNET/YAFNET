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
    /// Topic Status Admin Edit Page.
    /// </summary>
    public partial class topicstatus : AdminPage
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
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                YafBuildLink.GetLink(ForumPages.admin_admin));
            this.PageLinks.AddLink(this.GetText("ADMIN_TOPICSTATUS", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_TOPICSTATUS", "TITLE"));
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.list.DataSource = this.GetRepository<TopicStatus>().GetByBoardId();
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
                        JavaScriptBlocks.OpenModalJs("TopicStatusEditDialog"));

                    break;
                case "edit":
                    this.EditDialog.BindData(e.CommandArgument.ToType<int>());

                    YafContext.Current.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("TopicStatusEditDialog"));
                    break;
                case "delete":
                    this.GetRepository<TopicStatus>().DeleteById(e.CommandArgument.ToType<int>());
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
                "attachment; filename=TopicStatusExport.xml");

            var spamwordList =
                this.GetRepository<TopicStatus>().GetByBoardId();

            var element = new XElement(
                "YafTopicStatusList",
                from spamWord in spamwordList
                select new XElement("YafTopicStatus", new XElement("TopicStatusName", spamWord.TopicStatusName), new XElement("DefaultDescription", spamWord.DefaultDescription)));

            element.Save(this.Response.OutputStream);

            this.Get<HttpResponseBase>().Flush();
            this.Get<HttpResponseBase>().End();
        }

        #endregion
    }
}