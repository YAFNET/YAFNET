/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
/// The Admin spam words page.
/// </summary>
public partial class SpamWords : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpamWords"/> class. 
    /// </summary>
    public SpamWords()
        : base("ADMIN_SPAMWORDS", ForumPages.Admin_SpamWords)
    {
    }

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

        this.PageSize.DataSource = StaticDataHelper.PageEntries();
        this.PageSize.DataTextField = "Name";
        this.PageSize.DataValueField = "Value";
        this.PageSize.DataBind();

        try
        {
            this.PageSize.SelectedValue = this.PageBoardContext.PageUser.PageSize.ToString();
        }
        catch (Exception)
        {
            this.PageSize.SelectedValue = "5";
        }

        this.BindData();
    }

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
    /// The page size on selected index changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PageSizeSelectedIndexChanged(object sender, EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot().AddAdminIndex()
            .AddLink(this.GetText("ADMIN_SPAMWORDS", "TITLE"));
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

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
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
        this.PagerTop.PageSize = this.PageSize.SelectedValue.ToType<int>();

        var searchText = this.SearchInput.Text.Trim();

        List<Spam_Words> bannedList;

        if (searchText.IsSet())
        {
            bannedList = this.GetRepository<Spam_Words>().GetPaged(
                x => x.BoardID == this.PageBoardContext.PageBoardID && x.SpamWord == searchText,
                this.PagerTop.CurrentPageIndex,
                this.PagerTop.PageSize);
        }
        else
        {
            bannedList = this.GetRepository<Spam_Words>().GetPaged(
                x => x.BoardID == this.PageBoardContext.PageBoardID,
                this.PagerTop.CurrentPageIndex,
                this.PagerTop.PageSize);
        }

        this.list.DataSource = bannedList;

        this.PagerTop.Count = !bannedList.NullOrEmpty()
                                  ? this.GetRepository<Spam_Words>()
                                      .Count(x => x.BoardID == this.PageBoardContext.PageBoardID).ToType<int>()
                                  : 0;

        if (this.list.Items.Count == 0)
        {
            this.NoInfo.Visible = true;
        }

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

        var spamWordList =
            this.GetRepository<Spam_Words>().GetByBoardId();

        var element = new XElement(
            "YafSpamWordsList",
            from spamWord in spamWordList
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

                this.PageBoardContext.PageElements.RegisterJsBlockStartup(
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
}