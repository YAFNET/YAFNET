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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
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
    /// The attachments Page Class.
    /// </summary>
    public partial class attachments : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "attachments" /> class.
        /// </summary>
        public attachments()
            : base("ATTACHMENTS")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Click event of the Back control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Back_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.cp_profile);
        }

        /// <summary>
        /// Handles the Load event of the Delete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("ATTACHMENTS", "CONFIRM_DELETE"));
        }

        /// <summary>
        /// Handles the ItemCommand event of the List control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void List_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "delete":
                    this.GetRepository<Attachment>().Delete(e.CommandArgument.ToType<int>());

                    this.BindData();
                    break;
            }
        }

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

            var displayName = this.PageContext.BoardSettings.EnableDisplayName
                                  ? UserMembershipHelper.GetDisplayNameFromID(this.PageContext.PageUserID)
                                  : UserMembershipHelper.GetUserNameFromID(this.PageContext.PageUserID);
            this.PageLinks.Clear();
            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(
                displayName,
                YafBuildLink.GetLink(ForumPages.profile, "u={0}", this.PageContext.PageUserID, displayName));
            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);


            this.Back.Text = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t").IsNotSet()
                                 ? this.GetText("BACK")
                                 : this.GetText("COMMON", "CONTINUE");

            this.BindData();
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
            var url = isImage
                          ? "{0}resource.ashx?i={1}&b={2}&editor=true".FormatWith(
                              YafForumInfo.ForumClientFileRoot,
                              attach.ID,
                              this.PageContext.PageBoardID)
                          : "{0}Images/document.png".FormatWith(YafForumInfo.ForumClientFileRoot);

            var dataUrl = isImage ? " data-url=\"{0}\"".FormatWith(url) : string.Empty;

            return "<img src=\"{0}\" alt=\"{1}\" title=\"{1}\"{2} style=\"max-width:30px\" />".FormatWith(url, fileName, dataUrl);
        }

        protected void DeleteAttachments_Click(object sender, EventArgs e)
        {
            foreach (var item in from RepeaterItem item in this.List.Items
                                          where
                                              item.ItemType == ListItemType.Item
                                              || item.ItemType == ListItemType.AlternatingItem
                                          where item.FindControlAs<CheckBox>("Selected").Checked
                                          select item)
            {
                this.GetRepository<Attachment>()
                    .DeleteById(item.FindControlAs<ThemeButton>("ThemeButtonDelete").CommandArgument.ToType<int>());
            }

            this.BindData();
        }
        
        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.PagerTop.PageSize = this.Get<YafBoardSettings>().MemberListPageSize;

            var dt = this.GetRepository<Attachment>().GetPaged(
                a => a.UserID == this.PageContext.PageUserID,
                pageIndex: this.PagerTop.CurrentPageIndex,
                pageSize: this.PagerTop.PageSize);

            this.List.DataSource = dt;
            this.PagerTop.Count = dt != null && dt.Any()
                                      ? this.GetRepository<Attachment>().Count(a => a.UserID == this.PageContext.PageUserID).ToType<int>()
                                      : 0;

            this.DataBind();
        }

        #endregion
    }
}