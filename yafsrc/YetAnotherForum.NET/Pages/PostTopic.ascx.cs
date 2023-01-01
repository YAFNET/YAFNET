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

using YAF.Types.Models;

namespace YAF.Pages;

/// <summary>
/// The post New Topic Page.
/// </summary>
public partial class PostTopic : ForumPage
{
    /// <summary>
    ///   The forum editor.
    /// </summary>
    private ForumEditor forumEditor;

    /// <summary>
    ///   The Spam Approved Indicator
    /// </summary>
    private bool spamApproved = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostTopic"/> class.
    /// </summary>
    public PostTopic()
        : base("POSTTOPIC", ForumPages.PostTopic)
    {
    }

    /// <summary>
    /// Canceling Posting New Message Or editing Message.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        // new topic -- cancel back to forum
        this.Get<LinkBuilder>().Redirect(
            ForumPages.Topics,
            new {f = this.PageBoardContext.PageForumID, name = this.PageBoardContext.PageForum.Name });
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
        if (this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess
                                          || this.PageBoardContext.BoardSettings.PostFloodDelay <= 0)
        {
            return false;
        }

        // see if they've past that delay point
        if (this.Get<ISession>().LastPost
            <= DateTime.UtcNow.AddSeconds(-this.PageBoardContext.BoardSettings.PostFloodDelay))
        {
            return false;
        }

