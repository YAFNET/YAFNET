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
/// The User Medal Group Add/Edit Dialog.
/// </summary>
public partial class UserMedalEdit : BaseUserControl
{
    #region Methods

    /// <summary>
    /// Gets or sets the banned identifier.
    /// </summary>
    /// <value>
    /// The banned identifier.
    /// </value>
    public int? MedalId
    {
        get => this.ViewState["MedalId"].ToType<int?>();

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
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Binds the data.
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="medalId">
    /// The medal identifier.
    /// </param>
    public void BindData(int? userId, int? medalId)
    {
        this.MedalId = medalId;
        this.UserId = userId;

        // clear controls
        this.UserID.Text = null;
        this.UserName.Text = null;
        this.UserNameList.Items.Clear();
        this.UserMessage.Text = null;
        this.UserHide.Checked = false;
        this.UserSortOrder.Text = "0";

        // set controls visibility and availability
        this.UserName.Enabled = true;
        this.UserName.Visible = true;
        this.UserNameList.Visible = false;
        this.FindUsers.Visible = true;
        this.Clear.Visible = false;

        // focus on save button
        this.AddUserSave.Focus();

        if (this.UserId.HasValue)
        {
            // Edit
            // load user-medal to the controls
            var row = this.GetRepository<UserMedal>().List(this.UserId, this.MedalId.Value).FirstOrDefault();

            // tweak it for editing
            this.UserMedalEditTitle.Text = this.GetText("ADMIN_EDITMEDAL", "EDIT_MEDAL_USER");
            this.UserName.Enabled = false;
            this.FindUsers.Visible = false;

            // load data to controls
            this.UserID.Text = row.Item3.ID.ToString();
            this.UserName.Text = row.Item3.Name;
            this.UserMessage.Text = row.Item2.Message.IsSet() ? row.Item2.Message : row.Item1.Message;
            this.UserSortOrder.Text = row.Item2.SortOrder.ToString();
            this.UserHide.Checked = row.Item2.Hide;
            this.Name = row.Item1.Name;

            this.UserMedalEditTitle.Text = this.Name;
        }
        else
        {
            // Add
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
    }

    /// <summary>
    /// Handles clear button click event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void ClearClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        // clear drop down
        this.UserNameList.Items.Clear();

        // hide it and show empty UserName text box
        this.UserNameList.Visible = false;
        this.UserName.Text = null;
        this.UserName.Visible = true;
        this.UserID.Text = null;

        // show find users and all users (if user is admin)
        this.FindUsers.Visible = true;

        // clear button is not necessary now
        this.Clear.Visible = false;

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            "openModalJs",
            JavaScriptBlocks.OpenModalJs("UserEditDialog"));
    }

    /// <summary>
    /// Handles find users button click event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void FindUsersClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        // try to find users by user name
        var users = this.GetRepository<User>().Get(
            u => u.BoardID == this.PageBoardContext.PageBoardID && (u.Flags & 2) == 2 && (u.Flags & 4) != 4 &&
                 u.Name.Contains(this.UserName.Text) || u.DisplayName.Contains(this.UserName.Text));

        if (!users.Any())
        {
            return;
        }

        // we found a user(s)
        this.UserNameList.DataSource = users;
        this.UserNameList.DataValueField = "ID";
        this.UserNameList.DataTextField = "Name";
        this.UserNameList.DataBind();

        // hide To text box and show To drop down
        this.UserNameList.Visible = true;
        this.UserName.Visible = false;

        // find is no more needed
        this.FindUsers.Visible = false;

        // we need clear button displayed now
        this.Clear.Visible = true;

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            "openModalJs",
            JavaScriptBlocks.OpenModalJs("UserEditDialog"));
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

        // test if there is specified user name/id
        if (this.UserNameList.SelectedValue.IsNotSet()
            && this.UserName.Text.IsNotSet())
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

        if (this.UserNameList.SelectedValue.IsNotSet() && this.UserID.Text.IsNotSet())
        {
            // only username is specified, we must find id for it
            var users = this.GetRepository<User>().Get(
                u => u.BoardID == this.PageBoardContext.PageBoardID && (u.Flags & 2) == 2 && (u.Flags & 4) != 4 &&
                     u.Name.Contains(this.UserName.Text) || u.DisplayName.Contains(this.UserName.Text));

            if (users.Count > 1)
            {
                // more than one user is available for this username
                this.PageBoardContext.Notify(
                    this.GetText("ADMIN_EDITMEDAL", "MSG_AMBIGOUS_USER"),
                    MessageTypes.warning);

                this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                    "openModalJs",
                    JavaScriptBlocks.OpenModalJs("UserEditDialog"));
                return;
            }

            if (!users.Any())
            {
                // no user found
                this.PageBoardContext.Notify(
                    this.GetText("ADMIN_EDITMEDAL", "MSG_VALID_USER"),
                    MessageTypes.warning);

                this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                    "openModalJs",
                    JavaScriptBlocks.OpenModalJs("UserEditDialog"));
                return;
            }

            // save id to the control
            this.UserID.Text = users.First().ID.ToString();
        }
        else if (this.UserID.Text.IsNotSet())
        {
            // user is selected in dropdown, we must get id to UserID control
            this.UserID.Text = this.UserNameList.SelectedValue;
        }

        if (this.UserId.HasValue)
        {
            // save user, if there is no message specified, pass null
            this.GetRepository<UserMedal>().Save(
                this.UserID.Text.ToType<int>(),
                this.MedalId.Value,
                this.UserMessage.Text.IsNotSet() ? null : this.UserMessage.Text,
                this.UserHide.Checked,
                this.UserSortOrder.Text.ToType<byte>());
        }
        else
        {
            this.GetRepository<UserMedal>().SaveNew(
                this.UserID.Text.ToType<int>(),
                this.MedalId.Value,
                this.UserMessage.Text.IsNotSet() ? null : this.UserMessage.Text,
                this.UserHide.Checked,
                this.UserSortOrder.Text.ToType<byte>());
        }

        if (this.PageBoardContext.BoardSettings.EmailUserOnMedalAward)
        {
            this.Get<ISendNotification>().ToUserWithNewMedal(this.UserID.Text.ToType<int>(), this.Name);
        }

        // clear cache...
        // remove user from cache...
        this.Get<IDataCache>().Remove(string.Format(Constants.Cache.UserMedals, this.UserId));

        // re-bind data
        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditMedal, new { medalid = this.MedalId.Value });
    }

    #endregion
}