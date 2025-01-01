﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Pages;

/// <summary>
/// The Search Page.
/// </summary>
public partial class Search : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "Search" /> class.
    ///   The search page constructor.
    /// </summary>
    public Search()
        : base("SEARCH", ForumPages.Search)
    {
    }

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    override protected void OnPreRender(EventArgs e)
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
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        var doSearch = false;

        this.txtSearchStringFromWho.Attributes.Add("data-display", this.PageBoardContext.BoardSettings.EnableDisplayName.ToString());

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

        // Handle search by url
        var searchString = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("search");

        if (searchString.IsSet() && searchString.Length < 50)
        {
            this.searchInput.Text = this.Server.UrlDecode(searchString);
            doSearch = true;
        }

        if (searchString.IsSet())
        {
            try
            {
                this.ForumListSelected.Value = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("forum");
            }
            catch (Exception)
            {
                this.ForumListSelected.Value = "0";
            }
        }

        var postedBy = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("postedby");

        if (postedBy.IsSet() && postedBy.Length < 50)
        {
            this.txtSearchStringFromWho.Text = this.Server.UrlDecode(postedBy);
            doSearch = true;
        }

        var tag = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("tag");

        if (tag.IsSet())
        {
            this.SearchStringTag.Text = this.Server.UrlDecode(tag);
            doSearch = true;
        }

        if (doSearch)
        {
            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                "openModalJs",
                JavaScriptBlocks.                DoSearchJs);
        }
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
    }
}