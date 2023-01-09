/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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
    [BindProperty]
    public IList<Replace_Words> List { get; set; }

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
    public void OnGet()
    {
        this.BindData();
    }

    /// <summary>
    /// Export the Report Word(s)
    /// </summary>
    /// <returns>IActionResult.</returns>
    public IActionResult OnPostExport()
    {
        var spamWordList = this.GetRepository<Replace_Words>().GetByBoardId();

        const string FileName = "ReplaceWordsExport.xml";

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

        return this.File(stream.ToArray(), "application/xml", FileName);
    }

    public void OnPostDelete(int id)
    {
        this.GetRepository<Replace_Words>().DeleteById(id);
        this.Get<IObjectStore>().Remove(Constants.Cache.ReplaceWords);
        this.BindData();
    }

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

    public PartialViewResult OnGetAdd()
    {
        return new PartialViewResult
               {
                   ViewName = "Dialogs/ReplaceWordsEdit",
                   ViewData = new ViewDataDictionary<ReplaceWordsEditModal>(
                       this.ViewData,
                       new ReplaceWordsEditModal())
               };
    }

    public PartialViewResult OnGetEdit(int id)
    {
        // Edit
        var replaceWord = this.GetRepository<Replace_Words>().GetById(id);

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
    private void BindData()
    {
        this.List = this.GetRepository<Replace_Words>().GetByBoardId();
    }
}