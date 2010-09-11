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

    using AjaxPro;

  using System;
  using System.Data;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  

  using Classes;
  using Classes.Core;
  using Classes.Data;
  using Classes.Extensions;
  using Classes.Utils;
  using Controls;
  using Editors;
  using Utilities;

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
        if (Request.QueryString.GetFirstOrDefault("threaded") != null)
        {
          Session["IsThreaded"] = bool.Parse(Request.QueryString.GetFirstOrDefault("threaded"));
        }
        else if (Session["IsThreaded"] == null)
        {
          Session["IsThreaded"] = false;
        }

        return (bool)Session["IsThreaded"];
      }

      set
      {
        Session["IsThreaded"] = value;
      }
    }


    /// <summary>
    ///   Gets or sets CurrentMessage.
    /// </summary>
    protected int CurrentMessage
    {
        get
        {
            if (ViewState["CurrentMessage"] != null)
            {
                return (int)ViewState["CurrentMessage"];
            }

            return 0;
        }

        set
        {
            ViewState["CurrentMessage"] = value;
        }
    }

    /// <summary>
    ///   Gets VotingCookieName.
    /// </summary>
    protected string VotingCookieName
    {
      get
      {
        return "poll#{0}".FormatWith(this._topic["PollID"]);
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
      ((LinkButton)sender).Attributes["onclick"] = "return confirm('{0}')".FormatWith(this.GetText("confirm_deletemessage"));
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
      ((ThemeButton)sender).Attributes["onclick"] = "return confirm('{0}')".FormatWith(this.GetText("confirm_deletetopic"));
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
            return "<img src='{1}images/spacer.gif' width='{0}' alt='' height='2'/>".FormatWith(iIndent*32, YafForumInfo.ForumClientFileRoot);
        }

        return string.Empty;
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
      if (!IsThreaded || CurrentMessage == (int)row["MessageID"])
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

      brief = YafFormatMessage.AddSmiles(brief);

      html.AppendFormat("<tr class='post'><td colspan='3' nowrap>");
      html.AppendFormat(GetIndentImage(row["Indent"]));
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

      return !IsThreaded || CurrentMessage == (int)row["MessageID"];
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
      LockTopic1.Visible = !LockTopic1.Visible;
      UnlockTopic1.Visible = !UnlockTopic1.Visible;
      LockTopic2.Visible = LockTopic1.Visible;
      UnlockTopic2.Visible = UnlockTopic1.Visible;

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
      if (Pager.CurrentPageIndex == 0 && e.Item.ItemIndex == 0)
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
      if (!PageContext.ForumModeratorAccess)
      {
        YafBuildLink.AccessDenied( /*"You are not a forum moderator."*/);
      }

      YafBuildLink.Redirect(ForumPages.movetopic, "t={0}", PageContext.PageTopicID);
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
      if (_forumFlags.IsLocked)
      {
        PageContext.AddLoadMessage(GetText("WARN_FORUM_LOCKED"));
        return;
      }

      YafBuildLink.Redirect(ForumPages.postmessage, "f={0}", PageContext.PageForumID);
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
      QuickReply.Click += QuickReply_Click;
      Pager.PageChange += Pager_PageChange;

      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      InitializeComponent();
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
      if (!PageContext.IsGuest)
      {
        // Register Ajax Pro.
        Utility.RegisterTypeForAjax(typeof(YafFavoriteTopic));

        // The html code for "Favorite Topic" theme buttons.
        string tagButtonHTML =
          "'<a class=\"yafcssbigbutton rightItem\" href=\"javascript:addFavoriteTopic(' + res.value + ');\" onclick=\"blur();\" title=\"{0}\"><span>{1}</span></a>'".FormatWith(PageContext.Localization.GetText("BUTTON_TAGFAVORITE_TT"), PageContext.Localization.GetText("BUTTON_TAGFAVORITE"));
        string untagButtonHTML =
          "'<a class=\"yafcssbigbutton rightItem\" href=\"javascript:removeFavoriteTopic(' + res.value + ');\" onclick=\"blur();\" title=\"{0}\"><span>{1}</span></a>'".FormatWith(PageContext.Localization.GetText("BUTTON_UNTAGFAVORITE_TT"), PageContext.Localization.GetText("BUTTON_UNTAGFAVORITE"));

        // Register the client side script for the "Favorite Topic".
        YafContext.Current.PageElements.RegisterJsBlockStartup(
          "addFavoriteTopicJs", JavaScriptBlocks.addFavoriteTopicJs(untagButtonHTML));
        YafContext.Current.PageElements.RegisterJsBlockStartup(
          "removeFavoriteTopicJs", JavaScriptBlocks.removeFavoriteTopicJs(tagButtonHTML));
        YafContext.Current.PageElements.RegisterJsBlockStartup(
          "asynchCallFailedJs", JavaScriptBlocks.asynchCallFailedJs);

        // Has the user already tagged this topic as favorite?
        if (YafServices.FavoriteTopic.IsFavoriteTopic(PageContext.PageTopicID))
        {
          // Generate the "Untag" theme button with appropriate JS calls for onclick event.
          TagFavorite1.NavigateUrl = "javascript:removeFavoriteTopic(" + PageContext.PageTopicID + ");";
          TagFavorite2.NavigateUrl = "javascript:removeFavoriteTopic(" + PageContext.PageTopicID + ");";
          TagFavorite1.TextLocalizedTag = "BUTTON_UNTAGFAVORITE";
          TagFavorite1.TitleLocalizedTag = "BUTTON_UNTAGFAVORITE_TT";
          TagFavorite2.TextLocalizedTag = "BUTTON_UNTAGFAVORITE";
          TagFavorite2.TitleLocalizedTag = "BUTTON_UNTAGFAVORITE_TT";
        }
        else
        {
          // Generate the "Tag" theme button with appropriate JS calls for onclick event.
          TagFavorite1.NavigateUrl = "javascript:addFavoriteTopic(" + PageContext.PageTopicID + ");";
          TagFavorite2.NavigateUrl = "javascript:addFavoriteTopic(" + PageContext.PageTopicID + ");";
          TagFavorite1.TextLocalizedTag = "BUTTON_TAGFAVORITE";
          TagFavorite1.TitleLocalizedTag = "BUTTON_TAGFAVORITE_TT";
          TagFavorite2.TextLocalizedTag = "BUTTON_TAGFAVORITE";
          TagFavorite2.TitleLocalizedTag = "BUTTON_TAGFAVORITE_TT";
        }
      }
      else
      {
        TagFavorite1.Visible = false;
        TagFavorite2.Visible = false;
      }

      _quickReplyEditor.BaseDir = YafForumInfo.ForumClientFileRoot + "editors";
      _quickReplyEditor.StyleSheet = PageContext.Theme.BuildThemePath("theme.css");

      _topic = DB.topic_info(PageContext.PageTopicID);

      // in case topic is deleted or not existant
      if (_topic == null)
      {
        YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
      }

      // get topic flags
      _topicFlags = new TopicFlags(_topic["Flags"]);

      using (DataTable dt = DB.forum_list(PageContext.PageBoardID, PageContext.PageForumID))
      {
        _forum = dt.Rows[0];
      }

      _forumFlags = new ForumFlags(_forum["Flags"]);

      if (PageContext.IsGuest && !PageContext.ForumReadAccess)
      {
        // attempt to get permission by redirecting to login...
        YafServices.Permissions.HandleRequest(ViewPermissions.RegisteredUsers);
      }
      else if (!PageContext.ForumReadAccess)
      {
        YafBuildLink.AccessDenied();
      }

      if (!IsPostBack)
      {
        if (PageContext.Settings.LockedForum == 0)
        {
          PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
          PageLinks.AddLink(
            PageContext.PageCategoryName, 
            YafBuildLink.GetLink(ForumPages.forum, "c={0}", PageContext.PageCategoryID));
        }

        QuickReply.Text = GetText("POSTMESSAGE", "SAVE");
        DataPanel1.TitleText = GetText("QUICKREPLY");
        DataPanel1.ExpandText = GetText("QUICKREPLY_SHOW");
        DataPanel1.CollapseText = GetText("QUICKREPLY_HIDE");

        PageLinks.AddForumLinks(PageContext.PageForumID);
        PageLinks.AddLink(
          YafServices.BadWordReplace.Replace(Server.HtmlDecode(PageContext.PageTopicName)), string.Empty);

        TopicTitle.Text = HtmlEncode(YafServices.BadWordReplace.Replace((string)_topic["Topic"]));

        ViewOptions.Visible = PageContext.BoardSettings.AllowThreaded;
        ForumJumpHolder.Visible = PageContext.BoardSettings.ShowForumJump &&
                                       PageContext.Settings.LockedForum == 0;

        RssTopic.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
          ForumPages.rsstopic, "pg={0}&t={1}", Request.QueryString.GetFirstOrDefault("g"), PageContext.PageTopicID);
        RssTopic.Visible = PageContext.BoardSettings.ShowRSSLink;

        QuickReplyPlaceHolder.Visible = PageContext.BoardSettings.ShowQuickAnswer;

        if ((PageContext.IsGuest && PageContext.BoardSettings.EnableCaptchaForGuests) ||
            (PageContext.BoardSettings.EnableCaptchaForPost && !PageContext.IsCaptchaExcluded))
        {
          imgCaptcha.ImageUrl = "{0}resource.ashx?c=1".FormatWith(YafForumInfo.ForumClientFileRoot);
          CaptchaDiv.Visible = true;
        }

        if (!PageContext.ForumPostAccess || (_forumFlags.IsLocked && !PageContext.ForumModeratorAccess))
        {
          NewTopic1.Visible = false;
          NewTopic2.Visible = false;
        }

        // Ederon : 9/9/2007 - moderators can reply in locked topics
        if (!PageContext.ForumReplyAccess ||
            ((_topicFlags.IsLocked || _forumFlags.IsLocked) && !PageContext.ForumModeratorAccess))
        {
          PostReplyLink1.Visible = PostReplyLink2.Visible = false;
          QuickReplyPlaceHolder.Visible = false;
        }

        if (PageContext.ForumModeratorAccess)
        {
          MoveTopic1.Visible = true;
          MoveTopic2.Visible = true;
        }
        else
        {
          MoveTopic1.Visible = false;
          MoveTopic2.Visible = false;
        }

        if (!PageContext.ForumModeratorAccess)
        {
          LockTopic1.Visible = false;
          UnlockTopic1.Visible = false;
          DeleteTopic1.Visible = false;
          LockTopic2.Visible = false;
          UnlockTopic2.Visible = false;
          DeleteTopic2.Visible = false;
        }
        else
        {
          LockTopic1.Visible = !_topicFlags.IsLocked;
          UnlockTopic1.Visible = !LockTopic1.Visible;
          LockTopic2.Visible = LockTopic1.Visible;
          UnlockTopic2.Visible = !LockTopic2.Visible;
        }
      }

      // Mark topic read
      Mession.SetTopicRead(PageContext.PageTopicID, DateTime.UtcNow);

      BindData();
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
        if (_topicFlags.IsLocked)
        {
          PageContext.AddLoadMessage(GetText("WARN_TOPIC_LOCKED"));
          return;
        }

        if (_forumFlags.IsLocked)
        {
          PageContext.AddLoadMessage(GetText("WARN_FORUM_LOCKED"));
          return;
        }
      }

      YafBuildLink.Redirect(
        ForumPages.postmessage, "t={0}&f={1}", PageContext.PageTopicID, PageContext.PageForumID);
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

      if (WatchTopicID.InnerText == string.Empty)
      {
        DB.watchtopic_add(PageContext.PageUserID, PageContext.PageTopicID);
        PageContext.AddLoadMessage(GetText("INFO_WATCH_TOPIC"));
      }
      else
      {
        int tmpID = Convert.ToInt32(WatchTopicID.InnerText);
        DB.watchtopic_delete(tmpID);
        PageContext.AddLoadMessage(GetText("INFO_UNWATCH_TOPIC"));
      }

      HandleWatchTopic();

      BindData();
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
      LockTopic1.Visible = !LockTopic1.Visible;
      UnlockTopic1.Visible = !UnlockTopic1.Visible;
      LockTopic2.Visible = LockTopic1.Visible;
      UnlockTopic2.Visible = UnlockTopic1.Visible;
      PostReplyLink1.Visible = PageContext.ForumReplyAccess;
      PostReplyLink2.Visible = PageContext.ForumReplyAccess;
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
      var row = (DataRowView)o;
      return (int)row.Row["Stats"]* 80 / 100;
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

        if (Page.Header != null && PageContext.BoardSettings.AddDynamicPageMetaTags)
        {
            YafFormatMessage.MessageCleaned message = YafFormatMessage.GetCleanedTopicMessage(firstMessage, PageContext.PageTopicID);
            var meta = Page.Header.FindControlType<HtmlMeta>();

            if (message.MessageTruncated.IsSet())
            {
                HtmlMeta descriptionMeta;

                string content = "{0}: {1}".FormatWith(this._topic["Topic"], message.MessageTruncated);

                if (meta.Exists(x => x.Name.Equals("description")))
                {
                    // use existing...
                    descriptionMeta = meta.Where(x => x.Name.Equals("description")).FirstOrDefault();
                    if (descriptionMeta != null)
                    {
                        descriptionMeta.Content = content;

                        Page.Header.Controls.Remove(descriptionMeta);

                        descriptionMeta = ControlHelper.MakeMetaDiscriptionControl(content);
                        // add to the header...
                        Page.Header.Controls.Add(descriptionMeta);
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
                HtmlMeta keywordMeta;

                var keywordStr = message.MessageKeywords.Where(x => x.IsSet()).ToList().ToDelimitedString(",");

                if (meta.Exists(x => x.Name.Equals("keywords")))
                {
                    // use existing...
                    keywordMeta = meta.Where(x => x.Name.Equals("keywords")).FirstOrDefault();
                    keywordMeta.Content = keywordStr;

                    Page.Header.Controls.Remove(keywordMeta);
                    // add to the header...
                    Page.Header.Controls.Add(keywordMeta);
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
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      _dataBound = true;

      Pager.PageSize = PageContext.BoardSettings.PostsPerPage;

      if (_topic == null)
      {
        YafBuildLink.Redirect(ForumPages.topics, "f={0}", PageContext.PageForumID);
      }

      DataTable postListDataTable = DB.post_list(
        PageContext.PageTopicID, 
        IsPostBack ? 0 : 1, 
        PageContext.BoardSettings.ShowDeletedMessages, 
        YafContext.Current.BoardSettings.UseStyledNicks);

      if (YafContext.Current.BoardSettings.EnableThanksMod)
      {
        // Add nescessary columns for later use in displaypost.ascx (Prevent repetitive 
        // calls to database.)  
        if (!postListDataTable.Columns.Contains("ThanksInfo"))
        {
          postListDataTable.Columns.Add("ThanksInfo", Type.GetType("System.String"));
        }

        postListDataTable.Columns.AddRange(
          new[]
            {
              // General Thanks Info
              // new DataColumn("ThanksInfo", Type.GetType("System.String")),
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

        postListDataTable.AcceptChanges();
      }

      if (YafContext.Current.BoardSettings.UseStyledNicks)
      {
        // needs to be moved to the paged data below -- so it doesn't operate on unnecessary rows
        new StyleTransform(PageContext.Theme).DecodeStyleByTable(ref postListDataTable, true);
      }

      // convert to linq...
      var rowList = postListDataTable.AsEnumerable();

      // see if the deleted messages need to be edited out...
      if (PageContext.BoardSettings.ShowDeletedMessages && !PageContext.BoardSettings.ShowDeletedMessagesToAll &&
          !PageContext.IsAdmin && !PageContext.IsForumModerator)
      {
        // remove posts that are deleted and do not belong to this user...
        rowList =
          rowList.Where(x => !(x.Field<bool>("IsDeleted") && x.Field<int>("UserID") != PageContext.PageUserID));
      }

      // set the sorting
      if (IsThreaded)
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

      Pager.Count = rowList.Count();
      int findMessageId = GetFindMessageId();

      if (findMessageId > 0)
      {
        CurrentMessage = findMessageId;
        var selectedMessage = rowList.Where(row => row.Field<int>("MessageID") == findMessageId).FirstOrDefault();
        if (selectedMessage != null)
        {
          Pager.CurrentPageIndex =
            (int)Math.Floor((double)selectedMessage.Field<int>("Position") / Pager.PageSize);
        }
      }
      else
      {
        CurrentMessage = rowList.Last().Field<int>("MessageID");
      }

      var pagedData = rowList.Skip(Pager.SkipIndex).Take(Pager.PageSize);

      // Add thanks info and styled nicks if they are enabled
      if (YafContext.Current.BoardSettings.EnableThanksMod)
      {
        YafServices.DBBroker.AddThanksInfo(pagedData);
      }
      
      // dynamic load messages that are needed...
      YafServices.DBBroker.LoadMessageText(pagedData);

      if (pagedData.Any())
      {
          // handle add description/keywords for SEO
          AddMetaData(pagedData.First()["Message"]);
      }

        MessageList.DataSource = pagedData;

     /* if (_topic["PollID"] != DBNull.Value)
      {
        Poll.Visible = true;
        _dtPoll = DB.poll_stats(_topic["PollID"]);
        Poll.DataSource = _dtPoll;
      } */

      DataBind();
    }

      protected int PollGroupId()
      {
          return !_topic["PollID"].IsNullOrEmptyDBField() ? Convert.ToInt32(_topic["PollID"]) : 0 ;
      }
 

      protected bool ShowPollButtons()
      {
          return false;
         /* return (Convert.ToInt32(_topic["UserID"]) == PageContext.PageUserID) || PageContext.IsModerator ||
                 PageContext.IsAdmin; */
      }

      /// <summary>
      /// Gets the message ID if "find" is in the query string
      /// </summary>
      /// <returns>
      /// The get find message id.
      /// </returns>
      private int GetFindMessageId()
    {
      int findMessageId = 0;

      try
      {
        if (_ignoreQueryString)
        {
        }
        else if (Request.QueryString.GetFirstOrDefault("m") != null)
        {
          // Show this message
          findMessageId = int.Parse(Request.QueryString.GetFirstOrDefault("m"));
        }
        else if (Request.QueryString.GetFirstOrDefault("find") != null && Request.QueryString.GetFirstOrDefault("find").ToLower() == "unread")
        {
          // Find next unread
          using (DataTable unread = DB.message_findunread(PageContext.PageTopicID, Mession.LastVisit))
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
        DB.eventlog_create(PageContext.PageUserID, this, x);
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
          TrackTopic.Text = GetText("UNWATCHTOPIC");
          foreach (DataRow row in dt.Rows)
          {
            WatchTopicID.InnerText = row["WatchTopicID"].ToString();
            return true;
          }
        }
        else
        {
          // not subscribed
          WatchTopicID.InnerText = string.Empty;
          TrackTopic.Text = GetText("WATCHTOPIC");
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
    //  Poll.ItemCommand += Poll_ItemCommand;
      PreRender += posts_PreRender;
      OptionsMenu.ItemClick += OptionsMenu_ItemClick;
      ViewMenu.ItemClick += ViewMenu_ItemClick;
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
          YafBuildLink.Redirect(
            ForumPages.rsstopic, "pg={0}&t={1}", Request.QueryString.GetFirstOrDefault("g"), PageContext.PageTopicID);
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
      _ignoreQueryString = true;
      SmartScroller1.Reset();
      BindData();
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
      if (!PageContext.ForumReplyAccess || (_topicFlags.IsLocked && !PageContext.ForumModeratorAccess))
      {
        YafBuildLink.AccessDenied();
      }

      if (_quickReplyEditor.Text.Length <= 0)
      {
        PageContext.AddLoadMessage(GetText("EMPTY_MESSAGE"));
        return;
      }

      if (((PageContext.IsGuest && PageContext.BoardSettings.EnableCaptchaForGuests) ||
           (PageContext.BoardSettings.EnableCaptchaForPost && !PageContext.IsCaptchaExcluded)) &&
          String.Compare(Session["CaptchaImageText"].ToString(), tbCaptcha.Text.Trim(), StringComparison.InvariantCultureIgnoreCase) != 0)
      {
        PageContext.AddLoadMessage(GetText("BAD_CAPTCHA"));
        return;
      }

      if (!(PageContext.IsAdmin || PageContext.IsModerator) &&
          PageContext.BoardSettings.PostFloodDelay > 0)
      {
        if (Mession.LastPost > DateTime.UtcNow.AddSeconds(-PageContext.BoardSettings.PostFloodDelay))
        {
          PageContext.AddLoadMessage(
            GetTextFormatted(
              "wait", 
              (Mession.LastPost - DateTime.UtcNow.AddSeconds(-PageContext.BoardSettings.PostFloodDelay)).Seconds));
          return;
        }
      }

      Mession.LastPost = DateTime.UtcNow;

      // post message...
      long nMessageId = 0;
      object replyTo = -1;
      string msg = _quickReplyEditor.Text;
      long topicID = PageContext.PageTopicID;

      var tFlags = new MessageFlags
                       {
                           IsHtml = _quickReplyEditor.UsesHTML,
                           IsBBCode = _quickReplyEditor.UsesBBCode,
                           IsApproved = PageContext.IsAdmin || PageContext.IsModerator
                       };

        // Bypass Approval if Admin or Moderator.

        if (
        !DB.message_save(
          topicID, 
          PageContext.PageUserID, 
          msg, 
          null, 
          Request.UserHostAddress, 
          null, 
          replyTo, 
          tFlags.BitValue, 
          ref nMessageId))
      {
        topicID = 0;
      }

      // Check to see if the user has enabled "auto watch topic" option in his/her profile.
      if (PageContext.CurrentUserData.AutoWatchTopics)
      {
        using (DataTable dt = DB.watchtopic_check(PageContext.PageUserID, PageContext.PageTopicID))
        {
          if (dt.Rows.Count == 0)
          {
            // subscribe to this forum
            DB.watchtopic_add(PageContext.PageUserID, PageContext.PageTopicID);
          }
        }
      }

      bool bApproved = false;

      using (DataTable dt = DB.message_list(nMessageId))
      {
        foreach (DataRow row in dt.Rows)
        {
          bApproved = ((int)row["Flags"] & 16) == 16;
        }
      }

      if (bApproved)
      {
        // send new post notification to users watching this topic/forum
        YafServices.SendNotification.ToWatchingUsers(nMessageId);

        // redirect to newly posted message
        YafBuildLink.Redirect(ForumPages.posts, "m={0}&#post{0}", nMessageId);
      }
      else
      {
        if (PageContext.BoardSettings.EmailModeratorsOnModeratedPost)
        {
          // not approved, notifiy moderators
          YafServices.SendNotification.ToModeratorsThatMessageNeedsApproval(PageContext.PageForumID, (int)nMessageId);
        }

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
      OptionsMenu.AddPostBackItem("watch", isWatched ? GetText("UNWATCHTOPIC") : GetText("WATCHTOPIC"));
      if (PageContext.BoardSettings.AllowEmailTopic)
      {
        OptionsMenu.AddPostBackItem("email", GetText("EMAILTOPIC"));
      }

      OptionsMenu.AddPostBackItem("print", GetText("PRINTTOPIC"));
      if (PageContext.BoardSettings.ShowRSSLink)
      {
        OptionsMenu.AddPostBackItem("rssfeed", GetText("RSSTOPIC"));
      }

      // view menu
      ViewMenu.AddPostBackItem("normal", GetText("NORMAL"));
      ViewMenu.AddPostBackItem("threaded", GetText("THREADED"));

      // attach both the menus to HyperLinks
      OptionsMenu.Attach(OptionsLink);
      ViewMenu.Attach(ViewLink);

      if (!_dataBound)
      {
        BindData();
      }
    }

    #endregion
  }
}