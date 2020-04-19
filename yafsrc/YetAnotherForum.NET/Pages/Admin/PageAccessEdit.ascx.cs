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
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Core.BasePages;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Admin Edit Admin Page Access Page
    /// </summary>
    public partial class PageAccessEdit : AdminPage
    {
        #region Methods

        /// <summary>
        /// Cancels the click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void CancelClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // get back to access admin list
            BuildLink.Redirect(ForumPages.Admin_PageAccessList);
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

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_PAGEACCESSEDIT", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMIN_PAGEACCESSLIST", "TITLE")} - {this.GetText("ADMIN_PAGEACCESSEDIT", "TITLE")}";
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

            this.BindData();
        }

        /// <summary>
        /// Handles the Click event of the Save control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void SaveClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // retrieve access mask ID from parameter (if applicable)
            if (!this.Get<HttpRequestBase>().QueryString.Exists("u"))
            {
                return;
            }

            var userId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("u");

            this.AccessList.Items.Cast<RepeaterItem>().ForEach(
                ri =>
                {
                    var readAccess = ri.FindControlAs<CheckBox>("ReadAccess").Checked;
                    var pageName = ri.FindControlAs<Label>("PageName").Text.Trim();

                    if (readAccess || string.Equals("admin_admin", pageName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // save it
                        this.GetRepository<AdminPageUserAccess>().Save(userId, pageName);
                    }
                    else
                    {
                        this.GetRepository<AdminPageUserAccess>().Delete(userId, pageName);
                    }
                });

            BuildLink.Redirect(ForumPages.Admin_PageAccessList);
        }

        /// <summary>
        /// Grants all click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void GrantAllClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // save permissions to table -  checked only
            if (this.Get<HttpRequestBase>().QueryString.Exists("u"))
            {
                var userId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("u");

                this.AccessList.Items.Cast<RepeaterItem>().ForEach(
                    ri =>
                        {
                            // save it
                            this.GetRepository<AdminPageUserAccess>().Save(
                                userId,
                                ri.FindControlAs<Label>("PageName").Text.Trim());
                        });
            }

            BuildLink.Redirect(ForumPages.Admin_PageAccessList);
        }

        /// <summary>
        /// The RevokeAll _Click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void RevokeAllClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // revoke permissions by deleting records from table. Number of records there should be minimal.
            if (this.Get<HttpRequestBase>().QueryString.Exists("u"))
            {
                var userId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("u");

                this.AccessList.Items.Cast<RepeaterItem>().ForEach(ri =>
                    {
                        var pageName = ri.FindControlAs<Label>("PageName").Text.Trim();

                        // save it - admin index should be always available
                        if (!string.Equals("admin_admin", pageName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            this.GetRepository<AdminPageUserAccess>().Delete(
                                userId,
                                ri.FindControlAs<Label>("PageName").Text.Trim());
                        }
                    });
            }

            BuildLink.Redirect(ForumPages.Admin_PageAccessList);
        }

        /// <summary>
        /// The PollGroup item command.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void AccessList_OnItemDataBound([NotNull] object source, [NotNull] RepeaterItemEventArgs e)
        {
            var item = e.Item;
            var row = (AdminPageAccess)e.Item.DataItem;

            if (item.ItemType != ListItemType.Item && item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var pageName = item.FindControlRecursiveAs<Label>("PageName");
            var readAccess = item.FindControlRecursiveAs<CheckBox>("ReadAccess");
            pageName.Text = row.PageName;
            readAccess.Checked = row.ReadAccess;
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            var found = false;

            if (this.Get<HttpRequestBase>().QueryString.Exists("u"))
            {
                // Load the page access list.
                var dt = this.GetRepository<AdminPageUserAccess>().List(
                    this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("u"),
                    null);

                // Get admin pages by page prefixes.
                var listPages = Enum.GetNames(typeof(ForumPages))
                    .Where(e => e.IndexOf("admin_", StringComparison.Ordinal) >= 0);

                // Initialize list with a helper class.
                var adminPageAccesses = new List<AdminPageAccess>();

                // Protected host-admin pages
                var hostPages = new[]
                                    {
                                        "admin_boards", "admin_hostsettings", "admin_pageaccesslist",
                                        "admin_pageaccessedit"
                                    };

                // Iterate thru all admin pages
                listPages.ToList().ForEach(
                    listPage =>
                        {
                            if (dt != null && dt.Rows.Cast<DataRow>().Any(
                                    dr => dr["PageName"].ToString() == listPage
                                          && hostPages.All(s => s != dr["PageName"].ToString())))
                            {
                                found = true;
                                adminPageAccesses.Add(
                                    new AdminPageAccess
                                        {
                                            UserId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u").ToType<int>(),
                                            PageName = listPage,
                                            ReadAccess = true
                                        });
                            }

                            // If it doesn't contain page for the user add it.
                            if (!found && hostPages.All(s => s != listPage))
                            {
                                adminPageAccesses.Add(
                                    new AdminPageAccess
                                        {
                                            UserId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u").ToType<int>(),
                                            PageName = listPage,
                                            ReadAccess = false
                                        });
                            }

                            // Reset flag in the end of the outer loop
                            found = false;
                        });

                this.IconHeader.Text =
                    $"{this.GetText("HEADER")}{this.GetText("USERNAME")}: <strong>{this.HtmlEncode(this.Get<IUserDisplayName>().GetName(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u").ToType<int>()))}</strong>";

                // get admin pages list with access flags.
                this.AccessList.DataSource = adminPageAccesses.AsEnumerable();
            }

            this.DataBind();
        }

        #endregion
    }

    /// <summary>
    /// Provides a common wrapper for variables of various origins.
    /// </summary>
    internal class AdminPageAccess
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        internal int UserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the page.
        /// </summary>
        /// <value>
        /// The name of the page.
        /// </value>
        internal string PageName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [read access].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [read access]; otherwise, <c>false</c>.
        /// </value>
        internal bool ReadAccess { get; set; }
    }
}