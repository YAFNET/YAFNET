/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages;

using System.Web.UI.HtmlControls;
using YAF.Controls;
using YAF.Types.Exceptions;
using YAF.Web.EventsArgs;
using YAF.Types.Models;

/// <summary>
/// The Posts Page.
/// </summary>
public partial class Posts : ForumPage
{
    /// <summary>
    ///   The _data bound.
    /// </summary>
    private bool dataBound;

    /// <summary>
    ///   The forum flags.
    /// </summary>
    private ForumFlags forumFlags;

    /// <summary>
    ///   The _topic.
    /// </summary>
    private Topic topic;

    /// <summary>
    ///   Initializes a new instance of the <see cref = "Posts" /> class.
    /// </summary>
    public Posts()
        : base("POSTS", ForumPages.Posts)
    {
    }

    /// <summary>
    /// The delete topic_ click.
    /// </summary>
    /// <param name="sender">
    /// <param name="sender">The source of the event.</param>
    /// </param>
    /// <param name="e">
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    /// </param>
    protected void DeleteTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.PageBoardContext.ForumModeratorAccess)
        {
            /*"You don't have access to delete topics."*/
            this.Get<LinkBuilder>().AccessDenied();
        }

        this.GetRepository<Topic>().Delete(this.PageBoardContext.PageForumID, this.PageBoardContext.PageTopicID, true);

        this.Get<LinkBuilder>().Redirect(
            ForumPages.Topics,
            new { f = this.PageBoardContext.PageForumID, name = this.PageBoardContext.PageForum.Name });
    }

    /// <summary>
    /// The email topic_ click.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void EmailTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.User == null)
        {
            this.PageBoardContext.Notify(this.GetText("WARN_EMAILLOGIN"), MessageTypes.warning);
            return;
        }

        this.Get<LinkBuilder>().Redirect(ForumPages.EmailTopic, new { t = this.PageBoardContext.PageTopicID });
    }

    /// <summary>
    /// The lock topic_ click.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void LockTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.PageBoardContext.ForumModeratorAccess)
        {
            // "You are not a forum moderator.
            this.Get<LinkBuilder>().AccessDenied();
        }

        var flags = this.topic.TopicFlags;

        flags.IsLocked = true;

        this.GetRepository<Topic>().Lock(this.PageBoardContext.PageTopicID, flags.BitValue);

        this.PageBoardContext.Notify(this.GetText("INFO_TOPIC_LOCKED"), MessageTypes.info);

        this.LockTopic1.Visible = false;
        this.LockTopic2.Visible = false;

        this.UnlockTopic1.Visible = true;
        this.UnlockTopic2.Visible = true;
    }

    /// <summary>
    /// The message list_ on item created.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void MessageList_OnItemCreated([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
    {
        if (this.Pager.CurrentPageIndex != 0 || e.Item.ItemIndex != 0)
        {
            return;
        }

        var connectControl = e.Item.FindControlAs<DisplayConnect>("DisplayConnect");

        if (connectControl != null && this.PageBoardContext.IsGuest &&
            this.PageBoardContext.BoardSettings.ShowConnectMessageInTopic)
        {
            connectControl.Visible = true;
        }

        // first message... show the ad below this message
        var displayAd = e.Item.FindControlAs<DisplayAd>("DisplayAd");

        // check if need to display the ad...
        if (this.PageBoardContext.BoardSettings.AdPost.IsSet() && displayAd != null)
        {
            displayAd.Visible = this.PageBoardContext.IsGuest || this.PageBoardContext.BoardSettings.ShowAdsToSignedInUsers;
        }
    }

    /// <summary>
    /// The next topic_ click.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void NextTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        var nextTopic = this.GetRepository<Topic>().FindNext(this.topic);

        if (nextTopic == null)
        {
            this.PageBoardContext.Notify(this.GetText("INFO_NOMORETOPICS"), MessageTypes.info);
            return;
        }

        this.Get<LinkBuilder>().Redirect(
            ForumPages.Posts,
            new { t = nextTopic.ID, name = nextTopic.TopicName });
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    protected override void OnInit([NotNull] EventArgs e)
    {
        this.InitializeComponent();
        base.OnInit(e);
    }

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        var isWatched = this.HandleWatchTopic();

        // share menu...
        this.ShareMenu.Visible = this.ShareLink.Visible =
                                     this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.ShowShareTopicTo);

        if (this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.ShowShareTopicTo))
        {
            var topicUrl = this.Get<LinkBuilder>().GetAbsoluteLink(
                ForumPages.Posts,
                new { t = this.PageBoardContext.PageTopicID, name = this.PageBoardContext.PageTopic.TopicName });

            if (this.PageBoardContext.BoardSettings.AllowEmailTopic)
            {
                this.ShareMenu.AddPostBackItem("email", this.GetText("EMAILTOPIC"), "fa fa-paper-plane");
            }

            this.ShareMenu.AddClientScriptItem(
                this.GetText("LINKBACK_TOPIC"),
                JavaScriptBlocks.BootBoxPromptJs(
                    this.GetText("LINKBACK_TOPIC"),
                    this.GetText("LINKBACK_TOPIC_PROMT"),
                    this.GetText("CANCEL"),
                    this.GetText("OK"),
                    topicUrl),
                "fa fa-link");
            this.ShareMenu.AddPostBackItem("retweet", this.GetText("RETWEET_TOPIC"), "fab fa-twitter");

            var facebookUrl = $"http://www.facebook.com/plugins/like.php?href={this.Server.UrlEncode(topicUrl)}";

            this.ShareMenu.AddClientScriptItem(
                this.GetText("FACEBOOK_TOPIC"),
                $@"window.open('{facebookUrl}','{this.GetText("FACEBOOK_TOPIC")}','width=300,height=200,resizable=yes');",
                "fab fa-facebook");

            var facebookShareUrl =
                $"https://www.facebook.com/sharer/sharer.php?u={this.Server.UrlEncode(topicUrl)}";

            this.ShareMenu.AddClientScriptItem(
                this.GetText("FACEBOOK_SHARE_TOPIC"),
                $@"window.open('{facebookShareUrl}','{this.GetText("FACEBOOK_SHARE_TOPIC")}','width=550,height=690,resizable=yes');",
                "fab fa-facebook");
            this.ShareMenu.AddPostBackItem("reddit", this.GetText("REDDIT_TOPIC"), "fab fa-reddit");

            this.ShareMenu.AddPostBackItem("tumblr", this.GetText("TUMBLR_TOPIC"), "fab fa-tumblr");
        }
        else
        {
            if (this.PageBoardContext.BoardSettings.AllowEmailTopic)
            {
                this.OptionsMenu.AddPostBackItem("email", this.GetText("EMAILTOPIC"), "fa fa-email");
            }
        }

        if (!this.PageBoardContext.IsGuest)
        {
            this.OptionsMenu.AddPostBackItem(
                isWatched ? "unwatch" : "watch",
                isWatched ? this.GetText("UNWATCHTOPIC") : this.GetText("WATCHTOPIC"),
                isWatched ? "fa fa-eye-slash" : "fa fa-eye");
        }

        this.OptionsMenu.AddPostBackItem("print", this.GetText("PRINTTOPIC"), "fa fa-print");

        if (this.PageBoardContext.BoardSettings.ShowAtomLink &&
            this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.PostsFeedAccess))
        {
            this.OptionsMenu.AddPostBackItem("atomfeed", this.GetText("ATOMTOPIC"), "fa fa-rss");
        }

        // attach the menus to HyperLinks
        this.ShareMenu.Attach(this.ShareLink);
        this.OptionsMenu.Attach(this.OptionsLink);

        if (!this.dataBound)
        {
            this.BindData();
        }

        base.OnPreRender(e);
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.PageBoardContext.IsGuest)
        {
            if (this.PageBoardContext.PageUser.Activity)
            {
                this.GetRepository<Activity>().UpdateTopicNotification(
                    this.PageBoardContext.PageUserID,
                    this.PageBoardContext.PageTopicID);
            }
        }

        this.topic = this.PageBoardContext.PageTopic;

        if (this.topic.PollID.HasValue)
        {
            this.PollList.TopicId = this.PageBoardContext.PageTopicID;
            this.PollList.Visible = true;
            this.PollList.PollId = this.topic.PollID.Value;
        }

        this.BindData();

        var firstPost = ((List<PagedMessage>)this.MessageList.DataSource).FirstOrDefault();

        this.forumFlags = new ForumFlags(firstPost.ForumFlags);

        if (this.PageBoardContext.IsGuest && !this.PageBoardContext.ForumReadAccess)
        {
            // attempt to get permission by redirecting to login...
            this.Get<IPermissions>().HandleRequest(ViewPermissions.RegisteredUsers);
        }
        else if (!this.PageBoardContext.ForumReadAccess)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        var yafBoardSettings = this.PageBoardContext.BoardSettings;

        if (this.IsPostBack)
        {
            return;
        }

        // Clear Multi-quotes if topic is different
        if (this.Get<ISession>().MultiQuoteIds != null)
        {
            if (!this.Get<ISession>().MultiQuoteIds.Any(m => m.TopicID.Equals(this.PageBoardContext.PageTopicID)))
            {
                this.Get<ISession>().MultiQuoteIds = null;
            }
        }

        this.NewTopic2.NavigateUrl = this.NewTopic1.NavigateUrl = this.Get<LinkBuilder>().GetLink(
                                         ForumPages.PostTopic,
                                         new { f = this.PageBoardContext.PageForumID });

        this.PostReplyLink1.NavigateUrl = this.PostReplyLink2.NavigateUrl = this.Get<LinkBuilder>().GetLink(
                                              ForumPages.PostMessage,
                                              new
                                                  {
                                                      t = this.PageBoardContext.PageTopicID,
                                                      f = this.PageBoardContext.PageForumID
                                                  });

        var topicSubject = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.topic.TopicName));

        this.TopicTitle.Text = this.topic.Description.IsSet()
                                   ? $"{topicSubject} - <em>{this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.topic.Description))}</em>"
                                   : this.Get<IBadWordReplace>().Replace(topicSubject);

        this.TopicLink.ToolTip = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.topic.Description));
        this.TopicLink.NavigateUrl = this.Get<LinkBuilder>().GetLink(
            ForumPages.Posts,
            new { t = this.PageBoardContext.PageTopicID, name = this.PageBoardContext.PageTopic.TopicName });

        this.QuickReplyDialog.Visible = yafBoardSettings.ShowQuickAnswer;
        this.QuickReplyLink1.Visible = yafBoardSettings.ShowQuickAnswer;
        this.QuickReplyLink2.Visible = yafBoardSettings.ShowQuickAnswer;

        if (!this.PageBoardContext.ForumPostAccess || this.forumFlags.IsLocked && !this.PageBoardContext.ForumModeratorAccess)
        {
            this.NewTopic1.Visible = false;
            this.NewTopic2.Visible = false;
        }

        // Ederon : 9/9/2007 - moderators can reply in locked topics
        if (!this.PageBoardContext.ForumReplyAccess || (this.topic.TopicFlags.IsLocked || this.forumFlags.IsLocked) &&
            !this.PageBoardContext.ForumModeratorAccess)
        {
            this.PostReplyLink1.Visible = this.PostReplyLink2.Visible = false;
            this.QuickReplyDialog.Visible = false;
            this.QuickReplyLink1.Visible = false;
            this.QuickReplyLink2.Visible = false;
        }

        if (this.PageBoardContext.ForumModeratorAccess)
        {
            this.MoveTopic1.Visible = true;
            this.MoveTopic2.Visible = true;

            this.Tools1.Visible = true;
            this.Tools2.Visible = true;
        }
        else
        {
            this.MoveTopic1.Visible = false;
            this.MoveTopic2.Visible = false;

            this.Tools1.Visible = false;
            this.Tools2.Visible = false;
        }

        if (!this.PageBoardContext.ForumModeratorAccess)
        {
            this.LockTopic1.Visible = false;
            this.UnlockTopic1.Visible = false;
            this.DeleteTopic1.Visible = false;
            this.LockTopic2.Visible = false;
            this.UnlockTopic2.Visible = false;
            this.DeleteTopic2.Visible = false;
        }
        else
        {
            this.LockTopic1.Visible = !this.topic.TopicFlags.IsLocked;
            this.UnlockTopic1.Visible = !this.LockTopic1.Visible;
            this.LockTopic2.Visible = this.LockTopic1.Visible;
            this.UnlockTopic2.Visible = !this.LockTopic2.Visible;
        }
    }

    /// <summary>
    /// The create page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);

        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);
        this.PageBoardContext.PageLinks.AddLink(
            this.Get<IBadWordReplace>().Replace(this.Server.HtmlDecode(this.PageBoardContext.PageTopic.TopicName)),
            string.Empty);
    }

    /// <summary>
    /// The post reply link_ click.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void PostReplyLink_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        // Ederon : 9/9/2007 - moderator can reply in locked posts
        if (this.PageBoardContext.ForumModeratorAccess)
        {
            return;
        }

        if (this.topic.TopicFlags.IsLocked)
        {
            this.PageBoardContext.Notify(this.GetText("WARN_TOPIC_LOCKED"), MessageTypes.warning);
            return;
        }

        if (!this.forumFlags.IsLocked)
        {
            return;
        }

        this.PageBoardContext.Notify(this.GetText("WARN_FORUM_LOCKED"), MessageTypes.warning);
    }

    /// <summary>
    /// The Previous topic click.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void PrevTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        var previousTopic = this.GetRepository<Topic>().FindPrevious(this.topic);

        if (previousTopic == null)
        {
            this.PageBoardContext.Notify(this.GetText("INFO_NOMORETOPICS"), MessageTypes.info);
            return;
        }

        this.Get<LinkBuilder>().Redirect(
            ForumPages.Posts,
            new { t = this.PageBoardContext.PageTopicID, name = previousTopic.TopicName });
    }

    /// <summary>
    /// Watch Topic
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    protected void TrackTopicClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.GetRepository<WatchTopic>().Add(this.PageBoardContext.PageUserID, this.PageBoardContext.PageTopicID);
        this.PageBoardContext.Notify(this.GetText("INFO_WATCH_TOPIC"), MessageTypes.warning);

        this.HandleWatchTopic();
    }

    /// <summary>
    /// Un-Watch Topic
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    protected void UnTrackTopicClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.GetRepository<WatchTopic>().Delete(
            w => w.TopicID == this.PageBoardContext.PageTopicID && w.UserID == this.PageBoardContext.PageUserID);

        this.PageBoardContext.Notify(this.GetText("INFO_UNWATCH_TOPIC"), MessageTypes.info);

        this.HandleWatchTopic();
    }

    /// <summary>
    /// The unlock topic_ click.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void UnlockTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.PageBoardContext.ForumModeratorAccess)
        {
            // "You are not a forum moderator.
            this.Get<LinkBuilder>().AccessDenied();
        }

        var flags = this.topic.TopicFlags;

        flags.IsLocked = false;

        this.GetRepository<Topic>().Lock(this.PageBoardContext.PageTopicID, flags.BitValue);

        this.PageBoardContext.Notify(this.GetText("INFO_TOPIC_UNLOCKED"), MessageTypes.info);

        this.LockTopic1.Visible = true;
        this.LockTopic2.Visible = true;

        this.UnlockTopic1.Visible = false;
        this.UnlockTopic2.Visible = false;
    }

    /// <summary>
    /// Adds meta data: description and keywords to the page header.
    /// </summary>
    /// <param name="firstMessage">
    /// first message in the topic
    /// </param>
    private void AddMetaData([NotNull] string firstMessage)
    {
        try
        {
            if (firstMessage.IsNotSet())
            {
                return;
            }
        }
        catch (Exception)
        {
            return;
        }

        if (this.Page.Header == null || !this.PageBoardContext.BoardSettings.AddDynamicPageMetaTags)
        {
            return;
        }

        var message = this.Get<IFormatMessage>().GetCleanedTopicMessage(firstMessage, this.PageBoardContext.PageTopicID);
        var meta = this.Page.Header.FindControlType<HtmlMeta>();

        var htmlMetas = meta as IList<HtmlMeta> ?? meta.ToList();
        if (message.MessageTruncated.IsSet())
        {
            HtmlMeta descriptionMeta;

            string descriptionContent;

            // Use Topic Description if set
            if (!this.topic.Description.IsNullOrEmptyField())
            {
                var topicDescription = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.topic.Description));

                descriptionContent = topicDescription.Length > 50
                                         ? topicDescription
                                         : $"{topicDescription} - {message.MessageTruncated}";
            }
            else
            {
                descriptionContent = message.MessageTruncated;
            }

            if (htmlMetas.Any(x => x.Name.Equals("description")))
            {
                // use existing...
                descriptionMeta = htmlMetas.FirstOrDefault(x => x.Name.Equals("description"));
                if (descriptionMeta != null)
                {
                    descriptionMeta.Content = descriptionContent;

                    this.Page.Header.Controls.Remove(descriptionMeta);

                    descriptionMeta = ControlHelper.MakeMetaDescriptionControl(descriptionContent);

                    // add to the header...
                    this.Page.Header.Controls.Add(descriptionMeta);
                }
            }
            else
            {
                descriptionMeta = ControlHelper.MakeMetaDescriptionControl(descriptionContent);

                // add to the header...
                this.Page.Header.Controls.Add(descriptionMeta);
            }
        }

        if (message.MessageKeywords.Count <= 0)
        {
            return;
        }

        HtmlMeta keywordMeta;

        var keywordStr = message.MessageKeywords.Where(x => x.IsSet()).ToList().ToDelimitedString(",");

        //// this.Tags.Text = "Tags: {0}".FormatWith(keywordStr);
        if (htmlMetas.Any(x => x.Name.Equals("keywords")))
        {
            // use existing...
            keywordMeta = htmlMetas.FirstOrDefault(x => x.Name.Equals("keywords"));
            keywordMeta.Content = keywordStr;

            this.Page.Header.Controls.Remove(keywordMeta);

            // add to the header...
            this.Page.Header.Controls.Add(keywordMeta);
        }
        else
        {
            keywordMeta = ControlHelper.MakeMetaKeywordsControl(keywordStr);

            // add to the header...
            this.Page.Header.Controls.Add(keywordMeta);
        }
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.dataBound = true;

        var showDeleted = this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess ||
                          this.PageBoardContext.BoardSettings.ShowDeletedMessagesToAll;

        var findMessageId = this.GetFindMessageId(showDeleted, out var messagePosition);

        // Mark topic read
        this.Get<IReadTrackCurrentUser>().SetTopicRead(this.PageBoardContext.PageTopicID);
        this.Pager.PageSize = this.PageBoardContext.BoardSettings.PostsPerPage;

        var postList = this.GetRepository<Message>().PostListPaged(
            this.PageBoardContext.PageTopicID,
            this.PageBoardContext.PageUserID,
            !this.IsPostBack && !this.PageBoardContext.IsCrawler,
            showDeleted,
            DateTimeHelper.SqlDbMinTime(),
            DateTime.UtcNow,
            this.Pager.CurrentPageIndex,
            this.Pager.PageSize,
            messagePosition);

        if (!postList.Any())
        {
            var topicException = new NoPostsFoundForTopicException(
                this.PageBoardContext.PageTopicID,
                this.PageBoardContext.PageUserID,
                !this.IsPostBack && !this.PageBoardContext.IsCrawler,
                showDeleted,
                DateTimeHelper.SqlDbMinTime(),
                DateTime.UtcNow,
                this.Pager.CurrentPageIndex,
                this.Pager.PageSize,
                messagePosition);

            this.Logger.Error(topicException, "No posts were found for topic");

            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        var firstPost = postList.FirstOrDefault();

        this.Pager.Count = firstPost.TotalRows;

        if (findMessageId > 0)
        {
            this.Pager.CurrentPageIndex = firstPost.PageIndex;

            // move to this message on load...
            if (!this.PageBoardContext.IsCrawler)
            {
                this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                    this,
                    "GotoAnchorJs",
                    JavaScriptBlocks.LoadGotoAnchor($"post{findMessageId}"));
            }
        }
        else
        {
            // move to this message on load...
            if (!this.PageBoardContext.IsCrawler)
            {
                this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                    this,
                    "GotoAnchorJs",
                    JavaScriptBlocks.LoadGotoAnchor($"post{firstPost.MessageID}"));
            }
        }

        // if current index is 0 we are on the first page and the metadata can be added.
        if (this.Pager.CurrentPageIndex == 0)
        {
            // handle add description/keywords for SEO
            this.AddMetaData(firstPost.Message);
        }

        this.MessageList.DataSource = postList;

        this.DataBind();
    }

    /// <summary>
    /// Gets the message ID if "find" is in the query string
    /// </summary>
    /// <param name="showDeleted">
    /// The show Deleted.
    /// </param>
    /// <param name="messagePosition">
    /// The message Position.
    /// </param>
    /// <returns>
    /// The get find message id.
    /// </returns>
    private int GetFindMessageId(bool showDeleted, out int messagePosition)
    {
        var findMessageId = 0;
        messagePosition = -1;

        try
        {
            if (this.Get<HttpRequestBase>().QueryString.Exists("p"))
            {
                return findMessageId;
            }

            if (this.Get<HttpRequestBase>().QueryString.Exists("m") && int.TryParse(
                    this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"),
                    out var messageId))
            {
                var unreadFirst = this.GetRepository<Message>().FindUnread(
                    this.PageBoardContext.PageTopicID,
                    messageId,
                    DateTimeHelper.SqlDbMinTime(),
                    showDeleted);

                findMessageId = unreadFirst.MessageID;
                messagePosition = unreadFirst.MessagePosition;
            }
            else
            {
                // find first unread message
                var lastRead = !this.PageBoardContext.IsCrawler
                                   ? this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                                       this.PageBoardContext.PageForumID,
                                       this.PageBoardContext.PageTopicID,
                                       null,
                                       null)
                                   : DateTime.UtcNow;

                var unreadFirst = this.GetRepository<Message>().FindUnread(
                    this.PageBoardContext.PageTopicID,
                    null,
                    lastRead,
                    showDeleted);

                findMessageId = unreadFirst.MessageID;
                messagePosition = unreadFirst.MessagePosition;

                if (this.PageBoardContext.PageUser.Activity)
                {
                    this.GetRepository<Activity>().UpdateNotification(this.PageBoardContext.PageUserID, findMessageId);
                }
            }
        }
        catch (Exception x)
        {
            this.Logger.Log(this.PageBoardContext.PageUserID, this, x);
        }

        return findMessageId;
    }

    /// <summary>
    /// The handle watch topic.
    /// </summary>
    /// <returns>
    /// Returns The handle watch topic.
    /// </returns>
    private bool HandleWatchTopic()
    {
        if (this.PageBoardContext.IsGuest)
        {
            return false;
        }

        var watchTopicId = this.GetRepository<WatchTopic>().Check(
            this.PageBoardContext.PageUserID,
            this.PageBoardContext.PageTopicID);

        // check if this forum is being watched by this user
        return watchTopicId.HasValue;
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    ///   the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        // in case topic is deleted or not existent
        if (this.PageBoardContext.PageTopic == null)
        {
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.ShareMenu.ItemClick += this.ShareMenuItemClick;
        this.OptionsMenu.ItemClick += this.OptionsMenuItemClick;
    }

    /// <summary>
    /// The options menu_ item click.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The Pop Event Arguments.</param>
    private void ShareMenuItemClick([NotNull] object sender, [NotNull] PopEventArgs e)
    {
        var topicUrl = this.Get<LinkBuilder>().GetAbsoluteLink(
            ForumPages.Posts,
            new { t = this.PageBoardContext.PageTopicID, name = this.PageBoardContext.PageTopic.TopicName });

        switch (e.Item.ToLower())
        {
            case "email":
                this.EmailTopic_Click(sender, e);
                break;
            case "tumblr":
                {
                    // process message... clean html, strip html, remove BBCode, etc...
                    var tumblrTopicName = BBCodeHelper
                        .StripBBCode(HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(this.topic.TopicName)))
                        .RemoveMultipleWhitespace();

                    var meta = this.Page.Header.FindControlType<HtmlMeta>().ToList();

                    var description = string.Empty;

                    if (meta.Any(x => x.Name.Equals("description")))
                    {
                        var descriptionMeta = meta.FirstOrDefault(x => x.Name.Equals("description"));
                        if (descriptionMeta != null)
                        {
                            description = $"&description={descriptionMeta.Content}";
                        }
                    }

                    var tumblrUrl =
                        $"http://www.tumblr.com/share/link?url={this.Server.UrlEncode(topicUrl)}&name={tumblrTopicName}{description}";

                    this.Get<HttpResponseBase>().Redirect(tumblrUrl);
                }

                break;
            case "retweet":
                {
                    var twitterName = this.PageBoardContext.BoardSettings.TwitterUserName.IsSet()
                                          ? $"@{this.PageBoardContext.BoardSettings.TwitterUserName} "
                                          : string.Empty;

                    // process message... clean html, strip html, remove bbcode, etc...
                    var twitterMsg = BBCodeHelper
                        .StripBBCode(HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(this.topic.TopicName)))
                        .RemoveMultipleWhitespace();

                    var tweetUrl =
                        $"http://twitter.com/share?url={this.Server.UrlEncode(topicUrl)}&text={this.Server.UrlEncode(string.Format("RT {1}Thread: {0}", twitterMsg.Truncate(100), twitterName))}";

                    this.Get<HttpResponseBase>().Redirect(tweetUrl);
                }

                break;
            case "reddit":
                {
                    var redditUrl =
                        $"http://www.reddit.com/submit?url={this.Server.UrlEncode(topicUrl)}&title={this.Server.UrlEncode(this.topic.TopicName)}";

                    this.Get<HttpResponseBase>().Redirect(redditUrl);
                }

                break;
            default:
                throw new ApplicationException(e.Item);
        }
    }

    /// <summary>
    /// The options menu_ item click.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void OptionsMenuItemClick([NotNull] object sender, [NotNull] PopEventArgs e)
    {
        switch (e.Item.ToLower())
        {
            case "print":
                this.Get<LinkBuilder>().Redirect(ForumPages.PrintTopic, new { t = this.PageBoardContext.PageTopicID });
                break;
            case "watch":
                this.TrackTopicClick(sender, e);
                break;
            case "unwatch":
                this.UnTrackTopicClick(sender, e);
                break;
            case "email":
                this.EmailTopic_Click(sender, e);
                break;
            case "atomfeed":
                this.Get<LinkBuilder>().Redirect(
                    ForumPages.Feed,
                    new {feed = RssFeeds.Posts.ToInt(), t = this.PageBoardContext.PageTopicID });
                break;
            default:
                throw new ApplicationException(e.Item);
        }
    }
}