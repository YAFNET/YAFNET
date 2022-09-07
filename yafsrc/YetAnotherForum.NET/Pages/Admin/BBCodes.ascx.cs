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
/// The Admin BBCode Page.
/// </summary>
public partial class BBCodes : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BBCodes"/> class. 
    /// </summary>
    public BBCodes()
        : base("ADMIN_BBCODE", ForumPages.Admin_BBCodes)
    {
    }

    /// <summary>
    /// The get selected bb code i ds.
    /// </summary>
    /// <returns>
    /// The Id of the BB Code
    /// </returns>
    [NotNull]
    protected List<int> GetSelectedBbCodeIDs()
    {
        // get checked items....
        return (from RepeaterItem item in this.bbCodeList.Items
                let sel = item.FindControlAs<CheckBox>("chkSelected")
                where sel.Checked
                select item.FindControlAs<HiddenField>("hiddenBBCodeID") into hiddenId
                select hiddenId.Value.ToType<int>()).ToList();
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
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_BBCODE", "TITLE"), string.Empty);
    }

    /// <summary>
    /// The BBCode list item command.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
    protected void BbCodeListItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "add":
                this.Get<LinkBuilder>().Redirect(ForumPages.Admin_BBCode_Edit);
                break;
            case "edit":
                this.Get<LinkBuilder>().Redirect(ForumPages.Admin_BBCode_Edit, new { b = e.CommandArgument });
                break;
            case "delete":
                this.GetRepository<BBCode>().DeleteById(e.CommandArgument.ToType<int>());
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
    /// Exports the selected list.
    /// </summary>
    private void ExportList()
    {
        var codeIDs = this.GetSelectedBbCodeIDs();

        if (codeIDs.Count > 0)
        {
            this.Get<HttpResponseBase>().Clear();
            this.Get<HttpResponseBase>().ClearContent();
            this.Get<HttpResponseBase>().ClearHeaders();

            this.Get<HttpResponseBase>().ContentType = "text/xml";
            this.Get<HttpResponseBase>().AppendHeader(
                "content-disposition",
                "attachment; filename=BBCodeExport.xml");

            // export this list as XML...
            var list =
                this.GetRepository<BBCode>().GetByBoardId();

            var selectedList = new List<BBCode>();

            codeIDs.ForEach(id =>
                {
                    var found = list.First(e => e.ID == id);

                    selectedList.Add(found);
                });

            var element = new XElement(
                "YafBBCodeList",
                from bbCode in selectedList
                select new XElement(
                    "YafBBCode",
                    new XElement("Name", bbCode.Name),
                    new XElement("Description", bbCode.Description),
                    new XElement("OnClickJS", bbCode.OnClickJS),
                    new XElement("DisplayJS", bbCode.DisplayJS),
                    new XElement("EditJS", bbCode.EditJS),
                    new XElement("DisplayCSS", bbCode.DisplayCSS),
                    new XElement("SearchRegex", bbCode.SearchRegex),
                    new XElement("ReplaceRegex", bbCode.ReplaceRegex),
                    new XElement("Variables", bbCode.Variables),
                    new XElement("UseModule", bbCode.UseModule),
                    new XElement("UseToolbar", bbCode.UseToolbar),
                    new XElement("ModuleClass", bbCode.ModuleClass),
                    new XElement("ExecOrder", bbCode.ExecOrder)));

            element.Save(this.Get<HttpResponseBase>().OutputStream);

            this.Get<HttpResponseBase>().Flush();
            this.Get<HttpResponseBase>().End();
        }
        else
        {
            this.PageBoardContext.Notify(
                this.GetText("ADMIN_BBCODE", "MSG_NOTHING_SELECTED"),
                MessageTypes.warning);
        }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.PagerTop.PageSize = this.PageSize.SelectedValue.ToType<int>();

        var list = this.GetRepository<BBCode>().ListPaged(
            this.PageBoardContext.PageBoardID,
            this.PagerTop.CurrentPageIndex,
            this.PagerTop.PageSize);

        this.bbCodeList.DataSource = list;

        this.PagerTop.Count = !list.NullOrEmpty()
                                  ? this.GetRepository<BBCode>()
                                      .Count(x => x.BoardID == this.PageBoardContext.PageBoardID).ToType<int>()
                                  : 0;

        this.DataBind();
    }
}