/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Text;
    using System.Web;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Core.Services.Auth;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// DisplayPost Class.
    /// </summary>
    public partial class DisplayPost : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The current Post Data for this post.
        /// </summary>
        private PostDataHelperWrapper _postDataHelperWrapper;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets Current Page Index.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        ///   Gets or sets the DataRow.
        /// </summary>
        [CanBeNull]
        public DataRow DataRow { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether IsAlt.
        /// </summary>
        public bool IsAlt { get; set; }

        /// <summary>
        ///   Gets a value indicating whether IsGuest.
        /// </summary>
        public bool IsGuest
        {
            get
            {
                return this.PostData == null || UserMembershipHelper.IsGuestUser(this.PostData.UserId);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether IsThreaded.
        /// </summary>
        public bool IsThreaded { get; set; }

        /// <summary>
        ///   Gets or sets Post Count.
        /// </summary>
        public int PostCount { get; set; }

        /// <summary>
        ///   Gets Post Data helper functions.
        /// </summary>
        public PostDataHelperWrapper PostData
        {
            get
            {
                if (this._postDataHelperWrapper == null && this.DataRow != null)
                {
                    this._postDataHelperWrapper = new PostDataHelperWrapper(this.DataRow);
                }

                return this._postDataHelperWrapper;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the dvThanksInfo section.
        /// </summary>
        /// <param name="rawStr">
        /// The raw Str. 
        /// </param>
        /// <returns>
        /// The format thanks info. 
        /// </returns>
        [NotNull]
        protected string FormatThanksInfo([NotNull] string rawStr)
        {
            var sb = new StringBuilder();

            bool showDate = this.Get<YafBoardSettings>().ShowThanksDate;

            // Extract all user IDs, usernames and (If enabled thanks dates) related to this message.
            foreach (var chunk in rawStr.Split(','))
            {
                var subChunks = chunk.Split('|');

                int userId = int.Parse(subChunks[0]);
                DateTime thanksDate = DateTime.Parse(subChunks[1]);

                if (sb.Length > 0)
                {
                    sb.Append(",&nbsp;");
                }

                // Get the username related to this User ID
                string displayName = this.Get<IUserDisplayName>().GetName(userId);

                sb.AppendFormat(
                    @"<a id=""{0}"" href=""{1}""><u>{2}</u></a>",
                    userId,
                    YafBuildLink.GetLink(ForumPages.profile, "u={0}", userId),
                    this.Get<HttpServerUtilityBase>().HtmlEncode(displayName));

                // If showing thanks date is enabled, add it to the formatted string.
                if (showDate)
                {
                    sb.AppendFormat(
                        @" {0}",
                        this.GetText("DEFAULT", "ONDATE").FormatWith(this.Get<IDateTime>().FormatDateShort(thanksDate)));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// The get indent cell.
        /// </summary>
        /// <returns>
        /// Returns indent cell. 
        /// </returns>
        protected string GetIndentCell()
        {
            if (!this.IsThreaded)
            {
                return string.Empty;
            }

            var indent = (int)this.DataRow["Indent"];

            if (indent > 0)
            {
                return
                    @"<td rowspan=""4"" width=""1%""><img src=""{1}images/spacer.gif"" width=""{0}"" height=""2"" alt=""""/></td>"
                        .FormatWith(indent * 32, YafForumInfo.ForumClientFileRoot);
            }

            return string.Empty;
        }

        /// <summary>
        /// Get Row Span
        /// </summary>
        /// <returns>
        /// Returns the row Span value 
        /// </returns>
        [NotNull]
        protected string GetRowSpan()
        {
            if (this.DataRow != null && this.Get<YafBoardSettings>().AllowSignatures &&
                this.DataRow["Signature"] != DBNull.Value &&
                this.DataRow["Signature"].ToString().ToLower() != "<p>&nbsp;</p>" &&
                this.DataRow["Signature"].ToString().Trim().Length > 0)
            {
                return "rowspan=\"2\"";
            }

            return string.Empty;
        }

        /// <summary>
        /// The get indent span.
        /// </summary>
        /// <returns>
        /// Returns the indent span. 
        /// </returns>
        [NotNull]
        protected string GetIndentSpan()
        {
            return !this.IsThreaded || (int)this.DataRow["Indent"] == 0 ? "2" : "1";
        }

        /// <summary>
        /// The get post class.
        /// </summary>
        /// <returns>
        /// Returns the post class. 
        /// </returns>
        [NotNull]
        protected string GetPostClass()
        {
            return this.IsAlt ? "post_alt" : "post";
        }

        // Prevents a high user box when displaying a deleted post.

        /// <summary>
        /// The get user box height.
        /// </summary>
        /// <returns>
        /// Returns a fake user box height (Not the Real one). 
        /// </returns>
        [NotNull]
        protected string GetUserBoxHeight()
        {
            return this.PostData.PostDeleted ? "0" : "100";
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e. 
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.PreRender += this.DisplayPost_PreRender;
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
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Set column span for layout depending on is it TreeView or not.
            this.NameCell.ColSpan = int.Parse(this.GetIndentSpan());

            if (this.IsGuest)
            {
                return;
            }

            // Set up popup menu if it's not a guest.
            this.PopMenu1.Visible = true;
            this.SetupPopupMenu();
        }

        /// <summary>
        /// Adds the user reputation.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data. 
        /// </param>
        protected void AddUserReputation(object sender, EventArgs e)
        {
            if (!YafReputation.CheckIfAllowReputationVoting(this.DataRow["ReputationVoteDate"]))
            {
                return;
            }

            this.AddReputation.Visible = false;
            this.RemoveReputation.Visible = false;

            LegacyDb.user_addpoints(this.PostData.UserId, this.PageContext.PageUserID, 1);

            this.DataRow["ReputationVoteDate"] = DateTime.UtcNow;

            // Reload UserBox
            this.PageContext.CurrentForumPage.PageCache[Constants.Cache.UserBoxes] = null;

            this.DataRow["Points"] = this.DataRow["Points"].ToType<int>() + 1;
            this.UserBox1.PageCache = null;

            this.PageContext.AddLoadMessage(
                this.GetTextFormatted(
                    "REP_VOTE_UP_MSG",
                    this.Get<HttpServerUtilityBase>().HtmlEncode(
                        this.DataRow[this.Get<YafBoardSettings>().EnableDisplayName ? "DisplayName" : "UserName"].ToString())),
                MessageTypes.Success);

            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "reputationprogressjs",
                JavaScriptBlocks.ReputationProgressChangeJs(
                    YafReputation.GenerateReputationBar(this.DataRow["Points"].ToType<int>(), this.PostData.UserId),
                    this.PostData.UserId.ToString()));
        }

        /// <summary>
        /// Removes the user reputation.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data. 
        /// </param>
        protected void RemoveUserReputation(object sender, EventArgs e)
        {
            if (!YafReputation.CheckIfAllowReputationVoting(this.DataRow["ReputationVoteDate"]))
            {
                return;
            }

            this.AddReputation.Visible = false;
            this.RemoveReputation.Visible = false;

            LegacyDb.user_removepoints(this.PostData.UserId, this.PageContext.PageUserID, 1);

            this.DataRow["ReputationVoteDate"] = DateTime.UtcNow;

            // Reload UserBox
            this.PageContext.CurrentForumPage.PageCache[Constants.Cache.UserBoxes] = null;

            this.DataRow["Points"] = this.DataRow["Points"].ToType<int>() - 1;
            this.UserBox1.PageCache = null;

            this.PageContext.AddLoadMessage(
                this.GetTextFormatted(
                   "REP_VOTE_DOWN_MSG",
                   this.Get<HttpServerUtilityBase>().HtmlEncode(
                        this.DataRow[this.Get<YafBoardSettings>().EnableDisplayName ? "DisplayName" : "UserName"].ToString())),
                MessageTypes.Success);

            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "reputationprogressjs",
                JavaScriptBlocks.ReputationProgressChangeJs(
                    YafReputation.GenerateReputationBar(this.DataRow["Points"].ToType<int>(), this.PostData.UserId),
                    this.PostData.UserId.ToString()));
        }

        /// <summary>
        /// Retweets Message thru the Twitter API
        /// </summary>
        /// <param name="sender">
        /// The source of the event. 
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data. 
        /// </param>
        protected void Retweet_Click(object sender, EventArgs e)
        {
            var twitterName = this.Get<YafBoardSettings>().TwitterUserName.IsSet()
                                  ? "@{0} ".FormatWith(this.Get<YafBoardSettings>().TwitterUserName)
                                  : string.Empty;

            // process message... clean html, strip html, remove bbcode, etc...
            var twitterMsg =
                StringExtensions.RemoveMultipleWhitespace(
                    BBCodeHelper.StripBBCode(
                        HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString((string)this.DataRow["Message"]))));

            var topicUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "m={0}#post{0}", this.DataRow["MessageID"]);

            // Send Retweet Directlly thru the Twitter API if User is Twitter User
            if (Config.TwitterConsumerKey.IsSet() && Config.TwitterConsumerSecret.IsSet() &&
                this.Get<IYafSession>().TwitterToken.IsSet() && this.Get<IYafSession>().TwitterTokenSecret.IsSet() &&
                this.Get<IYafSession>().TwitterTokenSecret.IsSet() && this.PageContext.IsTwitterUser)
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
                    this.Server.UrlEncode("RT {1}: {0} {2}".FormatWith(twitterMsg.Truncate(100), twitterName, topicUrl)),
                    string.Empty);
            }
            else
            {
                this.Get<HttpResponseBase>().Redirect(
                    "http://twitter.com/share?url={0}&text={1}".FormatWith(
                        this.Server.UrlEncode(this.Get<HttpRequestBase>().Url.ToString()),
                        this.Server.UrlEncode(
                            "RT {1}: {0} {2}".FormatWith(twitterMsg.Truncate(100), twitterName, topicUrl))));
            }
        }

        private void ShowIPInfo()
        {
            // Display admin/moderator only info
            if (this.PageContext.IsAdmin ||
                (this.Get<YafBoardSettings>().AllowModeratorsViewIPs && this.PageContext.ForumModeratorAccess))
            {
                // We should show IP
                this.IPSpan1.Visible = true;
                var ip = IPHelper.GetIp4Address(this.PostData.DataRow["IP"].ToString());
                this.IPLink1.HRef = this.Get<YafBoardSettings>().IPInfoPageURL.FormatWith(ip);
                this.IPLink1.Title = this.GetText("COMMON", "TT_IPDETAILS");
                this.IPLink1.InnerText = this.HtmlEncode(ip);
            }

            
        }

        /// <summary>
        /// Setup the popup menu.
        /// </summary>
        private void SetupPopupMenu()
        {
            this.PopMenu1.ItemClick += this.PopMenu1_ItemClick;
            this.PopMenu1.AddPostBackItem("userprofile", this.GetText("POSTS", "USERPROFILE"));

            this.PopMenu1.AddPostBackItem("lastposts", this.GetText("PROFILE", "SEARCHUSER"));

            if (this.Get<YafBoardSettings>().EnableThanksMod)
            {
                this.PopMenu1.AddPostBackItem("viewthanks", this.GetText("VIEWTHANKS", "TITLE"));
            }

            if (this.PageContext.IsAdmin)
            {
                this.PopMenu1.AddPostBackItem("edituser", this.GetText("POSTS", "EDITUSER"));
            }

            if (!this.PageContext.IsGuest)
            {
                if (this.Get<IUserIgnored>().IsIgnored(this.PostData.UserId))
                {
                    this.PopMenu1.AddPostBackItem("toggleuserposts_show", this.GetText("POSTS", "TOGGLEUSERPOSTS_SHOW"));
                }
                else
                {
                    this.PopMenu1.AddPostBackItem("toggleuserposts_hide", this.GetText("POSTS", "TOGGLEUSERPOSTS_HIDE"));
                }
            }

            if (this.Get<YafBoardSettings>().EnableBuddyList &&
                this.PageContext.PageUserID != (int)this.DataRow["UserID"])
            {
                // Should we add the "Add Buddy" item?
                if (!this.Get<IBuddy>().IsBuddy((int)this.DataRow["UserID"], false) && !this.PageContext.IsGuest)
                {
                    this.PopMenu1.AddPostBackItem("addbuddy", this.GetText("BUDDY", "ADDBUDDY"));
                }
                else if (this.Get<IBuddy>().IsBuddy((int)this.DataRow["UserID"], true) && !this.PageContext.IsGuest)
                {
                    // Are the users approved buddies? Add the "Remove buddy" item.
                    this.PopMenu1.AddClientScriptItemWithPostback(
                        this.GetText("BUDDY", "REMOVEBUDDY"),
                        "removebuddy",
                        "if (confirm('{0}')) {1}".FormatWith(
                            this.GetText("CP_EDITBUDDIES", "NOTIFICATION_REMOVE"), "{postbackcode}"));
                }
            }

            this.PopMenu1.Attach(this.UserProfileLink);
        }

        /// <summary>
        /// The display post_ pre render.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        private void DisplayPost_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.PageContext.IsGuest)
            {
                this.PostFooter.TogglePost.Visible = false;
            }
            else if (this.Get<IUserIgnored>().IsIgnored(this.PostData.UserId))
            {
                this.panMessage.Attributes["style"] = "display:none";
                this.PostFooter.TogglePost.Visible = true;
                this.PostFooter.TogglePost.Attributes["onclick"] =
                    "toggleMessage('{0}'); return false;".FormatWith(this.panMessage.ClientID);
            }
            else if (!this.Get<IUserIgnored>().IsIgnored(this.PostData.UserId))
            {
                this.panMessage.Attributes["style"] = "display:block";
                this.panMessage.Visible = true;
            }

            this.Retweet.Visible = this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ShowRetweetMessageTo);

            this.Attach.Visible = !this.PostData.PostDeleted && this.PostData.CanAttach && !this.PostData.IsLocked;
            this.Attach.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.attachments, "m={0}", this.PostData.MessageId);
            this.Edit.Visible = !this.PostData.PostDeleted && this.PostData.CanEditPost && !this.PostData.IsLocked;
            this.Edit.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.postmessage, "m={0}", this.PostData.MessageId);
            this.MovePost.Visible = this.PageContext.ForumModeratorAccess && !this.PostData.IsLocked;
            this.MovePost.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.movemessage, "m={0}", this.PostData.MessageId);
            this.Delete.Visible = !this.PostData.PostDeleted && this.PostData.CanDeletePost && !this.PostData.IsLocked;
            this.Delete.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.deletemessage, "m={0}&action=delete", this.PostData.MessageId);
            this.UnDelete.Visible = this.PostData.CanUnDeletePost && !this.PostData.IsLocked;
            this.UnDelete.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.deletemessage, "m={0}&action=undelete", this.PostData.MessageId);

            this.Quote.Visible = !this.PostData.PostDeleted && this.PostData.CanReply && !this.PostData.IsLocked;
            this.MultiQuote.Visible = !this.PostData.PostDeleted && this.PostData.CanReply && !this.PostData.IsLocked;

            this.Quote.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.postmessage,
                "t={0}&f={1}&q={2}&page={3}",
                this.PageContext.PageTopicID,
                this.PageContext.PageForumID,
                this.PostData.MessageId,
                this.CurrentPage);

            // setup jQuery and YAF JS...
            YafContext.Current.PageElements.RegisterJQuery();
            YafContext.Current.PageElements.RegisterJQueryUI();

            YafContext.Current.PageElements.RegisterJsResourceInclude("yafjs", "js/yaf.js");
            YafContext.Current.PageElements.RegisterJsBlock("toggleMessageJs", JavaScriptBlocks.ToggleMessageJs);

            YafContext.Current.PageElements.RegisterJsResourceInclude("yafPageMethodjs", "js/jquery.pagemethod.js");

            // Setup Ceebox js
            YafContext.Current.PageElements.RegisterJsResourceInclude("ceeboxjs", "js/jquery.ceebox.js");
            YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.ceebox.css");
            YafContext.Current.PageElements.RegisterJsBlock("ceeboxloadjs", JavaScriptBlocks.CeeBoxLoadJs);

            // Setup Syntax Highlight JS
            YafContext.Current.PageElements.RegisterJsResourceInclude(
                "syntaxhighlighter", "js/jquery.syntaxhighligher.js");
            YafContext.Current.PageElements.RegisterCssIncludeResource("css/jquery.syntaxhighligher.css");
            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "syntaxhighlighterjs", JavaScriptBlocks.SyntaxHighlightLoadJs);

            if (this.MultiQuote.Visible)
            {
                this.MultiQuote.Attributes.Add("onclick", "handleMultiQuoteButton(this, '{0}')".FormatWith(this.PostData.MessageId));

                YafContext.Current.PageElements.RegisterJsBlockStartup(
                    "MultiQuoteButtonJs", JavaScriptBlocks.MultiQuoteButtonJs);
                YafContext.Current.PageElements.RegisterJsBlockStartup(
                  "MultiQuoteCallbackSuccessJS", JavaScriptBlocks.MultiQuoteCallbackSuccessJS);

                this.MultiQuote.Text = this.GetText("BUTTON_MULTI_QUOTE");
                this.MultiQuote.ToolTip = this.GetText("BUTTON_MULTI_QUOTE_TT");
            }

            if (this.Get<YafBoardSettings>().EnableUserReputation)
            {
                // Setup UserBox Reputation Script Block
                YafContext.Current.PageElements.RegisterJsBlockStartup(
                    "reputationprogressjs", JavaScriptBlocks.RepuatationProgressLoadJs);

                this.AddReputationControls();
            }

            // Is User Suspended
            var suspended = this.DataRow["Suspended"].ToType<DateTime?>();

            if (suspended.HasValue && suspended.Value > DateTime.UtcNow)
            {
                this.ThemeImgSuspended.LocalizedTitle =
                    this.GetText("POSTS", "USERSUSPENDED").FormatWith(
                        this.Get<IDateTime>().FormatDateTimeShort(suspended.Value));

                this.ThemeImgSuspended.Visible = true;
                this.OnlineStatusImage.Visible = false;
            }

            YafContext.Current.PageElements.RegisterJsBlockStartup("asynchCallFailedJs", "function CallFailed(res){ alert('Error Occurred'); }");
            
            this.FormatThanksRow();

            this.ShowIPInfo();
        }

        /// <summary>
        /// Add Reputation Controls to the User PopMenu
        /// </summary>
        private void AddReputationControls()
        {
            if (this.PageContext.PageUserID != this.DataRow["UserID"].ToType<int>() &&
                this.Get<YafBoardSettings>().EnableUserReputation && !this.IsGuest && !this.PageContext.IsGuest)
            {
                if (YafReputation.CheckIfAllowReputationVoting(this.DataRow["ReputationVoteDate"]))
                {
                    // Check if the User matches minimal requirements for voting up
                    if (this.PageContext.Reputation >= this.Get<YafBoardSettings>().ReputationMinUpVoting)
                    {
                        this.AddReputation.Visible = true;
                    }

                    // Check if the User matches minimal requirements for voting down
                    if (this.PageContext.Reputation >= this.Get<YafBoardSettings>().ReputationMinDownVoting)
                    {
                        // Check if the Value is 0 or Bellow
                        if (!this.Get<YafBoardSettings>().ReputationAllowNegative &&
                            this.DataRow["Points"].ToType<int>() > 0 ||
                            this.Get<YafBoardSettings>().ReputationAllowNegative)
                        {
                            this.RemoveReputation.Visible = true;
                        }
                    }
                }
            }
            else
            {
                this.AddReputation.Visible = false;
                this.RemoveReputation.Visible = false;
            }
        }

        /// <summary>
        /// Do thanks row formatting.
        /// </summary>
        private void FormatThanksRow()
        {
            if (!this.Get<YafBoardSettings>().EnableThanksMod)
            {
                return;
            }

            // Register Javascript
            const string AddThankBoxHTML =
                "'<a class=\"yaflittlebutton\" href=\"javascript:addThanks(' + res.d.MessageID + ');\" onclick=\"jQuery(this).blur();\" title=' + res.d.Title + '><span>' + res.d.Text + '</span></a>'";

            const string RemoveThankBoxHTML =
                "'<a class=\"yaflittlebutton\" href=\"javascript:removeThanks(' + res.d.MessageID + ');\" onclick=\"jQuery(this).blur();\" title=' + res.d.Title + '><span>' + res.d.Text + '</span></a>'";

            var thanksJs = JavaScriptBlocks.AddThanksJs(RemoveThankBoxHTML) + Environment.NewLine + JavaScriptBlocks.RemoveThanksJs(AddThankBoxHTML);

            YafContext.Current.PageElements.RegisterJsBlockStartup("ThanksJs", thanksJs);

            this.Thank.Visible = this.PostData.CanThankPost && !this.PageContext.IsGuest &&
                                 this.Get<YafBoardSettings>().EnableThanksMod;

            if (Convert.ToBoolean(this.DataRow["IsThankedByUser"]))
            {
                this.Thank.NavigateUrl = "javascript:removeThanks({0});".FormatWith(this.DataRow["MessageID"]);
                this.Thank.TextLocalizedTag = "BUTTON_THANKSDELETE";
                this.Thank.TitleLocalizedTag = "BUTTON_THANKSDELETE_TT";
            }
            else
            {
                this.Thank.NavigateUrl = "javascript:addThanks({0});".FormatWith(this.DataRow["MessageID"]);
                this.Thank.TextLocalizedTag = "BUTTON_THANKS";
                this.Thank.TitleLocalizedTag = "BUTTON_THANKS_TT";
            }

            var thanksNumber = this.DataRow["MessageThanksNumber"].ToType<int>();

            if (thanksNumber == 0)
            {
                return;
            }

            var username =
                this.HtmlEncode(
                    this.Get<YafBoardSettings>().EnableDisplayName
                        ? UserMembershipHelper.GetDisplayNameFromID(this.PostData.UserId)
                        : UserMembershipHelper.GetUserNameFromID(this.PostData.UserId));

            var thanksLabelText = thanksNumber == 1
                                  ? this.Get<ILocalization>().GetText("THANKSINFOSINGLE").FormatWith(username)
                                  : this.Get<ILocalization>().GetText("THANKSINFO").FormatWith(thanksNumber, username);

            this.ThanksDataLiteral.Text =
                "<img id=\"ThanksInfoImage{0}\" src=\"{1}\" alt=\"thanks\"  runat=\"server\" />&nbsp;{2}".FormatWith(
                    this.DataRow["MessageID"],
                    this.Get<ITheme>().GetItem("ICONS", "THANKSINFOLIST_IMAGE"),
                    thanksLabelText);

            this.ThanksDataLiteral.Visible = true;

            this.thanksDataExtendedLiteral.Text = this.FormatThanksInfo(this.DataRow["ThanksInfo"].ToString());
            this.thanksDataExtendedLiteral.Visible = true;
        }

        /// <summary>
        /// The pop menu 1_ item click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="YAF.Controls.PopEventArgs"/> instance containing the event data.</param>
        private void PopMenu1_ItemClick([NotNull] object sender, [NotNull] PopEventArgs e)
        {
            switch (e.Item)
            {
                case "userprofile":
                    YafBuildLink.Redirect(ForumPages.profile, "u={0}", this.PostData.UserId);
                    break;
                case "lastposts":
                    YafBuildLink.Redirect(
                        ForumPages.search,
                        "postedby={0}",
                        this.Get<YafBoardSettings>().EnableDisplayName ? this.DataRow["DisplayName"] : this.DataRow["UserName"]);
                    break;
                case "addbuddy":
                    this.PopMenu1.RemovePostBackItem("addbuddy");
                    string[] strBuddyRequest = this.Get<IBuddy>().AddRequest(this.PostData.UserId);
                    if (Convert.ToBoolean(strBuddyRequest[1]))
                    {
                        this.PageContext.AddLoadMessage(
                            this.GetTextFormatted("NOTIFICATION_BUDDYAPPROVED_MUTUAL", strBuddyRequest[0]), MessageTypes.Success);

                        this.PopMenu1.AddClientScriptItemWithPostback(
                            this.GetText("BUDDY", "REMOVEBUDDY"),
                            "removebuddy",
                            "if (confirm('{0}')) {1}".FormatWith(
                                this.GetText("CP_EDITBUDDIES", "NOTIFICATION_REMOVE"), "{postbackcode}"));
                    }
                    else
                    {
                        this.PageContext.AddLoadMessage(this.GetText("NOTIFICATION_BUDDYREQUEST"));
                    }

                    break;
                case "removebuddy":
                    {
                        this.PopMenu1.RemovePostBackItem("removebuddy");
                        this.PopMenu1.AddPostBackItem("addbuddy", this.GetText("BUDDY", "ADDBUDDY"));
                        this.PageContext.AddLoadMessage(
                            this.GetTextFormatted(
                                "REMOVEBUDDY_NOTIFICATION", this.Get<IBuddy>().Remove(this.PostData.UserId)),
                            MessageTypes.Success);
                        break;
                    }

                case "edituser":
                    YafBuildLink.Redirect(ForumPages.admin_edituser, "u={0}", this.PostData.UserId);
                    break;
                case "toggleuserposts_show":
                    this.Get<IUserIgnored>().RemoveIgnored(this.PostData.UserId);
                    this.Response.Redirect(this.Request.RawUrl);
                    break;
                case "toggleuserposts_hide":
                    this.Get<IUserIgnored>().AddIgnored(this.PostData.UserId);
                    this.Response.Redirect(this.Request.RawUrl);
                    break;
                case "viewthanks":
                    YafBuildLink.Redirect(ForumPages.viewthanks, "u={0}", this.PostData.UserId);
                    break;
            }
        }

        #endregion
    }
}