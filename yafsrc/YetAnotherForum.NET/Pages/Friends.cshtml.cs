/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Services;
using YAF.Types.Objects.Model;

using System.Collections.Generic;
using System.Linq;

using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The Edit PageUser Album Images Page.
/// </summary>
public class FriendsModel : ForumPageRegistered
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "FriendsModel" /> class.
    /// </summary>
    public FriendsModel()
        : base("FRIENDS", ForumPages.Friends)
    {
    }

    [BindProperty]
    public int Mode { get; set; }

    [BindProperty]
    public int ListCount { get; set; }

    [BindProperty]
    public List<BuddyUser> ListView { get; set; }

    public SelectList FriendListModes { get; set; }

    public string Header { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(
            this.PageBoardContext.PageUser.DisplayOrUserName(),
            this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("BUDDYLIST_TT"), string.Empty);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public void OnGet()
    {
        this.Mode = FriendMode.Friends.ToInt();

        this.BindData();
    }

    public void OnPost()
    {
        this.BindData();
    }

    public IActionResult OnPostRemove(int userId)
    {
        this.PageBoardContext.SessionNotify(
            string.Format(this.GetText("REMOVEBUDDY_NOTIFICATION"), this.Get<IFriends>().Remove(userId)),
            MessageTypes.success);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Friends);
    }

    public IActionResult OnPostRemoveRequest(int userId)
    {
        this.GetRepository<Buddy>().RemoveRequest(userId);

        this.PageBoardContext.SessionNotify(this.GetText("NOTIFICATION_REQUESTREMOVED"), MessageTypes.success);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Friends);
    }

    public IActionResult OnPostApprove(int userId)
    {
        this.PageBoardContext.SessionNotify(
            string.Format(
                this.GetText("NOTIFICATION_BUDDYAPPROVED"),
                this.Get<IFriends>().ApproveRequest(userId, false)),
            MessageTypes.success);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Friends);
    }

    public IActionResult OnPostApproveAdd(int userId)
    {
        this.PageBoardContext.SessionNotify(
            string.Format(
                this.GetText("NOTIFICATION_BUDDYAPPROVED_MUTUAL"),
                this.Get<IFriends>().ApproveRequest(userId, true)),
            MessageTypes.success);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Friends);
    }

    public IActionResult OnPostApproveAll()
    {
        this.Get<IFriends>().ApproveAllRequests(false);

        this.PageBoardContext.SessionNotify(this.GetText("NOTIFICATION_ALL_APPROVED"), MessageTypes.success);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Friends);
    }

    public IActionResult OnPostApproveAddAll()
    {
        this.Get<IFriends>().ApproveAllRequests(true);

        this.PageBoardContext.SessionNotify(this.GetText("NOTIFICATION_ALL_APPROVED_ADDED"), MessageTypes.success);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Friends);
    }

    public IActionResult OnPostDeny(int userId)
    {
        this.Get<IFriends>().DenyRequest(userId);

        this.PageBoardContext.SessionNotify(this.GetText("NOTIFICATION_BUDDYDENIED"), MessageTypes.info);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Friends);
    }

    public IActionResult OnPostDenyAll()
    {
        this.Get<IFriends>().DenyAllRequests();

        this.PageBoardContext.SessionNotify(this.GetText("NOTIFICATION_ALL_DENIED"), MessageTypes.info);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Friends);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.FriendListModes = new SelectList(
            StaticDataHelper.FriendListModes(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));
        this.PageSizeList = new SelectList(
            StaticDataHelper.PageEntries(),
            nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        this.ListView = new List<BuddyUser>();

        var mode = this.Mode.ToEnum<FriendMode>();

        var friends = new List<BuddyUser>();

        // In what mode should this control work?
        // Refer to "rptBuddy_ItemCreate" event for more info.
        switch (mode)
        {
            case FriendMode.Friends:
                friends = this.GetRepository<Buddy>().GetAllFriends(this.PageBoardContext.PageUserID);
                break;
            case FriendMode.ReceivedRequests:
                friends = this.GetRepository<Buddy>().GetReceivedRequests(this.PageBoardContext.PageUserID);
                break;
            case FriendMode.SendRequests:
                friends = this.GetRepository<Buddy>().GetSendRequests(this.PageBoardContext.PageUserID);
                break;
        }

        this.ListCount = friends.Count;

        var pager = new Paging { CurrentPageIndex = this.PageBoardContext.PageIndex, PageSize = this.Size, Count = this.ListView.Count};

        this.ListView = friends.GetPaged(pager).ToList();
        
        this.Header = mode switch {
            FriendMode.ReceivedRequests => this.GetText("FRIENDS", "PENDING_REQUESTS"),
            FriendMode.SendRequests => this.GetText("FRIENDS", "YOUR_REQUESTS"),
            _ => this.Get<ILocalization>().GetText("FRIENDS", "BUDDYLIST")
        };
    }
}