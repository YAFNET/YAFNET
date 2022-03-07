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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Types.Objects.Model;
    using YAF.Web.Controls;

    using Forum = YAF.Types.Models.Forum;

    #endregion

    /// <summary>
    /// The category list.
    /// </summary>
    public partial class CategoryList : BaseUserControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        private Tuple<List<SimpleModerator>, List<ForumRead>> Data { get; set; }

        #endregion

        #region Methods

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

            var forums = this.GetRepository<Forum>().ListRead(
                this.PageContext.PageBoardID,
                this.PageContext.PageUserID,
                categoryId,
                null,
                false);

            var watchForums = this.GetRepository<WatchForum>().List(this.PageContext.PageUserID);

            forums.ForEach(
                forum =>
                {
                    if (!watchForums.Any(
                        w => w.Item1.ForumID == forum.ForumID && w.Item1.UserID == this.PageContext.PageUserID))
                    {
                        this.GetRepository<WatchForum>().Add(this.PageContext.PageUserID, forum.ForumID);
                    }
                });

            this.PageContext.AddLoadMessage(this.GetText("SAVED_NOTIFICATION_SETTING"), MessageTypes.success);

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
            var markAll = (ThemeButton)sender;

            var categoryId = markAll.CommandArgument.ToType<int?>();

            var forums = this.GetRepository<Forum>().ListRead(
                this.PageContext.PageBoardID,
                this.PageContext.PageUserID,
                categoryId,
                null,
                false);

            this.Get<IReadTrackCurrentUser>().SetForumRead(forums.Select(f => f.ForumID));

            this.PageContext.AddLoadMessage(this.GetText("MARKALL_MESSAGE"), MessageTypes.success);

            this.BindData();
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
        private void BindData()
        {
            this.Data = this.Get<DataBroker>().BoardLayout(
                this.PageContext.PageBoardID,
                this.PageContext.PageUserID,
                this.PageContext.PageCategoryID,
                null);

            // Filter Categories
            var categories = this.Data.Item2.DistinctBy(x => x.CategoryID).ToList();

            this.Categories.DataSource = categories;
            this.Categories.DataBind();
        }

        #endregion
    }
}