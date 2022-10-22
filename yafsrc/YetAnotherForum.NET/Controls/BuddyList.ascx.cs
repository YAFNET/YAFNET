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

namespace YAF.Controls;

using System.Web.UI;
using YAF.Types.Constants;
using YAF.Types.Models;
using YAF.Web.Controls;

/// <summary>
/// Buddy List Control
/// </summary>
public partial class BuddyList : BaseUserControl
{
    /// <summary>
    ///   The parent control of the current control. (Used in rptBuddy_ItemCommand method)
    /// </summary>
    public Control Container { get; set; }

    /// <summary>
    ///   Gets or sets the user ID.
    /// </summary>
    public int CurrentUserID { get; set; }

    /// <summary>
    ///   Gets or sets the Determines what is th current mode of the control.
    /// </summary>
    public FriendMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the count.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the Friends Table.
    /// </summary>
    public List<BuddyUser> FriendsList { get; set; }

    /// <summary>
    /// Called when the page loads
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="e">
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        this.PageSize.DataSource = StaticDataHelper.PageEntries();
        this.PageSize.DataTextField = "Name";
        this.PageSize.DataValueField = "Value";
        this.PageSize.DataBind();

        try
        {
            this.PageSize.SelectedValue = this.PageBoardContext.PageUser.PageSize.ToString();
        }
        catch (Exception)
        {
            this.PageSize.SelectedValue = "5";
        }

