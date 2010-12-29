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
  using System.Web;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Controls;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Editors;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Flags;
  using YAF.Types.Interfaces;
  using YAF.Utilities;
  using YAF.Utils;
  using YAF.Utils.Helpers;

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
    protected ForumEditor _quickReplyEditor;

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
        if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("threaded") != null)
        {
          this.Session["IsThreaded"] = bool.Parse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("threaded"));
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

        return 0;
      }

      set
      {
        this.ViewState["CurrentMessage"] = value;
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
    protected void DeleteMessage_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ((LinkButton)sender).Attributes["onclick"] =
        "return confirm('{0}')".FormatWith(this.GetText("confirm_deletemessage"));
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
    protected void DeleteTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
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
    protected void DeleteTopic_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ((ThemeButton)sender).Attributes["onclick"] =
        "return confirm('{0}')".FormatWith(this.GetText("confirm_deletetopic"));
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
    protected void EmailTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
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
    protected string GetIndentImage([NotNull] object o)
    {
      if (!this.IsThreaded)
      {
        return string.Empty;
      }

      var currentIndex = (int)o;
      if (currentIndex > 0)
      {
        return "<img src='{1}images/spacer.gif' width='{0}' alt='' height='2'/>".FormatWith(
          currentIndex * 32, YafForumInfo.ForumClientFileRoot);
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
    [NotNull]
    protected string GetThreadedRow([NotNull] object o)
    {
      var row = (DataRow)o;
      var messageId = (int)row["MessageID"];

      if (!this.IsThreaded || this.CurrentMessage == messageId)
      {
        return string.Empty;
      }

      var html = new StringBuilder();

      // Threaded
      string brief =
        StringExtensions.RemoveMultipleWhitespace(
          BBCodeHelper.StripBBCode(HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(row["Message"].ToString()))));

      brief = StringExtensions.Truncate(this.Get<IBadWordReplace>().Replace(brief), 100);
      brief = YafFormatMessage.AddSmiles(brief);

      if (brief.IsNotSet())
      {
        brief = "...";
      }

      html.AppendFormat(@"<tr class=""post""><td colspan=""3"" style=""white-space:nowrap;"">");
      html.AppendFormat(this.GetIndentImage(row["Indent"]));

      string avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(row.Field<int>("UserID"));

      if (avatarUrl.IsNotSet())
      {
        avatarUrl = "{0}images/noavatar.gif".FormatWith(YafForumInfo.ForumClientFileRoot);
      }

      html.Append(@"<span class=""threadedRowCollapsed"">");
      html.AppendFormat(@"<img src=""{0}"" alt="""" class=""avatarimage"" />", avatarUrl);
      html.AppendFormat(
        @"<a href=""{0}"" class=""threadUrl"">{1}</a>", 
        YafBuildLink.GetLink(ForumPages.posts, "m={0}#post{0}", messageId), 
        brief);

      html.Append(" (");
        html.Append(
            new UserLink { ID = "UserLinkForRow{0}".FormatWith(messageId), UserID = row.Field<int>("UserID") }.
                RenderToString());

        html.AppendFormat(
            " - {0})</span>",
            new DisplayDateTime { DateTime = row["Posted"], Format = DateTimeFormat.BothTopic }.RenderToString());

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
    protected bool IsCurrentMessage([NotNull] object o)
    {
      CodeContracts.ArgumentNotNull(o, "o");

      var row = (DataRow)o;

      return !this.IsThreaded || this.CurrentMessage == (int)row["MessageID"];
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
    protected void LockTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
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
    protected void MessageList_OnItemCreated([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
    {
        if (this.Pager.CurrentPageIndex != 0 || e.Item.ItemIndex != 0)
        {
            return;
        }

        // check if need to display the ad...
        bool showAds = true;

        if (this.User != null)
        {
            showAds = this.PageContext.BoardSettings.ShowAdsToSignedInUsers;
        }

        if (string.IsNullOrEmpty(this.PageContext.BoardSettings.AdPost) || !showAds)
        {
            return;
        }

        // first message... show the ad below this message
        var adControl = (DisplayAd)e.Item.FindControl("DisplayAd");
        if (adControl != null)
        {
            adControl.Visible = true;
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
    protected void MoveTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
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
    protected void NewTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
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
    protected void NextTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
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
    protected override void OnInit([NotNull] EventArgs e)
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
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      base.OnPreRender(e);
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
      if (!this.PageContext.IsGuest)
      {
        YafContext.Current.PageElements.RegisterJsResourceInclude("yafPageMethodjs", "js/jquery.pagemethod.js");

        // The html code for "Favorite Topic" theme buttons.
        string tagButtonHTML =
          "'<a class=\"yafcssbigbutton rightItem\" href=\"javascript:addFavoriteTopic(' + res.d + ');\" onclick=\"jQuery(this).blur();\" title=\"{0}\"><span>{1}</span></a>'"
            .FormatWith(
              this.PageContext.Localization.GetText("BUTTON_TAGFAVORITE_TT"), 
              this.PageContext.Localization.GetText("BUTTON_TAGFAVORITE"));
        string untagButtonHTML =
          "'<a class=\"yafcssbigbutton rightItem\" href=\"javascript:removeFavoriteTopic(' + res.d + ');\" onclick=\"jQuery(this).blur();\" title=\"{0}\"><span>{1}</span></a>'"
            .FormatWith(
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
        if (this.Get<IFavoriteTopic>().IsFavoriteTopic(this.PageContext.PageTopicID))
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

      if (this.PageContext.IsGuest && !this.PageContext.ForumReadAccess)
      {
        // attempt to get permission by redirecting to login...
        this.Get<IPermissions>().HandleRequest(ViewPermissions.RegisteredUsers);
      }
      else if (!this.PageContext.ForumReadAccess)
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
          this.Get<IBadWordReplace>().Replace(this.Server.HtmlDecode(this.PageContext.PageTopicName)), string.Empty);

        this.TopicTitle.Text = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this._topic["Topic"]));

        this.ViewOptions.Visible = this.PageContext.BoardSettings.AllowThreaded;
        this.ForumJumpHolder.Visible = this.PageContext.BoardSettings.ShowForumJump &&
                                       this.PageContext.Settings.LockedForum == 0;

        this.RssTopic.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
          ForumPages.rsstopic, 
          "pg={0}&t={1}", 
          this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("g"), 
          this.PageContext.PageTopicID);
        this.RssTopic.Visible = this.PageContext.BoardSettings.ShowRSSLink;

        this.QuickReplyPlaceHolder.Visible = this.PageContext.BoardSettings.ShowQuickAnswer;

        if ((this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableCaptchaForGuests) ||
            (this.PageContext.BoardSettings.EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded))
        {
          this.imgCaptcha.ImageUrl = "{0}resource.ashx?c=1".FormatWith(YafForumInfo.ForumClientFileRoot);
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
      YafContext.Current.Get<IYafSession>().SetTopicRead(this.PageContext.PageTopicID, DateTime.UtcNow);

      this.BindData();
    }

    /// <summary>
    /// The poll group id.
    /// </summary>
    /// <returns>
    /// The poll group id.
    /// </returns>
    protected int PollGroupId()
    {
      return !this._topic["PollID"].IsNullOrEmptyDBField() ? Convert.ToInt32(this._topic["PollID"]) : 0;
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
    protected void PostReplyLink_Click([NotNull] object sender, [NotNull] EventArgs e)
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
    protected void PrevTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
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
    protected void PrintTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.printtopic, "t={0}", this.PageContext.PageTopicID);
    }

    /// <summary>
    /// The show poll buttons.
    /// </summary>
    /// <returns>
    /// The show poll buttons.
    /// </returns>
    protected bool ShowPollButtons()
    {
      return false;

      /* return (Convert.ToInt32(_topic["UserID"]) == PageContext.PageUserID) || PageContext.IsModerator ||
                 PageContext.IsAdmin; */
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
    protected void TrackTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
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
    protected void UnlockTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
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
    /// Adds meta data: description and keywords to the page header.
    /// </summary>
    /// <param name="firstMessage">
    /// first message in the topic
    /// </param>
    private void AddMetaData([NotNull] object firstMessage)
    {
      if (firstMessage.IsNullOrEmptyDBField())
      {
        return;
      }

      if (this.Page.Header != null && this.PageContext.BoardSettings.AddDynamicPageMetaTags)
      {
        YafFormatMessage.MessageCleaned message = YafFormatMessage.GetCleanedTopicMessage(
          firstMessage, this.PageContext.PageTopicID);
        var meta = this.Page.Header.FindControlType<HtmlMeta>();

        if (message.MessageTruncated.IsSet())
        {
          HtmlMeta descriptionMeta;

          string content = "{0}: {1}".FormatWith(this._topic["Topic"], message.MessageTruncated);

          if (meta.Any(x => x.Name.Equals("description")))
          {
            // use existing...
            descriptionMeta = meta.Where(x => x.Name.Equals("description")).FirstOrDefault();
            if (descriptionMeta != null)
            {
              descriptionMeta.Content = content;

              this.Page.Header.Controls.Remove(descriptionMeta);

              descriptionMeta = ControlHelper.MakeMetaDiscriptionControl(content);

              // add to the header...
              this.Page.Header.Controls.Add(descriptionMeta);
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
          HtmlMeta keywordMeta;

          var keywordStr = message.MessageKeywords.Where(x => x.IsSet()).ToList().ToDelimitedString(",");

          if (meta.Any(x => x.Name.Equals("keywords")))
          {
            // use existing...
            keywordMeta = meta.Where(x => x.Name.Equals("keywords")).FirstOrDefault();
            keywordMeta.Content = keywordStr;

            this.Page.Header.Controls.Remove(keywordMeta);

            // add to the header...
            this.Page.Header.Controls.Add(keywordMeta);
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
        new StyleTransform(this.PageContext.Theme).DecodeStyleByTable(ref postListDataTable, true);
      }

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

        // NOTE : tha_watcha: I think Not Needed anymore
        // move to this message on load...
        /* PageContext.PageElements.RegisterJsBlockStartup(
                    this, "GotoAnchorJs", JavaScriptBlocks.LoadGotoAnchor("post{0}".FormatWith(findMessageId)));*/
      }

      var pagedData = rowList.Skip(this.Pager.SkipIndex).Take(this.Pager.PageSize);

      // Add thanks info and styled nicks if they are enabled
      if (YafContext.Current.BoardSettings.EnableThanksMod)
      {
        this.Get<IDBBroker>().AddThanksInfo(pagedData);
      }

      // dynamic load messages that are needed...
      this.Get<IDBBroker>().LoadMessageText(pagedData);

      if (pagedData.Any())
      {
        // handle add description/keywords for SEO
        this.AddMetaData(pagedData.First()["Message"]);
      }

      if (pagedData.Any() && this.CurrentMessage == 0)
      {
        // set it to the first...
        this.CurrentMessage = pagedData.First().Field<int>("MessageID");
      }

      this.MessageList.DataSource = pagedData;

      /* if (_topic["PollID"] != DBNull.Value)
      {
        Poll.Visible = true;
        _dtPoll = DB.poll_stats(_topic["PollID"]);
        Poll.DataSource = _dtPoll;
      } */
      this.DataBind();
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
        if (this._ignoreQueryString)
        {
        }
        else if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m") != null)
        {
          // Show this message
          findMessageId = int.Parse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"));
        }
        else if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("find") != null &&
                 this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("find").ToLower() == "unread")
        {
          // Find next unread
          using (
            DataTable unread = DB.message_findunread(
              this.PageContext.PageTopicID, YafContext.Current.Get<IYafSession>().LastVisit))
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
      // Poll.ItemCommand += Poll_ItemCommand;
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
    private void OptionsMenu_ItemClick([NotNull] object sender, [NotNull] PopEventArgs e)
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
            ForumPages.rsstopic, 
            "pg={0}&t={1}&ft=0", 
            this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("g"), 
            this.PageContext.PageTopicID);
          break;
        case "atomfeed":
          YafBuildLink.Redirect(
            ForumPages.rsstopic, 
            "pg={0}&t={1}&ft=1", 
            this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("g"), 
            this.PageContext.PageTopicID);
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
    private void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
    {
      this._ignoreQueryString = true;
      this.SmartScroller1.Reset();
      this.BindData();
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
    private void QuickReply_Click([NotNull] object sender, [NotNull] EventArgs e)
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
          !CaptchaHelper.IsValid(this.tbCaptcha.Text.Trim()))
      {
        this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"));
        return;
      }

      if (!(this.PageContext.IsAdmin || this.PageContext.IsModerator) &&
          this.PageContext.BoardSettings.PostFloodDelay > 0)
      {
        if (YafContext.Current.Get<IYafSession>().LastPost >
            DateTime.UtcNow.AddSeconds(-this.PageContext.BoardSettings.PostFloodDelay))
        {
          this.PageContext.AddLoadMessage(
            this.GetTextFormatted(
              "wait", 
              (YafContext.Current.Get<IYafSession>().LastPost -
               DateTime.UtcNow.AddSeconds(-this.PageContext.BoardSettings.PostFloodDelay)).Seconds));
          return;
        }
      }

      YafContext.Current.Get<IYafSession>().LastPost = DateTime.UtcNow;

      // post message...
      long nMessageId = 0;
      object replyTo = -1;
      string msg = this._quickReplyEditor.Text;
      long topicID = this.PageContext.PageTopicID;

      var tFlags = new MessageFlags
        {
          IsHtml = this._quickReplyEditor.UsesHTML, 
          IsBBCode = this._quickReplyEditor.UsesBBCode, 
          IsApproved = this.PageContext.IsAdmin || this.PageContext.IsModerator
        };

      // Bypass Approval if Admin or Moderator.
      if (
        !DB.message_save(
          topicID, 
          this.PageContext.PageUserID, 
          msg, 
          null, 
          this.Get<HttpRequestBase>().UserHostAddress, 
          null, 
          replyTo, 
          tFlags.BitValue, 
          ref nMessageId))
      {
        topicID = 0;
      }

      // Check to see if the user has enabled "auto watch topic" option in his/her profile.
      if (this.PageContext.CurrentUserData.AutoWatchTopics)
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
        this.Get<ISendNotification>().ToWatchingUsers(nMessageId.ToType<int>());

        // redirect to newly posted message
        YafBuildLink.Redirect(ForumPages.posts, "m={0}&#post{0}", nMessageId);
      }
      else
      {
        if (this.PageContext.BoardSettings.EmailModeratorsOnModeratedPost)
        {
          // not approved, notifiy moderators
          this.Get<ISendNotification>().ToModeratorsThatMessageNeedsApproval(
            this.PageContext.PageForumID, (int)nMessageId);
        }

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
    private void ViewMenu_ItemClick([NotNull] object sender, [NotNull] PopEventArgs e)
    {
      switch (e.Item.ToLower())
      {
        case "normal":
         /* this.IsThreaded = false;
          this.BindData();*/
              YafBuildLink.Redirect(ForumPages.posts, "t={0}&threaded=false", this.PageContext.PageTopicID);
          break;
        case "threaded":
          /*this.IsThreaded = true;
          this.BindData();*/
          YafBuildLink.Redirect(ForumPages.posts, "t={0}&threaded=true", this.PageContext.PageTopicID);
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
    private void posts_PreRender([NotNull] object sender, [NotNull] EventArgs e)
    {
      bool isWatched = this.HandleWatchTopic();

      // options menu...
      this.OptionsMenu.AddPostBackItem("watch", isWatched ? this.GetText("UNWATCHTOPIC") : this.GetText("WATCHTOPIC"));
      if (this.PageContext.BoardSettings.AllowEmailTopic)
      {
        this.OptionsMenu.AddPostBackItem("email", this.GetText("EMAILTOPIC"));
      }

      this.OptionsMenu.AddPostBackItem("print", this.GetText("PRINTTOPIC"));

      if (this.PageContext.BoardSettings.ShowAtomLink &&
          this.Get<IPermissions>().Check(this.PageContext.BoardSettings.PostsFeedAccess))
      {
        this.OptionsMenu.AddPostBackItem("atomfeed", this.GetText("ATOMTOPIC"));
      }

      if (this.PageContext.BoardSettings.ShowRSSLink &&
          this.Get<IPermissions>().Check(this.PageContext.BoardSettings.PostsFeedAccess))
      {
        this.OptionsMenu.AddPostBackItem("rssfeed", this.GetText("RSSTOPIC"));
      }

      // view menu
        if (this.IsThreaded)
        {
            this.ViewMenu.AddPostBackItem("normal", this.GetText("NORMAL"));
            this.ViewMenu.AddPostBackItem("threaded", "&#187; {0}".FormatWith(this.GetText("THREADED")));
        }
        else
        {
            this.ViewMenu.AddPostBackItem("normal", "&#187; {0}".FormatWith(this.GetText("NORMAL")));
            this.ViewMenu.AddPostBackItem("threaded", this.GetText("THREADED"));
        }

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