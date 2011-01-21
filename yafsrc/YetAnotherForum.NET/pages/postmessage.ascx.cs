/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2011 Jaben Cargman
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
  using System.Web;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.BBCode;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Flags;
  using YAF.Types.Interfaces;
  using YAF.Utilities;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// Summary description for postmessage.
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

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "postmessage" /> class.
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
    ///   Gets PollGroupId if the topic has a poll attached
    /// </summary>
    protected int? PollGroupId { get; set; }

    /// <summary>
    ///   Gets QuotedMessageID.
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
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
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
    /// The handle post to blog.
    /// </returns>
    protected string HandlePostToBlog([NotNull] string message, [NotNull] string subject)
    {
      string blogPostID = string.Empty;

      // Does user wish to post this to their blog?
      if (this.PageContext.BoardSettings.AllowPostToBlog && this.PostToBlog.Checked)
      {
        try
        {
          // Post to blog
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
          this.PageContext.AddLoadMessage(this.GetText("POSTTOBLOG_FAILED"));
        }
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
      if (!(this.PageContext.IsAdmin || this.PageContext.IsModerator) &&
          this.PageContext.BoardSettings.PostFloodDelay > 0)
      {
        // see if they've past that delay point
        if (YafContext.Current.Get<IYafSession>().LastPost >
            DateTime.UtcNow.AddSeconds(-this.PageContext.BoardSettings.PostFloodDelay) && this.EditMessageID == null)
        {
          this.PageContext.AddLoadMessage(
            this.GetTextFormatted(
              "wait", 
              (YafContext.Current.Get<IYafSession>().LastPost -
               DateTime.UtcNow.AddSeconds(-this.PageContext.BoardSettings.PostFloodDelay)).Seconds));
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Handles verification of the PostReply. Adds javascript message if there is a problem.
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
        this.PageContext.AddLoadMessage(this.GetText("ISEMPTY"));
        return false;
      }

      // No need to check whitespace if they are actually posting something
      if (this._forumEditor.Text.Length >= YafContext.Current.BoardSettings.MaxPostSize)
      {
        this.PageContext.AddLoadMessage(this.GetText("ISEXCEEDED"));
        return false;
      }

      if (this.SubjectRow.Visible && this.TopicSubjectTextBox.Text.IsNotSet())
      {
        this.PageContext.AddLoadMessage(this.GetText("NEED_SUBJECT"));
        return false;
      }
      if (this.Get<IPermissions>().Check(this.PageContext.BoardSettings.AllowCreateTopicsSameName) && LegacyDb.topic_findduplicate(this.TopicSubjectTextBox.Text.Trim()) == 1 && this.TopicID == null &&
          this.EditMessageID == null)
      {
        this.PageContext.AddLoadMessage(this.GetText("SUBJECT_DUPLICATE"));
        return false;
      }

      if (((this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableCaptchaForGuests) ||
           (this.PageContext.BoardSettings.EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded)) &&
          !CaptchaHelper.IsValid(this.tbCaptcha.Text.Trim()))
      {
        this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"));
        return false;
      }

      return true;
    }

    /// <summary>
    /// The new topic.
    /// </summary>
    /// <returns>
    /// The new topic.
    /// </returns>
    protected bool NewTopic()
    {
      return !(this.PollGroupId > 0);
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      // get the forum editor based on the settings
      this._forumEditor =
        this.PageContext.EditorModuleManager.GetBy(this.PageContext.BoardSettings.ForumEditor);
      this.EditorLine.Controls.Add(this._forumEditor);

      base.OnInit(e);
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.PageContext.QueryIDs = new QueryStringIDHelper(new[] { "m", "t", "q" }, false);

      DataRow currentRow = null;

      // we reply to a post with a quote
      if (this.QuotedMessageID != null)
      {
        currentRow = LegacyDb.message_list(this.QuotedMessageID).GetFirstRowOrInvalid();
        this.OriginalMessage = currentRow["Message"].ToString();

        if (Convert.ToInt32(currentRow["TopicID"]) != this.PageContext.PageTopicID)
        {
          YafBuildLink.AccessDenied();
        }

        if (!this.CanQuotePostCheck(currentRow))
        {
          YafBuildLink.AccessDenied();
        }

        this.PollGroupId = currentRow["PollID"].IsNullOrEmptyDBField() ? 0 : Convert.ToInt32(currentRow["PollID"]);

        // if this is a quoted message (a new reply with a quote)  we should transfer the TopicId value only to return
        this.PollList.TopicId = (int)this.TopicID;

        if (this.TopicID == null)
        {
          this.PollList.TopicId = currentRow["TopicID"].IsNullOrEmptyDBField()
                                    ? 0
                                    : Convert.ToInt32(currentRow["TopicID"]);
        }
      }
      else if (this.EditMessageID != null)
      {
        currentRow = LegacyDb.message_list(this.EditMessageID).GetFirstRowOrInvalid();
        this.OriginalMessage = currentRow["Message"].ToString();

        this._ownerUserId = Convert.ToInt32(currentRow["UserId"]);

        if (!this.CanEditPostCheck(currentRow))
        {
          YafBuildLink.AccessDenied();
        }

        this.PollGroupId = currentRow["PollID"].IsNullOrEmptyDBField() ? 0 : Convert.ToInt32(currentRow["PollID"]);

        // we edit message and should transfer both the message ID and TopicID for PageLinks. 
        this.PollList.EditMessageId = (int)this.EditMessageID;

        if (this.TopicID == null)
        {
          this.PollList.TopicId = currentRow["TopicID"].IsNullOrEmptyDBField()
                                    ? 0
                                    : Convert.ToInt32(currentRow["TopicID"]);
        }
      }

      if (this.PageContext.PageForumID == 0)
      {
        YafBuildLink.AccessDenied();
      }

      if (this.Get<HttpRequestBase>()["t"] == null && this.Get<HttpRequestBase>()["m"] == null &&
          !this.PageContext.ForumPostAccess)
      {
        YafBuildLink.AccessDenied();
      }

      if (this.Get<HttpRequestBase>()["t"] != null && !this.PageContext.ForumReplyAccess)
      {
        YafBuildLink.AccessDenied();
      }

      // Message.EnableRTE = PageContext.BoardSettings.AllowRichEdit;
      this._forumEditor.StyleSheet = this.PageContext.Get<ITheme>().BuildThemePath("theme.css");
      this._forumEditor.BaseDir = YafForumInfo.ForumClientFileRoot + "editors";

      this.Title.Text = this.GetText("NEWTOPIC");
      this.LocalizedLblMaxNumberOfPost.Param0 = YafContext.Current.BoardSettings.MaxPostSize.ToString();

      if (!this.IsPostBack)
      {
        // helper bool -- true if this is a completely new topic...
        bool isNewTopic = (this.TopicID == null) && (this.QuotedMessageID == null) && (this.EditMessageID == null);

        this.Priority.Items.Add(new ListItem(this.GetText("normal"), "0"));
        this.Priority.Items.Add(new ListItem(this.GetText("sticky"), "1"));
        this.Priority.Items.Add(new ListItem(this.GetText("announcement"), "2"));
        this.Priority.SelectedIndex = 0;

        this.EditReasonRow.Visible = false;

        this.PriorityRow.Visible = this.PageContext.ForumPriorityAccess;

        // Show post to blog option only to a new post
        this.BlogRow.Visible = this.PageContext.BoardSettings.AllowPostToBlog && isNewTopic && !this.PageContext.IsGuest;

        // update options...
        this.PostOptions1.Visible = !this.PageContext.IsGuest;
        this.PostOptions1.PersistantOptionVisible = this.PageContext.ForumPriorityAccess;
        this.PostOptions1.AttachOptionVisible = this.PageContext.ForumUploadAccess;
        this.PostOptions1.WatchOptionVisible = !this.PageContext.IsGuest;
        this.PostOptions1.PollOptionVisible = this.PageContext.ForumPollAccess && isNewTopic;

        if (!this.PageContext.IsGuest)
        {
          if (this.PageContext.PageTopicID > 0)
          {
            this.PostOptions1.WatchChecked =
              this.TopicWatchedId(this.PageContext.PageUserID, this.PageContext.PageTopicID).HasValue;
          }
          else
          {
            this.PostOptions1.WatchChecked = new CombinedUserDataHelper(this.PageContext.PageUserID).AutoWatchTopics;
          }
        }

        if ((this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableCaptchaForGuests) ||
            (this.PageContext.BoardSettings.EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded))
        {
          this.imgCaptcha.ImageUrl = "{0}resource.ashx?c=1".FormatWith(YafForumInfo.ForumClientFileRoot);
          this.tr_captcha1.Visible = true;
          this.tr_captcha2.Visible = true;
        }

        if (this.PageContext.Settings.LockedForum == 0)
        {
          this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
          this.PageLinks.AddLink(
            this.PageContext.PageCategoryName, 
            YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
        }

        this.PageLinks.AddForumLinks(this.PageContext.PageForumID);

        // check if it's a reply to a topic...
        if (this.TopicID != null)
        {
          this.InitReplyToTopic();

          this.PollList.TopicId = (int)this.TopicID;
        }

        // If currentRow != null, we are quoting a post in a new reply, or editing an existing post
        if (currentRow != null)
        {
          var messageFlags = new MessageFlags(currentRow["Flags"]);
          string message = currentRow["Message"].ToString();
          this.OriginalMessage = currentRow["Message"].ToString();

          if (this.QuotedMessageID != null)
          {
            // quoting a reply to a topic...
            this.InitQuotedReply(currentRow, message, messageFlags);
            this.PollList.TopicId = (int)this.TopicID;
          }
          else if (this.EditMessageID != null)
          {
            // editing a message...
            this.InitEditedPost(currentRow, message, messageFlags);
            this.PollList.EditMessageId = (int)this.EditMessageID;
          }

          this.PollGroupId = currentRow["PollID"].IsNullOrEmptyDBField() ? 0 : Convert.ToInt32(currentRow["PollID"]);
        }

        // add the "New Topic" page link last...
        if (isNewTopic)
        {
          this.PageLinks.AddLink(this.GetText("NEWTOPIC"));
        }

        // form user is only for "Guest"
        this.From.Text = this.PageContext.PageUserName;
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
    /// The post reply handle edit post.
    /// </returns>
    protected long PostReplyHandleEditPost()
    {
      long messageId = 0;

      if (!this.PageContext.ForumEditAccess)
      {
        YafBuildLink.AccessDenied();
      }

      string subjectSave = string.Empty;

      if (this.TopicSubjectTextBox.Enabled)
      {
        subjectSave = this.TopicSubjectTextBox.Text;
      }

      // Mek Suggestion: This should be removed, resetting flags on edit is a bit lame.
      // Ederon : now it should be better, but all this code around forum/topic/message flags needs revamp
      // retrieve message flags
      var messageFlags = new MessageFlags(LegacyDb.message_list(this.EditMessageID).Rows[0]["Flags"])
        {
          IsHtml = this._forumEditor.UsesHTML, 
          IsBBCode = this._forumEditor.UsesBBCode, 
          IsPersistent = this.PostOptions1.PersistantChecked
        };

      bool isModeratorChanged = this.PageContext.PageUserID != this._ownerUserId;
      LegacyDb.message_update(
        this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"), 
        this.Priority.SelectedValue, 
        this._forumEditor.Text.Trim(), 
        subjectSave.Trim(), 
        messageFlags.BitValue, 
        this.HtmlEncode(this.ReasonEditor.Text), 
        isModeratorChanged, 
        this.PageContext.IsAdmin || this.PageContext.IsModerator, 
        this.OriginalMessage, 
        this.PageContext.PageUserID);

      messageId = this.EditMessageID.Value;

      this.HandlePostToBlog(this._forumEditor.Text, this.TopicSubjectTextBox.Text);

      // remove cache if it exists...
      this.PageContext.Cache.Remove(
        Constants.Cache.FirstPostCleaned.FormatWith(this.PageContext.PageBoardID, this.TopicID));

      return messageId;
    }

    /// <summary>
    /// The post reply handle new post.
    /// </summary>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    /// <returns>
    /// The post reply handle new post.
    /// </returns>
    protected long PostReplyHandleNewPost(out long topicId)
    {
      long messageId = 0;

      if (!this.PageContext.ForumPostAccess)
      {
        YafBuildLink.AccessDenied();
      }

      // make message flags
      var messageFlags = new MessageFlags
        {
          IsHtml = this._forumEditor.UsesHTML, 
          IsBBCode = this._forumEditor.UsesBBCode, 
          IsPersistent = this.PostOptions1.PersistantChecked, 
          /* Bypass Approval if Admin or Moderator.*/
          IsApproved = this.PageContext.IsAdmin || this.PageContext.IsModerator
        };

      string blogPostID = this.HandlePostToBlog(this._forumEditor.Text, this.TopicSubjectTextBox.Text);

      // Save to Db
      topicId = LegacyDb.topic_save(
        this.PageContext.PageForumID, 
        this.TopicSubjectTextBox.Text.Trim(), 
        this._forumEditor.Text, 
        this.PageContext.PageUserID, 
        this.Priority.SelectedValue, 
        this.User != null ? null : this.From.Text, 
        this.Get<HttpRequestBase>().UserHostAddress, 
        null, 
        blogPostID, 
        messageFlags.BitValue, 
        ref messageId);

      this.UpdateWatchTopic(this.PageContext.PageUserID, (int)topicId);

      // clear caches as stats changed
      if (messageFlags.IsApproved)
      {
        this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.BoardStats));
        this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.BoardUserStats));
      }

      return messageId;
    }

    /// <summary>
    /// The post reply handle reply to topic.
    /// </summary>
    /// <returns>
    /// The post reply handle reply to topic.
    /// </returns>
    protected long PostReplyHandleReplyToTopic()
    {
      long messageId = 0;

      if (!this.PageContext.ForumReplyAccess)
      {
        YafBuildLink.AccessDenied();
      }

      object replyTo = (this.QuotedMessageID != null) ? this.QuotedMessageID.Value : -1;

      // make message flags
      var messageFlags = new MessageFlags();

      messageFlags.IsHtml = this._forumEditor.UsesHTML;
      messageFlags.IsBBCode = this._forumEditor.UsesBBCode;
      messageFlags.IsPersistent = this.PostOptions1.PersistantChecked;
      messageFlags.IsApproved = this.PageContext.IsAdmin || this.PageContext.IsModerator;

      LegacyDb.message_save(
        this.TopicID.Value, 
        this.PageContext.PageUserID, 
        this._forumEditor.Text, 
        this.User != null ? null : this.From.Text, 
        this.Get<HttpRequestBase>().UserHostAddress, 
        null, 
        replyTo, 
        messageFlags.BitValue, 
        ref messageId);

      this.UpdateWatchTopic(this.PageContext.PageUserID, this.PageContext.PageTopicID);

      if (messageFlags.IsApproved)
      {
        this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.BoardStats));
        this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.BoardUserStats));
      }

      return messageId;
    }

    /// <summary>
    /// Handles the PostReply click including: Replying, Editing and New post.
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="e">
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

      // Check if the topic name is not too long
      if (YafContext.Current.BoardSettings.MaxWordLength > 0 &&
          this.TopicSubjectTextBox.Text.Trim().AreAnyWordsOverMaxLength(YafContext.Current.BoardSettings.MaxWordLength))
      {
        this.PageContext.AddLoadMessage(
          this.GetTextFormatted("TOPICNAME_TOOLONG", this.PageContext.BoardSettings.MaxWordLength));
        return;
      }

      // vzrus: Common users should not use HTML tags in a topic header if not allowed
      if (!(this.PageContext.IsModerator || this.PageContext.IsForumModerator || this.PageContext.IsAdmin))
      {
        string tag = this.Get<IFormatMessage>().CheckHtmlTags(
          this.TopicSubjectTextBox.Text, this.PageContext.BoardSettings.AcceptedHeadersHTML, ',');

        if (tag.IsSet())
        {
          this.PageContext.AddLoadMessage(tag);
          return;
        }
      }

      // update the last post time...
      YafContext.Current.Get<IYafSession>().LastPost = DateTime.UtcNow.AddSeconds(30);

      long messageId = 0;
      long newTopic = 0;

      if (this.TopicID != null)
      {
        // Reply to topic
        messageId = this.PostReplyHandleReplyToTopic();
        newTopic = (long)this.TopicID;
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
            YafBuildLink.Redirect(ForumPages.posts, "m={0}&#post{0}", messageId);
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
        if (this.PageContext.BoardSettings.EmailModeratorsOnModeratedPost)
        {
          // not approved, notifiy moderators
          this.Get<ISendNotification>().ToModeratorsThatMessageNeedsApproval(
            this.PageContext.PageForumID, (int)messageId);
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
          // PageContext.AddLoadMessage("Since you posted to a moderated forum, a forum moderator must approve your post before it will become visible.");
          string url = YafBuildLink.GetLink(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
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
    /// The preview_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Preview_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.PreviewRow.Visible = true;

      this.PreviewMessagePost.MessageFlags.IsHtml = this._forumEditor.UsesHTML;
      this.PreviewMessagePost.MessageFlags.IsBBCode = this._forumEditor.UsesBBCode;
      this.PreviewMessagePost.Message = this._forumEditor.Text;

      if (this.PageContext.BoardSettings.AllowSignatures)
      {
        using (DataTable userDt = LegacyDb.user_list(this.PageContext.PageBoardID, this.PageContext.PageUserID, true))
        {
          if (userDt.Rows.Count > 0)
          {
            DataRow userRow = userDt.Rows[0];

            if (!userRow.IsNull("Signature"))
            {
              this.PreviewMessagePost.Signature = userRow["Signature"].ToString();
            }
          }
        }
      }
    }

    /// <summary>
    /// The can edit post check.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <returns>
    /// The can edit post check.
    /// </returns>
    private bool CanEditPostCheck([NotNull] DataRow message)
    {
      bool postLocked = false;

      if (!this.PageContext.IsAdmin && this.PageContext.BoardSettings.LockPosts > 0)
      {
        var edited = (DateTime)message["Edited"];

        if (edited.AddDays(this.PageContext.BoardSettings.LockPosts) < DateTime.UtcNow)
        {
          postLocked = true;
        }
      }

      DataRow forumInfo, topicInfo;

      // get topic and forum information
      topicInfo = LegacyDb.topic_info(this.PageContext.PageTopicID);
      using (DataTable dt = LegacyDb.forum_list(this.PageContext.PageBoardID, this.PageContext.PageForumID))
      {
        forumInfo = dt.Rows[0];
      }

      // Ederon : 9/9/2007 - moderator can edit in locked topics
      return ((!postLocked && !forumInfo["Flags"].BinaryAnd(ForumFlags.Flags.IsLocked) &&
               !topicInfo["Flags"].BinaryAnd(TopicFlags.Flags.IsLocked) &&
               (SqlDataLayerConverter.VerifyInt32(message["UserID"]) == this.PageContext.PageUserID)) ||
              this.PageContext.ForumModeratorAccess) && this.PageContext.ForumEditAccess;
    }

    /// <summary>
    /// The can have poll.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <returns>
    /// The can have poll.
    /// </returns>
    private bool CanHavePoll([NotNull] DataRow message)
    {
      return (this.TopicID == null && this.QuotedMessageID == null && this.EditMessageID == null) ||
             (message != null && SqlDataLayerConverter.VerifyInt32(message["Position"]) == 0);
    }

    /// <summary>
    /// The can quote post check.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <returns>
    /// The can quote post check.
    /// </returns>
    private bool CanQuotePostCheck([NotNull] DataRow message)
    {
      DataRow forumInfo, topicInfo;

      // get topic and forum information
      topicInfo = LegacyDb.topic_info(this.PageContext.PageTopicID);
      using (DataTable dt = LegacyDb.forum_list(this.PageContext.PageBoardID, this.PageContext.PageForumID))
      {
        forumInfo = dt.Rows[0];
      }

      if (topicInfo == null || forumInfo == null)
      {
        return false;
      }

      // Ederon : 9/9/2007 - moderator can reply to locked topics
      return (!forumInfo["Flags"].BinaryAnd(ForumFlags.Flags.IsLocked) &&
              !topicInfo["Flags"].BinaryAnd(TopicFlags.Flags.IsLocked) || this.PageContext.ForumModeratorAccess) &&
             this.PageContext.ForumReplyAccess;
    }

    /// <summary>
    /// The get poll id.
    /// </summary>
    /// <returns>
    /// The get poll id.
    /// </returns>
    [CanBeNull]
    private object GetPollID()
    {
      return null;
    }

    /// <summary>
    /// The has poll.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <returns>
    /// The has poll.
    /// </returns>
    private bool HasPoll([NotNull] DataRow message)
    {
      return message != null && message["PollID"] != DBNull.Value && message["PollID"] != null;
    }

    /// <summary>
    /// The init edited post.
    /// </summary>
    /// <param name="currentRow">
    /// The current row.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="messageFlags">
    /// The message flags.
    /// </param>
    private void InitEditedPost(
      [NotNull] DataRow currentRow, [NotNull] string message, [NotNull] MessageFlags messageFlags)
    {
      // If the message is in YafBBCode but the editor uses HTML, convert the message text to HTML
      if (messageFlags.IsBBCode && this._forumEditor.UsesHTML)
      {
        message = this.Get<IBBCode>().ConvertBBCodeToHtmlForEdit(message);
      }

      this._forumEditor.Text = message;

      this.Title.Text = this.GetText("EDIT");

      // add topic link...
      this.PageLinks.AddLink(
        this.Server.HtmlDecode(currentRow["Topic"].ToString()), 
        YafBuildLink.GetLink(ForumPages.posts, "m={0}", this.EditMessageID));

      // editing..
      this.PageLinks.AddLink(this.GetText("EDIT"));

      string blogPostID = currentRow["BlogPostID"].ToString();
      if (blogPostID != string.Empty)
      {
        // The user used this post to blog
        this.BlogPostID.Value = blogPostID;
        this.PostToBlog.Checked = true;
        this.BlogRow.Visible = true;
      }

      this.TopicSubjectTextBox.Text = this.Server.HtmlDecode(Convert.ToString(currentRow["Topic"]));

      if ((Convert.ToInt32(currentRow["TopicOwnerID"]) == Convert.ToInt32(currentRow["UserID"])) ||
          this.PageContext.ForumModeratorAccess)
      {
        // allow editing of the topic subject
        this.TopicSubjectTextBox.Enabled = true;
      }
      else
      {
        // disable the subject
        this.TopicSubjectTextBox.Enabled = false;
      }

      this.Priority.SelectedItem.Selected = false;
      this.Priority.Items.FindByValue(currentRow["Priority"].ToString()).Selected = true;
      this.EditReasonRow.Visible = true;
      this.ReasonEditor.Text = this.Server.HtmlDecode(Convert.ToString(currentRow["EditReason"]));
      this.PostOptions1.PersistantChecked = messageFlags.IsPersistent;
    }

    /// <summary>
    /// The init quoted reply.
    /// </summary>
    /// <param name="currentRow">
    /// The current row.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="messageFlags">
    /// The message flags.
    /// </param>
    private void InitQuotedReply(
      [NotNull] DataRow currentRow, [NotNull] string message, [NotNull] MessageFlags messageFlags)
    {
      if (this.PageContext.BoardSettings.RemoveNestedQuotes)
      {
        message = this.Get<IFormatMessage>().RemoveNestedQuotes(message);
      }

      // If the message being quoted in YafBBCode but the editor uses HTML, convert the message text to HTML
      if (messageFlags.IsBBCode && this._forumEditor.UsesHTML)
      {
        message = this.Get<IBBCode>().ConvertBBCodeToHtmlForEdit(message);
      }

      // Ensure quoted replies have bad words removed from them
      message = this.Get<IBadWordReplace>().Replace(message);

      // Quote the original message
      this._forumEditor.Text =
        "[quote={0};{1}]{2}[/quote]\n".FormatWith(
          this.PageContext.Get<IUserDisplayName>().GetName(currentRow.Field<int>("UserID")), 
          currentRow.Field<int>("MessageID"), 
          message).TrimStart();
    }

    /// <summary>
    /// The init reply to topic.
    /// </summary>
    private void InitReplyToTopic()
    {
      DataRow topic = LegacyDb.topic_info(this.TopicID);
      var topicFlags = new TopicFlags(SqlDataLayerConverter.VerifyInt32(topic["Flags"]));

      // Ederon : 9/9/2007 - moderators can reply in locked topics
      if (topicFlags.IsLocked && !this.PageContext.ForumModeratorAccess)
      {
        this.Response.Redirect(this.Get<HttpRequestBase>().UrlReferrer.ToString());
      }

      this.SubjectRow.Visible = false;
      this.Title.Text = this.GetText("reply");

      // add topic link...
      this.PageLinks.AddLink(
        this.Server.HtmlDecode(topic["Topic"].ToString()), YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.TopicID));

      // add "reply" text...
      this.PageLinks.AddLink(this.GetText("reply"));

      // show attach file option if its a reply...
      if (this.PageContext.ForumUploadAccess)
      {
        this.PostOptions1.Visible = true;
        this.PostOptions1.AttachOptionVisible = true;
      }

      // show the last posts AJAX frame...
      this.LastPosts1.Visible = true;
      this.LastPosts1.TopicID = this.TopicID.Value;
    }

    /// <summary>
    /// Returns true if the topic is set to watch for userId
    /// </summary>
    /// <param name="userId">
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    /// <returns>
    /// </returns>
    private int? TopicWatchedId(int userId, int topicId)
    {
      return LegacyDb.watchtopic_check(userId, topicId).GetFirstRowColumnAsValue<int?>("WatchTopicID", null);
    }

    /// <summary>
    /// Updates Watch Topic based on controls/settings for user...
    /// </summary>
    /// <param name="userId">
    /// </param>
    /// <param name="topicId">
    /// </param>
    private void UpdateWatchTopic(int userId, int topicId)
    {
      var topicWatchedID = this.TopicWatchedId(userId, topicId);

      if (topicWatchedID.HasValue && !this.PostOptions1.WatchChecked)
      {
        // unsubscribe...
        LegacyDb.watchtopic_delete(topicWatchedID.Value);
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
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    private void WatchTopic(int userId, int topicId)
    {
      if (!this.TopicWatchedId(userId, topicId).HasValue)
      {
        // subscribe to this forum
        LegacyDb.watchtopic_add(userId, topicId);
      }
    }

    #endregion
  }
}