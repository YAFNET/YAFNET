/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;

    using Forum = YAF.Types.Models.Forum;

    #endregion

    /// <summary>
    /// The forum category list.
    /// </summary>
    public partial class ForumCategoryList : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Gets the Category Image
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetCategoryImage([NotNull] DataRowView row)
        {
            var hasCategoryImage = row["CategoryImage"].ToString().IsSet();

            var image = new Image
                            {
                                ImageUrl =
                                    $"{BoardInfo.ForumClientFileRoot}{BoardFolders.Current.Categories}/{row["CategoryImage"]}",
                                AlternateText = row["Name"].ToString()
                            };

            var icon = new Icon { IconName = "folder", IconType = "text-warning", IconSize = "fa-2x" };

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

            int? categoryId = null;

            if (int.TryParse(markAll.CommandArgument, out var resultId))
            {
                categoryId = resultId;
            }

            var dt = this.GetRepository<Forum>().ListReadAsDataTable(
                this.PageContext.PageBoardID,
                this.PageContext.PageUserID,
                categoryId,
                null,
                false,
                false);

            var watchForums = this.GetRepository<WatchForum>().List(this.PageContext.PageUserID);

            dt.AsEnumerable().Select(r => r.Field<int>("ForumID")).ForEach(
                forumId =>
                    {
                        if (!watchForums.Any(
                                w => w.Item1.ForumID == forumId && w.Item1.UserID == this.PageContext.PageUserID))
                        {
                            this.GetRepository<WatchForum>().Add(this.PageContext.PageUserID, forumId);
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

            int? categoryId = null;

            if (int.TryParse(markAll.CommandArgument, out var resultId))
            {
                categoryId = resultId;
            }

            var dt = this.GetRepository<Forum>().ListReadAsDataTable(
                this.PageContext.PageBoardID,
                this.PageContext.PageUserID,
                categoryId,
                null,
                false,
                false);

            this.Get<IReadTrackCurrentUser>().SetForumRead(dt.AsEnumerable().Select(r => r["ForumID"].ToType<int>()));

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
        /// Bind Data
        /// </summary>
        private void BindData()
        {
            var ds = this.Get<DataBroker>().BoardLayout(
                this.PageContext.PageBoardID,
                this.PageContext.PageUserID,
                this.PageContext.PageCategoryID,
                null);

            this.CategoryList.DataSource = ds.Tables["Category"];
            this.CategoryList.DataBind();
        }

        #endregion
    }
}