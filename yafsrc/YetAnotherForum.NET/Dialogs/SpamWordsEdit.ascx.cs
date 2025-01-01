﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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
/// The Admin Spam Words Add/Edit Dialog.
/// </summary>
public partial class SpamWordsEdit : BaseUserControl
{
    /// <summary>
    /// Gets or sets the spam word identifier.
    /// </summary>
    /// <value>
    /// The spam word identifier.
    /// </value>
    public int? SpamWordId
    {
        get => this.ViewState["SpamWordId"].ToType<int?>();

        set => this.ViewState["SpamWordId"] = value;
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    /// <param name="spamWordId">The spam word identifier.</param>
    public void BindData(int? spamWordId)
    {
        this.SpamWordId = spamWordId;

        this.Title.LocalizedPage = "ADMIN_SPAMWORDS_EDIT";
        this.Save.TextLocalizedPage = "ADMIN_SPAMWORDS";

        if (this.SpamWordId.HasValue)
        {
            // Edit
            var spamWord = this.GetRepository<Spam_Words>().GetById(this.SpamWordId.Value);

            if (spamWord != null)
            {
                this.spamword.Text = spamWord.SpamWord;
            }

            this.Title.LocalizedTag = "TITLE_EDIT";
            this.Save.TextLocalizedTag = "SAVE";
        }
        else
        {
            // Add
            this.spamword.Text = string.Empty;

            this.Title.LocalizedTag = "TITLE";
            this.Save.TextLocalizedTag = "ADD";
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

        if (!ValidationHelper.IsValidRegex(this.spamword.Text.Trim()))
        {
            this.PageBoardContext.Notify(
                this.GetText("ADMIN_SPAMWORDS_EDIT", "MSG_REGEX_SPAM"),
                MessageTypes.danger);

            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                "openModalJs",
                JavaScriptBlocks.OpenModalJs("SpamWordsEditDialog"));
        }
        else
        {
            this.GetRepository<Spam_Words>().Save(
                this.SpamWordId,
                this.spamword.Text);

            this.Get<LinkBuilder>().Redirect(ForumPages.Admin_SpamWords);
        }
    }
}