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

namespace YAF.Pages.Profile
{
    #region Using

    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Web.Controls;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The attachments Page Class.
    /// </summary>
    public partial class Attachments : ProfilePage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Attachments" /> class.
        /// </summary>
        public Attachments()
            : base("ATTACHMENTS")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The pager top_ page change.
        /// </summary>
        /// <param name="sender">
        /// The source of the event. 
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data. 
        /// </param>
        protected void PagerTop_PageChange([NotNull] object sender, [NotNull] EventArgs e)
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

            this.PageSize.DataSource = StaticDataHelper.PageEntries();
            this.PageSize.DataTextField = "Name";
            this.PageSize.DataValueField = "Value";
            this.PageSize.DataBind();

            this.PageSize.SelectedValue = this.PageContext.User.PageSize.ToString();

            this.BindData();
        }

        /// <summary>
        /// The create page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            var displayName = this.PageContext.User.DisplayOrUserName();
            this.PageLinks.Clear();
            this.PageLinks.AddRoot();
            this.PageLinks.AddUser(this.PageContext.PageUserID, displayName);
            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
        }

        /// <summary>
        /// Gets the preview image.
        /// </summary>
        /// <param name="o">The Data Row object.</param>
        /// <returns>Returns the Preview Image</returns>
        protected string GetPreviewImage([NotNull] object o)
        {
            var attach = o.ToType<Attachment>();

            var fileName = attach.FileName;
            var isImage = fileName.IsImageName();
            var url =
                $"{BoardInfo.ForumClientFileRoot}resource.ashx?i={attach.ID}&b={this.PageContext.PageBoardID}&editor=true";

            return isImage
                       ? $"<img src=\"{url}\" alt=\"{fileName}\" title=\"{fileName}\" data-url=\"{url}\" style=\"max-width:30px\" class=\"me-2\" />"
                       : "<i class=\"far fa-file-alt attachment-icon me-2\"></i>";
        }

        /// <summary>
        /// Delete all selected attachment(s)
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void DeleteAttachments_Click(object sender, EventArgs e)
        {
            var items = (from RepeaterItem item in this.List.Items
                         where item.ItemType is ListItemType.Item or ListItemType.AlternatingItem
                         where item.FindControlAs<CheckBox>("Selected").Checked
                         select item).ToList();

            if (items.Any())
            {
                items.ForEach(
                    item => this.GetRepository<Attachment>().DeleteById(
                        item.FindControlAs<ThemeButton>("ThemeButtonDelete").CommandArgument.ToType<int>()));

                this.PageContext.AddLoadMessage(this.GetTextFormatted("DELETED", items.Count), MessageTypes.success);
            }

            this.BindData();
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.PagerTop.PageSize = this.PageSize.SelectedValue.ToType<int>();

            var dt = this.GetRepository<Attachment>().GetPaged(
                a => a.UserID == this.PageContext.PageUserID,
                this.PagerTop.CurrentPageIndex,
                this.PagerTop.PageSize);

            this.List.DataSource = dt;
            this.PagerTop.Count = dt != null && dt.Any()
                                      ? this.GetRepository<Attachment>().Count(a => a.UserID == this.PageContext.PageUserID).ToType<int>()
                                      : 0;

            this.DataBind();
        }

        #endregion
    }
}