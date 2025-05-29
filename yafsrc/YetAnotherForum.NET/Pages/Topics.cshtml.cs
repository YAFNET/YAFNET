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

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
/// The topics list page
/// </summary>
public class TopicsModel : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "TopicsModel" /> class.
    /// </summary>
    public TopicsModel()
        : base("TOPICS", ForumPages.Topics)
    {
    }

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public SelectList ShowList { get; set; }

    /// <summary>
    /// Gets or sets the sub forums.
    /// </summary>
    public Tuple<List<SimpleModerator>, List<ForumRead>> SubForums { get; set; }

    /// <summary>
    /// Gets or sets the announcements.
    /// </summary>
    [BindProperty]
    public List<PagedTopic> Announcements { get; set; }

    /// <summary>
    /// Gets or sets the topics.
    /// </summary>
    [BindProperty]
    public List<PagedTopic> Topics { get; set; }

    /// <summary>
    /// Gets or sets the show topic list selected.
    /// </summary>
    [BindProperty]
    public int ShowTopicListSelected { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);

        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum, true);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public IActionResult OnGet()
    {
        this.ShowTopicListSelected = this.ShowTopicListSelected == -1
                                         ? this.PageBoardContext.BoardSettings.ShowTopicsDefault
                                         : this.ShowTopicListSelected;

        if (this.PageBoardContext.IsGuest && !this.PageBoardContext.ForumReadAccess)
        {
            // attempt to get permission by redirecting to login...
            var result = this.Get<IPermissions>().HandleRequest(ViewPermissions.RegisteredUsers);

            if (result != null)
            {
                return result;
            }
        }
        else if (!this.PageBoardContext.ForumReadAccess)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        if (this.PageBoardContext.PageForum.RemoteURL.IsSet())
        {
            return this.Redirect(this.PageBoardContext.PageForum.RemoteURL);
        }

        this.BindData();

        return this.Page();
    }

    /// <summary>
    /// Toggle Collapse State
    /// </summary>
    /// <param name="target">
    /// The target.
    /// </param>
    public void OnPostToggleCollapse(string target)
    {
        this.Get<ISessionService>().PanelState.TogglePanelState(target, this.PageBoardContext.BoardSettings.DefaultCollapsiblePanelState);

        this.BindData();
    }

    /// <summary>
    /// Change Topics Count
    /// </summary>
    public void OnPostPageSize()
    {
        this.BindData();
    }

    /// <summary>
    /// Change Topics listed by x Date
    /// </summary>
    public void OnPostShowList()
    {
        this.BindData();
    }

    /// <summary>
    /// Search Current Forum
    /// </summary>
    /// <param name="forumSearch">
    /// The forum search.
    /// </param>
    public IActionResult OnPostForumSearch(string forumSearch)
    {
        if (forumSearch.IsSet())
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Search,
                new { search = forumSearch, forum = this.PageBoardContext.PageForumID });
        }

        this.BindData();
        return this.Page();
    }

    /// <summary>
    /// Add / Remove Watch Forum
    /// </summary>
    public void OnPostWatchForum()
    {
        if (!this.PageBoardContext.ForumReadAccess)
        {
            return;
        }

        if (this.PageBoardContext.IsGuest)
        {
            this.PageBoardContext.Notify(this.GetText("WARN_LOGIN_FORUMWATCH"), MessageTypes.warning);
            return;
        }

        var watchForumId = this.GetRepository<WatchForum>().Check(this.PageBoardContext.PageUserID, this.PageBoardContext.PageForumID);

        if (watchForumId.HasValue)
        {
            this.GetRepository<WatchForum>().Delete(
                w => w.ForumID == this.PageBoardContext.PageForumID
                     && w.UserID == this.PageBoardContext.PageUserID);

            this.PageBoardContext.Notify(this.GetText("INFO_UNWATCH_FORUM"), MessageTypes.success);
        }
        else
        {
            this.GetRepository<WatchForum>().Add(this.PageBoardContext.PageUserID, this.PageBoardContext.PageForumID);

            this.PageBoardContext.Notify(this.GetText("INFO_WATCH_FORUM"), MessageTypes.success);
        }

        this.BindData();
    }

    /// <summary>
    /// Mark Forum as Read
    /// </summary>
    public void OnPostMarkRead()
    {
        this.Get<IReadTrackCurrentUser>().SetForumRead(this.PageBoardContext.PageForumID);

        this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.ShowList = new SelectList(
            StaticDataHelper.TopicTimes(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        var forums = this.Get<DataBroker>().BoardLayout(
            this.PageBoardContext.PageBoardID,
            this.PageBoardContext.PageUserID,
            0,
            20000,
            this.PageBoardContext.PageCategoryID,
            this.PageBoardContext.PageForumID);

        // Render Sub forum(s)
        if (forums.Item2.Count != 0)
        {
            this.SubForums = forums;
        }

        this.Announcements = this.GetRepository<Topic>().ListAnnouncementsPaged(
            this.PageBoardContext.PageForumID,
            this.PageBoardContext.PageUserID,
            0,
            10,
            this.PageBoardContext.BoardSettings.UseReadTrackingByDatabase);

        int[] days = [1, 2, 7, 14, 31, 2 * 31, 6 * 31, 356];

        this.Topics = this.GetRepository<Topic>().ListPaged(
            this.PageBoardContext.PageForumID,
            this.PageBoardContext.PageUserID,
            this.ShowTopicListSelected == 0 ? DateTimeHelper.SqlDbMinTime() : DateTime.UtcNow.AddDays(-days[this.ShowTopicListSelected]),
            this.PageBoardContext.PageIndex,
            this.Size,
            this.PageBoardContext.BoardSettings.UseReadTrackingByDatabase);
    }
}