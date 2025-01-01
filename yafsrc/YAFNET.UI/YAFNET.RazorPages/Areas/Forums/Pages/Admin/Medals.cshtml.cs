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

namespace YAF.Pages.Admin;

using System.Linq;
using System.Text;

using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Types.Models;

/// <summary>
/// Administration Page for managing medals.
/// </summary>
public class MedalsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    [BindProperty]
    public IOrderedEnumerable<Medal> MedalList { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "MedalsModel" /> class.
    ///   Default constructor.
    /// </summary>
    public MedalsModel()
        : base("ADMIN_MEDALS", ForumPages.Admin_Medals)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        // administration index
        this.PageBoardContext.PageLinks.AddAdminIndex();

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_MEDALS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Delete Medal
    /// </summary>
    /// <param name="medalId">The medal identifier.</param>
    public void OnPostDelete(int medalId)
    {
        // delete medal
        this.GetRepository<UserMedal>().Delete(m => m.MedalID == medalId);
        this.GetRepository<GroupMedal>().Delete(m => m.MedalID == medalId);

        this.GetRepository<Medal>().Delete(m => m.ID == medalId);

        // re-bind data
        this.BindData();
    }

    /// <summary>
    /// Handles page load event.
    /// </summary>
    public void OnGet()
    {
        // bind data
        this.BindData();
    }

    /// <summary>
    /// Formats HTML output to display image representation of a medal.
    /// </summary>
    /// <param name="medal">
    /// The Medal.
    /// </param>
    /// <returns>
    /// HTML markup with image representation of a medal.
    /// </returns>
    public string RenderImages(Medal medal)
    {
        var output = new StringBuilder();

        // image of medal
        output.AppendFormat(
            "<img src=\"/{2}/{0}\" alt=\"{1}\" />",
            medal.MedalURL,
            this.GetText("ADMIN_MEDALS", "DISPLAY_BOX"),
            this.Get<BoardFolders>().Medals);

        return output.ToString();
    }

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private void BindData()
    {
        // list medals for this board
        this.MedalList = this.GetRepository<Medal>().Get(m => m.BoardID == this.PageBoardContext.PageBoardID)
            .OrderBy(m => m.Category);
    }
}