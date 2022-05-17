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

namespace YAF.Pages.Moderate;

using YAF.Types.Models;

/// <summary>
/// Base root control for moderating, linking to other moderating controls/pages.
/// </summary>
public partial class Index : ModerateForumPage
{
    /// <summary>
    /// The forums.
    /// </summary>
    private List<ModerateForum> forumsList;

    /// <summary>
    ///   Initializes a new instance of the <see cref = "Index" /> class. 
    ///   Default constructor.
    /// </summary>
    public Index()
        : base("MODERATE_DEFAULT", ForumPages.Moderate_Index)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    protected override void CreatePageLinks()
    {
        // forum index
        this.PageLinks.AddRoot();

        // moderation index
        this.PageLinks.AddLink(this.GetText("TITLE"));
    }

    /// <summary>
    /// The category list_ on item data bound.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void CategoryList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
        {
            return;
        }

        var category = (Category)e.Item.DataItem;

        var forums = this.forumsList.Where(f => f.CategoryID == category.ID);

        var forumRepeater = e.Item.FindControlAs<Repeater>("ForumList");

        if (!forums.Any())
        {
            e.Item.Visible = false;
        }
        else
        {
            forumRepeater.DataSource = forums;
            forumRepeater.DataBind();
        }
    }

    /// <summary>
    /// Handles event of item commands for each forum.
    /// </summary>
    /// <param name="source">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
    protected void ForumListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
    {
        // which command are we handling
        switch (e.CommandName.ToLower())
        {
            case "viewunapprovedposts":

                // go to unapproved posts for selected forum
                this.Get<LinkBuilder>().Redirect(ForumPages.Moderate_UnapprovedPosts, new { f = e.CommandArgument });
                break;
            case "viewreportedposts":

                // go to spam reports for selected forum
                this.Get<LinkBuilder>().Redirect(ForumPages.Moderate_ReportedPosts, new { f = e.CommandArgument });
                break;
        }
    }

    /// <summary>
    /// Handles page load event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        // this needs to be done just once, not during post-backs
        if (this.IsPostBack)
        {
            return;
        }

        // bind data
        this.BindData();
    }

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private void BindData()
    {
        this.forumsList = this.GetRepository<Forum>()
            .ModerateList(this.PageBoardContext.PageUserID, this.PageBoardContext.PageBoardID);

        this.CategoryList.DataSource = this.GetRepository<Category>().GetByBoardId().OrderBy(c => c.SortOrder);

        // bind data to controls
        this.DataBind();

        if (this.CategoryList.Items.Count.Equals(0))
        {
            this.InfoPlaceHolder.Visible = true;
        }
        else
        {
            this.InfoPlaceHolder.Visible = !this.CategoryList.Items.Cast<RepeaterItem>().Any(item => item.Visible);
        }
    }
}