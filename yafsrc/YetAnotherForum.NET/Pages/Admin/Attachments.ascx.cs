/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

using YAF.Web.Controls;
using YAF.Types.Models;

/// <summary>
/// The Admin Manage User Attachments Page.
/// </summary>
public partial class Attachments : AdminPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "Attachments" /> class.
    /// </summary>
    public Attachments()
        : base("ADMIN_ATTACHMENTS", ForumPages.Admin_Attachments)
    {
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // do it only once, not on post-backs
        if (this.IsPostBack)
        {
            return;
        }

        this.PageSize.DataSource = StaticDataHelper.PageEntries();
        this.PageSize.DataTextField = "Name";
        this.PageSize.DataValueField = "Value";
        this.PageSize.DataBind();

        try
        {
            this.PageSize.SelectedValue = this.PageBoardContext.PageUser.PageSize.ToString();
        }
        catch (Exception)
        {
            this.PageSize.SelectedValue = "5";
        }

        // bind data to controls
        this.BindData();
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();

        // administration index second
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_ATTACHMENTS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// The pager top_ page change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void PagerTopPageChange(object sender, EventArgs e)
    {
        // rebind
        this.BindData();
    }

    /// <summary>
    /// The page size on selected index changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PageSizeSelectedIndexChanged(object sender, EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// Handles single record commands in a repeater.
    /// </summary>
    /// <param name="source">The source of the event.</param>
    /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
    protected void ListItemCommand(object source, RepeaterCommandEventArgs e)
    {
        // what command are we serving?
        switch (e.CommandName)
        {
            // delete log entry
            case "delete":

                // with the file of the attachment
                this.GetRepository<Attachment>().Delete(e.CommandArgument.ToType<int>());

                // re-bind controls
                this.BindData();
                break;
        }
    }

    /// <summary>
    /// Gets the preview image.
    /// </summary>
    /// <param name="o">The Data Row object.</param>
    /// <returns>Returns the Preview Image</returns>
    protected string GetPreviewImage(object o)
    {
        var attach = o.ToType<Attachment>();

        var fileName = attach.FileName;
        var isImage = fileName.IsImageName();
        var url =
            $"{BoardInfo.ForumClientFileRoot}resource.ashx?i={attach.ID}&editor=true";

        return isImage
                   ? $"<img src=\"{url}\" alt=\"{fileName}\" title=\"{fileName}\" data-url=\"{url}\" style=\"max-width:30px\" class=\"me-2 img-thumbnail attachments-preview\" />"
                   : "<i class=\"far fa-file-alt attachment-icon me-2\"></i>";
    }

    /// <summary>
    /// Renders the UserLink
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    protected string UserLink(User item)
    {
        var userLink = new UserLink
                           {
                               UserID = item.ID,
                               Suspended = item.Suspended,
                               Style = item.UserStyle,
                               ReplaceName = item.DisplayOrUserName()
                           };

        return userLink.RenderToString();
    }

    /// <summary>
    /// Gets or sets the posts that show the attachments of the page (by the attachment id).
    /// </summary>
    protected Dictionary<int, List<(int MessageId, int TopicId, string TopicName)>> Usages { get; set; } = new();

    /// <summary>
    /// Gets the posts that show an attachment.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns>The posts.</returns>
    protected List<(int MessageId, int TopicId, string TopicName)> GetPosts(int attachmentId)
    {
        return this.Usages.TryGetValue(attachmentId, out var posts)
                   ? posts
                   : new List<(int MessageId, int TopicId, string TopicName)>();
    }

    /// <summary>
    /// The button that leads to the posts with the attachment (a menu with more than one post).
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns>The html of the button.</returns>
    protected string GetPostsButton(int attachmentId)
    {
        var posts = this.GetPosts(attachmentId);
        var text = this.HtmlEncode(this.GetText("ADMIN_ATTACHMENTS", "SHOW_POSTS"));

        switch (posts.Count)
        {
            case 0:
                return string.Empty;
            case 1:
                return
                    $@"<a href=""{this.GetPostLink(posts[0].MessageId, posts[0].TopicName)}"" class=""btn btn-info"" title=""{this.HtmlEncode(posts[0].TopicName)}""><i class=""fas fa-comment me-1""></i>{text}</a>";
            default:
                var items = string.Concat(
                    posts.Select(
                        post =>
                            $@"<li><a class=""dropdown-item text-break"" href=""{this.GetPostLink(post.MessageId, post.TopicName)}"">{this.HtmlEncode(post.TopicName)}</a></li>"));

                return
                    $@"<div class=""btn-group btn-group-sm""><button type=""button"" class=""btn btn-info dropdown-toggle"" data-bs-toggle=""dropdown"" aria-expanded=""false""><i class=""fas fa-comments me-1""></i>{text}</button><ul class=""dropdown-menu"">{items}</ul></div>";
        }
    }

    /// <summary>
    /// The context menu entries of the posts with the attachment.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns>The html of the entries.</returns>
    protected string GetPostsMenuItems(int attachmentId)
    {
        var posts = this.GetPosts(attachmentId);

        if (posts.Count == 0)
        {
            return string.Empty;
        }

        return string.Concat(
                   posts.Select(
                       post =>
                           $@"<a class=""dropdown-item text-break"" href=""{this.GetPostLink(post.MessageId, post.TopicName)}""><i class=""fas fa-comment me-1""></i>{this.HtmlEncode(post.TopicName)}</a>"))
               + @"<div class=""dropdown-divider""></div>";
    }

    /// <summary>
    /// Gets the link to a post.
    /// </summary>
    /// <param name="messageId">The message id.</param>
    /// <param name="topicName">The topic name.</param>
    /// <returns>The link.</returns>
    private string GetPostLink(int messageId, string topicName)
    {
        return this.Get<LinkBuilder>().GetLink(ForumPages.Posts, new { m = messageId, name = topicName });
    }

    /// <summary>
    /// Populates data source and binds data to controls.
    /// </summary>
    private void BindData()
    {
        var baseSize = this.PageSize.SelectedValue.ToType<int>();
        var currentPageIndex = this.PagerTop.CurrentPageIndex;
        this.PagerTop.PageSize = baseSize;

        // list event for this board
        var list = this.GetRepository<Attachment>().GetByBoardPaged(
            out var count,
            this.PageBoardContext.PageBoardID,
            currentPageIndex,
            baseSize);

        this.List.DataSource = list;

        // the posts with the attachments (the search index found the number in any post, and only after an update)
        this.Usages = list.NullOrEmpty()
                          ? new Dictionary<int, List<(int MessageId, int TopicId, string TopicName)>>()
                          : this.GetRepository<Attachment>().GetUsages(list.Select(x => x.Item2.ID).ToList());

        this.PagerTop.Count = !list.NullOrEmpty() ? count : 0;

        // bind data to controls
        this.DataBind();

        if (this.List.Items.Count == 0)
        {
            this.NoInfo.Visible = true;
        }
    }
}