        this.BindData();
    }

    /// <summary>
    /// The page size on selected index changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PageSizeSelectedIndexChanged(object sender, EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// The pager_ page change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// The ItemCommand method for the link buttons in the last column.
    /// </summary>
    /// <param name="sender">
    /// the sender.
    /// </param>
    /// <param name="e">
    /// the e.
    /// </param>
    protected void rptBuddy_ItemCommand([NotNull] object sender, [NotNull] CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "remove":
                this.PageBoardContext.LoadMessage.AddSession(
                    string.Format(
                        this.GetText("REMOVEBUDDY_NOTIFICATION"),
                        this.Get<IFriends>().Remove(e.CommandArgument.ToType<int>())),
                    MessageTypes.success);
                this.CurrentUserID = this.PageBoardContext.PageUserID;
                break;
            case "removeRequest":
                this.GetRepository<Buddy>().RemoveRequest(e.CommandArgument.ToType<int>());

                this.PageBoardContext.LoadMessage.AddSession(this.GetText("NOTIFICATION_REQUESTREMOVED"),
                    MessageTypes.success);
                this.CurrentUserID = this.PageBoardContext.PageUserID;
                break;
            case "approve":
                this.PageBoardContext.LoadMessage.AddSession(
                    string.Format(
                        this.GetText("NOTIFICATION_BUDDYAPPROVED"),
                        this.Get<IFriends>().ApproveRequest(e.CommandArgument.ToType<int>(), false)),
                    MessageTypes.success);
                break;
            case "approveadd":
                this.PageBoardContext.LoadMessage.AddSession(
                    string.Format(
                        this.GetText("NOTIFICATION_BUDDYAPPROVED_MUTUAL"),
                        this.Get<IFriends>().ApproveRequest(e.CommandArgument.ToType<int>(), true)),
                    MessageTypes.success);
                break;
            case "approveall":
                this.Get<IFriends>().ApproveAllRequests(false);
                this.PageBoardContext.LoadMessage.AddSession(this.GetText("NOTIFICATION_ALL_APPROVED"), MessageTypes.success);
                break;
            case "approveaddall":
                this.Get<IFriends>().ApproveAllRequests(true);
                this.PageBoardContext.LoadMessage.AddSession(this.GetText("NOTIFICATION_ALL_APPROVED_ADDED"), MessageTypes.success);
                break;
            case "deny":
                this.Get<IFriends>().DenyRequest(e.CommandArgument.ToType<int>());
                this.PageBoardContext.LoadMessage.AddSession(this.GetText("NOTIFICATION_BUDDYDENIED"), MessageTypes.info);
                break;
            case "denyall":
                this.Get<IFriends>().DenyAllRequests();
                this.PageBoardContext.LoadMessage.AddSession(this.GetText("NOTIFICATION_ALL_DENIED"), MessageTypes.info);
                break;
        }

        // Update all buddy list controls in Friends.ascx page.
        this.Get<LinkBuilder>().Redirect(ForumPages.Friends);
    }

    /// <summary>
    /// The rpt buddy_ item created.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void rptBuddy_ItemCreated([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
    {
        // In what mode should this control work?
        // 2: display the buddy list and ("Remove Buddy") buttons.
        // 3: display pending buddy list posted to current user and add ("approve","approve all", "deny",
        // "deny all","approve and add", "approve and add all") buttons.
        // 4: show the pending requests posted from the current user.
        switch (this.Mode)
        {
            case FriendMode.Friends:
                if (e.Item.ItemType is ListItemType.Item or ListItemType.AlternatingItem)
                {
                    e.Item.FindControlAs<PlaceHolder>("pnlRemove").Visible = true;
                }

                break;
            case FriendMode.ReceivedRequests:
                if (e.Item.ItemType is ListItemType.Item or ListItemType.AlternatingItem)
                {
                    e.Item.FindControlAs<PlaceHolder>("pnlPending").Visible = true;
                }

                if (e.Item.ItemType == ListItemType.Footer)
                {
                    if (this.rptBuddy.Items.Count > 0)
                    {
                        e.Item.FindControlAs<PlaceHolder>("Footer").Visible = true;
                    }
                }

                break;
            case FriendMode.SendRequests:
                if (e.Item.ItemType is ListItemType.Item or ListItemType.AlternatingItem)
                {
                    e.Item.FindControlAs<PlaceHolder>("pnlRequests").Visible = true;
                    e.Item.FindControlAs<ThemeButton>("RequestRemove").Visible = true;
                }

                break;
        }
    }

    /// <summary>
    /// Renders the Icon Header Text
    /// </summary>
    protected string GetHeaderText()
    {
        return this.Mode switch
            {
                FriendMode.Friends => this.GetText("FRIENDS", "BUDDYLIST"),
                FriendMode.ReceivedRequests => this.GetText("FRIENDS", "PENDING_REQUESTS"),
                FriendMode.SendRequests => this.GetText("FRIENDS", "YOUR_REQUESTS"),
                _ => this.GetText("FRIENDS", "BUDDYLIST")
            };
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.Pager.PageSize = this.PageSize.SelectedValue.ToType<int>();

        // set the Data table
        var buddyList = this.FriendsList;

        if (!buddyList.NullOrEmpty())
        {
            var buddyListView = buddyList;
            // In what mode should this control work?
            // Refer to "rptBuddy_ItemCreate" event for more info.
            switch (this.Mode)
            {
                case FriendMode.Friends:
                    buddyListView = buddyList.Where(x => x.Approved).DistinctBy(x => x.UserID).ToList();
                    break;
                case FriendMode.ReceivedRequests:
                    buddyListView = buddyList.Where(x => !x.Approved && x.FromUserID != this.CurrentUserID).ToList();
                    break;
                case FriendMode.SendRequests:
                    buddyListView = buddyList.Where(x => !x.Approved && x.FromUserID == this.CurrentUserID).ToList();
                    break;
            }

            this.Pager.Count = buddyListView.Count;

            var pds = new PagedDataSource
                          {
                              DataSource = buddyListView,
                              AllowPaging = true,
                              CurrentPageIndex = this.Pager.CurrentPageIndex,
                              PageSize = this.Pager.PageSize
                          };

            this.rptBuddy.DataSource = pds;
        }

        this.DataBind();

        this.Count = this.rptBuddy.Items.Count;

        switch (this.Mode)
        {
            case FriendMode.Friends:
                this.Info.Controls.Add(new Literal { Text = $"<i class=\"fas fa-info text-info pe-1\"></i>{this.GetText("INFO_NO")}" });
                break;
            case FriendMode.ReceivedRequests:
            case FriendMode.SendRequests:
                this.Info.Controls.Add(new Literal { Text = $"<i class=\"fas fa-check text-success pe-1\"></i>{this.GetText("INFO_PENDING")}" });
                break;
        }
    }
}