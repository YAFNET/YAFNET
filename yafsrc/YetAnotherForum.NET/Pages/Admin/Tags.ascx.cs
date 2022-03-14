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
namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Admin Manage Tags Page.
    /// </summary>
    public partial class Tags : AdminPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Tags" /> class.
        /// </summary>
        public Tags()
            : base("ADMIN_TAGS", ForumPages.Admin_Tags)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // do it only once, not on post-backs
            if (this.IsPostBack)
            {
                return;
            }

            this.PageSize.DataSource = StaticDataHelper.PageEntries();
            this.PageSize.DataTextField = "Name";
            this.PageSize.DataValueField = "Value";
            this.PageSize.DataBind();

            try
            {
                this.PageSize.SelectedValue = this.PageBoardContext.PageUser.PageSize.ToString();
            }
            catch (Exception)
            {
                this.PageSize.SelectedValue = "5";
            }

            // bind data to controls
            this.BindData();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();

            // administration index second
            this.PageLinks.AddAdminIndex();

            this.PageLinks.AddLink(this.GetText("ADMIN_TAGS", "TITLE"), string.Empty);
        }

        /// <summary>
        /// The pager top_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PagerTopPageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            // rebind
            this.BindData();
        }

        /// <summary>
        /// The page size on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PageSizeSelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// Handles single record commands in a repeater.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void ListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            // what command are we serving?
            switch (e.CommandName)
            {
                // delete log entry
                case "delete":

                    var tagId = e.CommandArgument.ToType<int>();

                    this.GetRepository<TopicTag>().Delete(x => x.TagID == tagId);

                    this.GetRepository<Tag>().DeleteById(tagId);

                    // re-bind controls
                    this.BindData();
                    break;
            }
        }

        /// <summary>
        /// Populates data source and binds data to controls.
        /// </summary>
        private void BindData()
        {
            var baseSize = this.PageSize.SelectedValue.ToType<int>();
            var currentPageIndex = this.PagerTop.CurrentPageIndex;
            this.PagerTop.PageSize = baseSize;

            // list event for this board
            var list = this.GetRepository<Tag>().GetPaged(
                x => x.BoardID == this.PageBoardContext.PageBoardID,
                currentPageIndex,
                baseSize);

            this.List.DataSource = list;

            this.PagerTop.Count = !list.NullOrEmpty() ? this.GetRepository<Tag>().GetByBoardId().Count : 0;

            // bind data to controls
            this.DataBind();

            if (this.List.Items.Count == 0)
            {
                this.NoInfo.Visible = true;
            }
        }

        #endregion
    }
}