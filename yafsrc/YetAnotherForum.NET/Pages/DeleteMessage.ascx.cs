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
    using System.Data;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
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
        ///   The _forum flags.
        /// </summary>
        protected ForumFlags _forumFlags;

        /// <summary>
        ///   The _is moderator changed.
        /// </summary>
        protected bool _isModeratorChanged;

        /// <summary>
        ///   The _message row.
        /// </summary>
        protected DataRow _messageRow;

        /// <summary>
        ///   The _owner user id.
        /// </summary>
        protected int _ownerUserId;

        /// <summary>
        ///   The _topic flags.
        /// </summary>
        protected TopicFlags _topicFlags;

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
            (!this.PostLocked && !this._forumFlags.IsLocked && !this._topicFlags.IsLocked
             && this._messageRow["UserID"].ToType<int>() == this.PageContext.PageUserID
             || this.PageContext.ForumModeratorAccess) && this.PageContext.ForumDeleteAccess;

        /// <summary>
        ///   Gets a value indicating whether CanUnDeletePost.
        /// </summary>
        public bool CanUnDeletePost => this.PostDeleted && this.CanDeletePost;

        /// <summary>
        ///   Gets a value indicating whether PostDeleted.
        /// </summary>
        private bool PostDeleted
        {
            get
            {
                var deleted = this._messageRow["Flags"].ToType<int>() & 8;

                return deleted == 8;
            }
        }

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

                var edited = this._messageRow["Edited"].ToType<DateTime>();

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
            if (this.Get<HttpRequestBase>().QueryString.Exists("t")
                || this.Get<HttpRequestBase>().QueryString.Exists("m"))
            {
                // reply to existing topic or editing of existing topic
                BuildLink.Redirect(ForumPages.Posts, "t={0}", this.PageContext.PageTopicID);
            }
            else
            {
                // new topic -- cancel back to forum
                BuildLink.Redirect(ForumPages.Topics, "f={0}", this.PageContext.PageForumID);
            }
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
            /* get the forum editor based on the settings
      Message = yaf.editor.EditorHelper.CreateEditorFromType(PageContext.BoardSettings.ForumEditor);
      EditorLine.Controls.Add(Message); 
       */
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
            this._messageRow = null;

            if (this.Get<HttpRequestBase>().QueryString.Exists("m"))
            {
                this._messageRow = this.GetRepository<Message>().ListAsDataTable(
                        Security.StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m")))
                    .GetFirstRowOrInvalid();

                if (!this.PageContext.ForumModeratorAccess
                    && this.PageContext.PageUserID != (int)this._messageRow["UserID"])
                {
                    BuildLink.AccessDenied();
                }
            }

            this._forumFlags = new ForumFlags(this._messageRow["ForumFlags"]);
            this._topicFlags = new TopicFlags(this._messageRow["TopicFlags"]);
            this._ownerUserId = (int)this._messageRow["UserID"];
            this._isModeratorChanged = this.PageContext.PageUserID != this._ownerUserId;

            if (this.PageContext.PageForumID == 0)
            {
                BuildLink.AccessDenied();
            }

            if (!this.Get<HttpRequestBase>().QueryString.Exists("t")
                && !this.PageContext.ForumPostAccess)
            {
                BuildLink.AccessDenied();
            }

            if (this.Get<HttpRequestBase>().QueryString.Exists("t")
                && !this.PageContext.ForumReplyAccess)
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
            
            if (!this.Get<HttpRequestBase>().QueryString.Exists("m"))
            {
                return;
            }

            // delete message...
            this.PreviewRow.Visible = true;

            var tempdb = this.GetRepository<Message>().RepliesListAsDataTable(
                this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m").ToType<int>());

            if (tempdb.HasRows() && (this.PageContext.ForumModeratorAccess || this.PageContext.IsAdmin))
            {
                this.LinkedPosts.Visible = true;
                this.LinkedPosts.DataSource = tempdb;
                this.LinkedPosts.DataBind();
            }

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("action").ToLower() == "delete")
            {
                this.Title.Text = this.GetText("EDIT");
                this.Delete.TextLocalizedTag = "DELETE";

                if (this.PageContext.IsAdmin)
                {
                    this.EraseRow.Visible = true;
                }
            }
            else
            {
                this.Title.Text = this.GetText("EDIT");
                this.Delete.TextLocalizedTag = "UNDELETE";
                this.Delete.Icon = "trash-restore";
            }

            this.Subject.Text = Convert.ToString(this._messageRow["Topic"]);
            this.DeleteReasonRow.Visible = true;
            this.ReasonEditor.Text = Convert.ToString(this._messageRow["DeleteReason"]);

            // populate the message preview with the message data-row...
            this.MessagePreview.Message = this._messageRow["message"].ToString();

            var messageFlags = new MessageFlags(this._messageRow["Flags"]) { IsDeleted = false };

            this.MessagePreview.MessageFlags = messageFlags;
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // setup page links
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.PageContext.PageCategoryName,
                BuildLink.GetLink(ForumPages.Board, "c={0}", this.PageContext.PageCategoryID));
            this.PageLinks.AddForum(this.PageContext.PageForumID);
        }

        /// <summary>
        /// Delete Message(s)
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ToogleDeleteStatus_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.CanDeletePost)
            {
                return;
            }

            // Create objects for easy access
            var tmpMessageID = this._messageRow["MessageID"];
            var tmpForumID = this._messageRow["ForumID"];
            var tmpTopicID = this._messageRow["TopicID"];

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
            this.GetRepository<Message>().Delete(tmpMessageID.ToType<int>(),
                this._isModeratorChanged,
                HttpUtility.HtmlEncode(this.ReasonEditor.Text),
                this.PostDeleted ? 0 : 1,
                deleteAllLinked,
                this.EraseMessage.Checked);

            // retrieve topic information.
            var topic = this.GetRepository<Topic>().GetById(tmpTopicID.ToType<int>());

            // If topic has been deleted, redirect to topic list for active forum, else show remaining posts for topic
            if (topic == null)
            {
                BuildLink.Redirect(ForumPages.Topics, "f={0}", tmpForumID);
            }
            else
            {
                BuildLink.Redirect(ForumPages.Posts, "t={0}", tmpTopicID);
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