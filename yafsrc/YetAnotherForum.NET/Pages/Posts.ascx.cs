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
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Controls;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Services.Auth;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Exceptions;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.EventsArgs;
    using YAF.Web.Extensions;

    using Forum = YAF.Types.Models.Forum;

    #endregion

    /// <summary>
    /// The Posts Page.
    /// </summary>
    public partial class Posts : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   The _data bound.
        /// </summary>
        private bool dataBound;

        /// <summary>
        ///   The _forum.
        /// </summary>
        private Forum forum;

        /// <summary>
        ///   The _forum flags.
        /// </summary>
        private ForumFlags forumFlags;

        /// <summary>
        ///   The _topic.
        /// </summary>
        private Topic topic;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Posts" /> class.
        /// </summary>
        public Posts()
            : base("POSTS")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets CurrentMessage.
        /// </summary>
        protected int CurrentMessage
        {
            get
            {
                if (this.ViewState["CurrentMessage"] != null)
                {
                    return (int)this.ViewState["CurrentMessage"];
                }

                return 0;
            }

            set => this.ViewState["CurrentMessage"] = value;
        }

        #endregion

        #region Methods

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
            if (!this.PageContext.ForumModeratorAccess)
            {
                /*"You don't have access to delete topics."*/
                BuildLink.AccessDenied();
            }

            this.GetRepository<Topic>().Delete(this.PageContext.PageTopicID, true);
            BuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
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
                this.PageContext.AddLoadMessage(this.GetText("WARN_EMAILLOGIN"), MessageTypes.warning);
                return;
            }

            BuildLink.Redirect(ForumPages.EmailTopic, "t={0}", this.PageContext.PageTopicID);
        }

        /// <summary>
        /// The lock topic_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void LockTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageContext.ForumModeratorAccess)
            {
                // "You are not a forum moderator.
                BuildLink.AccessDenied();
            }

            this.topic.TopicFlags.IsLocked = true;

            this.GetRepository<Topic>().LockTopic(
                this.PageContext.PageTopicID,
                this.topic.TopicFlags.BitValue);

            this.PageContext.AddLoadMessage(this.GetText("INFO_TOPIC_LOCKED"), MessageTypes.info);

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

            if (connectControl != null && this.PageContext.IsGuest
                && this.Get<BoardSettings>().ShowConnectMessageInTopic)
            {
                connectControl.Visible = true;
            }

            // first message... show the ad below this message
            var displayAd = e.Item.FindControlAs<DisplayAd>("DisplayAd");

            // check if need to display the ad...
            if (this.Get<BoardSettings>().AdPost.IsSet() && displayAd != null)
            {
                displayAd.Visible = this.PageContext.IsGuest || this.Get<BoardSettings>().ShowAdsToSignedInUsers;
            }
        }

        /// <summary>
        /// The next topic_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void NextTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var nextTopic = this.GetRepository<Topic>().FindNextTopic(this.topic);

            if (nextTopic == null)
            {
                this.PageContext.AddLoadMessage(this.GetText("INFO_NOMORETOPICS"), MessageTypes.info);
                return;
            }

            BuildLink.Redirect(ForumPages.Posts, "t={0}", nextTopic.ID.ToString());
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
                                         this.Get<IPermissions>().Check(this.Get<BoardSettings>().ShowShareTopicTo);

            if (this.Get<IPermissions>().Check(this.Get<BoardSettings>().ShowShareTopicTo))
            {
                var topicUrl = BuildLink.GetLinkNotEscaped(
                    ForumPages.Posts, true, "t={0}", this.PageContext.PageTopicID);

                if (this.Get<BoardSettings>().AllowEmailTopic)
                {
                    this.ShareMenu.AddPostBackItem(
                        "email", this.GetText("EMAILTOPIC"), "fa fa-paper-plane");
                }

                this.ShareMenu.AddClientScriptItem(
                    this.GetText("LINKBACK_TOPIC"),
                    $@"bootbox.prompt({{ 
                                      title: '{this.GetText("LINKBACK_TOPIC")}',
                                      message: '{this.GetText("LINKBACK_TOPIC_PROMT")}',
	                                  value: '{topicUrl}',
                                      buttons: {{cancel:{{label:'{this.GetText("CANCEL")}'}}, confirm:{{label:'{this.GetText("OK")}'}}}},
                                      callback: function(){{}}
	                              }});",
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
                this.ShareMenu.AddPostBackItem(
                    "reddit", this.GetText("REDDIT_TOPIC"), "fab fa-reddit");

                this.ShareMenu.AddPostBackItem(
                    "tumblr", this.GetText("TUMBLR_TOPIC"), "fab fa-tumblr");
            }
            else
            {
                if (this.Get<BoardSettings>().AllowEmailTopic)
                {
                    this.OptionsMenu.AddPostBackItem(
                        "email", this.GetText("EMAILTOPIC"), "fa fa-email");
                }
            }

            // options menu...
            this.OptionsMenu.AddPostBackItem(
                "watch",
                isWatched ? this.GetText("UNWATCHTOPIC") : this.GetText("WATCHTOPIC"),
                isWatched ? "fa fa-eye-slash" : "fa fa-eye");

            this.OptionsMenu.AddPostBackItem(
                "print", this.GetText("PRINTTOPIC"), "fa fa-print");

            if (this.Get<BoardSettings>().ShowAtomLink
                && this.Get<IPermissions>().Check(this.Get<BoardSettings>().PostsFeedAccess))
            {
                this.OptionsMenu.AddPostBackItem(
                    "atomfeed", this.GetText("ATOMTOPIC"), "fa fa-rss");
            }

            if (this.Get<BoardSettings>().ShowRSSLink
                && this.Get<IPermissions>().Check(this.Get<BoardSettings>().PostsFeedAccess))
            {
                this.OptionsMenu.AddPostBackItem(
                    "rssfeed", this.GetText("RSSTOPIC"), "fa fa-rss-square");
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
            if (!this.PageContext.IsGuest)
            {
                if (this.PageContext.CurrentUserData.Activity)
                {
                    this.GetRepository<Activity>().UpdateTopicNotification(
                        this.PageContext.PageUserID,
                        this.PageContext.PageTopicID);
                }

                // The html code for "Favorite Topic" theme buttons.
                var tagButtonHtml =
                    $"'<a class=\"btn btn-secondary\" href=\"javascript:addFavoriteTopic(' + response + ');\" title=\"{this.GetText("BUTTON_TAGFAVORITE_TT")}\"><span><i class=\"fa fa-star fa-fw\"></i>&nbsp;{this.GetText("BUTTON_TAGFAVORITE")}</span></a>'";
                var untagButtonHtml =
                    $"'<a class=\"btn btn-secondary\" href=\"javascript:removeFavoriteTopic(' + response + ');\" title=\"{this.GetText("BUTTON_UNTAGFAVORITE_TT")}\"><span><i class=\"fa fa-star fa-fw\"></i>&nbsp;{this.GetText("BUTTON_UNTAGFAVORITE")}</span></a>'";

                // Register the client side script for the "Favorite Topic".
                var favoriteTopicJs = JavaScriptBlocks.AddFavoriteTopicJs(untagButtonHtml) + Environment.NewLine +
                                      JavaScriptBlocks.RemoveFavoriteTopicJs(tagButtonHtml);

                this.PageContext.PageElements.RegisterJsBlockStartup("favoriteTopicJs", favoriteTopicJs);
                this.PageContext.PageElements.RegisterJsBlockStartup("asynchCallFailedJs", "function CallFailed(res){ console.log(res);alert('Error Occurred'); }");

                // Has the user already tagged this topic as favorite?
                if (this.Get<IFavoriteTopic>().IsFavoriteTopic(this.PageContext.PageTopicID))
                {
                    // Generate the "Untag" theme button with appropriate JS calls for onclick event.
                    this.TagFavorite1.NavigateUrl = $"javascript:removeFavoriteTopic({this.PageContext.PageTopicID});";
                    this.TagFavorite2.NavigateUrl = $"javascript:removeFavoriteTopic({this.PageContext.PageTopicID});";
                    this.TagFavorite1.TextLocalizedTag = "BUTTON_UNTAGFAVORITE";
                    this.TagFavorite1.TitleLocalizedTag = "BUTTON_UNTAGFAVORITE_TT";
                    this.TagFavorite2.TextLocalizedTag = "BUTTON_UNTAGFAVORITE";
                    this.TagFavorite2.TitleLocalizedTag = "BUTTON_UNTAGFAVORITE_TT";
                }
                else
                {
                    // Generate the "Tag" theme button with appropriate JS calls for onclick event.
                    this.TagFavorite1.NavigateUrl = $"javascript:addFavoriteTopic({this.PageContext.PageTopicID});";
                    this.TagFavorite2.NavigateUrl = $"javascript:addFavoriteTopic({this.PageContext.PageTopicID});";
                    this.TagFavorite1.TextLocalizedTag = "BUTTON_TAGFAVORITE";
                    this.TagFavorite1.TitleLocalizedTag = "BUTTON_TAGFAVORITE_TT";
                    this.TagFavorite2.TextLocalizedTag = "BUTTON_TAGFAVORITE";
                    this.TagFavorite2.TitleLocalizedTag = "BUTTON_TAGFAVORITE_TT";
                }
            }
            else
            {
                this.TagFavorite1.Visible = false;
                this.TagFavorite2.Visible = false;
            }

            this.topic = this.GetRepository<Topic>().GetById(this.PageContext.PageTopicID);

            // in case topic is deleted or not existent
            if (this.topic == null)
            {
                BuildLink.RedirectInfoPage(InfoMessage.Invalid);
            }

            var dt = this.GetRepository<Forum>().List(
                this.PageContext.PageBoardID,
                this.PageContext.PageForumID);

            this.forum = dt.FirstOrDefault();
            
            this.forumFlags = this.forum.ForumFlags;

            if (this.PageContext.IsGuest && !this.PageContext.ForumReadAccess)
            {
                // attempt to get permission by redirecting to login...
                this.Get<IPermissions>().HandleRequest(ViewPermissions.RegisteredUsers);
            }
            else if (!this.PageContext.ForumReadAccess)
            {
                BuildLink.AccessDenied();
            }

            var yafBoardSettings = this.Get<BoardSettings>();

            if (!this.IsPostBack)
            {
                // Clear Multi-quotes if topic is different
                if (this.Get<ISession>().MultiQuoteIds != null)
                {
                    if (!this.Get<ISession>().MultiQuoteIds.Any(m => m.TopicID.Equals(this.PageContext.PageTopicID)))
                    {
                        this.Get<ISession>().MultiQuoteIds = null;
                    }
                }

                this.NewTopic2.NavigateUrl =
                    this.NewTopic1.NavigateUrl =
                    BuildLink.GetLinkNotEscaped(ForumPages.PostTopic, "f={0}", this.PageContext.PageForumID);

                this.PostReplyLink1.NavigateUrl =
                    this.PostReplyLink2.NavigateUrl =
                    BuildLink.GetLinkNotEscaped(
                        ForumPages.PostMessage,
                        "t={0}&f={1}",
                        this.PageContext.PageTopicID,
                        this.PageContext.PageForumID);

                var topicSubject = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.topic.TopicName));

                if (this.topic.Description.IsSet()
                    && yafBoardSettings.EnableTopicDescription)
                {
                    this.TopicTitle.Text =
                        $"{topicSubject} - <em>{this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.topic.Description))}</em>";
                }
                else
                {
                    this.TopicTitle.Text = this.Get<IBadWordReplace>().Replace(topicSubject);
                }

                this.TopicLink.ToolTip = this.Get<IBadWordReplace>().Replace(
                    this.HtmlEncode(this.topic.Description));
                this.TopicLink.NavigateUrl = BuildLink.GetLinkNotEscaped(
                    ForumPages.Posts, "t={0}", this.PageContext.PageTopicID);

                this.QuickReplyDialog.Visible = yafBoardSettings.ShowQuickAnswer;
                this.QuickReplyLink1.Visible = yafBoardSettings.ShowQuickAnswer;
                this.QuickReplyLink2.Visible = yafBoardSettings.ShowQuickAnswer;

                if (!this.PageContext.ForumPostAccess || this.forumFlags.IsLocked && !this.PageContext.ForumModeratorAccess)
                {
                    this.NewTopic1.Visible = false;
                    this.NewTopic2.Visible = false;
                }

                // Ederon : 9/9/2007 - moderators can reply in locked topics
                if (!this.PageContext.ForumReplyAccess ||
                    (this.topic.TopicFlags.IsLocked || this.forumFlags.IsLocked) && !this.PageContext.ForumModeratorAccess)
                {
                    this.PostReplyLink1.Visible = this.PostReplyLink2.Visible = false;
                    this.QuickReplyDialog.Visible = false;
                    this.QuickReplyLink1.Visible = false;
                    this.QuickReplyLink2.Visible = false;
                }

                if (this.PageContext.ForumModeratorAccess)
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

                if (!this.PageContext.ForumModeratorAccess)
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

                if (this.PageContext.ForumReplyAccess ||
                    (!this.topic.TopicFlags.IsLocked || !this.forumFlags.IsLocked) && this.PageContext.ForumModeratorAccess)
                {
                    this.PageContext.PageElements.RegisterJsBlockStartup(
                        "SelectedQuotingJs",
                        JavaScriptBlocks.SelectedQuotingJs(
                            BuildLink.GetLinkNotEscaped(
                                ForumPages.PostMessage,
                                "t={0}&f={1}",
                                this.PageContext.PageTopicID,
                                this.PageContext.PageForumID),
                                this.GetText("POSTS", "QUOTE_SELECTED")));
                }
            }

            #endregion

            this.BindData();
        }

        /// <summary>
        /// The create page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            if (this.PageContext.Settings.LockedForum == 0)
            {
                this.PageLinks.AddRoot();
                this.PageLinks.AddLink(
                    this.PageContext.PageCategoryName,
                    BuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
            }

            this.PageLinks.AddForum(this.PageContext.PageForumID);
            this.PageLinks.AddLink(
                this.Get<IBadWordReplace>().Replace(this.Server.HtmlDecode(this.PageContext.PageTopicName)),
                string.Empty);
        }

        /// <summary>
        /// The poll group id.
        /// </summary>
        /// <returns>
        /// Returns The poll group id.
        /// </returns>
        protected int PollGroupId()
        {
            return this.topic.PollID ?? 0;
        }

        /// <summary>
        /// The post reply link_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PostReplyLink_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Ederon : 9/9/2007 - moderator can reply in locked posts
            if (this.PageContext.ForumModeratorAccess)
            {
                return;
            }

            if (this.topic.TopicFlags.IsLocked)
            {
                this.PageContext.AddLoadMessage(this.GetText("WARN_TOPIC_LOCKED"), MessageTypes.warning);
                return;
            }

            if (!this.forumFlags.IsLocked)
            {
                return;
            }

            this.PageContext.AddLoadMessage(this.GetText("WARN_FORUM_LOCKED"), MessageTypes.warning);
        }

        /// <summary>
        /// The Previous topic click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PrevTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var previousTopic = this.GetRepository<Topic>().FindPreviousTopic(this.topic);

            if (previousTopic == null)
            {
                this.PageContext.AddLoadMessage(this.GetText("INFO_NOMORETOPICS"), MessageTypes.info);
                return;
            }

            BuildLink.Redirect(ForumPages.Posts, "t={0}", previousTopic.ID.ToString());
        }

        /// <summary>
        /// The show poll buttons.
        /// </summary>
        /// <returns>
        /// Returns The show poll buttons.
        /// </returns>
        protected bool ShowPollButtons()
        {
            return false;

            /* return (Convert.ToInt32(_topic["UserID"]) == PageContext.PageUserID) || PageContext.IsModerator || PageContext.IsAdmin; */
        }

        /// <summary>
        /// The track topic_ click.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected void TrackTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.PageContext.IsGuest)
            {
                this.PageContext.AddLoadMessage(this.GetText("WARN_WATCHLOGIN"), MessageTypes.warning);
                return;
            }

            if (this.WatchTopicID.InnerText == string.Empty)
            {
                this.GetRepository<WatchTopic>().Add(this.PageContext.PageUserID, this.PageContext.PageTopicID);
                this.PageContext.AddLoadMessage(this.GetText("INFO_WATCH_TOPIC"), MessageTypes.warning);
            }
            else
            {
                var tmpID = this.WatchTopicID.InnerText.ToType<int>();

                this.GetRepository<WatchTopic>().DeleteById(tmpID);

                this.PageContext.AddLoadMessage(this.GetText("INFO_UNWATCH_TOPIC"), MessageTypes.info);
            }

            this.HandleWatchTopic();
        }

        /// <summary>
        /// The unlock topic_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UnlockTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageContext.ForumModeratorAccess)
            {
                BuildLink.AccessDenied(/*"You are not a forum moderator."*/);
            }

            this.topic.TopicFlags.IsLocked = false;

            this.GetRepository<Topic>().LockTopic(
                this.PageContext.PageTopicID,
                this.topic.TopicFlags.BitValue);

            this.PageContext.AddLoadMessage(this.GetText("INFO_TOPIC_UNLOCKED"), MessageTypes.info);

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
        private void AddMetaData([NotNull] object firstMessage)
        {
            try
            {
                if (firstMessage.IsNullOrEmptyDBField())
                {
                    return;
                }
            }
            catch (Exception)
            {
               return;
            }
            
            if (this.Page.Header == null || !this.Get<BoardSettings>().AddDynamicPageMetaTags)
            {
                return;
            }

            var message = this.Get<IFormatMessage>().GetCleanedTopicMessage(
                firstMessage, this.PageContext.PageTopicID);
            var meta = this.Page.Header.FindControlType<HtmlMeta>();

            var htmlMetas = meta as IList<HtmlMeta> ?? meta.ToList();
            if (message.MessageTruncated.IsSet())
            {
                HtmlMeta descriptionMeta;

                string descriptionContent;

                // Use Topic Description if set
                if (!this.topic.Description.IsNullOrEmptyDBField())
                {
                    var topicDescription =
                        this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.topic.Description));

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
            if (this.topic == null)
            {
                BuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
            }

            this.dataBound = true;

            var showDeleted = false;
            var userId = this.PageContext.PageUserID;

            if (this.PageContext.IsAdmin
                || this.PageContext.ForumModeratorAccess || this.Get<BoardSettings>().ShowDeletedMessagesToAll)
            {
                showDeleted = true;
            }

            if (!this.Get<BoardSettings>().ShowDeletedMessages
                || !this.Get<BoardSettings>().ShowDeletedMessagesToAll)
            {
                // normally, users can always see own deleted topics or stubs we set a false id to hide them.
                userId = -1;
            }

            var findMessageId = this.GetFindMessageId(showDeleted, userId, out var messagePosition);
            if (findMessageId > 0)
            {
                this.CurrentMessage = findMessageId;
            }

            // Mark topic read
            this.Get<IReadTrackCurrentUser>().SetTopicRead(this.PageContext.PageTopicID);
            this.Pager.PageSize = this.Get<BoardSettings>().PostsPerPage;

            var postListDataTable = this.GetRepository<Message>().PostListAsDataTable(
                this.PageContext.PageTopicID,
                this.PageContext.PageUserID,
                userId,
                this.IsPostBack || this.PageContext.IsCrawler ? 0 : 1,
                showDeleted,
                this.Get<BoardSettings>().UseStyledNicks,
                !this.PageContext.IsGuest && this.Get<BoardSettings>().DisplayPoints,
                DateTimeHelper.SqlDbMinTime(),
                System.DateTime.UtcNow,
                DateTimeHelper.SqlDbMinTime(),
                System.DateTime.UtcNow,
                this.Pager.CurrentPageIndex,
                this.Pager.PageSize,
                1,
                0,
                0,
                this.Get<BoardSettings>().EnableThanksMod,
                messagePosition);

            if (this.Get<BoardSettings>().EnableThanksMod)
            {
                // Add necessary columns for later use in displaypost.ascx (Prevent repetitive
                // calls to database.)
                if (!postListDataTable.Columns.Contains("ThanksInfo"))
                {
                    postListDataTable.Columns.Add("ThanksInfo", typeof(string));
                }

                postListDataTable.Columns.AddRange(
                    new[]
                        {
                            // How many times has this message been thanked.
                            new DataColumn("IsThankedByUser", typeof(bool)),

                            //// How many times has the message poster thanked others?
                            new DataColumn("MessageThanksNumber", typeof(int)),

                            //// How many times has the message poster been thanked?
                            new DataColumn("ThanksFromUserNumber", typeof(int)),

                            //// In how many posts has the message poster been thanked?
                            new DataColumn("ThanksToUserNumber", typeof(int)),

                            //// In how many posts has the message poster been thanked?
                            new DataColumn("ThanksToUserPostsNumber", typeof(int))
                        });

                postListDataTable.AcceptChanges();
            }

            if (this.Get<BoardSettings>().UseStyledNicks)
            {
                // needs to be moved to the paged data below -- so it doesn't operate on unnecessary rows
                this.Get<IStyleTransform>().DecodeStyleByTable(postListDataTable, true);
            }

            if (!postListDataTable.HasRows())
            {
                throw new NoPostsFoundForTopicException(
                    this.PageContext.PageTopicID,
                    this.PageContext.PageUserID,
                    userId,
                    this.IsPostBack || this.PageContext.IsCrawler ? 0 : 1,
                    showDeleted,
                    this.Get<BoardSettings>().UseStyledNicks,
                    !this.PageContext.IsGuest && this.Get<BoardSettings>().DisplayPoints,
                    DateTimeHelper.SqlDbMinTime(),
                    System.DateTime.UtcNow,
                    DateTimeHelper.SqlDbMinTime(),
                    System.DateTime.UtcNow,
                    this.Pager.CurrentPageIndex,
                    this.Pager.PageSize,
                    1,
                    0,
                    0,
                    this.Get<BoardSettings>().EnableThanksMod,
                    messagePosition);
            }

            // convert to linq...
            var rowList = postListDataTable.AsEnumerable();

            var firstPost = rowList.First();

            // set the sorting
            this.Pager.Count = firstPost.Field<int>("TotalRows");

            if (findMessageId > 0)
            {
                this.Pager.CurrentPageIndex = firstPost.Field<int>("PageIndex");

                // move to this message on load...
                if (!this.PageContext.IsCrawler)
                {
                    this.PageContext.PageElements.RegisterJsBlockStartup(
                        this, "GotoAnchorJs", JavaScriptBlocks.LoadGotoAnchor($"post{findMessageId}"));
                }
            }
            else
            {
                // move to this message on load...
                if (!this.PageContext.IsCrawler)
                {
                    this.PageContext.PageElements.RegisterJsBlockStartup(
                        this,
                        "GotoAnchorJs",
                        JavaScriptBlocks.LoadGotoAnchor($"post{firstPost.Field<int>("MessageID")}"));
                }
            }

            var pagedData = rowList; // .Skip(this.Pager.SkipIndex).Take(this.Pager.PageSize);

            // Add thanks info and styled nicks if they are enabled
            if (this.Get<BoardSettings>().EnableThanksMod)
            {
                this.Get<DataBroker>().AddThanksInfo(pagedData);
            }

            // if current index is 0 we are on the first page and the metadata can be added.
            if (this.Pager.CurrentPageIndex == 0)
            {
                // handle add description/keywords for SEO
                this.AddMetaData(pagedData.First()["Message"]);
            }

            this.MessageList.DataSource = pagedData;

            this.DataBind();
        }

        /// <summary>
        /// Gets the message ID if "find" is in the query string
        /// </summary>
        /// <param name="showDeleted">
        /// The show Deleted.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="messagePosition">
        /// The message Position.
        /// </param>
        /// <returns>
        /// The get find message id.
        /// </returns>
        private int GetFindMessageId(bool showDeleted, int userId, out int messagePosition)
        {
            var findMessageId = 0;
            messagePosition = -1;

            try
            {
                // temporary find=lastpost code until all last/unread post links are find=lastpost and find=unread
                if (!this.Get<HttpRequestBase>().QueryString.Exists("find"))
                {
                    if (this.Get<HttpRequestBase>().QueryString.Exists("m") && int.TryParse(
                            this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"),
                            out var messageId))
                    {
                        // we find message position always by time.
                        using (var lastPost = this.GetRepository<Message>().FindUnreadAsDataTable(
                            this.PageContext.PageTopicID,
                            messageId,
                            DateTimeHelper.SqlDbMinTime(),
                            showDeleted,
                            userId))
                        {
                            var unreadFirst = lastPost.AsEnumerable().FirstOrDefault();
                            if (unreadFirst != null)
                            {
                                findMessageId = unreadFirst.Field<int>("MessageID");
                                var first = lastPost.AsEnumerable().FirstOrDefault();
                                if (first != null)
                                {
                                    // if Message is deleted
                                    if (first["MessagePosition"] == DBNull.Value)
                                    {
                                        findMessageId = 0;
                                        return -1;
                                    }

                                    messagePosition = first.Field<int>("MessagePosition");
                                }
                            }
                        }
                    }
                }
                else
                {
                    switch (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("find").ToLower())
                    {
                        case "unread":
                            {
                                // find first unread message
                                var lastRead = !this.PageContext.IsCrawler
                                                   ? this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                                                       this.PageContext.PageForumID,
                                                       this.PageContext.PageTopicID)
                                                   : System.DateTime.UtcNow;

                                using (var unread = this.GetRepository<Message>().FindUnreadAsDataTable(
                                    this.PageContext.PageTopicID,
                                    0,
                                    lastRead,
                                    showDeleted,
                                    userId))
                                {
                                    var unreadFirst = unread.AsEnumerable().FirstOrDefault();
                                    if (unreadFirst != null)
                                    {
                                        // if Message is deleted
                                        if (unreadFirst["MessagePosition"] == DBNull.Value)
                                        {
                                            findMessageId = 0;
                                            return -1;
                                        }

                                        findMessageId = unreadFirst.Field<int>("MessageID");
                                        messagePosition = unreadFirst.Field<int>("MessagePosition");

                                        if (this.Get<HttpRequestBase>().QueryString.Exists("m") && this.PageContext.CurrentUserData.Activity)
                                        {
                                            this.GetRepository<Activity>().UpdateNotification(this.PageContext.PageUserID, findMessageId);
                                        }
                                    }
                                }
                            }

                            break;
                        case "lastpost":
                            using (var unread = this.GetRepository<Message>().FindUnreadAsDataTable(
                                this.PageContext.PageTopicID,
                                0,
                                System.DateTime.UtcNow,
                                showDeleted,
                                userId))
                            {
                                var unreadFirst = unread.AsEnumerable().FirstOrDefault();
                                if (unreadFirst != null)
                                {
                                    // if Message is deleted
                                    if (unreadFirst["MessagePosition"] == DBNull.Value)
                                    {
                                        findMessageId = 0;
                                        return -1;
                                    }

                                    findMessageId = unreadFirst.Field<int>("MessageID");
                                    messagePosition = unreadFirst.Field<int>("MessagePosition");
                                }
                            }

                            break;
                    }
                }
            }
            catch (Exception x)
            {
                this.Logger.Log(this.PageContext.PageUserID, this, x);
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
            if (this.PageContext.IsGuest)
            {
                return false;
            }

            var watchTopicId = this.GetRepository<WatchTopic>().Check(this.PageContext.PageUserID, this.PageContext.PageTopicID);

            // check if this forum is being watched by this user
            if (watchTopicId.HasValue)
            {
                // subscribed to this forum
                this.WatchTopicID.InnerText = watchTopicId.Value.ToString();

                return true;
            }

            // not subscribed
            this.WatchTopicID.InnerText = string.Empty;

            return false;
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // Poll.ItemCommand += Poll_ItemCommand;
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
            var topicUrl = BuildLink.GetLinkNotEscaped(ForumPages.Posts, true, "t={0}", this.PageContext.PageTopicID);

            switch (e.Item.ToLower())
            {
                case "email":
                    this.EmailTopic_Click(sender, e);
                    break;
                case "tumblr":
                    {
                        // process message... clean html, strip html, remove BBCode, etc...
                        var tumblrTopicName =
                            BBCodeHelper.StripBBCode(
                                HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(this.topic.TopicName))).RemoveMultipleWhitespace();

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
                        var twitterName = this.Get<BoardSettings>().TwitterUserName.IsSet()
                                              ? $"@{this.Get<BoardSettings>().TwitterUserName} "
                                              : string.Empty;

                        // process message... clean html, strip html, remove bbcode, etc...
                        var twitterMsg =
                            BBCodeHelper.StripBBCode(
                                HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(this.topic.TopicName))).RemoveMultipleWhitespace();

                        var tweetUrl =
                            $"http://twitter.com/share?url={this.Server.UrlEncode(topicUrl)}&text={this.Server.UrlEncode(string.Format("RT {1}Thread: {0}", twitterMsg.Truncate(100), twitterName))}";

                        // Send Re-tweet directly thru the Twitter API if User is Twitter User
                        if (Config.TwitterConsumerKey.IsSet() && Config.TwitterConsumerSecret.IsSet()
                            && this.Get<ISession>().TwitterToken.IsSet()
                            && this.Get<ISession>().TwitterTokenSecret.IsSet() && this.PageContext.IsTwitterUser)
                        {
                            var auth = new OAuthTwitter
                                {
                                    ConsumerKey = Config.TwitterConsumerKey,
                                    ConsumerSecret = Config.TwitterConsumerSecret,
                                    Token = this.Get<ISession>().TwitterToken,
                                    TokenSecret = this.Get<ISession>().TwitterTokenSecret
                                };

                            var tweets = new TweetAPI(auth);

                            tweets.UpdateStatus(
                                TweetAPI.ResponseFormat.json,
                                this.Server.UrlEncode(
                                    string.Format("RT {1}: {0} {2}", twitterMsg.Truncate(100), twitterName, topicUrl)),
                                string.Empty);
                        }
                        else
                        {
                            this.Get<HttpResponseBase>().Redirect(tweetUrl);
                        }
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
                    BuildLink.Redirect(ForumPages.PrintTopic, "t={0}", this.PageContext.PageTopicID);
                    break;
                case "watch":
                    this.TrackTopic_Click(sender, e);
                    break;
                case "email":
                    this.EmailTopic_Click(sender, e);
                    break;
                case "rssfeed":
                    BuildLink.Redirect(
                        ForumPages.RssTopic,
                        "pg={0}&t={1}&ft=0",
                        RssFeeds.Posts.ToInt(),
                        this.PageContext.PageTopicID);
                    break;
                case "atomfeed":
                    BuildLink.Redirect(
                        ForumPages.RssTopic,
                        "pg={0}&t={1}&ft=1",
                        RssFeeds.Posts.ToInt(),
                        this.PageContext.PageTopicID);
                    break;
                default:
                    throw new ApplicationException(e.Item);
            }
        }
    }
}