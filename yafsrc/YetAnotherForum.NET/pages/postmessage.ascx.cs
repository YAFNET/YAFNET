/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.UI;
  using YAF.Classes.Utils;
  using YAF.Controls;
  using YAF.Editors;
  using YAF.Utilities;

  #endregion

  /// <summary>
  /// Summary description for postmessage.
  /// </summary>
  public partial class postmessage : ForumPage
  {
    #region Constants and Fields

    /// <summary>
    /// The _forum editor.
    /// </summary>
    protected BaseForumEditor _forumEditor;    

    /// <summary>
    /// The _owner user id.
    /// </summary>
    protected int _ownerUserId;

    /// <summary>
    /// The _ux no edit subject.
    /// </summary>
    protected Label _uxNoEditSubject;

    /// <summary>
    /// The original message.
    /// </summary>
    protected string _originalMessage;

    /// <summary>
    /// Table with choices
    /// </summary>
     protected DataTable choices;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="postmessage"/> class.
    /// </summary>
    public postmessage()
      : base("POSTMESSAGE")
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets EditTopicID.
    /// </summary>
    protected long? EditTopicID
    {
      get
      {
        return this.PageContext.QueryIDs["m"];
      }
    }

    /// <summary>
    /// Gets QuotedTopicID.
    /// </summary>
    protected long? QuotedTopicID
    {
      get
      {
        return this.PageContext.QueryIDs["q"];
      }
    }

    /// <summary>
    /// Gets TopicID.
    /// </summary>
    protected long? TopicID
    {
      get
      {
        return this.PageContext.QueryIDs["t"];
      }
    }

    /// <summary>
    /// Gets or sets OriginalMessage.
    /// </summary>
    protected string OriginalMessage
    {
        get
        {
            return _originalMessage;
        }

        set
        {
            _originalMessage = value;
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
    protected void Cancel_Click(object sender, EventArgs e)
    {
      if (this.TopicID != null || this.EditTopicID != null)
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
    /// The change poll show status.
    /// </summary>
    /// <param name="newStatus">
    /// The new status.
    /// </param>
    protected void ChangePollShowStatus(bool newStatus)
    {
      this.CreatePollRow.Visible = !newStatus;
      this.RemovePollRow.Visible = newStatus;
      this.PollRowExpire.Visible = newStatus;
      this.ChoiceRepeater.Visible = newStatus;
    
        var pollRow = (HtmlTableRow)this.FindControl(String.Format("PollRow{0}", 1));

        if (pollRow != null)
        {
          pollRow.Visible = newStatus;
        }      
    }

    /// <summary>
    /// The create poll_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void CreatePoll_Click(object sender, EventArgs e)
    {
      this.ChangePollShowStatus(true);

      // clear the fields...
      this.PollExpire.Text = string.Empty;
      this.Question.Text = string.Empty;

      // Add dummy table
      choices = new DataTable();
      choices.Columns.Add("ChoiceID", typeof(int));
      choices.Columns.Add("Choice", typeof(string));
      choices.Columns.Add("ChoiceOrderID", typeof(int));
      for (int i = 1; i <= PageContext.BoardSettings.AllowedPollChoiceNumber; i++)
      {
          DataRow newChoiceRow = choices.NewRow();
          newChoiceRow["ChoiceOrderID"] = i;
          choices.Rows.Add(newChoiceRow);
      }
      this.ChoiceRepeater.DataSource = choices;
      this.ChoiceRepeater.DataBind();

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
    protected string HandlePostToBlog(string message, string subject)
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
        if (Mession.LastPost > DateTime.Now.AddSeconds(-this.PageContext.BoardSettings.PostFloodDelay) &&
            this.EditTopicID == null)
        {
          this.PageContext.AddLoadMessage(
            this.GetTextFormatted(
              "wait", 
              (Mession.LastPost - DateTime.Now.AddSeconds(-this.PageContext.BoardSettings.PostFloodDelay)).Seconds));
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

      if (postedMessage.Length == 0)
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

      if (this.SubjectRow.Visible && this.Subject.Text.Length <= 0)
      {
        this.PageContext.AddLoadMessage(this.GetText("NEED_SUBJECT"));
        return false;
      }

      if (this.PollRow1.Visible)
      {
        if (this.Question.Text.Trim().Length == 0)
        {
          this.PageContext.AddLoadMessage(this.GetText("NEED_QUESTION"));
          return false;
        }

        int notNullcount = 0;  
        foreach (RepeaterItem ri in this.ChoiceRepeater.Items)
        {
           if (!string.IsNullOrEmpty(((TextBox)ri.FindControl("PollChoice")).Text.Trim()))
           {
               notNullcount++;
           }
        }
       
        if (notNullcount < 2)
        {
          this.PageContext.AddLoadMessage(this.GetText("NEED_CHOICES"));
          return false;
        }
      }

      if (((this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableCaptchaForGuests) ||
           (this.PageContext.BoardSettings.EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded)) &&
          this.Session["CaptchaImageText"].ToString() != this.tbCaptcha.Text.Trim())
      {
        this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"));
        return false;
      }

      return true;
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      // get the forum editor based on the settings
      this._forumEditor =
        this.PageContext.EditorModuleManager.GetEditorInstance(this.PageContext.BoardSettings.ForumEditor);
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
    protected void Page_Load(object sender, EventArgs e)
    {
      this.PageContext.QueryIDs = new QueryStringIDHelper(new[] { "m", "t", "q" }, false);

      DataRow currentRow = null;

      if (this.QuotedTopicID != null)
      {
        currentRow = DBHelper.GetFirstRowOrInvalid(DB.message_list(this.QuotedTopicID));
        this.OriginalMessage = currentRow["Message"].ToString();
        if (Convert.ToInt32(currentRow["TopicID"]) != this.PageContext.PageTopicID)
        {
          YafBuildLink.AccessDenied();
        }

        if (!this.CanQuotePostCheck(currentRow))
        {
          YafBuildLink.AccessDenied();
        }
      }
      else if (this.EditTopicID != null)
      {
        currentRow = DBHelper.GetFirstRowOrInvalid(DB.message_list(this.EditTopicID));
        this.OriginalMessage = currentRow["Message"].ToString();
        this._ownerUserId = Convert.ToInt32(currentRow["UserId"]);

        if (!this.CanEditPostCheck(currentRow))
        {
          YafBuildLink.AccessDenied();
        }
      }

      if (this.PageContext.PageForumID == 0)
      {
        YafBuildLink.AccessDenied();
      }

      if (this.Request["t"] == null && this.Request["m"] == null && !this.PageContext.ForumPostAccess)
      {
        YafBuildLink.AccessDenied();
      }

      if (this.Request["t"] != null && !this.PageContext.ForumReplyAccess)
      {
        YafBuildLink.AccessDenied();
      }

      // Message.EnableRTE = PageContext.BoardSettings.AllowRichEdit;
      this._forumEditor.StyleSheet = this.PageContext.Theme.BuildThemePath("theme.css");
      this._forumEditor.BaseDir = YafForumInfo.ForumClientFileRoot + "editors";

      this.Title.Text = this.GetText("NEWTOPIC");
      this.PollExpire.Attributes.Add("style", "width:50px");
      this.LocalizedLblMaxNumberOfPost.Param0 = YafContext.Current.BoardSettings.MaxPostSize.ToString();
      
      if (!this.IsPostBack)
      {
        // helper bool -- true if this is a completely new topic...
        bool isNewTopic = (this.TopicID == null) && (this.QuotedTopicID == null) && (this.EditTopicID == null);

        this.Priority.Items.Add(new ListItem(this.GetText("normal"), "0"));
        this.Priority.Items.Add(new ListItem(this.GetText("sticky"), "1"));
        this.Priority.Items.Add(new ListItem(this.GetText("announcement"), "2"));
        this.Priority.SelectedIndex = 0;

        this.EditReasonRow.Visible = false;

        this.PriorityRow.Visible = this.PageContext.ForumPriorityAccess;
        this.CreatePollRow.Visible = !this.HasPoll(currentRow) && this.CanHavePoll(currentRow) &&
                                     this.PageContext.ForumPollAccess;
        this.RemovePollRow.Visible = this.HasPoll(currentRow) && this.CanHavePoll(currentRow) &&
                                     this.PageContext.ForumPollAccess && this.PageContext.ForumModeratorAccess;

        if (this.RemovePollRow.Visible)
        {
          this.InitPollUI(currentRow);
        }

        // Show post to blog option only to a new post
        this.BlogRow.Visible = this.PageContext.BoardSettings.AllowPostToBlog && isNewTopic && !this.PageContext.IsGuest;

        // update options...
        this.PostOptions1.Visible = !this.PageContext.IsGuest;
        this.PostOptions1.PersistantOptionVisible = this.PageContext.ForumPriorityAccess;
        this.PostOptions1.AttachOptionVisible = this.PageContext.ForumUploadAccess;
        this.PostOptions1.WatchOptionVisible = !this.PageContext.IsGuest;

        if (!this.PageContext.IsGuest && this.PageContext.PageTopicID == 0)
        {
          this.PostOptions1.WatchChecked = new CombinedUserDataHelper(this.PageContext.PageUserID).AutoWatchTopics;
        }
        else if (!this.PageContext.IsGuest && this.PageContext.PageTopicID > 0)
        {
          this.PostOptions1.WatchChecked =
            this.TopicWatchedId(this.PageContext.PageUserID, this.PageContext.PageTopicID).HasValue;
        }

        if ((this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableCaptchaForGuests) ||
            (this.PageContext.BoardSettings.EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded))
        {
          this.Session["CaptchaImageText"] = CaptchaHelper.GetCaptchaString();
          this.imgCaptcha.ImageUrl = String.Format("{0}resource.ashx?c=1", YafForumInfo.ForumClientFileRoot);
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
        }

        // If currentRow != null, we are quoting a post in a new reply, or editing an existing post
        if (currentRow != null)
        {
          var messageFlags = new MessageFlags(currentRow["Flags"]);
          string message = currentRow["Message"].ToString();
          this.OriginalMessage = currentRow["Message"].ToString();

          if (this.QuotedTopicID != null)
          {
            // quoting a reply to a topic...
            this.InitQuotedReply(currentRow, message, messageFlags);
          }
          else if (this.EditTopicID != null)
          {
            // editing a message...
            this.InitEditedPost(currentRow, message, messageFlags);
          }
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
      }
    }

    /// <summary>
    /// Handles the PostReply click including: Replying, Editing and New post.
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="e">
    /// </param>
    protected void PostReply_Click(object sender, EventArgs e)
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
      if (!FormatMsg.WordLengthChecker(this.Subject.Text))
      {
          this.PageContext.AddLoadMessage(this.GetTextFormatted("TOPICNAME_TOOLONG", this.PageContext.BoardSettings.MaxWordLength));
          return;
      }    
         
      // update the last post time...
      Mession.LastPost = DateTime.Now.AddSeconds(30);

      long messageId = 0;
     
      if (this.TopicID != null)
      {
        // Reply to topic
        messageId = this.PostReplyHandleReplyToTopic();
      }
      else if (this.EditTopicID != null)
      {
        // Edit existing post
        messageId = this.PostReplyHandleEditPost();
      }
      else
      {
        // New post
        messageId = this.PostReplyHandleNewPost();
      }

      // Check if message is approved
      bool isApproved = false;
      using (DataTable dt = DB.message_list(messageId))
      {
        foreach (DataRow row in dt.Rows)
        {
          isApproved = General.BinaryAnd(row["Flags"], MessageFlags.Flags.IsApproved);
        }
      }

      // Create notification emails
      if (isApproved)
      {
        CreateMail.WatchEmail(messageId);

        if (this.PageContext.ForumUploadAccess && this.PostOptions1.AttachChecked)
        {
          // redirect to the attachment page...
          YafBuildLink.Redirect(ForumPages.attachments, "m={0}", messageId);
        }
        else
        {
          // regular redirect...
          YafBuildLink.Redirect(ForumPages.posts, "m={0}&#post{0}", messageId);
        }
      }
      else
      {
        // Tell user that his message will have to be approved by a moderator
        // PageContext.AddLoadMessage("Since you posted to a moderated forum, a forum moderator must approve your post before it will become visible.");
        string url = YafBuildLink.GetLink(ForumPages.topics, "f={0}", this.PageContext.PageForumID);

        if (Config.IsRainbow)
        {
          YafBuildLink.Redirect(ForumPages.info, "i=1");
        }
        else
        {
          YafBuildLink.Redirect(ForumPages.info, "i=1&url={0}", this.Server.UrlEncode(url));
        }
      }
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

      if (this.Subject.Enabled)
      {
        subjectSave = this.HtmlEncode(this.Subject.Text);
      }

      // Mek Suggestion: This should be removed, resetting flags on edit is a bit lame.
      // Ederon : now it should be better, but all this code around forum/topic/message flags needs revamp
      // retrieve message flags
      var messageFlags = new MessageFlags(DB.message_list(this.EditTopicID).Rows[0]["Flags"])
        {
          IsHtml = this._forumEditor.UsesHTML,
          IsBBCode = this._forumEditor.UsesBBCode,
          IsPersistent = this.PostOptions1.PersistantChecked
        };

      bool isModeratorChanged = this.PageContext.PageUserID != this._ownerUserId;
      DB.message_update(
        this.Request.QueryString["m"], 
        this.Priority.SelectedValue, 
        this._forumEditor.Text, 
        subjectSave, 
        messageFlags.BitValue, 
        this.HtmlEncode(this.ReasonEditor.Text), 
        isModeratorChanged,
        this.PageContext.IsAdmin || this.PageContext.IsModerator, this.OriginalMessage, PageContext.PageUserID);

      // update poll
      if (!string.IsNullOrEmpty(this.RemovePoll.CommandArgument) || this.PollRow1.Visible)
      {
        DB.topic_poll_update(null, this.Request.QueryString["m"], this.GetPollID());
      }

      messageId = this.EditTopicID.Value;

      this.HandlePostToBlog(this._forumEditor.Text, this.Subject.Text);

      // remove cache if it exists...
      this.PageContext.Cache.Remove(
        string.Format(Constants.Cache.FirstPostCleaned, this.PageContext.PageBoardID, this.TopicID));

      return messageId;
    }

    /// <summary>
    /// The post reply handle new post.
    /// </summary>
    /// <returns>
    /// The post reply handle new post.
    /// </returns>
    protected long PostReplyHandleNewPost()
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

      string blogPostID = this.HandlePostToBlog(this._forumEditor.Text, this.Subject.Text);

      // Save to Db
      long topicID = DB.topic_save(
        this.PageContext.PageForumID, 
        this.HtmlEncode(this.Subject.Text), 
        this._forumEditor.Text, 
        this.PageContext.PageUserID, 
        this.Priority.SelectedValue, 
        this.GetPollID(), 
        this.User != null ? null : this.From.Text, 
        this.Request.UserHostAddress, 
        null, 
        blogPostID, 
        messageFlags.BitValue, 
        ref messageId);

      this.UpdateWatchTopic(this.PageContext.PageUserID, (int)topicID);

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

      object replyTo = (this.QuotedTopicID != null) ? this.QuotedTopicID.Value : -1;

      // make message flags
      var messageFlags = new MessageFlags();

      messageFlags.IsHtml = this._forumEditor.UsesHTML;
      messageFlags.IsBBCode = this._forumEditor.UsesBBCode;
      messageFlags.IsPersistent = this.PostOptions1.PersistantChecked;
      messageFlags.IsApproved = this.PageContext.IsAdmin || this.PageContext.IsModerator;

      DB.message_save(
        this.TopicID.Value, 
        this.PageContext.PageUserID, 
        this._forumEditor.Text, 
        this.User != null ? null : this.From.Text, 
        this.Request.UserHostAddress, 
        null, 
        replyTo, 
        messageFlags.BitValue, 
        ref messageId);

      this.UpdateWatchTopic(this.PageContext.PageUserID, this.PageContext.PageTopicID);

      return messageId;
    }

    /// <summary>
    /// Updates Watch Topic based on controls/settings for user...
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="topicId"></param>
    private void UpdateWatchTopic(int userId, int topicId)
    {
      var topicWatchedID = this.TopicWatchedId(userId, topicId);

      if (topicWatchedID.HasValue && !this.PostOptions1.WatchChecked)
      {
        // unsubscribe...
        DB.watchtopic_delete(topicWatchedID.Value);
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
    /// <param name="userId"></param>
    private void WatchTopic(int userId, int topicId)
    {
      if (!this.TopicWatchedId(userId, topicId).HasValue)
      {
          // subscribe to this forum
        DB.watchtopic_add(userId, topicId);
      }
    }

    /// <summary>
    /// Returns true if the topic is set to watch for userId
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private int? TopicWatchedId(int userId, int topicId)
    {
      return DB.watchtopic_check(userId, topicId).GetFirstRowColumnAsValue<int?>("WatchTopicID", null);
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
    protected void Preview_Click(object sender, EventArgs e)
    {
      this.PreviewRow.Visible = true;

      this.PreviewMessagePost.MessageFlags.IsHtml = this._forumEditor.UsesHTML;
      this.PreviewMessagePost.MessageFlags.IsBBCode = this._forumEditor.UsesBBCode;
      this.PreviewMessagePost.Message = this._forumEditor.Text;

      if (this.PageContext.BoardSettings.AllowSignatures)
      {
        using (DataTable userDt = DB.user_list(this.PageContext.PageBoardID, this.PageContext.PageUserID, true))
        {
          if (!userDt.Rows[0].IsNull("Signature"))
          {
            this.PreviewMessagePost.Signature = userDt.Rows[0]["Signature"].ToString();
          }
        }
      }
    }

    /// <summary>
    /// The remove poll_ command.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void RemovePoll_Command(object sender, CommandEventArgs e)
    {
      this.ChangePollShowStatus(false);

      if (e.CommandArgument != null && e.CommandArgument.ToString() != string.Empty)
      {
        DB.poll_remove(e.CommandArgument);
        ((ThemeButton)sender).CommandArgument = null;
      }
    }

    /// <summary>
    /// The remove poll_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void RemovePoll_Load(object sender, EventArgs e)
    {
      ((ThemeButton)sender).Attributes["onclick"] = String.Format(
        "return confirm('{0}');", this.GetText("ASK_POLL_DELETE"));
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
    private bool CanEditPostCheck(DataRow message)
    {
      bool postLocked = false;

      if (!this.PageContext.IsAdmin && this.PageContext.BoardSettings.LockPosts > 0)
      {
        var edited = (DateTime)message["Edited"];

        if (edited.AddDays(this.PageContext.BoardSettings.LockPosts) < DateTime.Now)
        {
          postLocked = true;
        }
      }

      DataRow forumInfo, topicInfo;

      // get topic and forum information
      topicInfo = DB.topic_info(this.PageContext.PageTopicID);
      using (DataTable dt = DB.forum_list(this.PageContext.PageBoardID, this.PageContext.PageForumID))
      {
        forumInfo = dt.Rows[0];
      }

      // Ederon : 9/9/2007 - moderator can edit in locked topics
      return ((!postLocked && !General.BinaryAnd(forumInfo["Flags"], ForumFlags.Flags.IsLocked) &&
               !General.BinaryAnd(topicInfo["Flags"], TopicFlags.Flags.IsLocked) &&
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
    private bool CanHavePoll(DataRow message)
    {
      return (this.TopicID == null && this.QuotedTopicID == null && this.EditTopicID == null) ||
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
    private bool CanQuotePostCheck(DataRow message)
    {
      DataRow forumInfo, topicInfo;

      // get topic and forum information
      topicInfo = DB.topic_info(this.PageContext.PageTopicID);
      using (DataTable dt = DB.forum_list(this.PageContext.PageBoardID, this.PageContext.PageForumID))
      {
        forumInfo = dt.Rows[0];
      }

      if (topicInfo == null || forumInfo == null)
      {
        return false;
      }

      // Ederon : 9/9/2007 - moderator can reply to locked topics
      return (!General.BinaryAnd(forumInfo["Flags"], ForumFlags.Flags.IsLocked) &&
              !General.BinaryAnd(topicInfo["Flags"], TopicFlags.Flags.IsLocked) || this.PageContext.ForumModeratorAccess) &&
             this.PageContext.ForumReplyAccess;
    }

    /// <summary>
    /// The get poll id.
    /// </summary>
    /// <returns>
    /// The get poll id.
    /// </returns>
    private object GetPollID()
    {
      int daysPollExpire = 0;
      object datePollExpire = null;

      if (int.TryParse(this.PollExpire.Text.Trim(), out daysPollExpire))
      {
        datePollExpire = DateTime.Now.AddDays(daysPollExpire);
      }

      // we are just using existing poll
      if (!string.IsNullOrEmpty(this.RemovePoll.CommandArgument))
      {
        int pollID = Convert.ToInt32(this.RemovePoll.CommandArgument);
        DB.poll_update(pollID, this.Question.Text, datePollExpire);
           
            foreach (RepeaterItem ri in ChoiceRepeater.Items)
            {
                string choice = ((TextBox)ri.FindControl("PollChoice")).Text.Trim();
                string chid = ((HiddenField)ri.FindControl("PollChoiceID")).Value;

                if (string.IsNullOrEmpty(chid) && !string.IsNullOrEmpty(choice))
          {
            // add choice
              DB.choice_add(pollID, choice);
          }
                else if (!string.IsNullOrEmpty(chid) && !string.IsNullOrEmpty(choice))
          {
            // update choice
              DB.choice_update(chid, choice);
          }
                else if (!string.IsNullOrEmpty(chid) && string.IsNullOrEmpty(choice))
          {
            // remove choice
              DB.choice_delete(chid);
          }
        }

        return Convert.ToInt32(this.RemovePoll.CommandArgument);
      }
      else if (this.PollRow1.Visible)
      {
        // User wishes to create a poll
        
        // vzrus: always one in current code - a number of  questions
         int questionsTotal = 1;

         System.Collections.Generic.List<PollSaveList> pollList =
             new System.Collections.Generic.List<PollSaveList>(questionsTotal);
          string[] rawChoices = new string[ChoiceRepeater.Items.Count];
          int j = 0;
          foreach(RepeaterItem ri in ChoiceRepeater.Items)
         {
             rawChoices[j] = ((TextBox)ri.FindControl("PollChoice")).Text.Trim();
               j++; 
         }
          
          pollList.Add(new PollSaveList(this.Question.Text,
          rawChoices,
          (DateTime?)datePollExpire)); 
          return DB.poll_save(pollList); 

          /* return DB.poll_save(
          this.Question.Text, 
          this.PollChoice1.Text, 
          this.PollChoice2.Text, 
          this.PollChoice3.Text, 
          this.PollChoice4.Text, 
          this.PollChoice5.Text, 
          this.PollChoice6.Text, 
          this.PollChoice7.Text, 
          this.PollChoice8.Text, 
          this.PollChoice9.Text, 
          datePollExpire); */
      }

      return null; // A poll was not created on this post
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
    private bool HasPoll(DataRow message)
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
    private void InitEditedPost(DataRow currentRow, string message, MessageFlags messageFlags)
    {
      // If the message is in YafBBCode but the editor uses HTML, convert the message text to HTML
      if (messageFlags.IsBBCode && this._forumEditor.UsesHTML)
      {
        message = YafBBCode.ConvertBBCodeToHtmlForEdit(message);
      }

      this._forumEditor.Text = message;

      this.Title.Text = this.GetText("EDIT");

      // add topic link...
      this.PageLinks.AddLink(
        this.Server.HtmlDecode(currentRow["Topic"].ToString()), 
        YafBuildLink.GetLink(ForumPages.posts, "m={0}", this.EditTopicID));

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

      this.Subject.Text = this.Server.HtmlDecode(Convert.ToString(currentRow["Topic"]));

      if ((Convert.ToInt32(currentRow["TopicOwnerID"]) == Convert.ToInt32(currentRow["UserID"])) ||
          this.PageContext.ForumModeratorAccess)
      {
        // allow editing of the topic subject
        this.Subject.Enabled = true;
      }
      else
      {
        // disable the subject
        this.Subject.Enabled = false;
      }

      this.Priority.SelectedItem.Selected = false;
      this.Priority.Items.FindByValue(currentRow["Priority"].ToString()).Selected = true;
      this.EditReasonRow.Visible = true;
      this.ReasonEditor.Text = this.Server.HtmlDecode(Convert.ToString(currentRow["EditReason"]));
      this.PostOptions1.PersistantChecked = messageFlags.IsPersistent;
    }

    /// <summary>
    /// The init poll ui.
    /// </summary>
    /// <param name="currentRow">
    /// The current row.
    /// </param>
    private void InitPollUI(DataRow currentRow)
    {
      this.RemovePoll.CommandArgument = currentRow["PollID"].ToString();

      if (currentRow["PollID"] != DBNull.Value)
      {
        choices = DB.poll_stats(currentRow["PollID"]);
        choices.Columns.Add("ChoiceOrderID", typeof(int));

        this.Question.Text = choices.Rows[0]["Question"].ToString();
        if (choices.Rows[0]["Closes"] != DBNull.Value)
        {
          TimeSpan closing = (DateTime)choices.Rows[0]["Closes"] - DateTime.Now;

          this.PollExpire.Text = SqlDataLayerConverter.VerifyInt32(closing.TotalDays + 1).ToString();
        }
        else
        {
          this.PollExpire.Text = null;
        }

        // First existing values
        int existingRowsCount = 1;
        int allExistingRowsCount = choices.Rows.Count;
        foreach (DataRow choiceRow in choices.Rows)
        {
            choiceRow["ChoiceOrderID"] = existingRowsCount;
            existingRowsCount++;
        }
        int dummyRowsCount = PageContext.BoardSettings.AllowedPollChoiceNumber - allExistingRowsCount - 1;
        for (int i = 0; i <= dummyRowsCount; i++)
        {
          DataRow drow = choices.NewRow();
          drow["ChoiceOrderID"] = existingRowsCount + i;
          choices.Rows.Add(drow);          
        }
        this.ChoiceRepeater.DataSource = choices;
        this.ChoiceRepeater.DataBind();
        this.ChangePollShowStatus(true);
      }
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
    private void InitQuotedReply(DataRow currentRow, string message, MessageFlags messageFlags)
    {
      if (this.PageContext.BoardSettings.RemoveNestedQuotes)
      {
        message = FormatMsg.RemoveNestedQuotes(message);
      }

      // If the message being quoted in YafBBCode but the editor uses HTML, convert the message text to HTML
      if (messageFlags.IsBBCode && this._forumEditor.UsesHTML)
      {
        message = YafBBCode.ConvertBBCodeToHtmlForEdit(message);
      }

      // Ensure quoted replies have bad words removed from them
      message = YafServices.BadWordReplace.Replace(message);

      // Quote the original message
      this._forumEditor.Text = String.Format("[quote={0}]{1}[/quote]\n", PageContext.UserDisplayName.GetName(currentRow.Field<int>("UserID")), message).TrimStart();
    }

    /// <summary>
    /// The init reply to topic.
    /// </summary>
    private void InitReplyToTopic()
    {
      DataRow topic = DB.topic_info(this.TopicID);
      var topicFlags = new TopicFlags(SqlDataLayerConverter.VerifyInt32(topic["Flags"]));

      // Ederon : 9/9/2007 - moderators can reply in locked topics
      if (topicFlags.IsLocked && !this.PageContext.ForumModeratorAccess)
      {
        this.Response.Redirect(this.Request.UrlReferrer.ToString());
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

    #endregion
  }
}