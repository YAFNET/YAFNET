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

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The my topics page.
/// </summary>
public class MyTopicsModel : ForumPageRegistered
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public MyTopicsInputModel Input { get; set; }

    /// <summary>
    ///   default since date is now
    /// </summary>
    private DateTime sinceDate;

    /// <summary>
    /// Gets or sets the topic list.
    /// </summary>
    /// <value>The topic list.</value>
    public List<PagedTopic> TopicList { get; set; }

    /// <summary>
    /// Gets or sets the since select items.
    /// </summary>
    /// <value>The since.</value>
    [BindProperty]
    public List<SelectListItem> Since { get; set; }

    /// <summary>
    /// Gets or sets the topic mode.
    /// </summary>
    /// <value>The topic mode.</value>
    [BindProperty]
    public List<SelectListItem> TopicMode { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTopicsModel"/> class.
    /// </summary>
    public MyTopicsModel()
        : base("MYTOPICS", ForumPages.MyTopics)
    {
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(), this.Get<ILinkBuilder>().GetLink(ForumPages.MyAccount));

        this.PageBoardContext.PageLinks.AddLink(this.GetText("MEMBERTITLE"), string.Empty);
    }

    /// <summary>
    /// The Page_ Load Event.
    /// </summary>
    public  void OnGet()
    {
        this.Input = new MyTopicsInputModel
        {
            // default since option is "since last visit"
            SinceValue = 0
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

    /// <summary>
    /// The bind data.
    /// </summary>
    public void BindData()
    {
        this.LoadControls();

        // we'll hold topics in this table
        this.TopicList = null;

        // page index in db which is returned back  is +1 based!
        var currentPageIndex = this.PageBoardContext.PageIndex;

        // default since date is now
        this.sinceDate = DateTime.UtcNow;

        this.sinceDate = this.Input.SinceValue switch
        {
            // decrypt selected option
            9999 => DateTimeHelper.SqlDbMinTime(),
            > 0 => DateTime.UtcNow - TimeSpan.FromDays(this.Input.SinceValue),
            < 0 => DateTime.UtcNow + TimeSpan.FromHours(this.Input.SinceValue),
            _ => this.sinceDate
        };

        // we want to filter topics since last visit
        if (this.Input.SinceValue == 0)
        {
            this.sinceDate = this.Get<ISessionService>().LastVisit ?? DateTime.UtcNow;
        }

        this.TopicList = this.Input.TopicModeValue.ToEnum<TopicListMode>() switch
        {
            TopicListMode.Active => this.GetRepository<Topic>()
                .ListActivePaged(this.PageBoardContext.PageUserID, this.sinceDate, DateTime.UtcNow, currentPageIndex,
                    this.Size, this.PageBoardContext.BoardSettings.UseReadTrackingByDatabase),
            TopicListMode.Unanswered => this.GetRepository<Topic>()
                .ListUnansweredPaged(this.PageBoardContext.PageUserID, this.sinceDate, DateTime.UtcNow,
                    currentPageIndex, this.Size, this.PageBoardContext.BoardSettings.UseReadTrackingByDatabase),
            TopicListMode.Watch => this.GetRepository<Topic>()
                .ListWatchedPaged(this.PageBoardContext.PageUserID, this.sinceDate, DateTime.UtcNow, currentPageIndex,
                    this.Size, this.PageBoardContext.BoardSettings.UseReadTrackingByDatabase),
            TopicListMode.User => this.GetRepository<Topic>()
                .ListByUserPaged(this.PageBoardContext.PageUserID, this.sinceDate, DateTime.UtcNow, currentPageIndex,
                    this.Size, this.PageBoardContext.BoardSettings.UseReadTrackingByDatabase),
            _ => this.TopicList
        };
    }

    /// <summary>
    /// Mark all Topics in the List as Read
    /// </summary>
    public void OnPostMarkAll()
    {
        this.BindData();

        if (this.TopicList.Count <= 0)
        {
            return;
        }

        this.TopicList.ForEach(
                item => this.Get<IReadTrackCurrentUser>().SetTopicRead(item.TopicID));

        // Rebind
        this.BindData();
    }

    /// <summary>
    /// The load and bind controls.
    /// </summary>
    private void LoadControls()
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        // Load Topic Mode
        this.TopicMode = [
            ..new SelectList(StaticDataHelper.TopicListModes(), nameof(SelectListItem.Value),
                nameof(SelectListItem.Text))
        ];

        this.InitSinceDropdown();
    }

    /// <summary>
    /// Initializes dropdown with options to filter results by date.
    /// </summary>
    private void InitSinceDropdown()
    {
        var lastVisit = this.Get<ISessionService>().LastVisit;

        this.Since = [
            new SelectListItem(
                this.GetTextFormatted(
                    "last_visit",
                    this.Get<IDateTimeService>().FormatDateTime(
                        lastVisit.HasValue && lastVisit.Value != DateTimeHelper.SqlDbMinTime()
                            ? lastVisit.Value
                            : DateTime.UtcNow)),
                "0"),
            // negative values for hours backward

            new SelectListItem(this.GetText("last_hour"), "-1"),
            new SelectListItem(this.GetText("last_two_hours"), "-2"),
            new SelectListItem(this.GetText("last_eight_hours"), "-8"),
            // positive values for days backward
            new SelectListItem(this.GetText("last_day"), "1"),
            new SelectListItem(this.GetText("last_two_days"), "2"),
            new SelectListItem(this.GetText("last_week"), "7"),
            new SelectListItem(this.GetText("last_two_weeks"), "14"),
            new SelectListItem(this.GetText("last_month"), "31"),
            new SelectListItem(this.GetText("show_all"), "9999")
        ];
    }
}