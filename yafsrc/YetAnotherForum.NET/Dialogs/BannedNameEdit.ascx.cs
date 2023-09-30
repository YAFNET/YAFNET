/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Dialogs;

using YAF.Types.Models;

/// <summary>
/// The Admin Banned Name Add/Edit Dialog.
/// </summary>
public partial class BannedNameEdit : BaseUserControl
{
    /// <summary>
    /// Gets or sets the banned identifier.
    /// </summary>
    /// <value>
    /// The banned identifier.
    /// </value>
    public int? BannedId
    {
        get => this.ViewState["BannedId"].ToType<int?>();

        set => this.ViewState["BannedId"] = value;
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    /// <param name="bannedId">The banned identifier.</param>
    public void BindData(int? bannedId)
    {
        this.BannedId = bannedId;

        this.Title.LocalizedPage = "ADMIN_BANNEDNAME_EDIT";
        this.Save.TextLocalizedPage = "ADMIN_BANNEDNAME";

        if (this.BannedId.HasValue)
        {
            // Edit
            var banned = this.GetRepository<BannedName>().GetById(this.BannedId.Value);

            if (banned != null)
            {
                this.mask.Text = banned.Mask;
                this.BanReason.Text = banned.Reason;
            }

            this.Title.LocalizedTag = "TITLE_EDIT";
            this.Save.TextLocalizedTag = "SAVE";
        }
        else
        {
            // Add
            this.mask.Text = string.Empty;
            this.BanReason.Text = string.Empty;

            this.Title.LocalizedTag = "TITLE";
            this.Save.TextLocalizedTag = "ADD_IP";
        }
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
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            return;
        }

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            "loadValidatorFormJs",
            JavaScriptBlocks.FormValidatorJs(this.Save.ClientID));
    }

    /// <summary>
    /// Handles the Click event of the Add control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Save_OnClick(object sender, EventArgs e)
    {
        if (!this.Page.IsValid)
        {
            return;
        }

        if (!this.GetRepository<BannedName>().Save(this.BannedId, this.mask.Text.Trim(), this.BanReason.Text.Trim()))
        {
            this.PageBoardContext.LoadMessage.AddSession(
                this.GetText("ADMIN_BANNEDNAME", "MSG_EXIST"),
                MessageTypes.warning);
        }

        // go back to banned names administration page
        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_BannedNames);
    }
}