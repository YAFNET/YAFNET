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
using System.Linq;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

using Core.Model;

using Types.Models;

using YAF.Core.Extensions;

/// <summary>
/// The privacy model.
/// </summary>
public class MyAccountModel : ForumPageRegistered
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "MyAccountModel" /> class.
    /// </summary>
    public MyAccountModel()
        : base("ACCOUNT", ForumPages.MyAccount)
    {
    }

    /// <summary>
    /// The groups.
    /// </summary>
    [BindProperty]
    public List<Group> Groups => this.GetRepository<UserGroup>().List(this.PageBoardContext.PageUserID);

    /// <summary>
    /// Gets or sets the stream.
    /// </summary>
    [BindProperty]
    public List<Tuple<Activity, Topic>> Stream { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether created topic.
    /// </summary>
    [BindProperty]
    public bool CreatedTopic { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether created reply.
    /// </summary>
    [BindProperty]
    public bool CreatedReply { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether given thanks.
    /// </summary>
    [BindProperty]
    public bool GivenThanks { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether are friends.
    /// </summary>
    [BindProperty]
    public bool BecomeFriends { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(
            this.PageBoardContext.PageUser.DisplayOrUserName(),
            this.Get<ILinkBuilder>().GetLink(ForumPages.MyAccount));
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public IActionResult OnGet()
    {
        this.Reset();

        return this.Page();
    }

    /// <summary>
    /// The on post.
    /// </summary>
    public void OnPost()
    {
        this.BindData();
    }

    /// <summary>
    /// Reset Filter
    /// </summary>
    public void OnPostReset()
    {
        // Clear stale posted checkbox values so the reset state below is what gets rendered.
        this.ModelState.Clear();

        this.Reset();
    }

    /// <summary>
    /// Loads more Activity for infinite scrolling, also used to (re-)apply the filter checkboxes via AJAX.
    /// </summary>
    /// <param name="page">
    /// The zero-based page index to load.
    /// </param>
    /// <param name="size">
    /// The page size to load.
    /// </param>
    /// <param name="createdTopic">Filter: created topic.</param>
    /// <param name="createdReply">Filter: created reply.</param>
    /// <param name="givenThanks">Filter: given thanks.</param>
    /// <param name="becomeFriends">Filter: become friends.</param>
    public IActionResult OnGetLoadMoreActivity(
        int page,
        int size,
        bool createdTopic,
        bool createdReply,
        bool givenThanks,
        bool becomeFriends)
    {
        var stream = this.GetFilteredStream(createdTopic, createdReply, givenThanks, becomeFriends);

        var paged = stream.Skip(page * size).Take(size).ToList();

        return new PartialViewResult
        {
            ViewName = "_MyAccountActivityListItems",
            ViewData = new ViewDataDictionary<List<Tuple<Activity, Topic>>>(this.ViewData, paged)
        };
    }

    /// <summary>
    /// Gets the current user's activity stream, filtered by the given activity flags.
    /// </summary>
    private List<Tuple<Activity, Topic>> GetFilteredStream(
        bool createdTopic,
        bool createdReply,
        bool givenThanks,
        bool becomeFriends)
    {
        var stream = this.GetRepository<Activity>().Timeline(this.PageBoardContext.PageUserID);

        if (!createdTopic)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.CreatedTopic);
        }

        if (!createdReply)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.CreatedReply);
        }

        if (!givenThanks)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.GivenThanks);
        }

        if (!becomeFriends)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.BecomeFriends);
        }

        return stream;
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        var stream = this.GetFilteredStream(this.CreatedTopic, this.CreatedReply, this.GivenThanks, this.BecomeFriends);

        this.Stream =
        [
            .. stream
                .Skip(this.PageBoardContext.PageIndex * this.Size).Take(this.Size)
        ];
    }

    /// <summary>
    /// Resets Filter and Load Data
    /// </summary>
    private void Reset()
    {
        this.CreatedTopic = true;
        this.CreatedReply = true;
        this.GivenThanks = true;
        this.BecomeFriends = true;

        this.BindData();
    }
}