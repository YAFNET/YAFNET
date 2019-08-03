namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Text;
    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Exceptions;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The topic line.
    /// </summary>
    public partial class TopicLine : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The last post tooltip string.
        /// </summary>
        private string altLastPost;

        /// <summary>
        ///   The first unread post tooltip string.
        /// </summary>
        private string altFirstUnreadPost;

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

            set => this.ViewState["AllowSelection"] = value;
        }

        /// <summary>
        ///   Gets or sets Alt.
        /// </summary>
        [NotNull]
        public string AltLastPost
        {
            get => this.altLastPost.IsNotSet() ? string.Empty : this.altLastPost;

            set => this.altLastPost = value;
        }

        /// <summary>
        ///   Gets or sets Alt Unread Post.
        /// </summary>
        [NotNull]
        public string AltLastUnreadPost
        {
            get => this.altFirstUnreadPost.IsNotSet() ? string.Empty : this.altFirstUnreadPost;

            set => this.altFirstUnreadPost = value;
        }

        /// <summary>
        ///   Sets DataRow.
        /// </summary>
        public object DataRow
        {
            set
            {
                this.TopicRow = value as DataRowView;
                this.TopicRowID = this.TopicRow["LinkTopicID"].ToType<int>();
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether IsAlt.
        /// </summary>
        public bool IsAlt { get; set; }

        /// <summary>
        ///   Gets a value indicating whether IsSelected.
        /// </summary>
        public bool IsSelected => this.chkSelected.Checked;

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

            set => this.ViewState["ShowTopicPosted"] = value;
        }

        /// <summary>
        ///   Gets or sets TopicRowID.
        /// </summary>
        public int? TopicRowID
        {
            get => (int?)this.ViewState["TopicRowID"];

            set => this.ViewState["TopicRowID"] = value;
        }

        /// <summary>
        ///  Gets the TopicRow.
        /// </summary>
        protected DataRowView TopicRow { get; private set; }

        #endregion

        #region Methods

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
                strReturn.AppendLine(
                    this.MakeLink("1", YafBuildLink.GetLink(ForumPages.posts, "t={0}", topicID), 1));
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
                             ? this.Get<IStyleTransform>()
                                   .DecodeStyleByString(this.TopicRow["Styles"].ToString())
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
                    $"<span class=\"badge badge-secondary\"><i class=\"fas fa-arrows-alt fa-fw\"></i> {this.GetText("MOVED")}</span>";
            }
            else if (row["PollID"].ToString() != string.Empty)
            {
                strReturn =
                    $"<span class=\"badge badge-secondary\"><i class=\"fas fa-poll-h fa-fw\"></i> {this.GetText("POLL")}</span>";
            }
            else
            {
                switch (int.Parse(row["Priority"].ToString()))
                {
                    case 1:
                        strReturn =
                            $"<span class=\"badge badge-secondary\"><i class=\"fas fa-sticky-note fa-fw\"></i> {this.GetText("STICKY")}</span>";
                        break;
                    case 2:
                        strReturn =
                            $"<span class=\"badge badge-secondary\"><i class=\"fas fa-bullhorn fa-fw\"></i> {this.GetText("ANNOUNCEMENT")}</span>";
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
                    "<i class=\"fas fa-comment fa-stack-2x\"></i><i class=\"fas fa-arrows-alt fa-stack-1x fa-inverse\"></i>";
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
                        "<i class=\"fas fa-comment fa-stack-2x text-success\"></i><i class=\"fas fa-poll-h fa-stack-1x fa-inverse\"></i>";
                }

                switch (row["Priority"].ToString())
                {
                    case "1":
                        return
                            "<i class=\"fas fa-comment fa-stack-2x text-success\"></i><i class=\"fas fa-sticky-note fa-stack-1x fa-inverse\"></i>";
                    case "2":
                        return
                            "<i class=\"fas fa-comment fa-stack-2x text-success\"></i><i class=\"fas fa-bullhorn fa-stack-1x fa-inverse\"></i>";
                    default:
                        if (topicFlags.IsLocked || forumFlags.IsLocked)
                        {
                            return
                                "<i class=\"fas fa-comment fa-stack-2x text-success\"></i><i class=\"fas fa-lock fa-stack-1x fa-inverse\"></i>";
                        }

                        if (isHot)
                        {
                            return
                                "<i class=\"fas fa-comment fa-stack-2x text-success\"></i><i class=\"fas fa-fire fa-stack-1x fa-inverse\"></i>";
                        }

                        return
                            "<i class=\"fas fa-comment fa-stack-2x text-success\"></i><i class=\"fas fa-comment fa-stack-1x fa-inverse\"></i>";
                }
            }

            if (row["PollID"] != DBNull.Value)
            {
                return "<i class=\"fas fa-comment fa-stack-2x text-secondary\"></i><i class=\"fas fa-poll fa-stack-1x fa-inverse\"></i>";
            }

            switch (row["Priority"].ToString())
            {
                case "1":
                    return
                        "<i class=\"fas fa-comment fa-stack-2x text-secondary\"></i><i class=\"fas fa-sticky-note fa-stack-1x fa-inverse\"></i>";
                case "2":
                    return
                        "<i class=\"fas fa-comment fa-stack-2x text-secondary\"></i><i class=\"fas fa-bullhorn fa-stack-1x fa-inverse\"></i>";
                default:
                    if (topicFlags.IsLocked || forumFlags.IsLocked)
                    {
                        return
                            "<i class=\"fas fa-comment fa-stack-2x text-secondary\"></i><i class=\"fas fa-lock fa-stack-1x fa-inverse\"></i>";
                    }

                    if (isHot)
                    {
                        return
                            "<i class=\"fas fa-comment fa-stack-2x text-secondary\"></i><i class=\"fas fa-fire fa-stack-1x fa-inverse\"></i>";
                    }

                    return
                        "<i class=\"fas fa-comment fa-stack-2x text-secondary\"></i><i class=\"fas fa-comment fa-stack-1x fa-inverse\"></i>";
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
                $@"<a href=""{link}"" title=""{string.Format(this.GetText("GOTO_POST_PAGER"), pageId)}"" class=""btn btn-secondary btn-sm py-0 px-1"">{text}</a>";
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.UpdateUI();
        }

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (this.TopicRow == null)
            {
                throw new NoTopicRowException(this.TopicRowID);
            }
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        private void UpdateUI()
        {
            this.SelectionHolder.Visible = this.AllowSelection;
            this.chkSelected.Checked = this.IsSelected;

            try
            {
                var priorityMessage = this.GetPriorityMessage(this.TopicRow);

                if (priorityMessage.IsSet())
                {
                    this.Priority.Visible = true;
                    this.Priority.Text = priorityMessage;
                }
            }
            catch (Exception)
            {
                this.Priority.Visible = false;
            }

            try
            {
                var favoriteCount = this.TopicRow["FavoriteCount"].ToType<int>();

                if (favoriteCount > 0)
                {
                    this.FavoriteCount.Visible = true;
                    this.FavoriteCount.Text =
                        $" <span class=\"badge badge-info\" title=\"{this.GetText("FAVORITE_COUNT_TT")}\"><i class=\"fas fa-star\"></i> +{favoriteCount}</span>";
                }
            }
            catch (Exception)
            {
                this.FavoriteCount.Visible = false;
            }
            
            this.TopicLink.NavigateUrl = YafBuildLink.GetLink(
                ForumPages.posts,
                "t={0}",
                this.TopicRow["LinkTopicID"]);
            this.TopicLink.ToolTip = this.Get<IFormatMessage>()
                .GetCleanedTopicMessage(this.TopicRow["FirstMessage"], this.TopicRow["LinkTopicID"]).MessageTruncated;
            this.TopicLink.Text = this.FormatTopicName();

            this.Description.Visible = this.TopicRow["Description"].ToString().IsSet();
            this.Description.Text = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.TopicRow["Description"]));

            this.topicStarterLink.UserID = this.TopicRow["UserID"].ToType<int>();
            this.topicStarterLink.ReplaceName = this
                .TopicRow[this.Get<YafBoardSettings>().EnableDisplayName ? "StarterDisplay" : "Starter"].ToString();
            this.topicStarterLink.Style = this.TopicRow["StarterStyle"].ToString();

            this.StartDate.Format = DateTimeFormat.BothTopic;
            this.StartDate.DateTime = this.TopicRow["Posted"];
            this.StartDate.Visible = this.ShowTopicPosted;

            this.UserLast.UserID = this.TopicRow["LastUserID"].ToType<int>();
            this.UserLast.ReplaceName = this.TopicRow[this.Get<YafBoardSettings>().EnableDisplayName ? "LastUserDisplayName" : "LastUserName"].ToString();
            this.UserLast.Style = this.TopicRow["LastUserStyle"].ToString();

            this.LastDate.Format = DateTimeFormat.BothTopic;
            this.LastDate.DateTime = this.TopicRow["LastPosted"];
            this.LastDate.Visible = this.ShowTopicPosted;

            if (!this.TopicRow["LastMessageID"].IsNullOrEmptyDBField())
            {
                var lastRead =
                    this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                        forumId: this.TopicRow["ForumID"].ToType<int>(),
                        topicId: this.TopicRow["TopicID"].ToType<int>(),
                        forumReadOverride: this.TopicRow["LastForumAccess"].ToType<DateTime?>() ?? DateTimeHelper.SqlDbMinTime(),
                        topicReadOverride: this.TopicRow["LastTopicAccess"].ToType<DateTime?>() ?? DateTimeHelper.SqlDbMinTime());

                if (this.AltLastPost.IsNotSet())
                {
                    this.AltLastPost = this.GetText("DEFAULT", "GO_LAST_POST");
                }

                if (this.AltLastUnreadPost.IsNotSet())
                {
                    this.AltLastUnreadPost = this.GetText("DEFAULT", "GO_LASTUNREAD_POST");
                }

                this.GoToLastPost.NavigateUrl = YafBuildLink.GetLink(ForumPages.posts, "m={0}#post{0}", this.TopicRow["LastMessageID"]);

                this.GoToLastUnread.NavigateUrl = YafBuildLink.GetLink(
                    ForumPages.posts,
                    "t={0}&find=unread",
                    this.TopicRow["TopicID"]);
                this.GoToLastUnread.Visible = this.Get<YafBoardSettings>().ShowLastUnreadPost;

                if (this.TopicRow["LastPosted"].ToType<DateTime>() > lastRead)
                {
                    this.NewMessage.Visible = true;
                    this.NewMessage.Text =
                        " <span class=\"fa-stack\"><i class=\"far fa-square fa-stack-2x\"></i><i class=\"fab fa-twitter fa-stack-1x\"></i></span>";
                }
                else
                {
                    this.NewMessage.Visible = false;
                }
            }

            this.TopicIcon.Text = $"<span class=\"fa-stack\">{this.GetTopicImage(this.TopicRow)}</span>";
        }

        #endregion
    }
}