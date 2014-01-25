/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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

using YAF.Utils.Helpers;

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

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
            set
            {
                this.SubforumList.DataSource = value;
            }
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
            int forumID = row["ForumID"].ToType<int>();

            // get the Forum Description
            string output = Convert.ToString(row["Forum"]);

            if (int.Parse(row["ReadAccess"].ToString()) > 0)
            {
                output =
                    "<a href=\"{0}\" title=\"{1}\" >{2}</a>".FormatWith(
                        YafBuildLink.GetLink(ForumPages.topics, "f={0}", forumID),
                        this.GetText("COMMON", "VIEW_FORUM"),
                        output);
            }
            else
            {
                // no access to this forum
                output = "{0} {1}".FormatWith(output, this.GetText("NO_FORUM_ACCESS"));
            }

            return output;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The subforum list_ item created.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void SubforumList_ItemCreated([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var row = (DataRow)e.Item.DataItem;

            DateTime lastRead = this.Get<IReadTrackCurrentUser>().GetForumRead(
                row["ForumID"].ToType<int>(), row["LastForumAccess"].ToType<DateTime?>() ?? DateTimeHelper.SqlDbMinTime());

            DateTime lastPosted = row["LastPosted"].ToType<DateTime?>() ?? lastRead;

            var subForumIcon = e.Item.FindControl("ThemeSubforumIcon") as ThemeImage;

            if (subForumIcon == null)
            {
                return;
            }

            try
            {
                if (lastPosted > lastRead)
                {
                    subForumIcon.ThemeTag = "SUBFORUM_NEW";
                    subForumIcon.LocalizedTitlePage = "ICONLEGEND";
                    subForumIcon.LocalizedTitleTag = "NEW_POSTS";
                }
                else
                {
                    subForumIcon.ThemeTag = "SUBFORUM";
                    subForumIcon.LocalizedTitlePage = "ICONLEGEND";
                    subForumIcon.LocalizedTitleTag = "NO_NEW_POSTS";
                }
            }
            catch
            {
                subForumIcon.ThemeTag = "SUBFORUM";
                subForumIcon.LocalizedTitlePage = "ICONLEGEND";
                subForumIcon.LocalizedTitleTag = "NO_NEW_POSTS";
            }
        }

        #endregion
    }
}