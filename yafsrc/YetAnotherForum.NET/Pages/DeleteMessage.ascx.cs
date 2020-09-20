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
namespace YAF.Pages
{
    #region Using

    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Utilities;
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
    /// The Delete Message Page.
    /// </summary>
    public partial class DeleteMessage : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   The is moderator changed.
        /// </summary>
        private bool isModeratorChanged;

        /// <summary>
        ///   The message row.
        /// </summary>
        private Tuple<Topic, Message, User, Forum> message;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DeleteMessage" /> class.
        /// </summary>
        public DeleteMessage()
            : base("DELETEMESSAGE")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether CanDeletePost.
        /// </summary>
        public bool CanDeletePost =>
            (!this.PostLocked && !this.message.Item4.ForumFlags.IsLocked && !this.message.Item1.TopicFlags.IsLocked
             && this.message.Item1.UserID == this.PageContext.PageUserID
             || this.PageContext.ForumModeratorAccess) && this.PageContext.ForumDeleteAccess;

        /// <summary>
        ///   Gets a value indicating whether CanUnDeletePost.
        /// </summary>
        public bool CanUnDeletePost => this.message.Item2.MessageFlags.IsDeleted && this.CanDeletePost;

        /// <summary>
        /// The message id.
        /// </summary>
        protected int MessageId => Security.StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"));

        /// <summary>
        ///   Gets a value indicating whether PostLocked.
        /// </summary>
        private bool PostLocked
        {
            get
            {
                if (this.PageContext.IsAdmin || this.Get<BoardSettings>().LockPosts <= 0)
                {
                    return false;
                }

                var edited = this.message.Item2.Edited.Value;

                return edited.AddDays(this.Get<BoardSettings>().LockPosts) < DateTime.UtcNow;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cancel Deleting and return to topic or forum
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // new topic -- cancel back to forum
            BuildLink.Redirect(
                ForumPages.Topics,
                "f={0}&name={1}",
                this.PageContext.PageForumID,
                this.PageContext.PageForumName);
        }

        /// <summary>
        /// Gets the action text.
        /// </summary>
        /// <returns>
        /// Returns the Action Text
        /// </returns>
        protected string GetActionText()
        {
            return this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("action").ToLower() == "delete"
                       ? this.GetText("DELETE")
                       : this.GetText("UNDELETE");
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
                    this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("action").ToLower() == "delete"
                        ? "DELETE_REASON"
                        : "UNDELETE_REASON");
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.LinkedPosts.ItemDataBound += this.LinkedPosts_ItemDataBound;

            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PageContext.PageElements.RegisterJsBlockStartup(
                nameof(JavaScriptBlocks.FormValidatorJs),
                JavaScriptBlocks.FormValidatorJs(this.Delete.ClientID));

            this.message = this.GetRepository<Message>().GetMessage(this.MessageId);

            this.isModeratorChanged = this.PageContext.PageUserID != this.message.Item1.UserID;

            if (!this.PageContext.ForumModeratorAccess
                && this.isModeratorChanged)
            {
                BuildLink.AccessDenied();
            }

            if (this.PageContext.PageForumID == 0)
            {
                BuildLink.AccessDenied();
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.EraseMessage.Checked = false;
            this.EraseMessage.Text = this.GetText("erasemessage");
            this.EraseRow.Visible = false;
            this.DeleteReasonRow.Visible = false;
            this.LinkedPosts.Visible = false;

            // delete message...
            this.PreviewRow.Visible = true;

            var replies = this.GetRepository<Message>().Replies(
                this.MessageId);

            if (replies.Any() && (this.PageContext.ForumModeratorAccess || this.PageContext.IsAdmin))
            {
                this.LinkedPosts.Visible = true;
                this.LinkedPosts.DataSource = replies;
                this.LinkedPosts.DataBind();
            }

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("action").ToLower() == "delete")
            {
                this.Title.Text = this.GetText("EDIT");
                this.Delete.TextLocalizedTag = "DELETE";
                this.Delete.TitleLocalizedTag = "DELETE_TT";

                if (this.PageContext.IsAdmin)
                {
                    this.EraseRow.Visible = true;
                }

                this.DeleteUndelete.Visible = false;
            }
            else
            {
                this.Title.Text = this.GetText("EDIT");
                this.Delete.TextLocalizedTag = "UNDELETE";
                this.Delete.TitleLocalizedTag = "UNDELETE_TT";
                this.Delete.Icon = "trash-restore";
                this.Delete.Type = ButtonStyle.Warning;

                this.DeleteUndelete.TextLocalizedTag = "BUTTON_DELETE_UNDELETE";
                this.DeleteUndelete.TitleLocalizedTag = "BUTTON_DELETE_UNDELETE_TT";

                if (this.PageContext.IsAdmin)
                {
                    this.DeleteUndelete.Visible = true;
                }
            }

            this.Subject.Text = this.message.Item1.TopicName;
            this.DeleteReasonRow.Visible = true;
            this.ReasonEditor.Text = this.message.Item2.DeleteReason;

            // populate the message preview with the message data-row...
            this.MessagePreview.Message = this.message.Item2.MessageText;
            this.MessagePreview.MessageID = this.message.Item2.ID;
            this.MessagePreview.MessageFlags = this.message.Item2.MessageFlags;
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // setup page links
            this.PageLinks.AddRoot();
            this.PageLinks.AddCategory(this.PageContext.PageCategoryName, this.PageContext.PageCategoryID);
            this.PageLinks.AddForum(this.PageContext.PageForumID);
        }

