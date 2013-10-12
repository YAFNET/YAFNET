/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
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
    #region Using

    using System;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Services.Auth;
    using YAF.Editors;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utilities;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Posts Page.
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
                    this.Session["IsThreaded"] =
                        bool.Parse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("threaded"));
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
        /// <param name="sender">The source of the event.</param>
        /// </param>
        /// <param name="e">
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
        /// <param name="sender">The source of the event.</param>
        /// </param>
        /// <param name="e">
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// </param>
        protected void DeleteTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageContext.ForumModeratorAccess)
            {
                /*"You don't have access to delete topics."*/
                YafBuildLink.AccessDenied();
            }

            LegacyDb.topic_delete(this.PageContext.PageTopicID);
            YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
        }

        /// <summary>
        /// The delete topic_ load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteTopic_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}')".FormatWith(this.GetText("confirm_deletetopic"));
        }

        /// <summary>
        /// The email topic_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void EmailTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.User == null)
            {
                this.PageContext.AddLoadMessage(this.GetText("WARN_EMAILLOGIN"), MessageTypes.Warning);
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
        /// Returns the indent image.
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
        /// Returns the threaded row.
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

            brief = this.Get<IBadWordReplace>().Replace(brief).Truncate(100);
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
            html.AppendFormat(@"<img src=""{0}"" alt="""" class=""avatarimage img-rounded"" />", avatarUrl);
            html.AppendFormat(
                @"<a href=""{0}"" class=""threadUrl"">{1}</a>",
                YafBuildLink.GetLink(ForumPages.posts, "m={0}#post{0}", messageId),
                brief);

            html.Append(" (");
            html.Append(
                new UserLink
                    {
                        ID = "UserLinkForRow{0}".FormatWith(messageId), 
                        UserID = row.Field<int>("UserID")
                    }.RenderToString());

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
        /// Returns if it the current message.
        /// </returns>
        protected bool IsCurrentMessage([NotNull] object o)
        {
            CodeContracts.VerifyNotNull(o, "o");

            var row = (DataRow)o;

            return !this.IsThreaded || this.CurrentMessage == (int)row["MessageID"];
        }

        /// <summary>
        /// The lock topic_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void LockTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageContext.ForumModeratorAccess)
            {
                // "You are not a forum moderator.
                YafBuildLink.AccessDenied();
            }

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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void MoveTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageContext.ForumModeratorAccess)
            {
                YafBuildLink.AccessDenied(/*"You are not a forum moderator."*/);
            }
        }

        /// <summary>
        /// The new topic_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void NewTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this._forumFlags.IsLocked)
            {
                return;
            }

            this.PageContext.AddLoadMessage(this.GetText("WARN_FORUM_LOCKED"));
        }

        /// <summary>
        /// The next topic_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
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
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            base.OnPreRender(e);
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafContext.Current.PageElements.RegisterJsResourceInclude("yafPageMethodjs", "js/jquery.pagemethod.js");

            if (!this.PageContext.IsGuest)
            {
                // The html code for "Favorite Topic" theme buttons.
                string tagButtonHTML =
                    "'<a class=\"yafcssbigbutton rightItem\" href=\"javascript:addFavoriteTopic(' + res.d + ');\" onclick=\"jQuery(this).blur();\" title=\"{0}\"><span>{1}</span></a>'"
                        .FormatWith(this.GetText("BUTTON_TAGFAVORITE_TT"), this.GetText("BUTTON_TAGFAVORITE"));
                string untagButtonHTML =
                    "'<a class=\"yafcssbigbutton rightItem\" href=\"javascript:removeFavoriteTopic(' + res.d + ');\" onclick=\"jQuery(this).blur();\" title=\"{0}\"><span>{1}</span></a>'"
                        .FormatWith(this.GetText("BUTTON_UNTAGFAVORITE_TT"), this.GetText("BUTTON_UNTAGFAVORITE"));

                // Register the client side script for the "Favorite Topic".
                var favoriteTopicJs = JavaScriptBlocks.AddFavoriteTopicJs(untagButtonHTML) + Environment.NewLine +
                                      JavaScriptBlocks.RemoveFavoriteTopicJs(tagButtonHTML);

                YafContext.Current.PageElements.RegisterJsBlockStartup("favoriteTopicJs", favoriteTopicJs);
                YafContext.Current.PageElements.RegisterJsBlockStartup("asynchCallFailedJs", "function CallFailed(res){ alert('Error Occurred'); }");

                // Has the user already tagged this topic as favorite?
                if (this.Get<IFavoriteTopic>().IsFavoriteTopic(this.PageContext.PageTopicID))
                {
                    // Generate the "Untag" theme button with appropriate JS calls for onclick event.
                    this.TagFavorite1.NavigateUrl = "javascript:removeFavoriteTopic(" + this.PageContext.PageTopicID
                                                    + ");";
                    this.TagFavorite2.NavigateUrl = "javascript:removeFavoriteTopic(" + this.PageContext.PageTopicID
                                                    + ");";
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

            this._quickReplyEditor.BaseDir = "{0}editors".FormatWith(YafForumInfo.ForumClientFileRoot);
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
                // Clear Multiquotes
                this.Get<IYafSession>().MultiQuoteIds = null;

                if (this.PageContext.Settings.LockedForum == 0)
                {
                    this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
                    this.PageLinks.AddLink(
                        this.PageContext.PageCategoryName,
                        YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
                }

                this.NewTopic2.NavigateUrl =
                    this.NewTopic1.NavigateUrl =
                    YafBuildLink.GetLinkNotEscaped(ForumPages.postmessage, "f={0}", this.PageContext.PageForumID);

                this.MoveTopic1.NavigateUrl =
                    this.MoveTopic2.NavigateUrl =
                    YafBuildLink.GetLinkNotEscaped(ForumPages.movetopic, "t={0}", this.PageContext.PageTopicID);

                this.PostReplyLink1.NavigateUrl =
                    this.PostReplyLink2.NavigateUrl =
                    YafBuildLink.GetLinkNotEscaped(
                        ForumPages.postmessage,
                        "t={0}&f={1}",
                        this.PageContext.PageTopicID,
                        this.PageContext.PageForumID);

                this.QuickReply.Text = this.GetText("POSTMESSAGE", "SAVE");
                this.DataPanel1.TitleText = this.GetText("QUICKREPLY");
                this.DataPanel1.ExpandText = this.GetText("QUICKREPLY_SHOW");
                this.DataPanel1.CollapseText = this.GetText("QUICKREPLY_HIDE");

                this.PageLinks.AddForumLinks(this.PageContext.PageForumID);
                this.PageLinks.AddLink(
                    this.Get<IBadWordReplace>().Replace(this.Server.HtmlDecode(this.PageContext.PageTopicName)),
                    string.Empty);

                var topicSubject = this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this._topic["Topic"]));

                if (this._topic["Status"].ToString().IsSet() && this.Get<YafBoardSettings>().EnableTopicStatus)
                {
                    var topicStatusIcon = this.Get<ITheme>().GetItem("TOPIC_STATUS", this._topic["Status"].ToString());

                    if (topicStatusIcon.IsSet() && !topicStatusIcon.Contains("[TOPIC_STATUS."))
                    {
                        topicSubject =
                            "<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" class=\"topicStatusIcon\" />&nbsp;{2}"
                                .FormatWith(
                                    this.Get<ITheme>().GetItem("TOPIC_STATUS", this._topic["Status"].ToString()),
                                    this.GetText("TOPIC_STATUS", this._topic["Status"].ToString()),
                                    topicSubject);
                    }
                    else
                    {
                        topicSubject =
                            "[{0}]&nbsp;{1}".FormatWith(
                                this.GetText("TOPIC_STATUS", this._topic["Status"].ToString()), topicSubject);
                    }
                }

                if (!this._topic["Description"].IsNullOrEmptyDBField()
                    && this.Get<YafBoardSettings>().EnableTopicDescription)
                {
                    this.TopicTitle.Text = "{0} - <em>{1}</em>".FormatWith(
                        topicSubject, this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this._topic["Description"])));
                }
                else
                {
                    this.TopicTitle.Text = this.Get<IBadWordReplace>().Replace(topicSubject);
                }

                this.TopicLink.ToolTip = this.Get<IBadWordReplace>().Replace(
                    this.HtmlEncode(this._topic["Description"]));
                this.TopicLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.posts, "t={0}", this.PageContext.PageTopicID);
                this.ViewOptions.Visible = this.Get<YafBoardSettings>().AllowThreaded;
                this.ForumJumpHolder.Visible = this.Get<YafBoardSettings>().ShowForumJump
                                               && this.PageContext.Settings.LockedForum == 0;

                this.RssTopic.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.rsstopic, "pg={0}&t={1}", YafRssFeeds.Posts.ToInt(), this.PageContext.PageTopicID);
                this.RssTopic.Visible = this.Get<YafBoardSettings>().ShowRSSLink;

                this.QuickReplyPlaceHolder.Visible = this.Get<YafBoardSettings>().ShowQuickAnswer;

                if ((this.PageContext.IsGuest && this.Get<YafBoardSettings>().EnableCaptchaForGuests)
                    || (this.Get<YafBoardSettings>().EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded))
                {
                    this.imgCaptcha.ImageUrl = "{0}resource.ashx?c=1".FormatWith(YafForumInfo.ForumClientFileRoot);
                    this.CaptchaDiv.Visible = true;
                }

                if (!this.PageContext.ForumPostAccess
                    || (this._forumFlags.IsLocked && !this.PageContext.ForumModeratorAccess))
                {
                    this.NewTopic1.Visible = false;
                    this.NewTopic2.Visible = false;
                }

                // Ederon : 9/9/2007 - moderators can reply in locked topics
                if (!this.PageContext.ForumReplyAccess
                    ||
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

            this.BindData();

            if (!this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ShowShareTopicTo)
                || !Config.FacebookAPIKey.IsSet())
            {
                return;
            }

            YafContext.Current.PageElements.RegisterJsBlockStartup("facebookInitJs", JavaScriptBlocks.FacebookInitJs);

            var message =
                StringExtensions.RemoveMultipleWhitespace(
                    BBCodeHelper.StripBBCode(
                        HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString((string)this._topic["Topic"]))));

            var meta = this.Page.Header.FindControlType<HtmlMeta>();
            string description = string.Empty;

            if (meta.Any(x => x.Name.Equals("description")))
            {
                var descriptionMeta = meta.FirstOrDefault(x => x.Name.Equals("description"));
                if (descriptionMeta != null)
                {
                    description = descriptionMeta.Content;
                }
            }

            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "facebookPostJs",
                JavaScriptBlocks.FacebookPostJs(
                    this.Server.HtmlEncode(message),
                    this.Server.HtmlEncode(description),
                    this.Get<HttpRequestBase>().Url.ToString(),
                    "{0}/YAFLogo.jpg".FormatWith(
                        Path.Combine(YafForumInfo.ForumBaseUrl, YafBoardFolders.Current.Images)),
                    "Logo"));
        }

        /// <summary>
        /// The poll group id.
        /// </summary>
        /// <returns>
        /// Returns The poll group id.
        /// </returns>
        protected int PollGroupId()
        {
            return !this._topic["PollID"].IsNullOrEmptyDBField() ? this._topic["PollID"].ToType<int>() : 0;
        }

        /// <summary>
        /// The post reply link_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PostReplyLink_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Ederon : 9/9/2007 - moderator can reply in locked posts
            if (this.PageContext.ForumModeratorAccess)
            {
                return;
            }

            if (this._topicFlags.IsLocked)
            {
                this.PageContext.AddLoadMessage(this.GetText("WARN_TOPIC_LOCKED"));
                return;
            }

            if (!this._forumFlags.IsLocked)
            {
                return;
            }

            this.PageContext.AddLoadMessage(this.GetText("WARN_FORUM_LOCKED"));
        }

        /// <summary>
        /// The Previous topic click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PrintTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.printtopic, "t={0}", this.PageContext.PageTopicID);
        }

        /// <summary>
        /// The show poll buttons.
        /// </summary>
        /// <returns>
        /// Returns The show poll buttons.
        /// </returns>
        protected bool ShowPollButtons()
        {
            return false;

            /* return (Convert.ToInt32(_topic["UserID"]) == PageContext.PageUserID) || PageContext.IsModerator || PageContext.IsAdmin; */
        }

        /// <summary>
        /// The track topic_ click.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
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
                this.GetRepository<WatchTopic>().Add(this.PageContext.PageUserID, this.PageContext.PageTopicID);
                this.PageContext.AddLoadMessage(this.GetText("INFO_WATCH_TOPIC"));
            }
            else
            {
                int tmpID = this.WatchTopicID.InnerText.ToType<int>();

                this.GetRepository<WatchTopic>().DeleteByID(tmpID);

                this.PageContext.AddLoadMessage(this.GetText("INFO_UNWATCH_TOPIC"));
            }

            this.HandleWatchTopic();
        }

        /// <summary>
        /// The unlock topic_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void UnlockTopic_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageContext.ForumModeratorAccess)
            {
                YafBuildLink.AccessDenied(/*"You are not a forum moderator."*/);
            }

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

                // Use Topic Description if set
                var descriptionContent = !this._topic["Description"].IsNullOrEmptyDBField()
                                         ? this.Get<IBadWordReplace>().Replace(
                                             this.HtmlEncode(this._topic["Description"]))
                                         : "{0}: {1}".FormatWith(this._topic["Topic"], message.MessageTruncated);

                if (meta.Any(x => x.Name.Equals("description")))
                {
                    // use existing...
                    descriptionMeta = meta.FirstOrDefault(x => x.Name.Equals("description"));
                    if (descriptionMeta != null)
                    {
                        descriptionMeta.Content = descriptionContent;

                        this.Page.Header.Controls.Remove(descriptionMeta);

                        descriptionMeta = ControlHelper.MakeMetaDiscriptionControl(descriptionContent);

                        // add to the header...
                        this.Page.Header.Controls.Add(descriptionMeta);
                    }
                }
                else
                {
                    descriptionMeta = ControlHelper.MakeMetaDiscriptionControl(descriptionContent);

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

            //// this.Tags.Text = "Tags: {0}".FormatWith(keywordStr);

            if (meta.Any(x => x.Name.Equals("keywords")))
            {
                // use existing...
                keywordMeta = meta.FirstOrDefault(x => x.Name.Equals("keywords"));
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
            if (this._topic == null)
            {
                YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
            }

            this._dataBound = true;

            bool showDeleted = false;
            int userId = this.PageContext.PageUserID;

            if ((this.PageContext.IsAdmin
                || this.PageContext.ForumModeratorAccess) || this.Get<YafBoardSettings>().ShowDeletedMessagesToAll)
            {
                showDeleted = true;
            }

           int messagePosition;
           if (!this.Get<YafBoardSettings>().ShowDeletedMessages || !this.Get<YafBoardSettings>().ShowDeletedMessagesToAll)
           {
               // normally, users can always see own deleted topics or stubs we set a false id to hide them.
               userId = -1;
           }

            int findMessageId = this.GetFindMessageId(showDeleted, userId, out messagePosition);
            if (findMessageId > 0)
            {
                this.CurrentMessage = findMessageId;
            }         

            // Mark topic read
            this.Get<IReadTrackCurrentUser>().SetTopicRead(this.PageContext.PageTopicID);
            this.Pager.PageSize = this.Get<YafBoardSettings>().PostsPerPage;

            DataTable postListDataTable = LegacyDb.post_list(
                this.PageContext.PageTopicID,
                this.PageContext.PageUserID,
                userId,
                this.IsPostBack || PageContext.IsCrawler ? 0 : 1,
                showDeleted,
                this.Get<YafBoardSettings>().UseStyledNicks,
                !this.PageContext.IsGuest && this.Get<YafBoardSettings>().DisplayPoints,
                DateTimeHelper.SqlDbMinTime(),
                DateTime.UtcNow,
                DateTimeHelper.SqlDbMinTime(),
                DateTime.UtcNow,
                this.Pager.CurrentPageIndex,
                this.Pager.PageSize,
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
                            //// How many times has the message poster thanked others?   
                            new DataColumn("MessageThanksNumber", Type.GetType("System.Int32")),
                            //// How many times has the message poster been thanked?
                            new DataColumn("ThanksFromUserNumber", Type.GetType("System.Int32")),
                            //// In how many posts has the message poster been thanked? 
                            new DataColumn("ThanksToUserNumber", Type.GetType("System.Int32")),
                            //// In how many posts has the message poster been thanked? 
                            new DataColumn("ThanksToUserPostsNumber", Type.GetType("System.Int32"))
                        });

                postListDataTable.AcceptChanges();
            }

            if (this.Get<YafBoardSettings>().UseStyledNicks)
            {
                // needs to be moved to the paged data below -- so it doesn't operate on unnecessary rows
                this.Get<IStyleTransform>().DecodeStyleByTable(postListDataTable, true);
            }

            // convert to linq...
            var rowList = postListDataTable.AsEnumerable();

            // see if the deleted messages need to be edited out...
            /*if (this.Get<YafBoardSettings>().ShowDeletedMessages && !this.Get<YafBoardSettings>().ShowDeletedMessagesToAll &&
     !this.PageContext.IsAdmin && !this.PageContext.IsForumModerator)
            {
                            // remove posts that are deleted and do not belong to this user...
                            rowList =
                                            rowList.Where(
                                                            x => !(x.Field<bool>("IsDeleted") && x.Field<int>("UserID") != this.PageContext.PageUserID));
            }*/

            // set the sorting
            /*if (!this.IsThreaded)
            {
                            // reset position for updated sorting...
                            rowList.ForEachIndex(
                                            (row, i) =>
                                                            {
                                                                            row.BeginEdit();
                                                                            row["Position"] = (Pager.CurrentPageIndex * Pager.PageSize) + i;
                                                                            row.EndEdit();
                                                            });
            }*/

            // set the sorting
            this.Pager.Count = rowList.First().Field<int>(columnName: "TotalRows");

            if (findMessageId > 0)
            {
                this.Pager.CurrentPageIndex = rowList.First().Field<int>(columnName: "PageIndex");
                
                // move to this message on load...
                if (!this.PageContext.IsCrawler)
                {
                    PageContext.PageElements.RegisterJsBlockStartup(
                        this, "GotoAnchorJs", JavaScriptBlocks.LoadGotoAnchor("post{0}".FormatWith(findMessageId)));
                }
            }
            else
            {
                // move to this message on load...
                if (!this.PageContext.IsCrawler)
                {
                    PageContext.PageElements.RegisterJsBlockStartup(
                        this,
                        "GotoAnchorJs",
                        JavaScriptBlocks.LoadGotoAnchor("post{0}".FormatWith(rowList.First().Field<int>(columnName: "MessageID"))));
                }
            }

            var pagedData = rowList; // .Skip(this.Pager.SkipIndex).Take(this.Pager.PageSize);

            // Add thanks info and styled nicks if they are enabled
            if (this.Get<YafBoardSettings>().EnableThanksMod)
            {
                this.Get<YafDbBroker>().AddThanksInfo(dataRows: pagedData);
            }

            // if current index is 0 we are on the first page and the metadata can be added.
            if (this.Pager.CurrentPageIndex == 0)
            {
                // handle add description/keywords for SEO
                this.AddMetaData(firstMessage: pagedData.First()["Message"]);
            }

            if (pagedData.Any() && this.CurrentMessage == 0)
            {
                // set it to the first...
                // this.CurrentMessage = pagedData.First().Field<int>("MessageID");
            }

            this.MessageList.DataSource = pagedData;

            /*if (this._topic["PollID"] != DBNull.Value)
            {
                            Poll.Visible = true;
                            _dtPoll = DB.poll_stats(this._topic["PollID"]);
                            Poll.DataSource = _dtPoll;
            }*/

            this.ImageLastUnreadMessageLink.Visible = this.Get<YafBoardSettings>().ShowLastUnreadPost;

            this.ImageMessageLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.posts, "t={0}&find=lastpost", this.PageContext.PageTopicID);

            this.LastPostedImage.LocalizedTitle = this.GetText("DEFAULT", "GO_LAST_POST");

            if (this.ImageLastUnreadMessageLink.Visible)
            {
                this.ImageLastUnreadMessageLink.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.posts, "t={0}&find=unread", this.PageContext.PageTopicID);

                this.LastUnreadImage.LocalizedTitle = this.GetText(page: "DEFAULT", tag: "GO_LASTUNREAD_POST");
            }

            if (this._topic["LastPosted"] != DBNull.Value)
            {
                DateTime lastRead =
                    this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                        forumId: this.PageContext.PageForumID, topicId: this.PageContext.PageTopicID);

                this.LastUnreadImage.ThemeTag = (DateTime.Parse(this._topic["LastPosted"].ToString()) > lastRead)
                                                    ? "ICON_NEWEST_UNREAD"
                                                    : "ICON_LATEST_UNREAD";

                this.LastPostedImage.ThemeTag = (DateTime.Parse(this._topic["LastPosted"].ToString()) > lastRead)
                                                    ? "ICON_NEWEST"
                                                    : "ICON_LATEST";
            }

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
                if (!this._ignoreQueryString)
                {                
                    // temporary find=lastpost code until all last/unread post links are find=lastpost and find=unread
                    if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("find") == null)
                    {
                        if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m") != null)
                        {
                            // we find message position always by time.
                            using (
                                DataTable lastPost = LegacyDb.message_findunread(
                                    topicID: this.PageContext.PageTopicID,
                                    messageId: this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"),
                                    lastRead: DateTimeHelper.SqlDbMinTime(),
                                    showDeleted: showDeleted,
                                    authorUserID: userId))
                            {
                                var unreadFirst = lastPost.AsEnumerable().FirstOrDefault();
                                if (unreadFirst != null)
                                {
                                    findMessageId = unreadFirst.Field<int>("MessageID");
                                    DataRow first = lastPost.AsEnumerable().FirstOrDefault();
                                    if (first != null)
                                    {
                                        messagePosition = first.Field<int>("MessagePosition");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // find first unread message
                        if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("find").ToLower() == "unread")
                        {
                            DateTime lastRead = !this.PageContext.IsCrawler
                                                    ? this.Get<IReadTrackCurrentUser>()
                                                          .GetForumTopicRead(
                                                              forumId: this.PageContext.PageForumID,
                                                              topicId: this.PageContext.PageTopicID)
                                                    : DateTime.UtcNow;

                            // Find next unread
                            using (
                                DataTable unread = LegacyDb.message_findunread(
                                    topicID: this.PageContext.PageTopicID,
                                    messageId: 0,
                                    lastRead: lastRead,
                                    showDeleted: showDeleted,
                                    authorUserID: userId))
                            {
                                var unreadFirst = unread.AsEnumerable().FirstOrDefault();
                                if (unreadFirst != null)
                                {
                                    findMessageId = unreadFirst.Field<int>("MessageID");
                                    messagePosition = unreadFirst.Field<int>("MessagePosition");
                                }
                            }
                        }

                        using (
                            DataTable unread = LegacyDb.message_findunread(
                                topicID: this.PageContext.PageTopicID,
                                messageId: 0,
                                lastRead: DateTime.UtcNow,
                                showDeleted: showDeleted,
                                authorUserID: userId))
                        {
                            var unreadFirst = unread.AsEnumerable().FirstOrDefault();
                            if (unreadFirst != null)
                            {
                                findMessageId = unreadFirst.Field<int>("MessageID");
                                messagePosition = unreadFirst.Field<int>("MessagePosition");
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
                this.Logger.Log(this.PageContext.PageUserID, this, x);
            }

            return findMessageId;
        }

        /// <summary>
        /// The handle watch topic.
        /// </summary>
        /// <returns>
        /// Returns The handle watch topic.
        /// </returns>
        private bool HandleWatchTopic()
        {
            if (this.PageContext.IsGuest)
            {
                return false;
            }

            var watchTopicId = this.GetRepository<WatchTopic>().Check(this.PageContext.PageUserID, this.PageContext.PageTopicID);

            // check if this forum is being watched by this user
            if (watchTopicId.HasValue)
            {
                // subscribed to this forum
                this.TrackTopic.Text = this.GetText("UNWATCHTOPIC");
                this.WatchTopicID.InnerText = watchTopicId.Value.ToString();

                return true;
            }

            // not subscribed
            this.WatchTopicID.InnerText = string.Empty;
            this.TrackTopic.Text = this.GetText("WATCHTOPIC");

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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The Pop Event Arguments.</param>
        private void ShareMenu_ItemClick([NotNull] object sender, [NotNull] PopEventArgs e)
        {
            var topicUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "t={0}", this.PageContext.PageTopicID);
            switch (e.Item.ToLower())
            {
                case "email":
                    this.EmailTopic_Click(sender, e);
                    break;
                case "tumblr":
                    {
                        // process message... clean html, strip html, remove bbcode, etc...
                        var tumblrMsg =
                            StringExtensions.RemoveMultipleWhitespace(
                                BBCodeHelper.StripBBCode(
                                    HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString((string)this._topic["Topic"]))));

                        var meta = this.Page.Header.FindControlType<HtmlMeta>();

                        string description = string.Empty;

                        if (meta.Any(x => x.Name.Equals("description")))
                        {
                            var descriptionMeta = meta.FirstOrDefault(x => x.Name.Equals("description"));
                            if (descriptionMeta != null)
                            {
                                description = "&description={0}".FormatWith(descriptionMeta.Content);
                            }
                        }

                        var tumblrUrl =
                            "http://www.tumblr.com/share/link?url={0}&name={1}{2}".FormatWith(
                                this.Server.UrlEncode(topicUrl), tumblrMsg, description);

                        this.Get<HttpResponseBase>().Redirect(tumblrUrl);
                    }

                    break;
                case "retweet":
                    {
                        var twitterName = this.Get<YafBoardSettings>().TwitterUserName.IsSet()
                                              ? "@{0} ".FormatWith(this.Get<YafBoardSettings>().TwitterUserName)
                                              : string.Empty;

                        // process message... clean html, strip html, remove bbcode, etc...
                        var twitterMsg =
                            StringExtensions.RemoveMultipleWhitespace(
                                BBCodeHelper.StripBBCode(
                                    HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString((string)this._topic["Topic"]))));

                        var tweetUrl =
                            "http://twitter.com/share?url={0}&text={1}".FormatWith(
                                this.Server.UrlEncode(topicUrl),
                                this.Server.UrlEncode(
                                    "RT {1}Thread: {0}".FormatWith(twitterMsg.Truncate(100), twitterName)));

                        // Send Retweet Directlly thru the Twitter API if User is Twitter User
                        if (Config.TwitterConsumerKey.IsSet() && Config.TwitterConsumerSecret.IsSet()
                            && this.Get<IYafSession>().TwitterToken.IsSet()
                            && this.Get<IYafSession>().TwitterTokenSecret.IsSet() && this.PageContext.IsTwitterUser)
                        {
                            var oAuth = new OAuthTwitter
                                {
                                    ConsumerKey = Config.TwitterConsumerKey,
                                    ConsumerSecret = Config.TwitterConsumerSecret,
                                    Token = this.Get<IYafSession>().TwitterToken,
                                    TokenSecret = this.Get<IYafSession>().TwitterTokenSecret
                                };

                            var tweets = new TweetAPI(oAuth);

                            tweets.UpdateStatus(
                                TweetAPI.ResponseFormat.json,
                                this.Server.UrlEncode(
                                    "RT {1}: {0} {2}".FormatWith(twitterMsg.Truncate(100), twitterName, topicUrl)),
                                string.Empty);
                        }
                        else
                        {
                            this.Get<HttpResponseBase>().Redirect(tweetUrl);
                        }
                    }

                    break;
                case "digg":
                    {
                        var diggUrl =
                            "http://digg.com/submit?url={0}&title={1}".FormatWith(
                                this.Server.UrlEncode(topicUrl), this.Server.UrlEncode((string)this._topic["Topic"]));

                        this.Get<HttpResponseBase>().Redirect(diggUrl);
                    }

                    break;
                case "reddit":
                    {
                        var redditUrl =
                            "http://www.reddit.com/submit?url={0}&title={1}".FormatWith(
                                this.Server.UrlEncode(topicUrl), this.Server.UrlEncode((string)this._topic["Topic"]));

                        this.Get<HttpResponseBase>().Redirect(redditUrl);
                    }

                    break;
                case "googleplus":
                    {
                        var googlePlusUrl =
                            "https://plusone.google.com/_/+1/confirm?hl=en&url={0}".FormatWith(
                                this.Server.UrlEncode(topicUrl));

                        this.Get<HttpResponseBase>().Redirect(googlePlusUrl);
                    }

                    break;
                default:
                    throw new ApplicationException(e.Item);
            }
        }

        /// <summary>
        /// The options menu_ item click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
                        YafRssFeeds.Posts.ToInt(),
                        this.PageContext.PageTopicID);
                    break;
                case "atomfeed":
                    YafBuildLink.Redirect(
                        ForumPages.rsstopic,
                        "pg={0}&t={1}&ft=1",
                        YafRssFeeds.Posts.ToInt(),
                        this.PageContext.PageTopicID);
                    break;
                default:
                    throw new ApplicationException(e.Item);
            }
        }

        /// <summary>
        /// The pager_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            this._ignoreQueryString = true;
            this.SmartScroller1.Reset();
            this.BindData();
        }

        /// <summary>
        /// The quick reply_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void QuickReply_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageContext.ForumReplyAccess
                || (this._topicFlags.IsLocked && !this.PageContext.ForumModeratorAccess))
            {
                YafBuildLink.AccessDenied();
            }

            if (this._quickReplyEditor.Text.Length <= 0)
            {
                this.PageContext.AddLoadMessage(this.GetText("EMPTY_MESSAGE"), MessageTypes.Warning);
                return;
            }

            // No need to check whitespace if they are actually posting something
            if (this.Get<YafBoardSettings>().MaxPostSize > 0
                && this._quickReplyEditor.Text.Length >= this.Get<YafBoardSettings>().MaxPostSize)
            {
                this.PageContext.AddLoadMessage(this.GetText("ISEXCEEDED"), MessageTypes.Warning);
                return;
            }

            if (((this.PageContext.IsGuest && this.Get<YafBoardSettings>().EnableCaptchaForGuests)
                 || (this.Get<YafBoardSettings>().EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded))
                && !CaptchaHelper.IsValid(this.tbCaptcha.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"), MessageTypes.Warning);
                return;
            }

            if (!(this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess)
                && this.Get<YafBoardSettings>().PostFloodDelay > 0)
            {
                if (YafContext.Current.Get<IYafSession>().LastPost
                    > DateTime.UtcNow.AddSeconds(-this.Get<YafBoardSettings>().PostFloodDelay))
                {
                    this.PageContext.AddLoadMessage(
                        this.GetTextFormatted(
                            "wait",
                            (YafContext.Current.Get<IYafSession>().LastPost
                             - DateTime.UtcNow.AddSeconds(-this.Get<YafBoardSettings>().PostFloodDelay)).Seconds), MessageTypes.Warning);
                    return;
                }
            }

            YafContext.Current.Get<IYafSession>().LastPost = DateTime.UtcNow;

            // post message...
            long messageId = 0;
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
            if (!this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess && !this.Get<YafBoardSettings>().SpamServiceType.Equals(0))
            {
                var spamChecker = new YafSpamCheck();
                string spamResult;

                // Check content for spam
                if (
                    spamChecker.CheckPostForSpam(
                        this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                        YafContext.Current.Get<HttpRequestBase>().GetUserRealIPAddress(),
                        this._quickReplyEditor.Text,
                        this.PageContext.IsGuest ? null : this.PageContext.User.Email,
                        out spamResult))
                {
                    if (this.Get<YafBoardSettings>().SpamMessageHandling.Equals(1))
                    {
                        spamApproved = false;

                        this.Get<ILogger>()
                            .Info(
                                "Spam Check detected possible SPAM ({2}) posted by User: {0}, it was flagged as unapproved post. Content was: {1}",
                                this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                                this._quickReplyEditor.Text,
                                spamResult);
                    }
                    else if (this.Get<YafBoardSettings>().SpamMessageHandling.Equals(2))
                    {
                        this.Get<ILogger>()
                            .Info(
                                "Spam Check detected possible SPAM ({2}) posted by User: {0}, post was rejected. Content was: {1}",
                                this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                                this._quickReplyEditor.Text,
                                spamResult);
                        
                        this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.Error);

                        return;
                    }
                }
                /*
                // check user for spam bot
                if (spamChecker.CheckUserForSpamBot(
                    this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                    this.PageContext.IsGuest ? null : this.PageContext.User.Email,
                    this.Get<HttpRequestBase>().GetUserRealIPAddress()))
                {
                    if (this.Get<YafBoardSettings>().SpamMessageHandling.Equals(1))
                    {
                        spamApproved = false;

                        this.Get<ILogger>()
                            .Info(
                                "Spam Check detected possible SPAM posted by User: {0}, it was flagged as unapproved post. Content was: {1}",
                                this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                                this._quickReplyEditor.Text);
                    }
                    else if (this.Get<YafBoardSettings>().SpamMessageHandling.Equals(2))
                    {
                        this.Get<ILogger>()
                           .Info(
                               "Spam Check detected possible SPAM posted by User: {0}, post was rejected. Content was: {1}",
                               this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                               this._quickReplyEditor.Text);

                        this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.Error);

                        return;
                    }
                }*/
            }

            // If Forum is Moderated
            if (isForumModerated)
            {
                spamApproved = false;
            }

            // Bypass Approval if Admin or Moderator
            if (this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess)
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
                    this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                    null,
                    replyTo,
                    tFlags.BitValue,
                    ref messageId))
            {
                topicID = 0;
            }

            // Check to see if the user has enabled "auto watch topic" option in his/her profile.
            if (this.PageContext.CurrentUserData.AutoWatchTopics)
            {
                var watchTopicId = this.GetRepository<WatchTopic>().Check(this.PageContext.PageUserID, this.PageContext.PageTopicID);

                if (!watchTopicId.HasValue)
                {
                    // subscribe to this topic
                    this.GetRepository<WatchTopic>().Add(this.PageContext.PageUserID, this.PageContext.PageTopicID);
                }
            }

            bool bApproved = false;

            using (DataTable dt = LegacyDb.message_list(messageId))
            {
                foreach (DataRow row in dt.Rows)
                {
                    bApproved = ((int)row["Flags"] & 16) == 16;
                }
            }

            if (bApproved)
            {
                // send new post notification to users watching this topic/forum
                this.Get<ISendNotification>().ToWatchingUsers(messageId.ToType<int>());

                if (Config.IsDotNetNuke)
                {
                    this.Get<IActivityStream>()
                           .AddReplyToStream(
                               this.PageContext.PageForumID,
                               this.PageContext.PageTopicID,
                               messageId.ToType<int>(),
                               this.PageContext.PageTopicName,
                               msg);
                }

                // redirect to newly posted message
                YafBuildLink.Redirect(ForumPages.posts, "m={0}&#post{0}", messageId);
            }
            else
            {
                if (this.Get<YafBoardSettings>().EmailModeratorsOnModeratedPost)
                {
                    // not approved, notifiy moderators
                    this.Get<ISendNotification>().ToModeratorsThatMessageNeedsApproval(
                        this.PageContext.PageForumID, (int)messageId);
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ViewMenu_ItemClick([NotNull] object sender, [NotNull] PopEventArgs e)
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void posts_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            bool isWatched = this.HandleWatchTopic();

            // share menu...
            this.ShareMenu.Visible = this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ShowShareTopicTo);
            this.ShareLink.Visible = this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ShowShareTopicTo);

            this.ShareLink.ToolTip = this.GetText("SHARE_TOOLTIP");

            if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ShowShareTopicTo))
            {
                var topicUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.posts, true, "t={0}", this.PageContext.PageTopicID);
                if (this.Get<YafBoardSettings>().AllowEmailTopic)
                {
                    this.ShareMenu.AddPostBackItem(
                        "email", this.GetText("EMAILTOPIC"), this.Get<ITheme>().GetItem("ICONS", "EMAIL"));
                }

                this.ShareMenu.AddClientScriptItem(
                    this.GetText("LINKBACK_TOPIC"),
                    "prompt('{0}','{1}');return false;".FormatWith(this.GetText("LINKBACK_TOPIC_PROMT"), topicUrl),
                    this.Get<ITheme>().GetItem("ICONS", "LINKBACK"));
                this.ShareMenu.AddPostBackItem(
                    "retweet", this.GetText("RETWEET_TOPIC"), this.Get<ITheme>().GetItem("ICONS", "TWITTER"));
                this.ShareMenu.AddPostBackItem(
                    "googleplus", this.GetText("GOOGLEPLUS_TOPIC"), this.Get<ITheme>().GetItem("ICONS", "GOOGLEPLUS"));
                /* this.ShareMenu.AddPostBackItem(
                 * "facebook", this.GetText("FACEBOOK_TOPIC"), this.Get<ITheme>().GetItem("ICONS", "FACEBOOK"));*/

                var facebookUrl =
                    "http://www.facebook.com/plugins/like.php?href={0}".FormatWith(
                        this.Server.UrlEncode(topicUrl), this.Server.UrlEncode((string)this._topic["Topic"]));

                this.ShareMenu.AddClientScriptItem(
                    this.GetText("FACEBOOK_TOPIC"),
                    @"window.open('{0}','Facebook','width=300,height=200,resizable=yes');".FormatWith(facebookUrl),
                    this.Get<ITheme>().GetItem("ICONS", "FACEBOOK"));

                if (Config.FacebookAPIKey.IsSet())
                {
                    this.ShareMenu.AddClientScriptItem(
                        this.GetText("FACEBOOK_SHARE_TOPIC"),
                        "postToFacebook()",
                        this.Get<ITheme>().GetItem("ICONS", "FACEBOOK"));
                }

                this.ShareMenu.AddPostBackItem(
                    "digg", this.GetText("DIGG_TOPIC"), this.Get<ITheme>().GetItem("ICONS", "DIGG"));
                this.ShareMenu.AddPostBackItem(
                    "reddit", this.GetText("REDDIT_TOPIC"), this.Get<ITheme>().GetItem("ICONS", "REDDIT"));

                this.ShareMenu.AddPostBackItem(
                    "tumblr", this.GetText("TUMBLR_TOPIC"), this.Get<ITheme>().GetItem("ICONS", "TUMBLR"));
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

            this.OptionsMenu.AddPostBackItem(
                "print", this.GetText("PRINTTOPIC"), this.Get<ITheme>().GetItem("ICONS", "PRINT"));

            if (this.Get<YafBoardSettings>().ShowAtomLink
                && this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().PostsFeedAccess))
            {
                this.OptionsMenu.AddPostBackItem(
                    "atomfeed", this.GetText("ATOMTOPIC"), this.Get<ITheme>().GetItem("ICONS", "ATOMFEED"));
            }

            if (this.Get<YafBoardSettings>().ShowRSSLink
                && this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().PostsFeedAccess))
            {
                this.OptionsMenu.AddPostBackItem(
                    "rssfeed", this.GetText("RSSTOPIC"), this.Get<ITheme>().GetItem("ICONS", "RSSFEED"));
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