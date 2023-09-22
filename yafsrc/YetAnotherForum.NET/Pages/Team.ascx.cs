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

using YAF.Types.Objects;
using YAF.Web.Controls;
using YAF.Types.Models;

/// <summary>
/// The Team Page
/// </summary>
public partial class Team : ForumPage
{
    /// <summary>
    ///   The Moderators List
    /// </summary>
    private List<SimpleModerator> completeModsList = new ();

    /// <summary>
    ///   Initializes a new instance of the <see cref = "Team" /> class.
    /// </summary>
    public Team()
        : base("TEAM", ForumPages.Team)
    {
    }

    /// <summary>
    /// Get all Admins.
    /// </summary>
    /// <returns>
    /// Moderators List
    /// </returns>
    [NotNull]
    protected List<User> GetAdmins()
    {
        // get a row with user lazy data...
        var adminList = this.Get<IDataCache>().GetOrSet(
            Constants.Cache.BoardAdmins,
            () => this.GetRepository<User>().ListAdmins(),
            TimeSpan.FromMinutes(this.PageBoardContext.BoardSettings.BoardModeratorsCacheTimeout));

        return adminList;
    }

    /// <summary>
    /// Get all Moderators Without Groups.
    /// </summary>
    /// <returns>
    /// Moderators List
    /// </returns>
    [NotNull]
    protected List<SimpleModerator> GetModerators()
    {
        var moderators = this.Get<DataBroker>().GetModerators();

        var modsSorted = new List<SimpleModerator>();

        moderators.Where(m => !m.IsGroup).ForEach(
            mod =>
                {
                    var sortedMod = mod;

                    // Check if Mod is already in modsSorted
                    if (modsSorted.Find(
                            s => s.Name.Equals(sortedMod.Name) && s.ModeratorID.Equals(sortedMod.ModeratorID)) != null)
                    {
                        return;
                    }

                    // Get All Items from that MOD
                    var modList = moderators.Where(m => m.Name.Equals(sortedMod.Name)).ToList();
                    var forumsCount = modList.Count;

                    sortedMod.ForumIDs = new ModeratorsForums[forumsCount];

                    for (var i = 0; i < forumsCount; i++)
                    {
                        var forumsId = new ModeratorsForums
                                           {
                                               CategoryID = modList[i].CategoryID,
                                               CategoryName = modList[i].CategoryName,
                                               ParentID = modList[i].ParentID,
                                               ForumID = modList[i].ForumID, 
                                               ForumName = modList[i].ForumName
                                           };

                        sortedMod.ForumIDs[i] = forumsId;
                    }

                    modsSorted.Add(sortedMod);
                });

        return modsSorted;
    }

