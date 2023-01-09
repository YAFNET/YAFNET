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
using YAF.Types.Modals;
using YAF.Types.Models;

/// <summary>
/// The Admin Profile Definitions Page.
/// </summary>
public class ProfileDefinitionsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    [BindProperty]
    public IList<ProfileDefinition> List { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileDefinitionsModel"/> class. 
    /// </summary>
    public ProfileDefinitionsModel()
        : base("ADMIN_PROFILEDEFINITIONS", ForumPages.Admin_ProfileDefinitions)
    {
    }

    /// <summary>
    /// Creates navigation page links on top of forum (breadcrumbs).
    /// </summary>
    public override void CreatePageLinks()
    {
        // administration index
        this.PageBoardContext.PageLinks.AddAdminIndex();

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_PROFILEDEFINITIONS", "TITLE"));
    }

    public PartialViewResult OnGetAdd()
    {
        return new PartialViewResult {
                                         ViewName = "Dialogs/EditProfileDefinition",
                                         ViewData = new ViewDataDictionary<EditProfileDefinitionModal>(
                                             this.ViewData,
                                             new EditProfileDefinitionModal {
                                                                                Length = 1
                                                                            })
                                     };
    }

    public PartialViewResult OnGetEdit(int id)
    {
        // Edit
        var item = this.GetRepository<ProfileDefinition>().GetById(id);

        return new PartialViewResult {
                                         ViewName = "Dialogs/EditProfileDefinition",
                                         ViewData = new ViewDataDictionary<EditProfileDefinitionModal>(
                                             this.ViewData,
                                             new EditProfileDefinitionModal {
                                                                                Id = item.ID,
                                                                                Name = item.Name,
                                                                                DataType = item.DataType,
                                                                                Length = item.Length,
                                                                                Required = item.Required,
                                                                                ShowInUserInfo = item.ShowInUserInfo,
                                                                                ShowOnRegisterPage =
                                                                                    item.ShowOnRegisterPage,
                                                                                DefaultValue = item.DefaultValue
                                                                            })
                                     };
    }

    public void OnPostDelete(int id)
    {
        this.GetRepository<ProfileCustom>().Delete(x => x.ProfileDefinitionID == id);

        this.GetRepository<ProfileDefinition>().DeleteById(id);

        this.BindData();
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
        // bind data
        this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        // list all access masks for this board
       this.List = this.GetRepository<ProfileDefinition>().GetByBoardId();
    }
}