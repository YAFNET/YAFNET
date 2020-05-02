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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Xml.Linq;

    using YAF.Configuration;
    
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
    /// The Admin spam words page.
    /// </summary>
    public partial class SpamWords : AdminPage
    {
        #region Methods

        /// <summary>
        /// The pager top_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PagerTopChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            // rebind
            this.BindData();
        }

        /// <summary>
        /// Handles the Click event of the Search control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void SearchClick(object sender, EventArgs e)
        {
            this.BindData();
        }

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

            this.BindData();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot()
                .AddLink(this.GetText("ADMIN_ADMIN", "Administration"), BuildLink.GetLink(ForumPages.Admin_Admin))
                .AddLink(this.GetText("ADMIN_SPAMWORDS", "TITLE"));

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_SPAMWORDS", "TITLE")}";
        }

        /// <summary>
        /// The add click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void AddClick(object sender, EventArgs e)
        {
            this.EditDialog.BindData(null);

            BoardContext.Current.PageElements.RegisterJsBlockStartup(
                "openModalJs",
                JavaScriptBlocks.OpenModalJs("SpamWordsEditDialog"));
        }

        /// <summary>
        /// The export click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ExportClick(object sender, EventArgs e)
        {
            this.ExportWords();
        }

        /// <summary>
        /// Loads the Data
        /// </summary>
        private void BindData()
        {
            this.PagerTop.PageSize = this.Get<BoardSettings>().MemberListPageSize;

            var searchText = this.SearchInput.Text.Trim();

            List<Spam_Words> bannedList;

            if (searchText.IsSet())
            {
                bannedList = this.GetRepository<Spam_Words>().GetPaged(
                    x => x.BoardID == this.PageContext.PageBoardID && x.SpamWord == searchText,
                    this.PagerTop.CurrentPageIndex,
                    this.PagerTop.PageSize);
            }
            else
            {
                bannedList = this.GetRepository<Spam_Words>().GetPaged(
                    x => x.BoardID == this.PageContext.PageBoardID,
                    this.PagerTop.CurrentPageIndex,
                    this.PagerTop.PageSize);
            }

            this.list.DataSource = bannedList;

            this.PagerTop.Count = bannedList != null && bannedList.Any()
                                      ? this.GetRepository<Spam_Words>()
                                          .Count(x => x.BoardID == this.PageContext.PageBoardID).ToType<int>()
                                      : 0;
            this.DataBind();
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
                "attachment; filename=SpamWordsExport.xml");

            var spamwordList =
                this.GetRepository<Spam_Words>().GetByBoardId();

            var element = new XElement(
                "YafSpamWordsList",
                from spamWord in spamwordList
                select new XElement("YafSpamWords", new XElement("SpamWord", spamWord.SpamWord)));

            element.Save(this.Get<HttpResponseBase>().OutputStream);

            this.Get<HttpResponseBase>().Flush();
            this.Get<HttpResponseBase>().End();
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
                case "edit":
                    this.EditDialog.BindData(e.CommandArgument.ToType<int>());

                    BoardContext.Current.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("SpamWordsEditDialog"));

                    break;
                case "delete":
                    this.GetRepository<Spam_Words>().DeleteById(e.CommandArgument.ToType<int>());
                    this.Get<IObjectStore>().Remove(Constants.Cache.SpamWords);
                    this.BindData();
                    break;
            }
        }

        #endregion
    }
}