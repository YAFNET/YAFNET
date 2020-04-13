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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Model;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Search Page.
    /// </summary>
    public partial class Search : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Search" /> class.
        ///   The search page constructor.
        /// </summary>
        public Search()
            : base("SEARCH")
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
            // setup jQuery and Jquery Ui Tabs.
            BoardContext.Current.PageElements.RegisterJsBlock(
                "dropDownToggleJs",
                JavaScriptBlocks.DropDownToggleJs());

            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            var doSearch = false;

            this.txtSearchStringFromWho.Attributes.Add("data-display", this.Get<BoardSettings>().EnableDisplayName.ToString());

            // Load result dropdown
            this.listResInPage.Items.Add(new ListItem(this.GetText("result5"), "5"));
            this.listResInPage.Items.Add(new ListItem(this.GetText("result10"), "10"));
            this.listResInPage.Items.Add(new ListItem(this.GetText("result25"), "25"));
            this.listResInPage.Items.Add(new ListItem(this.GetText("result50"), "50"));

            // Load listSearchWhat dropdown
            this.listSearchWhat.Items.Add(new ListItem(this.GetText("match_exact"), "2"));
            this.listSearchWhat.Items.Add(new ListItem(this.GetText("match_any"), "1"));
            this.listSearchWhat.Items.Add(new ListItem(this.GetText("match_all"), "0"));

            // Load TitleOnly dropdown
            this.TitleOnly.Items.Add(new ListItem(this.GetText("POST_AND_TITLE"), "0"));
            this.TitleOnly.Items.Add(new ListItem(this.GetText("TITLE_ONLY"), "1"));

            this.listSearchWhat.SelectedIndex = 2;

            // Load forum's combo
            var forumList = this.GetRepository<Forum>().ListAllSortedAsDataTable(
                this.PageContext.PageBoardID,
                this.PageContext.PageUserID);

            this.listForum.AddForumAndCategoryIcons(forumList);

            this.listForum.DataValueField = "ForumID";
            this.listForum.DataTextField = "Title";

            this.listForum.DataBind();

            this.listForum.Items.Insert(0, new ListItem(this.GetText("allforums"), "0"));

            // Handle search by url
            var searchString = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("search");

            if (searchString.IsSet() && searchString.Length < 50)
            {
                this.searchInput.Text = this.Server.UrlDecode(searchString);
                doSearch = true;
            }

            var forumString = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("forum");

            if (searchString.IsSet() && this.listForum.Enabled)
            {
                try
                {
                    this.listForum.SelectedValue = this.Server.UrlDecode(forumString);
                }
                catch (Exception)
                {
                    this.listForum.SelectedValue = "0";
                }
            }

            var postedBy = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("postedby");

            if (postedBy.IsSet() && postedBy.Length < 50)
            {
                this.txtSearchStringFromWho.Text = this.Server.UrlDecode(postedBy);
                doSearch = true;
            }

            if (doSearch)
            {
                BoardContext.Current.PageElements.RegisterJsBlockStartup(
                    "openModalJs",
                    JavaScriptBlocks.DoSearchJs());
            }

            var tag = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("tag");

            if (tag.IsSet())
            {
                this.SearchStringTag.Text = this.Server.UrlDecode(tag);
                doSearch = true;
            }

            if (doSearch)
            {
                BoardContext.Current.PageElements.RegisterJsBlockStartup(
                    "openModalJs",
                    JavaScriptBlocks.DoSearchJs());
            }
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
        }

        #endregion
    }
}