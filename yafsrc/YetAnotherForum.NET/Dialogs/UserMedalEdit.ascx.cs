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

using YAF.Types.Models;

/// <summary>
/// The User Medal Group Add/Edit Dialog.
/// </summary>
public partial class UserMedalEdit : BaseUserControl
{
    public string MedalName
    {
        get => this.ViewState["MedalName"].ToString();

        set => this.ViewState["MedalName"] = value;
    }

    /// <summary>
    /// Gets or sets the banned identifier.
    /// </summary>
    /// <value>
    /// The banned identifier.
    /// </value>
    public int MedalId
    {
        get => this.ViewState["MedalId"].ToType<int>();

        set => this.ViewState["MedalId"] = value;
    }

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    public int? UserId
    {
        get => this.ViewState["UserId"].ToType<int?>();

        set => this.ViewState["UserId"] = value;
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="medalId">
    /// The medal identifier.
    /// </param>
    public void BindData(int? userId, int medalId)
    {
        this.MedalId = medalId;
        this.UserId = userId;

        // clear controls
       
        this.UserName.Text = null;
        this.UserMessage.Text = null;
        this.UserHide.Checked = false;
        this.UserSortOrder.Text = "0";

        // set controls visibility and availability
        this.UserName.Enabled = true;
        this.UserName.Visible = true;

        // focus on save button
        this.AddUserSave.Focus();

        if (this.UserId.HasValue)
        {
            // Edit
            // load user-medal to the controls
            var row = this.GetRepository<UserMedal>().List(this.UserId, this.MedalId).FirstOrDefault();

            // tweak it for editing
            this.UserMedalEditTitle.Text = this.GetText("ADMIN_EDITMEDAL", "EDIT_MEDAL_USER");
            this.UserName.Enabled = false;

            // load data to controls
            this.UserName.Text = row.Item3.Name;
            this.UserMessage.Text = row.Item2.Message.IsSet() ? row.Item2.Message : row.Item1.Message;
            this.UserSortOrder.Text = row.Item2.SortOrder.ToString();
            this.UserHide.Checked = row.Item2.Hide;
            this.MedalName = row.Item1.Name;
        }
        else
        {
            var medal = this.GetRepository<Medal>().GetById(this.MedalId);

            this.UserSelectHolder.Visible = true;
            this.UserName.Visible = false;

            this.MedalName = medal.Name;

            // set title
            this.UserMedalEditTitle.Text = this.GetText("ADMIN_EDITMEDAL", "ADD_TOUSER");
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
            JavaScriptBlocks.FormValidatorJs(this.AddUserSave.ClientID));

        if (!this.UserId.HasValue)
        {
            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                nameof(JavaScriptBlocks.SelectUsersLoadJs),
                JavaScriptBlocks.SelectUsersLoadJs(
                    "UserEditDialog",
                    "UserSelect",
                    this.SelectedUserID.ClientID));
        }
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

        if (this.UserId.HasValue)
        {
            // save user, if there is no message specified, pass null
            this.GetRepository<UserMedal>().Save(
                this.UserId.Value,
                this.MedalId,
                this.UserMessage.Text.IsNotSet() ? null : this.UserMessage.Text,
                this.UserHide.Checked,
                this.UserSortOrder.Text.ToType<byte>());
        }
        else
        {
            // test if there is specified user name/id
            if (this.SelectedUserID.Value.IsNotSet())
            {
                // no username, nor userID specified
                this.PageBoardContext.Notify(
                    this.GetText("ADMIN_EDITMEDAL", "MSG_VALID_USER"),
                    MessageTypes.warning);

                this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                    "openModalJs",
                    JavaScriptBlocks.OpenModalJs("UserEditDialog"));

                return;
            }

            var userId = this.SelectedUserID.Value.ToType<int>();

            this.GetRepository<UserMedal>().SaveNew(
                userId,
                this.MedalId,
                this.UserMessage.Text.IsNotSet() ? null : this.UserMessage.Text,
                this.UserHide.Checked,
                this.UserSortOrder.Text.ToType<byte>());

            if (this.PageBoardContext.BoardSettings.EmailUserOnMedalAward)
            {
                this.Get<ISendNotification>().ToUserWithNewMedal(userId, this.MedalName);
            }
        }

        // clear cache...
        // remove user from cache...
        this.Get<IDataCache>().Remove(string.Format(Constants.Cache.UserMedals, this.UserId));

        // re-bind data
        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditMedal, new { medalid = this.MedalId });
    }
}