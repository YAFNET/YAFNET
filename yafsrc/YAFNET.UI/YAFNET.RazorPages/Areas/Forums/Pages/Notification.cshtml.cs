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
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using Core.Helpers;
using Core.Model;

using Types.Models;

using YAF.Core.Extensions;
using YAF.Core.Services;
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
    /// Gets or sets the notifications count.
    /// </summary>
    /// <value>The notifications count.</value>
    [BindProperty]
    public int NotificationsCount { get; set; }

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
            this.Get<LinkBuilder>().GetLink(ForumPages.Notification));
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public IActionResult OnGet()
    {
        this.BindData();

        return this.Page();
    }

    /// <summary>
    /// Called when [post update].
    /// </summary>
    /// <returns>IActionResult.</returns>
    public IActionResult OnPostUpdate()
    {
        this.BindData();

        return this.Page();
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
    /// The update filter click.
    /// </summary>
    public void OnPostUpdateFilter()
    {
        this.BindData();
    }

    /// <summary>
    /// Reset Filter
    /// </summary>
    public void OnPostReset()
    {
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
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        var stream = this.GetRepository<Activity>().Notifications(this.PageBoardContext.PageUserID);

        if (!this.WasMentioned)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.WasMentioned);
        }

        if (!this.ReceivedThanks)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.ReceivedThanks);
        }

        if (!this.WasQuoted)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.WasQuoted);
        }

        if (!this.WatchForumReply)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.WatchForumReply);
        }

        if (!this.WatchTopicReply)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.WatchTopicReply);
        }

        if (!this.BecomeFriends)
        {
            stream.RemoveAll(a => a.Item1.ActivityFlags.BecomeFriends);
        }

        stream.RemoveAll(a => a.Item1.ActivityFlags.GivenThanks);

        this.NotificationsCount = stream.Count;

        var paged = stream
            .Skip(this.PageBoardContext.PageIndex * this.Size).Take(this.Size).ToList();

        this.Notifications = paged;
    }
}