namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

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

    private DataRowView _theTopicRow;

    private CheckBox _selectedCheckbox;

    /// <summary>
    ///   The TopicRow.
    /// </summary>
    protected DataRowView TopicRow
    {
      get
      {
        return this._theTopicRow;
      }
    }

    public int? TopicRowID
    {
      get
      {
        if (ViewState["TopicRowID"] == null)
        {
          return null;
        }

        return (int?)ViewState["TopicRowID"];
      }

      set
      {
        ViewState["TopicRowID"] = value;
      }
    }

    #endregion

    #region Properties

    public bool AllowSelection
    {
      get
      {
        if (ViewState["AllowSelection"] == null)
        {
          return false;
        }

        return (bool)ViewState["AllowSelection"];
      }

      set
      {
        ViewState["AllowSelection"] = value;
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
        if (string.IsNullOrEmpty(this._altLastPost))
        {
          return string.Empty;
        }

        return this._altLastPost;
      }

      set
      {
        this._altLastPost = value;
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
        this.TopicRowID = Convert.ToInt32(this.TopicRow["LinkTopicID"]);
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether FindUnread.
    /// </summary>
    public bool FindUnread
    {
      get
      {
        return (this.ViewState["FindUnread"] != null) ? Convert.ToBoolean(this.ViewState["FindUnread"]) : false;
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

    #endregion

    #region Methods

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
      string strReturn = string.Empty;

      int NumToDisplay = 4;
      var pageCount = (int)Math.Ceiling((double)count / pageSize);

      if (pageCount > 1)
      {
        if (pageCount > NumToDisplay)
        {
          strReturn += this.MakeLink("1", YafBuildLink.GetLink(ForumPages.posts, "t={0}", topicID));
          strReturn += " ... ";
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
              strReturn += ", ";
            }

            strReturn += this.MakeLink(
              iPost.ToString(), YafBuildLink.GetLink(ForumPages.posts, "t={0}&p={1}", topicID, iPost));
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
              strReturn += ", ";
            }

            strReturn += this.MakeLink(
              iPost.ToString(), YafBuildLink.GetLink(ForumPages.posts, "t={0}&p={1}", topicID, iPost));
          }
        }
      }

      return strReturn;
    }

    /// <summary>
    /// The format replies.
    /// </summary>
    /// <returns>
    /// The format replies.
    /// </returns>
    protected string FormatReplies()
    {
      string repStr = "&nbsp;";

      int nReplies = Convert.ToInt32(this.TopicRow["Replies"]);
      int numDeleted = Convert.ToInt32(this.TopicRow["NumPostsDeleted"]);

      if (nReplies >= 0)
      {
        if (this.PageContext.BoardSettings.ShowDeletedMessages && numDeleted > 0)
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
      CodeContracts.ArgumentNotNull(row, "row");

      string strReturn = string.Empty;

      if (row["TopicMovedID"].ToString().Length > 0)
      {
        strReturn = this.PageContext.Localization.GetText("MOVED");
      }
      else if (row["PollID"].ToString() != string.Empty)
      {
        strReturn = this.PageContext.Localization.GetText("POLL");
      }
      else
      {
        switch (int.Parse(row["Priority"].ToString()))
        {
          case 1:
            strReturn = this.PageContext.Localization.GetText("STICKY");
            break;
          case 2:
            strReturn = this.PageContext.Localization.GetText("ANNOUNCEMENT");
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
    /// The get topic image.
    /// </returns>
    protected string GetTopicImage([NotNull] object o, [NotNull] ref string imgTitle)
    {
      CodeContracts.ArgumentNotNull(o, "o");
      CodeContracts.ArgumentNotNull(imgTitle, "imgTitle");

      var row = (DataRowView)o;
      DateTime lastPosted = row["LastPosted"] != DBNull.Value ? (DateTime)row["LastPosted"] : new DateTime(2000, 1, 1);
      var topicFlags = new TopicFlags(row["TopicFlags"]);
      var forumFlags = new ForumFlags(row["ForumFlags"]);

      // Obsolette : Ederon
      // bool isLocked = General.BinaryAnd(row["TopicFlags"], TopicFlags.Locked);
      imgTitle = "???";

      try
      {
        // Obsolette : Ederon
        // bool bIsLocked = isLocked || General.BinaryAnd( row ["ForumFlags"], ForumFlags.Locked );
        if (row["TopicMovedID"].ToString().Length > 0)
        {
          imgTitle = this.PageContext.Localization.GetText("MOVED");
          return this.PageContext.Theme.GetItem("ICONS", "TOPIC_MOVED");
        }

        DateTime lastRead = YafContext.Current.Get<IYafSession>().GetTopicRead((int)row["TopicID"]);
        DateTime lastReadForum = YafContext.Current.Get<IYafSession>().GetForumRead((int)row["ForumID"]);
        if (lastReadForum > lastRead)
        {
          lastRead = lastReadForum;
        }

        if (lastPosted > lastRead)
        {
          YafContext.Current.Get<IYafSession>().UnreadTopics++;

          if (row["PollID"] != DBNull.Value)
          {
            imgTitle = this.PageContext.Localization.GetText("POLL_NEW");
            return this.PageContext.Theme.GetItem("ICONS", "TOPIC_POLL_NEW");
          }
          else if (row["Priority"].ToString() == "1")
          {
            imgTitle = this.PageContext.Localization.GetText("STICKY");
            return this.PageContext.Theme.GetItem("ICONS", "TOPIC_STICKY");
          }
          else if (row["Priority"].ToString() == "2")
          {
            imgTitle = this.PageContext.Localization.GetText("ANNOUNCEMENT");
            return this.PageContext.Theme.GetItem("ICONS", "TOPIC_ANNOUNCEMENT_NEW");
          }
          else if (topicFlags.IsLocked || forumFlags.IsLocked)
          {
            imgTitle = this.PageContext.Localization.GetText("NEW_POSTS_LOCKED");
            return this.PageContext.Theme.GetItem("ICONS", "TOPIC_NEW_LOCKED");
          }
          else
          {
            imgTitle = this.PageContext.Localization.GetText("NEW_POSTS");
            return this.PageContext.Theme.GetItem("ICONS", "TOPIC_NEW");
          }
        }
        else
        {
          if (row["PollID"] != DBNull.Value)
          {
            imgTitle = this.PageContext.Localization.GetText("POLL");
            return this.PageContext.Theme.GetItem("ICONS", "TOPIC_POLL");
          }
          else if (row["Priority"].ToString() == "1")
          {
            imgTitle = this.PageContext.Localization.GetText("STICKY");
            return this.PageContext.Theme.GetItem("ICONS", "TOPIC_STICKY");
          }
          else if (row["Priority"].ToString() == "2")
          {
            imgTitle = this.PageContext.Localization.GetText("ANNOUNCEMENT");
            return this.PageContext.Theme.GetItem("ICONS", "TOPIC_ANNOUNCEMENT");
          }
          else if (topicFlags.IsLocked || forumFlags.IsLocked)
          {
            imgTitle = this.PageContext.Localization.GetText("NO_NEW_POSTS_LOCKED");
            return this.PageContext.Theme.GetItem("ICONS", "TOPIC_LOCKED");
          }
          else
          {
            imgTitle = this.PageContext.Localization.GetText("NO_NEW_POSTS");
            return this.PageContext.Theme.GetItem("ICONS", "TOPIC");
          }
        }
      }
      catch (Exception)
      {
        return this.PageContext.Theme.GetItem("ICONS", "TOPIC");
      }
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

    public bool IsSelected
    {
      get
      {
        return this.chkSelected.Checked;
      }
    }

    private void UpdateUI()
    {
      this.SelectionHolder.Visible = this.AllowSelection;
      this.chkSelected.Checked = this.IsSelected;
    }

    /// <summary>
    /// The format views.
    /// </summary>
    /// <returns>
    /// The format views.
    /// </returns>
    protected string FormatViews()
    {
      var nViews = this.TopicRow["Views"].ToType<int>();
      return (this.TopicRow["TopicMovedID"].ToString().Length > 0) ? "&nbsp;" : "{0:N0}".FormatWith(nViews);
    }

    /// <summary>
    /// The get avatar url from id.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// The get avatar url from id.
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
    /// The make link.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="link">
    /// The link.
    /// </param>
    /// <returns>
    /// The make link.
    /// </returns>
    protected string MakeLink([NotNull] string text, [NotNull] string link)
    {
      return "<a href=\"{0}\">{1}</a>".FormatWith(link, text);
    }

    #endregion
  }
}