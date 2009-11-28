/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Pages
{
  // YAF.Pages
  using System;
  using System.Data;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for topics.
  /// </summary>
  public partial class topics : ForumPage
  {
    /// <summary>
    /// The _forum.
    /// </summary>
    DataRow _forum;

    /// <summary>
    /// The _forum flags.
    /// </summary>
    ForumFlags _forumFlags;

    /// <summary>
    /// The _show topic list selected.
    /// </summary>
    protected int _showTopicListSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="topics"/> class. 
    /// Overloads the topics page.
    /// </summary>
    public topics()
      : base("TOPICS")
    {
    }

    /// <summary>
    /// The topics_ unload.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    void topics_Unload(object sender, EventArgs e)
    {
      if (Mession.UnreadTopics == 0)
      {
        Mession.SetForumRead(PageContext.PageForumID, DateTime.Now);
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
    protected void Page_Load(object sender, EventArgs e)
    {
      Mession.UnreadTopics = 0;
      this.RssFeed.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.rsstopic, "pg=topics&f={0}", Request.QueryString["f"]);
      this.RssFeed.Text = GetText("RSSFEED");
      this.RssFeed.Visible = PageContext.BoardSettings.ShowRSSLink;
      this.MarkRead.Text = GetText("MARKREAD");
      this.RSSLinkSpacer.Visible = PageContext.BoardSettings.ShowRSSLink;
      this.ForumJumpHolder.Visible = PageContext.BoardSettings.ShowForumJump && PageContext.Settings.LockedForum == 0;

      if (!IsPostBack)
      {
        // PageLinks.Clear();
        if (PageContext.Settings.LockedForum == 0)
        {
          this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
          this.PageLinks.AddLink(PageContext.PageCategoryName, YafBuildLink.GetLink(ForumPages.forum, "c={0}", PageContext.PageCategoryID));
        }

        this.PageLinks.AddForumLinks(PageContext.PageForumID, true);

        this.ShowList.DataSource = StaticDataHelper.TopicTimes();
        this.ShowList.DataTextField = "TopicText";
        this.ShowList.DataValueField = "TopicValue";
        this._showTopicListSelected = (Mession.ShowList == -1) ? PageContext.BoardSettings.ShowTopicsDefault : Mession.ShowList;

        HandleWatchForum();
      }

      if (Request.QueryString["f"] == null)
      {
        YafBuildLink.AccessDenied();
      }

      if (!PageContext.ForumReadAccess)
      {
        YafBuildLink.AccessDenied();
      }

      using (DataTable dt = DB.forum_list(PageContext.PageBoardID, PageContext.PageForumID))
      {
        this._forum = dt.Rows[0];
      }

      if (this._forum["RemoteURL"] != DBNull.Value)
      {
        Response.Clear();
        Response.Redirect((string) this._forum["RemoteURL"]);
      }

      this._forumFlags = new ForumFlags(this._forum["Flags"]);

      this.PageTitle.Text = (string) this._forum["Name"];

      BindData(); // Always because of yaf:TopicLine

      if (!PageContext.ForumPostAccess || (this._forumFlags.IsLocked && !PageContext.ForumModeratorAccess))
      {
        this.NewTopic1.Visible = false;
        this.NewTopic2.Visible = false;
      }

      if (!PageContext.ForumModeratorAccess)
      {
        this.moderate1.Visible = false;
        this.moderate2.Visible = false;
      }
    }

    /// <summary>
    /// The handle watch forum.
    /// </summary>
    void HandleWatchForum()
    {
      if (PageContext.IsGuest || !PageContext.ForumReadAccess)
      {
        return;
      }

      // check if this forum is being watched by this user
      using (DataTable dt = DB.watchforum_check(PageContext.PageUserID, PageContext.PageForumID))
      {
        if (dt.Rows.Count > 0)
        {
          // subscribed to this forum
          this.WatchForum.Text = GetText("unwatchforum");
          foreach (DataRow row in dt.Rows)
          {
            this.WatchForumID.InnerText = row["WatchForumID"].ToString();
            break;
          }
        }
        else
        {
          // not subscribed
          this.WatchForumID.InnerText = string.Empty;
          this.WatchForum.Text = GetText("watchforum");
        }
      }
    }

    /// <summary>
    /// The mark read_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    void MarkRead_Click(object sender, EventArgs e)
    {
      Mession.SetForumRead(PageContext.PageForumID, DateTime.Now);
      BindData();
    }

    /// <summary>
    /// The pager_ page change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    void Pager_PageChange(object sender, EventArgs e)
    {
      this.SmartScroller1.Reset();
      BindData();
    }

    /// <summary>
    /// The moderate_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void moderate_Click(object sender, EventArgs e)
    {
      if (PageContext.ForumModeratorAccess)
      {
        YafBuildLink.Redirect(ForumPages.moderate, "f={0}", PageContext.PageForumID);
      }
    }

    /// <summary>
    /// The show list_ selected index changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    void ShowList_SelectedIndexChanged(object sender, EventArgs e)
    {
      this._showTopicListSelected = this.ShowList.SelectedIndex;
      BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    void BindData()
    {
      DataSet ds = YafServices.DBBroker.BoardLayout(PageContext.PageBoardID, PageContext.PageUserID, PageContext.PageCategoryID, PageContext.PageForumID);
      if (ds.Tables[YafDBAccess.GetObjectName("Forum")].Rows.Count > 0)
      {
        this.ForumList.DataSource = ds.Tables[YafDBAccess.GetObjectName("Forum")].Rows;
        this.SubForums.Visible = true;
      }

      this.Pager.PageSize = PageContext.BoardSettings.TopicsPerPage;

      // when userId is null it returns the count of all deleted messages
      int? userId = null;

      // get the userID to use for the deleted posts count...
      if (!PageContext.BoardSettings.ShowDeletedMessagesToAll)
      {
        // only show deleted messages that belong to this user if they are not admin/mod
        if (!PageContext.IsAdmin && !PageContext.ForumModeratorAccess)
        {
          userId = PageContext.PageUserID;
        }
      }

      DataTable dt = StyleTransformDataTable(DB.topic_list(PageContext.PageForumID, userId, 1, null, 0, 10, PageContext.BoardSettings.UseStyledNicks));

      int nPageSize = Math.Max(5, this.Pager.PageSize - dt.Rows.Count);
      this.Announcements.DataSource = dt;

      /*if ( !m_bIgnoreQueryString && Request.QueryString ["p"] != null )
			{
				// show specific page (p is 1 based)
				int tPage = (int)Security.StringToLongOrRedirect( Request.QueryString ["p"] );

				if ( tPage > 0 )
				{
					Pager.CurrentPageIndex = tPage - 1;
				}
			}*/
      int nCurrentPageIndex = this.Pager.CurrentPageIndex;

      DataTable dtTopics;
      if (this._showTopicListSelected == 0)
      {
        dtTopics =
          StyleTransformDataTable(
            DB.topic_list(PageContext.PageForumID, userId, 0, null, nCurrentPageIndex * nPageSize, nPageSize, PageContext.BoardSettings.UseStyledNicks));
      }
      else
      {
        DateTime date = DateTime.Now;
        switch (this._showTopicListSelected)
        {
          case 1:
            date -= TimeSpan.FromDays(1);
            break;
          case 2:
            date -= TimeSpan.FromDays(2);
            break;
          case 3:
            date -= TimeSpan.FromDays(7);
            break;
          case 4:
            date -= TimeSpan.FromDays(14);
            break;
          case 5:
            date -= TimeSpan.FromDays(31);
            break;
          case 6:
            date -= TimeSpan.FromDays(2 * 31);
            break;
          case 7:
            date -= TimeSpan.FromDays(6 * 31);
            break;
          case 8:
            date -= TimeSpan.FromDays(365);
            break;
        }

        dtTopics =
          StyleTransformDataTable(
            DB.topic_list(PageContext.PageForumID, userId, 0, date, nCurrentPageIndex * nPageSize, nPageSize, PageContext.BoardSettings.UseStyledNicks));
      }

      int nRowCount = 0;
      if (dtTopics.Rows.Count > 0)
      {
        nRowCount = (int) dtTopics.Rows[0]["RowCount"];
      }

      int nPageCount = (nRowCount + nPageSize - 1) / nPageSize;

      this.TopicList.DataSource = dtTopics;

      DataBind();

      // setup the show topic list selection after data binding
      this.ShowList.SelectedIndex = this._showTopicListSelected;
      Mession.ShowList = this._showTopicListSelected;

      this.Pager.Count = nRowCount;
    }

    /// <summary>
    /// The style transform func wrap.
    /// </summary>
    /// <param name="dt">
    /// The DateTable
    /// </param>
    /// <returns>
    /// The style transform wrap.
    /// </returns>
    public DataTable StyleTransformDataTable(DataTable dt)
    {
      if (YafContext.Current.BoardSettings.UseStyledNicks)
      {
        var styleTransform = new StyleTransform(YafContext.Current.Theme);
        styleTransform.DecodeStyleByTable(ref dt, true, "StarterStyle", "LastUserStyle");
      }

      return dt;
    }

    /// <summary>
    /// The new topic_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void NewTopic_Click(object sender, EventArgs e)
    {
      if (this._forumFlags.IsLocked)
      {
        PageContext.AddLoadMessage(GetText("WARN_FORUM_LOCKED"));
        return;
      }

      if (!PageContext.ForumPostAccess)
      {
        YafBuildLink.AccessDenied( /*"You don't have access to post new topics in this forum."*/);
      }

      YafBuildLink.Redirect(ForumPages.postmessage, "f={0}", PageContext.PageForumID);
    }

    /// <summary>
    /// The watch forum_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void WatchForum_Click(object sender, EventArgs e)
    {
      if (!PageContext.ForumReadAccess)
      {
        return;
      }

      if (PageContext.IsGuest)
      {
        PageContext.AddLoadMessage(GetText("WARN_LOGIN_FORUMWATCH"));
        return;
      }

      if (this.WatchForumID.InnerText == string.Empty)
      {
        DB.watchforum_add(PageContext.PageUserID, PageContext.PageForumID);
        PageContext.AddLoadMessage(GetText("INFO_WATCH_FORUM"));
      }
      else
      {
        int tmpID = Convert.ToInt32(this.WatchForumID.InnerText);
        DB.watchforum_delete(tmpID);
        PageContext.AddLoadMessage(GetText("INFO_UNWATCH_FORUM"));
      }

      HandleWatchForum();
    }

    /// <summary>
    /// The get sub forum title.
    /// </summary>
    /// <returns>
    /// The get sub forum title.
    /// </returns>
    protected string GetSubForumTitle()
    {
      return GetTextFormatted("SUBFORUMS", PageContext.PageForumName);
    }

    #region Web Form Designer generated code

    /// <summary>
    /// The initialization script for the topics page.
    /// </summary>
    /// <param name="e">
    /// The EventArgs object for the topics page.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      this.Unload += new EventHandler(this.topics_Unload);
      moderate1.Click += new EventHandler(this.moderate_Click);
      moderate2.Click += new EventHandler(this.moderate_Click);
      ShowList.SelectedIndexChanged += new EventHandler(this.ShowList_SelectedIndexChanged);
      MarkRead.Click += new EventHandler(this.MarkRead_Click);
      Pager.PageChange += new EventHandler(this.Pager_PageChange);
      this.NewTopic1.Click += new EventHandler(this.NewTopic_Click);
      this.NewTopic2.Click += new EventHandler(this.NewTopic_Click);
      this.WatchForum.Click += new EventHandler(this.WatchForum_Click);

      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      base.OnInit(e);
    }

    #endregion
  }
}