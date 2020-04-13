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
    using System.Web.UI.WebControls;

    using YAF.Core.BaseControls;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;

    using DateTime = System.DateTime;

    #endregion

    /// <summary>
    /// The most active users.
    /// </summary>
    public partial class MostActiveUsers : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets DisplayNumber.
        /// </summary>
        public int DisplayNumber { get; set; } = 10;

        /// <summary>
        ///   Gets or sets Number of Days.
        /// </summary>
        public int LastNumOfDays { get; set; } = 7;

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            // bind data
            this.BindData();
        }

        /// <summary>
        /// The users_ on item data bound.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Users_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var row = (DataRowView)e.Item.DataItem;

            var userLink = e.Item.FindControlAs<UserLink>("UserLink");

            // render UserLink...
            userLink.UserID = row["ID"].ToType<int>();
        }

        /// <summary>
        /// Binds the Data.
        /// </summary>
        private void BindData()
        {
            var users = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.MostActiveUsers,
                () => this.GetRepository<User>().ActivityRankAsDataTable(
                    this.PageContext.PageBoardID,
                    DateTime.UtcNow.AddDays(-this.LastNumOfDays),
                    this.DisplayNumber),
                TimeSpan.FromMinutes(5));

            this.Users.DataSource = users;
            this.Users.DataBind();

            this.IconHeader.Param0 = this.LastNumOfDays.ToString();
        }

        #endregion
    }
}