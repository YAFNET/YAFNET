/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Controls
{
    using System;
    using System.Data;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    /// <summary>
    /// Alert Message Control
    /// </summary>
    [ToolboxData("<{0}:TopicContainer runat=server></{0}:TopicContainer>")]
    public class TopicContainer : BaseControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The last post tooltip string.
        /// </summary>
        private string _altLastPost;

        /// <summary>
        ///   The first unread post tooltip string.
        /// </summary>
        private string _altFirstUnreadPost;

        /// <summary>
        ///   The _the topic row.
        /// </summary>
        private DataRowView theTopicRow;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether AllowSelection.
        /// </summary>
        public bool AllowSelection
        {
            get
            {
                if (this.ViewState["AllowSelection"] == null)
                {
                    return false;
                }

                return (bool)this.ViewState["AllowSelection"];
            }

            set
            {
                this.ViewState["AllowSelection"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets Alt.
        /// </summary>
        [NotNull]
        public string AltLastPost
        {
            get
            {
                return string.IsNullOrEmpty(this._altLastPost) ? string.Empty : this._altLastPost;
            }

            set
            {
                this._altLastPost = value;
            }
        }

        /// <summary>
        ///   Gets or sets Alt Unread Post.
        /// </summary>
        [NotNull]
        public string AltLastUnreadPost
        {
            get
            {
                return string.IsNullOrEmpty(this._altFirstUnreadPost) ? string.Empty : this._altFirstUnreadPost;
            }

            set
            {
                this._altFirstUnreadPost = value;
            }
        }

        /// <summary>
        ///   Sets DataRow.
        /// </summary>
        public object DataRow
        {
            set
            {
                this.theTopicRow = value as DataRowView;
                this.TopicRowID = this.TopicRow["LinkTopicID"].ToType<int>();
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether FindUnread.
        /// </summary>
        public bool FindUnread
        {
            get
            {
                return this.ViewState["FindUnread"] != null && Convert.ToBoolean(this.ViewState["FindUnread"]);
            }

            set
            {
                this.ViewState["FindUnread"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether IsAlt.
        /// </summary>
        public bool IsAlt { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether ShowTopicPosted.
        /// </summary>
        public bool ShowTopicPosted
        {
            get
            {
                if (this.ViewState["ShowTopicPosted"] == null)
                {
                    return true;
                }

                return (bool)this.ViewState["ShowTopicPosted"];
            }

            set
            {
                this.ViewState["ShowTopicPosted"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets TopicRowID.
        /// </summary>
        public int? TopicRowID
        {
            get
            {
                if (this.ViewState["TopicRowID"] == null)
                {
                    return null;
                }

                return (int?)this.ViewState["TopicRowID"];
            }

            set
            {
                this.ViewState["TopicRowID"] = value;
            }
        }

        /// <summary>
        ///  Gets the TopicRow.
        /// </summary>
        protected DataRowView TopicRow
        {
            get
            {
                return this.theTopicRow;
            }
        }

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
                repStr = "{0:N0}".FormatWith(replies + numDeleted);
            }
            else
            {
                repStr = "{0:N0}".FormatWith(replies);
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
            return this.TopicRow["TopicMovedID"].ToString().Length > 0 ? "&nbsp;" : "{0:N0}".FormatWith(views);
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
                topicSubjectStyled = "<span style=\"{0}\">{1}</span>".FormatWith(this.HtmlEncode(styles), topicSubject);
            }

            return topicSubjectStyled.IsSet() ? topicSubjectStyled : topicSubject;
        }

        /// <summary>
        /// The get avatar url from id.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <returns>
        /// Returns the Avatar Url for the User
        /// </returns>
        protected string GetAvatarUrlFromID(int userID)
        {
            var avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(userID);

            if (avatarUrl.IsNotSet())
            {
                avatarUrl = "{0}images/noavatar.gif".FormatWith(YafForumInfo.ForumClientFileRoot);
            }

            return avatarUrl;
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
                strReturn = "<span class=\"badge badge-secondary\"><i class=\"fa fa-arrows-alt fa-fw\"></i> {0}</span>"
                    .FormatWith(this.GetText("MOVED"));
            }
            else if (row["PollID"].ToString() != string.Empty)
            {
                strReturn = "<span class=\"badge badge-secondary\"><i class=\"fa fa-poll-h fa-fw\"></i> {0}</span>"
                    .FormatWith(this.GetText("POLL"));
            }
            else
            {
                switch (int.Parse(row["Priority"].ToString()))
                {
                    case 1:
                        strReturn =
                            "<span class=\"badge badge-secondary\"><i class=\"fa fa-sticky-note fa-fw\"></i> {0}</span>"
                                .FormatWith(this.GetText("STICKY"));
                        break;
                    case 2:
                        strReturn =
                            "<span class=\"badge badge-secondary\"><i class=\"fa fa-bullhorn fa-fw\"></i> {0}</span>"
                                .FormatWith(this.GetText("ANNOUNCEMENT"));
                        break;
                }
            }

            return strReturn;
        }

        /// <summary>
        /// Gets the topic image.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>
        /// Returns the Topic Image
        /// </returns>
        protected string GetTopicImage([NotNull] DataRowView row)
        {
            CodeContracts.VerifyNotNull(row, "row");

            var lastPosted = row["LastPosted"] != DBNull.Value
                                 ? (DateTime)row["LastPosted"]
                                 : DateTimeHelper.SqlDbMinTime();

            var topicFlags = new TopicFlags(row["TopicFlags"]);
            var forumFlags = new ForumFlags(row["ForumFlags"]);

            var isHot = this.IsPopularTopic(lastPosted, row);

            if (row["TopicMovedID"].ToString().Length > 0)
            {
                return
                    "<i class=\"fa fa-comment fa-stack-2x\"></i><i class=\"fa fa-arrows-alt fa-stack-1x fa-inverse\"></i>";
            }

            var lastRead = this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                row["ForumID"].ToType<int>(),
                row["TopicID"].ToType<int>(),
                row["LastForumAccess"].IsNullOrEmptyDBField()
                    ? DateTimeHelper.SqlDbMinTime()
                    : row["LastForumAccess"].ToType<DateTime?>(),
                row["LastTopicAccess"].IsNullOrEmptyDBField()
                    ? DateTimeHelper.SqlDbMinTime()
                    : row["LastForumAccess"].ToType<DateTime?>());

            if (lastPosted > lastRead)
            {
                this.Get<IYafSession>().UnreadTopics++;

                if (row["PollID"] != DBNull.Value)
                {
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\" style=\"color:green\"></i><i class=\"fa fa-poll-h fa-stack-1x fa-inverse\"></i>";
                }

                switch (row["Priority"].ToString())
                {
                    case "1":
                        return
                            "<i class=\"fa fa-comment fa-stack-2x\" style=\"color:green\"></i><i class=\"fa fa-sticky-note fa-stack-1x fa-inverse\"></i>";
                    case "2":
                        return
                            "<i class=\"fa fa-comment fa-stack-2x\" style=\"color:green\"></i><i class=\"fa fa-bullhorn fa-stack-1x fa-inverse\"></i>";
                    default:
                        if (topicFlags.IsLocked || forumFlags.IsLocked)
                        {
                            return
                                "<i class=\"fa fa-comment fa-stack-2x\" style=\"color:green\"></i><i class=\"fa fa-lock fa-stack-1x fa-inverse\"></i>";
                        }

                        if (isHot)
                        {
                            return
                                "<i class=\"fa fa-comment fa-stack-2x\" style=\"color:green\"></i><i class=\"fa fa-fire fa-stack-1x fa-inverse\"></i>";
                        }

                        return
                            "<i class=\"fa fa-comment fa-stack-2x\" style=\"color:green\"></i><i class=\"fa fa-comment fa-stack-1x fa-inverse\"></i>";
                }
            }

            if (row["PollID"] != DBNull.Value)
            {
                return "<i class=\"fa fa-comment fa-stack-2x\"></i><i class=\"fa fa-poll fa-stack-1x fa-inverse\"></i>";
            }

            switch (row["Priority"].ToString())
            {
                case "1":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\"></i><i class=\"fa fa-sticky-note fa-stack-1x fa-inverse\"></i>";
                case "2":
                    return
                        "<i class=\"fa fa-comment fa-stack-2x\"></i><i class=\"fa fa-bullhorn fa-stack-1x fa-inverse\"></i>";
                default:
                    if (topicFlags.IsLocked || forumFlags.IsLocked)
                    {
                        return
                            "<i class=\"fa fa-comment fa-stack-2x\"></i><i class=\"fa fa-lock fa-stack-1x fa-inverse\"></i>";
                    }

                    if (isHot)
                    {
                        return
                            "<i class=\"fa fa-comment fa-stack-2x\"></i><i class=\"fa fa-fire fa-stack-1x fa-inverse\"></i>";
                    }

                    return
                        "<i class=\"fa fa-comment fa-stack-2x\"></i><i class=\"fa fa-comment fa-stack-1x fa-inverse\"></i>";
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
            return @"<a href=""{0}"" title=""{1}"" class=""btn btn-secondary btn-sm"">{2}</a>".FormatWith(
                link,
                this.GetText("GOTO_POST_PAGER").FormatWith(pageId),
                text);
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

            writer.Write("<div class=\"row\">");
            writer.Write("<div class=\"col-md-6\">");
            writer.Write("<h5>");

            writer.Write("<span class=\"fa-stack fa-1x\">{0}</span>".FormatWith(this.GetTopicImage(this.TopicRow)));

            var priorityMessage = this.GetPriorityMessage(this.TopicRow);

            if (priorityMessage.IsSet())
            {
                writer.Write(priorityMessage);

            }

            var topicLink = new HyperLink
                                {
                                    NavigateUrl =
                                        YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.TopicRow["LinkTopicID"]),
                                    ToolTip = this.Get<IFormatMessage>().GetCleanedTopicMessage(
                                        this.TopicRow["FirstMessage"],
                                        this.TopicRow["LinkTopicID"]).MessageTruncated,
                                    Text = this.FormatTopicName()
                                };

            var topicStarterLink = new UserLink
                                       {
                                           UserID = this.TopicRow["UserID"].ToType<int>(),
                                           ReplaceName = this
                                               .TopicRow[this.Get<YafBoardSettings>().EnableDisplayName
                                                             ? "StarterDisplay"
                                                             : "Starter"].ToString(),
                                           Style = this.TopicRow["StarterStyle"].ToString()
                                       };
            var startDate = new DisplayDateTime
                                {
                                    Format = DateTimeFormat.BothTopic,
                                    DateTime = this.TopicRow["Posted"],
                                    Visible = this.ShowTopicPosted
                                };

            var userLast = new UserLink
                               {
                                   UserID = this.TopicRow["LastUserID"].ToType<int>(),
                                   ReplaceName = this
                                       .TopicRow[this.Get<YafBoardSettings>().EnableDisplayName
                                                     ? "LastUserDisplayName"
                                                     : "LastUserName"].ToString(),
                                   Style = this.TopicRow["LastUserStyle"].ToString()
                               };

            var lastDate = new DisplayDateTime
                               {
                                   Format = DateTimeFormat.BothTopic,
                                   DateTime = this.TopicRow["LastPosted"],
                                   Visible = this.ShowTopicPosted
                               };

            if (!this.TopicRow["LastMessageID"].IsNullOrEmptyDBField())
            {
                var lastRead = this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                    forumId: this.TopicRow["ForumID"].ToType<int>(),
                    topicId: this.TopicRow["TopicID"].ToType<int>(),
                    forumReadOverride: this.TopicRow["LastForumAccess"].ToType<DateTime?>()
                                       ?? DateTimeHelper.SqlDbMinTime(),
                    topicReadOverride: this.TopicRow["LastTopicAccess"].ToType<DateTime?>()
                                       ?? DateTimeHelper.SqlDbMinTime());

                if (string.IsNullOrEmpty(this.AltLastPost))
                {
                    this.AltLastPost = this.GetText("DEFAULT", "GO_LAST_POST");
                }

                if (string.IsNullOrEmpty(this.AltLastUnreadPost))
                {
                    this.AltLastUnreadPost = this.GetText("DEFAULT", "GO_LASTUNREAD_POST");
                }

                if (this.TopicRow["LastPosted"].ToType<DateTime>() > lastRead)
                {
                    writer.Write("<span class=\"badge badge-success\">{0}</span>", this.GetText("NEW_POSTS"));
                }
            }

            writer.Write(topicLink.RenderToString());

            var favoriteCount = this.TopicRow["FavoriteCount"].ToType<int>();

            if (favoriteCount > 0)
            {
                writer.Write("<span title=\"{0}\">+{1}</span>", this.GetText("FAVORITE_COUNT_TT"), favoriteCount);
            }

            writer.Write(" </h5>");

            if (this.TopicRow["Description"].ToString().IsSet())
            {
                writer.Write(this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.TopicRow["Description"])));
            }

            writer.Write(" <p class=\"card-text\">");

            writer.Write(topicStarterLink.RenderToString());
            writer.Write("<i class=\"fa fa-calendar-alt fa-fw\"></i>&nbsp;");
            writer.Write(startDate.RenderToString());

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

            if (pager != string.Empty)
            {

                var altMultipages = this.GetText("GOTO_POST_PAGER").FormatWith(string.Empty);
                writer.Write("<span> - <i class=\"fa fa-file fa-fw\"></i>{0}</span>", pager);
            }

            writer.Write("</p>");
            writer.Write("</div>");
            writer.Write("<div class=\"col-md-2\">");
            writer.Write("<ul>");
            writer.Write("<li class=\"list-unstyled\">");
            writer.Write(this.GetText("MODERATE", "REPLIES"));
            writer.Write(this.FormatReplies());
            writer.Write(" </li>");
            writer.Write("<li class=\"list-unstyled\">");
            writer.Write(this.GetText("MODERATE", "VIEWS"));
            writer.Write(this.FormatViews());
            writer.Write("   </li>");
            writer.Write("  </ul>");
            writer.Write(" </div>");

            if (!this.TopicRow["LastMessageID"].IsNullOrEmptyDBField())
            {
                writer.Write(" <div class=\"col-md-4\">");
                writer.Write(" <h6>");

                var goToLastPost = new HyperLink
                                       {
                                           NavigateUrl = YafBuildLink.GetLink(
                                               ForumPages.posts,
                                               "m={0}#post{0}",
                                               this.TopicRow["LastMessageID"]),
                                           ToolTip = this.AltLastPost,
                                           Text = "<i class=\"fa fa-fast-forward fa-fw\"></i> {0}".FormatWith(
                                               this.AltLastPost)
                                       };

                var goToLastUnread = new HyperLink
                                         {
                                             NavigateUrl =
                                                 YafBuildLink.GetLink(
                                                     ForumPages.posts,
                                                     "t={0}&find=unread",
                                                     this.TopicRow["TopicID"]),
                                             ToolTip = this.AltLastUnreadPost,
                                             Text = "<i class=\"fa fa-step-forward fa-fw\"></i> {0}".FormatWith(
                                                 this.AltLastUnreadPost),
                                             Visible = this.Get<YafBoardSettings>().ShowLastUnreadPost
                                         };

                writer.Write(goToLastPost.RenderToString());
                writer.Write(goToLastUnread.RenderToString());
                writer.Write("</h6>");
                writer.Write(" <hr/>");
                writer.Write(" <h6>");
                writer.Write(userLast.RenderToString());

                writer.Write("&nbsp;<i class=\"fa fa-calendar-alt fa-fw\"></i>&nbsp;");
                writer.Write(lastDate.RenderToString());
                writer.Write("</h6>");
                writer.Write("</div>");
            }

            writer.Write("</div>");
        }
    }
}