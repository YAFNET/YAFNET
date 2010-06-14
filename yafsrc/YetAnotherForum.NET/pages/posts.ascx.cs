/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Pages
{
  // YAF.Pages
  #region Using

  using System;
  using System.Data;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Web;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  using AjaxPro;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Extensions;
  using YAF.Classes.UI;
  using YAF.Classes.Utils;
  using YAF.Controls;
  using YAF.Editors;
  using YAF.Utilities;

  #endregion

  /// <summary>
  /// Summary description for posts.
  /// </summary>
  public partial class posts : ForumPage
  {
    #region Constants and Fields

    /// <summary>
    ///   The _quick reply editor.
    /// </summary>
    protected BaseForumEditor _quickReplyEditor;

    /// <summary>
    ///   The _data bound.
    /// </summary>
    private bool _dataBound;

    /// <summary>
    ///   The _dt poll.
    /// </summary>
    private DataTable _dtPoll;

    /// <summary>
    ///   The _forum.
    /// </summary>
    private DataRow _forum;

    /// <summary>
    ///   The _forum flags.
    /// </summary>
    private ForumFlags _forumFlags;

    /// <summary>
    ///   The _ignore query string.
    /// </summary>
    private bool _ignoreQueryString;

    /// <summary>
    ///   The _topic.
    /// </summary>
    private DataRow _topic;

    /// <summary>
    ///   The _topic flags.
    /// </summary>
    private TopicFlags _topicFlags;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "posts" /> class.
    /// </summary>
    public posts()
      : base("POSTS")
    {
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets a value indicating whether IsThreaded.
    /// </summary>
    public bool IsThreaded
    {
      get
      {
        if (this.Request.QueryString["threaded"] != null)
        {
          this.Session["IsThreaded"] = bool.Parse(this.Request.QueryString["threaded"]);
        }
        else if (this.Session["IsThreaded"] == null)
        {
          this.Session["IsThreaded"] = false;
        }

        return (bool)this.Session["IsThreaded"];
      }

      set
      {
        this.Session["IsThreaded"] = value;
      }
    }

    /// <summary>
    ///   Property to verify if the current user can vote in this poll.
    /// </summary>
    protected bool CanVote
    {
      get
      {
        // rule out users without voting rights
        if (!this.PageContext.ForumVoteAccess)
        {
          return false;
        }

        if (this.IsPollClosed())
        {
          return false;
        }

        // check for voting cookie
        if (this.Request.Cookies[this.VotingCookieName] != null)
        {
          return false;
        }

        // voting is not tied to IP and they are a guest...
        if (this.PageContext.IsGuest && !this.PageContext.BoardSettings.PollVoteTiedToIP)
        {
          return true;
        }

        object UserID = null;
        object RemoteIP = null;

        if (this.PageContext.BoardSettings.PollVoteTiedToIP)
        {
          RemoteIP = IPHelper.IPStrToLong(this.Request.UserHostAddress).ToString();
        }

        if (!this.PageContext.IsGuest)
        {
          UserID = this.PageContext.PageUserID;
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
    ///   Gets or sets CurrentMessage.
    /// </summary>
    protected int CurrentMessage
    {
      get
      {
        if (this.ViewState["CurrentMessage"] != null)
        {
          return (int)this.ViewState["CurrentMessage"];
        }
        else
        {
          return 0;
        }
      }

      set
      {
        this.ViewState["CurrentMessage"] = value;
      }
    }

    /// <summary>
    ///   Gets VotingCookieName.
    /// </summary>
    protected string VotingCookieName
    {
      get
      {
        return String.Format("poll#{0}", this._topic["PollID"]);
      }
    }

    #endregion

    #region Methods

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
      ((LinkButton)sender).Attributes["onclick"] = String.Format(
        "return confirm('{0}')", this.GetText("confirm_deletemessage"));
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
      if (!this.PageContext.ForumModeratorAccess)
      {
        YafBuildLink.AccessDenied( /*"You don't have access to delete topics."*/);
      }

      // Take away 10 points once!
      DB.user_removepointsByTopicID(this.PageContext.PageTopicID, 10);
      DB.topic_delete(this.PageContext.PageTopicID);
      YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
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
      ((ThemeButton)sender).Attributes["onclick"] = String.Format(
        "return confirm('{0}')", this.GetText("confirm_deletetopic"));
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
      if (this.User == null)
      {
        this.PageContext.AddLoadMessage(this.GetText("WARN_EMAILLOGIN"));
        return;
      }

      YafBuildLink.Redirect(ForumPages.emailtopic, "t={0}", this.PageContext.PageTopicID);
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
      if (!this.IsThreaded)
      {
        return string.Empty;
      }

      var iIndent = (int)o;
      if (iIndent > 0)
      {
        return string.Format(
          "<img src='{1}images/spacer.gif' width='{0}' alt='' height='2'/>", 
          iIndent * 32, 
          YafForumInfo.ForumClientFileRoot);
      }
      else
      {
        return string.Empty;
      }
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
      if (this.IsPollClosed())
      {
        strPollClosed = this.GetText("POLL_CLOSED");
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
      return this.HtmlEncode(YafServices.BadWordReplace.Replace(this._dtPoll.Rows[0]["Question"].ToString()));
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
      var row = (DataRow)o;
      if (!this.IsThreaded || this.CurrentMessage == (int)row["MessageID"])
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
      html.AppendFormat(this.GetIndentImage(row["Indent"]));
      html.AppendFormat(
        "\n<a href='{0}'>{2} ({1}", 
        YafBuildLink.GetLink(ForumPages.posts, "m={0}#{0}", row["MessageID"]), 
        row["UserName"], 
        brief);
      html.AppendFormat(" - {0})</a>", YafServices.DateTime.FormatDateTimeShort(row["Posted"]));
      html.AppendFormat("</td></tr>");

      return html.ToString();
    }

    /// <summary>
    /// The get total.
    /// </summary>
    /// <returns>
    /// The get total.
    /// </returns>
    protected string GetTotal()
    {
      return this.HtmlEncode(this._dtPoll.Rows[0]["Total"].ToString());
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
      var row = (DataRow)o;

      return !this.IsThreaded || this.CurrentMessage == (int)row["MessageID"];
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
        if (tCloses < DateTime.UtcNow)
        {
          bIsClosed = true;
        }
      }

      return bIsClosed;
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
      DB.topic_lock(this.PageContext.PageTopicID, true);
      this.BindData();
      this.PageContext.AddLoadMessage(this.GetText("INFO_TOPIC_LOCKED"));
      this.LockTopic1.Visible = !this.LockTopic1.Visible;
      this.UnlockTopic1.Visible = !this.UnlockTopic1.Visible;
      this.LockTopic2.Visible = this.LockTopic1.Visible;
      this.UnlockTopic2.Visible = this.UnlockTopic1.Visible;

      /*PostReplyLink1.Visible = false;
			PostReplyLink2.Visible = false;*/
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

        if (this.User != null)
        {
          showAds = this.PageContext.BoardSettings.ShowAdsToSignedInUsers;
        }

        if (!string.IsNullOrEmpty(this.PageContext.BoardSettings.AdPost) && showAds)
        {
          // first message... show the ad below this message
          var adControl = (DisplayAd)e.Item.FindControl("DisplayAd");
          if (adControl != null)
          {
            adControl.Visible = true;
          }
        }
      }
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
      if (!this.PageContext.ForumModeratorAccess)
      {
        YafBuildLink.AccessDenied( /*"You are not a forum moderator."*/);
      }

      YafBuildLink.Redirect(ForumPages.movetopic, "t={0}", this.PageContext.PageTopicID);
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
        this.PageContext.AddLoadMessage(this.GetText("WARN_FORUM_LOCKED"));
        return;
      }

      YafBuildLink.Redirect(ForumPages.postmessage, "f={0}", this.PageContext.PageForumID);
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
      using (DataTable dt = DB.topic_findnext(this.PageContext.PageTopicID))
      {
        if (dt.Rows.Count == 0)
        {
          this.PageContext.AddLoadMessage(this.GetText("INFO_NOMORETOPICS"));
          return;
        }

        YafBuildLink.Redirect(ForumPages.posts, "t={0}", dt.Rows[0]["TopicID"]);
      }
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      // Quick Reply Modification Begin
      this._quickReplyEditor = new BasicBBCodeEditor();
      this.QuickReplyLine.Controls.Add(this._quickReplyEditor);
      this.QuickReply.Click += this.QuickReply_Click;
      this.Pager.PageChange += this.Pager_PageChange;

      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      this.InitializeComponent();
      base.OnInit(e);
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
      if (!this.PageContext.IsGuest)
      {
        // Register Ajax Pro.
        Utility.RegisterTypeForAjax(typeof(YafFavoriteTopic));

        // The html code for "Favorite Topic" theme buttons.
        string tagButtonHTML =
          string.Format(
            "'<a class=\"yafcssbigbutton rightItem\" href=\"javascript:addFavoriteTopic(' + res.value + ');\" onclick=\"this.blur();\" title=\"{0}\"><span>{1}</span></a>'", 
            this.PageContext.Localization.GetText("BUTTON_TAGFAVORITE_TT"), 
            this.PageContext.Localization.GetText("BUTTON_TAGFAVORITE"));
        string untagButtonHTML =
          string.Format(
            "'<a class=\"yafcssbigbutton rightItem\" href=\"javascript:removeFavoriteTopic(' + res.value + ');\" onclick=\"this.blur();\" title=\"{0}\"><span>{1}</span></a>'", 
            this.PageContext.Localization.GetText("BUTTON_UNTAGFAVORITE_TT"), 
            this.PageContext.Localization.GetText("BUTTON_UNTAGFAVORITE"));

        // Register the client side script for the "Favorite Topic".
        YafContext.Current.PageElements.RegisterJsBlockStartup(
          "addFavoriteTopicJs", JavaScriptBlocks.addFavoriteTopicJs(untagButtonHTML));
        YafContext.Current.PageElements.RegisterJsBlockStartup(
          "removeFavoriteTopicJs", JavaScriptBlocks.removeFavoriteTopicJs(tagButtonHTML));
        YafContext.Current.PageElements.RegisterJsBlockStartup(
          "asynchCallFailedJs", JavaScriptBlocks.asynchCallFailedJs);

        // Has the user already tagged this topic as favorite?
        if (YafServices.FavoriteTopic.IsFavoriteTopic(this.PageContext.PageTopicID))
        {
          // Generate the "Untag" theme button with appropriate JS calls for onclick event.
          this.TagFavorite1.NavigateUrl = "javascript:removeFavoriteTopic(" + this.PageContext.PageTopicID + ");";
          this.TagFavorite2.NavigateUrl = "javascript:removeFavoriteTopic(" + this.PageContext.PageTopicID + ");";
          this.TagFavorite1.TextLocalizedTag = "BUTTON_UNTAGFAVORITE";
          this.TagFavorite1.TitleLocalizedTag = "BUTTON_UNTAGFAVORITE_TT";
          this.TagFavorite2.TextLocalizedTag = "BUTTON_UNTAGFAVORITE";
          this.TagFavorite2.TitleLocalizedTag = "BUTTON_UNTAGFAVORITE_TT";
        }
        else
        {
          // Generate the "Tag" theme button with appropriate JS calls for onclick event.
          this.TagFavorite1.NavigateUrl = "javascript:addFavoriteTopic(" + this.PageContext.PageTopicID + ");";
          this.TagFavorite2.NavigateUrl = "javascript:addFavoriteTopic(" + this.PageContext.PageTopicID + ");";
          this.TagFavorite1.TextLocalizedTag = "BUTTON_TAGFAVORITE";
          this.TagFavorite1.TitleLocalizedTag = "BUTTON_TAGFAVORITE_TT";
          this.TagFavorite2.TextLocalizedTag = "BUTTON_TAGFAVORITE";
          this.TagFavorite2.TitleLocalizedTag = "BUTTON_TAGFAVORITE_TT";
        }
      }
      else
      {
        this.TagFavorite1.Visible = false;
        this.TagFavorite2.Visible = false;
      }

      this._quickReplyEditor.BaseDir = YafForumInfo.ForumClientFileRoot + "editors";
      this._quickReplyEditor.StyleSheet = this.PageContext.Theme.BuildThemePath("theme.css");

      this._topic = DB.topic_info(this.PageContext.PageTopicID);

      // in case topic is deleted or not existant
      if (this._topic == null)
      {
        YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
      }

      // get topic flags
      this._topicFlags = new TopicFlags(this._topic["Flags"]);

      using (DataTable dt = DB.forum_list(this.PageContext.PageBoardID, this.PageContext.PageForumID))
      {
        this._forum = dt.Rows[0];
      }

      this._forumFlags = new ForumFlags(this._forum["Flags"]);

      if (!this.PageContext.ForumReadAccess)
      {
        YafBuildLink.AccessDenied();
      }

      if (!this.IsPostBack)
      {
        if (this.PageContext.Settings.LockedForum == 0)
        {
          this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
          this.PageLinks.AddLink(
            this.PageContext.PageCategoryName, 
            YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
        }

        this.QuickReply.Text = this.GetText("POSTMESSAGE", "SAVE");
        this.DataPanel1.TitleText = this.GetText("QUICKREPLY");
        this.DataPanel1.ExpandText = this.GetText("QUICKREPLY_SHOW");
        this.DataPanel1.CollapseText = this.GetText("QUICKREPLY_HIDE");

        this.PageLinks.AddForumLinks(this.PageContext.PageForumID);
        this.PageLinks.AddLink(
          YafServices.BadWordReplace.Replace(this.Server.HtmlDecode(this.PageContext.PageTopicName)), string.Empty);

        this.TopicTitle.Text = YafServices.BadWordReplace.Replace((string)this._topic["Topic"]);

        this.ViewOptions.Visible = this.PageContext.BoardSettings.AllowThreaded;
        this.ForumJumpHolder.Visible = this.PageContext.BoardSettings.ShowForumJump &&
                                       this.PageContext.Settings.LockedForum == 0;

        this.RssTopic.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
          ForumPages.rsstopic, "pg={0}&t={1}", this.Request.QueryString["g"], this.PageContext.PageTopicID);
        this.RssTopic.Visible = this.PageContext.BoardSettings.ShowRSSLink;

        this.QuickReplyPlaceHolder.Visible = this.PageContext.BoardSettings.ShowQuickAnswer;

        if ((this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableCaptchaForGuests) ||
            (this.PageContext.BoardSettings.EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded))
        {
          this.Session["CaptchaImageText"] = CaptchaHelper.GetCaptchaString();
          this.imgCaptcha.ImageUrl = String.Format("{0}resource.ashx?c=1", YafForumInfo.ForumClientFileRoot);
          this.CaptchaDiv.Visible = true;
        }

        if (!this.PageContext.ForumPostAccess || (this._forumFlags.IsLocked && !this.PageContext.ForumModeratorAccess))
        {
          this.NewTopic1.Visible = false;
          this.NewTopic2.Visible = false;
        }

        // Ederon : 9/9/2007 - moderators can reply in locked topics
        if (!this.PageContext.ForumReplyAccess ||
            ((this._topicFlags.IsLocked || this._forumFlags.IsLocked) && !this.PageContext.ForumModeratorAccess))
        {
          this.PostReplyLink1.Visible = this.PostReplyLink2.Visible = false;
          this.QuickReplyPlaceHolder.Visible = false;
        }

        if (this.PageContext.ForumModeratorAccess)
        {
          this.MoveTopic1.Visible = true;
          this.MoveTopic2.Visible = true;
        }
        else
        {
          this.MoveTopic1.Visible = false;
          this.MoveTopic2.Visible = false;
        }

        if (!this.PageContext.ForumModeratorAccess)
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
      Mession.SetTopicRead(this.PageContext.PageTopicID, DateTime.UtcNow);

      this.BindData();
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
      if (!this.PageContext.ForumModeratorAccess)
      {
        if (this._topicFlags.IsLocked)
        {
          this.PageContext.AddLoadMessage(this.GetText("WARN_TOPIC_LOCKED"));
          return;
        }

        if (this._forumFlags.IsLocked)
        {
          this.PageContext.AddLoadMessage(this.GetText("WARN_FORUM_LOCKED"));
          return;
        }
      }

      YafBuildLink.Redirect(
        ForumPages.postmessage, "t={0}&f={1}", this.PageContext.PageTopicID, this.PageContext.PageForumID);
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
      using (DataTable dt = DB.topic_findprev(this.PageContext.PageTopicID))
      {
        if (dt.Rows.Count == 0)
        {
          this.PageContext.AddLoadMessage(this.GetText("INFO_NOMORETOPICS"));
          return;
        }

        YafBuildLink.Redirect(ForumPages.posts, "t={0}", dt.Rows[0]["TopicID"]);
      }
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
      YafBuildLink.Redirect(ForumPages.printtopic, "t={0}", this.PageContext.PageTopicID);
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
      if (this.PageContext.IsGuest)
      {
        this.PageContext.AddLoadMessage(this.GetText("WARN_WATCHLOGIN"));
        return;
      }

      if (this.WatchTopicID.InnerText == string.Empty)
      {
        DB.watchtopic_add(this.PageContext.PageUserID, this.PageContext.PageTopicID);
        this.PageContext.AddLoadMessage(this.GetText("INFO_WATCH_TOPIC"));
      }
      else
      {
        int tmpID = Convert.ToInt32(this.WatchTopicID.InnerText);
        DB.watchtopic_delete(tmpID);
        this.PageContext.AddLoadMessage(this.GetText("INFO_UNWATCH_TOPIC"));
      }

      this.HandleWatchTopic();

      this.BindData();
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
      DB.topic_lock(this.PageContext.PageTopicID, false);
      this.BindData();
      this.PageContext.AddLoadMessage(this.GetText("INFO_TOPIC_UNLOCKED"));
      this.LockTopic1.Visible = !this.LockTopic1.Visible;
      this.UnlockTopic1.Visible = !this.UnlockTopic1.Visible;
      this.LockTopic2.Visible = this.LockTopic1.Visible;
      this.UnlockTopic2.Visible = this.UnlockTopic1.Visible;
      this.PostReplyLink1.Visible = this.PageContext.ForumReplyAccess;
      this.PostReplyLink2.Visible = this.PageContext.ForumReplyAccess;
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
      var row = (DataRow)o;
      return (int)row["Stats"] * 80 / 100;
    }

    /// <summary>
    /// Adds meta data: description and keywords to the page header.
    /// </summary>
    /// <param name="firstMessage">
    /// first message in the topic
    /// </param>
    private void AddMetaData(object firstMessage)
    {
      if (firstMessage.IsNullOrEmptyDBField())
      {
        return;
      }

      if (this.Page.Header != null && this.PageContext.BoardSettings.AddDynamicPageMetaTags)
      {
        FormatMsg.MessageCleaned message = FormatMsg.GetCleanedTopicMessage(firstMessage, this.PageContext.PageTopicID);
        var meta = this.Page.Header.FindControlType<HtmlMeta>();

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
            this.Page.Header.Controls.Add(descriptionMeta);
          }
        }

        if (message.MessageKeywords.Count > 0)
        {
          HtmlMeta keywordMeta = null;

          var keywordStr = message.MessageKeywords.Where(x => !String.IsNullOrEmpty(x)).ToList().ToDelimitedString(",");

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
            this.Page.Header.Controls.Add(keywordMeta);
          }
        }
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this._dataBound = true;

      this.Pager.PageSize = this.PageContext.BoardSettings.PostsPerPage;

      if (this._topic == null)
      {
        YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
      }

      DataTable postListDataTable = DB.post_list(
        this.PageContext.PageTopicID, 
        this.IsPostBack ? 0 : 1, 
        this.PageContext.BoardSettings.ShowDeletedMessages, 
        YafContext.Current.BoardSettings.UseStyledNicks, 
        YafContext.Current.BoardSettings.ShowThanksDate, 
        YafContext.Current.BoardSettings.EnableThanksMod);

      // get the message ids as a string list
      postListDataTable = this.UpdateDataTableWithThanksAndStyles(postListDataTable);

      // convert to linq...
      var rowList = postListDataTable.AsEnumerable();

      // see if the deleted messages need to be edited out...
      if (this.PageContext.BoardSettings.ShowDeletedMessages && !this.PageContext.BoardSettings.ShowDeletedMessagesToAll &&
          !this.PageContext.IsAdmin && !this.PageContext.IsForumModerator)
      {
        // remove posts that are deleted and do not belong to this user...
        rowList =
          rowList.Where(x => !(x.Field<bool>("IsDeleted") && x.Field<int>("UserID") != this.PageContext.PageUserID));
      }

      // set the sorting
      if (this.IsThreaded)
      {
        rowList = rowList.OrderBy(x => x.Field<int>("Position"));
      }
      else
      {
        rowList = rowList.OrderBy(x => x.Field<DateTime>("Posted"));

        // reset position for updated sorting...
        rowList.ForEachIndex(
          (row, i) =>
            {
              row.BeginEdit();
              row["Position"] = i;
              row.EndEdit();
            });
      }

      this.Pager.Count = rowList.Count();
      int findMessageId = this.GetFindMessageId();

      if (findMessageId > 0)
      {
        this.CurrentMessage = findMessageId;
        var selectedMessage = rowList.Where(row => row.Field<int>("MessageID") == findMessageId).FirstOrDefault();
        if (selectedMessage != null)
        {
          this.Pager.CurrentPageIndex =
            (int)Math.Floor((double)selectedMessage.Field<int>("Position") / this.Pager.PageSize);
        }
      }
      else
      {
        this.CurrentMessage = rowList.Last().Field<int>("MessageID");
      }

      if (postListDataTable.Rows.Count > 0)
      {
        // handle add description/keywords for SEO
        this.AddMetaData(postListDataTable.Rows[0]["Message"]);
      }

      var pagedData = rowList.Skip(this.Pager.SkipIndex).Take(this.Pager.PageSize);
      
      // dynamic load messages that are needed...
      YafServices.DBBroker.LoadMessageText(pagedData);

      this.MessageList.DataSource = pagedData;

      if (this._topic["PollID"] != DBNull.Value)
      {
        this.Poll.Visible = true;
        this._dtPoll = DB.poll_stats(this._topic["PollID"]);
        this.Poll.DataSource = this._dtPoll;
      }

      this.DataBind();
    }

    /// <summary>
    /// Gets the message ID if "find" is in the query string
    /// </summary>
    /// <param name="pds">
    /// </param>
    /// <returns>
    /// The get find message id.
    /// </returns>
    private int GetFindMessageId()
    {
      int findMessageId = 0;

      try
      {
        if (this._ignoreQueryString)
        {
        }
        else if (this.Request.QueryString["m"] != null)
        {
          // Show this message
          findMessageId = int.Parse(this.Request.QueryString["m"]);
        }
        else if (this.Request.QueryString["find"] != null && this.Request.QueryString["find"].ToLower() == "unread")
        {
          // Find next unread
          using (DataTable unread = DB.message_findunread(this.PageContext.PageTopicID, Mession.LastVisit))
          {
            var unreadFirst = unread.AsEnumerable().FirstOrDefault();

            if (unreadFirst != null)
            {
              findMessageId = unreadFirst.Field<int>("MessageID");
            }
          }
        }
      }
      catch (Exception x)
      {
        DB.eventlog_create(this.PageContext.PageUserID, this, x);
      }

      return findMessageId;
    }

    /// <summary>
    /// The handle watch topic.
    /// </summary>
    /// <returns>
    /// The handle watch topic.
    /// </returns>
    private bool HandleWatchTopic()
    {
      if (this.PageContext.IsGuest)
      {
        return false;
      }

      // check if this forum is being watched by this user
      using (DataTable dt = DB.watchtopic_check(this.PageContext.PageUserID, this.PageContext.PageTopicID))
      {
        if (dt.Rows.Count > 0)
        {
          // subscribed to this forum
          this.TrackTopic.Text = this.GetText("UNWATCHTOPIC");
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
          this.TrackTopic.Text = this.GetText("WATCHTOPIC");
        }
      }

      return false;
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    ///   the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.Poll.ItemCommand += this.Poll_ItemCommand;
      this.PreRender += this.posts_PreRender;
      this.OptionsMenu.ItemClick += this.OptionsMenu_ItemClick;
      this.ViewMenu.ItemClick += this.ViewMenu_ItemClick;
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
          YafBuildLink.Redirect(ForumPages.printtopic, "t={0}", this.PageContext.PageTopicID);
          break;
        case "watch":
          this.TrackTopic_Click(sender, e);
          break;
        case "email":
          this.EmailTopic_Click(sender, e);
          break;
        case "rssfeed":
          YafBuildLink.Redirect(
            ForumPages.rsstopic, "pg={0}&t={1}", this.Request.QueryString["g"], this.PageContext.PageTopicID);
          break;
        default:
          throw new ApplicationException(e.Item);
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
      this.BindData();
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
      if (e.CommandName == "vote" && this.PageContext.ForumVoteAccess)
      {
        if (!this.CanVote)
        {
          this.PageContext.AddLoadMessage(this.GetText("WARN_ALREADY_VOTED"));
          return;
        }

        if (this._topicFlags.IsLocked)
        {
          this.PageContext.AddLoadMessage(this.GetText("WARN_TOPIC_LOCKED"));
          return;
        }

        if (this.IsPollClosed())
        {
          this.PageContext.AddLoadMessage(this.GetText("WARN_POLL_CLOSED"));
          return;
        }

        object userID = null;
        object remoteIP = null;

        if (this.PageContext.BoardSettings.PollVoteTiedToIP)
        {
          remoteIP = IPHelper.IPStrToLong(this.Request.ServerVariables["REMOTE_ADDR"]).ToString();
        }

        if (!this.PageContext.IsGuest)
        {
          userID = this.PageContext.PageUserID;
        }

        DB.choice_vote(e.CommandArgument, userID, remoteIP);

        // save the voting cookie...
        var c = new HttpCookie(this.VotingCookieName, e.CommandArgument.ToString());
        c.Expires = DateTime.UtcNow.AddYears(1);
        this.Response.Cookies.Add(c);

        this.PageContext.AddLoadMessage(this.GetText("INFO_VOTED"));
        this.BindData();
      }
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
      if (!this.PageContext.ForumReplyAccess || (this._topicFlags.IsLocked && !this.PageContext.ForumModeratorAccess))
      {
        YafBuildLink.AccessDenied();
      }

      if (this._quickReplyEditor.Text.Length <= 0)
      {
        this.PageContext.AddLoadMessage(this.GetText("EMPTY_MESSAGE"));
        return;
      }

      if (((this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableCaptchaForGuests) ||
           (this.PageContext.BoardSettings.EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded)) &&
          this.Session["CaptchaImageText"].ToString() != this.tbCaptcha.Text.Trim())
      {
        this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"));
        return;
      }

      if (!(this.PageContext.IsAdmin || this.PageContext.IsModerator) &&
          this.PageContext.BoardSettings.PostFloodDelay > 0)
      {
        if (Mession.LastPost > DateTime.UtcNow.AddSeconds(-this.PageContext.BoardSettings.PostFloodDelay))
        {
          this.PageContext.AddLoadMessage(
            this.GetTextFormatted(
              "wait", 
              (Mession.LastPost - DateTime.UtcNow.AddSeconds(-this.PageContext.BoardSettings.PostFloodDelay)).Seconds));
          return;
        }
      }

      Mession.LastPost = DateTime.UtcNow;

      // post message...
      long nMessageID = 0;
      object replyTo = -1;
      string msg = this._quickReplyEditor.Text;
      long topicID = this.PageContext.PageTopicID;

      var tFlags = new MessageFlags();

      tFlags.IsHtml = this._quickReplyEditor.UsesHTML;
      tFlags.IsBBCode = this._quickReplyEditor.UsesBBCode;

      // Bypass Approval if Admin or Moderator.
      tFlags.IsApproved = this.PageContext.IsAdmin || this.PageContext.IsModerator;

      if (
        !DB.message_save(
          topicID, 
          this.PageContext.PageUserID, 
          msg, 
          null, 
          this.Request.UserHostAddress, 
          null, 
          replyTo, 
          tFlags.BitValue, 
          ref nMessageID))
      {
        topicID = 0;
      }

      // Check to see if the user has enabled "auto watch topic" option in his/her profile.
      var userData = new CombinedUserDataHelper(this.PageContext.PageUserID);
      if (userData.AutoWatchTopics)
      {
        using (DataTable dt = DB.watchtopic_check(this.PageContext.PageUserID, this.PageContext.PageTopicID))
        {
          if (dt.Rows.Count == 0)
          {
            // subscribe to this forum
            DB.watchtopic_add(this.PageContext.PageUserID, this.PageContext.PageTopicID);
          }
        }
      }

      bool bApproved = false;

      using (DataTable dt = DB.message_list(nMessageID))
      {
        foreach (DataRow row in dt.Rows)
        {
          bApproved = ((int)row["Flags"] & 16) == 16;
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
        string url = YafBuildLink.GetLink(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
        if (Config.IsRainbow)
        {
          YafBuildLink.Redirect(ForumPages.info, "i=1");
        }
        else
        {
          YafBuildLink.Redirect(ForumPages.info, "i=1&url={0}", this.Server.UrlEncode(url));
        }
      }
    }

    /// <summary>
    /// The update data table with thanks and styles.
    /// </summary>
    /// <param name="postListDataTable">
    /// The post list data table.
    /// </param>
    /// <returns>
    /// </returns>
    private DataTable UpdateDataTableWithThanksAndStyles(DataTable postListDataTable)
    {
      var messageIds = postListDataTable.AsEnumerable().Select(x => x.Field<int>("MessageID"));

      // Add nescessary columns for later use in displaypost.ascx (Prevent repetitive 
      // calls to database.)  
      if (this.PageContext.BoardSettings.EnableThanksMod)
      {
        postListDataTable.Columns.AddRange(
          new[]
            {
              // How many times has this message been thanked.
              new DataColumn("IsThankedByUser", Type.GetType("System.Boolean")), 
              // How many times has the message poster thanked others?   
              new DataColumn("MessageThanksNumber", Type.GetType("System.Int32")), 
              // How many times has the message poster been thanked?
              new DataColumn("ThanksFromUserNumber", Type.GetType("System.Int32")), 
              // In how many posts has the message poster been thanked? 
              new DataColumn("ThanksToUserNumber", Type.GetType("System.Int32")), 
              // In how many posts has the message poster been thanked? 
              new DataColumn("ThanksToUserPostsNumber", Type.GetType("System.Int32"))
            });

        // Initialize the "IsthankedByUser" column.
        foreach (DataRow dr in postListDataTable.Rows)
        {
          dr["IsThankedByUser"] = "false";
        }
      }

      // Make the "MessageID" Column the primary key to the datatable.
      postListDataTable.PrimaryKey = new[] { postListDataTable.Columns["MessageID"] };
      if (this.PageContext.BoardSettings.EnableThanksMod)
      {
        // Iterate through all the thanks relating to this topic and make appropriate
        // changes in columns.
        using (DataTable dt0AllThanks = DB.message_GetAllThanks(messageIds.ToDelimitedString(",")))
        {
          // get the default view...
          DataView dtAllThanks = dt0AllThanks.DefaultView;

          foreach (DataRow drThanks in dtAllThanks.Table.Rows)
          {
            if (drThanks["FromUserID"] != DBNull.Value)
            {
              if (Convert.ToInt32(drThanks["FromUserID"]) == this.PageContext.PageUserID)
              {
                postListDataTable.Rows.Find(drThanks["MessageID"])["IsThankedByUser"] = "true";
              }
            }
          }

          foreach (DataRow dataRow in postListDataTable.Rows)
          {
            dtAllThanks.RowFilter = String.Format(
              "MessageID = {0} AND FromUserID is not NULL", Convert.ToInt32(dataRow["MessageID"]));
            dataRow["MessageThanksNumber"] = dtAllThanks.Count;
            dtAllThanks.RowFilter = String.Format("MessageID = {0}", Convert.ToInt32(dataRow["MessageID"]));
            dataRow["ThanksFromUserNumber"] = dtAllThanks.Count > 0 ? dtAllThanks[0]["ThanksFromUserNumber"] : 0;
            dataRow["ThanksToUserNumber"] = dtAllThanks.Count > 0 ? dtAllThanks[0]["ThanksToUserNumber"] : 0;
            dataRow["ThanksToUserPostsNumber"] = dtAllThanks.Count > 0 ? dtAllThanks[0]["ThanksToUserPostsNumber"] : 0;
          }
        }
      }

      if (YafContext.Current.BoardSettings.UseStyledNicks)
      {
        new StyleTransform(this.PageContext.Theme).DecodeStyleByTable(ref postListDataTable, true);
      }

      return postListDataTable;
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
          this.IsThreaded = false;
          this.BindData();
          break;
        case "threaded":
          this.IsThreaded = true;
          this.BindData();
          break;
        default:
          throw new ApplicationException(e.Item);
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
      bool isWatched = this.HandleWatchTopic();

      // options menu...
      this.OptionsMenu.AddPostBackItem("watch", isWatched ? this.GetText("UNWATCHTOPIC") : this.GetText("WATCHTOPIC"));
      if (this.PageContext.BoardSettings.AllowEmailTopic)
      {
        this.OptionsMenu.AddPostBackItem("email", this.GetText("EMAILTOPIC"));
      }

      this.OptionsMenu.AddPostBackItem("print", this.GetText("PRINTTOPIC"));
      if (this.PageContext.BoardSettings.ShowRSSLink)
      {
        this.OptionsMenu.AddPostBackItem("rssfeed", this.GetText("RSSTOPIC"));
      }

      // view menu
      this.ViewMenu.AddPostBackItem("normal", this.GetText("NORMAL"));
      this.ViewMenu.AddPostBackItem("threaded", this.GetText("THREADED"));

      // attach both the menus to HyperLinks
      this.OptionsMenu.Attach(this.OptionsLink);
      this.ViewMenu.Attach(this.ViewLink);

      if (!this._dataBound)
      {
        this.BindData();
      }
    }

    #endregion
  }
}