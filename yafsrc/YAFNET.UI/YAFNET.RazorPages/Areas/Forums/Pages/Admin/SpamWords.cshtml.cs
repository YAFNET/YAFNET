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
using YAF.Types.Extensions;
using YAF.Types.Modals;
using YAF.Types.Models;

/// <summary>
/// The Admin spam words page.
/// </summary>
public class SpamWordsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the list.
    /// </summary>
    /// <value>The list.</value>
    [BindProperty] public List<SpamWords> List { get; set; }

    /// <summary>
    /// Gets or sets the search input.
    /// </summary>
    /// <value>The search input.</value>
    [BindProperty] public string SearchInput { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpamWordsModel"/> class.
    /// </summary>
    public SpamWordsModel()
        : base("ADMIN_SPAMWORDS", ForumPages.Admin_SpamWords)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex()
            .AddLink(this.GetText("ADMIN_SPAMWORDS", "TITLE"));
    }

    /// <summary>
    /// Handles the Click event of the Search control.
    /// </summary>
    public void OnPostSearch()
    {
        this.BindData();
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
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
    /// Exports the spam words.
    /// </summary>
    public async Task<IActionResult> OnPostExportAsync()
    {
        var spamWordList =
            await this.GetRepository<SpamWords>().GetByBoardIdAsync();

        const string fileName = "SpamWordsExport.xml";

        var element = new XElement(
            "YafSpamWordsList",
            from spamWord in spamWordList
            select new XElement("YafSpamWords", new XElement("SpamWord", spamWord.SpamWord)));

        var writer = new System.Xml.Serialization.XmlSerializer(element.GetType());
        var stream = new MemoryStream();
        writer.Serialize(stream, element);

        return this.File(stream.ToArray(), "application/xml", fileName);
    }

    /// <summary>
    /// Called when [get import].
    /// </summary>
    /// <returns>PartialViewResult.</returns>
    public PartialViewResult OnGetImport()
    {
        return new PartialViewResult {
            ViewName = "Dialogs/SpamWordsImport",
            ViewData = new ViewDataDictionary<ImportModal>(
                this.ViewData,
                new ImportModal())
        };
    }

    /// <summary>
    /// Called when [get add].
    /// </summary>
    /// <returns>PartialViewResult.</returns>
    public PartialViewResult OnGetAdd()
    {
        return new PartialViewResult {
            ViewName = "Dialogs/SpamWordsEdit",
            ViewData = new ViewDataDictionary<SpamWordsEditModal>(
                this.ViewData,
                new SpamWordsEditModal { Id = 0 })
        };
    }

    /// <summary>
    /// Called when [get edit].
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>PartialViewResult.</returns>
    public PartialViewResult OnGetEdit(int id)
    {
        // Edit
        var spamWord = this.GetRepository<SpamWords>().GetById(id);

        return new PartialViewResult {
            ViewName = "Dialogs/SpamWordsEdit",
            ViewData = new ViewDataDictionary<SpamWordsEditModal>(
                this.ViewData,
                new SpamWordsEditModal {
                    Id = spamWord.ID,
                    SpamWord = spamWord.SpamWord
                })
        };
    }

    /// <summary>
    /// Loads the Data
    /// </summary>
    private void BindData()
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        var searchText = this.SearchInput;

        List<SpamWords> bannedList;

        if (searchText.IsSet())
        {
            bannedList = this.GetRepository<SpamWords>().GetPaged(
                x => x.BoardID == this.PageBoardContext.PageBoardID && x.SpamWord == searchText,
                this.PageBoardContext.PageIndex,
                this.Size);
        }
        else
        {
            bannedList = this.GetRepository<SpamWords>().GetPaged(
                x => x.BoardID == this.PageBoardContext.PageBoardID,
                this.PageBoardContext.PageIndex,
                this.Size);
        }

        this.List = bannedList;
    }

    /// <summary>
    /// Called when [post delete].
    /// </summary>
    /// <param name="id">The identifier.</param>
    public async Task OnPostDeleteAsync(int id)
    {
        await this.GetRepository<SpamWords>().DeleteByIdAsync(id);

        this.Get<IObjectStore>().Remove(Constants.Cache.SpamWords);

        this.BindData();
    }
}