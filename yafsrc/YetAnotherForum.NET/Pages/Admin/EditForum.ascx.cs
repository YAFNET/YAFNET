/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Administrative Page for the editing of forum properties.
    /// </summary>
    public partial class EditForum : AdminPage
    {
        /// <summary>
        /// The access mask list.
        /// </summary>
        private IList<AccessMask> accessMaskList;

        #region Public Methods

        /// <summary>
        /// Handles the Change event of the Category control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        public void CategoryChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.BindParentList();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the AccessMaskID event of the BindData control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected void BindDataAccessMaskId([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!(sender is DropDownList dropDownList))
            {
                return;
            }

            dropDownList.DataSource = this.accessMaskList;
            dropDownList.DataValueField = "ID";
            dropDownList.DataTextField = "Name";
        }

        /// <summary>
        /// The create images data table.
        /// </summary>
        protected void CreateImagesDataTable()
        {
            using (var dt = new DataTable("Files"))
            {
                dt.Columns.Add("FileID", typeof(long));
                dt.Columns.Add("FileName", typeof(string));
                dt.Columns.Add("Description", typeof(string));
                var dr = dt.NewRow();
                dr["FileID"] = 0;
                dr["FileName"] =
                    BoardInfo.GetURLToContent("images/spacer.gif"); // use spacer.gif for Description Entry
                dr["Description"] = this.GetText("COMMON", "NONE");
                dt.Rows.Add(dr);

                var dir = new DirectoryInfo(
                    this.Get<HttpRequestBase>().MapPath($"{BoardInfo.ForumServerFileRoot}{BoardFolders.Current.Forums}"));
                if (dir.Exists)
                {
                    var files = dir.GetFiles("*.*");
                    long fileId = 1;

                    var filesList = from file in files
                                    let sExt = file.Extension.ToLower()
                                    where sExt == ".png" || sExt == ".gif" || sExt == ".jpg"
                                    select file;

                    filesList.ForEach(
                        file =>
                            {
                                dr = dt.NewRow();
                                dr["FileID"] = fileId++;
                                dr["FileName"] =
                                    $"{BoardInfo.ForumClientFileRoot}{BoardFolders.Current.Forums}/{file.Name}";
                                dr["Description"] = file.Name;
                                dt.Rows.Add(dr);
                            });
                }

                this.ForumImages.DataSource = dt;
                this.ForumImages.DataValueField = "FileName";
                this.ForumImages.DataTextField = "Description";
                this.ForumImages.DataBind();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.CategoryList.AutoPostBack = true;
            this.Save.Click += this.SaveClick;
            this.Cancel.Click += this.CancelClick;
            base.OnInit(e);
        }

        /// <summary>
        /// Registers the needed Java Scripts
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.accessMaskList = this.GetRepository<AccessMask>().GetByBoardId();

            this.ModerateAllPosts.Text = this.GetText("MODERATE_ALL_POSTS");

            // Populate Forum Images Table
            this.CreateImagesDataTable();

            this.BindData();

            var forumId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("fa")
                          ?? this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("copy");

            if (!this.Get<HttpRequestBase>().QueryString.Exists("fa")
                && this.Get<HttpRequestBase>().QueryString.Exists("copy") || !forumId.HasValue)
            {
                this.LocalizedLabel1.LocalizedTag = "NEW_FORUM";

                this.IconHeader.Text = this.GetText("NEW_FORUM");

                var sortOrder = 1;

                try
                {
                    // Currently creating a New Forum, and auto fill the Forum Sort Order + 1
                    var forum = this.GetRepository<Forum>().List(this.PageContext.PageBoardID, null)
                        .OrderByDescending(a => a.SortOrder).FirstOrDefault();

                    sortOrder = forum.SortOrder + sortOrder;
                }
                catch
                {
                    sortOrder = 1;
                }

                this.SortOrder.Text = sortOrder.ToString();

                if (!forumId.HasValue)
                {
                    return;
                }
            }

            var row = this.GetRepository<Forum>().GetById(forumId.Value);

            this.Name.Text = row.Name;
            this.Description.Text = row.Description;
            this.SortOrder.Text = row.SortOrder.ToString();
            this.HideNoAccess.Checked = row.ForumFlags.IsHidden;
            this.Locked.Checked = row.ForumFlags.IsLocked;
            this.IsTest.Checked = row.ForumFlags.IsTest;
            this.Moderated.Checked = row.ForumFlags.IsModerated;

            this.IconHeader.Text = $"{this.GetText("ADMIN_EDITFORUM", "HEADER1")} <strong>{this.Name.Text}</strong>";

            this.ModeratedPostCountRow.Visible = this.Moderated.Checked;
            this.ModerateNewTopicOnlyRow.Visible = this.Moderated.Checked;

            if (!row.ModeratedPostCount.HasValue)
            {
                this.ModerateAllPosts.Checked = true;
            }
            else
            {
                this.ModerateAllPosts.Checked = false;
                this.ModeratedPostCount.Visible = true;
                this.ModeratedPostCount.Text = row.ModeratedPostCount.Value.ToString();
            }

            this.ModerateNewTopicOnly.Checked = row.IsModeratedNewTopicOnly;

            this.Styles.Text = row.Styles;

            this.CategoryList.SelectedValue = row.CategoryID.ToString();

            var item = this.ForumImages.Items.FindByText(row.ImageURL);
            if (item != null)
            {
                item.Selected = true;
            }

            // populate parent forums list with forums according to selected category
            this.BindParentList();

            if (row.ParentID.HasValue)
            {
                this.ParentList.SelectedValue = row.ParentID.ToString();
            }

            if (row.ThemeURL.IsSet())
            {
                this.ThemeList.SelectedValue = row.ThemeURL;
            }

            this.remoteurl.Text = row.RemoteURL;
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.Admin_Admin));

            this.PageLinks.AddLink(this.GetText("ADMINMENU", "ADMIN_FORUMS"), BuildLink.GetLink(ForumPages.Admin_Forums));
            this.PageLinks.AddLink(this.GetText("ADMIN_EDITFORUM", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("TEAM", "FORUMS")} - {this.GetText("ADMIN_EDITFORUM", "TITLE")}";
        }

        /// <summary>
        /// Sets the index of the drop down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected void SetDropDownIndex([NotNull] object sender, [NotNull] EventArgs e)
        {
            var list = (DropDownList)sender;

            try
            {
                list.Items.FindByValue(list.Attributes["value"]).Selected = true;
            }
            catch (Exception)
            {
                var item = list.Items.FindByText("Member Access");

                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }

        /// <summary>
        /// Sets the Visibility of the ModeratedPostCount Row
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected void ModeratedCheckedChanged(object sender, EventArgs e)
        {
            this.ModeratedPostCountRow.Visible = this.Moderated.Checked;
            this.ModerateNewTopicOnlyRow.Visible = this.Moderated.Checked;
        }

        /// <summary>
        /// Sets the Visibility of the ModeratedPostCount TextBox
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected void ModerateAllPostsCheckedChanged(object sender, EventArgs e)
        {
            this.ModeratedPostCount.Visible = !this.ModerateAllPosts.Checked;
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            var forumId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("fa")
                          ?? this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("copy");

            this.CategoryList.DataSource = this.GetRepository<Category>().List();
            this.CategoryList.DataBind();

            if (forumId.HasValue)
            {
                this.AccessList.DataSource = this.GetRepository<ForumAccess>().GetForumAccessList(forumId.Value).Select(
                    i => new { GroupID = i.Item2.ID, GroupName = i.Item2.Name, i.Item1.AccessMaskID });
                this.AccessList.DataBind();
            }
            else
            {
                this.AccessList.DataSource = BoardContext.Current.GetRepository<Group>().GetByBoardId()
                    .Select(i => new { GroupID = i.ID, GroupName = i.Name, AccessMaskID = 0 });
                this.AccessList.DataBind();
            }

            // Load forum's combo
            this.BindParentList();

            // Load forum's themes
            var listItem = new ListItem
                                 {
                                     Text = this.GetText("ADMIN_EDITFORUM", "CHOOSE_THEME"), Value = string.Empty
                                 };

            this.ThemeList.DataSource = StaticDataHelper.Themes();
            this.ThemeList.DataBind();
            this.ThemeList.Items.Insert(0, listItem);
        }

        /// <summary>
        /// Binds the parent list.
        /// </summary>
        private void BindParentList()
        {
            this.ParentList.DataSource = this.GetRepository<Forum>().ListAllFromCategory(
                this.CategoryList.SelectedValue.ToType<int>());

            this.ParentList.DataValueField = "ForumID";
            this.ParentList.DataTextField = "Title";

            this.ParentList.DataBind();
        }

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        private void CancelClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Admin_Forums);
        }

        /// <summary>
        /// Clears the caches.
        /// </summary>
        private void ClearCaches()
        {
            // clear moderators cache
            this.Get<IDataCache>().Remove(Constants.Cache.ForumModerators);

            // clear category cache...
            this.Get<IDataCache>().Remove(Constants.Cache.ForumCategory);
        }

        /// <summary>
        /// Handles the Click event of the Save control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        private void SaveClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.CategoryList.SelectedValue.Trim().Length == 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITFORUM", "MSG_CATEGORY"), MessageTypes.warning);
                return;
            }

            if (this.Name.Text.Trim().Length == 0)
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITFORUM", "MSG_NAME_FORUM"),
                    MessageTypes.warning);
                return;
            }

            if (this.SortOrder.Text.Trim().Length == 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITFORUM", "MSG_VALUE"), MessageTypes.warning);
                return;
            }

            if (!ValidationHelper.IsValidPosShort(this.SortOrder.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITFORUM", "MSG_POSITIVE_VALUE"),
                    MessageTypes.warning);
                return;
            }

            if (!short.TryParse(this.SortOrder.Text.Trim(), out var sortOrder))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITFORUM", "MSG_NUMBER"), MessageTypes.warning);
                return;
            }

            // Forum
            // vzrus: it's stored in the DB as int
            var forumId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("fa");
            var forumCopyId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("copy");

            int? parentId = null;

            if (this.ParentList.SelectedValue.Length > 0)
            {
                parentId = this.ParentList.SelectedValue.ToType<int>();
            }

            // parent selection check.
            if (parentId != null && parentId.ToString() == this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("fa"))
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITFORUM", "MSG_PARENT_SELF"),
                    MessageTypes.warning);
                return;
            }

            // The picked forum cannot be a child forum as it's a parent
            // If we update a forum ForumID > 0 
            if (forumId.HasValue && parentId != null)
            {
                var dependency = this.GetRepository<Forum>()
                    .SaveParentsChecker(forumId.Value, parentId.Value);
                if (dependency > 0)
                {
                    this.PageContext.AddLoadMessage(
                        this.GetText("ADMIN_EDITFORUM", "MSG_CHILD_PARENT"),
                        MessageTypes.warning);
                    return;
                }
            }

            // duplicate name checking...
            if (!forumId.HasValue)
            {
                var forumList = this.GetRepository<Forum>().Get(f => f.Name == this.Name.Text.Trim());

                if (forumList.Any() && !this.Get<BoardSettings>().AllowForumsWithSameName)
                {
                    this.PageContext.AddLoadMessage(
                        this.GetText("ADMIN_EDITFORUM", "MSG_FORUMNAME_EXISTS"),
                        MessageTypes.warning);
                    return;
                }
            }

            var themeUrl = string.Empty;

            if (this.ThemeList.SelectedValue.Length > 0)
            {
                themeUrl = this.ThemeList.SelectedValue;
            }

            int? moderatedPostCount = null;

            if (this.ModerateAllPosts.Checked)
            {
                moderatedPostCount = this.ModeratedPostCount.Text.ToType<int>();
            }

            // empty out access table(s)
            this.GetRepository<Active>().DeleteAll();
            this.GetRepository<ActiveAccess>().DeleteAll();

            var newForumId = this.GetRepository<Forum>().Save(
                forumId,
                this.CategoryList.SelectedValue.ToType<int>(),
                parentId,
                this.Name.Text.Trim(),
                this.Description.Text.Trim(),
                sortOrder.ToType<int>(),
                this.Locked.Checked,
                this.HideNoAccess.Checked,
                this.IsTest.Checked,
                this.Moderated.Checked,
                moderatedPostCount,
                this.ModerateNewTopicOnly.Checked,
                this.remoteurl.Text,
                themeUrl,
                this.ForumImages.SelectedIndex > 0 ? this.ForumImages.SelectedItem.Text : string.Empty,
                this.Styles.Text);

            // Access
            if (forumId.HasValue || forumCopyId.HasValue)
            {
                this.AccessList.Items.OfType<RepeaterItem>().ForEach(
                    item =>
                        {
                            var groupId = int.Parse(item.FindControlAs<HiddenField>("GroupID").Value);

                            this.GetRepository<ForumAccess>().Save(
                                newForumId.ToType<int>(),
                                groupId,
                                item.FindControlAs<DropDownList>("AccessmaskID").SelectedValue.ToType<int>());
                        });
            }
            else
            {
                this.AccessList.Items.OfType<RepeaterItem>().ForEach(
                    item =>
                        {
                            var groupId = int.Parse(item.FindControlAs<HiddenField>("GroupID").Value);

                            this.GetRepository<ForumAccess>().Create(
                                newForumId.ToType<int>(),
                                groupId,
                                item.FindControlAs<DropDownList>("AccessmaskID").SelectedValue.ToType<int>());
                        });
            }

            this.ClearCaches();

            BuildLink.Redirect(ForumPages.Admin_Forums);
        }

        #endregion
    }
}