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

namespace YAF.Pages.Admin;

using YAF.Types.Models;

/// <summary>
/// The Admin Page Admin Access list page
/// </summary>
public partial class PageAccessList : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PageAccessList"/> class. 
    /// </summary>
    public PageAccessList()
        : base("ADMIN_PAGEACCESSLIST", ForumPages.Admin_PageAccessList)
    {
    }

    /// <summary>
    /// Creates navigation page links on top of forum (breadcrumbs).
    /// </summary>
    public override void CreatePageLinks()
    {
        // board index
        this.PageBoardContext.PageLinks.AddRoot();

        // administration index
        this.PageBoardContext.PageLinks.AddAdminIndex();

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_PAGEACCESSLIST", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the ItemCommand event of the List control.
    /// </summary>
    /// <param name="source">The source of the event.</param>
    /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
    protected void ListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "edit":

                // redirect to editing page
                this.Get<LinkBuilder>().Redirect(ForumPages.Admin_PageAccessEdit, new { u = e.CommandArgument });
                break;
        }
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
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        // list admins but not host admins
        this.List.DataSource = this.GetRepository<User>().ListAdmins(this.PageBoardContext.PageBoardID)
            .Where(u => !u.UserFlags.IsHostAdmin);
        this.DataBind();
    }
}