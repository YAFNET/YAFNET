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
    /// Mark all Forums as Read
    /// </summary>
    public Task OnPostMarkAllAsync()
    {
        var forums = this.Get<ISessionService>().Forums;

        this.Get<IReadTrackCurrentUser>().SetForumRead(forums.Select(f => f.ForumID));

        this.PageBoardContext.Notify(this.GetText("MARKALL_MESSAGE"), MessageTypes.success);

        return this.BindDataAsync();
    }

    /// <summary>
    /// Watch All Forums
    /// </summary>
    public async Task OnPostWatchAllAsync()
    {
        int? categoryId = this.PageBoardContext.PageCategoryID != 0 ? this.PageBoardContext.PageCategoryID : null;

        var forums = this.Get<ISessionService>().Forums.Where(x => x.CategoryID == categoryId);

        var watchForums = await this.GetRepository<WatchForum>().ListAsync(this.PageBoardContext.PageUserID);

        foreach (var forum in forums.Where(forum => !watchForums.Exists(
                     w => w.ForumID == forum.ForumID && w.UserID == this.PageBoardContext.PageUserID)))
        {
            await this.GetRepository<WatchForum>().AddAsync(this.PageBoardContext.PageUserID, forum.ForumID);
        }

        this.PageBoardContext.Notify(this.GetText("SAVED_NOTIFICATION_SETTING"), MessageTypes.success);

        await this.BindDataAsync();
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

            var mods = this.Get<ISessionService>().Mods;

            foreach (var mod in newData.Item1.Where(mod => mods.TrueForAll(x => x.ForumID != mod.ForumID)))
            {
                mods.Add(mod);
            }

            var forums = this.Get<ISessionService>().Forums;

            foreach (var forum in newData.Item2.Where(mod => forums.TrueForAll(x => x.ForumID != mod.ForumID)))
            {
                forums.Add(forum);
            }

            this.Get<ISessionService>().Mods = mods;
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

            this.Get<ISessionService>().Mods = data.Item1;
            this.Get<ISessionService>().Forums = data.Item2;
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