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

namespace YAF.Controls;

using YAF.Types.Objects;
using YAF.Web.Controls;
using YAF.Types.Models;

/// <summary>
/// The category list.
/// </summary>
public partial class CategoryList : BaseUserControl
{
    /// <summary>
    /// Gets or sets the page index.
    /// </summary>
    public int PageIndex
    {
        get => this.ViewState["PageIndex"]?.ToType<int>() ?? 0;

        set => this.ViewState["PageIndex"] = value;
    }

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    private Tuple<List<SimpleModerator>, List<ForumRead>> Data
    {
        get => this.ViewState["Data"]?.ToType<Tuple<List<SimpleModerator>, List<ForumRead>>>();

        set => this.ViewState["Data"] = value;
    }

    /// <summary>
    /// Gets the Category Image
    /// </summary>
    /// <param name="forum">
    /// The forum.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string GetCategoryImage([NotNull] ForumRead forum)
    {
        var hasCategoryImage = forum.CategoryImage.IsSet();

        var image = new Image
                        {
                            ImageUrl =
                                $"{BoardInfo.ForumClientFileRoot}{this.Get<BoardFolders>().Categories}/{forum.CategoryImage}",
                            AlternateText = forum.Category
                        };

        var icon = new Icon { IconName = "folder", IconType = "text-warning" };

        return hasCategoryImage
                   ? $"{image.RenderToString()}&nbsp;"
                   : $"{icon.RenderToString()}&nbsp;";
    }

    /// <summary>
    /// The mark all_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void WatchAllClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        var markAll = (ThemeButton)sender;

        var categoryId = markAll.CommandArgument.ToType<int?>();

        var forums = this.Data.Item2.Where(x => x.CategoryID == categoryId);

        var watchForums = this.GetRepository<WatchForum>().List(this.PageBoardContext.PageUserID);

        forums.ForEach(
            forum =>
                {
                    if (!watchForums.Any(
                            w => w.ForumID == forum.ForumID && w.UserID == this.PageBoardContext.PageUserID))
                    {
                        this.GetRepository<WatchForum>().Add(this.PageBoardContext.PageUserID, forum.ForumID);
                    }
                });

        this.PageBoardContext.Notify(this.GetText("SAVED_NOTIFICATION_SETTING"), MessageTypes.success);

        this.BindData();
    }

    /// <summary>
    /// The mark all_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void MarkAllClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        var forums = this.Data.Item2;

        this.Get<IReadTrackCurrentUser>().SetForumRead(forums.Select(f => f.ForumID));

        this.PageBoardContext.Notify(this.GetText("MARKALL_MESSAGE"), MessageTypes.success);

        this.BindData();
    }

    /// <summary>
    /// Load more categories and forums.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ShowMoreClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.PageIndex++;

        this.BindData(true);
    }

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.LoadMoreOnScrolling),
            JavaScriptBlocks.LoadMoreOnScrolling(this.ShowMore.UniqueID, this.ShowMore.ClientID));

        base.OnPreRender(e);
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// Gets the Forums.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// Returns the Forums
    /// </returns>
    protected Tuple<List<SimpleModerator>, List<ForumRead>> GetForums([NotNull] ForumRead item)
    {
        var forums = this.Data;

        return new Tuple<List<SimpleModerator>, List<ForumRead>>(
            forums.Item1,
            forums.Item2.Where(forum => forum.CategoryID == item.CategoryID).ToList());
    }

    /// <summary>
    /// Bind Data
    /// </summary>
    /// <param name="appendData">
    /// Append data or re-load data
    /// </param>
    private void BindData(bool appendData = false)
    {
        if (appendData)
        {
            var newData = this.Get<DataBroker>().BoardLayout(
                this.PageBoardContext.PageBoardID,
                this.PageBoardContext.PageUserID,
                this.PageIndex,
                20,
                this.PageBoardContext.PageCategoryID,
                null);

            this.Data.Item1.AddRange(newData.Item1);

            this.Data.Item2.AddRange(newData.Item2);
        }
        else
        {
            if (!this.IsPostBack)
            {
                this.Data = this.Get<DataBroker>().BoardLayout(
                    this.PageBoardContext.PageBoardID,
                    this.PageBoardContext.PageUserID,
                    this.PageIndex,
                    20,
                    this.PageBoardContext.PageCategoryID,
                    null);
            }
        }

        if (this.Data.Item2.Any())
        {
            if (this.Data.Item2.First().Total > this.Data.Item2.Count)
            {
                this.ShowMore.Visible = true;
            }

            if (this.Data.Item2.First().Total > 20)
            {
                this.ForumsShown.Visible = true;
                this.ForumsShownLabel.Text = this.GetTextFormatted("FORUMS_SHOWN", this.Data.Item2.Count, this.Data.Item2.First().Total);
            }
        }

        // Filter Categories
        var categories = this.Data.Item2.DistinctBy(x => x.CategoryID).ToList();

        this.Categories.DataSource = categories;
        this.Categories.DataBind();
    }
}