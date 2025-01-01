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

namespace YAF.Pages.Admin;

using System.Xml.Linq;

using YAF.Types.Models;

/// <summary>
/// The Replace Words Admin Page.
/// </summary>
public partial class ReplaceWords : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReplaceWords"/> class. 
    /// </summary>
    public ReplaceWords()
        : base("ADMIN_REPLACEWORDS", ForumPages.Admin_ReplaceWords)
    {
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    override protected void OnInit(EventArgs e)
    {
        this.list.ItemCommand += this.ListItemCommand;

        base.OnInit(e);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
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
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot()
            .AddAdminIndex()
            .AddLink(this.GetText("ADMIN_REPLACEWORDS", "TITLE"));
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.list.DataSource = this.GetRepository<Replace_Words>().GetByBoardId();

        this.DataBind();

        if (this.list.Items.Count == 0)
        {
            this.EmptyState.Visible = true;
        }
    }

    /// <summary>
    /// Handles the ItemCommand event of the List control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
    private void ListItemCommand(object sender, RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "add":
                this.EditDialog.BindData(null);

                this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                    "openModalJs",
                    JavaScriptBlocks.OpenModalJs("ReplaceWordsEditDialog"));

                break;
            case "edit":
                this.EditDialog.BindData(e.CommandArgument.ToType<int>());

                this.PageBoardContext.PageElements.RegisterJsBlockStartup(
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

    protected void AddClick(object sender, EventArgs e)
    {
        this.EditDialog.BindData(null);

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            "openModalJs",
            JavaScriptBlocks.OpenModalJs("ReplaceWordsEditDialog"));
    }

    protected void ExportClick(object sender, EventArgs e)
    {
        this.ExportWords();
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

        var spamWordList = this.GetRepository<Replace_Words>().GetByBoardId();

        var element = new XElement(
            "YafReplaceWordsList",
            from spamWord in spamWordList
            select new XElement(
                "YafReplaceWords",
                new XElement("BadWord", spamWord.BadWord),
                new XElement("GoodWord", spamWord.GoodWord)));

        element.Save(this.Get<HttpResponseBase>().OutputStream);

        this.Get<HttpResponseBase>().Flush();
        this.Get<HttpResponseBase>().End();
    }
}