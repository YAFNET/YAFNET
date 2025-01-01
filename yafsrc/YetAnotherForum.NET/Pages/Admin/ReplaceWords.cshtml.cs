/* Yet Another Forum.NET
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

using System.Threading.Tasks;

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

using YAF.Core.Extensions;
using YAF.Types.Modals;
using YAF.Types.Models;

/// <summary>
/// The Replace Words Admin Page.
/// </summary>
public class ReplaceWordsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the list.
    /// </summary>
    /// <value>The list.</value>
    [BindProperty]
    public IList<ReplaceWords> List { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReplaceWordsModel"/> class.
    /// </summary>
    public ReplaceWordsModel()
        : base("ADMIN_REPLACEWORDS", ForumPages.Admin_ReplaceWords)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex()
            .AddLink(this.GetText("ADMIN_REPLACEWORDS", "TITLE"));
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public Task OnGetAsync()
    {
        return this.BindDataAsync();
    }

    /// <summary>
    /// Export the Report Word(s)
    /// </summary>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostExportAsync()
    {
        var spamWordList = await this.GetRepository<ReplaceWords>().GetByBoardIdAsync();

        const string fileName = "ReplaceWordsExport.xml";

        var element = new XElement(
            "YafReplaceWordsList",
            from spamWord in spamWordList
            select new XElement(
                "YafReplaceWords",
                new XElement("BadWord", spamWord.BadWord),
                new XElement("GoodWord", spamWord.GoodWord)));

        var writer = new System.Xml.Serialization.XmlSerializer(element.GetType());
        var stream = new MemoryStream();
        writer.Serialize(stream, element);

        return this.File(stream.ToArray(), "application/xml", fileName);
    }

    /// <summary>
    /// On post delete as an asynchronous operation.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task OnPostDeleteAsync(int id)
    {
        await this.GetRepository<ReplaceWords>().DeleteByIdAsync(id);
        this.Get<IObjectStore>().Remove(Constants.Cache.ReplaceWords);
        await this.BindDataAsync();
    }

    /// <summary>
    /// Called when [get import].
    /// </summary>
    /// <returns>PartialViewResult.</returns>
    public PartialViewResult OnGetImport()
    {
        return new PartialViewResult
               {
                   ViewName = "Dialogs/ReplaceWordsImport",
                   ViewData = new ViewDataDictionary<ImportModal>(
                       this.ViewData,
                       new ImportModal())
               };
    }

    /// <summary>
    /// Open Add ReplaceWords Dialog.
    /// </summary>
    /// <returns>PartialViewResult.</returns>
    public PartialViewResult OnGetAdd()
    {
        return new PartialViewResult {
                                         ViewName = "Dialogs/ReplaceWordsEdit",
                                         ViewData = new ViewDataDictionary<ReplaceWordsEditModal>(
                                             this.ViewData,
                                             new ReplaceWordsEditModal { Id = 0 })
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
        var replaceWord = this.GetRepository<ReplaceWords>().GetById(id);

        return new PartialViewResult {
                                         ViewName = "Dialogs/ReplaceWordsEdit",
                                         ViewData = new ViewDataDictionary<ReplaceWordsEditModal>(
                                             this.ViewData,
                                             new ReplaceWordsEditModal {
                                                                           Id = replaceWord.ID,
                                                                           BadWord = replaceWord.BadWord,
                                                                           GoodWord = replaceWord.GoodWord
                                                                       })
                                     };
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private async Task BindDataAsync()
    {
        this.List = await this.GetRepository<ReplaceWords>().GetByBoardIdAsync();
    }
}