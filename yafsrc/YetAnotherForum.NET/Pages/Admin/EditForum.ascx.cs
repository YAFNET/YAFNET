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

namespace YAF.Pages.Admin;

using System.IO;

using YAF.Types.Objects;
using YAF.Types.Models;

using ListItem = ListItem;

/// <summary>
/// Administrative Page for the editing of forum properties.
/// </summary>
public partial class EditForum : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditForum"/> class. 
    /// </summary>
    public EditForum()
        : base("ADMIN_EDITFORUM", ForumPages.Admin_EditForum)
    {
    }

    /// <summary>
    /// The access mask list.
    /// </summary>
    private IList<AccessMask> accessMaskList;

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
        if (sender is not DropDownList dropDownList)
        {
            return;
        }

        dropDownList.DataSource = this.accessMaskList;
        dropDownList.DataValueField = "ID";
        dropDownList.DataTextField = "Name";
    }

    /// <summary>
    /// Create images list.
    /// </summary>
    protected void CreateImagesList()
    {
        var list = new List<NamedParameter>
                       {
                           new(this.GetText("COMMON", "NONE"), "")
                       };

        var dir = new DirectoryInfo(
            this.Get<HttpRequestBase>().MapPath($"{BoardInfo.ForumServerFileRoot}{this.Get<BoardFolders>().Forums}"));

        if (dir.Exists)
        {
            var files = dir.GetFiles("*.*").ToList();

            list.AddImageFiles(files, this.Get<BoardFolders>().Forums);
        }

        this.ForumImages.PlaceHolder = this.GetText("COMMON", "NONE");
        this.ForumImages.AllowClear = true;

        this.ForumImages.DataSource = list;
        this.ForumImages.DataValueField = "Value";
        this.ForumImages.DataTextField = "Name";
        this.ForumImages.DataBind();
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
        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.FormValidatorJs),
            JavaScriptBlocks.FormValidatorJs(this.Save.ClientID));

        if (this.IsPostBack)
        {
            this.Body.CssClass = "card-body was-validated";

            return;
        }

        this.accessMaskList = this.GetRepository<AccessMask>().GetByBoardId();

        this.ModerateAllPosts.Text = this.GetText("MODERATE_ALL_POSTS");

        // Populate Forum Images
        this.CreateImagesList();

        this.BindData();

        var forumId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("fa")
                      ?? this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("copy");

        if (!this.Get<HttpRequestBase>().QueryString.Exists("fa")
            && this.Get<HttpRequestBase>().QueryString.Exists("copy") || !forumId.HasValue)
        {
            this.IconHeader.Text = this.GetText("NEW_FORUM");

            var sortOrder = 1;

            try
            {
                // Currently creating a New Forum, and auto fill the Forum Sort Order + 1
                var forumCheck = this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID)
                    .OrderByDescending(a => a.Item2.SortOrder).FirstOrDefault();

                sortOrder = forumCheck.Item2.SortOrder + sortOrder;
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

        var forum = this.GetRepository<Forum>().GetById(forumId.Value);

        if (forum == null)
        {
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            return;
        }

        this.Name.Text = forum.Name;
        this.Description.Text = forum.Description;
        this.SortOrder.Text = forum.SortOrder.ToString();
        this.HideNoAccess.Checked = forum.ForumFlags.IsHidden;
        this.Locked.Checked = forum.ForumFlags.IsLocked;
        this.IsTest.Checked = forum.ForumFlags.IsTest;
        this.Moderated.Checked = forum.ForumFlags.IsModerated;

        this.IconHeader.Text = $"{this.GetText("ADMIN_EDITFORUM", "HEADER1")} <strong>{this.Name.Text}</strong>";

        this.ModeratedPostCountRow.Visible = this.Moderated.Checked;
        this.ModerateNewTopicOnlyRow.Visible = this.Moderated.Checked;

        if (!forum.ModeratedPostCount.HasValue)
        {
            this.ModerateAllPosts.Checked = true;
        }
        else
        {
            this.ModerateAllPosts.Checked = false;
            this.ModeratedPostCount.Visible = true;
            this.ModeratedPostCount.Text = forum.ModeratedPostCount.Value.ToString();
        }

        this.ModerateNewTopicOnly.Checked = forum.IsModeratedNewTopicOnly;

        this.Styles.Text = forum.Styles;

        this.CategoryList.SelectedValue = forum.CategoryID.ToString();

        var item = this.ForumImages.Items.FindByText(forum.ImageURL);

        if (item != null)
        {
            this.ForumImages.ClearSelection();
            item.Selected = true;
        }

        // populate parent forums list with forums according to selected category
        this.BindParentList();

        if (forum.ParentID.HasValue)
        {
            this.ParentList.SelectedValue = forum.ParentID.ToString();
        }

        if (forum.ThemeURL.IsSet())
        {
            this.ThemeList.SelectedValue = forum.ThemeURL;
        }

        this.remoteurl.Text = forum.RemoteURL;
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMINMENU", "ADMIN_FORUMS"), this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Forums));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITFORUM", "TITLE"), string.Empty);
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

        if (list.Items.FindByValue(list.Attributes["value"]) != null)
        {
            list.ClearSelection();
            list.Items.FindByValue(list.Attributes["value"]).Selected = true;
        }
        else
        {
            // Load default from board settings
            var item = list.Items.FindByValue(this.PageBoardContext.BoardSettings.ForumDefaultAccessMask.ToString());

            if (item != null)
            {
                list.ClearSelection();

                item.Selected = true;
            }
            else
            {
                // Fallback if default access mask from board settings doesn't exist
                item = list.Items.FindByText("Member Access");

                if (item == null) return;

                list.ClearSelection();

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
        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Forums);
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
            this.AccessList.DataSource = this.PageBoardContext.GetRepository<Group>().GetByBoardId()
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
        var forums = this.GetRepository<Forum>().ListAllFromCategory(this.CategoryList.SelectedValue.ToType<int>());

        forums.Insert(0, new ForumSorted {ForumID = 0, Forum = this.GetText("NONE")});

        this.ParentList.DataSource = forums;

        this.ParentList.DataValueField = "ForumID";
        this.ParentList.DataTextField = "Forum";

        this.ParentList.DataBind();

        this.ParentList.Items.Cast<ListItem>().ForEach(
            item => item.Attributes.Add(
                "data-custom-properties",
                $$"""{ "label": "<i class='fas fa-comments fa-fw text-secondary me-1'></i><span><span>{{item.Text}}" }"""));
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
            this.PageBoardContext.Notify(this.GetText("ADMIN_EDITFORUM", "MSG_CATEGORY"), MessageTypes.warning);
            return;
        }

        // Forum
        var forumId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("fa");
        var forumCopyId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("copy");

        int? parentId = null;

        if (this.ParentList.SelectedIndex > 0)
        {
            parentId = this.ParentList.SelectedValue.ToType<int>();
        }

        // The picked forum cannot be a child forum as it's a parent
        // If we update a forum ForumID > 0 
        if (forumId.HasValue && parentId.HasValue)
        {
            // check if parent and forum is the same
            if (parentId.Value == forumId.Value)
            {
                this.PageBoardContext.Notify(
                    this.GetText("ADMIN_EDITFORUM", "MSG_PARENT_SELF"),
                    MessageTypes.warning);
                return;
            }

            if (this.GetRepository<Forum>()
                .IsParentsChecker(forumId.Value))
            {
                this.PageBoardContext.Notify(
                    this.GetText("ADMIN_EDITFORUM", "MSG_CHILD_PARENT"),
                    MessageTypes.warning);
                return;
            }
        }

        // duplicate name checking...
        if (!forumId.HasValue)
        {
            var forumList = this.GetRepository<Forum>().Get(f => f.Name == this.Name.Text.Trim());

            if (forumList.Any() && !this.PageBoardContext.BoardSettings.AllowForumsWithSameName)
            {
                this.PageBoardContext.Notify(
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

        if (!this.ModerateAllPosts.Checked)
        {
            moderatedPostCount = this.ModeratedPostCount.Text.ToType<int>();
        }

        var newForumId = this.GetRepository<Forum>().Save(
            forumId,
            this.CategoryList.SelectedValue.ToType<int>(),
            parentId,
            this.Name.Text.Trim(),
            this.Description.Text.Trim(),
            this.SortOrder.Text.ToType<short>(),
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

        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Forums);
    }
}