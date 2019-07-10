/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * http://www.yetanotherforum.net/
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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Core;
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
                                    $"{YafForumInfo.ForumClientFileRoot}{YafBoardFolders.Current.Categories}/{row["CategoryImage"]}"
                            };

            return hasCategoryImage
                       ? $"{image.RenderToString()}&nbsp;"
                       : @"<i class=""fas fa-folder fa-fw text-warning"" aria-hidden=""true""></i>&nbsp;";
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
                boardID: this.PageContext.PageBoardID,
                userID: this.PageContext.PageUserID,
                categoryID: categoryId,
                parentID: null,
                useStyledNicks: false,
                findLastRead: false);

            dt.AsEnumerable().Select(r => r["ForumID"].ToType<int>()).ForEach(
                forumId => this.GetRepository<WatchForum>().Add(this.PageContext.PageUserID, forumId));

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
                boardID: this.PageContext.PageBoardID,
                userID: this.PageContext.PageUserID,
                categoryID: categoryId,
                parentID: null,
                useStyledNicks: false,
                findLastRead: false);

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
            var ds = this.Get<YafDbBroker>().BoardLayout(
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