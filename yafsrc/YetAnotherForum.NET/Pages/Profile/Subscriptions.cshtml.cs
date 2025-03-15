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

namespace YAF.Pages.Profile;

using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using Core.Extensions;
using Core.Helpers;
using Core.Model;

using Types.EventProxies;
using Types.Extensions;
using Types.Interfaces.Events;
using Types.Models;

using YAF.Core.Context;

using System.Threading.Tasks;

/// <summary>
/// User Page To Manage Email Subscriptions
/// </summary>
public class SubscriptionsModel : ProfilePage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "SubscriptionsModel" /> class.
    /// </summary>
    public SubscriptionsModel()
        : base("SUBSCRIPTIONS", ForumPages.Profile_Subscriptions)
    {
    }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public SubscriptionsInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the size forums.
    /// </summary>
    [BindProperty]
    public int SizeForums { get; set; } = BoardContext.Current.PageUser.PageSize;

    /// <summary>
    /// Gets or sets the page size forums.
    /// </summary>
    public SelectList PageSizeForums { get; set; }

    /// <summary>
    /// Gets or sets the size topics.
    /// </summary>
    [BindProperty]
    public int SizeTopics { get; set; } = BoardContext.Current.PageUser.PageSize;

    /// <summary>
    /// Gets or sets the page size topics.
    /// </summary>
    public SelectList PageSizeTopics { get; set; }

    /// <summary>
    /// Gets or sets the notification types.
    /// </summary>
    public List<SelectListItem> NotificationTypes { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(), this.Get<ILinkBuilder>().GetLink(ForumPages.MyAccount));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("SUBSCRIPTIONS","TITLE"), string.Empty);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="forums">
    /// The forums.
    /// </param>
    /// <param name="topics">
    /// The topics.
    /// </param>
    public IActionResult OnGet(int forums = 0, int topics = 0)
    {
        this.Input = new SubscriptionsInputModel
        {
                                        NotificationType = this.PageBoardContext.PageUser.NotificationSetting.ToInt().ToString()
                                    };

        this.BindData();

        this.BindDataForums(forums);

        this.BindDataTopics(topics);

        this.UpdateSubscribeUi(this.PageBoardContext.PageUser.NotificationSetting);

        return this.Page();
    }

    /// <summary>
    /// The on post.
    /// </summary>
    /// <param name="forums">
    /// The forums.
    /// </param>
    /// <param name="topics">
    /// The topics.
    /// </param>
    public void OnPost(int forums = 0, int topics = 0)
    {
        this.BindData();

        var selectedValue = this.Input.NotificationType.ToEnum<UserNotificationSetting>();

        this.BindDataForums(forums);
        this.BindDataTopics(topics);

        this.UpdateSubscribeUi(selectedValue);
    }

    /// <summary>
    /// The on post save.
    /// </summary>
    /// <param name="forums">
    /// The forums.
    /// </param>
    /// <param name="topics">
    /// The topics.
    /// </param>
    public void OnPostSave(int forums = 0, int topics = 0)
    {
        this.BindData();

        this.BindDataForums(forums);
        this.BindDataTopics(topics);

        var selectedValue = this.Input.NotificationType.ToEnum<UserNotificationSetting>();

        this.UpdateSubscribeUi(selectedValue);

        var autoWatchTopicsEnabled = selectedValue == UserNotificationSetting.TopicsIPostToOrSubscribeTo;

        // save the settings...
        this.GetRepository<User>().SaveNotification(
            this.PageBoardContext.PageUserID,
            autoWatchTopicsEnabled,
            this.Input.NotificationType.ToType<int>(),
            this.Input.DailyDigestEnabled);

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageBoardContext.PageUserID));

        this.PageBoardContext.Notify(this.GetText("SAVED_NOTIFICATION_SETTING"), MessageTypes.success);
    }

    /// <summary>
    /// The on post unsubscribe forums.
    /// </summary>
    /// <param name="forums">
    /// The forums.
    /// </param>
    /// <param name="topics">
    /// The topics.
    /// </param>
    public async Task<IActionResult> OnPostUnsubscribeForumsAsync(int forums = 0, int topics = 0)
    {
        var items = this.Input.Forums.Where(x => x.Selected).Select(x => x.ID).ToList();

        if (items.Count != 0)
        {
            await this.GetRepository<WatchForum>().DeleteByIdsAsync(items);
        }
        else
        {
            this.PageBoardContext.SessionNotify(this.GetText("WARN_SELECTFORUMS"), MessageTypes.warning);
        }

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Profile_Subscriptions);
    }

    /// <summary>
    /// The on post unsubscribe topics.
    /// </summary>
    /// <param name="forums">
    /// The forums.
    /// </param>
    /// <param name="topics">
    /// The topics.
    /// </param>
    public async Task<IActionResult> OnPostUnsubscribeTopicsAsync(int forums = 0, int topics = 0)
    {
        var items = this.Input.Topics.Where(x => x.Selected).Select(x => x.ID).ToList();

        if (items.Count != 0)
        {
            await this.GetRepository<WatchTopic>().DeleteByIdsAsync(items);
        }
        else
        {
            this.PageBoardContext.SessionNotify(this.GetText("WARN_SELECTTOPICS"), MessageTypes.warning);
        }

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Profile_Subscriptions);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.PageSizeForums = new SelectList(
            StaticDataHelper.PageEntries(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        this.PageSizeTopics = new SelectList(
            StaticDataHelper.PageEntries(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        this.NotificationTypes = [];

        var items = EnumHelper.EnumToDictionary<UserNotificationSetting>();

        items.ForEach(x => this.NotificationTypes.Add(new SelectListItem(this.GetText(x.Value), x.Key.ToString())));
    }

    /// <summary>
    /// The update subscribe UI.
    /// </summary>
    /// <param name="selectedValue">
    /// The selected value.
    /// </param>
    private void UpdateSubscribeUi(UserNotificationSetting selectedValue)
    {
        var showSubscribe = selectedValue is not UserNotificationSetting.NoNotification;

        this.Input.ShowSubscribeList = showSubscribe;
    }

    /// <summary>
    /// The bind data forums.
    /// </summary>
    /// <param name="pageIndex">
    /// The page Index.
    /// </param>
    private void BindDataForums(int pageIndex)
    {
        if (pageIndex > 0)
        {
            pageIndex--;
        }

        var list = this.GetRepository<WatchForum>().List(
            this.PageBoardContext.PageUserID,
            pageIndex,
            this.SizeForums);

        if (list is null)
        {
            return;
        }

        this.Input.Forums = list;
    }

    /// <summary>
    /// The bind data topics.
    /// </summary>
    /// <param name="pageIndex">
    /// The page Index.
    /// </param>
    private void BindDataTopics(int pageIndex)
    {
        if (pageIndex > 0)
        {
            pageIndex--;
        }

        var list = this.GetRepository<WatchTopic>().List(
            this.PageBoardContext.PageUserID,
            pageIndex,
            this.SizeTopics);

        if (list is null)
        {
            return;
        }

        this.Input.Topics = list;
    }
}