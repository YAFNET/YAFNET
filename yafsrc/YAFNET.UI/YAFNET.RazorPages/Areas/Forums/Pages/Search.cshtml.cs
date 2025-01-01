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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages;

using System.Collections.Generic;
using System.Web;

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Extensions;
using YAF.Types.Extensions;

/// <summary>
/// Class SearchModel.
/// Implements the <see cref="YAF.Core.BasePages.ForumPage" />
/// </summary>
/// <seealso cref="YAF.Core.BasePages.ForumPage" />
public class SearchModel : ForumPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public SearchInputModel Input { get; set; }

    public List<SelectListItem> SearchWhatList { get; set; }

    public List<SelectListItem> TitleOnlyList { get; set; }

    public List<SelectListItem> ResultsPerPageList { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchModel"/> class.
    /// </summary>
    public SearchModel()
        : base("SEARCH", ForumPages.Search)
    {
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
    }

    /// <summary>
    /// Called when [get].
    /// </summary>
    /// <param name="search">The search.</param>
    /// <param name="forum">The forum.</param>
    /// <param name="postedBy">The posted by.</param>
    /// <param name="tag">The tag.</param>
    public IActionResult OnGet(string search = null, string forum = null, string postedBy = null, string tag = null)
    {
        this.Input = new SearchInputModel();

        var doSearch = false;

        // Load result dropdown
        this.ResultsPerPageList = [
            new SelectListItem { Text = this.GetText("result5"), Value = "5" },
            new SelectListItem { Text = this.GetText("result10"), Value = "10" },
            new SelectListItem { Text = this.GetText("result25"), Value = "25" },
            new SelectListItem { Text = this.GetText("result50"), Value = "50" }
        ];

        // Load listSearchWhat dropdown
        this.SearchWhatList = [
            new SelectListItem {
                Text = this.GetText("match_exact"),
                Value = "2"
            },

            new SelectListItem {
                Text = this.GetText("match_any"),
                Value = "1"
            },

            new SelectListItem {
                Text = this.GetText("match_all"),
                Value = "0"
            }
        ];

        // Load TitleOnly dropdown
        this.TitleOnlyList = [
            new SelectListItem { Text = this.GetText("POST_AND_TITLE"), Value = "0" },
            new SelectListItem { Text = this.GetText("TITLE_ONLY"), Value = "1" }
        ];

        this.Input.SearchWhat = "2";

        // Handle search by url
        var searchString = search;

        if (searchString.IsSet() && searchString.Length < 50)
        {
            this.Input.SearchInput = HttpUtility.UrlDecode(searchString);
            doSearch = true;
        }

        if (searchString.IsSet())
        {
            try
            {
                this.Input.ForumListSelected = forum;
            }
            catch (Exception)
            {
                this.Input.ForumListSelected = "0";
            }
        }

        if (postedBy.IsSet() && postedBy.Length < 50)
        {
            this.Input.SearchStringFromWho = HttpUtility.UrlDecode(postedBy);
            doSearch = true;
        }

        if (tag.IsSet())
        {
            this.Input.SearchStringTag = HttpUtility.UrlDecode(tag);
            doSearch = true;
        }

        if (doSearch)
        {
            this.PageBoardContext.RegisterJsBlock(
                JavaScriptBlocks.DoSearchJs);
        }

        return this.Page();
    }
}