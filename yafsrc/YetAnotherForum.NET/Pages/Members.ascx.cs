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

using YAF.Types.Models;

/// <summary>
/// The Members List Page
/// </summary>
public partial class Members : ForumPage
{
    /// <summary>
    /// The user List.
    /// </summary>
    private List<PagedUser> userList;

    /// <summary>
    ///   Initializes a new instance of the <see cref = "Members" /> class.
    /// </summary>
    public Members()
        : base("MEMBERS", ForumPages.Members)
    {
    }

    /// <summary>
    /// Handles the Click event of the Search control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    public void Search_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        // re-bind data
        this.BindData();
    }

    /// <summary>
    /// Handles the Click event of the Reset control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    public void Reset_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        // re-direct to self.
        this.Get<LinkBuilder>().Redirect(ForumPages.Members);
    }

    /// <summary>
    /// Registers the needed Java Scripts
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlock("dropDownToggleJs", JavaScriptBlocks.DropDownToggleJs());

        base.OnPreRender(e);
    }

    /// <summary>
    /// Gets the avatar Url for the user
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="avatarString">The avatar string.</param>
    /// <param name="hasAvatarImage">if set to <c>true</c> [has avatar image].</param>
    /// <returns>Returns the File Url</returns>
    protected string GetAvatarUrlFileName(int userId, string avatarString, bool hasAvatarImage)
    {
        var avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(userId, avatarString, hasAvatarImage);

        return avatarUrl.IsNotSet() ? $"{BoardInfo.ForumClientFileRoot}resource.ashx?u={userId}" : avatarUrl;
    }

    /// <summary>
    /// Get all users from for this board.
    /// </summary>
    /// <param name="startLetter">
    /// The start Letter.
    /// </param>
    /// <param name="userName">
    /// The user Name.
    /// </param>
    /// <param name="totalCount">
    /// The total Count.
    /// </param>
    /// <returns>
    /// The Members List
    /// </returns>
    protected List<PagedUser> GetUserList(char startLetter, string userName, out int totalCount)
    {
        this.userList = this.Get<IAspNetUsersHelper>().ListMembersPaged(
            this.PageBoardContext.PageBoardID,
            this.Group.SelectedIndex <= 0 ? null : this.Group.SelectedValue.ToType<int?>(),
            this.Ranks.SelectedIndex <= 0 ? null : this.Ranks.SelectedValue.ToType<int?>(),
            startLetter,
            userName,
            this.Pager.CurrentPageIndex,
            this.Pager.PageSize,
            this.ViewState["SortNameField"].ToType<int?>(),
            this.ViewState["SortRankNameField"].ToType<int?>(),
            this.ViewState["SortJoinedField"].ToType<int?>(),
            this.ViewState["SortNumPostsField"].ToType<int?>(),
            this.ViewState["SortLastVisitField"].ToType<int?>(),
            this.NumPostsTB.Text.Trim().IsSet() ? this.NumPostsTB.Text.Trim().ToType<int?>() : null,
            this.NumPostDDL.SelectedIndex < 0 ? 3 : this.NumPostsTB.Text.Trim().IsSet() ? this.NumPostDDL.SelectedValue.ToType<int>() : 0);

        totalCount = this.userList.Any() ? this.userList.FirstOrDefault().TotalRows : 0;

        return this.userList;
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.UserSearchName.Focus();

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

        this.ViewState["SortNameField"] = 1;
        this.ViewState["SortRankNameField"] = 0;
        this.ViewState["SortJoinedField"] = 0;
        this.ViewState["SortNumPostsField"] = 0;
        this.ViewState["SortLastVisitField"] = 0;

        this.PageLinks.AddRoot().AddLink(this.GetText("TITLE"));

        this.NumPostDDL.Items.Add(new ListItem(this.GetText("MEMBERS", "NUMPOSTSEQUAL"), "1"));
        this.NumPostDDL.Items.Add(new ListItem(this.GetText("MEMBERS", "NUMPOSTSLESSOREQUAL"), "2"));
        this.NumPostDDL.Items.Add(new ListItem(this.GetText("MEMBERS", "NUMPOSTSMOREOREQUAL"), "3"));

        this.NumPostDDL.DataBind();

        // get list of user ranks for filtering
        var ranks = this.GetRepository<Rank>().GetByBoardId().OrderBy(r => r.SortOrder).ToList();

        ranks.Insert(0, new Rank { Name = this.GetText("ALL"), ID = 0 });

        ranks.RemoveAll(r => r.Name == "Guest");

        this.Ranks.DataSource = ranks;
        this.Ranks.DataTextField = "Name";
        this.Ranks.DataValueField = "ID";
        this.Ranks.DataBind();

        // get list of user ranks for filtering
        var groups = this.GetRepository<Group>().GetByBoardId().OrderBy(r => r.SortOrder).ToList();

        groups.Insert(0, new Group { Name = this.GetText("ALL"), ID = 0 });

        groups.RemoveAll(r => r.Name == "Guests");

        this.Group.DataSource = groups;
        this.Group.DataTextField = "Name";
        this.Group.DataValueField = "ID";
        this.Group.DataBind();

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
    protected void Pager_PageChange(object sender, EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// Sort by Joined ascending
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void JoinedAsc_Click(object sender, EventArgs e)
    {
        this.SetSort("Joined", 1);

        this.ViewState["SortNameField"] = 0;
        this.ViewState["SortRankNameField"] = 0;
        this.ViewState["SortNumPostsField"] = 0;
        this.ViewState["SortLastVisitField"] = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by Joined descending
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void JoinedDesc_Click(object sender, EventArgs e)
    {
        this.SetSort("Joined", 2);

        this.ViewState["SortNameField"] = 0;
        this.ViewState["SortRankNameField"] = 0;
        this.ViewState["SortNumPostsField"] = 0;
        this.ViewState["SortLastVisitField"] = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by Last Visit ascending
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void LastVisitAsc_Click(object sender, EventArgs e)
    {
        this.SetSort("LastVisit", 1);

        this.ViewState["SortNameField"] = 0;
        this.ViewState["SortRankNameField"] = 0;
        this.ViewState["SortJoinedField"] = 0;
        this.ViewState["SortNumPostsField"] = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by Last visit descending
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void LastVisitDesc_Click(object sender, EventArgs e)
    {
        this.SetSort("LastVisit", 2);

        this.ViewState["SortNameField"] = 0;
        this.ViewState["SortRankNameField"] = 0;
        this.ViewState["SortJoinedField"] = 0;
        this.ViewState["SortNumPostsField"] = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by Posts ascending
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PostsAsc_Click(object sender, EventArgs e)
    {
        this.SetSort("NumPosts", 1);

        this.ViewState["SortNameField"] = 0;
        this.ViewState["SortRankNameField"] = 0;
        this.ViewState["SortJoinedField"] = 0;
        this.ViewState["SortLastVisitField"] = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by Posts descending
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PostsDesc_Click(object sender, EventArgs e)
    {
        this.SetSort("NumPosts", 2);

        this.ViewState["SortNameField"] = 0;
        this.ViewState["SortRankNameField"] = 0;
        this.ViewState["SortJoinedField"] = 0;
        this.ViewState["SortLastVisitField"] = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by Rank ascending
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void RankAsc_Click(object sender, EventArgs e)
    {
        this.SetSort("RankName", 1);

        this.ViewState["SortNameField"] = 0;
        this.ViewState["SortJoinedField"] = 0;
        this.ViewState["SortNumPostsField"] = 0;
        this.ViewState["SortLastVisitField"] = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by rank descending
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void RankDesc_Click(object sender, EventArgs e)
    {
        this.SetSort("RankName", 2);

        this.ViewState["SortNameField"] = 0;
        this.ViewState["SortJoinedField"] = 0;
        this.ViewState["SortNumPostsField"] = 0;
        this.ViewState["SortLastVisitField"] = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by User name ascending
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void UserNameAsc_Click(object sender, EventArgs e)
    {
        this.SetSort("Name", 1);

        this.ViewState["SortRankNameField"] = 0;
        this.ViewState["SortJoinedField"] = 0;
        this.ViewState["SortNumPostsField"] = 0;
        this.ViewState["SortLastVisitField"] = 0;

        this.BindData();
    }

    /// <summary>
    /// Sort by user name descending
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void UserNameDesc_Click(object sender, EventArgs e)
    {
        this.SetSort("Name", 2);

        this.ViewState["SortRankNameField"] = 0;
        this.ViewState["SortJoinedField"] = 0;
        this.ViewState["SortNumPostsField"] = 0;
        this.ViewState["SortLastVisitField"] = 0;

        this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.Pager.PageSize = this.PageSize.SelectedValue.ToType<int>();

        this.NumPostDDL.SelectedValue = "3";

        if (this.NumPostsTB.Text.IsSet())
        {
            var numberOfPosts = this.NumPostsTB.Text.ToType<int>();

            if (numberOfPosts < 0)
            {
                this.PageBoardContext.Notify(this.GetText("MEMBERS", "INVALIDPOSTSVALUE"), MessageTypes.warning);
                return;
            }
        }

        // get the user list...
        this.userList = this.GetUserList(
            this.AlphaSort1.CurrentLetter,
            this.UserSearchName.Text.Trim(),
            out var totalCount);

        this.Pager.Count = totalCount;
        this.MemberList.DataSource = this.userList;
        this.DataBind();

        this.NoInfo.Visible = this.MemberList.Items.Count == 0;

        switch (this.ViewState["SortNameField"].ToType<int?>())
        {
            case 1:
                this.SortUserNameAsc.Icon = "check-square";

                this.SortUserNameDesc.Icon = "sort-alpha-down-alt";
                break;
            case 2:
                this.SortUserNameDesc.Icon = "check-square";

                this.SortUserNameAsc.Icon = "sort-alpha-down";
                break;
            default:
                this.SortUserNameAsc.Icon = "sort-alpha-down";

                this.SortUserNameDesc.Icon = "sort-alpha-down-alt";
                break;
        }

        switch (this.ViewState["SortRankNameField"].ToType<int?>())
        {
            case 1:
                this.SortRankAsc.Icon = "check-square";

                this.SortRankDesc.Icon = "sort-alpha-down-alt";
                break;
            case 2:
                this.SortRankDesc.Icon = "check-square";

                this.SortRankAsc.Icon = "sort-alpha-down";
                break;
            default:
                this.SortRankAsc.Icon = "sort-alpha-down";

                this.SortRankDesc.Icon = "sort-alpha-down-alt";
                break;
        }

        switch (this.ViewState["SortJoinedField"].ToType<int?>())
        {
            case 1:
                this.SortJoinedAsc.Icon = "check-square";

                this.SortJoinedDesc.Icon = "sort-alpha-down-alt";
                break;
            case 2:
                this.SortJoinedDesc.Icon = "check-square";

                this.SortJoinedAsc.Icon = "sort-alpha-down";
                break;
            default:
                this.SortJoinedAsc.Icon = "sort-alpha-down";

                this.SortJoinedDesc.Icon = "sort-alpha-down-alt";
                break;
        }

        switch (this.ViewState["SortNumPostsField"].ToType<int?>())
        {
            case 1:
                this.SortPostsAsc.Icon = "check-square";

                this.SortPostsDesc.Icon = "sort-alpha-down-alt";
                break;
            case 2:
                this.SortPostsDesc.Icon = "check-square";

                this.SortPostsAsc.Icon = "sort-alpha-down";
                break;
            default:
                this.SortPostsAsc.Icon = "sort-alpha-down";

                this.SortPostsDesc.Icon = "sort-alpha-down-alt";
                break;
        }

        switch (this.ViewState["SortLastVisitField"].ToType<int?>())
        {
            case 1:
                this.SortLastVisitAsc.Icon = "check-square";

                this.SortLastVisitDesc.Icon = "sort-alpha-down-alt";
                break;
            case 2:
                this.SortLastVisitDesc.Icon = "check-square";

                this.SortLastVisitAsc.Icon = "sort-alpha-down";
                break;
            default:
                this.SortLastVisitAsc.Icon = "sort-alpha-down";

                this.SortLastVisitDesc.Icon = "sort-alpha-down-alt";
                break;
        }
    }

    /// <summary>
    /// Helper function for setting up the current sort on
    /// the member list view
    /// </summary>
    /// <param name="field">
    /// The field.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    private void SetSort(string field, int value)
    {
        switch (field)
        {
            case "Name":
                this.ViewState["SortNameField"] = value;
                break;
            case "RankName":
                this.ViewState["SortRankNameField"] = value;
                break;
            case "Joined":
                this.ViewState["SortJoinedField"] = value;
                break;
            case "NumPosts":
                this.ViewState["SortNumPostsField"] = value;
                break;
            case "LastVisit":
                this.ViewState["SortLastVisitField"] = value;
                break;
        }
    }
}