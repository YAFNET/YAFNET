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

namespace YAF.Web.Controls
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

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
        ///   Sets DataRow.
        /// </summary>
        public object DataRow
        {
            set
            {
                this.TopicRow = value as DataRowView;
                this.TopicRowID = this.TopicRow?["LinkTopicID"].ToType<int>();
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
        protected DataRowView TopicRow { get; private set; }

        #endregion

        /// <summary>
        ///   Checks if the Topic is Hot or not
        /// </summary>
        /// <param name="lastPosted">
        ///   The last Posted DateTime.
        /// </param>
        /// <param name="row">
        ///   The Topic Data Row
        /// </param>
        /// <returns>
        ///   Returns if the Topic is Hot or not
        /// </returns>
        public bool IsPopularTopic(DateTime lastPosted, DataRowView row)
        {
            if (lastPosted > DateTime.Now.AddDays(-this.Get<BoardSettings>().PopularTopicDays))
            {
                return row["Replies"].ToType<int>() >= this.Get<BoardSettings>().PopularTopicReplys
                       || row["Views"].ToType<int>() >= this.Get<BoardSettings>().PopularTopicViews;
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
                this.TopicRow["ForumID"].ToType<int>(),
                this.TopicRow["TopicID"].ToType<int>(),
                this.TopicRow["LastForumAccess"].ToType<DateTime?>() ?? DateTimeHelper.SqlDbMinTime(),
                this.TopicRow["LastTopicAccess"].ToType<DateTime?>() ?? DateTimeHelper.SqlDbMinTime());

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
            writer.WriteAttribute("data-toggle", "popover");
            writer.WriteAttribute(HtmlTextWriterAttribute.Href.ToString(), "#!");

            writer.Write(HtmlTextWriter.TagRightChar);

            this.GetTopicIcon(this.TopicRow, lastRead).RenderControl(writer);

            writer.WriteEndTag(HtmlTextWriterTag.A.ToString());

            this.RenderPriorityMessage(writer, this.TopicRow);

            var topicLink = new HyperLink
                                {
                                    NavigateUrl = BuildLink.GetLinkNotEscaped(
                                        ForumPages.Posts,
                                        "t={0}",
                                        this.TopicRow["LinkTopicID"]),
                                    Text = this.FormatTopicName(),
                                    CssClass = "topic-starter-popover"
                                };

            topicLink.Attributes.Add("data-toggle", "popover");

            var topicStartedDateTime = this.TopicRow["Posted"].ToType<DateTime>();

            var formattedStartedDatetime = this.Get<BoardSettings>().ShowRelativeTime
                                               ? topicStartedDateTime.ToString(
                                                   "yyyy-MM-ddTHH:mm:ssZ",
                                                   CultureInfo.InvariantCulture)
                                               : this.Get<IDateTime>().Format(
                                                   DateTimeFormat.BothTopic,
                                                   topicStartedDateTime);

            var topicStarterLink = new UserLink
                                       {
                                           UserID = this.TopicRow["UserID"].ToType<int>(),
                                           ReplaceName = this
                                               .TopicRow[this.Get<BoardSettings>().EnableDisplayName
                                                             ? "StarterDisplay"
                                                             : "Starter"].ToString(),
                                           Style = this.TopicRow["StarterStyle"].ToString()
                                       };

            var span = this.Get<BoardSettings>().ShowRelativeTime ? @"<span class=""popover-timeago"">" : "<span>";

            var dateTimeIcon = new Icon
                                   {
                                       IconName = "calendar-day",
                                       IconType = "text-secondary",
                                       IconNameBadge = "clock",
                                       IconBadgeType = "text-secondary"
                                   }.RenderToString();

            topicLink.Attributes.Add(
                "data-content",
                $@"{topicStarterLink.RenderToString()}{dateTimeIcon}{span}{formattedStartedDatetime}</span>");

            if (!this.TopicRow["LastMessageID"].IsNullOrEmptyDBField())
            {
                if (this.TopicRow["LastPosted"].ToType<DateTime>() > lastRead)
                {
                    var success = new Label { CssClass = "badge badge-success mr-1", Text = this.GetText("NEW_POSTS") };

                    success.RenderControl(writer);
                }
            }

            writer.Write(topicLink.RenderToString());

            var favoriteCount = this.TopicRow["FavoriteCount"].ToType<int>();

            if (favoriteCount > 0)
            {
                var favoriteLabel = new Label
                                        {
                                            CssClass = "badge badge-light ml-1",
                                            Text = new IconHeader
                                                       {
                                                           IconName = "star",
                                                           IconType = " ",
                                                           IconStyle = "far",
                                                           Text = favoriteCount.ToString()
                                                       }.RenderToString(),
                                            ToolTip = this.GetText("FAVORITE_COUNT_TT")
                                        };

                favoriteLabel.Attributes.Add("data-toggle", "tooltip");

                favoriteLabel.RenderControl(writer);
            }

            // Render Replies & Views
            var repliesLabel = new Label
                                   {
                                       CssClass = "badge badge-light ml-1 mr-1",
                                       Text = new IconHeader
                                                  {
                                                      IconName = "comment",
                                                      IconType = " ",
                                                      IconStyle = "far",
                                                      Text = this.FormatReplies()
                                                  }.RenderToString(),
                                       ToolTip = this.GetText("MODERATE", "REPLIES"),
                                   };

            repliesLabel.Attributes.Add("data-toggle", "tooltip");

            repliesLabel.RenderControl(writer);

            var viewsLabel = new Label
                                 {
                                     CssClass = "badge badge-light",
                                     Text = new IconHeader
                                                {
                                                    IconName = "eye",
                                                    IconType = " ",
                                                    IconStyle = "far",
                                                    Text = this.FormatViews()
                                                }.RenderToString(),
                                     ToolTip = this.GetText("MODERATE", "VIEWS"),
                                 };

            viewsLabel.Attributes.Add("data-toggle", "tooltip");

            viewsLabel.RenderControl(writer);

            // Render Pager
            var actualPostCount = this.TopicRow["Replies"].ToType<int>() + 1;

            if (this.Get<BoardSettings>().ShowDeletedMessages)
            {
                // add deleted posts not included in replies...");
                actualPostCount += this.TopicRow["NumPostsDeleted"].ToType<int>();
            }

            this.CreatePostPager(
                writer,
                actualPostCount,
                this.Get<BoardSettings>().PostsPerPage,
                this.TopicRow["LinkTopicID"].ToType<int>());

            writer.WriteEndTag(HtmlTextWriterTag.H5.ToString());

            var topicDescription = this.TopicRow["Description"].ToString();

            if (topicDescription.IsSet())
            {
                writer.Write($"<h6 class=\"card-subtitle text-muted\">{topicDescription}</h6>");
            }

            writer.WriteEndTag(HtmlTextWriterTag.Div.ToString());

            if (!this.TopicRow["LastMessageID"].IsNullOrEmptyDBField())
            {
                writer.Write("<div class=\"col-md-4 text-secondary\">");

                writer.Write($"{this.GetText("LASTPOST")}:");

                var infoLastPost = new ThemeButton
                                       {
                                           Size = ButtonSize.Small,
                                           Icon = "info-circle",
                                           IconCssClass = "fas fa-lg",
                                           IconColor = "text-secondary",
                                           Type = ButtonStyle.Link,
                                           DataToggle = "popover",
                                           CssClass = "topic-link-popover",
                                           NavigateUrl = "#!"
                                       };

                var lastPostedDateTime = this.TopicRow["LastPosted"].ToType<DateTime>();

                var formattedDatetime = this.Get<BoardSettings>().ShowRelativeTime
                                            ? lastPostedDateTime.ToString(
                                                "yyyy-MM-ddTHH:mm:ssZ",
                                                CultureInfo.InvariantCulture)
                                            : this.Get<IDateTime>().Format(
                                                DateTimeFormat.BothTopic,
                                                lastPostedDateTime);

                var userLast = new UserLink
                                   {
                                       UserID = this.TopicRow["LastUserID"].ToType<int>(),
                                       ReplaceName = this
                                           .TopicRow[this.Get<BoardSettings>().EnableDisplayName
                                                         ? "LastUserDisplayName"
                                                         : "LastUserName"].ToString(),
                                       Style = this.TopicRow["LastUserStyle"].ToString()
                                   };

                infoLastPost.DataContent =
                    $@"{userLast.RenderToString()}{dateTimeIcon}{span}{formattedDatetime}</span>";

                infoLastPost.TextLocalizedTag = "by";
                infoLastPost.TextLocalizedPage = "DEFAULT";
                infoLastPost.ParamText0 = this
                    .TopicRow[this.Get<BoardSettings>().EnableDisplayName ? "LastUserDisplayName" : "LastUserName"]
                    .ToString();

                writer.Write(infoLastPost.RenderToString());

                var gotoLastPost = new ThemeButton
                                       {
                                           NavigateUrl =
                                               BuildLink.GetLink(
                                                   ForumPages.Posts,
                                                   "m={0}#post{0}",
                                                   this.TopicRow["LastMessageID"]),
                                           Size = ButtonSize.Small,
                                           Icon = "share-square",
                                           Type = ButtonStyle.OutlineSecondary,
                                           TitleLocalizedTag = "GO_LAST_POST",
                                           DataToggle = "tooltip"
                                       };

                var gotoLastUnread = new ThemeButton
                                         {
                                             NavigateUrl =
                                                 BuildLink.GetLink(
                                                     ForumPages.Posts,
                                                     "t={0}&find=unread",
                                                     this.TopicRow["TopicID"]),
                                             Size = ButtonSize.Small,
                                             Icon = "book-reader",
                                             Type = ButtonStyle.OutlineSecondary,
                                             TitleLocalizedTag = "GO_LASTUNREAD_POST",
                                             DataToggle = "tooltip"
                                         };

                writer.Write(@"<div class=""btn-group"" role=""group"">");
                gotoLastUnread.RenderControl(writer);
                gotoLastPost.RenderControl(writer);
                writer.WriteEndTag(HtmlTextWriterTag.Div.ToString());

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

            writer.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), "btn-group btn-group-sm mb-1");

            writer.WriteAttribute("role", "group");

            writer.WriteAttribute("aria-label", "topic pager");

            writer.Write(HtmlTextWriter.TagRightChar);

            if (pageCount > NumToDisplay)
            {
                this.MakeLink("1", BuildLink.GetLink(ForumPages.Posts, "t={0}", topicID), 1).RenderControl(writer);
                writer.Write(" ... ");

                // show links from the end
                for (var i = pageCount - (NumToDisplay - 1); i < pageCount; i++)
                {
                    var post = i + 1;

                    this.MakeLink(
                        post.ToString(),
                        BuildLink.GetLink(ForumPages.Posts, "t={0}&p={1}", topicID, post),
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
                        BuildLink.GetLink(ForumPages.Posts, "t={0}&p={1}", topicID, post),
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

            var replies = this.TopicRow["Replies"].ToType<int>();
            var numDeleted = this.TopicRow["NumPostsDeleted"].ToType<int>();

            if (replies < 0)
            {
                return repStr;
            }

            if (this.Get<BoardSettings>().ShowDeletedMessages && numDeleted > 0)
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
            var views = this.TopicRow["Views"].ToType<int>();
            return this.TopicRow["TopicMovedID"].ToString().Length > 0 ? "&nbsp;" : $"{views:N0}";
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
            var topicSubject = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.TopicRow["Subject"]));

            var styles = this.Get<BoardSettings>().UseStyledTopicTitles
                             ? this.Get<IStyleTransform>().DecodeStyleByString(this.TopicRow["Styles"].ToString())
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
        /// <param name="row">
        /// Current Topic Data Row
        /// </param>
        protected void RenderPriorityMessage([NotNull] HtmlTextWriter writer, [NotNull] DataRowView row)
        {
            CodeContracts.VerifyNotNull(row, "row");

            var priorityLabel = new Label();

            if (row["TopicMovedID"].ToString().Length > 0)
            {
                priorityLabel.Text = new IconHeader { LocalizedTag = "MOVED", IconName = "arrows-alt", IconType = " " }
                    .RenderToString();
                priorityLabel.CssClass = "badge badge-secondary mr-1";

                priorityLabel.RenderControl(writer);
            }
            else if (row["PollID"].ToString() != string.Empty)
            {
                priorityLabel.Text = new IconHeader { LocalizedTag = "POLL", IconName = "poll-h", IconType = " " }
                    .RenderToString();
                priorityLabel.CssClass = "badge badge-secondary mr-1";

                priorityLabel.RenderControl(writer);
            }
            else
            {
                switch (int.Parse(row["Priority"].ToString()))
                {
                    case 1:
                        priorityLabel.Text =
                            new IconHeader { LocalizedTag = "STICKY", IconName = "thumbtack", IconType = " " }
                                .RenderToString();
                        priorityLabel.CssClass = "badge badge-warning mr-1";

                        priorityLabel.RenderControl(writer);
                        break;
                    case 2:
                        priorityLabel.Text =
                            new IconHeader { LocalizedTag = "ANNOUNCEMENT", IconName = "bullhorn", IconType = " " }
                                .RenderToString();
                        priorityLabel.CssClass = "badge badge-primary mr-1";

                        priorityLabel.RenderControl(writer);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the topic image.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="lastRead">
        /// The last Read.
        /// </param>
        /// <returns>
        /// Returns the Topic Image
        /// </returns>
        protected Icon GetTopicIcon([NotNull] DataRowView row, DateTime lastRead)
        {
            CodeContracts.VerifyNotNull(row, "row");

            var lastPosted = row["LastPosted"] != DBNull.Value
                                 ? (DateTime)row["LastPosted"]
                                 : DateTimeHelper.SqlDbMinTime();

            var topicFlags = new TopicFlags(row["TopicFlags"]);
            var forumFlags = new ForumFlags(row["ForumFlags"]);

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

            var isHotTopic = this.IsPopularTopic(lastPosted, row);

            if (row["TopicMovedID"].ToString().Length > 0)
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
        protected HyperLink MakeLink([NotNull] string text, [NotNull] string link, [NotNull] int pageId)
        {
            return new HyperLink
                       {
                           NavigateUrl = link,
                           ToolTip = this.GetTextFormatted("GOTO_POST_PAGER", pageId),
                           CssClass = "btn btn-secondary btn-sm",
                           Text = text
                       };
        }
    }
} 