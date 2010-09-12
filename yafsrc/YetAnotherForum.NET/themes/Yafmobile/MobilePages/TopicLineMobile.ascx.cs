/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System;
using System.Data;
using System.Text;
using System.Web.UI;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Controls
{
  using Classes.UI;

  /// <summary>
  /// The topic line.
  /// </summary>
  [ParseChildren(false)]
  public partial class TopicLineMobile : BaseUserControl
  {
    /// <summary>
    /// The _is alt.
    /// </summary>
    private bool _isAlt;

    /// <summary>
    /// The _row.
    /// </summary>
    private DataRowView _row = null;

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
    /// Gets or sets a value indicating whether FindUnread.
    /// </summary>
    public bool FindUnread
    {
      get
      {
        return (ViewState["FindUnread"] != null) ? Convert.ToBoolean(ViewState["FindUnread"]) : false;
      }

      set
      {
        ViewState["FindUnread"] = value;
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
      writer.WriteAttribute("class", IsAlt ? "topicRow_Alt post_alt" : "topicRow post");
      writer.Write(HtmlTextWriter.TagRightChar);

      // Icon
      string imgTitle = string.Empty;
      string imgSrc = GetTopicImage(this._row, ref imgTitle);

      writer.WriteBeginTD();
      writer.RenderImgTag(imgSrc, imgTitle, imgTitle);
      writer.WriteEndTD();

      // Topic
      writer.WriteBeginTD("topicMain");

      string priorityMessage = GetPriorityMessage(this._row);
      if (priorityMessage.IsSet())
      {
        writer.WriteBeginTag("span");
        writer.WriteAttribute("class", "post_priority");
        writer.Write(HtmlTextWriter.TagRightChar);
        writer.Write(priorityMessage);
        writer.WriteEndTag("span");
      }

      string linkParams = "t={0}";
      if (FindUnread)
      {
        linkParams += "&find=unread";
      }

      writer.RenderAnchorBegin(
        YafBuildLink.GetLink(ForumPages.posts, linkParams, this._row["LinkTopicID"]),
        "post_link",
        YafFormatMessage.GetCleanedTopicMessage(this._row["FirstMessage"], this._row["LinkTopicID"]).MessageTruncated);

      writer.WriteLine(YafServices.BadWordReplace.Replace(Convert.ToString(this._row["Subject"])));
      writer.WriteEndTag("a");

      int actualPostCount = Convert.ToInt32(this._row["Replies"]) + 1;

      if (PageContext.BoardSettings.ShowDeletedMessages)
      {
        // add deleted posts not included in replies...
        actualPostCount += Convert.ToInt32(this._row["NumPostsDeleted"]);
      }

      string tPager = CreatePostPager(actualPostCount, PageContext.BoardSettings.PostsPerPage, Convert.ToInt32(this._row["LinkTopicID"]));

      if (tPager != String.Empty)
      {
        writer.WriteLine();
        writer.WriteBreak();
        writer.WriteBeginTag("span");
        writer.WriteAttribute("class", "topicPager smallfont");
        writer.Write(HtmlTextWriter.TagRightChar);

        // more then one page to show
        writer.Write(this.PageContext.Localization.GetText("GOTO_POST_PAGER").FormatWith(tPager));
        writer.WriteEndTag("span");
        writer.WriteLine();
      }

      writer.WriteEndTD();
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
      return (this._row["TopicMovedID"].ToString().Length > 0) ? "&nbsp;" : String.Format("{0:N0}", nViews);
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
          imgTitle = PageContext.Localization.GetText("MOVED");
          return PageContext.Theme.GetItem("ICONS", "TOPIC_MOVED");
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
            imgTitle = PageContext.Localization.GetText("POLL_NEW");
            return PageContext.Theme.GetItem("ICONS", "TOPIC_POLL_NEW");
          }
          else if (row["Priority"].ToString() == "1")
          {
            imgTitle = PageContext.Localization.GetText("STICKY");
            return PageContext.Theme.GetItem("ICONS", "TOPIC_STICKY");
          }
          else if (row["Priority"].ToString() == "2")
          {
            imgTitle = PageContext.Localization.GetText("ANNOUNCEMENT");
            return PageContext.Theme.GetItem("ICONS", "TOPIC_ANNOUNCEMENT_NEW");
          }
          else if (topicFlags.IsLocked || forumFlags.IsLocked)
          {
            imgTitle = PageContext.Localization.GetText("NEW_POSTS_LOCKED");
            return PageContext.Theme.GetItem("ICONS", "TOPIC_NEW_LOCKED");
          }
          else
          {
            imgTitle = PageContext.Localization.GetText("NEW_POSTS");
            return PageContext.Theme.GetItem("ICONS", "TOPIC_NEW");
          }
        }
        else
        {
          if (row["PollID"] != DBNull.Value)
          {
            imgTitle = PageContext.Localization.GetText("POLL");
            return PageContext.Theme.GetItem("ICONS", "TOPIC_POLL");
          }
          else if (row["Priority"].ToString() == "1")
          {
            imgTitle = PageContext.Localization.GetText("STICKY");
            return PageContext.Theme.GetItem("ICONS", "TOPIC_STICKY");
          }
          else if (row["Priority"].ToString() == "2")
          {
            imgTitle = PageContext.Localization.GetText("ANNOUNCEMENT");
            return PageContext.Theme.GetItem("ICONS", "TOPIC_ANNOUNCEMENT");
          }
          else if (topicFlags.IsLocked || forumFlags.IsLocked)
          {
            imgTitle = PageContext.Localization.GetText("NO_NEW_POSTS_LOCKED");
            return PageContext.Theme.GetItem("ICONS", "TOPIC_LOCKED");
          }
          else
          {
            imgTitle = PageContext.Localization.GetText("NO_NEW_POSTS");
            return PageContext.Theme.GetItem("ICONS", "TOPIC");
          }
        }
      }
      catch (Exception)
      {
        return PageContext.Theme.GetItem("ICONS", "TOPIC");
      }
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
        strReturn = PageContext.Localization.GetText("MOVED");
      }
      else if (row["PollID"].ToString() != string.Empty)
      {
        strReturn = PageContext.Localization.GetText("POLL");
      }
      else
      {
        switch (int.Parse(row["Priority"].ToString()))
        {
          case 1:
            strReturn = PageContext.Localization.GetText("STICKY");
            break;
          case 2:
            strReturn = PageContext.Localization.GetText("ANNOUNCEMENT");
            break;
        }
      }

      if (strReturn.Length > 0)
      {
        strReturn = String.Format("[ {0} ] ", strReturn);
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
        if (PageContext.BoardSettings.ShowDeletedMessages && numDeleted > 0)
        {
          repStr = String.Format("{0:N0}", nReplies + numDeleted);
        }
        else
        {
          repStr = String.Format("{0:N0}", nReplies);
        }
      }

      return repStr;
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
      string strReturn = string.Empty;

      int NumToDisplay = 4;
      var PageCount = (int)Math.Ceiling((double)count / pageSize);

      if (PageCount > 1)
      {
        if (PageCount > NumToDisplay)
        {
          strReturn += MakeLink("1", YafBuildLink.GetLink(ForumPages.posts, "t={0}", topicID));
          strReturn += " ... ";
          bool bFirst = true;

          // show links from the end
          for (int i = PageCount - (NumToDisplay - 1); i < PageCount; i++)
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

            strReturn += MakeLink(iPost.ToString(), YafBuildLink.GetLink(ForumPages.posts, "t={0}&p={1}", topicID, iPost));
          }
        }
        else
        {
          bool bFirst = true;
          for (int i = 0; i < PageCount; i++)
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

            strReturn += MakeLink(iPost.ToString(), YafBuildLink.GetLink(ForumPages.posts, "t={0}&p={1}", topicID, iPost));
          }
        }
      }

      return strReturn;
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
      return String.Format("<a href=\"{0}\">{1}</a>", link, text);
    }
  }
}