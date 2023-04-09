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

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Attributes;
using YAF.Types.Models;

/// <summary>
/// The Admin Retrieve NNTP Articles Page
/// </summary>
public class NntpRetrieveModel : AdminPage
{
    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    [BindProperty]
    public IEnumerable<Tuple<NntpForum, NntpServer, Forum>> List { get; set; }

    [BindProperty]
    public int Seconds { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NntpRetrieveModel"/> class. 
    /// </summary>
    public NntpRetrieveModel()
        : base("ADMIN_NNTPRETRIEVE", ForumPages.Admin_NntpRetrieve)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_NNTPRETRIEVE", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Gets the last message number
    /// </summary>
    /// <param name="forum">
    /// The forum.
    /// </param>
    /// <returns>
    /// The last message no.
    /// </returns>
    public string LastMessageNo([NotNull] Tuple<NntpForum, NntpServer, Forum> forum)
    {
        return $"{forum.Item1.LastMessageNo:N0}";
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
        this.Seconds = 30;

        this.BindData();
    }

    /// <summary>
    /// Retrieves the click.
    /// </summary>
    public IActionResult OnPostRetrieve()
    {
        if (this.Seconds < 1)
        {
            this.Seconds = 1;
        }

        var articleCount = this.Get<INewsreader>()
            .ReadArticles(
                this.PageBoardContext.PageBoardID,
                10,
                this.Seconds,
                this.PageBoardContext.BoardSettings.CreateNntpUsers);

        this.BindData();

        return this.PageBoardContext.Notify(
            string
                .Format(this.GetText("ADMIN_NNTPRETRIEVE", "Retrieved"), articleCount, (double)articleCount / this.Seconds),
            MessageTypes.success);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.List = this.GetRepository<NntpForum>().NntpForumList(this.PageBoardContext.PageBoardID, true)
            .Where(n => (n.Item1.LastUpdate - DateTime.UtcNow).Minutes > 10);
    }
}