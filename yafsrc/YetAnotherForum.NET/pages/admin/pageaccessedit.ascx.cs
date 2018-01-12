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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Admin Edit Admin Page Access Page
    /// </summary>
    public partial class pageaccessedit : AdminPage
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
            YafBuildLink.Redirect(ForumPages.admin_pageaccesslist);
        }

        /// <summary>
        /// Creates navigation page links on top of forum (breadcrumbs).
        /// </summary>
        protected override void CreatePageLinks()
        {
            // beard index
            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));

            // administration index
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                YafBuildLink.GetLink(ForumPages.admin_admin));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_PAGEACCESSEDIT", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_PAGEACCESSLIST", "TITLE"),
                this.GetText("ADMIN_PAGEACCESSEDIT", "TITLE"));
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
            if (this.Request.QueryString.GetFirstOrDefault("u") == null)
            {
                return;
            }

            object userId = this.Request.QueryString.GetFirstOrDefault("u");

            foreach (RepeaterItem ri in this.AccessList.Items)
            {
                var readAccess = ((CheckBox)ri.FindControl("ReadAccess")).Checked;
                var pageName = ((Label)ri.FindControl("PageName")).Text.Trim();
                if (readAccess || "admin_admin".ToLowerInvariant() == pageName.ToLowerInvariant())
                {
                    // save it
                    LegacyDb.adminpageaccess_save(userId, pageName);
                }
                else
                {
                    LegacyDb.adminpageaccess_delete(userId, pageName);
                }
            }

            YafBuildLink.Redirect(ForumPages.admin_pageaccesslist);
        }

        /// <summary>
        /// Grants all click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void GrantAllClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // save permissions to table -  checked only
            if (this.Request.QueryString.GetFirstOrDefault("u") != null)
            {
                object userId = this.Request.QueryString.GetFirstOrDefault("u");
                foreach (RepeaterItem ri in this.AccessList.Items)
                {
                    // save it
                    LegacyDb.adminpageaccess_save(userId, ((Label)ri.FindControl("PageName")).Text.Trim());
                }
            }

            YafBuildLink.Redirect(ForumPages.admin_pageaccesslist);
        }

        /// <summary>
        /// The RevokeAll _Click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void RevokeAllClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // revoke permissions by deleting records from table. Number of records ther should be minimal.
            if (this.Request.QueryString.GetFirstOrDefault("u") != null)
            {
                object userId = this.Request.QueryString.GetFirstOrDefault("u");
                foreach (RepeaterItem ri in this.AccessList.Items)
                {
                    var pageName = ((Label)ri.FindControl("PageName")).Text.Trim();

                    // save it - admin index should be always available
                    if ("admin_admin".ToLowerInvariant() != pageName.ToLowerInvariant())
                    {
                        LegacyDb.adminpageaccess_delete(userId, ((Label)ri.FindControl("PageName")).Text.Trim());
                    }
                }
            }

            YafBuildLink.Redirect(ForumPages.admin_pageaccesslist);
        }

        /// <summary>
        /// The PollGroup item command.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void AccessList_OnItemDataBound([NotNull] object source, [NotNull] RepeaterItemEventArgs e)
        {
            var item = e.Item;
            var drowv = (AdminPageAccess)e.Item.DataItem;

            if (item.ItemType != ListItemType.Item && item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var pageName = item.FindControlRecursiveAs<Label>("PageName");
            var pageText = item.FindControlRecursiveAs<Label>("PageText");
            var readAccess = item.FindControlRecursiveAs<CheckBox>("ReadAccess");
            pageText.Text = this.GetText("ACTIVELOCATION", drowv.PageName.ToUpperInvariant());
            pageName.Text = drowv.PageName;
            readAccess.Checked = drowv.ReadAccess;
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            var found = false;

            if (this.Request.QueryString.GetFirstOrDefault("u") != null)
            {
                // Load the page access list.
                var dt = LegacyDb.adminpageaccess_list(this.Request.QueryString.GetFirstOrDefault("u"), null);

                // Get admin pages by page prefixes.
                var listPages = Enum.GetNames(typeof(ForumPages))
                    .Where(e => e.IndexOf("admin_", StringComparison.Ordinal) >= 0);

                // Initialize list with a helper class.
                var adminPageAccesses = new List<AdminPageAccess>();

                // Protected hostadmin pages
                var hostPages = new[]
                                    {
                                        "admin_boards", "admin_hostsettings", "admin_pageaccesslist",
                                        "admin_pageaccessedit", "admin_eventloggroups", "admin_eventloggroupaccess"
                                    };

                // Iterate thru all admin pages
                foreach (var listPage in listPages.ToList())
                {
                    if (dt != null && dt.Rows.Cast<DataRow>().Any(
                            dr => dr["PageName"].ToString() == listPage
                                  && hostPages.All(s => s != dr["PageName"].ToString())))
                    {
                        found = true;
                        adminPageAccesses.Add(
                            new AdminPageAccess
                                {
                                    UserId = this.Request.QueryString.GetFirstOrDefault("u")
                                        .ToType<int>(),
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
                                    UserId = this.Request.QueryString.GetFirstOrDefault("u")
                                        .ToType<int>(),
                                    PageName = listPage,
                                    ReadAccess = false
                                });
                    }

                    // Reset flag in the end of the outer loop
                    found = false;
                }

                this.UserName.Text = this.HtmlEncode(
                    this.Get<IUserDisplayName>()
                        .GetName(this.Request.QueryString.GetFirstOrDefault("u").ToType<int>()));

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