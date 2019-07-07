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

namespace YAF.Dialogs
{
    #region Using

    using System;

    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Admin Medal Group Add/Edit Dialog.
    /// </summary>
    public partial class GroupMedalEdit : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Gets or sets the banned identifier.
        /// </summary>
        /// <value>
        /// The banned identifier.
        /// </value>
        public int? MedalId
        {
            get => this.ViewState[key: "MedalId"].ToType<int?>();

            set => this.ViewState[key: "MedalId"] = value;
        }

        /// <summary>
        /// Gets or sets the group id.
        /// </summary>
        public int? GroupId
        {
            get => this.ViewState[key: "GroupId"].ToType<int?>();

            set => this.ViewState[key: "GroupId"] = value;
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="groupId">
        /// The group Id.
        /// </param>
        /// <param name="medalId">
        /// The medal identifier.
        /// </param>
        public void BindData(int? groupId, int? medalId)
        {
            this.MedalId = medalId;
            this.GroupId = groupId;

            // clear controls
            this.AvailableGroupList.SelectedIndex = -1;
            this.GroupMessage.Text = null;
            this.GroupOnlyRibbon.Checked = false;
            this.GroupHide.Checked = false;
            this.GroupSortOrder.Text = "1";

            // set controls visibility and availability
            this.AvailableGroupList.Enabled = true;

            var groups = this.GetRepository<Group>().List(boardId: this.PageContext.PageBoardID);

            this.AvailableGroupList.DataSource = groups;

            this.AvailableGroupList.DataTextField = "Name";
            this.AvailableGroupList.DataValueField = "ID";

            this.AvailableGroupList.DataBind();

            // focus on save button
            this.AddGroupSave.Focus();

            if (this.MedalId.HasValue)
            {
                // Edit
                // load group-medal to the controls
                var row = this.GetRepository<Medal>().GroupMedalListAsDataTable(groupID: this.GroupId, medalID: this.MedalId)
                    .GetFirstRow();

                // tweak it for editing
                this.GroupMedalEditTitle.Text = this.GetText(page: "ADMIN_EDITMEDAL", tag: "EDIT_MEDAL_GROUP");
                this.AvailableGroupList.Enabled = false;

                // load data to controls
                this.AvailableGroupList.SelectedIndex = -1;
                this.AvailableGroupList.Items.FindByValue(value: row[columnName: "GroupID"].ToString()).Selected = true;
                this.GroupMessage.Text = row[columnName: "Message"].ToString();
                this.GroupSortOrder.Text = row[columnName: "SortOrder"].ToString();
                this.GroupOnlyRibbon.Checked = row[columnName: "OnlyRibbon"].ToType<bool>();
                this.GroupHide.Checked = row[columnName: "Hide"].ToType<bool>();

                // remove all user medals...
                this.Get<IDataCache>().Remove(
                    whereFunc: k => k.StartsWith(value: string.Format(format: Constants.Cache.UserMedals, arg0: string.Empty)));
            }
            else
            {
                // Add
                // set title
                this.GroupMedalEditTitle.Text = this.GetText(page: "ADMIN_EDITMEDAL", tag: "ADD_TOGROUP");
            }
        }

        /// <summary>
        /// Handles the Click event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Save_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // test if there is specified user name/id
            if (this.AvailableGroupList.SelectedIndex < 0)
            {
                // no group selected
                this.PageContext.AddLoadMessage(message: "Please select user group!", messageType: MessageTypes.warning);
                return;
            }

            // save group, if there is no message specified, pass null
            this.GetRepository<Medal>().GroupMedalSave(
                groupID: this.AvailableGroupList.SelectedValue,
                medalID: this.MedalId,
                message: this.GroupMessage.Text.IsNotSet() ? null : this.GroupMessage.Text,
                hide: this.GroupHide.Checked,
                onlyRibbon: this.GroupOnlyRibbon.Checked,
                sortOrder: this.GroupSortOrder.Text);

            // re-bind data
            YafBuildLink.Redirect(page: ForumPages.admin_editmedal, format: "medalid={0}", this.MedalId.Value);
        }

        #endregion
    }
}