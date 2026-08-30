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

using Microsoft.AspNetCore.Mvc.ViewFeatures;

using Core.Model;

using Types.Models;

using YAF.Core.Extensions;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces.Events;

/// <summary>
/// The privacy model.
/// </summary>
public class NotificationModel : ForumPageRegistered
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "NotificationModel" /> class.
    /// </summary>
    public NotificationModel()
        : base("NOTIFICATION", ForumPages.Notification)
    {
    }

    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    [BindProperty]
    public List<Tuple<Activity, User, Topic>> Notifications { get; set; }

    /// <summary>
    /// The was mentioned.
    /// </summary>
    [BindProperty]
    public bool WasMentioned { get; set; } = true;

    /// <summary>
    /// The was quoted.
    /// </summary>
    [BindProperty]
    public bool WasQuoted { get; set; } = true;

    /// <summary>
    /// The received thanks.
    /// </summary>
    [BindProperty]
    public bool ReceivedThanks { get; set; } = true;

    /// <summary>
    /// The watch forum reply.
    /// </summary>
    [BindProperty]
    public bool WatchForumReply { get; set; } = true;

    /// <summary>
    /// The watch topic reply.
    /// </summary>
    [BindProperty]
    public bool WatchTopicReply { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether [become friends].
    /// </summary>
    /// <value><c>true</c> if [become friends]; otherwise, <c>false</c>.</value>
    [BindProperty]
    public bool BecomeFriends { get; set; } = true;

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(
            this.PageBoardContext.PageUser.DisplayOrUserName(),
            this.Get<ILinkBuilder>().GetLink(ForumPages.Notification));
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public void OnGet()
    {
        this.BindData();
    }

    /// <summary>
    /// Called when [post update].
    /// </summary>
    /// <returns>IActionResult.</returns>
    public void OnPost()
    {
        this.BindData();
    }

    /// <summary>
    /// Mark all Activity as read
    /// </summary>
    public void OnPostMarkAll()
    {
        this.GetRepository<Activity>().MarkAllAsRead(this.PageBoardContext.PageUserID);

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageBoardContext.PageUserID));

        this.BindData();
    }

    /// <summary>
    /// Reset Filter
    /// </summary>
    public void OnPostReset()
    {
        // Clear stale posted checkbox values so the reset state below is what gets rendered.
        this.ModelState.Clear();

        this.WasMentioned = true;
        this.ReceivedThanks = true;
        this.WasQuoted = true;
        this.WatchForumReply = true;
        this.WatchTopicReply = true;
        this.BecomeFriends = true;

        this.BindData();
    }

    /// <summary>
    /// The activity stream_ on item command.
    /// </summary>
    public void OnPostMarkRead(int id)
    {
        this.GetRepository<Activity>().UpdateNotification(this.PageBoardContext.PageUserID, id);

        this.BindData();
    }

    /// <summary>
    /// Loads more Notifications for infinite scrolling, also used to (re-)apply the filter checkboxes via AJAX.
    /// </summary>
    /// <param name="page">
    /// The zero-based page index to load.
    /// </param>
    /// <param name="size">
    /// The page size to load.
    /// </param>
    /// <param name="wasMentioned">Filter: was mentioned.</param>
    /// <param name="receivedThanks">Filter: received thanks.</param>
    /// <param name="wasQuoted">Filter: was quoted.</param>
    /// <param name="watchForumReply">Filter: watch forum reply.</param>
    /// <param name="watchTopicReply">Filter: watch topic reply.</param>
    /// <param name="becomeFriends">Filter: become friends.</param>
    public IActionResult OnGetLoadMoreNotifications(
        int page,
        int size,
        bool wasMentioned,
        bool receivedThanks,
        bool wasQuoted,
        bool watchForumReply,
        bool watchTopicReply,
        bool becomeFriends)
    {
        var stream = this.GetFilteredNotifications(
            wasMentioned, receivedThanks, wasQuoted, watchForumReply, watchTopicReply, becomeFriends);

        var paged = stream.Skip(page * size).Take(size).ToList();

        return new PartialViewResult
        {
            ViewName = "_NotificationListItems",
            ViewData = new ViewDataDictionary<List<Tuple<Activity, User, Topic>>>(this.ViewData, paged)
        };
    }

    /// <summary>
    /// Gets the current user's notification stream, filtered by the given activity flags.
    /// </summary>
    private List<Tuple<Activity, User, Topic>> GetFilteredNotifications(
        bool wasMentioned,
        bool receivedThanks,
        bool wasQuoted,
        bool watchForumReply,
        bool watchTopicReply,
        bool becomeFriends)
    {
        var stream = this.GetRepository<Activity>().Notifications(this.PageBoardContext.PageUserID);

        if (!wasMentioned)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.WasMentioned);
        }

        if (!receivedThanks)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.ReceivedThanks);
        }

        if (!wasQuoted)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.WasQuoted);
        }

        if (!watchForumReply)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.WatchForumReply);
        }

        if (!watchTopicReply)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.WatchTopicReply);
        }

        if (!becomeFriends)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.BecomeFriends);
        }

        stream.RemoveAll(a => a.Item1.ActivityFlags.GivenThanks);

        return stream;
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        var stream = this.GetFilteredNotifications(
            this.WasMentioned, this.ReceivedThanks, this.WasQuoted, this.WatchForumReply, this.WatchTopicReply, this.BecomeFriends);

        this.Notifications =
        [
            .. stream
                .Skip(this.PageBoardContext.PageIndex * this.Size).Take(this.Size)
        ];
    }
}