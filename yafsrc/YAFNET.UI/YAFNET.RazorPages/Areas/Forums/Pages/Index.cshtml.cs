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

using System.Threading.Tasks;

namespace YAF.Pages;

using System.Collections.Generic;
using System.Linq;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The index model (Board Page).
/// </summary>
public class IndexModel : ForumPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndexModel"/> class.
    /// </summary>
    public IndexModel()
        : base("DEFAULT", ForumPages.Index)
    {
    }

    /// <summary>
    /// Gets or sets the categories.
    /// </summary>
    public List<ForumRead> Categories { get; set; }

    /// <summary>
    /// Gets the time now.
    /// </summary>
    public string TimeNow { get; private set; }

    /// <summary>
    /// Gets the last visit.
    /// </summary>
    public string LastVisit { get; private set; }

    /// <summary>
    /// The create page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        if (this.PageBoardContext.PageCategoryID == 0)
        {
            return;
        }

        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public async Task<IActionResult> OnGetAsync()
    {
        // Return 404 for unrecognized handler names that fall through to OnGetAsync
        // (e.g. /asdf would otherwise render the index page)
        if (this.RouteData.Values.TryGetValue("handler", out var handler) && handler is not null)
        {
            return this.NotFound();
        }

        await this.BindDataAsync();

        return this.Page();
    }

    /// <summary>
    /// Toggle Collapse State
    /// </summary>
    /// <param name="target">
    /// The target.
    /// </param>
    public Task OnPostToggleCollapseAsync(string target)
    {
        this.Get<ISessionService>().PanelState.TogglePanelState(target, this.PageBoardContext.BoardSettings.DefaultCollapsiblePanelState);

        return this.BindDataAsync();
    }

    /// <summary>
    /// Load More Forums and Categories
    /// </summary>
    public async Task<IActionResult> OnGetShowMoreAsync(int index)
    {
        if (!this.Get<ISessionService>().Forums.HasItems())
        {
            return this.Partial("_CategoryList", this.Categories);
        }

        this.Get<ISessionService>().BoardForumsIndex = index;


        await this.BindDataAsync(true);

        return this.Partial("_CategoryList", this.Categories);
    }

    /// <summary>
    /// Mark all Forums (of the current category) as Read, not only the forums loaded so far
    /// </summary>
    public Task OnPostMarkAllAsync()
    {
        this.Get<IReadTrackCurrentUser>().SetForumRead(this.ReadableForums().Select(f => f.ID));

        this.PageBoardContext.Notify(this.GetText("MARKALL_MESSAGE"), MessageTypes.success);

        return this.BindDataAsync();
    }

    /// <summary>
    /// Watch All Forums (of the current category) the user can read
    /// </summary>
    public async Task<IActionResult> OnPostWatchAllAsync()
    {
        if (this.PageBoardContext.IsGuest)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        var forums = this.ReadableForums();

        var watchForums = await this.GetRepository<WatchForum>().ListAsync(this.PageBoardContext.PageUserID);

        foreach (var forum in forums.Where(forum => !watchForums.Exists(
                     w => w.ForumID == forum.ID && w.UserID == this.PageBoardContext.PageUserID)))
        {
            await this.GetRepository<WatchForum>().AddAsync(this.PageBoardContext.PageUserID, forum.ID);
        }

        this.PageBoardContext.Notify(this.GetText("SAVED_NOTIFICATION_SETTING"), MessageTypes.success);

        await this.BindDataAsync();

        return this.Page();
    }

    /// <summary>
    /// All forums the user can read, of the current category if one is shown
    /// </summary>
    /// <returns>Returns the forums</returns>
    private IEnumerable<Forum> ReadableForums()
    {
        var categoryId = this.PageBoardContext.PageCategoryID;

        return this.GetRepository<Forum>()
            .ListAllWithAccess(this.PageBoardContext.PageBoardID, this.PageBoardContext.PageUserID)
            .Select(x => x.Item1).Where(forum => categoryId == 0 || forum.CategoryID == categoryId);
    }

    /// <summary>
    /// Bind Data
    /// </summary>
    /// <param name="appendData">
    /// The append Data.
    /// </param>
    private async Task BindDataAsync(bool appendData = false)
    {
        if (appendData)
        {
            var newData = await this.Get<DataBroker>().BoardLayoutAsync(
                this.PageBoardContext.PageBoardID,
                this.PageBoardContext.PageUserID,
                this.Get<ISessionService>().BoardForumsIndex,
                20,
                this.PageBoardContext.PageCategoryID,
                null);

            var forums = this.Get<ISessionService>().Forums;

            foreach (var forum in newData.Where(mod => forums.TrueForAll(x => x.ForumID != mod.ForumID)))
            {
                forums.Add(forum);
            }

            this.Get<ISessionService>().Forums = forums;
        }
        else
        {
            this.Get<ISessionService>().BoardForumsIndex = 0;

            var data = await this.Get<DataBroker>().BoardLayoutAsync(
                this.PageBoardContext.PageBoardID,
                this.PageBoardContext.PageUserID,
                this.Get<ISessionService>().BoardForumsIndex,
                20,
                this.PageBoardContext.PageCategoryID,
                null);
            this.Get<ISessionService>().Forums = data;
        }

        // Filter Categories
        this.Categories = [.. this.Get<ISessionService>().Forums.DistinctBy(x => x.CategoryID)];

        this.TimeNow = this.GetTextFormatted(
            "Current_Time",
            this.Get<IDateTimeService>().FormatTime(DateTime.UtcNow));

        var lastVisit = this.Get<ISessionService>().LastVisit;

        if (lastVisit.HasValue && lastVisit.Value != DateTimeHelper.SqlDbMinTime())
        {
            this.LastVisit = this.GetTextFormatted(
                "last_visit",
                this.Get<IDateTimeService>().FormatDateTime(lastVisit.Value));
        }
    }
}