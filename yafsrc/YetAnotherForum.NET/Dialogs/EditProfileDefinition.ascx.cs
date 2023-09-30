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
/// The Profile Definition Add/Edit Dialog.
/// </summary>
public partial class EditProfileDefinition : BaseUserControl
{
    /// <summary>
    /// Gets or sets the banned identifier.
    /// </summary>
    /// <value>
    /// The Definition identifier.
    /// </value>
    public int? DefinitionId
    {
        get => this.ViewState["DefinitionId"].ToType<int?>();

        set => this.ViewState["DefinitionId"] = value;
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    /// <param name="definitionId">
    /// The definition Id.
    /// </param>
    public void BindData(int? definitionId)
    {
        this.DefinitionId = definitionId;

        this.Title.LocalizedPage = "ADMIN_EDIT_PROFILEDEFINITION";
        this.Save.TextLocalizedPage = "ADMIN_EDIT_PROFILEDEFINITION";

        this.DataTypes.DataSource = Enum.GetNames(typeof(DataType));
        this.DataTypes.DataBind();

        if (this.DefinitionId.HasValue)
        {
            // Edit
            var item = this.GetRepository<ProfileDefinition>().GetById(this.DefinitionId.Value);

            if (item != null)
            {
                this.Name.Text = item.Name;
                this.DataTypes.Items.FindByText(item.DataType).Selected = true;
                this.Length.Text = item.Length.ToString();
                this.Required.Checked = item.Required;
                this.ShowInUserInfo.Checked = item.ShowInUserInfo;
                this.ShowOnRegisterPage.Checked = item.ShowOnRegisterPage;
                this.DefaultValue.Text = item.DefaultValue;
            }

            this.Title.LocalizedTag = "TITLE_EDIT";
            this.Save.TextLocalizedTag = "SAVE";
        }
        else
        {
            // Add
            this.Name.Text = string.Empty;

            this.Title.LocalizedTag = "TITLE";
            this.Save.TextLocalizedTag = "SAVE_NEW";
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
    protected void SaveClick(object sender, EventArgs e)
    {
        if (!this.Page.IsValid)
        {
            return;
        }

        this.GetRepository<ProfileDefinition>().Upsert(
            new ProfileDefinition
                {
                    BoardID = this.PageBoardContext.PageBoardID,
                    ID = this.DefinitionId ?? 0,
                    Name = this.Name.Text,
                    DataType = this.DataTypes.SelectedValue,
                    DefaultValue = this.DefaultValue.Text,
                    Length = this.Length.Text.ToType<int>(),
                    Required = this.Required.Checked,
                    ShowInUserInfo = this.ShowInUserInfo.Checked,
                    ShowOnRegisterPage = this.ShowOnRegisterPage.Checked
                });

        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_ProfileDefinitions);
    }
}