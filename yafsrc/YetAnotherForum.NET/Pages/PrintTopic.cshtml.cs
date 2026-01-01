
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

namespace YAF.Pages;

using System.Collections.Generic;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// Print topic Page.
/// </summary>
public class PrintTopicModel : ForumPage
{
    /// <summary>
    /// Gets or sets the announcements.
    /// </summary>
    [BindProperty]
    public List<PagedMessage> Posts { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "PrintTopicModel" /> class.
    /// </summary>
    public PrintTopicModel()
        : base("PRINTTOPIC", ForumPages.PrintTopic)
    {
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);

        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);
        this.PageBoardContext.PageLinks.AddTopic(this.PageBoardContext.PageTopic);
    }

    /// <summary>
    /// Called when [get].
    /// </summary>
    /// <param name="t">The topic Id.</param>
    public IActionResult OnGet(int? t = null)

    {
        if (!t.HasValue || !this.PageBoardContext.ForumReadAccess)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        var showDeleted = this.PageBoardContext.BoardSettings.ShowDeletedMessagesToAll;

        var posts = this.GetRepository<Message>().PostListPaged(
            this.PageBoardContext.PageTopicID,
            this.PageBoardContext.PageUserID,
            !this.PageBoardContext.IsCrawler,
            showDeleted,
            DateTimeHelper.SqlDbMinTime(),
            DateTime.UtcNow,
            0,
            500,
            -1);

        this.Posts = posts;

        return this.Page();
    }
}