        this.PageBoardContext.Notify(
            this.GetTextFormatted(
                "wait",
                (this.Get<ISession>().LastPost
                 - DateTime.UtcNow.AddSeconds(-this.PageBoardContext.BoardSettings.PostFloodDelay)).Seconds),
            MessageTypes.warning);
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
        var postedMessage = HtmlHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(this.forumEditor.Text.Trim()));

        if (postedMessage.IsNotSet())
        {
            this.PageBoardContext.Notify(this.GetText("ISEMPTY"), MessageTypes.warning);
            return false;
        }

        // No need to check whitespace if they are actually posting something
        if (this.PageBoardContext.BoardSettings.MaxPostSize > 0
            && this.forumEditor.Text.Length >= this.PageBoardContext.BoardSettings.MaxPostSize)
        {
            this.PageBoardContext.Notify(this.GetText("ISEXCEEDED"), MessageTypes.warning);
            return false;
        }

        // Check if the Entered Guest Username is not too long
        if (this.FromRow.Visible && this.From.Text.Trim().Length > 100)
        {
            this.PageBoardContext.Notify(this.GetText("GUEST_NAME_TOOLONG"), MessageTypes.warning);

            this.From.Text = this.From.Text.Substring(100);
            return false;
        }

        if (HtmlHelper.StripHtml(this.TopicSubjectTextBox.Text).IsNotSet())
        {
            this.PageBoardContext.Notify(this.GetText("NEED_SUBJECT"), MessageTypes.warning);
            return false;
        }

        if (!this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.AllowCreateTopicsSameName)
            && this.GetRepository<Topic>().CheckForDuplicate(HtmlHelper.StripHtml(this.TopicSubjectTextBox.Text).Trim()))
        {
            this.PageBoardContext.Notify(this.GetText("SUBJECT_DUPLICATE"), MessageTypes.warning);
            return false;
        }

        if ((!this.PageBoardContext.IsGuest || !this.PageBoardContext.BoardSettings.EnableCaptchaForGuests)
            && (!this.PageBoardContext.BoardSettings.EnableCaptchaForPost || this.PageBoardContext.PageUser.UserFlags.IsCaptchaExcluded)
            || CaptchaHelper.IsValid(this.tbCaptcha.Text.Trim()))
        {
            return true;
        }

        this.PageBoardContext.Notify(this.GetText("BAD_CAPTCHA"), MessageTypes.danger);
        return false;
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnInit([NotNull] EventArgs e)
    {
        if (this.PageBoardContext.UploadAccess)
        {
            this.PageBoardContext.PageElements.AddScriptReference("FileUploadScript");

            this.PageBoardContext.PageElements.RegisterCssIncludeContent("jquery.fileupload.comb.min.css");
        }

        this.forumEditor = ForumEditorHelper.GetCurrentForumEditor();
        this.forumEditor.MaxCharacters = this.PageBoardContext.BoardSettings.MaxPostSize;

        this.EditorLine.Controls.Add(this.forumEditor);

        base.OnInit(e);
    }

    /// <summary>
    /// Registers the java scripts
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        // setup jQuery and Jquery Ui Tabs.
        this.PageBoardContext.PageElements.RegisterJsBlock(
            nameof(JavaScriptBlocks.GetBoardTagsJs),
            JavaScriptBlocks.GetBoardTagsJs("Tags", this.TagsValue.ClientID));

        base.OnPreRender(e);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.PageBoardContext.PageForumID == 0)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        if (!this.PageBoardContext.ForumPostAccess)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        this.Title.Text = this.GetText("NEWTOPIC");

        this.HandleUploadControls();

        if (this.IsPostBack)
        {
            return;
        }

        var normal = new ListItem(this.GetText("normal"), "0");

        normal.Attributes.Add(
            "data-content",
            $"<span class='select2-image-select-icon'><i class='far fa-comment fa-fw text-secondary me-1'></i>{this.GetText("normal")}</span>");

        this.Priority.Items.Add(normal);

        var sticky = new ListItem(this.GetText("sticky"), "1");

        sticky.Attributes.Add(
            "data-content",
            $"<span class='select2-image-select-icon'><i class='fas fa-thumbtack fa-fw text-secondary me-1'></i>{this.GetText("sticky")}</span>");

        this.Priority.Items.Add(sticky);

        var announcement = new ListItem(this.GetText("announcement"), "2");

        announcement.Attributes.Add(
            "data-content",
            $"<span class='select2-image-select-icon'><i class='fas fa-bullhorn fa-fw text-secondary me-1'></i>{this.GetText("announcement")}</span>");

        this.Priority.Items.Add(announcement);

        this.Priority.SelectedIndex = 0;

        // Allow the Styling of Topic Titles only for Mods or Admins
        if (this.PageBoardContext.BoardSettings.UseStyledTopicTitles
            && (this.PageBoardContext.ForumModeratorAccess || this.PageBoardContext.IsAdmin))
        {
            this.StyleRow.Visible = true;
        }
        else
        {
            this.StyleRow.Visible = false;
        }

        this.PriorityRow.Visible = this.PageBoardContext.ForumPriorityAccess;

        // update options...
        this.PostOptions1.Visible = !this.PageBoardContext.IsGuest;
        this.PostOptions1.PersistentOptionVisible =
            this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess;
        this.PostOptions1.WatchOptionVisible = !this.PageBoardContext.IsGuest;
        this.PostOptions1.PollOptionVisible = this.PageBoardContext.ForumPollAccess;

        if (!this.PageBoardContext.IsGuest)
        {
            this.PostOptions1.WatchChecked = this.PageBoardContext.PageTopicID > 0
                ? this.GetRepository<WatchTopic>().Check(this.PageBoardContext.PageUserID, this.PageBoardContext.PageTopicID).HasValue
                : this.PageBoardContext.PageUser.AutoWatchTopics;
        }

        if (this.PageBoardContext.IsGuest && this.PageBoardContext.BoardSettings.EnableCaptchaForGuests
            || this.PageBoardContext.BoardSettings.EnableCaptchaForPost && !this.PageBoardContext.PageUser.UserFlags.IsCaptchaExcluded)
        {
            this.imgCaptcha.ImageUrl = CaptchaHelper.GetCaptcha();
            this.tr_captcha1.Visible = true;
            this.tr_captcha2.Visible = true;
        }

        // enable similar topics search
        this.TopicSubjectTextBox.CssClass += " searchSimilarTopics";

        if (!this.PageBoardContext.IsGuest)
        {
            return;
        }

        // form user is only for "Guest"
        this.From.Text = this.PageBoardContext.PageUser.DisplayOrUserName();

        if (this.User != null)
        {
            this.FromRow.Visible = false;
        }
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);

        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);

        // add the "New Topic" page link last...
        this.PageBoardContext.PageLinks.AddLink(this.GetText("NEWTOPIC"));
    }

    /// <summary>
    /// The post reply handle new post.
    /// </summary>
    /// <returns>
    /// Returns the Message Id.
    /// </returns>
    protected Message PostReplyHandleNewTopic()
    {
        if (!this.PageBoardContext.ForumPostAccess)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        // Check if Forum is Moderated
        var isForumModerated = this.CheckForumModerateStatus(this.PageBoardContext.PageForum, true);

        // If Forum is Moderated
        if (isForumModerated)
        {
            this.spamApproved = false;
        }

        // Bypass Approval if Admin or Moderator
        if (this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess)
        {
            this.spamApproved = true;
        }

        // make message flags
        var messageFlags = new MessageFlags
        {
            IsHtml = false,
            IsBBCode = this.forumEditor.UsesBBCode,
            IsPersistent = this.PostOptions1.PersistentChecked,
            IsApproved = this.spamApproved
        };

        var messageText = this.forumEditor.Text;

        // Save to Db
        var newTopic = this.GetRepository<Topic>().SaveNew(
            this.PageBoardContext.PageForum,
            HtmlHelper.StripHtml(this.TopicSubjectTextBox.Text.Trim()),
            string.Empty,
            HtmlHelper.StripHtml(this.TopicStylesTextBox.Text.Trim()),
            HtmlHelper.StripHtml(this.TopicDescriptionTextBox.Text.Trim()),
            HtmlHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(messageText)),
            this.PageBoardContext.PageUser,
            this.Priority.SelectedValue.ToType<short>(),
            this.PageBoardContext.IsGuest ? this.From.Text : this.PageBoardContext.PageUser.Name,
            this.PageBoardContext.IsGuest ? this.From.Text : this.PageBoardContext.PageUser.DisplayName,
            this.Get<HttpRequestBase>().GetUserRealIPAddress(),
            DateTime.UtcNow,
            messageFlags,
            out var message);

        message.Topic = newTopic;

        this.UpdateWatchTopic(this.PageBoardContext.PageUserID, newTopic.ID);

        return message;
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

        var isPossibleSpamMessage = false;

        var message = HtmlHelper.StripHtml(this.forumEditor.Text);

        // Check for SPAM
        if (!this.PageBoardContext.IsAdmin && !this.PageBoardContext.ForumModeratorAccess)
        {
            // Check content for spam
            if (
                this.Get<ISpamCheck>().CheckPostForSpam(
                    this.PageBoardContext.IsGuest ? this.From.Text : this.PageBoardContext.PageUser.DisplayOrUserName(),
                    this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                    BBCodeHelper.StripBBCode(
                            HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(this.forumEditor.Text)))
                        .RemoveMultipleWhitespace(),
                    this.PageBoardContext.IsGuest ? null : this.PageBoardContext.MembershipUser.Email,
                    out var spamResult))
            {
                var description =
                    $@"Spam Check detected possible SPAM ({spamResult}) 
                           posted by PageUser: {(this.PageBoardContext.IsGuest ? "Guest" : this.PageBoardContext.PageUser.DisplayOrUserName())}";

                switch (this.PageBoardContext.BoardSettings.SpamPostHandling)
                {
                    case SpamPostHandling.DoNothing:
                        this.Logger.SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            description);
                        break;
                    case SpamPostHandling.FlagMessageUnapproved:
                        this.spamApproved = false;
                        isPossibleSpamMessage = true;
                        this.Logger.SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            $"{description}, it was flagged as unapproved post.");
                        break;
                    case SpamPostHandling.RejectMessage:
                        this.Logger.SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            $"S{description}, post was rejected");
                        this.PageBoardContext.Notify(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);
                        return;
                    case SpamPostHandling.DeleteBanUser:
                        this.Logger.SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            $"{description}, user was deleted and banned");

                        this.Get<IAspNetUsersHelper>().DeleteAndBanUser(
                            this.PageBoardContext.PageUserID,
                            this.PageBoardContext.MembershipUser,
                            this.PageBoardContext.PageUser.IP);

                        return;
                }
            }
        }

        // update the last post time...
        this.Get<ISession>().LastPost = DateTime.UtcNow.AddSeconds(30);

        // New Topic
        var newMessage = this.PostReplyHandleNewTopic();

        // Check if message is approved
        var isApproved = newMessage.MessageFlags.IsApproved;

        // vzrus^ the poll access controls are enabled and this is a new topic - we add the variables
        var attachPollParameter = this.PageBoardContext.ForumPollAccess;

        // Create notification emails
        if (isApproved)
        {
            this.Get<ISendNotification>().ToWatchingUsers(newMessage, true);

            if (!this.PageBoardContext.IsGuest && this.PageBoardContext.PageUser.Activity)
            {
                // Handle Mentions
                BBCodeHelper.FindMentions(message).ForEach(
                    user =>
                    {
                        var userId = this.Get<IUserDisplayName>().FindUserByName(user).ID;

                        if (userId != this.PageBoardContext.PageUserID)
                        {
                            this.Get<IActivityStream>().AddMentionToStream(
                                userId,
                                newMessage.TopicID,
                                newMessage.ID,
                                this.PageBoardContext.PageUserID);
                        }
                    });

                // Handle User Quoting
                BBCodeHelper.FindUserQuoting(message).ForEach(
                    user =>
                    {
                        var userId = this.Get<IUserDisplayName>().FindUserByName(user).ID;

                        if (userId != this.PageBoardContext.PageUserID)
                        {
                            this.Get<IActivityStream>().AddQuotingToStream(
                                userId,
                                newMessage.TopicID,
                                newMessage.ID,
                                this.PageBoardContext.PageUserID);
                        }
                    });

                this.Get<IActivityStream>().AddTopicToStream(
                    Config.IsDotNetNuke ? this.PageBoardContext.PageForumID : this.PageBoardContext.PageUserID,
                    newMessage.TopicID,
                    newMessage.ID,
                    HtmlHelper.StripHtml(this.TopicSubjectTextBox.Text),
                    message);

                // Add tags
                if (this.TagsValue.Value.IsSet())
                {
                    this.GetRepository<TopicTag>().AddTagsToTopic(this.TagsValue.Value, newMessage.TopicID);
                }
            }

            if (!attachPollParameter || !this.PostOptions1.PollChecked)
            {
                // regular redirect...
                this.Get<LinkBuilder>().Redirect(ForumPages.Posts, new {m = newMessage.ID, name = newMessage.Topic.TopicName});
            }
            else
            {
                // poll edit redirect...
                this.Get<LinkBuilder>().Redirect(ForumPages.PollEdit, new {t = newMessage.TopicID });
            }
        }
        else
        {
            // Not Approved
            if (this.PageBoardContext.BoardSettings.EmailModeratorsOnModeratedPost)
            {
                // not approved, notify moderators
                this.Get<ISendNotification>()
                    .ToModeratorsThatMessageNeedsApproval(
                        this.PageBoardContext.PageForumID,
                        newMessage.ID,
                        isPossibleSpamMessage);
            }

            // 't' variable is required only for poll and this is a attach poll token for attachments page
            if (!this.PostOptions1.PollChecked)
            {
                attachPollParameter = false;
            }

            // Tell user that his message will have to be approved by a moderator
            var url = this.Get<LinkBuilder>().GetForumLink(this.PageBoardContext.PageForum);

            if (!attachPollParameter)
            {
                this.Get<LinkBuilder>().Redirect(ForumPages.Info, new {i = 1, url = this.Server.UrlEncode(url)});
            }
            else
            {
                this.Get<LinkBuilder>().Redirect(
                    ForumPages.PollEdit,
                    new {ra = 1, t = newMessage.TopicID, f = this.PageBoardContext.PageForumID});
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

        this.PreviewMessagePost.MessageFlags = new MessageFlags
        {
            IsHtml = false,
            IsBBCode = this.forumEditor.UsesBBCode
        };

        this.PreviewMessagePost.MessageID = null;

        this.PreviewMessagePost.Message = this.forumEditor.Text;
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
        if (this.PostOptions1.WatchChecked)
        {
            // subscribe to this topic...
            this.GetRepository<WatchTopic>().Add(userId, topicId);
        }
    }

    /// <summary>
    /// Checks the forum moderate status.
    /// </summary>
    /// <param name="forumInfo">The forum information.</param>
    /// <param name="isNewTopic">if set to <c>true</c> [is new topic].</param>
    /// <returns>
    /// Returns if the forum needs to be moderated
    /// </returns>
    private bool CheckForumModerateStatus(Forum forumInfo, bool isNewTopic)
    {
        // User Moderate override
        if (this.PageBoardContext.PageUser.UserFlags.Moderated)
        {
            return true;
        }

        var forumModerated = forumInfo.ForumFlags.IsModerated;

        if (!forumModerated)
        {
            return false;
        }

        if (forumInfo.IsModeratedNewTopicOnly && !isNewTopic)
        {
            return false;
        }

        if (!forumInfo.ModeratedPostCount.HasValue || this.PageBoardContext.IsGuest)
        {
            return true;
        }

        var moderatedPostCount = forumInfo.ModeratedPostCount;

        return !(this.PageBoardContext.PageUser.NumPosts >= moderatedPostCount);
    }

    /// <summary>
    /// Handles the upload controls.
    /// </summary>
    private void HandleUploadControls()
    {
        this.forumEditor.UserCanUpload = this.PageBoardContext.UploadAccess;
        this.UploadDialog.Visible = this.PageBoardContext.UploadAccess;
    }
}