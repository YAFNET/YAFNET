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

namespace YAF.Pages;

using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using Core.Helpers;
using Core.Model;

using Types.Models;

using YAF.Core.Extensions;
using YAF.Core.Services;

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
    /// Gets or sets the total items.
    /// </summary>
    [BindProperty]
    public int TotalItems { get; set; }

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
            this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));
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
        this.Reset();
    }

    /// <summary>
    /// The get last item class.
    /// </summary>
    /// <param name="itemIndex">
    /// The item index.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetLastItemClass(int itemIndex)
    {
        return itemIndex == this.Stream.Count - 1 ? string.Empty : "border-right";
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.PageSizeList = new SelectList(
            StaticDataHelper.PageEntries(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        this.PageSizeList = new SelectList(
            StaticDataHelper.PageEntries(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        var stream = this.GetRepository<Activity>().Timeline(this.PageBoardContext.PageUserID);

        if (!this.CreatedTopic)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.CreatedTopic);
        }

        if (!this.CreatedReply)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.CreatedReply);
        }

        if (!this.GivenThanks)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.GivenThanks);
        }

        if (!this.BecomeFriends)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.BecomeFriends);
        }

        this.TotalItems = stream.Count;

        var paged = stream.Skip(this.PageBoardContext.PageIndex * this.Size).Take(this.Size).ToList();

        this.Stream = paged;
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