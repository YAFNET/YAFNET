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
    using System.Text;
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
            if (lastPosted > DateTime.Now.AddDays(-this.Get<YafBoardSettings>().PopularTopicDays))
            {
                return row["Replies"].ToType<int>() >= this.Get<YafBoardSettings>().PopularTopicReplys
                       || row["Views"].ToType<int>() >= this.Get<YafBoardSettings>().PopularTopicViews;
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
                this.TopicRow["LastForumAccess"].ToType<DateTime?>()
                ?? DateTimeHelper.SqlDbMinTime(),
                this.TopicRow["LastTopicAccess"].ToType<DateTime?>()
                ?? DateTimeHelper.SqlDbMinTime());

            if (!this.AllowSelection)
            {
                writer.Write("<div class=\"row\">");
                writer.Write("<div class=\"col-md-6\">");
                writer.Write("<h5>");
            }

            writer.Write($"<span class=\"fa-stack fa-1x\">{this.GetTopicImage(this.TopicRow, lastRead)}</span>");

            var priorityMessage = this.GetPriorityMessage(this.TopicRow);

            if (priorityMessage.IsSet())
            {
                writer.Write("{0}&nbsp;", priorityMessage);
            }

            var topicLink = new HyperLink
                                {
                                    NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                                        ForumPages.posts,
                                        "t={0}",
                                        this.TopicRow["LinkTopicID"]),
                                    Text = this.FormatTopicName(),
                                    CssClass = "topic-starter-popover"
                                };

            topicLink.Attributes.Add("data-toggle", "popover");

            var topicStartedDateTime = this.TopicRow["Posted"].ToType<DateTime>();

            var formattedStartedDatetime = this.Get<YafBoardSettings>().ShowRelativeTime
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
                                               .TopicRow[this.Get<YafBoardSettings>().EnableDisplayName
                                                             ? "StarterDisplay"
                                                             : "Starter"].ToString(),
                                           Style = this.TopicRow["StarterStyle"].ToString()
                                       };

            topicLink.Attributes.Add(
                "data-content",
                $@"{topicStarterLink.RenderToString()}
                          <span class=""fa-stack"">
                                                    <i class=""fa fa-calendar-day fa-stack-1x text-secondary""></i>
                                                    <i class=""fa fa-circle fa-badge-bg fa-inverse fa-outline-inverse""></i>
                                                    <i class=""fa fa-clock fa-badge text-secondary""></i>
                                                </span>&nbsp;<span class=""popover-timeago"">{formattedStartedDatetime}</span>
                         ");

            /*if (this.TopicRow["Description"].ToString().IsSet())
            {
                topicLink.ToolTip = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.TopicRow["Description"]));
            }*/

            if (!this.TopicRow["LastMessageID"].IsNullOrEmptyDBField())
            {
                if (this.TopicRow["LastPosted"].ToType<DateTime>() > lastRead)
                {
                    writer.Write("<span class=\"badge badge-success\">{0}</span>&nbsp;", this.GetText("NEW_POSTS"));
                }
            }

            writer.Write(topicLink.RenderToString());

            var favoriteCount = this.TopicRow["FavoriteCount"].ToType<int>();

            if (favoriteCount > 0)
            {
                writer.Write("&nbsp;<span class=\"badge badge-info\" title=\"{0}\"><i class=\"fas fa-star\"></i> +{1}</span>", this.GetText("FAVORITE_COUNT_TT"), favoriteCount);
            }

            // Render Pager
            var actualPostCount = this.TopicRow["Replies"].ToType<int>() + 1;

            if (this.Get<YafBoardSettings>().ShowDeletedMessages)
            {
                // add deleted posts not included in replies...");
                actualPostCount += this.TopicRow["NumPostsDeleted"].ToType<int>();
            }

            var pager = this.CreatePostPager(
                actualPostCount,
                this.Get<YafBoardSettings>().PostsPerPage,
                this.TopicRow["LinkTopicID"].ToType<int>());

            if (pager.IsSet())
            {
                writer.Write("&nbsp;<div class=\"btn-group btn-group-sm mb-1\" role=\"group\" aria-label\"topic pager\">{0}</div>", pager);
            }

            writer.Write(" </h5>");

            writer.Write("</div>");
            writer.Write("<div class=\"col-md-2 text-secondary\">");
            writer.Write("<div class=\"d-flex flex-row flex-md-column justify-content-between justify-content-md-start\">");
            writer.Write("<div>");
            writer.Write("{0}: ", this.GetText("MODERATE", "REPLIES"));
            writer.Write(this.FormatReplies());
            writer.Write(" </div>");
            writer.Write("<div>");
            writer.Write("{0}: ", this.GetText("MODERATE", "VIEWS"));
            writer.Write(this.FormatViews());
            writer.Write("   </div>");
            writer.Write("  </div>");
            writer.Write(" </div>");

            if (!this.TopicRow["LastMessageID"].IsNullOrEmptyDBField())
            {
                writer.Write("<div class=\"col-md-4 text-secondary\">");

                writer.Write($"{this.GetText("LASTPOST")}:&nbsp;");
                
                var infoLastPost = new ThemeButton
                                       {
                                           Size = ButtonSize.Small,
                                           Icon = "info-circle",
                                           Type = ButtonAction.OutlineInfo,
                                           DataToggle = "popover",
                                           CssClass = "topic-link-popover mr-1",
                                           NavigateUrl = "#!"
                                       };
                                       
                var lastPostedDateTime = this.TopicRow["LastPosted"].ToType<DateTime>();

                var formattedDatetime = this.Get<YafBoardSettings>().ShowRelativeTime
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
                                           .TopicRow[this.Get<YafBoardSettings>().EnableDisplayName
                                                         ? "LastUserDisplayName"
                                                         : "LastUserName"].ToString(),
                                       Style = this.TopicRow["LastUserStyle"].ToString()
                                   };

                infoLastPost.DataContent = $@"
                          {userLast.RenderToString()}
                          <span class=""fa-stack"">
                                                    <i class=""fa fa-calendar-day fa-stack-1x text-secondary""></i>
                                                    <i class=""fa fa-circle fa-badge-bg fa-inverse fa-outline-inverse""></i>
                                                    <i class=""fa fa-clock fa-badge text-secondary""></i>
                                                </span>&nbsp;<span class=""popover-timeago"">{formattedDatetime}</span>
                         ";

                var gotoLastPost = new ThemeButton
                                       {
                                           NavigateUrl =
                                               YafBuildLink.GetLink(
                                                   ForumPages.posts,
                                                   "m={0}#post{0}",
                                                   this.TopicRow["LastMessageID"]),
                                           Size = ButtonSize.Small,
                                           Icon = "share-square",
                                           Type = ButtonAction.OutlineSecondary,
                                           TitleLocalizedTag = "GO_LAST_POST",
                                           TextLocalizedTag = "GO_LAST_POST",
                                           DataToggle = "tooltip",
                                           CssClass = "mr-1"
                                       };

                var gotoLastUnread = new ThemeButton
                                         {
                                             NavigateUrl =
                                                 YafBuildLink.GetLink(
                                                     ForumPages.posts,
                                                     "t={0}&find=unread",
                                                     this.TopicRow["TopicID"]),
                                             Size = ButtonSize.Small,
                                             Icon = "book-reader",
                                             Type = ButtonAction.OutlineSecondary,
                                             TitleLocalizedTag = "GO_LASTUNREAD_POST",
                                             TextLocalizedTag = "GO_LASTUNREAD_POST",
                                             DataToggle = "tooltip"
                                         };

                writer.Write(infoLastPost.RenderToString());
                writer.Write(gotoLastPost.RenderToString());
                writer.Write(gotoLastUnread.RenderToString());
                
                writer.Write("</div>");
            }

            writer.Write("</div>");
        }

        /// <summary>
        /// Create pager for post.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="pageSize">
        /// The page Size.
        /// </param>
        /// <param name="topicID">
        /// The topic ID.
        /// </param>
        /// <returns>
        /// The create post pager.
        /// </returns>
        protected string CreatePostPager(int count, int pageSize, int topicID)
        {
            var strReturn = new StringBuilder();

            const int NumToDisplay = 4;
            var pageCount = (int)Math.Ceiling((double)count / pageSize);

            if (pageCount <= 1)
            {
                return strReturn.ToString();
            }

            if (pageCount > NumToDisplay)
            {
                strReturn.AppendLine(this.MakeLink("1", YafBuildLink.GetLink(ForumPages.posts, "t={0}", topicID), 1));
                strReturn.AppendLine(" ... ");

                // show links from the end
                for (var i = pageCount - (NumToDisplay - 1); i < pageCount; i++)
                {
                    var post = i + 1;

                    strReturn.AppendLine(
                        this.MakeLink(
                            post.ToString(),
                            YafBuildLink.GetLink(ForumPages.posts, "t={0}&p={1}", topicID, post),
                            post));
                }
            }
            else
            {
                for (var i = 0; i < pageCount; i++)
                {
                    var post = i + 1;

                    strReturn.AppendLine(
                        this.MakeLink(
                            post.ToString(),
                            YafBuildLink.GetLink(ForumPages.posts, "t={0}&p={1}", topicID, post),
                            post));
                }
            }

            return strReturn.ToString();
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

            if (this.Get<YafBoardSettings>().ShowDeletedMessages && numDeleted > 0)
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

            var styles = this.Get<YafBoardSettings>().UseStyledTopicTitles
                             ? this.Get<IStyleTransform>().DecodeStyleByString(this.TopicRow["Styles"].ToString())
                             : string.Empty;

            var topicSubjectStyled = string.Empty;

            if (styles.IsSet())
            {
                topicSubjectStyled = $"<span style=\"{this.HtmlEncode(styles)}\">{topicSubject}</span>";
            }

            return topicSubjectStyled.IsSet() ? topicSubjectStyled : topicSubject;
        }

        /// <summary>
        /// Creates the status message text for a topic. (i.e. Moved, Poll, Sticky, etc.)
        /// </summary>
        /// <param name="row">
        /// Current Topic Data Row
        /// </param>
        /// <returns>
        /// Topic status text
        /// </returns>
        protected string GetPriorityMessage([NotNull] DataRowView row)
        {
            CodeContracts.VerifyNotNull(row, "row");

            var strReturn = string.Empty;

            if (row["TopicMovedID"].ToString().Length > 0)
            {
                strReturn =
                    $"<span class=\"badge badge-secondary\"><i class=\"fa fa-arrows-alt fa-fw\"></i> {this.GetText("MOVED")}</span>";
            }
            else if (row["PollID"].ToString() != string.Empty)
            {
                strReturn =
                    $"<span class=\"badge badge-secondary\"><i class=\"fa fa-poll-h fa-fw\"></i> {this.GetText("POLL")}</span>";
            }
            else
            {
                switch (int.Parse(row["Priority"].ToString()))
                {
                    case 1:
                        strReturn =
                            $"<span class=\"badge badge-warning\"><i class=\"fa fa-sticky-note fa-fw\"></i> {this.GetText("STICKY")}</span>";
                        break;
                    case 2:
                        strReturn =
                            $"<span class=\"badge badge-primary\"><i class=\"fa fa-bullhorn fa-fw\"></i> {this.GetText("ANNOUNCEMENT")}</span>";
                        break;
                }
            }

            return strReturn;
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
        protected string GetTopicImage([NotNull] DataRowView row, DateTime lastRead)
        {
            CodeContracts.VerifyNotNull(row, "row");

            var lastPosted = row["LastPosted"] != DBNull.Value
                                 ? (DateTime)row["LastPosted"]
                                 : DateTimeHelper.SqlDbMinTime();

            var topicFlags = new TopicFlags(row["TopicFlags"]);
            var forumFlags = new ForumFlags(row["ForumFlags"]);

            var isHotTopic = this.IsPopularTopic(lastPosted, row);

            if (row["TopicMovedID"].ToString().Length > 0)
            {
                return
                    "<i class=\"fa fa-comment fa-stack-2x text-secondary\"></i><i class=\"fa fa-arrows-alt fa-stack-1x fa-inverse\"></i>";
            }

            if (lastPosted > lastRead)
            {
                this.Get<IYafSession>().UnreadTopics++;

                if (row["PollID"] != DBNull.Value)
                {
                    return
                        "<i class=\"fa fa-comment fa-stack-2x text-success\"></i><i class=\"fa fa-poll-h fa-stack-1x fa-inverse\"></i>";
                }

                switch (row["Priority"].ToString())
                {
                    case "1":
                        return
                            "<i class=\"fa fa-comment fa-stack-2x text-success\"></i><i class=\"fa fa-sticky-note fa-stack-1x fa-inverse\"></i>";
                    case "2":
                        return
                            "<i class=\"fa fa-comment fa-stack-2x text-success\"></i><i class=\"fa fa-bullhorn fa-stack-1x fa-inverse\"></i>";
                    default:
                        if (topicFlags.IsLocked || forumFlags.IsLocked)
                        {
                            return
                                "<i class=\"fa fa-comment fa-stack-2x text-success\"></i><i class=\"fa fa-lock fa-stack-1x fa-inverse\"></i>";
                        }

                        if (isHotTopic)
                        {
                            return
                                "<i class=\"fa fa-comment fa-stack-2x text-success\"></i><i class=\"fa fa-fire fa-stack-1x fa-inverse\"></i>";
                        }

                        return
                            "<i class=\"fa fa-comment fa-stack-2x text-success\"></i><i class=\"fa fa-comment fa-stack-1x fa-inverse\"></i>";
                }
            }

            if (row["PollID"] != DBNull.Value)
            {
                return "<i class=\"fa fa-comment fa-stack-2x text-secondary\"></i><i class=\"fa fa-poll fa-stack-1x fa-inverse\"></i>";
            }

            switch (row["Priority"].ToString())
            {
                case "1":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x text-secondary\"></i><i class=\"fa fa-sticky-note fa-stack-1x fa-inverse\"></i>";
                case "2":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x text-secondary\"></i><i class=\"fa fa-bullhorn fa-stack-1x fa-inverse\"></i>";
                default:
                    if (topicFlags.IsLocked || forumFlags.IsLocked)
                    {
                        return
                            "<i class=\"fa fa-comment fa-stack-2x text-secondary\"></i><i class=\"fa fa-lock fa-stack-1x fa-inverse\"></i>";
                    }

                    if (isHotTopic)
                    {
                        return
                            "<i class=\"fa fa-comment fa-stack-2x text-secondary\"></i><i class=\"fa fa-fire fa-stack-1x fa-inverse\"></i>";
                    }

                    return
                        "<i class=\"fa fa-comment fa-stack-2x text-secondary\"></i><i class=\"fa fa-comment fa-stack-1x fa-inverse\"></i>";
            }
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
        protected string MakeLink([NotNull] string text, [NotNull] string link, [NotNull] int pageId)
        {
            return
                $@"<a href=""{link}"" title=""{this.GetTextFormatted("GOTO_POST_PAGER", pageId)}"" class=""btn btn-secondary btn-sm"">{text}</a>";
        }
    }
}