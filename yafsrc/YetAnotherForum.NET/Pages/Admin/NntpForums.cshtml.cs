
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

using Microsoft.AspNetCore.Mvc.ViewFeatures;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Modals;
using YAF.Types.Models;

/// <summary>
/// Admin Page to Edit NNTP Forums
/// </summary>
public class NntpForumsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    [BindProperty]
    public List<Tuple<NntpForum, NntpServer, Forum>> List { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NntpForumsModel"/> class. 
    /// </summary>
    public NntpForumsModel()
        : base("ADMIN_NNTPFORUMS", ForumPages.Admin_NntpForums)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_NNTPFORUMS", "TITLE"), string.Empty);
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
                                         ViewName = "Dialogs/NntpForumEdit",
                                         ViewData = new ViewDataDictionary<NntpForumEditModal>(
                                             this.ViewData,
                                             new NntpForumEditModal { Id = 0 })
                                     };
    }

    public PartialViewResult OnGetEdit(int id)
    {
        // Edit
        var forum = this.GetRepository<NntpForum>().GetById(id);

        return new PartialViewResult {
                                         ViewName = "Dialogs/NntpForumEdit",
                                         ViewData = new ViewDataDictionary<NntpForumEditModal>(
                                             this.ViewData,
                                             new NntpForumEditModal {
                                                                        Id = forum.ID,
                                                                        NntpServerID = forum.NntpServerID,
                                                                        GroupName = forum.GroupName,
                                                                        ForumID = forum.ForumID,
                                                                        Active = forum.Active,
                                                                        DateCutOff = forum.DateCutOff ?? DateTime.MinValue
                                                                    })
                                     };
    }

    public IActionResult OnPostDelete(int id)
    {
        this.GetRepository<NntpTopic>().Delete(n => n.NntpForumID == id);
        this.GetRepository<NntpForum>().Delete(n => n.ID == id);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_NntpForums);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.List = this.GetRepository<NntpForum>()
            .NntpForumList(this.PageBoardContext.PageBoardID, null);
    }
}