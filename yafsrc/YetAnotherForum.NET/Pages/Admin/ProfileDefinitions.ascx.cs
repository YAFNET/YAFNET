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
/// The Admin Profile Definitions Page.
/// </summary>
public partial class ProfileDefinitions : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileDefinitions"/> class. 
    /// </summary>
    public ProfileDefinitions()
        : base("ADMIN_PROFILEDEFINITIONS", ForumPages.Admin_ProfileDefinitions)
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
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_PROFILEDEFINITIONS", "TITLE"));
    }

    /// <summary>
    /// Lists the item command.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
    protected void ListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
    {
        var defId = e.CommandArgument.ToType<int>();

        switch (e.CommandName)
        {
            case "new":
                this.EditDialog.BindData(null);

                this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                    "openModalJs",
                    JavaScriptBlocks.OpenModalJs("EditDialog"));
                break;
            case "edit":

                this.EditDialog.BindData(defId);

                this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                    "openModalJs",
                    JavaScriptBlocks.OpenModalJs("EditDialog"));
                break;
            case "delete":

                this.GetRepository<ProfileCustom>().Delete(x => x.ProfileDefinitionID == defId);

                this.GetRepository<ProfileDefinition>().DeleteById(defId);

                this.BindData();

                // quit switch
                break;
        }
    }

    protected void AddClick(object sender, EventArgs e)
    {
        this.EditDialog.BindData(null);

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            "openModalJs",
            JavaScriptBlocks.OpenModalJs("EditDialog"));
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
        this.List.DataSource = this.GetRepository<ProfileDefinition>().GetByBoardId();
        this.DataBind();

        if (this.List.Items.Count == 0)
        {
            this.EmptyState.Visible = true;
        }
    }
}