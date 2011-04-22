/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2011 Jaben Cargman
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
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Core.Services.CheckForSpam;
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
            LegacyDb.user_removepointsByTopicID(this.PageContext.PageTopicID, 10);
            LegacyDb.topic_delete(this.PageContext.PageTopicID);
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
                    BBCodeHelper.StripBBCode(
                        BBCodeHelper.StripBBCodeQuotes(
                            HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(row["Message"].ToString())))));

            brief = StringExtensions.Truncate(this.Get<IBadWordReplace>().Replace(brief), 100);
            brief = this.Get<IBBCode>().AddSmiles(brief);

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
                new UserLink
                    {
                        ID = "UserLinkForRow{0}".FormatWith(messageId), UserID = row.Field<int>("UserID")
                    }.RenderToString());

            html.AppendFormat(
                " - {0})</span>",
                new DisplayDateTime
                    {
                        DateTime = row["Posted"], Format = DateTimeFormat.BothTopic
                    }.RenderToString());

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
            LegacyDb.topic_lock(this.PageContext.PageTopicID, true);
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
                showAds = this.Get<YafBoardSettings>().ShowAdsToSignedInUsers;
            }

            if (string.IsNullOrEmpty(this.Get<YafBoardSettings>().AdPost) || !showAds)
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
                YafBuildLink.AccessDenied(/*"You are not a forum moderator."*/);
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
            using (DataTable dt = LegacyDb.topic_findnext(this.PageContext.PageTopicID))
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
                      this.GetText("BUTTON_TAGFAVORITE_TT"),
                      this.GetText("BUTTON_TAGFAVORITE"));
                string untagButtonHTML =
                  "'<a class=\"yafcssbigbutton rightItem\" href=\"javascript:removeFavoriteTopic(' + res.d + ');\" onclick=\"jQuery(this).blur();\" title=\"{0}\"><span>{1}</span></a>'"
                    .FormatWith(
                      this.GetText("BUTTON_UNTAGFAVORITE_TT"),
                      this.GetText("BUTTON_UNTAGFAVORITE"));

                // Register the client side script for the "Favorite Topic".
                var favoriteTopicJs =
                    this.Get<IScriptBuilder>().CreateStatement().Add(
                        JavaScriptBlocks.AddFavoriteTopicJs(untagButtonHTML)).AddLine().Add(
                            JavaScriptBlocks.RemoveFavoriteTopicJs(tagButtonHTML));

                YafContext.Current.PageElements.RegisterJsBlockStartup(
               "favoriteTopicJs", favoriteTopicJs);

                /* YafContext.Current.PageElements.RegisterJsBlockStartup(
                   "addFavoriteTopicJs", JavaScriptBlocks.addFavoriteTopicJs(untagButtonHTML));
                 YafContext.Current.PageElements.RegisterJsBlockStartup(
                   "removeFavoriteTopicJs", JavaScriptBlocks.removeFavoriteTopicJs(tagButtonHTML));*/

                var asynchCallFailedJs =
                  this.Get<IScriptBuilder>().CreateStatement().AddFunc(
                    f => f.Name("CallFailed").WithParams("res").Func(s => s.Add("alert('Error Occurred');")));

                YafContext.Current.PageElements.RegisterJsBlockStartup(
                  "asynchCallFailedJs", asynchCallFailedJs);

                /*YafContext.Current.PageElements.RegisterJsBlockStartup(
                  "asynchCallFailedJs", JavaScriptBlocks.asynchCallFailedJs);*/

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
            this._quickReplyEditor.StyleSheet = this.Get<ITheme>().BuildThemePath("theme.css");

            this._topic = LegacyDb.topic_info(this.PageContext.PageTopicID);

            // in case topic is deleted or not existant
            if (this._topic == null)
            {
                YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
            }

            // get topic flags
            this._topicFlags = new TopicFlags(this._topic["Flags"]);

            using (DataTable dt = LegacyDb.forum_list(this.PageContext.PageBoardID, this.PageContext.PageForumID))
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
                    this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
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

                this.ViewOptions.Visible = this.Get<YafBoardSettings>().AllowThreaded;
                this.ForumJumpHolder.Visible = this.Get<YafBoardSettings>().ShowForumJump &&
                                               this.PageContext.Settings.LockedForum == 0;

                this.RssTopic.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                  ForumPages.rsstopic,
                  "pg={0}&t={1}",
                  this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("g"),
                  this.PageContext.PageTopicID);
                this.RssTopic.Visible = this.Get<YafBoardSettings>().ShowRSSLink;

                this.QuickReplyPlaceHolder.Visible = this.Get<YafBoardSettings>().ShowQuickAnswer;

                if ((this.PageContext.IsGuest && this.Get<YafBoardSettings>().EnableCaptchaForGuests) ||
                    (this.Get<YafBoardSettings>().EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded))
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

            #endregion

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
            return !this._topic["PollID"].IsNullOrEmptyDBField() ? this._topic["PollID"].ToType<int>() : 0;
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
            using (DataTable dt = LegacyDb.topic_findprev(this.PageContext.PageTopicID))
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
                LegacyDb.watchtopic_add(this.PageContext.PageUserID, this.PageContext.PageTopicID);
                this.PageContext.AddLoadMessage(this.GetText("INFO_WATCH_TOPIC"));
            }
            else
            {
                int tmpID = this.WatchTopicID.InnerText.ToType<int>();
                LegacyDb.watchtopic_delete(tmpID);
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
            LegacyDb.topic_lock(this.PageContext.PageTopicID, false);
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

            if (this.Page.Header == null || !this.Get<YafBoardSettings>().AddDynamicPageMetaTags)
            {
                return;
            }

            MessageCleaned message = this.Get<IFormatMessage>().GetCleanedTopicMessage(
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

            if (message.MessageKeywords.Count <= 0)
            {
                return;
            }

            HtmlMeta keywordMeta;

            var keywordStr = message.MessageKeywords.Where(x => x.IsSet()).ToList().ToDelimitedString(",");

            //this.Tags.Text = "Tags: {0}".FormatWith(keywordStr);

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

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this._dataBound = true;

            bool showDeleted = false;
            int userId = 0;
            if (this.Get<YafBoardSettings>().ShowDeletedMessagesToAll)
            {
                showDeleted = true;
            }

            if (!showDeleted && ((this.Get<YafBoardSettings>().ShowDeletedMessages &&
                               !this.Get<YafBoardSettings>().ShowDeletedMessagesToAll)
                || this.PageContext.IsAdmin ||
                               this.PageContext.IsForumModerator))
            {
                userId = this.PageContext.PageUserID;
            }

            this.Pager.PageSize = this.Get<YafBoardSettings>().PostsPerPage;
            int messagePosition;
            int findMessageId = this.GetFindMessageId(showDeleted, userId, out messagePosition);
            if (findMessageId > 0)
            {
                this.CurrentMessage = findMessageId;
            }

            if (this._topic == null)
            {
                YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
            }

            DataTable postListDataTable = LegacyDb.post_list(
                this.PageContext.PageTopicID,
                userId,
                this.IsPostBack ? 0 : 1,
                showDeleted,
                this.Get<YafBoardSettings>().UseStyledNicks,
                DateTime.MinValue.AddYears(1901),
                DateTime.UtcNow,
                DateTime.MinValue.AddYears(1901),
                DateTime.UtcNow,
                Pager.CurrentPageIndex,
                Pager.PageSize,
                1,
                0,
                this.IsThreaded ? 1 : 0,
                this.Get<YafBoardSettings>().EnableThanksMod,
                messagePosition);

            if (this.Get<YafBoardSettings>().EnableThanksMod)
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

            if (this.Get<YafBoardSettings>().UseStyledNicks)
            {
                // needs to be moved to the paged data below -- so it doesn't operate on unnecessary rows
                new StyleTransform(this.Get<ITheme>()).DecodeStyleByTable(ref postListDataTable, true);
            }

            // convert to linq...
            var rowList = postListDataTable.AsEnumerable();

            // see if the deleted messages need to be edited out...
            /*  if (this.Get<YafBoardSettings>().ShowDeletedMessages && !this.Get<YafBoardSettings>().ShowDeletedMessagesToAll &&
                  !this.PageContext.IsAdmin && !this.PageContext.IsForumModerator)
              {
                // remove posts that are deleted and do not belong to this user...
                rowList =
                  rowList.Where(x => !(x.Field<bool>("IsDeleted") && x.Field<int>("UserID") != this.PageContext.PageUserID));
              } */

            // set the sorting
            /*  if (!this.IsThreaded)
              {
                // reset position for updated sorting...
                rowList.ForEachIndex(
                  (row, i) =>
                    {
                      row.BeginEdit();
                      row["Position"] = ((Pager.CurrentPageIndex) * Pager.PageSize) + i;
                      row.EndEdit();
                    }); 
              } */

            // set the sorting
            this.Pager.Count = rowList.First().Field<int>("TotalRows");

            if (findMessageId > 0)
            {
                this.Pager.CurrentPageIndex = rowList.First().Field<int>("PageIndex");
            }

            var pagedData = rowList; // .Skip(this.Pager.SkipIndex).Take(this.Pager.PageSize);

            // Add thanks info and styled nicks if they are enabled
            if (this.Get<YafBoardSettings>().EnableThanksMod)
            {
                this.Get<IDBBroker>().AddThanksInfo(pagedData);
            }

            // if current index is 0 we are on the first page and the metadata can be added.
            if (Pager.CurrentPageIndex == 0)
            {
                // handle add description/keywords for SEO
                this.AddMetaData(pagedData.First()["Message"]);
            }

            if (pagedData.Any() && this.CurrentMessage == 0)
            {
                // set it to the first...
                // this.CurrentMessage = pagedData.First().Field<int>("MessageID");
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
        /// <param name="showDeleted">
        /// The show Deleted.
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="messagePosition">
        /// The message Position.
        /// </param>
        /// <returns>
        /// The get find message id.
        /// </returns>
        private int GetFindMessageId(bool showDeleted, int userId, out int messagePosition)
        {
            int findMessageId = 0;
            messagePosition = -1;

            try
            {
                if (this._ignoreQueryString)
                {
                }
                else
                {
                    // temporary find=lastpost code until all last/unread post links are find=lastpost and find=unread
                    if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("find") == null)
                    {
                        if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m") != null)
                        {
                            // we find message position always by time.
                            using (DataTable unread = LegacyDb.message_findunread(
                                this.PageContext.PageTopicID, this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"), DateTime.MinValue.AddYears(1902), showDeleted, userId))
                            {
                                var unreadFirst = unread.AsEnumerable().FirstOrDefault();
                                if (unreadFirst != null)
                                {
                                    findMessageId = unreadFirst.Field<int>("MessageID");
                                    messagePosition = unread.AsEnumerable().FirstOrDefault().Field<int>("MessagePosition");
                                }
                            }
                        }
                    }
                    else
                    {
                        // find last unread message
                        if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("find").ToLower() == "unread")
                        {
                            // Find next unread
                            using (
                                DataTable unread = LegacyDb.message_findunread(
                                    this.PageContext.PageTopicID,
                                    0,
                                    YafContext.Current.Get<IYafSession>().LastVisit,
                                    showDeleted,
                                    userId))
                            {
                                var unreadFirst = unread.AsEnumerable().FirstOrDefault();
                                if (unreadFirst != null)
                                {
                                    findMessageId = unreadFirst.Field<int>("MessageID");
                                    messagePosition = unreadFirst.Field<int>("MessagePosition");

                                    // move to this message on load...
                                    PageContext.PageElements.RegisterJsBlockStartup(
                                        this,
                                        "GotoAnchorJs",
                                        JavaScriptBlocks.LoadGotoAnchor("post{0}".FormatWith(findMessageId)));
                                }
                            }
                        }

                        // find last post
                        if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("find").ToLower() == "lastpost")
                        {
                            // Find next unread
                            using (
                                DataTable unread = LegacyDb.message_findunread(
                                    this.PageContext.PageTopicID, 0, DateTime.UtcNow, showDeleted, userId))
                            {
                                var unreadFirst = unread.AsEnumerable().FirstOrDefault();
                                if (unreadFirst != null)
                                {
                                    findMessageId = unreadFirst.Field<int>("MessageID");
                                    messagePosition = unreadFirst.Field<int>("MessagePosition");

                                    // move to this message on load...
                                    PageContext.PageElements.RegisterJsBlockStartup(
                                        this,
                                        "GotoAnchorJs",
                                        JavaScriptBlocks.LoadGotoAnchor("post{0}".FormatWith(findMessageId)));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
                LegacyDb.eventlog_create(this.PageContext.PageUserID, this, x);
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
            using (DataTable dt = LegacyDb.watchtopic_check(this.PageContext.PageUserID, this.PageContext.PageTopicID))
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
            this.ShareMenu.ItemClick += this.ShareMenu_ItemClick;
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
        private void ShareMenu_ItemClick([NotNull] object sender, [NotNull] PopEventArgs e)
        {
            switch (e.Item.ToLower())
            {
                case "email":
                    this.EmailTopic_Click(sender, e);
                    break;
                case "retweet":
                    {
                        var twitterName = this.Get<YafBoardSettings>().TwitterUserName.IsSet()
                                              ? "@{0} ".FormatWith(this.Get<YafBoardSettings>().TwitterUserName)
                                              : string.Empty;

                        // process message... clean html, strip html, remove bbcode, etc...
                        var twitterMsg =
                          StringExtensions.RemoveMultipleWhitespace(
                            BBCodeHelper.StripBBCode(HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString((string)this._topic["Topic"]))));

                        var tweetUrl =
                            "http://twitter.com/share?url={0}&text={1}".FormatWith(
                                this.Server.UrlEncode(this.Get<HttpRequestBase>().Url.ToString()),
                                this.Server.UrlEncode(
                                    "RT {1}Thread: {0}".FormatWith(StringExtensions.Truncate(twitterMsg, 100), twitterName)));

                        this.Get<HttpResponseBase>().Redirect(tweetUrl);
                    }

                    break;
                case "digg":
                    {
                        var diggUrl =
                            "http://digg.com/submit?url={0}&title={1}".FormatWith(
                                this.Server.UrlEncode(this.Get<HttpRequestBase>().Url.ToString()),
                                this.Server.UrlEncode((string)this._topic["Topic"]));

                        this.Get<HttpResponseBase>().Redirect(diggUrl);
                    }

                    break;
                case "reddit":
                    {
                        var redditUrl =
                            "http://www.reddit.com/submit?url={0}&title={1}".FormatWith(
                                this.Server.UrlEncode(this.Get<HttpRequestBase>().Url.ToString()),
                                this.Server.UrlEncode((string)this._topic["Topic"]));

                        this.Get<HttpResponseBase>().Redirect(redditUrl);
                    }

                    break;
                case "buzz":
                    {
                        var buzzUrl =
                            "http://www.google.com/buzz/post?url={0}&messag={1}".FormatWith(
                                this.Server.UrlEncode(this.Get<HttpRequestBase>().Url.ToString()),
                                this.Server.UrlEncode("Thread: {0}".FormatWith((string)this._topic["Topic"])));

                        this.Get<HttpResponseBase>().Redirect(buzzUrl);
                    }

                    break;
                /*case "facebook":
                    {
                        var facebookUrl =
                            "http://www.facebook.com/plugins/like.php?href={0}".FormatWith(
                                this.Server.UrlEncode(this.Get<HttpRequestBase>().Url.ToString()),
                                this.Server.UrlEncode((string)this._topic["Topic"]));

                        this.Get<HttpResponseBase>().Redirect(facebooUrl);
                    }

                    break;*/
                default:
                    throw new ApplicationException(e.Item);
            }
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

            if (((this.PageContext.IsGuest && this.Get<YafBoardSettings>().EnableCaptchaForGuests) ||
                 (this.Get<YafBoardSettings>().EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded)) &&
                !CaptchaHelper.IsValid(this.tbCaptcha.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"));
                return;
            }

            if (!(this.PageContext.IsAdmin || this.PageContext.IsModerator) &&
                this.Get<YafBoardSettings>().PostFloodDelay > 0)
            {
                if (YafContext.Current.Get<IYafSession>().LastPost >
                    DateTime.UtcNow.AddSeconds(-this.Get<YafBoardSettings>().PostFloodDelay))
                {
                    this.PageContext.AddLoadMessage(
                      this.GetTextFormatted(
                        "wait",
                        (YafContext.Current.Get<IYafSession>().LastPost -
                         DateTime.UtcNow.AddSeconds(-this.Get<YafBoardSettings>().PostFloodDelay)).Seconds));
                    return;
                }
            }

            YafContext.Current.Get<IYafSession>().LastPost = DateTime.UtcNow;

            // post message...
            long nMessageId = 0;
            object replyTo = -1;
            string msg = this._quickReplyEditor.Text;
            long topicID = this.PageContext.PageTopicID;

            // SPAM Check

            // Check if Forum is Moderated
            DataRow forumInfo;
            bool isForumModerated = false;

            using (DataTable dt = LegacyDb.forum_list(this.PageContext.PageBoardID, this.PageContext.PageForumID))
            {
                forumInfo = dt.Rows[0];
            }

            if (forumInfo != null)
            {
                isForumModerated = forumInfo["Flags"].BinaryAnd(ForumFlags.Flags.IsModerated);
            }

            bool spamApproved = true;

            // Check for SPAM
            if (!this.PageContext.IsAdmin || !this.PageContext.IsModerator)
            {
                if (this.IsPostSpam())
                {
                    if (this.Get<YafBoardSettings>().SpamMessageHandling.Equals(1))
                    {
                        spamApproved = false;
                    }
                    else if (this.Get<YafBoardSettings>().SpamMessageHandling.Equals(2))
                    {
                        this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"));
                        return;
                    }
                }
            }

            // If Forum is Moderated
            if (isForumModerated)
            {
                spamApproved = false;
            }

            // Bypass Approval if Admin or Moderator
            if (this.PageContext.IsAdmin || this.PageContext.IsModerator)
            {
                spamApproved = true;
            }

            var tFlags = new MessageFlags
              {
                  IsHtml = this._quickReplyEditor.UsesHTML,
                  IsBBCode = this._quickReplyEditor.UsesBBCode,
                  IsApproved = spamApproved
              };

            // Bypass Approval if Admin or Moderator.
            if (
              !LegacyDb.message_save(
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
                using (DataTable dt = LegacyDb.watchtopic_check(this.PageContext.PageUserID, this.PageContext.PageTopicID))
                {
                    if (dt.Rows.Count == 0)
                    {
                        // subscribe to this forum
                        LegacyDb.watchtopic_add(this.PageContext.PageUserID, this.PageContext.PageTopicID);
                    }
                }
            }

            bool bApproved = false;

            using (DataTable dt = LegacyDb.message_list(nMessageId))
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
                if (this.Get<YafBoardSettings>().EmailModeratorsOnModeratedPost)
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
        /// Check This Post for SPAM against the BlogSpam.NET API or Akismet Service
        /// </summary>
        /// <returns>
        /// Returns if Post is SPAM or not
        /// </returns>
        private bool IsPostSpam()
        {
            if (this.Get<YafBoardSettings>().SpamServiceType.Equals(0))
            {
                return false;
            }

            string ipAdress = this.Get<HttpRequestBase>().UserHostAddress;

            if (ipAdress.Equals("::1"))
            {
                ipAdress = "127.0.0.1";
            }

            string whiteList = String.Empty;

            if (ipAdress.Equals("127.0.0.1"))
            {
                whiteList = "whitelist=127.0.0.1";
            }

            string email, username;

            if (this.User == null)
            {
                email = null;
                username = "Guest";
            }
            else
            {
                email = this.User.Email;
                username = this.User.UserName;
            }

            // Use BlogSpam.NET API
            if (this.Get<YafBoardSettings>().SpamServiceType.Equals(1))
            {
                try
                {
                    return
                        BlogSpamNet.CommentIsSpam(
                            new BlogSpamComment
                            {
                                comment = this._quickReplyEditor.Text,
                                ip = ipAdress,
                                agent = this.Get<HttpRequestBase>().UserAgent,
                                email = email,
                                name = username,
                                version = String.Empty,
                                options = whiteList,
                                subject = this.PageContext.PageTopicName
                            },
                            true);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            // Use Akismet API
            if (this.Get<YafBoardSettings>().SpamServiceType.Equals(2) && !string.IsNullOrEmpty(this.Get<YafBoardSettings>().AkismetApiKey))
            {
                try
                {
                    var service = new AkismetSpamClient(this.Get<YafBoardSettings>().AkismetApiKey, new Uri(BaseUrlBuilder.BaseUrl));

                    return
                        service.CheckCommentForSpam(
                            new Comment(IPAddress.Parse(ipAdress), this.Get<HttpRequestBase>().UserAgent)
                            {
                                Content = this._quickReplyEditor.Text,
                                Author = username,
                                AuthorEmail = email
                            });
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
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

            // share menu...
            this.ShareMenu.Visible = this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ShowShareTopicTo);
            this.ShareLink.Visible = this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ShowShareTopicTo);

            this.ShareLink.ToolTip = this.GetText("SHARE_TOOLTIP");

            if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ShowShareTopicTo))
            {
                if (this.Get<YafBoardSettings>().AllowEmailTopic)
                {
                    this.ShareMenu.AddPostBackItem(
                        "email", this.GetText("EMAILTOPIC"), this.Get<ITheme>().GetItem("ICONS", "EMAIL"));
                }

                this.ShareMenu.AddPostBackItem(
                    "retweet", this.GetText("RETWEET_TOPIC"), this.Get<ITheme>().GetItem("ICONS", "TWITTER"));
                this.ShareMenu.AddPostBackItem(
                    "buzz", this.GetText("BUZZ_TOPIC"), this.Get<ITheme>().GetItem("ICONS", "BUZZ"));
               /* this.ShareMenu.AddPostBackItem(
                    "facebook", this.GetText("FACEBOOK_TOPIC"), this.Get<ITheme>().GetItem("ICONS", "FACEBOOK"));*/

                var facebookUrl =
                            "http://www.facebook.com/plugins/like.php?href={0}".FormatWith(
                                this.Server.UrlEncode(this.Get<HttpRequestBase>().Url.ToString()),
                                this.Server.UrlEncode((string)this._topic["Topic"]));

                this.ShareMenu.AddClientScriptItem(
                    this.GetText("FACEBOOK_TOPIC"),
                    @"window.open('{0}','Facebook','width=300,height=200,resizable=yes');".FormatWith(facebookUrl),
                    this.Get<ITheme>().GetItem("ICONS", "FACEBOOK"));

                this.ShareMenu.AddPostBackItem(
                    "digg", this.GetText("DIGG_TOPIC"), this.Get<ITheme>().GetItem("ICONS", "DIGG"));
                this.ShareMenu.AddPostBackItem(
                    "reddit", this.GetText("REDDIT_TOPIC"), this.Get<ITheme>().GetItem("ICONS", "REDDIT"));
            }
            else
            {
                if (this.Get<YafBoardSettings>().AllowEmailTopic)
                {
                    this.OptionsMenu.AddPostBackItem(
                        "email", this.GetText("EMAILTOPIC"), this.Get<ITheme>().GetItem("ICONS", "EMAIL"));
                }
            }

            // options menu...
            this.OptionsLink.ToolTip = this.GetText("OPTIONS_TOOLTIP");

            this.OptionsMenu.AddPostBackItem(
                "watch",
                isWatched ? this.GetText("UNWATCHTOPIC") : this.GetText("WATCHTOPIC"),
                this.Get<ITheme>().GetItem("ICONS", "EMAIL"));

            this.OptionsMenu.AddPostBackItem("print", this.GetText("PRINTTOPIC"), this.Get<ITheme>().GetItem("ICONS", "PRINT"));

            if (this.Get<YafBoardSettings>().ShowAtomLink &&
                this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().PostsFeedAccess))
            {
                this.OptionsMenu.AddPostBackItem("atomfeed", this.GetText("ATOMTOPIC"), this.Get<ITheme>().GetItem("ICONS", "ATOMFEED"));
            }

            if (this.Get<YafBoardSettings>().ShowRSSLink &&
                this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().PostsFeedAccess))
            {
                this.OptionsMenu.AddPostBackItem("rssfeed", this.GetText("RSSTOPIC"), this.Get<ITheme>().GetItem("ICONS", "RSSFEED"));
            }

            // view menu
            this.ViewLink.ToolTip = this.GetText("VIEW_TOOLTIP");

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

            // attach the menus to HyperLinks
            this.ShareMenu.Attach(this.ShareLink);
            this.OptionsMenu.Attach(this.OptionsLink);
            this.ViewMenu.Attach(this.ViewLink);

            if (!this._dataBound)
            {
                this.BindData();
            }
        }
    }
}