        /// <summary>
        /// The delete Un-delete click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void DeleteUndelete_Click(object sender, EventArgs e)
        {
            this.ToggleDelete(0, true);
        }

        /// <summary>
        /// Delete Message(s)
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ToggleDeleteStatus_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.ToggleDelete(this.message.Item2.MessageFlags.IsDeleted ? 0 : 1, this.EraseMessage.Checked);
        }

        /// <summary>
        /// The toggle delete.
        /// </summary>
        /// <param name="deleteAction">
        /// The delete action.
        /// </param>
        /// <param name="eraseMessage">
        /// The erase message.
        /// </param>
        private void ToggleDelete(int deleteAction, bool eraseMessage)
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
                    var deleteAllPosts = this.LinkedPosts.Controls[0].Controls[1].ToType<CheckBox>();
                    deleteAllLinked = deleteAllPosts.Checked;
                }
                catch (Exception)
                {
                    deleteAllLinked = false;
                }
            }

            // Toggle delete message -- if the message is currently deleted it will be un-deleted.
            // If it's not deleted it will be marked deleted.
            // If it is the last message of the topic, the topic is also deleted
            this.GetRepository<Message>().Delete(
                this.message.Item1.ForumID,
                this.message.Item1.ID,
                this.message.Item2.ID,
                this.isModeratorChanged,
                HttpUtility.HtmlEncode(this.ReasonEditor.Text),
                deleteAction,
                deleteAllLinked,
                eraseMessage);

            // retrieve topic information.
            var topic = this.GetRepository<Topic>().GetById(this.message.Item2.TopicID);

            // If topic has been deleted, redirect to topic list for active forum, else show remaining posts for topic
            if (topic == null)
            {
                BuildLink.Redirect(ForumPages.Topics, "f={0}&name={1}", this.message.Item3.ID, this.message.Item3.Name);
            }
            else
            {
                BuildLink.Redirect(ForumPages.Posts, "t={0}&name={1}", topic.ID, topic.TopicName);
            }
        }

        /// <summary>
        /// Check if current user has the rights to delete all linked posts
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
        private void LinkedPosts_ItemDataBound([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Header)
            {
                return;
            }

            var deleteAllPosts = e.Item.FindControlAs<CheckBox>("DeleteAllPosts");
            deleteAllPosts.Checked =
                deleteAllPosts.Enabled = this.PageContext.ForumModeratorAccess || this.PageContext.IsAdmin;
        }

        #endregion
    }
}