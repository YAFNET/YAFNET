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
using System.Linq;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Types.Modals;
using YAF.Types.Models;

/// <summary>
/// The Admin NNTP server page
/// </summary>
public class NntpServersModel : AdminPage
{
    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    [BindProperty]
    public IList<NntpServer> List { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NntpServersModel"/> class. 
    /// </summary>
    public NntpServersModel()
        : base("ADMIN_NNTPSERVERS", ForumPages.Admin_NntpServers)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex()
            .AddLink(this.GetText("ADMIN_NNTPSERVERS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
        this.BindData();
    }

    public PartialViewResult OnGetAdd()
    {
        return new PartialViewResult {
                                         ViewName = "Dialogs/NntpServerEdit",
                                         ViewData = new ViewDataDictionary<NntpServerEditModal>(
                                             this.ViewData,
                                             new NntpServerEditModal {
                                                                         Port = 119
                                                                     })
                                     };
    }

    public PartialViewResult OnGetEdit(int id)
    {
        // Edit
        var server = this.GetRepository<NntpServer>().GetById(id);

        return new PartialViewResult
        {
            ViewName = "Dialogs/NntpServerEdit",
            ViewData = new ViewDataDictionary<NntpServerEditModal>(
                                             this.ViewData,
                                             new NntpServerEditModal
                                             {
                                                 Id = server.ID,
                                                 Name = server.Name,
                                                 Address = server.Address,
                                                 Port = server.Port,
                                                 UserName = server.UserName,
                                                 UserPass = server.UserPass,
                                             })
        };
    }

    public IActionResult OnPostDelete(int id)
    {
        var forums = this.GetRepository<NntpForum>().Get(n => n.NntpServerID == id).Select(forum => forum.ForumID)
            .ToList();

        this.GetRepository<NntpTopic>().DeleteByIds(forums);
        this.GetRepository<NntpForum>().Delete(n => n.NntpServerID == id);
        this.GetRepository<NntpForum>().Delete(n => n.NntpServerID == id);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_NntpServers);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.List = this.GetRepository<NntpServer>().GetByBoardId();
    }
}