/* Yet Another Forum.NET
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;
  using System.Text;
  using System.Web.UI;
  using System.Data.DataSetExtensions;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.UI;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The topic line.
  /// </summary>
  [ParseChildren(false)]
  public class TopicLine : BaseControl
  {
    #region Constants and Fields

    /// <summary>
    /// The _is alt.
    /// </summary>
    private bool _isAlt;

    /// <summary>
    /// The last post tooltip string. 
    /// </summary>
    private string _altLastPost; 

    /// <summary>
    /// The _row.
    /// </summary>
    private DataRowView _row = null;

    #endregion

    #region Properties

    /// <summary>
    /// Sets DataRow.
    /// </summary>
    public object DataRow
    {
      set
      {
        this._row = (DataRowView)value;
      }
    }

    /// <summary>
    /// Gets or sets Alt.
    /// </summary>
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
    /// Gets or sets a value indicating whether FindUnread.
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
    /// Gets or sets a value indicating whether IsAlt.
    /// </summary>
    public bool IsAlt
    {
      get
      {
        return this._isAlt;
      }

      set
      {
        this._isAlt = value;
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

      int nReplies = Convert.ToInt32(this._row["Replies"]);
      int numDeleted = Convert.ToInt32(this._row["NumPostsDeleted"]);

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
    protected string GetPriorityMessage(DataRowView row)
    {
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
    protected string GetTopicImage(object o, ref string imgTitle)
    {
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

        DateTime lastRead = Mession.GetTopicRead((int)row["TopicID"]);
        DateTime lastReadForum = Mession.GetForumRead((int)row["ForumID"]);
        if (lastReadForum > lastRead)
        {
          lastRead = lastReadForum;
        }

        if (lastPosted > lastRead)
        {
          Mession.UnreadTopics++;

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
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      var html = new StringBuilder(2000);

      writer.WriteBeginTag("tr");
      writer.WriteAttribute("class", this.IsAlt ? "topicRow_Alt post_alt" : "topicRow post");
      writer.Write(HtmlTextWriter.TagRightChar);

      writer.WriteBeginTD("topicImage");

      // Icon
      string imgTitle = string.Empty;
      string imgSrc = this.GetTopicImage(this._row, ref imgTitle);
      writer.RenderImgTag(imgSrc, imgTitle, imgTitle);

      writer.WriteEndTD();

      // Topic
      writer.WriteBeginTD("topicMain");

      if (PageContext.BoardSettings.ShowAvatarsInTopic)
      {
        var avatarUrl = GetAvatarUrlFromID(Convert.ToInt32(_row["UserID"]));
        writer.RenderImgTag(avatarUrl, this.AltLastPost, this.AltLastPost, "avatarimage");
      }

      int actualPostCount = RenderTopic(writer);

      RenderTopicStarter(writer);

      RenderPosted(writer);

      RenderPostPager(writer, actualPostCount);

      writer.WriteEndTD();

      // Replies
      writer.WriteBeginTag("td");
      writer.WriteAttribute("class", "topicReplies");
      writer.Write(HtmlTextWriter.TagRightChar);
      writer.Write(this.FormatReplies());
      writer.WriteEndTag("td");

      // Views
      writer.WriteBeginTag("td");
      writer.WriteAttribute("class", "topicViews");
      writer.Write(HtmlTextWriter.TagRightChar);
      writer.Write(this.FormatViews());
      writer.WriteEndTag("td");

      // Last Post
      writer.WriteBeginTag("td");
      writer.WriteAttribute("class", "topicLastPost smallfont");
      writer.Write(HtmlTextWriter.TagRightChar);
      this.RenderLastPost(writer);
      writer.WriteEndTag("td");

      this.RenderChildren(writer);

      writer.WriteEndTag("tr");
      writer.WriteLine();
    }

    private int RenderTopic(HtmlTextWriter writer)
    {
      string priorityMessage = this.GetPriorityMessage(this._row);
      if (priorityMessage.IsSet())
      {
        writer.WriteBeginTag("span");
        writer.WriteAttribute("class", "post_priority");
        writer.Write(HtmlTextWriter.TagRightChar);
        writer.Write(priorityMessage);
        writer.WriteEndTag("span");
      }

      string linkParams = "t={0}";
      if (this.FindUnread)
      {
        linkParams += "&find=unread";
      }

      writer.RenderAnchorBegin(
        YafBuildLink.GetLink(ForumPages.posts, linkParams, this._row["LinkTopicID"]), 
        "post_link", 
        YafFormatMessage.GetCleanedTopicMessage(this._row["FirstMessage"], this._row["LinkTopicID"]).MessageTruncated);

      writer.WriteLine(YafServices.BadWordReplace.Replace(Convert.ToString(HtmlEncode(this._row["Subject"]))));
      writer.WriteEndTag("a");

      int actualPostCount = Convert.ToInt32(this._row["Replies"]) + 1;

      if (this.PageContext.BoardSettings.ShowDeletedMessages)
      {
        // add deleted posts not included in replies...
        actualPostCount += Convert.ToInt32(this._row["NumPostsDeleted"]);
      }
      return actualPostCount;
    }

    private void RenderPostPager(HtmlTextWriter writer, int actualPostCount)
    {
      string tPager = this.CreatePostPager(
        actualPostCount, this.PageContext.BoardSettings.PostsPerPage, Convert.ToInt32(this._row["LinkTopicID"]));

      if (tPager != String.Empty)
      {
        writer.WriteLine();
        writer.WriteBeginTag("span");
        writer.WriteAttribute("class", "topicPager smallfont");
        writer.Write(HtmlTextWriter.TagRightChar);

        writer.Write(" - ");

        // more then one page to show
        writer.Write(this.PageContext.Localization.GetText("GOTO_POST_PAGER").FormatWith(tPager));
        writer.WriteEndTag("span");
        writer.WriteLine();
      }
    }

    private void RenderPosted(HtmlTextWriter writer)
    {
      writer.WriteLine();
      writer.WriteBeginTag("span");
      writer.WriteAttribute("class", "topicPosted");
      writer.Write(HtmlTextWriter.TagRightChar);

      writer.Write(YafServices.DateTime.FormatDateTimeTopic(this._row.Row.Field<DateTime>("Posted")));

      writer.WriteEndTag("span");
      writer.WriteLine();
    }

    private void RenderTopicStarter(HtmlTextWriter writer)
    {
      writer.WriteLine();
      writer.WriteBreak();
      writer.WriteBeginTag("span");
      writer.WriteAttribute("class", "topicStarter");
      writer.Write(HtmlTextWriter.TagRightChar);

      // Topic Starter
      var topicStarterLink = new UserLink();
      topicStarterLink.ID = this.GetUniqueID("topicStarterLink");
      topicStarterLink.UserID = Convert.ToInt32(this._row["UserID"]);
      topicStarterLink.Style = this._row["StarterStyle"].ToString();

      // render the user link control
      //this.WriteBeginTD(writer, "topicStarter");
      topicStarterLink.RenderControl(writer);

      writer.Write(",");

      writer.WriteEndTag("span");
      writer.WriteLine();
    }

    /// <summary>
    /// Formats the Last Post for the Topic Line
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <returns>
    /// The render last post.
    /// </returns>
    protected string RenderLastPost(HtmlTextWriter writer)
    {
      string strReturn = this.PageContext.Localization.GetText("no_posts");
      DataRowView row = this._row;

      if (row["LastMessageID"].ToString().Length > 0)
      {
        int userID = Convert.ToInt32(row["LastUserID"]);

        if (PageContext.BoardSettings.ShowAvatarsInTopic)
        {
          string avatarUrl = GetAvatarUrlFromID(userID);
          writer.RenderImgTag(avatarUrl, this.AltLastPost, this.AltLastPost, "avatarimage");
        }

        string strMiniPost = this.PageContext.Theme.GetItem(
          "ICONS", 
          (DateTime.Parse(row["LastPosted"].ToString()) > Mession.GetTopicRead((int)this._row["TopicID"]))
            ? "ICON_NEWEST"
            : "ICON_LATEST");
        
        //writer.Write(this.PageContext.Localization.GetText("by").FormatWith(string.Empty));

        var byLink = new UserLink { UserID = userID, Style = row["LastUserStyle"].ToString() };

        byLink.RenderControl(writer);

        writer.Write("&nbsp;");

        writer.WriteBeginTag("a");
        if (string.IsNullOrEmpty(this.AltLastPost))
        {
            this.AltLastPost = this.PageContext.Localization.GetText("DEFAULT", "GO_LAST_POST");
        }

        writer.WriteAttribute("href", YafBuildLink.GetLink(ForumPages.posts, "m={0}#post{0}", row["LastMessageID"]));
        writer.WriteAttribute("title", this.AltLastPost);
        writer.Write(HtmlTextWriter.TagRightChar);

        writer.RenderImgTag(
          strMiniPost, 
          this.AltLastPost, 
          this.AltLastPost);

        writer.WriteEndTag("a");

        writer.WriteBreak();

        writer.Write(YafServices.DateTime.FormatDateTimeTopic(Convert.ToDateTime(row["LastPosted"])));
      }

      return strReturn;
    }

    private string GetAvatarUrlFromID(int userID)
    {
      string avatarUrl = YafServices.Avatar.GetAvatarUrlForUser(userID);

      if (avatarUrl.IsNotSet())
      {
        avatarUrl = "{0}images/noavatar.gif".FormatWith(YafForumInfo.ForumClientFileRoot);
      }
      return avatarUrl;
    }

    /// <summary>
    /// The format views.
    /// </summary>
    /// <returns>
    /// The format views.
    /// </returns>
    private string FormatViews()
    {
      int nViews = Convert.ToInt32(this._row["Views"]);
      return (this._row["TopicMovedID"].ToString().Length > 0) ? "&nbsp;" : "{0:N0}".FormatWith(nViews);
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
    private string MakeLink(string text, string link)
    {
      return "<a href=\"{0}\">{1}</a>".FormatWith(link, text);
    }

    #endregion
  }
}