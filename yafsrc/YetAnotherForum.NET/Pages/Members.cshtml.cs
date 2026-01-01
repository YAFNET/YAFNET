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
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The Team Page
/// </summary>
public class MembersModel : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "MembersModel" /> class.
    /// </summary>
    public MembersModel()
        : base("MEMBERS", ForumPages.Members)
    {
    }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public MembersInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the admins.
    /// </summary>
    [BindProperty]
    public List<PagedUser> UserList { get; set; }

    /// <summary>
    /// Gets or sets the groups list.
    /// </summary>
    /// <value>The groups list.</value>
    [BindProperty]
    public List<SelectListItem> GroupsList { get; set; }

    /// <summary>
    /// Gets or sets the ranks list.
    /// </summary>
    /// <value>The ranks list.</value>
    [BindProperty]
    public List<SelectListItem> RanksList { get; set; }

    /// <summary>
    /// Gets or sets the number post list.
    /// </summary>
    /// <value>The number post list.</value>
    [BindProperty]
    public List<SelectListItem> NumPostList { get; set; }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.GetText("MEMBERS","TITLE"), string.Empty);
    }

    /// <summary>
    ///   Gets actually selected letter.
    /// </summary>
    public char CurrentLetter {
        get {
            var currentLetter = char.MinValue;

            var value = this.Request.GetQueryOrRouteValue<string>("letter");

            if (value is null)
            {
                return currentLetter;
            }

            // try to convert to char
            _ = char.TryParse(value, out currentLetter);

            // since we cannot use '#' in URL, we use '_' instead, this is to give it the right meaning
            if (currentLetter == '_')
            {
                currentLetter = '#';
            }

            return currentLetter;
        }
    }

    /// <summary>
    /// Handles the Click event of the Search control.
    /// </summary>
    public void OnPostSearch()
    {
        // re-bind data
        this.BindData();
    }

    /// <summary>
    /// Handles the Click event of the Reset control.
    /// </summary>
    public IActionResult OnPostReset()
    {
        // re-direct to self.
        return this.Get<ILinkBuilder>().Redirect(ForumPages.Members);
    }

    /// <summary>
    /// Get all users from for this board.
    /// </summary>
    /// <param name="startLetter">
    /// The start Letter.
    /// </param>
    /// <param name="userName">
    /// The username.
    /// </param>
    /// <returns>
    /// The Members List
    /// </returns>
    protected void GetUserList(char startLetter, string userName)
    {
        this.UserList = this.Get<IAspNetUsersHelper>().ListMembersPaged(
            this.PageBoardContext.PageBoardID,
            this.Input.Group == 0 ? null : this.Input.Group,
            this.Input.Rank == 0 ? null : this.Input.Rank,
            startLetter,
            userName,
            this.PageBoardContext.PageIndex,
            this.Size,
            this.Input.SortNameField,
            this.Input.SortRankNameField,
            this.Input.SortJoinedField,
            this.Input.SortNumPostsField,
            this.Input.SortLastVisitField,
            this.Input.NumPosts,
            this.Input.NumPostValue);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
        this.Input = new MembersInputModel {
                                        SortNameField = 1,
                                        SortRankNameField = 0,
                                        SortJoinedField = 0,
                                        SortNumPostsField = 0,
                                        SortLastVisitField = 0
                                    };

        this.BindData();
    }

    /// <summary>
    /// Called when [post].
    /// </summary>
    public void OnPost()
    {
        this.BindData();
    }

    public void OnPostPager(int p)
    {
        this.BindData();
    }

    /// <summary>
    /// Sort by Joined ascending
    /// </summary>
    public void OnPostJoinedAsc()
    {
        this.SetSort("Joined", 1);

        this.Input.SortNameField = 0;
        this.Input.SortRankNameField = 0;
        this.Input.SortNumPostsField = 0;
        this.Input.SortLastVisitField = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by Joined descending
    /// </summary>
    public void OnPostJoinedDesc()
    {
        this.SetSort("Joined", 2);

        this.Input.SortNameField = 0;
        this.Input.SortRankNameField = 0;
        this.Input.SortNumPostsField = 0;
        this.Input.SortLastVisitField = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by Last Visit ascending
    /// </summary>
    public void OnPostLastVisitAsc()
    {
        this.SetSort("LastVisit", 1);

        this.Input.SortNameField = 0;
        this.Input.SortRankNameField = 0;
        this.Input.SortJoinedField = 0;
        this.Input.SortNumPostsField = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by Last visit descending
    /// </summary>
    public void OnPostLastVisitDesc()
    {
        this.SetSort("LastVisit", 2);

        this.Input.SortNameField = 0;
        this.Input.SortRankNameField = 0;
        this.Input.SortJoinedField = 0;
        this.Input.SortNumPostsField = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by Posts ascending
    /// </summary>
    public void OnPostPostsAsc()
    {
        this.SetSort("NumPosts", 1);

        this.Input.SortNameField = 0;
        this.Input.SortRankNameField = 0;
        this.Input.SortJoinedField = 0;
        this.Input.SortLastVisitField = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by Posts descending
    /// </summary>
    public void OnPostPostsDesc()
    {
        this.SetSort("NumPosts", 2);

        this.Input.SortNameField = 0;
        this.Input.SortRankNameField = 0;
        this.Input.SortJoinedField = 0;
        this.Input.SortLastVisitField = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by Rank ascending
    /// </summary>
    public void OnPostRankAsc()
    {
        this.SetSort("RankName", 1);

        this.Input.SortNameField = 0;
        this.Input.SortJoinedField = 0;
        this.Input.SortNumPostsField = 0;
        this.Input.SortLastVisitField = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by rank descending
    /// </summary>
    public void OnPostRankDesc()
    {
        this.SetSort("RankName", 2);

        this.Input.SortNameField = 0;
        this.Input.SortJoinedField = 0;
        this.Input.SortNumPostsField = 0;
        this.Input.SortLastVisitField = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by User name ascending
    /// </summary>
    public void OnPostUserNameAsc()
    {
        this.SetSort("Name", 1);

        this.Input.SortRankNameField = 0;
        this.Input.SortJoinedField = 0;
        this.Input.SortNumPostsField = 0;
        this.Input.SortLastVisitField = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by user name descending
    /// </summary>
    public void OnPostUserNameDesc()
    {
        this.SetSort("Name", 2);

        this.Input.SortRankNameField = 0;
        this.Input.SortJoinedField = 0;
        this.Input.SortNumPostsField = 0;
        this.Input.SortLastVisitField = 0;

        this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.Input.NumPostValue = 3;

        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        this.NumPostList = [
            new SelectListItem(this.GetText("MEMBERS", "NUMPOSTSEQUAL"), "1"),
            new SelectListItem(this.GetText("MEMBERS", "NUMPOSTSLESSOREQUAL"), "2"),
            new SelectListItem(this.GetText("MEMBERS", "NUMPOSTSMOREOREQUAL"), "3")
        ];

        // get list of user ranks for filtering
        var ranks = this.GetRepository<Rank>().GetByBoardId().OrderBy(r => r.SortOrder).ToList();

        ranks.Insert(0, new Rank { Name = this.GetText("ALL"), ID = 0 });

        ranks.RemoveAll(r => r.Name == "Guest");

        this.RanksList = [..new SelectList(ranks, nameof(Rank.ID), nameof(Rank.Name))];

        // get list of user ranks for filtering
        var groups = this.GetRepository<Group>().GetByBoardId().Where(g => !g.GroupFlags.IsGuest)
            .OrderBy(r => r.SortOrder).ToList();

        groups.Insert(0, new Group { Name = this.GetText("ALL"), ID = 0 });

        this.GroupsList = [..new SelectList(groups, nameof(Group.ID), nameof(Group.Name))];

        var numberOfPosts = this.Input.NumPosts;

        if (numberOfPosts < 0)
        {
            this.PageBoardContext.Notify(this.GetText("MEMBERS", "INVALIDPOSTSVALUE"), MessageTypes.warning);
            return;
        }

        // get the user list...
        this.GetUserList(
            this.CurrentLetter,
            this.Input.UserSearchName);
    }

    /// <summary>
    /// Helper function for setting up the current sort on
    /// the member list view
    /// </summary>
    /// <param name="field">
    /// The field.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    private void SetSort(string field, int value)
    {
        switch (field)
        {
            case "Name":
                this.Input.SortNameField = value;
                break;
            case "RankName":
                this.Input.SortRankNameField = value;
                break;
            case "Joined":
                this.Input.SortJoinedField = value;
                break;
            case "NumPosts":
                this.Input.SortNumPostsField = value;
                break;
            case "LastVisit":
                this.Input.SortLastVisitField = value;
                break;
        }
    }
}