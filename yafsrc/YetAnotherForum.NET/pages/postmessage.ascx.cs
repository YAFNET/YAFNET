/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utilities;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The post message Page.
    /// </summary>
    public partial class postmessage : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   The _forum editor.
        /// </summary>
        protected ForumEditor _forumEditor;

        /// <summary>
        ///   The original message.
        /// </summary>
        protected string _originalMessage;

        /// <summary>
        ///   The _owner user id.
        /// </summary>
        protected int _ownerUserId;

        /// <summary>
        ///   The _ux no edit subject.
        /// </summary>
        protected Label _uxNoEditSubject;

        /// <summary>
        ///   Table with choices
        /// </summary>
        protected DataTable choices;

        /// <summary>
        ///   The Spam Approved Indicator
        /// </summary>
        private bool spamApproved = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="postmessage" /> class.
        /// </summary>
        public postmessage()
            : base("POSTMESSAGE")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets EditMessageID.
        /// </summary>
        protected long? EditMessageID
        {
            get
            {
                return this.PageContext.QueryIDs["m"];
            }
        }

        /// <summary>
        ///   Gets or sets OriginalMessage.
        /// </summary>
        protected string OriginalMessage
        {
            get
            {
                return this._originalMessage;
            }

            set
            {
                this._originalMessage = value;
            }
        }

        /// <summary>
        ///   Gets Page.
        /// </summary>
        protected long? PageIndex
        {
            get
            {
                return this.PageContext.QueryIDs["page"];
            }
        }

        /// <summary>
        ///   Gets or sets the PollGroupId if the topic has a poll attached
        /// </summary>
        protected int? PollGroupId { get; set; }

        /// <summary>
        ///   Gets Quoted Message ID.
        /// </summary>
        protected long? QuotedMessageID
        {
            get
            {
                return this.PageContext.QueryIDs["q"];
            }
        }

        /// <summary>
        ///   Gets TopicID.
        /// </summary>
        protected long? TopicID
        {
            get
            {
                return this.PageContext.QueryIDs["t"];
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Canceling Posting New Message Or editing Message.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.TopicID != null || this.EditMessageID != null)
            {
                // reply to existing topic or editing of existing topic
                YafBuildLink.Redirect(ForumPages.posts, "t={0}", this.PageContext.PageTopicID);
            }
            else
            {
                // new topic -- cancel back to forum
                YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
            }
        }

        /// <summary>
        /// The get poll group id.
        /// </summary>
        /// <returns>
        /// Returns the PollGroup Id 
        /// </returns>
        protected int? GetPollGroupID()
        {
            return this.PollGroupId;
        }

        /// <summary>
        /// The handle post to blog.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <param name="subject">
        /// The subject. 
        /// </param>
        /// <returns>
        /// Returns the Blog Post ID 
        /// </returns>
        protected string HandlePostToBlog([NotNull] string message, [NotNull] string subject)
        {
            string blogPostID = string.Empty;

            // Does user wish to post this to their blog?
            if (!this.Get<YafBoardSettings>().AllowPostToBlog || !this.PostToBlog.Checked)
            {
                return blogPostID;
            }

            try
            {
                // Post to blogblog
                var blog = new MetaWeblog(this.PageContext.Profile.BlogServiceUrl);
                blogPostID = blog.newPost(
                    this.PageContext.Profile.BlogServicePassword,
                    this.PageContext.Profile.BlogServiceUsername,
                    this.BlogPassword.Text,
                    subject,
                    message);
            }
            catch
            {
                this.PageContext.AddLoadMessage(this.GetText("POSTTOBLOG_FAILED"), MessageTypes.Error);
            }

            return blogPostID;
        }

        /// <summary>
        /// Verifies the user isn't posting too quickly, if so, tells them to wait.
        /// </summary>
        /// <returns>
        /// True if there is a delay in effect. 
        /// </returns>
        protected bool IsPostReplyDelay()
        {
            // see if there is a post delay
            if (this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess
                || this.Get<YafBoardSettings>().PostFloodDelay <= 0)
            {
                return false;
            }

            // see if they've past that delay point
            if (this.Get<IYafSession>().LastPost
                <= DateTime.UtcNow.AddSeconds(-this.Get<YafBoardSettings>().PostFloodDelay)
                || this.EditMessageID != null)
            {
                return false;
            }

            this.PageContext.AddLoadMessage(
                this.GetTextFormatted(
                    "wait",
                    (this.Get<IYafSession>().LastPost
                     - DateTime.UtcNow.AddSeconds(-this.Get<YafBoardSettings>().PostFloodDelay)).Seconds),
                MessageTypes.Warning);
            return true;
        }

        /// <summary>
        /// Handles verification of the PostReply. Adds java script message if there is a problem.
        /// </summary>
        /// <returns>
        /// true if everything is verified 
        /// </returns>
        protected bool IsPostReplyVerified()
        {
            // To avoid posting whitespace(s) or empty messages
            string postedMessage = this._forumEditor.Text.Trim();

            if (postedMessage.IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("ISEMPTY"), MessageTypes.Warning);
                return false;
            }

            // No need to check whitespace if they are actually posting something
            if (this.Get<YafBoardSettings>().MaxPostSize > 0
                && this._forumEditor.Text.Length >= this.Get<YafBoardSettings>().MaxPostSize)
            {
                this.PageContext.AddLoadMessage(this.GetText("ISEXCEEDED"), MessageTypes.Warning);
                return false;
            }

            // Check if the Entered Guest Username is not too long
            if (this.FromRow.Visible && this.From.Text.Trim().Length > 100)
            {
                this.PageContext.AddLoadMessage(this.GetText("GUEST_NAME_TOOLONG"), MessageTypes.Warning);

                this.From.Text = this.From.Text.Substring(100);
                return false;
            }

            // Check if the topic name is not too long
            if (this.Get<YafBoardSettings>().MaxWordLength > 0
                && this.TopicSubjectTextBox.Text.Trim()
                    .AreAnyWordsOverMaxLength(this.Get<YafBoardSettings>().MaxWordLength))
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("TOPIC_NAME_WORDTOOLONG", this.Get<YafBoardSettings>().MaxWordLength),
                    MessageTypes.Warning);

                try
                {
                    this.TopicSubjectTextBox.Text =
                        this.TopicSubjectTextBox.Text.Substring(this.Get<YafBoardSettings>().MaxWordLength)
                            .Substring(255);
                }
                catch (Exception)
                {
                    this.TopicSubjectTextBox.Text =
                        this.TopicSubjectTextBox.Text.Substring(this.Get<YafBoardSettings>().MaxWordLength);
                }

                return false;
            }

            // Check if the topic description words are not too long
            if (this.Get<YafBoardSettings>().MaxWordLength > 0
                && this.TopicDescriptionTextBox.Text.Trim()
                    .AreAnyWordsOverMaxLength(this.Get<YafBoardSettings>().MaxWordLength))
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("TOPIC_DESCRIPTION_WORDTOOLONG", this.Get<YafBoardSettings>().MaxWordLength),
                    MessageTypes.Warning);

                try
                {
                    this.TopicDescriptionTextBox.Text =
                        this.TopicDescriptionTextBox.Text.Substring(this.Get<YafBoardSettings>().MaxWordLength)
                            .Substring(255);
                }
                catch (Exception)
                {
                    this.TopicDescriptionTextBox.Text =
                        this.TopicDescriptionTextBox.Text.Substring(this.Get<YafBoardSettings>().MaxWordLength);
                }

                return false;
            }

            if (this.SubjectRow.Visible && this.TopicSubjectTextBox.Text.IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("NEED_SUBJECT"), MessageTypes.Warning);
                return false;
            }

            if (!this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().AllowCreateTopicsSameName)
                && LegacyDb.topic_findduplicate(this.TopicSubjectTextBox.Text.Trim()) == 1 && this.TopicID == null
                && this.EditMessageID == null)
            {
                this.PageContext.AddLoadMessage(this.GetText("SUBJECT_DUPLICATE"), MessageTypes.Warning);
                return false;
            }

            if (((this.PageContext.IsGuest && this.Get<YafBoardSettings>().EnableCaptchaForGuests)
                 || (this.Get<YafBoardSettings>().EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded))
                && !CaptchaHelper.IsValid(this.tbCaptcha.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"), MessageTypes.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// The new topic.
        /// </summary>
        /// <returns>
        /// Returns if New Topic 
        /// </returns>
        protected bool NewTopic()
        {
            return !(this.PollGroupId > 0);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            // get the forum editor based on the settings
            string editorId = this.Get<YafBoardSettings>().ForumEditor;

            if (this.Get<YafBoardSettings>().AllowUsersTextEditor)
            {
                // Text editor
                editorId = !string.IsNullOrEmpty(this.PageContext.TextEditor)
                    ? this.PageContext.TextEditor
                    : this.Get<YafBoardSettings>().ForumEditor;
            }

            // Check if Editor exists, if not fallback to default editorid=1
            this._forumEditor = this.Get<IModuleManager<ForumEditor>>().GetBy(editorId, false)
                                ?? this.Get<IModuleManager<ForumEditor>>().GetBy("1");

            // Override Editor when mobile device with default Yaf BBCode Editor
            if (this.PageContext.IsMobileDevice)
            {
                this._forumEditor = this.Get<IModuleManager<ForumEditor>>().GetBy("1");
            }

            this.EditorLine.Controls.Add(this._forumEditor);

            // setup jQuery and YAF JS...
            YafContext.Current.PageElements.RegisterJQuery();

            // Setup Syntax Highlight JS
            YafContext.Current.PageElements.RegisterJsResourceInclude(
                "syntaxhighlighter",
                "js/jquery.syntaxhighligher.js");
            YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.syntaxhighligher.css");
            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "syntaxhighlighterjs",
                JavaScriptBlocks.SyntaxHighlightLoadJs);

            // Setup SpellChecker JS
            YafContext.Current.PageElements.RegisterJsResourceInclude(
                "jqueryspellchecker",
                "js/jquery.spellchecker.min.js");
            YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.spellchecker.css");

            var editorClientId = this._forumEditor.ClientID;

            editorClientId = editorClientId.Replace(
                editorClientId.Substring(editorClientId.LastIndexOf("_", StringComparison.Ordinal)),
                "_YafTextEditor");

            var editorSpellBtnId = "{0}_spell".FormatWith(editorClientId);

            /*if (this._forumEditor.ModuleId.Equals("5") ||
                                this._forumEditor.ModuleId.Equals("0"))
                        {
                                var spellCheckBtn = new Button
                                        {
                                                CssClass = "pbutton", 
                                                ID = "SpellCheckBtn", 
                                                Text = this.GetText("COMMON", "SPELL")
                                        };

                                this.EditorLine.Controls.Add(spellCheckBtn);

                                editorSpellBtnId = spellCheckBtn.ClientID;
                        }*/
            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "spellcheckerjs",
                JavaScriptBlocks.SpellCheckerLoadJs(
                    editorClientId,
                    editorSpellBtnId,
                    this.PageContext.CultureUser.IsSet()
                        ? this.PageContext.CultureUser.Substring(0, 2)
                        : this.Get<YafBoardSettings>().Culture,
                    this.GetText("SPELL_CORRECT")));

            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PageContext.QueryIDs = new QueryStringIDHelper(new[] { "m", "t", "q", "page" }, false);

            TypedMessageList currentMessage = null;
            DataRow topicInfo = LegacyDb.topic_info(this.PageContext.PageTopicID);

            // we reply to a post with a quote
            if (this.QuotedMessageID != null)
            {
                currentMessage =
                    LegacyDb.MessageList(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("q").ToType<int>())
                        .FirstOrDefault();

                this.OriginalMessage = currentMessage.Message;

                if (currentMessage.TopicID.ToType<int>() != this.PageContext.PageTopicID)
                {
                    YafBuildLink.AccessDenied();
                }

                if (!this.CanQuotePostCheck(topicInfo))
                {
                    YafBuildLink.AccessDenied();
                }

                this.PollGroupId = currentMessage.PollID.ToType<int>().IsNullOrEmptyDBField()
                    ? 0
                    : currentMessage.PollID;

                // if this is a quoted message (a new reply with a quote)  we should transfer the TopicId value only to return
                this.PollList.TopicId = this.TopicID.ToType<int>();

                if (this.TopicID == null)
                {
                    this.PollList.TopicId = currentMessage.TopicID.ToType<int>().IsNullOrEmptyDBField()
                        ? 0
                        : currentMessage.TopicID.ToType<int>();
                }
            }
            else if (this.EditMessageID != null)
            {
                currentMessage = LegacyDb.MessageList(this.EditMessageID.ToType<int>()).FirstOrDefault();

                this.OriginalMessage = currentMessage.Message;

                this._ownerUserId = currentMessage.UserID.ToType<int>();

                if (!this.CanEditPostCheck(currentMessage, topicInfo))
                {
                    YafBuildLink.AccessDenied();
                }

                this.PollGroupId = currentMessage.PollID.ToType<int>().IsNullOrEmptyDBField()
                    ? 0
                    : currentMessage.PollID;

                // we edit message and should transfer both the message ID and TopicID for PageLinks. 
                this.PollList.EditMessageId = this.EditMessageID.ToType<int>();

                if (this.TopicID == null)
                {
                    this.PollList.TopicId = currentMessage.TopicID.ToType<int>().IsNullOrEmptyDBField()
                        ? 0
                        : currentMessage.TopicID.ToType<int>();
                }
            }

            if (this.PageContext.PageForumID == 0)
            {
                YafBuildLink.AccessDenied();
            }

            if (this.Get<HttpRequestBase>()["t"] == null && this.Get<HttpRequestBase>()["m"] == null
                && !this.PageContext.ForumPostAccess)
            {
                YafBuildLink.AccessDenied();
            }

            if (this.Get<HttpRequestBase>()["t"] != null && !this.PageContext.ForumReplyAccess)
            {
                YafBuildLink.AccessDenied();
            }

            // Message.EnableRTE = PageContext.BoardSettings.AllowRichEdit;
            this._forumEditor.StyleSheet = this.Get<ITheme>().BuildThemePath("theme.css");
            this._forumEditor.BaseDir = "{0}editors".FormatWith(YafForumInfo.ForumClientFileRoot);

            this.Title.Text = this.GetText("NEWTOPIC");

            if (this.Get<YafBoardSettings>().MaxPostSize == 0)
            {
                this.LocalizedLblMaxNumberOfPost.Visible = false;
            }
            else
            {
                this.LocalizedLblMaxNumberOfPost.Visible = true;
                this.LocalizedLblMaxNumberOfPost.Param0 =
                    this.Get<YafBoardSettings>().MaxPostSize.ToString(CultureInfo.InvariantCulture);
            }

            if (!this.IsPostBack)
            {
                if (this.Get<YafBoardSettings>().EnableTopicDescription)
                {
                    this.DescriptionRow.Visible = true;
                }

                // helper bool -- true if this is a completely new topic...
                bool isNewTopic = (this.TopicID == null) && (this.QuotedMessageID == null)
                                  && (this.EditMessageID == null);

                this.Priority.Items.Add(new ListItem(this.GetText("normal"), "0"));
                this.Priority.Items.Add(new ListItem(this.GetText("sticky"), "1"));
                this.Priority.Items.Add(new ListItem(this.GetText("announcement"), "2"));
                this.Priority.SelectedIndex = 0;

                if (this.Get<YafBoardSettings>().EnableTopicStatus)
                {
                    this.StatusRow.Visible = true;

                    this.TopicStatus.Items.Add(new ListItem("   ", "-1"));

                    foreach (DataRow row in LegacyDb.TopicStatus_List(this.PageContext.PageBoardID).Rows)
                    {
                        var text = this.GetText("TOPIC_STATUS", row["TopicStatusName"].ToString());

                        this.TopicStatus.Items.Add(
                            new ListItem(
                                text.IsSet() ? text : row["DefaultDescription"].ToString(),
                                row["TopicStatusName"].ToString()));
                    }

                    this.TopicStatus.SelectedIndex = 0;
                }

                // Allow the Styling of Topic Titles only for Mods or Admins
                if (this.Get<YafBoardSettings>().UseStyledTopicTitles
                    && (this.PageContext.ForumModeratorAccess || this.PageContext.IsAdmin))
                {
                    this.StyleRow.Visible = true;
                }
                else
                {
                    this.StyleRow.Visible = false;
                }

                this.EditReasonRow.Visible = false;

                this.PriorityRow.Visible = this.PageContext.ForumPriorityAccess;

                // Show post to blog option only to a new post
                this.BlogRow.Visible = this.Get<YafBoardSettings>().AllowPostToBlog && isNewTopic
                                       && !this.PageContext.IsGuest;

                // update options...
                this.PostOptions1.Visible = !this.PageContext.IsGuest;
                this.PostOptions1.PersistantOptionVisible = this.PageContext.ForumPriorityAccess;
                this.PostOptions1.AttachOptionVisible = this.PageContext.ForumUploadAccess;
                this.PostOptions1.WatchOptionVisible = !this.PageContext.IsGuest;
                this.PostOptions1.PollOptionVisible = this.PageContext.ForumPollAccess && isNewTopic;

                ////this.Attachments1.Visible = !this.PageContext.IsGuest;

                // get topic and forum information
                /*DataRow topicInfo = LegacyDb.topic_info(this.PageContext.PageTopicID);
                                using (DataTable dt = LegacyDb.forum_list(this.PageContext.PageBoardID, this.PageContext.PageForumID))
                                {
                                        DataRow forumInfo = dt.Rows[0];
                                }*/

                if (!this.PageContext.IsGuest)
                {
                    /*this.Attachments1.Visible = this.PageContext.ForumUploadAccess;
                                        this.Attachments1.Forum = forumInfo;
                                        this.Attachments1.Topic = topicInfo;
                                        // todo message id*/
                    this.PostOptions1.WatchChecked = this.PageContext.PageTopicID > 0
                        ? this.TopicWatchedId(this.PageContext.PageUserID, this.PageContext.PageTopicID).HasValue
                        : new CombinedUserDataHelper(this.PageContext.PageUserID).AutoWatchTopics;
                }

                if ((this.PageContext.IsGuest && this.Get<YafBoardSettings>().EnableCaptchaForGuests)
                    || (this.Get<YafBoardSettings>().EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded))
                {
                    this.imgCaptcha.ImageUrl = "{0}resource.ashx?c=1".FormatWith(YafForumInfo.ForumClientFileRoot);
                    this.tr_captcha1.Visible = true;
                    this.tr_captcha2.Visible = true;
                }

                if (this.PageContext.Settings.LockedForum == 0)
                {
                    this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
                    this.PageLinks.AddLink(
                        this.PageContext.PageCategoryName,
                        YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
                }

                this.PageLinks.AddForumLinks(this.PageContext.PageForumID);

                // check if it's a reply to a topic...
                if (this.TopicID != null)
                {
                    this.InitReplyToTopic();

                    this.PollList.TopicId = this.TopicID.ToType<int>();
                }

                // If currentRow != null, we are quoting a post in a new reply, or editing an existing post
                if (currentMessage != null)
                {
                    this.OriginalMessage = currentMessage.Message;

                    if (this.QuotedMessageID != null)
                    {
                        if (this.Get<IYafSession>().MultiQuoteIds != null)
                        {
                            if (
                                !this.Get<IYafSession>()
                                    .MultiQuoteIds.Contains(
                                        this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("q").ToType<int>()))
                            {
                                this.Get<IYafSession>()
                                    .MultiQuoteIds.Add(
                                        this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("q").ToType<int>());
                            }

                            var messages = LegacyDb.post_list(
                                this.TopicID,
                                this.PageContext.PageUserID,
                                this.PageContext.PageUserID,
                                0,
                                false,
                                false,
                                false,
                                DateTimeHelper.SqlDbMinTime(),
                                DateTime.UtcNow,
                                DateTimeHelper.SqlDbMinTime(),
                                DateTime.UtcNow,
                                this.PageIndex.ToType<int>(),
                                this.Get<YafBoardSettings>().PostsPerPage,
                                1,
                                0,
                                0,
                                false,
                                0);

                            // quoting a reply to a topic...
                            foreach (var msg in
                                this.Get<IYafSession>()
                                    .MultiQuoteIds.ToArray()
                                    .Select(
                                        id =>
                                            messages.AsEnumerable()
                                                .Select(t => new TypedMessageList(t))
                                                .Where(m => m.MessageID == id.ToType<int>()))
                                    .SelectMany(quotedMessage => quotedMessage))
                            {
                                this.InitQuotedReply(msg);
                            }

                            // Clear Multiquotes
                            this.Get<IYafSession>().MultiQuoteIds = null;
                        }
                        else
                        {
                            this.InitQuotedReply(currentMessage);
                        }

                        this.PollList.TopicId = this.TopicID.ToType<int>();
                    }
                    else if (this.EditMessageID != null)
                    {
                        // editing a message...
                        this.InitEditedPost(currentMessage);
                        this.PollList.EditMessageId = this.EditMessageID.ToType<int>();
                    }

                    this.PollGroupId = currentMessage.PollID.ToType<int>().IsNullOrEmptyDBField()
                        ? 0
                        : currentMessage.PollID.ToType<int>();
                }

                // add the "New Topic" page link last...
                if (isNewTopic)
                {
                    this.PageLinks.AddLink(this.GetText("NEWTOPIC"));
                }

                // form user is only for "Guest"
                this.From.Text = this.Get<IUserDisplayName>().GetName(this.PageContext.PageUserID);
                if (this.User != null)
                {
                    this.FromRow.Visible = false;
                }

                /*   if (this.TopicID == null)
                                {
                                        this.PollList.TopicId = (currentRow["TopicID"].IsNullOrEmptyDBField() ? 0 : Convert.ToInt32(currentRow["TopicID"]));
                                } */
            }

            this.PollList.PollGroupId = this.PollGroupId;
        }

        /// <summary>
        /// The post reply handle edit post.
        /// </summary>
        /// <returns>
        /// Returns the Message Id 
        /// </returns>
        protected long PostReplyHandleEditPost()
        {
            if (!this.PageContext.ForumEditAccess)
            {
                YafBuildLink.AccessDenied();
            }

            string subjectSave = string.Empty;
            string descriptionSave = string.Empty;
            string stylesSave = string.Empty;

            if (this.TopicSubjectTextBox.Enabled)
            {
                subjectSave = this.TopicSubjectTextBox.Text;
            }

            if (this.TopicDescriptionTextBox.Enabled)
            {
                descriptionSave = this.TopicDescriptionTextBox.Text;
            }

            if (this.TopicStylesTextBox.Enabled)
            {
                stylesSave = this.TopicStylesTextBox.Text;
            }

            // Mek Suggestion: This should be removed, resetting flags on edit is a bit lame.
            // Ederon : now it should be better, but all this code around forum/topic/message flags needs revamp
            // retrieve message flags
            var messageFlags = new MessageFlags(LegacyDb.message_list(this.EditMessageID).Rows[0]["Flags"])
                               {
                                   IsHtml =
                                       this
                                       ._forumEditor
                                       .UsesHTML,
                                   IsBBCode
                                       =
                                       this
                                       ._forumEditor
                                       .UsesBBCode,
                                   IsPersistent
                                       =
                                       this
                                       .PostOptions1
                                       .PersistantChecked
                               };

            bool isModeratorChanged = this.PageContext.PageUserID != this._ownerUserId;

            LegacyDb.message_update(
                this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"),
                this.Priority.SelectedValue,
                this._forumEditor.Text.Trim(),
                descriptionSave.Trim(),
                this.TopicStatus.SelectedValue.Equals("-1") || this.TopicStatus.SelectedIndex.Equals(0)
                    ? string.Empty
                    : this.TopicStatus.SelectedValue,
                stylesSave.Trim(),
                subjectSave.Trim(),
                messageFlags.BitValue,
                this.HtmlEncode(this.ReasonEditor.Text),
                isModeratorChanged,
                this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess,
                this.OriginalMessage,
                this.PageContext.PageUserID);

            long messageId = this.EditMessageID.Value;

            this.UpdateWatchTopic(this.PageContext.PageUserID, this.PageContext.PageTopicID);

            this.HandlePostToBlog(this._forumEditor.Text, this.TopicSubjectTextBox.Text);

            // remove cache if it exists...
            this.Get<IDataCache>()
                .Remove(Constants.Cache.FirstPostCleaned.FormatWith(this.PageContext.PageBoardID, this.TopicID));

            return messageId;
        }

        /// <summary>
        /// The post reply handle new post.
        /// </summary>
        /// <param name="topicId">
        /// The topic Id. 
        /// </param>
        /// <returns>
        /// Returns the Message Id. 
        /// </returns>
        protected long PostReplyHandleNewPost(out long topicId)
        {
            long messageId = 0;

            if (!this.PageContext.ForumPostAccess)
            {
                YafBuildLink.AccessDenied();
            }

            // Check if Forum is Moderated
            DataRow forumInfo;
            bool isForumModerated = false;

            using (DataTable dt = LegacyDb.forum_list(this.PageContext.PageBoardID, this.PageContext.PageForumID))
            {
                forumInfo = dt.Rows[0];
            }

            if (forumInfo != null)
            {
                isForumModerated = forumInfo["Flags"].BinaryAnd(ForumFlags.Flags.IsModerated);
            }

            // If Forum is Moderated
            if (isForumModerated)
            {
                this.spamApproved = false;
            }

            // Bypass Approval if Admin or Moderator
            if (this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess)
            {
                this.spamApproved = true;
            }

            // make message flags
            var messageFlags = new MessageFlags
                               {
                                   IsHtml = this._forumEditor.UsesHTML,
                                   IsBBCode = this._forumEditor.UsesBBCode,
                                   IsPersistent = this.PostOptions1.PersistantChecked,
                                   IsApproved = this.spamApproved
                               };

            string blogPostID = this.HandlePostToBlog(this._forumEditor.Text, this.TopicSubjectTextBox.Text);

            // Save to Db
            topicId = LegacyDb.topic_save(
                this.PageContext.PageForumID,
                this.TopicSubjectTextBox.Text.Trim(),
                this.TopicStatus.SelectedValue.Equals("-1") || this.TopicStatus.SelectedIndex.Equals(0)
                    ? string.Empty
                    : this.TopicStatus.SelectedValue,
                this.TopicStylesTextBox.Text.Trim(),
                this.TopicDescriptionTextBox.Text.Trim(),
                this._forumEditor.Text,
                this.PageContext.PageUserID,
                this.Priority.SelectedValue,
                this.User != null ? null : this.From.Text,
                this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                DateTime.UtcNow,
                blogPostID,
                messageFlags.BitValue,
                ref messageId);

            this.UpdateWatchTopic(this.PageContext.PageUserID, (int)topicId);

            // clear caches as stats changed
            if (messageFlags.IsApproved)
            {
                this.Get<IDataCache>().Remove(Constants.Cache.BoardStats);
                this.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);
            }

            return messageId;
        }

        /// <summary>
        /// The post reply handle reply to topic.
        /// </summary>
        /// <param name="isSpamApproved">
        /// The is Spam Approved. 
        /// </param>
        /// <returns>
        /// Returns the Message Id. 
        /// </returns>
        protected long PostReplyHandleReplyToTopic(bool isSpamApproved)
        {
            long messageId = 0;

            if (!this.PageContext.ForumReplyAccess)
            {
                YafBuildLink.AccessDenied();
            }

            // Check if Forum is Moderated
            DataRow forumInfo;
            bool isForumModerated = false;

            using (DataTable dt = LegacyDb.forum_list(this.PageContext.PageBoardID, this.PageContext.PageForumID))
            {
                forumInfo = dt.Rows[0];
            }

            if (forumInfo != null)
            {
                isForumModerated = forumInfo["Flags"].BinaryAnd(ForumFlags.Flags.IsModerated);
            }

            // If Forum is Moderated
            if (isForumModerated)
            {
                isSpamApproved = false;
            }

            // Bypass Approval if Admin or Moderator
            if (this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess)
            {
                isSpamApproved = true;
            }

            object replyTo = (this.QuotedMessageID != null) ? this.QuotedMessageID.Value : -1;

            // make message flags
            var messageFlags = new MessageFlags
                               {
                                   IsHtml = this._forumEditor.UsesHTML,
                                   IsBBCode = this._forumEditor.UsesBBCode,
                                   IsPersistent = this.PostOptions1.PersistantChecked,
                                   IsApproved = isSpamApproved
                               };

            LegacyDb.message_save(
                this.TopicID.Value,
                this.PageContext.PageUserID,
                this._forumEditor.Text,
                this.User != null ? null : this.From.Text,
                this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                DateTime.UtcNow,
                replyTo,
                messageFlags.BitValue,
                ref messageId);

            this.UpdateWatchTopic(this.PageContext.PageUserID, this.PageContext.PageTopicID);

            if (messageFlags.IsApproved)
            {
                this.Get<IDataCache>().Remove(Constants.Cache.BoardStats);
                this.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);
            }

            return messageId;
        }

        /// <summary>
        /// Handles the PostReply click including: Replying, Editing and New post.
        /// </summary>
        /// <param name="sender">
        /// The Sender Object. 
        /// </param>
        /// <param name="e">
        /// The Event Arguments. 
        /// </param>
        protected void PostReply_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsPostReplyVerified())
            {
                return;
            }

            if (this.IsPostReplyDelay())
            {
                return;
            }

            // Check for SPAM
            if (!this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess
                && !this.Get<YafBoardSettings>().SpamServiceType.Equals(0))
            {
                var spamChecker = new YafSpamCheck();
                string spamResult;

                // Check content for spam
                if (
                    spamChecker.CheckPostForSpam(
                        this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                        YafContext.Current.Get<HttpRequestBase>().GetUserRealIPAddress(),
                        BBCodeHelper.StripBBCode(
                            HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(this._forumEditor.Text)))
                            .RemoveMultipleWhitespace(),
                        this.PageContext.IsGuest ? null : this.PageContext.User.Email,
                        out spamResult))
                {
                    if (this.Get<YafBoardSettings>().SpamMessageHandling.Equals(1))
                    {
                        this.spamApproved = false;

                        this.Get<ILogger>()
                            .Info(
                                "Spam Check detected possible SPAM ({2}) posted by User: {0}, it was flagged as unapproved post. Content was: {1}",
                                this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                                this._forumEditor.Text,
                                spamResult);
                    }
                    else if (this.Get<YafBoardSettings>().SpamMessageHandling.Equals(2))
                    {
                        this.Get<ILogger>()
                            .Info(
                                "Spam Check detected possible SPAM ({2}) posted by User: {0}, post was rejected. Content was: {1}",
                                this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                                this._forumEditor.Text,
                                spamResult);

                        this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.Error);

                        return;
                    }
                }

                // check user for spam bot
                /*if (spamChecker.CheckUserForSpamBot(
                    this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                    this.PageContext.IsGuest ? null : this.PageContext.User.Email,
                    this.Get<HttpRequestBase>().GetUserRealIPAddress()
                    ))
                {
                    if (this.Get<YafBoardSettings>().SpamMessageHandling.Equals(1))
                    {
                        this.spamApproved = false;

                        this.Get<ILogger>()
                            .Info(
                                "Spam Check detected possible SPAM posted by User: {0}, it was flagged as unapproved post. Content was: {1}",
                                this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                                this._forumEditor.Text);
                    }
                    else if (this.Get<YafBoardSettings>().SpamMessageHandling.Equals(2))
                    {
                        this.Get<ILogger>()
                            .Info(
                                "Spam Check detected possible SPAM posted by User: {0}, post was rejected. Content was: {1}",
                                this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                                this._forumEditor.Text);

                        this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.Error);
                        
                        return;
                    }
                }*/
            }

            // TODO
            /*if (this.Get<YafBoardSettings>().UserPostsRequiredForUrls < this.PageContext.CurrentUserData.NumPosts)
            {
                this._forumEditor.Text = BBCodeHelper.StripBBCodeUrls(this._forumEditor.Text);
            }*/

            // update the last post time...
            this.Get<IYafSession>().LastPost = DateTime.UtcNow.AddSeconds(30);

            long messageId;
            long newTopic = 0;

            if (this.TopicID != null)
            {
                // Reply to topic
                messageId = this.PostReplyHandleReplyToTopic(this.spamApproved);
                newTopic = this.TopicID.ToType<long>();
            }
            else if (this.EditMessageID != null)
            {
                // Edit existing post
                messageId = this.PostReplyHandleEditPost();
            }
            else
            {
                // New post
                messageId = this.PostReplyHandleNewPost(out newTopic);
            }

            // Check if message is approved
            bool isApproved = false;
            using (DataTable dt = LegacyDb.message_list(messageId))
            {
                foreach (DataRow row in dt.Rows)
                {
                    isApproved = row["Flags"].BinaryAnd(MessageFlags.Flags.IsApproved);
                }
            }

            // vzrus^ the poll access controls are enabled and this is a new topic - we add the variables
            string attachp = string.Empty;
            string retforum = string.Empty;

            if (this.PageContext.ForumPollAccess && this.PostOptions1.PollOptionVisible && newTopic > 0)
            {
                // new topic poll token
                attachp = "&t={0}".FormatWith(newTopic);

                // new return forum poll token
                retforum = "&f={0}".FormatWith(this.PageContext.PageForumID);
            }

            // Create notification emails
            if (isApproved)
            {
                this.Get<ISendNotification>().ToWatchingUsers(messageId.ToType<int>());

                if (Config.IsDotNetNuke && this.EditMessageID == null)
                {
                    if (this.TopicID != null)
                    {
                        this.Get<IActivityStream>()
                            .AddReplyToStream(
                                this.PageContext.PageForumID,
                                newTopic,
                                messageId.ToType<int>(),
                                this.PageContext.PageTopicName,
                                this._forumEditor.Text);
                    }
                    else
                    {
                        this.Get<IActivityStream>()
                           .AddTopicToStream(
                               this.PageContext.PageForumID,
                               newTopic,
                               messageId.ToType<int>(),
                               this.TopicSubjectTextBox.Text,
                               this._forumEditor.Text);
                    }
                }

                if (this.PageContext.ForumUploadAccess && this.PostOptions1.AttachChecked)
                {
                    // 't' variable is required only for poll and this is a attach poll token for attachments page
                    if (!this.PostOptions1.PollChecked)
                    {
                        attachp = string.Empty;
                    }

                    // redirect to the attachment page...
                    YafBuildLink.Redirect(ForumPages.attachments, "m={0}{1}", messageId, attachp);
                }
                else
                {
                    if (attachp.IsNotSet() || (!this.PostOptions1.PollChecked))
                    {
                        // regular redirect...
                        YafBuildLink.Redirect(ForumPages.posts, "m={0}#post{0}", messageId);
                    }
                    else
                    {
                        // poll edit redirect...
                        YafBuildLink.Redirect(ForumPages.polledit, "{0}", attachp);
                    }
                }
            }
            else
            {
                // Not Approved
                if (this.Get<YafBoardSettings>().EmailModeratorsOnModeratedPost)
                {
                    // not approved, notifiy moderators
                    this.Get<ISendNotification>()
                        .ToModeratorsThatMessageNeedsApproval(this.PageContext.PageForumID, (int)messageId);
                }

                // 't' variable is required only for poll and this is a attach poll token for attachments page
                if (!this.PostOptions1.PollChecked)
                {
                    attachp = string.Empty;
                }

                if (this.PostOptions1.AttachChecked && this.PageContext.ForumUploadAccess)
                {
                    // redirect to the attachment page...
                    YafBuildLink.Redirect(ForumPages.attachments, "m={0}&ra=1{1}{2}", messageId, attachp, retforum);
                }
                else
                {
                    // Tell user that his message will have to be approved by a moderator
                    string url = YafBuildLink.GetLink(ForumPages.topics, "f={0}", this.PageContext.PageForumID);

                    if (this.PageContext.PageTopicID > 0)
                    {
                        url = YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.PageContext.PageTopicID);
                    }

                    if (attachp.Length <= 0)
                    {
                        YafBuildLink.Redirect(ForumPages.info, "i=1&url={0}", this.Server.UrlEncode(url));
                    }
                    else
                    {
                        YafBuildLink.Redirect(ForumPages.polledit, "&ra=1{0}{1}", attachp, retforum);
                    }

                    if (Config.IsRainbow)
                    {
                        YafBuildLink.Redirect(ForumPages.info, "i=1");
                    }
                }
            }
        }

        /// <summary>
        /// Previews the new Message
        /// </summary>
        /// <param name="sender">
        /// The Sender Object. 
        /// </param>
        /// <param name="e">
        /// The Event Arguments. 
        /// </param>
        protected void Preview_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PreviewRow.Visible = true;

            this.PreviewMessagePost.MessageFlags.IsHtml = this._forumEditor.UsesHTML;
            this.PreviewMessagePost.MessageFlags.IsBBCode = this._forumEditor.UsesBBCode;
            this.PreviewMessagePost.Message = this._forumEditor.Text;

            if (!this.Get<YafBoardSettings>().AllowSignatures)
            {
                return;
            }

            string userSig = LegacyDb.user_getsignature(this.PageContext.PageUserID);

            if (userSig.IsSet())
            {
                this.PreviewMessagePost.Signature = userSig;
            }
        }

        /// <summary>
        /// The can edit post check.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <param name="topicInfo">
        /// The topic Info. 
        /// </param>
        /// <returns>
        /// Returns if user can edit post check. 
        /// </returns>
        private bool CanEditPostCheck([NotNull] TypedMessageList message, DataRow topicInfo)
        {
            bool postLocked = false;

            if (!this.PageContext.IsAdmin && this.Get<YafBoardSettings>().LockPosts > 0)
            {
                var edited = message.Edited.ToType<DateTime>();

                if (edited.AddDays(this.Get<YafBoardSettings>().LockPosts) < DateTime.UtcNow)
                {
                    postLocked = true;
                }
            }

            DataRow forumInfo;

            // get  forum information
            using (DataTable dt = LegacyDb.forum_list(this.PageContext.PageBoardID, this.PageContext.PageForumID))
            {
                forumInfo = dt.Rows[0];
            }

            // Ederon : 9/9/2007 - moderator can edit in locked topics
            return ((!postLocked && !forumInfo["Flags"].BinaryAnd(ForumFlags.Flags.IsLocked)
                     && !topicInfo["Flags"].BinaryAnd(TopicFlags.Flags.IsLocked)
                     && (message.UserID.ToType<int>() == this.PageContext.PageUserID))
                    || this.PageContext.ForumModeratorAccess) && this.PageContext.ForumEditAccess;
        }

        /// <summary>
        /// Determines whether this instance [can quote post check] the specified topic info.
        /// </summary>
        /// <param name="topicInfo">
        /// The topic info. 
        /// </param>
        /// <returns>
        /// The can quote post check. 
        /// </returns>
        private bool CanQuotePostCheck(DataRow topicInfo)
        {
            DataRow forumInfo;

            // get topic and forum information
            using (DataTable dt = LegacyDb.forum_list(this.PageContext.PageBoardID, this.PageContext.PageForumID))
            {
                forumInfo = dt.Rows[0];
            }

            if (topicInfo == null || forumInfo == null)
            {
                return false;
            }

            // Ederon : 9/9/2007 - moderator can reply to locked topics
            return (!forumInfo["Flags"].BinaryAnd(ForumFlags.Flags.IsLocked)
                    && !topicInfo["Flags"].BinaryAnd(TopicFlags.Flags.IsLocked) || this.PageContext.ForumModeratorAccess)
                   && this.PageContext.ForumReplyAccess;
        }

        /// <summary>
        /// The init edited post.
        /// </summary>
        /// <param name="currentMessage">
        /// The current message. 
        /// </param>
        private void InitEditedPost([NotNull] TypedMessageList currentMessage)
        {
            if (this._forumEditor.UsesHTML && currentMessage.Flags.IsBBCode)
            {
                // If the message is in YafBBCode but the editor uses HTML, convert the message text to HTML
                currentMessage.Message = this.Get<IBBCode>().ConvertBBCodeToHtmlForEdit(currentMessage.Message);
            }

            if (this._forumEditor.UsesBBCode && currentMessage.Flags.IsHtml)
            {
                // If the message is in HTML but the editor uses YafBBCode, convert the message text to BBCode
                currentMessage.Message = this.Get<IBBCode>().ConvertHtmltoBBCodeForEdit(currentMessage.Message);
            }

            this._forumEditor.Text = currentMessage.Message;

            this.Title.Text = this.GetText("EDIT");
            this.PostReply.TextLocalizedTag = "SAVE";
            this.PostReply.TextLocalizedPage = "COMMON";

            // add topic link...
            this.PageLinks.AddLink(
                this.Server.HtmlDecode(currentMessage.Topic),
                YafBuildLink.GetLink(ForumPages.posts, "m={0}", this.EditMessageID));

            // editing..
            this.PageLinks.AddLink(this.GetText("EDIT"));

            string blogPostID = currentMessage.BlogPostID;

            if (blogPostID != string.Empty)
            {
                // The user used this post to blog
                this.BlogPostID.Value = blogPostID;
                this.PostToBlog.Checked = true;

                this.BlogRow.Visible = this.Get<YafBoardSettings>().AllowPostToBlog;
            }

            this.TopicSubjectTextBox.Text = this.Server.HtmlDecode(currentMessage.Topic);
            this.TopicDescriptionTextBox.Text = this.Server.HtmlDecode(currentMessage.Description);

            if ((currentMessage.TopicOwnerID.ToType<int>() == currentMessage.UserID.ToType<int>())
                || this.PageContext.ForumModeratorAccess)
            {
                // allow editing of the topic subject
                this.TopicSubjectTextBox.Enabled = true;
                if (this.Get<YafBoardSettings>().EnableTopicDescription)
                {
                    this.DescriptionRow.Visible = true;
                }

                if (this.Get<YafBoardSettings>().EnableTopicStatus)
                {
                    this.StatusRow.Visible = true;
                }

                this.TopicStatus.Enabled = true;
            }
            else
            {
                this.TopicSubjectTextBox.Enabled = false;
                this.TopicDescriptionTextBox.Enabled = false;
                this.TopicStatus.Enabled = false;
            }

            // Allow the Styling of Topic Titles only for Mods or Admins
            if (this.Get<YafBoardSettings>().UseStyledTopicTitles
                && (this.PageContext.ForumModeratorAccess || this.PageContext.IsAdmin))
            {
                this.StyleRow.Visible = true;
            }
            else
            {
                this.StyleRow.Visible = false;
                this.TopicStylesTextBox.Enabled = false;
            }

            this.TopicStylesTextBox.Text = currentMessage.Styles;

            this.Priority.SelectedItem.Selected = false;
            this.Priority.Items.FindByValue(currentMessage.Priority.ToString()).Selected = true;

            if (this.TopicStatus.SelectedItem != null)
            {
                this.TopicStatus.SelectedItem.Selected = false;
            }

            if (this.TopicStatus.Items.FindByValue(currentMessage.Status) != null)
            {
                this.TopicStatus.Items.FindByValue(currentMessage.Status).Selected = true;
            }

            this.EditReasonRow.Visible = true;
            this.ReasonEditor.Text = this.Server.HtmlDecode(currentMessage.EditReason);
            this.PostOptions1.PersistantChecked = currentMessage.Flags.IsPersistent;

            // this.Attachments1.MessageID = (int)this.EditMessageID;
        }

        /// <summary>
        /// The init quoted reply.
        /// </summary>
        /// <param name="message">
        /// The current TypedMessage. 
        /// </param>
        private void InitQuotedReply(TypedMessageList message)
        {
            var messageContent = message.Message;

            if (this.Get<YafBoardSettings>().RemoveNestedQuotes)
            {
                messageContent = this.Get<IFormatMessage>().RemoveNestedQuotes(messageContent);
            }

            if (this._forumEditor.UsesHTML && message.Flags.IsBBCode)
            {
                // If the message is in YafBBCode but the editor uses HTML, convert the message text to HTML
                messageContent = this.Get<IBBCode>().ConvertBBCodeToHtmlForEdit(messageContent);
            }

            if (this._forumEditor.UsesBBCode && message.Flags.IsHtml)
            {
                // If the message is in HTML but the editor uses YafBBCode, convert the message text to BBCode
                messageContent = this.Get<IBBCode>().ConvertHtmltoBBCodeForEdit(messageContent);
            }

            // Ensure quoted replies have bad words removed from them
            messageContent = this.Get<IBadWordReplace>().Replace(messageContent);

            // Remove HIDDEN Text
            messageContent = this.Get<IFormatMessage>().RemoveHiddenBBCodeContent(messageContent);

            // Quote the original message
            this._forumEditor.Text +=
                "[quote={0};{1}]{2}[/quote]\n\n".FormatWith(
                    this.Get<IUserDisplayName>().GetName(message.UserID.ToType<int>()),
                    message.MessageID,
                    messageContent).TrimStart();
        }

        /// <summary>
        /// The init reply to topic.
        /// </summary>
        private void InitReplyToTopic()
        {
            DataRow topic = LegacyDb.topic_info(this.TopicID);
            var topicFlags = new TopicFlags(topic["Flags"].ToType<int>());

            // Ederon : 9/9/2007 - moderators can reply in locked topics
            if (topicFlags.IsLocked && !this.PageContext.ForumModeratorAccess)
            {
                this.Response.Redirect(this.Get<HttpRequestBase>().UrlReferrer.ToString());
            }

            this.PriorityRow.Visible = false;
            this.SubjectRow.Visible = false;
            this.DescriptionRow.Visible = false;
            this.StatusRow.Visible = false;
            this.StyleRow.Visible = false;
            this.Title.Text = this.GetText("reply");

            // add topic link...
            this.PageLinks.AddLink(
                this.Server.HtmlDecode(topic["Topic"].ToString()),
                YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.TopicID));

            // add "reply" text...
            this.PageLinks.AddLink(this.GetText("reply"));

            // show attach file option if its a reply...
            if (this.PageContext.ForumUploadAccess)
            {
                this.PostOptions1.Visible = true;
                this.PostOptions1.AttachOptionVisible = true;

                // this.Attachments1.Visible = true;
            }

            // show the last posts AJAX frame...
            this.LastPosts1.Visible = true;
            this.LastPosts1.TopicID = this.TopicID.Value;
        }

        /// <summary>
        /// Returns true if the topic is set to watch for userId
        /// </summary>
        /// <param name="userId">
        /// The user Id. 
        /// </param>
        /// <param name="topicId">
        /// The topic Id. 
        /// </param>
        /// <returns>
        /// The topic watched id. 
        /// </returns>
        private int? TopicWatchedId(int userId, int topicId)
        {
            return this.GetRepository<WatchTopic>().Check(userId, topicId);
        }

        /// <summary>
        /// Updates Watch Topic based on controls/settings for user...
        /// </summary>
        /// <param name="userId">
        /// The user Id. 
        /// </param>
        /// <param name="topicId">
        /// The topic Id. 
        /// </param>
        private void UpdateWatchTopic(int userId, int topicId)
        {
            var topicWatchedID = this.TopicWatchedId(userId, topicId);

            if (topicWatchedID.HasValue && !this.PostOptions1.WatchChecked)
            {
                // unsubscribe...
                this.GetRepository<WatchTopic>().DeleteByID(topicWatchedID.Value);
            }
            else if (!topicWatchedID.HasValue && this.PostOptions1.WatchChecked)
            {
                // subscribe to this topic...
                this.WatchTopic(userId, topicId);
            }
        }

        /// <summary>
        /// Checks if this topic is watched, if not, adds it.
        /// </summary>
        /// <param name="userId">
        /// The user Id. 
        /// </param>
        /// <param name="topicId">
        /// The topic Id. 
        /// </param>
        private void WatchTopic(int userId, int topicId)
        {
            if (!this.TopicWatchedId(userId, topicId).HasValue)
            {
                this.GetRepository<WatchTopic>().Add(userId, topicId);
            }
        }

        #endregion
    }
}