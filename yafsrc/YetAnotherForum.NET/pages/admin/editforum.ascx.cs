/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
    using System.IO;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
   using YAF.Web;
    using YAF.Core;
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
    using YAF.Web.Controls;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Administrative Page for the editing of forum properties.
    /// </summary>
    public partial class editforum : AdminPage
    {
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
            if (sender is DropDownList dropDownList)
            {
                dropDownList.DataSource = this.GetRepository<AccessMask>().GetByBoardId();
                dropDownList.DataValueField = "ID";
                dropDownList.DataTextField = "Name";
            }
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
                    YafForumInfo.GetURLToContent("images/spacer.gif"); // use spacer.gif for Description Entry
                dr["Description"] = this.GetText("COMMON", "NONE");
                dt.Rows.Add(dr);

                var dir = new DirectoryInfo(
                    this.Request.MapPath($"{YafForumInfo.ForumServerFileRoot}{YafBoardFolders.Current.Forums}"));
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
                                    $"{YafForumInfo.ForumClientFileRoot}{YafBoardFolders.Current.Forums}/{file.Name}";
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
        /// The get query string as int.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="int?"/>.
        /// </returns>
        protected int? GetQueryStringAsInt([NotNull] string name)
        {
            int value;

            if (this.Request.QueryString.GetFirstOrDefault(name) != null && int.TryParse(
                    this.Request.QueryString.GetFirstOrDefault(name),
                    out value))
            {
                return value;
            }

            return null;
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
            this.Cancel.Click += this.Cancel_Click;
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

            this.ModerateAllPosts.Text = this.GetText("MODERATE_ALL_POSTS");

            // Populate Forum Images Table
            this.CreateImagesDataTable();

            this.BindData();

            var forumId = this.GetQueryStringAsInt("fa") ?? this.GetQueryStringAsInt("copy");

            if (!forumId.HasValue)
            {

                this.LocalizedLabel1.LocalizedTag = this.LocalizedLabel2.LocalizedTag = "NEW_FORUM";

                var sortOrder = 1;

                try
                {
                    // Currently creating a New Forum, and auto fill the Forum Sort Order + 1
                    var forum = this.GetRepository<Types.Models.Forum>().List(this.PageContext.PageBoardID, null)
                        .OrderByDescending(a => a.SortOrder).FirstOrDefault();

                    sortOrder = forum.SortOrder + sortOrder;
                }
                catch
                {
                    sortOrder = 1;
                }

                this.SortOrder.Text = sortOrder.ToString();

                return;

            }

            var dt = this.GetRepository<Types.Models.Forum>().List(this.PageContext.PageBoardID, forumId);

            var row = dt.FirstOrDefault();
            this.Name.Text = row.Name;
            this.Description.Text = row.Description;
            this.SortOrder.Text = row.SortOrder.ToString();
            this.HideNoAccess.Checked = row.ForumFlags.IsHidden;
            this.Locked.Checked = row.ForumFlags.IsLocked;
            this.IsTest.Checked = row.ForumFlags.IsTest;
            this.ForumNameTitle.Text = this.Label1.Text = this.Name.Text;
            this.Moderated.Checked = row.ForumFlags.IsModerated;

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


            this.NewGroupRow.Visible = false;
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("TEAM", "FORUMS"), YafBuildLink.GetLink(ForumPages.admin_forums));
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
            try
            {
                var list = (DropDownList)sender;
                list.Items.FindByValue(list.Attributes["value"]).Selected = true;
            }
            catch (Exception)
            {
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
            var forumId = this.GetQueryStringAsInt("fa") ?? this.GetQueryStringAsInt("copy");

            this.CategoryList.DataSource = this.GetRepository<Category>().List();
            this.CategoryList.DataBind();

            if (forumId.HasValue)
            {
                this.AccessList.DataSource = this.GetRepository<ForumAccess>().GetForumAccessList(forumId.Value);
                this.AccessList.DataBind();
            }

            // Load forum's combo
            this.BindParentList();

            // Load forum's themes
            var listheader = new ListItem
                                 {
                                     Text = this.GetText("ADMIN_EDITFORUM", "CHOOSE_THEME"), Value = string.Empty
                                 };

            this.AccessMaskID.DataBind();

            this.ThemeList.DataSource = StaticDataHelper.Themes();
            this.ThemeList.DataBind();
            this.ThemeList.Items.Insert(0, listheader);
        }

        /// <summary>
        /// Binds the parent list.
        /// </summary>
        private void BindParentList()
        {
            this.ParentList.DataSource = this.GetRepository<Types.Models.Forum>().ListAllFromCatAsDataTable(
                this.PageContext.PageBoardID,
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
        private void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_forums);
        }

        /// <summary>
        /// Clears the caches.
        /// </summary>
        private void ClearCaches()
        {
            // clear moderatorss cache
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

            short sortOrder;

            if (!ValidationHelper.IsValidPosShort(this.SortOrder.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITFORUM", "MSG_POSITIVE_VALUE"),
                    MessageTypes.warning);
                return;
            }

            if (!short.TryParse(this.SortOrder.Text.Trim(), out sortOrder))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITFORUM", "MSG_NUMBER"), MessageTypes.warning);
                return;
            }

            if (this.remoteurl.Text.IsSet())
            {
                /*
                // add http:// by default
                if (!Regex.IsMatch(this.remoteurl.Text.Trim(), @"^(http|https|ftp|ftps|git|svn|news)\://.*"))
                {
                    this.remoteurl.Text = "http://" + this.remoteurl.Text.Trim();
                }

                if (!ValidationHelper.IsValidURL(this.remoteurl.Text))
                {
                    this.PageContext.AddLoadMessage(
                        this.GetText("ADMIN_EDITFORUM", "MSG_INVALID_URL"),
                        MessageTypes.warning);
                    return;
                }*/
            }

            // Forum
            // vzrus: it's stored in the DB as int
            var forumId = this.GetQueryStringAsInt("fa");
            var forumCopyId = this.GetQueryStringAsInt("copy");

            int? parentId = null;

            if (this.ParentList.SelectedValue.Length > 0)
            {
                parentId = this.ParentList.SelectedValue.ToType<int>();
            }

            // parent selection check.
            if (parentId != null && parentId.ToString() == this.Request.QueryString.GetFirstOrDefault("fa"))
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITFORUM", "MSG_PARENT_SELF"));
                return;
            }

            // The picked forum cannot be a child forum as it's a parent
            // If we update a forum ForumID > 0 
            if (forumId.HasValue && parentId != null)
            {
                var dependency = this.GetRepository<Types.Models.Forum>().SaveParentsChecker(forumId.Value, parentId.Value);
                if (dependency > 0)
                {
                    this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITFORUM", "MSG_CHILD_PARENT"));
                    return;
                }
            }

            // inital access mask
            if (!forumId.HasValue && !forumCopyId.HasValue)
            {
                if (this.AccessMaskID.SelectedValue.Length == 0)
                {
                    this.PageContext.AddLoadMessage(
                        this.GetText("ADMIN_EDITFORUM", "MSG_INITAL_MASK"),
                        MessageTypes.warning);
                    return;
                }
            }

            // duplicate name checking...
            if (!forumId.HasValue)
            {
                var forumList = this.GetRepository<Types.Models.Forum>().List(this.PageContext.PageBoardID, null);

                if (forumList.Any() && !this.Get<YafBoardSettings>().AllowForumsWithSameName
                                    && forumList.Any(dr => dr.Name == this.Name.Text.Trim()))
                {
                    this.PageContext.AddLoadMessage(
                        this.GetText("ADMIN_EDITFORUM", "MSG_FORUMNAME_EXISTS"),
                        MessageTypes.warning);
                    return;
                }
            }

            object themeUrl = null;

            if (this.ThemeList.SelectedValue.Length > 0)
            {
                themeUrl = this.ThemeList.SelectedValue;
            }

            var newForumId = this.GetRepository<Types.Models.Forum>().Save(
                forumId,
                this.CategoryList.SelectedValue,
                parentId,
                this.Name.Text.Trim(),
                this.Description.Text.Trim(),
                sortOrder,
                this.Locked.Checked,
                this.HideNoAccess.Checked,
                this.IsTest.Checked,
                this.Moderated.Checked,
                this.ModerateAllPosts.Checked ? null : this.ModeratedPostCount.Text,
                this.ModerateNewTopicOnly.Checked,
                this.AccessMaskID.SelectedValue,
                IsNull(this.remoteurl.Text),
                themeUrl,
                this.ForumImages.SelectedIndex > 0 ? this.ForumImages.SelectedValue.Trim() : null,
                this.Styles.Text,
                false);

            // empty out access table(s)
            this.GetRepository<Active>().DeleteAll();
            this.GetRepository<ActiveAccess>().DeleteAll();

            // Access
            if (forumId.HasValue || forumCopyId.HasValue)
            {
                this.AccessList.Items.OfType<RepeaterItem>().ForEach(
                    item =>
                        {
                            var groupId = int.Parse(item.FindControlAs<Label>("GroupID").Text);

                            this.GetRepository<ForumAccess>().Save(
                                newForumId.ToType<int>(),
                                groupId,
                                item.FindControlAs<DropDownList>("AccessmaskID").SelectedValue.ToType<int>());
                        });
            }

            this.ClearCaches();

            if (forumId.HasValue)
            {
                YafBuildLink.Redirect(ForumPages.admin_forums);
            }
            else
            {
                YafBuildLink.Redirect(ForumPages.admin_editforum, "fa={0}", newForumId);
            }
        }

        #endregion
    }
}