    /// <summary>
    /// Called when the page loads
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e(EventArgs).
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.ShowTeamTo))
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        if (this.IsPostBack)
        {
            return;
        }

        this.BindData();
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TEAM", "TITLE"), string.Empty);
    }

    /// <summary>
    /// The admins list_ on item data bound.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void AdminsList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
        {
            return;
        }

        var adminAvatar = e.Item.FindControlAs<Image>("AdminAvatar");

        var user = (User)e.Item.DataItem;
        var displayName = user.DisplayOrUserName();

        adminAvatar.ImageUrl = this.Get<IAvatars>().GetAvatarUrlForUser(user);

        adminAvatar.AlternateText = displayName;
        adminAvatar.ToolTip = displayName;

        // User Buttons
        var adminUserButton = e.Item.FindControlAs<ThemeButton>("AdminUserButton");
        var pm = e.Item.FindControlAs<ThemeButton>("PM");
        var email = e.Item.FindControlAs<ThemeButton>("Email");

        adminUserButton.Visible = this.PageBoardContext.IsAdmin;

        if (user.ID == this.PageBoardContext.PageUserID)
        {
            return;
        }

        var isFriend = this.Get<IFriends>().IsBuddy(user.ID, true);

        pm.Visible = !this.PageBoardContext.IsGuest && this.User != null && this.PageBoardContext.BoardSettings.AllowPrivateMessages;

        if (pm.Visible)
        {
            if (user.Block.BlockPMs)
            {
                pm.Visible = false;
            }

            if (this.PageBoardContext.IsAdmin || isFriend)
            {
                pm.Visible = true;
            }
        }

        pm.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.PostPrivateMessage, new { u = user.ID });
        pm.ParamTitle0 = displayName;

        // email link
        email.Visible = !this.PageBoardContext.IsGuest && this.User != null && this.PageBoardContext.BoardSettings.AllowEmailSending;

        if (!email.Visible)
        {
            return;
        }

        if (user.Block.BlockEmails)
        {
            email.Visible = false;
        }

        if (this.PageBoardContext.IsAdmin && isFriend)
        {
            email.Visible = true;
        }

        email.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.Email, new { u = user.ID });
        email.ParamTitle0 = displayName;
    }

    /// <summary>
    /// The moderators list_ on item data bound.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ModeratorsList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
        {
            return;
        }

        var forumsJump  = e.Item.FindControlAs<DropDownList>("Forums");

        var modLink = e.Item.FindControlAs<UserLink>("ModLink");

        var mod = this.completeModsList.Find(m => m.ModeratorID.Equals(modLink.UserID));

        var forums = mod.ForumIDs.Select(forumsItem => forumsItem).ToList();

        var forumList = this.GetRepository<Types.Models.Forum>().SortModeratorList(forums);

        if (forums.Any())
        {
            forumsJump.AddForumAndCategoryIcons(forumList, this.GetTextFormatted("VIEW_FORUMS", forums.Count()));

            forumsJump.Attributes["PlaceHolder"] = this.GetTextFormatted("VIEW_FORUMS", forums.Count());
        }
        else
        {
            forumsJump.Visible = false;
        }

        // User Buttons
        var adminUserButton = e.Item.FindControlAs<ThemeButton>("AdminUserButton");
        var pm = e.Item.FindControlAs<ThemeButton>("PM");
        var email = e.Item.FindControlAs<ThemeButton>("Email");

        adminUserButton.Visible = this.PageBoardContext.IsAdmin;

        var itemDataItem = (SimpleModerator)e.Item.DataItem;
        var userid = itemDataItem.ModeratorID.ToType<int>();
        var displayName = this.PageBoardContext.BoardSettings.EnableDisplayName ? itemDataItem.DisplayName : itemDataItem.Name;

        var modAvatar = e.Item.FindControlAs<Image>("ModAvatar");

        modAvatar.ImageUrl = this.Get<IAvatars>().GetAvatarUrlForUser(
            userid,
            itemDataItem.Avatar,
            itemDataItem.AvatarImage != null);

        modAvatar.AlternateText = displayName;
        modAvatar.ToolTip = displayName;

        if (userid == this.PageBoardContext.PageUserID)
        {
            return;
        }

        var isFriend = this.Get<IFriends>().IsBuddy(userid, true);

        pm.Visible = !this.PageBoardContext.IsGuest && this.User != null && this.PageBoardContext.BoardSettings.AllowPrivateMessages;

        if (pm.Visible)
        {
            if (mod.UserBlockFlags.BlockPMs)
            {
                pm.Visible = false;
            }

            if (this.PageBoardContext.IsAdmin || isFriend)
            {
                pm.Visible = true;
            }
        }

        pm.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.PostPrivateMessage, new { u = userid });
        pm.ParamTitle0 = displayName;

        // email link
        email.Visible = !this.PageBoardContext.IsGuest && this.User != null && this.PageBoardContext.BoardSettings.AllowEmailSending;

        if (!email.Visible)
        {
            return;
        }

        if (mod.UserBlockFlags.BlockEmails && !this.PageBoardContext.IsAdmin)
        {
            email.Visible = false;
        }

        if (this.PageBoardContext.IsAdmin || isFriend)
        {
            email.Visible = true;
        }

        email.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.Email, new { u = userid });
        email.ParamTitle0 = displayName;
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.AdminsList.DataSource = this.GetAdmins();

        this.completeModsList = this.GetModerators();

        if (this.completeModsList.Count > 0)
        {
            this.ModsTable.Visible = true;

            this.ModeratorsList.DataSource = this.completeModsList;
        }
        else
        {
            this.ModsTable.Visible = false;
        }

        this.DataBind();
    }
}