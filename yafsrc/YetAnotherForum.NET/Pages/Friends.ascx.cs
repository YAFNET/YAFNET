/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

using YAF.Controls;

/// <summary>
/// The user Friends Control Panel
/// </summary>
public partial class Friends : ForumPageRegistered
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Friends"/> class.
    /// </summary>
    public Friends()
        : base("FRIENDS", ForumPages.Friends)
    {
    }

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        // setup jQuery and Jquery Ui Tabs.
        this.PageBoardContext.PageElements.RegisterJsBlock(
            "yafBuddiesTabsJs",
            JavaScriptBlocks.BootstrapTabsLoadJs(this.BuddiesTabs.ClientID, this.hidLastTab.ClientID));

        base.OnPreRender(e);
    }

    /// <summary>
    /// Called when the page loads
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(), this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("BUDDYLIST_TT"), string.Empty);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        var buddyList = this.Get<IFriends>().GetForUser(this.PageBoardContext.PageUserID);

        this.InitializeBuddyList(this.BuddyList1, FriendMode.Friends, buddyList);
        this.InitializeBuddyList(this.PendingBuddyList, FriendMode.ReceivedRequests, buddyList);
        this.InitializeBuddyList(this.BuddyRequested, FriendMode.SendRequests, buddyList);
    }

    /// <summary>
    /// Initializes the values of BuddyList control's properties.
    /// </summary>
    /// <param name="customBuddyList">
    /// The custom BuddyList control.
    /// </param>
    /// <param name="mode">
    /// The mode of this BuddyList.
    /// </param>
    /// <param name="buddyList">
    /// The buddy List.
    /// </param>
    private void InitializeBuddyList([NotNull] BuddyList customBuddyList, FriendMode mode, List<BuddyUser> buddyList)
    {
        customBuddyList.FriendsList = buddyList;
        customBuddyList.CurrentUserID = this.PageBoardContext.PageUserID;
        customBuddyList.Mode = mode;
        customBuddyList.Container = this;
    }
}