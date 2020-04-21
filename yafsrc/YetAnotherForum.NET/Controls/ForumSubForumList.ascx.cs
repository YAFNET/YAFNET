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
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The forum sub forum list.
    /// </summary>
    public partial class ForumSubForumList : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Sets DataSource.
        /// </summary>
        public IEnumerable DataSource
        {
            set => this.SubforumList.DataSource = value;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Provides the "Forum Link Text" for the ForumList control.
        ///   Automatically disables the link if the current user doesn't
        ///   have proper permissions.
        /// </summary>
        /// <param name="row">
        /// Current data row
        /// </param>
        /// <returns>
        /// Forum link text
        /// </returns>
        public string GetForumLink([NotNull] DataRow row)
        {
            var forumID = row["ForumID"].ToType<int>();

            // get the Forum Description
            var output = Convert.ToString(row["Forum"]);

            output = int.Parse(row["ReadAccess"].ToString()) > 0 ? $"<a class=\"card-link small\" href=\"{BuildLink.GetLink(ForumPages.Topics, "f={0}&name={1}", forumID, output)}\" title=\"{this.GetText("COMMON", "VIEW_FORUM")}\" >{output}</a>" : $"{output} {this.GetText("NO_FORUM_ACCESS")}";

            return output;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the ItemCreated event of the SubForumList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void SubForumList_ItemCreated([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    {
                        var row = e.Item.DataItem.ToType<DataRow>();
                        var lastRead = this.Get<IReadTrackCurrentUser>()
                            .GetForumRead(
                                row["ForumID"].ToType<int>(),
                                row["LastForumAccess"].ToType<DateTime?>() ?? DateTimeHelper.SqlDbMinTime());

                        var lastPosted = row["LastPosted"].ToType<DateTime?>() ?? lastRead;

                        var forumIcon = e.Item.FindControlAs<PlaceHolder>("ForumIcon");

                        var icon = new Literal { Text = "<i class=\"fa fa-comments text-secondary\"></i>" };

                        try
                        {
                            if (lastPosted > lastRead)
                            {
                                icon.Text = "<i class=\"fa fa-comments text-success\"></i>";
                            }
                        }
                        catch
                        {
                            icon = new Literal { Text = "<i class=\"fa fa-comments text-secondary\"></i>" };
                        }

                        forumIcon.Controls.Add(icon);
                    }

                    break;
                case ListItemType.EditItem:
                    break;
                case ListItemType.Footer:
                    {
                        var repeater = sender as Repeater;
                        var dataSource = repeater.DataSource.ToType<IEnumerable>();

                        if (dataSource != null
                            && dataSource.ToType<ArrayList>().Count >= this.Get<BoardSettings>().SubForumsInForumList)
                        {
                            e.Item.FindControl("CutOff").Visible = true;
                        }
                    }

                    break;
                default:
                    return;
            }
        }

        #endregion
    }
}