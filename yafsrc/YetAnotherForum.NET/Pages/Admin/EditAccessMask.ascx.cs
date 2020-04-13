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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Web;

    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Admin Page for Editing or Creating an Forum Access Mask
    /// </summary>
    public partial class EditAccessMask : AdminPage
    {
        #region Methods

        /// <summary>
        /// Cancel Edit and Return Back To Access Mask List Page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CancelClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // get back to access masks administration
            BuildLink.Redirect(ForumPages.Admin_AccessMasks);
        }

        /// <summary>
        /// Creates navigation page links on top of forum (breadcrumbs).
        /// </summary>
        protected override void CreatePageLinks()
        {
            // beard index
            this.PageLinks.AddRoot();

            // administration index
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.Admin_Admin));

            this.PageLinks.AddLink(
                this.GetText("ADMIN_ACCESSMASKS", "TITLE"),
                BuildLink.GetLink(ForumPages.Admin_AccessMasks));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_EDITACCESSMASKS", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_ACCESSMASKS", "TITLE")} - {this.GetText("ADMIN_EDITACCESSMASKS", "TITLE")}";
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
        /// Saves The Access Mask
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // retrieve access mask ID from parameter (if applicable)
            int? accessMaskId = null;

            if (this.Get<HttpRequestBase>().QueryString.Exists("i"))
            {
                accessMaskId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("i");
            }

            if (this.Name.Text.Trim().Length <= 0)
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITACCESSMASKS", "MSG_MASK_NAME"),
                    MessageTypes.warning);
                return;
            }

            if (!ValidationHelper.IsValidPosShort(this.SortOrder.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITACCESSMASKS", "MSG_POSITIVE_SORT"),
                    MessageTypes.warning);
                return;
            }

            if (!short.TryParse(this.SortOrder.Text.Trim(), out var sortOrder))
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITACCESSMASKS", "MSG_NUMBER_SORT"),
                    MessageTypes.warning);
                return;
            }

            // save it
            this.GetRepository<AccessMask>()
                .Save(
                    accessMaskId,
                    this.Name.Text,
                    this.ReadAccess.Checked,
                    this.PostAccess.Checked,
                    this.ReplyAccess.Checked,
                    this.PriorityAccess.Checked,
                    this.PollAccess.Checked,
                    this.VoteAccess.Checked,
                    this.ModeratorAccess.Checked,
                    this.EditAccess.Checked,
                    this.DeleteAccess.Checked,
                    this.UploadAccess.Checked,
                    this.DownloadAccess.Checked,
                    sortOrder);

            // empty out access table(s)
            this.GetRepository<Active>().DeleteAll();
            this.GetRepository<ActiveAccess>().DeleteAll();

            // clear cache
            this.Get<IDataCache>().Remove(Constants.Cache.ForumModerators);

            // get back to access masks administration
            BuildLink.Redirect(ForumPages.Admin_AccessMasks);
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            if (this.Get<HttpRequestBase>().QueryString.Exists("i"))
            {
                var accessMask = this.GetRepository<AccessMask>()
                    .GetById(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("i"));

                if (accessMask != null)
                {
                    // get access mask properties
                    this.Name.Text = accessMask.Name;
                    this.SortOrder.Text = accessMask.SortOrder.ToString();

                    // get flags
                    var flags = new AccessFlags(accessMask.Flags);
                    this.ReadAccess.Checked = flags.ReadAccess;
                    this.PostAccess.Checked = flags.PostAccess;
                    this.ReplyAccess.Checked = flags.ReplyAccess;
                    this.PriorityAccess.Checked = flags.PriorityAccess;
                    this.PollAccess.Checked = flags.PollAccess;
                    this.VoteAccess.Checked = flags.VoteAccess;
                    this.ModeratorAccess.Checked = flags.ModeratorAccess;
                    this.EditAccess.Checked = flags.EditAccess;
                    this.DeleteAccess.Checked = flags.DeleteAccess;
                    this.UploadAccess.Checked = flags.UploadAccess;
                    this.DownloadAccess.Checked = flags.DownloadAccess;
                }
            }
            else
            {
                this.SortOrder.Text =
                    (this.GetRepository<AccessMask>().Count(x => x.BoardID == this.PageContext.PageBoardID) + 1)
                    .ToString();
            }

            this.DataBind();
        }

        #endregion
    }
}