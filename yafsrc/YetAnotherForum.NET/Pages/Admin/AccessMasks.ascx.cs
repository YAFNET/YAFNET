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
    using System.Web.UI.WebControls;

    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Admin Access Masks Page.
    /// </summary>
    public partial class AccessMasks : AdminPage
    {
        #region Methods

        /// <summary>
        /// The bit set.
        /// </summary>
        /// <param name="_o">
        /// The _o.
        /// </param>
        /// <param name="bitmask">
        /// The bitmask.
        /// </param>
        /// <returns>
        /// The bit set.
        /// </returns>
        protected bool BitSet([NotNull] object _o, int bitmask)
        {
            var i = (int)_o;
            return (i & bitmask) != 0;
        }

        /// <summary>
        /// Creates navigation page links on top of forum (breadcrumbs).
        /// </summary>
        protected override void CreatePageLinks()
        {
            // board index
            this.PageLinks.AddRoot();

            // administration index
            this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), BuildLink.GetLink(ForumPages.Admin_Admin));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_ACCESSMASKS", "TITLE"));

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_ACCESSMASKS", "TITLE")}";
        }

        /// <summary>
        /// Format access mask setting color formatting.
        /// </summary>
        /// <param name="enabled">
        /// The enabled.
        /// </param>
        /// <returns>
        /// Set access mask flags are rendered green if true, and if not red
        /// </returns>
        protected string GetItemColor(bool enabled)
        {
            // show enabled flag red
            return enabled ? "badge badge-success" : "badge badge-danger";
        }

        /// <summary>
        /// Get a user friendly item name.
        /// </summary>
        /// <param name="enabled">
        /// The enabled.
        /// </param>
        /// <returns>
        /// Item Name.
        /// </returns>
        protected string GetItemName(bool enabled)
        {
            return enabled ? this.GetText("DEFAULT", "YES") : this.GetText("DEFAULT", "NO");
        }

        /// <summary>
        /// Lists the item command.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void ListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":

                    // redirect to editing page
                    BuildLink.Redirect(ForumPages.Admin_EditAccessMask, "i={0}", e.CommandArgument);
                    break;
                case "delete":

                    // attempt to delete access masks
                    if (this.GetRepository<AccessMask>().DeleteById(e.CommandArgument.ToType<int>()))
                    {
                        // remove cache of forum moderators
                        this.Get<IDataCache>().Remove(Constants.Cache.ForumModerators);
                        this.BindData();
                    }
                    else
                    {
                        // used masks cannot be deleted
                        this.PageContext.AddLoadMessage(
                            this.GetText("ADMIN_ACCESSMASKS", "MSG_NOT_DELETE"),
                            MessageTypes.warning);
                    }

                    // quit switch
                    break;
            }
        }

        /// <summary>
        /// Handles the Click event of the New control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void NewClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // redirect to page for access mask creation
            BuildLink.Redirect(ForumPages.Admin_EditAccessMask);
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

            // bind data
            this.BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            // list all access masks for this board
            this.List.DataSource = this.GetRepository<AccessMask>().GetByBoardId();
            this.DataBind();
        }

        #endregion
    }
}