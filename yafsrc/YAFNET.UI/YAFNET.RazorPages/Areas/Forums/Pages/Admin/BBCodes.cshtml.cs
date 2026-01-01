/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

using System.Threading.Tasks;

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Modals;
using YAF.Types.Models;

/// <summary>
/// The Admin BBCode Page.
/// </summary>
public class BBCodesModel : AdminPage
{
    /// <summary>
    /// Gets or sets the list.
    /// </summary>
    /// <value>The list.</value>
    [BindProperty]
    public List<BBCode> List { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BBCodesModel"/> class.
    /// </summary>
    public BBCodesModel()
        : base("ADMIN_BBCODE", ForumPages.Admin_BBCodes)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_BBCODE", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet()
    {
        this.BindData();

        return this.Page();
    }

    /// <summary>
    /// Exports the selected list.
    /// </summary>
    public async Task<IActionResult> OnPostExportAsync()
    {
        var codeIDs = this.List.Where(x => x.Selected).Select(x => x.ID).ToList();

        if (codeIDs.Count > 0)
        {
            // export this list as XML...
            var list = await this.GetRepository<BBCode>().GetByBoardIdAsync();

            var selectedList = new List<BBCode>();

            codeIDs.ForEach(
                id =>
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

            const string fileName = "BBCodeExport.xml";

            var writer = new System.Xml.Serialization.XmlSerializer(element.GetType());
            var stream = new MemoryStream();
            writer.Serialize(stream, element);

            return this.File(stream.ToArray(), "application/xml", fileName);
        }

        this.BindData();

        return this.PageBoardContext.Notify(this.GetText("ADMIN_BBCODE", "MSG_NOTHING_SELECTED"), MessageTypes.warning);
    }

    /// <summary>
    /// Delete entry.
    /// </summary>
    /// <param name="b">The b.</param>
    public async Task OnPostDeleteAsync(int b)
    {
        await this.GetRepository<BBCode>().DeleteByIdAsync(b);
        this.BindData();
    }

    /// <summary>
    /// The page size on selected index changed.
    /// </summary>
    public void OnPost()
    {
        this.BindData();
    }

    /// <summary>
    /// Opens the import dialog.
    /// </summary>
    /// <returns>PartialViewResult.</returns>
    public PartialViewResult OnGetImport()
    {
        return new PartialViewResult
               {
                   ViewName = "Dialogs/BBCodeImport",
                   ViewData = new ViewDataDictionary<ImportModal>(
                       this.ViewData,
                       new ImportModal())
               };
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        var list = this.GetRepository<BBCode>().ListPaged(
            this.PageBoardContext.PageBoardID,
            this.PageBoardContext.PageIndex,
            this.Size);

        this.List = list;
    }
}