/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Admin Topic Status Add/Edit Dialog.
    /// </summary>
    public partial class TopicStatusEdit : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Gets or sets the spam word identifier.
        /// </summary>
        /// <value>
        /// The spam word identifier.
        /// </value>
        public int? TopicStatusId
        {
            get
            {
                return this.ViewState["TopicStatusId"].ToType<int?>();
            }

            set
            {
                this.ViewState["TopicStatusId"] = value;
            }
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="topicStatusId">The topic status identifier.</param>
        public void BindData(int? topicStatusId)
        {
            this.TopicStatusId = topicStatusId;

            this.Title.LocalizedPage = "ADMIN_TOPICSTATUS_EDIT";
            this.Save.TextLocalizedPage = "ADMIN_TOPICSTATUS";

            if (this.TopicStatusId.HasValue)
            {
                // Edit
                var topicStatus = this.GetRepository<TopicStatus>().GetById(this.TopicStatusId.Value);

                this.TopicStatusName.Text = topicStatus.TopicStatusName;
                this.DefaultDescription.Text = topicStatus.DefaultDescription;

                this.Title.LocalizedTag = "TITLE_EDIT";
                this.Save.TextLocalizedTag = "SAVE";
            }
            else
            {
                // Add
                this.TopicStatusName.Text = string.Empty;
                this.DefaultDescription.Text = string.Empty;

                this.Title.LocalizedTag = "TITLE";
                this.Save.TextLocalizedTag = "ADD";
            }
        }

        /// <summary>
        /// Handles the Click event of the Add control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Save_OnClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.TopicStatusName.Text.Trim().IsNotSet() || this.DefaultDescription.Text.Trim().IsNotSet())
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_TOPICSTATUS_EDIT", "MSG_ENTER"),
                    MessageTypes.warning);
            }
            else
            {
                this.GetRepository<TopicStatus>().Save(
                    this.TopicStatusId,
                    this.TopicStatusName.Text.Trim(),
                    this.DefaultDescription.Text.Trim(),
                    this.PageContext.PageBoardID);

                YafBuildLink.Redirect(ForumPages.admin_topicstatus);
            }
        }

        #endregion
    }
}