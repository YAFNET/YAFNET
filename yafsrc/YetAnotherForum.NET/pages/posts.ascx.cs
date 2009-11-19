/* Yet Another Forum.NET
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
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Web;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.UI;
  using YAF.Classes.Utils;
  using YAF.Controls;
  using YAF.Editors;

  /// <summary>
  /// Summary description for posts.
  /// </summary>
  public partial class posts : ForumPage
  {
    /// <summary>
    /// The _data bound.
    /// </summary>
    private bool _dataBound = false;

    /// <summary>
    /// The _dt poll.
    /// </summary>
    private DataTable _dtPoll;

    /// <summary>
    /// The _forum.
    /// </summary>
    private DataRow _forum;

    /// <summary>
    /// The _forum flags.
    /// </summary>
    private ForumFlags _forumFlags = null;

    /// <summary>
    /// The _ignore query string.
    /// </summary>
    private bool _ignoreQueryString = false;

    /// <summary>
    /// The _quick reply editor.
    /// </summary>
    protected BaseForumEditor _quickReplyEditor;

    /// <summary>
    /// The _topic.
    /// </summary>
    private DataRow _topic;

    /// <summary>
    /// The _topic flags.
    /// </summary>
    private TopicFlags _topicFlags = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="posts"/> class.
    /// </summary>
    public posts()
      : base("POSTS")
    {
    }

    /// <summary>
    /// Gets VotingCookieName.
    /// </summary>
    protected string VotingCookieName
    {
      get
      {
        return String.Format("poll#{0}", this._topic["PollID"]);
      }
    }

    /// <summary>
    /// Property to verify if the current user can vote in this poll.
    /// </summary>
    protected bool CanVote
    {
      get
      {
        // rule out users without voting rights
        if (!PageContext.ForumVoteAccess)
        {
          return false;
        }

        if (IsPollClosed())
        {
          return false;
        }

        // check for voting cookie
        if (Request.Cookies[VotingCookieName] != null)
        {
          return false;
        }

        // voting is not tied to IP and they are a guest...
        if (PageContext.IsGuest && !PageContext.BoardSettings.PollVoteTiedToIP)
        {
          return true;
        }

        object UserID = null;
        object RemoteIP = null;

        if (PageContext.BoardSettings.PollVoteTiedToIP)
        {
          RemoteIP = IPHelper.IPStrToLong(Request.UserHostAddress).ToString();
        }

        if (!PageContext.IsGuest)
        {
          UserID = PageContext.PageUserID;
        }

        // check for a record of a vote
        using (DataTable dt = DB.pollvote_check(this._topic["PollID"], UserID, RemoteIP))
        {
          if (dt.Rows.Count == 0)
          {
            // user hasn't voted yet...
            return true;
          }
        }

        return false;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether IsThreaded.
    /// </summary>
    public bool IsThreaded
    {
      get
      {
        if (Request.QueryString["threaded"] != null)
        {
          Session["IsThreaded"] = bool.Parse(Request.QueryString["threaded"]);
        }
        else if (Session["IsThreaded"] == null)
        {
          Session["IsThreaded"] = false;
        }

        return (bool) Session["IsThreaded"];
      }

      set
      {
        Session["IsThreaded"] = value;
      }
    }

    /// <summary>
    /// Gets or sets CurrentMessage.
    /// </summary>
    protected int CurrentMessage
    {
      get
      {
        if (ViewState["CurrentMessage"] != null)
        {
          return (int) ViewState["CurrentMessage"];
        }
        else
        {
          return 0;
        }
      }

      set
      {
        ViewState["CurrentMessage"] = value;
      }
    }

    /// <summary>
    /// The posts_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void posts_PreRender(object sender, EventArgs e)
    {
      bool isWatched = HandleWatchTopic();

      // options menu...
      this.OptionsMenu.AddPostBackItem("watch", isWatched ? GetText("UNWATCHTOPIC") : GetText("WATCHTOPIC"));
      if (PageContext.BoardSettings.AllowEmailTopic)
      {
        this.OptionsMenu.AddPostBackItem("email", GetText("EMAILTOPIC"));
      }

      this.OptionsMenu.AddPostBackItem("print", GetText("PRINTTOPIC"));
      if (PageContext.BoardSettings.ShowRSSLink)
      {
        this.OptionsMenu.AddPostBackItem("rssfeed", GetText("RSSTOPIC"));
      }

      // view menu
      this.ViewMenu.AddPostBackItem("normal", GetText("NORMAL"));
      this.ViewMenu.AddPostBackItem("threaded", GetText("THREADED"));

      // attach both the menus to HyperLinks
      this.OptionsMenu.Attach(this.OptionsLink);
      this.ViewMenu.Attach(this.ViewLink);

      if (!this._dataBound)
      {
        BindData();
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
      this._quickReplyEditor.BaseDir = YafForumInfo.ForumRoot + "editors";
      this._quickReplyEditor.StyleSheet = PageContext.Theme.BuildThemePath("theme.css");

      this._topic = DB.topic_info(PageContext.PageTopicID);

      // in case topic is deleted or not existant
      if (this._topic == null)
      {
        YafBuildLink.Redirect(ForumPages.info, "i=6"); // invalid argument message
      }

      // get topic flags
      this._topicFlags = new TopicFlags(this._topic["Flags"]);

      using (DataTable dt = DB.forum_list(PageContext.PageBoardID, PageContext.PageForumID))
      {
        this._forum = dt.Rows[0];
      }

      this._forumFlags = new ForumFlags(this._forum["Flags"]);

      if (!PageContext.ForumReadAccess)
      {
        YafBuildLink.AccessDenied();
      }

      if (!IsPostBack)
      {
        if (PageContext.Settings.LockedForum == 0)
        {
          this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
          this.PageLinks.AddLink(PageContext.PageCategoryName, YafBuildLink.GetLink(ForumPages.forum, "c={0}", PageContext.PageCategoryID));
        }

        this.QuickReply.Text = GetText("POSTMESSAGE", "SAVE");
        this.DataPanel1.TitleText = GetText("QUICKREPLY");
        this.DataPanel1.ExpandText = GetText("QUICKREPLY_SHOW");
        this.DataPanel1.CollapseText = GetText("QUICKREPLY_HIDE");

        this.PageLinks.AddForumLinks(PageContext.PageForumID);
        this.PageLinks.AddLink(YafServices.BadWordReplace.Replace(Server.HtmlDecode(PageContext.PageTopicName)), string.Empty);

        this.TopicTitle.Text = YafServices.BadWordReplace.Replace((string) this._topic["Topic"]);

        this.ViewOptions.Visible = PageContext.BoardSettings.AllowThreaded;
        this.ForumJumpHolder.Visible = PageContext.BoardSettings.ShowForumJump && PageContext.Settings.LockedForum == 0;

        this.RssTopic.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.rsstopic, "pg={0}&t={1}", Request.QueryString["g"], PageContext.PageTopicID);
        this.RssTopic.Visible = PageContext.BoardSettings.ShowRSSLink;

        this.QuickReplyPlaceHolder.Visible = PageContext.BoardSettings.ShowQuickAnswer;

        if ((PageContext.IsGuest && PageContext.BoardSettings.EnableCaptchaForGuests) ||
            (PageContext.BoardSettings.EnableCaptchaForPost && !PageContext.IsCaptchaExcluded))
        {
          Session["CaptchaImageText"] = CaptchaHelper.GetCaptchaString();
          this.imgCaptcha.ImageUrl = String.Format("{0}resource.ashx?c=1", YafForumInfo.ForumRoot);
          this.CaptchaDiv.Visible = true;
        }

        if (!PageContext.ForumPostAccess || (this._forumFlags.IsLocked && !PageContext.ForumModeratorAccess))
        {
          this.NewTopic1.Visible = false;
          this.NewTopic2.Visible = false;
        }

        // Ederon : 9/9/2007 - moderators can reply in locked topics
        if (!PageContext.ForumReplyAccess || ((this._topicFlags.IsLocked || this._forumFlags.IsLocked) && !PageContext.ForumModeratorAccess))
        {
          this.PostReplyLink1.Visible = this.PostReplyLink2.Visible = false;
          this.QuickReplyPlaceHolder.Visible = false;
        }

        if (PageContext.ForumModeratorAccess)
        {
          this.MoveTopic1.Visible = true;
          this.MoveTopic2.Visible = true;
        }
        else
        {
          this.MoveTopic1.Visible = false;
          this.MoveTopic2.Visible = false;
        }

        if (!PageContext.ForumModeratorAccess)
        {
          this.LockTopic1.Visible = false;
          this.UnlockTopic1.Visible = false;
          this.DeleteTopic1.Visible = false;
          this.LockTopic2.Visible = false;
          this.UnlockTopic2.Visible = false;
          this.DeleteTopic2.Visible = false;
        }
        else
        {
          this.LockTopic1.Visible = !this._topicFlags.IsLocked;
          this.UnlockTopic1.Visible = !this.LockTopic1.Visible;
          this.LockTopic2.Visible = this.LockTopic1.Visible;
          this.UnlockTopic2.Visible = !this.LockTopic2.Visible;
        }
      }

      // Mark topic read
      Mession.SetTopicRead(PageContext.PageTopicID, DateTime.Now);

      BindData();
    }

    /// <summary>
    /// Adds meta data: description and keywords to the page header.
    /// </summary>
    /// <param name="firstMessage">
    /// first message in the topic
    /// </param>
    private void AddMetaData(object firstMessage)
    {
      if (Page.Header != null && PageContext.BoardSettings.AddDynamicPageMetaTags)
      {
        FormatMsg.MessageCleaned message = FormatMsg.GetCleanedTopicMessage(firstMessage, PageContext.PageTopicID);
        var meta = ControlHelper.FindControlType<HtmlMeta>(Page.Header);

        if (!String.IsNullOrEmpty(message.MessageTruncated))
        {
          HtmlMeta descriptionMeta = null;

          string content = String.Format("{0}: {1}", this._topic["Topic"], message.MessageTruncated);

          if (meta.Exists(x => x.Name.Equals("description")))
          {
            // use existing...
            descriptionMeta = meta.Where(x => x.Name.Equals("description")).FirstOrDefault();
            if (descriptionMeta != null)
            {
              descriptionMeta.Content = content;
            }
          }
          else
          {
            descriptionMeta = ControlHelper.MakeMetaDiscriptionControl(content);

            // add to the header...
            Page.Header.Controls.Add(descriptionMeta);
          }
        }

        if (message.MessageKeywords.Count > 0)
        {
          HtmlMeta keywordMeta = null;

          var keywordStr = message.MessageKeywords.Where(x => !String.IsNullOrEmpty(x)).ToList().ListToString(",");

          if (meta.Exists(x => x.Name.Equals("keywords")))
          {
            // use existing...
            keywordMeta = meta.Where(x => x.Name.Equals("keywords")).FirstOrDefault();
            keywordMeta.Content = keywordStr;
          }
          else
          {
            keywordMeta = ControlHelper.MakeMetaKeywordsControl(keywordStr);

            // add to the header...
            Page.Header.Controls.Add(keywordMeta);
          }
        }
      }
    }

    /// <summary>
    /// The delete message_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteMessage_Load(object sender, EventArgs e)
    {
      ((LinkButton) sender).Attributes["onclick"] = String.Format("return confirm('{0}')", GetText("confirm_deletemessage"));
    }

    /// <summary>
    /// The delete topic_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteTopic_Load(object sender, EventArgs e)
    {
      ((ThemeButton) sender).Attributes["onclick"] = String.Format("return confirm('{0}')", GetText("confirm_deletetopic"));
    }

    /// <summary>
    /// The quick reply_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void QuickReply_Click(object sender, EventArgs e)
    {
      if (!PageContext.ForumReplyAccess || (this._topicFlags.IsLocked && !PageContext.ForumModeratorAccess))
      {
        YafBuildLink.AccessDenied();
      }

      if (this._quickReplyEditor.Text.Length <= 0)
      {
        PageContext.AddLoadMessage(GetText("EMPTY_MESSAGE"));
        return;
      }

      if (((PageContext.IsGuest && PageContext.BoardSettings.EnableCaptchaForGuests) ||
           (PageContext.BoardSettings.EnableCaptchaForPost && !PageContext.IsCaptchaExcluded)) &&
          Session["CaptchaImageText"].ToString() != this.tbCaptcha.Text.Trim())
      {
        PageContext.AddLoadMessage(GetText("BAD_CAPTCHA"));
        return;
      }

      if (!(PageContext.IsAdmin || PageContext.IsModerator) && PageContext.BoardSettings.PostFloodDelay > 0)
      {
        if (Mession.LastPost > DateTime.Now.AddSeconds(-PageContext.BoardSettings.PostFloodDelay))
        {
          PageContext.AddLoadMessage(GetTextFormatted("wait", (Mession.LastPost - DateTime.Now.AddSeconds(-PageContext.BoardSettings.PostFloodDelay)).Seconds));
          return;
        }
      }

      Mession.LastPost = DateTime.Now;

      // post message...
      long nMessageID = 0;
      object replyTo = -1;
      string msg = this._quickReplyEditor.Text;
      long topicID = PageContext.PageTopicID;

      var tFlags = new MessageFlags();

      tFlags.IsHtml = this._quickReplyEditor.UsesHTML;
      tFlags.IsBBCode = this._quickReplyEditor.UsesBBCode;

      // Bypass Approval if Admin or Moderator.
      tFlags.IsApproved = PageContext.IsAdmin || PageContext.IsModerator;

      if (!DB.message_save(topicID, PageContext.PageUserID, msg, null, Request.UserHostAddress, null, replyTo, tFlags.BitValue, ref nMessageID))
      {
        topicID = 0;
      }

      bool bApproved = false;

      using (DataTable dt = DB.message_list(nMessageID))
      {
        foreach (DataRow row in dt.Rows)
        {
          bApproved = ((int) row["Flags"] & 16) == 16;
        }
      }

      if (bApproved)
      {
        // Ederon : 7/26/2007
        // send new post notification to users watching this topic/forum
        CreateMail.WatchEmail(nMessageID);

        // redirect to newly posted message
        YafBuildLink.Redirect(ForumPages.posts, "m={0}&#post{0}", nMessageID);
      }
      else
      {
        string url = YafBuildLink.GetLink(ForumPages.topics, "f={0}", PageContext.PageForumID);
        if (Config.IsRainbow)
        {
          YafBuildLink.Redirect(ForumPages.info, "i=1");
        }
        else
        {
          YafBuildLink.Redirect(ForumPages.info, "i=1&url={0}", Server.UrlEncode(url));
        }
      }
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
    private void Pager_PageChange(object sender, EventArgs e)
    {
      this._ignoreQueryString = true;
      this.SmartScroller1.Reset();
      BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this._dataBound = true;
      this.Pager.PageSize = PageContext.BoardSettings.PostsPerPage;

      if (this._topic == null)
      {
        YafBuildLink.Redirect(ForumPages.topics, "f={0}", PageContext.PageForumID);
      }

      var pds = new PagedDataSource();
      pds.AllowPaging = true;
      pds.PageSize = this.Pager.PageSize;

      DataTable dt0 = DB.post_list(
        PageContext.PageTopicID, 
        IsPostBack ? 0 : 1, 
        PageContext.BoardSettings.ShowDeletedMessages, 
        YafContext.Current.BoardSettings.UseStyledNicks, 
        YafContext.Current.BoardSettings.ShowThanksDate);

      var messageIDs = new StringBuilder();

      // Get All the MessageIDs in this topic.
      foreach (DataRow dr in dt0.Rows)
      {
        if (messageIDs.Length > 0)
        {
          messageIDs.Append(",");
        }

        messageIDs.AppendFormat("{0}", dr["MessageID"]);
      }

      // Add nescessary columns for later use in displaypost.ascx (Prevent repetitive 
      // calls to database.)
      var dc = new DataColumn("IsThankedByUser", Type.GetType("System.Boolean"));
      dt0.Columns.Add(dc);

      // How many times has this message been thanked.
      dc = new DataColumn("MessageThanksNumber", Type.GetType("System.Int32"));
      dt0.Columns.Add(dc);

      // How many times has the message poster thanked others?
      dc = new DataColumn("ThanksFromUserNumber", Type.GetType("System.Int32"));
      dt0.Columns.Add(dc);

      // How many times has the message poster been thanked?
      dc = new DataColumn("ThanksToUserNumber", Type.GetType("System.Int32"));
      dt0.Columns.Add(dc);

      // In how many posts has the message poster been thanked?
      dc = new DataColumn("ThanksToUserPostsNumber", Type.GetType("System.Int32"));
      dt0.Columns.Add(dc);

      // Make the "MessageID" Column the primary key to the datatable.
      dt0.PrimaryKey = new[]
        {
          dt0.Columns["MessageID"]
        };

      // Initialize the "IsthankedByUser" column.
      foreach (DataRow dr in dt0.Rows)
      {
        dr["IsThankedByUser"] = "false";
      }

      // Iterate through all the thanks relating to this topic and make appropriate
      // changes in columns.
      using (DataTable dt0AllThanks = DB.message_GetAllThanks(messageIDs.ToString()))
      {
        // get the default view...
        DataView dtAllThanks = dt0AllThanks.DefaultView;

        foreach (DataRow drThanks in dtAllThanks.Table.Rows)
        {
          if (drThanks["FromUserID"] != DBNull.Value)
          {
            if (Convert.ToInt32(drThanks["FromUserID"]) == PageContext.PageUserID)
            {
              dt0.Rows.Find(drThanks["MessageID"])["IsThankedByUser"] = "true";
            }
          }
        }

        foreach (DataRow dr in dt0.Rows)
        {
          dtAllThanks.RowFilter = String.Format("MessageID = {0} AND FromUserID is not NULL", Convert.ToInt32(dr["MessageID"]));
          dr["MessageThanksNumber"] = dtAllThanks.Count;
          dtAllThanks.RowFilter = String.Format("MessageID = {0}", Convert.ToInt32(dr["MessageID"]));
          dr["ThanksFromUserNumber"] = dtAllThanks.Count > 0 ? dtAllThanks[0]["ThanksFromUserNumber"] : 0;
          dr["ThanksToUserNumber"] = dtAllThanks.Count > 0 ? dtAllThanks[0]["ThanksToUserNumber"] : 0;
          dr["ThanksToUserPostsNumber"] = dtAllThanks.Count > 0 ? dtAllThanks[0]["ThanksToUserPostsNumber"] : 0;
        }
      }

      if (YafContext.Current.BoardSettings.UseStyledNicks)
      {
        StyleHelper.DecodeStyleByTable(ref dt0, true);
      }

      // get the default view...
      DataView dt = dt0.DefaultView;

      // see if the deleted messages need to be edited out...
      if (PageContext.BoardSettings.ShowDeletedMessages && !PageContext.BoardSettings.ShowDeletedMessagesToAll && !PageContext.IsAdmin &&
          !PageContext.IsForumModerator)
      {
        // remove posts that are deleted and do not belong to this user...
        dt.RowFilter = "IsDeleted = 1 AND UserID <> " + PageContext.PageUserID.ToString();

        foreach (DataRowView delRow in dt)
        {
          delRow.Delete();
        }

        dt.Table.AcceptChanges();

        // set row filter back to nothing...
        dt.RowFilter = null;
      }

      // set the sorting
      if (IsThreaded)
      {
        dt.Sort = "Position";
      }
      else
      {
        dt.Sort = "Posted";

        // reset position for updated sorting...
        int position = 0;
        foreach (DataRowView dataRow in dt)
        {
          dataRow.BeginEdit();
          dataRow["Position"] = position;
          position++;
          dataRow.EndEdit();
        }
      }

      pds.DataSource = dt;
      this.Pager.Count = dt.Count;

      int nFindMessage = 0;
      try
      {
        if (this._ignoreQueryString)
        {
        }
        else if (Request.QueryString["p"] != null)
        {
          // show specific page (p is 1 based)
          int tPage = Convert.ToInt32(Request.QueryString["p"]);
          if (pds.PageCount >= tPage)
          {
            pds.CurrentPageIndex = tPage - 1;
            this.Pager.CurrentPageIndex = pds.CurrentPageIndex;
          }
        }
        else if (Request.QueryString["m"] != null)
        {
          // Show this message
          nFindMessage = int.Parse(Request.QueryString["m"]);
        }
        else if (Request.QueryString["find"] != null && Request.QueryString["find"].ToLower() == "unread")
        {
          // Find next unread
          using (DataTable dtUnread = DB.message_findunread(PageContext.PageTopicID, Mession.LastVisit))
          {
            foreach (DataRow row in dtUnread.Rows)
            {
              nFindMessage = (int) row["MessageID"];
              break;
            }
          }
        }
      }
      catch (Exception x)
      {
        DB.eventlog_create(PageContext.PageUserID, this, x);
      }

      if (nFindMessage > 0)
      {
        CurrentMessage = nFindMessage;

        // Find correct page for message
        for (int foundRow = 0; foundRow < dt.Count; foundRow++)
        {
          if ((int) dt[foundRow]["MessageID"] == nFindMessage)
          {
            pds.CurrentPageIndex = foundRow / pds.PageSize;
            this.Pager.CurrentPageIndex = pds.CurrentPageIndex;
            break;
          }
        }
      }
      else
      {
        foreach (DataRowView row in dt)
        {
          CurrentMessage = (int) row["MessageID"];
          break;
        }
      }

      // handle add description/keywords for SEO
      AddMetaData(dt0.Rows[0]["Message"]);

      dt0 = null;
      pds.CurrentPageIndex = this.Pager.CurrentPageIndex;

      if (pds.CurrentPageIndex >= pds.PageCount)
      {
        pds.CurrentPageIndex = pds.PageCount - 1;
      }

      this.MessageList.DataSource = pds;

      if (this._topic["PollID"] != DBNull.Value)
      {
        this.Poll.Visible = true;
        this._dtPoll = DB.poll_stats(this._topic["PollID"]);
        this.Poll.DataSource = this._dtPoll;
      }

      DataBind();
    }

    /// <summary>
    /// The handle watch topic.
    /// </summary>
    /// <returns>
    /// The handle watch topic.
    /// </returns>
    private bool HandleWatchTopic()
    {
      if (PageContext.IsGuest)
      {
        return false;
      }

      // check if this forum is being watched by this user
      using (DataTable dt = DB.watchtopic_check(PageContext.PageUserID, PageContext.PageTopicID))
      {
        if (dt.Rows.Count > 0)
        {
          // subscribed to this forum
          this.TrackTopic.Text = GetText("UNWATCHTOPIC");
          foreach (DataRow row in dt.Rows)
          {
            this.WatchTopicID.InnerText = row["WatchTopicID"].ToString();
            return true;
          }
        }
        else
        {
          // not subscribed
          this.WatchTopicID.InnerText = string.Empty;
          this.TrackTopic.Text = GetText("WATCHTOPIC");
        }
      }

      return false;
    }

    /// <summary>
    /// The message list_ on item created.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void MessageList_OnItemCreated(object sender, RepeaterItemEventArgs e)
    {
      if (this.Pager.CurrentPageIndex == 0 && e.Item.ItemIndex == 0)
      {
        // check if need to display the ad...
        bool showAds = true;

        if (User != null)
        {
          showAds = PageContext.BoardSettings.ShowAdsToSignedInUsers;
        }

        if (!string.IsNullOrEmpty(PageContext.BoardSettings.AdPost) && showAds)
        {
          // first message... show the ad below this message
          var adControl = (DisplayAd) e.Item.FindControl("DisplayAd");
          if (adControl != null)
          {
            adControl.Visible = true;
          }
        }
      }
    }

    /// <summary>
    /// The delete topic_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteTopic_Click(object sender, EventArgs e)
    {
      if (!PageContext.ForumModeratorAccess)
      {
        YafBuildLink.AccessDenied( /*"You don't have access to delete topics."*/);
      }

      // Take away 10 points once!
      DB.user_removepointsByTopicID(PageContext.PageTopicID, 10);
      DB.topic_delete(PageContext.PageTopicID);
      YafBuildLink.Redirect(ForumPages.topics, "f={0}", PageContext.PageForumID);
    }

    /// <summary>
    /// The lock topic_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void LockTopic_Click(object sender, EventArgs e)
    {
      DB.topic_lock(PageContext.PageTopicID, true);
      BindData();
      PageContext.AddLoadMessage(GetText("INFO_TOPIC_LOCKED"));
      this.LockTopic1.Visible = !this.LockTopic1.Visible;
      this.UnlockTopic1.Visible = !this.UnlockTopic1.Visible;
      this.LockTopic2.Visible = this.LockTopic1.Visible;
      this.UnlockTopic2.Visible = this.UnlockTopic1.Visible;

      /*PostReplyLink1.Visible = false;
			PostReplyLink2.Visible = false;*/
    }

    /// <summary>
    /// The unlock topic_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void UnlockTopic_Click(object sender, EventArgs e)
    {
      DB.topic_lock(PageContext.PageTopicID, false);
      BindData();
      PageContext.AddLoadMessage(GetText("INFO_TOPIC_UNLOCKED"));
      this.LockTopic1.Visible = !this.LockTopic1.Visible;
      this.UnlockTopic1.Visible = !this.UnlockTopic1.Visible;
      this.LockTopic2.Visible = this.LockTopic1.Visible;
      this.UnlockTopic2.Visible = this.UnlockTopic1.Visible;
      this.PostReplyLink1.Visible = PageContext.ForumReplyAccess;
      this.PostReplyLink2.Visible = PageContext.ForumReplyAccess;
    }

    /// <summary>
    /// The poll_ item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Poll_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
      if (e.CommandName == "vote" && PageContext.ForumVoteAccess)
      {
        if (!CanVote)
        {
          PageContext.AddLoadMessage(GetText("WARN_ALREADY_VOTED"));
          return;
        }

        if (this._topicFlags.IsLocked)
        {
          PageContext.AddLoadMessage(GetText("WARN_TOPIC_LOCKED"));
          return;
        }

        if (IsPollClosed())
        {
          PageContext.AddLoadMessage(GetText("WARN_POLL_CLOSED"));
          return;
        }

        object userID = null;
        object remoteIP = null;

        if (PageContext.BoardSettings.PollVoteTiedToIP)
        {
          remoteIP = IPHelper.IPStrToLong(Request.ServerVariables["REMOTE_ADDR"]).ToString();
        }

        if (!PageContext.IsGuest)
        {
          userID = PageContext.PageUserID;
        }

        DB.choice_vote(e.CommandArgument, userID, remoteIP);

        // save the voting cookie...
        var c = new HttpCookie(VotingCookieName, e.CommandArgument.ToString());
        c.Expires = DateTime.Now.AddYears(1);
        Response.Cookies.Add(c);

        PageContext.AddLoadMessage(GetText("INFO_VOTED"));
        BindData();
      }
    }

    /// <summary>
    /// The is poll closed.
    /// </summary>
    /// <returns>
    /// The is poll closed.
    /// </returns>
    protected bool IsPollClosed()
    {
      bool bIsClosed = false;

      if (this._dtPoll.Rows[0]["Closes"] != DBNull.Value)
      {
        DateTime tCloses = Convert.ToDateTime(this._dtPoll.Rows[0]["Closes"]);
        if (tCloses < DateTime.Now)
        {
          bIsClosed = true;
        }
      }

      return bIsClosed;
    }

    /// <summary>
    /// The get poll is closed.
    /// </summary>
    /// <returns>
    /// The get poll is closed.
    /// </returns>
    protected string GetPollIsClosed()
    {
      string strPollClosed = string.Empty;
      if (IsPollClosed())
      {
        strPollClosed = GetText("POLL_CLOSED");
      }

      return strPollClosed;
    }

    /// <summary>
    /// The get poll question.
    /// </summary>
    /// <returns>
    /// The get poll question.
    /// </returns>
    protected string GetPollQuestion()
    {
      return HtmlEncode(YafServices.BadWordReplace.Replace(this._dtPoll.Rows[0]["Question"].ToString()));
    }

    /// <summary>
    /// The post reply link_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PostReplyLink_Click(object sender, EventArgs e)
    {
      // Ederon : 9/9/2007 - moderator can reply in locked posts
      if (!PageContext.ForumModeratorAccess)
      {
        if (this._topicFlags.IsLocked)
        {
          PageContext.AddLoadMessage(GetText("WARN_TOPIC_LOCKED"));
          return;
        }

        if (this._forumFlags.IsLocked)
        {
          PageContext.AddLoadMessage(GetText("WARN_FORUM_LOCKED"));
          return;
        }
      }

      YafBuildLink.Redirect(ForumPages.postmessage, "t={0}&f={1}", PageContext.PageTopicID, PageContext.PageForumID);
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

      YafBuildLink.Redirect(ForumPages.postmessage, "f={0}", PageContext.PageForumID);
    }

    /// <summary>
    /// The track topic_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void TrackTopic_Click(object sender, EventArgs e)
    {
      if (PageContext.IsGuest)
      {
        PageContext.AddLoadMessage(GetText("WARN_WATCHLOGIN"));
        return;
      }

      if (this.WatchTopicID.InnerText == string.Empty)
      {
        DB.watchtopic_add(PageContext.PageUserID, PageContext.PageTopicID);
        PageContext.AddLoadMessage(GetText("INFO_WATCH_TOPIC"));
      }
      else
      {
        int tmpID = Convert.ToInt32(this.WatchTopicID.InnerText);
        DB.watchtopic_delete(tmpID);
        PageContext.AddLoadMessage(GetText("INFO_UNWATCH_TOPIC"));
      }

      HandleWatchTopic();

      BindData();
    }

    /// <summary>
    /// The move topic_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void MoveTopic_Click(object sender, EventArgs e)
    {
      if (!PageContext.ForumModeratorAccess)
      {
        YafBuildLink.AccessDenied( /*"You are not a forum moderator."*/);
      }

      YafBuildLink.Redirect(ForumPages.movetopic, "t={0}", PageContext.PageTopicID);
    }

    /// <summary>
    /// The prev topic_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PrevTopic_Click(object sender, EventArgs e)
    {
      using (DataTable dt = DB.topic_findprev(PageContext.PageTopicID))
      {
        if (dt.Rows.Count == 0)
        {
          PageContext.AddLoadMessage(GetText("INFO_NOMORETOPICS"));
          return;
        }

        YafBuildLink.Redirect(ForumPages.posts, "t={0}", dt.Rows[0]["TopicID"]);
      }
    }

    /// <summary>
    /// The next topic_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void NextTopic_Click(object sender, EventArgs e)
    {
      using (DataTable dt = DB.topic_findnext(PageContext.PageTopicID))
      {
        if (dt.Rows.Count == 0)
        {
          PageContext.AddLoadMessage(GetText("INFO_NOMORETOPICS"));
          return;
        }

        YafBuildLink.Redirect(ForumPages.posts, "t={0}", dt.Rows[0]["TopicID"]);
      }
    }

    /// <summary>
    /// The email topic_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void EmailTopic_Click(object sender, EventArgs e)
    {
      if (User == null)
      {
        PageContext.AddLoadMessage(GetText("WARN_EMAILLOGIN"));
        return;
      }

      YafBuildLink.Redirect(ForumPages.emailtopic, "t={0}", PageContext.PageTopicID);
    }

    /// <summary>
    /// The print topic_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PrintTopic_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.printtopic, "t={0}", PageContext.PageTopicID);
    }

    /// <summary>
    /// The vote width.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The vote width.
    /// </returns>
    protected int VoteWidth(object o)
    {
      var row = (DataRowView) o;
      return (int) row["Stats"] * 80 / 100;
    }

    /// <summary>
    /// The is current message.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The is current message.
    /// </returns>
    protected bool IsCurrentMessage(object o)
    {
      var row = (DataRowView) o;

      return !IsThreaded || CurrentMessage == (int) row["MessageID"];
    }

    /// <summary>
    /// The get threaded row.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The get threaded row.
    /// </returns>
    protected string GetThreadedRow(object o)
    {
      var row = (DataRowView) o;
      if (!IsThreaded || CurrentMessage == (int) row["MessageID"])
      {
        return string.Empty;
      }

      var html = new StringBuilder(1000);

      // Threaded
      string brief = row["Message"].ToString();

      RegexOptions options = RegexOptions.IgnoreCase /*| RegexOptions.Singleline | RegexOptions.Multiline*/;
      options |= RegexOptions.Singleline;
      while (Regex.IsMatch(brief, @"\[quote=(.*?)\](.*)\[/quote\]", options))
      {
        brief = Regex.Replace(brief, @"\[quote=(.*?)\](.*)\[/quote\]", string.Empty, options);
      }

      while (Regex.IsMatch(brief, @"\[quote\](.*)\[/quote\]", options))
      {
        brief = Regex.Replace(brief, @"\[quote\](.*)\[/quote\]", string.Empty, options);
      }

      while (Regex.IsMatch(brief, @"<.*?>", options))
      {
        brief = Regex.Replace(brief, @"<.*?>", string.Empty, options);
      }

      while (Regex.IsMatch(brief, @"\[.*?\]", options))
      {
        brief = Regex.Replace(brief, @"\[.*?\]", string.Empty, options);
      }

      brief = YafServices.BadWordReplace.Replace(brief);

      if (brief.Length > 42)
      {
        brief = brief.Substring(0, 40) + "...";
      }

      brief = FormatMsg.AddSmiles(brief);

      html.AppendFormat("<tr class='post'><td colspan='3' nowrap>");
      html.AppendFormat(GetIndentImage(row["Indent"]));
      html.AppendFormat("\n<a href='{0}'>{2} ({1}", YafBuildLink.GetLink(ForumPages.posts, "m={0}#{0}", row["MessageID"]), row["UserName"], brief);
      html.AppendFormat(" - {0})</a>", YafServices.DateTime.FormatDateTimeShort(row["Posted"]));
      html.AppendFormat("</td></tr>");

      return html.ToString();
    }

    /// <summary>
    /// The get indent image.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The get indent image.
    /// </returns>
    protected string GetIndentImage(object o)
    {
      if (!IsThreaded)
      {
        return string.Empty;
      }

      var iIndent = (int) o;
      if (iIndent > 0)
      {
        return string.Format("<img src='{1}images/spacer.gif' width='{0}' alt='' height='2'/>", iIndent * 32, YafForumInfo.ForumRoot);
      }
      else
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// The get total.
    /// </summary>
    /// <returns>
    /// The get total.
    /// </returns>
    protected string GetTotal()
    {
      return HtmlEncode(this._dtPoll.Rows[0]["Total"].ToString());
    }

    /// <summary>
    /// The options menu_ item click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    /// <exception cref="ApplicationException">
    /// </exception>
    private void OptionsMenu_ItemClick(object sender, PopEventArgs e)
    {
      switch (e.Item.ToLower())
      {
        case "print":
          YafBuildLink.Redirect(ForumPages.printtopic, "t={0}", PageContext.PageTopicID);
          break;
        case "watch":
          TrackTopic_Click(sender, e);
          break;
        case "email":
          EmailTopic_Click(sender, e);
          break;
        case "rssfeed":
          YafBuildLink.Redirect(ForumPages.rsstopic, "pg={0}&t={1}", Request.QueryString["g"], PageContext.PageTopicID);
          break;
        default:
          throw new ApplicationException(e.Item);
      }
    }

    /// <summary>
    /// The view menu_ item click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    /// <exception cref="ApplicationException">
    /// </exception>
    private void ViewMenu_ItemClick(object sender, PopEventArgs e)
    {
      switch (e.Item.ToLower())
      {
        case "normal":
          IsThreaded = false;
          BindData();
          break;
        case "threaded":
          IsThreaded = true;
          BindData();
          break;
        default:
          throw new ApplicationException(e.Item);
      }
    }

    #region Web Form Designer generated code

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      // Quick Reply Modification Begin
      _quickReplyEditor = new BasicBBCodeEditor();
      QuickReplyLine.Controls.Add(_quickReplyEditor);
      QuickReply.Click += new EventHandler(QuickReply_Click);
      Pager.PageChange += new EventHandler(Pager_PageChange);

      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      InitializeComponent();
      base.OnInit(e);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.Poll.ItemCommand += new RepeaterCommandEventHandler(this.Poll_ItemCommand);
      this.PreRender += new EventHandler(posts_PreRender);
      this.OptionsMenu.ItemClick += new PopEventHandler(OptionsMenu_ItemClick);
      this.ViewMenu.ItemClick += new PopEventHandler(ViewMenu_ItemClick);
    }

    #endregion
  }
}