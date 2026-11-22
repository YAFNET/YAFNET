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
using System.Net;

using ServiceStack.OrmLite;

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
    /// Gets or sets the number of the users that have a value of a definition (by the definition id).
    /// </summary>
    public Dictionary<int, int> UsageCounts { get; set; } = [];

    /// <summary>
    /// Gets the icon of a data type.
    /// </summary>
    /// <param name="dataType">The data type.</param>
    /// <returns>The icon.</returns>
    public static string GetTypeIcon(string dataType)
    {
        return dataType switch
        {
            "Number" => "hashtag",
            "Check" => "square-check",
            _ => "font"
        };
    }

    /// <summary>
    /// Gets the question of the delete dialog of a definition: the values of the users are deleted too.
    /// </summary>
    /// <param name="item">The definition.</param>
    /// <param name="usages">The number of the users with a value.</param>
    /// <returns>The question.</returns>
    public string GetDeleteText(ProfileDefinition item, int usages)
    {
        var name = WebUtility.HtmlEncode(item.Name);

        return usages > 0
                   ? this.GetTextFormatted("DELETE_USED", name, usages)
                   : this.GetTextFormatted("DELETE_UNUSED", name);
    }

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

    /// <summary>
    /// Called when [get add].
    /// </summary>
    /// <returns>PartialViewResult.</returns>
    public PartialViewResult OnGetAdd()
    {
        return new PartialViewResult {
                                         ViewName = "Dialogs/EditProfileDefinition",
                                         ViewData = new ViewDataDictionary<EditProfileDefinitionModal>(
                                             this.ViewData,
                                             new EditProfileDefinitionModal {
                                                                                Id = 0,
                                                                                Length = 1
                                                                            })
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
        var item = this.GetRepository<ProfileDefinition>().GetSingle(x => x.ID == id && x.BoardID == this.PageBoardContext.PageBoardID) ?? new ProfileDefinition();

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

    /// <summary>
    /// On post delete as an asynchronous operation.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task OnPostDeleteAsync(int id)
    {
        // only profile definitions of the board
        if (await this.GetRepository<ProfileDefinition>().ExistsAsync(x => x.ID == id && x.BoardID == this.PageBoardContext.PageBoardID))
        {
            await this.GetRepository<ProfileCustom>().DeleteAsync(x => x.ProfileDefinitionID == id);

            await this.GetRepository<ProfileDefinition>().DeleteByIdAsync(id);
        }

        await this.BindDataAsync();
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public Task OnGetAsync()
    {
        // bind data
        return this.BindDataAsync();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private async Task BindDataAsync()
    {
        // list all access masks for this board
       this.List = await this.GetRepository<ProfileDefinition>().GetByBoardIdAsync();

       // the number of the users with a value of each definition of the board (one query)
       var boardId = this.PageBoardContext.PageBoardID;

       this.UsageCounts = await this.GetRepository<ProfileCustom>().DbAccess.ExecuteAsync(
           db =>
           {
               var expression = OrmLiteConfig.DialectProvider.SqlExpression<ProfileCustom>()
                   .Join<ProfileDefinition>((c, d) => d.ID == c.ProfileDefinitionID)
                   .Where<ProfileDefinition>(d => d.BoardID == boardId)
                   .GroupBy(c => c.ProfileDefinitionID)
                   .Select(c => new { c.ProfileDefinitionID, Count = Sql.CountDistinct(c.UserID) });

               return db.DictionaryAsync<int, int>(expression);
           });
    }
}