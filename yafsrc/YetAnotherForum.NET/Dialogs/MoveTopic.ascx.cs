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

namespace YAF.Dialogs
{
    #region Using

    using System;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Move Topic Dialog.
    /// </summary>
    public partial class MoveTopic : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageContext.ForumModeratorAccess)
            {
                this.Visible = false;
                return;
            }

            if (this.IsPostBack)
            {
                return;
            }

            var showMoved = this.Get<BoardSettings>().ShowMoved;

            // Ederon : 7/14/2007 - by default, leave pointer is set on value defined on host level
            this.LeavePointer.Checked = showMoved;

            this.trLeaveLink.Visible = showMoved;
            this.trLeaveLinkDays.Visible = showMoved;

            if (showMoved)
            {
                this.LinkDays.Text = "1";
            }

            var forumList = this.GetRepository<Forum>().ListAllSortedAsDataTable(
                this.PageContext.PageBoardID,
                this.PageContext.PageUserID);

            this.ForumList.AddForumAndCategoryIcons(forumList);

            this.DataBind();

            var pageItem = this.ForumList.Items.FindByValue(this.PageContext.PageForumID.ToString());

            if (pageItem != null)
            {
                pageItem.Selected = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the Move control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Move_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            int? linkDays = null;
            var ld = -2;

            if (this.LeavePointer.Checked && this.LinkDays.Text.IsSet() && !int.TryParse(this.LinkDays.Text, out ld))
            {
                this.PageContext.AddLoadMessage(this.GetText("POINTER_DAYS_INVALID"), MessageTypes.warning);
                return;
            }

            if (this.ForumList.SelectedValue.ToType<int>() <= 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("CANNOT_MOVE_TO_CATEGORY"), MessageTypes.warning);
                return;
            }

            // only move if it's a destination is a different forum.
            if (this.ForumList.SelectedValue.ToType<int>() != this.PageContext.PageForumID)
            {
                if (ld >= -2)
                {
                    linkDays = ld;
                }

                // Ederon : 7/14/2007
                this.GetRepository<Topic>().MoveTopic(
                    this.PageContext.PageTopicID,
                    this.ForumList.SelectedValue.ToType<int>(),
                    this.LeavePointer.Checked,
                    linkDays.Value);
            }

            BuildLink.Redirect(ForumPages.Topics, "f={0}", this.PageContext.PageForumID);
        }

        #endregion
    }
}