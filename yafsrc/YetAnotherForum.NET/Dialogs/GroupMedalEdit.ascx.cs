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

namespace YAF.Dialogs
{
    #region Using

    using System;
    using System.Linq;

    using YAF.Core.BaseControls;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

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
            get => this.ViewState["MedalId"].ToType<int?>();

            set => this.ViewState["MedalId"] = value;
        }

        /// <summary>
        /// Gets or sets the group id.
        /// </summary>
        public int? GroupId
        {
            get => this.ViewState["GroupId"].ToType<int?>();

            set => this.ViewState["GroupId"] = value;
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

            if (this.GroupId.HasValue)
            {
                // Edit
                // load group-medal to the controls
                var row = this.GetRepository<GroupMedal>().List(this.GroupId.Value, this.MedalId.Value).FirstOrDefault();

                // tweak it for editing
                this.GroupMedalEditTitle.Text = this.GetText("ADMIN_EDITMEDAL", "EDIT_MEDAL_GROUP");
                this.AvailableGroupList.Enabled = false;

                // load data to controls
                this.AvailableGroupList.SelectedIndex = -1;
                this.AvailableGroupList.Items.FindByValue(row.Item2.GroupID.ToString()).Selected = true;

                this.GroupMessage.Text = row.Item2.Message.IsSet() ? row.Item2.Message : row.Item1.Message;
                this.GroupSortOrder.Text = row.Item2.SortOrder.ToString();
                this.GroupHide.Checked = row.Item2.Hide;

                // remove all user medals...
                this.Get<IDataCache>().Remove(
                    k => k.StartsWith(string.Format(Constants.Cache.UserMedals, string.Empty)));
            }
            else
            {
                // Add
                // set title
                this.GroupMedalEditTitle.Text = this.GetText("ADMIN_EDITMEDAL", "ADD_TOGROUP");
            }
        }

        /// <summary>
        /// Handles the Click event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Save_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.GroupId.HasValue)
            {
                // save group, if there is no message specified, pass null
                this.GetRepository<GroupMedal>().Save(
                    this.GroupId.Value,
                    this.MedalId.Value,
                    this.GroupMessage.Text.IsNotSet() ? null : this.GroupMessage.Text,
                    this.GroupHide.Checked,
                    this.GroupSortOrder.Text.ToType<byte>());
            }
            else
            {
                this.GetRepository<GroupMedal>().SaveNew(
                    this.AvailableGroupList.SelectedValue.ToType<int>(),
                    this.MedalId.Value,
                    this.GroupMessage.Text.IsNotSet() ? null : this.GroupMessage.Text,
                    this.GroupHide.Checked,
                    this.GroupSortOrder.Text.ToType<byte>());
            }

            // re-bind data
            this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditMedal, "medalid={0}", this.MedalId.Value);
        }

        #endregion
    }
}