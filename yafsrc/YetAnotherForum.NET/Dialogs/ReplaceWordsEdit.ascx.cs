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

namespace YAF.Dialogs;

#region Using

using YAF.Types.Models;

#endregion

/// <summary>
/// The Admin Replace Words Add/Edit Dialog.
/// </summary>
public partial class ReplaceWordsEdit : BaseUserControl
{
    #region Methods

    /// <summary>
    /// Gets or sets the spam word identifier.
    /// </summary>
    /// <value>
    /// The spam word identifier.
    /// </value>
    public int? ReplaceWordId
    {
        get => this.ViewState["ReplaceWordId"].ToType<int?>();

        set => this.ViewState["ReplaceWordId"] = value;
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    /// <param name="replaceWordId">The replace word identifier.</param>
    public void BindData(int? replaceWordId)
    {
        this.ReplaceWordId = replaceWordId;

        this.Title.LocalizedPage = "ADMIN_REPLACEWORDS_EDIT";
        this.Save.TextLocalizedPage = "ADMIN_REPLACEWORDS";

        if (this.ReplaceWordId.HasValue)
        {
            // Edit
            var replaceWord = this.GetRepository<Replace_Words>().GetById(this.ReplaceWordId.Value);

            this.badword.Text = replaceWord.BadWord;
            this.goodword.Text = replaceWord.GoodWord;

            this.Title.LocalizedTag = "TITLE_EDIT";
            this.Save.TextLocalizedTag = "SAVE";
        }
        else
        {
            // Add
            this.badword.Text = string.Empty;
            this.goodword.Text = string.Empty;

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
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
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
    protected void Save_OnClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.Page.IsValid)
        {
            return;
        }

        if (!ValidationHelper.IsValidRegex(this.badword.Text.Trim()))
        {
            this.PageBoardContext.Notify(
                this.GetText("ADMIN_REPLACEWORDS_EDIT", "MSG_REGEX_BAD"),
                MessageTypes.warning);

            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                "openModalJs",
                JavaScriptBlocks.OpenModalJs("ReplaceWordsEditDialog"));

            return;
        }

        this.GetRepository<Replace_Words>()
            .Save(
                this.ReplaceWordId,
                this.badword.Text,
                this.goodword.Text);

        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_ReplaceWords);
    }

    #endregion
}