/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2017 Ingo Herbote
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
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Classes;
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
    /// The Admin spam words page.
    /// </summary>
    public partial class spamwords : AdminPage
    {
        #region Methods

        /// <summary>
        /// The pager top_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PagerTop_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            // rebind
            this.BindData();
        }

        /// <summary>
        /// Handles the Click event of the Search control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Search_Click(object sender, EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// Handles the Load event of the Delete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("ADMIN_SPAMWORDS", "MSG_DELETE"));
        }

        /// <summary>
        /// Adds Localized Text to Button
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void AddLoad(object sender, EventArgs e)
        {
            var add = (LinkButton)sender;
            add.Text = "<i class=\"fa fa-plus-square fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("ADMIN_SPAMWORDS", "ADD"));
        }

        /// <summary>
        /// Adds Localized Text to Button
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        protected void ExportLoad(object sender, EventArgs e)
        {
            var export = (LinkButton)sender;
            export.Text = "<i class=\"fa fa-download fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("ADMIN_SPAMWORDS", "EXPORT"));
        }

        /// <summary>
        /// Adds Localized Text to Button
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        protected void ImportLoad(object sender, EventArgs e)
        {
            var import = (LinkButton)sender;
            import.Text = "<i class=\"fa fa-upload fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("ADMIN_SPAMWORDS", "IMPORT"));
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

            this.BindData();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot()
                .AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin))
                .AddLink(this.GetText("ADMIN_SPAMWORDS", "TITLE"));

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_SPAMWORDS", "TITLE"));
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.PagerTop.PageSize = this.Get<YafBoardSettings>().MemberListPageSize;

            var searchText = this.SearchInput.Text.Trim();

            var bannedList = this.GetRepository<Spam_Words>()
                .List(
                    mask: searchText.IsSet() ? searchText : null,
                    pageIndex: this.PagerTop.CurrentPageIndex,
                    pageSize: this.PagerTop.PageSize);

            this.list.DataSource = bannedList;

            this.PagerTop.Count = bannedList != null && bannedList.HasRows()
                                      ? bannedList.AsEnumerable().First().Field<int>("TotalRows")
                                      : 0;
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
                    YafBuildLink.Redirect(ForumPages.admin_spamwords_edit);
                    break;
                case "edit":
                    YafBuildLink.Redirect(ForumPages.admin_spamwords_edit, "i={0}", e.CommandArgument);
                    break;
                case "delete":
                    this.GetRepository<Spam_Words>().DeleteByID(e.CommandArgument.ToType<int>());
                    this.Get<IObjectStore>().Remove(Constants.Cache.SpamWords);
                    this.BindData();
                    break;
                case "export":
                    {
                        var spamWords = this.GetRepository<Spam_Words>().ListTyped();

                        this.Get<HttpResponseBase>().Clear();
                        this.Get<HttpResponseBase>().ClearContent();
                        this.Get<HttpResponseBase>().ClearHeaders();

                        this.Get<HttpResponseBase>().ContentType = "application/vnd.text";
                        this.Get<HttpResponseBase>()
                            .AppendHeader("content-disposition", "attachment; filename=YafSpamWordsExport.txt");

                        var streamWriter = new StreamWriter(this.Get<HttpResponseBase>().OutputStream);

                        foreach (var word in spamWords)
                        {
                            streamWriter.Write(word.SpamWord);
                            streamWriter.Write(streamWriter.NewLine);
                        }

                        streamWriter.Close();

                        this.Response.End();
                    }

                    break;
                case "import":
                    YafBuildLink.Redirect(ForumPages.admin_spamwords_import);
                    break;
            }
        }

        #endregion
    }
}