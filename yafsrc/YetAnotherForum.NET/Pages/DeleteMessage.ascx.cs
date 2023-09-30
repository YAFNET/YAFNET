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

namespace YAF.Pages;

using YAF.Types.Models;

/// <summary>
/// The Delete Message Page.
/// </summary>
public partial class DeleteMessage : ForumPage
{
    /// <summary>
    ///   The is moderator changed.
    /// </summary>
    private bool isModeratorChanged;

    /// <summary>
    ///   Initializes a new instance of the <see cref = "DeleteMessage" /> class.
    /// </summary>
    public DeleteMessage()
        : base("DELETEMESSAGE", ForumPages.DeleteMessage)
    {
    }

    /// <summary>
    ///   Gets a value indicating whether CanDeletePost.
    /// </summary>
    public bool CanDeletePost =>
        (!this.PostLocked && !this.PageBoardContext.PageForum.ForumFlags.IsLocked && !this.PageBoardContext.PageTopic.TopicFlags.IsLocked
         && this.PageBoardContext.PageMessage.UserID == this.PageBoardContext.PageUserID
         || this.PageBoardContext.ForumModeratorAccess) && this.PageBoardContext.ForumDeleteAccess;

    /// <summary>
    ///   Gets a value indicating whether CanUnDeletePost.
    /// </summary>
    public bool CanUnDeletePost => this.PageBoardContext.PageMessage.MessageFlags.IsDeleted && this.CanDeletePost;

    /// <summary>
    ///   Gets a value indicating whether PostLocked.
    /// </summary>
    private bool PostLocked
    {
        get
        {
            if (this.PageBoardContext.IsAdmin || this.PageBoardContext.BoardSettings.LockPosts <= 0)
            {
                return false;
            }

            var edited = this.PageBoardContext.PageMessage.Edited ?? this.PageBoardContext.PageMessage.Posted;

            return edited.AddDays(this.PageBoardContext.BoardSettings.LockPosts) < DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Cancel Deleting and return to topic or forum
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Cancel_Click(object sender, EventArgs e)
    {
        // new topic -- cancel back to forum
        this.Get<LinkBuilder>().Redirect(
            ForumPages.Topics,
            new { f = this.PageBoardContext.PageForumID, name = this.PageBoardContext.PageForum.Name });
    }

    /// <summary>
    /// Gets the reason text.
    /// </summary>
    /// <returns>
    /// Returns the reason text.
    /// </returns>
    protected string GetReasonText()
    {
        return
            this.GetText(
                this.PageBoardContext.PageMessage.MessageFlags.IsDeleted
                    ? "UNDELETE_REASON"
                    : "DELETE_REASON");
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
        this.LinkedPosts.ItemDataBound += this.LinkedPosts_ItemDataBound;

        base.OnInit(e);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.FormValidatorJs),
            JavaScriptBlocks.FormValidatorJs(this.Delete.ClientID));

        this.isModeratorChanged = this.PageBoardContext.PageUserID != this.PageBoardContext.PageMessage.UserID;

        if (!this.PageBoardContext.ForumModeratorAccess
            && this.isModeratorChanged)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        if (this.PageBoardContext.PageForumID == 0)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        if (this.IsPostBack)
        {
            return;
        }

        this.EraseMessage.Checked = false;
        this.EraseMessage.Text = this.GetText("erasemessage");
        this.EraseRow.Visible = false;
        this.LinkedPosts.Visible = false;

        // delete message...
        this.PreviewRow.Visible = true;

        var replies = this.GetRepository<Message>().Replies(
            this.PageBoardContext.PageMessage.ID);

        if (replies.Any() && (this.PageBoardContext.ForumModeratorAccess || this.PageBoardContext.IsAdmin))
        {
            this.LinkedPosts.Visible = true;
            this.LinkedPosts.DataSource = replies;
            this.LinkedPosts.DataBind();
        }

        if (this.PageBoardContext.PageMessage.MessageFlags.IsDeleted)
        {
            this.Title.Text = this.GetText("UNDELETE");

            this.Restore.Visible = true;
        }
        else
        {
            this.Title.Text = this.GetText("DELETE");

            if (this.PageBoardContext.IsAdmin)
            {
                this.EraseRow.Visible = true;
            }

            this.Delete.Visible = true;
        }

        this.Subject.Text = this.PageBoardContext.PageTopic.TopicName;
        this.ReasonEditor.Text = this.PageBoardContext.PageMessage.DeleteReason;

        // populate the message preview with the message data-row...
        this.MessagePreview.Message = this.PageBoardContext.PageMessage.MessageText;
        this.MessagePreview.MessageID = this.PageBoardContext.PageMessage.ID;

        var messageFlags = this.PageBoardContext.PageMessage.MessageFlags;

        if (this.PageBoardContext.PageMessage.MessageFlags.IsDeleted)
        {
            // Override Delete Flag to show Message if Un-Delete action
            messageFlags.IsDeleted = false;
        }

        this.MessagePreview.MessageFlags = messageFlags;
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        // setup page links
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);
        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);
    }

    /// <summary>
    /// The Delete Message click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteClick(object sender, EventArgs e)
    {
        if (!this.CanDeletePost)
        {
            return;
        }

        var deleteAllLinked = false;

        if (this.LinkedPosts.Visible)
        {
            try
            {
                var deleteAllPosts = this.LinkedPosts.Controls[0].Controls[3].ToType<CheckBox>();
                deleteAllLinked = deleteAllPosts.Checked;
            }
            catch (Exception)
            {
                deleteAllLinked = false;
            }
        }

        this.GetRepository<Message>().Delete(
            this.PageBoardContext.PageForumID,
            this.PageBoardContext.PageMessage.TopicID,
            this.PageBoardContext.PageMessage,
            this.isModeratorChanged,
            HttpUtility.HtmlEncode(this.ReasonEditor.Text),
            deleteAllLinked,
            this.EraseMessage.Checked);

        var topic = this.GetRepository<Topic>().GetById(this.PageBoardContext.PageMessage.TopicID);

        // If topic has been deleted, redirect to topic list for active forum, else show remaining posts for topic
        if (topic == null)
        {
            this.Get<LinkBuilder>().Redirect(ForumPages.Topics, new { f = this.PageBoardContext.PageForumID, name = this.PageBoardContext.PageForum.Name });
        }
        else
        {
            this.Get<LinkBuilder>().Redirect(ForumPages.Posts, new { t = topic.ID, name = topic.TopicName });
        }
    }

    /// <summary>
    /// Delete Message(s)
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void RestoreClick(object sender, EventArgs e)
    {
        this.GetRepository<Message>().Restore(
            this.PageBoardContext.PageForumID,
            this.PageBoardContext.PageMessage.TopicID,
            this.PageBoardContext.PageMessage);

        this.Get<LinkBuilder>().Redirect(
            ForumPages.Posts,
            new { m = this.PageBoardContext.PageMessage.ID, name = this.PageBoardContext.PageTopic.TopicName });
    }

    /// <summary>
    /// Check if current user has the rights to delete all linked posts
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
    private void LinkedPosts_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Header)
        {
            return;
        }

        var deleteAllPosts = e.Item.FindControlAs<CheckBox>("DeleteAllPosts");
        deleteAllPosts.Checked =
            deleteAllPosts.Enabled = this.PageBoardContext.ForumModeratorAccess || this.PageBoardContext.IsAdmin;
    }
}