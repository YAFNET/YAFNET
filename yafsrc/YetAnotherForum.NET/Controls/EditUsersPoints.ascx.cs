/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

    using YAF.Core.BaseControls;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// The edit users points.
    /// </summary>
    public partial class EditUsersPoints : BaseUserControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the current edit user.
        /// </summary>
        /// <value>The user.</value>
        public User User { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The add points_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void AddPoints_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.GetRepository<User>().AddPoints(this.User.ID, null, this.txtAddPoints.Text.ToType<int>());

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
            if (this.IsPostBack)
            {
                return;
            }

            this.BindData();
        }

        /// <summary>
        /// The remove points_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void RemovePoints_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.GetRepository<User>().RemovePoints(
                this.User.ID,
                null,
                this.txtRemovePoints.Text.ToType<int>());
            this.BindData();
        }

        /// <summary>
        /// The set user points_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void SetUserPoints_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.GetRepository<User>().SetPoints(this.User.ID, this.txtUserPoints.Text.ToType<int>());

            this.BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.ltrCurrentPoints.Text = this.txtUserPoints.Text = this.User.Points.ToString();
        }

        #endregion
    }
}