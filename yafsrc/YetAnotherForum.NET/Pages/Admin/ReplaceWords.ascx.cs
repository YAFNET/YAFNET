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
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Xml.Linq;

    using YAF.Core;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Replace Words Admin Page.
    /// </summary>
    public partial class ReplaceWords : AdminPage
    {
        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.list.ItemCommand += this.ListItemCommand;

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
                .AddLink(this.GetText("ADMIN_ADMIN", "Administration"), BuildLink.GetLink(ForumPages.Admin_Admin))
                .AddLink(this.GetText("ADMIN_REPLACEWORDS", "TITLE"));

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_REPLACEWORDS", "TITLE")}";

            this.BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.list.DataSource = this.GetRepository<Replace_Words>().GetByBoardId();
            this.DataBind();
        }

        /// <summary>
        /// Handles the ItemCommand event of the List control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        private void ListItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "add":
                    this.EditDialog.BindData(null);

                    BoardContext.Current.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("ReplaceWordsEditDialog"));

                    break;
                case "edit":
                    this.EditDialog.BindData(e.CommandArgument.ToType<int>());

                    BoardContext.Current.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("ReplaceWordsEditDialog"));
                    break;
                case "delete":
                    this.GetRepository<Replace_Words>().DeleteById(e.CommandArgument.ToType<int>());
                    this.Get<IObjectStore>().Remove(Constants.Cache.ReplaceWords);
                    this.BindData();
                    break;
                case "export":
                    {
                        this.ExportWords();
                    }

                    break;
            }
        }

        /// <summary>
        /// Exports the spam words.
        /// </summary>
        private void ExportWords()
        {
            this.Get<HttpResponseBase>().Clear();
            this.Get<HttpResponseBase>().ClearContent();
            this.Get<HttpResponseBase>().ClearHeaders();

            this.Get<HttpResponseBase>().ContentType = "text/xml";
            this.Get<HttpResponseBase>().AppendHeader(
                "content-disposition",
                "attachment; filename=ReplaceWordsExport.xml");

            var spamwordList = this.GetRepository<Replace_Words>().GetByBoardId();

            var element = new XElement(
                "YafReplaceWordsList",
                from spamWord in spamwordList
                select new XElement(
                    "YafReplaceWords",
                    new XElement("BadWord", spamWord.BadWord),
                    new XElement("GoodWord", spamWord.GoodWord)));

            element.Save(this.Get<HttpResponseBase>().OutputStream);

            this.Get<HttpResponseBase>().Flush();
            this.Get<HttpResponseBase>().End();
        }

        #endregion
    }
}