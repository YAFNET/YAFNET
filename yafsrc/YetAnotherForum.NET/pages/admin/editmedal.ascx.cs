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
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    using Image = System.Drawing.Image;

    #endregion

    /// <summary>
    /// Administration Page to Add/Edit Medals
    /// </summary>
    public partial class editmedal : AdminPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "editmedal" /> class. 
        ///   Default constructor.
        /// </summary>
        public editmedal()
            : base("ADMIN_EDITMEDAL")
        {
        }

        #endregion

        /// <summary>
        /// Gets the current medal identifier.
        /// </summary>
        /// <value>
        /// The current medal identifier.
        /// </value>
        protected int? CurrentMedalId => this.Request.QueryString.GetFirstOrDefault("medalid") != null
                                             ? this.Request.QueryString.GetFirstOrDefault("medalid").ToType<int?>()
                                             : null;

        #region Methods

        /// <summary>
        /// Hides group add/edit controls.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void AddGroupCancelClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // set visibility
            this.AddGroupRow.Visible = true;
            this.AddGroupPanel.Visible = false;
        }

        /// <summary>
        /// Handles click on save group button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void AddGroupSaveClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // test if there is specified unsername/user id
            if (this.AvailableGroupList.SelectedIndex < 0)
            {
                // no group selected
                this.PageContext.AddLoadMessage("Please select user group!", MessageTypes.warning);
                return;
            }

            // save group, if there is no message specified, pass null
            LegacyDb.group_medal_save(
              this.AvailableGroupList.SelectedValue,
              this.CurrentMedalId,
              this.GroupMessage.Text.IsNotSet() ? null : this.GroupMessage.Text,
              this.GroupHide.Checked,
              this.GroupOnlyRibbon.Checked,
              this.GroupSortOrder.Text);

            // disable/hide edit controls
            this.AddGroupCancelClick(sender, e);

            // re-bind data
            this.BindData();
        }

        /// <summary>
        /// Handles click on add group button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Shows user-medal adding/editing controls.
        /// </remarks>
        protected void AddGroupClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // set title
            this.GroupMedalEditTitle.Text = this.GetText("ADMIN_EDITMEDAL", "ADD_TOGROUP");

            // clear controls
            this.AvailableGroupList.SelectedIndex = -1;
            this.GroupMessage.Text = null;
            this.GroupOnlyRibbon.Checked = false;
            this.GroupHide.Checked = false;
            this.GroupSortOrder.Text = this.SortOrder.Text;

            // set controls visibility and availability
            this.AvailableGroupList.Enabled = true;

            // show editing controls and hide row with add user button
            this.AddGroupRow.Visible = false;
            this.AddGroupPanel.Visible = true;

            // focus on save button
            this.AddGroupSave.Focus();
        }

        /// <summary>
        /// Hides user add/edit controls.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void AddUserCancelClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // set visibility
            this.AddUserRow.Visible = true;
            this.AddUserPanel.Visible = false;
        }

        /// <summary>
        /// Handles click on save user button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void AddUserSaveClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // test if there is specified unsername/user id
            if (this.UserID.Text.IsNotSet() && this.UserNameList.SelectedValue.IsNotSet() && this.UserName.Text.IsNotSet())
            {
                // no username, nor userID specified
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITMEDAL", "MSG_VALID_USER"), MessageTypes.warning);
                return;
            }

            if (this.UserNameList.SelectedValue.IsNotSet() && this.UserID.Text.IsNotSet())
            {
                // only username is specified, we must find id for it
                var users = this.GetRepository<User>()
                    .FindUserTyped(filter: true, userName: this.UserName.Text, displayName: this.UserName.Text);

                if (users.Count > 1)
                {
                    // more than one user is avalilable for this username
                    this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITMEDAL", "MSG_AMBIGOUS_USER"), MessageTypes.warning);
                    return;
                }

                if (!users.Any())
                {
                    // no user found
                    this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITMEDAL", "MSG_VALID_USER"), MessageTypes.warning);
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

            // save user, if there is no message specified, pass null
            LegacyDb.user_medal_save(
              this.UserID.Text,
              this.CurrentMedalId,
              this.UserMessage.Text.IsNotSet() ? null : this.UserMessage.Text,
              this.UserHide.Checked,
              this.UserOnlyRibbon.Checked,
              this.UserSortOrder.Text,
              null);

            if (this.Get<YafBoardSettings>().EmailUserOnMedalAward)
            {
                this.Get<ISendNotification>().ToUserWithNewMedal(
                    this.UserID.Text.ToType<int>(), this.Name.Text);
            }

            // disable/hide edit controls
            this.AddUserCancelClick(sender, e);

            // clear cache...
            this.RemoveUserFromCache(this.UserID.Text.ToType<int>());

            // re-bind data
            this.BindData();
        }

        /// <summary>
        /// Handles click on add user button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Shows user-medal adding/editing controls.
        /// </remarks>
        protected void AddUserClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // set title
            this.UserMedalEditTitle.Text = this.GetText("ADMIN_EDITMEDAL", "ADD_TOUSER");

            // clear controls
            this.UserID.Text = null;
            this.UserName.Text = null;
            this.UserNameList.Items.Clear();
            this.UserMessage.Text = null;
            this.UserOnlyRibbon.Checked = false;
            this.UserHide.Checked = false;
            this.UserSortOrder.Text = this.SortOrder.Text;

            // set controls visibility and availability
            this.UserName.Enabled = true;
            this.UserName.Visible = true;
            this.UserNameList.Visible = false;
            this.FindUsers.Visible = true;
            this.Clear.Visible = false;

            // show editing controls and hide row with add user button
            this.AddUserRow.Visible = false;
            this.AddUserPanel.Visible = true;

            // focus on save button
            this.AddUserSave.Focus();

            // disable global save button to prevent confusion
            // this.Save.Enabled = false;
        }

        /// <summary>
        /// Handles click on cancel button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void CancelClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // go back to medals administration
            YafBuildLink.Redirect(ForumPages.admin_medals);
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
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // forum index
            this.PageLinks.AddRoot();

            // administration index
            this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_MEDALS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_medals));

            // current page label (no link)
            this.PageLinks.AddLink(this.GetText("ADMIN_EDITMEDAL", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
               this.GetText("ADMIN_ADMIN", "Administration"),
               this.GetText("ADMIN_MEDALS", "TITLE"),
               this.GetText("ADMIN_EDITMEDAL", "TITLE"));
        }

        /// <summary>
        /// Handles find users button click event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void FindUsersClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            // try to find users by user name
            var users = this.GetRepository<User>()
                    .FindUserTyped(filter: true, userName: this.UserName.Text, displayName: this.UserName.Text);

            if (!users.Any())
            {
                return;
            }

            // we found a user(s)
            this.UserNameList.DataSource = users;
            this.UserNameList.DataValueField = "UserID";
            this.UserNameList.DataTextField = "Name";
            this.UserNameList.DataBind();

            // hide To text box and show To drop down
            this.UserNameList.Visible = true;
            this.UserName.Visible = false;

            // find is no more needed
            this.FindUsers.Visible = false;

            // we need clear button displayed now
            this.Clear.Visible = true;
        }

        /// <summary>
        /// Creates link to group editing admin interface.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The format group link.
        /// </returns>
        protected string FormatGroupLink([NotNull] object data)
        {
            var dr = (DataRowView)data;

            return "<a href=\"{1}\">{0}</a>".FormatWith(
              dr["GroupName"], YafBuildLink.GetLink(ForumPages.admin_editgroup, "i={0}", dr["GroupID"]));
        }

        /// <summary>
        /// Creates link to user editing admin interface.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The format user link.
        /// </returns>
        protected string FormatUserLink([NotNull] object data)
        {
            var dr = (DataRowView)data;

            return "<a href=\"{2}\">{0}({1})</a>".FormatWith(
              this.HtmlEncode(dr["DisplayName"]), this.HtmlEncode(dr["UserName"]), YafBuildLink.GetLink(ForumPages.admin_edituser, "u={0}", dr["UserID"]));
        }

        /// <summary>
        /// Handles click on GroupList repeaters item command link button.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void GroupListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":

                    // load group-medal to the controls
                    using (var dt = LegacyDb.group_medal_list(e.CommandArgument, this.CurrentMedalId))
                    {
                        // prepare editing interface
                        this.AddGroupClick(null, e);

                        // tweak it for editing
                        this.GroupMedalEditTitle.Text = this.GetText("ADMIN_EDITMEDAL", "EDIT_MEDAL_GROUP");
                        this.AvailableGroupList.Enabled = false;

                        // we are intereseted inly in first row
                        var r = dt.Rows[0];

                        // load data to controls
                        this.AvailableGroupList.SelectedIndex = -1;
                        this.AvailableGroupList.Items.FindByValue(r["GroupID"].ToString()).Selected = true;
                        this.GroupMessage.Text = r["MessageEx"].ToString();
                        this.GroupSortOrder.Text = r["SortOrder"].ToString();
                        this.GroupOnlyRibbon.Checked = r["OnlyRibbon"].ToType<bool>();
                        this.GroupHide.Checked = r["Hide"].ToType<bool>();

                        // remove all user medals...
                        this.RemoveMedalsFromCache();
                    }

                    break;
                case "remove":
                    LegacyDb.group_medal_delete(e.CommandArgument, this.CurrentMedalId);

                    // remove all user medals...
                    this.RemoveMedalsFromCache();

                    this.BindData();
                    break;
            }
        }

        /// <summary>
        /// Adds Java Script popup to remove group link button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void GroupRemoveLoad([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("ADMIN_EDITMEDAL", "CONFIRM_REMOVE_GROUP"));
        }

        /// <summary>
        /// Handles page load event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // this needs to be done just once, not during postbacks
            if (!this.IsPostBack)
            {
                // bind data
                this.BindData();
            }

            // set previews
            this.SetPreview(this.MedalImage, this.MedalPreview);
            this.SetPreview(this.RibbonImage, this.RibbonPreview);
            this.SetPreview(this.SmallMedalImage, this.SmallMedalPreview);
            this.SetPreview(this.SmallRibbonImage, this.SmallRibbonPreview);
        }

        /// <summary>
        /// Removals all medals from the cache...
        /// </summary>
        protected void RemoveMedalsFromCache()
        {
            // remove all user medals...
          this.Get<IDataCache>().Remove(k => k.StartsWith(Constants.Cache.UserMedals.FormatWith(string.Empty)));
        }

        /// <summary>
        /// Removes an individual user from the cache...
        /// </summary>
        /// <param name="userId">The user id.</param>
        protected void RemoveUserFromCache(int userId)
        {
            // remove user from cache...
            this.Get<IDataCache>().Remove(Constants.Cache.UserMedals.FormatWith(userId));
        }

        /// <summary>
        /// Handles save button click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.MedalImage.SelectedIndex <= 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITMEDAL", "MSG_IMAGE"), MessageTypes.warning);
                return;
            }

            if (this.SmallMedalImage.SelectedIndex <= 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITMEDAL", "MSG_SMALL_IMAGE"), MessageTypes.warning);
                return;
            }

            if (this.SortOrder.Text.Trim().Length == 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITFORUM", "MSG_VALUE"), MessageTypes.warning);
                return;
            }

            byte sortOrder;

            if (!ValidationHelper.IsValidPosShort(this.SortOrder.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITFORUM", "MSG_POSITIVE_VALUE"), MessageTypes.warning);
                return;
            }

            if (!byte.TryParse(this.SortOrder.Text.Trim(), out sortOrder))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITFORUM", "MSG_CATEGORY"), MessageTypes.warning);
                return;
            }

            // data
            string ribbonUrl = null, smallRibbonUrl = null;
            short? ribbonWidth = null, ribbonHeight = null;
            Size imageSize;
            
            // flags
            var flags = new MedalFlags(0)
                            {
                                ShowMessage = this.ShowMessage.Checked,
                                AllowRibbon = this.AllowRibbon.Checked,
                                AllowReOrdering = this.AllowReOrdering.Checked,
                                AllowHiding = this.AllowHiding.Checked
                            };

            // get medal images
            var imageUrl = this.MedalImage.SelectedValue;
            var smallImageUrl = this.SmallMedalImage.SelectedValue;
            if (this.RibbonImage.SelectedIndex > 0)
            {
                ribbonUrl = this.RibbonImage.SelectedValue;
            }

            if (this.SmallRibbonImage.SelectedIndex > 0)
            {
                smallRibbonUrl = this.SmallRibbonImage.SelectedValue;

                imageSize = this.GetImageSize(smallRibbonUrl);
                ribbonWidth = imageSize.Width.ToType<short>();
                ribbonHeight = imageSize.Height.ToType<short>();
            }

            // get size of small image
            imageSize = this.GetImageSize(smallImageUrl);

            // save medal
            this.GetRepository<Medal>().Save(
                this.CurrentMedalId,
                this.Name.Text,
                this.Description.Text,
                this.Message.Text,
                this.Category.Text,
                imageUrl,
                ribbonUrl,
                smallImageUrl,
                smallRibbonUrl,
                (short)imageSize.Width,
                (short)imageSize.Height,
                ribbonWidth,
                ribbonHeight,
                sortOrder,
                flags.BitValue);

            // go back to medals administration
            YafBuildLink.Redirect(ForumPages.admin_medals);
        }

        /// <summary>
        /// Handles click on UserList repeaters item command link button.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void UserListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":

                    // load user-medal to the controls
                    using (var dt = LegacyDb.user_medal_list(e.CommandArgument, this.CurrentMedalId))
                    {
                        // prepare editing interface
                        this.AddUserClick(null, e);

                        // tweak it for editing
                        this.UserMedalEditTitle.Text = this.GetText("ADMIN_EDITMEDAL", "EDIT_MEDAL_USER");
                        this.UserName.Enabled = false;
                        this.FindUsers.Visible = false;

                        // we are intereseted inly in first row
                        var r = dt.Rows[0];

                        // load data to controls
                        this.UserID.Text = r["UserID"].ToString();
                        this.UserName.Text = r["UserName"].ToString();
                        this.UserMessage.Text = r["MessageEx"].ToString();
                        this.UserSortOrder.Text = r["SortOrder"].ToString();
                        this.UserOnlyRibbon.Checked = r["OnlyRibbon"].ToType<bool>();
                        this.UserHide.Checked = r["Hide"].ToType<bool>();
                    }

                    break;
                case "remove":

                    // delete user-medal
                    LegacyDb.user_medal_delete(e.CommandArgument, this.CurrentMedalId);

                    // clear cache...
                    this.RemoveUserFromCache(this.CurrentMedalId.Value);
                    this.BindData();
                    break;
            }
        }

        /// <summary>
        /// Adds Java Script popup to remove user link button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void UserRemoveLoad([NotNull] object sender, [NotNull] EventArgs e)
        {
            ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_EDITMEDAL", "CONFIRM_REMOVE_USER"));
        }

        /// <summary>
        /// Registers the needed Java Scripts
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            base.OnPreRender(e);
        }

        /// <summary>
        /// Bind data for this control.
        /// </summary>
        private void BindData()
        {
            // load available images from images/medals folder
            using (var dt = new DataTable("Files"))
            {
                // create structure
                dt.Columns.Add("FileID", typeof(long));
                dt.Columns.Add("FileName", typeof(string));
                dt.Columns.Add("Description", typeof(string));

                // add blank row
                var dr = dt.NewRow();
                dr["FileID"] = 0;
                dr["FileName"] = YafForumInfo.GetURLToContent("images/spacer.gif"); // use spacer.gif for Description Entry
                dr["Description"] = this.GetText("ADMIN_EDITMEDAL", "SELECT_IMAGE");
                dt.Rows.Add(dr);

                // add files from medals folder
                var dir =
                  new DirectoryInfo(
                    this.Request.MapPath("{0}{1}".FormatWith(YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Medals)));
                var files = dir.GetFiles("*.*");

                long fileId = 1;

                foreach (var file in from file in files
                                          let sExt = file.Extension.ToLower()
                                          where sExt == ".png" || sExt == ".gif" || sExt == ".jpg"
                                          select file)
                {
                    dr = dt.NewRow();
                    dr["FileID"] = fileId++;
                    dr["FileName"] = file.Name;
                    dr["Description"] = file.Name;
                    dt.Rows.Add(dr);
                }

                // medal image
                this.MedalImage.DataSource = dt;
                this.MedalImage.DataValueField = "FileName";
                this.MedalImage.DataTextField = "Description";

                // ribbon bar image
                this.RibbonImage.DataSource = dt;
                this.RibbonImage.DataValueField = "FileName";
                this.RibbonImage.DataTextField = "Description";

                // small medal image
                this.SmallMedalImage.DataSource = dt;
                this.SmallMedalImage.DataValueField = "FileName";
                this.SmallMedalImage.DataTextField = "Description";

                // small ribbon bar image
                this.SmallRibbonImage.DataSource = dt;
                this.SmallRibbonImage.DataValueField = "FileName";
                this.SmallRibbonImage.DataTextField = "Description";
            }

            // bind data to controls
            this.DataBind();

            // load existing medal if we are editing one
            if (this.CurrentMedalId.HasValue)
            {
                // load users and groups who has been assigned this medal
                this.UserList.DataSource = LegacyDb.user_medal_list(null, this.CurrentMedalId);
                this.UserList.DataBind();
                this.GroupList.DataSource = LegacyDb.group_medal_list(null, this.CurrentMedalId);
                this.GroupList.DataBind();

                // enable adding users/groups
                this.AddUserRow.Visible = true;
                this.AddGroupRow.Visible = true;

                using (var dt = this.GetRepository<Medal>().List(this.CurrentMedalId))
                {
                    // get data row
                    var row = dt.Rows[0];

                    // load flags
                    var flags = new MedalFlags(row["Flags"]);

                    // set controls
                    this.Name.Text = row["Name"].ToString();
                    this.Description.Text = row["Description"].ToString();
                    this.Message.Text = row["Message"].ToString();
                    this.Category.Text = row["Category"].ToString();
                    this.SortOrder.Text = row["SortOrder"].ToString();
                    this.ShowMessage.Checked = flags.ShowMessage;
                    this.AllowRibbon.Checked = flags.AllowRibbon;
                    this.AllowHiding.Checked = flags.AllowHiding;
                    this.AllowReOrdering.Checked = flags.AllowReOrdering;

                    // select images
                    this.SelectImage(this.MedalImage, this.MedalPreview, row["MedalURL"]);
                    this.SelectImage(this.RibbonImage, this.RibbonPreview, row["RibbonURL"]);
                    this.SelectImage(this.SmallMedalImage, this.SmallMedalPreview, row["SmallMedalURL"]);
                    this.SelectImage(this.SmallRibbonImage, this.SmallRibbonPreview, row["SmallRibbonURL"]);
                }

                var groups = this.GetRepository<Group>().List(boardId: this.PageContext.PageBoardID);

                this.AvailableGroupList.DataSource = groups;

                this.AvailableGroupList.DataTextField = "Name";
                this.AvailableGroupList.DataValueField = "ID";

                this.AvailableGroupList.DataBind();
            }
            else
            {
                // set all previews on blank image
                var spacerPath = YafForumInfo.GetURLToContent("images/spacer.gif"); // use spacer.gif for Description Entry

                this.MedalPreview.Src = spacerPath;
                this.RibbonPreview.Src = spacerPath;
                this.SmallMedalPreview.Src = spacerPath;
                this.SmallRibbonPreview.Src = spacerPath;
            }
        }

        /// <summary>
        /// Gets size of image located in medals directory.
        /// </summary>
        /// <param name="filename">
        /// Name of file.
        /// </param>
        /// <returns>
        /// Size of image.
        /// </returns>
        private Size GetImageSize([NotNull] string filename)
        {
            using (
              var img =
                Image.FromFile(
                  this.Server.MapPath(
                    "{0}{1}/{2}".FormatWith(YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Medals, filename))))
            {
                return img.Size;
            }
        }

        /// <summary>
        /// Select image in dropdown list and sets appropriate preview.
        /// </summary>
        /// <param name="list">
        /// DropDownList where to search.
        /// </param>
        /// <param name="preview">
        /// Preview image.
        /// </param>
        /// <param name="imageUrl">
        /// URL to search for.
        /// </param>
        private void SelectImage([NotNull] DropDownList list, [NotNull] HtmlImage preview, [NotNull] object imageUrl)
        {
            this.SelectImage(list, preview, imageUrl.ToString());
        }

        /// <summary>
        /// Select image in dropdown list and sets appropriate preview.
        /// </summary>
        /// <param name="list">
        /// DropDownList where to search.
        /// </param>
        /// <param name="preview">
        /// Preview image.
        /// </param>
        /// <param name="imageUrl">
        /// URL to search for.
        /// </param>
        private void SelectImage([NotNull] DropDownList list, [NotNull] HtmlImage preview, [NotNull] string imageUrl)
        {
            // try to find item in a list
            var item = list.Items.FindByText(imageUrl);

            if (item != null)
            {
                // select found item
                item.Selected = true;

                // set preview image
                preview.Src = "{0}{1}/{2}".FormatWith(
                  YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Medals, imageUrl);
            }
            else
            {
                // if we found nothing, set blank image as preview
                preview.Src = YafForumInfo.GetURLToContent("images/spacer.gif"); // use spacer.gif for Description Entry
            }
        }

        /// <summary>
        /// Sets the Image preview.
        /// </summary>
        /// <param name="imageSelector">DropDownList with image file listed.</param>
        /// <param name="imagePreview">Image for showing preview.</param>
        private void SetPreview([NotNull] WebControl imageSelector, [NotNull] HtmlControl imagePreview)
        {
            // create javascript
            imageSelector.Attributes["onchange"] =
              "getElementById('{2}').src='{0}{1}/' + this.value".FormatWith(
                YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Medals, imagePreview.ClientID);
        }

        #endregion
    }
}