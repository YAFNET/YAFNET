/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Web.Controls
{
    using System;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Services;
    using YAF.Core.Utilities.Helpers;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Objects.Model;

    using DateTime = System.DateTime;

    /// <summary>
    /// Topic Container Control
    /// </summary>
    [ToolboxData("<{0}:TopicContainer runat=server></{0}:TopicContainer>")]
    public class TopicContainer : BaseControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether AllowSelection.
        /// </summary>
        public bool AllowSelection
        {
            get => this.ViewState["AllowSelection"] != null && this.ViewState["AllowSelection"].ToType<bool>();

            set => this.ViewState["AllowSelection"] = value;
        }

        /// <summary>
        /// Sets the item.
        /// </summary>
        public PagedTopic Item
        {
            set
            {
                this.TopicItem = value;

                this.TopicRowID = this.TopicItem.LinkTopicID;
            }
        }

        /// <summary>
        ///   Gets or sets TopicRowID.
        /// </summary>
        public int? TopicRowID
        {
            get => this.ViewState["TopicRowID"].ToType<int?>();

            set => this.ViewState["TopicRowID"] = value;
        }

        /// <summary>
        ///  Gets the TopicRow.
        /// </summary>
        protected PagedTopic TopicItem { get; private set; }

        #endregion

        /// <summary>
        /// Checks if the Topic is Hot or not
        /// </summary>
        /// <param name="lastPosted">
        /// The last Posted DateTime.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// Returns if the Topic is Hot or not
        /// </returns>
        public bool IsPopularTopic(DateTime lastPosted, PagedTopic item)
        {
            if (lastPosted > DateTime.Now.AddDays(-this.PageContext.BoardSettings.PopularTopicDays))
            {
                return item.Replies >= this.PageContext.BoardSettings.PopularTopicReplys ||
                       item.Views >= this.PageContext.BoardSettings.PopularTopicViews;
            }

            return false;
        }

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!this.Visible)
            {
                return;
            }

            var lastRead = this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                this.TopicItem.ForumID,
                this.TopicItem.TopicID,
                this.TopicItem.LastForumAccess ?? DateTimeHelper.SqlDbMinTime(),
                this.TopicItem.LastTopicAccess ?? DateTimeHelper.SqlDbMinTime());

            if (!this.AllowSelection)
            {
                writer.WriteBeginTag(HtmlTextWriterTag.Div.ToString());
                writer.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), "row");
                writer.Write(HtmlTextWriter.TagRightChar);

                writer.WriteBeginTag(HtmlTextWriterTag.Div.ToString());
                writer.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), "col-md-8");
                writer.Write(HtmlTextWriter.TagRightChar);

                writer.WriteBeginTag(HtmlTextWriterTag.H5.ToString());
                writer.Write(HtmlTextWriter.TagRightChar);
            }

            writer.WriteBeginTag(HtmlTextWriterTag.A.ToString());

            writer.WriteAttribute(HtmlTextWriterAttribute.Tabindex.ToString(), "0");
            writer.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), "topic-icon-legend-popvover");
            writer.WriteAttribute("role", "button");
            writer.WriteAttribute("data-bs-toggle", "popover");
            writer.WriteAttribute(HtmlTextWriterAttribute.Href.ToString(), "#!");

            writer.Write(HtmlTextWriter.TagRightChar);

            this.GetTopicIcon(this.TopicItem, lastRead).RenderControl(writer);

            writer.WriteEndTag(HtmlTextWriterTag.A.ToString());

            this.RenderPriorityMessage(writer, this.TopicItem);

            var topicLink = new HyperLink
            {
                NavigateUrl = this.Get<LinkBuilder>().GetTopicLink(
                    this.TopicItem.LinkTopicID,
                    this.TopicItem.Subject),
                Text = this.FormatTopicName(),
                CssClass = "topic-starter-popover"
            };

            topicLink.Attributes.Add("data-bs-toggle", "popover");

            var topicStartedDateTime = this.TopicItem.Posted;

            var formattedStartedDatetime = this.PageContext.BoardSettings.ShowRelativeTime
                ? topicStartedDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
                : this.Get<IDateTimeService>().Format(DateTimeFormat.BothTopic, topicStartedDateTime);

            var topicStarterLink = new UserLink
            {
                IsGuest = true,
                Suspended = this.TopicItem.StarterSuspended,
                UserID = this.TopicItem.UserID,
                ReplaceName = this.PageContext.BoardSettings.EnableDisplayName ? this.TopicItem.StarterDisplay : this.TopicItem.Starter,
                Style = this.TopicItem.StarterStyle
            };

            var span = this.PageContext.BoardSettings.ShowRelativeTime ? @"<span class=""popover-timeago"">" : "<span>";

            var dateTimeIcon = new Icon
            {
                IconName = "calendar-day",
                IconType = "text-secondary",
                IconNameBadge = "clock",
                IconBadgeType = "text-secondary"
            }.RenderToString();

            topicLink.Attributes.Add(
                "data-bs-content",
                $@"{topicStarterLink.RenderToString()}{dateTimeIcon}{span}{formattedStartedDatetime}</span>");

            if (this.TopicItem.LastMessageID.HasValue && this.TopicItem.LastPosted > lastRead)
            {
                var success = new Label { CssClass = "badge bg-success me-1", Text = this.GetText("NEW_POSTS") };

                success.RenderControl(writer);
            }

            writer.Write(topicLink.RenderToString());

            var favoriteCount = this.TopicItem.FavoriteCount;

            if (favoriteCount > 0)
            {
                var favoriteLabel = new Label
                {
                    CssClass = "badge bg-light text-dark ms-1",
                    Text = new IconHeader
                    {
                        IconName = "star", IconType = " ", IconStyle = "far", Text = favoriteCount.ToString()
                    }.RenderToString(),
                    ToolTip = this.GetText("FAVORITE_COUNT_TT")
                };

                favoriteLabel.Attributes.Add("data-bs-toggle", "tooltip");

                favoriteLabel.RenderControl(writer);
            }

            // Render Replies & Views
            var repliesLabel = new Label
            {
                CssClass = "badge bg-light text-dark ms-1 me-1",
                Text = new IconHeader
                {
                    IconName = "comment", IconType = " ", IconStyle = "far", Text = this.FormatReplies()
                }.RenderToString(),
                ToolTip = this.GetText("MODERATE", "REPLIES"),
            };

            repliesLabel.Attributes.Add("data-bs-toggle", "tooltip");

            repliesLabel.RenderControl(writer);

            var viewsLabel = new Label
            {
                CssClass = "badge bg-light text-dark",
                Text = new IconHeader
                {
                    IconName = "eye", IconType = " ", IconStyle = "far", Text = this.FormatViews()
                }.RenderToString(),
                ToolTip = this.GetText("MODERATE", "VIEWS"),
            };

            viewsLabel.Attributes.Add("data-bs-toggle", "tooltip");

            viewsLabel.RenderControl(writer);

            // Render Pager
            var actualPostCount = this.TopicItem.Replies + 1;

            if (this.PageContext.BoardSettings.ShowDeletedMessages)
            {
                // add deleted posts not included in replies...
                actualPostCount += this.TopicItem.NumPostsDeleted;
            }

            this.CreatePostPager(
                writer,
                actualPostCount,
                this.PageContext.BoardSettings.PostsPerPage,
                this.TopicItem.LinkTopicID);

            writer.WriteEndTag(HtmlTextWriterTag.H5.ToString());

            var topicDescription = this.TopicItem.Description;

            if (topicDescription.IsSet())
            {
                writer.Write($"<h6 class=\"card-subtitle text-muted\">{topicDescription}</h6>");
            }

            writer.WriteEndTag(HtmlTextWriterTag.Div.ToString());

            if (this.TopicItem.LastMessageID.HasValue)
            {
                writer.Write("<div class=\"col-md-4 text-secondary\">");

                writer.Write($"{this.GetText("LASTPOST")}:");

                var infoLastPost = new ThemeButton
                {
                    Size = ButtonSize.Small,
                    Icon = "info-circle",
                    IconColor = "text-secondary",
                    Type = ButtonStyle.Link,
                    DataToggle = "popover",
                    CssClass = "topic-link-popover",
                    NavigateUrl = "#!"
                };

                var lastPostedDateTime = this.TopicItem.LastPosted;

                var formattedDatetime = this.PageContext.BoardSettings.ShowRelativeTime
                    ? lastPostedDateTime.Value.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
                    : this.Get<IDateTimeService>().Format(DateTimeFormat.BothTopic, lastPostedDateTime);

                var userLast = new UserLink
                {
                    IsGuest = true,
                    Suspended = this.TopicItem.LastUserSuspended,
                    UserID = this.TopicItem.LastUserID.Value,
                    ReplaceName = this.PageContext.BoardSettings.EnableDisplayName
                            ? this.TopicItem.LastUserDisplayName
                            : this.TopicItem.LastUserName,
                    Style = this.TopicItem.LastUserStyle
                };

                infoLastPost.DataContent =
                    $@"{userLast.RenderToString()}{dateTimeIcon}{span}{formattedDatetime}</span>";

                infoLastPost.TextLocalizedTag = "by";
                infoLastPost.TextLocalizedPage = "DEFAULT";
                infoLastPost.ParamText0 = this.PageContext.BoardSettings.EnableDisplayName
                    ? this.TopicItem.LastUserDisplayName
                    : this.TopicItem.LastUserName;

                writer.Write(infoLastPost.RenderToString());

                var gotoLastPost = new ThemeButton
                {
                    NavigateUrl =
                       this.Get<LinkBuilder>().GetLink(
                            ForumPages.Posts,
                            "t={0}&name={1}",
                            this.TopicItem.TopicID,
                            this.TopicItem.Subject),
                    Size = ButtonSize.Small,
                    Icon = "share-square",
                    Type = ButtonStyle.OutlineSecondary,
                    TitleLocalizedTag = "GO_LAST_POST",
                    DataToggle = "tooltip"
                };

                gotoLastPost.RenderControl(writer);

                writer.WriteEndTag(HtmlTextWriterTag.Div.ToString());
            }

            writer.WriteEndTag(HtmlTextWriterTag.Div.ToString());
        }

        /// <summary>
        /// Create pager for post.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="pageSize">
        /// The page Size.
        /// </param>
        /// <param name="topicID">
        /// The topic ID.
        /// </param>
        protected void CreatePostPager(HtmlTextWriter writer, int count, int pageSize, int topicID)
        {
            const int NumToDisplay = 4;
            var pageCount = (int)Math.Ceiling((double)count / pageSize);

            if (pageCount <= 1)
            {
                // No Paging
                return;
            }

            writer.WriteBeginTag(HtmlTextWriterTag.Div.ToString());

            writer.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), "btn-group btn-group-sm ms-2");

            writer.WriteAttribute("role", "group");

            writer.WriteAttribute("aria-label", "topic pager");

            writer.Write(HtmlTextWriter.TagRightChar);

            if (pageCount > NumToDisplay)
            {
                this.MakeLink(
                    "1",
                    this.Get<LinkBuilder>().GetLink(ForumPages.Posts, "t={0}&name={1}", topicID, this.TopicItem.Subject),
                    1).RenderControl(writer);

                // show links from the end
                for (var i = pageCount - (NumToDisplay - 1); i < pageCount; i++)
                {
                    var post = i + 1;

                    this.MakeLink(
                        post.ToString(),
                        this.Get<LinkBuilder>().GetLink(
                            ForumPages.Posts,
                            "t={0}&name={2}&p={1}",
                            topicID,
                            post,
                            this.TopicItem.Subject),
                        post).RenderControl(writer);
                }
            }
            else
            {
                for (var i = 0; i < pageCount; i++)
                {
                    var post = i + 1;

                    this.MakeLink(
                        post.ToString(),
                        this.Get<LinkBuilder>().GetLink(
                            ForumPages.Posts,
                            "t={0}&name={2}&p={1}",
                            topicID,
                            post,
                            this.TopicItem.Subject),
                        post).RenderControl(writer);
                }
            }

            writer.WriteEndTag(HtmlTextWriterTag.Div.ToString());
        }

        /// <summary>
        /// Formats the replies.
        /// </summary>
        /// <returns>
        /// Returns the formatted replies.
        /// </returns>
        protected string FormatReplies()
        {
            var repStr = "&nbsp;";

            var replies = this.TopicItem.Replies;
            var numDeleted = this.TopicItem.NumPostsDeleted;

            if (replies < 0)
            {
                return repStr;
            }

            if (this.PageContext.BoardSettings.ShowDeletedMessages && numDeleted > 0)
            {
                repStr = $"{replies + numDeleted:N0}";
            }
            else
            {
                repStr = $"{replies:N0}";
            }

            return repStr;
        }

        /// <summary>
        /// Formats the views.
        /// </summary>
        /// <returns>
        /// Returns the formatted views.
        /// </returns>
        protected string FormatViews()
        {
            var views = this.TopicItem.Views;
            return this.TopicItem.TopicMovedID.HasValue ? "&nbsp;" : $"{views:N0}";
        }

        /// <summary>
        /// Format the Topic Name and Add Status Icon/Text 
        /// if enabled and available
        /// </summary>
        /// <returns>
        /// Returns the Topic Name (with Status Icon)
        /// </returns>
        protected string FormatTopicName()
        {
            var topicSubject = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.TopicItem.Subject));

            var styles = this.PageContext.BoardSettings.UseStyledTopicTitles
                ? this.Get<IStyleTransform>().Decode(this.TopicItem.Styles)
                : string.Empty;

            if (styles.IsNotSet())
            {
                return topicSubject;
            }

            var nameLabel = new Label { Text = topicSubject };

            nameLabel.Attributes.Add("style", this.HtmlEncode(styles));

            return nameLabel.RenderToString();
        }

        /// <summary>
        /// Creates the status message text for a topic. (i.e. Moved, Poll, Sticky, etc.)
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        protected void RenderPriorityMessage([NotNull] HtmlTextWriter writer, [NotNull] PagedTopic item)
        {
            var priorityLabel = new Label();

            if (item.TopicMovedID.HasValue)
            {
                priorityLabel.Text = new IconHeader { LocalizedTag = "MOVED", IconName = "arrows-alt", IconType = " " }
                    .RenderToString();
                priorityLabel.CssClass = "badge bg-secondary me-1";

                priorityLabel.RenderControl(writer);
            }
            else if (item.PollID.HasValue)
            {
                priorityLabel.Text = new IconHeader { LocalizedTag = "POLL", IconName = "poll-h", IconType = " " }
                    .RenderToString();
                priorityLabel.CssClass = "badge bg-secondary me-1";

                priorityLabel.RenderControl(writer);
            }
            else
            {
                switch (item.Priority)
                {
                    case 1:
                        priorityLabel.Text =
                            new IconHeader { LocalizedTag = "STICKY", IconName = "thumbtack", IconType = " " }
                                .RenderToString();
                        priorityLabel.CssClass = "badge bg-warning text-dark me-1";

                        priorityLabel.RenderControl(writer);
                        break;
                    case 2:
                        priorityLabel.Text =
                            new IconHeader { LocalizedTag = "ANNOUNCEMENT", IconName = "bullhorn", IconType = " " }
                                .RenderToString();
                        priorityLabel.CssClass = "badge bg-primary me-1";

                        priorityLabel.RenderControl(writer);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the topic image.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="lastRead">
        /// The last Read.
        /// </param>
        /// <returns>
        /// Returns the Topic Image
        /// </returns>
        protected Icon GetTopicIcon([NotNull] PagedTopic item, DateTime lastRead)
        {
            var lastPosted = item.LastPosted ?? DateTimeHelper.SqlDbMinTime();

            var topicFlags = new TopicFlags(item.TopicFlags);
            var forumFlags = new ForumFlags(item.ForumFlags);

            var iconNew = new Icon
            {
                IconName = "comment",
                IconStackName = "comment",
                IconStackType = "fa-inverse",
                IconStackSize = "fa-1x",
                IconType = "text-success"
            };

            var icon = new Icon
            {
                IconName = "comment",
                IconStackName = "comment",
                IconStackType = "fa-inverse",
                IconStackSize = "fa-1x",
                IconType = "text-secondary"
            };

            var isHotTopic = this.IsPopularTopic(lastPosted, item);

            if (item.TopicMovedID.HasValue)
            {
                icon.IconStackName = "arrows-alt";

                return icon;
            }

            var topic = isHotTopic ? "fire" : "comment";

            if (lastPosted > lastRead)
            {
                this.Get<ISession>().UnreadTopics++;

                if (topicFlags.IsLocked || forumFlags.IsLocked)
                {
                    iconNew.IconStackName = "lock";
                    return iconNew;
                }

                iconNew.IconStackName = topic;

                return iconNew;
            }

            if (topicFlags.IsLocked || forumFlags.IsLocked)
            {
                icon.IconStackName = "lock";
                return icon;
            }

            icon.IconStackName = topic;

            return icon;
        }

        /// <summary>
        /// Makes the link.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="link">The link.</param>
        /// <param name="pageId">The page id.</param>
        /// <returns>
        /// Returns the created link.
        /// </returns>
        protected ThemeButton MakeLink([NotNull] string text, [NotNull] string link, [NotNull] int pageId)
        {
            return new()
            {
                NavigateUrl = link,
                TitleLocalizedTag = "GOTO_POST_PAGER",
                ParamTitle0 = pageId.ToString(),
                Type = ButtonStyle.Secondary,
                Size = ButtonSize.Small,
                Text = text
            };
        }
    }
}