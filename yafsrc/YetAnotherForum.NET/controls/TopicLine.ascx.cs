namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Text;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
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
        private string _altLastPost;

        /// <summary>
        ///   The first unread post tooltip string.
        /// </summary>
        private string _altFirstUnreadPost;

        /// <summary>
        ///   The _selected checkbox.
        /// </summary>
        // private CheckBox _selectedCheckbox;

        /// <summary>
        ///   The _the topic row.
        /// </summary>
        private DataRowView _theTopicRow;

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
                this._theTopicRow = (DataRowView)value;
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
                return (this.ViewState["FindUnread"] != null) && Convert.ToBoolean(this.ViewState["FindUnread"]);
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
        ///   Gets a value indicating whether IsSelected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.chkSelected.Checked;
            }
        }

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
                return this._theTopicRow;
            }
        }

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
                return (row["Replies"].ToType<int>() >= this.Get<YafBoardSettings>().PopularTopicReplys) ||
                       (row["Views"].ToType<int>() >= this.Get<YafBoardSettings>().PopularTopicViews);
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

            if (pageCount > 1)
            {
                if (pageCount > NumToDisplay)
                {
                    strReturn.AppendLine(
                        this.MakeLink("1", YafBuildLink.GetLink(ForumPages.posts, "t={0}", topicID), 1));
                    strReturn.AppendLine(" ... ");
                    bool bFirst = true;

                    // show links from the end
                    for (int i = pageCount - (NumToDisplay - 1); i < pageCount; i++)
                    {
                        int iPost = i + 1;

                        if (bFirst)
                        {
                            bFirst = false;
                        }
                        else
                        {
                            strReturn.AppendLine(", ");
                        }

                        strReturn.AppendLine(
                            this.MakeLink(
                                iPost.ToString(),
                                YafBuildLink.GetLink(ForumPages.posts, "t={0}&p={1}", topicID, iPost),
                                iPost));
                    }
                }
                else
                {
                    bool bFirst = true;
                    for (int i = 0; i < pageCount; i++)
                    {
                        int iPost = i + 1;

                        if (bFirst)
                        {
                            bFirst = false;
                        }
                        else
                        {
                            strReturn.AppendLine(", ");
                        }

                        strReturn.AppendLine(
                            this.MakeLink(
                                iPost.ToString(),
                                YafBuildLink.GetLink(ForumPages.posts, "t={0}&p={1}", topicID, iPost),
                                iPost));
                    }
                }
            }

            return strReturn.ToString();
        }

        /// <summary>
        /// The format replies.
        /// </summary>
        /// <returns>
        /// Returns the formated replies.
        /// </returns>
        protected string FormatReplies()
        {
            string repStr = "&nbsp;";

            int nReplies = this.TopicRow["Replies"].ToType<int>();
            int numDeleted = this.TopicRow["NumPostsDeleted"].ToType<int>();

            if (nReplies >= 0)
            {
                if (this.Get<YafBoardSettings>().ShowDeletedMessages && numDeleted > 0)
                {
                    repStr = "{0:N0}".FormatWith(nReplies + numDeleted);
                }
                else
                {
                    repStr = "{0:N0}".FormatWith(nReplies);
                }
            }

            return repStr;
        }

        /// <summary>
        /// The format views.
        /// </summary>
        /// <returns>
        /// Returns the formated views.
        /// </returns>
        protected string FormatViews()
        {
            var nViews = this.TopicRow["Views"].ToType<int>();
            return (this.TopicRow["TopicMovedID"].ToString().Length > 0) ? "&nbsp;" : "{0:N0}".FormatWith(nViews);
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
                             ? this.Get<IStyleTransform>().DecodeStyleByString(
                                 this.TopicRow["Styles"].ToString(), false)
                             : string.Empty;

            var topicSubjectStyled = string.Empty;

            if (styles.IsSet())
            {
                topicSubjectStyled = "<span style=\"{0}\">{1}</span>".FormatWith(this.HtmlEncode(styles), topicSubject);
            }

            if (this.TopicRow["Status"].ToString().IsSet() && this.Get<YafBoardSettings>().EnableTopicStatus)
            {
                var topicStatusIcon = this.Get<ITheme>().GetItem("TOPIC_STATUS", this.TopicRow["Status"].ToString());

                if (topicStatusIcon.IsSet() && !topicStatusIcon.Contains("[TOPIC_STATUS."))
                {
                    return
                        "<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" class=\"topicStatusIcon\" />&nbsp;{2}"
                            .FormatWith(
                                this.Get<ITheme>().GetItem("TOPIC_STATUS", this.TopicRow["Status"].ToString()),
                                this.GetText("TOPIC_STATUS", this.TopicRow["Status"].ToString()),
                                topicSubjectStyled.IsSet() ? topicSubjectStyled : topicSubject);
                }

                return "[{0}]&nbsp;{1}".FormatWith(
                    this.GetText("TOPIC_STATUS", this.TopicRow["Status"].ToString()),
                    topicSubjectStyled.IsSet() ? topicSubjectStyled : topicSubject);
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
            string avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(userID);

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

            string strReturn = string.Empty;

            if (row["TopicMovedID"].ToString().Length > 0)
            {
                strReturn = this.GetText("MOVED");
            }
            else if (row["PollID"].ToString() != string.Empty)
            {
                strReturn = this.GetText("POLL");
            }
            else
            {
                switch (int.Parse(row["Priority"].ToString()))
                {
                    case 1:
                        strReturn = this.GetText("STICKY");
                        break;
                    case 2:
                        strReturn = this.GetText("ANNOUNCEMENT");
                        break;
                }
            }

            if (strReturn.Length > 0)
            {
                strReturn = "[ {0} ] ".FormatWith(strReturn);
            }

            return strReturn;
        }

        /// <summary>
        /// The get topic image.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="imgTitle">
        /// The img title.
        /// </param>
        /// <returns>
        /// Returns the Topic Image
        /// </returns>
        protected string GetTopicImage([NotNull] object o, [NotNull] ref string imgTitle)
        {
            CodeContracts.VerifyNotNull(o, "o");
            CodeContracts.VerifyNotNull(imgTitle, "imgTitle");

            var row = (DataRowView)o;
            DateTime lastPosted = row["LastPosted"] != DBNull.Value
                                      ? (DateTime)row["LastPosted"]
                                      : DateTimeHelper.SqlDbMinTime();

            var topicFlags = new TopicFlags(row["TopicFlags"]);
            var forumFlags = new ForumFlags(row["ForumFlags"]);

            var isHot = this.IsPopularTopic(lastPosted, row);

            if (row["TopicMovedID"].ToString().Length > 0)
            {
                imgTitle = this.GetText("MOVED");
                return this.Get<ITheme>().GetItem("ICONS", "TOPIC_MOVED");
            }

            DateTime lastRead = this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                row["ForumID"].ToType<int>(),
                row["TopicID"].ToType<int>(),
                row["LastForumAccess"].IsNullOrEmptyDBField() ? DateTimeHelper.SqlDbMinTime() : row["LastForumAccess"].ToType<DateTime?>(),
                row["LastTopicAccess"].IsNullOrEmptyDBField() ? DateTimeHelper.SqlDbMinTime() : row["LastForumAccess"].ToType<DateTime?>());
                
            if (lastPosted > lastRead)
            {
                this.Get<IYafSession>().UnreadTopics++;

                if (row["PollID"] != DBNull.Value)
                {
                    imgTitle = this.GetText("POLL_NEW");
                    return this.Get<ITheme>().GetItem("ICONS", "TOPIC_POLL_NEW");
                }

                switch (row["Priority"].ToString())
                {
                    case "1":
                        imgTitle = this.GetText("STICKY_NEW");
                        return this.Get<ITheme>().GetItem("ICONS", "TOPIC_STICKY_NEW");
                    case "2":
                        imgTitle = this.GetText("ANNOUNCEMENT");
                        return this.Get<ITheme>().GetItem("ICONS", "TOPIC_ANNOUNCEMENT_NEW");
                    default:
                        if (topicFlags.IsLocked || forumFlags.IsLocked)
                        {
                            imgTitle = this.GetText("NEW_POSTS_LOCKED");
                            return this.Get<ITheme>().GetItem("ICONS", "TOPIC_NEW_LOCKED");
                        }

                        if (isHot)
                        {
                            imgTitle = this.GetText("ICONLEGEND", "HOT_NEW_POSTS");
                            return this.Get<ITheme>().GetItem(
                                "ICONS", "TOPIC_HOT_NEW", this.Get<ITheme>().GetItem("ICONS", "TOPIC_NEW"));
                        }

                        imgTitle = this.GetText("ICONLEGEND", "NEW_POSTS");
                        return this.Get<ITheme>().GetItem("ICONS", "TOPIC_NEW");
                }
            }

            if (row["PollID"] != DBNull.Value)
            {
                imgTitle = this.GetText("POLL");
                return this.Get<ITheme>().GetItem("ICONS", "TOPIC_POLL");
            }

            switch (row["Priority"].ToString())
            {
                case "1":
                    imgTitle = this.GetText("STICKY");
                    return this.Get<ITheme>().GetItem("ICONS", "TOPIC_STICKY");
                case "2":
                    imgTitle = this.GetText("ANNOUNCEMENT");
                    return this.Get<ITheme>().GetItem("ICONS", "TOPIC_ANNOUNCEMENT");
                default:
                    if (topicFlags.IsLocked || forumFlags.IsLocked)
                    {
                        imgTitle = this.GetText("NO_NEW_POSTS_LOCKED");
                        return this.Get<ITheme>().GetItem("ICONS", "TOPIC_LOCKED");
                    }

                    if (isHot)
                    {
                        imgTitle = this.GetText("HOT_NO_NEW_POSTS");
                        return this.Get<ITheme>().GetItem(
                            "ICONS", "TOPIC_HOT", this.Get<ITheme>().GetItem("ICONS", "TOPIC"));
                    }

                    imgTitle = this.GetText("NO_NEW_POSTS");
                    return this.Get<ITheme>().GetItem("ICONS", "TOPIC");
            }
        }

        /// <summary>
        /// The make link.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="link">
        /// The link.
        /// </param>
        /// <param name="pageid">
        /// The pageid.
        /// </param>
        /// <returns>
        /// Returns the created link.
        /// </returns>
        protected string MakeLink([NotNull] string text, [NotNull] string link, [NotNull] int pageid)
        {
            return "<a href=\"{0}\" title=\"{1}\">{2}</a>".FormatWith(
                link, this.GetText("GOTO_POST_PAGER").FormatWith(pageid), text);
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
            this.UpdateUI();
        }

        /// <summary>
        /// The update ui.
        /// </summary>
        private void UpdateUI()
        {
            this.SelectionHolder.Visible = this.AllowSelection;
            this.chkSelected.Checked = this.IsSelected;
        }

        #endregion
    }
}